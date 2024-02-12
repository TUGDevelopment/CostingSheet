-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spUpStdSecPKGCost
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Amount nvarchar(max),
	@Currency nvarchar(max),
	@Unit nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @id nvarchar(max) = '3AAOLN2CJAJN5MUU00|VB-001|Media'
	declare @SAPCodeDigit nvarchar(50),
		@Customer nvarchar(50),
		@ShipTo nvarchar(50) 
	DECLARE @tbl TABLE (Value INT,String VARCHAR(MAX))
	INSERT INTO @tbl VALUES(1,@id);
	--select @company=,@material,@request
	select @SAPCodeDigit=SAPCodeDigit,@Customer=Customer,@ShipTo=ShipTo from 
	(SELECT t3.Value,[1] as SAPCodeDigit,[2] as Customer,[3] as ShipTo
	FROM @tbl as t1
	CROSS APPLY [dbo].[DelimitedSplit8K](String,'|') as t2
	PIVOT(MAX(Item) FOR ItemNumber IN ([1],[2],[3])) as t3) #a
	--declare @ID nvarchar(max)='3AAOLN2CJAJN5MUU00',	@Code nvarchar(max)='VB-001',@GroupType nvarchar(max)='Media'
	Update StdSecPKGCost set Amount=@Amount,Currency=@Currency,Unit=@Unit where Material=@SAPCodeDigit and Customer=@Customer and ShipTo=@ShipTo
END
go

