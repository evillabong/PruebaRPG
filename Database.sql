USE PruebaSolicitudes;
GO

IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Audit') AND TYPE IN (N'U')) DROP TABLE [Audit]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Request') AND TYPE IN (N'U')) DROP TABLE [Request]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'UserRole') AND TYPE IN (N'U')) DROP TABLE [UserRole]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'Role') AND TYPE IN (N'U')) DROP TABLE [Role]
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'User') AND TYPE IN (N'U')) DROP TABLE [User]

CREATE TABLE [User] (
    Id INT PRIMARY KEY IDENTITY,
    [Name] NVARCHAR(100),
    [Username] NVARCHAR(50) UNIQUE,
    [Password] NVARCHAR(128), -- SHA-512
);
CREATE TABLE [Role](
	Id INT PRIMARY KEY IDENTITY,
	[Name] NVARCHAR(50) UNIQUE
)
CREATE TABLE [UserRole](
	Id INT PRIMARY KEY IDENTITY,
	UserId INT NOT NULL,
	RoleId INT NOT NULL,

	CONSTRAINT FK_UserRole1 FOREIGN KEY (UserId) REFERENCES [User](Id),
	CONSTRAINT FK_UserRole2 FOREIGN KEY (RoleId) REFERENCES [Role](Id),
)
CREATE TABLE Request (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL, 
    [Description] NVARCHAR(255),
    Amount DECIMAL(18,2),
    AwaitedAt DATETIME,
    [Status] INT NOT NULL DEFAULT 0, -- 0 Pending
    Comment NVARCHAR(500),
    CreatedAt DATETIME DEFAULT GETDATE(),
    ResponsedAt DATETIME,

	CONSTRAINT FK_Request1 FOREIGN KEY (UserId) REFERENCES [User](Id),
);
CREATE TABLE [Audit] (
    Id INT PRIMARY KEY IDENTITY,
    UserId INT,
    CreatedAt DATETIME DEFAULT GETDATE(),
    [Action] NVARCHAR(100),
    Detail NVARCHAR(1000)
);

INSERT INTO [User] ([Name], Username, [Password]) VALUES
('Juan Pérez', 'juan', CONVERT(NVARCHAR(128), HASHBYTES('SHA2_512', '123'), 2)),
('Ana Torres', 'ana', CONVERT(NVARCHAR(128), HASHBYTES('SHA2_512', '123'), 2));


INSERT INTO [Role] ([Name]) VALUES 
('User'),
('Supervisor' )


INSERT INTO [UserRole] (UserId,RoleId ) VALUES
(1,1), (2,2)

INSERT INTO Request (UserId, [Description], Amount, AwaitedAt, [Status], Comment)
VALUES (1, 'Compra de cartuchos de impresora', 120.50, cast('2025-15-06' as datetime), 0, NULL);

INSERT INTO Request (UserId, [Description], Amount, AwaitedAt, [Status], Comment, CreatedAt, ResponsedAt)
VALUES (1, 'Servidor de respaldo para base de datos', 8500.00, cast('2025-20-06' as datetime), 1, 'Aprobado por infraestructura', GETDATE(), GETDATE());

INSERT INTO Request (UserId, [Description], Amount, AwaitedAt, [Status], Comment, CreatedAt, ResponsedAt)
VALUES (2, 'Compra de decoración navideña', 300.00, cast('2025-06-10' as datetime), 2, 'No es prioridad presupuestaria', GETDATE(), GETDATE());


INSERT INTO [Audit] (UserId, [Action], Detail)
VALUES (1, 'Pendiente', 'Monto: 120.50, Fecha Esperada: 2025-15-06');

INSERT INTO [Audit] (UserId, [Action], Detail)
VALUES (2, 'Aprobada', 'SolicitudId: 2, Estado: 1');

INSERT INTO [Audit] (UserId, [Action], Detail)
VALUES (2, 'Rechazada', 'SolicitudId: 3, Estado: 2');
						

GO

--SP PARA MANEJAR CRUD

