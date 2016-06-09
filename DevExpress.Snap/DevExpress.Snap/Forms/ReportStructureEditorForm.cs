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

using DevExpress.Snap.Core;
using DevExpress.Snap.Core.Forms;
using DevExpress.Snap.Localization;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit.Model;
#region FxCop suppressions
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.ReportStructureEditorForm.lblSplit")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.ReportStructureEditorForm.tv")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.ReportStructureEditorForm.btnOk")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.ReportStructureEditorForm.btnCancel")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.ReportStructureEditorForm.btnUp")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051", Scope = "member", Target = "DevExpress.Snap.Forms.ReportStructureEditorForm.btnDown")]
#endregion
namespace DevExpress.Snap.Forms {
	public partial class ReportStructureEditorForm : XtraForm {
		readonly ISnapControl control;
		readonly ReportStructureEditorFormController controller;
		ReportStructureEditorForm() {
			InitializeComponent();
		}
		public ReportStructureEditorForm(ReportStructureEditorFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			InitializeComponent();
			this.controller = CreateController(controllerParameters);
			UpdateForm();
		}
		public ISnapControl Control { get { return control; } }
		public ReportStructureEditorFormController Controller { get { return controller; } }
		protected internal virtual ReportStructureEditorFormController CreateController(ReportStructureEditorFormControllerParameters controllerParameters) {
			return new ReportStructureEditorFormController(controllerParameters);
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
		protected internal virtual void SubscribeControlsEvents() {
			tv.AfterFocusNode += ReportExplorerAfterFocusNode;
		}
		protected internal virtual void UnsubscribeControlsEvents() {
			tv.AfterFocusNode -= ReportExplorerAfterFocusNode;
		}
		protected internal virtual void UpdateFormCore() {
			InitializeReportExplorer();
			this.Text = SnapLocalizer.GetString(SnapStringId.ReorderReportStructureForm_Text);
		}
		void InitializeReportExplorer() {
			Field field = Controller.FindSelectedListField();
			this.tv.Controller.Control = Control;
			this.tv.Controller.FillNodesByParentList(field, tv.Nodes);
			tv.ExpandAll();
			this.tv.SetFocusedNode(this.tv.FindNodeByTag(this.tv.Nodes, Controller.FocusedNodeTag));
			UpdateUpDownButtons();
		}
		void ReportExplorerAfterFocusNode(object sender, XtraTreeList.NodeEventArgs e) {
			Controller.FocusedNodeTag = tv.GetFocusedNodeTag();
			UpdateUpDownButtons();
		}
		void UpdateUpDownButtons() {
			this.btnUp.Enabled = this.tv.CanMoveUp();
			this.btnDown.Enabled = this.tv.CanMoveDown();
		}
		void btnOk_Click(object sender, System.EventArgs e) {
			Controller.ApplyChanges();
			this.DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		void btnUp_Click(object sender, System.EventArgs e) {
			Controller.MoveUp();
			this.tv.MoveFocusedNodeUp();
			Controller.FocusedNodeTag = this.tv.GetFocusedNodeTag();
			UpdateUpDownButtons();
		}
		void btnDown_Click(object sender, System.EventArgs e) {
			Controller.MoveDown();
			this.tv.MoveFocusedNodeDown();
			Controller.FocusedNodeTag = this.tv.GetFocusedNodeTag();
			UpdateUpDownButtons();
		}
	}
}
