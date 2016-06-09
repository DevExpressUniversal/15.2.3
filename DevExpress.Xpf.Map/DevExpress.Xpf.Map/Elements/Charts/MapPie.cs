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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class MapPieControl : Control {
		public MapPieControl() {
			DefaultStyleKey = typeof(MapPieControl);
		}
	}
	public enum RotationDirection {
		Clockwise,
		CounterClockwise
	}
	public class MapPie : MapChartItemBase, IMapChartItem, IMapContainerDataItem, IPointCore {
		internal const int DefaultGroupIndex = -1;
		const string templateSource = "<local:MapPieControl/>";
		internal static readonly DependencyPropertyKey TotalValuePropertyKey = DependencyPropertyManager.RegisterReadOnly("TotalValue",
			typeof(double), typeof(MapPie), new PropertyMetadata(0.0));
		public static readonly DependencyProperty TotalValueProperty = TotalValuePropertyKey.DependencyProperty;
		public static readonly DependencyProperty RotationDirectionProperty = DependencyPropertyManager.Register("RotationDirection",
			typeof(RotationDirection), typeof(MapPie), new PropertyMetadata(RotationDirection.Clockwise, SegmentsGeometryChanged));
		public static readonly DependencyProperty RotationAngleProperty = DependencyPropertyManager.Register("RotationAngle",
			typeof(double), typeof(MapPie), new PropertyMetadata(0.0, SegmentsGeometryChanged));
		public static readonly DependencyProperty SegmentsProperty = DependencyPropertyManager.Register("Segments",
			typeof(PieSegmentCollection), typeof(MapPie), new PropertyMetadata(null, SegmentsPropertyChanged));
		public static readonly DependencyProperty HoleRadiusPercentProperty = DependencyPropertyManager.Register("HoleRadiusPercent",
			typeof(double), typeof(MapPie), new PropertyMetadata(0.0, SegmentsGeometryChanged), new ValidateValueCallback(HoleRadiusPercentValidation));
		static void SegmentsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPie pie = d as MapPie;
			if (pie != null && e.NewValue != e.OldValue) {
				CommonUtils.SetOwnerForValues(e.OldValue, e.NewValue, pie);
				pie.OnSegmentsChanged();
			}
		}
		static void SegmentsGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapPie pie = d as MapPie;
			if (pie != null)
				pie.UpdateSegmentsGeometry();
		}
		internal static bool HoleRadiusPercentValidation(object holeRadiusPercent) {
			return (double)holeRadiusPercent >= 0 && (double)holeRadiusPercent <= 100;
		}
		[Category(Categories.Layout)]
		public double RotationAngle {
			get { return (double)GetValue(RotationAngleProperty); }
			set { SetValue(RotationAngleProperty, value); }
		}
		[Category(Categories.Layout)]
		public RotationDirection RotationDirection {
			get { return (RotationDirection)GetValue(RotationDirectionProperty); }
			set { SetValue(RotationDirectionProperty, value); }
		}
		[Category(Categories.Layout)]
		public double HoleRadiusPercent {
			get { return (double)GetValue(HoleRadiusPercentProperty); }
			set { SetValue(HoleRadiusPercentProperty, value); }
		}
		[Category(Categories.Data)]
		public PieSegmentCollection Segments {
			get { return (PieSegmentCollection)GetValue(SegmentsProperty); }
			set { SetValue(SegmentsProperty, value); }
		}
		[Category(Categories.Data)]
		public double TotalValue {
			get { return (double)GetValue(TotalValueProperty); }
		}
		static ControlTemplate template = GetDefaultTemplate();
		static ControlTemplate GetDefaultTemplate() {
			XamlHelper.SetLocalNamespace(CommonUtils.localNamespace);
			return XamlHelper.GetControlTemplate(templateSource);
		}
		List<double> segmentsValue = new List<double>();
		internal List<double> SegmentsValue { get { return segmentsValue; } }
		protected override double ItemValue {
			get { return TotalValue; }
			set { }
		}
		protected internal override ControlTemplate ItemTemplate { get { return template; } }
		protected internal override Brush ActualFill { get { return VectorLayer != null && Fill == null ? VectorLayer.ShapeFill : Fill; } }
		public MapPie() {
			SetValue(SegmentsProperty, new PieSegmentCollection());
		}
		#region IMapContatinerDataItem implementation
		IMapChartDataItem IMapContainerDataItem.CreateSegment() {
			return new PieSegment();
		}
		void IMapContainerDataItem.AddSegment(IMapChartDataItem child) {
			Segments.Add((PieSegment)child);
		}
		#endregion
		void ForEachSegment(Action<PieSegment> segmentAction) {
			if (Segments != null)
				foreach (PieSegment segment in Segments) {
					segmentAction(segment);
				}
		}
		void ApplySegmentAppearance(PieSegment segment) {
			segment.ApplyAppearance();
		}
		void UpdateSegmentsGeometry() {
			if (Segments != null) {
				UpdateTotalValue();
				UpdatePercentage();
				UpdateSegmentsAngles();
			}
		}
		double CalculateFinishAngle(double startAngle, double percent) {
			double angleIncrement = percent * 360.0;
			startAngle += RotationDirection == RotationDirection.CounterClockwise ? -angleIncrement : angleIncrement;
			return startAngle;
		}
		void UpdateSegmentsAngles() {
			double currentAngle = 0.0;
			for (int i = 0; i < Segments.Count; i++) {
				double segmentStartAngle = currentAngle;
				currentAngle = CalculateFinishAngle(currentAngle, SegmentsValue[i]);
				if (RotationDirection == RotationDirection.Clockwise)
					Segments[i].UpdateAngles(segmentStartAngle, currentAngle);
				else
					Segments[i].UpdateAngles(currentAngle, segmentStartAngle);
			}
		}
		protected override void OnSizeChanged() {
			OnSegmentsChanged();
			base.OnSizeChanged();
		}
		protected override MapDependencyObject CreateObject() {
			return new MapPie();
		}
		protected internal override void ApplyAppearance() {
			base.ApplyAppearance();
			ForEachSegment(ApplySegmentAppearance);
		}
		protected override void ColorizeCore(MapColorizer colorizer) {
			ForEachSegment((s) => { s.Colorize(colorizer); });
		}
		protected internal override void ResetColor() {
			ForEachSegment((s) => { s.ResetColor(); });
		}
		protected internal override ToolTipPatternParser CreateToolTipPatternParser() {
			IList<object> segmentArguments = new List<object>();
			IList<double> segmentValues = new List<double>();
			foreach(PieSegment segment in Segments) {
				string id = segment.SegmentId != null ? segment.SegmentId.ToString() : string.Empty;
				segmentArguments.Add(id);
				segmentValues.Add(segment.Value);
			}
			return new PieToolTipPatternParser(ActualToolTipPattern, ItemId, TotalValue, segmentArguments, segmentValues);
		}
		protected internal override IList<DevExpress.Map.CoordPoint> GetItemPoints() {
			return new DevExpress.Map.CoordPoint[] { Location };
		}
		internal void OnSegmentsChanged() {
			UpdateSegmentsGeometry();
		}
		internal void UpdatePercentage() {
			SegmentsValue.Clear();
			for (int i = 0; i < Segments.Count; i++)
				SegmentsValue.Add(TotalValue == 0 ? 0 : Segments[i].Value / TotalValue);
		}
		internal void UpdateTotalValue() {
			double total = 0;
			for (int i = 0; i < Segments.Count; i++)
				total += Segments[i].Value;
			this.SetValue(TotalValuePropertyKey, total);
		}
	}
	public class MapPieItemsControl : ItemsControl {
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			PieSegmentPresentationControl pieSegmentControl = (PieSegmentPresentationControl)element;
			PieSegment segment = (PieSegment)item;
			pieSegmentControl.DataContext = segment.Info;
			Binding binding = new Binding("Clip");
			BindingOperations.SetBinding(pieSegmentControl, UIElement.ClipProperty, binding);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new PieSegmentPresentationControl();
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return false;
		}
	}
}
