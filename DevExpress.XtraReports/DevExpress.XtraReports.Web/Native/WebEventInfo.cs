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

using System.Collections.Generic;
namespace DevExpress.XtraReports.Web {
	class WebEventInfo {
		public static WebEventInfo Create(string eventArgument) {
			string[] items = eventArgument.Split(new[] { '=' }, 2);
			if(items.Length != 2)
				return null;
			return new WebEventInfo(items[0], items[1]);
		}
		readonly Dictionary<string, string> parameters;
		public string EventId { get; private set; }
		public string EventArgument { get; private set; }
		public string this[string key] {
			get {
				string result;
				parameters.TryGetValue(key, out result);
				return result;
			}
		}
		public string Txt {
			get { return this["txt"]; }
		}
		public string Up {
			get { return this["up"]; }
		}
		public string Word {
			get { return this["word"]; }
		}
		public string Case {
			get { return this["case"]; }
		}
		public string Path {
			get { return this["path"]; }
		}
		public string PageIndexString {
			get { return this["idx"]; }
		}
		public string Format {
			get { return this["format"]; }
		}
		public string ShowPrintDialog {
			get { return this["showPrintDialog"]; }
		}
		public bool IsPrintEvent {
			get { return EventId == "print"; }
		}
		public bool IsSaveToWindowEvent {
			get { return EventId == "saveToWindow"; }
		}
		internal WebEventInfo(string eventId, string eventArgument) {
			parameters = ParseToDictionary(eventArgument);
			EventId = eventId;
			EventArgument = eventArgument;
		}
		static Dictionary<string, string> ParseToDictionary(string value) {
			const char OuterSplitter = ';';
			const char InnerSplitter = ':';
			var result = new Dictionary<string, string>();
			string[] items = value.Split(OuterSplitter);
			var innerSplitters = new[] { InnerSplitter };
			foreach(string item in items) {
				string[] subItems = item.Split(innerSplitters, 2);
				if(subItems.Length == 2) {
					string val = subItems[1]
						.Replace("&c", ":")
						.Replace("&s", ";")
						.Replace("&a", "&");
					result[subItems[0]] = val;
				}
			}
			return result;
		}
	}
}
