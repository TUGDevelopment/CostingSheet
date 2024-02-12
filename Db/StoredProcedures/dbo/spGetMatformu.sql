-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetMaterial2]
	-- Add the parameters for the stored procedure here
AS
BEGIN
	--declare @n nvarchar(50)='0',	@Keyword nvarchar(max)='13L200031003'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
    -- Insert statements for procedure here
	--	DECLARE @cols AS NVARCHAR(MAX),
	--	@query  AS NVARCHAR(MAX);
	--	if (@n=0 and @Keyword<>'')
	
	--set @query='WHERE ID='''+@Keyword+''''
	--else
	--set @query= 'WHERE ID LIKE ''%'+ @Keyword +'%'' Or Name LIKE ''%'+ @Keyword +'%'''
	--set @cols=  'SELECT top 100 * from MasMaterial '+ @query;
	--print @cols;
	--declare @bu nvarchar(max)='102'--(select Bu from ulogin where [user_name]=@user)
    -- Insert statements for procedure here
	SELECT a.ID ,a.Name--,Material,[Description],Yield
	from MasMaterial a --left join MasYield b on a.ID	=b.RawMaterial   
 
	--	execute(@cols)
END
go

