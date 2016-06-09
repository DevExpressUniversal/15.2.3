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

using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Localization;
using System.ComponentModel;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Commands {
	#region SnapInsertTableOfContentsCommand
	public class SnapInsertTableOfContentsCommand : InsertTableOfContentsCommand {
		public SnapInsertTableOfContentsCommand(IRichEditControl control)
			: base(control) {
		}
		public override void UpdateUIState(Utils.Commands.ICommandUIState state) {
			base.UpdateUIState(state);
			SnapMailMergeVisualOptions options = ((SnapDocumentModel)DocumentModel).SnapMailMergeVisualOptions;
			state.Enabled &= !options.IsMailMergeEnabled;
		}
	}
	#endregion
	#region SnapUpdateTableOfContentsCommand
	public class SnapUpdateTableOfContentsCommand : UpdateTableOfContentsCommand {
		public SnapUpdateTableOfContentsCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			SnapMailMergeVisualOptions options = ((SnapDocumentModel)DocumentModel).SnapMailMergeVisualOptions;
			state.Enabled &= !options.IsMailMergeEnabled;
		}
	}
	#endregion
	#region SnapUpdateTableOfFiguresCommand
	public class SnapUpdateTableOfFiguresCommand : SnapUpdateTableOfContentsCommand {
		public SnapUpdateTableOfFiguresCommand(IRichEditControl control)
			: base(control) { }
		public override RichEditCommandId Id { get { return RichEditCommandId.UpdateTableOfFigures; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateTableOfFigures; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateTableOfFiguresDescription; } }
	}
	#endregion
	#region SnapInsertTableOfFiguresPlaceholderCommand
	public class SnapInsertTableOfFiguresPlaceholderCommand : InsertTableOfFiguresPlaceholderCommand {
		public SnapInsertTableOfFiguresPlaceholderCommand(IRichEditControl control)
			: base(control) {
		}
		protected override void UpdateUIStateCore(DevExpress.Utils.Commands.ICommandUIState state) {
			base.UpdateUIStateCore(state);
			SnapMailMergeVisualOptions options = ((SnapDocumentModel)DocumentModel).SnapMailMergeVisualOptions;
			state.Enabled &= !options.IsMailMergeEnabled;
		}
	}
	#endregion
}
