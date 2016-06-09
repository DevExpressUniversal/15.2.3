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
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Core.ModelEditor {
	public class AspectManagementForm : XtraForm {
		private System.Windows.Forms.Panel pnlButtons;
		private DevExpress.XtraEditors.SimpleButton okButton;
		private DevExpress.XtraEditors.SimpleButton cancelButton;
		private System.Windows.Forms.Panel pnlControls;
		private System.Windows.Forms.Panel pnlAddRemove;
		private System.Windows.Forms.Panel pnlList;
		private DevExpress.XtraEditors.SimpleButton buttonAdd;
		private DevExpress.XtraEditors.SimpleButton buttonRemove;
		private DevExpress.XtraEditors.ListBoxControl listAspects;
		private System.ComponentModel.Container components = null;
		private bool readOnly = true;
		private string readonlyAspect;
		private List<string> aspects = new List<string>();
		protected override void Dispose(bool disposing) {
			try {
				if(disposing) {
					if(components != null) {
						components.Dispose();
					}
				}
			}
			finally {
				base.Dispose(disposing);
			}
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.pnlButtons = new System.Windows.Forms.Panel();
			this.okButton = new DevExpress.XtraEditors.SimpleButton();
			this.cancelButton = new DevExpress.XtraEditors.SimpleButton();
			this.pnlControls = new System.Windows.Forms.Panel();
			this.pnlList = new System.Windows.Forms.Panel();
			this.listAspects = new DevExpress.XtraEditors.ListBoxControl();
			this.pnlAddRemove = new System.Windows.Forms.Panel();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
			this.pnlButtons.SuspendLayout();
			this.pnlControls.SuspendLayout();
			this.pnlList.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.listAspects)).BeginInit();
			this.pnlAddRemove.SuspendLayout();
			this.SuspendLayout();
			this.pnlButtons.Controls.Add(this.okButton);
			this.pnlButtons.Controls.Add(this.cancelButton);
			this.pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.pnlButtons.Location = new System.Drawing.Point(0, 234);
			this.pnlButtons.Name = "pnlButtons";
			this.pnlButtons.Size = new System.Drawing.Size(292, 32);
			this.pnlButtons.TabIndex = 0;
			this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(131, 6);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(211, 6);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.pnlControls.Controls.Add(this.pnlList);
			this.pnlControls.Controls.Add(this.pnlAddRemove);
			this.pnlControls.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlControls.Location = new System.Drawing.Point(0, 0);
			this.pnlControls.Name = "pnlControls";
			this.pnlControls.Size = new System.Drawing.Size(292, 234);
			this.pnlControls.TabIndex = 1;
			this.pnlList.Controls.Add(this.listAspects);
			this.pnlList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pnlList.DockPadding.Left = 5;
			this.pnlList.DockPadding.Top = 5;
			this.pnlList.Location = new System.Drawing.Point(0, 0);
			this.pnlList.Name = "pnlList";
			this.pnlList.Size = new System.Drawing.Size(204, 234);
			this.pnlList.TabIndex = 1;
			this.listAspects.Dock = System.Windows.Forms.DockStyle.Fill;
			this.listAspects.Location = new System.Drawing.Point(5, 5);
			this.listAspects.Name = "listAspects";
			this.listAspects.Size = new System.Drawing.Size(199, 229);
			this.listAspects.SortOrder = System.Windows.Forms.SortOrder.Descending;
			this.listAspects.TabIndex = 0;
			this.pnlAddRemove.Controls.Add(this.buttonRemove);
			this.pnlAddRemove.Controls.Add(this.buttonAdd);
			this.pnlAddRemove.Dock = System.Windows.Forms.DockStyle.Right;
			this.pnlAddRemove.Location = new System.Drawing.Point(204, 0);
			this.pnlAddRemove.Name = "pnlAddRemove";
			this.pnlAddRemove.Size = new System.Drawing.Size(88, 234);
			this.pnlAddRemove.TabIndex = 0;
			this.buttonRemove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonRemove.Enabled = false;
			this.buttonRemove.Visible = false;
			this.buttonRemove.Location = new System.Drawing.Point(7, 37);
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.TabIndex = 7;
			this.buttonRemove.Text = "Remove";
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonAdd.Location = new System.Drawing.Point(7, 8);
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.TabIndex = 6;
			this.buttonAdd.Text = "Add";
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.AutoScaleMode = AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.pnlControls);
			this.Controls.Add(this.pnlButtons);
			this.KeyPreview = true;
			this.ShowInTaskbar = false;
			this.Name = "AspectManagmentForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Languages Manager";
			this.pnlButtons.ResumeLayout(false);
			this.pnlControls.ResumeLayout(false);
			this.pnlList.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.listAspects)).EndInit();
			this.pnlAddRemove.ResumeLayout(false);
			LookAndFeelUtils.ApplyStyle(this);
			this.ResumeLayout(false);
		}
		#endregion
		private void buttonAdd_Click(object sender, System.EventArgs e) {
			using(NewAspectForm newAspectForm = new NewAspectForm()) {
				newAspectForm.Icon = this.Icon;
				newAspectForm.FillAspects(aspects);
				if (newAspectForm.ShowDialog(this) == DialogResult.OK) {
					string newAspectName = newAspectForm.SelectedAspect;
					bool inserted = false;
					for(int i = 0; i < aspects.Count; i++) {
						if(string.Compare(newAspectName, aspects[i]) < 0) {
							aspects.Insert(i, newAspectName);
							inserted = true;
							break;
						}
					}
					if(!inserted) {
						aspects.Add(newAspectName);
					}
				}
			}
		}
		private void buttonRemove_Click(object sender, System.EventArgs e) {
			aspects.Remove((string)listAspects.SelectedValue);
		}
		private void listAspects_SelectedIndexChanged(object sender, EventArgs e) {
			buttonRemove.Enabled = !ReadOnly && ((string)listAspects.SelectedValue) != readonlyAspect;
		}
		private void listAspects_DataSourceChanged(object sender, EventArgs e) {
			buttonRemove.Enabled = !ReadOnly && (aspects.Count > 1 || aspects.Count == 1 && ((string)aspects[0]) != readonlyAspect);
		}
		public AspectManagementForm() {
			InitializeComponent();
			listAspects.SelectedIndexChanged += new EventHandler(listAspects_SelectedIndexChanged);
			listAspects.DataSourceChanged += new EventHandler(listAspects_DataSourceChanged);
			listAspects.DataSource = aspects;
		}
		public void SetAvailableAspects(string readonlyAspect, ICollection<string> aspects) {
			this.readonlyAspect = readonlyAspect;
			this.aspects.Clear();
			this.aspects.AddRange(aspects);
		}
		public List<string> Aspects { get { return aspects; } }
		public bool ReadOnly {
			get { return readOnly; }
			set {
				if(readOnly != value) {
					readOnly = value;
					okButton.Enabled = !value;
				}
			}
		}
	}
}
