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
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows;
using DevExpress.DocumentView;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Printing.PreviewControl.Native;
using DevExpress.Xpf.Printing.PreviewControl.Native.Models;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Enumerators;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing.PreviewControl.Rendering {
	public interface INativeRendererImpl : IDisposable {
		bool RenderToGraphics(Graphics graphics, RenderedContent renderedContent, double zoomFactor, double angle);
	}
	public class DirectNativeRendererImpl : INativeRendererImpl {
		public bool RenderToGraphics(Graphics graphics, RenderedContent renderedContent, double zoomFactor, double angle) {
			foreach(var pageModel in renderedContent.RenderedPages) {
				PageViewModel page = pageModel.Page;
				if(page.Page.Document == null)
					return true;
				Rect rect = pageModel.Rectangle;
				var location = new PointF((float)(rect.Location.X * ScreenHelper.ScaleX / zoomFactor), (float)(rect.Location.Y * ScreenHelper.ScaleX / zoomFactor) );
				graphics.ResetTransform();
				graphics.SmoothingMode = zoomFactor < 1.0 ? SmoothingMode.HighQuality : SmoothingMode.Default;
				graphics.ScaleTransform((float)(zoomFactor / ScreenHelper.ScaleX), (float)(zoomFactor / ScreenHelper.ScaleX));
				((DevExpress.DocumentView.IPage)page.Page).Draw(graphics, location);
				renderedContent.SelectionService.Do(x=> x.OnDrawPage(page.Page, graphics, PSUnitConverter.PixelToDoc(location)));
				DrawBookmarks(page.Page, graphics, PSUnitConverter.PixelToDoc(location));
			}
			return true;
		}
		protected void DrawBookmarks(Page page, Graphics gr, PointF position) {
			var printingSystem = page.Document.PrintingSystem;
			if(page.PageSize.IsEmpty)
				return;
			List<Brick> markedBricks = new List<Brick>(printingSystem.GetMarkedBricks(page));
			Func<BrickBase, PointF, IIndexedEnumerator> method = (item, itemPosition) => {
				if(item.InnerBrickList.Count > 5 * BrickMapConst.Graduation && item is CompositeBrick) {
					((CompositeBrick)item).ForceBrickMap();
					PointF viewOrigin = new PointF(itemPosition.X + item.InnerBrickListOffset.X, itemPosition.Y + item.InnerBrickListOffset.Y);
					return new MappedIndexedEnumerator(((CompositeBrick)item).BrickMap, item.InnerBrickList) { ClipBounds = gr.ClipBounds, ViewOrigin = viewOrigin };
				}
				return new IndexedEnumerator(item.InnerBrickList);
			};
			new BrickNavigator(page, method) { BrickPosition = position }.IterateBricks((brick, brickRect, brickClipRect) => {
				if(markedBricks.Remove(brick)) {
					DrawBookmark(gr, brickRect);
				}
				return markedBricks.Count == 0;
			});
		}
		Pen markPen;
		Pen MarkPen {
			get {
				if(markPen == null)
					markPen = CreateMarkPen();
				return markPen;
			}
		}
		static Pen CreateMarkPen() {
			using(Bitmap image = ResourceImageHelper.CreateBitmapFromResources("Core.Images.MarkBrush.bmp", typeof(DevExpress.Printing.ResFinder))) {
				image.MakeTransparent(DXColor.Magenta);
				using(Brush brush = new TextureBrush(image)) {
					return new Pen(brush, System.Math.Max(image.Width, image.Height));
				}
			}
		}
		void DrawBookmark(Graphics gr, RectangleF rect) {
			float inflateValue = PSUnitConverter.PixelToDoc(3);
			rect = RectangleF.Inflate(rect, inflateValue, inflateValue);
			gr.DrawRectangle(MarkPen, Rectangle.Round(rect));
		}
		public void Dispose() {
			MarkPen.Dispose();
		}
	}
}
