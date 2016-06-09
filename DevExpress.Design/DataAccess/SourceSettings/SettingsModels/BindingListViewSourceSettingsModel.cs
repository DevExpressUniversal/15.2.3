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
	class BindingListViewSourceSettingsModel : SortingSettingsModel, IBindingListViewSourceSettingsModel {
		public BindingListViewSourceSettingsModel(IDataSourceInfo info)
			: base(info) {
			AllowSorting = true;
			ResetFilterCommand = new Design.UI.WpfDelegateCommand<string>(ResetFilter, CanResetFilter);
		}
		protected sealed override System.Type GetKey() {
			return typeof(IBindingListViewSourceSettingsModel);
		}
		#region Properties
		string filterCore;
		public string Filter {
			get { return filterCore; }
			set { SetProperty(ref filterCore, value, "Filter", OnFilterChanged); }
		}
		#endregion Properties
		#region Commands
		public Design.UI.ICommand<string> ResetFilterCommand {
			get;
			private set;
		}
		bool CanResetFilter(string filter) {
			return !string.IsNullOrEmpty(filter);
		}
		void ResetFilter(string filter) {
			Filter = null;
		}
		#endregion Commands
		protected virtual void OnFilterChanged() { }
	}
	class ExcelBindingListViewSourceSettingsModel : BindingListViewSourceSettingsModel {
		public ExcelBindingListViewSourceSettingsModel(IDataSourceInfo info)
			: base(info) {
			this.excelPathCore = ((IExcelDataSourceInfo)info).ExcelPath;
		}
		string excelPathCore;
		public string ExcelPath {
			get { return excelPathCore; }
		}
		protected override void RegisterValidationRules() { }
	}
}
