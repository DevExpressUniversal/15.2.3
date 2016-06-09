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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates {
	public class CollectionViewConfigurationViewModel : ConfigurationViewModelBase {
		IList<ListSortDirection> directions;
		static List<string> NoSortingCollectionViewTypes = new List<string>() { "BindingListCollectionView", "CollectionView" };
		static List<string> NoGroupingCollectionViewTypes = new List<string>() { "CollectionView" };
		#region static
		public static readonly DependencyProperty GroupDescriptionsProperty;
		public static readonly DependencyProperty SortDescriptionsProperty;
		public static readonly DependencyProperty SelectedCultureProperty;
		public static readonly DependencyProperty SelectedCollectionViewTypeProperty;
		public static readonly DependencyProperty IsCollectionViewCanSortProperty;
		public static readonly DependencyProperty IsCollectionViewCanGroupProperty;
		public static readonly DependencyProperty IsPageSizeEnableProperty;
		static CollectionViewConfigurationViewModel() {
			GroupDescriptionsProperty =
			DependencyProperty.Register("GroupDescriptions", typeof(ObservableCollection<GroupDescription>), typeof(CollectionViewConfigurationViewModel), new FrameworkPropertyMetadata());
			SortDescriptionsProperty =
			DependencyProperty.Register("SortDescriptions", typeof(SortDescriptionCollection), typeof(CollectionViewConfigurationViewModel), new FrameworkPropertyMetadata());
			SelectedCultureProperty =
			DependencyProperty.Register("SelectedCulture", typeof(string), typeof(CollectionViewConfigurationViewModel), new FrameworkPropertyMetadata());
			SelectedCollectionViewTypeProperty =
			DependencyProperty.Register("SelectedCollectionViewType", typeof(Type), typeof(CollectionViewConfigurationViewModel),
			new FrameworkPropertyMetadata((o, e) => ((CollectionViewConfigurationViewModel)o).OnSelectedCollectionViewTypeChanged()));
			IsCollectionViewCanSortProperty =
			DependencyProperty.Register("IsCollectionViewCanSort", typeof(bool), typeof(CollectionViewConfigurationViewModel), new FrameworkPropertyMetadata(true));
			IsCollectionViewCanGroupProperty =
			DependencyProperty.Register("IsCollectionViewCanGroup", typeof(bool), typeof(CollectionViewConfigurationViewModel), new FrameworkPropertyMetadata(true));
			IsPageSizeEnableProperty =
			DependencyProperty.Register("IsPageSizeEnable", typeof(bool), typeof(CollectionViewConfigurationViewModel), new FrameworkPropertyMetadata(false));
		}
		#endregion
		public CollectionViewConfigurationViewModel(List<DataTable> tables)
			: base(tables) {
			SetProperties();
		}
		public CollectionViewConfigurationViewModel(List<DataTable> tables, List<Type> collectionViewTypes)
			: this(tables) {
			this.CollectionViewTypes = new ObservableCollection<Type>(collectionViewTypes);
		}
		public string SelectedCulture {
			get { return (string)GetValue(SelectedCultureProperty); }
			set { SetValue(SelectedCultureProperty, value); }
		}
		public bool IsCollectionViewCanSort {
			get { return (bool)GetValue(IsCollectionViewCanSortProperty); }
			set { SetValue(IsCollectionViewCanSortProperty, value); }
		}
		public bool IsCollectionViewCanGroup {
			get { return (bool)GetValue(IsCollectionViewCanGroupProperty); }
			set { SetValue(IsCollectionViewCanGroupProperty, value); }
		}
		public bool IsPageSizeEnable {
			get { return (bool)GetValue(IsPageSizeEnableProperty); }
			set { SetValue(IsPageSizeEnableProperty, value); }
		}
		public Type SelectedCollectionViewType {
			get { return (Type)GetValue(SelectedCollectionViewTypeProperty); }
			set { SetValue(SelectedCollectionViewTypeProperty, value); }
		}
		public ObservableCollection<GroupDescription> GroupDescriptions {
			get { return (ObservableCollection<GroupDescription>)GetValue(GroupDescriptionsProperty); }
			set { SetValue(GroupDescriptionsProperty, value); }
		}
		public SortDescriptionCollection SortDescriptions {
			get { return (SortDescriptionCollection)GetValue(SortDescriptionsProperty); }
			set { SetValue(SortDescriptionsProperty, value); }
		}
		public string SelectedSortFieldName { get; set; }
		public string SelectedGroupFieldName { get; set; }
		public int SelectedGroupDescription { get; set; }
		public int SelectedSortDescription { get; set; }
		public int PageSize { get; set; }
		public bool IsSynchronizedWithCurrentItem { get; set; }
		public ListSortDirection SelectedSortDirection { get; set; }
		public IList<ListSortDirection> Directions { get { return directions; } }
		public ObservableCollection<string> Cultures { get; private set; }
		public ObservableCollection<Type> CollectionViewTypes { get; private set; }
		public ICommand DeleteGroup { get; private set; }
		public ICommand DeleteSort { get; private set; }
		public ICommand AddGroup { get; private set; }
		public ICommand AddSort { get; private set; }
		public bool CanDeleteGroup() {
			return SelectedGroupDescription >= 0;
		}
		public bool CanDeleteSort() {
			return SelectedSortDescription >= 0;
		}
		public override void OnSelectedTableChanged() {
			base.OnSelectedTableChanged();
			SortDescriptions = new SortDescriptionCollection();
			GroupDescriptions = new ObservableCollection<GroupDescription>();
		}
		public override void OnTypeNameChanged() {
			base.OnTypeNameChanged();
			SortDescriptions = new SortDescriptionCollection();
			GroupDescriptions = new ObservableCollection<GroupDescription>();
		}
		private void OnSelectedCollectionViewTypeChanged() {
			IsPageSizeEnable = SelectedCollectionViewType.Name == "PagedCollectionView";
			IsCollectionViewCanSort = NoSortingCollectionViewTypes.Contains(SelectedCollectionViewType.Name) ? false : true;
			IsCollectionViewCanGroup = NoGroupingCollectionViewTypes.Contains(SelectedCollectionViewType.Name) ? false : true;
		}
		private void SetProperties() {
			directions = new List<ListSortDirection>() { ListSortDirection.Ascending, ListSortDirection.Descending };
			Cultures = GetCultures();
			GroupDescriptions = new ObservableCollection<GroupDescription>();
			SortDescriptions = new SortDescriptionCollection();
			IsSynchronizedWithCurrentItem = true;
			DeleteGroup = new DelegateCommand(OnDeleteGroup, CanDeleteGroup);
			DeleteSort = new DelegateCommand(OnDeleteSort, CanDeleteSort);
			AddGroup = new DelegateCommand(OnAddGroup);
			AddSort = new DelegateCommand(OnAddSort);
		}
		private CultureInfo GetCurrentCulture() {
			return CultureInfo.CurrentCulture;
		}
		private ObservableCollection<Type> GetCollectionViewTypes() {
			ObservableCollection<Type> types = new ObservableCollection<Type>() { typeof(ListCollectionView), typeof(BindingListCollectionView) };
			return types;
		}
		private ObservableCollection<string> GetCultures() {
			ObservableCollection<string> cultures = new ObservableCollection<string> { "" };
			foreach(CultureInfo cInfo in CultureInfo.GetCultures(CultureTypes.AllCultures)) {
				cultures.Add(cInfo.Name);
			}
			return cultures;
		}
		private void OnDeleteGroup() {
			if(SelectedGroupDescription >= 0 && GroupDescriptions.Count > 0)
				GroupDescriptions.RemoveAt(SelectedGroupDescription);
		}
		private void OnDeleteSort() {
			if(SelectedSortDescription >= 0 && SortDescriptions.Count > 0) {
				GroupDescriptions.Remove(FindGroupDescription(SelectedSortDescription));
				SortDescriptions.RemoveAt(SelectedSortDescription);
			}
		}
		private void OnAddGroup() {
			if(string.IsNullOrEmpty(SelectedGroupFieldName) || ContainsGroupDescription(SelectedGroupFieldName) || !IsCollectionViewCanGroup)
				return;
			GroupDescriptions.Add(new PropertyGroupDescription(SelectedGroupFieldName));
			if(!ContainsSortDescription(SelectedGroupFieldName) && IsCollectionViewCanSort)
				SortDescriptions.Add(new SortDescription(SelectedGroupFieldName, ListSortDirection.Ascending));
		}
		private void OnAddSort() {
			if(string.IsNullOrEmpty(SelectedSortFieldName) || ContainsSortDescription(SelectedSortFieldName) || !IsCollectionViewCanSort)
				return;
			SortDescriptions.Add(new SortDescription(SelectedSortFieldName, SelectedSortDirection));
		}
		private bool ContainsGroupDescription(string fieldName) {
			foreach(PropertyGroupDescription desc in GroupDescriptions)
				if(desc.PropertyName == fieldName)
					return true;
			return false;
		}
		private PropertyGroupDescription FindGroupDescription(int index) {
			string name = SortDescriptions[SelectedSortDescription].PropertyName;
			foreach(PropertyGroupDescription desc in GroupDescriptions)
				if(desc.PropertyName == name)
					return desc;
			return null;
		}
		private bool ContainsSortDescription(string fieldName) {
			foreach(SortDescription desc in SortDescriptions)
				if(desc.PropertyName == fieldName)
					return true;
			return false;
		}
	}
}
