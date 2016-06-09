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
using System.Collections;
namespace DevExpress.PivotGrid.Internal {
	public class UnsafeIListWrapper<TDerived1, TDerived2, TBase> : IList<TDerived1>
																	where TDerived1 : TBase
																	where TDerived2 : TBase {
		readonly IList<TDerived2> sourceList;
		public UnsafeIListWrapper(IList<TDerived2> sourceList) {
			this.sourceList = sourceList;
		}
		#region IList<TDerived> Members
		static TDerived2 Cast(TDerived1 item) {
			return CastExtension.Cast<TDerived2>(item);
		}
		static TDerived1 Cast(TDerived2 item) {
			return CastExtension.Cast<TDerived1>(item);
		}
		public int IndexOf(TDerived1 item) {
			return sourceList.IndexOf(Cast(item));
		}
		public void Insert(int index, TDerived1 item) {
			sourceList.Insert(index, Cast(item));
		}
		public void RemoveAt(int index) {
			sourceList.RemoveAt(index);
		}
		public TDerived1 this[int index] {
			get { return Cast(sourceList[index]); }
			set { sourceList[index] = Cast(value); }
		}
		#endregion
		#region ICollection<TDerived> Members
		ICollection<TDerived2> SourceCollection { get { return (ICollection<TDerived2>)sourceList; } }
		public void Add(TDerived1 item) {
			SourceCollection.Add(Cast(item));
		}
		public void Clear() {
			SourceCollection.Clear();
		}
		public bool Contains(TDerived1 item) {
			return SourceCollection.Contains(Cast(item));
		}
		public void CopyTo(TDerived1[] array, int arrayIndex) {
			TDerived2[] baseArray = new TDerived2[array.Length];
			SourceCollection.CopyTo(baseArray, 0);
			baseArray.CopyTo(array, arrayIndex);
		}
		public int Count {
			get { return SourceCollection.Count; }
		}
		public bool IsReadOnly {
			get { return SourceCollection.IsReadOnly; }
		}
		public bool Remove(TDerived1 item) {
			return SourceCollection.Remove(Cast(item));
		}
		#endregion
		#region IEnumerable<TDerived> Members
		public IEnumerator<TDerived1> GetEnumerator() {
			return new IEnumeratorWrapper<TDerived1, TDerived2, TBase>(sourceList.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		IEnumerable SourceEnumerable { get { return (IEnumerable)sourceList; } }
		IEnumerator IEnumerable.GetEnumerator() {
			return SourceEnumerable.GetEnumerator();
		}
		#endregion
		class IEnumeratorWrapper<TD1, TD2, TB> : IEnumerator<TD1> where TD1 : TB
																  where TD2 : TB {
			IEnumerator<TD2> sourceEnum;
			public IEnumeratorWrapper(IEnumerator<TD2> sourceEnum) {
				this.sourceEnum = sourceEnum;
			}
			#region IEnumerator<TD> Members
			public TD1 Current {
				get { return CastExtension.Cast<TD1>(sourceEnum.Current); }
			}
			#endregion
			#region IDisposable Members
			public void Dispose() {
				if(sourceEnum != null)
					sourceEnum.Dispose();
				sourceEnum = null;
			}
			#endregion
			#region IEnumerator Members
			IEnumerator Enumerator { get { return (IEnumerator)sourceEnum; } }
			object IEnumerator.Current {
				get { return Enumerator.Current; ; }
			}
			public bool MoveNext() {
				return Enumerator.MoveNext();
			}
			public void Reset() {
				Enumerator.Reset();
			}
			#endregion
		}
		static class CastExtension {
			public static T Cast<T>(object value) {
				return (T)value;
			}
		}
	}
	public class SafeToBaseIListWrapper<TDerived, TBase> : UnsafeIListWrapper<TBase, TDerived, TBase>
																			  where TDerived : TBase
																			   {
		public SafeToBaseIListWrapper(IList<TDerived> sourceList)
			: base(sourceList) { }
	}
}
