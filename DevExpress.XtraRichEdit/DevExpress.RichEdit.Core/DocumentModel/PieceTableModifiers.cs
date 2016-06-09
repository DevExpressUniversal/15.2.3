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
using DevExpress.Office;
using DevExpress.Office.Model;
using DevExpress.Office.Utils;
using DevExpress.Office.History;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
#if !SL
using System.Drawing.Printing;
#else
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Drawing;
#endif
namespace DevExpress.XtraRichEdit.Model {
	#region ObjectInserter
	public abstract class ObjectInserter {
		#region Fields
		readonly PieceTable pieceTable;
		readonly DocumentModel documentModel;
		int textLength;
		#endregion
		protected ObjectInserter(PieceTable pieceTable) {
			this.pieceTable = pieceTable;
			this.documentModel = pieceTable.DocumentModel;
		}
		#region Properties
		public int TextLength { get { return textLength; } set { textLength = value; } }
		protected DocumentModel DocumentModel { get { return documentModel; } }
		protected PieceTable PieceTable { get { return pieceTable; } }
		#endregion
		public abstract bool CanMerge(DocumentLogPosition logPosition);
		public abstract void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex);
		public abstract void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible);
	}
	#endregion
	#region TextInserter
	public class TextInserter : ObjectInserter {
		public TextInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return logPosition == PieceTable.LastInsertedRunInfo.LogPosition && !(PieceTable.LastInsertedRunInfo.Run is LayoutDependentTextRun);
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			TextRunAppendTextHistoryItem item = new TextRunAppendTextHistoryItem(PieceTable);
			item.ParagraphIndex = paragraphIndex;
			item.RunIndex = PieceTable.LastInsertedRunInfo.RunIndex;
			item.LogPosition = logPosition;
			item.TextLength = TextLength;
			RichEditDocumentHistory history = DocumentModel.History as RichEditDocumentHistory;
			if (history != null)
				history.AddRangeTextAppendedHistoryItem(item);
			else
				DocumentModel.History.Add(item);
			item.Execute();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - TextLength;
			InsertTextRun(paragraph, runIndex, oldGrowBufferLength, TextLength, forceVisible);
			PieceTable.LastInsertedRunInfo.LogPosition = logPosition + TextLength;
		}
		void InsertTextRun(Paragraph paragraph, RunIndex where, int startIndex, int length, bool forceVisible) {
			TextRunInsertedHistoryItem item = CreateRunInsertedHistoryItem();
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.NewLength = length;
			item.ParagraphIndex = paragraph.Index;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected virtual TextRunInsertedHistoryItem CreateRunInsertedHistoryItem() {
			return new TextRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region LayoutDependentTextInserter
	public class LayoutDependentTextInserter : ObjectInserter {
		FieldResultFormatting fieldResultFormatting;
		public LayoutDependentTextInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public FieldResultFormatting FieldResultFormatting { get { return fieldResultFormatting; } set { fieldResultFormatting = value; } }
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - TextLength;
			InsertLayoutDependenTextRun(paragraph, runIndex, oldGrowBufferLength, TextLength, forceVisible);
			PieceTable.LastInsertedRunInfo.LogPosition = logPosition + TextLength;
		}
		void InsertLayoutDependenTextRun(Paragraph paragraph, RunIndex where, int startIndex, int length, bool forceVisible) {
			LayoutDependentTextRunInsertedHistoryItem item = CreateRunInsertedHistoryItem();
			AssignHistoryItemProperties(item, paragraph, where, startIndex, length, forceVisible);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected internal virtual void AssignHistoryItemProperties(LayoutDependentTextRunInsertedHistoryItem item, Paragraph paragraph, RunIndex where, int startIndex, int length, bool forceVisible) {
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.NewLength = length;
			item.ParagraphIndex = paragraph.Index;
			item.FieldResultFormatting = FieldResultFormatting;
		}
		protected virtual LayoutDependentTextRunInsertedHistoryItem CreateRunInsertedHistoryItem() {
			return new LayoutDependentTextRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region FootNoteRunInserterBase<T> (abstract class)
	public abstract class FootNoteRunInserterBase<T> : LayoutDependentTextInserter where T : FootNoteBase<T> {
		int noteIndex;
		protected FootNoteRunInserterBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public int NoteIndex { get { return noteIndex; } set { noteIndex = value; } }
		protected internal override void AssignHistoryItemProperties(LayoutDependentTextRunInsertedHistoryItem item, Paragraph paragraph, RunIndex where, int startIndex, int length, bool forceVisible) {
			base.AssignHistoryItemProperties(item, paragraph, where, startIndex, length, forceVisible);
			FootNoteRunInsertedHistoryItemBase<T> footNoteInsertedHistoryItem = (FootNoteRunInsertedHistoryItemBase<T>)item;
			footNoteInsertedHistoryItem.NoteIndex = NoteIndex;
		}
	}
	#endregion
	#region FootNoteRunInserter
	public class FootNoteRunInserter : FootNoteRunInserterBase<FootNote> {
		public FootNoteRunInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override LayoutDependentTextRunInsertedHistoryItem CreateRunInsertedHistoryItem() {
			return new FootNoteRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region EndNoteRunInserter
	public class EndNoteRunInserter : FootNoteRunInserterBase<EndNote> {
		public EndNoteRunInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override LayoutDependentTextRunInsertedHistoryItem CreateRunInsertedHistoryItem() {
			return new EndNoteRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region FieldRunInserterBase (abstract class)
	public abstract class FieldRunInserterBase : ObjectInserter {
		protected FieldRunInserterBase(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			MarkRunInsertedHistoryItemBase item = CreateHistoryItem();
			item.ForceVisible = forceVisible;
			item.RunIndex = runIndex;
			item.StartIndex = PieceTable.TextBuffer.Length - 1;
			item.ParagraphIndex = paragraph.Index;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected abstract MarkRunInsertedHistoryItemBase CreateHistoryItem();
	}
	#endregion
	#region FieldResultEndRunInserter
	public class FieldResultEndRunInserter : FieldRunInserterBase {
		public FieldResultEndRunInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override MarkRunInsertedHistoryItemBase CreateHistoryItem() {
			return new FieldResultEndRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region FieldCodeStartRunInserter
	public class FieldCodeStartRunInserter : FieldRunInserterBase {
		public FieldCodeStartRunInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override MarkRunInsertedHistoryItemBase CreateHistoryItem() {
			return new FieldCodeStartRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region FieldCodeEndRunInserter
	public class FieldCodeEndRunInserter : FieldRunInserterBase {
		public FieldCodeEndRunInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		protected override MarkRunInsertedHistoryItemBase CreateHistoryItem() {
			return new FieldCodeEndRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region ParagraphInserter
	public class ParagraphInserter : ObjectInserter {
		public ParagraphInserter(PieceTable pieceTable)
			: base(pieceTable) {
			this.TextLength = 1;
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - 1;
			InsertParagraphRun(paragraph, runIndex, oldGrowBufferLength, forceVisible);
			InsertParagraphIntoParagraphTable(paragraph, runIndex, logPosition);
		}
		void InsertParagraphRun(Paragraph paragraph, RunIndex where, int startIndex, bool forceVisible) {
			ParagraphRunInsertedHistoryItem item = CreateInsertParagraphRangeHistoryItem();
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.ParagraphIndex = paragraph.Index;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		public virtual ParagraphRunInsertedHistoryItem CreateInsertParagraphRangeHistoryItem() {
			return new ParagraphRunInsertedHistoryItem(PieceTable);
		}
		void InsertParagraphIntoParagraphTable(Paragraph paragraph, RunIndex paragraphMarkRunIndex, DocumentLogPosition logPosition) {
			ParagraphInsertedBaseHistoryItem item = new ParagraphInsertedBaseHistoryItem(PieceTable);
			item.ParagraphIndex = paragraph.Index;
			item.LogPosition = logPosition;
			item.ParagraphMarkRunIndex = paragraphMarkRunIndex;
			item.SetTableCell(paragraph.GetCell());
			DocumentModel.History.Add(item);
			item.Execute();
		}
	}
	#endregion
	#region SectionInserter
	public class SectionInserter : ParagraphInserter {
		public SectionInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}
		public override ParagraphRunInsertedHistoryItem CreateInsertParagraphRangeHistoryItem() {
			return new SectionRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region InlinePictureInserter
	public class InlinePictureInserter : ObjectInserter {
		#region Fields
		readonly OfficeImage image;
		readonly float scaleX;
		readonly float scaleY;
		readonly bool useScreenDpi;
		#endregion
		public InlinePictureInserter(PieceTable pieceTable, OfficeImage image, float scaleX, float scaleY, bool useScreenDpi)
			: base(pieceTable) {
			this.TextLength = 1;
			this.image = image;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
			this.useScreenDpi = useScreenDpi;
		}
		public InlinePictureInserter(PieceTable pieceTable, OfficeImage image, float scaleX, float scaleY)
			: this(pieceTable, image, scaleX, scaleY, false) {
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - 1;
			InsertInlinePictureRun(paragraph, runIndex, oldGrowBufferLength, forceVisible);
		}
		void InsertInlinePictureRun(Paragraph paragraph, RunIndex where, int startIndex, bool forceVisible) {
			InlinePictureRunInsertedHistoryItem item = CreateRunInsertedHistoryItem(PieceTable);
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.ParagraphIndex = paragraph.Index;
			item.Image = image;
			DocumentModel.History.Add(item);
			item.Execute();
			InlinePictureRun run = (InlinePictureRun)PieceTable.Runs[where];
			InlinePictureProperties properties = run.Properties;
			properties.BeginUpdate();
			try {
				properties.ScaleX = scaleX;
				properties.ScaleY = scaleY;
			}
			finally {
				properties.EndUpdate();
			}
			run.PictureContent.EnsureActualSize(this.useScreenDpi);
		}
		protected virtual InlinePictureRunInsertedHistoryItem CreateRunInsertedHistoryItem(PieceTable pieceTable) {
			return new InlinePictureRunInsertedHistoryItem(pieceTable);
		}
	}
	#endregion
	#region CustomRunInserter
	public class CustomRunInserter : ObjectInserter {
		#region Fields
		readonly ICustomRunObject customRunObject;
		#endregion
		public CustomRunInserter(PieceTable pieceTable, ICustomRunObject customRunObject)
			: base(pieceTable) {
			Guard.ArgumentNotNull(customRunObject, "customRunObject");
			this.TextLength = 1;
			this.customRunObject = customRunObject;
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - 1;
			InsertCustomRun(paragraph, runIndex, oldGrowBufferLength, forceVisible);
		}
		void InsertCustomRun(Paragraph paragraph, RunIndex where, int startIndex, bool forceVisible) {
			CustomRunInsertedHistoryItem item = new CustomRunInsertedHistoryItem(PieceTable);
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.ParagraphIndex = paragraph.Index;
			item.CustomRunObject = customRunObject;
			DocumentModel.History.Add(item);
			item.Execute();
		}
	}
	#endregion
	#region FloatingObjectAnchorInserter
	public class FloatingObjectAnchorInserter : ObjectInserter {
		public FloatingObjectAnchorInserter(PieceTable pieceTable)
			: base(pieceTable) {
			this.TextLength = 1;
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - 1;
			InsertFloatingObjectAnchorRun(paragraph, runIndex, oldGrowBufferLength, forceVisible);
		}
		void InsertFloatingObjectAnchorRun(Paragraph paragraph, RunIndex where, int startIndex, bool forceVisible) {
			FloatingObjectAnchorRunInsertedHistoryItem item = new FloatingObjectAnchorRunInsertedHistoryItem(PieceTable);
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.ParagraphIndex = paragraph.Index;
			DocumentModel.History.Add(item);
			item.Execute();
		}
	}
	#endregion
	#region InlineCustomObjectInserter
	public class InlineCustomObjectInserter : ObjectInserter {
		#region Fields
		readonly IInlineCustomObject customObject;
		readonly float scaleX;
		readonly float scaleY;
		#endregion
		public InlineCustomObjectInserter(PieceTable pieceTable, IInlineCustomObject customObject, float scaleX, float scaleY)
			: base(pieceTable) {
			this.TextLength = 1;
			this.customObject = customObject;
			this.scaleX = scaleX;
			this.scaleY = scaleY;
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			int oldGrowBufferLength = PieceTable.TextBuffer.Length - 1;
			InsertInlineCustomObjectRun(paragraph, runIndex, oldGrowBufferLength, forceVisible);
		}
		void InsertInlineCustomObjectRun(Paragraph paragraph, RunIndex where, int startIndex, bool forceVisible) {
			InlineCustomObjectRunInsertedHistoryItem item = new InlineCustomObjectRunInsertedHistoryItem(PieceTable);
			item.ForceVisible = forceVisible;
			item.RunIndex = where;
			item.StartIndex = startIndex;
			item.ParagraphIndex = paragraph.Index;
			item.CustomObject = customObject;
			item.ScaleX = scaleX;
			item.ScaleY = scaleY;
			DocumentModel.History.Add(item);
			item.Execute();
		}
	}
	#endregion
	#region SeparatorTextRunInserter
	class SeparatorTextRunInserter : ObjectInserter {
		public SeparatorTextRunInserter(PieceTable pieceTable)
			: base(pieceTable) {
		}		
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			SeparatorRunInsertedHistoryItem item = CreateHistoryItem();
			item.ForceVisible = forceVisible;
			item.RunIndex = runIndex;
			item.StartIndex = PieceTable.TextBuffer.Length - 1;
			item.ParagraphIndex = paragraph.Index;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		protected virtual SeparatorRunInsertedHistoryItem CreateHistoryItem() {
			return new SeparatorRunInsertedHistoryItem(PieceTable);
		}
	}
	#endregion
	#region DataContainerRunInserter
	public class DataContainerRunInserter : ObjectInserter {
		readonly IDataContainer dataContainer;
		public DataContainerRunInserter(PieceTable pieceTable, IDataContainer dataContainer)
			: base(pieceTable) {
			Guard.ArgumentNotNull(dataContainer, "dataContainer");
			this.TextLength = 1;
			this.dataContainer = dataContainer;
		}
		public override bool CanMerge(DocumentLogPosition logPosition) {
			return false;
		}
		public override void Merge(DocumentLogPosition logPosition, ParagraphIndex paragraphIndex) {
			Exceptions.ThrowInternalException();
		}
		public override void PerformInsert(Paragraph paragraph, RunIndex runIndex, DocumentLogPosition logPosition, bool forceVisible) {
			DataContainerRunInsertedHistoryItem item = new DataContainerRunInsertedHistoryItem(PieceTable);
			item.ForceVisible = forceVisible;
			item.RunIndex = runIndex;
			item.StartIndex = PieceTable.TextBuffer.Length - 1;
			item.ParagraphIndex = paragraph.Index;
			item.DataContainer = dataContainer;
			DocumentModel.History.Add(item);
			item.Execute();
		}
	}
	#endregion
	#region RunPropertyModifierBase (abstract class)
	public abstract class RunPropertyModifierBase {
		public abstract void ModifyTextRun(TextRunBase run, RunIndex runIndex);
	}
	#endregion
	#region InlinePictureRunPropertyModifierBase (abstract class)
	public abstract class InlinePictureRunPropertyModifierBase {
		public abstract void ModifyPictureRun(InlinePictureRun run, RunIndex runIndex);
	}
	#endregion
	#region RectangularObjectPropertyModifierBase (abstract class)
	public abstract class RectangularObjectPropertyModifierBase {
		public abstract void ModifyRectangularObject(IRectangularObject rectangularObject, RunIndex runIndex);
	}
	#endregion
	#region ParagraphPropertyModifierBase
	public abstract class ParagraphPropertyModifierBase {
		public abstract void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex);
	}
	#endregion
	#region RunChangeCaseModifierBase (abstract class)
	public abstract class RunChangeCaseModifierBase : RunPropertyModifierBase {
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			TextRun textRun = run as TextRun;
			if (textRun != null && run.PieceTable.VisibleTextFilter.GetRunVisibilityInField(runIndex) != RunVisibility.Hidden)
				ChangeRunCase(textRun, runIndex);
		}
		protected internal virtual void ChangeRunCase(TextRun textRun, RunIndex runIndex) {			
			TextRunChangeCaseHistoryItem item = CreateHistoryItem(textRun.Paragraph.PieceTable, runIndex);
			DocumentHistory history = textRun.Paragraph.DocumentModel.History;
			history.Add(item);
			item.Execute();
			if (textRun.CharacterProperties.UseAllCaps)
				textRun.CharacterProperties.ResetUse(CharacterFormattingOptions.Mask.UseAllCaps);
			if (textRun.AllCaps) {
				textRun.AllCaps = false;
			}
		}
		protected internal abstract TextRunChangeCaseHistoryItem CreateHistoryItem(PieceTable pieceTable, RunIndex runIndex);
	}
	#endregion
	#region RunMakeUpperCaseModifier
	public class RunMakeUpperCaseModifier : RunChangeCaseModifierBase {
		protected internal override TextRunChangeCaseHistoryItem CreateHistoryItem(PieceTable pieceTable, RunIndex runIndex) {
			return new TextRunMakeUpperCaseHistoryItem(pieceTable, runIndex);
		}
	}
	#endregion
	#region RunMakeLowerCaseModifier
	public class RunMakeLowerCaseModifier : RunChangeCaseModifierBase {
		protected internal override TextRunChangeCaseHistoryItem CreateHistoryItem(PieceTable pieceTable, RunIndex runIndex) {
			return new TextRunMakeLowerCaseHistoryItem(pieceTable, runIndex);
		}
	}
	#endregion
	#region RunToggleCaseModifier
	public class RunToggleCaseModifier : RunChangeCaseModifierBase {
		protected internal override TextRunChangeCaseHistoryItem CreateHistoryItem(PieceTable pieceTable, RunIndex runIndex) {
			return new TextRunToggleCaseHistoryItem(pieceTable, runIndex);
		}
	}
	#endregion
	#region RunCapitalizeEachWordCaseModifier
	public class RunCapitalizeEachWordCaseModifier : RunChangeCaseModifierBase {
		protected internal override TextRunChangeCaseHistoryItem CreateHistoryItem(PieceTable pieceTable, RunIndex runIndex) {
			return new TextRunCapitalizeEachWordCaseHistoryItem(pieceTable, runIndex);
		}
	}
	#endregion
	#region RunPropertyModifier<T>
	public abstract class RunPropertyModifier<T> : RunPropertyModifierBase {
		readonly T newValue;
		protected RunPropertyModifier(T newValue) {
			this.newValue = ValidateNewValue(newValue);
		}
		public T NewValue { get { return newValue; } }
		protected internal virtual T ValidateNewValue(T newValue) {
			return newValue;
		}
		public virtual void ModifyInputPosition(InputPosition pos) {
			ModifyInputPositionCore(pos);
		}
		public abstract T GetRunPropertyValue(TextRunBase run);
		public abstract void ModifyInputPositionCore(InputPosition pos);
		public abstract T GetInputPositionPropertyValue(InputPosition pos);
	}
	#endregion
	#region InlinePictureRunPropertyModifier<T>
	public abstract class InlinePictureRunPropertyModifier<T> : InlinePictureRunPropertyModifierBase {
		readonly T newValue;
		protected InlinePictureRunPropertyModifier(T newValue) {
			this.newValue = ValidateNewValue(newValue);
		}
		public T NewValue { get { return newValue; } }
		protected internal virtual T ValidateNewValue(T newValue) {
			return newValue;
		}
	}
	#endregion
	#region RectangularObjectPropertyModifier<T>
	public abstract class RectangularObjectPropertyModifier<T> : RectangularObjectPropertyModifierBase {
		readonly T newValue;
		protected RectangularObjectPropertyModifier(T newValue) {
			this.newValue = ValidateNewValue(newValue);
		}
		public T NewValue { get { return newValue; } }
		protected internal virtual T ValidateNewValue(T newValue) {
			return newValue;
		}
	}
	#endregion    
	#region FloatingObjectRunPropertyModifierBase (abstract class)
	public abstract class FloatingObjectRunPropertyModifierBase {
		public abstract void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex);
	}
	#endregion
	#region FloatingObjectRunPropertyModifier<T>
	public abstract class FloatingObjectRunPropertyModifier<T> : FloatingObjectRunPropertyModifierBase {
		readonly T newValue;
		protected FloatingObjectRunPropertyModifier(T newValue) {
			this.newValue = ValidateNewValue(newValue);
		}
		public T NewValue { get { return newValue; } }
		protected internal virtual T ValidateNewValue(T newValue) {
			return newValue;
		}
		public abstract T GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun);
	}
	#endregion
	#region FloatingObjectRunTextWrapTypeModifier
	public class FloatingObjectRunTextWrapTypeModifier : FloatingObjectRunPropertyModifier<FloatingObjectTextWrapType> {
		public FloatingObjectRunTextWrapTypeModifier(FloatingObjectTextWrapType newValue)
			: base(newValue) {
		}
		protected internal override FloatingObjectTextWrapType ValidateNewValue(FloatingObjectTextWrapType newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.FloatingObjectProperties.TextWrapType = NewValue;
		}
		public override FloatingObjectTextWrapType GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.FloatingObjectProperties.TextWrapType;
		}
	}
	#endregion
	#region FloatingObjectRunHorizontalPositionAlignmentModifier
	public class FloatingObjectRunHorizontalPositionAlignmentModifier : FloatingObjectRunPropertyModifier<FloatingObjectHorizontalPositionAlignment> {
		public FloatingObjectRunHorizontalPositionAlignmentModifier(FloatingObjectHorizontalPositionAlignment newValue)
			: base(newValue) {
		}
		protected internal override FloatingObjectHorizontalPositionAlignment ValidateNewValue(FloatingObjectHorizontalPositionAlignment newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.FloatingObjectProperties.HorizontalPositionAlignment = NewValue;
		}
		public override FloatingObjectHorizontalPositionAlignment GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.FloatingObjectProperties.HorizontalPositionAlignment;
		}
	}
	#endregion
	#region FloatingObjectRunVerticalPositionAlignmentModifier
	public class FloatingObjectRunVerticalPositionAlignmentModifier : FloatingObjectRunPropertyModifier<FloatingObjectVerticalPositionAlignment> {
		public FloatingObjectRunVerticalPositionAlignmentModifier(FloatingObjectVerticalPositionAlignment newValue)
			: base(newValue) {
		}
		protected internal override FloatingObjectVerticalPositionAlignment ValidateNewValue(FloatingObjectVerticalPositionAlignment newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.FloatingObjectProperties.VerticalPositionAlignment = NewValue;
		}
		public override FloatingObjectVerticalPositionAlignment GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.FloatingObjectProperties.VerticalPositionAlignment;
		}
	}
	#endregion
	#region FloatingObjectRunHorizontalPositionTypeModifier
	public class FloatingObjectRunHorizontalPositionTypeModifier : FloatingObjectRunPropertyModifier<FloatingObjectHorizontalPositionType> {
		public FloatingObjectRunHorizontalPositionTypeModifier(FloatingObjectHorizontalPositionType newValue)
			: base(newValue) {
		}
		protected internal override FloatingObjectHorizontalPositionType ValidateNewValue(FloatingObjectHorizontalPositionType newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.FloatingObjectProperties.HorizontalPositionType = NewValue;
		}
		public override FloatingObjectHorizontalPositionType GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.FloatingObjectProperties.HorizontalPositionType;
		}
	}
	#endregion
	#region FloatingObjectRunVerticalPositionTypeModifier
	public class FloatingObjectRunVerticalPositionTypeModifier : FloatingObjectRunPropertyModifier<FloatingObjectVerticalPositionType> {
		public FloatingObjectRunVerticalPositionTypeModifier(FloatingObjectVerticalPositionType newValue)
			: base(newValue) {
		}
		protected internal override FloatingObjectVerticalPositionType ValidateNewValue(FloatingObjectVerticalPositionType newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.FloatingObjectProperties.VerticalPositionType = NewValue;
		}
		public override FloatingObjectVerticalPositionType GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.FloatingObjectProperties.VerticalPositionType;
		}
	}
	#endregion
	#region FloatingObjectRunFillColorModifier
	public class FloatingObjectRunFillColorModifier : FloatingObjectRunPropertyModifier<Color> {
		public FloatingObjectRunFillColorModifier(Color newValue)
			: base(newValue) {
		}
		protected internal override Color ValidateNewValue(Color newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.Shape.FillColor = NewValue;
		}
		public override Color GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.Shape.FillColor;
		}
	}
	#endregion
	#region FloatingObjectRunOutlineColorModifier
	public class FloatingObjectRunOutlineColorModifier : FloatingObjectRunPropertyModifier<Color> {
		public FloatingObjectRunOutlineColorModifier(Color newValue)
			: base(newValue) {
		}
		protected internal override Color ValidateNewValue(Color newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.Shape.OutlineColor = NewValue;
		}
		public override Color GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.Shape.OutlineColor;
		}
	}
	#endregion
	#region FloatingObjectRunOutlineWidthModifier
	public class FloatingObjectRunOutlineWidthModifier : FloatingObjectRunPropertyModifier<int> {
		public FloatingObjectRunOutlineWidthModifier(int newValue)
			: base(newValue) {
		}
		protected internal override int ValidateNewValue(int newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.Shape.OutlineWidth = NewValue;
		}
		public override int GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.Shape.OutlineWidth;
		}
	}
	#endregion
	#region FloatingObjectRunOutlineWidthAndColorModifier
	public class FloatingObjectRunOutlineWidthAndColorModifier : FloatingObjectRunOutlineWidthModifier {
		public FloatingObjectRunOutlineWidthAndColorModifier(int newValue)
			: base(newValue) {
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			base.ModifyFloatingObjectRun(run, runIndex);
			if (DXColor.IsTransparentOrEmpty(run.Shape.OutlineColor))
				run.Shape.OutlineColor = DXColor.Black;
		}
	}
	#endregion
	#region FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier
	public class FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier : FloatingObjectRunPropertyModifier<bool> {
		public FloatingObjectRunIsBehindDocTextWrapTypeNoneModifier(bool newValue)
			: base(newValue) {
		}
		protected internal override bool ValidateNewValue(bool newValue) {
			return newValue;
		}
		public override void ModifyFloatingObjectRun(FloatingObjectAnchorRun run, RunIndex runIndex) {
			run.FloatingObjectProperties.IsBehindDoc = NewValue;
			run.FloatingObjectProperties.TextWrapType = FloatingObjectTextWrapType.None;
		}
		public override bool GetFloatingObjectValue(FloatingObjectAnchorRun anchorRun) {
			return anchorRun.FloatingObjectProperties.IsBehindDoc;
		}
	}
	#endregion
	#region MergedRunPropertyModifier
	public abstract class MergedRunPropertyModifier<T> : RunPropertyModifier<T> {
		protected MergedRunPropertyModifier(T newValue)
			: base(newValue) {
		}
		protected internal virtual bool CanModifyRun(TextRunBase run) {
			return true;
		}
		public abstract T Merge(T leftValue, T rightValue);
	}
	#endregion
	#region ParagraphPropertyModifier<T>
	public abstract class ParagraphPropertyModifier<T> : ParagraphPropertyModifierBase {
		readonly T newValue;
		protected ParagraphPropertyModifier(T newValue) {
			this.newValue = newValue;
		}
		public T NewValue { get { return newValue; } }
		public abstract T GetParagraphPropertyValue(Paragraph paragraph);
	}
	#endregion
	#region MergedParagraphPropertyModifier
	public abstract class MergedParagraphPropertyModifier<T> : ParagraphPropertyModifier<T> {
		protected MergedParagraphPropertyModifier(T newValue)
			: base(newValue) {
		}
		public abstract T Merge(T leftValue, T rightValue);
	}
	#endregion
	#region RunCharacterStyleModifier
	public class RunCharacterStyleModifier : RunPropertyModifier<int> {
		readonly bool resetProperties;
		public RunCharacterStyleModifier(int styleIndex, bool resetProperties)
			: base(styleIndex) {
			this.resetProperties = resetProperties;
		}
		public RunCharacterStyleModifier(int styleIndex)
			: this(styleIndex, true) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			DocumentModel documentModel = run.Paragraph.DocumentModel;
			documentModel.History.BeginTransaction();
			try {
				if (resetProperties)
					run.ResetCharacterProperties();
				run.CharacterStyleIndex = NewValue;
			}
			finally {
				documentModel.History.EndTransaction();
			}
		}
		public override int GetRunPropertyValue(TextRunBase run) {
			return run.CharacterStyleIndex;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
		}
		public override int GetInputPositionPropertyValue(InputPosition pos) {
			return pos.CharacterStyleIndex;
		}
	}
	#endregion
	#region RunCharacterStyleKeepOldStylePropertiesModifier
	public class RunCharacterStyleKeepOldStylePropertiesModifier : RunCharacterStyleModifier {
		readonly bool applyDefaultHyperlinkStyle;
		readonly CharacterFormattingOptions.Mask ignoredOptions;
		public RunCharacterStyleKeepOldStylePropertiesModifier(int styleIndex, bool applyDefaultHyperlinkStyle, CharacterFormattingOptions.Mask ignoredOptions)
			: base(styleIndex, false) {
			this.applyDefaultHyperlinkStyle = applyDefaultHyperlinkStyle;
			this.ignoredOptions = ignoredOptions;
		}
		public RunCharacterStyleKeepOldStylePropertiesModifier(int styleIndex, bool applyDefaultHyperlinkStyle)
			: this(styleIndex, applyDefaultHyperlinkStyle, CharacterFormattingOptions.Mask.UseNone) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			if (run.CharacterStyleIndex == NewValue)
				return;
			ModifyTextRunCore(run);
		}
		void ModifyTextRunCore(TextRunBase run) {
			DocumentModel documentModel = run.Paragraph.DocumentModel;
			documentModel.History.BeginTransaction();
			try {
				MergedCharacterProperties properties = run.CharacterStyle.GetMergedCharacterProperties();
				if (applyDefaultHyperlinkStyle) {
					MergedCharacterProperties stypeProperties = documentModel.CharacterStyles[NewValue].GetMergedCharacterProperties();
					CharacterFormattingOptions.Mask styleOptions = stypeProperties.Options.Value;
					styleOptions &= ~ignoredOptions;
					properties.Options.Value &= ~styleOptions;
					run.CharacterProperties.ResetUse(styleOptions);
				}
				CharacterPropertiesMerger merger = new CharacterPropertiesMerger(run.CharacterProperties);
				merger.Merge(properties);
				MergedCharacterProperties mergedProperties = merger.MergedProperties;
				run.CharacterProperties.CopyFrom(mergedProperties);
				run.CharacterStyleIndex = NewValue;
			}
			finally {
				documentModel.History.EndTransaction();
			}
		}
	}
	#endregion
	#region ReplaceRunCharacterStylePropertiesModifier
	public class ReplaceRunCharacterStylePropertiesModifier : RunCharacterStyleModifier {
		readonly MergedCharacterProperties properties;
		public ReplaceRunCharacterStylePropertiesModifier(int styleIndex, MergedCharacterProperties sourceProperties)
			: base(styleIndex, false) {
			Guard.ArgumentNotNull(sourceProperties, "sourceProperties");
			this.properties = sourceProperties;
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			DocumentModel documentModel = run.Paragraph.DocumentModel;
			documentModel.History.BeginTransaction();
			try {
				if (run.CharacterStyleIndex != NewValue)
					return;
				CharacterStyle style = documentModel.CharacterStyles[NewValue];
				MergedCharacterProperties newProperties = LeaveUseInStyleProperties(style, properties);
				CharacterPropertiesMerger merger = new CharacterPropertiesMerger(run.CharacterProperties);
				merger.Merge(newProperties);
				run.CharacterProperties.CopyFrom(merger.MergedProperties);
				run.CharacterStyleIndex = CharacterStyleCollection.EmptyCharacterStyleIndex;
			}
			finally {
				documentModel.History.EndTransaction();
			}
		}
		MergedCharacterProperties LeaveUseInStyleProperties(CharacterStyle style, MergedCharacterProperties properties) {
			MergedCharacterProperties result = new MergedCharacterProperties(properties.Info, properties.Options);
			MergedCharacterProperties styleProperties = style.GetMergedCharacterProperties();
			result.Options.Value = styleProperties.Options.Value;
			return result;
		}
	}
	#endregion
	#region RunFontNamePropertyModifier
	public class RunFontNamePropertyModifier : RunPropertyModifier<string> {
		public RunFontNamePropertyModifier(string val)
			: base(val) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.FontName = NewValue;
		}
		public override string GetRunPropertyValue(TextRunBase run) {
			return run.FontName;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.FontName = NewValue;
			pos.MergedCharacterFormatting.FontName = NewValue;
		}
		public override string GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.FontName;
		}
	}
	#endregion
	#region RunDoubleFontSizePropertyModifier
	public class RunDoubleFontSizePropertyModifier : RunPropertyModifier<int> {
		public RunDoubleFontSizePropertyModifier(int val)
			: base(val) {
		}
		protected internal override int ValidateNewValue(int newValue) {
			return PredefinedFontSizeCollection.ValidateFontSize(newValue);
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.DoubleFontSize = NewValue;
		}
		public override int GetRunPropertyValue(TextRunBase run) {
			return run.DoubleFontSize;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.DoubleFontSize = NewValue;
			pos.MergedCharacterFormatting.DoubleFontSize = NewValue;
		}
		public override int GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.DoubleFontSize;
		}
	}
	#endregion
	#region RunFontSizePropertyModifier
	public class RunFontSizePropertyModifier : RunPropertyModifier<float> {
		public RunFontSizePropertyModifier(float val)
			: base(val) {
		}
		int GetDoubleFontSize(float fontSize) {
			int doubleFontSize = (int)(fontSize * 2);
			return doubleFontSize;
		}
		protected internal override float ValidateNewValue(float newValue) {
			return PredefinedFontSizeCollection.ValidateFontSize(GetDoubleFontSize(newValue)) / 2f;
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.DoubleFontSize = GetDoubleFontSize(NewValue);
		}
		public override float GetRunPropertyValue(TextRunBase run) {
			return run.DoubleFontSize / 2f;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.DoubleFontSize = GetDoubleFontSize(NewValue);
			pos.MergedCharacterFormatting.DoubleFontSize = GetDoubleFontSize(NewValue);
		}
		public override float GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.DoubleFontSize / 2f;
		}
	}
	#endregion
	#region RunFontBoldModifier
	public class RunFontBoldModifier : RunPropertyModifier<bool> {
		public RunFontBoldModifier(bool bold)
			: base(bold) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.FontBold = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.FontBold;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.FontBold = NewValue;
			pos.MergedCharacterFormatting.FontBold = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.FontBold;
		}
	}
	#endregion
	#region RunFontItalicModifier
	public class RunFontItalicModifier : RunPropertyModifier<bool> {
		public RunFontItalicModifier(bool italic)
			: base(italic) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.FontItalic = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.FontItalic;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.FontItalic = NewValue;
			pos.MergedCharacterFormatting.FontItalic = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.FontItalic;
		}
	}
	#endregion
	#region RunClearCharacterFormattingModifier
	public class RunClearCharacterFormattingModifier : RunPropertyModifier<bool> {
		public RunClearCharacterFormattingModifier(bool newValue)
			: base(newValue) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.CharacterProperties.Reset();
			SplitRunByCharset(run, runIndex);
			run.CharacterStyleIndex = 0;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return true;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {			
			CharacterFormattingBase emptyInfo = pos.DocumentModel.Cache.CharacterFormattingCache[CharacterFormattingCache.EmptyCharacterFormattingIndex];			
			pos.CharacterFormatting.CopyFrom(emptyInfo.Info, emptyInfo.Options);
			pos.MergedCharacterFormatting.CopyFrom(pos.DocumentModel.DefaultCharacterProperties.Info.Info);
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.CharacterFormatting.InfoIndex != pos.DocumentModel.DefaultCharacterProperties.Info.InfoIndex;
		}
		void SplitRunByCharset(TextRunBase run, RunIndex runIndex) {
			run.DocumentModel.DeferredChanges.ApplyChanges(run.PieceTable, CharacterFormattingChangeActionsCalculator.CalculateChangeActions(CharacterFormattingChangeType.FontName), runIndex, runIndex);
		}
	}
	#endregion
	#region RunHiddenModifier
	public class RunHiddenModifier : RunPropertyModifier<bool> {
		public RunHiddenModifier(bool value)
			: base(value) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			if (runIndex < new RunIndex(run.Paragraph.PieceTable.Runs.Count - 1))
				run.Hidden = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.Hidden;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.Hidden = NewValue;
			pos.MergedCharacterFormatting.Hidden = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.Hidden;
		}
	}
	#endregion
	#region RunLanguageModifier
	public class RunLanguageTypeModifier : RunPropertyModifier<LangInfo> {
		public RunLanguageTypeModifier(LangInfo value) : base(value) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			if (runIndex < new RunIndex(run.Paragraph.PieceTable.Runs.Count - 1))
				run.LangInfo = NewValue;
		}
		public override LangInfo GetRunPropertyValue(TextRunBase run) {
			return run.LangInfo;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.LangInfo = NewValue;
			pos.MergedCharacterFormatting.LangInfo = NewValue;
		}
		public override LangInfo GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.LangInfo;
		} 
	}
	#endregion
	#region RunNoProofModifier
	public class RunNoProofModifier : RunPropertyModifier<bool> {
		public RunNoProofModifier(bool value)
			: base(value) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			if (runIndex < new RunIndex(run.Paragraph.PieceTable.Runs.Count - 1))
				run.NoProof = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.NoProof;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.NoProof = NewValue;
			pos.MergedCharacterFormatting.NoProof = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.NoProof;
		}
	}
	#endregion
	#region RunAllCapsModifier
	public class RunAllCapsModifier : RunPropertyModifier<bool> {
		public RunAllCapsModifier(bool allCaps)
			: base(allCaps) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.AllCaps = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.AllCaps;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.AllCaps = NewValue;
			pos.MergedCharacterFormatting.AllCaps = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.AllCaps;
		}
	}
	#endregion
	#region RunStrikeoutWordsOnlyModifier
	public class RunStrikeoutWordsOnlyModifier : RunPropertyModifier<bool> {
		public RunStrikeoutWordsOnlyModifier(bool strikeoutWordsOnly)
			: base(strikeoutWordsOnly) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.StrikeoutWordsOnly = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.StrikeoutWordsOnly;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.StrikeoutWordsOnly = NewValue;
			pos.MergedCharacterFormatting.StrikeoutWordsOnly = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.StrikeoutWordsOnly;
		}
	}
	#endregion
	#region RunUnderlineWordsOnlyModifier
	public class RunUnderlineWordsOnlyModifier : RunPropertyModifier<bool> {
		public RunUnderlineWordsOnlyModifier(bool underlineWordsOnly)
			: base(underlineWordsOnly) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.UnderlineWordsOnly = NewValue;
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.UnderlineWordsOnly;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.UnderlineWordsOnly = NewValue;
			pos.MergedCharacterFormatting.UnderlineWordsOnly = NewValue;
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.UnderlineWordsOnly;
		}
	}
	#endregion
	#region RunFontUnderlineTypeModifier
	public class RunFontUnderlineTypeModifier : RunPropertyModifier<UnderlineType> {
		public RunFontUnderlineTypeModifier(UnderlineType underlineType)
			: base(underlineType) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.FontUnderlineType = NewValue;
		}
		public override UnderlineType GetRunPropertyValue(TextRunBase run) {
			return run.FontUnderlineType;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.FontUnderlineType = NewValue;
			pos.MergedCharacterFormatting.FontUnderlineType = NewValue;
		}
		public override UnderlineType GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.FontUnderlineType;
		}
	}
	#endregion
	#region RangeFontStrikeoutModifier
	public class RunFontStrikeoutTypeModifier : RunPropertyModifier<StrikeoutType> {
		public RunFontStrikeoutTypeModifier(StrikeoutType strikeoutType)
			: base(strikeoutType) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.FontStrikeoutType = NewValue;
		}
		public override StrikeoutType GetRunPropertyValue(TextRunBase run) {
			return run.FontStrikeoutType;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.FontStrikeoutType = NewValue;
			pos.MergedCharacterFormatting.FontStrikeoutType = NewValue;
		}
		public override StrikeoutType GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.FontStrikeoutType;
		}
	}
	#endregion
	#region RunBackColorModifier
	public class RunBackColorModifier : RunPropertyModifier<Color> {
		public RunBackColorModifier(Color backColor)
			: base(backColor) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.BackColor = NewValue;
		}
		public override Color GetRunPropertyValue(TextRunBase run) {
			return run.BackColor;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.BackColor = NewValue;
			pos.MergedCharacterFormatting.BackColor = NewValue;
		}
		public override Color GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.BackColor;
		}
	}
	#endregion
	#region RunForeColorModifier
	public class RunForeColorModifier : RunPropertyModifier<Color> {
		public RunForeColorModifier(Color foreColor)
			: base(foreColor) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.ForeColor = NewValue;
		}
		public override Color GetRunPropertyValue(TextRunBase run) {
			return run.ForeColor;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.ForeColor = NewValue;
			pos.MergedCharacterFormatting.ForeColor = NewValue;
		}
		public override Color GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.ForeColor;
		}
	}
	#endregion
	#region RunStrikeoutColorModifier
	public class RunStrikeoutColorModifier : RunPropertyModifier<Color> {
		public RunStrikeoutColorModifier(Color StrikeoutColor)
			: base(StrikeoutColor) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.StrikeoutColor = NewValue;
		}
		public override Color GetRunPropertyValue(TextRunBase run) {
			return run.StrikeoutColor;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.StrikeoutColor = NewValue;
			pos.MergedCharacterFormatting.StrikeoutColor = NewValue;
		}
		public override Color GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.StrikeoutColor;
		}
	}
	#endregion
	#region RunUnderlineColorModifier
	public class RunUnderlineColorModifier : RunPropertyModifier<Color> {
		public RunUnderlineColorModifier(Color UnderlineColor)
			: base(UnderlineColor) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.UnderlineColor = NewValue;
		}
		public override Color GetRunPropertyValue(TextRunBase run) {
			return run.UnderlineColor;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.UnderlineColor = NewValue;
			pos.MergedCharacterFormatting.UnderlineColor = NewValue;
		}
		public override Color GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.UnderlineColor;
		}
	}
	#endregion
	#region RunScriptModifier
	public class RunScriptModifier : RunPropertyModifier<CharacterFormattingScript> {
		public RunScriptModifier(CharacterFormattingScript val)
			: base(val) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.Script = NewValue;
		}
		public override CharacterFormattingScript GetRunPropertyValue(TextRunBase run) {
			return run.Script;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.Script = NewValue;
			pos.MergedCharacterFormatting.Script = NewValue;
		}
		public override CharacterFormattingScript GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.Script;
		}
	}
	#endregion
	#region RunFontModifier
	public class RunFontModifier : RunPropertyModifier<Font> {
		public RunFontModifier(Font font)
			: base(font) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			DocumentModel documentModel = run.Paragraph.DocumentModel;
			documentModel.History.BeginTransaction();
			try {
				CharacterPropertiesFontAssignmentHelper.AssignFont(run, NewValue);
			}
			finally {
				documentModel.History.EndTransaction();
			}
		}
		public override Font GetRunPropertyValue(TextRunBase run) {
			return null;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			CharacterFormattingBase formatting = pos.CharacterFormatting;
			formatting.BeginUpdate();
			try {
				CharacterPropertiesFontAssignmentHelper.AssignFont(formatting, NewValue);
			}
			finally {
				formatting.EndUpdate();
			}
		}
		public override Font GetInputPositionPropertyValue(InputPosition pos) {
			return null;
		}
	}
	#endregion
	#region RunIncrementFontSizeModifier
	public class RunIncrementFontSizeModifier : RunPropertyModifier<int> {
		public RunIncrementFontSizeModifier()
			: base(0) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.DoubleFontSize = PredefinedFontSizeCollection.ValidateFontSize(run.DoubleFontSize / 2 + 1) * 2;
		}
		public override int GetRunPropertyValue(TextRunBase run) {
			return run.DoubleFontSize;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			int value = PredefinedFontSizeCollection.ValidateFontSize(pos.CharacterFormatting.DoubleFontSize / 2 + 1) * 2;
			pos.CharacterFormatting.DoubleFontSize = value;
			pos.MergedCharacterFormatting.DoubleFontSize = value;
		}
		public override int GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.DoubleFontSize;
		}
	}
	#endregion
	#region RunDecrementFontSizeModifier
	public class RunDecrementFontSizeModifier : RunPropertyModifier<int> {
		public RunDecrementFontSizeModifier()
			: base(0) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.DoubleFontSize = PredefinedFontSizeCollection.ValidateFontSize(run.DoubleFontSize - 1);
		}
		public override int GetRunPropertyValue(TextRunBase run) {
			return run.DoubleFontSize;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			int value = PredefinedFontSizeCollection.ValidateFontSize(pos.CharacterFormatting.DoubleFontSize - 1);
			pos.CharacterFormatting.DoubleFontSize = value;
			pos.MergedCharacterFormatting.DoubleFontSize = value;
		}
		public override int GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.DoubleFontSize;
		}
	}
	#endregion
	#region RunIncreaseFontSizeModifier
	public class RunIncreaseFontSizeModifier : RunPropertyModifier<int> {
		PredefinedFontSizeCollection predefinedFontSizeCollection;
		public RunIncreaseFontSizeModifier(PredefinedFontSizeCollection predefinedFontSizeCollection)
			: base(0) {
			this.predefinedFontSizeCollection = predefinedFontSizeCollection;
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.DoubleFontSize = predefinedFontSizeCollection.CalculateNextFontSize(run.DoubleFontSize);
		}
		public override int GetRunPropertyValue(TextRunBase run) {
			return run.DoubleFontSize;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			int value = predefinedFontSizeCollection.CalculateNextFontSize(pos.CharacterFormatting.DoubleFontSize);
			pos.CharacterFormatting.DoubleFontSize = value;
			pos.MergedCharacterFormatting.DoubleFontSize = value;
		}
		public override int GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.DoubleFontSize;
		}
	}
	#endregion
	#region RunDecreaseFontSizeModifier
	public class RunDecreaseFontSizeModifier : RunPropertyModifier<int> {
		readonly PredefinedFontSizeCollection predefinedFontSizeCollection;
		public RunDecreaseFontSizeModifier(PredefinedFontSizeCollection predefinedFontSizeCollection)
			: base(0) {
			this.predefinedFontSizeCollection = predefinedFontSizeCollection;
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.DoubleFontSize = predefinedFontSizeCollection.CalculatePreviousFontSize(run.DoubleFontSize);
		}
		public override int GetRunPropertyValue(TextRunBase run) {
			return run.DoubleFontSize;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			int value = predefinedFontSizeCollection.CalculatePreviousFontSize(pos.CharacterFormatting.DoubleFontSize);
			pos.CharacterFormatting.DoubleFontSize = value;
			pos.MergedCharacterFormatting.DoubleFontSize = value;
		}
		public override int GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.DoubleFontSize;
		}
	}
	#endregion
	#region RunResetUseModifier
	public class RunResetUseModifier : RunPropertyModifier<bool> {
		public RunResetUseModifier(bool value)
			: base(value) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			if (runIndex < new RunIndex(run.Paragraph.PieceTable.Runs.Count - 1)) {
				run.CharacterProperties.ResetAllUse();
			}
		}
		public override bool GetRunPropertyValue(TextRunBase run) {
			return run.CharacterProperties.Info.Options.Value == CharacterFormattingOptions.Mask.UseNone;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
		}
		public override bool GetInputPositionPropertyValue(InputPosition pos) {
			return false;
		}
	}
	#endregion
	#region RunResetUseMaskModifier
	public class RunResetUseMaskModifier : RunPropertyModifier<CharacterFormattingOptions.Mask> {
		public RunResetUseMaskModifier(CharacterFormattingOptions.Mask mask)
			: base(mask) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			if (runIndex < new RunIndex(run.Paragraph.PieceTable.Runs.Count - 1)) {
				run.CharacterProperties.ResetUse(this.NewValue);
			}
		}
		public override CharacterFormattingOptions.Mask GetRunPropertyValue(TextRunBase run) {
			return run.CharacterProperties.Info.Options.Value;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {			
		}
		public override CharacterFormattingOptions.Mask GetInputPositionPropertyValue(InputPosition pos) {			
			return pos.CharacterFormatting.Options.Value;
		}
	}
	#endregion
	#region InlinePictureScaleModifier
	public class InlinePictureScaleModifier : InlinePictureRunPropertyModifier<int> {
		int newScaleY;
		public InlinePictureScaleModifier(int newScaleX, int newScaleY)
			: base(newScaleX) {
			this.newScaleY = ValidateNewValue(newScaleY);
		}
		public override void ModifyPictureRun(InlinePictureRun run, RunIndex runIndex) {
			run.ScaleX = NewValue;
			run.ScaleY = newScaleY;
		}
	}
	#endregion
	#region RectangularObjectWidthModifier
	public class RectangularObjectWidthModifier : RectangularObjectPropertyModifier<int> {
		public RectangularObjectWidthModifier(int newValue)
			: base(newValue) {
		}
		public override void ModifyRectangularObject(IRectangularObject rectangularObject, RunIndex runIndex) {
			rectangularObject.ActualSize = new Size(NewValue, rectangularObject.ActualSize.Height);
		}
	}
	#endregion
	#region RectangularObjectHeightModifier
	public class RectangularObjectHeightModifier : RectangularObjectPropertyModifier<int> {
		public RectangularObjectHeightModifier(int newValue)
			: base(newValue) {
		}
		public override void ModifyRectangularObject(IRectangularObject rectangularObject, RunIndex runIndex) {
			rectangularObject.ActualSize = new Size(rectangularObject.ActualSize.Width, NewValue);
		}
	}
	#endregion
	#region RectangularObjectSizeModifier
	public class RectangularObjectSizeModifier : RectangularObjectPropertyModifier<Size> {		
		public RectangularObjectSizeModifier(Size newSize)
			: base(newSize) {			
		}
		public override void ModifyRectangularObject(IRectangularObject rectangularObject, RunIndex runIndex) {
			rectangularObject.ActualSize = NewValue;			
		}
	}
	#endregion
	#region RectangularObjectScaleModifier
	public class RectangularObjectScaleModifier : RectangularObjectPropertyModifier<int> {
		int newScaleY;
		public RectangularObjectScaleModifier(int newScaleX, int newScaleY)
			: base(newScaleX) {
			this.newScaleY = ValidateNewValue(newScaleY);
		}
		public override void ModifyRectangularObject(IRectangularObject rectangularObject, RunIndex runIndex) {
			IRectangularScalableObject scalableObject = rectangularObject as IRectangularScalableObject;
			if (scalableObject == null)
				return;
			scalableObject.ScaleX = NewValue;
			scalableObject.ScaleY = newScaleY;
		}
	}
	#endregion
	#region ParagraphAlignmentModifier
	public class ParagraphAlignmentModifier : ParagraphPropertyModifier<ParagraphAlignment> {
		public ParagraphAlignmentModifier(ParagraphAlignment alignment)
			: base(alignment) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.Alignment = NewValue;
		}
		public override ParagraphAlignment GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.Alignment;
		}
	}
	#endregion
	#region ParagraphLeftIndentModifier
	public class ParagraphLeftIndentModifier : ParagraphPropertyModifier<int> {
		public ParagraphLeftIndentModifier(int leftIndent)
			: base(leftIndent) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.LeftIndent = NewValue;
		}
		public override int GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.LeftIndent;
		}
	}
	#endregion
	#region ParagraphRightIndentModifier
	public class ParagraphRightIndentModifier : ParagraphPropertyModifier<int> {
		public ParagraphRightIndentModifier(int rightIndent)
			: base(rightIndent) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.RightIndent = NewValue;
		}
		public override int GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.RightIndent;
		}
	}
	#endregion
	#region AssignParagraphLeftIndentModifier
	public class AssignParagraphLeftIndentModifier : ParagraphPropertyModifier<int> {
		int maxValue;
		public AssignParagraphLeftIndentModifier(int leftIndent, int maxValue)
			: base(leftIndent) {
			this.maxValue = maxValue;
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			int newLeftIndent = paragraph.LeftIndent + NewValue;
			if (newLeftIndent >= 0) {
				if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging) {
					int firstLineLeftIndent = newLeftIndent - paragraph.FirstLineIndent;
					if (firstLineLeftIndent < 0)
						newLeftIndent -= firstLineLeftIndent;
				}
				paragraph.LeftIndent = Math.Min(maxValue, newLeftIndent);
				if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Indented) {
					int distanceToRight = maxValue - (paragraph.LeftIndent + paragraph.FirstLineIndent);
					if (distanceToRight < 0)
						paragraph.FirstLineIndent += distanceToRight;
				}
			}
		}
		public override int GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.LeftIndent;
		}
	}
	#endregion
	#region ParagraphFirstLineIndentModifier
	public class ParagraphFirstLineIndentModifier : ParagraphPropertyModifier<int> {
		int maxIndent;
		public ParagraphFirstLineIndentModifier(int firstLineIndent)
			: this(firstLineIndent, Int32.MaxValue / 4) {
		}
		public ParagraphFirstLineIndentModifier(int firstLineIndent, int maxIndent)
			: base(firstLineIndent) {
			this.maxIndent = maxIndent;
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			if (NewValue > 0) {
				paragraph.FirstLineIndentType = ParagraphFirstLineIndent.Indented;
				int value = NewValue;
				int distanceToRight = maxIndent - (paragraph.LeftIndent + value);
				if (distanceToRight < 0)
					value += distanceToRight;
				paragraph.FirstLineIndent = value;
			}
			else {
				paragraph.FirstLineIndentType = ParagraphFirstLineIndent.Hanging;
				paragraph.FirstLineIndent = -NewValue;
			}
		}
		public override int GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.FirstLineIndent;
		}
	}
	#endregion
	#region ParagraphSpacingModifier
	public class ParagraphSpacingModifier : ParagraphPropertyModifier<ParagraphLineSpacing> {
		public ParagraphSpacingModifier(ParagraphLineSpacing lineSpacingType)
			: base(lineSpacingType) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.LineSpacingType = NewValue;
		}
		public override ParagraphLineSpacing GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.LineSpacingType;
		}
	}
	#endregion
	#region ParagraphSpacingBeforeModifier
	public class ParagraphSpacingBeforeModifier : ParagraphPropertyModifier<int> {
		public ParagraphSpacingBeforeModifier(int value)
			: base(value) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.SpacingBefore = NewValue;
		}
		public override int GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.SpacingBefore;
		}
	}
	#endregion
	#region ParagraphSpacingAfterModifier
	public class ParagraphSpacingAfterModifier : ParagraphPropertyModifier<int> {
		public ParagraphSpacingAfterModifier(int value)
			: base(value) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.SpacingAfter = NewValue;
		}
		public override int GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.SpacingAfter;
		}
	}
	#endregion
	#region ParagraphClearParagraphFormattingModifier
	public class ParagraphClearParagraphFormattingModifier : ParagraphPropertyModifier<bool> {
		public ParagraphClearParagraphFormattingModifier(bool theThing)
			: base(theThing) {
		}
		public override bool GetParagraphPropertyValue(Paragraph paragraph) {
			return true;
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.ParagraphProperties.Reset();
		}
	}
	#endregion
	#region ParagraphSuppressLineNumbersModifier
	public class ParagraphSuppressLineNumbersModifier : ParagraphPropertyModifier<bool> {
		public ParagraphSuppressLineNumbersModifier(bool value)
			: base(value) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.SuppressLineNumbers = NewValue;
		}
		public override bool GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.SuppressLineNumbers;
		}
	}
	#endregion
	#region ParagraphSuppressHyphenationModifier
	public class ParagraphSuppressHyphenationModifier : ParagraphPropertyModifier<bool> {
		public ParagraphSuppressHyphenationModifier(bool value)
			: base(value) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			paragraph.SuppressHyphenation = NewValue;
		}
		public override bool GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.SuppressHyphenation;
		}
	}
	#endregion
	#region ParagraphBackColorModifier
	public class ParagraphBackColorModifier : ParagraphPropertyModifier<Color> {
		public ParagraphBackColorModifier(Color value)
			: base(value) {
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			if(paragraph.GetCell() != null && DXColor.IsEmpty(NewValue))
				paragraph.BackColor = DXColor.White;
			else
				paragraph.BackColor = NewValue;
		}
		public override Color GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.BackColor;
		}
	}
	#endregion
	#region RunFontColorPropertyModifier
	public class RunFontColorPropertyModifier : RunPropertyModifier<Color> {
		public RunFontColorPropertyModifier(Color val)
			: base(val) {
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			run.ForeColor = NewValue;
		}
		public override Color GetRunPropertyValue(TextRunBase run) {
			return run.ForeColor;
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			pos.CharacterFormatting.ForeColor = NewValue;
			pos.MergedCharacterFormatting.ForeColor = NewValue;
		}
		public override Color GetInputPositionPropertyValue(InputPosition pos) {
			return pos.MergedCharacterFormatting.ForeColor;
		}
	}
	#endregion
	#region FontPropertiesModifier
	public class FontPropertiesModifier : MergedRunPropertyModifier<MergedCharacterProperties> {
		public FontPropertiesModifier(MergedCharacterProperties newValue)
			: base(newValue) {
		}
		public override MergedCharacterProperties GetRunPropertyValue(TextRunBase run) {
			CharacterFormattingInfo formattingInfo = run.MergedCharacterFormatting;
			CharacterFormattingOptions options = new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseAll);
			return new MergedCharacterProperties(formattingInfo, options);
		}
		protected internal override bool CanModifyRun(TextRunBase run) {
			return !(run is SeparatorTextRun);
		}
		public override void ModifyTextRun(TextRunBase run, RunIndex runIndex) {
			ApplyCharacterProperties(run);
		}
		public override void ModifyInputPositionCore(InputPosition pos) {
			ApplyCharacterProperties(pos.CharacterFormatting);
			ApplyCharacterProperties(pos.MergedCharacterFormatting);
		}
		protected internal virtual void ApplyCharacterProperties(ICharacterProperties properties) {
			CharacterFormattingInfo info = NewValue.Info;
			CharacterFormattingOptions options = NewValue.Options;
			if (options.UseFontName)
				properties.FontName = info.FontName;
			if (options.UseFontBold)
				properties.FontBold = info.FontBold;
			if (options.UseFontItalic)
				properties.FontItalic = info.FontItalic;
			if (options.UseDoubleFontSize)
				properties.DoubleFontSize = info.DoubleFontSize;
			if (options.UseForeColor)
				properties.ForeColor = info.ForeColor;
			if (options.UseFontUnderlineType)
				properties.FontUnderlineType = info.FontUnderlineType;
			if (options.UseUnderlineColor)
				properties.UnderlineColor = info.UnderlineColor;
			if (options.UseFontStrikeoutType)
				properties.FontStrikeoutType = info.FontStrikeoutType;
			if (options.UseScript)
				properties.Script = info.Script;
			if (options.UseAllCaps)
				properties.AllCaps = info.AllCaps;
			if (options.UseUnderlineWordsOnly)
				properties.UnderlineWordsOnly = info.UnderlineWordsOnly;
			if (options.UseHidden)
				properties.Hidden = info.Hidden;
		}
		public override MergedCharacterProperties GetInputPositionPropertyValue(InputPosition pos) {
			return new MergedCharacterProperties(pos.MergedCharacterFormatting, new CharacterFormattingOptions(CharacterFormattingOptions.Mask.UseAll));
		}
		public override MergedCharacterProperties Merge(MergedCharacterProperties leftValue, MergedCharacterProperties rightValue) {
			MergedCharacterProperties value = new MergedCharacterProperties(leftValue.Info, leftValue.Options);
			CharacterFormattingOptions targetOptions = value.Options;
			CharacterFormattingInfo targetInfo = value.Info;
			CharacterFormattingInfo runInfo = rightValue.Info;
			targetOptions.UseAllCaps = targetOptions.UseAllCaps && targetInfo.AllCaps == runInfo.AllCaps;
			targetOptions.UseBackColor = targetOptions.UseBackColor && targetInfo.BackColor == runInfo.BackColor;
			targetOptions.UseFontBold = targetOptions.UseFontBold && targetInfo.FontBold == runInfo.FontBold;
			targetOptions.UseFontItalic = targetOptions.UseFontItalic && targetInfo.FontItalic == runInfo.FontItalic;
			targetOptions.UseFontName = targetOptions.UseFontName && targetInfo.FontName == runInfo.FontName;
			targetOptions.UseDoubleFontSize = targetOptions.UseDoubleFontSize && targetInfo.DoubleFontSize == runInfo.DoubleFontSize;
			targetOptions.UseFontStrikeoutType = targetOptions.UseFontStrikeoutType && targetInfo.FontStrikeoutType == runInfo.FontStrikeoutType;
			targetOptions.UseFontUnderlineType = targetOptions.UseFontUnderlineType && targetInfo.FontUnderlineType == runInfo.FontUnderlineType;
			targetOptions.UseForeColor = targetOptions.UseForeColor && targetInfo.ForeColor == runInfo.ForeColor;
			targetOptions.UseScript = targetOptions.UseScript && targetInfo.Script == runInfo.Script;
			targetOptions.UseStrikeoutColor = targetOptions.UseStrikeoutColor && targetInfo.StrikeoutColor == runInfo.StrikeoutColor;
			targetOptions.UseStrikeoutWordsOnly = targetOptions.UseStrikeoutWordsOnly && targetInfo.StrikeoutWordsOnly == runInfo.StrikeoutWordsOnly;
			targetOptions.UseUnderlineColor = targetOptions.UseUnderlineColor && targetInfo.UnderlineColor == runInfo.UnderlineColor;
			targetOptions.UseUnderlineWordsOnly = targetOptions.UseUnderlineWordsOnly && targetInfo.UnderlineWordsOnly == runInfo.UnderlineWordsOnly;
			targetOptions.UseHidden = targetOptions.UseHidden && targetInfo.Hidden == runInfo.Hidden;
			return value;
		}
	}
	#endregion
	#region ParagraphPropertiesModifier
	public class ParagraphPropertiesModifier : MergedParagraphPropertyModifier<MergedParagraphProperties> {
		public ParagraphPropertiesModifier(MergedParagraphProperties newValue)
			: base(newValue) {
		}
		public override MergedParagraphProperties GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.GetMergedParagraphProperties();
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			ParagraphFormattingInfo info = NewValue.Info;
			ParagraphFormattingOptions options = NewValue.Options;
			if (options.UseRightIndent)
				paragraph.RightIndent = info.RightIndent;
			if (options.UseLeftIndent)
				paragraph.LeftIndent = info.LeftIndent;
			if (options.UseFirstLineIndent) {
				paragraph.FirstLineIndentType = info.FirstLineIndentType;
				paragraph.FirstLineIndent = info.FirstLineIndent;
			}
			if (options.UseAlignment)
				paragraph.Alignment = info.Alignment;
			if (options.UseSpacingAfter)
				paragraph.SpacingAfter = info.SpacingAfter;
			if (options.UseSpacingBefore)
				paragraph.SpacingBefore = info.SpacingBefore;
			if (options.UseLineSpacing) {
				paragraph.LineSpacingType = info.LineSpacingType;
				paragraph.LineSpacing = info.LineSpacing;
			}
			if (options.UseSuppressHyphenation)
				paragraph.SuppressHyphenation = info.SuppressHyphenation;
			if (options.UseSuppressLineNumbers)
				paragraph.SuppressLineNumbers = info.SuppressLineNumbers;
			if (options.UseContextualSpacing) 
				paragraph.ContextualSpacing = info.ContextualSpacing;
			if (options.UsePageBreakBefore) 
				paragraph.PageBreakBefore = info.PageBreakBefore;
			if (options.UseBeforeAutoSpacing) 
				paragraph.BeforeAutoSpacing = info.BeforeAutoSpacing;
			if (options.UseAfterAutoSpacing)
				paragraph.AfterAutoSpacing = info.AfterAutoSpacing;
			if (options.UseKeepWithNext)
				paragraph.KeepWithNext = info.KeepWithNext;
			if (options.UseKeepLinesTogether)
				paragraph.KeepLinesTogether = info.KeepLinesTogether;
			if (options.UseWidowOrphanControl) 
				paragraph.WidowOrphanControl = info.WidowOrphanControl;
			if (options.UseOutlineLevel)
				paragraph.OutlineLevel = info.OutlineLevel;
			if (options.UseBackColor)
				paragraph.BackColor = info.BackColor;
		}
		public override MergedParagraphProperties Merge(MergedParagraphProperties leftValue, MergedParagraphProperties rightValue) {
			MergedParagraphProperties value = new MergedParagraphProperties(leftValue.Info, leftValue.Options);
			ParagraphFormattingOptions targetOptions = value.Options;
			ParagraphFormattingInfo targetInfo = value.Info;
			ParagraphFormattingInfo runInfo = rightValue.Info;
			targetOptions.UseLeftIndent = targetOptions.UseLeftIndent && targetInfo.LeftIndent == runInfo.LeftIndent;
			targetOptions.UseRightIndent = targetOptions.UseRightIndent && targetInfo.RightIndent == runInfo.RightIndent;
			targetOptions.UseFirstLineIndent = targetOptions.UseFirstLineIndent && targetInfo.FirstLineIndent == runInfo.FirstLineIndent && targetInfo.FirstLineIndentType == runInfo.FirstLineIndentType;
			targetOptions.UseAlignment = targetOptions.UseAlignment && targetInfo.Alignment == runInfo.Alignment;
			targetOptions.UseSpacingBefore = targetOptions.UseSpacingBefore && targetInfo.SpacingBefore == runInfo.SpacingBefore;
			targetOptions.UseSpacingAfter = targetOptions.UseSpacingAfter && targetInfo.SpacingAfter == runInfo.SpacingAfter;
			targetOptions.UseLineSpacing = targetOptions.UseLineSpacing && targetInfo.LineSpacing == runInfo.LineSpacing && targetInfo.LineSpacingType == runInfo.LineSpacingType;
			targetOptions.UseSuppressHyphenation = targetOptions.UseSuppressHyphenation && targetInfo.SuppressHyphenation == runInfo.SuppressHyphenation;
			targetOptions.UseSuppressLineNumbers = targetOptions.UseSuppressLineNumbers && targetInfo.SuppressLineNumbers == runInfo.SuppressLineNumbers;
			targetOptions.UseContextualSpacing = targetOptions.UseContextualSpacing && targetInfo.ContextualSpacing == runInfo.ContextualSpacing;
			targetOptions.UsePageBreakBefore = targetOptions.UsePageBreakBefore && targetInfo.PageBreakBefore == runInfo.PageBreakBefore;
			targetOptions.UseBeforeAutoSpacing = targetOptions.UseBeforeAutoSpacing && targetInfo.BeforeAutoSpacing == runInfo.BeforeAutoSpacing;
			targetOptions.UseAfterAutoSpacing = targetOptions.UseAfterAutoSpacing && targetInfo.AfterAutoSpacing == runInfo.AfterAutoSpacing;
			targetOptions.UseKeepWithNext = targetOptions.UseKeepWithNext && targetInfo.KeepWithNext == runInfo.KeepWithNext;
			targetOptions.UseKeepLinesTogether = targetOptions.UseKeepLinesTogether && targetInfo.KeepLinesTogether == runInfo.KeepLinesTogether;
			targetOptions.UseWidowOrphanControl = targetOptions.UseWidowOrphanControl && targetInfo.WidowOrphanControl == runInfo.WidowOrphanControl;
			targetOptions.UseOutlineLevel = targetOptions.UseOutlineLevel && targetInfo.OutlineLevel == runInfo.OutlineLevel;
			targetOptions.UseBackColor = targetOptions.UseBackColor && targetInfo.BackColor == runInfo.BackColor;
			return value;
		}
	}
	#endregion
	#region TabFormattingInfoModifier
	public class TabFormattingInfoModifier : MergedParagraphPropertyModifier<TabFormattingInfo> {
		int newDefaultTabWidth;
		public TabFormattingInfoModifier(TabFormattingInfo newValue, int defaultTabWidth)
			: base(newValue) {
			this.NewDefaultTabWidth = defaultTabWidth;
		}
		public int NewDefaultTabWidth { get { return newDefaultTabWidth; } set { newDefaultTabWidth = value; } }
		public override TabFormattingInfo GetParagraphPropertyValue(Paragraph paragraph) {
			return paragraph.GetTabs();
		}
		public override void ModifyParagraph(Paragraph paragraph, ParagraphIndex paragraphIndex) {
			TabFormattingInfo result = GetMergedTabInfo(NewValue, paragraph.GetOwnTabs(), paragraph.ParagraphStyle.GetTabs());
			paragraph.SetOwnTabs(result);
			paragraph.DocumentModel.DocumentProperties.DefaultTabWidth = NewDefaultTabWidth;
		}
		protected internal TabFormattingInfo GetMergedTabInfo(TabFormattingInfo newTabInfo, TabFormattingInfo oldOwnTabInfo, TabFormattingInfo styleTabInfo) {
			TabFormattingInfo merged = TabFormattingInfo.Merge(newTabInfo, styleTabInfo);
			int count = merged.Count;
			for (int i = 0; i < count; i++) {
				TabInfo item = merged[i];
				if (!newTabInfo.Contains(item) && !oldOwnTabInfo.Contains(item)) 
					newTabInfo.Add(new TabInfo(item.Position, item.Alignment, item.Leader, true));
			}
			return newTabInfo;
		}
		public override TabFormattingInfo Merge(TabFormattingInfo leftValue, TabFormattingInfo rightValue) {
			if (leftValue.Equals(rightValue))
				return leftValue.Clone();
			TabFormattingInfo result = new TabFormattingInfo();
			return result;
		}
	}
	#endregion
	#region SectionPropertyModifierBase
	public abstract class SectionPropertyModifierBase {
		public abstract void ModifySection(Section section, SectionIndex sectionIndex);
	}
	#endregion
	#region SectionPropertyModifier<T>
	public abstract class SectionPropertyModifier<T> : SectionPropertyModifierBase {
		readonly T newValue;
		protected SectionPropertyModifier(T newValue) {
			this.newValue = newValue;
		}
		public T NewValue { get { return newValue; } }
		public abstract T GetSectionPropertyValue(Section section);
	}
	#endregion
	#region MergedSectionPropertyModifier
	public abstract class MergedSectionPropertyModifier<T> : SectionPropertyModifier<T> {
		protected MergedSectionPropertyModifier(T newValue)
			: base(newValue) {
		}
		public abstract T Merge(T leftValue, T rightValue);
	}
	#endregion
	#region SectionPageOrientationLandscapeModifier
	public class SectionPageOrientationLandscapeModifier : SectionPropertyModifier<bool> {
		public SectionPageOrientationLandscapeModifier(bool value)
			: base(value) {
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			if (section.Page.Landscape == NewValue)
				return;
			section.Page.Landscape = NewValue;
			int width = section.Page.Width;
			section.Page.Width = section.Page.Height;
			section.Page.Height = width;
		}
		public override bool GetSectionPropertyValue(Section section) {
			return section.Page.Landscape;
		}
	}
	#endregion
	#region PageMargins
	public struct PageMargins {
		int left;
		int right;
		int top;
		int bottom;
		public int Left { get { return left; } set { left = value; } }
		public int Right { get { return right; } set { right = value; } }
		public int Top { get { return top; } set { top = value; } }
		public int Bottom { get { return bottom; } set { bottom = value; } }
	}
	#endregion
	#region SectionPageMarginsModifier
	public class SectionPageMarginsModifier : SectionPropertyModifier<PageMargins> {
		public SectionPageMarginsModifier(PageMargins value)
			: base(value) {
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			SectionMargins margins = section.Margins;
			PageMargins value = NewValue;
			margins.Left = value.Left;
			margins.Right = value.Right;
			margins.Top = value.Top;
			margins.Bottom = value.Bottom;
		}
		public override PageMargins GetSectionPropertyValue(Section section) {
			SectionMargins margins = section.Margins;
			PageMargins value = new PageMargins();
			value.Left = margins.Left;
			value.Right = margins.Right;
			value.Top = margins.Top;
			value.Bottom = margins.Bottom;
			return value;
		}
	}
	#endregion
	#region SectionPaperKindAndSizeModifier
	public class SectionPaperKindAndSizeModifier : SectionPropertyModifier<PaperKind> {
		public SectionPaperKindAndSizeModifier(PaperKind value)
			: base(value) {
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			SectionPage page = section.Page;
			page.PaperKind = NewValue;
			if (NewValue != PaperKind.Custom) {
				Size paperSizeInTwips = PaperSizeCalculator.CalculatePaperSize(NewValue);
				DocumentModelUnitConverter unitConverter = section.DocumentModel.UnitConverter;
				if (page.Landscape) {
					page.Height = unitConverter.TwipsToModelUnits(paperSizeInTwips.Width);
					page.Width = unitConverter.TwipsToModelUnits(paperSizeInTwips.Height);
				}
				else {
					page.Width = unitConverter.TwipsToModelUnits(paperSizeInTwips.Width);
					page.Height = unitConverter.TwipsToModelUnits(paperSizeInTwips.Height);
				}
			}
		}
		public override PaperKind GetSectionPropertyValue(Section section) {
			return section.Page.PaperKind;
		}
	}
	#endregion
	#region SectionLineNumberingStepAndRestartModifier
	public class SectionLineNumberingStepAndRestartModifier : SectionPropertyModifier<LineNumberingRestart> {
		public SectionLineNumberingStepAndRestartModifier(LineNumberingRestart value)
			: base(value) {
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			section.LineNumbering.NumberingRestartType = NewValue;
			section.LineNumbering.Step = NewValue == (LineNumberingRestart)(-1) ? 0 : 1;
		}
		public override LineNumberingRestart GetSectionPropertyValue(Section section) {
			if (section.LineNumbering.Step > 0)
				return section.LineNumbering.NumberingRestartType;
			else
				return (LineNumberingRestart)(-1);
		}
	}
	#endregion
	#region SectionLineNumberingModifier
	public class SectionLineNumberingModifier : MergedSectionPropertyModifier<LineNumberingInfo> {
		public SectionLineNumberingModifier(LineNumberingInfo newValue)
			: base(newValue) {
		}
		public override LineNumberingInfo GetSectionPropertyValue(Section section) {
			return section.LineNumbering.Info.Clone();
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			section.LineNumbering.ReplaceInfo(NewValue, section.LineNumbering.GetBatchUpdateChangeActions());
		}
		public override LineNumberingInfo Merge(LineNumberingInfo leftValue, LineNumberingInfo rightValue) {
			LineNumberingInfo value = new LineNumberingInfo();
			value.CopyFrom(rightValue);
			return value;
		}
	}
	#endregion
	#region SectionPageSetupModifier
	public class SectionPageSetupModifier : MergedSectionPropertyModifier<PageSetupInfo> {
		public SectionPageSetupModifier(PageSetupInfo newValue)
			: base(newValue) {
		}
		public override PageSetupInfo GetSectionPropertyValue(Section section) {
			PageSetupInfo result = new PageSetupInfo();
			SectionMargins margins = section.Margins;
			result.LeftMargin = margins.Left;
			result.RightMargin = margins.Right;
			result.TopMargin = margins.Top;
			result.BottomMargin = margins.Bottom;
			SectionPage pageInfo = section.Page;
			result.PaperWidth = pageInfo.Width;
			result.PaperHeight = pageInfo.Height;
			result.PaperKind = pageInfo.PaperKind;
			result.Landscape = pageInfo.Landscape;
			SectionGeneralSettings generalSettings = section.GeneralSettings;
			result.DifferentFirstPage = generalSettings.DifferentFirstPage;
			result.SectionStartType = generalSettings.StartType;
			result.DifferentOddAndEvenPages = section.DocumentModel.DocumentProperties.DifferentOddAndEvenPages;
			return result;
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			SectionPage pageInfo = section.Page;
			bool oldLandscape = pageInfo.Landscape;
			if (NewValue.Landscape.HasValue)
				pageInfo.Landscape = NewValue.Landscape.Value;
			bool newLandscape = pageInfo.Landscape;
			if (NewValue.PaperKind.HasValue)
				pageInfo.PaperKind = NewValue.PaperKind.Value;
			if (oldLandscape != newLandscape)
				ChangeOrientation(section, newLandscape);
			if (NewValue.PaperWidth.HasValue)
				pageInfo.Width = NewValue.PaperWidth.Value;
			if (NewValue.PaperHeight.HasValue)
				pageInfo.Height = NewValue.PaperHeight.Value;
			SectionMargins margins = section.Margins;
			if (NewValue.LeftMargin.HasValue)
				margins.Left = NewValue.LeftMargin.Value;
			if (NewValue.RightMargin.HasValue)
				margins.Right = NewValue.RightMargin.Value;
			if (NewValue.TopMargin.HasValue)
				margins.Top = NewValue.TopMargin.Value;
			if (NewValue.BottomMargin.HasValue)
				margins.Bottom = NewValue.BottomMargin.Value;
			SectionGeneralSettings generalSettings = section.GeneralSettings;
			if (NewValue.DifferentFirstPage.HasValue)
				generalSettings.DifferentFirstPage = NewValue.DifferentFirstPage.Value;
			if (NewValue.SectionStartType.HasValue)
				generalSettings.StartType = NewValue.SectionStartType.Value;
			if (NewValue.DifferentOddAndEvenPages.HasValue)
				section.DocumentModel.DocumentProperties.DifferentOddAndEvenPages = NewValue.DifferentOddAndEvenPages.Value;
		}
		protected internal virtual void ChangeOrientation(Section section, bool landscape) {
			SectionPage pageInfo = section.Page;
			int value = pageInfo.Width;
			pageInfo.Width = pageInfo.Height;
			pageInfo.Height = value;
			SectionMargins margins = section.Margins;
			int left = margins.Left;
			int right = margins.Right;
			int top = margins.Top;
			int bottom = margins.Bottom;
			if (landscape) {
				margins.Left = top;
				margins.Right = bottom;
				margins.Top = right;
				margins.Bottom = left;
			}
			else {
				margins.Left = bottom;
				margins.Right = top;
				margins.Top = left;
				margins.Bottom = right;
			}
		}
		public override PageSetupInfo Merge(PageSetupInfo leftValue, PageSetupInfo rightValue) {
			PageSetupInfo value = new PageSetupInfo();
			value.AvailableApplyType = leftValue.AvailableApplyType;
			value.ApplyType = leftValue.ApplyType;
			if (leftValue.Landscape == rightValue.Landscape)
				value.Landscape = leftValue.Landscape;
			if (leftValue.LeftMargin == rightValue.LeftMargin)
				value.LeftMargin = leftValue.LeftMargin;
			if (leftValue.RightMargin == rightValue.RightMargin)
				value.RightMargin = leftValue.RightMargin;
			if (leftValue.TopMargin == rightValue.TopMargin)
				value.TopMargin = leftValue.TopMargin;
			if (leftValue.BottomMargin == rightValue.BottomMargin)
				value.BottomMargin = leftValue.BottomMargin;
			if (leftValue.PaperWidth == rightValue.PaperWidth)
				value.PaperWidth = leftValue.PaperWidth;
			if (leftValue.PaperHeight == rightValue.PaperHeight)
				value.PaperHeight = leftValue.PaperHeight;
			if (leftValue.PaperKind == rightValue.PaperKind)
				value.PaperKind = leftValue.PaperKind;
			if (leftValue.SectionStartType == rightValue.SectionStartType)
				value.SectionStartType = leftValue.SectionStartType;
			if (leftValue.DifferentFirstPage == rightValue.DifferentFirstPage)
				value.DifferentFirstPage = leftValue.DifferentFirstPage;
			if (leftValue.DifferentOddAndEvenPages == rightValue.DifferentOddAndEvenPages)
				value.DifferentOddAndEvenPages = leftValue.DifferentOddAndEvenPages;
			return value;
		}
	}
	#endregion
	#region SectionColumnsSetupModifier
	public class SectionColumnsSetupModifier : MergedSectionPropertyModifier<ColumnsInfoUI> {
		public SectionColumnsSetupModifier(ColumnsInfoUI newValue)
			: base(newValue) {
		}
		public override ColumnsInfoUI GetSectionPropertyValue(Section section) {
			ColumnsInfoUI result = new ColumnsInfoUI();
			result.PageWidth = section.Page.Width - (section.Margins.Left + section.Margins.Right);
			result.EqualColumnWidth = section.Columns.EqualWidthColumns;
			if (section.Columns.EqualWidthColumns)
				result.ChangeColumnCount(section.Columns.ColumnCount);
			else
				result.ChangeColumnCount(section.Columns.GetColumns().Count);
			if (section.Columns.EqualWidthColumns) {
				result.CalculateUniformColumnsByColumnSpacing(section.Columns.Space);
			}
			else {
				ColumnInfoCollection columnInfoCollection = section.Columns.GetColumns();
				for (int i = 0; i < result.ColumnCount; i++) {
					result.Columns[i].Width = columnInfoCollection[i].Width;
					result.Columns[i].Spacing = columnInfoCollection[i].Space;
				}
			}
			return result;
		}
		public override void ModifySection(Section section, SectionIndex sectionIndex) {
			section.Columns.ColumnCount = Math.Max(section.Columns.ColumnCount, section.Columns.GetColumns().Count);
			bool previousEqualColumnValue = section.Columns.EqualWidthColumns;
			ColumnInfoCollection columnInfoCollection = section.Columns.GetColumns();
			if (NewValue.EqualColumnWidth.HasValue)
				section.Columns.EqualWidthColumns = NewValue.EqualColumnWidth.Value;
			if (NewValue.ColumnCount.HasValue && NewValue.ColumnCount.Value > 0)
				section.Columns.ColumnCount = NewValue.ColumnCount.Value;
			if (section.Columns.EqualWidthColumns) {
				if (NewValue.Columns.Count > 0 && NewValue.Columns[0].Spacing.HasValue)
					section.Columns.Space = NewValue.Columns[0].Spacing.Value;
				else
					if (columnInfoCollection.Count > 0)
						section.Columns.Space = columnInfoCollection[0].Space;
			}
			else {
				Guard.Equals(NewValue.EqualColumnWidth, section.Columns.EqualWidthColumns);
				Guard.Equals(NewValue.ColumnCount, section.Columns.ColumnCount);
				ColumnsInfoUI columnsInfo = NewValue.Clone();
				if (columnsInfo.HasColumnsInfoUINull()) {
					if ((!columnsInfo.ColumnCount.HasValue || columnsInfo.Columns.Count <= 0) && (columnInfoCollection.Count == section.Columns.ColumnCount) && (!previousEqualColumnValue))
						return;
					if (!columnsInfo.EqualColumnWidth.HasValue)
						columnsInfo.EqualColumnWidth = section.Columns.EqualWidthColumns;
					if (!columnsInfo.ColumnCount.HasValue)
						columnsInfo.ChangeColumnCount(section.Columns.ColumnCount);
					columnsInfo.CalculateUniformColumnsByColumnSpacing(section.Columns.Space);
				}
				columnInfoCollection.Clear();
				for (int i = 0; i < section.Columns.ColumnCount; i++) {
					ColumnInfo columnInfo = new ColumnInfo();
					columnInfo.Width = columnsInfo.Columns[i].Width.Value;
					columnInfo.Space = columnsInfo.Columns[i].Spacing.Value;
					columnInfoCollection.Add(columnInfo);
				}
				section.Columns.SetColumns(columnInfoCollection);
			}
		}
		public override ColumnsInfoUI Merge(ColumnsInfoUI leftValue, ColumnsInfoUI rightValue) {
			ColumnsInfoUI value = new ColumnsInfoUI();
			value.AvailableApplyType = leftValue.AvailableApplyType;
			value.ApplyType = leftValue.ApplyType;
			value.PageWidth = Math.Min(leftValue.PageWidth, rightValue.PageWidth);
			if (leftValue.EqualColumnWidth == rightValue.EqualColumnWidth)
				value.EqualColumnWidth = leftValue.EqualColumnWidth;
			if (leftValue.ColumnCount == rightValue.ColumnCount)
				value.ColumnCount = leftValue.ColumnCount;
			if (value.ColumnCount != null) {
				if (leftValue.Columns.Count != value.ColumnCount.Value)
					return value;
				if (rightValue.Columns.Count != value.ColumnCount.Value)
					return value;
				for (int i = 0; i < value.ColumnCount.Value; i++) {
					if (leftValue.Columns[i].Width != rightValue.Columns[i].Width)
						return value;
					if (leftValue.Columns[i].Spacing != rightValue.Columns[i].Spacing)
						return value;
				}
				value.ChangeColumnCount(leftValue.ColumnCount.Value);
				for (int i = 0; i < value.ColumnCount.Value; i++) {
					value.Columns[i].Width = leftValue.Columns[i].Width;
					value.Columns[i].Spacing = leftValue.Columns[i].Spacing;
				}
			}
			return value;
		}
	}
	#endregion
}
