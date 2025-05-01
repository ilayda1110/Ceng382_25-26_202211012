CREATE TABLE Classes (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(255) NOT NULL,
    PersonCount INT NOT NULL,
    Description NVARCHAR(MAX),
    IsActive BIT NOT NULL
);