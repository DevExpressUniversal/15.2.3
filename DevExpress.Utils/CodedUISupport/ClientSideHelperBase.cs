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
using System.Windows.Forms;
using System.Windows.Interop;
namespace DevExpress.Utils.CodedUISupport {
	[ToolboxItem(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ClientSideHelperBase : MarshalByRefObject {
		public RemoteObject remoteObject;
		public ClientSideHelperBase(RemoteObject remoteObject) {
			this.remoteObject = remoteObject;
		}
		public bool CheckConnection(IntPtr windowHandle) {
			return true;
		}
		protected T GetHelper<T>(string helperTypeName, string assemblyName) {
			try {
				Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
				foreach(Assembly assembly in assemblies)
					if(assembly.GetName().Name == assemblyName) {
						Type helperType = assembly.GetType(helperTypeName);
						if(helperType != null)
							return GetHelperFromType<T>(helperType);
						Type[] types = assembly.GetTypes();
						Type interfaceType = typeof(T);
						foreach(Type type in types)
							if(interfaceType.IsAssignableFrom(type) && interfaceType != type)
								return GetHelperFromType<T>(helperType);
						break;
					}
			}
			catch(System.Reflection.ReflectionTypeLoadException) {
			}
			catch(System.TypeLoadException) {
			}
			return default(T);
		}
		private T GetHelperFromType<T>(Type helperType) {
			return (T)Activator.CreateInstance(helperType, new object[] { this.remoteObject });
		}
		protected T RunClientSideMethod<T>(object helper, IntPtr controlHandle, Func<T> method) {
			if(helper == null)
				return default(T);
			else return CrossThreadRunMethod(controlHandle, method);
		}
		protected void RunClientSideMethod(object helper, IntPtr controlHandle, Action method) {
			Func<bool> newMethod = delegate() {
				method();
				return true;
			};
			CrossThreadRunMethod(controlHandle, newMethod);
		}
		protected T CrossThreadRunMethod<T>(IntPtr controlHandle, Func<T> method) {
			object result = InvokeMethod(controlHandle, method);
			if(result is T)
				return (T)result;
			else return default(T);
		}
		object InvokeMethod<T>(IntPtr controlHandle, Func<T> method) {
			try {
				Control control = Control.FromHandle(controlHandle);
				if(control != null) {
					if(control.IsDisposed || control.Disposing)
						return null;
					else
						return control.Invoke(method);
				}
				else {
					HwndSource hwndSource = HwndSource.FromHwnd(controlHandle);
					if(hwndSource != null) {
						if(hwndSource.IsDisposed || hwndSource.Dispatcher == null)
							return null;
						else
							return hwndSource.Dispatcher.Invoke(method);
					}
					else {
						bool methodExecuted = false;
						object returnValue = null;
						CodedUINativeMethods.EnumChildWindows(controlHandle, delegate(IntPtr childHandle, IntPtr lParam) {
							control = Control.FromHandle(childHandle);
							if(control != null && !control.IsDisposed && !control.Disposing) {
								methodExecuted = true;
								returnValue = control.Invoke(method);
								return false;
							}
							return true;
						}, IntPtr.Zero);
						if(!methodExecuted)
							return method();
						else
							return returnValue;
					}
				}
			}
			catch(ObjectDisposedException) {
				return null;
			}
		}
	}
}
