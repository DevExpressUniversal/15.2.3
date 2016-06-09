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
	abstract class DefaultSortingSettingsModel : DataSourceSettingsModelBase, IDefaultSortingSettingsModel {
		public DefaultSortingSettingsModel(IDataSourceInfo info)
			: base(info) {
				ResetDefaultSortingCommand = new Design.UI.WpfDelegateCommand<string>(ResetDefaultSorting, CanResetDefaultSorting);
		}
		#region Properties
		string sortFieldCore;
		public string SortField {
			get { return sortFieldCore; }
			set { SetProperty(ref sortFieldCore, value, "SortField", OnSortFieldChanged); }
		}
		ListSortDirection sortDirectionCore;
		public ListSortDirection SortDirection {
			get { return sortDirectionCore; }
			set { SetProperty(ref sortDirectionCore, value, "SortDirection", OnSortDirectionChanged); }
		}
		bool isDefaultSortingAvailableCore;
		public bool IsDefaultSortingAvailable {
			get { return isDefaultSortingAvailableCore; }
			private set { SetProperty(ref isDefaultSortingAvailableCore, value, "IsDefaultSortingAvailable"); }
		}
		string defaultSortingCore;
		public string DefaultSorting {
			get { return defaultSortingCore; }
			private set { SetProperty(ref defaultSortingCore, value, "DefaultSorting"); }
		}
		#endregion Properties
		#region Commands
		public Design.UI.ICommand<string> ResetDefaultSortingCommand {
			get;
			private set;
		}
		bool CanResetDefaultSorting(string sortField) {
			return !string.IsNullOrEmpty(sortField);
		}
		void ResetDefaultSorting(string sortField) {
			SortField = null;
		}
		#endregion Commands
		protected virtual void OnSortFieldChanged() {
			UpdateDefaultSorting();
		}
		protected virtual void OnSortDirectionChanged() {
			UpdateDefaultSorting();
		}
		protected void UpdateDefaultSorting() {
			IsDefaultSortingAvailable = !string.IsNullOrEmpty(SortField);
			DefaultSorting = IsDefaultSortingAvailable ? GetDefaultSorting() : null;
		}
		protected virtual string GetDefaultSorting() {
			return SortField + " " + SortDirection.ToSortString();
		}
	}
}
