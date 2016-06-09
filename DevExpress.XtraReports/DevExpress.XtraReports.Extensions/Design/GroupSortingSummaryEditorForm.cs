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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using System.ComponentModel.Design;
using DevExpress.Data.Browsing;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.Native.Summary;
using DevExpress.XtraReports.Native;
namespace DevExpress.XtraReports.Design {
	public partial class GroupSortingSummaryEditorForm : SummaryEditorFormBase {
		private System.ComponentModel.IContainer components = null;
		private DevExpress.XtraEditors.LabelControl lbSortOrder;
		private DevExpress.XtraEditors.LookUpEdit lkeSortOrder;
		GroupHeaderBand band;
		ReportDesigner reportDesigner;
		private DevExpress.XtraEditors.CheckEdit chkEnabled;
		string fieldName;
		IDesignerHost designerHost;
		public GroupSortingSummaryEditorForm(GroupHeaderBand band, IServiceContainer serviceContainer)
			: base(band.SortingSummary.IgnoreNullValues, serviceContainer) {
			InitializeComponent();
			this.band = band;
			designerHost = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
			reportDesigner = designerHost.GetDesigner(designerHost.RootComponent) as ReportDesigner;
			fieldName = band.SortingSummary.FieldName;
			InitFunctionCombo();
			InitSortOrderCombo();
			chkEnabled.Checked = band.SortingSummary.Enabled;
			UpdateFieldNameComboText();
			UpdatePreview();
			UpdateControls();
		}
		public GroupSortingSummaryEditorForm() {
			InitializeComponent();
		}
		protected new PopupFieldNamePicker BindingPicker { get { return (PopupFieldNamePicker)base.BindingPicker; } }
		public XRColumnSortOrder SortOrder { get { return (XRColumnSortOrder)lkeSortOrder.EditValue; } }
		public string FieldName { get { return fieldName; } }
		public SortingSummaryFunction Function {
			get {
				TypeConverter typeConverter = TypeDescriptor.GetConverter(typeof(SortingSummaryFunction));
				return (SortingSummaryFunction)typeConverter.ConvertFromString((string)cbSummaryFunction.EditValue);
			}
		}
		public bool SortingEnabled { get { return chkEnabled.Checked; } }
		void InitFunctionCombo() {
			cbSummaryFunction.Properties.Items.AddRange(GetEnumDisplayNames(typeof(SortingSummaryFunction)));
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(SortingSummaryFunction));
			cbSummaryFunction.EditValue = converter.ConvertToString(new StubTypeDescriptorContext(), band.SortingSummary.Function);
		}
		void InitSortOrderCombo() {
			DataTable table = new DataTable();
			table.Columns.Add("Text", typeof(string));
			table.Columns.Add("Value", typeof(object));
			table.Rows.Add(DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(XRColumnSortOrder.Ascending), XRColumnSortOrder.Ascending);
			table.Rows.Add(DevExpress.XtraPrinting.Native.DisplayTypeNameHelper.GetDisplayTypeName(XRColumnSortOrder.Descending), XRColumnSortOrder.Descending);
			lkeSortOrder.Properties.DataSource = table;
			lkeSortOrder.Properties.DisplayMember = "Text";
			lkeSortOrder.Properties.ValueMember = "Value";
			lkeSortOrder.Properties.Columns.Add(new LookUpColumnInfo("Text", 0, null));
			lkeSortOrder.EditValue = band.SortingSummary.SortOrder;
		}
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupSortingSummaryEditorForm));
			this.lbSortOrder = new DevExpress.XtraEditors.LabelControl();
			this.lkeSortOrder = new DevExpress.XtraEditors.LookUpEdit();
			this.chkEnabled = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbSummaryFunction.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreNullValues.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBoundField.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lkeSortOrder.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEnabled.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.lblSummaryFunction, "lblSummaryFunction");
			resources.ApplyResources(this.lblSummaryField, "lblSummaryField");
			resources.ApplyResources(this.cbSummaryFunction, "cbSummaryFunction");
			resources.ApplyResources(this.chkIgnoreNullValues, "chkIgnoreNullValues");
			resources.ApplyResources(this.cbBoundField, "cbBoundField");
			resources.ApplyResources(this.lbSortOrder, "lbSortOrder");
			this.lbSortOrder.Name = "lbSortOrder";
			resources.ApplyResources(this.lkeSortOrder, "lkeSortOrder");
			this.lkeSortOrder.Name = "lkeSortOrder";
			this.lkeSortOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("lkeSortOrder.Properties.Buttons"))))});
			this.lkeSortOrder.Properties.ShowFooter = false;
			this.lkeSortOrder.Properties.ShowHeader = false;
			resources.ApplyResources(this.chkEnabled, "chkEnabled");
			this.chkEnabled.Name = "chkEnabled";
			this.chkEnabled.Properties.Caption = resources.GetString("checkEdit1.Properties.Caption");
			this.chkEnabled.CheckedChanged += new System.EventHandler(this.chkEnabled_CheckedChanged);
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.lbSortOrder);
			this.Controls.Add(this.lkeSortOrder);
			this.Controls.Add(this.chkEnabled);
			this.Name = "GroupSummaryEditorForm";
			this.Controls.SetChildIndex(this.lblSummaryFunction, 0);
			this.Controls.SetChildIndex(this.chkEnabled, 0);
			this.Controls.SetChildIndex(this.chkIgnoreNullValues, 0);
			this.Controls.SetChildIndex(this.cbSummaryFunction, 0);
			this.Controls.SetChildIndex(this.lkeSortOrder, 0);
			this.Controls.SetChildIndex(this.lbSortOrder, 0);
			this.Controls.SetChildIndex(this.lblSummaryField, 0);
			this.Controls.SetChildIndex(this.cbBoundField, 0);
			((System.ComponentModel.ISupportInitialize)(this.cbSummaryFunction.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkIgnoreNullValues.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBoundField.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lkeSortOrder.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.chkEnabled.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		void chkEnabled_CheckedChanged(object sender, EventArgs e) {
			UpdateControls();
		}
		void UpdateControls() {
			chkIgnoreNullValues.Enabled = SortingEnabled;
			lblSummaryField.Enabled = SortingEnabled;
			cbBoundField.Enabled = SortingEnabled;
			lblSummaryFunction.Enabled = SortingEnabled;
			cbSummaryFunction.Enabled = SortingEnabled;
			lbSortOrder.Enabled = SortingEnabled;
			lkeSortOrder.Enabled = SortingEnabled;
		}
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		protected override object CalcSummaryResult(System.Collections.ArrayList values) {
			return SummaryHelper.CalcResult(Function, values);
		}
		#region field name edit
		void UpdateFieldNameComboText() {
			cbBoundField.Text = band.Report.GetShortFieldDisplayName(fieldName);
		}
		protected override PopupBindingPickerBase CreatePopupBindingPicker() {
			return new PopupFieldNamePicker();
		}
		protected override void StartFieldNameEdit() {
			BindingPicker.Start(
				serviceProvider,
				ReportHelper.GetEffectiveDataSource(band.Report),
				band.Report.DataMember,
				band.SortingSummary.FieldName, 
				cbBoundField);
		}
		protected override void EndFieldNameEdit() {
			fieldName = BindingPicker.EndFieldNamePicker();
			UpdateFieldNameComboText();
		}
		protected override Type GetPropertyType(DataContext dataContext) {
			return typeof(System.Int32);
		}
		#endregion
	}
}
