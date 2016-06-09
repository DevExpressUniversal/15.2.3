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
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Layout.Export {
	#region CharacterBoxLevelDocumentLayoutExporter
	public class CharacterBoxLevelDocumentLayoutExporter : DocumentLayoutExporter {
		#region Fields
		readonly BoxMeasurer measurer;
		readonly CharacterBoxCollection chars;
		Rectangle rowBounds;
		#endregion
		public CharacterBoxLevelDocumentLayoutExporter(DocumentModel documentModel, CharacterBoxCollection chars, BoxMeasurer measurer)
			: base(documentModel) {
			Guard.ArgumentNotNull(measurer, "measurer");
			Guard.ArgumentNotNull(chars, "chars");
			this.chars = chars;
			this.measurer = measurer;
		}
		#region Properties
		public override DevExpress.Office.Drawing.Painter Painter {
			get { throw new NotImplementedException(); }
		}
		public CharacterBoxCollection Chars { get { return chars; } }
		public IObjectMeasurer Measurer { get { return measurer; } }
		#endregion
		public override void ExportTextBox(TextBox box) {
			Rectangle[] characterBounds = CalculateCharacterBounds(box);
			ExportMultiCharacterBox(box, characterBounds);
		}
		public override void ExportLayoutDependentTextBox(LayoutDependentTextBox box) {
			ExportSingleCharacterBox(box);
		}
		protected internal Rectangle[] CalculateCharacterBounds(TextBox box) {
			return measurer.MeasureCharactersBounds(GetBoxText(box), box.GetFontInfo(PieceTable), box.Bounds);
		}
		public override void ExportSpaceBox(Box spaceBox) {
			int spaceCount = spaceBox.EndPos.Offset - spaceBox.StartPos.Offset + 1;
			Rectangle[] characterBounds = RectangleUtils.SplitHorizontally(spaceBox.Bounds, spaceCount);
			ExportMultiCharacterBox(spaceBox, characterBounds);
		}
		protected internal virtual void AppendCharacterBox(FormatterPosition startPos, int offset, Rectangle bounds, Rectangle tightBounds) {
			CharacterBox characterBox = new CharacterBox();
			characterBox.Bounds = bounds;
			characterBox.TightBounds = tightBounds;
			FormatterPosition newStartPos = new FormatterPosition(startPos.RunIndex, startPos.Offset + offset, startPos.BoxIndex);
			characterBox.StartPos = newStartPos;
			characterBox.EndPos = newStartPos; 
			Chars.Add(characterBox);
		}
		protected internal virtual void ExportMultiCharacterBox(Box box, Rectangle[] characterBounds) {
			FormatterPosition startPos = box.StartPos;
			int count = box.EndPos.Offset - startPos.Offset + 1;
			Debug.Assert(characterBounds.Length == count);
			for (int i = 0; i < count; i++)
				AppendCharacterBox(startPos, i, GetActualCharacterBounds(characterBounds[i]), characterBounds[i]);
		}
		protected internal virtual Rectangle GetActualCharacterBounds(Rectangle characterBounds) {
			return new Rectangle(characterBounds.X, rowBounds.Y, characterBounds.Width, rowBounds.Height);
		}
		protected internal override void ExportRowCore() {
			this.rowBounds = CurrentRow.Bounds;
			PieceTable previousPieceTable = PieceTable;
			try {
				PieceTable = CurrentRow.Paragraph.PieceTable;
				base.ExportRowCore();
			}
			finally {
				PieceTable = previousPieceTable;
			}
		}
		public virtual void ExportRowBox(Row row, Box box) {
			this.rowBounds = row.Bounds;
			PieceTable previousPieceTable = PieceTable;
			try {
				PieceTable = row.Paragraph.PieceTable;
				box.ExportTo(this);
			}
			finally {
				PieceTable = previousPieceTable;
			}
		}
		protected internal virtual void ExportSingleCharacterBox(Box box) {
			AppendCharacterBox(box.StartPos, 0, GetActualCharacterBounds(box.Bounds), box.Bounds);
		}
		public override void ExportHyphenBox(HyphenBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportInlinePictureBox(InlinePictureBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportCustomRunBox(CustomRunBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportTabSpaceBox(TabSpaceBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportSeparatorBox(SeparatorBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportDataContainerRunBox(DataContainerRunBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportParagraphMarkBox(ParagraphMarkBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportSectionMarkBox(SectionMarkBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportLineBreakBox(LineBreakBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportPageBreakBox(PageBreakBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportColumnBreakBox(ColumnBreakBox box) {
			ExportSingleCharacterBox(box);
		}
		public override void ExportNumberingListBox(NumberingListBox box) {
			ExportSingleCharacterBox(box);
		}
	}
	#endregion
}
