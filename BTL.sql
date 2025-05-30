USE [CuaHangThuocBVTV]
GO
/****** Object:  Table [dbo].[Invoices]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Invoices](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NULL,
	[InvoiceDate] [datetime] NULL,
	[TotalAmount] [decimal](12, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[OrderDetails]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] [int] IDENTITY(1,1) NOT NULL,
	[OrderID] [int] NOT NULL,
	[ProductID] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](12, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderDetailID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Orders]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[OrderID] [int] IDENTITY(1,1) NOT NULL,
	[CustomerID] [int] NULL,
	[OrderDate] [datetime] NULL,
	[TotalAmount] [decimal](12, 2) NULL,
	[CreatedBy] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[OrderID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[UnitPrice] [decimal](12, 2) NOT NULL,
	[Unit] [nvarchar](50) NULL,
	[Quantity] [int] NOT NULL,
	[SupplierID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Roles]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleID] [int] NOT NULL,
	[RoleName] [nvarchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[RoleName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Suppliers]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Suppliers](
	[SupplierID] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Phone] [nvarchar](20) NULL,
	[Email] [nvarchar](100) NULL,
	[Address] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[SupplierID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 23/04/2025 09:40:45 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserID] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NOT NULL,
	[PasswordHash] [nvarchar](255) NOT NULL,
	[FullName] [nvarchar](100) NULL,
	[Email] [nvarchar](100) NULL,
	[Phone] [nvarchar](20) NULL,
	[Address] [nvarchar](max) NULL,
	[RoleID] [int] NOT NULL,
	[CreatedAt] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[UserID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
UNIQUE NONCLUSTERED 
(
	[Username] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[Invoices] ADD  DEFAULT (getdate()) FOR [InvoiceDate]
GO
ALTER TABLE [dbo].[Orders] ADD  DEFAULT (getdate()) FOR [OrderDate]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT ((0)) FOR [Quantity]
GO
ALTER TABLE [dbo].[Users] ADD  DEFAULT (getdate()) FOR [CreatedAt]
GO
ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([OrderID])
REFERENCES [dbo].[Orders] ([OrderID])
GO
ALTER TABLE [dbo].[OrderDetails]  WITH CHECK ADD FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Users] ([UserID])
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD FOREIGN KEY([SupplierID])
REFERENCES [dbo].[Suppliers] ([SupplierID])
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO

ALTER TABLE [dbo].[Products]
ADD [ImportDate] [datetime] NOT NULL DEFAULT GETDATE();

ALTER TABLE [dbo].[Products]
ADD [ImagePath] NVARCHAR(255) NULL;

ALTER TABLE [dbo].[Users]
ADD [ImagePath] NVARCHAR(255) NULL;
ALTER TABLE [dbo].[Suppliers]
ADD [ImagePath] NVARCHAR(255) NULL;

USE [CuaHangThuocBVTV];
GO

USE CuaHangThuocBVTV;

INSERT INTO Suppliers (Name, Phone, Email, Address) VALUES
('Công ty ABC', '0123456789', 'abc@example.com', '123 Đường A, Quận X, TP.HCM'),
('Công ty XYZ', '0987654321', 'xyz@example.com', '456 Đường B, Quận Y, Hà Nội'),
('Cửa hàng 123', '0333222111', 'ch123@example.com', '789 Đường C, TP. Đà Nẵng'),
('Nhà phân phối A', '0777888999', 'nppA@example.com', '101 Đường D, TP. Cần Thơ'),
('Đại lý BCD', '0555444333', 'dailyBCD@example.com', '222 Đường E, TP. Hải Phòng'),
('Doanh nghiệp TM', '0888111222', 'tm@example.com', '333 Đường F, TP. Huế'),
('Công ty TNHH K', '0222999888', 'tnhhK@example.com', '444 Đường G, TP. Nha Trang'),
('Cửa hàng Nông Sản', '0444777666', 'nongsan@example.com', '555 Đường H, TP. Vinh'),
('Nhà cung cấp Xanh', '0666555444', 'xanh@example.com', '666 Đường I, TP. Biên Hòa'),
('Công ty CP Y', '0999333222', 'cpy@example.com', '777 Đường K, TP. Mỹ Tho');

USE [CuaHangThuocBVTV];
 GO
 INSERT INTO Products (Name, Description, UnitPrice, Unit, Quantity, SupplierID, ImportDate, ImagePath)
 VALUES
 (N'Thuốc trừ sâu Abamectin', N'Thuốc trừ sâu sinh học', 25000, N'Chai', 100, 1, GETDATE(), NULL),
 (N'Thuốc trừ bệnh Validamycin', N'Thuốc trừ bệnh cây trồng', 30000, N'Chai', 150, 2, GETDATE(), NULL),
 (N'Phân bón lá NPK', N'Phân bón cho mọi loại cây', 50000, N'Bao', 200, 3, GETDATE(), NULL),
 (N'Thuốc diệt cỏ Glyphosate', N'Thuốc diệt cỏ mạnh', 40000, N'Chai', 120, 1, GETDATE(), NULL),
 (N'Thuốc trừ ốc Maize', N'Thuốc trừ ốc bưu vàng', 35000, N'Chai', 80, 2, GETDATE(), NULL),
 (N'Thuốc kích rễ NAA', N'Thuốc kích thích ra rễ', 45000, N'Chai', 90, 3, GETDATE(), NULL),
 (N'Thuốc trừ nhện Reasgant', N'Thuốc trừ nhện hại cây', 28000, N'Chai', 110, 1, GETDATE(), NULL),
 (N'Thuốc trừ nấm Ridomil', N'Thuốc trừ nấm bệnh', 32000, N'Gói', 130, 2, GETDATE(), NULL),
 (N'Phân bón hữu cơ Vi sinh', N'Phân bón hữu cơ', 60000, N'Bao', 160, 3, GETDATE(), NULL),
 (N'Thuốc trừ sâu Confidor', N'Thuốc trừ sâu phổ rộng', 27000, N'Chai', 140, 1, GETDATE(), NULL);
 GO


 ALTER TABLE [dbo].[Users]
ADD [Status] [int] NOT NULL DEFAULT 1;
