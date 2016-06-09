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
using System.Drawing;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.XtraRichEdit.Services;
using DevExpress.Office.Drawing;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Drawing;
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region XpfNotPrintableGraphicsBoxExporter
	public class XpfNotPrintableGraphicsBoxExporter : NotPrintableGraphicsBoxExporter {
		readonly RichEditView view;
		Point offset;
		public XpfNotPrintableGraphicsBoxExporter(DocumentModel documentModel, RichEditView view, Painter painter, ICustomMarkExporter customMarkExporter, Point offset)
			: base(documentModel, painter, view, customMarkExporter) {
			this.view = view;
			this.offset = offset;
		}
		public XpfNotPrintableGraphicsBoxExporter(DocumentModel documentModel, RichEditView view, Painter painter, ICustomMarkExporter customMarkExporter)
			: this(documentModel, view, painter, customMarkExporter, Point.Empty) {
		}
		protected internal override Rectangle GetActualBounds(Rectangle bounds) {
			bounds.Offset(this.offset);
			return bounds;
		}
		protected internal override float PixelsToDrawingUnits(float value) {
			return UnitConverter.PixelsToLayoutUnitsF(value, Painter.DpiY);
		}
		protected internal override void ExportCustomMarkBoxCore(CustomMarkBox box) {
			Rectangle drawBounds = GetActualCustomMarkBounds(box.Bounds);
			XpfPainterOverwrite xpfPainter = Painter as XpfPainterOverwrite;
			if (xpfPainter == null)
				return;
			xpfPainter.DrawCustomMark(box.CustomMark, drawBounds);
		}
		protected internal override void ExportImeBoxes() {
			IImeService imeService = view.Control.GetService(typeof(IImeService)) as IImeService;
			if (imeService == null)
				return;
			ImeBoxExporter imeExporter = new ImeBoxExporter(this, imeService);
			imeExporter.Export(CurrentRow);
		}
	}
	#endregion
	public class ImeBoxExporter : UnderlineBoxExporter<UnderlineBox> {
		readonly IImeService imeService;
		readonly RunInfo imeStringRunInfo;
		readonly UnderlineDotted underlineDotted;
		public ImeBoxExporter(NotPrintableGraphicsBoxExporter exporter, IImeService imeService)
			: base(exporter) {
			Guard.ArgumentNotNull(imeService, "imeService");
			this.imeService = imeService;
			if (ImeService.IsActive)
				this.imeStringRunInfo = ImeService.GetImeStringRange();
			this.underlineDotted = new UnderlineDotted();
		}
		protected internal override Color UnderlineColor { get { return DXColor.Black; } }
		RunInfo ImeStringRunInfo { get { return imeStringRunInfo; } }
		IImeService ImeService { get { return imeService; } }
		public virtual void Export(Row row) {
			if (!ImeService.IsActive || ImeStringRunInfo == null)
				return;
			ImeUnderlineCalculator calculator = new ImeUnderlineCalculator(row.Paragraph.PieceTable, ImeStringRunInfo.NormalizedStart.RunIndex, ImeStringRunInfo.NormalizedEnd.RunIndex);
			calculator.Calculate(row);
			ExportTo(calculator.UnderlineBoxesByType);
		}
		protected override void DrawUnderlineByLines(RectangleF bounds, Pen pen) {
			LinePainter.DrawUnderline(underlineDotted, bounds, pen.Color);
		}
	}
	public class ImeUnderlineCalculator : DevExpress.XtraRichEdit.Layout.Engine.UnderlineCalculator {
		readonly RunIndex startRunIndex;
		readonly RunIndex endRunIndex;
		public ImeUnderlineCalculator(PieceTable pieceTable, RunIndex startRunIndex, RunIndex endRunIndex)
			: base(pieceTable) {
			this.startRunIndex = startRunIndex;
			this.endRunIndex = endRunIndex;
		}
		protected internal override void BeforeCalculate(Row row) {
			base.BeforeCalculate(row);
		}
		protected internal override void AfterCalculate() {
			base.AfterCalculate();
			UnderlineBoxesByType.ForEach(SetUnderlineBoxBounds);
		}
		protected internal override UnderlineType GetRunCharacterLineType(TextRun run, RunIndex runIndex) {
			if (runIndex >= startRunIndex && runIndex <= endRunIndex)
				return UnderlineType.Dotted;
			else
				return UnderlineType.None;
		}
		protected internal override bool GetRunCharacterLineUseForWordsOnly(TextRun run, RunIndex runIndex) {
			return false;
		}
	}
}
