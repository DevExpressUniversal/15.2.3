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
using System.Collections;
namespace DevExpress.XtraExport {
	[CLSCompliant(false)]
	public class XlsRecordItem {
		int size;
		DynamicByteBuffer data;
		public XlsRecordItem(int size, DynamicByteBuffer data) {
			this.size = size;
			this.data = data;
		}
		public int Size {
			get {
				return size;
			}
		}
		public DynamicByteBuffer Data {
			get {
				return data;
			}
		}
	}
	[CLSCompliant(false)]
	public class XlsRecordList : ArrayList {
		ushort id;
		public XlsRecordList(ushort recordId)
			: base() {
			id = recordId;
		}
		public new XlsRecordItem this[int index] {
			get {
				return (XlsRecordItem)base[index];
			}
		}
		public int AddData(DynamicByteBuffer data, int size) {
			return Add(new XlsRecordItem(size, data));
		}
		public int AddUniqueStyleData(DynamicByteBuffer data, XlsRecordList styles, DynamicByteBuffer dataExt, XlsRecordList stylesExt) {
			ushort size = BitConverter.ToUInt16(data.GetElements(0, 2), 0);
			for(int i = 0; i < Count; i++) {
				DynamicByteBuffer style = styles[i].Data;
				DynamicByteBuffer styleExt = stylesExt[i].Data;
				if(data.Compare(0, style.Data) && dataExt.Compare(30, styleExt.Data, 30, 3)) {
					dataExt = null;
					data = null;
					return i;
				}
			}
			int result = Add(new XlsRecordItem(size, data));
			stylesExt.AddStyleExtData(dataExt, result + XlsConsts.CountOfXFStyles);
			return result;
		}
		public void AddStyleChecksum(int countXFs, int value) {
			DynamicByteBuffer data = new DynamicByteBuffer((byte[])XlsConsts.XFChecksum.Clone());
			ushort size = BitConverter.ToUInt16(data.GetElements(0, 2), 0);
			data.SetElements(16, BitConverter.GetBytes(countXFs));
			data.SetElements(18, BitConverter.GetBytes(value));
			Add(new XlsRecordItem(size, data));
		}
		public void AddStyleExtData(DynamicByteBuffer data, int currentXF) {
			DynamicByteBuffer buf = new DynamicByteBuffer((byte[])data.Data.Clone());
			buf.SetElements(16, BitConverter.GetBytes(currentXF));
			Add(new XlsRecordItem(40, buf));
		}
		public int AddUniqueFormatData(DynamicByteBuffer data) {
			ushort size = BitConverter.ToUInt16(data.GetElements(0, 2), 0);
			for(int i = 0; i < Count; i++) {
				DynamicByteBuffer item = this[i].Data;
				if(data.Compare(4, item.Data, 4, size - 2)) {
					data = null;
					return i;
				}
			}
			return Add(new XlsRecordItem(size, data));
		}
		public int AddUniqueFontData(DynamicByteBuffer data) {
			ushort size = BitConverter.ToUInt16(data.GetElements(0, 2), 0);
			int hashCode = BitConverter.ToInt32(data.GetElements(size + 2, 4), 0);
			for(int i = 0; i < Count; i++) {
				DynamicByteBuffer item = this[i].Data;
				int offset = BitConverter.ToUInt16(item.GetElements(0, 2), 0);
				if((hashCode == BitConverter.ToInt32(item.GetElements(offset + 2, 4), 0) &&
					data.Compare(0, item.Data, 0, size))) {
					data = null;
					return i;
				}
			}
			return Add(new XlsRecordItem(size, data));
		}
		public void SaveToStream(XlsStream stream) {
			for(int i = 0; i < Count; i++) {
				stream.Write(BitConverter.GetBytes(id), 0, 2);
				DynamicByteBuffer item = this[i].Data;
				item.WriteToStream(stream, this[i].Size + 2);
			}
		}
		public int GetSize(int index) {
			return this[index].Size;
		}
		public int GetFullSize() {
			int result = Count << 2;
			for(int i = 0; i < Count; i++)
				result += this[i].Size;
			return result;
		}
	}
}
