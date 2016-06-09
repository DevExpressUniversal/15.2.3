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
using DevExpress.XtraPrinting.Control;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraScheduler.Design;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraScheduler.Localization;
using System.Collections.Generic;
using System.IO;
using DevExpress.XtraScheduler.Reporting.UI;
using DevExpress.XtraReports.UI;
using DevExpress.XtraScheduler.Internal.Implementations;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.xtraTabControl")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.tpFormat")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.tpResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.labelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.cbResourcesKind")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lblResourcesKind")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.chkPrintCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.grpCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lblAvailableResource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lblCustomResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lbResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnToCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnAllToCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnAllFromCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnFromCustomCollection")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnMoveUp")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnMoveDown")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lbCustomResources")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.panelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnClose")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lbPageRangeComment")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.labelControl2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnedReportFile")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.edtEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.edtStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lblEnd")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.lblStart")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnPreview")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraScheduler.Reporting.Forms.ReportTemplateForm.btnEdit")]
#endregion
namespace DevExpress.XtraScheduler.Reporting.Forms {
	public class ReportTemplateForm : XtraForm
	{
		#region inner classes
		private class SimplePrintControl : PrintControl
		{
			private DevExpress.XtraPrinting.PrintingSystem ps;
			public SimplePrintControl() {
				SetControlVisibility(new Control[] {vScrollBar, bottomPanel}, false);
				ps = new DevExpress.XtraPrinting.PrintingSystem();
				PrintingSystem = ps;
				fMinZoom = 0.00001f;
			}
			void SetControlVisibility(Control[] controls, bool visible) {
				foreach(Control control in controls)
					control.Visible = visible;
			}
			protected override void OnHandleCreated(EventArgs e) {
				base.OnHandleCreated(e);
				CreateDocument();
				ViewWholePage();
			}
			private void CreateDocument() {
				ps.Begin();
				ps.Graph.Modifier = BrickModifier.Detail;
				EmptyBrick brick = new EmptyBrick();
				brick.Rect = new RectangleF(0, 0, 100, 100);
				ps.Graph.DrawBrick(brick);
				ps.End();
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					ps.Dispose();
				}
				base.Dispose(disposing);
			}
		}
		#endregion 
		#region Fields
		protected DevExpress.XtraTab.XtraTabControl xtraTabControl;
		protected DevExpress.XtraTab.XtraTabPage tpFormat;
		protected DevExpress.XtraTab.XtraTabPage tpResources;
		protected LabelControl labelControl1;
		protected ImageComboBoxEdit cbResourcesKind;
		protected LabelControl lblResourcesKind;
		protected CheckEdit chkPrintCustomCollection;
		protected GroupControl grpCustomCollection;
		protected LabelControl lblAvailableResource;
		protected LabelControl lblCustomResources;
		protected ListBoxControl lbResources;
		protected SimpleButton btnToCustomCollection;
		protected SimpleButton btnAllToCustomCollection;
		protected SimpleButton btnAllFromCustomCollection;
		protected SimpleButton btnFromCustomCollection;
		protected SimpleButton btnMoveUp;
		protected SimpleButton btnMoveDown;
		protected ListBoxControl lbCustomResources;
		protected PanelControl panelControl1;
		ReportTemplateForm.SimplePrintControl pc;
		protected DevExpress.XtraEditors.SimpleButton btnClose;
		protected DevExpress.XtraEditors.LabelControl lbPageRangeComment;
		protected LabelControl labelControl2;
		protected ButtonEdit btnedReportFile;
		protected DateEdit edtEnd;
		protected DateEdit edtStart;
		protected LabelControl lblEnd;
		protected LabelControl lblStart;
		protected DevExpress.XtraEditors.SimpleButton btnPreview;
		protected SimpleButton btnEdit;
		private System.ComponentModel.IContainer components = null;
		XtraSchedulerReport report;
		XtraSchedulerReport previewReport;
		SchedulerControlPrintAdapter printAdapter;
		string reportFileName = string.Empty;
		ResourceBaseCollection printResources = new ResourceBaseCollection();		
		#endregion
		#region Properties
		public SchedulerControlPrintAdapter PrintAdapter { get { return printAdapter; } }
		public ResourceBaseCollection PrintResources {get {return printResources; } }		
		public DateTime EndDate { get { return edtEnd.DateTime.AddDays(1); } set { edtEnd.DateTime = value.AddDays(-1); } }
		public DateTime StartDate { get { return edtStart.DateTime; } set { edtStart.DateTime = value; } }		
		public string ReportFileName { get { return reportFileName; } }
		#endregion
		public ReportTemplateForm(SchedulerControlPrintAdapter printAdapter) {
			this.printAdapter = printAdapter;
			InitializeComponent();
			this.edtStart.DateTime = DateTime.Today.AddDays(-2);
			this.edtEnd.DateTime = DateTime.Today.AddDays(7);
			btnPreview.Enabled = false;
			btnEdit.Enabled = false;
			SubscribeIntervalControlsEvents();
			PopulateResourceKindItems();
			PopulateAvailableResources();						
			UpdateControlsVisibility();
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				if (components != null)
					components.Dispose();
				if (printAdapter != null)
					UnsubscribePrintAdapterEvents(printAdapter);
				if (report != null) {
					DisposeReport(report);
					report = null;
				}
				if (previewReport != null) {
					DisposeReport(previewReport);
					previewReport = null;
				}
			}
			base.Dispose(disposing);
		}
		void DisposeReport(XtraSchedulerReport report) {
			UnsubscribePrintAdapterEvents(report.SchedulerAdapter);
			report.Dispose();
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReportTemplateForm));
			this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
			this.lbPageRangeComment = new DevExpress.XtraEditors.LabelControl();
			this.btnClose = new DevExpress.XtraEditors.SimpleButton();
			this.xtraTabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tpFormat = new DevExpress.XtraTab.XtraTabPage();
			this.edtEnd = new DevExpress.XtraEditors.DateEdit();
			this.edtStart = new DevExpress.XtraEditors.DateEdit();
			this.lblEnd = new DevExpress.XtraEditors.LabelControl();
			this.lblStart = new DevExpress.XtraEditors.LabelControl();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.pc = new SimplePrintControl();
			this.tpResources = new DevExpress.XtraTab.XtraTabPage();
			this.cbResourcesKind = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.lblResourcesKind = new DevExpress.XtraEditors.LabelControl();
			this.chkPrintCustomCollection = new DevExpress.XtraEditors.CheckEdit();
			this.grpCustomCollection = new DevExpress.XtraEditors.GroupControl();
			this.lblAvailableResource = new DevExpress.XtraEditors.LabelControl();
			this.lblCustomResources = new DevExpress.XtraEditors.LabelControl();
			this.lbResources = new DevExpress.XtraEditors.ListBoxControl();
			this.btnToCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnAllToCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnAllFromCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnFromCustomCollection = new DevExpress.XtraEditors.SimpleButton();
			this.btnMoveUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnMoveDown = new DevExpress.XtraEditors.SimpleButton();
			this.lbCustomResources = new DevExpress.XtraEditors.ListBoxControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.btnedReportFile = new DevExpress.XtraEditors.ButtonEdit();
			this.btnEdit = new DevExpress.XtraEditors.SimpleButton();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).BeginInit();
			this.xtraTabControl.SuspendLayout();
			this.tpFormat.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties.VistaTimeProperties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			this.tpResources.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbResourcesKind.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintCustomCollection.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpCustomCollection)).BeginInit();
			this.grpCustomCollection.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbResources)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCustomResources)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.btnedReportFile.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnPreview, "btnPreview");
			this.btnPreview.Name = "btnPreview";
			this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
			resources.ApplyResources(this.lbPageRangeComment, "lbPageRangeComment");
			this.lbPageRangeComment.Name = "lbPageRangeComment";
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
			resources.ApplyResources(this.btnClose, "btnClose");
			this.btnClose.Name = "btnClose";
			resources.ApplyResources(this.xtraTabControl, "xtraTabControl");
			this.xtraTabControl.Name = "xtraTabControl";
			this.xtraTabControl.SelectedTabPage = this.tpFormat;
			this.xtraTabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tpFormat,
			this.tpResources});
			this.tpFormat.Controls.Add(this.edtEnd);
			this.tpFormat.Controls.Add(this.edtStart);
			this.tpFormat.Controls.Add(this.lblEnd);
			this.tpFormat.Controls.Add(this.lblStart);
			this.tpFormat.Controls.Add(this.panelControl1);
			this.tpFormat.Name = "tpFormat";
			resources.ApplyResources(this.tpFormat, "tpFormat");
			this.tpFormat.TooltipTitle = null;
			resources.ApplyResources(this.edtEnd, "edtEnd");
			this.edtEnd.Name = "edtEnd";
			this.edtEnd.Properties.AccessibleName = resources.GetString("edtEnd.Properties.AccessibleName");
			this.edtEnd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtEnd.Properties.Buttons"))))});
			this.edtEnd.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.edtStart, "edtStart");
			this.edtStart.Name = "edtStart";
			this.edtStart.Properties.AccessibleName = resources.GetString("edtStart.Properties.AccessibleName");
			this.edtStart.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("edtStart.Properties.Buttons"))))});
			this.edtStart.Properties.VistaTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			resources.ApplyResources(this.lblEnd, "lblEnd");
			this.lblEnd.Name = "lblEnd";
			resources.ApplyResources(this.lblStart, "lblStart");
			this.lblStart.Name = "lblStart";
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.panelControl1.Controls.Add(this.pc);
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.Name = "panelControl1";
			resources.ApplyResources(this.pc, "pc");
			this.pc.Name = "pc";
			this.pc.ShowPageMargins = false;
			this.pc.TabStop = false;
			this.pc.Zoom = 0.2301799F;
			this.tpResources.Controls.Add(this.cbResourcesKind);
			this.tpResources.Controls.Add(this.lblResourcesKind);
			this.tpResources.Controls.Add(this.chkPrintCustomCollection);
			this.tpResources.Controls.Add(this.grpCustomCollection);
			this.tpResources.Name = "tpResources";
			resources.ApplyResources(this.tpResources, "tpResources");
			this.tpResources.TooltipTitle = null;
			resources.ApplyResources(this.cbResourcesKind, "cbResourcesKind");
			this.cbResourcesKind.Name = "cbResourcesKind";
			this.cbResourcesKind.Properties.AccessibleName = resources.GetString("cbResourcesKind.Properties.AccessibleName");
			this.cbResourcesKind.Properties.AccessibleRole = System.Windows.Forms.AccessibleRole.ComboBox;
			this.cbResourcesKind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbResourcesKind.Properties.Buttons"))))});
			resources.ApplyResources(this.lblResourcesKind, "lblResourcesKind");
			this.lblResourcesKind.Name = "lblResourcesKind";
			resources.ApplyResources(this.chkPrintCustomCollection, "chkPrintCustomCollection");
			this.chkPrintCustomCollection.Name = "chkPrintCustomCollection";
			this.chkPrintCustomCollection.Properties.AutoWidth = true;
			this.chkPrintCustomCollection.Properties.Caption = resources.GetString("chkPrintCustomCollection.Properties.Caption");
			this.chkPrintCustomCollection.CheckedChanged += new System.EventHandler(this.chkPrintCustomCollection_CheckedChanged);
			this.grpCustomCollection.AccessibleRole = System.Windows.Forms.AccessibleRole.Grouping;
			this.grpCustomCollection.Controls.Add(this.lblAvailableResource);
			this.grpCustomCollection.Controls.Add(this.lblCustomResources);
			this.grpCustomCollection.Controls.Add(this.lbResources);
			this.grpCustomCollection.Controls.Add(this.btnToCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnAllToCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnAllFromCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnFromCustomCollection);
			this.grpCustomCollection.Controls.Add(this.btnMoveUp);
			this.grpCustomCollection.Controls.Add(this.btnMoveDown);
			this.grpCustomCollection.Controls.Add(this.lbCustomResources);
			resources.ApplyResources(this.grpCustomCollection, "grpCustomCollection");
			this.grpCustomCollection.Name = "grpCustomCollection";
			resources.ApplyResources(this.lblAvailableResource, "lblAvailableResource");
			this.lblAvailableResource.Name = "lblAvailableResource";
			resources.ApplyResources(this.lblCustomResources, "lblCustomResources");
			this.lblCustomResources.Name = "lblCustomResources";
			resources.ApplyResources(this.lbResources, "lbResources");
			this.lbResources.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbResources.Name = "lbResources";
			this.lbResources.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			resources.ApplyResources(this.btnToCustomCollection, "btnToCustomCollection");
			this.btnToCustomCollection.Name = "btnToCustomCollection";
			this.btnToCustomCollection.Click += new System.EventHandler(this.btnToCustomCollection_Click);
			resources.ApplyResources(this.btnAllToCustomCollection, "btnAllToCustomCollection");
			this.btnAllToCustomCollection.Name = "btnAllToCustomCollection";
			this.btnAllToCustomCollection.Click += new System.EventHandler(this.btnAllToCustomCollection_Click);
			resources.ApplyResources(this.btnAllFromCustomCollection, "btnAllFromCustomCollection");
			this.btnAllFromCustomCollection.Name = "btnAllFromCustomCollection";
			this.btnAllFromCustomCollection.Click += new System.EventHandler(this.btnAllFromCustomCollection_Click);
			resources.ApplyResources(this.btnFromCustomCollection, "btnFromCustomCollection");
			this.btnFromCustomCollection.Name = "btnFromCustomCollection";
			this.btnFromCustomCollection.Click += new System.EventHandler(this.btnFromCustomCollection_Click);
			resources.ApplyResources(this.btnMoveUp, "btnMoveUp");
			this.btnMoveUp.Name = "btnMoveUp";
			this.btnMoveUp.Click += new System.EventHandler(this.btnMoveUp_Click);
			resources.ApplyResources(this.btnMoveDown, "btnMoveDown");
			this.btnMoveDown.Name = "btnMoveDown";
			this.btnMoveDown.Click += new System.EventHandler(this.btnMoveDown_Click);
			resources.ApplyResources(this.lbCustomResources, "lbCustomResources");
			this.lbCustomResources.AccessibleRole = System.Windows.Forms.AccessibleRole.List;
			this.lbCustomResources.Name = "lbCustomResources";
			this.lbCustomResources.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			resources.ApplyResources(this.btnedReportFile, "btnedReportFile");
			this.btnedReportFile.Name = "btnedReportFile";
			this.btnedReportFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.btnedReportFile.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.btnedReportFile.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit1_Properties_ButtonClick);
			resources.ApplyResources(this.btnEdit, "btnEdit");
			this.btnEdit.Name = "btnEdit";
			this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.btnEdit);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.btnedReportFile);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.btnPreview);
			this.Controls.Add(this.xtraTabControl);
			this.Controls.Add(this.btnClose);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ReportTemplateForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl)).EndInit();
			this.xtraTabControl.ResumeLayout(false);
			this.tpFormat.ResumeLayout(false);
			this.tpFormat.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtEnd.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties.VistaTimeProperties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.edtStart.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.tpResources.ResumeLayout(false);
			this.tpResources.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbResourcesKind.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkPrintCustomCollection.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpCustomCollection)).EndInit();
			this.grpCustomCollection.ResumeLayout(false);
			this.grpCustomCollection.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbResources)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbCustomResources)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.btnedReportFile.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private void SubscribePrintAdapterEvents(SchedulerPrintAdapter adapter) {
			if (adapter != null)
				adapter.ValidateResources += new ResourcesValidationEventHandler(PrintAdapter_ValidateResources);
		}
		private void UnsubscribePrintAdapterEvents(SchedulerPrintAdapter adapter) {
			if (adapter != null)
				adapter.ValidateResources -= new ResourcesValidationEventHandler(PrintAdapter_ValidateResources);
		}
		private void SubscribeIntervalControlsEvents() {
			this.edtStart.EditValueChanged += new EventHandler(StartEditValueChanged);
			this.edtEnd.EditValueChanged += new EventHandler(EndEditValueChanged);
		}
		private void UnsubscribeIntervalControlsEvents() {
			this.edtStart.EditValueChanged -= new EventHandler(StartEditValueChanged);
			this.edtEnd.EditValueChanged -= new EventHandler(EndEditValueChanged);
		}
		void StartEditValueChanged(object sender, EventArgs e) {
			UnsubscribeIntervalControlsEvents();
			if (!IsValidInterval(StartDate, EndDate))
				edtEnd.EditValue = StartDate;			
			SubscribeIntervalControlsEvents();			
		}
		protected internal virtual bool IsValidInterval(DateTime start, DateTime end) {
			return start <= end;
		}
		void EndEditValueChanged(object sender, EventArgs e) {
			UnsubscribeIntervalControlsEvents();
			if (!IsValidInterval(StartDate, EndDate))
				edtStart.EditValue = EndDate.AddDays(-1);			
			SubscribeIntervalControlsEvents();			
		}
		private void PrintAdapter_ValidateResources(object sender, ResourcesValidationEventArgs e) {
			e.Resources.Clear();
			e.Resources.AddRange(PrintResources);
		}
		private void PopulateResourceKindItems() {
			cbResourcesKind.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_AllResources), ResourcesKind.All));
			cbResourcesKind.Properties.Items.Add(new ImageComboBoxItem(SchedulerLocalizer.GetString(SchedulerStringId.Caption_OnScreenResources), ResourcesKind.OnScreen));
			cbResourcesKind.SelectedIndex = 0;
		}
		private void buttonEdit1_Properties_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			try {
				dlg.CheckPathExists = true;
				dlg.Filter = "Report template files (*.schrepx)|*.schrepx|All files (*.*)|*.*";
				string folderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				dlg.InitialDirectory = new DirectoryInfo(folderPath).FullName;
				DialogResult result = dlg.ShowDialog();
				if (result != DialogResult.OK)
					return;
				reportFileName = dlg.FileName;
				btnedReportFile.Text = reportFileName;
				CreateReports();
				UpdatePreview();
				bool enabled = !String.IsNullOrEmpty(reportFileName);
				btnPreview.Enabled = enabled;
				btnEdit.Enabled = enabled;
			}
			catch (Exception ex) {
				XtraMessageBox.Show(ex.Message, Application.ProductName);
			}
			finally {
				dlg.Dispose();
			}
		}
		private void UpdatePreview() {
			if (previewReport == null)
				return;
			pc.PrintingSystem = previewReport.PrintingSystem;
			previewReport.ReportPrintOptions.DetailCount = 1;
			UpdateReport(previewReport);
		}
		private void btnPreview_Click(object sender, EventArgs e) {
			UpdateReport(report);
			if (report != null)
				report.ShowPreview();
		}
		private void btnEdit_Click(object sender, EventArgs e) {
			UpdateReport(report);
			if (report != null) {
				new SchedulerReportDesignTool(report).ShowDesignerDialog();
			}
		}		
		private void UpdateReport(XtraSchedulerReport report) {
			if (report == null)
				return;
			report.SchedulerAdapter.TimeInterval = new TimeInterval(StartDate, EndDate);
			UpdatePrintResources();
			report.PrintingSystem.ClearContent();
			report.CreateDocument(true);
		}
		private void UpdatePrintResources() {
			PrintResources.Clear();
			ResourceBaseCollection resources = GetPrintResources();
			PrintResources.AddRange(resources);
		}
		private ResourceBaseCollection GetPrintResources() {
			if (chkPrintCustomCollection.Checked)
				return GetCustomResources();
			ResourcesKind resourcesKind = (ResourcesKind)cbResourcesKind.EditValue; 
			switch (resourcesKind) {
				case ResourcesKind.All:
					return GetAvailableResources();
				case ResourcesKind.OnScreen:
					return GetOnScreenResources();				
				default:
					return new ResourceBaseCollection();
			}
		}
		private ResourceBaseCollection GetAvailableResources() {
			return PrintAdapter.SchedulerControl.DataStorage.Resources.Items;
		}
		private ResourceBaseCollection GetOnScreenResources() {
			return PrintAdapter.SchedulerControl.ActiveView.GetResources();
		}
		private ResourceBaseCollection GetCustomResources() {
			ResourceBaseCollection result = new ResourceBaseCollection();
			int count = lbCustomResources.ItemCount;
			for (int i = 0; i < count; i++) {
				ObjectWrapper objectWrapper = (ObjectWrapper)lbCustomResources.Items[i];
				result.Add((ResourceBase)objectWrapper.Object);	
			}
			return result;
		}
		private void CreateReports() {
			CreateReport();
			CreatePreviewReport();
		}
		private void CreateReport() {
			if (report != null) {
				DisposeReport(report);
			}
			this.report = CreateReportCore();
		}
		private void CreatePreviewReport() {
			if (previewReport != null) {
				DisposeReport(previewReport);
			}
			this.previewReport = CreateReportCore();
		}
		private XtraSchedulerReport CreateReportCore() {
			XtraSchedulerReport report = new XtraSchedulerReport();
			report.LoadLayout(reportFileName);
			if(report.SchedulerAdapter != null)
				report.SchedulerAdapter.SetSourceObject(PrintAdapter.SchedulerControl); else
				report.SchedulerAdapter = PrintAdapter;
			report.SchedulerAdapter.EnableSmartSync = reportFileName.ToLower().Contains("trifold");
			SubscribePrintAdapterEvents(report.SchedulerAdapter);
			report.PrintColorSchema = PrintColorSchema.FullColor;
			return report;
		}
		private void chkPrintCustomCollection_CheckedChanged(object sender, EventArgs e) {
			UpdateControlsVisibility();
		}
		private void UpdateControlsVisibility() {
			bool isCustomResources = chkPrintCustomCollection.Checked;
			this.lbCustomResources.Enabled = isCustomResources;
			this.lbResources.Enabled = isCustomResources;
			this.cbResourcesKind.Enabled = !isCustomResources;
			UpdateResourceButtonsVisibility(isCustomResources);
		}
		private void UpdateResourceButtonsVisibility(bool isCustomResources) {
			this.btnAllToCustomCollection.Enabled = isCustomResources && lbResources.ItemCount > 0;
			this.btnToCustomCollection.Enabled = isCustomResources && lbResources.ItemCount > 0;
			this.btnAllFromCustomCollection.Enabled = isCustomResources && lbCustomResources.ItemCount > 0;
			this.btnFromCustomCollection.Enabled = isCustomResources && lbCustomResources.ItemCount > 0;
			this.btnMoveDown.Enabled = isCustomResources;
			this.btnMoveUp.Enabled = isCustomResources;
		}
		private void PopulateAvailableResources() {
			lbResources.Items.Clear();
			ResourceBaseCollection resources = GetAvailableResources();
			int count = resources.Count;
			for (int i = 0; i < count; i++)
				lbResources.Items.Add(new ObjectWrapper(resources[i], resources[i].Caption));
		}
		private void btnAllToCustomCollection_Click(object sender, EventArgs e) {
			MoveAllItems(lbResources, lbCustomResources);
		}
		private void btnAllFromCustomCollection_Click(object sender, EventArgs e) {
			MoveAllItems(lbCustomResources, lbResources);			
		}
		private void btnToCustomCollection_Click(object sender, EventArgs e) {
			MoveSelectedItems(lbResources, lbCustomResources);
		}
		private void btnFromCustomCollection_Click(object sender, EventArgs e) {
			MoveSelectedItems(lbCustomResources, lbResources);
		}
		private void btnMoveUp_Click(object sender, EventArgs e) {
			MoveSelectedItems(lbCustomResources, true);
		}
		private void btnMoveDown_Click(object sender, EventArgs e) {
			MoveSelectedItems(lbCustomResources, false);
		}
		private void MoveAllItems(ListBoxControl source, ListBoxControl target) {
			int count = source.ItemCount;
			for (int i = 0; i < count; i++) 			
			  target.Items.Add(source.Items[i]);			
			source.Items.Clear();
			UpdateResourceButtonsVisibility(true);
		}
		private void MoveSelectedItems(ListBoxControl source, ListBoxControl target) {
			List<object> selectedItems = new List<object>();
			int count = source.SelectedItems.Count;
			for (int i = 0; i < count; i++) {
				object item = source.SelectedItems[i];
				target.Items.Add(item);
				selectedItems.Add(item);
			}
			for (int i = 0; i < count; i++)
				source.Items.Remove(selectedItems[i]);
			UpdateResourceButtonsVisibility(true);
		}
		private void MoveSelectedItems(ListBoxControl listBox, bool moveUp) {
			if (!CanMoveItems(listBox, moveUp))
				return;			
			List<int> selectedIndices = GetSelectedIndixes(listBox);
			List<object> selectedItems = GetSelectedItems(listBox);
			if (moveUp)
				MoveSelectedItemsUp(listBox, selectedIndices);
			else
				MoveSelectedItemsDown(listBox, selectedIndices);
			UpdateSelection(listBox, selectedItems);
			UpdateResourceButtonsVisibility(true);	 
		}
		private void MoveSelectedItemsUp(ListBoxControl listBox, List<int> selectedIndices) {
			int count = selectedIndices.Count;
			for (int i = 0; i < count; i++) {
				int index = selectedIndices[i];
				object item = listBox.Items[index];
				listBox.Items.RemoveAt(index);
				listBox.Items.Insert(index - 1, item);
			}
		}
		private void MoveSelectedItemsDown(ListBoxControl listBox, List<int> selectedIndices) {
			int count = selectedIndices.Count;
			for (int i = count - 1; i >= 0; i--) {
				int index = selectedIndices[i];
				object item = listBox.Items[index];
				listBox.Items.RemoveAt(index);
				listBox.Items.Insert(index + 1, item);
			}
		}
		private void UpdateSelection(ListBoxControl listBox, List<object> selectedItems) {
			int count = listBox.Items.Count;
			for (int i = 0; i < count; i++) {
				bool selected = selectedItems.Contains(listBox.Items[i]);
				listBox.SetSelected(i, selected);
			}
		}
		private List<int> GetSelectedIndixes(ListBoxControl listBox) {
			List<int> result = new List<int>();
			int count = listBox.SelectedIndices.Count;
			for (int i = 0; i < count; i++) 
				result.Add(listBox.SelectedIndices[i]);
			return result;
		}
		private List<object> GetSelectedItems(ListBoxControl listBox) {
			List<object> result = new List<object>();
			int count = listBox.SelectedItems.Count;
			for (int i = 0; i < count; i++)
				result.Add(listBox.SelectedItems[i]);
			return result;
		}
		private bool CanMoveItems(ListBoxControl listBox, bool moveUp) {
			BaseListBoxControl.SelectedIndexCollection indices = listBox.SelectedIndices;
			int selectedIndexesCount = indices.Count;
			if (selectedIndexesCount == 0)
				return false;
			return moveUp ? indices[0] > 0 : indices[selectedIndexesCount - 1] < listBox.ItemCount - 1;
		}
	}
	public enum ResourcesKind { All, OnScreen }
}
