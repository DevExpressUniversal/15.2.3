#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.Data.Filtering;
using DevExpress.Xpo;
namespace DevExpress.ExpressApp.EF {
	public class EFCollection<T> : EFCollection, IList<T> {
		public EFCollection(EFObjectSpace objectSpace, CriteriaOperator criteria, IList<SortProperty> sorting, Boolean inTransaction)
			: base(objectSpace, typeof(T), criteria, sorting, inTransaction) {
		}
		public Int32 IndexOf(T obj) {
			return base.IndexOf(obj);
		}
		public void Insert(Int32 index, T obj) {
			base.Insert(index, obj);
		}
		public new T this[Int32 index] {
			get { return (T)base[index]; }
			set { base[index] = value; }
		}
		public void Add(T obj) {
			base.Add(obj);
		}
		public Boolean Contains(T obj) {
			return base.Contains(obj);
		}
		public void CopyTo(T[] array, Int32 index) {
			base.CopyTo(array, index);
		}
		public Boolean Remove(T obj) {
			base.Remove(obj);
			return true;
		}
		IEnumerator<T> IEnumerable<T>.GetEnumerator() {
			InitObjects();
			return objects.Cast<T>().GetEnumerator();
		}
	}
}
