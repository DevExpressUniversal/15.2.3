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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office.Layout;
using DevExpress.XtraPrinting;
using DevExpress.Office.Utils;
using DevExpress.Office.Drawing;
using DevExpress.Compatibility.System.Drawing;
#if SL
using System.Windows.Controls;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	#region InlinePictureBox
	public class InlinePictureBox : SinglePositionBox {
		public override Box CreateBox() {
			return new InlinePictureBox();
		}
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public override bool IsInlinePicture { get { return true; } }
		public override int CalcDescent(PieceTable pieceTable) {
			return 0;
		}
		public override int CalcAscentAndFree(PieceTable pieceTable) {
			return Bounds.Height;
		}
		public override int CalcBaseAscentAndFree(PieceTable pieceTable) {
			return CalcAscentAndFree(pieceTable);
		}
		public override int CalcBaseDescent(PieceTable pieceTable) {
			return CalcDescent(pieceTable);
		}
		public OfficeImage GetImage(PieceTable pieceTable, bool readOnly) {
			TextRunBase run = GetRun(pieceTable);
			return GetImageCore(run, readOnly);
		}
		protected virtual OfficeImage GetImageCore(TextRunBase run, bool readOnly) {
			InlinePictureRun pictureRun = run as InlinePictureRun;
			if (pictureRun != null)
				return pictureRun.Image;
			FloatingObjectAnchorRun floatingObjectAnchorRun = run as FloatingObjectAnchorRun;
			if (floatingObjectAnchorRun != null) {
				PictureFloatingObjectContent content = floatingObjectAnchorRun.Content as PictureFloatingObjectContent;
				if (content != null)
					return content.Image;
			}
			return null;
		}
		public Size GetImageActualSizeInLayoutUnits(PieceTable pieceTable) {
			InlinePictureRun run = GetRun(pieceTable) as InlinePictureRun;
			if (run == null)
				return Size.Empty;
			DocumentLayoutUnitConverter converter = pieceTable.DocumentModel.LayoutUnitConverter;
			Size imgSizeInPixels = run.Image.SizeInPixels;
			float hRes = run.Image.HorizontalResolution;
			float vRes = run.Image.VerticalResolution;
			return new Size(converter.PixelsToLayoutUnits(imgSizeInPixels.Width, hRes), converter.PixelsToLayoutUnits(imgSizeInPixels.Height, vRes));
		}
		public ImageSizeMode GetSizing(PieceTable pieceTable) {
			InlinePictureRun run = GetRun(pieceTable) as InlinePictureRun;
			if (run == null)
				return ImageSizeMode.StretchImage;
			return run.Sizing;
		}
		public ResizingShadowDisplayMode GetResizingShadowDisplayMode(PieceTable pieceTable) {
			InlinePictureRun run = GetRun(pieceTable) as InlinePictureRun;
			if (run == null)
				return ResizingShadowDisplayMode.Content;
			return run.ResizingShadowDisplayMode;
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportInlinePictureBox(this);
		}
		protected internal virtual void ExportHotZones(Painter painter) {
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateInlinePictureBoxHitTestManager(this);
		}
		public override void Accept(IRowBoxesVisitor visitor) {
			visitor.Visit(this);
		}
	}
	#endregion
}
