-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
 CREATE Procedure [dbo].[spGetPlant]
  @usertype nvarchar(max),
  @Bu nvarchar(max)
  AS
BEGIN
--declare @usertype nvarchar(max)='1',@Bu nvarchar(max)='103'
--declare @query nvarchar(max)
--declare @table table (Code nvarchar(max),Title nvarchar(max),Company nvarchar(max))
--insert into @table
--select Code,Name,fn from MasCompany where Code in (select distinct value from dbo.FNC_SPLIT( @Bu,'|')) 
--select * from @table order by Code 
declare @query nvarchar(max)
If Object_ID('tempdb..#temp')  is not null  drop table #temp
select * from MasPlant  where Company in (select distinct value from dbo.FNC_SPLIT( @Bu,'|')) 
and dbo.fnc_checktype(usertype,@usertype)>0
order by Code 
end
go

