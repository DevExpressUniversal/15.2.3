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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	public enum HorizontalPosition {
		LeftOutside,
		Left,
		Center,
		Right,
		RightOutside
	}
	public enum VerticalPosition {
		TopOutside,
		Top,
		Center,
		Bottom,
		BottomOutside
	}
	public class Legend : ChartElement, IHitTestableElement, IInteractiveElement, ISupportVisibilityControlElement {
		[IgnoreDependencyPropertiesConsistencyCheckerAttribute]
		public static readonly DependencyProperty ItemsProperty = DependencyPropertyManager.Register("Items", typeof(List<LegendItem>), typeof(Legend));
		public static readonly DependencyProperty ReverseItemsProperty = DependencyPropertyManager.Register("ReverseItems", typeof(bool), typeof(Legend), new PropertyMetadata(false));
		public static readonly DependencyProperty HorizontalPositionProperty = DependencyPropertyManager.Register("HorizontalPosition",
			typeof(HorizontalPosition), typeof(Legend), new PropertyMetadata(HorizontalPosition.RightOutside, PropertyChanged));
		public static readonly DependencyProperty VerticalPositionProperty = DependencyPropertyManager.Register("VerticalPosition",
			typeof(VerticalPosition), typeof(Legend), new PropertyMetadata(VerticalPosition.Top, PropertyChanged));
		public static readonly DependencyProperty IndentFromDiagramProperty = DependencyPropertyManager.Register("IndentFromDiagram", typeof(Thickness), typeof(Legend), new PropertyMetadata(PropertyChanged));
		public static readonly DependencyProperty ItemTemplateProperty = DependencyPropertyManager.Register("ItemTemplate", typeof(DataTemplate), typeof(Legend));
		public static readonly DependencyProperty ItemsPanelProperty = DependencyPropertyManager.Register("ItemsPanel", typeof(ItemsPanelTemplate), typeof(Legend));
		public static readonly DependencyProperty OrientationProperty = DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(Legend), new PropertyMetadata(Orientation.Vertical));
		public static readonly DependencyProperty UseCheckBoxesProperty = DependencyPropertyManager.Register("UseCheckBoxes", typeof(bool), typeof(Legend), new PropertyMetadata(false, ChartElementHelper.Update));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool?), typeof(Legend), new PropertyMetadata(null, VisiblePropertyChanged));
		static void PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Legend legend = d as Legend;
			if (legend != null && legend.ChartControl != null)
				legend.ChartControl.InvalidateChartElementPanel();
		}
		static void VisiblePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			Legend legend = d as Legend;
			if (legend != null && legend.ChartControl != null) {
				legend.InvalidateMeasure();
				ChartElementHelper.Update(d);
			}
		}
		SelectionInfo selectionInfo;
		Rect bounds = Rect.Empty;
		bool autoLayoutVisible = true;
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendItems"),
#endif
		Browsable(false)
		]
		public List<LegendItem> Items {
			get { return (List<LegendItem>)GetValue(ItemsProperty); }
			set { SetValue(ItemsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendReverseItems"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ReverseItems {
			get { return (bool)GetValue(ReverseItemsProperty); }
			set { SetValue(ReverseItemsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendHorizontalPosition"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public HorizontalPosition HorizontalPosition {
			get { return (HorizontalPosition)GetValue(HorizontalPositionProperty); }
			set { SetValue(HorizontalPositionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendVerticalPosition"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public VerticalPosition VerticalPosition {
			get { return (VerticalPosition)GetValue(VerticalPositionProperty); }
			set { SetValue(VerticalPositionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendIndentFromDiagram"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public Thickness IndentFromDiagram {
			get { return (Thickness)GetValue(IndentFromDiagramProperty); }
			set { SetValue(IndentFromDiagramProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendItemTemplate"),
#endif
		Category(Categories.Presentation)
		]
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendItemsPanel"),
#endif
		Category(Categories.Presentation)
		]
		public ItemsPanelTemplate ItemsPanel {
			get { return (ItemsPanelTemplate)GetValue(ItemsPanelProperty); }
			set { SetValue(ItemsPanelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendOrientation"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendUseCheckBoxes"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool UseCheckBoxes {
			get { return (bool)GetValue(UseCheckBoxesProperty); }
			set { SetValue(UseCheckBoxesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfChartsLocalizedDescription("LegendVisible"),
#endif
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool? Visible {
			get { return (bool?)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXBrowsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public SelectionInfo SelectionInfo { get { return selectionInfo; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool Focusable { get { return base.Focusable; } set { base.Focusable = value; } }
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new object ToolTip { get { return base.ToolTip; } set { base.ToolTip = value; } }
		internal ChartControl ChartControl { get { return ((IChartElement)this).Owner as ChartControl; } }
		internal bool ActualVisible {
			get {
				if (Visible.HasValue)
					return Visible.Value;
				return autoLayoutVisible;
			}
		}
		#region IHitTestableElement implementation
		object IHitTestableElement.Element { get { return this; } }
		object IHitTestableElement.AdditionalElement { get { return null; } }
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
		#region ISupportVisibilityControlElement implementation
		int ISupportVisibilityControlElement.Priority { get { return (int)ChartElementVisibilityPriority.Legend; } }
		bool ISupportVisibilityControlElement.Visible {
			get {
				if (!this.Visible.HasValue)
					return autoLayoutVisible;
				return this.Visible.Value;
			}
			set {
				if (!this.Visible.HasValue)
					autoLayoutVisible = value;
			}
		}
		GRealRect2D ISupportVisibilityControlElement.Bounds {
			get {
				if (double.IsInfinity(bounds.Left) || double.IsInfinity(bounds.Top) || double.IsInfinity(bounds.Width) || double.IsInfinity(bounds.Height))
					return new GRealRect2D();
				return new GRealRect2D(bounds.Left, bounds.Top, bounds.Width, bounds.Height);
			}
		}
		VisibilityElementOrientation ISupportVisibilityControlElement.Orientation {
			get {
				if ((this.HorizontalPosition == Charts.HorizontalPosition.LeftOutside && this.VerticalPosition == Charts.VerticalPosition.BottomOutside) ||
					(this.HorizontalPosition == Charts.HorizontalPosition.LeftOutside && this.VerticalPosition == Charts.VerticalPosition.TopOutside) ||
					(this.HorizontalPosition == Charts.HorizontalPosition.RightOutside && this.VerticalPosition == Charts.VerticalPosition.BottomOutside) ||
					(this.HorizontalPosition == Charts.HorizontalPosition.RightOutside && this.VerticalPosition == Charts.VerticalPosition.TopOutside))
					return VisibilityElementOrientation.Corner;
				if (this.HorizontalPosition == Charts.HorizontalPosition.LeftOutside || this.HorizontalPosition == Charts.HorizontalPosition.RightOutside)
					return VisibilityElementOrientation.Vertical;
				if (this.VerticalPosition == Charts.VerticalPosition.BottomOutside || this.VerticalPosition == Charts.VerticalPosition.TopOutside)
					return VisibilityElementOrientation.Horizontal;
				return VisibilityElementOrientation.Inside;
			}
		}
		#endregion
		public Legend() {
			DefaultStyleKey = typeof(Legend);
			DXSerializer.AddCustomGetSerializablePropertiesHandler(this, CustomPropertiesSerializationUtils.SerializableControlProperties);
			selectionInfo = new SelectionInfo();
		}
		protected override Size MeasureOverride(Size constraint) {
			Size size = base.MeasureOverride(constraint);
			if (!size.IsEmpty && size.Height > 0 && size.Width > 0)
				bounds = new Rect(0, 0, size.Width, size.Height);
			if (!ActualVisible)
				return base.MeasureOverride(new Size(0, 0));
			return size;
		}
		public bool ShouldSerializeVisible(XamlDesignerSerializationManager manager) {
			return this.Visible.HasValue;
		}
		public bool ShouldSerializeItems(XamlDesignerSerializationManager manager) {
			return false;
		}
	}
}
