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
using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model.History {
	#region SectionInsertedHistoryItem
	public class SectionInsertedHistoryItem : ParagraphBaseHistoryItem {
		Section newSection;
		public SectionInsertedHistoryItem(DocumentModel documentModel)
			: base(documentModel.MainPieceTable) {
		}
		public override void Execute() {
			SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			Debug.Assert(sectionIndex >= new SectionIndex(0));
			Section section = DocumentModel.Sections[sectionIndex];
			this.newSection = new Section(DocumentModel);
			newSection.CopyFrom(section);
			base.Execute();
		}
		protected override void UndoCore() {
			SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			Debug.Assert(sectionIndex >= new SectionIndex(0));
			Section section = DocumentModel.Sections[sectionIndex];
			Section newSection = DocumentModel.Sections[sectionIndex + 1];
			section.LastParagraphIndex = newSection.LastParagraphIndex;
			DocumentModel.OnSectionRemoved(sectionIndex + 1);
			DocumentModel.Sections[sectionIndex + 1].UnsubscribeHeadersFootersEvents();
			DocumentModel.Sections.RemoveAt(sectionIndex + 1);
		}
		protected override void RedoCore() {
			SectionIndex sectionIndex = DocumentModel.MainPieceTable.LookupSectionIndexByParagraphIndex(ParagraphIndex);
			Debug.Assert(sectionIndex >= new SectionIndex(0));
			Section section = DocumentModel.Sections[sectionIndex];
			newSection.FirstParagraphIndex = ParagraphIndex + 1;
			newSection.LastParagraphIndex = section.LastParagraphIndex;
			DocumentModel.Sections.Insert(sectionIndex + 1, newSection);
			newSection.SubscribeInnerObjectsEvents();
			section.LastParagraphIndex = ParagraphIndex;
			DocumentModel.OnSectionInserted(sectionIndex + 1);
		}
	}
	#endregion
	#region SectionsDeletedHistoryItem
	public class SectionsDeletedHistoryItem : ParagraphBaseHistoryItem {
		#region Fields
		int deletedSectionsCount = -1;
		List<Section> deletedSections;
		#endregion
		public SectionsDeletedHistoryItem(DocumentModel documentModel)
			: base(documentModel.MainPieceTable) {
		}
		#region Properties
		public int DeletedSectionsCount { get { return deletedSectionsCount; } set { deletedSectionsCount = value; } }
		#endregion
		protected override void UndoCore() {
			SectionCollection sections = DocumentModel.Sections;
			SectionIndex count = SectionIndex + DeletedSectionsCount;
			for (SectionIndex i = SectionIndex; i < count; i++) {
				Section newSection = deletedSections[i - SectionIndex];
				sections[i].FirstParagraphIndex = newSection.LastParagraphIndex + 1;
				sections.Insert(SectionIndex, newSection);
				newSection.SubscribeHeadersFootersEvents();
				DocumentModel.OnSectionInserted(SectionIndex);
			}
		}
		protected override void RedoCore() {
			deletedSections = new List<Section>();
			SectionCollection sections = DocumentModel.Sections;
			SectionIndex count = SectionIndex + DeletedSectionsCount;
			for (SectionIndex i = count - 1; i >= SectionIndex; i--) {
				deletedSections.Add(sections[i]);
				ParagraphIndex firstParagraphIndex = sections[i].FirstParagraphIndex;
				DocumentModel.OnSectionRemoved(i);
				sections[i].UnsubscribeHeadersFootersEvents();
				sections.RemoveAt(i);
				sections[i].FirstParagraphIndex = firstParagraphIndex;
			}
		}
	}
	#endregion
}
