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
		static List<Module> Create_DXPivotGrid_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "CustomerReports",
					displayName: @"Customer Reports",
					group: "Sample Reports",
					type: "PivotGridDemo.PivotGrid.CustomerReports",
					shortDescription: @"Includes a set of sample reports that demonstrate how to analyze the same business data in different forms.",
					description: @"
                        <Paragraph>
                        This demo includes a set of reports which allows you analyze the same business data in different forms. Use the Radio buttons to switch between the following reports:
                        </Paragraph>
                        <Paragraph>
                        <Span>• Customers:</Span> summarizes the orders made by customers in a specific time period. The quantities ordered are given for each quarter and for each product which was bought by a customer;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Products(filtering):</Span> displays the products bought by customers in a particular year. Use the Year and Quarter combo boxes to select which period is displayed and analyze the information in the control;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Top 2 Products:</Span> lists the two most popular products for each customer (the ones which generated the most interest). Clicking the 'Product Name' field header reverses the current sort order and the control will show you the least popular products for each customer;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Top 10 Customers:</Span> lists the top 10 customers, who purchased the most items. Clicking the 'Customer' field header reverses the sort order and displays the customers who purchased the least amount of items.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "OrderReports",
					displayName: @"Order Reports",
					group: "Sample Reports",
					type: "PivotGridDemo.PivotGrid.OrderReports",
					shortDescription: @"Includes a set of sample reports that demonstrate how to analyze the same business data in different forms.",
					description: @"
                        <Paragraph>
                        This demo includes a set of reports which allows you analyze the same business data in different forms. Use the Radio buttons to switch between the following reports:
                        </Paragraph>
                        <Paragraph>
                        <Span>• Orders:</Span> shows all the orders in a database and for each the details are listed as sub-categories. The DXPivotGrid control calculates multiple summaries so you can see the Average Unit Price, Total Quantity, Average Discount and Total Price for each order;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Orders (filtering):</Span> resembles the previous one. However, it allows you to view only one order at a time. Selecting a value from the 'Options' combobox applies a filter to the DXPivotGrid and thus displays the selected order's details;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Quantity:</Span> displays how many units of each product were ordered by customers;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Average Unit Price:</Span> calculates the average unit price for each product. To see how unit prices differ between different orders drag the 'OrderID' field's header to the right of the 'Product Name' field's header.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ProductReports",
					displayName: @"Product Reports",
					group: "Sample Reports",
					type: "PivotGridDemo.PivotGrid.ProductReports",
					shortDescription: @"Includes a set of sample reports that demonstrate how to analyze the same business data in different forms.",
					description: @"
                        <Paragraph>
                        This demo includes a set of reports which allows you analyze the same business data in different forms. Use the Radio buttons to switch between the following reports:
                        </Paragraph>
                        <Paragraph>
                        <Span>• Category Sales:</Span> displays the total amount of sales for each category of product;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Product Sales:</Span> show the total amount of sales for each product. Check the Show Categories check box to view products by categories. For each category a total will be automatically calculated;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Interval Grouping:</Span> show the amount of sales for each category and product according to the shipping date. You can categorize the information by year, quarter and(or) month;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Multiple Subtotals:</Span> calculate multiple summaries (Sum, Average, Max and Min) for each product category. It breaks the information down into years and quarterly intervals so that you can analyze the information according to the shipping date;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Average Sales:</Span> calculates the total, average and minimum sales amount for each category;
                        </Paragraph>
                        <Paragraph>
                        <Span>• Top 3 Products:</Span> show the three most popular products in each category. Clicking the 'Product Name' field header reverses the current sort order and the report will show you the three least popular products in each category.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "OLAPBrowser",
					displayName: @"OLAP Browser",
					group: "OLAP Features",
					type: "PivotGridDemo.PivotGrid.OLAPBrowser",
					shortDescription: @"Demonstrates pivot grid capabilities of displaying data from OLAP cubes.",
					description: @"
                        <Paragraph>
                        This demo shows a pivot grid bound to an OLAP cube.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the customization form on the right displays measures and dimensions contained in a bound cube. Drag them from the Hidden Fields section to other sections (or the corresponding pivot grid areas) to slice and dice data in different manners.
                        </Paragraph>
                        <Paragraph>
                        You can also view underlying data for each summary value. Double-click any cell to invoke a popup window that lists underlying records that were used to calculate this value.
                        </Paragraph>
                        <Paragraph>
                        Use the 'New connection' button to connect to another OLAP server or cube. This button invokes a New Connection dialog, where you should specify the following parameters:
                        </Paragraph>
                        <Paragraph>
                        • the name of an Analysis Services server or the path to a local cube file;
                        </Paragraph>
                        <Paragraph>
                        • the name of a catalog where the desired cube is located;
                        </Paragraph>
                        <Paragraph>
                        • the name of the cube.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to an OLAP Server",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8015")
					}
				),
				new WpfModule(demo,
					name: "OLAPKPI",
					displayName: @"OLAP KPI",
					group: "OLAP Features",
					type: "PivotGridDemo.PivotGrid.OLAPKPI",
					shortDescription: @"Demonstrates how a PivotGridControl can automatically display KPI information from the bound OLAP cube.",
					description: @"
                        <Paragraph>
                        If an OLAP cube contains KPI (Key Performance Indicator) information, PivotGridControl can automatically recognize it and display it in an appropriate format. This demo displays a sample report for the Internet Revenue KPI from the Adventure Works sample cube.
                        </Paragraph>
                        <Paragraph>
                        All KPI status and trend values have associated graphics that can be specified using the PivotGridField.KPIGraphic property. In this demo, you can choose a required image via the 'Status Graphics' and 'Trend Graphics' comboboxes or using the field header context menu.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SortByColumn",
					displayName: @"Sort By Column",
					group: "Summary",
					type: "PivotGridDemo.PivotGrid.SortByColumn",
					shortDescription: @"Demonstrates how field values can be sorted by summary values from another column or row.",
					description: @"
                        <Paragraph>
                        This example demonstrates how to sort fields by column summaries. To do this, right-click on the column's last level cell and select the required field. To sort fields in code, use the SortByConditions property.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Sorting by Summary",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8072"),
						new WpfModuleLink(
							title: "Sort by Summary",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Sort+by+Summary&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "SingleTotal",
					displayName: @"Single Total",
					group: "Summary",
					type: "PivotGridDemo.PivotGrid.SingleTotal",
					shortDescription: @"Shows automatic totals calculated for value groups.",
					description: @"
                        <Paragraph>
                        The DXPivotGrid control automatically calculates grand totals for each row and column. Additionally, totals are automatically calculated for each value group. The type of the automatic totals always matches the type of the summaries calculated in cells. Thus, automatic totals give you a more general data view while you still displaying the details.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can use a number of view options to control the availability of totals.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Automatic Summaries",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8051")
					}
				),
				new WpfModule(demo,
					name: "SortBySummary",
					displayName: @"Sort By Summary",
					group: "Summary",
					type: "PivotGridDemo.PivotGrid.SortBySummary",
					shortDescription: @"Demonstrates how field values can be sorted by summary values calculated against a specific data field.",
					description: @"
                        <Paragraph>
                        This demo illustrates the Sorting By Summary feature which lets you sort the values of a particular column field or row field by the summary values calculated against a specific data field. In this example, the values of the 'Sales Person' field are actually sorted by summary values calculated against another data field. You can select this data field via the combo box at the top of the DXPivotGrid control. Clicking the 'Sales Person' field will reverse the current sort order.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Sorting by Summary",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8072"),
						new WpfModuleLink(
							title: "Sort by Summary",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Sort+by+Summary&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "TotalsLocation",
					displayName: @"Totals Location",
					group: "Summary",
					type: "PivotGridDemo.PivotGrid.TotalsLocation",
					shortDescription: @"Demonstrates how to control the location of total and grand total rows and columns.",
					description: @"
                        <Paragraph>
                        This demo shows you how to control the location of group and grand totals. These totals can be displayed either before or after the corresponding data cells.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "MultipleTotals",
					displayName: @"Multiple Totals",
					group: "Summary",
					type: "PivotGridDemo.PivotGrid.MultipleTotals",
					shortDescription: @"Shows multiple totals calculated for value groups using pre-defined summary types.",
					description: @"
                        <Paragraph>
                        For each field, you can manually specify the number and type of group totals to be displayed. This demo shows how to display the Average, Sum, Maximum and Minimum summaries for each Category group.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Custom Totals",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8059"),
						new WpfModuleLink(
							title: "CustomTotals",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=CustomTotals&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "SummaryDisplayMode",
					displayName: @"Summary Display Mode",
					group: "Summary",
					type: "PivotGridDemo.PivotGrid.SummaryDisplayMode",
					shortDescription: @"Demonstrates how to replace raw summary values with values that reflect correlation between summaries.",
					description: @"
                        <Paragraph>
                        Rather than displaying raw summary values in cells, you can show how these values correlate to values in other cells, and so perform a range of different data analyses. For instance, you can display the differences between summaries in the current and previous cells, or the percentage of a column's or row's total. Use the Summary Display Type combo box to choose the summary display mode, and see how this changes values in every second data column.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Summary Display Modes",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8053")
					}
				),
				new WpfModule(demo,
					name: "CellTemplates",
					displayName: @"Cell Templates",
					group: "Data Visualization",
					type: "PivotGridDemo.PivotGrid.CellTemplates",
					shortDescription: @"Demonstrates how to use templates to customize the appearance of data cells.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use templates to customize the appearance of pivot grid cells. The following options are available:
                        </Paragraph>
                        <Paragraph>
                        <Span>• None.</Span> The default cell template is used.
                        </Paragraph>
                        <Paragraph>
                        <Span>• Share Only.</Span> Each cell displays a progress bar, which indicates the current share value.
                        </Paragraph>
                        <Paragraph>
                        <Span>• Value and Share.</Span> Each cell displays both a progress bar and a share value itself.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you are able to move a mouse pointer above pivot grid cells to illustrate that it will invoke tooltips with custom texts for these cells.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Styles and Templates",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8399"),
						new WpfModuleLink(
							title: "Templates",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Templates&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "ChartsIntegration",
					displayName: @"Charts Integration",
					group: "Data Visualization",
					type: "PivotGridDemo.PivotGrid.ChartsIntegration",
					shortDescription: @"Demonstrates how to export pivot grid data to the Chart control.",
					description: @"
                        <Paragraph>
                        You can easily chart data from the PivotGridControl. When you attach a PivotGridControl to a ChartControl, the latter graphically displays data in the current selection of the PivotGridControl. The ChartControl will use row values as series and column values as arguments if you wish to chart data horizontally, or vice versa if you want to do this vertically. When you want to chart an entire row or column, you don't have to select all its cells. Just select the total of a row or column.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Update Delay of the Bound Chart Control",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2010/11/29/update-delay-of-the-bound-chart-control-coming-in-v2010-vol-2.aspx"),
						new WpfModuleLink(
							title: "Limiting Number of Series and Points in a Chart Data Source",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2010/11/14/limiting-number-of-series-and-points-in-a-chart-data-source-coming-in-2010-vol2.aspx"),
						new WpfModuleLink(
							title: "Integration with the DXCharts Suite",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8016"),
						new WpfModuleLink(
							title: "Charting",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Charting&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "KPIDashboard",
					displayName: @"KPI Dashboard",
					group: "Data Visualization",
					type: "PivotGridDemo.PivotGrid.KPIDashboard",
					shortDescription: @"Includes a sample Key Performance Indicator (KPI) Dashboard with custom-designed Gauge controls.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to create a <Span>Key Performance Indicator</Span> (KPI) with the DXPivotGrid control, and a small custom Gauge control, specially designed for this demo.
                        </Paragraph>
                        <Paragraph>
                        The DXPivotGrid control displays sales information for specific products and dates, and three gauges below it represent:
                        </Paragraph>
                        <Paragraph>
                        • the average value for all sales currently shown within the pivot grid;
                        </Paragraph>
                        <Paragraph>
                        • comparison of sales for two years, if two years are selected;
                        </Paragraph>
                        <Paragraph>
                        • comparison of the sales in the last visible year to the projected sales for this year.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can customize the pivot grid and see how it affects on gauge information. For example, you can select one of the predefined pivot reports on the <Span>Options</Span> pane on the right, or adjust a pivot grid in any other way (e.g. filter the <Span>Year</Span> column).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CustomGroupInterval",
					displayName: @"Custom Group Interval",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.CustomGroupInterval",
					shortDescription: @"Demonstrates how to group field values in a custom manner.",
					description: @"
                        <Paragraph>
                        In this demo we use the Custom Group Intervals feature to group product names and make the report more readable. The Product axis will show three intervals (""A-E"", ""F-S"" and ""T-Z"").
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Grouping Field Values on Axes",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8061"),
						new WpfModuleLink(
							title: "CustomGroupInterval",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=CustomGroupInterval&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "AsynchronousMode",
					displayName: @"Asynchronous Mode",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.AsynchronousMode",
					shortDescription: @"Demonstrates the Pivot Grid capability to process data in a background thread.",
					description: @"
                        <Paragraph>
                        This demo shows the PivotGridControl capabilities to perform data-aware operations (such as retrieving data from a data source, calculating summaries, sorting and filtering) in a background thread. This allows an entire application to stay responsive to end-user actions while Pivot Grid is processing data.
                        </Paragraph>
                        <Paragraph>
                        In this demo, a Pivot Grid control is bound to a large data source. You can choose whether it is an OLAP cube, or a randomly generated table data source. Try to reorder fields, collapse and expand field values, toggle field sorting, and apply filtering to fields. While processing data, the Pivot Grid control will display a wait indicator to show that it is busy. The rest of the application UI will continue responding to your actions.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "AsyncMode for Pivot Grid",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2011/04/19/asyncmode-for-pivot-grid-coming-in-v2011-vol-1.aspx"),
						new WpfModuleLink(
							title: "Asynchronous Mode",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9776")
					}
				),
				new WpfModule(demo,
					name: "ServerMode",
					displayName: @"Server Mode",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.ServerMode",
					shortDescription: @"Demonstrates the Pivot Grid's capability to perform data processing on the database server side.",
					description: @"
                        <Paragraph>
                        The DevExpress Server Mode feature improves performance when binding the PivotGridControl to a large data set. In this mode, only small portions of data are loaded into the PivotGridControl on demand, and all required data processing is performed on the database server side. This technique significantly reduces the application's response time.
                        </Paragraph>
                        <Paragraph>
                        Switch between the LINQ to SQL Server Mode and the Client Mode, and perform the same data operations (for instance, sorting by summary values or layout changing) in the PivotGridControl control, to test the application performance.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "IntervalGrouping",
					displayName: @"Interval Grouping",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.IntervalGrouping",
					shortDescription: @"Demonstrates how to group field values using standard grouping capabilities.",
					description: @"
                        <Paragraph>
                        This example demonstrates the data grouping feature provided by the DXPivotGrid control. The OrderDate field is bound to a data source field which contains date/time values. As you can see, however, the OrderDate field doesn't display actual dates; instead it combines the information by years, months or quarterly intervals depending upon what you select. In addition, the data grouping feature is applied to the Product field and this combines records into a single category if they start with the same letter.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Grouping Field Values on Axes",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8061"),
						new WpfModuleLink(
							title: "Group Field Values",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Group+Values&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "Prefilter",
					displayName: @"Prefilter",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.Prefilter",
					shortDescription: @"Demonstrates how to filter data using complex filter criteria.",
					description: @"
                        <Paragraph>
                        The Prefilter allows end-users to build complex filter criteria with an unlimited number of filter conditions, combined by logical operators. End-users can open the Prefilter by right-clicking on any header or header area and selecting the Show Prefilter menu item. Or, end-user can open it by clicking the pencil icon displayed within the Prefilter panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Prefilter Most Recently Used List",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2010/10/26/prefilter-most-recently-used-list-for-the-dxpivotgrid-coming-in-2010-vol2.aspx"),
						new WpfModuleLink(
							title: "Prefilter",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8062")
					}
				),
				new WpfModule(demo,
					name: "UnboundExpressions",
					displayName: @"Unbound Expressions",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.UnboundExpressions",
					shortDescription: @"Demonstrates how to create fields whose summary values are calculated using custom formulas.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to populate unbound fields with data using formulas (string expressions). Values of the New Year Bonus, Order Count Bonus, Bonus Amount data fields and the Sales Person row field are calculated according to specific expressions.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can use the combo-box on the top options panel to specify expressions for Sales Person field values, or specify custom expressions via the Expression Editor, invoked by clicking the field value. Also, you can add a new bonus and specify its summary expression using the bottom options panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Unbound Fields",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8025"),
						new WpfModuleLink(
							title: "Unbound Data",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=unbound&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "FilterPopup",
					displayName: @"Filter Popup",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.FilterPopup",
					shortDescription: @"Demonstrates end-user capabilities of filtering data against individual fields and field groups using filter popup windows.",
					description: @"
                        <Paragraph>
                        This demo shows end-user capabilities of filtering data against individual fields and field groups using filter popup windows.
                        </Paragraph>
                        <Paragraph>
                        Click the filter button in a column or row field's header to invoke the filter popup. In the popup, check items that should be displayed in the pivot grid, and uncheck those that should be hidden.
                        </Paragraph>
                        <Paragraph>
                        Use the 'Group Filter Mode' combo box to define how data is filtered against field groups. Selecting the 'Tree' value in the combo box enables the Group Filter. In this instance, the filter popup is invoked by clicking the filter button of the first header in the group, and contains values from all group fields arranged into a tree.
                        </Paragraph>
                        <Paragraph>
                        Otherwise, if the 'List' value is selected in the 'Group Filter Mode' combo box, the Group Filter is disabled. In this instance, the filter popup can be invoked for every field in the group, containing values from the current field only.
                        </Paragraph>
                        <Paragraph>
                        Use the 'Enable Filter Popup Menu' check box to specify whether the context menu can be invoked in the filter popup. This menu allows you to invert the filter condition (check items that used to be unchecked, and vice versa).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Pivot Grid supporting Group Filter",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2010/10/23/pivot-grid-supporting-group-filter-coming-in-v2010-vol-2.aspx"),
						new WpfModuleLink(
							title: "Data Filtering",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8010"),
						new WpfModuleLink(
							title: "Group Filter",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8447"),
						new WpfModuleLink(
							title: "Filtering",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Filtering&p=T4|P6|158&d=16"),
						new WpfModuleLink(
							title: "Filter Drop-Down",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Filter+Drop-Down&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "TopValues",
					displayName: @"Top Values",
					group: "Data Processing",
					type: "PivotGridDemo.PivotGrid.TopValues",
					shortDescription: @"Demonstrates how to display only those field values that correspond to the top summary values.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Top X Values feature of the DXPivotGrid control. For any column field or row field you can specify how many values should be displayed and used to calculate summaries. Thus you can only select the most significant values and ignore less important information. In this example, the DXPivotGrid control displays the specified number of values for the selected field. Note that the values in this field are sorted against the values in the 'Order Amount' field.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Display Top N Values",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8063"),
						new WpfModuleLink(
							title: "Top N Values",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Top+N+Values&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "ConditionalFormatting",
					displayName: @"Conditional Formatting",
					group: "Appearance and Layout",
					type: "PivotGridDemo.PivotGrid.ConditionalFormatting",
					shortDescription: @"Use conditional formatting to explore data. Conditional formatting features are available from column context menu.",
					description: @"
                        <Paragraph>
                        This example demonstrates the Format Conditions feature which lets you customize the appearance settings of particular cells depending upon specific conditions. You can customize Style Conditions or create new ones involving a specific field's values.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "FieldsCustomization",
					displayName: @"Fields Customization",
					group: "Appearance and Layout",
					type: "PivotGridDemo.PivotGrid.FieldsCustomization",
					shortDescription: @"Demonstrates how to hide, restore, and move fields using the Customization Form.",
					description: @"
                        <Paragraph>
                        This example shows the Fields Customization Form, which allows an end-user to temporarily hide specific fields and then restore them again when needed. You can switch between the simple and advanced (Office2007) display modes. In the advanced mode, it’s possible to move fields between areas by dragging their headers within the Customization Form.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Filtering and Sorting in the Customization Form",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2010/11/09/filtering-and-sorting-in-the-customization-form-coming-in-v2010-vol-2.aspx"),
						new WpfModuleLink(
							title: "OLAP Fields Tree",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2010/11/08/olap-fields-tree-in-dxpivotgrid-for-wpf-coming-in-v2010-vol-2.aspx"),
						new WpfModuleLink(
							title: "Customization Form",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8018"),
						new WpfModuleLink(
							title: "Customization Form",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Customization+Form&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "CustomAppearance",
					displayName: @"Custom Appearance",
					group: "Appearance and Layout",
					type: "PivotGridDemo.PivotGrid.CustomAppearance",
					shortDescription: @"Demonstrates how to customize the appearances of field values and data cells.",
					description: @"
                        <Paragraph>
                        This demo shows how field values and cell appearance can be conditionally customized. In this sample, a Pivot Grid control highlights a row and column where the focused cell resides, along with corresponding field values. To do this, the CustomCellAppearance and CustomValueAppearance events are handled to specify custom appearance for data cells and field values, respectively.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Field Value Appearance Customization",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/woody/archive/2011/05/25/wpf-pivotgrid-field-value-appearance-customization-coming-in-v2011-vol-1.aspx"),
						new WpfModuleLink(
							title: "CustomAppearance",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=CustomAppearance&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "Groups",
					displayName: @"Groups",
					group: "Appearance and Layout",
					type: "PivotGridDemo.PivotGrid.Groups",
					shortDescription: @"Demonstrates how to arrange related fields into groups.",
					description: @"
                        <Paragraph>
                        The PivotGridControl allows you to join related fields into groups. When end-users drag a field that's included into a group the entire group is moved as the result. End-users cannot break the established groups. They can expand and collapse groups at any levels to show or hide the data related to particular fields. <LineBreak/> In this demo, fields in the Row, Column and Data areas are joined into groups. Try to drag them, expand and collapse individual fields.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Field Groups",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8020"),
						new WpfModuleLink(
							title: "Group Fields",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Group+Fields&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "CustomLayout",
					displayName: @"Custom Layout",
					group: "Appearance and Layout",
					type: "PivotGridDemo.PivotGrid.CustomLayout",
					shortDescription: @"Shows how to arrange Field Header Areas in a custom manner.",
					description: @"
                        <Paragraph>
                        PivotGridControl allows you to arrange its Field Header Areas in a custom manner. This is possible since standalone FieldList controls can display not only hidden fields, but fields from any specified pivot grid area.
                        </Paragraph>
                        <Paragraph>
                        All you need to do is to hide Field Header Areas displayed by default (disable all the Show...Headers properties), put a required number of FieldList controls onto a form (page), bind them to PivotGridControl via their Owner properties, and set their Area properties so that they display fields from different areas.
                        </Paragraph>
                        <Paragraph>
                        After that, you can place FieldList controls anywhere you wish, and even create a tabbed or docking-based layout.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PrintTemplates",
					displayName: @"Print Templates",
					group: "Printing",
					type: "PivotGridDemo.PivotGrid.PrintTemplates",
					shortDescription: @"Demonstrates how to apply pre-defined templates to customize the print output of the DXPivotGrid control.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to apply predefined templates to customize a print output of the DXPivotGrid control. In this demo, you can choose one of the following templates from the options pane:
                        </Paragraph>
                        <Paragraph>
                        • Default. The PivotGrid is printed using the default template.
                        </Paragraph>
                        <Paragraph>
                        • Moon Phase. Shows sales variations based on Moon phases. Axis values are replaced with the Moon images via the templates.
                        </Paragraph>
                        <Paragraph>
                        • Custom Print Theme. The PivotGrid is printed using the Office 2007 Blue theme colors.
                        </Paragraph>
                        <Paragraph>
                        To see the printout preview, click the Print Preview button. Note that the preview window can be used not only for previewing and printing a document, but also for exporting it into different file formats (e.g. PDF, XLS, XPS, etc.).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Printing and Exporting",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXPivotGridPrintAndExport"),
						new WpfModuleLink(
							title: "Print Templates",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Print+Templates&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "PrintOptions",
					displayName: @"Print Options",
					group: "Printing",
					type: "PivotGridDemo.PivotGrid.PrintOptions",
					shortDescription: @"Demonstrates how to customize the print output of the DXPivotGrid control.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to customize a print output of the DXPivotGrid control. Use the options pane to specify the visibility of field headers, field values, grid lines, etc. To see the printout preview, click the Print Preview button. Note that the preview window can be used not only for previewing and printing a document, but also for exporting it into different file formats (e.g. PDF, XLS, XPS, etc.).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Printing and Exporting",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXPivotGridPrintAndExport"),
						new WpfModuleLink(
							title: "Printing and Exporting",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8446")
					}
				),
				new WpfModule(demo,
					name: "Serialization",
					displayName: @"Serialization",
					group: "Miscellaneous",
					type: "PivotGridDemo.PivotGrid.Serialization",
					shortDescription: @"Demonstrates how to save the current pivot grid layout, and restore the previously saved one.",
					description: @"
                        <Paragraph>
                        This demo illustrates the layout serialization feature of the DXPivotGrid control. Using this feature, you can easily save and restore the DXPivotGrid's layout to keep, for example, the same layout between application runs. Another use case for this is to provide end-users with multiple predefined layouts so that they can switch from one to another, depending on their current requirements.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can customize the grid's layout and then click <Span>Save Layout</Span> to store it to the stream, and <Span>Restore Layout</Span> to apply the previously saved layout to the grid.
                        </Paragraph>
                        <Paragraph>
                        Also, this demo's options contain several predefined layouts (Original, Brief view and Full View), which demonstrate some real-life applications of the serialization feature.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Save and Restore Layout",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8023"),
						new WpfModuleLink(
							title: "Save and Restore Layout",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Save+Restore+Layout&p=T4|P6|158&d=16")
					}
				),
				new WpfModule(demo,
					name: "ContextMenu",
					displayName: @"Context Menu",
					group: "Miscellaneous",
					type: "PivotGridDemo.PivotGrid.ContextMenu",
					shortDescription: @"Demonstrates how to customize pivot grid context menus.",
					description: @"
                        <Paragraph>
                        This demo shows how you can add new items and remove predefined items from pivot grid context menus. The following customizations have been made for demonstration purposes.
                        </Paragraph>
                        <Paragraph>
                        • The Totals submenu has been added to Field Header and Field Header Area context menus. You can use this menu to specify the visibility settings for Totals and Grand Totals.
                        </Paragraph>
                        <Paragraph>
                        • All items, except for the new Totals submenu item, have been removed from the Field Header Area context menu.
                        </Paragraph>
                        <Paragraph>
                        • The Summary Type submenu has been added to the context menu invoked for Data Fields.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Context Menus",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8314"),
						new WpfModuleLink(
							title: "How to: Customize a Context Menu for Field Values",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8301"),
						new WpfModuleLink(
							title: "How to: Remove Items from the Context Menu",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8347")
					}
				)
			};
		}
	}
}
