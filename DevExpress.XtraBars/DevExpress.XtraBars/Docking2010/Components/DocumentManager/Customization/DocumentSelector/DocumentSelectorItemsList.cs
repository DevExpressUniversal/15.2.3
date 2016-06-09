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
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public class DocumentSelectorItemsListInfo : ObjectInfoArgs {
		int topRowIndexCore;
		int columnCount;
		public DocumentSelectorAdornerElementInfoArgs Owner { get; private set; }
		internal IList<DocumentSelectorItemInfo> ItemsInfo = new List<DocumentSelectorItemInfo>();
		public IList<DocumentSelectorItemInfo> PaintItemInfo { get; private set; }
		int maxRowCountCore = 15;
		public int MaxRowCount {
			get { return maxRowCountCore; }
			set { maxRowCountCore = value; }
		}
		public int MaxRowCountWithScroll { get { return MaxRowCount - 1; } }
		public int HotItemIndex { get; set; }
		public int SelectItemIndex { get; set; }
		public int TopRowIndex {
			get { return topRowIndexCore; }
			set {
				if(value < 0)
					value = 0;
				if(value == 1)
					if(topRowIndexCore > value)
						value = 0;
					else
						value = 2;
				topRowIndexCore = (value > ItemsInfo.Count - GetMaxNumberElementsWithScroll()) ? ItemsInfo.Count - GetMaxNumberElementsWithScroll() : value;
			}
		}
		public AppearanceObject PaintAppearance { get; private set; }
		public DocumentSelectorItemsListInfo(IList<DocumentSelectorItem> list, int columnCount, DocumentSelectorAdornerElementInfoArgs owner) {
			Owner = owner;
			for(int i = 0; i < list.Count; i++) {
				DocumentSelectorItemInfo info = new DocumentSelectorItemInfo(list[i].Caption, list[i].Image);
				AssignItemFormat(list[i], info);
				info.Owner = Owner;
				info.Index = i;
				info.AllowGlyphSkinning = list[i].AllowGlyphSkinning;
				ItemsInfo.Add(info);
			}
			HotItemIndex = -1;
			SelectItemIndex = -1;
			TopRowIndex = 0;
			this.columnCount = columnCount;
			PaintAppearance = new FrozenAppearance();
		}
		void AssignItemFormat(DocumentSelectorItem item, DocumentSelectorItemInfo itemInfo) {
			itemInfo.Header = item.Header;
			itemInfo.Footer = item.Footer;
			itemInfo.ItemFormat = item.CaptionFormat;
			itemInfo.HeaderFormat = item.HeaderFormat;
			itemInfo.FooterFormat = item.FooterFormat;
		}
		public void Calc(GraphicsCache cache, DocumentSelectorItemsListPainter painter, Point offset) {
			this.Cache = cache;
			PaintAppearance.Assign(painter.DefaultAppearance);
			UpdateAppearances();
			CreatePaintItemInfo();
			CalcBounds(painter, offset);
		}
		void UpdateAppearances() {
			for(int i = 0; i < ItemsInfo.Count; i++) {
				ItemsInfo[i].State = ObjectState.Normal;
				ItemsInfo[i].PaintAppearance.AssignInternal(Owner.Normal);
			}
			if(HotItemIndex >= 0) {
				ItemsInfo[HotItemIndex].State |= ObjectState.Hot;
				ItemsInfo[HotItemIndex].PaintAppearance.AssignInternal(Owner.Hot);
			}
			if(SelectItemIndex >= 0) {
				ItemsInfo[SelectItemIndex].State |= ObjectState.Selected;
				ItemsInfo[SelectItemIndex].PaintAppearance.AssignInternal(Owner.Selected);
			}
		}
		void CalcBounds(DocumentSelectorItemsListPainter painter, Point offset) {
			if(ItemsInfo.Count == 0) {
				Bounds = new Rectangle(offset, new Size(0, 0));
				return;
			}
			int offsetHeight = 0;
			int offsetWidth = 0;
			int tmpOffsetHeight = 0;
			int check = 0;
			bool needToCorrection = true;
			DocumentSelectorItemPainter itemPainter = painter.CreateDocumentSelectorItemPainter();
			DocumentSelectorItemPainter scroollItemPainter = painter.CreateDocumentSelectorScrollItemPainter();
			foreach(DocumentSelectorItemInfo item in PaintItemInfo) {
				DocumentSelectorItemPainter p = item is DocumentSelectorScrollItemInfo ? scroollItemPainter : itemPainter;
				item.Calc(this.Cache, p, new Point(offsetWidth + offset.X, tmpOffsetHeight + offset.Y));
				tmpOffsetHeight += item.Bounds.Height;
				needToCorrection = true;
				if(check == MaxRowCountWithScroll) {
					needToCorrection = false;
					offsetHeight = (offsetHeight < tmpOffsetHeight) ? tmpOffsetHeight : offsetHeight;
					check = -1;
					offsetWidth += item.Bounds.Width;
					tmpOffsetHeight = 0;
				}
				check++;
			}
			if(needToCorrection) offsetWidth += PaintItemInfo[0].Bounds.Width;
			offsetHeight = (offsetHeight < tmpOffsetHeight) ? tmpOffsetHeight : offsetHeight;
			Size content = new Size(offsetWidth, offsetHeight);
			Bounds = new Rectangle(offset, content);
		}
		void CreatePaintItemInfo() {
			PaintItemInfo = new List<DocumentSelectorItemInfo>();
			bool hasScroll = (ItemsInfo.Count > MaxRowCount * columnCount);
			bool scrollUp = false;
			bool srollDown = false;
			if(hasScroll) {
				scrollUp = (TopRowIndex > 0);
				srollDown = (ItemsInfo.Count - TopRowIndex > GetMaxNumberElementsWithScroll());
				if(scrollUp)
					PaintItemInfo.Add(CreateScrollItemInfo(true));
				for(int i = TopRowIndex; i < GetMaxListIndex(scrollUp, srollDown); i++) {
					PaintItemInfo.Add(ItemsInfo[i]);
				}
				if(srollDown)
					PaintItemInfo.Add(CreateScrollItemInfo(false));
			}
			else {
				foreach(DocumentSelectorItemInfo item in ItemsInfo) {
					PaintItemInfo.Add(item);
				}
			}
		}
		protected virtual DocumentSelectorScrollItemInfo CreateScrollItemInfo(bool scrollUp) {
			return scrollUp ? (DocumentSelectorScrollItemInfo)new DocumentSelectorScrollUpItemInfo() :
				new DocumentSelectorScrollDownItemInfo();
		}
		int GetMaxNumberElementsWithScroll() {
			return (MaxRowCountWithScroll + MaxRowCount * (columnCount - 1));
		}
		int GetMaxListIndex(bool scrollUp, bool scrollDown) {
			return TopRowIndex + MaxRowCount * columnCount - Convert.ToInt32(scrollDown) - Convert.ToInt32(scrollUp);
		}
	}
	public class DocumentSelectorItemsListPainter : ObjectPainter {
		public override void DrawObject(ObjectInfoArgs e) {
			DocumentSelectorItemsListInfo info = e as DocumentSelectorItemsListInfo;
			DrawBackground(info.Cache, info);
			DrawContent(info.Cache, info);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			return Rectangle.Inflate(client, 5, 5);
		}
		protected virtual void DrawBackground(GraphicsCache cache, DocumentSelectorItemsListInfo info) {
			info.PaintAppearance.DrawBackground(cache, info.Bounds);
		}
		protected void DrawContent(GraphicsCache cache, DocumentSelectorItemsListInfo info) {
			DocumentSelectorItemPainter itemPainter = CreateDocumentSelectorItemPainter();
			DocumentSelectorItemPainter scroollItemPainter = CreateDocumentSelectorScrollItemPainter();
			foreach(DocumentSelectorItemInfo item in info.PaintItemInfo) {
				if(item is DocumentSelectorScrollItemInfo)
					ObjectPainter.DrawObject(cache, scroollItemPainter, item);
				else ObjectPainter.DrawObject(cache, itemPainter, item);
			}
		}
		protected internal virtual DocumentSelectorItemPainter CreateDocumentSelectorItemPainter() {
			return new DocumentSelectorItemPainter();
		}
		protected internal virtual DocumentSelectorItemPainter CreateDocumentSelectorScrollItemPainter() {
			return new DocumentSelectorScrollItemPainter();
		}
	}
	public class DocumentSelectorItemsListSkinPainter : DocumentSelectorItemsListPainter {
		ISkinProvider providerCore;
		public DocumentSelectorItemsListSkinPainter(ISkinProvider provider) {
			this.providerCore = provider;
		}
		protected Skin GetSkin() {
			return RibbonSkins.GetSkin(providerCore);
		}
		protected SkinElement GetBackground() {
			return GetSkin()[RibbonSkins.SkinPopupGalleryBackground];
		}
		protected internal override DocumentSelectorItemPainter CreateDocumentSelectorItemPainter() {
			return new DocumentSelectorItemSkinPainter(providerCore);
		}
		protected internal override DocumentSelectorItemPainter CreateDocumentSelectorScrollItemPainter() {
			return new DocumentSelectorScrollItemSkinPainter(providerCore);
		}
		protected override void DrawBackground(GraphicsCache cache, DocumentSelectorItemsListInfo info) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), info.Bounds);
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, elementInfo);
		}
		public override Rectangle CalcBoundsByClientRectangle(ObjectInfoArgs e, Rectangle client) {
			SkinElementInfo elementInfo = new SkinElementInfo(GetBackground(), client);
			return SkinElementPainter.Default.CalcBoundsByClientRectangle(elementInfo, client);
		}
		public override AppearanceDefault DefaultAppearance {
			get {
				AppearanceDefault appearance = new AppearanceDefault();
				GetBackground().ApplyForeColorAndFont(appearance);
				return appearance;
			}
		}
	}
}
