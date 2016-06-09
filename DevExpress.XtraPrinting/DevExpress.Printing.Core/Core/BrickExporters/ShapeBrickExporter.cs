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
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.Export;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class ShapeBrickExporter : VisualBrickExporter {
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			SmoothingMode oldSmoothingMode = gr.SmoothingMode;
			try {
				gr.SmoothingMode = SmoothingMode.AntiAlias;
				(Brick as ShapeBrick).Shape.DrawContent(gr, clientRect, (Brick as ShapeBrick), gr.PrintingSystem.Gdi);
			}
			finally {
				gr.SmoothingMode = oldSmoothingMode;
			}
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			FillHtmlTableCellWithImage(exportProvider);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(exportContext.RawDataMode)
				return new BrickViewData[0];
			return DrawContentToViewData(exportContext, GraphicsUnitConverter.Round(rect), Style.TextAlignment);
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			FillTableCellWithImage(exportProvider, ImageSizeMode.StretchImage, ImageAlignment.Default, exportProvider.CurrentData.OriginalBounds);
		}
		protected override Image DrawContentToImage(ExportContext exportContext, RectangleF rect) {
			return DrawContentToImage(exportContext.PrintingSystem, exportContext.PrintingSystem.GarbageImages, rect, false, GraphicsDpi.Pixel);
		}
	}
}
