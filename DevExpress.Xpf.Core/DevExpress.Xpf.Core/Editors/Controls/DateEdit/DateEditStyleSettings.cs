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
using System.Linq;
using System.Text;
using System.Windows;
using System.Collections;
using System.Windows.Controls.Primitives;
using System.Windows.Controls;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.ComponentModel;
using System.Windows.Automation.Peers;
using DevExpress.Xpf.Utils;
#if !SL
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Popups.Calendar;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Editors.Automation;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Popups.Calendar;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
using DateTimeTypeConverter = DevExpress.Xpf.Core.DateTimeTypeConverter;
#endif
#if !SL
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
#else
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PopupFooterButtonsType = DevExpress.Xpf.Editors.PopupFooterButtons;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Editors.Helpers;
#endif
namespace DevExpress.Xpf.Editors {
	public enum DateEditPopupContentType {
#if !SL
		DateTimePicker,
#endif
		Calendar
	}
	public abstract class DateEditStyleSettingsBase : PopupBaseEditStyleSettings {
	}
#if !SL
	public class DateEditPickerStyleSettings : DateEditStyleSettingsBase {
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			var dateEditor = (DateEdit)editor;
			if(!dateEditor.IsPropertySet(PopupBaseEdit.FocusPopupOnOpenProperty))
				dateEditor.FocusPopupOnOpen = true;
			if(!dateEditor.IsPropertySet(PopupBaseEdit.PopupMaxHeightProperty) || double.IsPositiveInfinity(dateEditor.PopupMaxHeight))
				dateEditor.PopupMaxHeight = ScreenHelper.GetNearestScreenRect(ScreenHelper.GetScreenPoint(dateEditor)).Height / 3d;
			dateEditor.DateEditPopupContentType = DateEditPopupContentType.DateTimePicker;
		}
	}
#endif
	public class DateEditCalendarStyleSettings : DateEditStyleSettingsBase {
		public override void ApplyToEdit(BaseEdit editor) {
			base.ApplyToEdit(editor);
			var dateEditor = (DateEdit)editor;
			dateEditor.DateEditPopupContentType = DateEditPopupContentType.Calendar;
		}
		public override PopupFooterButtonsType GetPopupFooterButtons(PopupBaseEdit editor) {
			return PopupFooterButtonsType.None;
		}
	}
}
