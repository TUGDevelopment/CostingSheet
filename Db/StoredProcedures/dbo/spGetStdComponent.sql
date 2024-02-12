/****** Script for SelectTopNRows command from SSMS  ******/
CREATE procedure spGetStdComponent
@Id nvarchar(max),
@SubId nvarchar(max)
as begin
SELECT [ID]
      ,[Component]
      ,[Result]
      ,[Price]
      ,[Unit]
      ,[SubID]
      ,[RequestNo]
  FROM [dbo].[TransStdComponent] where RequestNo=@Id and SubId=@SubId
end
go

