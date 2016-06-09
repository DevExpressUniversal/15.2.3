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
using System.IO;
using System.Drawing;
using DevExpress.Skins;
using DevExpress.Utils;
using System.Reflection;
using DevExpress.LookAndFeel;
using DevExpress.Office.Utils;
using DevExpress.Office.Layout;
using DevExpress.Utils.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.Office.Drawing;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
namespace DevExpress.XtraSpreadsheet.Drawing {
	public class PageSpreadsheetSelectionSkinPainter : PageSpreadsheetSelectionPainter {
		#region Fields
		UserLookAndFeel lookAndFeel;
		#endregion
		public PageSpreadsheetSelectionSkinPainter(GraphicsCache cache, UserLookAndFeel lookAndFeel)
			: base(cache) {
			this.lookAndFeel = lookAndFeel;
		}
		protected internal override Color GetSelectionColor() {
			return CommonSkins.GetSkin(lookAndFeel)[CommonSkins.SkinSelection].Color.GetBackColor();
		}
		protected internal override Color GetSelectionBorderColor() {
			Color color = SpreadsheetSkins.GetSkin(lookAndFeel).Colors.GetColor(SpreadsheetSkins.ColorSelectionBorder, Color.Empty);
			if (color != Color.Empty)
				return color;
			return base.GetSelectionBorderColor();
		}
		protected internal override int GetSelectionBorderWidth() {
			int width = SpreadsheetSkins.GetSkin(lookAndFeel).Properties.GetInteger(SpreadsheetSkins.OptSelectionBorderWidth, Int32.MinValue);
			if (width != Int32.MinValue)
				return width;
			return base.GetSelectionBorderWidth();
		}
		protected override void DrawColumnAutoFilterBackground(Rectangle bounds) {
			SkinPaintHelper.DrawSkinElement(lookAndFeel, Cache, GridSkins.SkinHeaderSpecial, bounds, 0);
		}
		protected override Image ObtainActualAutoFilterGlyph(Image image) {
			return SkinImageDecorator.CreateImageColoredToButtonForeColor(image, lookAndFeel);
		}
		protected override void DrawPivotTableExpandCollapseHotZone(Rectangle bounds, bool isCollapsed) {
			int imageIndex = isCollapsed ? 0 : 1;
			SkinPaintHelper.DrawSkinElement(lookAndFeel, Cache, GridSkins.SkinPlusMinusEx, bounds, imageIndex);
		}
	}
}
