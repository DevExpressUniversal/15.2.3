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
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Drawing;
namespace DevExpress.XtraDataLayout.DesignTime {
	[ToolboxItem(false)]
	public class DataLayoutWizardForm : WizardForm {
		private CheckEdit cbShowOnNextStart;
		private LabelControl labelControl1;
		protected DataLayoutDesigner ownercore;
		public DataLayoutWizardForm(DataLayoutDesigner owner) {
			this.ownercore = owner;
			InitializeComponent();
			this.CallOnWizardFinishForEachPage = true;
			WizardButtons = WizardButton.Back | WizardButton.Next | WizardButton.Finish;
			cbShowOnNextStart.Checked = DataLayoutDesigner.ShowWizardOnComponentAdding;
		}
		protected override Control CreateWizardButton() {
			return new SimpleButton();
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			WizardPage page = e.Control as WizardPage;
			if(page != null) {
				page.Size = new Size(ClientRectangle.Width, labelControl1.Location.Y);
			}
		}
		public void Init() {
			Controls.AddRange(new Control[] {
													 new WizardPageStep1(ownercore),
													 new WizardPageStep2(ownercore),
													 new WizardPageStep3()
													 }
					);
		}
		private void InitializeComponent() {
			this.cbShowOnNextStart = new DevExpress.XtraEditors.CheckEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.cbShowOnNextStart.Properties)).BeginInit();
			this.SuspendLayout();
			this.btnBack.Location = new System.Drawing.Point(289, 300);
			this.btnNext.Location = new System.Drawing.Point(368, 300);
			this.btnCancel.Location = new System.Drawing.Point(456, 300);
			this.btnFinish.Location = new System.Drawing.Point(368, 300);
			this.separator.Location = new System.Drawing.Point(-27, 281);
			this.separator.Size = new System.Drawing.Size(20, 10);
			this.cbShowOnNextStart.EditValue = true;
			this.cbShowOnNextStart.Location = new System.Drawing.Point(12, 297);
			this.cbShowOnNextStart.Name = "cbShowOnNextStart";
			this.cbShowOnNextStart.Properties.Caption = " Show the wizard every time a new \r\n DataLayoutControl is dropped onto the form";
			this.cbShowOnNextStart.Size = new System.Drawing.Size(265, 31);
			this.cbShowOnNextStart.TabIndex = 12;
			this.cbShowOnNextStart.CheckedChanged += new System.EventHandler(this.cbShowOnNextStart_CheckedChanged);
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.LineColor = System.Drawing.SystemColors.Control;
			this.labelControl1.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.labelControl1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Location = new System.Drawing.Point(0, 283);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(546, 3);
			this.labelControl1.TabIndex = 13;
			this.ClientSize = new System.Drawing.Size(545, 335);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.cbShowOnNextStart);
			this.Name = "DataLayoutWizardForm";
			this.Controls.SetChildIndex(this.separator, 0);
			this.Controls.SetChildIndex(this.btnFinish, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.btnNext, 0);
			this.Controls.SetChildIndex(this.btnBack, 0);
			this.Controls.SetChildIndex(this.cbShowOnNextStart, 0);
			this.Controls.SetChildIndex(this.labelControl1, 0);
			((System.ComponentModel.ISupportInitialize)(this.cbShowOnNextStart.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		private void cbShowOnNextStart_CheckedChanged(object sender, System.EventArgs e) {
			DataLayoutDesigner.ShowWizardOnComponentAdding = cbShowOnNextStart.Checked;
		}
	}
}
