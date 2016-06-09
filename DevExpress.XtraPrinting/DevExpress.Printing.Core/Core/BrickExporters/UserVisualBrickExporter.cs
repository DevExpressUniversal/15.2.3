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
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class UserVisualBrickExporter : BrickExporter {
		UserVisualBrick UserVisualBrick { get { return Brick as UserVisualBrick; } }
		IBrick UserBrick { get { return UserVisualBrick.UserBrick; } }
		RectangleF Rect { get { return UserVisualBrick.Rect; } }
		public override void Draw(Graphics gr, RectangleF rect) {
			if (UserBrick != null) {
				BrickExporter exporter = GetExporter(UserVisualBrick.PrintingSystem, UserBrick) as BrickExporter;
				exporter.Draw(gr, rect);
			}
		}
		internal protected override void DrawPdf(PdfGraphics gr, RectangleF rect) {
			if (UserBrick != null)
				if (rect.Width > 0 && rect.Height > 0)
					gr.DrawImage(GetBrickImage(UserVisualBrick, gr.PrintingSystem, rect), rect);
		}
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			if (UserBrick != null) {
				Image img = GetBrickImage(exportProvider.HtmlExportContext.PrintingSystem, Rect);
				exportProvider.SetCellImage(img, null, ImageSizeMode.Normal, ImageAlignment.TopLeft, new Rectangle(Point.Empty, img.Size), img.Size, PaddingInfo.Empty, null);
			}
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			if(UserBrick != null) {
				Image img = GetBrickImage(exportProvider.ExportContext.PrintingSystem, Rect);
				exportProvider.SetCellImage(img, null, ImageSizeMode.Normal, ImageAlignment.TopLeft, new Rectangle(Point.Empty, img.Size), img.Size, PaddingInfo.Empty, null);
			}
		}
		Image GetBrickImage(UserVisualBrick userVisualBrick, PrintingSystemBase ps, RectangleF rect) {
			return PSBrickPaint.GetBrickImage(userVisualBrick.UserBrick, rect, ps.Graph.PageBackColor, ps);
		}
		Image GetBrickImage(PrintingSystemBase ps, RectangleF rect) {
			return PSBrickPaint.GetBrickImage(UserBrick, rect, ps.Graph.PageBackColor, ps);
		}
	}
}
