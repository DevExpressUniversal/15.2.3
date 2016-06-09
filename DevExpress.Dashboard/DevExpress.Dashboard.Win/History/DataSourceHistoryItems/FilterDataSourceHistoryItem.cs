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

using System.Collections.Generic;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardWin.Localization;
using DevExpress.Data;
namespace DevExpress.DashboardWin.Native {
	public class FilterDataSourceHistoryItem : DashboardParametersHistoryItem {
		readonly IDashboardDataSource dataSource;
		readonly string oldCriteria, newCriteria;
		public override string Caption {
			get { return string.Format(DashboardWinLocalizer.GetString(DashboardWinStringId.HistoryItemFilterDataSource), dataSource.Name); }
		}
		public FilterDataSourceHistoryItem(IDashboardDataSource dataSource, string oldCriteria, string newCriteria, DashboardParameterCollection parameters, ParameterChangesCollection parametersChanged)
			: base(parameters, parametersChanged) {
			this.dataSource = dataSource;
			this.oldCriteria = oldCriteria;
			this.newCriteria = newCriteria;
		}
		protected override IEnumerable<IDashboardDataSource> GetForcedDataSources(IEnumerable<IDashboardDataSource> dataSources) {
			if (newCriteria != oldCriteria)
				yield return dataSource;
		}
		protected override void PerformOperation(DashboardDesigner designer, bool redo) {
			base.PerformOperation(designer, redo);
			dataSource.Filter = redo ? newCriteria : oldCriteria;
		}
	}
}
