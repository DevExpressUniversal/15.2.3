#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Text;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Native {
	static class HashcodeHelper {
		static class ItemHashCode<T> {
			static bool isValueType = typeof(T).IsValueType();
			public static int Get(T item) {
				bool isNull = !isValueType && object.ReferenceEquals(item, null);
				return isNull ? 0 : item.GetHashCode();
			}
		}
		static int FNV_prime_32 = 16777619;
		static int FNV_offset_basis_32 = unchecked((int)2166136261);
		static int FNVHashItem(int item, int hash) {
			return unchecked((hash ^ item) * FNV_prime_32); 
		}
		public static int GetCompositeHashCode<T>(IEnumerable<T> items) {
			unchecked {
				int hash = FNV_offset_basis_32;
				foreach(T item in items)
					hash = FNVHashItem(ItemHashCode<T>.Get(item), hash);
				return hash;
			}
		}
		public static int GetCompositeHashCode<T>(T[] items) {
			unchecked {
				int hash = FNV_offset_basis_32;
				for(int i = 0; i < items.Length; i++)
					hash = FNVHashItem(ItemHashCode<T>.Get(items[i]), hash);
				return hash;
			}
		}
		public static int GetCompositeHashCode<T>(T item1, T item2) {
			unchecked {
				int hash = FNV_offset_basis_32;
				hash = FNVHashItem(ItemHashCode<T>.Get(item1), hash);
				hash = FNVHashItem(ItemHashCode<T>.Get(item2), hash);
				return hash;
			}
		}
		public static int GetCompositeHashCode<T>(T item1, T item2, T item3) {
			unchecked {
				int hash = GetCompositeHashCode(item1, item2);
				hash = FNVHashItem(ItemHashCode<T>.Get(item3), hash);
				return hash;
			}
		}
		public static int GetCompositeHashCode<T>(T item1, T item2, T item3, T item4) {
			unchecked {
				int hash = GetCompositeHashCode(item1, item2, item3);
				hash = FNVHashItem(ItemHashCode<T>.Get(item4), hash);
				return hash;
			}
		}
		public static int GetCompositeHashCode<T>(T item1, T item2, T item3, T item4, T item5) {
			unchecked {
				int hash = GetCompositeHashCode(item1, item2, item3, item4);
				hash = FNVHashItem(ItemHashCode<T>.Get(item5), hash);
				return hash;
			}
		}
		public static int GetCompositeHashCode<T>(T item1, T item2, T item3, T item4, T item5, params T[] restItems) {
			unchecked {
				int hash = GetCompositeHashCode(item1, item2, item3, item4, item5);
				foreach(T item in restItems) {
					hash = FNVHashItem(ItemHashCode<T>.Get(item), hash);
				}
				return hash;
			}
		}
	}
}
