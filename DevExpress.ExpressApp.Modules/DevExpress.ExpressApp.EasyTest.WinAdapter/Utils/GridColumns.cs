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
using System.Windows.Forms;
using System.Reflection;
using DevExpress.EasyTest.Framework;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System.Collections;
using DevExpress.ExpressApp.EasyTest.WinAdapter;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Standard;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.DevExpressControls;
using DevExpress.ExpressApp.EasyTest.WinAdapter.TestControls.Xaf;
namespace DevExpress.ExpressApp.EasyTest.WinAdapter.Utils {
	public class MarshalByRefGridColumnEnumerator : MarshalByRefObject, IEnumerable<IGridColumn>, IEnumerator<IGridColumn>, IDisposable {
		IEnumerable<IGridColumn> enumerable;
		IEnumerator<IGridColumn> enumerator;
		internal MarshalByRefGridColumnEnumerator(IEnumerator<IGridColumn> enumerator) {
			this.enumerator = enumerator;
		}
		public MarshalByRefGridColumnEnumerator(IEnumerable<IGridColumn> enumerable) {
			this.enumerable = enumerable;
		}
		#region IEnumerable<IGridColumn> Members
		public IEnumerator<IGridColumn> GetEnumerator() {
			return new MarshalByRefGridColumnEnumerator(enumerable.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		IEnumerator IEnumerable.GetEnumerator() {
			return new MarshalByRefGridColumnEnumerator(enumerable.GetEnumerator());
		}
		#endregion
		#region IEnumerator<IGridColumn> Members
		public IGridColumn Current {
			get { return enumerator.Current; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			enumerator.Dispose();
			enumerator = null;
			enumerable = null;
		}
		#endregion
		#region IEnumerator Members
		object IEnumerator.Current {
			get { return enumerator.Current; }
		}
		public bool MoveNext() {
			return enumerator.MoveNext();
		}
		public void Reset() {
			enumerator.Reset();
		}
		#endregion
	}
}
