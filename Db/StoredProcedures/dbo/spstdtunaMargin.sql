-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spstdtunaMargin]
	-- Add the parameters for the stored procedure here
	@Zone nvarchar(max),
	@SHD nvarchar(max),
	@validDate date
AS
BEGIN
	--declare @Company nvarchar(max)='102',	@ID nvarchar(max)='348',@NetWeight nvarchar(max)='80',@Packaging nvarchar(max)='Can'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select * from StdTunafixMargin where [Zone]=@Zone and Style=@SHD 
	and @validDate between validfrom and validto

END
--select SUBSTRING('02001',1,2)
--update StdTunafixMargin set Margin=1


go

