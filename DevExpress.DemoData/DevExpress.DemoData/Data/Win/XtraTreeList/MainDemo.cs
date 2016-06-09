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
		static List<Module> Create_XtraTreeList_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraTreeList %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraTreeList.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "CodeExamples",
					displayName: @"Code Examples",
					group: "API Code Examples",
					type: "DevExpress.XtraTreeList.Demos.CodeExamples",
					description: @"This section contains examples that illustrate the TreeList control's API. The samples demonstrate how to populate the TreeList control with items, customize its settings and cover other essential TreeList concepts.",
					addedIn: KnownDXVersion.V152,
					isCodeExample: true
				),
				new SimpleModule(demo,
					name: "TreeListRegViewer",
					displayName: @"Reg Viewer",
					group: "Data Sources",
					type: "DevExpress.XtraTreeList.Demos.TreeListRegViewer",
					description: @"This demo shows how to work with the XtraTreeList in unbound mode.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\TreeListMainDemo\Modules\RegViewer.cs",
						@"\WinForms\VB\TreeListMainDemo\Modules\RegViewer.vb"
					}
				),
				new SimpleModule(demo,
					name: "ExplorerNew",
					displayName: @"Explorer(Virtual Tree)",
					group: "Data Sources",
					type: "DevExpress.XtraTreeList.Demos.ExplorerNew",
					description: @"This example demonstrates how to dynamically populate the TreeList control with data via events. More information on this functionality can be found in the ""Dynamic Data Loading via Events"" help topic.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "BusinessObjectBinding",
					displayName: @"Business Object Binding",
					group: "Data Sources",
					type: "DevExpress.XtraTreeList.Demos.BusinessObjectBinding",
					description: @"The XtraTreeList provides a simple way to display a custom business object that provides hierarchical data. This can be done by implementing the IVirtualTreeListData interface or by handling specific events. This demo illustrates the first approach - binding to a business object that implements the IVirtualTreeListData interface. In this demo, the business object allows data to be modified via the TreeList control, and supports change notifications via the IBindingList interface. Practice changing values in the TreeList control, and see how corresponding values in the property grid on the right are changed.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "TreeListMultiEditors",
					displayName: @"Multi Editors",
					group: "Data Editing",
					type: "DevExpress.XtraTreeList.Demos.TreeListMultiEditors",
					description: @"In this demo editors are dynamically assigned to individual cells. For the cells in the last node that use progress bar in-place editors you can press the '+' or '-' keys to increase or decrease cell values, respectively.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\TreeListMainDemo\Modules\MultiEditors.cs",
						@"\WinForms\VB\TreeListMainDemo\Modules\MultiEditors.vb"
					}
				),
				new SimpleModule(demo,
					name: "LookUp",
					displayName: @"TreeListLookUp Editor",
					group: "Data Editing",
					type: "DevExpress.XtraTreeList.Demos.LookUp",
					description: @"This demo illustrates the TreeListLookUpEdit control that provides the lookup functionality. It uses an embedded XtraTreeList control to represent the dropdown hierarchy. See how the TreeListLookUpEdit control is used in standalone and in-place modes.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "NodesFiltering",
					displayName: @"Nodes Filtering",
					group: "Data Filtering",
					type: "DevExpress.XtraTreeList.Demos.NodesFiltering",
					description: @"This example demonstrates the filtering functionality of the XtraTreeList. Filter conditions can be defined via Microsoft Excel Style Filter Dropdowns, Automatic Filter Row or Advanced Filter Editor Dialog. The current filter expression is displayed within the Filter Panel displayed at the TreeList's  bottom-most edge.",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "FormatRules",
					displayName: @"Conditional Formatting (MS Excel style)",
					group: "Data Filtering",
					type: "DevExpress.XtraTreeList.Demos.FormatRules",
					description: @"This example demonstrates the XtraTreeList’s Excel inspired conditional formatting. This feature allows you to highlight cells or rows based on specified criteria, without writing a single line of code. You can highlight cells using data bars, icons, predefined appearance schemes or custom appearances. At runtime, end-users can apply a style condition to a column via the column’s context menu.",
					addedIn: KnownDXVersion.V142,
					updatedIn: KnownDXVersion.V151
				),									   
				new SimpleModule(demo,
					name: "ClipboardFormats",
					displayName: @"Clipboard formats",
					group: "Miscellaneous",
					type: "DevExpress.XtraTreeList.Demos.ClipboardFormats",
					description: @"The TreeList allows its data to be copied to the clipboard either as plain text or as formatted data. This example demonstrates copying treelist data to the clipboard along with formatting. The data copied is then pasted in three formats (MS Excel, RTF and HTML) to three external controls. To see the copy/paste functionality in action, use the ""Copy and Paste"" button, or select a cell range and press the CTRL+C combination.To enable the default plain copy/paste mode, use the ClipboardMode setting.",
					addedIn: KnownDXVersion.V152,
					isFeatured: true,
					associatedFiles: new [] {
						@"\WinForms\CS\TreeListMainDemo\Modules\Clipboard.cs",
						@"\WinForms\VB\TreeListMainDemo\Modules\Clipboard.vb"
					}
				),
				new SimpleModule(demo,
					name: "FixedColumns",
					displayName: @"Fixed Columns",
					group: "Layout",
					type: "DevExpress.XtraTreeList.Demos.FixedColumns",
					description: @"Any column within the XtraTreeList can be anchored to the left or right edge. Such columns are not horizontally scrolled. Right-click a column's header to fix the column via a popup menu.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Bands",
					displayName: @"Banded Layout",
					group: "Layout",
					type: "DevExpress.XtraTreeList.Demos.Bands",
					description: @"The XtraTreeList Control allows you to organize column headers into bands and deliver advanced layout and customization options for complex datasets. End-users can hide, display and reorder entire column sets instead of performing the same operation on each column individually.",
					addedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new SimpleModule(demo,
					name: "TreeListDragDrop",
					displayName: @"Drag And Drop",
					group: "Miscellaneous",
					type: "DevExpress.XtraTreeList.Demos.TreeListDragDrop",
					description: @"Select a listview element and drag it onto the XtraTreeList. You also can move nodes from the XtraTreeList to the recycle bin (when you are performing internal Drag And Drop and the CTRL key is pressed then the dragged node is copied).",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\TreeListMainDemo\Modules\DragDrop.cs",
						@"\WinForms\VB\TreeListMainDemo\Modules\DragDrop.vb"
					}
				),
				new SimpleModule(demo,
					name: "MultiDragAndDrop",
					displayName: @"Multi Drag And Drop",
					group: "Miscellaneous",
					type: "DevExpress.XtraTreeList.Demos.MultiDragAndDrop",
					description: @"Select a node, and drag it to a different location within this control or a separate TreeList control. Enable the AllowMultiDrag option to be able to drag multiple nodes simultaneously. If you hold the CTRL key down when dropping a node to a new location, it will be copied instead of being moved from its old location.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\TreeListMainDemo\Modules\MultiDragAndDrop.cs",
						@"\WinForms\VB\TreeListMainDemo\Modules\MultiDragAndDrop.vb"
					}
				),				
				new SimpleModule(demo,
					name: "MultiSelect",
					displayName: @"Multi Select",
					group: "Miscellaneous",
					type: "DevExpress.XtraTreeList.Demos.MultiSelect",
					description: @"This example demonstrates the multi-selection capabilities of the XtraTreeList. You can switch between row and cell selection. Hold the CTRL button to select individual rows or cells. Hold the SHIFT button or the mouse button to select blocks of rows/cells.",
					addedIn: KnownDXVersion.V152,
					associatedFiles: new [] {
						@"\WinForms\CS\TreeListMainDemo\Modules\MultiSelect.cs",
						@"\WinForms\VB\TreeListMainDemo\Modules\MultiSelect.vb"
					}
				),				
			};
		}
	}
}
