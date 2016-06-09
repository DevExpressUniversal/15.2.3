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
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public abstract class MarkerItem : DependencyObject, IWeakEventListener, INotifyPropertyChanged {
		public static readonly DependencyProperty MarkerBrushProperty = DependencyPropertyManager.Register("MarkerBrush",
			typeof(Brush), typeof(MarkerItem), new PropertyMetadata(MarkerBrushPropertyChanged));
		public static readonly DependencyProperty MarkerLineStyleProperty = DependencyPropertyManager.Register("MarkerLineStyle",
			typeof(LineStyle), typeof(MarkerItem), new PropertyMetadata(null, MarkerLineStylePropertyChanged));
		public static readonly DependencyProperty MarkerLineBrushProperty = DependencyPropertyManager.Register("MarkerLineBrush",
			typeof(Brush), typeof(MarkerItem), new PropertyMetadata(MarkerLineBrushPropertyChanged));
		public static readonly DependencyProperty MarkerTemplateProperty = DependencyPropertyManager.Register("MarkerTemplate", typeof(DataTemplate), typeof(MarkerItem));
		static readonly DependencyPropertyKey RepresentedElementPropertyKey = DependencyPropertyManager.RegisterReadOnly("RepresentedElement",
			typeof(ILegendVisible), typeof(MarkerItem), new PropertyMetadata(null));
		public static readonly DependencyProperty RepresentedElementProperty = RepresentedElementPropertyKey.DependencyProperty;
		public Brush MarkerBrush {
			get { return (Brush)GetValue(MarkerBrushProperty); }
			set { SetValue(MarkerBrushProperty, value); }
		}
		public Brush MarkerLineBrush {
			get { return (Brush)GetValue(MarkerLineBrushProperty); }
			set { SetValue(MarkerLineBrushProperty, value); }
		}
		public DataTemplate MarkerTemplate {
			get { return (DataTemplate)GetValue(MarkerTemplateProperty); }
			set { SetValue(MarkerTemplateProperty, value); }
		}
		public LineStyle MarkerLineStyle {
			get { return (LineStyle)GetValue(MarkerLineStyleProperty); }
			set { SetValue(MarkerLineStyleProperty, value); }
		}
		public ILegendVisible RepresentedElement {
			get { return (ILegendVisible)GetValue(RepresentedElementProperty); }
		}
		static void MarkerLineStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MarkerItem marker = d as MarkerItem;
			if (marker != null) {
				marker.OnLineThicknessChanged();
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as LineStyle, e.NewValue as LineStyle, marker);
			}
		}
		static void MarkerBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MarkerItem marker = d as MarkerItem;
			if (marker != null)
				marker.NotifyPropertyChanged("ActualMarkerBrush");
		}
		static void MarkerLineBrushPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MarkerItem marker = d as MarkerItem;
			if (marker != null)
				marker.NotifyPropertyChanged("ActualMarkerLineBrush");
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double ActualLineThickness {
			get { return GetActualLineThickness(); }
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush OpacityMask {
			get { return GetOpacityMask(); }
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush ActualMarkerBrush {
			get { return GetActualMarkerBrush(); }
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Brush ActualMarkerLineBrush {
			get { return GetActualMarkerLineBrush(); }
		}
		[DXBrowsable(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Thickness ActualMarkerMargin {
			get { return GetActualMarkerMargin(); }
		}
		internal MarkerItem(ILegendVisible browsableObject, Brush markerBrush, Brush markerLineBrush, LineStyle markerLineStyle) {
			this.SetValue(RepresentedElementPropertyKey, browsableObject);
			MarkerBrush = markerBrush;
			MarkerLineBrush = markerLineBrush;
			MarkerLineStyle = markerLineStyle;
			CreateBinding();
		}
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region INotifyPropertyChanged implementation
		public event PropertyChangedEventHandler PropertyChanged;
		protected void NotifyPropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager) && (sender is LineStyle)) {
				OnLineThicknessChanged();
				success = true;
			}
			return success;
		}
		void CreateBinding() {
			Binding template = new Binding("RepresentedElement.LegendMarkerTemplate");
			template.RelativeSource = new RelativeSource(RelativeSourceMode.Self);
			BindingOperations.SetBinding(this, LegendItem.MarkerTemplateProperty, template);
		}
		protected void OnLineThicknessChanged() {
			NotifyPropertyChanged("ActualLineThickness");
		}
		protected virtual double GetActualLineThickness() {
			return MarkerLineStyle.Thickness;
		}
		protected virtual Brush GetOpacityMask() {
			return null;
		}
		protected virtual Brush GetActualMarkerBrush() {
			return MarkerBrush;
		}
		protected virtual Brush GetActualMarkerLineBrush() {
			return MarkerLineBrush;
		}
		protected virtual Thickness GetActualMarkerMargin() {
			return new Thickness();
		}
	}
	[NonCategorized]
	public class CrosshairMarkerItem : MarkerItem {
		internal CrosshairMarkerItem(ILegendVisible browsableObject, Brush markerBrush, Brush markerLineBrush, LineStyle markerLineStyle)
			: base(browsableObject, markerBrush, markerLineBrush, markerLineStyle) {			
		}
	}
}
