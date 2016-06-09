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

using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region SwitchActiveViewCommand (abstract class)
	public abstract class SwitchActiveViewCommand : RichEditMenuItemSimpleCommand {
		protected SwitchActiveViewCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal abstract RichEditViewType ViewType { get; }
		protected internal override void ExecuteCore() {
			InnerControl.ActiveViewType = ViewType;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Checked = ActiveViewType == ViewType;
			state.Enabled = true;
			state.Visible = true;
		}
	}
	#endregion
	#region SwitchToDraftViewCommand
	public class SwitchToDraftViewCommand : SwitchActiveViewCommand {
		public SwitchToDraftViewCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override RichEditViewType ViewType { get { return RichEditViewType.Draft; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToDraftViewCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.SwitchToDraftView; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToDraftViewCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SwitchToDraftView; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToDraftViewCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SwitchToDraftViewDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToDraftViewCommandImageName")]
#endif
		public override string ImageName { get { return "DraftView"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region SwitchToPrintLayoutViewCommand
	public class SwitchToPrintLayoutViewCommand : SwitchActiveViewCommand {
		public SwitchToPrintLayoutViewCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override RichEditViewType ViewType { get { return RichEditViewType.PrintLayout; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToPrintLayoutViewCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.SwitchToPrintLayoutView; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToPrintLayoutViewCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SwitchToPrintLayoutView; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToPrintLayoutViewCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SwitchToPrintLayoutViewDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToPrintLayoutViewCommandImageName")]
#endif
		public override string ImageName { get { return "PrintLayoutView"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
	#region SwitchToSimpleViewCommand
	public class SwitchToSimpleViewCommand : SwitchActiveViewCommand {
		public SwitchToSimpleViewCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		protected internal override RichEditViewType ViewType { get { return RichEditViewType.Simple; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToSimpleViewCommandId")]
#endif
public override RichEditCommandId Id { get { return RichEditCommandId.SwitchToSimpleView; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToSimpleViewCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_SwitchToSimpleView; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToSimpleViewCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_SwitchToSimpleViewDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("SwitchToSimpleViewCommandImageName")]
#endif
		public override string ImageName { get { return "SimpleView"; } }
		#endregion
		protected override void UpdateUIStateCore(ICommandUIState state) {
			base.UpdateUIStateCore(state);
			if (!Control.InnerControl.DocumentModel.ActivePieceTable.CanContainCompositeContent())
				state.Enabled = false;
		}
	}
	#endregion
}
