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
using System.IO;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DevExpress.CodeParser;
using DevExpress.XtraLayout;
using DevExpress.XtraTreeList;
using DevExpress.CodeParser.VB;
using DevExpress.CodeParser.CSharp;
using DevExpress.XtraTreeList.Columns;
using System.Collections;
using DevExpress.XtraBars;
using System.Drawing;
using System.Windows.Forms;
namespace DevExpress.DXperience.Demos.CodeDemo {
	[ToolboxItem(false)]
	public partial class CodeTutorialControlBase : TutorialControlBase {
		ExampleCodeEditor codeEditor;
		ExampleEvaluatorByTimer evaluator;
		public List<CodeExampleGroup> examples;
		readonly List<RichEditUserControl> richControls;
		CodeExample SelectedExample;
		bool treeListRootNodeLoading = true;
		readonly RichEditUserControl richEditUserControlForExampleCode;
		ComponentResourceManager resources;
		public CodeTutorialControlBase() {
			CreateWaitDialog();
			InitializeComponent();
			resources = new ComponentResourceManager(typeof(CodeTutorialControlBase));
			richEditUserControlForExampleCode = new RichEditUserControl();
			richControls = new List<RichEditUserControl>();
			richControls.Add(richEditUserControlForExampleCode);
			Assembly callingAssembly = GetCallingAssembly();
			Type[] typesFromAssembly = callingAssembly.GetTypes();
			examples = FillExamplesGroupFormType(typesFromAssembly);
			FillCodeExamplesFromResourceFiles(callingAssembly, ref examples);
			ShowExamplesInTreeList(codeTreeList);
			this.codeEditor = new ExampleCodeEditor(richEditUserControlForExampleCode.richEditControl, CurrentExampleLanguage);
			evaluator = new ExampleEvaluatorByTimer();
			this.evaluator.QueryEvaluate += OnExampleEvaluatorQueryEvaluate;
			this.evaluator.OnBeforeCompile += evaluator_OnBeforeCompile;
			this.evaluator.OnAfterCompile += evaluator_OnAfterCompile;
			ShowFirstExample();
		}
		protected virtual Assembly GetCallingAssembly() {
			return GetType().Assembly;
		}
		protected override void DoShow() {
			base.DoShow();
			if(ParentFormMain != null) {
				SetExportBarItemAvailability(ParentFormMain.ShowInVisualStudio, true, true);
				ParentFormMain.ShowInVisualStudio.ItemClick += visualStudioSimpleButton_Click;
				if(ParentFormMain.ShowInVisualStudio.Glyph == null) {
					ParentFormMain.ShowInVisualStudio.Glyph = resources.GetObject(String.Format("{0}.Glyph", CurrentExampleLanguage == ExampleLanguage.Csharp ? "cs" : "vb")) as Image;
					ParentFormMain.ShowInVisualStudio.LargeGlyph = resources.GetObject(String.Format("{0}.LargeGlyph", CurrentExampleLanguage == ExampleLanguage.Csharp ? "cs" : "vb")) as Image;
				}
			}
		}
		protected override void DoHide() {
			base.DoHide();
			if(ParentFormMain != null) ParentFormMain.ShowInVisualStudio.ItemClick -= visualStudioSimpleButton_Click;
		}
		protected virtual bool UseSameTutorialControlNameForGenerateExample { get { return false; } }
		protected virtual string[] FileNamesForModule { get { return new string[] { }; } }
		protected virtual ExampleLanguage CurrentExampleLanguage {
			get { return ExampleLanguage.Csharp; }
		}
		#region Initialize CodeExamples
		void FillCodeExamplesFromResourceFiles(Assembly callingAssembly, ref List<CodeExampleGroup> examples) {
			string[] names = callingAssembly.GetManifestResourceNames();
			foreach(CodeExampleGroup codeExampleGroup in examples) {
				string fileNameInAssembly;
				try {
					fileNameInAssembly = names.First(e => e.Contains(codeExampleGroup.FileName));
				}
				catch {
					throw new ApplicationException(String.Format("Add {0} link.", codeExampleGroup.FileName));
				}
				string code;
				using(Stream stream = callingAssembly.GetManifestResourceStream(fileNameInAssembly)) {
					var sr = new StreamReader(stream);
					code = sr.ReadToEnd();
				}
				if(CurrentExampleLanguage == ExampleLanguage.Csharp)
					FillCSCodeExampleCore(codeExampleGroup, code);
				else
					FillVBCodeExampleCore(codeExampleGroup, code);
			}
		}
		public static string GetModuleMethodCode(Type moduleType, string methodName) {
			string moduleResourceInAssembly = moduleType.Assembly.GetManifestResourceNames()
				.Where(n => !string.IsNullOrEmpty(n) && n.Length > 3)
				.FirstOrDefault(n => n.Substring(0, n.Length - 3).EndsWith(moduleType.Name));
			if(string.IsNullOrEmpty(moduleResourceInAssembly))
				return null;
			var code = GetModuleCode(moduleType.Assembly, moduleResourceInAssembly);
			if(!string.IsNullOrEmpty(code)) {
				ExampleLanguage language = moduleResourceInAssembly.EndsWith(".cs") ?
					ExampleLanguage.Csharp : ExampleLanguage.VB;
				using(var reader = new SourceStringReader(code)) {
					SourceFile sourceFile = (language == ExampleLanguage.Csharp) ?
							new CSharp30Parser().Parse(reader) as SourceFile :
							new VB90Parser().Parse(reader) as SourceFile;
					code = GetMethodCode(sourceFile, GetLinesFromCodeString(code), moduleType, methodName);
				}
			}
			return code;
		}
		public static string GetModuleClassCode(Type moduleType, string className) {
			string moduleResourceInAssembly = moduleType.Assembly.GetManifestResourceNames()
				.Where(n => !string.IsNullOrEmpty(n) && n.Length > 3)
				.FirstOrDefault(n => n.Substring(0, n.Length - 3).EndsWith(moduleType.Name));
			if(string.IsNullOrEmpty(moduleResourceInAssembly))
				return null;
			var code = GetModuleCode(moduleType.Assembly, moduleResourceInAssembly);
			if(!string.IsNullOrEmpty(code)) {
				ExampleLanguage language = moduleResourceInAssembly.EndsWith(".cs") ?
					ExampleLanguage.Csharp : ExampleLanguage.VB;
				using(var reader = new SourceStringReader(code)) {
					SourceFile sourceFile = (language == ExampleLanguage.Csharp) ?
							new CSharp30Parser().Parse(reader) as SourceFile :
							new VB90Parser().Parse(reader) as SourceFile;
					code = GetClassCode(sourceFile, GetLinesFromCodeString(code), moduleType, className);
				}
			}
			return code;
		}
		static string GetMethodCode(SourceFile sourceFile, string[] lines, Type moduleType, string methodName) {
			Class classElement = sourceFile.FindChildByName(moduleType.Name) as Class;
			if(classElement != null) {
				Method methodElement = classElement.AllMethods.OfType<Method>()
					.FirstOrDefault(m => m.Name == methodName) as Method;
				if(methodElement != null)
					return GetElementCode(lines, methodElement);
				return GetElementCode(lines, classElement);
			}
			return null;
		}
		static string GetClassCode(SourceFile sourceFile, string[] lines, Type moduleType, string className) {
			Class classElement = sourceFile.FindChildByName(className) as Class;
			if(classElement != null) {
				string result = string.Empty;
				int tabCount = lines[classElement.StartLine - classElement.AttributeCount - 1]
					.TakeWhile(Char.IsWhiteSpace).Count();
				for(int i = classElement.StartLine - classElement.AttributeCount - 1; i < classElement.EndLine; i++)
					result += NewLineWithoutStartTabulation(lines[i], tabCount);
				return result;
			}
			return null;
		}
		static string GetModuleCode(Assembly moduleAssembly, string fileNameInAssembly) {
			using(Stream stream = moduleAssembly.GetManifestResourceStream(fileNameInAssembly)) {
				using(var reader = new StreamReader(stream))
					return reader.ReadToEnd();
			}
		}
		void FillVBCodeExampleCore(CodeExampleGroup codeExampleGroup, string code) {
			VB90Parser parserVB = new VB90Parser();
			SourceFile sourceFileVBLanguageElement = parserVB.Parse(new SourceStringReader(code)) as SourceFile;
			string[] linesOfCode = GetLinesFromCodeString(code);
			Class groupClassElement = sourceFileVBLanguageElement.FindChildByName(codeExampleGroup.RootType.Name) as Class;
			if(codeExampleGroup.SetUp != null) {
				var setup = groupClassElement.FindChildByName(codeExampleGroup.SetUp.Name);
				codeExampleGroup.SetUpCode = GetCodeFromLanguageElement(setup, linesOfCode, true);
			}
			if(codeExampleGroup.TearDown != null) {
				var tearDown = groupClassElement.FindChildByName(codeExampleGroup.TearDown.Name);
				codeExampleGroup.TearDownCode = GetCodeFromLanguageElement(tearDown, linesOfCode, true);
			}
			foreach(Type nestedType in codeExampleGroup.NestedTypes) {
				Class nestedClass = groupClassElement.FindChildByName(nestedType.Name) as Class;
				FillNestedClassStrings(codeExampleGroup, nestedType, linesOfCode, nestedClass);
			}
			foreach(CodeExample codeExample in codeExampleGroup.Examples) {
				VBMethod method = (groupClassElement.FindChildByName(codeExample.MethodInfo.Name)) as VBMethod;
				codeExample.BeginVBCode = GetBeginVBCodeFromUsingList(sourceFileVBLanguageElement.UsingList, linesOfCode[method.StartLine - 1]);
				codeExample.EndVBCode = String.Format("{0}{3}{1}{3}{2}{3}", "End Sub", "End Class", "End Namespace", Environment.NewLine);
				codeExample.CodeVB = codeExample.UserCodeVB = GetCodeFromLanguageElement(method, linesOfCode, false);
			}
		}
		void FillCSCodeExampleCore(CodeExampleGroup codeExampleGroup, string code) {
			CSharp30Parser parserCS = new CSharp30Parser();
			SourceFile sourceFileCSLanguageElement = parserCS.Parse(new SourceStringReader(code)) as SourceFile;
			string[] linesOfCode = GetLinesFromCodeString(code);
			Class groupClassElement = sourceFileCSLanguageElement.FindChildByName(codeExampleGroup.RootType.Name) as Class;
			if(codeExampleGroup.SetUp != null) {
				var setup = groupClassElement.FindChildByName(codeExampleGroup.SetUp.Name);
				codeExampleGroup.SetUpCode = GetCodeFromLanguageElement(setup, linesOfCode, true);
			}
			if(codeExampleGroup.TearDown != null) {
				var tearDown = groupClassElement.FindChildByName(codeExampleGroup.TearDown.Name);
				codeExampleGroup.TearDownCode = GetCodeFromLanguageElement(tearDown, linesOfCode, true);
			}
			foreach(Type nestedType in codeExampleGroup.NestedTypes) {
				Class nestedClass = groupClassElement.FindChildByName(nestedType.Name) as Class;
				FillNestedClassStrings(codeExampleGroup, nestedType, linesOfCode, nestedClass);
			}
			foreach(CodeExample codeExample in codeExampleGroup.Examples) {
				Method method = groupClassElement.FindChildByName(codeExample.MethodInfo.Name) as Method;
				codeExample.BeginCSCode = GetBeginCSCodeFromUsingList(sourceFileCSLanguageElement.UsingList, linesOfCode[method.StartLine - 1]);
				codeExample.EndCSCode = String.Format("{0}{1}{0}{1}{0}{1}", "}", Environment.NewLine);
				codeExample.CodeCS = codeExample.UserCodeCS = GetCodeFromLanguageElement(method, linesOfCode, false);
			}
		}
		void FillNestedClassStrings(CodeExampleGroup codeExampleGroup, Type nestedType, string[] linesOfCode, Class nestedClass) {
			if(codeExampleGroup.NestedClassStrings == null)
				codeExampleGroup.NestedClassStrings = new Dictionary<Type, NestedCodeContainer>();
			string result = GetNestedClassCode(linesOfCode, nestedClass);
			NestedCodeContainer nestedCodeContainer = new NestedCodeContainer(result);
			codeExampleGroup.NestedClassStrings.Add(nestedType, nestedCodeContainer);
		}
		static string GetElementCode(string[] linesOfCode, DocumentElement element, int tabCount = 8) {
			string result = string.Empty;
			int startCorrection = element is Method ? -1 : 0;
			int endCorrection = element is Method ? 0 : -1;
			for(int i = element.StartLine + startCorrection; i < element.EndLine + endCorrection; i++) {
				result += NewLineWithoutStartTabulation(linesOfCode[i], tabCount);
			}
			return result;
		}
		static string GetNestedClassCode(string[] linesOfCode, Class nestedClass) {
			string result = string.Empty;
			for(int i = nestedClass.StartLine - nestedClass.AttributeCount; i < nestedClass.EndLine; i++) {
				result += NewLineWithoutStartTabulation(linesOfCode[i]);
			}
			return result;
		}
		static string NewLineWithoutStartTabulation(string lineOfCode, int tabCount = 8) {
			if(lineOfCode.Length > 8)
				return (lineOfCode + Environment.NewLine).Substring(tabCount); 
			else
				return (lineOfCode + Environment.NewLine);
		}
		string GetBeginVBCodeFromUsingList(SortedList sortedList, string line) {
			string result = String.Empty;
			foreach(var usingString in sortedList.GetValueList()) {
				result += String.Format("Imports {0} {1}", usingString, Environment.NewLine);
			}
			string paramsMethod = Regex.Match(line, @"\(([^\)]*)\)").Value;
			result += String.Format("Namespace DXSample {0} Public Class SampleClass {0} {2} {0} Public Shared Sub Process{1} {0}", Environment.NewLine, paramsMethod, StringContainer.NestedStringToReplace);
			return result;
		}
		static string[] GetLinesFromCodeString(string code) {
			List<string> result = new List<string>();
			using(StringReader reader = new StringReader(code)) {
				while(true) {
					string s = reader.ReadLine();
					if(s == null) break;
					result.Add(s);
				}
			}
			return result.ToArray();
		}
		string GetBeginCSCodeFromUsingList(SortedList sortedList, string line) {
			string result = String.Empty;
			foreach(var usingString in sortedList.GetValueList()) {
				result += String.Format("using {0}; {1}", usingString, Environment.NewLine);
			}
			string paramsMethod = Regex.Match(line, @"\(([^\)]*)\)").Value;
			result += String.Format("namespace DXSample {0} {1} public class SampleClass {0} {1} {3} {1} public static void Process{2} {0} {1}", "{", Environment.NewLine, paramsMethod, StringContainer.NestedStringToReplace);
			return result;
		}
		string GetCodeFromLanguageElement(LanguageElement method, string[] linesOfCode, bool tearDownOrSetUp) {
			string result = string.Empty;
			for(int i = method.StartLine + (tearDownOrSetUp ? -1 : 0); i < method.EndLine + (tearDownOrSetUp ? 0 : -1); i++) {
				result += NewLineWithoutStartTabulation(linesOfCode[i]);
			}
			return result;
		}
		List<CodeExampleGroup> FillExamplesGroupFormType(Type[] typesFromAssembly) {
			List<CodeExampleGroup> result = new List<CodeExampleGroup>();
			foreach(Type type in typesFromAssembly) {
				var exampleAttributes = type.GetCustomAttributes(typeof(CodeExampleClass), false);
				if(exampleAttributes.Length == 1) {
					string fileName = (exampleAttributes[0] as CodeExampleClass).FileName;
					if(UseSameTutorialControlNameForGenerateExample && !this.GetType().Name.Contains(fileName.Substring(0, fileName.Length - 3))) continue;
					if(FileNamesForModule != null && FileNamesForModule.Length != 0 && !FileNamesForModule.Contains(fileName)) continue;
					CodeExampleGroup group = new CodeExampleGroup()
					{
						RootType = type,
						FileName = fileName,
						Name = (exampleAttributes[0] as CodeExampleClass).Category,
						SetUp = type.GetMethods().Where(e => e.GetCustomAttributes(typeof(CodeExampleSetUp), false).Any()).FirstOrDefault(),
						TearDown = type.GetMethods().Where(e => e.GetCustomAttributes(typeof(CodeExampleTearDown), false).Any()).FirstOrDefault(),
						NestedTypes = type.GetNestedTypes().Where(e => e.GetCustomAttributes(typeof(CodeExampleNestedClass), false).Any()).ToList()
					};
					group.Examples = FillCodeExamplesFromType(group);
					result.Add(group);
				}
			}
			return result;
		}
		List<CodeExample> FillCodeExamplesFromType(CodeExampleGroup group) {
			List<CodeExample> codeExamples = new List<CodeExample>();
			foreach(MethodInfo mi in group.RootType.GetMethods()) {
				var methodAttributes = mi.GetCustomAttributes(typeof(CodeExampleCase), false);
				if(methodAttributes.Length == 1) {
					CodeExample codeExample = new CodeExample();
					codeExample.MethodInfo = mi;
					codeExample.Name = (methodAttributes[0] as CodeExampleCase).Category;
					codeExample.NestedTypes = (methodAttributes[0] as CodeExampleCase).Types;
					codeExample.Parent = group;
					var codeExampleGroupNameAttribute = mi.GetCustomAttributes(typeof(CodeExampleGroupName), false);
					if(codeExampleGroupNameAttribute.Length == 1) {
						codeExample.GroupName = (codeExampleGroupNameAttribute[0] as CodeExampleGroupName).GroupName;
					}
					codeExamples.Add(codeExample);
				}
			}
			return codeExamples;
		}
		internal static List<string> GetListStringFromNestedTypes(CodeExample newExample, ExampleLanguage language) {
			List<string> result = new List<string>();
			foreach(Type nestedType in newExample.NestedTypes) {
				NestedCodeContainer nestedClassCode;
				if(newExample.Parent.NestedClassStrings.TryGetValue(nestedType, out nestedClassCode)) {
					CodeExampleNestedClass attribute = nestedType.GetCustomAttributes(typeof(CodeExampleNestedClass), false)[0] as CodeExampleNestedClass;
					result.Add(CreateCodeWithDescription(nestedClassCode.UserCode, language, attribute.Category));
				}
			}
			return result;
		}
		static string CreateCodeWithDescription(string nestedClassCode, ExampleLanguage language, string description) {
			if(string.IsNullOrEmpty(description)) return nestedClassCode;
			if(nestedClassCode.Contains(description)) return nestedClassCode;
			string result = string.Empty;
			if(language == ExampleLanguage.Csharp) {
				result += "// " + Environment.NewLine;
				result += "// " + description + Environment.NewLine;
				result += "// " + Environment.NewLine;
				result += nestedClassCode;
			}
			else {
				result += "'' " + Environment.NewLine;
				result += "'' " + description + Environment.NewLine;
				result += "'' " + Environment.NewLine;
				result += nestedClassCode;
			}
			return result;
		}
		#endregion
		void InitializeCodeEvaluationEventArgs(CodeEvaluationEventArgs e) {
			try {
				InitializeCodeEvaluationEventArgsCore(e);
			}
			catch {
				e.Result = false;
			}
		}
		void InitializeCodeEvaluationEventArgsCore(CodeEvaluationEventArgs e) {
			var selectedNode = codeTreeList.GetDataRecordByNode(codeTreeList.Selection[0]);
			CodeExample example = GetCodeExampleFromDataRecordByNode(selectedNode);
			e.CodeExample = example;
			if(CurrentExampleLanguage == ExampleLanguage.Csharp) e.CodeExample.UserCodeCS = richEditUserControlForExampleCode.richEditControl.Text;
			else e.CodeExample.UserCodeVB = richEditUserControlForExampleCode.richEditControl.Text;
			if(e.CodeExample.NestedTypes != null) {
				foreach(var type in e.CodeExample.NestedTypes) {
					NestedCodeContainer codeContainer = null;
					if(e.CodeExample.Parent.NestedClassStrings.TryGetValue(type, out codeContainer)) {
						string text = richControls.Where(rich => rich.CurrentNestedType == type).First().richEditControl.Text;
						codeContainer.UserCode = text;
					}
				}
			}
			e.RootUserControl = rootContainer;
			e.Language = CurrentExampleLanguage;
			e.Result = true;
		}
		void evaluator_OnAfterCompile(object sender, OnAfterCompileEventArgs args) {
			codeEditor.AfterCompile(args.Result);
		}
		void evaluator_OnBeforeCompile(object sender, EventArgs e) {
			codeEditor.BeforeCompile();
		}
		void ShowFirstExample() {
			codeTreeList.ExpandAll();
			if(codeTreeList.Nodes.Count > 0)
				codeTreeList.FocusedNode = codeTreeList.MoveFirst().FirstNode;
		}
		void OnExampleEvaluatorQueryEvaluate(object sender, CodeEvaluationEventArgs e) {
			e.Result = false;
			if(CheckRichTextChanged()) {
				TimeSpan span = DateTime.Now - codeEditor.LastExampleCodeModifiedTime;
				if(span < TimeSpan.FromMilliseconds(1000)) {
					codeEditor.ResetLastExampleModifiedTime();
					return;
				}
				InitializeCodeEvaluationEventArgs(e);
				InvokeTearDown(e.CodeExample);
			}
		}
		bool CheckRichTextChanged() {
			var selectedNode = codeTreeList.GetDataRecordByNode(codeTreeList.Selection[0]);
			CodeExample example = GetCodeExampleFromDataRecordByNode(selectedNode);
			if(example == null) return false;
			if(CurrentExampleLanguage == ExampleLanguage.Csharp && SelectedExample.UserCodeCS != richEditUserControlForExampleCode.richEditControl.Text)
				return true;
			if(CurrentExampleLanguage == ExampleLanguage.VB && SelectedExample.UserCodeVB != richEditUserControlForExampleCode.richEditControl.Text)
				return true;
			if(example.NestedTypes != null) {
				foreach(var type in example.NestedTypes) {
					NestedCodeContainer codeContainer = null;
					if(SelectedExample.Parent.NestedClassStrings.TryGetValue(type, out codeContainer)) {
						string text = richControls.Where(rich => rich.CurrentNestedType == type).First().richEditControl.Text;
						if(codeContainer.UserCode != text) return true;
					}
				}
			}
			return false;
		}
		CodeExample GetCodeExampleFromDataRecordByNode(object dataRecord) {
			if(dataRecord is CodeExampleGroup) return (dataRecord as CodeExampleGroup).Examples.First();
			if(dataRecord is GroupNode) return (dataRecord as GroupNode).Children[0] as CodeExample;
			if(dataRecord is CodeExample) return dataRecord as CodeExample;
			return null;
		}
		void ShowExamplesInTreeList(TreeList treeList) {
			treeList.OptionsPrint.UsePrintStyles = true;
			treeList.FocusedNodeChanged += OnNewExampleSelected;
			treeList.OptionsView.ShowColumns = false;
			treeList.OptionsView.ShowIndicator = false;
			treeList.VirtualTreeGetChildNodes += OnVirtualTreeGetChildNodes;
			treeList.VirtualTreeGetCellValue += OnVirtualTreeGetCellValue;
			TreeListColumn col1 = new TreeListColumn();
			col1.VisibleIndex = 0;
			col1.OptionsColumn.AllowEdit = false;
			col1.OptionsColumn.AllowMove = false;
			col1.OptionsColumn.ReadOnly = true;
			treeList.Columns.AddRange(new TreeListColumn[] { col1 });
			treeList.DataSource = new object();
			treeList.ExpandAll();
		}
		void OnVirtualTreeGetCellValue(object sender, VirtualTreeGetCellValueInfo args) {
			CodeExampleGroup group = args.Node as CodeExampleGroup;
			if(group != null)
				args.CellData = group.Name;
			CodeExample example = args.Node as CodeExample;
			if(example != null)
				args.CellData = example.Name;
			GroupNode groupNode = args.Node as GroupNode;
			if(groupNode != null)
				args.CellData = groupNode.Name;
		}
		void OnVirtualTreeGetChildNodes(object sender, VirtualTreeGetChildNodesInfo args) {
			if(treeListRootNodeLoading) {
				args.Children = examples.Where(e => e.Examples.Any()).ToArray();
				treeListRootNodeLoading = false;
			}
			else {
				if(args.Node == null)
					return;
				CodeExampleGroup group = args.Node as CodeExampleGroup;
				if(group != null) {
					List<object> result = new List<object>();
					var examplesWithGroupName = group.Examples.Select((x) => x).Where((x) => x.GroupName != null).GroupBy((x) => x.GroupName).ToList();
					var examplesWithoutGroupName = group.Examples.Select((x) => x).Where((x) => x.GroupName == null).ToList();
					result.AddRange(examplesWithoutGroupName);
					if(examplesWithGroupName.Count > 0) {
						var groups = examplesWithGroupName.Select((x) => new GroupNode(x.Key, x.ToList())).ToList();
						result.AddRange(groups);
					}
					args.Children = result;
				}
				GroupNode groupNode = args.Node as GroupNode;
				if(groupNode != null)
					args.Children = groupNode.Children;
			}
		}
		void OnNewExampleSelected(object sender, FocusedNodeChangedEventArgs e) {
			var newNode = (sender as TreeList).GetDataRecordByNode(e.Node);
			var oldNode = (sender as TreeList).GetDataRecordByNode(e.OldNode);
			CodeExample oldExample = null, newExample = null;
			newExample = GetCodeExampleFromDataRecordByNode(newNode);
			oldExample = GetCodeExampleFromDataRecordByNode(oldNode);
			if(newExample == null)
				return;
			if(oldExample != null)
				InvokeTearDown(oldExample);
			SelectedExample = newExample;
			CreateLayoutForExample(newExample);
			codeEditor.ShowExample(newExample);
			CodeEvaluationEventArgs args = new CodeEvaluationEventArgs();
			InitializeCodeEvaluationEventArgs(args);
			evaluator.ForceCompile(args);
		}
		void InvokeTearDown(CodeExample oldExample) {
			try {
				if(oldExample.TearDown != null)
					oldExample.TearDown.Invoke(null, new object[] { rootContainer });
				if(rootContainer != null)
					CleanUp(rootContainer);
			}
			catch { }
		}
		void CleanUp(UserControl container) {
			Control[] controls = new Control[container.Controls.Count];
			container.Controls.CopyTo(controls, 0);
			for(int i = 0; i < controls.Length; i++)
				controls[i].Dispose();
			container.Controls.Clear();
		}
		void visualStudioSimpleButton_Click(object sender, ItemClickEventArgs e) {
			if(SelectedExample == null) return;
			string pathToSolution = CreateProject(CurrentExampleLanguage);
			try {
				string endFileName = CurrentExampleLanguage == ExampleLanguage.Csharp ? "cs" : "vb";
				DevExpress.DemoData.Model.DemoRunner.RunProject(pathToSolution + "\\DxSample.sln", new string[] { SelectedExample.UserControlName(false) + "." + endFileName });
			}
			catch { }
		}
		#region CreateUIForNestedClass
		void CreateLayoutForExample(CodeExample newExample) {
			layoutControlForExampleCode.BeginUpdate();
			layoutControlForExampleCode.Clear(true, false);
			if(newExample.Parent.HasNestedClassStrings && newExample.NestedTypes != null) {
				TabbedControlGroup tabbedControlgroup = InitializeTabbedControlGroup();
				if(newExample.NestedTypes.Length > richControls.Count - 1) {
					CreateRichIfNeeded(newExample);
				}
				FillTabbedControlGroup(newExample, tabbedControlgroup);
				tabbedControlgroup.SelectedTabPageIndex = 0;
			}
			else {
				layoutControlForExampleCode.AddItem("", richControls[0]).TextVisible = false;
			}
			layoutControlForExampleCode.EndUpdate();
		}
		private TabbedControlGroup InitializeTabbedControlGroup() {
			TabbedControlGroup tabbedControlgroup = layoutControlForExampleCode.Root.AddTabbedGroup();
			LayoutControlGroup group = tabbedControlgroup.AddTabPage();
			group.Text = "ExampleCode";
			group.AddItem("", richControls[0]).TextVisible = false;
			return tabbedControlgroup;
		}
		void FillTabbedControlGroup(CodeExample newExample, TabbedControlGroup tabbedControlgroup) {
			List<string> nestedCodeStrings = GetListStringFromNestedTypes(newExample, CurrentExampleLanguage);
			for(int i = 0; i < newExample.NestedTypes.Length; i++) {
				LayoutControlGroup nestedGroup = tabbedControlgroup.AddTabPage();
				nestedGroup.AddItem("", richControls[i + 1]).TextVisible = false;
				nestedGroup.Text = newExample.NestedTypes[i].Name + (CurrentExampleLanguage == ExampleLanguage.Csharp ? ".cs" : ".vb");
				richControls[i + 1].richEditControl.Text = nestedCodeStrings[i];
				richControls[i + 1].CurrentNestedType = newExample.NestedTypes[i];
			}
		}
		void CreateRichIfNeeded(CodeExample newExample) {
			for(int i = richControls.Count - 1; i < newExample.NestedTypes.Length; i++) {
				RichEditUserControl nestedRichEditControl = new RichEditUserControl();
				codeEditor.AddCodeEditor(nestedRichEditControl.richEditControl);
				richControls.Add(nestedRichEditControl);
				if(codeEditor.CurrentExampleLanguage == ExampleLanguage.Csharp) richControls[i + 1].richEditControl.InitializeDocument += codeEditor.InitializeSyntaxHighlightForCs;
				else richControls[i + 1].richEditControl.InitializeDocument += codeEditor.InitializeSyntaxHighlightForVb;
			}
		}
		#endregion
		#region ProjectGenerator
		string CreateProject(ExampleLanguage language) {
			string resultUserControlDesignerCode = string.Empty;
			string resultUserControlCode = language == ExampleLanguage.Csharp ? GetCSUserControlCode(SelectedExample, ref resultUserControlDesignerCode) : GetVBUserControlCode(SelectedExample, ref resultUserControlDesignerCode);
			string pathToSolution = Path.GetTempFileName();
			if(File.Exists(pathToSolution)) File.Delete(pathToSolution);
			if(!Directory.Exists(pathToSolution)) Directory.CreateDirectory(pathToSolution);
			string endFileName = language == ExampleLanguage.Csharp ? "cs" : "vb";
			WriteResourceToFile("DevExpress.Tutorials.CodeDemoBase.Template" + endFileName.ToUpper() + "Project.Form1." + endFileName, pathToSolution + "\\Form1." + endFileName);
			WriteResourceToFile("DevExpress.Tutorials.CodeDemoBase.Template" + endFileName.ToUpper() + "Project.Program." + endFileName, pathToSolution + "\\Program." + endFileName);
			string formDesignerCode = GetFormDesignerString(CurrentExampleLanguage);
			formDesignerCode = formDesignerCode.Replace("//UpperName", SelectedExample.UserControlName(false));
			formDesignerCode = formDesignerCode.Replace("//LowerName", SelectedExample.UserControlName(true));
			formDesignerCode = formDesignerCode.Replace("//NameSpace", "DxSample" + SelectedExample.Parent.NameSpace());
			File.WriteAllText(pathToSolution + "\\Form1.Designer." + endFileName, formDesignerCode);
			string csprojString = GetPROJString(CurrentExampleLanguage);
			string guidCsproj = Guid.NewGuid().ToString().ToUpper();
			csprojString = csprojString.Replace("//Guid", guidCsproj);
			csprojString = csprojString.Replace("//Reference", GetReferenceString(SelectedExample));
			csprojString = csprojString.Replace("//UpperName", SelectedExample.UserControlName(false));
			File.WriteAllText(pathToSolution + "\\DxSample.sln", GetSLNString(guidCsproj, CurrentExampleLanguage));
			File.WriteAllText(pathToSolution + "\\DxSample." + endFileName + "proj", csprojString);
			File.WriteAllText(pathToSolution + "\\" + SelectedExample.UserControlName(false) + ".Designer." + endFileName, resultUserControlDesignerCode);
			File.WriteAllText(pathToSolution + "\\" + SelectedExample.UserControlName(false) + "." + endFileName, resultUserControlCode);
			return pathToSolution;
		}
		string GetSLNString(string guid, ExampleLanguage language) {
			string result = StringContainer.SLNString;
			result = result.Replace("//GUIDCSproj", guid);
			result = result.Replace("//GUIDSLN", Guid.NewGuid().ToString().ToUpper());
			result = result.Replace("#csprojOrvbprog", language == ExampleLanguage.Csharp ? "csproj" : "vbproj");
			return result;
		}
		public static string GetReferenceString(CodeExample codeExample) {
			string result = string.Empty;
			System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(codeExample.Parent.RootType.TypeHandle);
			var assemblies = AppDomain.CurrentDomain.GetAssemblies();
			foreach(Assembly asm in assemblies) {
				if(!asm.IsDynamic && !string.IsNullOrEmpty(asm.Location) && asm.GlobalAssemblyCache)
					result += String.Format(@"<Reference Include=""{0}""/>{1}", asm.GetName().Name, Environment.NewLine);
			}
			return result;
		}
		static string GetPROJString(ExampleLanguage langauge) {
			string csprojString = String.Empty;
			using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(
				langauge == ExampleLanguage.Csharp ? "DevExpress.Tutorials.CodeDemoBase.TemplateCSProject.DxSample.csproj" : "DevExpress.Tutorials.CodeDemoBase.TemplateVBProject.DxSample.vbproj")) {
				resource.Position = 0;
				using(StreamReader reader = new StreamReader(resource, Encoding.UTF8)) {
					csprojString = reader.ReadToEnd();
				}
			}
			return csprojString;
		}
		static string GetFormDesignerString(ExampleLanguage langauge) {
			string formDesignerCode = String.Empty;
			using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(
				 langauge == ExampleLanguage.Csharp ? "DevExpress.Tutorials.CodeDemoBase.TemplateCSProject.Form1.Designer.cs" : "DevExpress.Tutorials.CodeDemoBase.TemplateVBProject.Form1.Designer.vb")) {
				resource.Position = 0;
				using(StreamReader reader = new StreamReader(resource, Encoding.UTF8)) {
					formDesignerCode = reader.ReadToEnd();
				}
			}
			return formDesignerCode;
		}
		static void WriteResourceToFile(string resourceName, string fileName) {
			using(var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)) {
				using(var file = new FileStream(fileName, FileMode.Create, FileAccess.Write)) {
					resource.CopyTo(file);
				}
			}
		}
		static string FillUsingString(SortedList sortedList, ExampleLanguage language) {
			string result = String.Empty;
			foreach(var usingString in sortedList.GetValueList()) {
				if(language == ExampleLanguage.Csharp) result += String.Format("using {0};{1}", usingString, Environment.NewLine);
				else result += String.Format("Imports {0}{1}", usingString, Environment.NewLine);
			}
			return result;
		}
		static readonly string Tabulation = new string(' ', 8);
		static string GetCodeFromLanguageElement(ExampleLanguage language, Method method, string[] linesOfCode, bool replaceToThis = false, bool isTearDown = false, List<string> parameters = null) {
			if(method == null)
				return string.Empty;
			StringBuilder builder = new StringBuilder();
			if(parameters == null) {
				var @this = (language == ExampleLanguage.Csharp) ? "this" : "Me";
				for(int i = method.StartLine; i < method.EndLine - (isTearDown ? 1 : 2); i++) {
					builder.Append(Tabulation);
					if(replaceToThis)
						builder.AppendLine(linesOfCode[i].Replace(method.Parameters[0].Name, @this));
					else
						builder.AppendLine(linesOfCode[i]);
				}
			}
			else {
				for(int i = method.StartLine; i < method.EndLine - 1; i++) {
					if(method.Parameters.Count != parameters.Count)
						throw new Exception(string.Format("Method: {0}. Wrong parameters count.", method.Name));
					builder.Append(Tabulation);
					string line = linesOfCode[i];
					for(int p = 0; p < method.Parameters.Count; p++)
						line = line.Replace(method.Parameters[p].Name, parameters[p]);
					builder.AppendLine(line);
				}
			}
			return builder.ToString();
		}
		public string GetCSUserControlCode(CodeExample example, ref string resultUserControlDesignerCode) {
			resultUserControlDesignerCode = StringContainer.UserControlDesignerCSCode;
			string resultUserControlCode = StringContainer.UserControlCSCode;
			string code = ExampleCodeEvaluator.CreateCode(example, CurrentExampleLanguage);
			if(code.Contains("using DevExpress.DXperience.Demos.CodeDemo; ")) {
				code = code.Replace("using DevExpress.DXperience.Demos.CodeDemo; ", "");
			}
			CSharp30Parser parserCS = new CSharp30Parser();
			GetUserControlCodeCore(example, ref resultUserControlDesignerCode, ref resultUserControlCode, code, parserCS);
			return resultUserControlCode;
		}
		public string GetVBUserControlCode(CodeExample example, ref string resultUserControlDesignerCode) {
			resultUserControlDesignerCode = StringContainer.UserControlDesignerVBCode;
			string resultUserControlCode = StringContainer.UserControlVBCode;
			string code = ExampleCodeEvaluator.CreateCode(example, CurrentExampleLanguage);
			if(code.Contains("Imports DevExpress.DXperience.Demos.CodeDemo")) {
				code = code.Replace("Imports DevExpress.DXperience.Demos.CodeDemo", "");
			}
			VB90Parser parserVB = new VB90Parser();
			GetUserControlCodeCore(example, ref resultUserControlDesignerCode, ref resultUserControlCode, code, parserVB);
			return resultUserControlCode;
		}
		void GetUserControlCodeCore(CodeExample example, ref string resultUserControlDesignerCode, ref string resultUserControlCode, string code, FormattingParserBase parser) {
			SourceFile sourceFileLanguageElement = parser.ParseString(code) as SourceFile;
			string[] linesOfCode = GetLinesFromCodeString(code);
			Method methodSetUp = (example.Parent.SetUp != null) ? (sourceFileLanguageElement.FindChildByName(example.Parent.SetUp.Name)) as Method : null;
			Method methodTearDown = (example.Parent.TearDown != null) ? (sourceFileLanguageElement.FindChildByName(example.Parent.TearDown.Name)) as Method : null;
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.UserControlName, example.UserControlName(false));
			resultUserControlDesignerCode = resultUserControlDesignerCode.Replace(StringContainer.UserControlName, example.UserControlName(false));
			resultUserControlDesignerCode = resultUserControlDesignerCode.Replace(StringContainer.NameSpace, "DxSample" + example.Parent.NameSpace());
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.Usings, FillUsingString(sourceFileLanguageElement.UsingList, CurrentExampleLanguage));
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.SetUp, GetCodeFromLanguageElement(CurrentExampleLanguage, methodSetUp, linesOfCode, true));   
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.TearDown, GetCodeFromLanguageElement(CurrentExampleLanguage, methodTearDown, linesOfCode, true, true));
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.RegionExampleCodeName, example.RegionName());
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.NameSpace, "DxSample" + example.Parent.NameSpace());
			ArrayCreateExpression array = (methodSetUp.LastNode as Return).Expression as ArrayCreateExpression;
			List<string> SetUpParameters = new List<string>();
			foreach(Expression expression in array.Initializer.Initializers) {
				SetUpParameters.Add(expression.Name);
			}
			Method method = (sourceFileLanguageElement.FindChildByName("Process")) as Method;
			resultUserControlCode = resultUserControlCode.Replace(StringContainer.ExampleMethod, GetCodeFromLanguageElement(CurrentExampleLanguage, method, linesOfCode, true, false, SetUpParameters));
			if(example.NestedTypes != null && example.NestedTypes.Length != 0) {
				StringBuilder nestedBuilder = new StringBuilder();
				foreach(string nestedStringClassCode in CodeTutorialControlBase.GetListStringFromNestedTypes(example, CurrentExampleLanguage)) {
					string[] strings = GetLinesFromCodeString(nestedStringClassCode);
					foreach(string line in strings) {
						nestedBuilder.Append(new string(' ', 8));
						nestedBuilder.AppendLine(line);
					}
				}
				resultUserControlCode = resultUserControlCode.Replace(StringContainer.NestedClasses, nestedBuilder.ToString());
			}
			else resultUserControlCode = resultUserControlCode.Replace(StringContainer.NestedClasses, "");
		}
		#endregion
	}
	class GroupNode {
		public GroupNode(string name, IList children) {
			Name = name;
			Children = children;
		}
		public string Name { get; set; }
		public IList Children { get; set; }
	}
}
