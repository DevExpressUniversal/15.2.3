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
		static List<Module> Create_XtraEditors_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraEditors %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraEditors.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "InplaceEditors",
					displayName: @"Inplace Grid Cell Editors",
					group: "Inplace Editing",
					type: "DevExpress.XtraEditors.Demos.InplaceEditors",
					description: @"In this example, you can explore the many inplace editors that ship as part of the DevExpress WinForms Subscription. And as you can see, the XtraGrid can actually be used as an inverted grid view – displaying field values to the right of column headers.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "MultiEditors",
					displayName: @"Dynamically Assigned Editors",
					group: "Inplace Editing",
					type: "DevExpress.XtraEditors.Demos.MultiEditors",
					description: @"Be it at design or runtime, flexibility lies at the heart of the XtraGrid Suite and the XtraEditors Library. In this example, we demonstrate how you can dynamically assign inline data editors to individual grid cells. As you’ll discover once you begin using DevExpress Data Editors, you have a massive set of options at your disposal for inline editing within the grid. Take for instance the use of the progress bar for the Relevance column…you can control its value via the + and – keys. How’s that for awesome.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleCheckEdit",
					displayName: @"Check Edit",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleCheckEdit",
					description: @"This tutorial demonstrates a CheckEdit editor that has checked, unchecked and, optionally, grayed states. It's most commonly used to edit Boolean values.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleRatingControl",
					displayName: @"Rating Control",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleRatingControl",
					description: @"Rating Control",
					addedIn: KnownDXVersion.V142
				),
				new SimpleModule(demo,
					name: "ModuleProgressBar",
					displayName: @"Progress Bar",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleProgressBar",
					description: @"This tutorial demonstrates a ProgressBarControl. It's most commonly used to indicate the progress of a lengthy operation.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new SimpleModule(demo,
					name: "ModuleProgressPanel",
					displayName: @"Progress Panel",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleProgressPanel",
					description: @"This demo illustrates the ProgressPanel, commonly used to indicate that the application is busy.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new SimpleModule(demo,
					name: "ModuleRadioGroup",
					displayName: @"Radio Group",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleRadioGroup",
					description: @"This tutorial demonstrates a RadioGroup editor which provides a group of radio buttons. It allows a user to select one of several options.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModulePictureEdit",
					displayName: @"Picture Edit",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModulePictureEdit",
					description: @"This tutorial demonstrates a PictureEdit editor which is used to display images stored as a bitmap, metafile, icon, JPEG, GIF or PNG.",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V151
				),
				new SimpleModule(demo,
					name: "ModuleMarqueeProgressBar",
					displayName: @"Marquee Progress Bar",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleMarqueeProgressBar",
					description: @"This tutorial demonstrates a MarqueeProgressBarControl. It's most commonly used to indicate that an operation is going on by continuously scrolling a block from left to right.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleTrackBar",
					displayName: @"Track Bar",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleTrackBar",
					description: @"This tutorial demonstrates a TrackBarControl. This is a scrollable control which is used to slide a small thumb along a continuous line.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleRangeTrackBar",
					displayName: @"Range Track Bar",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleRangeTrackBar",
					description: @"This tutorial demonstrates the RangeTrackBarControl that allows the range of values to be specified. Use the editor's Value property to specify the minimum and maximum limits of the range.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleZoomTrackBar",
					displayName: @"Zoom Track Bar",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleZoomTrackBar",
					description: @"This tutorial demonstrates a ZoomTrackBarControl. This control emulates the Zoom track bar introduced in MS Office 2007. This control is appropriate for implementing a zooming bar in your applications.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleSparklineEdit",
					displayName: @"Sparkline Edit",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleSparklineEdit",
					description: @"This tutorial demonstrates a SparklineEdit control. This control visualizes data in a highly condensed way thus allowing end-users to quickly understand and compare values from different sources.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleChartRangeControlClients",
					displayName: @"Chart Range Control Clients",
					group: "Range Control",
					type: "DevExpress.XtraEditors.Demos.ModuleChartRangeControlClients",
					description: @"This demo illustrates how to use date-time and numeric chart clients of a range control to paint chart data within the range control's viewport. In this demo you can change the chart client view and the default palette in the Template View section. Note that the grid properties of the range control (grid spacing, snap spacing for each client type; grid alignment, snap alignment for the date-time chart client) are calculated automatically.  To specify custom settings for these properties, uncheck the ""Automatic"" check boxes.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleToggleSwitch",
					displayName: @"Toggle Switch",
					group: "Editors without Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleToggleSwitch",
					description: @"This tutorial demonstrates a ToggleSwitch editor that has on and off states.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleTextEdit",
					displayName: @"Text Edit",
					group: "Editors with Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleTextEdit",
					description: @"This tutorial demonstrates a TextEdit – a single-line text editor.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleButtonEdit",
					displayName: @"Button Edit",
					group: "Editors with Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleButtonEdit",
					description: @"This tutorial demonstrates a ButtonEdit editor which lets you display an unlimited number of buttons within the edit box.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleHyperLinkEdit",
					displayName: @"Hyper Link",
					group: "Editors with Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleHyperLinkEdit",
					description: @"This tutorial demonstrates a HyperLinkEdit editor which lets you edit hyperlinks and navigate to their targets.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleMemoEdit",
					displayName: @"Memo Edit",
					group: "Editors with Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleMemoEdit",
					description: @"This tutorial demonstrates a MemoEdit editor which enables multi-line text to be displayed and edited.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleSpinEdit",
					displayName: @"Spin Edit",
					group: "Spin (Up/Down) Editors",
					type: "DevExpress.XtraEditors.Demos.ModuleSpinEdit",
					description: @"This tutorial demonstrates a SpinEdit editor which allows numeric values to be edited using spin buttons.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleTimeEdit",
					displayName: @"Time Edit",
					group: "Spin (Up/Down) Editors",
					type: "DevExpress.XtraEditors.Demos.ModuleTimeEdit",
					description: @"This tutorial demonstrates a TimeEdit editor which allows time values to be edited using spin buttons.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleCalcEdit",
					displayName: @"Calc Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleCalcEdit",
					description: @"This tutorial demonstrates a CalcEdit editor which provides a dropdown calculator. Using the dropdown calculator, you can perform basic calculations such as addition, multiplication, getting the inverse of a number and working with a memory register etc.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleColorEdit",
					displayName: @"Color Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleColorEdit",
					description: @"This tutorial demonstrates a ColorEdit editor which allows you to select a color from the dropdown window. The dropdown contains three tabbed pages displaying predefined custom, web and system colors.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleColorPickEdit",
					displayName: @"Color Pick Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleColorPickEdit",
					description: @"This tutorial demonstrates a ColorEdit editor which allows you to select a color from the dropdown window. The dropdown contains three tabbed pages displaying predefined custom, web and system colors.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleComboBoxEdit",
					displayName: @"Combobox Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleComboBoxEdit",
					description: @"This tutorial demonstrates a ComboBoxEdit editor which allows you to select the predefined items from the dropdown list.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleDateEdit",
					displayName: @"Date Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleDateEdit",
					description: @"This tutorial demonstrates a DateEdit editor which allows date/time values to be edited using a dropdown calendar.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleImageComboBoxEdit",
					displayName: @"Image Combobox Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleImageComboBoxEdit",
					description: @"This tutorial demonstrates an ImageComboBoxEdit editor. This editor represents a combo box editor whose items can display custom images.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleImageEdit",
					displayName: @"Image Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleImageEdit",
					description: @"This tutorial demonstrates an ImageEdit editor which displays images (bitmap, metafile, icon, JPEG, GIF or PNG file) in its popup window.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleLookUpEdit",
					displayName: @"LookUp Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleLookUpEdit",
					description: @"This tutorial demonstrates a LookUpEdit editor which is used to display a multi-column record list from an associated data source.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleMemoExEdit",
					displayName: @"MemoEx Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleMemoExEdit",
					description: @"This tutorial demonstrates a MemoExEdit editor which allows multi-line text to be edited in its popup window.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleMRUEdit",
					displayName: @"MRU Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleMRUEdit",
					description: @"This tutorial demonstrates a MRUEdit editor which allows the most recently entered value to be chosen from the dropdown list.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModulePopupContainerEdit",
					displayName: @"Popup Container Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModulePopupContainerEdit",
					description: @"This tutorial demonstrates a PopupContainerEdit editor which is used to display custom controls within its popup window. You must additionally create a PopupContainerControl control which will represent the popup window. You can place any control onto the popup container control. To display the control's contents within an editor's popup window, you should set the editor's PopupControl property to this control.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleFontEdit",
					displayName: @"Font Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleFontEdit",
					description: @"This tutorial demonstrates the FontEdit control. The editor allows an end-user to select a specific font by picking up from among all available fonts. After a font has been selected, it's added to the list, which contains the most recently used fonts, and these are listed at the top of the editor's dropdown. Practice selecting fonts and customizing the length of the MRU font list.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleCheckedComboBoxEdit",
					displayName: @"Checked Combobox Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleCheckedComboBoxEdit",
					description: @"This tutorial demonstrates the CheckedComboBoxEdit control that allows you to display and edit a set of Boolean options or bit fields in a popup window.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleDropDownButton",
					displayName: @"Drop Down Button",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleDropDownButton",
					description: @"This tutorial demonstrates the DropDownButton control - a button that can be associated with a popup control or a context menu. You can invoke the associated control by clicking the button's drop-down arrow, or the button itself (if the arrow is hidden).",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleNavigator",
					displayName: @"Data Navigator",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleNavigator",
					description: @"This tutorial demonstrates a DataNavigator control which provides a graphical interface to navigate and manipulate the records in a data source.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleMaskBox",
					displayName: @"Mask Box",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleMaskBox",
					description: @"This tutorial demonstrates how to use masks during editing. Masks provide restricted data input as well as formatted data output. This can be useful when the entered string should match a specific format.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleListBox",
					displayName: @"List Box",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleListBox",
					description: @"This tutorial demonstrates a ListBoxControl which is used to display a list of items that a user can select.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleCalendar",
					displayName: @"Calendar Control",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleCalendar",
					description: @"The Calendar Control allows you to select a date, a continuous date range or multiple dates and date ranges. It can display one or multiple months simultaneously. The optional time edit box enables you to enter the time portion of a DateTime value. Use the options on the left to customize the Calendar Control's presentation.
The Custom tab demonstrates how to highlight certain calendar cells, display custom text and icons in certain dates. The appearance settings and display text in calendar cells are customized using a CellStyleProvider object. Icons displayed in cells are context buttons created and customized using the ContextButtons collection and ContextButtonCustomize event. When you click on a context button, the ContextButtonClick event fires, which shows a flyout panel with a date description.",
					addedIn: KnownDXVersion.V152
				),
				new SimpleModule(demo,
					name: "ModuleToolTipController",
					displayName: @"Tool Tip Controller",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleToolTipController",
					description: @"This tutorial demonstrates the functionality of the ToolTipController component which manages tooltips for controls.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleDXValidationProvider",
					displayName: @"DXValidation Provider",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleDXValidationProvider",
					description: @"This tutorial demonstrates how to perform data validation via the DXValidationProvider component. This component allows you to associate specific validation rules with editors, and perform validation manually or automatically (on leaving editors). In the example, validation rules are created and applied to editors in code. However, these can also be created at design time via the component's designer. Practice changing the editors' values to violate validation rules and see how the editors respond.",
					addedIn: KnownDXVersion.Before142,
					associatedFiles: new [] {
						@"\WinForms\CS\EditorsMainDemo\Modules\DXValidationProvider.cs",
						@"\WinForms\VB\EditorsMainDemo\Modules\DXValidationProvider.vb"
					}
				),
				new SimpleModule(demo,
					name: "ModuleBeakForm",
					displayName: @"Beak Form",
					group: "Multi-Purpose",
					type: "DevExpress.XtraEditors.Demos.ModuleBeakForm",
					description: @"In this demo, you can see the Beak Form - a modern pop-up panel with a beak that can be displayed at a specific position related to its owner control. This panel is fully customizable. You can set the panel's background, display or hide, close buttons, and specify whether this panel should be hidden on an outside click or on the close button click only.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleBreadCrumbEdit",
					displayName: @"BreadCrumb Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleBreadCrumbEdit",
					description: @"This demo demonstrates the Breadcrumb Edit control. This control allows you to create a branchy node tree. In this demo, nodes are populated dynamically depending on your current PC folders' structure. You can click a node caption to navigate to this node. Each node displays its own drop-down button. A click on this button invokes a drop-down list that displays all child nodes for this node. The Breadcrumb Edit control can also operate in Edit mode. In this mode, nodes are not clickable and the current editor path is presented as plain text. End-users can manually edit this path via the keyboard to navigate to specific nodes. Note that the first two nodes in this demo remain constantly visible wherever you navigate. These nodes are called persistent nodes and are designed to store shortcuts to the most significant or frequently used nodes within the tree (e.g., the 'Computer' node stores shortcuts to local disc drives).",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleTokenEdit",
					displayName: @"Token Edit",
					group: "Editors with Textboxes",
					type: "DevExpress.XtraEditors.Demos.ModuleTokenEdit",
					description: @"This demo illustrates the Token Edit control - an advanced text editor that performs custom validation upon an entered text. Any text block that successfully passes validation is transorfmed into a token - an interactive element with an icon and popup panel, displayed on hover. If the Token Edit history is enabled, whenever you enter text, a drop-down with matching tokens list is displayed. You can select an item from this list instead of manually typing the entire text. You can also manually specify the maximum number of expand lines. If the entered text occupies more lines, vertical scrollbars will appear.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "ModuleTimeSpanEdit",
					displayName: @"Time Span Edit",
					group: "Editors with Dropdowns",
					type: "DevExpress.XtraEditors.Demos.ModuleTimeSpanEdit",
					description: @"This example demonstrates use of the TimeSpanEdit control which allows you to edit time span values (time intervals) using a Tile based UI. The control's dropdown window provides scrollable tiles used to specify days, hours, minutes and seconds. You can modify values using touch gestures, the mouse or keyboard.",
					addedIn: KnownDXVersion.V142
				)
			};
		}
	}
}
