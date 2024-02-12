-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetOHRate]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@DataType nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @ID nvarchar(max)='7991',@DataType nvarchar(max)='OH'
	SET NOCOUNT ON;
	declare 
	@company nvarchar(max),
	@userType nvarchar(max),
	@productcate nvarchar(max)
	select @company=Company,
	@userType=UserType,
	@productcate=PetCategory
	from TransTechnical where ID=@ID
    -- Insert statements for procedure here
	print @productcate;
	SELECT a.ID,a.Name,a.Rate,
	(SELECT abc = STUFF((SELECT DISTINCT  ','+ convert(nvarchar(max),b.ID) +':'+ b.Name
                                         FROM      MasPetCategory b
                                         WHERE  b.ID in (select value from dbo.FNC_SPLIT(replace(a.Category,',',';'),';')) FOR XML PATH('')),1,1,'')) 'CateName'
										 from MasOHSGA a where @company like Company+'%' and dbo.fnc_checktype(@userType,usertype)>0 and dbo.fnc_checktype(@productcate,Category)>0
	and DataType=@DataType
	--select * from MasOHSGA
END
go

