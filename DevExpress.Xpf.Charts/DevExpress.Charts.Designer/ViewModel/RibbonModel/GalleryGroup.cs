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
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
namespace DevExpress.Charts.Designer.Native {
	public class GalleryGroupViewModel : RibbonItemViewModelBase {
		ObservableCollection<BarButtonViewModel> items = new ObservableCollection<BarButtonViewModel>();
		bool hideGroupIfAllItemsAreDisabled;
		bool areAllCommandsDisabled;
		public ObservableCollection<BarButtonViewModel> Items {
			get { return items; }
		}
		public bool AreAllCommandsDisabled {
			get { return areAllCommandsDisabled; }
			private set {
				if (areAllCommandsDisabled != value) {
					areAllCommandsDisabled = value;
					OnPropertyChanged("AreAllCommandsDisabled");
				}
			}
		}
		public GalleryGroupViewModel(bool hideGroupIfAllItemsAreDisabled = false) {
			this.hideGroupIfAllItemsAreDisabled = hideGroupIfAllItemsAreDisabled;
			Items.CollectionChanged += ItemsCollectionChanged;
			if (hideGroupIfAllItemsAreDisabled)
				UpdateVisibilityAndCheckEnabledOfCommands();
			else
				CheckEnabledOfCommandsOnly();
		}
		void ItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.OldItems != null)
				foreach (BarButtonViewModel item in e.OldItems)
					item.Command.CanExecuteChanged -= OnCanExecuteChanged;
			if (e.NewItems != null)
				foreach (BarButtonViewModel item in e.NewItems)
					item.Command.CanExecuteChanged += OnCanExecuteChanged;
			if (this.hideGroupIfAllItemsAreDisabled)
				UpdateVisibilityAndCheckEnabledOfCommands();
			else
				CheckEnabledOfCommandsOnly();
		}
		void OnCanExecuteChanged(object sender, EventArgs e) {
			if (hideGroupIfAllItemsAreDisabled)
				UpdateVisibilityAndCheckEnabledOfCommands();
			else
				CheckEnabledOfCommandsOnly();
		}
		void UpdateVisibilityAndCheckEnabledOfCommands() {
			IsVisible = false;
			foreach (BarButtonViewModel item in Items)
				IsVisible |= item.Command.CanExecute(item.CommandParameter);
			bool oldAreAllCommandsDisabled = areAllCommandsDisabled;
			AreAllCommandsDisabled = !IsVisible;
		}
		void CheckEnabledOfCommandsOnly() {
			foreach (BarButtonViewModel item in Items)
				if (item.Command.CanExecute(item.CommandParameter)) {
					AreAllCommandsDisabled = false;
					return;
				}
				else
					AreAllCommandsDisabled = true;
		}
		public override void CleanUp() {
			base.CleanUp();
			foreach (var model in Items)
				model.CleanUp();
		}
	}
}
