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

using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Mvvm.UI.Native;
#if SILVERLIGHT
using MessageBoxButton = DevExpress.Xpf.Core.DXMessageBoxButton;
#endif
namespace DevExpress.Xpf.Core {
	[TargetTypeAttribute(typeof(UserControl))]
	[TargetTypeAttribute(typeof(Window))]
	public class DialogService : ViewServiceBase, IDialogService, IMessageBoxButtonLocalizer, IMessageButtonLocalizer, IDocumentOwner {
		internal const string ShowDialogException = "Cannot use dialogButtons and dialogCommands parameters simultaneously.";
		public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(DialogService),  
			new PropertyMetadata(null, (d,e) => ((DialogService)d).OnTitleChanged()));
		[IgnoreDependencyPropertiesConsistencyChecker, System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Code Defects", "DXCA001")]
		static readonly DependencyProperty TitleFromViewModelProperty = DependencyProperty.Register("TitleFromViewModel", typeof(string), typeof(DialogService),
			new PropertyMetadata(null, (d, e) => ((DialogService)d).OnTitleFromViewModelChanged()));
		public static readonly DependencyProperty DialogStyleProperty = DependencyProperty.Register("DialogStyle", typeof(Style), typeof(DialogService), new PropertyMetadata(null));
		public static readonly DependencyProperty DialogWindowStartupLocationProperty = DependencyProperty.Register("DialogWindowStartupLocation", typeof(WindowStartupLocation), typeof(DialogService), new PropertyMetadata(WindowStartupLocation.CenterScreen));
		public static readonly DependencyProperty SetWindowOwnerProperty = DependencyProperty.Register("SetWindowOwner", typeof(bool), typeof(DialogService), new PropertyMetadata(true));
		public string Title { get { return (string)GetValue(TitleProperty); } set { SetValue(TitleProperty, value); } }
		string TitleFromViewModel { get { return (string)GetValue(TitleFromViewModelProperty); } set { SetValue(TitleFromViewModelProperty, value); } }
		public Style DialogStyle { get { return (Style)GetValue(DialogStyleProperty); } set { SetValue(DialogStyleProperty, value); } }
		public WindowStartupLocation DialogWindowStartupLocation { get { return (WindowStartupLocation)GetValue(DialogWindowStartupLocationProperty); } set { SetValue(DialogWindowStartupLocationProperty, value); } }
		public bool SetWindowOwner { get { return (bool)GetValue(SetWindowOwnerProperty); } set { SetValue(SetWindowOwnerProperty, value); } }
		[System.Security.SecuritySafeCritical]
		void UpdateWindowOwner(DXDialogWindow dialogWindow) {
			if(!SetWindowOwner || dialogWindow == null) return;
			if(!ViewModelBase.IsInDesignMode)
				dialogWindow.Owner = Window.GetWindow(AssociatedObject);
			else {
				System.Windows.Interop.WindowInteropHelper windowInteropHelper = new System.Windows.Interop.WindowInteropHelper(dialogWindow);
				windowInteropHelper.Owner = NativeMethods.GetActiveWindow();
			}
		}
		protected virtual DXDialogWindow CreateDialogWindow(object view) {
			var dialogWindow = new DXDialogWindow() {
				Content = view,
				WindowStartupLocation = DialogWindowStartupLocation,
			};
			UpdateWindowOwner(dialogWindow);
			InitializeDocumentContainer(dialogWindow, DXDialogWindow.ContentProperty, DialogStyle);
			UpdateThemeName(dialogWindow);
			windows.Add(new WeakReference(dialogWindow));
			return dialogWindow;
		}
		object GetViewModel(DXDialogWindow window) {
			return ViewHelper.GetViewModelFromView(window.Content);
		}
		List<WeakReference> windows = new List<WeakReference>();
		IEnumerable<DXDialogWindow> GetWindows() {
			for(int windowIndex = windows.Count; --windowIndex >= 0;) {
				DXDialogWindow window = (DXDialogWindow)windows[windowIndex].Target;
				if(window == null)
					windows.RemoveAt(windowIndex);
				else
					yield return window;
			}
		}
		void OnTitleChanged() {
			UpdateTitle();
		}
		void OnTitleFromViewModelChanged() {
			UpdateTitle();
		}
		void UpdateTitle() {
			if(DialogWindow == null) return;
			if(!string.IsNullOrEmpty(Title)) {
				DialogWindow.Title = Title;
				return;
			}
			if(!string.IsNullOrEmpty(TitleFromViewModel)) {
				DialogWindow.Title = TitleFromViewModel;
				return;
			}
			if(!string.IsNullOrEmpty(initTitle)) {
				DialogWindow.Title = initTitle;
				return;
			}
		}
		protected DXDialogWindow DialogWindow { get; private set; }
		string initTitle = null;
		UICommand IDialogService.ShowDialog(IEnumerable<UICommand> dialogCommands, string title, string documentType, object viewModel, object parameter, object parentViewModel) {
			object view = CreateAndInitializeView(documentType, viewModel, parameter, parentViewModel, this);
			return ShowDialog(dialogCommands, title, view);
		}
		protected UICommand ShowDialog(IEnumerable<UICommand> dialogCommands, string title, object view) {
			DialogWindow = CreateDialogWindow(view);
			initTitle = title;
			DocumentUIServiceBase.SetTitleBinding(view, TitleFromViewModelProperty, this, true);
			UpdateTitle();
			SubscribeWindow(DialogWindow);
			if(dialogCommands != null)
				DialogWindow.CommandsSource = dialogCommands;
			return DialogWindow.ShowDialogWindow();
		}
		void SubscribeWindow(Window window) {
			window.Closing += OnDialogWindowClosing;
			window.Closed += OnDialogWindowClosed;
		}
		void UnsubcribeWindow(Window window) {
			window.Closing -= OnDialogWindowClosing;
			window.Closed -= OnDialogWindowClosed;
		}
		protected virtual void OnDialogWindowClosing(object sender, CancelEventArgs e) {
			DXDialogWindow window = (DXDialogWindow)sender;
			DocumentViewModelHelper.OnClose(GetViewModel(window), e);
		}
		protected virtual void OnDialogWindowClosed(object sender, EventArgs e) {
			DXDialogWindow window = (DXDialogWindow)sender;
			UnsubcribeWindow(window);
			DocumentViewModelHelper.OnDestroy(GetViewModel(window));
			DialogWindow = null;
		}
		string IMessageBoxButtonLocalizer.Localize(MessageBoxResult button) {
			return Localize(button);
		}
		string IMessageButtonLocalizer.Localize(MessageResult button) {
			return Localize(button.ToMessageBoxResult());
		}
		string Localize(MessageBoxResult button) {
			return new DXDialogWindowMessageBoxButtonLocalizer().Localize(button);
		}
		void IDocumentOwner.Close(IDocumentContent documentContent, bool force) {
			DXDialogWindow documentWindow = GetWindows().Where(w => ViewHelper.GetViewModelFromView(w.Content) == documentContent).FirstOrDefault();
			if(documentWindow == null) return;
			if(force)
				documentWindow.Closing -= OnDialogWindowClosing;
			documentWindow.Close();
		}
	}
}
