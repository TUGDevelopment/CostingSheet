-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE spGetBuildDetail
	-- Add the parameters for the stored procedure here
	@id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	select *,isnull(Code,'') as 'Material'
        ,isnull((select top 1 z.UserType from TransCostingHeader z where Id=a.Requestno),0)UserType
	    ,isnull((select top 1 a.NetWeight from TransProductList a where Id=pid),0)NetWeight
        ,isnull((select top 1 a.FixedFillWeight from TransProductList a where Id=pid),0)FixedFillWeight
        ,isnull((select top 1 a.PackSize from TransProductList a where Id=pid),0)PackSize from TransFormulaHeader a where a.Requestno=@id 
END
go

