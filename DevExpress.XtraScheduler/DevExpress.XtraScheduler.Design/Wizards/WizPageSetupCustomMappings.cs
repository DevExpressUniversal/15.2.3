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
using System.Collections.Specialized;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraScheduler.Native;
using DevExpress.Data;
namespace DevExpress.XtraScheduler.Design.Wizards {
	public partial class WizPageSetupCustomMappings<T> : DevExpress.Utils.InteriorWizardPage where T : IPersistentObject {
		SetupCustomFieldMappingsWizard<T> wizard;
		public WizPageSetupCustomMappings() {
			InitializeComponent();
		}
		public WizPageSetupCustomMappings(SetupCustomFieldMappingsWizard<T> wizard) {
			if (wizard == null)
				Exceptions.ThrowArgumentNullException("wizard");
			this.wizard = wizard;
			InitializeComponent();
		}
		protected override bool OnSetActive() {
			wizard.Initialize();
			lvSourceFields.DataSource = wizard.AvailableFields;
			lvCustomFields.DataSource = wizard.CustomFields;
			gvSourceFields.RefreshData();
			gvCustomFields.RefreshData();
			UpdatePromoteButtons();
			UpdateRevokeButtons();
			this.btnCorrectCustomFieldName.Checked = wizard.AutoCorrectCustomFieldName;
			this.lblDescription.Text = "Capitalize words, remove spaces and non-alphanumeric characters.";
			return base.OnSetActive();
		}
		protected override string OnWizardNext() {
			if (!ValidateMappings())
				return WizardForm.NoPageChange;
			ApplyChanges();
			return base.OnWizardNext();
		}
		protected override bool OnWizardFinish() {
			if (!ValidateMappings())
				return false;
			try {
				ApplyChanges();
			}
			catch {
			}
			return base.OnWizardFinish();
		}
		void gvSourceFields_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			UpdatePromoteButtons();
		}
		void gvCustomFields_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			UpdateRevokeButtons();
		}
		void btnPromoteFieldToCustomMapping_Click(object sender, EventArgs e) {
			PromoteSelectedFieldsToCustomMappings();
		}
		void btnPromoteFieldsToCustomMapping_Click(object sender, EventArgs e) {
			PromoteAllFieldsToCustomMappings();
		}
		void btnRevokeCustomField_Click(object sender, EventArgs e) {
			RevokeSelectedCustomMappings();
		}
		void btnRevokeCustomFields_Click(object sender, EventArgs e) {
			RevokeAllCustomMappings();
		}
		void gvSourceFields_DoubleClick(object sender, EventArgs e) {
			PromoteSelectedFieldsToCustomMappings();
		}
		void gvCustomFields_DoubleClick(object sender, EventArgs e) {
			RevokeSelectedCustomMappings();
		}
		protected internal virtual bool ValidateMappings() {
			StringBuilder errorMessage = new StringBuilder();
			if (!wizard.ValidateMappings(errorMessage))
				return ShowInvalidMappingsErrorMessage(errorMessage.ToString());
			else
				return true;
		}
		protected internal virtual bool ShowInvalidMappingsErrorMessage(string errorMessage) {
			return XtraMessageBox.Show(this, errorMessage, Wizard.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Error) == DialogResult.Yes;
		}
		protected internal virtual void ApplyChanges() {
			wizard.ApplyChanges();
		}
		protected internal virtual void UpdatePromoteButtons() {
			btnPromoteFieldToCustomMapping.Enabled = (gvSourceFields.SelectedRowsCount > 0);
			btnPromoteFieldsToCustomMapping.Enabled = (gvSourceFields.RowCount > 0);
		}
		protected internal virtual void UpdateRevokeButtons() {
			btnRevokeCustomField.Enabled = (gvCustomFields.SelectedRowsCount > 0);
			btnRevokeCustomFields.Enabled = (gvCustomFields.RowCount > 0);
		}
		protected internal virtual void UpdateControls() {
			gvSourceFields.RefreshData();
			gvCustomFields.RefreshData();
			UpdatePromoteButtons();
			UpdateRevokeButtons();
		}
		protected internal virtual void PromoteSelectedFieldsToCustomMappings() {
			DataFieldInfoCollection fields = new DataFieldInfoCollection();
			int[] rows = gvSourceFields.GetSelectedRows();
			int count = rows.Length;
			for (int i = 0; i < count; i++)
				fields.Add((DataFieldInfo)gvSourceFields.GetRow(rows[i]));
			PromoteFieldsToCustomMappings(fields);
		}
		protected internal virtual void PromoteAllFieldsToCustomMappings() {
			DataFieldInfoCollection fields = new DataFieldInfoCollection();
			int count = gvSourceFields.RowCount;
			for (int i = 0; i < count; i++)
				fields.Add((DataFieldInfo)gvSourceFields.GetRow(i));
			PromoteFieldsToCustomMappings(fields);
		}
		protected internal virtual void PromoteFieldsToCustomMappings(DataFieldInfoCollection fields) {
			int count = fields.Count;
			for (int i = 0; i < count; i++)
				wizard.ConvertFieldInfoIntoCustomFieldMapping(fields[i]);
			UpdateControls();
		}
		protected internal virtual void RevokeSelectedCustomMappings() {
			CustomFieldMappingCollectionBase<T> mappings = new CustomFieldMappingCollectionBase<T>();
			int[] rows = gvCustomFields.GetSelectedRows();
			int count = rows.Length;
			for (int i = 0; i < count; i++)
				mappings.Add((CustomFieldMappingBase<T>)gvCustomFields.GetRow(rows[i]));
			RevokeCustomMappings(mappings);
		}
		protected internal virtual void RevokeAllCustomMappings() {
			CustomFieldMappingCollectionBase<T> mappings = new CustomFieldMappingCollectionBase<T>();
			int count = gvCustomFields.RowCount;
			for (int i = 0; i < count; i++)
				mappings.Add((CustomFieldMappingBase<T>)gvCustomFields.GetRow(i));
			RevokeCustomMappings(mappings);
		}
		protected internal virtual void RevokeCustomMappings(CustomFieldMappingCollectionBase<T> mappings) {
			int count = mappings.Count;
			for (int i = 0; i < count; i++)
				wizard.RemoveCustomFieldMapping(mappings[i]);
			UpdateControls();
		}
		void OnBtnCorrectCustomFieldNameCheckedChanged(object sender, EventArgs e) {
			if (wizard == null)
				return;
			wizard.AutoCorrectCustomFieldName = this.btnCorrectCustomFieldName.Checked;
		}
	}
}
