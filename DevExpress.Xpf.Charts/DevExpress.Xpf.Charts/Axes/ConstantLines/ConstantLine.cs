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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Design;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public class ConstantLine : ChartElement,  IConstantLine, ILegendVisible, IWeakEventListener, IInteractiveElement {
		static readonly DependencyPropertyKey ActualLineStylePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualLineStyle", typeof(LineStyle), typeof(ConstantLine), new PropertyMetadata(null));
		public static readonly DependencyProperty ActualLineStyleProperty = ActualLineStylePropertyKey.DependencyProperty;
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(ConstantLine), new PropertyMetadata(true, VisiblePropertyChanged));
		public static readonly DependencyProperty CheckedInLegendProperty = DependencyPropertyManager.Register("CheckedInLegend",
			typeof(bool), typeof(ConstantLine), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty CheckableInLegendProperty = DependencyPropertyManager.Register("CheckableInLegend",
			typeof(bool), typeof(ConstantLine), new PropertyMetadata(true));
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value",
			typeof(object), typeof(ConstantLine), new PropertyMetadata(null, ValuePropertyChanged));
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(Brush), typeof(ConstantLine));
		public static readonly DependencyProperty LineStyleProperty = DependencyPropertyManager.Register("LineStyle",
			typeof(LineStyle), typeof(ConstantLine), new PropertyMetadata(LineStylePropertyChanged));
		public static readonly DependencyProperty LegendTextProperty = DependencyPropertyManager.Register("LegendText",
			typeof(string), typeof(ConstantLine), new PropertyMetadata(String.Empty, ChartElementHelper.Update));
		public static readonly DependencyProperty TitleProperty = DependencyPropertyManager.Register("Title",
			typeof(ConstantLineTitle), typeof(ConstantLine), new PropertyMetadata(ChartElementHelper.ChangeOwnerAndUpdateXYDiagram2DItems));
		public static readonly DependencyProperty LegendMarkerTemplateProperty = DependencyPropertyManager.Register("LegendMarkerTemplate",
			typeof(DataTemplate), typeof(ConstantLine));
		public static readonly DependencyProperty ElementTemplateProperty = DependencyPropertyManager.Register("ElementTemplate",
			typeof(DataTemplate), typeof(ConstantLine));
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ConstantLine constantLine = d as ConstantLine;
			if (constantLine != null) {
				object value = e.NewValue;
				string stringValue = value as string;
				if (stringValue == String.Empty)
					value = null;
				IAxisData axis = constantLine.Axis;
				if (value == null || axis == null)
					constantLine.axisValue = value;
				else {
					AxisScaleTypeMap map = axis.AxisScaleTypeMap;
					constantLine.axisValue = map == null ? value : map.ConvertValue(value, CultureInfo.InvariantCulture);
				}
				if (value == null)
					constantLine.valueInternal = Double.NaN;
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(constantLine, axis));
			}
		}
		static void LineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ConstantLine constantLine = d as ConstantLine;
			if (constantLine != null) {
				LineStyle newLineStyle = e.NewValue as LineStyle;
				constantLine.SetValue(ActualLineStylePropertyKey, newLineStyle == null ? new LineStyle() : newLineStyle);
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LineStyle, newLineStyle, constantLine);
			}
			ChartElementHelper.Update(d, e);
		}
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ConstantLine constantLine = d as ConstantLine;
			if (constantLine != null)
				ChartElementHelper.Update(d, new AxisElementUpdateInfo(constantLine, constantLine.Axis));
		}
		object axisValue;
		double valueInternal = Double.NaN;
		SelectionInfo selectionInfo;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public LineStyle ActualLineStyle { get { return (LineStyle)GetValue(ActualLineStyleProperty); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineCheckedInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckedInLegend {
			get { return (bool)GetValue(CheckedInLegendProperty); }
			set { SetValue(CheckedInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineCheckableInLegend"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckableInLegend {
			get { return (bool)GetValue(CheckableInLegendProperty); }
			set { SetValue(CheckableInLegendProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineValue"),
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
	DevExpressXpfChartsLocalizedDescription("ConstantLineBrush"),
#endif
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineLineStyle"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle LineStyle {
			get { return (LineStyle)GetValue(LineStyleProperty); }
			set { SetValue(LineStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineLegendText"),
#endif
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string LegendText {
			get { return (string)GetValue(LegendTextProperty); }
			set { SetValue(LegendTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineTitle"),
#endif
		Category(Categories.Elements),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ConstantLineTitle Title {
			get { return (ConstantLineTitle)GetValue(TitleProperty); }
			set { SetValue(TitleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineLegendMarkerTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate LegendMarkerTemplate {
			get { return (DataTemplate)GetValue(LegendMarkerTemplateProperty); }
			set { SetValue(LegendMarkerTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("ConstantLineElementTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate ElementTemplate {
			get { return (DataTemplate)GetValue(ElementTemplateProperty); }
			set { SetValue(ElementTemplateProperty, value); }
		}
		internal Axis2D Axis { get { return ((IChartElement)this).Owner as Axis2D; } }
		internal object AxisValue { get { return axisValue; } }
		internal double InternalValue { get { return valueInternal; } }
		#region Hidden properties
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Background { get { return base.Background; } set { base.Background = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush BorderBrush { get { return base.BorderBrush; } set { base.BorderBrush = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness BorderThickness { get { return base.BorderThickness; } set { base.BorderThickness = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush Foreground { get { return base.Foreground; } set { base.Foreground = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Thickness Padding { get { return base.Padding; } set { base.Padding = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double Opacity { get { return base.Opacity; } set { base.Opacity = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Brush OpacityMask { get { return base.OpacityMask; } set { base.OpacityMask = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Effect Effect { get { return base.Effect; } set { base.Effect = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform RenderTransform { get { return base.RenderTransform; } set { base.RenderTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Point RenderTransformOrigin { get { return base.RenderTransformOrigin; } set { base.RenderTransformOrigin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Visibility Visibility { get { return base.Visibility; } set { base.Visibility = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Geometry Clip { get { return base.Clip; } set { base.Clip = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		ConstantLine(object value, string title)
			: this() {
			Value = value;
			if (Title == null)
				Title = new ConstantLineTitle();
			Title.Content = title;
		}
		public ConstantLine(string value, string title)
			: this((object)value, title) {
		}
		public ConstantLine(DateTime value, string title)
			: this((object)value, title) {
		}
		public ConstantLine(double value, string title)
			: this((object)value, title) {
		}
		public ConstantLine() {
			DefaultStyleKey = typeof(ConstantLine);
			this.SetValue(ActualLineStylePropertyKey, new LineStyle());
			selectionInfo = new SelectionInfo();
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
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			if (managerType != typeof(PropertyChangedWeakEventManager))
				return false;
			ChartElementHelper.Update(this);
			return true;
		}
		#endregion
		#region IInteractiveElement implementation
		bool IInteractiveElement.IsHighlighted {
			get { return selectionInfo.IsHighlighted; }
			set { selectionInfo.IsHighlighted = value; }
		}
		bool IInteractiveElement.IsSelected {
			get { return selectionInfo.IsSelected; }
			set { selectionInfo.IsSelected = value; }
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		#region ILegendVisible implementation
		DataTemplate ILegendVisible.LegendMarkerTemplate { get { return LegendMarkerTemplate; } }
		bool ILegendVisible.CheckedInLegend { get { return CheckedInLegend; } }
		bool ILegendVisible.CheckableInLegend { get { return CheckableInLegend; } }
		#endregion
		bool IConstantLine.Visible { get { return GetActualVisible(); } }
		internal bool GetActualVisible() {
			if (Visible) {
				if (Axis != null && Axis.Diagram2D != null && Axis.Diagram2D.ChartControl != null && Axis.Diagram2D.ChartControl.LegendUseCheckBoxes)
					return !CheckableInLegend || CheckedInLegend;
				return true;
			}
			return false;
		}
		internal LegendItem GetLegendItem() {
			string legendText = LegendText;
			return (String.IsNullOrEmpty(legendText) || !Visible) ? null : new LegendItem(this, this, legendText, Brush, ActualLineStyle);
		}
	}
	public class ConstantLineCollection : ChartElementCollection<ConstantLine>, IEnumerable<IConstantLine> {
		protected override ChartElementChange Change { get { return base.Change | ChartElementChange.UpdateXYDiagram2DItems; } }
		IEnumerator<IConstantLine> IEnumerable<IConstantLine>.GetEnumerator() {
			foreach (IConstantLine constantLine in this)
				yield return constantLine;
		}
		internal void FillConstantLinesAndTitles(ObservableCollection<object> items) {
			foreach (ConstantLine constantLine in this) {
				items.Add(constantLine);
				ConstantLineTitle title = constantLine.Title;
				if (title != null)
					items.Add(title);
			}
		}
		internal void FillConstantLineItems(ObservableCollection<ConstantLineItem> constantLineItems, ObservableCollection<ConstantLineTitleItem> constantLineTitleItems) {
			foreach (ConstantLine constantLine in this) {
				ConstantLineItem item = new ConstantLineItem(constantLine);
				constantLineItems.Add(item);
				ConstantLineTitle title = constantLine.Title;
				if (title != null)
					constantLineTitleItems.Add(new ConstantLineTitleItem(title, item));
			}
		}
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
		}
		protected override ChartUpdateInfoBase CreateUpdateInfo(System.Collections.Specialized.NotifyCollectionChangedAction action, System.Collections.IList newItems, System.Collections.IList oldItems, int newStartingIndex, int oldStartingIndex) {
			return new AxisElementUpdateInfo(this, (AxisBase)Owner);
		}
	}
}
