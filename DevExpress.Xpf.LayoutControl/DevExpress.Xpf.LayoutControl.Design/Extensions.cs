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

extern alias Platform;
using Microsoft.Windows.Design.Model;
using Platform::DevExpress.Xpf.LayoutControl;
namespace DevExpress.Xpf.LayoutControl.Design {
	using System;
	using LayoutControl = Platform::DevExpress.Xpf.LayoutControl.LayoutControl;
	static class Extensions {
		public static ModelItem GetLayoutControl(this ModelItem item, bool considerSelf) {
			ModelItem result = item;
			do {
				if (result.IsItemOfType(typeof(LayoutControl)) && (considerSelf || result != item))
					return result;
				result = result.Parent;
			}
			while (result != null);
			return result;
		}
		public static bool IsEmptyLayoutGroup(this ModelItem item) {
			return item.IsLayoutGroup() && item.IsEmptyPanel();
		}
		public static bool IsEmptyPanel(this ModelItem item) {
			return item.Properties["Children"].Collection.Count == 0;
		}
		public static bool IsLayoutGroup(this ModelItem item) {
			return item.IsItemOfType(typeof(LayoutGroup)) && !item.IsItemOfType(typeof(LayoutControl));
		}
		public static bool IsTabbedLayoutGroup(this ModelItem item) {
			return item.IsLayoutGroup() && item.Properties["View"].ComputedValue.Equals(LayoutGroupView.Tabs);
		}
		public static void ResetLayout(this ModelItem item) {
			using (ModelEditingScope editingScope = item.BeginEdit("Reset Layout")) {
				item.Properties["HorizontalAlignment"].ClearValue();
				item.Properties["VerticalAlignment"].ClearValue();
				item.Properties["Width"].ClearValue();
				item.Properties["Height"].ClearValue();
				editingScope.Complete();
			}
		}
		public static void SetValueIfNotEqual(this ModelProperty property, object value) {
			object currentValue = !property.IsSet && property.DefaultValue != null ? property.DefaultValue : property.ComputedValue;
			if (!currentValue.Equals(value))
				property.SetValue(value);
		}
		public static void SetValueIfNotSet(this ModelProperty property, object value) {
			if (!property.IsSet)
				property.SetValue(value);
		}
	}
	internal static class TypeExtensions {
		internal static Type GetNonNullableType(this Type type) {
			return type.IsNullable() ? type.GetGenericArguments()[0] : type;
		}
		internal static bool IsNullable(this Type type) {
			return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
		}
		internal static bool IsSimple(this Type type) {
			type = type.GetNonNullableType();
			return type.IsPrimitive || type.IsEnum || type == typeof(string) || type == typeof(DateTime) || type == typeof(Decimal);
		}
	}
}
