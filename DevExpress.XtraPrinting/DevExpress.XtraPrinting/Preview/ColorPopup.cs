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
using System.Windows.Forms;
using DevExpress.XtraBars;
using System.ComponentModel;
using System.ComponentModel.Design;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
using System.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraTab;
using DevExpress.XtraEditors.Controls;
using System.Reflection;
using System.Collections;
using DevExpress.Utils.Serializing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
namespace DevExpress.XtraPrinting.Preview
{
	public class ColorPopup : IDisposable {
		Color resultColor;
		BarItem item;
		Image initalImage = null;
		PopupControlContainer container;
		Rectangle colorRectangle = new Rectangle(1, 13, 14, 3);
		bool drawColorRectangle = true;
		DevExpress.XtraEditors.Repository.RepositoryItemColorEdit rItem;
		XtraTabControl tabControl;
		ColorCellsControl colorCellsControl;
		ColorListBox colorListBox;
		internal bool DrawColorRectangle {
			get { return drawColorRectangle; }
			set { drawColorRectangle = value; } 
		}
		internal XtraTabControl TabControl {
			get { return tabControl; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public BarItem Item { get { return item; } set { item = value; }
		}
		Image InitalImage {
			get {
				if(initalImage == null)
					initalImage = GetInitalImage();
				return initalImage;
			}
		}
		public ColorPopup(PopupControlContainer container) {
			this.container = container; 
			this.resultColor = Color.Empty;
			tabControl = new XtraTabControl();
			tabControl.SelectedPageChanged += new TabPageChangedEventHandler(OnSelectedPageChanged);
			tabControl.LookAndFeel.ParentLookAndFeel = container.LookAndFeel;
			tabControl.TabStop = false;
			colorCellsControl = CreateColorCellsControl();
			AddTabPage(tabControl, colorCellsControl, StringId.ColorTabCustom);
			colorListBox = CreateColorListBox(ColorListBoxViewInfo.WebColors);
			AddTabPage(tabControl, colorListBox, StringId.ColorTabWeb);
			colorListBox = CreateColorListBox(ColorListBoxViewInfo.SystemColors);
			AddTabPage(tabControl, colorListBox, StringId.ColorTabSystem);
			tabControl.Dock = DockStyle.Fill;
			this.container.Controls.Add(tabControl);
			Size size = colorCellsControl.GetBestSize();
			size.Height = GetNearestBestClientHeight(size.Height, colorListBox);
			this.container.Size = tabControl.CalcSizeByPageClient(size);
		}
		void OnSelectedPageChanged(object sender, TabPageChangedEventArgs e) {
			SetFocusToInnerControl();
		}
		internal void SetFocusToInnerControl() {
			tabControl.SelectedTabPage.Controls[0].Focus();
		}
		#region IDisposable implementation
		void Dispose(bool disposing) {
			if(disposing) {
				if(colorCellsControl != null)
					colorCellsControl.EnterColor -= new EnterColorEventHandler(OnEnterColor);
				if(colorListBox != null)
					colorListBox.EnterColor -= new EnterColorEventHandler(OnEnterColor);
				if(rItem != null) {
					rItem.Dispose();
					rItem = null;
				}
				if(tabControl != null)
					tabControl.SelectedPageChanged -= new TabPageChangedEventHandler(OnSelectedPageChanged);
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ColorPopup() {
			Dispose(false);
		}
		#endregion
		void AddTabPage(XtraTabControl tabControl, BaseControl control, StringId id) {
			XtraTabPage tabPage = new XtraTabPage();
			tabPage.Text = Localizer.Active.GetLocalizedString(id);
			control.Dock = DockStyle.Fill;
			control.BorderStyle = BorderStyles.NoBorder;
			control.LookAndFeel.ParentLookAndFeel = container.LookAndFeel;
			tabPage.Controls.Add(control);
			tabControl.TabPages.Add(tabPage);
		}
		ColorListBox CreateColorListBox(object[] items) {
			ColorListBox colorBox = new ColorListBox();
			colorBox.Items.AddRange(items);
			colorBox.EnterColor += new EnterColorEventHandler(OnEnterColor);
			return colorBox;
		}
		ColorCellsControl CreateColorCellsControl() {
			ColorCellsControl control = new ColorCellsControl(null, 8, 6);
			rItem = new DevExpress.XtraEditors.Repository.RepositoryItemColorEdit();
			rItem.ShowColorDialog = false;
			control.Properties = rItem;
			control.Properties.ShowCustomColors = false;
			control.EnterColor += new EnterColorEventHandler(OnEnterColor);
			return control;
		}
		private void OnEnterColor(object sender, EnterColorEventArgs e) {
			resultColor = e.Color;
			OnEnterColor(e.Color);
		}
		private void OnEnterColor(Color clr) {
			container.HidePopup();
			ResultColor = clr;
		}
		private void UpdateColor() {
			if(!drawColorRectangle) return;
			try {
				Image image = InitalImage;
				if(image == null) return;
				image = (Image)image.Clone();
				Graphics g = Graphics.FromImage(image);
				g.FillRectangle(new SolidBrush(resultColor), colorRectangle);
				item.Glyph = image;
				item.ImageIndex = -1;
				foreach(BarItemLink link in item.Links) {
					link.Invalidate();
				}
			} catch {}
		}
		Image GetInitalImage() {
			if(item.Glyph == null) {
				DevExpress.Utils.ImageCollection iml = item.Images as DevExpress.Utils.ImageCollection;
				if(iml == null)
					return null;
				return (Image)iml.Images[item.ImageIndex];
			}
			return Item.Glyph;
		}
		private int GetNearestBestClientHeight(int height, ColorListBox OwnerControl) {
			ColorListBoxViewInfo viewInfo = null;
			try {
				viewInfo = (ColorListBoxViewInfo)typeof(ColorListBox).InvokeMember("ViewInfo",
					BindingFlags.GetProperty | BindingFlags.NonPublic | BindingFlags.Instance,
					null, OwnerControl, null);
			} catch {
				throw new Exception("ColorListBox doesn't contain 'ViewInfo' property");
			}
			int rowHeight = viewInfo.ItemHeight;
			int rows = height / rowHeight;
			if(rows * rowHeight == height)
				return height;
			return (rows + 1) * rowHeight;
		}
		public Color ResultColor { get { return resultColor; } 
			set { 
				resultColor = value; 
				UpdateColor();
			}
		}
	} 
	public class ColorPopupControlContainer : PrintPreviewPopupControlContainer {
		ColorPopup colorPopup;
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public Color ResultColor { get { return colorPopup.ResultColor; } set { colorPopup.ResultColor = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public BarItem Item { get { return colorPopup.Item; } set { colorPopup.Item = value; }
		}
		[
		Category(NativeSR.CatPrinting),
		Obsolete("This property is ignored.")
		]
		public Rectangle ColorRectangle { get { return Rectangle.Empty; } set {} 
		}
		public ColorPopupControlContainer() {
			colorPopup = new ColorPopup(this);
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public bool DrawColorRectangle { get { return colorPopup.DrawColorRectangle; } set { colorPopup.DrawColorRectangle = value; }
		}
		protected override void OnPopup() {
			LookAndFeel.ParentLookAndFeel = Item.Manager.GetController().LookAndFeel;
			base.OnPopup();	 
		}
		public override void ShowPopup(BarManager manager, Point p) {
			colorPopup.TabControl.SelectedTabPageIndex = 1;
			base.ShowPopup(manager, p);
			colorPopup.SetFocusToInnerControl();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(colorPopup != null) {
					colorPopup.Dispose();
					colorPopup = null;
				}
			}
			base.Dispose(disposing);
		}
	}
}
