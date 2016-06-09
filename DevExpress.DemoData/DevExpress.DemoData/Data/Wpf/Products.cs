#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
{                                                                   }
{       Copyright (c) 2000-2015 Developer Express Inc.              }
{       ALL RIGHTS RESERVED                                         }
{                                                                   }
{   The entire contents of this file is protected by U.S. and       }
{   International Copyright Laws. Unauthorized reproduction,        }
{   reverse-engineering, and distribution of all or any portion of  }
{   the code contained in this file is strictly prohibited and may  }
{   result in severe civil and criminal penalties and will be       }
{   prosecuted to the maximum extent possible under the law.        }
{                                                                   }
{   RESTRICTIONS                                                    }
{                                                                   }
{   THIS SOURCE CODE AND ALL RESULTING INTERMEDIATE FILES           }
{   ARE CONFIDENTIAL AND PROPRIETARY TRADE                          }
{   SECRETS OF DEVELOPER EXPRESS INC. THE REGISTERED DEVELOPER IS   }
{   LICENSED TO DISTRIBUTE THE PRODUCT AND ALL ACCOMPANYING .NET    }
{   CONTROLS AS PART OF AN EXECUTABLE PROGRAM ONLY.                 }
{                                                                   }
{   THE SOURCE CODE CONTAINED WITHIN THIS FILE AND ALL RELATED      }
{   FILES OR ANY PORTION OF ITS CONTENTS SHALL AT NO TIME BE        }
{   COPIED, TRANSFERRED, SOLD, DISTRIBUTED, OR OTHERWISE MADE       }
{   AVAILABLE TO OTHER INDIVIDUALS WITHOUT EXPRESS WRITTEN CONSENT  }
{   AND PERMISSION FROM DEVELOPER EXPRESS INC.                      }
{                                                                   }
{   CONSULT THE END USER LICENSE AGREEMENT FOR INFORMATION ON       }
{   ADDITIONAL RESTRICTIONS.                                        }
{                                                                   }
{*******************************************************************}
*/
#endregion Copyright (c) 2000-2015 Developer Express Inc.

