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
using System.Linq;
using System.Text;
using DevExpress.DemoData.Helpers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using DevExpress.DemoData.Utils;
using System.Windows.Threading;
using System.Threading;
using System.Windows;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Mvvm.DataAnnotations;
namespace DevExpress.Xpf.DemoBase.Internal {
	class DemoBaseControlModulesPage : DemoBaseControlPart {
		public event ThePropertyChangedEventHandler<ProductDescription> ActualProductChanged;
		public event EventHandler CurrentProductChanged;
		public DemoBaseControlModulesPage(DemoBaseControl demoBaseControl)
			: base(demoBaseControl)
		{
			UseAnimation = true;
			FilterText = string.Empty;
			demoBaseControl.ActualPageChanged += OnActualPageChanged;
			Initialized();
		}
		public override void OnNavigatedTo(CompletePageState prevState, CompletePageState newState) {
			ModuleSelector.Do(ms => ms.OnNavigatedTo(prevState, newState));
			if(newState.Product == null)
				return;
			CurrentProduct = newState.Product;
			FilterText = newState.Filter;
		}
		public override void OnNavigatingFrom(CompletePageState initialState) {
			initialState = initialState.WithFilter(FilterText);
		}
		void OnActualPageChanged(object sender, DepPropertyChangedEventArgs e) {
			UseAnimation = (DemoBaseControlPage)e.NewValue == DemoBaseControlPage.Modules;
		}
		public string FilterText {
			get { return GetProperty(() => FilterText); }
			set { SetProperty(() => FilterText, value, UpdateModuleSelectorFilter); }
		}
		public bool UseAnimation {
			get { return GetProperty(() => UseAnimation); }
			set { SetProperty(() => UseAnimation, value); }
		}
		public DemoBaseControlModuleSelector ModuleSelector {
			get { return GetProperty(() => ModuleSelector); }
			set { SetProperty(() => ModuleSelector, value, UpdateModuleSelectorFilter); }
		}
		public ProductDescription NextProduct {
			get { return GetProperty(() => NextProduct); }
			set { SetProperty(() => NextProduct, value, OnNextProductChanged); }
		}
		public ProductDescription CurrentProduct {
			get { return GetProperty(() => CurrentProduct); }
			set { SetProperty(() => CurrentProduct, value, OnCurrentProductChanged); }
		}
		public ICommand RaiseActualProductChangedCommand { get { return new DelegateCommand(OnActualProductChanged); } }
		public ICommand CheckExamplesCommand { get { return new DelegateCommand(CheckExamples); } }
		void CheckExamples() {
			string link = string.Format("https://www.devexpress.com/Support/Center/Example/ChangeFilterSet?FavoritesOnly=False&MyItemsOnly=False&MyTeamItemsOnly=False&TechnologyId=4b2d6f97-c4ae-48fc-87f6-8c5da6541e40&PlatFormId=01f38b55-4ae1-49d2-be96-fef364931fdf&ProductId={0}&TicketType=Examples",
				CurrentProduct.ExamplesProductName);
			DocumentPresenter.OpenLink(link);
		}
		void OnCurrentProductChanged() {
			ModuleSelector = CurrentProduct == null ? null : new DemoBaseControlModuleSelector(CurrentProduct, DemoBaseControl);
			NextProduct = CurrentProduct;
			if(CurrentProductChanged != null)
				CurrentProductChanged(this, EventArgs.Empty);
		}
		void OnNextProductChanged() {
			if(NextProduct == CurrentProduct) return;
			if(NextProduct != null)
				NextProduct.Select();
		}
		void OnActualProductChanged() {
			if(ActualProductChanged != null)
				ActualProductChanged(this, new ThePropertyChangedEventArgs<ProductDescription>(null, CurrentProduct));
		}
		void UpdateModuleSelectorFilter() {
			if(ModuleSelector == null) return;
			ModuleSelector.Filter(FilterText);
		}
	}
}
