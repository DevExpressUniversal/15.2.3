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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.Data;
using DevExpress.XtraPivotGrid.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
namespace DevExpress.DashboardCommon.Native {
	public interface IDashboardDataSourceInternal {
		event EventHandler<NameChangedEventArgs> NameChanged;
		event EventHandler CaptionChanged;
		event EventHandler<DataChangedEventArgs> DataSourceDataChanged;
		event EventHandler<DataProcessingModeChangedEventArgs> DataProcessingModeChanged;
		Dashboard Dashboard { get; set; }
		DataSourceProperties Properties { get; }
		XElement SaveToXml();
		void LoadFromXml(XElement element);
		bool GetIsSqlServerMode(DataProcessingMode dataProcessingMode, string queryName);
		bool IsSqlServerMode(string queryName);
		object GetDataSchema(string queryName);
		Type ServerGetUnboundExpressionType(string expression, string queryName);
		CalculatedFieldDataColumnInfo CreateCalculatedFieldColumnInfo(CalculatedField field, IEnumerable<IParameter> parameters);
		IEnumerable<string> GetDataSets();
		IList GetListSource(string dataMember);
		IPivotGridDataSource GetPivotDataSource(string dataMember);
		IStorage GetStorage(string dataMember);
		List<ViewModel.ParameterValueViewModel> GetParameterValues(string valueMember, string displayMember, string queryName, IActualParametersProvider provider);
		bool ContainsParametersDisplayMember(string valueMember, string displayMember, string queryName);
		void SetParameters(IEnumerable<IParameter> parameters);
		string GetName_13_1();
	}
	public class DataProcessingModeChangedEventArgs : EventArgs {
		public DashboardSqlDataSource DataSource { get; private set; }
		public DataProcessingModeChangedEventArgs(DashboardSqlDataSource dataSource) {
			DataSource = dataSource;
		}
	}
}
