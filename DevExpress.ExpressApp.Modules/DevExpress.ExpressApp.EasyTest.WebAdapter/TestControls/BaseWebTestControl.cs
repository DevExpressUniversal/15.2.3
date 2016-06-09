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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.EasyTest.Framework;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public interface IControlDescription {
		IScriptExecutor ScriptExecutor { get; }
		string ControlType { get; }
		string ControlParameters { get; }
	}
	public class WebEasyTestControlDescription : IControlDescription {
		#region IControlDescription Members
		IScriptExecutor scriptExecutor;
		string controlType;
		string controlParameters;
		public WebEasyTestControlDescription(IScriptExecutor scriptExecutor, string controlType, string controlParameters) {
			this.scriptExecutor = scriptExecutor;
			this.controlType = controlType;
			this.controlParameters = controlParameters;
		}
		public IScriptExecutor ScriptExecutor {
			get { return scriptExecutor; }
		}
		public string ControlType {
			get { return controlType; }
		}
		public string ControlParameters {
			get { return controlParameters; }
		}
		#endregion
	}
	public interface IWebTestControl {
		string ScriptText { get; }
		string RegisterControlType { get; }
		string JSControlName { get; }
		string JSControlBaseName { get; }
	}
	public class BaseWebTestControl : IWebTestControl {
		IControlDescription controlDescription;
		JSTestControl jsTestControl = null;
		public BaseWebTestControl(IControlDescription controlDescription) {
			this.controlDescription = controlDescription;
		}
		protected string GetScriptName(string postfix) {
			return postfix;
		}
		protected object ExecuteFunction(string functionName, object[] param) {
			string script = String.Format(
@"
var control = new {0}({1});
control.className = '{2}';
var wrappedControl = new WrappedClass_JS(control);
return wrappedControl;", JSControlName, controlDescription.ControlParameters, GetType().Name);
			if(jsTestControl == null) {
				IReflect control = (IReflect)controlDescription.ScriptExecutor.ExecuteScript(script);
				jsTestControl = new JSTestControl(Adapter, control);
			}
			return jsTestControl.InvokeMethod(functionName,
								delegate(object[] args) {
									throw new EasyTestException(string.Format("The '{0}' method is not implemented.", functionName));
								}, param);
		}
		protected T ExecuteFunction<T>(string functionName, object[] param) {
			object functionResult = ExecuteFunction(functionName, param);
			if((functionResult != null) && !typeof(T).IsAssignableFrom(functionResult.GetType())) {
				throw new InvalidOperationException(string.Format("Error occurs while executing function '{0}': the '{1}' result type is not descendant of '{2}'", functionName, functionResult.GetType(), typeof(T)));
			}
			return (T)functionResult;
		}
		protected virtual string ScriptText {
			get {
				return ReadScriptFormResource("DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls.TestControlBase.js");
			}
		}
		protected virtual string JSControlName { 
			get { return "TestControlBase_JS"; } 
		}
		protected string JSControlBaseName {
			get {
				Type baseType = GetType().BaseType;
				if(baseType != typeof(object)) {
					BaseWebTestControl baseTestControl = Activator.CreateInstance(baseType, controlDescription) as BaseWebTestControl;
					if(baseTestControl != null) {
						return baseTestControl.JSControlName;
					}
				}
				return null;
			}
		}
		protected virtual string RegisterControlType { get { return "BaseTestObject"; } }
		protected string ReadScriptFormResource(string resourceName) {
			Assembly ass = this.GetType().Assembly;
			System.IO.Stream stream = ass.GetManifestResourceStream(resourceName);
			System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
			return reader.ReadToEnd();
		}
		public WebCommandAdapter Adapter {
			get { return (WebCommandAdapter)controlDescription.ScriptExecutor; }
		}
		#region IWebTestControl Members
		string IWebTestControl.ScriptText {
			get { return ScriptText; }
		}
		string IWebTestControl.RegisterControlType {
			get { return RegisterControlType; }
		}
		string IWebTestControl.JSControlName {
			get { return JSControlName; }
		}
		string IWebTestControl.JSControlBaseName {
			get { return JSControlBaseName; }
		}
		#endregion
	}
}
