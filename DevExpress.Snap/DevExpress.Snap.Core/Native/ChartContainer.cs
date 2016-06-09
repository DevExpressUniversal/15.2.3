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

using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Native.Services;
using DevExpress.Utils.Commands;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Utils.KeyboardHandler;
using DevExpress.XtraCharts;
using DevExpress.XtraCharts.Commands;
using DevExpress.XtraCharts.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraPrinting;
namespace DevExpress.Snap.Core.Native {
	public class ChartContainer : ISNChartContainer {
		readonly IServiceProvider serviceProvider;
		SNChart chart;
		readonly DataContext dataContext;
		public ChartContainer(IServiceProvider serviceProvider, DataContext dataContext) {
			this.serviceProvider = serviceProvider;
			this.dataContext = dataContext;
		}
		#region ISupportBarsInteraction implementation
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { } remove { } }
		EventHandler updateUI;
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI { add { updateUI += value; } remove { updateUI -= value; } }
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return null;
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		object IServiceProvider.GetService(Type serviceType) {
			IServiceProvider provider = ((IChartContainer)this).ServiceProvider;
			return provider != null ? provider.GetService(serviceType) : null;
		}
		public void RaiseUIUpdated() {
			updateUI.SafeRaise(this, EventArgs.Empty);
		}
		#endregion
		#region IChartDataProvider implementation
		public event BoundDataChangedEventHandler BoundDataChanged {
			add { }
			remove { }
		}
		public object DataAdapter {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		public DataContext DataContext {
			get {
				return dataContext;
			}
		}
		public object DataSource {
			get { return Chart.DataContainer.DataSource; }
			set { Chart.DataContainer.DataSource = value; }
		}
		public bool SeriesDataSourceVisible { get { return true; } }
		public object ParentDataSource { get { return null; } }
		public bool CanUseBoundPoints { get { return true; } }
		public bool ShouldSerializeDataSource(object dataSource) {
			throw new NotImplementedException();
		}
		public void OnBoundDataChanged(EventArgs e) { }
		public void OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) { }
		public void OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) { }
		#endregion
		#region IChartRenderProvider implementation
		bool IChartRenderProvider.IsPrintingAvailable { get { return false; } }
		DevExpress.XtraPrinting.IPrintable IChartRenderProvider.Printable { get { return null; } }
		public Rectangle DisplayBounds { get; set; }
		public object LookAndFeel {
			get { return null; } 
		}
		ComponentExporter IChartRenderProvider.CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		public void Invalidate() { }
		public void InvokeInvalidate() {
			throw new NotImplementedException();
		}
		public Bitmap LoadBitmap(string url) {
			return null;
		}
		#endregion
		#region IChartContainer Members
		public SNChart Chart { get { return chart; } }
		Chart IChartContainer.Chart { get { return chart; } }
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return null; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return null; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		public ChartContainerType ControlType { get { return ChartContainerType.WinControl; } }
		public IServiceProvider ServiceProvider { get { return serviceProvider; } }
		public bool DesignMode { get { return false; } }
		public bool IsEndUserDesigner { get { return false; } }
		public bool Loading { get { return false; } }
		public bool ShouldEnableFormsSkins { get { return true; } }
		public void Assign(SNChart chart) {
			this.chart = chart;
		}
		void IChartContainer.Assign(Chart chart) {
			Assign((SNChart)chart);
		}
		public void ShowErrorMessage(string message, string title) {
			IMessageBoxService service = (IMessageBoxService)ServiceProvider.GetService(typeof(IMessageBoxService));
			service.ShowMessageBox(title, message);
		}
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		#region Not implemented
		public event EventHandler EndLoading {
			add { }
			remove { }
		}
		public IComponent Parent {
			get { throw new NotImplementedException(); }
		}
		public bool ShowDesignerHints {
			get { throw new NotImplementedException(); }
		}
		public ISite Site {
			get { return  null; }
			set { }
		}
		public void Changed() { }
		public void Changing() { }
		public void LockChangeService() { }
		public void UnlockChangeService() { }
		public void RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) { }
		#endregion
		#endregion
		public void Dispose() {
			Chart.Dispose();
			IDisposable disposableServiceProvider = ServiceProvider as IDisposable;
			if(disposableServiceProvider != null)
				disposableServiceProvider.Dispose();
		}
	}
}
