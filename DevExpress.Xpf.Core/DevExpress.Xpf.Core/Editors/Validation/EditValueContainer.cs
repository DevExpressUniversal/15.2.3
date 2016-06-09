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
using System.Windows;
using System.Windows.Threading;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
#if !SL
using DevExpress.Xpf.Editors.Validation.Native;
using System.Globalization;
using DevExpress.Xpf.Editors.Helpers;
#else
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Editors.Validation {
	class NullableContainer {
		public object Value { get; private set; }
		public bool HasValue { get; private set; }
		public void SetValue(object value) {
			Value = value;
			HasValue = true;
		}
		public void Reset() {
			Value = null;
			HasValue = false;
		}
	}
	public class EditValueContainer {
		Locker editValueChanging = new Locker();
		Locker postponedValueChanging = new Locker();
		NullableContainer editValueCandidate = new NullableContainer();
		NullableContainer tempEditValue = new NullableContainer();
		DispatcherTimer postTimer;
		public PostMode PostMode { 
			get {
#if DEBUGTEST
				if (PostModeHelper.SuppressDelayedPost)
					return PostMode.Immediate;
#endif
				return Editor.EditValuePostMode;
			} 
		}
		public bool IsLockedByValueChanging { get { return editValueChanging.IsLocked; } }
		public bool IsPostponedValueChanging { get { return postponedValueChanging.IsLocked; } }
		BaseEdit Editor { get; set; }
		protected EditStrategyBase Strategy { get { return Editor.EditStrategy; } }
		DispatcherTimer PostTimer { get { return postTimer ?? (postTimer = CreatePostTimer()); } }
		DispatcherTimer CreatePostTimer() {
#if !SL
			DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Send);
#else
			DispatcherTimer timer = new DispatcherTimer();
#endif
			timer.Tick += timer_Tick;
			timer.Interval = TimeSpan.FromMilliseconds(Editor.EditValuePostDelay);
			return timer;
		}
		void timer_Tick(object sender, EventArgs e) {
			FlushEditValuePostpone();
			StopPostTimer();
		}
		void StartPostTimer() {
			if (PostTimer.IsEnabled)
				StopPostTimer();
			postponedValueChanging.Lock();
			PostTimer.Start();
		}
		void StopPostTimer() {
			if (!PostTimer.IsEnabled)
				return;
			PostTimer.Stop();
			postponedValueChanging.Unlock();
		}
		void FlushEditValuePostpone() {
			postponedValueChanging.DoLockedAction(() => {
				FlushEditValueImmediate();
#if !SL
				System.Windows.Input.CommandManager.InvalidateRequerySuggested();
#endif
			});
		}
		void FlushEditValueImmediate() {
			if (HasTempValue)
				BaseEditHelper.SetCurrentValue(Editor, BaseEdit.EditValueProperty, TempEditValue);
			ResetTempValue();
		}
		public void FlushEditValue() {
			FlushEditValueImmediate();
			ResetPostTimer();
		}
		public void UndoTempValue() {
			ResetPostTimer();
			ResetTempValue();
		}
		public object EditValue {
			get {
				if (HasValueCandidate)
					return EditValueCandidate;
				return HasTempValue ? TempEditValue : Editor.PropertyProvider.EditValue;
			}
		}
		public object TempEditValue { 
			get { return tempEditValue.HasValue ? tempEditValue.Value : EditValue; }
			set { tempEditValue.SetValue(value);}
		}
		public object EditValueCandidate { get { return editValueCandidate.Value; } }
		public bool HasValueCandidate { get { return editValueCandidate.HasValue; } }
		public bool HasTempValue { get { return tempEditValue.HasValue; } }
		public UpdateEditorSource UpdateSource { get; private set; }
		public void FlushEditValueCandidate(object editValue, UpdateEditorSource updateSource) {
			if (!HasValueCandidate) {
				FlushEditValue();
				return;
			}
			object value;
			if (Strategy.ProvideEditValue(editValue, out value, updateSource)) {
				SetEditValueInternal(value, updateSource);
				FlushEditValue();
			}
		}
		public EditValueContainer(BaseEdit editor) {
			Editor = editor;
		}
		public void Reset() {
			editValueCandidate.Reset();
		}
		void ResetTempValue() {
			tempEditValue.Reset();
		}
		void PostEditValueInternal(object editValue, UpdateEditorSource updateSource) {
			UpdateSource = updateSource;
			if (PostMode == PostMode.Immediate || updateSource != UpdateEditorSource.TextInput) {
				BaseEditHelper.SetCurrentValue(Editor, BaseEdit.EditValueProperty, editValue);
				return;
			}
			TempEditValue = editValue;
			StartPostTimer();
		}
		void PostEditValue(object value, UpdateEditorSource updateSource) {
			editValueChanging.DoLockedActionIfNotLocked(() => PostEditValueInternal(value, updateSource));
		}
		public bool SetEditValue(object value, UpdateEditorSource updateSource) {
			editValueCandidate.SetValue(value);
			if (Strategy.DoValidateInternal(value, updateSource)) {
				object provideValue;
				bool result = Strategy.ProvideEditValue(value, out provideValue, updateSource);
				if (result)
					SetEditValueInternal(provideValue, updateSource);
				return result;
			}
			return false;
		}
		void SetEditValueInternal(object value, UpdateEditorSource updateSource) {
			Reset();
			if (!object.Equals(Strategy.ConvertToBaseValue(EditValue), Strategy.ConvertToBaseValue(value))) {
				Strategy.ResetValidationError();
				PostEditValue(value, updateSource);
			}
		}
		public void Update(UpdateEditorSource updateSource) {
			SetEditValue(EditValue, updateSource);
		}
		public bool GetIsValid(UpdateEditorSource updateSource) {
			if (updateSource == UpdateEditorSource.ValueChanging || updateSource == UpdateEditorSource.DoValidate)
				return false;
			return HasValueCandidate && object.Equals(EditValueCandidate, Editor.EditValue);
		}
		public void UpdatePostMode() {
			ResetPostTimer();
		}
		void ResetPostTimer() {
			if (postTimer == null)
				return;
			StopPostTimer();
			postTimer.Tick -= timer_Tick;
			postTimer = null;
		}
	}
}
