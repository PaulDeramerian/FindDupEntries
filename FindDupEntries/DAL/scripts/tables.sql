USE [Advertiser]
GO
/****** Object:  Table [dbo].[AdvertiserNames]    Script Date: 3/15/2016 3:59:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvertiserNames](
	[Id] [uniqueidentifier] NOT NULL,
	[OriginName] [nvarchar](255) NOT NULL,
	[CompareWithName] [nvarchar](255) NOT NULL,
	[WordCount] [int] NOT NULL,
 CONSTRAINT [PK_AdvertiserNames] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdvertiserNameWords]    Script Date: 3/15/2016 3:59:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdvertiserNameWords](
	[Id] [uniqueidentifier] NOT NULL,
	[NameSourceId] [uniqueidentifier] NOT NULL,
	[Word] [nvarchar](50) NOT NULL,
	[SourceWordCount] [int] NOT NULL,
 CONSTRAINT [PK_AdvertiserNameWords] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EntryWordMappers]    Script Date: 3/15/2016 3:59:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EntryWordMappers](
	[Id] [uniqueidentifier] NOT NULL,
	[EntryId] [uniqueidentifier] NOT NULL,
	[MatchEntryId] [uniqueidentifier] NOT NULL,
	[WordMatchCount] [int] NOT NULL,
 CONSTRAINT [PK_EntryWordMappers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
