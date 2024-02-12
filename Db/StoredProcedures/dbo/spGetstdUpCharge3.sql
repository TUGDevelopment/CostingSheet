-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetstdUpCharge3]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@SubID nvarchar(max),
	@Code nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
	--declare @ID nvarchar(max) =35380,	@SubID nvarchar(max)=828,@code nvarchar(max)='3AAOLO2OSAANAIKAQW'
    -- Insert statements for procedure here
	SELECT ID,UpchargeGroup,UpCharge,Price,Quantity,Currency,Result,stdPackSize,SubID,RequestNo  from TransUpCharge where RequestNo=@ID
	and SubID=@SubID 
	--union select ID,UpchargeGroup,UpCharge,Value,1,Currency,Value,StdPackSize,@SubID,@ID from StandardUpcharge where isnull(sapdigit,'')=substring(@Code,12,2) 

END
go

