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
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
namespace DevExpress.Mvvm {
	public class WizardController : ViewModelBase {
		protected static T GetServiceFromViewModel<T>(object viewModel) where T : class {
			GuardHelper.ArgumentNotNull(viewModel, "viewModel");
			return GuardHelper.ArgumentMatchType<ISupportServices>(viewModel, "viewModel").ServiceContainer.GetRequiredService<T>();
		}
		readonly Func<INavigationService> getNavigationServiceCallback;
		readonly Func<ICurrentDialogService> getCurrentDialogServiceCallback;
		string startPageViewType;
		object startPageViewModel;
		readonly object parameter;
		public WizardController(object viewModel, object parameter)
			: this(null, null, parameter) {
			GuardHelper.ArgumentNotNull(viewModel, "viewModel");
			this.SetParentViewModel(viewModel);
			this.getNavigationServiceCallback = () => GetServiceFromViewModel<INavigationService>(viewModel);
			this.getCurrentDialogServiceCallback = () => GetServiceFromViewModel<ICurrentDialogService>(viewModel);
			Initialize();
		}
		public WizardController(Func<INavigationService> getNavigationServiceCallback, Func<ICurrentDialogService> getCurrentDialogServiceCallback, object parameter, object parentViewModel)
			: this(null, null, parameter) {
			GuardHelper.ArgumentNotNull(getNavigationServiceCallback, "getNavigationServiceCallback");
			GuardHelper.ArgumentNotNull(getCurrentDialogServiceCallback, "getCurrentDialogServiceCallback");
			this.SetParentViewModel(parentViewModel);
			this.getNavigationServiceCallback = getNavigationServiceCallback;
			this.getCurrentDialogServiceCallback = getCurrentDialogServiceCallback;
			Initialize();
		}
		public WizardController(string startPageViewType, object startPageViewModel, object parameter) {
			if(startPageViewType != null && startPageViewModel != null)
				throw new ArgumentException();
			this.startPageViewType = startPageViewType;
			this.startPageViewModel = startPageViewModel;
			this.parameter = parameter;
			this.getNavigationServiceCallback = () => GetServiceFromViewModel<INavigationService>(this);
			this.getCurrentDialogServiceCallback = () => GetServiceFromViewModel<ICurrentDialogService>(this);
		}
		DelegateCommand onLoadedCommand;
		public ICommand OnLoadedCommand {
			get {
				if(onLoadedCommand == null)
					onLoadedCommand = new DelegateCommand(OnLoaded);
				return onLoadedCommand;
			}
		}
		protected virtual void OnLoaded() {
			Initialize();
		}
		protected void Initialize() {
			NavigateToPageCommand = new DelegateCommand<string>(NavigateToPage);
			NavigateToCommand = new DelegateCommand<object>(NavigateTo);
			NextCommand = new DelegateCommand(Next, CanNext);
			FinishCommand = new DelegateCommand<CancelEventArgs>(Finish, CanFinish);
			BackCommand = new DelegateCommand(Back, CanBack);
			CancelCommand = new DelegateCommand<CancelEventArgs>(Cancel);
			var navigationService = NavigationService;
			navigationService.CurrentChanged += OnNavigationServiceCurrentChanged;
			navigationService.CanGoBackChanged += OnNavigationServiceCanGoBackChanged;
			navigationService.CanGoForwardChanged += OnNavigationServiceCanGoForwardChanged;
			if(startPageViewType != null) {
				NavigateToPage(startPageViewType);
				startPageViewType = null;
			} else if(startPageViewModel != null) {
				NavigateTo(startPageViewModel);
				startPageViewModel = null;
			}
		}
		protected INavigationService NavigationService { get { return getNavigationServiceCallback(); } }
		protected ICurrentDialogService CurrentDialogService { get { return getCurrentDialogServiceCallback(); } }
		protected bool IsInitnialized { get { return this.getNavigationServiceCallback != null; } }
		protected void AssertIsInitialized() {
			if(!IsInitnialized)
				throw new InvalidOperationException();
		}
		ICommand<string> navigateToPageCommand;
		public ICommand<string> NavigateToPageCommand {
			get { return navigateToPageCommand; }
			private set { SetProperty(ref navigateToPageCommand, value, () => NavigateToPageCommand); }
		}
		ICommand<object> navigateToCommand;
		public ICommand<object> NavigateToCommand {
			get { return navigateToCommand; }
			private set { SetProperty(ref navigateToCommand, value, () => NavigateToCommand); }
		}
		public void NavigateToPage(string page) {
			AssertIsInitialized();
			NavigationService.Navigate(page, parameter, this);
		}
		public void NavigateTo(object pageViewModel) {
			AssertIsInitialized();
			NavigationService.Navigate(pageViewModel, parameter, this);
		}
		void OnNavigationServiceCanGoForwardChanged(object sender, EventArgs e) {
			UpdateNextCommand();
		}
		void OnNavigationServiceCanGoBackChanged(object sender, EventArgs e) {
			UpdateBackCommand();
		}
		void OnNavigationServiceCurrentChanged(object sender, EventArgs e) {
			CurrentPage = NavigationService.Current;
		}
		object currentPage;
		public object CurrentPage {
			get { return currentPage; }
			private set {
				object oldValue = currentPage;
				if(SetProperty(ref currentPage, value, () => CurrentPage))
					OnCurrentPageChanged(oldValue);
			}
		}
		void OnCurrentPageChanged(object oldValue) {
			var oldPage = oldValue as INotifyPropertyChanged;
			if(oldPage != null)
				oldPage.PropertyChanged -= OnPagePropertyChanged;
			var newPage = CurrentPage as INotifyPropertyChanged;
			if(newPage != null)
				newPage.PropertyChanged += OnPagePropertyChanged;
			UpdateNextCommand();
			UpdateFinishCommand();
		}
		void OnPagePropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == ExpressionHelper.GetPropertyName((ISupportWizardNextCommand x) => x.CanGoForward))
				UpdateNextCommand();
			else if(e.PropertyName == ExpressionHelper.GetPropertyName((ISupportWizardFinishCommand x) => x.CanFinish))
				UpdateFinishCommand();
		}
		void UpdateNextCommand() {
			((DelegateCommand)NextCommand).RaiseCanExecuteChanged();
		}
		void UpdateFinishCommand() {
			((DelegateCommand<CancelEventArgs>)FinishCommand).RaiseCanExecuteChanged();
		}
		void UpdateBackCommand() {
			((DelegateCommand)BackCommand).RaiseCanExecuteChanged();
		}
		ICommand nextCommand;
		public ICommand NextCommand {
			get { return nextCommand; }
			private set { SetProperty(ref nextCommand, value, () => NextCommand); }
		}
		public void Next() {
			AssertIsInitialized();
			var supportWizard = CurrentPage as ISupportWizard;
			if(supportWizard == null && !NavigationService.CanGoForward) return;
			var args = new CancelEventArgs();
			(CurrentPage as ISupportWizardNextCommand).Do(x => x.OnGoForward(args));
			if(args.Cancel) return;
			if(supportWizard == null)
				NavigationService.GoForward();
			else
				supportWizard.NavigateToNextPage(this);
		}
		public bool CanNext() {
			AssertIsInitialized();
			var supportNextCommand = CurrentPage as ISupportWizardNextCommand;
			if(supportNextCommand != null && !supportNextCommand.CanGoForward) return false;
			return NavigationService.CanGoForward || CurrentPage is ISupportWizard;
		}
		ICommand<CancelEventArgs> finishCommand;
		public ICommand<CancelEventArgs> FinishCommand {
			get { return finishCommand; }
			private set { SetProperty(ref finishCommand, value, () => FinishCommand); }
		}
		public void Finish(CancelEventArgs e) {
			AssertIsInitialized();
			var supportFinishCommand = CurrentPage as ISupportWizardFinishCommand;
			var args = e ?? new CancelEventArgs();
			if(supportFinishCommand != null)
				supportFinishCommand.OnFinish(args);
			if(e == null && !args.Cancel)
				CurrentDialogService.Close(MessageResult.OK);
		}
		public bool CanFinish(CancelEventArgs e) {
			AssertIsInitialized();
			var supportFinishCommand = CurrentPage as ISupportWizardFinishCommand;
			return supportFinishCommand == null || supportFinishCommand.CanFinish;
		}
		ICommand backCommand;
		public ICommand BackCommand {
			get { return backCommand; }
			private set { SetProperty(ref backCommand, value, () => BackCommand); }
		}
		public void Back() {
			AssertIsInitialized();
			var args = new CancelEventArgs();
			(CurrentPage as ISupportWizardBackCommand).Do(x => x.OnGoBack(args));
			if(!args.Cancel)
				NavigationService.GoBack();
		}
		public bool CanBack() {
			AssertIsInitialized();
			return NavigationService.CanGoBack;
		}
		ICommand<CancelEventArgs> cancelCommand;
		public ICommand<CancelEventArgs> CancelCommand {
			get { return cancelCommand; }
			private set { SetProperty(ref cancelCommand, value, () => CancelCommand); }
		}
		public void Cancel(CancelEventArgs e) {
			AssertIsInitialized();
			var supportCancelCommand = CurrentPage as ISupportWizardCancelCommand;
			var args = e ?? new CancelEventArgs();
			if(supportCancelCommand != null)
				supportCancelCommand.OnCancel(args);
			if(e == null && !args.Cancel)
				CurrentDialogService.Close(MessageResult.Cancel);
		}
	}
}
