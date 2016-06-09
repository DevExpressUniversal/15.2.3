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
using System.Windows;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Data;
using DevExpress.Mvvm;
namespace DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard.Templates {
	public class DomainDataSourceConfigurationViewModel : ConfigurationViewModelBase {
		IList<ListSortDirection> directions;
		public DomainDataSourceConfigurationViewModel(List<DataTable> tables) : base(tables) {
			SetProperties();
		}
		#region static
		public static readonly DependencyProperty GroupDescriptionsProperty;
		public static readonly DependencyProperty SortDescriptionsProperty;
		static DomainDataSourceConfigurationViewModel() {
			Type ownerType = typeof(DomainDataSourceConfigurationViewModel);
			GroupDescriptionsProperty =
			DependencyProperty.Register("GroupDescriptions", typeof(ObservableCollection<GroupDescription>), ownerType, new FrameworkPropertyMetadata());
			SortDescriptionsProperty =
			DependencyProperty.Register("SortDescriptions", typeof(SortDescriptionCollection), ownerType, new FrameworkPropertyMetadata());
		}
		#endregion
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
		public bool AutoLoad { get; set; }
		public int LoadDelay { get; set; }
		public int LoadInterval { get; set; }
		public int LoadSize { get; set; }
		public int RefreshInterval { get; set; }
		public bool IsSynchronizedWithCurrentItem { get; set; }
		public ListSortDirection SelectedSortDirection { get; set; }
		public IList<ListSortDirection> Directions { get { return directions; } }
		public ICommand DeleteGroup { get; private set; }
		public ICommand DeleteSort { get; private set; }
		public ICommand AddGroup { get; private set; }
		public ICommand AddSort { get; private set; }
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
		public bool CanDeleteGroup() {
			return SelectedGroupDescription >= 0;
		}
		public bool CanDeleteSort() {
			return SelectedSortDescription >= 0;
		}
		private void SetProperties() {
			directions = new List<ListSortDirection>() { ListSortDirection.Ascending, ListSortDirection.Descending };
			GroupDescriptions = new ObservableCollection<GroupDescription>();
			SortDescriptions = new SortDescriptionCollection();
			IsSynchronizedWithCurrentItem = true;
			AutoLoad = true;
			LoadSize = 1000;
			DeleteGroup = new DelegateCommand(OnDeleteGroup, CanDeleteGroup);
			DeleteSort = new DelegateCommand(OnDeleteSort, CanDeleteSort);
			AddGroup = new DelegateCommand(OnAddGroup);
			AddSort = new DelegateCommand(OnAddSort);
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
			if(string.IsNullOrEmpty(SelectedGroupFieldName) || ContainsGroupDescription(SelectedGroupFieldName))
				return;
			GroupDescriptions.Add(new PropertyGroupDescription(SelectedGroupFieldName));
			if(!ContainsSortDescription(SelectedGroupFieldName))
				SortDescriptions.Add(new SortDescription(SelectedGroupFieldName, ListSortDirection.Ascending));
		}
		private void OnAddSort() {
			if(string.IsNullOrEmpty(SelectedSortFieldName) || ContainsSortDescription(SelectedSortFieldName))
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
