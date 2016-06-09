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

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
namespace DevExpress.XtraBars.Design
{
	public class contextMenuForm : System.Windows.Forms.Form
	{
		internal System.Windows.Forms.ComboBox cbConvert;
		private System.Windows.Forms.Button btConvert;
		private System.Windows.Forms.Button button2;
		private System.ComponentModel.Container components = null;
		public contextMenuForm()
		{
			InitializeComponent();
		}
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent()
		{
			this.cbConvert = new System.Windows.Forms.ComboBox();
			this.btConvert = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			this.cbConvert.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cbConvert.Location = new System.Drawing.Point(16, 40);
			this.cbConvert.Name = "cbConvert";
			this.cbConvert.Size = new System.Drawing.Size(208, 21);
			this.cbConvert.TabIndex = 0;
			this.btConvert.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btConvert.Location = new System.Drawing.Point(232, 40);
			this.btConvert.Name = "btConvert";
			this.btConvert.Size = new System.Drawing.Size(72, 24);
			this.btConvert.TabIndex = 1;
			this.btConvert.Text = "Import";
			this.btConvert.Click += new System.EventHandler(this.btConvert_Click);
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(112, 88);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(72, 24);
			this.button2.TabIndex = 2;
			this.button2.Text = "Exit";
			this.AcceptButton = this.button2;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(312, 126);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.button2,
																		  this.btConvert,
																		  this.cbConvert});
			this.Name = "contextMenuForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Import from ContextMenu";
			this.ResumeLayout(false);
		}
		#endregion
		private void btConvert_Click(object sender, System.EventArgs e) {
		}
	}
}
