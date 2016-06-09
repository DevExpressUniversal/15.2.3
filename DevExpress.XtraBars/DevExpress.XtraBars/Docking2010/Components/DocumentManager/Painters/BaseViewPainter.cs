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

using System.Drawing;
using DevExpress.Utils.Drawing;
namespace DevExpress.XtraBars.Docking2010.Views {
	public abstract class BaseViewPainter {
		BaseView viewCore;
		public BaseView View {
			get { return viewCore; }
		}
		public BaseViewPainter(BaseView view) {
			viewCore = view;
		}
		protected internal virtual int GetRootMargin() {
			return 0;
		}
		string OverlapAllControlsWarning { get { return DocumentManagerLocalizer.GetString(DocumentManagerStringId.OverlapAllControlsWarning); } }
		public void Draw(GraphicsCache cache, Rectangle clip) {
			if(clip.Width <= 0 || clip.Height <= 0) return;
			using(XtraBufferedGraphics bg = XtraBufferedGraphicsManager.Current.Allocate(cache.Graphics, clip)) {
				using(GraphicsCache bufferedCache = new GraphicsCache(bg.Graphics)) {
					DrawCore(bufferedCache, clip);
					if(View.CanShowOverlapWarning())
						DrawWarning(bufferedCache, clip);
				}
				bg.Render();
			}
		}
		int cachedWarningWidth = 0;
		protected virtual void DrawWarning(GraphicsCache bufferedCache, Rectangle clip) {
			int threeLineHeight = 0;
			GetOptimalWidth(bufferedCache, out threeLineHeight);
			Size textSize = Size.Round(WarningAppearance.CalcTextSize(bufferedCache, OverlapAllControlsWarning, cachedWarningWidth));
			using(Font font = GetFont(bufferedCache, clip, textSize)) {
				bufferedCache.DrawString(OverlapAllControlsWarning, font, WarningAppearance.GetForeBrush(bufferedCache), GetWarningBounds(clip, textSize, threeLineHeight), WarningAppearance.GetStringFormat());
			}
		}
		Font GetFont(GraphicsCache bufferedCache, Rectangle clip, Size textSize) {
			var fontFamily = DevExpress.Utils.AppearanceObject.DefaultFont.FontFamily;
			float fontSize = GetFontSize(bufferedCache, clip.Size, textSize, fontFamily);
			return new System.Drawing.Font(fontFamily, fontSize);
		}
		void GetOptimalWidth(GraphicsCache bufferedCache, out int threeLineHeight) {
			Size oneLineSize = Size.Round(WarningAppearance.CalcTextSize(bufferedCache, OverlapAllControlsWarning, 0));
			threeLineHeight = oneLineSize.Height * 3 + 10;
			int optimalWidth = 0;
			if(cachedWarningWidth == 0) {
				int newHeight = 0;
				while(threeLineHeight - 10 != newHeight) {
					newHeight = Size.Round(WarningAppearance.CalcTextSize(bufferedCache, OverlapAllControlsWarning, optimalWidth)).Height;
					cachedWarningWidth = optimalWidth;
					optimalWidth += 100;
				}
			}
		}
		float GetFontSize(GraphicsCache g, Size clipSize, Size textSize, FontFamily fontFamily) {
			float fontSize = 14;
			while((clipSize.Width < textSize.Width || clipSize.Height < textSize.Height + 10) && fontSize > 10) {
				fontSize--;
				using(Font font = new Font(fontFamily, fontSize)) {
					textSize = Size.Round(g.CalcTextSize(OverlapAllControlsWarning, font, WarningAppearance.GetStringFormat(), textSize.Width));
				}
			}
			return fontSize < 10 ? 10 : fontSize;
		}
		Rectangle GetWarningBounds(Rectangle clip, Size textSize, int threeLineHeight) {
			int padding = 16;
			if(cachedWarningWidth != 0)
				textSize.Width = clip.Width > cachedWarningWidth - padding ? cachedWarningWidth - padding : clip.Width - padding;
			else
				textSize.Width = clip.Width - padding;
			textSize.Width = clip.Height < threeLineHeight ? clip.Width - padding : textSize.Width;
			textSize.Height = clip.Height;
			return DevExpress.Utils.PlacementHelper.Arrange(textSize, clip, ContentAlignment.MiddleCenter);
		}
		DevExpress.Utils.AppearanceObject warningAppearance;
		DevExpress.Utils.AppearanceObject WarningAppearance {
			get {
				if(warningAppearance == null) {
					warningAppearance = new DevExpress.Utils.AppearanceObject();
					warningAppearance.FontSizeDelta = 5;
					warningAppearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
					warningAppearance.TextOptions.Trimming = DevExpress.Utils.Trimming.EllipsisCharacter;
					warningAppearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
				}
				return warningAppearance;
			}
		}
		protected abstract void DrawCore(GraphicsCache bufferedCache, Rectangle clip);
		protected void DrawBackground(GraphicsCache cache, Rectangle bounds) {
			DrawBackgroundCore(cache, bounds);
			View.RaiseCustomDrawBackground(cache, bounds);
		}
		protected virtual void DrawBackgroundCore(GraphicsCache cache, Rectangle bounds) {
			Image backgroundImage = View.ParentBackgroundImage;
			if(backgroundImage != null) {
				BackgroundImagePainter.DrawBackgroundImage(cache.Graphics, backgroundImage, View.ParentBackColor,
					View.ParentBackgroundImageLayout, bounds, bounds, Point.Empty, System.Windows.Forms.RightToLeft.No);
			}
			else {
				if(View.ViewInfo.PaintAppearance.BackColor == Color.Transparent) {
					var paintArgs = new System.Windows.Forms.PaintEventArgs(cache.Graphics, bounds);
					var container = View.Manager.GetContainer();
					BackgroundPaintHelper.PaintTransparentBackground(container, paintArgs, bounds);
				}
				else View.ViewInfo.PaintAppearance.FillRectangle(cache, bounds);
			}
			DrawBackgroundImage(cache.Graphics, View.BackgroundImage, bounds);
		}
		protected virtual void DrawBackgroundImage(Graphics g, Image img, Rectangle bounds) {
			if(img == null) return;
			if(!View.BackgroundImageStretchMargins.HasValue)
				g.DrawImage(img, ImageLayoutHelper.GetImageBounds(bounds, img.Size, View.BackgroundImageLayoutMode));
			else DevExpress.Utils.Helpers.PaintHelper.DrawImage(g, img, bounds, View.BackgroundImageStretchMargins.Value);
		}
		protected internal virtual Color GetBackColor(Color parentBackColor) {
			return parentBackColor;
		}
		protected internal virtual ObjectPainter GetDocumentSelectorHeaderPainter() {
			return new Customization.DocumentSelectorHeaderPainter();
		}
		protected internal virtual ObjectPainter GetDocumentSelectorFooterPainter() {
			return new Customization.DocumentSelectorFooterPainter();
		}
		protected internal virtual ObjectPainter GetDocumentSelectorItemsListPainter() {
			return new Customization.DocumentSelectorItemsListPainter();
		}
		protected internal virtual ObjectPainter GetDocumentSelectorPreviewPainter() {
			return new Customization.DocumentSelectorPreviewPainter();
		}
		protected internal virtual ObjectPainter GetDocumentSelectorBackgroundPainter() {
			return new Customization.DocumentSelectorBackgroundPainter();
		}
		protected internal virtual DevExpress.Utils.AppearanceObject GetDocumentSelectorHeaderAppearance() {
			return DevExpress.Utils.AppearanceObject.EmptyAppearance;
		}
		protected internal virtual DevExpress.Utils.AppearanceObject GetDocumentSelectorFooterAppearance() {
			return DevExpress.Utils.AppearanceObject.EmptyAppearance;
		}
		public virtual ObjectPainter GetWaitScreenPainter() {
			return new Customization.WaitScreenPainter();
		}
	}
}
