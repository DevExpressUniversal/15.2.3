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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
namespace DevExpress.Diagram.Core {
	public abstract class EquatableObject<T> where T : EquatableObject<T> {
		protected EquatableObject() { }
		public sealed override bool Equals(object obj) {
			var other = obj as T;
			return other != null && Equals(other);
		}
		protected abstract bool Equals(T other);
		public override int GetHashCode() {
			throw new NotImplementedException();
		}
	}
	public class DelegateEqualityComparer<T> : IEqualityComparer<T> {
		static readonly Func<T, int> DefaultGetHashCode = x => x.GetHashCode();
		readonly Func<T, T, bool> equals;
		readonly Func<T, int> getHashCode;
		public DelegateEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode = null) {
			this.equals = equals;
			this.getHashCode = getHashCode ?? DefaultGetHashCode;
		}
		bool IEqualityComparer<T>.Equals(T x, T y) {
			return equals(x, y);
		}
		int IEqualityComparer<T>.GetHashCode(T obj) {
			return getHashCode(obj);
		}
	}
	public class ComparerHelper<T> : IComparer<T>, IComparer {
		readonly Func<T, T, int> compareFunc;
		public ComparerHelper(Func<T, T, int> compareFunc) {
			this.compareFunc = compareFunc;
		}
		public int Compare(T x, T y) {
			return compareFunc(x, y);
		}
		int IComparer.Compare(object x, object y) {
			return compareFunc((T)x, (T)y);
		}
	}
}
