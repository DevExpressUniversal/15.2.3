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
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Reflection;
namespace DevExpress.Charts.Designer.Native {
	public static class CommandUtils {
		static PropertyInfo GetPropertyInfo(string propertyPath, ref object targetObject) {
			string[] pathes = propertyPath.Split('.');
			string[] objectPath = new string[pathes.Length - 1];
			Array.Copy(pathes, 0, objectPath, 0, pathes.Length - 1);
			string targetPropertyName = pathes[pathes.Length - 1];
			foreach (string propertyPathPart in objectPath) {
				Type elementType = targetObject.GetType();
				PropertyInfo propertyInfo = elementType.GetProperty(propertyPathPart);
				var cacheTargetObject = targetObject; 
				targetObject = propertyInfo.GetValue(targetObject, new object[0]);
				if (targetObject == null) {
					propertyInfo = elementType.GetProperty("Actual" + propertyPathPart);
					targetObject = propertyInfo.GetValue(cacheTargetObject, new object[0]);
				}
			}
			Type objectType = targetObject.GetType();
			return objectType.GetProperty(targetPropertyName);
		}
		static IModelItem GetModelItem(IModelItem modelItem, string propertyName) {
			string[] pathes = propertyName.Split('.');
			IModelItem targetItem = modelItem;
			string[] objectPath = new string[pathes.Length - 1];
			Array.Copy(pathes, 0, objectPath, 0, pathes.Length - 1);
			foreach (string propertyPath in objectPath)
				targetItem = targetItem.Properties[propertyPath].Value;
			return targetItem;
		}
		public static int ExtractInt(object value, int defaultValue = 0) {
			if (value != null) {
				if (value.GetType() == typeof(string)) {
					int result;
					if (int.TryParse((string)value, out result))
						return result;
					return defaultValue;
				}
				else if (value.GetType() == typeof(int)) {
					return (int)value;
				}
				else if (value.GetType() == typeof(decimal)) {
					return (int)(decimal)value;
				}
				else if (value.GetType() == typeof(float)) {
					return (int)(float)value;
				}
				else if (value.GetType() == typeof(double)) {
					return (int)(double)value;
				}
				else if (value.GetType() == typeof(long)) {
					return (int)(long)value;
				}
			}
			return defaultValue;
		}
		public static object GetObjectProperty(object targetObject, string propertyPath) {
			PropertyInfo targetPropertyInfo = GetPropertyInfo(propertyPath, ref targetObject);
			return targetPropertyInfo.GetValue(targetObject, new object[0]);
		}
		public static void SetObjectProperty(object targetObject, string propertyPath, object value) {
			PropertyInfo targetPropertyInfo = GetPropertyInfo(propertyPath, ref targetObject);
			targetPropertyInfo.SetValue(targetObject, value, new object[0]);
		}
		public static IModelItem GetModelItemProperty(IModelItem modelItem, string propertyName) {
			string[] pathes = propertyName.Split('.');
			IModelItem targetItem = GetModelItem(modelItem, propertyName);
			string targetPropertyName = pathes[pathes.Length - 1];
			return targetItem.Properties[targetPropertyName].Value;
		}
		public static void SetModelItemProperty(IModelItem modelItem, string propertyName, object value) {
			string[] pathes = propertyName.Split('.');
			IModelItem targetItem = GetModelItem(modelItem, propertyName);
			string targetPropertyName = pathes[pathes.Length - 1];
			targetItem.Properties[targetPropertyName].SetValue(value);
		}
	}
}
