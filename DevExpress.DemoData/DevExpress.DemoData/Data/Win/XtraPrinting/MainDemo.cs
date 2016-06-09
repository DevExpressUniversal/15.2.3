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
		static List<Module> Create_XtraPrinting_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraPrinting %MarketingVersion%",
					group: "About",
					type: "XtraPrintingDemos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "HierarchicalReport",
					displayName: @"Master-Detail",
					group: "Miscellaneous",
					type: "XtraPrintingDemos.HierarchicalReport.PreviewControl",
					description: @"This demo illustrates a hierarchical master-detail report created with the XtraPrinting library. To show a separate print preview of a detail report, click the 'book' icon in the left column of each data row.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\MasterDetail\MasterDetailControl.cs",
						@"\WinForms\VB\PrintingMainDemo\MasterDetail\MasterDetailControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "ReportService",
					displayName: @"Remote Documents",
					group: "Miscellaneous",
					type: "XtraPrintingDemos.ReportService.PreviewControl",
					description: @"This demo illustrates the capability to display the Print Preview for documents that are supplied by a remote WCF report service. In this demo, to select a remotely created document to preview, use the Reports button in the ribbon menu.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\ReportService\PreviewControl.cs",
						@"\WinForms\VB\PrintingMainDemo\ReportService\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "Calendar",
					displayName: @"Calendar",
					group: "Miscellaneous",
					type: "XtraPrintingDemos.Calendar.CalendarModule",
					description: @"This demo illustrates publishing a custom calendar. To make the calendar show a different date, use the Date and Month View buttons on the Demo Actions toolbar tab.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\Calendar\Calendar.cs",
						@"\WinForms\VB\PrintingMainDemo\Calendar\Calendar.vb"
					}
				),
				new SimpleModule(demo,
					name: "BioLifePrintingLabels",
					displayName: @"Labels",
					group: "Layout",
					type: "XtraPrintingDemos.BioLifePrinting.PreviewControlLabels",
					description: @"This demo illustrates how to create a document in which every data record is represented as a separate label. In this demo you can change the data records in the bound database, using the navigation bar located at the bottom of the print control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\Layout\BioLifePrinting.cs",
						@"\WinForms\VB\PrintingMainDemo\Layout\BioLifePrinting.vb"
					}
				),
				new SimpleModule(demo,
					name: "BioLifePrintingTable",
					displayName: @"Table",
					group: "Layout",
					type: "XtraPrintingDemos.BioLifePrinting.PreviewControlTable",
					description: @"This demo illustrates how to create a document in which every data record is represented as a table row, and the entire document is arranged into a table. In this demo you can change the data records in the bound database, using the navigation bar located at the bottom of the print control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\Layout\BioLifePrinting.cs",
						@"\WinForms\VB\PrintingMainDemo\Layout\BioLifePrinting.vb"
					}
				),
				new SimpleModule(demo,
					name: "BioLifePrintingGroups",
					displayName: @"Groups",
					group: "Layout",
					type: "XtraPrintingDemos.BioLifePrinting.PreviewControlGroups",
					description: @"This demo illustrates how to create a document with grouped data, in which every data record is represented as a single group. In this demo you can change the data records in the bound database, using the navigation bar located at the bottom of the print control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\Layout\BioLifePrinting.cs",
						@"\WinForms\VB\PrintingMainDemo\Layout\BioLifePrinting.vb"
					}
				),
				new SimpleModule(demo,
					name: "DataGridPrinting",
					displayName: @"DataGrid",
					group: "Printable Controls",
					type: "XtraPrintingDemos.DataGridPrinting.PreviewControl",
					description: @"This demo illustrates an implementation of a printing link to publish the content of a standard DataGrid control for Windows Forms. To populate the grid with different data, use the drop-down menu on the Demo Actions toolbar tab and click Refresh.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\DataGrid\PreviewControl.cs",
						@"\WinForms\VB\PrintingMainDemo\DataGrid\PreviewControl.vb"
					}
				),
				new SimpleModule(demo,
					name: "PrintableList",
					displayName: @"ListView",
					group: "Printable Controls",
					type: "XtraPrintingDemos.PrintableList.PreviewControl",
					description: @"This demo illustrates the implementation of the IPrintable interface to publish custom Windows Forms controls. To apply the options that are provided to the custom control in this demo, click the Options button in the Print Preview toolbar tab.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\ListView\PrintableListView.cs",
						@"\WinForms\VB\PrintingMainDemo\ListView\PrintableListView.vb"
					}
				),
				new SimpleModule(demo,
					name: "RichRext",
					displayName: @"RichTextBox",
					group: "Printable Controls",
					type: "XtraPrintingDemos.RichRext.PreviewControl",
					description: @"This demo illustrates an implementation of a printing link to publish the content of a standard RichTextBox control for Windows Forms. After modifying the document content using the Dock Panel's editor, click Refresh on the Demo Actions toolbar tab.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\PrintingMainDemo\RichText\PreviewControl.cs",
						@"\WinForms\VB\PrintingMainDemo\RichText\PreviewControl.vb"
					}
				)
			};
		}
	}
}
