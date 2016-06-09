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

using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts.Native {
	public abstract class Series3DData : SeriesData {
		public Series3DData(Series series) : base(series) {
		}
		protected override PointGeometry CreateSeriesPointGeometry(Diagram3DDomain domain, RefinedPoint refinedPoint) {
			Diagram3DDomain domain3D = domain as Diagram3DDomain;
			if (domain3D == null)
				return null;
			ContentPresenter presenter = CreateSeriesPointLabelContentPresenter(refinedPoint);
			return new Series3DPointGeometry(Series.ActualLabel, presenter, Label3DHelper.CreateModel(presenter, Series.ActualLabel), CreateSeriesPointModelInfo(refinedPoint, domain3D));
		}
		protected abstract Model3DInfo CreateSeriesPointModelInfo(RefinedPoint refinedPoint, Diagram3DDomain domain);
		public void CreateLabelsModel(Diagram3DDomain domain, Model3DGroup modelHolder, List<LabelModelContainer> transparentLabelContainers, IRefinedSeries refinedSeries) {
			foreach (RefinedPoint pointInfo in refinedSeries.Points)
				if (!pointInfo.IsEmpty) {
					Series3DPointGeometry pointGeometry = GetPointGeometry(pointInfo, domain) as Series3DPointGeometry;
					if (pointGeometry != null && pointGeometry.ModelInfo != null) {
						Model3DInfo modelInfo = pointGeometry.ModelInfo as Model3DInfo;
						if (modelInfo != null) {
							SeriesLabel3DLayout labelLayout = modelInfo.LabelLayout;
							if (labelLayout != null && pointGeometry.IsLabelPresent)
								transparentLabelContainers.Add(labelLayout.CreateModel(domain, pointGeometry, Series.GetPointLabelColor(pointInfo), modelHolder));
						}
					}
				}
		}
		public virtual void CreateModel(Diagram3DDomain domain, Model3DGroup modelHolder, IRefinedSeries refinedSeries) {
			foreach (RefinedPoint pointInfo in refinedSeries.Points)
				if (!pointInfo.IsEmpty) {
					Series3DPointGeometry pointGeometry = GetPointGeometry(pointInfo, domain) as Series3DPointGeometry;
					if (pointGeometry != null) {
						Model3DInfo modelInfo = pointGeometry.ModelInfo;
						if (modelInfo != null) {
							pointGeometry.ModelInfo.UpdateTransform(null);
							Model3D model = modelInfo.Model;
							if (model != null) {
								ChartControl.SetSeriesPointHitTestInfo(model, pointInfo);
								modelHolder.Children.Add(model);
								domain.OnModelAdd(pointInfo, model);
							}
						}
					}
				}
		}
	}
}
