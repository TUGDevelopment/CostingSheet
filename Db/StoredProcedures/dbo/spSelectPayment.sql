-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[spSelectPayment]
--@Code nvarchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON; 
--declare @Code nvarchar(max) ='10000101'
If Object_ID('tempdb..#MasCustomer')  is not null  drop table #MasCustomer
select * into #MasCustomer from(
select case when Code is null Or Code ='' then Custom else Code end as 'Code',Name,isnull(Zone,'')as'Zone' from MasCustomer union select '','','')#a

If Object_ID('tempdb..#tsapKNVV')  is not null  drop table #tsapKNVV
select knvv_zterm,KNVV_KUNNR into #tsapKNVV from tsapKNVV where KNVV_VTWEG='EX' group by knvv_zterm,KNVV_KUNNR
select * from(
select  isnull(knvv_zterm,'0')'knvv_zterm',
(case when ISNUMERIC(a.Code)=0 then convert(int,a.Code) else 
  a.Code end) as 'Code',a.Name,
isnull(isnull((select top 1 isnull(c.LeadTime,'0') from MasPaymentFix c where c.Code=b.knvv_zterm),(select top 1 p.LeadTime from MasPaymentTerm p where p.Code=b.knvv_zterm)),'0') 'LeadTime'
from #MasCustomer a left join #tsapKNVV b on REPLACE(LTRIM(REPLACE(b.KNVV_KUNNR, '0', ' ')),' ', '0')=a.Code --where a.Code = @Code 
union select '0',code,Name,'0' from MasCustomer where Code=1 )#a order by Code  

END

--SELECT REPLACE(LTRIM(REPLACE('0000000101', '0', ' ')),' ', '0')
go

