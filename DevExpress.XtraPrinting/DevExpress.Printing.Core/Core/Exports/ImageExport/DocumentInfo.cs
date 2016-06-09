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
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export.Imaging {
	internal class DocumentInfo {
		int brickCount;
		float leftOfBricks;
		float rightOfBricks;
		float bottomOfBricks;
		float pageWidth;
		float pagesHeight;
		float borderWidth;
		public float PageWidth { get { return pageWidth; } }
		public float PagesHeight { get { return pagesHeight; } }
		public int BrickCount { get { return brickCount; } }
		public float LeftOfBricks { get { return leftOfBricks == float.MaxValue ? 0 : leftOfBricks; } }
		public float RightOfBricks { get { return rightOfBricks; } }
		public float BottomOfBricks { get { return bottomOfBricks; } }
		public float BorderWidth { get { return borderWidth; } }
		public DocumentInfo() {
			Clear();
		}
		void Clear() {
			brickCount = 0;
			leftOfBricks = float.MaxValue;
			rightOfBricks = 0;
			bottomOfBricks = 0;
			pageWidth = 0;
			pagesHeight = 0;
		}
		public void Update(List<BrickWithOffset> bricks) {
			Clear();
			foreach(BrickWithOffset brick in bricks) {
				UpdateContent(brick);
			}
		}
		public void Update(PageList pages, ImageExportOptions options) {
			borderWidth = options.PageBorderWidth != 0 ? Math.Max(1f, GraphicsUnitConverter.Convert(options.PageBorderWidth, options.Resolution, GraphicsDpi.Document)): 0;
			foreach(int index in ExportOptionsHelper.GetPageIndices(options, pages.Count)) {
				pageWidth = Math.Max(pages[index].PageData.PageSize.Width + 2 * borderWidth, pageWidth);
				pagesHeight += pages[index].PageData.PageSize.Height + borderWidth;
			}
			pagesHeight += borderWidth;
		}
		void UpdateContent(BrickWithOffset brickInfo) {
			RectangleF rect = brickInfo.Rect;
			if(brickInfo.Brick is VisualBrick) { 
				BrickStyle style = ((VisualBrick)brickInfo.Brick).Style;
				BorderSide sides = (BorderSide.Right | BorderSide.Bottom) & style.Sides; 
				rect = style.InflateBorderWidth(rect, GraphicsDpi.Document, true, sides);
			}
			leftOfBricks = Math.Min(leftOfBricks, rect.Left);
			rightOfBricks = Math.Max(rightOfBricks, rect.Right);
			bottomOfBricks = Math.Max(bottomOfBricks, rect.Bottom);
		}
	}
}
