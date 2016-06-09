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

using System.Collections.Generic;
using DevExpress.CodeParser;
using DevExpress.CodeParser.VB;
using DevExpress.CodeParser.JavaScript;
using DevExpress.CodeParser.CSharp;
namespace DevExpress.XtraReports.Design {
	class MethodsHelper {
		IScriptSource scriptSource;
		public MethodsHelper(IScriptSource scriptSource) {
			this.scriptSource = scriptSource;
		}
		public int GetMethodTopLine(string code, string methodName) {
			Method method = GetMethodByName(code, methodName);
			return method != null ? method.Range.Top.Line : 0;
		}
		public bool ContainsMethod(string code, string methodName) {
			return GetMethodByName(code, methodName) != null;
		}
		public List<string> GetCompatibleMethodsNames(string code, string methodCode) {
			List<string> result = new List<string>();
			MethodEntry etalonMethod = GetMethods(methodCode)[0];
			List<MethodEntry> methods = GetMethods(code);
			if(methods != null)
				foreach(MethodEntry method in methods)
					if(method.EqualSignature(etalonMethod))
						result.Add(method.Name);
			return result;
		}
		Method GetMethodByName(string code, string methodName) {
			foreach(MethodEntry method in GetMethods(code))
				if(method.Name == methodName)
					return method.Method;
			return null;
		}
		List<MethodEntry> GetMethods(string code) {
			List<MethodEntry> result = new List<MethodEntry>();
			NodeList methods = GetNodeList(code);
			if(methods != null)
				foreach(object element in methods) {
					Method method = element as Method;
					if(method != null)
						result.Add(new MethodEntry(method));
				}
			return result;
		}
		NodeList GetNodeList(string code) {
			LanguageElement root = GetParser().ParseString(code);
			if(root == null)
				return null;
			return root.Nodes;
		}
		ParserBase GetParser() {
			ParserLanguageID parserLanguageID =
				scriptSource.ScriptLanguage == ScriptLanguage.VisualBasic ? ParserLanguageID.Basic :
				scriptSource.ScriptLanguage == ScriptLanguage.JScript ? ParserLanguageID.JavaScript : ParserLanguageID.CSharp;
			return ParserFactory.CreateParser(parserLanguageID, GetParserVersion());
		}
		public static ParserVersion GetParserVersion() {
			return DevExpress.XtraReports.Native.EnvironmenHelper.FrameworkLess(4) ? ParserVersion.VS2008 : ParserVersion.VS2010;
		}
	}
}
