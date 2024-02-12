CREATE PROCEDURE [dbo].[spUpdateLeadTime]
AS
BEGIN
If Object_ID('tempdb..#temp')  is not null  drop table #temp
SELECT [T052S_ZTERM]
      ,[T052S_RATNR]
      ,[T052S_RATPZ]
	  ,CAST((select top 1 isnull([LeadTime],'0')  FROM  [MasPaymentTerm] where Code=[T052S_RATZT]) * ([T052S_RATPZ]/100)AS decimal(18,2)) [LeadTime] into #temp
  FROM [DevCostingSheet].[dbo].[tsapT052S] where [Log] is null
  If Object_ID('tempdb..#t')  is not null  drop table #t
  select --sum(convert(decimal(18,2), [LeadTime]))dLeadTime,
  sum(CAST([LeadTime] AS decimal(18,2)))dLeadTime,
  [T052S_ZTERM] into #t from #temp group by [T052S_ZTERM]
--select * from tsapT052s where T052S_ZTERM='i109'
--select * from #t where T052S_ZTERM='i109'
UPDATE A
SET LeadTime = convert(decimal(18,2), B.dLeadTime)
FROM MasPaymentTerm A
left join #t B
    ON A.Code = B.T052S_ZTERM
WHERE LeadTime =0

--select * from tsapT052 where ZTERM='i101'
UPDATE A
SET LeadTime = B.LeadTime
FROM MasPaymentTerm A
left join tsapT052 B
    ON A.Code = B.ZTERM
WHERE a.Code in (select z.ZTERM from tsapT052 z where z.LeadTime<>0)

--select * from MasPaymentTerm where code='i109'
end

go

