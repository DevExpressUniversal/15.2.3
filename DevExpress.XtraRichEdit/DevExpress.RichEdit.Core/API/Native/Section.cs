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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Compatibility.System.Drawing.Printing;
using Debug = System.Diagnostics.Debug;
#if !SL
using System.Drawing.Printing;
#else
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.XtraRichEdit.API.Native {
	[ComVisible(true)]
	public enum HeaderFooterType {
		First = 0,
		Odd = 1,
		Even = 2,
		Primary = Odd
	}
	#region SectionStartType
	[ComVisible(true)]
	public enum SectionStartType {
		NextPage = DevExpress.XtraRichEdit.Model.SectionStartType.NextPage,
		OddPage = DevExpress.XtraRichEdit.Model.SectionStartType.OddPage,
		EvenPage = DevExpress.XtraRichEdit.Model.SectionStartType.EvenPage,
	}
	#endregion
	#region Section
	[ComVisible(true)]
	public interface Section {
		ReadOnlyParagraphCollection Paragraphs { get; }
		SectionMargins Margins { get; }
		SectionPage Page { get; }
		SectionColumns Columns { get; }
		SectionLineNumbering LineNumbering { get; }
		SectionPageNumbering PageNumbering { get; }
		bool DifferentFirstPage { get; set; }
		SectionStartType StartType { get; set; }
		int FirstPageTray { get; set; }
		int OtherPagesTray { get; set; }
		SubDocument BeginUpdateHeader();
		SubDocument BeginUpdateHeader(HeaderFooterType type);
		void EndUpdateHeader(SubDocument document);
		bool HasHeader(HeaderFooterType type);
		SubDocument BeginUpdateFooter();
		SubDocument BeginUpdateFooter(HeaderFooterType type);
		void EndUpdateFooter(SubDocument document);
		bool HasFooter(HeaderFooterType type);
		bool IsHeaderLinkedToPrevious();
		void LinkHeaderToPrevious();
		void UnlinkHeaderFromPrevious();
		bool IsFooterLinkedToPrevious();
		void LinkFooterToPrevious();
		void UnlinkFooterFromPrevious();
		bool IsHeaderLinkedToPrevious(HeaderFooterType type);
		void LinkHeaderToPrevious(HeaderFooterType type);
		void UnlinkHeaderFromPrevious(HeaderFooterType type);
		bool IsFooterLinkedToPrevious(HeaderFooterType type);
		void LinkFooterToPrevious(HeaderFooterType type);
		void UnlinkFooterFromPrevious(HeaderFooterType type);
		bool IsHeaderLinkedToNext();
		void LinkHeaderToNext();
		void UnlinkHeaderFromNext();
		bool IsFooterLinkedToNext();
		void LinkFooterToNext();
		void UnlinkFooterFromNext();
		bool IsHeaderLinkedToNext(HeaderFooterType type);
		void LinkHeaderToNext(HeaderFooterType type);
		void UnlinkHeaderFromNext(HeaderFooterType type);
		bool IsFooterLinkedToNext(HeaderFooterType type);
		void LinkFooterToNext(HeaderFooterType type);
		void UnlinkFooterFromNext(HeaderFooterType type);
	}
	#endregion
	#region SectionMargins
	[ComVisible(true)]
	public interface SectionMargins {
		float Left { get; set; }
		float Top { get; set; }
		float Right { get; set; }
		float Bottom { get; set; }
		float HeaderOffset { get; set; }
		float FooterOffset { get; set; }
	}
	#endregion
	#region SectionPage
	[ComVisible(true)]
	public interface SectionPage {
		float Width { get; set; }
		float Height { get; set; }
		PaperKind PaperKind { get; set; }
		bool Landscape { get; set; }
	}
	#endregion
	#region LineNumberingRestart
	[ComVisible(true)]
	public enum LineNumberingRestart {
		NewPage,
		NewSection,
		Continuous,
	}
	#endregion
	#region SectionLineNumbering
	[ComVisible(true)]
	public interface SectionLineNumbering {
		int Start { get; set; }
		int CountBy { get; set; }
		LineNumberingRestart RestartType { get; set; }
		float Distance { get; set; }
	}
	#endregion
	#region SectionPageNumbering
	[ComVisible(true)]
	public interface SectionPageNumbering {
		int Start { get; set; }
		NumberingFormat NumberingFormat { get; set; }
	}
	#endregion
	#region SectionCollection
	[ComVisible(true)]
	public interface SectionCollection : ISimpleCollection<Section> {
	}
	#endregion
	[ComVisible(true)]
	public class SectionColumn {
		float width;
		float spacing;
		public SectionColumn() {
		}
		public SectionColumn(float width, float spacing) {
			this.width = width;
			this.spacing = spacing;
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SectionColumnWidth")]
#endif
		public float Width { get { return width; } set { width = value; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SectionColumnSpacing")]
#endif
		public float Spacing { get { return spacing; } set { spacing = value; } }
	}
	[ComVisible(true)]
	public class SectionColumnCollection : List<SectionColumn> {
	}
	[ComVisible(true)]
	public interface SectionColumns {
		int Count { get; }
		SectionColumnCollection GetColumns();
		void SetColumns(SectionColumnCollection columns);
		SectionColumnCollection CreateUniformColumns(float columnWidth, float columnSpacing, int columnCount);
		SectionColumnCollection CreateUniformColumns(SectionPage page, float columnSpacing, int columnCount);
	}
}
namespace DevExpress.XtraRichEdit.API.Native.Implementation {
	using ModelSection = DevExpress.XtraRichEdit.Model.Section;
	using ModelSectionMargins = DevExpress.XtraRichEdit.Model.SectionMargins;
	using ModelSectionPage = DevExpress.XtraRichEdit.Model.SectionPage;
	using ModelSectionColumns = DevExpress.XtraRichEdit.Model.SectionColumns;
	using ModelSectionLineNumbering = DevExpress.XtraRichEdit.Model.SectionLineNumbering;
	using ModelSectionPageNumbering = DevExpress.XtraRichEdit.Model.SectionPageNumbering;
	using ModelHeaderFooterType = DevExpress.XtraRichEdit.Model.HeaderFooterType;
	using ModelSectionHeader = DevExpress.XtraRichEdit.Model.SectionHeader;
	using ModelSectionFooter = DevExpress.XtraRichEdit.Model.SectionFooter;
	using System.Drawing;
	using DevExpress.Office.Utils;
	using DevExpress.XtraRichEdit.Internal;
	using Compatibility.System.Drawing.Printing;
	using Compatibility.System.Drawing;
	using System.Diagnostics;
	using Debug = System.Diagnostics.Debug;
	#region NativeSection
	public class NativeSection : Section {
		#region Fields
		readonly NativeDocument document;
		readonly ModelSection innerSection;
		readonly NativeSectionMargins margins;
		readonly NativeSectionPage page;
		readonly NativeSectionColumns columns;
		readonly NativeSectionLineNumbering lineNumbering;
		readonly NativeSectionPageNumbering pageNumbering;
		readonly SectionParagraphCollection paragraphs;
		bool isValid;
		#endregion
		internal NativeSection(NativeDocument document, ModelSection innerSection) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(innerSection, "innerSection");
			this.document = document;
			this.innerSection = innerSection;
			this.paragraphs = new SectionParagraphCollection(Document.Paragraphs, InnerSection);
			this.margins = new NativeSectionMargins(Document, innerSection.Margins);
			this.page = new NativeSectionPage(Document, innerSection.Page);
			this.columns = new NativeSectionColumns(Document, innerSection.Columns, page);
			this.lineNumbering = new NativeSectionLineNumbering(Document, innerSection.LineNumbering);
			this.pageNumbering = new NativeSectionPageNumbering(Document, innerSection.PageNumbering);
			this.isValid = true;
		}
		#region Properties
		public ModelSection InnerSection { get { return innerSection; } }
		public NativeDocument Document { get { return document; } }
		public SectionParagraphCollection Paragraphs {
			get {
				CheckValid();
				return paragraphs;
			}
		}
		public int FirstPageTray {
			get {
				CheckValid();
				return innerSection.GeneralSettings.FirstPagePaperSource;
			}
			set {
				CheckValid();
				innerSection.GeneralSettings.FirstPagePaperSource = value;
			}
		}
		public int OtherPagesTray {
			get {
				CheckValid();
				return innerSection.GeneralSettings.OtherPagePaperSource;
			}
			set {
				CheckValid();
				innerSection.GeneralSettings.OtherPagePaperSource = value;
			}
		}
		public bool DifferentFirstPage {
			get {
				CheckValid();
				return innerSection.GeneralSettings.DifferentFirstPage;
			}
			set {
				CheckValid();
				innerSection.GeneralSettings.DifferentFirstPage = value;
			}
		}
		public SectionStartType StartType {
			get {
				CheckValid();
				return (SectionStartType)innerSection.GeneralSettings.StartType;
			}
			set {
				CheckValid();
				innerSection.GeneralSettings.StartType = (DevExpress.XtraRichEdit.Model.SectionStartType)value;
			}
		}
		public NativeSectionMargins Margins {
			get {
				CheckValid();
				return margins;
			}
		}
		public NativeSectionPage Page {
			get {
				CheckValid();
				return page;
			}
		}
		public NativeSectionColumns Columns {
			get {
				CheckValid();
				return columns;
			}
		}
		public NativeSectionLineNumbering LineNumbering {
			get {
				CheckValid();
				return lineNumbering;
			}
		}
		public NativeSectionPageNumbering PageNumbering {
			get {
				CheckValid();
				return pageNumbering;
			}
		}
		public bool IsValid { get { return isValid; } set { isValid = value; } }
		#endregion
		internal void CheckValid() {
			if (!isValid)
				RichEditExceptions.ThrowInvalidOperationException(XtraRichEditStringId.Msg_UseDeletedSectionError);
#if DEBUG || DEBUGTEST
			Debug.Assert(document.DocumentModel.Sections.IndexOf(innerSection) >= new SectionIndex(0));
#endif
		}
		#region Section Members
		ReadOnlyParagraphCollection Section.Paragraphs { get { return this.Paragraphs; } }
		SectionMargins Section.Margins { get { return this.Margins; } }
		SectionPage Section.Page { get { return this.Page; } }
		SectionColumns Section.Columns { get { return this.Columns; } }
		SectionLineNumbering Section.LineNumbering { get { return this.LineNumbering; } }
		SectionPageNumbering Section.PageNumbering { get { return this.PageNumbering; } }
		public SubDocument BeginUpdateHeader() {
			return BeginUpdateHeader(HeaderFooterType.Odd);
		}
		public SubDocument BeginUpdateHeader(HeaderFooterType type) {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
			SectionHeader header = innerSection.Headers.GetObject(modelType);
			if (header == null) {
				innerSection.Headers.Create(modelType);
				header = innerSection.Headers.GetObject(modelType);
			}
			return CreateSubDocument(header.PieceTable, document.DocumentServer);
		}
		public void EndUpdateHeader(SubDocument document) {
			CheckValid();
			NativeSubDocument nativeDocument = (NativeSubDocument)document;
			nativeDocument.ReferenceCount--;
			innerSection.DocumentModel.EndUpdate();
		}
		public bool HasHeader(HeaderFooterType type) {
			ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
			return innerSection.Headers.GetObject(modelType) != null;
		}
		public SubDocument BeginUpdateFooter() {
			return BeginUpdateFooter(HeaderFooterType.Odd);
		}
		public SubDocument BeginUpdateFooter(HeaderFooterType type) {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
			SectionFooter footer = innerSection.Footers.GetObject(modelType);
			if (footer == null) {
				innerSection.Footers.Create(modelType);
				footer = innerSection.Footers.GetObject(modelType);
			}
			return CreateSubDocument(footer.PieceTable, document.DocumentServer);
		}
		public void EndUpdateFooter(SubDocument document) {
			CheckValid();
			NativeSubDocument nativeDocument = (NativeSubDocument)document;
			nativeDocument.ReferenceCount--;
			innerSection.DocumentModel.EndUpdate();
		}
		protected virtual SubDocument CreateSubDocument(PieceTable pieceTable, InnerRichEditDocumentServer server) {
			return new NativeSubDocument(pieceTable, server);
		}
		public bool HasFooter(HeaderFooterType type) {
			ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
			return innerSection.Footers.GetObject(modelType) != null;
		}
		bool IsLinkedToPrevious<TObject, TIndex>(HeaderFooterType type, SectionHeadersFooters<TObject, TIndex> headersFooters)
			where TObject : SectionHeaderFooterBase
			where TIndex : struct, IConvertToInt<TIndex> {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			try {
				ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
				TObject headerFooter = headersFooters.GetObject(modelType);
				if (headerFooter != null)
					return headerFooter.GetContainer(this.InnerSection).IsLinkedToPrevious(headerFooter.Type);
				else
					return false;
			}
			finally {
				innerSection.DocumentModel.EndUpdate();
			}
		}
		void LinkToPrevious<TObject, TIndex>(HeaderFooterType type, SectionHeadersFooters<TObject, TIndex> headersFooters)
			where TObject : SectionHeaderFooterBase
			where TIndex : struct, IConvertToInt<TIndex> {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			try {
				ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
				TObject headerFooter = headersFooters.GetObject(modelType);
				if (headerFooter == null) {
					headersFooters.Create(modelType);
					headerFooter = headersFooters.GetObject(modelType);
				}
				SectionHeadersFootersBase container = headerFooter.GetContainer(this.InnerSection);
				if (!container.IsLinkedToPrevious(headerFooter.Type))
					container.LinkToPrevious(headerFooter.Type);
			}
			finally {
				innerSection.DocumentModel.EndUpdate();
			}
		}
		void UnlinkFromPrevious<TObject, TIndex>(HeaderFooterType type, SectionHeadersFooters<TObject, TIndex> headersFooters)
			where TObject : SectionHeaderFooterBase
			where TIndex : struct, IConvertToInt<TIndex> {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			try {
				ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
				TObject headerFooter = headersFooters.GetObject(modelType);
				if (headerFooter == null) {
					headersFooters.Create(modelType);
					headerFooter = headersFooters.GetObject(modelType);
				}
				SectionHeadersFootersBase container = headerFooter.GetContainer(this.InnerSection);
				if (container.IsLinkedToPrevious(headerFooter.Type))
					container.UnlinkFromPrevious(headerFooter.Type);
			}
			finally {
				innerSection.DocumentModel.EndUpdate();
			}
		}
		bool IsLinkedToNext<TObject, TIndex>(HeaderFooterType type, SectionHeadersFooters<TObject, TIndex> headersFooters)
			where TObject : SectionHeaderFooterBase
			where TIndex : struct, IConvertToInt<TIndex> {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			try {
				ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
				TObject headerFooter = headersFooters.GetObject(modelType);
				if (headerFooter != null)
					return headerFooter.GetContainer(this.InnerSection).IsLinkedToNext(headerFooter.Type);
				else
					return false;
			}
			finally {
				innerSection.DocumentModel.EndUpdate();
			}
		}
		void LinkToNext<TObject, TIndex>(HeaderFooterType type, SectionHeadersFooters<TObject, TIndex> headersFooters)
			where TObject : SectionHeaderFooterBase
			where TIndex : struct, IConvertToInt<TIndex> {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			try {
				ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
				TObject headerFooter = headersFooters.GetObject(modelType);
				if (headerFooter == null) {
					headersFooters.Create(modelType);
					headerFooter = headersFooters.GetObject(modelType);
				}
				SectionHeadersFootersBase container = headerFooter.GetContainer(this.InnerSection);
				if (!container.IsLinkedToNext(headerFooter.Type))
					container.LinkToNext(headerFooter.Type);
			}
			finally {
				innerSection.DocumentModel.EndUpdate();
			}
		}
		void UnlinkFromNext<TObject, TIndex>(HeaderFooterType type, SectionHeadersFooters<TObject, TIndex> headersFooters)
			where TObject : SectionHeaderFooterBase
			where TIndex : struct, IConvertToInt<TIndex> {
			CheckValid();
			innerSection.DocumentModel.BeginUpdate();
			try {
				ModelHeaderFooterType modelType = (ModelHeaderFooterType)type;
				TObject headerFooter = headersFooters.GetObject(modelType);
				if (headerFooter == null) {
					headersFooters.Create(modelType);
					headerFooter = headersFooters.GetObject(modelType);
				}
				SectionHeadersFootersBase container = headerFooter.GetContainer(this.InnerSection);
				if (container.IsLinkedToNext(headerFooter.Type))
					container.UnlinkFromNext(headerFooter.Type);
			}
			finally {
				innerSection.DocumentModel.EndUpdate();
			}
		}
		public bool IsHeaderLinkedToPrevious() {
			return IsLinkedToPrevious(HeaderFooterType.Odd, innerSection.Headers);
		}
		public void LinkHeaderToPrevious() {
			LinkToPrevious(HeaderFooterType.Odd, innerSection.Headers);
		}
		public void UnlinkHeaderFromPrevious() {
			UnlinkFromPrevious(HeaderFooterType.Odd, innerSection.Headers);
		}
		public bool IsFooterLinkedToPrevious() {
			return IsLinkedToPrevious(HeaderFooterType.Odd, innerSection.Footers);
		}
		public void LinkFooterToPrevious() {
			LinkToPrevious(HeaderFooterType.Odd, innerSection.Footers);
		}
		public void UnlinkFooterFromPrevious() {
			UnlinkFromPrevious(HeaderFooterType.Odd, innerSection.Footers);
		}
		public bool IsHeaderLinkedToPrevious(HeaderFooterType type) {
			return IsLinkedToPrevious(type, innerSection.Headers);
		}
		public void LinkHeaderToPrevious(HeaderFooterType type) {
			LinkToPrevious(type, innerSection.Headers);
		}
		public void UnlinkHeaderFromPrevious(HeaderFooterType type) {
			UnlinkFromPrevious(type, innerSection.Headers);
		}
		public bool IsFooterLinkedToPrevious(HeaderFooterType type) {
			return IsLinkedToPrevious(type, innerSection.Footers);
		}
		public void LinkFooterToPrevious(HeaderFooterType type) {
			LinkToPrevious(type, innerSection.Footers);
		}
		public void UnlinkFooterFromPrevious(HeaderFooterType type) {
			UnlinkFromPrevious(type, innerSection.Footers);
		}
		public bool IsHeaderLinkedToNext() {
			return IsLinkedToNext(HeaderFooterType.Odd, innerSection.Headers);
		}
		public void LinkHeaderToNext() {
			LinkToNext(HeaderFooterType.Odd, innerSection.Headers);
		}
		public void UnlinkHeaderFromNext() {
			UnlinkFromNext(HeaderFooterType.Odd, innerSection.Headers);
		}
		public bool IsFooterLinkedToNext() {
			return IsLinkedToNext(HeaderFooterType.Odd, innerSection.Footers);
		}
		public void LinkFooterToNext() {
			LinkToNext(HeaderFooterType.Odd, innerSection.Footers);
		}
		public void UnlinkFooterFromNext() {
			UnlinkFromNext(HeaderFooterType.Odd, innerSection.Footers);
		}
		public bool IsHeaderLinkedToNext(HeaderFooterType type) {
			return IsLinkedToNext(type, innerSection.Headers);
		}
		public void LinkHeaderToNext(HeaderFooterType type) {
			LinkToNext(type, innerSection.Headers);
		}
		public void UnlinkHeaderFromNext(HeaderFooterType type) {
			UnlinkFromNext(type, innerSection.Headers);
		}
		public bool IsFooterLinkedToNext(HeaderFooterType type) {
			return IsLinkedToNext(type, innerSection.Footers);
		}
		public void LinkFooterToNext(HeaderFooterType type) {
			LinkToNext(type, innerSection.Footers);
		}
		public void UnlinkFooterFromNext(HeaderFooterType type) {
			UnlinkFromNext(type, innerSection.Footers);
		}
		#endregion
	}
	#endregion
	#region NativeSectionCollection
	public class NativeSectionCollection : List<NativeSection>, SectionCollection {
		#region ISimpleCollection<Section> Members
		Section ISimpleCollection<Section>.this[int index] {
			get { return this[index]; }
		}
		#endregion
		#region IEnumerable<Section> Members
		IEnumerator<Section> IEnumerable<Section>.GetEnumerator() {
			return new EnumeratorAdapter<Section, NativeSection>(this.GetEnumerator()); 
		}
		#endregion
	}
	#endregion
	#region SectionParagraphCollection
	public class SectionParagraphCollection : ReadOnlyParagraphCollection {
		readonly ParagraphCollection paragraphs;
		readonly ModelSection innerSection;
		public SectionParagraphCollection(ParagraphCollection paragraphs, ModelSection innerSection) {
			Guard.ArgumentNotNull(paragraphs, "paragraphs");
			Guard.ArgumentNotNull(innerSection, "innerSection");
			this.paragraphs = paragraphs;
			this.innerSection = innerSection;
		}
		#region ISimpleCollection<Paragraph> Members
		public Paragraph this[int index] {
			get {
				IConvertToInt<ParagraphIndex> firstParagraphIndex = innerSection.FirstParagraphIndex;
				return paragraphs[firstParagraphIndex.ToInt() + index];
			}
		}
		public int Count { get { return innerSection.LastParagraphIndex - innerSection.FirstParagraphIndex + 1; } }
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			int index = 0;
			while (index < Count)
				yield return this[index++];
		}
		#endregion
		#region IEnumerable<Paragraph> Members
		IEnumerator<Paragraph> IEnumerable<Paragraph>.GetEnumerator() {
			int index = 0;
			while (index < Count)
				yield return this[index++];
		}
		#endregion
		#region ICollection Members
		void ICollection.CopyTo(Array array, int index) {
			ICollection collection = this;
			collection.CopyTo(array, index);
		}
		bool ICollection.IsSynchronized {
			get {
				ICollection collection = this;
				return collection.IsSynchronized;
			}
		}
		object ICollection.SyncRoot {
			get {
				ICollection collection = this;
				return collection.SyncRoot;
			}
		}
		#endregion
		public ReadOnlyParagraphCollection Get(DocumentRange range) {
			NativeReadOnlyParagraphCollection result = new NativeReadOnlyParagraphCollection();
			int count = paragraphs.Count;
			for (int i = 0; i < count; i++) {
				Paragraph paragraph = paragraphs[i];
				if ((paragraph.Range.Start >= range.Start) && (paragraph.Range.End <= range.End))
					result.Add(paragraph);
			}
			return result;
		}
		public Paragraph Get(DocumentPosition pos) {
			NativeDocumentPosition position = pos as NativeDocumentPosition;
			ParagraphIndex paragraphIndex = position.Position.ParagraphIndex;
			int index = ((IConvertToInt<ParagraphIndex>)paragraphIndex).ToInt();
			if (index < 0)
				return null;
			return this[index];
		}
	}
	#endregion
	#region
	public class NativeReadOnlyParagraphCollection : NativeReadOnlyCollection<Paragraph, NativeReadOnlyParagraphCollection, ReadOnlyParagraphCollection>, ReadOnlyParagraphCollection {
		protected override NativeReadOnlyParagraphCollection CreateCollection() {
			return new NativeReadOnlyParagraphCollection();
		}
		protected override bool Contains(DocumentRange range, Paragraph item) {
			DocumentPosition start = item.Range.Start;
			return range.Contains(start);
		}
		public Paragraph Get(DocumentPosition pos) {
			NativeDocumentPosition position = pos as NativeDocumentPosition;
			ParagraphIndex paragraphIndex = position.Position.ParagraphIndex;
			int index = ((IConvertToInt<ParagraphIndex>)paragraphIndex).ToInt();
			if (index < 0)
				return null;
			return this[index];
		}
	}
	#endregion
	#region NativeSectionMargins
	public class NativeSectionMargins : SectionMargins {
		#region Fields
		readonly NativeDocument document;
		readonly ModelSectionMargins margins;
		#endregion
		public NativeSectionMargins(NativeDocument document, ModelSectionMargins margins) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(margins, "margins");
			this.document = document;
			this.margins = margins;
		}
		#region Properties
		public ModelSectionMargins Margins { get { return margins; } }
		public NativeDocument Document { get { return document; } }
		#endregion
		#region SectionMargins Members
		float SectionMargins.Left {
			get { return Document.ModelUnitsToUnits(Margins.Left); }
			set { Margins.Left = Document.UnitsToModelUnits(value); }
		}
		float SectionMargins.Top {
			get { return Document.ModelUnitsToUnits(Margins.Top); }
			set { Margins.Top = Document.UnitsToModelUnits(value); }
		}
		float SectionMargins.Right {
			get { return Document.ModelUnitsToUnits(Margins.Right); }
			set { Margins.Right = Document.UnitsToModelUnits(value); }
		}
		float SectionMargins.Bottom {
			get { return Document.ModelUnitsToUnits(Margins.Bottom); }
			set { Margins.Bottom = Document.UnitsToModelUnits(value); }
		}
		float SectionMargins.HeaderOffset {
			get { return Document.ModelUnitsToUnits(Margins.HeaderOffset); }
			set { Margins.HeaderOffset = Document.UnitsToModelUnits(value); }
		}
		float SectionMargins.FooterOffset {
			get { return Document.ModelUnitsToUnits(Margins.FooterOffset); }
			set { Margins.FooterOffset = Document.UnitsToModelUnits(value); }
		}		
		#endregion
	}
	#endregion
	#region NativeSectionPage
	public class NativeSectionPage : SectionPage {
		#region Fields
		readonly NativeDocument document;
		readonly ModelSectionPage page;
		#endregion
		public NativeSectionPage(NativeDocument document, ModelSectionPage page) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(page, "page");
			this.document = document;
			this.page = page;
		}
		#region Properties
		public ModelSectionPage Page { get { return page; } }
		public NativeDocument Document { get { return document; } }
		#endregion
		#region SectionPage Members
		public float Width {
			get {
				if (Landscape)
					return ActualHeight;
				else
					return ActualWidth;
			}
			set {
				if (Landscape)
					ActualHeight = value;
				else
					ActualWidth = value;
			}
		}
		public float Height {
			get {
				if (Landscape)
					return ActualWidth;
				else
					return ActualHeight;
			}
			set {
				if (Landscape)
					ActualWidth = value;
				else
					ActualHeight = value;
			}
		}
		public PaperKind PaperKind {
			get { return Page.PaperKind; }
			set {
				if (Page.PaperKind == value)
					return;
				SetPaperKind(value);
			}
		}
		public bool Landscape {
			get { return Page.Landscape; }
			set {
				if (Page.Landscape == value)
					return;
				SetLandscape(value);
			}
		}
		protected internal virtual float ActualWidth {
			get { return Document.ModelUnitsToUnits(Page.Width); }
			set {
				int width = Document.UnitsToModelUnits(value);
				if (width == Page.Width)
					return;
				SetPageWidth(width);
			}
		}
		protected internal virtual float ActualHeight {
			get { return Document.ModelUnitsToUnits(Page.Height); }
			set {
				int height = Document.UnitsToModelUnits(value);
				if (height == Page.Height)
					return;
				SetPageHeight(height);
			}
		}
		#endregion
		protected internal delegate void SetValueDelegate<T>(T value);
		protected internal void SetPageProperty<T>(SetValueDelegate<T> setter, T value) {
			Document.InternalAPI.DocumentModel.BeginUpdate();
			try {
				Page.BeginUpdate();
				try {
					setter(value);
				}
				finally {
					Page.EndUpdate();
				}
			}
			finally {
				Document.InternalAPI.DocumentModel.EndUpdate();
			}
		}
		protected internal virtual void SetPaperKind(PaperKind paperKind) {
			SetPageProperty(SetPaperKindCore, paperKind);
		}
		protected internal virtual void SetPaperKindCore(PaperKind paperKind) {
			if (paperKind != PaperKind.Custom) {
				Size paperSizeInTwips = PaperSizeCalculator.CalculatePaperSize(paperKind);
				Size paperSizeInModelUnits = document.DocumentModel.UnitConverter.TwipsToModelUnits(paperSizeInTwips);
				Width = Document.ModelUnitsToUnits(paperSizeInModelUnits.Width);
				Height = Document.ModelUnitsToUnits(paperSizeInModelUnits.Height);
			}
			Page.PaperKind = paperKind;
		}
		protected internal virtual void SetPageWidth(int value) {
			SetPageProperty(SetPageWidthCore, value);
		}
		protected internal virtual void SetPageWidthCore(int value) {
			Page.Width = value;
			Page.PaperKind = PaperKind.Custom;
		}
		protected internal virtual void SetPageHeight(int value) {
			SetPageProperty(SetPageHeightCore, value);
		}
		protected internal virtual void SetPageHeightCore(int value) {
			Page.Height = value;
			Page.PaperKind = PaperKind.Custom;
		}
		protected internal virtual void SetLandscape(bool value) {
			SetPageProperty(SetLandscapeCore, value);
		}
		protected internal virtual void SetLandscapeCore(bool value) {
			Page.Landscape = value;
			int width = Page.Width;
			Page.Width = Page.Height;
			Page.Height = width;
		}
	}
	#endregion
	public class NativeSectionColumns : SectionColumns {
		#region Fields
		readonly NativeDocument document;
		readonly ModelSectionColumns columns;
		readonly NativeSectionPage page;
		#endregion
		public NativeSectionColumns(NativeDocument document, ModelSectionColumns columns, NativeSectionPage page) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(columns, "columns");
			Guard.ArgumentNotNull(page, "page");
			this.document = document;
			this.columns = columns;
			this.page = page;
		}
		#region Properties
		public ModelSectionColumns Columns { get { return columns; } }
		public NativeSectionPage Page { get { return page; } }
		public NativeDocument Document { get { return document; } }
		#endregion
		#region SectionColumns Members
		public int Count { get { return GetActualColumnCount(); } } 
		public SectionColumnCollection GetColumns() {
			if (Columns.EqualWidthColumns)
				return CreateUniformColumns(Page, Document.ModelUnitsToUnits(Columns.Space), Count);
			else
				return CreateNonUniformColumns();
		}
		public void SetColumns(SectionColumnCollection columns) {
			int count = columns.Count;
			if (count <= 0)
				return;
			if (IsEqualWidthColumns(columns))
				SetUniformColumns(columns[0].Spacing, count);
			else
				SetNonUniformColumns(columns);
		}
		public SectionColumnCollection CreateUniformColumns(float columnWidth, float columnSpacing, int columnCount) {
			SectionColumnCollection result = new SectionColumnCollection();
			for (int i = 0; i < columnCount; i++)
				result.Add(new SectionColumn(columnWidth, columnSpacing));
			return result;
		}
		public SectionColumnCollection CreateUniformColumns(SectionPage page, float columnSpacing, int columnCount) {
			if (columnCount <= 0)
				return new SectionColumnCollection();
			float columnWidth = (page.Width - (columnSpacing * (columnCount - 1))) / columnCount;
			return CreateUniformColumns(columnWidth, columnSpacing, columnCount);
		}
		#endregion
		int GetActualColumnCount() {
			if (Columns.EqualWidthColumns)
				return Columns.ColumnCount;
			return Columns.GetColumns().Count;
		}
		protected internal virtual SectionColumnCollection CreateNonUniformColumns() {
			SectionColumnCollection result = new SectionColumnCollection();
			ColumnInfoCollection innerColumns = Columns.GetColumns();
			int count = innerColumns.Count;
			for (int i = 0; i < count; i++) {
				float width = Document.ModelUnitsToUnitsF(innerColumns[i].Width);
				float spacing = Document.ModelUnitsToUnitsF(innerColumns[i].Space);
				result.Add(new SectionColumn(width, spacing));
			}
			return result;
		}
		protected internal virtual bool IsEqualWidthColumns(SectionColumnCollection columns) {
			int count = columns.Count;
			if (count <= 0)
				return false;
			float firstWidth = columns[0].Width;
			float firstSpacing = columns[0].Spacing;
			float totalWidth = -columns[count - 1].Spacing;
			for (int i = 0; i < count; i++) {
				float width = columns[i].Width;
				if (width != firstWidth)
					return false;
				float spacing = columns[i].Spacing;
				if (spacing != firstSpacing)
					return false;
				totalWidth += width + spacing;
			}
			return totalWidth == Page.Width;
		}
		protected internal virtual void SetUniformColumns(float spacing, int columnCount) {
			Columns.BeginUpdate();
			try {
				Columns.EqualWidthColumns = true;
				Columns.ColumnCount = columnCount;
				Columns.Space = Document.UnitsToModelUnits(spacing);
			}
			finally {
				Columns.EndUpdate();
			}
		}
		protected internal virtual void SetNonUniformColumns(SectionColumnCollection columns) {
			int count = columns.Count;
			Columns.BeginUpdate();
			try {
				Columns.EqualWidthColumns = false;
				ColumnInfoCollection innerColumns = new ColumnInfoCollection();
				for (int i = 0; i < count; i++) {
					ColumnInfo column = new ColumnInfo();
					column.Width = Document.UnitsToModelUnits(columns[i].Width);
					column.Space = Document.UnitsToModelUnits(columns[i].Spacing);
					innerColumns.Add(column);
				}
				Columns.SetColumns(innerColumns);
			}
			finally {
				Columns.EndUpdate();
			}
		}
	}
	#region NativeSectionLineNumbering
	public class NativeSectionLineNumbering : SectionLineNumbering {
		#region Fields
		readonly NativeDocument document;
		readonly ModelSectionLineNumbering lineNumbering;
		#endregion
		public NativeSectionLineNumbering(NativeDocument document, ModelSectionLineNumbering lineNumbering) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(lineNumbering, "margins");
			this.document = document;
			this.lineNumbering = lineNumbering;
		}
		#region Properties
		public ModelSectionLineNumbering LineNumbering { get { return lineNumbering; } }
		public NativeDocument Document { get { return document; } }
		#endregion
		#region SectionLineNumbering Members
		float SectionLineNumbering.Distance {
			get { return Document.ModelUnitsToUnits(LineNumbering.Distance); }
			set { LineNumbering.Distance = Document.UnitsToModelUnits(value); }
		}
		int SectionLineNumbering.Start {
			get { return LineNumbering.StartingLineNumber; }
			set { LineNumbering.StartingLineNumber = value; }
		}
		int SectionLineNumbering.CountBy {
			get { return LineNumbering.Step; }
			set { LineNumbering.Step = value; }
		}
		LineNumberingRestart SectionLineNumbering.RestartType {
			get { return (LineNumberingRestart)LineNumbering.NumberingRestartType; }
			set { LineNumbering.NumberingRestartType = (DevExpress.XtraRichEdit.Model.LineNumberingRestart)value; }
		}
		#endregion
	}
	#endregion
	#region NativeSectionPageNumbering
	public class NativeSectionPageNumbering : SectionPageNumbering {
		#region Fields
		readonly NativeDocument document;
		readonly ModelSectionPageNumbering pageNumbering;
		#endregion
		public NativeSectionPageNumbering(NativeDocument document, ModelSectionPageNumbering pageNumbering) {
			Guard.ArgumentNotNull(document, "document");
			Guard.ArgumentNotNull(pageNumbering, "pageNumbering");
			this.document = document;
			this.pageNumbering = pageNumbering;
		}
		#region Properties
		public ModelSectionPageNumbering PageNumbering { get { return pageNumbering; } }
		public NativeDocument Document { get { return document; } }
		#endregion
		#region SectionPageNumbering Members
		int SectionPageNumbering.Start {
			get { return pageNumbering.StartingPageNumber; }
			set { pageNumbering.StartingPageNumber = value; }
		}
		NumberingFormat SectionPageNumbering.NumberingFormat {
			get { return (NumberingFormat)pageNumbering.NumberingFormat; }
			set { pageNumbering.NumberingFormat = (DevExpress.Office.NumberConverters.NumberingFormat)value; }
		}
		#endregion
	}
	#endregion
}
