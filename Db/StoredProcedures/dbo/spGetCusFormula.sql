-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetCusFormula]
	-- Add the parameters for the stored procedure here
	@Id int,
	@formula nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Id int=8,	@formula nvarchar(max)=1, @type nvarchar(max)=''
    -- Insert statements for procedure here
	SELECT  Id, Component, SubType, Description, Material, Result, Yield, RawMaterial, 
	Name, case when isnull(PriceOfUnit,'0') <> isnull(AdjustPrice,'0') then AdjustPrice else PriceOfUnit end 'PriceOfUnit', AdjustPrice, Currency, Unit, ExchangeRate, BaseUnit, 
	PriceOfCarton, Formula, IsActive, Remark, LBOh, LBRate, RequestNo,isnull(NW,'0')'NW'
	,isnull(Batch,'0')'Batch',isnull(Portion,'0')'Portion'
	from TransCusFormula where (case when @type='C' then isnull(RequestNo,'') else isnull(SubID,'') end)=@Id and Formula in 
	(select value from dbo.FNC_SPLIT(@formula,','))
END
   --delete TransCusFormula where id=855
go

