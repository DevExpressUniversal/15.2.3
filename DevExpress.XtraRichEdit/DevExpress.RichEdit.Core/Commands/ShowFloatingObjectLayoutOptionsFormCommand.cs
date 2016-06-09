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
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using System.Collections.Generic;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.SpellChecker;
using System.ComponentModel;
using DevExpress.XtraRichEdit.Model.History;
using DevExpress.XtraRichEdit.Layout;
using DevExpress.Office;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Internal.PrintLayout;
using System.Drawing;
namespace DevExpress.XtraRichEdit.Commands {
	public class ShowFloatingObjectLayoutOptionsFormCommand : SelectionBasedCommandBase {
		private enum ObjectType {
			FloatingObject = 0,
			InlineObject,
			None
		}
		ObjectType isFloatingObject = ObjectType.None;
		public ShowFloatingObjectLayoutOptionsFormCommand(IRichEditControl control)
			: base(control) {
		}
		#region Properties
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFloatingObjectLayoutOptionsFormCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.ShowFloatingObjectLayoutOptionsForm; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFloatingObjectLayoutOptionsFormCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowFloatingObjectLayoutOptionsForm; }}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFloatingObjectLayoutOptionsFormCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_ShowFloatingObjectLayoutOptionsFormDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFloatingObjectLayoutOptionsFormCommandImageName")]
#endif
		public override string ImageName { get { return "FloatingObjectLayoutOptions"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("ShowFloatingObjectLayoutOptionsFormCommandShowsModalDialog")]
#endif
		public override bool ShowsModalDialog { get { return true; } }
		#endregion
		public override void ForceExecute(ICommandUIState state) {
			CheckExecutedAtUIThread();
			NotifyBeginCommandExecution(state);
			try {
				if (isFloatingObject == ObjectType.FloatingObject) {
					FloatingObjectAnchorRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as FloatingObjectAnchorRun;
					Control.ShowFloatingInlineObjectLayoutOptionsForm(new FloatingInlineObjectParameters(run), ShowFloatingInlineObjectLayoutOptionsFormCallback, null);
				}
				else if (isFloatingObject == ObjectType.InlineObject) {
					InlinePictureRun run = ActivePieceTable.Runs[DocumentModel.Selection.Interval.NormalizedStart.RunIndex] as InlinePictureRun;
					Control.ShowFloatingInlineObjectLayoutOptionsForm(new FloatingInlineObjectParameters(run), ShowFloatingInlineObjectLayoutOptionsFormCallback, null);
				}
			}
			finally {
				NotifyEndCommandExecution(state);
			}
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			if (DocumentModel.Selection.IsFloatingObjectSelected())
				isFloatingObject = ObjectType.FloatingObject;
			else if (DocumentModel.Selection.IsInlinePictureSelected())
				isFloatingObject = ObjectType.InlineObject;
			else
				isFloatingObject = ObjectType.None;
			bool needEnable = IsContentEditable && (isFloatingObject != ObjectType.None);
			state.Enabled = needEnable;
			state.Visible = needEnable;
			ApplyDocumentProtectionToSelectedCharacters(state);
		}
		protected internal virtual void ShowFloatingInlineObjectLayoutOptionsFormCallback(FloatingInlineObjectParameters parameters, object data) {
		}
	}
}
