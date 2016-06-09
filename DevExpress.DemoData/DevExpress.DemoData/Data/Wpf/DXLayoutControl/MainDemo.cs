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
		static List<Module> Create_DXLayoutControl_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "pageRealEstate",
					displayName: @"Real Estate",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageRealEstate",
					shortDescription: @"This demo shows how LayoutControl can be used to organize the presentation of a large amount of information.",
					description: @"
                        <Paragraph>
                        This demo shows how <Bold>LayoutControl</Bold> can be used to organize the presentation of a large amount of information.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "pageMortgageApplication",
					displayName: @"Mortgage Application",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageMortgageApplication",
					shortDescription: @"This demo illustrates how LayoutControl can help with the building of complex entry forms.",
					description: @"
                        <Paragraph>
                        This demo illustrates how <Bold>LayoutControl</Bold> can help with the building of complex entry forms.
                        It was built completely within the Visual Studio designer using drag and drop, the context menus for the controls, and the property inspector, with only minimal editing of the XAML.
                        </Paragraph>
                        <Paragraph>
                        Try dragging the content in order to scroll it.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "pageDashboard",
					displayName: @"Dashboard",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageDashboard",
					shortDescription: @"This demo demonstrates how TileLayoutControl can be used to build a dashboard screen.",
					description: @"
                        <Paragraph>
                        This demo demonstrates how <Bold>TileLayoutControl</Bold> can be used to build a dashboard screen.
                        There is no code-behind in this demo module, besides data definition. All UI and functionality is described in XAML.
                        </Paragraph>
                        <Paragraph>
                        Three of the tiles have live content.
                        The Listings and Agents tiles have their <Italic>ContentSource</Italic> properties bound to the List-type properties of the demo module.
                        They also define custom content data templates via the <Italic>ContentTemplate</Italic> property.
                        The Zillow tile has 2 values for its <Italic>Content</Italic> defined directly in XAML.
                        <LineBreak/>
                        All these live tiles have different <Italic>ContentChangeInterval</Italic> values that set the interval at which the automatic content switch happens.
                        </Paragraph>
                        <Paragraph>
                        Try to drag and drop tiles to customize the dashboard layout the way you prefer.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "pageEmployees",
					displayName: @"Employees",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageEmployees",
					shortDescription: @"This demo demonstrates how FlowLayoutControl and LayoutControl can be used to build an employee directory.",
					description: @"
                        <Paragraph>
                        This demo demonstrates how <Bold>FlowLayoutControl</Bold> and <Bold>LayoutControl</Bold> can be used to build an employee directory.
                        It uses the MVVM pattern to bind a model data to a view through view models.
                        </Paragraph>
                        <Paragraph>
                        <Bold>GroupBox</Bold> is used in the item template to represent an employee information card.
                        Its built-in support for the <Bold>FlowLayoutControl</Bold>'s item maximization capability allows you to zoom in to information about a specific employee.
                        The built-in animation is used during the item maximization to improve the user experience.
                        <LineBreak/>
                        In addition to the <Italic>ContentTemplate</Italic> property <Bold>GroupBox</Bold> also provides the <Italic>MaximizedContentTemplate</Italic> property that is applied when <Bold>GroupBox</Bold> is maximized.
                        It allows you to define a more detailed layout when <Bold>GroupBox</Bold> has more space for its content.
                        </Paragraph>
                        <Paragraph>
                        <Bold>LayoutControl</Bold> is used to build the maximized content template.
                        Its <Italic>GroupBoxStyle</Italic> property is overridden to provide a custom template that uses a ""lighter"" <Bold>GroupFrame</Bold> to represent a group box.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "pageCars",
					displayName: @"Cars",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageCars",
					shortDescription: @"This demo shows how FlowLayoutControl can be used to represent a structured list of items.",
					description: @"
                        <Paragraph>
                        This demo shows how <Bold>FlowLayoutControl</Bold> can be used to represent a structured list of items.
                        Its content was generated using the MVVM pattern from the data provided through the <Italic>ItemsSource</Italic> property.
                        Defining the object set as <Italic>ItemTemplateSelector</Italic> meant that different item templates could be provided for the Brand and Car objects.
                        </Paragraph>
                        <Paragraph>
                        Try dragging the content in order to scroll it.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "pageImageGallery",
					displayName: @"Image Gallery",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageImageGallery",
					shortDescription: @"This example demonstrates the customization capabilities of FlowLayoutControl (item reordering, resizing, and maximization).",
					description: @"
                        <Paragraph>
                        This example demonstrates the customization capabilities of <Bold>FlowLayoutControl</Bold> (item reordering, resizing, and maximization).
                        Animation is used for item maximization and drag and drop to provide a better user experience.
                        </Paragraph>
                        <Paragraph>
                        The content of this demo was generated using the MVVM pattern: the view model data is provided via the <Italic>ItemsSource</Italic> property and the item view is defined by the <Italic>ItemTemplate</Italic> property.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "pageProducts",
					displayName: @"Products",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageProducts",
					shortDescription: @"This demo uses DockLayoutControls and FlowLayoutControls to show a complex information chart.",
					description: @"
                        <Paragraph>
                        This demo uses <Bold>DockLayoutControl</Bold>s and <Bold>FlowLayoutControl</Bold>s to show a complex information chart.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				 new WpfModule(demo,
					name: "pageFilteringUI",
					displayName: @"Filtering UI",
					group: "Demos",
					type: "DevExpress.Xpf.LayoutControlDemo.pageFilteringUI",
					shortDescription: @"Provides user-friendly customizable filtering interface.",
					description: @"
                        <Paragraph>
                        Filtering UI is an intuitive easy-implementable tool to filter contents of a data-bound control. End-users can set filter criteria using editors that are generated based on attributes that are applied to the data source properties.
                        </Paragraph>
                        <Paragraph>
                        In this demo, you can switch between two Filtering UI Modes:
                        <LineBreak/>
                        <Bold>Default</Bold> - Filtering UI out of the box.
                        <LineBreak/>
                        <Bold>Custom</Bold> - Filtering UI customized via templates.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "pageLayoutControl",
					displayName: @"LayoutControl",
					group: "Controls",
					type: "DevExpress.Xpf.LayoutControlDemo.pageLayoutControl",
					shortDescription: @"In this demo you can experiment with the features and options available in LayoutControl.",
					description: @"
                        <Paragraph>
                        <Bold>LayoutControl</Bold> allows you to generate highly complex layouts with ease and minimum effort yet maintain the flexibility so very important in today's most effective applications.
                        </Paragraph>
                        <Paragraph>
                        The concept of <Bold>LayoutControl</Bold> is simple: it uses LayoutGroups that arrange items horizontally or vertically and can also contain other LayoutGroups with different orientation.
                        This architecture allows you to build complex and flexible layouts using simple elements.
                        Best of all, the <Bold>LayoutControl</Bold> will generate and delete these LayoutGroups based on drag and drop of items inside it, so you don't have to worry about layout structure and can focus on the visual layout you want to create.
                        </Paragraph>
                        <Paragraph>
                        LayoutGroups can be represented visually as lookless (invisible) containers, tab controls or group boxes.
                        In the latter case a LayoutGroup can be collapsed using a collapse button located in its header.
                        <LineBreak/>
                        A control with a label can be represented using a LayoutItem - a special control created for this specific purpose.
                        A <Bold>LayoutControl</Bold> provides tight integration with LayoutItems: the labels of these items can be aligned with each other inside one LayoutGroup or across the boundaries of several LayoutGroups as long as these items have the same horizontal offset.
                        </Paragraph>
                        <Paragraph>
                        In addition to drag and drop, the <Bold>LayoutControl</Bold> provides re-sizing capabilities for its items and layout groups and the ability to change horizontal and vertical alignment of any item.
                        Headers of layout groups and labels of layout items can be modified as well.
                        You can allow your end-users to remove items from the layout control.
                        Items that have been removed can be restored via drag and drop at a later time.
                        New groups (presented visually as group boxes or tabs) can also be created via drag and drop or with the help of the customization toolbar.
                        User-defined groups that are not needed anymore can be deleted from the available items list.
                        <LineBreak/>
                        This customization functionality is available for both developers and their end-users.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "pageDataLayoutControl",
					displayName: @"DataLayoutControl",
					group: "Controls",
					type: "DevExpress.Xpf.LayoutControlDemo.pageDataLayoutControl",
					shortDescription: @"In this demo you will learn how to automatically generate UI using Data Annotation attributes or DevExpress Fluent API.",
					description: @"
                        <Paragraph>
                        The <Bold>DataLayoutControl</Bold> automatically generates a user interface (UI) based on the object assigned to its <Italic>CurrentItem</Italic> property.
                        </Paragraph>
                        <Paragraph>
                        The generation engine uses the information about object's properties, their types and associated Data Annotation attributes or DevExpress Fluent API modifications to create content and layout.
                        <LineBreak/>
                        Data Annotation attributes and DevExpress Fluent API modifications deliver a way to specify configurations in the underlying data source.
                        These configurations are recognized by the DevExpress <Bold>DataLayoutControl</Bold> as well as other DevExpress WPF Controls and can be used to automatically generate the layout and configure data editor
                        <LineBreak/>
                        The representation and editing of the object's properties is implemented using DXEditors.
                        <LineBreak/>
                        You can customize the generated UI by catching and processing events.
                        Events are fired on the creation of each item or group and after the entire UI generation is completed.
                        </Paragraph>
                        <Paragraph>
                        <Bold>DataLayoutControl</Bold> is a subclass of LayoutControl and so allows you to use all of its functionality.
                        This includes the ability to create complex layouts using nested groups of different types.
                        End-user customization is also available.
                        </Paragraph>
                        <Paragraph>
                        You can define the location of the generated items: the control itself (by default) or its <Italic>AvailableItems</Italic> collection.
                        The latter case is useful when you want <Bold>DataLayoutControl</Bold> to just generate items and allow your end-users to build the layout themselves by using the generated items.
                        </Paragraph>
                        <Paragraph>
                        You can explore the functionality of the LayoutControl by using its demo - this demo shows off the specific features of the <Bold>DataLayoutControl</Bold>.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "LayoutControl",
							type: WpfModuleLinkType.Demos,
							url: "local:LayoutControl")
					}
				),
				new WpfModule(demo,
					name: "pageFlowLayoutControl",
					displayName: @"FlowLayoutControl",
					group: "Controls",
					type: "DevExpress.Xpf.LayoutControlDemo.pageFlowLayoutControl",
					shortDescription: @"In this demo you can experiment with the features and options available in FlowLayoutControl.",
					description: @"
                        <Paragraph>
                        The <Bold>FlowLayoutControl</Bold> arranges its items across horizontal or vertical layers.
                        The beginning of a new layer is defined by the <Italic>IsFlowBreak</Italic> attached property and the <Italic>BreakFlowToFit</Italic> property allows you to place an item on a new layer if it does not fit within the current layer.
                        <LineBreak/>
                        Any item can be maximized to occupy as much space as needed in order to display its contents fully.
                        In these instances, the remaining items are placed in one layer across the side of the layout control.
                        <LineBreak/>
                        All items can be stretched to occupy the maximum possible layer width.
                        In this scenario, they will be displayed in one layer.
                        </Paragraph>
                        <Paragraph>
                        The <Bold>FlowLayoutControl</Bold> provides a way to change an item's position using drag and drop.
                        The end-user can also add new flow breaks and change the existing ones.
                        Animation can be turned on for dragging and dropping items to provide a better user experience.
                        <LineBreak/>
                        Layers of individual items can be resized using built-in layer separators and the position of the maximized item can be changed using drag and drop as well.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "pageTileLayoutControl",
					displayName: @"TileLayoutControl",
					group: "Controls",
					type: "DevExpress.Xpf.LayoutControlDemo.pageTileLayoutControl",
					shortDescription: @"In this demo you can experiment with the features and options available in TileLayoutControl.",
					description: @"
                        <Paragraph>
                        <Bold>TileLayoutControl</Bold> can help you to create a UI similar to the Start screen of Windows 8.
                        In addition to being used as an application start screen, it can be applied to different dashboard scenarios.
                        </Paragraph>
                        <Paragraph>
                        <Bold>TileLayoutControl</Bold> is based on FlowLayoutControl which provides most of its functionality.
                        But there are some unique features that you can find only in <Bold>TileLayoutControl</Bold>.
                        It also overrides some aspects of the FlowLayoutControl's default behavior.
                        <Bold>TileLayoutControl</Bold> has item drag and drop and its animation turned on by default, it has different default padding and offsets between items and layers of items.
                        It does not support layer separators and layer resizing. Also its items cannot be stretched.
                        </Paragraph>
                        <Paragraph>
                        You can put any controls inside <Bold>TileLayoutControl</Bold>, but there is a special one, created specifically to be used inside this container - the <Bold>Tile</Bold> control.
                        <LineBreak/>
                        This control can hold any object as its content and the content source can be static or dynamic.
                        The dynamic content source is provided through the <Italic>ContentSource</Italic> property that accepts any IEnumerable-based source.
                        This source can be a collection of objects or an iterator that provides content on request.
                        The <Bold>Tile</Bold> control iterates through all content values automatically if you provide a dynamic content source.
                        In this case you can control the interval at which the content is switched.
                        The change of content is animated by default.
                        <LineBreak/>
                        You can set <Bold>Tile</Bold>'s <Italic>Header</Italic> to provide a description and <Italic>Size</Italic> to define its size: Extra Small, Small, Large or Extra Large.
                        Large <Bold>Tile</Bold> completely occupies an item slot. <Bold>Tile</Bold> with the ExtraLarge <Italic>Size</Italic> has a double height of a Large <Bold>Tile</Bold>.
                        <Bold>TileLayoutControl</Bold> can put 2 <Bold>Tile</Bold>s in one item slot if they have their <Italic>Size</Italic> property set to Small.
                        If the <Italic>Size</Italic> property has a value of ExtraSmall, then 4 of such <Bold>Tile</Bold>s can share half of a slot.
                        <LineBreak/>
                        When the FlowLayoutControl's attached property <Italic>IsFlowBreak</Italic> is applied to a tile it defines a hard flow break - the beginning of a new visual group of tiles.
                        You can provide a header for such a group using the <Bold>TileLayoutControl</Bold>'s <Italic>GroupHeader</Italic> attached property set on the first tile in a group.
                        <LineBreak/>
                        You can handle the user clicking on a tile with the help of <Bold>Tile</Bold>.<Italic>Click</Italic> and <Bold>TileLayoutControl</Bold>.<Italic>TileClick</Italic> events.
                        If you prefer to process actions in your view model, <Bold>Tile</Bold>.<Italic>Command</Italic> and <Bold>TileLayoutControl</Bold>.<Italic>TileClickCommand</Italic> are available to handle the click in the MVVM manner.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,	
					links: new[] {
						new WpfModuleLink(
							title: "FlowLayoutControl",
							type: WpfModuleLinkType.Demos,
							url: "local:FlowLayoutControl")
					}
				),
				new WpfModule(demo,
					name: "pageDockLayoutControl",
					displayName: @"DockLayoutControl",
					group: "Controls",
					type: "DevExpress.Xpf.LayoutControlDemo.pageDockLayoutControl",
					shortDescription: @"In this demo you can experiment with the features and options available in DockLayoutControl.",
					description: @"
                        <Paragraph>
                        The <Bold>DockLayoutControl</Bold> allows items to be docked to its sides or to occupy space not used by other docked items.
                        It is a ""lite"" version of the more powerful LayoutControl, which can be useful in simple layout scenarios.
                        Just like the LayoutControl, it provides built-in re-sizing capabilities for all of its items.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
