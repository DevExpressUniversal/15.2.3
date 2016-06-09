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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum ValueLevel {
		Value = ValueLevelInternal.Value, 
		Value2 = ValueLevelInternal.Value_2, 
		Low = ValueLevelInternal.Low, 
		High = ValueLevelInternal.High, 
		Open = ValueLevelInternal.Open,
		Close = ValueLevelInternal.Close
	}
	public interface ISupportIndicatorLabel {
		IndicatorLabel Label { get; set; }
	}
	[TemplatePart(Name = "PART_IdicatorGeometry", Type = typeof(ChartContentPresenter))]
	public abstract class Indicator : ChartElement, ILegendVisible, IInteractiveElement {
		#region Dependency property
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
		  typeof(bool), typeof(Indicator), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty CheckedInLegendProperty = DependencyPropertyManager.Register("CheckedInLegend",
			typeof(bool), typeof(Indicator), new PropertyMetadata(true, ChartElementHelper.Update));
		public static readonly DependencyProperty CheckableInLegendProperty = DependencyPropertyManager.Register("CheckableInLegend",
			typeof(bool), typeof(Indicator), new PropertyMetadata(true));
		public static readonly DependencyProperty BrushProperty = DependencyPropertyManager.Register("Brush",
			typeof(Brush), typeof(Indicator), new PropertyMetadata(OnBrushPropertyChanged));
		public static readonly DependencyProperty LegendTextProperty = DependencyPropertyManager.Register("LegendText",
			typeof(string), typeof(Indicator), new PropertyMetadata(String.Empty, ChartElementHelper.Update)); 
		public static readonly DependencyProperty LegendMarkerTemplateProperty = DependencyPropertyManager.Register("LegendMarkerTemplate",
			typeof(DataTemplate), typeof(Indicator)); 
		public static readonly DependencyProperty LineStyleProperty = DependencyPropertyManager.Register("LineStyle",
			typeof(LineStyle), typeof(Indicator), new PropertyMetadata(OnLineStylePropertyChanged));
		public static readonly DependencyProperty ShowInLegendProperty = DependencyPropertyManager.Register("ShowInLegend",
			typeof(bool), typeof(Indicator), new PropertyMetadata(false, ChartElementHelper.Update));
		#endregion
		readonly IndicatorItem indicatorItem;
		internal Brush BrushInternal { get; private set; }
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckedInLegend {
			get { return (bool)GetValue(CheckedInLegendProperty); }
			set { SetValue(CheckedInLegendProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool CheckableInLegend {
			get { return (bool)GetValue(CheckableInLegendProperty); }
			set { SetValue(CheckableInLegendProperty, value); }
		}
		[
		Category(Categories.Brushes),
		XtraSerializableProperty
		]
		public Brush Brush {
			get { return (Brush)GetValue(BrushProperty); }
			set { SetValue(BrushProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public LineStyle LineStyle {
			get { return (LineStyle)GetValue(LineStyleProperty); }
			set { SetValue(LineStyleProperty, value); }
		}
		[
		Category(Categories.Presentation),
		XtraSerializableProperty
		]
		public string LegendText {
			get { return (string)GetValue(LegendTextProperty); }
			set { SetValue(LegendTextProperty, value); }
		}
		[Category(Categories.Presentation)]
		public DataTemplate LegendMarkerTemplate {
			get { return (DataTemplate)GetValue(LegendMarkerTemplateProperty); }
			set { SetValue(LegendMarkerTemplateProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowInLegend {
			get { return (bool)GetValue(ShowInLegendProperty); }
			set { SetValue(ShowInLegendProperty, value); }
		}
		[Browsable(false), 
		 EditorBrowsable(EditorBrowsableState.Never)]
		public IndicatorItem Item { get { return indicatorItem; } }
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
		public new Thickness Margin { get { return base.Margin; } set { base.Margin = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontFamily FontFamily { get { return base.FontFamily; } set { base.FontFamily = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new double FontSize { get { return base.FontSize; } set { base.FontSize = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontStyle FontStyle { get { return base.FontStyle; } set { base.FontStyle = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontWeight FontWeight { get { return base.FontWeight; } set { base.FontWeight = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new FontStretch FontStretch { get { return base.FontStretch; } set { base.FontStretch = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new Transform LayoutTransform { get { return base.LayoutTransform; } set { base.LayoutTransform = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		#endregion
		protected internal virtual List<IndicatorLabelItem> LabelItems { get; protected set; }
		protected internal XYSeries2D XYSeries { get { return ((IOwnedElement)this).Owner as XYSeries2D; } }
		protected internal Pane Pane { get { return XYSeries.ActualPane as Pane; } }
		public Indicator() {
			DefaultStyleKey = typeof(Indicator);
			indicatorItem = new IndicatorItem(this);
			BrushInternal = this.Brush;
		}
		protected internal virtual void CalculateLayout(IRefinedSeries refineSeries) {
			Item.IndicatorGeometry = null;
		}
		protected internal virtual void CreateLabelItems() {
			LabelItems = null;
		}
		protected internal virtual IndicatorLabelLayout CalculateLabelLayout(IndicatorLabelItem labelItem, Size labelSize, object dataForLayoutCalculation) {
			return null;
		}
		protected abstract Indicator CreateInstance();
		protected virtual void Assign(Indicator indicator) {
			CopyPropertyValueHelper.CopyPropertyValue(this, indicator, VisibleProperty);
			CopyPropertyValueHelper.CopyPropertyValue(this, indicator, BrushProperty);
			CopyPropertyValueHelper.CopyPropertyValue(this, indicator, LegendTextProperty);
			CopyPropertyValueHelper.CopyPropertyValue(this, indicator, LegendMarkerTemplateProperty);
			CopyPropertyValueHelper.CopyPropertyValue(this, indicator, LineStyleProperty);
			CopyPropertyValueHelper.CopyPropertyValue(this, indicator, ShowInLegendProperty);
		}
		protected internal Indicator CloneForBinding() {
			Indicator indicator = CreateInstance();
			if (indicator != null)
				indicator.Assign(this);
			return indicator;
		}
		protected internal virtual void CompleteDeserializing() {
		}
		internal bool GetActualVisible() {
			if (Visible && XYSeries != null && XYSeries.GetActualVisible()) {
				if (XYSeries.Diagram != null && XYSeries.Diagram.ChartControl != null && XYSeries.Diagram.ChartControl.LegendUseCheckBoxes)
					return !CheckableInLegend || CheckedInLegend;
				else
					return true;
			}
			return false;
		}
		public abstract Geometry CreateGeometry();
		static void OnBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Indicator indicator = d as Indicator;
			if (d != null && e.OldValue != e.NewValue) {
				indicator.Item.Brush = indicator.Brush;
				indicator.BrushInternal = indicator.Item.Brush;
				if (indicator.Item.LegendItem != null)
					indicator.Item.LegendItem.MarkerLineBrush = indicator.Brush;
			}
		}
		static void OnLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Indicator indicator = d as Indicator;
			if (d != null && e.OldValue != e.NewValue) {
				indicator.Item.LineStyle = indicator.LineStyle;
				if (indicator.Item.LegendItem != null)
					indicator.Item.LegendItem.MarkerLineStyle = indicator.LineStyle;
			}
		}
		#region IInteractiveElement implementation
		bool isSelected = false;
		bool isHighlighted = false;
		bool IInteractiveElement.IsSelected {
			get { return isSelected; }
			set {
				if (isSelected != value) {
					isSelected = value;
					Item.UpdateSelection();
				}
			}
		}
		bool IInteractiveElement.IsHighlighted {
			get { return isHighlighted; }
			set {
				if (isHighlighted != value) {
					isHighlighted = value;
					Item.UpdateSelection();
				}
			}
		}
		object IInteractiveElement.Content { get { return this; } }
		#endregion
		#region ILegendVisible implementation
		DataTemplate ILegendVisible.LegendMarkerTemplate { get { return LegendMarkerTemplate; } }
		bool ILegendVisible.CheckedInLegend { get { return CheckedInLegend; } }
		bool ILegendVisible.CheckableInLegend { get { return CheckableInLegend; } }
		#endregion
	}
}
