﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Native.Templates;
namespace DevExpress.Snap.Core.Commands {
	public class SnapInsertTableColumnToTheLeftCommand : InsertTableColumnToTheLeftCommand {
		public SnapInsertTableColumnToTheLeftCommand(IRichEditControl control)
			: base(control) {
		}
		protected override InsertTableColumnCoreCommandBase CreateInsertTableColumnCommand(IRichEditControl control) {
			return new SnapInsertTableColumnToTheLeftCoreCommand(control);
		}
	}
	public class SnapInsertTableColumnToTheRightCommand : InsertTableColumnToTheRightCommand {
		public SnapInsertTableColumnToTheRightCommand(IRichEditControl control)
			: base(control) {
		}
		protected override InsertTableColumnCoreCommandBase CreateInsertTableColumnCommand(IRichEditControl control) {
			return new SnapInsertTableColumnToTheRightCoreCommand(control);
		}
	}
	public class SnapInsertTableColumnToTheLeftCoreCommand : InsertTableColumnToTheLeftCoreCommand {
		public SnapInsertTableColumnToTheLeftCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void PerformModifyModel() {
			((SnapDocumentModel)DocumentModel).SelectionInfo.PerformModifyModelBySelection(base.PerformModifyModel);
			EnsureCaretVisible();
		}
		protected internal override void ModifyModel() {
			TableCell cell = InsertColumn(PatternCell);
			InsertColumnCommandTemplateUpdater rowUpdater = new InsertColumnCommandTemplateUpdater(cell, false);
			rowUpdater.UpdateTemplates();
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
	}
	public class SnapInsertTableColumnToTheRightCoreCommand : InsertTableColumnToTheRightCoreCommand {
		public SnapInsertTableColumnToTheRightCoreCommand(IRichEditControl control)
			: base(control) {
		}
		protected internal override void PerformModifyModel() {
			((SnapDocumentModel)DocumentModel).SelectionInfo.PerformModifyModelBySelection(base.PerformModifyModel);
			EnsureCaretVisible();
		}
		protected internal override void ModifyModel() {
			TableCell cell = InsertColumn(PatternCell);
			InsertColumnCommandTemplateUpdater rowUpdater = new InsertColumnCommandTemplateUpdater(cell, true);
			rowUpdater.UpdateTemplates();
#if DEBUGTEST
			DocumentModel.ActivePieceTable.ForceCheckTablesIntegrity();
#endif
		}
	}
}
