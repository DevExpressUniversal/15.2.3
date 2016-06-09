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
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
namespace DevExpress.ExpressApp.ReportsV2 {
	public class XtraReportDataTypeProvider {
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
		public XtraReportDataTypeProvider() {
		}
		public void Attach(XtraReportExtensionBase extension) {
			Guard.ArgumentNotNull(extension, "extension");
			extension.CustomAddParameterTypes += new EventHandler<AddCustomParameterTypesEventArgs>(extension_CustomAddParameterTypes);
			extension.GetCustomEditableDataTypes += new EventHandler<GetCustomEditableDataTypesEventArgs>(extension_GetCustomEditableDataTypes);
		}
		public event EventHandler<AddCustomParameterTypesEventArgs> CustomAddParameterTypes;
		public event EventHandler<GetCustomEditableDataTypesEventArgs> GetCustomEditableDataTypes;
	}   
}
