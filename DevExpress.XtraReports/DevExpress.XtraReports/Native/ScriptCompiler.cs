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
using System.Text;
using System.Collections.Specialized;
using System.CodeDom.Compiler;
using System.IO;
using System.Security.Policy;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.XtraReports.Native {
	class ScriptCompiler : Compiler {
		StringCollection userReferences;
		string methodStartMarker;
		ScriptLanguage language;
		internal static ScriptsDebugSwitch DebugSwitch = new ScriptsDebugSwitch("AllowDebugXtraReportScripts", "", "false");
		public ScriptCompiler(string source, StringCollection userReferences, ScriptLanguage language, string methodStartMarker, IApplicationPathService applicationPathService)
			: base(source, applicationPathService) {
			this.userReferences = userReferences;
			this.methodStartMarker = methodStartMarker;
			this.language = language;
		}
		protected internal override string[] GetReferencesFromSource() {
			return (string[])new System.Collections.ArrayList(userReferences).ToArray(typeof(string));
		}
		protected override CompilerParameters CreateCompilerParams(string[] references, Evidence evidence) {
			CompilerParameters compilerParams = base.CreateCompilerParams(references, evidence);
			compilerParams.GenerateExecutable = false;
			compilerParams.IncludeDebugInformation = true;
			if(DebugSwitch.Enabled) {
				compilerParams.OutputAssembly = Path.GetTempFileName() + ".dll";
				compilerParams.GenerateInMemory = false;
			}
			return compilerParams;
		}
		protected override CompilerResults CompileAssemblyFromSourceCore(CompilerParameters parameters, string source) {
			if(DebugSwitch.Enabled) {
				string sourcePath = Path.GetTempFileName() + GetSourceFileExtension();
				File.WriteAllText(sourcePath, source);
				return GetCodeProvider().CompileAssemblyFromFile(parameters, sourcePath);
			}
			return base.CompileAssemblyFromSourceCore(parameters, source);
		}
		protected override int ComputeSourceHash(string source) {
			string[] sourceLines = System.Text.RegularExpressions.Regex.Split(source, "\r\n");
			StringBuilder builder = new StringBuilder();
			foreach(string sourceLine in sourceLines) {
				if(!sourceLine.Contains(methodStartMarker))
					builder.AppendLine(sourceLine);
			}
			return builder.ToString().GetHashCode();
		}
		protected override CodeDomProvider GetCodeProvider() {
			return CodeDomProviderHelper.CreateDomProvider(language);
		}
		string GetSourceFileExtension() {
			switch(language) {
				case ScriptLanguage.CSharp: return ".cs";
				case ScriptLanguage.VisualBasic: return ".vb";
				case ScriptLanguage.JScript: return ".jsl";
			}
			throw new ArgumentException();
		}
	}
	public enum ScriptsDebugLevel {
		False = 0,
		True = 1,
		BreakWhenDebuggerAttached = 2
	}
	[System.Diagnostics.SwitchLevel(typeof(ScriptsDebugLevel))]
	public class ScriptsDebugSwitch : System.Diagnostics.Switch {
		public bool Enabled {
			get { return (base.SwitchSetting != 0); }
		}
		public ScriptsDebugLevel State {
			get { return ((ScriptsDebugLevel)base.SwitchSetting); }
			set { base.SwitchSetting = (int)value; }
		}
		public ScriptsDebugSwitch(string displayName, string description)
			: base(displayName, description) {
		}
		public ScriptsDebugSwitch(string displayName, string description, string defaultSwitchValue)
			: base(displayName, description, defaultSwitchValue) {
		}
		protected override void OnValueChanged() {
			ScriptsDebugLevel value;
			if(ScriptsDebugLevel.TryParse(base.Value, true, out value)) {
				base.SwitchSetting = (int)value;
			} else {
				base.OnValueChanged();
			}
		}
	}
}
