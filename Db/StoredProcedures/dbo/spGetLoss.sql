-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetLoss]
	-- Add the parameters for the stored procedure here
	@To DateTime,
	@PackageType nvarchar(max),
	@u nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
--declare @To DateTime='5/4/2022',	@PackageType nvarchar(max)='Pouch',	@u nvarchar(max)	=1
 select * from MasPFLoss where PackageType=@PackageType and dbo.fnc_checktype(isnull(BU,''),@u)>0
 and (@To between Validfrom and Validto) 
ORDER BY 
 CASE  SubType WHEN 'Raw Materrial & Ingredient' THEN 0 
 when 'Primary Packaging' then 1 
 when 'Secondary Packaging' then 2 END
END
go

