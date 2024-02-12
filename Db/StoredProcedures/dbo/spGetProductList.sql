-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetProductList]
	-- Add the parameters for the stored procedure here
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Id nvarchar(max)='6952'
	SET NOCOUNT ON;
    declare @Pivot_Column nvarchar(max)
	SELECT @Pivot_Column= COALESCE(@Pivot_Column+',','')+  (convert(nvarchar(max),ID)) FROM  
	(SELECT DISTINCT [ID] FROM TransProductList where RequestNo=@Id)Tab  
	select @Pivot_Column c
END
go

