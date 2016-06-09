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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class XYDiagram3DDomain : Diagram3DDomain {
		readonly Axis3DInfo axisXInfo;
		readonly Axis3DInfo axisYInfo;
		Box bottom;
		Box back;
		Box fore;
		Box left;
		Box right;
		Material backgroundMaterial;
		MaterialGroup axisXMaterial;
		MaterialGroup axisYMaterial;
		MaterialGroup axesMaterial;
		protected new XYDiagram3DBox DiagramBox { get { return (XYDiagram3DBox)base.DiagramBox; } }
		public new XYDiagram3D Diagram { get { return (XYDiagram3D)base.Diagram; } }
		public Box Bottom { get { return bottom; } }
		public Box Back { get { return back; } }
		public Box Fore { get { return fore; } }
		public Box Left { get { return left; } }
		public Box Right { get { return right; } }
		public XYDiagram3DDomain(XYDiagram3D diagram, Rect viewport) : base(diagram, viewport, diagram.CreateSeriesDataList(), new XYDiagram3DBox(diagram)) {
			AxisX3D axisX = Diagram.ActualAxisX;
			AxisY3D axisY = Diagram.ActualAxisY;
			GridAndTextDataEx xData = new GridAndTextDataEx(axisX.GetSeries(), axisX, false, ((IAxisData)axisX).VisualRange, ((IAxisData)axisX).VisualRange, viewport.Width, (axisX.ActualLabel == null) ? false : axisX.ActualLabel.Staggered);
			GridAndTextDataEx yData = new GridAndTextDataEx(axisY.GetSeries(), axisY, false, ((IAxisData)axisY).VisualRange, ((IAxisData)axisY).VisualRange, viewport.Height, (axisY.ActualLabel == null) ? false : axisY.ActualLabel.Staggered);
			AxisCache axisXCache = Diagram.Cache.GetAxisCache(axisX);
			AxisCache axisYCache = Diagram.Cache.GetAxisCache(axisY);
			Rect bounds = DiagramBox.Bounds;
			axisXInfo = new Axis3DInfo(this, axisX, xData, axisXCache, bounds);
			axisYInfo = new Axis3DInfo(this, axisY, yData, axisYCache, bounds);
		}
		void CreateDiagramModel(XYDiagram3DCache cache, Model3DGroup modelHolder) {
			modelHolder.Children.Add(cache.Bottom);
			BoxPlane planes = CalcVisiblePlanes();
			if((planes & BoxPlane.Back) > 0)
				modelHolder.Children.Add(cache.Back);
			if((planes & BoxPlane.Fore) > 0)
				modelHolder.Children.Add(cache.Fore);
			if((planes & BoxPlane.Left) > 0)
				modelHolder.Children.Add(cache.Left);
			if((planes & BoxPlane.Right) > 0)
				modelHolder.Children.Add(cache.Right);
		}
		void CalcDiagramPlanes() {
			double correction = XYDiagram3DBox.DiagramPlanesCorrection;
			Point3D zeroPoint = new Point3D(-correction, -correction, -correction);
			correction *= 2;
			double width = DiagramBox.InnerWidth + correction;
			double height = DiagramBox.InnerHeight + correction;
			double depth = DiagramBox.InnerDepth + correction;
			bottom = new Box(zeroPoint, width, -Diagram.PlaneDepthFixed, depth);
			back = new Box(zeroPoint, width, height, -Diagram.PlaneDepthFixed);
			fore = new Box(MathUtils.Offset(zeroPoint, 0, 0, depth), width, height, Diagram.PlaneDepthFixed);
			left = new Box(zeroPoint, -Diagram.PlaneDepthFixed, height, depth);
			right = new Box(MathUtils.Offset(zeroPoint, width, 0, 0), Diagram.PlaneDepthFixed, height, depth);
		}
		void CreateDiagramMaterials() {
			backgroundMaterial = CreateMaterial(Diagram.DomainBrush);
			axisXMaterial = new MaterialGroup();
			axisXMaterial.Children.Add(backgroundMaterial);
			Material material = CreateMaterial(axisXInfo, false);
			if(material != null)
				axisXMaterial.Children.Add(material);
			axisYMaterial = new MaterialGroup();
			axisYMaterial.Children.Add(backgroundMaterial);
			material = CreateMaterial(axisYInfo, true);
			if(material != null)
				axisYMaterial.Children.Add(material);
			axesMaterial = new MaterialGroup();
			axesMaterial.Children.Add(backgroundMaterial);
			material = CreateMaterial(axisXInfo, axisYInfo);
			if(material != null)
				axesMaterial.Children.Add(material);
		}
		Material CreateMaterial(Brush brush) {
			if(Diagram.Material == null || brush == null)
				return null;
			Material material = Diagram.Material.CloneCurrentValue();
			Graphics3DUtils.SetMaterialBrush(material, brush, false);
			return material;
		}
		Material CreateMaterial(Axis3DInfo axisInfo, bool isVertical) {
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen()) {
				int depth = (int)Math.Ceiling(DiagramBox.InnerDepth);
				axisInfo.RenderInterlaced(drawingContext, depth, isVertical);
				axisInfo.RenderGridLines(drawingContext, depth, isVertical);
				drawingContext.Close();
				Size size = isVertical ? new Size(DiagramBox.InnerDepth, DiagramBox.InnerHeight) : new Size(DiagramBox.InnerWidth, DiagramBox.InnerDepth);
				return CreateMaterial(CreateBrush(drawingVisual, size));
			}
		}
		Material CreateMaterial(Axis3DInfo axisXInfo, Axis3DInfo axisYInfo) {
			DrawingVisual drawingVisual = new DrawingVisual();
			using (DrawingContext drawingContext = drawingVisual.RenderOpen()) {
				axisXInfo.RenderInterlaced(drawingContext, axisXInfo.AxisMapping.OrthSize, true);
				axisYInfo.RenderInterlaced(drawingContext, axisYInfo.AxisMapping.OrthSize, true);
				axisXInfo.RenderGridLines(drawingContext, axisXInfo.AxisMapping.OrthSize, true);
				axisYInfo.RenderGridLines(drawingContext, axisYInfo.AxisMapping.OrthSize, true);
				drawingContext.Close();
				return CreateMaterial(CreateBrush(drawingVisual, new Size(DiagramBox.InnerWidth, DiagramBox.InnerHeight)));
			}
		}
		ImageBrush CreateBrush(DrawingVisual drawingVisual, Size size) {
			RenderTargetBitmap bitmap = new RenderTargetBitmap((int)size.Width > 0 ? (int)size.Width : 1, (int)size.Height > 0 ? (int)size.Height : 1, 96, 96, PixelFormats.Default);
			bitmap.Render(drawingVisual);
			ImageBrush brush = new ImageBrush(bitmap);
			Graphics3DUtils.CacheBrush(brush);
			return brush;
		} 
		void CreateDiagramModel(Model3DGroup modelHolder) {
			XYDiagram3DCache cache = (XYDiagram3DCache)Diagram.Cache;
			if(!cache.IsDiagramModelValid) {
				CalcDiagramPlanes();
				CreateDiagramMaterials();
				TextureMapping();
				cache.Bottom = bottom.GetModel();
				cache.Back = back.GetModel();
				cache.Fore = fore.GetModel();
				cache.Left = left.GetModel();
				cache.Right = right.GetModel();
			}
			CreateDiagramModel(cache, modelHolder);
		}
		void BackPlaneTextureMapping() {
			back.Fore.Material = axesMaterial;
			back.Left.Material = axisYMaterial;
			back.Right.Material = axisYMaterial;
			back.Bottom.Material = axisXMaterial;
			back.Top.Material = axisXMaterial;
		}
		void ForePlaneTextureMapping() {
			fore.Back.Material = axesMaterial;
			fore.Left.Material = axisYMaterial;
			fore.Right.Material = axisYMaterial;
			fore.Bottom.Material = axisXMaterial;
			fore.Top.Material = axisXMaterial;
		}
		void LeftPlaneTextureMapping() {
			left.Right.Material = axisYMaterial;
			left.Back.Material = axisYMaterial;
			left.Fore.Material = axisYMaterial;
			left.Bottom.Material = backgroundMaterial;
			left.Top.Material = backgroundMaterial;
		}
		void RightPlaneTextureMapping() {
			right.Left.Material = axisYMaterial;
			right.Back.Material = axisYMaterial;
			right.Fore.Material = axisYMaterial;
			right.Top.Material = backgroundMaterial;
			right.Bottom.Material = backgroundMaterial;
		}
		void BottomPlaneTextureMapping() {
			bottom.Top.Material = axisXMaterial;
			bottom.Bottom.Material = axisXMaterial;
			bottom.Fore.Material = axisXMaterial;
			bottom.Back.Material = axisXMaterial;
			bottom.Left.Material = backgroundMaterial;
			bottom.Right.Material = backgroundMaterial;
		}
		void TextureMapping() {
			BackPlaneTextureMapping();
			ForePlaneTextureMapping();
			LeftPlaneTextureMapping();
			RightPlaneTextureMapping();
			BottomPlaneTextureMapping();
		}
		Model3D CreateSeriesModel() {
			XYDiagram3DCache cache = Diagram.Cache as XYDiagram3DCache;
			Model3DGroup seriesModel = cache == null ? null : cache.SeriesModel;
			if (seriesModel == null) {
				seriesModel = new Model3DGroup();
				int childrenCount = 0;
				foreach (Series3DData seriesData in SeriesDataList) {
					seriesData.CreateModel(this, seriesModel, Diagram.ViewController.GetRefinedSeries(seriesData.Series));
					for (int i = childrenCount; i < seriesModel.Children.Count; i++)
						ChartControl.SetSeriesHitTestInfo(seriesModel.Children[i], seriesData.Series as Series);
					childrenCount = seriesModel.Children.Count;
				}
				if (cache != null)
					cache.SeriesModel = seriesModel;
			}
			return seriesModel;
		}
		void CreateSeriesLabelsModel(Model3DGroup modelHolder, List<LabelModelContainer> transparentLabelContainers) {
			foreach(Series3DData seriesData in SeriesDataList)
				seriesData.CreateLabelsModel(this, modelHolder, transparentLabelContainers, Diagram.ViewController.GetRefinedSeries(seriesData.Series));
		}
		protected override void Render3DContent(Model3DGroup modelHolder) {
			CreateDiagramModel(modelHolder);
			Model3D seriesModel = CreateSeriesModel();
			if(seriesModel != null)
				modelHolder.Children.Add(seriesModel);
			List<LabelModelContainer> labelContainers = new List<LabelModelContainer>();
			CreateSeriesLabelsModel(modelHolder, labelContainers);
			axisXInfo.CreateLabelsAndTitle(labelContainers);
			axisYInfo.CreateLabelsAndTitle(labelContainers);
			labelContainers.Sort(LabelModelContainer.CompareByDistance);
			foreach(LabelModelContainer modelContainer in labelContainers)
				modelHolder.Children.Add(modelContainer.Model);
		}
		protected override void Render2DContent(DrawingVisual visualHolder) {
		}
		public double GetValueByAxisX(double value) {
			return DiagramBox.GetValueByAxisX(value);
		}
		public Point3D GetDiagramPoint(Series series, double argument, double value) {
			return DiagramBox.GetDiagramPoint(series, argument, value);
		}
		public Point GetClippedDiagramPoint(double argument, double value) {
			return DiagramBox.GetClippedDiagramPoint(argument, value);
		}
		public BoxPlane CalcVisiblePlanes() {
			BoxPlane planes = BoxPlane.Bottom;
			Point3D left = new Point3D(-Width / 2, 0, 0);
			Point3D rigth = new Point3D(Width / 2, 0, 0);
			Point3D back = new Point3D(0, 0, -Depth / 2);
			Point3D fore = new Point3D(0, 0, Depth / 2);
			left = Diagram.ActualContentTransform.Transform(left);
			rigth = Diagram.ActualContentTransform.Transform(rigth);
			back = Diagram.ActualContentTransform.Transform(back);
			fore = Diagram.ActualContentTransform.Transform(fore);
			double leftDistance = MathUtils.CalcDistance(CameraPosition, left);
			double rigthDistance = MathUtils.CalcDistance(CameraPosition, rigth);
			double backDistance = MathUtils.CalcDistance(CameraPosition, back);
			double foreDistance = MathUtils.CalcDistance(CameraPosition, fore);
			planes |= leftDistance < rigthDistance ? BoxPlane.Right : BoxPlane.Left;
			planes |= backDistance < foreDistance ? BoxPlane.Fore : BoxPlane.Back;
			return planes;
		}
		public override void OnModelAdd(object modelHolder, params Model3D[] models) {
			base.OnModelAdd(modelHolder, models);
			XYDiagram3DCache cache = Diagram.Cache as XYDiagram3DCache;
			if (cache != null)
				cache.AddModels(modelHolder, models);
		}
	}
	public class XYDiagram3DBox : Diagram3DBox {
		public const int PredefinedWidth = 1000;
		public const int DiagramPlanesCorrection = 1;
		readonly XYDiagram3D diagram;
		readonly List<List<ISeries>> seriesGroups = new List<List<ISeries>>();
		readonly double innerWidth;
		readonly double innerHeight;
		readonly double innerDepth;
		readonly IAxisMapping axisXMapping;
		readonly IAxisMapping axisYMapping;
		AxisX3D AxisX { get { return diagram.ActualAxisX; } }
		AxisY3D AxisY { get { return diagram.ActualAxisY; } }
		public double InnerWidth { get { return innerWidth; } }
		public double InnerHeight { get { return innerHeight; } }
		public double InnerDepth { get { return innerDepth; } }
		public Rect Bounds { get { return new Rect(0, 0, innerWidth + 1, innerHeight + 1); } }
		public XYDiagram3DBox(XYDiagram3D diagram) {
			this.diagram = diagram;
			CalcSeriesGroups();
			innerWidth = PredefinedWidth;
			innerHeight = PredefinedWidth * diagram.HeightToWidthRatio;
			Rect bounds = Bounds;
			axisXMapping = AxisX.CreateMapping(bounds);
			axisYMapping = AxisY.CreateMapping(bounds);
			innerDepth = CalcDiagramDepth(); 
			double correction = (diagram.PlaneDepthFixed + DiagramPlanesCorrection) * 2;
			Width = innerWidth + correction;
			Height = innerHeight + correction;
			Depth = innerDepth + correction;
		}
		void CalcSeriesGroups() {
			seriesGroups.Clear();
			List<ISeries> seriesGroup = new List<ISeries>();
			IRefinedSeries groupSeries = null;
			foreach (IRefinedSeries refinedSeries in diagram.ViewController.ActiveRefinedSeries) {
				if (GetSeriesIndex(refinedSeries.Series) < 0)
					continue;
				if (seriesGroup.Count == 0) {
					seriesGroup.Add(refinedSeries.Series);
					groupSeries = refinedSeries;
				} else {
					if (groupSeries.IsSameContainers(refinedSeries))
						seriesGroup.Add(refinedSeries.Series);
					else {
						AddSeriesGroup(seriesGroup);
						seriesGroup = new List<ISeries>();
						seriesGroup.Add(refinedSeries.Series);
						groupSeries = refinedSeries;
					}
				}
			}
			if (seriesGroup.Count > 0)
				AddSeriesGroup(seriesGroup);
		}
		int GetSeriesIndex(ISeries series) {
			for(int i = 0; i < diagram.ViewController.ActiveRefinedSeries.Count; i++)
				if (Object.ReferenceEquals(diagram.ViewController.ActiveRefinedSeries[i].Series, series))
					return i;
			return -1;
		}
		int GetSeriesGroupIndex(Series series) {
			for(int i = 0; i < seriesGroups.Count; i++) {
				if(seriesGroups[i].Contains(series))
					return i;
			}
			throw new ArgumentException("series");
		}
		void AddSeriesGroup(List<ISeries> group) {
			int index = GetSeriesIndex(group[0]);
			int pos;
			for(pos = 0; pos < seriesGroups.Count; pos++)
				if(GetSeriesIndex(seriesGroups[pos][0]) > index)
					break;
			seriesGroups.Insert(pos, group);
		}
		int GetActualVisibleSeries() {
			int actualVisibleSeries = 0;
			foreach (RefinedSeries refinedSeries in diagram.ViewController.ActiveRefinedSeries) {
				Series series = (Series)refinedSeries.Series;
				actualVisibleSeries += series.GetActualVisible() ? 1 : 0;
			}
			return actualVisibleSeries;
		}
		double CalcDiagramDepth() {
			double distance = CalcSeriesPadding() * 2;
			int visibleSeriesCount = GetActualVisibleSeries();
			if (visibleSeriesCount > 0)
				distance += CalcSeriesDistance() * (seriesGroups.Count - 1);
			return distance;
		}
		double CalcSeriesPadding() {
			return GetValueByAxisX(diagram.ActualSeriesPadding);
		}
		double CalcSeriesDistance() {
			return GetValueByAxisX(diagram.ActualSeriesDistance);
		}
		double CalcSeriesOffset(Series series) {
			int index = GetSeriesGroupIndex(series);
			if(index < 0)
				throw new ArgumentException("series");
			return CalcSeriesPadding() + CalcSeriesDistance() * index;
		}
		public double GetValueByAxisX(double value) {
			return Math.Abs(axisXMapping.GetAxisValue(0) - axisXMapping.GetAxisValue(value));
		}
		public Point GetDiagramPoint(double argument, double value) {
			return new Point(axisXMapping.GetAxisValue(argument), axisYMapping.GetAxisValue(value));
		}
		public Point GetClippedDiagramPoint(double argument, double value) {
			return new Point(axisXMapping.GetClampedAxisValue(argument), axisYMapping.GetClampedAxisValue(value));
		}
		public Point3D GetDiagramPoint(Series series, double argument, double value) {
			Point point = GetDiagramPoint(argument, value);
			double seriesOffset = CalcSeriesOffset(series);
			return new Point3D(point.X, point.Y, seriesOffset);
		}
		public override Transform3D CreateModelTransform() {
			return new TranslateTransform3D(-innerWidth * 0.5, -innerHeight * 0.5, -innerDepth * 0.5);
		}
	}
}
