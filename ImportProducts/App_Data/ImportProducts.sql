/****** Object:  Table [dbo].[Feeds]    Script Date: 03/22/2012 18:51:58 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Feeds]') AND type in (N'U'))
DROP TABLE [dbo].[Feeds]
GO
/****** Object:  Default [DF_Feeds_PortalId]    Script Date: 03/22/2012 18:51:58 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Feeds_PortalId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Feeds]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Feeds_PortalId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Feeds] DROP CONSTRAINT [DF_Feeds_PortalId]
END


End
GO
/****** Object:  Default [DF__Feeds__VendorId__060DEAE8]    Script Date: 03/22/2012 18:51:58 ******/
IF  EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF__Feeds__VendorId__060DEAE8]') AND parent_object_id = OBJECT_ID(N'[dbo].[Feeds]'))
Begin
IF  EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Feeds__VendorId__060DEAE8]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Feeds] DROP CONSTRAINT [DF__Feeds__VendorId__060DEAE8]
END


End
GO
/****** Object:  Table [dbo].[Feeds]    Script Date: 03/22/2012 18:51:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Feeds]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Feeds](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[URL] [nvarchar](1000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[LastRun] [datetime] NULL,
	[Status] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
	[PortalId] [int] NOT NULL,
	[Category] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
	[VendorId] [int] NOT NULL,
	[AdvancedCategoryRoot] [nvarchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_Feeds] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)
END
GO
SET IDENTITY_INSERT [dbo].[Feeds] ON
INSERT [dbo].[Feeds] ([Id], [Name], [URL], [LastRun], [Status], [PortalId], [Category], [VendorId], [AdvancedCategoryRoot]) VALUES (1, N'Laterooms', N'http://xmlfeed.laterooms.com/staticdata/hotels_standard.zip', CAST(0x0000A013006D416A AS DateTime), N'Cancel', 0, N'Hotels', 2, N'Hotels')
INSERT [dbo].[Feeds] ([Id], [Name], [URL], [LastRun], [Status], [PortalId], [Category], [VendorId], [AdvancedCategoryRoot]) VALUES (2, N'Trade Doubler', N'http://pf.tradedoubler.com/export/export?myFeed=13267926451624184&myFormat=12196865151077321', NULL, N'Error', 0, N'Hotels', 3, N'Hotels')
INSERT [dbo].[Feeds] ([Id], [Name], [URL], [LastRun], [Status], [PortalId], [Category], [VendorId], [AdvancedCategoryRoot]) VALUES (3, N'Home and garden', N'http://pf.tradedoubler.com/export/export?myFeed=13305109961196734&myFormat=12196865151077321', CAST(0x0000A01200C1BBDF AS DateTime), N'Success', 0, N'Home and garden', 4, N'Home and garden')
INSERT [dbo].[Feeds] ([Id], [Name], [URL], [LastRun], [Status], [PortalId], [Category], [VendorId], [AdvancedCategoryRoot]) VALUES (6, N'Productserve', N'http://datafeed.api.productserve.com/datafeed/download/apikey/c0911d4e694475e27242163c636ab155/mid/978/columns/merchant_id,merchant_name,aw_product_id,merchant_product_id,upc,ean,mpn,isbn,model_number,product_name,description,specifications,promotional_text,merchant_category,category_id,category_name,language,brand_name,merchant_deep_link,merchant_thumb_url,merchant_image_url,aw_deep_link,aw_thumb_url,aw_image_url,delivery_time,valid_from,valid_to,currency,search_price,store_price,rrp_price,display_price,delivery_cost,web_offer,pre_order,in_stock,stock_quantity,is_for_sale,warranty,condition,product_type,parent_product_id,commission_group/format/xml/compression/zip/', CAST(0x0000A014008074D9 AS DateTime), N'Success', 0, N'Sweets', 5, N'Sweets')
INSERT [dbo].[Feeds] ([Id], [Name], [URL], [LastRun], [Status], [PortalId], [Category], [VendorId], [AdvancedCategoryRoot]) VALUES (7, N'Webgains', N'http://content.webgains.com/affiliates/datafeed.html?action=download&campaign=57531&username=web@cowrie.co.uk&password=applications&format=xml&zipformat=none&fields=extended&programs=all&allowedtags=&categories=all', CAST(0x0000A018015435C8 AS DateTime), N'Success', 0, N'Mobile phones', 6, N'Mobile phones')
SET IDENTITY_INSERT [dbo].[Feeds] OFF
/****** Object:  Default [DF_Feeds_PortalId]    Script Date: 03/22/2012 18:51:58 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF_Feeds_PortalId]') AND parent_object_id = OBJECT_ID(N'[dbo].[Feeds]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF_Feeds_PortalId]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Feeds] ADD  CONSTRAINT [DF_Feeds_PortalId]  DEFAULT ((0)) FOR [PortalId]
END


End
GO
/****** Object:  Default [DF__Feeds__VendorId__060DEAE8]    Script Date: 03/22/2012 18:51:58 ******/
IF Not EXISTS (SELECT * FROM sys.default_constraints WHERE object_id = OBJECT_ID(N'[dbo].[DF__Feeds__VendorId__060DEAE8]') AND parent_object_id = OBJECT_ID(N'[dbo].[Feeds]'))
Begin
IF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[DF__Feeds__VendorId__060DEAE8]') AND type = 'D')
BEGIN
ALTER TABLE [dbo].[Feeds] ADD  CONSTRAINT [DF__Feeds__VendorId__060DEAE8]  DEFAULT ((1)) FOR [VendorId]
END


End
GO
