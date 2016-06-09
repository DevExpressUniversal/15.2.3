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

using System.Collections;
using DevExpress.XtraRichEdit.Model; 
using DevExpress.Web.ASPxRichEdit.Export;
using System;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public class DeleteRunsCommand : WebRichEditUpdateModelCommandBase {
		public DeleteRunsCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.DeleteRuns; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			DocumentLogPosition start = new DocumentLogPosition((int)Parameters["position"]);
			int length = (int)Parameters["length"];
			var ri = PieceTable.ObtainAffectedRunInfo(start, length);
			if(ri.Start.ParagraphIndex != ri.End.ParagraphIndex)
				throw new Exception("This command removes runs only inside one paragraph");
			DocumentModel.UnsafeEditor.DeleteRuns(PieceTable, ri.Start.RunIndex, ri.End.RunIndex - ri.Start.RunIndex + 1);
		}
	}
	public class MergeParagraphsCommand : WebRichEditUpdateModelCommandBase {
		public MergeParagraphsCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.MergeParagraphs; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			DocumentLogPosition position = new DocumentLogPosition((int)Parameters["position"]);
			var getPropertiesFromNext = (bool)Parameters["getPropertiesFromNext"];
			MergeParagraphsCore(position, getPropertiesFromNext);
		}
		protected void MergeParagraphsCore(DocumentLogPosition position, bool getPropertiesFromNext) {
			var paragraphIndex = PieceTable.FindParagraphIndex(position);
			var firstParagraph = PieceTable.Paragraphs[paragraphIndex];
			var secondParagraph = PieceTable.Paragraphs[paragraphIndex + 1];
			var secondParagraphProperties = secondParagraph.ParagraphProperties.Info.Clone();
			if(firstParagraph.Length == 1)
				RemoveWholeParagraph(paragraphIndex);
			else {
				DocumentModel.UnsafeEditor.DeleteRuns(PieceTable, firstParagraph.LastRunIndex, 1);
				DocumentModel.UnsafeEditor.MergeParagraphs(PieceTable, firstParagraph, secondParagraph, !getPropertiesFromNext, null);
				if(getPropertiesFromNext)
					firstParagraph.ParagraphProperties.ReplaceInfo(secondParagraphProperties, ParagraphFormattingChangeActionsCalculator.CalculateChangeActions(ParagraphFormattingChangeType.BatchUpdate));
			}
		}
		protected void RemoveWholeParagraph(ParagraphIndex paragraphIndex) {
			DocumentModel.UnsafeEditor.DeleteAllRunsInParagraph(PieceTable, paragraphIndex);
			DocumentModel.UnsafeEditor.DeleteParagraphs(PieceTable, paragraphIndex, 1, null);
		}
	}
	public class MergeSectionsCommand : MergeParagraphsCommand {
		public MergeSectionsCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		public override CommandType Type { get { return CommandType.MergeSections; } }
		protected override bool IsEnabled() { return true; }
		protected override void PerformModifyModel() {
			DocumentLogPosition position = new DocumentLogPosition((int)Parameters["position"]);
			var getPropertiesFromNext = (bool)Parameters["getPropertiesFromNext"];
			var startSectionIndex = DocumentModel.FindSectionIndex(position);
			DocumentModel.UnsafeEditor.DeleteSections(startSectionIndex, 1);
			MergeParagraphsCore(position, getPropertiesFromNext);
		}
	}
}
