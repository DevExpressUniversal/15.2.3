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
using System.ComponentModel.Design;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Design {
	public partial class TileItemFrameEditorControl : XtraUserControl {
		public TileItemFrameEditorControl() {
			InitializeComponent();
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			AssignTileItemAppearance(listControl);
		}
		ITileItem tileItem;
		[DefaultValue(null)]
		public ITileItem TileItem {
			get { return tileItem; }
			set {
				tileItem = value;
				OnTileItemChanged();
			}
		}
		public void Assign(ITileControl dst, ITileControl src) {
			dst.Properties.Assign(src.Properties);
			dst.Properties.Orientation = Orientation.Vertical;
			dst.Properties.AllowSelectedItem = true;
			dst.Properties.ColumnCount = 1;
			dst.Properties.ItemSize = src.Properties.ItemSize;
			dst.AppearanceItem.Assign(src.AppearanceItem);
			dst.AppearanceItem.Selected.Assign(src.AppearanceItem.Normal);
			dst.Images = src.Images;
		}
		public void Assign(ITileItem dst, ITileItem src) {
			dst.Properties.Assign(src.Properties);
			if(src is DevExpress.XtraBars.Docking2010.Views.WindowsUI.BaseTile && src.Properties.ItemSize == TileItemSize.Default)
				dst.Properties.ItemSize = TileItemSize.Wide;
			dst.Padding = src.Padding;
			dst.BackgroundImage = src.BackgroundImage;
			dst.Appearances.Assign(src.Appearances);
			dst.Elements.Assign(src.Elements);
			if(src.Control != null) {
				previewControl.ItemSize = src.Control.Properties.ItemSize;
				previewControl.IndentBetweenItems = src.Control.Properties.IndentBetweenItems;
			}
		}
		void FillListControl() {
			if(this.listControl.Groups.Count == 0)
				this.listControl.Groups.Add(new TileGroup());
			if(TileItem == null)
				return;
			if(TileItem.Control != null) {
				Assign(this.listControl, TileItem.Control);
			}
			foreach(TileItemFrame itemInfo in TileItem.Frames) {
				this.listControl.Groups[0].Items.Add(CreateItemForFrame(itemInfo));
			}
			if(this.listControl.Groups[0].Items.Count > 0)
				this.listControl.SelectedItem = this.listControl.Groups[0].Items[0];
		}
		protected internal void UpdatePreview() {
			this.previewControl.BeginUpdate();
			try {
				if(this.previewControl.Groups.Count == 0)
					this.previewControl.Groups.Add(new TileGroup());
				this.previewControl.AppearanceItem.Assign(listControl.AppearanceItem);
				this.previewControl.Groups[0].Items.Clear();
				TileItem previewItem = new TileItem();
				if(TileItem != null)
					Assign(previewItem, TileItem);
				foreach(TileItem item in this.listControl.Groups[0].Items) {
					previewItem.Frames.Add((TileItemFrame)item.Tag);
				}
				this.previewControl.Groups[0].Items.Add(previewItem);
			} finally {
				this.previewControl.EndUpdate();
				this.previewControl.Groups[0].Items[0].StartContentAnimation();
			}
		}
		ITileItem CreateItemForFrame(TileItemFrame frame) {
			ITileItem item = new TileItem();
			Assign(item, TileItem);
			AssignAppearanceFromFrame(item, frame);
			item.SetContent(frame, false);
			item.Tag = frame;
			return item;
		}
		void AssignAppearanceFromFrame(ITileItem item, TileItemFrame frame) {
			if(frame.Appearance != AppearanceObject.EmptyAppearance) {
				item.Appearances.Normal.Assign(frame.Appearance);
				item.Appearances.Pressed.Assign(frame.Appearance);
				item.Appearances.Hovered.Assign(frame.Appearance);
				item.Appearances.Selected.Assign(frame.Appearance);
			}
		}
		protected virtual void OnTileItemChanged() {
			FillListControl();
			if(TileItem != null)
				this.propertyGrid1.Site = ((IComponent)TileItem).Site;
			UpdatePreview();
		}
		public ITileControl Control { get { return TileItem.Control; } }
		protected IServiceProvider ServiceProvider { get { return ((IComponent)TileItem).Site; } }
		void listControl_SelectedItemChanged(object sender, TileItemEventArgs e) {
			UpdateButtons();
			object frame = this.listControl.SelectedItem == null ? null : this.listControl.SelectedItem.Tag;
			this.propertyGrid1.SelectedObject = frame;
			UpdateSelectedKeyValue(frame, e.Item);
		}
		void UpdateSelectedKeyValue(object tileItemFrame, TileItem item) {
			IDictionaryService svc = ServiceProvider.GetService(typeof(IDictionaryService)) as IDictionaryService;
			if(svc != null && tileItemFrame != null) svc.SetValue(tileItemFrame, item);
		}
		void UpdateButtons() {
			this.btnRemoveInfo.Enabled = this.listControl.SelectedItem != null;
		}
		void btnAddInfo_Click(object sender, EventArgs e) {
			AddFrame();
		}
		void btnRemoveInfo_Click(object sender, EventArgs e) {
			RemoveSelectedInfo();
		}
		void AddFrame() {
			TileItemFrame frame = new TileItemFrame();
			frame.UseText = true;
			frame.UseImage = true;
			frame.UseBackgroundImage = true;
			frame.AnimateText = true;
			frame.AnimateImage = true;
			frame.AnimateBackgroundImage = true;
			frame.Elements.Assign(TileItem.Elements);
			frame.BackgroundImage = TileItem.BackgroundImage;
			this.listControl.Groups[0].Items.Add(CreateItemForFrame(frame));
			this.listControl.SelectedItem = this.listControl.Groups[0].Items[this.listControl.Groups[0].Items.Count - 1];
			UpdatePreview();
			UpdateButtons();
		}
		void RemoveSelectedInfo() {
			int itemIndex = this.listControl.Groups[0].Items.IndexOf(this.listControl.SelectedItem);
			this.listControl.Groups[0].Items.Remove(this.listControl.SelectedItem);
			if(this.listControl.Groups[0].Items.Count <= itemIndex)
				itemIndex = this.listControl.Groups[0].Items.Count - 1;
			this.listControl.SelectedItem =  itemIndex == -1? null: this.listControl.Groups[0].Items[itemIndex];
			UpdatePreview();
			UpdateButtons();
		}
		void propertyGrid1_PropertyValueChanged(object s, PropertyValueChangedEventArgs e) {
			TileItemFrame frame = (TileItemFrame)this.propertyGrid1.SelectedObject;
			ITileItem tile = GetTileItemByTag(frame);
			if(tile != null) {
				AssignAppearanceFromFrame(tile, frame);
				tile.SetContent(frame, true);
			}
			if(e.OldValue is TileItemElementCollection)
				RefreshTileItem();
			UpdatePreview();
		}
		protected virtual void RefreshTileItem() {
			ITileItem tileItem = this.listControl.SelectedItem;
			if(tileItem == null)
				return;
			TileItemFrame frame = tileItem.Tag as TileItemFrame;
			if(frame == null)
				return;
			foreach(TileItemElement element in tileItem.Elements) {
				AppearanceObject normal = element.Appearance.Normal;
				element.Appearance.Selected.Assign(normal);
				element.Appearance.Hovered.Assign(normal);
			}
		}
		protected void AssignTileItemAppearance(TileControl listControl) {
			TileItem item = listControl.SelectedItem;
			if(item != null) AssignTileItemAppearanceCore(item, item.Tag as TileItemFrame);
		}
		protected virtual void AssignTileItemAppearanceCore(TileItem item, TileItemFrame frame) {
			if(item == null || frame == null || item.Elements.Count != frame.Elements.Count)
				return;
			for(int i = 0; i < item.Elements.Count; i++) {
				TileItemElement itemElement = item.Elements[i];
				TileItemElement frameElement = frame.Elements[i];
				itemElement.Appearance.Normal.Assign(frameElement.Appearance.Normal);
				itemElement.Appearance.Selected.Assign(frameElement.Appearance.Selected);
				itemElement.Appearance.Hovered.Assign(frameElement.Appearance.Hovered);
			}
			RefreshTileItem();
		}
		protected virtual TileItem GetTileItemByTag(TileItemFrame frame) {
			foreach(TileItem item in this.listControl.Groups[0].Items) {
				if(item.Tag == frame)
					return item;
			}
			return null;
		}
		public List<TileItemFrame> GetItemInfoCollection() {
			List<TileItemFrame> res = new List<TileItemFrame>();
			foreach(TileItem item in this.listControl.Groups[0].Items) {
				res.Add((TileItemFrame)item.Tag);
			}
			return res;
		}
		void listControl_EndItemDragging(object sender, DevExpress.XtraEditors.TileItemDragEventArgs e) {
			UpdatePreview();
		}
		void previewControl_StartItemDragging(object sender, DevExpress.XtraEditors.TileItemDragEventArgs e) {
			e.Cancel = true;
		}
	}
}
