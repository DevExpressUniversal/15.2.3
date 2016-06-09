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
using DevExpress.Xpf.AssemblyLoader;
namespace DevExpress.Xpf.Utils {
	public class AssemblyLoaderProxy : IAssemblyLoader {
		static AssemblyLoaderProxy instance;
		protected AssemblyLoaderProxy() { }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public object Loader { get ; set; }
		public static AssemblyLoaderProxy Instance {
			get {
				if (instance == null) {
					instance = new AssemblyLoaderProxy();
				}
				return instance;
			}
		}
		public Action<object> CompletedCallback {
			get {
				if(!IsAvailable)
					return null;
				return (Action<object>)ReflectorHelper.GetValue(Loader, "CompletedCallback");
			}
			set {
				if(!IsAvailable)
					return;
				ReflectorHelper.SetValue(Loader, "CompletedCallback", value);
			}
		}
		public Action<double> ProgressChangedCallback {
			get {
				if(!IsAvailable)
					return null;
				return (Action<double>)ReflectorHelper.GetValue(Loader, "ProgressChangedCallback");
			}
			set {
				if(!IsAvailable)
					return;
				ReflectorHelper.SetValue(Loader, "ProgressChangedCallback", value);
			}
		}
		public bool IsAvailable { get { return Loader != null; } }
		public bool CanLoadAssembly(string name) {
			if(!IsAvailable)
				return false;
			MethodInfo methodInfo = ReflectorHelper.GetMethod(Loader.GetType(), "CanLoadAssembly");
			return (bool)methodInfo.Invoke(Loader, new object[] { name });
		}
		public void LoadAssembly(string name) {
			if(!IsAvailable)
				return;
			MethodInfo methodInfo = ReflectorHelper.GetMethod(Loader.GetType(), "LoadAssembly");
			methodInfo.Invoke(Loader, new object[] { name });
		}
	}
	public static class ReflectorHelper {
		public static PropertyInfo GetProperty(Type type, string propertyName, BindingFlags flags) {
			return type.GetProperty(propertyName, flags);
		}
		public static PropertyInfo GetProperty(Type type, string propertyName) {
			return GetProperty(type, propertyName, BindingFlags.Public | BindingFlags.Instance);
		}
		public static PropertyInfo GetStaticProperty(Type type, string propertyName) {
			return type.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static);
		}
		public static MethodInfo GetMethod(Type type, string methodName) {
			return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
		}
		public static MethodInfo GetStaticMethod(Type type, string methodName) {
			return type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static);
		}
		public static object GetValue(object obj, string propertyName, BindingFlags flags) {
			return GetProperty(obj.GetType(), propertyName, flags).GetValue(obj, null);
		}
		public static object GetValue(object obj, string propertyName) {
			return GetProperty(obj.GetType(), propertyName).GetValue(obj, null);
		}
		public static void SetValue(object obj, string propertyName, object value) {
			GetProperty(obj.GetType(), propertyName).SetValue(obj, value, null);
		}
	}
}
