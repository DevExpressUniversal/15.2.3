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
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class Bar3DRepresentationCalculator {
		const int segmentsCountMin = 5;
		const int segmentsCountMax = 36;
		const double facetPercent = 0.05;
		Bar3DSeriesView view;
		bool IsCenterVerticalGradientMode {
			get {
				if (view.FillStyle.FillMode != FillMode3D.Gradient)
					return false;
				RectangleGradientFillOptions options = view.FillStyle.Options as RectangleGradientFillOptions;
				if (options == null)
					return false;
				return options.GradientMode == RectangleGradientMode.ToCenterVertical ||
					options.GradientMode == RectangleGradientMode.FromCenterVertical;
			}
		} 
		public Bar3DRepresentationCalculator(Bar3DSeriesView view) {
			this.view = view;
		}
		int CalcSegmentsCount(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData) {
			double width, depth;
			view.CalcWidthAndDepth(coordsCalculator, barData.Width, out width, out depth);
			int segmentsCount = (int)(Math.Max(width, depth) * 0.004 * coordsCalculator.Diagram.ZoomPercent);
			if (segmentsCount < segmentsCountMin)
				segmentsCount = segmentsCountMin;
			else if (segmentsCount > segmentsCountMax)
				segmentsCount = segmentsCountMax;
			return segmentsCount;
		}
		double CalcFacetSize(YPlaneRectangle barBase, double barHeight) {
			double minSize = Math.Min(barBase.Height, barBase.Width);
			double facetSize = minSize * facetPercent;
			if (facetSize > Math.Abs(barHeight))
				facetSize = Math.Abs(barHeight);
			return facetSize;
		}
		void CalcPointParams(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData, out YPlaneRectangle barBase, out double height, out double beginHeight, out double endHeight) {
			double argument = barData.DisplayArgument;
			double width, depth;
			view.CalcWidthAndDepth(coordsCalculator, barData.Width, out width, out depth);
			double halfWidth = width / 2.0;
			double halfDepth = depth / 2.0;
			DiagramPoint bottom = coordsCalculator.GetDiagramPoint(view.Series, argument, 0.0, false);
			DiagramPoint barBottom = coordsCalculator.GetDiagramPoint(view.Series, argument, barData.ZeroValue, false);
			DiagramPoint barTop = coordsCalculator.GetDiagramPoint(view.Series, argument, barData.ActualValue, false);
			DiagramPoint top = coordsCalculator.GetDiagramPoint(view.Series, argument, barData.MaxValue, false);
			bottom.X += barData.FixedOffset;
			DiagramPoint leftBack = DiagramPoint.Offset(bottom, -halfWidth, 0, -halfDepth);
			height = top.Y - bottom.Y;
			beginHeight = barBottom.Y - bottom.Y;
			endHeight = barTop.Y - bottom.Y;
			barBase = new YPlaneRectangle(leftBack, depth, width);
		}
		void CalcPointParams(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData, out YPlaneRectangle barBase, out double height) {
			double beginHeight, endHeight;
			CalcPointParams(coordsCalculator, barData, out barBase, out height, out beginHeight, out endHeight);
			barBase.Offset(0, beginHeight, 0);
			height = endHeight - beginHeight;
		}
		PlanePolygon[] ConstructBox(YPlaneRectangle boxBase, double height, bool constructWithTopBase, bool constructWithBottomBase) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			Box box = new Box(boxBase, height);
			result.AddRange(box.GetLaterals());
			YPlaneRectangle bottomBase = height > 0 ? box.Bottom : box.Top;
			if (constructWithBottomBase) {
				result.Add(bottomBase.CalcTopHalf());
				result.Add(bottomBase.CalcBottomHalf());
			}
			YPlaneRectangle topBase = height > 0 ? box.Top : box.Bottom;
			if (constructWithTopBase) {
				result.Add(topBase.CalcTopHalf());
				result.Add(topBase.CalcBottomHalf());
			}
			return result.ToArray();
		}
		PlanePolygon[] ConstructFacetBox(YPlaneRectangle boxBase, double height, bool constructWithBottomBase) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			double facetSize = CalcFacetSize(boxBase, height);
			double boxHeight = height > 0 ? height - facetSize : height + facetSize;
			Box box = new Box(boxBase, boxHeight);
			YPlaneRectangle bottomBase = boxHeight > 0 ? box.Bottom : box.Top;
			if (constructWithBottomBase) {
				result.Add(bottomBase.CalcTopHalf());
				result.Add(bottomBase.CalcBottomHalf());
			}
			result.AddRange(box.GetLaterals());
			YPlaneRectangle pyramidBottomBase = (YPlaneRectangle)YPlaneRectangle.Offset(boxBase, 0, boxHeight, 0);
			YPlaneRectangle pyramidTopBase = (YPlaneRectangle)YPlaneRectangle.Inflate(boxBase, -facetSize, -facetSize);
			pyramidTopBase.Offset(0, height, 0);
			Pyramid pyramid = new Pyramid(pyramidBottomBase, pyramidTopBase);
			result.AddRange(pyramid.Laterals);
			if (pyramid.TopBase != null)
				result.Add(pyramid.TopBase);
			return result.ToArray();
		}
		PlanePolygon[] CalcBoxRepresentation(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData, out Box boundingBox) {
			YPlaneRectangle barBase;
			double height;
			CalcPointParams(coordsCalculator, barData, out barBase, out height);
			boundingBox = new Box(barBase, height);
			List<PlanePolygon> representation = new List<PlanePolygon>();
			if (IsCenterVerticalGradientMode) {
				representation.AddRange(ConstructBox(barBase, height / 2, false, true));
				YPlaneRectangle centerBase = (YPlaneRectangle)YPlaneRectangle.Offset(barBase, 0, height / 2, 0);
				if (barData.ShowFacet)
					representation.AddRange(ConstructFacetBox(centerBase, height / 2, false));
				else
					representation.AddRange(ConstructBox(centerBase, height / 2, true, false));
			}
			else {
				if (barData.ShowFacet)
					representation.AddRange(ConstructFacetBox(barBase, height, true));
				else
					representation.AddRange(ConstructBox(barBase, height, true, true));
			}
			return representation.ToArray();
		}
		PlanePolygon[] ConstructCylinder(YPlaneRectangle cylinderBase, double height, int segmentsCount, bool consrtuctWithTopBase, bool consrtuctWithBottomBase) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			Cylinder cylinder = new Cylinder(cylinderBase, height, segmentsCount);
			result.AddRange(cylinder.Laterals);
			if (consrtuctWithBottomBase)
				result.Add(cylinder.BottomBase);
			if (consrtuctWithTopBase)
				result.Add(cylinder.TopBase);
			return result.ToArray();
		}
		PlanePolygon[] ConstructFacetCylinder(YPlaneRectangle cylinderBase, double height, int segmentsCount, bool consrtuctWithBottomBase) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			double facetSize = CalcFacetSize(cylinderBase, height);
			double newHeight = height > 0 ? height - facetSize : height + facetSize;
			Cylinder cylinder = new Cylinder(cylinderBase, newHeight, segmentsCount);
			if (consrtuctWithBottomBase)
				result.Add(cylinder.BottomBase);
			result.AddRange(cylinder.Laterals);
			YPlaneRectangle coneBottomBase = (YPlaneRectangle)YPlaneRectangle.Offset(cylinderBase, 0, newHeight, 0);
			YPlaneRectangle coneTopBase = (YPlaneRectangle)YPlaneRectangle.Inflate(cylinderBase, -facetSize, -facetSize);
			coneTopBase.Offset(0, height, 0);
			Cone cone = new Cone(coneBottomBase, coneTopBase, segmentsCount);
			result.AddRange(cone.Laterals);
			if (cone.TopBase != null)
				result.Add(cone.TopBase);
			return result.ToArray();
		}
		PlanePolygon[] CalcCylinderRepresentation(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData, out Box boundingBox) {
			YPlaneRectangle barBase;
			double height;
			CalcPointParams(coordsCalculator, barData, out barBase, out height);
			boundingBox = new Box(barBase, height);
			int segmentsCount = CalcSegmentsCount(coordsCalculator, barData);
			List<PlanePolygon> representation = new List<PlanePolygon>();
			if (IsCenterVerticalGradientMode) {
				representation.AddRange(ConstructCylinder(barBase, height / 2, segmentsCount, false, true));
				YPlaneRectangle centerBase = (YPlaneRectangle)YPlaneRectangle.Offset(barBase, 0, height / 2, 0);
				if (barData.ShowFacet)
					representation.AddRange(ConstructFacetCylinder(centerBase, height / 2, segmentsCount, false));
				else
					representation.AddRange(ConstructCylinder(centerBase, height / 2, segmentsCount, true, false));
			}
			else {
				if (barData.ShowFacet)
					representation.AddRange(ConstructFacetCylinder(barBase, height, segmentsCount, true));
				else
					representation.AddRange(ConstructCylinder(barBase, height, segmentsCount, true, true));
			}
			return representation.ToArray();
		}
		PlanePolygon[] ConstructConeOrPyramid(YPlaneRectangle barBase, double height, double beginHeight, double endHeight, int segmentsCount, bool constructWithTopBase, bool consrtuctWithBottomBase) {
			List<PlanePolygon> result = new List<PlanePolygon>();
			Cone primitive;
			if (view.Model == Bar3DModel.Cone)
				primitive = new Cone(barBase, height, beginHeight, endHeight, segmentsCount);
			else
				primitive = new Pyramid(barBase, height, beginHeight, endHeight);
			result.AddRange(primitive.Laterals);
			if (consrtuctWithBottomBase)
				result.Add(primitive.BottomBase);
			if(constructWithTopBase && primitive.TopBase != null)
				result.Add(primitive.TopBase);
			return result.ToArray();
		}
		PlanePolygon[] CalcConeOrPyramidRepresentation(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData, out Box boundingBox) {
			YPlaneRectangle barBase;
			double height, beginHeight, endHeight;
			CalcPointParams(coordsCalculator, barData, out barBase, out height, out beginHeight, out endHeight);
			int segmentsCount = CalcSegmentsCount(coordsCalculator, barData);
			double newWidth = barBase.Height * (height - beginHeight) / height;
			double newDepth = barBase.Width * (height - beginHeight) / height;
			DiagramPoint location = DiagramPoint.Offset(barBase.Location, (barBase.Height - newWidth) / 2, beginHeight, 0);
			boundingBox = new Box(location, newWidth, endHeight - beginHeight, newDepth);
			if (IsCenterVerticalGradientMode) {
				List<PlanePolygon> result = new List<PlanePolygon>();
				double centerHeight = (beginHeight + endHeight) / 2;
				result.AddRange(ConstructConeOrPyramid(barBase, height, centerHeight, endHeight, segmentsCount, true, false));
				result.AddRange(ConstructConeOrPyramid(barBase, height, beginHeight, centerHeight, segmentsCount, false, true));
				return result.ToArray();
			}
			else
				return ConstructConeOrPyramid(barBase, height, beginHeight, endHeight, segmentsCount, true, true);
		}
		public PlanePolygon[] CalcRepresentation(XYDiagram3DCoordsCalculator coordsCalculator, Bar3DData barData, out Box boundingBox) {
			switch (view.Model) {
				case Bar3DModel.Box:
					return CalcBoxRepresentation(coordsCalculator, barData, out boundingBox);
				case Bar3DModel.Cone:
					return CalcConeOrPyramidRepresentation(coordsCalculator, barData, out boundingBox);
				case Bar3DModel.Cylinder:
					return CalcCylinderRepresentation(coordsCalculator, barData, out boundingBox);
				case Bar3DModel.Pyramid:
					return CalcConeOrPyramidRepresentation(coordsCalculator, barData, out boundingBox);
				default:
					ChartDebug.Fail("Unknown Bar3D model.");
					boundingBox = null;
					return null;
			}
		}
	}
}
