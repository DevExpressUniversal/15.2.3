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
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.lblInsert")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.btnInsert")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.rgFieldsSource")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.lblFields")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.panelControl1")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.panelControl2")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.XtraRichEdit.Forms.InsertMergeFieldForm.lbMergeFields")]
#endregion
namespace DevExpress.XtraRichEdit.Forms {
	public enum FieldsType { Address, Database }
	public partial class InsertMergeFieldForm : DevExpress.XtraEditors.XtraForm {
		readonly IRichEditControl control;
		InsertMergeFieldForm() {
			InitializeComponent();
		}
		public InsertMergeFieldForm(InsertMergeFieldFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.rgFieldsSource.SelectedIndex = 0;
			SubsribeControlsEvents();
			UpdateMergeFieldList();
		}
		public IRichEditControl Control { get { return control; } }
		protected DocumentModel DocumentModel { get { return Control.InnerControl.DocumentModel; } }
		protected FieldsType FieldsType { get { return this.rgFieldsSource.SelectedIndex == 0 ? FieldsType.Database : FieldsType.Address; } }
		protected virtual void SubsribeControlsEvents() {
			this.rgFieldsSource.SelectedIndexChanged += new EventHandler(OnSelectedIndexChanged);
		}
		protected virtual void UnsubsribeControlsEvents() {
			this.rgFieldsSource.SelectedIndexChanged -= new EventHandler(OnSelectedIndexChanged);
		}
		protected virtual void OnSelectedIndexChanged(object sender, EventArgs e) {
			UpdateMergeFieldList();
		}
		protected virtual void UpdateMergeFieldList() {
			this.lbMergeFields.Items.Clear();
			this.lbMergeFields.Items.AddRange(GetFieldNames());
			this.lbMergeFields.SelectedIndex = 0;
		}
		protected virtual MergeFieldName[] GetFieldNames() {
			if (FieldsType == FieldsType.Database) {
				MergeFieldName[] databaseFieldsNames = DocumentModel.GetDatabaseFieldNames();
				return Control.InnerControl.RaiseCustomizeMergeFields(new CustomizeMergeFieldsEventArgs(databaseFieldsNames));
			}
			return DocumentModel.GetAddressFieldNames();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			this.Close();
		}
		void btnInsert_Click(object sender, EventArgs e) {
			InsertMergeField();
		}
		protected virtual void InsertMergeField() {
			MergeFieldName argument = (MergeFieldName)this.lbMergeFields.SelectedItem;
			InsertMergeFieldCommand command = control.CreateCommand(RichEditCommandId.InsertMailMergeField) as InsertMergeFieldCommand;
			if (command != null) {
				command.FieldArgument = argument.Name;
				command.Execute();
			}
		}
		void InsertMergeFieldForm_FormClosed(object sender, FormClosedEventArgs e) {
			UnsubsribeControlsEvents();
		}
		void lbMergeFields_MouseDoubleClick(object sender, MouseEventArgs e) {
			if (e.Button != MouseButtons.Left)
				return;
			Rectangle rect = this.lbMergeFields.GetItemRectangle(this.lbMergeFields.SelectedIndex);
			if (rect.Contains(e.X, e.Y))
				InsertMergeField();
		}
	}
}
