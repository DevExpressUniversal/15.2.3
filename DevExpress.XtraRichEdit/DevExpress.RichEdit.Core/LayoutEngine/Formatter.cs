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
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Services;
using DevExpress.XtraRichEdit.Layout.TableLayout;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.Office.Drawing;
using DevExpress.Office.API.Internal;
using DevExpress.Office.Layout;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using DevExpress.XtraRichEdit.LayoutEngine;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Compatibility.System.Drawing;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Layout.Engine {
	#region FormattingProcess
	public enum FormattingProcess {
		ContinueFromParagraph,
		Continue,
		RestartFromTheStartOfRow,
		Finish
	}
	#endregion
	public class FormattingProcessResult {
		FormattingProcess formattingProcess;
		ParagraphIndex paragraphIndex;
		DocumentModelPosition restartPosition;
		bool forceRestart;
		public FormattingProcessResult(FormattingProcess formattingProcess) {
			this.formattingProcess = formattingProcess;
		}
		public FormattingProcessResult(ParagraphIndex paragraphIndex) {
			this.formattingProcess = FormattingProcess.ContinueFromParagraph;
			this.paragraphIndex = paragraphIndex;
		}
		public FormattingProcessResult(DocumentModelPosition restartPosition)
			: this(restartPosition, false) {
		}
		public FormattingProcessResult(DocumentModelPosition restartPosition, bool forceRestart) {
			this.formattingProcess = FormattingProcess.RestartFromTheStartOfRow;
			this.restartPosition = restartPosition;
			this.forceRestart = forceRestart;
		}
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
		public FormattingProcess FormattingProcess { get { return formattingProcess; } }
		public DocumentModelPosition RestartPosition { get { return restartPosition; } }
		public bool ForceRestart { get { return forceRestart; } }
	}
	#region AddBoxResult
	public enum AddBoxResult {
		Success,
		HorizontalIntersect,
		LeaveColumnPlain,
		LeaveColumnFirstCellRow,
		IntersectWithFloatObject,
		RestartDueFloatingObject,
	}
	#endregion
	#region CharacterFormatterState
	public enum CharacterFormatterState {
		Start,
		Spaces,
		Tab,
		FirstDash,
		Dash,
		Separator,
		LineBreak,
		PageBreak,
		ColumnBreak,
		Text,
		InlineObject,
		FloatingObject,
		LayoutDependentText,
		Final
	}
	#endregion
	public abstract class FormatterStateBase<TIterator, TStateEnum>
		where TIterator : ParagraphIteratorBase {
		protected const int maxTextLength = 3000;
		public abstract TStateEnum Type { get; }
		public abstract BoxMeasurer Measurer { get; }
		public abstract bool FormattingComplete { get; }
		public abstract TIterator Iterator { get; }
		protected abstract bool IsTerminatorChar(char ch);
		protected abstract StateContinueFormatResult AppendBox(BoxInfo boxInfo);
		protected abstract void MeasureBoxContent(BoxInfo boxInfo);
		protected abstract CompleteFormattingResult FinishParagraph();
		protected abstract CompleteFormattingResult FinishSection();
		protected abstract void ChangeState(TStateEnum type);
		protected abstract void FinishParagraphCore(TIterator iterator);
		protected virtual StateContinueFormatResult ContinueFormatByCharacter(BoxInfo boxInfo, LayoutDependentTextBox layoutDependentTextBox) {
			TIterator iterator = Iterator;
			for (; ; ) {
				boxInfo.IteratorResult = iterator.Next();
				Debug.Assert(layoutDependentTextBox == null || boxInfo.IteratorResult != ParagraphIteratorResult.Success);
				boxInfo.Box = layoutDependentTextBox;
				if (boxInfo.IteratorResult != ParagraphIteratorResult.Success) {
					boxInfo.EndPos = iterator.GetPreviousPosition();
					StateContinueFormatResult appendBoxResult = AppendBox(boxInfo);
					if (appendBoxResult != StateContinueFormatResult.Succes)
						return appendBoxResult;
					FinishParagraphCore(iterator);
					return StateContinueFormatResult.Succes;
				}
				else {
					bool terminatorChar = IsTerminatorChar(iterator.CurrentChar);
					if (terminatorChar || MaxTextLengthExceeded(boxInfo)) {
						if (!terminatorChar)
							boxInfo.IteratorResult = ParagraphIteratorResult.RunFinished;
						boxInfo.EndPos = iterator.GetPreviousPosition();
						return AppendBox(boxInfo);
					}
				}
			}
		}
		protected virtual bool MaxTextLengthExceeded(BoxInfo box) {
			return false;
		}
	}
	#region CharacterFormatterStateBase (abstract)
	public abstract class CharacterFormatterStateBase : FormatterStateBase<ParagraphCharacterIterator, CharacterFormatterState> {
		readonly ParagraphCharacterFormatter formatter;
		protected CharacterFormatterStateBase(ParagraphCharacterFormatter formatter) {
			Guard.ArgumentNotNull(formatter, "formatter");
			this.formatter = formatter;
		}
		#region Properties
		public override ParagraphCharacterIterator Iterator { get { return Formatter.Iterator; } }
		public virtual Paragraph Paragraph { get { return Iterator.Paragraph; } }
		public override bool FormattingComplete { get { return false; } }
		protected ParagraphCharacterFormatter Formatter { get { return formatter; } }
		public override BoxMeasurer Measurer { get { return Formatter.Measurer; } }
		public virtual CharacterFormatterState DashState { get { return CharacterFormatterState.FirstDash; } }
		#endregion
		public virtual bool ContinueFormat() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			return ContinueFormatByCharacter(boxInfo, null) == StateContinueFormatResult.Succes;
		}
		protected override void FinishParagraphCore(ParagraphCharacterIterator iterator) {
		}
		protected override CompleteFormattingResult FinishParagraph() {
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			AddBox(ParagraphCharacterFormatter.paragraphBox, boxInfo, true);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			AddBox(ParagraphCharacterFormatter.sectionBox, boxInfo, true);
			return CompleteFormattingResult.Success;
		}
		protected void AppendBoxCore(Box type, BoxInfo boxInfo) {
			bool measured = false;
			if (type != ParagraphCharacterFormatter.textBox) {
				if (type != ParagraphCharacterFormatter.layoutDependentTextBox)
					MeasureBoxContent(boxInfo);
				measured = true;
			}
			AddBox(type, boxInfo, measured);
			SwitchToNextState();
		}
		protected void SwitchToNextState() {
			CharacterFormatterState nextState = GetNextState();
			ChangeState(nextState);
		}
		void AddBox(Box type, BoxInfo boxInfo, bool measured) {
			Box box = type.CreateBox();
			box.StartPos = boxInfo.StartPos;
			box.EndPos = boxInfo.EndPos;
			if (measured)
				box.Bounds = new Rectangle(new Point(0, 0), boxInfo.Size);
			else
				Formatter.AddTextBoxToQueue(box, boxInfo);
			Paragraph.BoxCollection.Add(box);
		}
		BoxInfo CreateParagraphMarkBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos; 
			Measurer.MeasureParagraphMark(boxInfo);
			return boxInfo;
		}
		BoxInfo CreateSectionMarkBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos; 
			Measurer.MeasureSectionMark(boxInfo);
			return boxInfo;
		}
		protected override void ChangeState(CharacterFormatterState stateType) {
			Formatter.ChangeState(stateType);
		}
		protected virtual CharacterFormatterState GetNextState() {
			TextRunBase currentRun = Formatter.PieceTable.Runs[Iterator.RunIndex];
			if (IsInlineObjectRun(currentRun))
				return CharacterFormatterState.InlineObject;
			if (IsSeparatorRun(currentRun))
				return CharacterFormatterState.Separator;
			if (IsFloatingObjectRun(currentRun))
				return CharacterFormatterState.FloatingObject;
			if (IsLayoutDependentTextRun(currentRun))
				return CharacterFormatterState.LayoutDependentText;
			switch (Iterator.CurrentChar) {
				case Characters.ParagraphMark:
					if (IsParagraphMarkRun(currentRun)) {
						FinishParagraph();
						return CharacterFormatterState.Final;
					}
					else
						return CharacterFormatterState.Text;
				case Characters.SectionMark:
					if (IsParagraphMarkRun(currentRun)) {
						FinishSection();
						return CharacterFormatterState.Final;
					}
					else
						return CharacterFormatterState.Text;
				case Characters.Space:
				case Characters.EmSpace:
				case Characters.EnSpace:
					return CharacterFormatterState.Spaces;
				case Characters.Dash:
				case Characters.EmDash:
				case Characters.EnDash:
					return DashState;
				case Characters.TabMark:
					return CharacterFormatterState.Tab;
				case Characters.LineBreak:
					return CharacterFormatterState.LineBreak;
				case Characters.PageBreak:
					if (Paragraph.IsInCell())
						return CharacterFormatterState.Spaces;
					else
						return CharacterFormatterState.PageBreak;
				case Characters.ColumnBreak:
					if (Paragraph.IsInCell())
						return CharacterFormatterState.Spaces;
					else
						return CharacterFormatterState.ColumnBreak;
				default:
					return CharacterFormatterState.Text;
			}
		}
		bool IsParagraphMarkRun(TextRunBase run) {
			return run is ParagraphRun;
		}
		bool IsInlineObjectRun(TextRunBase run) {
			return run is IInlineObjectRun;
		}
		bool IsSeparatorRun(TextRunBase run) {
			return run is SeparatorTextRun;
		}
		bool IsFloatingObjectRun(TextRunBase run) {
			return run is FloatingObjectAnchorRun;
		}
		bool IsLayoutDependentTextRun(TextRunBase run) {
			return run is LayoutDependentTextRun;
		}
	}
	#endregion
	#region CharacterFormatterStartState
	public class CharacterFormatterStartState : CharacterFormatterStateBase {
		public CharacterFormatterStartState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Start; } }
		public override bool ContinueFormat() {
			SwitchToNextState();
			return true;
		}
		protected override bool IsTerminatorChar(char ch) {
			Debug.Assert(false);
			return true;
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			Debug.Assert(false);
			return StateContinueFormatResult.Succes;
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Debug.Assert(false);
		}
	}
	#endregion
	#region CharacterFormatterSpacesState
	public class CharacterFormatterSpacesState : CharacterFormatterStateBase {
		public CharacterFormatterSpacesState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Spaces; } }
		protected override bool IsTerminatorChar(char ch) {
			return !FormatterHelper.IsCharSpace(ch);
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureSpaces(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.GetSpaceBoxTemplate(boxInfo), boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterTextState
	public class CharacterFormatterTextState : CharacterFormatterStateBase {
		public CharacterFormatterTextState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Text; } }
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak || FormatterHelper.IsCharDash(ch);
		}
		protected override bool MaxTextLengthExceeded(BoxInfo box) {
			return Iterator.Offset - box.StartPos.Offset > maxTextLength;
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureText(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			if (Paragraph.PieceTable.Runs[boxInfo.StartPos.RunIndex] is SpecialTextRun)
				AppendBoxCore(ParagraphCharacterFormatter.specialTextBox, boxInfo);
			else
				AppendBoxCore(ParagraphCharacterFormatter.textBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterDashState
	public class CharacterFormatterDashState : CharacterFormatterStateBase {
		public CharacterFormatterDashState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Dash; } }
		protected override bool IsTerminatorChar(char ch) {
			return !FormatterHelper.IsCharDash(ch);
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureText(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.dashBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterFirstDashState
	public class CharacterFormatterFirstDashState : CharacterFormatterDashState {
		public CharacterFormatterFirstDashState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState DashState { get { return CharacterFormatterState.Dash; } }
		public override CharacterFormatterState Type { get { return CharacterFormatterState.FirstDash; } }
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
	}
	#endregion
	#region CharacterFormatterLayoutDependentTextState
	public class CharacterFormatterLayoutDependentTextState : CharacterFormatterStateBase {
		public CharacterFormatterLayoutDependentTextState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.LayoutDependentText; } }
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak;
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureText(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.layoutDependentTextBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterInlinePictureState
	public class CharacterFormatterInlineObjectState : CharacterFormatterStateBase {
		public CharacterFormatterInlineObjectState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.InlineObject; } }
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak || FormatterHelper.IsCharDash(ch);
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			TextRunBase run = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex];
			Debug.Assert(run is IInlineObjectRun);
			run.Measure(boxInfo, Measurer);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			IInlineObjectRun inlineObjectRun = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex] as IInlineObjectRun;
			Debug.Assert(inlineObjectRun != null);
			AppendBoxCore(inlineObjectRun.CreateBox(), boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterSeparatorState
	public class CharacterFormatterSeparatorState : CharacterFormatterStateBase {
		public CharacterFormatterSeparatorState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Separator; } }
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak || FormatterHelper.IsCharDash(ch);
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			TextRunBase run = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex];
			Debug.Assert(run is SeparatorTextRun);
			run.Measure(boxInfo, Measurer);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
#if DEBUG
			SeparatorTextRun separatorRun = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex] as SeparatorTextRun;
			Debug.Assert(separatorRun != null);
#endif
			AppendBoxCore(ParagraphCharacterFormatter.separatorBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterFloatingObjectState
	public class CharacterFormatterFloatingObjectState : CharacterFormatterStateBase {
		public CharacterFormatterFloatingObjectState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.InlineObject; } }
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak || FormatterHelper.IsCharDash(ch);
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
#if DEBUG
			FloatingObjectAnchorRun run = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex] as FloatingObjectAnchorRun;
			Debug.Assert(run != null);
