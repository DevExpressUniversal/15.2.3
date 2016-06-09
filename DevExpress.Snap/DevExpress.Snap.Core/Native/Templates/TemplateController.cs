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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.Snap.Core.Native.Templates {
	public enum TemplateContentType {
		Text,
		Table,
		NoTemplate
	}
	public class TemplateController {
		readonly SnapPieceTable pieceTable;
		public TemplateController(SnapPieceTable pieceTable) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");
			this.pieceTable = pieceTable;
		}
		public DocumentLogInterval GetActualTemplateInterval(DocumentLogInterval interval) {
			Guard.ArgumentNotNull(interval, "interval");
			DocumentModelPosition startPosition = PositionConverter.ToDocumentModelPosition(pieceTable, interval.Start);
			ParagraphRun paragraphRun = pieceTable.Runs[startPosition.RunIndex] as ParagraphRun;
			if (paragraphRun == null)
				return interval;
			return new DocumentLogInterval(interval.Start + 1, interval.Length - 1);
		}
		TemplateStartEndType GetTemplateStartEndType(DocumentLogPosition logPosition) {
			DocumentModelPosition position = PositionConverter.ToDocumentModelPosition(pieceTable, logPosition);
			Paragraph paragraph = pieceTable.Paragraphs[position.ParagraphIndex];
			if (paragraph.IsInCell())
				return TemplateStartEndType.Table;
			return paragraph.EndLogPosition == logPosition ? TemplateStartEndType.ParagraphRun : TemplateStartEndType.Text;
		}
		public TemplateContentType GetTemplateType(DocumentLogInterval interval, out Table table) {
			Guard.ArgumentNotNull(interval, "interval");
			table = null;
			DocumentModelPosition start = PositionConverter.ToDocumentModelPosition(pieceTable, interval.Start);
			DocumentModelPosition end = PositionConverter.ToDocumentModelPosition(pieceTable, interval.Start + interval.Length - 1);
			bool startFromParagraphRun = pieceTable.Runs[start.RunIndex] is ParagraphRun;
			bool endByParagraphRun = pieceTable.Runs[end.RunIndex] is ParagraphRun;
			if (!startFromParagraphRun || !endByParagraphRun)
				return TemplateContentType.Text;			
			Paragraph secondParagraph = pieceTable.Paragraphs[start.ParagraphIndex + 1];
			Paragraph lastParagraph = pieceTable.Paragraphs[end.ParagraphIndex];
			TableCell startTableCell = secondParagraph.GetCell();
			if (startTableCell == null)
				return TemplateContentType.Text;
			Table startTable = startTableCell.Table;
			if (startTable == null)
				return TemplateContentType.Text;
			TableCell endTableCell = lastParagraph.GetCell();
			if (endTableCell == null)
				return TemplateContentType.Text;
			Table endTable = endTableCell.Table;
			if (endTable == null)
				return TemplateContentType.Text;
			if (Object.ReferenceEquals(startTable, endTable) || endTable.ContainsTable(startTable)) {
				if (endTable.LastRow.LastCell.EndParagraphIndex == lastParagraph.Index) {
					table = startTable;
					return TemplateContentType.Table;
				}
				else return TemplateContentType.Text;
			}
			else
				return TemplateContentType.Text;
		}
		public TemplateInfo GetTemplateInfo(DocumentLogInterval interval, SnapTemplateIntervalType templateType) {
			if (interval == null)
				return new TemplateInfo(pieceTable, null, null, TemplateContentType.NoTemplate, TemplateStartEndType.Text, TemplateStartEndType.Text, templateType);
			DocumentLogInterval actualInterval = GetActualTemplateInterval(interval);
			TemplateStartEndType startType = GetTemplateStartEndType(actualInterval.Start);			
			TemplateStartEndType endType = actualInterval.Length > 0 ? GetTemplateStartEndType(actualInterval.Start + actualInterval.Length - 1) : startType;
			Table table;
			TemplateContentType templateContentType = GetTemplateType(interval, out table);
			return new TemplateInfo(pieceTable, interval, actualInterval, templateContentType, startType, endType, templateType);
		}
		public DocumentLogInterval GetActualSourceInterval(DocumentLogInterval sourceInterval, SnapTemplateInterval snapTemplateInterval) {
			DocumentLogInterval actualTemplateInterval = GetActualTemplateInterval(new DocumentLogInterval(snapTemplateInterval.Start, snapTemplateInterval.Length));
			TemplateStartEndType startType = GetTemplateStartEndType(actualTemplateInterval.Start);
			if (startType == TemplateStartEndType.Text || startType == TemplateStartEndType.Table)
				return new DocumentLogInterval(sourceInterval.Start + 1, sourceInterval.Length);
			else
				return sourceInterval;
		}
		public DocumentLogInterval GetActualResultInterval(RunInfo runInfo) {
			DocumentLogPosition start = runInfo.Start.LogPosition;
			DocumentLogPosition end = runInfo.End.LogPosition;
			if (pieceTable.Runs[runInfo.Start.RunIndex] is SeparatorTextRun)
				start = Algorithms.Min(pieceTable.DocumentEndLogPosition, start + 1);
			TextRunBase lastRun = pieceTable.Runs[runInfo.End.RunIndex];
			if (lastRun is SeparatorTextRun || lastRun is FieldResultEndRun)
				end = Algorithms.Max(pieceTable.DocumentStartLogPosition, end - 1);
			return new DocumentLogInterval(start, end - start);
		}
	}
	public enum TemplateStartEndType {
		Text,
		ParagraphRun,
		Table
	}
	public class TemplateInfo {
		public TemplateInfo(SnapPieceTable pieceTable, DocumentLogInterval interval, DocumentLogInterval actualInterval, TemplateContentType templateContentType, TemplateStartEndType startType, TemplateStartEndType endType, SnapTemplateIntervalType templateType) {
			Guard.ArgumentNotNull(pieceTable, "pieceTable");			
			if (templateContentType != Templates.TemplateContentType.NoTemplate) {
				Guard.ArgumentNotNull(interval, "interval");
				Guard.ArgumentNotNull(actualInterval, "actualInterval");
			}
			PieceTable = pieceTable;
			TemplateContentType = templateContentType;			
			ActualInterval = actualInterval;
			Interval = interval;			
			StartType = startType;
			EndType = endType;
			TemplateType = templateType;
		}
		public SnapPieceTable PieceTable { get; private set; }
		public DocumentLogInterval Interval { get; private set; }
		public DocumentLogInterval ActualInterval { get; private set; }
		public TemplateContentType TemplateContentType { get; private set; }
		public TemplateStartEndType StartType { get; private set; }
		public TemplateStartEndType EndType { get; private set; }
		public SnapTemplateIntervalType TemplateType { get; set; }
		public bool SeparateAsPageBreakBefore { get; set; }
	}
}
