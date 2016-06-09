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
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class FieldCriteriaFilterItems : PivotGridFilterItems {
		ExpressionEvaluator evaluator;
		public FieldCriteriaFilterItems(PivotGridData data, PivotGridFieldBase field, bool radioMode, bool showOnlyAvailableItems, bool deferUpdates, CriteriaOperator criteria)
			: base(data, field, radioMode, showOnlyAvailableItems, deferUpdates) {
			OLAPMetadataColumn column = ((IOLAPHelpersOwner)data.OLAPDataSource).Metadata.Columns[field.FieldName];
			if(ReferenceEquals(null, criteria) && column.HasCustomDefaultMember) {
				criteria = new BinaryOperator(column.UniqueName, column.DefaultMemberName);
			}
			evaluator = new ExpressionEvaluator(new QueryMemberEvaluator(), QueryCriteriaOptimizer.Optimize(criteria, column));
		}
		protected override bool? IsItemChecked(object value) {
			return evaluator.Fit(Data.GetOLAPMemberByUniqueName(Field.FieldName, value));
		}
	}
}
