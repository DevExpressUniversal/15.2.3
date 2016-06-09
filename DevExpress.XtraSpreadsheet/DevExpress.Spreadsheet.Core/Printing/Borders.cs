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
using DevExpress.Utils;
using DevExpress.Office.Printing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Layout;
using LayoutPage = DevExpress.XtraSpreadsheet.Layout.Page;
using DevExpress.Export.Xl;
using DevExpress.Office.Model;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Printing {
	public partial class PrintingDocumentExporter {
		TransformationBrick currentBorderContainer;
		void ExportPageBorders(LayoutPage page, List<PageBorderCollection> borders) {
			int count = borders.Count;
			for (int i = 0; i < count; i++)
				ExportBorders(page, borders[i]);
		}
		void ExportBorders(LayoutPage page, PageBorderCollection borders) {
			List<BorderLineBox> boxes = borders.Boxes;
			int count = boxes.Count;
			for (int i = 0; i < count; i++)
				ExportBorder(boxes[i], borders.GetBounds(page, boxes[i]));
		}
		void ExportBorder(BorderLineBox box, Rectangle boxBounds) {
			PatternLineBrick<XlBorderLineStyle> brick;
			if (boxBounds.Width >= boxBounds.Height)
				brick = new BorderLineBrick(DocumentModel.LayoutUnitConverter);
			else
				brick = new VerticalBorderLineBrick(DocumentModel.LayoutUnitConverter);
			VisualBrickHelper.InitializeBrick(brick, PrintingSystem, GetDrawingBounds(boxBounds));
			ColorModelInfo borderColorInfo = DocumentModel.Cache.ColorModelInfoCache[box.ColorIndex];
			Color borderColor = borderColorInfo.ToRgb(DocumentModel.StyleSheet.Palette, DocumentModel.OfficeTheme.Colors);
			if (DXColor.IsEmpty(borderColor))
				borderColor = DXColor.Black;
			brick.BackColor = DXColor.Transparent;
			brick.BorderColor = borderColor;
			brick.LineBounds = new RectangleF(0, 0, boxBounds.Width, boxBounds.Height);
			brick.NoClip = true;
			brick.PatternLineType = box.LineStyle;
			AddBorderBrickToCurrentPage(brick);
		}
		protected internal virtual void AddBorderBrickToCurrentPage(Brick brick) {
			currentBorderContainer.Bricks.Add(brick);
		}
	}
}
