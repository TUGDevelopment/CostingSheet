-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date, ,>
-- Description:	<Description, ,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_getcd]
(
	-- Add the parameters for the function here
	@ID nvarchar(max)
)
RETURNS nvarchar(max)
AS
BEGIN
 declare @temp  table(empid nvarchar(max),Sublevel nvarchar(max))
 declare @Requester nvarchar(max)--,@ID int=297
 set @Requester =(select CONCAT(Requester,',',isnull(Assignee,''))'Requester' from TransTechnical where ID=@ID)
 insert into @temp
 select b.EmpId,b.Sublevel from dbo.FNC_SPLIT(@Requester,',') a left join MasApprovAssign b on b.EmpId=a.value where value<>''
 --select * from #temp

 DECLARE @cols AS NVARCHAR(MAX),
		@query  AS NVARCHAR(MAX);
 SET @cols = STUFF((SELECT distinct ',' + (c.EmpId) --QUOTENAME(c.EmpId) 
            FROM @temp c where c.Sublevel in (3,4)
            FOR XML PATH(''), TYPE
            ).value('.', 'NVARCHAR(MAX)') 
        ,1,1,'')
	--print @cols
	-- Return the result of the function
	RETURN @cols

END
go

