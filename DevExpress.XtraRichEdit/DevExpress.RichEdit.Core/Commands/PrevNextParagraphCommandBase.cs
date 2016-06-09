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
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Native;
namespace DevExpress.XtraRichEdit.Commands {
	#region PrevNextParagraphCommandBase (abstract class)
	public abstract class PrevNextParagraphCommandBase : RichEditSelectionCommand {
		protected PrevNextParagraphCommandBase(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override bool TryToKeepCaretX { get { return false; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected internal override DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel { get { return DocumentLayoutDetailsLevel.None; } }
		protected IVisibleTextFilter VisibleTextFilter { get { return ActivePieceTable.NavigationVisibleTextFilter; } }
		#endregion
		protected internal override bool CanChangePosition(DocumentModelPosition pos) {
			return true;
		}
		protected virtual DocumentLogPosition GetVisibleLogPosition(Paragraph paragraph) {
			RunIndex runIndex = paragraph.FirstRunIndex;
			if (VisibleTextFilter.IsRunVisible(runIndex))
				return paragraph.LogPosition;
			return GetValidLogPosition(GetNextVisiblePosition(runIndex), paragraph);
		}
		protected virtual DocumentLogPosition GetValidLogPosition(DocumentModelPosition newPos, Paragraph paragraph) {
			return newPos.LogPosition;
		}
		protected virtual DocumentModelPosition GetNextVisiblePosition(RunIndex runIndex) {
			RunIndex result = VisibleTextFilter.GetNextVisibleRunIndex(runIndex);
			return DocumentModelPosition.FromRunStart(ActivePieceTable, result);
		}
		protected virtual DocumentModelPosition GetPrevVisiblePosition(RunIndex runIndex) {
			RunIndex result = VisibleTextFilter.GetPrevVisibleRunIndex(runIndex);
			return DocumentModelPosition.FromRunStart(ActivePieceTable, result);
		}
		protected virtual DocumentLogPosition GetNextVisiblePosition(DocumentModelPosition pos) {
			DocumentLogPosition result = VisibleTextFilter.GetNextVisibleLogPosition(pos, false);
			return result < ActivePieceTable.DocumentEndLogPosition ? result - 1 : result;
		}
		protected virtual DocumentLogPosition GetPrevVisiblePosition(DocumentModelPosition pos) {
			DocumentLogPosition result = VisibleTextFilter.GetPrevVisibleLogPosition(pos, false);
			return result > ActivePieceTable.DocumentStartLogPosition ? result + 1 : result;
		}
	}
	#endregion
}
