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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Services;
#if !SL
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Internal;
#else
using DevExpress.Xpf.Editors.Validation.Native;
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
#endif
namespace DevExpress.Xpf.Editors {
	public abstract class RangeEditorStrategy<T> : TextEditStrategy where T : struct, IComparable {
		public abstract bool ShouldRoundToBounds { get; }
		readonly PostponedAction minMaxPropertyChangedAction;
		MinMaxUpdateHelper MinMaxUpdater { get; set; }
		Locker MinMaxChangedLocker { get; set; }
		new ButtonEdit Editor { get { return base.Editor as ButtonEdit; } }
		public RangeEditorStrategy(TextEdit editor)
			: base(editor) {
				MinMaxChangedLocker = new Locker();
			minMaxPropertyChangedAction = new PostponedAction(() => IsInSupportInitialize);
			MinMaxUpdater = CreateMinMaxHelper();
		}
		protected abstract MinMaxUpdateHelper CreateMinMaxHelper();
		protected internal virtual void MinValueChanged(T? value) {
			if(MinMaxChangedLocker)
				return;
			MinMaxUpdater.MinValue = new IComparableWrapper(GetMinValue(), value == null);
			using(MinMaxChangedLocker.Lock()) {
				minMaxPropertyChangedAction.PerformPostpone(() => MinMaxUpdater.Update<T>(MinMaxUpdateSource.MinChanged));
			}
		}
		protected internal virtual void MaxValueChanged(T? value) {
			if(MinMaxChangedLocker)
				return;
			MinMaxUpdater.MaxValue = new IComparableWrapper(GetMaxValue(), value == null);
			using(MinMaxChangedLocker.Lock()) {
				minMaxPropertyChangedAction.PerformPostpone(() => MinMaxUpdater.Update<T>(MinMaxUpdateSource.MaxChanged));
			}
		}
		public override void OnInitialized() {
			using (MinMaxChangedLocker.Lock()) {
				minMaxPropertyChangedAction.PerformForce(() => MinMaxUpdater.Update<T>(MinMaxUpdateSource.ISupportInitialize));
			}
			base.OnInitialized();
		}
		protected override object GetEditValueInternal() {
			object editValue = base.GetEditValueInternal();
			if(!Editor.AllowNullInput && IsNullValue(editValue))
				editValue = Correct(editValue);
			return editValue;
		}
		public override bool ProvideEditValue(object value, out object provideValue, UpdateEditorSource updateSource) {
			bool updateMaskManager = ShouldRoundToBounds && !InRange(value);
			if (updateMaskManager)
				value = Correct(value);
			base.ProvideEditValue(value, out provideValue, updateSource);
			if (updateMaskManager)
				TextInputService.SetInitialEditValue(provideValue);
			return true;
		}
		protected virtual bool ShouldCorrectEditValue() {
			return !InRange(ValueContainer.EditValue);
		}
		protected internal abstract T GetMinValue();
		protected internal abstract T GetMaxValue();
		public virtual void Maximize() {
			ValueContainer.SetEditValue(GetMaxValue(), UpdateEditorSource.TextInput);
		}
		public virtual void Minimize() {
			ValueContainer.SetEditValue(GetMinValue(), UpdateEditorSource.TextInput);
		}
		public virtual bool CanMaximize() {
			return true;
		}
		public virtual bool CanMinimize() {
			return true;
		}
		public bool InRange(object value) {
			if(Editor.AllowNullInput && IsNullValue(value))
				return true;
			RangeValueConverter<T> container = CreateValueConverter(value);
			return container.CompareTo(GetMinValue()) >= 0 && container.CompareTo(GetMaxValue()) <= 0;
		}
		public virtual object Correct(object baseValue) {
			return ShouldRoundToBounds ? ToRange(baseValue) : CreateValueConverter(baseValue).Value;
		}
		public virtual object ToRange(object baseValue) {
			RangeValueConverter<T> value = CreateValueConverter(baseValue);
			if (value.CompareTo(GetMinValue()) < 0)
				return GetMinValue();
			if (value.CompareTo(GetMaxValue()) > 0)
				return GetMaxValue();
			return value.Value;
		}
		public RangeValueConverter<T> CreateValueConverter(object value) {
			return new RangeValueConverter<T>(IsNullValue(value) ? null : value, () => IsNativeNullValue(GetNullableValue()) ? GetDefaultValue() : GetNullableValue(), GetDefaultValue);
		}
		protected internal override void PrepareForCheckAllowLostKeyboardFocus() {
			base.PrepareForCheckAllowLostKeyboardFocus();
			if (IsInSupportInitialize)
				return;
			if(InRange(ValueContainer.EditValue))
				return;
			TextInputService.SetInitialEditValue(ValueContainer.EditValue);
			ValueContainer.SetEditValue(ValueContainer.EditValue, UpdateEditorSource.ValueChanging);
		}
		protected internal override void OnNullValueChanged(object nullValue) {
			base.OnNullValueChanged(nullValue);
			Editor.DoValidate();
		}
		public virtual void RoundToBoundsChanged(bool value) {
			if (ShouldLockUpdate)
				return;
			T syncValue = CreateValueConverter(ValueContainer.EditValue).Value;
			ValueContainer.SetEditValue(syncValue, UpdateEditorSource.TextInput);
		}
		protected override object GetValueForDisplayText() {
			object value = base.GetValueForDisplayText();
			return Editor.AllowNullInput ? value : CreateValueConverter(value).Value;
		}
	}
}
namespace DevExpress.Xpf.Editors.Native {
	public class MinMaxUpdateHelper {
		BaseEdit Editor { get; set; }
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty MinValueProperty;
		[IgnoreDependencyPropertiesConsistencyChecker]
		readonly DependencyProperty MaxValueProperty;
		public IComparableWrapper MinValue { get; set; }
		public IComparableWrapper MaxValue { get; set; }
		public MinMaxUpdateHelper(BaseEdit editor, DependencyProperty minProperty, DependencyProperty maxProperty) {
			Editor = editor;
			MinValueProperty = minProperty;
			MaxValueProperty = maxProperty;
		}
		public void Update<T>(MinMaxUpdateSource updateSource) where T : struct, IComparable {
			if(updateSource == MinMaxUpdateSource.MinChanged && MaxValue != null && MinValue.CompareTo(MaxValue) > 0)
				MaxValue = MinValue;
			if(updateSource == MinMaxUpdateSource.MaxChanged && MinValue != null && MaxValue.CompareTo(MinValue) < 0)
				MinValue = MaxValue;
			if(updateSource == MinMaxUpdateSource.ISupportInitialize) {
				if(MaxValue != null && MaxValue.CompareTo(default(T)) < 0)
					MaxValue = MinValue != null && MinValue.CompareTo(MaxValue) > 0 ? MinValue : MaxValue;
				else if(MinValue != null && MinValue.CompareTo(default(T)) > 0)
					MinValue = MaxValue != null && MaxValue.CompareTo(MinValue) < 0 ? MaxValue : MinValue;
			}
			if (MinValue != null && !MinValue.IsInfinity && !object.Equals(MinValue.Value, Editor.GetValue(MinValueProperty)))
				Editor.SetCurrentValue(MinValueProperty, MinValue.Value);
			if (MaxValue != null && !MaxValue.IsInfinity && !object.Equals(MaxValue.Value, Editor.GetValue(MaxValueProperty)))
				Editor.SetCurrentValue(MaxValueProperty, MaxValue.Value);
			Editor.EditStrategy.SyncWithValue();
			Editor.DoValidate();
		}
	}
	public enum MinMaxUpdateSource {
		MinChanged,
		MaxChanged,
		ISupportInitialize
	}
	public class IComparableWrapper : IComparable {
		public IComparable Value { get; private set; }
		public bool IsInfinity { get; private set; }
		public IComparableWrapper(IComparable value, bool isInfinity) {
			Value = value;
			IsInfinity = isInfinity;
		}
		#region IComparable Members
		public int CompareTo(object obj) {
			IComparableWrapper wrapper = obj as IComparableWrapper;
			if(wrapper != null)
				return Value.CompareTo(wrapper.Value);
			return Value.CompareTo(obj);
		}
		#endregion
	}
}
