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
		static List<Module> Create_DXGrid_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "MultiView",
					displayName: @"Switch View Types",
					group: "Layout and Customization",
					type: "GridDemo.MultiView",
					shortDescription: @"The DXGrid control provides three view types and the ability to create multi-row layouts.",
					description: @"
                        <Paragraph>
                        The DXGrid control uses Views to display data from a data source. A View specifies the layout of data fields and records, provides options and settings that control the availability of individual data management capabilities (sorting, grouping, editing, etc.) and the appearance of grid elements. The DXGrid supports three view types:
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Table View</Bold> (Default View)<LineBreak /> A Table View represents data in a two-dimensional table. Data source fields are represented by grid columns. Data records are represented by data rows.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Card View</Bold><LineBreak /> A Card View represents data as cards. Data records are represented by cards. A card arranges data source fields vertically, in a single column.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>TreeList View</Bold><LineBreak /> A TreeList View is designed to display information in a tree from a self-referenced data structures in either bound or unbound mode. Data records are represented by nodes.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Multi-Row Table View</Bold><LineBreak /> Provides an extended layout and greater customization options for complex datasets. End-users can hide, show, reorder entire column sets instead of having to perform the same operation on each individual column.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
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
							title: "Table View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6294"),
						new WpfModuleLink(
							title: "TreeList View Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9686"),
						new WpfModuleLink(
							title: "Card View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6297"),
						new WpfModuleLink(
							title: "TableView",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=TableView&p=T4%7cP6%7c0&d=16"),
						new WpfModuleLink(
							title: "TreeListView",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=TreeListView&p=T4%7cP6%7c0&d=16"),
						new WpfModuleLink(
							title: "CardView",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=CardView&p=T4%7cP6%7c0&d=16"),
						new WpfModuleLink(
							title: "TreeList View",
							type: WpfModuleLinkType.Demos,
							url: "local:TreeList View"),
						new WpfModuleLink(
							title: "DXTreeList demo: End-User Customization",
							type: WpfModuleLinkType.Demos,
							url: "TreeListDemo:End-User Customization")
					}
				),
				new WpfModule(demo,
					name: "StandardTableView",
					displayName: @"Standard Table View",
					group: "Layout and Customization",
					type: "GridDemo.StandardTableView",
					shortDescription: @"Provides a complete set of UI customization options to deliver an outstanding end-user experience.",
					description: @"
                        <Paragraph>
                        The Table View is the most widely accepted and used method of presenting data. This view presents data in a standard tabular format where columns represent data fields and rows represent data records.
                        </Paragraph>
                        <Paragraph>
                        The Table View supports numerous data management and layout customization features, including Unlimited Data Sorting and Grouping, Data Filtering and Data Summary Calculation, Cascading Data Updates, Extended Record Scrolling, Fixed Columns and Column Auto Width, Column Reordering and Resizing, Automatic Column Width Adjustment (Best Fit), Customizable Grid Lines and much more...
                        </Paragraph>
                        <Paragraph>
                        In this demo, use the Options Panel to see how this or that option affects the grid's appearance and/or behavior.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,   
					updatedIn: KnownDXVersion.V151,
					links: new[] {
						new WpfModuleLink(
							title: "Table View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6294"),
						new WpfModuleLink(
							title: "Table View",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Table+View&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "BandedView",
					displayName: @"Banded Layout",
					group: "Layout and Customization",
					type: "GridDemo.BandedView",
					shortDescription: @"Table View allows you to arrange column headers into bands and create multi-row layout.",
					description: @"
                        <Paragraph>
                        The Table View allows you to arrange column headers into bands and provides an extended layout and greater customization options for complex datasets. End-users can hide, show, reorder entire column sets instead of having to perform the same operation on each individual column. You can also create data cells that occupy multiple rows.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "FixedBands",
					displayName: @"Fixed Bands",
					group: "Layout and Customization",
					type: "GridDemo.FixedBands",
					shortDescription: @"You can anchor one or more bands to a Grid's left or rightmost edge.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to anchor bands to the left or rightmost edge of the DXGrid similar to fixed data columns. Anchored bands are constantly visible on screen while other bands are scrolled horizontally.
                        Each time a DXGrid is horizontally scrolled, it renders only those columns that are displayed onscreen. This is the horizontal scrolling virtualization feature which is enabled by default and speeds up the grid's performance when displaying multiple columns.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TreeListView",
					displayName: @"TreeList View",
					group: "Layout and Customization",
					type: "GridDemo.TreeListView",
					shortDescription: @"With the TreeList View you can represent data to end-users in the form of a tree, a list or a grid.",
					description: @"
                        <Paragraph>
                        The TreeListView can represent any self-referenced data structure in either bound or unbound mode. Along with the standard data-aware and presentation features, such as data editing, sorting, filtering, summary calculation, built-in validation, unbound columns, runtime column customization and so on, its specific features include:
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Represent Hierarchical Data Relationships</Bold><LineBreak /> With the TreeListView you can represent any self-referenced data structure in either bound or unbound mode.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Unbound Mode Support</Bold><LineBreak /> In unbound mode, you can manually create the entire tree in code or XAML.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>On-Demand Data Loading in Unbound Mode</Bold><LineBreak /> In unbound mode, you can implement on-demand data loading (handle a special event to dynamically create child nodes before a parent node is expanded).
                        </Paragraph>
                        <Paragraph>
                        • <Bold>TreeNode Iterator</Bold><LineBreak /> To simplify node traversal, the TreeListView is shipped with an easy to use node iterator class.
                        </Paragraph>
                        <Paragraph>
                        • <Bold>Node Images</Bold><LineBreak /> You can explicitly specify node images or obtain images from a field in a data source with long binary data.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					isFeatured: true,	
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
							title: "TreeList View Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9686"),
						new WpfModuleLink(
							title: "TreeListView",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=TreeListView&p=T4%7cP6%7c0&d=16"),
						new WpfModuleLink(
							title: "Multi View",
							type: WpfModuleLinkType.Demos,
							url: "local:Multi View"),
						new WpfModuleLink(
							title: "DXTreeList demo: End-User Customization",
							type: WpfModuleLinkType.Demos,
							url: "TreeListDemo:End-User Customization")
					}
				),
				new WpfModule(demo,
					name: "CardView",
					displayName: @"Card View",
					group: "Layout and Customization",
					type: "GridDemo.CardView",
					shortDescription: @"You can display data rows within the DXGrid using a card metaphor.",
					description: @"
                        <Paragraph>
                        In addition to tables, the DXGrid control can display data rows as Cards. The Options pane allows you to modify settings associated with the Card View.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Cards Layout</Bold> - Specifies whether Cards should be arranged across columns (<Bold>Horizontal</Bold>) or rows (<Bold>Vertical</Bold>);
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Allow Resizing</Bold> - Allow/Prohibit the resizing of a column or row using the splitter;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Fixed Size</Bold> - Specifies the fixed size (height or width depending upon current orientation) of a Card;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Cards in a Row</Bold> - Specifies an unlimited number of Cards within a row/column or limits them using <Bold>MaxValue</Bold>;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Cards Alignment</Bold> - Specifies how Cards are aligned inside a visible area.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> If the <Bold>Allow Resizing</Bold> option is checked, you can resize a row or column using the splitter that is shown between individual Cards.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					updatedIn: KnownDXVersion.V151,
					links: new[] {
						new WpfModuleLink(
							title: "Card View",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6297"),
						new WpfModuleLink(
							title: "CardView",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=CardView&p=T4|P4|107&d=16")
					}
				),
				  new WpfModule(demo,
					name: "CardWinExplorer",
					displayName: @"Card View File Explorer",
					group: "Layout and Customization",
					type: "GridDemo.CardWinExplorer",
					shortDescription: @"A functional File Explorer based on the GridControl with CardView.",
					description: @"
                        <Paragraph>
                         This example demonstrates the customizing capabilities of the GridControl.
                        </Paragraph>
                        <Paragraph>
                         You can navigate through your files and folders that are represented via cards.
                         The custom context menu that is invoked via a right mouse click on a card allows you to open the file/folder or view the properties.
                        </Paragraph>
                        <Paragraph>
                         All features of the Card View are present: the ability to switch the layout and grouping of items, the scrolling experience, smooth animation of expanding and collapsing groups etc.
                        </Paragraph>",
					addedIn: KnownDXVersion.V151
				),
				new WpfModule(demo,
					name: "UnboundColumns",
					displayName: @"Unbound Columns",
					group: "Layout and Customization",
					type: "GridDemo.UnboundColumns",
					shortDescription: @"Unbound columns are not bound to any field in a data source and must be populated manually.",
					description: @"
                        <Paragraph>
                        With DXGrid it is possible to display not only data from the datasource, but also unbound columns specified by a string expression (e.g. =""<<Price>> * <<Quantity>>"" ). Note that unbound columns are treated by DXGrid in the same way as regular columns, so they can be used in filters and data bindings.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the grid displays two unbound columns ( <Bold>Discount Amount</Bold> and <Bold>Total</Bold> ). Values of these columns are calculated according to specific expressions, which can be edited via the <Bold>Expression Editor</Bold> . This editor can be invoked using an unbound column's context menu. In this demo, you can select a required unbound column in 'Columns', and click the 'Show Expression Editor' button
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> In this demo, a <Bold>DataTemplateSelector</Bold> is used to apply a custom cell template to <Bold>Total</Bold> cells with values greater than <Bold>500</Bold> .
                        </Paragraph>
                        <Paragraph>
                        There is another unbound column in the grid, one that is invisible. It is used to represent the size of an animated arrow for those <Bold>Total</Bold> cells that are above 500. This demonstrates how easy it is to bind to unbound columns using data templates.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> In a cell template, you can bind to any column value, not just to the current column value. Thanks to the powerful data binding model implemented in DXGrid, the same is true for all parts of the grid (such as card templates, row templates, and so on).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Unbound Columns For DXGrid",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#UnboundColumnsForDXGrid;DXGrid+for+WPF.product;4"),
						new WpfModuleLink(
							title: "Silverlight and WPF Grid – Custom Expressions for Unbound Columns in Server Mode",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/05/10/silverlight-and-wpf-grid-custom-expressions-for-unbound-columns-in-server-mode-coming-in-v2011-1.aspx"),
						new WpfModuleLink(
							title: "Unbound Columns",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6124"),
						new WpfModuleLink(
							title: "Unbound Columns",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Unbound+Columns&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "RowTemplate",
					displayName: @"Row Templates",
					group: "Layout and Customization",
					type: "GridDemo.RowTemplate",
					shortDescription: @"Use Templates to deliver a unique look and feel across individual DXGrid elements.",
					description: @"
                        <Paragraph>
                        This demo illustrates the customization options available to you when using row templates to create unique data presentation styles.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Animated Details</Bold> - Rows present tabular data using a default row template; row details are expanded with an animation when a row obtains focus.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Details</Bold> - Rows present data using a default row template; all row details are expanded. The width of the image is bound to the first visible column's width. Resize the First Name column and see how the image is resized.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Mail Merge</Bold> - Rows and row details are presented as a flow document, substituting data fields (including images) into a formatted document (this is commonly known as ""mail merge""). The image is wrapped with text when you resize the window.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Tooltip</Bold> - Rows present data using a default row template; row details are shown in a tooltip (displayed when a mouse pointer hovers over a row).
                        </Paragraph>
                        <Paragraph>
                        Additionally, cells within this demo can be represented as a hyperlink (check/uncheck the Use Email Template option and click the Email column to send a message).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Grid Elements That Support Templates",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6770"),
						new WpfModuleLink(
							title: "DataRowTemplate",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=DataRowTemplate&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "CardTemplates",
					displayName: @"Card Templates",
					group: "Layout and Customization",
					type: "GridDemo.CardTemplates",
					shortDescription: @"Use Templates to deliver a unique look and feel across individual DXGrid Card elements.",
					description: @"
                        <Paragraph>
                        This demo illustrates the customization options available to you when using card templates to create unique data presentation styles. The structure of a card is more complex than the structure of a row and consists of different levels:
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Card Header.</Bold> To display data in a card header, <Bold>CardHeaderBinding</Bold> is used. You can define your own card header template and bind to the values of any column or header.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Cards Contents.</Bold> You can define any complex template to render the card. For example, you can choose <Bold>Tabbed Details</Bold> to show details using a tab control, <Bold>Expandable Notes</Bold> to make it possible to collapse/expand Notes, or use the Default template.
                        </Paragraph>
                        <Paragraph>
                        You can also define a template for a card data member cell or a template for the entire card data member.
                        </Paragraph>
                        <Paragraph>
                        If card contents are defined using the <Bold>Default</Bold> template, card data members are sorted in the same order as the columns in the expandable columns window. Changing this order causes the card data members to be reordered. To see this in action, click on the arrow button in the top-right corner, invoke the editor and change the order of two columns in it.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Grid Elements That Support Templates",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6770")
					}
				),
				new WpfModule(demo,
					name: "PersistentRowState",
					displayName: @"Persistent Row State",
					group: "Layout and Customization",
					type: "GridDemo.PersistentRowState",
					shortDescription: @"External information can be associated with rows and preserved when grouping/sorting/scrolling data.",
					description: @"
                        <Paragraph>
                        With DXGrid, it's possible to store external information in the dependency object associated with each data row so that this information is preserved when grouping and sorting grid data. This feature is useful in various scenarios; for example, to store the expanded/collapsed state of a card.
                        </Paragraph>
                        <Paragraph>
                        To see this feature in the <Bold>Table View,</Bold> first resize any row and scroll its contents, and then scroll, group and sort the grid and see that the state of all rows has been preserved.
                        </Paragraph>
                        <Paragraph>
                        To see this feature in the <Bold>Card View,</Bold> collapse a few cards or use the ""+"" and ""-"" buttons to change their size. Then scroll through the grid records, sort and group them, and see that the visual state of all the cards is saved.
                        </Paragraph>
                        <Paragraph>
                        This feature is also used in the <Bold>Card Templates</Bold> demo to preserve the selected tab index or the expanded state of the <Bold>Notes</Bold> panel within the cards.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Grid Elements That Support Templates",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6770"),
						new WpfModuleLink(
							title: "DataRowTemplate",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=DataRowTemplate&p=T4%7cP4%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "CurrentDataRow",
					displayName: @"Current Data Row",
					group: "Layout and Customization",
					type: "GridDemo.CurrentDataRow",
					shortDescription: @"This demo shows how to show detailed information about the focused row anywhere outside the grid.",
					description: @"
                        <Paragraph>
                        This feature is common in many real-life applications (such as email programs). It allows the end-user to select a row and see the data for that row in more detail.
                        </Paragraph>
                        <Paragraph>
                        To see how it works, select any grid row, and see how smoothly the panel with grid row details appears at the bottom of the grid.
                        </Paragraph>
                        <Paragraph>
                        Note in this demo the code-behind has no code.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FixedDataColumns",
					displayName: @"Fixed Data Columns",
					group: "Layout and Customization",
					type: "GridDemo.FixedDataColumns",
					shortDescription: @"One or more columns can be anchored to a Grid's left or rightmost edge.",
					description: @"
                        <Paragraph>
                        This demo illustrates how you can anchor columns to the left or rightmost edge of the DXGrid. This option is useful when you need data in a specific column to be constantly visible on screen while other columns are scrolled horizontally.
                        </Paragraph>
                        <Paragraph>
                        To begin, scroll the columns and note that the <Bold>Company Name</Bold> and <Bold>Phone</Bold> items are fixed. To fix any other column, click the ""wrench"" icon in its column header, and choose either the <Bold>Left</Bold> or <Bold>Right</Bold> fixed style. To fix different columns, click the ""wrench"" icon in the column header, and choose either the <Bold>Left</Bold> or <Bold>Right</Bold> fixed column style. To “unfix” the column, choose <Bold>None</Bold> from this options window.
                        </Paragraph>
                        <Paragraph>
                        Additionally, this demo allows you to customize the width of the fixed line that separates fixed and scrollable columns within the DXGrid.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Using Fixed Columns",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridFixedColumns;DXGrid+for+WPF.product;1")
					}
				),
				new WpfModule(demo,
					name: "ColumnCustomization",
					displayName: @"Column Customization",
					group: "Layout and Customization",
					type: "GridDemo.ColumnCustomization",
					shortDescription: @"The Column Chooser allows end-users to display/hide columns from the View via drag & drop.",
					description: @"
                        <Paragraph>
                        This demo illustrates the capabilities of the <Bold>Column Chooser</Bold> window – an intuitive way in which to enable end-user customization of DXGrid columns. The Column Chooser lists all grid columns that are present in the grid's Columns collection, but whose <Bold>Visible</Bold> property is set to <Bold>False</Bold>.
                        </Paragraph>
                        <Paragraph>
                        End-users can drag any column from the Column Chooser and drop it either as a column header to add the corresponding column, or drop it onto the Group Panel to group by that column. To hide a column from the DXGrid’s display, users can drag the appropriate column to the Column Chooser window.
                        </Paragraph>
                        <Paragraph>
                        To help demonstrate the UI flexibility available to you, the Options panel of this demo allows you to position the Column Chooser either at its Default location (within the DXGrid) or a Custom location (anywhere your needs dictate).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Column Chooser",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6154"),
						new WpfModuleLink(
							title: "Column Chooser",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Column+Chooser&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "Sparklines",
					displayName: @"Sparklines",
					group: "Layout and Customization",
					type: "GridDemo.Sparklines",
					shortDescription: @"This demo shows how to use the Sparkline control as a column editor.",
					description: @"
                        <Paragraph>
                        In this demo, the Sparkline control shows a line chart representing order statistics for each employee. In this chart, blue and red markes specify the lowest and highest payments respectively.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ConditionalFormatting",
					displayName: @"Conditional Formatting",
					group: "Layout and Customization",
					type: "GridDemo.ConditionalFormatting",
					shortDescription: @"Use conditional formatting to explore financial data. Conditional formatting features are available from column context menu.",
					description: @"
                        <Paragraph>
                        The DXGrid allows you to apply conditional formatting and change the appearance of individual cells and row based on specific conditions. This powerful option helps highlight critical information or describe trends within cells by using data bars, color scales or built-in icon sets.
                        </Paragraph>
                        <Paragraph>
                        To apply or remove a conditional format for a column, select the Conditional Formatting item in the column context menu. You can either call a formatting dialog specific to the column for which the menu is called, or click Manage Rules to invoke the Conditional Formatting Rules Manager - a convenient tool which allows you to view and edit all formatting rules currently applied to the view.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "CellMerging",
					displayName: @"Cell Merging",
					group: "Layout and Customization",
					type: "GridDemo.CellMerging",
					shortDescription: @"This example demonstrates the use of the DXGrid's cell merging feature.",
					description: @"
                        <Paragraph>
                        When cell merging is enabled, neighboring data cells across different rows are merged whenever they display matching values. This allows your applications to deliver data clarity and avoid duplication.
                        </Paragraph>
                        <Paragraph>
                        Cell merging supports numerous GridControl features including data editing, conditional formatting and data printing.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "WCFInstantFeedback",
					displayName: @"Server Mode",
					group: "Performance",
					type: "GridDemo.WCFInstantFeedback",
					shortDescription: @"With the patent-pending server side data processing, our data grid delivers unmatched speeds during all data shaping operations.",
					description: @"
                        <Paragraph>
                        The Instant Feedback™ UI Mode (an asynchronous loading mode) can be used with WCF Data Services. To activate this feature, the grid should be bound to <Bold>WcfInstantFeedbackDataSource</Bold>, which accesses data from the WCF Data Services using the OData protocol. The grid and application will always remain responsive to user actions regardless of data operations initiated against the grid - record scrolling, sorting, grouping, and data filtering. Data loading is performed asynchronously, in a background thread.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "MasterDetailView",
					displayName: @"Master-Detail View",
					group: "Master-Detail",
					type: "GridDemo.MasterDetailView",
					shortDescription: @"This demo shows the grid’s built-in support for master-detail data representation.",
					description: @"
                        <Paragraph>
                        In this demo, you will learn how to display master-detail data using the GridControl’s build-in functionality. To access detail sections, use the expand-collapse buttons displayed within master rows.
                        As you can notice the detail section content is freely customizable. You simply place required controls into the special detail area placeholder (DetailDescriptor) to setup detail layout and content.
                        When you place another Grid Control into the detail area placeholder, you can experience the full power of grid’s master-detail support. First of all, different details will be synchronized with each other. Try modifying a detail grid by reordering or resizing columns and see how it affects all other details.
                        Another thing worth mentioning is integration with the master view. If you enable the Group By box or filter the view, you’ll see how the Group Panel and Filter Panel are shared between master and detail.
                        Note that the grid control supports unlimited number of details per row and unlimited detail nesting levels. For instance, the Orders detail view has its own details.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "MasterDetailViewViaEntityFramework",
					displayName: @"Master-Detail via Entity Framework",
					group: "Master-Detail",
					type: "GridDemo.MasterDetailViewViaEntityFramework",
					shortDescription: @"Display master-detail data from an Entity Framework data source.",
					description: @"
                        <Paragraph>
                        In this demo, you will learn how to specify proper data binding for master-detail data representation with an Entity Framework data source.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "EmbeddedTableView",
					displayName: @"Embedded Table View",
					group: "Master-Detail",
					type: "GridDemo.EmbeddedTableView",
					shortDescription: @"Shows how to display a detail grid without using master-detail integration features.",
					description: @"
                        <Paragraph>
                        As you can see in the Master-Detail View Demo, there are benefits in using a Grid Control to represent detail data. It can fully integrate with the Master View by using a common Group By box and Filter Panel. You will also use a single scrollbar to navigate though both master and details.
                        </Paragraph>
                        <Paragraph>
                        Sometimes this integration is unnecessary and thus the DevExpress WPF Grid allows an alternative way to display detail data using another grid control.
                        </Paragraph>
                        <Paragraph>
                        When specifying the detail view, use a ContentDetailDescriptor object instead of DataControlDetailDescriptor. In this instance, the embedded grid control will be treated as any other control and no special integration features will be enabled.
                        Detail views will have separate scrollbars, Group By boxes, filter panels and will not be synchronized.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "EntityFrameworkModule",
					displayName: @"Entity Framework",
					group: "Server-Side Data Processing",
					type: "GridDemo.EntityFrameworkModule",
					shortDescription: @"In this demo, the DXGrid operates in Server Mode and is bound to the Entity Framework classes.",
					description: @"
                        <Paragraph>
                        In this demo, the grid can be bound to a <Bold>EntityInstantFeedbackSource</Bold> or <Bold>EntityServerModeSource</Bold> object. Both data source types allow you to use ‘LINQ to Entities Classes’ and enable grid to query data against the	Entity Framework. EntityServerModeSource enables a regular (synchronous) Server Mode, while EntityInstantFeedbackSource enables an Instant Feedback Mode (asynchronous server mode).
                        </Paragraph>
                        <Paragraph>
                        In a regular server mode, the control passes code execution to the data source and does not respond to a user's actions until data is retrieved. In Instant Feedback mode, a control does not freeze, but rather continues responding to a user's actions, while the data is being retrieved by the data source in a background thread.
                        </Paragraph>
                        <Paragraph>
                        Toggle the ‘Instant Feedback’ box to switch between synchronous and asynchronous server modes.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Data Grid – Server Mode using LINQ",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridLINQServerMode;DXGrid+for+WPF.product;1"),
						new WpfModuleLink(
							title: "Entity Framework 4.0 Server Mode",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument8601"),
						new WpfModuleLink(
							title: "LINQ Server Mode",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=LINQ+Server+Mode&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "MultiThreadDataProcessing",
					displayName: @"Multithread Data Processing",
					group: "Performance",
					type: "GridDemo.MultiThreadDataProcessing",
					shortDescription: @"In this demo we use all the available cores on the system to improve performance for data-intensive operations (sorting, grouping, etc).",
					description: @"
                        <Paragraph>
                        In this demo, the DevExpress Grid Control for WPF operates in <Bold>Instant Feedback</Bold> data binding mode on in-memory data. All operations on data (e.g. sorting, groping, filtering, summary calculation, etc.) are performed asynchronously and parallelized on multiple processors. This allows the computing power of your hardware to be utilized to the full extent without UI freezing.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Data Grid Control – PLINQ Data Support",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/08/22/wpf-data-grid-control-plinq-data-support.aspx"),
						new WpfModuleLink(
							title: "Binding to In-Memory Data Using PLINQ",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument10472"),
						new WpfModuleLink(
							title: "How to use PLINQ Instant Feedback UI mode",
							type: WpfModuleLinkType.KB,
							url: "http://www.devexpress.com/Support/Center/p/E3382.aspx")
					}
				),
				new WpfModule(demo,
					name: "LINQToSQLServer",
					displayName: @"LINQ to SQL Server",
					group: "Server-Side Data Processing",
					type: "GridDemo.LINQToSQLServer",
					shortDescription: @"The server mode can be enabled for any LINQ provider.",
					description: @"
                        <Paragraph>
                        The DXGrid control is the only WPF grid control on the market that can delegate all data processing operations to the database server for blazing fast data operations.
                        </Paragraph>
                        <Paragraph>
                        After you set connection options and create a connection to a SQL Server instance, the grid works in <Bold>server mode</Bold> using LINQ. In server mode, the DXGrid control loads data in small portions, optimizing memory usage and data transmission. In addition, all data-aware operations initiated by end-users via the grid control (sorting, grouping and filtering data, calculating summaries) are performed on the server side. All of this ensures rapid access to data and eliminates the various problems related to loading large volumes of data and processing it on the client side.
                        </Paragraph>
                        <Paragraph>
                        In this demo, the grid can be bound to a <Bold>LinqInstantFeedbackSource</Bold> or <Bold>LinqServerModeSource</Bold> object. Both data source types allow you to use ‘LINQ to SQL Classes’. LinqServerModeSource enables a regular (synchronous) Server Mode, while LinqInstantFeedbackSource enables an Instant Feedback Mode (asynchronous server mode).
                        </Paragraph>
                        <Paragraph>
                        In a regular server mode, the control passes code execution to the data source and does not respond to a user's actions until data is retrieved. In Instant Feedback mode, a control does not freeze, but rather continues responding to a user's actions, while the data is being retrieved by the data source in a background thread.
                        </Paragraph>
                        <Paragraph>
                        Toggle the ‘Instant Feedback’ box to switch between synchronous and asynchronous server modes.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> Try to add a large number of data rows to the grid (e.g. 300,000) and scroll, sort, group, expand/collapse groups, to see that DXGrid is really fast.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Data Grid – Server Mode using LINQ",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridLINQServerMode;DXGrid+for+WPF.product;1"),
						new WpfModuleLink(
							title: "LINQ Server Mode",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6280"),
						new WpfModuleLink(
							title: "LINQ Server Mode",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=LINQ+Server+Mode&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "XPOInstantFeedback",
					displayName: @"eXpress Persistent Objects",
					group: "Server-Side Data Processing",
					type: "GridDemo.XPOInstantFeedback",
					shortDescription: @"In this demo, the DXGrid is bound to eXpress Persistent Objects provided by the WCF service in Instant Feedback UI Mode.",
					description: @"
                        <Paragraph>
                        In this demo, the Grid Control operates in <Bold>Instant Feedback</Bold> data binding mode (asynchronous server mode).
                        </Paragraph>
                        <Paragraph>
                        The grid is bound to an <Bold>XPInstantFeedbackSource</Bold> object which accesses eXpress Persistent Objects via the WCF service in Instant Feedback mode. In this mode, you will no longer experience any UI freezing. Data operations will be performed asynchronously, in a background thread and both the Grid Control and your application will be always highly responsive.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Data Grid - Instant Feedback UI Mode",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridWPFInstantFeedback;DXGrid+for+WPF.product;1"),
						new WpfModuleLink(
							title: "WPF Grid Control and the New Instant Feedback UI Mode",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/03/01/wpf-grid-control-and-the-new-instant-feedback-ui-mode-coming-in-v2011-1.aspx"),
						new WpfModuleLink(
							title: "Instant Feedback",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Instant+Feedback&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "CollectionView",
					displayName: @"Collection View",
					group: "Data Binding",
					type: "GridDemo.CollectionView",
					shortDescription: @"This demo shows how to bind a grid to a CollectionView.",
					description: @"
                        <Paragraph>
                        The DevExpress WPF Grid Control supports ICollectionViews and automatically synchronizes its grouping, filtering, sorting, and the focused row (the current item).
                        </Paragraph>
                        <Paragraph>
                        In this demo, the grid is bound to ICollectionView. Use the Options Panel displayed on the left to group and sort data via ICollectionView. This can also be done via the grid. All the changes will be automatically synchronized between the grid and ICollectionView.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SmartColumnsGeneration",
					displayName: @"Smart Columns Generation",
					group: "Data Binding",
					type: "GridDemo.SmartColumnsGeneration",
					shortDescription: @"In this demo you will learn how to automatically generate UI using Data Annotation attributes or DevExpress Fluent API.",
					description: @"
                        <Paragraph>
                        The DevExpress WPF Grid Control introduces the Smart Columns Generation feature. When this feature is enabled, the Grid automatically creates the layout and configures data editors for all generated columns according to data types, Data Annotations and DevExpress Fluent API attributes specified in the underlying data source.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BindingToDynamicObject",
					displayName: @"Binding to Dynamic Object",
					group: "Data Binding",
					type: "GridDemo.BindingToDynamicObject",
					shortDescription: @"This demo shows how to bind a grid to a collection of dynamic objects.",
					description: @"
                        <Paragraph>
                        This demo shows how to bind a grid to a collection of dynamic objects.
                        </Paragraph>
                        <Paragraph>
                        The DXGrid supports binding to properties of Dynamic objects. In this demo, a grid is bound to a collection of DynamicDictionary objects, which is derived from the DynamicObject class. The DynamicDictionary class contains an object of the Dictionary (string, object) to store key-value pairs, and overrides the TrySetMember and TryGetMember methods to support the new syntax. Initially, it provides the Id, FirstName and LastName properties.
                        </Paragraph>
                        <Paragraph>
                        Dynamic objects are created and initialized at runtime. You can add new dynamic properties using data editors displayed within the Options panel. Once a new dynamic property is declared, the grid will automatically create a corresponding column and populate it with default values.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BindingToXML",
					displayName: @"Binding to XML",
					group: "Data Binding",
					type: "GridDemo.BindingToXML",
					shortDescription: @"This demo shows how to display structured data from an XML file.",
					description: @"
                        <Paragraph>
                        This example shows how to load data from an XML file and display it within the DXGrid control. To display structured data from an XML file, do the following:
                        </Paragraph>
                        <Paragraph>
                        1. Bind the grid to an ordered collection of XML nodes.
                        </Paragraph>
                        <Paragraph>
                        2. Create required grid columns and bind them to the corresponding node properties using the <Bold>Binding</Bold> property.
                        </Paragraph>
                        <Paragraph>
                        3. Specify column captions using the <Bold>Header</Bold> property.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ExcelItemsSource",
					displayName: @"Excel Items Source",
					group: "Data Binding",
					type: "GridDemo.ExcelItemsSource",
					shortDescription: @"Demonstrates a grid bound to an Excel Items Source.",
					description: @"
                        <Paragraph>
                        Starting with v15.2, the DevExpress WPF Grid Control supports the Excel Source. This demo shows how to use the ExcelItemsSource object to bind a grid to data contained in an XLS file. To display data from an XLS file, do the following:
                        </Paragraph>
                        <Paragraph>
                        1. Create an ExcelItemsSource object, specify the document format using the StreamDocumentFormat property and set the path to the document file using the FileUri property.
                        </Paragraph>
                        <Paragraph>
                        2. Create the required ExcelColumn objects, bind them to the corresponding data properties and specify their type via Name and ColumnType properties.
                        </Paragraph>
                        <Paragraph>
                        3. Bind the grid to the ExcelItemsSoure object.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "LargeDataSource",
					displayName: @"Large Data Source",
					group: "Performance",
					type: "GridDemo.LargeDataSource",
					shortDescription: @"With multi-directional UI virtualization our grid introduces high performance when operating with hundred columns and million rows.",
					description: @"
                        <Paragraph>
                        Like other grid controls, the DXGrid for WPF ships with numerous ""must-have"", ""make-your-life-easy"" and ""look-what-I-can-do"" features. When push comes to shove, however, these features take second place to the grid's ability to manage extremely large datasets. Not only must the grid be able to display large datasets effectively, it must also execute common data shaping operations such as scrolling, grouping, sorting, and filtering quickly.
                        </Paragraph>
                        <Paragraph>
                        To see how the DXGrid successfully manages large datasets, choose the desired number of <Bold>Rows</Bold> (up to ten million) and <Bold>Columns</Bold> (up to ten thousand) in the demo options section and then click <Bold>Apply</Bold>. Once the data has been generated, you can start testing the grid and its performance.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Note:</Bold> The data in the DXGrid control in this demo is fully editable. You can modify cell values using the various editors we ship, such as our text edit, check box, mask edit, etc.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "RealtimeDataSource",
					displayName: @"Real-Time Data Processing",
					group: "Performance",
					type: "GridDemo.RealtimeDataSource",
					shortDescription: @"With our data grid you can develop high performance and data intensive applications for real-time data processing.",
					description: @"
                        <Paragraph>
                        This demo shows a grid with the RealTimeSource control. This control is bound to the data source that changes very rapidly (e.g. 10 000 changes per second). However, the grid always stays responsive. You can speed up or slow down the data source changes imitation via the buttons above.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "AdvancedPrintingOptions",
					displayName: @"Printing Options",
					group: "Printing and Exporting",
					type: "GridDemo.AdvancedPrintingOptions",
					shortDescription: @"The DevExpress WPF Printing Library allows you to instantly render and print the DXGrid.",
					description: @"
                        <Paragraph>
                        This demo illustrates the flexibility and customization options of the DevExpress Printing Library when used to render the contents of the DXGrid (whether you print to paper or export to multiple file formats).
                        </Paragraph>
                        <Paragraph>
                        You can instantly define the visibility of column headers and total summaries in the resulting report. You can choose whether to automatically adjust the grid’s layout before it is printed (expand all its groups and adjust column widths) or to print the grid as-is.
                        </Paragraph>
                        <Paragraph>
                        To view the rendered contents of the DXGrid, click the <Bold>New Tab Preview</Bold> or <Bold>New Window Preview</Bold> button. The Preview window allows you to both print the document and export its contents to multiple file formats (e.g. PDF, XLS, XPS, etc).
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Printing and Exporting",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6160"),
						new WpfModuleLink(
							title: "Printing",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Printing&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "CardViewPrinting",
					displayName: @"CardView Printing",
					group: "Printing and Exporting",
					type: "GridDemo.CardViewPrinting",
					shortDescription: @"This demo illustrates the print output of the CardView.",
					description: @"
                        <Paragraph>
                            The Options Sidebar contains a number of printing options. You can enable the auto width feature and specify the number of cards in a row, select whether to expand all groups, print summaries and unselected rows.
                        </Paragraph>
                        <Paragraph>
                            Templates can be used to customize the print output. Switch the Print Style to Uniform Cards Size to see it in action.
                        </Paragraph>
                        <Paragraph>
                            To view the rendered contents of the CardView, click the New Tab Preview or New Window Preview button. The Preview windows allows you to both print the document and export its contents to multiple file formats (e.g. PDF, XLS, XPS, etc).
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.V152,
					links: new[] {
						new WpfModuleLink(
							title: "Printing and Exporting",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6160"),
						new WpfModuleLink(
							title: "Printing",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Printing&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "UsingPrintingTemplates",
					displayName: @"Printing Templates",
					group: "Printing and Exporting",
					type: "GridDemo.UsingPrintingTemplates",
					shortDescription: @"The DevExpress Printing Library allows you to customize templates used to render DXGrid elements.",
					description: @"
                        <Paragraph>
                        This demo helps illustrate the power of predefined templates to customize the print output of the DXGrid control. In this options pane, you can specify one of the following templates:
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Default.</Bold> The grid is printed using a default table-like template.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Detail.</Bold> Each grid row is represented by a card with detailed information.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Mail Merge.</Bold> Each grid row is represented in the form of free text with injected data values.
                        </Paragraph>
                        <Paragraph>
                        To view the rendered contents of the DXGrid, click the New Tab Preview or New Window Preview button. The Preview window allows you to both print the document and export its contents to multiple file formats (e.g. PDF, XLS, XPS, etc).
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PrintingMasterDetail",
					displayName: @"Print & Export",
					group: "Master-Detail",
					type: "GridDemo.PrintingMasterDetail",
					shortDescription: @"The DXGrid control provides a fast and flexible way to print and export master-detail contents.",
					description: @"
                        <Paragraph>
                        The DXGrid control provides a fast and flexible way to bring master-detail contents to a printed page, or export the data to a file or stream in various formats - PDF, RTF, XLS, etc.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can do the following:
                        </Paragraph>
                        <Paragraph>
                        -	Specify whether or not to print detail data via the Allow Print Details option.
                        </Paragraph>
                        <Paragraph>
                        -	Specify whether or not to print column headers and total summaries for each detail grid.
                        </Paragraph>
                        <Paragraph>
                        -	Specify whether or not to print only selected rows. Note that the master-detail grid now supports the multi-selection feature.
                        </Paragraph>
                        <Paragraph>
                        -	Click the Preview button to show the Preview window allowing you to customize and print the document, export the grid to multiple file formats (e.g., PDF, XLS, XPS, etc.).
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Printing and Exporting",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6160"),
						new WpfModuleLink(
							title: "Printing",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Printing&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "Grouping",
					displayName: @"Grouping",
					group: "Data Shaping",
					type: "GridDemo.Grouping",
					shortDescription: @"DXGrid supports data grouping against an unlimited number of columns. Fully customizable and interactive UI.",
					description: @"
                        <Paragraph>
                        Both the Table View and the Card View fully support unlimited data grouping. When grouping data, you can include as many columns as your business needs dictate.
                        </Paragraph>
                        <Paragraph>
                        In this demo, apply predefined grouping via the <Bold>Group By</Bold> list, or set it to None and manually drag-and-drop column headers to group by them.
                        </Paragraph>
                        <Paragraph>
                        Enable the <Bold>Allow Fixed Groups</Bold> option to activate a scrolling mode that is perfect when viewing large amounts of grouped data. The top row for the visible group will always be displayed as you scroll through grouped data. The anchored group row(s) is shadowed to indicate that the grouped data is only partially displayed.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Custom Data Grouping",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridDataGrouping;DXGrid+for+WPF.product;3"),
						new WpfModuleLink(
							title: "Grouping Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6151"),
						new WpfModuleLink(
							title: "Grouping",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Grouping&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "MultiValueGrouping",
					displayName: @"Multi-Value Grouping",
					group: "Data Shaping",
					type: "GridDemo.MultiValueGrouping",
					shortDescription: @"This demo emulates the MS Outlook grouping feature, where tasks can belong to multiple categories.",
					description: @"
                        <Paragraph>
                        In this demo, tasks shown in the grid can belong to multiple categories. When data is grouped by the <Bold>Category</Bold> column, such tasks will be displayed in multiple groups at the same time.
                        </Paragraph>
                        <Paragraph>
                        On the Options panel of this demo, you can toggle the ""Ungroup by 'Category'""/""Group by 'Category'"" button to see the raw data and the result of multiple grouping.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> Try to edit the values for a task that is shown in multiple categories. You'll see that your changes are simultaneously applied to all the grid rows that represent this task.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "IntervalGrouping",
					displayName: @"Interval Grouping",
					group: "Data Shaping",
					type: "GridDemo.IntervalGrouping",
					shortDescription: @"This demo shows how to group data by text, numeric and date intervals.",
					description: @"
                        <Paragraph>
                        This demo illustrates the ""Group Intervals"" feature of the DXGrid control.
                        </Paragraph>
                        <Paragraph>
                        With DXGrid, it is possible to specify, for each column, how data rows should be combined when grouping is applied against this column: by its value (default), by a specific group internal, or by a custom rule.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Alphabetical</Bold> - Enables grouping by the first letter of the <Bold>Country</Bold> field;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Date: Month</Bold> - Enables grouping by the month of the <Bold>Order Date</Bold> field;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Date: Year</Bold> - Enables grouping by the year of the <Bold>Order Date</Bold> field;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Date: Range</Bold> - Enables grouping by the <Bold>Order Date</Bold> field using built-in date-time ranges, as in Microsoft Outlook;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Custom Interval</Bold> - Enables grouping by the <Bold>Unit Price</Bold> field using custom intervals.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Custom Data Grouping",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridDataGrouping;DXGrid+for+WPF.product;3"),
						new WpfModuleLink(
							title: "Silverlight and WPF Grid – Group Intervals for DateTime Columns in Server Mode",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/05/10/silverlight-and-wpf-grid-group-intervals-for-datetime-columns-in-server-mode-coming-v2011-1.aspx"),
						new WpfModuleLink(
							title: "Group Modes and Custom Grouping",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6139"),
						new WpfModuleLink(
							title: "Group Intervals",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Group+Intervals&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "DataGroupSummaries",
					displayName: @"Data Group Summaries",
					group: "Data Shaping",
					type: "GridDemo.DataGroupSummaries",
					shortDescription: @"Summaries can be calculated against column values within groups, and displayed within group rows and group footers.",
					description: @"
                        <Paragraph>
                        Calculating summaries for groups is a ""must-have"" feature for any data grid control.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can see how to show a group summary and customize its appearance via templates. The following templates are available:
                        </Paragraph>
                        <Paragraph>
                        • A customizable group summary template that allows a user to change a summary type and a summary field at runtime (this is implemented completely in XAML).
                        </Paragraph>
                        <Paragraph>
                        • A conditional summary template that uses the default group summary template and highlights sums larger than 10000 in red and less than 5000 in green (it is implemented via DataTemplateSelector).
                        </Paragraph>
                        <Paragraph>
                        • A custom summary template that shows an alternate way of representing a group summary.
                        </Paragraph>
                        <Paragraph>
                        • The default mode that shows the standard DXGrid group summary appearance.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Display Group Summaries",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridGroupSummaries;DXGrid+for+WPF.product;2"),
						new WpfModuleLink(
							title: "Group Summary",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6127"),
						new WpfModuleLink(
							title: "Group Summary",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Group+Summary&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "AlignGroupSummariesByColumns",
					displayName: @"Align Group Summaries to Columns",
					group: "Data Shaping",
					type: "GridDemo.AlignGroupSummariesByColumns",
					shortDescription: @"This demo shows how to align data group summaries to columns.",
					description: @"
                        <Paragraph>
                        In this demo, data group summaries are displayed under corresponding columns and are separated by vertical lines. To activate automatic group summary alignment, set the GroupSummaryDisplayMode property to 'AlignByColumns'.
                        </Paragraph>
                        <Paragraph>
                        To customize group summaries at runtime, right-click the grouping column's header and select 'Group Summary Editor...'
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "GridSummaries",
					displayName: @"Total Summaries",
					group: "Data Shaping",
					type: "GridDemo.GridSummaries",
					shortDescription: @"Grid summaries can be calculated against all values within a column, and displayed inside the footer.",
					description: @"
                        <Paragraph>
                        Calculating total summaries for data columns is a ""must-have"" feature for any data grid control.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can hide and show summary panels (footers) using the <Bold>Show Summary Panel</Bold> and <Bold>Show Fixed Summary Panel</Bold> options. Total summaries can be created and customized using the runtime summary editors (use the corresponding button within the ‘Options’ panel).
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> You can click on any column header, and sort grid data by this column's values. When you click on the column header for the first time, data is sorted in ascending order. Clicking on it a second time sorts the data in descending order. Further clicks switches the sort order back and forth.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Display Total Summaries",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridTotalSummaries;DXGrid+for+WPF.product;2"),
						new WpfModuleLink(
							title: "Total Summary",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6128"),
						new WpfModuleLink(
							title: "Total Summary",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Total+Summary&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "SortBySummary",
					displayName: @"Sort by Summary",
					group: "Data Shaping",
					type: "GridDemo.SortBySummary",
					shortDescription: @"Group rows can be sorted by their summary values.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to sort groups by summary in a certain column using the DXGrid control.
                        </Paragraph>
                        <Paragraph>
                        Since this feature makes sense only when grouping is enabled (because it sorts groups, and if there are no groups, there is nothing to sort), all data is always grouped by the <Bold>Order Date</Bold> column in this demo, and you can't change this grouping.
                        </Paragraph>
                        <Paragraph>
                        However, in the demo options, you can select a sorting rule, specifying whether grid rows should be sorted by the summary of the <Bold>Unit Price</Bold> or the <Bold>Order Sum</Bold> column, as well as the sort order ( <Bold>Ascending</Bold> or <Bold>Descending</Bold> ).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ExpandCollapseGroups",
					displayName: @"Expand/Collapse Groups",
					group: "Data Shaping",
					type: "GridDemo.ExpandCollapseGroups",
					shortDescription: @"This demo shows how to customize the animation of group rows when they are expanded or collapsed.",
					description: @"
                        <Paragraph>
                        Note that in DXGrid we've implemented a hierarchical visual tree with full virtualization support. This approach allows you to not only draw individual elements such as rounded corners bordering group contents but also to create custom expand/collapse animations. This is impossible with a simple linear visual tree where group rows are at the same level as data rows.
                        </Paragraph>
                        <Paragraph>
                        In this demo you can see some different animation effects.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Fade In</Bold> - A group is expanded immediately, then fades in;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Cascade Fade In</Bold> - A group is expanded with an animation; rows fading in as they appear.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Blinds</Bold> - A group is expanded immediately and then all rows are scaled from 0 to 1;
                        </Paragraph>
                        <Paragraph>
                        <Bold>• Cascade Blinds</Bold> - A group is expanded with an animation; rows scaling from 0 to 1 as they appear.
                        </Paragraph>
                        <Paragraph>
                        <Bold>• None</Bold> - Immediate expand/collapse without any animation.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "GridSearchPanel",
					displayName: @"Search Panel",
					group: "Filtering",
					type: "GridDemo.GridSearchPanel",
					shortDescription: @"The built-in Search Panel delivers an easy way for end-users to locate information within the grid.",
					description: @"
                        <Paragraph>
                        The Search Panel is available within all Grid Control's Views. The Search Panel can be enabled at any time by using the CTRL+F shortcut. To execute a search, enter text within the Search box and the grid will display those records that have matching values.
                        </Paragraph>
                        <Paragraph>
                        Various options allow you to control the display and behavior of the Search Panel, specify search columns, choose between automatic and manual search modes, allow search results to be highlighted within located records, etc.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "Filtering",
					displayName: @"Data Filtering",
					group: "Filtering",
					type: "GridDemo.Filtering",
					shortDescription: @"The DevExpress WPF Grid provides numerous filtering options to simplify data management.",
					description: @"
                        <Paragraph>
                        The DevExpress WPF Grid provides a rich collection of data filtering options to you and your end-users – making it easy and straightforward to locate data within the grid control.
                        </Paragraph>
                        <Paragraph>
                        To enable filtering, the <Bold>Allow Filtering</Bold> option (which is bound to the View.AllowColumnFiltering property) must be checked. Once enabled, a unique “filter” icon is displayed when the mouse is hovered over a given grid column header. By default, the DXGrid control uses a <Bold>Popup List Box</Bold> as its predefined column filter editor (see the Region, City, Order Date, and Unit Price columns). As an alternative, you can change this filter editor to a Popup Checked List Box – allowing you to specify multiple search values simultaneously (see the Country column).
                        </Paragraph>
                        <Paragraph>
                        With the use of templates, you can implement a wide range of custom filter controls. For example, the Quantity column uses a track bar control to specify the minimum allowed value for its filter criteria.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,	
					links: new[] {
						new WpfModuleLink(
							title: "Filtering Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6130"),
						new WpfModuleLink(
							title: "Filtering",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Filtering&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "AutoFilterRow",
					displayName: @"Auto-Filter Row",
					group: "Filtering",
					type: "GridDemo.AutoFilterRow",
					shortDescription: @"The automatic filter row allows end-users to filter data on the fly, by typing text directly into the row.",
					description: @"
                        <Paragraph>
                        This demo illustrates the ""Auto Filter Row"" feature of the DXGrid control.
                        </Paragraph>
                        <Paragraph>
                        This feature is represented by an additional grid row, which has empty cells by default, and which is displayed at the top of the grid above all other rows. The main purpose of this row is to provide a quick and visual way to filter grid data. To see this feature in action, enter any value into any cell of this row, and note that the grid refreshes to contain only the rows that match the entered value in the corresponding column.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Note:</Bold> You may notice that some columns ( <Bold>Id, Priority, From, Hours</Bold> ) are filtered using the LIKE comparison operator, while the others ( <Bold>To, Sent, Has Attachment</Bold> ) are filtered using the EQUALS operator.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					updatedIn: KnownDXVersion.V152,
					links: new[] {
						new WpfModuleLink(
							title: "Automatic Filter Row",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6132"),
						new WpfModuleLink(
							title: "AutoFilterRow",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=AutoFilterRow&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "FilterControl",
					displayName: @"Filter Control",
					group: "Filtering",
					type: "GridDemo.FilterControl",
					shortDescription: @"The Filter Control allows end-users to build complex filter criteria.",
					description: @"
                        <Paragraph>
                        End-users can open the <Bold>Filter Control</Bold> by clicking the <Bold>Edit Filter</Bold> button displayed within the Filter Panel or via a column’s context menu.
                        </Paragraph>
                        <Paragraph>
                        In this demo, a standalone <Bold>Filter Control</Bold> is displayed above the grid, allowing you to create filter criteria and apply them to the grid without invoking the grid’s built-in <Bold>Filter Editor</Bold> .
                        </Paragraph>
                        <Paragraph>
                        <Bold>Add/Remove Conditions</Bold> <LineBreak/> To create and customize filter criteria, use the <InlineUIContainer> <Image Stretch=""None""  xmlns:ddd=""clr-namespace:DevExpress.DemoData;assembly=DevExpress.DemoData.v14.2.Core"" ddd:DemoImage.Path=""Products/DXGrid/MainDemo/FilterControl/add.png""  /> </InlineUIContainer> and <InlineUIContainer> <Image Stretch=""None""  xmlns:ddd=""clr-namespace:DevExpress.DemoData;assembly=DevExpress.DemoData.v14.2.Core"" ddd:DemoImage.Path=""Products/DXGrid/MainDemo/FilterControl/delete.png""  /> </InlineUIContainer> buttons.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Change a Column in a Filter Condition</Bold> <LineBreak/> To change a condition's column, invoke the column list and choose the required column.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Change an Operator in a Filter Condition</Bold> <LineBreak/> To change a condition's operator, invoke the operator list and choose the required operator.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Edit a Condition's Value</Bold> <LineBreak/> To edit a condition's value, click the operand value and type text. To discard changes to the value and close the active edit box, press ESC.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Filter Editor",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridFilterEditor;DXGrid+for+WPF.product;2"),
						new WpfModuleLink(
							title: "Filter Editor",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7788")
					}
				),
				new WpfModule(demo,
					name: "CellEditors",
					displayName: @"Cell Editors",
					group: "Editing",
					type: "GridDemo.CellEditors",
					shortDescription: @"Our Data Editors Library provides controls that can be embedded into grid cells for editing.",
					description: @"
                        <Paragraph>
                        This demo illustrates how DXGrid uses a set of specific editors to provide data editing capabilities. To compare editable and read-only grid modes, toggle the <Bold>Allow Editing</Bold> option.
                        </Paragraph>
                        <Paragraph>
                        As different user interfaces have different requirements regarding when editors and editor buttons should be displayed, the DXGrid control supports several behavior modes, which can be switched by choosing different values of the <Bold>Editor Show Mode</Bold> and <Bold>Editor Button Show Mode</Bold> options.
                        </Paragraph>
                        <Paragraph>
                        Note that the in-place editors are different for different columns, and their type depends upon the type of data displayed in a specific column. However, for the <Bold>Boolean</Bold> type, it is natural to use True/False strings or check boxes - so in this demo you can choose one of the following <Bold>Boolean Editor</Bold> options: <Bold>Check Edit</Bold> , <Bold>Text Edit</Bold> or <Bold>Combo Box Edit</Bold> .
                        </Paragraph>
                        <Paragraph>
                        Also, for a numerical data column you can change the display template to one that's similar to a ""progress bar"" (via the <Bold>Use alternative display template</Bold> option) and the edit template to one that's similar to a ""track bar"" (via the <Bold>Use alternative edit template</Bold> option).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Assign a ComboBox Editor to a Column",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridComboBoxToColumn;DXGrid+for+WPF.product;3"),
						new WpfModuleLink(
							title: "In-place Editors Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6606")
					}
				),
				new WpfModule(demo,
					name: "DataEditorTypes",
					displayName: @"Data Editor Types",
					group: "Editing",
					type: "GridDemo.DataEditorTypes",
					shortDescription: @"DevExpress Editors Library ships with over 20 controls that can be used within the grid cells for in-place data editing.",
					description: @"
                        <Paragraph>
                        This demo illustrates the power of the DevExpress WPF Grid and its rich collection of in-cell data editors.
                        </Paragraph>
                        <Paragraph>
                        The grid’s data source includes a <Bold>EditorType</Bold> field. This field contains strings that correspond to cell editor types. A <Bold>MultiEditorsTemplateSelector</Bold> object assigned to a column’s <Bold>CellTemplateSelector</Bold> returns a cell editor template based on the value in the <Bold>EditorType</Bold> column. This editor is used to edit cell values displayed within the processed row.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "NewItemRow",
					displayName: @"New Item Row",
					group: "Editing",
					type: "GridDemo.NewItemRow",
					shortDescription: @"To allow end-user to add new records, enable the New Item Row feature.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Bold>New Item Row</Bold> feature. It's a service row displayed either at the bottom or the top of the grid.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can click the New Item Row to add and initialize a new data row. Once you've done that, input a value into any cell and press ENTER to move to the next cell. After all cell values are entered and focus leaves the newly created row, it is automatically committed to the grid's datasource.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "New Item Row",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6123"),
						new WpfModuleLink(
							title: "New Item Row",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=New+Item+Row&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "CustomCardLayout",
					displayName: @"Custom Card Layout",
					group: "Editing",
					type: "GridDemo.CustomCardLayout",
					shortDescription: @"A complete set of templates allows you to provide a different look and feel of grid elements.",
					description: @"
                        <Paragraph>
                        With templates supported throughout the DevExpress DXGrid for WPF, you can design custom interfaces to satisfy your most demanding users. This demo shows how to provide custom layout of cards in a <Bold>Card View</Bold>.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "Validation",
					displayName: @"Data Validation",
					group: "Editing",
					type: "GridDemo.Validation",
					shortDescription: @"This demo illustrates data validation via DataAnnotation attributes and DevExpress Fluent API in the Grid Control.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to validate data, whether obtained from a grid's data source or entered by the end-user.
                        </Paragraph>
                        <Paragraph>
                        With DXGrid, it is possible to implement data validation on one of two levels - at the data level, or at the grid level. In this demo, data validation is performed at the data level.
                        </Paragraph>
                        <Paragraph>
                        You don't need to implement data validation for each column in the code-behind. It is done automatically when the <Bold>Grid Control</Bold> is bound to a data source that contains <Bold>DataAnnotation</Bold> attributes or provides validation attributes via <Bold>DevExpress Fluent API</Bold>.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Input Validation Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6112"),
						new WpfModuleLink(
							title: "Validation",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Validation&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "DataErrorInfo",
					displayName: @"Data Error Indication",
					group: "Editing",
					type: "GridDemo.DataErrorInfo",
					shortDescription: @"The DXGrid control supports standard and common to all DevExpress .NET components data validation mechanisms.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the <Bold>IDXDataErrorInfo</Bold> object, which is useful when it is necessary to show error marks of a different kind. This functionality is impossible when using the standard <Bold>IDataErrorInfo</Bold> objects.
                        </Paragraph>
                        <Paragraph>
                        This demo shows the following errors:
                        </Paragraph>
                        <Paragraph>
                        • The <Bold>Last Name</Bold> field can't be empty;
                        </Paragraph>
                        <Paragraph>
                        • The <Bold>Address</Bold> field must be entered;
                        </Paragraph>
                        <Paragraph>
                        • The <Bold>Email</Bold> field must be entered;
                        </Paragraph>
                        <Paragraph>
                        • The <Bold>Email</Bold> field must contain @ and have at least one character before and after it;
                        </Paragraph>
                        <Paragraph>
                        • Either <Bold>Phone Number</Bold> or <Bold>Email</Bold> fields should be specified for a row (otherwise it is marked in pink).
                        </Paragraph>
                        <Paragraph>
                        Note that in this demo all validation is performed at the data level, not at the grid level. This means that invalid data is marked by custom error signs, but you can input invalid data into any cell and then leave it.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "GridCellMultiColumnLookupEditor",
					displayName: @"Multi-Column Lookup Cell Editor",
					group: "Editing",
					type: "GridDemo.GridCellMultiColumnLookupEditor",
					shortDescription: @"This demo shows how you can use a Multi-Column Lookup Editor within the DXGrid.",
					description: @"
                        <Paragraph>
                        The DevExpress <Bold>Multi-Column Lookup Editor</Bold> allows you to display lookup values using multiple columns. This new WPF control can be used as a grid cell editor or on a standalone basis.
                        </Paragraph>
                        <Paragraph>
                        In this demo, it is embedded within the DXGrid control to simplify the editing of Employee and Customer column values. To customize the behavior of the Multi-Column Lookup Editor within the demo, use the Properties list.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Show Size Grip</Bold> – Activates a size grip so your end users can change the size of the drop down to more efficiently display the information contained within the lookup.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Immediate Popup</Bold> – When active, data entry within a lookup cell automatically activates the drop down.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Allow Auto Complete</Bold> – Allows the lookup control to automatically update cell contents based on characters entered within it.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Allow Incremental Filtering</Bold> – Restricts the lookup list to records that match characters entered within the cell.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "StandaloneMultiColumnLookupEditor",
					displayName: @"Multi-Column Lookup Standalone Editor",
					group: "Editing",
					type: "CommonDemo.StandaloneMultiColumnLookupEditor",
					shortDescription: @"The DevExpress Multi-Column Lookup Editor can be used as a standalone data editing control.",
					description: @"
                        <Paragraph>
                        The DevExpress Multi-Column Lookup Editor allows you to display lookup information in a highly efficient manner – allowing end-users to edit data without aggravation. Key features include:
                        </Paragraph>
                        <Paragraph>
                        • Display and edit values can be obtained from separate data fields (<Bold>Display Member</Bold> and <Bold>Value Member</Bold>)
                        </Paragraph>
                        <Paragraph>
                        • Automatic completion and incremental data list filtering (<Bold>Allow Auto Complete</Bold> and <Bold>Allow Incremental Filtering</Bold>)
                        </Paragraph>
                        <Paragraph>
                        • Custom processing for data values that are not in the lookup list (<Bold>Allow Processing of New Values</Bold>). A specially designed event allows you to insert new records into the underlying data source with ease.
                        </Paragraph>
                        <Paragraph>
                        • Embeddable within the DevExpress WPF Grid Control.
                        </Paragraph>
                        <Paragraph>
                        • A nearly unlimited set of options to customize the dropdown list by exploiting the features provided by the DevExpress WPF Grid control. These include customizable columns collection, data summaries, sorting, grouping and filtering dropdown list data by column values, and much more.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "VerticalScrollingOptions",
					displayName: @"Vertical Scrolling Options",
					group: "Selection and Usability",
					type: "GridDemo.VerticalScrollingOptions",
					shortDescription: @"With Per-Pixel Scrolling, grid rows can be scrolled smoothly and not on a 'record-by-record' basics.",
					description: @"
                        <Paragraph>
                        By default, when a view is vertically scrolled, the grid synchronously loads all rows that should be displayed on screen. To speed up the grid's performance, enable the <Bold>'Allow Cascade Update'</Bold> option. In this instance, visible rows will be asynchronously loaded, one by one. To provide visual feedback, the grid plays an animation while the data is being retrieved. In this demo, you can also enable/disable per-pixel scrolling, animation played when data is vertically scrolled; customize the animation settings (its duration and type).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Silverlight and WPF Grid – New Scrolling Features",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/05/02/silverlight-and-wpf-grid-new-scrolling-features-coming-in-v2011-1.aspx"),
						new WpfModuleLink(
							title: "WPF and Silverlight Data Grid – MVVM Enhancements",
							type: WpfModuleLinkType.Blogs,
							url: "http://community.devexpress.com/blogs/theprogressbar/archive/2011/07/19/wpf-and-silverlight-data-grid-mvvm-enhancements.aspx"),
						new WpfModuleLink(
							title: "Per-Pixel Vertical Scrolling",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9791"),
						new WpfModuleLink(
							title: "Cascading Data Updates",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument9787"),
						new WpfModuleLink(
							title: "Scrolling Options",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Scrolling+Options&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "CheckBoxColumn",
					displayName: @"Web Style Row Selection",
					group: "Selection and Usability",
					type: "GridDemo.CheckBoxColumn",
					shortDescription: @"A straightforward way to select multiple records via a CheckBox column.",
					description: @"
                        <Paragraph>
                        The Grid's CheckBox column allows end-users to toggle selection for rows within the grid. In this example, we've enabled checkboxes at the column header level (to select all rows), at the group level (to select rows within a group) and at the individual row level.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "DataAwareExport",
					displayName: @"Data Aware Export",
					group: "Printing and Exporting",
					type: "GridDemo.DataAwareExport",
					shortDescription: @"This demo illustrates the enhanced data export engine.",
					description: @"
                        <Paragraph>
                        The data export functionality makes use of a range of MS Excel features. The following grid features are fully supported for all Excel export operations:
                        </Paragraph>
                        <Paragraph>
                        • Data Grouping - with the ability to collapse/expand groups within a worksheet.
                        </Paragraph>
                        <Paragraph>
                        • Data Sorting and Filtering - allowing end-users to display relevant data in the desired order.
                        </Paragraph>
                        <Paragraph>
                        • Totals and Group Summaries - with the ability to modify/change formulas.
                        </Paragraph>
                        <Paragraph>
                        • Excel Style Format Rules
                        </Paragraph>
                        <Paragraph>
                        • Data Validation for lookup and combobox сolumns
                        </Paragraph>
                        <Paragraph>
                        • Fixed Columns
                        </Paragraph>",
					addedIn: KnownDXVersion.V142
				),				 
				new WpfModule(demo,
					name: "MultiRowSelection",
					displayName: @"Multiple Row Selection",
					group: "Selection and Usability",
					type: "GridDemo.MultiRowSelection",
					shortDescription: @"End-users can select multiple rows using the mouse or keyboard.",
					description: @"
                        <Paragraph>
                        DXGrid provides the capability to simultaneously select more than one grid row. When multi-selection is enabled (a grid view's <Bold>MultiSelectMode</Bold> property is set to <Bold>Row</Bold> ), a user can select multiple rows in one of the following way:
                        </Paragraph>
                        <Paragraph>
                        • by holding either the SHIFT or CTRL key, and selecting rows using the mouse pointer;
                        </Paragraph>
                        <Paragraph>
                        • by holding the SHIFT key, and selecting rows using the TOP and BOTTOM arrow keys;
                        </Paragraph>
                        <Paragraph>
                        • by holding the CTRL key, navigating to rows using the TOP and BOTTOM arrow keys and selecting some of them using the SPACE key.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Note:</Bold> Multi-selection works in the same way in both Table and Card views. To explore this, you can change the current view on the Options pane.
                        </Paragraph>
                        <Paragraph>
                        In addition, the DXGrid control provides API to programmatically add or remove specific rows in the current selection. This is implemented for specific buttons on the Options pane that can be used to select/deselect rows with a specific <Bold>Product</Bold> and <Bold>Price.</Bold>
                        </Paragraph>
                        <Paragraph>
                        You can find brief information on the currently selected row in the list at the bottom of the Options pane.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> Try to group the grid against any column, and note that group summaries and total summaries are calculated only for the currently selected records.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152,	
					links: new[] {
						new WpfModuleLink(
							title: "Multiple Row Selection",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7359")
					}
				),
				new WpfModule(demo,
					name: "MultiCellSelection",
					displayName: @"Multiple Cell Selection",
					group: "Selection and Usability",
					type: "GridDemo.MultiCellSelection",
					shortDescription: @"End-users can select individual cells or blocks of cells.",
					description: @"
                        <Paragraph>
                        In this demo, you can select contiguous blocks of cells and individual cells within different data rows by clicking them while holding the SHIFT or CTRL key down.
                        </Paragraph>
                        <Paragraph>
                        To enable the multiple cell selection, set the table view’s <Bold>MultiSelectMode</Bold> property to <Bold>Cell</Bold> and the <Bold>NavigationStyle</Bold> property should be set to <Bold>Cell</Bold> .
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,						
					links: new[] {
						new WpfModuleLink(
							title: "Multiple Cell Selection",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7556")
					}
				),
				new WpfModule(demo,
					name: "ManagingLiveData",
					displayName: @"Managing Live Data",
					group: "Selection and Usability",
					type: "GridDemo.ManagingLiveData",
					shortDescription: @"The DXGrid can be automatically updated when its underlying data is constantly changing.",
					description: @"
                        <Paragraph>
                        This demo helps illustrate the ability of the DXGrid to consume and display live data.
                        </Paragraph>
                        <Paragraph>
                        To begin, active <Bold>Allow Updating</Bold>. Once enabled, you can choose the <Bold>Update Mode</Bold> to specify whether processes should be removed, added or updated, along with the <Bold>Max Process Count</Bold> value, which is in use when processes are added and removed (this demo only simulates the addition and removal of processes).
                        </Paragraph>
                        <Paragraph>
                        You can customize the <Bold>Update Interval</Bold> and <Bold>History Update Interval</Bold>in order to specify update parameters (in milliseconds) used to update the grid. You can also activate different animation options to see how they appear on screen when grid updating.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> Note that the Process Count and Total Memory Usage summaries at the bottom of the grid are constantly recalculated to reflect the current state of the grid. You can shape the contents of the grid by scrolling, grouping and sorting regardless of the grid’s update state.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "CopyPasteOperations",
					displayName: @"Copy/Paste Operations",
					group: "Selection and Usability",
					type: "GridDemo.CopyPasteOperations",
					shortDescription: @"Data can be copied to the clipboard. Special events allow you to manually copy and paste data.",
					description: @"
                        <Paragraph>
                        If the grid view's <Bold>ClipboardCopyAllowed</Bold> property is set to <Bold>True</Bold> , it is possible to copy grid rows to the clipboard. To see how this works, select several rows from the upper grid, then copy them to the clipboard via CTRL+Insert (CTRL+C), or via the context menu, and paste them into the grid below.
                        </Paragraph>
                        <Paragraph>
                        <Bold>Hint:</Bold> You can paste grid rows information not only into another grid, but anywhere else. For example, in this demo, you can also paste information from the clipboard into the text editor. When you copy and paste grid rows into the text editor, it may be required to include or exclude the row header, which is controlled by the ""Copy Header"" option. If you copy and paste rows into another grid, it may be required to insert new rows either after the currently focused row, or after all rows.
                        </Paragraph>
                        <Paragraph>
                        In addition, the DXGrid control provides an API to programmatically copy, cut, paste and delete grid rows. This is implemented for specific buttons on the Options pane.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "HitTesting",
					displayName: @"Hit Testing",
					group: "Selection and Usability",
					type: "GridDemo.HitTesting",
					shortDescription: @"Determine which grid element is located at the specified screen coordinates when users hover or click it.",
					description: @"
                        <Paragraph>
                        This demo shows the <Bold>Hit Testing</Bold> feature, which allows you to recognize which element is located at the required location. For instance, you may have to determine which part of a View a user has clicked or double-clicked.
                        </Paragraph>
                        <Paragraph>
                        To obtain the hit information, use a view's <Bold>CalcHitInfo</Bold> method. This method accepts a test dependency object and returns the information about its position within a view.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "WPF Grid - Hit Testing",
							type: WpfModuleLinkType.Video,
							url: "http://tv.devexpress.com/#DXGridHitTesting;DXGrid+for+WPF.product;1"),
						new WpfModuleLink(
							title: "Hit Information",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument7557")
					}
				),
				new WpfModule(demo,
					name: "DragDrop",
					displayName: @"Drag and Drop",
					group: "Selection and Usability",
					type: "GridDemo.DragDrop",
					shortDescription: @"This demo shows the drag and drop functionality provided by the DXGrid control.",
					description: @"
                        <Paragraph>
                        Enabling drag and drop you allow your end-users to reorder rows in Table Views, rearrange the node hierarchy in TreeList Views and move data objects (rows or nodes) outside the grid to external controls that support drag and drop.
                        </Paragraph>
                        <Paragraph>
                        In this demo, an end-user can do the following:
                        </Paragraph>
                        <Paragraph>
                        • Rearrange data rows within the grid.
                        </Paragraph>
                        <Paragraph>
                        • Move selected data rows to a group row.
                        </Paragraph>
                        <Paragraph>
                        • Merge group rows by dragging one group row and dropping it into another group row.
                        </Paragraph>
                        <Paragraph>
                        • Hide selected rows by dragging them to the <Bold>Recycle Bin</Bold> list box.
                        </Paragraph>
                        <Paragraph>
                        • Move rows from the Recycle Bin back to the grid, etc.
                        </Paragraph>",
					allowRtl: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "BuiltInContextMenus",
					displayName: @"Built-In Context Menus",
					group: "Selection and Usability",
					type: "GridDemo.BuiltInContextMenus",
					shortDescription: @"Four types of popup menus enable an end-user to manage data, show and hide UI elements.",
					description: @"
                        <Paragraph>
                        This demo illustrates the built-in context menu of the DXGrid control. To see how this works, right-click any grid element (for example, the column header, grid row, group panel or grid footer), and explore the menu items available for different grid elements.
                        </Paragraph>
                        <Paragraph>
                        On the Options panel of this demo, you can uncheck the Enabled check box to disable a context menu for a particular grid element. In addition, you can add or remove items from given context menus, or even move items from one context menu to another. Note that this can be done completely in XAML via special customization objects.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Context Menus",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6587"),
						new WpfModuleLink(
							title: "Context Menu",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Context+Menu&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "Serialization",
					displayName: @"Save and Restore Layout",
					group: "Selection and Usability",
					type: "GridDemo.Serialization",
					shortDescription: @"This feature allows you to save and restore the grid's layout to keep the same layout.",
					description: @"
                        <Paragraph>
                        In this demo, you can customize the grid's layout (change grouping, resize and move columns, add column totals, etc.) and then click <Bold>Save Layout</Bold> to store it to the stream, and <Bold>Restore Layout</Bold> to apply the previously saved layout to the grid.
                        </Paragraph>
                        <Paragraph>
                        Also, this demo's options contain several predefined layouts (Original, Brief, Full and Banded), which demonstrate some real-life applications of the serialization feature.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Saving and Restoring Layout",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6797"),
						new WpfModuleLink(
							title: "Saving and Restoring Layout",
							type: WpfModuleLinkType.KB,
							url: "http://search.devexpress.com/?q=Saving+and+Restoring+Layout&p=T4%7cP6%7c0&d=16")
					}
				),
				new WpfModule(demo,
					name: "ClipboardFormats",
					displayName: @"Clipboard Formats",
					group: "Selection and Usability",
					type: "GridDemo.ClipboardFormats",
					shortDescription: @"Demonstrates copying the grid content to the Excel, RTF and HTML formats.",
					description: @"
                        <Paragraph>
                        The <Bold>DXGrid</Bold> allows its content to be copied to the clipboard as a formatted data.  This functionality is managed via the <Bold>View.ClipboardMode</Bold> option that is enabled in this demo.
                        </Paragraph>
                        <Paragraph> 
                        To see the copy/paste functionality for the formatted data in action, select cells and click the <Bold>Copy and Paste</Bold> button.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152					
				),
				new WpfModule(demo,
					name: "ServerModeLookUpEdit",
					displayName: @"Grid LookUp Server Mode",
					group: "Performance",
					type: "GridDemo.ServerModeLookUpEdit",
					shortDescription: @"Demonstrates the DevExpress WPF LookUpEdit operating in Server Mode.",
					description: @"
                        <Paragraph>
                        In this demo, the LookUpEdit control that is bound to a large data source operates in Server Mode. The bound data is being loaded in small portions on demand instead of being loaded into memory in its entirety. This significantly speeds up invoking the lookup's dropdown while not limiting available data operations.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "InplaceEditForm",
					displayName: @"Inline Edit Form",
					group: "Editing",
					type: "GridDemo.InplaceEditForm",
					shortDescription: @"This demo shows the Inline Data Editing Form.",
					description: @"
                        <Paragraph>
                        The Inline Edit Form option allows you to provide an alternative way to edit grid's data. In addition to the default inplace row editing, you have 3 UI options:
                        </Paragraph>
                        <Paragraph>
                        • Display the edit form below the edited row during edit operations.
                        </Paragraph>
                        <Paragraph>
                        • Hide the edited row and display the edit form between rows.
                        </Paragraph>
                        <Paragraph>
                        • Display the edit form as a dialog window.
                        </Paragraph>
                        <Paragraph>
                        To invoke the Inline Edit Form, double-click a row or select it and press Enter or F2.
                        </Paragraph>
                        <Paragraph>
                        You can switch between editing modes using the corresponding option in the Options menu.
                        </Paragraph>
                        <Paragraph>
                        If the Post Mode is set to Cached, changes made to a row in the Edit Form are only shown within the grid after posting them to the data source. You can change this by switching the Post Mode to Immediate.
                        </Paragraph>
                        <Paragraph>
                        If the Edit Form contains unsaved changes, a confirmation dialog is displayed on an attempt to move focus to another grid row. The Post Confirmation option manages whether the dialog is shown and allows you to change its type.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				)
			};
		}
	}
}
