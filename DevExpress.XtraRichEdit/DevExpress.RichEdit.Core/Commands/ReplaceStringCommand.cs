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
using DevExpress.Utils.Commands;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	#region ReplaceCommand
	public class ReplaceCommand : FindAndReplaceStringCommand {
		public ReplaceCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReplaceCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.Replace; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReplaceCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ReplaceDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReplaceCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Replace; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ReplaceCommandImageName")]
#endif
		public override string ImageName { get { return "Replace"; } }
		#endregion
		protected override void ShowForm(string searchString) {
			Control.ShowReplaceForm();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
	}
	#endregion
	#region ReplaceInnerCommand
	public class ReplaceInnerCommand : RichEditMenuItemSimpleCommand {
		public ReplaceInnerCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.InnerReplace; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.Msg_InternalError; } }
		public virtual DocumentLogPosition StartPosition { get; set; }
		public virtual int Length { get; set; }
		public virtual string Replacement { get; set; }
		#endregion
		protected internal override void ExecuteCore() {
			ActivePieceTable.ReplaceText(StartPosition, Length, Replacement);
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = IsContentEditable;
			state.Visible = true;
		}
	}
	#endregion
}
