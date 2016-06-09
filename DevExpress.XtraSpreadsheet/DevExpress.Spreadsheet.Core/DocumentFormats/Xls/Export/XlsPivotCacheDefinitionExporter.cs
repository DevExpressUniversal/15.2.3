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

using DevExpress.Office.Services;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using System;
using System.IO;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsPivotCacheDefinitionExporter : XlsExporterBase {
		#region Fields
		XlsExporter mainExporter;
		#endregion
		#region Properties
		public WorksheetCollection Sheets { get { return DocumentModel.Sheets; } }
		public XlsExporter MainExporter { get { return mainExporter; } }
		#endregion
		public XlsPivotCacheDefinitionExporter(BinaryWriter writer, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet, XlsExporter mainExporter)
			: base(writer, documentModel, exportStyleSheet) {
				this.mainExporter = mainExporter;
		}
		public override void WriteContent() {
			short streamId = 1;
			foreach (PivotCache cache in Sheets.Workbook.PivotCaches) {
				BinaryWriter binWriter = MainExporter.CreatePivotCacheWriter(streamId);
				XlsPivotCacheExporter pivotCacheExporter = new XlsPivotCacheExporter(binWriter, DocumentModel, ExportStyleSheet, cache, streamId);
				pivotCacheExporter.WriteContent();
				bool shouldExport = binWriter.BaseStream.Length > 0 && cache.Source.Type != PivotCacheType.External;
				if (!shouldExport)
					ExcludePivotCache(cache, binWriter, XtraSpreadsheetStringId.Msg_CantExportPivotCacheSourceExternal);
				else {
					for (int i = 0; i < cache.CacheFields.Count; ++i)
						if (cache.CacheFields[i].SharedItems.Count > 32500) {
							shouldExport = false;
							ExcludePivotCache(cache, binWriter, XtraSpreadsheetStringId.Msg_PivotFieldContainsTooMuschUniqueItems);
							break;
						}
					if (shouldExport) {
						WritePivotCacheStreamId(streamId);
						WritePivotCacheType(cache);
						WriteDataConsolidations(cache);
						WritePivotAddl(cache, streamId);
						mainExporter.IncludedPivotCaches.Add(cache);
					}
				}
				streamId++;
			}
		}
		void ExcludePivotCache(PivotCache cache, BinaryWriter binWriter, XtraSpreadsheetStringId id) {
			string message = XtraSpreadsheetLocalizer.GetString(id);
			DocumentModel.LogMessage(LogCategory.Warning, message);
			binWriter.BaseStream.SetLength(0);
			mainExporter.ExcludedPivotCaches.Add(cache);
		}
		#region ABNF -> PCDEFINITION -> SXStreamID
		protected void WritePivotCacheStreamId(short streamId) {
			XlsCommandPivotCacheStreamId command = new XlsCommandPivotCacheStreamId();
			command.Value = streamId;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXVS
		protected void WritePivotCacheType(PivotCache cache) {
			XlsCommandPivotCacheType command = new XlsCommandPivotCacheType();
			command.CacheType = cache.Source.Type;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC
		protected void WriteDataConsolidations(PivotCache cache) {
			if (cache.Source != null) {
				switch(cache.Source.Type){
					case PivotCacheType.Worksheet:
						WriteDataConsolidation(cache);
						break;
					case PivotCacheType.Consolidation:
						WriteMultipleConsolidationRanges(cache);
						break;
					case PivotCacheType.External:
						WriteDataOrParamQuery(cache.Source as PivotCacheSourceExternal);
						break;
				}
			}
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC -> DREF
		protected void WriteDataConsolidation(PivotCache cache) {
			PivotCacheSourceWorksheet container = ((PivotCacheSourceWorksheet)cache.Source);
			PivotCacheSourceWorksheetHelper helper = new PivotCacheSourceWorksheetHelper();
			helper.GenerateValues(container, DocumentModel.DataContext);
			string virtualPath = null;
			if (!string.IsNullOrEmpty(helper.Book))
				virtualPath = XlsVirtualPath.GetVirtualPath("[" + helper.Book + "]" + helper.Sheet);
			else
				if (!string.IsNullOrEmpty(helper.Sheet))
					virtualPath = "\u0002" + helper.Sheet;
			if (!string.IsNullOrEmpty(helper.Reference))
				WriteDataConsolidationReferenceWorksheet(helper.Range, virtualPath);
			else
				if (!string.IsNullOrEmpty(helper.Name))
					if (helper.Name.StartsWith("_xlnm."))
						WriteDataConsolidationBuiltInNameWorksheet(helper.Name, virtualPath);
					else
						WriteDataConsolidationNameWorksheet(helper.Name, virtualPath);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC -> DREF -> DConName
		void WriteDataConsolidationNameWorksheet(string name, string virtualPath) {
			XlsCommandDataConsolidationName command = new XlsCommandDataConsolidationName();
			command.Name = name;
			command.VirtualPath = virtualPath;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC -> DREF -> DConBin
		void WriteDataConsolidationBuiltInNameWorksheet(string name, string virtualPath) {
			XlsCommandDataConsolidationBuiltInName command = new XlsCommandDataConsolidationBuiltInName();
			command.Name = name;
			command.VirtualPath = virtualPath;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC -> DREF -> DConRef
		void WriteDataConsolidationReferenceWorksheet(CellRange range, string virtualPath) {
			XlsCommandDataConsolidationReference command = new XlsCommandDataConsolidationReference();
			command.Range = new CellRangeInfo(range.TopLeft, range.BottomRight);
			command.VirtualPath = virtualPath;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC -> SXTBL
		protected void WriteMultipleConsolidationRanges(PivotCache cache) { 
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXSRC -> DBQUERY
		protected void WriteDataOrParamQuery(PivotCacheSourceExternal source) {
			if (source != null) {
				XlsCommandDbOrParamQuery command = new XlsCommandDbOrParamQuery();
				command.DbQuery = new XlsDbQuery();
				command.ParamQuery = new XlsParamQuery();
				command.Write(StreamWriter);
				XlsCommandString strCommand = new XlsCommandString();
				XlsCommandContinue continueCommand = new XlsCommandContinue();
				using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, strCommand, continueCommand)) {
					XLUnicodeString str = new XLUnicodeString();
					str.Value = "Test Record";
					str.Write(writer);
				}
			}
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE
		void WriteAddlCommand(IXlsPivotAddl entity) {
			XlsCommandPivotAddl command = new XlsCommandPivotAddl();
			command.Data = entity;
			command.Write(StreamWriter);
		}
		protected void WritePivotAddl(PivotCache cache, int streamId) {
			WriteSxAddlDId(streamId);
			WriteSxAddlVersion10(cache);
			if (cache.CreatedVersion != 0)
				WriteSxMacro(cache);
			if (cache.CreatedVersion > 2) {
				WriteVersionUpdate(2);
				WriteSxDInfo12(cache);
				WriteRefreshReal(cache);
				WriteVersionUpdate(0xFF);
			}
			WriteSxDEnd();
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SxAddlId
		void WriteSxAddlDId(int id) {
			XlsPivotAddlCacheId command = new XlsPivotAddlCacheId();
			command.Id = id;
			WriteAddlCommand(command);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SxAddlVersion10
		void WriteSxAddlVersion10(PivotCache cache) {
			XlsPivotAddlCacheVersion10 command = new XlsPivotAddlCacheVersion10();
			command.MaxUnusedItems = cache.MissingItemsLimit;
			command.VersionCacheLastRefresh = (byte)(cache.RefreshedVersion > 4 ? 4 : cache.RefreshedVersion);
			command.VersionCacheRefreshableMin = cache.MinRefreshableVersion;
			VariantValue lastRefresh = new VariantValue();
			lastRefresh.SetDateTime(cache.RefreshedDate, DateSystem.Date1900);
			command.DateLastRefreshed = lastRefresh.NumericValue;
			WriteAddlCommand(command);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SxAddlSxMacro
		void WriteSxMacro(PivotCache cache) {
			XlsPivotAddlCacheVerMacro command = new XlsPivotAddlCacheVerMacro();
			command.Version = (byte)(cache.CreatedVersion > 4 ? 4 : cache.CreatedVersion);
			WriteAddlCommand(command);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SxAddlSxDEnd
		void WriteSxDEnd() {
			WriteAddlCommand(new XlsPivotAddlCacheEnd());
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SXADDLCACHE12 -> SxAddlVersionUpdate
		void WriteVersionUpdate(byte version) {
			XlsPivotAddlCacheVersionUpdate command = new XlsPivotAddlCacheVersionUpdate();
			command.Version = version;
			WriteAddlCommand(command);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SXADDLCACHE12 -> SxAddlSxDInfo12
		void WriteSxDInfo12(PivotCache cache) {
			XlsPivotAddlCacheInfo12 command = new XlsPivotAddlCacheInfo12();
			command.SupportsAttributeDrillDown = cache.SupportAdvancedDrill;
			command.SupportsSubQuery = cache.SupportSubquery;
			WriteAddlCommand(command);
		}
		#endregion
		#region ABNF -> PCDEFINITION -> SXADDLCACHE -> SXADDLCACHE12 -> SxAddlRefreshReal
		void WriteRefreshReal(PivotCache cache) {
			XlsPivotAddlCacheInvRefreshReal command = new XlsPivotAddlCacheInvRefreshReal();
			command.Invalid = cache.Invalid;
			command.EnableRefresh = cache.EnableRefresh;
			WriteAddlCommand(command);
		}
		#endregion
	}
}
