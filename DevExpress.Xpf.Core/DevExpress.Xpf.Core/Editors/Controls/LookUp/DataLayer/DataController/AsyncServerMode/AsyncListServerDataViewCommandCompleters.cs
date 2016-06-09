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

using DevExpress.Data.Async;
namespace DevExpress.Xpf.Editors.Helpers {
	public interface IAsyncListServerDataViewCommandCompletedContainer {
		void Completed(object args);
	}
	public class GetRowCompleter : IAsyncListServerDataViewCommandCompletedContainer {
		public IAsyncListServerDataView View { get; private set; }
		public object Row { get; private set; }
		public int ControllerIndex { get; private set; }
		public GetRowCompleter(IAsyncListServerDataView view) {
			View = view;
		}
		public void Completed(object args) {
			CommandGetRow command = (CommandGetRow)(args);
			if (!CompletedInternal(command))
				return;
			ProcessCommand(command);
		}
		protected virtual void ProcessCommand(CommandGetRow command) {
			View.NotifyLoaded(ControllerIndex);
		}
		protected virtual bool CompletedInternal(CommandGetRow command) {
			if (command.Index < 0)
				return false;
			Row = command.Row;
			ControllerIndex = command.Index;
			return true;
		}
	}
	public class GetRowOnFindIncrementalCompleter : GetRowCompleter {
		public string Text { get; private set; }
		public int StartIndex { get; private set; }
		public bool SearchNext { get; private set; }
		public bool IgnoreStartIndex { get; private set; }
		public GetRowOnFindIncrementalCompleter(IAsyncListServerDataView view, string text, int startIndex, bool searchNext, bool ignoreStartIndex) : base(view) {
			Text = text;
			StartIndex = startIndex;
			SearchNext = searchNext;
			IgnoreStartIndex = ignoreStartIndex;
		}
		protected override void ProcessCommand(CommandGetRow command) {
			base.ProcessCommand(command);
			View.NotifyFindIncrementalCompleted(Text, StartIndex, SearchNext, IgnoreStartIndex, ControllerIndex);
		}
	}
	public class FindRowIndexByValueOnFindIncrementalCompleter : IAsyncListServerDataViewCommandCompletedContainer {
		public IAsyncListServerDataView View { get; private set; }
		public string Text { get; private set; }
		public int StartIndex { get; private set; }
		public bool SearchNext { get; private set; }
		public bool IgnoreStartIndex { get; private set; }
		public FindRowIndexByValueOnFindIncrementalCompleter(IAsyncListServerDataView view, string text, int startIndex, bool searchNext, bool ignoreStartIndex) {
			View = view;
			Text = text;
			StartIndex = startIndex;
			SearchNext = searchNext;
			IgnoreStartIndex = ignoreStartIndex;
		}
		public void Completed(object args) {
			CommandLocateByValue command = (CommandLocateByValue)args;
			if (command.RowIndex < 0)
				return;
			View.NotifyFindIncrementalCompleted(Text, StartIndex, SearchNext, IgnoreStartIndex, command.RowIndex);
		}
	}
	public class FindRowByTextCompleter : IAsyncListServerDataViewCommandCompletedContainer {
		public IAsyncListServerDataView View { get; private set; }
		public string Text { get; private set; }
		public int StartIndex { get; private set; }
		public bool SearchNext { get; private set; }
		public bool IgnoreStartIndex { get; private set; }
		public FindRowByTextCompleter(IAsyncListServerDataView view, string text, int startIndex, bool searchNext, bool ignoreStartIndex) {
			View = view;
			Text = text;
			StartIndex = startIndex;
			SearchNext = searchNext;
			IgnoreStartIndex = ignoreStartIndex;
		}
		public void Completed(object args) {
			CommandFindIncremental command = (CommandFindIncremental)args;
			if (command.RowIndex < 0)
				return;
			View.NotifyFindIncrementalCompleted(Text, StartIndex, SearchNext, IgnoreStartIndex, command.RowIndex);
		}
	}
}
