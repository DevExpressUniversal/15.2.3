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

namespace DevExpress.Design.UI {
	using System.Collections.Generic;
	public class StepByStepConfiguratorViewModel<T, TContext> : WpfViewModelBase, IStepByStepConfiguratorViewModel<T, TContext>
		where T : IStepByStepConfiguratorPageViewModel<TContext> {
		List<T> pages;
		readonly TContext contextCore;
		protected StepByStepConfiguratorViewModel(IViewModelBase parentViewModel, TContext context)
			: base(parentViewModel) {
			this.contextCore = context;
			InitCore();
		}
		public TContext Context {
			get { return contextCore; }
		}
		protected virtual IEnumerable<T> GetPages() {
			return new T[] { };
		}
		public int Count {
			get {
				if(pages == null)
					return -1;
				return pages.Count;
			}
		}
		void InitCore() {
			InitServices();
			pages = new List<T>(GetPages());
			if(Count > 0)
				SelectedIndex = GetStartPageIndex();
			InitCommands();
		}
		protected virtual int GetStartPageIndex() {
			return 0;
		}
		#region Properties
		int selectedIndexCore = -1;
		public int SelectedIndex {
			get { return selectedIndexCore; }
			set {
				int oldValue = selectedIndexCore;
				if(SetProperty(ref selectedIndexCore, value, "SelectedIndex"))
					OnSelectedIndexChanged(oldValue, value);
			}
		}
		public IEnumerable<T> Pages {
			get { return pages; }
		}
		void InvokePagesPropertyChanged() {
			this.RaisePropertyChanged("Pages");
		}
		T selectedPageCore;
		public T SelectedPage {
			get { return selectedPageCore; }
			set {
				T oldValue = selectedPageCore;
				if(SetProperty(ref selectedPageCore, value, "SelectedPage"))
					OnSelectedPageChanged(oldValue, value);
			}
		}
		public bool? Result {
			get;
			private set;
		}
		bool isNextPageAvailableCore;
		public bool IsNextPageAvailable {
			get { return isNextPageAvailableCore; }
			set { SetProperty(ref isNextPageAvailableCore, value, "IsNextPageAvailable"); }
		}
		bool isPrevPageAvailableCore;
		public bool IsPrevPageAvailable {
			get { return isPrevPageAvailableCore; }
			set { SetProperty(ref isPrevPageAvailableCore, value, "IsPrevPageAvailable"); }
		}
		bool isFinishAvailableCore;
		public bool IsFinishAvailable {
			get { return isFinishAvailableCore; }
			set { SetProperty(ref isFinishAvailableCore, value, "IsFinishAvailable"); }
		}
		#endregion Properties
		#region Commands
		public ICommand<T> NextPageCommand {
			get;
			private set;
		}
		public ICommand<T> PrevPageCommand {
			get;
			private set;
		}
		public ICommand<T> FinishCommand {
			get;
			private set;
		}
		public ICommand<T> CloseCommand {
			get;
			private set;
		}
		#endregion Commands
		protected virtual void InitServices() { }
		void InitCommands() {
			NextPageCommand = new WpfDelegateCommand<T>(SelectNextPage, CanSelectNextPage);
			PrevPageCommand = new WpfDelegateCommand<T>(SelectPrevPage, CanSelectPrevPage);
			FinishCommand = new WpfDelegateCommand<T>(Finish, CanFinish);
			CloseCommand = new WpfDelegateCommand<T>(Close);
		}
		bool CanSelectNextPage(T selectedPage) {
			return (selectedPage != null) && selectedPage.IsCompleted && HasNext(SelectedIndex);
		}
		bool HasNext(int index) {
			return (Count > 1) && (index + 1) < Count;
		}
		void SelectNextPage(T selectedPage) {
			SelectedIndex++;
		}
		protected virtual bool CanSelectPrevPage(T selectedPage) {
			return (selectedPage != null) && HasPrev(SelectedIndex);
		}
		bool HasPrev(int index) {
			return index > 0;
		}
		void SelectPrevPage(T selectedPage) {
			SelectedIndex--;
		}
		bool CanFinish(T selectedPage) {
			return (selectedPage != null) && selectedPage.IsCompleted && IsLast(SelectedIndex);
		}
		bool IsLast(int index) {
			return index == (Count - 1);
		}
		void Finish(T selectedPage) {
			if(OnFinishing(selectedPage)) {
				Result = true;
				OnFinished(selectedPage);
			}
		}
		void Close(T selectedPage) {
			if(OnClosing(selectedPage)) {
				Result = false;
				OnClosed(selectedPage);
			}
		}
		public bool? Close() {
			if(!Result.HasValue)
				Close(SelectedPage);
			return Result;
		}
		protected virtual bool OnFinishing(T selectedPage) { return true; }
		protected virtual bool OnClosing(T selectedPage) { return true; }
		protected virtual void OnClosed(T selectedPage) {
			IDXDesignWindowViewModel windowModel = GetParentViewModel<IDXDesignWindowViewModel>();
			if(windowModel != null && windowModel.Window != null)
				windowModel.Window.DialogResult = Result;
		}
		protected virtual void OnFinished(T selectedPage) {
			IDXDesignWindowViewModel windowModel = GetParentViewModel<IDXDesignWindowViewModel>();
			if(windowModel != null && windowModel.Window != null)
				windowModel.Window.DialogResult = Result;
		}
		protected virtual void OnSelectedIndexChanged(int oldValue, int newValue) {
			SelectedPage = System.Linq.Enumerable.ElementAt(Pages, SelectedIndex);
			UpdateIsAvailableProperties(SelectedIndex);
		}
		protected virtual void UpdateIsAvailableProperties(int selectedIndex) {
			IsNextPageAvailable = HasNext(selectedIndex);
			IsPrevPageAvailable = HasPrev(selectedIndex);
			IsFinishAvailable = IsLast(selectedIndex);
		}
		protected virtual void LeavePage(T page) {
			if(page != null)
				page.Leave();
		}
		protected virtual void EnterPage(T page) {
			if(page != null)
				page.Enter();
		}
		protected virtual void OnSelectedPageChanged(T oldPage, T page) {
			UnsubscribePage(oldPage);
			LeavePage(oldPage);
			SubscribePage(page);
			EnterPage(page);
		}
		void SubscribePage(T page) {
			if(page != null) page.PropertyChanged += Page_PropertyChanged;
		}
		void UnsubscribePage(T page) {
			if(page != null) page.PropertyChanged -= Page_PropertyChanged;
		}
		void Page_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if(e.PropertyName == "IsCompleted")
				UpdateIsAvailableProperties(SelectedIndex);
		}
		public void Add(IEnumerable<T> pages) {
			if(pages == null || this.pages == null)
				return;
			BeginUpdate();
			try {
				foreach(T page in pages)
					if(!this.pages.Contains(page))
						this.pages.Add(page);
				this.pages = new List<T>(this.pages);
			}
			finally {
				EndUpdate();
			}
		}
		public void Remove(IEnumerable<T> pages) {
			if(pages == null || this.pages == null)
				return;
			BeginUpdate();
			try {
				foreach(T page in pages) {
					int index = this.pages.IndexOf(page);
					if(index >= 0 && index > SelectedIndex)
						this.pages.Remove(page);
				}
				this.pages = new List<T>(this.pages);
			}
			finally {
				EndUpdate();
			}
		}
		public void RemoveLastPages() {
			if(this.pages == null)
				return;
			List<T> toRemove = new List<T>();
			int count = Count;
			for(int i = SelectedIndex + 1; i < count; i++)
				toRemove.Add(this.pages[i]);
			this.Remove(toRemove);
		}
		int updatersCounter = 0;
		public void BeginUpdate() {
			updatersCounter++;
		}
		public void EndUpdate() {
			updatersCounter--;
			if(updatersCounter == 0) {
				InvokePagesPropertyChanged();
				UpdateIsAvailableProperties(SelectedIndex);
			}
			if(updatersCounter < 0)
				updatersCounter = 0;
		}
	}
}
