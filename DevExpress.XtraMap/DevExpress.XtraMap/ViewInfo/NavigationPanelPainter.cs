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
using System.Drawing;
using DevExpress.Utils.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public class NavigationPanelPainter : OverlayViewInfoPainter {
		public NavigationPanelPainter(IViewInfoStyleProvider viewInfoStyleProvider)
			: base(viewInfoStyleProvider) {
		}
		public override void Draw(GraphicsCache cache, SelfpaintingViewinfo viewInfo) {
			NavigationPanelViewInfo vi = (NavigationPanelViewInfo)viewInfo;
			NavigationPanelAppearance paintAppearance = (NavigationPanelAppearance)(viewInfo.PaintAppearance);
			BackgroundStyle bgStyle = paintAppearance.BackgroundStyle;
			BackgroundStyle itemStyle = paintAppearance.ItemStyle;
			DrawBackground(cache, vi, bgStyle);
			if(vi.ShowCoordinates) {
				TextElementStyle coordinatesStyle = paintAppearance.CoordinatesStyle;
				DrawCoordinates(cache, vi, coordinatesStyle);
			}
			if(vi.ShowZoomElements) {
				BackgroundStyle zoomTrackBarThumbstyle = GetInteractiveElementStyle(vi.ZoomTrackBarViewInfo, vi);
				DrawZoomTrackBar(cache, vi, itemStyle, zoomTrackBarThumbstyle);
				BackgroundStyle zoomInButtonStyle = GetInteractiveElementStyle(vi.ZoomInBtnViewInfo, vi);
				DrawZoomButton(cache, vi.ZoomInBtnViewInfo, vi.ZoomInBtnViewInfo.ClientBounds, zoomInButtonStyle);
				BackgroundStyle zoomOutButtonStyle = GetInteractiveElementStyle(vi.ZoomOutBtnViewInfo, vi);
				DrawZoomButton(cache, vi.ZoomOutBtnViewInfo, vi.ZoomOutBtnViewInfo.ClientBounds, zoomOutButtonStyle);
			}
			if(vi.ShowScrollButtons) {
				BackgroundStyle scrollButtonsStyle = GetInteractiveElementStyle(vi.ScrollButtonsViewInfo, vi);
				DrawScrollButtonsBackground(cache, vi, scrollButtonsStyle);
				DrawScrollButtonsArrows(cache, vi, scrollButtonsStyle);
			}
			if(vi.ShowScale) {
				TextElementStyle scaleInfoStyle = paintAppearance.ScaleStyle;
				DrawScaleInfo(cache, vi, itemStyle, scaleInfoStyle);
			}
		}
		void DrawScaleInfo(GraphicsCache cache, NavigationPanelViewInfo viewInfo, BackgroundStyle itemStyle, TextElementStyle scaleInfoStyle) {
			cache.FillRectangle(itemStyle.Fill, viewInfo.ScaleViewInfo.LineRect);
			if(viewInfo.ScaleViewInfo.ViewType == ScaleViewType.Meters)
				DrawMetersScaleInfo(cache, viewInfo.ScaleViewInfo, itemStyle, scaleInfoStyle);
			else if(viewInfo.ScaleViewInfo.ViewType == ScaleViewType.Miles)
				DrawMilesScaleInfo(cache, viewInfo.ScaleViewInfo, itemStyle, scaleInfoStyle);
			else {
				DrawMetersScaleInfo(cache, viewInfo.ScaleViewInfo, itemStyle, scaleInfoStyle);
				DrawMilesScaleInfo(cache, viewInfo.ScaleViewInfo, itemStyle, scaleInfoStyle);
			}
		}
		private void DrawMilesScaleInfo(GraphicsCache cache, ScaleViewInfo scaleViewInfo, BackgroundStyle itemStyle, TextElementStyle scaleInfoStyle) {
			cache.FillRectangle(itemStyle.Fill, scaleViewInfo.MilesLeftLineRect);
			cache.FillRectangle(itemStyle.Fill, scaleViewInfo.MilesRightLineRect);
			Brush brush = cache.GetSolidBrush(scaleInfoStyle.TextColor);
			MapUtils.TextPainter.DrawString(cache, scaleViewInfo.MilesText, scaleInfoStyle.Font, brush, scaleViewInfo.MilesTextRect, StringFormat.GenericTypographic);
		}
		private void DrawMetersScaleInfo(GraphicsCache cache, ScaleViewInfo scaleViewInfo, BackgroundStyle itemStyle, TextElementStyle scaleInfoStyle) {
			cache.FillRectangle(itemStyle.Fill, scaleViewInfo.MetersLeftLineRect);
			cache.FillRectangle(itemStyle.Fill, scaleViewInfo.MetersRightLineRect);
			Brush brush = cache.GetSolidBrush(scaleInfoStyle.TextColor);
			MapUtils.TextPainter.DrawString(cache, scaleViewInfo.MetersText, scaleInfoStyle.Font, brush, scaleViewInfo.MetersTextRect, StringFormat.GenericTypographic);
		}
		void DrawZoomButton(GraphicsCache cache, ZoomButtonViewInfoBase viewInfo, Rectangle bounds, BackgroundStyle buttonStyle) {
			using(GraphicsSmoothHelper helper = new GraphicsSmoothHelper(cache, System.Drawing.Drawing2D.SmoothingMode.AntiAlias)) {
				Pen pen = cache.GetPen(buttonStyle.Fill, (int)(ZoomButtonViewInfoBase.LineWidth * 0.75f));
				Brush brush = cache.GetSolidBrush(buttonStyle.Fill);
				cache.Graphics.DrawEllipse(pen, bounds);
				foreach(RectangleF rect in viewInfo.ButtonSymbolRects) {
					cache.Graphics.FillRectangle(brush, rect);
				}
			}
		}
		BackgroundStyle GetInteractiveElementStyle(NonTextElementViewInfoBase viewInfo, NavigationPanelViewInfo navigationPanelViewInfo) {
			NavigationPanelAppearance panelAppearance = (NavigationPanelAppearance)(navigationPanelViewInfo.PaintAppearance);
			if(viewInfo == null)
				return panelAppearance.ItemStyle;
			if(viewInfo.Pressed)
				return panelAppearance.PressedItemStyle;
			return viewInfo.HotTracked ? panelAppearance.HotTrackedItemStyle : panelAppearance.ItemStyle;
		}
		void DrawBackground(GraphicsCache cache, NavigationPanelViewInfo viewInfo, BackgroundStyle bgStyle) {
			cache.FillRectangle(bgStyle.Fill, viewInfo.ClientBounds);
		}
		void DrawCoordinates(GraphicsCache cache, NavigationPanelViewInfo viewInfo, TextElementStyle coordinatesStyle) {
			CoordinateViewInfo longitudeViewInfo = viewInfo.XCoordinateViewInfo;
			CoordinateViewInfo latitudeViewInfo = viewInfo.YCoordinateViewInfo;
			Brush brush = cache.GetSolidBrush(coordinatesStyle.TextColor);
			if(longitudeViewInfo.ClientBounds != Rectangle.Empty)
				MapUtils.TextPainter.DrawString(cache, longitudeViewInfo.Text, coordinatesStyle.Font, brush, longitudeViewInfo.ClientBounds, StringFormat.GenericTypographic);
			if(latitudeViewInfo.ClientBounds != Rectangle.Empty)
				MapUtils.TextPainter.DrawString(cache, latitudeViewInfo.Text, coordinatesStyle.Font, brush, latitudeViewInfo.ClientBounds, StringFormat.GenericTypographic);
		}
		void DrawZoomTrackBar(GraphicsCache cache, NavigationPanelViewInfo viewInfo, BackgroundStyle lineStyle, BackgroundStyle thumbStyle) {
			cache.FillRectangle(lineStyle.Fill, viewInfo.ZoomTrackBarViewInfo.LineBoundsLeft);
			cache.FillRectangle(lineStyle.Fill, viewInfo.ZoomTrackBarViewInfo.LineBoundsRight);
			cache.FillRectangle(thumbStyle.Fill, viewInfo.ZoomTrackBarViewInfo.ThumbBounds);
		}
		void DrawScrollButtonsBackground(GraphicsCache cache, NavigationPanelViewInfo viewInfo, BackgroundStyle itemStyle) {
			using(GraphicsSmoothHelper helper = new GraphicsSmoothHelper(cache, System.Drawing.Drawing2D.SmoothingMode.AntiAlias)) {
				Pen pen = cache.GetPen(itemStyle.Fill, ScrollButtonsViewInfo.BackgroundLineThickness);
				cache.Graphics.DrawEllipse(pen, viewInfo.ScrollButtonsViewInfo.ClientBounds);
			}
		}
		void DrawScrollButtonsArrows(GraphicsCache cache, NavigationPanelViewInfo viewInfo, BackgroundStyle itemStyle) {
			Brush brush = cache.GetSolidBrush(itemStyle.Fill);
			ScrollButtonsViewInfo scrollButtonsViewInfo = viewInfo.ScrollButtonsViewInfo;
			cache.Graphics.FillPolygon(brush, scrollButtonsViewInfo.LeftArrowPolygonPoints);
			cache.Graphics.FillPolygon(brush, scrollButtonsViewInfo.TopArrowPolygonPoints);
			cache.Graphics.FillPolygon(brush, scrollButtonsViewInfo.RightArrowPolygonPoints);
			cache.Graphics.FillPolygon(brush, scrollButtonsViewInfo.BottomArrowPolygonPoints);
		}
	}
}
