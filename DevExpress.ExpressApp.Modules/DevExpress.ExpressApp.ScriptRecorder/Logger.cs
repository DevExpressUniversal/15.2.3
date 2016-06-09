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

using System;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Persistent.Base;
namespace DevExpress.ExpressApp.ScriptRecorder {
	public static class Logger {
		public const string SelectRecords = "*SelectRecords";
		public static ScriptLogger Instance {
			get {
				IValueManager<ScriptLogger> manager = ValueManager.GetValueManager<ScriptLogger>("Logger_ScriptLogger");
				if(manager.Value == null) {
					manager.Value = new ScriptLogger();
				}
				return manager.Value;
			}
		}
		public static void SetLogger(ScriptLogger logger) {
			ValueManager.GetValueManager<ScriptLogger>("Logger_ScriptLogger").Value = logger;
		}
	}
	public class ScriptLogger {
		private Script script = new Script();
		private bool setHeader = false;
		protected virtual AddMessageEventArgs OnAddMessage(string message) {
			AddMessageEventArgs arg = new AddMessageEventArgs(message);
			if(AddMessage != null) {
				AddMessage(this, arg);
			}
			return arg;
		}
		public virtual void WriteMessage(string message) {
			AddMessageEventArgs arg = OnAddMessage(message);
			if(!arg.Cancel) {
				script.ScriptLog += Environment.NewLine + arg.Message;
			}
		}
		public Script Script {
			get { return script; }
		}
		public void SetHeader(string applicationName, string dbName) {
			if(!setHeader) {
				setHeader = true;
				if(!string.IsNullOrEmpty(dbName)) {
					WriteMessage(";#DropDB " + dbName);
				}
				WriteMessage("#Application " + applicationName + Environment.NewLine);
			}
		}
		public event EventHandler<AddMessageEventArgs> AddMessage;
	}
	public class AddMessageEventArgs : CancelEventArgs {
		string _message = null;
		public AddMessageEventArgs(string message) {
			_message = message;
		}
		public string Message {
			get { return _message; }
			set { _message = value; }
		}
	}
	[DomainComponent]
	public class Script {
		private string scriptLog;
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public string ScriptLog {
			get {
				UniteMessage();
				return scriptLog;
			}
			set { scriptLog = value; }
		}
		private void UniteMessage() {
			scriptLog = UniteFillRormMessages();
			scriptLog = UniteSelectRecordMessages();
		}
		private string UniteFillRormMessages() {
			return UniteMessage(PropertyEditorListener.FillForm + Environment.NewLine, scriptLog);
		}
		private string UniteSelectRecordMessages() {
			return UniteSelectRecordMessage(Logger.SelectRecords + Environment.NewLine, ScriptRecorderActionsListenerBase.ProcessRecordPrefix + Environment.NewLine, scriptLog);
		}
		private string UniteSelectRecordMessage(string pattern_1, string pattern_2, string input) {
			if(string.IsNullOrEmpty(input)) {
				return input;
			}
			string result = null;
			string[] inputToArray = input.Split('*');
			for(int counter = 1; counter < inputToArray.Length; counter++) {
				inputToArray[counter] = "*" + inputToArray[counter];
			}
			for(int counter = 0; counter < inputToArray.Length; counter++) {
				if(inputToArray[counter].StartsWith(pattern_1) && counter > 0) {
					if(counter + 1 < inputToArray.Length) {
						if(inputToArray[counter + 1].StartsWith(pattern_2)) {
							continue;
						}
						else {
							result += inputToArray[counter];
						}
					}
					else {
						result += inputToArray[counter];
					}
				}
				else {
					result += inputToArray[counter];
				}
			}
			return result;
		}
		private string UniteMessage(string pattern, string input) {
			if(string.IsNullOrEmpty(input)) {
				return input;
			}
			string result = null;
			string[] inputToArray = input.Split('*');
			for(int counter = 1; counter < inputToArray.Length; counter++) {
				inputToArray[counter] = "*" + inputToArray[counter];
			}
			for(int counter = 0; counter < inputToArray.Length; counter++) {
				if(inputToArray[counter].StartsWith(pattern) && counter > 0) {
					if(inputToArray[counter - 1].StartsWith(pattern)) {
						result += inputToArray[counter].Replace(pattern, "");
					} else {
						result += inputToArray[counter];
					}
				} else {
					result += inputToArray[counter];
				}
			}
			return result;
		}
		private bool IsUniteString(string input) {
			return input.IndexOf(Environment.NewLine + "*") == -1;
		}
	}
}
