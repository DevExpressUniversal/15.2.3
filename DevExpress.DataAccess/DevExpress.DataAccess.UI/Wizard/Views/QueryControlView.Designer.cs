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

namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class QueryControlView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControl = new DevExpress.XtraLayout.LayoutControl();
			this.labelCaption = new DevExpress.XtraEditors.LabelControl();
			this.layoutGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemCaption = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).BeginInit();
			this.layoutControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCaption)).BeginInit();
			this.SuspendLayout();
			this.layoutControl.AllowCustomization = false;
			this.layoutControl.Controls.Add(this.labelCaption);
			this.layoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl.Location = new System.Drawing.Point(0, 0);
			this.layoutControl.Name = "layoutControl";
			this.layoutControl.Root = this.layoutGroup;
			this.layoutControl.Size = new System.Drawing.Size(457, 325);
			this.layoutControl.TabIndex = 0;
			this.labelCaption.Location = new System.Drawing.Point(12, 12);
			this.labelCaption.Name = "labelCaption";
			this.labelCaption.Size = new System.Drawing.Size(53, 13);
			this.labelCaption.StyleController = this.layoutControl;
			this.labelCaption.TabIndex = 4;
			this.labelCaption.Text = "SQL string:";
			this.layoutGroup.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroup.GroupBordersVisible = false;
			this.layoutGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemCaption});
			this.layoutGroup.Location = new System.Drawing.Point(0, 0);
			this.layoutGroup.Name = "layoutGroup";
			this.layoutGroup.Size = new System.Drawing.Size(457, 325);
			this.layoutGroup.TextVisible = false;
			this.layoutItemCaption.Control = this.labelCaption;
			this.layoutItemCaption.Location = new System.Drawing.Point(0, 0);
			this.layoutItemCaption.Name = "layoutItemCaption";
			this.layoutItemCaption.Size = new System.Drawing.Size(437, 305);
			this.layoutItemCaption.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCaption.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl);
			this.Name = "QueryControlView";
			this.Size = new System.Drawing.Size(457, 325);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl)).EndInit();
			this.layoutControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCaption)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutControl;
		private XtraEditors.LabelControl labelCaption;
		private XtraLayout.LayoutControlGroup layoutGroup;
		private XtraLayout.LayoutControlItem layoutItemCaption;
	}
}
