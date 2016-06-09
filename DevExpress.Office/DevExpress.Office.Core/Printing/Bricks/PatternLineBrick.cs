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
using System.Drawing;
using DevExpress.Office.Drawing;
using DevExpress.Office.Model;
using DevExpress.Office.Layout;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
#if !SL
using System.Drawing.Drawing2D;
using DevExpress.XtraPrinting.BrickExporters;
using DevExpress.Office.Printing;
#else
#endif
namespace DevExpress.Office.Printing {
	#region PatternLineBrick<T> (abstract class)
	public abstract class PatternLineBrick<T> : VisualBrick where T : struct {
		readonly DocumentLayoutUnitConverter unitConverter;
		T patternLineType;
		RectangleF lineBounds;
		protected PatternLineBrick(DocumentLayoutUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public T PatternLineType { get { return patternLineType; } set { patternLineType = value; } }
		public RectangleF LineBounds { get { return lineBounds; } set { lineBounds = value; } }
		protected internal virtual DocumentLayoutUnitConverter GetPainterUnitConverter(IGraphics gr) {
#if !SL
			if (gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics)
				return new DocumentLayoutUnitDocumentConverter();
			else
#endif
				return unitConverter;
		}
		protected internal virtual RectangleF CalculateLineBounds(IGraphics gr, RectangleF clientRect) {
#if !SL
			if (gr is DevExpress.XtraPrinting.Export.Pdf.PdfGraphics) {
				RectangleF bounds = unitConverter.LayoutUnitsToDocuments(Rectangle.Round(LineBounds));
				bounds.Offset(clientRect.Location);
				return bounds;
			}
			else {
#endif
				RectangleF bounds = LineBounds;
				int x = unitConverter.DocumentsToLayoutUnits((int)Math.Round(clientRect.X));
				int y = unitConverter.DocumentsToLayoutUnits((int)Math.Round(clientRect.Y));
				bounds.Offset(x, y);
				return bounds;
#if !SL
			}
#endif
		}
		protected override void Scale(double scaleFactor) {
			base.Scale(scaleFactor);
			LineBounds = MathMethods.Scale(LineBounds, scaleFactor);
		}
		protected internal abstract PatternLine<T> GetPatternLine();
	}
	#endregion
#if !SL
	#region PatternLineBrickExporter<T> (abstract class)
	public abstract class PatternLineBrickExporter<T> : VisualBrickExporter where T : struct {
		protected PatternLineBrick<T> PatternLineBrick { get { return Brick as PatternLineBrick<T>; } }
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			using (IGraphicsPainter painter = new IGraphicsPainter(gr)) {
				IPatternLinePainter<T> linePainter = CreateLinePainter(painter, PatternLineBrick.GetPainterUnitConverter(gr));
				PatternLine<T> line = PatternLineBrick.GetPatternLine();
				line.Draw(linePainter, PatternLineBrick.CalculateLineBounds(gr, clientRect), PatternLineBrick.BorderColor);
			}
		}
		protected abstract IPatternLinePainter<T> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter);
	}
	#endregion
	#region VerticalPatternLineBrickExporter<T>
	public abstract class VerticalPatternLineBrickExporter<T> : VisualBrickExporter where T : struct {
		protected PatternLineBrick<T> PatternLineBrick { get { return Brick as PatternLineBrick<T>; } }
		protected override void DrawClientContent(IGraphics gr, RectangleF clientRect) {
			using (IGraphicsPainter painter = new IGraphicsPainter(gr)) {
				IPatternLinePainter<T> linePainter = CreateLinePainter(painter, PatternLineBrick.GetPainterUnitConverter(gr));
				PatternLine<T> line = PatternLineBrick.GetPatternLine();
				line.Draw(linePainter, PatternLineBrick.CalculateLineBounds(gr, clientRect), PatternLineBrick.BorderColor);
			}
		}
		protected abstract IPatternLinePainter<T> CreateLinePainter(IGraphicsPainter painter, DocumentLayoutUnitConverter unitConverter);
	}
	#endregion
#endif
}
