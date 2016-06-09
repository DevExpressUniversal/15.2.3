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
using DevExpress.EasyTest.Framework;
using mshtml;
namespace DevExpress.ExpressApp.EasyTest.WebAdapter {
	public class ControlFinder {
		private readonly WebAdapter adapter;
		public static string TestTagToAttributName(string controlType) {
			return "test" + controlType.ToLower();
		}
		private object FindInRegisteredControls(ITestControls testControls, string controlType, string controlName) {
			EasyTestTracer.Tracer.InProcedure("FindInRegisteredControls");
			string controlFullName = GetTestControlFullName(testControls, controlType, controlName);
			EasyTestTracer.Tracer.LogText("TestControlFullName: " + controlFullName);
			object result = null;
			if(!string.IsNullOrEmpty(controlFullName)) {
				result = CreateFoundControl(testControls, controlType, controlFullName);
			}
			EasyTestTracer.Tracer.OutProcedure("FindInRegisteredControls");
			return result;
		}
		private string GetTestControlFullName(ITestControls testControls, string controlType, string controlName) {
			EasyTestTracer.Tracer.InProcedure("GetTestControlFullName");
			string result = null;
			string errorMessage = null;
			if(string.IsNullOrEmpty(controlName)) {
				string[] defaultTestControls = testControls.FindControlsFullName(controlType, null, null);
				if(defaultTestControls != null && defaultTestControls.Length > 0) {
					if(defaultTestControls.Length == 1) {
						result = defaultTestControls[0];
					}
					else {
						errorMessage = string.Format("Multiple controls with the same {0} control type detected. Use one of the following fully qualified control names: {1}.", controlType, string.Join(", ", defaultTestControls));
					}
				}
			}
			else {
				string[] controlsByName = testControls.FindControlsFullName(controlType, controlName, null);
				if(controlsByName != null && controlsByName.Length == 1) {
					result = controlsByName[0];
				}
				else {
					string[] controlsByFullName = testControls.FindControlsFullName(controlType, null, controlName);
					if(controlsByFullName != null && controlsByFullName.Length == 1) {
						result = controlsByFullName[0];
					}
					else {
						string[] controlFullNames = controlsByName;
						if(controlFullNames == null || controlsByName.Length == 0) {
							controlFullNames = controlsByFullName;
						}
						if(controlFullNames != null && controlFullNames.Length > 0) {
							errorMessage = string.Format("Ambiguous '{0}' control reference. Use one of the following fully qualified control names: {1}.", controlName, string.Join(", ", controlFullNames));
						}
					}
				}
			}
			if(errorMessage != null) {
				LogTestControlsDescriptions(testControls.GetTestControlsDescriptions());
				throw new AdapterOperationException(errorMessage);
			}
			EasyTestTracer.Tracer.OutProcedure("GetTestControlFullName");
			return result;
		}
		private object CreateFoundControl(ITestControls testControls, string controlType, string controlFullName) {
			EasyTestTracer.Tracer.InProcedure("CreateFoundControl");
			Object testControl = testControls.FindControl(controlType, null, controlFullName);
			EasyTestTracer.Tracer.OutProcedure("CreateFoundControl");
			return testControl;
		}
		private IHTMLElement FindHTMLElement(string controlType, string name) {
			IHTMLElement element = FindElementbyAttribute(controlType, name);
			if(element == null) {
				adapter.WaitForBrowserResponse();
				element = FindElementbyAttribute(controlType, name);
			}
			return element;
		}
		private IHTMLElement FindElementbyAttribute(string controlType, string name) {
			int browserIndex = adapter.WebBrowsers.Count - 1;
			if(browserIndex >= 0) {
				try {
					IHTMLDocument2 htmlDocument = adapter.GetDocument(browserIndex);
					IHTMLElement htmlElement = null;
					for(int i = 0; i < htmlDocument.frames.length; i++) {
						object pvarIndex = i;
						IHTMLWindow2 frameHtmlWindow = (IHTMLWindow2)htmlDocument.frames.item(ref pvarIndex);
						if(!string.IsNullOrEmpty(frameHtmlWindow.document.url)) {
							htmlElement = FindElementbyAttribute(frameHtmlWindow.document, controlType, name);
							if(htmlElement != null) {
								break;
							}
						}
					}
					if(htmlElement == null) {
						htmlElement = FindElementbyAttribute(htmlDocument, controlType, name);
					}
					return htmlElement;
				}
				catch { }
			}
			return null;
		}
		private IHTMLElement FindElementbyAttribute(IHTMLDocument2 htmlDocument, string controlType, string name) {
			if(IsFindingPossible(htmlDocument.body.innerHTML, controlType, name)) {
				return FindElementbyAttribute(htmlDocument.all, controlType, name);
			}
			return null;
		}
		private bool IsFindingPossible(string innerHTML, string controlType, string name) {
			if(innerHTML != null) { 
				string testTag = TestTagToAttributName(controlType);
				return name == "" || innerHTML.Contains(string.Format("{0}=\"{1}\"", testTag, name));
			}
			return false;
		}
		private IHTMLElement FindElementbyAttribute(IHTMLElementCollection collection, string controlType, string name) {
			string testTag = TestTagToAttributName(controlType);
			for(int i = 0; i < collection.length; i++) {
				IHTMLElement element = (IHTMLElement)collection.item(i, i);
				if(element != null) {
					object att = element.getAttribute(testTag, 0);
					if(att != null && att.ToString() != "" && (name == "" || att.ToString() == name)) {
						return element;
					}
				}
			}
			return null;
		}
		public ControlFinder(WebAdapter adapter) {
			this.adapter = adapter;
		}
		public object FindControl(ITestControls testControls, string controlType, string name) {
			object result = FindInRegisteredControls(testControls, controlType, name);
			if((result == null || result == DBNull.Value) && adapter != null) {
				LogTestControlsDescriptions(testControls.GetTestControlsDescriptions());
				EasyTestTracer.Tracer.LogText("FindInRegisteredControls return null");
				EasyTestTracer.Tracer.LogText("ControlType: {0}, Name: {1}", controlType, name);
				result = null;
				IHTMLElement element = FindHTMLElement(controlType, name);
				if(element != null) {
					string controlClassName = element.getAttribute("testControlClassName", 0) as string;
					string clientID = string.IsNullOrEmpty(element.id) ? element.getAttribute("name", 0) as string : element.id;
					if(!string.IsNullOrEmpty(clientID) && !string.IsNullOrEmpty(controlClassName)) {
						result = testControls.CreateControl(controlClassName, clientID, name);
					}
				}
			}
			return result;
		}
		public void LogTestControlsDescriptions(string descriptions) {
			if(string.IsNullOrEmpty(descriptions)) {
				EasyTestTracer.Tracer.LogText("TestControlsDescriptions IS NULL");
			}
			else {
				EasyTestTracer.Tracer.LogText("TestControlsDescriptions ->");
				foreach(string control in descriptions.Split(';')) {
					EasyTestTracer.Tracer.LogText("                           " + control);
				}
				EasyTestTracer.Tracer.LogText("TestControlsDescriptions <-");
			}
		}
	}
	public class TestControlsNotInitializedException : WarningException {
		const string ErrorMessage = "The TestControls variable isn't initialized";
		public TestControlsNotInitializedException() : base(ErrorMessage) { }
		public TestControlsNotInitializedException(string additionalMessage) : base(additionalMessage + " " + ErrorMessage) { }
	}
}
