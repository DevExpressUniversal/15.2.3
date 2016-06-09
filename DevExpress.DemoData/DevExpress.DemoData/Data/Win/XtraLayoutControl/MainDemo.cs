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
		static List<Module> Create_XtraLayoutControl_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraLayout %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraLayout.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Employees",
					displayName: @"Nwind Employees",
					group: "Sample Layouts",
					type: "DevExpress.XtraLayout.Demos.Employees",
					description: @"This demo demonstrates a sample layout implemented using the XtraLayoutControl. All the controls within the layout are bound to the 'Nwind' database. The basic features provided by the XtraLayoutControl are regular layout items, regular and tabbed layout groups, runtime layout customization. This allows professional user interfaces to be created. To customize the current layout you should invoke its Customization Form. To do this, right click within the layout control's client area and select the 'Show Customization Form' item. The layout can be loaded from a file in XML format. Use the 'Xml files' combo box to select the desired file. Click the 'Load Layout' button to restore the previously saved layout.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Cars",
					displayName: @"Cars Example",
					group: "Sample Layouts",
					type: "DevExpress.XtraLayout.Demos.Cars",
					description: @"In this demo all the controls within the layout are bound to the 'Cars' database. The basic features provided by the XtraLayoutControl are regular layout items, regular and tabbed layout groups, runtime layout customization. This allows professional user interfaces to be created. To customize the current layout you should invoke its Customization Form. To do this, right click within the layout control's client area and select the 'Show Customization Form' item. The layout can be loaded from a file in XML format. Use the 'Xml files' combo box to select the desired file. Click the 'Load Layout' button to restore the previously saved layout.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "MasterDetail",
					displayName: @"Master-Detail",
					group: "Sample Layouts",
					type: "DevExpress.XtraLayout.Demos.MasterDetail",
					description: @"In this demo all the controls are bound to the 'Nwind' database. The basic features provided by the XtraLayoutControl are regular layout items, regular and tabbed layout groups, runtime layout customization. This allows professional user interfaces to be created. To customize the current layout you should invoke its Customization Form. To do this, right click within the layout control's client area and select the 'Show Customization Form' item. The layout can be loaded from a file in XML format. Use the 'Xml files' combo box to select the desired file. Click the 'Load Layout' button to restore the previously saved layout.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Validating",
					displayName: @"Validating",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.Validating",
					description: @"In this demo all fields marked with a '*' are validated using the DXErrorProvider component. Hover mouse over any field to see its validation rules description.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Multilanguage",
					displayName: @"Localization",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.Multilanguage",
					description: @"This example illustrates the layout management feature for a separate localized form that contains a LayoutControl. Regardless of the form localization, its size and DPI setting, the LayoutControl maintains a consistent layout of controls, without them being overlapped and misaligned. This example also provides a capability to switch between regular and flow layout modes for the LayoutControl. In regular layout mode, layout items tend to stretch within the control and can have any size. In flow layout mode, layout items are not stretched; they are automatically arranged in rows wrapping at the control's right edge.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142,
					associatedFiles: new [] {
						@"\WinForms\CS\LayoutMainDemo\Modules\Multilanguage\Multilanguage.cs",
						@"\WinForms\VB\LayoutMainDemo\Modules\Multilanguage\Multilanguage.vb"
					}
				),
				new SimpleModule(demo,
					name: "ImageLayout",
					displayName: @"ImageLayout",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.ImageLayout",
					description: @"This demo illustrates creating an attractive and elegant UI using the XtraLayoutControl. The top bar represents a LayoutControl displaying a set of static images. Layout items that represent background images are allowed to be stretched, while layout items that display car icons cannot be stretched. You can practice resizing the form to see how it works.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ItemsVisibility",
					displayName: @"Items Visibility (simple)",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.ItemsVisibility",
					description: @"This demo illustrates the XtraLayoutControl's smart visibility feature. Practice hiding and restoring layout items by toggling the check boxes. If you hide a specific layout item and then make it visible, it will be displayed at the same position and the same size as before (provided that the layout of controls hasn't been changed). Images and description text are taken from the http://www.kiddicare.com",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "UserDetailsEditViewForm",
					displayName: @"Items Visibility (advanced)",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.UserDetailsEditViewForm",
					description: @"This demo is an advanced illustration of the XtraLayoutControl's smart visibility feature. Each layout item provides the Visibility property which specifies when the layout item should be visible at runtime: always visible, always hidden, visible only in customization mode, and visible when runtime customization is not performed. In this demo, only non-empty layout items are initially displayed. Pressing the Edit Data button modifies the Visibility properties of all layout items, so that they are all made visible. Pressing the button again hides only empty fields.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ContentImages",
					displayName: @"Group Content Images",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.ContentImages",
					description: @"Layout groups support content images, displayed in the background, that can be aligned to any edge of the group. So, when you resize a group, the content image is stuck to the associated group's edge and moved, unchanged, accordingly. Practice resizing the form to see how the WELCOME image is moved.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\LayoutMainDemo\Modules\ContentImagesInLayoutGroups.cs",
						@"\WinForms\VB\LayoutMainDemo\Modules\ContentImagesInLayoutGroups.vb"
					}
				),
				new SimpleModule(demo,
					name: "LayoutControlDragDrop",
					displayName: @"Drag Drop",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.LayoutControlDragDrop",
					description: @"The LayoutControl provides events that allow the drag-and-drop functionality to be implemented. This example demonstrates how to implement drag-and-drop between a layout control and a ListView and another layout control.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\LayoutMainDemo\Modules\DragDrop.cs",
						@"\WinForms\VB\LayoutMainDemo\Modules\DragDrop.vb"
					}
				),
				new SimpleModule(demo,
					name: "Browser",
					displayName: @"Browser Demo",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.Browser",
					description: @"This demo illustrates the creation of a tabbed interface using the LayoutControl's tabbed groups. Tab pages support Close buttons. In this demo, the visibility of Close buttons for each tab page is controlled dynamically. Practice changing the tabs' orientation, position and multi-line options.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Layout",
					displayName: @"Save&Restore Layout",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.Layout",
					description: @"The current layout can be saved to a file in XML format, a stream or the system registry. In this demo the layout is automatically saved to a file in XML format each time the OK button is pressed. To restore the default layout press the 'Delete xml file' button. To customize the current layout you should invoke the Customization Form. To do this, press the button which is displayed at the 'Login' form's bottom-left corner or right click within the layout control's client area and select the 'Show Customization Form' item.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Printing",
					displayName: @"Waybill Printing",
					group: "Sample Layouts",
					type: "DevExpress.XtraLayout.Demos.LayoutInteractivePrinting",
					description: @"This demo illustrates the printing capabilities of the Layout Control. By default, the Layout Control follows a ""Flow layout"" approach when arranging items in the printed document or exported file. Layout items are optimally arranged, back-to-back, when populating page space. The flow of items automatically wraps at the page's rightmost edge. Embedded IPrintable controls, such as the Data Grid in this example, are printed using the XtraPrinting Library, which provides high-quality output.",
					addedIn: KnownDXVersion.V142,
					newUpdatedPriority: 1
				),
				new SimpleModule(demo,
					name: "CodeFirst",
					displayName: @"Code First",
					group: "API Code Examples",
					type: "DevExpress.XtraLayout.Demos.CodeFirstModule",
					description: @"This section includes examples demonstrating the LayoutControl's API. The examples show how to customize a layout and perform various layout-aware tasks in code. You can modify the code snippets at runtime and immediately see the result.",
					addedIn: KnownDXVersion.V151,
					newUpdatedPriority: 0,
					isCodeExample: true
				),
				 new SimpleModule(demo,
					name: "FlowLayout",
					displayName: @"Flow Layout",
					group: "Features",
					type: "DevExpress.XtraLayout.Demos.TechnoLayout",
					description: @"This demo illustrates the Layout Control’s flow layout mode. In this mode, the control region is divided into cells of a fixed size, where each layout item occupies a specific number of cells. Layout items are automatically arranged across rows wrapped at the control’s rightmost edge. Layout item size does not change during Layout Control resizing, but the item’s position may be updated.",
					addedIn: KnownDXVersion.V142,
					newUpdatedPriority: 0,
					associatedFiles: new [] {
						@"\WinForms\CS\LayoutMainDemo\Modules\TechnoLayout\TechnoLayout.cs",
						@"\WinForms\VB\LayoutMainDemo\Modules\TechnoLayout\TechnoLayout.vb"
					}
				),
				new SimpleModule(demo,
					name: "OneNote",
					displayName: @"One Note Demo",
					group: "Sample Layouts",
					type: "DevExpress.XtraLayout.Demos.OneNote",
					description: @"This demo shows how to use the XtraLayoutControl to emulate the layout of MS One Note. Sections and pages are created using Tabbed Groups. Note that custom colors are assigned to tabs. These colors are automatically mixed with the currently applied skin.",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
