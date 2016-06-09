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
using DevExpress.Data;
using DevExpress.Data.Browsing.Design;
using DevExpress.Office;
using DevExpress.Snap.Core.Commands;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.XtraRichEdit.Forms;
using DevExpress.XtraRichEdit.Model;
namespace DevExpress.Snap.Core.Forms {
	public class SortingFormControllerBaseParameters : FormControllerParameters {
		internal SortingFormControllerBaseParameters(ISnapControl control)
			: base(control) {
		}
		new public ISnapControl Control { get { return (ISnapControl)base.Control; } }
	}
	public class SortField : ICloneable<SortField> {
		public SortField() {
			DisplayName = FieldName = "";
			SortOrder = ColumnSortOrder.Ascending;
			IsGroupSorting = false;
			GroupIndex = GroupFieldInfoIndex = 0;
		}
		public SortField(string displayName, ColumnSortOrder sortOrder, string fieldName, bool isGroupSorting, int groupIndex, int groupFieldInfoIndex) {
			DisplayName = displayName;
			SortOrder = sortOrder;
			FieldName = fieldName;
			IsGroupSorting = isGroupSorting;
			GroupIndex = groupIndex;
			GroupFieldInfoIndex = groupFieldInfoIndex;
		}
		public string DisplayName { get; set; }
		public ColumnSortOrder SortOrder { get; set; }
		public string FieldName { get; set; }
		public bool IsGroupSorting { get; set; }
		public int GroupIndex { get; set; }
		public int GroupFieldInfoIndex { get; set; }
		public SortField Clone() {
			return new SortField(DisplayName, SortOrder, FieldName, IsGroupSorting, GroupIndex, GroupFieldInfoIndex);
		}
	}
	public class CustomSortFormController : SortingFormControllerBase {
		public CustomSortFormController(SortingFormControllerBaseParameters controllerParameters) : base(controllerParameters) { }
		protected override FieldPathInfo GetFieldPathInfo() {
			FieldPathInfo result;
			if (FieldPathService == null)
				return null;
			InstructionController controller = CreateInstructionControllerByFieldName(ListName);
			if (controller == null)
				return null;
			result = FieldPathService.FromString(controller.GetArgumentAsString(0));
			return result;
		}
		protected override SnapListFieldInfo GetListFieldInfo() {
			return FieldsHelper.GetSelectedSNListField(DocumentModel);
		}
		public override void ApplyChanges() {
			DocumentModel.BeginUpdate();
			try {
				ApplyChangesCore();
				Selection selection = DocumentModel.Selection;
				SnapCaretPosition caretPos = DocumentModel.SelectionInfo.GetSnapCaretPositionFromSelection(selection.PieceTable, selection.NormalizedStart);
				PieceTable.FieldUpdater.UpdateFieldAndNestedFields(ListFieldInfo.Field);
				if(caretPos != null)
					DocumentModel.SelectionInfo.RestoreCaretPosition(caretPos);
				if(!DocumentModel.IsUpdateLocked)
					Control.InnerControl.ActiveView.EnsureCaretVisible();
				else
					DocumentModel.DeferredChanges.EnsureCaretVisible = true;
			}
			finally {
				DocumentModel.EndUpdate();
			}
		}
		void ApplyChangesCore() {
			if(FieldPathInfo == null || ListFieldInfo == null)
				return;
			InstructionController controller = CreateInstructionControllerByFieldName(ListName);
			if(controller == null)
				return;
			if(FieldPath.Items.Count > 0 && FieldPath.Items[0].HasGroups)
				FieldPath.Items[0].Groups.RemoveAll(p => !p.HasGroupTemplates);
			for(int i = GetSortFieldIndex(); i < SortList.Count; i++) {
				GroupProperties groupProperties = new GroupProperties();
				GroupFieldInfo groupInfo = new GroupFieldInfo(SortList[i].FieldName);
				groupInfo.SortOrder = SortList[i].SortOrder;
				groupProperties.GroupFieldInfos.Add(groupInfo);
				FieldPath.AddGroup(groupProperties);
			}
			controller.SuppressFieldsUpdateAfterUpdateInstruction = true;
			controller.SetArgument(0, FieldPathService.GetStringPath(FieldPathInfo));
			controller.ApplyDeferredActions();
			PieceTable.UpdateTemplateByField(controller.Field, false);
		}
		int GetSortFieldIndex() {
			int count = SortList.Count;
			FieldDataMemberInfoItem dataMemberInfo = FieldPath.Items[0];
			for(int i = 0; i < SortList.Count; i++) {
				SortField sortField = SortList[i];
				if(!sortField.IsGroupSorting)
					return i;
				dataMemberInfo.Groups[sortField.GroupIndex].GroupFieldInfos[sortField.GroupFieldInfoIndex].SortOrder = sortField.SortOrder;
			}
			return count;
		}
	}
}
