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
namespace DevExpress.XtraReports.Data {
	public class FilteringOperatorsByTableDictionary : Dictionary<string, IList<CriteriaOperator>> {
		public FilteringOperatorsByTableDictionary() {
		}
		public FilteringOperatorsByTableDictionary(int capacity)
			: base(capacity) {
		}
		public FilteringOperatorsByTableDictionary(FilteringOperatorsByTableDictionary dictionary)
			: base(dictionary) {
		}
		#region Equality
		public static bool operator ==(FilteringOperatorsByTableDictionary first, FilteringOperatorsByTableDictionary second) {
			if(object.ReferenceEquals(first, second))
				return true;
			return !object.ReferenceEquals(first, null)
				&& !object.ReferenceEquals(second, null)
				&& first.Equals(second);
		}
		public static bool operator !=(FilteringOperatorsByTableDictionary first, FilteringOperatorsByTableDictionary second) {
			return !(first == second);
		}
		public override bool Equals(object obj) {
			FilteringOperatorsByTableDictionary another = obj as FilteringOperatorsByTableDictionary;
			return !object.ReferenceEquals(another, null) && Equals(this, another);
		}
		static bool Equals(FilteringOperatorsByTableDictionary first, FilteringOperatorsByTableDictionary second) {
			if(first.Count != second.Count)
				return false;
			foreach(var filteringOperatorsAndTablePair in first) {
				string key = filteringOperatorsAndTablePair.Key;
				if(!second.ContainsKey(key) || !Equals(first[key], second[key]))
					return false;
			}
			return true;
		}
		static bool Equals(IList<CriteriaOperator> first, IList<CriteriaOperator> second) {
			if(first.Count != second.Count)
				return false;
			for(int i = 0; i < first.Count; i++) {
				if(!CriteriaOperator.CriterionEquals(first[i], second[i]))
					return false;
			}
			return true;
		}
		public override int GetHashCode() {
			return base.GetHashCode();
		}
		#endregion
	}
}
