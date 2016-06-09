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
namespace DevExpress.Persistent.Base {
	#region Obsolete 8.3
	[Obsolete("Use Enumerator.Combine", true)]
	public class CombinedEnumerator<OfType> : IEnumerator<OfType>, System.Collections.IEnumerator {
		private IEnumerator<OfType> enum1;
		private IEnumerator<OfType> enum2;
		private bool first = true;
		public CombinedEnumerator(IEnumerator<OfType> enum1, IEnumerator<OfType> enum2) {
			this.enum1 = enum1;
			this.enum2 = enum2;
		}
		#region IEnumerator<OfType> Members
		public OfType Current {
			get { return first ? enum1.Current : enum2.Current; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			enum1.Dispose();
			enum2.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}
		public bool MoveNext() {
			if (first) {
				first = enum1.MoveNext();
			}
			if (!first) {
				first = false;
				return enum2.MoveNext();
			}
			return true;
		}
		public void Reset() {
			enum1.Reset();
			enum2.Reset();
		}
		#endregion
		#region IEnumerator Members
		bool System.Collections.IEnumerator.MoveNext() {
			return MoveNext();
		}
		void System.Collections.IEnumerator.Reset() {
			Reset();
		}
		#endregion
	}
	#endregion
}
