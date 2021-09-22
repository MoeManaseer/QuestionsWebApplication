IF DB_ID('TempQuestionsDatabase') IS NULL
	BEGIN
		CREATE DATABASE [TempQuestionsDatabase]
	END
GO

USE [TempQuestionsDatabase]
GO

IF OBJECT_ID(N'[dbo].[QuestionsState]', N'U') IS NULL  
	BEGIN
		CREATE TABLE [dbo].[QuestionsState](
			[CurrentState] [int] NOT NULL
		) ON [PRIMARY];

		ALTER TABLE [dbo].[QuestionsState] ADD  CONSTRAINT [DF_QuestionsState_Id]  DEFAULT ((0)) FOR [CurrentState];
		
		DECLARE @QuestionsStateCount INT;
		SET @QuestionsStateCount = (SELECT COUNT([CurrentState]) FROM QuestionsState);

		IF (@QuestionsStateCount = 0)
			INSERT INTO QuestionsState VALUES(0);
	END
GO

IF OBJECT_ID(N'[dbo].[AllQuestions]', N'U') IS NULL  
		CREATE TABLE [dbo].[AllQuestions](
			[Id] [int] IDENTITY(1,1) NOT NULL,
			[Type] [nvarchar](50) NOT NULL,
			[Order] [TINYINT] NOT NULL,
			[Text] [nvarchar](255) NOT NULL,
		 CONSTRAINT [PK_AllQuestions] PRIMARY KEY CLUSTERED 
		(
			[Id] ASC
		)
		) ON [PRIMARY]
GO

IF OBJECT_ID(N'[dbo].[Order_constraint]') IS NULL  
		ALTER TABLE AllQuestions
		ADD CONSTRAINT Order_constraint CHECK ([Order] >= 0 AND [Order] <= 100);
GO

IF OBJECT_ID(N'[dbo].[SliderQuestions]', N'U') IS NULL 
	BEGIN
			CREATE TABLE [dbo].[SliderQuestions](
				[Id] [int] NOT NULL,
				[StartValue] [TINYINT] NOT NULL,
				[EndValue] [TINYINT] NOT NULL,
				[StartValueCaption] [nvarchar](250) NOT NULL,
				[EndValueCaption] [nvarchar](250) NOT NULL,
			 CONSTRAINT [IX_SliderQuestions] UNIQUE NONCLUSTERED 
			(
				[Id] ASC
			)
			) ON [PRIMARY];

			ALTER TABLE [dbo].[SliderQuestions]  WITH CHECK ADD  CONSTRAINT [FK_SliderQuestions_AllQuestions] FOREIGN KEY([Id])
			REFERENCES [dbo].[AllQuestions] ([Id])

			ALTER TABLE [dbo].[SliderQuestions] CHECK CONSTRAINT [FK_SliderQuestions_AllQuestions]
	END
GO

IF OBJECT_ID(N'[dbo].[Values_constraints]') IS NULL  
		ALTER TABLE [SliderQuestions]
		ADD CONSTRAINT Values_constraints CHECK (StartValue <= EndValue AND StartValue >= 0 AND StartValue <= 100 AND EndValue >= 0 AND EndValue <= 100);
GO

IF OBJECT_ID(N'[dbo].[SmileyQuestions]', N'U') IS NULL  
	BEGIN
		CREATE TABLE [dbo].[SmileyQuestions](
			[Id] [int] NOT NULL,
			[NumberOfSmiley] [TINYINT] NOT NULL,
		 CONSTRAINT [IX_SmileyQuestions] UNIQUE NONCLUSTERED 
		(
			[Id] ASC
		)
		) ON [PRIMARY]

		ALTER TABLE [dbo].[SmileyQuestions]  WITH CHECK ADD  CONSTRAINT [FK_SmileyQuestions_AllQuestions] FOREIGN KEY([Id])
		REFERENCES [dbo].[AllQuestions] ([Id])

		ALTER TABLE [dbo].[SmileyQuestions] CHECK CONSTRAINT [FK_SmileyQuestions_AllQuestions]
	END
GO

IF OBJECT_ID(N'[dbo].[SmileyNumber_constraints]') IS NULL  
		ALTER TABLE SmileyQuestions
		ADD CONSTRAINT SmileyNumber_constraints CHECK (NumberOfSmiley >= 2 AND NumberOfSmiley <= 5);
GO

IF OBJECT_ID(N'[dbo].[StarQuestions]', N'U') IS NULL  
	BEGIN
		CREATE TABLE [dbo].[StarQuestions](
			[Id] [int] NOT NULL,
			[NumberOfStar] [TINYINT] NOT NULL,
		 CONSTRAINT [IX_StarQuestions] UNIQUE NONCLUSTERED 
		(
			[Id] ASC
		)
		) ON [PRIMARY]

		ALTER TABLE [dbo].[StarQuestions]  WITH CHECK ADD  CONSTRAINT [FK_StarQuestions_AllQuestions] FOREIGN KEY([Id])
		REFERENCES [dbo].[AllQuestions] ([Id])

		ALTER TABLE [dbo].[StarQuestions] CHECK CONSTRAINT [FK_StarQuestions_AllQuestions]
	END
