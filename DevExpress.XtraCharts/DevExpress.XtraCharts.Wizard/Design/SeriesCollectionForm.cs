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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.LookAndFeel;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraTab;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
namespace DevExpress.XtraCharts.Design {
	public class SeriesCollectionForm : XtraForm {
		readonly Chart chart;
		readonly SeriesCollection collection;
		readonly IDesignerHost designerHost;
		IContainer components = null;
		PointsGrid pointsGrid;
		ImageList seriesImages;
		XtraTabControl tabControl;
		XtraTabPage tabPagePoints;
		XtraTabPage tabPageProperties;
		ImageListBoxControl lvSeries;
		SimpleButton btnUp;
		SimpleButton btnDown;
		SimpleButton btnAdd;
		SimpleButton btnCopy;
		SimpleButton btnRemove;
		SimpleButton btnClose;
		PanelControl panelSeries;
		PanelControl panelIncompatibility;
		MemoEdit memoIncompatibility;
		LabelControl labelSeparator;
		PropertyGridControl propertyGrid;
		int startSeriesIndex;
		bool firstActivated = true;
		bool lockSeriesChanging;
		Font boldListBoxFont;
		int SelectedIndex {
			get {
				int selectedIndex = lvSeries.SelectedIndex;
				return selectedIndex < 0 ? selectedIndex : (selectedIndex + startSeriesIndex);
			}
			set {
				int index = value - startSeriesIndex;
				if (index >= 0 && index < lvSeries.ItemCount)
					lvSeries.SelectedIndex = index;
			}
		}
		SeriesCollectionForm() {
			InitializeComponent();
		}
		public SeriesCollectionForm(Chart chart) : this() {
			this.chart = chart;
			collection = chart.Series;
			btnUp.ImageLocation = ImageLocation.MiddleCenter;
			btnDown.ImageLocation = ImageLocation.MiddleCenter;
			IChartContainer container = chart.Container;
			IServiceProvider serviceProvider = container.ServiceProvider;
			if (serviceProvider != null) {
				propertyGrid.ServiceProvider = chart.Container;
				designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
				if (designerHost != null)
					designerHost.TransactionClosed += new DesignerTransactionCloseEventHandler(OnTransactionClosed);
			}
			UserLookAndFeel lookAndFeel = (UserLookAndFeel)container.RenderProvider.LookAndFeel;
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			pointsGrid.LookAndFeel = lookAndFeel;
			SyncListBox();
			if (lvSeries.ItemCount > 0)
				SelectedIndex = startSeriesIndex;
			EnableControls();
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesCollectionForm));
			DevExpress.LookAndFeel.Design.UserLookAndFeelDefault userLookAndFeelDefault1 = new DevExpress.LookAndFeel.Design.UserLookAndFeelDefault();
			this.pointsGrid = new DevExpress.XtraCharts.Design.PointsGrid();
			this.seriesImages = new System.Windows.Forms.ImageList(this.components);
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tabPagePoints = new DevExpress.XtraTab.XtraTabPage();
			this.tabPageProperties = new DevExpress.XtraTab.XtraTabPage();
			this.lvSeries = new DevExpress.XtraEditors.ImageListBoxControl();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnCopy = new DevExpress.XtraEditors.SimpleButton();
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.panelSeries = new DevExpress.XtraEditors.PanelControl();
			this.panelIncompatibility = new DevExpress.XtraEditors.PanelControl();
			this.memoIncompatibility = new DevExpress.XtraEditors.MemoEdit();
			this.labelSeparator = new DevExpress.XtraEditors.LabelControl();
			this.propertyGrid = new DevExpress.XtraVerticalGrid.PropertyGridControl();
			((System.ComponentModel.ISupportInitialize)(this.pointsGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tabPagePoints.SuspendLayout();
			this.tabPageProperties.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelSeries)).BeginInit();
			this.panelSeries.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelIncompatibility)).BeginInit();
			this.panelIncompatibility.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.memoIncompatibility.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).BeginInit();
			this.SuspendLayout();
			this.pointsGrid.AllowSorting = false;
			this.pointsGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.pointsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.pointsGrid.CaptionVisible = false;
			this.pointsGrid.DataMember = "";
			resources.ApplyResources(this.pointsGrid, "pointsGrid");
			this.pointsGrid.FlatMode = true;
			this.pointsGrid.HeaderForeColor = System.Drawing.SystemColors.ControlText;
			this.pointsGrid.LookAndFeel = userLookAndFeelDefault1;
			this.pointsGrid.Name = "pointsGrid";
			this.pointsGrid.ParentRowsVisible = false;
			this.seriesImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			resources.ApplyResources(this.seriesImages, "seriesImages");
			this.seriesImages.TransparentColor = System.Drawing.Color.Magenta;
			resources.ApplyResources(this.tabControl, "tabControl");
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tabPagePoints;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabPagePoints,
			this.tabPageProperties});
			this.tabControl.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_SelectedPageChanged);
			this.tabPagePoints.Controls.Add(this.pointsGrid);
			this.tabPagePoints.Name = "tabPagePoints";
			resources.ApplyResources(this.tabPagePoints, "tabPagePoints");
			this.tabPageProperties.Controls.Add(this.propertyGrid);
			this.tabPageProperties.Name = "tabPageProperties";
			resources.ApplyResources(this.tabPageProperties, "tabPageProperties");
			this.lvSeries.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.lvSeries, "lvSeries");
			this.lvSeries.ImageList = this.seriesImages;
			this.lvSeries.Name = "lvSeries";
			this.lvSeries.SelectedIndexChanged += new System.EventHandler(this.lvSeries_SelectedIndexChanged);
			this.lvSeries.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lvSeries_DrawItem);
			this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			resources.ApplyResources(this.btnCopy, "btnCopy");
			this.btnCopy.Name = "btnCopy";
			this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnClose.Name = "btnClose";
			this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
			resources.ApplyResources(this.panelSeries, "panelSeries");
			this.panelSeries.Controls.Add(this.lvSeries);
			this.panelSeries.Controls.Add(this.panelIncompatibility);
			this.panelSeries.Name = "panelSeries";
			this.panelIncompatibility.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelIncompatibility.Controls.Add(this.memoIncompatibility);
			resources.ApplyResources(this.panelIncompatibility, "panelIncompatibility");
			this.panelIncompatibility.Name = "panelIncompatibility";
			resources.ApplyResources(this.memoIncompatibility, "memoIncompatibility");
			this.memoIncompatibility.Name = "memoIncompatibility";
			this.memoIncompatibility.Properties.AllowFocused = false;
			this.memoIncompatibility.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("memoIncompatibility.Properties.Appearance.BackColor")));
			this.memoIncompatibility.Properties.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("memoIncompatibility.Properties.Appearance.ForeColor")));
			this.memoIncompatibility.Properties.Appearance.Options.UseBackColor = true;
			this.memoIncompatibility.Properties.Appearance.Options.UseForeColor = true;
			this.memoIncompatibility.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.memoIncompatibility.Properties.ReadOnly = true;
			this.memoIncompatibility.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.memoIncompatibility.Properties.WordWrap = false;
			resources.ApplyResources(this.labelSeparator, "labelSeparator");
			this.labelSeparator.LineVisible = true;
			this.labelSeparator.Name = "labelSeparator";
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.CellValueChanged += new DevExpress.XtraVerticalGrid.Events.CellValueChangedEventHandler(this.propertyGrid_CellValueChanged);
			resources.ApplyResources(this, "$this");
			this.CancelButton = this.btnClose;
			this.ControlBox = false;
			this.Controls.Add(this.labelSeparator);
			this.Controls.Add(this.panelSeries);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.btnCopy);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.tabControl);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SeriesCollectionForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Activated += new System.EventHandler(this.SeriesCollectionForm_Activated);
			this.Closed += new System.EventHandler(this.SeriesCollectionForm_Closed);
			((System.ComponentModel.ISupportInitialize)(this.pointsGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tabPagePoints.ResumeLayout(false);
			this.tabPageProperties.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelSeries)).EndInit();
			this.panelSeries.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelIncompatibility)).EndInit();
			this.panelIncompatibility.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.memoIncompatibility.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.propertyGrid)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		void SyncListBox() {
			lvSeries.Items.Clear();
			seriesImages.Images.Clear();
			startSeriesIndex = -1;
			for (int i = 0, index = 0; i < collection.Count; i++) {
				Series series = collection[i];
				if (SeriesDataBindingUtils.IsPermanent(series) && !series.IsAutoCreated) {
					if (startSeriesIndex < 0)
						startSeriesIndex = i;
					seriesImages.Images.Add(ImageResourcesUtils.GetImageFromResources(series.View, SeriesViewImageType.SmallImage));
					lvSeries.Items.Add(new ImageListBoxItem(series, index++));
				}
			}
			if (startSeriesIndex < 0)
				startSeriesIndex = 0;
		}
		void EnableControls() {
			int selectedIndex = SelectedIndex;
			bool isAutocreated = selectedIndex >= startSeriesIndex && collection[selectedIndex].IsAutoCreated;
			btnUp.Enabled = !isAutocreated && selectedIndex > startSeriesIndex && !collection[selectedIndex - 1].IsAutoCreated;
			btnDown.Enabled = !isAutocreated && selectedIndex < collection.Count - 1;
			btnRemove.Enabled = selectedIndex >= startSeriesIndex && !isAutocreated;
			btnCopy.Enabled = selectedIndex >= startSeriesIndex && !isAutocreated;
		}
		void UpdatePropertyGrid(Series series) {
			propertyGrid.SelectedObject = series.IsAutoCreated ? chart.DataContainer.SeriesTemplate : series;
		}
		void UpdateSeriesStatusPanel() {
			int selectedIndex = SelectedIndex;
			if (selectedIndex >= startSeriesIndex) {
				SeriesIncompatibilityStatistics seriesIncompatibilityStatistics = CommonUtils.GetSeriesIncompatibilityStatistics(chart);
				SeriesIncompatibilityInfo info = seriesIncompatibilityStatistics[collection[selectedIndex]];
				if (info != null && info.Count > 0) {
					string test = SeriesIncompatibilityHelper.ConstructMessage(info);
					if (string.IsNullOrEmpty(test)) {
						panelIncompatibility.Visible = false;
						return;
					}
					memoIncompatibility.Text = ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesHeader) + test;
					panelIncompatibility.Size = new Size(0, memoIncompatibility.Font.Height * (info.Count + 1));
					panelIncompatibility.Visible = true;
					lvSeries.MakeItemVisible(selectedIndex);
					return;
				}
			}
			panelIncompatibility.Visible = false;
		}
		void OnSelectedValueChanged() {
			if (SelectedIndex >= startSeriesIndex) {
				Series series = collection[SelectedIndex];
				if (tabControl.SelectedTabPageIndex == 1 && tabPageProperties.IsHandleCreated)
					UpdatePropertyGrid(series);
				pointsGrid.DataSource = series.Points;
				pointsGrid.Visible = true;
				pointsGrid.Enabled = CommonUtils.IsUnboundMode(series);
				chart.SelectHitElement(series);
				pointsGrid.HorizScrollBar.Visible = true;
			}
			else {
				if (tabPageProperties.IsHandleCreated)
					propertyGrid.SelectedObject = null;
				pointsGrid.DataSource = null;
				pointsGrid.Visible = false;
			}
			EnableControls();
			UpdateSeriesStatusPanel();
		}
		void OnTransactionClosed(object sender, DesignerTransactionCloseEventArgs args) {
			if (!args.TransactionCommitted) {
				int index = SelectedIndex;
				SyncListBox();
				SelectedIndex = index;
			}
		}
		void SeriesCollectionForm_Activated(object sender, EventArgs e) {
			if (!firstActivated) {
				pointsGrid.Focus();
				firstActivated = true;
			}
		}
		void SeriesCollectionForm_Closed(object sender, EventArgs e) {
			if (designerHost != null)
				designerHost.TransactionClosed -= new DesignerTransactionCloseEventHandler(OnTransactionClosed);
		}
		void tabControl_SelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			if (SelectedIndex >= startSeriesIndex) {
				pointsGrid.ValidateCollection();
				Series series = collection[SelectedIndex];
				if (tabControl.SelectedTabPageIndex == 0) {
					pointsGrid.Enabled = CommonUtils.IsUnboundMode(series);
					pointsGrid.DataSource = null;
					pointsGrid.DataSource = series.Points;
					pointsGrid.HorizScrollBar.Visible = true;
				}
				else if (tabControl.SelectedTabPageIndex == 1)
					UpdatePropertyGrid(series);
			}
		}
		void lvSeries_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			Series series = (Series)e.Item;
			if (boldListBoxFont == null)
				boldListBoxFont = new Font(e.Appearance.Font, FontStyle.Bold);
			if (series.IsAutoCreated)
				e.Appearance.Font = boldListBoxFont;
		}
		void lvSeries_SelectedIndexChanged(object sender, EventArgs e) {
			if (!lockSeriesChanging)
				OnSelectedValueChanged();
		}
		void btnAdd_Click(object sender, EventArgs e) {
			if (SelectedIndex >= startSeriesIndex)
				pointsGrid.ValidateCollection();
			using (ViewTypesForm form = new ViewTypesForm()) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				form.ShowDialog();
				string viewStringId = form.EditValue;
				if (!String.IsNullOrEmpty(viewStringId)) {
					Series series = new Series(collection.GenerateName(), SeriesViewFactory.GetViewType(viewStringId));
					ChartDesignHelper.InitializeDefaultGanttScaleType(series);
					int index = collection.Add(series);
					SyncListBox();
					SelectedIndex = index;
					lvSeries.MakeItemVisible(index - startSeriesIndex);
				}
			}
		}
		void btnCopy_Click(object sender, EventArgs e) {
			if (SelectedIndex >= 0) {
				pointsGrid.ValidateCollection();
				Series currentSeries = collection[SelectedIndex];
				Series series = (Series)((ICloneable)currentSeries).Clone();
				series.Name = collection.GenerateName();
				int index = collection.Add(series);
				SyncListBox();
				SelectedIndex = index;
				lvSeries.MakeItemVisible(index - startSeriesIndex);
			}
		}
		void btnRemove_Click(object sender, EventArgs e) {
			if (SelectedIndex >= 0) {
				collection.RemoveAt(SelectedIndex);
				int nextIndex = SelectedIndex;
				if (nextIndex == collection.Count)
					nextIndex--;
				SyncListBox();
				if (nextIndex >= startSeriesIndex) {
					SelectedIndex = nextIndex;
					lvSeries.MakeItemVisible(nextIndex - startSeriesIndex);
				}
				if (collection.Count == startSeriesIndex)
					OnSelectedValueChanged();
			}
		}
		void btnUp_Click(object sender, EventArgs e) {
			int index = SelectedIndex;
			if (index > startSeriesIndex) {
				pointsGrid.ValidateCollection();
				collection.Swap(collection[index - 1], collection[index]);
				SyncListBox();
				SelectedIndex = index - 1;
				lvSeries.MakeItemVisible(SelectedIndex);
			}
		}
		void btnDown_Click(object sender, EventArgs e) {
			int index = SelectedIndex;
			if (index >= startSeriesIndex && index < collection.Count - 1) {
				pointsGrid.ValidateCollection();
				collection.Swap(collection[index], collection[index + 1]);
				SyncListBox();
				SelectedIndex = index + 1;
				lvSeries.MakeItemVisible(SelectedIndex - startSeriesIndex);
				if (!btnDown.Enabled)
					btnUp.Focus();
			}
		}
		void btnClose_Click(object sender, EventArgs e) {
			pointsGrid.ValidateCollection();
			Controls.Clear(); 
		}
		void propertyGrid_CellValueChanged(object sender, CellValueChangedEventArgs e) {
			if (e.Value != null) {
				lockSeriesChanging = true;
				try {
					int index = SelectedIndex;
					SyncListBox();
					SelectedIndex = index;
					UpdateSeriesStatusPanel();
				}
				finally {
					lockSeriesChanging = false;
				}
				if (typeof(SeriesViewBase).IsAssignableFrom(e.Value.GetType())) {
					using (Image image = ImageResourcesUtils.GetImageFromResources((SeriesViewBase)e.Value, SeriesViewImageType.SmallImage))
						seriesImages.Images[SelectedIndex - startSeriesIndex] = image;
					pointsGrid.UpdateTableStyle();
				}
			}
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
				if (boldListBoxFont != null) {
					boldListBoxFont.Dispose();
					boldListBoxFont = null;
				}
			}
			base.Dispose(disposing);
		}
		public void EditPoints(Series series) {
			int result = collection.IndexOf(series);
			if (result >= 0) {
				SelectedIndex = result;
				firstActivated = false;
			}
		}
	}
}
