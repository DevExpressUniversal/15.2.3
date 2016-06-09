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
namespace DevExpress.Utils.Serializing.Helpers {
	public abstract class VirtualXtraPropertyCollectionBase : IXtraPropertyCollection {
		bool enumeratorCreated;
		protected VirtualXtraPropertyCollectionBase() {
		}
		#region IXtraPropertyCollection Members
		public XtraPropertyInfo this[string name] { get { throw new InvalidOperationException(); } }
		public XtraPropertyInfo this[int index] { get { throw new InvalidOperationException(); } }
		public bool IsSinglePass { get { return true; } }
		public void AddRange(ICollection props) {
			throw new InvalidOperationException();
		}
		public void Add(XtraPropertyInfo prop) {
			throw new InvalidOperationException();
		}
		#endregion
		#region ICollection Members
		public void CopyTo(Array array, int index) {
			throw new InvalidOperationException();
		}
		public virtual int Count { get { throw new InvalidOperationException(); } }
		public bool IsSynchronized { get { return false; } }
		public object SyncRoot { get { return null; } }
		#endregion
		#region IEnumerable Members
		public IEnumerator GetEnumerator() {
			if(enumeratorCreated)
				throw new InvalidOperationException();
			enumeratorCreated = true;
			return CreateEnumerator();
		}
		#endregion
		protected abstract CollectionItemInfosEnumeratorBase CreateEnumerator();
	}
}
