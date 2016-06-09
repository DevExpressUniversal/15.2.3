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
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Extensions.Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class DesignTimeImagePickerMainViewModel : BindableBase {
		readonly IDesignTimeImagePicker control;
		readonly Func<IEnumerable<IDesignTimeImagePickerImageInfo>, Thread> updateImagesAsync;
		readonly DesignTimeImagePickerFilterItem localCollection = new DesignTimeImagePickerFilterItem("(Local)") { IsSelected = true };
		readonly DesignTimeImagePickerFilterItem grayscaleCollection = new DesignTimeImagePickerFilterItem("DX Grayscale") { IsSelected = false };
		readonly DesignTimeImagePickerFilterItem devAVCollection = new DesignTimeImagePickerFilterItem("DX DevAV") { IsSelected = false };
		readonly DesignTimeImagePickerFilterItem coloredCollection = new DesignTimeImagePickerFilterItem("DX Colored") { IsSelected = true };
		readonly DesignTimeImagePickerFilterItem officeCollection = new DesignTimeImagePickerFilterItem("DX Office2013") { IsSelected = false };
		readonly DesignTimeImagePickerFilterItem smallSize = new DesignTimeImagePickerFilterItem("16x16");
		readonly DesignTimeImagePickerFilterItem largeSize = new DesignTimeImagePickerFilterItem("32x32");
		bool isFirstWay = true;
		ICommand okCommand;
		ICommand cancelCommand;
		DesignTimeImagePickerImageViewModel selectedImage;
		string filterText;
		ICommand selectAndCloseCommand;
		Dispatcher dispatcher;
		string selectedImageOriginalString;
		public DesignTimeImagePickerMainViewModel(IDesignTimeImagePicker control) {
			this.control = control;
			dispatcher = Dispatcher.CurrentDispatcher;
			Categories = new DesignTimeImagePickerFilterItemCollection("Select All");
			Images = new ObservableCollection<DesignTimeImagePickerImageViewModel>();
			ImagesCollection = CollectionViewSource.GetDefaultView(Images);
			ImagesCollection.GroupDescriptions.Add(new PropertyGroupDescription("Group"));
			updateImagesAsync = AsyncHelper.Create<IEnumerable<IDesignTimeImagePickerImageInfo>>(UpdateImagesAsync);
			control.GroupsChanged += OnControlGroupsChanged;
			control.PropertyNameChanged += OnPropertyNameChanged;
			control.SelectedImageOriginalStringChanged += OnSelectedImageOriginalStringChanged;
			OnControlGroupsChanged(this.control, new ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>>(null, this.control.Groups));
			this.control.DataProviderChanged += OnControlDataProviderChanged;
			OnControlDataProviderChanged(this.control, new ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier>(null, this.control.DataProvider));
		}
		public DesignTimeImagePickerFilterItemCollection Categories { get; private set; }
		public ObservableCollection<DesignTimeImagePickerImageViewModel> Images { get; private set; }
		public ICollectionView ImagesCollection { get; private set; }
		public DesignTimeImagePickerImageViewModel SelectedImage {
			get { return selectedImage; }
			set { SetProperty(ref selectedImage, value, () => SelectedImage); }
		}
		public string PropertyName {
			get { return control.PropertyName; }
		}
		public string SelectedImageOriginalString {
			get { return selectedImageOriginalString; }
			set { SetProperty(ref selectedImageOriginalString, value, () => SelectedImageOriginalString); }
		}
		public IEnumerable<DesignTimeImagePickerFilterItem> Sizes {
			get {
				yield return smallSize;
				yield return largeSize;
			}
		}
		public IEnumerable<DesignTimeImagePickerFilterItem> Collections {
			get {
				yield return localCollection;
				yield return grayscaleCollection;
				yield return officeCollection;
				yield return coloredCollection;
				yield return devAVCollection;
			}
		}
		public Thread UpdateImagesThread { get; private set; }
		public void Done() {
			control.SelectedImage = SelectedImage == null ? null : SelectedImage.Image;
			Close();
		}
		public void Close() {
			if(control.CloseCommand != null && control.CloseCommand.CanExecute(control.CloseCommandParameter))
				control.CloseCommand.Execute(control.CloseCommandParameter);
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
		public ICommand SelectAndCloseCommand {
			get {
				if(selectAndCloseCommand == null)
					selectAndCloseCommand = new WpfDelegateCommand<DesignTimeImagePickerImageViewModel>(SelectAndClose, false);
				return selectAndCloseCommand;
			}
		}
		public string FilterText {
			get { return filterText; }
			set { SetProperty(ref filterText, value, () => FilterText, () => Filter(Images)); }
		}
		public Thread FilterThread { get; private set; }
		void Filter(IList images, int imagesCount = -1) {
			if(imagesCount < 0)
				imagesCount = images.Count;
			string text = FilterText == null ? string.Empty : FilterText.ToLowerInvariant();
			for(int i = 0; i < imagesCount; ++i) {
				FilterImage(text, (DesignTimeImagePickerImageViewModel)images[i]);
			}
		}
		static void FilterImage(string text, DesignTimeImagePickerImageViewModel image) {
			image.IsVisibleCore = image.Image.Name.ToLowerInvariant().Contains(text);
		}
		void OnControlDataProviderChanged(object sender, ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier> e) {
			if(e.OldValue != null)
				e.OldValue.ImagesChanged -= OnDataProviderImagesChanged;
			if(e.NewValue != null)
				e.NewValue.ImagesChanged += OnDataProviderImagesChanged;
			UpdateImagesBase(isFirstWay);
			LoadData();
		}
		void OnControlGroupsChanged(object sender, ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>> e) {
			Categories.Items = control.Groups == null ? new DesignTimeImagePickerFilterItem[] { } : control.Groups.Select(g => new DesignTimeImagePickerFilterItem(g.Name) { IsSelected = true }).ToArray();
		}
		void OnDataProviderImagesChanged(object sender, EventArgs e) {
			UpdateImagesBase(false);
		}
		void UpdateImagesBase(bool newIsFirstWay) {
			if(isFirstWay)
				UpdateImages();
			else
				UpdateSelectedImage();
			isFirstWay = newIsFirstWay;
		}
		void UpdateSelectedImage() {
			string selectedImageString = SelectedImage == null ? null : SelectedImage.Image.Uri.OriginalString;
			if(string.Equals(selectedImageString, SelectedImageOriginalString)) return;
			if(SelectedImageOriginalString == null)
				UpdateImagesIsSelected(i => i.IsSelected, false);
			else
				UpdateImagesIsSelected(i => string.Equals(i.Image.Uri.OriginalString, SelectedImageOriginalString), true);
		}
		void UpdateImagesIsSelected(Func<DesignTimeImagePickerImageViewModel, bool> condition, bool isSelected) {
			foreach(var item in Images) {
				if(condition(item)) {
					item.IsSelected = isSelected;
					return;
				}
			}
		}
		void LoadData() {
			IDesignTimeImagePickerDataProvier dataProvider = control.DataProvider;
			if(dataProvider != null)
				dataProvider.Load();
		}
		void UpdateImages() {
			IDesignTimeImagePickerDataProvier dataProvider = control.DataProvider;
			IEnumerable<IDesignTimeImagePickerImageInfo> images = dataProvider == null ? null : dataProvider.Images;
			UpdateImagesThread = updateImagesAsync(images);
		}
		void SelectAndClose(DesignTimeImagePickerImageViewModel imageViewModel) {
			SelectedImage = imageViewModel;
			Done();
		}
		void OnPropertyNameChanged(object sender, EventArgs e) {
			if(PropertyName != null) {
				smallSize.IsSelected = !IsContainsPropertyName("large");
				largeSize.IsSelected = !IsContainsPropertyName("small");
			}
		}
		void OnSelectedImageOriginalStringChanged(object sender, EventArgs e) {
			SelectedImageOriginalString = control.SelectedImageOriginalString;
		}
		bool IsContainsPropertyName(string str) {
			return (PropertyName.ToLowerInvariant()).Contains(str.ToLowerInvariant());
		}
		DesignTimeImagePickerFilterItem GetCollectionByImageType(ImageType? imageType) {
			if(imageType == null) return localCollection;
			switch(imageType.Value) {
				case ImageType.Colored: return coloredCollection;
				case ImageType.GrayScaled: return grayscaleCollection;
				case ImageType.DevAV: return devAVCollection;
				case ImageType.Office2013: return officeCollection;
			}
			throw new NotImplementedException(imageType.Value.ToString());
		}
		IEnumerable<ManualResetEvent> UpdateImagesAsync(IEnumerable<IDesignTimeImagePickerImageInfo> images, CancellationToken cancellationToken) {
			List<ManualResetEvent> deferredOperations = new List<ManualResetEvent>();
			if(cancellationToken.IsCancellationRequested) return deferredOperations;
			AsyncHelper.DoEvents(dispatcher, DispatcherPriority.ApplicationIdle);
			deferredOperations.Add(AsyncHelper.DoWithDispatcher(dispatcher, () => Images.Clear()));
			if(images == null) return deferredOperations;
			if(cancellationToken.IsCancellationRequested) return deferredOperations;
			DesignTimeImagePickerGroupViewModel group = null;
			int imagesToAddLength = 1;
			if(cancellationToken.IsCancellationRequested) return deferredOperations;
			IEnumerator<IDesignTimeImagePickerImageInfo> imageEnumerator = images.GetEnumerator();
			if(cancellationToken.IsCancellationRequested) return deferredOperations;
			bool exit = false;
			bool isSelectedImage = false;
			while(!exit) {
				AsyncHelper.DoEvents(dispatcher, DispatcherPriority.ApplicationIdle);
				if(cancellationToken.IsCancellationRequested) return deferredOperations;
				DesignTimeImagePickerImageViewModel[] imagesToAdd = new DesignTimeImagePickerImageViewModel[imagesToAddLength];
				imagesToAddLength = (int)(1.2f * imagesToAddLength) + 1;
				int imagesToAddCount = 0;
				while(true) {
					if(!imageEnumerator.MoveNext()) {
						exit = true;
						break;
					}
					IDesignTimeImagePickerImageInfo image = imageEnumerator.Current;
					if(group == null || !string.Equals(group.Category.Name, image.Group, StringComparison.Ordinal))
						group = new DesignTimeImagePickerGroupViewModel(Categories.Items.Where(c => string.Equals(c.Name, image.Group, StringComparison.Ordinal)).First());
					DesignTimeImagePickerFilterItem collection = GetCollectionByImageType(image.ImageType);
					DesignTimeImagePickerFilterItem size = image.Size == ImageSize.Size16x16 ? smallSize : image.Size == ImageSize.Size32x32 ? largeSize : null;
					DesignTimeImagePickerImageViewModel imageViewModel = new DesignTimeImagePickerImageViewModel(image, group, this, new DesignTimeImagePickerFilterItem[] { collection, size });
					imageViewModel.BeginInit();
					imagesToAdd[imagesToAddCount] = imageViewModel;
					++imagesToAddCount;
					if(imagesToAddCount == imagesToAdd.Length) break;
				}
				deferredOperations.Add(AsyncHelper.DoWithDispatcher(dispatcher, () => {
					Filter(imagesToAdd, imagesToAddCount);
					for(int i = 0; i < imagesToAddCount; ++i) {
						imagesToAdd[i].EndInit();
						Images.Add(imagesToAdd[i]);
						if(SelectedImageOriginalString != null && !isSelectedImage) {
							isSelectedImage = imagesToAdd[i].Image.Uri.OriginalString == SelectedImageOriginalString;
							imagesToAdd[i].IsSelected = isSelectedImage;
						}
					}
				}, DispatcherPriority.ApplicationIdle));
			}
			return deferredOperations;
		}
	}
	public class DesignTimeImagePickerGroupViewModel : WpfBindableBase {
		bool isVisible = false;
		int visibleItemsCount = 0;
		public DesignTimeImagePickerGroupViewModel(DesignTimeImagePickerFilterItem category) {
			Category = category;
			Category.IsSelectedChanged += OnCategoryIsSelectedChanged;
		}
		public DesignTimeImagePickerFilterItem Category { get; private set; }
		public bool IsVisible {
			get { return isVisible; }
			set { SetProperty(ref isVisible, value, () => IsVisible); }
		}
		public void AddItem(bool itemIsVisible) {
			if(itemIsVisible) {
				++visibleItemsCount;
				UpdateIsVisible();
			}
		}
		public void RemoveItem(bool itemIsVisible) {
			if(itemIsVisible) {
				--visibleItemsCount;
				UpdateIsVisible();
			}
		}
		public void SetItemIsVisible(bool itemIsVisible) {
			if(itemIsVisible)
				++visibleItemsCount;
			else
				--visibleItemsCount;
			UpdateIsVisible();
		}
		void UpdateIsVisible() {
			IsVisible = (bool)Category.IsSelected && visibleItemsCount != 0;
		}
		void OnCategoryIsSelectedChanged(object sender, EventArgs e) { UpdateIsVisible(); }
	}
	public class DesignTimeImagePickerImageViewModel : BindableBase, ISupportInitialize {
		DesignTimeImagePickerGroupViewModel initialGroup;
		bool isVisibleCore = true;
		bool isVisible = true;
		readonly DesignTimeImagePickerFilterItem[] filters;
		bool isSelected;
		DesignTimeImagePickerGroupViewModel group;
		public DesignTimeImagePickerImageViewModel(IDesignTimeImagePickerImageInfo image, DesignTimeImagePickerGroupViewModel group, DesignTimeImagePickerMainViewModel selector, DesignTimeImagePickerFilterItem[] filters) {
			this.initialGroup = group;
			Selector = selector;
			Image = image;
			this.filters = filters;
			foreach(DesignTimeImagePickerFilterItem filter in filters) {
				if(filter == null) continue;
				filter.IsSelectedChanged += OnFilterIsSelectedChanged;
			}
			UpdateIsVisible();
		}
		public DesignTimeImagePickerMainViewModel Selector { get; private set; }
		public IDesignTimeImagePickerImageInfo Image { get; private set; }
		public DesignTimeImagePickerGroupViewModel Group {
			get { return group; }
			set {
				if(group == value) return;
				if(group != null)
					group.RemoveItem(IsVisible);
				group = value;
				if(group != null)
					group.AddItem(IsVisible);
			}
		}
		public bool IsVisibleCore {
			get { return isVisibleCore; }
			set { SetProperty(ref isVisibleCore, value, () => IsVisibleCore, UpdateIsVisible); }
		}
		public bool IsVisible {
			get {
				return isVisible;
			}
			private set { SetProperty(ref isVisible, value, () => IsVisible, UpdateGroup); }
		}
		public bool IsSelected {
			get { return isSelected; }
			set { SetProperty(ref isSelected, value, () => IsSelected, OnIsSelectedChanged); }
		}
		public void BeginInit() { }
		public void EndInit() {
			Group = initialGroup;
		}
		void OnIsSelectedChanged() {
			if(IsSelected)
				Selector.SelectedImage = this;
			else if(Selector.SelectedImage == this)
				Selector.SelectedImage = null;
		}
		void OnFilterIsSelectedChanged(object sender, EventArgs e) {
			UpdateIsVisible();
		}
		void UpdateGroup() {
			if(Group != null)
				Group.SetItemIsVisible(IsVisible);
		}
		void UpdateIsVisible() {
			if(!IsVisibleCore) {
				IsVisible = false;
				return;
			}
			foreach(DesignTimeImagePickerFilterItem filter in filters) {
				if(filter == null) continue;
				if(filter.IsSelected != null && !filter.IsSelected.Value) {
					IsVisible = false;
					return;
				}
			}
			IsVisible = true;
		}
	}
	public class DesignTimeImagePickerFilterItem : WpfBindableBase {
		bool? isSelected;
		WeakEventHandler<EventArgs, EventHandler> isSelectedChanged;
		public DesignTimeImagePickerFilterItem(string name) {
			Name = name;
		}
		public string Name { get; private set; }
		public bool? IsSelected {
			get { return isSelected; }
			set { SetProperty(ref isSelected, value, () => IsSelected, () => isSelectedChanged.SafeRaise(this, EventArgs.Empty)); }
		}
		public event EventHandler IsSelectedChanged { add { isSelectedChanged += value; } remove { isSelectedChanged -= value; } }
	}
	public class DesignTimeImagePickerFilterItemCollection : WpfBindableBase {
		IEnumerable<DesignTimeImagePickerFilterItem> items = new DesignTimeImagePickerFilterItem[] { };
		public DesignTimeImagePickerFilterItemCollection(string selectAllItemName) {
			SelectAllItem = new DesignTimeImagePickerFilterItem(selectAllItemName) { IsSelected = true };
			SelectAllItem.IsSelectedChanged += OnSelectAllItemIsSelectedChanged;
		}
		public DesignTimeImagePickerFilterItem SelectAllItem { get; private set; }
		public IEnumerable<DesignTimeImagePickerFilterItem> Items {
			get { return items; }
			set {
				IEnumerable<DesignTimeImagePickerFilterItem> oldValue = items;
				if(SetProperty(ref items, value, () => Items))
					OnItemsChanged(oldValue);
			}
		}
		protected virtual void OnItemsChanged(IEnumerable<DesignTimeImagePickerFilterItem> oldItems) {
			foreach(DesignTimeImagePickerFilterItem item in oldItems)
				item.IsSelectedChanged -= OnItemIsSelectedChanged;
			foreach(DesignTimeImagePickerFilterItem item in Items)
				item.IsSelectedChanged += OnItemIsSelectedChanged;
			SelectAllItem.IsSelected = CalcSelectAllItemIsSelected();
			this.RaisePropertyChanged(() => ItemsWithSelectAll);
		}
		void OnItemIsSelectedChanged(object sender, EventArgs e) {
			SelectAllItem.IsSelected = CalcSelectAllItemIsSelected();
		}
		public IEnumerable<DesignTimeImagePickerFilterItem> ItemsWithSelectAll {
			get {
				yield return SelectAllItem;
				foreach(DesignTimeImagePickerFilterItem item in Items)
					yield return item;
			}
		}
		bool? CalcSelectAllItemIsSelected() {
			bool selectedItemsExist = false;
			bool unselectedItemsExist = false;
			foreach(DesignTimeImagePickerFilterItem item in Items) {
				DebugHelper.Assert(item.IsSelected != null);
				if(item.IsSelected == null) continue;
				if(item.IsSelected.Value)
					selectedItemsExist = true;
				else
					unselectedItemsExist = true;
				if(selectedItemsExist && unselectedItemsExist) return null;
			}
			return !unselectedItemsExist;
		}
		void OnSelectAllItemIsSelectedChanged(object sender, EventArgs e) {
			bool? isSelected = SelectAllItem.IsSelected;
			if(isSelected == null) return;
			foreach(var item in Items) {
				item.IsSelected = isSelected;
			}
		}
	}
	public interface IDesignTimeImagePicker {
		IDesignTimeImagePickerDataProvier DataProvider { get; }
		IEnumerable<IDesignTimeImagePickerGroupInfo> Groups { get; }
		IDesignTimeImagePickerImageInfo SelectedImage { get; set; }
		string PropertyName { get; set; }
		string SelectedImageOriginalString { get; set; }
		ICommand CloseCommand { get; }
		object CloseCommandParameter { get; }
		event EventHandler<ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier>> DataProviderChanged;
		event EventHandler<ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>>> GroupsChanged;
		event EventHandler PropertyNameChanged;
		event EventHandler SelectedImageOriginalStringChanged;
	}
}
