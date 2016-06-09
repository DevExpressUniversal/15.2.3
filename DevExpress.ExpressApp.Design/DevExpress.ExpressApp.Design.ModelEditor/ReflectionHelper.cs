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
using System.Linq;
using System.Text;
using System.Reflection;
namespace DevExpress.ExpressApp.Design.Core {
	public interface IModelEditorSettings { }
	public class DesignCorePackageReflectionHelper {
		public static EventInfo GetEventInfo(Type ownerType, string eventName) {
			EventInfo info = ownerType.GetEvent(eventName);
			if(info == null) {
				info = ownerType.GetEvent(eventName, BindingFlags.NonPublic | BindingFlags.Instance);
			}
			return info;
		}
		public static FieldInfo GetFieldInfo(Type ownerType, string fieldName) {
			FieldInfo info = ownerType.GetField(fieldName);
			if(info == null) {
				info = ownerType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			}
			return info;
		}
		public static object GetFieldValue(Type ownerType, object owner, string fieldName) {
			FieldInfo info = GetFieldInfo(ownerType, fieldName);
			return info != null? info.GetValue(owner) : null;
		}
		public static void SetFieldValue(Type ownerType, object owner, string fieldName, object newValue) {
			FieldInfo info = GetFieldInfo(ownerType, fieldName);
			if(info != null) {
				info.SetValue(owner, newValue);
			}
		}
		protected static PropertyInfo GetPropertyInfo(Type ownerType, string propertyName, Type[] indexerTypes) {
			PropertyInfo pinfo = ownerType.GetProperty(propertyName, indexerTypes);
			if(pinfo == null && indexerTypes.Length == 0) {
				pinfo = ownerType.GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Instance);
			}
			if(pinfo == null) {
				if(indexerTypes.Length == 0)
					return null;
				else {
					string types = string.Empty;
					foreach(Type indexerType in indexerTypes)
						types += indexerType.Name + ", ";
					types.TrimEnd(',');
					return null;
				}
			}
			return pinfo;
		}
		public static PropertyInfo GetPropertyInfo(Type ownerType, string propertyName) {
			return GetPropertyInfo(ownerType, propertyName, new Type[] { });
		}
		public static object GetPropertyValue(Type ownerType, object owner, string propertyName) {
			PropertyInfo propertyInfo = GetPropertyInfo(ownerType, propertyName);
			if(propertyInfo != null) {
				return propertyInfo.GetValue(owner, null);
			}
			return null;
		}
		public static void SetPropertyValue(Type ownerType, object owner, string propertyName, object newValue) {
			PropertyInfo propertyInfo = GetPropertyInfo(ownerType, propertyName);
			if(propertyInfo != null) {
				propertyInfo.SetValue(owner, newValue, null);
			}
		}
		public static MethodInfo GetMethodInfo(Type ownerType, string methodName) {
			MethodInfo methodInfo = null;
			try {
				methodInfo = ownerType.GetMethod(methodName);
				if(methodInfo == null) {
					methodInfo = ownerType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
				}
				if(methodInfo == null) {
					return null;
				}
			}
			catch(AmbiguousMatchException) {
				methodInfo = GetMethodInfo(ownerType, methodName, Type.EmptyTypes);
			}
			return methodInfo;
		}
		public static MethodInfo GetMethodInfo(Type ownerType, string methodName, Type[] types) {
			MethodInfo methodInfo = ownerType.GetMethod(methodName, types);
			if(methodInfo == null) {
				methodInfo = ownerType.GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance, null, types, null);
			}
			if(methodInfo == null) {
				return null;
			}
			return methodInfo;
		}
	}
}
