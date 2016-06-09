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
		static List<Module> Create_XtraReportsForXpf_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "MasterDetail",
					displayName: @"Master-Detail Report",
					group: "Report Types",
					type: "ReportWpfDemo.MasterDetail",
					shortDescription: @"This demo illustrates a quick and easy way of creating a master-detail report in a single report class, using the DetailReport bands.",
					description: @"
                        <Paragraph>
                        This demo illustrates a quick and easy way of creating a master-detail report in a single report class. For this, the DetailReport bands are used, which are much better than usual subreports for creating a master-detail report.
                        </Paragraph>
                        <Paragraph>
                        The resulting report represents data from the Northwind database. It consists of three levels of information on products and their suppliers with the product orders grouped by their prices.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "SideBySideReports",
					displayName: @"Side-by-Side Reports",
					group: "Report Types",
					type: "ReportWpfDemo.SideBySideReports",
					shortDescription: @"This demo illustrates a report with side-by-side comparison of various employee metrics.",
					description: @"
                        <Paragraph>
                        Two <Span FontWeight=""Bold"">Subreport</Span> controls are used to inject data from separate employee reports into a single composite document.
                        </Paragraph>
                        <Paragraph>
                        To select persons for the comparison, use the drop-down editors in the <Span FontWeight=""Bold"">Parameters</Span> panel. Selecting a value in one of the editors will filter the values displayed by the other and will exclude the selected person from it.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Table",
					displayName: @"Table Report",
					group: "Report Types",
					type: "ReportWpfDemo.Table",
					shortDescription: @"This demo illustrates a way of creating a simple tabular report with XtraReports, using the XRTable control.",
					description: @"
                        <Paragraph>
                        This demo illustrates a way of creating a simple tabular report with XtraReports. For this, the XRTable control may be useful. It allows you to quickly draw a table in a report and then maintain all its cells, their text style and borders at once, without having to use numerous separate labels instead.
                        </Paragraph>
                        <Paragraph>
                        The example creates a report with details on the customer's order. You may enter an appropriate order ID and then a report's datasource is filtered according to the specified value.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "ReportMerging",
					displayName: @"Report Merging",
					group: "Report Types",
					type: "ReportWpfDemo.ReportMerging",
					shortDescription: @"This demo illustrates how different reports can be combined into one report, e.g. to print multiple documents as a single printer job, or to export many reports into a single PDF file.",
					description: @"
                        <Paragraph>
                        This demo illustrates how different reports can be combined into one report. This may be useful, for instance, to print multiple documents as a single printer job, or to export many reports into a single PDF file.
                        </Paragraph>
                        <Paragraph>
                        Note that page orientation in the resulting document may be different for different pages, as well as the page size and other settings (e.g. watermarks, etc.) All this can be correctly previewed and exported with XtraReports, except for continuous export formats (like XLS, TXT, CSV or single-file HTML, MHT, RTF and Image).
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Subreports",
					displayName: @"Subreports",
					group: "Report Types",
					type: "ReportWpfDemo.Subreports",
					shortDescription: @"This demo illustrates the creation of a master-detail report by placing the Subreport control onto the Detail band of a report.",
					description: @"
                        <Paragraph>
                        The report displays data corresponding to a specific date-time range. This range is defined by two parameters that can be assigned custom values using the editors contained in the <Span FontWeight=""Bold"">Parameters</Span> panel.
                        </Paragraph>
                        <Paragraph>
                        To avoid the selection of irrelevant dates within these editors, their available value range has been restricted.
                        </Paragraph>
                        <Paragraph>
                        Note: A different approach to master-detail report generation is illustrated in the <Span FontWeight=""Bold"">Master-Detail Report</Span> demo.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "MultiColumn",
					displayName: @"Multi-Column Report",
					group: "Report Types",
					type: "ReportWpfDemo.MultiColumn",
					shortDescription: @"This demo illustrates a way of creating a multi-column report with XtraReports. In this report, the labels are used to display customers' details.",
					description: @"
                        <Paragraph>
                        This demo illustrates a way of creating a multi-column report with XtraReports. In this report, labels display customer details.
                        </Paragraph>
                        <Paragraph>
                        A report parameter is used to specify how the labels should be printed - across (in rows) or down (in columns). Note that the labels are displayed using alternating even/odd styles.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LabelReport",
					displayName: @"Label Report",
					group: "Report Types",
					type: "ReportWpfDemo.LabelReport",
					shortDescription: @"This demo illustrates how to create labels of any predefined size to be later on printed on a specific label printer.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create labels of any pre-defined size, to print them using a specific label printer. To accomplish this, XtraReports ships an intelligent Label Wizard which contains more than 1500 predefined label types.
                        </Paragraph>
                        <Paragraph>
                        Note that if you are not satisfied with the label created by the Wizard, you can create your own custom labels.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DrillDown",
					displayName: @"Drill-Down Report",
					group: "Report Types",
					type: "ReportWpfDemo.DrillDown",
					shortDescription: @"This demo illustrates the capability to create interactive reports with XtraReports.",
					description: @"
                        <Paragraph>
                        This demo illustrates a drill-down report with collapsible document sections, which can be expanded by clicking an element in Document Preview.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "ProductList",
					displayName: @"Product List",
					group: "Real-Life Reports",
					type: "ReportWpfDemo.ProductList",
					shortDescription: @"This demo shows how to create complex real-life reports, and introduces the most advanced features - data grouping and data filtering, using different report bands and so on.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create complex real-life reports with XtraReports. It introduces the most advanced XtraReports features - data grouping and filtering, use of different report bands and so on.
                        </Paragraph>
                        <Paragraph>
                        This example contains the Products List report built from the Northwind database.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FallCatalog",
					displayName: @"Fall Catalog",
					group: "Real-Life Reports",
					type: "ReportWpfDemo.FallCatalog",
					shortDescription: @"This demo illustrates some of the most advanced features that help you easily create complex real-life reports.",
					description: @"
                        <Paragraph>
                        This demo illustrates a complex real-life report created with XtraReports and introduces such features as data grouping, sorting groups by summary function results, generating a table of contents, etc.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can modify the Sort Categories By and Sort Order parameters and click Submit to see how these settings affect the demo report.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Invoice",
					displayName: @"Invoice",
					group: "Real-Life Reports",
					type: "ReportWpfDemo.Invoice",
					shortDescription: @"This demo shows how to create complex real-life reports, and introduces the most advanced features - data grouping and data filtering, using different report bands and so on.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create complex real-life reports with XtraReports. It introduces the most advanced XtraReports features - data grouping and data filtering, using different report bands and some other minor features.
                        </Paragraph>
                        <Paragraph>
                        This example contains the Invoice report built from the Northwind database, and so this report simulates the Northwind Traders reporting system.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ShrinkGrow",
					displayName: @"Shrink & Grow",
					group: "Layout Features",
					type: "ReportWpfDemo.ShrinkGrow",
					shortDescription: @"This demo shows a report containing controls with the non-fixed height, which depends on their contents populated at runtime, using the Shrink&Grow feature.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a report with a layout consisting of controls with the non-fixed height, which depends on their contents populated at runtime. For this, the Shrink&amp;Grow feature of XtraReports is used.
                        </Paragraph>
                        <Paragraph>
                        This feature is implemented via the XRControl.CanShrink and XRControl.CanGrow properties (note, that growing and shrinking is possible only for the XRLabel, XRTableCell and XRRichText controls). The report created in this example represents a list of employees from the Northwind database and illustrates how to make a list of items of different height using the Shrink&amp;Grow feature.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "VerticalAnchoring",
					displayName: @"Anchoring",
					group: "Layout Features",
					type: "ReportWpfDemo.VerticalAnchoring",
					shortDescription: @"This demo illustrates the anchoring of report controls.",
					description: @"
                        <Paragraph>
                        Toggle the <Span FontWeight=""Bold"">Landscape</Span> option in the <Span FontWeight=""Bold"">Parameters</Span> panel to generate a different report layout.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "LargeQuantityOfData",
					displayName: @"Large Quantity of Data",
					group: "Data Binding",
					type: "ReportWpfDemo.LargeQuantityOfData",
					shortDescription: @"This demo illustrates the capability to show a report document during its generation.",
					description: @"
                        <Paragraph>
                        This demo illustrates the speed of report generation against a dataset with a large amount of data (100,000 records). This is a sample datasource which is populated with typical data (numeric, text, currency, date and Boolean) before starting report creation.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can click Refresh and see that the report starts creating its pages. Then you can zoom in and out, scroll through pages and even interrupt report creation via the Stop button in the status bar.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "IListDataSource",
					displayName: @"IList Data Source",
					group: "Data Binding",
					type: "ReportWpfDemo.IListDataSource",
					shortDescription: @"This demo illustrates a report bound to an object data source that implements the System.Collections.IList interface.",
					description: @"
                        <Paragraph>
                        Report data is obtained from a custom object by using the <Span FontWeight=""Bold"">ObjectDataSource</Span> component of the Data Access library.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "CalculatedFields",
					displayName: @"Calculated Fields",
					group: "Data Binding",
					type: "ReportWpfDemo.CalculatedFields",
					shortDescription: @"This demo illustrates the capability add an unlimited number of calculated fields based upon the values of data fields stored in a report's data source.",
					description: @"
                        <Paragraph>
                        With XtraReports, you can add an unlimited number of calculated fields based upon the values of data fields stored in a datasource, which is bound to a report.
                        </Paragraph>
                        <Paragraph>
                        This demo enables you to choose the expression of a calculated field displayed in the last column of a report, and see how it is dynamically changed.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "MailMerge",
					displayName: @"Mail Merge",
					group: "Data Binding",
					type: "ReportWpfDemo.MailMerge",
					shortDescription: @"This demo illustrates how to implement Mail Merge in XtraReports, using embedded fields.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to implement Mail Merge in XtraReports. For this embedded fields should be used. An embedded field is the name of a data column placed in square brackets in a control's text. This field is automatically recognized by the report builder, and the real data values are substituted in place of the name.
                        </Paragraph>
                        <Paragraph>
                        The Mail Merge feature allows data of any kind to be inserted: texts, dates, and even pictures. Note that you're able to use embedded fields with the XRLabel, XRRichText, XRTableCell and XRCheckBox controls.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ConditionalFormatting",
					displayName: @"Conditional Formatting",
					group: "Appearance",
					type: "ReportWpfDemo.ConditionalFormatting",
					shortDescription: @"This demo shows how to use formatting rules, to conditionally change the visibility and appearance of report controls.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use formatting rules to conditionally format report controls. With this feature, you can change the appearance of report controls or bands that satisfy a specific condition. Besides color and font settings - you can also change an element's visibility.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can select one of the predefined conditions via the Select Condition drop-down list, and the style, which should be applied when the corresponding condition is True.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "OddEvenStyles",
					displayName: @"Odd/Even Styles",
					group: "Appearance",
					type: "ReportWpfDemo.OddEvenStyles",
					shortDescription: @"This demo illustrates the capability to create appearance styles for any of the report controls, and then apply it to the control when it's painted.",
					description: @"
                        <Paragraph>
                        This demo illustrates the style concept of XtraReports. You're able to create appearance styles for any of the report controls, and then apply it to the control when it's painted. Note that a style can either be always applied to a control, or applied only to odd or even data rows when the control is printed.
                        </Paragraph>
                        <Paragraph>
                        Look at the odd and even table rows in this report which illustrates this concept. Odd rows are painted with a Light Blue background, while even rows are painted with a White background.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PivotGridAndChart",
					displayName: @"Pivot Grid and Chart",
					group: "Report Controls",
					type: "ReportWpfDemo.PivotGridAndChart",
					shortDescription: @"This demo illustrates how you can use a linked pair of the XRPivotGrid and XRChart controls in your report.",
					description: @"
                        <Paragraph>
                        This demo illustrates how you can use a linked pair of the XRPivotGrid and XRChart controls in your report.
                        </Paragraph>
                        <Paragraph>
                        In this demo, a Pivot Grid instance is assigned to a Chart's DataSource property, and the options of this bridging are adjusted for best performance. In Print Preview, you can adjust some of these options in the Parameters panel.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Sparkline",
					displayName: @"Sparkline",
					group: "Report Controls",
					type: "ReportWpfDemo.Sparkline",
					shortDescription: @"This demo shows how to use the Sparkline control in your reports.",
					description: @"
                        <Paragraph>
                        In this demo, the Sparkline control shows a line chart representing monthly payment statistics for each customer. In this chart, blue and red markers specify the lowest and highest payments respectively. If required, you can add more conditional markers to the chart in the edit mode.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BarCodes",
					displayName: @"Bar Code",
					group: "Report Controls",
					type: "ReportWpfDemo.BarCodes",
					shortDescription: @"This demo illustrates how to incorporate barcodes into your report, using the XRBarCode control shipped with the XtraReports suite.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to incorporate barcodes into your report, using the XRBarCode control shipped with the XtraReports suite.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can check the ""AutoModule"" option, to control whether or not barcodes should automatically calculate the Module property value, so as to best fit their width.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RichText",
					displayName: @"Rich Text",
					group: "Report Controls",
					type: "ReportWpfDemo.RichText",
					shortDescription: @"This example demonstrates how to use the XRRichText control in XtraReports, for inserting rich text (with various text formatting, special fonts, pictures, and so on).",
					description: @"
                        <Paragraph>
                        This example illustrates how to use the XRRichText control in XtraReports. This control allows you to embed rich text (with multiple text formatting options, special fonts, and the capability to insert pictures) in a report.
                        </Paragraph>
                        <Paragraph>
                        The demo creates a report containing some information about cars from the CarDB database via binding data fields to the bindable XRRichText.Rtf of the XRRichText control.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PivotGrid",
					displayName: @"Pivot Grid",
					group: "Report Controls",
					type: "ReportWpfDemo.PivotGrid",
					shortDescription: @"This demo shows how to create a cross-tab report, using the XRPivotGrid control (which can be added to any report band).",
					description: @"
                        <Paragraph>
                        This demo shows how to create a cross-tab report. To accomplish this, use the XRPivotGrid control (which can be added to any report band) and then bind it to a report's (or its own) datasource to display data in a matrix form.
                        </Paragraph>
                        <Paragraph>
                        Note that the XRPivotGrid control is shipped within XtraReports. So when you purchase XtraReports, you can create cross-tabs in your reports. However, to be able to add pivot grids to your Windows Forms and ASP.NET applications, you need to purchase the XtraPivotGrid Suite separately.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Charts",
					displayName: @"Chart",
					group: "Report Controls",
					type: "ReportWpfDemo.Charts",
					shortDescription: @"This demo shows how to use the XRChart control in XtraReports. In this report, every detail band contains a Bar chart, which shows Products for a specific Category.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the XRChart control in XtraReports. In this report, every detail band contains a Bar chart, which shows Products for a specific Category. To create such report, a master-detail datasource is assigned to a report, and a chart's DataMember property is set to the Categories.CategoriesProducts ADO.NET relation.
                        </Paragraph>
                        <Paragraph>
                        Note that the XRChart control is shipped within XtraReports, so when you purchase XtraReports, you can create charts in your reports. Though to be able to use charts in your Windows Forms and ASP .NET application, you need to purchase the XtraCharts suite separately.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Shape",
					displayName: @"Shape",
					group: "Report Controls",
					type: "ReportWpfDemo.Shape",
					shortDescription: @"This demo illustrates how to incorporate shapes into your report, using the XRShape control shipped with the XtraReports suite.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to incorporate shapes into your report, using the XRShape control that is shipped with the XtraReports suite.
                        </Paragraph>
                        <Paragraph>
                        This example creates a report with the list of all shape types available in XtraReports (rectangle, ellipse, arrow, polygon, star, cross, line, brace, bracket, etc.).
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CrossBandControls",
					displayName: @"Cross-band Controls",
					group: "Report Controls",
					type: "ReportWpfDemo.CrossBandControls",
					shortDescription: @"This demo illustrates how to use the XRCrossBandLine and XRCrossBandBox controls in XtraReports, to draw a line or a box across the entire page.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the XRCrossBandLine and XRCrossBandBox controls in XtraReports. These controls allow you to draw a line or a rectangle across several bands and through the entire page height.
                        </Paragraph>
                        <Paragraph>
                        Another way to fill an empty area on every page is to handle the XtraReport.FillEmptySpace event, and add all required controls at runtime. Choose the ""Draw Z below the table"" option to see how this can be done.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomControl",
					displayName: @"Custom Control",
					group: "Report Controls",
					type: "ReportWpfDemo.CustomControl",
					shortDescription: @"This demo illustrates how to use events for custom drawing report controls.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use events for custom drawing report controls. In this demo, the XRControl.BeforePrint and XRControl.HtmlItemCreated events are handled, to replace the standard drawing procedures of an XRControl object.
                        </Paragraph>
                        <Paragraph>
                        This example creates a report containing a list of the 10 most heavily populated countries in the selected continent. The population value is represented graphically by a progress bar, which is drawn using the Custom Draw feature.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
			};
		}
	}
}
