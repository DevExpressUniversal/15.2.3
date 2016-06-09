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
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Threading;
namespace DevExpress.Xpf.Editors {
	public class TextSearchEngine {
		public bool IsActive { get; private set; }
		string prefix;
		public string Prefix {
			get { return prefix; }
			private set {
				if (!string.IsNullOrEmpty(value))
					SeachText = value;
				prefix = value;
			}
		}
		public string SeachText{ get; private set; }
		public int MatchedItemIndex { get; private set; }
		List<string> charsEntered;
		DispatcherTimer timeoutTimer;
		TimeSpan TimeOut { get { return TimeSpan.FromMilliseconds(SystemInformation.DoubleClickTime * 2); } }
		readonly Func<string, int, bool, int> searchCallback;
		public TextSearchEngine(Func<string, int, bool, int> searchCallback) {
			this.searchCallback = searchCallback;
			ResetState();
		}
		void ResetState() {
			IsActive = false;
			Prefix = string.Empty;
			MatchedItemIndex = -1;
			if (charsEntered == null)
				charsEntered = new List<string>(10);
			else
				charsEntered.Clear();
			if (timeoutTimer != null)
				timeoutTimer.Stop();
			timeoutTimer = null;
		}
		internal bool DoSearch(string nextChar, int currentItemIndex) {
			bool repeatedChar = false;
			int startItemIndex = -1;
			if (IsActive)
				startItemIndex = currentItemIndex;
			if (charsEntered.Count > 0 && (string.Compare(charsEntered[charsEntered.Count - 1], nextChar, true, CultureInfo.CurrentCulture) == 0))
				repeatedChar = true;
			bool wasNewCharUsed = false;
			int matchedItemIndex = FindMatchingPrefix(Prefix, nextChar, startItemIndex, repeatedChar, ref wasNewCharUsed);
			if (matchedItemIndex != -1) {
				if (!IsActive || matchedItemIndex != startItemIndex)
					MatchedItemIndex = matchedItemIndex;
				if (wasNewCharUsed) 
					AddCharToPrefix(nextChar);
				if (!IsActive) {
					IsActive = true;
					SeachText = Prefix;
				}
			}
			else if (IsBackSpace(nextChar) && Prefix.Length > 0)
				Prefix = Prefix.Substring(0, Prefix.Length - 1);
			if (IsActive) 
				ResetTimeout();
			return (matchedItemIndex != -1);
		}
		bool IsBackSpace(string nextChar) {
			return nextChar == "\b";
		}
		int FindMatchingPrefix(string prefix, string newChar, int startItemIndex, bool lookForFallbackMatchToo, ref bool wasNewCharUsed) {
			int matchedItemIndex = -1;
			string newPrefix = prefix;
			if (IsBackSpace(newChar) && newPrefix.Length > 0)
				newPrefix = newPrefix.Substring(0, newPrefix.Length - 1);
			else
				newPrefix = prefix + newChar;
			if (string.IsNullOrEmpty(newPrefix))
				return -1;
			wasNewCharUsed = false;
			int itemIndex = searchCallback(newPrefix, startItemIndex, false);
			if (itemIndex > -1) {
				wasNewCharUsed = true;
				matchedItemIndex = itemIndex;
			}
			else {
				if (lookForFallbackMatchToo)
					itemIndex = searchCallback(prefix, startItemIndex, true);
				if (itemIndex > -1)
					matchedItemIndex = itemIndex;
				else {
					itemIndex = searchCallback(prefix, -1, false);
					if (itemIndex > -1)
						matchedItemIndex = itemIndex;
				}
			}
			return matchedItemIndex;
		}
		void AddCharToPrefix(string newChar) {
			if (IsBackSpace(newChar) && Prefix.Length > 0)
				Prefix = Prefix.Substring(0, Prefix.Length - 1);
			else {
				Prefix += newChar;
				charsEntered.Add(newChar);
			}
		}
		void ResetTimeout() {
			if (timeoutTimer == null) {
				timeoutTimer = new DispatcherTimer(DispatcherPriority.Normal);
				timeoutTimer.Tick += OnTimeout;
			}
			else
				timeoutTimer.Stop();
			timeoutTimer.Interval = TimeOut;
			timeoutTimer.Start();
		}
		void OnTimeout(object sender, EventArgs e) {
			ResetState();
		}
		internal bool DeleteLastCharacter() {
			if (IsActive) {
				if (charsEntered.Count > 0) {
					string lastChar = this.charsEntered[this.charsEntered.Count - 1];
					string prefix = Prefix;
					charsEntered.RemoveAt(this.charsEntered.Count - 1);
					Prefix = prefix.Substring(0, prefix.Length - lastChar.Length);
					SeachText = Prefix;
					ResetTimeout();
					return true;
				}
			}
			return false;
		}
	}
}
