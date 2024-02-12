-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spinsertMargin]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@MarginName nvarchar(max),
	@MarginRate nvarchar(max),
	@tcode nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @Company nvarchar(max)='162',	@tcode nvarchar(max)='Can'
	SET NOCOUNT ON;
	declare @MarginCode nvarchar(max),@PercentMargin nvarchar(max)='%',@LBOh nvarchar(max)
	set @LBOh=(select LBOh from MasPrimary where Name=@tcode)
    -- Insert statements for procedure here
	--select * from MasMargin
	select @MarginCode=(max(convert(int,SUBSTRING(margincode,3,3)))) from MasMargin where Company=@Company
	and SUBSTRING(margincode,1,2) = @LBOh 
	--select @LBOh+''+format(isnull(@MarginCode,0)+1,'000');
	Insert into MasMargin values(@Company,@LBOh+''+format(isnull(@MarginCode,0)+1,'000'),@MarginName,@MarginRate,@PercentMargin)
END


go

