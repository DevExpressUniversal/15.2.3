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
using System.IO;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsXMapInfo // Feat11XMap structure
	public class XlsXMapInfo {
		const short fixedPartSize = 2;
		#region Properties
		public XMapEntry XMapEntry { get; set; }
		protected internal short Size { get { return GetSize(); } }
		#endregion
		public void Read(BinaryReader reader) {
			int count = reader.ReadUInt16();
			if (count > 0) {
				XMapEntry = new XMapEntry();
				XMapEntry.Read(reader);
			}
		}
		public void Write(BinaryWriter writer) {
			if (XMapEntry == null)  
				writer.Write((ushort)0);
			else {
				writer.Write((ushort)1);
				XMapEntry.Write(writer);
			}
		}
		short GetSize() {
			int result = fixedPartSize;
			if (XMapEntry != null)
				result += XMapEntry.Size;
			return (short)result;
		}
	}
	#endregion
	#region XMapEntry // Feat11XMapEntry structure
	public class XMapEntry {
		#region Fields
		const short fixedPartSize = 4;
		readonly XMapEntryDetails details = new XMapEntryDetails();
		#endregion
		#region Properties
		public XMapEntryDetails Details { get { return details; } }
		public bool IsCanBeSingle { get; set; }
		protected internal short Size { get { return (short)(fixedPartSize + Details.Size); } }
		#endregion 
		public void Read(BinaryReader reader) {
			uint bitwiseField = reader.ReadUInt32();
			IsCanBeSingle = Convert.ToBoolean(bitwiseField & 0x00000004);
			Details.Read(reader);
		}
		public void Write(BinaryWriter writer) {
			uint bitwiseField = 0x02;
			if (IsCanBeSingle)
				bitwiseField |= 0x00000004;
			writer.Write(bitwiseField);
			Details.Write(writer);
		}
	}
	#endregion
	#region XMapEntryDetails // Feat11XMapEntry2 structure
	public class XMapEntryDetails {
		#region Fileds
		const short fixedPartSize = 4;
		XLUnicodeString xPathExpession = new XLUnicodeString();
		#endregion
		#region Properties
		public XLUnicodeString XPathExpession { get { return xPathExpession; } set { xPathExpession = value; } }
		public int MapId { get; set; }
		protected internal short Size { get { return (short)(fixedPartSize + XPathExpession.Length); } }
		#endregion
		public void Read(BinaryReader reader) {
			MapId = reader.ReadInt32();
			XPathExpession = XLUnicodeString.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((uint)MapId);
			XPathExpession.Write(writer);
		}
	}
	#endregion
}
