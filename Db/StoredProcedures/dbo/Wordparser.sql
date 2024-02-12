CREATE FUNCTION [dbo].[Wordparser]
(
  @multiwordstring VARCHAR(255),
  @wordnumber      NUMERIC
)
returns VARCHAR(255)
AS
  BEGIN
      DECLARE @remainingstring VARCHAR(255)
      SET @remainingstring=@multiwordstring

      DECLARE @numberofwords NUMERIC
      SET @numberofwords=(LEN(@remainingstring) - LEN(REPLACE(@remainingstring, ',', '')) + 1)

      DECLARE @word VARCHAR(50)
      DECLARE @parsedwords TABLE
      (
         line NUMERIC IDENTITY(1, 1),
         word VARCHAR(255)
      )

      WHILE @numberofwords > 1
        BEGIN
            SET @word=LEFT(@remainingstring, CHARINDEX(',', @remainingstring) - 1)

            INSERT INTO @parsedwords(word)
            SELECT @word

            SET @remainingstring= REPLACE(@remainingstring, Concat(@word, ','), '')
            SET @numberofwords=(LEN(@remainingstring) - LEN(REPLACE(@remainingstring, ',', '')) + 1)

            IF @numberofwords = 1
              BREAK

            ELSE
              CONTINUE
        END

      IF @numberofwords = 1
        SELECT @word = @remainingstring
      INSERT INTO @parsedwords(word)
      SELECT @word

      RETURN
        (SELECT word
         FROM   @parsedwords
         WHERE  line = @wordnumber)

  END
go

