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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraLayout;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraLayout.Converter {
	public partial class ConverterBaseForm : XtraForm {
		private SimpleButton simpleButton1;
		private SimpleButton simpleButton2;
		private LabelControl labelControl1;
		private PictureEdit pictureEdit1;
		private IContainer components;
		private DevExpress.Utils.ImageCollection imageCollection1;
		private DevExpress.Utils.ToolTipController toolTipController1;
		private ImageComboBoxEdit imageComboBoxEdit1;
		protected LayoutConverter converter;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConverterBaseForm));
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
			this.imageComboBoxEdit1 = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			this.SuspendLayout();
			this.simpleButton1.Location = new System.Drawing.Point(133, 70);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(75, 23);
			this.simpleButton1.TabIndex = 0;
			this.simpleButton1.Text = "Convert";
			this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
			this.simpleButton2.Location = new System.Drawing.Point(214, 70);
			this.simpleButton2.Name = "simpleButton2";
			this.simpleButton2.Size = new System.Drawing.Size(75, 23);
			this.simpleButton2.TabIndex = 1;
			this.simpleButton2.Text = "Cancel";
			this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
			this.imageComboBoxEdit1.Location = new System.Drawing.Point(12, 44);
			this.imageComboBoxEdit1.Name = "imageComboBoxEdit1";
			this.imageComboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.imageComboBoxEdit1.Size = new System.Drawing.Size(277, 20);
			this.imageComboBoxEdit1.TabIndex = 2;
			this.imageComboBoxEdit1.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxEdit1_SelectedIndexChanged);
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelControl1.Location = new System.Drawing.Point(12, 12);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(277, 26);
			this.labelControl1.TabIndex = 3;
			this.labelControl1.Text = "";
			this.pictureEdit1.Location = new System.Drawing.Point(111, 75);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Size = new System.Drawing.Size(16, 16);
			this.pictureEdit1.TabIndex = 4;
			this.pictureEdit1.ToolTip = "";
			this.pictureEdit1.ToolTipController = this.toolTipController1;
			this.pictureEdit1.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
			this.pictureEdit1.ToolTipTitle = "Note";
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.toolTipController1.AutoPopDelay = 15000;
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(298, 105);
			this.Controls.Add(this.pictureEdit1);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.imageComboBoxEdit1);
			this.Controls.Add(this.simpleButton2);
			this.Controls.Add(this.simpleButton1);
			this.Name = "ConvertToStandardLayoutForm";
			this.toolTipController1.SetSuperTip(this, null);
			this.Text = " XtraLayout Converter";
			this.Load += new System.EventHandler(this.ConverterBaseForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			this.ResumeLayout(false);
		}
		private void ConverterBaseForm_Load(object sender, EventArgs e) {
			pictureEdit1.Image = imageCollection1.Images[2];
			imageComboBoxEdit1_SelectedIndexChanged(null, EventArgs.Empty);
		}
		public ConverterBaseForm(LayoutConverter converter) {
			base.Visible = false;
			base.ShowInTaskbar = false;
			base.MinimizeBox = false;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.converter = converter;
			InitializeComponent();
			this.pictureEdit1.ToolTip = GetToolTip();
			this.labelControl1.Text = GetLabelText();
			this.Name = GetName();
			base.MaximumSize = Size;
			base.MinimumSize = Size;
			base.StartPosition = FormStartPosition.CenterParent;
			FillLayoutControlsList();
		}
		protected virtual string GetName() {
			return "ConvertToStandardLayoutForm";
		}
		protected virtual string GetLabelText() {
			return "Select an XtraLayoutControl that is to be converted to a regular layout";
		}
		protected virtual string GetToolTip() {
			return "The selected XtraLayoutControl will be destroyed and its controls will be added t" +
							"o the parent control preserving their size and position";
		}
		protected void FillLayoutControlsList() {
			LayoutControl tLayout;
			foreach(IComponent tComponent in converter.Container.Components) {
				tLayout = tComponent as LayoutControl;
				if(tLayout != null)
					imageComboBoxEdit1.Properties.Items.Add(new ImageComboBoxItem(tLayout.Name, tLayout));
				if(imageComboBoxEdit1.Properties.Items.Count > 0) imageComboBoxEdit1.SelectedIndex = 0;
			}
		}
		protected virtual void Convert() {
		}
		private void simpleButton1_Click(object sender, EventArgs e) {
			Convert();
		}
		private void simpleButton2_Click(object sender, EventArgs e) {
			Close();
		}
		private void imageComboBoxEdit1_SelectedIndexChanged(object sender, EventArgs e) {
			 simpleButton1.Enabled = !(imageComboBoxEdit1.SelectedIndex < 0);
		}
		protected internal ImageComboBoxEdit ImageComboBoxEdit { get { return imageComboBoxEdit1; } }
		protected internal LayoutConverter LayoutConverter { get { return converter; } }
	}
}
