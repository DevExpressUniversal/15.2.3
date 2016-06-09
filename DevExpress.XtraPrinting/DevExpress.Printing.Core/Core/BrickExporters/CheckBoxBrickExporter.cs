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
using System.Windows.Forms;
using DevExpress.XtraPrinting.Export;
namespace DevExpress.XtraPrinting.BrickExporters {
	public class CheckBoxBrickExporter : VisualBrickExporter {
		CheckBoxBrick CheckBoxBrick { get { return Brick as CheckBoxBrick; } }
		CheckState CheckState { get { return CheckBoxBrick.CheckState; } }
		protected override void DrawObject(IGraphics gr, RectangleF rect) {
			BrickPaint.DrawCheck(CheckState, gr, rect, GetScaledCheckSize(gr), gr.PrintingSystem.Gdi, CheckBoxBrick.ShouldAlignToBottom, CheckBoxBrick.ToDpi);
		}
		protected override object[] GetSpecificKeyPart() {
			return new object[] { CheckState };
		}
		protected override RectangleF DeflateBorderWidth(RectangleF rect) {
			return rect;
		}
		internal SizeF GetScaledCheckSize(IPrintingSystemContext context) {
			return MathMethods.Scale(CheckBoxBrick.CheckSize, VisualBrick.GetScaleFactor(context));
		}
		protected override void FillHtmlTableCellCore(IHtmlExportProvider exportProvider) {
			Image img = DrawContentToImage(exportProvider.HtmlExportContext, new RectangleF(PointF.Empty, GetScaledCheckSize(exportProvider.ExportContext)));
			exportProvider.SetCellImage(img, null, ImageSizeMode.Normal, ImageAlignment.MiddleCenter, exportProvider.CurrentData.Bounds, img.Size, PaddingInfo.Empty, Url);
		}
		protected override void FillRtfTableCellCore(IRtfExportProvider exportProvider) {
			FillTableCell(exportProvider);
		}
		protected internal override void FillXlsTableCellInternal(IXlsExportProvider exportProvider) {
			if(exportProvider.XlsExportContext.RawDataMode)
				exportProvider.SetCellText(ConvertToValue(CheckBoxBrick.CheckState), null);
			else
				FillTableCell(exportProvider);
		}
		static object ConvertToValue(CheckState checkState) {
			if(checkState == CheckState.Checked)
				return true;
			else if(checkState == CheckState.Unchecked)
				return false;
			else
				return System.DBNull.Value;
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			RectangleF rect = new RectangleF(Point.Empty, exportProvider.CurrentData.BoundsF.Size);
			Image img = DrawContentToImage(exportProvider.ExportContext, Style.DeflateBorderWidth(rect, GraphicsDpi.DeviceIndependentPixel, true));
			exportProvider.SetCellImage(img, null, ImageSizeMode.Normal, ImageAlignment.TopLeft, new Rectangle(Point.Empty, img.Size), img.Size, PaddingInfo.Empty, Url);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			exportProvider.SetCellText(CheckBoxBrick.CheckStateText, null);
		}
	}
}
