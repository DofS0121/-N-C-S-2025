--CREATE DATABASE VivoWeb_7;
--GO
--USE VivoWeb_7;
-- Tạo bảng Users
CREATE TABLE Users (
    UserId INT PRIMARY KEY IDENTITY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    FullName NVARCHAR(100),
    Email NVARCHAR(100),
    Phone NVARCHAR(20),
    Role NVARCHAR(20) DEFAULT 'Customer',
    GoogleId NVARCHAR(255) NULL,
    Provider NVARCHAR(50) NULL,
    Image NVARCHAR(255)
);
go
-- Tạo bảng TourCategories
CREATE TABLE TourCategories (
    CategoryId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(100) NOT NULL
);
go
-- Tạo bảng Locations
CREATE TABLE Locations (
    LocationId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(200) NOT NULL,
    ImageUrl NVARCHAR(255),
    CategoryId INT,
    FOREIGN KEY (CategoryId) REFERENCES TourCategories(CategoryId)
);
go
-- Tạo bảng Tours (đã thêm cột Price)
CREATE TABLE Tours (
    TourId INT PRIMARY KEY IDENTITY,
    Name NVARCHAR(255),
    Description NVARCHAR(MAX),
    Duration VARCHAR(10),
    Itinerary NVARCHAR(MAX),
    ImageUrl NVARCHAR(500),
    LocationId INT NOT NULL,
    DepartureLocation NVARCHAR(255),
    Price DECIMAL(18,2) NOT NULL DEFAULT 0,
    FOREIGN KEY (LocationId) REFERENCES Locations(LocationId)
);
go
-- Tạo bảng TourSchedules (không còn cột Price)
CREATE TABLE TourSchedules (
    ScheduleId INT PRIMARY KEY IDENTITY,
    TourId INT NOT NULL,
    StartDate DATE NOT NULL,
    EndDate DATE NOT NULL,
    AvailableSeats INT NOT NULL CHECK (AvailableSeats >= 0),
    FOREIGN KEY (TourId) REFERENCES Tours(TourId)
);
go
-- Tạo bảng Bookings
CREATE TABLE Bookings (
    BookingId INT PRIMARY KEY IDENTITY,
    UserId INT NOT NULL,
    ScheduleId INT NOT NULL,
    TravelDate DATE NOT NULL,
    NumberOfPeople INT NOT NULL,
    CustomerFullName NVARCHAR(100),
	CustomerPhone NVARCHAR(20),
    CustomerEmail NVARCHAR(100),
	Total DECIMAL(18, 2) NOT NULL,
    Status NVARCHAR(50) DEFAULT N'Chờ xác nhận',
    BookingDate DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (UserId) REFERENCES Users(UserId),
    FOREIGN KEY (ScheduleId) REFERENCES TourSchedules(ScheduleId)
);
go
CREATE TABLE BookingPassengers (
    PassengerId INT PRIMARY KEY IDENTITY,
    BookingId INT NOT NULL,
    FullName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(10) NOT NULL,
    DateOfBirth DATE NOT NULL,
    PassengerType NVARCHAR(20) NOT NULL,
    PassportNumber NVARCHAR(50),
    Nationality NVARCHAR(50),
    Note NVARCHAR(255),
    FOREIGN KEY (BookingId) REFERENCES Bookings(BookingId)
);
go
-- Tạo bảng Payments

