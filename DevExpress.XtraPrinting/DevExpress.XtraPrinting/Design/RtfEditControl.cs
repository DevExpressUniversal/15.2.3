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
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraBars;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Design {
	[ToolboxItem(false)]
	public partial class RtfEditControl : UserControl {
		static byte[] fontSizeSet = { 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72 };
		string FontName { get { return (string)this.itemFontName.EditValue; } }
		float FontSize { get { return Convert.ToSingle(this.itemFontSize.EditValue); } }
		FontStyle FontStyle {
			get {
				FontStyle fontStyle = FontStyle.Regular;
				if(this.itemBold.Down)
					fontStyle |= FontStyle.Bold;
				if(this.itemItalic.Down)
					fontStyle |= FontStyle.Italic;
				if(this.itemUnderline.Down)
					fontStyle |= FontStyle.Underline;
				return fontStyle;
			}
		}
		public string Rtf { 
			get {
				return !string.IsNullOrEmpty(this.richTextBox.Text) ? 
					this.richTextBox.Rtf : string.Empty;
			} 
			set { this.richTextBox.Rtf = value; } }
		public RtfEditControl() {
			InitializeComponent();
			DevExpress.XtraEditors.FontServiceBase.FillRepositoryItemComboBox(this.cmbFontSizes, fontSizeSet);
			this.itemFontName.EditValue = "Times New Roman";
			this.itemFontSize.EditValue = 9.75;			
			this.richTextBox.Font = new System.Drawing.Font("Times New Roman", 9.75F);
			DevExpress.Utils.ImageCollection imageCollection = ImageCollectionHelper.CreateVoidImageCollection();
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.Bold_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.Italic_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.Underline_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.FontColor_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.Highlight_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.AlignLeft_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.AlignCenter_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.AlignRight_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			ImageCollectionHelper.AddImagesToCollectionFromResources(imageCollection, "DevExpress.XtraPrinting.Images.AlignJustify_16x16.png", System.Reflection.Assembly.GetExecutingAssembly());
			this.barManager.Images = imageCollection;
			InitializeColorEditor(this.itemBackColor);
			InitializeColorEditor(this.itemForeColor);
			((ColorPopupControlContainer)this.itemForeColor.DropDownControl).ResultColor = this.richTextBox.SelectionColor;
			this.itemFontName.EditValueChanged += new System.EventHandler(this.itemFont_EditValueChanged);
			this.itemFontSize.EditValueChanged += new System.EventHandler(this.itemFont_EditValueChanged);
		}
		void InitializeColorEditor(BarButtonItem barItem) {
			ColorPopupControlContainer colorPopupControlContainer = new ColorPopupControlContainer();
			colorPopupControlContainer.Visible = false;
			barItem.DropDownControl = colorPopupControlContainer;
			colorPopupControlContainer.Item = barItem;
			colorPopupControlContainer.CloseUp += new EventHandler(colorPopupControlContainer_CloseUp);
		}
		void colorPopupControlContainer_CloseUp(object sender, EventArgs e) {
			ColorPopupControlContainer colorPopupControlContainer = (ColorPopupControlContainer)sender;
			if(colorPopupControlContainer.Item == this.itemBackColor) {
				this.richTextBox.SelectionBackColor = colorPopupControlContainer.ResultColor;
			} else if(colorPopupControlContainer.Item == this.itemForeColor) {
				this.richTextBox.SelectionColor = colorPopupControlContainer.ResultColor;
			}
		}
		private void barManager_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
			if(e.Item == this.itemBold || e.Item == this.itemItalic || e.Item == this.itemUnderline) {
				SetFont(CreateFont());
			} else if(e.Item == this.itemLeft) {
				this.richTextBox.SelectionAlignmentEx = DevExpress.XtraPrinting.Native.HorizontalAlignmentEx.Left;
			} else if(e.Item == this.itemRights) {
				this.richTextBox.SelectionAlignmentEx = DevExpress.XtraPrinting.Native.HorizontalAlignmentEx.Right;
			} else if(e.Item == this.itemCenter) {
				this.richTextBox.SelectionAlignmentEx = DevExpress.XtraPrinting.Native.HorizontalAlignmentEx.Center;
			} else if(e.Item == this.itemJustify) {
				this.richTextBox.SelectionAlignmentEx = DevExpress.XtraPrinting.Native.HorizontalAlignmentEx.Justify;
			}
		}
		private void itemFont_EditValueChanged(object sender, EventArgs e) {
			SetFont(CreateFont());
		}
		void SetFont(Font font) {
			this.richTextBox.SelectionFont = font;
		}
		Font CreateFont() {
			return new Font(FontName, FontSize, FontStyle);
		}
	}
}
