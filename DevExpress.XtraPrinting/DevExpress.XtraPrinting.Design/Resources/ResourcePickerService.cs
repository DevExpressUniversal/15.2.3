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
using System.Text;
using EnvDTE;
using System.Reflection;
using System.IO;
using System.CodeDom.Compiler;
using DevExpress.Data.Utils;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.Design.Resources {
	public class ResourcePickerService {
		public static Type GetVSType(IServiceProvider serviceProvider) {
			return VSTypeHelper.GetType(serviceProvider, "Microsoft.VisualStudio.Windows.Forms", "Microsoft.VisualStudio.Windows.Forms.ResourcePickerService"); ;
		}
		Type resourcePickerServiceType;
		object resourcePickerService;
		public object Instance {
			get { return resourcePickerService; }
		}
		public string ActiveProjectPath {
			get {
				PropertyInfo pi = resourcePickerServiceType.GetProperty("ActiveProjectPath", BindingFlags.NonPublic | BindingFlags.Instance);
				return (string)pi.GetValue(resourcePickerService, null);
			}
		}
		public CodeDomProvider CodeProvider {
			get {
				PropertyInfo pi = resourcePickerServiceType.GetProperty("CodeProvider", BindingFlags.NonPublic | BindingFlags.Instance);
				return (CodeDomProvider)pi.GetValue(resourcePickerService, null);
			}
		}
		public ResourcePickerService(IServiceProvider serviceProvider) {
			resourcePickerServiceType = GetVSType(serviceProvider);
			ConstructorInfo ci = resourcePickerServiceType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[] { typeof(_DTE), typeof(IServiceProvider) }, new ParameterModifier[2]);
			resourcePickerService = ci.Invoke(new object[] { VSTypeHelper.GetDTE(serviceProvider), serviceProvider });
		}
		public Dictionary<string, string> GetProjectResXFileNames() {
			return (Dictionary<string, string>)CallMethod("GetProjectResXFileNames");
		}
		public void AddResXFileToProject(string fullName) {
			CallMethod("AddResXFileToProject", fullName);
		}
		object CallMethod(String name, params object[] args) {
			MethodInfo mi = resourcePickerServiceType.GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic);
			return mi.Invoke(resourcePickerService, args);
		}
	}
	static class VSTypeHelper {
		public static _DTE GetDTE(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			return serviceProvider.GetService(typeof(DTE)) as _DTE;
		}
		public static Type GetType(IServiceProvider serviceProvider, string assemblyName, string typeName) {
			string qualifiedTypeName = typeName + "," + assemblyName;
			string versionString = GetVersionString(serviceProvider);
			if(!string.IsNullOrEmpty(versionString)) {
				qualifiedTypeName += GetNameTail(versionString);
			}
			return Type.GetType(qualifiedTypeName);
		}
		static string GetVersionString(IServiceProvider serviceProvider) {
			string version = GetDTE(serviceProvider).Version;
			return version == "8.0" ? "2" : new Version(version).Major.ToString();
		}
		static string GetNameTail(string version) {
			return string.Concat(", Version=", version, ".0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a");
		}
	}
}
