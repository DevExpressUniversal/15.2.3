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
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Reflection;
namespace DevExpress.XtraEditors.Design.TasksSolution {
	public class EventCreator {
		object sourceObject;
		TaskEvent taskEvent;
		EventDescriptor firstEvent;
		string firstHandler;
		DesignerTransaction transaction;
		public EventCreator(object sourceObject, TaskEvent taskEvent) {
			this.sourceObject = sourceObject;
			this.taskEvent = taskEvent;
		}
		public object SourceObject { get { return sourceObject; } }
		public Component SourceComponent { get { return SourceObject as Component; } }
		public TaskEvent TaskEvent { get { return taskEvent; } }
		public void Generate() {
			if(SourceComponent == null || SourceComponent.Site == null) return;
			IEventBindingService eventBindingService = (IEventBindingService)SourceComponent.Site.GetService(typeof(IEventBindingService));
			IDesignerHost host = (IDesignerHost)SourceComponent.Site.GetService(typeof(IDesignerHost));
			if(eventBindingService == null || host == null) return;
			this.firstEvent = null;
			this.firstHandler = null;
			this.transaction = null;
			Generate(TaskEvent, eventBindingService, host);
			if (transaction != null) {
				transaction.Commit();
			}
			if (firstHandler != null && firstEvent != null) {
				eventBindingService.ShowCode(SourceComponent, firstEvent);
			}
		}
		void Generate(TaskEvent taskEvent, IEventBindingService eventBindingService, IDesignerHost host) {
			Generate(taskEvent.EventName, eventBindingService, host);
			for(int i = 0; i < taskEvent.Count; i ++)
				Generate(taskEvent[i], eventBindingService, host);
		}
		void Generate(string eventName, IEventBindingService eventBindingService, IDesignerHost host) {
			if(eventName == string.Empty) return;
			EventDescriptor eventDescriptor = TypeDescriptor.GetEvents(SourceComponent).Find(eventName , true); 
			if(eventDescriptor == null) return;
			PropertyDescriptor propEvent = eventBindingService.GetEventProperty(eventDescriptor);
			if(propEvent == null || propEvent.IsReadOnly) return;
			try {
				if (host != null && transaction == null) {
					transaction = host.CreateTransaction("TaskSolutionCenter AddEvents for " + SourceComponent.Site.Name);
				}
			}
			catch (CheckoutException cxe) {
				if (cxe == CheckoutException.Canceled)
					return;
				throw cxe;
			}
			string handler = (string)propEvent.GetValue(SourceComponent);
			if (handler == null) {
				handler = eventBindingService.CreateUniqueMethodName((IComponent)SourceComponent, eventDescriptor);
				CreateFunction(handler, host.RootComponentClassName, eventDescriptor);
				propEvent.SetValue(SourceObject, handler);
			}
			if (handler != null && eventDescriptor != null) {
				AddCode(handler, host.RootComponentClassName);
			}
			if(firstEvent == null || firstHandler == null)  {
				firstEvent = eventDescriptor;
				firstHandler = handler;
			}
		}
		void CreateFunction(string handler, string rootComponentClassName, EventDescriptor eventDescriptor) {
			if(rootComponentClassName == string.Empty) return;
			ParameterInfo[] parameters = GetParameters(eventDescriptor);
			ProjectItem item = SourceComponent.Site.GetService(typeof(ProjectItem)) as ProjectItem;
			if(parameters != null && item != null) {
				CodeClass codeClass = FindClass(item.FileCodeModel.CodeElements, rootComponentClassName);
				string text = GetEventText(codeClass);
				if(text != string.Empty) {
					if(codeClass.Language == CodeModelLanguageConstants.vsCMLanguageVB) {
						TextPoint textPoint = codeClass.GetEndPoint(vsCMPart.vsCMPartBody);
						EditPoint editPoint = textPoint.CreateEditPoint();
						string subHeader = "Private Sub " + handler + "(";
						for(int i = 0; i < parameters.Length; i ++) {
							subHeader += "ByVal " + parameters[i].Name + " As " + parameters[i].ParameterType.ToString();
							if(i < parameters.Length - 1)
								subHeader += ", ";
						}
						subHeader += ") Handles " + SourceComponent.Site.Name + "." + eventDescriptor.Name;
						editPoint.Insert(subHeader);
						editPoint.Insert(System.Environment.NewLine);
						editPoint.Insert(System.Environment.NewLine);
						editPoint.Insert("End Sub");
						editPoint.Insert(System.Environment.NewLine);
						editPoint.SmartFormat(textPoint);
					} else {
						try {
							CodeFunction codeFunction = codeClass.AddFunction(handler, vsCMFunction.vsCMFunctionFunction, vsCMTypeRef.vsCMTypeRefVoid, -1, vsCMAccess.vsCMAccessPrivate, string.Empty);
							for(int i = 0; i < parameters.Length; i ++)
								codeFunction.AddParameter(parameters[i].Name, parameters[i].ParameterType.ToString(), i);
						}
						catch {}
					}
				}
			}
		}
		ParameterInfo[] GetParameters(EventDescriptor eventDescriptor) {
			MethodInfo invokeMethod = eventDescriptor.EventType.GetMethod("Invoke");
			return invokeMethod != null ? invokeMethod.GetParameters() : null;
		}
		void AddCode(string handler, string rootComponentClassName) {
			if(rootComponentClassName == string.Empty) return;
			ProjectItem item = SourceComponent.Site.GetService(typeof(ProjectItem)) as ProjectItem;
			if(item != null) {
				CodeClass codeClass = FindClass(item.FileCodeModel.CodeElements, rootComponentClassName);
				string text = GetEventText(codeClass);
				if(text != string.Empty) {
					CodeFunction codeFunction = FindMethod(codeClass.Members, handler);
					if(codeFunction != null) {
						TextPoint textPoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartBody);
						EditPoint editPoint = textPoint.CreateEditPoint();
						editPoint.Insert(System.Environment.NewLine);
						editPoint.Insert(text);
						editPoint.SmartFormat(textPoint);
						if(item.Document == null) {
							item.Open(Constants.vsViewKindCode);
						}
						if(item.Document != null) {
							TextDocument textDocument = item.Document.Object("TextDocument") as TextDocument;
							if(textDocument != null) {
								textDocument.Selection.MoveToPoint(textPoint, false);
								textDocument.Selection.WordRight(true, 1);
								textDocument.Selection.Cancel();
							}
						}
					}
				}
			}
		}
		string GetEventText(CodeClass codeClass) {
			return codeClass != null ? GetEventText(codeClass.Language) : string.Empty;
		}
		string GetEventText(string language) {
			return TaskEvent.Languages[GetLanguage(language)];
		}
		string GetLanguage(string language) {
			if(language == CodeModelLanguageConstants.vsCMLanguageCSharp)
				return "CSharp";
			if(language == CodeModelLanguageConstants.vsCMLanguageVB)
				return "VB";
			return "Unknown";
		}
		CodeFunction FindMethod(CodeElements elements, string handler) {
			for (int i = elements.Count; i >= 1; i--) {
				CodeFunction codeFunction = elements.Item(i) as CodeFunction;
				if (codeFunction != null && codeFunction.Name == handler)
					return codeFunction;
			}
			return null;
		}
		CodeClass FindClass(CodeElements elements, string name) {
			CodeClass lElement = null;
			for (int i = 1; i <= elements.Count; i++) {
				if (elements.Item(i) is CodeNamespace) {
					CodeNamespace lNamespace = (CodeNamespace)elements.Item(i);
					lElement = FindClass(lNamespace.Members, name);
					if (lElement != null)
						break;
				}
				else if (elements.Item(i) is CodeClass) {
					if (elements.Item(i).FullName == name) {
						lElement =(CodeClass) elements.Item(i);
						break;
					}
				}
			}
			return lElement;
		}
		Assembly GetDTEAssembly() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			for(int i = 0; i < assemblies.Length; i ++)
				if(assemblies[i].FullName.IndexOf("EnvDTE") == 0)
					return assemblies[i];
			return null;
		}
	}
}
