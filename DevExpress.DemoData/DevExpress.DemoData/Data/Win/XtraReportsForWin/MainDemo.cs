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
		static List<Module> Create_XtraReportsForWin_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraReports %MarketingVersion%",
					group: "About",
					type: "XtraReportsDemos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "DrillDown",
					displayName: @"Drill-Down Report",
					group: "Report Types",
					type: "XtraReportsDemos.DrillDownReport.PreviewControl",
					description: @"This demo illustrates a drill-down report with collapsible document sections, which can be expanded by clicking an element in Document Preview.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\DrillDownReport\DrillDownReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\DrillDownReport\DrillDownReport.vb"
					}
				),
				new SimpleModule(demo,
					name: "TableReport",
					displayName: @"Table Report",
					group: "Report Types",
					type: "XtraReportsDemos.TableReport.PreviewControl",
					description: @"This demo illustrates a way of creating a simple tabular report with XtraReports. For this, the XRTable control may be useful. It allows you to quickly draw a table in a report and then maintain all its cells, their text style and borders at once, without having to use numerous separate labels instead. The example creates a report with details on the customer's order. You may enter an appropriate order ID and then a report's datasource is filtered according to the specified value.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\TableReport\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\TableReport\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "MasterDetailReport",
					displayName: @"Master-Detail Report",
					group: "Report Types",
					type: "XtraReportsDemos.MasterDetailReport.PreviewControl",
					description: @"This demo illustrates a quick and easy way of creating a master-detail report in a single report class. For this, the DetailReport bands are used, which are much better than usual subreports for creating a master-detail report. The resulting report represents data from the Northwind database. It consists of three levels of information on products and their suppliers with the product orders grouped by their prices.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\MasterDetailReport\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\MasterDetailReport\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "Subreports",
					displayName: @"Subreports",
					group: "Report Types",
					type: "XtraReportsDemos.Subreports.PreviewControl",
					description: @"This demo illustrates the creation of a master-detail report by placing the Subreport control onto the Detail band of a report.
The report displays data corresponding to a specific date-time range. This range is defined by two parameters that can be assigned custom values using the editors contained in the Parameters panel.
To avoid the selection of irrelevant dates within these editors, their available value range has been restricted.
Note: A different approach to master-detail report generation is illustrated in the Master-Detail Report demo.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\Subreports\MasterReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\Subreports\MasterReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "MultiColumnReport",
					displayName: @"Multi-Column Report",
					group: "Report Types",
					type: "XtraReportsDemos.MultiColumnReport.PreviewControl",
					description: @"This demo illustrates a way of creating a multi-column report with XtraReports. In this report, labels display customer details. A report parameter is used to specify how the labels should be printed - across (in rows) or down (in columns). Note that the labels are displayed using alternating even/odd styles.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\MultiColumnReport\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\MultiColumnReport\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "LabelReport",
					displayName: @"Label Report",
					group: "Report Types",
					type: "XtraReportsDemos.LabelReport.PreviewControl",
					description: @"This demo illustrates how to create labels of any pre-defined size, to print them using a specific label printer. To accomplish this, XtraReports ships an intelligent Label Wizard which contains more than 1500 predefined label types. Note that if you are not satisfied with the label created by the Wizard, you can create your own custom labels.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\BarCodes\ProductLabelsReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\BarCodes\ProductLabelsReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "ReportMerging",
					displayName: @"Report Merging",
					group: "Report Types",
					type: "XtraReportsDemos.ReportMerging.PreviewControl",
					description: @"This demo illustrates how different reports can be combined into one report. This may be useful, for instance, to print multiple documents as a single printer job, or to export many reports into a single PDF file. Note that page orientation in the resulting document may be different for different pages, as well as the page size and other settings (e.g. watermarks, etc.) All this can be correctly previewed and exported with XtraReports, except for continuous export formats (like XLS, TXT, CSV or single-file HTML, MHT, RTF and Image).",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 2,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\MergedReport\MergedReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\MergedReport\MergedReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "SideBySideReports",
					displayName: @"Side-by-Side Reports",
					group: "Report Types",
					type: "XtraReportsDemos.SideBySideReports.PreviewControl",
					description: @"This demo illustrates a report with side-by-side comparison of various employee metrics.
Two Subreport controls are used to inject data from separate employee reports into a single composite document.
To select persons for the comparison, use the drop-down editors in the Parameters panel.
Selecting a value in one of the editors will filter the values displayed by the other and will exclude the selected person from it.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					featuredPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\SideBySideReports\EmployeeComparisonReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\SideBySideReports\EmployeeComparisonReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "NorthwindTradersProductList",
					displayName: @"Products List",
					group: "Real-life Reports",
					type: "XtraReportsDemos.NorthwindTraders.ProductListPreviewControl",
					description: @"This demo illustrates how to create complex real-life reports with XtraReports. It introduces the most advanced XtraReports features - data grouping and filtering, use of different report bands and so on. This example contains the Products List report built from the Northwind database.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\NorthwindTraders\ProductListReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\NorthwindTraders\ProductListReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "NorthwindTradersCatalog",
					displayName: @"Fall Catalog",
					group: "Real-life Reports",
					type: "XtraReportsDemos.NorthwindTraders.CatalogPreviewControl",
					description: @"This demo illustrates a complex real-life report created with XtraReports and introduces such features as data grouping, sorting groups by summary function results, generating a table of contents, etc. In this demo, you can modify the Sort Categories By and Sort Order parameters and click Submit to see how these settings affect the demo report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\NorthwindTraders\CatalogReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\NorthwindTraders\CatalogReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "NorthwindTradersInvoice",
					displayName: @"Invoice",
					group: "Real-life Reports",
					type: "XtraReportsDemos.NorthwindTraders.InvoicePreviewControl",
					description: @"This demo illustrates how to create complex real-life reports with XtraReports. It introduces the most advanced XtraReports features - data grouping and data filtering, using different report bands and some other minor features. This example contains the Invoice report built from the Northwind database, and so this report simulates the Northwind Traders reporting system.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\NorthwindTraders\InvoiceReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\NorthwindTraders\InvoiceReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "ShrinkGrow",
					displayName: @"Shrink & Grow",
					group: "Layout Features",
					type: "XtraReportsDemos.ShrinkGrow.PreviewControl",
					description: @"This demo illustrates how to create a report with a layout consisting of controls with the non-fixed height, which depends on their contents populated at runtime. For this, the Shrink&Grow feature of XtraReports is used. This feature is implemented via the XRControl.CanShrink and XRControl.CanGrow properties (note, that growing and shrinking is possible only for the XRLabel, XRTableCell and XRRichText controls). The report created in this example represents a list of employees from the Northwind database and illustrates how to make a list of items of different height using the Shrink&Grow feature.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\ShrinkGrow\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\ShrinkGrow\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "AnchorVertical",
					displayName: @"Anchoring",
					group: "Layout Features",
					type: "XtraReportsDemos.AnchorVertical.PreviewControl",
					description: @"This demo illustrates the anchoring of report controls.
Toggle the Landscape option in the Parameters panel to generate a different report layout.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\AnchorDemo\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\AnchorDemo\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "PivotGridAndChart",
					displayName: @"PivotGrid And Chart",
					group: "Data Binding",
					type: "XtraReportsDemos.PivotGridAndChart.PreviewControl",
					description: @"This demo illustrates how you can use a linked pair of the XRPivotGrid and XRChart controls in your report. In this demo, a Pivot Grid instance is assigned to a Chart's DataSource property, and the options of this bridging are adjusted for best performance. In Print Preview, you can adjust some of these options in the Parameters panel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\PivotGridAndChart\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\PivotGridAndChart\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "IListDatasource",
					displayName: @"IList Data Source",
					group: "Data Binding",
					type: "XtraReportsDemos.IListDatasource.PreviewControl",
					description: @"This demo illustrates a report bound to an object data source that implements the System.Collections.IList interface.
Report data is obtained from a custom object by using the ObjectDataSource component of the Data Access library.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\IListDataSource\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\IListDataSource\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "CalculatedFieldsReport",
					displayName: @"Calculated Fields",
					group: "Data Binding",
					type: "XtraReportsDemos.CalculatedFieldsReport.PreviewControl",
					description: @"With XtraReports, you can add an unlimited number of calculated fields based upon the values of data fields stored in a datasource, which is bound to a report. This demo enables you to choose the expression of a calculated field displayed in the last column of a report, and see how it is dynamically changed.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\CalculatedFields\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\CalculatedFields\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "MailMerge",
					displayName: @"Mail Merge",
					group: "Data Binding",
					type: "XtraReportsDemos.MailMerge.PreviewControl",
					description: @"This demo illustrates how to implement Mail Merge in XtraReports. For this embedded fields should be used. An embedded field is the name of a data column placed in brackets (""["" and ""]"") in a control's text. This field is automatically recognized by the report builder, and the real data values are substituted in place of the name. The Mail Merge feature allows data of any kind to be inserted: texts, dates, and even pictures. Note that you're able to use embedded fields with the XRLabel, XRRichText, XRTableCell and XRCheckBox controls.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\MailMerge\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\MailMerge\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "HugeAmountRecords",
					displayName: @"Large Quantity of Data",
					group: "Data Binding",
					type: "XtraReportsDemos.HugeAmountRecords.PreviewControl",
					description: @"This demo illustrates the speed of report generation against a dataset with a large amount of data (100,000 records). This is a sample datasource which is populated with typical data (numeric, text, currency, date and Boolean) before starting report creation. In this demo, you can click Refresh and see that the report starts creating its pages. Then you can zoom in and out, scroll through pages, change their watermark and even interrupt report creation via the Stop button in the status bar.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\HugeAmountRecords\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\HugeAmountRecords\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "FormattingRules",
					displayName: @"Conditional Formatting",
					group: "Appearance",
					type: "XtraReportsDemos.FormattingRules.PreviewControl",
					description: @"This demo illustrates how to use formatting rules to conditionally format report controls. With this feature, you can change the appearance of report controls or bands that satisfy a specific condition. Besides color and font settings - you can also change an element's visibility. In this demo, you can select one of the predefined conditions via the Select Condition drop-down list, and the style, which should be applied when the corresponding condition is True.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\FormattingRules\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\FormattingRules\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "OddEvenStyles",
					displayName: @"Odd/Even Styles",
					group: "Appearance",
					type: "XtraReportsDemos.OddEvenStyles.PreviewControl",
					description: @"This demo illustrates the style concept of XtraReports. You're able to create appearance styles for any of the report controls, and then apply it to the control when it's painted. Note that a style can either be always applied to a control, or applied only to odd or even data rows when the control is printed. Look at the odd and even table rows in this report which illustrates this concept. Odd rows are painted with a Light Blue background, while even rows are painted with a White background.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\OddEvenStyles\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\OddEvenStyles\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "Sparkline",
					displayName: @"Sparkline",
					group: "Report Controls",
					type: "XtraReportsDemos.Sparkline.PreviewControl",
					description: @"This demo shows how to use the Sparkline control in your reports.
In this demo, the Sparkline control shows a line chart representing monthly payment statistics for each customer. In this chart, blue and red markers specify the lowest and highest payments respectively.
If required, you can add more conditional markers to the chart in the edit mode.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\Sparkline\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\Sparkline\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "RichText",
					displayName: @"Rich Text",
					group: "Report Controls",
					type: "XtraReportsDemos.RichText.PreviewControl",
					description: @"This example illustrates how to use the XRRichText control in XtraReports. This control allows you to embed rich text (with multiple text formatting options, special fonts, and the capability to insert pictures) in a report.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\RichText\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\RichText\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "BarCodes",
					displayName: @"Bar Code",
					group: "Report Controls",
					type: "XtraReportsDemos.BarCodes.PreviewControl",
					description: @"This demo illustrates how to incorporate barcodes into your report, using the XRBarCode control shipped with the XtraReports suite. In this demo, you can check the ""AutoModule"" option, to control whether or not barcodes should automatically calculate the Module property value, so as to best fit their width.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\BarCodes\BarCodeTypesReport.cs",
						@"\Reporting\VB\DevExpress.DemoReports\BarCodes\BarCodeTypesReport.vb"
					}				
				),
				new SimpleModule(demo,
					name: "Shape",
					displayName: @"Shape",
					group: "Report Controls",
					type: "XtraReportsDemos.Shape.PreviewControl",
					description: @"This demo illustrates how to incorporate shapes into your report, using the XRShape control that is shipped with the XtraReports suite. This example creates a report with the list of all shape types available in XtraReports (rectangle, ellipse, arrow, polygon, star, cross, line, brace, bracket, etc.).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\Shape\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\Shape\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "Charts",
					displayName: @"Chart",
					group: "Report Controls",
					type: "XtraReportsDemos.Charts.PreviewControl",
					description: @"This demo illustrates how to use the XRChart control in XtraReports. In this report, every detail band contains a Bar chart, which shows Products for a specific Category. To create such report, a master-detail datasource is assigned to a report, and a chart's DataMember property is set to the Categories.CategoriesProducts ADO.NET relation. Note that the XRChart control is shipped within XtraReports, so when you purchase XtraReports, you can create charts in your reports. Though to be able to use charts in your Windows Forms and ASP .NET application, you need to purchase the XtraCharts suite separately.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 5,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\Charts\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\Charts\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "PivotGrid",
					displayName: @"Pivot Grid",
					group: "Report Controls",
					type: "XtraReportsDemos.PivotGrid.PreviewControl",
					description: @"This demo shows how to create a cross-tab report. To accomplish this, use the XRPivotGrid control (which can be added to any report band) and then bind it to a report's (or its own) datasource to display data in a matrix form. Note that the XRPivotGrid control is shipped within XtraReports. So when you purchase XtraReports, you can create cross-tabs in your reports. However, to be able to add pivot grids to your Windows Forms and ASP.NET applications, you need to purchase the XtraPivotGrid Suite separately.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 4,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\PivotGrid\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\PivotGrid\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "CrossBandControls",
					displayName: @"Cross-band Controls",
					group: "Report Controls",
					type: "XtraReportsDemos.CrossBandControls.PreviewControl",
					description: @"This demo illustrates how to use the XRCrossBandLine and XRCrossBandBox controls in XtraReports. These controls allow you to draw a line or a rectangle across several bands and through the entire page height.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\CrossBandControls\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\CrossBandControls\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "CustomDraw",
					displayName: @"Custom Control",
					group: "Report Controls",
					type: "XtraReportsDemos.CustomDraw.PreviewControl",
					description: @"This demo illustrates how to use events for custom drawing report controls. In this demo, the XRControl.BeforePrint and XRControl.HtmlItemCreated events are handled, to replace the standard drawing procedures of an XRControl object. This example creates a report containing a list of the 10 most heavily populated countries in the selected continent. The population value is represented graphically by a progress bar, which is drawn using the Custom Draw feature.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\CustomDraw\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\CustomDraw\Report.vb"
					}				
				),
				new SimpleModule(demo,
					name: "TreeView",
					displayName: @"TreeView",
					group: "WinForms Controls",
					type: "XtraReportsDemos.TreeView.PreviewControl",
					description: @"This demo illustrates the capability to print Windows Forms controls using XtraReports, by using PrintableComponentContainer. The report shown in this demo is created based on the DevExpress.XtraTreeList.TreeList control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\DevExpress.DemoReports\TreeView\Report.cs",
						@"\Reporting\VB\DevExpress.DemoReports\TreeView\Report.vb"
					}				
				)
			};
		}
	}
}
