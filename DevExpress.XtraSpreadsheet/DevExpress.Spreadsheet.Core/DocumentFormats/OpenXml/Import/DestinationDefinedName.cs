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
using System.Xml;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region DefinedNamesDestination
	public abstract class DefinedNamesDestinationBase<T> : ElementDestination<SpreadsheetMLBaseImporter> where T : DefinedNameBase {
		#region Fields
		readonly IModelWorkbook workbook;
		#endregion
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("definedName", OnDefinedNameDestination);
			return result;
		}
		static DefinedNamesDestinationBase<T> GetThis(SpreadsheetMLBaseImporter importer) {
			return (DefinedNamesDestinationBase<T>)importer.PeekDestination();
		}
		internal protected DefinedNamesDestinationBase(SpreadsheetMLBaseImporter importer, IModelWorkbook workbook)
			: base(importer) {
			this.workbook = workbook;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		internal protected abstract Destination GetDefinedNameDestination(SpreadsheetMLBaseImporter importer, IModelWorkbook workbook);
		static Destination OnDefinedNameDestination(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DefinedNamesDestinationBase<T> thisImporter = GetThis(importer);
			return thisImporter.GetDefinedNameDestination(importer, thisImporter.workbook);
		}
	}
	#endregion
	public abstract class DefinedNameDestinationBase<T> : LeafElementDestination<SpreadsheetMLBaseImporter> where T : DefinedNameBase {
		readonly IModelWorkbook workBook;
		T definedName;
		internal protected DefinedNameDestinationBase(SpreadsheetMLBaseImporter importer, IModelWorkbook workBook)
			: base(importer) {
			this.workBook = workBook;
		}
		public T DefinedName { get { return definedName; } set { definedName = value; } }
		public IModelWorkbook WorkBook { get { return workBook; } }
		protected internal abstract T GetDefinedNameInstance(string name, IModelWorkbook workbook, int sheetId);
		protected internal virtual T CreateWorkBookScoped(string name) {
			T result = GetDefinedNameInstance(name, workBook, -1);
			AddDefinedNameToCollection(result, WorkBook.DefinedNames);
			return result;
		}
		protected internal virtual T CreateSheetScoped(string name, int sheetId) {
			T result;
			if (sheetId >= workBook.SheetCount)
				Importer.ThrowInvalidFile("Sheet id greater of equal to sheets count");
			IWorksheet targetSheet = workBook.GetSheetByIndex(sheetId);
			result = GetDefinedNameInstance(name, workBook, sheetId);
			AddDefinedNameToCollection(result, targetSheet.DefinedNames);
			return result;
		}
		protected internal virtual void AddDefinedNameToCollection(T item, DefinedNameCollectionBase definedNames) {
			if (!definedNames.Contains(item.Name))
				definedNames.AddWithoutHistoryAndNotifications(item);
		}
	}
	#region DefinedNamesDestination
	public class DefinedNamesDestination : DefinedNamesDestinationBase<DefinedName> {
		public DefinedNamesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer, importer.DocumentModel) {
		}
		protected internal override Destination GetDefinedNameDestination(SpreadsheetMLBaseImporter importer, IModelWorkbook workbook) {
			return new DefinedNameDestination(importer, workbook);
		}
	}
	#endregion
	public class DefinedNameDestination : DefinedNameDestinationBase<DefinedName> {
		public DefinedNameDestination(SpreadsheetMLBaseImporter importer, IModelWorkbook workBook)
			: base(importer, workBook) {
		}
		public override bool ProcessText(XmlReader reader) {
			if (DefinedName == null)
				return true;
			Importer.DefinedNameReferences.Add(DefinedName, reader.Value);
			return true;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string name = Importer.ReadAttribute(reader, "name");
			if (String.IsNullOrEmpty(name))
				return;
			int sheetId = Importer.GetIntegerValue(reader, "localSheetId", Int32.MinValue);
			DefinedName = (sheetId == Int32.MinValue)
				? CreateWorkBookScoped(name)
				: CreateSheetScoped(name, sheetId);
			DefinedName.SetIsHiddenCore(Importer.GetWpSTOnOffValue(reader, "hidden", false));
			DefinedName.IsXlmMacro = Importer.GetWpSTOnOffValue(reader, "function", false);
			DefinedName.IsVbaMacro = Importer.GetWpSTOnOffValue(reader, "vbProcedure", false);
			DefinedName.IsMacro = Importer.GetWpSTOnOffValue(reader, "xlm", false);
			DefinedName.FunctionGroupId = Importer.GetWpSTIntegerValue(reader, "functionGroupId", 0);
			string comment = Importer.ReadAttribute(reader, "comment");
			DefinedName.SetCommentCore(Importer.DecodeXmlChars(comment));
		}
		protected internal override DefinedName GetDefinedNameInstance(string name, IModelWorkbook workbook, int sheetId) {
			int scope = sheetId;
			if(scope >= 0) {
				IWorksheet targetSheet = WorkBook.GetSheetByIndex(sheetId);
				scope = targetSheet.SheetId;
			}
			DocumentModel documentModel = (DocumentModel)workbook;
			return new DefinedName(documentModel, name, string.Empty, scope);
		}
	}
}
