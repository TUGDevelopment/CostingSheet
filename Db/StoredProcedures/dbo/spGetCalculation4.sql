-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create Procedure [dbo].[spGetCalculation4]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Param nvarchar(max),
	@Option nvarchar(max)
AS
BEGIN
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	--declare @ID nvarchar(max)=0,@Param nvarchar(max)=4304,@Option nvarchar(max)=1
	declare @table table (ID int,ExchangeRate nvarchar(max),Packaging nvarchar(max))
		declare @strSQL nvarchar(max)='select ID,ExchangeRate,Packaging from TransCostingHeader where StatusApp = 4' 
	if(@Option=0) begin
		 set @strSQL = @strSQL + ' and RequestNo= '+@Param
	end
	else if (@Option=1) begin
		set @strSQL = @strSQL + ' and ID= '+@Param
	end
		insert into @table EXEC sp_executesql @strSQL
		select *,'0'Overprice,'0' as Extracost,'0'SubContainers,'0'OfferPrice,ID as 'RowID',(select ExchangeRate from @table b where b.ID=a.RequestNo)'ExchangeRate'
		from TransFormulaHeader a where RequestNo in (select ID from @table)
END

 

go

