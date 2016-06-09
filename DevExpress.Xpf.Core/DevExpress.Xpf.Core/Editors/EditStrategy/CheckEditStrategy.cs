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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
#if !SL
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Media;
using DevExpress.Xpf.Editors.Helpers;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation.Native;
#endif
#if SL
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DevExpress.Xpf.Bars;
#endif
namespace DevExpress.Xpf.Editors {
	public partial class CheckEditStrategy : EditStrategyBase {
		new CheckEditSettings Settings { get { return base.Settings as CheckEditSettings; } }
		ToggleButton CheckBox { get { return ((CheckEdit)Editor).CheckBox; } }
		new CheckEdit Editor { get { return (CheckEdit)base.Editor; } }
		public virtual bool IsEnabledCore { get; private set; }
		public bool IsInsideListBoxItem { get { return LayoutHelper.FindParentObject<ListBoxEditItem>(Editor.Parent as Visual) != null; } }
		public CheckEditStrategy(CheckEdit editor)
			: base(editor) {
			IsEnabledCore = true;
		}
		protected internal override bool ShouldProcessNullInput(KeyEventArgs e) {
			return base.ShouldProcessNullInput(e) && Editor.IsThreeState;
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			UpdateCheckBoxValue();
		}
		protected override void SyncEditCorePropertiesInternal() {
			base.SyncEditCorePropertiesInternal();
			UpdateCheckBoxValue();
		}
		protected virtual void UpdateCheckBoxValue() {
			if(CheckBox != null)
				CheckBox.IsChecked = CheckEditSettings.GetBooleanFromEditValue(ValueContainer.EditValue, Editor.IsThreeState);
		}
		protected override void SyncWithEditorInternal() {
			ValueContainer.SetEditValue(CheckBox.IsChecked, UpdateEditorSource.ValueChanging);
			base.SyncWithEditorInternal();
		}
		public override void RaiseValueChangedEvents(object oldValue, object newValue) {
			base.RaiseValueChangedEvents(oldValue, newValue);
			if(ShouldLockRaiseEvents)
				return;
			if(Editor.IsChecked == true)
				Editor.OnChecked(new RoutedEventArgs(CheckEdit.CheckedEvent));
			else if(Editor.IsChecked == false)
				Editor.OnUnchecked(new RoutedEventArgs(CheckEdit.UncheckedEvent));
			else
				Editor.OnIndeterminate(new RoutedEventArgs(CheckEdit.IndeterminateEvent));
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(CheckEdit.IsCheckedProperty, baseValue => baseValue, (baseValue) => CheckEditSettings.GetBooleanFromEditValue(baseValue, Editor.IsThreeState));
		}
		public void PerformToggle() {
			if(!AllowEditing || !AllowKeyHandling)
				return;
			bool? editValue = CheckEditSettings.GetBooleanFromEditValue(ValueContainer.EditValue, Editor.IsThreeState);
			if (editValue == true)
				ValueContainer.SetEditValue(Editor.IsThreeState ? null : (bool?)false, UpdateEditorSource.TextInput);
			else
				ValueContainer.SetEditValue(!Editor.IsThreeState || editValue.HasValue, UpdateEditorSource.TextInput);
		}
		bool IsMouseLeftButtonReleased {
			get { return MouseHelper.IsMouseLeftButtonReleased(null); }
		}
		bool ProcessIsMouseOverChanged() {
			if(Editor.ClickMode == ClickMode.Hover && Editor.IsMouseOver) {
				OnClick();
				return true;
			}
			return false;
		}
		bool isToggleKeyDown = false;
		protected internal bool IsToggleKeyDown {
			get { return isToggleKeyDown; }
			set {
				if (value == isToggleKeyDown) return;
				isToggleKeyDown = value;
#if SL
				IsToggleKeyDownChanged();
#endif
			}
		}
		void CaptureMouse() {
			Editor.CaptureMouse();
		}
		void UncaptureMouse() {
			Editor.ReleaseMouseCapture();
		}
		protected virtual void OnClick() {
			PerformToggle();
			RaiseClickEvent();
			ExecuteCommand();
		}
		void RaiseClickEvent() {
#if !SL
			Editor.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, Editor));
#endif
		}
		public virtual void ExecuteCommand() {
			ICommand command = Editor.Command;
			if (command == null)
				return;
			if (command.CanExecute(Editor.CommandParameter))
				command.Execute(Editor.CommandParameter);
		}
		bool IsToggleCheckGesture(Key key) {
			return !Editor.IsReadOnly && Editor.IsEnabled && (key == Key.Space
#if !SL
 || ((bool)Editor.GetValue(KeyboardNavigation.AcceptsReturnProperty) && key == Key.Enter)
#endif
);
		}
		public bool ProcessKeyDown(Key key) {
			bool result = false;
			if(Editor.ClickMode != ClickMode.Hover && IsMouseLeftButtonReleased) {
				if(IsToggleCheckGesture(key)) {
					result = true;
					if(!IsToggleKeyDown) {
						IsToggleKeyDown = true;
						CaptureMouse();
						if(Editor.ClickMode == ClickMode.Press)
							OnClick();
					}
				}
			}
			return result;
		}
		public bool ProcessKeyUp(Key key) {
			bool result = false;
			if(Editor.ClickMode != ClickMode.Hover && IsMouseLeftButtonReleased)
				if(IsToggleCheckGesture(key)) {
					result = true;
					IsToggleKeyDown = false;
					UncaptureMouse();
					if(Editor.ClickMode == ClickMode.Release)
						OnClick();
				}
			return result;
		}
		public bool ProcessMouseLeftButtonDown() {
#if SL
			SLProcessPreviewMouseLeftButtonDown();
#endif
			bool result = false;
			if(Editor.EditMode != EditMode.InplaceInactive && Editor.ClickMode != ClickMode.Hover) {
				result = !IsInsideListBoxItem;
				CaptureMouse();
#if SL
				this.capturedValue = Editor.IsChecked;
#endif
				if(Editor.ClickMode == ClickMode.Press)
					OnClick();
			}
			return result;
		}
