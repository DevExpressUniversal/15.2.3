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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.IO;
using DevExpress.Utils.Design;
using EnvDTE;
namespace DevExpress.Design.VSIntegration {
	[CLSCompliant(false)]
	public class DTEServiceBase {
		static string GetTypeFullName(object obj) {
			return obj != null ? obj.GetType().FullName : null;
		}
		private IServiceProvider serviceProvider;
		public string ProjectFullName {
			get {
				ProjectItem projectItem = serviceProvider.GetService(typeof(ProjectItem)) as ProjectItem;
				if(projectItem != null) {
					string path = DTEHelper.GetPropertyValue(projectItem.ContainingProject, "FullPath");
					if(File.Exists(path))
						path = Path.GetDirectoryName(path);
					return path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
				}
				return String.Empty;
			}
		}
		public DTEServiceBase(IServiceProvider serviceProvider) {
			this.serviceProvider = serviceProvider;
		}
		public string[] GetClassesInfo(Type filerType, IList ignoreClassNames) {
			List<string> names = new List<string>();
			if(filerType != null) {
				string[] typeNames = GetClassesInfo(filerType, null, null);
				foreach(string typeName in typeNames) {
					if(ignoreClassNames != null && ignoreClassNames.Contains(typeName))
						continue;
					if(names.IndexOf(typeName) == -1)
						names.Add(typeName);
				}
			}
			return names.ToArray();
		}
		public string[] GetClassesInfo(Type baseType, Attribute attribute, string attributeValue) {
			try {
				ProjectItem projectItem = (ProjectItem)serviceProvider.GetService(typeof(ProjectItem));
				if(projectItem != null) {
					Project project = projectItem.ContainingProject;
					projectItem = null;
					return GetClassesInfo(project.ProjectItems, baseType, attribute, attributeValue);
				}
			} catch { }
			return new string[] { };
		}
		public string[] GetClassesInfo(ProjectItems projectItems, Type baseType) {
			return GetClassesInfo(projectItems, baseType, null, null);
		}
		public string[] GetClassesInfo(ProjectItems projectItems, Type baseType, Attribute attribute, string attributeValue) {
			return GetClassesInfo(projectItems, baseType, new AttributeCodeTypeFilter(attribute, attributeValue));
		}
		public string[] GetClassesInfo(ProjectItems projectItems, Type baseType, ICodeTypeFilter codeTypeFilter) {
			try {
				CodeType[] codeElements = ProjectHelper.FindCodeElements(projectItems, baseType.FullName);
				List<string> typeNames = GetTypeNames(codeElements, codeTypeFilter);
				MergeStrings(typeNames, GetTypeNames(baseType, codeTypeFilter));
				return typeNames.ToArray();
			} catch {
				return new string[] { };
			}
		}
		List<string> GetTypeNames(Type baseType, ICodeTypeFilter codeTypeFilter) {
			List<string> names = new List<string>();
			if(serviceProvider != null) {
				try {
					IDesignerHost host = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
					ITypeDiscoveryService svc = (ITypeDiscoveryService)host.GetService(typeof(ITypeDiscoveryService));
					ICollection types = svc.GetTypes(baseType, true);
					foreach(Type type in types) {
						if(codeTypeFilter.MatchType(type))
							names.Add(type.FullName);
					}
				} catch { }
			}
			return names;
		}
		static void MergeStrings(List<string> array1, List<string> array2) {
			foreach(string item in array2)
				if(!array1.Contains(item))
					array1.Add(item);
		}
		List<string> GetTypeNames(CodeType[] codeElements, ICodeTypeFilter codeTypeFilter) {
			List<string> names = new List<string>();
			try {
				foreach(CodeType codeType in codeElements) {
					if(codeTypeFilter.MatchCodeType(codeType))
						names.Add(codeType.FullName);
				}
			} catch { }
			return names;
		}
	}
	[CLSCompliant(false)]
	public interface ICodeTypeFilter {
		bool MatchCodeType(CodeType codeType);
		bool MatchType(Type type);
	}
	[CLSCompliant(false)]
	public class AttributeCodeTypeFilter : ICodeTypeFilter {
		protected string attributeTypeName;
		protected string attributeValue;
		Attribute attribute;
		public AttributeCodeTypeFilter(Attribute attribute, string attributeValue) {
			this.attribute = attribute;
			this.attributeTypeName = GetTypeFullName(attribute);
			this.attributeValue = attributeValue;
		}
		string GetTypeFullName(object obj) {
			return obj != null ? obj.GetType().FullName : null;
		}
		public bool MatchType(Type type) {
			return type != null && (attribute == null || AttributesContainAttribute(TypeDescriptor.GetAttributes(type), attribute));
		}
		bool AttributesContainAttribute(AttributeCollection attributes, Attribute attribute) {
			return attribute != null && attribute.Match(attributes[attribute.GetType()]);
		}
		public bool MatchCodeType(CodeType codeType) {
			return codeType != null && (attribute == null || ElementsContainAttribute(codeType.Attributes));
		}
		bool ElementsContainAttribute(CodeElements codeElements) {
			if(attributeTypeName != null && attributeValue != null) {
				foreach(CodeElement codeElement in codeElements) {
					if(codeElement.Kind == vsCMElement.vsCMElementAttribute && MatchCodeElementName(codeElement, attributeTypeName))
						return ((CodeAttribute)codeElement).Value.ToLower() == attributeValue.ToLower();
				}
			}
			return false;
		}
		protected virtual bool MatchCodeElementName(CodeElement codeElement, string name) {
			return codeElement.FullName == name || codeElement.Name == name || codeElement.Name == "Global." + name;
		}
	}
	[CLSCompliant(false)]
	public class DTEService : DTEServiceBase, IDTEService {
		public DTEService(IServiceProvider serviceProvider) : base(serviceProvider) { 
		}
	}
}
