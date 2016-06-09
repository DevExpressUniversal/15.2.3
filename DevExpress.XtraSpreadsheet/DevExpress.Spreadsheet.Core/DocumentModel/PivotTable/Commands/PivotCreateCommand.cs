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
using System.Diagnostics;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Services;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCreateCommand
	public class PivotCreateCommand : ErrorHandledWorksheetCommand {
		string name;
		CellRange location;
		PivotCacheCreateCommand createCacheCommand;
		public PivotCreateCommand(IErrorHandler errorHandler, IPivotCacheSource source, CellRange location)
			: this(errorHandler, source, location, string.Empty) {
		}
		public PivotCreateCommand(IErrorHandler errorHandler, IPivotCacheSource source, CellRange location, string name)
			: base((Worksheet)location.Worksheet, errorHandler) {
			this.name = name;
			this.location = PivotTableLocation.GetDefaultCellRange(location);
			this.createCacheCommand = new PivotCacheCreateCommand(DocumentModel, errorHandler, source);
		}
		protected internal override bool Validate() {
			if (!createCacheCommand.Validate())
				return false;
			if (!PivotRefreshDataOnWorksheetCommand.Validate(location, null, location, null, false, ErrorHandler))
				return false;
			return true;
		}
		protected internal override void ExecuteCore() {
			createCacheCommand.ExecuteCore();
			PivotCache pivotCache = (PivotCache)createCacheCommand.Result;
			if (string.IsNullOrEmpty(name)) {
				IPivotTableNameCreationService service = DocumentModel.GetService<IPivotTableNameCreationService>();
				if (service == null)
					throw new InvalidOperationException("Name for the Pivot table can not be assigned: service is missing.");
				name = service.GetNewName(Worksheet.PivotTables.GetExistingNames());
			}
			PivotTable pivotTable = new PivotTable(DocumentModel, name, new PivotTableLocation(location), pivotCache);
			pivotTable.BeginTransaction(ErrorHandler);
			pivotTable.CalculationInfo.Transaction.SuppressRefreshDataOnWorksheetValidation = true;
			for (int i = 0; i < pivotCache.CacheFields.Count; ++i) {
				PivotField field = pivotTable.CreateField();
				pivotTable.Fields.AddCore(field);
			}
			pivotTable.CalculationInfo.InvalidateCalculatedCache();
			pivotTable.StyleInfo.SetStyleNameCore(DocumentModel.StyleSheet.TableStyles.DefaultPivotStyle.Name.Name);
			bool pivotRefreshed = pivotTable.EndTransaction();
			pivotTable.CreatedVersion = PivotTable.Excel2013Version;
			Worksheet.PivotTables.Add(pivotTable);
			Result = pivotTable;
			Debug.Assert(pivotRefreshed);
		}
	}
	#endregion
	#region PivotCacheCreateCommand
	public class PivotCacheCreateCommand : ErrorHandledWorksheetCommand {
		readonly IPivotCacheSource source;
		public PivotCacheCreateCommand(DocumentModel documentModel, IErrorHandler errorHandler, IPivotCacheSource source)
			: base(documentModel, errorHandler) {
			this.source = source;
		}
		public IPivotCacheSource Source { get { return source; } }
		protected internal override bool Validate() {
			return Validate(source, DataContext, ErrorHandler);
		}
		public static bool Validate(IPivotCacheSource source, WorkbookDataContext dataContext, IErrorHandler errorHandler) {
			if (source == null)
				if (errorHandler.HandleError(new ModelErrorInfo(ModelErrorType.InvalidReference)) == ErrorHandlingResult.Abort)
					return false;
			IModelErrorInfo error = source.CheckSourceBeforeRefresh(dataContext);
			if (error != null)
				return errorHandler.HandleError(error) != ErrorHandlingResult.Abort;
			return true;
		}
		protected internal override void ExecuteCore() {
			PivotCache cache = DocumentModel.PivotCaches.TryGetPivotCache(source);
			if (cache == null) {
				cache = new PivotCache(DocumentModel, source);
				SetCacheDefaultProperties(cache);
				PivotCacheRefreshCommand command = new PivotCacheRefreshCommand(ErrorHandler, cache);
				command.ShouldClearHistory = false;
				command.Execute();
				DocumentModel.PivotCaches.Add(cache);
			}
			Result = cache;
		}
		void SetCacheDefaultProperties(PivotCache cache) {
			cache.Invalid = false;
			cache.SaveData = true;
			cache.OptimizeMemory = false;
			cache.RefreshOnLoad = false;
			cache.EnableRefresh = true;
			cache.BackgroundQuery = false;
			cache.UpgradeOnRefresh = false;
			cache.HasTupleCache = false;
			cache.SupportSubquery = false;
			cache.SupportAdvancedDrill = false;
		}
	}
	#endregion
}
