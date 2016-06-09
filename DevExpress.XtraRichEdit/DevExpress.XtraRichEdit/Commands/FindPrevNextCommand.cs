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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Forms;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region FindPrevNextCommand (abstract class)
	public abstract class FindPrevNextCommand : RichEditMenuItemSimpleCommand {
		protected FindPrevNextCommand(IRichEditControl control)
			: base(control) {
		}
		protected SearchParameters SearchParameters { get { return DocumentModel.SearchParameters; } }
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !String.IsNullOrEmpty(SearchParameters.SearchString);
			state.Visible = true;
		}
	}
	#endregion
	#region FindNextCommand
	public class FindNextCommand : FindPrevNextCommand {
		public FindNextCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FindNextCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.FindNext; } }
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FindNextCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FindNext; } }
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FindNextCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FindNextDescription; } }
		protected internal override void ExecuteCore() {
			using (SearchHelper searchHelper = new SearchHelper(Control)) {
				searchHelper.Direction = TextSearchDirection.Down;
				searchHelper.ExecuteFindCommand();
			}
		}
	}
	#endregion
	#region FindPrevCommand
	public class FindPrevCommand : FindPrevNextCommand {
		public FindPrevCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FindPrevCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.FindPrev; } }
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FindPrevCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_FindPrev; } }
#if !SL
	[DevExpressXtraRichEditLocalizedDescription("FindPrevCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_FindPrevDescription; } }
		protected internal override void ExecuteCore() {
			using (SearchHelper searchHelper = new SearchHelper(Control)) {
				searchHelper.Direction = TextSearchDirection.Up;
				searchHelper.ExecuteFindCommand();
			}
		}
	}
	#endregion
}
