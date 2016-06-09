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
		static List<Module> Create_XtraPivotGrid_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraPivotGrid %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "AsynchronousMode",
					displayName: @"Asynchronous Mode",
					group: "Data Processing",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.AsynchronousMode",
					description: @"This demo shows the PivotGridControl capabilities to perform data-aware operations (such as retrieving data from a data source, calculating summaries, sorting and filtering) in a background thread. This allows an entire application to stay responsive to end-user actions while Pivot Grid is processing data. In this demo, a Pivot Grid control is bound to a large data source. You can choose whether it is an OLAP cube, or a randomly generated table data source. Try to reorder fields, collapse and expand field values, toggle field sorting, and apply filtering to fields. While processing data, the Pivot Grid control will display a wait indicator to show that it is busy. The rest of the application UI will continue responding to your actions.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\AsyncMode.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\AsyncMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "ServerMode",
					displayName: @"Server Mode",
					group: "Data Processing",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.ServerMode",
					description: @"The DevExpress Server Mode feature improves performance when binding the PivotGridControl to a large data set. In this mode, only small portions of data are loaded into the PivotGridControl on demand, and all required data processing is performed on the database server side. This technique significantly reduces the application's response time.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\ServerMode.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\ServerMode.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomerReports",
					displayName: @"Customer Reports",
					group: "Sample Reports",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.CustomerReports",
					description: @"*",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\CustomerReports.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\CustomerReports.vb"
					}
				),
				new SimpleModule(demo,
					name: "ProductReports",
					displayName: @"Product Reports",
					group: "Sample Reports",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.ProductReports",
					description: @"*",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\ProductReports.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\ProductReports.vb"
					}
				),
				new SimpleModule(demo,
					name: "OrderReports",
					displayName: @"Order Reports",
					group: "Sample Reports",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.OrderReports",
					description: @"*",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\OrderReports.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\OrderReports.vb"
					}
				),
				new SimpleModule(demo,
					name: "SingleTotal",
					displayName: @"Single Total",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.SingleTotal",
					description: @"The PivotGridControl automatically calculates grand totals for each row and column. Additionally, totals are automatically calculated for each value group. The type of the automatic totals always matches the type of the summaries calculated in cells. Thus, automatic totals give you a more general data view while you still displaying the details. In this demo, you can use a number of view options to control the availability of totals.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\SingleTotal.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\SingleTotal.vb"
					}
				),
				new SimpleModule(demo,
					name: "MultipleTotals",
					displayName: @"Multiple Totals",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.MultipleTotals",
					description: @"For each field, you can manually specify the number and type of group totals to be displayed. This demo shows how to display the Average, Sum, Maximum and Minimum summaries for each Category group.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 5,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\MultipleTotals.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\MultipleTotals.vb"
					}
				),
				new SimpleModule(demo,
					name: "SummaryVariation",
					displayName: @"Summary Display Mode",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.SummaryVariation",
					description: @"Rather than displaying raw summary values in cells, you can show how these values correlate to values in other cells, and so perform a range of different data analyses. For instance, you can display the differences between summaries in the current and previous cells, or the percentage of a column's or row's total. Use the Summary Display Type combo box to choose the summary display mode, and see how this changes values in every second data column.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 2,
					newUpdatedPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\SummaryVariation.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\SummaryVariation.vb"
					}
				),
				new SimpleModule(demo,
					name: "TotalsLocation",
					displayName: @"Totals Location",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.TotalsLocation",
					description: @"This demo shows you how to control the location of group and grand totals. These totals can be displayed either before or after the corresponding data cells.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\TotalsLocation.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\TotalsLocation.vb"
					}
				),
				new SimpleModule(demo,
					name: "SortBySummary",
					displayName: @"Sort by Summary",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.SortBySummary",
					description: @"This demo illustrates the Sorting By Summary feature which lets you sort the values of a particular column field or row field by the summary values calculated against a specific data field. In this example, the values of the 'Sales Person' field are actually sorted by summary values calculated against another data field. You can select this data field via the combo box at the top of the XtraPivotGrid control. Clicking the 'Sales Person' field will reverse the current sort order.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\SortBySummary.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\SortBySummary.vb"
					}
				),
				new SimpleModule(demo,
					name: "ConditionalSortBySummary",
					displayName: @"Sort by Column",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.ConditionalSortBySummary",
					description: @"This example demonstrates how to sort fields by column summaries. To do this, right-click on the column's last level cell and select the required field. To sort fields in code, use the SortBySummaryInfo.Conditions property.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\ConditionalSortBySummary.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\ConditionalSortBySummary.vb"
					}
				),
				new SimpleModule(demo,
					name: "TopValues",
					displayName: @"Top Values",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.TopValues",
					description: @"This example demonstrates the Top X Values feature of the XtraPivotGrid control. For any column field or row field you can specify how many values should be displayed and used to calculate summaries. Thus you can only select the most significant values and ignore less important information. In this example, the XtraPivotGrid control displays the specified number of values for the selected field. Note that the values in this field are sorted against the values in the 'Order Amount' field.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\TopValues.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\TopValues.vb"
					}
				),
				new SimpleModule(demo,
					name: "RunningTotal",
					displayName: @"Running Totals",
					group: "Summary",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.RunningTotal",
					description: @"This example demonstrates how to include previous cell values into the values of the next cell. To do so, please set the RunningTotal property of the corresponding field to true.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\RunningTotal.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\RunningTotal.vb"
					}
				),
				new SimpleModule(demo,
					name: "GroupInterval",
					displayName: @"Interval Grouping",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.GroupInterval",
					description: @"This example demonstrates the data grouping feature provided by the XtraPivotGrid control. The OrderDate field is bound to a data source field which contains date/time values. As you can see, however, the OrderDate field doesn't display actual dates; instead it combines the information by years, months or quarterly intervals depending upon what you select. In addition, the data grouping feature is applied to the Product field and this combines records into a single category if they start with the same letter.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\GroupInterval.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\GroupInterval.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomGroupInterval",
					displayName: @"Custom Group Interval",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.CustomGroupInterval",
					description: @"In this demo we use the Custom Group Intervals feature to group product names and make the report more readable. The Product axis will show three intervals (""A-E"", ""F-S"" and ""T-Z"").",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\CustomGroupInterval.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\CustomGroupInterval.vb"
					}
				),
				new SimpleModule(demo,
					name: "UnboundColumns",
					displayName: @"Unbound Fields",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.UnboundColumns",
					description: @"This demo illustrates the powerful Unbound Fields functionality. The XtraPivotGrid control lets you create Unbound Fields which are not bound to any field in the control's underlying data source. These fields should be populated manually (for instance, they can display custom information and even use the summary results calculated by the grid). The unbound 'Order Amount' and 'Sales Person' fields are populated via the CustomUnboundFieldData event (this is an equivalent of the calculated columns provided by various data sources). You can specify how values for these fields are calculated using the Sales Person Format and Order Amount Rule combo boxes. The data cells that correspond to unbound 'Percent of OrderTotal' and 'Bonus Amount' unbound fields are populated via the CustomCellDisplayText event. Cell values of the 'Bonus Amount' field are calculated depending upon the value of the 'Order Amount' field. Cell values of the ' Percent of OrderTotal' field specify the percentage ratio of the current order's sum as compared to the total order's sum.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\UnboundColumns.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\UnboundColumns.vb"
					}
				),
				new SimpleModule(demo,
					name: "UnboundExpressions",
					displayName: @"Unbound Expressions",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.UnboundExpressions",
					description: @"This demo illustrates how to populate unbound fields with data using formulas (string expressions). Values of the New Year Bonus, Order Count Bonus, Bonus Amount data fields and the Sales Person row field are calculated according to specific expressions. In this demo, you can use the combo-box on the left options panel to specify expressions for Sales Person field values, or specify custom expressions via the Expression Editor, invoked by clicking the field value. Also, you can add a new bonus and specify its summary expression using the right options panel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\UnboundExpressions.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\UnboundExpressions.vb"
					}
				),
				new SimpleModule(demo,
					name: "FieldsCustomization",
					displayName: @"Fields Customization",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.FieldsCustomization",
					description: @"This example shows the Fields Customization Form, which allows an end-user to temporarily hide specific fields and then restore them again when needed. You can switch between the simple and advanced (Office2007) display modes. In the advanced mode, it is possible to move fields between areas by dragging their headers within the Customization Form.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\FieldsCustomization.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\FieldsCustomization.vb"
					}
				),
				new SimpleModule(demo,
					name: "Groups",
					displayName: @"Groups",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.Groups",
					description: @"The PivotGridControl allows you to join related fields into groups. When end-users drag a field that's included into a group the entire group is moved as the result. End-users cannot break the established groups. They can expand and collapse groups at any levels to show or hide the data related to particular fields. In this demo, fields in the Row, Column and Data areas are joined into groups. Try to drag them, expand and collapse individual fields.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\Groups.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\Groups.vb"
					}
				),
				new SimpleModule(demo,
					name: "HitInfo",
					displayName: @"Hit Info",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.HitInfo",
					description: @"The PivotGridControl lets you easily determine the element located at a particular point. For instance, you can determine the element that's currently under the mouse pointer. This demo shows you how to do this. Move the mouse pointer around and the upper panel will display information that uniquely identifies the hot-tracked element.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\HitInfo.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\HitInfo.vb"
					}
				),
				new SimpleModule(demo,
					name: "RunTimeChangeSummaryType",
					displayName: @"Runtime Summary Change",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.RunTimeChangeSummaryType",
					description: @"For data fields, you can enable the runtime summary change feature, where an end-user can click a data field header, to select a summary type for a field. This allows you to present information more compactly, limiting the number of visible data fields, while still having the ability to calculate a specific summary type. In this demo, the summary change feature is enabled when the Compact Layout option is selected. Select this option, and click the data field's header to access the available summary types.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\RunTimeChangeSummaryType.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\RunTimeChangeSummaryType.vb"
					}
				),
				new SimpleModule(demo,
					name: "Prefilter",
					displayName: @"Prefilter",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.Prefilter",
					description: @"The Prefilter allows end-users to build complex filter criteria with an unlimited number of filter conditions, combined by logical operators. End-users can open the Prefilter by right-clicking on any header or header area and selecting the Show Prefilter menu item. Or, end-user can open it by clicking the 'Edit Prefilter' button displayed within the Prefilter panel.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\Prefilter.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\Prefilter.vb"
					}
				),
				new SimpleModule(demo,
					name: "FilterPopup",
					displayName: @"Filter Popup",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.FilterPopup",
					description: @"This demo shows the extended capabilities of filter popup windows, which are used to filter against fields and groups. See these features in action, by enabling them via corresponding check boxes. Click the Product Name or Year field filter button to open a field or group filter popup.",
					addedIn: KnownDXVersion.Before142,
					newUpdatedPriority: 0,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\FilterPopup.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\FilterPopup.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomCellValue",
					displayName: @"Custom Cell Values",
					group: "Data Shaping Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.CustomCellValue",
					description: @"This demo shows how to replace cell values with custom ones. In the example, a dedicated event is handled to replace the values of total row cells with values of the preceding rows.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\CustomCellValue.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\CustomCellValue.vb"
					}
				),
				new SimpleModule(demo,
					name: "OLAP",
					displayName: @"OLAP Browser",
					group: "OLAP Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.OLAP",
					description: @"Possibly the most important PivotGridControl functionality is OLAP support. The PivotGridControl supports both Microsoft Analysis Services 2000, 2005 and 2008. As a data source, it can use an ordinary server, http server or cube file. You can play with OLAP support by clicking 'New Connection' button and setting up the connection. You should have msolap 8.0 or 9.0 OLEDB provider installed properly on your system. This demo also illustrates the advanced display style (Office2007) for the Fields Customization Form.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 1,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\OLAP.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\OLAP.vb"
					}
				),
				new SimpleModule(demo,
					name: "OlapCustomTotals",
					displayName: @"OLAP Multiple Totals",
					group: "OLAP Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.OlapCustomTotals",
					description: @"For each field, you can manually specify the number and type of group totals to be displayed. This demo shows how to display the Average, Sum, Maximum and Minimum summaries for each Category group.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\OlapCustomTotals.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\OlapCustomTotals.vb"
					}
				),
				new SimpleModule(demo,
					name: "OLAPDrillDown",
					displayName: @"OLAP Drill Down",
					group: "OLAP Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.OLAPDrillDown",
					description: @"In this demo, double click a summary cell to view the records from the control's underlying data source associated with this cell. The obtained data is displayed by the DataGridView within a popup window.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 4,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\OLAPDrillDown.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\OLAPDrillDown.vb"
					}
				),
				new SimpleModule(demo,
					name: "OLAPKPI",
					displayName: @"OLAP KPI",
					group: "OLAP Features",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.OLAPKPI",
					description: @"If an OLAP cube contains KPI (Key Performance Indicator) information, PivotGridControl can automatically recognize it and display it in an appropriate format. This demo displays a sample report for the Internet Revenue KPI from the Adventure Works sample cube. All KPI status and trend values have associated graphics that can be specified using the PivotGridField.KPIGraphic property. In this demo the required images can be specified via the 'Status Graphics' and 'Trend Graphics' comboboxes.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\OLAPKPI.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\OLAPKPI.vb"
					}
				),
				new SimpleModule(demo,
					name: "FormatRules",
					displayName: @"Format Rules",
					group: "Appearance",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.FormatRules",
					description: @"This example demonstrates the Format Rules feature which lets you customize the appearance settings of particular cells depending upon specific conditions. You can customize Style Conditions or create new ones involving a specific field's values.",
					addedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\FormatRules.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\FormatRules.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomDrawEvents",
					displayName: @"Custom Draw Events",
					group: "Appearance",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.CustomDrawEvents",
					description: @"This example demonstrates the PivotGrid custom draw feature. CustomDraw events allows you to change the PivotGrid default appearance the way you want it to be. For example, common tasks are: show changed cells, highlight focused columns and rows, dynamically change the font at runtime. To implement it, you should handle a proper event: CustomDrawCell - to override cells' appearance, CustomDrawFieldValue - to override field values' appearance. There are two ways to handle them: partial and full. In the partial way, you need to change the e.Appearance object, but don't draw anything yourself. In the full way you should draw everything yourself. To switch between modes, assign true to the e.Handled property for the full mode, or false for the partial.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\CustomDrawEvents.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\CustomDrawEvents.vb"
					}
				),
				new SimpleModule(demo,
					name: "CompactLayout",
					displayName: @"Compact Layout",
					group: "Appearance",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.CompactLayout",
					description: @"By default, the PivotGridControl shows all rows in a hierarchical way, which is common for pivot tables (Full Layout). However, when there are a lot of data fields in a Row Area, it may be necessary to show them in a tree-like manner (Compact Layout). In this demo, you can switch from Compact Layout to Full Layout and move data fields from one area to another to see what the pivot grid looks like with these settings.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\CompactLayout.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\CompactLayout.vb"
					}
				),
				new SimpleModule(demo,
					name: "InplaceEditors",
					displayName: @"In-place Editors",
					group: "Editing",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.InplaceEditors",
					description: @"This example shows how to make the PivotGridControl editable. You should assign a RepositoryItem descendant to the PivotGridField.FieldEdit property of the data field you want to make editable. In this demo the first data field has a RepositoryItemCalcEdit editor and the second has a RepositoryItemProgressBar editor. To activate the editor double click on the cell.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\InplaceEditors.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\InplaceEditors.vb"
					}
				),
				new SimpleModule(demo,
					name: "Validation",
					displayName: @"Validation",
					group: "Editing",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.Validation",
					description: @"This example shows how to check values entered by end-users. You can prompt the user to change entered values if they don't fit applied criteria. Handle the ValidatingEditor event to perform this check. In this demo, you can change criteria using the input fields above. Cells that don't fit into the criteria are marked in red.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\Validation.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\Validation.vb"
					}
				),
				new SimpleModule(demo,
					name: "ChartGeneralOptions",
					displayName: @"General Chart Options",
					group: "Charts Integration",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.ChartGeneralOptions",
					description: @"You can easily chart data from the PivotGridControl. When you attach a PivotGridControl to a ChartControl, the latter graphically displays data in the current selection of the PivotGridControl. The ChartControl will use row values as series and column values as arguments if you wish to chart data horizontally, or vice versa if you want to do this vertically. When you want to chart an entire row or column, you don't have to select all its cells. Just select the total of a row or column.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\ChartGeneralOptions.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\ChartGeneralOptions.vb"
					}
				),
				new SimpleModule(demo,
					name: "CustomChartData",
					displayName: @"Custom Chart Data",
					group: "Charts Integration",
					type: "DevExpress.XtraPivotGrid.Demos.Modules.CustomChartData",
					description: @"This demo illustrates how to customize data passed from the XtraPivotGrid to the Chart control. The XtraPivotGrid allows you to customize data exported to the Chart control. In this demo, use the 'Pivot Grid Settings' options group to specify how row field values should be represented in the Chart control. You can also specify the minimum data cell value passed to the chart. Data cells with smaller values are exported as zeros. Use the 'Chart Settings' options group to specify the chart type, and define whether point labels should be visible. Expand and collapse field groups in the pivot grid to change the data scale.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PivotGridMainDemo\Modules\CustomChartData.cs",
						@"\WinForms\VB\PivotGridMainDemo\Modules\CustomChartData.vb"
					}
				)
			};
		}
	}
}
