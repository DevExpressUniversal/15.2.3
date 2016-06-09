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
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.Native;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class SelectHierarchicalDataSourceColumnsPage<TModel> : WizardPageBase<ISelectHierarchicalDataSourceColumnsPageView, TModel> where TModel : ReportModel {
		#region Fields & Properties
		readonly PickManagerBase pickManager;
		readonly List<ColumnInfo> selectedColumns = new List<ColumnInfo>();
		readonly IColumnInfoCache columnInfoCache;
		public override bool MoveNextEnabled { get { return selectedColumns.Count > 0; } }
		public override bool FinishEnabled { get { return MoveNextEnabled; } }
		#endregion
		#region ctor
		public SelectHierarchicalDataSourceColumnsPage(ISelectHierarchicalDataSourceColumnsPageView view, PickManagerBase pickManager, IColumnInfoCache columnInfoCache)
			: base(view)  {
			Guard.ArgumentNotNull(pickManager, "pickManager");
			Guard.ArgumentNotNull(columnInfoCache, "columnInfoCache");
			this.pickManager = pickManager;
			this.columnInfoCache = columnInfoCache;
			view.ActiveAvailableColumnChanged += view_ActiveAvailableColumnChanged;
			view.ActiveSelectedColumnsChanged += view_ActiveSelectedColumnsChanged;
			view.AddColumnClicked += view_AddColumnClicked;
			view.RemoveColumnsClicked += view_RemoveColumnsClicked;
			view.RemoveAllColumnsClicked += view_RemoveAllColumnsClicked;
		}
		#endregion
		#region Methods
		void view_ActiveAvailableColumnChanged(object sender, EventArgs e) {
			RefreshButtons();
		}
		void view_ActiveSelectedColumnsChanged(object sender, EventArgs e) {
			RefreshButtons();
		}
		private void RefreshButtons() {
			View.EnableAddColumnButton(CanAddDataMemberNode(View.ActiveAvailableDataMemberNode));
			View.EnableRemoveColumnsButton(View.ActiveSelectedColumns.Length > 0);
			View.EnableRemoveAllColumnsButton(selectedColumns.Count > 0);
		}
		void RefreshView() {
			View.FillSelectedColumns(selectedColumns.ToArray());
			RefreshButtons();
			RaiseChanged();
		}
		void view_RemoveAllColumnsClicked(object sender, EventArgs e) {
			selectedColumns.Clear();
			RefreshView();
		}
		void view_RemoveColumnsClicked(object sender, EventArgs e) {
			foreach(var columnInfo in View.ActiveSelectedColumns) {
				selectedColumns.Remove(columnInfo);
			}
			RefreshView();
		}
		void view_AddColumnClicked(object sender, EventArgs e) {
			var node = View.ActiveAvailableDataMemberNode;
			if(!CanAddDataMemberNode(node)) 
				return;
			var columnInfo = new ColumnInfo() {
				Name = node.DataMember,
				DisplayName = string.Format("{0} ({1})", node.PropertyDescriptor.DisplayName, node.DataMember),
				TypeSpecifics = node.PropertyDescriptor.Specifics
			};
			selectedColumns.Add(columnInfo);
			RefreshView();
		}
		public override void Begin() {
			View.ShowWaitIndicator(true);
			pickManager.FillContent(View.RootNodes, new string[] { Model.DataSourceName }, false);
			pickManager.Executor.AddAction(pickManager_FillContentCompleted);
			RefreshButtons();
		}
		public override void Commit() {
			Model.Columns = selectedColumns.Select(c => c.Name).ToArray();
			columnInfoCache.Columns = selectedColumns.ToArray();
		}
		public override Type GetNextPageType() {
			return typeof(AddGroupingLevelPage<TModel>);
		}
		void pickManager_FillContentCompleted() {
			View.ShowWaitIndicator(false);
			RefreshButtons();
		}
		bool CanAddDataMemberNode(IPickManagerDataMemberNode node) {
			return
				node != null &&
				!node.PropertyDescriptor.IsComplex &&
				!selectedColumns.Any(x => x.Name == node.DataMember);
		}
		#endregion
	}
}
