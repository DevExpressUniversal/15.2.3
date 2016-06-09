#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Linq;
using System.Text;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Web.Native {
	public class ActionBuilder {
		class JsAction {
			public string Name { get; set; }
			public object[] Args { get; set; }
			public bool IsAssignment { get; set; }
		}
		readonly List<JsAction> actions = new List<JsAction>();
		public void AddAction(string name, params object[] args) {
			AddActionCore(name, args);
		}
		public void AddAssignment(string name, params object[] args) {
			var action = AddActionCore(name, args);
			action.IsAssignment = true;
		}
		public string RenderToScript(string clientInstanceName) {
			var builder = new StringBuilder();
			foreach(JsAction action in actions) {
				builder.Append(clientInstanceName + "." + action.Name);
				if(action.IsAssignment)
					AppendAssignment(builder, action);
				else
					AppendFunctionArgumens(builder, action);
			}
			return builder.ToString();
		}
		JsAction AddActionCore(string name, params object[] args) {
			var action = new JsAction {
				Name = name,
				Args = args
			};
			actions.Add(action);
			return action;
		}
		static void AppendAssignment(StringBuilder builder, JsAction action) {
			builder.Append('=');
			var isArray = action.Args.Length > 1;
			if(isArray)
				builder.Append('[');
			AppendArgs(builder, action);
			if(isArray) {
				builder.Append(']');
			}
			builder.AppendLine(";");
		}
		static void AppendFunctionArgumens(StringBuilder builder, JsAction action) {
			builder.Append('(');
			AppendArgs(builder, action);
			builder.AppendLine(");");
		}
		static void AppendArgs(StringBuilder builder, JsAction action) {
			var stringArgs = action.Args.Select(ConvertToString);
			var joinedArgs = string.Join(",", stringArgs);
			builder.Append(joinedArgs);
		}
		static string ConvertToString(object arg) {
			if(arg is string)
				return "'" + arg + "'";
			if(arg is bool)
				return arg.ToString().ToLower();
			return arg.ToString();
		}
	}
}
