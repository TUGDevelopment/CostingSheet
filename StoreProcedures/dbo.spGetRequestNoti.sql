USE [DevCostingSheet]
GO

/****** Object:  StoredProcedure [dbo].[spGetRequestNoti]    Script Date: 9/15/2023 1:48:28 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[spGetRequestNoti] 
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	--declare @Id nvarchar(max)=''
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	declare @rd nvarchar(max)
	set @rd= (
	select concat(h.Username,',',replace(h.reason,'|','')) from MasHistory h  where tablename='TransTechnical' and h.StatusApp in (5) and RequestNo =@Id )

	select u.user_name, email,'5' as 'Statusapp' into #temp from ulogin u where u.user_name in (select value from dbo.FNC_SPLIT(@rd,',')) 
	if (select count(*) from #temp )>0
	select * from #temp
	else
	select user_name,email,'5' as 'Statusapp' from ulogin where user_name in (select value from dbo.FNC_SPLIT((select (dbo.fnc_getuser(6,Company,UserType)) from TransTechnical where id=@Id and RequestType=1),','))


END
GO


