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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
using DevExpress.Utils;
namespace DevExpress.Xpf.Charts.Native {
	public class PieSeries3DData : SeriesData {
		Pie pie;
		PieSeries3D PieSeries { get { return (PieSeries3D)base.Series; } }
		public PieSeries3DData(PieSeries3D series) : base(series) {
			pie = new Pie(series.Diagram.ViewController.GetRefinedSeries(series), PieSweepDirection.Counterclockwise);
		}
		protected override PointGeometry CreateSeriesPointGeometry(Diagram3DDomain domain, RefinedPoint refinedPoint) {
			SimpleDiagram3DDomain domain3D = domain as SimpleDiagram3DDomain;
			if (domain3D == null)
				return null;
			ContentPresenter presenter = CreateSeriesPointLabelContentPresenter(refinedPoint);
			return new Series3DPointGeometry(Series.ActualLabel, presenter, Label3DHelper.CreateModel(presenter, Series.ActualLabel), CreateSeriesPointModelInfo(refinedPoint, domain3D));
		}
		Model3DInfo CreateSeriesPointModelInfo(RefinedPoint refinedPoint, SimpleDiagram3DDomain domain) {
			Slice slice = pie[refinedPoint.SeriesPoint];
			if (slice == null || slice.IsEmpty)
				return null;
			double radius = domain.Width / 2;
			Pie3DLabelLayout labelLayout;
			SeriesPoint point = SeriesPoint.GetSeriesPoint(refinedPoint.SeriesPoint);
			if (point == null)
				return null;
			Model3D model = Pie3DCalculator.CalculateSlice(domain, Series.ActualLabel, PieSeries.ActualModel, radius,
				PieSeries.HoleRadiusPercent / 100.0, PieSeries.DepthTransform, slice.StartAngle, slice.FinishAngle,
				true, true, 200, Series.MixColor(Series.GetSeriesPointColor(refinedPoint)), out labelLayout);
			double angle = slice.StartAngle + (slice.FinishAngle - slice.StartAngle) / 2.0;
			return new PieModel3DInfo(point, model, angle, radius, labelLayout);
		}
		public void Create3DContent(SimpleDiagram3DDomain domain, IRefinedSeries refinedSeries, Model3DGroup modelHolder) {
			CreateModel(modelHolder, GetAdditionalTransform(domain), refinedSeries, domain);
			List<LabelModelContainer> labelContainers = new List<LabelModelContainer>();
			CreateLabelsModel(domain, modelHolder, labelContainers, refinedSeries);
			labelContainers.Sort(LabelModelContainer.CompareByDistance);
			foreach (LabelModelContainer modelContainer in labelContainers)
				modelHolder.Children.Add(modelContainer.Model);
		}
		public void CreateLabelsVisual(Diagram3DDomain domain, DrawingContext context, IRefinedSeries refinedSeries) {
			ContentPresenter contentPresenter = null;
			Transform3D additionalTransform = GetAdditionalTransform(domain);
			List<IPieLabelLayout> labelLayoutList = new List<IPieLabelLayout>();
			foreach (RefinedPoint refinedPoint in refinedSeries.Points)
				if (!refinedPoint.IsEmpty) {
					Series3DPointGeometry pointGeometry = GetPointGeometry(refinedPoint, domain) as Series3DPointGeometry;
					if (pointGeometry != null && pointGeometry.ModelInfo != null) {
						Model3DInfo modelInfo = pointGeometry.ModelInfo;
						contentPresenter = pointGeometry.LabelContentPresenter;
						if (modelInfo != null && contentPresenter != null && pointGeometry.IsLabelPresent) {
							Pie3DLabelLayout labelLayout = modelInfo.LabelLayout as Pie3DLabelLayout;
							if (labelLayout != null){
								labelLayout.ContentPresenter = contentPresenter;
								CalculateLabelLayout(labelLayout, domain, additionalTransform, contentPresenter);
								labelLayout.LabelConectorColor = Series.GetPointLabelColor(refinedPoint);
								labelLayout.CompleteLayout(domain as SimpleDiagram3DDomain);
								labelLayoutList.Add(labelLayout);
							}							
						}
					}
				}
			if (labelLayoutList.Count != 0) {
				if (((Pie3DLabelLayout)labelLayoutList[0]).Position == PieLabelPosition.TwoColumns && PieSeries.LabelsResolveOverlapping == true)
					SimpleDiagrammResiolveOverlapping.ArrangeByColumn(labelLayoutList, new GRealRect2D(0, 0, domain.Viewport.Width, domain.Viewport.Height), PieSeries.LabelsResolveOverlappingMinIndent);
			}
			RenderLabels(labelLayoutList, context);
			RenderLabelConnectors(labelLayoutList, context, domain);
		}
		protected internal override Point3D CalculateToolTipPoint(SeriesPointCache pointCache, Diagram3DDomain domain) {
			Point3D toolTipPoint = new Point3D();
			SeriesPoint seriesPoint = SeriesPoint.GetSeriesPoint(((ISeriesPointData)pointCache).SeriesPoint);
			Series3DPointGeometry geometry = pointCache.PointGeometry as Series3DPointGeometry;
			if (seriesPoint != null && geometry != null) {
				Slice slice = pie[seriesPoint];
				double startAngle = slice.StartAngle;
				double finishAngle = slice.FinishAngle;
				GeometryModel3D sectionModel = Pie3DCalculator.CreateSectionModel(PieSeries.ActualModel);
				if (sectionModel != null && !slice.IsEmpty) {
					MeshGeometry3D section = (MeshGeometry3D)sectionModel.Geometry;
					double radius = domain.Width / 2;
					double heightRatio = PieSeries.DepthTransform;
					double holeRadiusRatio = PieSeries.HoleRadiusPercent / 100.0;
					Pie3DCalculator.Prepare(section, heightRatio, radius, holeRadiusRatio);
					toolTipPoint = ((SimpleDiagram3DDomain)domain).IsTopVisible ? Pie3DCalculator.GetMaxTop(section, startAngle, finishAngle) : Pie3DCalculator.GetMaxBottom(section, startAngle, finishAngle);
					Transform3D transform = geometry.ModelInfo.Model.Transform;
					toolTipPoint = transform.Transform(toolTipPoint);
				}
			}
			return toolTipPoint;
		}
		Size GetMaxSize(Pie3DLabelLayout labelLayout, ContentPresenter labelPresenter) {
			if (labelLayout.Position == PieLabelPosition.Outside || labelLayout.Position == PieLabelPosition.TwoColumns)
				return labelPresenter.DesiredSize;
			else
				return new Size();
		}
		Transform3D GetAdditionalTransform(Diagram3DDomain domain) {
			double d = ((PieSeries)Series).GetMaxExplodedDistance();
			double k1 = 1 / (1 + d);
			double k2 = domain.Width > 0 ? 1 - 2 * GetMaxLabelSize(domain) / domain.Width : 0;
			k2 = k2 > 0 ? k2 : 0.001;
			double k = k1 * k2;
			return k == 1 ? null : new ScaleTransform3D(k, k, k);
		}
		void CreateModel(Model3DGroup modelHolder, Transform3D commonTransform, IRefinedSeries refinedSeries, Diagram3DDomain domain) {
			foreach (RefinedPoint refinedPoint in refinedSeries.Points)
				if (!refinedPoint.IsEmpty) {
					Series3DPointGeometry pointGeometry = GetPointGeometry(refinedPoint, domain) as Series3DPointGeometry;
					if (pointGeometry != null && pointGeometry.ModelInfo != null) {
						pointGeometry.ModelInfo.UpdateTransform(commonTransform);
						Model3D model = pointGeometry.ModelInfo.Model;
						if (model != null) {
							ChartControl.SetSeriesPointHitTestInfo(model, refinedPoint);
							modelHolder.Children.Add(model);
						}
					}
				}
		}
		void CreateLabelsModel(Diagram3DDomain domain, Model3DGroup modelHolder, List<LabelModelContainer> transparentLabelContainers, IRefinedSeries refinedSeries) {
			List<IPieLabelLayout> labelLayoutList = new List<IPieLabelLayout>();
			foreach (RefinedPoint refinedPoint in refinedSeries.Points)
				if (!refinedPoint.IsEmpty) {
					Series3DPointGeometry pointGeometry = GetPointGeometry(refinedPoint, domain) as Series3DPointGeometry;
					if (pointGeometry != null) {
						Model3DInfo modelInfo = pointGeometry.ModelInfo;
						if (modelInfo != null && modelInfo.LabelLayout != null && pointGeometry.IsLabelPresent) {
							labelLayoutList.Add(modelInfo.LabelLayout as IPieLabelLayout);
							LabelModelContainer labelContainer = modelInfo.LabelLayout.CreateModel(domain, pointGeometry,Series.GetPointLabelColor(refinedPoint), modelHolder);
							if (labelContainer != null)
								transparentLabelContainers.Add(labelContainer);
						}
					}
				}
		}
		void RenderLabels(List<IPieLabelLayout> labelLayoutList, DrawingContext context) {
			foreach (IPieLabelLayout labelLayout in labelLayoutList) {
				Pie3DLabelLayout pieLabelLayout = labelLayout as Pie3DLabelLayout;
				context.DrawRectangle(RenderHelper.CreateBrush(pieLabelLayout.ContentPresenter, pieLabelLayout.Label), null, LabelBoundsAsRect(labelLayout.LabelBounds));
			}
		}
		void RenderLabelConnectors(List<IPieLabelLayout> labelLayoutList, DrawingContext context, Diagram3DDomain domain) {
			foreach (IPieLabelLayout labelLayout in labelLayoutList) {
				Pie3DLabelLayout pieLabelLayout = labelLayout as Pie3DLabelLayout;
				RenderLabelConnector(pieLabelLayout, context, domain);
			}
		}
		void RenderLabelConnector(Pie3DLabelLayout labelLayout, DrawingContext context, Diagram3DDomain domain) {
			if (labelLayout.Position != PieLabelPosition.TwoColumns || !labelLayout.Label.ConnectorVisible)
				return;
			SimpleDiagram3DDomain simpleDiagramDomain = domain as SimpleDiagram3DDomain;
			Point dockPoint = labelLayout.GetLabelPoint2D(simpleDiagramDomain);
			GRealPoint2D dockGRealPoint = new GRealPoint2D(dockPoint.X, dockPoint.Y);
			GRealPoint2D? salientGRealPoint = ResolveOverlappingHelper.CalculateSalientPoint(dockGRealPoint, labelLayout.LabelBounds, labelLayout.Angle);
			Point salientPoint = salientGRealPoint == null ? dockPoint : new Point(salientGRealPoint.Value.X, salientGRealPoint.Value.Y);
			Point connectPoint = labelLayout.CalculateLabelConnectPoint(simpleDiagramDomain);
			context.DrawLine(new Pen(new SolidColorBrush(labelLayout.LabelConectorColor), labelLayout.Label.ConnectorThickness), dockPoint, salientPoint);
			context.DrawLine(new Pen(new SolidColorBrush(labelLayout.LabelConectorColor), labelLayout.Label.ConnectorThickness), salientPoint, connectPoint);
		}
		Rect LabelBoundsAsRect (GRect2D gRect){
			return new Rect(gRect.Left, gRect.Top, gRect.Width, gRect.Height);
		}
		void CalculateLabelLayout(Pie3DLabelLayout labelLayout, Diagram3DDomain domain, Transform3D additionalTransform, ContentPresenter labelPresenter) {
			SimpleDiagram3DDomain simpleDomain = domain as SimpleDiagram3DDomain;
			switch (labelLayout.Position) {
				case PieLabelPosition.Inside:
					CalculateLabelsInsidePosition(labelLayout, simpleDomain, labelPresenter);
					break;
				case PieLabelPosition.Outside:
					break;
				case PieLabelPosition.TwoColumns:
					CalculateLabelsTwoColumnsPosition(labelLayout, simpleDomain, additionalTransform, labelPresenter);
					break;
			}
		}
		void CalculateLabelsInsidePosition(Pie3DLabelLayout labelLayout, SimpleDiagram3DDomain domain, ContentPresenter presenter) {
			Point3D point = labelLayout.GetDockPoint(domain);
			Point location = domain.Project(point);
			location.Offset(-presenter.DesiredSize.Width / 2, -presenter.DesiredSize.Height / 2);
			labelLayout.LabelBounds = ConvertRectToGrect2D(new Rect(location, presenter.DesiredSize));
		}
		GRect2D ConvertRectToGrect2D(Rect rect) {
			return new GRect2D(MathUtils.StrongRound(rect.Left), MathUtils.StrongRound(rect.Top), MathUtils.StrongRound(rect.Width), MathUtils.StrongRound(rect.Height));
		}
		void CalculateLabelsTwoColumnsPosition(Pie3DLabelLayout labelLayout, SimpleDiagram3DDomain domain, Transform3D additionalTransform, ContentPresenter presenter) {
			Point dockPoint = labelLayout.GetLabelPoint2D(domain);
			Vector direction2D = labelLayout.GetDirection2D(domain);
			Point3D outermostRightPoint3D = domain.BackwardRotationOnlyTransform.Transform(additionalTransform.Transform(new Point3D(domain.ViewRadius, 0, 0)));
			Point outermostRightPoint2D = domain.Project(outermostRightPoint3D);
			Point3D outermostLeftPoint3D = domain.BackwardRotationOnlyTransform.Transform(additionalTransform.Transform(new Point3D(-domain.ViewRadius, 0, 0)));
			Point outermostLeftPoint2D = domain.Project(outermostLeftPoint3D);
			outermostLeftPoint2D.X -= labelLayout.LabelIndent;
			outermostRightPoint2D.X += labelLayout.LabelIndent;
			Point SalientPoint = Point.Add(dockPoint, direction2D * labelLayout.LabelIndent);
			Point labelConnectPoint = new Point();
			labelConnectPoint.Y = SalientPoint.Y;
			if (direction2D.X < 0)
				labelConnectPoint.X = CalculateConnectorPointXCoordInLeftColumn(outermostLeftPoint2D.X, presenter.DesiredSize.Width);
			else
				labelConnectPoint.X = CalculateConnectorPointXCoordInRightColumn(outermostRightPoint2D.X, domain.Viewport.Width, presenter.DesiredSize.Width);
			Point labelLocation = new Point(labelConnectPoint.X, labelConnectPoint.Y - 0.5 * presenter.DesiredSize.Height);
			labelLocation.X = direction2D.X < 0 ? labelLocation.X - presenter.DesiredSize.Width : labelLocation.X;
			labelLayout.LabelBounds = ConvertRectToGrect2D(new Rect(labelLocation, presenter.DesiredSize));
		}
		double CalculateConnectorPointXCoordInLeftColumn(double outermostLeftPointXCoord, double labelWidth) {
			const double leftBoundXCoord = 0;
			return leftBoundXCoord + labelWidth < outermostLeftPointXCoord ? leftBoundXCoord + labelWidth : outermostLeftPointXCoord;
		}
		double CalculateConnectorPointXCoordInRightColumn(double outermostRightPointXCoord, double rightBoundXCoord, double labelWidth) {
			return rightBoundXCoord - labelWidth > outermostRightPointXCoord ? rightBoundXCoord - labelWidth : outermostRightPointXCoord;
		}
		double GetMaxLabelSize(Diagram3DDomain domain) {
			Size maxSize = new Size();
			Transform3D scale = domain.CreateScaleTransform(new Point3D());
			foreach (SeriesPointCache seriesPointCache in Series.Cache) {
				Series3DPointGeometry pointGeometry = seriesPointCache.PointGeometry as Series3DPointGeometry;
				if (pointGeometry != null) {
					ContentPresenter labelPresenter = pointGeometry.LabelContentPresenter;
					Model3DInfo modelInfo = pointGeometry.ModelInfo;
					if (labelPresenter != null && modelInfo != null) {
						Pie3DLabelLayout labelLayout = modelInfo.LabelLayout as Pie3DLabelLayout;
						if (labelLayout != null) {
							Size size = GetMaxSize(labelLayout, labelPresenter);
							maxSize.Width = Math.Max(maxSize.Width, size.Width);
							maxSize.Height = Math.Max(maxSize.Height, size.Height);
						}
					}
				}
			}			
			if (PieSeries3D.GetLabelPosition(Series.ActualLabel) == PieLabelPosition.TwoColumns) {
				maxSize.Height = Series.ActualLabel.Indent + maxSize.Height * 0.5;
				Point3D outermostPiePoint3D = domain.BackwardRotationOnlyTransform.Transform(new Point3D(domain.ViewRadius, 0, 0));
				Point outermostPiePoint2D = domain.ProjectWithoutZoomAndScroll(outermostPiePoint3D);
				double outermostXCoord = outermostPiePoint2D.X + maxSize.Width + Series.ActualLabel.Indent;
				double dist = outermostXCoord - domain.Viewport.Width;
				maxSize.Width = dist > 0 ? dist : 0;
			}
			else {
				maxSize.Height += Series.ActualLabel.Indent;
				maxSize.Width += Series.ActualLabel.Indent;
			}
			Rect3D rect = new Rect3D(new Point3D(), new Size3D(maxSize.Width, maxSize.Height, 0));
			rect = scale.TransformBounds(rect);
			return Math.Max(rect.SizeX, rect.SizeY);
		}
	}
}
