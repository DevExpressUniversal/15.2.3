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
		static List<Product> CreateWinProducts(Platform p) {
			return new List<Product> {
				new Product(p,
					CreateXtraGridDemos,
					name: "XtraGrid",
					displayName: "Data Grid and Editors",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Multiple data view types, including Grid View, Contact View, Free-Layout View, Carousel View and Banded Grid Views.",
					description: @"
                        <Paragraph>
                        Over 40 controls in one integrated WinForms control suite - Compare the features, capabilities, and options of the XtraGrid Suite to the competition and you'll find that no other product suite compares.
                        </Paragraph>
                        <Paragraph>
                        The XtraGrid Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000001,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraReportsForWinDemos,
					name: "XtraReportsForWin",
					displayName: "Reporting",
					componentName: "XtraReportsWin",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "DevExpress Reporting platform showcased within a WinForms application.",
					description: @"
                        <Paragraph>
                        In addition to the feature-complete report building engine fully integrated into Microsoft® Visual Studio®, the XtraReports Suite provides Windows Forms developers with fully-customizable report preview controls, Ribbon-based UI and End-User Report Designer.
                        </Paragraph>
                        <Paragraph>
                        The XtraReports Suite allows you to integrate reports into Windows Forms, ASP.NET, WPF and Silverlight applications.
                        </Paragraph>",
					licenseInfo: 0x0000000000000010,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraPivotGridDemos,
					name: "XtraPivotGrid",
					displayName: "Pivot Grid",
					componentName: "Windows Forms",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "MS Excel style Pivot Table Control for multi-dimensional data analysis and cross-tab report generation.",
					description: @"
                        <Paragraph>
                        Full support for user customization and native integration with DevExpress Charting Control allow end-users to produce a nearly endless array of reports by simply dragging and clicking.
                        </Paragraph>
                        <Paragraph>
                        The XtraPivotGrid Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000100,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraChartsDemos,
					name: "XtraCharts",
					displayName: "Charting",
					componentName: "Windows Forms",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "Chart Control for WinForms, providing over 30 view types for 2D charting and over 20 view types for 3D charting.",
					description: @"
                        <Paragraph>
                        Over 55 total chart types for 2D and 3D charting, featuring end-user zooming, scrolling, rotation and native integration with DevExpress OLAP data mining control.
                        </Paragraph>
                        <Paragraph>
                        The XtraCharts Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000400,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraBarsDemos,
					name: "XtraBars",
					displayName: "Ribbon and Menu",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Ribbon, Toolbars, Main Menu, Context Menu, Status Bar Controls.",
					description: @"
                        <Paragraph>
                        A complete set of Microsoft® Office® and Visual Studio® style navigation controls in an integrated and cohesive package.
                        </Paragraph>
                        <Paragraph>
                        The XtraBars Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraSpreadsheetDemos,
					name: "XtraSpreadsheet",
					displayName: "Spreadsheet",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "A control that makes it possible for you to add the functionality of a worksheet to a WinForms application.",
					description: @"
                        <Paragraph>
                        Provides end-users with the ability to view and edit spreadsheet files in Microsoft® Excel® format. Delivers a comprehensive API allowing you to create, manage, print and convert spreadsheet files. Your application would not require Microsoft Excel to be installed on your computer to accomplish these tasks.
                        </Paragraph>
                        <Paragraph>
                        The XtraSpreadsheet Suite is available as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraRichEditDemos,
					name: "XtraRichEdit",
					displayName: "Word Processing RTF",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Microsoft Word® style Rich Text Editor with the complete built-in UI and Mail Merge functionality.",
					description: @"
                        <Paragraph>
                        Rich text formatting and editing capabilities, including Mail Merge, in a Microsoft Office® style user interface.
                        </Paragraph>
                        <Paragraph>
                        The XtraRichEdit Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000800000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraTreeListDemos,
					name: "XtraTreeList",
					displayName: "Multi-Column TreeView",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Data-aware or unbound TreeView-Grid combination, providing newsreader style UI.",
					description: @"
                        <Paragraph>
                        Create data-aware or unbound tree views or hierarchical newsreader style grids with full data editing suppport.
                        </Paragraph>
                        <Paragraph>
                        The XtraTreeList Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000020,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraSchedulerDemos,
					name: "XtraScheduler",
					displayName: "Calendar and Scheduling",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Outlook® style Day, Week, Month and Timeline Views with over 15 auxiliary scheduling controls.",
					description: @"
                        <Paragraph>
                        Features multiple calendar view types and side-by-side calendar display. Ships with complete Microsoft Outlook® style end-user interface.
                        </Paragraph>
                        <Paragraph>
                        The XtraScheduler Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000200,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraEditorsDemos,
					name: "XtraEditors",
					displayName: "Data Editors and Controls",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "25 embeddable data editors, 22 common controls and six components for centralized behavior and appearance control.",
					description: @"
                        <Paragraph>
                        Full support for DevExpress Skinning technology and uncompromising data editing capabilities for use in standalone controls or within data-aware containers.
                        </Paragraph>
                        <Paragraph>
                        The XtraEditors Library can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000002,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraNavBarDemos,
					name: "XtraNavBar",
					displayName: "Navigation Bar",
					componentName: "Windows Forms",
					group: "Visual Studio Inspired Control Suites",
					shortDescription: "Outlook Side Bars, Windows Explorer Bars and Office 2007 Navigation Pane views available in numerous paint styles.",
					description: @"
                        <Paragraph>
                        Microsoft® Office® Style Side Bar, Navigation Pane and Microsoft Windows® style Explorer Bar Controls in one - with embedded controls and customizable link layouts.
                        </Paragraph>
                        <Paragraph>
                        The XtraNavBar can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000080,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraLayoutControlDemos,
					name: "XtraLayoutControl",
					displayName: "Automatic Form Layout Control",
					componentName: "Windows Forms",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Data-bound or unbound layout customization control and a conversion component allowing you to adopt existing forms.",
					description: @"
                        <Paragraph>
                        Use simple drag and drop operations to create and support user-customizable forms with automatic control arrangement and proportional resizing.
                        </Paragraph>
                        <Paragraph>
                        The XtraLayoutControl Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000800,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraPdfViewerDemos,
					name: "XtraPdfViewer",
					displayName: "PDF Viewer",
					componentName: "Windows Forms",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "Supports zooming, scrolling, embedded fonts, continuous page layout and more.",
					description: @"
                        <Paragraph>
                        Use the PDF Viewer control to display PDF files right in your WinForms application without the need to install an external PDF viewer application on your end users' machine. This control supports zooming, scrolling, embedded fonts, continuous page layout and provides a ready-to-go Ribbon tab, all of which can be quickly incorporated in your application.
                        </Paragraph>
                        <Paragraph>
                        The PDF Viewer can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0200000000000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraVerticalGridDemos,
					name: "XtraVerticalGrid",
					displayName: "Property Grid",
					componentName: "Windows Forms",
					group: "Visual Studio Inspired Control Suites",
					shortDescription: "Data-bound or unbound rotated Grid Control, plus a Property Grid allowing you to inspect and edit object properties.",
					description: @"
                        <Paragraph>
                        Data-aware, unbound and Property Grid data access modes combined with multi-record or single-record data representation.
                        </Paragraph>
                        <Paragraph>
                        The XtraVerticalGrid Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000040,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraMapDemos,
					name: "XtraMap",
					displayName: "Mapping",
					componentName: "Windows Forms",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "Supports Bing Maps, OpenStreetMap, unlimited number of map layers and loading shapes from Shapefile and KML formats.",
					description: @"
                        <Paragraph>
                        The Map Control for WinForms provides all functionality required to embed popular map services into your applications. You are free to choose from any existing map data resource (like Bing Maps™ or OpenStreetMap) or establish your own map data server inside your corporate network. In addition to using raster map images, you can also utilize vector elements of any shape, stored either in Shapefiles or other formats. The control has built-in navigation elements, supports animated zooming, element highlighting and so much more...
                        </Paragraph>
                        <Paragraph>
                        The Map Control can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000800,
					isAvailableOnline: false
				),
				new Product(p,
					CreateSnapForWinDemos,
					name: "SnapForWin",
					displayName: "WYSIWYG\r\nReport Writer",
					componentName: "XtraReportsWin",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "Word Processing meets Banded Report Design: A Microsoft Word Inspired WinForms reporting platform.",
					description: @"
                        <Paragraph>
                        Combining extended data shaping capabilities with an intuitive and familiar user interface, Snap is a bold statement on the market of end-user reporting solutions. Let your end-users create virtually any report with no need to overcome a steep learning curve, and export their reports to plenty of formats.
                        </Paragraph>
                        <Paragraph>
                        The Snap can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0002000000000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateTilesPhonyDemos,
					name: "TilesPhony",
					displayName: "Tiles",
					componentName: "Windows Forms",
					group: "Windows 8 Inspired Control Suites",
					shortDescription: "Ribbon, Toolbars, Main Menu, Context Menu, Status Bar Controls, Dock Windows and Tabbed MDI Interfaces.",
					description: @"
                ",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				),
				new Product(p,
					CreateDockingPhonyDemos,
					name: "DockingPhony",
					displayName: "Docking",
					componentName: "Windows Forms",
					group: "Visual Studio Inspired Control Suites",
					shortDescription: "Multiple data view types, including Grid View, Contact View, Free-Layout View, Carousel View and Banded Grid Views.",
					description: @"
                ",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraGaugesDemos,
					name: "XtraGauges",
					displayName: "Gauge and Indicators",
					componentName: "Windows Forms",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "Five Circular, two Linear, three Digital and State Indicator Gauge types. 21 appearance options provide over 225 gauge presets.",
					description: @"
                        <Paragraph>
                        Over 180 ready-to-use gauge presets, including circular, digital, linear and state indicator gauges.
                        </Paragraph>
                        <Paragraph>
                        The XtraGauges Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000100000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraSpellCheckerDemos,
					name: "XtraSpellChecker",
					displayName: "Spell Checker",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "MS Office style Spell Checker component supporting DevExpress and several standard controls.",
					description: @"
                        <Paragraph>
                        Microsoft® Word® style WinForms spell checker with built-in Office® style error correction dialogs and ""check as you type"" support.
                        </Paragraph>
                        <Paragraph>
                        The XtraSpellChecker can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000001000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateWin8UIManagerPhonyDemos,
					name: "Win8UIManagerPhony",
					displayName: "Windows 8 UI Manager",
					componentName: "Windows Forms",
					group: "Windows 8 Inspired Control Suites",
					shortDescription: "Target next-gen Windows devices and bring the power of touch and Modern UI to your next WinForms project.",
					description: @"
                ",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraPrintingDemos,
					name: "XtraPrinting",
					displayName: "Print Your UI\r\nand Data Export",
					componentName: "Windows Forms",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "A powerhouse control suite allowing you to bring your UI to the printed page.",
					description: @"
                        <Paragraph>
                        Instantly create complex reports by rendering DevExpress UI controls. Export your reports to PDF, XLSX, RTF, HTML and other popular formats.
                        </Paragraph>
                        <Paragraph>
                        The XtraPrinting Library can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000008,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraWizardDemos,
					name: "XtraWizard",
					displayName: "Form Wizard",
					componentName: "Windows Forms",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Easily create multi-step dialogs supporting Wizard 97 or Wizard Aero standard.",
					description: @"
                        <Paragraph>
                        Build Wizard 97 or Windows® Aero wizards with superior appearance and pixel-perfect element layout.
                        </Paragraph>
                        <Paragraph>
                        The XtraWizard can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000200000000,
					isAvailableOnline: false
				),
				new Product(p,
					CreateApplicationUIDemos,
					name: "ApplicationUI",
					displayName: "Desktop UI\r\nManager",
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Dock Windows, Widgets, Tabbed UI Interfaces and Multi-Purpose Controls.",
					description: @"
                        <Paragraph>
                        A complete set of Microsoft® Office® and Visual Studio® style navigation and layout controls in an integrated and cohesive package.
                        </Paragraph>
                        <Paragraph>
                        The XtraBars Suite can be purchased as part of the Universal, DXperience, or WinForms Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				),
				new Product(p,
					CreateMVVMDemos,
					name: "MVVM",
					displayName: "MVVM Best Practices",
					componentName: "Windows Forms",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Application Infrastructural MVVM pattern best practices.",
					description: @"
                        <Paragraph>
                        DevExpress MVVM Best Practices.
                        </Paragraph>
                        <Paragraph>
                        Elegant and fully adaptable infrastructural pattern for the WinForms platform powered by DevExpress Desktop Controls.
                        </Paragraph>",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				),
				new Product(p,
					CreateXtraDiagramControlDemos,
					name: "XtraDiagramControl",
					displayName: "Diagrams",  
					componentName: "Windows Forms",
					group: "Office Inspired Control Suites",
					shortDescription: "Diagram Control short description",
					description: @"
                        <Paragraph>
                        Diagram Control
                        </Paragraph>
                        <Paragraph>
                        Long description
                        </Paragraph>",
					licenseInfo: 0x0000000000000004,
					isAvailableOnline: false
				)
			};
		}
	}
}
