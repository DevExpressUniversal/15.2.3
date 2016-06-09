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

using System.Collections.Generic;
using System.Diagnostics;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraSpreadsheet.Model.CopyOperation;
using DevExpress.Spreadsheet;
namespace DevExpress.XtraSpreadsheet.Model {
	public partial class DocumentModel {
		public DocumentModel Clone() {
			return Clone(true);
		}
		public DocumentModel Clone(bool copyFormulas) {
			ModelPasteSpecialFlags flags = ModelPasteSpecialFlags.All;
			if (!copyFormulas)
				flags &= ~ModelPasteSpecialFlags.Formulas;
			return this.Clone(flags, -1);
		}
		public DocumentModel Clone(int sheetId) {
			return this.Clone(ModelPasteSpecialFlags.All, sheetId);
		}
		DocumentModel Clone(ModelPasteSpecialFlags flags, int sourceSheetId) {
			DocumentModel newDocumentModel = new DocumentModel();
			try {
				newDocumentModel.History.DisableHistory();
				newDocumentModel.BeginSetContent(false);
				newDocumentModel.SuppressCellValueAssignment = false;
				CloneCore(newDocumentModel, flags, sourceSheetId);
			}
			finally {
				Debug.Assert(newDocumentModel.IsUpdateLocked);
				newDocumentModel.SuppressCellValueAssignment = true;
				if (new ModelPasteSpecialOptions(flags).ShouldCopyFormulas)
					newDocumentModel.RecalculateAfterLoad = true;
				newDocumentModel.History.EnableHistory();
				newDocumentModel.EndSetContent(DocumentModelChangeType.LoadNewDocument);
			}
			return newDocumentModel;
		}
		void CloneCore(DocumentModel newDocumentModel, ModelPasteSpecialFlags flags, int sourceSheetId) {
			CopyZeroDefaultCacheItems(newDocumentModel);
			newDocumentModel.OfficeTheme.CopyFrom(this.OfficeTheme);
			newDocumentModel.SheetDefinitions.CopyFrom(this.SheetDefinitions);
			newDocumentModel.BehaviorOptions.CopyFrom(this.BehaviorOptions);
			newDocumentModel.ProtectionOptions.CopyFrom(this.ProtectionOptions);
			newDocumentModel.DocumentCoreProperties.CopyFrom(this.DocumentCoreProperties);
			newDocumentModel.DocumentApplicationProperties.CopyFrom(this.DocumentApplicationProperties);
			newDocumentModel.DocumentCustomProperties.CopyFrom(this.DocumentCustomProperties);
			newDocumentModel.checkForCycleReferences = this.checkForCycleReferences;
			newDocumentModel.CommentAuthors.CopyFrom(this.CommentAuthors);
			newDocumentModel.connectionsContent = this.ConnectionsContent;
			newDocumentModel.contentVersion = this.ContentVersion; 
			newDocumentModel.culture = this.Culture;
			newDocumentModel.currentAuthor = this.CurrentAuthor;
			CopyDefinedNames(newDocumentModel);
			newDocumentModel.DocumentCapabilities.CopyFrom(this.DocumentCapabilities);
			newDocumentModel.DocumentExportOptions.CopyFrom(this.DocumentExportOptions);
			newDocumentModel.DocumentImportOptions.CopyFrom(this.DocumentImportOptions);
			newDocumentModel.DocumentSaveOptions.CopyFrom(this.DocumentSaveOptions);
			newDocumentModel.ViewOptions.CopyFrom(this.ViewOptions);
			newDocumentModel.ExternalLinks.CopyFrom(this.ExternalLinks, this);
			newDocumentModel.MailOptions.CopyFrom(this.MailOptions);
			newDocumentModel.Properties.CopyFrom(this.Properties);
			newDocumentModel.QueryTablesContent.AddRange(this.QueryTablesContent); 
			newDocumentModel.StyleSheet.CopyFrom(this.StyleSheet);
			newDocumentModel.customFunctionProvider.CopyFrom(CustomFunctionProvider);
			newDocumentModel.builtInOverridesFunctionProvider.CopyFrom(BuiltInOverridesFunctionProvider);
			if (sourceSheetId == -1)
				CopyWorksheets(newDocumentModel, flags);
			else {
				Worksheet sourceSheet = this.sheets.GetById(sourceSheetId);
				List<IWorksheet> targetSheets = new List<IWorksheet>();
				Worksheet targetWorksheet = newDocumentModel.CreateWorksheet(sourceSheet.Name);
				newDocumentModel.sheets.Add(targetWorksheet);
				targetSheets.Add(targetWorksheet);
				CopySingleWorksheet(newDocumentModel, flags, targetSheets, sourceSheet);
			}
			CopyKnownServices(newDocumentModel);
		}
		protected internal void CopyZeroDefaultCacheItems(DocumentModel newDocumentModel) {
			DocumentCache newCache = newDocumentModel.Cache;
			DocumentCache sourceCache = Cache;
			newDocumentModel.BeginUpdate();
			try {
				newCache.CellAlignmentInfoCache.DefaultItem.CopyFrom(sourceCache.CellAlignmentInfoCache.DefaultItem);
				newCache.ColorModelInfoCache.DefaultItem.CopyFrom(sourceCache.ColorModelInfoCache.DefaultItem);
				newCache.BorderInfoCache.DefaultItem.CopyFrom(sourceCache.BorderInfoCache.DefaultItem);
				newCache.FontInfoCache.DefaultItem.CopyFrom(sourceCache.FontInfoCache.DefaultItem);
				newCache.FillInfoCache.DefaultItem.CopyFrom(sourceCache.FillInfoCache.DefaultItem);
				newCache.NumberFormatCache.DefaultItem.CopyFrom(sourceCache.NumberFormatCache.DefaultItem);
				newCache.CellFormatCache.DefaultItem.CopyFrom(sourceCache.CellFormatCache.DefaultItem);
				newCache.MarginInfoCache.DefaultItem.CopyFrom(sourceCache.MarginInfoCache.DefaultItem);
				newCache.PrintSetupInfoCache.DefaultItem.CopyFrom(sourceCache.PrintSetupInfoCache.DefaultItem);
				newCache.AutoFilterColumnInfoCache.DefaultItem.CopyFrom(sourceCache.AutoFilterColumnInfoCache.DefaultItem);
				newCache.SortStateInfoCache.DefaultItem.CopyFrom(sourceCache.SortStateInfoCache.DefaultItem);
				newCache.SheetFormatInfoCache.DefaultItem.CopyFrom(sourceCache.SheetFormatInfoCache.DefaultItem);
				newCache.SortConditionInfoCache.DefaultItem.CopyFrom(sourceCache.SortConditionInfoCache.DefaultItem);
				newCache.TableInfoCache.DefaultItem.CopyFrom(sourceCache.TableInfoCache.DefaultItem);
				newCache.VmlShapeInfoCache.DefaultItem.CopyFrom(sourceCache.VmlShapeInfoCache.DefaultItem);
				newCache.DateGroupingInfoCache.DefaultItem.CopyFrom(sourceCache.DateGroupingInfoCache.DefaultItem);
				newCache.WindowInfoCache.DefaultItem.CopyFrom(sourceCache.WindowInfoCache.DefaultItem);
				newCache.WorksheetProtectionInfoCache.DefaultItem.CopyFrom(sourceCache.WorksheetProtectionInfoCache.DefaultItem);
				newCache.WorksheetProtectionInfoCache.SchemaDefaultItem.CopyFrom(sourceCache.WorksheetProtectionInfoCache.SchemaDefaultItem);
				newCache.DataValidationInfoCache.DefaultItem.CopyFrom(sourceCache.DataValidationInfoCache.DefaultItem);
				newCache.CalculationOptionsInfoCache.DefaultItem.CopyFrom(sourceCache.CalculationOptionsInfoCache.DefaultItem);
				newCache.TableStyleElementFormatCache.DefaultItem.CopyFrom(sourceCache.TableStyleElementFormatCache.DefaultItem);
				newCache.ConditionalFormattingValueCache.DefaultItem.CopyFrom(sourceCache.ConditionalFormattingValueCache.DefaultItem);
				newCache.ConditionalFormattingInfoCache.DefaultItem.CopyFrom(sourceCache.ConditionalFormattingInfoCache.DefaultItem);
				newCache.DrawingObjectInfoCache.DefaultItem.CopyFrom(sourceCache.DrawingObjectInfoCache.DefaultItem);
				newCache.PictureInfoCache.DefaultItem.CopyFrom(sourceCache.PictureInfoCache.DefaultItem);
				newCache.Transform2DInfoCache.DefaultItem.CopyFrom(sourceCache.Transform2DInfoCache.DefaultItem);
				newCache.ShapeStyleInfoCache.DefaultItem.CopyFrom(sourceCache.ShapeStyleInfoCache.DefaultItem);
				newCache.ShapePropertiesInfoCache.DefaultItem.CopyFrom(sourceCache.ShapePropertiesInfoCache.DefaultItem);
			}
			finally {
				newDocumentModel.EndUpdate();
			}
		}
		void CopyKnownServices(DocumentModel newDocumentModel) {
			Debug.Assert(newDocumentModel.GetService<IColumnWidthCalculationService>() != null);
			Debug.Assert(newDocumentModel.GetService<DevExpress.XtraSpreadsheet.Services.IWorksheetNameCreationService>() != null);
			newDocumentModel.ReplaceService<DevExpress.XtraSpreadsheet.Internal.IDocumentImportManagerService>(this.GetService<DevExpress.XtraSpreadsheet.Internal.IDocumentImportManagerService>());
			newDocumentModel.ReplaceService<DevExpress.XtraSpreadsheet.Internal.IDocumentExportManagerService>(this.GetService<DevExpress.XtraSpreadsheet.Internal.IDocumentExportManagerService>());
			newDocumentModel.ReplaceService<DevExpress.Office.Services.IUriStreamService>(this.GetService<DevExpress.Office.Services.IUriStreamService>());
			newDocumentModel.ReplaceService<DevExpress.Office.Services.Implementation.IThreadPoolService>(this.GetService<DevExpress.Office.Services.Implementation.IThreadPoolService>());
			newDocumentModel.ReplaceService<DevExpress.Office.Services.ILogService>(this.GetService<DevExpress.Office.Services.ILogService>());
			newDocumentModel.ReplaceService<IColumnWidthCalculationService>(this.GetService<IColumnWidthCalculationService>());
			newDocumentModel.ReplaceService<ITableNameCreationService>(this.GetService<ITableNameCreationService>());
			newDocumentModel.ReplaceService<IPivotTableNameCreationService>(this.GetService<IPivotTableNameCreationService>());
			newDocumentModel.ReplaceService<IPivotCacheFieldNameCreationService>(this.GetService<IPivotCacheFieldNameCreationService>());
			newDocumentModel.ReplaceService<ITableStyleNameCreationService>(this.GetService<ITableStyleNameCreationService>());
			newDocumentModel.ReplaceService<IPivotStyleNameCreationService>(this.GetService<IPivotStyleNameCreationService>());
			newDocumentModel.ReplaceService<ICellStyleNameCreationService>(this.GetService<ICellStyleNameCreationService>());
			newDocumentModel.ReplaceService<IChartControllerFactoryService>(this.GetService<IChartControllerFactoryService>());
			newDocumentModel.ReplaceService<IChartImageService>(this.GetService<IChartImageService>());
			newDocumentModel.ReplaceService<IPictureImageService>(this.GetService<IPictureImageService>());
#if !DXPORTABLE
			newDocumentModel.ReplaceService(GetService<IShapeRenderService>());
#endif
		}
		#region DefinedNames
		protected internal void CopyDefinedNames(DocumentModel newDocumentModel) {
			foreach (DefinedName sourceDefinedName in definedNames) {
				ParsedExpression targetDefinedNameExpression = sourceDefinedName.Expression;
				DefinedName newTargetDefinedName = newDocumentModel.CreateDefinedName(sourceDefinedName.Name, targetDefinedNameExpression);
				newTargetDefinedName.Comment = sourceDefinedName.Comment;
				newTargetDefinedName.IsHidden = sourceDefinedName.IsHidden;
			}
		}
#endregion
		void CopyWorksheets(DocumentModel newDocumentModel, ModelPasteSpecialFlags flags) {
			List<IWorksheet> targetWorksheets = new List<IWorksheet>();
			foreach (Worksheet sourceWorksheet in this.Sheets) {
				Worksheet targetWorksheet = newDocumentModel.CreateWorksheet(sourceWorksheet.Name);
				newDocumentModel.Sheets.Add(targetWorksheet);
				targetWorksheets.Add(targetWorksheet);
			}
			foreach (Worksheet sourceWorksheet in this.Sheets) {
				CopySingleWorksheet(newDocumentModel, flags, targetWorksheets, sourceWorksheet);
			}
		}
		void CopySingleWorksheet(DocumentModel newDocumentModel, ModelPasteSpecialFlags flags, List<IWorksheet> targetWorksheets, Worksheet sourceWorksheet) {
			Worksheet targetWorksheet = newDocumentModel.sheets[sourceWorksheet.Name];
			var ranges = new SourceTargetRangesForCopy(sourceWorksheet, targetWorksheet);
			CopyWorksheetOperation copyOperation = new CopyWorksheetOperation(ranges);
			copyOperation.GenerateNewTableName = false;
			copyOperation.CopyCellStyles = false;
			ModelPasteSpecialFlags sheetFlags = flags;
			if (sourceWorksheet.ActiveView.ShowFormulas)
				sheetFlags |= ModelPasteSpecialFlags.Formulas;
			copyOperation.PasteSpecialOptions.InnerFlags = sheetFlags;
			copyOperation.DisabledHistory = !newDocumentModel.IsNormalHistory;
			copyOperation.SuppressChecks = true;
			copyOperation.SetTargetWorksheets(targetWorksheets); 
			copyOperation.SheetDefinitionToOuterAreasReplaceMode = SheetDefToOuterAreasReplaceMode.ThingToValue;
			copyOperation.Execute();
		}
		public DocumentModel CreateEmptyCopy() {
			DocumentModel copy = new DocumentModel();
			this.CopyKnownServices(copy);
			copy.InnerCulture = this.InnerCulture;
			return copy;
		}
	}
}
