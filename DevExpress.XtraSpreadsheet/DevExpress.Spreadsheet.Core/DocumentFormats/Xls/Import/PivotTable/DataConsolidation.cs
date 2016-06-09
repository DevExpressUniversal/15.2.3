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
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandDataConsolidationReference
	public class XlsCommandDataConsolidationReference : XlsCommandRangeBase {
		XLUnicodeStringNoCch virtualPath = new XLUnicodeStringNoCch();
		#region Properties
		public string VirtualPath {
			get { return virtualPath.Value; }
			set { virtualPath.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			base.ReadCore(reader, contentBuilder);
			int cchFile = reader.ReadUInt16();
			virtualPath = XLUnicodeStringNoCch.FromStream(reader, cchFile);
			int bytesToRead = Size - (8 + virtualPath.Length);
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			IXlsPivotCacheBuilder pivotCacheBuilder = contentBuilder.CurrentPivotCacheBuilder;
			if (pivotCacheBuilder.CacheType == PivotCacheType.Worksheet) {
				XlsPivotWorksheetCacheBuilder worksheetPivotCacheBuilder = (XlsPivotWorksheetCacheBuilder)pivotCacheBuilder;
				worksheetPivotCacheBuilder.DataRef.Range = Range;
				worksheetPivotCacheBuilder.DataRef.VirtualPath = VirtualPath;
			}
			else if (pivotCacheBuilder.CacheType == PivotCacheType.Consolidation) {
				XlsPivotConsolidationCacheBuilder consolidationPivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)pivotCacheBuilder;
				XlsPivotDataConsolidationContainer dataRef = new XlsPivotDataConsolidationContainer(Range, VirtualPath);
				consolidationPivotCacheBuilder.DataRefs.Add(dataRef);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			base.WriteCore(writer);
			writer.Write((ushort)virtualPath.Value.Length);
			virtualPath.Write(writer);
			if (XlsVirtualPath.IsSelfReference(virtualPath.Value)) {
				if (virtualPath.HasHighBytes)
					writer.Write((ushort)0);
				else
					writer.Write((byte)0);
			}
		}
		protected override short GetSize() {
			int result = 8 + virtualPath.Length;
			if (XlsVirtualPath.IsSelfReference(virtualPath.Value))
				result += virtualPath.HasHighBytes ? 2 : 1;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandDataConsolidationSourceBase (abstract)
	public abstract class XlsCommandDataConsolidationSourceBase : XlsCommandBase {
		XLUnicodeStringNoCch virtualPath = new XLUnicodeStringNoCch();
		#region Properties
		public string VirtualPath {
			get { return virtualPath.Value; }
			set { virtualPath.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int cchFile = reader.ReadUInt16();
			if (cchFile == 0)
				virtualPath = new XLUnicodeStringNoCch();
			else
				virtualPath = XLUnicodeStringNoCch.FromStream(reader, cchFile);
		}
		protected override void WriteCore(BinaryWriter writer) {
			int cchFile = virtualPath.Value.Length;
			writer.Write((ushort)cchFile);
			if (cchFile > 0) {
				virtualPath.Write(writer);
				if (XlsVirtualPath.IsSelfReference(virtualPath.Value)) {
					if (virtualPath.HasHighBytes)
						writer.Write((ushort)0);
					else
						writer.Write((byte)0);
				}
			}
		}
		protected override short GetSize() {
			int result = 2;
			if (virtualPath.Value.Length > 0) {
				result += virtualPath.Length;
				if (XlsVirtualPath.IsSelfReference(virtualPath.Value))
					result += virtualPath.HasHighBytes ? 2 : 1;
			}
			return (short)result;
		}
	}
	#endregion
	#region XlsCommandDataConsolidationName
	public class XlsCommandDataConsolidationName : XlsCommandDataConsolidationSourceBase {
		XLUnicodeString name = new XLUnicodeString();
		#region Properties
		public string Name { 
			get { return name.Value; } 
			set { name.Value = value; } 
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			long initialPos = reader.Position;
			name = XLUnicodeString.FromStream(reader);
			base.ReadCore(reader, contentBuilder);
			int bytesToRead = (int)(Size - (reader.Position - initialPos));
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			IXlsPivotCacheBuilder pivotCacheBuilder = contentBuilder.CurrentPivotCacheBuilder;
			if (pivotCacheBuilder.CacheType == PivotCacheType.Worksheet) {
				XlsPivotWorksheetCacheBuilder worksheetPivotCacheBuilder = (XlsPivotWorksheetCacheBuilder)pivotCacheBuilder;
				worksheetPivotCacheBuilder.DataRef.Name = Name;
				worksheetPivotCacheBuilder.DataRef.VirtualPath = VirtualPath;
			}
			else if (pivotCacheBuilder.CacheType == PivotCacheType.Consolidation) {
				XlsPivotConsolidationCacheBuilder consolidationPivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)pivotCacheBuilder;
				XlsPivotDataConsolidationContainer dataRef = new XlsPivotDataConsolidationContainer(Name, VirtualPath);
				consolidationPivotCacheBuilder.DataRefs.Add(dataRef);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			name.Write(writer);
			base.WriteCore(writer);
		}
		protected override short GetSize() {
			return (short)(base.GetSize() + name.Length);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandDataConsolidationBuiltInName
	public class XlsCommandDataConsolidationBuiltInName : XlsCommandDataConsolidationSourceBase {
		#region Built-in names
		static readonly string[] builtInNames = new string[] { 
			"_xlnm.Consolidate_Area", 
			"_xlnm.Auto_Open", 
			"_xlnm.Auto_Close", 
			"_xlnm.Extract", 
			"_xlnm.Database", 
			"_xlnm.Criteria", 
			"_xlnm.Print_Area", 
			"_xlnm.Print_Titles", 
			"_xlnm.Recorder", 
			"_xlnm.Data_Form", 
			"_xlnm.Auto_Activate", 
			"_xlnm.Auto_Deactivate", 
			"_xlnm.Sheet_Title", 
			"_xlnm._FilterDatabase" 
		};
		#endregion
		byte builtInId;
		#region Propertis
		public string Name {
			get {
				if (builtInId >= 0 && builtInId < builtInNames.Length)
					return builtInNames[builtInId];
				return string.Empty;
			}
			set {
				int index = -1;
				for (int i = 0; i < builtInNames.Length; i++) {
					if (value == builtInNames[i]) {
						index = i;
						break;
					}
				}
				if (index == -1)
					throw new ArgumentException("Invalid built-in defined name");
				builtInId = (byte)index;
			}
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			long initialPos = reader.Position;
			builtInId = reader.ReadByte();
			reader.ReadUInt16(); 
			reader.ReadByte(); 
			base.ReadCore(reader, contentBuilder);
			int bytesToRead = (int)(Size - (reader.Position - initialPos));
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.IsEmpty)
				return;
			IXlsPivotCacheBuilder pivotCacheBuilder = contentBuilder.CurrentPivotCacheBuilder;
			if (pivotCacheBuilder.CacheType == PivotCacheType.Worksheet) {
				XlsPivotWorksheetCacheBuilder worksheetPivotCacheBuilder = (XlsPivotWorksheetCacheBuilder)pivotCacheBuilder;
				worksheetPivotCacheBuilder.DataRef.Name = Name;
				worksheetPivotCacheBuilder.DataRef.VirtualPath = VirtualPath;
			}
			else if (pivotCacheBuilder.CacheType == PivotCacheType.Consolidation) {
				XlsPivotConsolidationCacheBuilder consolidationPivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)pivotCacheBuilder;
				XlsPivotDataConsolidationContainer dataRef = new XlsPivotDataConsolidationContainer(Name, VirtualPath);
				consolidationPivotCacheBuilder.DataRefs.Add(dataRef);
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(builtInId);
			writer.Write((ushort)0); 
			writer.Write((byte)0); 
			base.WriteCore(writer);
		}
		protected override short GetSize() {
			return (short)(base.GetSize() + 4);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
