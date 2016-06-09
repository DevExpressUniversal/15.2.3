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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Web.ASPxRichEdit.Export;
namespace DevExpress.Web.ASPxRichEdit.Internal {
	public abstract class WebRichEditCommand {
		DocumentModel documentModel;
		protected WebRichEditCommand(CommandManager commandManager, Hashtable parameters) {
			Initialize(commandManager, parameters);
		}
		protected WebRichEditCommand(CommandManager commandManager, Hashtable parameters, DocumentModel documentModel) {
			this.documentModel = documentModel;
			Initialize(commandManager, parameters);
		}
		void Initialize(CommandManager commandManager, Hashtable parameters) {
			Manager = commandManager;
			Parameters = (Hashtable)parameters["serverParams"] ?? new Hashtable();
			IncrementalId = Convert.ToInt32(parameters["incId"]);
			if(parameters.ContainsKey("editIncId"))
				EditIncrementalId = Convert.ToInt32(parameters["editIncId"]);
			if(parameters.ContainsKey("fiIndex"))
				ClientFontInfoCacheLength = Convert.ToInt32(parameters["fiIndex"]);
		}
		public abstract CommandType Type { get; }
		protected abstract bool IsEnabled();
		protected abstract void ExecuteInternal(Hashtable result);
		public bool IsModification {
			get { return (int)Type < 0; }
		}
		protected CommandManager Manager { get; private set; }
		protected Hashtable Parameters { get; private set; }
		protected DocumentModel DocumentModel {
			get { return this.documentModel ?? Manager.WorkSession.RichEdit.DocumentModel; }
		}
		public int IncrementalId { get; private set; }
		public int? EditIncrementalId { get; private set; }
		protected internal int? ClientFontInfoCacheLength { get; set; }
		protected WorkSessionClient Client { get { return Manager.Client; } }
		protected RichEditWorkSession WorkSession { get { return Manager.WorkSession; } }
		protected internal Hashtable Execute() {
			Hashtable result = new Hashtable() {
				{ "type", (int)Type },
				{ "incId", IncrementalId }
			};
			if(IsEnabled())
				ExecuteInternal(result);
			return result;
		}
		static protected string RestoreTextFromRequest(string text) {
			return text
				.Replace("&lt;", "<")
				.Replace("&gt;", ">")
				.Replace("&quot;", @"""")
				.Replace("&amp;", "&");
		}
	}
	public abstract class WebRichEditLoadModelCommandBase : WebRichEditCommand {
		HashSet<PieceTable> requiredPieceTables;
		protected WebRichEditLoadModelCommandBase(CommandManager commandManager, Hashtable parameters)
			: base(commandManager, parameters) { }
		protected WebRichEditLoadModelCommandBase(CommandManager commandManager, Hashtable parameters, DocumentModel documentModel)
			: base(commandManager, parameters, documentModel) { }
		protected WebRichEditLoadModelCommandBase(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters)
			: base(parentCommand.Manager, parameters) {
			ParentCommand = parentCommand;
		}
		protected WebRichEditLoadModelCommandBase(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters, DocumentModel documentModel)
			: base(parentCommand.Manager, parameters, documentModel) {
			ParentCommand = parentCommand;
		}
		protected abstract void FillHashtable(Hashtable result);
		protected HashSet<PieceTable> RequiredPieceTables { 
			get {
				if(ParentCommand != null)
					return ParentCommand.RequiredPieceTables;
				if(requiredPieceTables == null)
					requiredPieceTables = new HashSet<PieceTable>();
				return requiredPieceTables;
			}
		}
		protected WebRichEditLoadModelCommandBase ParentCommand { get; private set; }
		protected override void ExecuteInternal(Hashtable result) {
			FillHashtable(result);
			if(ParentCommand == null) {
				if(RequiredPieceTables.Any()) {
					var subDocuments = new Hashtable();
					foreach(var pieceTable in RequiredPieceTables) {
						int docLength = pieceTable.DocumentEndLogPosition - DocumentLogPosition.Zero + 1;
						subDocuments.Add(pieceTable.Id, new ArrayList() { (int)GetDocumentInfoType(pieceTable), new LoadPieceTableCommand(this, pieceTable, docLength).Execute() });
					}
					result["subDocuments"] = subDocuments;
				}
				if(ClientFontInfoCacheLength.HasValue)
					WorkSession.FontInfoCache.FillHashtable(result, ClientFontInfoCacheLength.Value);
			}
		}
		protected internal int LoadAdditionalPieceTableOnClient(PieceTable pieceTable) {
			if(ParentCommand != null)
				return ParentCommand.LoadAdditionalPieceTableOnClient(pieceTable);
			if(!Client.LoadedPieceTableIds.Contains(pieceTable.Id)) {
				RequiredPieceTables.Add(pieceTable);
				Client.LoadedPieceTableIds.Add(pieceTable.Id);
			}
			return pieceTable.Id;
		}
		SubDocumentInfoType GetDocumentInfoType(PieceTable pieceTable) {
			if(pieceTable.ContentType.GetType() == typeof(SectionHeader))
				return SubDocumentInfoType.Header;
			if(pieceTable.ContentType.GetType() == typeof(SectionFooter))
				return SubDocumentInfoType.Footer;
			throw new NotImplementedException();
		}
	}
	public abstract class WebRichEditLoadPieceTableCommandBase : WebRichEditLoadModelCommandBase {
		protected WebRichEditLoadPieceTableCommandBase(CommandManager commandManager, Hashtable parameters)
			: base(commandManager, parameters) {
			var pieceTableID = (int)Parameters["sdid"];
			PieceTable = DocumentModel.GetPieceTables(true).Find(pt => pt.Id == pieceTableID);
		}
		protected WebRichEditLoadPieceTableCommandBase(CommandManager commandManager, Hashtable parameters, PieceTable pieceTable)
			: base(commandManager, parameters, pieceTable.DocumentModel) {
			PieceTable = pieceTable;
		}
		protected WebRichEditLoadPieceTableCommandBase(WebRichEditLoadModelCommandBase parentCommand, Hashtable parameters, PieceTable pieceTable)
			: base(parentCommand, parameters, pieceTable.DocumentModel) {
			PieceTable = pieceTable;
		}
		protected PieceTable PieceTable { get; private set; }
	}
	public abstract class WebRichEditUpdateModelCommandBase : WebRichEditCommand {
		protected WebRichEditUpdateModelCommandBase(CommandManager commandManager, Hashtable parameters)
			: base(commandManager, parameters) { }
		protected override void ExecuteInternal(Hashtable result) {
			if(EditIncrementalId > DocumentModel.LastExecutedEditCommandId) {
				if(!IsValidOperation())
					throw new AccessViolationException("Operation is disabled by the server-side settings");
				DocumentModel.BeginUpdate();
				try {
					PerformModifyModel();
				}
				finally {
					DocumentModel.EndUpdate();
				}
				DocumentModel.LastExecutedEditCommandId = EditIncrementalId.Value;
			}
		}
		protected virtual bool IsValidOperation() { return true; } 
		protected abstract void PerformModifyModel();
		PieceTable pieceTable;
		protected PieceTable PieceTable {
			get { 
				if(pieceTable == null) {
					var pieceTableId = (int)Parameters["sdid"];
					pieceTable = DocumentModel.GetPieceTables(true).Find(pt => pt.Id == pieceTableId);
				}
				return pieceTable;
			}
		}
		protected void ApplyParagraphStyle(Paragraph paragraph) {
			string styleName = (string)Parameters["styleName"];
			if(styleName != null)
				paragraph.ParagraphStyleIndex = DocumentModel.ParagraphStyles.GetStyleIndexByName(styleName);
		}
		protected void ApplyParagraphFormatting(Paragraph paragraph) {
			if(Parameters.ContainsKey("paragraphProperties"))
				ApplyHashtableToParagraphProperties(paragraph.PieceTable, paragraph.ParagraphProperties, (Hashtable)Parameters["paragraphProperties"]);
			else if(Parameters.ContainsKey("paragraphPropertiesIndex"))
				paragraph.ParagraphProperties.CopyFrom(DocumentModel.Cache.ParagraphFormattingCache[(int)Parameters["paragraphPropertiesIndex"]]);
		}
		protected void ApplySectionProperties(Section section) {
			Hashtable sectionProperties = Parameters["sectionProperties"] as Hashtable;
			if(sectionProperties != null) {
				WebSectionPropertiesExporter exporter = new WebSectionPropertiesExporter();
				exporter.RestoreInfo(sectionProperties, section);
			}
		}
		protected void CopyCharacterPropertiesFromHashtable(CharacterFormattingBase properties, Hashtable ht) {
			CharacterFormattingBaseExporter exporter = new CharacterFormattingBaseExporter(DocumentModel, WorkSession.FontInfoCache);
			exporter.RestoreInfo(ht, properties);
		}
		protected void ApplyHashtableToParagraphProperties(PieceTable pieceTable, ParagraphProperties properties, Hashtable ht) {
			ParagraphFormattingBase paragraphFormattingBase = new ParagraphFormattingBase(pieceTable, DocumentModel, ParagraphFormattingInfoCache.DefaultItemIndex,
				ParagraphFormattingOptionsCache.EmptyParagraphFormattingOptionIndex);
			ParagraphFormattingBaseExporter exporter = new ParagraphFormattingBaseExporter(DocumentModel);
			exporter.RestoreInfo(ht, paragraphFormattingBase);
			properties.SetIndexInitial(DocumentModel.Cache.ParagraphFormattingCache.AddItem(paragraphFormattingBase));
		}
		protected void ApplyHashtableToCharacterProperties(PieceTable pieceTable, CharacterProperties properties, Hashtable ht) {
			CharacterFormattingBase characterFormattingBase = new CharacterFormattingBase(pieceTable, DocumentModel, CharacterFormattingInfoCache.DefaultItemIndex,
				CharacterFormattingOptionsCache.EmptyCharacterFormattingOptionIndex);
			CopyCharacterPropertiesFromHashtable(characterFormattingBase, ht);
			properties.SetIndexInitial(DocumentModel.Cache.CharacterFormattingCache.AddItem(characterFormattingBase));
		}
		protected void ApplyHashtableToListLevelProperties(ListLevelProperties properties, Hashtable ht) {
			ListLevelPropertiesExporter exporter = new ListLevelPropertiesExporter();
			exporter.RestoreInfo(ht, properties);
		}
		protected void ApplyTableCellFormatting(TableCell cell, Hashtable clientCell) {
			cell.Properties.ColumnSpan = (int)clientCell["columnSpan"];
			cell.Properties.CellConditionalFormatting = (ConditionalTableStyleFormattingTypes)clientCell["conditionalFormatting"];
			TableCellPropertiesExporter.ConvertFromHashtable((Hashtable)clientCell["properties"], cell.Properties);
			WidthUnitExporter.FromHashtable((Hashtable)clientCell["preferredWidth"], cell.Properties.PreferredWidth);
			if (clientCell["styleName"] != null)
				cell.SetTableCellStyleIndexCore(PieceTable.DocumentModel.TableCellStyles.IndexOf(PieceTable.DocumentModel.TableCellStyles.GetStyleByName((string)clientCell["styleName"])));
			cell.Properties.VerticalMerging = (MergingState)clientCell["verticalMerging"];
		}
		protected void ApplyTableRowFormatting(TableRow row, Hashtable clientRow) {
			row.Properties.GridAfter = (int)clientRow["gridAfter"];
			row.Properties.GridBefore = (int)clientRow["gridBefore"];
			TableRowPropertiesExporter.ConvertFromHashtable((Hashtable)clientRow["properties"], row.Properties);
			HeightUnitExporter.FromHashtable((Hashtable)clientRow["height"], row.Properties.Height);
			TablePropertiesExporter.ConvertFromHashtable((Hashtable)clientRow["tablePropertiesException"], row.TablePropertiesException);
			WidthUnitExporter.FromHashtable((Hashtable)clientRow["widthAfter"], row.Properties.WidthAfter);
			WidthUnitExporter.FromHashtable((Hashtable)clientRow["widthBefore"], row.Properties.WidthBefore);
		}
		protected void ApplyTableFormatting(Table table, Hashtable clientTable) {
			TablePropertiesExporter.ConvertFromHashtable((Hashtable)clientTable["properties"], table.TableProperties);
			if (clientTable["styleName"] != null)
				table.SetTableStyleIndexCore(PieceTable.DocumentModel.TableStyles.IndexOf(PieceTable.DocumentModel.TableStyles.GetStyleByName((string)clientTable["styleName"])));
			WidthUnitExporter.FromHashtable((Hashtable)clientTable["preferredWidth"], table.TableProperties.PreferredWidth);
			table.TableProperties.TableLook = (TableLookTypes)clientTable["lookTypes"];
		}
		protected Table GetTableByPositionInfo(ArrayList tablePosition) {
			var paragraphIndex = PieceTable.FindParagraphIndex(new DocumentLogPosition((int)tablePosition[0]));
			return PieceTable.TableCellsManager.GetCellByNestingLevel(paragraphIndex, (int)tablePosition[1]).Table;
		}
		protected TableCell CreateTableCell(TableRow row, int newIndex, int startPosition, int endPosition, bool shiftNextCell) {
			var newCell = new TableCell(row);
			row.Cells.AddCellCore(newIndex, newCell);
			var startParagraphIndex = PieceTable.FindParagraphIndex(new DocumentLogPosition(startPosition));
			var endParagraphIndex = PieceTable.FindParagraphIndex(new DocumentLogPosition(endPosition) - 1);
			if (shiftNextCell) {
				var nextCell = newCell.Next;
				if (nextCell != null)
					PieceTable.ChangeCellStartParagraphIndex(nextCell, endParagraphIndex + 1);
			}
			PieceTable.TableCellsManager.InitializeTableCell(newCell, startParagraphIndex, endParagraphIndex);
			return newCell;
		}
		protected void RemoveTableCell(TableCell cell) {
			var row = cell.Row;
			PieceTable.TableCellsManager.RemoveTableCell(cell);
			row.Cells.DeleteInternal(cell);
		}
	}
	public abstract class WebRichEditStateBasedCommand<T> : WebRichEditUpdateModelCommandBase where T : ICommandState, new() {
		protected WebRichEditStateBasedCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModel() {
			foreach(var stateObject in GetStateObjects())
				PerformModifyModelCore(stateObject);
		}
		IEnumerable<T> GetStateObjects() {
			ArrayList stateList = (ArrayList)Parameters["state"];
			foreach(ArrayList item in stateList) {
				T stateObject = new T();
				stateObject.Load(item);
				yield return stateObject;
			}
		}
		protected abstract void PerformModifyModelCore(T stateObject);
	}
	public abstract class WebRichEditPropertyStateBasedCommand<T, TProperty> : WebRichEditStateBasedCommand<T> where T : ICommandState, new() {
		protected WebRichEditPropertyStateBasedCommand(CommandManager commandManager, Hashtable parameters) : base(commandManager, parameters) { }
		protected override void PerformModifyModelCore(T stateObject) {
			TProperty property = (TProperty)Parameters["property"];
			IModelModifier<T> modifier = CreateModifier(property);
			if(modifier == null)
				return;
			modifier.Modify(stateObject);
		}
		protected abstract IModelModifier<T> CreateModifier(TProperty property);
	}
	public class RequestSortingComparer : IComparer {
		public int Compare(object x, object y) {
			var xId = (int)((Hashtable)x)["incId"];
			var yId = (int)((Hashtable)y)["incId"];
			return Comparer<int>.Default.Compare(xId, yId);
		}
	}
	public delegate IModelModifier<T> JSONPieceTableModifier<T>(PieceTable pieceTable) where T : ICommandState, new();
	public delegate IModelModifier<T> JSONModelModifier<T>(DocumentModel model) where T : ICommandState, new();
}
