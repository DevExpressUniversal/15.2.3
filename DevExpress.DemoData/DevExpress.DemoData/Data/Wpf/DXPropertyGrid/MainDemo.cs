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
		static List<Module> Create_DXPropertyGrid_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "SelectedItemEditingModule",
					displayName: @"Selected Item Editing",
					group: "Demos",
					type: "PropertyGridDemo.SelectedItemEditingModule",
					shortDescription: @"In this demo, the Property Grid is used to edit cell values in the focused grid row.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">Property Grid</Span> displays properties of the item currently selected within the grid and allows you to modify their values. The changes made are automatically applied to the selected item.
                        </Paragraph>
                        <Paragraph>
                        You can select multiple grid rows and modify their properties simultaneously. A property whose value differs between the selected rows has a blank value in the property grid.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "DataEditorsModule",
					displayName: @"Data Editors",
					group: "Demos",
					type: "PropertyGridDemo.DataEditorsModule",
					shortDescription: @"A powerful collection of data edit controls can be used within a Property Grid control for data editing and presentation.",
					description: @"
                        <Paragraph>
                        Our <Span FontWeight=""Bold"">WPF Data Editors Library</Span> provides a collection of controls that can be embedded into grid cells for editing and better data presentation. From masked data input to built-in data validation, the <Span FontWeight=""Bold"">WPF Data Editors Library</Span> offers powerful data editing options.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ObjectInspector",
					displayName: @"Object Inspector",
					group: "Tutorials",
					type: "PropertyGridDemo.ObjectInspector",
					shortDescription: @"This demos shows the Property Grid's default behavior.",
					description: @"
                        <Paragraph>
                        In this demo, the <Span FontWeight=""Bold"">Property Grid</Span> browsers properties of the SimpleEditingViewModel object specified using the <Span FontWeight=""Bold"">SelectedObject</Span> property. The <Span FontWeight=""Bold"">Property Grid</Span> automatically creates data editors used to display and edit properties of the selected object. The built-in Search Panel is displayed by default. To execute a search via the Search Panel, enter the text within the <Span FontWeight=""Bold"">Search</Span> box and the <Span FontWeight=""Bold"">Property Grid</Span> will display matching properties.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PropertyAttributesModule",
					displayName: @"Property Attributes",
					group: "Tutorials",
					type: "PropertyGridDemo.PropertyAttributesModule",
					shortDescription: @"The Property Grid control can recognize property attributes.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">Property Grid</Span> recognizes property attributes and displays them instead of corresponding properties. If properties have a category attribute, the <Span FontWeight=""Bold"">Property Grid</Span> control displays these properties within the corresponding group.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CategoryAttributesModule",
					displayName: @"Category Attributes",
					group: "Tutorials",
					type: "PropertyGridDemo.CategoryAttributesModule",
					shortDescription: @"The Property Grid control can recognize category attributes.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">Property Grid</Span> recognizes property attributes and displays them instead of corresponding properties. If properties have a category attribute, the <Span FontWeight=""Bold"">Property Grid</Span> control displays these properties within the corresponding group.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DataAnnotationAttributesModule",
					displayName: @"DataAnnotation Attributes",
					group: "Tutorials",
					type: "PropertyGridDemo.DataAnnotationAttributesModule",
					shortDescription: @"The Property Grid control can recognise DataAnnotation attributes.",
					description: @"
                        <Paragraph>
                        Manage how the <Span FontWeight=""Bold"">Property Grid</Span> displays properties via DataAnnotation attributes. Use these attributes to set display names for properties, groups within which to display the properties, data types that affect data edit controls, etc.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DataAnnotationAttributesFluentAPIModule",
					displayName: @"DataAnnotation Attributes (Fluent API)",
					group: "Tutorials",
					type: "PropertyGridDemo.DataAnnotationAttributesFluentAPIModule",
					shortDescription: @"The Property Grid control can recognize category attributes.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">Property Grid</Span> recognizes property attributes and displays them instead of corresponding properties. If properties have a category attribute, the <Span FontWeight=""Bold"">Property Grid</Span> control displays these properties within the corresponding group.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "LayoutCustomizationModule",
					displayName: @"Layout Customization",
					group: "Tutorials",
					type: "PropertyGridDemo.LayoutCustomizationModule",
					shortDescription: @"Use templates to implement unique look and feel for individual PropertyGrid elements.",
					description: @"
                        <Paragraph>
                        This demo illustrates customization options available to you when using templates to create unique data presentation styles.<LineBreak/>
                        You can define a template for a header cell, content cell or an entire row.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ExpandabilityCustomizationModule",
					displayName: @"Expandability Customization",
					group: "Tutorials",
					type: "PropertyGridDemo.ExpandabilityCustomizationModule",
					shortDescription: @"This demo illustrates how to customize the expandability of a collection property.",
					description: @"
                        <Paragraph>
                        The property grid allows managing the expandability of collection properties via the <Bold>AllowExpanding</Bold> property. In this demo, the data source consists of three collection properties:
                        </Paragraph>
                        <Paragraph>
                        <Bold>Simple:</Bold> no TypeConverter is defined;<LineBreak/>
                        <Bold>Expandable:</Bold> ExpandableObjectConverter;<LineBreak/>
                        <Bold>NotExpandable:</Bold> NotExpandableConverter.
                        </Paragraph>
                        <Paragraph>
                        The following expandability modes are available:
                        </Paragraph>
                        <Paragraph>
                        • Allow the end-user to expand any collection property that does not have the <Bold>NotExpandableConverter</Bold> type converter.<LineBreak/>
                        • Allow the end-user to expand all collection properties.<LineBreak/>
                        • Allow the end-user to expand only collection properties with the <Bold>ExpandableObjectConverter</Bold> type converter.<LineBreak/>
                        • Restrict the end-user to expand any collection property.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CollectionEditingModule",
					displayName: @"Collection Editing",
					group: "Tutorials",
					type: "PropertyGridDemo.CollectionEditingModule",
					shortDescription: @"This demo shows how to use Property Grid to browse and manage (add, edit, etc.) collection properties.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">Property Grid</Span> control can be used to browse and manage objects contained within collection properties. To enable collection browsing, you should add a <Span FontWeight=""Bold"">CollectionDefinition</Span> object to <Span FontWeight=""Bold"">PropertyGrid.PropertyDefinitions</Span> and specify the property path. You can also use the <Span FontWeight=""Bold"">NewItemInitializer</Span> property to specify which items a user can create and initialize a new item.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ValidationDataAnnotationAttributesModule",
					displayName: @"Validation (DataAnnotation attributes)",
					group: "Tutorials",
					type: "PropertyGridDemo.ValidationDataAnnotationAttributesModule",
					shortDescription: @"This demo illustrates data validation via DataAnnotation attributes in the Property Grid.",
					description: @"
                        <Paragraph>
                        The <Bold>Property Grid</Bold> control supports data validation and displays warning icons. You don't need to adjust data validation settings manually when creating property descriptions. It is done automatically when the <Bold>Property Grid</Bold> is bound to a data source that contains <Bold>DataAnnotation</Bold> attributes.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ValidationFluentAPIModule",
					displayName: @"Validation (Fluent API)",
					group: "Tutorials",
					type: "PropertyGridDemo.ValidationFluentAPIModule",
					shortDescription: @"This demo illustrates data validation via DevExpress Fluent API in the Property Grid.",
					description: @"
                        <Paragraph>
                        The <Bold>Property Grid</Bold> control supports data validation and displays warning icons. You don't need to adjust data validation settings manually when creating property descriptions. It is done automatically when the <Bold>Property Grid</Bold> is bound to a data source that provides validation attributes via <Bold>DevExpress Fluent API</Bold>.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ValidationIDataErrorInfoModule",
					displayName: @"Validation (IDataErrorInfo)",
					group: "Tutorials",
					type: "PropertyGridDemo.ValidationIDataErrorInfoModule",
					shortDescription: @"The Property Grid supports standard data validation mechanisms common for all DevExpress .NET components.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use the <Bold>IDataErrorInfo</Bold> interface to display custom error information. In this demo, all validation is performed at the data level, not at the property grid level.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TypeConvertersFluentAPIModule",
					displayName: @"TypeConverters (Fluent API)",
					group: "Tutorials",
					type: "PropertyGridDemo.TypeConvertersFluentAPIModule",
					shortDescription: @"In this demo the Property Grid uses type converters to generate the content of collection properties.",
					description: @"
                        <Paragraph>
                        This demo shows the MVVM capabilities of the <Bold>Property Grid</Bold> control. Type converters defined at the data level via <Bold>DevExpress Fluent API</Bold> are used to generate the content of collection properties based on the user input.
                        </Paragraph>
                        <Paragraph>
                        A text entered at the value cell of the Product collection property is used by the corresponding TypeConverter to set the Name property of the object the type converter generates.
                        </Paragraph>
                        <Paragraph>
                        The TypeConverter that corresponds to the Tags property creates an array of as many items as there are words separated by space in the text entered at the value cell. The number of items is shown in this value cell.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.V142
				)
			};
		}
	}
}
