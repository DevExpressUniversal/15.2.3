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
using System.Collections.Generic;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public abstract class TestControlBase : ITestControl {
		private string name;
		private string controlType;
		#region ITestControl Members
		public virtual T FindInterface<T>() {
			if(this is T) {
				object result = this;
				return (T)result;
			}
			return default(T);
		}
		public T GetInterface<T>() {
			T result = FindInterface<T>();
			if(result == null) {
				throw new EasyTestException(string.Format("The '{0}' interface isn't supported by the {1}", typeof(T).Name, GetType().Name));
			}
			return result;
		}
		public string Name {
			get { return name; }
			set { name = value; }
		}
		public string ControlType {
			get { return controlType; }
			set { controlType = value; }
		}
		#endregion
		#region IDisposable Members
		public virtual void Dispose() {
		}
		#endregion
	}
	public class WebTestControlBase : ITestControl {
		public const string InterfaceIsntSupported = "The {0} interface is not supported for the control: {1}\r\nAvailable interfaces: {2}";
		private string name;
		private string testControlType;
		internal string controlTypeName;
		protected Dictionary<Type, object> interfaces;
		private string FormatAvailalbeInterfaces(IEnumerable<KeyValuePair<Type, object>> interfaces) {
			List<string> availalbeInterfaces = new List<string>();
			foreach(KeyValuePair<Type, object> interfaceDescription in interfaces) {
				availalbeInterfaces.Add(interfaceDescription.Key.FullName + "(" + interfaceDescription.Value.GetType().FullName + ")");
			}
			return string.Join(",\r\n", availalbeInterfaces.ToArray());
		}
		public WebTestControlBase(IControlDescription controlDescription) {
			this.controlTypeName = controlDescription.ControlType;
			this.interfaces = new Dictionary<Type, object>();
		}
		public void AddInterface(Type InterfaceType, object interfaceImpl) {
			interfaces[InterfaceType] = interfaceImpl;
		}
		public object FindInterface(Type interfaceType) {
			object result;
			interfaces.TryGetValue(interfaceType, out result);
			return result;
		}
		public virtual IEnumerable<KeyValuePair<Type, object>> GetAvailalbeInterfaces() {
			return interfaces;
		}
		#region ITestControl Members
		public virtual InterfaceType FindInterface<InterfaceType>() {
			object result;
			interfaces.TryGetValue(typeof(InterfaceType), out result);
			return (InterfaceType)result;
		}
		public InterfaceType GetInterface<InterfaceType>() {
			object result = FindInterface<InterfaceType>();
			if(result == null) {
				string availalbeInterfaces = FormatAvailalbeInterfaces(GetAvailalbeInterfaces());
				throw new WarningException(String.Format(InterfaceIsntSupported, typeof(InterfaceType).FullName, controlTypeName, availalbeInterfaces));
			}
			return (InterfaceType)result;
		}
		public virtual string Name {
			get { return name; }
			set { name = value; }
		}
		public virtual string ControlType {
			get { return testControlType; }
			set { testControlType = value; }
		}
		#endregion
	}
}
