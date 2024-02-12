-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spCustomerRequest] 
	-- Add the parameters for the stored procedure here
	@Category nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @usertype nvarchar(max) 
    -- Insert statements for procedure here
	--declare @table table(Id int IDENTITY(1,1),Name nvarchar(50))
	--insert into @table
	--select value from dbo.FNC_SPLIT('Product Spec.;Prototype Photo;Costing,CS Profile',';')
 --    Insert statements for procedure here
	--select * from @table
	  set @usertype = (select usertype from MasPetCategory where ID=@Category)
	  select * from MasCustomerRequest where dbo.fnc_checktype(usertype,@usertype)>0
END



go

