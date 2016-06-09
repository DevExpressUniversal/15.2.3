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

using System.Windows.Input;
using DevExpress.Utils;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraRichEdit.Model;
using System;
namespace DevExpress.XtraRichEdit {
	public class SearchPanelViewModel : ISearchPanelViewModel, IDisposable {
		RichEditControl richEditControl;
		public SearchPanelViewModel(RichEditControl richEditControl, bool replaceMode) {
			Guard.ArgumentNotNull(richEditControl, "RichEditControl");
			this.richEditControl = richEditControl;
			ReplaceMode = replaceMode;
			SubscribeToSearchCompleteEvent();
		}
		#region ISearchPanelViewModel Members
		public ICommand FindForwardCommand {
			get { return RichEditUICommand.EditFindForward; }
		}
		public ICommand FindBackwardCommand {
			get { return RichEditUICommand.EditFindBackward; }
		}
		public ICommand ReplaceForwardCommand {
			get { return RichEditUICommand.EditReplaceForward; }
		}
		public ICommand ReplaceAllForwardCommand {
			get { return RichEditUICommand.EditReplaceAllForward; }
		}
		public ICommand CloseCommand {
			get { return null; }
		}
		public string SearchString {
			get { return richEditControl.DocumentModel.SearchParameters.SearchString; }
			set { richEditControl.DocumentModel.SearchParameters.SearchString = value; }
		}
		public string ReplaceString {
			get { return richEditControl.DocumentModel.SearchParameters.ReplaceString; }
			set { richEditControl.DocumentModel.SearchParameters.ReplaceString = value; }
		}
		public bool CaseSensitive {
			get { return richEditControl.DocumentModel.SearchParameters.CaseSensitive; }
			set { richEditControl.DocumentModel.SearchParameters.CaseSensitive = value; }
		}
		public bool FindWholeWord {
			get { return richEditControl.DocumentModel.SearchParameters.FindWholeWord; }
			set { richEditControl.DocumentModel.SearchParameters.FindWholeWord = value; }
		}
		public bool UseRegularExpression {
			get { return richEditControl.DocumentModel.SearchParameters.UseRegularExpression; }
			set { richEditControl.DocumentModel.SearchParameters.UseRegularExpression = value; }
		}
		public bool ShowCaseSensitiveOption {
			get { return true; }
		}
		public bool ShowFindWholeWordOption {
			get { return true; }
		}
		public bool ShowUseRegularExpressionOption {
			get { return true; }
		}
		public bool ReplaceMode {
			get;
			private set;
		}
		#endregion
		void OnSearchComplete(object sender, SearchCompleteEventArgs e) {
			e.Continue = true;
		}
		void SubscribeToSearchCompleteEvent() {
			richEditControl.InnerControl.SearchComplete += OnSearchComplete;
		}
		void UnsubscribeToSearchCompleteEvent() {
			richEditControl.InnerControl.SearchComplete -= OnSearchComplete;
		}
		#region IDisposable Members
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (richEditControl != null) {
					UnsubscribeToSearchCompleteEvent();
					richEditControl = null;
				}
			}
		}
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
}
