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
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Compatibility.System;
namespace DevExpress.XtraRichEdit {
	#region DocumentExportCapabilities
	[Serializable, StructLayout(LayoutKind.Sequential), ComVisible(false)]
	public struct DocumentExportCapabilities : IConvertToInt<DocumentExportCapabilities> {
		int m_value;
		internal DocumentExportCapabilities(DocumentExportCapabilitiesFlags value) {
			m_value = (int)value;
		}
		#region Properties
		public bool CharacterFormatting {
			get { return GetValue(DocumentExportCapabilitiesFlags.CharacterFormatting); }
			set { SetValue(DocumentExportCapabilitiesFlags.CharacterFormatting, value); }
		}
		public bool ParagraphFormatting {
			get { return GetValue(DocumentExportCapabilitiesFlags.ParagraphFormatting); }
			set { SetValue(DocumentExportCapabilitiesFlags.ParagraphFormatting, value); }
		}
		public bool InlineObjects {
			get { return GetValue(DocumentExportCapabilitiesFlags.InlineObjects); }
			set { SetValue(DocumentExportCapabilitiesFlags.InlineObjects, value); }
		}
		public bool InlinePictures {
			get { return GetValue(DocumentExportCapabilitiesFlags.InlinePictures); }
			set { SetValue(DocumentExportCapabilitiesFlags.InlinePictures, value); }
		}
		public bool Sections {
			get { return GetValue(DocumentExportCapabilitiesFlags.Sections); }
			set { SetValue(DocumentExportCapabilitiesFlags.Sections, value); }
		}
		public bool Styles {
			get { return GetValue(DocumentExportCapabilitiesFlags.Styles); }
			set { SetValue(DocumentExportCapabilitiesFlags.Styles, value); }
		}
		public bool Hyperlinks {
			get { return GetValue(DocumentExportCapabilitiesFlags.Hyperlinks); }
			set { SetValue(DocumentExportCapabilitiesFlags.Hyperlinks, value); }
		}
		public bool Bookmarks {
			get { return GetValue(DocumentExportCapabilitiesFlags.Bookmarks); }
			set { SetValue(DocumentExportCapabilitiesFlags.Bookmarks, value); }
		}
		public bool Headers {
			get { return GetValue(DocumentExportCapabilitiesFlags.Headers); }
			set { SetValue(DocumentExportCapabilitiesFlags.Headers, value); }
		}
		public bool Footers {
			get { return GetValue(DocumentExportCapabilitiesFlags.Footers); }
			set { SetValue(DocumentExportCapabilitiesFlags.Footers, value); }
		}
		#endregion
		internal bool GetValue(DocumentExportCapabilitiesFlags mask) {
			return (m_value & (int)mask) != 0;
		}
		internal void SetValue(DocumentExportCapabilitiesFlags mask, bool value) {
			if (value)
				m_value |= (int)mask;
			else
				m_value &= ~(int)mask;
		}
		public bool Contains(DocumentExportCapabilities value) {
			int val = ((IConvertToInt<DocumentExportCapabilities>)value).ToInt();
			return (m_value & val) == val;
		}
		public override bool Equals(object obj) {
			return ((obj is DocumentExportCapabilities) && (this.m_value == ((DocumentExportCapabilities)obj).m_value));
		}
		public override int GetHashCode() {
			return m_value.GetHashCode();
		}
		public override string ToString() {
			return m_value.ToString();
		}
		public static bool operator ==(DocumentExportCapabilities id1, DocumentExportCapabilities id2) {
			return id1.m_value == id2.m_value;
		}
		public static bool operator !=(DocumentExportCapabilities id1, DocumentExportCapabilities id2) {
			return id1.m_value != id2.m_value;
		}
		#region IConvertToInt<DocumentExportCapabilities> Members
		int IConvertToInt<DocumentExportCapabilities>.ToInt() {
			return m_value;
		}
		DocumentExportCapabilities IConvertToInt<DocumentExportCapabilities>.FromInt(int value) {
			return new DocumentExportCapabilities((DocumentExportCapabilitiesFlags)value);
		}
		#endregion
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Model {
	#region DocumentCapabilitiesFlags
	[Flags]
	public enum DocumentExportCapabilitiesFlags {
		None = 0x00000000,
		CharacterFormatting = 0x00000001,
		ParagraphFormatting = 0x00000002,
		InlineObjects = 0x00000004,
		InlinePictures = 0x00000008,
		Sections = 0x00000010,
		Styles = 0x00000020,
		Fields = 0x00000040,
		Hyperlinks = 0x00000080,
		Bookmarks = 0x00000100,
		Headers = 0x00000200,
		Footers = 0x00000400,
		FootNotes = 0x00000800,
		EndNotes = 0x00001000,
	}
	#endregion
	#region DocumentExportCapabilitiesCalculator
	public class DocumentExportCapabilitiesCalculator : DocumentModelExporter {
		DocumentExportCapabilitiesFlags capabilities;
		public DocumentExportCapabilitiesCalculator(DocumentModel documentModel)
			: base(documentModel) {
		}
		protected internal override bool ShouldExportHiddenText { get { return true; } }
		public virtual DocumentExportCapabilities Calculate() {
			return new DocumentExportCapabilities(CalculateFlags());
		}
		protected internal virtual DocumentExportCapabilitiesFlags CalculateFlags() {
			this.capabilities = DocumentExportCapabilitiesFlags.None;
			Export();
			return capabilities;
		}
		protected internal override void ExportSection(Section section) {
			if (section.FirstParagraphIndex != ParagraphIndex.Zero ||
				section.LastParagraphIndex != PieceTable.Paragraphs.Last.Index ||
				section.Columns.Index != 0 ||
				section.GeneralSettings.Index != 0 ||
				section.LineNumbering.Index != 0 ||
				section.Margins.Index != 0 ||
				section.Page.Index != 0 ||
				section.PageNumbering.Index != 0 ||
				section.InnerFirstPageHeader != null ||
				section.InnerOddPageHeader != null ||
				section.InnerEvenPageHeader != null ||
				section.InnerFirstPageFooter != null ||
				section.InnerOddPageFooter != null ||
				section.InnerEvenPageFooter != null)
				capabilities |= DocumentExportCapabilitiesFlags.Sections;
			base.ExportSection(section);
		}
		protected internal override void ExportFirstPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			capabilities |= DocumentExportCapabilitiesFlags.Headers;
			base.ExportFirstPageHeader(sectionHeader, linkedToPrevious);
		}
		protected internal override void ExportOddPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			capabilities |= DocumentExportCapabilitiesFlags.Headers;
			base.ExportOddPageHeader(sectionHeader, linkedToPrevious);
		}
		protected internal override void ExportEvenPageHeader(SectionHeader sectionHeader, bool linkedToPrevious) {
			capabilities |= DocumentExportCapabilitiesFlags.Headers;
			base.ExportEvenPageHeader(sectionHeader, linkedToPrevious);
		}
		protected internal override void ExportFirstPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			capabilities |= DocumentExportCapabilitiesFlags.Footers;
			base.ExportFirstPageFooter(sectionFooter, linkedToPrevious);
		}
		protected internal override void ExportOddPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			capabilities |= DocumentExportCapabilitiesFlags.Footers;
			base.ExportOddPageFooter(sectionFooter, linkedToPrevious);
		}
		protected internal override void ExportEvenPageFooter(SectionFooter sectionFooter, bool linkedToPrevious) {
			capabilities |= DocumentExportCapabilitiesFlags.Footers;
			base.ExportEvenPageFooter(sectionFooter, linkedToPrevious);
		}
		protected internal override ParagraphIndex ExportParagraph(Paragraph paragraph) {
			if (paragraph.ParagraphStyleIndex != 0)
				capabilities |= DocumentExportCapabilitiesFlags.Styles;
			if (paragraph.ParagraphProperties.Index != 0)
				capabilities |= DocumentExportCapabilitiesFlags.ParagraphFormatting;
			return base.ExportParagraph(paragraph);
		}
		protected internal override void ExportTextRun(TextRun run) {
			if (run.CharacterStyleIndex != 0)
				capabilities |= DocumentExportCapabilitiesFlags.Styles;
			if (run.CharacterProperties.Index != 0)
				capabilities |= DocumentExportCapabilitiesFlags.CharacterFormatting;
		}
		protected internal override void ExportInlineObjectRun(InlineObjectRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.InlineObjects;
		}
		protected internal override void ExportInlinePictureRun(InlinePictureRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.InlinePictures;
		}
		protected internal override void ExportParagraphRun(ParagraphRun run) {
		}
		protected internal override void ExportSectionRun(SectionRun run) {
		}
		protected internal virtual void CheckFieldIsHyperlink(TextRunBase run) {
			Field field = PieceTable.FindFieldByRunIndex(run.GetRunIndex());
			if (PieceTable.HyperlinkInfos.IsHyperlink(field.Index))
				capabilities |= DocumentExportCapabilitiesFlags.Hyperlinks;
		}
		protected internal override void ExportFieldCodeStartRun(FieldCodeStartRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.Fields;
			CheckFieldIsHyperlink(run);
		}
		protected internal override void ExportFieldCodeEndRun(FieldCodeEndRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.Fields;
			CheckFieldIsHyperlink(run);
		}
		protected internal override void ExportFieldResultEndRun(FieldResultEndRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.Fields;
			CheckFieldIsHyperlink(run);
		}
		protected internal override void ExportFootNoteRun(FootNoteRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.FootNotes;
		}
		protected internal override void ExportEndNoteRun(EndNoteRun run) {
			capabilities |= DocumentExportCapabilitiesFlags.EndNotes;
		}
		protected internal override void ExportBookmarkStart(Bookmark bookmark) {
			capabilities |= DocumentExportCapabilitiesFlags.Bookmarks;
		}
		protected internal override void ExportBookmarkEnd(Bookmark bookmark) {
			capabilities |= DocumentExportCapabilitiesFlags.Bookmarks;
		}
		protected internal override ProgressIndication CreateProgressIndication() {
			return new EmptyProgressIndication(DocumentModel);
		}
	}
	#endregion
}
