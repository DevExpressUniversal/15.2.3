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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout.Export;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
#if !SL
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout {
	#region NumberingListBox
	public class NumberingListBox : MultiPositionBox {
		Rectangle initialBounds;
		public override bool IsVisible { get { return true; } }
		public override bool IsNotWhiteSpaceBox { get { return true; } }
		public override bool IsLineBreak { get { return false; } }
		public Rectangle InitialBounds { get { return initialBounds; } set { initialBounds = value; } }
		public override bool IsHyperlinkSupported { get { return false; } }
		public string NumberingListText { get; set; }
		protected internal override DocumentModelPosition GetDocumentPosition(PieceTable pieceTable, FormatterPosition pos) {
			Exceptions.ThrowInternalException();
			return base.GetDocumentPosition(pieceTable, pos);
		}
		public override FormatterPosition GetFirstFormatterPosition() {
			Exceptions.ThrowInternalException();
			return base.GetFirstFormatterPosition();
		}
		public override FormatterPosition GetLastFormatterPosition() {
			Exceptions.ThrowInternalException();
			return base.GetLastFormatterPosition();
		}
		public override DocumentModelPosition GetFirstPosition(PieceTable pieceTable) {
			Exceptions.ThrowInternalException();
			return base.GetFirstPosition(pieceTable);
		}
		public override DocumentModelPosition GetLastPosition(PieceTable pieceTable) {
			Exceptions.ThrowInternalException();
			return base.GetLastPosition(pieceTable);
		}
		public override Box CreateBox() {
			return new NumberingListBox();
		}
		public override void ExportTo(IDocumentLayoutExporter exporter) {
			exporter.ExportNumberingListBox(this);
		}
		public override BoxHitTestManager CreateHitTestManager(IBoxHitTestCalculator calculator) {
			return calculator.CreateNumberingListBoxHitTestManager(this);
		}
		public override string GetText(PieceTable table) {
			return NumberingListText;
		}
		public override FontInfo GetFontInfo(PieceTable pieceTable) {
			Paragraph paragraph = pieceTable.Runs[this.StartPos.RunIndex].Paragraph;
			return paragraph.GetNumerationFontInfo();
		}
		public MergedCharacterProperties GetNumerationCharacterProperties(PieceTable pieceTable) {
			Paragraph paragraph = pieceTable.Runs[this.StartPos.RunIndex].Paragraph;
			return paragraph.GetNumerationCharacterProperties();
		}
		public override Color GetActualForeColor(PieceTable pieceTable, TextColors textColors, Color backColor) {
			MergedCharacterProperties characterProperties = GetNumerationCharacterProperties(pieceTable);
			Color color = characterProperties.Info.ForeColor;
			return AutoColorUtils.GetActualForeColor(backColor, color, textColors);
		}
		protected internal override Color GetUnderlineColorCore(PieceTable pieceTable) {
			MergedCharacterProperties characterProperties = GetNumerationCharacterProperties(pieceTable);
			return characterProperties.Info.UnderlineColor;
		}
		protected internal override Color GetStrikeoutColorCore(PieceTable pieceTable) {
			MergedCharacterProperties characterProperties = GetNumerationCharacterProperties(pieceTable);
			return characterProperties.Info.StrikeoutColor;
		}
		public override UnderlineType GetFontUnderlineType(PieceTable pieceTable) {
			MergedCharacterProperties characterProperties = GetNumerationCharacterProperties(pieceTable);
			return characterProperties.Info.FontUnderlineType;
		}
		public override StrikeoutType GetFontStrikeoutType(PieceTable pieceTable) {
			MergedCharacterProperties characterProperties = GetNumerationCharacterProperties(pieceTable);
			return characterProperties.Info.FontStrikeoutType;
		}
	}
	#endregion
	#region NumberingListBoxWithSeparator
	public class NumberingListBoxWithSeparator : NumberingListBox {
		Box separatorBox;
		public Box SeparatorBox { get { return separatorBox; } set { separatorBox = value; } }
		public override void OffsetRunIndices(int delta) {
#if DEBUGTEST || DEBUG
			if (Object.ReferenceEquals(StartPos, SeparatorBox.StartPos) || Object.ReferenceEquals(EndPos, SeparatorBox.StartPos) ||
				Object.ReferenceEquals(StartPos, SeparatorBox.EndPos) || Object.ReferenceEquals(EndPos, SeparatorBox.EndPos))
				Exceptions.ThrowInternalException();
#endif
			base.OffsetRunIndices(delta);
			SeparatorBox.OffsetRunIndices(delta);
		}
	}
	#endregion
}
