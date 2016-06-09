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
using System.Text;
using System.Collections;
using DevExpress.XtraReports.UI;
using System.Windows.Forms;
using System.Data;
namespace DevExpress.XtraReports.Native.Data {
	public class DataSourceCollector {
		protected XtraReport report;
		public DataSourceCollector(XtraReport report) {
			this.report = report;
		}
		public object[] Collect() {
			ArrayList dataSources = new ArrayList();
			CollectCore(dataSources);
			FilterDataSources(dataSources);
			AddDataSource(dataSources, report.SchemaDataSource);
			AddDataSource(dataSources, report.ParametersDataSource);
			return dataSources.ToArray();
		}
		protected virtual void CollectCore(ArrayList dataSources) {
			TypedComponentEnumerator detailReportBandEnumerator = new TypedComponentEnumerator(report.OrderedBands, typeof(DetailReportBand));
			while(detailReportBandEnumerator.MoveNext()) {
				DetailReportBand detailReportBand = (DetailReportBand)detailReportBandEnumerator.Current;
				AddDataSource(dataSources, GetDataSource(detailReportBand));
			}
			AddDataSource(dataSources, GetDataSource(report));
		}
		static object GetDataSource(XtraReportBase report) {
			return ValidateDataSource(report.GetEffectiveDataSource());
		}
		void FilterDataSources(ArrayList dataSources) {
			List<BindingSource> bindingSources = new List<BindingSource>();
			for(int i = dataSources.Count - 1; i >= 0; i--) {
				if(dataSources[i] is BindingSource) {
					bindingSources.Add((BindingSource)dataSources[i]);
					dataSources.RemoveAt(i);
				}
			}
			for(int i = bindingSources.Count - 1; i >= 0; i--) {
				int index = dataSources.IndexOf(bindingSources[i].DataSource);
				if(index >= 0) {
					if(Object.Equals(report.DataSource, dataSources[index]))
						bindingSources.RemoveAt(i);
					else
						dataSources.RemoveAt(index);
				}
			}
			dataSources.AddRange(bindingSources);
		}
		protected static void AddDataSource(ArrayList dataSources, object dataSource) {
			if(dataSource != null && !dataSources.Contains(dataSource))
				dataSources.Add(dataSource);
		}
		static object ValidateDataSource(object dataSource) {
			return dataSource is DataTable && ((DataTable)dataSource).DataSet != null ?
				((DataTable)dataSource).DataSet : dataSource;
		}
	}
}
