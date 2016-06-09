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
using System.Globalization;
using DevExpress.Utils;
namespace DevExpress.XtraPrinting.HtmlExport.Native {
	internal class DXHashCodeCombiner {
#region static
		internal static int CombineHashCodes(int h1, int h2) {
			return (((h1 << 5) + h1) ^ h2);
		}
		internal static int CombineHashCodes(int h1, int h2, int h3) {
			return CombineHashCodes(CombineHashCodes(h1, h2), h3);
		}
		internal static int CombineHashCodes(int h1, int h2, int h3, int h4) {
			return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
		}
		internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5) {
			return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
		}
#endregion
		long combinedHash;
		internal DXHashCodeCombiner() {
			combinedHash = 0x1505L;
		}
		internal DXHashCodeCombiner(long initialCombinedHash) {
			combinedHash = initialCombinedHash;
		}
		internal void AddArray(string[] a) {
			if(a != null) {
				int length = a.Length;
				for(int i = 0; i < length; i++) {
					AddObject(a[i]);
				}
			}
		}
		internal void AddCaseInsensitiveString(string s) {
			if(s != null)
				AddInt(StringExtensions.ComparerInvariantCultureIgnoreCase.GetHashCode(s));
		}
		internal void AddDateTime(DateTime dt) {
			AddInt(dt.GetHashCode());
		}
		internal void AddInt(int n) {
			combinedHash = ((combinedHash << 5) + combinedHash) ^ n;
		}
		internal void AddObject(bool b) {
			AddInt(b.GetHashCode());
		}
		internal void AddObject(byte b) {
			AddInt(b.GetHashCode());
		}
		internal void AddObject(int n) {
			AddInt(n);
		}
		internal void AddObject(long l) {
			AddInt(l.GetHashCode());
		}
		internal void AddObject(object o) {
			if(o != null)
				AddInt(o.GetHashCode());
		}
		internal void AddObject(string s) {
			if(s != null)
				AddInt(s.GetHashCode());
		}
		internal long CombinedHash {
			get { return combinedHash; }
		}
		internal int CombinedHash32 {
			get { return combinedHash.GetHashCode(); }
		}
		internal string CombinedHashString {
			get { return combinedHash.ToString("x", CultureInfo.InvariantCulture); }
		}
	}
}
