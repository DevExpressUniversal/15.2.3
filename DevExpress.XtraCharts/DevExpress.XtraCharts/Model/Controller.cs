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
using System.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.Utils;
using Model = DevExpress.Charts.Model;
using DevExpress.XtraCharts.ModelSupport.Implementation;
namespace DevExpress.XtraCharts.ModelSupport {
	public class XtraChartsModelControllerFactory : Model.ModelControllerFactoryBase {
		public override Model.Controller CreateController() {
			return new ModelController();
		}
		public override Model.IChartRenderContext CreateRenderContext(Model.ModelRect bounds, params object[] renderParams) {
			try {
				Graphics gr = (Graphics)renderParams[0];
				Model.ModelRect windowBounds = (renderParams.Length > 1) ?  (Model.ModelRect )renderParams[1] : bounds;
				return new ChartRenderContext(gr, bounds, windowBounds);
			}
			catch {
				return null;
			}
		}
	}
	public class ChartRenderContext : Model.IChartRenderContext {
		Graphics graphics;
		Model.ModelRect bounds;
		Model.ModelRect windowBounds;
		public Graphics Graphics { get { return graphics; } }
		public Model.ModelRect Bounds { get { return bounds; } }
		public Model.ModelRect WindowBounds { get { return windowBounds; } }
		public ChartRenderContext(Graphics gr, Model.ModelRect bounds, Model.ModelRect windowBounds) {
			this.graphics = gr;
			this.bounds = bounds;
			this.windowBounds = windowBounds;
		}
	}
	public class ModelController : Model.Controller, IDisposable {
		readonly Chart chart;
		readonly ChartConfigurator configurator;
		protected ChartConfigurator Configurator { get { return configurator; } }
		protected Chart Chart { get { return chart; } }
		public ModelController() {
			ModelChartContainer container = new ModelChartContainer();
			this.chart = new Chart(container);
			container.SetChart(chart);
			this.configurator = new ChartConfigurator(chart);
		}
		#region IDisposable implementation
		void Dispose(bool disposing) {
			if(disposing) {
				ChartModel = null;
				if(Chart != null)
					Chart.Dispose();
			}
		}
		~ModelController() {
			Dispose(false);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
		protected override void OnModelChanged() {
			ApplyModel();
		}
		public override void ClearSelection() {
			chart.ClearSelection();
		}
		public override void Unselect(Model.ModelElement element) {
			UpdateSelection(element, false);
		}
		public override void Select(Model.ModelElement element) {
			UpdateSelection(element, true);
		}
		protected void UpdateSelection(Model.ModelElement element, bool selected) {
			if(element == null) return;
			object viewEl = ElementContainer.FindViewObject(element);
			if(viewEl != null) chart.SetObjectSelection(viewEl, selected);
		}
		protected override void OnModelUpdated(Model.UpdateInfo update) {
			base.OnModelUpdated(update);
			if (!update.Handled)
				Configurator.Update(update);
			if (!update.Handled)
				Configurator.Configure(ChartModel, ElementContainer);
		}
		protected void ApplyModel() {
			Configurator.Configure(ChartModel, ElementContainer);
		}
		public override Model.HitInfo CalcHitInfo(double x, double y) {
			if (ChartModel is Model.IChart3D)
				return Model.HitInfo.Empty;
			ChartHitInfo chartHitInfo = Chart.CalcHitInfo(Convert.ToInt32(x), Convert.ToInt32(y));
			if (chartHitInfo.HitTest == ChartHitTest.None)
				return Model.HitInfo.Empty;
			object hitObject = chartHitInfo.HitObject;
			if(chartHitInfo.HitTest == ChartHitTest.Chart || chartHitInfo.HitTest == ChartHitTest.Diagram)
				hitObject = chartHitInfo.Chart.Chart;
			return CreateHitInfo(hitObject);
		}
		public override void RenderChart(Model.IChartRenderContext renderContext) {
			ChartRenderContext context = renderContext as ChartRenderContext;
			if (context != null) {
				Model.ModelRect rect = context.Bounds;
				Rectangle bounds = new Rectangle((int)rect.Left, (int)rect.Top, (int)rect.Width, (int)rect.Height);
				Model.ModelRect windowsRect = context.WindowBounds;
				Rectangle windowsBounds = new Rectangle((int)windowsRect.Left, (int)windowsRect.Top, (int)windowsRect.Width, (int)windowsRect.Height);
				bool enableAntialiasing = false;
				if (ChartModel is Model.IChart3D)
					enableAntialiasing = ((Model.IChart3D)ChartModel).Options3D.EnableAntialiasing;
				Chart.DrawContent(context.Graphics, bounds, windowsBounds, enableAntialiasing);
			}
		}
	}
}
