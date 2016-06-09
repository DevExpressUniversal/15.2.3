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
	using System.ComponentModel;
	public interface IViewModelBase : INotifyPropertyChanged {
		IServiceContainer ServiceContainer { get; }
		T GetParentViewModel<T>() where T : class, IViewModelBase;
	}
	public interface IStepByStepConfiguratorViewModel<T, TContext> : IViewModelBase
		where T : IStepByStepConfiguratorPageViewModel<TContext> {
		TContext Context { get; }
		T SelectedPage { get; set; }
		IEnumerable<T> Pages { get; }
		void Add(IEnumerable<T> pages);
		void Remove(IEnumerable<T> pages);
		void RemoveLastPages();
		void BeginUpdate();
		void EndUpdate();
		int SelectedIndex { get; }
		bool IsNextPageAvailable { get; }
		bool IsPrevPageAvailable { get; }
		bool IsFinishAvailable { get; }
		ICommand<T> NextPageCommand { get; }
		ICommand<T> PrevPageCommand { get; }
		ICommand<T> FinishCommand { get; }
		ICommand<T> CloseCommand { get; }
		bool? Result { get; }
	}	
	public interface IStepByStepConfiguratorPageViewModel<TContext> : IViewModelBase {
		TContext Context { get; }
		bool IsCompleted { get; }
		void Enter();
		void Leave();
	}
	public interface IWindow {
		bool? DialogResult { get; set; }
	}
	public interface IMessageWindow : IWindow {
	}
	public interface IMessageWindowViewModel {
		bool? OwnerWindowResult { get; }
		IWindow OwnerWindow { get; }
		string Title { get; }
		string Message { get; }
		string ButtonText { get; }
		ICommand<IMessageWindow> AcceptCommand { get; }
		ICommand<IMessageWindow> CancelCommand { get; }
	}
	public interface IDXDesignWindow : IWindow {
	}
	public interface IDXDesignWindowViewModel : IViewModelBase {
		IWindow Window { get; }
		string Title { get; }
		string SupportMail { get; }
		string SupportTextLine1 { get; }
		string SupportTextLine2 { get; }
	}
	public interface IDXDesignWindowContentViewModel : IViewModelBase {
		bool? Close();
	}
	public interface IDXDesignWindowViewModel<TContentViewModel> : IDXDesignWindowViewModel
		where TContentViewModel : IDXDesignWindowContentViewModel {
		TContentViewModel ContentViewModel { get; }
		ICommand<IDXDesignWindow> MailToSupportCommand { get; }
		ICommand<IDXDesignWindow> CloseCommand { get; }
	}
}
