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
using System.Windows.Forms.Design;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting;
using DevExpress.Utils;
using DevExpress.XtraReports.UserDesigner;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public class TextAlignmentEdit : System.Windows.Forms.Control
	{
		private System.Windows.Forms.RadioButton btnTopLeft;
		private System.Windows.Forms.RadioButton btnTopCenter;
		private System.Windows.Forms.RadioButton btnTopJustify;
		private System.Windows.Forms.RadioButton btnTopRight;
		private System.Windows.Forms.RadioButton btnMiddleCenter;
		private System.Windows.Forms.RadioButton btnMiddleRight;
		private System.Windows.Forms.RadioButton btnMiddleLeft;
		private System.Windows.Forms.RadioButton btnMiddleJustify;
		private System.Windows.Forms.RadioButton btnBottomCenter;
		private System.Windows.Forms.RadioButton btnBottomRight;
		private System.Windows.Forms.RadioButton btnBottomLeft;
		private System.Windows.Forms.RadioButton btnBottomJustify;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList imageList1;
		private object editValue;
		private IWindowsFormsEditorService edSvc;
		private bool programmaticCheck;
		public object Value { get { return editValue; } }
		public TextAlignmentEdit()
		{
			InitializeComponent();
			ResourceImageHelper.FillImageListFromResources(imageList1, "Images.TextAlignment.bmp", typeof(DevExpress.Utils.UI.ResFinder));
			this.BackColor = System.Drawing.SystemColors.Control;
			btnTopLeft.Tag = TextAlignment.TopLeft;
			btnTopCenter.Tag = TextAlignment.TopCenter;
			btnTopJustify.Tag = TextAlignment.TopJustify;
			btnTopRight.Tag = TextAlignment.TopRight;
			btnMiddleLeft.Tag = TextAlignment.MiddleLeft;
			btnMiddleCenter.Tag = TextAlignment.MiddleCenter;
			btnMiddleJustify.Tag = TextAlignment.MiddleJustify;
			btnMiddleRight.Tag = TextAlignment.MiddleRight;
			btnBottomLeft.Tag = TextAlignment.BottomLeft;
			btnBottomCenter.Tag = TextAlignment.BottomCenter;
			btnBottomJustify.Tag = TextAlignment.BottomJustify;
			btnBottomRight.Tag = TextAlignment.BottomRight;
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
			this.btnTopJustify = new System.Windows.Forms.RadioButton();
			this.btnTopRight = new System.Windows.Forms.RadioButton();
			this.btnMiddleCenter = new System.Windows.Forms.RadioButton();
			this.btnMiddleRight = new System.Windows.Forms.RadioButton();
			this.btnMiddleLeft = new System.Windows.Forms.RadioButton();
			this.btnMiddleJustify = new System.Windows.Forms.RadioButton();
			this.btnBottomCenter = new System.Windows.Forms.RadioButton();
			this.btnBottomRight = new System.Windows.Forms.RadioButton();
			this.btnBottomLeft = new System.Windows.Forms.RadioButton();
			this.btnBottomJustify = new System.Windows.Forms.RadioButton();
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
			this.btnTopJustify.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnTopJustify.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnTopJustify.ImageIndex = 2;
			this.btnTopJustify.ImageList = this.imageList1;
			this.btnTopJustify.Location = new System.Drawing.Point(73, 0);
			this.btnTopJustify.Name = "btnTopJustify";
			this.btnTopJustify.Size = new System.Drawing.Size(30, 23);
			this.btnTopJustify.TabIndex = 2;
			this.btnTopJustify.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnTopRight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnTopRight.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnTopRight.ImageIndex = 3;
			this.btnTopRight.ImageList = this.imageList1;
			this.btnTopRight.Location = new System.Drawing.Point(114, 0);
			this.btnTopRight.Name = "btnTopRight";
			this.btnTopRight.Size = new System.Drawing.Size(30, 23);
			this.btnTopRight.TabIndex = 3;
			this.btnTopRight.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnMiddleCenter.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnMiddleCenter.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnMiddleCenter.ImageIndex = 1;
			this.btnMiddleCenter.ImageList = this.imageList1;
			this.btnMiddleCenter.Location = new System.Drawing.Point(41, 32);
			this.btnMiddleCenter.Name = "btnMiddleCenter";
			this.btnMiddleCenter.Size = new System.Drawing.Size(30, 23);
			this.btnMiddleCenter.TabIndex = 5;
			this.btnMiddleCenter.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.btnMiddleRight.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.btnMiddleRight.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnMiddleRight.ImageIndex = 3;
			this.btnMiddleRight.ImageList = this.imageList1;
			this.btnMiddleRight.Location = new System.Drawing.Point(114, 32);
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
			this.btnMiddleJustify.Anchor = System.Windows.Forms.AnchorStyles.None;
			this.btnMiddleJustify.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnMiddleJustify.ImageIndex = 2;
			this.btnMiddleJustify.ImageList = this.imageList1;
			this.btnMiddleJustify.Location = new System.Drawing.Point(73, 32);
			this.btnMiddleJustify.Name = "btnMiddleJustify";
			this.btnMiddleJustify.Size = new System.Drawing.Size(30, 23);
			this.btnMiddleJustify.TabIndex = 6;
			this.btnMiddleJustify.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
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
			this.btnBottomRight.Location = new System.Drawing.Point(114, 64);
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
			this.btnBottomJustify.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.btnBottomJustify.Appearance = System.Windows.Forms.Appearance.Button;
			this.btnBottomJustify.ImageIndex = 2;
			this.btnBottomJustify.ImageList = this.imageList1;
			this.btnBottomJustify.Location = new System.Drawing.Point(73, 64);
			this.btnBottomJustify.Name = "btnBottomJustify";
			this.btnBottomJustify.Size = new System.Drawing.Size(30, 23);
			this.btnBottomJustify.TabIndex = 10;
			this.btnBottomJustify.CheckedChanged += new System.EventHandler(this.chbox_CheckedChanged);
			this.ClientSize = new System.Drawing.Size(144, 86);
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.btnTopLeft,
																		  this.btnTopCenter,
																		  this.btnTopJustify,
																		  this.btnTopRight,
																		  this.btnMiddleCenter,
																		  this.btnMiddleRight,
																		  this.btnMiddleLeft,
																		  this.btnMiddleJustify,
																		  this.btnBottomCenter,
																		  this.btnBottomRight,
																		  this.btnBottomLeft,
																		  this.btnBottomJustify});
			this.Name = "XRTextAlignmentEditorForm";
			this.Size = new System.Drawing.Size(144, 86);
			this.ResumeLayout(false);
		}
		#endregion
		void chbox_CheckedChanged(object sender, System.EventArgs e) {
			if (programmaticCheck)
				return;
			RadioButton btn = sender as RadioButton;
			if (btn.Checked) {
				editValue = (TextAlignment)btn.Tag;
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
