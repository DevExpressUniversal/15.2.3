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

using DevExpress.Utils;
using DevExpress.Xpo.DB;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.Xpo.Design {
	public class AddClassesFromDbForm : WizardForm {
		private DevExpress.Xpo.Design.ConnectionPage connectionPage;
		private string connectionString;
		private IDataStoreSchemaExplorerSp connectionProvider;
		internal IDisposable[] ObjectsToDisposeOnDisconect = null;
		private PanelControl descriptionPanel;
		private LabelControl descriptionLabel;
		private LabelControl separatorLabel;
		private DevExpress.Xpo.Design.StructurePage structurePage;
		private LabelControl labelControl2;
		private DevExpress.Xpo.Design.StructureSPPage structureSPPage;
		public AddClassesFromDbForm(Language language) {
			this.InitializeComponent();
			this.descriptionLabel.Appearance.BackColor = DevExpress.Skins.CommonSkins.GetSkin(this.LookAndFeel).Colors["Window"];
			this.connectionPage = new DevExpress.Xpo.Design.ConnectionPage(this);
			this.connectionPage.Parent = this;
			this.structurePage = new DevExpress.Xpo.Design.StructurePage(this, language);
			this.structurePage.Parent = this;
			this.structureSPPage = new DevExpress.Xpo.Design.StructureSPPage(this, language);
			this.structureSPPage.Parent = this;
			this.structurePage.StructureSPPage = this.structureSPPage;
			this.WizardButtons = WizardButton.Cancel | WizardButton.Next | WizardButton.Back;
			this.FormBorderStyle = FormBorderStyle.Sizable;
			this.btnBack.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			this.btnNext.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			this.btnFinish.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			this.btnCancel.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
			this.separator.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom;
			this.StartPosition = FormStartPosition.CenterParent;
			this.MinimumSize = this.Size;
		}
		protected override Control CreateWizardButton() {
			return new SimpleButton();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.ObjectsToDisposeOnDisconect != null) {
					for(int i = 0; i < this.ObjectsToDisposeOnDisconect.Length; i++) {
						this.ObjectsToDisposeOnDisconect[i].Dispose();
					}
				}
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.separatorLabel = new DevExpress.XtraEditors.LabelControl();
			this.descriptionPanel = new DevExpress.XtraEditors.PanelControl();
			this.descriptionLabel = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.descriptionPanel)).BeginInit();
			this.descriptionPanel.SuspendLayout();
			this.SuspendLayout();
			this.btnBack.Location = new System.Drawing.Point(248, 325);
			this.btnBack.Text = "&Back";
			this.btnNext.Location = new System.Drawing.Point(329, 325);
			this.btnNext.Text = "&Next";
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(410, 325);
			this.btnFinish.Location = new System.Drawing.Point(329, 325);
			this.separator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.separator.ForeColor = System.Drawing.Color.Transparent;
			this.separator.Location = new System.Drawing.Point(0, 301);
			this.separator.Size = new System.Drawing.Size(499, 4);
			this.separator.Visible = false;
			this.separatorLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.separatorLabel.Appearance.ForeColor = System.Drawing.Color.Transparent;
			this.separatorLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.separatorLabel.LineVisible = true;
			this.separatorLabel.Location = new System.Drawing.Point(0, 38);
			this.separatorLabel.Name = "separatorLabel";
			this.separatorLabel.Size = new System.Drawing.Size(497, 4);
			this.separatorLabel.TabIndex = 13;
			this.descriptionPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.descriptionPanel.Controls.Add(this.descriptionLabel);
			this.descriptionPanel.Controls.Add(this.separatorLabel);
			this.descriptionPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.descriptionPanel.Location = new System.Drawing.Point(0, 0);
			this.descriptionPanel.Name = "descriptionPanel";
			this.descriptionPanel.Size = new System.Drawing.Size(497, 46);
			this.descriptionPanel.TabIndex = 12;
			this.descriptionLabel.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			this.descriptionLabel.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.descriptionLabel.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.descriptionLabel.Dock = System.Windows.Forms.DockStyle.Top;
			this.descriptionLabel.Location = new System.Drawing.Point(0, 0);
			this.descriptionLabel.Name = "descriptionLabel";
			this.descriptionLabel.Padding = new System.Windows.Forms.Padding(10, 0, 50, 0);
			this.descriptionLabel.Size = new System.Drawing.Size(497, 41);
			this.descriptionLabel.TabIndex = 13;
			this.descriptionLabel.Text = "labelControl1";
			this.labelControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelControl2.Appearance.ForeColor = System.Drawing.Color.Transparent;
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl2.LineVisible = true;
			this.labelControl2.Location = new System.Drawing.Point(0, 310);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(497, 5);
			this.labelControl2.TabIndex = 14;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(497, 360);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.descriptionPanel);
			this.Name = "AddClassesFromDbForm";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.Text = "Generating Persistent Classes for an Existing Database";
			this.Controls.SetChildIndex(this.btnBack, 0);
			this.Controls.SetChildIndex(this.btnCancel, 0);
			this.Controls.SetChildIndex(this.btnNext, 0);
			this.Controls.SetChildIndex(this.btnFinish, 0);
			this.Controls.SetChildIndex(this.descriptionPanel, 0);
			this.Controls.SetChildIndex(this.labelControl2, 0);
			this.Controls.SetChildIndex(this.separator, 0);
			((System.ComponentModel.ISupportInitialize)(this.descriptionPanel)).EndInit();
			this.descriptionPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		protected override void OnControlAdded(ControlEventArgs e) {
			base.OnControlAdded(e);
			WizardPage page = e.Control as WizardPage;
			if(page != null) {
				page.Bounds = new Rectangle(0, descriptionPanel.Bottom, base.ClientSize.Width, base.separator.Top - descriptionPanel.Bottom);
				page.Anchor = AnchorStyles.Right | AnchorStyles.Left | AnchorStyles.Bottom | AnchorStyles.Top;
			}
		}
		protected override void OnPageChanged() {
			descriptionLabel.Text = ActivePage.Description;
		}
		public string Code { get { return this.structurePage.Code; } }
		public string Log { get { return this.structurePage.Log; } }
		public string DatabaseName { get { return this.connectionPage.DatabaseName; } }
		internal new PCGWizardPage ActivePage { get { return (PCGWizardPage)base.ActivePage; } }
		internal ConnectionPage ConnectionPage { get { return connectionPage; } }
		StructurePage StructurePage { get { return structurePage; } }
		[Browsable(false)]
		internal Language Language { set { structurePage.Language = value; } }
		internal string ConnectionString {
			get {
				return this.connectionString;
			}
			set {
				this.connectionString = value;
			}
		}
		internal IDataStoreSchemaExplorerSp ConnectionProvider {
			get {
				return this.connectionProvider;
			}
			set {
				this.connectionProvider = value;
			}
		}
	}
}
