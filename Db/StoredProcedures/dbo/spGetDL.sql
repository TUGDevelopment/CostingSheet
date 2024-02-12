-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetDL]
	-- Add the parameters for the stored procedure here
	@RequestNo nvarchar(max),
	@Company nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @RequestNo nvarchar(max)='8078',	@Company nvarchar(max)='101'
	declare 
	--@company nvarchar(max),
	@userType nvarchar(max),
	@productcate nvarchar(max)
	select @company=Company,
	@userType=UserType,
	@productcate=PetCategory
	from TransTechnical where ID=@RequestNo
	--print @userType;
    -- Insert statements for procedure here
	SELECT * from MasDL where @company like Company+'%' and 
	 dbo.fnc_checktype(@productcate,Category)>0 and dbo.fnc_checktype(@userType,BU)>0 
END


go

