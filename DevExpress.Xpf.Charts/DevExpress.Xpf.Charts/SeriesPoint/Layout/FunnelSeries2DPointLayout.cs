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

using DevExpress.Charts.Native;
using DevExpress.Xpf.Charts.Native;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
namespace DevExpress.Xpf.Charts {
	public class FunnelSeries2DPointLayout : SeriesPointLayout, INotifyPropertyChanged {
		readonly Rect initialBounds;
		readonly Point initTopLeftPoint;
		readonly Point initBottomLeftPoint;
		readonly Point initTopRightPoint;
		readonly Point initBottomRightPoint;
		readonly FunnelPointInfo pointInfo;
		Point topLeftPoint;
		Point bottomLeftPoint;
		Point topRightPoint;
		Point bottomRightPoint;
		Rect geometryBounds;
		Geometry pointGeometry;
		Geometry borderGeometry;
		internal Point InitTopLeftPoint { get { return initTopLeftPoint; } }
		internal Point InitBottomLeftPoint { get { return initBottomLeftPoint; } }
		internal Point InitTopRightPoint { get { return initTopRightPoint; } }
		internal Point InitBottomRightPoint { get { return initBottomRightPoint; } }
		internal Rect GeometryBounds { get { return geometryBounds; } }
		internal FunnelPointInfo PointInfo { get { return pointInfo; } }
		public override Rect Bounds {
			get { return initialBounds; }
		}
		public override Geometry ClipGeometry {
			get {
				PathFigure figure = new PathFigure();
				figure.StartPoint = topLeftPoint;
				figure.IsClosed = true;
				figure.IsFilled = true;
				PolyLineSegment polyline = new PolyLineSegment();
				polyline.Points.Add(topLeftPoint);
				polyline.Points.Add(topRightPoint);
				polyline.Points.Add(bottomRightPoint);
				polyline.Points.Add(bottomLeftPoint);
				figure.Segments.Add(polyline);
				PathGeometry geomerty = new PathGeometry();
				geomerty.Figures.Add(figure);
				return geomerty;
			}
		}
		public override Transform Transform { get { return null; } }
		[
		Category(Categories.Data)]
		public Geometry PointGeometry {
			get { return pointGeometry; }
			set {
				if (pointGeometry != value) {
					pointGeometry = value;
					if (PropertyChanged != null)
						PropertyChanged(this, new PropertyChangedEventArgs("PointGeometry"));
				}
			}
		}
		[
		Category(Categories.Data)]
		public Geometry BorderGeometry {
			get { return borderGeometry; }
			set {
				if (borderGeometry != value) {
					borderGeometry = value;
					if (PropertyChanged != null)
						PropertyChanged(this, new PropertyChangedEventArgs("BorderGeometry"));
				}
			}
		}
		public FunnelSeries2DPointLayout(Rect viewport, Rect bounds, FunnelPointInfo pointInfo)
			: base(viewport) {
			this.initialBounds = bounds;
			this.pointInfo = pointInfo;
			this.initTopLeftPoint = new Point(MathUtils.StrongRound(pointInfo.TopLeftPoint.X), MathUtils.StrongRound(pointInfo.TopLeftPoint.Y));
			this.initTopRightPoint = new Point(MathUtils.StrongRound(pointInfo.TopRightPoint.X), MathUtils.StrongRound(pointInfo.TopRightPoint.Y));
			this.initBottomRightPoint = new Point(MathUtils.StrongRound(pointInfo.BottomRightPoint.X), MathUtils.StrongRound(pointInfo.BottomRightPoint.Y));
			this.initBottomLeftPoint = new Point(MathUtils.StrongRound(pointInfo.BottomLeftPoint.X), MathUtils.StrongRound(pointInfo.BottomLeftPoint.Y));
			CalculateGeometryBounds();
			PointGeometry = ClipGeometry;
			BorderGeometry = ClipGeometry;
		}
		void CalculateGeometryBounds() {
			if (initTopLeftPoint.X <= initBottomLeftPoint.X)
				geometryBounds = new Rect(
					initTopLeftPoint.X, 
					initTopLeftPoint.Y, 
					initTopRightPoint.X - initTopLeftPoint.X, 
					initBottomLeftPoint.Y - initTopLeftPoint.Y);
			else
				geometryBounds = new Rect(
					initBottomLeftPoint.X,
					initTopLeftPoint.Y,
					initBottomRightPoint.X - initBottomLeftPoint.X,
					initBottomLeftPoint.Y - initTopLeftPoint.Y);
		}
		void CalculatePoints(Rect bounds) { 
			if (initTopLeftPoint.X <= initBottomLeftPoint.X) {
				topLeftPoint = new Point(bounds.X, bounds.Y);
				topRightPoint = new Point(bounds.X + bounds.Width, bounds.Y);
				double bottomTopWidthRatio = (initBottomLeftPoint.X - initBottomRightPoint.X) / (initTopLeftPoint.X - initTopRightPoint.X);
				double bottomWidth = bounds.Width * bottomTopWidthRatio;
				bottomLeftPoint = new Point(bounds.X + (bounds.Width - bottomWidth) / 2, bounds.Y + bounds.Height);
				bottomRightPoint = new Point(bounds.X + bounds.Width - (bounds.Width - bottomWidth) / 2, bounds.Y + bounds.Height);
			}
			else {
				bottomLeftPoint = new Point(bounds.X, bounds.Y + bounds.Height);
				bottomRightPoint = new Point(bounds.X + bounds.Width, bounds.Y + bounds.Height);
				double topBottomWidthRatio = (initTopLeftPoint.X - initTopRightPoint.X) / (initBottomLeftPoint.X - initBottomRightPoint.X);
				double topWidth = bounds.Width * topBottomWidthRatio;
				topLeftPoint = new Point(bounds.X + (bounds.Width - topWidth) / 2, bounds.Y);
				topRightPoint = new Point(bounds.X + bounds.Width - (bounds.Width - topWidth) / 2, bounds.Y);
			}
		}
		internal void Complete(Rect animatedBounds) {
			CalculatePoints(animatedBounds);
			PointGeometry = ClipGeometry;
			BorderGeometry = ClipGeometry;
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
