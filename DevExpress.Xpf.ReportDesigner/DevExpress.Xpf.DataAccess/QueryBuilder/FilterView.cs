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
using DevExpress.Data;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Filtering;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class FilterView : IFilterView {
		readonly Action<Action<IDialogService>> doWithFilterDialogService;
		protected FilterView(Action<Action<IDialogService>> doWithFilterDialogService) {
			this.doWithFilterDialogService = doWithFilterDialogService;
			this.additionalProperties = new PropertyInfoCollection();
		}
		IFilterPresenter presenter;
		readonly PropertyInfoCollection additionalProperties;
		public PropertyInfoCollection AdditionalProperties { get { return additionalProperties; } }
		#region IFilterView
		public string FilterString { get; set; }
		void IFilterView.SetColumnsInfo(IEnumerable<FilterViewTableInfo> info) {
			foreach(var table in info)
				foreach(var column in table.Columns)
					additionalProperties.Add(new PropertyInfo(column.ColumnName, null, column.ColumnType));
		}
		void IFilterView.SetExistingParameters(IEnumerable<IParameter> parameters) { }
		#endregion
		#region IView<IFilterPresenter>
		void IView<IFilterPresenter>.BeginUpdate() { }
		EventHandler cancel;
		event EventHandler IView<IFilterPresenter>.Cancel {
			add { cancel += value; }
			remove { cancel -= value; }
		}
		void IView<IFilterPresenter>.EndUpdate() { }
		EventHandler ok;
		event EventHandler IView<IFilterPresenter>.Ok {
			add { ok += value; }
			remove { ok -= value; }
		}
		void IView<IFilterPresenter>.RegisterPresenter(IFilterPresenter presenter) {
			this.presenter = presenter;
		}
		void IView<IFilterPresenter>.Start() {
			this.doWithFilterDialogService(dialog => {
				if(dialog.ShowDialog(MessageButton.OKCancel, DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView), this) == MessageResult.OK)
					ok.Do(x => x(this, EventArgs.Empty));
				else
					cancel.Do(x => x(this, EventArgs.Empty));
			});
		}
		void IView<IFilterPresenter>.Stop() { }
		void IView<IFilterPresenter>.Warning(string message) { }
		#endregion
	}
}
