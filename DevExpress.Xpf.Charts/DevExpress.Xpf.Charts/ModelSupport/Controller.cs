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

using System.Windows;
using System.Windows.Controls;
using Model = DevExpress.Charts.Model;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Xpf.Charts.ModelSupport {
	public class XpfChartsModelControllerFactory : Model.ModelControllerFactoryBase{
		public override Model.Controller CreateController() {
			return new ModelController();
		}
		public override Model.IChartRenderContext CreateRenderContext(Model.ModelRect bounds, params object[] renderParams) {
			if (renderParams.Length > 0) {
				ContentPresenter container = renderParams[0] as ContentPresenter;
				if (container != null)
					return new ChartRenderContext(container, bounds);
			}
			return null;
		}
	}
	public class ChartRenderContext : Model.IChartRenderContext {
		ContentPresenter container;
		Model.ModelRect bounds;
		public Model.ModelRect Bounds { get { return bounds; } }
		public ContentPresenter Container { get { return container; } }
		public ChartRenderContext(ContentPresenter container, Model.ModelRect bounds) {
			this.container = container;
			this.bounds = bounds;
		}
	}
	public class ModelController : Model.Controller {
		readonly ChartControl chart;
		ChartConfigurator configurator;
		IInteractiveElement selectedElement;
		public ModelController() {
			this.chart = new ChartControl();
		}
		object GetHitObject(ChartHitInfo hitInfo) {
			if (hitInfo.InSeries)
				return hitInfo.Series;
			if (hitInfo.InSeriesLabel)
				return hitInfo.SeriesLabel.Series;
			if (hitInfo.InAxis)
				return hitInfo.Axis;
			if (hitInfo.InAxisLabel)
				return hitInfo.AxisLabel.Axis;
			if (hitInfo.InAxisTitle)
				return hitInfo.AxisTitle.Axis;
			if (hitInfo.InDiagram || hitInfo.InConstantLine || hitInfo.InIndicator || hitInfo.InStrip || hitInfo.InPane)
				return chart;
			if (hitInfo.InLegend)
				return hitInfo.Legend;
			if (hitInfo.InTitle)
				return hitInfo.Title;
			return null;
		}
		void ApplyModel() {
			if (configurator != null)
				configurator.Configure(chart);
		}
		protected override void OnModelChanged() {
			configurator = ChartConfigurator.CreateChartConfigurator(ChartModel, ElementContainer);
			ApplyModel();
		}
		protected override void OnModelUpdated(Model.UpdateInfo update) {
			base.OnModelUpdated(update);
			if (!update.Handled && configurator != null)
				configurator.Update(chart, update);
			if (!update.Handled && configurator != null)
				configurator.Configure(chart);
		}
		public override void RenderChart(Model.IChartRenderContext renderContext) {
			ChartRenderContext context = renderContext as ChartRenderContext;
			if(context != null && context.Container != null){
				chart.Width = context.Bounds.Width;
				chart.Height = context.Bounds.Height;
				context.Container.Content = chart;
			}
		}
		public override Model.HitInfo CalcHitInfo(double x, double y) {
			ChartHitInfo hitInfo = chart.CalcHitInfo(new Point(x, y));
			object hitObject = GetHitObject(hitInfo);
			return CreateHitInfo(hitObject);
		}
		public override void Select(Model.ModelElement element) {
			if (selectedElement != null && selectedElement != element)
				selectedElement.IsSelected = false;
			selectedElement = ElementContainer.FindViewObject(element) as IInteractiveElement;
			if (selectedElement != null)
				selectedElement.IsSelected = true;
		}
		public override void Unselect(Model.ModelElement element) {
			selectedElement = ElementContainer.FindViewObject(element) as IInteractiveElement;
			if (selectedElement != null)
				selectedElement.IsSelected = false;
		}
		public override void ClearSelection() {
			if (selectedElement != null)
				selectedElement.IsSelected = false;
		}
	}
}
