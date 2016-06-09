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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Export.Pdf;
using DevExpress.XtraPrinting.Export;
using System;
using DevExpress.Printing;
namespace DevExpress.XtraPrinting.BrickExporters {
	public static class PageExporterExtension {
		public static void DrawPage(this PageExporter exporter, IGraphics gr, PointF location) {
			exporter.Draw(gr, new RectangleF(location, exporter.Page.PageSize), RectangleF.Empty);
		}
	}
	public class PageExporter : CompositeBrickExporterBase {
		public bool IsPrinting { get; set; }
		public Page Page { get { return Brick as Page; } }
		public PageExporter() {
			IsPrinting = false;
		}
		public override void Draw(IGraphics gr, RectangleF rect, RectangleF parentRect) {
			PrintingSystemBase printingSystem = gr.PrintingSystem;
			if(rect.IsEmpty)
				return;
			gr.SetDrawingPage(Page);
			try {
				DrawBackground(gr, rect, printingSystem.Graph.PageBackColor);
				if(Page.ActualWatermark.ShowBehind)
					DrawWatermark(gr, rect);
				PagePaintEventArgs args = new PagePaintEventArgs(Page, gr, RectHelper.DeflateRect(rect, Page.PageData.MinMarginsF));
				printingSystem.OnBeforePagePaint(args);
				base.Draw(gr, rect, parentRect);
				if(!Page.ActualWatermark.ShowBehind)
					DrawWatermark(gr, rect);
				printingSystem.OnAfterPagePaint(args);
			} finally {
				gr.ResetDrawingPage();
			}
			PrintingDocument doc = printingSystem.Document as PrintingDocument;
			if(doc != null && doc.InfoString != string.Empty)
				DrawInformation(doc.InfoString, gr, rect, printingSystem);
		}
		static void DrawInformation(string infoString, IGraphics gr, RectangleF rect, PrintingSystemBase ps) {
			SizeF size = InformationHelper.CalcSize(infoString, ps.Graph.Dpi, gr.Measurer);
			RectangleF brickRect = new RectangleF(rect.Left + (rect.Width - size.Width) / 2.0f,
				rect.Bottom - size.Height - 30, size.Width, size.Height);
			using(StringFormat infoFormat = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center }) {
				using(Brush foreBrush = new SolidBrush(InformationHelper.Color)) {
					gr.DrawString(infoString, InformationHelper.Font, foreBrush, brickRect, infoFormat);
				}
			}
		}
		void DrawBackground(IGraphics gr, RectangleF rect, Color backColor) {
			if (gr is PdfGraphics && backColor.A == 0)
				return;
			SolidBrush brush = new SolidBrush(ValidateBackgrColor(backColor));
			gr.FillRectangle(brush, rect);
			brush.Dispose();
		}
		void DrawWatermark(IGraphics gr, RectangleF rect) {
			GraphicsModifier modifier = ((IServiceProvider)gr.PrintingSystem).GetService(typeof(GraphicsModifier)) as GraphicsModifier;
			if(!(modifier is GdiPlusGraphicsModifier)) return;
			Page.ActualWatermark.Draw(gr, RectHelper.DeflateRect(rect, Page.PageData.MinMarginsF), Page.Index, gr.PrintingSystem.Document.PageCount);
		}
		Color ValidateBackgrColor(Color color) {
			if(IsPrinting)
				return color == DXSystemColors.Window ? DXColor.White : color;
			else
				return PSNativeMethods.ValidateBackgrColor(color);
		}
		internal override void ProcessLayout(PageLayoutBuilder layoutBuilder, PointF pos, RectangleF clipRect) {
			ProcessLayoutCore(layoutBuilder, clipRect, clipRect);
		}
	}
}
