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
using DevExpress.XtraRichEdit.Fields;
using System.ComponentModel;
using DevExpress.Snap.Core.Native;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.Snap.Extensions.Localization;
using DevExpress.Snap.Core.Fields;
namespace DevExpress.Snap.Extensions.Native.ActionLists {
	public class DataFieldNameActionList : FieldActionList<SNMergeFieldSupportsEmptyFieldDataAlias> {
		public DataFieldNameActionList(SnapFieldInfo fieldInfo, IServiceProvider serviceProvider)
			: base(fieldInfo, serviceProvider) {
		}
		[
		RefreshProperties(RefreshProperties.All),
		]
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_Binding")]
		public DesignBinding Binding {
			get {
				return FieldsHelper.GetFieldDesignBindingEx(FieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, FieldInfo);
			}
			set {
				ApplyNewValue((controller, newDataFieldName) => controller.SetArgument(0, newDataFieldName), GetDataMember(value));
			}
		}
		[
		RefreshProperties(RefreshProperties.All),
		]
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_EmptyFieldDataAlias")]
		public string EmptyFieldDataAlias {
			get {
				return ParsedInfo.EmptyFieldDataAlias;
			}
			set {
				if (string.IsNullOrEmpty(value))
					ApplyNewValue((controller, newMode) => controller.RemoveSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EmptyFieldDataAliasSwitch), string.Empty);
				else if (EmptyFieldDataAlias != value) {
					ApplyNewValue((controller, newText) => controller.SetSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EmptyFieldDataAliasSwitch, newText), value);
				}
			}
		}
		[RefreshProperties(RefreshProperties.All),]
		[ResDisplayName(typeof(ResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapExtensionsStringId.ActionList_EnableEmptyFieldDataAlias")]
		public bool EnableEmptyFieldDataAlias{
			get {
				return ParsedInfo.EnableEmptyFieldDataAlias;
			}
			set{
				ApplyNewValue(ChangeEnableEmptyFieldDataAlias, value);
			}
		}
		void ChangeEnableEmptyFieldDataAlias(InstructionController controller, bool useAlias) {
			if (useAlias)
				controller.SetSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EnableEmptyFieldDataAliasSwitch);
			else
				controller.RemoveSwitch(SNMergeFieldSupportsEmptyFieldDataAlias.EnableEmptyFieldDataAliasSwitch);
		}
		string GetDataMember(DesignBinding designBinding) {
			if(designBinding.DataSource is ParametersDataSource)
				return designBinding.DataMember;
			return DesignBindingHelper.GetDataMember(FieldInfo, designBinding);
		}
		protected override void FillActionItemCollection(ActionItemCollection actionItems) {
			base.FillActionItemCollection(actionItems);
			AddPropertyItem(actionItems, "DataFieldName", "DataFieldName");
			AddPropertyItem(actionItems, "Binding", "Binding");
			AddPropertyItem(actionItems, "EmptyFieldDataAlias", "EmptyFieldDataAlias");
			AddPropertyItem(actionItems, "EnableEmptyFieldDataAlias", "EnableEmptyFieldDataAlias");
		}
	}
}
