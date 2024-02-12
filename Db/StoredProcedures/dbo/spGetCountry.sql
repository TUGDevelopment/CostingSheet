CREATE PROCEDURE [dbo].[spGetCountry]
	-- Add the parameters for the stored procedure here
	 @group nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @group nvarchar(max)=7986
    -- Insert statements for procedure here

	select #a.*,a.Zone from TransDestination a left join 
	(select Code, [Description] as Name from [DevQCAnalysis].dbo.[transGrade] where ProductType='PF' --and IsActive=0
	group by Code,[Description]
	)#a on #a.Code = a.Country collate Thai_CI_AS
	 where RequestNo=@group 
END

go

