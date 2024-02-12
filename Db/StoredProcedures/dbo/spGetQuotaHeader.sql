-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetQuotaHeader]
	-- Add the parameters for the stored procedure here
	@user_name nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
	--declare @user_name nvarchar(max)='FO5910155',@type nvarchar(max)=0
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#find')  is not null  drop table #find
	select * into #find from dbo.FindULevel(@user_name)
	--select * from #find
	SET NOCOUNT ON;
	declare @temp tabletype;delete @temp
	insert into @temp
	select * from(select editor from #find)#a
	declare @editor nvarchar(max)= dbo.fnc_stuff(@temp)
	--print @editor;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	SELECT ID,
	(select top 1 firstname +' '+ lastname from ulogin where user_name=a.CreateBy)'Requester',
	CreateBy,
	CreateOn,
	Brand,
	(select top 1 Name from MasCustomer where Code=a.Customer)'Customer',
	(select top 1 Name from MasCustomer where Code=a.ShipTo)'ShipTo',
	a.PaymentTerm,
	a.Incoterm,
	a.StatusApp,
	a.RequestNo
	into #temp from TransQuotationHeader a
    -- Insert statements for procedure here
		if (@type=1)
		SELECT * from #temp a where a.ID in
		(select distinct(RequestNo) from MasHistory where Username in (select distinct value from dbo.FNC_SPLIT(@editor,','))
		and tablename in ('3','TransQuotationHeader')) 
			else
			begin
			if(select count(*) from #find where Sublevel = 3)>0
			SELECT * from #temp where StatusApp in (0) and CreateBy=@user_name
			if(select count(*) from #find where Sublevel = 4)>0
			SELECT * from #temp where StatusApp in (8)
			if(select count(*) from #find where Sublevel = 11)>0
			SELECT * from #temp where StatusApp in (9)
	end
END
go

