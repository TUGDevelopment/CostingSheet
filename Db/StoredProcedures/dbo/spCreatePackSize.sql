-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create PROCEDURE spCreatePackSize
	-- Add the parameters for the stored procedure here
	@ID nvarchar(max),
	@Param nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	--declare @ID nvarchar(max)='266',@Param nvarchar(max)='24,120'
	SET NOCOUNT ON;
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	select ID,RequestNo,format(RequireDate,'dd-MM-yyyy')'RequireDate',PackSize, Packaging, 
	RIGHT(CONCAT('00',Revised), 2)'Revised',RequestType,format(RequestDate,'dd-MM-yyyy')'RequestDate',NetWeight,
	DATEDIFF(day,RequireDate,GETDATE())x,Modified into #temp 
	from TransTechnical Where ID=@ID
	--select * from #temp
	declare @table table(
	ID int IDENTITY(1,1) NOT NULL,
	ParentID int,
	RequestNo nvarchar(max),
	RequireDate nvarchar(max),
	PackSize nvarchar(max), 
	Packaging nvarchar(max), 
	Revised int,
	RequestType nvarchar(max),
	RequestDate nvarchar(max),Modified datetime,NetWeight nvarchar(max)
	)
    -- Insert statements for procedure here
	insert into @table
	select 0,
	b.RequestNo,
	b.RequireDate,
	convert(int,value),
	b.Packaging,
	b.Revised,
	b.RequestType,
	b.RequestDate,b.Modified,
	b.NetWeight from dbo.FNC_SPLIT(@Param,',') a, #temp b

	select ID,
	ParentID,
	RequestNo,
	Packaging,
	PackSize  from @table order by Modified,PackSize,NetWeight desc
END
go

