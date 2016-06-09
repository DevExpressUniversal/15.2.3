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
using System.Text;
using System.Reflection;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class ControlReflectWrapper {
		private IReflect controlReflect = null;
		public ControlReflectWrapper(IReflect controlReflect) {
			this.controlReflect = controlReflect;
		}
		public MethodInfo GetMethodInfo(string methodName) {
			return controlReflect.GetMethod(methodName, WebCommandAdapter.CommonBindingFlags);
		}
		public object GetMethodValue(string methodName, BindingFlags bindingFlags, object[] arg) {
			object result = null;
			MethodInfo mi = controlReflect.GetMethod(methodName, bindingFlags);
			if(mi != null) {
				result = controlReflect.GetType().InvokeMember(methodName, System.Reflection.BindingFlags.InvokeMethod, null, controlReflect, arg);
			}
			return result;
		}
		public void SetPropertyValue(string propertyName, BindingFlags commonBindingFlags, object value) {
			PropertyInfo p = controlReflect.GetProperty(propertyName, commonBindingFlags);
			controlReflect.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.SetProperty, null, controlReflect, new object[] { value });
		}
		public object GetPropertyValue(string propertyName, BindingFlags bindingFlags) {
			object result = null;
			PropertyInfo idPropertyInfo = controlReflect.GetProperty(propertyName, bindingFlags);
			if(idPropertyInfo != null) {
				result = controlReflect.GetType().InvokeMember(propertyName, System.Reflection.BindingFlags.GetProperty, null, controlReflect, null);
			}
			return result;
		}
	}
}
