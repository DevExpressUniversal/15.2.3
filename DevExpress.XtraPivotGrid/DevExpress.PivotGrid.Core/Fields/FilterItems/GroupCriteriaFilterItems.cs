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
using DevExpress.Utils.Controls;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class GroupCriteriaFilterItems : PivotGroupFilterItems {
		QueryGroupFilterEvaluator queryGroupFilterEvaluator;
		QueryFilterVisitor filterVisitor;
		public GroupCriteriaFilterItems(PivotGridData data, PivotGridFieldBase field, CriteriaOperator criteria) : this(data, field, criteria, ((IOLAPHelpersOwner)field.Data.OLAPDataSource).Metadata.Columns[field.FieldName]) {
		}
		internal GroupCriteriaFilterItems(PivotGridData data, PivotGridFieldBase field, CriteriaOperator criteria, OLAPMetadataColumn column) : base(data, field) {
			queryGroupFilterEvaluator = new QueryGroupFilterEvaluator();
			filterVisitor = new QueryFilterVisitor(queryGroupFilterEvaluator, criteria, column);
		}
		protected override bool? IsItemChecked(IFilterItem parent, object[] parentValues, object value) {
			if(parent != null && parent.IsChecked.HasValue)
				return parent.IsChecked.Value;
			List<string> values = new List<string>();
			if(parentValues != null) {
				foreach(string str in parentValues)
					values.Add(str);
			}
			values.Add((string)value);
			QueryContextCache contextCache = new QueryContextCache();
			OLAPMember lastMember = null;
			for(int i = 0; i < values.Count; i++) {
				lastMember = i == 0 ? (OLAPMember)Data.OLAPDataSource.GetMemberByUniqueName(Field.FieldName, values[0]) : lastMember.ChildMembers.GetMemberByUniqueLevelValue(values[i]);
				contextCache.Add(Field.Group[i].Name, lastMember);
			}
			return filterVisitor.Fit(contextCache);
		}
	}
}
