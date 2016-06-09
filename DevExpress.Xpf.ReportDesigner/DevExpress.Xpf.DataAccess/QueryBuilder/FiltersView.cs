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
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Editors.Filtering;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	[POCOViewModel]
	public class FiltersView : FilterView, IFiltersView {
		public static FiltersView Create(Action<Action<IDialogService>> doWithFilterDialogService) {
			return ViewModelSource.Create(() => new FiltersView(doWithFilterDialogService));
		}
		protected FiltersView(Action<Action<IDialogService>> doWithFilterDialogService)
			: base(doWithFilterDialogService) {
			this.doWithFilterDialogService = doWithFilterDialogService;
			this.groupFilterAdditionalProperties = new PropertyInfoCollection();
		}
		readonly Action<Action<IDialogService>> doWithFilterDialogService;
		readonly PropertyInfoCollection groupFilterAdditionalProperties;
		public PropertyInfoCollection GroupFilterAdditionalProperties { get { return groupFilterAdditionalProperties; } }
		#region IFiltersView
		public bool GroupFilterEnabled { get; set; }
		public string GroupFilterString { get; set; }
		public virtual bool RecordsLimitationEnabled { get; set; }
		protected void OnRecordsLimitationEnabledChanged() {
			recordsLimitationChanged.Do(x => x(this, EventArgs.Empty));
		}
		public virtual bool SkipEditEnabled { get; set; }
		public virtual int SkipRecords { get; set; }
		public virtual bool TopEditEnabled { get; set; }
		public virtual int TopRecords { get; set; }
		EventHandler recordsLimitationChanged;
		event EventHandler IFiltersView.ChangeRecordsLimitationState {
			add { recordsLimitationChanged += value; }
			remove { recordsLimitationChanged -= value; }
		}
		void IFiltersView.SetGroupColumnsInfo(IEnumerable<FilterViewTableInfo> info) {
			foreach(var table in info)
				foreach(var column in table.Columns)
					groupFilterAdditionalProperties.Add(new PropertyInfo(column.ColumnName, null, column.ColumnType));
		}
		#endregion
	}
}
