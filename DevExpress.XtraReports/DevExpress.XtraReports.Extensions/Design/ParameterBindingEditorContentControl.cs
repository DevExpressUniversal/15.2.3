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
using System.Linq;
using System.Text;
using DevExpress.Utils.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTreeList.Nodes;
using System.Collections;
using System.IO;
namespace DevExpress.XtraReports.Design {
	class ParameterBindingEditorContentControl : CollectionEditorContentControl {
		private DevExpress.XtraEditors.SimpleButton buttonSync;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItemSync;
		Tuple<XRSubreport, XtraReport, bool> reportContainer;
		public ParameterBindingEditorContentControl() {
			InitializeComponent();
			UpdateGroupButtonsLayout();
		}
		public ParameterBindingEditorContentControl(IServiceProvider provider, CollectionEditor collectionEditor)
			: base(provider, collectionEditor) {
			InitializeComponent();
			UpdateGroupButtonsLayout();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeReportSource();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ParameterBindingEditorContentControl));
			this.buttonSync = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlItemSync = new XtraLayout.LayoutControlItem();
			this.SuspendLayout();
			resources.ApplyResources(this.buttonSync, "buttonSync");
			this.buttonSync.Name = "buttonSync";
			this.buttonSync.Click += new System.EventHandler(this.buttonSync_Click);
			resources.ApplyResources(this.layoutControlItemSync, "layoutControlItemSync");
			this.layoutControlItemSync.Control = this.buttonSync;
			this.layoutControlItemSync.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItemSync.Name = "layoutControlItem4";
			this.layoutControlItemSync.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItemSync.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemSync.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.Name = "ParameterBindingEditorContentControl";
			this.ResumeLayout(false);
		}
		void UpdateGroupButtonsLayout() {
			(this.grpButtons.Items as IList).Remove(layoutControlItemUp);
			(this.grpButtons.Items as IList).Remove(layoutControlItemDown);
			this.layoutControl1.Controls.Remove(buttonUp);
			this.layoutControl1.Controls.Remove(buttonDown);
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions[0].SizeType = System.Windows.Forms.SizeType.AutoSize;
			(this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions as IList).Clear();
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
				new DevExpress.XtraLayout.ColumnDefinition() { SizeType = System.Windows.Forms.SizeType.Absolute, Width = 80},
				new DevExpress.XtraLayout.ColumnDefinition() { SizeType = System.Windows.Forms.SizeType.Absolute, Width = 4},
				new DevExpress.XtraLayout.ColumnDefinition() { SizeType = System.Windows.Forms.SizeType.Absolute, Width = 80},
				new DevExpress.XtraLayout.ColumnDefinition() { SizeType = System.Windows.Forms.SizeType.Absolute, Width = 4},
				new DevExpress.XtraLayout.ColumnDefinition() { SizeType = System.Windows.Forms.SizeType.Absolute, Width = 80}
			});
			this.layoutControl1.Controls.Add(buttonSync);
			(this.grpButtons.Items as IList).Add(layoutControlItemSync);
			layoutControlItemSync.OptionsTableLayoutItem.ColumnIndex = 0;
			layoutControlItemAdd.OptionsTableLayoutItem.ColumnIndex = 2;
			layoutControlItemRemove.OptionsTableLayoutItem.ColumnIndex = 4;
		}
		protected override void InitializeButtonsLayout(){
			InitializeButtonLayout(buttonSync, 0, grpButtons);
			InitializeButtonLayout(buttonAdd, 2, grpButtons);
			InitializeButtonLayout(buttonRemove, 4, grpButtons);
		}
		protected override void OnPropertyGridCellChanged(object sender, XtraVerticalGrid.Events.CellValueChangedEventArgs e) {
			base.OnPropertyGridCellChanged(sender, e);
			UpdateSyncButton();
		}
		protected override void UpdateUI() {
			base.UpdateUI();
			UpdateSyncButton();
		}
		 void UpdateSyncButton() {
			XRSubreport subreport = Context.Instance as XRSubreport;
			XtraReport reportSource = GetReportSource(subreport);
			if(reportSource == null) {
				buttonSync.Enabled = false;
				return;
			}
			ParameterBindingHelper helper = new ParameterBindingHelper(reportSource.Parameters, subreport.ParameterBindings);
			buttonSync.Enabled = helper.GetOddBindings().Length > 0 || helper.GetMissedBindings().Length > 0;
		}
		void buttonSync_Click(object sender, EventArgs e) {
			XRSubreport subreport = Context.Instance as XRSubreport;
			XtraReport reportSource = GetReportSource(subreport);
			if(reportSource == null) return;
			ParameterBindingHelper helper = new ParameterBindingHelper(reportSource.Parameters, subreport.ParameterBindings);
			foreach(ParameterBinding item in helper.GetOddBindings()) {
				var node = GetNode(item);
				if(node != null) RemoveItem(node);
			}
			foreach(ParameterBinding item in helper.GetMissedBindings()) {
				AddInstance(item);
			}
		}
		TreeListNode GetNode(object item) {
			return tv.Nodes.First<TreeListNode>(n => ReferenceEquals(item, n.GetItem()));
		}
		XtraReport GetReportSource(XRSubreport subreport) {
			UpdateReportSource(subreport);
			return reportContainer != null ? reportContainer.Item2 : null;
		}
		void UpdateReportSource(XRSubreport subreport) {
			if(reportContainer != null && ReferenceEquals(reportContainer.Item1, subreport))
				return;
			DisposeReportSource();
			if(subreport != null)
				reportContainer = CreateReportContainer(subreport);
		}
		Tuple<XRSubreport, XtraReport, bool> CreateReportContainer(XRSubreport subreport) {
			if(subreport.ReportSource != null)
				return new Tuple<XRSubreport, XtraReport, bool>(subreport, subreport.ReportSource, false);
			if(string.IsNullOrEmpty(subreport.ReportSourceUrl))
				return null;
			byte[] layout = ReportStorageService.GetData(subreport.ReportSourceUrl);
			if(layout == null)
				layout = ReportStorageService.GetData(subreport.Id.ToString());
			if(layout == null) return null;
			Stream stream = new MemoryStream(layout);
			try {
				XtraReport report = XtraReport.FromStream(stream, true);
				return new Tuple<XRSubreport, XtraReport, bool>(subreport, report, true);
			} catch {
				return null;
			} finally {
				stream.Dispose();
			}
		}
		void DisposeReportSource() {
			if(reportContainer != null && reportContainer.Item3)
				((XtraReport)reportContainer.Item2).Dispose();
			reportContainer = null;
		}
	}
}