USE [PruebaSolicitudes]
GO
/****** Object:  StoredProcedure [dbo].[RequestManager]    Script Date: 6/6/2025 01:00:46 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE OR ALTER   PROCEDURE [dbo].[RequestManager]
    @Operation           VARCHAR(1),
	@Mode				 INT,
    @RequestId           INT		   = NULL,
    @UserId              INT		   = NULL,
	@Username            VARCHAR(50)   = NULL,
    @Description         VARCHAR(255)  = NULL,
    @Amount              DECIMAL(18,2) = NULL,
    @AwaitedAt           DATE		   = NULL,
    @Status              INT		   = NULL,
    @Comment             VARCHAR(500)  = NULL,
    @ResultCode          INT		   = NULL OUTPUT,
    @Message             VARCHAR(150)  = NULL OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

declare @w_UserId INT;
declare @w_Amount DECIMAL(18,2);

    IF @Operation = 'C'
    BEGIN
		IF @Mode = 0 
		BEGIN
			select @w_UserId = Id from [User] where  Username = @Username;
			
			IF @w_UserId IS NOT NULL
			BEGIN
				INSERT INTO Request (UserId, [Description], Amount, AwaitedAt, [Status], Comment)
				VALUES (@w_UserId, @Description, @Amount, @AwaitedAt, ISNULL(@Status, 0), @Comment);

				INSERT INTO Audit (UserId, [Action], Detail)
				VALUES (@w_UserId, 'Crear Solicitud', CONCAT('Monto: ', @Amount, ', Fecha Esperada: ', @AwaitedAt));

				SELECT @ResultCode = 0, @Message = 'OK';
				RETURN 0;
			END
			ELSE
			BEGIN
				SELECT @ResultCode = 1, @Message = 'Usuario inválido';
				RETURN 0;
			END
		END
    END
    ELSE IF @Operation = 'R'
    BEGIN
		IF @Mode = 0 -- LEER TODAS LAS SOLICITUDES
		BEGIN
			SELECT 
				R.Id,
				R.UserId,
				U.Username,
				R.[Description],
				R.Amount,
				R.AwaitedAt,
				R.[Status],
				R.Comment,
				R.CreatedAt,
				R.ResponsedAt
			FROM Request R
			left join [User] U on R.UserId = U.Id
				WHERE 
					U.Username = ISNULL(@Username,U.Username) AND
					R.[Status] = ISNULL(@Status , R.[Status]);


			SELECT @ResultCode = 0, @Message = 'OK';
			RETURN 0;
		END
		IF @Mode = 1 -- LEER UNA SOLICITUD
		BEGIN
			SELECT R.Id,
				R.UserId,
				R.[Description],
				R.Amount,
				R.AwaitedAt,
				R.[Status],
				R.Comment,
				R.CreatedAt,
				R.ResponsedAt
				FROM Request  R WHERE Id = @RequestId;
				SELECT @ResultCode = 0, @Message = 'OK';
				RETURN;
		END
		IF @Mode = 2 -- LEER ADUTORIA
		BEGIN		
			SELECT 
				A.Id,	
				A.Detail,
				A.[Action],
				A.UserId,
				A.CreatedAt,
				U.Username
				FROM [Audit] A 
					left join [User] U on A.UserId = U.Id
				WHERE U.Username = ISNULL(@Username,U.Username);

				SELECT @ResultCode = 0, @Message = 'OK';
				RETURN 0;
		END
    END
    ELSE IF @Operation = 'U'
    BEGIN
		IF @Mode = 0 
		BEGIN
			set @Amount = 0;
			select @w_Amount = Amount from Request where Id = @RequestId;

			IF @Status = 1 AND @w_Amount > 5000 AND (LTRIM(RTRIM(ISNULL(@Comment, ''))) = '')
			BEGIN
				SELECT @ResultCode = 98, @Message = 'El comentario es obligatorio para montos mayores a $5000';
				RETURN 1;
			END

			select @w_UserId = Id from [User] where  Username = @Username;
			
			IF @w_UserId IS NULL
			BEGIN
				SELECT @ResultCode = 1, @Message = 'Usuario inválido';
				RETURN 1;
			END

			UPDATE Request
			SET Amount = @Amount,
				AwaitedAt = @AwaitedAt,
				[Status] = @Status,
				Comment = @Comment,
				ResponsedAt = GETDATE()
			WHERE Id = @RequestId;

			INSERT INTO [Audit] (UserId, [Action], Detail)
			VALUES (@w_UserId, 
					CASE WHEN @Status = 1 THEN 'Aprobada' 
						 WHEN @Status = 2 THEN 'Rechazada'
							ELSE 'Pendiente' END ,
					CONCAT('SolicitudId: ', @RequestId, ', Estado: ', @Status));

			SELECT @ResultCode = 0, @Message = 'OK';
			RETURN 0;
		END
    END
    ELSE IF @Operation = 'D'
    BEGIN
		IF @Mode = 0 
		BEGIN
			select @w_UserId = Id from [User] where  Username = @Username;

			IF @w_UserId IS NULL
			BEGIN
				SELECT @ResultCode = 1, @Message = 'Usuario inválido';
				RETURN 1;
			END

			DELETE FROM Request WHERE Id = @RequestId;

			INSERT INTO [Audit] (UserId, [Action], Detail)
			VALUES (@w_UserId, 'Eliminar Solicitud', CONCAT('SolicitudId: ', @RequestId));

			SELECT @ResultCode = 0, @Message = 'OK';
			RETURN 0;
		END
    END
    ELSE
    BEGIN
        SELECT @ResultCode = 99, @Message = 'Operación inválida';
        RETURN 1;
    END
END;
