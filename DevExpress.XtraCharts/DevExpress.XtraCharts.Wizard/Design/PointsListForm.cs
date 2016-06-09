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
using System.Windows.Forms;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraEditors;
namespace DevExpress.XtraCharts.Design {
	public class PointsListForm : XtraForm {
		class SeriesPointItem : ListViewItem {
			readonly SeriesPoint point;
			public SeriesPoint Point { get { return point; } }
			public SeriesPointItem(SeriesPoint point) {
				this.point = point;
				Text = point.Argument;
				for (int i = 0; i < point.Values.Length; i++)
					SubItems.Add(point.GetValueString(i));
			}
		}
		readonly SeriesPoint parentPoint;
		ListView listViewPoints;
		PanelControl panelControlPoints;
		LabelControl labelControlHeader;
		SimpleButton simpleButtonOk;
		SimpleButton simpleButtonCancel;
		protected virtual bool DelayedUpdate { get { return false; } }
		PointsListForm() {
			InitializeComponent();
		}
		public PointsListForm(SeriesPoint parentPoint) : this() {
			this.parentPoint = parentPoint;
			if(!DelayedUpdate)
				UpdateForm();
		}
		protected void UpdateForm() {
			object seriesObject = GetSeriesObject();
			int width = 10;
			listViewPoints.Columns.Add(ChartLocalizer.GetString(ChartStringId.ArgumentMember), width, HorizontalAlignment.Left);
			SeriesViewBase view = GetSeriesView(seriesObject);
			for(int i = 0; i < ((IViewArgumentValueOptions)view).PointDimension; i++)
				listViewPoints.Columns.Add(view.GetValueCaption(i), width, HorizontalAlignment.Left);
			foreach(object point in EnumeratePoints(seriesObject))
				if(!IsParentPoint(point) && IsPointInRelations(point))
					listViewPoints.Items.Add(CreateItem(point));
			AlignListView(listViewPoints);
			UpdateControls();
		}
		protected virtual ListViewItem CreateItem(object point) {
			return new SeriesPointItem((SeriesPoint)point);
		}
		protected virtual IEnumerable<object> EnumeratePoints(object seriesObject) {
			foreach(SeriesPoint point in ((Series)seriesObject).Points)
				yield return point;
		}
		protected virtual bool IsPointInRelations(object point) {
			return parentPoint.Relations.GetByChildSeriesPoint((SeriesPoint)point) == null;
		}
		protected virtual bool IsParentPoint(object point) {
			return ((SeriesPoint)point).SeriesPointID == parentPoint.SeriesPointID;
		}
		protected virtual SeriesViewBase GetSeriesView(object seriesObject) {
			return ((Series)seriesObject).View;
		}
		protected virtual object GetSeriesObject() {
			return ((IOwnedElement)parentPoint).Owner as Series;
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PointsListForm));
			this.listViewPoints = new System.Windows.Forms.ListView();
			this.panelControlPoints = new DevExpress.XtraEditors.PanelControl();
			this.labelControlHeader = new DevExpress.XtraEditors.LabelControl();
			this.simpleButtonOk = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButtonCancel = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.panelControlPoints)).BeginInit();
			this.panelControlPoints.SuspendLayout();
			this.SuspendLayout();
			this.listViewPoints.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this.listViewPoints, "listViewPoints");
			this.listViewPoints.FullRowSelect = true;
			this.listViewPoints.GridLines = true;
			this.listViewPoints.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listViewPoints.HideSelection = false;
			this.listViewPoints.Name = "listViewPoints";
			this.listViewPoints.UseCompatibleStateImageBehavior = false;
			this.listViewPoints.View = System.Windows.Forms.View.Details;
			this.listViewPoints.DoubleClick += new System.EventHandler(this.listViewPoints_DoubleClick);
			this.listViewPoints.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listViewPoints_KeyDown);
			resources.ApplyResources(this.panelControlPoints, "panelControlPoints");
			this.panelControlPoints.Controls.Add(this.listViewPoints);
			this.panelControlPoints.Name = "panelControlPoints";
			this.panelControlPoints.TabStop = true;
			this.labelControlHeader.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			resources.ApplyResources(this.labelControlHeader, "labelControlHeader");
			this.labelControlHeader.Name = "labelControlHeader";
			resources.ApplyResources(this.simpleButtonOk, "simpleButtonOk");
			this.simpleButtonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.simpleButtonOk.Name = "simpleButtonOk";
			resources.ApplyResources(this.simpleButtonCancel, "simpleButtonCancel");
			this.simpleButtonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.simpleButtonCancel.Name = "simpleButtonCancel";
			resources.ApplyResources(this, "$this");
			this.ControlBox = false;
			this.Controls.Add(this.simpleButtonCancel);
			this.Controls.Add(this.simpleButtonOk);
			this.Controls.Add(this.labelControlHeader);
			this.Controls.Add(this.panelControlPoints);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PointsListForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Resize += new System.EventHandler(this.PointsListForm_Resize);
			((System.ComponentModel.ISupportInitialize)(this.panelControlPoints)).EndInit();
			this.panelControlPoints.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		void AlignListView(ListView listView) {
			if (listView.Columns.Count > 0) {
				if (listView.Columns.Count == 1)
					listView.Columns[0].Width = listView.Width;
				else {
					int sum = 0;
					NativeUtils.SendMessage(listView.Handle, NativeUtils.LVM_SETCOLUMNWIDTH, 0, NativeUtils.LVSCW_AUTOSIZE_USEHEADER);
					sum += listView.Columns[0].Width;
					int width = (int)Math.Floor((listView.ClientSize.Width - listView.Columns[0].Width) / (double)(listView.Columns.Count - 1));
					for (int i = 1; i < listView.Columns.Count - 1; i++) {
						listView.Columns[i].Width = width;
						sum += width;
					}
					listView.Columns[listView.Columns.Count - 1].Width = listView.ClientSize.Width - sum;
				}
			}
		}
		void PointsListForm_Resize(object sender, EventArgs e) {
			listViewPoints.BeginUpdate();
			AlignListView(listViewPoints);
			listViewPoints.EndUpdate();
		}
		void listViewPoints_DoubleClick(object sender, EventArgs e) {
			if (listViewPoints.SelectedItems.Count > 0) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}
		void listViewPoints_KeyDown(object sender, KeyEventArgs e) {
			if (e.KeyData == Keys.Enter) {
				DialogResult = DialogResult.OK;
				Close();
			}
		}
		void UpdateControls() {
			simpleButtonOk.Enabled = listViewPoints.Items.Count > 0;
			if (listViewPoints.Items.Count > 0)
				listViewPoints.Items[0].Selected = true;
		}
		protected virtual object GetPoint(ListViewItem item) {
			return ((SeriesPointItem)item).Point;
		}
		public object[] GetSelectedPoints() {
			object[] points = new object[listViewPoints.SelectedItems.Count];
			for(int i = 0; i < points.Length; i++)
				points[i] = GetPoint(listViewPoints.SelectedItems[i]);
			return points;
		}
	}
}
