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
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class BrickContainerExporter : BrickContainerBaseExporter {
		static RectangleF InflateRectByBorderWidth(RectangleF clipRect, RectangleF brickRect, BrickStyle style) {
			float borderWidth = GetOuterBorderWidth(style);
			if(borderWidth == 0f || style.Sides == BorderSide.None)
				return clipRect;
			if(clipRect.X == brickRect.X && (style.Sides & BorderSide.Left) > 0) {
				clipRect.X -= borderWidth;
				clipRect.Width += borderWidth;
			}
			if(clipRect.Y == brickRect.Y && (style.Sides & BorderSide.Top) > 0) {
				clipRect.Y -= borderWidth;
				clipRect.Height += borderWidth;
			}
			if(clipRect.Right == brickRect.Right && (style.Sides & BorderSide.Right) > 0)
				clipRect.Width += borderWidth;
			if(clipRect.Bottom == brickRect.Bottom && (style.Sides & BorderSide.Bottom) > 0)
				clipRect.Height += borderWidth;
			return clipRect;
		}
		static float GetOuterBorderWidth(BrickStyle style) {
			float borderWidth = GraphicsUnitConverter.PixelToDoc(style.BorderWidth);
			if(style.BorderStyle == BrickBorderStyle.Center)
				return borderWidth / 2;
			else if(style.BorderStyle == BrickBorderStyle.Outset)
				return borderWidth;
			return 0f;
		}
		BrickContainer BrickContainer { get { return Brick as BrickContainer; } }
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			RectangleF oldClip = gr.ClipBounds;
			try {
				RectangleF clipBounds = rect;
				VisualBrick visualBrick = BrickContainer.Brick as VisualBrick;
				if(visualBrick != null)
					clipBounds = InflateRectByBorderWidth(rect, BrickContainer.GetBrickRect(rect.Location), visualBrick.Style);
				clipBounds = RectangleF.Intersect(gr.ClipBounds, clipBounds);
				gr.ClipBounds = clipBounds;
				base.Draw(gr, rect, parentRect);
			} finally {
				gr.ClipBounds = oldClip;
			}
		}
		internal override void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
			RectangleF brickBounds = BrickContainer.GetViewRectangle();
			brickBounds.Location = BrickContainer.AdjustLocation(brickBounds.Location);
			brickBounds.Height = BrickContainer.Brick.Height;
			brickBounds.Width = BrickContainer.Brick.Width;
			brickBounds = layoutBuilder.ValidateLayoutRect(BrickContainer.GetRealBrick(), brickBounds);
			brickBounds.Offset(pos);
			RectangleF brickClipRect = layoutBuilder.ValidateLayoutRect(BrickContainer.GetRealBrick(), BrickContainer.GetViewRectangle());
			brickClipRect.Offset(pos);
			ProcessLayoutCore(layoutBuilder, brickBounds, RectangleF.Intersect(clipRect, brickClipRect));
		}
	}
}
