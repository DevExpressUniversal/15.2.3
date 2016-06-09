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
using DevExpress.XtraRichEdit;
using DevExpress.Utils.Commands;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Commands {
	#region ToggleShowRulersCommandBase (abstract class)
	public abstract class ToggleShowRulersCommandBase : RichEditMenuItemSimpleCommand {
		protected ToggleShowRulersCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			RulerOptions rulerOptions = GetModifyOptions();
			if (IsRulerVisible(rulerOptions.Visibility))
				rulerOptions.Visibility = RichEditRulerVisibility.Hidden;
			else
				rulerOptions.Visibility = RichEditRulerVisibility.Auto;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = true;
			state.Enabled = ShowRulerByDefault;
			RulerOptions rulerOptions = GetModifyOptions();
			RichEditRulerVisibility visibility = rulerOptions.Visibility;
			state.Checked = IsRulerVisible(visibility);
		}
		protected internal virtual bool IsRulerVisible(RichEditRulerVisibility visibility) {
			return visibility == RichEditRulerVisibility.Visible || (visibility == RichEditRulerVisibility.Auto && ShowRulerByDefault);
		}
		protected internal abstract bool ShowRulerByDefault { get; }
		protected internal abstract RulerOptions GetModifyOptions();
	}
	#endregion
	#region ToggleShowHorizontalRulerCommand
	public class ToggleShowHorizontalRulerCommand : ToggleShowRulersCommandBase {
		public ToggleShowHorizontalRulerCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowHorizontalRulerCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ToggleShowHorizontalRuler; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowHorizontalRulerCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleShowHorizontalRuler; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowHorizontalRulerCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ToggleShowHorizontalRulerDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ToggleShowHorizontalRulerCommandImageName")]
#endif
		public override string ImageName { get { return "RulerHorizontal"; } }
		protected internal override bool ShowRulerByDefault { get { return ActiveView.ShowHorizontalRulerByDefault; } }
		#endregion
		protected internal override RulerOptions GetModifyOptions() {
			return Options.HorizontalRuler;
		}
	}
	#endregion
}
