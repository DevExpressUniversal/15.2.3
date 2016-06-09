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
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraLayout.Converter {
	public partial class ConvertToXtraLayoutForm : XtraForm {
		private LabelControl labelControl1;
		private ImageComboBoxEdit imageComboBoxEdit1;
		private SimpleButton simpleButton2;
		private DevExpress.Utils.ImageCollection imageCollection1;
		private IContainer components;
		private PictureEdit pictureEdit1;
		private DevExpress.Utils.ToolTipController toolTipController1;
		private SimpleButton simpleButton1;
		private LabelControl labelControl2;
		private HyperLinkEdit hyperLinkEdit1;
		protected LayoutConverter converter;
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConvertToXtraLayoutForm));
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.imageComboBoxEdit1 = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.simpleButton2 = new DevExpress.XtraEditors.SimpleButton();
			this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
			this.imageCollection1 = new DevExpress.Utils.ImageCollection(this.components);
			this.pictureEdit1 = new DevExpress.XtraEditors.PictureEdit();
			this.toolTipController1 = new DevExpress.Utils.ToolTipController(this.components);
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.hyperLinkEdit1 = new DevExpress.XtraEditors.HyperLinkEdit();
			((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.hyperLinkEdit1.Properties)).BeginInit();
			this.SuspendLayout();
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelControl1.Location = new System.Drawing.Point(12, 12);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(277, 26);
			this.labelControl1.TabIndex = 7;
			this.labelControl1.Text = "Select a container whose controls will be converted to an XtraLayout";
			this.imageComboBoxEdit1.Location = new System.Drawing.Point(12, 44);
			this.imageComboBoxEdit1.Name = "imageComboBoxEdit1";
			this.imageComboBoxEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.imageComboBoxEdit1.Size = new System.Drawing.Size(277, 20);
			this.imageComboBoxEdit1.TabIndex = 6;
			this.imageComboBoxEdit1.SelectedIndexChanged += new System.EventHandler(this.imageComboBoxEdit1_SelectedIndexChanged);
			this.simpleButton2.Location = new System.Drawing.Point(214, 70);
			this.simpleButton2.Name = "simpleButton2";
			this.simpleButton2.Size = new System.Drawing.Size(75, 23);
			this.simpleButton2.TabIndex = 5;
			this.simpleButton2.Text = "Cancel";
			this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
			this.simpleButton1.Location = new System.Drawing.Point(133, 70);
			this.simpleButton1.Name = "simpleButton1";
			this.simpleButton1.Size = new System.Drawing.Size(75, 23);
			this.simpleButton1.TabIndex = 4;
			this.simpleButton1.Text = "Convert";
			this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
			this.imageCollection1.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("imageCollection1.ImageStream")));
			this.pictureEdit1.Location = new System.Drawing.Point(111, 74);
			this.pictureEdit1.Name = "pictureEdit1";
			this.pictureEdit1.Size = new System.Drawing.Size(16, 16);
			this.pictureEdit1.TabIndex = 8;
			this.pictureEdit1.ToolTip = "A new XtraLayoutControl will be created, and the selected container's controls will be added to it. The size and position of controls will remain as close to the original layout as possible, although they may not match exactly due to the controls arrangement algorithm used in the XtraLayout";
			this.pictureEdit1.ToolTipController = this.toolTipController1;
			this.pictureEdit1.ToolTipIconType = DevExpress.Utils.ToolTipIconType.Information;
			this.pictureEdit1.ToolTipTitle = "Note";
			this.toolTipController1.AutoPopDelay = 15000;
			this.labelControl2.Appearance.Options.UseTextOptions = true;
			this.labelControl2.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
			this.labelControl2.Location = new System.Drawing.Point(12, 99);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(277, 26);
			this.labelControl2.TabIndex = 9;
			this.labelControl2.Text = "Warning: all localized property values of the container\'s labels, groups and pane" +
	"ls will be lost";
			this.hyperLinkEdit1.EditValue = "T125874";
			this.hyperLinkEdit1.Location = new System.Drawing.Point(191, 110);
			this.hyperLinkEdit1.Name = "hyperLinkEdit1";
			this.hyperLinkEdit1.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.hyperLinkEdit1.Properties.Appearance.Options.UseBackColor = true;
			this.hyperLinkEdit1.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.hyperLinkEdit1.Size = new System.Drawing.Size(52, 18);
			this.hyperLinkEdit1.TabIndex = 10;
			this.hyperLinkEdit1.OpenLink += new DevExpress.XtraEditors.Controls.OpenLinkEventHandler(this.hyperLinkEdit1_OpenLink);
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
			this.ClientSize = new System.Drawing.Size(294, 137);
			this.Controls.Add(this.hyperLinkEdit1);
			this.Controls.Add(this.labelControl2);
			this.Controls.Add(this.pictureEdit1);
			this.Controls.Add(this.labelControl1);
			this.Controls.Add(this.imageComboBoxEdit1);
			this.Controls.Add(this.simpleButton2);
			this.Controls.Add(this.simpleButton1);
			this.Name = "ConvertToXtraLayoutForm";
			this.Text = " XtraLayout Converter";
			this.Load += new System.EventHandler(this.ConvertToXtraLayoutForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.imageComboBoxEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.imageCollection1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.hyperLinkEdit1.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		public ConvertToXtraLayoutForm(LayoutConverter converter) {
			base.Visible = false;
			base.ShowInTaskbar = false;
			base.MinimizeBox = false;
			base.SizeGripStyle = SizeGripStyle.Hide;
			base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.converter = converter;
			InitializeComponent();
			base.MaximumSize = Size;
			base.MinimumSize = Size;
			base.StartPosition = FormStartPosition.CenterParent;
			FillContainersList();
		}
		protected bool AllowAddToConvertList(IComponent component) {
			ContainerControl tContainer = component as ContainerControl;
			if(tContainer is PropertyGrid) return false;
			if(tContainer is SplitContainer) return false;
			if(component is SplitContainerControl) return false;
			if(component.GetType().ToString().EndsWith("DockPanel")) return false;
			if(component.GetType().ToString().EndsWith("ControlContainer")) return false;
			if(tContainer != null && tContainer.Controls.Count > 0) return true;
			Panel panel = component as Panel;
			if(panel != null && panel.Controls.Count > 0) return true;
			XtraPanel xpanel = component as XtraPanel;
			if(xpanel != null && xpanel.Controls.Count > 0) return true;
			GroupBox groupBox = component as GroupBox;
			if(groupBox != null && groupBox.Controls.Count > 0) return true;
			DevExpress.XtraTab.XtraTabControl xtab = component as DevExpress.XtraTab.XtraTabControl;
			if(xtab != null && xtab.TabPages.Count > 0) return true;
			return false;
		}
		protected void FillContainersList() {
			foreach(IComponent tComponent in converter.Container.Components) {
				if(AllowAddToConvertList(tComponent)) {
					imageComboBoxEdit1.Properties.Items.Add(new ImageComboBoxItem(tComponent.Site != null ? tComponent.Site.Name : "unknown name", tComponent));
				}
				if(imageComboBoxEdit1.Properties.Items.Count > 0) imageComboBoxEdit1.SelectedIndex = 0;
			}
		}
		protected void Convert() {
			if(imageComboBoxEdit1.SelectedIndex < 0) {
				XtraMessageBox.Show("Nothing to convert");
				return;
			}
			converter.ConvertToXtraLayout(imageComboBoxEdit1.Properties.Items[imageComboBoxEdit1.SelectedIndex].Value as Control);
			Close();
		}
		private void ConvertToXtraLayoutForm_Load(object sender, EventArgs e) {
			pictureEdit1.Image = imageCollection1.Images[2];
			imageComboBoxEdit1_SelectedIndexChanged(null, EventArgs.Empty);
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
		private void hyperLinkEdit1_OpenLink(object sender, OpenLinkEventArgs e) {
			e.EditValue = "https://www.devexpress.com/Support/Center/Question/Details/T125874";
		}
	}
}
