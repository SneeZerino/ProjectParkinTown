DROP DATABASE Parkhaus;
CREATE DATABASE Parkhaus;

USE Parkhaus;

-- Tables
CREATE TABLE Parkhaus (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name varchar(100) NOT NULL,
	Ort varchar(100) NOT NULL,
	Tagestarif FLOAT,
	Monatsmiete FLOAT
)

CREATE TABLE Tarif (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Typ BIT,
	Von TIME,
	Bis TIME,
	Preis FLOAT,
	Parkhaus_Id INT NOT NULL FOREIGN KEY REFERENCES Parkhaus(Id)
)

CREATE TABLE Stockwerk (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Name varchar(100) NOT NULL,
	AnzahlParkplaetze INT,
	Parkhaus_Id INT NOT NULL FOREIGN KEY References Parkhaus(Id)
)



CREATE TABLE Dauermieter (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ParkplatzNummer INT NOT NULL,
	Stockwerk_Id INT NOT NULL FOREIGN KEY References Stockwerk(Id),
	Name varchar(100) NOT NULL,
	Code varchar(100) NOT NULL,
	Gesperrt BIT Default 0
)

CREATE TABLE Ticket (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	ParkplatzNummer INT NOT NULL,
	Stockwerk_Id INT NOT NULL FOREIGN KEY References Stockwerk(Id),
	Eintrittszeit DATETIME,
	Bezahlt BIT Default 0,
	BezahltZeit DATETIME,
	Benutzt BIT Default 0
)

CREATE TABLE Log (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Event BIT NOT NULL,
	Typ BIT NOT NULL,
	Dauermieter_Id INT FOREIGN KEY REFERENCES Dauermieter(Id),
	Zeit DATETIME,
	ParkplatzNummer INT NOT NULL,
	Stockwerk_Id INT NOT NULL FOREIGN KEY References Stockwerk(Id)
)

CREATE TABLE Zahlung (
	Id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	Typ BIT NOT NULL,
	Kosten FLOAT,
	Zeit DATETIME,
	Ticket_Id INT FOREIGN KEY REFERENCES Ticket(Id),
	Dauermieter_Id INT FOREIGN KEY REFERENCES Dauermieter(Id),
	Stockwerk_Id INT NOT NULL FOREIGN KEY References Stockwerk(Id)
)


-- Test Parkhäuser
INSERT INTO Parkhaus(Name, Ort, Tagestarif, Monatsmiete) VALUES
('Parkhaus City West', 'Bern', 40, 200),
('Parkhaus Casino', 'Zürich', 40, 200)

-- Test Stockwerke
INSERT INTO Stockwerk (Name, AnzahlParkplaetze, Parkhaus_Id) VALUES 
(N'EG', 30, 1),
(N'1. OG', 30, 1),
(N'EG', 25, 2),
(N'1. OG', 20, 2)


-- Test Dauermieter
INSERT INTO Dauermieter (Stockwerk_Id, ParkplatzNummer, Name, Code, Gesperrt) VALUES
(1, 5, 'Max Muster', '1111', 0), 
(2, 100, 'Adam Adonis', '2222', 0),
(3, 5, 'Eva Adonis', '3333', 1),
(4, 100, 'Martin Morgenstern', '4444', 0)

-- Test Zahlungen Dauermieter
INSERT INTO Zahlung (Typ, Zeit, Dauermieter_Id, Stockwerk_Id, Kosten) VALUES
(1, '2022-12-10 11:00:20', (SELECT Id FROM Dauermieter WHERE Code = '1111'), 1, 200.0),
(1, '2023-01-10 11:25:20', (SELECT Id FROM Dauermieter WHERE Code = '1111'), 1, 200.0),
(1, '2023-02-10 11:35:05', (SELECT Id FROM Dauermieter WHERE Code = '1111'), 1, 200.0),
(1, '2023-03-10 11:45:15', (SELECT Id FROM Dauermieter WHERE Code = '1111'), 1, 200.0),
(1, '2022-12-05 11:00:20', (SELECT Id FROM Dauermieter WHERE Code = '2222'), 1, 200.0),
(1, '2023-01-05 12:25:20', (SELECT Id FROM Dauermieter WHERE Code = '2222'), 1, 200.0),
(1, '2023-02-05 06:35:05', (SELECT Id FROM Dauermieter WHERE Code = '2222'), 2, 200.0),
(1, '2023-01-12 09:35:05', (SELECT Id FROM Dauermieter WHERE Code = '4444'), 3, 200.0),
(1, '2023-02-12 10:35:05', (SELECT Id FROM Dauermieter WHERE Code = '4444'), 3, 200.0),
(1, '2023-03-12 07:35:05', (SELECT Id FROM Dauermieter WHERE Code = '4444'), 3, 200.0)

-- Tarife für Parkhaus 1
INSERT INTO Tarif (Typ, Von, Bis, Preis, Parkhaus_Id) VALUES 
(0, CAST(N'00:00:00' AS Time), CAST(N'05:59:59' AS Time), 3, 1),
(0, CAST(N'06:00:00' AS Time), CAST(N'08:59:59' AS Time), 3.4, 1),
(0, CAST(N'09:00:00' AS Time), CAST(N'17:59:59' AS Time), 4.2, 1),
(0, CAST(N'18:00:00' AS Time), CAST(N'20:59:59' AS Time), 3.4, 1),
(0, CAST(N'21:00:00' AS Time), CAST(N'23:59:59' AS Time), 3, 1),
(1, CAST(N'00:00:00' AS Time), CAST(N'08:59:59' AS Time), 3, 1),
(1, CAST(N'09:00:00' AS Time), CAST(N'17:59:59' AS Time), 3.8, 1),
(1, CAST(N'18:00:00' AS Time), CAST(N'23:59:59' AS Time), 3, 1)

-- Tarife für Parkhaus 2
INSERT INTO Tarif (Typ, Von, Bis, Preis, Parkhaus_Id) VALUES 
(0, CAST(N'00:00:00' AS Time), CAST(N'05:59:59' AS Time), 3.2, 2),
(0, CAST(N'06:00:00' AS Time), CAST(N'08:59:59' AS Time), 3, 2),
(0, CAST(N'09:00:00' AS Time), CAST(N'17:59:59' AS Time), 4, 2),
(0, CAST(N'18:00:00' AS Time), CAST(N'20:59:59' AS Time), 3.8, 2),
(0, CAST(N'21:00:00' AS Time), CAST(N'23:59:59' AS Time), 3, 2),
(1, CAST(N'00:00:00' AS Time), CAST(N'08:59:59' AS Time), 3.5, 2),
(1, CAST(N'09:00:00' AS Time), CAST(N'17:59:59' AS Time), 3.6, 2),
(1, CAST(N'18:00:00' AS Time), CAST(N'23:59:59' AS Time), 3.1, 2)