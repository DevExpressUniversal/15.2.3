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
using System.IO;
using System.Reflection;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandExtendedSST
	public class XlsCommandExtendedSST : XlsCommandBase {
		const int coreSize = 2;
		const int sizeOfSSTInfo = 8;
		#region Fields
		int stringsInBucket = XlsDefs.MinStringsInBucket;
		List<XlsSSTInfo> items = new List<XlsSSTInfo>();
		#endregion
		#region Properties
		public int StringsInBucket {
			get { return stringsInBucket; }
			set {
				Guard.ArgumentNonNegative(value, "StringsInBucket value");
				stringsInBucket = value;
			} 
		}
		public IList<XlsSSTInfo> Items { get { return items; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			long initialPosition = reader.Position;
			StringsInBucket = reader.ReadUInt16();
			if(StringsInBucket > 0) {
				int sstInfoCount = (Size - coreSize) / sizeOfSSTInfo;
				int uniqueCount = contentBuilder.DocumentModel.SharedStringTable.Count;
				int count = uniqueCount / StringsInBucket;
				if((uniqueCount % StringsInBucket) != 0)
					count++;
				count = Math.Min(count, sstInfoCount);
				for(int i = 0; i < count; i++) {
					XlsSSTInfo item = new XlsSSTInfo();
					item.StreamPosition = reader.ReadInt32();
					item.Offset = reader.ReadUInt16();
					reader.Seek(2, SeekOrigin.Current); 
					Items.Add(item);
				}
			}
			long bytesToSkip = Size - (reader.Position - initialPosition);
			if(bytesToSkip > 0)
				reader.Seek(bytesToSkip, SeekOrigin.Current); 
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)StringsInBucket);
			foreach(XlsSSTInfo item in items) {
				writer.Write(item.StreamPosition);
				writer.Write((ushort)item.Offset);
				writer.Write((ushort)0x00); 
			}
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override short GetSize() {
			return (short)(coreSize + items.Count * sizeOfSSTInfo);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandExtendedSST();
		}
	}
	#endregion
}
