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
		static List<Module> Create_XtraSpreadsheet_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraSpreadsheet %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraSpreadsheet.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "StatisticFunctions",
					displayName: @"Trend Analysis",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.StatisticFunctionsModule",
					description: @"This demo illustrates how to use the spreadsheet's statistical functions to perform trend analysis.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\StatisticFunctions.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\StatisticFunctions.vb"
					}
				),
				new SimpleModule(demo,
					name: "InvoiceWithTotal",
					displayName: @"Invoice",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.InvoiceWithTotalModule",
					description: @"This demo helps illustrate the power of the XtraSpreadsheet’s API. The sample invoice template is generate in code at runtime. Switch to the Data tab to modify the contents of an order, then return to the Invoice tab to view the results. As you can imagine, the invoice is a live document so you can change product quantity, price and discount values. All invoice values are then re-calculated automatically.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\InvoiceWithTotal.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\InvoiceWithTotal.vb"
					}
				),
				new SimpleModule(demo,
					name: "LoanAmortizationSchedule",
					displayName: @"Loan Amortization Schedule",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.LoanAmortizationScheduleModule",
					description: @"This demo illustrates how to use the XtraSpreadsheet to create a loan amortization schedule template and automatically calculate a loan schedule. Give it a try... modify the values stored in the loan amount, annual interest rate, loan period in years, number of payments, start date and optional payments cells and see how fast the spreadsheet re-calculates the loan schedule.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\LoanAmortizationSchedule.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\LoanAmortizationSchedule.vb"
					}
				),
				new SimpleModule(demo,
					name: "ExpenseReport",
					displayName: @"Expense Report",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.ExpenseReportModule",
					description: @"In this demo, we illustrate the power of the XtraSpreadsheet’s API and how you can generate an expense report in code at runtime. And remember, with built-in printing and export support, you can distribute spreadsheet documents such as this throughout your enterprise with absolute ease.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\ExpenseReport.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\ExpenseReport.vb"
					}
				),
				new SimpleModule(demo,
					name: "EmployeeInformation",
					displayName: @"Employee Information",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.EmployeeInformationModule",
					description: @"In this demo, we use the XtraSpreadsheet’s API to create an employee paystubs template in code at runtime. Use the Employee Information or the Payroll Calculator sheet to modify values and view results of your changes in the Individual Paystubs sheet.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\EmployeeInformation.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\EmployeeInformation.vb"
					}
				),
				new SimpleModule(demo,
					name: "HomeAccounting",
					displayName: @"Personal Finance",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.HomeAccountingModule",
					description: @"This demo shows an application that uses a spreadsheet for tracking income and expenses. The workbook is created in code at runtime. It is initially read-only and contains income and expenses columns broken in categories and organized in tabs by month. To enter a new record, click one of the top left buttons to invoke a dialog to collect required information. After you click OK, the data are automatically stored in a proper column within a proper tab. You can review the records as well as the summary page and modify them if you uncheck the read-only checkbox.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\HomeAccounting.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\HomeAccounting.vb"
					}
				),
				new SimpleModule(demo,
					name: "ShiftSchedule",
					displayName: @"Shift Schedule",
					group: "Overview",
					type: "DevExpress.XtraSpreadsheet.Demos.ShiftScheduleModule",
					description: @"This demo illustrates how to use the Spreadsheet API to create a Shift Schedule spreadsheet template. The TOTAL column contains array formulas for calculating the total number of work hours per shift by employees.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\ShiftSchedule.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\ShiftSchedule.vb"
					}
				),
				new SimpleModule(demo,
					name: "Outline",
					displayName: @"Outline",
					group: "Data Analysis",
					type: "DevExpress.XtraSpreadsheet.Demos.OutlineGroupingSubtotal",
					description: @"The Spreadsheet Control allows you to group/summarize data and to create an outline to efficiently display summary rows, columns, and detail data for each group. This demo illustrates how to split worksheet data across separate groups and display summary rows/columns for each group.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\OutlineGroupingSubtotal.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\OutlineGroupingSubtotal.vb"
					}
				),
				new SimpleModule(demo,
					name: "AnalysisPivotTable",
					displayName: @"Pivot Table",
					group: "Data Analysis",
					type: "DevExpress.XtraSpreadsheet.Demos.PivotTableModule",
					description: @"This demo illustrates how to use the powerful Pivot Table functionality of the XtraSpreadsheet to categorize and subtotal data in a worksheet. In particular, this PivotTable report helps summarize sales data located in the Data worksheet. To rearrange PivotTable fields and create a new report layout, drag and drop required fields between the four areas of the Field List on the right. Use various options on the PivotTable Tools ribbon tab to modify the pivot table (adjust the report layout, change formatting, specify field settings, etc.).",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\PivotTableModule.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\PivotTableModule.vb"
					},
					newUpdatedPriority: 1
				),
				new SimpleModule(demo,
					name: "AutoFilter",
					displayName: @"Sort and Filter",
					group: "Data Analysis",
					type: "DevExpress.XtraSpreadsheet.Demos.AutoFilterModule",
					description: @"This demo illustrates the XtraSpreadsheet's filtering capabilities. To apply a filter, click the drop-down arrow in a column header and select filter type: Text Filter, Number Filter or Filter by Values. Once the filter is applied, only rows that match your criteria will be displayed.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\AutoFilter.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\AutoFilter.vb"
					}
				),
				new SimpleModule(demo,
					name: "BreakevenAnalysis",
					displayName: @"Charting",
					group: "Data Visualization",
					type: "DevExpress.XtraSpreadsheet.Demos.BreakevenAnalysisModule",
					description: @"The DevExpress WinForms Spreadsheet allows you to replicate many of the capabilities in Microsoft Excel, including the ability to embed charts within worksheets. The chart types and layouts available within our spreadsheet are compatible with those of Excel. In this demo, we use both a line chart and a pie chart to visualize breakeven points.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\BreakevenAnalysis.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\BreakevenAnalysis.vb"
					}
				),
				new SimpleModule(demo,
					name: "ConditionalFormatting",
					displayName: @"Conditional Formatting",
					group: "Data Visualization",
					type: "DevExpress.XtraSpreadsheet.Demos.TopTradingPartnersModule",
					description: @"The XtraSpreadsheet allows you to apply conditional formatting and change the appearance of individual cells based on specific conditions. This powerful option helps highlight critical information or describe trends within cells by using data bars, color scales or built-in icon sets. To apply or remove a conditional format for a column, select or clear the appropriate checkbox at the top of the worksheet. Conditional formatting rules and their appearance are compatible with Microsoft Excel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\ConditionalFormatting.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\ConditionalFormatting.vb"
					}
				),
				new SimpleModule(demo,
					name: "Comments",
					displayName: @"Comments",
					group: "Document Features",
					type: "DevExpress.XtraSpreadsheet.Demos.CommentsModule",
					description: @"This demo illustrates use of annotations within a worksheet. The XtraSpreadsheet displays comments in a floating box anchored to a cell. You can add new comments, edit existing comments, move and resize the comment box, hide or delete comments if they are no longer needed.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\Comments.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\Comments.vb"
					}
				),
				new SimpleModule(demo,
					name: "DataValidation",
					displayName: @"Data Validation",
					group: "Document Features",
					type: "DevExpress.XtraSpreadsheet.Demos.DataValidationModule",
					description: @"The XtraSpreadsheet allows you to impose restrictions on worksheet cells to prevent end-users from entering wrong values. This demo shows a simple document with data validation applied. Each column containing validated cells is accommodated with a comment explaining what values can be entered into these cells. To view or modify validation settings, click the Data Validation button on the ribbon to invoke the Data Validation dialog.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\DataValidation.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\DataValidation.vb"
					},
					newUpdatedPriority: 0
				),
				new SimpleModule(demo,
					name: "DocumentProperties",
					displayName: @"Document Properties",
					group: "Document Features",
					type: "DevExpress.XtraSpreadsheet.Demos.DocumentPropertiesModule",
					description: @"This demo allows you to view and modify document properties. Document properties are the metadata that can be stored with the document. They are named values which are either built into the document, or custom properties, which are user defined. The demo makes use of a custom worksheet function (UDF) to display the values of document properties in worksheet cells. This function is implemented in this demo and named DOCPROP - review the source code for details.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\DocumentProperties.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\DocumentProperties.vb"
					}
				),
				new SimpleModule(demo,
					name: "HeaderFooter",
					displayName: @"Headers and Footers",
					group: "Document Features",
					type: "DevExpress.XtraSpreadsheet.Demos.HeaderFooterModule",
					description: @"This demo shows the printout of a sales report with a header and footer inserted at the top and bottom of the page. To change the applied header or footer, invoke the Page Setup dialog by clicking the Headers/Footers button at the top of the Print Preview window.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\HeaderFooter.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\HeaderFooter.vb"
					},
					newUpdatedPriority: 1
				),
				new SimpleModule(demo,
					name: "Protection",
					displayName: @"Worksheet Protection",
					group: "Document Features",
					type: "DevExpress.XtraSpreadsheet.Demos.ProtectionModule",
					description: @"This demo shows a simple monthly budget with worksheet protection applied. You can change income and expenses amount values. All other content is protected and cannot be modified. Password to unprotect a sheet is '123'.""",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\Protection.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\Protection.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMergeTableReport",
					displayName: @"Merge Database Records",
					group: "Mail Merge",
					type: "DevExpress.XtraSpreadsheet.Demos.MailMergeTableReportModule",
					description: @"The XtraSpreadsheet was built so you can address a wide range of use-case scenarios, including the ability to use mail merge to generate personalized letters and a variety of business-centric reports. In this demo, we use the spreadsheet to generate a detailed report for customer orders. A template with mail merge fields is bound to a database and opened in the Spreadsheet Control. Use the Parameters panel to specify the Order ID. To preview a resulting document, click the Mail Merge Preview button.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\MailMergeTableReport.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\MailMergeTableReport.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMergePictures",
					displayName: @"Merge Images",
					group: "Mail Merge",
					type: "DevExpress.XtraSpreadsheet.Demos.MailMergePicturesModule",
					description: @"Whether you wish use the DevExpress WinForms Spreadsheet to generate personalized letters, company directories or interactive business reports, the data merge capabilities built into the product offer you countless runtime options. In this demo, we illustrate the merging of images along with personal information for contacts from the Employees worksheet using Object Binding. Switch to the Employees sheet and edit the employees' information if required. The resulting document will be updated to display your changes. To preview results, click the Mail Merge Preview button.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\MailMergePictures.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\MailMergePictures.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMergeMasterDetail",
					displayName: @"Master Detail Reports",
					group: "Mail Merge",
					type: "DevExpress.XtraSpreadsheet.Demos.MailMergeMasterDetailModule",
					description: @"The XtraSpreadsheet’s mail merge functionality is comprehensive and will allow you to generate master-detail reports of any complexity. In this demo, we’ve created a template (opened in the SpreadsheetControl) with three data levels: one for a supplier, product and associated orders grouped by a unit price. Click the Mail Merge Preview button to view the merged document, wherein a separate worksheet is created for each supplier automatically.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\MailMergeMasterDetail.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\MailMergeMasterDetail.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMergeFallCatalog",
					displayName: @"Fall Catalog",
					group: "Mail Merge",
					type: "DevExpress.XtraSpreadsheet.Demos.MailMergeFallCatalogModule",
					description: @"This demo illustrates how to use the SpreadsheetControl’s mail merge functionality to generate a real-life catalog with data retrieved from a database.
In this document, data is sorted and grouped by the CategoryName data field. To review the specified sort criteria, select any cell within the template detail range and click Sort Fields on the Mail Merge tab. The group header range in the template specifies data to be displayed at the beginning of a group of records in a resulting document. To preview a resulting document, click the Mail Merge Preview button.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\MailMergeFallCatalog.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\MailMergeFallCatalog.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMergeInvoice",
					displayName: @"Mail Merge Invoice",
					group: "Mail Merge",
					type: "DevExpress.XtraSpreadsheet.Demos.MailMergeInvoiceModule",
					description: @"This demo shows the Invoice document template built using data from the Northwind database by the SpreadsheetControl’s mail merge tools. The demo introduces template ranges, data sorting and data grouping.
Click the Mail Merge Preview button to view the merged document, where a separate worksheet is created for each invoice automatically.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\MailMergeInvoice.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\MailMergeInvoice.vb"
					}
				),
				new SimpleModule(demo,
					name: "SpreadsheetAPI",
					displayName: @"Spreadsheet API",
					group: "API",
					type: "DevExpress.XtraSpreadsheet.Demos.SpreadsheetAPIModule",
					description: @"This demo introduces objects and methods available via Spreadsheet API. The TreeView on the right displays action names. By clicking the action you are presented with a code of the method with the same name and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\SpreadsheetAPIModule.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\SpreadsheetAPIModule.vb"
					}
				),
				new SimpleModule(demo,
					name: "ChartAPI",
					displayName: @"Chart API",
					group: "API",
					type: "DevExpress.XtraSpreadsheet.Demos.ChartAPIModule",
					description: @"This demo introduces objects and methods available via Chart API of SpreadsheetControl. The TreeView on the right displays action names. By clicking the action you are presented with a code of the method with the same name and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\ChartAPIModule.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\ChartAPIModule.vb"
					}
				),
				new SimpleModule(demo,
					name: "PivotTableAPI",
					displayName: @"Pivot Table API",
					group: "API",
					type: "DevExpress.XtraSpreadsheet.Demos.PivotTableAPIModule",
					description: @"This demo introduces objects and methods available via Spreadsheet Pivot Table API. The TreeView on the right displays action names. By clicking the action you are presented with a code of the method with the same name and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\PivotTableAPIModule.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\PivotTableAPIModule.vb"
					},
					newUpdatedPriority: 2
				),
				new SimpleModule(demo,
					name: "OperationRestrictions",
					displayName: @"Operation Restrictions",
					group: "API",
					type: "DevExpress.XtraSpreadsheet.Demos.OperationRestrictionsModule",
					description: @"This demo illustrates how you can restrict end-users from certain worksheet operations. You can also hide disabled command bars associated with restricted commands. To reinforce the protective effect, you can prevent the popup menu from being displayed.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\OperationRestrictions.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\OperationRestrictions.vb"
					}
				),
				new SimpleModule(demo,
					name: "CellPropertyViewer",
					displayName: @"Cell Properties Viewer",
					group: "API",
					type: "DevExpress.XtraSpreadsheet.Demos.CellPropertiesViewerModule",
					description: @"This demo illustrates the XtraSpreadsheet capabilities to edit a cell selected in a worksheet through API. Select a cell in the SpreadsheetControl and review its properties in the property grid. Enter a value or formula into a cell, or format this cell and look how values of corresponding properties are changed. You can also modify the selected cell by setting its formula, formatting or layout properties in corresponding property grid editors.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\CellPropertyViewer.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\CellPropertyViewer.vb"
					}
				),
				new SimpleModule(demo,
					name: "SpreadsheetEventViewer",
					displayName: @"Event Viewer",
					group: "API",
					type: "DevExpress.XtraSpreadsheet.Demos.SpreadsheetEventViewerModule",
					description: @"This demo allows you to examine events that fire when performing different actions in SpreadsheetControl. For example, enter data into any cell or create a new worksheet, and review event logs. To filter events that should be logged, select event names in the checked list box on the right.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\SpreadsheetEventViewer.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\SpreadsheetEventViewer.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomInplaceEditors",
					displayName: @"Custom Cell Editors",
					group: "Customization",
					type: "DevExpress.XtraSpreadsheet.Demos.CustomInplaceEditors",
					description: @"This demo illustrates how to use the XtraSpreadsheet's API to display custom controls instead of the default cell editor. In this example, we use the DevExpress DateEdit, LookUpEdit, and CheckEdit controls to facilitate and validate user input. Give it a try. Edit a cell value in the Order Date, Category, or Discount column. In the custom control that appears, select a required value and press ENTER to commit changes. As a result, the selected value will be displayed in a cell.",
					addedIn: KnownDXVersion.V151,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\CustomInplaceEditors.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\CustomInplaceEditors.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomDraw",
					displayName: @"Custom Draw",
					group: "Customization",
					type: "DevExpress.XtraSpreadsheet.Demos.CustomDrawModule",
					description: @"This demo illustrates how to use a custom draw functionality to display callouts and highlight cell content. Enter values to calculate an area of a shape using selected formula. If a value or a combination of values is incorrect, a cell is highlighted and a callout containing text message is displayed above the cell to draw user attention.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\CustomDraw.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\CustomDraw.vb"
					}
				),
				new SimpleModule(demo,
					name: "HighlightText",
					displayName: @"Highlight Text",
					group: "Customization",
					type: "DevExpress.XtraSpreadsheet.Demos.HighlightTextModule",
					description: @"This demo allows you to find and highlight text in visible cells. Type in text to search for in the Highlight Text box. All occurrences of text within visible cells will be highlighted.The demo uses search and custom draw functionality of the Spreadsheet.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\HighlightText.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\HighlightText.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomFunction",
					displayName: @"Custom Function",
					group: "Customization",
					type: "DevExpress.XtraSpreadsheet.Demos.CustomFunctionModule",
					description: @"This demo illustrates how to implement a custom function for use in a spreadsheet. Enter a positive integer number into the cell at the top and review how this number is automatically written in words in different cultures. The SPELLNUMBER function returns a cardinal or ordinal number in words. To create a custom function, implement the DevExpress.Spreadsheet.Functions.CustomFunction interface and add a function to the DevExpress.Spreadsheet.Functions.CustomFunctionCollection.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\CustomFunction.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\CustomFunction.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomTooltip",
					displayName: @"Custom Tooltip",
					group: "Customization",
					type: "DevExpress.XtraSpreadsheet.Demos.CustomTooltipModule",
					description: @"This demo illustrates how to use the SpreadsheetControl’s GetCellFromPoint method to get a cell over which the mouse hovers and display a custom tooltip for this cell. The ToolTipController component is used to customize the tooltip appearance and behavior options.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\CustomTooltip.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\CustomTooltip.vb"
					}
				),
				new SimpleModule(demo,
					name: "DocumentThemes",
					displayName: @"Document Themes",
					group: "Miscellaneous",
					type: "DevExpress.XtraSpreadsheet.Demos.DocumentThemesModule",
					description: @"This demo illustrates how to use Spreadsheet styles to implement document themes. Select a theme from the list to apply it to the document.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\DocumentThemes.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\DocumentThemes.vb"
					}
				),
				new SimpleModule(demo,
					name: "Hyperlinks",
					displayName: @"Hyperlinks",
					group: "Miscellaneous",
					type: "DevExpress.XtraSpreadsheet.Demos.HyperlinksModule",
					description: @"This module demonstrates local and external hyperlinks in a workbook. Browse through the first sheet and click an underlined album title (for example, “Animals”) to navigate to the sheet with detailed information on this album. Click “Back to Top Albums” to return to the first sheet. At the bottom of the first sheet, there is an external hyperlink that refers to the existing Web page.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\Hyperlinks.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\Hyperlinks.vb"
					}
				),
				new SimpleModule(demo,
					name: "UnitConverter",
					displayName: @"Unit Converter",
					group: "Miscellaneous",
					type: "DevExpress.XtraSpreadsheet.Demos.UnitConverterModule",
					description: @"This demo illustrates how to use the built-in CONVERT function to create a simple unit converter. Select a sheet containing the required unit of measure and enter a value in the cell for that unit to automatically convert it to other units.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\UnitConverter.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\UnitConverter.vb"
					}
				),
				new SimpleModule(demo,
					name: "Minesweeper",
					displayName: @"Minesweeper",
					group: "Miscellaneous",
					type: "DevExpress.XtraSpreadsheet.Demos.MinesweeperModule",
					description: @"This demo allows you to play Minesweeper game in a worksheet. This example takes advantage of the SpreadsheetControl hot tracking feature available via the GetCellFromPoint method, handles mouse events of the SpreadsheetControl and uses its API.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\Minesweeper.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\Minesweeper.vb"
					}
				),
				new SimpleModule(demo,
					name: "SportResults",
					displayName: @"Sport Results",
					group: "Miscellaneous",
					type: "DevExpress.XtraSpreadsheet.Demos.SportResultsModule",
					description: @"This demo illustrates how to use built-in functions (INDEX, LARGE, SMALL, ROW, IF, COUNTIF, LOOKUP), array formulas and 3D references to implement a sample application for processing sport results. Select the “Results” sheet and edit drivers’ positions in the “Race N” columns of the “RESULTS” table.  Total points scored by drivers and their Top 10 rank are calculated automatically.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\SportResults.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\SportResults.vb"
					}
				),
				new SimpleModule(demo,
					name: "WeatherInCalifornia",
					displayName: @"Weather in California",
					group: "Miscellaneous",
					type: "DevExpress.XtraSpreadsheet.Demos.WeatherInCaliforniaModule",
					description: @"This demo helps illustrate the power of conditional formatting in a fun way. We display weather information for the State of California and use the XtraSpreadsheet’s conditional formatting features to highlight important information using data bars and color scales. All conditional formatting rules and associated appearances are compatible with Microsoft Excel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\SpreadsheetMainDemo\Modules\WeatherInCalifornia.cs",
						@"\WinForms\VB\SpreadsheetMainDemo\Modules\WeatherInCalifornia.vb"
					}
				)
			};
		}
	}
}
