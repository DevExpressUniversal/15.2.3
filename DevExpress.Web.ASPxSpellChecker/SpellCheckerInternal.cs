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
using DevExpress.XtraSpellChecker;
using DevExpress.Web.Internal;
using DevExpress.XtraSpellChecker.Strategies;
using DevExpress.XtraSpellChecker.Parser;
namespace DevExpress.ASPxSpellChecker.Native {
	public static class SpellCheckerCallbackResultProperties {
		public const string
			DialogContent = "dialogContent",
			ErrorCount = "errorCount",
			Settings = "settings",
			StartErrorWordPositionArray = "startErrorWordPositionArray",
			WrongWordLengthArray = "wrongWordLengthArray",
			SuggestionsArray = "suggestionsArray",
			CheckComplete = "checkComplete";
	}
	public class SpellCheckerCallbackReader {
		Dictionary<string, string> callbackArgs = new Dictionary<string, string>();
		public virtual string DialogFormName {
			get {
				return this.callbackArgs.ContainsKey(RenderUtils.DialogFormCallbackStatus) ?
				this.callbackArgs[RenderUtils.DialogFormCallbackStatus] : "";
			}
		}
		public virtual string TextToCheck {
			get {
				return this.callbackArgs.ContainsKey(DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker.SpellCheckerCheckTextPrefix) ?
				this.callbackArgs[DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker.SpellCheckerCheckTextPrefix] : "";
			}
		}
		public virtual string WordToAdd {
			get {
				return this.callbackArgs.ContainsKey(DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker.SpellCheckerAddWordPrefix) ?
				this.callbackArgs[DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker.SpellCheckerAddWordPrefix] : "";
			}
		}
		public virtual int StartIndex {
			get {
				string key = DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker.SpellCheckerStartIndexPrefix;
				if (this.callbackArgs.ContainsKey(key)) {
					string value = this.callbackArgs[key];
					try {
						return Convert.ToInt32(value);
					}
					catch {
					}
				}
				return 0;
			}
		}
		public virtual bool IsOptionsFormCallBack {
			get { return string.Compare(DialogFormName, DevExpress.Web.ASPxSpellChecker.ASPxSpellChecker.SpellCheckOptionsFormName) == 0; }
		}
		public SpellCheckerCallbackReader(string arguments) {
			this.callbackArgs = new Dictionary<string, string>();
			ReadCallbacksArgs(arguments);
		}
		private void ReadCallbacksArgs(string arguments) {
			int colonIndex = arguments.IndexOf(':');
			while(colonIndex != -1) {
				string argument = arguments.Substring(0, colonIndex);
				if(argument.Contains("("))
					arguments = ExtractTextArgument(arguments);
				else {
					int endColonIndex = arguments.IndexOf(':', colonIndex + 1);
					if(endColonIndex == -1)
						endColonIndex = arguments.Length;
					callbackArgs[argument] = arguments.Substring(colonIndex + 1, endColonIndex - colonIndex - 1);
					if(endColonIndex != arguments.Length)
						arguments = arguments.Substring(endColonIndex + 1);
					else
						break;
				}
				arguments = arguments.Trim();
				colonIndex = arguments.IndexOf(':');
			}
		}
		private string ExtractTextArgument(string arguments) {
			int leftBracketIndex = arguments.IndexOf('(');
			int rightBracketIndex = arguments.IndexOf(')');
			int textLength = Convert.ToInt32(arguments.Substring(leftBracketIndex + 1, rightBracketIndex - leftBracketIndex - 1));
			this.callbackArgs[arguments.Substring(0, leftBracketIndex)] =
				arguments.Substring(rightBracketIndex + 2, textLength); 
			if(arguments.Length >= textLength + rightBracketIndex + 3)
				return arguments.Remove(0, textLength + rightBracketIndex + 3); 
			return string.Empty;
		}
	}
	public class WebSpellChecker : SpellCheckerBase {
		public new int SuggestionCount { get { return base.SuggestionCount; } set { base.SuggestionCount = value; } }
		public void Check(string text, int startIndex) {
			if (CanCheck()) {
				DoBeforeCheck();
				try {
					SearchStrategy = new SimpleTextSearchStrategy(this, new SimpleTextController(), new IntPosition(startIndex), new IntPosition(text.Length));
					SearchStrategy.Text = text;
					SearchStrategy.Check();
				}
				finally {
					DoAfterCheck(SearchStrategy);
				}
			}
		}
	}
}
