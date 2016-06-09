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
using DevExpress.Snap.Core.Native;
using DevExpress.Data.Filtering;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraRichEdit.Commands;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.FilterFieldCommand_MenuCaption, Localization.SnapStringId.FilterFieldCommand_Description)]
	public class SnapFilterFieldCommand : SnapMultiCommand {
		public SnapFilterFieldCommand(IRichEditControl control)
			: base(control) {
		}
		public SnapFilterFieldCommand(IRichEditControl control, string dataFieldName, string[] values)
			: base(control, false) {
			CreateCommands(dataFieldName, values);
		}
		public SnapFilterFieldCommand(IRichEditControl control, string dataFieldName, CriteriaOperator filterCriteria)
			: base(control, false) {
			CreateCommands(dataFieldName, filterCriteria);
		}
		public override string ImageName { get { return "QuickFilter"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.SnapFilterField; } }
		protected internal override MultiCommandExecutionMode ExecutionMode { get { return MultiCommandExecutionMode.ExecuteFirstAvailable; } }
		protected internal override MultiCommandUpdateUIStateMode UpdateUIStateMode { get { return MultiCommandUpdateUIStateMode.EnableIfAnyAvailable; } }
		protected internal override void CreateCommands() {
			Commands.Add(new FilterFieldCommand(Control));
			Commands.Add(new MailMergeFilterFieldCommand(Control));
		}
		protected internal virtual void CreateCommands(string dataFieldName, string[] values) {
			Commands.Add(new FilterFieldCommand(Control, dataFieldName, values));
			Commands.Add(new MailMergeFilterFieldCommand(Control, dataFieldName, values));
		}
		protected internal virtual void CreateCommands(string dataFieldName, CriteriaOperator filterCriteria) {
			Commands.Add(new FilterFieldCommand(Control, dataFieldName, filterCriteria));
			Commands.Add(new MailMergeFilterFieldCommand(Control, dataFieldName, filterCriteria));
		}
	}
	[CommandLocalization(Localization.SnapStringId.FilterFieldCommand_MenuCaption, Localization.SnapStringId.FilterFieldCommand_Description)]
	public class FilterFieldCommand : FilterListCommandBase {
		readonly string dataFieldName; 
		readonly string[] values;
		readonly CriteriaOperator filterCriteria;
		readonly IDataAccessService dataAccessService;
		public FilterFieldCommand(IRichEditControl control) : base(control) {
			this.dataAccessService = this.Control.GetService(typeof(IDataAccessService)) as IDataAccessService;
		}
		public FilterFieldCommand(IRichEditControl control, string dataFieldName) 
			: this(control) {
			this.dataFieldName = dataFieldName;
		}
		public FilterFieldCommand(IRichEditControl control, string dataFieldName, string[] values)
			: this(control, dataFieldName) {
			this.values = values;
		}
		public FilterFieldCommand(IRichEditControl control, string dataFieldName, CriteriaOperator filterCriteria)
			: this(control, dataFieldName) {
			this.filterCriteria = filterCriteria;
		}
		public override string ImageName { get { return "Filter"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.ListHeader; } }
		protected override bool IsEnabled() {
			if (!base.IsEnabled() || dataAccessService == null)
				return false;
			SNDataInfo dataInfo = GetDataInfo();
			if (dataInfo == null)
				return false;
			DataMemberInfo dataMemberInfo = DataMemberInfo.Create(dataInfo.DataPaths);
			return dataAccessService.AllowFilter(dataInfo.Source, dataMemberInfo.ParentDataMemberInfo.DataMember, dataMemberInfo.ColumnName);
		}
		protected override bool ModifyFilterString(DesignBinding binding, ref string filterString) {
			CriteriaOperator criteriaOperator = values != null ? FilterStringHelper.ModifyFilterString(filterString, values, dataFieldName) : FilterStringHelper.ModifyFilterString(filterString, filterCriteria, dataFieldName);
			filterString = CriteriaOperator.ToString(criteriaOperator);
			return true;
		}
	}
}
