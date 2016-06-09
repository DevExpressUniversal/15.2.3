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
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using DevExpress.Charts.Native;
using DevExpress.Utils.Commands;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraPrinting.Native.WebClientUIControl;
namespace DevExpress.XtraCharts.Web.Designer.Native {
	public class PreviewChartContainer : IChartContainer, IDisposable {
		public static PreviewChartContainer CreateFromContract(ChartLayout contract) {
			var chartContainer = new PreviewChartContainer();
			chartContainer.Width = (int)contract.Width;
			chartContainer.Height = (int)contract.Height;
			var xmlLayout = JsonConverter.JsonToXml(contract.Chart);
			using (var stream = new MemoryStream(xmlLayout))
				chartContainer.LoadFromStream(stream);
			return chartContainer;
		}
		Chart chart;
		ISeriesView baseView;
		ISeries baseSeries;
		public int Width { get; set; }
		public int Height { get; set; }
		PreviewChartContainer() {
			this.chart = new Chart(this);
		}
		~PreviewChartContainer() {
			Dispose(false);
		}
		#region IChartContainer
		event EventHandler IChartContainer.EndLoading {
			add { }
			remove { }
		}
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose {
			add { throw new NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI {
			add { throw new System.NotImplementedException(); }
			remove { throw new NotImplementedException(); }
		}
		Chart IChartContainer.Chart {
			get { return this.chart; }
		}
		ChartContainerType IChartContainer.ControlType {
			get { return ChartContainerType.WinControl; }
		}
		IChartDataProvider IChartContainer.DataProvider {
			get { return null; }
		}
		bool IChartContainer.DesignMode {
			get { return true; }
		}
		IChartEventsProvider IChartContainer.EventsProvider {
			get { return null; }
		}
		IChartInteractionProvider IChartContainer.InteractionProvider {
			get { return null; }
		}
		bool IChartContainer.IsDesignControl {
			get { throw new NotImplementedException(); }
		}
		bool IChartContainer.IsEndUserDesigner {
			get { throw new NotImplementedException(); }
		}
		bool IChartContainer.Loading {
			get { return false; }
		}
		IComponent IChartContainer.Parent {
			get { throw new NotImplementedException(); }
		}
		IChartRenderProvider IChartContainer.RenderProvider {
			get { return null; }
		}
		IServiceProvider IChartContainer.ServiceProvider {
			get { throw new NotImplementedException(); }
		}
		bool IChartContainer.ShouldEnableFormsSkins {
			get { throw new NotImplementedException(); }
		}
		ISite IChartContainer.Site {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		bool IChartContainer.ShowDesignerHints {
			get { throw new NotImplementedException(); }
		}
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler {
			get { throw new NotImplementedException(); }
		}
		void IChartContainer.Assign(Chart chart) {
			throw new NotImplementedException();
		}
		void IChartContainer.Changed() { }
		void IChartContainer.Changing() { }
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		bool IChartContainer.CanDisposeItems { get { return true; } }
		void IChartContainer.LockChangeService() {
			throw new NotImplementedException();
		}
		void IChartContainer.RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) { }
		void IChartContainer.ShowErrorMessage(string message, string title) {
			throw new NotImplementedException();
		}
		void IChartContainer.UnlockChangeService() {
			throw new NotImplementedException();
		}
		void ISupportBarsInteraction.RaiseUIUpdated() { }
		object IServiceProvider.GetService(Type serviceType) {
			throw new NotImplementedException();
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
			throw new NotImplementedException();
		}
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			throw new NotImplementedException();
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
			throw new NotImplementedException();
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			throw new NotImplementedException();
		}
		#endregion
		void LoadFromStream(Stream stream) {
			stream.Seek(0L, SeekOrigin.Begin);
			if (!XtraSerializingHelper.IsValidXml(stream))
				throw new LayoutStreamException();
			stream.Seek(0L, SeekOrigin.Begin);
			chart.LoadLayout(stream);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (chart != null) {
					chart.Dispose();
					chart = null;
				}
			}
		}
		public int[] GetIndexesOfIncompatibleViews() {
			baseSeries = null;
			baseView = null;
			DevExpress.XtraCharts.Native.Chart chart = ((IChartContainer)this).Chart;
			var indexes = new List<int>();
			var dataContainer = (IChartDataContainer)chart.DataContainer;
			var seriesRepository = new RefinedSeriesRepository(dataContainer);
			var incompatibilityCalculator = new RefinedSeriesIncompatibilityCalculator(new SeriesIncompatibilityStatistics());
			if (dataContainer.ShouldUseSeriesTemplate) {
				baseView = dataContainer.SeriesTemplate.SeriesView;
				IXYSeriesView xySeriesView = dataContainer.SeriesTemplate.SeriesView as IXYSeriesView;
				if (xySeriesView != null)
					incompatibilityCalculator.AddTemplateView(xySeriesView, dataContainer.SeriesTemplate.ArgumentScaleType, dataContainer.SeriesTemplate.ValueScaleType);
			}
			else {
				baseSeries = chart.Series.FirstOrDefault();
				if (baseSeries != null)
					baseView = baseSeries.SeriesView;
			}
			incompatibilityCalculator.Initialize(baseSeries, baseView);
			int i = 0;
			foreach (Series series in chart.Series) {
				if (!series.IsAutoCreated) {
					var refSeries = new RefinedSeries(series, seriesRepository);
					if (!incompatibilityCalculator.IsCompatible(refSeries)) {
						indexes.Add(i);
					}
					i++;
				}
			}
			return indexes.ToArray();
		}
		public Bitmap CreateBitmap() {
			var size = new Size(Width, Height);
			return this.chart.CreateBitmap(size);
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
