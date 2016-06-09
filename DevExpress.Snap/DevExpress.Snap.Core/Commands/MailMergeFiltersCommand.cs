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
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Data.Filtering;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.MailMergeFilters_MenuCaption, Localization.SnapStringId.MailMergeFilters_Description)]
	public class MailMergeFiltersCommand : SnapMailMergeCommandBase {
		public MailMergeFiltersCommand(IRichEditControl control) : base(control) { }
		public override XtraRichEdit.Commands.RichEditCommandId Id { get { return SnapCommandId.MailMergeFilters; } }
		public override string ImageName { get { return "Filter"; } }
		protected override bool IfCheckedCore(SnapMailMergeVisualOptions options) {
			return !string.IsNullOrEmpty(options.FilterString);
		}
		protected internal override void ExecuteCore() {
			SnapMailMergeVisualOptions mailMergeOptions = DocumentModel.SnapMailMergeVisualOptions;
			string filter = mailMergeOptions.FilterString;
			if (((ISnapControl)Control).ShowFilterStringEditorForm(new DesignBinding(mailMergeOptions.DataSource, mailMergeOptions.DataMember), ref filter))
				mailMergeOptions.FilterString = filter;
		}
	}
	[CommandLocalization(Localization.SnapStringId.FilterFieldCommand_MenuCaption, Localization.SnapStringId.FilterFieldCommand_Description)]
	public class MailMergeFilterFieldCommand : MailMergeEditFieldCommandBase {
		readonly string dataFieldName;
		readonly string[] values;
		readonly CriteriaOperator filterCriteria;
		public MailMergeFilterFieldCommand(IRichEditControl control) : base(control) {
		}
		public MailMergeFilterFieldCommand(IRichEditControl control, string dataFieldName)
			: this(control) {
			this.dataFieldName = dataFieldName;
		}
		public MailMergeFilterFieldCommand(IRichEditControl control, string dataFieldName, string[] values)
			: this(control, dataFieldName) {
			this.values = values;
		}
		public MailMergeFilterFieldCommand(IRichEditControl control, string dataFieldName, CriteriaOperator filterCriteria)
			: this(control, dataFieldName) {
			this.filterCriteria = filterCriteria;
		}
		public override string ImageName { get { return "Filter"; } }
		public override RichEditCommandId Id { get { return SnapCommandId.MailMergeFilterField; } }
		protected override bool IsEnabledCore(DesignBinding binding) {
			return DataAccessService.AllowFilter(MailMergeVisulaOptions.DataSource, MailMergeVisulaOptions.DataMember, binding.DataMember);
		}
		protected override void ExcecuteCoreInternal() {
			string filterString = DocumentModel.SnapMailMergeVisualOptions.FilterString;
			CriteriaOperator criteriaOperator = values != null ? FilterStringHelper.ModifyFilterString(filterString, values, dataFieldName) : FilterStringHelper.ModifyFilterString(filterString, filterCriteria, dataFieldName);
			DocumentModel.SnapMailMergeVisualOptions.FilterString = CriteriaOperator.ToString(criteriaOperator);
		}
	}
}
