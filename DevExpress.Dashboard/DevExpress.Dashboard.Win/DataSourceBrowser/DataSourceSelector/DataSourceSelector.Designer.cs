#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class DataSourceSelector {
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DataSourceSelector));
			this.cbDataSource = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblDataSource = new DevExpress.XtraEditors.LabelControl();
			this.pnlDataMember = new System.Windows.Forms.Panel();
			this.lblDataMember = new DevExpress.XtraEditors.LabelControl();
			this.cbDataMember = new DevExpress.XtraEditors.ImageComboBoxEdit();
			((System.ComponentModel.ISupportInitialize)(this.cbDataSource.Properties)).BeginInit();
			this.pnlDataMember.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDataMember.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.cbDataSource, "cbDataSource");
			this.cbDataSource.Name = "cbDataSource";
			this.cbDataSource.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDataSource.Properties.Buttons"))))});
			this.cbDataSource.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDataSource.SelectedIndexChanged += new System.EventHandler(this.OnDataSourceSelectedIndexChanged);
			resources.ApplyResources(this.lblDataSource, "lblDataSource");
			this.lblDataSource.Name = "lblDataSource";
			resources.ApplyResources(this.pnlDataMember, "pnlDataMember");
			this.pnlDataMember.Controls.Add(this.lblDataMember);
			this.pnlDataMember.Controls.Add(this.cbDataMember);
			this.pnlDataMember.Name = "pnlDataMember";
			resources.ApplyResources(this.lblDataMember, "lblDataMember");
			this.lblDataMember.Name = "lblDataMember";
			resources.ApplyResources(this.cbDataMember, "cbDataMember");
			this.cbDataMember.Name = "cbDataMember";
			this.cbDataMember.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbDataMember.Properties.Buttons"))))});
			this.cbDataMember.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbDataMember.SelectedIndexChanged += new System.EventHandler(this.OnDataMemberSelectedIndexChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.pnlDataMember);
			this.Controls.Add(this.lblDataSource);
			this.Controls.Add(this.cbDataSource);
			this.Name = "DataSourceSelector";
			((System.ComponentModel.ISupportInitialize)(this.cbDataSource.Properties)).EndInit();
			this.pnlDataMember.ResumeLayout(false);
			this.pnlDataMember.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbDataMember.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraEditors.ComboBoxEdit cbDataSource;
		private XtraEditors.LabelControl lblDataSource;
		private System.Windows.Forms.Panel pnlDataMember;
		private XtraEditors.LabelControl lblDataMember;
		private XtraEditors.ImageComboBoxEdit cbDataMember;
	}
}
