CREATE Procedure [dbo].[spTunaStdExchangeRat]
	-- Add the parameters for the stored procedure here
	@from datetime,
	@to datetime
AS
BEGIN	
--declare @from datetime='2020-01-01',	@to datetime='2020-12-31'
  --declare @d datetime =cast(Getdate() as date)
  select top 1 isnull(Rate,0)'Rate',Currency from stdExchangeRat where 
	(--@from between cast(Validfrom as date) and cast (Validto as date) Or 
	@to between cast(Validfrom as date) and cast (Validto as date))--@d between Validfrom and Validto
End



go

