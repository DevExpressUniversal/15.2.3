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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
namespace DevExpress.XtraRichEdit.Commands {
	#region ViewCommentsCommand
	public class ViewCommentsCommand : RichEditMenuItemSimpleCommand {
		public ViewCommentsCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.ViewComments; } }
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_Comment; } }
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_CommentDescription; } }
		public override string ImageName { get { return "ShowComments"; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (Control.InnerControl.ActiveView.DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible) {
				Control.InnerControl.Options.Comments.BeginUpdate();
				Control.InnerControl.Options.Comments.ShowAllAuthors = false;
				Control.InnerControl.Options.Comments.Visibility = RichEditCommentVisibility.Hidden;
				Control.InnerControl.Options.Comments.EndUpdate();
			}
			else {
				Control.InnerControl.Options.Comments.BeginUpdate();
				Control.InnerControl.Options.Comments.ShowAllAuthors = true;
				Control.InnerControl.Options.Comments.Visibility = RichEditCommentVisibility.Visible;
				Control.InnerControl.Options.Comments.EndUpdate();
			}
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			if (Control.InnerControl.ActiveView.DocumentModel.CommentOptions.Visibility == RichEditCommentVisibility.Visible)
				state.Checked = true;
			else
				state.Checked = false;
		}
	}
	#endregion
	#region ReviewersCommand
	public class ReviewersCommand : RichEditMenuItemSimpleCommand {
		public ReviewersCommand(IRichEditControl control)
			: base(control) { 
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReviewersCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.Reviewers; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReviewersCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_Reviewers; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReviewersCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_ReviewersDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReviewersCommandImageName")]
#endif
		public override string ImageName { get { return "Reviewers"; } }
		#endregion
		protected internal override void ExecuteCore() { }
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) { }
	}
	#endregion
	#region ShowReviewingPaneCommand
	public class ShowReviewingPaneCommand : RichEditMenuItemSimpleCommand {
		public ShowReviewingPaneCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowReviewingPaneCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowReviewingPane; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowReviewingPaneCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_ReviewingPane; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowReviewingPaneCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_ReviewingPaneDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowReviewingPaneCommandImageName")]
#endif
		public override string ImageName { get { return "ReviewingPane"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowReviewingPaneCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.ShowReviewingPaneForm(DocumentModel, null, false);
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			state.Visible = true;
			state.Checked = Control.IsVisibleReviewingPane();
		}
	}
	#endregion   
	#region ToggleReviewingPaneCommand
	public class ToggleReviewingPaneCommand : RichEditMenuItemSimpleCommand {
		public ToggleReviewingPaneCommand(IRichEditControl control)
			: base(control) {
		}
		#region Property
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleReviewingPaneCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleReviewingPane; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleReviewingPaneCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_ReviewingPane; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleReviewingPaneCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_ReviewingPaneDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleReviewingPaneCommandImageName")]
#endif
		public override string ImageName { get { return "ReviewingPane"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleReviewingPaneCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			if (Control.IsVisibleReviewingPane())
				Control.CloseReviewingPaneForm();
			else
				Control.ShowReviewingPaneForm(DocumentModel, null, false);
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			state.Visible = true;
			state.Checked = Control.IsVisibleReviewingPane();
		}
	}
	#endregion   
}
