IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FlashcardSets] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(max) NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [CreatedDate] datetime2 NOT NULL,
    [ReviewInterval] int NOT NULL,
    [NextReviewDate] datetime2 NOT NULL,
    [ImagePath] nvarchar(max) NOT NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_FlashcardSets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FlashcardSets_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Flashcards] (
    [Id] int NOT NULL IDENTITY,
    [Term] nvarchar(max) NOT NULL,
    [Definition] nvarchar(max) NOT NULL,
    [IsFamiliar] bit NOT NULL,
    [ImagePath] nvarchar(max) NOT NULL,
    [FlashcardSetId] int NOT NULL,
    CONSTRAINT [PK_Flashcards] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Flashcards_FlashcardSets_FlashcardSetId] FOREIGN KEY ([FlashcardSetId]) REFERENCES [FlashcardSets] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Flashcards_FlashcardSetId] ON [Flashcards] ([FlashcardSetId]);
GO

CREATE INDEX [IX_FlashcardSets_UserId] ON [FlashcardSets] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241106035646_SetupTables', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241107050412_UpdateUserModel', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Description', N'ImagePath', N'Name', N'NextReviewDate', N'ReviewInterval', N'UserId') AND [object_id] = OBJECT_ID(N'[FlashcardSets]'))
    SET IDENTITY_INSERT [FlashcardSets] ON;
INSERT INTO [FlashcardSets] ([Id], [CreatedDate], [Description], [ImagePath], [Name], [NextReviewDate], [ReviewInterval], [UserId])
VALUES (1, '2024-11-09T00:43:03.0563617Z', N'Essential Spanish words for beginners', N'/images/gre.jpg', N'GRE verbal vocabs', '2024-11-09T00:43:03.0563622Z', -1, N'b0dbe1be-79c1-4ea8-aeb8-56ca5314922e'),
(2, '2024-11-08T19:43:03.0563628-05:00', N'Linear Algebra Crash Course', N'/images/linear_algebra.jpg', N'Linear Algebra', '2024-11-08T19:43:03.0563682-05:00', -1, N'b0dbe1be-79c1-4ea8-aeb8-56ca5314922e');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'CreatedDate', N'Description', N'ImagePath', N'Name', N'NextReviewDate', N'ReviewInterval', N'UserId') AND [object_id] = OBJECT_ID(N'[FlashcardSets]'))
    SET IDENTITY_INSERT [FlashcardSets] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Definition', N'FlashcardSetId', N'ImagePath', N'IsFamiliar', N'Term') AND [object_id] = OBJECT_ID(N'[Flashcards]'))
    SET IDENTITY_INSERT [Flashcards] ON;
INSERT INTO [Flashcards] ([Id], [Definition], [FlashcardSetId], [ImagePath], [IsFamiliar], [Term])
VALUES (1, N'to reduce in amount, degree, or severity', 1, N'/images/gre.jpg', CAST(0 AS bit), N'abate'),
(2, N'deviating from the norm', 1, N'/images/gre.jpg', CAST(0 AS bit), N'aberrant'),
(3, N'temporary suppression or suspension', 1, N'/images/gre.jpg', CAST(0 AS bit), N'abeyance'),
(4, N'to reject; abandon formally', 1, N'/images/gre.jpg', CAST(0 AS bit), N'abjure'),
(5, N'to abolish, usually by authority', 1, N'/images/gre.jpg', CAST(0 AS bit), N'abrogate'),
(6, N'to leave hurriedly and secretly, typically to avoid detection or arrest', 1, N'/images/gre.jpg', CAST(0 AS bit), N'abscond'),
(7, N'sparing in eating and drinking; temperate', 1, N'/images/gre.jpg', CAST(0 AS bit), N'abstemious'),
(8, N'to caution or advise against something; to scold mildly; to remind of a duty', 1, N'/images/gre.jpg', CAST(0 AS bit), N'admonish'),
(9, N'A matrix is a collection of numbers arranged into a fixed number of rows and columns.', 2, N'/images/linear_algebra.jpg', CAST(0 AS bit), N'Matrix'),
(10, N'A determinant is a scalar value derived from a square matrix.', 2, N'/images/linear_algebra.jpg', CAST(0 AS bit), N'Determinant'),
(11, N'An eigenvalue is a scalar that is a special set of scalars associated with a linear system of equations.', 2, N'/images/linear_algebra.jpg', CAST(0 AS bit), N'Eigenvalue'),
(12, N'An eigenvector is a nonzero vector that stays in the same direction after a linear transformation.', 2, N'/images/linear_algebra.jpg', CAST(0 AS bit), N'Eigenvector'),
(13, N'The transpose of a matrix is a new matrix whose rows are the columns of the original.', 2, N'/images/linear_algebra.jpg', CAST(0 AS bit), N'Transpose');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Definition', N'FlashcardSetId', N'ImagePath', N'IsFamiliar', N'Term') AND [object_id] = OBJECT_ID(N'[Flashcards]'))
    SET IDENTITY_INSERT [Flashcards] OFF;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241109004303_SeedData', N'8.0.10');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[FlashcardSets]') AND [c].[name] = N'ImagePath');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [FlashcardSets] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [FlashcardSets] ALTER COLUMN [ImagePath] nvarchar(max) NULL;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Flashcards]') AND [c].[name] = N'ImagePath');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Flashcards] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Flashcards] ALTER COLUMN [ImagePath] nvarchar(max) NULL;
GO

UPDATE [FlashcardSets] SET [CreatedDate] = '2024-11-17T16:46:17.9374062Z', [NextReviewDate] = '2024-11-17T16:46:17.9374065Z'
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [FlashcardSets] SET [CreatedDate] = '2024-11-17T16:46:17.9374068Z', [NextReviewDate] = '2024-11-17T16:46:17.9374069Z'
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241117164618_MakeImagePathNullable', N'8.0.10');
GO

COMMIT;
GO

