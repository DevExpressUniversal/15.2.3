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
using DevExpress.Office;
using DevExpress.Office.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Layout.Engine;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Fields;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region TextRun
	public class TextRun : TextRunBase, IHighlightableTextRun {
		public TextRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public TextRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		public override bool CanPlaceCaretBefore { get { return true; } }
		public bool MatchFormatting(CharacterFormattingInfo formatting, CharacterFormattingOptions options, int styleIndex) {
			if (CharacterStyleIndex != styleIndex)
				return false;
			CharacterProperties properties = CharacterProperties;
			CharacterFormattingBase formattingBaseInfo = properties.Info;
			CharacterFormattingOptions runOptions = formattingBaseInfo.Options;
			if (!options.Equals(runOptions))
				return false;
			CharacterFormattingInfo runInfo = formattingBaseInfo.Info;			
			return 
				(!runOptions.UseFontName || runInfo.FontName == formatting.FontName) &&
				(!runOptions.UseDoubleFontSize || runInfo.DoubleFontSize == formatting.DoubleFontSize) &&
				(!runOptions.UseFontBold || runInfo.FontBold == formatting.FontBold) &&
				(!runOptions.UseFontItalic || runInfo.FontItalic == formatting.FontItalic) &&
				(!runOptions.UseFontStrikeoutType || runInfo.FontStrikeoutType == formatting.FontStrikeoutType) &&
				(!runOptions.UseFontUnderlineType || runInfo.FontUnderlineType == formatting.FontUnderlineType) &&
				(!runOptions.UseAllCaps || runInfo.AllCaps == formatting.AllCaps) &&
				(!runOptions.UseHidden || runInfo.Hidden == formatting.Hidden) &&
				(!runOptions.UseStrikeoutWordsOnly || runInfo.StrikeoutWordsOnly == formatting.StrikeoutWordsOnly) &&
				(!runOptions.UseUnderlineWordsOnly || runInfo.UnderlineWordsOnly == formatting.UnderlineWordsOnly) &&
				(!runOptions.UseForeColor || runInfo.ForeColor == formatting.ForeColor) &&
				(!runOptions.UseBackColor || runInfo.BackColor == formatting.BackColor) &&
				(!runOptions.UseUnderlineColor || runInfo.UnderlineColor == formatting.UnderlineColor) &&
				(!runOptions.UseStrikeoutColor || runInfo.StrikeoutColor == formatting.StrikeoutColor) &&
				(!runOptions.UseLangInfo || runInfo.LangInfo.Equals(formatting.LangInfo)) &&
				(!runOptions.UseNoProof || runInfo.NoProof == formatting.NoProof) &&
				(!runOptions.UseScript || runInfo.Script == formatting.Script);
		}
		public void InheritStyleAndFormattingFrom(TextRun run) {
			if (run == null) {
				Exceptions.ThrowArgumentException("run", run);
				return;
			}
			this.CharacterStyleIndex = run.CharacterStyleIndex;
			this.CharacterProperties.CopyFrom(run.CharacterProperties);
			this.InnerFontCacheIndex = run.InnerFontCacheIndex;
			this.InnerMergedCharacterFormattingCacheIndex = run.InnerMergedCharacterFormattingCacheIndex;
			this.RowProcessingFlags = run.RowProcessingFlags;
		}
		public override bool CanJoinWith(TextRunBase nextRun) {
			if (nextRun == null)
				Exceptions.ThrowArgumentException("nextRun", nextRun);
			if (!(nextRun is TextRun))
				return false;
			if (IsFieldRun(nextRun))
				return false;
			if (!Object.ReferenceEquals(Paragraph, nextRun.Paragraph))
				return false;
			if (StartIndex + Length != nextRun.StartIndex)
				return false;
			return CharacterStyleIndex == nextRun.CharacterStyleIndex &&
				this.CharacterProperties.Index == nextRun.CharacterProperties.Index;
		}
		bool IsFieldRun(TextRunBase run) {
			return run is FieldCodeRunBase || run is FieldResultEndRun;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			string text = PieceTable.GetTextFromSingleRun(boxInfo.StartPos, boxInfo.EndPos);
			FontInfo fontInfo = DocumentModel.FontCache[FontCacheIndex];
			measurer.MeasureText(boxInfo, text, fontInfo);
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {
			string text = PieceTable.GetTextFromSingleRun(boxInfo.StartPos, boxInfo.EndPos);
			FontInfo fontInfo = DocumentModel.FontCache[FontCacheIndex];
			return measurer.TryAdjustEndPositionToFit(boxInfo, text, fontInfo, maxWidth);
		}
		public override string GetText(ChunkedStringBuilder growBuffer, int from, int to) {
			string text = base.GetText(growBuffer, from, to);
			if (AllCaps)
				text = text.ToUpper();
			return text;
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		public override TextRunBase Copy(DocumentModelCopyManager copyManager) {
			PieceTable targetPieceTable = copyManager.TargetPieceTable;
			DocumentModelPosition targetPosition = copyManager.TargetPosition;
			Debug.Assert(this.DocumentModel == copyManager.SourceModel);
			Debug.Assert(targetPosition.PieceTable == targetPieceTable);
			Debug.Assert(targetPosition.RunOffset == 0);
			CopyContentCore(copyManager);
			TextRunBase run = targetPieceTable.Runs[targetPosition.RunIndex];
			CopyCore(copyManager.TargetModel, run);
			return run;
		}
		protected internal virtual void CopyCore(DocumentModel targetModel, TextRunBase targetRun) {
			DocumentCapabilitiesOptions options = targetModel.DocumentCapabilities;
			if (options.CharacterFormattingAllowed)
				targetRun.CharacterProperties.CopyFrom(this.CharacterProperties.Info);
			if (options.CharacterStyleAllowed)
				targetRun.CharacterStyleIndex = CharacterStyle.Copy(targetModel);
		}
		internal virtual TextRun CreateRun(Paragraph paragraph, int startIndex, int length) {
			return new TextRun(paragraph, startIndex, length);
		}
		protected virtual void CopyContentCore(DocumentModelCopyManager copyManager) {
			DocumentLogPosition logPosition = copyManager.TargetPosition.LogPosition;
			if (!Object.ReferenceEquals(copyManager.TargetPieceTable, copyManager.TargetModel.MainPieceTable))
				copyManager.TargetPieceTable.InsertText(logPosition, GetTextFast(copyManager.SourcePieceTable.TextBuffer).Replace(Characters.PageBreak, Characters.LineBreak));
			else 
				copyManager.TargetPieceTable.InsertText(logPosition, GetTextFast(copyManager.SourcePieceTable.TextBuffer));
		}
	}
	#endregion
	#region SpecialTextRun (abstract class)
	public abstract class SpecialTextRun : TextRun {
		protected SpecialTextRun(Paragraph paragraph)
			: base(paragraph) {
		}
		protected SpecialTextRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {
		}
		protected internal override RowProcessingFlags RowProcessingFlags {
			get { return base.RowProcessingFlags | RowProcessingFlags.ProcessSpecialTextBoxes; }
			set { base.RowProcessingFlags = value; }
		}
	}
	#endregion
	#region LayoutDependentTextRun
	public class LayoutDependentTextRun : TextRun {
		FieldResultFormatting fieldResultFormatting; 
		public LayoutDependentTextRun(Paragraph paragraph)
			: base(paragraph) {
		}
		public LayoutDependentTextRun(Paragraph paragraph, int startIndex, int length)
			: base(paragraph, startIndex, length) {			
		}
		public override bool CanPlaceCaretBefore { get { return true; } }
		public FieldResultFormatting FieldResultFormatting { get { return fieldResultFormatting; } set { fieldResultFormatting = value; } }
		public override bool CanJoinWith(TextRunBase nextRun) {
			return false;
		}
		protected internal override void Measure(BoxInfo boxInfo, IObjectMeasurer measurer) {
			FontInfo fontInfo = DocumentModel.FontCache[FontCacheIndex];
			measurer.MeasureText(boxInfo, ((LayoutDependentTextBox)boxInfo.Box).CalculatedText, fontInfo);
		}
		protected internal override bool TryAdjustEndPositionToFit(BoxInfo boxInfo, int maxWidth, IObjectMeasurer measurer) {			
			return false;
		}
		public override string GetText(ChunkedStringBuilder growBuffer, int from, int to) {
			return "#";
		}
		public override void Export(IDocumentModelExporter exporter) {
			exporter.Export(this);
		}
		internal override TextRun CreateRun(Paragraph paragraph, int startIndex, int length) {
			Exceptions.ThrowInternalException();
			return null;
		}
		protected override void CopyContentCore(DocumentModelCopyManager copyManager) {
			DocumentLogPosition logPosition = copyManager.TargetPosition.LogPosition;
			ParagraphIndex paragraphIndex = copyManager.TargetPosition.ParagraphIndex;
			copyManager.TargetPieceTable.InsertLayoutDependentTextRun(paragraphIndex, logPosition, fieldResultFormatting);
		}
		protected internal override RowProcessingFlags CalculateRowProcessingFlags() {
			return base.CalculateRowProcessingFlags() | RowProcessingFlags.ProcessLayoutDependentText;
		}
		protected override void InheritRowProcessgFlags(TextRunBase run) {
			this.RowProcessingFlags = run.RowProcessingFlags | RowProcessingFlags.ProcessLayoutDependentText; 
		}
	}
	#endregion
}
