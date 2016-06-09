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
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Design {
	public class SeriesPointListForm : XtraForm {
		private System.Windows.Forms.TabPage tpPoints;
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ImageList seriesImages;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.PanelControl panelControl2;
		private SimpleButton simpleButtonOk;
		private SimpleButton simpleButtonCancel;	   
		readonly SeriesCollection collection;
		SeriesPoint point = null;
		PanelControl pointsPanel;
		ImageListBoxControl lvSeries;
		MemoEdit memoEdit1;
		DataGridView pointsGrid;
		LabelControl labelControl1;
		Font boldListBoxFont;
		protected Label label;
		protected Label label1;
		int SeriesSelectedIndex {
			get {
				return (collection.Count > 0 && lvSeries.SelectedIndices.Count > 0) ?
					lvSeries.SelectedIndices[0] : -1;
			}			
		}
		protected virtual bool IsInitialized { get { return true; } }
		protected virtual bool IsPointSelected { get { return point != null; } }
		public SeriesPoint EditValue {
			get {
				return point;
			}
			set {
				point = value;
				UpdateControls();
			}
		}
		SeriesPointListForm() {
			InitializeComponent();
		}
		public SeriesPointListForm(SeriesCollection series) : this() {
			this.collection = series;
			InitSeriesListBox();
		}
		protected override void Dispose(bool disposing) {
			if (boldListBoxFont != null) {
				boldListBoxFont.Dispose();
				boldListBoxFont = null;
			}
			if (disposing && components != null) 
				components.Dispose();
			base.Dispose(disposing);
		}
		#region Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SeriesPointListForm));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			this.tpPoints = new System.Windows.Forms.TabPage();
			this.seriesImages = new System.Windows.Forms.ImageList(this.components);
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.lvSeries = new DevExpress.XtraEditors.ImageListBoxControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
			this.simpleButtonOk = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
			this.label = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.pointsPanel = new DevExpress.XtraEditors.PanelControl();
			this.pointsGrid = new System.Windows.Forms.DataGridView();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pointsPanel)).BeginInit();
			this.pointsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pointsGrid)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.tpPoints, "tpPoints");
			this.tpPoints.Name = "tpPoints";
			this.seriesImages.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			resources.ApplyResources(this.seriesImages, "seriesImages");
			this.seriesImages.TransparentColor = System.Drawing.Color.Magenta;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.panelControl1.Controls.Add(this.lvSeries);
			this.panelControl1.Controls.Add(this.panelControl2);
			this.panelControl1.Name = "panelControl1";
			this.lvSeries.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.lvSeries, "lvSeries");
			this.lvSeries.ImageList = this.seriesImages;
			this.lvSeries.Name = "lvSeries";
			this.lvSeries.SelectedIndexChanged += new System.EventHandler(this.lvSeries_SelectedIndexChanged);
			this.lvSeries.DrawItem += new DevExpress.XtraEditors.ListBoxDrawItemEventHandler(this.lvSeries_DrawItem);
			this.panelControl2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl2.Controls.Add(this.memoEdit1);
			resources.ApplyResources(this.panelControl2, "panelControl2");
			this.panelControl2.Name = "panelControl2";
			resources.ApplyResources(this.memoEdit1, "memoEdit1");
			this.memoEdit1.Name = "memoEdit1";
			this.memoEdit1.Properties.AllowFocused = false;
			this.memoEdit1.Properties.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("memoEdit1.Properties.Appearance.BackColor")));
			this.memoEdit1.Properties.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("memoEdit1.Properties.Appearance.ForeColor")));
			this.memoEdit1.Properties.Appearance.Options.UseBackColor = true;
			this.memoEdit1.Properties.Appearance.Options.UseForeColor = true;
			this.memoEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.memoEdit1.Properties.ReadOnly = true;
			this.memoEdit1.Properties.ScrollBars = System.Windows.Forms.ScrollBars.None;
			this.memoEdit1.Properties.WordWrap = false;
			this.memoEdit1.UseOptimizedRendering = true;
			resources.ApplyResources(this.simpleButtonOk, "simpleButtonOk");
			this.simpleButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButtonOk.Name = "simpleButtonOk";
			resources.ApplyResources(this.simpleButtonCancel, "simpleButtonCancel");
			this.simpleButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.simpleButtonCancel.Name = "simpleButtonCancel";
			resources.ApplyResources(this.label, "label");
			this.label.Name = "label";
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			resources.ApplyResources(this.pointsPanel, "pointsPanel");
			this.pointsPanel.Controls.Add(this.pointsGrid);
			this.pointsPanel.Name = "pointsPanel";
			this.pointsGrid.AllowUserToAddRows = false;
			this.pointsGrid.AllowUserToDeleteRows = false;
			this.pointsGrid.AllowUserToResizeRows = false;
			this.pointsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
			this.pointsGrid.BackgroundColor = System.Drawing.SystemColors.Window;
			this.pointsGrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.pointsGrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Tahoma", 8.25F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.pointsGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.pointsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.pointsGrid, "pointsGrid");
			this.pointsGrid.EnableHeadersVisualStyles = false;
			this.pointsGrid.GridColor = System.Drawing.SystemColors.ControlLight;
			this.pointsGrid.MultiSelect = false;
			this.pointsGrid.Name = "pointsGrid";
			this.pointsGrid.ReadOnly = true;
			this.pointsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
			this.pointsGrid.RowHeadersVisible = false;
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
			this.pointsGrid.RowsDefaultCellStyle = dataGridViewCellStyle2;
			this.pointsGrid.RowTemplate.DefaultCellStyle.ForeColor = System.Drawing.SystemColors.WindowText;
			this.pointsGrid.RowTemplate.Height = 17;
			this.pointsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.pointsGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.pointsGrid_CellMouseDoubleClick);
			this.pointsGrid.SelectionChanged += new System.EventHandler(this.pointsGrid_SelectionChanged);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.pointsPanel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.simpleButtonOk);
			this.Controls.Add(this.panelControl1);
			this.Controls.Add(this.simpleButtonCancel);
			this.Controls.Add(this.label);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SeriesPointListForm";
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lvSeries)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.memoEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pointsPanel)).EndInit();
			this.pointsPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pointsGrid)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		Series CreateChartItem(string name) {
			using (ViewTypesForm form = new ViewTypesForm()) {
				form.LookAndFeel.ParentLookAndFeel = LookAndFeel;
				form.ShowDialog();
				string viewStringId = form.EditValue;
				if (viewStringId == null) 
					return null;
				Series series = new Series(name, SeriesViewFactory.GetViewType(viewStringId));
				ChartDesignHelper.InitializeDefaultGanttScaleType(series);
				return series;
			}
		}
		void UpdateSeriesStatusPanel() {
			int selectedIndex = SeriesSelectedIndex;
			if (selectedIndex >= 0) {
				IChartContainer continaer = ((IOwnedElement)collection).ChartContainer;
				SeriesIncompatibilityInfo info = CommonUtils.GetSeriesIncompatibilityStatistics(continaer.Chart)[collection[selectedIndex]];
				if (info != null && info.Count > 0) {
					memoEdit1.Text = ChartLocalizer.GetString(ChartStringId.IncompatibleSeriesHeader) + SeriesIncompatibilityHelper.ConstructMessage(info);
					panelControl2.Size = new Size(0, memoEdit1.Font.Height * (info.Count + 1));
					panelControl2.Visible = true;
					lvSeries.MakeItemVisible(selectedIndex);
					return;
				}
			}
			panelControl2.Visible = false;
		}
		void UpdatePointsGrid() {
			pointsGrid.Rows.Clear();
			pointsGrid.Columns.Clear();
			if (SeriesSelectedIndex >= 0) {
				Series series = collection[SeriesSelectedIndex];
				DataGridViewCell pointCell = new DataGridViewTextBoxCell();
				pointCell.ValueType = typeof(SeriesPoint);
				DataGridViewColumn pointColumn = new DataGridViewColumn(pointCell);
				pointColumn.Visible = false;
				pointsGrid.Columns.Add(pointColumn);
				pointsGrid.Columns.Add(ChartLocalizer.GetString(ChartStringId.ArgumentMember), ChartLocalizer.GetString(ChartStringId.ArgumentMember));
				for (int i = 0; i < ((IViewArgumentValueOptions)series.View).PointDimension; i++)
					pointsGrid.Columns.Add(series.View.GetValueCaption(i), series.View.GetValueCaption(i));
				for(int i = 0; i < series.Points.Count; i++) {
					SeriesPoint point = series.Points[i];
					List<object> cells = new List<object>();
					cells.Add(GetPointObject(SeriesSelectedIndex, i));
					cells.Add(point.Argument);
					for (int j = 0; j < point.Values.Length; j++)
						cells.Add(point.GetValueString(j));
					pointsGrid.Rows.Add(cells.ToArray());
				}
			}
		}
		void lvSeries_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			Series series = GetDrawnSeries(e.Item);
			if (boldListBoxFont == null)
				boldListBoxFont = new Font(e.Appearance.Font, FontStyle.Bold);
			if (series.IsAutoCreated)
				e.Appearance.Font = boldListBoxFont;
		}
		void lvSeries_SelectedIndexChanged(object sender, EventArgs e) {
			UpdatePointsGrid();
			UpdateSeriesStatusPanel();
		}
		void pointsGrid_SelectionChanged(object sender, EventArgs e) {
			if (pointsGrid.SelectedRows.Count > 0)
				SetPointValue(pointsGrid.SelectedRows[0].Cells[0].Value);
			else
				point = null;
			UpdateControls();
		}
		void pointsGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e) {
			if(IsPointSelected && (e.RowIndex >= 0 && e.RowIndex < pointsGrid.Rows.Count))
				DialogResult = DialogResult.OK;
		}
		protected void InitSeriesListBox() {
			if(!IsInitialized)
				return;
			lvSeries.Items.Clear();
			seriesImages.Images.Clear();
			for(int i = 0; i < collection.Count; i++) {
				seriesImages.Images.Add(ImageResourcesUtils.GetImageFromResources(collection[i].View, SeriesViewImageType.SmallImage));
				ImageListBoxItem item = new ImageListBoxItem(GetSeriesObject(i), i);
				lvSeries.Items.Add(item);
			}
			if(collection.Count > 0)
				lvSeries.SelectedIndex = 0;
		}
		protected void UpdateControls() {
			simpleButtonOk.Enabled = IsPointSelected;
		}
		protected virtual object GetSeriesObject(int index) {
			return collection[index];
		}
		protected virtual object GetPointObject(int seriesIndex, int pointIndex) {
			return collection[seriesIndex].Points[pointIndex];
		}
		protected virtual Series GetDrawnSeries(object item) {
			return (Series)item;
		}
		protected virtual void SetPointValue(object value) {
			point = value as SeriesPoint;
		}
	}
}
