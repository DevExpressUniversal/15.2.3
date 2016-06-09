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

using DevExpress.Data.Helpers;
using DevExpress.Entity.Model;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Mvvm.UI.Native.ViewGenerator {
	public static class PropertyInfoExtensions {
		public static Type GetUnderlyingClrType(this IEdmPropertyInfo propertyInfo) {
			return Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;
		}
		public static bool HasNullableType(this IEdmPropertyInfo propertyInfo) {
			return Nullable.GetUnderlyingType(propertyInfo.PropertyType) != null;
		}
		public static string GetDisplayName(this IEdmPropertyInfo propertyInfo) {
			if (HasDisplayAttribute(propertyInfo))
				return propertyInfo.DisplayName;
			return MasterDetailHelper.SplitPascalCaseString(propertyInfo.Name);
		}
		public static bool HasDisplayAttribute(this IEdmPropertyInfo propertyInfo) {
			return propertyInfo.DisplayName != propertyInfo.Name;
		}
		public static IEnumerable<IEdmPropertyInfo> GetProperties(this IEntityProperties properties) {
			return properties.AllProperties.Where(x => !x.IsNavigationProperty);
		}
		public static IEnumerable<IEdmPropertyInfo> GetNavigationProperties(this IEntityProperties properties) {
			return properties.AllProperties.Where(x => x.IsNavigationProperty);
		}
		public static string GetActualMask(this IEdmPropertyInfo propertyInfo, string mask) {
			return (propertyInfo.Attributes.ApplyFormatInEditMode && !string.IsNullOrEmpty(propertyInfo.Attributes.DataFormatString)) ? propertyInfo.Attributes.DataFormatString : mask;
		}
	}
}
