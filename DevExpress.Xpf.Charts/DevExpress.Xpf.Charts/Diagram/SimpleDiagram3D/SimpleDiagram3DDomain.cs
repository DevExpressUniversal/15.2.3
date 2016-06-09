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
using System.Windows.Media;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public class SimpleDiagram3DDomain : Diagram3DDomain {
		PieSeries3DData SeriesData { get { return SeriesDataList.Count > 0 ? SeriesDataList[0] as PieSeries3DData : null; } }
		public bool IsTopVisible { get { return RotationOnlyTransform.Transform(new Vector3D(0, 1, 0)).Z > 0; } }
		public SimpleDiagram3DDomain(SimpleDiagram3D diagram, Rect viewport, PieSeries3D series) : base(diagram, viewport, new SeriesData[] { series.CreateSeriesData() }, new SimpleDiagram3DBox(series)) {
		}
		public override void ValidateSeriesPointsCache() {
			foreach (SeriesData seriesData in SeriesDataList) {
				IRefinedSeries refinedSeries = Diagram.ViewController.GetRefinedSeries(seriesData.Series);
				seriesData.ValidateSeriesPointsCache(this, refinedSeries);
			}
		}
		protected override void Render3DContent(Model3DGroup modelHolder) {
			if(SeriesData != null) {
				IRefinedSeries refinedSeries = Diagram.ViewController.GetRefinedSeries(SeriesData.Series);
				if (refinedSeries != null)
					SeriesData.Create3DContent(this, refinedSeries, modelHolder);
			}
		}
		protected override void Render2DContent(DrawingVisual visualHolder) {
			if(SeriesData != null) {
				IRefinedSeries refinedSeries = Diagram.ViewController.GetRefinedSeries(SeriesData.Series);
				if (refinedSeries != null) {
					DrawingVisual visual = new DrawingVisual();
					using(DrawingContext context = visual.RenderOpen()) {
						SeriesData.CreateLabelsVisual(this, context, refinedSeries);
					}
					visualHolder.Children.Add(visual);
				}
			}
		}
	}
	public class SimpleDiagram3DBox : Diagram3DBox {
		const int magicRadius = 1000;
		readonly PieSeries3D series;
		public SimpleDiagram3DBox(PieSeries3D series) {
			this.series = series;
			double depthToWidth = CalcDepthToWidthRatio();
			Width = Math.Sqrt(4 * magicRadius * magicRadius / (1 + depthToWidth * depthToWidth));
			Height = Width;
			Depth = depthToWidth * Width;
		}
		double CalcDepthToWidthRatio() {
			if(series == null || series.ActualModel == null)
				return 0;
			Size size = series.ActualModel.GetSize();
			if(size.IsEmpty || size.Width == 0)
				return 0;
			double modelHeightToWidthRatio = series.DepthTransform * size.Height / size.Width;
			double modelWidthRatio = 1 - Pie3DCalculator.NormalizeHoleRadiusRatio(series.HoleRadiusPercent / 100.0);
			return modelWidthRatio * modelHeightToWidthRatio / 2;
		}
		public override double CalcViewRadius() {
			return magicRadius;
		}
		public override Transform3D CreateModelTransform() {
			return new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(1, 0, 0), 90));
		}
	}
}
