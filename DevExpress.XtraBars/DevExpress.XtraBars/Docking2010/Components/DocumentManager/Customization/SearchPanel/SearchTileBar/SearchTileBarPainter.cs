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

using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System.Drawing;
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SearchTileBarPainter : TileBarPainter {
		protected override void DrawBackground(TileControlInfoArgs e) {
			e.ViewInfo.PaintAppearance.DrawBackground(e.Cache, e.ViewInfo.Bounds);
		}
		protected override void DrawItemSelection(TileControlInfoArgs e, TileItemViewInfo itemInfo) { }
		protected override void DrawItem(TileControlInfoArgs e, TileItemViewInfo itemInfo) {
			base.DrawItem(e, itemInfo);
			if(itemInfo is SearchTileBarExpandViewInfo)
				DrawItemExpandButton(e, (SearchTileBarExpandViewInfo)itemInfo);
		}
		protected virtual void DrawItemExpandButton(TileControlInfoArgs e, SearchTileBarExpandViewInfo itemInfo) {
			using(SolidBrush brush = new SolidBrush(itemInfo.PaintAppearance.ForeColor)) {
				using(Pen pen = new Pen(brush, 2)) {
					e.Cache.Graphics.DrawLines(pen, itemInfo.LeftLinePoints);
					e.Cache.Graphics.DrawLines(pen, itemInfo.RightLinePoints);
				}
			}
		}
		protected override  void DrawElementImageColorized(TileControlInfoArgs e, TileItemElementViewInfo elemInfo, Rectangle bounds) {
			Color color = elemInfo.PaintAppearance.ForeColor;
			System.Drawing.Imaging.ImageAttributes attr = DevExpress.Utils.Drawing.ImageColorizer.GetColoredAttributes(color, color.A);
			DrawImageColorizedCore(e, elemInfo.Image, bounds, attr);			
		}
	}
}
