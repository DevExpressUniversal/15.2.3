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
using DevExpress.XtraRichEdit;
using System.ComponentModel;
using System.Drawing.Design;
using DevExpress.Snap.Core.Native.Data;
using DevExpress.Snap.Core.Native;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data;
using System.ComponentModel.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using DevExpress.Services.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Fields;
using DevExpress.Snap.Core.Fields;
using DevExpress.Snap.Core.Native.Services;
namespace DevExpress.Snap.Core.Commands {
	[CommandLocalization(Localization.SnapStringId.GroupFieldsCollectionCommand_MenuCaption, Localization.SnapStringId.GroupFieldsCollectionCommand_Description)]
	public class GroupFieldsCollectionCommand : EditListCommandBase, IServiceProvider {
		class DataSourceCollectionProvider : IDataSourceCollectionProvider {
			public object[] GetDataSourceCollection(IServiceProvider serviceProvider) {
				SnapListFieldInfo snapListFieldInfo = (SnapListFieldInfo)serviceProvider.GetService(typeof(SnapListFieldInfo));
				object dataSource = FieldsHelper.GetFieldDesignBinding(snapListFieldInfo.PieceTable.DocumentModel.DataSourceDispatcher, snapListFieldInfo).DataSource;
				return new[] { dataSource };
			}
		}
		class SNListFieldProvider : IParsedInfoProvider {
			readonly SNListField snList;
			public SNListFieldProvider(SNListField snList) {
				this.snList = snList;
			}
			public CalculatedFieldBase GetParsedInfo() {
				return snList;
			}
			public void InvalidateParsedInfo() {
			}
		}
		readonly DataSourceCollectionProvider dataSourceCollectionProvider;
		FieldChanger fieldChanger;
		public GroupFieldsCollectionCommand(IRichEditControl control)
			: base(control) {
			this.dataSourceCollectionProvider = new DataSourceCollectionProvider();
		}
		protected override bool IsEnabled() {
			if (EditedFieldInfo == null)
				return false;
			return SnapObjectModelController.IsBookmarkCorrespondsToGroup(Bookmark);
		}
		public override string ImageName {
			get {
				return "GroupFieldsCollection";
			}
		}
		[Editor("DevExpress.Snap.Extensions.Native.GroupFieldsEditor, " + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
		public List<GroupField> GroupFields {
			get {
				FieldPathInfo info = FieldPathService.FromString(EditedFieldInfo.ParsedInfo.DataSourceName);
				if (info == null)
					return null;
				DesignBinding designBinding = FieldsHelper.GetFieldDesignBinding(DocumentModel.DataSourceDispatcher, EditedFieldInfo);
				List<GroupField> groupFields = new List<GroupField>();
				foreach (var item in info.DataMemberInfo.Items[0].Groups[GroupIndex].GroupFieldInfos) {
					string dataMember = string.IsNullOrEmpty(designBinding.DataMember) ? item.FieldName : string.Format("{0}.{1}", designBinding.DataMember, item.FieldName);
					groupFields.Add(new GroupField(new DesignBinding(designBinding.DataSource, dataMember)));
				}
				return groupFields;
			}
		}
		public int GroupIndex { get; protected set; }
		protected internal override void ExecuteCore() {
			GroupIndex = EditedFieldInfo.ParsedInfo.GetGroupIndex(EditedFieldInfo.PieceTable.DocumentModel, Bookmark.TemplateInterval.TemplateInfo.FirstGroupIndex);
			this.fieldChanger = new FieldChanger(EditedFieldInfo, new SNListFieldProvider(EditedFieldInfo.ParsedInfo));
			((ISnapControl)Control).ShowGroupFieldsForm(this, this);
		}
		public object GetService(Type serviceType) {
			if (serviceType == typeof(IFieldChanger))
				return fieldChanger;
			if (serviceType == typeof(IDataSourceCollectionProvider))
				return dataSourceCollectionProvider;
			if (serviceType == typeof(SnapListFieldInfo))
				return EditedFieldInfo;
			if (serviceType == typeof(IFieldPathService))
				return FieldPathService;
			if(serviceType == typeof(IWin32Window))
				return this.Control;
			return this.DocumentModel.GetService(serviceType);
		}
	}
}
