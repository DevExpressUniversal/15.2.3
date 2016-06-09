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
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Text;
using System.Security;
using System.Security.Permissions;
using System.CodeDom;
using System.CodeDom.Compiler;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Design;
using System.Security.Policy;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraReports {
	internal static class ScriptGenerator {
		static string GenerateTypeSource(ScriptLanguage language, CodeTypeDeclaration typeDecl) {
			CodeDomProvider provider = CodeDomProviderHelper.CreateDomProvider(language);
			StringBuilder sb = new StringBuilder();
			StringWriter writer = new StringWriter(sb);
			provider.GenerateCodeFromType(typeDecl, writer, new CodeGeneratorOptions());
			return sb.ToString();
		}
		static string ExtractMethodSource(string emptyTypeSource, string typeSource) {
			string[] full = XRConvert.StringToStringArray(typeSource);
			string[] empty = XRConvert.StringToStringArray(emptyTypeSource);
			int fullIndex = 0;
			int count = empty.Length;
			StringCollection methodLines = new StringCollection();
			for(int emptyIndex = 0; emptyIndex < count; emptyIndex++, fullIndex++) {
				while(full[fullIndex] != empty[emptyIndex]) {
					methodLines.Add(full[fullIndex]);
					fullIndex++;
				}
			}
			count = methodLines.Count;
			int whiteSpacesCount = int.MaxValue;
			for(int i = 0; i < count; i++) {
				string line = methodLines[i];
				string trimmedLine = line.Trim();
				if(trimmedLine.Length > 0)
					whiteSpacesCount = Math.Min(line.Length - trimmedLine.Length, whiteSpacesCount);
			}
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < count; i++) {
				string line = methodLines[i];
				if(line.Length >= whiteSpacesCount)
					line = line.Substring(whiteSpacesCount);
				if(!String.IsNullOrEmpty(line.Trim())) {
					sb.Append(line);
					sb.Append("\r\n");
				}
			}
			return sb.ToString();
		}
		static public string GenerateMethod(MethodInfo mi, string methodName, ScriptLanguage language) {
			return GenerateMethod(methodName, mi.ReturnType, mi.GetParameters(), language);
		}
		static public string GenerateMethod(string methodName, Type returnType, ParameterInfo[] parameters, ScriptLanguage language) {
			CodeTypeDeclaration typeDecl = new CodeTypeDeclaration("X");
			CodeMemberMethod method = new CodeMemberMethod();
			method.Name = methodName;
			method.ReturnType = new CodeTypeReference(returnType);
			int count = parameters.Length;
			for(int i = 0; i < count; i++) {
				ParameterInfo pi = parameters[i];
				CodeParameterDeclarationExpression parameter = new CodeParameterDeclarationExpression(pi.ParameterType, pi.Name);
				parameter.Name = pi.Name;
				method.Parameters.Add(parameter);
			}
			if(returnType != typeof(void)) {
				if(!returnType.IsValueType) {
					CodePrimitiveExpression expression = new CodePrimitiveExpression(null);
					CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(expression);
					method.Statements.Add(returnStatement);
				}
			}
			string emptyTypeSource = GenerateTypeSource(language, typeDecl);
			typeDecl.Members.Add(method);
			string typeSource = GenerateTypeSource(language, typeDecl);
			string methodString = ExtractMethodSource(emptyTypeSource, typeSource);
			return methodString.Insert(methodString.IndexOf("\r\n"), "\r\n");
		}
	}
	internal class ScriptManager : IDisposable {
		#region static
		static StringCollection staticImports = new StringCollection();
		static string singleLineCommentCS = "//";
		static string singleLineCommentVB = "'";
		static string singleLineCommentJS = "//";
		internal static readonly object NotResult;
		static ScriptManager() {
			NotResult = new object();
			staticImports.Add("System");
			staticImports.Add("System.Collections");
			staticImports.Add("System.Drawing");
			staticImports.Add("System.Drawing.Printing");
			staticImports.Add("DevExpress.Data");
			staticImports.Add("DevExpress.Utils");
			staticImports.Add("DevExpress.XtraCharts");
			staticImports.Add("DevExpress.XtraPrinting");
			staticImports.Add("DevExpress.XtraReports");
			staticImports.Add("DevExpress.XtraReports.UI");
		}
		static void GenerateBreakpointInConstructor(CodeTypeDeclaration type) {
			CodeMemberMethod method = new CodeConstructor();
			method.Attributes = MemberAttributes.Public;
			CodeTypeReferenceExpression typeRef = new CodeTypeReferenceExpression(typeof(System.Diagnostics.Debugger));
			CodeMethodInvokeExpression invoke = new CodeMethodInvokeExpression(typeRef, "Break");
			if(ScriptCompiler.DebugSwitch.State == ScriptsDebugLevel.BreakWhenDebuggerAttached) {
				CodeExpression condition = new CodePropertyReferenceExpression(typeRef, "IsAttached");
				CodeConditionStatement conditionStatement = new CodeConditionStatement(condition, new CodeExpressionStatement(invoke));
				method.Statements.Add(conditionStatement);
			} else
				method.Statements.Add(invoke);
			type.Members.Add(method);
		}
		static string FormatErrorString(CompilerError error) {
			return String.Format("line {1}, column {2}: error {3}: {4}\r\n", error.FileName, error.Line, error.Column, error.ErrorNumber, error.ErrorText);
		}
		#endregion
		#region inner classes
		class ScriptVariable {
			string name;
			object val;
			public string Name { get { return name; } }
			public object Value { get { return val; } }
			public ScriptVariable(string name, object val) {
				this.name = name;
				this.val = val;
			}
		}
		#endregion
		Assembly asm;
		StringCollection userReferences = new StringCollection();
		StringCollection imports = new StringCollection();
		List<ScriptVariable> variables = new List<ScriptVariable>();
		ArrayList securityPermissions = new ArrayList();
		PermissionSet permissionSet;
		Type targetType;
		object targetObject;
		ScriptLanguage language = ScriptLanguage.CSharp;
		string namespaceName;
		string className;
		CodeDomProvider provider;
		bool compilationError;
		string errorsString;
		CompilerErrorCollection errors;
		ScriptSourceCompilerCorrector sourceCorrector;
		string scriptSource;
		Evidence evidence;
		IApplicationPathService applicationPathService;
		bool enableResolving;
		bool subscribed;
		ScriptSourceCompilerCorrector SourceCorrector {
			get {
				if(sourceCorrector == null)
					sourceCorrector = new ScriptSourceCompilerCorrector(language, scriptSource);
				return sourceCorrector;
			}
		}
		public void Initialize(string scriptSource, ScriptLanguage language, Evidence evidence) {
			if(this.scriptSource != scriptSource) {
				this.scriptSource = scriptSource;
				Reset();
			}
			if(!object.ReferenceEquals(this.evidence, evidence)) {
				this.evidence = evidence;
				Reset();
			}
			if(this.language != language) {
				this.language = language;
				Reset();
			}
		}
		CodeDomProvider Provider {
			get {
				if(provider == null) {
					provider = CreateDomProvider();
				}
				return provider;
			}
		}
		string SingleLineComment {
			get {
				switch(language) {
					case ScriptLanguage.CSharp:
						return singleLineCommentCS;
					case ScriptLanguage.VisualBasic:
						return singleLineCommentVB;
					case ScriptLanguage.JScript:
						return singleLineCommentJS;
					default:
						return singleLineCommentCS;
				}
			}
		}
		string MethodStartMarker {
			get { return SingleLineComment + "--MethodStart--"; }
		}
		public string ErrorMessages { get { return errorsString; } }
		public CompilerErrorCollection Errors { get { return errors; } }
		public string NamespaceName { get { return namespaceName; } set { namespaceName = value; } }
		public string ClassName { get { return className; } set { className = value; } }
		public string FullClassName { get { return namespaceName + '.' + className; } }
		public ScriptManager(IApplicationPathService applicationPathService) {
			NamespaceName = "ScriptingNamespace";
			ClassName = "ScriptingReport";
			imports.AddRange((string[])new ArrayList(staticImports).ToArray(typeof(string)));
			this.applicationPathService = applicationPathService;
		}
		CodeDomProvider CreateDomProvider() {
			return CodeDomProviderHelper.CreateDomProvider(language);
		}
		bool BuildAssembly(string source, string[] sourceLines) {
			if(asm != null)
				return true;
			if(compilationError)
				return false;
			Compiler compiler = new ScriptCompiler(source, userReferences, language, MethodStartMarker, applicationPathService);
			Assembly compiledAssembly = compiler.GetCompiledAssembly(evidence);
			if(compiledAssembly == null) {
				ResetAssembly();
				compilationError = true;
				StringBuilder sb = new StringBuilder();
				errors = SourceCorrector.GetScriptErrors(compiler.Results.Errors, source);
				foreach(CompilerError error in errors)
					sb.Append(FormatErrorString(error));
				errorsString = sb.ToString();
				return false;
			} else {
				if(errors != null)
					errors.Clear();
				asm = compiledAssembly;
				targetType = asm.GetType(FullClassName, false);
				try {
					targetObject = Activator.CreateInstance(targetType);
				} catch {
					return false;
				}
				AssignFields();
			}
			return true;
		}
		void ResetAssembly() {
			compilationError = false;
			targetObject = null;
			targetType = null;
			asm = null;
		}
		public void Reset() {
			provider = null;
			userReferences.Clear();
			variables.Clear();
			securityPermissions.Clear();
			permissionSet = null;
			sourceCorrector = null;
			ResetAssembly();
		}
		void AssignFields() {
			foreach(ScriptVariable var in variables) {
				FieldInfo fi = targetType.GetField(var.Name, BindingFlags.Instance | BindingFlags.NonPublic);
				if(fi != null)
					fi.SetValue(targetObject, var.Value);
			}
		}
		CodeCompileUnit GenerateCodeTree() {
			ResetImports();
			CodeCompileUnit unit = new CodeCompileUnit();
			GenetateFW4Compatability(unit);
			CodeNamespace nameSpace = new CodeNamespace(NamespaceName);
			unit.Namespaces.Add(nameSpace);
			foreach(string import in imports)
				nameSpace.Imports.Add(new CodeNamespaceImport(import));
			CodeTypeDeclaration type = new CodeTypeDeclaration(ClassName);
			type.BaseTypes.Add(new CodeTypeReference(typeof(ScriptingReportBase)));
			nameSpace.Types.Add(type);
			type.Members.Add(new CodeSnippetTypeMember(SourceCorrector.Scripts));
			GenerateScriptVariables(type);
			if(ScriptCompiler.DebugSwitch.Enabled)
				GenerateBreakpointInConstructor(type);
			return unit;
		}
		void ResetImports() {
			imports = new StringCollection();
			imports.AddRange((string[])new ArrayList(staticImports).ToArray(typeof(string)));
			imports.AddRange(SourceCorrector.Imports);
		}
		static void GenetateFW4Compatability(CodeCompileUnit unit) {
			Type securityRulesType = Type.GetType("System.Security.SecurityRulesAttribute");
			if(securityRulesType != null) {
				CodeExpression enumMember = new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Security.SecurityRuleSet"), "Level1");
				unit.AssemblyCustomAttributes.Add(
					new CodeAttributeDeclaration(
						new CodeTypeReference(securityRulesType),
						new CodeAttributeArgument(enumMember)
					)
				);
			}
		}
		void GenerateScriptVariables(CodeTypeDeclaration type) {
			foreach(ScriptVariable var in variables) {
				Type variableType = var.Value.GetType();
				if(typeof(XtraReport).IsAssignableFrom(variableType)) {
					variableType = typeof(XtraReport);
					CodeMemberProperty ownerReportProperty = new CodeMemberProperty();
					ownerReportProperty.Attributes = MemberAttributes.Public | MemberAttributes.Override;
					ownerReportProperty.Name = "OwnerReport";
					ownerReportProperty.Type = new CodeTypeReference(variableType);
					CodePropertyReferenceExpression reportRef = new CodePropertyReferenceExpression(new CodeThisReferenceExpression(), var.Name);
					CodeMethodReturnStatement returnStatement = new CodeMethodReturnStatement(reportRef);
					ownerReportProperty.GetStatements.Add(returnStatement);
					type.Members.Add(ownerReportProperty);
				}
				CodeMemberField field = new CodeMemberField(variableType, var.Name);
				type.Members.Add(field);
			}
		}
		void GenerateSource(StringBuilder sb) {
			StringWriter writer = new StringWriter(sb);
			CodeCompileUnit unit = GenerateCodeTree();
			CodeGeneratorOptions opts = new CodeGeneratorOptions();
			opts.BlankLinesBetweenMembers = false;
			Provider.GenerateCodeFromCompileUnit(unit, writer, opts);
		}
		string MakeSource(out string[] sourceLines) {
			StringBuilder sb = new StringBuilder();
			GenerateSource(sb);
			string source = sb.ToString();
			sourceLines = System.Text.RegularExpressions.Regex.Split(source, "\r\n");
			return source;
		}
		bool BuildAssembly() {
			if(asm != null)
				return true;
			string[] sourceLines;
			string source = MakeSource(out sourceLines);
			return BuildAssembly(source, sourceLines);
		}
		public Delegate GetHandler(Type type, string scriptName) {
			if(string.IsNullOrEmpty(SourceCorrector.Scripts) || !BuildAssembly() || targetType == null || targetObject == null)
				return null;
			System.Reflection.MethodInfo mi = targetObject.GetType().GetMethod(scriptName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			return !ReferenceEquals(mi, null) ? Delegate.CreateDelegate(type, targetObject, mi) : null;
		}
		public void Run<E>(EventHandler<E> handler, object sender, E e) where E : EventArgs {
			if(!subscribed) {
				SubscribeAssemblyResolve();
				subscribed = true;
			}
			enableResolving = true;
			try {
				PermissionSet ps = GetPermissionSet();
				if(ps.Count > 0)
#pragma warning disable 0618
					ps.Deny();
#pragma warning restore 0618
				handler(sender, e);
				if(ps.Count > 0)
#pragma warning disable 0618
					CodeAccessPermission.RevertDeny();
#pragma warning restore 0618
			} finally {
				enableResolving = false;
			}
		}
		PermissionSet GetPermissionSet() {
			if(permissionSet == null) {
				permissionSet = new PermissionSet(PermissionState.None);
				foreach(ScriptSecurityPermission scriptPermission in securityPermissions) {
					IPermission permission;
					if(scriptPermission.Deny && scriptPermission.TryGetPermission(out permission))
						permissionSet.AddPermission(permission);
				}
			}
			return permissionSet;
		}
		[System.Security.SecuritySafeCritical]
		void SubscribeAssemblyResolve() {
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}
		[System.Security.SecuritySafeCritical]
		void UnsubscribeAssemblyResolve() {
			AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}
		Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args) {
			if(!enableResolving)
				return null;
			try {
				string nameForResolve = args.Name.Substring(0, args.Name.IndexOf(","));
				foreach(string reference in userReferences) {
					if(!File.Exists(reference))
						continue;
					string fileName = Path.GetFileNameWithoutExtension(reference);
					if(fileName == nameForResolve)
						return Assembly.LoadFrom(reference);
				}
			} catch {
			}
			return null;
		}
		public void AddVariable(string name, object val) {
			if(String.IsNullOrEmpty(name)) {
				if(val == null) return;
				name = val.GetType().Name;
				if(variables.Any<ScriptVariable>(item => { return string.Equals(name, item.Name); }))
					return;
			}
			variables.Add(new ScriptVariable(name, val));
		}
		public void AddReferences(string[] references) {
			this.userReferences.AddRange(references);
		}
		public void AddSecurityPermissions(ScriptSecurityPermissionCollection securityPermissions) {
			this.securityPermissions.AddRange(securityPermissions);
		}
		public bool CheckScript() {
			return string.IsNullOrEmpty(SourceCorrector.Scripts) || BuildAssembly();
		}
		public void Dispose() {
			Release();
		}
		public void Release() {
			if(subscribed) {
				UnsubscribeAssemblyResolve();
				subscribed = false;
			}
		}
	}
	public class XREventsScriptManager : IDisposable {
		#region static
		static public bool ContainsEventScript(XRControl control, string eventName) {
			return !string.IsNullOrEmpty(control.Scripts.GetProcName(eventName));
		}
		#endregion
		ScriptManager scriptExecutor;
		XtraReport report;
		static readonly EventHandler NotSetDelegate = new EventHandler(NotSetHandler);
		public Evidence Evidence { get; set; }
		public string ErrorMessage {
			get { return scriptExecutor.ErrorMessages; }
		}
		public CompilerErrorCollection Errors { get { return scriptExecutor.Errors; } }
		public XREventsScriptManager(XtraReport report) {
			this.report = report;
			IApplicationPathService serv = report.Site.GetService<IApplicationPathService>();
			if(serv == null)
				serv = report.PrintingSystem.GetService<IApplicationPathService>();
			scriptExecutor = new ScriptManager(serv);
		}
		public void AddVariable(string name, object val) {
			scriptExecutor.AddVariable(name, val);
		}
		public void AddReferences(string[] references) {
			scriptExecutor.AddReferences(references);
		}
		public void AddSecurityPermissions(ScriptSecurityPermissionCollection securityPermissions) {
			scriptExecutor.AddSecurityPermissions(securityPermissions);
		}
		static void NotSetHandler(object s, EventArgs e) { throw new InvalidOperationException(); }
		public bool RunEventScript<E>(object eventId, string eventName, IScriptable scriptable, E e) where E : EventArgs {
			System.Diagnostics.Debug.Assert(scriptable != null);
			try {
				Delegate handler = GetHandler<E>(eventId, eventName, scriptable.Scripts);
				if(!IsHandlerPresent(handler))
					return false;
				scriptExecutor.Run<E>((EventHandler<E>)handler, scriptable, e);
				return true;
			} catch(Exception ex) {
				Exception ex2 = null;
				if(ex is TargetInvocationException && ex.InnerException is SecurityException) {
					ex2 = new SecurityException(ReportStringId.Msg_ScriptingPermissionErrorMessage.GetString(ex.InnerException.Message), ex.InnerException);
				} else {
					string msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
					ex2 = new Exception(ReportStringId.Msg_ScriptExecutionError.GetString(scriptable.Scripts.ComponentName + "." + eventName, msg), ex);
				}
				throw ex2;
			}
		}
		static bool IsHandlerPresent(Delegate handler) {
			return !object.ReferenceEquals(handler, NotSetDelegate);
		}
		Delegate GetHandler<E>(object eventId, string eventName, XRScriptsBase scripts) where E : EventArgs {
			System.Diagnostics.Debug.Assert(scripts != null);
			Delegate handler = scripts.Events[eventId];
			if(handler == null) {
				string procName = GetProcName(XRControl.EventNames.HandlerPrefix + eventName, scripts);
				Delegate tmpHandler = procName != null ? scriptExecutor.GetHandler(typeof(EventHandler<E>), procName) : null;
				scripts.Events[eventId] = handler = tmpHandler ?? NotSetDelegate;
			}
			return handler;
		}
		string GetProcName(string eventName, XRScriptsBase scripts) {
			string procName = scripts.GetProcName(eventName);
			if(scripts.NeedConvertScript(procName)) {
				ConvertAllScripts(report);
				procName = scripts.GetProcName(eventName);
			}
			return procName;
		}
		static void ConvertAllScripts(XRControl rootControl) {
			foreach(XRControl control in rootControl.Controls) {
				IScriptable scriptable = control as IScriptable;
				if(control != null)
					scriptable.Scripts.ConvertScripts(control.RootReport);
				if(control.Controls.Count > 0)
					ConvertAllScripts(control);
			}
		}
		public bool ValidateScripts() {
			scriptExecutor.Initialize(report.ScriptsSource, report.ScriptLanguage, Evidence);
			return scriptExecutor.CheckScript();
		}
		public void Initialize(string[] references, ScriptSecurityPermissionCollection securityPermissions) {
			scriptExecutor.Reset();
			scriptExecutor.Initialize(report.ScriptsSource, report.ScriptLanguage, Evidence);
			AddReferences(references);
			AddSecurityPermissions(securityPermissions);
		}
		public void Release() {
			scriptExecutor.Release();
		}
		public void Dispose() {
			if(scriptExecutor != null) {
				scriptExecutor.Dispose();
				scriptExecutor = null;
			}
		}
	}
}
