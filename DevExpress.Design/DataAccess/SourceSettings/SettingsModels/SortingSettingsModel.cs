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
	using System.ComponentModel;
	using DevExpress.Design.ComponentModel;
	abstract class SortingSettingsModel : DataSourceSettingsModelBase, ISortingSettingsModel {
		public SortingSettingsModel(IDataSourceInfo info)
			: base(info) {
			SortDescriptions = new PropertySortDescriptionCollection();
			AddSortCommand = new Design.UI.WpfDelegateCommand<string>(AddSort, CanAddSort);
			DeleteSortCommand = new Design.UI.WpfDelegateCommand<PropertySortDescription>(DeleteSort, CanDeleteSort);
			InvertSortDirectionCommand = new Design.UI.WpfDelegateCommand<PropertySortDescription>(InvertSortDirection, CanInvertSortDirection);
		}
		protected override void OnSelectedElementChanged() {
			SortDescriptions.Clear();
			UpdateIsSortingAvailable();
			base.OnSelectedElementChanged();
		}
		bool allowSortCore;
		public bool AllowSorting {
			get { return allowSortCore; }
			set { SetProperty(ref allowSortCore, value, "AllowSorting", OnAllowSortingChanged); }
		}
		public PropertySortDescriptionCollection SortDescriptions {
			get;
			private set;
		}
		PropertySortDescription sortDescriptionCore;
		public PropertySortDescription SortDescription {
			get { return sortDescriptionCore; }
			set { SetProperty(ref sortDescriptionCore, value, "SortDescription"); }
		}
		bool isSortingAvailableCore;
		public bool IsSortingAvailable {
			get { return isSortingAvailableCore; }
			private set { SetProperty(ref isSortingAvailableCore, value, "IsSortingAvailable"); }
		}
		string sortFieldCore;
		public string SortField {
			get { return sortFieldCore; }
			set { SetProperty(ref sortFieldCore, value, "SortField", OnSortFieldChanged); }
		}
		ListSortDirection sortDirectionCore;
		public ListSortDirection SortDirection {
			get { return sortDirectionCore; }
			set { SetProperty(ref sortDirectionCore, value, "SortDirection"); }
		}
		#region Commands
		public Design.UI.ICommand<string> AddSortCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<PropertySortDescription> DeleteSortCommand {
			get;
			private set;
		}
		public Design.UI.ICommand<PropertySortDescription> InvertSortDirectionCommand {
			get;
			private set;
		}
		bool CanAddSort(string propertyName) {
			return AllowSorting && IsSortingAvailable && !SortDescriptions.Contains(propertyName);
		}
		void AddSort(string propertyName) {
			SortDescriptions.Add(new PropertySortDescription(propertyName, SortDirection));
		}
		bool CanDeleteSort(PropertySortDescription description) {
			return description != null;
		}
		protected virtual void DeleteSort(PropertySortDescription description) {
			SortDescriptions.Remove(description);
		}
		bool CanInvertSortDirection(PropertySortDescription description) {
			return description != null;
		}
		void InvertSortDirection(PropertySortDescription description) {
			SortDescriptions.InvertDirection(description);
		}
		#endregion Commands
		protected virtual void OnAllowSortingChanged() { }
		protected virtual void OnSortFieldChanged() {
			UpdateIsSortingAvailable();
		}
		protected void UpdateIsSortingAvailable() {
			IsSortingAvailable = !string.IsNullOrEmpty(SortField);
		}
	}
}
