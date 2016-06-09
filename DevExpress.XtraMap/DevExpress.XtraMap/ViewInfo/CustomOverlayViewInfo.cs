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

using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraMap.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace DevExpress.XtraMap.Drawing {
	public class CustomOverlayViewInfo : OverlayViewInfoBase, ISupportIndexOverlay, IViewInfoSupportAlignment, ISupportViewinfoLayout, IHitTestableViewinfo {
		const int MaxCustomOverlayIndex = 1000;
		readonly MapOverlay overlay;
		readonly OverlayItemViewInfoBase[] overlayItemViewinfos;
		readonly MapStyleCollection overlayUserAppearance;
		int overlayIndex;
		protected override int KeyInternal { get { return 0x10000; }}
		public new CustomOverlayPainter Painter { get { return (CustomOverlayPainter)base.Painter; } }
		protected override MapStyleCollection UserAppearance { get { return overlayUserAppearance; } }
		protected override MapStyleCollection DefaultAppearance { get { return Painter.ViewInfoAppearanceProvider.DefaultCustomOverlayAppearance; } }
		public override ViewInfoUpdateType SupportedUpdateType { get { return ViewInfoUpdateType.CustomOverlay; } }
		public OverlayItemViewInfoBase[] OverlayItemViewinfos { get { return overlayItemViewinfos; } }
		public int OverlayIndex {
			get { return overlayIndex; }
			set {
				if(value > MaxCustomOverlayIndex || value < 0)
					throw new ArgumentException("CustomOverlayIndex");
				overlayIndex = value;
			}
		}
		public CustomOverlayViewInfo(InnerMap map, MapOverlay overlay, MapUIElementsPainter painter)
			: base(map, new CustomOverlayAppearance(null), painter.CustomOverlayPainter) {
			Guard.ArgumentNotNull(overlay, "Overlay");
			this.overlay = overlay;
			this.overlayUserAppearance = overlay.Appearance;
			this.overlayItemViewinfos = new OverlayItemViewInfoBase[overlay.Items.Count];
			for(int i = 0; i < overlayItemViewinfos.Length; i++)
				overlayItemViewinfos[i] = overlay.Items[i].CreateViewinfo(map, painter);
		}
		#region ISupportIndexOverlay implementation
		int ISupportIndexOverlay.Index {
			get { return OverlayIndex; }
			set { OverlayIndex = value; }
		}
		int ISupportIndexOverlay.MaxIndex {
			get { return MaxCustomOverlayIndex; }
		}
		#endregion
		#region IViewInfoSupportAlignment implementation
		Rectangle IViewInfoSupportAlignment.LayoutRect {
			get { return Bounds; }
			set { Bounds = value; }
		}
		ContentAlignment IViewInfoSupportAlignment.Alignment { get { return overlay.Alignment; } }
		Orientation IViewInfoSupportAlignment.JoiningOrientation { get { return overlay.JoiningOrientation; } }
		#endregion
		#region ISupportViewinfoLayout implementation
		OverlayArrangement ISupportViewinfoLayout.CreateOverlayLayout(ViewInfoUpdateType updateType) {
			if(!updateType.HasFlag(SupportedUpdateType))
				return null;
			Rectangle[] overlayItemsBounds = new Rectangle[OverlayItemViewinfos.Length];
			for(int i = 0; i < overlayItemsBounds.Length; i++) {
				Size marginOffset = new Size(overlayItemViewinfos[i].Item.Margin.Left, overlayItemViewinfos[i].Item.Margin.Top);
				overlayItemsBounds[i] = new Rectangle(overlayItemViewinfos[i].Bounds.Location + marginOffset, overlayItemViewinfos[i].ClientBounds.Size);
			}
			return new OverlayArrangement(overlayItemsBounds) { OverlayLayout = new Rectangle(Bounds.Location + (Size)ClientBounds.Location, ClientBounds.Size) };
		}
		void ISupportViewinfoLayout.ApplyLayout(OverlayArrangement overlayLayout) {
			Bounds = overlayLayout.OverlayLayout;
			ClientBounds = new Rectangle(Point.Empty, Bounds.Size);
			for(int i = 0; i < overlayLayout.ItemLayouts.Length; i++)
				OverlayItemViewinfos[i].ApplyLayout(overlayLayout.ItemLayouts[i]);
		}
		#endregion
		#region IHitTestableViewinfo implementation
		IMapUiHitInfo IHitTestableViewinfo.CalcHitInfo(Point point) {
			Point clientHitPoint = new Point(point.X - Bounds.X, point.Y - Bounds.Y);
			if(!ClientBounds.Contains(clientHitPoint))
				return null;
			return new MapOverlayHitInfo(clientHitPoint, MapHitUiElementType.Overlay, overlay, GetHittedOverlayItem(clientHitPoint));
		}
		#endregion
		Rectangle CalculateBounds(Rectangle controlBounds) {
			Rectangle result = new Rectangle();
			Size overlaySize = CalculateOverlaySize();
			result.Size = overlaySize + new Size(overlay.Margin.Horizontal + overlay.Padding.Horizontal, overlay.Margin.Vertical + overlay.Padding.Vertical);
			result.Location = overlay.Location == MapOverlay.DefaultLocation ? Point.Empty : overlay.Location;
			return result;
		}
		Size CalculateItemsSize() {
			foreach(OverlayItemViewInfoBase itemViewinfo in overlayItemViewinfos)
				itemViewinfo.UpdateBounds();
			ViewInfoLayoutCalculator layoutCalculator = new ViewInfoLayoutCalculator();
			foreach(OverlayItemViewInfoBase itemViewinfo in overlayItemViewinfos)
				if(itemViewinfo.Item.Visible)
					layoutCalculator.Add(itemViewinfo);
			if(layoutCalculator.CanAlign)
				layoutCalculator.Align();
			foreach(OverlayItemViewInfoBase itemViewinfo in overlayItemViewinfos)
				itemViewinfo.MoveLayout(new Point(overlay.Padding.Left, overlay.Padding.Top));
			return layoutCalculator.ViewinfosSize;
		}
		MapOverlayItemBase GetHittedOverlayItem(Point clientHitPoint) {
			foreach(OverlayItemViewInfoBase overlayItemViewinfo in OverlayItemViewinfos)
				if(overlayItemViewinfo.ClientBounds.Contains(clientHitPoint))
					return overlayItemViewinfo.Item;
			return null;
		}
		Size GetOverlaySize(Size itemsSize) {
			return overlay.Size == MapOverlay.DefaultSize ? itemsSize : overlay.Size;
		}
		internal Size CalculateOverlaySize() {
			Size desiredSize = CalculateItemsSize();
			return GetOverlaySize(desiredSize);
		}
		protected override void CalculateNestedPaintAppearance() {
			base.CalculateNestedPaintAppearance();
			foreach(OverlayItemViewInfoBase itemViewinfo in OverlayItemViewinfos)
				itemViewinfo.CalculatePaintAppearance();
		}
		protected internal override void CalculateLayout(Rectangle controlBounds) {
			base.CalculateLayout(controlBounds);
			Bounds = CalculateBounds(controlBounds);
			ClientBounds = new Rectangle(overlay.Margin.Left, overlay.Margin.Top, Bounds.Size.Width - overlay.Margin.Horizontal, Bounds.Size.Height - overlay.Margin.Vertical);
		}
		protected internal override void CalculateOverlay(Graphics gr, Rectangle controlBounds) {
			base.CalculateOverlay(gr, controlBounds);
			Point mouseClientPosition = new Point(MousePosition.X - Bounds.Left, MousePosition.Y - Bounds.Top);
			foreach(OverlayItemViewInfoBase overlayItemViewinfo in OverlayItemViewinfos) {
				overlayItemViewinfo.Calculate(mouseClientPosition);
			}
		}
	}
	public class CustomOverlayPainter : OverlayViewInfoPainter {
		public CustomOverlayPainter(IViewInfoStyleProvider provider)
			: base(provider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			CustomOverlayViewInfo vi = (CustomOverlayViewInfo)viewInfo;
			CustomOverlayAppearance paintAppearance = (CustomOverlayAppearance)(viewInfo.PaintAppearance);
			BackgroundStyle bgStyle = paintAppearance.BackgroundStyle;
			cache.FillRectangle(bgStyle.Fill, viewInfo.ClientBounds);
			foreach(OverlayItemViewInfoBase itemViewinfo in vi.OverlayItemViewinfos)
				itemViewinfo.Draw(cache);
		}
	}
}
