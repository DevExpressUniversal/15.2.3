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

using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Data.Export {
	public class MsoCrc32Compute {
		static readonly uint[] crcCache = new uint[256];
		uint crcValue;
		public int CrcValue {
			get { return (int)crcValue; }
			set { crcValue = (uint)value; }
		}
		public MsoCrc32Compute() {
		}
		static MsoCrc32Compute() {
			uint value;
			for(int i = 0; i < 256; i++) {
				value = (uint)i;
				value <<= 24;
				for(int j = 0; j < 8; j++) {
					if((value & 0x80000000) == 0x80000000) {
						value <<= 1;
						value ^= 0xAF;
					} else {
						value <<= 1;
					}
				}
				value &= 0xffff;
				crcCache[i] = value;
			}
		}
		public void Add(byte data) {
			uint index = crcValue >> 24;
			index ^= data;
			crcValue <<= 8;
			crcValue ^= crcCache[index];
		}
		public void Add(byte[] data) {
			Guard.ArgumentNotNull(data, "data");
			AddCore(data);
		}
		public void Add(byte[] data, int start, int count) {
			Guard.ArgumentNotNull(data, "data");
			Guard.ArgumentNonNegative(start, "start");
			Guard.ArgumentNonNegative(count, "count");
			if(data.Length < (start + count))
				throw new ArgumentException("Data is not long enough for that start and count!");
			AddCore(data, start, count);
		}
		public void Add(short data) {
			AddCore(BitConverter.GetBytes(data));
		}
		public void Add(int data) {
			AddCore(BitConverter.GetBytes(data));
		}
		void AddCore(byte[] data) {
			int count = data.Length;
			for(int i = 0; i < count; i++)
				Add(data[i]);
		}
		void AddCore(byte[] data, int start, int count) {
			for(int i = 0; i < count; i++)
				Add(data[start + i]);
		}
	}
}
