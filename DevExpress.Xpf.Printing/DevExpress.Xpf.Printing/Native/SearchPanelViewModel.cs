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
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Navigation;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Printing.Native {
	public class SearchPanelViewModel : ISearchPanelViewModel, INotifyPropertyChanged {
		readonly DelegateCommand<object> findNextCommand;
		readonly DelegateCommand<object> findPrevCommand;
		readonly DelegateCommand<object> closeCommand;
		int brickIndex = -1;
		string searchString = string.Empty;
#if DEBUGTEST
		internal
#endif
		BrickPagePairCollection brickPagePairs;
		IDocumentPreviewModel model;
		bool caseSensitive;
		bool findWholeWord;
		public SearchPanelViewModel() {
			findNextCommand = DelegateCommandFactory.Create<object>(FindNext, CanFind, false);
			findPrevCommand = DelegateCommandFactory.Create<object>(FindPrev, CanFind, false);
			closeCommand = DelegateCommandFactory.Create<object>(Close, (p) => { return true; }, false);
			brickPagePairs = new BrickPagePairCollection();
		}
		public IDocumentPreviewModel Model {
			get { return model; }
			set {
				if(IsModelCorrect)
					((PrintingSystemPreviewModel)Model).PrintingSystem.DocumentChanged -= PrintingSystem_DocumentChanged;
				model = value;
				if(IsModelCorrect)
					((PrintingSystemPreviewModel)Model).PrintingSystem.DocumentChanged += PrintingSystem_DocumentChanged;
				RaiseFindCommandsCanExecute();
				Clear();
			}
		}
		bool IsModelCorrect {
			get { return Model != null && Model as PrintingSystemPreviewModel != null && ((PrintingSystemPreviewModel)Model).PrintingSystem != null; }
		}
		void PrintingSystem_DocumentChanged(object sender, EventArgs e) {
			RaiseFindCommandsCanExecute();
			Clear();
		}
		public Document Document {
			get {
				if(IsModelCorrect)
					return ((PrintingSystemPreviewModel)Model).PrintingSystem.Document;
				else
					return null;
			}
		}
		int MinBrickIndex {
			get { return 0; }
		}
		int MaxBrickIndex {
			get { return brickPagePairs.Count == 0 ? 0 : brickPagePairs.Count - 1; }
		}
		void Close(object parameter) {
			if(Model == null || Model as DocumentPreviewModelBase == null)
				return;
			ICommand command = ((DocumentPreviewModelBase)Model).ToggleSearchPanelCommand;
			if(command != null && command.CanExecute(null))
				command.Execute(null);
		}
		bool CanFind(object parameter) {
			return !string.IsNullOrEmpty(SearchString) && Model != null && Document != null;
		}
		void FindNext(object parameter) {
			FindCore(SearchDirection.Down);
		}
		void FindPrev(object parameter) {
			FindCore(SearchDirection.Up);
		}
		void FindCore(SearchDirection searchDirection) {
			if(brickPagePairs.Count == 0) {
				PrintingSystemBase ps = ((PrintingSystemPreviewModel)Model).PrintingSystem;
				TextBrickSelector selector = new TextBrickSelector(SearchString, FindWholeWord, CaseSensitive, ps);
				brickPagePairs = NavigateHelper.SelectBrickPagePairs(Document, selector, new BrickPagePairComparer(ps.Pages));
			}
			if(brickPagePairs.Count != 0) {
				CalcBrickIndex(searchDirection);
				BrickPagePair brickPagePair = brickPagePairs[brickIndex];
				string brickTag = DocumentMapTreeViewNodeHelper.GetTagByIndices(brickPagePair.BrickIndices, brickPagePair.PageIndex);
				Model.FoundBrickInfo = new BrickInfo(brickTag, brickPagePair.PageIndex);
			} else {
				Model.FoundBrickInfo = BrickInfo.Empty;
				((DocumentPreviewModelBase)Model).DialogService.ShowInformation(PrintingLocalizer.GetString(PrintingStringId.Search_EmptyResult), PrintingLocalizer.GetString(PrintingStringId.Information));
			}
		}
		void CalcBrickIndex(SearchDirection searchDirection) {
			switch(searchDirection) {
				case SearchDirection.Down:
					brickIndex++;
					break;
				case SearchDirection.Up:
					brickIndex--;
					break;
				default:
					throw new NotSupportedException("searchDirection");
			}
			if(brickIndex < MinBrickIndex)
				brickIndex = MaxBrickIndex;
			if(brickIndex > MaxBrickIndex)
				brickIndex = MinBrickIndex;
		}
		void Clear() {
			brickIndex = -1;
			brickPagePairs.Clear();
			if(Model != null)
				Model.FoundBrickInfo = null;
		}
		void RaiseFindCommandsCanExecute() {
			if(findNextCommand != null)
				findNextCommand.RaiseCanExecuteChanged();
			if(findPrevCommand != null)
				findPrevCommand.RaiseCanExecuteChanged();
		}
		#region ISearchPanelViewModel Members
		public bool ReplaceMode {
			get { return false; }
		}
		public bool ShowCaseSensitiveOption {
			get { return true; }
		}
		public bool ShowFindWholeWordOption {
			get { return true; }
		}
		public bool ShowUseRegularExpressionOption {
			get { return false; }
		}
		public bool CaseSensitive {
			get { return caseSensitive; }
			set {
				if(caseSensitive == value)
					return;
				caseSensitive = value;
				Clear();
			}
		}
		public bool FindWholeWord {
			get { return findWholeWord; }
			set {
				if(findWholeWord == value)
					return;
				findWholeWord = value;
				Clear();
			}
		}
		public bool UseRegularExpression {
			get;
			set;
		}
		public string SearchString {
			get { return searchString; }
			set {
				searchString = value;
				Clear();
				RaiseFindCommandsCanExecute();
			}
		}
		public ICommand CloseCommand {
			get { return closeCommand; }
		}
		public ICommand FindBackwardCommand {
			get { return findPrevCommand; }
		}
		public ICommand FindForwardCommand {
			get { return findNextCommand; }
		}
		public ICommand ReplaceAllForwardCommand {
			get { return null; }
		}
		public ICommand ReplaceForwardCommand {
			get { return null; }
		}
		public string ReplaceString {
			get;
			set;
		}
		#endregion
		#region INotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		protected void RaisePropertyChanged<T>(Expression<Func<T>> property) {
			PropertyExtensions.RaisePropertyChanged(this, PropertyChanged, property);
		}
		#endregion
	}
}
