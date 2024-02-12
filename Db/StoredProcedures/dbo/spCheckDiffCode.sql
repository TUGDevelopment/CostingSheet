-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spCheckDiffCode]
	-- Add the parameters for the stored procedure here
	@Code nvarchar(max),
	@SapCodedigit nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--declare @Code nvarchar(max)='3AA5CBABJAEN5IUARU', @SapCodedigit nvarchar(max)='3AAAUN2NXFB9SIUURL'
	
	 if
	 (select count(*) from(
	select distinct(FishGroup) from MasFishSpecies where SAPCode in (select value from dbo.FNC_SPLIT(concat(substring(@Code,3,2),';',substring(@SapCodedigit,3,2)),';')) group by FishGroup) #a) = 1 and 
	 (
	 select count(*) from(
	 select distinct(FishCert) from MasFishCert where sapcode in (select value from dbo.FNC_SPLIT(concat(substring(@Code,15,2),';',substring(@SapCodedigit,15,2)),';')) group by FishCert)#a)=1 and
	 (select count(*) from(
	 select distinct(GroupStyle) from StandardStyle where SAPCodeDigit in (select value from dbo.FNC_SPLIT(concat(substring(@Code,5,1),';',substring(@SapCodedigit,5,1)),';')) group by GroupStyle)#a)=1
	 select '0' Result
	 else
	 select '1' Result
END
go

