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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraVerticalGrid.Blending {
	internal class AlphaStyleEditor : DevExpress.XtraEditors.XtraForm {
		private System.ComponentModel.Container components = null;
		private DevExpress.XtraEditors.SimpleButton btnCancel;
		private DevExpress.XtraEditors.SimpleButton btnOK;
		private DevExpress.XtraEditors.SimpleButton btnOpenNewBitmap;
		private System.Windows.Forms.Panel pnlImage;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		AlphaStyles styles = null;
		AlphaStyles mainStyles = null; 
		VGridControlBase vGrid = null;
		Font defaultFont = null;
		bool isRefresh = false;
		private DevExpress.XtraEditors.ListBoxControl lbStyles;
		private TrackBarControl tbStyles;
		private DevExpress.XtraEditors.SpinEdit numStyles;
		private System.Windows.Forms.Label lblPreview;
		private System.Windows.Forms.Panel pnlPreview;
		private DevExpress.XtraEditors.SimpleButton btnApply;
		private DevExpress.XtraEditors.SimpleButton btnDefault;
		private DevExpress.XtraEditors.SimpleButton btnClearPicture;
		private DevExpress.XtraTab.XtraTabControl xtraTabControl1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage1;
		private DevExpress.XtraTab.XtraTabPage xtraTabPage2;
		IServiceProvider provider = null;
		AlphaStyleEditor() { InitializeComponent(); } 
		public AlphaStyleEditor(AlphaStyles styles, IServiceProvider provider) : this(styles, provider, null) {}
		public AlphaStyleEditor(AlphaStyles styles, IServiceProvider provider, VGridControlBase vGrid) {
			InitializeComponent();
			defaultFont = (Font)lblPreview.Font.Clone();
			this.provider = provider;
			if(vGrid == null) {
				this.vGrid = ((XtraVertGridBlending)styles.parent).VertGridControl;
				isRefresh = true;
			} else this.vGrid = vGrid;
			if(this.vGrid == null) {
				btnOpenNewBitmap.Enabled = false;
				xtraTabControl1.SelectedTabPageIndex = 1;
			} else 
				pnlImage.BackgroundImage = this.vGrid.BackgroundImage;
			btnClearPicture.Enabled = pnlImage.BackgroundImage != null;
			this.mainStyles = styles;
			this.styles = (AlphaStyles)styles.Clone();
			FillStyles();
			SetEnabled(false);
		}
		private void FillStyles() {
			if(vGrid != null) {
				string s = "";
				for(int i = 0; i < XtraVertGridBlending.defaultAlphaStyles.Length; i++) { 
					s = XtraVertGridBlending.defaultAlphaStyles[i].StyleName;
					AppearanceObject obj = vGrid.ViewInfo.PaintAppearance.GetAppearance(s);
					if(obj != null && !obj.BackColor.IsEmpty)
						lbStyles.Items.Add(s);
				}
				lbStyles.SortOrder = SortOrder.Ascending;
				lbStyles.SelectedIndex = 0;
			} else 
				tbStyles.Enabled = numStyles.Enabled = btnDefault.Enabled = false;
		}
		protected override void Dispose( bool disposing ) {
			if( disposing ) 
				if(components != null)
					components.Dispose();
			base.Dispose( disposing );
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnClearPicture = new DevExpress.XtraEditors.SimpleButton();
			this.btnOpenNewBitmap = new DevExpress.XtraEditors.SimpleButton();
			this.pnlImage = new System.Windows.Forms.Panel();
			this.btnDefault = new DevExpress.XtraEditors.SimpleButton();
			this.pnlPreview = new System.Windows.Forms.Panel();
			this.lblPreview = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.numStyles = new DevExpress.XtraEditors.SpinEdit();
			this.label1 = new System.Windows.Forms.Label();
			this.tbStyles = new DevExpress.XtraEditors.TrackBarControl();
			this.lbStyles = new DevExpress.XtraEditors.ListBoxControl();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
			this.xtraTabPage1 = new DevExpress.XtraTab.XtraTabPage();
			this.xtraTabPage2 = new DevExpress.XtraTab.XtraTabPage();
			this.pnlPreview.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numStyles.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbStyles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tbStyles.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbStyles)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).BeginInit();
			this.xtraTabControl1.SuspendLayout();
			this.xtraTabPage1.SuspendLayout();
			this.xtraTabPage2.SuspendLayout();
			this.SuspendLayout();
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(205, 250);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(68, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "&Cancel";
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(133, 250);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(68, 23);
			this.btnOK.TabIndex = 1;
			this.btnOK.Text = "&OK";
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			this.btnClearPicture.Location = new System.Drawing.Point(197, 36);
			this.btnClearPicture.Name = "btnClearPicture";
			this.btnClearPicture.Size = new System.Drawing.Size(68, 23);
			this.btnClearPicture.TabIndex = 4;
			this.btnClearPicture.Text = "C&lear";
			this.btnClearPicture.Click += new System.EventHandler(this.btnClearPicture_Click);
			this.btnOpenNewBitmap.Location = new System.Drawing.Point(197, 8);
			this.btnOpenNewBitmap.Name = "btnOpenNewBitmap";
			this.btnOpenNewBitmap.Size = new System.Drawing.Size(68, 23);
			this.btnOpenNewBitmap.TabIndex = 3;
			this.btnOpenNewBitmap.Text = "O&pen...";
			this.btnOpenNewBitmap.Click += new System.EventHandler(this.btnOpenNewBitmap_Click);
			this.pnlImage.BackColor = System.Drawing.SystemColors.Window;
			this.pnlImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pnlImage.Location = new System.Drawing.Point(8, 8);
			this.pnlImage.Name = "pnlImage";
			this.pnlImage.Size = new System.Drawing.Size(183, 201);
			this.pnlImage.TabIndex = 0;
			this.pnlImage.BackgroundImageChanged += new System.EventHandler(this.pnlImage_BackgroundImageChanged);
			this.btnDefault.Location = new System.Drawing.Point(141, 187);
			this.btnDefault.Name = "btnDefault";
			this.btnDefault.Size = new System.Drawing.Size(125, 23);
			this.btnDefault.TabIndex = 7;
			this.btnDefault.Text = "Set &Default";
			this.btnDefault.Click += new System.EventHandler(this.btnDefault_Click);
			this.pnlPreview.Controls.Add(this.lblPreview);
			this.pnlPreview.Location = new System.Drawing.Point(141, 103);
			this.pnlPreview.Name = "pnlPreview";
			this.pnlPreview.Size = new System.Drawing.Size(125, 64);
			this.pnlPreview.TabIndex = 6;
			this.lblPreview.BackColor = System.Drawing.Color.Transparent;
			this.lblPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.lblPreview.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lblPreview.Location = new System.Drawing.Point(0, 0);
			this.lblPreview.Name = "lblPreview";
			this.lblPreview.Size = new System.Drawing.Size(125, 64);
			this.lblPreview.TabIndex = 5;
			this.lblPreview.Text = "preview";
			this.lblPreview.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.label2.BackColor = System.Drawing.Color.Transparent;
			this.label2.Location = new System.Drawing.Point(141, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(56, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Preview:";
			this.numStyles.EditValue = new decimal(new int[] {
			0,
			0,
			0,
			0});
			this.numStyles.Location = new System.Drawing.Point(197, 55);
			this.numStyles.Name = "numStyles";
			this.numStyles.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.numStyles.Properties.IsFloatValue = false;
			this.numStyles.Properties.Mask.EditMask = "N00";
			this.numStyles.Properties.MaxValue = new decimal(new int[] {
			255,
			0,
			0,
			0});
			this.numStyles.Properties.ValidateOnEnterKey = true;
			this.numStyles.Size = new System.Drawing.Size(68, 20);
			this.numStyles.TabIndex = 6;
			this.numStyles.EditValueChanged += new System.EventHandler(this.numStyles_ValueChanged);
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.Location = new System.Drawing.Point(141, 59);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(49, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Alpha :";
			this.tbStyles.EditValue = null;
			this.tbStyles.Location = new System.Drawing.Point(138, 4);
			this.tbStyles.Name = "tbStyles";
			this.tbStyles.Properties.Maximum = 255;
			this.tbStyles.Properties.TickFrequency = 15;
			this.tbStyles.Size = new System.Drawing.Size(128, 45);
			this.tbStyles.TabIndex = 5;
			this.tbStyles.ValueChanged += new System.EventHandler(this.tbStyles_ValueChanged);
			this.lbStyles.ItemHeight = 16;
			this.lbStyles.Location = new System.Drawing.Point(8, 8);
			this.lbStyles.Name = "lbStyles";
			this.lbStyles.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
			this.lbStyles.Size = new System.Drawing.Size(124, 202);
			this.lbStyles.TabIndex = 4;
			this.lbStyles.SelectedIndexChanged += new System.EventHandler(this.lbStyles_SelectedIndexChanged);
			this.btnApply.Location = new System.Drawing.Point(61, 250);
			this.btnApply.Name = "btnApply";
			this.btnApply.Size = new System.Drawing.Size(68, 23);
			this.btnApply.TabIndex = 0;
			this.btnApply.Text = "&Apply";
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
			this.xtraTabControl1.Name = "xtraTabControl1";
			this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
			this.xtraTabControl1.Size = new System.Drawing.Size(278, 244);
			this.xtraTabControl1.TabIndex = 4;
			this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.xtraTabPage1,
			this.xtraTabPage2});
			this.xtraTabPage1.Controls.Add(this.btnOpenNewBitmap);
			this.xtraTabPage1.Controls.Add(this.btnClearPicture);
			this.xtraTabPage1.Controls.Add(this.pnlImage);
			this.xtraTabPage1.Name = "xtraTabPage1";
			this.xtraTabPage1.Size = new System.Drawing.Size(272, 218);
			this.xtraTabPage1.Text = "Background Bitmap";
			this.xtraTabPage2.Controls.Add(this.numStyles);
			this.xtraTabPage2.Controls.Add(this.lbStyles);
			this.xtraTabPage2.Controls.Add(this.label2);
			this.xtraTabPage2.Controls.Add(this.tbStyles);
			this.xtraTabPage2.Controls.Add(this.btnDefault);
			this.xtraTabPage2.Controls.Add(this.pnlPreview);
			this.xtraTabPage2.Controls.Add(this.label1);
			this.xtraTabPage2.Name = "xtraTabPage2";
			this.xtraTabPage2.Size = new System.Drawing.Size(272, 218);
			this.xtraTabPage2.Text = "AlphaStyles Collection";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(278, 281);
			this.Controls.Add(this.xtraTabControl1);
			this.Controls.Add(this.btnApply);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.MaximizeBox = false;
			this.Name = "AlphaStyleEditor";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "AlphaStyle Editor";
			this.pnlPreview.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numStyles.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbStyles.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tbStyles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbStyles)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.xtraTabControl1)).EndInit();
			this.xtraTabControl1.ResumeLayout(false);
			this.xtraTabPage1.ResumeLayout(false);
			this.xtraTabPage2.ResumeLayout(false);
			this.xtraTabPage2.PerformLayout();
			this.ResumeLayout(false);
		}
		#endregion
		private void btnOpenNewBitmap_Click(object sender, System.EventArgs e) {
			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Filter = "All image files(*.bmp, *.gif, *.jpg, *.jpeg, *.png, *.ico, *.emf, *.wmf)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.ico;*.emf;*.wmf|" +
				"Bitmap files(*.bmp, *.gif, *.jpg, *.jpeg, *.png, *.ico)|*.bmp;*.gif;*.jpg;*.jpeg;*.png;*.ico|" + 
				"Metafiles(*.emf, *.wmf)|*.emf;*.wmf";
			if(dlg.ShowDialog() == DialogResult.OK) 
				try {
					SetEnabled(true);
					pnlImage.BackgroundImage = Image.FromFile(dlg.FileName);
				} catch {}
		}
		private void ChangePreview() {
			lblPreview.Font = PreviewFont(lbStyles);
			lblPreview.BackColor = PreviewBackColor(lbStyles);
			lblPreview.ForeColor = PreviewForeColor(lbStyles);
		}
		private void tbStyles_ValueChanged(object sender, System.EventArgs e) {
			numStyles.Value = tbStyles.Value;
			if(!lockUpdate) {
				ChangePreview();
				numStyles.BackColor = SystemColors.Window;
				SetValues(lbStyles, tbStyles.Value);
				SetEnabled(true);
			}
		}
		private void numStyles_ValueChanged(object sender, System.EventArgs e) {
			if(!lockUpdate) {
				tbStyles.Value = (int)numStyles.Value;
				SetValues(lbStyles, tbStyles.Value);
				SetEnabled(true);
			}
		}
		bool lockUpdate = false;
		private void lbStyles_SelectedIndexChanged(object sender, System.EventArgs e) {
			if(lockStyles) return;
			tbStyles.Enabled = numStyles.Enabled = lbStyles.SelectedIndices.Count != 0;
			int n = SelectedValue(lbStyles);
			lockUpdate = true;
			numStyles.BackColor = (n == -1 ? SystemColors.Control : SystemColors.Window); 
			tbStyles.Value = (n == -1 ? 0 : n);
			ChangePreview();
			lockUpdate = false;
		}
		private int SelectedValue(DevExpress.XtraEditors.ListBoxControl lb) {
			int ret = -1;
			for(int i = 0; i < lb.SelectedIndices.Count; i++) {
				int n = (int)styles[lb.GetItem(lb.SelectedIndices[i])];
				if(ret == -1) ret = n;
				if(ret != n) return -1;
			}
			return ret;
		}
		private void SetValues(DevExpress.XtraEditors.ListBoxControl lb, int val) {
			for(int i = 0; i < lb.SelectedIndices.Count; i++) 
				styles[lb.GetItem(lb.SelectedIndices[i])] = val;
		}
		private Font PreviewFont(DevExpress.XtraEditors.ListBoxControl lb) {
			if(vGrid == null || lb.SelectedIndices.Count != 1)
				return defaultFont;
			else 
				return (Font)vGrid.ViewInfo.PaintAppearance.GetAppearance(lb.SelectedItem.ToString()).Font.Clone();
		}
		private Color PreviewBackColor(DevExpress.XtraEditors.ListBoxControl lb) {
			if(vGrid == null || lb.SelectedIndices.Count != 1)
				return Color.FromArgb(tbStyles.Value, SystemColors.Window);
			else 
				return Color.FromArgb(tbStyles.Value, vGrid.ViewInfo.PaintAppearance.GetAppearance(lb.SelectedItem.ToString()).BackColor);
		}
		private Color PreviewForeColor(DevExpress.XtraEditors.ListBoxControl lb) {
			if(vGrid == null || lb.SelectedIndices.Count != 1)
				return SystemColors.WindowText;
			else 
				return vGrid.ViewInfo.PaintAppearance.GetAppearance(lb.SelectedItem.ToString()).ForeColor;
		}
		private void btnOK_Click(object sender, System.EventArgs e) {
			Apply();
		}
		private void Apply() {
			if(((XtraVertGridBlending)mainStyles.parent).VertGridControl != null) 
				((XtraVertGridBlending)mainStyles.parent).VertGridControl.BackgroundImage = pnlImage.BackgroundImage;
			foreach(DictionaryEntry entry in styles) 
				mainStyles[entry.Key] = entry.Value;
			if(provider != null) { 
				IComponentChangeService srv = provider.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
				if(srv != null) srv.OnComponentChanged(mainStyles.parent, null, null, null);
			} else {
				((XtraVertGridBlending)styles.parent).RefreshStyles();
			}
			if(!isRefresh) {
				((XtraVertGridBlending)styles.parent).RefreshStyles();
				vGrid.BackgroundImage = pnlImage.BackgroundImage;
			}
			SetEnabled(false);
		}
		private void SetEnabled(bool isEnabled) {
			btnOK.Enabled = btnApply.Enabled = isEnabled;
			btnCancel.Text = isEnabled ? "Cancel" : "Close";
		}
		private void pnlImage_BackgroundImageChanged(object sender, System.EventArgs e) {
			pnlPreview.BackgroundImage = pnlImage.BackgroundImage;
			btnClearPicture.Enabled = pnlImage.BackgroundImage != null;
		}
		private void btnApply_Click(object sender, System.EventArgs e) {
			Apply();
		}
		private bool lockStyles = false;
		private void btnDefault_Click(object sender, System.EventArgs e) {
			XtraVertGridBlending.FillAlphaStyles(styles, XtraVertGridBlending.defaultAlphaStyles);
			lockStyles = true;
			lbStyles.SelectedItem = null;
			lockStyles = false;
			lbStyles.SelectedIndex = 0;
			SetEnabled(true);
		}
		private void btnClearPicture_Click(object sender, System.EventArgs e) {
			pnlImage.BackgroundImage = null;
			SetEnabled(true);
		}
	}
	internal class UIAlphaStyleEditor : UITypeEditor {
		private IWindowsFormsEditorService edSvc = null;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object objValue) {
			if (context != null && context.Instance != null	&& provider != null) {
				edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));	
				if (edSvc != null) {
					try {
						AlphaStyleEditor editor = new AlphaStyleEditor((AlphaStyles)objValue, provider);
						edSvc.ShowDialog(editor);
					} catch {}
				}
			}
			return objValue;
		}
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			if (context != null && context.Instance != null) {
				return UITypeEditorEditStyle.Modal;
			}
			return base.GetEditStyle(context);
		}
	}
}
