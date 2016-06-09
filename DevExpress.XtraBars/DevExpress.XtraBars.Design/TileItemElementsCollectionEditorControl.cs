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
using DevExpress.XtraEditors;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Design {
	public partial class TileItemElementsCollectionEditorControl : XtraUserControl {
		ITileItem itemCore;
		ITileItem sourceItem;
		ISite site;
		protected virtual TileItemElement CreateTileItemElement() {
			return new TileItemElement();
		}
		protected virtual bool NeedCreateTilePreview {
			get { return true; }
		}
		protected virtual bool ControlPanelIsHorizontal {
			get { return true; }
		}
		protected virtual bool ShowGridSearchPanel {
			get { return false; }
		}
		public TileItemElementsCollectionEditorControl() {
			InitializeComponent();
		}
		public void Assign(ISite site, ITileItem sourceItem) {
			this.site = site;
			this.sourceItem = sourceItem;
			this.InitHelper = TileItemInitHelper.Create(sourceItem.Tag is TileItemFrame);
			this.itemCore = CreatePreviewItem(sourceItem);
			InitCollectionEditor();
		}
		public void Assign<T>(ISite site, ITileItem sourceItem) where T : TileItem {
			this.site = site;
			this.sourceItem = sourceItem;
			this.InitHelper = TileItemInitHelper.Create(sourceItem.Tag is TileItemFrame);
			this.itemCore = CreatePreviewItem<T>(sourceItem);
			InitCollectionEditor();
		}
		public void OnClosing() {
			this.propertyGrid.Site = null;
		}
		protected internal TileItemElementCollection Elements {
			get { return this.itemCore.Elements; }
		}
		TileItemInitHelper InitHelper { get; set; }
		void InitCollectionEditor() {
			InitTileControl();
			InitListBoxData();
			InitPropertyGrid();
			RefreshPreview();
			RefreshBtnsState();
			UpdateUI();
		}
		protected virtual void UpdateUI() {
			if(!NeedCreateTilePreview)
				HidePreview();
			if(ControlPanelIsHorizontal)
				UpdateByHiDPI();
			else
				splitContainerControl2.Horizontal = false;
			propertyGrid.ShowSearchPanel = ShowGridSearchPanel;
		}
		SizeF scaleFactor = new SizeF(1f, 1f);
		protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
			this.scaleFactor = factor;
			base.ScaleControl(factor, specified);
		}
		void UpdateByHiDPI() {
			int btnLeft = btnAddElement.PointToScreen(Point.Empty).X;
			int listLeft = listBox.PointToScreen(Point.Empty).X;
			if(btnLeft < listLeft)
				splitContainerControl2.SplitterPosition += (int)((listLeft - btnLeft) / scaleFactor.Width);
		}
		void InitTileControl() {
			if(!NeedCreateTilePreview) return;
			ITileControl control = this.sourceItem.Control;
			if(control != null) { 
				if(control.Images != null)
					this.tileControl.Images = control.Images;
				if(control.BackgroundImage != null){
					this.tileControl.BackgroundImage = control.BackgroundImage;
					this.tileControl.BackgroundImageLayout = control.Properties.BackgroundImageLayout;
				}
			}
			this.tileControl.BackColor = GetTileControlBackColor();
		}
		static readonly Color DefaultTileControlBackColor = Color.Transparent;
		Color GetTileControlBackColor() {
			ITileControl tileControl = this.sourceItem.Control;
			if(tileControl == null)
				return DefaultTileControlBackColor;
			TileControlViewInfo vi = tileControl.ViewInfo;
			if(vi == null || vi.PaintAppearance == null)
				return DefaultTileControlBackColor;
			return vi.PaintAppearance.BackColor;
		}
		void InitListBoxData() {
			this.listBox.Items.Clear();
			foreach(TileItemElement element in Elements)
				AddListBoxItem(element);
		}
		void InitPropertyGrid() {
			this.propertyGrid.Site = this.site;
		}
		void RefreshPreview() {
			this.tileControl.BeginUpdate();
			try {
				if(this.tileControl.Groups.Count == 0)
					this.tileControl.Groups.Add(new TileGroup());
				this.tileControl.Groups[0].Items.Clear();
				this.tileControl.Groups[0].Items.Add(itemCore);
				this.tileControl.AllowDrag = false;
				if(sourceItem.Control != null) {
					this.tileControl.ItemSize = sourceItem.Control.Properties.ItemSize;
					this.tileControl.IndentBetweenItems = sourceItem.Control.Properties.IndentBetweenItems;
					this.tileControl.AppearanceItem.Assign(sourceItem.Control.AppearanceItem);
					(this.tileControl as ITileControl).Properties.LargeItemWidth =
						(sourceItem.Control as ITileControl).Properties.LargeItemWidth;
				}
			}
			finally {
				this.tileControl.EndUpdate();
				this.tileControl.Groups[0].Items[0].StartContentAnimation();
			}
		}
		void HidePreview() {
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel2;
		}
		T CreatePreviewItem<T>(ITileItem source) where T : TileItem {
			T res = (T)Activator.CreateInstance(typeof(T));
			try {
				InitHelper.BeginInit(source);
				InitHelper.Init(res, source);
			}
			finally {
				InitHelper.EndInit(source);
			}
			return res as T;
		}
		TileItem CreatePreviewItem(ITileItem source){
			TileItem res = new TileItem();
			try {
				InitHelper.BeginInit(source);
				InitHelper.Init(res, source);
			}
			finally {
				InitHelper.EndInit(source);
			}
			return res;
		}
		void AssignPropertyGrid(TileItemElement element) {
			this.propertyGrid.SelectedObject = element;
		}
		TileItemElement CreateNewElement() {
			TileItemElement element = CreateTileItemElement();
			element.Text = GetTileItemElementText();
			return element;
		}
		string GetTileItemElementText() {
			string prefix = "element";
			for(int i = 1; true; i++) {
				string res = string.Concat(prefix, i.ToString());
				if(!IsTileItemElementExist(res)) return res;
			}
		}
		bool IsTileItemElementExist(string text) {
			foreach(ImageListBoxItem item in this.listBox.Items) {
				TileItemElement element = (TileItemElement)item.Value;
				if(string.Equals(element.Text, text))
					return true;
			}
			return false;
		}
		protected TileItemElement GetLastElement() {
			int count = this.listBox.Items.Count;
			if(count == 0) return null;
			return this.listBox.Items[count - 1].Value as TileItemElement;
		}
		Image GetTileItemElementImage(TileItemElement element) {
			if(element.Image != null)
				return element.Image;
			ITileControl control = this.sourceItem.Control;
			if(control != null && control.Images != null && element.ImageIndex != -1)
				return ImageCollection.GetImageListImage(control.Images, element.ImageIndex);
			return null;
		}
		void AddListBoxItem(TileItemElement element) {
			ImageListBoxItem item = new ImageListBoxItem(element);
			Image image = GetTileItemElementImage(element);
			if(image != null)
				AssignImageListBoxItemImage(item, image);
			this.listBox.Items.Add(item);
		}
		void RemoveListBoxItem(TileItemElement element) {
			for(int i = 0; i < listBox.Items.Count; i++) {
				ImageListBoxItem item = listBox.Items[i];
				if(object.ReferenceEquals(item.Value, element)) {
					listBox.Items.RemoveAt(i);
					return;
				}
			}
		}
		protected void AddNewItem() {
			TileItemElement element = CreateNewElement();
			AddListBoxItem(element);
			this.tileControl.Groups[0].Items[0].Elements.Add(element);
			SelectLastListBoxItem();
		}
		void SelectLastListBoxItem() {
			int itemsCount = this.listBox.Items.Count;
			if(itemsCount > 0) this.listBox.SelectedIndex = itemsCount - 1;
		}
		void RemoveSelectedItem() {
			TileItemElement element = this.listBox.SelectedValue as TileItemElement;
			if(element == null)
				return;
			RemoveListBoxItem(element);
			RemoveTileControlItem(element);
		}
		void RemoveTileControlItem(TileItemElement element) {
			this.listBox.Items.Remove(element);
			this.tileControl.Groups[0].Items[0].Elements.Remove(element);
		}
		void RefreshListBox(PropertyValueChangedEventArgs e) {
			if(IsImagePropertyChanged(e.ChangedItem.Label))
				RefreshListBoxImage();
			this.listBox.Refresh();
		}
		void RefreshListBoxImage() {
			ImageListBoxItem item = this.listBox.SelectedItem as ImageListBoxItem;
			if(item == null) return;
			Image image = GetTileItemElementImage((TileItemElement)item.Value);
			if(image == null) item.ImageIndex = -1;
			else AssignImageListBoxItemImage(item, image);
		}
		void AssignImageListBoxItemImage(ImageListBoxItem item, Image image) {
			this.listBoxImageCollection.AddImage(image);
			item.ImageIndex = this.listBoxImageCollection.Images.Count - 1;
		}
		bool IsImagePropertyChanged(string propertyName) {
			string[] names = { "Image", "ImageIndex" };
			return Array.Exists(names, delegate(string name) {
				return string.Equals(name, propertyName, StringComparison.Ordinal);
			});
		}
		void GoToUp() {
			int pos = this.listBox.SelectedIndex;
			if(pos == -1 || pos == 0)
				return;
			UpdateListBoxItemPos(pos, pos - 1);
			UpdateTileItemElementsPos(pos, pos - 1);
		}
		void GoToDown() {
			int pos = this.listBox.SelectedIndex;
			if(pos == -1 || pos == this.listBox.Items.Count - 1)
				return;
			UpdateListBoxItemPos(pos, pos + 1);
			UpdateTileItemElementsPos(pos, pos + 1);
		}
		void UpdateListBoxItemPos(int pos, int newPos) {
			ImageListBoxItem item = this.listBox.Items[pos];
			this.listBox.Items.RemoveAt(pos);
			this.listBox.Items.Insert(newPos, item);
			this.listBox.SelectedIndex = newPos;
		}
		void UpdateTileItemElementsPos(int pos, int newPos) {
			List<TileItemElement> referenceOldElement = new List<TileItemElement>();
			List<TileItemElement> referenceNewElement = new List<TileItemElement>();
			TileItemElement element = this.itemCore.Elements[pos];
			ChechReferenceElements(pos, referenceOldElement);
			ChechReferenceElements(newPos, referenceNewElement);
			this.itemCore.Elements.RemoveAt(pos);
			this.itemCore.Elements.Insert(newPos, element);
			UpdateElementsAnchorIndex(pos, newPos, referenceOldElement, referenceNewElement);
		}
		void ChechReferenceElements(int index, List<TileItemElement> list) {
			foreach(TileItemElement elem in this.itemCore.Elements)
				if(elem.AnchorElementIndex == index)
					list.Add(elem);
		}
		void UpdateElementsAnchorIndex(int oldIndex, int newIndex, List<TileItemElement> referenceOldElement, List<TileItemElement> referenceNewElement) {
			foreach(TileItemElement elem in this.itemCore.Elements) {
				if(referenceNewElement.Contains(elem))
					elem.AnchorElementIndex = oldIndex;
				if(referenceOldElement.Contains(elem))
					elem.AnchorElementIndex = newIndex;
			}
		}
		protected void RefreshBtnsState() {
			this.btnRemoveElement.Enabled = this.listBox.Items.Count != 0;
			this.btnUpCmd.Enabled = this.listBox.Items.Count > 1;
			this.btnDownCmd.Enabled = this.listBox.Items.Count > 1;
		}
		#region Handlers
		void btnAddElement_Click(object sender, EventArgs e) {
			AddNewItem();
			RefreshBtnsState();
			OnChanged();
		}
		protected virtual void OnChanged() { }
		void btnRemoveElement_Click(object sender, EventArgs e) {
			RemoveSelectedItem();
			RefreshBtnsState();
			OnChanged();
		}
		void btnUpCmd_Click(object sender, EventArgs e) {
			GoToUp();
			RefreshBtnsState();
			OnChanged();
		}
		void btnDownCmd_Click(object sender, EventArgs e) {
			GoToDown();
			RefreshBtnsState();
			OnChanged();
		}
		void listBox_SelectedValueChanged(object sender, EventArgs e) {
			TileItemElement element = (TileItemElement)this.listBox.SelectedValue;
			AssignPropertyGrid(element);
		}
		void propertyGrid_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			RefreshListBox(e);
			OnChanged();
		}
		void ListBox_DrawItem(object sender, ListBoxDrawItemEventArgs e) {
			e.Appearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
		}
		#endregion
	}
	class TileItemInitHelper {
		public static TileItemInitHelper Create(bool frameItem) {
			if(frameItem)
				return new FrameTileItemInitHelper();
			return new TileItemInitHelper();
		}
		public virtual void Init(ITileItem res, ITileItem source) {
			res.Padding = source.Padding;
			res.BackgroundImage = source.BackgroundImage;
			res.Properties.Assign(source.Properties);
			if(source is DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseTile && source.Properties.ItemSize == TileItemSize.Default)
				res.Properties.ItemSize = TileItemSize.Wide;
			res.Appearances.Assign(source.Appearances);
			res.Elements.Assign(source.Elements);
		}
		public virtual void BeginInit(ITileItem source) { }
		public virtual void EndInit(ITileItem source) { }
	}
	class FrameTileItemInitHelper : TileItemInitHelper {
		TileItemFrameStateInfo frameInfo = null;
		public override void BeginInit(ITileItem source) {
			TileItemFrame frame = source.Tag as TileItemFrame;
			if(frame == null)
				return;
			this.frameInfo = new TileItemFrameStateInfo(frame);
			source.SetContent(this.frameInfo.Frame, false);
		}
		public override void EndInit(ITileItem source) {
			if(this.frameInfo == null)
				return;
			frameInfo.Restore();
			source.SetContent(this.frameInfo.Frame, false);
			this.frameInfo = null;
		}
	}
	class TileItemFrameStateInfo {
		TileItemFrame frame;
		public TileItemFrameStateInfo(TileItemFrame frame) {
			this.frame = frame;
			Assign(frame);
		}
		public void Assign(TileItemFrame frame) {
			this.UseText = frame.UseText;
			this.UseImage = frame.UseImage;
			this.UseBackgroundImage = frame.UseBackgroundImage;
			frame.UseText = frame.UseImage = frame.UseBackgroundImage = true;
		}
		public void Restore() {
			this.frame.UseText = this.UseText;
			this.frame.UseImage = this.UseImage;
			this.frame.UseBackgroundImage = this.UseBackgroundImage;
		}
		public bool UseText { get; private set; }
		public bool UseImage { get; private set; }
		public bool UseBackgroundImage { get; private set; }
		public TileItemFrame Frame { get { return this.frame; } }
	}
}
