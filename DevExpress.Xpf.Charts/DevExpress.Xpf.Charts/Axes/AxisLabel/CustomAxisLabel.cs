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
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[ContentProperty("Content")]
	public class CustomAxisLabel : ChartNonVisualElement, ICustomAxisLabel {
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value", typeof(object), typeof(CustomAxisLabel), new PropertyMetadata(null, ValuePropertyChanged));
		public static readonly DependencyProperty ContentProperty = DependencyPropertyManager.Register("Content", typeof(object), typeof(CustomAxisLabel), new PropertyMetadata(string.Empty, ChartElementHelper.Update));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(CustomAxisLabel), new PropertyMetadata(true, VisiblePropertyChanged));
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CustomAxisLabel label = d as CustomAxisLabel;
			if (label != null) {
				object value = e.NewValue;
				if ((value is string) && (string)value == String.Empty)
					value = null;
				IAxisData axis = label.Axis;
				label.axisValue = (value == null || axis == null || axis.AxisScaleTypeMap == null) ? value :
					axis.AxisScaleTypeMap.ConvertValue(value, CultureInfo.InvariantCulture);
				if (value == null)
					label.valueInternal = Double.NaN;
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(label, axis));
			}
		}
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CustomAxisLabel label = d as CustomAxisLabel;
			if (label != null)
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(label, label.Axis));
		}
		object axisValue;
		double valueInternal = Double.NaN;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomAxisLabelValue"),
#endif
		Category(Categories.Behavior),
		TypeConverter(typeof(AxisValueTypeConverter)),
		XtraSerializableProperty
		]
		public object Value {
			get { return GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomAxisLabelContent"),
#endif
		Category(Categories.Common),
		XtraSerializableProperty
		]
		public object Content {
			get { return (object)GetValue(ContentProperty); }
			set { SetValue(ContentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("CustomAxisLabelVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
#if !SL
	[DevExpressXpfChartsLocalizedDescription("CustomAxisLabelLabel")]
#endif
		public AxisLabel Label { get { return ((IOwnedElement)this).Owner as AxisLabel; } }
		internal object AxisValue { get { return axisValue; } }
		AxisBase Axis { 
			get { 
				AxisLabel label = Label;
				return label == null ? null : label.Axis; 
			} 
		}
		#region IAxisValueContainer implementation
		IAxisData IAxisValueContainer.Axis { get { return Axis; } }
		bool IAxisValueContainer.IsEnabled { get { return Visible; } }
		CultureInfo IAxisValueContainer.Culture { get { return CultureInfo.InvariantCulture; } }
		object IAxisValueContainer.GetAxisValue() { return axisValue; }
		void IAxisValueContainer.SetAxisValue(object axisValue) { this.axisValue = axisValue; }
		double IAxisValueContainer.GetValue() { return valueInternal; }
		void IAxisValueContainer.SetValue(double value) { valueInternal = value; }
		#endregion
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool IsEnabled { get { return base.IsEnabled; } set { base.IsEnabled = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new BindingGroup BindingGroup { get { return base.BindingGroup; } set { base.BindingGroup = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new ContextMenu ContextMenu { get { return base.ContextMenu; } set { base.ContextMenu = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Cursor Cursor { get { return base.Cursor; } set { base.Cursor = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool ForceCursor { get { return base.ForceCursor; } set { base.ForceCursor = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new InputScope InputScope { get { return base.InputScope; } set { base.InputScope = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool OverridesDefaultStyle { get { return base.OverridesDefaultStyle; } set { base.OverridesDefaultStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style Style { get { return base.Style; } set { base.Style = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Style FocusVisualStyle { get { return base.FocusVisualStyle; } set { base.FocusVisualStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		CustomAxisLabel(object value, string text) {
			Value = value;
			Content = text;
		}
		public CustomAxisLabel() {
		}
		public CustomAxisLabel(string value, string text) : this((object)value, text) {
		}
		public CustomAxisLabel(DateTime value, string text) : this((object)value, text) {
		}
		public CustomAxisLabel(double value, string text) : this((object)value, text) {
		}
	}
	public class CustomAxisLabelCollection : ChartElementCollection<CustomAxisLabel>, IEnumerable<ICustomAxisLabel> {
		protected override ChartElementChange Change { get { return base.Change; } }
		IEnumerator<ICustomAxisLabel> IEnumerable<ICustomAxisLabel>.GetEnumerator() {
			foreach (ICustomAxisLabel customLabel in this)
				yield return customLabel;
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(System.Collections.Specialized.NotifyCollectionChangedAction action, System.Collections.IList newItems, System.Collections.IList oldItems, int newStartingIndex, int oldStartingIndex) {
			AxisLabel axisLabel = Owner as AxisLabel;
			if (axisLabel != null && axisLabel.Axis != null)
				return new AxisElementUpdateInfo(this, axisLabel.Axis);
			else
				return base.CreateUpdateInfo(action, newItems, oldItems, newStartingIndex, oldStartingIndex);
		}
	}
}
