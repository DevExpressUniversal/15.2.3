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
using System.Windows;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class AxisLabelsResolveOverlappingHelper {
		public static void Process(List<IAxisLabelLayout> labels, AxisBase axis, AxisAlignment axisAlignment, bool isRadar) {
			AxisLabelOverlappingResolver resolver = CreateOverlappingResolver(labels, axis, axisAlignment, isRadar);
			resolver.Process();
		}
		public static AxisLabelOverlappingResolver CreateOverlappingResolver(List<IAxisLabelLayout> labels, AxisBase axis, AxisAlignment axisAlignment, bool isRadar) {
			return new AxisLabelOverlappingResolver(labels, axis, isRadar, axisAlignment == AxisAlignment.Far, false);
		}
	}
	public abstract class Axis2DLabelItemLayout : ILayout, IAxisLabelLayout {
		readonly Rect initialBounds;
		readonly int gridIndex;
		readonly bool isCustomLabel;
		bool visible = true;
		double angle = 0.0;
		double radianAngle = 0.0;
		double normalizedAngle = 0.0;
		GRealPoint2D offset = new GRealPoint2D();
		GRealPoint2D limitsOffset = new GRealPoint2D();
		Point location;
		Size size;
		public Point BasePoint {
			get { return new Point(initialBounds.Left, initialBounds.Top); }
		}	  
		public string Text { get { return " "; } }
		public GRealPoint2D Pivot { get { return new GRealPoint2D(location.X + size.Width / 2.0, location.Y + size.Height / 2.0); } }
		public bool Visible {
			get { return visible; }
			set { visible = value; }
		}
		public GRealPoint2D Offset {
			get { return offset; }
			set {
				ApplyOffset(offset, value);
				offset = value;
			}
		}
		public Size Size { get { return size; } }
		public Point Location { get { return location; } }
		protected abstract TextRotation CalculateRotation(double degreeAngle);
		protected abstract GRealPoint2D GetLeftTopPoint(GRealRect2D bounds, double degreeAngle, double radianAngle);
		public virtual Rect Bounds { get { return GetBounds(); } }
		public virtual double Angle {
			get { return angle; }
			set {
				angle = value;
				normalizedAngle = MathUtils.CancelAngle(angle);
				radianAngle = MathUtils.Degree2Radian(normalizedAngle);
				Rotate();
			}
		}
		public bool IsOutOfRange { get; set; }
		#region ILayout members
		Rect ILayout.ClipBounds { get { return Rect.Empty; } }
		#endregion
		#region IAxisLabelLayout members
		bool IAxisLabelLayout.IsCustomLabel { get { return isCustomLabel; } }
		GRealSize2D IAxisLabelLayout.Size { get { return new GRealSize2D(size.Width, size.Height); } }
		int IAxisLabelLayout.GridIndex { get { return this.gridIndex; } }
		GRealPoint2D IAxisLabelLayout.LimitsOffset {
			get { return limitsOffset; }
			set {
				ApplyOffset(limitsOffset, value);
				limitsOffset = value;
			}
		}
		GRealRect2D IAxisLabelLayout.Bounds {
			get { return IsOutOfRange ? GRealRect2D.Empty : Bounds.ToGRealRect2D(); }
		}
		#endregion
		public Axis2DLabelItemLayout(Rect bounds, AxisLabelItem labelItem) {
			this.gridIndex = labelItem.GridIndex;
			this.isCustomLabel = labelItem.IsCustomLabel;
			initialBounds = bounds;
			IsOutOfRange = false;
			size = new Size(bounds.Width, bounds.Height);
			location = new Point(bounds.Left, bounds.Top);
		}
		void ApplyOffset(GRealPoint2D oldOffset, GRealPoint2D newOffset) {
			location = new Point(location.X - oldOffset.X, location.Y - oldOffset.Y);
			location = new Point(location.X + newOffset.X, location.Y + newOffset.Y);
		}
		Rect GetBounds() {
			GRealPoint2D[] points = AxisLabelRotationHelper.CalculateRotatedItemPoints(new GRealRect2D(location.X, location.Y, size.Width, size.Height), radianAngle);
			double minX = Double.MaxValue, minY = Double.MaxValue, maxX = Double.MinValue, maxY = Double.MinValue;
			foreach (GRealPoint2D point in points) {
				if (point.X < minX)
					minX = point.X;
				if (point.X > maxX)
					maxX = point.X;
				if (point.Y < minY)
					minY = point.Y;
				if (point.Y > maxY)
					maxY = point.Y;
			}
			return new Rect(minX, minY, maxX - minX, maxY - minY);
		}
		void Rotate() {
			GRealRect2D convertedBounds = new GRealRect2D(initialBounds.Left + Offset.X, initialBounds.Top + Offset.Y, initialBounds.Width, initialBounds.Height);
			GRealPoint2D offset = AxisLabelRotationHelper.CalculateOffset(CalculateRotation(normalizedAngle), convertedBounds, radianAngle);
			GRealPoint2D leftTopPoint = GetLeftTopPoint(convertedBounds, normalizedAngle, radianAngle);
			location = new Point(leftTopPoint.X + offset.X, leftTopPoint.Y + offset.Y);
			size = new Size(initialBounds.Width, initialBounds.Height);
		}
	}
	public class LeftAxis2DLabelItemLayout : Axis2DLabelItemLayout {
		public LeftAxis2DLabelItemLayout(Rect bounds, AxisLabelItem labelItem) : base(bounds, labelItem) {
		}
		protected override TextRotation CalculateRotation(double degreeAngle) {
			return AxisLabelRotationHelper.CalculateRotationForLeftNearPosition(degreeAngle);
		}
		protected override GRealPoint2D GetLeftTopPoint(GRealRect2D bounds, double degreeAngle, double radianAngle) {
			return AxisLabelRotationHelper.CalculateLeftTopPointForLeftPosition(bounds, degreeAngle, radianAngle, false);
		}
	}
	public class RightAxis2DLabelItemLayout : Axis2DLabelItemLayout {
		public RightAxis2DLabelItemLayout(Rect bounds, AxisLabelItem labelItem) : base(bounds, labelItem) {
		}
		protected override TextRotation CalculateRotation(double degreeAngle) {
			return AxisLabelRotationHelper.CalculateRotationForRightNearPosition(degreeAngle);
		}
		protected override GRealPoint2D GetLeftTopPoint(GRealRect2D bounds, double degreeAngle, double radianAngle) {
			return AxisLabelRotationHelper.CalculateLeftTopPointForRightPosition(bounds, degreeAngle, radianAngle, false);
		}
	}
	public class TopAxis2DLabelItemLayout : Axis2DLabelItemLayout {
		public TopAxis2DLabelItemLayout(Rect bounds, AxisLabelItem labelItem) : base(bounds, labelItem) {
		}
		protected override TextRotation CalculateRotation(double degreeAngle) {
			return AxisLabelRotationHelper.CalculateRotationForTopNearPosition(degreeAngle);
		}
		protected override GRealPoint2D GetLeftTopPoint(GRealRect2D bounds, double degreeAngle, double radianAngle) {
			return AxisLabelRotationHelper.CalculateLeftTopPointForTopPosition(bounds, degreeAngle, radianAngle, false);
		}
	}
	public class BottomAxis2DLabelItemLayout : Axis2DLabelItemLayout {
		public BottomAxis2DLabelItemLayout(Rect bounds, AxisLabelItem labelItem) : base(bounds, labelItem) {
		}
		protected override TextRotation CalculateRotation(double degreeAngle) {
			return AxisLabelRotationHelper.CalculateRotationForBottomNearPosition(degreeAngle);
		}
		protected override GRealPoint2D GetLeftTopPoint(GRealRect2D bounds, double degreeAngle, double radianAngle) {
			return AxisLabelRotationHelper.CalculateLeftTopPointForBottomPosition(bounds, degreeAngle, radianAngle, false);
		}
	}
	public class CircularAxis2DLabelItemLayout : Axis2DLabelItemLayout {
		public override double Angle {
			get { return 0; }
			set { }
		}
		public override Rect Bounds {
			get { return new Rect(Location, Size); }
		}
		public CircularAxis2DLabelItemLayout(Rect bounds, AxisLabelItem labelItem) : base(bounds, labelItem) {
		}
		protected override TextRotation CalculateRotation(double degreeAngle) {
			return TextRotation.CenterCenter;
		}
		protected override GRealPoint2D GetLeftTopPoint(GRealRect2D bounds, double degreeAngle, double radianAngle) {
			return new GRealPoint2D();
		}
	}
}
