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
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
#if !SL
using System.Drawing;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandCellStyle
	public class XlsCommandCellStyle : XlsCommandContentBase {
		XlsContentStyle content = new XlsContentStyle();
		#region Properties
		public string StyleName { get { return content.StyleName; } set { content.StyleName = value; } }
		public bool IsHidden { get { return content.IsHidden; } set { content.IsHidden = value; } }
		public int BuiltInId { get { return content.BuiltInId; } set { content.BuiltInId = value; } }
		public int OutlineLevel { get { return content.OutlineLevel; } set { content.OutlineLevel = value; } }
		public int StyleFormatId { get { return content.StyleFormatId; } set { content.StyleFormatId = value; } }
		public bool IsBuiltIn { get { return content.IsBuiltIn; } set { content.IsBuiltIn = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			if (IsBuiltIn) {
				if(BuiltInId != 0x01 && BuiltInId != 0x02)
					StyleName = BuiltInCellStyleCalculator.CalculateName(BuiltInId);
				else
					StyleName = OutlineCellStyle.CalculateName(BuiltInId == 0x01, OutlineLevel);
			}
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			XlsStyleInfo info = new XlsStyleInfo();
			info.StyleXFIndex = StyleFormatId;
			info.IsBuiltIn = IsBuiltIn;
			info.IsHidden = IsHidden;
			info.BuiltInId = BuiltInId;
			info.OutlineLevel = OutlineLevel;
			info.Name = StyleName;
			contentBuilder.StyleSheet.AddStyle(info);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandCellStyle();
		}
		protected override IXlsContent GetContent() {
			return content;
		}
	}
	#endregion
	#region XlsCommandCellStyleExt
	public class XlsCommandCellStyleExt : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 16;
		LPWideString styleName = new LPWideString();
		readonly XFProperties properties = new XFProperties();
		#endregion
		#region Properties
		public bool IsBuiltIn { get; set; }
		public bool IsHidden { get; set; }
		public bool CustomBuiltIn { get; set; }
		public StyleCategory Category { get; set; }
		public int BuiltInId { get; set; }
		public int OutlineLevel { get; set; }
		public string StyleName { get { return this.styleName.Value; } set { this.styleName.Value = value; } }
		public XFProperties Properties { get { return properties; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			byte bitwiseField = reader.ReadByte();
			IsBuiltIn = Convert.ToBoolean(bitwiseField & 0x01);
			IsHidden = Convert.ToBoolean(bitwiseField & 0x02);
			CustomBuiltIn = Convert.ToBoolean(bitwiseField & 0x04);
			Category = (StyleCategory)reader.ReadByte();
			if(!IsBuiltIn) {
				reader.ReadUInt16();
				BuiltInId = Int32.MinValue;
				OutlineLevel = Int32.MinValue;
			}
			else {
				BuiltInId = reader.ReadByte();
				OutlineLevel = reader.ReadByte() + 1;
				if(BuiltInId != 0x01 && BuiltInId != 0x02)
					OutlineLevel = Int32.MinValue;
			}
			this.styleName = LPWideString.FromStream(reader);
			this.properties.Read(reader);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			if(!contentBuilder.UseXFExt)
				return;
			XlsImportStyleSheet styleSheet = contentBuilder.StyleSheet;
			if(!styleSheet.IsFormatsRegistered) {
				XlsStyleInfo style = styleSheet.GetLastStyle();
				if(style == null) return;
				if(!IsBuiltIn && (style.Name != StyleName)) return;
				ApplyContent(style, styleSheet);
			}
			else {
				XlsStyleInfo style = styleSheet.GetStyleInfo(StyleName);
				if(style == null) return;
				ApplyContent(style, styleSheet);
			}
		}
		void ApplyContent(XlsStyleInfo style, XlsImportStyleSheet styleSheet) {
			style.IsBuiltIn = IsBuiltIn;
			style.IsHidden = IsHidden;
			style.CustomBuiltIn = CustomBuiltIn;
			if(BuiltInId != 0xff)
				style.BuiltInId = BuiltInId;
			style.OutlineLevel = OutlineLevel;
			if(Properties.Count > 0) {
				XlsExtendedFormatInfo xfInfo = styleSheet.GetExtendedFormatInfo(style.StyleXFIndex);
				XlsExtendedFormatInfoAdapter adapter = new XlsExtendedFormatInfoAdapter(xfInfo, styleSheet);
				Properties.ApplyContent(adapter);
				xfInfo.IsRegistered = false;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandCellStyleExt));
			header.Write(writer);
			byte bitwiseField = 0;
			if(IsBuiltIn) bitwiseField |= 0x01;
			if(IsHidden) bitwiseField |= 0x02;
			if(CustomBuiltIn) bitwiseField |= 0x04;
			writer.Write(bitwiseField);
			writer.Write((byte)Category);
			if(!IsBuiltIn) {
				writer.Write((ushort)0xffff);
			}
			else {
				writer.Write((byte)BuiltInId);
				if(BuiltInId == 0x01 || BuiltInId == 0x02)
					writer.Write((byte)(OutlineLevel - 1));
				else
					writer.Write((byte)0xff);
			}
			this.styleName.Write(writer);
			this.properties.Write(writer);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + this.styleName.Length + this.properties.GetSize());
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandCellStyleExt();
		}
	}
	#endregion
}
