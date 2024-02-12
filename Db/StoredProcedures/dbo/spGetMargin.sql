-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetMargin]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@ID nvarchar(max),
	@NetWeight nvarchar(max),
	@Packaging nvarchar(max),
	@UserType nvarchar(max)
AS
BEGIN
	--declare @Company nvarchar(max)='101',	@ID nvarchar(max)='7991',@NetWeight nvarchar(max)='10',@Packaging nvarchar(max)='Can',@UserType nvarchar(max)='2'
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	declare 
	@productcate nvarchar(max)
	select 
	@productcate=PetCategory
	from TransTechnical where ID=@ID
	If Object_ID('tempdb..#table')  is not null  drop table #table
	SET NOCOUNT ON;
	if(select CustomPrice from TransTechnical where ID=@ID)='2'
	begin
	if(@Company='101')begin
	select * from MasMargin where @company like Company+'%' and dbo.fnc_checktype(@userType,isnull(BU,''))>0 and dbo.fnc_checktype(@productcate,Category)>0
	end else begin
	select b.LBOh,convert(float,@NetWeight) as 'NetWeight' into #table from MasPrimary b 
	where b.Name=@Packaging and b.UserType in (select UserType from TransTechnical where ID=@ID)
	
	--select * from #table
	select b.ID,MarginCode,MarginName,MarginRate, PercentMargin from #table a left join MasMargin b on substring(MarginCode,1,2) = a.Lboh
	where Company=@Company 
	order by MarginName end end
	else 
	select ID,MarginCode,MarginName,MarginRate, PercentMargin from MasMargin  
	where Company=@Company and MarginCode='99001'
END
--select SUBSTRING('02001',1,2)
--select * from MasMargin


go

