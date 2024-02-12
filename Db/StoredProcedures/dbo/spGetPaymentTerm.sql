-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spGetPaymentTerm]
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
	--select * from MasPaymentTerm
    -- Insert statements for procedure here
	select Code,Value,isnull((select top 1 isnull(a.LeadTime,'0') from MasPaymentfix a where a.Code=MasPaymentTerm.Code),MasPaymentTerm.LeadTime) 'LeadTime' 
	from MasPaymentTerm where code not in (select a.ZTERM from stdFixExcl a )
	union select '0','None',0
END
go

