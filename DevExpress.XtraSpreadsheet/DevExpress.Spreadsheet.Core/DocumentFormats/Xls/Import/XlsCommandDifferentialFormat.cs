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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandDifferentialFormat (DXF)
	public class XlsCommandDifferentialFormat : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 14;
		bool uiFill = true;
		bool newBorders = true;
		readonly XlsDifferentialFormatInfo info = new XlsDifferentialFormatInfo();
		readonly XFProperties properties = new XFProperties();
		#endregion
		#region Properties
		public bool UiFill { get { return uiFill; } set { uiFill = value; } }
		public bool NewBorders { get { return newBorders; } set { newBorders = value; } }
		public XFProperties Properties { get { return properties; } }
		protected internal XlsDifferentialFormatInfo Info { get { return info; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			ushort bitwiseField = reader.ReadUInt16();
			UiFill = Convert.ToBoolean(bitwiseField & 0x0001);
			NewBorders = Convert.ToBoolean(bitwiseField & 0x0002);
			info.IsForegroundBackgroundColorSwapped = !UiFill;
			if (!NewBorders)
				info.BorderOutline = NewBorders;
			Properties.Read(reader);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			Properties.ApplyContent(new XlsDifferentialFormatInfoAdapter(info));
			info.Register(contentBuilder.StyleSheet);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandDifferentialFormat));
			header.Write(writer);
			ushort bitwiseField = 0;
			if (UiFill)
				bitwiseField |= 0x0001;
			if (NewBorders)
				bitwiseField |= 0x0002;
			writer.Write(bitwiseField);
			Properties.Write(writer);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + Properties.GetSize());
		}
		protected internal void AssignDifferentialFormat(XlsDifferentialFormatInfo info) {
			info.CreateDifferentialFormatProperties(this);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandDifferentialFormat();
		}
	}
	#endregion
}
