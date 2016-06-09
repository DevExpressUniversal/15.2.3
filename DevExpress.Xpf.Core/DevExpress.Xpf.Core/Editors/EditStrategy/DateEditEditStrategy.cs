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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Services;
using DevExpress.Xpf.Editors.Validation.Native;
#if !SL
using DevExpress.Xpf.Editors.Validation;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Core;
#endif
namespace DevExpress.Xpf.Editors {
	public class DateEditStrategy : RangeEditorStrategy<DateTime> {
		public override bool ShouldRoundToBounds { get { return Editor.AllowRoundOutOfRangeValue; } }
		new DateEdit Editor { get { return (DateEdit)base.Editor; } }
		public DateEditStrategy(DateEdit editor)
			: base(editor) {
		}
		protected override MinMaxUpdateHelper CreateMinMaxHelper() {
			return new MinMaxUpdateHelper(Editor, DateEdit.MinValueProperty, DateEdit.MaxValueProperty);
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(DateEdit.DateTimeProperty, baseValue => baseValue, Correct);
		}
		protected internal override DateTime GetMinValue() {
			if(Editor.MinValue.HasValue)
				return Editor.MinValue.Value;
			return DateTime.MinValue;
		}
		protected internal override DateTime GetMaxValue() {
			if(Editor.MaxValue.HasValue)
				return Editor.MaxValue.Value;
			return DateTime.MaxValue;
		}
		protected override object GetDefaultValue() {
			return DateTime.Today;
		}
		public override object CoerceMaskType(MaskType maskType) {
			return maskType == MaskType.DateTime || maskType == MaskType.DateTimeAdvancingCaret ? maskType : MaskType.DateTime;
		}
		public virtual object CoerceDateTime(object baseValue) {
			return CoerceValue(DateEdit.DateTimeProperty, CreateValueConverter(baseValue).Value);
		}
		public virtual void DateTimeChanged(DateTime oldDateTime, DateTime dateTime) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(DateEdit.DateTimeProperty, oldDateTime, dateTime);
		}
		public void UpdateCalendarShowClearButton() {
			if(Editor.Calendar != null)
				Editor.Calendar.ShowClearButton = AllowKeyHandling && !Editor.IsReadOnly && (Editor.ShowClearButton && Editor.AllowNullInput);
		}
		public void SetDateTime(DateTime editValue, UpdateEditorSource updateEditorSource) {
			ValueContainer.SetEditValue(editValue, updateEditorSource);
			TextInputService.SetInitialEditValue(editValue);
		}
		protected override BaseEditingSettingsService CreateTextInputSettingsService() {
			return new PopupBaseEditSettingsService(Editor);
		}
		protected override EditorSpecificValidator CreateEditorValidatorService() {
			return new DateEditValidator(Editor);
		}
		protected override RangeEditorService CreateRangeEditService() {
			return new DateEditRangeService(Editor);
		}
		public virtual bool IsClosePopupWithAcceptGesture(Key key, ModifierKeys modifiers) {
			return key == Key.Enter;
		}
		public virtual bool IsClosePopupWithCancelGesture(Key key, ModifierKeys modifiers) {
			return false;
		}
	}
}