#endif
			boxInfo.Size = Size.Empty;
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.floatingObjectBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region CharacterFormatterTabState
	public class CharacterFormatterTabState : CharacterFormatterStateBase {
		public CharacterFormatterTabState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Tab; } }
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureTab(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.tabSpaceBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
	}
	#endregion
	#region CharacterFormatterLineBreak
	public class CharacterFormatterLineBreak : CharacterFormatterStateBase {
		public CharacterFormatterLineBreak(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.LineBreak; } }
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureLineBreakMark(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.lineBreakBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
	}
	#endregion
	#region CharacterFormatterPageBreak
	public class CharacterFormatterPageBreak : CharacterFormatterStateBase {
		public CharacterFormatterPageBreak(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.PageBreak; } }
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasurePageBreakMark(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.pageBreakBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
	}
	#endregion
	#region CharacterFormatterColumnBreak
	public class CharacterFormatterColumnBreak : CharacterFormatterStateBase {
		public CharacterFormatterColumnBreak(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.ColumnBreak; } }
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Measurer.MeasureColumnBreakMark(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			AppendBoxCore(ParagraphCharacterFormatter.columnBreakBox, boxInfo);
			return StateContinueFormatResult.Succes;
		}
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
	}
	#endregion
	#region CharacterFormatterFinalState
	public class CharacterFormatterFinalState : CharacterFormatterStateBase {
		public CharacterFormatterFinalState(ParagraphCharacterFormatter formatter)
			: base(formatter) {
		}
		public override CharacterFormatterState Type { get { return CharacterFormatterState.Final; } }
		public override bool FormattingComplete { get { return true; } }
		public override bool ContinueFormat() {
			Debug.Assert(false);
			return true;
		}
		protected override bool IsTerminatorChar(char ch) {
			Debug.Assert(false);
			return true;
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			Debug.Assert(false);
			return StateContinueFormatResult.Succes;
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Debug.Assert(false);
		}
	}
	#endregion
	#region ParagraphFormatterBase (abstract class)
	public abstract class ParagraphFormatterBase<TIterator, TState, TStateEnum>
		where TState : FormatterStateBase<TIterator, TStateEnum>
		where TIterator : ParagraphIteratorBase {
		internal static readonly TextBox textBox = new TextBox();
		internal static readonly SpecialTextBox specialTextBox = new SpecialTextBox();
		internal static readonly LayoutDependentTextBox layoutDependentTextBox = new LayoutDependentTextBox();
		internal static readonly InlinePictureBox inlinePictureBox = new InlinePictureBox();
		internal static readonly FloatingObjectAnchorBox floatingObjectBox = new FloatingObjectAnchorBox();
		internal static readonly SpaceBoxa spaceBox = new SpaceBoxa();
		internal static readonly SingleSpaceBox singleSpaceBox = new SingleSpaceBox();
		internal static readonly ParagraphMarkBox paragraphBox = new ParagraphMarkBox();
		internal static readonly SectionMarkBox sectionBox = new SectionMarkBox();
		internal static readonly LineBreakBox lineBreakBox = new LineBreakBox();
		internal static readonly PageBreakBox pageBreakBox = new PageBreakBox();
		internal static readonly ColumnBreakBox columnBreakBox = new ColumnBreakBox();
		internal static readonly HyphenBox hyphenBox = new HyphenBox();
		internal static readonly TabSpaceBox tabSpaceBox = new TabSpaceBox();
		internal static readonly SeparatorBox separatorBox = new SeparatorBox();
		internal static readonly NumberingListBox numberingListBox = new NumberingListBox();
		internal static readonly DashBox dashBox = new DashBox();
		readonly PieceTable pieceTable;
		BoxMeasurer measurer;
		TIterator iterator;
		TState state;
		protected ParagraphFormatterBase(PieceTable pieceTable, BoxMeasurer measurer) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			Guard.ArgumentNotNull(measurer, "measurer");
			this.pieceTable = pieceTable;
			this.measurer = measurer;
		}
		#region Properties
		public PieceTable PieceTable { get { return pieceTable; } }
		public DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		public BoxMeasurer Measurer { get { return measurer; } }
		public TIterator Iterator { get { return iterator; } set { iterator = value; } }
		public TState State { get { return state; } set { state = value; } }
		#endregion
		protected internal virtual void OnNewMeasurementAndDrawingStrategyChanged(BoxMeasurer measurer) {
			Guard.ArgumentNotNull(measurer, "measurer");
			this.measurer = measurer;
		}
		protected abstract void BeginParagraph(bool beginFromParagraphStart);
		protected internal abstract TStateEnum GetInitialStateType(bool beginFromParagraphStart);
		protected internal static Box GetSpaceBoxTemplate(BoxInfo boxInfo) {
			if (boxInfo.StartPos == boxInfo.EndPos)
				return singleSpaceBox;
			else
				return spaceBox;
		}
	}
	#endregion
	#region ParagraphCharacterFormatter
	public class ParagraphCharacterFormatter : ParagraphFormatterBase<ParagraphCharacterIterator, CharacterFormatterStateBase, CharacterFormatterState> {
		List<BoxInfo> textBoxInfos;
		List<Box> textBoxes;
		public ParagraphCharacterFormatter(PieceTable pieceTable, BoxMeasurer measurer)
			: base(pieceTable, measurer) {
		}
		protected internal override CharacterFormatterState GetInitialStateType(bool beginFromParagraphStart) {
			return CharacterFormatterState.Start;
		}
		public virtual void ChangeState(CharacterFormatterState stateType) {
			switch (stateType) {
				case CharacterFormatterState.Start:
					State = new CharacterFormatterStartState(this);
					break;
				case CharacterFormatterState.Text:
					State = new CharacterFormatterTextState(this);
					break;
				case CharacterFormatterState.Dash:
					State = new CharacterFormatterDashState(this);
					break;
				case CharacterFormatterState.FirstDash:
					State = new CharacterFormatterFirstDashState(this);
					break;
				case CharacterFormatterState.LayoutDependentText:
					State = new CharacterFormatterLayoutDependentTextState(this);
					break;
				case CharacterFormatterState.InlineObject:
					State = new CharacterFormatterInlineObjectState(this);
					break;
				case CharacterFormatterState.Separator:
					State = new CharacterFormatterSeparatorState(this);
					break;
				case CharacterFormatterState.FloatingObject:
					State = new CharacterFormatterFloatingObjectState(this);
					break;
				case CharacterFormatterState.Spaces:
					State = new CharacterFormatterSpacesState(this);
					break;
				case CharacterFormatterState.Tab:
					State = new CharacterFormatterTabState(this);
					break;
				case CharacterFormatterState.LineBreak:
					State = new CharacterFormatterLineBreak(this);
					break;
				case CharacterFormatterState.PageBreak:
					State = new CharacterFormatterPageBreak(this);
					break;
				case CharacterFormatterState.ColumnBreak:
					State = new CharacterFormatterColumnBreak(this);
					break;
				case CharacterFormatterState.Final:
					State = new CharacterFormatterFinalState(this);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		protected override void BeginParagraph(bool beginFromParagraphStart) {
			Paragraph paragraph = Iterator.Paragraph;
			ParagraphBoxCollection boxCollection = paragraph.BoxCollection;
			boxCollection.Clear();
			if (paragraph.IsInList())
				FormatNumberingListBoxes();
		}
		protected virtual void FormatNumberingListBoxes() {
			Paragraph paragraph = Iterator.Paragraph;
			BoxInfo boxInfo = new NumberingListBoxInfo();
			NumberingList numberingList = DocumentModel.NumberingLists[paragraph.GetNumberingListIndex()];
			FormatterPosition position = new FormatterPosition(paragraph.FirstRunIndex, 0, 0);
			boxInfo.StartPos = position;
			boxInfo.EndPos = position;
			string numberingListText = paragraph.GetNumberingListText();
			Measurer.MeasureText(boxInfo, numberingListText, paragraph.GetNumerationFontInfo());
			int listLevelIndex = paragraph.GetListLevelIndex();
			IListLevel listLevel = numberingList.Levels[listLevelIndex];
			NumberingListBox numberingListBox = CreateNumberingListBox(listLevel.ListLevelProperties.Separator, position);
			numberingListBox.NumberingListText = numberingListText;
			numberingListBox.StartPos = position;
			numberingListBox.EndPos = position; 
			numberingListBox.InitialBounds = new Rectangle(Point.Empty, boxInfo.Size);
			paragraph.BoxCollection.NumberingListBox = numberingListBox;
		}
		private NumberingListBox CreateNumberingListBox(char separatorChar, FormatterPosition position) {
			if (separatorChar == '\u0000')
				return new NumberingListBox();
			NumberingListBoxWithSeparator result = new NumberingListBoxWithSeparator();
			Box separatorBox = CreateSeparatorBox(separatorChar, position);
			separatorBox.StartPos = position; 
			separatorBox.EndPos = position; 
			result.SeparatorBox = separatorBox;
			return result;
		}
		Box CreateSeparatorBox(char separator, FormatterPosition position) {
			if (separator == Characters.TabMark) {
				TabSpaceBox result = new TabSpaceBox();
				result.Bounds = Rectangle.Empty;
				return result;
			}
			else {
				Debug.Assert(FormatterHelper.IsCharSpace(separator));
				BoxInfo boxInfo = new BoxInfo();
				boxInfo.StartPos = position;
				boxInfo.EndPos = position;
				Measurer.MeasureSingleSpace(boxInfo);
				SingleSpaceBox result = new SingleSpaceBox();
				result.Bounds = new Rectangle(Point.Empty, boxInfo.Size);
				return result;
			}
		}
		public virtual void Format(ParagraphCharacterIterator iterator) {
			this.textBoxInfos = new List<BoxInfo>();
			this.textBoxes = new List<Box>();
			FormatParagraph(iterator);
			MeasureTextBoxes();
		}
		protected internal virtual void FormatParagraph(ParagraphCharacterIterator iterator) {
			if (iterator == null)
				Exceptions.ThrowArgumentException("iterator", iterator);
			Iterator = iterator;
			BeginParagraph(true);
			ChangeState(GetInitialStateType(true));
			do {
				State.ContinueFormat();
			}
			while (!State.FormattingComplete);
		}
		public void AddTextBoxToQueue(Box box, BoxInfo boxInfo) {
#if DEBUG
			Debug.Assert(PieceTable.Runs[boxInfo.StartPos.RunIndex] is TextRun);
			Debug.Assert(box is TextBox);
#endif
			textBoxInfos.Add(boxInfo);
			textBoxes.Add(box);
		}
		void MeasureTextBoxes() {
			int count = textBoxInfos.Count;
			Debug.Assert(count == textBoxes.Count);
			TextRunCollection runs = PieceTable.Runs;
			Measurer.BeginTextMeasure();
			try {
				for (int i = 0; i < count; i++) {
					BoxInfo boxInfo = textBoxInfos[i];
					TextRunBase run = runs[boxInfo.StartPos.RunIndex];
					run.Measure(boxInfo, Measurer);
				}
			}
			finally {
				Measurer.EndTextMeasure();
			}
			for (int i = 0; i < count; i++) {
				BoxInfo boxInfo = textBoxInfos[i];
				textBoxes[i].Bounds = new Rectangle(0, 0, boxInfo.Size.Width, boxInfo.Size.Height);
			}
		}
	}
	#endregion
	public enum StateContinueFormatResult {
		Succes,
		RestartDueFloatingObject,
		RestartDueOrphanedObjects
	}
	#region ParagraphBoxFormatter
	public class ParagraphBoxFormatter : ParagraphFormatterBase<ParagraphBoxIterator, BoxFormatterStateBase, FormatterState>, IDisposable {
		#region Fields
		static readonly FormatterPosition InvalidPosition = new FormatterPosition(new RunIndex(-1), -1, -1);
		readonly IHyphenationService hyphenationService;
		SyllableBoxIterator syllableIterator;
		RowsController rowsController;
		int paragraphStartRowCount;
		FormatterPosition paragraphStartPos;
		FormatterPosition lastRestartDueToFloatingObjectParagraphStartPos;
		FormatterPosition rowStartPos;
		FormatterPosition wordStartPos;
		FormatterPosition lastTabStartPos;
		FormatterPosition unapprovedFloatingObjectsStartPos;
		bool hasUnapprovedFloatingObjects;
		bool suppressHyphenation;
		Page pageNumberSource;
		int maxHeight = int.MaxValue;
		bool forceFormattingComplete;
		Stack<ParagraphBoxFormatterTableStateBase> tableStates;
		bool hasDeferredNumberingListBox;
		#endregion
		public ParagraphBoxFormatter(PieceTable pieceTable, BoxMeasurer measurer, RowsController rowsController)
			: base(pieceTable, measurer) {
			if (PieceTable.DocumentModel.DocumentProperties.HyphenateDocument)
				this.hyphenationService = DocumentModel.GetService<IHyphenationService>();
			Initialize(rowsController);
		}
		#region Properties
		public ParagraphBoxIterator SyllableIterator { get { return syllableIterator; } }
		public Row CurrentRow { get { return RowsController.CurrentRow; } }
		public FormatterPosition ParagraphStartPos { get { return paragraphStartPos; } }
		public FormatterPosition RowStartPos { get { return rowStartPos; } }
		public FormatterPosition WordStartPos { get { return wordStartPos; } }
		internal RowsController RowsController { get { return rowsController; } }
		internal bool SuppressHyphenation { get { return suppressHyphenation; } }
		internal int MaxHeight { get { return maxHeight; } set { maxHeight = value; } }
		internal bool ForceFormattingComplete { get { return forceFormattingComplete; } set { forceFormattingComplete = value; } }
		internal Page PageNumberSource { get { return pageNumberSource; } set { pageNumberSource = value; } }
		protected internal bool HasDeferredNumberingListBox { get { return hasDeferredNumberingListBox; } set { hasDeferredNumberingListBox = value; } }
		public MergedFrameProperties ActualParagraphFrameProperties { get { return GetActualParagraphFrameProperties(Iterator.Paragraph); } }
		#endregion
		public MergedFrameProperties GetActualParagraphFrameProperties(Paragraph paragraph) {
			MergedFrameProperties result = paragraph.GetMergedFrameProperties();
			if (result != null && !paragraph.IsInCell())
				return result;
			else
				return null;
		}
		public void ChangeStateContinueFromParagraph(ParagraphIndex paragraphIndex) {
			State = new StateContinueFormattingFromParagraph(this, paragraphIndex);
		}
		#region ChangeState(FormatterState stateType)
		public virtual void ChangeState(FormatterState stateType) {
#if DEBUGTEST
#endif
			switch (stateType) {
				case FormatterState.RowEmpty:
					StartNewRow();
					State = new StateRowEmpty(this);
					break;
				case FormatterState.RowEmptyAfterFloatingObject:
					State = new StateRowEmptyAfterFloatingObject(this);
					break;
				case FormatterState.ParagraphStart:
					StartNewRow();
					State = new StateParagraphStart(this);
					break;
				case FormatterState.ParagraphStartAfterFloatingObject:
					State = new StateParagraphStartAfterFloatingObject(this);
					break;
				case FormatterState.ParagraphStartFromTheMiddle:
					StartNewRow();
					State = new StateParagraphStartFromTheMiddle(this);
					break;
				case FormatterState.ParagraphStartFromTheMiddleAfterFloatingObject:
					State = new StateParagraphStartFromTheMiddleAfterFloatingObject(this);
					break;
				case FormatterState.RowWithSpacesOnly:
					State = new StateRowWithSpacesOnly(this);
					break;
				case FormatterState.RowWithTextOnly:
					State = new StateRowWithTextOnly(this);
					StartNewWord();
					break;
				case FormatterState.RowWithInlineObjectOnly:
					State = new StateRowWithInlineObjectOnly(this);
					StartNewWord();
					break;
				case FormatterState.RowWithTextOnlyAfterFloatingObject:
					State = new StateRowWithTextOnly(this);
					break;
				case FormatterState.RowWithDashOnly:
					StartNewWord();
					State = new StateRowWithDashOnly(this);
					break;
				case FormatterState.RowWithDashAfterTextOnly:
					StartNewWord();
					State = new StateRowWithDashAfterTextOnly(this);
					break;
				case FormatterState.RowWithDashOnlyAfterFloatingObject:
					State = new StateRowWithDashOnly(this);
					break;
				case FormatterState.RowWithTextOnlyAfterFirstLeadingTab:
					State = new StateRowWithTextOnlyAfterFirstLeadingTab(this);
					StartNewWord();
					break;
				case FormatterState.RowWithTextOnlyAfterFirstLeadingTabAfterFloatingObject:
					State = new StateRowWithTextOnlyAfterFirstLeadingTab(this);
					break;
				case FormatterState.RowFirstLeadingTab:
					State = new StateRowFirstLeadingTab(this);
					break;
				case FormatterState.RowLeadingTab:
					StartNewTab();
					State = new StateRowLeadingTab(this);
					break;
				case FormatterState.RowLeadingTabAfterFloatingObject:
					State = new StateRowLeadingTab(this);
					break;
				case FormatterState.RowTab:
					StartNewTab();
					State = new StateRowTab(this);
					break;
				case FormatterState.RowTabAfterFloatingObject:
					State = new StateRowTab(this);
					break;
				case FormatterState.RowSpaces:
					ResetLastTabPosition();
					State = new StateRowSpaces(this);
					break;
				case FormatterState.RowText:
					StartNewWord();
					State = new StateRowText(this);
					break;
				case FormatterState.RowInlineObject:
					StartNewWord();
					State = new StateRowInlineObject(this);
					break;
				case FormatterState.RowTextAfterFloatingObject:
					State = new StateRowText(this);
					break;
				case FormatterState.RowDashAfterText:
					State = new StateRowDashAfterText(this);
					break;
				case FormatterState.RowDash:
					StartNewWord();
					State = new StateRowDash(this);
					break;
				case FormatterState.RowDashAfterFloatingObject:
					State = new StateRowDash(this);
					break;
				case FormatterState.RowTextSplit:
					StartNewRow();
					State = new StateRowTextSplit(this);
					break;
				case FormatterState.RowTextSplitAfterFloatingObject:
					State = new StateRowTextSplitAfterFloatingObject(this);
					break;
				case FormatterState.RowTextSplitAfterFirstLeadingTab:
					State = new StateRowTextSplitAfterFirstLeadingTab(this);
					StartNewWord();
					break;
				case FormatterState.RowTextSplitAfterFirstLeadingTabAfterFloatingObject:
					State = new StateRowTextSplitAfterFirstLeadingTab(this);
					break;
				case FormatterState.RowDashSplit:
					StartNewRow();
					State = new StateRowDashSplit(this);
					break;
				case FormatterState.RowDashSplitAfterFloatingObject:
					State = new StateRowDashSplitAfterFloatingObject(this);
					break;
				case FormatterState.RowEmptyHyphenation:
					UpdateSyllableIterator();
					StartNewRow();
					State = new StateRowEmptyHyphenation(this);
					break;
				case FormatterState.RowEmptyHyphenationAfterFloatingObject:
					UpdateSyllableIterator();
					State = new StateRowEmptyHyphenationAfterFloatingObject(this);
					break;
				case FormatterState.RowTextHyphenationFirstSyllable:
					UpdateSyllableIterator();
					State = new StateRowTextHyphenationFirstSyllable(this);
					StartNewWord();
					break;
				case FormatterState.RowTextHyphenationFirstSyllableAfterFloatingObject:
					UpdateSyllableIterator();
					State = new StateRowTextHyphenationFirstSyllable(this);
					break;
				case FormatterState.RowTextHyphenationFirstSyllableAfterFirstLeadingTab:
					UpdateSyllableIterator();
					State = new StateRowTextHyphenationFirstSyllableAfterFirstLeadingTab(this);
					StartNewWord();
					break;
				case FormatterState.RowTextHyphenationFirstSyllableAfterFirstLeadingTabAfterFloatingObject:
					UpdateSyllableIterator();
					State = new StateRowTextHyphenationFirstSyllableAfterFirstLeadingTab(this);
					break;
				case FormatterState.RowTextHyphenation:
					UpdateSyllableIterator();
					State = new StateRowTextHyphenation(this);
					StartNewWord();
					break;
				case FormatterState.RowTextHyphenationAfterFloatingObject:
					UpdateSyllableIterator();
					State = new StateRowTextHyphenation(this);
					break;
				case FormatterState.RowLineBreak:
					State = new StateRowLineBreak(this, State);
					break;
				case FormatterState.RowPageBreak:
					State = new StateRowPageBreak(this, State);
					break;
				case FormatterState.RowColumnBreak:
					State = new StateRowColumnBreak(this, State);
					break;
				case FormatterState.SectionBreakAfterParagraphMark:
					State = new StateSectionBreakAfterParagraphMark(this);
					break;
				case FormatterState.FloatingObject:
					StartFloatingObjects();
					State = new StateFloatingObject(this, State);
					break;
				case FormatterState.Final:
					State = new StateFinal(this);
					break;
				default:
					Exceptions.ThrowInternalException();
					break;
			}
		}
		#endregion
		protected internal override FormatterState GetInitialStateType(bool beginFromParagraphStart) {
			TextRunCollection runs = PieceTable.Runs;
			if (!(runs[Iterator.RunIndex] is SectionRun))
				return beginFromParagraphStart ? FormatterState.ParagraphStart : FormatterState.ParagraphStartFromTheMiddle;
			if (runs[Iterator.RunIndex] != runs.First) {
				if (runs[Iterator.RunIndex - 1].GetType() == typeof(ParagraphRun)) {
					Paragraph prevParagraph = runs[Iterator.RunIndex - 1].Paragraph;
					if (!prevParagraph.IsInCell() && GetActualParagraphFrameProperties(prevParagraph) == null)
						return FormatterState.SectionBreakAfterParagraphMark;
				}
			}
			return beginFromParagraphStart ? FormatterState.ParagraphStart : FormatterState.ParagraphStartFromTheMiddle;
		}
		protected virtual void Initialize(RowsController rowsController) {
			if (rowsController == null)
				Exceptions.ThrowArgumentException("rowsController", rowsController);
			this.rowsController = rowsController;
			this.tableStates = new Stack<ParagraphBoxFormatterTableStateBase>();
			this.tableStates.Push(new ParagraphBoxFormatterTextState(this));
			SubscribeRowsControllerEvents();
		}
		protected virtual void SubscribeRowsControllerEvents() {
			this.rowsController.TableStarted += new EventHandler(OnTableStart);
		}
		protected virtual void UnsubscribeRowsControllerEvents() {
			this.rowsController.TableStarted -= new EventHandler(OnTableStart);
		}
		void OnTableStart(object sender, EventArgs e) {
			this.tableStates.Push(new ParagraphBoxFormatterTableState(this));
		}
		void OnTableEnd(object sender, EventArgs e) {
			tableStates.Pop();
		}
		protected internal ParagraphBoxIterator GetActiveIterator() {
			if (State != null) {
				ParagraphBoxIterator iter = State.Iterator;
				if (iter != null)
					return iter;
			}
			return Iterator;
		}
		public void UpdateSyllableIterator() {
			if (syllableIterator == null)
				syllableIterator = new SyllableBoxIterator(Iterator, hyphenationService);
		}
		public void ClearSyllableIterator() {
			syllableIterator = null;
		}
		public void BeginParagraphFormatting(ParagraphBoxIterator iterator, bool beginFromParagraphStart) {
			Iterator = iterator;
			HasDeferredNumberingListBox = false;
			BeginParagraph(beginFromParagraphStart);
			ChangeState(GetInitialStateType(beginFromParagraphStart));
		}
		public FormattingProcessResult FormatNextRow() {
			for (; ; ) {
				StateContinueFormatResult continueFormatResult = State.ContinueFormat();
				if (continueFormatResult != StateContinueFormatResult.Succes) {
					if (continueFormatResult == StateContinueFormatResult.RestartDueFloatingObject) {
						lastRestartDueToFloatingObjectParagraphStartPos = paragraphStartPos;
						DocumentModelPosition restartPosition = RowsController.RestartModelPosition;
						Debug.Assert(restartPosition != null);
						RowsController.RestartModelPosition = null;
						hasUnapprovedFloatingObjects = false;
						return new FormattingProcessResult(restartPosition);
					}
					else {
						RowsController.ColumnController.PageAreaController.PageController.SetPageLastRunIndex(lastRestartDueToFloatingObjectParagraphStartPos.RunIndex - 1);
						Debug.Assert(RowsController.LastRestartDueToFloatingObjectModelPosition != null);
						RowsController.RestartModelPosition = null;
						return new FormattingProcessResult(RowsController.LastRestartDueToFloatingObjectModelPosition, true);
					}
				}
				switch (State.Type) {
					case FormatterState.RowEmpty:
						return new FormattingProcessResult(FormattingProcess.Continue);
					case FormatterState.RowEmptyHyphenation:
						return new FormattingProcessResult(FormattingProcess.Continue);
					case FormatterState.Final:
						return new FormattingProcessResult(FormattingProcess.Finish);
					case FormatterState.ContinueFromParagraph:
						return new FormattingProcessResult(((StateContinueFormattingFromParagraph)State).ParagraphIndex);
				}
			}
		}
		public void EndParagraphFormatting() {
			EndParagraph();
		}
		protected override void BeginParagraph(bool beginFromParagraphStart) {
			Paragraph paragraph = Iterator.Paragraph;
			if (beginFromParagraphStart)
				this.paragraphStartPos = new FormatterPosition(Iterator.RunIndex, 0, 0);
			else
				this.paragraphStartPos = new FormatterPosition(PieceTable.VisibleTextFilter.FindVisibleRunForward(paragraph.FirstRunIndex), 0, 0);
			this.paragraphStartRowCount = RowsController.CurrentColumn.Rows.Count;
			bool shouldAddPageBreak = beginFromParagraphStart && ShouldPageBreakBeforeParagraph(paragraph) && paragraphStartRowCount > 0 && PieceTable.IsMain && CanBreakPageBefore(paragraph);
			if (shouldAddPageBreak)
				RowsController.AddPageBreakBeforeRow();
			RowsController.BeginParagraph(paragraph, beginFromParagraphStart);
			suppressHyphenation = (hyphenationService == null || paragraph.SuppressHyphenation || !DocumentModel.DocumentProperties.HyphenateDocument);
			if (shouldAddPageBreak) {
				RowsController.OnPageBreakBeforeParagraph();
			}
		}
		bool CanBreakPageBefore(Paragraph paragraph) {
			if (!RowsController.TablesController.IsInsideTable)
				return true;
			TableCell cell = paragraph.GetCell();
			if (cell == null)
				return true;
			while (cell.Table.ParentCell != null)
				cell = cell.Table.ParentCell;
			return cell.IsFirstCellInRow && cell.StartParagraphIndex == paragraph.Index;
		}
		protected virtual bool ShouldPageBreakBeforeParagraph(Paragraph paragraph) {
			bool pageBreakBefore = paragraph.PageBreakBefore;
			if (!pageBreakBefore || paragraph.Length > 1)
				return pageBreakBefore;
			return !(PieceTable.Runs[paragraph.FirstRunIndex] is SectionRun);
		}
		protected internal void EndParagraph() {
			if (Iterator.CurrentChar == Characters.ParagraphMark)
				RowsController.EndParagraph();
			else
				RowsController.EndSection();
		}
		void StartNewRow() {
			rowStartPos = GetActiveIterator().GetCurrentPosition();
			ResetLastTabPosition();
			StartNewWord();
		}
		public void StartNewTab() {
			StartNewWord();
			lastTabStartPos = GetActiveIterator().GetCurrentPosition();
		}
		public void StartNewWord() {
			wordStartPos = GetActiveIterator().GetCurrentPosition();
			RowsController.StartNewWord();
		}
		void RollbackToPositionAndClearLastRow(FormatterPosition pos) {
			ParagraphBoxIterator iter = GetActiveIterator();
			iter.SetPosition(pos);
			RowsController.ClearRow(true);
			ResetLastTabPosition();
		}
		void StartFloatingObjects() {
			if (!hasUnapprovedFloatingObjects) {
				unapprovedFloatingObjectsStartPos = GetActiveIterator().GetCurrentPosition();
				hasUnapprovedFloatingObjects = true;
			}
		}
		CompleteFormattingResult RollbackToParagraphStart(FormatterPosition pos, int initialRowCount) {
			CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
			if (result != CompleteFormattingResult.Success)
				return result;
			RollbackToPositionAndClearLastRow(pos);
			RowCollection rows = RowsController.CurrentColumn.Rows;
			rows.RemoveRange(initialRowCount, rows.Count - initialRowCount);
			RowsController.MoveRowToNextColumn();
			ChangeState(FormatterState.ParagraphStart);
			return result;
		}
		public CompleteFormattingResult RollbackToStartOfRowCore(CanFitCurrentRowToColumnResult canFit, FormatterState nextState) {
			hasUnapprovedFloatingObjects = false;
			if (canFit == CanFitCurrentRowToColumnResult.FirstCellRowNotFitted) {
				RollbackToPositionAndClearLastRow(RowStartPos);
				ParagraphIndex startParagraphIndex = RollbackToStartOfRowTable(canFit);
				ChangeStateContinueFromParagraph(startParagraphIndex);
			}
			else if (canFit == CanFitCurrentRowToColumnResult.TextAreasRecreated) {
				RollbackToPositionAndClearLastRow(RowStartPos);
				ChangeState(nextState);
			}
			else {
				if (paragraphStartRowCount > 0 && RowsController.Paragraph.KeepLinesTogether && PieceTable.IsMain && !RowsController.TablesController.IsInsideTable) {
					CompleteFormattingResult result = RollbackToParagraphStart(ParagraphStartPos, paragraphStartRowCount);
					if (result != CompleteFormattingResult.Success)
						return result;
					paragraphStartRowCount = -1;
				}
				else {
					CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
					if (result != CompleteFormattingResult.Success)
						return result;
					RollbackToPositionAndClearLastRow(RowStartPos);
					RowsController.MoveRowToNextColumn();
					ChangeState(nextState);
				}
			}
			return CompleteFormattingResult.Success;
		}
		ParagraphIndex RollbackToStartOfRowTable(CanFitCurrentRowToColumnResult canFit) {
			return RowsController.TablesController.RollbackToStartOfRowTableOnFirstCellRowColumnOverfull();
		}
		public CompleteFormattingResult RollbackToStartOfRow(CanFitCurrentRowToColumnResult canFit) {
			RunIndex firstVisibleRunIndex = Iterator.VisibleTextFilter.FindVisibleRunForward(Iterator.Paragraph.FirstRunIndex);
			bool rollbackToParagraphStart = RowStartPos.RunIndex == firstVisibleRunIndex && RowStartPos.Offset == 0;
			FormatterState nextState = rollbackToParagraphStart ? FormatterState.ParagraphStart : FormatterState.RowEmpty;
			return RollbackToStartOfRowCore(canFit, nextState);
		}
		void ResetLastTabPosition() {
			lastTabStartPos = InvalidPosition;
		}
		public void RollbackToLastTab() {
			ParagraphBoxIterator iter = GetActiveIterator();
			RollbackToLastTab(iter);
		}
		public void RollbackToLastTab(ParagraphBoxIterator iter) {
			if (lastTabStartPos != InvalidPosition)
				iter.SetPosition(lastTabStartPos);
		}
		public void RollbackToStartOfWord() {
			ParagraphBoxIterator iter = GetActiveIterator();
			RollbackToStartOfWord(iter);
		}
		public void RollbackToStartOfWord(ParagraphBoxIterator iter) {
			iter.SetPosition(WordStartPos);
			RowsController.RemoveLastTextBoxes();
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (rowsController != null) {
					UnsubscribeRowsControllerEvents();
					rowsController = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~ParagraphBoxFormatter() {
			Dispose(false);
		}
		#endregion
		public void ApproveFloatingObjects() {
			hasUnapprovedFloatingObjects = false;
		}
		internal void RollbackUnapprovedFloatingObjects() {
			if (hasUnapprovedFloatingObjects) {
				ParagraphBoxIterator iter = GetActiveIterator();
				iter.SetPosition(unapprovedFloatingObjectsStartPos);
				hasUnapprovedFloatingObjects = false;
			}
		}
	}
	#endregion
	public abstract class ParagraphBoxFormatterTableStateBase {
		readonly ParagraphBoxFormatter formatter;
		protected ParagraphBoxFormatterTableStateBase(ParagraphBoxFormatter formatter) {
			Guard.ArgumentNotNull(formatter, "formatter");
			this.formatter = formatter;
		}
		protected ParagraphBoxFormatter Formatter { get { return formatter; } }
		protected abstract void OnColumnOverfull();
	}
	public class ParagraphBoxFormatterTextState : ParagraphBoxFormatterTableStateBase {
		public ParagraphBoxFormatterTextState(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected override void OnColumnOverfull() {
		}
	}
	public class ParagraphBoxFormatterTableState : ParagraphBoxFormatterTableStateBase {
		public ParagraphBoxFormatterTableState(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected override void OnColumnOverfull() {
		}
	}
	#region FormatterState
	public enum FormatterState {
		ParagraphStart,
		ParagraphStartFromTheMiddle,
		ParagraphStartAfterFloatingObject,
		ParagraphStartFromTheMiddleAfterFloatingObject,
		RowEmpty,
		RowEmptyAfterFloatingObject,
		RowWithSpacesOnly,
		RowWithTextOnly,
		RowWithInlineObjectOnly,
		RowWithTextOnlyAfterFloatingObject,
		RowWithDashOnly,
		RowWithDashOnlyAfterFloatingObject,
		RowWithTextOnlyAfterFirstLeadingTab,
		RowWithTextOnlyAfterFirstLeadingTabAfterFloatingObject,
		RowSpaces,
		RowText,
		RowInlineObject,
		RowTextAfterFloatingObject,
		RowDashAfterText,
		RowDash,
		RowDashAfterFloatingObject,
		RowWithDashAfterTextOnly,
		RowTextSplit,
		RowTextSplitAfterFloatingObject,
		RowTextSplitFloatingObject,
		RowTextSplitAfterFirstLeadingTab,
		RowTextSplitAfterFirstLeadingTabAfterFloatingObject,
		RowDashSplit,
		RowDashSplitAfterFloatingObject,
		RowEmptyHyphenation,
		RowEmptyHyphenationAfterFloatingObject,
		RowTextHyphenationFirstSyllable,
		RowTextHyphenationFirstSyllableAfterFloatingObject,
		RowTextHyphenationFirstSyllableAfterFirstLeadingTab,
		RowTextHyphenationFirstSyllableAfterFirstLeadingTabAfterFloatingObject,
		RowTextHyphenation,
		RowTextHyphenationAfterFloatingObject,
		RowFirstLeadingTab,
		RowLeadingTab,
		RowLeadingTabAfterFloatingObject,
		RowTab,
		RowTabAfterFloatingObject,
		RowLineBreak,
		RowPageBreak,
		RowColumnBreak,
		SectionBreakAfterParagraphMark,
		FloatingObject,
		ParagraphFrame,
		ContinueFromParagraph,
		Final,
	}
	#endregion
	public interface ISupportsChangeStateManually {
		void ChangeStateManuallyIfNeeded(ParagraphIteratorResult iteratorResult);
	}
	#region BoxFormatterStateBase (abstract)
	public abstract class BoxFormatterStateBase : FormatterStateBase<ParagraphBoxIterator, FormatterState> {
		readonly ParagraphBoxFormatter formatter;
		protected BoxFormatterStateBase(ParagraphBoxFormatter formatter) {
			Guard.ArgumentNotNull(formatter, "formatter");
			this.formatter = formatter;
		}
		#region Properties
		public virtual FormatterState StateAfterFloatingObject { get { return Type; } }
		public override ParagraphBoxIterator Iterator { get { return formatter.Iterator; } }
		public Row CurrentRow { get { return formatter.CurrentRow; } }
		public override bool FormattingComplete { get { return ForceFormattingComplete; } }
		public override BoxMeasurer Measurer { get { return Formatter.Measurer; } }
		protected virtual bool CanUseBox { get { return false; } }
		protected bool ForceFormattingComplete { get { return Formatter.ForceFormattingComplete; ; } set { Formatter.ForceFormattingComplete = value; } }
		protected ParagraphBoxFormatter Formatter { get { return formatter; } }
		protected RowsController RowsController { get { return formatter.RowsController; } }
		#endregion
		protected StateContinueFormatResult GetContinueFormatResult(CompleteFormattingResult result) {
			if (result == CompleteFormattingResult.Success)
				return StateContinueFormatResult.Succes;
			else
				return StateContinueFormatResult.RestartDueOrphanedObjects;
		}
		public virtual StateContinueFormatResult ContinueFormat() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			if (CanUseBox && boxInfo.StartPos.AreEqual(Iterator.CurrentBox.StartPos)) {
				StateContinueFormatResult addExistingBoxResult = AddExistedBox(boxInfo, Iterator);
				if (addExistingBoxResult != StateContinueFormatResult.Succes)
					return addExistingBoxResult;
				if (Iterator.IsEnd) {
					if (Iterator.IsParagraphFrame() && this is StateFloatingObject)
						return StateContinueFormatResult.Succes;
					if (formatter.SyllableIterator == null || formatter.SyllableIterator.IsEnd) {
						return GetContinueFormatResult(FinishParagraphOrSecton());
					}
				}
				return StateContinueFormatResult.Succes;
			}
			return ContinueFormatByCharacter(boxInfo, Iterator.CurrentBox as LayoutDependentTextBox);
		}
		protected override bool MaxTextLengthExceeded(BoxInfo box) {
			return Iterator.Offset - box.StartPos.Offset > maxTextLength;
		}
		protected internal virtual CompleteFormattingResult FinishParagraphOrSecton() {
			if (Iterator.CurrentChar == Characters.ParagraphMark)
				return FinishParagraph();
			else
				return FinishSection();
		}
		protected override void FinishParagraphCore(ParagraphBoxIterator iterator) {
			if (iterator.IsEnd) {
				if (formatter.SyllableIterator == null || formatter.SyllableIterator.IsEnd)
					FinishParagraph();
			}
		}
		StateContinueFormatResult AddExistedBox(BoxInfo boxInfo, ParagraphBoxIterator iterator) {
			boxInfo.Box = iterator.CurrentBox;
			iterator.NextBox();
			boxInfo.EndPos = iterator.GetPreviousPosition();
			return AppendBox(boxInfo);
		}
		protected abstract void AddSuccess(BoxInfo boxInfo);
		protected abstract CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo);
		protected abstract CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo);
		protected abstract AddBoxResult CanAddBox(BoxInfo boxInfo);
		protected override CompleteFormattingResult FinishParagraph() {
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			ApplyParagraphMarkSize(boxInfo);
			FinalizeParagraph(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			ApplyParagraphMarkSize(boxInfo);
			FinalizeSection(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected internal virtual CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			FinalizeLine(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected internal virtual CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			return FinalizePage(boxInfo);
		}
		protected internal virtual CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			return FinalizeColumn(boxInfo);
		}
		protected virtual AddBoxResult CanAddBoxWithoutHyphen(BoxInfo boxInfo) {
			return CanAddBox(boxInfo);
		}
		protected virtual void ApplyParagraphMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected internal virtual void ApplyLineBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected internal virtual void ApplyPageBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected internal virtual void ApplyColumnBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected void FinalizeParagraph(BoxInfo boxInfo) {
			RowsController.AddBox(ParagraphBoxFormatter.paragraphBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			ChangeState(FormatterState.Final);
		}
		protected void FinalizeSection(BoxInfo boxInfo) {
			RowsController.AddBox(ParagraphBoxFormatter.sectionBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			ChangeState(FormatterState.Final);
		}
		protected void FinalizeLine(BoxInfo boxInfo) {
			RowsController.AddBox(ParagraphBoxFormatter.lineBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			ChangeState(FormatterState.RowEmpty);
		}
		protected CompleteFormattingResult FinalizePage(BoxInfo boxInfo) {
			RowsController.AddBox(ParagraphBoxFormatter.pageBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
			if (result != CompleteFormattingResult.Success)
				return result;
			RowsController.MoveRowToNextPage();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected CompleteFormattingResult FinalizeColumn(BoxInfo boxInfo) {
			if (!Formatter.Iterator.Paragraph.IsInCell() && RowsController.SupportsColumnAndPageBreaks) {
				CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
				if (result != CompleteFormattingResult.Success)
					return result;
				RowsController.AddBox(ParagraphBoxFormatter.columnBreakBox, boxInfo);
				Formatter.ApproveFloatingObjects();
				EndRow();
				RowsController.MoveRowToNextColumn();
				ChangeState(FormatterState.RowEmpty);
			}
			else {
				if (CurrentRow.Boxes.Count > 0)
					FinalizeLine(boxInfo);
				else
					ChangeState(FormatterState.RowEmpty);
			}
			return CompleteFormattingResult.Success;
		}
		protected virtual void AddNumberingListBox() {
			if (CurrentRow.NumberingListBox != null)
				return;
			NumberingListBox numberingListBox = Iterator.Paragraph.BoxCollection.NumberingListBox;
			NumberingListIndex index = Iterator.Paragraph.GetNumberingListIndex();
			int listLevelIndex = Iterator.Paragraph.GetListLevelIndex();
			IListLevel listLevel = Iterator.Paragraph.DocumentModel.NumberingLists[index].Levels[listLevelIndex];
			BoxInfo boxInfo = new NumberingListBoxInfo();
			boxInfo.Box = numberingListBox;
			boxInfo.StartPos = numberingListBox.StartPos;
			boxInfo.EndPos = numberingListBox.EndPos;
			boxInfo.Size = numberingListBox.InitialBounds.Size;
			numberingListBox.Bounds = numberingListBox.InitialBounds;
			RowsController.UpdateCurrentRowHeight(boxInfo);
			RowsController.AddNumberingListBox(numberingListBox, listLevel.ListLevelProperties, Formatter);
		}
		protected internal virtual bool CanInsertNumberingListBox() {
			char ch = Iterator.CurrentChar;
			return ch != Characters.PageBreak && ch != Characters.ColumnBreak;
		}
		protected BoxInfo CreateParagraphMarkBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.Box = Iterator.CurrentBox;
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			Formatter.Measurer.MeasureParagraphMark(boxInfo);
			return boxInfo;
		}
		protected virtual BoxInfo CreateSectionMarkBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.Box = Iterator.CurrentBox;
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			Formatter.Measurer.MeasureSectionMark(boxInfo);
			return boxInfo;
		}
		protected BoxInfo CreateLineBreakBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			Formatter.Measurer.MeasureLineBreakMark(boxInfo);
			return boxInfo;
		}
		protected BoxInfo CreatePageBreakBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			Formatter.Measurer.MeasurePageBreakMark(boxInfo);
			return boxInfo;
		}
		protected BoxInfo CreateColumnBreakBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			Formatter.Measurer.MeasureColumnBreakMark(boxInfo);
			return boxInfo;
		}
		protected virtual void CalculateAndMeasureLayoutDependentTextBox(LayoutDependentTextBox layoutDependentTextBox, BoxInfo boxInfo) {
			LayoutDependentTextRun run = (LayoutDependentTextRun)Formatter.PieceTable.Runs[layoutDependentTextBox.StartPos.RunIndex];
			layoutDependentTextBox.CalculatedText = run.FieldResultFormatting.GetValue(Formatter, Formatter.DocumentModel);
			MeasureBoxContent(boxInfo);
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Formatter.Measurer.MeasureText(boxInfo);
		}
		protected void MeasureBoxContentCore(BoxInfo boxInfo) {
			Box box = boxInfo.Box;
			LayoutDependentTextBox layoutDependentTextBox = box as LayoutDependentTextBox;
			if (box != null && layoutDependentTextBox == null)
				boxInfo.Size = box.Bounds.Size;
			else
				if (layoutDependentTextBox != null)
					CalculateAndMeasureLayoutDependentTextBox(layoutDependentTextBox, boxInfo);
				else
					MeasureBoxContent(boxInfo);
		}
		AddBoxResult ObtainAddBoxResult(BoxInfo boxInfo) {
			if (Iterator.CurrentChar == Characters.Hyphen)
				return CanAddBox(boxInfo);
			else
				return CanAddBoxWithoutHyphen(boxInfo);
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			MeasureBoxContentCore(boxInfo);
			AddBoxResult result = ObtainAddBoxResult(boxInfo);
			return DispatchAddBoxResult(boxInfo, result);
		}
		StateContinueFormatResult DispatchAddBoxResult(BoxInfo boxInfo, AddBoxResult result) {
			switch (result) {
				case AddBoxResult.Success:
					AddSuccess(boxInfo);
					break;
				case AddBoxResult.HorizontalIntersect:
					return GetContinueFormatResult(HorizontalOverfull(boxInfo));
				case AddBoxResult.LeaveColumnFirstCellRow:
				case AddBoxResult.LeaveColumnPlain:
					return GetContinueFormatResult(ColumnOverfull(GetCanFitCurrentRowToColumn(result), boxInfo));
				case AddBoxResult.IntersectWithFloatObject:
					return GetContinueFormatResult(Formatter.RollbackToStartOfRow(CanFitCurrentRowToColumnResult.TextAreasRecreated));
				case AddBoxResult.RestartDueFloatingObject:
					return StateContinueFormatResult.RestartDueFloatingObject;
				default:
					Debug.Assert(false);
					Exceptions.ThrowInternalException();
					break;
			}
			return StateContinueFormatResult.Succes;
		}
		CanFitCurrentRowToColumnResult GetCanFitCurrentRowToColumn(AddBoxResult addBoxResult) {
			switch (addBoxResult) {
				case AddBoxResult.LeaveColumnPlain:
					return CanFitCurrentRowToColumnResult.PlainRowNotFitted;
				case AddBoxResult.LeaveColumnFirstCellRow:
					return CanFitCurrentRowToColumnResult.FirstCellRowNotFitted;
				default:
					Debug.Assert(false);
					return CanFitCurrentRowToColumnResult.RowFitted;
			}
		}
		protected override void ChangeState(FormatterState stateType) {
			if (Formatter.HasDeferredNumberingListBox && CanInsertNumberingListBox()) {
				Formatter.RowsController.OnDeferredBeginParagraph();
				AddNumberingListBox();
				Formatter.HasDeferredNumberingListBox = false;
			}
			Formatter.ChangeState(stateType);
		}
		protected void AddTextBox(BoxInfo boxInfo) {
			TextRunBase run = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex];
			IInlineObjectRun inlineObjectRun = run as IInlineObjectRun;
			if (inlineObjectRun != null) {
				Box box = RowsController.AddBox(inlineObjectRun.CreateBox(), boxInfo);
				RowsController.UpdateCurrentRowHeightFromInlineObjectRun(run, box, Formatter.Measurer);
			}
			else {
				LayoutDependentTextRun layoutDependentTextRun = run as LayoutDependentTextRun;
				if (layoutDependentTextRun == null)
					RowsController.AddBox(ParagraphBoxFormatter.textBox, boxInfo);
				else {
					RowsController.AddBox(ParagraphBoxFormatter.layoutDependentTextBox, boxInfo);
					if (layoutDependentTextRun is FootNoteRun)
						RowsController.CurrentRow.ProcessingFlags |= RowProcessingFlags.ContainsFootNotes;
					if (layoutDependentTextRun is EndNoteRun)
						RowsController.CurrentRow.ProcessingFlags |= RowProcessingFlags.ContainsEndNotes;
				}
				RowsController.UpdateCurrentRowHeight(boxInfo);
			}
		}
		protected virtual AddBoxResult GetAddBoxResultColumnOverfull(CanFitCurrentRowToColumnResult canFit) {
			switch (canFit) {
				case CanFitCurrentRowToColumnResult.PlainRowNotFitted: return AddBoxResult.LeaveColumnPlain;
				case CanFitCurrentRowToColumnResult.FirstCellRowNotFitted: return AddBoxResult.LeaveColumnFirstCellRow;
				default:
					Exceptions.ThrowInternalException();
					return AddBoxResult.LeaveColumnPlain;
			}
		}
		protected virtual AddBoxResult CanAddBoxCore(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
				if (canFit == CanFitCurrentRowToColumnResult.TextAreasRecreated)
					return AddBoxResult.IntersectWithFloatObject;
				else if (canFit == CanFitCurrentRowToColumnResult.RestartDueFloatingObject)
					return AddBoxResult.RestartDueFloatingObject;
				return GetAddBoxResultColumnOverfull(canFit);
			}
			if (!RowsController.CanFitBoxToCurrentRow(boxInfo.Size))
				return AddBoxResult.HorizontalIntersect;
			return AddBoxResult.Success;
		}
		Size CalcBoxSizeWithHyphen(BoxInfo boxInfo) {
			int hyphenWidth = Formatter.Measurer.MeasureHyphen(boxInfo.EndPos, null).Width;
			Size boxSize = boxInfo.Size;
			boxSize.Width += hyphenWidth;
			return boxSize;
		}
		protected AddBoxResult CanAddBoxWithHyphenCore(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
				if (canFit == CanFitCurrentRowToColumnResult.TextAreasRecreated)
					return AddBoxResult.IntersectWithFloatObject;
				else if (canFit == CanFitCurrentRowToColumnResult.RestartDueFloatingObject)
					return AddBoxResult.RestartDueFloatingObject;
				return GetAddBoxResultColumnOverfull(canFit);
			}
			Size boxSize = CalcBoxSizeWithHyphen(boxInfo);
			if (!RowsController.CanFitBoxToCurrentRow(boxSize))
				return AddBoxResult.HorizontalIntersect;
			return AddBoxResult.Success;
		}
		protected enum SplitBoxResult {
			Success,
			SuccessSuppressedHorizontalOverfull,
			FailedHorizontalOverfull
		}
		bool IsCurrentRowEmpty() {
			return RowsController.CurrentRow.Boxes.Count <= 0;
		}
		protected SplitBoxResult SplitBox(BoxInfo boxInfo) {
			FormatterPosition startPos = boxInfo.StartPos;
			Debug.Assert(startPos.RunIndex == boxInfo.EndPos.RunIndex);
			AdjustEndPositionResult result = AdjustEndPositionToFit(boxInfo);
			if (result == AdjustEndPositionResult.Success) {
				Iterator.SetNextPosition(boxInfo.EndPos);
				return SplitBoxResult.Success;
			}
			ParagraphBoxIterator iterator = Iterator;
			iterator.SetPosition(startPos);
			if (IsCurrentRowEmpty()) {
				iterator.Next();
				return SplitBoxResult.SuccessSuppressedHorizontalOverfull;
			}
			else
				return SplitBoxResult.FailedHorizontalOverfull;
		}
		private enum AdjustEndPositionResult {
			Success,
			Failure
		}
		const int splitTextBinarySearchBounds = 300;
		AdjustEndPositionResult AdjustEndPositionToFit(BoxInfo boxInfo) {
			if (boxInfo.StartPos == boxInfo.EndPos)
				return AdjustEndPositionResult.Failure;
			boxInfo.Box = null;
			int maxWidth = RowsController.GetMaxBoxWidth();
			FormatterPosition originalEndPos = boxInfo.EndPos;
			if (Formatter.Measurer.TryAdjustEndPositionToFit(boxInfo, maxWidth)) {
				Formatter.Measurer.MeasureText(boxInfo);
				if (CanAddBoxWithoutHyphen(boxInfo) == AddBoxResult.Success) {
					return AdjustEndPositionResult.Success;
				}
				boxInfo.EndPos = originalEndPos;
			}
			int offset = 0;
			if (boxInfo.EndPos.Offset - boxInfo.StartPos.Offset >= splitTextBinarySearchBounds) {
				int low = boxInfo.StartPos.Offset;
				int hi = boxInfo.StartPos.Offset + splitTextBinarySearchBounds;
				offset = BinarySearchFittedBox(boxInfo, low, hi);
				if (~offset > hi) {
					offset = 0;
					boxInfo.EndPos = originalEndPos;
				}
			}
			if (offset == 0)
				offset = BinarySearchFittedBox(boxInfo);
			Debug.Assert(offset < 0);
			offset = ~offset - 1;
			if (offset < boxInfo.StartPos.Offset) {
				boxInfo.EndPos = new FormatterPosition(boxInfo.EndPos.RunIndex, boxInfo.StartPos.Offset, boxInfo.EndPos.BoxIndex);
				return AdjustEndPositionResult.Failure;
			}
			else {
				boxInfo.EndPos = new FormatterPosition(boxInfo.EndPos.RunIndex, offset, boxInfo.EndPos.BoxIndex);
				Formatter.Measurer.MeasureText(boxInfo);
				return AdjustEndPositionResult.Success;
			}
		}
		protected virtual int BinarySearchFittedBox(BoxInfo boxInfo) {
			return BinarySearchFittedBox(boxInfo, boxInfo.StartPos.Offset, boxInfo.EndPos.Offset);
		}
		protected virtual int BinarySearchFittedBox(BoxInfo boxInfo, int low, int hi) {
			FormatterPosition endPos = boxInfo.EndPos;
			while (low <= hi) {
				int median = (low + ((hi - low) >> 1));
				boxInfo.EndPos = new FormatterPosition(endPos.RunIndex, median, endPos.BoxIndex);
				Formatter.Measurer.MeasureText(boxInfo);
				if (CanAddBoxWithoutHyphen(boxInfo) == AddBoxResult.Success)
					low = median + 1;
				else
					hi = median - 1;
			}
			return ~low;
		}
		protected void SetCurrentRowHeightToLastBoxHeight() {
			Row currentRow = CurrentRow;
			Debug.Assert(currentRow.Height <= 0);
			int boxesCount = currentRow.Boxes.Count;
			Debug.Assert(boxesCount > 0);
			currentRow.Height = currentRow.Boxes[boxesCount - 1].Bounds.Height;
		}
		protected void EndRow() {
			Formatter.RollbackUnapprovedFloatingObjects();
			Formatter.RowsController.EndRow(Formatter);
			int currentBottom = Formatter.RowsController.CurrentRow.Bounds.Bottom;
			if (currentBottom >= Formatter.MaxHeight && !Iterator.Paragraph.IsInCell() && Iterator.Paragraph.FrameProperties == null || Formatter.RowsController.ForceFormattingComplete)
				ForceFormattingComplete = true;
		}
		protected bool IsInlineObject() {
			return Iterator.CurrentChar == Characters.ObjectMark && Iterator.IsInlinePictureRun();
		}
	}
	#endregion
	#region FormatterStartEndState (abstract)
	public abstract class FormatterStartEndState : BoxFormatterStateBase {
		protected FormatterStartEndState(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			Debug.Assert(false);
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override bool IsTerminatorChar(char ch) {
			return false;
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			return StateContinueFormatResult.Succes;
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return AddBoxResult.Success;
		}
	}
	#endregion
	#region StateContinueFormattingFromParagraph
	public class StateContinueFormattingFromParagraph : FormatterStartEndState {
		ParagraphIndex paragraphIndex;
		public StateContinueFormattingFromParagraph(ParagraphBoxFormatter formatter, ParagraphIndex paragraphIndex)
			: base(formatter) {
			this.paragraphIndex = paragraphIndex;
		}
		public override FormatterState Type { get { return FormatterState.ContinueFromParagraph; } }
		public ParagraphIndex ParagraphIndex { get { return paragraphIndex; } }
	}
	#endregion
	#region RowEmptyHyphenation
	public class StateRowEmptyHyphenation : BoxFormatterStateBase {
		bool switchToRowEmpty;
		public StateRowEmptyHyphenation(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		#region Properties
		public override ParagraphBoxIterator Iterator { get { return Formatter.SyllableIterator; } }
		public override FormatterState Type { get { return FormatterState.RowEmptyHyphenation; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowEmptyHyphenationAfterFloatingObject; } }
		protected virtual FormatterState NextState { get { return FormatterState.RowTextHyphenation; } }
		#endregion
		protected override bool IsTerminatorChar(char ch) {
			return ch == Characters.Hyphen;
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			Formatter.ApproveFloatingObjects();
			AddTextBox(boxInfo);
			if (Iterator.CurrentChar == Characters.Hyphen) {
				Iterator.Next();
				ChangeState(NextState);
			}
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			SplitBoxResult splitBoxResult = SplitBox(boxInfo);
			if (splitBoxResult != SplitBoxResult.FailedHorizontalOverfull) {
				Formatter.ApproveFloatingObjects();
				AddTextBox(boxInfo);
			}
			if (!IsHyphenationOfWordComplete()) {
				EndRow();
				ChangeState(FormatterState.RowEmptyHyphenation);
				return CompleteFormattingResult.Success;
			}
			if (Formatter.Iterator.IsEnd) 
				return CompleteFormattingResult.Success;
			if (Iterator.CurrentChar == Characters.PageBreak)
				return CompleteFormattingResult.Success;
			if (Iterator.CurrentChar == Characters.ColumnBreak)
				return CompleteFormattingResult.Success;
			if (Iterator.CurrentChar != Characters.LineBreak || splitBoxResult == SplitBoxResult.SuccessSuppressedHorizontalOverfull) {
				EndRow();
				switchToRowEmpty = true;
			}
			return CompleteFormattingResult.Success;
		}
		protected bool IsHyphenationOfWordComplete() {
			return Iterator.IsEnd;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			return Formatter.RollbackToStartOfRowCore(canFit, FormatterState.RowEmptyHyphenation);
		}
		protected override CompleteFormattingResult FinishParagraph() {
			try {
				if (Formatter.Iterator.IsEnd) {
					Debug.Assert(CurrentRow.Height > 0);
					if (Formatter.Iterator.CurrentChar == Characters.ParagraphMark)
						return base.FinishParagraph();
					else
						return base.FinishSection();
				}
				else {
					char currentChar = Formatter.Iterator.CurrentChar;
					ChangeState(CalcFinalState(currentChar));
				}
			}
			finally {
				Formatter.ClearSyllableIterator();
			}
			return CompleteFormattingResult.Success;
		}
		FormatterState CalcFinalState(char currentChar) {
			if (switchToRowEmpty)
				return FormatterState.RowEmpty;
			if (FormatterHelper.IsCharSpace(currentChar))
				return FormatterState.RowSpaces;
			if (currentChar == Characters.TabMark)
				return FormatterState.RowTab;
			if (currentChar == Characters.LineBreak)
				return FormatterState.RowLineBreak;
			if (currentChar == Characters.PageBreak)
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowSpaces;
				else if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowPageBreak;
			if (currentChar == Characters.ColumnBreak) {
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowSpaces;
				else if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowColumnBreak;
			}
			Debug.Assert(false);
			Exceptions.ThrowInternalException();
			return FormatterState.Final;
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return CanAddBoxWithHyphenCore(boxInfo);
		}
		protected override AddBoxResult CanAddBoxWithoutHyphen(BoxInfo boxInfo) {
			return CanAddBoxCore(boxInfo);
		}
	}
	#endregion
	#region StateRowEmptyHyphenationAfterFloatingObject
	public class StateRowEmptyHyphenationAfterFloatingObject : StateRowEmptyHyphenation {
		public StateRowEmptyHyphenationAfterFloatingObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowEmptyHyphenationAfterFloatingObject; } }
	}
	#endregion
	#region StateRowEmptyAfterFloatingObject
	public class StateRowEmptyAfterFloatingObject : StateRowEmpty {
		public StateRowEmptyAfterFloatingObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowEmptyAfterFloatingObject; } }
	}
	#endregion
	#region RowTextSplit
	public class StateRowTextSplit : BoxFormatterStateBase, ISupportsChangeStateManually {
		public StateRowTextSplit(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTextSplit; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTextSplitAfterFloatingObject; } }
		public virtual FormatterState NextSplitState { get { return FormatterState.RowTextSplit; } }
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			SplitBoxResult splitBoxResult = SplitBox(boxInfo);
			if (splitBoxResult != SplitBoxResult.FailedHorizontalOverfull) {
				AddTextBox(boxInfo);
				Formatter.ApproveFloatingObjects();
				if (!Iterator.IsEnd) {
					if (Iterator.CurrentChar == Characters.PageBreak && !RowsController.TablesController.IsInsideTable) {
						if (!RowsController.SupportsColumnAndPageBreaks)
							ChangeState(FormatterState.RowLineBreak);
						else
							ChangeState(FormatterState.RowPageBreak);
						return CompleteFormattingResult.Success;
					}
					if (Iterator.CurrentChar == Characters.ColumnBreak && !RowsController.TablesController.IsInsideTable) {
						if (!RowsController.SupportsColumnAndPageBreaks)
							ChangeState(FormatterState.RowLineBreak);
						else
							ChangeState(FormatterState.RowColumnBreak);
						return CompleteFormattingResult.Success;
					}
					if (Iterator.CurrentChar == Characters.LineBreak && splitBoxResult != SplitBoxResult.SuccessSuppressedHorizontalOverfull)
						ChangeState(FormatterState.RowLineBreak);
					else {
						if (!(Iterator.CurrentBox is FloatingObjectAnchorBox)) {
							EndRow();
							if (IsTerminatorChar(Iterator.CurrentChar))
								ChangeState(FormatterState.RowEmpty);
							else
								ChangeState(NextSplitState);
						}
					}
				}
			}
			else {
				return HandleUnsuccessfulSplitting();
			}
			return CompleteFormattingResult.Success;
		}
		protected virtual CompleteFormattingResult HandleUnsuccessfulSplitting() {
			EndRow();
			ChangeState(FormatterState.RowTextSplit);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			return Formatter.RollbackToStartOfRow(canFit);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return CanAddBoxCore(boxInfo);
		}
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak || ch == Characters.FloatingObjectMark | FormatterHelper.IsCharDash(ch);
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			Formatter.ApproveFloatingObjects();
			AddTextBox(boxInfo);
			ChangeStateManuallyIfNeeded(boxInfo.IteratorResult);
		}
		protected bool ShouldChangeStateManually(ParagraphIteratorResult iteratorResult, char currentCharacter) {
			return iteratorResult == ParagraphIteratorResult.Success || (!Iterator.IsEnd && IsTerminatorChar(currentCharacter)) || IsInlineObject();
		}
		protected virtual FormatterState CalcNextState(char currentCharacter) {
			if (FormatterHelper.IsCharSpace(currentCharacter))
				return FormatterState.RowSpaces;
			if (currentCharacter == Characters.TabMark)
				return FormatterState.RowTab;
			if (FormatterHelper.IsCharDash(currentCharacter))
				return FormatterState.RowDash;
			if (currentCharacter == Characters.LineBreak)
				return FormatterState.RowLineBreak;
			if (currentCharacter == Characters.PageBreak) {
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowSpaces;
				else if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowPageBreak;
			}
			if (currentCharacter == Characters.ColumnBreak) {
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowSpaces;
				else if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowColumnBreak;
			}
			if (currentCharacter == Characters.FloatingObjectMark)
				if (Iterator.IsFloatingObjectAnchorRun())
					return FormatterState.FloatingObject;
				else
					return FormatterState.RowWithTextOnly;
			if (currentCharacter == Characters.ObjectMark) {
				if (Iterator.IsInlinePictureRun())
					return FormatterState.RowInlineObject;
				else
					return FormatterState.RowWithTextOnly;
			}
			Debug.Assert(false);
			Exceptions.ThrowInternalException();
			return FormatterState.Final;
		}
		protected override CompleteFormattingResult FinishParagraph() {
			Debug.Assert(CurrentRow.Height > 0);
			return base.FinishParagraph();
		}
		protected override CompleteFormattingResult FinishSection() {
			Debug.Assert(CurrentRow.Height > 0);
			return base.FinishSection();
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height > 0);
			Formatter.RowsController.AddBox(ParagraphBoxFormatter.lineBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height > 0);
			Formatter.RowsController.AddBox(ParagraphBoxFormatter.pageBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
			if (result != CompleteFormattingResult.Success)
				return result;
			RowsController.MoveRowToNextPage();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height > 0);
			CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
			if (result != CompleteFormattingResult.Success)
				return result;
			Formatter.RowsController.AddBox(ParagraphBoxFormatter.columnBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			RowsController.MoveRowToNextColumn();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		public void ChangeStateManuallyIfNeeded(ParagraphIteratorResult iteratorResult) {
			char currentChar = Iterator.CurrentChar;
			if (ShouldChangeStateManually(iteratorResult, currentChar)) {
				FormatterState nextState = CalcNextState(currentChar);
				ChangeState(nextState);
			}
		}
	}
	#endregion
	#region StateRowTextSplitAfterFloatingObject
	public class StateRowTextSplitAfterFloatingObject : StateRowTextSplit {
		public StateRowTextSplitAfterFloatingObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTextSplitAfterFloatingObject; } }
	}
	#endregion
	#region RowTextSplitAfterFirstLeadingTab
	public class StateRowTextSplitAfterFirstLeadingTab : StateRowTextSplit {
		public StateRowTextSplitAfterFirstLeadingTab(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTextSplitAfterFirstLeadingTab; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTextSplitAfterFirstLeadingTabAfterFloatingObject; } }
		CompleteFormattingResult CompleteRowProcessing() {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn();
			if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
				return Formatter.RollbackToStartOfRow(canFit);
			}
			else {
				EndRow();
				ChangeState(FormatterState.RowEmpty);
				return CompleteFormattingResult.Success;
			}
		}
		protected override CompleteFormattingResult HandleUnsuccessfulSplitting() {
			Formatter.RollbackToStartOfWord();
			SetCurrentRowHeightToLastBoxHeight();
			return CompleteRowProcessing();
		}
	}
	#endregion
	#region RowDashSplit
	public class StateRowDashSplit : StateRowTextSplit {
		public StateRowDashSplit(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowDashSplit; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowDashSplitAfterFloatingObject; } }
		public override FormatterState NextSplitState { get { return FormatterState.RowDashSplit; } }
		protected override CompleteFormattingResult HandleUnsuccessfulSplitting() {
			if (CurrentRow.Height <= 0) {
				SetCurrentRowHeightToLastBoxHeight();
				CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn();
				if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
					return Formatter.RollbackToStartOfRow(canFit);
				}
			}
			EndRow();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected override bool IsTerminatorChar(char ch) {
			return !FormatterHelper.IsCharDash(ch);
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (!base.IsTerminatorChar(currentCharacter))
				return FormatterState.RowText;
			return base.CalcNextState(currentCharacter);
		}
	}
	#endregion
	#region StateRowDashSplitAfterFloatingObject
	public class StateRowDashSplitAfterFloatingObject : StateRowDashSplit {
		public StateRowDashSplitAfterFloatingObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowDashSplitAfterFloatingObject; } }
	}
	#endregion
	#region RowTextHyphenationFirstSyllable
	public class StateRowTextHyphenationFirstSyllable : StateRowEmptyHyphenation {
		public StateRowTextHyphenationFirstSyllable(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTextHyphenationFirstSyllable; } }
		protected override FormatterState NextState { get { return FormatterState.RowTextHyphenation; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTextHyphenationFirstSyllableAfterFloatingObject; } }
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord(Formatter.Iterator);
			Formatter.RollbackToStartOfWord();
			Formatter.RollbackToLastTab(Formatter.Iterator);
			Formatter.RollbackToLastTab();
			RowsController.TryToRemoveLastTabBox();
			if (CurrentRow.Height <= 0) {
				SetCurrentRowHeightToLastBoxHeight();
				CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn();
				if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
					Formatter.ClearSyllableIterator();
					return Formatter.RollbackToStartOfRow(canFit);
				}
			}
			EndRow();
			Formatter.ClearSyllableIterator();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region RowTextHyphenationFirstSyllableAfterFirstLeadingTab
	public class StateRowTextHyphenationFirstSyllableAfterFirstLeadingTab : StateRowTextHyphenationFirstSyllable {
		public StateRowTextHyphenationFirstSyllableAfterFirstLeadingTab(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTextHyphenationFirstSyllableAfterFirstLeadingTab; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTextHyphenationFirstSyllableAfterFirstLeadingTabAfterFloatingObject; } }
		CompleteFormattingResult CompleteRowProcessing() {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn();
			if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
				return Formatter.RollbackToStartOfRow(canFit);
			}
			else {
				EndRow();
				ChangeState(FormatterState.RowEmpty);
			}
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			SplitBoxResult splitBoxResult = SplitBox(boxInfo);
			if (splitBoxResult != SplitBoxResult.FailedHorizontalOverfull) {
				AddTextBox(boxInfo);
				EndRow();
				ChangeState(FormatterState.RowEmptyHyphenation);
				return CompleteFormattingResult.Success;
			}
			else {
				Formatter.ClearSyllableIterator();
				Formatter.RollbackToStartOfWord();
				SetCurrentRowHeightToLastBoxHeight();
				return CompleteRowProcessing();
			}
		}
	}
	#endregion
	#region RowTextHyphenation
	public class StateRowTextHyphenation : StateRowEmptyHyphenation {
		public StateRowTextHyphenation(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTextHyphenation; } }
		protected override FormatterState NextState { get { return FormatterState.RowEmptyHyphenation; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTextHyphenationAfterFloatingObject; } }
		protected override void AddSuccess(BoxInfo boxInfo) {
			Formatter.ApproveFloatingObjects();
			AddTextBox(boxInfo);
			if (Iterator.CurrentChar == Characters.Hyphen) {
				Iterator.Next();
				Formatter.StartNewWord();
			}
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			InsertHyphenBox();
			EndRow();
			ChangeState(NextState);
			return CompleteFormattingResult.Success;
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return CanAddBoxWithHyphenCore(boxInfo);
		}
		protected override AddBoxResult CanAddBoxWithoutHyphen(BoxInfo boxInfo) {
			return CanAddBoxCore(boxInfo);
		}
		void InsertHyphenBox() {
			BoxInfo boxInfo = CreateHyphenBoxInfo();
			RowsController.AddBox(ParagraphBoxFormatter.hyphenBox, boxInfo);
		}
		protected BoxInfo CreateHyphenBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetPreviousPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			boxInfo.Size = Formatter.Measurer.MeasureHyphen(boxInfo.StartPos, boxInfo);
			return boxInfo;
		}
	}
	#endregion
	public delegate void FinalizeHandler(BoxInfo boxInfo);
	#region StateRowEmptyBase (abstract class)
	public abstract class StateRowEmptyBase : BoxFormatterStateBase {
		protected StateRowEmptyBase(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override StateContinueFormatResult ContinueFormat() {
			char ch = Iterator.CurrentChar;
			if (ch == Characters.ParagraphMark) {
				if (Iterator.IsParagraphMarkRun())
					return GetContinueFormatResult(FinishParagraph());
				else
					ChangeState(FormatterState.RowWithTextOnly);
			}
			else if (ch == Characters.SectionMark) {
				if (Iterator.IsParagraphMarkRun())
					return GetContinueFormatResult(FinishSection());
				else
					ChangeState(FormatterState.RowWithTextOnly);
			}
			else if (FormatterHelper.IsCharSpace(ch))
				ChangeState(FormatterState.RowWithSpacesOnly);
			else if (FormatterHelper.IsCharDash(ch))
				ChangeState(FormatterState.RowWithDashOnly);
			else if (ch == Characters.TabMark)
				ChangeState(FormatterState.RowFirstLeadingTab);
			else if (ch == Characters.LineBreak)
				ChangeState(FormatterState.RowLineBreak);
			else if (ch == Characters.PageBreak) {
				if (RowsController.TablesController.IsInsideTable)
					ChangeState(FormatterState.RowWithSpacesOnly);
				else if (!RowsController.SupportsColumnAndPageBreaks)
					ChangeState(FormatterState.RowLineBreak);
				else
					ChangeState(FormatterState.RowPageBreak);
			}
			else if (ch == Characters.ColumnBreak) {
				if (RowsController.TablesController.IsInsideTable)
					ChangeState(FormatterState.RowWithSpacesOnly);
				else if (!RowsController.SupportsColumnAndPageBreaks)
					ChangeState(FormatterState.RowLineBreak);
				else
					ChangeState(FormatterState.RowColumnBreak);
			}
			else if (ch == Characters.FloatingObjectMark) {
				if (Iterator.IsFloatingObjectAnchorRun())
					ChangeState(FormatterState.FloatingObject);
				else
					ChangeState(FormatterState.RowWithTextOnly);
			}
			else if (ch == Characters.ObjectMark) {
				if (Iterator.IsInlinePictureRun())
					ChangeState(FormatterState.RowWithInlineObjectOnly);
				else
					ChangeState(FormatterState.RowWithTextOnly);
			}
			else
				ChangeState(FormatterState.RowWithTextOnly);
			return StateContinueFormatResult.Succes;
		}
		protected override CompleteFormattingResult FinishParagraph() {
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			return FinishParagraphCore(boxInfo, FinalizeParagraph);
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			return FinishParagraphCore(boxInfo, FinalizeSection);
		}
		CompleteFormattingResult FinishParagraphCore(BoxInfo boxInfo, FinalizeHandler finalizeHandler) {
			ApplyParagraphMarkSize(boxInfo);
			CanFitCurrentRowToColumnResult canFit = CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				finalizeHandler(boxInfo);
			else {
				return Formatter.RollbackToStartOfRow(canFit);
			}
			return CompleteFormattingResult.Success;
		}
		protected virtual CanFitCurrentRowToColumnResult CanFitCurrentRowToColumn(BoxInfo boxInfo) {
			return RowsController.CanFitCurrentRowToColumn(boxInfo);
		}
		protected override void ApplyParagraphMarkSize(BoxInfo boxInfo) {
#if DEBUGTEST || DEBUG
			if ((CurrentRow.ProcessingFlags & RowProcessingFlags.LastInvisibleEmptyCellRowAfterNestedTable) == 0) {
				Debug.Assert(CurrentRow.NumberingListBox != null || (CurrentRow.NumberingListBox == null && (CurrentRow.Height == RowsController.DefaultRowHeight || (CurrentRow.Boxes.Count <= 0 && RowsController.HorizontalPositionController is FloatingObjectsCurrentHorizontalPositionController))));
			}
#endif
			if ((CurrentRow.ProcessingFlags & RowProcessingFlags.LastInvisibleEmptyCellRowAfterNestedTable) == 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyLineBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || (CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight));
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyPageBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || (CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight));
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyColumnBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || (CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight));
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				FinalizeLine(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				return FinalizePage(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				return FinalizeColumn(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
		}
	}
	#endregion
	#region RowEmpty
	public class StateRowEmpty : StateRowEmptyBase {
		public StateRowEmpty(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowEmpty; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowEmptyAfterFloatingObject; } }
		protected override void AddSuccess(BoxInfo boxInfo) {
			Debug.Assert(false);
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override bool IsTerminatorChar(char ch) {
			return false;
		}
		protected override StateContinueFormatResult AppendBox(BoxInfo boxInfo) {
			return StateContinueFormatResult.Succes;
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return AddBoxResult.Success;
		}
	}
	#endregion
	#region StateParagraphStart
	public class StateParagraphStart : StateRowEmpty {
		public StateParagraphStart(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.ParagraphStart; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.ParagraphStartAfterFloatingObject; } }
		public override StateContinueFormatResult ContinueFormat() {
			InsertNumberingListBox();
			return base.ContinueFormat();
		}
		protected virtual void InsertNumberingListBox() {
			if (Iterator.Paragraph.IsInList()) {
				if (CanInsertNumberingListBox())
					AddNumberingListBox();
				else
					Formatter.HasDeferredNumberingListBox = true;
			}
		}
		protected override CanFitCurrentRowToColumnResult CanFitCurrentRowToColumn(BoxInfo boxInfo) {
			if ((RowsController.CurrentRow.ProcessingFlags & RowProcessingFlags.LastInvisibleEmptyCellRowAfterNestedTable) != 0)
				return CanFitCurrentRowToColumnResult.RowFitted;
			return base.CanFitCurrentRowToColumn(boxInfo);
		}
	}
	#endregion
	#region StateParagraphStartAfterFloatingObject
	public class StateParagraphStartAfterFloatingObject : StateParagraphStart {
		public StateParagraphStartAfterFloatingObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.ParagraphStartAfterFloatingObject; } }
	}
	#endregion
	#region StateParagraphStartFromTheMiddle
	public class StateParagraphStartFromTheMiddle : StateParagraphStart {
		public StateParagraphStartFromTheMiddle(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.ParagraphStartFromTheMiddle; } }
		public override FormatterState StateAfterFloatingObject { get { return Type; } }
		protected override void InsertNumberingListBox() {
		}
	}
	#endregion
	#region StateParagraphStartFromTheMiddleAfterFloatingObject
	public class StateParagraphStartFromTheMiddleAfterFloatingObject : StateParagraphStartFromTheMiddle {
		public StateParagraphStartFromTheMiddleAfterFloatingObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.ParagraphStartFromTheMiddleAfterFloatingObject; } }
		public override FormatterState StateAfterFloatingObject { get { return Type; } }
	}
	#endregion
	public class StateFloatingObject : BoxFormatterStateBase {
		readonly BoxFormatterStateBase previousState;
		public StateFloatingObject(ParagraphBoxFormatter formatter, BoxFormatterStateBase previousState)
			: base(formatter) {
			Guard.ArgumentNotNull(previousState, "previousState");
			this.previousState = previousState;
		}
		public override FormatterState Type { get { return FormatterState.FloatingObject; } }
		protected override bool CanUseBox {
			get {
				return true; 
			}
		}
		protected override bool IsTerminatorChar(char ch) {
			return ch != Characters.FloatingObjectMark;
		}
		public override StateContinueFormatResult ContinueFormat() {
			if (Iterator.IsFloatingObjectAnchorRun())
				return base.ContinueFormat();
			else {
				SwitchToNextState();
				return StateContinueFormatResult.Succes;
			}
		}
		protected virtual void SwitchToNextState() {
			ChangeState(previousState.StateAfterFloatingObject);
			ISupportsChangeStateManually newState = Formatter.State as ISupportsChangeStateManually;
			if (newState != null)
				newState.ChangeStateManuallyIfNeeded(ParagraphIteratorResult.RunFinished);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return CanAddBoxCore(boxInfo);
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			TextRunBase run = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex];
			FloatingObjectAnchorRun objectAnchorRun = run as FloatingObjectAnchorRun;
			if (objectAnchorRun != null) {
				if (!objectAnchorRun.ExcludeFromLayout) {
					FloatingObjectAnchorBox anchorBox = boxInfo.Box as FloatingObjectAnchorBox;
					if (anchorBox != null)
						RowsController.AddFloatingObjectToLayout(anchorBox, boxInfo);
				}
			}
			else {
				MergedFrameProperties frameProperties = Formatter.GetActualParagraphFrameProperties(run.Paragraph);
				if (frameProperties != null) {
					this.RowsController.FrameParagraphIndex = run.Paragraph.Index;
					RowsController.AddParagraphFrameToLayout(new ParagraphFrameBox(run.Paragraph), boxInfo);
				}
			}
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishParagraphOrSecton() {
			return previousState.FinishParagraphOrSecton();
		}
	}
	public class StateParagraphFrame : BoxFormatterStateBase {
		readonly BoxFormatterStateBase previousState;
		public StateParagraphFrame(ParagraphBoxFormatter formatter, BoxFormatterStateBase previousState)
			: base(formatter) {
			Guard.ArgumentNotNull(previousState, "previousState");
			this.previousState = previousState;
		}
		public override FormatterState Type { get { return FormatterState.ParagraphFrame; } }
		protected override bool CanUseBox { get { return true; } }
		protected override bool IsTerminatorChar(char ch) {
			return ch != Characters.FloatingObjectMark;
		}
		public override StateContinueFormatResult ContinueFormat() {
			if (Iterator.IsParagraphFrame())
				return FormatParagraphFrame();
			else {
				SwitchToNextState();
				return StateContinueFormatResult.Succes;
			}
		}
		protected virtual void SwitchToNextState() {
			ChangeState(previousState.StateAfterFloatingObject);
			ISupportsChangeStateManually newState = Formatter.State as ISupportsChangeStateManually;
			if (newState != null)
				newState.ChangeStateManuallyIfNeeded(ParagraphIteratorResult.RunFinished);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return CanAddBoxCore(boxInfo);
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			TextRunBase run = Formatter.PieceTable.Runs[boxInfo.StartPos.RunIndex];
			MergedFrameProperties frameProperties = Formatter.GetActualParagraphFrameProperties(run.Paragraph);
			if (frameProperties != null) {
				this.RowsController.FrameParagraphIndex = run.Paragraph.Index;
				RowsController.AddParagraphFrameToLayout(new ParagraphFrameBox(run.Paragraph), boxInfo);
			}
		}
		public StateContinueFormatResult FormatParagraphFrame() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			boxInfo.Box = new ParagraphFrameBox(this.Formatter.Iterator.Paragraph);
			if (CanUseBox && boxInfo.StartPos.AreEqual(Iterator.CurrentBox.StartPos)) {
				StateContinueFormatResult addExistingBoxResult = AppendBox(boxInfo);
				if (addExistingBoxResult != StateContinueFormatResult.Succes)
					return addExistingBoxResult;
				if (Iterator.IsEnd) {
					if (Iterator.IsParagraphFrame() && this is StateParagraphFrame)
						return StateContinueFormatResult.Succes;
					if (Formatter.SyllableIterator == null || Formatter.SyllableIterator.IsEnd) {
						return GetContinueFormatResult(FinishParagraphOrSecton());
					}
				}
				return StateContinueFormatResult.Succes;
			}
			return ContinueFormatByCharacter(boxInfo, Iterator.CurrentBox as LayoutDependentTextBox);
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishParagraphOrSecton() {
			return previousState.FinishParagraphOrSecton();
		}
		protected override AddBoxResult CanAddBoxCore(BoxInfo boxInfo) {
			if (boxInfo.Box == null) {
				boxInfo.Box = new ParagraphFrameBox(this.Formatter.Iterator.Paragraph);
			}
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowWithFrameToColumn(boxInfo);
			if (Formatter.ActualParagraphFrameProperties != null) {
				if (boxInfo.Box != null && boxInfo.Box is ParagraphFrameBox) {
					if (!RowsController.CanFitBoxToCurrentRow(boxInfo.Size))
						return AddBoxResult.HorizontalIntersect;
					return AddBoxResult.Success;
				}
			}
			if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
				if (canFit == CanFitCurrentRowToColumnResult.TextAreasRecreated)
					return AddBoxResult.IntersectWithFloatObject;
				else if (canFit == CanFitCurrentRowToColumnResult.RestartDueFloatingObject)
					return AddBoxResult.RestartDueFloatingObject;
				return GetAddBoxResultColumnOverfull(canFit);
			}
			if (!RowsController.CanFitBoxToCurrentRow(boxInfo.Size))
				return AddBoxResult.HorizontalIntersect;
			return AddBoxResult.Success;
		}
	}
	#region StateSectionBreakAfterParagraphMark
	public class StateSectionBreakAfterParagraphMark : FormatterStartEndState {
		public StateSectionBreakAfterParagraphMark(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.SectionBreakAfterParagraphMark; } }
		protected override CompleteFormattingResult FinishParagraph() {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			if (RowsController.CanAddSectionBreakToPrevRow()) {
				RowsController.AddSectionBreakBoxToPrevRow(ParagraphBoxFormatter.sectionBox, boxInfo);
				ChangeState(FormatterState.Final);
				return CompleteFormattingResult.Success;
			}
			else {
				if (CurrentRow.Height == 0)
					RowsController.UpdateCurrentRowHeight(boxInfo);
				ApplyParagraphMarkSize(boxInfo);
				FinalizeSection(boxInfo);
				return CompleteFormattingResult.Success;
			}
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		public override StateContinueFormatResult ContinueFormat() {
			Debug.Assert(Iterator.CurrentChar == Characters.SectionMark);
			return GetContinueFormatResult(FinishSection());
		}
		protected override BoxInfo CreateSectionMarkBoxInfo() {
			BoxInfo boxInfo = new BoxInfo();
			boxInfo.Box = Iterator.CurrentBox;
			boxInfo.StartPos = Iterator.GetCurrentPosition();
			boxInfo.EndPos = boxInfo.StartPos;
			Formatter.Measurer.MeasureSectionMark(boxInfo);
			return boxInfo;
		}
	}
	#endregion
	#region Final
	public class StateFinal : FormatterStartEndState {
		public StateFinal(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.Final; } }
		public override bool FormattingComplete { get { return true; } }
		protected override CompleteFormattingResult FinishParagraph() {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region StateRowBreakBase (abstract class)
	public abstract class StateRowBreakBase : FormatterStartEndState {
		readonly BoxFormatterStateBase prevState;
		protected StateRowBreakBase(ParagraphBoxFormatter formatter, BoxFormatterStateBase prevState)
			: base(formatter) {
			if (prevState == null)
				Exceptions.ThrowArgumentException("prevState", prevState);
			this.prevState = prevState;
		}
		protected BoxFormatterStateBase PrevState { get { return prevState; } }
		protected override bool CanUseBox { get { return true; } }
		protected override CompleteFormattingResult FinishParagraph() {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
	}
	#endregion
	#region RowLineBreak
	public class StateRowLineBreak : StateRowBreakBase {
		public StateRowLineBreak(ParagraphBoxFormatter formatter, BoxFormatterStateBase prevState)
			: base(formatter, prevState) {
		}
		public override FormatterState Type { get { return FormatterState.RowLineBreak; } }
		public override StateContinueFormatResult ContinueFormat() {
			BoxInfo boxInfo = new BoxInfo();
			Box currentBox = Iterator.CurrentBox;
			boxInfo.Box = currentBox;
			boxInfo.StartPos = currentBox.StartPos;
			boxInfo.EndPos = currentBox.EndPos;
			boxInfo.Size = boxInfo.Box.Bounds.Size;
			PrevState.ApplyLineBreakMarkSize(boxInfo);
			Iterator.Next();
			return GetContinueFormatResult(PrevState.FinishLine(boxInfo));
		}
	}
	#endregion
	#region RowPageBreak
	public class StateRowPageBreak : StateRowBreakBase {
		public StateRowPageBreak(ParagraphBoxFormatter formatter, BoxFormatterStateBase prevState)
			: base(formatter, prevState) {
		}
		public override FormatterState Type { get { return FormatterState.RowPageBreak; } }
		public override StateContinueFormatResult ContinueFormat() {
			if (!RowsController.TablesController.IsInsideTable) {
				BoxInfo boxInfo = new BoxInfo();
				Box currentBox = Iterator.CurrentBox;
				boxInfo.Box = currentBox;
				boxInfo.StartPos = currentBox.StartPos;
				boxInfo.EndPos = currentBox.EndPos;
				boxInfo.Size = boxInfo.Box.Bounds.Size;
				PrevState.ApplyPageBreakMarkSize(boxInfo);
				Iterator.Next();
				return GetContinueFormatResult(PrevState.FinishPage(boxInfo));
			}
			else {
				Iterator.Next();
				ChangeState(PrevState.Type);
			}
			return StateContinueFormatResult.Succes;
		}
	}
	#endregion
	#region RowColumnBreak
	public class StateRowColumnBreak : StateRowBreakBase {
		public StateRowColumnBreak(ParagraphBoxFormatter formatter, BoxFormatterStateBase prevState)
			: base(formatter, prevState) {
		}
		public override FormatterState Type { get { return FormatterState.RowColumnBreak; } }
		public override StateContinueFormatResult ContinueFormat() {
			BoxInfo boxInfo = new BoxInfo();
			Box currentBox = Iterator.CurrentBox;
			boxInfo.Box = currentBox;
			boxInfo.StartPos = currentBox.StartPos;
			boxInfo.EndPos = currentBox.EndPos;
			boxInfo.Size = boxInfo.Box.Bounds.Size;
			PrevState.ApplyColumnBreakMarkSize(boxInfo);
			Iterator.Next();
			return GetContinueFormatResult(PrevState.FinishColumn(boxInfo));
		}
	}
	#endregion
	#region RowWithSpacesOnly
	public class StateRowWithSpacesOnly : StateRowSpaces {
		public StateRowWithSpacesOnly(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowWithSpacesOnly; } }
		protected override void ApplyParagraphMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyLineBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyPageBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyColumnBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (currentCharacter == '\t')
				return FormatterState.RowLeadingTab;
			if (currentCharacter == Characters.LineBreak)
				return FormatterState.RowLineBreak;
			if (currentCharacter == Characters.PageBreak) {
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowText;
				if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowPageBreak;
			}
			if (currentCharacter == Characters.ColumnBreak) {
				Debug.Assert(!RowsController.TablesController.IsInsideTable);
				if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowColumnBreak;
			}
			if (FormatterHelper.IsCharDash(currentCharacter))
				return FormatterState.RowDash;
			if (currentCharacter == Characters.FloatingObjectMark && Iterator.IsFloatingObjectAnchorRun())
				return FormatterState.FloatingObject;
			else
				return FormatterState.RowText;
		}
		protected override CompleteFormattingResult FinishParagraph() {
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			return FinishParagraphCore(boxInfo, FinalizeParagraph);
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			return FinishParagraphCore(boxInfo, FinalizeSection);
		}
		private CompleteFormattingResult FinishParagraphCore(BoxInfo boxInfo, FinalizeHandler finalizeHandler) {
			ApplyParagraphMarkSize(boxInfo);
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				finalizeHandler(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				FinalizeLine(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				return FinalizePage(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				return FinalizeColumn(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
		}
	}
	#endregion
	#region RowTabBase (abstract)
	public abstract class StateRowTabBase : BoxFormatterStateBase, ISupportsChangeStateManually {
		TabInfo currentTab;
		protected TabInfo CurrentTab { get { return currentTab; } set { currentTab = value; } }
		protected override bool CanUseBox { get { return true; } }
		protected internal int CurrentTabPosition { get { return CurrentTab.GetLayoutPosition(Formatter.DocumentModel.ToDocumentLayoutUnitConverter); } }
		protected StateRowTabBase(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Formatter.Measurer.MeasureTab(boxInfo);
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			Formatter.ApproveFloatingObjects();
			boxInfo.Size = new Size(0, boxInfo.Size.Height);
			RowsController.AddTabBox(CurrentTab, boxInfo);
			TabsController.CalculatePreciselyLeaderCount((TabSpaceBox)boxInfo.Box, RowsController.PieceTable, Formatter.Measurer);
			SwitchToNextState();
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected internal abstract void SwitchToNextState();
		public void ChangeStateManuallyIfNeeded(ParagraphIteratorResult iteratorResult) {
			SwitchToNextState();
		}
	}
	#endregion
	#region RowLeadingTab
	public class StateRowLeadingTab : StateRowTabBase {
		public StateRowLeadingTab(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowLeadingTab; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowLeadingTabAfterFloatingObject; } }
		protected internal override void SwitchToNextState() {
			FormatterState nextState;
			switch (Iterator.CurrentChar) {
				case Characters.Space:
				case Characters.EmSpace:
				case Characters.EnSpace:
					nextState = FormatterState.RowWithSpacesOnly;
					break;
				case Characters.TabMark:
					Formatter.StartNewTab();
					return;
				case Characters.LineBreak:
					nextState = FormatterState.RowLineBreak;
					break;
				case Characters.PageBreak:
					if (RowsController.TablesController.IsInsideTable)
						nextState = FormatterState.RowWithSpacesOnly;
					else if (!RowsController.SupportsColumnAndPageBreaks)
						nextState = FormatterState.RowLineBreak;
					else
						nextState = FormatterState.RowPageBreak;
					break;
				case Characters.ColumnBreak:
					if (RowsController.TablesController.IsInsideTable)
						nextState = FormatterState.RowWithSpacesOnly;
					else if (!RowsController.SupportsColumnAndPageBreaks)
						nextState = FormatterState.RowLineBreak;
					else
						nextState = FormatterState.RowColumnBreak;
					break;
				case Characters.ParagraphMark:
					if (Iterator.IsParagraphMarkRun())
						return;
					else
						nextState = FormatterState.RowText;
					break;
				case Characters.SectionMark:
					if (Iterator.IsParagraphMarkRun())
						return;
					else
						nextState = FormatterState.RowText;
					break;
				case Characters.FloatingObjectMark:
					if (Iterator.IsFloatingObjectAnchorRun())
						nextState = FormatterState.FloatingObject;
					else
						nextState = FormatterState.RowText;
					break;
				default:
					nextState = FormatterState.RowText;
					break;
			}
			ChangeState(nextState);
		}
		protected override CompleteFormattingResult FinishParagraph() {
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			return FinishParagraphCore(boxInfo, FinalizeParagraph);
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			return FinishParagraphCore(boxInfo, FinalizeSection);
		}
		private CompleteFormattingResult FinishParagraphCore(BoxInfo boxInfo, FinalizeHandler finalizeHandler) {
			ApplyParagraphMarkSize(boxInfo);
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				finalizeHandler(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
			return CompleteFormattingResult.Success;
		}
		protected override void ApplyParagraphMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				FinalizeLine(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				return FinalizePage(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn(boxInfo);
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted)
				return FinalizeColumn(boxInfo);
			else
				return Formatter.RollbackToStartOfRow(canFit);
		}
		protected internal override void ApplyLineBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyPageBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected internal override void ApplyColumnBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.NumberingListBox != null || CurrentRow.NumberingListBox == null && CurrentRow.Height == RowsController.DefaultRowHeight);
			RowsController.UpdateCurrentRowHeight(boxInfo);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			this.CurrentTab = RowsController.GetNextTab(Formatter);
			if (!RowsController.CanFitBoxToCurrentRow(Size.Empty))
				return AddBoxResult.HorizontalIntersect;
			if (!CurrentTab.IsDefault)
				return AddBoxResult.Success;
			if (RowsController.IsPositionOutsideRightParagraphBound(CurrentTabPosition + RowsController.CurrentColumn.Bounds.Left))
				return AddBoxResult.HorizontalIntersect;
			return AddBoxResult.Success;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			SetCurrentRowHeightToLastBoxHeight();
			CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn();
			if (canFit == CanFitCurrentRowToColumnResult.RowFitted) {
				EndRow();
				if (RowsController.CanFitCurrentRowToColumn() != CanFitCurrentRowToColumnResult.RowFitted) {
					CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
					if (result != CompleteFormattingResult.Success)
						return result;
					RowsController.MoveRowToNextColumn();
				}
				ChangeState(FormatterState.RowEmpty);
			}
			else {
				return Formatter.RollbackToStartOfRow(canFit);
			}
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region RowFirstLeadingTab
	public class StateRowFirstLeadingTab : StateRowLeadingTab {
		public StateRowFirstLeadingTab(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowFirstLeadingTab; } }
		protected internal override void SwitchToNextState() {
			FormatterState nextState;
			switch (Iterator.CurrentChar) {
				case Characters.Space:
				case Characters.EmSpace:
				case Characters.EnSpace:
					nextState = FormatterState.RowWithSpacesOnly;
					break;
				case Characters.TabMark:
					nextState = FormatterState.RowLeadingTab;
					break;
				case Characters.LineBreak:
					nextState = FormatterState.RowLineBreak;
					break;
				case Characters.Dash:
				case Characters.EmDash:
				case Characters.EnDash:
					nextState = FormatterState.RowWithDashOnly;
					break;
				case Characters.PageBreak:
					if (RowsController.TablesController.IsInsideTable)
						nextState = FormatterState.RowWithSpacesOnly;
					else if (!RowsController.SupportsColumnAndPageBreaks)
						nextState = FormatterState.RowLineBreak;
					else
						nextState = FormatterState.RowPageBreak;
					break;
				case Characters.ColumnBreak:
					if (RowsController.TablesController.IsInsideTable)
						nextState = FormatterState.RowWithSpacesOnly;
					else if (!RowsController.SupportsColumnAndPageBreaks)
						nextState = FormatterState.RowLineBreak;
					else
						nextState = FormatterState.RowColumnBreak;
					break;
				case Characters.ParagraphMark:
					if (Iterator.IsParagraphMarkRun())
						return;
					else
						nextState = FormatterState.RowWithTextOnlyAfterFirstLeadingTab;
					break;
				case Characters.SectionMark:
					if (Iterator.IsParagraphMarkRun())
						return;
					else
						nextState = FormatterState.RowWithTextOnlyAfterFirstLeadingTab;
					break;
				case Characters.FloatingObjectMark:
					if (Iterator.IsFloatingObjectAnchorRun())
						nextState = FormatterState.FloatingObject;
					else
						nextState = FormatterState.RowWithTextOnlyAfterFirstLeadingTab;
					break;
				default:
					nextState = FormatterState.RowWithTextOnlyAfterFirstLeadingTab;
					break;
			}
			ChangeState(nextState);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			this.CurrentTab = RowsController.GetNextTab(Formatter);
			return AddBoxResult.Success;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region RowTab
	public class StateRowTab : StateRowTabBase {
		public StateRowTab(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowTab; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTabAfterFloatingObject; } }
		protected internal override void SwitchToNextState() {
			FormatterState nextState;
			switch (Iterator.CurrentChar) {
				case Characters.Space:
				case Characters.EmSpace:
				case Characters.EnSpace:
					nextState = FormatterState.RowSpaces;
					break;
				case Characters.TabMark:
					Formatter.StartNewTab();
					return;
				case Characters.LineBreak:
					nextState = FormatterState.RowLineBreak;
					break;
				case Characters.PageBreak:
					if (RowsController.TablesController.IsInsideTable)
						nextState = FormatterState.RowSpaces;
					else if (!RowsController.SupportsColumnAndPageBreaks)
						nextState = FormatterState.RowLineBreak;
					else
						nextState = FormatterState.RowPageBreak;
					break;
				case Characters.ColumnBreak:
					if (RowsController.TablesController.IsInsideTable)
						nextState = FormatterState.RowSpaces;
					else if (!RowsController.SupportsColumnAndPageBreaks)
						nextState = FormatterState.RowLineBreak;
					else
						nextState = FormatterState.RowColumnBreak;
					break;
				case Characters.ParagraphMark:
					if (Iterator.IsParagraphMarkRun())
						return;
					else
						nextState = FormatterState.RowText;
					break;
				case Characters.SectionMark:
					if (Iterator.IsParagraphMarkRun())
						return;
					else
						nextState = FormatterState.RowText;
					break;
				case Characters.FloatingObjectMark:
					if (Iterator.IsFloatingObjectAnchorRun())
						nextState = FormatterState.FloatingObject;
					else
						nextState = FormatterState.RowText;
					break;
				default:
					nextState = FormatterState.RowText;
					break;
			}
			ChangeState(nextState);
		}
		protected override CompleteFormattingResult FinishParagraph() {
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			ApplyParagraphMarkSize(boxInfo);
			FinalizeParagraph(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			ApplyParagraphMarkSize(boxInfo);
			FinalizeSection(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			FinalizeLine(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			return FinalizePage(boxInfo);
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			return FinalizeColumn(boxInfo);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			this.CurrentTab = RowsController.GetNextTab(Formatter);
			if (!CurrentTab.IsDefault)
				return AddBoxResult.Success;
			if (!RowsController.SuppressHorizontalOverfull &&
				RowsController.IsPositionOutsideRightParagraphBound(CurrentTabPosition + RowsController.CurrentColumn.Bounds.Left))
				return AddBoxResult.HorizontalIntersect;
			return AddBoxResult.Success;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			EndRow();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region RowWithTextOnlyBase (abstract)
	public abstract class StateRowWithTextOnlyBase : BoxFormatterStateBase, ISupportsChangeStateManually {
		protected StateRowWithTextOnlyBase(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected abstract FormatterState HyphenationState { get; }
		protected abstract FormatterState NoHyphenationNextState { get; }
		protected override bool CanUseBox { get { return true; } }
		protected virtual FormatterState DashAfterTextState { get { return FormatterState.RowDashAfterText; } }
		protected override bool IsTerminatorChar(char ch) {
			return FormatterHelper.IsCharSpace(ch) || ch == Characters.TabMark || ch == Characters.LineBreak || ch == Characters.PageBreak || ch == Characters.ColumnBreak || ch == Characters.FloatingObjectMark || FormatterHelper.IsCharDash(ch);
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			Formatter.ApproveFloatingObjects();
			AddTextBox(boxInfo);
			ChangeStateManuallyIfNeeded(boxInfo.IteratorResult);
		}
		protected virtual FormatterState CalcNextState(char currentCharacter) {
			if (FormatterHelper.IsCharSpace(currentCharacter))
				return FormatterState.RowSpaces;
			if (currentCharacter == Characters.TabMark)
				return FormatterState.RowTab;
			if (FormatterHelper.IsCharDash(currentCharacter))
				return DashAfterTextState;
			if (currentCharacter == Characters.LineBreak)
				return FormatterState.RowLineBreak;
			if (currentCharacter == Characters.PageBreak) {
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowSpaces;
				else if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowPageBreak;
			}
			if (currentCharacter == Characters.ColumnBreak) {
				if (RowsController.TablesController.IsInsideTable)
					return FormatterState.RowSpaces;
				else if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowColumnBreak;
			}
			if (currentCharacter == Characters.FloatingObjectMark)
				if (Iterator.IsFloatingObjectAnchorRun())
					return FormatterState.FloatingObject;
				else
					return FormatterState.RowSpaces;
			if (currentCharacter == Characters.ObjectMark && Iterator.IsInlinePictureRun())
				return FormatterState.RowInlineObject;
			Debug.Assert(false);
			Exceptions.ThrowInternalException();
			return FormatterState.Final;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			if (Formatter.SuppressHyphenation)
				ChangeState(NoHyphenationNextState);
			else
				ChangeState(HyphenationState);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishParagraph() {
			Debug.Assert(CurrentRow.Boxes.Count <= 0 || CurrentRow.Height > 0);
			BoxInfo boxInfo = CreateParagraphMarkBoxInfo();
			ApplyParagraphMarkSize(boxInfo);
			FinalizeParagraph(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult FinishSection() {
			Debug.Assert(CurrentRow.Boxes.Count <= 0 || CurrentRow.Height > 0);
			BoxInfo boxInfo = CreateSectionMarkBoxInfo();
			ApplyParagraphMarkSize(boxInfo);
			FinalizeSection(boxInfo);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishLine(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Boxes.Count <= 0 || CurrentRow.Height > 0);
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			Formatter.RowsController.AddBox(ParagraphBoxFormatter.lineBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishPage(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Boxes.Count <= 0 || CurrentRow.Height > 0);
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			Formatter.RowsController.AddBox(ParagraphBoxFormatter.pageBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
			if (result != CompleteFormattingResult.Success)
				return result;
			RowsController.MoveRowToNextPage();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected internal override CompleteFormattingResult FinishColumn(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Boxes.Count <= 0 || CurrentRow.Height > 0);
			CompleteFormattingResult result = RowsController.CompleteCurrentColumnFormatting();
			if (result != CompleteFormattingResult.Success)
				return result;
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			Formatter.RowsController.AddBox(ParagraphBoxFormatter.columnBreakBox, boxInfo);
			Formatter.ApproveFloatingObjects();
			EndRow();
			RowsController.MoveRowToNextColumn();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
		protected bool ShouldChangeStateManually(char currentCharacter) {
			return (!Iterator.IsEnd && IsTerminatorChar(currentCharacter)) || IsInlineObject();
		}
		protected override void ApplyParagraphMarkSize(BoxInfo boxInfo) {
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			else
				base.ApplyParagraphMarkSize(boxInfo);
		}
		protected internal override void ApplyLineBreakMarkSize(BoxInfo boxInfo) {
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			else
				base.ApplyLineBreakMarkSize(boxInfo);
		}
		protected internal override void ApplyPageBreakMarkSize(BoxInfo boxInfo) {
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			else
				base.ApplyPageBreakMarkSize(boxInfo);
		}
		protected internal override void ApplyColumnBreakMarkSize(BoxInfo boxInfo) {
			if (CurrentRow.Boxes.Count <= 0)
				RowsController.UpdateCurrentRowHeight(boxInfo);
			else
				base.ApplyColumnBreakMarkSize(boxInfo);
		}
		public void ChangeStateManuallyIfNeeded(ParagraphIteratorResult iteratorResult) {
			char currentChar = Iterator.CurrentChar;
			if (ShouldChangeStateManually(currentChar)) {
				FormatterState nextState = CalcNextState(currentChar);
				ChangeState(nextState);
			}
		}
	}
	#endregion
	#region RowWithTextOnly
	public class StateRowWithTextOnly : StateRowWithTextOnlyBase {
		public StateRowWithTextOnly(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowWithTextOnly; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowWithTextOnlyAfterFloatingObject; } }
		protected override FormatterState HyphenationState { get { return FormatterState.RowEmptyHyphenation; } }
		protected override FormatterState NoHyphenationNextState { get { return FormatterState.RowTextSplit; } }
		protected override FormatterState DashAfterTextState { get { return FormatterState.RowWithDashAfterTextOnly; } }
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			return Formatter.RollbackToStartOfRow(canFit);
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return CanAddBoxCore(boxInfo);
		}
	}
	#endregion
	#region StateRowWithInlineObjectOnly
	public class StateRowWithInlineObjectOnly : StateRowWithTextOnly {
		public StateRowWithInlineObjectOnly(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected override bool CanUseBox { get { return true; } }
		public override FormatterState Type { get { return FormatterState.RowWithInlineObjectOnly; } }
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (base.IsTerminatorChar(currentCharacter))
				return base.CalcNextState(currentCharacter);
			return FormatterState.RowText;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			AddTextBox(boxInfo);
			Formatter.ApproveFloatingObjects();
			if (!Iterator.IsEnd) {
				EndRow();
				ChangeState(FormatterState.RowEmpty);
			}
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region RowWithTextOnlyAfterFirstLeadingTab
	public class StateRowWithTextOnlyAfterFirstLeadingTab : StateRowWithTextOnly {
		public StateRowWithTextOnlyAfterFirstLeadingTab(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowWithTextOnlyAfterFirstLeadingTab; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowWithTextOnlyAfterFirstLeadingTabAfterFloatingObject; } }
		protected override FormatterState HyphenationState { get { return FormatterState.RowTextHyphenationFirstSyllableAfterFirstLeadingTab; } }
		protected override FormatterState NoHyphenationNextState { get { return FormatterState.RowTextSplitAfterFirstLeadingTab; } }
	}
	#endregion
	#region RowSpaces
	public class StateRowSpaces : BoxFormatterStateBase, ISupportsChangeStateManually {
		public StateRowSpaces(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowSpaces; } }
		protected override bool CanUseBox { get { return true; } }
		protected override void MeasureBoxContent(BoxInfo boxInfo) {
			Formatter.Measurer.MeasureSpaces(boxInfo);
		}
		protected override bool IsTerminatorChar(char ch) {
			return !FormatterHelper.IsCharSpace(ch) || ((ch == Characters.PageBreak || ch == Characters.ColumnBreak) && RowsController.TablesController.IsInsideTable);
		}
		protected override void AddSuccess(BoxInfo boxInfo) {
			Formatter.ApproveFloatingObjects();
			RowsController.AddBox(ParagraphBoxFormatter.GetSpaceBoxTemplate(boxInfo), boxInfo);
			ChangeStateManuallyIfNeeded(boxInfo.IteratorResult);
		}
		protected virtual FormatterState CalcNextState(char currentCharacter) {
			if (currentCharacter == Characters.TabMark)
				return FormatterState.RowTab;
			if (currentCharacter == Characters.LineBreak)
				return FormatterState.RowLineBreak;
			if (currentCharacter == Characters.PageBreak) {
				Debug.Assert(!RowsController.TablesController.IsInsideTable);
				if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowPageBreak;
			}
			if (currentCharacter == Characters.ColumnBreak) {
				Debug.Assert(!RowsController.TablesController.IsInsideTable);
				if (!RowsController.SupportsColumnAndPageBreaks)
					return FormatterState.RowLineBreak;
				else
					return FormatterState.RowColumnBreak;
			}
			else if (currentCharacter == Characters.FloatingObjectMark && Iterator.IsFloatingObjectAnchorRun())
				return FormatterState.FloatingObject;
			else if (FormatterHelper.IsCharDash(currentCharacter))
				return FormatterState.RowDash;
			else if (currentCharacter == Characters.ObjectMark && Iterator.IsInlinePictureRun())
				return FormatterState.RowInlineObject;
			else
				return FormatterState.RowText;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override CompleteFormattingResult ColumnOverfull(CanFitCurrentRowToColumnResult canFit, BoxInfo boxInfo) {
			Debug.Assert(false);
			return CompleteFormattingResult.Success;
		}
		protected override AddBoxResult CanAddBox(BoxInfo boxInfo) {
			return AddBoxResult.Success;
		}
		protected override void ApplyParagraphMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected internal override void ApplyLineBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected internal override void ApplyPageBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected internal override void ApplyColumnBreakMarkSize(BoxInfo boxInfo) {
			Debug.Assert(CurrentRow.Height != 0);
		}
		protected bool ShouldChangeStateManually(char currentCharacter) {
			return !Iterator.IsEnd && IsTerminatorChar(currentCharacter);
		}
		public void ChangeStateManuallyIfNeeded(ParagraphIteratorResult iteratorResult) {
			char currentChar = Iterator.CurrentChar;
			if (ShouldChangeStateManually(currentChar)) {
				FormatterState nextState = CalcNextState(currentChar);
				ChangeState(nextState);
			}
		}
	}
	#endregion
	#region RowText
	public class StateRowText : StateRowWithTextOnly {
		public StateRowText(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowText; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowTextAfterFloatingObject; } }
		protected override FormatterState DashAfterTextState { get { return FormatterState.RowDashAfterText; } }
		protected override FormatterState HyphenationState { get { return FormatterState.RowTextHyphenationFirstSyllable; } }
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			if (Formatter.SuppressHyphenation) {
				return WrapLine();
			}
			else
				return base.HorizontalOverfull(boxInfo);
		}
		protected CompleteFormattingResult WrapLine() {
			Formatter.RollbackToStartOfWord();
			Formatter.RollbackToLastTab();
			RowsController.TryToRemoveLastTabBox();
			if (CurrentRow.Height <= 0) {
				SetCurrentRowHeightToLastBoxHeight();
				CanFitCurrentRowToColumnResult canFit = RowsController.CanFitCurrentRowToColumn();
				if (canFit != CanFitCurrentRowToColumnResult.RowFitted) {
					Formatter.ClearSyllableIterator();
					return Formatter.RollbackToStartOfRow(canFit);
				}
			}
			EndRow();
			ChangeState(FormatterState.RowEmpty);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region StateRowInlineObject
	public class StateRowInlineObject : StateRowText {
		public StateRowInlineObject(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		protected override bool CanUseBox { get { return true; } }
		public override FormatterState Type { get { return FormatterState.RowInlineObject; } }
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (base.IsTerminatorChar(currentCharacter))
				return base.CalcNextState(currentCharacter);
			return FormatterState.RowText;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			return WrapLine();
		}
	}
	#endregion
	#region RowDashAfterText
	public class StateRowDashAfterText : StateRowText {
		public StateRowDashAfterText(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowText; } }
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (FormatterHelper.IsCharDash(currentCharacter))
				return FormatterState.RowDash;
			if (base.IsTerminatorChar(currentCharacter))
				return base.CalcNextState(currentCharacter);
			else
				return FormatterState.RowText;
		}
	}
	#endregion
	#region RowWithDashAfterTextOnly
	public class StateRowWithDashAfterTextOnly : StateRowWithTextOnly {
		public StateRowWithDashAfterTextOnly(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowWithDashAfterTextOnly; } }
		protected override bool IsTerminatorChar(char ch) {
			return true;
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (FormatterHelper.IsCharDash(currentCharacter))
				return FormatterState.RowDash;
			if (base.IsTerminatorChar(currentCharacter))
				return base.CalcNextState(currentCharacter);
			else
				return FormatterState.RowText;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			ChangeState(FormatterState.RowDashSplit);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region RowDash
	public class StateRowDash : StateRowText {
		public StateRowDash(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowDash; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowDashAfterFloatingObject; } }
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			ChangeState(FormatterState.RowDashSplit);
			return CompleteFormattingResult.Success;
		}
		protected override bool IsTerminatorChar(char ch) {
			return !FormatterHelper.IsCharDash(ch) || ((ch == Characters.PageBreak || ch == Characters.ColumnBreak) && RowsController.TablesController.IsInsideTable);
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (base.IsTerminatorChar(currentCharacter))
				return base.CalcNextState(currentCharacter);
			else
				return FormatterState.RowText;
		}
	}
	#endregion
	#region RowWithDashOnly
	public class StateRowWithDashOnly : StateRowWithTextOnly {
		public StateRowWithDashOnly(ParagraphBoxFormatter formatter)
			: base(formatter) {
		}
		public override FormatterState Type { get { return FormatterState.RowWithDashOnly; } }
		public override FormatterState StateAfterFloatingObject { get { return FormatterState.RowWithDashOnlyAfterFloatingObject; } }
		protected override bool IsTerminatorChar(char ch) {
			return !FormatterHelper.IsCharDash(ch) || ((ch == Characters.PageBreak || ch == Characters.ColumnBreak) && RowsController.TablesController.IsInsideTable);
		}
		protected override FormatterState CalcNextState(char currentCharacter) {
			if (base.IsTerminatorChar(currentCharacter))
				return base.CalcNextState(currentCharacter);
			else
				return FormatterState.RowText;
		}
		protected override CompleteFormattingResult HorizontalOverfull(BoxInfo boxInfo) {
			Formatter.RollbackToStartOfWord();
			ChangeState(FormatterState.RowDashSplit);
			return CompleteFormattingResult.Success;
		}
	}
	#endregion
	#region ParagraphFinalFormatter
	public class ParagraphFinalFormatter {
		#region Fields
		PieceTable pieceTable;
		Paragraph currentParagraph;
		Column currentColumn;
		LineSpacingCalculatorBase lineSpacingCalculator;
		ParagraphAlignmentCalculatorBase alignmentCalculator;
		readonly UnderlineCalculator underlineCalculator;
		readonly StrikeoutCalculator strikeoutCalculator;
		readonly BookmarkBoxCalculator bookmarkCalculator;
		readonly RangePermissionBoxCalculator rangePermissionCalculator;
		readonly CommentBoxCalculator commentCalculator;
		readonly CustomMarkBoxCalculator customMarkCalculator;
		readonly DocumentLayout documentLayout;
		readonly RangePermissionColorer rangePermissionColorer;
		readonly CommentColorer commentColorer;
		int currentCommentCount;
		CommentPadding commentPadding;
		#endregion
		public ParagraphFinalFormatter(DocumentLayout documentLayout, CommentPadding commentSkinPadding) {
			Guard.ArgumentNotNull(documentLayout, "documentLayout");
			Guard.ArgumentNotNull(commentSkinPadding, "commentSkinPadding");
			this.pieceTable = documentLayout.DocumentModel.MainPieceTable;
			this.underlineCalculator = new UnderlineCalculator(PieceTable);
			this.strikeoutCalculator = new StrikeoutCalculator(PieceTable);
			this.documentLayout = documentLayout;
			this.bookmarkCalculator = new BookmarkBoxCalculator(PieceTable, documentLayout.MeasurerProvider);
			this.rangePermissionCalculator = new RangePermissionBoxCalculator(PieceTable, documentLayout.MeasurerProvider);
			this.commentCalculator = new CommentBoxCalculator(PieceTable, documentLayout.MeasurerProvider);
			this.customMarkCalculator = new CustomMarkBoxCalculator(PieceTable, documentLayout.MeasurerProvider);
			this.rangePermissionColorer = new RangePermissionColorer();
			this.commentColorer = new CommentColorer(PieceTable.DocumentModel.CommentOptions);
			this.currentCommentCount = 0;
			this.commentPadding = commentSkinPadding;
		}
		protected DocumentModel DocumentModel { get { return pieceTable.DocumentModel; } }
		protected internal PieceTable PieceTable {
			get { return pieceTable; }
			set {
				Guard.ArgumentNotNull(pieceTable, "pieceTable");
				if (pieceTable == value)
					return;
				pieceTable = value;
				underlineCalculator.PieceTable = value;
				strikeoutCalculator.PieceTable = pieceTable;
				bookmarkCalculator.PieceTable = pieceTable;
				rangePermissionCalculator.PieceTable = pieceTable;
				commentCalculator.PieceTable = pieceTable;
				customMarkCalculator.PieceTable = pieceTable;
			}
		}
		protected internal BookmarkBoxCalculator BookmarkCalculator { get { return bookmarkCalculator; } }
		protected internal CustomMarkBoxCalculator CustomMarkCalculator { get { return customMarkCalculator; } }
		protected internal RangePermissionBoxCalculator RangePermissionCalculator { get { return rangePermissionCalculator; } }
		protected internal CommentBoxCalculator CommentCalculator { get { return commentCalculator; } }
		internal RangePermissionColorer RangePermissionColorer { get { return rangePermissionColorer; } }
		internal CommentColorer CommentColorer { get { return commentColorer; } }
		public void FormatPages(PageCollection pages) {
			pages.ForEach(FormatPage);
		}
		public void FormatPage(Page page) {
			PieceTable mainPieceTable = PieceTable;
			if (page.Header != null) {
				PieceTable = page.Header.Header.PieceTable;
				FormatPageArea(page.Header);
			}
			if (page.Footer != null) {
				PieceTable = page.Footer.Footer.PieceTable;
				FormatPageArea(page.Footer);
			}
			PieceTable = mainPieceTable;
			page.Areas.ForEach(FormatPageArea);
			FormateTextBoxes(page.InnerBackgroundFloatingObjects);
			FormateTextBoxes(page.InnerFloatingObjects);
			FormateTextBoxes(page.InnerForegroundFloatingObjects);
			List<Comment> comments = CalculateCommet(mainPieceTable, page);
			page.EnsureCommentBounds(page, DocumentModel);
			int count = comments.Count;
			if (count > 0) {
				for (int i = 0; i < count; i++) {
					currentCommentCount += 1;
					CommentViewInfo commentViewInfo = new CommentViewInfo();
					FormatComments(page, comments[i].Content, commentViewInfo, mainPieceTable);
				}
			}
			count = page.Comments.Count;
			if (count > 0) {
				CommentSizeCalculator calculator = new CommentSizeCalculator();
				calculator.CalculateCommentClientBounds(page.Comments, page.ClientBounds, page.CommentBounds, commentPadding);
			}
		}
		void FormateTextBoxes(List<FloatingObjectBox> list) {
			if (list == null || list.Count == 0)
				return;
			foreach (FloatingObjectBox box in list)
				FormatTextBox(box);
		}
		void FormatTextBox(FloatingObjectBox box) {
			DocumentLayout documentLayout = box.DocumentLayout;
			if (documentLayout == null)
				return;
			PageCollection pages = documentLayout.Pages;
			if (pages.Count == 0)
				return;
			PageAreaCollection areas = pages[0].Areas;
			if (areas.Count == 0)
				return;
			ColumnCollection columns = areas[0].Columns;
			if (columns.Count == 0)
				return;
			RowCollection rows = columns[0].Rows;
			int count = rows.Count;
			for (int i = 0; i < count; i++) {
				Row row = rows[i];
				if (row.ShouldProcessLayoutDependentText) {
					if (row.Paragraph != currentParagraph)
						UpdateCurrentParagraph(row.Paragraph);
					FloatingObjectAnchorRun run = box.GetFloatingObjectRun();
					TextBoxFloatingObjectContent contentType = run.Content as TextBoxFloatingObjectContent;
					if (contentType != null) {
						PieceTable oldPieceTable = pieceTable;
						pieceTable = contentType.TextBox.PieceTable;
						try {
							UpdateLayoutDependentBox(row);
							FormatCore(row);
						}
						finally {
							pieceTable = oldPieceTable;
						}
					}
				}
			}
		}
		List<Comment> CalculateCommet(PieceTable mainPieceTable, Page page) {
			List<Comment> result = new List<Comment>();
			DocumentLogPosition start = page.GetFirstPosition(mainPieceTable).LogPosition;
			DocumentLogPosition end = page.GetLastPosition(mainPieceTable).LogPosition;
			CommentBoundaryIterator iterator = new CommentBoundaryIterator(mainPieceTable);
			for (; !iterator.IsDone(); iterator.MoveNext()) {
				Comment comment = (Comment)iterator.Current.VisitableInterval;
				if ((comment.End >= start) && (comment.End <= end) && !result.Contains(comment) && !PageContainsComment(page, comment) && CommentAuthorVisibity(comment))
					result.Add(comment);
			}
			result.Sort(CompareCommentEnd);
			return result;
		}
		bool PageContainsComment(Page page, Comment comment) {
			int count = page.Comments.Count;
			if (count == 0)
				return false;
			for (int i = 0; i < count; i++)
				if (page.Comments[i].Comment == comment)
					return true;
			return false;
		}
		bool CommentAuthorVisibity(Comment comment) {
			if (comment.ParentComment == null)
				return DocumentModel.CommentOptions.VisibleAuthors.Contains(comment.Author);
			return (DocumentModel.CommentOptions.VisibleAuthors.Contains(comment.Author) && DocumentModel.CommentOptions.VisibleAuthors.Contains(comment.ParentComment.Author));
		}
		int CompareCommentEnd(Comment comment1, Comment comment2) {
			return comment1.End - comment2.End;
		}
		void FormatComments(Page page, CommentContentType content, CommentViewInfo commentViewInfo, PieceTable pieceTable) {
			using (CommentContentPrinter printer = new CommentContentPrinter(content, page, this.documentLayout.Measurer)) {
				printer.ColumnLocation = new Point(0, 0);
				int commentLeft = page.CommentBounds.Left + commentPadding.CommentLeft;
				int commentRight = page.CommentBounds.Right - commentPadding.CommentRight;
				int contentLeft = commentLeft + commentPadding.ContentLeft;
				int contentTop = page.ClientBounds.Top;
				int contentWidth = commentRight - commentPadding.ContentRight - contentLeft;
				int maxContentHeight = Int32.MaxValue / 4;
				printer.CommentHeading = ShapeHeadComment(content.ReferenceComment);
				printer.ColumnSize = new Size(contentWidth, maxContentHeight);
				int actualSize = printer.Format(printer.ColumnLocation.Y + printer.ColumnSize.Height);
				actualSize -= printer.ColumnLocation.Y;
				commentViewInfo.ActualSize = actualSize;
				commentViewInfo.CommentDocumentLayout = printer.DocumentLayout;
				commentViewInfo.Comment = content.ReferenceComment;
				commentViewInfo.CommentHeading = ShapeHeadComment(commentViewInfo.Comment);
				if (content.ReferenceComment.ParentComment == null)
					commentViewInfo.Bounds = new Rectangle(commentLeft, contentTop, commentRight - commentLeft, actualSize + commentPadding.ContentTop + commentPadding.ContentBottom);
				else
					commentViewInfo.Bounds = new Rectangle(commentLeft + commentPadding.CommentLeft, contentTop, commentRight - commentLeft - commentPadding.CommentLeft, actualSize + commentPadding.ContentTop + commentPadding.ContentBottom);
				commentViewInfo.OriginalHeight = commentViewInfo.Bounds.Height;
				commentViewInfo.ContentBounds = new Rectangle(contentLeft, contentTop, contentWidth, actualSize);
				commentViewInfo.CommentHeadingBounds = printer.CommentHeadingBounds;
				commentViewInfo.CommentHeadingFontInfo = printer.CommentHeadingFontInfo;
				commentViewInfo.CommentMoreButtonBounds = new Rectangle(0, 0, 0, 0);
				commentViewInfo.CommentContainTableInFirstRow = false;
				commentViewInfo.CommentHeadingOffset = printer.CommentHeadingOffset;
				commentViewInfo.LastVisiblePosition = commentViewInfo.Comment.Content.PieceTable.DocumentEndLogPosition;
				commentViewInfo.CommentDocumentLayout.Pages.First.Areas.First.Bounds = commentViewInfo.CommentDocumentLayout.Pages.First.Areas.First.Columns.First.Bounds;
				CalculateCommentViewInfoCharacter(pieceTable, page, commentViewInfo);
				commentViewInfo.CommentViewInfoPage = page;
				page.Comments.Add(commentViewInfo);
			}
		}
		string ShapeHeadComment(Comment sourceComment) {
			return String.Format(DocumentModel.CommentHeading + ": ", sourceComment.Name, sourceComment.Index + 1);
		}
		void CalculateCommentViewInfoCharacter(PieceTable pieceTable, Page page, CommentViewInfo commentViewInfo) {
			DocumentLogPosition logPosition = commentViewInfo.Comment.End;
			DocumentLayoutPosition position = new DocumentLayoutPosition(this.documentLayout, pieceTable, logPosition);
			DocumentLayoutDetailsLevel detailsLevel = DocumentLayoutDetailsLevel.Character;
			position.Page = page;
			position.IncreaseDetailsLevel(DocumentLayoutDetailsLevel.Page);
			position.SuppressSuspendFormatting = true;
			DocumentLayoutDetailsLevel resulLevel = position.UpdatePageAreaRecursive(page, 0, detailsLevel);
			position.SuppressSuspendFormatting = false;
			if (resulLevel >= detailsLevel)
				commentViewInfo.Character = position.Character;
			else
				Exceptions.ThrowArgumentException("position.resultLevel", resulLevel);
		}
		public void FormatPageArea(PageArea pageArea) {
			pageArea.Columns.ForEach(FormatColumn);
			pageArea.LineNumbers.ForEach(AlignLineNumberToBaseLine);
		}
		public void FormatColumn(Column column) {
			this.currentParagraph = null;
			this.currentColumn = column;
			Format(column.Rows);
			if (column.InnerTables != null)
				ApplyTableCellsVerticalContentAlignment(column.InnerTables);
			if (column.ShouldProcessParagraphFrames)
				FormatParagraphFrames(column.InnerParagraphFrames);
			this.currentColumn = null;
		}
		void Format(RowCollection rows) {
			int count = rows.Count;
			for (int i = 0; i < count; i++)
				Format(rows[i]);
		}
		void Format(Row row) {
			if (row.Paragraph != currentParagraph)
				UpdateCurrentParagraph(row.Paragraph);
			if (row.ShouldProcessLayoutDependentText)
				UpdateLayoutDependentBox(row);
			FormatCore(row);
		}
		void FormatCore(Row row) {
			lineSpacingCalculator.FormatRow(this, row);
			alignmentCalculator.AlignRow(row);
			if (row.ShouldProcessCharacterLines) {
				underlineCalculator.Calculate(row);
				strikeoutCalculator.Calculate(row);
			}
			Rectangle tightRowBounds = CalculateTightRowBounds(row);
			if (row.ShouldProcessTextHighlight)
				CalculateBackgroundHighlight(row, tightRowBounds);
			CalculateFieldsHighlight(row, tightRowBounds);
			if (row.ShouldProcessHiddenText)
				CalculateHiddenTextBoxes(row);
			if (row.ShouldProcessSpecialTextBoxes)
				CalculateSpecialTextBoxes(row);
			bookmarkCalculator.Calculate(row);
			rangePermissionCalculator.Calculate(row);
			CalculateRangePermissionHighlight(row, tightRowBounds);
			commentCalculator.Calculate(row);
			CalculateCommentHighlight(row, tightRowBounds);
			CalculateCustomMarkBounds(row);
		}
		void AlignLineNumberToBaseLine(LineNumberBox box) {
			Row row = box.Row;
			Rectangle r = box.Bounds;
			r.Y = row.Bounds.Top + row.BaseLineOffset - box.CalcAscentAndFree(PieceTable);
			box.Bounds = r;
		}
		protected virtual void CalculateCustomMarkBounds(Row row) {
			customMarkCalculator.Calculate(row);
		}
		void UpdateLayoutDependentBox(Row row) {
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			BoxMeasurer measurer = this.documentLayout.Measurer;
			for (int i = 0; i < count; i++) {
				LayoutDependentTextBox box = boxes[i] as LayoutDependentTextBox;
				if (box == null)
					continue;
				LayoutDependentTextRun run = (LayoutDependentTextRun)PieceTable.Runs[box.StartPos.RunIndex];
				if (!run.FieldResultFormatting.RecalculateOnSecondaryFormatting)
					continue;
				PieceTable oldPieceTable = measurer.PieceTable;
				try {
					measurer.PieceTable = pieceTable;
					box.CalculatedText = run.FieldResultFormatting.GetValue(null, DocumentModel);
					SubstituteFont(box); 
					BoxInfo boxInfo = new BoxInfo() { Box = box, StartPos = box.StartPos, EndPos = box.EndPos };
					measurer.MeasureText(boxInfo);
					Rectangle bounds = box.Bounds;
					bounds.Size = boxInfo.Size;
					box.Bounds = bounds;
				}
				finally {
					measurer.PieceTable = oldPieceTable;
				}
			}
		}
		void SubstituteFont(LayoutDependentTextBox box) {
			FontCache fontCache = DocumentModel.FontCache;
			FontInfo sourceFontInfo = box.GetFontInfo(PieceTable);
			string sourceFontName = sourceFontInfo.Name;
			FontCharacterSet sourceFontCharacterSet = fontCache.GetFontCharacterSet(sourceFontName);
			if (sourceFontCharacterSet == null)
				return;
			char ch = box.CalculatedText[0];
			if (!sourceFontCharacterSet.ContainsChar(ch)) {
				string substFontName = fontCache.FindSubstituteFont(sourceFontName, ch);
				box.GetRun(PieceTable).FontName = substFontName;
			}
		}
		public virtual int CalcBoxDescent(Box box) {
			return box.CalcDescent(PieceTable);
		}
		public virtual int CalcBoxAscentAndFree(Box box) {
			return box.CalcAscentAndFree(PieceTable);
		}
		public virtual int CalcBaseBoxDescent(Box box) {
			return box.CalcBaseDescent(PieceTable);
		}
		public virtual int CalcBaseBoxAscentAndFree(Box box) {
			return box.CalcBaseAscentAndFree(PieceTable);
		}
		public virtual int CalculateSubscriptOrSuperScriptOffset(Box box) {
			switch (box.GetRun(PieceTable).Script) {
				case CharacterFormattingScript.Normal:
					return 0;
				case CharacterFormattingScript.Subscript:
					return box.GetFontInfo(PieceTable).SubscriptOffset.Y;
				case CharacterFormattingScript.Superscript:
					return box.GetFontInfo(PieceTable).SuperscriptOffset.Y;
				default:
					Exceptions.ThrowInternalException();
					return 0;
			}
		}
		void UpdateCurrentParagraph(Paragraph paragraph) {
			this.currentParagraph = paragraph;
			this.lineSpacingCalculator = LineSpacingCalculatorBase.Create(currentParagraph);
			this.alignmentCalculator = ParagraphAlignmentCalculatorBase.GetAlignmentCalculator(paragraph.Alignment);
		}
		protected internal virtual Rectangle CalculateTightRowBounds(Row row) {
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			if (count <= 0)
				return row.Bounds;
			int minY = int.MaxValue;
			int maxY = int.MinValue;
			int baselinePosition = row.Bounds.Top + row.BaseLineOffset;
			for (int i = 0; i < count; i++) {
				Box box = boxes[i];
				if (box.IsNotWhiteSpaceBox && !box.IsLineBreak) {
					FontInfo fontInfo = box.GetFontInfo(PieceTable);
					int ascentAndFree = fontInfo.AscentAndFree;
					int descent = fontInfo.Descent;
					minY = Math.Min(minY, baselinePosition - ascentAndFree);
					maxY = Math.Max(maxY, baselinePosition + descent);
				}
			}
			if (minY == int.MaxValue || maxY == int.MinValue)
				return row.Bounds;
			return new Rectangle(row.Bounds.Left, minY, row.Bounds.Width, maxY - minY);
		}
		protected internal virtual void CalculateRangePermissionHighlight(Row row, Rectangle tightRowBounds) {
			row.ClearRangePermissionHighlightAreas();
			if (DocumentModel.RangePermissionOptions.Visibility == RichEditRangePermissionVisibility.Hidden)
				return;
			RunIndex startRunIndex = row.Boxes.First.StartPos.RunIndex;
			RunIndex endRunIndex = row.Boxes.Last.EndPos.RunIndex;
			int index = 0;
			RangePermissionCollection rangePermissions = PieceTable.RangePermissions;
			int count = rangePermissions.Count;
			for (; index < count; index++) {
				RangePermission rangePermission = rangePermissions[index];
				if (DocumentModel.ProtectionProperties.EnforceProtection && !PieceTable.IsPermissionGranted(rangePermission))
					continue;
				FormatterPosition startPosition = new FormatterPosition(rangePermission.Interval.Start.RunIndex, rangePermission.Interval.Start.RunOffset, 0);
				FormatterPosition endPosition = new FormatterPosition(rangePermission.Interval.End.RunIndex, rangePermission.Interval.End.RunOffset, 0);
				if (endPosition >= row.Boxes.First.StartPos && startPosition <= row.Boxes.Last.EndPos) {
					RunIndex start = Algorithms.Max(startRunIndex, rangePermission.Interval.NormalizedStart.RunIndex);
					RunIndex end = Algorithms.Min(endRunIndex, rangePermission.Interval.NormalizedEnd.RunIndex);
					row.RangePermissionHighlightAreas.Add(CreateRangePermissionHighlightAreaCore(row, start, end, tightRowBounds, rangePermission));
				}
			}
		}
		protected internal virtual void CalculateCommentHighlight(Row row, Rectangle tightRowBounds) {
			row.ClearCommentHighlightAreas();
			if ((DocumentModel.CommentOptions.Visibility != RichEditCommentVisibility.Visible) && !DocumentModel.CommentOptions.HighlightCommentedRange)
				return;
			RunIndex startRunIndex = row.Boxes.First.StartPos.RunIndex;
			RunIndex endRunIndex = row.Boxes.Last.EndPos.RunIndex;
			int index = 0;
			CommentCollection comments = PieceTable.Comments;
			int count = comments.Count;
			for (; index < count; index++) {
				Comment comment = comments[index];
				if (DocumentModel.ProtectionProperties.EnforceProtection || !DocumentModel.CommentOptions.VisibleAuthors.Contains(comment.Author))
					continue;
				FormatterPosition startPosition = new FormatterPosition(comment.Interval.Start.RunIndex, comment.Interval.Start.RunOffset, 0);
				FormatterPosition endPosition = new FormatterPosition(comment.Interval.End.RunIndex, comment.Interval.End.RunOffset, 0);
				if (endPosition >= row.Boxes.First.StartPos && startPosition <= row.Boxes.Last.EndPos) {
					RunIndex start = Algorithms.Max(startRunIndex, comment.Interval.NormalizedStart.RunIndex);
					RunIndex end = Algorithms.Min(endRunIndex, comment.Interval.NormalizedEnd.RunIndex);
					HighlightArea area = CreateCommentHighlightAreaCore(row, start, end, tightRowBounds, comment);
					if (area != null)
						row.CommentHighlightAreas.Add(area);
				}
			}
		}
		protected internal virtual void CalculateFieldsHighlight(Row row, Rectangle tightRowBounds) {
			row.ClearFieldHighlightAreas();
			if (DocumentModel.FieldOptions.HighlightMode != FieldsHighlightMode.Always)
				return;
			int index = LookupFirstFieldIndexByRunIndex(row, row.Boxes.First.StartPos.RunIndex);
			if (index < 0)
				return;
			RunIndex startRunIndex = row.Boxes.First.StartPos.RunIndex;
			RunIndex endRunIndex = row.Boxes.Last.EndPos.RunIndex;
			FieldCollection fields = PieceTable.Fields;
			int count = fields.Count;
			for (; index < count; index++) {
				index = fields.LookupParentFieldIndex(index);
				Field field = fields[index];
				if (field.FirstRunIndex > endRunIndex)
					break;
				if (field.LastRunIndex >= startRunIndex && field.FirstRunIndex <= endRunIndex) {
					RunIndex start = Algorithms.Max(startRunIndex, field.FirstRunIndex);
					RunIndex end = Algorithms.Min(endRunIndex, field.LastRunIndex);
					Color color = DocumentModel.FieldOptions.HighlightColor;
					HighlightArea area = CreateFieldHighlightAreaCore(row, start, end, tightRowBounds, color);
					if (area != null)
						row.FieldHighlightAreas.Add(area);
				}
			}
		}
		protected internal virtual int LookupFirstFieldIndexByRunIndex(Row row, RunIndex runIndex) {
			FieldCollection fields = PieceTable.Fields;
			FieldRunIndexComparable comparator = new FieldRunIndexComparable(runIndex);
			int index = Algorithms.BinarySearch(fields, comparator);
			if (index < 0)
				index = ~index;
			if (index >= fields.Count)
				return -1;
			else
				return index;
		}
		protected internal virtual HighlightArea CreateFieldHighlightAreaCore(Row row, RunIndex start, RunIndex end, Rectangle tightRowBounds, Color color) {
			BoxCollection boxes = row.Boxes;
			int firstBoxIndex = FindBoxIndex(boxes, 0, start);
			Box firstBox = boxes[firstBoxIndex];
			if (firstBox.StartPos.RunIndex > end)
				return null;
			int lastBoxIndex = FindBoxIndex(boxes, firstBoxIndex + 1, end + 1) - 1;
			int left = firstBox.Bounds.Left;
			int right = boxes[lastBoxIndex].Bounds.Right;
			return new HighlightArea(color, new Rectangle(left, tightRowBounds.Top, right - left, tightRowBounds.Height));
		}
		protected internal virtual HighlightArea CreateRangePermissionHighlightAreaCore(Row row, RunIndex start, RunIndex end, Rectangle tightRowBounds, RangePermission rangePermission) {
			BookmarkBoxCollection rangePermissionBoxes = row.InnerRangePermissionBoxes;
			BoxCollection boxes = row.Boxes;
			int firstBoxIndex = FindBoxIndex(boxes, 0, start);
			int lastBoxIndex = FindBoxIndex(boxes, firstBoxIndex + 1, end + 1) - 1;
			int left = boxes[firstBoxIndex].Bounds.Left;
			int right = boxes[lastBoxIndex].Bounds.Right;
			int rangePermissionBoxesCount = rangePermissionBoxes == null ? 0 : rangePermissionBoxes.Count;
			if (rangePermissionBoxesCount > 0) {
				VisitableDocumentIntervalBox startBox = null;
				VisitableDocumentIntervalBox endBox = null;
				for (int i = 0; i < rangePermissionBoxesCount; i++) {
					VisitableDocumentIntervalBox itemBox = rangePermissionBoxes[i];
					if (itemBox.Interval == rangePermission) {
						if (itemBox is BookmarkStartBox)
							startBox = itemBox;
						else
							endBox = itemBox;
					}
				}
				if (startBox != null)
					left = startBox.HorizontalPosition;
				if (endBox != null)
					right = endBox.HorizontalPosition;
			}
			Color color = DocumentModel.RangePermissionOptions.HighlightColor;
			if (!DocumentModel.ProtectionProperties.EnforceProtection)
				color = RangePermissionColorer.GetColor(rangePermission);
			return new HighlightArea(color, new Rectangle(left, tightRowBounds.Top, right - left, tightRowBounds.Height));
		}
		protected internal virtual HighlightArea CreateCommentHighlightAreaCore(Row row, RunIndex start, RunIndex end, Rectangle tightRowBounds, Comment comment) {
			BookmarkBoxCollection commentBoxes = row.InnerCommentBoxes;
			BoxCollection boxes = row.Boxes;
			int firstBoxIndex = FindBoxIndex(boxes, 0, start);
			int lastBoxIndex = FindBoxIndex(boxes, firstBoxIndex + 1, end + 1) - 1;
			int left = boxes[firstBoxIndex].Bounds.Left;
			int right = boxes[lastBoxIndex].Bounds.Right;
			for (int j = lastBoxIndex; j >= firstBoxIndex; j--)
				if (boxes[j].IsNotWhiteSpaceBox && !boxes[j].IsLineBreak) {
					right = boxes[j].Bounds.Right;
					break;
				}
			if (boxes[firstBoxIndex].IsLineBreak)
				right = boxes[firstBoxIndex].Bounds.Left;
			int commentBoxesCount = commentBoxes == null ? 0 : commentBoxes.Count;
			if (commentBoxesCount > 0) {
				VisitableDocumentIntervalBox startBox = null;
				VisitableDocumentIntervalBox endBox = null;
				for (int i = 0; i < commentBoxesCount; i++) {
					VisitableDocumentIntervalBox itemBox = commentBoxes[i];
					if (itemBox.Interval == comment) {
						if (itemBox is CommentStartBox)
							startBox = itemBox;
						else
							endBox = itemBox;
					}
				}
				if (startBox != null)
					left = startBox.HorizontalPosition;
				if (endBox != null)
					right = endBox.HorizontalPosition;
			}
			Color color = DocumentModel.CommentOptions.Color;
			if (color == DXColor.Empty)
				color = DocumentModel.CommentColorer.GetColor(comment);
			if (right == left)
				return null;
			else
				return new HighlightArea(color, new Rectangle(left, tightRowBounds.Top, right - left, tightRowBounds.Height));
		}
		protected internal virtual int FindBoxIndex(BoxCollection boxes, int from, RunIndex indexToSearch) {
			int count = boxes.Count;
			for (int i = from; i < count; i++) {
				if (boxes[i].StartPos.RunIndex >= indexToSearch)
					return i;
			}
			return count;
		}
		protected internal virtual void CalculateBackgroundHighlight(Row row, Rectangle tightRowBounds) {
			row.ClearHighlightAreas();
			Debug.Assert(row.Boxes.Count > 0);
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			RunIndex startRunIndex = boxes[0].StartPos.RunIndex;
			Box startBox = boxes[0];
			Box lastProcessedBox = null;
			for (int i = 1; i < count; i++) {
				Box currentBox = boxes[i];
				RunIndex currentRunIndex = currentBox.StartPos.RunIndex;
				bool isSolidBoxInterval = startRunIndex == currentRunIndex;
				if (isSolidBoxInterval && i < count - 1)
					continue;
				IHighlightableTextRun run = PieceTable.Runs[startRunIndex] as IHighlightableTextRun;
				startRunIndex = currentRunIndex;
				Box prevStartBox = startBox;
				startBox = currentBox;
				if (run == null)
					continue;
				Color backColor = run.BackColor;
				if (DXColor.IsTransparentOrEmpty(backColor))
					continue;
				Box prevBox = boxes[i - 1];
				if (isSolidBoxInterval && !(currentBox is PageBreakBox))
					prevBox = currentBox;
				lastProcessedBox = prevBox;
				int left = prevStartBox.Bounds.Left;
				int width = prevBox.Bounds.Right - left;
				Rectangle bounds = new Rectangle(left, tightRowBounds.Top, width, tightRowBounds.Height);
				row.HighlightAreas.Add(new HighlightArea(backColor, bounds));
			}
			Box lastBox = boxes.Last;
			if (lastProcessedBox != lastBox && !(lastBox is PageBreakBox)) {
				RunIndex lastBoxRunIndex = lastBox.StartPos.RunIndex;
				IHighlightableTextRun run = PieceTable.Runs[lastBoxRunIndex] as IHighlightableTextRun;
				if (run == null || DXColor.IsTransparentOrEmpty(run.BackColor))
					return;
				Rectangle lastBoxBounds = lastBox.Bounds;
				int left = lastBoxBounds.Left;
				int width = lastBoxBounds.Right - left;
				Rectangle bounds = new Rectangle(left, tightRowBounds.Top, width, tightRowBounds.Height);
				row.HighlightAreas.Add(new HighlightArea(run.BackColor, bounds));
			}
		}
		protected internal virtual void CalculateHiddenTextBoxes(Row row) {
			row.ClearHiddenTextBoxes();
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			int hiddenBoxesCount = 0;
			for (int i = 0; i < count; i++) {
				RunIndex runIndex = boxes[i].StartPos.RunIndex;
				if (PieceTable.Runs[runIndex].Hidden && i < count - 1) {
					hiddenBoxesCount++;
					continue;
				}
				if (hiddenBoxesCount == 0)
					continue;
				Box startBox = boxes[i - hiddenBoxesCount];
				int start = startBox.Bounds.Left;
				DevExpress.Office.Drawing.FontInfo fontInfo = startBox.GetFontInfo(PieceTable);
				int bottomOffset = fontInfo.UnderlinePosition;
				int end = boxes[i].Bounds.Left;
				HiddenTextUnderlineBox box = new HiddenTextUnderlineBox(start, end, bottomOffset);
				row.HiddenTextBoxes.Add(box);
				hiddenBoxesCount = 0;
			}
		}
		protected internal virtual void CalculateSpecialTextBoxes(Row row) {
			row.ClearSpecialTextBoxes();
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			for (int i = 0; i < count; i++) {
				SpecialTextBox specialTextBox = boxes[i] as SpecialTextBox;
				if (specialTextBox != null)
					row.SpecialTextBoxes.Add(specialTextBox);
			}
		}
		protected internal virtual void FormatParagraphFrames(ParagraphFrameBoxCollection paragraphFrames) {
			ParagraphFrameBox nextBox = null;
			int count = paragraphFrames.Count;
			for (int i = count - 1; i >= 0; i--) {
				FormatParagraphFrame(paragraphFrames[i], nextBox);
				nextBox = paragraphFrames[i];
			}
		}
		protected internal virtual void FormatParagraphFrame(ParagraphFrameBox paragraphFrameBox, ParagraphFrameBox nextParagraphFrameBox) {
			RowCollection rows = currentColumn.Rows;
			ParagraphIndex paragraphIndex = paragraphFrameBox.ParagraphIndex;
			int firstRowIndex = FindRowByParagraphIndex(rows, paragraphIndex);
			if (firstRowIndex < 0)
				return;
			paragraphFrameBox.FirstRow = rows[firstRowIndex];
			int top = CalculateRowBoxesTop(paragraphFrameBox.FirstRow);
			TableCellRow tableCellRow = paragraphFrameBox.FirstRow as TableCellRow;
			Row row = CalculateBottomRow(rows, paragraphIndex + 1, tableCellRow);
			int bottom = CalculateBottomPosition(paragraphFrameBox, nextParagraphFrameBox, row, tableCellRow);
			Rectangle currentColumnBounds;
			if (tableCellRow == null)
				currentColumnBounds = currentColumn.Bounds;
			else
				currentColumnBounds = tableCellRow.CellViewInfo.GetBounds();
			Paragraph paragraph = paragraphFrameBox.GetParagraph();
			int leftIndent = DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.LeftIndent);
			if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging && paragraph.FirstLineIndent > 0)
				leftIndent -= DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.FirstLineIndent);
			currentColumnBounds.X += leftIndent;
			currentColumnBounds.Width -= leftIndent;
			currentColumnBounds.Width -= DocumentModel.ToDocumentLayoutUnitConverter.ToLayoutUnits(paragraph.RightIndent);
			currentColumnBounds.Width = Math.Max(0, currentColumnBounds.Width);
			paragraphFrameBox.Bounds = new Rectangle(currentColumnBounds.Left, top, currentColumnBounds.Width, bottom - top);
			if (paragraphFrameBox.ContentBounds.IsEmpty)
				paragraphFrameBox.ContentBounds = paragraphFrameBox.Bounds;
		}
		protected internal virtual Row CalculateBottomRow(RowCollection rows, ParagraphIndex paragraphIndex, TableCellRow cellRow) {
			int lastRowIndex = FindRowByParagraphIndex(rows, paragraphIndex);
			if (lastRowIndex >= 0)
				return rows[lastRowIndex - 1];
			if (lastRowIndex < 0 && cellRow != null) {
				TableCellViewInfo cellViewInfo = cellRow.CellViewInfo;
				return cellViewInfo.GetLastRow(cellViewInfo.TableViewInfo.Column);
			}
			return rows[rows.Count - 1];
		}
		protected internal virtual int CalculateBottomPosition(ParagraphFrameBox frameBox, ParagraphFrameBox nextFrameBox, Row row, TableCellRow cellRow) {
			if (ShouldMergeParagraphFrameBoxes(frameBox, nextFrameBox, cellRow))
				return nextFrameBox.Bounds.Top;
			return CalculateRowBoxesBottom(row);
		}
		bool ShouldMergeParagraphFrameBoxes(ParagraphFrameBox paragraphFrameBox, ParagraphFrameBox nextParagraphFrameBox, TableCellRow cellRow) {
			if (nextParagraphFrameBox == null)
				return false;
			ParagraphIndex paragraphIndex = paragraphFrameBox.ParagraphIndex;
			if (nextParagraphFrameBox.ParagraphIndex - 1 != paragraphIndex)
				return false;
			if (cellRow != null && cellRow.CellViewInfo.Cell.EndParagraphIndex == paragraphIndex)
				return false;
			Paragraph nextParagraph = nextParagraphFrameBox.GetParagraph();
			Paragraph paragraph = paragraphFrameBox.GetParagraph();
			return nextParagraph.LeftIndent == paragraph.LeftIndent && nextParagraph.RightIndent == paragraph.RightIndent;
		}
		int CalculateRowBoxesTop(Row row) {
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			if (count <= 0)
				return row.Bounds.Top;
			int result = row.Bounds.Bottom;
			for (int i = 0; i < count; i++)
				result = Math.Min(result, boxes[i].Bounds.Top);
			return result;
		}
		int CalculateRowBoxesBottom(Row row) {
			BoxCollection boxes = row.Boxes;
			int count = boxes.Count;
			if (count <= 0)
				return row.Bounds.Bottom;
			int result = row.Bounds.Y;
			for (int i = 0; i < count; i++)
				result = Math.Max(result, boxes[i].Bounds.Bottom);
			return result;
		}
		int FindRowByParagraphIndex(RowCollection rows, ParagraphIndex paragraphIndex) {
			int result = Algorithms.BinarySearch(currentColumn.Rows, new RowParagraphComparer(paragraphIndex), 0, rows.Count - 1);
			if (result < 0)
				return result;
			for (int i = result - 1; i >= 0; i--) {
				if (rows[i].Paragraph.Index != paragraphIndex)
					break;
				result = i;
			}
			return result;
		}
		protected internal virtual void ApplyTableCellsVerticalContentAlignment(TableViewInfoCollection tables) {
			tables.ForEach(ApplyTableCellsVerticalContentAlignment);
		}
		protected internal virtual void ApplyTableCellsVerticalContentAlignment(TableViewInfo table) {
			table.Cells.ForEach(ApplyTableCellVerticalContentAlignment);
		}
		protected internal virtual void ApplyTableCellVerticalContentAlignment(TableCellViewInfo cell) {
			if (cell.IsStartOnPreviousTableViewInfo())
				return;
			int firstRowIndex = cell.GetFirstRowIndex(this.currentColumn);
			if (firstRowIndex < 0)
				return;
			int lastRowIndex = cell.GetLastRowIndex(this.currentColumn);
			if (lastRowIndex < 0)
				return;
			Rectangle actualContentVerticalBounds = CalculateCellContentVerticalBounds(cell, currentColumn.Rows[firstRowIndex], currentColumn.Rows[lastRowIndex]);
			if (actualContentVerticalBounds == Rectangle.Empty)
				return;
			if (cell.InitialContentTop == Int32.MinValue)
				cell.InitialContentTop = actualContentVerticalBounds.Top;
			int offset = CalculateContentOffset(cell, actualContentVerticalBounds.Bottom - actualContentVerticalBounds.Top);
			int initialOffset = actualContentVerticalBounds.Top - cell.InitialContentTop;
			offset -= initialOffset;
			ApplyTableCellsVerticalContentAlignment(cell.InnerTables, firstRowIndex, lastRowIndex, offset);
		}
		protected internal virtual void ApplyTableCellsVerticalContentAlignment(TableViewInfoCollection innerTables, int firstRowIndex, int lastRowIndex, int offset) {
			for (int i = firstRowIndex; i <= lastRowIndex; i++)
				currentColumn.Rows[i].MoveVertically(offset);
			int count = innerTables.Count;
			for (int i = 0; i < count; i++)
				innerTables[i].MoveVerticallyRecursive(offset);
		}
		protected internal virtual int CalculateContentOffset(TableCellViewInfo cell, int contentHeight) {
			TableCellVerticalAnchor topAnchor = cell.TableViewInfo.Anchors[cell.TopAnchorIndex];
			TableCellVerticalAnchor bottomAnchor = cell.TableViewInfo.Anchors[cell.BottomAnchorIndex];
			int top = topAnchor.VerticalPosition + topAnchor.BottomTextIndent;
			int bottom = bottomAnchor.VerticalPosition - bottomAnchor.TopTextIndent;
			int cellMaxContentHeight = bottom - top;
			return CalculateContentOffset(cell.Cell.VerticalAlignment, cellMaxContentHeight, contentHeight);
		}
		protected internal virtual int CalculateContentOffset(VerticalAlignment vertialAlignment, int maxContentHeight, int contentHeight) {
			switch (vertialAlignment) {
				default:
				case VerticalAlignment.Both:
				case VerticalAlignment.Top:
					return 0;
				case VerticalAlignment.Center:
					return Math.Max(0, (maxContentHeight - contentHeight) / 2);
				case Model.VerticalAlignment.Bottom:
					return Math.Max(0, maxContentHeight - contentHeight);
			}
		}
		protected internal virtual Rectangle CalculateCellContentVerticalBounds(TableCellViewInfo cell, Row firstRow, Row lastRow) {
			Rectangle rowsBounds = CalculateCellRowsVerticalBounds(cell, firstRow, lastRow);
			Rectangle innerTablesBounds = CalculateCellInnerTablesVerticalBounds(cell.InnerTables);
			if (rowsBounds == Rectangle.Empty)
				return innerTablesBounds;
			if (innerTablesBounds == Rectangle.Empty)
				return rowsBounds;
			return Rectangle.FromLTRB(0, Math.Min(rowsBounds.Top, innerTablesBounds.Top), 0, Math.Max(rowsBounds.Bottom, innerTablesBounds.Bottom));
		}
		protected internal virtual Rectangle CalculateCellRowsVerticalBounds(TableCellViewInfo cell, Row firstRow, Row lastRow) {
			return Rectangle.FromLTRB(0, firstRow.Bounds.Top, 0, lastRow.Bounds.Bottom);
		}
		protected internal virtual Rectangle CalculateCellInnerTablesVerticalBounds(TableViewInfoCollection tableViewInfoCollection) {
			int count = tableViewInfoCollection.Count;
			if (count <= 0)
				return Rectangle.Empty;
			ITableCellVerticalAnchor topAnchor = tableViewInfoCollection[0].TopAnchor;
			ITableCellVerticalAnchor bottomAnchor = tableViewInfoCollection[count - 1].BottomAnchor;
			return Rectangle.FromLTRB(0, topAnchor.VerticalPosition, 0, bottomAnchor.VerticalPosition + bottomAnchor.BottomTextIndent);
		}
		public void ApplyNewCommentPadding(CommentPadding commentPadding) {
			this.commentPadding = commentPadding;
		}
	}
	#endregion
	#region RowParagraphComparer
	public class RowParagraphComparer : IComparable<Row> {
		readonly ParagraphIndex index;
		public RowParagraphComparer(ParagraphIndex index) {
			this.index = index;
		}
		#region IComparable<Row> Members
		public int CompareTo(Row row) {
			return Comparer<ParagraphIndex>.Default.Compare(row.Paragraph.Index, index);
		}
		#endregion
	}
	#endregion
	#region FormatterHelper
	public static class FormatterHelper {
		public static bool IsCharSpace(char ch) {
			return ch == Characters.Space || ch == Characters.EmSpace || ch == Characters.EnSpace;
		}
		public static bool IsCharDash(char ch) {
			return ch == Characters.Dash || ch == Characters.EmDash || ch == Characters.EnDash;
		}
	}
	#endregion
}
