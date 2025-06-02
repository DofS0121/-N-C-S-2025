--create database VivoTourWeb

--use VivoTourWeb

CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    password VARCHAR(255) NOT NULL,
    role NVARCHAR(20) DEFAULT 'Customer',
    image NVARCHAR(255),
    createdAt DATETIME DEFAULT GETDATE()
);
go
CREATE TABLE TourCategories (
    category_id INT IDENTITY(1,1) PRIMARY KEY,
    category_name NVARCHAR(255) NOT NULL UNIQUE
);
go
CREATE TABLE Tours (
    tour_id INT IDENTITY(1,1) PRIMARY KEY,
    tour_name NVARCHAR(255) NOT NULL,
    description NVARCHAR(MAX),
    price DECIMAL(18,2) NOT NULL,
    image NVARCHAR(255),
    category_id INT NOT NULL,
    createdAt DATETIME DEFAULT GETDATE(),
    user_id INT NOT NULL, -- Người tạo tour
    quantity_instock INT DEFAULT 0,
    stock_warning BIT DEFAULT 0,
    FOREIGN KEY (category_id) REFERENCES TourCategories(category_id) ON DELETE CASCADE,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE CASCADE
);

CREATE TABLE Bookings (
    booking_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_id INT,
    booking_date DATE DEFAULT GETDATE(),
    delivery_date DATE,
    ispayment BIT,
    isship BIT,
    isreceived BIT DEFAULT 0,
    status NVARCHAR(20) DEFAULT 'Pending',
    FOREIGN KEY (customer_id) REFERENCES Users(user_id) ON DELETE CASCADE
);
go
CREATE TABLE BookingDetails (
    booking_id INT,
    tour_id INT,
    quantity INT,
    price DECIMAL(18,2),
    PRIMARY KEY(booking_id, tour_id),
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id) ON DELETE CASCADE,
    FOREIGN KEY (tour_id) REFERENCES Tours(tour_id) ON DELETE NO ACTION
);
go
CREATE TABLE Payments (
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    booking_id INT,
    payment_method NVARCHAR(50),
    amount DECIMAL(18,2),
    payment_date DATETIME DEFAULT GETDATE(),
    status NVARCHAR(50) DEFAULT 'Chưa thanh toán',
    FOREIGN KEY (booking_id) REFERENCES Bookings(booking_id) ON DELETE CASCADE
);
go
CREATE TABLE Customers (
    customer_id INT IDENTITY(1,1) PRIMARY KEY,
    customer_name NVARCHAR(50),
    email VARCHAR(50) UNIQUE,
    address NVARCHAR(100),
    numberphone VARCHAR(12),
    dob SMALLDATETIME,
    user_id INT UNIQUE,
    FOREIGN KEY (user_id) REFERENCES Users(user_id) ON DELETE SET NULL
);
go
INSERT INTO Users (username, password, role, image)
VALUES 
('admin', '123', 'Admin', 'admin.jpg')

go
INSERT INTO TourCategories (category_name)
VALUES 
(N'Tour nội địa'),
(N'Tour nước ngoài');

go
INSERT INTO Tours (tour_name, description, price, image, category_id, user_id, quantity_instock)
VALUES 
(N'Tour Đà Lạt 3N2Đ', N'Thưởng ngoạn không khí se lạnh và cảnh sắc tuyệt đẹp của Đà Lạt.', 2500000, 'dalat.jpg', 1, 1, 30),
(N'Tour Vũng Tàu 2N1Đ', N'Nghỉ dưỡng ven biển Vũng Tàu dịp cuối tuần.', 1200000, 'vungtau.jpg', 1, 1, 50);

select * from Tours
select * from TourCategories
select * from Users
select * from Customers
drop table Users
drop table TourCategories
drop table Customers
drop table Bookings
drop table BookingDetails
drop table Payments
drop table Tours
