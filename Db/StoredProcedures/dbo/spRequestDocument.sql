-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
 
-- =============================================
CREATE PROCEDURE [dbo].[spRequestDocument]
	-- Add the parameters for the stored procedure here
	@CreateBy nvarchar(max),
	@Condition nvarchar(max),
	@ProductGroup nvarchar(max),
	@ProductCode nvarchar(max),
	@Material nvarchar(max)
AS
BEGIN
	If Object_ID('tempdb..#temp')  is not null  drop table #temp
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	declare @run nvarchar(max),@Id int,@version nvarchar(max)
	set @run=  '1RD'+''+ case @Condition when 1 then 'H' when 2 then 'P' when 3 then 'V' end +''+ convert(nvarchar,getDate(),112) +''+
	(SELECT format(isnull(max(substring(DocumentNo,13,4)),0)+1, '0000') FROM transRequest where Condition=@Condition)
	
	Insert into transRequest (Condition,CreateBy,CreateOn,StatusApp,DocumentNo,ProductGroup,ProductCode,Material) --9 : UnAssign,0 : ChangePoint
	values (@Condition,@CreateBy,getdate(),0,@run,@ProductGroup,@ProductCode,@Material)
	set @Id = (SELECT CAST(scope_identity() AS int))
	Insert into TransStep values
     (@Id,0,@Condition,'RD',@CreateBy,getdate(),0),--RD
	 (@Id,0,@Condition,'RD_Approve',@CreateBy,getdate(),0),--RD_Approve
	 (@Id,0,@Condition,'QC',@CreateBy,getdate(),1),--QC
	 (@Id,0,@Condition,'MKT_Support',@CreateBy,getdate(),1),--MKT_Approve
	 (@Id,0,@Condition,'MDC_Approve',@CreateBy,getdate(),3)--MDC_Approve

	--+Ccreate History
	Insert Into TransHistory values(@Id,@CreateBy,9,getdate())
	SELECT * FROM transRequest Where Id = @Id
	--truncate table template
END




go

