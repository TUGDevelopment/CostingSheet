
CREATE PROCEDURE [dbo].[spGetCostnoRequestForm]
@Id nvarchar(max)
AS
BEGIN

select #a.*,t.UserType, t.RequestNo as 'TRFRequestNo' from (
select b.RequestNo as RequestID,a.CostNo,b.RequestNo,a.ID 'CostID'
from TransFormulaHeader a inner join TransCostingHeader b on b.ID=a.RequestNo
where a.ID=@Id)#a inner join TransTechnical t on t.ID=#a.RequestID

end
go

