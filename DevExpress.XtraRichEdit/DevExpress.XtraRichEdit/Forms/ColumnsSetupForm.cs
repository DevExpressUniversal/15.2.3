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
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Forms.Design;
using DevExpress.XtraEditors.Controls;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.grpPresets")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.grpColumns")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.lblColumnCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.edtColumnCount")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.chkLineBetween")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.chkEqualColumnWidth")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.cbApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.lblApplyTo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.chkStartNewColumn")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.columnsEdit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.columnsPresetControlOne")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.columnsPresetControlTwo")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.columnsPresetControlRight")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.columnsPresetControlLeft")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.ColumnsSetupForm.columnsPresetControlThree")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public partial class ColumnsSetupForm : XtraForm {
		readonly ColumnsSetupFormController controller;
		readonly IRichEditControl control;
		readonly List<ColumnsPresetControl> presetControls = new List<ColumnsPresetControl>();
		public ColumnsSetupForm() {
			InitializeComponent();
		}
		public ColumnsSetupForm(ColumnsSetupFormControllerParameters controllerParameters)
			: this() {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			InitializeForm();
			UpdateForm();
		}
		public ColumnsSetupFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		protected virtual ColumnsSetupFormController CreateController(ColumnsSetupFormControllerParameters controllerParameters) {
			return new ColumnsSetupFormController(controllerParameters);
		}
		void InitializeForm() {
			InitPresetControls();
			columnsEdit.DefaultUnitType = Control.InnerControl.UIUnit;
			columnsEdit.ColumnsInfo = controller.ColumnsInfo;
			columnsEdit.ValueUnitConverter = controller.ValueUnitConverter;
			FillApplyToCombo(cbApplyTo);
			SetApplyToComboInitialValue(cbApplyTo);
		}
		protected internal virtual void InitPresetControls() {
			AddPresetControl(columnsPresetControlOne, new SingleColumnsInfoPreset());
			AddPresetControl(columnsPresetControlTwo, new TwoUniformColumnsInfoPreset());
			AddPresetControl(columnsPresetControlThree, new ThreeUniformColumnsInfoPreset());
			AddPresetControl(columnsPresetControlLeft, new LeftNarrowColumnsInfoPreset());
			AddPresetControl(columnsPresetControlRight, new RightNarrowColumnsInfoPreset());
		}
		protected internal virtual void AddPresetControl(ColumnsPresetControl presetControl, ColumnsInfoPreset preset) {
			presetControl.Tag = preset;
			presetControls.Add(presetControl);
		}
		void OnColumnCountChanged(object sender, EventArgs e) {
			Controller.ChangeColumnCount((int)edtColumnCount.Value);
			UpdateForm();
		}
		void OnEqualColumnWidthChanged(object sender, EventArgs e) {
			controller.SetEqualColumnWidth(chkEqualColumnWidth.Checked);
			UpdateForm();
		}
		protected internal virtual void CommitValuesToController() {
			SectionPropertiesApplyTypeListBoxItem applyTypeItem = cbApplyTo.SelectedItem as SectionPropertiesApplyTypeListBoxItem;
			if (applyTypeItem != null)
				controller.ApplyType = applyTypeItem.ApplyType;
		}
		protected internal virtual void SubscribeControlsEvents() {
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged += ColumnsPresetChecked;
			edtColumnCount.EditValueChanged += OnColumnCountChanged;
			chkEqualColumnWidth.CheckedChanged += OnEqualColumnWidthChanged;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			int count = presetControls.Count;
			for (int i = 0; i < count; i++)
				presetControls[i].CheckedChanged -= ColumnsPresetChecked;
			edtColumnCount.EditValueChanged -= OnColumnCountChanged;
			chkEqualColumnWidth.CheckedChanged -= OnEqualColumnWidthChanged;
		}
		protected internal virtual void ColumnsPresetChecked(object sender, EventArgs e) {
			UnsubscribeControlsEvents();
			try {
				ColumnsPresetControl control = (ColumnsPresetControl)sender;
				ColumnsInfoPreset preset = (ColumnsInfoPreset)control.Tag;
				Controller.ApplyPreset(preset);
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		protected internal virtual void UpdateFormCore() {
			chkEqualColumnWidth.CheckState = GetChkEqualColumnWidthValue(controller.ColumnsInfo.EqualColumnWidth);
			edtColumnCount.Value = GetEdtColumnCountValue(controller.ColumnsInfo.ColumnCount);
			columnsEdit.UpdateForm();
			int count = presetControls.Count;
			for (int i = 0; i < count; i++) {
				ColumnsInfoPreset preset = (ColumnsInfoPreset)presetControls[i].Tag;
				presetControls[i].Checked = preset.MatchTo(Controller.ColumnsInfo);
			}
		}
		protected internal virtual CheckState GetChkEqualColumnWidthValue(bool? value) {
			if (!value.HasValue)
				return CheckState.Indeterminate;
			if (value.Value)
				return CheckState.Checked;
			else
				return CheckState.Unchecked;
		}
		protected internal virtual int GetEdtColumnCountValue(int? value) {
			return (value.HasValue) ? value.Value : 0;
		}
		protected internal virtual void FillApplyToCombo(ComboBoxEdit combo) {
			ComboBoxItemCollection items = combo.Properties.Items;
			items.BeginUpdate();
			try {
				AddApplyToComboItem(items, SectionPropertiesApplyType.WholeDocument);
				AddApplyToComboItem(items, SectionPropertiesApplyType.CurrentSection);
				AddApplyToComboItem(items, SectionPropertiesApplyType.SelectedSections);
				AddApplyToComboItem(items, SectionPropertiesApplyType.ThisPointForward);
			}
			finally {
				items.EndUpdate();
			}
		}
		protected internal virtual void AddApplyToComboItem(ComboBoxItemCollection items, SectionPropertiesApplyType applyType) {
			if ((Controller.AvailableApplyType & applyType) == applyType)
				items.Add(new SectionPropertiesApplyTypeListBoxItem(applyType));
		}
		protected internal virtual void SetApplyToComboInitialValue(ComboBoxEdit combo) {
			combo.EditValue = null;
			ComboBoxItemCollection items = combo.Properties.Items;
			int count = items.Count;
			for (int i = 0; i < count; i++) {
				SectionPropertiesApplyTypeListBoxItem item = (SectionPropertiesApplyTypeListBoxItem)items[i];
				if (item.ApplyType == controller.ApplyType) {
					combo.SelectedIndex = i;
					break;
				}
			}
		}
		protected internal virtual void OnBtnOkClick(object sender, EventArgs e) {
			CommitValuesToController();
			Controller.ApplyChanges();
			this.DialogResult = DialogResult.OK;
		}
	}
}