-- Tạo bảng News
CREATE TABLE News (
    NewsId INT PRIMARY KEY IDENTITY,
    Title NVARCHAR(255),
    Content NVARCHAR(MAX),
    ImageUrl NVARCHAR(255),
    CreatedBy NVARCHAR(100),
    CreatedAt DATETIME DEFAULT GETDATE(),
    Category NVARCHAR(50)
);
go
-- Tạo bảng Reviews
CREATE TABLE Reviews (
    ReviewId INT PRIMARY KEY IDENTITY,
    TourId INT,
    UserId INT,
    Rating INT CHECK (Rating BETWEEN 1 AND 5),
    Content NVARCHAR(MAX),
    Image NVARCHAR(255),
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (TourId) REFERENCES Tours(TourId),
    FOREIGN KEY (UserId) REFERENCES Users(UserId)
);
go
INSERT INTO TourCategories (Name)
VALUES (N'Du lịch trong nước'),(N'Du lịch quốc tế');
--
INSERT INTO Locations (Name, ImageUrl, CategoryId)
VALUES 
(N'Hà Nội', '/Content/images/HN.jpg', 1),
(N'Đà Lạt', '/Content/images/DL.jpg', 1),
(N'Hồ Chí Minh', '/Content/images/HCM.png', 1),
(N'Huế', '/Content/images/H.jpg', 1),
(N'Phú Quốc', '/Content/images/PQ.jpg', 1),
(N'Hội An', '/Content/images/HA.jpg', 1),
(N'Singapore', '/Content/images/SGP.jpg', 2),
(N'Paris', '/Content/images/PR.jpg', 2),
(N'Bangkok', '/Content/images/BK.jpg', 2),
(N'Seoul', '/Content/images/S.jpg', 2),
(N'New York', '/Content/images/NY.jpg', 2),
(N'Sydney', '/Content/images/SD.jpg', 2);
--
go
INSERT INTO Tours (LocationId, Name, Description, Price, Duration, DepartureLocation, ImageUrl, Itinerary) 
VALUES
-- hà nội
(1, N'Tour Hà Nội 3N2Đ', 
 N'Tour tham quan thủ đô Hà Nội – nơi hội tụ nét đẹp lịch sử.',
 3000000, 
 N'3N2Đ',
 N'TP. Hồ Chí Minh', 
 '/Content/images/T1.jpg',
N'
🗓 Ngày 1: TP. Hồ Chí Minh ✈ Hà Nội – Văn Miếu – Hồ Gươm  
- Đón khách tại sân bay Nội Bài.  
- Tham quan **Văn Miếu - Quốc Tử Giám** – trường đại học đầu tiên của Việt Nam.  
- Dạo quanh khu vực **Hồ Gươm**, thăm **Tháp Rùa**, **Đền Ngọc Sơn**.  
- Ăn trưa đặc sản: bún chả, phở Hà Nội.  
- Nhận phòng khách sạn nghỉ ngơi.

🗓 Ngày 2: Lăng Bác – Chùa Một Cột – Phố cổ  
- Tham quan **Lăng Chủ tịch Hồ Chí Minh**, **Chùa Một Cột**, **Phủ Chủ tịch**.  
- Chiều: Dạo phố cổ, thưởng thức **kem Tràng Tiền**, **bánh cuốn Thanh Trì**, mua sắm tại **Hàng Đào, Hàng Ngang**.  
- Tối: Xem múa rối nước hoặc tự do khám phá Hà Nội về đêm.

🗓 Ngày 3: Chợ Đồng Xuân – Mua sắm đặc sản – Tiễn khách  
- Tham quan **Chợ Đồng Xuân**, mua quà lưu niệm, đặc sản như ô mai, chè sen.  
- Ăn trưa nhẹ và chuẩn bị ra sân bay.  
- Kết thúc tour – hẹn gặp lại quý khách lần sau!
'),

(1, N'Tour Hà Nội – Ninh Bình – Tràng An 4N3Đ', 
 N'Tận hưởng hành trình khám phá thủ đô ngàn năm văn hiến và danh thắng Ninh Bình – nơi được mệnh danh là "Vịnh Hạ Long trên cạn" với Tràng An, Tam Cốc, chùa Bái Đính.',
 4500000, 
 N'4N3Đ',
 N'TP. Hồ Chí Minh', 
 '/Content/images/T2.jpg',
N'
🗓 Ngày 1: TP. Hồ Chí Minh ✈ Hà Nội – Hồ Tây – Chùa Trấn Quốc  
- Đón khách tại sân bay Nội Bài, đưa về trung tâm thành phố.  
- Tham quan **Chùa Trấn Quốc** – ngôi chùa cổ nhất Hà Nội, dạo quanh **Hồ Tây**.  
- Ăn tối với món **bún ốc**, nhận phòng nghỉ ngơi.

🗓 Ngày 2: Hà Nội – Ninh Bình – Chùa Bái Đính  
- Di chuyển tới **Ninh Bình**, tham quan **chùa Bái Đính** – quần thể chùa lớn nhất Đông Nam Á.  
- Thưởng thức cơm cháy – dê núi đặc sản địa phương.  
- Nghỉ đêm tại khách sạn ở Ninh Bình.

🗓 Ngày 3: Tràng An – Tam Cốc – Về Hà Nội  
- Khám phá **quần thể danh thắng Tràng An** bằng thuyền.  
- Tham quan **Tam Cốc – Bích Động**, nơi được ví như chốn bồng lai tiên cảnh.  
- Chiều về Hà Nội, tự do khám phá **phố cổ**.

🗓 Ngày 4: Chợ Đồng Xuân – Mua sắm – Bay về  
- Mua đặc sản tại chợ Đồng Xuân: ô mai, cốm, bánh chè lam.  
- Trả phòng, tiễn khách ra sân bay.  
- Kết thúc tour đầy kỷ niệm.
'),

(1,N'Tour Hà Nội – Làng cổ Đường Lâm – Ba Vì 2N1Đ', 
 N'Chuyến đi ngắn ngày dành cho du khách muốn trải nghiệm làng quê Bắc Bộ cổ kính và thiên nhiên núi rừng Ba Vì trong lành, kết hợp nghỉ dưỡng và khám phá văn hóa dân gian.',
 2200000, 
 N'2N1Đ',
 N'TP. Hồ Chí Minh', 
 '/Content/images/T3.jpg',
N'
🗓 Ngày 1: Hà Nội – Làng cổ Đường Lâm – Sơn Tây  
- Di chuyển từ trung tâm Hà Nội đến **làng cổ Đường Lâm** – ngôi làng đá ong hàng trăm năm tuổi.  
- Thăm **đình Mông Phụ**, **chùa Mía**, **nhà cổ truyền thống**.  
- Ăn trưa với các món như gà mía, chè lam.  
- Chiều tham quan **thành cổ Sơn Tây**, tối nghỉ đêm tại resort gần Ba Vì.

🗓 Ngày 2: Ba Vì – Vườn Quốc gia – Trở về Hà Nội  
- Buổi sáng leo núi khám phá **vườn quốc gia Ba Vì**, thăm **đền Thượng** và **khu di tích Pháp cổ**.  
- Trưa ăn tại nhà hàng dưới chân núi.  
- Chiều về lại Hà Nội, dừng chân mua đặc sản tại khu vực **Tây Hồ**.  
- Kết thúc tour trong buổi chiều.
');
--
go
INSERT INTO TourSchedules (TourId, StartDate, EndDate, AvailableSeats) 
VALUES 
-- Tour 1: Hà Nội 3N2Đ
(1, '2025-07-10', '2025-07-12', 20),
(1, '2025-08-15', '2025-08-17', 15),

-- Tour 2: Hà Nội – Ninh Bình – Tràng An 4N3Đ
(2, '2025-07-20', '2025-07-23',  25),
(2, '2025-09-05', '2025-09-08', 18),

-- Tour 3: Hà Nội – Đường Lâm – Ba Vì 2N1Đ
(3, '2025-07-05', '2025-07-06',  18),
(3, '2025-08-01', '2025-08-02', 20);

select * from Bookings
