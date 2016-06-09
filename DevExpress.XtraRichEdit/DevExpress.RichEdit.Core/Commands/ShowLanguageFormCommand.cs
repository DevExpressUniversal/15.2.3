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
using System.ComponentModel;
using System.Linq;
using System.Text;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.XtraRichEdit.Commands {
	public class ShowLanguageFormCommand : RichEditSelectionCommand {	 
		public ShowLanguageFormCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLanguageFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowLanguageForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLanguageFormCommandMenuCaptionStringId")]
#endif
		public override Localization.XtraRichEditStringId MenuCaptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_Language; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLanguageFormCommandDescriptionStringId")]
#endif
		public override Localization.XtraRichEditStringId DescriptionStringId { get { return Localization.XtraRichEditStringId.MenuCmd_LanguageDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowLanguageFormCommandImageName")]
#endif
		public override string ImageName { get { return "Language"; } }
		protected internal override void ExecuteCore() {
			Control.ShowLanguageForm(DocumentModel);
		}
		public override void UpdateUIState(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIState(state);
		}
		protected internal override bool CanChangePosition(Model.DocumentModelPosition pos) {
			return true;
		}
		protected internal override bool TryToKeepCaretX {
			get { return false; }
		}
		protected internal override bool TreatStartPositionAsCurrent {
			get { return false; }
		}
		protected internal override bool ExtendSelection {
			get { return false; }
		}
		protected internal override Layout.DocumentLayoutDetailsLevel UpdateCaretPositionBeforeChangeSelectionDetailsLevel {
			get { return Layout.DocumentLayoutDetailsLevel.None; }
		}
		protected internal override Model.DocumentLogPosition ChangePosition(Model.DocumentModelPosition pos) {
			return pos.LogPosition;
		}
	}
}
