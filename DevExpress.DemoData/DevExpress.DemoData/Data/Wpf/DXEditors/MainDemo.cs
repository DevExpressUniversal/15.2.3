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
		static List<Module> Create_DXEditors_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new WpfModule(demo,
					name: "FontEditModule",
					displayName: @"FontEdit",
					group: "Editors",
					type: "EditorsDemo.FontEditModule",
					shortDescription: @"This editor displays available fonts and allows an end-user to select a specific font.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the FontEdit control. The editor allows an end-user to select a specific font by picking up from among all available fonts.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "InputValidationModule",
					displayName: @"Input Validation",
					group: "Validation",
					type: "EditorsDemo.InputValidationModule",
					shortDescription: @"Data editors can be manually validated by handling the Validate event.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to validate data input at the editor level. In order to implement validation of this type, it is necessary to handle the <Span FontWeight=""Bold"">Validate</Span> event of a specific editor and set the <Span FontWeight=""Bold"">e.IsValid</Span> property depending on whether the entered <Span FontWeight=""Bold"">e.Value</Span> is valid or not. Note that if the entered value is invalid, an automatic animated error icon is displayed with a tooltip showing the error message.
                        </Paragraph>
                        <Paragraph>
                        On the Options pane of this demo, it is possible to specify how editors should behave when they get invalid values: while <Span FontWeight=""Bold"">AllowLeaveEditor</Span> allows you to switch to another editor and re-enter a value for this editor later, the <Span FontWeight=""Bold"">WaitForValidValue</Span> mode locks the entire application until a correct value is entered.
                        </Paragraph>
                        <Paragraph>
                        Also, for every editor in this demo, you can customize the following options:
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Caused Validation. </Span> Validation is enabled for this editor. By default, it takes place when trying to move focus away from the editor;
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • On text input. </Span> In addition to when losing focus, validation takes place with each text modification operation;
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • On Enter key pressed. </Span> In addition to when losing focus, validation takes place when the Enter key is pressed.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Input Validation Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6954"),
						new WpfModuleLink(
							title: "Binding Validation",
							type: WpfModuleLinkType.Demos,
							url: "local:Binding Validation")
					}
				),
				new WpfModule(demo,
					name: "BindingValidationModule",
					displayName: @"Binding Validation",
					group: "Validation",
					type: "EditorsDemo.BindingValidationModule",
					shortDescription: @"Two standard approaches can be used to validate data input at the data binding level.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to validate data input at the data binding level. In this demo, two standard validation approaches are demonstrated. The first approach involves setting the <Span>Binding.ValidatesOnDataErrors</Span> property to True and binding editors to an object that implements the <Span>IDataErrorInfo</Span> interface. The second approach is implemented by the <Span>Binding.ValidationRules</Span> collection (in this demo it contains the <Span>EmptyStringValidationRule</Span> , intended to check whether entered strings are empty).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Input Validation Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6954"),
						new WpfModuleLink(
							title: "Input Validation",
							type: WpfModuleLinkType.Demos,
							url: "local:Input Validation")
					}
				),
				new WpfModule(demo,
					name: "TextEditModule",
					displayName: @"TextEdit",
					group: "Editors",
					type: "EditorsDemo.TextEditModule",
					shortDescription: @"This demo demonstrates the key features provided by a text editor.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">TextEdit</Span> - a single or multiline text editor. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • All basic text editing features are supported, including selections and built-in context menu;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Optional automatic word-wrapping, allowing you to display and edit multi-line content;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Word or character text trimming;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Full masked input support;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Advanced data validation capabilities.
                        </Paragraph>",
					allowTouchThemes: false,
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ButtonEditModule",
					displayName: @"ButtonEdit",
					group: "Editors",
					type: "EditorsDemo.ButtonEditModule",
					updatedIn:KnownDXVersion.V152, 
					shortDescription: @"A Button Editor is a text editor with embedded buttons.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">ButtonEdit</Span> editor, which allows you to display an unlimited number of buttons within the edit box. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Any number of buttons within the edit box;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Customizable button alignment;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Total control over button content;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Optional automatic default button;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Full masked input support;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Advanced data validation capabilities.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CheckEditModule",
					displayName: @"CheckEdit",
					group: "Editors",
					type: "EditorsDemo.CheckEditModule",
					shortDescription: @"This demo shows the check box editor.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">CheckEdit</Span> editor used to edit Boolean values. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Optional indeterminate check state;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Complete content customization support, including the use of WPF Templates;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Read-only operation mode.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "MemoEditModule",
					displayName: @"MemoEdit",
					group: "Editors",
					type: "EditorsDemo.MemoEditModule",
					shortDescription: @"A Memo Editor can be used to display and edit the multi-line text.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Span FontWeight=""Bold"">MemoEdit</Span> control. You can change the editor's appearance and behavior using the Options pane on the right.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SpinEditModule",
					displayName: @"SpinEdit",
					group: "Editors",
					type: "EditorsDemo.SpinEditModule",
					shortDescription: @"This demo shows the editor with spin buttons used to operate with numeric values.",
					description: @"
                        <Paragraph>
                        This demo illustrates the <Span FontWeight=""Bold"">SpinEdit</Span> control. You can change the editor's value using the controls and commands on the right. Other options allow you to change the editor's visual settings.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Numeric Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Numeric")
					}
				),
				new WpfModule(demo,
					name: "DateEditModule",
					displayName: @"DateEdit",
					group: "Editors",
					type: "EditorsDemo.DateEditModule",
					shortDescription: @"This demo shows the date editor with a dropdown calendar.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">DateEdit</Span> editor which allows DateTime values to be edited using a dropdown calendar. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Windows Vista style calendar views, allowing you to quickly select dates, months or years;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Customizable week day visibility;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Quick navigation to the current date;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Built-in date/time mask controlling text input;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • The capability to limit date input to a predefined range.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Date-Time Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Date-Time")
					}
				),
				new WpfModule(demo,
					name: "DatePickerModule",
					displayName: @"DatePicker",
					group: "Editors",
					type: "EditorsDemo.DatePickerModule",
					shortDescription: @"This demo shows the date editor with a Date-Time Picker.",
					description: @"
                        <Paragraph>
                        A <Span FontWeight=""Bold"">DateEdit</Span> allows you to edit DateTime values using a Windows 8 Inspired Date-Time Picker. The DateEditor's features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Built-in date/time mask controlling text input;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • The capability to limit date input to a predefined range.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Date-Time Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Date-Time")
					}
				),
				new WpfModule(demo,
					name: "ListBoxEditModule",
					displayName: @"ListBoxEdit",
					group: "Editors",
					type: "EditorsDemo.ListBoxEditModule",
					shortDescription: @"This editor displays a list of items which can be selected by an end-user.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">ListBoxEdit</Span> that displays a list of items which can be selected by an end-user. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Built-in list styles - Native, Checked and Radio;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Lookup functionality. The editor can load values for its list from a data field. The actual edit value (usually, ID) is fetched from a different field;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Optional item highlighting;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Multiple item selection.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to CollectionView",
							type: WpfModuleLinkType.Demos,
							url: "local:Binding to CollectionView")
					}
				),
				new WpfModule(demo,
					name: "ComboBoxEditModule",
					displayName: @"ComboBoxEdit",
					group: "Editors",
					type: "EditorsDemo.ComboBoxEditModule",
					updatedIn:KnownDXVersion.V151, 
					shortDescription: @"A Combo Box editor displays a list of items within a dropdown window.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">ComboBoxEdit</Span> editor which allows you to select predefined items from the dropdown list. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Lookup functionality. The editor can load values for its dropdown list from a data field. The actual edit value (usually, ID) is fetched from a different field;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Customizable dropdown list size;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Total control over item content;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Optional OK and Cancel buttons shown in the dropdown;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Optional size grip for the dropdown list.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to CollectionView",
							type: WpfModuleLinkType.Demos,
							url: "local:Binding to CollectionView")
					}
				),
				new WpfModule(demo,
					name: "TokenComboBoxModule",
					displayName: @"Token ComboBoxEdit",
					group: "Editors",
					type: "EditorsDemo.TokenComboBoxModule",
					shortDescription: @"The Token ComboBoxEdit gives you the ability to input, select and edit multiple values.",
					description: @"
                        <Paragraph>
                        The Token ComboBoxEdit was inspired by modern mail clients. It allows you to input, select and edit multiple values. Selected values are represented by special visual elements called tokens. You can click within tokens to edit values or click the remove glyph to delete the token.
                        </Paragraph>
                        <Paragraph>
                        As you can see, tokens can be arranged across one or more lines. Token mode supports all ComboBoxEdit features including autocomplete, incremental filtering and immediate popup
                        </Paragraph>",
					addedIn: KnownDXVersion.V142,
					updatedIn: KnownDXVersion.V151,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "LINQ To SQL Server",
					displayName: @"LINQ To SQL Server",
					group: "Server Mode",
					type: "EditorsDemo.EditorsServerModeSource",
					shortDescription: @"In this demo, DevExpress Editors operate in Server Mode and are bound to a large data source.",
					description: @"
                        <Paragraph>
                        In this demo, the editors can be bound to a LinqServerModeDataSource.
                        </Paragraph>
                        <Paragraph>
                        After you set connection options and create a connection to a SQL Server instance, editors work in synchronous server mode using LINQ. In server mode, editors load data in small portions, optimizing memory usage and data transmission. This ensures rapid access to data and eliminates the various problems related to loading large volumes of data and processing it on the client side. 
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: true
				),
								new WpfModule(demo,
					name: "MultiThread Data Processing",
					displayName: @"MultiThread Data Processing",
					group: "Server Mode",
					type: "EditorsDemo.ComboBoxEditWithPLINQSource",
					shortDescription: @"This demo features the usage of all available system cores to improve performance for data-intensive operations",
					description: @"
                        <Paragraph>
                        In this demo, all data operations are parallelized on multiple processor cores. This allows the computing power of your hardware to be utilized to the full extent.
                        </Paragraph>",
					addedIn: KnownDXVersion.V152,
					isFeatured: true
				),			 
				new WpfModule(demo,
					name: "LookUpEditWithMultipleSelection",
					displayName: @"LookUpEdit with multiple selection",
					group: "Editors",
					type: "CommonDemo.LookUpEditWithMultipleSelection",
					shortDescription: @"The DevExpress Multi-Column Lookup Editor supports multiple value selection.",
					description: @"
                        <Paragraph>
                        End-users can select more that one value by:
                        </Paragraph>
                        <Paragraph>
                        • holding either SHIFT or CTRL key, and selecting rows within the dropdown grid using the mouse pointer;
                        </Paragraph>
                        <Paragraph>
                        • holding the SHIFT key, and selecting rows using the TOP or BOTTOM arrow key;
                        </Paragraph>
                        <Paragraph>
                        • holding the CTRL key, navigating to rows using the TOP or BOTTOM arrow key and selecting required rows using the SPACE key.
                        </Paragraph>
                        <Paragraph>
                        Selected values are displayed within the edit box and separated via semicolons.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "SearchLookUpEditModule",
					displayName: @"SearchLookUpEdit",
					group: "Editors",
					type: "EditorsDemo.SearchLookUpEditModule",
					shortDescription: @"This demo demonstrates the new Search LookUp Editor.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the new <Bold>WPF Search LookUp Editor</Bold>.
                        </Paragraph>
                        <Paragraph>
                        The DevExpress <Bold>WPF Search LookUp Editor</Bold> combines lookup functionality with the <Bold>Find</Bold> feature. Like the <Bold>WPFLookUpEdit</Bold> control, the <Bold>WPF Search LookUp Editor</Bold> embeds a grid control in the dropdown, and provides numerous grid customization features.
                        </Paragraph>
                        <Paragraph>
                        The built-in <Bold>Find box</Bold> allows you to quickly locate data. Enter characters into the <Bold>Find box</Bold> and the control will filter records accordingly (those that meet the search criteria).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Binding to CollectionView",
							type: WpfModuleLinkType.Demos,
							url: "local:Binding to CollectionView")
					}
				),
				new WpfModule(demo,
					name: "TokenLookUpModule",
					displayName: @"Token LookUpEdit",
					group: "Editors",
					type: "EditorsDemo.TokenLookUpModule",
					shortDescription: @"The Token LookUpEdit gives you the ability to input, select and edit multiple values.",
					description: @"
                        <Paragraph>
                        The Token LookUpEdit was inspired by modern mail clients. It allows you to input, select and edit multiple values. Selected values are represented by special visual elements called tokens. You can click within tokens to edit values or click the remove glyph to delete the token.
                        </Paragraph>
                        <Paragraph>
                        As you can see, tokens can be arranged across one or more lines. Token mode supports all LookUpEdit features including autocomplete, incremental filtering and immediate popup.
                        </Paragraph>",
					addedIn: KnownDXVersion.V142,
					updatedIn: KnownDXVersion.V151,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "ProgressBarEditModule",
					displayName: @"ProgressBarEdit",
					group: "Editors",
					type: "EditorsDemo.ProgressBarEditModule",
					shortDescription: @"This demo shows the standard and marquee (intermediate) progress bars.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span>ProgressBarEdit</Span> editor used to indicate the progress of a lengthy operation. Its features include:
                        </Paragraph>
                        <Paragraph>
                        • Built-in content display modes (None, Value, Custom Content);
                        </Paragraph>
                        <Paragraph>
                        • The capability to limit the range of values;
                        </Paragraph>
                        <Paragraph>
                        • Values can be displayed as percentages;
                        </Paragraph>
                        <Paragraph>
                        • Horizontal and vertical orientation;
                        </Paragraph>
                        <Paragraph>
                        • Indeterminate progress style (marquee).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "TrackBarEditModule",
					displayName: @"TrackBarEdit",
					group: "Editors",
					type: "EditorsDemo.TrackBarEditModule",
					shortDescription: @"A track bar is a scrollable control which is used to slide a small thumb along a continuous line.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a <Span FontWeight=""Bold"">TrackBarEdit</Span> editor – a scrollable control used to slide a small thumb along a continuous line. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Built-in presentation styles: Native, Range, Zoom, Zoom and Range;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • The capability to limit the range of values;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Horizontal and vertical orientation;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Customizable position of ticks;
                        </Paragraph>
                        <Paragraph>
                        <Run Text="" ""/> • Customizable tick frequency.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RangeTrackBarEditModule",
					displayName: @"RangeTrackBarEdit",
					group: "Editors",
					type: "EditorsDemo.RangeTrackBarEditModule",
					shortDescription: @"A range track bar allows you to specify a range of values.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a range track bar, allowing a range of values to be specified.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "CalculatorModule",
					displayName: @"Calculator",
					group: "Editors",
					type: "EditorsDemo.CalculatorModule",
					shortDescription: @"Two variations of a calculator control can be used to perform arithmetic operations.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the <Span FontWeight=""Bold"">Calculator</Span> and <Span FontWeight=""Bold"">PopupCalcEdit</Span> controls, which represent the calculator. Their features include:
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Basic Math Functions </Span> (add, subtract, multiply, etc.)
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Memory Operations </Span> - store, recall, add (M+) and subtract (M-)
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Keyboard and Mouse Wheel Support </Span>
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Configurable Display Format </Span> (currency, percentage, etc.)
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Calculation History </Span>
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Localizable UI </Span>
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ColorEditModule",
					displayName: @"ColorEdit",
					group: "Editors",
					type: "EditorsDemo.ColorEditModule",
					shortDescription: @"Color picker controls allow you to select from the standard color palette or from the RGBA color space.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the <Span FontWeight=""Bold"">ColorEdit</Span> and <Span FontWeight=""Bold"">PopupColorEdit</Span> editors. These controls allow an end-user to choose colors from the RGBA color space. Their features include:
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • 20 Preset Color Palettes </Span> - Apex, Aspect, Civic, Office, Grayscale, Urban, etc.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Standard Colors Palette </Span> - includes standard colors (e.g. Red, Green, Yellow, etc.)
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Recent Colors Palette </Span> - includes custom colors which can be added via the More Colors dialog.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Unlimited Number of Color Palettes </Span> - by default, the editor displays three color palettes: Theme Colors, Gradient Theme Colors and Standard Colors. You can replace default palettes or append any number of custom color palettes. To create a color palette, create a new instance of the CustomPalette class and define the colors. Use the ColorPalette.CreateGradientPalette static method to create a palette with gradient colors. To display a custom palette within the editor, add it to the Palettes collection.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Configurable Palette Layout </Span> - you can manually specify the number of color columns. Alternatively, the number of columns is automatically calculated.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Customizable Default Color </Span> - you can specify the default color which is applied by clicking the Automatic button.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Optional Empty Color </Span> - you can specify the default color which is applied by clicking the No Color button
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Localizable UI </Span>
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "PasswordBoxEditModule",
					displayName: @"PasswordBoxEdit",
					group: "Editors",
					type: "EditorsDemo.PasswordBoxEditModule",
					shortDescription: @"A Password Box editor is designed to handle password entry.",
					description: @"
                        <Paragraph>
                        The <Span FontWeight=""Bold"">PasswordBoxEdit</Span> and control is designed for entering and handling passwords. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Configurable Password Char </Span> – you can specify any character which is echoed in the password box.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Maximum Password Length </Span> – restricts passwords typed by an end-user.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Input Validation </Span>
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ImageEditModule",
					displayName: @"ImageEdit",
					group: "Editors",
					type: "EditorsDemo.ImageEditModule",
					shortDescription: @"Image controls allow you to quickly load and display images.",
					description: @"
                        <Paragraph>
                        This demo shows two controls used to display images. The <Span FontWeight=""Bold"">ImageEdit</Span> represents a standalone image. The <Span FontWeight=""Bold"">PopupImageEdit</Span> control displays an image within a dropdown window. Their features include:
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Supported Image Formats </Span> - Bitmap, JPEG, GIF and PNG.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Configurable Image Size </Span> - an image can be resized to fill the image box, stretched or shrunk to fit the size of the box.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Built-in Menu </Span> - end-users can cut, copy, paste, delete, load or save an image. It’s also possible to remove the default commands and/or add custom commands. This menu is automatically shown when the mouse pointer enters the image, and is automatically hidden when the mouse pointer leaves the image.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Easy Image Loading </Span> - clicking within an image editor shows the Open dialog allowing you to select an image.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Auto-Sizing Image Popup </Span> - if the size of an image is smaller than the size of an editor’s popup window, the image popup is automatically resized to fit the image size.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Localizable UI </Span>
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Webcam support </Span> - you can click the Take Picture From Camera button within the Image Menu to show a webcam snapshot within the image editor.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V152
				),
				new WpfModule(demo,
					name: "SearchControlModule",
					displayName: @"SearchControl",
					group: "Controls",
					type: "EditorsDemo.SearchControlModule",
					shortDescription: @"Delivers an easy and straightforward way for end-users to locate information within data editors.",
					description: @"
                        <Paragraph>
                        <Span FontWeight=""Bold"">DevExpress WPF Search Control</Span> delivers an easy and straightforward way for end-users to locate information within data editors (list boxes, combo boxes, etc).
                        </Paragraph>
                        <Paragraph>
                        To execute a search, enter a text within the Find box and data editors linked to the <Span FontWeight=""Bold"">Search Control</Span> will display records with matching values.
                        </Paragraph>
                        <Paragraph>
                        Various options are available to control the display and behavior of the <Span FontWeight=""Bold"">Search Control</Span>. You can specify search columns, choose between automatic and manual search modes, optionally display control buttons and much more.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RangeControlAndScheduler",
					displayName: @"RangeControl and Scheduler",
					group: "Controls",
					type: "EditorsDemo.RangeControlAndScheduler",
					shortDescription: @"Delivers an easy and straightforward way for end-users to locate information within scheduler.",
					description: @"
                        <Paragraph>
                        This demo shows how to integrate the Range Control with the Scheduler Control and display appointments that match the specified period of time.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RangeControlDateSelectorModule",
					displayName: @"RangeControl - Date Selector",
					group: "Controls",
					type: "EditorsDemo.RangeControlDateSelectorModule",
					shortDescription: @"This demos shows how to use the Range Control to allow users choose a range of dates.",
					description: @"
                        <Paragraph>
                        In this demo, the Range Control represents the Date Range Selector. This is the perfect choice when your interface requires that users specify a range of date values.
                        With its smooth animated scrolling and zooming capabilities, and its ability to auto determine the appropriate information to display based on the scale factor, you can build modern UI applications and target Windows 8 and the Surface tablet using the WPF platform.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "ChartClientForRangeControlModule",
					displayName: @"Chart Client for Range Control",
					group: "Controls",
					type: "EditorsDemo.ChartClientForRangeControlModule",
					shortDescription: @"This demo shows a range control that is visualized by the date-time and numeric chart clients.",
					description: @"
                        <Paragraph>
                        This demo illustrates how to use date-time and numeric chart clients of the range control to paint chart data within the range control's viewport.
                        You can customize grid spacing in each chart view by using track bars in the numeric and date-time chart settings.   To change the date-time grid alignment, use the list displayed in the date-time chart settings.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "RangeControlFilteringModule",
					displayName: @"RangeControl Filtering",
					group: "Controls",
					type: "EditorsDemo.RangeControlFilteringModule",
					shortDescription: @"This demos shows how to filter data displayed within the grid using RangeControl.",
					description: @"
                        <Paragraph>
                        In this demo, the Range Control is used to filter issues displayed within the grid by dates. The grid displays only those issues that were created during the specified period of time. This period of time is defined by the Range Control.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "DateNavigatorModule",
					displayName: @"DateNavigator",
					group: "Controls",
					type: "EditorsDemo.DateNavigatorModule",
					shortDescription: @"This demo shows the DateNavigator.",
					description: @"
                        <Paragraph>
                        This demo demonstrates the DateNavigator control that allows end-users to navigate in a calendar and choose a day or range of days. Its features include:
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Multiple Calendar Views.</Span> Clicking the navigation pane on top of the date navigator changes a calendar view to display date ranges of different scope (Month, Year, Years and Range of Years).
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Multiple Monthly or Yearly Calendars Onscreen.</Span> The number of monthly or yearly calendars shown simultaneously in a date navigator is changed automatically according to the control's current size. However, you can explicitly specify the number of calendar rows and columns.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Holidays and Special Days.</Span> Saturdays and Sundays are default holidays in the date navigator. They are marked in red. You can add and remove individual days to a collection of custom holidays. In addition, the DateNavigator control provides you with the capability to mark individual days as special. These days are painted in bold.
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Workdays and Exact Workdays.</Span> By default, days from Monday through Friday are displayed as workdays. You can set workdays as you wish. In addition, DateNavigator allows you to strictly specify workdays (that will have priority over holidays).
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Selection of Multiple Days</Span>
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Optional  Week Numbers</Span>
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Optional Today Button</Span>
                        </Paragraph>
                        <Paragraph>
                        <Span FontWeight=""Bold""> <Run Text="" ""/> • Current Date Indication</Span>
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true
				),
				new WpfModule(demo,
					name: "DXMessageBoxModule",
					displayName: @"DXMessageBox",
					group: "Controls",
					type: "EditorsDemo.DXMessageBoxModule",
					shortDescription: @"The DXMessageBox represents a window that supports DevExpress WPF themes.",
					description: @"
                        <Paragraph>
                        This demo demonstrates a message box with customizable floating mode that can contain text, caption, buttons, icons, etc.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "DXWindowBorderHighlightingEffects",
					displayName: @"DXWindow Border Highlighting Effects",
					group: "Controls",
					type: "EditorsDemo.DXWindowBorderHighlightingEffects",
					shortDescription: @"This demo illustrates the border highlighting effects available for the DXWindow.",
					description: @"
                        <Paragraph>
                        When border highlighting is enabled, the window's border is displayed with either the shadow or glow effect, depending on the selected paint theme.
                        You can customize the colors used to paint the border highlighting effect using dedicated options.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "FlyoutControlModule",
					displayName: @"FlyoutControl",
					group: "Controls",
					type: "EditorsDemo.FlyoutControlModule",
					shortDescription: @"This demo shows the Flyout control. It can be represented by a hint or popup panel displayed to either side of the screen.",
					description: @"
                        <Paragraph>
                        This demo shows the Flyout control. It can be represented by a hint or popup panel displayed to either side of the screen.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142
				),
				new WpfModule(demo,
					name: "BindingToCollectionViewModule",
					displayName: @"Binding to CollectionView",
					group: "Data Binding",
					type: "EditorsDemo.BindingToCollectionViewModule",
					shortDescription: @"This demo shows how to bind data editors to ICollectionView.",
					description: @"
                        <Paragraph>
                        DevExpress WPF ComboBoxEdit, LookUpEdit, SearchEdit and ListBoxEdit controls now include support for ICollectionViews. You can manipulate current record and define rules for filtering, sorting and grouping.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "ListBoxEdit",
							type: WpfModuleLinkType.Demos,
							url: "local:ListBoxEdit"),
						new WpfModuleLink(
							title: "ComboBoxEdit",
							type: WpfModuleLinkType.Demos,
							url: "local:ComboBoxEdit"),
						new WpfModuleLink(
							title: "SearchLookUpEdit",
							type: WpfModuleLinkType.Demos,
							url: "local:SearchLookUpEdit"),
						new WpfModuleLink(
							title: "DXGrid demo: Collection View",
							type: WpfModuleLinkType.Demos,
							url: "GridDemo:Collection View")
					}
				),
				new WpfModule(demo,
					name: "NumericMaskEdit",
					displayName: @"Numeric",
					group: "Masked Input",
					type: "EditorsDemo.NumericMaskEdit",
					shortDescription: @"Numeric masks simplify the input of numeric values (currency, integer, float, etc).",
					description: @"
                        <Paragraph>
                        The Numeric mask type is specifically designed for entering numeric values (integer, float values, currencies, percentages, etc). Some specific numeric masks are dependent upon the current culture (regional) settings. The product has many predefined masks that allow numeric values to be entered using various common patterns, and, in addition, if you need to, you can create custom patterns.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Masked Input Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6945"),
						new WpfModuleLink(
							title: "Date-Time Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Date-Time"),
						new WpfModuleLink(
							title: "RegEx Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:RegEx"),
						new WpfModuleLink(
							title: "Simple Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Simple"),
						new WpfModuleLink(
							title: "Regular Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Regular")
					}
				),
				new WpfModule(demo,
					name: "DateTimeMaskEdit",
					displayName: @"Date-Time",
					group: "Masked Input",
					type: "EditorsDemo.DateTimeMaskEdit",
					shortDescription: @"DateTime masks accept only date-time values using common date-time patterns.",
					description: @"
                        <Paragraph>
                        Masks of this type significantly simplify the input of date/time values. The product has many predefined masks that can be used to enter values using common date/time patterns. It's also possible to create custom masks that specify which parts (year, month, day, hour, minute, etc) of a date/time value can be edited.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Masked Input Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6945"),
						new WpfModuleLink(
							title: "Numeric Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Numeric"),
						new WpfModuleLink(
							title: "RegEx Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:RegEx"),
						new WpfModuleLink(
							title: "Simple Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Simple"),
						new WpfModuleLink(
							title: "Regular Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Regular")
					}
				),
				new WpfModule(demo,
					name: "RegExMaskEdit",
					displayName: @"RegEx",
					group: "Masked Input",
					type: "EditorsDemo.RegExMaskEdit",
					shortDescription: @"In this mode you can construct masks using full functional regular expressions.",
					description: @"
                        <Paragraph>
                        The RegEx mask mode allows you to use full regular expressions to create masks, which in turn, gives you the greatest flexibility in controlling data input. You can control the number of characters in a valid input string, specify a subset of characters to be entered at a specific position, define alternative forms, and so on.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Masked Input Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6945"),
						new WpfModuleLink(
							title: "Date-Time Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Date-Time"),
						new WpfModuleLink(
							title: "Numeric Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Numeric"),
						new WpfModuleLink(
							title: "Simple Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Simple"),
						new WpfModuleLink(
							title: "Regular Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Regular")
					}
				),
				new WpfModule(demo,
					name: "SimpleMaskEdit",
					displayName: @"Simple",
					group: "Masked Input",
					type: "EditorsDemo.SimpleMaskEdit",
					shortDescription: @"Simple masks accept strings which have a fixed format and length.",
					description: @"
                        <Paragraph>
                        Masks of the Simple type are basic masks. Although they do not allow you to implement sophisticated data input patterns, they can still be useful if an editor should accept a string that has a fixed format and is of a fixed length (for instance, a phone number, zip code, etc).
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,	
					links: new[] {
						new WpfModuleLink(
							title: "Masked Input Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6945"),
						new WpfModuleLink(
							title: "Date-Time Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Date-Time"),
						new WpfModuleLink(
							title: "Numeric Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Numeric"),
						new WpfModuleLink(
							title: "RegEx Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:RegEx"),
						new WpfModuleLink(
							title: "Regular Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Regular")
					}
				),
				new WpfModule(demo,
					name: "RegularMaskEdit",
					displayName: @"Regular",
					group: "Masked Input",
					type: "EditorsDemo.RegularMaskEdit",
					shortDescription: @"In this mode masks use the simplified regular expression syntax.",
					description: @"
                        <Paragraph>
                        In this mode, masks use a simplified regular expression syntax. Such masks can be used when you need to specify a custom range of characters for a specific position, and when you need to provide a mask of variable length. However, they don't support alternative valid forms for values, and they also lack the auto-complete feature.
                        </Paragraph>",
					addedIn: KnownDXVersion.Before142,
					isFeatured: true,	
					links: new[] {
						new WpfModuleLink(
							title: "Masked Input Overview",
							type: WpfModuleLinkType.Documentation,
							url: "http://documentation.devexpress.com/#WPF/CustomDocument6945"),
						new WpfModuleLink(
							title: "Date-Time Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Date-Time"),
						new WpfModuleLink(
							title: "Numeric Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Numeric"),
						new WpfModuleLink(
							title: "RegEx Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:RegEx"),
						new WpfModuleLink(
							title: "Simple Mask Input",
							type: WpfModuleLinkType.Demos,
							url: "local:Simple")
					}
				),
				new WpfModule(demo,
					name: "BarCode2D",
					displayName: @"BarCode 2D",
					group: "Controls",
					type: "EditorsDemo.BarCode2D",
					shortDescription: @"This example demonstrates four types of two-dimensional barcodes created using the BarCodeEdit Control and allows you to set barcode values and customize related display options. ",
					description: @"
                ",
					allowDarkThemes: false,
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				),
				new WpfModule(demo,
					name: "LinearBarCode",
					displayName: @"Linear BarCode",
					group: "Controls",
					type: "EditorsDemo.LinearBarCode",
					shortDescription: @"This example demonstrates one dimensional barcodes created using the BarCodeEdit. The options on the right allow you to switch between individual barcode types, set barcode values and change corresponding display options. ",
					description: @"
                ",
					allowDarkThemes: false,
					allowSwitchingThemes: false,
					addedIn: KnownDXVersion.Before142,
					updatedIn: KnownDXVersion.V142
				)
			};
		}
	}
}
