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
namespace DevExpress.DataAccess.Native.Sql {
	public class FilteredResultList : ResultTableBase {
		readonly ResultTable originalTable;
		readonly List<ResultRow> records;
		public FilteredResultList(ResultTable originalTable, Func<ResultRow, bool> filter) {
			this.originalTable = originalTable;
			this.records = ((IList<ResultRow>)originalTable).AsParallel().AsOrdered().Where(filter).ToList();
		}
		public FilteredResultList(ResultTable originalTable, List<ResultRow> records) {
			this.originalTable = originalTable;
			this.records = records;
		}
		#region Overrides of ResultListBase
		public override string TableName { get { return this.originalTable.TableName; }
			set { }
		}
		public override List<ResultColumn> Columns { get { return this.originalTable.Columns; } }
		public override List<ResultRelation> Details { get { return this.originalTable.Details; } }
		public override IEnumerator<ResultRow> GetEnumerator() { return this.records.GetEnumerator(); }
		public override bool Contains(ResultRow item) { return this.records.Contains(item); }
		public override void CopyTo(ResultRow[] array, int arrayIndex) { this.records.CopyTo(array, arrayIndex); }
		public override int Count { get { return this.records.Count; } }
		public override int IndexOf(ResultRow item) { return this.records.IndexOf(item); }
		protected override ResultRow ElementAt(int index) { return this.records[index]; }
		#endregion
	}
}
