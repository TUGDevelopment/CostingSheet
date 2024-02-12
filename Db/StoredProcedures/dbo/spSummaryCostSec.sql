-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE  [dbo].[spSummaryCostSec]
	-- Add the parameters for the stored procedure here
	@UserNo nvarchar(max),
	@ID nvarchar(max)
AS
BEGIN

--declare @Keyword nvarchar(max)='[CreateOn] Between ''2020-03-01'' and ''2020-03-31''',@UserNo nvarchar(max)='FO5910155',@ID nvarchar(max)='12580'
	If Object_ID('tempdb..#TransCosting')  is not null  drop table #TransCosting
	select * into #TransCosting from TransCosting where RequestNo in (select value from dbo.FNC_SPLIT(@ID,',')) 
	and LOWER(component) in ('secondary packaging','primary packaging') 
	--select * from #TransCosting
	If Object_ID('tempdb..#tempClass')  is not null  drop table #tempClass
	select a.RequestNo,a.Formula,a.Amount,b.Description as 'Name' into #tempClass from #TransCosting a left join MaterialClass b on substring(a.SAPMaterial,2,1)=b.Id 
	
	
 
	declare @cols nvarchar(max),@query nvarchar(max)
	SET @cols = STUFF((SELECT distinct ',' + QUOTENAME(c.Name) 
            FROM #tempClass c
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
set @query = N'SELECT requestno,Formula,' + @cols + ' from 
            (
                select requestno,Formula,Amount,Name

                from #tempClass
           ) x
            pivot 
            (
                 min(Amount)
                for Name in (' + @cols + ')
            ) p '

execute (@Query)
END
go

