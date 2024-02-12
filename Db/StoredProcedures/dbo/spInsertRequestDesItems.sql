-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spInsertRequestDesItems] 
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@RequestNo nvarchar(max),
	@CostingSheet nvarchar(max),
	@Material nvarchar(max),
	@Series nvarchar(max),
	@user nvarchar(max),
	@Result nvarchar(max)
AS
BEGIN
	--@ID nvarchar(max)='0',
	--@RequestNo nvarchar(max)='8010',
	--@CostingSheet nvarchar(max),
	--@Material nvarchar(max)='3GACFK5WK2DN5EZU00',
	--@Series nvarchar(max)='1',
	--@user nvarchar(max)='FO5910155',

	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--if(select count(id) from TransRequestDesItems where Id=@ID)=0
	insert into 
	TransRequestDesItems values (@RequestNo,@Material,@CostingSheet,@Result,@Series,0,0,@user,getdate(),null,null)
END
--select * from TransRequestDesItems
--truncate table TransEditCosting

go

