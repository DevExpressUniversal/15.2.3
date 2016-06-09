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
using DevExpress.Utils.Commands;
namespace DevExpress.XtraRichEdit.Commands {
	#region GoToPageCommand
	public class GoToPageCommand : PrevNextPageCommandBase {
		int pageIndex;
		public GoToPageCommand(IRichEditControl control)
			: base(control) {
		}
		public GoToPageCommand(IRichEditControl control, int pageIndex)
			: base(control) {
			this.PageIndex = pageIndex;
		}
		#region Properties
		public int PageIndex {
			get { return pageIndex; }
			set {
				this.pageIndex = Math.Min(Pages.Count - 1, Math.Max(0, value));
			}
		}
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPage; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_GoToPageDescription; } }
		protected internal override bool TreatStartPositionAsCurrent { get { return false; } }
		protected internal override bool ExtendSelection { get { return false; } }
		protected PageCollection Pages { get { return ActiveView.FormattingController.PageController.Pages; } }
		public override RichEditCommandId Id { get { return RichEditCommandId.GoToPage; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			IValueBasedCommandUIState<int> valueState = state as IValueBasedCommandUIState<int>;
			if (valueState != null)
				PageIndex = valueState.Value - 1;
			base.ForceExecute(state);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			IValueBasedCommandUIState<int> valueState = state as IValueBasedCommandUIState<int>;
			PageBasedRichEditView view = ActiveView as PageBasedRichEditView;
			if (valueState != null && view != null)
				valueState.EditValue = view.CurrentPageIndex + 1;
		}
		public override ICommandUIState CreateDefaultCommandUIState() {
			return new DefaultValueBasedCommandUIState<int>();
		}
		protected internal override DocumentLogPosition ChangePosition(DocumentModelPosition pos) {
			PageCollection pages = ActiveView.FormattingController.PageController.Pages;
			return pages[pageIndex].GetFirstPosition(ActivePieceTable).LogPosition;
		}
	}
	#endregion
}
