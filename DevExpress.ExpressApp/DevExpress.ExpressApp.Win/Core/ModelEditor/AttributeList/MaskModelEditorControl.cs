#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Drawing;
using System.Drawing.Design;
using System.Reflection;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.ExpressApp.Editors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Design;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class MaskModelEditorControl : UITypeEditor {
		internal IWindowsFormsEditorService edSvc = null;
		private void DisableControl(Control control) {
			control.Enabled = false;
			control.EnabledChanged += new EventHandler(control_EnabledChanged);
		}
		void control_EnabledChanged(object sender, EventArgs e) {
			((Control)sender).Enabled = false;
		}
		private EditMaskEditorForm CreateEditMaskEditorForm(EditMaskType editMaskType) {
			EditMaskEditorForm form = new EditMaskEditorForm();
			DisableControl(form.Controls["checkEditBeepOnError"]);
			DisableControl(form.Controls["checkEditIgnoreMaskBlank"]);
			DisableControl(form.Controls["checkEditSaveLiteral"]);
			DisableControl(form.Controls["checkEditShowPlaceHolders"]);
			DisableControl(form.Controls["textEditPlaceHolder"]);
			DisableControl(form.Controls["comboBoxAutoComplete"]);
			DisableControl(form.Controls["labelAutocompleteMode"]);
			ComboBoxEdit maskTypeCombobox = ((ComboBoxEdit)form.Controls["maskTypeCombobox"]);
			maskTypeCombobox.SelectedText = editMaskType.ToString();
			maskTypeCombobox.SelectedValueChanged += new EventHandler(MaskModelEditorControl_SelectedValueChanged);
			form.Controls["btnOk"].EnabledChanged += new EventHandler(BtnOk_EnabledChanged);
			return form;
		}
		private void BtnOk_EnabledChanged(object sender, EventArgs e) {
			Control form = ((Control)sender).Parent;
			UpdateBtnOkEnabled(form);
		}
		private List<string> GetEditMaskTypeNames() {
			return new List<string>(Enum.GetNames(typeof(EditMaskType)));
		}
		private bool UpdateBtnOkEnabled(Control form) {
			ComboBoxEdit maskTypeCombobox = (ComboBoxEdit)form.Controls["maskTypeCombobox"];
			string selectedItem = (string)maskTypeCombobox.SelectedItem;
			bool btnOkEnabled = GetEditMaskTypeNames().Contains(selectedItem);
			form.Controls["btnOk"].Enabled = btnOkEnabled;
			if(!btnOkEnabled) {
				form.Controls["labelMaskTypeDescription"].Text = "This mask type is not supported";
			}
			form.Controls["labelMaskTypeDescription"].ForeColor = btnOkEnabled ? SystemColors.ControlText : Color.Red;
			return true;
		}
		private void MaskModelEditorControl_SelectedValueChanged(object sender, EventArgs e) {
			Control form = ((Control)sender).Parent;
			UpdateBtnOkEnabled(form);
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			if(context != null && context.Instance != null) {
				EditMaskType currentEditMaskType = EditMaskType.Default;
				IModelRegisteredPropertyEditor modelRegisteredPropertyEditor = context.Instance as IModelRegisteredPropertyEditor;
				IModelCommonMemberViewItem modelCommonMemberViewItem = context.Instance as IModelCommonMemberViewItem;
				if(modelRegisteredPropertyEditor != null) {
					currentEditMaskType = modelRegisteredPropertyEditor.DefaultEditMaskType;
				}
				else if(modelCommonMemberViewItem != null) {
					currentEditMaskType = modelCommonMemberViewItem.EditMaskType;
				}
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
				if(edSvc != null) {
					RepositoryItemTextEdit repositoryItem = new RepositoryItemTextEdit();
					MaskType currentMaskType = MaskType.None;
					if(Enum.TryParse<MaskType>(currentEditMaskType.ToString(), out currentMaskType)) {
						repositoryItem.Mask.MaskType = currentMaskType;
					}
					repositoryItem.Mask.EditMask = (string)value;
					using(EditMaskEditorForm form = CreateEditMaskEditorForm(currentEditMaskType)) {
						form.RepositoryItem = repositoryItem;
						if((edSvc.ShowDialog(form) == DialogResult.OK) && Enum.TryParse<EditMaskType>(repositoryItem.Mask.MaskType.ToString(), out currentEditMaskType)) {
							if(modelRegisteredPropertyEditor != null) {
								modelRegisteredPropertyEditor.DefaultEditMaskType = currentEditMaskType;
							}
							else if(modelCommonMemberViewItem != null) {
								modelCommonMemberViewItem.EditMaskType = currentEditMaskType;
							}
						}
						return repositoryItem.Mask.EditMask;
					}
				}
			}
			return value;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if(context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
