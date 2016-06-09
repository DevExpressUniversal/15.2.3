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
		static List<Product> CreateMvcProducts(Platform p) {
			return new List<Product> {
				new Product(p,
					CreateMVCxGridViewDemos,
					name: "GridView",
					displayName: "Grid and Data Editors",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "Blazing fast Data Grid with extensive data shaping options and a lightweight memory footprint.",
					description: @"
                        <Paragraph>
                        Unlimited master-detail levels, Web Accessibility support and unmatched end-user data analysis capabilities.
                        </Paragraph>
                        <Paragraph>
                        The GridView MVC extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000010000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxCardViewDemos,
					name: "CardView",
					displayName: "Card View",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "",
					description: "",
					licenseInfo: 0x0000000000010000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateXtraReportsForMvcDemos,
					name: "Report",
					displayName: "Reporting Tool",
					componentName: "XtraReportsWeb",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "DevExpress Reporting platform showcased within an ASP.NET MVC application.",
					description: @"
                        <Paragraph>
                        In addition to the feature-complete report building engine fully integrated into Microsoft® Visual Studio®, the XtraReports Suite provides ASP.NET developers with a Report Viewer control and a standalone Report Navigation Toolbar.
                        </Paragraph>
                        <Paragraph>
                        The XtraReports Suite allows you to integrate reports into Windows Forms, ASP.NET, ASP.NET MVC, WPF and Silverlight applications.
                        </Paragraph>",
					licenseInfo: 0x0000000000000010,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxDockAndPopupsDemos,
					name: "DockAndPopups",
					displayName: "Dock and Modal Popups",
					componentName: "ASP.NET",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Controls you need to create interactive, touch-friendly web solutions like touch-boards and web dashboards.",
					description: @"
                        <Paragraph>
                        All the controls you’ll need to create interactive, touch-friendly web solutions like touch-boards and web dashboards.
                        </Paragraph>
                        <Paragraph>
                        The Dock and Modal Popups MVC extensions can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000001000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxFileManagerAndUploadDemos,
					name: "FileManagerAndUpload",
					displayName: "File Manager and File Upload",
					componentName: "ASP.NET",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Controls allowing you to instantly introduce file management capabilities in your web application.",
					description: @"
                        <Paragraph>
                        A standalone Multi-File Upload Manager and a pre-built File Manager allow you to instantly introduce file management capabilities in your web applications.
                        </Paragraph>
                        <Paragraph>
                        The File Manager and File Upload extensions can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000001000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxImageAndDataNavigationDemos,
					name: "ImageAndDataNavigation",
					displayName: "Image and Data Navigation",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "Multi-purpose image and data navigation tools designed to address a wide-range of business use scenarios.",
					description: @"
                        <Paragraph>
                        A rich collection of multi-purpose image and data browsing tools designed to address a wide-range of business use scenarios.
                        </Paragraph>
                        <Paragraph>
                        The Image and Data Navigation extensions can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000001000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxNavigationAndLayoutDemos,
					name: "NavigationAndLayout",
					displayName: "Navigation and Layout",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "Form navigation and layout tools designed to present information on screen more effectively regardless of information complexity.",
					description: @"
                        <Paragraph>
                        A comprehensive set of site navigation and layout tools designed to more effectively present information on screen regardless of the information complexity.
                        </Paragraph>
                        <Paragraph>
                        The Navigation and Layout extensions can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000001000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxMultiUseControlsDemos,
					name: "MultiUseExtensions",
					displayName: "Multi-Use Site Extensions",
					componentName: "ASP.NET",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Multi-use controls and components deliver the exceptional functionality to your web application in the shortest possible time.",
					description: @"
                        <Paragraph>
                        A collection of multi-use site controls is everything you need to build your best web application and deliver exceptional functionality in the shortest possible time frame.
                        </Paragraph>
                        <Paragraph>
                        The Multi-Use Site extensions can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000001000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateXtraChartsForMvcDemos,
					name: "Chart",
					displayName: "Charting",
					componentName: "ASP.NET",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "Chart Control providing over 45 total chart types for 2D and 3D charting with native integration with DevExpress OLAP data mining controls.",
					description: @"
                        <Paragraph>
                        Over 55 total chart types for 2D and 3D charting, featuring end-user zooming, scrolling, rotation and native integration with DevExpress OLAP data mining control.
                        </Paragraph>
                        <Paragraph>
                        The Chart extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000000400,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxPivotGridDemos,
					name: "PivotGrid",
					displayName: "OLAP Data Mining",
					componentName: "ASP.NET",
					group: "Reporting and Analytics Control Suites",
					shortDescription: "MS Excel style Pivot Table Control for multi-dimensional data analysis.",
					description: @"
                        <Paragraph>
                        Simply bind the control to data to allow end-users to slice and dice data and thus generate a nearly endless array of cross-tab reports.
                        </Paragraph>
                        <Paragraph>
                        The PivotGrid extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000000400000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxSchedulerDemos,
					name: "Scheduler",
					displayName: "Calendar and Scheduling",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "Multiple view types from single day to month and timeline views, side-by-side calendars and complete AJAX callback support.",
					description: @"
                        <Paragraph>
                        Multiple view types from single day to month and timeline views, side-by-side calendars and complete AJAX callback support.
                        </Paragraph>
                        <Paragraph>
                        The Scheduler extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000004000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxSpreadsheetDemos,
					name: "Spreadsheet",
					displayName: "Spreadsheet",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "",
					description: @"
                ",
					licenseInfo: 0x0000000000010000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxRichEditDemos,
					name: "RichEdit",
					displayName: "Rich Edit",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "",
					description: @"
                ",
					licenseInfo: 0x0000000002000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxHtmlEditorDemos,
					name: "HtmlEditor",
					displayName: "HTML Editor",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "Synchronized WYSIWYG and HTML code editors with integrated toolbars and menus for MS Word style user experience.",
					description: @"
                        <Paragraph>
                        Microsoft Word® style WYSIWYG interface synchronized with HTML code editor - all built using DevExpress ASP.NET controls.
                        </Paragraph>
                        <Paragraph>
                        The HtmlEditor extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000020000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxTreeListDemos,
					name: "TreeList",
					displayName: "TreeView-Grid Hybrid",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "TreeView-Grid hybrid control that can display information as a tree, a grid, or a combination of both - in either data bound or an unbound mode.",
					description: @"
                        <Paragraph>
                        Fully supports data-bound and unbound modes, on-demand child node loading, multiple built-in themes and ASP.NET Templates.
                        </Paragraph>
                        <Paragraph>
                        The TreeList extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000080000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxEditorsDemos,
					name: "Editors",
					displayName: "Data Editors",
					componentName: "ASP.NET",
					group: "Multi-Purpose Control Suites",
					shortDescription: "Data Editors library with Advanced Masked Input and Data Validation support.",
					description: @"
                        <Paragraph>
                        Over 20 advanced editors that can be used either standalone or within container controls.
                        </Paragraph>
                        <Paragraph>
                        The Data Editors extensions can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000008000000,
					isAvailableOnline: true
				),
				new Product(p,
					CreateMVCxSpellCheckerDemos,
					name: "SpellChecker",
					displayName: "Spell Checker",
					componentName: "ASP.NET",
					group: "Office Inspired Control Suites",
					shortDescription: "MS Office style Spell Checker component supporting DevExpress and a few standard controls.",
					description: @"
                        <Paragraph>
                        Provides built-in Office style error correction dialogs built entirely on DevExpress ASP.NET MVC Extensions.
                        </Paragraph>
                        <Paragraph>
                        The SpellChecker MVC extension can be purchased as part of the Universal, DXperience, or ASP.NET Subscription.
                        </Paragraph>",
					licenseInfo: 0x0000000040000000,
					isAvailableOnline: true
				)
			};
		}
	}
}
