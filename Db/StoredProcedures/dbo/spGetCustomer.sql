 create PROCEDURE [dbo].[spGetCustomer]
AS
BEGIN

  select case when Code is null Or Code ='' then Custom else 
  convert(float, (case when ISNUMERIC(Code)=0 then convert(int,code) else 
  Code end) )
  
  end as 'Code',Name,isnull(Zone,'')as'Zone' from MasCustomer union select '','',''
end
go

