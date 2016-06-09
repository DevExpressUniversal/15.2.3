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

namespace DevExpress.Design.Metadata {
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	public abstract class TypeConstraint {
		readonly string BaseTypeName;
		public TypeConstraint(string baseTypeName) {
			AssertionException.IsNotNullOrEmpty(baseTypeName);
			BaseTypeName = baseTypeName;
		}
		public bool Match(Type type) {
			Type baseType = type.BaseType;
			return (baseType != null) && (GetFullName(baseType) == BaseTypeName) && MatchCore(type);
		}
		public abstract IEnumerable<MemberInfo> GetMembers(Type type);
		protected abstract bool MatchCore(Type type);
		protected internal static string GetFullName(Type type) {
			return (type != null) ? (type.Namespace + '.' + type.Name) : null;
		}
	}
	public class TypeNameConstraint : TypeConstraint {
		readonly string TypeName;
		public TypeNameConstraint(string typeName, string baseTypeName)
			: base(baseTypeName) {
			TypeName = typeName;
		}
		public override IEnumerable<MemberInfo> GetMembers(Type type) {
			yield break;
		}
		protected override bool MatchCore(Type type) {
			return GetFullName(type) == TypeName;
		}
	}
	public class PropertyConstraint : TypeConstraint {
		readonly string PropertyTypeName;
		public PropertyConstraint(string baseTypeName, string propertyTypeName)
			: base(baseTypeName) {
			AssertionException.IsNotNullOrEmpty(propertyTypeName);
			this.PropertyTypeName = propertyTypeName;
		}
		public override IEnumerable<MemberInfo> GetMembers(Type type) {
			var properties = type.GetProperties();
			foreach(PropertyInfo pInfo in properties) {
				if(Match(pInfo))
					yield return pInfo;
			}
		}
		protected override bool MatchCore(Type type) {
			var properties = type.GetProperties();
			foreach(PropertyInfo pInfo in properties) {
				if(Match(pInfo))
					return true;
			}
			return false;
		}
		bool Match(PropertyInfo p) {
			try {
				return GetFullName(p.PropertyType) == PropertyTypeName || GetFullName(p.PropertyType.BaseType) == PropertyTypeName;
			}
			catch { return false; }
		}
	}
	public class MethodConstraint : TypeConstraint {
		readonly string ReturnTypeName;
		readonly IEnumerable<string> ParameterTypes;
		public MethodConstraint(string baseTypeName, string returnTypeName, IEnumerable<string> parameterTypes)
			: base(baseTypeName) {
			AssertionException.IsNotNullOrEmpty(returnTypeName);
			this.ReturnTypeName = returnTypeName;
			this.ParameterTypes = parameterTypes ?? new string[0];
		}
		public override IEnumerable<MemberInfo> GetMembers(Type type) {
			var methods = type.GetMethods();
			foreach(MethodInfo mInfo in methods) {
				if(Match(mInfo))
					yield return mInfo;
			}
		}
		protected override bool MatchCore(Type type) {
			var methods = type.GetMethods();
			foreach(MethodInfo mInfo in methods) {
				if(Match(mInfo)) 
					return true;
			}
			return false;
		}
		bool Match(MethodInfo m) {
			try {
				bool matchReturmType = GetFullName(m.ReturnType) == ReturnTypeName;
				return matchReturmType && System.Linq.Enumerable.SequenceEqual(GetParameterTypes(m), ParameterTypes);
			}
			catch { return false; }
		}
		static IEnumerable<string> GetParameterTypes(MethodInfo methodInfo) {
			var parameters = methodInfo.GetParameters();
			foreach(ParameterInfo paramInfo in parameters) {
				yield return GetFullName(paramInfo.ParameterType);
			}
		}
	}
	public class TypeSetConstraint : TypeConstraint {
		readonly string RootTypeName;
		public TypeSetConstraint(string baseTypeName, string rootTypeName)
			: base(baseTypeName) {
			RootTypeName = rootTypeName;
		}
		public override IEnumerable<MemberInfo> GetMembers(Type type) {
			yield break;
		}
		protected override bool MatchCore(Type type) {
			Type rootType = GetRootType(type);
			return (rootType != null) && GetFullName(rootType) == RootTypeName;
		}
		static Type GetRootType(Type type) {
			Type rootType = type.BaseType;
			while(rootType.BaseType != null && rootType.BaseType != typeof(object)) {
				rootType = rootType.BaseType;
			}
			return rootType;
		}
	}
}
