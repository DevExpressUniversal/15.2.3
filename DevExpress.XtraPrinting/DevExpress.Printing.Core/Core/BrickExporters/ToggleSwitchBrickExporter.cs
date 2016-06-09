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
	public class ToggleSwitchBrickExporter : VisualBrickExporter {
		ToggleSwitchBrick ToggleSwitchBrick { get { return Brick as ToggleSwitchBrick; } }
		bool IsOn { get { return ToggleSwitchBrick.IsOn; } }
		protected override void DrawObject(IGraphics gr, RectangleF rect) {
			BrickPaint.DrawToggleSwitch(ToggleSwitchBrick.IsOn, gr, ToggleSwitchBrick.ImageList.ToArray(), rect, gr.PrintingSystem.Gdi);
		}
		protected override object[] GetSpecificKeyPart() {
			return new object[] { IsOn };
		}
		protected override RectangleF DeflateBorderWidth(RectangleF rect) {
			return rect;
		}
		internal SizeF GetScaledCheckSize(IPrintingSystemContext context) {
			return MathMethods.Scale(ToggleSwitchBrick.CheckSize, VisualBrick.GetScaleFactor(context));
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
				exportProvider.SetCellText(ToggleSwitchBrick.IsOn, null);
			else
				FillTableCell(exportProvider);
		}
		void FillTableCell(ITableExportProvider exportProvider) {
			Image img = DrawContentToImage(exportProvider.ExportContext, new RectangleF(PointF.Empty, exportProvider.CurrentData.BoundsF.Size));
			exportProvider.SetCellImage(img, null, ImageSizeMode.Normal, ImageAlignment.TopLeft, new Rectangle(Point.Empty, img.Size), img.Size, PaddingInfo.Empty, Url);
		}
		protected internal override void FillTextTableCellInternal(ITableExportProvider exportProvider, bool shouldSplitText) {
			exportProvider.SetCellText(ToggleSwitchBrick.CheckStateText, null);
		}
	}
}
