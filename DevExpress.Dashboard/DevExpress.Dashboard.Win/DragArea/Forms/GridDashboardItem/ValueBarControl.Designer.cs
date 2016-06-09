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
	partial class ValueBarControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ValueBarControl));
			this.barCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			this.valueCheckEdit = new DevExpress.XtraEditors.CheckEdit();
			((System.ComponentModel.ISupportInitialize)(this.barCheckEdit.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.valueCheckEdit.Properties)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.barCheckEdit, "barCheckEdit");
			this.barCheckEdit.Name = "barCheckEdit";
			this.barCheckEdit.Properties.Caption = resources.GetString("barCheckEdit.Properties.Caption");
			this.barCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.barCheckEdit.Properties.RadioGroupIndex = 2;
			this.barCheckEdit.TabStop = false;
			this.barCheckEdit.CheckedChanged += new System.EventHandler(this.BarCheckEditCheckedChanged);
			resources.ApplyResources(this.valueCheckEdit, "valueCheckEdit");
			this.valueCheckEdit.Name = "valueCheckEdit";
			this.valueCheckEdit.Properties.Caption = resources.GetString("valueCheckEdit.Properties.Caption");
			this.valueCheckEdit.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
			this.valueCheckEdit.Properties.RadioGroupIndex = 2;
			this.valueCheckEdit.CheckedChanged += new System.EventHandler(this.ValueCheckEditCheckedChanged);
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.barCheckEdit);
			this.Controls.Add(this.valueCheckEdit);
			this.Name = "ValueBarControl";
			((System.ComponentModel.ISupportInitialize)(this.barCheckEdit.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.valueCheckEdit.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.CheckEdit barCheckEdit;
		private XtraEditors.CheckEdit valueCheckEdit;
	}
}
