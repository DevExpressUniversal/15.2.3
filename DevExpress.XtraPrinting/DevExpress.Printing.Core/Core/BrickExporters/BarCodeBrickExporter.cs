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
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using System.Windows.Forms;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class BarCodeBrickExporter : VisualBrickExporter {
		BarCodeBrick BarCodeBrick { get { return Brick as BarCodeBrick; } }
		protected override RectangleF GetClientRect(RectangleF rect) {
			return BrickPaint.GetClientRect(rect);
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			BarCodeBrick.Generator.DrawContent(gr, clientRect, BarCodeBrick);
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			FillHtmlTableCellWithImage(exportProvider);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			if(exportContext.RawDataMode || !exportContext.CanPublish(BarCodeBrick))
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
	}
}
