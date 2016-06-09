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

using DevExpress.XtraBars.Navigation;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Docking2010.Customization {
	interface ISearchTileBarItemViewInfo {
		bool AllowSelected { get; }
	}
	class SearchTileBarViewInfo : TileBarViewInfo {		
		public SearchTileBarViewInfo(SearchTileBar control) : base(control) { }
		SearchTileBar SearchTileBar { get { return Owner as SearchTileBar; } }
		protected override int SelectionWidth { get { return 0; } }
		public override int IndentBetweenItems { get { return 0; } }
		protected override int ClipBoundsTopIndent { get { return 0; } }
		protected override int TextToGroupsIndent { get { return 0; } }
		public override int GroupTextToItemsIndent { get { return 10; } }
		Size GetSearchControlMinSize() {
			if(SearchTileBar.SearchControl == null) return Size.Empty;			
			return SearchTileBar.SearchControl.Size;
		}
		protected override Rectangle CalcTextBounds(Rectangle bounds) {
			Padding padding = Owner.Properties.Padding;
			Rectangle rect = new Rectangle(bounds.X + padding.Left, bounds.Y + padding.Top, bounds.Width - padding.Horizontal, bounds.Height);
			Rectangle textBounds = Owner.Properties.ShowText ? base.CalcTextBounds(rect) : Rectangle.Empty;
			SearchControlBounds = CalcSearchControlBounds(rect, textBounds);
			return textBounds;
		}
		public Rectangle SearchControlBounds { get; private set; }
		protected override Rectangle CalcScrollBarBounds() {
			if(IsHorizontal) return base.CalcScrollBarBounds();
			int y = GroupsClipBounds.Top;
			int h = ClientBounds.Height - GroupsClipBounds.Top;
			return new Rectangle(ClientBounds.Right - Owner.ScrollBar.GetDefaultVerticalScrollBarWidth(), y, Owner.ScrollBar.Width, h);
		}
		protected virtual Rectangle CalcSearchControlBounds(Rectangle contentBounds, Rectangle textBounds) {
			if(SearchTileBar.SearchControl == null || !SearchTileBar.SearchControl.Visible) return Rectangle.Empty;
			Size minSize = GetSearchControlMinSize();
			int y = contentBounds.Y;
			if(!textBounds.Size.IsEmpty && Owner.Properties.ShowText)
				y = textBounds.Bottom + TextToSearchControlIdent;
			return new Rectangle(contentBounds.X, y, contentBounds.Width, minSize.Height);
		}
		protected virtual int TextToSearchControlIdent { get { return 10; } }
		public override System.Drawing.Size GetItemSize(TileItemViewInfo itemInfo) {
			SearchTileBarItemViewInfoBase info = itemInfo as SearchTileBarItemViewInfoBase;
			Size size = Size.Empty;
			if(info != null)
				size = info.CalcSize(ContentBounds);
			int width = size.IsEmpty ? ContentBounds.Width : size.Width;
			int height = size.IsEmpty ? ItemSize : size.Height;
			return new Size(width, height);
		}
		protected override TileControlScrollMode ScrollMode { get { return TileControlScrollMode.ScrollBar; } }
		protected override Rectangle CalcContentBounds(Rectangle rect) {
			rect.Inflate(-1, -1);
			return rect;
		}
		protected override Rectangle CalcGroupsContentBounds(Rectangle contentBounds, Rectangle textBounds) {
			if(textBounds.Size.IsEmpty && SearchControlBounds.Size.IsEmpty) return contentBounds;
			Padding padding = Owner.Properties.Padding;
			int y = SearchControlBounds.Bottom + padding.Bottom + TextToGroupsIndent;
			return new Rectangle(contentBounds.X, y, contentBounds.Width, contentBounds.Bottom - y);
		}
		protected override ScrollBarBase CreateScrollBarCore(ScrollBarBase scrollBase) {
			return ScrollBarBase.ApplyUIMode(scrollBase, ScrollUIMode.Touch);
		}
		protected override TileControlLayoutCalculator GetNewLayoutCalculator(TileControlViewInfo viewInfo) {
			return new SearchTileBarLayoutCalculator(viewInfo);
		}
	}
	class SearchTileBarLayoutCalculator : TileControlLayoutCalculator {
		public SearchTileBarLayoutCalculator(TileControlViewInfo viewInfo) : base(viewInfo) { }
		protected override TileControlLayoutGroup CreateGroupLayoutInfo(TileGroup group, TileItemViewInfo dragItem, TileControlDropItemInfo dropInfo) {
			TileControlLayoutGroup layoutInfo = new TileControlLayoutGroup() { Group = group, GroupInfo = group.GroupInfo };
			foreach(TileItem item in group.Items) {
				if(!CanCreateItem(item, dragItem)) continue;
				if(dragItem != null && dropInfo != null && dropInfo.Group == group && dropInfo.DropItem == item) {
					layoutInfo.Items.Add(GetNewLayoutItemCore(Rectangle.Empty, dragItem.Item, dragItem, null));
				}
				layoutInfo.Items.Add(GetNewLayoutItemCore(Rectangle.Empty, item, item.ItemInfo, null));
			}
			if(dropInfo != null && dropInfo.DropItem != null) {
				if(dragItem != null && group == dropInfo.Group && dropInfo.Index >= group.Items.Count) {
					layoutInfo.Items.Add(GetNewLayoutItemCore(Rectangle.Empty, dragItem.Item, dragItem, null));
				}
			}
			return layoutInfo;
		}
	}
	class SearchTileBarItemViewInfoBase : TileBarItemViewInfo, ISearchTileBarItemViewInfo {
		public SearchTileBarItemViewInfoBase(SearchTileBarItemBase item) : base(item) { }
		protected override AppearanceDefault CreateDefaultAppearance() {
			if(ControlCore == null) return base.CreateDefaultAppearance();
			if(IsHovered || IsPressed || IsSelected)
				return GetModifiedDefaultAppearance();
			return base.CreateDefaultAppearance();
		}
		public Size CalcSize(Rectangle contentBounds) { return CalcSizeCore(contentBounds); }
		protected virtual Size CalcSizeCore(Rectangle contentBounds) { return Size.Empty; }
		#region ITileSearchClientItemViewInfo Members
		bool ISearchTileBarItemViewInfo.AllowSelected { get { return AllowSelectedCore; } }
		protected virtual bool AllowSelectedCore { get { return true; } }
		protected override System.Windows.Forms.Padding GetItemPaddingCore() { return new System.Windows.Forms.Padding(30, 8, 30, 8); }
		#endregion
	}
	class SearchTileBarExpandViewInfo : SearchTileBarItemViewInfoBase {
		public SearchTileBarExpandViewInfo(SearchTileBarExpand item) : base(item) { }
		protected override bool AllowSelectedCore { get { return false; } }
		protected override AppearanceDefault CreateDefaultAppearance() { return ControlInfoCore.OwnerCore.GetDefaultAppearanceItem(ItemCore); }
		public override AppearanceObject GetAppearance() {
			if(!IsHovered) 
				ItemNormalAppearance.ForeColor = Color.FromArgb(100, ItemNormalAppearance.ForeColor);
			return ItemNormalAppearance;
		}
		protected override void PatchDefaultBackColor(AppearanceDefault appDef, AppearanceObject appObj) { }
		protected virtual int LineToImageIndent { get { return 4; } }
		public Point[] LeftLinePoints { get { return CalcLinePoints(true); } }
		public Point[] RightLinePoints { get { return CalcLinePoints(false); } }
		Point[] CalcLinePoints(bool isLeft) {
			if(Elements[0] == null) return new Point[] { Point.Empty, Point.Empty };
			Rectangle imageBounds = Elements[0].ImageContentBounds;
			int xStart = isLeft ? ContentBounds.Left : imageBounds.Right + LineToImageIndent;
			int xEnd = isLeft ? imageBounds.Left - LineToImageIndent : ContentBounds.Right;
			int y = ContentBounds.Y + ContentBounds.Height / 2;
			return new Point[] { new Point(xStart, y), new Point(xEnd, y) };
		}
	}
	class SearchTileBarGroupViewInfo : TileBarGroupViewInfo {
		public SearchTileBarGroupViewInfo(SearchTileBarGroup group) : base(group) { }
		protected override TileItemViewInfoCollection CreateItemsCollection() { return new SearchTileBarItemViewInfoCollection(this); }
		protected override Point CalcTextBounds(Point start, ref Rectangle textBounds) {
			Point location = new Point(start.X + this.Group.Control.Properties.Padding.Left, start.Y);
			if(Items.Count > 0 && ControlInfo.Owner.Properties.ShowGroupText) {
				Size size = new Size(ControlInfo.AppearanceGroupText.CalcTextSize(ControlInfo.GInfo.Graphics, Group.Text, 0).ToSize().Width, ControlInfo.GroupTextHeight);
				textBounds = new Rectangle(location, size);
				start.Y += ControlInfo.GroupTextHeight + ControlInfo.GroupTextToItemsIndent;
			}
			else
				textBounds = new Rectangle(location, Size.Empty);
			return start;
		}
	}
	class SearchTileBarItemViewInfoCollection : TileItemViewInfoCollection {
		public SearchTileBarItemViewInfoCollection(SearchTileBarGroupViewInfo groupInfo) : base(groupInfo) { }
		public override TileItemViewInfo GetItemByPoint(Point pt) {
			foreach(TileItemViewInfo itemInfo in this) {
				if(itemInfo.Bounds.Contains(pt)) {
					if(itemInfo is SearchTileBarExpandViewInfo) return null;
					return itemInfo;
				}
			}
			return null;
		}
	}
}
