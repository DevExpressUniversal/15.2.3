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
using System.Collections.ObjectModel;
using DevExpress.EasyTest.Framework;
using System.Text;
using System.Reflection;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls {
	public class Singleton<T> where T : new() {
		private static T instance;
		public static void SetInstance(T instance_) {
			instance = instance_;
		}
		public static T Instance {
			get {
				if(instance == null) {
					instance = new T();
				}
				return instance;
			}
		}
	}
	public class TestControlFactoryWeb : TestControlFactory<TestControlFactoryWeb> {
		private string JScriptPrefix = Environment.NewLine +
			@"(function() { function executeScript() {" + Environment.NewLine;
		private string JScriptPostfix = Environment.NewLine +
			@"} EasyTestScriptLoader.LoadScript('EasyTestJScripts', executeScript);})();" + Environment.NewLine;
		static TestControlFactoryWeb() {
			SetInstance(new TestControlFactoryWeb());
		}
		public TestControlFactoryWeb() {
			RegisterInterface<BaseWebTestControl>();
			RegisterInterface<JSStandartTestControl>();
			RegisterInterface<JSASPxStandardTestControl>();
			RegisterInterface<JSNavigationTestControl>();
			RegisterInterface<JSPivotGridTestControl>();
			RegisterInterface<JSImagePropertyEditorTestControl>();
			RegisterInterface<JSTabsTestControl>();
			RegisterInterface<JSNavigationTabsTestControl>();
			RegisterInterface<JSFileDataTestControl>();
		}
		public override ITestControl CreateControl(object control) {
			LoadScript((IControlDescription)control);
			return base.CreateControl(control);
		}
		private Dictionary<Type, string> _registredInterfaces = new Dictionary<Type,string>();
		private string GetJSScriptBaseTypes(Dictionary<string, string> baseJSTypes) {
			string result = Environment.NewLine;
			foreach(KeyValuePair<string, string> baseJSClass in baseJSTypes) {
				result += string.Format("{0}.inherit = {1};{2}", baseJSClass.Key, baseJSClass.Value, Environment.NewLine);
			}
			return result;
		}
		private void LoadScript(IControlDescription control) {
			string scriptText = "";
			scriptText += GetScript("DevExpress.ExpressApp.EasyTest.WebAdapter.TestControls.EasyTestScriptLoader.js");
			scriptText += JScriptPrefix;
			Dictionary<string, string> baseJSTypes = new Dictionary<string, string>();
			foreach(Type item in TestControlFactoryWeb.Instance.RegistredInterfaces) {
				IWebTestControl obj = (IWebTestControl)Activator.CreateInstance(item, new object[] { control });
				if(!_registredInterfaces.ContainsKey(item)) {
					_registredInterfaces.Add(item, obj.RegisterControlType);
				}
				scriptText += Environment.NewLine;
				scriptText += "/*" + obj.RegisterControlType + "*/" + Environment.NewLine;
				scriptText += obj.ScriptText;
				if(!string.IsNullOrEmpty(obj.JSControlBaseName) && !string.IsNullOrEmpty(obj.JSControlName)) {
					baseJSTypes[obj.JSControlName] = obj.JSControlBaseName;
				}
			}
			scriptText += GetJSScriptBaseTypes(baseJSTypes);
			scriptText += JScriptPostfix;
			if(control.ScriptExecutor != null) {
				control.ScriptExecutor.AddScript(scriptText, "", "");
			}
		}
		public override void UnRegisterInterface<InterfaceType>() {
			base.UnRegisterInterface<InterfaceType>();
			_registredInterfaces.Remove(typeof(InterfaceType));
		}
		private string GetScript(string resourceName) {
			Assembly ass = this.GetType().Assembly;
			System.IO.Stream stream = ass.GetManifestResourceStream(resourceName);
			System.IO.StreamReader reader = new System.IO.StreamReader(stream, Encoding.UTF8);
			return reader.ReadToEnd();
		}
		protected override IEnumerable<KeyValuePair<Type, string>> GetCompatibleInterfaceImplementers(object controlType) {
			IList<string> types = (IList<string>)controlType;
			Dictionary<Type, string> compatibleTypes = new Dictionary<Type, string>();
			foreach(KeyValuePair<Type, string> item in _registredInterfaces) {
				foreach(string typeFullName in types) {
					if(item.Value == typeFullName) {
						compatibleTypes.Add(item.Key, item.Value);
					}
				}
			}
			return compatibleTypes;
		}
	}
	public class TestControlFactory<T> : Singleton<T> where T : TestControlFactory<T>, new() {
		private List<Type> registredInterfaces = new List<Type>();
		private IList<Type> GetBaseTypes(Type controlType) {
			List<Type> baseTypes = new List<Type>();
			Type type = controlType;
			while(type != null) {
				baseTypes.Add(type);
				type = type.BaseType;
			}
			return baseTypes;
		}
		protected virtual IEnumerable<KeyValuePair<Type, string>> GetCompatibleInterfaceImplementers(object controlType) {
			Dictionary<Type, string> compatibleTypes = new Dictionary<Type, string>();
			return compatibleTypes;
		}
		protected virtual IEnumerable<KeyValuePair<Type, Type>> FindCompatibleInterfaces(string controlType) {
			Dictionary<Type, Type> compatibleInterfaces = new Dictionary<Type, Type>();
			Dictionary<Type, string> interfaceImplementerTypeToControlType = new Dictionary<Type, string>();
			IList<string> baseTypes = controlType.Split(';');
			IEnumerable<KeyValuePair<Type, string>> compatibleTypes = GetCompatibleInterfaceImplementers(baseTypes);
			foreach(KeyValuePair<Type, string> item in compatibleTypes) {
				Type implementerType = item.Key;
				Type[] interfaces = implementerType.GetInterfaces();
				foreach(Type currentInterface in interfaces) {
					string compatibleControlType = item.Value;
					Type interfaceImplementer = null;
					if(compatibleInterfaces.TryGetValue(currentInterface, out interfaceImplementer)) {
						string currentControlType = interfaceImplementerTypeToControlType[currentInterface];
						if(baseTypes.IndexOf(currentControlType) > baseTypes.IndexOf(compatibleControlType)) {
							compatibleInterfaces[currentInterface] = implementerType;
						}
					}
					else {
						compatibleInterfaces.Add(currentInterface, implementerType);
						interfaceImplementerTypeToControlType[currentInterface] = compatibleControlType;
					}
				}
			}
			return compatibleInterfaces;
		}
		protected virtual void OnCustomizeInterfaceImplementer(object interfaceImplementer) {
			if(CustomizeInterfaceImplementer != null) {
				CustomizeInterfaceImplementer(interfaceImplementer, EventArgs.Empty);
			}
		}
		public event EventHandler CustomizeInterfaceImplementer;
		protected virtual Type GetControlGetType(object control) {
			return control.GetType();
		}
		public virtual ITestControl CreateControl(object obj) {
			IControlDescription controlDescription = (IControlDescription)obj;
			EasyTestTracer.Tracer.LogText("CreateControl: " + controlDescription.ControlType);
			WebTestControlBase result = new WebTestControlBase(controlDescription);
			bool isFound = false;
			foreach(KeyValuePair<Type, Type> item in FindCompatibleInterfaces(controlDescription.ControlType)) {
				object interfaceImplementer = item.Value.GetConstructors()[0].Invoke(new object[] { controlDescription });
				OnCustomizeInterfaceImplementer(interfaceImplementer);
				if(interfaceImplementer is ITestControlContainer) {
					((ITestControlContainer)interfaceImplementer).TestControl = result;
				}
				result.AddInterface(item.Key, interfaceImplementer);
				isFound = true;
			}
			TestControlWrapper wrappedTestControl = new TestControlWrapper(controlDescription, result);
			if(isFound) {
				return wrappedTestControl;
			}
			return null;
		}
		public void RegisterInterface<InterfaceType>() where InterfaceType : BaseWebTestControl {
			registredInterfaces.Add(typeof(InterfaceType));
		}
		public virtual void UnRegisterInterface<InterfaceType>() {
			registredInterfaces.Remove(typeof(InterfaceType));
		}
		public ReadOnlyCollection<Type> RegistredInterfaces {
			get {
				return new ReadOnlyCollection<Type>(registredInterfaces);
			}
		}
	}
	public class TestControlWrapper : WebTestControlBase {
		private WebTestControlBase testControl;
		public override IEnumerable<KeyValuePair<Type, object>> GetAvailalbeInterfaces() {
			return testControl.GetAvailalbeInterfaces();
		}
		public TestControlWrapper(IControlDescription controlDescription, WebTestControlBase control)
			: base(controlDescription) {
			this.testControl = control;
			controlTypeName = control.controlTypeName;
		}
		public override InterfaceType FindInterface<InterfaceType>() {
			InterfaceType result = base.FindInterface<InterfaceType>();
			if(result != null) {
				return result;
			}
			else {
				return testControl.FindInterface<InterfaceType>();
			}
		}
		public override string Name {
			get { return testControl.Name; }
			set {
				testControl.Name = value;
			}
		}
	}
}