GO

IF OBJECT_ID(N'[dbo].[StarNumber_constraint]') IS NULL  
		ALTER TABLE StarQuestions
		ADD CONSTRAINT StarNumber_constraint CHECK (NumberOfStar >= 1 AND NumberOfStar <= 10);
GO


CREATE OR ALTER PROCEDURE [dbo].Update_CurrentState
	AS
	BEGIN
		BEGIN TRY	
			BEGIN TRAN
			UPDATE QuestionsState SET [CurrentState] = [CurrentState] + 1;

			DECLARE @QuestionsStateCount INT;
			SET @QuestionsStateCount = (SELECT MAX([CurrentState]) FROM QuestionsState);

			IF (@QuestionsStateCount = 2147483647)
				UPDATE QuestionsState SET [CurrentState] = 0;
			COMMIT TRAN
		END TRY
		BEGIN CATCH
			ROLLBACK TRAN
			THROW;
		END CATCH
	END
GO

CREATE OR ALTER PROCEDURE [dbo].Add_StarQuestions
 (@Text nvarchar(250), @Order INT, @NumberOfStar INT, @Id INT = NULL OUTPUT)
 AS  
 BEGIN
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;

			-- Insert the new question in the main table
			INSERT INTO AllQuestions VALUES('Star', @Order, @Text);
			
			-- Gets the newly created question Id
			SET @Id = SCOPE_IDENTITY();

			-- Inserts the new question into the StarQuestions table
			INSERT INTO StarQuestions VALUES(@Id, @NumberOfStar);

			-- Call the update_CurrentState procedure to notify for changes
			EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN
		THROW;
	END CATCH
END  
GO

CREATE OR ALTER PROCEDURE [dbo].Add_SliderQuestions  
 (    @Text nvarchar(250),    @Order  INT,    @StartValue INT,    @EndValue INT,    @StartValueCaption nvarchar(250),    @EndValueCaption nvarchar(250), @Id INT = NULL OUTPUT)  
 AS  
 BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;

			-- Insert the new question in the main table
			INSERT INTO AllQuestions VALUES('Slider', @Order, @Text);
			
			-- Gets the newly created question Id
			SET @Id = SCOPE_IDENTITY();
			
			-- Inserts the new question into the SliderQuestions table
			INSERT INTO SliderQuestions VALUES(@Id, @StartValue, @EndValue, @StartValueCaption, @EndValueCaption);

			-- Call the update_CurrentState procedure to notify for changes
			EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END 
GO

CREATE OR ALTER PROCEDURE [dbo].Add_SmileyQuestions  
 (    @Text nvarchar(255),    @Order  INT,    @NumberOfSmiley INT, @Id INT = NULL OUTPUT)  
 AS  
 BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;
			
			-- Insert the new question in the main table
			INSERT INTO AllQuestions VALUES('Smiley', @Order, @Text);
			
			-- Gets the newly created question Id
			SET @Id = SCOPE_IDENTITY();
			
			-- Inserts the new question into the SliderQuestions table
			INSERT INTO SmileyQuestions VALUES(@Id, @NumberOfSmiley);

			-- Call the update_CurrentState procedure to notify for changes
			EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END  
GO

CREATE OR ALTER PROCEDURE [dbo].Update_StarQuestions  
(    @Text nvarchar(255),    @Order  INT,    @NumberOfStar INT,    @Id INT  )  
AS  
BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;
			
			-- Set the totlaRows variable to track number of rows changed
			DECLARE @totalRows INT;
			SET @totalRows = 0;
			
			-- Update the question in the main table with new data
			UPDATE AllQuestions SET Text = @Text, [Order] = @Order WHERE Id = @Id AND Type = 'Star'; 
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT
			
			-- Updates the question in the StarQuestions with new data
			UPDATE StarQuestions SET NumberOfStar = @NumberOfStar WHERE Id = @Id;   
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT

			-- If affected rows = 2, update the currentState with new values sine the
			-- question got updated successfuly
			if (@totalRows = 2)
				EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END 
GO

CREATE OR ALTER PROCEDURE [dbo].Update_SliderQuestions  
(    @Text nvarchar(250),    @Order  INT,    @StartValue INT,    @EndValue INT,    @StartValueCaption nvarchar(250),    @EndValueCaption nvarchar(250),    @Id INT  )
AS  
BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;

			-- Set the totlaRows variable to track number of rows changed
			DECLARE @totalRows INT;
			SET @totalRows = 0;
			
			-- Update the question in the main table with new data
			UPDATE AllQuestions SET Text = @Text, [Order] = @Order WHERE Id = @Id AND Type = 'Slider'; 
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT
			
			-- Updates the question in the SliderQuestions with new data
			UPDATE SliderQuestions SET StartValue = @StartValue, EndValue = @EndValue, StartValueCaption = @StartValueCaption, EndValueCaption = @EndValueCaption WHERE Id = @Id;   
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT

			-- If affected rows = 2, update the currentState with new values sine the
			-- question got updated successfuly
			if (@totalRows = 2)
				EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].Update_SmileyQuestions  
