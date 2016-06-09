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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region PrintCommand
	public class PrintCommand : RichEditMenuItemSimpleCommand {
		public PrintCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_Print; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_PrintDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintCommandImageName")]
#endif
		public override string ImageName { get { return "PrintDialog"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.Print; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("PrintCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		protected override bool ShouldBeExecutedOnKeyUpInSilverlightEnvironment { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.ShowPrintDialog();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			ApplyCommandsRestriction(state, Options.Behavior.Printing, Control.IsPrintingAvailable);
		}
	}
	#endregion
}
