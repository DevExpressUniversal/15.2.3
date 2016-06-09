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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.EditForm;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.XtraGrid.EditForm.Helpers.Controls {
	public class EditFormContainer : XtraUserControl {
		EditFormController controller;
		public EditFormContainer(EditFormController controller) {
			this.controller = controller;
		}
		protected override bool ProcessDialogKey(Keys keyData) {
			if(keyData == Keys.Escape) {
				DisableValidation();
				controller.CancelValues();
				return true;
			}
			if(keyData == (Keys.Control | Keys.Enter)) {
				controller.CloseForm();
				return true;
			}
			if(keyData == Keys.Enter) {
				if(ActiveControl is SimpleButton) {
					if(object.Equals(ActiveControl.Tag, "OkButton")) {
						controller.CloseForm();
						return true;
					}
					if(object.Equals(ActiveControl.Tag, "CancelButton")) {
						DisableValidation();
						controller.CancelValues();
						return true;
					}
				}
				Control activeControl = ActiveControl;
				if(activeControl is TextBoxMaskBox) activeControl = activeControl.Parent;
				if(activeControl is BaseEdit) {
					BaseEdit be = activeControl as BaseEdit;
					if(!be.IsNeededKey(new KeyEventArgs(keyData))) {
						this.SelectNextControl(ActiveControl, true, true, true, true);
						return true;
					}
				}
			}
			return base.ProcessDialogKey(keyData);
		}
		public void EnableValidation() {
			AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
		}
		public void DisableValidation() {
			AutoValidate = System.Windows.Forms.AutoValidate.Disable;
			GridEditorContainerHelper.ClearUnvalidatedControl(this, this);
		}
	}
	internal class XtraScrollableControlX : XtraScrollableControl {
		public XtraScrollableControlX() {
			SetStyle(ControlStyles.Selectable, false);
		}
	}
	internal class EditFormCancelButton : SimpleButton {
		public EditFormCancelButton() {
			CausesValidation = false;
		}
		protected override bool GetValidationCanceled() {
			return false;
		}
	}
	public interface IEditorFormTagProvider {
		string GetFieldName(Control control);
		string GetPropertyName(Control control);
	}
	public class DefaultEditFormTagProvider : IEditorFormTagProvider {
		static DefaultEditFormTagProvider _default;
		public static DefaultEditFormTagProvider Default {
			get {
				if(_default == null) _default = new DefaultEditFormTagProvider();
				return _default;
			}
		}
		string[] GetTag(Control control) {
			if(control.Tag == null) return null;
			string[] field = control.Tag.ToString().Split('/');
			if(field.Length != 2) return null;
			return field;
		}
		string IEditorFormTagProvider.GetFieldName(Control control) {
			var tag = GetTag(control);
			return tag == null ? null : tag[1];
		}
		string IEditorFormTagProvider.GetPropertyName(Control control) {
			var tag = GetTag(control);
			return tag == null ? null : tag[0];
		}
	}
}
