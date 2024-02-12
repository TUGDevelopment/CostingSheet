-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
create FUNCTION [dbo].[fnc_DateDif]
(	
	-- Add the parameters for the function here
	@StartDate datetime,
	@EndDate datetime

)
RETURNS @table  TABLE (DateDif datetime,StartDate datetime,
	EndDate datetime,[Days] smallint, [Hours] int,[Minutes] int ,Seconds int ,MS int)
AS
begin
	-- Add the SELECT statement with parameter references here
insert into @table 
select
        *,
        Days          = datediff(dd,0,DateDif),
        Hours         = datepart(hour,DateDif),
        Minutes       = datepart(minute,DateDif),
        Seconds       = datepart(second,DateDif),
        MS            = datepart(ms,DateDif)
from
        (
        select
                DateDif = EndDate-StartDate,
                aa.*
        from
                (  -- Test Data
                Select
                        StartDate = convert(datetime,@StartDate),
                        EndDate   = convert(datetime,@EndDate)
                ) aa
        ) a
RETURN  
end
go

