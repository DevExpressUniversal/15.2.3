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
using System;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native;
using DevExpress.Utils;
using DevExpress.Data.Browsing.Design;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Core.Commands {
	public abstract class FilterListCommandBase : EditListCommandBase {
		string filterString;
		protected FilterListCommandBase(IRichEditControl control)
			: base(control) {
		}
		protected internal override void ExecuteCore() {
			if (EditedFieldInfo == null)
				return;
			DesignBinding binding = FieldsHelper.GetFieldDesignBinding(DocumentModel.DataSourceDispatcher, EditedFieldInfo);
			IFilterStringAccessService filterStringAccessService = this.DocumentModel.GetService<IFilterStringAccessService>();
			this.filterString = filterStringAccessService != null ? filterStringAccessService.GetFilterString(EditedFieldInfo.ParsedInfo) : null;
			if (ModifyFilterString(binding, ref filterString))
				base.ExecuteCore();
		}
		protected internal override void UpdateFieldCode(XtraRichEdit.Fields.InstructionController controller) {
			string argument = controller.GetArgumentAsString(0);
			FieldPathInfo info = FieldPathService.FromString(argument);
			if (info.DataMemberInfo.IsEmpty)
				info.DataMemberInfo.Items.Add(new FieldDataMemberInfoItem(String.Empty));
			if (info.DataMemberInfo.LastItem.HasFilters)
				info.DataMemberInfo.LastItem.FilterProperties.Filters.Clear();
			info.DataMemberInfo.LastItem.AddFilter(this.filterString);
			controller.SetArgument(0, FieldPathService.GetStringPath(info));
		}
		protected abstract bool ModifyFilterString(DesignBinding binding, ref string filterString);
	}
	[CommandLocalization(Localization.SnapStringId.FilterListCommand_MenuCaption, Localization.SnapStringId.FilterListCommand_Description)]
	public class FilterListCommand : FilterListCommandBase {
		public FilterListCommand(IRichEditControl control)
			: base(control) {
		}
		public override XtraRichEdit.Commands.RichEditCommandId Id { get { return SnapCommandId.FilterList; } }
		protected override bool TryToKeepFieldSelection { get { return true; } }
		protected override bool ModifyFilterString(DesignBinding binding, ref string filterString) {
			return ((ISnapControl)Control).ShowFilterStringEditorForm(binding, ref filterString);
		}
		public override string ImageName {
			get {
				return "Filter";
			}
		}
	}
}
