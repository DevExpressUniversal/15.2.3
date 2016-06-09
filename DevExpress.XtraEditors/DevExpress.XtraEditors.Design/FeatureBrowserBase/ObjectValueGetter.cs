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
using System.ComponentModel;
using System.Reflection;
namespace DevExpress.Utils.Design {
	public class ObjectValueGetter	{
		object sourceObject;
		object getterObject;
		string sourceProperty;
		public ObjectValueGetter(object sourceObject) : this(sourceObject, string.Empty) {
		}
		public ObjectValueGetter(object sourceObject, string sourceProperty) {
			this.sourceObject = sourceObject;
			this.sourceProperty = sourceProperty;
			GetPropertyDescriptor(SourceObject, SourceProperty, out this.getterObject);
			if(this.getterObject == null)
				this.getterObject = this.sourceObject;
		}
		public object SourceObject { get { return sourceObject; } }
		public string SourceProperty { get { return sourceProperty; } }
		public object GetterObject { get { return getterObject; } }
		public object GetValue(string propertyName) {
			object obj;
			GetPropertyDescriptor(propertyName, out obj);
			return obj;
		}
		public PropertyDescriptor GetPropertyDescriptor(string propertyName) {
			object dummy;
			return GetPropertyDescriptor(GetterObject, propertyName, out dummy);
		}
		public PropertyDescriptor GetPropertyDescriptor(string propertyName, out object obj) {
			return GetPropertyDescriptor(GetterObject, propertyName, out obj);
		}
		PropertyDescriptor GetPropertyDescriptor(object obj, string propertyName, out object resObj) {
			resObj = null;
			PropertyDescriptor propertyDescriptor = null;
			if(propertyName == null || propertyName == string.Empty) return propertyDescriptor;
			resObj = obj;
			string[] names = propertyName.Split(new char[] {'.'});
			for(int i = 0; i < names.Length; i ++) {
				propertyDescriptor = GetPropertyDescriptor(resObj, names[i]);
				if(propertyDescriptor == null) { 
					resObj = null;
					return null;
				}
				resObj = propertyDescriptor.GetValue(resObj);
				if(resObj == null) return propertyDescriptor;
			}
			return propertyDescriptor;
		}
		PropertyDescriptor GetPropertyDescriptor(object obj, string propName) {
			return TypeDescriptor.GetProperties(obj).Find(propName, true);
		}
	}
}
