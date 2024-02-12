


/****** json_Item  ******/
CREATE FUNCTION [dbo].[json_Item](@id int,@parent int, @start int,@JSON NVARCHAR(MAX),@parseName int)
RETURNS @hierarchy table(id int,parent int,name nvarchar(2000),kind nvarchar(10),ppos int,pend int,value nvarchar(MAX) NOT NULL)
AS
	BEGIN
		declare @kind   nvarchar(10)
		declare @name   nvarchar(MAX)=''
		declare @value  nvarchar(MAX)
		declare @p		int
		
		if (@parseName=1) begin
			select  @kind=kind,@name=name,@start=pend from dbo.json_Name(@start,@json)
			if (@kind='ERROR') begin
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(0,@parent,'','ERROR',@start,-1,@name)
				return
			end
		end 
		set @kind=substring(@json,@start,1)
		set @start=dbo.json_Skip(@json,@start)
		-- Handle strings

		if (@kind='"') begin
			select @value=value,@p=p2 from dbo.json_String(@json,@start+1)
			if (@p=-1) begin
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,'','ERROR',@p,-1,isnull(@value,'Bad string'))
				return;																	
			end
			insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,@name,'STRING',@start,@p,@value)
			return
		end

		-- Handle Objects

		if (@kind='{') begin
			insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,@name,'OBJECT',@start,@start+1,'')
			insert into @hierarchy(id,parent,name,kind,ppos,pend,value) select id,parent,name,kind,ppos,pend,value from dbo.json_Object(@id+1,@id,@start+1,@json)
			if exists(select * from @hierarchy where kind='ERROR') return
			
			select @p=max(pend) from @hierarchy
			set @start = dbo.json_SkipUntil(@json,@p,'}')
			if (@start=0) begin
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,'','ERROR',@p,@p,'Expected [}] and have: '+dbo.json_Message(@json,@p))
				return;																	
				end
			update @hierarchy set pend=@start where id=@id
			return
			end
		
		-- Handle Arrays

		if (@kind='[') begin
			insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,@name,'ARRAY',@start,@start+1,'')
			insert into @hierarchy(id,parent,name,kind,ppos,pend,value) select id,parent,name,kind,ppos,pend,value from dbo.json_Array(@id+1,@id,@start+1,@json)
			if exists(select * from @hierarchy where kind='ERROR') return
			
			select @p=max(pend) from @hierarchy
			set @start = dbo.json_SkipUntil(@json,@p,']')
			
			if (@start=0) begin
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,'','ERROR',@p,@p,'Expected []] and have: '+dbo.json_Message(@json,@p))
				return;																	
				end
			update @hierarchy set pend=@start where id=@id
			return
			end
		
		-- Handle NUmbers
		if (@kind='-' or (@kind>='0' and @kind<='9')) begin
			select @p=p2,@value=value from dbo.json_Number(@json,@start)
			if (@p=-1) begin
					insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,'','ERROR',@p,-1,isnull(@value,'Bad number'))
					return;																	
				end
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,@name,'NUMBER',@start,@p,@value)
			return
		end	
		
		-- Handle TRUE
		if (upper(substring(@json,@start,4))='TRUE') begin
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,@name,'BOOL',@start,@start+4,'1')
				return
		end
		
		-- Handle FALSE
		if (upper(substring(@json,@start,5))='FALSE') begin
				insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(@id,@parent,@name,'BOOL',@start,@start+5,'0')
				return
		end
		
		insert into @hierarchy(id,parent,name,kind,ppos,pend,value) values(0,@parent,@name,'ERROR',@start,@start,'Unexpected token '+@kind)
		return		
end

go

