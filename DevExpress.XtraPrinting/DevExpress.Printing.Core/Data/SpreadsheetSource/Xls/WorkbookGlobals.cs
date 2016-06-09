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
using System.Linq;
using System.Text;
using DevExpress.Export.Xl;
using DevExpress.SpreadsheetSource;
using DevExpress.SpreadsheetSource.Implementation;
namespace DevExpress.SpreadsheetSource.Xls {
	using DevExpress.Office.Utils;
	using DevExpress.XtraExport.Xls;
	using DevExpress.Utils;
	#region XlsSpreadsheetSource
	public partial class XlsSpreadsheetSource {
		#region Fields
		readonly Stack<XlsContentType> contentTypes = new Stack<XlsContentType>();
		readonly Stack<IXlsSourceDataCollector> dataCollectors = new Stack<IXlsSourceDataCollector>();
		readonly List<string> sheetNames = new List<string>();
		readonly List<XlsXTI> externSheets = new List<XlsXTI>();
		int supBookCount = 0;
		int selfRefBookIndex = Int32.MinValue;
		#endregion
		#region Properties
		internal XlsContentType ContentType {
			get {
				if(contentTypes.Count == 0)
					return XlsContentType.None;
				return contentTypes.Peek();
			}
		}
		internal bool IsDate1904 { get; set; }
		internal IXlsSourceDataCollector DataCollector {
			get {
				if(dataCollectors.Count == 0)
					return null;
				return dataCollectors.Peek();
			}
		}
		internal List<string> SheetNames { get { return sheetNames; } }
		internal List<XlsXTI> ExternSheets { get { return externSheets; } }
		internal int SupBookCount { get { return supBookCount; } set { supBookCount = value; } }
		internal int SelfRefBookIndex { get { return selfRefBookIndex; } set { selfRefBookIndex = value; } }
		#endregion
		void ReadWorkbookGlobals() {
			while(WorkbookReader.Position != WorkbookReader.StreamLength) {
				XlsSourceCommandBOF beginOfSubstream = commandFactory.CreateCommand(WorkbookReader) as XlsSourceCommandBOF;
				if(beginOfSubstream == null)
					continue;
				beginOfSubstream.Read(WorkbookReader, this);
				beginOfSubstream.Execute(this);
				while(WorkbookReader.Position != WorkbookReader.StreamLength) {
					IXlsSourceCommand command = commandFactory.CreateCommand(WorkbookReader);
					command.Read(WorkbookReader, this);
					command.Execute(this);
					if(ContentType == XlsContentType.None) return;
				}
			}
			this.contentTypes.Clear();
		}
		void ReadTableDefinitions() {
			foreach(IWorksheet worksheet in InnerWorksheets)
				ReadTableDefinitions(worksheet as XlsWorksheet);
		}
		void ReadTableDefinitions(XlsWorksheet worksheet) {
			if(worksheet == null)
				return;
			WorkbookReader.Position = worksheet.StartPosition;
			XlsSourceCommandBOF beginOfSubstream = commandFactory.CreateCommand(WorkbookReader) as XlsSourceCommandBOF;
			if(beginOfSubstream == null)
				return;
			beginOfSubstream.Read(WorkbookReader, this);
			beginOfSubstream.Execute(this);
			currentSheet = worksheet;
			try {
				while(WorkbookReader.Position != WorkbookReader.StreamLength) {
					IXlsSourceCommand command = commandFactory.CreateCommand(WorkbookReader);
					if(command is XlsSourceCommandFeature11 || command.IsSubstreamBound) {
						command.Read(WorkbookReader, this);
						command.Execute(this);
					}
					else {
						command = commandFactory.CreateCommand(0);
						command.Read(WorkbookReader, this);
					}
					if(ContentType == XlsContentType.None) return;
				}
			}
			finally {
				currentSheet = null;
			}
		}
		#region ContentType methods
		internal void StartContent(XlsContentType contentType) {
			contentTypes.Push(contentType);
		}
		internal void StartContent(XlsSubstreamType substreamType) {
			StartContent(GetContentTypeBySubstreamType(substreamType));
		}
		internal void EndContent() {
			contentTypes.Pop();
		}
		XlsContentType GetContentTypeBySubstreamType(XlsSubstreamType dataType) {
			switch(dataType) {
				case XlsSubstreamType.WorkbookGlobals:
					return XlsContentType.WorkbookGlobals;
				case XlsSubstreamType.Sheet:
					return XlsContentType.Sheet;
				case XlsSubstreamType.MacroSheet:
					return XlsContentType.MacroSheet;
				case XlsSubstreamType.VisualBasicModule:
					return XlsContentType.VisualBasicModule;
				case XlsSubstreamType.Workspace:
					return XlsContentType.Workspace;
				case XlsSubstreamType.Chart:
					if(ContentType == XlsContentType.Sheet)
						return XlsContentType.Chart;
					return XlsContentType.ChartSheet;
			}
			return XlsContentType.None;
		}
		#endregion
		#region DataCollectors
		internal void PushDataCollector(IXlsSourceDataCollector collector) {
			Guard.ArgumentNotNull(collector, "collector");
			dataCollectors.Push(collector);
		}
		internal void PopDataCollector() {
			if(dataCollectors.Count > 0)
				dataCollectors.Pop();
		}
		internal void ClearDataCollectors() {
			dataCollectors.Clear();
		}
		#endregion
	}
	#endregion
	#region XlsWorksheet
	internal class XlsWorksheet : Worksheet {
		public XlsWorksheet(string name, XlSheetVisibleState visibleState, int startPosition)
			: base(name, visibleState) {
			StartPosition = startPosition;
		}
		public int StartPosition { get; private set; }
	}
	#endregion
}
