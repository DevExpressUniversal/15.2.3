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

using DevExpress.XtraBars.Ribbon.ViewInfo;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using System.Windows.Forms;
using DevExpress.XtraBars.Ribbon.Helpers;
using DevExpress.XtraBars.Utils;
using DevExpress.Utils.Drawing.Animation;
using System.Collections.Generic;
namespace DevExpress.XtraBars.Ribbon.ViewInfo {
	public class RibbonMiniToolbarViewInfo : BaseRibbonViewInfo  ,IRibbonGroupInfo {
		RibbonMiniToolbarControl control;
		RibbonItemViewInfoCollection items;
		int precalculatedWidth;
		public RibbonMiniToolbarViewInfo(RibbonMiniToolbarControl control) {
			this.control = control;
			this.items = CreateItems();
		}
		public BarButtonItemLink ContentButtonLink { get { return null; } }
		protected virtual RibbonItemViewInfoCollection CreateItems() {
			return new RibbonItemViewInfoCollection(this);
		}
		protected internal virtual bool ContainsLink(BarItemLink link) {
			return ContainsLink(link, Items);
		}
		protected internal virtual bool ContainsLink(BarItemLink link, RibbonItemViewInfoCollection coll) {
			return ContainsLink(link, coll, false);
		}
		protected internal virtual bool ContainsLink(BarItemLink link, RibbonItemViewInfoCollection coll, bool compareClonedLinks) {
			foreach(RibbonItemViewInfo itemInfo in coll) {
				BarItemLink l = (BarItemLink)itemInfo.Item;
				if(itemInfo.Item == link)
					return true;
				if(compareClonedLinks && link != null && l.ClonedFromLink != null && (l.ClonedFromLink == link || l.ClonedFromLink == link.ClonedFromLink))
					return true;
				RibbonButtonGroupItemViewInfo groupInfo = itemInfo as RibbonButtonGroupItemViewInfo;
				if(groupInfo != null && ContainsLink(link, groupInfo.Items, true))
					return true;
			}
			return false;
		}
		RibbonPageGroupItemsLayout IRibbonGroupInfo.ItemsLayout { get { return RibbonPageGroupItemsLayout.TwoRows; } }
		BarItemLinkCollection IRibbonGroupInfo.ItemLinks {
			get { return Toolbar.ItemLinks; }
		}
		CustomAnimationInvoker animationInvoker;
		public CustomAnimationInvoker AnimationInvoker {
			get {
				if(animationInvoker == null) animationInvoker = new CustomAnimationInvoker(Control.OnAnimation);
				return animationInvoker;
			}
		}
		protected internal override RibbonItemViewInfo CreateItemViewInfo(IRibbonItem item) {
			RibbonItemViewInfo vi = Manager.CreateItemViewInfo(this, item);
			BarEditItemLink link = item as BarEditItemLink;
			if(link == null || IsDesignMode) return vi;
			BarAnimatedItemsHelper.UpdateAnimatedItem(Control, AnimationInvoker, vi as RibbonEditItemViewInfo);
			return vi;
		}
		public bool HasLargeItems { get { return RibbonPageGroupViewInfo.HasLargeItemsCore(this); } }
		public bool HasGallery { get { return RibbonPageGroupViewInfo.HasGalleryCore(this); } }
		public override void CalcViewInfo(Rectangle bounds) {
			if(IsReady || Ribbon == null)
				return;
			Ribbon.CheckViewInfo();
			GInfo.AddGraphics(null);
			try {
				CalcConstants();
				Items.Clear();
				List<BarItemLink> links = InplaceLinksHelper.GetLinks(Ribbon.Manager, Control.Toolbar.ItemLinks, Control.Ribbon.AllowInplaceLinks, Control.Ribbon.IsDesignMode, (link) => Control.ViewInfo.ShouldCreateItemInfo(link));
				RibbonPageGroupViewInfo.CreateItemsViewInfo(this, links);
				RibbonMitiToolbarLayoutCalculator calc = new RibbonMitiToolbarLayoutCalculator(this);
				this.groupContentHeight = 0;
				this.groupContentHeight = calc.CalcGroupContentHeight(ViewInfo);
				SkinElementInfo backInfo = GetBackgroundInfo();
				Bounds = new Rectangle(Point.Empty, ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, backInfo, new Rectangle(0, 0, 10000, GroupContentHeight)).Size);
				backInfo.Bounds = Bounds;
				ContentBounds = ObjectPainter.GetObjectClientRectangle(GInfo.Graphics, SkinElementPainter.Default, backInfo);
				this.precalculatedWidth = CalcBestWidth(GroupContentHeight);
				ContentBounds = new Rectangle(ContentBounds.Location, new Size(PrecalculatedWidth, ContentBounds.Height));
				Bounds = ObjectPainter.CalcBoundsByClientRectangle(GInfo.Graphics, SkinElementPainter.Default, backInfo, ContentBounds);
				if(IsRightToLeft) {
					foreach(RibbonItemViewInfo item in Items)
						item.Bounds = BarUtilites.ConvertBoundsToRTL(item.Bounds, ContentBounds);
				}
				IsReady = true;
			}
			finally {
				GInfo.ReleaseGraphics();
			}
		}
		internal override bool IsRightToLeft { get { return ViewInfo.IsRightToLeft; } }
		public override RibbonBarManager Manager { get { return Ribbon == null? null: Ribbon.Manager; } }
		public override RibbonHitInfo CalcHitInfo(Point hitPoint) {
			RibbonHitInfo hi = new RibbonHitInfo();
			hi.HitPoint = hitPoint;
			foreach(RibbonItemViewInfo itemInfo in Items) {
				if(itemInfo.Bounds.Contains(hi.HitPoint)) {
					itemInfo.CalcHitInfo(hi);
				}
			}
			return hi;
		}
		public override bool IsRibbonFormActive {
			get {
				return Toolbar.Form.Visible;
			}
		}
		public RibbonItemViewInfoCollection Items { get { return items; } }
		public RibbonMiniToolbarControl Control { get { return control; } }
		public bool IsButtonGroup(int index) {
			return RibbonPageGroupViewInfo.IsButtonGroup(this, index);
		}
		public RibbonViewInfo ViewInfo { get { return Control.Ribbon.ViewInfo; } }
		public bool AllowHorizontalCenterdItems { get { return false; } }
		public int DefaultIndentBetweenButtonGroups { get { return ViewInfo.DefaultIndentBetweenButtonGroups; } }
		public int DefaultIndentBetweenColumns { get { return ViewInfo.DefaultIndentBetweenColumns; } }
		public VertAlignment ButtonGroupsVertAlign { get { return Control.Ribbon.ButtonGroupsVertAlign; } }
		public VertAlignment ItemsVertAlign { get { return VertAlignment.Center; } }
		public bool AllowMinimize { get { return false; } }
		public bool Minimized { get { return false; } }
		public int PrecalculatedWidth { get { return precalculatedWidth; } set { precalculatedWidth = value; } }
		public int CalcMinimizedWidth() { return 0; }
		public void Minimize() { }
		public int LargeRibbonButtonHeight { get { return ViewInfo.SingleLineLargeButtonHeight; } }
		public bool IsSingleLineLargeButton { get { return true; } }
		int groupContentHeight;
		public int GroupContentHeight { get { return groupContentHeight; } }
		public int CalcBestWidth(int contentHeight) {
			RibbonMitiToolbarLayoutCalculator calc = new RibbonMitiToolbarLayoutCalculator((IRibbonGroupInfo)this);
			calc.CalcSimpleLayout();
			int width = calc.UpdateGroupLayout(ContentBounds);
			return width;
		}
		public RibbonSeparatorItemViewInfo CreateSeparatorViewInfo(IRibbonItem item) { 
			return new RibbonSeparatorItemViewInfo(this, item);
		}
		protected internal virtual SkinElementInfo GetBackgroundInfo() {
			SkinElement elem = RibbonSkins.GetSkin(ViewInfo.Provider)[RibbonSkins.SkinQuickToolbarDropDown];
			return new SkinElementInfo(elem, Bounds);
		}
		public RibbonControl Ribbon { get { return Control.Ribbon; } }
		public RibbonMiniToolbar Toolbar { get { return Control.Toolbar; } }
		public override RibbonAppearances PaintAppearance {
			get { return Ribbon.ViewInfo.PaintAppearance; }
		}
		protected override RibbonItemViewInfo FindItem(IRibbonItem item, Rectangle bounds) {
			return Items.Find(item, bounds);
		}
		protected override DevExpress.XtraBars.Ribbon.Handler.BaseHandler Handler {
			get { return Control.Handler; }
		}
		public override bool IsDesignMode {
			get { return Ribbon.IsDesignMode; }
		}
		protected internal override void OnItemChanged(RibbonItemViewInfo item) {
			if(!IsReady)
				return;
			Toolbar.Form.UpdateToolbarBounds();
			Control.Refresh();
		}
		public override Control OwnerControl {
			get { return Control; }
		}
		public override void Invalidate(RibbonItemViewInfo itemInfo) {
			Control.Invalidate(itemInfo.Bounds);
		}
		public override void Invalidate(RibbonHitInfo hitInfo) {
			if(hitInfo.HitTest == RibbonHitTest.None)
				return;
			InRibbonGalleryViewInfo inRibbonViewInfo = hitInfo.GalleryInfo as InRibbonGalleryViewInfo;
			if(hitInfo.HitTest == RibbonHitTest.Item)
				Control.Invalidate(hitInfo.ItemInfo.Bounds);
			else if(inRibbonViewInfo != null) {
				if(hitInfo.HitTest == RibbonHitTest.GalleryDownButton)
					Control.Invalidate(inRibbonViewInfo.ButtonUpBounds);
				else if(hitInfo.HitTest == RibbonHitTest.GalleryDownButton)
					Control.Invalidate(inRibbonViewInfo.ButtonDownBounds);
				else if(hitInfo.HitTest == RibbonHitTest.GalleryDropDownButton)
					Control.Invalidate(inRibbonViewInfo.ButtonCommandBounds);
				else {
					Rectangle rect = hitInfo.GalleryInfo.Bounds;
					rect.Height = Bounds.Bottom - rect.Y;
					Control.Invalidate(rect);
				}
			}
			else
				Control.Invalidate();
		}
		public override ISkinProvider Provider {
			get { return Ribbon.ViewInfo.Provider; }
		}
	}
}
namespace DevExpress.XtraBars.Ribbon.Drawing {
	public class RibbonMiniToolbarPainter {
		public void Draw(GraphicsCache cache, RibbonMiniToolbarViewInfo viewInfo) {
			DrawBackground(cache, viewInfo);
			DrawItems(cache, viewInfo);
		}
		protected virtual void DrawBackground(GraphicsCache cache, RibbonMiniToolbarViewInfo viewInfo) {
			ObjectPainter.DrawObject(cache, SkinElementPainter.Default, viewInfo.GetBackgroundInfo());
		}
		protected virtual void DrawItems(GraphicsCache cache, RibbonMiniToolbarViewInfo viewInfo) {
			foreach(RibbonItemViewInfo itemInfo in viewInfo.Items) {
				RibbonItemViewInfoCalculator.DrawItem(cache, itemInfo);
			}
		}
	}	
}
