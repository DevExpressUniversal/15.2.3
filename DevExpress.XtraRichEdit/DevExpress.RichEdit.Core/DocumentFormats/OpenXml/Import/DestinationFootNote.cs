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
using System.Xml;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Export.OpenXml;
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region FootNotePropertiesDestinationBase (abstract class)
	public abstract class FootNotePropertiesDestinationBase : ElementDestination {
		readonly SectionFootNote footNote;
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		protected static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("pos", OnPlacement);
			result.Add("numStart", OnNumberingStart);
			result.Add("numFmt", OnNumberingFormat);
			result.Add("numRestart", OnNumberingRestart);
			return result;
		}
		protected FootNotePropertiesDestinationBase(WordProcessingMLBaseImporter importer, SectionFootNote footNote)
			: base(importer) {
			Guard.ArgumentNotNull(footNote, "footNote");
			this.footNote = footNote;
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected internal SectionFootNote FootNote { get { return footNote; } }
		static FootNotePropertiesDestinationBase GetThis(WordProcessingMLBaseImporter importer) {
			return (FootNotePropertiesDestinationBase)importer.PeekDestination();
		}
		static Destination OnPlacement(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer).CreatePlacementDestination(importer);
		}
		static Destination OnNumberingStart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FootNoteNumberingStartDestination(importer, GetThis(importer).FootNote);
		}
		static Destination OnNumberingFormat(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer).CreateNumberingFormatDestination(importer);
		}
		static Destination OnNumberingRestart(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FootNoteNumberingRestartTypeDestination(importer, GetThis(importer).FootNote);
		}
		protected internal abstract Destination CreatePlacementDestination(WordProcessingMLBaseImporter importer);
		protected internal abstract Destination CreateNumberingFormatDestination(WordProcessingMLBaseImporter importer);
	}
	#endregion
	#region DocumentLevelFootNotePropertiesDestination
	public class DocumentLevelFootNotePropertiesDestination : FootNotePropertiesDestinationBase {
		public DocumentLevelFootNotePropertiesDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote)
			: base(importer, footNote) {
		}
		protected internal override Destination CreatePlacementDestination(WordProcessingMLBaseImporter importer) {
			return new FootNotePlacementDestination(importer, FootNote, FootNotePosition.BottomOfPage);
		}
		protected internal override Destination CreateNumberingFormatDestination(WordProcessingMLBaseImporter importer) {
			return new FootNoteNumberingFormatDestination(importer, FootNote, NumberingFormat.Decimal);
		}
	}
	#endregion
	#region SectionLevelFootNotePropertiesDestination
	public class SectionLevelFootNotePropertiesDestination : FootNotePropertiesDestinationBase {
		public SectionLevelFootNotePropertiesDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote)
			: base(importer, footNote) {
		}
		protected internal override Destination CreatePlacementDestination(WordProcessingMLBaseImporter importer) {
			return new FootNotePlacementDestination(importer, FootNote, FootNotePosition.BottomOfPage);
		}
		protected internal override Destination CreateNumberingFormatDestination(WordProcessingMLBaseImporter importer) {
			return new FootNoteNumberingFormatDestination(importer, FootNote, NumberingFormat.Decimal);
		}
	}
	#endregion
	#region FootNotePropertiesLeafElementDestination (abstract class)
	public abstract class FootNotePropertiesLeafElementDestination : LeafElementDestination {
		readonly SectionFootNote footNote;
		protected FootNotePropertiesLeafElementDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote)
			: base(importer) {
			Guard.ArgumentNotNull(footNote, "footNote");
			this.footNote = footNote;
		}
		public SectionFootNote FootNote { get { return footNote; } }
	}
	#endregion
	#region FootNotePlacementDestination
	public class FootNotePlacementDestination : FootNotePropertiesLeafElementDestination {
		readonly FootNotePosition defaultPosition;
		public FootNotePlacementDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote, FootNotePosition defaultPosition)
			: base(importer, footNote) {
			this.defaultPosition = defaultPosition;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FootNote.Position = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.footNotePlacementTable, defaultPosition);
		}
	}
	#endregion
	#region FootNoteNumberingStartDestination
	public class FootNoteNumberingStartDestination : FootNotePropertiesLeafElementDestination {
		public FootNoteNumberingStartDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote)
			: base(importer, footNote) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FootNote.StartingNumber = Importer.GetWpSTIntegerValue(reader, "val", 1);
		}
	}
	#endregion
	#region FootNoteNumberingFormatDestination
	public class FootNoteNumberingFormatDestination : FootNotePropertiesLeafElementDestination {
		readonly NumberingFormat defaultNumberingFormat;
		public FootNoteNumberingFormatDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote, NumberingFormat defaultNumberingFormat)
			: base(importer, footNote) {
			this.defaultNumberingFormat = defaultNumberingFormat;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FootNote.NumberingFormat = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.pageNumberingFormatTable, defaultNumberingFormat);
		}
	}
	#endregion
	#region FootNoteNumberingRestartTypeDestination
	public class FootNoteNumberingRestartTypeDestination : FootNotePropertiesLeafElementDestination {
		public FootNoteNumberingRestartTypeDestination(WordProcessingMLBaseImporter importer, SectionFootNote footNote)
			: base(importer, footNote) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FootNote.NumberingRestartType = Importer.GetWpEnumValue(reader, "val", OpenXmlExporter.lineNumberingRestartTable, LineNumberingRestart.Continuous);
		}
	}
	#endregion
	#region FootNotesDestination
	public class FootNotesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("footnote", OnFootNote);
			return result;
		}
		public FootNotesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnFootNote(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new FootNoteDestination(importer);
		}
	}
	#endregion
	#region FootNoteDestinationBase<T> (abstract class)
	public abstract class FootNoteDestinationBase<T> : BodyDestinationBase where T : FootNoteBase<T> {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("p", OnParagraph);
			result.Add("tbl", OnTable);
			result.Add("bookmarkStart", OnBookmarkStart);
			result.Add("bookmarkEnd", OnBookmarkEnd);
			result.Add("permStart", OnRangePermissionStart);
			result.Add("permEnd", OnRangePermissionEnd);
			result.Add("sdt", OnStructuredDocument);
			result.Add("customXml", OnCustomXml);
			return result;
		}
		string id;
		T newFootNote;
		readonly List<FootNoteRunBase<T>> selfReferenceRuns = new List<FootNoteRunBase<T>>();
		protected FootNoteDestinationBase(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected List<FootNoteRunBase<T>> SelfReferenceRuns { get { return selfReferenceRuns; } }
		static FootNoteDestinationBase<T> GetThis(WordProcessingMLBaseImporter importer) {
			return (FootNoteDestinationBase<T>)importer.PeekDestination();
		}
		public override void ProcessElementOpen(XmlReader reader) {
			this.newFootNote = CreateTargetFootNoteBase();
			PieceTable pieceTable = newFootNote.PieceTable;
			DocumentModel.UnsafeEditor.InsertFirstParagraph(pieceTable);
			Importer.PushCurrentPieceTable(pieceTable);
			id = reader.GetAttribute("id", Importer.WordProcessingNamespaceConst);
		}
		public override void ProcessElementClose(XmlReader reader) {
			PieceTable.FixLastParagraph();
			Importer.InsertBookmarks();
			Importer.InsertRangePermissions();
			PieceTable.FixTables();
			Importer.PopCurrentPieceTable();
			int noteIndex = RegisterNote(newFootNote, id);
			int count = selfReferenceRuns.Count;
			for (int i = 0; i < count; i++)
				selfReferenceRuns[i].NoteIndex = noteIndex;
		}
		static Destination OnTable(WordProcessingMLBaseImporter importer, XmlReader reader) {
			if (importer.DocumentModel.DocumentCapabilities.TablesAllowed)
				return new TableDestination(importer);
			else
				return new TableDisabledDestination(importer);
		}
		static Destination OnParagraph(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return importer.CreateParagraphDestination();
		}
		static Destination OnFootNoteReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer).OnFootNoteReference(reader);
		}
		static Destination OnEndNoteReference(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return GetThis(importer).OnEndNoteReference(reader);
		}
		protected internal abstract T CreateTargetFootNoteBase();
		protected internal abstract int RegisterNote(T note, string id);
		protected internal abstract Destination OnFootNoteReference(XmlReader reader);
		protected internal abstract Destination OnEndNoteReference(XmlReader reader);
	}
	#endregion
	#region FootNoteDestination
	public class FootNoteDestination : FootNoteDestinationBase<FootNote> {
		public FootNoteDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override FootNote CreateTargetFootNoteBase() {
			return new FootNote(DocumentModel);
		}
		protected internal override int RegisterNote(FootNote note, string id) {
			return Importer.RegisterFootNote(note, id);
		}
		protected internal override Destination OnFootNoteReference(XmlReader reader) {
			return new FootNoteSelfReferenceDestination(Importer, SelfReferenceRuns);
		}
		protected internal override Destination OnEndNoteReference(XmlReader reader) {
			return null;
		}
	}
	#endregion
	#region FootNoteReferenceDestination
	public class FootNoteReferenceDestination : LeafElementDestination {
		public FootNoteReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			OpenXmlImporter importer = (OpenXmlImporter)Importer;
			string noteId = reader.GetAttribute("id", Importer.WordProcessingNamespaceConst);
			FootNote note;
			if (!importer.FootNotes.TryGetValue(noteId, out note))
				return;
			int noteIndex = DocumentModel.FootNotes.IndexOf(note);
			Debug.Assert(noteIndex >= 0);
			PieceTable.InsertFootNoteRun(importer.Position, noteIndex);
		}
	}
	#endregion
	#region FootNoteSelfReferenceDestination
	public class FootNoteSelfReferenceDestination : LeafElementDestination {
		readonly List<FootNoteRunBase<FootNote>> selfReferenceRuns;
		public FootNoteSelfReferenceDestination(WordProcessingMLBaseImporter importer, List<FootNoteRunBase<FootNote>> selfReferenceRuns)
			: base(importer) {
			Guard.ArgumentNotNull(selfReferenceRuns, "selfReferenceRuns");
			this.selfReferenceRuns = selfReferenceRuns;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			FootNoteRun run = PieceTable.InsertFootNoteRun(Importer.Position, -1);
			selfReferenceRuns.Add(run);
		}
	}
	#endregion
}
