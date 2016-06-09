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
using System.Collections;
using System.ComponentModel;
using System.Reflection;
namespace DevExpress.XtraPrinting {
	public class Accessor {
		public static object GetProperty(object obj, string name) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(obj)[name];
			return descriptor.GetValue(obj);
		}
		public static void SetProperty(object obj, string name, object value) {
			PropertyDescriptor descriptor = TypeDescriptor.GetProperties(obj)[name];
			descriptor.SetValue(obj, value);
		}
		public static void GetProperties(object obj, Hashtable ht) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
			foreach(PropertyDescriptor descriptor in properties) {
				object value = descriptor.GetValue(obj);
				string name = descriptor.Name;
				ht.Add(name, value);
			}
		}
		public static void SetProperties(object obj, Hashtable ht) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
			foreach(string name in ht.Keys) {
				PropertyDescriptor descriptor = properties[name];
				if(descriptor != null)
					descriptor.SetValue(obj, ht[name]);
			}
		}
		public static void SetProperties(object obj, object[,] array) {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(obj);
			int length = ((Array)array).GetLength(0);
			for(int i = 0; i < length; i++) {
				string name = (string)array[i, 0];
				object value = array[i, 1];
				PropertyDescriptor descriptor = properties[name];
				if(descriptor != null)
					descriptor.SetValue(obj, value);
			}
		}
		public static object InvokeMethod(object obj, string name, object[] args) {
			Type t = obj.GetType();
			return t.InvokeMember(name, BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, obj, args);
		}
		public static PropertyDescriptor CreateProperty(Type type, PropertyDescriptor oldPropertyDescriptor, Attribute[] attributes) {
			if(oldPropertyDescriptor == null)
				return null;
			ArrayList attrs = new ArrayList(oldPropertyDescriptor.Attributes);
			attrs.AddRange(attributes);
			return TypeDescriptor.CreateProperty(type, oldPropertyDescriptor, (Attribute[])attrs.ToArray(typeof(Attribute)));
		}
		public static EventDescriptor CreateEvent(Type type, EventDescriptor oldEventDescriptor, Attribute[] attributes) {
			if(oldEventDescriptor == null)
				return null;
			ArrayList attrs = new ArrayList(oldEventDescriptor.Attributes);
			attrs.AddRange(attributes);
			return TypeDescriptor.CreateEvent(type, oldEventDescriptor, (Attribute[])attrs.ToArray(typeof(Attribute)));
		}
	}
}
