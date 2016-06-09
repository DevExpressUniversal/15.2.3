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

using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Ribbon.Design {
	partial class RibbonMiniToolbarItemsManager {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.toolbarList = new DevExpress.XtraEditors.ComboBoxEdit();
			this.toolbarLabel = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.toolbarList.Properties)).BeginInit();
			this.SuspendLayout();
			this.splMain.Location = new System.Drawing.Point(440, 60);
			this.splMain.Size = new System.Drawing.Size(5, 370);
			this.pgMain.Location = new System.Drawing.Point(445, 60);
			this.pgMain.Size = new System.Drawing.Size(299, 370);
			this.pnlControl.Size = new System.Drawing.Size(744, 32);
			this.lbCaption.Size = new System.Drawing.Size(744, 42);
			this.pnlMain.Size = new System.Drawing.Size(440, 370);
			this.horzSplitter.Size = new System.Drawing.Size(744, 4);
			this.toolbarList.Location = new System.Drawing.Point(57, 8);
			this.toolbarList.Name = "toolbarList";
			this.toolbarList.Margin = new System.Windows.Forms.Padding(18, 5, 0, 5);
			this.toolbarList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Delete)});
			this.toolbarList.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.toolbarList.Size = new System.Drawing.Size(154, 20);
			this.toolbarList.TabIndex = 0;
			this.toolbarList.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.toolbarList_ButtonClick);
			this.toolbarList.SelectedIndexChanged += new System.EventHandler(this.toolbarList_SelectedIndexChanged);
			this.toolbarLabel.Location = new System.Drawing.Point(8, 8);
			this.toolbarLabel.Name = "toolbarLabel";
			this.toolbarLabel.Size = new System.Drawing.Size(37, 14);
			this.toolbarLabel.TabIndex = 1;
			this.toolbarLabel.Margin = new System.Windows.Forms.Padding(10, 8, 5, 5);
			this.toolbarLabel.Text = "Toolbar";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "RibbonMiniToolbarItemsManager";
			this.Size = new System.Drawing.Size(744, 430);
			((System.ComponentModel.ISupportInitialize)(this.pnlControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.toolbarList.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.ComboBoxEdit toolbarList;
		private DevExpress.XtraEditors.LabelControl toolbarLabel;
	}
}
