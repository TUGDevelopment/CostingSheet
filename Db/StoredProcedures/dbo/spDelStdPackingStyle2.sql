-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE [dbo].[spDelStdPackingStyle2]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @id nvarchar(max) = '3AAOLN2CJAJN5MUU00|VB-001|Media'
	declare @SAPCodeDigit nvarchar(50),
		@Size nvarchar(50) 
	DECLARE @tbl TABLE (Value INT,String VARCHAR(MAX))
	INSERT INTO @tbl VALUES(1,@id);
	--select @company=,@material,@request
	select @SAPCodeDigit=SAPCodeDigit,@Size=Size from 
	(SELECT t3.Value,[1] as SAPCodeDigit,[2] as Size
	FROM @tbl as t1
	CROSS APPLY [dbo].[DelimitedSplit8K](String,'|') as t2
	PIVOT(MAX(Item) FOR ItemNumber IN ([1],[2])) as t3) #a
	--declare @ID nvarchar(max)='3AAOLN2CJAJN5MUU00',	@Code nvarchar(max)='VB-001',@GroupType nvarchar(max)='Media'
	Delete StdPackingStyle2 Where SAPCodeDigit=@SAPCodeDigit and Size=@Size
END
go

