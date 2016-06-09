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
using System.Linq;
using System.Threading;
using DevExpress.Data.WizardFramework;
using DevExpress.DataAccess.Excel;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Excel;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Native;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.DataAccess.Wizard.Views;
using DevExpress.SpreadsheetSource;
namespace DevExpress.DataAccess.Wizard.Presenters {
	public class ChooseExcelFileDataRangePage<TModel> : WizardPageBase<IChooseExcelFileDataRangePageView, TModel> where TModel : IExcelDataSourceModel {
		readonly IWizardRunnerContext context;
		readonly IExcelSchemaProvider excelSchemaProvider;
		IWorksheetCollection worksheetCollection;
		ICollection<IDefinedName> definedNamesCollection;
		ITablesCollection tablesCollection;
		ListBoxItem[] listBoxItems;
		ExcelSourceOptions excelSourceOptions;
		ExcelSettingsBase excelSettings;
		public ChooseExcelFileDataRangePage(IChooseExcelFileDataRangePageView view, IWizardRunnerContext context, IExcelSchemaProvider excelSchemaProvider)
			: base(view) {
			this.context = context;
			this.excelSchemaProvider = excelSchemaProvider;
		}
		#region Overrides of WizardPageBase<IChooseExcelFileDataRangePageView,TModel>
		public override bool MoveNextEnabled {
			get { return View.SelectedItem != null; }
		}
		public override Type GetNextPageType() {
			return typeof(ConfigureExcelFileColumnsPage<TModel>);
		}
		public override void Begin() {
			using(ISpreadsheetSource spreadsheetSource = ExcelDataLoaderHelper.CreateSource(null, ExcelDataLoaderHelper.DetectFormat(Model.FileName), Model.FileName, Model.SourceOptions)) {
				worksheetCollection = spreadsheetSource.Worksheets;
				definedNamesCollection = spreadsheetSource.DefinedNames.Where(dn => !dn.IsHidden).ToList();
				tablesCollection = spreadsheetSource.Tables;
			}
			listBoxItems = worksheetCollection
					.Select(
						ws => new ListBoxItem {
							ItemName = ws.Name,
							ItemType = ItemType.Worksheet
						}
					)
					.Union(
						definedNamesCollection
							.Select(
								dn => new ListBoxItem {
									ItemName = dn.Name,
									ItemType = ItemType.DefinedName
								}
							)
					)
					.Union(
						tablesCollection
							.Select(
								t => new ListBoxItem {
									ItemName = t.Name,
									ItemType = ItemType.Table
								}
							)
					).ToArray();
			View.Initialize(listBoxItems);
			View.Changed += ViewOnChanged;
			excelSourceOptions = (ExcelSourceOptions)Model.SourceOptions;
			excelSettings = excelSourceOptions.ImportSettings;
			ExcelDefinedNameSettings dnSettings = excelSettings as ExcelDefinedNameSettings;
			if(dnSettings != null) {
				View.SelectedItem = GetListBoxItem(ItemType.DefinedName, dnSettings.DefinedName);
				return;
			}
			ExcelTableSettings tSelection = excelSettings as ExcelTableSettings;
			if(tSelection != null) {
				View.SelectedItem = GetListBoxItem(ItemType.Table, tSelection.TableName);
				return;
			}
			ExcelWorksheetSettings wsSettings = excelSettings as ExcelWorksheetSettings;
			if(wsSettings != null) {
				View.SelectedItem = GetListBoxItem(ItemType.Worksheet, wsSettings.WorksheetName);
			}
		}
		void ViewOnChanged(object sender, EventArgs eventArgs) {
			RaiseChanged();
		}
		public override bool Validate(out string errorMessage) {
			errorMessage = string.Empty;
			try {
				ListBoxItem item = View.SelectedItem;
				if(item == null) {
					return false;
				}
				var optionsClone = (ExcelSourceOptions)Model.SourceOptions.Clone();
				switch(item.ItemType) {
					case ItemType.Worksheet:
						IWorksheet worksheet = worksheetCollection.First(ws => ws.Name == item.ItemName);
						optionsClone.ImportSettings = new ExcelWorksheetSettings { WorksheetName = worksheet.Name };
						break;
					case ItemType.DefinedName:
						IDefinedName definedName = definedNamesCollection.First(dn => dn.Name == item.ItemName);
						optionsClone.ImportSettings = new ExcelDefinedNameSettings { DefinedName = definedName.Name, Scope = definedName.Scope };
						break;
					case ItemType.Table:
						ITable table = tablesCollection.First(t => t.Name == item.ItemName);
						optionsClone.ImportSettings = new ExcelTableSettings { TableName = table.Name };
						break;
				}
				Func<CancellationToken, FieldInfo[]> getSchema = (token) => excelSchemaProvider.GetSchema(Model.FileName, null, ExcelDocumentFormat.Csv, optionsClone, token);
				if(Model.Schema == null) {
					Model.Schema = ExcelSchemaLoaderAsync.GetSchema(getSchema, WaitFormActivator, ExceptionHandler);
					return Model.Schema != null;
				}
				if(!optionsClone.Equals(excelSettings)) {
					var oldSchema = Model.Schema;
					Model.Schema = ExcelSchemaLoaderAsync.GetSchema(getSchema, WaitFormActivator, ExceptionHandler);
					if(Model.Schema == null) {
						return false;
					}
					foreach(FieldInfo fieldInfo in Model.Schema) {
						var match = oldSchema.FirstOrDefault(fi => fi.Name == fieldInfo.Name);
						if(match != null)
							fieldInfo.Selected = match.Selected;
					}
				}
			}
			catch(Exception ex) {
				ExceptionHandler.HandleException(new CantLoadExcelFileSchemaException(ex));
				return false;
			}
			return true;
		}
		public override void Commit() {
			ListBoxItem item = View.SelectedItem;
			if(item == null) {
				excelSourceOptions.ImportSettings = null;
				return;
			}
			switch(item.ItemType) {
				case ItemType.Worksheet:
					IWorksheet worksheet = worksheetCollection.First(ws => ws.Name == item.ItemName);
					excelSourceOptions.ImportSettings = new ExcelWorksheetSettings { WorksheetName = worksheet.Name };
					break;
				case ItemType.DefinedName:
					IDefinedName definedName = definedNamesCollection.First(dn => dn.Name == item.ItemName);
					excelSourceOptions.ImportSettings = new ExcelDefinedNameSettings { DefinedName = definedName.Name, Scope = definedName.Scope };
					break;
				case ItemType.Table:
					ITable table = tablesCollection.First(t => t.Name == item.ItemName);
					excelSourceOptions.ImportSettings = new ExcelTableSettings { TableName = table.Name };
					break;
			}
		}
		#endregion
		protected virtual IWaitFormActivator WaitFormActivator { get { return context.WaitFormActivator; } }
		protected virtual IExceptionHandler ExceptionHandler { get { return context.CreateExceptionHandler(ExceptionHandlerKind.Default); } }
		ListBoxItem GetListBoxItem(ItemType itemType, string itemName) {
			return listBoxItems.FirstOrDefault(item => item.ItemType == itemType && item.ItemName == itemName);
		}
	}
	public class ListBoxItem {
		public string ItemName { get; set; }
		public ItemType ItemType { get; set; }
		public int ImageIndex { get { return (int)ItemType; } }
		#region Overrides of Object
		public override string ToString() {
			return ItemName;
		}
		#endregion
	}
	public enum ItemType {
		Worksheet,
		DefinedName,
		Table
	}
}
