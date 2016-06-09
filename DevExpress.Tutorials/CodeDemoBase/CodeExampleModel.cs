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
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Internal;
using Microsoft.CSharp;
using System.Linq;
namespace DevExpress.DXperience.Demos.CodeDemo {
	public delegate void CodeEvaluationEventHandler(object sender, CodeEvaluationEventArgs e);
	public delegate void OnAfterCompileEventHandler(object sender, OnAfterCompileEventArgs e);
	public enum ExampleLanguage {
		Csharp = 0,
		VB = 1
	}
	public class CodeEvaluationEventArgs : EventArgs {
		public CodeExample CodeExample { get; set; }
		public XtraUserControl RootUserControl { get; set; }
		public ExampleLanguage Language { get; set; }
		public bool Result { get; set; }
	}
	public class CodeExample {
		public string CodeCS { get; set; }
		public string GroupName { get; set; }
		public string UserCodeCS { get; set; }
		public string CodeVB { get; set; }
		public string UserCodeVB { get; set; }
		public string Name { get; set; }
		public MethodInfo MethodInfo { get; set; }
		public CodeExampleGroup Parent { get; set; }
		public string BeginCSCode { get; set; }
		public string EndCSCode { get; set; }
		public string BeginVBCode { get; set; }
		public string EndVBCode { get; set; }
		public MethodInfo TearDown { get; set; }
		public Type[] NestedTypes { get; set; }
		public string UserControlName(bool firstCharToLower) {
			return ToPascalCase(Name, firstCharToLower) + "UserControl";
		}
		public string RegionName() {
			return ToPascalCase(Name, true);
		}
		string ToPascalCase(string input, bool firstCharToLower = false) {
			Regex rgx = new Regex("[^a-zA-Z0-9]");
			input = rgx.Replace(input, " ");
			string[] words = input.Split(
				new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries);
			string result = string.Empty;
			for(int i = 0; i < words.Length; i++) {
				if(firstCharToLower && i == 0) {
					result +=
						 words[i].Substring(0, 1).ToLower() +
						 words[i].Substring(1);
				}
				else {
					result +=
						words[i].Substring(0, 1).ToUpper() +
						words[i].Substring(1);
				}
			}
			return result;
		}
		internal void Reset() {
			UserCodeCS = CodeCS;
			UserCodeVB = CodeVB;
			if(Parent != null && Parent.HasNestedClassStrings && NestedTypes != null && NestedTypes.Length != 0) {
				foreach(var item in Parent.NestedClassStrings) {
					if(NestedTypes.Contains(item.Key)) item.Value.UserCode = item.Value.Code;
				}
			}
		}
	}
	public class CodeExampleGroup {
		public List<CodeExample> Examples { get; set; }
		public string Name { get; set; }
		public string FileName { get; set; }
		public Type RootType { get; set; }
		public MethodInfo SetUp { get; set; }
		public string SetUpCode { get; set; }
		public MethodInfo TearDown { get; set; }
		public string TearDownCode { get; set; }
		public Dictionary<Type, NestedCodeContainer> NestedClassStrings { get; set; }
		public List<Type> NestedTypes { get; set; }
		public string NameSpace() {
			return ToPascalCase(Name);
		}
		public bool HasNestedClassStrings {
			get { return NestedClassStrings != null && NestedClassStrings.Count > 0; }
		}
		string ToPascalCase(string input, bool firstCharToLower = false) {
			Regex rgx = new Regex("[^a-zA-Z0-9]");
			input = rgx.Replace(input, " ");
			string[] words = input.Split(
				new char[] { ' ' },
				StringSplitOptions.RemoveEmptyEntries);
			string result = string.Empty;
			for(int i = 0; i < words.Length; i++) {
				if(firstCharToLower && i == 0) {
					result +=
						 words[i].Substring(0, 1).ToLower() +
						 words[i].Substring(1);
				}
				else {
					result +=
						words[i].Substring(0, 1).ToUpper() +
						words[i].Substring(1);
				}
			}
			return result;
		}
	}
	public class NestedCodeContainer {
		public NestedCodeContainer(string Code) {
			this.Code = this.UserCode = Code;
		}
		public string Code { get; set; }
		public string UserCode { get; set; }
	}
	public class ExampleCodeEditor {
		private readonly List<IRichEditControl> codeEditor;
		private readonly ExampleLanguage current;
		private DateTime lastExampleCodeModifiedTime = DateTime.Now;
		public ExampleCodeEditor(IRichEditControl codeEditor, ExampleLanguage currentLanguage) {
			this.codeEditor = new List<IRichEditControl>();
			this.codeEditor.Add(codeEditor);
			this.current = currentLanguage;
			if(current == ExampleLanguage.Csharp) {
				this.codeEditor[0].InnerControl.InitializeDocument += InitializeSyntaxHighlightForCs;
			}
			else {
				this.codeEditor[0].InnerControl.InitializeDocument += InitializeSyntaxHighlightForVb;
			}
		}
		public List<InnerRichEditControl> CodeEditors {
			get {
				return codeEditor.Select(e => e.InnerControl).ToList();
			}
		}
		public void AddCodeEditor(IRichEditControl codeEditor) {
			this.codeEditor.Add(codeEditor);
		}
		public ExampleLanguage CurrentExampleLanguage {
			get {
				return current;
			}
		}
		public DateTime LastExampleCodeModifiedTime {
			get {
				return lastExampleCodeModifiedTime;
			}
		}
		internal static void DisableRichEditFeatures(IRichEditControl codeEditor) {
			var options = codeEditor.InnerDocumentServer.Options;
			options.DocumentCapabilities.Hyperlinks = DocumentCapability.Disabled;
			options.DocumentCapabilities.Numbering.Bulleted = DocumentCapability.Disabled;
			options.DocumentCapabilities.Numbering.Simple = DocumentCapability.Disabled;
			options.DocumentCapabilities.Numbering.MultiLevel = DocumentCapability.Disabled;
			options.DocumentCapabilities.Tables = DocumentCapability.Disabled;
			options.DocumentCapabilities.Bookmarks = DocumentCapability.Disabled;
			options.DocumentCapabilities.CharacterStyle = DocumentCapability.Disabled;
			options.DocumentCapabilities.ParagraphStyle = DocumentCapability.Disabled;
		}
		internal void InitializeSyntaxHighlightForCs(object sender, EventArgs e) {
			SyntaxHightlightInitializeHelper.Initialize(sender as IRichEditControl, "cs");
			DisableRichEditFeatures(sender as IRichEditControl);
		}
		internal void InitializeSyntaxHighlightForVb(object sender, EventArgs e) {
			SyntaxHightlightInitializeHelper.Initialize(sender as IRichEditControl, "vb");
			DisableRichEditFeatures(sender as IRichEditControl);
		}
		void richEditControl_TextChanged(object sender, EventArgs e) {
			lastExampleCodeModifiedTime = DateTime.Now;
		}
		internal void SubscribeRichEditEvent() {
			foreach(var codeEditor in CodeEditors) {
				codeEditor.ContentChanged += richEditControl_TextChanged;
			}
		}
		internal void UnsubscribeRichEditEvents() {
			foreach(var codeEditor in CodeEditors) {
				codeEditor.ContentChanged -= richEditControl_TextChanged;
			}
		}
		private void UpdatePageBackground(bool codeEvaluated) {
			foreach(var codeEditor in CodeEditors) {
				codeEditor.Document.SetPageBackground((codeEvaluated) ? DXColor.Empty : DXColor.FromArgb(0xFF, 0xBC, 0xC8), true);
			}
		}
		internal void AfterCompile(bool codeExecutedWithoutExceptions) {
			UpdatePageBackground(codeExecutedWithoutExceptions);
			ResetLastExampleModifiedTime();
			SubscribeRichEditEvent();
		}
		internal void BeforeCompile() {
			UnsubscribeRichEditEvents();
		}
		public void ResetLastExampleModifiedTime() {
			lastExampleCodeModifiedTime = DateTime.Now;
		}
		public string ShowExample(CodeExample newExample) {
			var exampleCode = String.Empty;
			if(newExample != null) {
				try {
					exampleCode = (CurrentExampleLanguage == ExampleLanguage.Csharp) ? newExample.UserCodeCS : newExample.UserCodeVB;
					codeEditor[0].InnerControl.Text = exampleCode;
				}
				finally {
				}
			}
			return exampleCode;
		}
	}
	public class ExampleCodeEvaluator {
		public ExampleCodeEvaluator(ExampleLanguage language) {
			ExampleLanguage = language;
		}
		ExampleLanguage ExampleLanguage { get; set; }
		protected CodeDomProvider GetCodeDomProvider() {
			if(ExampleLanguage == CodeDemo.ExampleLanguage.Csharp) return new CSharpCodeProvider();
			return new Microsoft.VisualBasic.VBCodeProvider();
		}
		protected internal bool CompileAndRun(CodeExample codeExample, XtraUserControl userControl) {
			userControl.Controls.Clear();
			var CompilerParams = new CompilerParameters()
			{
				GenerateInMemory = true,
				TreatWarningsAsErrors = false,
				GenerateExecutable = false,
			};
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(codeExample.Parent.RootType.TypeHandle);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly asm in assemblies) {
				if(!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location))
					CompilerParams.ReferencedAssemblies.Add(asm.Location);
			}
			var provider = GetCodeDomProvider();
			CodeSnippetCompileUnit compileUnit = new CodeSnippetCompileUnit(CreateCode(codeExample, ExampleLanguage));
			var compile = provider.CompileAssemblyFromDom(CompilerParams, compileUnit);
			if(compile.Errors.HasErrors) {
				var text = string.Empty;
				foreach(CompilerError ce in compile.Errors) {
					text += (Environment.NewLine + ce);
				}
				XtraMessageBox.Show(text, "Compile errors");  
				return false;
			}
			Module module = null;
			try {
				module = compile.CompiledAssembly.GetModules()[0];
			}
			catch { }
			if(module == null)
				return false;
			object[] targetObjects = new object[0];
			Type moduleType = module.GetType("DXSample.SampleClass");
			if(moduleType == null)
				return false;
			if(codeExample.Parent.SetUp != null) {
				MethodInfo setUp = moduleType.GetMethod(codeExample.Parent.SetUp.Name);
				if(setUp != null) {
					try {
						targetObjects = setUp.Invoke(null, new object[] { userControl }) as object[];
					}
					catch(Exception e) {
						XtraMessageBox.Show(e.ToString(), "Compile errors");
						return false; }
				}
				if(codeExample.Parent.TearDown != null)
					codeExample.TearDown = moduleType.GetMethod(codeExample.Parent.TearDown.Name);
			}
			MethodInfo process = moduleType.GetMethod("Process");
			if(process != null) {
				try {
					process.Invoke(null, targetObjects);
				}
				catch(Exception e) {
					userControl.Controls.Clear();
					string exception = e.InnerException == null ? e.Message : e.InnerException.Message;
					XtraMessageBox.Show(exception, "Compile errors");
					return false;
				}
				return true;
			}
			return false;
		}
		public static string CreateCode(CodeExample codeExample, ExampleLanguage langage) {
			string result = CreateCodeCore(codeExample, langage);
			string nestedClassesString = codeExample.Parent.SetUpCode;
			nestedClassesString += Environment.NewLine + codeExample.Parent.TearDownCode;
			if(codeExample.NestedTypes != null && codeExample.NestedTypes.Length != 0) {
				foreach(string nestedString in CodeTutorialControlBase.GetListStringFromNestedTypes(codeExample, langage)) {
					nestedClassesString += nestedString + Environment.NewLine;
				}
			}
			result = result.Replace(StringContainer.NestedStringToReplace, nestedClassesString);
			return result;
		}
		static string CreateCodeCore(CodeExample codeExample, ExampleLanguage langage) {
			if(langage == CodeDemo.ExampleLanguage.Csharp) return codeExample.BeginCSCode + codeExample.UserCodeCS + codeExample.EndCSCode;
			return codeExample.BeginVBCode + codeExample.UserCodeVB + codeExample.EndVBCode;
		}
		public bool ExecuteCodeAndGenerateLayout(CodeEvaluationEventArgs args) {
			return CompileAndRun(args.CodeExample, args.RootUserControl);
		}
	}
	public class ExampleEvaluatorByTimer : IDisposable {
		public const int CompileTimeIntervalInMilliseconds = 2000;
		bool compileComplete = true;
		public Timer compileExampleTimer;
		readonly LeakSafeCompileEventRouter leakSafeCompileEventRouter;
		public ExampleEvaluatorByTimer()
			: this(true) {
		}
		public ExampleEvaluatorByTimer(bool enableTimer) {
			this.leakSafeCompileEventRouter = new LeakSafeCompileEventRouter(this);
			if(enableTimer) {
				this.compileExampleTimer = new Timer();
				this.compileExampleTimer.Interval = CompileTimeIntervalInMilliseconds;
				this.compileExampleTimer.Tick += new EventHandler(leakSafeCompileEventRouter.OnCompileExampleTimerTick);
				this.compileExampleTimer.Enabled = true;
			}
		}
		public event OnAfterCompileEventHandler OnAfterCompile;
		public event EventHandler OnBeforeCompile;
		public event CodeEvaluationEventHandler QueryEvaluate;
		private void CompileExampleAndShowPrintPreview(CodeEvaluationEventArgs args) {
			var evaluationSucceed = false;
			try {
				RaiseOnBeforeCompile();
				evaluationSucceed = Evaluate(args);
			}
			finally {
				RaiseOnAfterCompile(evaluationSucceed);
			}
		}
		private void RaiseOnAfterCompile(bool result) {
			if(OnAfterCompile != null) {
				OnAfterCompile(this, new OnAfterCompileEventArgs() { Result = result });
			}
		}
		private void RaiseOnBeforeCompile() {
			if(OnBeforeCompile != null) {
				OnBeforeCompile(this, new EventArgs());
			}
		}
		ExampleCodeEvaluator exampleCodeEvaluator;
		protected ExampleCodeEvaluator GetExampleCodeEvaluator(ExampleLanguage language) {
			if(exampleCodeEvaluator == null) {
				exampleCodeEvaluator = new ExampleCodeEvaluator(language);
			}
			return exampleCodeEvaluator;
		}
		protected internal virtual CodeEvaluationEventArgs RaiseQueryEvaluate() {
			if(QueryEvaluate != null) {
				var args = new CodeEvaluationEventArgs();
				QueryEvaluate(this, args);
				return args;
			}
			return null;
		}
		public void CompileExample() {
			if(!compileComplete) {
				return;
			}
			var args = RaiseQueryEvaluate();
			if(!args.Result) {
				return;
			}
			ForceCompile(args);
		}
		public void Dispose() {
			if(compileExampleTimer != null) {
				compileExampleTimer.Enabled = false;
				if(leakSafeCompileEventRouter != null) {
					compileExampleTimer.Tick -= new EventHandler(leakSafeCompileEventRouter.OnCompileExampleTimerTick);
				}
				compileExampleTimer.Dispose();
				compileExampleTimer = null;
			}
		}
		public bool Evaluate(CodeEvaluationEventArgs args) {
			var codeEvaluator = GetExampleCodeEvaluator(args.Language);
			return codeEvaluator.ExecuteCodeAndGenerateLayout(args);
		}
		public void ForceCompile(CodeEvaluationEventArgs args) {
			compileComplete = false;
			if(args.CodeExample != null) {
				CompileExampleAndShowPrintPreview(args);
			}
			compileComplete = true;
		}
	}
	public class LeakSafeCompileEventRouter {
		private readonly WeakReference weakControlRef;
		public LeakSafeCompileEventRouter(ExampleEvaluatorByTimer module) {
			this.weakControlRef = new WeakReference(module);
		}
		public void OnCompileExampleTimerTick(object sender, EventArgs e) {
			var module = (ExampleEvaluatorByTimer)weakControlRef.Target;
			if(module != null) {
				module.CompileExample();
			}
		}
	}
	public class OnAfterCompileEventArgs : EventArgs {
		public bool Result { get; set; }
	}
	[AttributeUsage(AttributeTargets.Class)]
	public class CodeExampleClass : CategoryAttribute {
		public CodeExampleClass(string codeExampleName, string fileName)
			: base(codeExampleName) {
			this.fileName = fileName;
		}
		readonly string fileName;
		public string FileName { get { return fileName; } }
	}
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
	public class CodeExampleNestedClass : CategoryAttribute {
		public CodeExampleNestedClass() : base(string.Empty) { }
		public CodeExampleNestedClass(string description) : base(description) { }
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class CodeExampleCase : CategoryAttribute {
		public CodeExampleCase(string val, Type[] types = null)
			: base(val) {
			this.Types = types;
		}
		public Type[] Types;
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class CodeExampleGroupName : CategoryAttribute {
		public CodeExampleGroupName(string groupName)
			: base(groupName) {
			GroupName = groupName;
		}
		public String GroupName { get; set; }
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class CodeExampleSetUp : Attribute {
	}
	[AttributeUsage(AttributeTargets.Method)]
	public class CodeExampleTearDown : Attribute {
	}
	internal static class StringContainer {
		internal static string Usings = "/* Usings */";
		internal static string SetUp = "/* SetUpCode */";
		internal static string TearDown = "/* TearDownCode */";
		internal static string ExampleMethod = "/* ExampleMethod */";
		internal static string NestedClasses = "/* NestedClasses */";
		internal static string UserControlName = "/* UserControlName */";
		internal static string RegionExampleCodeName = "/* RegionExampleCodeName */";
		internal static string NameSpace = "/* NameSpace */";
		internal static string NestedStringToReplace = "/* NestedStringToReplace */";
		#region  UserControlCSCode
		internal static string UserControlCSCode =
				Usings + Environment.NewLine +
				"namespace " + NameSpace + " {" + Environment.NewLine +
				new string(' ', 04) + "public partial class " + UserControlName + " : XtraUserControl {" + Environment.NewLine +
				new string(' ', 08) + "public " + UserControlName + "() {" + Environment.NewLine +
				new string(' ', 12) + "InitializeComponent();" + Environment.NewLine +
				new string(' ', 12) + "#region SetUp" + Environment.NewLine +
				SetUp +
				new string(' ', 12) + "#endregion SetUp" + Environment.NewLine +
				Environment.NewLine +
				new string(' ', 12) + "#region #" + RegionExampleCodeName + Environment.NewLine +
				ExampleMethod +
				new string(' ', 12) + "#endregion #" + RegionExampleCodeName + Environment.NewLine +
				new string(' ', 08) + "}" + Environment.NewLine +
				new string(' ', 08) + "#region CleanUp" + Environment.NewLine +
				new string(' ', 08) + "void OnDisposing() {" + Environment.NewLine +
				TearDown +
				new string(' ', 08) + "}" + Environment.NewLine +
				new string(' ', 08) + "#endregion CleanUp" + Environment.NewLine +
				NestedClasses +
				new string(' ', 04) + "}" + Environment.NewLine +
				"}";
		#endregion
		#region UserControlDesignerCSCode
		internal static string UserControlDesignerCSCode =
@"namespace " + NameSpace + @" {
    partial class " + UserControlName + @" {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name=""disposing"">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if(disposing && (components != null)) {
                OnDisposing();
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        }

        #endregion
    }
}";
		#endregion
		#region SLNString
		internal static string SLNString =
@"

Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2013
VisualStudioVersion = 12.0.31101.0
MinimumVisualStudioVersion = 10.0.40219.1
Project(""{//GUIDSLN}"") = ""DxSample"", ""DxSample.#csprojOrvbprog"", ""{//GUIDCSproj}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{//GUIDCSproj}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{//GUIDCSproj}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{//GUIDCSproj}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{//GUIDCSproj}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal

";
		#endregion
		#region UserControlDesignerVBCode
		internal static string UserControlDesignerVBCode =
@"Namespace " + NameSpace + @"
    Partial Class " + UserControlName + @"

        'UserControl overrides dispose to clean up the component list.
        <System.Diagnostics.DebuggerNonUserCode()> _
        Protected Overrides Sub Dispose(ByVal disposing As Boolean)
            Try
                If disposing AndAlso components IsNot Nothing Then
                    OnDisposing()
                    components.Dispose()
                End If
            Finally
                MyBase.Dispose(disposing)
            End Try
        End Sub

        'Required by the Windows Form Designer
        Private components As System.ComponentModel.IContainer

        'NOTE: The following procedure is required by the Windows Form Designer
        'It can be modified using the Windows Form Designer.  
        'Do not modify it using the code editor.
        <System.Diagnostics.DebuggerStepThrough()> _
        Private Sub InitializeComponent()
            components = New System.ComponentModel.Container()
            Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        End Sub
    End Class
End Namespace";
		#endregion
		#region  UserControlVBCode
		internal static string UserControlVBCode =
			   Usings + Environment.NewLine +
				"Namespace " + NameSpace + Environment.NewLine +
				new string(' ', 04) + "Partial Public Class " + UserControlName + Environment.NewLine +
				new string(' ', 08) + "Inherits DevExpress.XtraEditors.XtraUserControl" + Environment.NewLine +
				new string(' ', 08) + "Public Sub New() " + Environment.NewLine +
				new string(' ', 12) + "InitializeComponent()" + Environment.NewLine +
				SetUp +
				Environment.NewLine +
				ExampleMethod +
				new string(' ', 08) + "End Sub" + Environment.NewLine +
				new string(' ', 08) + " Private Sub OnDisposing()" + Environment.NewLine +
				TearDown +
				new string(' ', 08) + "End Sub" + Environment.NewLine +
				NestedClasses +
				new string(' ', 04) + "	End Class" + Environment.NewLine +
				"End Namespace";
	}
		#endregion
}
