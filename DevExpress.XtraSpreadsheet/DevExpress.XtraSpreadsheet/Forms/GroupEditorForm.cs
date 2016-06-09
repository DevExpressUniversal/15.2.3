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

using System.Windows.Forms;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Native;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Native;
namespace DevExpress.XtraSpreadsheet.Forms {
	public partial class GroupEditorForm : XtraForm {
		#region fields
		GroupEditorViewModel viewModel;
		string dataMember;
		SpreadsheetControl control;
		SpreadsheetGroupTreeList groupTreeList;
		SimpleButton btnCancel;
		SimpleButton btnOk;
		SimpleButton btnNew;
		SimpleButton btnDelete;
		SimpleButton btnUp;
		SimpleButton btnDown;
		#endregion
		GroupEditorForm() {
			InitializeComponent();
		}
		public GroupEditorForm(GroupEditorViewModel viewModel, SpreadsheetControl control) {
			this.viewModel = viewModel;
			this.control = control;
			this.dataMember = viewModel.DataMember;
			InitializeComponent();
			groupTreeList.FillGroupInfo(viewModel.GroupInfo);
			UpdateButtons();
		}
		#region Properties
		protected internal GroupEditorViewModel ViewModel { get { return viewModel; } }
		#endregion
		void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GroupEditorForm));
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOk = new DevExpress.XtraEditors.SimpleButton();
			this.btnNew = new DevExpress.XtraEditors.SimpleButton();
			this.btnDelete = new DevExpress.XtraEditors.SimpleButton();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.groupTreeList = new SpreadsheetGroupTreeList(control, dataMember);
			this.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupTreeList)).BeginInit();
			this.btnCancel.CausesValidation = false;
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			resources.ApplyResources(this.btnOk, "btnOk");
			this.btnOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			this.btnOk.Click += btnOk_Click;
			this.btnNew.CausesValidation = false;
			resources.ApplyResources(this.btnNew, "btnNew");
			this.btnNew.Click += btnNew_Click;
			this.btnDelete.CausesValidation = false;
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Click += btnDelete_Click;
			this.btnUp.CausesValidation = false;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.Click += btnUp_Click;
			this.btnDown.CausesValidation = false;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.Click += btnDown_Click;
			this.groupTreeList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.groupTreeList.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
			this.groupTreeList.Location = new System.Drawing.Point(12, 49);
			this.groupTreeList.Name = "groupTreeList";
			this.groupTreeList.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
			this.groupTreeList.Size = new System.Drawing.Size(288, 216);
			this.groupTreeList.TabIndex = 4;
			this.groupTreeList.FocusedNodeChanged += groupTreeList_FocusedNodeChanged;
			this.CancelButton = this.btnCancel;
			resources.ApplyResources(this, "$this");
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOk);
			this.Controls.Add(this.btnNew);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.btnUp);
			this.Controls.Add(this.btnDown);
			this.Controls.Add(this.groupTreeList);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "GroupEditorForm";
			this.ShowInTaskbar = false;
			((System.ComponentModel.ISupportInitialize)(this.groupTreeList)).EndInit();
			this.ResumeLayout(false);
		}
		void groupTreeList_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e) {
			UpdateButtons();
		}
		void btnDown_Click(object sender, System.EventArgs e) {
			groupTreeList.MoveNodeDown();
			UpdateButtons();
		}
		void btnUp_Click(object sender, System.EventArgs e) {
			groupTreeList.MoveNodeUp();
			UpdateButtons();
		}
		void btnDelete_Click(object sender, System.EventArgs e) {
			groupTreeList.DeleteNode();
			UpdateButtons();
		}
		void btnNew_Click(object sender, System.EventArgs e) {
			groupTreeList.CreateNewNode();
			UpdateButtons();
		}
		void btnOk_Click(object sender, System.EventArgs e) {
			viewModel.GroupInfo = groupTreeList.GetGroupInfo();
			this.DialogResult = DialogResult.OK;
		}
		void UpdateButtons() {
			btnDelete.Enabled = groupTreeList.Nodes.Count > 0;
			if (!btnDelete.Enabled)
				return;
			btnUp.Enabled = groupTreeList.FocusedNode.ParentNode != null;
			btnDown.Enabled = groupTreeList.FocusedNode.Nodes.Count > 0;
		}
	}
}
