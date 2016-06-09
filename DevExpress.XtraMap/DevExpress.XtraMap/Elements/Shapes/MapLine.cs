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

using DevExpress.Map;
using DevExpress.Map.Native;
using DevExpress.XtraMap.Drawing;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Diagnostics.CodeAnalysis;
using System.Collections.Generic;
using System.Drawing.Design;
using System.Linq;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap {
	public class MapLine : MapShape, ISupportCoordPoints, ISupportIntermediatePoints {
		internal const double DefaultX1 = 0.0;
		internal const double DefaultX2 = 10.0;
		internal const double DefaultY1 = 0.0;
		internal const double DefaultY2 = 10.0;
		const bool DefaultIsGeodesic = false;
		CoordPoint point1;
		CoordPoint point2;
		bool isGeodesic = DefaultIsGeodesic;
		IList<IList<CoordPoint>> actualPoints;
		#region Style properties
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color Fill { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color HighlightedFill { get { return Color.Empty; } set { ; } }
		[SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "value"),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)]
		public new Color SelectedFill { get { return Color.Empty; } set { ; } }
		#endregion
#if DEBUGTEST
		protected internal override MapItemType ItemType { get { return MapItemType.Line; } }
#endif
		protected internal bool ShouldUseStraightLine {
			get { return !IsGeodesic || UnitConverter.CoordinateSystem.PointType != CoordPointType.Geo; }
		}
		protected internal bool NeedPopulateActualPoints { get { return !GeometryIsValid; } }
		protected internal IList<IList<CoordPoint>> ActualVertices {
			get {
				if(NeedPopulateActualPoints)
					this.actualPoints = PopulateActualPoints();
				return actualPoints;
			}
		}
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLinePoint1"),
#endif
		Category(SRCategoryNames.Layout),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		]
		public CoordPoint Point1 {
			get { return point1; }
			set {
				if(point1 == value)
					return;
				if(!CoordinateSystemHelper.IsNumericCoordPoint(value))
					Exceptions.ThrowIncorrectCoordPointException();
				point1 = value;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Location);
			}
		}
		void ResetPoint1() { Point1 = UnitConverter.PointFactory.CreatePoint(DefaultX1, DefaultY1); }
		bool ShouldSerializePoint1() { return Point1 != UnitConverter.PointFactory.CreatePoint(DefaultX1, DefaultY1); }
		[
#if !SL
	DevExpressXtraMapLocalizedDescription("MapLinePoint2"),
#endif
		Category(SRCategoryNames.Layout),
		TypeConverter("DevExpress.XtraMap.Design.CoordPointTypeConverter," + "DevExpress.XtraMap" + AssemblyInfo.VSuffixDesign),
		]
		public CoordPoint Point2 {
			get { return point2; }
			set {
				if(point2 == value)
					return;
				if(!CoordinateSystemHelper.IsNumericCoordPoint(value))
					Exceptions.ThrowIncorrectCoordPointException();
				point2 = value;
				UpdateBoundingRect();
				UpdateItem(MapItemUpdateType.Location);
			}
		}
		void ResetPoint2() { Point2 = UnitConverter.PointFactory.CreatePoint(DefaultX2, DefaultY2); }
		bool ShouldSerializePoint2() { return Point2 != UnitConverter.PointFactory.CreatePoint(DefaultX2, DefaultY2); }
		[
		Category(SRCategoryNames.Layout),
		DefaultValue(DefaultIsGeodesic)
		]
		public bool IsGeodesic {
			get { return isGeodesic; }
			set {
				if(isGeodesic == value)
					return;
				isGeodesic = value;
				UpdateItem(MapItemUpdateType.Layout);
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public IEnumerable<CoordPoint> ActualPoints { get { return ActualVertices.SelectMany(x => x); } }
		#region ISupportCoordPoints implementation
		IList<CoordPoint> ISupportCoordPoints.Points { get { return GetItemPoints(); } }
		#endregion
		#region ISupportIntermediatePoints implementation
		IList<CoordPoint> ISupportIntermediatePoints.Vertices { get { return ActualPoints.ToList(); } }
		#endregion
		public MapLine() {
			point1 = UnitConverter.PointFactory.CreatePoint(DefaultX1, DefaultY1);
			point2 = UnitConverter.PointFactory.CreatePoint(DefaultX2, DefaultY2);
		}
		IList<IList<CoordPoint>> PopulateActualPoints() {
			if(ShouldUseStraightLine)
				return new CoordPoint[][] { new CoordPoint[] { point1, point2 } };
			return PopulateGeodesicPoints();
		}
		IList<IList<CoordPoint>> PopulateGeodesicPoints() {
			return OrthodromeHelper.CalculateLine(point1, point2);
		}
		protected override Color GetFill() {
			return Color.Empty;
		}
		protected override IMapItemGeometry CreateShapeGeometry() {
			List<MapUnit[]> units = new List<MapUnit[]>();
			for(int i = 0; i < ActualVertices.Count; i++) {
				units.Add(new MapUnit[ActualVertices[i].Count]);
				for(int j = 0; j < ActualVertices[i].Count; j++) {
					MapUnit unit = UnitConverter.CoordPointToMapUnit(actualPoints[i][j], false);
					units[i][j] = new MapUnit(unit.X, unit.Y) * MapShape.RenderScaleFactor;
				}
			}
			return new MultiLineUnitGeometry() { Bounds = base.Bounds, Type = UnitGeometryType.Linear, Segments = units };
		}
		protected override MapRect CalculateBounds() {
			if(Layer == null || Layer.Map == null)
				return MapRect.Empty;
			MapUnit unit = UnitConverter.CoordPointToMapUnit(ActualVertices[0][0], false);
			MapUnit min = unit;
			MapUnit max = unit;
			for(int i = 0; i < ActualVertices.Count; i++) {
				for(int j = 0; j < ActualVertices[i].Count; j++) {
					unit = UnitConverter.CoordPointToMapUnit(ActualVertices[i][j], false);
					if(unit.X > max.X) max.X = unit.X;
					if(unit.Y > max.Y) max.Y = unit.Y;
					if(unit.X < min.X) min.X = unit.X;
					if(unit.Y < min.Y) min.Y = unit.Y;
				}
			}
			MapPoint p1 = UnitConverter.MapUnitToScreenPoint(min, false);
			MapPoint p2 = UnitConverter.MapUnitToScreenPoint(max, false);
			p1 -= ActualStyle.StrokeWidth / 2;
			p2 += ActualStyle.StrokeWidth / 2;
			min = UnitConverter.ScreenPointToMapUnit(p1, false);
			max = UnitConverter.ScreenPointToMapUnit(p2, false);
			return new MapRect(min.X, min.Y, max.X - min.X, max.Y - min.Y);
		}
		protected override CoordBounds CalculateNativeBounds() {
			return CoordPointHelper.SelectItemBounds(this);
		}
		protected override void OnStrokeWidthChanged() {
			base.OnStrokeWidthChanged();
			UpdateItem(MapItemUpdateType.Layout);
		}
		protected internal override MapElementStyleBase GetDefaultItemStyle() {
			return DefaultStyleProvider.LineStyle;
		}
		protected override IHitTestGeometry CreateHitTestGeometry() {
			IList<PointF[]> points = new List<PointF[]>();
			for(int i = 0; i < ActualVertices.Count; i++) {
				points.Add(new PointF[ActualVertices[i].Count]);
				for(int j = 0; j < ActualVertices[i].Count; j++)
					points[i][j] = UnitConverter.CoordPointToScreenPoint(ActualVertices[i][j]).ToPoint();
			}
			return new MultiLineHitTestGeometry(points, ActualStyle.StrokeWidth);
		}
		protected internal override IList<CoordPoint> GetItemPoints() {
			return new List<CoordPoint>() { Point1, Point2 };
		}
		protected internal override CoordPoint GetCenterCore() {
			return UnitConverter.PointFactory.CreatePoint(0.5 * (Point1.GetX() + Point2.GetX()), 0.5 * (Point1.GetY() + Point2.GetY()));
		}
		protected internal override bool? CalculateDefaultIsInShapeBounds() {
			return true;
		}
		public override string ToString() {
			return "(MapLine)";
		}
	}
}
