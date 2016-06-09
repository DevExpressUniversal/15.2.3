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
using System.Text;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data.Utils;
namespace DevExpress.XtraPivotGrid.Fields {
	class PivotGridFieldReadOnlyCollections {
		static readonly int areaCount = Helpers.GetEnumValues(typeof(PivotArea)).Length;
		PivotGridFieldReadOnlyCollection[] fieldCollections;
		PivotGridFieldReadOnlyCollection columnFieldLevelCollection;
		PivotGridFieldReadOnlyCollection rowFieldLevelCollection;
		public PivotGridFieldReadOnlyCollections() {
			this.fieldCollections = new PivotGridFieldReadOnlyCollection[areaCount];
			for(int i = 0; i < fieldCollections.Length; i++)
				this.fieldCollections[i] = new PivotGridFieldReadOnlyCollection();
			this.columnFieldLevelCollection = new PivotGridFieldReadOnlyCollection();
			this.rowFieldLevelCollection = new PivotGridFieldReadOnlyCollection();
		}
		PivotGridFieldReadOnlyCollection[] FieldCollections {
			get { return fieldCollections; }
		}
		PivotGridFieldReadOnlyCollection ColumnFieldLevelCollection {
			get { return columnFieldLevelCollection; }
		}
		PivotGridFieldReadOnlyCollection RowFieldLevelCollection {
			get { return rowFieldLevelCollection; }
		}
		public void Clear() {
			for(int i = 0; i < FieldCollections.Length; i++)
				FieldCollections[i].Clear();
			ColumnFieldLevelCollection.Clear();
			RowFieldLevelCollection.Clear();
		}
		public bool IsEmpty {
			get {
				for(int i = 0; i < Count; i++) {
					if(FieldCollections[i].Count > 0)
						return false;
				}
				return true;
			}
		}
		public int Count {
			get { return areaCount; }
		}
		public PivotGridFieldReadOnlyCollection this[int index] {
			get { return FieldCollections[index]; }
		}
		public PivotGridFieldReadOnlyCollection this[PivotArea area] {
			get { return this[(int)area]; }
		}
		public PivotGridFieldReadOnlyCollection GetFieldLevelCollection(bool isColumn) {
			return isColumn ? ColumnFieldLevelCollection : RowFieldLevelCollection;
		}
	}
}
