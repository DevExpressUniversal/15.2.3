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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.SystemModule.ModelXmlConverters {
	public class ModelClassIdConverter : ModelXmlConverterBase {
		protected override void ConvertCore() {
			Version moduleVersion = GetModuleXmlVersion(typeof(SystemModule).Name);
			if(moduleVersion == null || moduleVersion < new Version(13, 1)) {
				IEnumerable<Type> boModelTypes = ModelSources != null ? ModelSources.BOModelTypes : Type.EmptyTypes;
				string actualIdName;
				string updatedValue;
				if(typeof(IModelClass).IsAssignableFrom(ConvertXmlParameters.NodeType)
					&& ContainsValueIgnoreCase("Name", out actualIdName)
					&& NeedUpdateClassName(GetValue(actualIdName), boModelTypes, out updatedValue)
				) {
					SetValue(actualIdName, updatedValue);
				}
				foreach(Tuple<string, ModelValueInfo> info in GetXmlValueNamesWithModelValueInfo()) {
					string xmlValueName = info.Item1;
					ModelValueInfo valueInfo = info.Item2;
					if(typeof(IModelClass).IsAssignableFrom(valueInfo.PropertyType) && NeedUpdateClassName(GetValue(xmlValueName), boModelTypes, out updatedValue)) {
						SetValue(xmlValueName, updatedValue);
					}
				}
			}
		}
		private static bool NeedUpdateClassName(string className, IEnumerable<Type> boModelTypes, out string updatedClassName) {
			bool result = false;
			if(NeedUpdateClassNameToCurrentVersion(className, out updatedClassName)) {
				className = updatedClassName;
				result = true;
			}
			if(NeedUpdateClassNameToNewId(className, boModelTypes, out updatedClassName)) {
				result = true;
			}
			return result;
		}
		private static bool NeedUpdateClassNameToCurrentVersion(string className, out string updatedClassName) {
			bool result = false;
			updatedClassName = null;
			int bracketStartIndex = className.IndexOf("[DevExpress.ExpressApp");
			if(bracketStartIndex >= 0) {
				int bracketEndIndex = className.IndexOf(']', bracketStartIndex);
				string genericArgumentFullName = className.Substring(bracketStartIndex + 1, bracketEndIndex - bracketStartIndex - 2);
				string[] genericArgumentFullNameParameters = genericArgumentFullName.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
				if(genericArgumentFullNameParameters.Length >= 3) {
					string versionText = genericArgumentFullNameParameters[2];
					const string versionPrefix = "Version=";
					if(versionText.StartsWith(versionPrefix)) {
						Version version = new Version(versionText.Substring(versionPrefix.Length));
						updatedClassName = className.Replace(versionPrefix + version.ToString(), versionPrefix + AssemblyInfo.Version);
						updatedClassName = updatedClassName.Replace(string.Format(".v{0}.{1}", version.Major, version.Minor), AssemblyInfo.VSuffix);
						result = true;
					}
				}
			}
			return result;
		}
		private static bool NeedUpdateClassNameToNewId(string className, IEnumerable<Type> boModelTypes, out string updatedClassName) {
			bool result = false;
			updatedClassName = null;
			if(className.Contains("`")) {
				foreach(Type type in boModelTypes) {
					string oldModelClassName = type.FullName;
					if(className == oldModelClassName) {
						updatedClassName = ModelNodeIdHelper.GetTypeId(type);
						result = true;
						break;
					}
				}
			}
			return result;
		}
	}
	public class ModelViewIdConverter : ModelXmlConverterBase {
		protected override void ConvertCore() {
			Version moduleVersion = GetModuleXmlVersion(typeof(SystemModule).Name);
			if(moduleVersion != null && moduleVersion >= new Version(13, 1) && moduleVersion < new Version(13, 1, 6)) {
				IEnumerable<Type> boModelTypes = ModelSources != null ? ModelSources.BOModelTypes : Type.EmptyTypes;
				string actualIdName;
				string updatedValue;
				if(typeof(IModelView).IsAssignableFrom(ConvertXmlParameters.NodeType)
					&& ContainsValueIgnoreCase("Id", out actualIdName)
					&& NeedUpdateViewId(GetValue(actualIdName), boModelTypes, out updatedValue)
				) {
					SetValue(actualIdName, updatedValue);
				}
				foreach(Tuple<string, ModelValueInfo> info in GetXmlValueNamesWithModelValueInfo()) {
					string xmlValueName = info.Item1;
					ModelValueInfo valueInfo = info.Item2;
					if(typeof(IModelView).IsAssignableFrom(valueInfo.PropertyType) && NeedUpdateViewId(GetValue(xmlValueName), boModelTypes, out updatedValue)) {
						SetValue(xmlValueName, updatedValue);
					}
				}
			}
		}
		private static bool NeedUpdateViewId(string viewId, IEnumerable<Type> boModelTypes, out string updatedViewId) {
			foreach(Type type in boModelTypes) {
				if(viewId == GetDetailViewOldId(type)) {
					updatedViewId = ModelNodeIdHelper.GetDetailViewId(type);
					return true;
				}
				if(viewId == GetListViewOldId(type)) {
					updatedViewId = ModelNodeIdHelper.GetListViewId(type);
					return true;
				}
				if(viewId == GetLookupListViewOldId(type)) {
					updatedViewId = ModelNodeIdHelper.GetLookupListViewId(type);
					return true;
				}
				if(viewId.EndsWith("_ListView") && viewId.IndexOf('_') + "_ListView".Length < viewId.Length) {
					foreach(PropertyInfo property in TypeHelper.GetProperties(type)) {
						if(typeof(IEnumerable).IsAssignableFrom(property.PropertyType)
							&& property.PropertyType != typeof(string)
							&& viewId == GetNestedListViewOldId(type, property.Name)) {
							updatedViewId = ModelNodeIdHelper.GetNestedListViewId(type, property.Name);
							return true;
						}
					}
				}
			}
			updatedViewId = null;
			return false;
		}
		public static string GetDetailViewOldId(Type type) {
			return ModelNodeIdHelper.GetTypeId(type) + "_DetailView";
		}
		public static string GetListViewOldId(Type type) {
			return ModelNodeIdHelper.GetTypeId(type) + "_ListView";
		}
		public static string GetLookupListViewOldId(Type type) {
			return ModelNodeIdHelper.GetTypeId(type) + "_LookupListView";
		}
		public static string GetNestedListViewOldId(Type type, string propertyName) {
			return ModelNodeIdHelper.GetTypeId(type) + "_" + propertyName + "_ListView";
		}
	}
	public class ModelNavigationItemIdConverter : ModelXmlConverterBase {
		protected override void ConvertCore() {
			Version moduleVersion = GetModuleXmlVersion(typeof(SystemModule).Name);
			if(moduleVersion != null && moduleVersion >= new Version(13, 1) && moduleVersion < new Version(13, 1, 6)) {
				IEnumerable<Type> boModelTypes = ModelSources != null ? ModelSources.BOModelTypes : Type.EmptyTypes;
				string actualIdName;
				string updatedValue;
				if(typeof(IModelNavigationItem).IsAssignableFrom(ConvertXmlParameters.NodeType)
					&& ContainsValueIgnoreCase("Id", out actualIdName)
					&& NeedUpdateNavigationItemId(GetValue(actualIdName), boModelTypes, out updatedValue)
				) {
					SetValue(actualIdName, updatedValue);
				}
				const string startupNavigationItemProperty = "StartupNavigationItem";
				string xmlValue;
				if(typeof(IModelRootNavigationItems).IsAssignableFrom(ConvertXmlParameters.NodeType)
					&& TryGetValue(startupNavigationItemProperty, out xmlValue)
					&& NeedUpdateNavigationItemId(xmlValue, boModelTypes, out updatedValue)
				) {
					SetValue(startupNavigationItemProperty, updatedValue);
				}
			}
		}
		private static bool NeedUpdateNavigationItemId(string navigationItemId, IEnumerable<Type> boModelTypes, out string updatedNavigationItemId) {
			foreach(Type type in boModelTypes) {
				if(navigationItemId == ModelViewIdConverter.GetListViewOldId(type)) {
					updatedNavigationItemId = ModelNodeIdHelper.GetListViewId(type);
					return true;
				}
			}
			updatedNavigationItemId = null;
			return false;
		}
	}
}
