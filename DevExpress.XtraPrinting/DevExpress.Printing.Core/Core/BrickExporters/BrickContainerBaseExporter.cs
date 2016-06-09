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
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class BrickContainerBaseExporter : BrickExporter {
		BrickContainerBase BrickContainerBase { get { return Brick as BrickContainerBase; } }
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			GetBaseBrickExporter(gr).Draw(gr, BrickContainerBase.GetBrickRect(rect.Location), parentRect);
		}
		protected internal override BrickViewData[] GetHtmlData(ExportContext htmlExportContext, RectangleF bounds, RectangleF clipBounds) {
			return GetBaseBrickExporter(htmlExportContext).GetHtmlData(htmlExportContext, bounds, clipBounds);
		}
		protected internal override BrickViewData[] GetXlsData(ExportContext xlsExportContext, RectangleF rect) {
			return GetBaseBrickExporter(xlsExportContext).GetXlsData(xlsExportContext, rect);
		}
		protected internal override BrickViewData[] GetTextData(ExportContext exportContext, RectangleF rect) {
			return GetBaseBrickExporter(exportContext).GetTextData(exportContext, rect);
		}
		protected internal override void FillRtfTableCellInternal(IRtfExportProvider exportProvider) {
			GetBaseBrickExporter(exportProvider.ExportContext).FillRtfTableCellInternal(exportProvider);
		}
		protected internal override void FillHtmlTableCellInternal(IHtmlExportProvider exportProvider) {
			GetBaseBrickExporter(exportProvider.ExportContext).FillHtmlTableCellInternal(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			GetBaseBrickExporter(exportProvider.ExportContext).FillXlsTableCellInternal(exportProvider);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			GetBaseBrickExporter(exportProvider.ExportContext).FillTextTableCellInternal(exportProvider, shouldSplitText);
		}
		protected internal override BrickViewData[] GetExportData(ExportContext exportContext, RectangleF rect, RectangleF clipRect) {
			return GetBaseBrickExporter(exportContext).GetExportData(exportContext, rect, clipRect);
		}
		BrickExporter GetBaseBrickExporter(IPrintingSystemContext context) {
			return GetExporter(context, BrickContainerBase.Brick) as BrickExporter;
		}
		internal override void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
			BrickExporter exporter = GetBaseBrickExporter(layoutBuilder.ExportContext);
			PointF containerOffset = BrickContainerBase.GetViewRectangle().Location;
			PointF brickOffset = BrickContainerBase.Brick.GetViewRectangle().Location;
			pos.X += containerOffset.X - brickOffset.X;
			pos.Y += containerOffset.Y - brickOffset.Y;
			exporter.ProcessLayout(layoutBuilder, pos, clipRect);
		}
	}
}
