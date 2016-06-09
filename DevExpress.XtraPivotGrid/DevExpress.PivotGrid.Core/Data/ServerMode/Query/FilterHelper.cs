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
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.ServerMode {
	class FilterHelper : QueryFilterHelper {
		readonly CurrentFilterInfos currentFilterInfos = new CurrentFilterInfos();
		IServerModeHelpersOwner owner;
		CriteriaOperator prefilterCriteria, criteria;
		bool criteriaValid = false;
		public CriteriaOperator Criteria {
			get {
				if(!criteriaValid) {
					criteria = prefilterCriteria;
					foreach(ICurrentFilterInfo info in currentFilterInfos)
						criteria = CriteriaOperator.And(criteria, info.Criteria);
					criteria = GroupIntervalFilterSimplifier.Patch(criteria, owner.CubeColumns);
				}
				return criteria;
			}
		}
		public FilterHelper(IServerModeHelpersOwner owner)
			: base() {
			this.owner = owner;
		}
		public override bool BeforeSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields) {
			criteriaValid = false;
			bool res = !object.Equals(prefilterCriteria, owner.Owner.PrefilterCriteria);
			prefilterCriteria = owner.Owner.PrefilterCriteria;
			return res;
		}
		public override bool AfterSpreadAreas(PivotGridFieldReadOnlyCollection sortedFields) {
			criteriaValid = false;
			bool res = false;
			List<QueryColumn> founded = new List<QueryColumn>();
			foreach(DevExpress.XtraPivotGrid.PivotGridFieldBase field in sortedFields) {
				ServerModeColumn column;
				if(!owner.CubeColumns.TryGetValue(field, out column))
					continue;
				founded.Add(column);
				if(currentFilterInfos.IsFieldChanged(column, field))
					res = true;
			}
			currentFilterInfos.Remove(founded);
			return res;
		}
		public override void ClearCache() {
			criteriaValid = false;
			currentFilterInfos.Clear();
		}
	}
}
