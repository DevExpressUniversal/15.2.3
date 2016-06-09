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
using System.Collections.Generic;
using DevExpress.Design.DataAccess;
namespace DevExpress.Design.CodeGenerator {
	public interface IModelServiceProvider {
		TService GetService<TService>() where TService : class;
	}
	#region DTE Info Classes
	public class DTEServiceProvider : IModelServiceProvider, IDisposable {
		IServiceProvider providerCore;
		public DTEServiceProvider(IServiceProvider provider) {
			providerCore = provider;
		}
		#region IModelServiceProvider Members
		public TService GetService<TService>() where TService : class {
			return providerCore.GetService(typeof(TService)) as TService;
		}
		#endregion
		#region IDisposable Members
		void IDisposable.Dispose() {
			providerCore = null;
		}
		#endregion
	}
	public class DTEParameterInfo {
		public DTEParameterInfo(string name, Type type, int position) {
			this.Name = name;
			this.Type = type;
			this.Position = position;
		}
		public Type Type { get;private set; }
		public string Name { get;private set; }
		public int Position { get;private set; }
	}
	public class DTEFunctionInfo {
		List<DTEParameterInfo> parameters;
		public DTEFunctionInfo(string name){
			this.parameters = new List<DTEParameterInfo>();
			this.Name = name;
		}
		public DTEParameterInfo[] Parameters { 
			get { return parameters.ToArray(); } 
		}
		public void AddParameter(string name, Type type, int position = -1) {
			parameters.Add(new DTEParameterInfo(name, type, (position == -1) ? parameters.Count : position));
		}
		public string Name { get;private set;}
		public string Comment { get; set; }
		public string Code { get; set; }
		public Type ResultType { get; set; }
	}
	public class DTEConstructorInfo : DTEFunctionInfo {
		public DTEConstructorInfo()
			: base(null) {
		}
	}
	public class DTEEventInfo : DTEFunctionInfo {
		public DTEEventInfo(string name, string eventName)
			: base(name) {
			EventName = eventName;
		}
		public string EventName { get; private set; }
		public string EventSubscriptionComment { get; set; }
	}
	#endregion DTE Info Classes
	public static class CodeGeneratorHelper {
		public static int AddUsing(IModelServiceProvider serviceProvider, string namespaceString, string strCode) {
			EnvDTE.CodeClass rootClass = CodeNavigator.GetRootComponentClass(serviceProvider);
			if(rootClass != null) {
				var codeModel = rootClass.ProjectItem.FileCodeModel;
				var languageID = CodeParserHelper.GetLanguageID(rootClass);
				EnvDTE.TextPoint importPoint;
				if(!CodeNavigator.ContainsImport(codeModel.CodeElements, namespaceString, out importPoint)) {
					if(languageID == CodeParser.ParserLanguageID.Basic)
						strCode = "Imports " + namespaceString;
					EnvDTE.EditPoint editPoint;
					if(importPoint == null) {
						strCode = strCode + System.Environment.NewLine;
						editPoint = rootClass.StartPoint.CreateEditPoint();
						editPoint.StartOfDocument();
					}
					else {
						strCode = System.Environment.NewLine + strCode;
						editPoint = importPoint.CreateEditPoint();
					}
					editPoint.Insert(strCode);
					return editPoint.Line;
				}
			}
			return -1;
		}
		public static int AddEventHandler(IModelServiceProvider serviceProvider, string target, DTEEventInfo info) {
			EnvDTE.CodeClass rootClass = CodeNavigator.GetRootComponentClass(serviceProvider);
			if(rootClass != null) {
				var languageID = CodeParserHelper.GetLanguageID(rootClass);
				if(AddFunction(rootClass, info, languageID, true) != -1) {
					string eventSubscriptionCode = info.EventSubscriptionComment +
						"this." + target + "." + info.EventName + "+=" + info.Name + ";";
					return AppendConstructorBody(serviceProvider, rootClass, eventSubscriptionCode, true);
				}
			}
			return -1;
		}
		public static int AddFunction(IModelServiceProvider serviceProvider, DTEFunctionInfo info) {
			EnvDTE.CodeClass rootClass = CodeNavigator.GetRootComponentClass(serviceProvider);
			if(rootClass != null) {
				object tResult = (info.ResultType == null) ? (object)EnvDTE.vsCMTypeRef.vsCMTypeRefVoid : info.ResultType.FullName;
				var languageID = CodeParserHelper.GetLanguageID(rootClass);
				return AddFunction(rootClass, info, languageID, false);
			}
			return -1;
		}
		static int AddFunction(EnvDTE.CodeClass rootClass, DTEFunctionInfo info, CodeParser.ParserLanguageID languageID, bool isEventSubscription) {
			if(rootClass != null) {
				object tResult = (info.ResultType == null) ? (object)EnvDTE.vsCMTypeRef.vsCMTypeRefVoid : info.ResultType.FullName;
				EnvDTE.vsCMFunction vsCMFunction = (languageID == CodeParser.ParserLanguageID.Basic) && (info.ResultType == null) ? 
					EnvDTE.vsCMFunction.vsCMFunctionSub : EnvDTE.vsCMFunction.vsCMFunctionFunction;
				return AddFunction(rootClass, info.Name, info.Comment, info.Code, tResult, info.Parameters, languageID, vsCMFunction, isEventSubscription);
			}
			return -1;
		}
		static int AddFunction(EnvDTE.CodeClass rootClass, string name, string comment, string strCode, object tResult, DTEParameterInfo[] parameters, CodeParser.ParserLanguageID languageID, EnvDTE.vsCMFunction vsCMFunction, bool isEventSubscription) {
			if(rootClass == null) return -1;
			EnvDTE.CodeFunction function = rootClass.AddFunction(name, vsCMFunction, tResult, -1);
			if(function != null) {
				function.Comment = comment;
				foreach(DTEParameterInfo parameter in parameters)
					function.AddParameter(parameter.Name, parameter.Type.FullName, parameter.Position);
				string generatedCode = CodeParserHelper.GetGeneratedCode(languageID, strCode, isEventSubscription);
				return AppendFunctionBody(function, generatedCode);
			}
			return -1;
		}
		public static int AppendConstructorBody(IModelServiceProvider serviceProvider, string strCode) {
			EnvDTE.CodeClass rootClass = CodeNavigator.GetRootComponentClass(serviceProvider);
			if(rootClass != null) {
				return AppendConstructorBody(serviceProvider, rootClass, strCode, false);
			}
			return -1;
		}
		static int AppendConstructorBody(IModelServiceProvider serviceProvider, EnvDTE.CodeClass rootClass, string strCode, bool isEventSubscription) {
			EnvDTE.CodeFunction constructor = CodeNavigator.GetCodeClassConstructor(rootClass);
			var languageID = CodeParserHelper.GetLanguageID(rootClass);
			if(constructor == null) {
				EnvDTE.vsCMFunction vsCMFunction = (languageID == CodeParser.ParserLanguageID.Basic) ? 
					EnvDTE.vsCMFunction.vsCMFunctionSub : EnvDTE.vsCMFunction.vsCMFunctionConstructor;
				string name = (languageID == CodeParser.ParserLanguageID.Basic) ? "New" : rootClass.Name;
				constructor = rootClass.AddFunction(name, vsCMFunction, EnvDTE.vsCMTypeRef.vsCMTypeRefVoid);
				string constructorCode = CodeParserHelper.GetGeneratedCode(languageID, "InitializeComponent();");
				AppendFunctionBody(constructor, constructorCode);
			}
			if(constructor != null) {
				string generatedCode = CodeParserHelper.GetGeneratedCode(languageID, strCode, isEventSubscription);
				return AppendFunctionBody(constructor, generatedCode);
			}
			return -1;
		}
		static int AppendFunctionBody(EnvDTE.CodeFunction function, string strCode) {
			var editPoint = function.GetEndPoint(EnvDTE.vsCMPart.vsCMPartBody).CreateEditPoint();
			int line = editPoint.Line;
			editPoint.Insert(strCode);
			editPoint.MoveToPoint(function.StartPoint);
			editPoint.SmartFormat(function.EndPoint);
			return line;
		}
	}
	[System.CLSCompliant(false)]
	public static class CodeNavigator {
		public static EnvDTE.CodeClass GetRootComponentClass(IModelServiceProvider serviceProvider) {
			var designerHost = serviceProvider.GetService<System.ComponentModel.Design.IDesignerHost>();
			if(designerHost != null) {
				var dte = serviceProvider.GetService<EnvDTE.DTE>();
				if(dte != null && dte.Solution.IsOpen) {
					EnvDTE.FileCodeModel fileCodeModel = dte.ActiveDocument.ProjectItem.FileCodeModel;
					string rootClassName = designerHost.RootComponentClassName;
					return GetCodeClass(fileCodeModel, rootClassName);
				}
			}
			return null;
		}
		public static EnvDTE.CodeFunction GetCodeClassConstructor(EnvDTE.CodeClass classElement) {
			foreach(EnvDTE.CodeElement member in classElement.Members) {
				if(member.Kind == EnvDTE.vsCMElement.vsCMElementFunction) {
					EnvDTE.CodeFunction function = (EnvDTE.CodeFunction)member;
					if(function.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionConstructor)
						return function;
				}
			}
			return null;
		}
		public static bool ContainsImport(EnvDTE.CodeElements elements, string namespaceString, out EnvDTE.TextPoint lastImport) {
			lastImport = null;
			EnvDTE.CodeElement lastElement = null;
			foreach(EnvDTE.CodeElement element in elements) {
				if(element.Kind == EnvDTE.vsCMElement.vsCMElementImportStmt) {
					if(((EnvDTE80.CodeImport)element).Namespace == namespaceString)
						return true;
					lastElement = element;
				}
			}
			if(lastElement != null)
				lastImport = lastElement.EndPoint;
			return false;
		}
		public static EnvDTE.CodeFunction GetCodeClassMethod(EnvDTE.CodeClass classElement, string methodName) {
			foreach(EnvDTE.CodeElement member in classElement.Members) {
				if(member.Kind == EnvDTE.vsCMElement.vsCMElementFunction) {
					EnvDTE.CodeFunction function = (EnvDTE.CodeFunction)member;
					if(function.FunctionKind == EnvDTE.vsCMFunction.vsCMFunctionFunction && function.Name == methodName)
						return function;
				}
			}
			return null;
		}
		static EnvDTE.CodeClass GetCodeClass(EnvDTE.FileCodeModel fileCodeModel, string fullName) {
			foreach(EnvDTE.CodeClass codeClass in GetClasses(fileCodeModel.CodeElements))
				if(codeClass.FullName == fullName) return codeClass;
			return null;
		}
		static IEnumerable<EnvDTE.CodeClass> GetClasses(EnvDTE.CodeElements codeElements) {
			foreach(EnvDTE.CodeElement element in codeElements) {
				if(element.Kind != EnvDTE.vsCMElement.vsCMElementClass) {
					if(element.Children.Count > 0) {
						foreach(EnvDTE.CodeClass childClass in GetClasses(element.Children))
							yield return childClass;
					}
				}
				else yield return (EnvDTE.CodeClass)element;
			}
		}
	}
}
