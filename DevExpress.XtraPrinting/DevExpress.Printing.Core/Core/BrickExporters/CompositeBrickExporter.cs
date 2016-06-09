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
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Enumerators;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class CompositeBrickExporterBase : BrickBaseExporter {
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			var clipBounds = RectHelper.SnapRectangle(RectangleF.Intersect(gr.ClipBounds, rect), GraphicsDpi.Document, gr.Dpi);
			ExecuteClippedAction(gr, clipBounds, () => DrawInnerBricks(gr, rect));
		}
		void DrawInnerBricks(IGraphics gr, RectangleF rect) {
			BrickIterator bricks = CreateBrickIterator(gr, rect);
			while(bricks.MoveNext()) {
				BrickBase brick = bricks.CurrentBrick;
				RectangleF brickRect = bricks.CurrentBrickRectangle;
				brickRect.Offset(rect.X, rect.Y);
				try {
					var brickExporter = GetExporter(gr, brick);
					brickExporter.Draw(gr, brickRect, rect);
				} catch(Exception e) {
					if(e is System.Runtime.InteropServices.ExternalException &&
						(uint)(((System.Runtime.InteropServices.ExternalException)e).ErrorCode) == 0x80004005u)
						continue;
					Tracer.TraceError(NativeSR.TraceSource, e);
					GetExporter(gr, brick).DrawWarningRect(gr, brickRect, e.Message);
#if !DEBUGTEST
					if(gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics && PrintingSettings.PassPdfDrawingExceptions)
#endif
					throw;
				}
			}
		}
		protected virtual BrickIterator CreateBrickIterator(IGraphics gr, RectangleF rect) {
			return new BrickIterator(BrickBase.InnerBrickList, BrickBase.InnerBrickListOffset, RectangleF.Empty);
		}
		internal override void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
			RectangleF brickRect = BrickBase.GetViewRectangle();
			brickRect.Offset(pos);
			ProcessLayoutCore(layoutBuilder, brickRect, RectangleF.Intersect(clipRect, brickRect));
		}
		protected void ProcessLayoutCore(PageLayoutBuilder layoutBuilder, RectangleF rect, RectangleF clipRect) {
			rect.Offset(BrickBase.InnerBrickListOffset);
			foreach(BrickBase brick in BrickBase.InnerBrickList) {
				var brickExporter = GetExporter(layoutBuilder.ExportContext, brick);
				brickExporter.ProcessLayout(layoutBuilder, rect.Location, clipRect);
			}
		}
	}
	public class CompositeBrickExporter : CompositeBrickExporterBase {
		CompositeBrick CompositeBrick {
			get { return (CompositeBrick)BrickBase; }
		}
		protected override BrickIterator CreateBrickIterator(IGraphics gr, RectangleF rect) {
			if(BrickBase.InnerBrickList.Count > 5 * BrickMapConst.Graduation) {
				CompositeBrick.ForceBrickMap();
				MappedIndexedEnumerator mappedIterator = new MappedIndexedEnumerator(CompositeBrick.BrickMap, CompositeBrick.InnerBrickList) { ClipBounds = gr.ClipBounds, ViewOrigin = rect.Location };
				return new BrickIterator(mappedIterator, CompositeBrick.InnerBrickListOffset, RectangleF.Empty);
			}
			return base.CreateBrickIterator(gr, rect);
		}
	}
}
