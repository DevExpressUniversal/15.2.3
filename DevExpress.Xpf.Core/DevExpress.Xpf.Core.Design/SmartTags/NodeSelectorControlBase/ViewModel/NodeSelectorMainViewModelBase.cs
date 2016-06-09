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

using DevExpress.Design.UI;
using System;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public abstract class NodeSelectorMainViewModelBase : WpfBindableBase {
		NodeSelectorItemSelectorViewModel itemSelector;
		ICommand okCommand;
		ICommand cancelCommand;
		public NodeSelectorMainViewModelBase(INodeSelectorControlBase control) {
			ControlBase = control;
			control.SelectedItemChanged += OnControlSelectedItemChanged;
			OnControlSelectedItemChanged(control, EventArgs.Empty);
		}
		public NodeSelectorItemSelectorViewModel ItemSelector {
			get {
				if(itemSelector == null)
					itemSelector = new NodeSelectorItemSelectorViewModel(this);
				return itemSelector;
			}
		}
		public ICommand OKCommand {
			get {
				if(okCommand == null)
					okCommand = new WpfDelegateCommand(Done, false);
				return okCommand;
			}
		}
		public ICommand CancelCommand {
			get {
				if(cancelCommand == null)
					cancelCommand = new WpfDelegateCommand(Close, false);
				return cancelCommand;
			}
		}
		public void Done() {
			ControlBase.SelectedItem = ItemSelector.SelectedTreeNode;
			Close();
		}
		public void Close() {
			if(ControlBase.CloseCommand != null && ControlBase.CloseCommand.CanExecute(ControlBase.CloseCommandParameter))
				ControlBase.CloseCommand.Execute(ControlBase.CloseCommandParameter);
		}
		protected INodeSelectorControlBase ControlBase { get; private set; }
		protected abstract INodeSelectorDataProvider GetDataProvider();
		protected virtual void OnControlDataProviderChanged(object sender, ThePropertyChangedEventArgs<INodeSelectorDataProvider> e) {
			if(e.OldValue != null) {
				e.OldValue.DefaultSelectedItemChanged -= OnDataProviderDefaultSelectedItemChanged;
				e.OldValue.RootChanged -= OnDataProviderRootChanged;
			}
			BeginLoading();
			if(e.NewValue != null) {
				e.NewValue.RootChanged += OnDataProviderRootChanged;
				e.NewValue.DefaultSelectedItemChanged += OnDataProviderDefaultSelectedItemChanged;
			}
			OnCurrentDataProviderChanged();
		}
		protected virtual void OnCurrentDataProviderChanged() {
			UpdateItemSelector();
			LoadData();
		}
		protected virtual void OnControlSelectedItemChanged(object sender, EventArgs e) {
			ItemSelector.SelectedTreeNode = ControlBase.SelectedItem;
		}
		void UpdateItemSelector() {
			INodeSelectorDataProvider dataProvider = GetDataProvider();
			if(dataProvider == null) {
				ItemSelector.RootTreeNode = null;
				ItemSelector.SelectedTreeNode = null;
				return;
			}
			INodeSelectorItem rootTreeNode = dataProvider.Root;
			if(rootTreeNode == null) return;
			ItemSelector.RootTreeNode = rootTreeNode;
			if(ItemSelector.SelectedTreeNode == null)
				ItemSelector.SelectedTreeNode = dataProvider == null ? null : dataProvider.DefaultSelectedItem;
		}
		void BeginLoading() {
			ItemSelector.Loading = true;
		}
		void EndLoading() {
			ItemSelector.Loading = false;
		}
		void OnDataProviderRootChanged(object sender, EventArgs e) {
			UpdateItemSelector();
			EndLoading();
		}
		void OnDataProviderDefaultSelectedItemChanged(object sender, EventArgs e) {
			UpdateItemSelector();
		}
		void LoadData() {
			INodeSelectorDataProvider dataProvider = GetDataProvider();
			if(dataProvider != null)
				dataProvider.Load();
			else
				EndLoading();
		}
	}
}
