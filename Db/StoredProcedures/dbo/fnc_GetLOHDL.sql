-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE FUNCTION [dbo].[fnc_GetLOHDL]( 
	@LOH float,
	@Formula nvarchar(max),
	@SubID nvarchar(max))
	RETURNS  nvarchar(max)  
AS
BEGIN
--declare @LOH nvarchar(max)='10',@Formula nvarchar(max)='1',@SubID nvarchar(max)='1225'
declare @sumObject float
if(@LOH>0) begin
declare @LOHFactor nvarchar(max),@DL nvarchar(max)
set @LOHFactor = (select top 1 convert(float,Quantity) from TransCosting where [Description]='LOH Factor Adjustment' and SellingUnit='%'
and RequestNo=@SubID and Formula=@Formula)

set @DL = (select top 1 convert(float,Quantity) from TransCosting where [Description]='Labelling & WH DL' and SellingUnit in ('USD','THB')
and RequestNo=@SubID and Formula=@Formula)

set @sumObject =((@LOHFactor / 100) * @LOH) + @LOH + @DL

--print @sumObject; 
end
RETURN isnull(@sumObject,@LOH)
END


--select dbo.fnc_GetLOHDL( '10','2','1225')
go

