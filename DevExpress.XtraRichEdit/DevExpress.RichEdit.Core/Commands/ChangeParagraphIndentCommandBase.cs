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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ChangeParagraphIndentCommandBase (abstract class)
	public abstract class ChangeParagraphIndentCommandBase<T> : ChangeParagraphFormattingCommandBase<T> where T : struct {
		List<int> tabsList;
		protected ChangeParagraphIndentCommandBase(IRichEditControl control)
			: base(control) {
			this.tabsList = new List<int>();
		}
		#region Properties
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementParagraphLeftIndent; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_IncrementParagraphLeftIndentDescription; } }
		protected internal List<int> TabsList { get { return tabsList; } set { tabsList = value; } }
		protected internal ParagraphIndex StartParagraphIndex { get { return DocumentModel.Selection.Interval.NormalizedStart.ParagraphIndex; } }
		protected internal ParagraphIndex EndParagraphIndex { get { return DocumentModel.Selection.Interval.NormalizedEnd.ParagraphIndex; } }
		protected internal int DefaultTabWidth { get { return DocumentModel.DocumentProperties.DefaultTabWidth; } }
		#endregion
		protected internal virtual void FillTabsList() {
			ParagraphCollection paragraphs = ActivePieceTable.Paragraphs;
			TabFormattingInfo tabFormattingInfo = paragraphs[StartParagraphIndex].GetTabs();
			for (int i = 0; i < tabFormattingInfo.Count; i++)
				TabsList.Add(tabFormattingInfo[i].Position);
			if (StartParagraphIndex == ParagraphIndex.Zero && StartParagraphIndex == EndParagraphIndex) {
				AddParagraphTabs(paragraphs[StartParagraphIndex]);
			}
			else {
				if (StartParagraphIndex > ParagraphIndex.Zero) {
					AddParagraphTabs(paragraphs[StartParagraphIndex - 1]);
				}
				if (EndParagraphIndex < new ParagraphIndex(paragraphs.Count - 1)) {
					AddParagraphTabs(paragraphs[EndParagraphIndex + 1]);
				}
			}
			TabsList.Sort();
		}
		protected internal virtual void AddParagraphTabs(Paragraph paragraph) {
			TabsList.Add(paragraph.LeftIndent);
			if(paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Hanging)
				TabsList.Add(paragraph.LeftIndent - paragraph.FirstLineIndent);
			else if (paragraph.FirstLineIndentType == ParagraphFirstLineIndent.Indented)
				TabsList.Add(paragraph.LeftIndent + paragraph.FirstLineIndent);
		}
		protected internal int GetNearRightDefaultTab(int leftIndent) {
			return ((leftIndent / DefaultTabWidth) + 1) * DefaultTabWidth;
		}
		protected internal int GetNearRightTab(int leftIndent) {
			for (int i = 0; i < TabsList.Count; i++) {
				if (leftIndent < TabsList[i])
					return TabsList[i];
			}
			return leftIndent;
		}
		protected internal int GetNearLeftDefaultTab(int leftIndent) {
			int nearestLeftDefaultTab = leftIndent / DefaultTabWidth;
			if (nearestLeftDefaultTab > 0) {
				if(leftIndent % DefaultTabWidth != 0)
					return nearestLeftDefaultTab * DefaultTabWidth;
				else
					return (nearestLeftDefaultTab - 1) * DefaultTabWidth;
			}
			return nearestLeftDefaultTab;
		}
		protected internal int GetNearLeftTab(int leftIndent) {
			for (int i = TabsList.Count - 1; i >= 0; i--) {
				if (leftIndent > TabsList[i])
					return TabsList[i];
			}
			return leftIndent;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
