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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Model;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region ShowNumberingListFormCommand
	public class ShowNumberingListFormCommand : SelectionBasedCommandBase {
		public ShowNumberingListFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowNumberingListFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowNumberingListForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowNumberingListFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowNumberingList; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowNumberingListFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowNumberingListDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowNumberingListFormCommandImageName")]
#endif
		public override string ImageName { get { return "ListBullets"; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				Control.ShowNumberingListForm(DocumentModel.Selection.GetSelectedParagraphs(), ShowNumberingListFormCallback, null);
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected internal virtual void ShowNumberingListFormCallback(ParagraphList paragraphs, object data) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NumberingOptions options = DocumentModel.DocumentCapabilities.Numbering;
			bool enabled = options.MultiLevelAllowed || options.BulletedAllowed || options.SimpleAllowed;
			state.Enabled = IsContentEditable && enabled;
			state.Visible = Control.CanShowNumberingListForm && (options.Bulleted != DocumentCapability.Hidden
				|| options.MultiLevel != DocumentCapability.Hidden
				|| options.Simple != DocumentCapability.Hidden);
			ApplyDocumentProtectionToSelectedParagraphs(state);
		}
	}
	#endregion
}
