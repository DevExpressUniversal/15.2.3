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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using System.Collections.ObjectModel;
using DevExpress.Data.XtraReports.DataProviders;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Data.Browsing;
using System.ComponentModel;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.XtraReports.Wizard;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class SelectColumnsPage : ReportWizardPageBase {
		readonly ColumnInfoCache columnsCache = new ColumnInfoCache();
		public static SelectColumnsPage Create(ReportWizardModel model) {
			return ViewModelSource.Create(() => new SelectColumnsPage(model));
		}
		readonly ObservableCollection<ColumnInfo> availableColumns = new ObservableCollection<ColumnInfo>();
		readonly ObservableCollection<ColumnInfo> selectedColumns = new ObservableCollection<ColumnInfo>();
		public ObservableCollection<ColumnInfo> AvailableColumns {
			get { return availableColumns; }
		}
		public ObservableCollection<ColumnInfo> SelectedColumns {
			get { return selectedColumns; }
		}
		public ICommand AddCommand { get; private set; }
		public ICommand AddAllCommand { get; private set; }
		public ICommand RemoveCommand { get; private set; }
		public ICommand RemoveAllCommand { get; private set; }
		#region ctor & initialization
		public SelectColumnsPage(ReportWizardModel model) : base(model) {
			InitializeCommands();
			FillColumns();
			SelectedColumns.CollectionChanged += (d, e) => {
				ReportModel.Columns = SelectedColumns.Select(x => x.Name).ToArray();
				columnsCache.Columns = SelectedColumns.ToArray();
			};
		}
		void FillColumns() {
			DataContextBase dataContext = new DataContextBase();
			PropertyDescriptorCollection properties = dataContext[ReportModel.DataSchema].GetItemProperties();
			TypeSpecificsService typeSpecificsService = new TypeSpecificsService();
			var columnsList = new List<ColumnInfo>();
			foreach (PropertyDescriptor property in properties) {
				columnsList.Add(new ColumnInfo() { Name = property.Name, DisplayName = property.DisplayName, TypeSpecifics = typeSpecificsService.GetPropertyTypeSpecifics(property) });
			}
			ReportModel.Columns.Do(columns => {
				columnsList.Where(x => columns.Contains(x.Name))
					.ForEach(x => SelectedColumns.Add(x));
			});
			columnsCache.Columns = SelectedColumns.ToArray();
			columnsList.Except(SelectedColumns)
				.ForEach(x => AvailableColumns.Add(x));
		}
		void InitializeCommands() {
			AddCommand = DelegateCommandFactory.Create<ColumnInfo>(Add, (column) => column != null && availableColumns.Contains(column));
			AddAllCommand = DelegateCommandFactory.Create(AddAll, () => availableColumns.Count > 0);
			RemoveCommand = DelegateCommandFactory.Create<ColumnInfo>(Remove, (column) => column != null && selectedColumns.Contains(column));
			RemoveAllCommand = DelegateCommandFactory.Create(RemoveAll, () => selectedColumns.Count > 0);
		}
		#endregion
		void Add(ColumnInfo column) {
			AvailableColumns.Remove(column);
			SelectedColumns.Add(column);
		}
		void AddAll() {
			var columns = AvailableColumns.ToArray();
			columns.ForEach(x => {
				Add(x);
			});
		}
		void Remove(ColumnInfo column) {
			SelectedColumns.Remove(column);
			AvailableColumns.Add(column);
		}
		void RemoveAll() {
			var columns = SelectedColumns.ToArray();
			columns.ForEach(x => {
				Remove(x);
			});
		}
		#region overrides
		public override bool CanFinish {
			get {
				return SelectedColumns.Count > 0;
			}
		}
		public override bool CanGoForward {
			get {
				return SelectedColumns.Count > 0;
			}
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			wizardController.NavigateTo(AddGroupingLevelPage.Create(ReportWizardModel, columnsCache));
		}
		#endregion
	}
}
