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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows.Media;
namespace DevExpress.Charts.Designer.Native {
	public sealed class RibbonPageGroupViewModel : RibbonItemViewModelBase {
		ObservableCollection<RibbonItemViewModelBase> barItems = new ObservableCollection<RibbonItemViewModelBase>();
		ImageSource image32x32 = null;
		bool allowCollapse = false;
		bool hideGroupIfAllCommandsAreDisabled;
		bool inInitialization = false;
		public ObservableCollection<RibbonItemViewModelBase> BarItems {
			get { return barItems; }
		}
		public ImageSource Image32x32 {
			get { return image32x32; }
			set {
				if (image32x32 != value) {
					image32x32 = value;
					OnPropertyChanged("Image32x32");
				}
			}
		}
		public bool AllowCollapse {
			get { return allowCollapse; }
			set {
				if (allowCollapse != value) {
					allowCollapse = value;
					OnPropertyChanged("AllowCollapse");
				}
			}
		}
		public RibbonPageGroupViewModel(bool hideGroupIfAllCommandsAreDisabled = false) {
			this.hideGroupIfAllCommandsAreDisabled = hideGroupIfAllCommandsAreDisabled;
			barItems.CollectionChanged += OnBarItemsCollectionChanged;
		}
		void UpdateGroupVisibilityByItemsEnabledAndVisibility() {
			bool isVisibleByVisibility = false;
			bool isVisibleByEnabled = false;
			int itemIndex = -1;
			foreach (RibbonItemViewModelBase item in barItems) {
				itemIndex++;
				if (item.IsVisible && !(item is BarStaticTextViewModel) && !(item is BarSeparatorViewModel))
					isVisibleByVisibility = true;
				if (item.IsEnabled && !(item is BarStaticTextViewModel) && !(item is BarSeparatorViewModel))
					isVisibleByEnabled = true;
				bool allItemsAfterSeparatorInvisible = false;
				if (item is BarSeparatorViewModel || item is BarStaticTextViewModel)
					allItemsAfterSeparatorInvisible = AreAllItemsAfterSeparatorInvisible(barItems, itemIndex);
				if (allItemsAfterSeparatorInvisible)
					HideStaticTextsAndSeparators(barItems, itemIndex);
				else
					ShowStaticTextsAndSeparators(barItems, itemIndex);
			}
			if (hideGroupIfAllCommandsAreDisabled)
				IsVisible = isVisibleByEnabled;
			else
				IsVisible = isVisibleByVisibility;
		}
		void HideStaticTextsAndSeparators(ObservableCollection<RibbonItemViewModelBase> barItems, int itemIndex) {
			this.inInitialization = true;
			for (int i = itemIndex; i < barItems.Count; i++)
				if (barItems[i] is BarStaticTextViewModel || barItems[i] is BarSeparatorViewModel)
					barItems[i].IsVisible = false;
			this.inInitialization = false;
		}
		void ShowStaticTextsAndSeparators(ObservableCollection<RibbonItemViewModelBase> barItems, int itemIndex) {
			this.inInitialization = true;
			for (int i = itemIndex; i < barItems.Count; i++)
				if (barItems[i] is BarStaticTextViewModel || barItems[i] is BarSeparatorViewModel)
					barItems[i].IsVisible = true;
			this.inInitialization = false;
		}
		bool AreAllItemsAfterSeparatorInvisible(ObservableCollection<RibbonItemViewModelBase> barItems, int itemIndex) {
			bool thereIsVisibleItems = false;
			for (int i = itemIndex; i < barItems.Count; i++)
				if (!(barItems[i] is BarStaticTextViewModel) && !(barItems[i] is BarSeparatorViewModel))
					thereIsVisibleItems |= barItems[i].IsVisible;
			return !thereIsVisibleItems;
		}
		void OnBarItemsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if (e.NewItems != null)
				foreach (RibbonItemViewModelBase item in e.NewItems)
					item.PropertyChanged += OnItemPropertyChanged;
			if (e.OldItems != null)
				foreach (RibbonItemViewModelBase item in e.OldItems)
					item.PropertyChanged -= OnItemPropertyChanged;
			UpdateGroupVisibilityByItemsEnabledAndVisibility();
		}
		void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e) {
			if (this.inInitialization)
				return;
			if (e.PropertyName == "IsEnabled" || e.PropertyName == "IsVisible")
				UpdateGroupVisibilityByItemsEnabledAndVisibility();
		}
		public override void CleanUp() {
			base.CleanUp();
			foreach (var model in BarItems)
				model.CleanUp();
		}
		public override string ToString() {
			string caption = Caption ?? string.Empty;
			return GetType().Name + "[" + caption + "]";
		}
	}
}
