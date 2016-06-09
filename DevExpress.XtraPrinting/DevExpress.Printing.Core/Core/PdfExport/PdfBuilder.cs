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
using System.IO;
using DevExpress.Utils;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting.Export.Pdf {
	public class PdfDocumentBuilder {
		public static void CreateDocument(Stream stream, DevExpress.XtraPrinting.Document document, PdfExportOptions pdfOptions) {
			PrintingSystemBase ps = document.PrintingSystem;
			PdfGraphics gr = new PdfGraphics(stream, pdfOptions, document.PrintingSystem);
			gr.ImageBackColor = GetValidColor(document.PrintingSystem.Graph.PageBackColor);
			try {
				int[] pageIndices = ExportOptionsHelper.GetPageIndices(pdfOptions, document.PageCount);
				gr.ProgressReflector.InitializeRange(pageIndices.Length + 3);
				for(int i = 0; i < pageIndices.Length; i++) {
					if(ps.CancelPending) break;
					Page page = document.Pages[pageIndices[i]];
					gr.AddPage(page.PageSize);
					PageExporter exporter = gr.PrintingSystem.ExportersFactory.GetExporter(page) as PageExporter;
					exporter.DrawPage(gr, PointF.Empty);
				}
				if(pageIndices.Length > 0 && document.BookmarkNodes.Count > 0)
					AddOutlineEntries(null, document.RootBookmark, gr, pageIndices);
			} finally {
				gr.Flush();
				gr.ProgressReflector.MaximizeRange();
			}
		}
		static Color GetValidColor(Color c) {
			return DXColor.IsTransparentOrEmpty(c) ? DXColor.White : c;
		}
		static void AddOutlineEntries(PdfOutlineEntry parent, BookmarkNode bmNode, PdfGraphics gr, int[] pageIndices) {
			RectangleF rect = bmNode.Pair.GetBrickBounds(gr.PrintingSystem.Pages);
			float top = rect != RectangleF.Empty ? rect.Top : 0;
			int rangeIndex = bmNode.GetPageRangeIndex(pageIndices);
			if(rangeIndex >= 0) {
				PdfOutlineEntry entry = gr.SetOutlineEntry(parent, bmNode.Text, rangeIndex, top);
				foreach(BookmarkNode item in bmNode.Nodes) {
					AddOutlineEntries(entry, item, gr, pageIndices);
				}
			}
		}
	}
}
