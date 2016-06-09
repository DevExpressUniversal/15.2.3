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
using System.Collections;
using DevExpress.Utils;
namespace DevExpress.Snap.Core.Native.Data.Implementations {
	public class HierarchicalEnumerator<T> : IEnumerator<T> {
		IEnumerator<T> parentEnumerator;
		IEnumerator<T> baseEnumerator;
		bool firstMove;
		public HierarchicalEnumerator(IEnumerator<T> parentEnumerator, IEnumerator<T> baseEnumerator) {
			Guard.ArgumentNotNull(parentEnumerator, "parentEnumerator");
			Guard.ArgumentNotNull(baseEnumerator, "baseEnumerator");
			this.parentEnumerator = parentEnumerator;
			this.baseEnumerator = baseEnumerator;
			Reset();
		}
		public T Current {
			get { return baseEnumerator.Current; }
		}
		public IEnumerator<T> ParentEnumerator { get { return parentEnumerator; } }
		public IEnumerator<T> BaseEnumerator { get { return baseEnumerator; } }
		#region IDisposable
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		~HierarchicalEnumerator() {
			Dispose(false);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				if (baseEnumerator != null)
					baseEnumerator.Dispose();
				if (parentEnumerator != null) {
					parentEnumerator.Dispose();
				}
			}
			baseEnumerator = null;
			parentEnumerator = null;
		}
		#endregion
		object IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (firstMove) {
				firstMove = false;
				if (!parentEnumerator.MoveNext())
					return false;
			}
			if (baseEnumerator.MoveNext())
				return true;
			while (parentEnumerator.MoveNext()) {
				baseEnumerator.Reset();
				if (baseEnumerator.MoveNext())
					return true;
			}
			return false;
		}
		public void Reset() {
			parentEnumerator.Reset();
			baseEnumerator.Reset();
			firstMove = true;
		}
	}
}
