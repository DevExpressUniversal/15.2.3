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
using DevExpress.XtraPrinting.NativeBricks;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native;
using DevExpress.Data.Utils;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class RichTextBoxBrickExporter : VisualBrickExporter {
		#region static
		static string CreateTextLayoutString(string text) {
			string s = text;
			s = s.Replace("\r\n", Convert.ToString('\x0000'));
			s = s.Replace("\n", Convert.ToString('\x0000'));
			s = s.Replace(Convert.ToString('\x0000'), "\r\n");
			return s;
		}
		#endregion
		RichTextBoxBrick RichTextBoxBrick { get { return Brick as RichTextBoxBrick; } }
		IRichTextBoxBrickOwner RichTextBoxBrickContainer { get { return RichTextBoxBrick.RichTextBoxBrickContainer; } }
		string RtfText { get { return RichTextBoxBrick.RtfText; } }
		protected override RectangleF GetClientRect(RectangleF rect) {
			return BrickPaint.GetClientRect(rect);
		}
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			if (RichTextBoxBrick.RtfText.GetHashCode() != gr.PrintingSystem.RichTextBoxForDrawing.Rtf.GetHashCode()) {
				gr.PrintingSystem.RichTextBoxForDrawing.Rtf = RichTextBoxBrick.RtfText;
			}
			gr.PrintingSystem.RichTextBoxForDrawing.BackColor = RichTextBoxBrick.BackColor;
			gr.PrintingSystem.RichTextBoxForDrawing.ForeColor = Style.ForeColor;
			gr.PrintingSystem.RichTextBoxForDrawing.DetectUrls = RichTextBoxBrick.DetectUrls;
			Image img = RichEditHelper.GetRtfImage(gr.PrintingSystem.RichTextBoxForDrawing, GraphicsDpi.Document, MathMethods.Scale(clientRect, 1 / VisualBrick.GetScaleFactor(gr)));
			if (img != null) {
				using (img) {
					gr.DrawImage(img, ImageTool.CalculateImageRect(clientRect, MathMethods.Scale(img.Size, VisualBrick.GetScaleFactor(gr)), ImageSizeMode.Normal), RichTextBoxBrick.BackColor);
				}
			}
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			string text = RichTextBoxBrickContainer.RichTextBox.Rtf;
			try {
				RichTextBoxBrickContainer.RichTextBox.Rtf = RtfText;
				exportProvider.SetCellText(RichTextBoxBrickContainer.RichTextBox.Text, null);
			}
			finally {
				RichTextBoxBrickContainer.RichTextBox.Rtf = text;
			}
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			string oldRtf = RichTextBoxBrickContainer.RichTextBox.Rtf;
			RichTextBoxBrickContainer.RichTextBox.Rtf = RtfText;
			try {
				exportProvider.SetCellText(CreateTextLayoutString(RichTextBoxBrickContainer.RichTextBox.Text), null);
			}
			finally {
				RichTextBoxBrickContainer.RichTextBox.Rtf = oldRtf;
			}
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			string savedRtf = RichTextBoxBrickContainer.RichTextBox.Rtf;
			try {
				RichTextBoxBrickContainer.RichTextBox.Rtf = RtfText;
				Image img = RichEditHelper.GetRtfImage(RichTextBoxBrickContainer.RichTextBox, GraphicsDpi.Pixel, RichTextBoxBrick.GetClientRectangle(exportProvider.CurrentData.Bounds, GraphicsDpi.Document));
				exportProvider.SetCellImage(img, null, ImageSizeMode.Normal, ImageAlignment.TopLeft, exportProvider.CurrentData.Bounds, exportProvider.CurrentData.Bounds.Size, PaddingInfo.Empty, null);
			}
			finally {
				RichTextBoxBrickContainer.RichTextBox.Rtf = savedRtf;
			}
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			string oldRtf = RichTextBoxBrickContainer.RichTextBox.Rtf;
			RichTextBoxBrickContainer.RichTextBox.Rtf = RtfText;
			try {
				exportProvider.SetCellText(RichTextBoxBrickContainer.RichTextBox.Text, null);
			}
			finally {
				RichTextBoxBrickContainer.RichTextBox.Rtf = oldRtf;
			}
		}
	}
}
