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
using System.Text.RegularExpressions;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Updating;
using DevExpress.ExpressApp.Utils;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.Workflow {
	public class WorkflowDefinitionsUpdater : ModuleUpdater {
		public const string XamlPropertyName = "Xaml";
		public const string TraceLogSectionName = "WorkflowDefinitionsUpdater.UpdateDatabaseAfterUpdateSchema";
		private Type workflowDefinitionType;
		private Type userActivityVersionType;
		public static string UpdateDxAssembliesVersions(string stringToUpdate, Version oldVersion, Version newVersion) {
			if(string.IsNullOrEmpty(stringToUpdate)) {
				return stringToUpdate;
			}
			string dxAssemblyVersionFormat = ".v{0}.{1}";
			string oldVersionString = string.Format(dxAssemblyVersionFormat, oldVersion.Major, oldVersion.Minor);
			string newVersionString = string.Format(dxAssemblyVersionFormat, newVersion.Major, newVersion.Minor);
			string dxVersionPattern = @"DevExpress\.([\w\.])*" + Regex.Escape(oldVersionString);
			Regex regex = new Regex(dxVersionPattern);
			string updatedString = regex.Replace(stringToUpdate, delegate(Match m) {
				return m.Value.Replace(oldVersionString, newVersionString);
			});
			return updatedString;
		}
		private void UpdateVersionInXaml(ITypeInfo objectsTypeInfo, Version oldVersion, Version newVersion) {
			if(objectsTypeInfo != null && objectsTypeInfo.IsPersistent) {
				IMemberInfo xamlMemberInfo = objectsTypeInfo.FindMember(XamlPropertyName);
				if(xamlMemberInfo == null) {
					throw new DevExpress.Persistent.Base.MemberNotFoundException(objectsTypeInfo.Type, XamlPropertyName);
				}
				Tracing.Tracer.LogText("UpdateVersionInXaml({0})", objectsTypeInfo.FullName);
				System.Collections.IList objectsToUpdate = ObjectSpace.GetObjects(objectsTypeInfo.Type);
				foreach(object objectToUpdate in objectsToUpdate) {
					Tracing.Tracer.LogVerboseValue("objectToUpdate", ObjectSpace.GetKeyValueAsString(objectToUpdate));
					string currentXaml = xamlMemberInfo.GetValue(objectToUpdate) as string;
					string updatedXaml = UpdateDxAssembliesVersions(currentXaml, oldVersion, newVersion);
					xamlMemberInfo.SetValue(objectToUpdate, updatedXaml);
					ObjectSpace.SetModified(objectToUpdate);
				}
				Tracing.Tracer.LogText("{0} objects are processed.", objectsToUpdate.Count);
				if(ObjectSpace.IsModified) {
					ObjectSpace.CommitChanges();
				}
			}
		}
		private Type FindModuleInfoType() {
			Type result = null;
			foreach(ITypeInfo typeInfo in XafTypesInfo.Instance.PersistentTypes) {
				if(typeof(IModuleInfo).IsAssignableFrom(typeInfo.Type) && ObjectSpace.CanInstantiate(typeInfo.Type)) {
					result = typeInfo.Type;
					return result;
				}
			}
			return result;
		}
		public WorkflowDefinitionsUpdater(IObjectSpace objectSpace, Version currentDBVersion)
			: base(objectSpace, currentDBVersion) {
		}
		public WorkflowDefinitionsUpdater(IObjectSpace objectSpace, Version currentDBVersion, Type workflowDefinitionType, Type userActivityVersionType) : this(objectSpace, currentDBVersion) {
			this.workflowDefinitionType = workflowDefinitionType;
			this.userActivityVersionType = userActivityVersionType;
		}
		public override void UpdateDatabaseAfterUpdateSchema() {
			base.UpdateDatabaseAfterUpdateSchema();
			Tracing.Tracer.LogSubSeparator(TraceLogSectionName);
			Version currentModuleVersion = typeof(WorkflowModule).Assembly.GetName().Version;			
			Type moduleInfoType = FindModuleInfoType();			
			if(moduleInfoType != null){
				IModuleInfo workflowModuleInfo = ObjectSpace.FindObject(moduleInfoType, new BinaryOperator("Name", WorkflowModule.ModuleName)) as IModuleInfo;
				Tracing.Tracer.LogValue("workflowModuleInfo", workflowModuleInfo);
				if(workflowModuleInfo != null) {
					Version dbModuleVersion = new Version(workflowModuleInfo.Version);
					Tracing.Tracer.LogValue("workflowModuleInfo.Version", dbModuleVersion);
					Tracing.Tracer.LogValue("currentModuleVersion", currentModuleVersion);
					if(dbModuleVersion < currentModuleVersion) {
						if(dbModuleVersion.Major != currentModuleVersion.Major || dbModuleVersion.Minor != currentModuleVersion.Minor) {
							UpdateVersionInXaml(XafTypesInfo.Instance.FindTypeInfo(workflowDefinitionType), dbModuleVersion, currentModuleVersion);
							UpdateVersionInXaml(XafTypesInfo.Instance.FindTypeInfo(userActivityVersionType), dbModuleVersion, currentModuleVersion);
						}
					}
				}
			}
			Tracing.Tracer.LogText(TraceLogSectionName + " completed");
		}
	}
}
