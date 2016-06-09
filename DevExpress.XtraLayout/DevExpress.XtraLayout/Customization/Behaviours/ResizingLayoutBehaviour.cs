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
using System.Linq;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using DevExpress.Skins;
using DevExpress.XtraEditors;
using DevExpress.Utils.Drawing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.Painting;
using DevExpress.XtraLayout.Resizing;
namespace DevExpress.XtraLayout.Customization.Behaviours {
	class ResizingLayoutBehaviour :LayoutBaseBehaviour {
		public ResizingLayoutBehaviour(LayoutAdornerWindowHandler handler)
			: base(handler) {
		}
		static void SetSizingTypeByItem(BaseLayoutItem sizingItem) {
			if(!sizingItem.IsGroup) {
				LayoutSizingType setType = sizingItem.sizingTypeCore;
				foreach(BaseLayoutItem item in sizingItem.Parent.Items) item.sizingTypeCore = setType;
			}
		}
		public override void Invalidate() {
			base.Invalidate();
			if(owner.CastedOwner.Disposing) return;
			if(!((ILayoutControl)owner.CastedOwner).DesignMode && !((ILayoutControl)owner.CastedOwner).EnableCustomizationMode) return;
			if(owner.CastedOwner.Root == null || !(owner.CastedOwner.Root.Handler is LayoutGroupHandler)) return;
			LayoutGroupHandler rootHandler = owner.CastedOwner.Root.Handler as LayoutGroupHandler;
			if(rootHandler == null || rootHandler.StartHitTest == null || rootHandler.StartHitTest.Item == null || !rootHandler.StartHitTest.IsSizing || rootHandler.State == LayoutHandlerState.Normal) return;
			BaseLayoutItem sizingItem = rootHandler.StartHitTest.Item;
			SetSizingTypeByItem(sizingItem);
			IEnumerable<BaseLayoutItem> listGroup = owner.CastedOwner.Items.Where(q => (q.SizingType != LayoutSizingType.None && q.Visible && q.Parent == sizingItem.Parent) || q == sizingItem);
			foreach(BaseLayoutItem lci in listGroup.OrderBy(e => e == sizingItem)) {
				glyphs.Add(new ResizingLayoutGlyph(lci, lci == sizingItem, owner.CastedOwner));
			}
		}
	}
	public class ResizingLayoutGlyph :LayoutGlyph {
		static SolidBrush brushForRectangle = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
		static System.Windows.Forms.Padding contentMargin = ResizingDrawingExtension.SkinElement == null ? new System.Windows.Forms.Padding(10) : ResizingDrawingExtension.SkinElement.ContentMargins.ToPadding();
		static Font fontToDraw = new Font(WindowsFormsSettings.DefaultFont.FontFamily, 8.25f, FontStyle.Bold);
		static Pen hotTrackedPen;
		static Pen limitedItemPen;
		static SizeF rectOffsetHorizontal = new SizeF(10, 10);
		static SizeF rectOffsetVertical = new SizeF(10, 10);
		BaseLayoutItem baseLayoutItem;
		bool isHitTestItem = false;
		public ResizingLayoutGlyph(BaseLayoutItem item, bool isHitTestItem, LayoutControl control)
			: base(control) {
			this.baseLayoutItem = item;
			this.isHitTestItem = isHitTestItem;
		}
		static void DrawSizeInRectangle(GraphicsCache cache, BaseLayoutItem baseLayoutItem) {
			string str = string.Empty;
			SizeF strSizeF;
			ConstraintsGlyphTypes constraintsType;
			PointF beginPoint = (baseLayoutItem.Owner as LayoutControl).PointToClient(Cursor.Position);
			RectangleF rectToDraw;
			switch(baseLayoutItem.SizingType) {
				case LayoutSizingType.Vertical:
					constraintsType = ConstraintsVisualizer.CalcGlyphType(baseLayoutItem, Utils.LayoutType.Vertical);
					str = GetStringToDraw(constraintsType, baseLayoutItem.MinSize.Height, baseLayoutItem.MaxSize.Height, baseLayoutItem.Height);
					strSizeF = cache.Graphics.MeasureString(str, fontToDraw);
					rectToDraw = new RectangleF(PointF.Add(beginPoint, rectOffsetVertical), strSizeF);
					rectToDraw.Size = SizeF.Add(rectToDraw.Size, new SizeF(contentMargin.Size.Width, contentMargin.Size.Height));
					cache.DrawRoundRectangle(rectToDraw, 8, brushForRectangle);
					cache.Graphics.DrawString(str, fontToDraw, Brushes.White, PointF.Add(rectToDraw.Location, new Size(contentMargin.Left, contentMargin.Top)));
					break;
				case LayoutSizingType.Horizontal:
					constraintsType = ConstraintsVisualizer.CalcGlyphType(baseLayoutItem, Utils.LayoutType.Horizontal);
					str = GetStringToDraw(constraintsType, baseLayoutItem.MinSize.Width, baseLayoutItem.MaxSize.Width, baseLayoutItem.Width);
					strSizeF = cache.Graphics.MeasureString(str, fontToDraw);
					rectToDraw = new RectangleF(PointF.Add(beginPoint, rectOffsetHorizontal), strSizeF);
					rectToDraw.Size = SizeF.Add(rectToDraw.Size, new SizeF(contentMargin.Size.Width, contentMargin.Size.Height));
					cache.DrawRoundRectangle(rectToDraw, 8, brushForRectangle);
					cache.Graphics.DrawString(str, fontToDraw, Brushes.White, PointF.Add(rectToDraw.Location, new Size(contentMargin.Left, contentMargin.Top)));
					break;
			}
		}
		static string GetStringToDraw(ConstraintsGlyphTypes constraintsType, int minSize, int maxSize, int size) {
			switch(constraintsType) {
				case ConstraintsGlyphTypes.Locked:
					return string.Format("Fixed Size: {0} px", minSize);
				case ConstraintsGlyphTypes.MinSize:
					return string.Format("Min: {0} px", minSize);
				case ConstraintsGlyphTypes.MaxSize:
					return string.Format("Max: {0} px", maxSize);
				default:
					return string.Format("{0} px", size);
			}
		}
		static void SetAntiAliasIfNeed(GraphicsCache GraphicsCache) {
			if(GraphicsCache.Graphics.TextRenderingHint != TextRenderingHint.AntiAlias)
				GraphicsCache.Graphics.TextRenderingHint = TextRenderingHint.AntiAlias;
		}
		public static Pen HotTrackedPen {
			get {
				if(hotTrackedPen == null) {
					hotTrackedPen = new Pen(Color.FromArgb(168, 0, 182, 249));
				}
				return hotTrackedPen;
			}
		}
		public static Pen LimitedItemPen {
			get {
				if(limitedItemPen == null) {
					limitedItemPen = new Pen(Color.FromArgb(168, 246, 82, 240));
				}
				return limitedItemPen;
			}
		}
		public override void Paint(GraphicsCache graphicsCache) {
			SetAntiAliasIfNeed(graphicsCache);
			Rectangle boundsRTC = baseLayoutItem.ViewInfo.BoundsRelativeToControl;
			if(isHitTestItem) {
				GraphicsState state = graphicsCache.Graphics.Save();
				if(layoutControl.CustomizationForm != null) {
					Point customizationFormPoint = (layoutControl as LayoutControl).PointToClient(layoutControl.CustomizationForm.Bounds.Location);
					graphicsCache.Graphics.SetClip(new Rectangle(customizationFormPoint, layoutControl.CustomizationForm.Bounds.Size), CombineMode.Exclude);
				}
				BaseLayoutItemPainter.DrawHotTrackedCore(graphicsCache, baseLayoutItem.ViewInfo.PainterBoundsRelativeToControl, HotTrackedPen, baseLayoutItem);
				graphicsCache.Graphics.Restore(state);
				DrawSizeInRectangle(graphicsCache, baseLayoutItem);
			} else {
				if(layoutControl.DesignMode) return;
				ConstraintsGlyphTypes constraintsType = ConstraintsVisualizer.CalcGlyphType(baseLayoutItem, baseLayoutItem.SizingType == LayoutSizingType.Vertical ? LayoutType.Vertical : LayoutType.Horizontal);
				if(constraintsType == ConstraintsGlyphTypes.NoConstraints) return;
				GraphicsState state = graphicsCache.Graphics.Save();
				if(layoutControl.CustomizationForm != null) {
					Point customizationFormPoint = (layoutControl as LayoutControl).PointToClient(layoutControl.CustomizationForm.Bounds.Location);
					graphicsCache.Graphics.SetClip(new Rectangle(customizationFormPoint, layoutControl.CustomizationForm.Bounds.Size), CombineMode.Exclude);
				}
				BaseLayoutItemPainter.DrawHotTrackedCore(graphicsCache, baseLayoutItem.ViewInfo.PainterBoundsRelativeToControl, LimitedItemPen, baseLayoutItem);
				graphicsCache.Graphics.Restore(state);
			}
		}
	}
	class DevExpressStyleDefaultSkinProvider :ISkinProvider {
		public string SkinName {
			get { return "DevExpress Style"; }
		}
	}
	static class ResizingDrawingExtension {
		static DevExpressStyleDefaultSkinProvider DefaultSkinProvider = new DevExpressStyleDefaultSkinProvider();
		static SkinElement skinElementCore;
		static SkinElement CreateSkinElement() {
			Skin skin = CommonSkins.GetSkin(DefaultSkinProvider);
			return CommonSkins.GetSkin(DefaultSkinProvider)[CommonSkins.SkinLayoutResizingGlyph];
		}
		static void DrawRoundRectangleWithoutSkin(this GraphicsCache cache, RectangleF bounds, float round, Brush myBrush) {
			GraphicsPath gp = new GraphicsPath();
			gp.AddArc(bounds.X, bounds.Y, round, round, 180, 90);
			gp.AddArc(bounds.X + bounds.Width - round, bounds.Y, round, round, 270, 90);
			gp.AddArc(bounds.X + bounds.Width - round, bounds.Y + bounds.Height - round, round, round, 0, 90);
			gp.AddArc(bounds.X, bounds.Y + bounds.Height - round, round, round, 90, 90);
			SmoothingMode old = cache.Graphics.SmoothingMode;
			cache.Graphics.SmoothingMode = SmoothingMode.HighQuality;
			cache.Graphics.FillPath(myBrush, gp);
			cache.Graphics.SmoothingMode = old;
		}
		internal static PointF BottomEdgeMiddlePoint(this Rectangle rect) {
			return new PointF(rect.X + rect.Width / 2, rect.Bottom);
		}
		internal static PointF CenterOfRectangle(this Rectangle rect) {
			return new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
		}
		internal static void DrawRoundRectangle(this GraphicsCache cache, RectangleF bounds, float round, Brush myBrush) {
			try {
				SkinElementInfo sei = new SkinElementInfo(SkinElement, Rectangle.Round(bounds));
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, sei);
			} catch {
				DrawRoundRectangleWithoutSkin(cache, bounds, round, myBrush);
			}
		}
		internal static PointF LeftEdgeMiddlePoint(this Rectangle rect) {
			return new PointF(rect.X, rect.Y + rect.Height / 2);
		}
		internal static PointF RightEdgeMiddlePoint(this Rectangle rect) {
			return new PointF(rect.Right, rect.Y + rect.Height / 2);
		}
		internal static PointF SetOffset(this PointF pointF, SizeF offset) {
			return new PointF(pointF.X - offset.Width, pointF.Y - offset.Height);
		}
		internal static PointF TopEdgeMiddlePoint(this Rectangle rect) {
			return new PointF(rect.X + rect.Width / 2, rect.Y);
		}
		internal static SkinElement SkinElement {
			get {
				if(skinElementCore == null) skinElementCore = CreateSkinElement();
				return skinElementCore;
			}
		}
	}
}
