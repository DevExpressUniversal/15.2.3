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
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
namespace DevExpress.XtraRichEdit.Commands {
	#region RichEditCaretBasedCommand (abstract class)
	public abstract class RichEditCaretBasedCommand : RichEditMenuItemSimpleCommand {
		protected RichEditCaretBasedCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal CaretPosition CaretPosition { get { return ActiveView.CaretPosition; } }
		#endregion
		protected internal virtual bool UpdateCaretPosition(DocumentLayoutDetailsLevel detailDevel) {
			return CaretPosition.Update(detailDevel);
		}
		protected internal virtual void ApplyLayoutPreferredPageIndex(int preferredPageIndex) {
			if (preferredPageIndex < 0)
				return;
			CaretPosition.PreferredPageIndex = preferredPageIndex;
			if (CaretPosition is HeaderFooterCaretPosition || CaretPosition is CommentCaretPosition) {
				if (DocumentModel.IsUpdateLockedOrOverlapped)
					CaretPosition.LayoutPosition.PieceTable.ApplyChangesCore(DocumentModelChangeActions.RaiseSelectionChanged | DocumentModelChangeActions.ResetSelectionLayout | DocumentModelChangeActions.ResetRuler | DocumentModelChangeActions.ResetCaretInputPositionFormatting | DocumentModelChangeActions.Redraw, RunIndex.DontCare, RunIndex.DontCare);
			}
			ActiveView.SelectionLayout.PreferredPageIndex = preferredPageIndex;
		}
	}
	#endregion
}