#if SL
		bool? capturedValue;
#endif
		public bool ProcessMouseLeftButtonUp() {
			if (Editor.EditMode == EditMode.InplaceInactive)
				return false;
			bool result = false;
			if (Editor.ClickMode != ClickMode.Hover) {
				result = !IsInsideListBoxItem;
				bool isCaptured = IsMouseCaptured;
				if (!IsToggleKeyDown)
					UncaptureMouse();
				if (Editor.ClickMode == ClickMode.Release && Editor.IsMouseOver && isCaptured
#if SL
					&& this.capturedValue == Editor.IsChecked
#endif
					)
					OnClick();
			}
			return result;
		}
		bool IsMouseCaptured {
#if !SL
			get { return Editor.IsMouseCaptured; }
#endif
#if SL
			get { return Editor.HasMouseCapture; }
#endif
		}
		public bool ProcessMouseEnter() {
			return ProcessIsMouseOverChanged();
		}
		public bool ProcessMouseLeave() {
			return ProcessIsMouseOverChanged();
		}
		protected internal override void ProcessEditModeChanged(EditMode oldValue, EditMode newValue) {
			base.ProcessEditModeChanged(oldValue, newValue);
			if(newValue == EditMode.InplaceInactive)
				UncaptureMouse();
		}
		protected internal virtual void IsCheckedChanged(bool? oldValue, bool? value) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(CheckEdit.IsCheckedProperty, oldValue, value);
		}
		public virtual void UpdateCanExecute(ICommand newValue) {
			IsEnabledCore = newValue == null || CommandHelper.CanExecuteCommandSource(Editor);
			Editor.UpdateIsEnabledCore();
		}
		protected override string FormatDisplayTextFast(object editValue) {
			bool? value = CheckEditSettings.GetBooleanFromEditValue(editValue, Editor.IsThreeState);
			if (value == null)
				return EditorLocalizer.GetString(EditorStringId.CheckIndeterminate);
			return value.Value ? EditorLocalizer.GetString(EditorStringId.CheckChecked) : EditorLocalizer.GetString(EditorStringId.CheckUnchecked);
		}
	}
	internal class CommandHelper {
		internal static bool CanExecuteCommandSource(ICommandSource commandSource) {
			ICommand command = commandSource.Command;
			if (command == null)
				return false;
			object commandParameter = commandSource.CommandParameter;
#if !SL
			IInputElement target = commandSource.CommandTarget;
			RoutedCommand routedCommand = command as RoutedCommand;
			if (routedCommand == null)
#endif
				return command.CanExecute(commandParameter);
#if !SL
			if (target == null)
				target = commandSource as IInputElement;
			return routedCommand.CanExecute(commandParameter, target);
#endif
		}
	}
}
