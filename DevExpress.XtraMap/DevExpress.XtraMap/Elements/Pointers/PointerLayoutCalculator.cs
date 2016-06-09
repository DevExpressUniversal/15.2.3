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
using System.Windows.Forms;
using DevExpress.XtraMap.Native;
namespace DevExpress.XtraMap.Drawing {
	public struct PointerLayoutResult {
		public Rectangle ImageRect { get; set; }
		public Rectangle TextRect { get; set; }
		public Rectangle Rect { get; set; }
	}
	public class PointerLayoutCalculator {
		int textPadding = MapPointer.DefaultTextPadding;
		int TextPadding {
			get { return textPadding; }
			set {
				if(textPadding == value)
					return;
				textPadding = value < 0 ? MapPointer.DefaultTextPadding : value;
			}
		}
		PointerLayoutResult CalculateTopLeft(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int gap = (imageRect.Height > 0 && textRect.Height > 0) ? TextPadding : 0;
			int height = imageRect.Height + textRect.Height + gap;
			int width = imageRect.Width + gap + textRect.Width;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			result.TextRect = new Rectangle(padding.Left, padding.Top, textRect.Width, textRect.Height);
			result.ImageRect = new Rectangle(result.TextRect.Right + gap, result.TextRect.Bottom + gap, imageRect.Width, imageRect.Height);
			return result;
		}
		PointerLayoutResult CalculateTopRight(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int gap = (imageRect.Height > 0 && textRect.Height > 0) ? TextPadding : 0;
			int height = imageRect.Height + textRect.Height + gap;
			int width = imageRect.Width + gap + textRect.Width;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.TopRight);
			textRect.Offset(-padding.Right, padding.Top);
			result.TextRect = textRect;
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.BottomLeft);
			imageRect.Offset(padding.Left, -padding.Bottom);
			result.ImageRect = imageRect;
			return result;
		}
		PointerLayoutResult CalculateTopCenter(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int gap = (imageRect.Height > 0 && textRect.Height > 0) ? TextPadding : 0;
			int height = imageRect.Height + textRect.Height + gap;
			int width = Math.Max(imageRect.Width, textRect.Width);
			bool imageWider = imageRect.Width > textRect.Width;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.TopLeft);
			textRect.X = imageWider ? padding.Left - textRect.Width / 2 + imageRect.Width / 2 : padding.Left;
			textRect.Y = padding.Top;
			result.TextRect = textRect;
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.BottomLeft);
			imageRect.X = imageWider ? padding.Left : padding.Left + textRect.Width / 2 - imageRect.Width / 2;
			imageRect.Y -= padding.Bottom;
			result.ImageRect = imageRect;
			return result;
		}
		PointerLayoutResult CalculateBottomLeft(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int gap = (imageRect.Height > 0 && textRect.Height > 0) ? TextPadding : 0;
			int height = imageRect.Height + textRect.Height + gap;
			int width = imageRect.Width + gap + textRect.Width;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.BottomLeft);
			textRect.Offset(padding.Left, -padding.Bottom);
			result.TextRect = textRect;
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.TopRight);
			imageRect.Offset(-padding.Right, padding.Top);
			result.ImageRect = imageRect;
			return result;
		}
		PointerLayoutResult CalculateBottomCenter(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int gap = (imageRect.Height > 0 && textRect.Height > 0) ? TextPadding : 0;
			int height = imageRect.Height + textRect.Height + gap;
			int width = Math.Max(imageRect.Width, textRect.Width);
			bool imageWider = imageRect.Width > textRect.Width;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.TopLeft);
			imageRect.X = imageWider ? padding.Left : padding.Left + textRect.Width / 2 - imageRect.Width / 2;
			imageRect.Y = padding.Top;
			result.ImageRect = imageRect;
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.BottomLeft);
			textRect.X = imageWider ? padding.Left - textRect.Width / 2 + imageRect.Width / 2 : padding.Left;
			textRect.Y -= padding.Bottom;
			result.TextRect = textRect;
			return result;
		}
		PointerLayoutResult CalculateBottomRight(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int gap = (imageRect.Height > 0 && textRect.Height > 0) ? TextPadding : 0;
			int height = imageRect.Height + textRect.Height + gap;
			int width = imageRect.Width + gap + textRect.Width;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.BottomRight);
			textRect.Offset(-padding.Right, -padding.Bottom);
			result.TextRect = textRect;
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.TopLeft);
			imageRect.Offset(padding.Left, padding.Top);
			result.ImageRect = imageRect;
			return result;
		}
		PointerLayoutResult CalculateMiddleLeft(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int height = Math.Max(imageRect.Height, textRect.Height);
			int gap = (imageRect.Width > 0 && textRect.Width > 0) ? TextPadding : 0;
			int width = imageRect.Width + gap + textRect.Width;
			bool imageHigher = imageRect.Height > textRect.Height;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.TopRight);
			imageRect.X -= padding.Right;
			imageRect.Y = imageHigher ? padding.Top : padding.Top + textRect.Height / 2 + imageRect.Height / 2;
			result.ImageRect = imageRect;
			textRect = new Rectangle(0, 0, textRect.Width, textRect.Height);
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.TopLeft);
			textRect.X = padding.Left;
			textRect.Y = imageHigher ? imageRect.Top - textRect.Height / 2 + imageRect.Height / 2 : padding.Top;
			result.TextRect = textRect;
			return result;
		}
		PointerLayoutResult CalculateMiddleRight(Rectangle imageRect, Rectangle textRect, Padding padding) {
			PointerLayoutResult result = new PointerLayoutResult();
			int height = Math.Max(imageRect.Height, textRect.Height);
			int gap = (imageRect.Width > 0 && textRect.Width > 0) ? TextPadding : 0;
			int width = imageRect.Width + gap + textRect.Width;
			bool imageHigher = imageRect.Height > textRect.Height;
			result.Rect = new Rectangle(0, 0, width + padding.Horizontal, height + padding.Vertical);
			imageRect = RectUtils.AlignRectangle(imageRect, result.Rect, ContentAlignment.TopLeft);
			imageRect.X = padding.Left;
			imageRect.Y = imageHigher ? padding.Top : padding.Top + (textRect.Height - imageRect.Height) / 2;
			result.ImageRect = imageRect;
			textRect = new Rectangle(0, 0, textRect.Width, textRect.Height);
			textRect = RectUtils.AlignRectangle(textRect, result.Rect, ContentAlignment.TopRight);
			textRect.X -= padding.Right;
			textRect.Y = imageHigher ? imageRect.Top - textRect.Height / 2 + imageRect.Height / 2 : padding.Top;
			result.TextRect = textRect;
			return result;
		}
		public PointerLayoutResult Calculate(Rectangle imageRect, Rectangle textRect, Padding padding, int textPadding, TextAlignment textAlignment) {
			TextPadding = textPadding;
			switch(textAlignment) {
				case TextAlignment.TopLeft:
					return CalculateTopLeft(imageRect, textRect, padding);
				case TextAlignment.TopCenter:
					return CalculateTopCenter(imageRect, textRect, padding);
				case TextAlignment.TopRight:
					return CalculateTopRight(imageRect, textRect, padding);
				case TextAlignment.MiddleLeft:
					return CalculateMiddleLeft(imageRect, textRect, padding);
				case TextAlignment.MiddleRight:
					return CalculateMiddleRight(imageRect, textRect, padding);
				case TextAlignment.BottomLeft:
					return CalculateBottomLeft(imageRect, textRect, padding);
				case TextAlignment.BottomCenter:
					return CalculateBottomCenter(imageRect, textRect, padding);
				case TextAlignment.BottomRight:
					return CalculateBottomRight(imageRect, textRect, padding);
			}
			return new PointerLayoutResult();
		}
	}
}
