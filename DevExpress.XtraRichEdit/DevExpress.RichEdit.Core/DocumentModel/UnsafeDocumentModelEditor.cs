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
using System.Text;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Utils;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Model {
	#region UnsafeDocumentModelEditor
	public class UnsafeDocumentModelEditor {
		readonly DocumentModel documentModel;
		public UnsafeDocumentModelEditor(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
		}
		public DocumentModel DocumentModel { get { return documentModel; } }
		public void InsertFirstSection() {
			Section section = new Section(DocumentModel);
			section.FirstParagraphIndex = new ParagraphIndex(0);
			section.LastParagraphIndex = new ParagraphIndex(0);
			DocumentModel.Sections.Add(section);
		}
		public void InsertFirstParagraph(PieceTable pieceTable) {
			Debug.Assert(pieceTable.Runs.Count == 0);
			pieceTable.TextBuffer.Append(Characters.ParagraphMark);
			Paragraph paragraph = new Paragraph(pieceTable);
#if UseOldIndicies
			paragraph.LastRunIndex = new RunIndex(0);
			paragraph.FirstRunIndex = new RunIndex(0);			
			paragraph.LogPosition = DocumentLogPosition.Zero;
#endif
			paragraph.Length = 1;
			pieceTable.Paragraphs.Add(paragraph);
			paragraph.SetRelativeFirstRunIndex(new RunIndex(0));
			paragraph.SetRelativeLastRunIndex(new RunIndex(0));
			paragraph.SetRelativeLogPosition(DocumentLogPosition.Zero);
			pieceTable.Paragraphs.CheckTree(paragraph);
			ParagraphRun paragraphMarkRun = new ParagraphRun(paragraph);
			paragraphMarkRun.StartIndex = pieceTable.TextBuffer.Length - 1;
			pieceTable.Runs.Add(paragraphMarkRun);
		}
		public void DeleteAllRunsInParagraph(PieceTable pieceTable, ParagraphIndex paragraphIndex) {
			ParagraphCollection paragraphs = pieceTable.Paragraphs;
			TextRunsDeletedHistoryItem item = new TextRunsDeletedHistoryItem(pieceTable);
			item.ParagraphIndex = paragraphIndex;
			item.RunIndex = paragraphs[paragraphIndex].FirstRunIndex;
			item.DeletedRunCount = paragraphs[paragraphIndex].LastRunIndex - paragraphs[paragraphIndex].FirstRunIndex + 1;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		internal void DeleteSections(SectionIndex startSectionIndex, int count) {
			SectionsDeletedHistoryItem item = new SectionsDeletedHistoryItem(DocumentModel);
			item.SectionIndex = startSectionIndex;
			item.DeletedSectionsCount = count;
			item.Execute();
			DocumentModel.History.Add(item);
		}
		public void DeleteParagraphs(PieceTable pieceTable, ParagraphIndex startParagraphIndex, int count, TableCell cell) {
			ParagraphsDeletedHistoryItem item = new ParagraphsDeletedHistoryItem(pieceTable);
			item.SectionIndex = pieceTable.LookupSectionIndexByParagraphIndex(startParagraphIndex);
			item.ParagraphIndex = startParagraphIndex;
			item.DeletedParagraphsCount = count;
			item.SetTableCell(cell);
			item.Execute();
			DocumentModel.History.Add(item);
		}
		public void DeleteRuns(PieceTable pieceTable, RunIndex startRunIndex, int count) {
			TextRunsDeletedHistoryItem item = new TextRunsDeletedHistoryItem(pieceTable);
			item.ParagraphIndex = pieceTable.Runs[startRunIndex].Paragraph.Index;
			item.RunIndex = startRunIndex;
			item.DeletedRunCount = count;
			DocumentModel.History.Add(item);
			item.Execute();
		}
		public void MergeParagraphs(PieceTable pieceTable, Paragraph firstParagraph, Paragraph secondParagraph, bool useFirstParagraphStyle, TableCell cell) {
			MergeParagraphsHistoryItem item = new MergeParagraphsHistoryItem(pieceTable);
			item.StartParagraph = firstParagraph;
			item.EndParagraph = secondParagraph;
			item.UseFirstParagraphStyle = useFirstParagraphStyle;
			item.SetTableCell(cell);
			DocumentModel.History.Add(item);
			item.Execute();
		}
		public void ReplaceSectionRunWithParagraphRun(PieceTable pieceTable, SectionRun sectionRun, RunIndex runIndex) {
			ParagraphRun paragraphRun = sectionRun.CreateParagraphRun();
			pieceTable.Runs[runIndex] = paragraphRun;
			pieceTable.TextBuffer[paragraphRun.StartIndex] = Characters.ParagraphMark;
		}
		public void ReplaceParagraphRunWithSectionRun(PieceTable pieceTable, SectionRun sectionRun, RunIndex runIndex) {
			ParagraphRun paragraphRun = pieceTable.Runs[runIndex] as ParagraphRun;
			if (paragraphRun != null) {
				pieceTable.Runs[runIndex] = sectionRun;
				pieceTable.TextBuffer[paragraphRun.StartIndex] = Characters.SectionMark;
			}
		}
	}
	#endregion
}
