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
using DevExpress.XtraRichEdit.Localization;
using DevExpress.XtraRichEdit;
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertPageCountFieldCommand
	public class InsertPageCountFieldCommand : InsertFieldCommand {
		public InsertPageCountFieldCommand(IRichEditControl control)
			: base(control) {
			FieldCode = "NUMPAGES";
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageCountFieldCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.InsertPageCountField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageCountFieldCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPageCountField; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageCountFieldCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_InsertPageCountFieldDescription; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageCountFieldCommandImageName")]
#endif
		public override string ImageName { get { return "InsertPageCount"; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageCountFieldCommandMenuCaption")]
#endif
		public override string MenuCaption { get { return XtraRichEditLocalizer.GetString(MenuCaptionStringId); } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("InsertPageCountFieldCommandDescription")]
#endif
		public override string Description { get { return XtraRichEditLocalizer.GetString(DescriptionStringId); } }
		public override void UpdateUIState(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIState(state);
			ApplyCommandRestrictionOnEditableControl(state, Options.DocumentCapabilities.HeadersFooters, state.Enabled);
			if (state.Enabled)
				state.Enabled = ActivePieceTable.IsHeaderFooter &&
					ActiveViewType == RichEditViewType.PrintLayout;
		}
	}
	#endregion
}
