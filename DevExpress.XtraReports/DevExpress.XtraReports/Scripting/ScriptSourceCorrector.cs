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

using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using System.CodeDom.Compiler;
using DevExpress.XtraReports.Localization;
using System;
using System.Collections;
namespace DevExpress.XtraReports {
	public class ScriptSourceCorrector {
		#region static
		static readonly Regex importCS = new Regex(@"\A\s*using\s+(?<ns>(\w+\s*=\s*)?\w+(\.\w+)*)\s*;", RegexOptions.Singleline);
		static readonly Regex importVB = new Regex(@"\A\s*Imports\s+(?<ns>\w+(\.\w+)*)\s*", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		static readonly Regex importJS = new Regex(@"\A\s*import\s+(?<ns>\w+(\.\w+)*)((\.\w+)|(\.\*))\s*;", RegexOptions.Singleline);
		static readonly Regex commentSlashSlash = new Regex(@"\A\s*//", RegexOptions.Singleline);
		static readonly Regex commentApostroph = new Regex(@"\A\s*'", RegexOptions.Singleline);
		static readonly Regex commentRem = new Regex(@"\A\s*rem", RegexOptions.Singleline | RegexOptions.IgnoreCase);
		static bool IsCommentedLineCSharp(string line) {
			return commentSlashSlash.Match(line).Success;
		}
		static bool IsCommentedLineVisualBasic(string line) {
			return commentApostroph.Match(line).Success || commentRem.Match(line).Success;
		}
		static bool IsCommentedLineJScript(string line) {
			return IsCommentedLineCSharp(line);
		}
		static string GetImportLine(string line, Regex r) {
			Match m = r.Match(line);
			if(m.Success && m.Index == 0 && m.Groups["ns"].Success)
				return m.Groups["ns"].Value;
			return string.Empty;
		}
		#endregion
		ScriptLanguage language;
		string importSource = string.Empty;
		string lines = string.Empty;
		public ScriptSourceCorrector(ScriptLanguage language, string source) {
			this.language = language;
			StringBuilder sb = new StringBuilder();
			Initialize(source, sb);
			lines = sb.ToString();
		}
		public string Scripts { get { return lines; } }
		public string CorrectSource { get { return ImportSource + Scripts; } }
		protected string ImportSource { get { return importSource; } }
		protected ScriptLanguage Language { get { return language; } }
		protected int SourceLinesCount { get { return lines.Length; } }
		void Initialize(string source, StringBuilder lines) {
			if(string.IsNullOrEmpty(source))
				return;
			string[] sourceLines = GetSourceLines(source);
			StringBuilder importLines = new StringBuilder();
			InitializeImportUnits();
			foreach(string sourceLine in sourceLines) {
				string importLine = GetImportLine(sourceLine);
				if(!string.IsNullOrEmpty(importLine)) {
					CreateImportUnit(importLine);
					importLines.AppendLine(sourceLine.Trim());
				} else {
					lines.AppendLine();
					lines.Append(sourceLine);
				}
			}
			if(lines.Length >= 2)
				lines.Remove(0, 2);
			importSource = importLines.ToString();
		}
		protected virtual void InitializeImportUnits() {
		}
		protected virtual void CreateImportUnit(string importLine) {
		}
		protected virtual string[] GetSourceLines(string source) {
			return XRConvert.StringToStringArray(source);
		}
		string GetImportLine(string line) {
			line = line.Trim();
			if(IsCommentedLine(line))
				return string.Empty;
			switch(language) {
				case ScriptLanguage.CSharp:
					return GetImportLine(line, importCS);
				case ScriptLanguage.VisualBasic:
					return GetImportLine(line, importVB);
				case ScriptLanguage.JScript:
					return GetImportLine(line, importJS);
				default:
					return string.Empty;
			}
		}
		protected bool IsCommentedLine(string line) {
			switch(language) {
				case ScriptLanguage.CSharp:
					return IsCommentedLineCSharp(line);
				case ScriptLanguage.VisualBasic:
					return IsCommentedLineVisualBasic(line);
				case ScriptLanguage.JScript:
					return IsCommentedLineJScript(line);
				default:
					return false;
			}
		}
	}
	public class ScriptSourceCompilerCorrector : ScriptSourceCorrector {
		#region static
		const string importCSstring = "using ";
		const string importVBstring = "Imports ";
		const string importJSstring = "import ";
		static string GetImportString(ScriptLanguage language) {
			if(language == ScriptLanguage.CSharp)
				return importCSstring;
			else if(language == ScriptLanguage.VisualBasic)
				return importVBstring;
			else if(language == ScriptLanguage.JScript)
				return importJSstring;
			else return string.Empty;
		}
		#endregion
		List<string> imports;
		public ScriptSourceCompilerCorrector(ScriptLanguage language, string source)
			: base(language, source) {
		}
		public string[] Imports { get { return imports.ToArray(); } }
		protected override void InitializeImportUnits() {
			imports = new List<string>();
		}
		protected override void CreateImportUnit(string importLine) {
			imports.Add(importLine);
		}
		protected override string[] GetSourceLines(string source) {
			string[] lines = XRConvert.StringToStringArray(source);
			if(Language == ScriptLanguage.VisualBasic) {
				int count = lines.Length;
				for(int i = 0; i < count; i++) {
					string line = lines[i].Trim();
					int len = line.Length;
					if(!IsCommentedLine(line) && len > 0 && line[len - 1] == '_') {
						if(i + 1 < count) {
							lines[i] = string.Empty;
							lines[i + 1] = line.Substring(0, len - 1) + lines[i + 1];
						}
						else
							lines[i] = line.Substring(0, len - 1);
					}
				}
			}
			return lines;
		}
		public CompilerErrorCollection GetScriptErrors(CompilerErrorCollection sourceErrors, string source) {
			CompilerErrorCollection errors = new CompilerErrorCollection();
			if(sourceErrors.Count == 0)
				return errors;
			int index = source.IndexOf(Scripts);
			string[] headerLines = XRConvert.StringToStringArray(source.Substring(0, index));
			string[] headerLines2 = XRConvert.StringToStringArray(ImportSource);
			int errorLineCorrection = Math.Max(0, headerLines.Length - headerLines2.Length);
			int importErrorColumnCorrection = 0;
			if(headerLines2.Length > 0 && headerLines2[0].Length > 0) {
				string importString = GetImportString(Language);
				int importLineIndex = Array.FindLastIndex<string>(headerLines, line => { return line.Trim().StartsWith(importString, StringComparison.InvariantCultureIgnoreCase); });
				if(importLineIndex >= 0)
					importErrorColumnCorrection = Math.Max(0, headerLines[importLineIndex].IndexOf(importString))
						- Math.Max(0, headerLines2[0].IndexOf(importString));
			}
			foreach(CompilerError error in sourceErrors) {
				int errorLine = error.Line;
				int errorColumn = error.Column;
				if(Language == ScriptLanguage.VisualBasic)
					errorColumn++;
				if(errorLine < headerLines.Length) {
					string import = errorLine - 1 >= 0 && errorLine - 1 < headerLines.Length  ? headerLines[errorLine - 1].Trim() : string.Empty;
					errorLine = !string.IsNullOrEmpty(import) ? Math.Max(1, Array.IndexOf<string>(headerLines2, import) + 1) : 1;
					System.Diagnostics.Debug.Assert(errorColumn >= importErrorColumnCorrection);
					errorColumn = Math.Max(1, errorColumn - importErrorColumnCorrection);
				} else {
					System.Diagnostics.Debug.Assert(errorLine >= errorLineCorrection);
					errorLine = Math.Max(1, errorLine - errorLineCorrection);
					errorColumn = Math.Max(1, errorColumn);
				}
				if(errorLine > SourceLinesCount - 1) {
					errors.Add(new CompilerError(error.FileName, 1, 1, error.ErrorNumber, ReportLocalizer.GetString(ReportStringId.Msg_ScriptCodeIsNotCorrect)));
					return errors;
				}
				errors.Add(new CompilerError(error.FileName, errorLine, errorColumn, error.ErrorNumber, error.ErrorText));
			}
			return errors;
		}
	}
}
