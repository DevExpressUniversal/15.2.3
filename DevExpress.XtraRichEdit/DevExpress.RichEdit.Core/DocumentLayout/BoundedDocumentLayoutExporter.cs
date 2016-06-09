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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Drawing;
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using TabsController = DevExpress.XtraRichEdit.Layout.Engine.TabsController;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region BoundedDocumentLayoutExporter (abstract class)
	public abstract class BoundedDocumentLayoutExporter : DocumentLayoutExporter, ICharacterLinePainter {
		Rectangle bounds;
		Rectangle visibleBounds;
		readonly RichEditPatternLinePainter horizontalLinePainter;
		readonly RichEditPatternLinePainter verticalLinePainter;
		readonly TextColors textColors;
		protected BoundedDocumentLayoutExporter(DocumentModel documentModel, Rectangle bounds, IPatternLinePaintingSupport linePaintingSupport, TextColors textColors)
			: base(documentModel) {
			Guard.ArgumentNotNull(linePaintingSupport, "linePaintingSupport");
			Guard.ArgumentNotNull(textColors, "textColors");
			this.horizontalLinePainter = CreateHorizontalLinePainter(linePaintingSupport);
			this.verticalLinePainter = CreateVerticalLinePainter(linePaintingSupport);
			this.bounds = bounds;
			this.textColors = textColors;
		}
		public TextColors TextColors { get { return textColors; } }
		internal Rectangle Bounds { get { return bounds; } set { bounds = value; } }
		public Rectangle VisibleBounds {
			get {
				return visibleBounds;
			}
			set {
				visibleBounds = value;
			}
		}
		protected virtual Point Offset { get { return bounds.Location; } }
		protected internal RichEditPatternLinePainter HorizontalLinePainter { get { return horizontalLinePainter; } }
		protected internal RichEditPatternLinePainter VerticalLinePainter { get { return verticalLinePainter; } }
		protected internal abstract RichEditPatternLinePainter CreateHorizontalLinePainter(IPatternLinePaintingSupport linePaintingSupport);
		protected internal abstract RichEditPatternLinePainter CreateVerticalLinePainter(IPatternLinePaintingSupport linePaintingSupport);
		protected internal override bool IsValidBounds(Rectangle boxBounds) {
#if DEBUGTEST || DEBUG
			Debug.Assert(!boxBounds.IsEmpty);
#endif
			return bounds.IntersectsWith(boxBounds) && !boxBounds.IsEmpty;
		}
		protected internal override Rectangle GetDrawingBounds(Rectangle bounds) {
			Rectangle drawingBounds = bounds;
			drawingBounds.Offset(Offset);
			return drawingBounds;
		}
		protected internal virtual char GetTabLeaderCharacter(TabLeaderType leaderType) {
			return TabsController.GetTabLeaderCharacter(leaderType);
		}
		protected internal virtual int GetTabLeaderCharacterWidth(TabSpaceBox box) {
			return TabsController.GetTabLeaderCharacterWidth(box, PieceTable);
		}
		protected internal virtual bool ShouldCenterTabLeaderLineVertically(TabSpaceBox box) {
			switch (box.TabInfo.Leader) {
				default:
				case TabLeaderType.Dots:
					return false;
				case TabLeaderType.MiddleDots:
					return true;
				case TabLeaderType.Hyphens:
					return true;
				case TabLeaderType.EqualSign:
					return true;
				case TabLeaderType.ThickLine:
				case TabLeaderType.Underline:
					return false;
			}
		}
		protected internal virtual UnderlineType GetTabLeaderUnderlineType(TabSpaceBox box) {
			switch (box.TabInfo.Leader) {
				default:
				case TabLeaderType.Dots:
					return UnderlineType.Dotted;
				case TabLeaderType.MiddleDots:
					return UnderlineType.Dotted;
				case TabLeaderType.Hyphens:
					return UnderlineType.Dashed;
				case TabLeaderType.EqualSign:
					return UnderlineType.Double;
				case TabLeaderType.ThickLine:
				case TabLeaderType.Underline:
					return UnderlineType.Single;
			}
		}
		protected internal override void ExportRows(Column column) {
			column.Rows.ForEach(ExportRowBackground);
			base.ExportRows(column);
		}
		protected internal virtual void ExportRowBackground(Row row) {
			if (!ShouldExportRowBackground(row)) 
				return;
			if (!ShouldExportRow(row))
				return;
			SetCurrentRow(row);
			ApplyCurrentRowTableCellClipping(row);
			ExportBackground();
			SetCurrentRow(null);
		}
		protected virtual bool ShouldExportRowBackground(Row row) {
			return ((row.InnerHighlightAreas != null && row.InnerHighlightAreas.Count > 0) || (row.InnerCommentHighlightAreas != null && row.InnerCommentHighlightAreas.Count > 0));
		}
		protected internal virtual string GetTabLeaderText(TabSpaceBox box, Rectangle textBounds) {
			if (box.TabInfo.Leader == TabLeaderType.None)
				return String.Empty;
			char character = GetTabLeaderCharacter(box.TabInfo.Leader);
			int count = box.LeaderCount;
			return new String(character, count);
		}
		protected virtual void ExportBackground() {
			ExportTextHighlighting();
		}
		protected void ExportTextHighlighting() {
			ExportHighlighting(CurrentRow.InnerHighlightAreas);
			ExportHighlighting(CurrentRow.InnerCommentHighlightAreas);
		}
		protected void ExportHighlighting(HighlightAreaCollection highlightAreas) {
			if (highlightAreas == null)
				return;
			int count = highlightAreas.Count;
			for (int i = 0; i < count; i++)
				ExportHighlightArea(highlightAreas[i]);
		}
		#region ICharacterLinePainter implementation
		protected internal void DrawDoubleSolidLine(RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawDoubleSolidLine(bounds, color);
		}
		protected internal void DrawPatternLine(RectangleF bounds, Color color, float[] pattern) {
			HorizontalLinePainter.DrawPatternLine(bounds, color, pattern);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineSingle underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDotted underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDashed underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDashSmallGap underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDashDotted underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDashDotDotted underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDouble underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineHeavyWave underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineLongDashed underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineThickSingle underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineThickDotted underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineThickDashed underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineThickDashDotted underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineThickDashDotDotted underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineThickLongDashed underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineDoubleWave underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IUnderlinePainter.DrawUnderline(UnderlineWave underline, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawUnderline(underline, bounds, color);
		}
		void IStrikeoutPainter.DrawStrikeout(StrikeoutSingle strikeout, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawStrikeout(strikeout, bounds, color);
		}
		void IStrikeoutPainter.DrawStrikeout(StrikeoutDouble Strikeout, RectangleF bounds, Color color) {
			HorizontalLinePainter.DrawStrikeout(Strikeout, bounds, color);
		}
		#endregion
		protected virtual Underline GetTableBorderLine(BorderLineStyle borderLineStyle) {
			if (borderLineStyle == BorderLineStyle.Single)
				return null;
			BorderLineRepository repository = this.DocumentModel.BorderLineRepository;
			return repository.GetCharacterLineByType(borderLineStyle);
		}
		protected int GetShapeOutlinePenWidth(FloatingObjectAnchorRun run, FloatingObjectBox box) {
			if (box.Bounds != box.ContentBounds)
				return Math.Max(1, DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(run.Shape.OutlineWidth));
			if (run.Shape.UseOutlineColor && !DXColor.IsTransparentOrEmpty(run.Shape.OutlineColor) && run.Shape.UseOutlineWidth && run.Shape.OutlineWidth == 0)
				return 0;
			return -1;
		}
		protected Box GetCharacterLineBoxByIndex(Row row, int index) {
			if (row.NumberingListBox != null) {
				if(index == 0)
					return row.NumberingListBox;
				else
					return row.Boxes[index - 1];
			}
			else
				return row.Boxes[index];
		}
	}
	#endregion
}
