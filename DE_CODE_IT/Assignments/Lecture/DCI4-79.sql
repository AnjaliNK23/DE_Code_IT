USE WideWorldImporters
Go

Create Table [DecodeIT] (
[Student ID ][int] NOT NUll,
[Student FirstName][Varchar](30) NOT NULL,
[Student LastName][varchar](30) NOT NULL,
[Student Street Address] [varchar](60) NOT NULL,
[Student City] [Varchar](30) NOT NULL,
[Student State][Varchar](30) NOT NULL,
[Student Zip][int] NOT  NULL 
Constraint [PK_DecodeIT] Primary key Clustered (
[Student ID] ASC 

	)
)

Insert Into [dbo].[DecodeIT]
Values(1,'Naganjali','Kotaprolu','1234 41st street','Omaha','Nebraska','12345')
Insert Into [dbo].[DecodeIT]
Values(2,'Amitabh','Bacchan','5678 41st street','San Ramon','California','11110');
Insert Into [dbo].[DecodeIT]
Values(3,'Arjun','Allu','3456 145th street','Kansas','Missouri','95468');
Insert Into [dbo].[DecodeIT]
Values(4, 'Sneha','Reddy','91011 144th street','Omaha','Nebraska','68102');
Insert Into [dbo].[DecodeIT]
Values(5, 'Rakul','Singh','1213 147th street','Omaha','Nebraska','68321');

Select * from DecodeIT



