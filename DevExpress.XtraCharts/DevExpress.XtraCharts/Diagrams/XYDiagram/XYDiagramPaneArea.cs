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

using System.Drawing;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Native {
	public class XYDiagramPaneArea {
		static PaneAreaSide GetIntersectedSide(int intersectedBound, int bound1, int bound2, PaneAreaSide side1, PaneAreaSide side2) {
			if(intersectedBound == bound1)
				return side1;
			if(intersectedBound == bound2)
				return side2;
			ChartDebug.Fail("Invalid intersection of pane areas.");
			return PaneAreaSide.CreateOuter();
		}
		public static XYDiagramPaneArea Intersect(XYDiagramPaneArea paneArea1, XYDiagramPaneArea paneArea2) {
			if(paneArea1 == null || paneArea2 == null)
				return null;
			if(paneArea1.raggedGeometry != paneArea2.raggedGeometry) {
				ChartDebug.Fail("The same ragged geometry expected.");
				return null;
			}
			if(paneArea1.wavedGeometry != paneArea2.wavedGeometry) {
				ChartDebug.Fail("The same waved geometry expected.");
				return null;
			}
			Rectangle bounds = Rectangle.Intersect(paneArea1.bounds, paneArea2.bounds);
			if(bounds == Rectangle.Empty)
				return null;
			PaneAreaSide leftSide = GetIntersectedSide(bounds.Left, paneArea1.bounds.Left, paneArea2.bounds.Left, paneArea1.leftSide, paneArea2.leftSide);
			PaneAreaSide topSide = GetIntersectedSide(bounds.Top, paneArea1.bounds.Top, paneArea2.bounds.Top, paneArea1.topSide, paneArea2.topSide);
			PaneAreaSide rightSide = GetIntersectedSide(bounds.Right, paneArea1.bounds.Right, paneArea2.bounds.Right, paneArea1.rightSide, paneArea2.rightSide);
			PaneAreaSide bottomSide = GetIntersectedSide(bounds.Bottom, paneArea1.bounds.Bottom, paneArea2.bounds.Bottom, paneArea1.bottomSide, paneArea2.bottomSide);
			return new XYDiagramPaneArea(paneArea1.raggedGeometry, paneArea1.wavedGeometry, bounds, leftSide, topSide, rightSide, bottomSide);
		}
		readonly RaggedEdgeGeometry raggedGeometry;
		readonly WavedEdgeGeometry wavedGeometry;
		readonly PaneAreaSide leftSide;
		readonly PaneAreaSide topSide;
		readonly PaneAreaSide rightSide;
		readonly PaneAreaSide bottomSide;		
		Rectangle bounds;
		GraphicsPath leftPath;
		GraphicsPath topPath;
		GraphicsPath rightPath;
		GraphicsPath bottomPath;
		GraphicsPath borderPath;
		public PaneAreaSide LeftSide { get { return leftSide; } }
		public PaneAreaSide TopSide { get { return topSide; } }
		public PaneAreaSide RightSide { get { return rightSide; } }
		public PaneAreaSide BottomSide { get { return bottomSide; } }
		public Rectangle Bounds { get { return bounds; } }
		public GraphicsPath BorderPath {
			get {
				if(borderPath == null)
					borderPath = CreateBorderPath();
				return borderPath;
			}
		}
		public XYDiagramPaneArea(RaggedEdgeGeometry raggedGeometry, WavedEdgeGeometry wavedGeometry, Rectangle bounds)
			: this(raggedGeometry, wavedGeometry, bounds, PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter(), PaneAreaSide.CreateOuter()) { }
		public XYDiagramPaneArea(RaggedEdgeGeometry raggedGeometry, WavedEdgeGeometry wavedGeometry, Rectangle bounds, PaneAreaSide leftSide, PaneAreaSide topSide, 
			PaneAreaSide rightSide, PaneAreaSide bottomSide) {
			this.raggedGeometry = raggedGeometry;
			this.wavedGeometry = wavedGeometry;
			this.bounds = bounds;
			this.leftSide = leftSide;
			this.topSide = topSide;
			this.rightSide = rightSide;
			this.bottomSide = bottomSide;
		}
		public void Complete() {
			EdgeGeometry leftSideGeometry = GetEdgeGeometryByScaleBreakStyle(leftSide.Style);
			if(leftSideGeometry != null)
				leftPath = leftSideGeometry.CreateLeftSideGraphicsPath(bounds);
			EdgeGeometry topSideGeometry = GetEdgeGeometryByScaleBreakStyle(topSide.Style);
			if(topSideGeometry != null)
				topPath = topSideGeometry.CreateTopSideGraphicsPath(bounds);
			EdgeGeometry rightSideGeometry = GetEdgeGeometryByScaleBreakStyle(rightSide.Style);
			if(rightSideGeometry != null)
				rightPath = rightSideGeometry.CreateRightSideGraphicsPath(bounds);
			EdgeGeometry bottomSideGeometry = GetEdgeGeometryByScaleBreakStyle(bottomSide.Style);
			if(bottomSideGeometry != null)
				bottomPath = bottomSideGeometry.CreateBottomSideGraphicsPath(bounds);
		}
		EdgeGeometry GetEdgeGeometryByScaleBreakStyle(ScaleBreakStyle style) {
			switch(style) {
				case ScaleBreakStyle.Ragged:
					return raggedGeometry;
				case ScaleBreakStyle.Waved:
					return wavedGeometry;
				default:
					return null;
			}
		}
		GraphicsPath CreateBorderPath() {
			GraphicsPath path = new GraphicsPath();
			if(topPath != null)
				path.AddPath(topPath, false);
			else
				path.AddLine(bounds.Left, bounds.Top, bounds.Right, bounds.Top);
			if(rightPath != null)
				path.AddPath(rightPath, true);
			else
				path.AddLine(bounds.Right, bounds.Top, bounds.Right, bounds.Bottom);
			if(bottomPath != null) {
				GraphicsPath reversedPath = (GraphicsPath)bottomPath.Clone();
				reversedPath.Reverse();
				path.AddPath(reversedPath, true);
			}
			else
				path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
			if(leftPath != null) {
				GraphicsPath reversedPath = (GraphicsPath)leftPath.Clone();
				reversedPath.Reverse();
				path.AddPath(reversedPath, true);
			}
			else
				path.AddLine(bounds.Left, bounds.Bottom, bounds.Left, bounds.Top);
			path.CloseFigure();
			return path;
		}
		public void RenderBorder(IRenderer renderer, int thickness, Color color, bool isInner) {
			if (topSide.IsInner != isInner && bottomSide.IsInner != isInner && leftSide.IsInner != isInner && rightSide.IsInner != isInner)
				return;
			if (thickness <= 0)
				return;
			int halfFirst, halfSecond;
			ThicknessUtils.SplitToHalfs(thickness, out halfFirst, out halfSecond);
			int left = bounds.Left - halfSecond;
			int top = bounds.Top - halfSecond;
			int right = bounds.Right - halfFirst - 1;
			int bottom = bounds.Bottom - halfFirst - 1;
			Size horizontalBorderSize = new Size(bounds.Width + halfSecond * 2, thickness);
			Size verticalBorderSize = new Size(thickness, bounds.Height);
			if (topSide.IsInner == isInner) {
				Color topColor = topSide.Color.IsEmpty ? color : topSide.Color;
				if (topPath != null)
					renderer.DrawPath(topPath, topColor, 1);
				else
					renderer.FillRectangle(new RectangleF(left, top, horizontalBorderSize.Width, horizontalBorderSize.Height), topColor);
			}
			if (bottomSide.IsInner == isInner) {
				Color bottomColor = bottomSide.Color.IsEmpty ? color : bottomSide.Color;
				if (bottomPath != null)
					renderer.DrawPath(bottomPath, bottomColor, 1);
				else
					renderer.FillRectangle(new RectangleF(left, bottom, horizontalBorderSize.Width, horizontalBorderSize.Height), bottomColor);
			}
			if (leftSide.IsInner == isInner) {
				Color leftColor = leftSide.Color.IsEmpty ? color : leftSide.Color;
				if (leftPath != null)
					renderer.DrawPath(leftPath, leftColor, 1);
				else
					renderer.FillRectangle(new RectangleF(left, bounds.Top, verticalBorderSize.Width, verticalBorderSize.Height), leftColor);
			}
			if (rightSide.IsInner == isInner) {
				Color rightColor = rightSide.Color.IsEmpty ? color : rightSide.Color;
				if (rightPath != null)
					renderer.DrawPath(rightPath, rightColor, 1);
				else
					renderer.FillRectangle(new RectangleF(right, bounds.Top, verticalBorderSize.Width, verticalBorderSize.Height), rightColor);
			}
		}
	}
	public class XYDiagramPaneAreas : List<XYDiagramPaneArea> {
		readonly bool shouldCalculateHitRegions;
		readonly Dictionary<XYDiagramMappingBase, Region> seriesClipRegions = new Dictionary<XYDiagramMappingBase, Region>();
		readonly Dictionary<XYDiagramMappingBase, HitRegionContainer> seriesHitRegions = new Dictionary<XYDiagramMappingBase, HitRegionContainer>();
		Region clipRegion;
		HitRegionContainer hitRegion;
		public Region ClipRegion { get { return clipRegion; } }
		public HitRegionContainer HitRegion { get { return hitRegion; } }
		public Dictionary<XYDiagramMappingBase, Region> SeriesClipRegions { get { return seriesClipRegions; } }
		public XYDiagramPaneAreas(RaggedEdgeGeometry raggedGeometry, WavedEdgeGeometry wavedGeometry, List<AxisViewData> axisViewDataList, XYDiagramSeriesLayoutList seriesLayoutList, bool shouldCalculateHitRegions) {
			this.shouldCalculateHitRegions = shouldCalculateHitRegions;
			if (axisViewDataList.Count > 0) {
				List<XYDiagramPaneArea> areas = XYDiagramPaneAreaGenerator.GenerateAreas(raggedGeometry, wavedGeometry, axisViewDataList[0]);
				for (int i = 1; i < axisViewDataList.Count; i++) {
					List<XYDiagramPaneArea> areas_i = XYDiagramPaneAreaGenerator.GenerateAreas(raggedGeometry, wavedGeometry, axisViewDataList[i]);
					List<XYDiagramPaneArea> newAreas = new List<XYDiagramPaneArea>();
					foreach (XYDiagramPaneArea area_i in areas_i)
						foreach (XYDiagramPaneArea area in areas) {
							XYDiagramPaneArea newArea = XYDiagramPaneArea.Intersect(area_i, area);
							if (newArea != null)
								newAreas.Add(newArea);
						}
					areas = newAreas;
				}
				foreach (XYDiagramPaneArea area in areas) {
					area.Complete();
					Add(area);
				}
				CalculateClipRegion();
				if(shouldCalculateHitRegions)
					CalculateHitRegion();
				CalculateClipRegions(seriesLayoutList.MappingList, shouldCalculateHitRegions);
			}
		}
		List<XYDiagramPaneArea> GetActualPaneAreas(XYDiagramMappingBase mapping) {
			Rectangle mappingBounds = GraphicUtils.InflateRect(mapping.Bounds, -1, -1);
			List<XYDiagramPaneArea> actualAreas = new List<XYDiagramPaneArea>();
			foreach(XYDiagramPaneArea area in this)
				if(area.Bounds.IntersectsWith(mappingBounds))
					actualAreas.Add(area);
			return actualAreas;
		}
		void CalculateClipRegions(XYDiagramMappingList mappingList, bool shouldCalculateHitRegions) {
			foreach (XYDiagramMappingContainer mappingContainer in mappingList.MappingContainers)
				foreach (XYDiagramMappingBase mapping in mappingContainer)
					CalculateSeriesRegions(mapping, shouldCalculateHitRegions);
		}
		void CalculateClipRegion() {
			clipRegion = new Region(Rectangle.Empty);
			foreach(XYDiagramPaneArea area in this)
				clipRegion.Union(area.BorderPath);
		}
		void CalculateHitRegion() {
			hitRegion = new HitRegionContainer();
			foreach(XYDiagramPaneArea area in this)
				hitRegion.Union(new HitRegion(area.BorderPath));
		}
		void CalculateSeriesRegions(XYDiagramMappingBase mapping, bool shouldCalculateHitRegions) {
			List<XYDiagramPaneArea> actualAreas = GetActualPaneAreas(mapping);
			CalculateSeriesClipRegions(mapping, actualAreas);
			if(shouldCalculateHitRegions)
				CalculateSeriesHitRegions(mapping, actualAreas);
		}
		void CalculateSeriesClipRegions(XYDiagramMappingBase mapping, List<XYDiagramPaneArea> actualAreas) {
			List<Region> clipRegions = new List<Region>();
			foreach(XYDiagramPaneArea area in actualAreas) {
				Region clipRegion = new Region(mapping.InflatedBounds);
				clipRegion.Intersect(area.BorderPath);
				clipRegions.Add(clipRegion);
			}
			Region seriesClipRegion = null;
			if(clipRegions.Count > 0) {
				seriesClipRegion = new Region(Rectangle.Empty);
				foreach(Region clipRegion in clipRegions)
					seriesClipRegion.Union(clipRegion);
			}
			if (!seriesClipRegions.ContainsKey(mapping))
				seriesClipRegions.Add(mapping, seriesClipRegion);
		}
		void CalculateSeriesHitRegions(XYDiagramMappingBase mapping, List<XYDiagramPaneArea> actualAreas) {
			List<HitRegionContainer> hitRegions = new List<HitRegionContainer>();
			foreach(XYDiagramPaneArea area in actualAreas) {
				HitRegionContainer hitRegion = new HitRegionContainer(new HitRegion(mapping.InflatedBounds));
				hitRegion.Intersect(new HitRegion(area.BorderPath));
				hitRegions.Add(hitRegion);
			}
			HitRegionContainer seriesHitRegion = null;
			if(hitRegions.Count > 0) {
				seriesHitRegion = new HitRegionContainer();
				foreach(HitRegionContainer hitRegion in hitRegions)
					seriesHitRegion.Union(hitRegion);
			}
			if (!seriesHitRegions.ContainsKey(mapping))
				seriesHitRegions.Add(mapping, seriesHitRegion);
		}
		public HitRegionContainer GetSeriesHitRegion(XYDiagramMappingBase mapping) {
			return seriesHitRegions.ContainsKey(mapping) ? seriesHitRegions[mapping] : null;
		}
		public void UpdateClipRegions(IndicatorLayoutList indicatorsLayout) {
			CalculateClipRegions(indicatorsLayout.MappingList, shouldCalculateHitRegions);
		}
	}
}
