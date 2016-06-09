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
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
namespace DevExpress.Entity.ProjectModel {	
	public class DXTypeInfo : IDXTypeInfo {
		protected Type type;		
		public DXTypeInfo(Type type) {
			this.type = type;
			Name = type.Name;
			if(type.IsNested) {
				Type t = type.DeclaringType;
				StringBuilder sb = new StringBuilder(t.Name);
				while(t.IsNested) {
					sb.Insert(0, '+');
					t = t.DeclaringType;
					sb.Insert(0, t.Name);
				}
				DeclaringTypeName = sb.ToString();
			}
			NamespaceName = type.Namespace;
		}
		public DXTypeInfo(string name, string namespaceName) : this(name, null, namespaceName) { }
		public DXTypeInfo(string name, string declaringTypeName, string namespaceName) {
			Name = name;
			DeclaringTypeName = declaringTypeName;
			NamespaceName = namespaceName;			
		}
		public IDXAssemblyInfo Assembly {
			get;
			set;
		}		
		public Type ResolveType() {
			return type;			
		}
		public string Name {
			get;
			private set;
		}
		public string DeclaringTypeName { get; private set; }
		public string NamespaceName {
			get;
			private set;
		}	   
		public virtual string FullName {
			get {
				return GetFullName(NamespaceName, DeclaringTypeName, Name);
			}
		}
		public static string GetFullName(string namespaceName, string name) {
			return GetFullName(namespaceName, null, name);
		}
		public static string GetFullName(string namespaceName, string declaringTypeName, string name){
			 StringBuilder sb = new StringBuilder(namespaceName);
			if(sb.Length != 0)
				sb.Append('.');
			if(!string.IsNullOrEmpty(declaringTypeName)) {
				sb.Append(declaringTypeName);
				sb.Append('+');
			}
			sb.Append(name);
			return sb.ToString();
		}
		public bool IsSolutionType {
			get {
				return Assembly != null && Assembly.IsSolutionAssembly;
			}
		}
	}	
	public class DXMemberInfo : IDXMemberInfo {
		public DXMemberInfo(MemberInfo memberInfo):this(memberInfo.Name) {
		}
		public DXMemberInfo(string name) {
			Name = name;
		}
		public string Name {
			get; private set;
		}		
	}
	public class DXMethodInfo : DXMemberInfo, IDXMethodInfo {
		public DXMethodInfo(string name, IDXTypeInfo returnType):base(name) {
			ReturnType = returnType;
		}
		public DXMethodInfo(MethodInfo methodInfo)
			: base(methodInfo) {
			ReturnType = MetaDataServices.GetExistingOrCreateNew(methodInfo.ReturnType);			 
		}
		public IDXTypeInfo ReturnType {
			get;
			private set;
		}
	}
	public class DXPropertyInfo : DXMemberInfo, IDXPropertyInfo {
		public DXPropertyInfo(PropertyInfo propertyInfo)
			: base(propertyInfo) {
				PropertyType = MetaDataServices.GetExistingOrCreateNew(propertyInfo.PropertyType);
		}
		public DXPropertyInfo(string name, IDXTypeInfo propertyType)
			: base(name) {
			PropertyType = propertyType;
		}
		public IDXTypeInfo PropertyType {
			get;
			private set;
		}
	}
	public class DXFieldInfo : DXMemberInfo, IDXFieldInfo {
		public DXFieldInfo(FieldInfo fieldInfo)
			: base(fieldInfo) {
				FieldType = MetaDataServices.GetExistingOrCreateNew(fieldInfo.FieldType);			
		}
		public DXFieldInfo(string name, IDXTypeInfo fieldType)
			: base(name) {
			FieldType = fieldType;
		}
		public IDXTypeInfo FieldType {
			get;
			private set;
		}
	}
}
