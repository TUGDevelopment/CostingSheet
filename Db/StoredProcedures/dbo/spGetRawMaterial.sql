-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
 
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetRawMaterial]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max), 
	@RequestNo nvarchar(max),
	@from datetime,@to datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Company nvarchar(max)='102',@RequestNo nvarchar(max)=214
	--select * from TransTechnical
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	--declare @form datetime,@to datetime
	--select @form=RequestDate,@to=RequireDate from TransTechnical where ID=@RequestNo

	select * into #temp from(
	select Material,[Description] from MasPricePolicy where (@from between cast([From] as date) and cast ([To] as date) 
	Or @to between cast([From] as date) and cast ([To] as date)) union
	select Material,[Description] from MasPriceStd where (@from between cast([From] as date) and cast ([To] as date)
	Or @to between cast([From] as date) and cast ([To] as date)) and Company=@Company)#a where substring(Material,1,1) not in ('5')
	--update MasPricePolicy set [From]='20180131'
	--Select Company,RawMaterial,count(RawMaterial)'XX' into #temp From MasYield a 
	--Where Company=@Company and substring(a.Material,1,1) not in ('5') group by Company,RawMaterial
	--insert into #temp select @Company,'',''
	--Select Company,RawMaterial,
	--(select top 1 [Description] from MasYield a where a.Company=c.Company and a.RawMaterial=c.RawMaterial)'Description'
	-- From #temp c
	--where RawMaterial ='11M420400001'
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select distinct Material as 'RawMaterial' into #table from #temp order by Material

	select RawMaterial as 'ID',(select top 1 [Description] from #temp b where b.Material=#table.RawMaterial) as [Description]
	from #table order by RawMaterial
END


go

