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
using System.Reflection;
using System.Windows;
namespace DevExpress.Mvvm.UI.Native {
	public static class ObjectPropertyHelper {
		public static PropertyInfo GetPropertyInfoSetter(object obj, string propName) {
			return GetPropertyInfo(obj, propName, BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.Instance);
		}
		public static PropertyInfo GetPropertyInfoGetter(object obj, string propName) {
			return GetPropertyInfo(obj, propName, BindingFlags.Public | BindingFlags.GetProperty | BindingFlags.Instance);
		}
		public static PropertyInfo GetPropertyInfo(object obj, string propName, BindingFlags flags) {
			if(string.IsNullOrWhiteSpace(propName) || obj == null)
				return null;
			return obj.GetType().GetProperty(propName, flags);
		}
		public static DependencyProperty GetDependencyProperty(object obj, string propName) {
			if(string.IsNullOrWhiteSpace(propName) || obj == null)
				return null;
			Type objType = obj.GetType();
			FieldInfo field = objType.GetField(propName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			field = field ?? objType.GetField(propName + "Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
			return field == null ? null : (DependencyProperty)field.GetValue(obj);
		}
	}
}
