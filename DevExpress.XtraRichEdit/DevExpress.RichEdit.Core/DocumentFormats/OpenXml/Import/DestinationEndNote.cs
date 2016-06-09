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
using DevExpress.XtraRichEdit.Utils;
using DevExpress.Office.NumberConverters;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region DocumentLevelEndNotePropertiesDestination
	public class DocumentLevelEndNotePropertiesDestination : FootNotePropertiesDestinationBase {
		public DocumentLevelEndNotePropertiesDestination(WordProcessingMLBaseImporter importer, SectionFootNote EndNote)
			: base(importer, EndNote) {
		}
		protected internal override Destination CreatePlacementDestination(WordProcessingMLBaseImporter importer) {
			return new FootNotePlacementDestination(importer, FootNote, FootNotePosition.EndOfDocument);
		}
		protected internal override Destination CreateNumberingFormatDestination(WordProcessingMLBaseImporter importer) {
			return new FootNoteNumberingFormatDestination(importer, FootNote, NumberingFormat.LowerRoman);
		}
	}
	#endregion
	#region SectionLevelEndNotePropertiesDestination
	public class SectionLevelEndNotePropertiesDestination : FootNotePropertiesDestinationBase {
		public SectionLevelEndNotePropertiesDestination(WordProcessingMLBaseImporter importer, SectionFootNote EndNote)
			: base(importer, EndNote) {
		}
		protected internal override Destination CreatePlacementDestination(WordProcessingMLBaseImporter importer) {
			return new FootNotePlacementDestination(importer, FootNote, FootNotePosition.EndOfDocument);
		}
		protected internal override Destination CreateNumberingFormatDestination(WordProcessingMLBaseImporter importer) {
			return new FootNoteNumberingFormatDestination(importer, FootNote, NumberingFormat.LowerRoman);
		}
	}
	#endregion
	#region EndNotesDestination
	public class EndNotesDestination : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("endnote", OnFootNote);
			return result;
		}
		public EndNotesDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
		protected static Destination OnFootNote(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new EndNoteDestination(importer);
		}
	}
	#endregion
	#region EndNoteDestination
	public class EndNoteDestination : FootNoteDestinationBase<EndNote> {
		public EndNoteDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override EndNote CreateTargetFootNoteBase() {
			return new EndNote(DocumentModel);
		}
		protected internal override int RegisterNote(EndNote note, string id) {
			return Importer.RegisterEndNote(note, id);
		}
		protected internal override Destination OnFootNoteReference(XmlReader reader) {
			return null;
		}
		protected internal override Destination OnEndNoteReference(XmlReader reader) {
			return new EndNoteSelfReferenceDestination(Importer, SelfReferenceRuns);
		}
	}
	#endregion
	#region EndNoteReferenceDestination
	public class EndNoteReferenceDestination : LeafElementDestination {
		public EndNoteReferenceDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			OpenXmlImporter importer = (OpenXmlImporter)Importer;
			string noteId = reader.GetAttribute("id", Importer.WordProcessingNamespaceConst);
			EndNote note;
			if (!importer.EndNotes.TryGetValue(noteId, out note))
				return;
			int noteIndex = DocumentModel.EndNotes.IndexOf(note);
			Debug.Assert(noteIndex >= 0);
			PieceTable.InsertEndNoteRun(importer.Position, noteIndex);
		}
	}
	#endregion
	#region EndNoteSelfReferenceDestination
	public class EndNoteSelfReferenceDestination : LeafElementDestination {
		readonly List<FootNoteRunBase<EndNote>> selfReferenceRuns;
		public EndNoteSelfReferenceDestination(WordProcessingMLBaseImporter importer, List<FootNoteRunBase<EndNote>> selfReferenceRuns)
			: base(importer) {
			Guard.ArgumentNotNull(selfReferenceRuns, "selfReferenceRuns");
			this.selfReferenceRuns = selfReferenceRuns;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			EndNoteRun run = PieceTable.InsertEndNoteRun(Importer.Position, -1);
			selfReferenceRuns.Add(run);
		}
	}
	#endregion
}
