-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetTunaStdItems2]
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	--declare @ID nvarchar(max) =971
	If Object_ID('tempdb..#table')  is not null  drop table #table
	select SubID,RequestNo,Name,Notes into #table from TransStdFileDetails t where t.RequestNo=@ID and t.Result= 'Margin'
	declare @table table(id int,material nvarchar(max),name nvarchar(max),old nvarchar(max))
	insert into @table EXECUTE spGetsapMaterial2
	SELECT a.[ID]
      ,[Material]
	  ,isnull((select top 1 FishGroup from MasFishSpecies where sapcode = substring([Material],3,2)),'None') as FishGroup
	  ,isnull((select top 1 FishCert from MasFishCert where sapcode = substring([Material],15,2)),'') as 'EUGEN'
      ,[Name]
      ,[Utilize]
      ,[From]
      ,[To]
      ,[RawMaterial]
      ,[SpecialFishPrice]
      ,[PackSize]
      ,[Yield]
      ,[FillWeight]
      ,[SubContainers]
      ,[Media]
      ,[Packaging]
      ,[LOHCost]
      ,[PackingStyle]
      ,[SecPackaging]
      ,[Upcharge]
      ,[Commission]
      ,[OverPrice]
      ,[OverType]
      ,[Pacifical]
      ,[MSC]
      ,[Margin]
      ,[Authorized_price]
      ,[Bidprice]
      ,[MarginBid]
      ,[MinPrice]
      ,[OfferPrice]
      --,(case when IsAccept = '' then 1 else 0 end) as 'IsAccept'
	  ,IsAccept
      ,[RequestNo]
	  ,
	case when isnull(material,'')='' then '' else (select top 1 b.name from @table b where b.Material=a.Material) end as'Description','' as Mark ,
	isnull((select top 1 n.Notes from #table n where n.SubID= a.ID),'') as Notes,
	isnull((select top 1 t.Name from #table t where t.SubID= a.ID),'')  as Attached,
	rawmaterial as 'Equivalent_Fish_price',isnull(SpecialFishPrice,'')as 'Announcement_Fish_price','' as Bidprice,
	Authorized_price,
	isnull((case  (select b.StatusApp from TransTunaStd b where b.ID=a.RequestNo) when 0 then '' when 1 then 1 end),0) as 'IsActive'
	from TransTunaStdItems a where RequestNo=@ID
END
 --select * from TransTunaStd where RequestNo='20220100021'
go

