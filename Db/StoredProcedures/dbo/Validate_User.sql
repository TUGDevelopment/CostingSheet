 
-- =============================================
CREATE Procedure [dbo].[Validate_User] 
	-- Add the parameters for the stored procedure here
	@UserId nvarchar(max),@role nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @UserId nvarchar(max)='fo5910155',@role nvarchar(max)=0
	SET NOCOUNT ON;
	declare @editor nvarchar(max) =1 
    -- Insert statements for procedure here
	If Object_ID('tempdb..#temp')  is not null  drop table #temp

	select * into #temp from dbo.FindULevel(@UserId) 
	if(select count(Sublevel) from #temp where Sublevel in (1,2,5,6))>0 and (@role=2)
	set @editor=3
		else
	if (select count(idx) 'role' from #temp where idx in(0,2,9) and
	(select count(value) from dbo.FNC_SPLIT(subapp,',') where value=@role)>0
	--idx=@role
	)>0
		begin
		set @editor=0
		end
	select @editor
	--select * from #temp
END
go

