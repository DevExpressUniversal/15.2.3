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
using System.IO;
using System.Text;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Drawing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing;
using System.Diagnostics;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraRichEdit.Import.Doc {
	#region DocumentState
	public enum DocContentState {
		MainDocument,
		Footnotes,
		HeadersFootersStory,
		HeadersFootersEnd,
		Macro,
		Comments,
		Endnotes,
		TextBoxes,
		HeaderTextBoxes,
		Final
	}
	#endregion
	#region DocContentIterator
	public class DocContentIterator {
		#region Fields
		int currentCharacterPosition;
		int mainDocumentLength;
		int footNotesTextLength;
		int headersFootersTextLength;
		int commentsTextLength;
		int endNotesTextLength;
		int textBoxesTextLength;
		int headerTextBoxesTextLength;
		DocBookmarkIterator bookmarkIterator;
		DocCommentsIterator commentsIterator;
		DocNotesIterator notesIterator;
		DocFloatingObjectsIterator floatingObjectsIterator;
		DocRangeEditPermissionIterator permissionsIterator;
		DocIteratorState state;
		DocIteratorStateFactory factory;
		DocObjectCollection mainTextDocObjects;
		DocObjectCollection macroObjects;
		DocHeadersFooters documentHeadersFooters;
		#endregion
		public DocContentIterator(FileInformationBlock fib, BinaryReader mainStreamReader, BinaryReader tableStreamReader, DocumentModel model) {
			this.mainTextDocObjects = new DocObjectCollection();
			this.macroObjects = new DocObjectCollection();
			this.documentHeadersFooters = new DocHeadersFooters();
			SetTextStreamBorders(fib);
			InitializeIterators(fib, mainStreamReader, tableStreamReader, model);
			this.factory = new DocIteratorStateFactory(fib, mainStreamReader, tableStreamReader, this);
			this.state = this.factory.CreateState(DocContentState.MainDocument);
			SwitchToNextState();
		}
		#region Properties
		protected int CurrentCharacterPosition { get { return currentCharacterPosition; } set { currentCharacterPosition = value; } }
		protected int MainDocumentLength { get { return mainDocumentLength; } set { mainDocumentLength = value; } }
		protected int FootNotesTextLength { get { return footNotesTextLength; } set { footNotesTextLength = value; } }
		protected int HeadersFootersTextLength { get { return headersFootersTextLength; } set { headersFootersTextLength = value; } }
		protected int CommentsTextLength { get { return commentsTextLength; } set { commentsTextLength = value; } }
		protected int EndNotesTextLength { get { return endNotesTextLength; } set { endNotesTextLength = value; } }
		protected int TextBoxesTextLength { get { return textBoxesTextLength; } set { textBoxesTextLength = value; } }
		protected int HeaderTextBoxesTextLength { get { return headerTextBoxesTextLength; } set { headerTextBoxesTextLength = value; } }
		protected int HeadersDocumentOrigin { get { return mainDocumentLength + footNotesTextLength; } }
		protected int CommentsDocumentOrigin { get { return HeadersDocumentOrigin + HeadersFootersTextLength; } }
		protected int EndNotesDocumentOrigin { get { return CommentsDocumentOrigin + CommentsTextLength; } }
		protected int TextBoxesDocumentOrigin { get { return EndNotesDocumentOrigin + EndNotesTextLength; } }
		protected int HeaderTextBoxesDocumentOrigin { get { return TextBoxesDocumentOrigin + TextBoxesTextLength; } }
		protected internal DocIteratorState State { get { return state; } }
		protected DocBookmarkIterator BookmarkIterator { get { return bookmarkIterator; } }
		protected internal DocCommentsIterator CommentsIterator { get { return commentsIterator; } }
		protected internal DocNotesIterator NotesIterator { get { return notesIterator; } }
		protected internal DocFloatingObjectsIterator FloatingObjectsIterator { get { return floatingObjectsIterator; } }
		protected internal DocRangeEditPermissionIterator PermissionsIterator { get { return permissionsIterator; } }
		public DocObjectCollection MainTextDocObjects { get { return mainTextDocObjects; } }
		public DocObjectCollection MacroObjects { get { return macroObjects; } }
		public DocHeadersFooters HeadersFooters { get { return documentHeadersFooters; } }
		public DocObjectCollection Destination { get { return State.Destination; } }
		#endregion
		void SetTextStreamBorders(FileInformationBlock fib) {
			MainDocumentLength = fib.MainDocumentLength;
			FootNotesTextLength = fib.FootNotesLength;
			HeadersFootersTextLength = fib.HeadersFootersLength;
			CommentsTextLength = fib.CommentsLength;
			EndNotesTextLength = fib.EndNotesLength;
			TextBoxesTextLength = fib.MainDocumentTextBoxesLength;
			HeaderTextBoxesTextLength = fib.HeaderTextBoxesLength;
		}
		void InitializeIterators(FileInformationBlock fib, BinaryReader mainStreamReader, BinaryReader tableStreamReader, DocumentModel model) {
			this.bookmarkIterator = new DocBookmarkIterator(fib, tableStreamReader);
			this.commentsIterator = new DocCommentsIterator(fib, tableStreamReader);
			this.notesIterator = new DocNotesIterator(fib, tableStreamReader);
			OfficeArtContent artContent = null;
			if (fib.DrawingObjectTableSize != 0)
				artContent = OfficeArtContent.FromStream(tableStreamReader, mainStreamReader, fib.DrawingObjectTableOffset, fib.DrawingObjectTableSize);
			FloatingObjectFormatting formatting = model.Cache.FloatingObjectFormattingCache.DefaultItem.Clone();
			this.floatingObjectsIterator = new DocFloatingObjectsIterator(tableStreamReader, fib, artContent, formatting);
			this.permissionsIterator = new DocRangeEditPermissionIterator(fib, tableStreamReader);
			InitializePageColor(artContent, model);
		}
		protected internal virtual void InitializePageColor(OfficeArtContent artContent, DocumentModel model) {
			if (artContent == null)
				return;
			int count = artContent.Drawings.Count;
			for (int i = 0; i < count; i++) {
				OfficeArtShapeContainer backgroundShape = artContent.Drawings[i].DrawingObjectsContainer.BackgroundShape;
				if (backgroundShape == null || backgroundShape.ShapeRecord.ShapeIdentifier != OfficeArtConstants.DefaultMainDocumentShapeIdentifier)
					continue;
				Color pageColor = backgroundShape.ArtProperties.FillColor;
				if (!DXColor.IsEmpty(pageColor))
					model.DocumentProperties.PageBackColor = pageColor;
				break;
			}
		}
		public bool ShouldProcessTextRun() {
			return !State.ReadingComplete;
		}
		public int AdvanceNext(int offset) {
			int currentObjectStartPosition = CurrentCharacterPosition;
			CurrentCharacterPosition += offset;
			State.AdvanceNext(offset);
			return currentObjectStartPosition;
		}
		public void UpdateState() {
			State.UpdateDestination();
			SwitchToNextState();
		}
		public bool AdvanceField(DocPropertyContainer propertyContainer) {
			if (State.FieldIterator == null)
				return false;
			return State.FieldIterator.MoveNext(propertyContainer);
		}
		public void CheckFieldCompatibility(DocObjectCollection destination) {
			State.FieldIterator.CheckFieldCompatibility(destination);
		}
		public DocFloatingObjectBase GetFloatingObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			return FloatingObjectsIterator.GetFloatingObject(GetRelativeCharacterPosition(objectInfo), objectInfo, propertyContainer);
		}
		public IDocObject GetNoteObject(DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			return NotesIterator.GetNoteObject(objectInfo, propertyContainer);
		}
		public FieldType GetCurrentFieldType() {
			return State.FieldIterator.GetCurrentFieldType();
		}
		public void AdvancePosition(DocumentLogPosition logPosition, int originalPosition, int length) {
			AdvanceBookmarks(logPosition, originalPosition, length);
			AdvanceComments(logPosition, originalPosition, length);
			AdvanceRangePermissions(logPosition, originalPosition, length);
		}
		void AdvanceBookmarks(DocumentLogPosition logPosition, int originalPosition, int length) {
			BookmarkIterator.AdvanceNext(logPosition, originalPosition, length);
		}
		void AdvanceComments(DocumentLogPosition logPosition, int originalPosition, int length) {
			CommentsIterator.AdvanceNext(logPosition, originalPosition, length);
		}
		void AdvanceRangePermissions(DocumentLogPosition logPosition, int originalPosition, int length) {
			PermissionsIterator.AdvanceNext(logPosition, originalPosition, length);
		}
		public void InsertBookmarks(PieceTable pieceTable) {
			BookmarkIterator.InsertBookmarks(pieceTable);
			PermissionsIterator.InsertRangeEditPermissions(pieceTable);
		}
		protected void SwitchToNextState() {
			while (State.ShouldChangeState(CurrentCharacterPosition)) {
				ChangeState();
			}
		}
		protected void ChangeState() {
			this.state.FinishState();
			this.state = this.factory.CreateState(State.NextState);
			FloatingObjectsIterator.State = State.CurrentState;
		}
		protected int GetRelativeCharacterPosition(DocObjectInfo info) {
			int position = info.Position;
			switch (State.CurrentState) {
				case DocContentState.MainDocument: return position;
				case DocContentState.Footnotes: return position - MainDocumentLength;
				case DocContentState.HeadersFootersStory: return position - HeadersDocumentOrigin;
				case DocContentState.Comments: return position - CommentsDocumentOrigin;
				case DocContentState.Endnotes: return position - EndNotesDocumentOrigin;
				case DocContentState.TextBoxes: return position - TextBoxesDocumentOrigin;
				case DocContentState.HeaderTextBoxes: return position - HeaderTextBoxesDocumentOrigin;
				default: return position;
			}
		}
		public void BeginEmbeddedContent() {
			BookmarkIterator.BeginEmbeddedContent();			
		}
		public void EndEmbeddedContent() {
			BookmarkIterator.EndEmbeddedContent();
		}
	}
	#endregion
	#region DocIteratorStateFactory
	public class DocIteratorStateFactory {
		delegate DocIteratorState CreateStateDelegate();
		#region Fields
		Dictionary<DocContentState, CreateStateDelegate> creators;
		FileInformationBlock fib;
		BinaryReader mainStreamReader;
		BinaryReader tableStreamReader;
		DocContentIterator contentIterator;
		DocHeadersFootersPositions headersFootersPositions;
		#endregion
		public DocIteratorStateFactory(FileInformationBlock fib, BinaryReader mainStreamReader, BinaryReader tableStreamReader, DocContentIterator iterator) {
			this.fib = fib;
			this.mainStreamReader = mainStreamReader;
			this.tableStreamReader = tableStreamReader;
			this.contentIterator = iterator;
			this.headersFootersPositions = DocHeadersFootersPositions.FromStream(tableStreamReader, fib.HeadersFootersPositionsOffset, fib.HeadersFootersPositionsSize);
			InitializeStateCreators();
		}
		#region Properties
		protected FileInformationBlock FileInfo { get { return this.fib; } }
		protected BinaryReader MainStreamReader { get { return this.mainStreamReader; } }
		protected BinaryReader TableStreamReader { get { return this.tableStreamReader; } }
		protected DocContentIterator ContentIterator { get { return this.contentIterator; } }
		protected DocHeadersFootersPositions HeadersFootersPositions { get { return this.headersFootersPositions; } }
		#endregion
		void InitializeStateCreators() {
			creators = new Dictionary<DocContentState, CreateStateDelegate>();
			creators.Add(DocContentState.MainDocument, CreateMainDocumentState);
			creators.Add(DocContentState.Footnotes, CreateFootnotesState);
			creators.Add(DocContentState.HeadersFootersStory, CreateHeadersFootersStoryState);
			creators.Add(DocContentState.HeadersFootersEnd, CreateHeadersFootersEndState);
			creators.Add(DocContentState.Macro, CreateMacroState);
			creators.Add(DocContentState.Comments, CreateCommentsState);
			creators.Add(DocContentState.Endnotes, CreateEndnotesState);
			creators.Add(DocContentState.TextBoxes, CreateTextBoxesState);
			creators.Add(DocContentState.HeaderTextBoxes, CreateHeaderTextBoxesState);
			creators.Add(DocContentState.Final, CreateFinalState);
		}
		public DocIteratorState CreateState(DocContentState stateType) {
			CreateStateDelegate creator;
			if (!creators.TryGetValue(stateType, out creator))
				creator = CreateFinalState;
			return creator();
		}
		MainDocumentIteratorState CreateMainDocumentState() {
			MainDocumentIteratorState result = new MainDocumentIteratorState(FileInfo.MainDocumentLength, ContentIterator.MainTextDocObjects);
			result.FieldIterator = CreateFieldIterator(DocContentState.MainDocument);
			return result;
		}
		FootNotesIteratorState CreateFootnotesState() {
			int threshold = FileInfo.FootNotesStart + FileInfo.FootNotesLength;
			DocNotesIterator notesIterator = ContentIterator.NotesIterator;
			FootNotesIteratorState result = new FootNotesIteratorState(threshold, notesIterator.FootNotes, notesIterator.FootNoteReferences, TableStreamReader, FileInfo);
			result.HasHeaders = !HeadersFootersPositions.IsEmpty();
			result.FieldIterator = CreateFieldIterator(DocContentState.Footnotes);
			return result;
		}
		HeaderFooterIteratorStoryState CreateHeadersFootersStoryState() {
			int threshold = FileInfo.HeadersFootersStart + HeadersFootersPositions.GetNextStoryPosition();
			DocHeadersFooters headersFooters = ContentIterator.HeadersFooters;
			headersFooters.HeadersFooters.Add(new DocObjectCollection());
			HeaderFooterIteratorStoryState result = new HeaderFooterIteratorStoryState(threshold, headersFooters.ActiveCollection, HeadersFootersPositions);
			result.FieldIterator = CreateFieldIterator(DocContentState.HeadersFootersStory);
			return result;
		}
		DocIteratorState CreateHeadersFootersEndState() {
			int threshold = FileInfo.HeadersFootersStart + FileInfo.HeadersFootersLength;
			HeaderFooterEndIteratorState result = new HeaderFooterEndIteratorState(threshold, new DocObjectCollection());
			return result;
		}
		MacroIteratorState CreateMacroState() {
			int threshold = FileInfo.MacroStart + FileInfo.MacroLength;
			MacroIteratorState result = new MacroIteratorState(threshold, ContentIterator.MacroObjects);
			return result;
		}
		CommentsIteratorState CreateCommentsState() {
			int threshold = FileInfo.CommentsStart + FileInfo.CommentsLength;
			DocCommentsIterator commentsIterator = ContentIterator.CommentsIterator;
			CommentsIteratorState result = new CommentsIteratorState(threshold, commentsIterator.CommentsContent, commentsIterator.CommentsReferences, TableStreamReader, FileInfo);
			result.FieldIterator = CreateFieldIterator(DocContentState.Comments);
			return result;
		}
		EndnotesIteratorState CreateEndnotesState() {
			int threshold = FileInfo.EndnotesStart + FileInfo.EndNotesLength;
			DocNotesIterator notesIterator = ContentIterator.NotesIterator;
			EndnotesIteratorState result = new EndnotesIteratorState(threshold, notesIterator.EndNotes, notesIterator.EndNoteReferences, TableStreamReader, FileInfo);
			result.FieldIterator = CreateFieldIterator(DocContentState.Endnotes);
			return result;
		}
		TextBoxesIteratorState CreateTextBoxesState() {
			int threshold = FileInfo.MainDocumentTextBoxesStart + FileInfo.MainDocumentTextBoxesLength;
			Dictionary<int, DocObjectCollection> textBoxes = ContentIterator.FloatingObjectsIterator.MainDocumentTextBoxes;
			List<int> references = ContentIterator.FloatingObjectsIterator.MainDocumentTextBoxReferences;
			TextBoxesIteratorState result = new TextBoxesIteratorState(threshold, textBoxes, references, TableStreamReader, FileInfo);
			result.FieldIterator = CreateFieldIterator(DocContentState.TextBoxes);
			return result;
		}
		HeaderTextBoxesIteratorState CreateHeaderTextBoxesState() {
			int threshold = FileInfo.HeaderTextBoxesStart + FileInfo.HeaderTextBoxesLength;
			Dictionary<int, DocObjectCollection> textBoxes = ContentIterator.FloatingObjectsIterator.HeaderDocumentTextBoxes;
			List<int> references = ContentIterator.FloatingObjectsIterator.HeaderDocumentTextBoxReferences;
			HeaderTextBoxesIteratorState result = new HeaderTextBoxesIteratorState(threshold, textBoxes, references, TableStreamReader, FileInfo);
			result.FieldIterator = CreateFieldIterator(DocContentState.HeaderTextBoxes);
			return result;
		}
		FinalIteratorState CreateFinalState() {
			return new FinalIteratorState(Int32.MaxValue, null);
		}
		DocFieldsIterator CreateFieldIterator(DocContentState stateType) {
			switch (stateType) {
				case DocContentState.MainDocument:
					return new DocFieldsIterator(TableStreamReader, FileInfo.MainDocumentFieldTableOffset, FileInfo.MainDocumentFieldTableSize);
				case DocContentState.Footnotes:
					return new DocFieldsIterator(TableStreamReader, FileInfo.FootNotesFieldTableOffset, FileInfo.FootNotesFieldTableSize);
				case DocContentState.HeadersFootersStory:
					return new DocFieldsIterator(TableStreamReader, FileInfo.HeadersFootersFieldTableOffset, FileInfo.HeadersFootersFieldTableSize);
				case DocContentState.Comments:
					return new DocFieldsIterator(TableStreamReader, FileInfo.CommentsFieldTableOffset, FileInfo.CommentsFieldTableSize);
				case DocContentState.Endnotes:
					return new DocFieldsIterator(TableStreamReader, FileInfo.EndNotesFieldTableOffset, FileInfo.EndNotesFieldTableSize);
				case DocContentState.TextBoxes:
					return new DocFieldsIterator(TableStreamReader, FileInfo.MainDocumentTextBoxesFieldTableOffset, FileInfo.MainDocumentTextBoxesFieldTableSize);
				case DocContentState.HeaderTextBoxes:
					return new DocFieldsIterator(TableStreamReader, FileInfo.HeaderTextBoxesFieldTableOffset, FileInfo.HeaderTextBoxesFieldTableSize);
			}
			return null;
		}
	}
	#endregion
	#region DocIteratorState (abstract class)
	public abstract class DocIteratorState {
		#region Fields
		readonly int threshold;
		DocObjectCollection destination;
		DocFieldsIterator fieldIterator;
		#endregion
		protected DocIteratorState(int threshold, DocObjectCollection destination) {
			this.threshold = threshold;
			this.destination = destination;
		}
		protected DocIteratorState(int threshold) {
			this.threshold = threshold;
			this.destination = new DocObjectCollection();
		}
		#region Properties
		public abstract DocContentState CurrentState { get; }
		public abstract DocContentState NextState { get; }
		public virtual bool ReadingComplete { get { return false; } }
		public int Threshold { get { return this.threshold; } }
		public DocObjectCollection Destination {
			get { return this.destination; }
			protected internal set { this.destination = value; }
		}
		public DocFieldsIterator FieldIterator {
			get { return this.fieldIterator; }
			protected internal set { this.fieldIterator = value; }
		}
		#endregion
		public virtual bool ShouldChangeState(int characterPosition) {
			return characterPosition == Threshold;
		}
		public virtual void FinishState() {
			NormalizeFields();
		}
		protected internal virtual void AdvanceNext(int offset) {
		}
		protected internal virtual void UpdateDestination() {
		}
		protected void FixLastParagraph() {
			if (Destination.Count != 0)
				Destination.RemoveAt(Destination.Count - 1);
		}
		protected virtual void NormalizeFields() {
			if (ShouldNormalizeFields(Destination)) {
				FieldIterator.Reset();
				NormalizeFieldsCore(Destination);
			}
		}
		protected bool ShouldNormalizeFields(DocObjectCollection docObjects) {
			if (FieldIterator == null)
				return false;
			int fieldBalance = 0;
			int count = Destination.Count;
			for (int i = 0; i < count; i++) {
				if (docObjects[i].DocObjectType == DocObjectType.FieldBegin)
					fieldBalance++;
				if (docObjects[i].DocObjectType == DocObjectType.FieldEnd)
					fieldBalance--;
				if (fieldBalance < 0)
					return true;
			}
			return fieldBalance != 0;
		}
		protected virtual void NormalizeFieldsCore(DocObjectCollection docObjects) {
			int count = docObjects.Count;
			for (int i = 0; i < count; i++) {
				IDocObject docObject = docObjects[i];
				DocObjectType type = docObject.DocObjectType;
				if (type == DocObjectType.FieldBegin || type == DocObjectType.FieldSeparator || type == DocObjectType.FieldEnd)
					FieldIterator.MoveNext(null);
				if (type == DocObjectType.ExpectedFieldBegin || type == DocObjectType.ExpectedFieldSeparator || type == DocObjectType.ExpectedFieldEnd)
					if (FieldIterator.MoveNext(null))
						((ExpectedDocObject)docObject).SetActualType();
			}
		}
	}
	#endregion
	#region MainDocumentIteratorState
	public class MainDocumentIteratorState : DocIteratorState {
		public MainDocumentIteratorState(int threshold, DocObjectCollection destination)
			: base(threshold, destination) { }
		public override DocContentState CurrentState { get { return DocContentState.MainDocument; } }
		public override DocContentState NextState { get { return DocContentState.Footnotes; } }
	}
	#endregion
	#region HeaderFooterStoryIteratorState
	public class HeaderFooterIteratorStoryState : DocIteratorState {
		DocHeadersFootersPositions positions;
		public HeaderFooterIteratorStoryState(int threshold, DocObjectCollection destination, DocHeadersFootersPositions positions)
			: base(threshold, destination) {
			this.positions = positions;
		}
		public override DocContentState CurrentState { get { return DocContentState.HeadersFootersStory; } }
		public override DocContentState NextState {
			get {
				if (!this.positions.IsLastHeaderFooter())
					return DocContentState.HeadersFootersStory;
				else
					return DocContentState.HeadersFootersEnd;
			}
		}
		public override void FinishState() {
			base.FinishState();
			this.positions.AdvanceNext();
			FixLastParagraph();
		}
	}
	#endregion
	#region HeaderFooterEndIteratorState
	public class HeaderFooterEndIteratorState : DocIteratorState {
		public HeaderFooterEndIteratorState(int threshold, DocObjectCollection destination)
			: base(threshold, destination) { }
		public override DocContentState CurrentState { get { return DocContentState.HeadersFootersEnd; } }
		public override DocContentState NextState { get { return DocContentState.Macro; } }
	}
	#endregion
	#region CompositeObjectIteratorState (abstract class)
	public abstract class CompositeObjectIteratorState : DocIteratorState {
		#region Fields
		int currentCharacterPosition;
		int currentItemIndex;
		bool shouldNormalizeFields;
		readonly List<int> positions;
		readonly List<int> references;
		readonly Dictionary<int, DocObjectCollection> compositeObjects;
		#endregion
		protected CompositeObjectIteratorState(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> references, BinaryReader reader, FileInformationBlock fib)
			: base(threshold) {
			this.compositeObjects = compositeObjects;
			this.references = references;
			this.positions = GetPositions(reader, fib);
		}
		#region Properties
		protected int CurrentCharacterPosition { get { return currentCharacterPosition; } }
		protected internal int CurrentItemIndex { get { return currentItemIndex; } set { currentItemIndex = value; } }
		protected List<int> Positions { get { return positions; } }
		protected List<int> References { get { return references; } }
		public Dictionary<int, DocObjectCollection> CompositeObjects { get { return compositeObjects; } }
		#endregion
		protected abstract List<int> GetPositions(BinaryReader reader, FileInformationBlock fib);
		protected List<int> GetPositionsCore(BinaryReader reader, int offset, int size) {
			List<int> result = new List<int>();
			int count = size / DocConstants.CharacterPositionSize;
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			for (int i = 0; i < count; i++)
				result.Add(reader.ReadInt32());
			return result;
		}
		protected internal override void AdvanceNext(int offset) {
			this.currentCharacterPosition += offset;
		}
		protected internal override void UpdateDestination() {
			if (!ShouldChangeItem())
				return;
			this.shouldNormalizeFields |= ShouldNormalizeFields(Destination);
			UpdateDestinationCore();
		}
		protected internal virtual void UpdateDestinationCore() {
			CompositeObjects.Add(References[CurrentItemIndex], Destination);
			Destination = new DocObjectCollection();
			CurrentItemIndex++;
		}
		protected bool ShouldChangeItem() {
			int nextItemIndex = CurrentItemIndex + 1;
			if (nextItemIndex >= Positions.Count || CurrentItemIndex >= References.Count)
				return false;
			return CurrentCharacterPosition == Positions[nextItemIndex];
		}
		protected override void NormalizeFields() {
			if (shouldNormalizeFields) {
				FieldIterator.Reset();
				for (int i = 0; i < References.Count; i++) {
					DocObjectCollection destination;
					if (CompositeObjects.TryGetValue(References[i], out destination))
						NormalizeFieldsCore(destination);
				}
			}
		}
	}
	#endregion
	#region FootnotesIteratorState
	public class FootNotesIteratorState : CompositeObjectIteratorState {
		#region Fields
		bool hasHeaders;
		#endregion
		public FootNotesIteratorState(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> references, BinaryReader reader, FileInformationBlock fib)
			: base(threshold, compositeObjects, references, reader, fib) { }
		protected internal bool HasHeaders { get { return hasHeaders; } set { hasHeaders = value; } }
		public override DocContentState CurrentState { get { return DocContentState.Footnotes; } }
		public override DocContentState NextState { get { return hasHeaders ? DocContentState.HeadersFootersStory : DocContentState.Macro; } }
		protected override List<int> GetPositions(BinaryReader reader, FileInformationBlock fib) {
			return GetPositionsCore(reader, fib.FootNotesTextOffset, fib.FootNotesTextSize);
		}
		public override void FinishState() {
			base.FinishState();
			FixLastParagraph();
		}
	}
	#endregion
	#region MacroIteratorState
	public class MacroIteratorState : DocIteratorState {
		public MacroIteratorState(int threshold, DocObjectCollection destination)
			: base(threshold, destination) { }
		public override DocContentState CurrentState { get { return DocContentState.Macro; } }
		public override DocContentState NextState { get { return DocContentState.Comments; } }
	}
	#endregion
	#region CommentsIteratorState
	public class CommentsIteratorState : CompositeObjectIteratorState {
		public CommentsIteratorState(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> references, BinaryReader reader, FileInformationBlock fib)
			: base(threshold, compositeObjects, references, reader, fib) {
		}
		public override DocContentState CurrentState { get { return DocContentState.Comments; } }
		public override DocContentState NextState { get { return DocContentState.Endnotes; } }
		public override void FinishState() {
			base.FinishState();
			FixLastParagraph();
		}
		protected override List<int> GetPositions(BinaryReader reader, FileInformationBlock fib) {
			return GetPositionsCore(reader, fib.CommentsTextOffset, fib.CommentsTextSize);
		}
	}
	#endregion
	#region EndnotesIteratorState
	public class EndnotesIteratorState : CompositeObjectIteratorState {
		public EndnotesIteratorState(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> references, BinaryReader reader, FileInformationBlock fib)
			: base(threshold, compositeObjects, references, reader, fib) {
		}
		public override DocContentState CurrentState { get { return DocContentState.Endnotes; } }
		public override DocContentState NextState { get { return DocContentState.TextBoxes; } }
		public override void FinishState() {
			base.FinishState();
			FixLastParagraph();
		}
		protected override List<int> GetPositions(BinaryReader reader, FileInformationBlock fib) {
			return GetPositionsCore(reader, fib.EndnotesTextOffset, fib.EndnotesTextSize);
		}
	}
	#endregion
	#region TextBoxesIteratorStateBase (abstract class)
	public abstract class TextBoxesIteratorStateBase : CompositeObjectIteratorState {
		protected TextBoxesIteratorStateBase(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> references, BinaryReader reader, FileInformationBlock fib)
			: base(threshold, compositeObjects, references, reader, fib) {
		}
		protected internal override void UpdateDestinationCore() {
			int currentReference = References[CurrentItemIndex];
			if (Destination.Count > 0)
				Destination.RemoveAt(Destination.Count - 1);
			if (currentReference > 0)
				CompositeObjects.Add(currentReference, Destination);
			Destination = new DocObjectCollection();
			CurrentItemIndex++;
		}
	}
	#endregion
	#region TextBoxesIteratorState
	public class TextBoxesIteratorState : TextBoxesIteratorStateBase {
		public TextBoxesIteratorState(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> referernces, BinaryReader reader, FileInformationBlock fib)
			: base(threshold, compositeObjects, referernces, reader, fib) { }
		public override DocContentState CurrentState { get { return DocContentState.TextBoxes; } }
		public override DocContentState NextState { get { return DocContentState.HeaderTextBoxes; } }
		protected override List<int> GetPositions(BinaryReader reader, FileInformationBlock fib) {
			List<int> result = new List<int>();
			reader.BaseStream.Seek(fib.MainDocumentTextBoxesTextOffset, SeekOrigin.Begin);
			int size = fib.MainDocumentTextBoxesTextSize;
			if (size == 0)
				return result;
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + DocFloatingObjectsIterator.TextBoxInfoSize) + 1;
			for (int i = 0; i < count; i++) {
				result.Add(reader.ReadInt32());
			}
			return result;
		}
	}
	#endregion
	#region HeaderTextBoxesIteratorState
	public class HeaderTextBoxesIteratorState : TextBoxesIteratorStateBase {
		public HeaderTextBoxesIteratorState(int threshold, Dictionary<int, DocObjectCollection> compositeObjects, List<int> references, BinaryReader reader, FileInformationBlock fib)
			: base(threshold, compositeObjects, references, reader, fib) { }
		public override DocContentState CurrentState { get { return DocContentState.HeaderTextBoxes; } }
		public override DocContentState NextState { get { return DocContentState.Final; } }
		protected override List<int> GetPositions(BinaryReader reader, FileInformationBlock fib) {
			List<int> result = new List<int>();
			reader.BaseStream.Seek(fib.HeaderTextBoxesTextOffset, SeekOrigin.Begin);
			int size = fib.HeaderTextBoxesTextSize;
			if (size == 0)
				return result;
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + DocFloatingObjectsIterator.TextBoxInfoSize) + 1;
			for (int i = 0; i < count; i++) {
				result.Add(reader.ReadInt32());
			}
			return result;
		}
	}
	#endregion
	#region FinalIteratorState
	public class FinalIteratorState : DocIteratorState {
		public FinalIteratorState(int threshold, DocObjectCollection destination)
			: base(threshold, destination) { }
		public override DocContentState CurrentState { get { return DocContentState.Final; } }
		public override DocContentState NextState { get { return DocContentState.Final; } }
		public override bool ReadingComplete { get { return true; } }
		public override bool ShouldChangeState(int characterPosition) { return false; }
	}
	#endregion
	#region DocFloatingObjectsIterator
	public class DocFloatingObjectsIterator {
		#region Fields
		const int unusedTextBoxInfoStart = 14;
		const int unusedTextBoxInfoEnd = 4;
		protected internal const int TextBoxInfoSize = 22;
		DocContentState state;
		FloatingObjectFormatting formatting;
		OfficeArtContent artContent;
		FileShapeAddressTable mainShapeAddresses;
		FileShapeAddressTable headerShapeAddresses;
		List<int> headerDocumentTextBoxReferences;
		List<int> mainDocumentTextBoxReferences;
		Dictionary<int, DocObjectCollection> headerDocumentTextBoxes;
		Dictionary<int, DocObjectCollection> mainDocumentTextBoxes;
		BreakDescriptorTable mainDocumentBreakDescriptors;
		BreakDescriptorTable headerDocumentBreakDescriptors;
		#endregion
		public DocFloatingObjectsIterator(BinaryReader reader, FileInformationBlock fib, OfficeArtContent artContent, FloatingObjectFormatting formatting) {
			this.artContent = artContent;
			this.formatting = formatting;			
			this.headerDocumentTextBoxes = new Dictionary<int, DocObjectCollection>();
			this.mainDocumentTextBoxes = new Dictionary<int, DocObjectCollection>();
			ReadFileShapeAddresses(reader, fib);
			ReadTextBoxReferences(reader, fib);
			ReadTextBoxBreakDescriptors(reader, fib);
		}
		#region Properties
		protected internal FloatingObjectFormatting Formatting { get { return formatting; } }
		protected internal DocContentState State { get { return state; } set { state = value; } }
		protected internal FileShapeAddressTable ShapeAddresses {
			get { return State == DocContentState.MainDocument ? mainShapeAddresses : headerShapeAddresses; }
		}
		protected internal BlipsWithProperties BlipsWithProperties {
			get {
				if (artContent == null)
					return null;
				return State == DocContentState.MainDocument ? artContent.MainDocumentBlips : artContent.HeadersBlips;
			}
		}
		protected List<int> TextBoxReferences {
			get { return State == DocContentState.MainDocument ? mainDocumentTextBoxReferences : headerDocumentTextBoxReferences; }
		}
		protected internal List<int> HeaderDocumentTextBoxReferences { get { return headerDocumentTextBoxReferences; } }
		protected internal List<int> MainDocumentTextBoxReferences { get { return mainDocumentTextBoxReferences; } }
		protected internal Dictionary<int, DocObjectCollection> HeaderDocumentTextBoxes { get { return headerDocumentTextBoxes; } }
		protected internal Dictionary<int, DocObjectCollection> MainDocumentTextBoxes { get { return mainDocumentTextBoxes; } }
		protected internal BreakDescriptorTable MainTextBoxesBreakTable { get { return mainDocumentBreakDescriptors; } }
		protected internal BreakDescriptorTable HeaderDocumentTextBoxesBreakTable { get { return headerDocumentBreakDescriptors; } }
		#endregion
		void ReadFileShapeAddresses(BinaryReader reader, FileInformationBlock fib) {
			this.headerShapeAddresses = FileShapeAddressTable.FromStream(reader, fib.HeadersFootersFileShapeTableOffset, fib.HeadersFootersFileShapeTableSize);
			this.mainShapeAddresses = FileShapeAddressTable.FromStream(reader, fib.MainDocumentFileShapeTableOffset, fib.MainDocumentFileShapeTableSize);
		}
		void ReadTextBoxReferences(BinaryReader reader, FileInformationBlock fib) {
			this.mainDocumentTextBoxReferences = ReadTextBoxReferencesCore(reader, fib.MainDocumentTextBoxesTextOffset, fib.MainDocumentTextBoxesTextSize);
			this.headerDocumentTextBoxReferences = ReadTextBoxReferencesCore(reader, fib.HeaderTextBoxesTextOffset, fib.HeaderTextBoxesTextSize);
		}
		void ReadTextBoxBreakDescriptors(BinaryReader reader, FileInformationBlock fib) {
			this.mainDocumentBreakDescriptors = BreakDescriptorTable.FromStream(reader, fib.MainTextBoxBreakTableOffset, fib.MainTextBoxBreakTableSize);
			this.headerDocumentBreakDescriptors = BreakDescriptorTable.FromStream(reader, fib.HeadersFootersFieldTableOffset, fib.HeadersFootersFieldTableSize);
		}
		List<int> ReadTextBoxReferencesCore(BinaryReader reader, int offset, int size) {
			List<int> result = new List<int>();
			if (size == 0)
				return result;
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + TextBoxInfoSize);
			reader.BaseStream.Seek(offset + (count + 1) * DocConstants.CharacterPositionSize, SeekOrigin.Begin);
			for (int i = 0; i < count; i++) {
				reader.BaseStream.Seek(unusedTextBoxInfoStart, SeekOrigin.Current);
				result.Add(reader.ReadInt32());
				reader.BaseStream.Seek(unusedTextBoxInfoEnd, SeekOrigin.Current);
			}
			return result;
		}
		public DocFloatingObjectBase GetFloatingObject(int characterPosition, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			FileShapeAddress address;
			if (!ShapeAddresses.TranslationTable.TryGetValue(characterPosition, out address))
				return null;
			return GetFloatingObjectCore(address, objectInfo, propertyContainer);
		}
		DocFloatingObjectBase GetFloatingObjectCore(FileShapeAddress address, DocObjectInfo objectInfo, DocPropertyContainer propertyContainer) {
			DocFloatingObjectBase result = null;
			int shapeIdentifier = address.ShapeIdentifier;
			BlipBase blip;
			if (BlipsWithProperties != null && BlipsWithProperties.Blips.TryGetValue(shapeIdentifier, out blip))
				result = new DocPictureFloatingObject(objectInfo, propertyContainer, blip);
			if (TextBoxReferences.Contains(shapeIdentifier))
				result = new DocTextBoxFloatingObject(objectInfo, propertyContainer, shapeIdentifier);
			if (result == null)
				return null;
			result.Formatting = Formatting.Clone();
			result.ApplyFileShapeAddress(address);
			OfficeArtProperties properties;
			if (BlipsWithProperties.ShapeArtProperties.TryGetValue(shapeIdentifier, out properties))
				result.SetOfficeArtProperties(properties);
			OfficeArtTertiaryProperties tertiaryProperties;
			if (BlipsWithProperties.ShapeArtTertiaryProperties.TryGetValue(shapeIdentifier, out tertiaryProperties))
				result.SetOfficeArtTertiaryProperties(tertiaryProperties);			
			return result;
		}
		public DocObjectCollection GetMainTextBoxObjects(int shapeId) {
			DocObjectCollection result;
			if (!MainDocumentTextBoxes.TryGetValue(shapeId, out result))
				result = new DocObjectCollection();
			return result;
		}
		public DocObjectCollection GetHeaderTextBoxObjects(int shapeId) {
			DocObjectCollection result;
			if (!HeaderDocumentTextBoxes.TryGetValue(shapeId, out result))
				result = new DocObjectCollection();
			return result;
		}
	}
	#endregion
	#region DocFieldsIterator
	public class DocFieldsIterator {
		#region static
		static List<string> unsupportedFieldCodes;
		static DocFieldsIterator() {
			unsupportedFieldCodes = new List<string>();
			unsupportedFieldCodes.Add(@" SHAPE  \* MERGEFORMAT ");
			unsupportedFieldCodes.Add(@" FORMCHECKBOX ");
		}
		#endregion
		#region Fields
		int currentFieldIndex;
		Stack<DocFieldBeginDescriptor> fieldBeginDescriptors;
		DocFieldTable fieldTable;
		#endregion
		public DocFieldsIterator(BinaryReader reader, int offset, int size) {
			this.fieldTable = DocFieldTable.FromStream(reader, offset, size);
			this.fieldBeginDescriptors = new Stack<DocFieldBeginDescriptor>();
		}
		public bool MoveNext(DocPropertyContainer propertyContainer) {
			IDocFieldDescriptor fieldDescriptor = this.fieldTable.GetFieldDescriptorByIndex(this.currentFieldIndex);
			this.currentFieldIndex++;
			DocFieldBeginDescriptor fieldBeginDescriptor = fieldDescriptor as DocFieldBeginDescriptor;
			if (fieldBeginDescriptor != null)
				this.fieldBeginDescriptors.Push(fieldBeginDescriptor);
			DocFieldEndDescriptor fieldEndDescriptor = fieldDescriptor as DocFieldEndDescriptor;
			if (fieldEndDescriptor != null)
				if (this.fieldBeginDescriptors.Count == 0)
					return false;
				else {
					if(propertyContainer != null)
						propertyContainer.FieldProperties = fieldEndDescriptor.Properties;
					this.fieldBeginDescriptors.Pop();
				}
			return true;
		}
		public FieldType GetCurrentFieldType() {
			if (this.fieldBeginDescriptors.Count == 0)
				return FieldType.None;
			DocFieldBeginDescriptor currentFieldBeginDescriptor = this.fieldBeginDescriptors.Peek();
			return currentFieldBeginDescriptor.CalcFieldType();
		}
		public void CheckFieldCompatibility(DocObjectCollection destination) {
			Stack<IDocObject> fieldObjects = GetFieldObjects(destination);
			int fieldLength = fieldObjects.Count;
			string fieldCode = GetFieldCode(fieldObjects);
			if (unsupportedFieldCodes.Contains(fieldCode)) {
				int removeFrom = destination.Count - fieldLength;
				int fieldStartPosition = destination[destination.Count - fieldLength].Position;
				IDocObject lastDocObject = destination[destination.Count - 1];
				int fieldEndPosition = lastDocObject.Position + lastDocObject.Length;
				destination.RemoveRange(removeFrom, fieldLength);
				int count = fieldObjects.Count;
				for (int i = 0; i < count - 1; i++) {
					IDocObject docObject = fieldObjects.Pop();
					destination.Add(docObject);
					if (docObject.DocObjectType == DocObjectType.UnsupportedObject) {
						UnsupportedObject unsupportedObject = docObject as UnsupportedObject;
						if (unsupportedObject != null) {
							unsupportedObject.Position = fieldStartPosition;
							unsupportedObject.Length = fieldEndPosition - fieldStartPosition;
						}
					}
				}
#if DEBUGTEST || DEBUG
				Debug.Assert(fieldObjects.Count == 0 || fieldObjects.Count == 1);
				if (fieldObjects.Count == 1)
					Debug.Assert(fieldObjects.Pop().GetType() == typeof(DocFieldEnd));
#endif
			}
		}
		public void Reset() {
			this.currentFieldIndex = 0;
			this.fieldBeginDescriptors.Clear();
		}
		Stack<IDocObject> GetFieldObjects(DocObjectCollection docObjects) {
			Debug.Assert(docObjects.Count > 0);
			Stack<IDocObject> fieldObjects = new Stack<IDocObject>();
			int count = docObjects.Count - 1;
			for (int i = count; i >= 0; i--) {
				fieldObjects.Push(docObjects[i]);
				if (docObjects[i].GetType() == typeof(DocFieldBegin))
					break;
			}
			return fieldObjects;
		}
		string GetFieldCode(Stack<IDocObject> fieldObjects) {
			Debug.Assert(fieldObjects.Count > 0);
			Debug.Assert(fieldObjects.Pop().GetType() == typeof(DocFieldBegin));
			StringBuilder fieldCode = new StringBuilder();
			int count = fieldObjects.Count;
			for (int i = 0; i < count; i++) {
				IDocObject docObject = fieldObjects.Pop();
				DocTextRun textRun = docObject as DocTextRun;
				if (textRun != null)
					fieldCode.Append(textRun.Text);
				if (docObject.GetType() == typeof(DocFieldSeparator))
					break;
			}
			return fieldCode.ToString();
		}
	}
	#endregion
	#region DocNotesIterator
	public class DocNotesIterator {
		#region Fields
		protected const int FlagSize = 2;
		Dictionary<int, DocObjectCollection> footNotes;
		Dictionary<int, DocObjectCollection> endNotes;
		List<int> footNotesReferences;
		List<int> endNoteReferences;
		#endregion
		public DocNotesIterator(FileInformationBlock fib, BinaryReader reader) {
			this.footNotes = new Dictionary<int, DocObjectCollection>();
			this.endNotes = new Dictionary<int, DocObjectCollection>();
			GetReferences(reader, fib);
		}
		#region Properties
		public Dictionary<int, DocObjectCollection> FootNotes { get { return footNotes; } }
		public Dictionary<int, DocObjectCollection> EndNotes { get { return endNotes; } }
		protected internal List<int> FootNoteReferences { get { return footNotesReferences; } }
		protected internal List<int> EndNoteReferences { get { return endNoteReferences; } }
		#endregion
		void GetReferences(BinaryReader reader, FileInformationBlock fib) {
			this.footNotesReferences = GetNoteReferencesCore(reader, fib.FootNotesReferenceOffset, fib.FootNotesReferenceSize);
			this.endNoteReferences = GetNoteReferencesCore(reader, fib.EndNotesReferenceOffset, fib.EndNotesReferenceSize);
		}
		List<int> GetNoteReferencesCore(BinaryReader reader, int offset, int size) {
			List<int> result = new List<int>();
			int count = (size - DocConstants.CharacterPositionSize) / (DocConstants.CharacterPositionSize + FlagSize); 
			reader.BaseStream.Seek(offset, SeekOrigin.Begin);
			for (int i = 0; i < count; i++) {
				result.Add(reader.ReadInt32());
			}
			return result;
		}
		public IDocObject GetNoteObject(DocObjectInfo info, DocPropertyContainer propertyContainer) {
			int characterPosition = info.Position;
			if (FootNoteReferences.Contains(characterPosition))
				return DocObjectFactory.Instance.CreateDocObject(DocObjectType.AutoNumberedFootnoteReference, info, propertyContainer);
			if (EndNoteReferences.Contains(characterPosition))
				return DocObjectFactory.Instance.CreateDocObject(DocObjectType.EndnoteReference, info, propertyContainer);
			return DocObjectFactory.Instance.CreateDocObject(DocObjectType.NoteNumber, info, propertyContainer);
		}
		public DocObjectCollection GetFootNoteObjects(int characterPosition) {
			DocObjectCollection result;
			if (!FootNotes.TryGetValue(characterPosition, out result))
				result = new DocObjectCollection();
			return result;
		}
		public DocObjectCollection GetEndNoteObjects(int characterPosition) {
			DocObjectCollection result;
			if (!EndNotes.TryGetValue(characterPosition, out result))
				result = new DocObjectCollection();
			return result;
		}
	}
	#endregion
}
