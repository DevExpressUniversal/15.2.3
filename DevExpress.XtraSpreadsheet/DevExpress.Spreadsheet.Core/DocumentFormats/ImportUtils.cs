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
using System.ComponentModel;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using DevExpress.Utils;
using DevExpress.Utils.StructuredStorage.Writer;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Office.Import;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import {
	public class MergedCellsRangeOverlapChecker {
		const int rowDistance = 16;
		const int columnDistance = 64;
		const int lastClusterIndex = -1;
		abstract class MergedCellsCluster {
			public abstract void Add(CellRangeInfo range);
			public abstract bool IsNotOverlapped(CellRangeInfo range);
		}
		class MergedCellsColumnsCluster : MergedCellsCluster {
			List<CellRangeInfo> innerList = new List<CellRangeInfo>();
			public override void Add(CellRangeInfo range) {
				this.innerList.Add(range);
			}
			public override bool IsNotOverlapped(CellRangeInfo range) {
				int count = this.innerList.Count;
				for(int i = 0; i < count; i++) {
					if(range.HasCommonCells(this.innerList[i]))
						return false;
				}
				return true;
			}
		}
		class MergedCellsRowsCluster : MergedCellsCluster {
			Dictionary<int, MergedCellsCluster> clusters = new Dictionary<int, MergedCellsCluster>();
			public override void Add(CellRangeInfo range) {
				int index = GetIndexOfCluster(range);
				MergedCellsCluster cluster = GetCluster(index);
				cluster.Add(range);
			}
			public override bool IsNotOverlapped(CellRangeInfo range) {
				int index = GetIndexOfCluster(range);
				MergedCellsCluster cluster = GetCluster(index);
				bool result;
				if(index != lastClusterIndex) {
					result = cluster.IsNotOverlapped(range);
					if(result) {
						if(!GetCluster(lastClusterIndex).IsNotOverlapped(range))
							result = false;
					}
				}
				else {
					result = cluster.IsNotOverlapped(range);
					if(result) {
						int firstIndex = range.First.Column / columnDistance;
						int lastIndex = range.Last.Column / columnDistance;
						for(int i = firstIndex; i <= lastIndex; i++) {
							MergedCellsCluster clusterItem;
							if(this.clusters.TryGetValue(i, out clusterItem) && !clusterItem.IsNotOverlapped(range)) {
								result = false;
								break;
							}
						}
					}
				}
				return result;
			}
			int GetIndexOfCluster(CellRangeInfo range) {
				int firstIndex = range.First.Column / columnDistance;
				int lastIndex = range.Last.Column / columnDistance;
				if(firstIndex != lastIndex)
					return lastClusterIndex;
				return firstIndex;
			}
			MergedCellsCluster GetCluster(int index) {
				if(this.clusters.ContainsKey(index))
					return this.clusters[index];
				MergedCellsCluster cluster = new MergedCellsColumnsCluster();
				this.clusters.Add(index, cluster);
				return cluster;
			}
		}
		Dictionary<int, MergedCellsCluster> clusters;
		public MergedCellsRangeOverlapChecker() {
			this.clusters = new Dictionary<int, MergedCellsCluster>();
		}
		public void Clear() {
			this.clusters.Clear();
		}
		public bool IsNotOverlapped(CellRangeInfo range) {
			int index = GetIndexOfCluster(range);
			MergedCellsCluster cluster = GetCluster(index);
			bool result;
			if(index != lastClusterIndex) {
				result = cluster.IsNotOverlapped(range);
				if(result) {
					if(GetCluster(lastClusterIndex).IsNotOverlapped(range))
						cluster.Add(range);
					else
						result = false;
				}
			}
			else {
				result = cluster.IsNotOverlapped(range);
				if(result) {
					int firstIndex = range.First.Row / rowDistance;
					int lastIndex = range.Last.Row / rowDistance;
					for(int i = firstIndex; i <= lastIndex; i++) {
						MergedCellsCluster clusterItem;
						if(this.clusters.TryGetValue(i, out clusterItem) && !clusterItem.IsNotOverlapped(range)) {
							result = false;
							break;
						}
					}
					if(result)
						cluster.Add(range);
				}
			}
			return result;
		}
		int GetIndexOfCluster(CellRangeInfo range) {
			int firstIndex = range.First.Row / rowDistance;
			int lastIndex = range.Last.Row / rowDistance;
			if(firstIndex != lastIndex)
				return lastClusterIndex;
			return firstIndex;
		}
		MergedCellsCluster GetCluster(int index) {
			if(this.clusters.ContainsKey(index))
				return this.clusters[index];
			MergedCellsCluster cluster = new MergedCellsRowsCluster();
			this.clusters.Add(index, cluster);
			return cluster;
		}
	}
	#region VmlUnitConverter
	public class VmlUnitConverter {
		readonly DocumentModelUnitConverter unitConverter;
		public VmlUnitConverter(DocumentModelUnitConverter unitConverter) {
			Guard.ArgumentNotNull(unitConverter, "unitConverter");
			this.unitConverter = unitConverter;
		}
		public int ToModelUnits(DXVmlUnit unit) {
			switch(unit.Type) {
				case DXUnitType.Cm: return (int)unitConverter.CentimetersToModelUnitsF(unit.Value);
				case DXUnitType.Mm: return (int)unitConverter.MillimetersToModelUnitsF(unit.Value);
				case DXUnitType.Inch: return (int)unitConverter.InchesToModelUnitsF(unit.Value);
				case DXUnitType.Point: return (int)unitConverter.PointsToModelUnitsF(unit.Value);
				case DXUnitType.Pica: return (int)unitConverter.PicasToModelUnitsF(unit.Value);
				case DXUnitType.Pixel: return (int)unitConverter.PointsToModelUnitsF(unit.Value);
			}
			return unitConverter.EmuToModelUnits((int)unit.Value);
		}
	}
	#endregion
	#region VmlInsetHelper
	public static class VmlInsetHelper {
		public static VmlInsetData GetDefault(DocumentModel workbook) {
			VmlInsetData result = new VmlInsetData();
			result.LeftMargin = (int)workbook.UnitConverter.InchesToModelUnitsF(0.1f);
			result.TopMargin = (int)workbook.UnitConverter.InchesToModelUnitsF(0.05f);
			result.RightMargin = (int)workbook.UnitConverter.InchesToModelUnitsF(0.1f);
			result.BottomMargin = (int)workbook.UnitConverter.InchesToModelUnitsF(0.05f);
			return result;
		}
		public static VmlInsetData Parse(string inset, DocumentModel workbook) {
			VmlInsetData result = GetDefault(workbook);
			if(!string.IsNullOrEmpty(inset)) {
				VmlUnitConverter converter = new VmlUnitConverter(workbook.UnitConverter);
				string[] parts = inset.Split(new char[] { ',' });
				if(parts.Length > 0 && !string.IsNullOrEmpty(parts[0]))
					result.LeftMargin = converter.ToModelUnits(new DXVmlUnit(parts[0]));
				if(parts.Length > 1 && !string.IsNullOrEmpty(parts[1]))
					result.TopMargin = converter.ToModelUnits(new DXVmlUnit(parts[1]));
				if(parts.Length > 2 && !string.IsNullOrEmpty(parts[2]))
					result.RightMargin = converter.ToModelUnits(new DXVmlUnit(parts[2]));
				if(parts.Length > 3 && !string.IsNullOrEmpty(parts[3]))
					result.BottomMargin = converter.ToModelUnits(new DXVmlUnit(parts[3]));
			}
			return result;
		}
	}
	#endregion
	#region CodeNameHelper
	public static class CodeNameHelper {
		const string validPattern = @"^[\p{L}\P{IsBasicLatin}][_\d\p{L}\P{IsBasicLatin}]*$";
		const string replacePattern = @"[^\p{L}\P{IsBasicLatin}]";
		public static bool IsValidCodeName(string value) {
			return Regex.IsMatch(value, validPattern);
		}
		public static string CleanUp(string value) {
			return Regex.Replace(value, replacePattern, "_");
		}
	}
	#endregion
	#region ViewPaneTypeHelper
	static class ViewPaneTypeHelper {
		public static ViewPaneType CodeToPaneType(byte code) {
			switch(code) {
				case 0: return ViewPaneType.BottomRight;
				case 1: return ViewPaneType.TopRight;
				case 2: return ViewPaneType.BottomLeft;
			}
			return ViewPaneType.TopLeft;
		}
		public static byte PaneTypeToCode(ViewPaneType paneType) {
			switch(paneType) {
				case ViewPaneType.BottomRight: return 0;
				case ViewPaneType.TopRight: return 1;
				case ViewPaneType.BottomLeft: return 2;
			}
			return 3;
		}
	}
	#endregion
}
namespace DevExpress.XtraSpreadsheet.Export {
	#region ConditionalFormattingGroupCollection
	class ConditionalFormattingGroupCollection : List<List<ConditionalFormatting>> {
		public int Register(ConditionalFormatting conditionalFormatting) {
			if(!conditionalFormatting.IsValid)
				return -1;
			int index = LocateGroup(conditionalFormatting);
			if(index < 0) {
				Add(new List<ConditionalFormatting>());
				index = Count - 1;
			}
			List<ConditionalFormatting> group = this[index];
			group.Add(conditionalFormatting);
			return index;
		}
		bool IsEqualRange(CellRangeBase cellRange1, CellRangeBase cellRange2) {
			return cellRange1.Equals(cellRange2);
		}
		protected int LocateGroup(ConditionalFormatting conditionalFormatting) {
			int count = Count;
			for(int i = 0; i < count; ++i) {
				List<ConditionalFormatting> item = this[i];
				if((item != null) && (item.Count > 0)) {
					ConditionalFormatting groupRoot = item[0];
					if(IsEqualRange(groupRoot.CellRange, conditionalFormatting.CellRange) && 
						(groupRoot.IsPivot == conditionalFormatting.IsPivot)) {
						return i;
					}
				}
			}
			return -1;
		}
	}
	#endregion
	#region CellFormatExportHelper
	static class CellFormatExportHelper {
		public static int GetActualBorderIndex(this CellFormatBase format) {
			CellFormat cellFormat = format as CellFormat;
			if(cellFormat != null) {
				if(cellFormat.ApplyBorder || !cellFormat.Style.FormatInfo.ApplyBorder) 
					return cellFormat.BorderIndex;
				return cellFormat.Style.FormatInfo.BorderIndex;
			}
			return format.BorderIndex;
		}
		public static FormatBase GetActualFillOwner(this CellFormatBase format) {
			CellFormat cellFormat = format as CellFormat;
			if (cellFormat != null) {
				if (cellFormat.ApplyFill || !cellFormat.Style.FormatInfo.ApplyFill)
					return format;
				return cellFormat.Style.FormatInfo;
			}
			return format;
		}
		public static int GetActualFontIndex(this CellFormatBase format) {
			CellFormat cellFormat = format as CellFormat;
			if(cellFormat != null) {
				if(cellFormat.ApplyFont || !cellFormat.Style.FormatInfo.ApplyFont) 
					return cellFormat.FontIndex;
				return cellFormat.Style.FormatInfo.FontIndex;
			}
			return format.FontIndex;
		}
		public static int GetActualNumberFormatIndex(this CellFormatBase format) {
			CellFormat cellFormat = format as CellFormat;
			if(cellFormat != null) {
				if(cellFormat.ApplyNumberFormat || !cellFormat.Style.FormatInfo.ApplyNumberFormat) 
					return cellFormat.NumberFormatIndex;
				return cellFormat.Style.FormatInfo.NumberFormatIndex;
			}
			return format.NumberFormatIndex;
		}
		public static CellAlignmentInfo GetActualAlignment(this CellFormatBase format) {
			CellFormat cellFormat = format as CellFormat;
			if(cellFormat != null) {
				if(cellFormat.ApplyAlignment || !cellFormat.Style.FormatInfo.ApplyAlignment)
					return format.AlignmentInfo;
				return cellFormat.Style.FormatInfo.AlignmentInfo;
			}
			return format.AlignmentInfo;
		}
		public static CellFormatFlagsInfo GetActualFormatFlags(this CellFormatBase format) {
			CellFormat cellFormat = format as CellFormat;
			if(cellFormat != null) {
				if(cellFormat.ApplyProtection || !cellFormat.Style.FormatInfo.ApplyProtection)
					return format.CellFormatFlagsInfo;
				return cellFormat.Style.FormatInfo.CellFormatFlagsInfo;
			}
			return format.CellFormatFlagsInfo;
		}
	}
	#endregion
	#region RowExportHelper
	static class RowExportHelper {
		public static float GetRowHeight(Row row, Worksheet sheet) {
			if(!row.IsCustomHeight && row.Height == 0) {
				if(sheet.Properties.FormatProperties.IsCustomHeight)
					return sheet.Properties.FormatProperties.DefaultRowHeight;
			}
			return row.Height;
		}
		public static bool GetIsCustomHeight(Row row, Worksheet sheet) {
			if(!row.IsCustomHeight && row.Height == 0)
				return sheet.Properties.FormatProperties.IsCustomHeight;
			return row.IsCustomHeight;
		}
	}
	#endregion
	#region StructuredStorageHelper
	static class StructuredStorageHelper {
		public static void AddStreamDirectoryEntry(StructuredStorageWriter writer, string name, BinaryWriter binaryWriter) {
			string[] parts = name.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
			DevExpress.Utils.StructuredStorage.Internal.Writer.StorageDirectoryEntry directoryEntry = writer.RootDirectoryEntry;
			for(int i = 0; i < parts.Length - 1; i++)
				directoryEntry = directoryEntry.AddStorageDirectoryEntry(parts[i]);
			directoryEntry.AddStreamDirectoryEntry(parts[parts.Length - 1], binaryWriter.BaseStream);
			binaryWriter.Flush();
		}
	}
	#endregion
	#region VbaProjectExportHelper
	public class VbaProjectEntry {
		public string StreamName { get; set; }
		public BinaryWriter Writer { get; set; }
	}
	static class VbaProjectExportHelper {
		public static void ExportVbaProjectItem(List<VbaProjectEntry> vbaProjectEntries, string name, byte[] data) {
			if(IsPerfomanceCache(name))
				return;
			MemoryStream ms = new MemoryStream(data.Length);
			VbaProjectEntry entry = new VbaProjectEntry();
			entry.StreamName = name;
			entry.Writer = new BinaryWriter(ms);
			entry.Writer.Write(data);
			vbaProjectEntries.Add(entry);
		}
		static bool IsPerfomanceCache(string name) {
			return name.Contains("__SRP_");
		}
	}
	#endregion
	#region PivotCacheStorage
	internal class PivotCacheEntry {
		public static PivotCacheEntry Create(int pivotCacheId) {
			PivotCacheEntry result = new PivotCacheEntry();
			result.StreamName = string.Format("_SX_DB_CUR\\{0:X4}", pivotCacheId);
			return result;
		}
		public string StreamName { get; set; }
		public BinaryWriter Writer { get; set; }
	}
	#endregion
	#region CustomXmlExportHelper
	public class CustomXmlDataEntry {
		public string StreamName { get; set; }
		public BinaryWriter ItemData { get; set; }
		public BinaryWriter Properties { get; set; }
	}
	static class CustomXmlExportHelper {
		public static void ExportCustomXmlData(List<CustomXmlDataEntry> customXmlDataEntries, string name, byte[] data) {
			CustomXmlDataEntry entry = new CustomXmlDataEntry {StreamName = name};
			MemoryStream itemMs = new MemoryStream(data.Length);
			entry.ItemData = new BinaryWriter(itemMs);
			entry.ItemData.Write(data);
			WriteProperties(entry);
			customXmlDataEntries.Add(entry);
		}
		static void WriteProperties(CustomXmlDataEntry entry) {
			string guid = Guid.NewGuid().ToString("B");
			string properties = String.Format(@"<?xml version=""1.0"" encoding=""UTF-8"" standalone=""no""?>" +
											  @"<ds:datastoreItem ds:itemID=""{0}"" xmlns:ds=""http://schemas.openxmlformats.org/officeDocument/2006/customXml"">" +
											  @"	<ds:schemaRefs/>" +
											  @"</ds:datastoreItem>", guid);
			MemoryStream propertiesMemoryStream = new MemoryStream();
			entry.Properties = new BinaryWriter(propertiesMemoryStream);
			entry.Properties.Write(properties);
		}
	}
	#endregion
	public struct CFHtmlOffsets {
		public int StartHtmlByteOffset { get; set; }
		public int EndHTMLByteOffset { get; set; }
		public int StartFragmentByteOffset { get; set; }
		public int EndFragmentByteOffset { get; set; }
	}
	public class HtmlToClipboardHtmlConverter {
		const string headerFormat = @"Version:0.9
StartHTML:{0:0000000000}
EndHTML:{1:0000000000}
StartFragment:{2:0000000000}
EndFragment:{3:0000000000}
";
		const string sourceUrlFormat = @"SourceURL:";
		const string documentBegin = "<html>\r\n<body>\r\n\t<!--StartFragment-->";
		const string documentEnd = "<!--EndFragment--></body>\r\n</html>";
		string uri;
		public HtmlToClipboardHtmlConverter() {
		}
		public string Uri { get { return uri; } set { uri = value; } }
		public string GetFormattedHeader() {
			return string.Format(headerFormat, 0, 0, 0, 0);
		}
		public CFHtmlOffsets CalculateOffsets(string utf8HtmlCode) {
			CFHtmlOffsets result = new CFHtmlOffsets();
			result.StartHtmlByteOffset = GetFormattedHeader().Length;
			if(!String.IsNullOrEmpty(uri))
				result.StartHtmlByteOffset += Encoding.UTF8.GetByteCount(String.Concat(sourceUrlFormat, uri, "\r\n"));
			string startFragmentComment = "<!--StartFragment-->";
			string endFragmentComment = "<!--EndFragment-->";
			result.StartFragmentByteOffset = result.StartHtmlByteOffset + utf8HtmlCode.IndexOf(startFragmentComment) + startFragmentComment.Length;
			int endFragmentPosition = utf8HtmlCode.IndexOf(endFragmentComment);
			string before = utf8HtmlCode.Substring(0, endFragmentPosition);
			int offset = Encoding.UTF8.GetByteCount(before);
			result.EndFragmentByteOffset = result.StartHtmlByteOffset + offset; 
			string documentAfterEndFragment = utf8HtmlCode.Substring(endFragmentPosition);  
			string htmlCloseTag = "</html>";
			int localHtmlEndPosition = documentAfterEndFragment.IndexOf(htmlCloseTag) + htmlCloseTag.Length + 2; 
			string afterEndFragmentBeforeEndHtml = documentAfterEndFragment.Substring(0, localHtmlEndPosition);
			offset = Encoding.UTF8.GetByteCount(afterEndFragmentBeforeEndHtml);
			result.EndHTMLByteOffset = result.EndFragmentByteOffset + offset - 1; 
			return result;
		}
		public string Convert(string utf8HtmlCode) {
			StringBuilder result = new StringBuilder();
			CFHtmlOffsets offsets = CalculateOffsets(utf8HtmlCode);
			result.Append(String.Format(headerFormat, offsets.StartHtmlByteOffset, offsets.EndHTMLByteOffset, offsets.StartFragmentByteOffset, offsets.EndFragmentByteOffset));
			if(!String.IsNullOrEmpty(uri)) {
				result.Append(sourceUrlFormat);
				result.Append(uri);
				result.Append("\r\n");
			}
			result.Append(utf8HtmlCode);
			return result.ToString();
		}
	}
}
