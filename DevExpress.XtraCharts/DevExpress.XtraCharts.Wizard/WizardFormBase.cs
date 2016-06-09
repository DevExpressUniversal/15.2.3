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
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
using DevExpress.XtraCharts.Wizard;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
namespace DevExpress.XtraCharts.Native {
	public partial class WizardFormBase : XtraForm {
		const double invisibleAreaPercent = 0.2;
		public static bool ShowWizardOnChartAdding {
			get {
				PropertyStore store = new PropertyStore(ChartWizard.XtraChartsRegistryPath);
				if (store == null)
					return true;
				store.Restore();
				return store.RestoreBoolProperty(ChartWizard.XtraChartsShowWizardRegistryEntry, true);
			}
			set {
				PropertyStore store = new PropertyStore(ChartWizard.XtraChartsRegistryPath);
				if (store == null)
					return;
				store.AddProperty(ChartWizard.XtraChartsShowWizardRegistryEntry, value);
				store.Store();
			}
		}
		bool loading = true;
		WizardFormLayout layout;
		FilterSeriesTypesCollection filterSeriesCollection;
		IDesignerHost designerHost;
		Chart originalChart;
		Chart chart;
		ChartDesignControl designControl;
		List<WizardPage> pages = new List<WizardPage>();
		WizardStateController controller;
		IChartContainer chartContainer;
		bool Loading { get { return loading; } }
		internal WizardFormLayout FormLayout { get { return layout; } }
		public Chart OriginalChart { get { return originalChart; } }
		public Chart Chart { get { return chart; } }
		public ChartDesignControl DesignControl { get { return designControl; } }
		public IDesignerHost DesignerHost { get { return designerHost; } }
		public FilterSeriesTypesCollection FilterSeriesCollection { get { return filterSeriesCollection; } }
		public WizardStateController Controller { get { return controller; } }
		public IServiceProvider ServiceProvider { get { return chartContainer; } }
		WizardFormBase() {
			InitializeComponent();
		}
		public WizardFormBase(ChartWizard wizard, IList<WizardPage> pages, UserLookAndFeel lookAndFeel) : this() {
			Text = wizard.Caption;
			lblTitle.Text = wizard.Description;
			if (wizard.Icon == null)
				ShowIcon = false;
			else {
				ShowIcon = true;
				Icon = wizard.Icon;
			}
			peWizardImage.Image = wizard.LeftImage;
			peLogo.Image = wizard.RightImage;
			chartContainer = wizard.ChartContainer;
			layout = wizard.Layout;
			filterSeriesCollection = wizard.FilterSeriesTypes;
			originalChart = wizard.ChartContainer.Chart;
			IChartDataProvider dataProvider = wizard.ChartContainer.DataProvider;
			if (dataProvider != null)
				designControl = new ChartDesignControl(chartContainer.ControlType, dataProvider.ParentDataSource, GetActualDataContext(wizard.ChartContainer.ServiceProvider, dataProvider.DataContext));
			else
				designControl = new ChartDesignControl(chartContainer.ControlType, null, GetActualDataContext(wizard.ChartContainer.ServiceProvider, null));
			chart = new Chart(designControl);
			designControl.Chart = chart;
			chart.Assign(originalChart);
			designControl.SelectionMode = ElementSelectionMode.None;
			designerHost = wizard.DesignerHost;
			cbShowOnNextStart.Checked = ShowWizardOnChartAdding;
			InitializeFormByLayout();
			LookAndFeel.Assign(lookAndFeel);
			this.pages.AddRange(pages);
			grWizardPanel.Initialize(this, this.pages);
			controller = new WizardStateController(this, this.pages);
			pnlTitle.Visible = wizard.LeftImage != null || wizard.RightImage != null || !String.IsNullOrEmpty(wizard.Description);
			EndLoading();
		}
		DataContext GetActualDataContext(IServiceProvider serviceProvider, DataContext defaultContext) {
			if (serviceProvider != null) {
				IDataContextService service = serviceProvider.GetService(typeof(IDataContextService)) as IDataContextService;
				if (service != null)
					return service.CreateDataContext(new DataContextOptions(true, true), false);
			}
			return defaultContext;
		}
		void InitializeFormByLayout() {
			StartPosition = layout.StartPosition;
			if (layout.Size != Size.Empty)
				Size = layout.Size;
			if (layout.Location != Point.Empty)
				Location = layout.Location;
			WindowState = layout.Maximized ? FormWindowState.Maximized : FormWindowState.Normal;
			Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;
			if (StartPosition == FormStartPosition.Manual && !workingArea.Contains(Bounds)) 
				if (workingArea.Left > Left && (workingArea.Left - Left) / (double)Width > invisibleAreaPercent ||
					workingArea.Top > Top && (workingArea.Top - Top) / (double)Height > invisibleAreaPercent ||
					Right > workingArea.Right && (Right - workingArea.Right) / (double)Width > invisibleAreaPercent ||
					Bottom > workingArea.Bottom && (Bottom - workingArea.Bottom) / (double)Height > invisibleAreaPercent) 
						StartPosition = FormStartPosition.CenterScreen;			
		}
		void EndLoading() {
			loading = false;
		}
		void sbPreviousPage_Click(object sender, EventArgs e) {
			SelectPreviousPage();
		}
		void sbNextPage_Click(object sender, EventArgs e) {
			SelectNextPage();
		}
		void sbFinish_Click(object sender, EventArgs e) {
			if (controller.CanChangeState() && controller.State.PrepareChangeState()) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}
		void sbCancel_Click(object sender, EventArgs e) {
			DialogResult = DialogResult.Cancel;
			Close();
		}
		void BaseWizardForm_SizeChanged(object sender, EventArgs e) {
			if (grWizardPanel.ParentControl != null)
				grWizardPanel.ParentControl.Control.Tag = null;
		}
		void ShowOnNextStart_CheckedChanged(object sender, EventArgs e) {
			ShowWizardOnChartAdding = cbShowOnNextStart.Checked;
		}
		void WizardFormBase_ResizeEnd(object sender, EventArgs e) {
			layout.Size = Size;
			layout.Location = Location;
		}
		void WizardFormBase_FormClosed(object sender, FormClosedEventArgs e) {
			if (layout != null)
				layout.Maximized = WindowState == FormWindowState.Maximized;
		}
		void WizardFormBase_Move(object sender, EventArgs e) {
			if (layout != null && WindowState == FormWindowState.Normal)
				layout.Location = Location;
		}
		void FocusMainControl() {
			ChartTypeControl chartTypeControl = controller.State.WizardControl as ChartTypeControl;
			if (chartTypeControl != null)
				chartTypeControl.FocusImagesControl();
		}
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			BeginInvoke(new MethodInvoker(delegate { FocusMainControl(); }));
		}
		public virtual void InitializeChart(WizardFormBase form) {
		} 
		public virtual void CompleteChanges() {
		}
		public virtual void Release() {
		}
		public virtual bool ValidateContent() {
			return Loading ? true : controller.CanChangeState();
		}
		public bool SelectCustomPage(int index) {
			return controller.CustomState(index);
		}
		public void SelectNextPage() {
			controller.NextState();
		}
		public void SelectPreviousPage() {
			controller.PreviousState();
		}
		public void HideStartupCheckBox() {
			cbShowOnNextStart.Visible = false;
		}
		public void SetNewChart(Chart chart) {
			this.chart = chart;
			designControl.Chart = chart;
		}
		public void EnableNextButton(bool enable) {
			sbNextPage.Enabled = enable;
		}
		public void EnablePreviousButton(bool enable) {
			sbPreviousPage.Enabled = enable;
		}
	}
}
