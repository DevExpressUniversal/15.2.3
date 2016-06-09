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
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraScheduler.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom;
using EnvDTE;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
namespace DevExpress.XtraScheduler.Design {
	#region StringUtil
	public static class StringUtil {
		internal static bool EmptyOrSpace(string str) {
			if (str != null) {
				return (0 >= str.Trim().Length);
			}
			return true;
		}
		internal static bool EqualValue(string str1, string str2) {
			return EqualValue(str1, str2, false);
		}
		internal static bool EqualValue(string str1, string str2, bool caseInsensitive) {
			if ((str1 != null) && (str2 != null)) {
				StringComparison comparisonType = caseInsensitive ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
				return string.Equals(str1, str2, comparisonType);
			}
			return (str1 == str2);
		}
	}
	#endregion
	#region DesignerCodeGenerationHelper
	internal static class DesignerCodeGenerationHelper {
		#region InsertionPoint
		internal enum InsertionPoint {
			Beginning,
			End
		}
		#endregion
		#region CodeElementSearchResult
		class CodeElementSearchResult {
			List<EnvDTE.CodeElement> result;
			EnvDTE.vsCMElement elementType;
			public CodeElementSearchResult(EnvDTE.vsCMElement elementType) {
				this.result = new List<EnvDTE.CodeElement>();
				this.elementType = elementType;
			}
			public List<EnvDTE.CodeElement> CodeElements { get { return result; } }
			public void Process(EnvDTE.CodeElement item) {
				if (item.Kind == this.elementType)
					this.result.Add(item);
			}
		}
		#endregion
		public static List<CodeElement> FindAllCodeClass(CodeElements codeElements) {
			CodeElementSearchResult result = new CodeElementSearchResult(vsCMElement.vsCMElementClass);
			SearchCodeElements(codeElements, result);
			return result.CodeElements;
		}
		public static CodeClass FindClass(CodeElements codeElements, string className) {
			foreach (CodeElement element in codeElements) {
				if (element is EnvDTE.CodeNamespace) {
					CodeClass codeClass = FindClass(element.Children, className);
					if (codeClass != null)
						return codeClass;
				} else {
					CodeClass codeClass = element as CodeClass;
					if (codeClass != null && StringUtil.EqualValue(element.Name, className))
						return codeClass;
				}
			}
			return null;
		}
		public static CodeFunction FindFunction(CodeClass codeClass, string methodName) {
			foreach (CodeElement element in codeClass.Members) {
				CodeFunction codeFunction = element as CodeFunction;
				if (codeFunction != null && StringUtil.EqualValue(element.Name, methodName))
					return codeFunction;
			}
			return null;
		}
		static void GenerateCodeForStatements(CodeDomProvider codeProvider, CodeStatementCollection statements, TextWriter textWriter) {
			CodeGeneratorOptions options = new CodeGeneratorOptions();
			foreach (CodeStatement statement in statements)
				codeProvider.GenerateCodeFromStatement(statement, textWriter, options);
		}
		internal static void InsertCode(IDesignerHost host, CodeTypeDeclaration codeType, CodeMemberMethod method, string code, InsertionPoint insertionPoint) {
			TextPoint startPoint;
			ProjectItem service = (ProjectItem)host.GetService(typeof(ProjectItem));
			if (service == null)
				return;
			CodeClass codeClass = FindClass(service.FileCodeModel.CodeElements, codeType.Name);
			if (codeClass == null)
				return;
			CodeFunction function = FindFunction(codeClass, method.Name);
			if (function == null)
				return;
			if (insertionPoint == InsertionPoint.Beginning)
				startPoint = function.GetStartPoint(vsCMPart.vsCMPartBody);
			else 
				startPoint = function.GetEndPoint(vsCMPart.vsCMPartBody);
			InsertCodeCore(startPoint, code);
		}
		internal static void InsertCodeCore(TextPoint where, string code) {
			EditPoint editPoint = where.CreateEditPoint();
			editPoint.ReplaceText(editPoint, code, (int)vsEPReplaceTextOptions.vsEPReplaceTextAutoformat);
		}
		internal static void InsertStatements(IDesignerHost host, CodeTypeDeclaration codeType, CodeMemberMethod method, CodeStatementCollection statements, InsertionPoint insertionPoint) {
			CodeDomProvider service = (CodeDomProvider)host.GetService(typeof(CodeDomProvider));
			if (service != null) {
				StringBuilder sb = new StringBuilder();
				StringWriter textWriter = new StringWriter(sb);
				GenerateCodeForStatements(service, statements, textWriter);
				InsertCode(host, codeType, method, sb.ToString(), insertionPoint);
			}
		}
		static void SearchCodeElements(CodeElements codeElements, CodeElementSearchResult searchResult) {
			foreach (EnvDTE.CodeElement item in codeElements) {
				searchResult.Process(item);
				SearchCodeElements(item.Children, searchResult);
			}
		}
	}
	#endregion
	public class CodeGenerationTarget {
		CodeTypeDeclaration type;
		CodeMemberMethod method;
		CodeStatementCollection methodStatements;
		public CodeGenerationTarget(CodeTypeDeclaration type, CodeMemberMethod method) {
			this.type = type;
			this.method = method;
			this.methodStatements = new CodeStatementCollection();
		}
		public CodeTypeDeclaration Type { get { return type; } }
		public CodeMemberMethod Method { get { return method; } }
		public CodeStatementCollection MethodStatements { get { return methodStatements; } }
		public bool IsValid { get { return type != null && method != null; } }
	}
	public class DesignDocumentChangeHelper : IDisposable {
		Type docDataServiceType;
		object docDataService;
		IDisposable changeMarker;
		public DesignDocumentChangeHelper(IDesignerHost host) {
			if (host == null)
				Exceptions.ThrowArgumentNullException("host");
			BeginChange(host);
		}
		public bool CanChangeDocument { get { return docDataService != null; } }
		protected internal virtual void BeginChange(IDesignerHost host) {
			this.docDataServiceType = LookupDesignerDocDataServiceType();
			if (this.docDataServiceType == null)
				return;
			docDataService = host.GetService(docDataServiceType);
			if (docDataService != null)
				changeMarker = CreateChangeMarker();
		}
		protected internal virtual void EndChange() {
			if (changeMarker != null)
				changeMarker.Dispose();
			changeMarker = null;
			docDataService = null;
		}
		protected internal virtual Type LookupDesignerDocDataServiceType() {
			Assembly asm = GetDocDataServiceTypeAssembly();
			if (asm == null)
				return null;
			return asm.GetType("Microsoft.VisualStudio.Shell.Design.Serialization.DesignerDocDataService");
		}
		protected internal virtual Assembly GetDocDataServiceTypeAssembly() {
			Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
			int count = assemblies.Length;
			for (int i = 0; i < count; i++) {
				if (assemblies[i].GetName().Name == "Microsoft.VisualStudio.Shell.Design")
					return assemblies[i];
			}
			return null;
		}
		protected internal virtual IDisposable CreateChangeMarker() {
			PropertyInfo property = docDataServiceType.GetProperty("PrimaryDocData");
			if (property == null)
				return null;
			object docData = property.GetValue(docDataService, new object[] { });
			if (docData == null)
				return null;
			MethodInfo method = docData.GetType().GetMethod("CreateChangeMarker");
			if (method == null)
				return null;
			IDisposable result = method.Invoke(docData, new object[] { }) as IDisposable;
			return result;
		}
		#region IDisposable implementation
		public void Dispose() {
			Dispose(true);
		}
		~DesignDocumentChangeHelper() {
			Dispose(false);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing)
				EndChange();
		}
		#endregion
	}
	public interface IEventHandlerCodeGenerator {
		void GenerateCode(CodeGenerationTarget handlerCode);
	}
	public class DesignTimeEventHandlerHelper {
		IDesignerHost host;
		IEventBindingService eventBindingService;
		public DesignTimeEventHandlerHelper(IDesignerHost host) {
			if (host == null)
				Exceptions.ThrowArgumentNullException("host");
			UseExistingEventHandler = true;
			this.host = host;
			this.eventBindingService = (IEventBindingService)host.GetService(typeof(IEventBindingService));
			if (this.eventBindingService == null)
				Exceptions.ThrowArgumentNullException("this.eventBindingService");
		}
		public bool UseExistingEventHandler { get; set; }
		public void GenerateEventHandler(IComponent component, string eventName, IEventHandlerCodeGenerator generator) {
			string handlerName = GenerateEventHandlerCore(component, eventName);
			if (String.IsNullOrEmpty(handlerName))
				return;
			CodeGenerationTarget target = LookupEventHandlerCode(handlerName);
			if (!target.IsValid)
				return;
			generator.GenerateCode(target);
			InsertEventHandlerStatements(target, target.MethodStatements, DesignerCodeGenerationHelper.InsertionPoint.Beginning);
			this.eventBindingService.ShowCode(component, TypeDescriptor.GetEvents(component)[eventName]);
			DevExpress.Utils.Design.EditorContextHelperEx.HideSmartPanel(component);
		}
		internal virtual void InsertEventHandlerStatements(CodeGenerationTarget handlerCode, CodeStatementCollection handlerStatements, DesignerCodeGenerationHelper.InsertionPoint insertionPoint) {
			using (DesignDocumentChangeHelper helper = new DesignDocumentChangeHelper(host)) {
				handlerCode.Method.Statements.AddRange(handlerStatements);
			}
		}
		protected internal virtual CodeGenerationTarget LookupEventHandlerCode(string handlerName) {
			CodeMemberMethod eventHandlerMethod = null;
			CodeTypeDeclaration formCodeType = (CodeTypeDeclaration)host.GetService(typeof(CodeTypeDeclaration));
			foreach (CodeTypeMember member in formCodeType.Members) {
				CodeMemberMethod method = member as CodeMemberMethod;
				if (method != null && StringUtil.EqualValue(method.Name, handlerName)) {
					eventHandlerMethod = method;
					break;
				}
			}
			return new CodeGenerationTarget(formCodeType, eventHandlerMethod);
		}
		protected internal virtual string GenerateEventHandlerCore(IComponent component, string eventName) {
			EventDescriptor eventDescriptor = TypeDescriptor.GetEvents(component)[eventName];
			if (eventDescriptor == null)
				return String.Empty;
			PropertyDescriptor eventProperty = eventBindingService.GetEventProperty(eventDescriptor);
			string result = eventProperty.GetValue(component) as string;
			if (StringUtil.EmptyOrSpace(result) || !UseExistingEventHandler) {
				result = eventBindingService.CreateUniqueMethodName(component, eventDescriptor);
				eventProperty.SetValue(component, result);
			}
			return result;
		}
	}
}
