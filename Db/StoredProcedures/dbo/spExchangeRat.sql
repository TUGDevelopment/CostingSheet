CREATE PROCEDURE [dbo].[spExchangeRat]
	-- Add the parameters for the stored procedure here
	@Id nvarchar(max)
AS
BEGIN	
--declare @id nvarchar(max)=4664
  declare @KeyDate nvarchar(max),@Company nvarchar(max),
	@Customer nvarchar(max)
	select @Company=substring(t.Company,1,3),@customer=t.CustomPrice ,@KeyDate=(case when t.CustomPrice=1 then RequireDate else RequestDate end)
	from TransTechnical t where ID=@Id
	declare @d datetime =cast(@KeyDate as date)
  if @Customer=1
	set @d= (DATEADD(yy, DATEDIFF(yy, 0, @KeyDate), 0))
  else 
	set @d = @KeyDate
  --print @d;
  set @d =(case when @customer=1 then (SELECT DATEADD(yy, DATEDIFF(yy, 0, @d), 0)) else @d end)
  select isnull(Rate,'')'Rate' from MasExchangeRat where @d between Validfrom and Validto
  and Company=@Company and ExchangeType=@Customer
End

--select * from TransTechnical where id=2131

go

