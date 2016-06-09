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
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowRangeEditingPermissionsFormCommand
	public class ShowRangeEditingPermissionsFormCommand : RichEditMenuItemSimpleCommand {
		public ShowRangeEditingPermissionsFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowRangeEditingPermissionsFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowRangeEditingPermissionsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowRangeEditingPermissionsFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowRangeEditingPermissionsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowRangeEditingPermissionsFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowRangeEditingPermissionsFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowRangeEditingPermissionsFormCommandImageName")]
#endif
		public override string ImageName { get { return "EditRangePermission"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowRangeEditingPermissionsFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		protected internal override void ExecuteCore() {
			Control.ShowRangeEditingPermissionsForm();
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Enabled = !DocumentModel.ProtectionProperties.EnforceProtection;
		}
	}
	#endregion
}
