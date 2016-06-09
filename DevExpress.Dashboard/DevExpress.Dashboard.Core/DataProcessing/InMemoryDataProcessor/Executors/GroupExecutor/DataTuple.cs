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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataProcessing.InMemoryDataProcessor.Executors {
	struct DataTuple {
		int index;
		public int Index { get { return index; } }
		public DataTuple(int index) {
			this.index = index;
		}
		public override int GetHashCode() {
			throw new NotSupportedException();
		}
		public override bool Equals(object obj) {
			throw new NotSupportedException();
		}
	}
	class DataTupleEqualityComparer : IEqualityComparer<DataTuple> {
		int columnsCount;
		GroupedColumnBase[] columns;
		public DataTupleEqualityComparer(GroupedColumnBase[] columns) {
			this.columns = columns;
			this.columnsCount = columns.Length;
		}
		public bool Equals(DataTuple x, DataTuple y) {
			for(int i = 0; i < columnsCount; i++)
				if(!columns[i].ValueEquals(x.Index, y.Index))
					return false;
			return true;
		}
		public int GetHashCode(DataTuple tuple) {
			unchecked {
				int FNV_prime_32 = 16777619;
				int FNV_offset_basis_32 = (int)2166136261;
				int hash = FNV_offset_basis_32;
				for(int i = 0; i < columnsCount; i++) {
					int item = columns[i].ValueHashCode(tuple.Index);
					hash = (hash ^ item) * FNV_prime_32;  
				}
				return hash;
			}
		}
	}
}
