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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraMap.Drawing;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using System.Reflection;
namespace DevExpress.XtraMap.Design {
	[ToolboxItem(false)]
	public class LegendAlignmentControl : Control{
		const string imgPath = "DevExpress.XtraMap.Design.Images.";
		private System.Windows.Forms.RadioButton btnTopLeft;
		private System.Windows.Forms.RadioButton btnTopCenter;
		private System.Windows.Forms.RadioButton btnTopRight;
		private System.Windows.Forms.RadioButton btnMiddleRight;
		private System.Windows.Forms.RadioButton btnMiddleLeft;
		private System.Windows.Forms.RadioButton btnBottomCenter;
		private System.Windows.Forms.RadioButton btnBottomRight;
		private System.Windows.Forms.RadioButton btnBottomLeft;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private object editValue;
		private IWindowsFormsEditorService edSvc;
		private bool programmaticCheck;
		public object Value { get { return editValue; } }
		public LegendAlignmentControl()
		{
			InitializeComponent();
			Assembly asm = Assembly.GetExecutingAssembly();
			ResourceImageHelper.FillImageListFromResources(imageList1, imgPath + "LegendAlignment.bmp", asm);
			this.BackColor = System.Drawing.SystemColors.Control;
			btnTopLeft.Tag = LegendAlignment.TopLeft;
			btnTopCenter.Tag = LegendAlignment.TopCenter;
			btnTopRight.Tag = LegendAlignment.TopRight;
			btnMiddleLeft.Tag = LegendAlignment.MiddleLeft;
			btnMiddleRight.Tag = LegendAlignment.MiddleRight;
			btnBottomLeft.Tag = LegendAlignment.BottomLeft;
			btnBottomCenter.Tag = LegendAlignment.BottomCenter;
			btnBottomRight.Tag = LegendAlignment.BottomRight;
		}
		public void DropDown(object value, IWindowsFormsEditorService edSvc) {
			this.edSvc = edSvc;
			editValue = value;
			CheckDefaultButton();
			edSvc.DropDownControl(this);
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
			this.components = new System.ComponentModel.Container();
			this.btnTopLeft = new System.Windows.Forms.RadioButton();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.btnTopCenter = new System.Windows.Forms.RadioButton();
			this.btnTopRight = new System.Windows.Forms.RadioButton();
			this.btnMiddleRight = new System.Windows.Forms.RadioButton();
			this.btnMiddleLeft = new System.Windows.Forms.RadioButton();
			this.btnBottomCenter = new System.Windows.Forms.RadioButton();
			this.btnBottomRight = new System.Windows.Forms.RadioButton();
			this.btnBottomLeft = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			this.btnTopLeft.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnTopLeft.ImageIndex = 0;
			this.btnTopLeft.ImageList = this.imageList1;
			this.btnTopLeft.Location = new System.Drawing.Point(0, 0);
			this.btnTopLeft.Name = "btnTopLeft";
			this.btnTopLeft.Size = new System.Drawing.Size(30, 23);
			this.btnTopLeft.TabIndex = 0;
			this.btnTopLeft.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.TransparentColor = System.Drawing.Color.Magenta;
			this.btnTopCenter.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnTopCenter.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnTopCenter.ImageIndex = 1;
			this.btnTopCenter.ImageList = this.imageList1;
			this.btnTopCenter.Location = new System.Drawing.Point(41, 0);
			this.btnTopCenter.Name = "btnTopCenter";
			this.btnTopCenter.Size = new System.Drawing.Size(30, 23);
			this.btnTopCenter.TabIndex = 1;
			this.btnTopCenter.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTopRight.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnTopRight.ImageIndex = 3;
			this.btnTopRight.ImageList = this.imageList1;
			this.btnTopRight.Location = new System.Drawing.Point(82, 0);
			this.btnTopRight.Name = "btnTopRight";
			this.btnTopRight.Size = new System.Drawing.Size(30, 23);
			this.btnTopRight.TabIndex = 3;
			this.btnTopRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnMiddleRight.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnMiddleRight.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnMiddleRight.ImageIndex = 3;
			this.btnMiddleRight.ImageList = this.imageList1;
			this.btnMiddleRight.Location = new System.Drawing.Point(82, 32);
			this.btnMiddleRight.Name = "btnMiddleRight";
			this.btnMiddleRight.Size = new System.Drawing.Size(30, 23);
			this.btnMiddleRight.TabIndex = 7;
			this.btnMiddleRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnMiddleLeft.Anchor = System.Windows.Forms.AnchorStyles.Left;
			this.btnMiddleLeft.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnMiddleLeft.ImageIndex = 0;
			this.btnMiddleLeft.ImageList = this.imageList1;
			this.btnMiddleLeft.Location = new System.Drawing.Point(0, 32);
			this.btnMiddleLeft.Name = "btnMiddleLeft";
			this.btnMiddleLeft.Size = new System.Drawing.Size(30, 23);
			this.btnMiddleLeft.TabIndex = 4;
			this.btnMiddleLeft.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnBottomCenter.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnBottomCenter.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnBottomCenter.ImageIndex = 1;
			this.btnBottomCenter.ImageList = this.imageList1;
			this.btnBottomCenter.Location = new System.Drawing.Point(41, 64);
			this.btnBottomCenter.Name = "btnBottomCenter";
			this.btnBottomCenter.Size = new System.Drawing.Size(30, 23);
			this.btnBottomCenter.TabIndex = 9;
			this.btnBottomCenter.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnBottomRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnBottomRight.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnBottomRight.ImageIndex = 3;
			this.btnBottomRight.ImageList = this.imageList1;
			this.btnBottomRight.Location = new System.Drawing.Point(82, 64);
			this.btnBottomRight.Name = "btnBottomRight";
			this.btnBottomRight.Size = new System.Drawing.Size(30, 23);
			this.btnBottomRight.TabIndex = 11;
			this.btnBottomRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnBottomLeft.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnBottomLeft.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnBottomLeft.ImageIndex = 0;
			this.btnBottomLeft.ImageList = this.imageList1;
			this.btnBottomLeft.Location = new System.Drawing.Point(0, 64);
			this.btnBottomLeft.Name = "btnBottomLeft";
			this.btnBottomLeft.Size = new System.Drawing.Size(30, 23);
			this.btnBottomLeft.TabIndex = 8;
			this.btnBottomLeft.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.ClientSize = new System.Drawing.Size(103, 86);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnTopLeft,
																		  this.btnTopCenter,
																		  this.btnTopRight,
																		  this.btnMiddleRight,
																		  this.btnMiddleLeft,
																		  this.btnBottomCenter,
																		  this.btnBottomRight,
																		  this.btnBottomLeft});
			this.Name = "LegendAlignmentControl";
			this.Size = new System.Drawing.Size(112, 87);
			this.ResumeLayout(false);
		}
		#endregion
		void chbox_CheckedChanged(object sender, System.EventArgs e) {
			if (programmaticCheck)
				return;
			RadioButton btn = sender as RadioButton;
			if (btn.Checked) {
				editValue = (LegendAlignment)btn.Tag;
				edSvc.CloseDropDown();
		}
		}
		void CheckDefaultButton() {
			programmaticCheck = true;
			foreach (Control ctr in Controls) {
				RadioButton btn = ctr as RadioButton;
				if (btn != null)
					btn.Checked = editValue != null && btn.Tag == editValue;
			}
			programmaticCheck = false;
		}
	}
}
