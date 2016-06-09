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
using System.Windows;
namespace DevExpress.DemoData.Helpers {
	public class BookmarkHelper {
		Dictionary<string, string> redirects = new Dictionary<string, string>();
		StartupBase startup;
		string nullState;
		bool redirectInProgress;
		string currentState;
		static BookmarkHelper() {
			UrlHelper = new EscapeHelper();
			UrlHelper.AddPair(" ", "_");
			UrlHelper.AddPair("_", "\\_");
			UrlHelper.AddPair("\\", "\\\\");
		}
		public static EscapeHelper UrlHelper { get; private set; }
		public BookmarkHelper(StartupBase startup, string nullState) {
			this.startup = startup;
			this.nullState = nullState;
		}
		public string CurrentState {
			get {
				if(this.currentState == null)
					this.currentState = BeginProcessBookmarks();
				return this.currentState;
			}
			set {
				if(this.currentState == value) return;
				this.currentState = value;
				this.redirectInProgress = true;
				if(this.currentState == this.nullState)
					this.startup.SaveNavigationHistory = false;
				this.startup.Bookmark = StateToBookmark(this.currentState);
				this.startup.SaveNavigationHistory = true;
				this.redirectInProgress = false;
				if(CurrentStateChanged != null)
					CurrentStateChanged(this, EventArgs.Empty);
			}
		}
		public event EventHandler CurrentStateChanged;
		public void SetRedirect(string source, string target) {
			source = UrlHelper.Screen(source);
			target = target == null ? null : UrlHelper.Screen(target);
			if(target == null) {
				if(this.redirects.ContainsKey(source))
					this.redirects.Remove(source);
			} else {
				if(this.redirects.ContainsKey(source))
					this.redirects[source] = target;
				else
					this.redirects.Add(source, target);
			}
		}
		string BeginProcessBookmarks() {
			this.startup.BookmarkChanged += OnStartupBookmarkChanged;
			return BookmarkToState(GetBookmark());
		}
		void OnStartupBookmarkChanged(object sender, DependencyPropertyChangedEventArgs e) {
			if(this.redirectInProgress) return;
			CurrentState = BookmarkToState(GetBookmark());
		}
		string GetBookmark() {
			string draftBookmark = this.startup.Bookmark;
			string cleanBookmark = ProcessBookmark(draftBookmark);
			this.redirectInProgress = true;
			this.startup.SaveNavigationHistory = false;
			this.startup.Bookmark = cleanBookmark;
			this.startup.SaveNavigationHistory = true;
			this.redirectInProgress = false;
			return cleanBookmark;
		}
		string ProcessBookmark(string b) {
			b = b.Trim('/', ' ');
			string x;
			return this.redirects.TryGetValue(b, out x) ? x : b;
		}
		string StateToBookmark(string state) {
			return UrlHelper.Screen(state);
		}
		string BookmarkToState(string bookmark) {
			return UrlHelper.Unscreen(bookmark);
		}
	}
}
