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

using DevExpress.Mvvm.Native;
using System;
using System.Reflection;
using System.Linq;
using DevExpress.Mvvm.POCO;
using System.ComponentModel;
namespace DevExpress.Mvvm {
	public static class IDataErrorInfoHelper {
#if !SILVERLIGHT
		public static bool HasErrors(IDataErrorInfo owner, int deep = 2) {
			return HasErrors(owner, false, deep);
		}
		public static bool HasErrors(IDataErrorInfo owner, bool ignoreOwnerError, int deep = 2) {
			if(owner == null) throw new ArgumentNullException("owner");
			if(--deep < 0) return false;
			var properties = TypeDescriptor.GetProperties(owner).Cast<PropertyDescriptor>();
			var errorProperty = properties.FirstOrDefault(p => p.Name == "Error");
			bool hasImplicitImplementation = ExpressionHelper.PropertyHasImplicitImplementation(owner, o => o.Error, false);
			if(errorProperty != null && hasImplicitImplementation) {
				properties = properties.Except(new[] { errorProperty });
			}
			bool propertiesHaveError = properties.Any(p => PropertyHasError(owner, p, deep));
			return propertiesHaveError || (!ignoreOwnerError && !string.IsNullOrEmpty(owner.Error));
		}
		static bool PropertyHasError(IDataErrorInfo owner, PropertyDescriptor property, int deep) {
			string simplePropertyError = owner[property.Name];
			if(!string.IsNullOrEmpty(simplePropertyError)) return true;
			object propertyValue;
			if(!TryGetPropertyValue(owner, property.Name, out propertyValue))
				return false;
			IDataErrorInfo nestedDataErrorInfo = propertyValue as IDataErrorInfo;
			return nestedDataErrorInfo != null && HasErrors(nestedDataErrorInfo, deep);
		}
#endif
		public static string GetErrorText(object owner, string propertyName) {
			if(owner == null)
				throw new ArgumentNullException("owner");
			int pathDelimiterIndex = propertyName.IndexOf('.');
			if(pathDelimiterIndex >= 0)
				return GetNestedPropertyErrorText(owner, propertyName, pathDelimiterIndex);
			return GetNonNestedErrorText(owner, propertyName) ?? string.Empty;
		}
		static string GetNestedPropertyErrorText(object owner, string path, int pathDelimiterIndex) {
			string propertyName = path.Remove(pathDelimiterIndex);
			object propertyValue;
			if(!TryGetPropertyValue(owner, propertyName, out propertyValue))
				return string.Empty;
			IDataErrorInfo nestedDataErrorInfo = propertyValue as IDataErrorInfo;
			if(nestedDataErrorInfo == null)
				return string.Empty;
			return nestedDataErrorInfo[path.Substring(pathDelimiterIndex + 1, path.Length - pathDelimiterIndex - 1)];
		}
		static string GetNonNestedErrorText(object obj, string propertyName) {
			Type objType = obj.GetType();
			if(obj is IPOCOViewModel) {
				objType = objType.BaseType;
			}
			PropertyValidator validator = GetPropertyValidator(objType, propertyName);
			if(validator == null)
				return null;
			object propertyValue;
			if(!TryGetPropertyValue(obj, propertyName, out propertyValue))
				return null;
			return validator.GetErrorText(propertyValue, obj);
		}
		static bool TryGetPropertyValue(object owner, string propertyName, out object propertyValue) {
			propertyValue = null;
			PropertyInfo pi = owner.GetType().GetProperty(propertyName);
			if(pi == null)
				return false;
			MethodInfo getter = pi.GetGetMethod();
			if(getter == null)
				return false;
			propertyValue = getter.Invoke(owner, null);
			return true;
		}
		static PropertyValidator GetPropertyValidator(Type type, string propertyName) {
			MemberInfo memberInfo = type.GetProperty(propertyName);
			if(memberInfo == null)
				return null;
			return PropertyValidator.FromAttributes(GetAllAttributes(memberInfo), propertyName);
		}
		static Attribute[] GetAllAttributes(MemberInfo member) {
			return MetadataHelper
				.GetAllAttributes(member);
		}
	}
}
