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
		static List<Module> Create_DXTreeList_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "EndUserCustomization",
					displayName: @"End-User Customization",
					group: "Features",
					type: "TreeListDemo.EndUserCustomization",
					shortDescription: @"A variety of options allows you to customize which operations are available for end-users.",
					description: @"
                        <Paragraph>
                        A wide variety of options enables you to fine tune end-user capabilities. These include moving, resizing, showing and hiding columns; sorting, filtering and editing data; using built-in context menus and many more.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,
					updatedIn: KnownDXVersion.V152,
					links: new[] {
						new WpfModuleLink(
							title: "WPF TreeList - Binding to Data",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXTreeListWPFDataBind;DXTreeList+for+WPF.product;1"),
						new WpfModuleLink(
							title: "Silverlight and WPF TreeList Control",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/04/26/silverlight-and-wpf-treelist-control-coming-in-v2011-1.aspx"),
						new WpfModuleLink(
							title: "Data Editing",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9920"),
						new WpfModuleLink(
							title: "Sorting",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9921"),
						new WpfModuleLink(
							title: "Showing and Hiding Columns",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9924"),
						new WpfModuleLink(
							title: "Navigating Through Nodes and Cells",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9925"),
						new WpfModuleLink(
							title: "Resizing Columns",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9926"),
						new WpfModuleLink(
							title: "Expanding and Collapsing Nodes",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9927"),
						new WpfModuleLink(
							title: "TreeList",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=TreeList&p=T4%7cP6%7c0&d=16"),
						new WpfModuleLink(
							title: "DXGrid demo: TreeList View",
							type: WpfModuleLinkType.Demos,
							url: "GridDemo:TreeList View"),
						new WpfModuleLink(
							title: "DXGrid demo: Multi View",
							type: WpfModuleLinkType.Demos,
							url: "GridDemo:Multi View")
					}
				),
				new WpfModule(demo,
					name: "BandedLayout",
					displayName: @"Banded Layout",
					group: "Features",
					type: "TreeListDemo.BandedLayout",
					shortDescription: @"DXTreeList allows you to arrange column headers into bands and create multi-row layout.",
					description: @"
                        <Paragraph>
                        DXTreeList allows you to arrange column headers into bands and provides an extended layout and greater customization options for complex datasets. End-users can hide, show, reorder entire column sets instead of having to perform the same operation on each individual column. You can also create data cells that occupy multiple rows.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "NodeChecking",
					displayName: @"Node Check Boxes",
					group: "Features",
					type: "TreeListDemo.NodeChecking",
					shortDescription: @"This demo shows how to select individual files/folders and display their total size.",
					description: @"
                        <Paragraph>
                        DXTreeList allows you to display check boxes within nodes (next to the Expand button). Use the CheckBoxFieldName property to bind check boxes to a Boolean field in a data source. The visibility of check boxes is controlled by the ShowCheckBoxes option.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the TreeList displays the file/folder tree. Use node check boxes to select files and folders. Their total size is automatically recalculated and displayed within the Fixed Summary Panel.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "DragDrop",
					displayName: @"Drag and Drop Nodes",
					group: "Features",
					type: "TreeListDemo.DragDrop",
					shortDescription: @"This demo shows the internal drag and drop functionality provided by the DXTreeList control.",
					description: @"
                        <Paragraph>
                        Enabling drag and drop you allow your end-users to restructure the node hierarchy within a treelist control, move nodes outside the treelist to external controls that support drag and drop and vice versa.
                        </Paragraph>
                        <Paragraph>
                        To rearrange nodes, focus a node or select multiple nodes (if multiple selection is enabled), drag and drop onto the required location. To move nodes to a child collection of another node that is collapsed, drop a node(s) onto the target node.
                        </Paragraph>
                        <Paragraph>
                        Custom drag and drop functionality can be implemented by handling the corresponding events provided by a Drag and Drop Manager.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "AdvancedPrintingOptions",
					displayName: @"Printing Options",
					group: "Features",
					type: "TreeListDemo.AdvancedPrintingOptions",
					shortDescription: @"The DevExpress WPF Printing Library allows you to instantly render and print the DXTreeList.",
					description: @"
                        <Paragraph>
                        This demo illustrates the flexibility and customization options of the <Bold>DevExpress Printing Library</Bold> when used to render the contents of the DXTreeList (whether you print to paper or export to multiple file formats).
                        </Paragraph>
                        <Paragraph>
                        You can instantly define the visibility of column headers and total summaries in the resulting report. You can choose whether to automatically adjust the DXTreeList’s layout before it is printed (expand all its nodes, show node images and expand buttons, adjust column widths) or to print the grid as-is.
                        </Paragraph>
                        <Paragraph>
                        To view the rendered contents of the DXTreeList, click the <Bold>New Tab Preview</Bold> or <Bold>New Window Preview</Bold> button. The Preview window allows you to both print the document and export its contents to multiple file formats (e.g. PDF, XLS, XPS, etc).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "NodeTemplate",
					displayName: @"Node Templates",
					group: "Features",
					type: "TreeListDemo.NodeTemplate",
					shortDescription: @"Use Templates to deliver a unique look and feel across individual TreeListView elements.",
					description: @"
                        <Paragraph>
                        This demo illustrates the customization options available to you when using node templates to create unique data presentation styles.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Details</Bold> – Nodes present data using a default template; all node details are expanded.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Animated Details</Bold> – Nodes present tabular data using a default template; node details are expanded with an animation when a node obtains focus.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Tooltip</Bold> – Nodes present data using a default template; node details are shown in a tooltip (displayed when a mouse pointer hovers over a node).
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Default</Bold> – No template is applied.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Note:</Bold> Images Courtesy NASA/JPL-Caltec
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Filtering",
					displayName: @"Data Filtering",
					group: "Features",
					type: "TreeListDemo.Filtering",
					shortDescription: @"DXTreeList allows to use all filtering features of DXGrid for hierarchical data.",
					description: @"
                        <Paragraph>
                        DXTreeList provides the <Bold>Smart Filtering Mode</Bold> that allows to hide nodes that doesn't meet the filter criteria and change the hierarchy level of visible nodes to preserve the integrity of the hierarchical data.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142, 
					updatedIn: KnownDXVersion.V151,
					links: new[] {
						new WpfModuleLink(
							title: "Filtering Overview",
							type: WpfModuleLinkType.Documentation,
							url: " http://documentation.devexpress.com/#WPF/CustomDocument6130"),
						new WpfModuleLink(
							title: "Nodes Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9615")
					}
				),
				new WpfModule(demo,
					name: "MultiSelection",
					displayName: @"Multiple Node Selection",
					group: "Features",
					type: "TreeListDemo.MultiSelection",
					shortDescription: @"End-users can select multiple nodes using the mouse or keyboard.",
					description: @"
                        <Paragraph>
                        DXTreeList provides the capability to select multiple nodes. To enable multiple node selection, set the TreeListView's <Bold>MultiSelectMode</Bold> property to <Bold>Row</Bold>. End-users can select multiple nodes in one of the following ways:
                        </Paragraph>
                        <Paragraph>
                        • by holding either the SHIFT or CTRL key, and selecting nodes using the mouse pointer;
                        </Paragraph>
                        <Paragraph>
                        • by holding the SHIFT key, and selecting nodes using the TOP and BOTTOM arrow keys;
                        </Paragraph>
                        <Paragraph>
                        • by holding the CTRL key, navigating to nodes using the TOP and BOTTOM arrow keys and selecting some of them using the SPACE key.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "UnboundColumns",
					displayName: @"Unbound Columns",
					group: "Features",
					type: "TreeListDemo.UnboundColumns",
					shortDescription: @"Unbound columns allow you to display data from a custom data source or calculated based upon the values of bound columns.",
					description: @"
                        <Paragraph>
                        Bound columns obtain their data from data fields in a control’s data source. Unbound columns are not bound to any field in the data source.
                        </Paragraph>
                        <Paragraph>
                        Unbound columns must be populated by specifying a string expression (e.g. <Italic>=&lt;&lt;FirstName&gt;&gt; + ' ' + &lt;&lt;LastName&gt;&gt;""</Italic>) or within the CustomUnboundColumnData event handler. Unbound columns are treated in the same way as regular (bound) columns, so they can be used in filters and data bindings.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the tree view displays two unbound columns (<Bold>Full Name</Bold> and <Bold>Age</Bold>). Their values are calculated according to unbound expressions, which can be edited via the <Bold>Expression Editor</Bold>. This editor can be invoked using an unbound column's context menu. In this demo, you can select the required expression in 'Expression Editor', click the 'Edit Column Expression' button and modify it.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BuildTreeFromSelfReferenceData",
					displayName: @"Self Referencing Data",
					group: "Data Binding",
					type: "TreeListDemo.BuildTreeFromSelfReferenceData",
					shortDescription: @"This demo shows how to create a self referencing data structure and display it within the TreeListView.",
					description: @"
                        <Paragraph>
                        This demo shows how to create a self referencing (hierarchical) data structure and display it within the TreeListView. To build a tree, a data source should contain two additional (service) fields:
                        </Paragraph>
                        <Paragraph>
                        <InlineUIContainer> <StackPanel Margin=""30,0,0,0""> <TextBlock><Bold>- Key Field</Bold></TextBlock> <TextBlock TextWrapping=""Wrap"">This field must contain unique values identifying nodes (data records). Its name must be assigned to the TreeListView.KeyFieldName property.</TextBlock> <TextBlock><Bold>- Parent Field</Bold></TextBlock> <TextBlock TextWrapping=""Wrap"">This field must contain values indicating a parent node for the current node. It is used to build Parent-Child relations. Its name should be assigned to the TreeListView.ParentFieldName property.</TextBlock> </StackPanel> </InlineUIContainer>
                        </Paragraph>
                        <Paragraph>
                        In this demo, the data source is represented by the list of Employee objects. The key and parent fields are represented by the <Bold>Id</Bold> and <Bold>ParentId</Bold> properties, respectively.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to Self-Referential Data Structure",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9571"),
						new WpfModuleLink(
							title: "WPF & Silverlight Tree List Control - Tree Derivation Modes",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/08/22/wpf-amp-silverlight-tree-list-control-tree-derivation-modes.aspx")
					}
				),
				new WpfModule(demo,
					name: "DataEditorTypes",
					displayName: @"Data Editor Types",
					group: "Features",
					type: "TreeListDemo.DataEditorTypes",
					shortDescription: @"DevExpress Editors Library ships with over 20 controls that can be used within the cells for in-place data editing.",
					description: @"
                        <Paragraph>
                        This demo illustrates the power of the DevExpress WPF TreeList control and its rich collection of in-cell data editors.
                        </Paragraph>
                        <Paragraph>
                        The TreeList's data source includes a <Bold>EditorType</Bold> field. This field contains strings that correspond to cell editor types. A <Bold>MultiEditorsTemplateSelector</Bold> object assigned to a column’s <Bold>CellTemplateSelector</Bold> returns a cell editor template based on the value in the <Bold>EditorType</Bold> column. This editor is used to edit cell values displayed within the processed row.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BuildTreeViaChildNodesSelector",
					displayName: @"Child Node Selector",
					group: "Data Binding",
					type: "TreeListDemo.BuildTreeViaChildNodesSelector",
					shortDescription: @"This demo shows how to build a tree from hierarchical data structure using a selector class.",
					description: @"
                        <Paragraph>
                        In data bound mode, the TreeList can display information in a tree from either self-referential (flat) or hierarchical data structure. For each type of a data structure, it provides the corresponding tree derivation mode(s).
                        </Paragraph>
                        <Paragraph>
                        This demo shows how to manually write code to specify where a data object's child items come from. A selector class that implements <Bold>DevExpress.Xpf.Grid.IChildNodesSelector</Bold> is created.  Its SelectChildren method is overridden to return the list of child nodes for the specified node.
                        </Paragraph>
                        <Paragraph>
                        Finally, the Child Nodes Selector is assigned to the <Bold>TreeListView.ChildNodesSelector</Bold> property and the TreeListView.TreeDerivationMode property is set to 'ChildNodesSelector'.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to Hierarchical Data Structure",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument10366"),
						new WpfModuleLink(
							title: "WPF & Silverlight Tree List Control - Tree Derivation Modes",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/08/22/wpf-amp-silverlight-tree-list-control-tree-derivation-modes.aspx")
					}
				),
				new WpfModule(demo,
					name: "BuildTreeViaHierarchicalDataTemplate",
					displayName: @"Hierarchical Data Template",
					group: "Data Binding",
					type: "TreeListDemo.BuildTreeViaHierarchicalDataTemplate",
					shortDescription: @"This demo shows how to build a tree from hierarchical data structure using HierarchicalDataTemplate.",
					description: @"
                        <Paragraph>
                        In data bound mode, the TreeList can display information in a tree from either self-referential (flat) or hierarchical data structure. For each type of a data structure, it provides the corresponding tree derivation mode(s).
                        </Paragraph>
                        <Paragraph>
                        This demo takes advantage of hierarchical data templates to build the entire hierarchy.
                        </Paragraph>",
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to Hierarchical Data Structure",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument10366"),
						new WpfModuleLink(
							title: "WPF & Silverlight Tree List Control - Tree Derivation Modes",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/08/22/wpf-amp-silverlight-tree-list-control-tree-derivation-modes.aspx")
					}
				),
				new WpfModule(demo,
					name: "DynamicNodeLoading",
					displayName: @"Load Nodes Dynamically",
					group: "Data Binding",
					type: "TreeListDemo.DynamicNodeLoading",
					shortDescription: @"In this demo, the control is not bound to a data source. The tree is manually created in code.",
					description: @"
                        <Paragraph>
                        In <Bold>unbound mode</Bold>, you need to manually create a tree (in code or XAML). The collection of root nodes can be accessed via the view’s Nodes collection. Individual nodes provide the Nodes collection, which stores their child nodes. To create a new node, create a new instance of the TreeListNode class, initialize its settings, and add it to the required collection.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the TreeListView displays the file/folder tree. Child nodes that correspond to sub folders or files contained within a folder are dynamically created when a parent node is being expanded.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Dynamic Data Loading",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9702")
					}
				),
				new WpfModule(demo,
					name: "UnboundMode",
					displayName: @"Unbound Data",
					group: "Data Binding",
					type: "TreeListDemo.UnboundMode",
					shortDescription: @"In unbound mode, you should manually build a TREE by creating nodes and adding them to corresponding node collections.",
					description: @"
                        <Paragraph>
                        Nodes are stored as nested collections because the <Bold>TreeListView</Bold> displays data in a tree. The collection of root level nodes can be accessed via the <Bold>TreeListView.Nodes</Bold> property. Each node has its own collection of child nodes available via the <Bold>TreeListNode.Nodes</Bold> property. These child nodes have their own children, etc.
                        </Paragraph>
                        <Paragraph>
                        In <Bold>unbound mode</Bold>, you should manually build a TREE by creating nodes (<Bold>TreeListNode</Bold>) and adding them to the corresponding node collections. In this demo, root level nodes are created in XAML. Child nodes are created in code.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Important Note:</Bold> Nodes can be represented by objects of different types. The only requirement is that these data objects should have common fields (columns).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "TreeList View Unbound Mode",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9567")
					}
				),
				new WpfModule(demo,
					name: "SmartColumnsGeneration",
					displayName: @"Smart Columns Generation",
					group: "Data Binding",
					type: "TreeListDemo.SmartColumnsGeneration",
					shortDescription: @"This demo illustrates the Smart Columns Generation feature.",
					description: @"
                        <Paragraph>
                        The DevExpress WPF TreeList control introduces the Smart Columns Generation feature. When this feature is enabled, the TreeList automatically creates the layout and configures data editors for all generated columns according to data types, data annotations, etc. specified in the underlying data source.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ConditionalFormatting",
					displayName: @"Conditional Formatting",
					group: "Features",
					type: "TreeListDemo.ConditionalFormatting",
					shortDescription: @"Use conditional formatting to explore market data. Conditional formatting features are available from column context menu.",
					description: @"
                        <Paragraph>
                        The <Bold>DXTreeList</Bold> allows you to apply conditional formatting and change the appearance of individual cells and row based on specific conditions. This powerful option helps highlight critical information or describe trends within cells by using data bars, color scales or built-in icon sets.
                        </Paragraph>
                        <Paragraph>
                        To apply or remove a conditional format for a column, select the <Bold>Conditional Formatting</Bold> item in the column context menu. You can either call a formatting dialog specific to the column for which the menu is called, or click <Bold>Manage Rules</Bold> to invoke the <Bold>Conditional Formatting Rules Manager</Bold> - a convenient tool which allows you to view and edit all formatting rules currently applied to the TreeList.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151,
					isFeatured: true
				),
			new WpfModule(demo,
					name: "ClipboardFormats",
					displayName: @"Clipboard Formats",
					group: "Features",
					type: "TreeListDemo.ClipboardFormats",
					shortDescription: @"Demonstrates copying the treelist content to the Excel, RTF and HTML formats",
					description: @"
                        <Paragraph>
                        The <Bold>DXTreeList</Bold> allows its content to be copied to the clipboard as a formatted data.  This functionality is managed via the <Bold>View.ClipboardMode</Bold> option that is enabled in this demo.
                        </Paragraph>
                        <Paragraph>
                        To see the copy/paste functionality for the formatted data in action, select cells and click the <Bold>Copy and Paste</Bold> button.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				)
			};
		}
	}
}
