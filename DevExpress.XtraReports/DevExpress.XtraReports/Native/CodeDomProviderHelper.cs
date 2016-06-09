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
using System.Text;
using System.CodeDom.Compiler;
namespace DevExpress.XtraReports.Native
{
	public static class CodeDomProviderHelper
	{
		public static System.CodeDom.Compiler.CodeDomProvider CreateDomProvider(string extension) {
			return CreateDomProvider(GetScriptLanguageFromExtension(extension));
		}
		static ScriptLanguage GetScriptLanguageFromExtension(string extension) {
			switch(extension) {
				case ".cs": return ScriptLanguage.CSharp;
				case ".vb": return ScriptLanguage.VisualBasic;
				case ".jsl": return ScriptLanguage.JScript;
				default:
					throw new Exception("Unsupported extension");
			}
		}
		public static System.CodeDom.Compiler.CodeDomProvider CreateDomProvider(ScriptLanguage language) {
			switch (language) {
				case ScriptLanguage.CSharp:
					return new Microsoft.CSharp.CSharpCodeProvider(GetProviderOptions());
				case ScriptLanguage.VisualBasic:
					return new Microsoft.VisualBasic.VBCodeProvider(GetProviderOptions());
				case ScriptLanguage.JScript:
					return (CodeDomProvider)Activator.CreateInstance("Microsoft.JScript, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", "Microsoft.JScript.JScriptCodeProvider").Unwrap();
				default:
					throw new Exception("Unsupported language"); 
			}
		}
		static IDictionary<string, string> GetProviderOptions() {
			Dictionary<string, string> options = new Dictionary<string, string>();
			if(EnvironmenHelper.FrameworkLess(4))
				options.Add("CompilerVersion", "v3.5");
			return options;
		}
	}
	public static class EnvironmenHelper {
		public static bool FrameworkLess(int major) {
			return DevExpress.Data.Utils.Helpers.GetFrameworkVersion() < new Version(major, 0);
		}
		public static bool FrameworkGreaterOrEqual(int major) {
			return DevExpress.Data.Utils.Helpers.GetFrameworkVersion() >= new Version(major, 0);
		}
	}
}
