-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spselectPackage]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max),
	@Company nvarchar(max),
	@type nvarchar(max),
	@material nvarchar(max),
	@from datetime,@to datetime
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='306',@Company nvarchar(max)='103',@type nvarchar(max)='0'
	SET NOCOUNT ON;
	declare @customer nvarchar(max),@valid datetime,@diff int
	--select ID,SAPMaterial,Name,Price,Currency,Unit from MasPrice where Company=@Company and SAPMaterial like '5%' and substring(SAPMaterial,2,1) in ('F')
	select top 1 --@diff=((month(RequireDate) - month(RequestDate))+1), 
	
	--@from=cast(RequestDate as date),@to=cast(RequireDate as date),
	@customer=CustomPrice from TransTechnical t where t.ID=@Id  
	set @diff= (select DATEDIFF(month, @from,@to))
	set @valid=@from
    -- Insert statements for procedure here
	set @from =(case when @customer=1 then (SELECT DATEADD(yy, DATEDIFF(yy, 0, @from), 0)) else @from end)
	--set @to = (case when @customer=1 then @from else @to end)
	declare @lastyear date =(select dateadd(ms,-2,dateadd(year,0,dateadd(year,datediff(year,0,@from)+1,0))) )
	set @to = (case when @to>@lastyear then @lastyear else @to end)

 	select Id,Company,Material,[Description],case when @diff < 7 then PriceStd1
	else PriceStd2 end as 'Price' ,Currency,Unit,[From],[To] from MasPriceStd  
	where company=@company and Material like '5%' and ISNUMERIC(substring(Material,2,1))= @type
	and Material=@material
	--and cast([From] as date) <= @from and cast ([To] as date)>=@to
	and (@from between cast([From] as date) and cast ([To] as date) Or @to between cast([From] as date) and cast ([To] as date))
END

go

