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

using System.ComponentModel;
using DevExpress.XtraEditors;
namespace DevExpress.DataAccess.UI.Native.Sql.DataConnectionControls {
	partial class ConnectionNameControl {
		private IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (this.components != null)) {
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionNameControl));
			this.dataConnectionNameTextEdit = new DevExpress.XtraEditors.TextEdit();
			this.dataConnectionNameLabel = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.dataConnectionNameTextEdit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.dataConnectionNameTextEdit, "dataConnectionNameTextEdit");
			this.dataConnectionNameTextEdit.Name = "dataConnectionNameTextEdit";
			this.dataConnectionNameTextEdit.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.dataConnectionNameTextEdit_EditValueChanging);
			resources.ApplyResources(this.dataConnectionNameLabel, "dataConnectionNameLabel");
			this.dataConnectionNameLabel.Name = "dataConnectionNameLabel";
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.dataConnectionNameTextEdit);
			this.Controls.Add(this.dataConnectionNameLabel);
			this.Name = "ConnectionNameControl";
			((System.ComponentModel.ISupportInitialize)(this.dataConnectionNameTextEdit.Properties)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private TextEdit dataConnectionNameTextEdit;
		private LabelControl dataConnectionNameLabel;
	}
}
