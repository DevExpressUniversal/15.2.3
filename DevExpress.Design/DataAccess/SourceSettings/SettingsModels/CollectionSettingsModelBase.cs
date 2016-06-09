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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.ComponentModel;
	using DevExpress.Design.ComponentModel;
	abstract class CollectionSettingsModelBase : SortingSettingsModel {
		public CollectionSettingsModelBase(IDataSourceInfo info)
			: base(info) {
			GroupDescriptions = new PropertyGroupDescriptionCollection();
			AddGroupCommand = new Design.UI.WpfDelegateCommand<string>(AddGroup, CanAddGroup);
			DeleteGroupCommand = new Design.UI.WpfDelegateCommand<GroupDescription>(DeleteGroup, CanDeleteGroup);
		}
		protected override void OnSelectedElementChanged() {
			GroupDescriptions.Clear();
			base.OnSelectedElementChanged();
		}
		bool isSynchronizedWithCurrentItemCore = true;
		public bool IsSynchronizedWithCurrentItem {
			get { return isSynchronizedWithCurrentItemCore; }
			set { SetProperty(ref isSynchronizedWithCurrentItemCore, value, "IsSynchronizedWithCurrentItem"); }
		}
		public bool AllowSortingOrGrouping {
			get { return AllowSorting || AllowGrouping; }
		}
		bool allowGroupCore;
		public bool AllowGrouping {
			get { return allowGroupCore; }
			set { SetProperty(ref allowGroupCore, value, "AllowGrouping", OnAllowGroupingChanged); }
		}
		bool allowPagingCore;
		public bool AllowPaging {
			get { return allowPagingCore; }
			set { SetProperty(ref allowPagingCore, value, "AllowPaging"); }
		}
		public PropertyGroupDescriptionCollection GroupDescriptions {
			get;
			private set;
		}
		GroupDescription groupDescriptionCore;
		public GroupDescription GroupDescription {
			get { return groupDescriptionCore; }
			set { SetProperty(ref groupDescriptionCore, value, "GroupDescription"); }
		}
		string groupFieldCore;
		public string GroupField {
			get { return groupFieldCore; }
			set { SetProperty(ref groupFieldCore, value, "GroupField"); }
		}
		int pageSizeCore;
		public int PageSize {
			get { return pageSizeCore; }
			set { SetProperty(ref pageSizeCore, Math.Max(0, value), "PageSize"); }
		}
		#region Commands
		public Design.UI.ICommand<string> AddGroupCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<GroupDescription> DeleteGroupCommand {
			get;
			private set;
		}
		bool CanAddGroup(string propertyName) {
			return AllowGrouping && !string.IsNullOrEmpty(propertyName) && !GroupDescriptions.Contains(propertyName);
		}
		void AddGroup(string propertyName) {
			GroupDescriptions.Add(propertyName);
			if(!SortDescriptions.Contains(propertyName) && AllowSorting)
				SortDescriptions.Add(new PropertySortDescription(propertyName, ListSortDirection.Ascending));
		}
		bool CanDeleteGroup(GroupDescription description) {
			return description != null;
		}
		void DeleteGroup(GroupDescription description) {
			GroupDescriptions.Remove((System.Windows.Data.PropertyGroupDescription)description);
		}
		protected override void DeleteSort(PropertySortDescription description) {
			SortDescriptions.Remove(description);
			GroupDescriptions.Remove(description.PropertyName);
		}
		#endregion Commands
		protected override void OnAllowSortingChanged() {
			base.OnAllowSortingChanged();
			RaisePropertyChanged("AllowSortingOrGrouping");
		}
		protected virtual void OnAllowGroupingChanged() {
			RaisePropertyChanged("AllowSortingOrGrouping");
		}
	}
}
