-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE Procedure [dbo].[spGetRequestDesall]
	-- Add the parameters for the stored procedure here
	@Keyword nvarchar(max),
	@user_name nvarchar(max),
	@type nvarchar(max)
AS
BEGIN
 	If Object_ID('tempdb..#temp')  is not null  drop table #temp
    select 
	[ID]
      ,[RequestNo]
      ,[Remark]
      ,[Assignee]
      ,[CreateOn]
      ,[ModifyBy]
      ,[ModifyOn]
      ,[Destination]
      ,[UniqueColumn]
      ,[StatusApp]
      ,[RequestDate]
      ,[RequireDate]
      ,[Company]
      , (select concat(FirstName ,' ',lastname) from ulogin where user_name=[CreateBy]) as 'CreateBy' from TransRequestDesForm
END
 
go

