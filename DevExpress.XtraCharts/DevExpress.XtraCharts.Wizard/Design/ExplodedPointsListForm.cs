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
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Design {
	public class ExplodedPointsListForm : XtraForm {
		class SeriesPointItem : ListViewItem {
			readonly SeriesPoint point;
			public SeriesPoint Point { get { return point; } }
			public bool Exploded { get { return base.Checked; } }
			public SeriesPointItem(SeriesPoint point, bool exploded) {
				this.point = point;
				Text = String.Empty;
				SubItems.Add(point.Argument);
				SubItems.Add(point.GetValueString(0));
				Checked = exploded;
			}
		}
		private PanelControl footerPanel;
		private PanelControl headerPanel;
		private PanelControl contentPanel;
		private SimpleButton simpleButtonClose;
		private LabelControl labelHeader;
		private ListView listViewPoints;
		readonly PieSeriesViewBase view;
		readonly SeriesPointCollection collection;
		readonly IExpoldedPointsCollectionAccessor explodedCollectionAccessor;
		bool loading;
		private DevExpress.XtraEditors.LabelControl labelError;
		ExplodedPointsListForm() {
			InitializeComponent();
		}
		public ExplodedPointsListForm(Series series, ExplodedSeriesPointCollection explodedPoints)
			: this(series, new ExpoldedPointsCollectionAccessor(explodedPoints)) {
		}
		public ExplodedPointsListForm(Series series, IExpoldedPointsCollectionAccessor explodedCollectionAccessor) : this() {
			const int ColumnWidth = 10;
			if (series == null) {
				labelError.Visible = true;
				listViewPoints.Enabled = false;
				headerPanel.Visible = false;
			}
			else {
				view = (PieSeriesViewBase)series.View;
				collection = series.Points;
				this.explodedCollectionAccessor = explodedCollectionAccessor;
				if (collection != null) {
					loading = true;
					listViewPoints.Columns.Add(ChartLocalizer.GetString(ChartStringId.ExplodedPointsDialogExplodedColumn), ColumnWidth, HorizontalAlignment.Center);
					listViewPoints.Columns.Add(ChartLocalizer.GetString(ChartStringId.ArgumentMember), ColumnWidth, HorizontalAlignment.Left);
					listViewPoints.Columns.Add(view.GetValueCaption(0), ColumnWidth, HorizontalAlignment.Left);
					listViewPoints.Items.Clear();
					foreach (SeriesPoint point in collection)
						if (!point.IsEmpty)
							listViewPoints.Items.Add(new SeriesPointItem(point, explodedCollectionAccessor.Contains(point)));
					AlignListView();
					bool unboundMode = CommonUtils.IsUnboundMode(series);
					labelError.Visible = !unboundMode;
					listViewPoints.Enabled = unboundMode;
					headerPanel.Visible = unboundMode;
					loading = false;
				}
			}
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ExplodedPointsListForm));
			this.footerPanel = new DevExpress.XtraEditors.PanelControl();
			this.simpleButtonClose = new DevExpress.XtraEditors.SimpleButton();
			this.headerPanel = new DevExpress.XtraEditors.PanelControl();
			this.labelHeader = new DevExpress.XtraEditors.LabelControl();
			this.contentPanel = new DevExpress.XtraEditors.PanelControl();
			this.listViewPoints = new System.Windows.Forms.ListView();
			this.labelError = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.footerPanel)).BeginInit();
			this.footerPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.headerPanel)).BeginInit();
			this.headerPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.contentPanel)).BeginInit();
			this.contentPanel.SuspendLayout();
			this.SuspendLayout();
			this.footerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.footerPanel.Controls.Add(this.simpleButtonClose);
			resources.ApplyResources(this.footerPanel, "footerPanel");
			this.footerPanel.Name = "footerPanel";
			resources.ApplyResources(this.simpleButtonClose, "simpleButtonClose");
			this.simpleButtonClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButtonClose.Name = "simpleButtonClose";
			this.headerPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.headerPanel.Controls.Add(this.labelHeader);
			resources.ApplyResources(this.headerPanel, "headerPanel");
			this.headerPanel.Name = "headerPanel";
			resources.ApplyResources(this.labelHeader, "labelHeader");
			this.labelHeader.Name = "labelHeader";
			this.contentPanel.Controls.Add(this.listViewPoints);
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.Name = "contentPanel";
			this.listViewPoints.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewPoints.CheckBoxes = true;
			resources.ApplyResources(this.listViewPoints, "listViewPoints");
			this.listViewPoints.FullRowSelect = true;
			this.listViewPoints.GridLines = true;
			this.listViewPoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewPoints.HideSelection = false;
			this.listViewPoints.Name = "listViewPoints";
			this.listViewPoints.UseCompatibleStateImageBehavior = false;
			this.listViewPoints.View = System.Windows.Forms.View.Details;
			this.listViewPoints.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listViewPoints_ItemChecked);
			this.labelError.Appearance.BackColor = ((System.Drawing.Color)(resources.GetObject("labelError.Appearance.BackColor")));
			this.labelError.Appearance.Font = ((System.Drawing.Font)(resources.GetObject("labelError.Appearance.Font")));
			this.labelError.Appearance.ForeColor = ((System.Drawing.Color)(resources.GetObject("labelError.Appearance.ForeColor")));
			this.labelError.AutoEllipsis = true;
			resources.ApplyResources(this.labelError, "labelError");
			this.labelError.Name = "labelError";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ControlBox = false;
			this.Controls.Add(this.contentPanel);
			this.Controls.Add(this.labelError);
			this.Controls.Add(this.headerPanel);
			this.Controls.Add(this.footerPanel);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ExplodedPointsListForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Resize += new System.EventHandler(this.ExplodedPointsListForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.footerPanel)).EndInit();
			this.footerPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.headerPanel)).EndInit();
			this.headerPanel.ResumeLayout(false);
			this.headerPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.contentPanel)).EndInit();
			this.contentPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		void AlignListView() {
			if (listViewPoints.Columns.Count != 0) {
				int lastColumnIndex = listViewPoints.Columns.Count - 1;
				NativeUtils.SendMessage(listViewPoints.Handle, NativeUtils.LVM_SETCOLUMNWIDTH, 0, NativeUtils.LVSCW_AUTOSIZE_USEHEADER);
				int firstColumnWidth = listViewPoints.Columns[0].Width;
				int sum = firstColumnWidth;
				int width = (int)Math.Floor((listViewPoints.ClientSize.Width - firstColumnWidth) / (double)lastColumnIndex);
				for (int i = 1; i < lastColumnIndex; i++) {
					listViewPoints.Columns[i].Width = width;
					sum += width;
				}
				listViewPoints.Columns[lastColumnIndex].Width = listViewPoints.ClientSize.Width - sum;
			}
		}
		void ExplodedPointsListForm_Resize(object sender, EventArgs e) {
			if (collection != null) {
				listViewPoints.BeginUpdate();
				AlignListView();
				listViewPoints.EndUpdate();
			}
		}
		void listViewPoints_ItemChecked(object sender, ItemCheckedEventArgs e) {
			if (!loading) {
				SeriesPointItem item = (SeriesPointItem)e.Item;
				if (item.Exploded)
					explodedCollectionAccessor.Add(item.Point);
				else
					explodedCollectionAccessor.Remove(item.Point);
			}
		}
	}
	public interface IExpoldedPointsCollectionAccessor {
		void Add(SeriesPoint seriesPoint);
		void Remove(SeriesPoint seriesPoint);
		bool Contains(SeriesPoint seriesPoint);
	}
	public class ExpoldedPointsCollectionAccessor : IExpoldedPointsCollectionAccessor {
		readonly ExplodedSeriesPointCollection collection;
		public ExpoldedPointsCollectionAccessor(ExplodedSeriesPointCollection collection) {
			this.collection = collection;
		}
		#region IExpoldedPointsCollectionAccessor Members
		void IExpoldedPointsCollectionAccessor.Add(SeriesPoint seriesPoint) {
			collection.Add(seriesPoint);
		}
		void IExpoldedPointsCollectionAccessor.Remove(SeriesPoint seriesPoint) {
			collection.Remove(seriesPoint);
		}
		bool IExpoldedPointsCollectionAccessor.Contains(SeriesPoint seriesPoint) {
			return collection.Contains(seriesPoint);
		}
		#endregion
	}
}
