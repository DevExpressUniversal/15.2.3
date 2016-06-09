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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Map.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	public class PieSegment : MapDependencyObject, IOwnedElement, IMapChartDataItem, IColorizerElement, IKeyColorizerElement, IMapItemAttributeOwner {
		internal static readonly DependencyPropertyKey AttributesPropertyKey = DependencyPropertyManager.RegisterReadOnly("Attributes",
			typeof(MapItemAttributeCollection), typeof(PieSegment), new PropertyMetadata());
		public static readonly DependencyProperty AttributesProperty = AttributesPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value",
			typeof(double), typeof(PieSegment), new PropertyMetadata(0.0, ValuePropertyChanged));
		public static readonly DependencyProperty SegmentIdProperty = DependencyPropertyManager.Register("SegmentId",
			typeof(object), typeof(PieSegment), new PropertyMetadata(null));
		public static readonly DependencyProperty FillProperty = DependencyPropertyManager.Register("Fill",
			typeof(Brush), typeof(PieSegment), new PropertyMetadata(null, FillPropertyChanged));
		static void FillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PieSegment segment = d as PieSegment;
			if(segment != null && e.NewValue != e.OldValue)
				segment.ApplyAppearance();
		}
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PieSegment segment = d as PieSegment;
			if(segment != null && e.NewValue != e.OldValue)
				segment.OnValuePropertyChanged();
		}
		[Category(Categories.Data)]
		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		[Category(Categories.Data)]
		public object SegmentId {
			get { return (object)GetValue(SegmentIdProperty); }
			set { SetValue(SegmentIdProperty, value); }
		}
		[Category(Categories.Data)]
		public MapItemAttributeCollection Attributes {
			get { return (MapItemAttributeCollection)GetValue(AttributesProperty); }
		}
		[Category(Categories.Appearance)]
		public Brush Fill {
			get { return (Brush)GetValue(FillProperty); }
			set { SetValue(FillProperty, value); }
		}
		object owner;
		int rowIndex = MapItem.UndefinedRowIndex;
		int[] listSourceRowIndices = new int[0];
		Color colorizerColor;
		readonly PieSegmentInfo info;
		Color ColorizerColor { get { return colorizerColor; } set { colorizerColor = value; ApplyAppearance(); } }
		Brush ActualFill 
		{
			get {
				if(ColorizerColor != MapColorizer.DefaultColor)
					return new SolidColorBrush(ColorizerColor);
				return Pie != null && Fill == null ? Pie.ActualFill : Fill;
			}
		}
		int[] ListSourceRowIndices {
			get { return listSourceRowIndices; }
			set {
				int[] indices = value != null ? value : new int[0];
				listSourceRowIndices = indices;
			}
		}
		internal MapPie Pie { get { return owner as MapPie; } }
		internal PieSegmentInfo Info { get { return info; } }
		public PieSegment() {
			this.SetValue(AttributesPropertyKey, new MapItemAttributeCollection(this));
			info = new PieSegmentInfo(this);
		}
		void OnValuePropertyChanged() {
			if(Pie != null)
				Pie.OnSegmentsChanged();
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if (owner != value) {
					owner = value;
					ApplyAppearance();
				}
			}
		}
		#endregion
		#region IMapDataItem implementation
		int IMapDataItem.RowIndex { get { return this.rowIndex; } set { this.rowIndex = value; } }
		int[] IMapDataItem.ListSourceRowIndices { get { return ListSourceRowIndices; } set { ListSourceRowIndices = value; } }
		void IMapDataItem.AddAttribute(IMapItemAttribute attribute) {
			Attributes.Add(new MapItemAttribute(attribute));
		}
		object IMapChartDataItem.Argument { get { return SegmentId; } set { this.SegmentId = value; } }
		#endregion
		#region IColorizerElement Members
		Color IColorizerElement.ColorizerColor {
			get {
				return ColorizerColor;
			}
			set {
				ColorizerColor = value;
			}
		}
		#endregion
		#region IKeyColorizerElement Members
		object IKeyColorizerElement.ColorItemKey { get { return SegmentId; } }
		#endregion
		#region IMapItemAttributeOwner Members
		IMapItemAttribute IMapItemAttributeOwner.GetAttribute(string name) {
			return Attributes[name];
		}
		#endregion
		protected override MapDependencyObject CreateObject() {
			return new PieSegment();
		}
		internal void ApplyAppearance() {
			info.Fill = ActualFill;
		}
		internal void UpdateAngles(double startAngle, double endAngle) {
			double radius = Pie.Size / 2.0;
			info.Clip = PieSegmentCalulator.CalculateClipBounds(new Point(radius, radius), radius, Pie.HoleRadiusPercent, endAngle, startAngle, Pie.RotationAngle);
		}
		internal void Colorize(MapColorizer colorizer) {
			ColorizerColor = colorizer.GetItemColor((IColorizerElement)this);
		}
		internal void ResetColor() {
			ColorizerColor = MapColorizer.DefaultColor;
		}
	}
	public class PieSegmentCollection : MapDependencyObjectCollection<PieSegment> {
		MapPie Pie { get { return Owner as MapPie; } }
		protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (Pie != null)
				Pie.OnSegmentsChanged();
		}
	}
	[NonCategorized]
	public class PieSegmentInfo : INotifyPropertyChanged {
		readonly PieSegment segment;
		Brush fill;
		Geometry clip;
		public event PropertyChangedEventHandler PropertyChanged;
		public MapChartItemInfo PieInfo { get { return segment.Pie != null ? (MapChartItemInfo)segment.Pie.Info : null; } }
		public Brush Fill {
			get { return fill; }
			set {
				if (fill != value) {
					fill = value;
					NotifyPropertyChanged("Fill");
				}
			}
		}
		public Geometry Clip {
			get { return clip; }
			set {
				if (clip != value) {
					clip = value;
					NotifyPropertyChanged("Clip");
				}
			}
		}
		public PieSegmentInfo(PieSegment segment) {
			this.segment = segment;
		}
		void NotifyPropertyChanged(string propertyName) {
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
	public class PieSegmentPresentationControl : Control {
		public PieSegmentPresentationControl() {
			DefaultStyleKey = typeof(PieSegmentPresentationControl);
		} 
	}
	class PieSegmentCalulator {
		static Point CalculatePoint(Point startPoint, double length, double angle) {
			double angleRadian = MathUtils.Degree2Radian(angle);
			return new Point(startPoint.X + Math.Cos(angleRadian) * length, startPoint.Y + Math.Sin(angleRadian) * length);
		}
		public static Geometry CalculateClipBounds(Point center, double radius, double holeRadiusPercent, double startAngle, double finishAngle, double rotation) {
			double deltaAngle = startAngle - finishAngle;
			double holeRadius = radius * holeRadiusPercent / 100.0;
			if (Math.Abs((float)deltaAngle) < 360) {
				bool isLargeArc = deltaAngle > 180;
				double start = MathUtils.NormalizeDegree(startAngle - rotation);
				double finish = MathUtils.NormalizeDegree(finishAngle - rotation);
				double rotationAngle = MathUtils.Degree2Radian(startAngle);
				Point startPoint = PieSegmentCalulator.CalculatePoint(center, 0, MathUtils.NormalizeDegree(0.5 * (finishAngle + startAngle) - rotation));
				PathFigure figure = new PathFigure();
				figure.StartPoint = PieSegmentCalulator.CalculatePoint(startPoint, holeRadius, start);
				ArcSegment outerArc = new ArcSegment();
				outerArc.Point = PieSegmentCalulator.CalculatePoint(startPoint, radius, finish);
				outerArc.RotationAngle = rotationAngle;
				outerArc.Size = new Size(radius, radius);
				outerArc.IsLargeArc = isLargeArc;
				outerArc.SweepDirection = SweepDirection.Counterclockwise;
				ArcSegment innerArc = new ArcSegment();
				innerArc.Point = figure.StartPoint;
				innerArc.Size = new Size(holeRadius, holeRadius);
				innerArc.RotationAngle = rotationAngle;
				innerArc.IsLargeArc = isLargeArc;
				innerArc.SweepDirection = SweepDirection.Clockwise;
				figure.Segments.Add(new LineSegment() { Point = PieSegmentCalulator.CalculatePoint(startPoint, radius, start) });
				figure.Segments.Add(outerArc);
				figure.Segments.Add(new LineSegment() { Point = PieSegmentCalulator.CalculatePoint(startPoint, holeRadius, finish) });
				figure.Segments.Add(innerArc);
				PathGeometry geometry = new PathGeometry();
				geometry.Figures.Add(figure);
				return geometry;
			}
			else {
				GeometryGroup geometry = new GeometryGroup() { FillRule = FillRule.EvenOdd };
				geometry.Children.Add(new EllipseGeometry() { Center = center, RadiusX = radius, RadiusY = radius });
				geometry.Children.Add(new EllipseGeometry() { Center = center, RadiusX = holeRadius, RadiusY = holeRadius });
				return geometry;
			}
		}
	}
}
