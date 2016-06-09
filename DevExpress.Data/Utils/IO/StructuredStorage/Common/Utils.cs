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
using System.Text;
namespace DevExpress.Utils.StructuredStorage.Internal {
	#region SectorType
	[CLSCompliant(false)]
	public static class SectorType {
		public const UInt32 MaxValue = 0xFFFFFFFA;
		public const UInt32 Dif = 0xFFFFFFFC;
		public const UInt32 Fat = 0xFFFFFFFD;
		public const UInt32 EndOfChain = 0xFFFFFFFE;
		public const UInt32 Free = 0xFFFFFFFF;
		public const UInt32 NoStream = 0xFFFFFFFF;
	}
	#endregion
	#region Measures
	public static class Measures {
		public const int DirectoryEntrySize = 128;
		public const int HeaderSize = 512;
	}
	#endregion
	#region DirectoryEntryType
	public enum DirectoryEntryType {
		MinValue = 0,
		Invalid = 0,
		Storage = 1,
		Stream = 2,
		LockBytes = 3,
		Property = 4,
		Root = 5,
		MaxValue = 5
	}
	#endregion
	#region DirectoryEntryColor
	public enum DirectoryEntryColor {
		MinValue = 0,
		Red = 0,
		Black = 1,
		MaxValue = 1
	}
	#endregion
	#region InternalBitConverter
	[CLSCompliant(false)]
	public class InternalBitConverter {
		public static InternalBitConverter Create(bool isLittleEndian) {
			if (BitConverter.IsLittleEndian ^ isLittleEndian)
				return new PrereverseInternalBitConverter();
			else
				return new InternalBitConverter();
		}
		protected internal virtual void Preprocess(byte[] value) {
		}
		[CLSCompliant(false)]
		public UInt64 ToUInt64(byte[] value) {
			Preprocess(value);
			return BitConverter.ToUInt64(value, 0);
		}
		[CLSCompliant(false)]
		public UInt32 ToUInt32(byte[] value) {
			Preprocess(value);
			return BitConverter.ToUInt32(value, 0);
		}
		[CLSCompliant(false)]
		public UInt16 ToUInt16(byte[] value) {
			Preprocess(value);
			return BitConverter.ToUInt16(value, 0);
		}
		public string ToString(byte[] value) {
			Preprocess(value);
			string result = Encoding.Unicode.GetString(value, 0, value.Length);
			int index = result.IndexOf('\0');
			if (index >= 0)
				result = result.Remove(index);
			return result;
		}
		[CLSCompliant(false)]
		public byte[] GetBytes(UInt16 value) {
			byte[] result = BitConverter.GetBytes(value);
			Preprocess(result);
			return result;
		}
		[CLSCompliant(false)]
		public byte[] GetBytes(UInt32 value) {
			byte[] result = BitConverter.GetBytes(value);
			Preprocess(result);
			return result;
		}
		[CLSCompliant(false)]
		public byte[] GetBytes(UInt64 value) {
			byte[] result = BitConverter.GetBytes(value);
			Preprocess(result);
			return result;
		}
		[CLSCompliant(false)]
		public List<byte> GetBytes(List<UInt32> input) {
			List<byte> result = new List<byte>(sizeof(UInt32) * input.Count);
			int count = input.Count;
			for (int i = 0; i < count; i++)
				result.AddRange(GetBytes(input[i]));
			return result;
		}
	}
	#endregion
	#region PrereverseInternalBitConverter
	class PrereverseInternalBitConverter : InternalBitConverter {
		protected internal override void Preprocess(byte[] value) {
			Array.Reverse(value);
		}
	}
	#endregion
}
