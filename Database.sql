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
    AwaitedAt DATE,
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
						

GO

--SP PARA MANEJAR CRUD

 
CREATE OR ALTER PROCEDURE PersonManager
    @Operation           VARCHAR(10),
	@Mode				 INT,
    @RequestId           INT = NULL,
    @UserId              INT = NULL,
    @Description         NVARCHAR(255) = NULL,
    @Amount              DECIMAL(18,2) = NULL,
    @AwaitedAt           DATE = NULL,
    @Status              INT = NULL,
    @Comment             NVARCHAR(500) = NULL,
    @ResultCode          INT OUTPUT,
    @Message             VARCHAR(150) OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    IF @Operation = 'C'
    BEGIN
		IF @Mode = 0 
		BEGIN
			INSERT INTO Request (UserId, [Description], Amount, AwaitedAt, [Status], Comment)
			VALUES (@UserId, @Description, @Amount, @AwaitedAt, ISNULL(@Status, 0), @Comment);

			INSERT INTO Audit (UserId, [Action], Detail)
			VALUES (@UserId, 'Crear Solicitud', CONCAT('Monto: ', @Amount, ', Fecha Esperada: ', @AwaitedAt));

			SELECT @ResultCode = 0, @Message = 'OK';
			RETURN 0;
		END
    END
    ELSE IF @Operation = 'R'
    BEGIN
		IF @Mode = 0 -- LEER TODAS LAS SOLICITUDES
		BEGIN
			SELECT * FROM Request;
			SELECT @ResultCode = 0, @Message = 'OK';
			RETURN 0;
		END
		IF @Mode = 0 -- LEER UNA SOLICITUD
		BEGIN
			SELECT * FROM Request WHERE Id = @RequestId;
				SELECT @ResultCode = 0, @Message = 'OK';
				RETURN 0;
		END
    END
    ELSE IF @Operation = 'U'
    BEGIN
		IF @Mode = 0 
		BEGIN
			IF @Status = 1 AND @Amount > 5000 AND (LTRIM(RTRIM(ISNULL(@Comment, ''))) = '')
			BEGIN
				SELECT @ResultCode = 98, @Message = 'El comentario es obligatorio para montos mayores a $5000';
				RETURN 1;
			END

			UPDATE Request
			SET [Description] = @Description,
				Amount = @Amount,
				AwaitedAt = @AwaitedAt,
				[Status] = @Status,
				Comment = @Comment,
				ResponsedAt = GETDATE()
			WHERE Id = @RequestId;

			INSERT INTO [Audit] (UserId, [Action], Detail)
			VALUES (@UserId, 'Actualizar Solicitud', CONCAT('SolicitudId: ', @RequestId, ', Estado: ', @Status));

			SELECT @ResultCode = 0, @Message = 'OK';
			RETURN 0;
		END
    END
    ELSE IF @Operation = 'D'
    BEGIN
		IF @Mode = 0 
		BEGIN
			DELETE FROM Request WHERE Id = @RequestId;

			INSERT INTO [Audit] (UserId, [Action], Detail)
			VALUES (@UserId, 'Eliminar Solicitud', CONCAT('SolicitudId: ', @RequestId));

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
GO