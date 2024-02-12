CREATE procedure spGetVarietyPack 
	@RequestNo nvarchar(max)
as begin
	select * from TransVarietyPack 
	where RequestNo=@RequestNo
end

go