(    @Text nvarchar(255),    @Order  INT,    @NumberOfSmiley INT,    @Id INT  )  
AS  
BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;      
			
			-- Set the totlaRows variable to track number of rows changed
			DECLARE @totalRows INT;
			SET @totalRows = 0;

			-- Update the question in the main table with new data
			UPDATE AllQuestions SET Text = @Text, [Order] = @Order WHERE Id = @Id AND Type = 'Smiley';  
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT
			
			-- Updates the question in the SmileyQuestions with new data
			UPDATE SmileyQuestions SET NumberOfSmiley = @NumberOfSmiley WHERE Id = @Id;   
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT

			-- If affected rows = 2, update the currentState with new values sine the
			-- question got updated successfuly
			if (@totalRows = 2)
				EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END 
GO

CREATE OR ALTER PROCEDURE [dbo].Delete_StarQuestions  
(    @Id INT  )
AS  
BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;
			
			-- Set the totlaRows variable to track number of rows changed
			DECLARE @totalRows INT;
			SET @totalRows = 0;
			
			-- Deletes a question from the StarQuestions table
			DELETE FROM StarQuestions WHERE Id = @Id;   
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT
			
			-- Deletes a question from the main questions table
			DELETE FROM AllQuestions WHERE Id = @Id AND Type = 'Star';
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT
			
			-- If affected rows = 2, update the currentState with new values sine the
			-- question got updated successfuly
			if (@totalRows = 2)
				EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END  
GO

CREATE OR ALTER PROCEDURE [dbo].Delete_SliderQuestions
(    @Id INT  )  
AS  
BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;

			-- Set the totlaRows variable to track number of rows changed
			DECLARE @totalRows INT;
			SET @totalRows = 0;
			
			-- Deletes a question from the SliderQuestions table
			DELETE FROM SliderQuestions WHERE Id = @Id;   
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT
			
			-- Deletes a question from the main questions table
			DELETE FROM AllQuestions WHERE Id = @Id AND Type = 'Slider';  
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT

			-- If affected rows = 2, update the currentState with new values sine the
			-- question got updated successfuly
			if (@totalRows = 2)
				EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END
GO

CREATE OR ALTER PROCEDURE [dbo].Delete_SmileyQuestions  
(    @Id INT  )
AS  
BEGIN   
	BEGIN TRY	
		BEGIN TRAN
			SET XACT_ABORT ON;
			
			-- Set the totlaRows variable to track number of rows changed
			DECLARE @totalRows INT;
			SET @totalRows = 0;

			-- Deletes a question from the SmileyQuestions table
			DELETE FROM SmileyQuestions WHERE Id = @Id;
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT

			-- Deletes a question from the main questions table
			DELETE FROM AllQuestions WHERE Id = @Id AND Type = 'Smiley';  
			-- Add the number of rows affected
			SET @totalRows = @totalRows + @@ROWCOUNT

			-- If affected rows = 2, update the currentState with new values sine the
			-- question got updated successfuly
			if (@totalRows = 2)
				EXEC [dbo].Update_CurrentState;
		COMMIT TRAN
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN;
		THROW;
	END CATCH
END  
GO

CREATE OR ALTER PROCEDURE [dbo].Get_SliderQuestions
	(@Id INT)
	AS
	BEGIN
		SELECT AllQuestions.Type, AllQuestions.[Order], AllQuestions.Text, SliderQuestions.StartValue, SliderQuestions.EndValue, SliderQuestions.StartValueCaption, SliderQuestions.EndValueCaption
		FROM AllQuestions
		INNER JOIN SliderQuestions ON AllQuestions.Id = SliderQuestions.Id
		WHERE AllQuestions.Id = @Id;
	END
GO

CREATE OR ALTER PROCEDURE [dbo].Get_SmileyQuestions
	(@Id INT)
	AS
	BEGIN
		SELECT AllQuestions.Type, AllQuestions.[Order], AllQuestions.Text, SmileyQuestions.NumberOfSmiley
		FROM AllQuestions
		INNER JOIN SmileyQuestions ON AllQuestions.Id = SmileyQuestions.Id
		WHERE AllQuestions.Id = @Id;
	END
GO

CREATE OR ALTER PROCEDURE [dbo].Get_StarQuestions
	(@Id INT)
	AS
	BEGIN
		SELECT AllQuestions.Type, AllQuestions.[Order], AllQuestions.Text, StarQuestions.NumberOfStar
		FROM AllQuestions
		INNER JOIN StarQuestions ON AllQuestions.Id = StarQuestions.Id
		WHERE AllQuestions.Id = @Id;
	END
GO
