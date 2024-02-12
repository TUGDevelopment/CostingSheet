-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE spDelStdOverheadCost
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @id nvarchar(max) = '3AAOLN2CJAJN5MUU00|VB-001|Media'
	declare @PackagingType nvarchar(50),
		@CanSize nvarchar(50),
		@Size nvarchar(50),
		@Title nvarchar(50),
		@From datetime,
		@To datetime,
		@StdPackSize nvarchar(50)
	DECLARE @tbl TABLE (Value INT,String VARCHAR(MAX))
	INSERT INTO @tbl VALUES(1,@id);
	--select @company=,@material,@request
	select @PackagingType=PackagingType,@CanSize=CanSize,@Size=Size,@Title=Title,@From=[From],@To=[To],@StdPackSize=StdPackSize from 
	(SELECT t3.Value,[1] as PackagingType,[2] as CanSize,[3] as Size,[4] as Title,[5] as [From],[6] as [To],[7] as StdPackSize
	FROM @tbl as t1
	CROSS APPLY [dbo].[DelimitedSplit8K](String,'|') as t2
	PIVOT(MAX(Item) FOR ItemNumber IN ([1],[2],[3],[4],[5],[6],[7])) as t3) #a
	--declare @ID nvarchar(max)='3AAOLN2CJAJN5MUU00',	@Code nvarchar(max)='VB-001',@GroupType nvarchar(max)='Media'
	Delete StdOverheadCost Where PackagingType=@PackagingType and CanSize=@CanSize and Size=@Size and Title=@Title and [From]=@From and [To]=@To and StdPackSize=@StdPackSize
END
go

