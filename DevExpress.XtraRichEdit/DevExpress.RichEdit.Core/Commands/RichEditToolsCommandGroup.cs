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
using System.Linq;
using System.Text;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit.Localization;
namespace DevExpress.XtraRichEdit.Commands {
	#region RichEditToolsCommandGroup (abstract class)
	public abstract class RichEditToolsCommandGroup : RichEditCommand {
		protected RichEditToolsCommandGroup(IRichEditControl control)
			: base(control) {
		}
		public override void ForceExecute(ICommandUIState state) {
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			state.Visible = IsContentEditable && IsToolsActive();
		}
		protected abstract bool IsToolsActive();
	}
	#endregion
	#region ToolsFloatingPictureCommandGroup
	public class ToolsFloatingPictureCommandGroup : RichEditToolsCommandGroup {
		public ToolsFloatingPictureCommandGroup(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.ToolsFloatingPictureCommandGroup; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_None; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_None; } }
		#endregion
		protected override bool IsToolsActive() {
			return Control.InnerControl.DocumentModel.Selection.IsFloatingObjectSelected();
		}
	}
	#endregion
	#region ToolsTableCommandGroup
	public class ToolsTableCommandGroup : RichEditToolsCommandGroup {
		public ToolsTableCommandGroup(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.ToolsTableCommandGroup; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_None; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_None; } }
		#endregion
		protected override bool IsToolsActive() {
			return Control.InnerControl.DocumentModel.Selection.IsWholeSelectionInOneTable();
		}
	}
	#endregion
	#region ToolsHeaderFooterCommandGroup
	public class ToolsHeaderFooterCommandGroup : RichEditToolsCommandGroup {
		public ToolsHeaderFooterCommandGroup(IRichEditControl control)
			: base(control) {
		}
		#region Properties
		public override RichEditCommandId Id { get { return RichEditCommandId.ToolsHeaderFooterCommandGroup; } }
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_None; } }
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_None; } }
		#endregion
		protected override bool IsToolsActive() {
			return Control.InnerControl.DocumentModel.Selection.PieceTable.IsHeaderFooter;
		}
	}
	#endregion
}
