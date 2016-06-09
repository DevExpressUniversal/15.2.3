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
using System.Windows;
using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class SeriesLabel2DConnectorLayout : NotifyPropertyChangedObject, ILayout {
		readonly Rect bounds;
		Rect clipBounds = Rect.Empty;
		public bool Visible { get { return true; } }
		public Rect Bounds { get { return bounds; } }
		public Rect ClipBounds {
			get { return clipBounds; }
			set { clipBounds = value; }
		}
		#region ILayout implementation
		Point ILayout.Location { get { return new Point(bounds.Left, bounds.Top); } }
		Size ILayout.Size { get { return new Size(bounds.Width, bounds.Height); } }
		double ILayout.Angle { get { return 0.0; } }
		#endregion
		public SeriesLabel2DConnectorLayout(Rect bounds) {
			this.bounds = bounds;
		}
	}
	[NonCategorized]
	public abstract class SeriesLabel2DLayout : ILayout {
		static Point CalculateCenterPoint(Point anchorPoint, Size labelSize, double indent, double angle) {
			double sin = Math.Sin(angle);
			double cos = Math.Cos(angle);
			double halfWidth = labelSize.Width / 2.0;
			double halfHeight = labelSize.Height / 2.0;
			double connectorLength = indent;
			if (halfHeight + sin == halfHeight)
				connectorLength += Math.Abs(halfWidth / cos);
			else if (halfWidth + cos == halfWidth)
				connectorLength += Math.Abs(halfHeight / sin);
			else
				connectorLength += Math.Min(Math.Abs(halfWidth / cos), Math.Abs(halfHeight / sin));
			return new Point(anchorPoint.X + connectorLength * cos, anchorPoint.Y + connectorLength * sin);
		}
		readonly SeriesLabelItem labelItem;
		readonly GRealPoint2D anchorPoint;
		Rect clipBounds = Rect.Empty;
		GRect2D labelBounds;
		protected SeriesLabelItem LabelItem { get { return labelItem; } }
		protected internal abstract bool IsVisibleForResolveOverlapping { get; }
		internal GRealPoint2D AnchorPoint { get { return anchorPoint; } }
		internal GRect2D LabelBounds { get { return labelBounds; } set { labelBounds = value; } }
		public Rect Bounds { get { return new Rect(labelBounds.Left, labelBounds.Top, labelItem.LabelSize.Width, labelItem.LabelSize.Height); } }
		public bool CanClip { get { return true; } }
		public Rect ClipBounds { get { return clipBounds; } set { clipBounds = value; } }
		public abstract bool Visible { get; }
		#region ILayoutImplementation
		Point ILayout.Location { get { return new Point(labelBounds.Left, labelBounds.Top); } }
		Size ILayout.Size { get { return labelItem.LabelSize; } }
		double ILayout.Angle { get { return 0.0; } }
		#endregion
		SeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint) {
			this.labelItem = labelItem;
			this.anchorPoint = new GRealPoint2D(anchorPoint.X, anchorPoint.Y);
		}
		protected SeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint, double indent, double angle)
			: this(labelItem, anchorPoint) {
			CalculateLabelBounds(CalculateCenterPoint(anchorPoint, labelItem.LabelSize, indent, angle));
		}
		protected SeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint, Point centerPoint)
			: this(labelItem, anchorPoint) {
			CalculateLabelBounds(centerPoint);
		}
		void CalculateLabelBounds(Point centerPoint) {
			int labelWidth = MathUtils.StrongRound(labelItem.LabelSize.Width);
			Rect bounds = new Rect(centerPoint.X - labelWidth / 2.0, centerPoint.Y - labelItem.LabelSize.Height / 2.0, labelWidth, labelItem.LabelSize.Height);
			LabelBounds = GraphicsUtils.ConvertRect(bounds);
		}
	}
	public class PieSeriesLabel2DLayout : SeriesLabel2DLayout, IPieLabelLayout {
		bool visible = true;
		readonly double initialAngle;
		PieSeries Series { get { return LabelItem != null && LabelItem.Label != null ? LabelItem.Label.Series as PieSeries : null; } }
		GRect2D ILabelLayout.LabelBounds { get { return LabelBounds; } set { LabelBounds = value; } }
		bool ILabelLayout.Visible { get { return visible; } set { visible = value; } }
		double IPieLabelLayout.Angle { get { return initialAngle; } }
		bool IPieLabelLayout.ResolveOverlapping { get { return Series != null ? Series.LabelsResolveOverlapping : false; } }
		protected internal override bool IsVisibleForResolveOverlapping { get { return visible; } }
		public override bool Visible { get { return visible; } }
		internal PieSeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint, double indent, double angle)
			: base(labelItem, anchorPoint, indent, angle) {
			initialAngle = MathUtils.Degree2Radian(angle);
		}
		internal PieSeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint, Point centerPoint, double angle)
			: base(labelItem, anchorPoint, centerPoint) {
			initialAngle = MathUtils.Degree2Radian(angle);
		}
	}
	public class FunnelSeriesLabel2DLayout : SeriesLabel2DLayout, IFunnelLabelLayout {
		bool visible = true;
		FunnelSeries2D Series { get { return LabelItem != null && LabelItem.Label != null ? LabelItem.Label.Series as FunnelSeries2D : null; } }
		GRect2D ILabelLayout.LabelBounds { get { return LabelBounds; } set { LabelBounds = value; } }
		bool ILabelLayout.Visible { get { return visible; } set { visible = value; } }
		protected internal override bool IsVisibleForResolveOverlapping { get { return visible; } }
		public override bool Visible { get { return visible; } }
		internal FunnelSeriesLabel2DLayout(SeriesLabelItem labelItem, Point anchorPoint, Point centerPoint)
			: base(labelItem, anchorPoint, centerPoint) {
		}
		bool IFunnelLabelLayout.IsLeftColumn {
			get { return FunnelSeries2D.GetLabelPosition(LabelItem.Label) == Funnel2DLabelPosition.Left || FunnelSeries2D.GetLabelPosition(LabelItem.Label) == Funnel2DLabelPosition.LeftColumn; }
		}
		bool IFunnelLabelLayout.ResolveOverlapping {
			get { return false; }
		}
	}
}
