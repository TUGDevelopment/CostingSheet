-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSelMaterial]
	-- Add the parameters for the stored procedure here
	@Company nvarchar(max),
	@requestno nvarchar(max)
AS
BEGIN
	--declare @Company nvarchar(max)='102',@RawMaterial nvarchar(max)='4100330',@requestno nvarchar(max)=7991
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	If Object_ID('tempdb..#table')  is not null  drop table #table
	SET NOCOUNT ON;
	declare @BU nvarchar(max),
	@f date ,
	@t date 
	select @bu =usertype,
	@f=cast(RequestDate as date),@t=cast(RequireDate as date) from TransTechnical where ID=@requestno
		select * into #table from MasPricePolicy where
	(@f between cast([From] as date) and cast ([To] as date) Or @t between cast([From] as date) and cast ([To] as date))

	Select ID,Material,[Description]
      ,[Jan]
      ,[Feb]
      ,[Mar]
      ,[Apr]
      ,[May]
      ,[Jun]
      ,[Jul]
      ,[Aug]
      ,[Sep]
      ,[Oct]
      ,[Nov]
      ,[Dec]
      ,[Currency]
      ,[Unit]
	  From #table where (select count(value) from dbo.FNC_SPLIT(BU,';') where value =@bu)>0 and
	(@f between cast([From] as date) and cast ([To] as date) Or @t between cast([From] as date) and cast ([To] as date))
END
 


go

