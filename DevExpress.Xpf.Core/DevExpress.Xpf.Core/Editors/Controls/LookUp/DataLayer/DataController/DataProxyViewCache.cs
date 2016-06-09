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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Access;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Xpf.Editors.Native;
namespace DevExpress.Xpf.Editors.Helpers {
	public abstract class DataProxyViewCache : IEnumerable<DataProxy> {
		public DataAccessor DataAccessor { get; private set; }
		public abstract DataProxy this[int index] { get; set; }
		public abstract int Count { get; }
		public abstract int FindIndexByText(CriteriaOperator compareOperand, CriteriaOperator compareOperator, string text, bool isCaseSensitive, int startItemIndex, bool searchNext, bool ignoreStartIndex);
		public abstract int FindIndexByValue(CriteriaOperator compareOperator, object value);
		protected DataProxyViewCache(DataAccessor dataAccessor) {
			this.DataAccessor = dataAccessor;
		}
		public abstract void Add(int index, DataProxy item);
		public abstract void Replace(int index, DataProxy item);
		public abstract void Remove(int index);
		public abstract IEnumerator<DataProxy> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public virtual int FindIndexByTextLocal(CriteriaOperator compareOperator, bool isCaseSensitive, IEnumerable<DataProxy> view, int startItemIndex, bool searchNext, bool ignoreStartIndex) {
			ExpressionEvaluator evaluator =
				new ExpressionEvaluator(
					new PropertyDescriptorCollection(new PropertyDescriptor[] { new GetStringFromLookUpValuePropertyDescriptor(DataListDescriptor.GetFastProperty(TypeDescriptor.GetProperties(DataAccessor.ElementType).Find(DataAccessor.DisplayPropertyName, true))) }),
					compareOperator, isCaseSensitive);
			var searchView = searchNext ? view : view.Reverse();
			foreach (var item in searchView) {
				bool skip = Skip(startItemIndex, searchNext, ignoreStartIndex, item);
				if (!skip && (bool)evaluator.Evaluate(item))
					return FindIndexByItem(item);
			}
			return -1;
		}
		bool Skip(int startItemIndex, bool searchNext, bool ignoreStartIndex, DataProxy item) {
			return SkipInternal(startItemIndex, searchNext, ignoreStartIndex, item);
		}
		protected virtual bool SkipInternal(int startItemIndex, bool searchNext, bool ignoreStartIndex, DataProxy item) {
			bool skip = searchNext
				? (ignoreStartIndex ? item.f_visibleIndex <= startItemIndex : item.f_visibleIndex < startItemIndex)
				: (ignoreStartIndex ? item.f_visibleIndex >= startItemIndex : item.f_visibleIndex > startItemIndex);
			return skip;
		}
		protected virtual int FindIndexByItem(DataProxy item) {
			return item.f_visibleIndex;
		}
		public abstract void Reset();
	}
}
