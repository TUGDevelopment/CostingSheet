-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPFType]
	-- Add the parameters for the stored procedure here
	@Primary nvarchar(max),
	@usertype nvarchar(max)
AS
BEGIN
    --declare @Primary nvarchar(max)='Pouch',@usertype nvarchar(max)='0,1'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @table table (ID int,Name nvarchar(max))
	--declare @strcount int
	--set @strcount = (SELECT count(value) FROM dbo.FNC_SPLIT(@usertype,';'))
	--if(@strcount>1)
	--select ID,Name from(Select * from MasPFType Where (HF = '1' or PF ='1' )
	--and @Primary in (select value from dbo.FNC_SPLIT(tcode,',')))#a union select 0,'-'
	--else
	select ID,Name from(Select * from MasPFType Where dbo.fnc_checktype(usertype,@usertype)>0
	and @Primary in (select value from dbo.FNC_SPLIT(tcode,',')))#a union select 0,'-'
END

go

