-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetTunaStdItems]
	@ID nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	declare @table table(material nvarchar(max),name nvarchar(max),old nvarchar(max))
	insert into @table EXECUTE spGetsapMaterial
	SELECT a.*,
	case when isnull(material,'')='' then '' else (select top 1 b.name from @table b where b.Material=a.Material) end as'Description','' as Mark ,
	'' as Notes,
	'' as Attached
	from TransTunaStdItems a where RequestNo=@ID
END

go