using System;
using System.Collections.Generic;
namespace DevExpress.DemoData.Model {
	public static partial class Repository {
		static List<Product> CreateWpfProducts(Platform p) {
			return new List<Product> {
				new Product(p,
					CreateDXGridDemos,
					name: "DXGrid",
					displayName: "Data Grid",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "Displays data using a Grid or Contacts view, both fully customizable via WPF Templates.",
					description: @"
                        <Paragraph>
                        Tabular, Tree or Cards data display with the majority of market-leading features (comprehensive data management, advanced scrolling options, full end-user customization, fast data printing and exporting, etc.) and blazing fast data processing in LINQ Server Mode and Instance Feedback UI Mode.
                        </Paragraph>
                        <Paragraph>
                        DXGrid for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("48c2a820-a314-4b4a-81a1-cfec537f6c7c"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateXtraReportsForXpfDemos,
					name: "XtraReportsForXpf",
					displayName: "Reporting Tool",
					componentName: "XtraReportsWpf",
					group: "Reporting and Analytics Controls",
					shortDescription: "DevExpress Reporting platform showcased within a WPF application.",
					description: @"
                        <Paragraph>
                        A flexible reporting tool with a powerful exporting engine and easy integration.
                        </Paragraph>",
					licenseInfo: 0x0000000000000010,
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXTreeListDemos,
					name: "DXTreeList",
					displayName: "Multi-Column TreeView",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "Data-aware or unbound TreeView-Grid combination, providing newsreader style UI.",
					description: @"
                        <Paragraph>
                        Displays information as a TREE, a GRID, or a combination of both - in either data bound or unbound mode. Supports on-demand child node loading, sorting, filtering, totals, etc.
                        </Paragraph>
                        <Paragraph>
                        DXTreeList for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("2b6d96db-f29b-4aa6-bdfa-0d66de4669a4"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXBarsDemos,
					name: "DXBars",
					displayName: "Toolbar-Menu System",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "MS Office style Toolbars, Main Menu, Status Bar and Context Menu controls with multiple item types and embedded editors.",
					description: @"
                        <Paragraph>
                        Provides you with an easy and flexible way to build the traditional Toolbar and Menu system for your application.
                        </Paragraph>
                        <Paragraph>
                        DXBars for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("8d09a36c-4251-4bdd-ae9f-dbdf18a8b5e4"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXRibbonDemos,
					name: "DXRibbon",
					displayName: "Ribbon Control",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "Feature-complete Microsoft® Office® 2007 or 2010 style Ribbon controls.",
					description: @"
                        <Paragraph>
                        Build Microsoft Office style navigation systems with complete end-user customization support.
                        </Paragraph>
                        <Paragraph>
                        DXRibbon for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("27451a9d-ffaf-4f3b-85fc-480d4cc45131"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXChartsDemos,
					name: "DXCharts",
					displayName: "Charting Control",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "A chart control featuring 20 chart types, including both 3D and 2D charts.",
					description: @"
                        <Paragraph>
                        About 30 chart types with full support for chart element modeling, built-in animation effects and predefined themes.
                        </Paragraph>
                        <Paragraph>
                        DXCharts for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("e9f1506a-471c-4b72-82df-151effca6b0d"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXGaugesDemos,
					name: "DXGauges",
					displayName: "Gauge Control",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "Six Circular and two Linear gauge types. 12 appearance options provide over 90 gauge presets.",
					description: @"
                        <Paragraph>
                        DXGauges introduce Circular and Linear gauge types with more than 20 predefined models, support for animation effects and amazing interactivity. The list of gauge elements includes scales, needles, markers, level bars, range bars, ranges, etc.
                        </Paragraph>
                        <Paragraph>
                        DXGauges for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("7e2e4779-666b-4264-8bf2-32969857f42c"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXMapDemos,
					name: "DXMap",
					displayName: "Map Control",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "Supports Bing Maps, OpenStreetMap, unlimited number of map layers and shapefile support.",
					description: @"
                        <Paragraph>
                        DXMap introduce different map types, support for animation effects and amazing interactivity.
                        </Paragraph>
                        <Paragraph>
                        DXMap for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("bc85ee42-ca72-4727-92e9-b01b4abebcb4"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXPivotGridDemos,
					name: "DXPivotGrid",
					displayName: "Pivot Grid",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "MS Excel style Pivot Table Control for multi-dimensional data analysis and cross-tab report generation.",
					description: @"
                        <Paragraph>
                        Full support for user customization and native integration with DevExpress Charting Control allow end-users to produce a nearly endless array of reports by simply dragging and clicking.
                        </Paragraph>
                        <Paragraph>
                        DXPivotGrid for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("a7bb8147-c010-423e-9143-f9870414b985"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXSchedulerDemos,
					name: "DXScheduler",
					displayName: "Calendar and Scheduling",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "MS Outlook style Day View, Week View, Month View and Timeline View Controls with full support for side-by-side calendars.",
					description: @"
                        <Paragraph>
                        Features multiple calendar view types and side-by-side calendar display. Ships with complete Microsoft Outlook® style end-user interface.
                        </Paragraph>
                        <Paragraph>
                        DXScheduler for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("81349f40-78df-4b8c-8567-73e8a44b8159"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXRichEditDemos,
					name: "DXRichEdit",
					displayName: "Rich Text Editor",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "MS Word style Rich Text Editor supporting Mail Merge and all popular storage formats.",
					description: @"
                        <Paragraph>
                        Rich text formatting and editing capabilities, including Mail Merge, in a Microsoft Office® style user interface.
                        </Paragraph>
                        <Paragraph>
                        DXRichEdit for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("d70bcd47-8459-453f-bbfe-0815d25d0b8c"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXDockingDemos,
					name: "DXDocking",
					displayName: "Dock Windows",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "MS Visual Studio style Dock Window Library suppporting tab containers, floating and auto-hidden panels.",
					description: @"
                        <Paragraph>
                        Dashboards and Microsoft® Visual Studio® Style Dock Window interfaces with built-in support for auto-hide windows, splitters and tab containers.
                        </Paragraph>
                        <Paragraph>
                        DXDocking for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("ddb391cb-13bb-4796-bbc9-2e7b0842102b"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXLayoutControlDemos,
					name: "DXLayoutControl",
					displayName: "Layout Manager",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "Includes various layout management controls that support end-user customization.",
					description: @"
                        <Paragraph>
                        Includes several controls designed to address all layout management tasks - from building an automatically-aligned gallery to end-user layout customization.
                        </Paragraph>
                        <Paragraph>
                        DXLayoutControl for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("70f0e900-a87e-4abf-a40b-b2f5c58b3363"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXPrintingDemos,
					name: "DXPrinting",
					displayName: "Printing-Exporting Library",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "Renders, prints, or exports banded reports created in XAML and DevExpress data-aware controls.",
					description: @"
                        <Paragraph>
                        A library for printing/exporting data-aware controls with a multitude of supported export formats.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("cc0c2d9f-837a-41c9-aa7e-c7139e288c76"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXEditorsDemos,
					name: "DXEditors",
					displayName: "Data Editors",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "Data Editors library with Advanced Masked Input and Data Validation support.",
					description: @"
                        <Paragraph>
                        DevExpress WPF Editor controls to be used standalone or within data-aware controls - featuring superior masked input and advanced validation mechanism.
                        </Paragraph>
                        <Paragraph>
                        DXEditors for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("48c7cac9-9f50-4299-b5f6-aada62f71f0e"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXPropertyGridDemos,
					name: "DXPropertyGrid",
					displayName: "Property Grid",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "Delivers functionality found in the Visual Studio Property Grid. Allows you to display and edit properties of any object.",
					description: @"
                        <Paragraph>
                        Property Grid Demos
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("bcc51128-a734-11e2-bed5-64700200865d"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXPdfViewerDemos,
					name: "DXPdfViewer",
					displayName: "PDF Viewer",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "Demonstrates PDF Viewer capabilities of displaying PDF documents.",
					description: @"
                        <Paragraph>
                        The PDF Viewer control allows you to view, print PDFs and provides blazingly fast performance.
                        </Paragraph>
                        <Paragraph>
                        DXPdfVIewer for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("3717b344-45f2-11e3-bee1-0050b660967c"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXDiagramDemos,
					name: "DXDiagram",
					displayName: "Diagram",
					componentName: "WPF Components",
					group: "",
					shortDescription: "Demonstrates Diagram features of making documents.",
					description: @"
                        <Paragraph>
                        The DevExpress Diagram control allows you to make all kinds of diagram.
                        </Paragraph>
                        <Paragraph>
                         This demo features main capabilities of the Diagram control.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("3717b984-45f2-11e3-bee1-0050e660967a"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXControlsDemos,
					name: "DXControls",
					displayName: "Utility Controls",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "All-purpose control library including Book, Tab Control and Workspace Manager.",
					description: @"
                        <Paragraph>
                        This library's primary purpose is to provide the basic functionality for advanced DevExpress WPF Controls.
                        </Paragraph>
                        <Paragraph>
                        DXControls for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("3339f8fd-707a-4c9f-bc98-0f4602961282"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXWindowsUIDemos,
					name: "DXWindowsUI",
					displayName: "Windows UI",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "",
					description: @"
                ",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("3339f8fd-707a-4c9f-bc98-0f4602961282"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXNavBarDemos,
					name: "DXNavBar",
					displayName: "Navigation Pane",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "Outlook Side Bars, Windows Explorer Bars and Office 2007 Navigation Pane views available in multiple paint styles.",
					description: @"
                        <Paragraph>
                        Microsoft® Office® Style Side Bar, Navigation Pane and Microsoft Windows® style Explorer Bar Controls in one - with customizable link layouts and complete template support.
                        </Paragraph>
                        <Paragraph>
                        DXNavBar for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("9926c3aa-ac22-445b-ac75-9d5d317a594e"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXSpellCheckerDemos,
					name: "DXSpellChecker",
					displayName: "Spell Checker",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "MS Office style Spell Checker component supporting DevExpress and several standard controls.",
					description: @"
                        <Paragraph>
                        Microsoft® Word® style WPF spell checker with built-in Office® style error correction dialogs and ""check as you type"" support.
                        </Paragraph>
                        <Paragraph>
                        DXSpellChecker for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("3638b382-2e89-4cd1-9bd9-d348faa3da6e"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXCarouselDemos,
					name: "DXCarousel",
					displayName: "Carousel",
					componentName: "WPF Components",
					group: "Multi-Purpose Controls",
					shortDescription: "Fully-customizable Carousel allowing you to build 3D books, iPod style Cover Flow and much more.",
					description: @"
                        <Paragraph>
                        Cutting-edge visualization for galleries and catalogs with unlimited capabilities for element customization along the carousel path.
                        </Paragraph>
                        <Paragraph>
                        DXCarousel for WPF can be purchased as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("6b69dfa5-821d-47f5-8d89-db92b7e1075a"),
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXSpreadsheetDemos,
					name: "DXSpreadsheet",
					displayName: "Spreadsheet",
					componentName: "WPF Components",
					group: "Office Inspired Control Suites",
					shortDescription: "A control that makes it possible for you to add the functionality of a worksheet to a WPF application.",
					description: @"
                        <Paragraph>
                        Provides end-users with the ability to view and edit spreadsheet files in Microsoft® Excel® format. Delivers a comprehensive API allowing you to create, manage, print and convert spreadsheet files. Your application would not require Microsoft Excel to be installed on your computer to accomplish these tasks.
                        </Paragraph>
                        <Paragraph>
                        The DXSpreadsheet Suite is available as part of the Universal, DXperience, or WPF Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateDXTreeMapDemos,
					name: "DXTreeMap",
					displayName: "TreeMap Control",
					componentName: "WPF Components",
					group: "Reporting and Analytics Controls",
					shortDescription: "TODO: Short Description.",
					description: @"
                        <Paragraph>
                        TODO: Description.
                        </Paragraph>",
					licenseInfo: 0x0000004000000000,
					id: Guid.Parse("3c6db67d-44e2-11e5-bf1c-6470020143f0"),
					isAvailableOnline: true
				)
			};
		}
	}
}
