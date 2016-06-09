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
using System.Linq;
using System.Linq.Expressions;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.Xpf.Grid.Filtering {
	public class DateFilterCache {
		readonly DataControlBase grid;
		readonly ColumnBase column;
		object[] values;
		public DateFilterCache(DataControlBase grid, ColumnBase column) {
			this.grid = grid;
			this.column = column;
		}
		public bool IsFilterVisibleForColumn(FilterData filter) {
			if(this.values == null) {
				this.values = this.grid.GetUniqueColumnValues(this.column, includeFilteredOut: true, roundDataTime: false);
			}
			if(this.values == null || this.values.Length == 0 || (AsyncServerModeDataController.IsNoValue(this.values[0]))) return true;
			var predicate = CompileToPredicate(filter);
			return this.values.AsParallel().Any(x => x is DateTime && predicate((DateTime)x));
		}
		public void UpdateValuesForColumn(object[] values) {
			this.values = values;
		}
		public void Clear() {
			this.values = null;
		}
		Func<DateTime, bool> CompileToPredicate(FilterData filter) {
			if(filter == null) return null;
			return CriteriaCompiler.ToPredicate<DateTime>(filter.Criteria, new DTDescriptor(this.column.FieldName));
		}
		class DTDescriptor : CriteriaCompilerDescriptor {
			readonly string PropertyName;
			public DTDescriptor(string propertyName) {
				PropertyName = propertyName;
			}
			public override Type ObjectType {
				get { return typeof(DateTime); }
			}
			public override Expression MakePropertyAccess(Expression baseExpression, string propertyPath) {
				if(propertyPath == PropertyName) {
					return baseExpression;
				}
				return Expression.Constant(null);
			}
		}
	}
}
