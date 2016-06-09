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
using System.ComponentModel;
using System.Diagnostics;
using DevExpress.Office.Utils;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Utils;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.XtraRichEdit.Commands {
	#region NextPageCommand
	public class NextPageCommand : PrevNextPageCommandBase {
		public NextPageCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextPageCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.NextPage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextPageCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveNextPage; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("NextPageCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_MoveNextPageDescription; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		#endregion
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			PageCollection pages = ActiveView.FormattingController.PageController.Pages;
			int pageIndex = pages.IndexOf(CaretPosition.LayoutPosition.Page);
			Debug.Assert(pageIndex >= 0);
			if (pageIndex + 1 < pages.Count) {
				pageIndex++;
				return pages[pageIndex].GetFirstPosition(ActivePieceTable).LogPosition;
			}
			else
				return GetLastPosition(pageIndex);
		}
		protected internal virtual DocumentLogPosition GetLastPosition(int pageIndex) {
			PageCollection pages = ActiveView.FormattingController.PageController.Pages;
			return pages[pageIndex].GetFirstPosition(ActivePieceTable).LogPosition;
		}
	}
	#endregion
	#region ExtendNextPageCommand
	public class ExtendNextPageCommand : NextPageCommand {
		public ExtendNextPageCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ExtendNextPageCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.ExtendNextPage; } }
		protected internal override bool ExtendSelection { get { return true; } }
		protected internal override DocumentLogPosition GetLastPosition(int pageIndex) {
			return ActivePieceTable.DocumentEndLogPosition;
		}
	}
	#endregion
}
