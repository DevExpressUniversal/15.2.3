#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.XtraReports.UI;
using DevExpress.Data;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraReports.Extensions;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraReports.Parameters;
namespace DevExpress.ExpressApp.Reports {
	public class CreateCustomReportDesignRepositoryItemEventArgs : HandledEventArgs {
		public CreateCustomReportDesignRepositoryItemEventArgs(XafApplication application, XtraReport report, Type dataType, DataColumnInfo dataColumnInfo, Parameter parameter) {
			this.Application = application;
			this.Report = report;
			this.DataType = dataType;
			this.DataColumnInfo = dataColumnInfo;
			this.Parameter = parameter;
		}
		public XafApplication Application { get; private set; }
		public XtraReport Report { get; private set; }
		public DataColumnInfo DataColumnInfo { get; private set; }
		public Parameter Parameter { get; private set; }
		public Type DataType  { get; private set; }
		public RepositoryItem RepositoryItem { get; set; }
	}
	public class AddCustomParameterTypesEventArgs : EventArgs {
		public AddCustomParameterTypesEventArgs(XafApplication application, IDictionary<Type, string> dictionary) {
			Guard.ArgumentNotNull(application, "application");
			Guard.ArgumentNotNull(dictionary, "dictionary");
			this.Dictionary = dictionary;
			this.Application = application;
		}
		public XafApplication Application { get; private set; }
		public IDictionary<Type, string> Dictionary { get; private set; }
	}
	public class GetCustomEditableDataTypesEventArgs : EventArgs {
		public GetCustomEditableDataTypesEventArgs(XafApplication application) {
			Guard.ArgumentNotNull(application, "application");
			this.Application = application;
		}
		public XafApplication Application { get; private set; }
		public Type[] Types { get; set; }
	}
	public class XafReportDataTypeProvider {
		private void extension_GetCustomEditableDataTypes(object sender, GetCustomEditableDataTypesEventArgs e) {
			Guard.ArgumentNotNull(e.Application, "e.Application");
			IDictionary<Type, string> result = new Dictionary<Type, string>();
			FillReportParametersTypes(e.Application.Model.BOModel, result, true);
			e.Types = new List<Type>(result.Keys).ToArray();
			if(GetCustomEditableDataTypes != null) {
				GetCustomEditableDataTypes(this, e);
			}
		}
		private void extension_CustomAddParameterTypes(object sender, AddCustomParameterTypesEventArgs e) {
			Guard.ArgumentNotNull(e.Application, "e.Application");
			Guard.ArgumentNotNull(e.Dictionary, "e.Dictionary");
			FillReportParametersTypes(e.Application.Model.BOModel, e.Dictionary, true);
			if(CustomAddParameterTypes != null) {
				CustomAddParameterTypes(this, e);
			}
		}
		private static void FillReportParametersTypes(IModelBOModel modelBOModel, IDictionary<Type, string> dictionary, bool includeEnums) {
			if(modelBOModel != null) {
				foreach(IModelClass currentModelClass in modelBOModel) {
					if(currentModelClass is IModelClassReportsVisibility && ((IModelClassReportsVisibility)currentModelClass).IsVisibleInReports) {
						if(currentModelClass.TypeInfo != null && currentModelClass.TypeInfo.Type != null) {
							Type classType = currentModelClass.TypeInfo.Type;
							if(!dictionary.ContainsKey(classType)) {
								dictionary.Add(classType, currentModelClass.Caption);
							}
							FillRelatedTypes(currentModelClass, modelBOModel, dictionary, includeEnums);
						}
					}
				}
			}
		}
		private static void FillRelatedTypes(IModelClass modelClass, IModelBOModel modelBOModel, IDictionary<Type, string> result, bool includeEnums) {
			foreach(IModelMember member in modelClass.AllMembers) {
				Guard.ArgumentNotNull(member.MemberInfo, "member.MemberInfo");
				Guard.ArgumentNotNull(member.MemberInfo.MemberTypeInfo, "member.MemberInfo.MemberTypeInfo");
				Guard.ArgumentNotNull(member.Type, "member.Type");
				if(member.MemberInfo.MemberTypeInfo.IsDomainComponent && member.MemberInfo.MemberTypeInfo.IsPersistent) {
					if(!result.ContainsKey(member.Type)) {
						IModelClass currentMemberModelClass = modelBOModel.GetClass(member.Type);
						result.Add(currentMemberModelClass.TypeInfo.Type, currentMemberModelClass.Caption);
						FillRelatedTypes(currentMemberModelClass, modelBOModel, result, includeEnums);
					}
				}
				if(includeEnums && member.Type.IsEnum) {
					if(!result.ContainsKey(member.Type)) {
						result.Add(member.Type, CaptionHelper.GetLocalizedText("Enums", member.Type.FullName, member.Type.Name));
					}
				}
			}
		}
		public XafReportDataTypeProvider() {
		}
		public void Attach(ReportExtension extension) {
			Guard.ArgumentNotNull(extension, "extension");
			extension.CustomAddParameterTypes += new EventHandler<AddCustomParameterTypesEventArgs>(extension_CustomAddParameterTypes);
			extension.GetCustomEditableDataTypes += new EventHandler<GetCustomEditableDataTypesEventArgs>(extension_GetCustomEditableDataTypes);
		}
		public event EventHandler<AddCustomParameterTypesEventArgs> CustomAddParameterTypes;
		public event EventHandler<GetCustomEditableDataTypesEventArgs> GetCustomEditableDataTypes;
	}   
	public class ReportExtension : ReportDesignExtension {
		public static XafApplication Application {
			get { return ValueManager.GetValueManager<XafApplication>("ReportExtension_XafApplication").Value; }
			set { ValueManager.GetValueManager<XafApplication>("ReportExtension_XafApplication").Value = value; }
		}
		protected override bool CanSerialize(object data) {
			if(data is ObjectSpaceComponent) {
				return true;
			}
			if(data != null) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(data.GetType());
				return typeInfo != null && typeInfo.IsPersistent;
			}
			return base.CanSerialize(data);
		}
		protected override string SerializeData(object data, XtraReport report) {
			if(data is ObjectSpaceComponent) {
				return ((ObjectSpaceComponent)data).DataType.FullName;
			}
			XafReport xafReport = report as XafReport;
			if(xafReport != null) {
				object targetObj = xafReport.ObjectSpace.GetObject(data);
				return xafReport.ObjectSpace.GetObjectHandle(targetObj);
			}
			return base.SerializeData(data, report);
		}
		protected override bool CanDeserialize(string value, string typeName) {
			if(typeName == typeof(ObjectSpaceComponent).FullName) {
				return true;
			}
			Type objectType = null;
			string keyString;
			if(ObjectHandleHelper.TryParseObjectHandle(XafTypesInfo.Instance, value, out objectType, out keyString)) {
				ITypeInfo typeInfo = XafTypesInfo.Instance.FindTypeInfo(objectType);
				if(typeInfo != null) {
					return typeInfo.IsPersistent;
				}
			}
			return base.CanDeserialize(value, typeName);
		}
		protected override object DeserializeData(string value, string typeName, XtraReport report) {
			if(typeName == typeof(ObjectSpaceComponent).FullName) {
				Guard.ArgumentNotNull(Application, "Application");
				Type type = XafTypesInfo.Instance.FindTypeInfo(value).Type;
				return new ObjectSpaceComponent(Application.CreateObjectSpace(type), type);
			}
			XafReport xafReport = report as XafReport;
			if(xafReport != null) {
				return xafReport.ObjectSpace.GetObjectByHandle(value);
			}
			return base.DeserializeData(value, typeName, report);
		}
		private bool OnCreateCustomReportRepositoryItem(XtraReport report, Type dataType, DataColumnInfo dataColumnInfo, Parameter parameter, out RepositoryItem repositoryItem) {
			repositoryItem = null;
			CreateCustomReportDesignRepositoryItemEventArgs args = new CreateCustomReportDesignRepositoryItemEventArgs(Application, report, dataType, dataColumnInfo, parameter);
			if(CreateCustomReportRepositoryItem != null) {
				CreateCustomReportRepositoryItem(this, args);
				if(args.Handled) {
					repositoryItem = args.RepositoryItem;
					return true;
				}
			}
			return false;
		}
		protected override RepositoryItem CreateRepositoryItem(DevExpress.Data.DataColumnInfo dataColumnInfo, Type dataType, XtraReport report) {
			RepositoryItem result;
			if(OnCreateCustomReportRepositoryItem(report, dataType, dataColumnInfo, null, out result)) {
				return result;
			}
			else {
				return base.CreateRepositoryItem(dataColumnInfo, dataType, report);
			}
		}
		protected override RepositoryItem CreateRepositoryItem(DevExpress.XtraReports.Parameters.Parameter parameter, Type dataType, XtraReport report) {
			RepositoryItem result;
			if(OnCreateCustomReportRepositoryItem(report, parameter.Type, null, parameter, out result)) {
				return result;
			}
			else {
				return base.CreateRepositoryItem(parameter, dataType, report);
			}			
		}
		public ReportExtension() {
		}
		public override Type[] GetEditableDataTypes() {
			if(GetCustomEditableDataTypes != null) {
				GetCustomEditableDataTypesEventArgs args = new GetCustomEditableDataTypesEventArgs(Application);
				GetCustomEditableDataTypes(this, args);
				return args.Types;
			}
			else {
				return base.GetEditableDataTypes();
			}
		}
		public override void AddParameterTypes(IDictionary<Type, string> dictionary) {
			if(CustomAddParameterTypes != null) {
				CustomAddParameterTypes(this, new AddCustomParameterTypesEventArgs(Application, dictionary));
			}
			else {
				base.AddParameterTypes(dictionary);
			}
		}
		public event EventHandler<CreateCustomReportDesignRepositoryItemEventArgs> CreateCustomReportRepositoryItem;
		public event EventHandler<AddCustomParameterTypesEventArgs> CustomAddParameterTypes;
		public event EventHandler<GetCustomEditableDataTypesEventArgs> GetCustomEditableDataTypes;
	}
}
