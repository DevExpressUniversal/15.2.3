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
using DevExpress.Snap.API.Native;
using DevExpress.Snap.Core.API;
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Snap.Core.Options;
namespace DevExpress.Snap.Core.Native.Options {
	public interface IDataSourceContainer {
		void RegisterDataSource(string dataSourceName, object dataSource);
	}
	internal class NativeSnapMailMergeExportOptions : SnapMailMergeExportOptions, IDataSourceContainer {
		int exportFrom = 0;
		int exportRecordsCount = -1;
		bool startEachRecordFromNewParagraph = true;
		bool copyTemplateStyles;
		bool headerFooterLinkToPrevious = true;
		bool progressIndicationFormVisible = true;
		object dataSource;
		string dataSourceName;
		string dataMember;
		string filterString;
		readonly SnapMailMergeExportSorting sorting;
		RecordSeparator recordSeparator = RecordSeparator.PageBreak;
		SnapNativeDocument customSeparator;
		SnapDocumentModel documentModel;
		InnerRichEditDocumentServer server;
		public NativeSnapMailMergeExportOptions(SnapDocumentModel documentModel, InnerRichEditDocumentServer server) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(server, "server");
			this.documentModel = documentModel;
			this.server = server;
			this.sorting = new SnapMailMergeExportSorting();
		}
		protected internal SnapDocumentModel DocumentModel { get { return documentModel; } }
		public int FirstRecordIndex { get { return exportFrom; } set { exportFrom = value; } }
		public int LastRecordIndex {
			get { return exportRecordsCount >= 0 ? exportFrom + exportRecordsCount - 1 : exportRecordsCount; }
			set { exportRecordsCount = value - exportFrom + 1; }
		}
		public bool CopyTemplateStyles { get { return copyTemplateStyles; } set { copyTemplateStyles = value; } }
		public bool HeaderFooterLinkToPrevious { get { return headerFooterLinkToPrevious; } set { headerFooterLinkToPrevious = value; } }
		public bool ProgressIndicationFormVisible { get { return progressIndicationFormVisible; } set { progressIndicationFormVisible = value; } }
		public int ExportFrom { get { return exportFrom; } set { exportFrom = value; } }
		public int ExportRecordsCount { get { return exportRecordsCount; } set { exportRecordsCount = value; } }
		public bool StartEachRecordFromNewParagraph { get { return startEachRecordFromNewParagraph; } set { startEachRecordFromNewParagraph = value; } }
		public object DataSource {
			get { return dataSource; }
			set {
				if (Object.ReferenceEquals(dataSource, value))
					return;
				this.dataSource = value;
				this.dataSourceName = this.documentModel.DataSourceDispatcher.FindDataSourceName(value);
			}
		}
		public string DataSourceName {
			get { return dataSourceName; }
			set {
				if (string.Compare(dataSourceName, value) == 0)
					return;
				dataSourceName = value;
				dataSource = this.documentModel.DataSourceDispatcher.GetDataSource(dataSourceName);
			}
		}
		public string DataMember { get { return dataMember; } set { dataMember = value; } }
		public RecordSeparator RecordSeparator {
			get { return recordSeparator; }
			set {
				if (recordSeparator == value)
					return;
				recordSeparator = value;
				ResetCustomSeparator();
			}
		}
		public SnapDocument CustomSeparator { get { return customSeparator; } }
		void ResetCustomSeparator() {
			if (RecordSeparator == RecordSeparator.Custom) {
				DocumentModel model = DocumentModel.CreateNew();
				this.customSeparator = new SnapNativeDocument((SnapPieceTable)model.MainPieceTable, this.server);
			}
			else
				this.customSeparator = null;
		}
		public string FilterString { get { return filterString; } set { filterString = value; } }
		public SnapListSorting Sorting { get { return sorting; } }
		public void CopyFrom(SnapMailMergeExportOptions value) {
			CopyFromCore(value);
			this.copyTemplateStyles = value.CopyTemplateStyles;
			this.headerFooterLinkToPrevious = value.HeaderFooterLinkToPrevious;
			this.startEachRecordFromNewParagraph = value.StartEachRecordFromNewParagraph;
			this.progressIndicationFormVisible = value.ProgressIndicationFormVisible;
			this.exportFrom = value.ExportFrom;
			this.exportRecordsCount = value.ExportRecordsCount;
			this.RecordSeparator = value.RecordSeparator;
			CopyCustomSeparator(value.CustomSeparator);
		}
		public void CopyFrom(SnapMailMergeVisualOptions value) {
			CopyFromCore(value);
		}
		void CopyFromCore(IDataDispatcherOptions value) {
			this.dataMember = value.DataMember;
			this.dataSource = value.DataSource;
			this.dataSourceName = value.DataSourceName;
			this.filterString = value.FilterString;
			this.sorting.AddRange(value.Sorting);
		}
		void CopyCustomSeparator(SnapDocument sourceDocument) {
			SnapNativeDocument nativeSourceDocument = sourceDocument as SnapNativeDocument;
			if (nativeSourceDocument == null || this.customSeparator == null)
				return;
			SnapPieceTable source = nativeSourceDocument.PieceTable;
			SnapPieceTable target = this.customSeparator.PieceTable;
			DocumentModelCopyManager copyManager = new DocumentModelCopyManager(source, target, ParagraphNumerationCopyOptions.CopyAlways);
			copyManager.TargetPosition.CopyFrom(DocumentModelPosition.FromParagraphStart(target, target.Paragraphs.First.Index));
			CopySectionOperation operation = source.DocumentModel.CreateCopySectionOperation(copyManager);
			operation.FixLastParagraph = true;
			operation.Execute(source.DocumentStartLogPosition, source.DocumentEndLogPosition - source.DocumentStartLogPosition + 1, false);
			target.DocumentModel.Sections.Last.CopyFromCore(source.DocumentModel.Sections.Last);
		}
		void IDataSourceContainer.RegisterDataSource(string dataSourceName, object dataSource) {
			if (object.ReferenceEquals(this.documentModel.DataSourceDispatcher.GetDataSource(dataSourceName), null)) {
				this.documentModel.BeginUpdateDataSource();
				this.documentModel.DataSources.Add(dataSourceName, dataSource);
				this.documentModel.EndUpdateDataSource();
			}
		}
	}
	public class SnapMailMergeExportSorting : List<SnapListGroupParam>, SnapListSorting {
	}
}
