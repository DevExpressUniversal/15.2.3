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
using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Extensions.Native;
using DevExpress.Snap.Native;
using DevExpress.LookAndFeel;
using DevExpress.Services.Internal;
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
using DevExpress.Snap.Core.Commands;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Extensions {
	public class ChartContainerProxy : IChartContainer, IChartRenderProvider, IChartDataProvider, IDisposable {
		readonly ServiceManager serviceManager;
		readonly ChartCommandFactory commandFactory;
		readonly Chart placeholderChart;
		public ChartContainerProxy() {
			this.serviceManager = new ServiceManager();
			this.commandFactory = new ChartCommandFactory(this);
			this.placeholderChart = new Chart(this);
		}
		public ISNChartContainer InnerContainer { get; private set; }
		public void SetInnerContainer(ISNChartContainer container) {
			DisposeInnerContainer();
			InnerContainer = container;
		}
		public void SaveChart(SNChart chart) {
			IFieldChanger service = (IFieldChanger)this.serviceManager.GetService(typeof(IFieldChanger));
			service.ApplyNewValue(controller => {
				DocumentModel newValue = SNChartHelper.CreateChartLayout(chart, controller.DocumentModel);
				controller.SetSwitch(SNChartField.ChartLayoutSwitch, newValue, false);
			});
			service.ApplyNewValue(controller => {
				DocumentModel newValue = SNChartHelper.CreateChartSeriesDataBindings(chart, controller.DocumentModel);
				controller.SetSwitch(SNChartField.ChartSeriesDataBindingsSwitch, newValue, false);
			});
			string chartDataSourceName = chart.GetChartDataBindingName();
			if(String.IsNullOrEmpty(chartDataSourceName))
				service.ApplyNewValue(controller => controller.RemoveSwitch(SNChartField.ChartDataBindingsSwitch));
			else
				service.ApplyNewValue(controller => controller.SetSwitch(SNChartField.ChartDataBindingsSwitch, chartDataSourceName));
		}
		public void UpdateFieldChangerService(SnapFieldInfo fieldInfo) {
			if(serviceManager.IsServiceExists(typeof(IFieldChanger)))
				serviceManager.RemoveService(typeof(IFieldChanger));
			if(fieldInfo != null)
				serviceManager.AddService(typeof(IFieldChanger), new FieldChanger(fieldInfo, new ParsedInfoProvider(fieldInfo)));
		}
		public void ClearInnerContainer() {
			SetInnerContainer(null);
			UpdateFieldChangerService(null);
		}
		#region ISupportBarsInteraction implementation
		CommandBasedKeyboardHandler<ChartCommandId> ICommandAwareControl<ChartCommandId>.KeyboardHandler { get { return null; } }
		EventHandler beforeDispose;
		event EventHandler ICommandAwareControl<ChartCommandId>.BeforeDispose { add { beforeDispose += value; } remove { beforeDispose -= value; } }
		event EventHandler ICommandAwareControl<ChartCommandId>.UpdateUI {
			add {
				if(InnerContainer != null)
					InnerContainer.UpdateUI += value;
			}
			remove {
				if(InnerContainer != null)
					InnerContainer.UpdateUI -= value;
			}
		}
		Command ICommandAwareControl<ChartCommandId>.CreateCommand(ChartCommandId id) {
			return new ExecuteAndSaveLayoutCommand(this, this.commandFactory.CreateCommand(id));
		}
		bool ICommandAwareControl<ChartCommandId>.HandleException(Exception e) {
			return false;
		}
		void ICommandAwareControl<ChartCommandId>.Focus() {
		}
		void ICommandAwareControl<ChartCommandId>.CommitImeContent() {
		}
		object IServiceProvider.GetService(Type serviceType) {
			return InnerContainer == null ? null : InnerContainer.GetService(serviceType);
		}
		public void RaiseUIUpdated() { }
		#endregion
		#region IChartContainer Members
		#region IChartDataProvider implementation
		public DataContext DataContext { get { return InnerContainer == null ? null : InnerContainer.DataContext; } }
		public object DataSource {
			get { return InnerContainer == null ? null : InnerContainer.DataSource; }
			set {
				if (InnerContainer != null)
					InnerContainer.DataSource = value;
			}
		}
		public bool SeriesDataSourceVisible { get { return InnerContainer == null ? true : InnerContainer.SeriesDataSourceVisible; } }
		public object ParentDataSource { get { return InnerContainer == null ? null : InnerContainer.ParentDataSource; } }
		public bool CanUseBoundPoints { get { return InnerContainer == null ? true : InnerContainer.CanUseBoundPoints; } }
		#endregion
		#region IChartRenderProvider implementation
		DevExpress.XtraPrinting.IPrintable IChartRenderProvider.Printable { get { return null; } }
		#endregion
		IChartDataProvider IChartContainer.DataProvider { get { return this; } }
		IChartRenderProvider IChartContainer.RenderProvider { get { return this; } }
		IChartEventsProvider IChartContainer.EventsProvider { get { return null; } }
		IChartInteractionProvider IChartContainer.InteractionProvider { get { return null; } }
		bool IChartContainer.IsDesignControl { get { return false; } }
		bool IChartContainer.CanDisposeItems { get { return true; } }
		public Chart Chart { get { return InnerContainer == null ? placeholderChart : InnerContainer.Chart; } }
		public ChartContainerType ControlType { get { return ChartContainerType.WinControl; } }
		public IServiceProvider ServiceProvider { get { return InnerContainer == null ? null : InnerContainer.ServiceProvider; } }
		public bool DesignMode { get { return InnerContainer == null ? false : InnerContainer.DesignMode; } }
		public Rectangle DisplayBounds { get { return InnerContainer == null ? Rectangle.Empty : InnerContainer.DisplayBounds; } }
		public bool IsEndUserDesigner { get { return InnerContainer == null ? false : InnerContainer.IsEndUserDesigner; } }
		public bool Loading { get { return InnerContainer == null ? false : InnerContainer.Loading; } }
		public bool ShouldEnableFormsSkins { get { return InnerContainer == null ? true : InnerContainer.ShouldEnableFormsSkins; } }
		public object LookAndFeel { get { return InnerContainer == null ? UserLookAndFeel.Default : (UserLookAndFeel)InnerContainer.LookAndFeel; } }
		public void Assign(Chart chart) {
			if(InnerContainer != null)
				InnerContainer.Assign(chart);
		}
		public Bitmap LoadBitmap(string url) {
			return InnerContainer == null ? null : InnerContainer.LoadBitmap(url);
		}
		public void ShowErrorMessage(string message, string title) {
			if(InnerContainer != null)
				InnerContainer.ShowErrorMessage(message, title);
		}
		bool IChartContainer.GetActualRightToLeft() {
			return false;
		}
		#region Not implemented
		public event BoundDataChangedEventHandler BoundDataChanged {
			add { }
			remove { }
		}
		public event EventHandler EndLoading {
			add { }
			remove { }
		}
		public object DataAdapter {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		public IComponent Parent {
			get { throw new NotImplementedException(); }
		}
		public bool ShowDesignerHints {
			get { throw new NotImplementedException(); }
		}
		public ISite Site {
			get { throw new NotImplementedException(); }
			set { throw new NotImplementedException(); }
		}
		public void InvokeInvalidate() {
			throw new NotImplementedException();
		}
		public bool ShouldSerializeDataSource(object dataSource) {
			throw new NotImplementedException();
		}
		public void Changed() { }
		public void Changing() { }
		public void Invalidate() { }
		public void LockChangeService() { }
		public void OnBoundDataChanged(EventArgs e) { }
		public void OnPivotGridSeriesExcluded(PivotGridSeriesExcludedEventArgs e) { }
		public void OnPivotGridSeriesPointsExcluded(PivotGridSeriesPointsExcludedEventArgs e) { }
		public void UnlockChangeService() { }
		public void RaiseRangeControlRangeChanged(object minValue, object maxValue, bool invalidate) { }
		public bool IsPrintingAvailable { get { return false; } }
		public ComponentExporter CreateComponentPrinter(IPrintable iPrintable) {
			return null;
		}
		#endregion
		#endregion
		public void Dispose() {
			this.beforeDispose.SafeRaise(this, EventArgs.Empty);
			DisposeInnerContainer();
			this.serviceManager.Dispose();
			this.placeholderChart.Dispose();
		}
		void DisposeInnerContainer() {
			IDisposable disposableContainer = InnerContainer as IDisposable;
			if(disposableContainer != null)
				disposableContainer.Dispose();
		}
	}
	public class ExecuteAndSaveLayoutCommand : ChartCommand {
		readonly ChartContainerProxy chartContainerProxy;
		readonly Command command;
		public ExecuteAndSaveLayoutCommand(ChartContainerProxy chartContainerProxy, Command command)
			: base(chartContainerProxy) {
			this.chartContainerProxy = chartContainerProxy;
			this.command = command;
		}
		public new SNChart Chart { get { return base.Chart as SNChart; } }
		public override string Description { get { return command.Description; } }
		public override string MenuCaption { get { return command.MenuCaption; } }
		public override Image Image { get { return command.Image; } }
		public override Image LargeImage { get { return command.LargeImage; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			command.UpdateUIState(state);
		}
		protected override void ExecuteCore(ICommandUIState state) {
			base.ExecuteCore(state);
			if (Chart == null)
				return;
			command.ForceExecute(state);
			chartContainerProxy.SaveChart(Chart);
		}
	}
}
