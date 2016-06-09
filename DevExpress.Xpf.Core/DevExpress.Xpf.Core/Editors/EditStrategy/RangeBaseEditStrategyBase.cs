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
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using System.Windows.Media.Animation;
using System.Windows.Input;
using System.Windows.Controls;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Editors {
	public abstract class RangeBaseEditStrategyBase : EditStrategyBase {
		EndInitPostponedAction MinMaxPropertyChangedAction;
		MinMaxUpdateHelper MinMaxUpdater { get; set; }
		Locker MinMaxChangeLocker { get; set; }
		protected override bool IsNullTextSupported { get { return true; } }
		protected override bool ApplyDisplayTextConversion { get { return true; } }
		protected new RangeBaseEdit Editor { get { return (RangeBaseEdit)base.Editor; } }
		protected RangeEditBasePanel Panel { get { return Editor.Panel; } }
		protected virtual RangeEditBaseInfo Info { get; private set; }
		public RangeBaseEditStrategyBase(BaseEdit editor)
			: base(editor) {
			Info = CreateEditInfo();
			MinMaxPropertyChangedAction = new EndInitPostponedAction(() => IsInSupportInitialize);
			MinMaxUpdater = CreateMinMaxHelper();
			MinMaxChangeLocker = new Locker();
		}
		protected virtual MinMaxUpdateHelper CreateMinMaxHelper() {
			return new MinMaxUpdateHelper(Editor, RangeBaseEdit.MinimumProperty, RangeBaseEdit.MaximumProperty) {
				MinValue = new IComparableWrapper(Editor.Minimum, false),
				MaxValue = new IComparableWrapper(Editor.Maximum, false)
			};
		}
		protected virtual RangeEditBaseInfo CreateEditInfo() {
			return new RangeEditBaseInfo();
		}
		#region property synchronization
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(RangeBaseEdit.ValueProperty, baseValue => baseValue, baseValue => ObjectToDoubleConverter.TryConvert(baseValue));
		}
		#endregion
		public override void OnLoaded() {
			base.OnLoaded();
			UpdatePositions();
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			if(Panel != null)
				UpdatePositions();
		}
		public virtual void MinimumChanged(double value) {
			if(MinMaxChangeLocker)
				return;
			MinMaxUpdater.MinValue = new IComparableWrapper(value, false);
			using(MinMaxChangeLocker.Lock()) {
				MinMaxPropertyChangedAction.PerformIfNotLoading(() => MinMaxUpdater.Update<double>(MinMaxUpdateSource.MinChanged));
			}
			if (ShouldLockUpdate)
				return;
			if (ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue) < value)
				ValueContainer.SetEditValue(value, UpdateEditorSource.ValueChanging);
			UpdatePositions();
		}
		public virtual void MaximumChanged(double value) {
			if(MinMaxChangeLocker)
				return;
			MinMaxUpdater.MaxValue = new IComparableWrapper(value, false);
			using(MinMaxChangeLocker.Lock()) {
				MinMaxPropertyChangedAction.PerformIfNotLoading(() => MinMaxUpdater.Update<double>(MinMaxUpdateSource.MaxChanged));
			}
			if (ShouldLockUpdate)
				return;
			if(ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue) > value)
				ValueContainer.SetEditValue(value, UpdateEditorSource.ValueChanging);
			UpdatePositions();
		}
		public override void OnInitialized() {
			base.OnInitialized();
			using(MinMaxChangeLocker.Lock()) {
				MinMaxPropertyChangedAction.PerformIfNotLoading(() => MinMaxUpdater.Update<double>(MinMaxUpdateSource.ISupportInitialize));
			}
		}
		public virtual void IncrementLarge(object parameter) {
			Increment(Editor.LargeStep);
		}
		public virtual void DecrementLarge(object parameter) {
			Decrement(Editor.LargeStep);
		}
		public virtual void IncrementSmall(object parameter) {
			Increment(Editor.SmallStep);
		}
		public virtual void DecrementSmall(object parameter) {
			Decrement(Editor.SmallStep);
		}
		public virtual void Increment(double value) {
			double result = ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue);
			result += value;
			ValueContainer.SetEditValue(ToRange(result), UpdateEditorSource.TextInput);
		}
		public virtual void Decrement(double value) {
			double result = ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue);
			result -= value;
			ValueContainer.SetEditValue(ToRange(result), UpdateEditorSource.TextInput);
		}
		public virtual void UpdatePositions() {
			if(Panel == null)
				return;
			double value = GetNormalValue();
			Info.LeftSidePosition = value;
			Info.RightSidePosition = 1 - value;
			UpdateDisplayText();
		}
		protected internal double GetNormalValue() {
			double normalValue = GetNormalValue(ToRange(ObjectToDoubleConverter.TryConvert(ValueContainer.EditValue)));
			return ToNormalRange(normalValue);
		}
		double ToNormalRange(double value) {
			return Math.Max(Math.Min(1, value), 0);
		}
		protected double GetNormalValue(double value) {
			double range = Editor.Maximum - Editor.Minimum;
			range = range.CompareTo(0d) == 0 ? 1 : range;
			return ((ObjectToDoubleConverter.TryConvert(value) - Editor.Minimum) / range);
		}
		protected internal double GetRealValue(double normalValue) {
			double range = Editor.Maximum - Editor.Minimum;
			range = range.CompareTo(0d) == 0 ? 1 : range;
			return Editor.Minimum + range * normalValue;
		}
		protected void SetNormalValue(double value) {
			ValueContainer.SetEditValue(GetRealValue(value), UpdateEditorSource.TextInput);
		}
		protected abstract void UpdateDisplayValue();
		protected bool InRange(double value) {
			return value >= Editor.Minimum && value <= Editor.Maximum; 
		}
		protected virtual double ToRange(double value) {
			if(value < Editor.Minimum)
				return Editor.Minimum;
			if(value > Editor.Maximum)
				return Editor.Maximum;
			return value;
		}
		protected double GetRange() {
			double value = Editor.Maximum - Editor.Minimum;
			return value.CompareTo(0d) != 0 ? value : 1;
		}
		public override void UpdateDataContext(DependencyObject target) {
			base.UpdateDataContext(target);
			RangeEditBaseInfo.SetLayoutInfo(target, Info);
		}
		protected internal virtual void OnApplyTemplate() {
		}
		public virtual void OrientationChanged(Orientation orientation) {
			ApplyStyleSettings(StyleSettings);
		}
		public virtual void Minimize(object parameter) {
			ValueContainer.SetEditValue(Editor.Minimum, UpdateEditorSource.TextInput);
		}
		public virtual void Maximize(object parameter) {
			ValueContainer.SetEditValue(Editor.Maximum, UpdateEditorSource.TextInput);
		}
		protected internal virtual void ValuePropertyChanged(double oldValue, double value) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(RangeBaseEdit.ValueProperty, oldValue, value);
		}
	}
	public class GridLengthConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			double dValue = (double)value;
			if(double.IsNaN(dValue))
				return GridLength.Auto;
			return new GridLength(dValue, GridUnitType.Star);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return value;
		}
	}
#if !SL
	public class RectConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if (values[0] == DependencyProperty.UnsetValue || values[1] == DependencyProperty.UnsetValue)
				return DependencyProperty.UnsetValue;
			double width = (double)values[0];
			double height = (double)values[1];
			return new Rect(0, 0, width, height);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#endif
	public static class ObjectToTrackBarRangeConverter {
		public static TrackBarEditRange TryConvert(object value) { 
			if(value is IConvertible) {
				double result = ObjectToDoubleConverter.TryConvert(value);
				return new TrackBarEditRange() { SelectionStart = result, SelectionEnd = result };
			}
			TrackBarEditRange range = value as TrackBarEditRange;
			return range == null ? new TrackBarEditRange()  : new TrackBarEditRange() { SelectionStart = range.SelectionStart, SelectionEnd = range.SelectionEnd };
		}
	}
	public static class ObjectToDoubleConverter {
		public static double TryConvert(object value) {
			double result;
			try {
				result = Convert.ToDouble(value);
			}
			catch {
				result = 0d;
			}
			return result;
		}
	}
}
