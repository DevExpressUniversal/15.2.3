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
using System.Drawing.Design;
using DevExpress.Snap.Core.Native.Data;
using System.Collections.ObjectModel;
using DevExpress.Utils.UI;
using DevExpress.Snap.Extensions.Native.ActionLists;
using DevExpress.Snap.Core.Native;
using DevExpress.Data.Browsing.Design;
using System.Windows.Forms.Design;
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.Snap.Core.Fields;
using System.Collections;
using DevExpress.Data;
using DevExpress.Snap.Core.Commands;
namespace DevExpress.Snap.Extensions.Native {
	public class GroupFieldsEditor : CollectionEditor {
		public GroupFieldsEditor()
			: base(typeof(List<GroupField>)) {
		}
		protected internal override object CreateInstance(Type itemType) {
			SnapListFieldInfo snapFieldInfo = (SnapListFieldInfo)this.Context.GetService(typeof(SnapListFieldInfo));
			object dataSource = FieldsHelper.GetFieldDesignBinding(snapFieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, snapFieldInfo).DataSource;
			return new GroupField(new DesignBinding(dataSource, string.Empty));
		}
		protected internal override bool CanRemoveInstance(object item, int count) {
			return count > 1;
		}
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			this.currentContext = context;
			IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			CollectionEditorFormBase form = CreateCollectionForm(provider);
			form.EditValue = value;
			if (form.ShowEditorDialog(edSvc) == DialogResult.OK) {
				IFieldPathService fieldPathService = (IFieldPathService)provider.GetService(typeof(IFieldPathService));
				SnapListFieldInfo listFieldInfo = (SnapListFieldInfo)provider.GetService(typeof(SnapListFieldInfo));
				SNListField listField = listFieldInfo.ParsedInfo;
				FieldPathInfo info = fieldPathService.FromString(listField.DataSourceName);
				if (info == null)
					return null;
				int groupIndex = ((GroupFieldsCollectionCommand)context.Instance).GroupIndex;
				GroupProperties groupProperties = info.DataMemberInfo.Items[0].Groups[groupIndex];
				IList editValue = (IList)form.EditValue;
				groupProperties.GroupFieldInfos.Clear();
				foreach (GroupField item in editValue) {
					string fieldName = DesignBindingHelper.GetDataMember(listFieldInfo, item.FieldName);
					groupProperties.GroupFieldInfos.Add(new GroupFieldInfo() { FieldName = fieldName, SortOrder = (ColumnSortOrder)item.SortOrder });
				}
				string argument = fieldPathService.GetStringPath(info);
				IFieldChanger fieldChanger = (IFieldChanger)provider.GetService(typeof(IFieldChanger));
				fieldChanger.ApplyNewValue((controller, newMode) => controller.SetArgument(0, newMode), argument);
			}
			return null;
		}
	}
}
