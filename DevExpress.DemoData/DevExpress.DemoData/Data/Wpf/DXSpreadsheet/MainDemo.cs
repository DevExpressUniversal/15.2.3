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
		static List<Module> Create_DXSpreadsheet_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "FirstLook",
					displayName: @"First Look",
					group: "Overview",
					type: "SpreadsheetDemo.FirstLook",
					shortDescription: @"Demonstrates the appearance and basic functionality of the Spreadsheet control.",
					description: @"
                        <Paragraph>
                        This demo showcases the basic functionality provided by the WPF Spreadsheet control. Use the Options sidebar to programmatically switch worksheets, to modify document properties and to select an arbitrary range of cells.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Invoice",
					displayName: @"Invoice",
					group: "Overview",
					type: "SpreadsheetDemo.Invoice",
					shortDescription: @"Explore the simple Invoice workbook template created in code at runtime.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the Spreadsheet API to create a customizable simple invoice template in code at runtime.
                        </Paragraph>
                        <Paragraph>
                        This demo helps illustrate the power of the Spreadsheet API. The sample invoice template is generate in code at runtime. Switch to the Data tab to modify the contents of an order, then return to the Invoice tab to view the results. As you can imagine, the invoice is a live document so you can change product quantity, price and discount values. All invoice values are then re-calculated automatically.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ExpenseReport",
					displayName: @"Expense Report",
					group: "Overview",
					type: "SpreadsheetDemo.ExpenseReport",
					shortDescription: @"Explore the Expense Report worksheet created in code at runtime.",
					description: @"
                        <Paragraph>
                        In this demo, we illustrate the power of the Spreadsheet API and how you can generate an expense report in code at runtime. And remember, with built-in printing and export support, you can distribute spreadsheet documents such as this throughout your enterprise with absolute ease.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "EmployeeInformation",
					displayName: @"Employee Information",
					group: "Overview",
					type: "SpreadsheetDemo.EmployeeInformation",
					shortDescription: @"Explore the Employee Information workbook template created in code at runtime.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the Spreadsheet API to create the Individual Paystubs spreadsheet template in code at runtime.
                        </Paragraph>
                        <Paragraph>
                        In this demo, we use the Spreadsheet API to create an employee paystubs template in code at runtime. Use the Employee Information or the Payroll Calculator sheet to modify values and view results of your changes in the Individual Paystubs sheet.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PersonalFinance",
					displayName: @"Personal Finance",
					group: "Overview",
					type: "SpreadsheetDemo.PersonalFinance",
					shortDescription: @"Use this spreadsheet to track income and expenses throughout the year.",
					description: @"
                        <Paragraph>
                        This demo shows an application that uses a spreadsheet for tracking income and expenses. The workbook is created in code at runtime.
                        </Paragraph>
                        <Paragraph>
                        The workbook is initially read-only and contains income and expenses columns broken in categories and organized in tabs by month. To enter a new record, click one of the top left buttons to invoke a dialog to collect required information. After you click OK, the data are automatically stored in a proper column within a proper tab. You can review the records as well as the summary page and modify them if you uncheck the read-only checkbox.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ShiftSchedule",
					displayName: @"Shift Schedule",
					group: "Overview",
					type: "SpreadsheetDemo.ShiftSchedule",
					shortDescription: @"Explore the Shift Schedule worksheet created in code at runtime.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the Spreadsheet API to create a Shift Schedule spreadsheet template in code at run time. The TOTAL column contains array formulas for calculating the total number of work hours per shift by employees.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SportResults",
					displayName: @"Sport Results",
					group: "Overview",
					type: "SpreadsheetDemo.SportResults",
					shortDescription: @"Explore the workbook that uses built-in worksheet functions to process sport results. The Sport Results workbook is created in code at run time.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use built-in functions (INDEX, LARGE, SMALL, ROW, IF, COUNTIF, LOOKUP), array formulas and 3D references to implement a sample application for processing sport results.
                        </Paragraph>
                        <Paragraph>
                        Select the “Results” sheet and edit positions in the “Race N” columns of the “RESULTS” table. Total points scored by drivers and their Top 10 rank are calculated automatically.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "StatisticFunctions",
					displayName: @"Trend Analysis",
					group: "Overview",
					type: "SpreadsheetDemo.StatisticFunctions",
					shortDescription: @"Perform the trend analysis by using statistical functions.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the spreadsheet's statistical functions to perform trend analysis.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "Grouping",
					displayName: @"Outline",
					group: "Data Analysis",
					type: "SpreadsheetDemo.Grouping",
					shortDescription: @"This demo illustrates how to group and summarize data in a worksheet.",
					description: @"
                        <Paragraph>
                        The Spreadsheet Control allows you to group/summarize data and to create an outline to efficiently display summary rows, columns, and detail data for each group.
                        </Paragraph>
                        <Paragraph>
                        This demo illustrates how to split worksheet data across separate groups and display summary rows/columns for each group.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "PivotTable",
					displayName: @"Pivot Table",
					group: "Data Analysis",
					type: "SpreadsheetDemo.PivotTable",
					shortDescription: @"Use the pivot table to summarize worksheet data.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the powerful Pivot Table functionality of the WPF Spreadsheet to categorize and subtotal data in a worksheet. In particular, this PivotTable report helps summarize sales data located in the Data worksheet.
                        </Paragraph>
                        <Paragraph>
                        To rearrange PivotTable fields and create a new report layout, drag and drop required fields between the four areas of the Field List. Use various options on the PivotTable Tools ribbon tab to modify the pivot table (adjust the report layout, change formatting, specify field settings, etc.).
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "AutoFilter",
					displayName: @"Sort and Filter",
					group: "Data Analysis",
					type: "SpreadsheetDemo.AutoFilter",
					shortDescription: @"Explore the filtering functionality of the Spreadsheet control.",
					description: @"
                        <Paragraph>
                        This demo illustrates the Spreadsheet Control's filtering capabilities. To apply a filter, click the drop-down arrow in a column header and select filter type: Text Filter, Number Filter or Filter by Values.
                        </Paragraph>
                        <Paragraph>
                        Once the filter is applied, only rows that match your criteria will be displayed.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "ChartingModule",
					displayName: @"Charting",
					group: "Data Visualization",
					type: "SpreadsheetDemo.ChartingModule",
					shortDescription: @"Explore the charting functionality of the Spreadsheet control.",
					description: @"
                        <Paragraph>
                        The DevExpress WPF Spreadsheet allows you to replicate many of the capabilities in Microsoft Excel, including the ability to embed charts within worksheets. The chart types and layouts available within our spreadsheet are compatible with those of Excel. In this demo, we use both a line chart and a pie chart to visualize breakeven points.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ConditionalFormatting",
					displayName: @"Conditional Formatting",
					group: "Data Visualization",
					type: "SpreadsheetDemo.ConditionalFormatting",
					shortDescription: @"Use conditional formatting to explore financial data.",
					description: @"
                        <Paragraph>
                        The WPF Spreadsheet allows you to apply conditional formatting and change the appearance of individual cells based on specific conditions. This powerful option helps highlight critical information or describe trends within cells by using data bars, color scales or built-in icon sets. To apply or remove a conditional format for a column, select or clear the appropriate checkbox at the top of the worksheet. Conditional formatting rules and their appearance are compatible with Microsoft Excel.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Comments",
					displayName: @"Comments",
					group: "Document Features",
					type: "SpreadsheetDemo.Comments",
					shortDescription: @"Use comments to attach notes to worksheet cells.",
					description: @"
                        <Paragraph>
                        This demo illustrates use of annotations within a worksheet. The WPF Spreadsheet displays comments in a floating box anchored to a cell. You can add new comments, edit existing comments, move and resize the comment box, hide or delete comments if they are no longer needed.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "DataValidation",
					displayName: @"Data Validation",
					group: "Document Features",
					type: "SpreadsheetDemo.DataValidation",
					shortDescription: @"Use data validation to restrict user input.",
					description: @"
                        <Paragraph>
                        The WPF Spreadsheet allows you to impose restrictions on worksheet cells to prevent end-users from entering wrong values. This demo shows a simple document with data validation applied. Each column containing validated cells is accommodated with a comment explaining what values can be entered into these cells. To view or modify validation settings, click the Data Validation button on the ribbon to invoke the Data Validation dialog.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "DocumentProperties",
					displayName: @"Document Properties",
					group: "Document Features",
					type: "SpreadsheetDemo.DocumentProperties",
					shortDescription: @"View and edit document properties in the Spreadsheet control.",
					description: @"
                        <Paragraph>
                        This demo allows you to view and modify document properties. Document properties are the metadata that can be stored with the document. They are named values which are either built into the document, or custom properties, which are user defined. The demo makes use of a custom worksheet function (UDF) to display the values of document properties in worksheet cells. This function is implemented in this demo and named DOCPROP - review the source code for details.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "HeaderFooter",
					displayName: @"Headers and Footers",
					group: "Document Features",
					type: "SpreadsheetDemo.HeaderFooter",
					shortDescription: @"Add headers and footers to the worksheet printout.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to insert a header and footer at the top and bottom of a printed document. To view the worksheet printout, click the Print Preview button at the top of the window. To change the specified header or footer, invoke the Page Setup dialog by clicking the Headers/Footers button.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "WorksheetProtection",
					displayName: @"Worksheet Protection",
					group: "Document Features",
					type: "SpreadsheetDemo.WorksheetProtection",
					shortDescription: @"Demonstrates worksheet protection.",
					description: @"
                        <Paragraph>
                        This demo shows a simple monthly budget with worksheet protection applied. You can change income and expenses amount values. All other content is protected and cannot be modified.
                        </Paragraph>
                        <Paragraph>
                        Password to unprotect a sheet is '123'.""
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SpreadsheetAPIModule",
					displayName: @"Spreadsheet API",
					group: "API",
					type: "SpreadsheetDemo.SpreadsheetAPIModule",
					shortDescription: @"This demo introduces objects and methods available via Spreadsheet API.",
					description: @"
                        <Paragraph>
                        This demo introduces objects and methods available via Spreadsheet API. The TreeView on the right displays action names. By clicking the action you are presented with a code of the method with the same name and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "ChartAPI",
					displayName: @"Chart API",
					group: "API",
					type: "SpreadsheetDemo.ChartAPI",
					shortDescription: @"This demo introduces objects and methods available via Chart API of Spreadsheet control.",
					description: @"
                        <Paragraph>
                        This demo introduces objects and methods available via Chart API. The TreeView on the right displays action names. By clicking the action you are presented with a code of the method with the same name and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PivotAPI",
					displayName: @"Pivot Table API",
					group: "API",
					type: "SpreadsheetDemo.PivotAPI",
					shortDescription: @"This demo introduces objects and methods available via Pivot Table API of Spreadsheet control.",
					description: @"
                        <Paragraph>
                        This demo introduces objects and methods available via Pivot Table API. The TreeView on the right displays action names. By clicking the action you are presented with a code of the method with the same name and a preview of what the resulting spreadsheet looks like. The parent node indicates the code file containing this method so you can easily locate it within the demo module.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "CustomFunction",
					displayName: @"Custom Function",
					group: "API",
					type: "SpreadsheetDemo.CustomFunction",
					shortDescription: @"Enter a number and it will be automatically written in words in different cultures.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to implement a custom function for use in a spreadsheet. Enter a positive integer number into the cell at the top and review how this number is automatically written in words in different cultures. The SPELLNUMBER function returns a cardinal or ordinal number in words. To create a custom function, implement the DevExpress.Spreadsheet.Functions.CustomFunction interface and add a function to the DevExpress.Spreadsheet.Functions.CustomFunctionCollection.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "OperationRestrictions",
					displayName: @"Operation Restrictions",
					group: "API",
					type: "SpreadsheetDemo.OperationRestrictions",
					shortDescription: @"This demo illustrates how you can restrict end-users from certain worksheet operations.",
					description: @"
                        <Paragraph>
                        This demo illustrates how you can restrict end-users from certain worksheet operations. You can also hide disabled command bars associated with restricted commands. To reinforce the protective effect, you can prevent the popup menu from being displayed.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "CellCustomization",
					displayName: @"Cell Customization",
					group: "Customization",
					type: "SpreadsheetDemo.CellCustomization",
					shortDescription: @"Use cell templates to paint cell background and display warnings.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use WPF templates to highlight cell content and display warnings.
                        </Paragraph>
                        <Paragraph>
                        Enter values to calculate an area of a shape using selected formula. If a value or a combination of values is incorrect, a cell is highlighted and a text note is displayed above the cell to draw user attention.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ContextMenuCustomization",
					displayName: @"Context Menu Customization",
					group: "Customization",
					type: "SpreadsheetDemo.ContextMenuCustomization",
					shortDescription: @"Include custom items in the spreadsheet context menu.",
					description: @"
                        <Paragraph>
                        Spreadsheet allows you to customize its context menus by disabling or removing existing items, or adding your own new items. Select or clear check boxes to add or remove appropriate items to/from the cell context menu.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DocumentThemes",
					displayName: @"Document Themes",
					group: "Miscellaneous",
					type: "SpreadsheetDemo.DocumentThemes",
					shortDescription: @"Select a theme and apply it to the workbook.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use Spreadsheet styles to implement document themes. Select a theme from the list to apply it to the document.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Hyperlinks",
					displayName: @"Hyperlinks",
					group: "Miscellaneous",
					type: "SpreadsheetDemo.Hyperlinks",
					shortDescription: @"Navigate using local and external hyperlinks.",
					description: @"
                        <Paragraph>
                        This module demonstrates local and external hyperlinks in a workbook.
                        </Paragraph>
                        <Paragraph>
                        Browse through the first sheet and click an underlined album title (for example, “Animals”) to navigate to the sheet with detailed information on this album. Click “Back to Top Albums” to return to the first sheet. At the bottom of the first sheet, there is an external hyperlink that refers to the existing Web page.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "UnitConverter",
					displayName: @"Unit Converter",
					group: "Miscellaneous",
					type: "SpreadsheetDemo.UnitConverter",
					shortDescription: @"Try the unit converter that uses the spreadsheet CONVERT function.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the built-in CONVERT function to create a simple unit converter. Select a sheet containing the required unit of measure and enter a value in the cell for that unit to automatically convert it to other units.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				 new WpfModule(demo,
					name: "WeatherInCalifornia",
					displayName: @"Weather in California",
					group: "Miscellaneous",
					type: "SpreadsheetDemo.WeatherInCalifornia",
					shortDescription: @"Use conditional formatting to explore weather data.",
					description: @"
                        <Paragraph>
                        The Spreadsheet allows you to apply a conditional format to change the appearance of cells based on certain conditions. This powerful option helps to highlight interesting data or visualize cell values by using data bars, color scales or built-in icon sets. To apply or remove a conditional format for the desired column, select or clear the appropriate checkbox at the top of the worksheet. Conditional formatting rules and appearance are compatible with Microsoft Excel.
                        </Paragraph>",
					allowTouchThemes: false,
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				)
		   };
		}
	}
}
