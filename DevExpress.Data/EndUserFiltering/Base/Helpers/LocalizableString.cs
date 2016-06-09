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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Linq.Expressions;
	using System.Reflection;
	class LocalizableString {
		string propertyName;
		string propertyValue;
		Type resourceType;
		Func<string> accessor;
		public LocalizableString(string propertyName) {
			this.propertyName = propertyName;
		}
		public LocalizableString(Expression<Func<string>> propertyExpression)
			: this(ExpressionHelper.GetPropertyName(propertyExpression)) {
		}
		public string Value {
			get { return propertyValue; }
			set {
				if(propertyValue == value) return;
				propertyValue = value;
				ResetAccessor();
			}
		}
		public Type ResourceType {
			get { return this.resourceType; }
			set {
				if(this.resourceType == value) return;
				this.resourceType = value;
				this.ResetAccessor();
			}
		}
		public string GetLocalizableValue() {
			if(this.accessor == null) {
				if(resourceType == null)
					this.accessor = () => propertyValue;
				else {
					PropertyInfo property = resourceType.GetProperty(propertyValue ?? propertyName);
					if(IsBadlyConfigured(property)) {
						var exception = new LocalizableStringBadlyConfiguredException(propertyName, resourceType, propertyValue);
						this.accessor = () => { throw exception; };
					}
					else this.accessor = () => (string)property.GetValue(null, null);
				}
			}
			return this.accessor();
		}
		bool IsBadlyConfigured(PropertyInfo property) {
			if(!resourceType.IsVisible || property == null || property.PropertyType != typeof(string))
				return true;
			else {
				MethodInfo getter = property.GetGetMethod();
				if(getter == null || !(getter.IsPublic && getter.IsStatic))
					return true;
			}
			return false;
		}
		void ResetAccessor() {
			this.accessor = null;
		}
		class LocalizableStringBadlyConfiguredException : InvalidOperationException {
			const string LocalizableString_LocalizationFailed = "Cannot retrieve property '{0}' because localization failed.  Type '{1}' is not public or does not contain a public static string property with the name '{2}'.";
			public LocalizableStringBadlyConfiguredException(string propertyName, Type resourceType, string propertyValue)
				: base(string.Format(System.Globalization.CultureInfo.CurrentCulture, LocalizableString_LocalizationFailed, propertyName, resourceType.FullName, propertyValue ?? propertyName)) {
			}
		}
	}
}
