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
using DevExpress.XtraRichEdit.Model;
using DevExpress.Utils;
using DevExpress.XtraRichEdit;
using System.Collections.Generic;
using System.ComponentModel;
namespace DevExpress.XtraRichEdit.Commands {
	#region UpdateFieldsCommand
	public class UpdateFieldsCommand : SelectionBasedPropertyChangeCommandBase {
		Field firstField;
		Field lastField;
		public UpdateFieldsCommand(IRichEditControl control)
			: base(control) {
		}
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldsCommandId")]
#endif
		public override RichEditCommandId Id { get { return RichEditCommandId.UpdateFields; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldsCommandMenuCaptionStringId")]
#endif
		public override XtraRichEditStringId MenuCaptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateFields; } }
#if !SL
	[DevExpressRichEditCoreLocalizedDescription("UpdateFieldsCommandDescriptionStringId")]
#endif
		public override XtraRichEditStringId DescriptionStringId { get { return XtraRichEditStringId.MenuCmd_UpdateFieldsDescription; } }
		public override string ImageName { get { return "UpdateField"; } }
		protected internal override DocumentModelChangeActions ChangeProperty(DocumentModelPosition start, DocumentModelPosition end, ICommandUIState state) {
			return DocumentModelChangeActions.None;
		}
		protected internal override void ModifyDocumentModelCore(ICommandUIState state) {
			List<Field> fieldsToUpdate = new List<Field>();
			for (int i = firstField.Index; i <= lastField.Index; i++)
				if (ActivePieceTable.Fields[i].Parent == null)
					fieldsToUpdate.Add(ActivePieceTable.Fields[i]);
			ActivePieceTable.FieldUpdater.UpdateFields(fieldsToUpdate, UpdateFieldOperationType.Normal);
		}	 
		Field FindFirstField(RunIndex runIndex) {
			int index = ActivePieceTable.FindFieldIndexByRunIndexCore(runIndex);
			if (index < 0)
				return null;
			return ActivePieceTable.Fields[index].GetTopLevelParent();
		}
		Field FindLastField(RunIndex runIndex) {
			int index = FindLastFieldIndex(runIndex);
			if (index < 0)
				return null;
			return ActivePieceTable.Fields[index].GetTopLevelParent();
		}
		int FindLastFieldIndex(RunIndex runIndex) {
			int count = ActivePieceTable.Fields.Count;
			FieldRunIndexComparable comparator = new FieldRunIndexComparable(runIndex);
			int index = Algorithms.BinarySearch(ActivePieceTable.Fields, comparator);
			if (index < 0) {
				index = ~index;
				if (index >= count)
					index = count;
				return index - 1;
			}
			else
				return index;
		}
		protected override void UpdateUIStateCore(ICommandUIState state) {
			CheckExecutedAtUIThread();
			state.Enabled = IsContentEditable;
			if (state.Enabled) {
				ValidateFields();
				if (firstField == null || lastField == null)
					state.Enabled = false;
			}
			state.Visible = true;
		}
		protected virtual void ValidateFields() {
			RunInfo runInfo = ActivePieceTable.FindRunInfo(DocumentModel.Selection.NormalizedStart, Math.Max(1, DocumentModel.Selection.Length));
			firstField = FindFirstField(runInfo.Start.RunIndex);
			if (firstField == null)
				return;
			lastField = FindLastField(runInfo.End.RunIndex);
			if (lastField == null)
				lastField = firstField;			
			bool intersect = firstField.FirstRunIndex <= runInfo.End.RunIndex &&runInfo.Start.RunIndex <= lastField.LastRunIndex;
			if(!intersect) {
				firstField = null;
				lastField = null;
			}
		}
	}
	#endregion
}
