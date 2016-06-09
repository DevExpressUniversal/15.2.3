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
using System.Drawing.Text;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraCharts.Design {
	public class FontEditControl : DevExpress.XtraEditors.XtraUserControl {
		private DevExpress.XtraEditors.SplitContainerControl splitContainerControl;
		private System.Windows.Forms.Label lblFont;
		private DevExpress.XtraEditors.ImageListBoxControl lbFonts;
		private System.Windows.Forms.Label lblFontStyle;
		private System.Windows.Forms.Label lblSize;
		private DevExpress.XtraEditors.SpinEdit txtSize;
		private DevExpress.XtraEditors.CheckedListBoxControl lbStyles;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ImageList fontsImageList;
		public Font EditedFont { 
			get { 
				if (lbFonts.SelectedIndex == -1)
					return null;
				FontStyle style = FontStyle.Regular;
				if (lbStyles.Items[0].CheckState == CheckState.Checked)
					style |= FontStyle.Bold;
				if (lbStyles.Items[1].CheckState == CheckState.Checked)
					style |= FontStyle.Italic;
				if (lbStyles.Items[2].CheckState == CheckState.Checked)
					style |= FontStyle.Strikeout;
				if (lbStyles.Items[3].CheckState == CheckState.Checked)
					style |= FontStyle.Underline;
				return new Font((string)((ImageListBoxItem)lbFonts.SelectedItem).Value, Convert.ToSingle(txtSize.EditValue), style);
			} 
			set { 
				if (value == null) {
					lbFonts.SelectedIndex = -1;
				}
				else {
					txtSize.EditValue = value.Size;
					lbStyles.Items[0].CheckState = (value.Style & FontStyle.Bold) == FontStyle.Bold ? CheckState.Checked : CheckState.Unchecked;
					lbStyles.Items[1].CheckState = (value.Style & FontStyle.Italic) == FontStyle.Italic? CheckState.Checked : CheckState.Unchecked;
					lbStyles.Items[2].CheckState = (value.Style & FontStyle.Strikeout) == FontStyle.Strikeout ? CheckState.Checked : CheckState.Unchecked;
					lbStyles.Items[3].CheckState = (value.Style & FontStyle.Underline) == FontStyle.Underline ? CheckState.Checked : CheckState.Unchecked;
					foreach (ImageListBoxItem item in lbFonts.Items)
						if ((string)item.Value == value.Name) {
							lbFonts.SelectedItem = item;
							return;
						}
					lbFonts.SelectedIndex = 0;
				}
			} 
		}
		public event EventHandler OnNeedClose;
		public FontEditControl(){
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
		#region Component Designer generated code
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FontEditControl));
			this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
			this.lbFonts = new DevExpress.XtraEditors.ImageListBoxControl();
			this.fontsImageList = new System.Windows.Forms.ImageList(this.components);
			this.lblFont = new System.Windows.Forms.Label();
			this.lblSize = new System.Windows.Forms.Label();
			this.txtSize = new DevExpress.XtraEditors.SpinEdit();
			this.lbStyles = new DevExpress.XtraEditors.CheckedListBoxControl();
			this.lblFontStyle = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).BeginInit();
			this.splitContainerControl.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lbFonts)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.txtSize.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbStyles)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.splitContainerControl, "splitContainerControl");
			this.splitContainerControl.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.Panel2;
			this.splitContainerControl.Name = "splitContainerControl";
			this.splitContainerControl.Panel1.Controls.Add(this.lbFonts);
			this.splitContainerControl.Panel1.Controls.Add(this.lblFont);
			this.splitContainerControl.Panel2.Controls.Add(this.lblSize);
			this.splitContainerControl.Panel2.Controls.Add(this.txtSize);
			this.splitContainerControl.Panel2.Controls.Add(this.lbStyles);
			this.splitContainerControl.Panel2.Controls.Add(this.lblFontStyle);
			this.splitContainerControl.SplitterPosition = 80;
			resources.ApplyResources(this.lbFonts, "lbFonts");
			this.lbFonts.ImageList = this.fontsImageList;
			this.lbFonts.Name = "lbFonts";
			this.lbFonts.DoubleClick += new System.EventHandler(this.lbFonts_DoubleClick);
			this.lbFonts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbFonts_KeyDown);
			this.fontsImageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			resources.ApplyResources(this.fontsImageList, "fontsImageList");
			this.fontsImageList.TransparentColor = System.Drawing.Color.Transparent;
			resources.ApplyResources(this.lblFont, "lblFont");
			this.lblFont.Name = "lblFont";
			resources.ApplyResources(this.lblSize, "lblSize");
			this.lblSize.Name = "lblSize";
			resources.ApplyResources(this.txtSize, "txtSize");
			this.txtSize.Name = "txtSize";
			this.txtSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.txtSize.Properties.Mask.EditMask = resources.GetString("txtSize.Properties.Mask.EditMask");
			this.txtSize.Properties.MaxValue = new decimal(new int[] {
			50,
			0,
			0,
			0});
			this.txtSize.Properties.MinValue = new decimal(new int[] {
			6,
			0,
			0,
			0});
			this.txtSize.Properties.ValidateOnEnterKey = true;
			this.txtSize.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtSize_KeyDown);
			resources.ApplyResources(this.lbStyles, "lbStyles");
			this.lbStyles.CheckOnClick = true;
			this.lbStyles.Items.AddRange(new DevExpress.XtraEditors.Controls.CheckedListBoxItem[] {
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(resources.GetString("lbStyles.Items")),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(resources.GetString("lbStyles.Items1")),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(resources.GetString("lbStyles.Items2")),
			new DevExpress.XtraEditors.Controls.CheckedListBoxItem(resources.GetString("lbStyles.Items3"))});
			this.lbStyles.Name = "lbStyles";
			this.lbStyles.KeyDown += new System.Windows.Forms.KeyEventHandler(this.lbStyles_KeyDown);
			resources.ApplyResources(this.lblFontStyle, "lblFontStyle");
			this.lblFontStyle.Name = "lblFontStyle";
			this.Controls.Add(this.splitContainerControl);
			this.Name = "FontEditControl";
			resources.ApplyResources(this, "$this");
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl)).EndInit();
			this.splitContainerControl.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lbFonts)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.txtSize.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbStyles)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public void SetLookAndFeel(UserLookAndFeel lookAndFeel) {
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			LookAndFeel.UseDefaultLookAndFeel = false;
			splitContainerControl.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			lbFonts.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			txtSize.LookAndFeel.ParentLookAndFeel = lookAndFeel;
			lbStyles.LookAndFeel.ParentLookAndFeel = lookAndFeel;
		}
		public void FillFonts() {
			if (lbFonts.ItemCount > 0)
				return;
			Cursor currentCursor = Cursor.Current;
			try {
				Cursor.Current = Cursors.WaitCursor;
				Rectangle rect = new Rectangle(0, 0, fontsImageList.ImageSize.Width, 
					fontsImageList.ImageSize.Height);
				lbFonts.ImageList = fontsImageList;
				int imageIndex = 0;
				FontFamily[] families = FontFamily.Families;
				using (StringFormat sf = (StringFormat)StringFormat.GenericDefault.Clone()) {
					sf.Alignment = StringAlignment.Near;
					sf.LineAlignment = StringAlignment.Near;
					foreach (FontFamily family in families)
						try {
							using (Font font = new Font(family.Name, 12)) {
								Image image = new Bitmap(fontsImageList.ImageSize.Width, fontsImageList.ImageSize.Height);
								try {
									using (Graphics g = Graphics.FromImage(image)) {
										g.TextRenderingHint = TextRenderingHint.AntiAlias;
										g.FillRectangle(Brushes.White, rect);
										g.DrawString("abc", font, Brushes.Black, rect, sf);
									}
									fontsImageList.Images.Add(image);
								}
								catch {
									image.Dispose();
									throw;
								}
								lbFonts.Items.Add(font.Name, imageIndex++);
							}
						}
						catch {
						}	
				}	
			}
			finally {
				Cursor.Current = currentCursor;
			}
		}
		void RaiseNeedClose(object sender) {
			if (OnNeedClose != null) 
				OnNeedClose(sender, EventArgs.Empty);
		}
		private void lbFonts_DoubleClick(object sender, System.EventArgs e) {
			RaiseNeedClose(this);
		}
		private void lbFonts_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter)
				RaiseNeedClose(this);
		}
		private void txtSize_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter && Validate())
				RaiseNeedClose(this);
		}
		private void lbStyles_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e) {
			if (e.KeyCode == Keys.Enter)
				RaiseNeedClose(this);
		}
	}
}
