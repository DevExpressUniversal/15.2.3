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

using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Design {
	partial class RtfEditControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RtfEditControl));
			this.barManager = new DevExpress.XtraBars.BarManager(this.components);
			this.bar = new DevExpress.XtraBars.Bar();
			this.itemFontName = new DevExpress.XtraBars.BarEditItem();
			this.cmbFontNames = new DevExpress.XtraEditors.Repository.RepositoryItemFontEdit();
			this.itemFontSize = new DevExpress.XtraBars.BarEditItem();
			this.cmbFontSizes = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.itemBold = new DevExpress.XtraBars.BarButtonItem();
			this.itemItalic = new DevExpress.XtraBars.BarButtonItem();
			this.itemUnderline = new DevExpress.XtraBars.BarButtonItem();
			this.itemForeColor = new DevExpress.XtraBars.BarButtonItem();
			this.itemBackColor = new DevExpress.XtraBars.BarButtonItem();
			this.itemLeft = new DevExpress.XtraBars.BarButtonItem();
			this.itemCenter = new DevExpress.XtraBars.BarButtonItem();
			this.itemRights = new DevExpress.XtraBars.BarButtonItem();
			this.itemJustify = new DevExpress.XtraBars.BarButtonItem();
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.richTextBox = new DevExpress.XtraPrinting.Native.RichTextBoxEx();
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbFontNames)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbFontSizes)).BeginInit();
			this.SuspendLayout();
			this.barManager.Bars.AddRange(new DevExpress.XtraBars.Bar[] {
			this.bar});
			this.barManager.DockControls.Add(this.barDockControlTop);
			this.barManager.DockControls.Add(this.barDockControlBottom);
			this.barManager.DockControls.Add(this.barDockControlLeft);
			this.barManager.DockControls.Add(this.barDockControlRight);
			this.barManager.Form = this;
			this.barManager.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.itemFontName,
			this.itemFontSize,
			this.itemBold,
			this.itemItalic,
			this.itemUnderline,
			this.itemForeColor,
			this.itemBackColor,
			this.itemLeft,
			this.itemCenter,
			this.itemRights,
			this.itemJustify});
			this.barManager.MaxItemId = 69;
			this.barManager.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.cmbFontNames,
			this.cmbFontSizes});
			this.barManager.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.barManager_ItemClick);
			this.bar.BarName = "Formatting Toolbar";
			this.bar.CanDockStyle = DevExpress.XtraBars.BarCanDockStyle.Top;
			this.bar.DockCol = 0;
			this.bar.DockRow = 0;
			this.bar.DockStyle = DevExpress.XtraBars.BarDockStyle.Top;
			this.bar.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[] {
			new DevExpress.XtraBars.LinkPersistInfo(this.itemFontName),
			new DevExpress.XtraBars.LinkPersistInfo(DevExpress.XtraBars.BarLinkUserDefines.Width, this.itemFontSize, "", false, true, true, 55),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemBold),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemItalic),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemUnderline),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemForeColor, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemBackColor),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemLeft, true),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemCenter),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemRights),
			new DevExpress.XtraBars.LinkPersistInfo(this.itemJustify)});
			this.bar.Text = "Formatting Toolbar";
			this.itemFontName.Caption = "Font Name";
			this.itemFontName.Edit = this.cmbFontNames;
			this.itemFontName.Hint = "Font Name";
			this.itemFontName.Id = 0;
			this.itemFontName.Name = "itemFontName";
			this.itemFontName.Width = 120;
			this.cmbFontNames.AutoHeight = false;
			this.cmbFontNames.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbFontNames.DropDownRows = 12;
			this.cmbFontNames.Name = "cmbFontNames";
			this.itemFontSize.Caption = "Font Size";
			this.itemFontSize.Edit = this.cmbFontSizes;
			this.itemFontSize.Hint = "Font Size";
			this.itemFontSize.Id = 1;
			this.itemFontSize.Name = "itemFontSize";
			this.itemFontSize.Width = 55;
			this.cmbFontSizes.AutoHeight = false;
			this.cmbFontSizes.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbFontSizes.Name = "cmbFontSizes";
			this.itemBold.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.itemBold.Caption = "&Bold";
			this.itemBold.Hint = "Make the font bold";
			this.itemBold.Id = 2;
			this.itemBold.ImageIndex = 0;
			this.itemBold.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.B));
			this.itemBold.Name = "itemBold";
			this.itemItalic.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.itemItalic.Caption = "&Italic";
			this.itemItalic.Hint = "Make the font italic";
			this.itemItalic.Id = 3;
			this.itemItalic.ImageIndex = 1;
			this.itemItalic.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.I));
			this.itemItalic.Name = "itemItalic";
			this.itemUnderline.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.Check;
			this.itemUnderline.Caption = "&Underline";
			this.itemUnderline.Hint = "Underline the font";
			this.itemUnderline.Id = 4;
			this.itemUnderline.ImageIndex = 2;
			this.itemUnderline.ItemShortcut = new DevExpress.XtraBars.BarShortcut((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.U));
			this.itemUnderline.Name = "itemUnderline";
			this.itemForeColor.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.itemForeColor.Caption = "For&eground Color";
			this.itemForeColor.ImageIndex = 3;
			this.itemForeColor.Hint = "Set the foreground color of the control";
			this.itemForeColor.Id = 5;
			this.itemForeColor.Name = "itemForeColor";
			this.itemBackColor.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.itemBackColor.Caption = "Bac&kground Color";
			this.itemBackColor.ImageIndex = 4;
			this.itemBackColor.Hint = "Set the background color of the control";
			this.itemBackColor.Id = 6;
			this.itemBackColor.Name = "itemBackColor";
			this.itemLeft.Caption = "&Left";
			this.itemLeft.Hint = "Align the control\'s text to the left";
			this.itemLeft.Id = 7;
			this.itemLeft.ImageIndex = 5;
			this.itemLeft.Name = "itemLeft";
			this.itemCenter.Caption = "&Center";
			this.itemCenter.Hint = "Align the control\'s text to the center";
			this.itemCenter.Id = 8;
			this.itemCenter.ImageIndex = 6;
			this.itemCenter.Name = "itemCenter";
			this.itemRights.Caption = "&Rights";
			this.itemRights.Hint = "Align the control\'s text to the right";
			this.itemRights.Id = 9;
			this.itemRights.ImageIndex = 7;
			this.itemRights.Name = "itemRights";
			this.itemJustify.Caption = "&Justify";
			this.itemJustify.Hint = "Justify the control\'s text";
			this.itemJustify.Id = 10;
			this.itemJustify.ImageIndex = 8;
			this.itemJustify.Name = "itemJustify";
			this.richTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richTextBox.Location = new System.Drawing.Point(0, 26);
			this.richTextBox.Name = "richTextBox";
			this.richTextBox.SelectionAlignmentEx = DevExpress.XtraPrinting.Native.HorizontalAlignmentEx.Left;
			this.richTextBox.Size = new System.Drawing.Size(613, 284);
			this.richTextBox.TabIndex = 4;
			this.richTextBox.Text = "";
			this.imageList.ColorDepth = System.Windows.Forms.ColorDepth.Depth8Bit;
			this.imageList.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.richTextBox);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.Name = "RtfEditControl";
			this.Size = new System.Drawing.Size(613, 310);
			((System.ComponentModel.ISupportInitialize)(this.barManager)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbFontNames)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbFontSizes)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraBars.BarManager barManager;
		private DevExpress.XtraBars.BarButtonItem itemForeColor;
		private DevExpress.XtraBars.BarButtonItem itemBackColor;
		private DevExpress.XtraBars.BarButtonItem itemBold;
		private DevExpress.XtraBars.BarButtonItem itemItalic;
		private DevExpress.XtraBars.BarButtonItem itemUnderline;
		private DevExpress.XtraBars.BarButtonItem itemLeft;
		private DevExpress.XtraBars.BarButtonItem itemCenter;
		private DevExpress.XtraBars.BarButtonItem itemRights;
		private DevExpress.XtraBars.BarButtonItem itemJustify;
		private DevExpress.XtraBars.Bar bar;
		private DevExpress.XtraBars.BarEditItem itemFontName;
		private DevExpress.XtraEditors.Repository.RepositoryItemFontEdit cmbFontNames;
		private DevExpress.XtraBars.BarEditItem itemFontSize;
		private DevExpress.XtraEditors.Repository.RepositoryItemComboBox cmbFontSizes;
		private DevExpress.XtraBars.BarDockControl barDockControlTop;
		private DevExpress.XtraBars.BarDockControl barDockControlBottom;
		private DevExpress.XtraBars.BarDockControl barDockControlLeft;
		private DevExpress.XtraBars.BarDockControl barDockControlRight;
		private RichTextBoxEx richTextBox;
		private ImageList imageList;
	}
}
