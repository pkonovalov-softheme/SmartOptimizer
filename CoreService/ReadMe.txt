C:\Windows\Microsoft.NET\Framework64\v4.0.30319\installutil.exe L:\optimizer\SmartOptimizer\CoreService\bin\x64\Release\CoreService.exe





USE [master]
GO

/****** Object:  Database [BlockOptimizationStats]    Script Date: 27.03.2016 14:11:38 ******/
CREATE DATABASE [BlockOptimizationStats]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'BlockOptimizationStats', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\BlockOptimizationStats.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'BlockOptimizationStats_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\BlockOptimizationStats_log.ldf' , SIZE = 13632KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO

ALTER DATABASE [BlockOptimizationStats] SET COMPATIBILITY_LEVEL = 120
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [BlockOptimizationStats].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [BlockOptimizationStats] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET ARITHABORT OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [BlockOptimizationStats] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [BlockOptimizationStats] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET  DISABLE_BROKER 
GO

ALTER DATABASE [BlockOptimizationStats] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [BlockOptimizationStats] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET RECOVERY FULL 
GO

ALTER DATABASE [BlockOptimizationStats] SET  MULTI_USER 
GO

ALTER DATABASE [BlockOptimizationStats] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [BlockOptimizationStats] SET DB_CHAINING OFF 
GO

ALTER DATABASE [BlockOptimizationStats] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [BlockOptimizationStats] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO

ALTER DATABASE [BlockOptimizationStats] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [BlockOptimizationStats] SET  READ_WRITE 
GO

USE [BlockOptimizationStats]
GO

/****** Object:  Table [dbo].[PositionsStats]    Script Date: 27.03.2016 14:16:47 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[PositionsStats](
	[Position] [int] NOT NULL,
	[BlockId] [int] NOT NULL,
	[Views] [int] NOT NULL,
	[Clicks] [int] NOT NULL,
	[Value] [int] NOT NULL,
	[InsertDate] [datetime] NOT NULL
) ON [PRIMARY]

GO


USE [BlockOptimizationStats]
GO

/****** Object:  Table [dbo].[Convertions]    Script Date: 27.03.2016 14:17:05 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Convertions](
	[AGviews] [int] NOT NULL,
	[AGclicks] [int] NOT NULL,
	[AGvalue] [int] NOT NULL,
	[BGviews] [int] NOT NULL,
	[BGclicks] [int] NOT NULL,
	[BGvalue] [int] NOT NULL,
	[InsertDate] [datetime] NOT NULL,
	[BlockId] [int] NOT NULL
) ON [PRIMARY]

GO



