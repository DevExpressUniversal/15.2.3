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

using System.Collections;
using System.Collections.Generic;
#if SL
using DevExpress.Xpf.Collections;
#endif
namespace DevExpress.XtraPivotGrid.Data {
	public class PivotGridFieldAreaIndexComparer : IComparer, IComparer<PivotGridFieldBase> {
		public PivotGridFieldAreaIndexComparer() { }
		int IComparer.Compare(object obj1, object obj2) {
			PivotGridFieldBase field1 = (obj1 as PivotGridFieldBase);
			PivotGridFieldBase field2 = (obj2 as PivotGridFieldBase);
			return CompareCore(field1, field2);
		}
		public int Compare(PivotGridFieldBase field1, PivotGridFieldBase field2) {
			return CompareCore(field1, field2);
		}
		int CompareCore(PivotGridFieldBase field1, PivotGridFieldBase field2) {
			int res = Comparer<int>.Default.Compare(field1.Visible ? 0 : 1, field2.Visible ? 0 : 1);
			if(res != 0) return res;
			res = Comparer<int>.Default.Compare((int)field1.Area, (int)field2.Area);
			if(res != 0) return res;
			if(field1.Group != null && field1.Group == field2.Group) {
				return Comparer.Default.Compare(field1.Group.IndexOf(field1), field2.Group.IndexOf(field2));
			}
			if(field1.Group != null) field1 = field1.Group[0];
			if(field2.Group != null) field2 = field2.Group[0];
			res = Comparer<int>.Default.Compare(GetCompareIndexByFieldIndex(field1), GetCompareIndexByFieldIndex(field2));
			if(res != 0) return res;
			res = Comparer.Default.Compare(field1.IsDataField ? 0 : 1, field2.IsDataField ? 0 : 1);
			if(res != 0) return res;
			res = Comparer<int>.Default.Compare(GetCompareIndexByFieldOldIndex(field1), GetCompareIndexByFieldOldIndex(field2));
			if(res != 0 || field1.Data == null) return res;
			return Comparer<int>.Default.Compare(field1.Data.Fields.IndexOf(field2), field1.Data.Fields.IndexOf(field1));
		}
		protected int GetCompareIndexByFieldIndex(PivotGridFieldBase field) {
			if(field.IsDataField) {
				int areaIndex = field.Data.OptionsDataField.AreaIndex;
				return areaIndex < 0 ? int.MaxValue : areaIndex;
			}
			return field.AreaIndex < 0 ? int.MaxValue : field.AreaIndex;
		}
		int GetCompareIndexByFieldOldIndex(PivotGridFieldBase field) {
			if(field.AreaIndex == field.AreaIndexOldCore)
				return GetFieldIndex(field);
			return field.AreaIndex > field.AreaIndexOldCore ? int.MaxValue : int.MinValue;
		}
		Dictionary<PivotGridFieldBase, int> fieldIndexes;
		int GetFieldIndex(PivotGridFieldBase field) {
			PivotGridFieldCollectionBase collection = field.Collection;
			if(fieldIndexes == null && collection != null && collection.Count > 10) {
				fieldIndexes = new Dictionary<PivotGridFieldBase, int>(collection.Count);
				for(int i = 0; i < collection.Count; i++)
					fieldIndexes.Add(collection[i], i);
			}
			if(fieldIndexes != null) {
				int res;
				if(fieldIndexes.TryGetValue(field, out res))
					return res;
				res = field.Index;
				fieldIndexes.Add(field, res);
				return res;
			} else
				return field.Index;
		}
	}
	class FieldsInternalIndexComparer : Comparer<PivotGridFieldBase> {
		public override int Compare(PivotGridFieldBase x, PivotGridFieldBase y) {
			if(x.IsNew && !y.IsNew)
				return 1;
			if(!x.IsNew && y.IsNew)
				return -1;
			return Comparer.Default.Compare(x.IndexInternal, y.IndexInternal);
		}
	}
}
