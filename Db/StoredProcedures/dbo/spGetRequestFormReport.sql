-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetRequestFormReport]
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @ID nvarchar(max)=13
    -- Insert statements for procedure here
select 
(select concat(code,':',Description) from [DevQCAnalysis].dbo.transCustomerBrand  where dbo.fnc_checktype( productgroup,  a.ProductGroup)>0 and Code=a.Zone collate Thai_CI_AS and ProductType='PF') 'BrandName',
(select concat(code,':',Description,',',ISNULL(MediaType,'')) from [DevQCAnalysis].dbo.transMediaType where dbo.fnc_checktype( productgroup, a.ProductGroup)>0 and ProductType='PF' and Code=a.MediaType collate Thai_CI_AS) 'MediaTypeName',
(select concat(Code,':',ContainerType,',',LidType) from  [DevQCAnalysis].dbo.transContainerLid  
		--where productgroup like (case when @group like  '%non%' then '____non fish base)' else '____fish base)'end  )
		where dbo.fnc_checktype( productgroup, a.ProductGroup)>0 and ProductType='PF' and code=a.ContainerLid collate Thai_CI_AS) 'ContainerLidName',
(select PH2Des from [DevQCAnalysis].dbo.HierDivision where PH2=a.Division collate Thai_CI_AS) as 'DivisionName',
(select Name from [DevQCAnalysis].dbo.HierNutrition where Code=a.Nutrition collate Thai_CI_AS) as 'NutritionName',
(select concat(code,':',CanSize,',',isnull( Media ,''),',',isnull(NW,'')) from  [DevQCAnalysis].dbo.transCanSize where dbo.fnc_checktype( productgroup, a.ProductGroup)>0 
and Packaging = (case when (select name from [DevQCAnalysis].dbo.tblProductGroup where ProductGroup=a.ProductGroup collate Thai_CI_AS) like  '%non can%' then 'Non Can' else 'Can'end )

and ProductType='PF' and code=a.NW collate Thai_CI_AS) 'CanSizeName',
(select concat(code,':',Description) from  [DevQCAnalysis].dbo.transProductStyle  where dbo.fnc_checktype( productgroup, a.ProductGroup)>0 and ProductType='PF' and IsActive='1' and code=a.StyleOfPack collate Thai_CI_AS) 'StyleName',
(select Name from [DevQCAnalysis].dbo.HierPetType where Code=a.PetType collate Thai_CI_AS) as 'PetTypeName',
(select Description from [DevQCAnalysis].dbo.transRawMaterial where ProductType='PF' and IsActive=0 and dbo.fnc_checktype(ProductGroup,a.ProductGroup)>0 and Code=a.RawMaterial collate Thai_CI_AS ) as RawMaterialName,
(select Description from [DevQCAnalysis].dbo.[transGrade] where ProductType='PF' and IsActive in (0,1,2) and dbo.fnc_checktype(ProductGroup,a.ProductGroup)>0 and Code=a.Grade collate Thai_CI_AS ) as ZoneName,
(select concat('(',ProductGroup,'): ',Name) as 'Name' from [DevQCAnalysis].dbo.tblProductGroup where GroupType='F' and IsActive='1' and ProductGroup=a.ProductGroup collate Thai_CI_AS ) as ProductGroupName,
* from TransRequestForm a where ID=@ID
END


go

