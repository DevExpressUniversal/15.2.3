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
		static List<Module> Create_SnapForWin_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress Snap %MarketingVersion%",
					group: "About",
					type: "SnapDemos.Modules.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "MasterDetailReport",
					displayName: @"Master-Detail Report",
					group: "Report Types",
					type: "SnapDemos.Modules.MasterDetailReport",
					description: @"This demo illustrates a master-detail report created from a hierarchical data source. Select fields from tables in the Data Explorer, drop them onto the document surface, and Snap will automatically create a master-detail layout based on table relations.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\MasterDetailReport.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\MasterDetailReport.vb"
					}
				),
				new SimpleModule(demo,
					name: "MultipleDataSources",
					displayName: @"Multiple Data Sources",
					group: "Data",
					type: "SnapDemos.Modules.MultipleDatasources",
					description: @"This demo illustrates a report created from multiple unrelated data sources of different types. Use the Data Explorer to browse the structure of available data sources, create new data sources (via the context menu) and modify existing data sources. After you select fields in the Data Explorer and drop them onto the document surface, Snap will automatically create the report layout.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 2,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\MultipleDatasources.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\MultipleDatasources.vb"
					}
				),
				new SimpleModule(demo,
					name: "TableOfContents",
					displayName: @"Table of Contents",
					group: "Report Types",
					type: "SnapDemos.Modules.TableOfContents",
					description: @"This demo illustrates a report with an automatically generated table of contents. To include a field's text into the table of contents, select the field and click Add Text in the References tab of the main menu. To maintain the hierarchical structure of your report, you can specify the level at which this text should appear in the table of contents. To apply changes, click Table of Contents in the References tab of the main menu. To view the final document (with all data and actual page numbers displayed), switch to the File menu and click Print Preview.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 3,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\TableOfContents.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\TableOfContents.vb"
					}
				),
				new SimpleModule(demo,
					name: "BarCode",
					displayName: @"Bar Code",
					group: "Report Types",
					type: "SnapDemos.Modules.BarCode",
					description: @"This demo illustrates the variety of bar code types supported in Snap.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\BarCode.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\BarCode.vb"
					}
				),
				new SimpleModule(demo,
					name: "API",
					displayName: @"Application Programming Interface",
					group: "Runtime",
					type: "SnapDemos.Modules.API",
					description: @"This demo shows how you can use Snap API (application programming interface) to fully control the creation of documents from code. In this demo, customize the grid’s layout (e.g., by applying a filtering, grouping or sorting criteria to the available data column) to view these changes being translated to the auto-generated Snap report. To view the document, click the Generate Report button.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\API.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\API.vb"
					}
				),
				new SimpleModule(demo,
					name: "Events",
					displayName: @"Events",
					group: "Runtime",
					type: "SnapDemos.Modules.Events",
					description: @"This demo illustrates the structure of Snap events that allow you to modify the default application behavior. To handle the events presented in this demo, enable the corresponding check boxes in the above menu. Then select fields in the Data Explorer and drop them onto the document surface. The program's behavior changes depending on which event handlers you activate.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\Events.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\Events.vb"
					}
				),
				new SimpleModule(demo,
					name: "Charts",
					displayName: @"Charts",
					group: "Report Types",
					type: "SnapDemos.Modules.Charts",
					description: @"This demo illustrates a report with charts. To add a new chart, switch to the Insert tab in the main menu and click Chart. To provide data to your chart, select items in Data Explorer and drop them onto the appropriate chart areas (Values and Arguments). Increase the chart's dimensions if it does not fit all of the data.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\Charts.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\Charts.vb"
					}
				),
				new SimpleModule(demo,
					name: "MultiColumnReport",
					displayName: @"Multi-Column Report",
					group: "Report Types",
					type: "SnapDemos.Modules.MultiColumnReport",
					description: @"This demo illustrates a report with a multiple column layout. To change the number of columns, switch to the Page Layout tab in the main menu, click Columns, and specify the required number of columns.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\MultiColumnReport.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\MultiColumnReport.vb"
					}
				),
				new SimpleModule(demo,
					name: "CalculatedFields",
					displayName: @"Calculated Fields",
					group: "Data",
					type: "SnapDemos.Modules.CalculatedFields",
					description: @"This demo illustrates the capability to perform calculations over data fields and show the results in your documents. In this demo, the Extended Price field is added to the Data Explorer. The field's Expression property returns an aggregated value consisting of the UnitPrice, Quantity and Discount fields. After a calculated field is created (by right-clicking a data table in the Data Explorer and choosing Add Calculated Field), you can drop it onto your document like an ordinary field.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\CalculatedFields.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\CalculatedFields.vb"
					}
				),
				new SimpleModule(demo,
					name: "IListDataSource",
					displayName: @"IList Data Source",
					group: "Data",
					type: "SnapDemos.Modules.IListDatasource",
					description: @"This demo illustrates a report bound to a custom data source (e.g., an object implementing the System.Collections.IList interface).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\IListDataSource.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\IListDataSource.vb"
					}
				),
				new SimpleModule(demo,
					name: "FormattedFields",
					displayName: @"Formatted Content",
					group: "Data",
					type: "SnapDemos.Modules.FormattedFields",
					description: @"This demo illustrates the capability to parse formatted content within rich text documents. This content may include the HTML, RTF, DOC, DOCX, WordML and OpenDocument formats.
The document shown in this demo lists the supported HTML tags and provides sample markup content that is obtained from the document’s data source.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\FormattedFields.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\FormattedFields.vb"
					}
				),
				new SimpleModule(demo,
					name: "Sparklines",
					displayName: @"Sparklines",
					group: "Report Types",
					type: "SnapDemos.Modules.Sparklines",
					description: @"This demo illustrates how sparklines can be used to enhance a report.

In this demo, the report embeds different types of sparklines to provide a visual cue to the dynamics of temperature change in world cities by years.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\Sparklines.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\Sparklines.vb"
					}
				),
				new SimpleModule(demo,
					name: "MailMergeReports",
					displayName: @"Mail Merge Reports",
					group: "Report Types",
					type: "SnapDemos.Modules.MailMergeReports",
					description: @"This demo illustrates the capability to create mail merge reports in Snap.

The mail merge feature allows you to insert dynamic data (text, dates, images, etc.) into a static document template.The report types that can be generated with the mail merge feature range from a simple form letter or envelop to a complex business report.",
					addedIn: KnownDXVersion.Before142,
					featuredPriority: 0,
					isFeatured: true,
					associatedFiles: new [] {
						@"\Reporting\CS\SnapMainDemo\Modules\MailMergeReports.cs",
						@"\Reporting\VB\SnapMainDemo\Modules\MailMergeReports.vb"
					}
				)
			};
		}
	}
}
