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

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Docking2010.Views.WindowsUI;
namespace DevExpress.XtraBars.Docking2010.Views.Widget {
	public class WidgetViewPainter : BaseViewPainter {
		const int DefaultAngle = 270;
		static Padding defaultPadding = new Padding(10, 10, 10, 10);
		public WidgetViewPainter(WidgetView view)
			: base(view) {
		}
		public WidgetViewInfo Info {
			get { return View.ViewInfo as WidgetViewInfo; }
		}
		public WidgetView WidgetView {
			get { return View as WidgetView; }
		}
		public AppearanceObject PaintAppearanceCaption { get; set; }
		AppearanceDefault defaultAppearanceCaptionCore;
		public AppearanceDefault DefaultAppearanceCaption {
			get {
				if(defaultAppearanceCaptionCore == null)
					defaultAppearanceCaptionCore = CreateDefaultAppearanceCaption();
				return defaultAppearanceCaptionCore;
			}
		}
		protected virtual AppearanceDefault CreateDefaultAppearanceCaption() {
			return new AppearanceDefault(SystemColors.ControlText, SystemColors.Control, SegoeUIFontsCache.GetFont("Segoe UI", 12f));
		}
		public Rectangle GetCaptionBoundsByClientRect(Rectangle client) {
			client.X -= defaultPadding.Left;
			client.Y -= defaultPadding.Top;
			client.Width += defaultPadding.Vertical;
			client.Height += defaultPadding.Horizontal;
			return client;
		}
		public Rectangle GetCaptionClientRect(Rectangle bounds) {
			bounds.X += defaultPadding.Left;
			bounds.Y += defaultPadding.Top;
			bounds.Width -= defaultPadding.Vertical;
			bounds.Height -= defaultPadding.Horizontal;
			return bounds;
		}
		protected override void DrawCore(GraphicsCache bufferedCache, Rectangle clip) {
			DrawBackground(bufferedCache, clip);
			DrawTableGroup(bufferedCache, clip);
			DrawStackGroup(bufferedCache, clip);
			DrawFlowLayoutGroup(bufferedCache, clip);
		}
		protected virtual void DrawFlowLayoutGroup(GraphicsCache bufferedCache, Rectangle clip) {
			if(WidgetView.LayoutMode != LayoutMode.FlowLayout) return;
			Rectangle emptyDocumentBounds = WidgetView.FlowLayoutGroup.Info.GetEmptyDocumentBounds();
			if(!emptyDocumentBounds.IsEmpty)
				DrawEmptyDocument(bufferedCache, emptyDocumentBounds);
		}
		bool? isDesignModeCore;
		public bool IsDesignMode {
			get {
				if(isDesignModeCore == null)
					isDesignModeCore = (View.Site != null && View.Site.DesignMode) || ((IDesignTimeSupport)View).IsLoaded;
				return isDesignModeCore.Value;
			}
		}
		protected virtual void DrawTableGroup(GraphicsCache bufferedCache, Rectangle clip) {
			if(WidgetView.LayoutMode == LayoutMode.StackLayout) return;
			if(IsDesignMode)
				DrawTabElementsBorder(bufferedCache);
			foreach(Document document in WidgetView.TableGroup.Items) {
				if(document is EmptyDocument && !document.Info.Bounds.IsEmpty && (document as EmptyDocument).Sizing) {
					DrawEmptyDocument(bufferedCache, document.Info.Bounds);
				}
				if(document is EmptyDocument) {
					Rectangle bounds = WidgetView.TableGroup.Info.GetTargetBounds(document);
					DrawEmptyDocument(bufferedCache, bounds);
				}
			}
		}
		protected virtual void DrawTabElementsBorder(GraphicsCache bufferedCache) {
			foreach(var columnInfo in WidgetView.TableGroup.Info.ColumnDefinitionInfos.Values) {
				int rightBorderX = columnInfo.Bounds.Right == View.Bounds.Right ? View.Bounds.Right - 1 : columnInfo.Bounds.Right;
				DrawBorderLine(bufferedCache, columnInfo.Bounds.Location, new Point(columnInfo.Bounds.X, columnInfo.Bounds.Bottom));
				DrawBorderLine(bufferedCache, new Point(rightBorderX, columnInfo.Bounds.Top), new Point(rightBorderX, columnInfo.Bounds.Bottom));
			}
			foreach(var item in WidgetView.TableGroup.Info.RowDefinitionInfos.Values) {
				int bottomBorderY = item.Bounds.Bottom == View.Bounds.Bottom ? item.Bounds.Bottom - 1 : item.Bounds.Bottom;
				DrawBorderLine(bufferedCache, item.Bounds.Location, new Point(item.Bounds.Right, item.Bounds.Y));
				DrawBorderLine(bufferedCache, new Point(item.Bounds.X, bottomBorderY), new Point(item.Bounds.Right, bottomBorderY));
			}
		}
		protected virtual void DrawStackGroup(GraphicsCache bufferedCache, Rectangle clip) {
			if(WidgetView.LayoutMode != LayoutMode.StackLayout) return;
			foreach(StackGroupInfo stackGroupInfo in Info.StackGroupInfos.Values) {
				if(stackGroupInfo.Group.ActualLength == 0) continue;
				if(IsDesignMode)
					DrawGroupBorder(bufferedCache, stackGroupInfo.Bounds);
				DrawCaption(bufferedCache, stackGroupInfo);
				Rectangle emptyDocumentBounds = stackGroupInfo.GetEmptyDocumentBounds();
				if(!emptyDocumentBounds.IsEmpty)
					DrawEmptyDocument(bufferedCache, emptyDocumentBounds);
			}
		}
		protected virtual void DrawCaption(GraphicsCache bufferedCache, StackGroupInfo stackGroupInfo) {
			var stringFormat = stackGroupInfo.PaintAppearance.GetStringFormat().Clone() as StringFormat;
			if(stackGroupInfo.PaintAppearance.TextOptions.HAlignment == DevExpress.Utils.HorzAlignment.Default)
				stringFormat.Alignment = StringAlignment.Center;
			if(WidgetView.Orientation == Orientation.Vertical) {
				stackGroupInfo.PaintAppearance.DrawString(bufferedCache, stackGroupInfo.Group.Caption, stackGroupInfo.TextBounds, stringFormat);
			}
			else {
				stackGroupInfo.PaintAppearance.DrawVString(bufferedCache, stackGroupInfo.Group.Caption,
					stackGroupInfo.PaintAppearance.GetFont(), stackGroupInfo.PaintAppearance.GetForeBrush(bufferedCache),
					stackGroupInfo.TextBounds, stringFormat, DefaultAngle);
			}
		}
		Point p1Cache;
		Point p2Cache;
		protected void DrawBorderLine(GraphicsCache bufferedCache, Point p1, Point p2) {
			if(p1 == p1Cache && p2 == p2Cache) return;
			bufferedCache.Graphics.DrawLine(BorderPen, p1, p2);
			p1Cache = p1;
			p2Cache = p2;
		}
		Pen BorderPen = new Pen(Color.Red) { DashPattern = new float[] { 5.0f, 5.0f } };
		protected void DrawGroupBorder(GraphicsCache bufferedCache, Rectangle bounds) {
			bounds = Rectangle.Intersect(View.Bounds, bounds);
			Rectangle viewBounds = View.Bounds;
			bufferedCache.Graphics.DrawRectangle(BorderPen, bounds);
		}
		protected void DrawEmptyDocument(GraphicsCache bufferedCache, Rectangle bounds) {
			bufferedCache.FillRectangle(GetDockZoneColor(), bounds);
		}
		protected virtual Color GetDockZoneColor() {
			return Color.FromArgb(0xff, 0xA5, 0xC2, 0xE4);
		}
	}
	public class WidgetViewSkinPainter : WidgetViewPainter {
		Skin skin;
		public WidgetViewSkinPainter(WidgetView view)
			: base(view) {
			skin = DockingSkins.GetSkin(View.ElementsLookAndFeel);
		}
		protected override void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			SkinElement element = skin[DockingSkins.SkinNativeMdiViewBackground];
			if(element != null)
				ObjectPainter.DrawObject(cache, SkinElementPainter.Default, new SkinElementInfo(element, bounds));
			else base.DrawBackgroundCore(cache, bounds);
		}
		protected internal override ObjectPainter GetDocumentSelectorHeaderPainter() {
			return new Customization.DocumentSelectorHeaderSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorFooterPainter() {
			return new Customization.DocumentSelectorFooterSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorItemsListPainter() {
			return new Customization.DocumentSelectorItemsListSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorPreviewPainter() {
			return new Customization.DocumentSelectorPreviewSkinPainter(View.ElementsLookAndFeel);
		}
		protected internal override ObjectPainter GetDocumentSelectorBackgroundPainter() {
			return new Customization.DocumentSelectorBackgroundSkinPainter(View.ElementsLookAndFeel);
		}
		protected override Color GetDockZoneColor() {
			return skin.Colors.GetColor("DockZoneBackColor", base.GetDockZoneColor());
		}
		protected override AppearanceDefault CreateDefaultAppearanceCaption() {
			Color foreColor = LookAndFeelHelper.GetSystemColor(View.ElementsLookAndFeel, SystemColors.ControlText);
			Color backColor = LookAndFeelHelper.GetSystemColor(View.ElementsLookAndFeel, SystemColors.Control);
			SkinElement element = skin[DockingSkins.SkinNativeMdiViewBackground];
			if(element != null)
				foreColor = element.Color.GetForeColor();
			return new AppearanceDefault(foreColor, backColor, SegoeUIFontsCache.GetFont("Segoe UI", 12f));
		}
	}
}
