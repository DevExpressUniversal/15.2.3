#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Drawing;
using System.Linq;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
namespace DevExpress.DashboardWin.Native {
	public class DimensionFilterColumn : DataItemFilterColumn<Dimension> {
		protected override Type DataType { get { return DataSourceHelper.GetDimensionDataType(DashboardItem.DataSource, DashboardItem.DataMember, Dimension).ToType(); } }
		public Dimension Dimension { get { return DataItem; } }
		public DimensionFilterColumn(DataDashboardItem dashboardItem, Dimension dataItem, IActualParametersProvider provider)
			: base(dashboardItem, dataItem, provider) {
		}
		protected override RepositoryItem CreateDefaultRepositoryItem(IActualParametersProvider provider) {
			bool isOlapFormatter = Dimension != null && DashboardItem.DataSource != null && DashboardItem.DataSource.GetIsOlap();
			FormatterBase formatter = FormatterBase.CreateFormatter(isOlapFormatter ? new ValueFormatViewModel { DataType = ValueDataType.String } : Dimension.CreateValueFormatViewModel());	
			RepositoryItemComboBox repositoryItem = new DataItemFilterRepositoryItemComboBox(formatter);
			IEnumerable<object> values = DashboardItem.DataSource.GetUniqueValues(Dimension, DashboardItem.DataMember, provider);
			if(values != null)
				repositoryItem.Items.AddRange(values.Select((v) => new FilterItem(v, formatter.Format(v))).ToArray());
			return repositoryItem;
		}
	}
	public class MeasureFilterColumn : DataItemFilterColumn<Measure> {
		protected override Type DataType { get { return DataSourceHelper.GetMeasureDataType(DashboardItem.DataSource, DashboardItem.DataMember, Measure).ToType(); } }
		public Measure Measure { get { return DataItem; } }
		public MeasureFilterColumn(DataDashboardItem dashboardItem, Measure dataItem, IActualParametersProvider provider)
			: base(dashboardItem, dataItem, provider) {
		}
	}
	public abstract class DataItemFilterColumn<T> : FilterColumn where T : DataItem {
		readonly DataDashboardItem dashboardItem;
		readonly T dataItem;
		readonly IActualParametersProvider parametersProvider;
		RepositoryItem repositoryItem;
		protected abstract Type DataType { get; }
		protected T DataItem { get { return dataItem; } }
		public override Image Image { get { return null; } }
		public override RepositoryItem ColumnEditor {
			get {
				if(repositoryItem == null) {
					if(DataType == typeof(DateTime))
						repositoryItem = new DataItemFilterRepositoryItemDateEdit();
					else
						repositoryItem = CreateDefaultRepositoryItem(parametersProvider);
				}
				return repositoryItem;
			}
		}
		protected DataDashboardItem DashboardItem { get { return dashboardItem; } }
		public override Type ColumnType { get { return DataType; } }
		public override string FieldName { get { return dashboardItem.DataItemRepository.GetActualID(dataItem); } }
		public override string ColumnCaption { get { return dataItem.DisplayName; } }
		protected DataItemFilterColumn(DataDashboardItem dashboardItem, T dataItem, IActualParametersProvider parametersProvider) {
			this.dashboardItem = dashboardItem;
			this.dataItem = dataItem;
			this.parametersProvider = parametersProvider;
		}
		protected virtual RepositoryItem CreateDefaultRepositoryItem(IActualParametersProvider provider) {
			return null;
		}
		public override void Dispose() {
			base.Dispose();
			if(repositoryItem != null) {
				repositoryItem.Dispose();
				repositoryItem = null;
			}
		}
	}
	public class DimensionFilterColumnCollection : FilterColumnCollection {
		public DimensionFilterColumnCollection(DataDashboardItem dashboardItem, IActualParametersProvider provider) {
			foreach(Dimension dimension in dashboardItem.UniqueDimensions)
				Add(new DimensionFilterColumn(dashboardItem, dimension, provider));
		}
	}
}
