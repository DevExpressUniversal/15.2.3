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
using System.Text;
namespace DevExpress.ExpressApp.Utils {
	public class EnumeratorConverter<OfType,FromType> : IEnumerator<OfType>  where FromType : OfType {
		IEnumerator<FromType> enumerator;
		public EnumeratorConverter(IEnumerator<FromType> enumerator) {
			this.enumerator = enumerator;
		}
		#region IEnumerator<OfType> Members
		public OfType Current {
			get { return (OfType)enumerator.Current; }
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			enumerator.Dispose();
		}
		#endregion
		#region IEnumerator Members
		object System.Collections.IEnumerator.Current {
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
	public class EnumerableConverter<OfType, FromType> : IEnumerable<OfType> where FromType : OfType {
		IEnumerable<FromType> source;
		public EnumerableConverter(IEnumerable<FromType> source) {
			this.source = source;
		}
		#region IEnumerable<OfType> Members
		public IEnumerator<OfType> GetEnumerator() {
			return new EnumeratorConverter<OfType, FromType>(source.GetEnumerator());
		}
		#endregion
		#region IEnumerable Members
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return ((System.Collections.IEnumerable)source).GetEnumerator();
		}
		#endregion
	}
}
