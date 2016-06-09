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
using DevExpress.Utils;
using DevExpress.Office.Services.Implementation;
namespace DevExpress.Office.Utils {
	#region PackedValues helper
	static class PackedValues {
		public static bool GetBoolBitValue(uint packedValues, uint mask) {
			return (packedValues & mask) != 0;
		}
		public static void SetBoolBitValue(ref uint packedValues, uint mask, bool value) {
			if (value)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		public static void SetBoolBitValue(ref byte packedValues, byte mask, bool value) {
			if (value)
				packedValues |= mask;
			else
				packedValues &= (byte)~mask;
		}
		public static int GetIntBitValue(uint packedValues, uint mask) {
			return (int)(packedValues & mask);
		}
		public static int GetIntBitValue(uint packedValues, uint mask, int offset) {
			return (int)((packedValues & mask) >> offset);
		}
		public static void SetIntBitValue(ref uint packedValues, uint mask, int value) {
			packedValues &= ~mask;
			packedValues |= (uint)value & mask;
		}
		public static void SetIntBitValue(ref uint packedValues, uint mask, int offset, int value) {
			packedValues &= ~mask;
			packedValues |= ((uint)value << offset) & mask;
		}
		public static void SetIntBitValue(ref byte packedValues, byte mask, byte offset, byte value) {
			packedValues &= (byte)~mask;
			packedValues |= (byte)((value << offset) & mask);
		}
	}
	#endregion
	public struct CombinedHashCode {
		public const long Initial = 0x1505;
		long combinedHash;
		public CombinedHashCode(long initial) {
			combinedHash = initial;
		}
		public void AddInt(int n) {
			this.combinedHash = ((combinedHash << 5) + combinedHash) ^ n;
		}
		public void AddObject(object obj) {
			if (obj != null)
				AddInt(obj.GetHashCode());
		}
		public void AddByteArray(byte[] array) {
			int count = array.Length;
			for (int i = 0; i < count; i++)
				AddInt((int)(array[i]));
		}
		public long CombinedHash { get { return combinedHash; } }
		public int CombinedHash32 { get { return combinedHash.GetHashCode(); } }
	}
}
