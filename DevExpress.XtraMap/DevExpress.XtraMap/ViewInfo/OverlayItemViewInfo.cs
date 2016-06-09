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

using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Drawing {
	public abstract class OverlayItemViewInfoBase : SelfpaintingViewinfo, IViewInfoSupportAlignment, IViewinfoInteractionControllerProvider {
		readonly MapOverlayItemBase item;
		Rectangle contentBounds;
		readonly IViewinfoInteractionController interactionController = new RectangularViewinfoInteractionController();
		protected internal Rectangle ContentBounds { get { return contentBounds; } }
		protected internal virtual bool ShouldUseItemSize { get { return item.Size != MapOverlayItemBase.DefaultSize; } }
		protected internal MapOverlayItemBase Item { get { return item; } }
		public bool HotTracked { get { return interactionController.HotTracked; } }
		protected OverlayItemViewInfoBase(InnerMap map, MapOverlayItemBase item, MapStyleCollection viewInfoPaintAppearance, OverlayItemPainter painter)
			: base(map, viewInfoPaintAppearance, painter) {
			this.item = item;
		}
		#region IViewInfoSupportAlignment implementation
		Rectangle IViewInfoSupportAlignment.LayoutRect {
			get { return Bounds; }
			set {
				Bounds = value;
				OnLayoutRectChanged();
			}
		}
		ContentAlignment IViewInfoSupportAlignment.Alignment { get { return item.Alignment; } }
		Orientation IViewInfoSupportAlignment.JoiningOrientation { get { return item.JoiningOrientation; } }
		#endregion
		#region IViewinfoInteractionControllerProvider implementation
		IViewinfoInteractionController IViewinfoInteractionControllerProvider.InteractionController { get { return interactionController; } }
		#endregion
		Size CalculateOverlayItemSize() {
			return ShouldUseItemSize ? item.Size : CalculateOverlayItemSizeCore();
		}
		void OnLayoutRectChanged() {
			Size actualSize = CalculateOverlayItemSize();
			ClientBounds = new Rectangle(Bounds.X + item.Margin.Left, Bounds.Y + item.Margin.Top, actualSize.Width + item.Padding.Horizontal, actualSize.Height + item.Padding.Vertical);
			this.contentBounds = new Rectangle(ClientBounds.X + item.Padding.Left, ClientBounds.Y + item.Padding.Top, actualSize.Width, actualSize.Height);
		}
		internal void MoveLayout(Point offset) {
			Bounds = RectUtils.Offset(Bounds, offset);
			ClientBounds = RectUtils.Offset(ClientBounds, offset);
			this.contentBounds = RectUtils.Offset(contentBounds, offset);
		}
		internal void ApplyLayout(Rectangle bounds) {
			ClientBounds = bounds;
			Bounds = bounds;
			this.contentBounds = new Rectangle(Bounds.X + Item.Padding.Left, Bounds.Y + Item.Padding.Top, Bounds.Width - Item.Padding.Horizontal, Bounds.Height - Item.Padding.Vertical);
		}
		internal void UpdateBounds() {
			Bounds = Item.Visible ? new Rectangle(item.Location, CalculateOverlayItemSize() + item.Padding.Size + item.Margin.Size) : Rectangle.Empty;
			if(Item.Visible)
				return;
			ClientBounds = Rectangle.Empty;
			this.contentBounds = Rectangle.Empty;
		}
		protected override void OnClientBoundsChanged() {
			base.OnClientBoundsChanged();
			interactionController.Bounds = ClientBounds;
		}
		protected abstract Size CalculateOverlayItemSizeCore();
		public void Draw(GraphicsCache cache) {
			Painter.Draw(cache, this);
		}
		public void Calculate(Point mouseClientPosition) {
			interactionController.RecalculateHotTracked(mouseClientPosition);
		}
	}
	public class OverlayTextItemViewInfo : OverlayItemViewInfoBase {
		readonly MapStyleCollection itemItemUserAppearance;
		bool ShouldCalculateHeight { get { return Item.Size.Height == 0 && Item.Size.Width != 0; } }
		protected internal new MapOverlayTextItem Item { get { return (MapOverlayTextItem)base.Item; } }
		protected internal override bool ShouldUseItemSize { get { return base.ShouldUseItemSize && !ShouldCalculateHeight; } }
		protected override MapStyleCollection DefaultAppearance { get { return Painter.ViewInfoAppearanceProvider.DefaultOverlayTextAppearance; } }
		protected override MapStyleCollection UserAppearance { get { return itemItemUserAppearance; } }
		public OverlayTextItemViewInfo(InnerMap map, MapOverlayTextItem item, OverlayTextItemPainter painter)
			: base(map, item, new OverlayTextAppearance(null), painter) {
			this.itemItemUserAppearance = item.Appearance;
		}
		protected override Size CalculateOverlayItemSizeCore() {
			TextElementStyle textStyle = ((OverlayTextAppearance)PaintAppearance).TextStyle;
			if(ShouldCalculateHeight) {
				int height = MapUtils.CalcStringPixelSize(Item.Text, textStyle.Font, Item.Size.Width).Height;
				return new Size(Item.Size.Width, height);
			}
			return MapUtils.CalcStringPixelSize(Item.Text, textStyle.Font);
		}
	}
	public class OverlayImageItemViewInfo : OverlayItemViewInfoBase {
		protected internal new MapOverlayImageItem Item { get { return (MapOverlayImageItem)base.Item; } }
		protected override MapStyleCollection DefaultAppearance { get { return EmptyAppearance.Instance; } }
		public OverlayImageItemViewInfo(InnerMap map, MapOverlayImageItem item, OverlayImageItemPainter painter)
			: base(map, item, new OverlayItemAppearance(null), painter) {
		}
		protected override Size CalculateOverlayItemSizeCore() {
			return ImageSafeAccess.GetSize(Item.GetActualImage());
		}
	}
	public abstract class OverlayItemPainter : OverlayViewInfoPainter {
		protected OverlayItemPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			OverlayItemViewInfoBase vi = (OverlayItemViewInfoBase)viewInfo;
			OverlayItemAppearance appearance = (OverlayItemAppearance)viewInfo.PaintAppearance;
			cache.FillRectangle(vi.HotTracked ? appearance.HotTrackedItemStyle.Fill : appearance.ItemStyle.Fill, viewInfo.ClientBounds);
		}
	}
	public class OverlayTextItemPainter : OverlayItemPainter {
		public OverlayTextItemPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			base.Draw(cache, viewInfo);
			OverlayTextAppearance paintAppearance = (OverlayTextAppearance)(viewInfo.PaintAppearance);
			OverlayTextItemViewInfo vi = (OverlayTextItemViewInfo)viewInfo;
			cache.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
			using(SolidBrush brush = new SolidBrush(paintAppearance.TextStyle.TextColor))
			using(StringFormat stringFormat = MapUtils.GetAlignedStringFormat(vi.Item.TextAlignment)) {
				MapUtils.TextPainter.DrawString(cache, vi.Item.Text, paintAppearance.TextStyle.Font, brush, vi.ContentBounds, stringFormat);
			}
		}
	}
	public class OverlayImageItemPainter : OverlayItemPainter {
		public OverlayImageItemPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			base.Draw(cache, viewInfo);
			OverlayImageItemViewInfo vi = (OverlayImageItemViewInfo)viewInfo;
			ImageSafeAccess.Draw(cache.Graphics, vi.Item.GetActualImage(), vi.ContentBounds);
		}
	}
}
