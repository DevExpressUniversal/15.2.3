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

using DevExpress.XtraRichEdit;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Data;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Snap.Core.Native;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.API;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Commands {
	#region SortFieldCommandBase
	public abstract class SortFieldCommandBase : EditListFromInnerFieldCommandBase {
		ColumnSortOrder? actualSortOrder;
		protected SortFieldCommandBase(IRichEditControl control)
			: base(control) {
		}
		public abstract ColumnSortOrder SortOrder { get; }
		protected abstract ColumnSortOrder InverseSortOrder { get; }
		protected virtual ColumnSortOrder GetActualSortOrder() {
			if (actualSortOrder != null)
				return actualSortOrder.Value;
			GroupFieldInfo groupFieldInfo = GetCorrespondingGroupFieldInfo();
			if (groupFieldInfo == null)
				return ColumnSortOrder.None;
			actualSortOrder = groupFieldInfo.SortOrder;
			return actualSortOrder.Value;
		}
		protected override bool IsChecked() {
			return GetActualSortOrder() == SortOrder;
		}
		protected override void UpdateGroupInfo(GroupProperties groupProperties, GroupFieldInfo groupFieldInfo, GroupModifyInfo modifyInfo) {
			if (GetActualSortOrder() != SortOrder)
				groupFieldInfo.SortOrder = SortOrder;
			else if (HasGroupTemplates)
				groupFieldInfo.SortOrder = InverseSortOrder;
			else
				groupFieldInfo.SortOrder = ColumnSortOrder.None;
		}
	}
	#endregion
	#region SortFieldAscendingCommand
	[CommandLocalization(Localization.SnapStringId.SortFieldAscendingCommand_MenuCaption, Localization.SnapStringId.SortFieldAscendingCommand_Description)]
	public class SortFieldAscendingCommand : SortFieldCommandBase {
		public SortFieldAscendingCommand(IRichEditControl control)
			: base(control) {
		}
		public override ColumnSortOrder SortOrder { get { return ColumnSortOrder.Ascending; } }
		protected override ColumnSortOrder InverseSortOrder { get { return ColumnSortOrder.Descending; } }
		public override string ImageName { get { return "SortAsc"; } }
	}
	#endregion
	#region SortFieldDescendingCommand
	[CommandLocalization(Localization.SnapStringId.SortFieldDescendingCommand_MenuCaption, Localization.SnapStringId.SortFieldDescendingCommand_Description)]
	public class SortFieldDescendingCommand : SortFieldCommandBase {
		public SortFieldDescendingCommand(IRichEditControl control)
			: base(control) {
		}
		public override ColumnSortOrder SortOrder { get { return ColumnSortOrder.Descending; } }
		protected override ColumnSortOrder InverseSortOrder { get { return ColumnSortOrder.Ascending; } }
		public override string ImageName { get { return "SortDesc"; } }
	}
	#endregion
	#region MailMergeSortFieldCommandBase
	public abstract class MailMergeSortFieldCommandBase : MailMergeEditFieldCommandBase {
		ColumnSortOrder? actualSortOrder;
		protected MailMergeSortFieldCommandBase(IRichEditControl control)
			: base(control) {
		}
		public abstract ColumnSortOrder SortOrder { get; }
		protected override bool IsChecked() {
			return GetActualSortOrder() == SortOrder;
		}
		protected override bool IsEnabledCore(DesignBinding binding) {
			return DataAccessService.AllowGroupAndSort(MailMergeVisulaOptions.DataSource, MailMergeVisulaOptions.DataMember, binding.DataMember);
		}
		protected override void ExcecuteCoreInternal() {
			SnapMailMergeVisualOptions options = DocumentModel.SnapMailMergeVisualOptions;
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, EditedFieldInfo);
			if (binding == null || binding.IsNull)
				return;
			SnapListGroupParam sorting = new SnapListGroupParam(binding.DataMember, SortOrder);
			if (GetActualSortOrder() != SortOrder) {
				for (int i = 0; i < options.Sorting.Count; i++) {
					if (options.Sorting[i].FieldName == sorting.FieldName) {
						options.Sorting.RemoveAt(i);
						break;
					}
				}
				options.Sorting.Add(sorting);
			}
			else
				options.Sorting.Remove(sorting);
		}
		protected virtual ColumnSortOrder GetActualSortOrder() {
			if (actualSortOrder != null)
				return actualSortOrder.Value;
			if (EditedFieldInfo == null)
				return ColumnSortOrder.None;
			SnapMailMergeVisualOptions options = DocumentModel.SnapMailMergeVisualOptions;
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DataSourceDispatcher, EditedFieldInfo);
			if (binding == null || binding.IsNull)
				return ColumnSortOrder.None;
			foreach (var item in options.Sorting)
				if (item.FieldName == binding.DataMember) {
					actualSortOrder = item.SortOrder;
					return actualSortOrder.Value;
				}
			return ColumnSortOrder.None;
		}
	}
	#endregion
	#region MailMergeSortFieldAscendingCommand
	[CommandLocalization(Localization.SnapStringId.SortFieldAscendingCommand_MenuCaption, Localization.SnapStringId.SortFieldAscendingCommand_Description)]
	public class MailMergeSortFieldAscendingCommand : MailMergeSortFieldCommandBase {
		public MailMergeSortFieldAscendingCommand(IRichEditControl control)
			: base(control) {
		}
		public override ColumnSortOrder SortOrder { get { return ColumnSortOrder.Ascending; } }
		public override string ImageName { get { return "SortAsc"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.MailMergeSortFieldAscending; } }
	}
	#endregion
	#region MailMergeSortFieldDescendingCommand
	[CommandLocalization(Localization.SnapStringId.SortFieldDescendingCommand_MenuCaption, Localization.SnapStringId.SortFieldDescendingCommand_Description)]
	public class MailMergeSortFieldDescendingCommand : MailMergeSortFieldCommandBase {
		public MailMergeSortFieldDescendingCommand(IRichEditControl control)
			: base(control) {
		}
		public override ColumnSortOrder SortOrder { get { return ColumnSortOrder.Descending; } }
		public override string ImageName { get { return "SortDesc"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.MailMergeSortFieldDescending; } }
	}
	#endregion
	#region SnapSortFieldAscendingCommand
	[CommandLocalization(Localization.SnapStringId.SortFieldAscendingCommand_MenuCaption, Localization.SnapStringId.SortFieldAscendingCommand_Description)]
	public class SnapSortFieldAscendingCommand : SnapMultiCommand {
		public SnapSortFieldAscendingCommand(IRichEditControl control)
			: base(control) {
		}
		public override string ImageName { get { return "SortAsc"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.SnapSortFieldAscending; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		protected internal override void CreateCommands() {
			Commands.Add(new SortFieldAscendingCommand(Control));
			Commands.Add(new MailMergeSortFieldAscendingCommand(Control));
		}
	}
	#endregion
	#region SnapSortFieldDescendingCommand
	[CommandLocalization(Localization.SnapStringId.SortFieldDescendingCommand_MenuCaption, Localization.SnapStringId.SortFieldDescendingCommand_Description)]
	public class SnapSortFieldDescendingCommand : SnapMultiCommand {
		public SnapSortFieldDescendingCommand(IRichEditControl control)
			: base(control) {
		}
		public override string ImageName { get { return "SortDesc"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.SnapSortFieldDescending; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		protected internal override void CreateCommands() {
			Commands.Add(new SortFieldDescendingCommand(Control));
			Commands.Add(new MailMergeSortFieldDescendingCommand(Control));
		}
	}
	#endregion
}
