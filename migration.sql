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
CREATE TABLE [Users] (
    [Id] int NOT NULL IDENTITY,
    [Username] nvarchar(100) NOT NULL,
    [PasswordHash] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id])
);

CREATE TABLE [Categories] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Type] int NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Categories] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Categories_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [SavingsGoals] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [TargetAmount] decimal(18,2) NOT NULL,
    [CurrentAmount] decimal(18,2) NOT NULL,
    [TargetDate] datetime2 NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_SavingsGoals] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_SavingsGoals_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Wallets] (
    [Id] int NOT NULL IDENTITY,
    [Name] nvarchar(50) NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UserId] int NOT NULL,
    CONSTRAINT [PK_Wallets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Wallets_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Budgets] (
    [Id] int NOT NULL IDENTITY,
    [CategoryId] int NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Month] int NOT NULL,
    [Year] int NOT NULL,
    [UserId] int NOT NULL,
    [UserId1] int NULL,
    CONSTRAINT [PK_Budgets] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Budgets_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Budgets_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Budgets_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id])
);

CREATE TABLE [RecurringTransactions] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(100) NOT NULL,
    [Amount] decimal(18,2) NOT NULL,
    [Type] int NOT NULL,
    [WalletId] int NOT NULL,
    [CategoryId] int NULL,
    [DayOfMonth] int NOT NULL,
    [LastProcessedDate] datetime2 NULL,
    [UserId] int NOT NULL,
    [UserId1] int NULL,
    CONSTRAINT [PK_RecurringTransactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_RecurringTransactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]),
    CONSTRAINT [FK_RecurringTransactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_RecurringTransactions_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_RecurringTransactions_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id]) ON DELETE CASCADE
);

CREATE TABLE [Transactions] (
    [Id] int NOT NULL IDENTITY,
    [Amount] decimal(18,2) NOT NULL,
    [Type] int NOT NULL,
    [Description] nvarchar(max) NOT NULL,
    [Date] datetime2 NOT NULL,
    [WalletId] int NOT NULL,
    [CategoryId] int NULL,
    [UserId] int NOT NULL,
    [UserId1] int NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Transactions_Categories_CategoryId] FOREIGN KEY ([CategoryId]) REFERENCES [Categories] ([Id]),
    CONSTRAINT [FK_Transactions_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Transactions_Users_UserId1] FOREIGN KEY ([UserId1]) REFERENCES [Users] ([Id]),
    CONSTRAINT [FK_Transactions_Wallets_WalletId] FOREIGN KEY ([WalletId]) REFERENCES [Wallets] ([Id]) ON DELETE CASCADE
);

CREATE INDEX [IX_Budgets_CategoryId] ON [Budgets] ([CategoryId]);

CREATE INDEX [IX_Budgets_UserId] ON [Budgets] ([UserId]);

CREATE INDEX [IX_Budgets_UserId1] ON [Budgets] ([UserId1]);

CREATE INDEX [IX_Categories_UserId] ON [Categories] ([UserId]);

CREATE INDEX [IX_RecurringTransactions_CategoryId] ON [RecurringTransactions] ([CategoryId]);

CREATE INDEX [IX_RecurringTransactions_UserId] ON [RecurringTransactions] ([UserId]);

CREATE INDEX [IX_RecurringTransactions_UserId1] ON [RecurringTransactions] ([UserId1]);

CREATE INDEX [IX_RecurringTransactions_WalletId] ON [RecurringTransactions] ([WalletId]);

CREATE INDEX [IX_SavingsGoals_UserId] ON [SavingsGoals] ([UserId]);

CREATE INDEX [IX_Transactions_CategoryId] ON [Transactions] ([CategoryId]);

CREATE INDEX [IX_Transactions_UserId] ON [Transactions] ([UserId]);

CREATE INDEX [IX_Transactions_UserId1] ON [Transactions] ([UserId1]);

CREATE INDEX [IX_Transactions_WalletId] ON [Transactions] ([WalletId]);

CREATE INDEX [IX_Wallets_UserId] ON [Wallets] ([UserId]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20261025191604_InitialWithUser', N'10.0.13');

COMMIT;
GO

