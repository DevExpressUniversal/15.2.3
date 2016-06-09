#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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

using DevExpress.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.ExpressApp.Web.Utils {
	public class ClientSideEventsHelper {
		public static string ToJSBoolean(bool value) {
			return value ? "true" : "false";
		}
		public static void AssignClientHandlerSafe(ASPxWebControl editor, string eventName, string script, string key) {
			string oldScript = editor.GetClientSideEventHandler(eventName);
			string newScript = ConcatenateScripts(oldScript, script, key);
			editor.SetClientSideEventHandler(eventName, newScript);
		}
		private static string ConcatenateScripts(string oldScript, string newScript, string key) {
			string result = newScript;
			string scriptkey = "/*" + key + "*/";
			if(!string.IsNullOrEmpty(oldScript)) {
				if(!oldScript.Contains(scriptkey)) {
					string testScript = scriptkey + newScript + scriptkey;
					result = string.Format("function(s,e) {{ ({0})(s,e); ({1})(s,e);}}", oldScript, testScript);
				}
				else {
					if(!oldScript.Contains(newScript)) {
						result = ReplaceScriptByKey(oldScript, newScript, scriptkey);
					}
					else {
						return oldScript;
					}
				}
			}
			else {
				result = scriptkey + result + scriptkey;
			}
			return result;
		}
		private static string ReplaceScriptByKey(string oldScript, string newScript, string key) {
			int startIndex = oldScript.IndexOf(key);
			int endIndex = oldScript.LastIndexOf(key);
			string prefix = oldScript.Substring(0, startIndex);
			string postfix = oldScript.Substring(endIndex);
			return prefix + key + newScript + postfix;
		}
	}
}
