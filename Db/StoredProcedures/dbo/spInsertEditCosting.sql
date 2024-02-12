-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertEditCosting] 
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@RequestNo nvarchar(max),
	@CostingSheet nvarchar(max),
	@Material nvarchar(max),
	@Series nvarchar(max),
	@user nvarchar(max),
	@SellingUnit nvarchar(max), 
	@Units  nvarchar(max),
	@TotalPack nvarchar(max),
	@PackingStyle nvarchar(max),
	@VarietyPack nvarchar(max)
AS
BEGIN
	--@ID nvarchar(max)='0',
	--@RequestNo nvarchar(max)='8010',
	--@CostingSheet nvarchar(max),
	--@Material nvarchar(max)='3GACFK5WK2DN5EZU00',
	--@Series nvarchar(max)='1',
	--@user nvarchar(max)='FO5910155',
	--@SellingUnit nvarchar(max)='2', 
	--@Units  nvarchar(max)='4',
	--@TotalPack nvarchar(max)='24',
	--@PackingStyle nvarchar(max)='6 Cans (2 Cans/ Flavor)/ Inner Box, 4 Inner Boxes/ Carton',
	--@VarietyPack nvarchar(max)='3VAE0004073Y'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	if(select count(id) from TransEditCosting where Id=@ID)=0
	insert into 
	TransEditCosting values (@RequestNo,@Material,@CostingSheet,0,@Series,0,0,@user,getdate(),null,null,@VarietyPack,@SellingUnit,@Units,@TotalPack,@PackingStyle)
END
--select * from TransEditCosting
--truncate table TransEditCosting

go

