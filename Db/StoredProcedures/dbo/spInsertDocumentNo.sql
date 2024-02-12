
CREATE Procedure [dbo].[spInsertDocumentNo]
	@id int,
	@Requester nvarchar(50),
	@RequestNo nvarchar(50),
	@Costno nvarchar(max),@Destination nvarchar(50),@Country nvarchar(max)
 AS
BEGIN
 declare @runid nvarchar(max)
	set @runid= FORMAT(GetDate(), 'yyyyMM') +''+ 
	(SELECT format(isnull(max(right(documentno,5)),0)+1, '00000') FROM TransRequestForm
	where SUBSTRING(documentno,1,6)=FORMAT(GetDate(), 'yyyyMM'))

	insert into TransRequestForm (CreateBy,Requester,RequestNo,Costno,StatusApp,CreateOn,documentno,
	[UniqueColumn],RequestDate,Country,Destination,RefSamples,Assignee
	)values (@Requester,@Requester,@RequestNo,@Costno,1,getdate(),@runid,NEWID(),getdate(),@Country,@Destination,
	(select a.RefSamples from TransFormulaHeader a where a.id=@Costno),(select  Receiver from TransTechnical where id=@RequestNo ))
	SET @Id = (SELECT CAST(scope_identity() AS int))
	select @Id
	--select * from TransRequestForm
	--delete TransRequestForm where id in (6,7)
end

--update TransRequestForm set StatusApp=1 where id=18403
--select isnull((select email from ulogin u where u.user_name = h.Username),'')uname,h.StatusApp from MasHistory h  where tablename='TransTechnical' and RequestNo=6479
go

