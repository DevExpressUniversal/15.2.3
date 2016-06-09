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
		static List<Module> Create_XtraVerticalGrid_MainDemo_Modules(Demo demo) {
			return new List<Module> {
				new SimpleModule(demo,
					name: "About",
					displayName: @"DevExpress XtraVerticalGrid %MarketingVersion%",
					group: "About",
					type: "DevExpress.XtraVerticalGrid.Demos.About",
					description: @"",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "FindPanel",
					displayName: @"Find Panel",
					group: "Main",
					type: "DevExpress.XtraVerticalGrid.Demos.FindPanel",
					description: @"This demo illustrates the search panel for the DevExpress Vertical Grid control. This panel can be either constantly visible (hidden) or invoked via the Ctrl+F shortcut. The search panel is able to find the desired text within both the entire control and specific columns only. The search itself can be performed in two different modes - either on the 'Find' button click only or on the fly, a certain period of time after an end-user has entered a text. You can use the corresponding editors above the search panel to select the desired search mode and delay, specify the target search columns, set whether the found text should be highlighted etc.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "FixedRows",
					displayName: @"Fixed Rows",
					group: "Main",
					type: "DevExpress.XtraVerticalGrid.Demos.FixedRows",
					description: @"Vertical Grid rows can be anchored to the top or bottom edge. Such rows are not vertically scrolled when scrolling the control. Practice scrolling the Vertical Grid in this example, where three rows are anchored to the top edge. Right-click a row's header to fix the row via a popup menu.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "UnboundExpressions",
					displayName: @"Unbound Expressions",
					group: "Main",
					type: "DevExpress.XtraVerticalGrid.Demos.UnboundExpressions",
					description: @"This example demonstrates the unbound expression feature that allows you to populate unbound rows with data using formulas (string expressions). Values of the Total and Total Amount rows are calculated according to specific expressions. You can edit these expressions via the Expression Editor, which can be invoked by selecting the Expression Editor command from the row header's context menu.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Layout",
					displayName: @"Layout",
					group: "Main",
					type: "DevExpress.XtraVerticalGrid.Demos.Layout",
					description: @"This example illustrates the available layout styles provided by the VerticalGridControl. Try switching options within the Layout Style group to see how this affects the appearance of the control. The demo also shows how the control is used in various data binding modes.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "PropertyGrid",
					displayName: @"Property Grid",
					group: "Main",
					type: "DevExpress.XtraVerticalGrid.Demos.PropertyGrid",
					description: @"This demo illustrates the PropertyGridControl which is designed to be used to display the properties of any object. To select the control whose settings are listed in the PropertyGridControl use the ComboBox or just click the required control on the form. Pay attention to the 'Use Default Editors Collection' option. If the option is checked, the control uses custom editors from its DefaultEditors collection to represent data of a specific type (for instance, a CheckEdit editor is used to edit Boolean values in this instance). You can also use the embedded find panel to search for specific properties by their names.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "UnboundField",
					displayName: @"Unbound Data Rows",
					group: "Main",
					type: "DevExpress.XtraVerticalGrid.Demos.UnboundField",
					description: @"This demo illustrates rows that are not bound to the data source. To provide their values, handle the CustomUnboundData event. Since this event was designed to handle both get and set requests, you can make an unbound row editable.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Formats",
					displayName: @"Formats",
					group: "Outdated",
					type: "DevExpress.XtraVerticalGrid.Demos.Formats",
					description: @"The XtraVerticalGrid Suite comes with multiple paint schemes which allow you to change the appearance of a control on the fly, as selecting a scheme automatically changes the appearance of the control in its entirety. Note that you can apply a paint scheme to the vertical grid control at design time using the control's Designer.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Options",
					displayName: @"Options",
					group: "Miscellaneous",
					type: "DevExpress.XtraVerticalGrid.Demos.Options",
					description: @"In this example multiple grid options are demonstrated. Experiment with the View and Behavior options and see how this affects the control's functionality.",
					addedIn: KnownDXVersion.Before142
				),
				new SimpleModule(demo,
					name: "Customize",
					displayName: @"Row Customization",
					group: "Miscellaneous",
					type: "DevExpress.XtraVerticalGrid.Demos.Customize",
					description: @"This demo illustrates the runtime row customization feature implemented by the vertical grid controls. This feature is enabled when the Customization Form is visible. In customization mode, end-users are able to access hidden rows which are listed in the Customization Form and make them visible using drag and drop. Or they can drag and so hide specific visible rows to the form. In addition it's possible to create custom categories.",
					addedIn: KnownDXVersion.Before142
				)
			};
		}
	}
}
