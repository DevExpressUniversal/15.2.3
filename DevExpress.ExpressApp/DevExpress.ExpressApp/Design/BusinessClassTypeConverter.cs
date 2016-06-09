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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Utils.Reflection;
namespace DevExpress.ExpressApp.Design {
	public static class DesignHelper {
		private static int AlphabeticalComparision(object x, object y) {
			if(x == null && y == null) {
				return 0;
			}
			else if(x == null && y != null) {
				return 1;
			}
			else if(x != null && y == null) {
				return -1;
			}
			else {
				return x.ToString().CompareTo(y.ToString());
			}
		}
		private static bool IsVisibleType(Type type) {
			bool isVisible = type.IsPublic;
			if(isVisible) {
				try {
					BrowsableAttribute attribute = FindAttribute<BrowsableAttribute>(type);
					isVisible = (attribute == null || attribute.Browsable);
				}
				catch {
					isVisible = false;
				}
			}
			return isVisible;
		}
		private static T FindAttribute<T>(Type type) where T : Attribute {
			T[] attributes = AttributeHelper.GetAttributes<T>(type, false);
			if(attributes.Length > 0) {
				return attributes[0];
			}
			return null;
		}
		public static ITypeDiscoveryService GetTypeDiscoveryService(IServiceProvider serviceProvider) {
			Guard.ArgumentNotNull(serviceProvider, "serviceProvider");
			ITypeDiscoveryService typeDiscoveryService = (ITypeDiscoveryService)serviceProvider.GetService(typeof(ITypeDiscoveryService));
			if(typeDiscoveryService == null) {
				throw new InvalidOperationException("Cannot find ITypeDiscoveryService");
			}
			return typeDiscoveryService;
		}
		public static IList FindBusinessClassDescendants<BusinessClass>(IServiceProvider serviceProvider, bool ignoreObsoleteAttribute) {
			return FindBusinessClassDescendants<BusinessClass>(GetTypeDiscoveryService(serviceProvider), ignoreObsoleteAttribute);
		}
		public static IList FindBusinessClassDescendants<BusinessClass>(ITypeDiscoveryService typeDiscoveryService, bool ignoreObsoleteAttribute) {
			Guard.ArgumentNotNull(typeDiscoveryService, "typeDiscoveryService");
			ICollection types = typeDiscoveryService.GetTypes(typeof(BusinessClass), false);
			EFDesignTimeTypeInfoHelper.InitTypesInfo(XafTypesInfo.Instance, typeDiscoveryService);
			List<Type> resultList = new List<Type>();
			foreach(Type type in types) {
				if(IsValidBusinessClass(type, ignoreObsoleteAttribute)) {
					resultList.Add(type);
				}
			}
			resultList.Sort(AlphabeticalComparision);
			return resultList;
		}
		public static bool IsValidBusinessClass(Type type, bool ignoreObsoleteAttribute) {
			return type != null
				&& IsVisibleType(type)
				&& (ignoreObsoleteAttribute || FindAttribute<ObsoleteAttribute>(type) == null)
				&& ExportedTypeHelpers.IsExportedType(type);
		}
	}
	public class BusinessClassTypeConverter<BaseType> : TypeListConverter {
		private const bool IgnoreObsoleteAttribute = true;
		public BusinessClassTypeConverter() : base(null) { }
		public override TypeConverter.StandardValuesCollection GetStandardValues(ITypeDescriptorContext context) {
			return new StandardValuesCollection(DesignHelper.FindBusinessClassDescendants<BaseType>(context, IgnoreObsoleteAttribute));
		}
		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value) {
			Object result = null;
			if(value is string) {
				string className = ((string)value).Trim();
				if(!String.IsNullOrEmpty(className) && !GetStandardValuesExclusive(context)) {
					ITypeResolutionService typeResolution = (ITypeResolutionService)((IServiceProvider)context).GetService(typeof(ITypeResolutionService));
					Type type = typeResolution.GetType(className);
					if(DesignHelper.IsValidBusinessClass(type, IgnoreObsoleteAttribute)) {
						result = type;
					}
					else {
						throw new ArgumentOutOfRangeException("value", className, "");
					}
				}
			}
			else {
				result = base.ConvertFrom(context, culture, value);
			}
			return result;
		}
		public override bool GetStandardValuesExclusive(ITypeDescriptorContext context) {
			return false;
		}
		public override bool GetStandardValuesSupported(ITypeDescriptorContext context) {
			return true;
		}
	}
}
