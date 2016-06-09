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

using System.Reflection;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public interface ITestControls {
		object FindControl(string controlType, string name, string fullName);
		string[] FindControlsFullName(string controlType, string name, string fullName);
		string GetTestControlsDescriptions();
		object CreateControl(string controlClassName, string id, string fullName);
		object ExecuteScript(string scriptText);
		void AddScript(string scriptText, string scriptName, string paramNames);
	}
	public class TestControlsImpl : ITestControls {
		private readonly IReflect testControls;
		public TestControlsImpl(IReflect testControls) {
			this.testControls = testControls;
		}
		private object InvokeMethod(string name, params object[] args) {
			return testControls.InvokeMember(name, BindingFlags.InvokeMethod | WebCommandAdapter.CommonBindingFlags, null, testControls, args, null, null, null);
		}
		public object FindControl(string controlType, string name, string fullName) {
			EasyTestTracer.Tracer.InProcedure("FindControl");
			object result = InvokeMethod("FindControl", controlType, name, fullName);
			EasyTestTracer.Tracer.OutProcedure("FindControl");
			return result;
		}
		public string[] FindControlsFullName(string controlType, string name, string fullName) {
			EasyTestTracer.Tracer.InProcedure("FindControlsFullName: " + controlType + ", " + name + ", " + fullName);
			string foundControlNames = InvokeMethod("FindControlsFullName", controlType, name, fullName) as string;
			EasyTestTracer.Tracer.OutProcedure("FindControlsFullName");
			return string.IsNullOrEmpty(foundControlNames) ? null : foundControlNames.Split(';');
		}
		public string GetTestControlsDescriptions() {
			return InvokeMethod("GetTestControlsDescriptions") as string;
		}
		public object CreateControl(string controlClassName, string id, string fullName) {
			return InvokeMethod("CreateControl", controlClassName, id, fullName);
		}
		public object ExecuteScript(string scriptText) {
			return InvokeMethod("ExecuteScript", scriptText);
		}
		public void AddScript(string scriptText, string scriptName, string paramNames) {
			InvokeMethod("AddScript", scriptText, scriptName, paramNames);
		}
	}
}
