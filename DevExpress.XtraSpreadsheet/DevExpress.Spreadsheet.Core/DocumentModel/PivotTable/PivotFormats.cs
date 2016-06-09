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
using DevExpress.XtraSpreadsheet.Model.History;
using System.Text;
namespace DevExpress.XtraSpreadsheet.Model {
	#region IPivotFormat
	public interface IPivotFormat {
		PivotArea PivotArea { get; }
		FormatAction FormatAction { get; set; }
		IDifferentialFormat DifferentialFormat { get; }
	}
	#endregion
	#region PivotFormat
	public class PivotFormat : SpreadsheetUndoableIndexBasedObject<FormatBase>, IPivotFormat, IDifferentialFormat {
		#region Fields
		readonly PivotArea pivotArea;
		FormatAction formatAction;
		#endregion
		#region Constructors
		public PivotFormat(DocumentModel documentModel, int indexDXF)
			: base(documentModel) {
			this.formatAction = FormatAction.Formatting;
			pivotArea = new PivotArea(documentModel);
			SetIndexInitial(indexDXF);
		}
		public PivotFormat(DocumentModel documentModel)
			: this(documentModel, CellFormatCache.DefaultDifferentialFormatIndex) {
		}
		#endregion
		#region Properties
		public PivotArea PivotArea { get { return pivotArea; } }
		public FormatAction FormatAction { get { return formatAction; } set { SetFormatAction(value); } }
		#endregion
		void SetFormatAction(FormatAction value) {
			if (FormatAction != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, (int)FormatAction, (int)value, SetFormatActionCore);
				DocumentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void SetFormatActionCore(int value) {
			formatAction = (FormatAction)value;
		}
		public override DocumentModelChangeActions GetBatchUpdateChangeActions() {
			return DocumentModelChangeActions.None;
		}
		protected override Office.UniqueItemsCache<FormatBase> GetCache(Office.IDocumentModel documentModel) {
			return DocumentModel.Cache.CellFormatCache;
		}
		public IDifferentialFormat DifferentialFormat {
			get { return this; }
		}
		public IRunFontInfo Font { get { return Info.Font; } }
		public ICellAlignmentInfo Alignment { get { return Info.Alignment; } }
		public IBorderInfo Border { get { return Info.Border; } }
		public IFillInfo Fill { get { return Info.Fill; } }
		public ICellProtectionInfo Protection { get { return Info.Protection; } }
		public string FormatString {
			get { return Info.FormatString; }
			set { SetPropertyValue(SetNumberFormat, value); }
		}
		DocumentModelChangeActions SetNumberFormat(FormatBase info, string value) {
			info.ForceSetFormatString(value);
			return DocumentModelChangeActions.None; 
		}
		public void CopyFromNoHistory(PivotFormat source) {
			CopyFrom(source);
			formatAction = source.formatAction;
		}
	}
	#endregion
	#region PivotConditionalFormat
	public class PivotConditionalFormat {
		#region Fields
		int priority;
		ConditionalFormatScope scope;
		ConditionalFormatType type;
		readonly DocumentModel documentModel;
		readonly PivotAreaCollection pivotAreas;
		#endregion
		#region Conctructors
		public PivotConditionalFormat(DocumentModel documentModel) {
			this.documentModel = documentModel;
			scope = ConditionalFormatScope.Selection;
			type = ConditionalFormatType.None;
			pivotAreas = new PivotAreaCollection(documentModel);
		}
		#endregion
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public int Priority { get { return priority; } set { SetPriority(value); } }
		public ConditionalFormatScope Scope { get { return scope; } set { SetScope(value); } }
		public ConditionalFormatType Type { get { return type; } set { SetType(value); } }
		public PivotAreaCollection PivotAreas { get { return pivotAreas; } }
		#endregion
		void SetPriority(int value) {
			if (Priority != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, Priority, value, SetPriorityCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetScope(ConditionalFormatScope value) {
			if (Scope != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, (int)Scope, (int)value, SetScopeCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetType(ConditionalFormatType value) {
			if (Type != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, (int)Type, (int)value, SetTypeCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void SetPriorityCore(int value) {
			priority = value;
		}
		protected internal void SetScopeCore(int value) {
			scope = (ConditionalFormatScope)value;
		}
		protected internal void SetTypeCore(int value){
			type = (ConditionalFormatType)value;
		}
		public PivotConditionalFormat CreateClone(DocumentModel anotherModel, Worksheet newWorksheet, CellPositionOffset rangeOffset) {
			var result = new PivotConditionalFormat(anotherModel);
			result.priority = this.priority;
			result.scope = this.scope;
			result.type = this.type;
			foreach (PivotArea sourcePivotArea in this.PivotAreas.InnerList) {
				var targetPivotArea = sourcePivotArea.Clone(anotherModel, newWorksheet, rangeOffset);
				result.pivotAreas.InnerList.Add(targetPivotArea);
			}
			return result;
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotConditionalFormat source) {
			priority = source.priority;
			scope = source.scope;
			type = source.type;
			pivotAreas.CopyFromNoHistory(newPivot, offset, source.pivotAreas);
		}
	}
	#endregion
	#region PivotChartFormat
	public class PivotChartFormat {
		#region Fields
		readonly DocumentModel documentModel;
		readonly PivotArea pivotArea;
		int chart;
		int format;
		bool series;
		#endregion
		#region Constructor
		public PivotChartFormat(DocumentModel documentModel) {
			this.documentModel = documentModel;
			pivotArea = new PivotArea(documentModel);
		}
		#endregion
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } }
		public PivotArea PivotArea { get { return pivotArea; } }
		public int ChartIndex { get { return chart;} set{ SetChartIndex(value);}}
		public int PivotFormatId { get {return format;} set { SetPivotFormatId(value); } }
		public bool SeriesFormat { get { return series; } set { SetSeriesFormat(value); } }
		#endregion
		void SetChartIndex(int value) {
			if (ChartIndex != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, ChartIndex, value, SetChartIndexCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetPivotFormatId(int value) {
			if (PivotFormatId != value) {
				ActionHistoryItem<int> historyItem = new ActionHistoryItem<int>(DocumentModel, PivotFormatId, value, SetPivotFormatIdCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		void SetSeriesFormat(bool value) {
			if (SeriesFormat != value) {
				ActionHistoryItem<bool> historyItem = new ActionHistoryItem<bool>(DocumentModel, SeriesFormat, value, SetSeriesFormatCore);
				documentModel.History.Add(historyItem);
				historyItem.Execute();
			}
		}
		protected internal void SetChartIndexCore(int value) {
			chart = value;
		}
		protected internal void SetPivotFormatIdCore(int value) {
			format = value;
		}
		protected internal void SetSeriesFormatCore(bool value) {
			series = value;
		}
		public PivotChartFormat Clone(DocumentModel anotherModel) {
			PivotChartFormat result = new PivotChartFormat(anotherModel);
			result.chart = this.chart;
			result.format = this.format;
			result.series = this.series;
			return result;
		}
		public void CopyFromNoHistory(PivotTable newPivot, CellPositionOffset offset, PivotChartFormat source) {
			pivotArea.CopyFromNoHistory(newPivot, offset, source.pivotArea);
			chart = source.chart;
			format = source.format;
			series = source.series;
		}
	}
	#endregion
	#region FormatAction
	public enum FormatAction {
		Blank,
		Formatting,
		Drill,
		Formula
	}
	#endregion
	#region ConditionalFormatScope
	public enum ConditionalFormatScope{
		Data,
		Field,
		Selection
	}
	#endregion
	#region ConditionalFormatType
	public enum ConditionalFormatType { 
		All,
		Column,
		None,
		Row
	}
	#endregion
}
