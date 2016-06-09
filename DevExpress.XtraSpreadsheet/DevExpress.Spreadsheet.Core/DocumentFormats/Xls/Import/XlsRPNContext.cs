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
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsDefinedNameInfo
	public class XlsDefinedNameInfo : IEquatable<XlsDefinedNameInfo> {
		public string Name { get; set; }
		public int Scope { get; set; }
		#region IEquatable<XlsDefinedNameInfo> Members
		public bool Equals(XlsDefinedNameInfo other) {
			if (other == null) return false;
			return StringExtensions.CompareInvariantCultureIgnoreCase(Name, other.Name) == 0 && Scope == other.Scope;
		}
		#endregion
	}
	#endregion
	#region IXlsRPNContext
	public interface IXlsRPNContext : IRPNContext {
		string CurrentSubject { get; }
		bool SuppressFormulaLengthCheck { get; }
		SheetDefinition GetSheetDefinitionNameX(int index, int nameIndex);
		SheetDefinition GetSheetDefinition(int firstSheetIndex, int lastSheetIndex);
		int IndexOfSheetDefinitionNameX(SheetDefinition sheetDefinition, string definedName);
		SheetScope GetSheetScope(SheetDefinition sheetDefinition);
		string GetDefinedName(int index);
		string GetDefinedName(int index, int nameIndex);
		int IndexOfDefinedName(string name);
		int IndexOfDefinedName(string name, SheetDefinition sheetDefinition);
	}
	#endregion
	#region XlsRPNContext
	public class XlsRPNContext : IXlsRPNContext {
		#region Fields
		BinaryRPNReaderXls binaryRPNReader;
		BinaryRPNWriterXls binaryRPNWriter;
		ExtNameBinaryRPNReader extNameBinaryRPNReader;
		ExtNameBinaryRPNWriter extNameBinaryRPNWriter;
		WorkbookDataContext workbookContext;
		readonly List<string> sheetNames = new List<string>();
		readonly List<XlsDefinedNameInfo> definedNames = new List<XlsDefinedNameInfo>();
		readonly List<XlsSupBookInfo> supBooks = new List<XlsSupBookInfo>();
		readonly Dictionary<int, int> supBooksTable = new Dictionary<int, int>();
		readonly List<XlsExternInfo> externSheets = new List<XlsExternInfo>();
		readonly DxStack<string> subjects = new DxStack<string>();
		#endregion
		public XlsRPNContext(WorkbookDataContext workbookContext) {
			this.workbookContext = workbookContext;
			this.binaryRPNWriter = new BinaryRPNWriterXls(this);
			this.binaryRPNReader = new BinaryRPNReaderXls(this);
			this.extNameBinaryRPNReader = new ExtNameBinaryRPNReader(this);
			this.extNameBinaryRPNWriter = new ExtNameBinaryRPNWriter(this);
		}
		#region Properties
		public List<string> SheetNames { get { return sheetNames; } }
		public List<XlsSupBookInfo> SupBooks { get { return supBooks; } }
		public Dictionary<int, int> SupBooksTable { get { return supBooksTable; } }
		public List<XlsExternInfo> ExternSheets { get { return externSheets; } }
		protected internal List<XlsDefinedNameInfo> DefinedNames { get { return definedNames; } }
		#endregion
		public byte[] ExpressionToBinary(ParsedExpression expression) {
			return binaryRPNWriter.GetBinary(expression);
		}
		public ParsedExpression BinaryToExpression(byte[] data) {
			return binaryRPNReader.FromBinary(data);
		}
		public ParsedExpression BinaryToExpression(byte[] data, int bytesCount) {
			return binaryRPNReader.FromBinary(data, 0, bytesCount);
		}
		public ParsedExpression BinaryToExpression(byte[] data, BinaryReader extraReader) {
			return binaryRPNReader.FromBinary(data, extraReader);
		}
		public byte[] ExtNameExpressionToBinary(ParsedExpression expression) {
			return extNameBinaryRPNWriter.GetBinary(expression);
		}
		public ParsedExpression ExtNameBinaryToExpression(byte[] data) {
			return extNameBinaryRPNReader.FromBinary(data);
		}
		public string BuildExpressionString(byte[] data, int startIndex) {
			throw new InvalidOperationException();
		}
		public VariantValue EvaluateBinaryExpression(byte[] binaryFormula, int startIndex, WorkbookDataContext context) {
			throw new InvalidOperationException();
		}
		public void PushCurrentSubject(string subject) {
			subjects.Push(subject);
		}
		public void PopCurrentSubject() {
			subjects.Pop();
		}
		#region IXlsRPNContext Members
		public WorkbookDataContext WorkbookContext { get { return workbookContext; } }
		public string CurrentSubject {
			get {
				if (subjects.Count > 0)
					return subjects.Peek();
				return string.Empty;
			}
		}
		public bool SuppressFormulaLengthCheck { get; set; }
		public SheetDefinition GetSheetDefinition(int index) {
			if (index < 0 || index >= ExternSheets.Count)
				Exceptions.ThrowArgumentException("index", index);
			XlsExternInfo externSheet = ExternSheets[index];
			if (externSheet.SupBookIndex >= SupBooks.Count)
				Exceptions.ThrowInternalException();
			return GetSheetDefinitionCore(externSheet.SupBookIndex, externSheet.FirstSheetIndex, externSheet.LastSheetIndex);
		}
		public SheetDefinition GetSheetDefinition(int firstSheetIndex, int lastSheetIndex) {
			if (SupBooks.Count == 0)
				Exceptions.ThrowInternalException();
			return GetSheetDefinitionCore(SupBooks.Count - 1, firstSheetIndex, lastSheetIndex);
		}
		public SheetDefinition GetSheetDefinitionNameX(int index, int nameIndex) {
			if (index < 0 || index >= ExternSheets.Count)
				Exceptions.ThrowArgumentException("index", index);
			XlsExternInfo externSheet = ExternSheets[index];
			if (externSheet.SupBookIndex >= SupBooks.Count)
				Exceptions.ThrowInternalException();
			XlsSupBookInfo supBook = SupBooks[externSheet.SupBookIndex];
			if (supBook.LinkType == XlsSupportingLinkType.Self && externSheet.EntireWorkbook && (nameIndex > 0 && nameIndex <= DefinedNames.Count)) {
				XlsDefinedNameInfo info = DefinedNames[nameIndex - 1];
				int scope = info.Scope == 0 ? -2 : info.Scope - 1;
				return GetSheetDefinitionCore(externSheet.SupBookIndex, scope, scope);
			}
			if (supBook.LinkType == XlsSupportingLinkType.ExternalWorkbook && externSheet.EntireWorkbook && (nameIndex > 0 && nameIndex <= supBook.ExternalNames.Count)) {
				XlsDefinedNameInfo info = supBook.ExternalNames[nameIndex - 1];
				int scope = info.Scope;
				return GetSheetDefinitionCore(externSheet.SupBookIndex, scope, scope);
			}
			return GetSheetDefinitionCore(externSheet.SupBookIndex, externSheet.FirstSheetIndex, externSheet.LastSheetIndex);
		}
		protected SheetDefinition GetSheetDefinitionCore(int supBookIndex, int firstSheetIndex, int lastSheetIndex) {
			XlsSupBookInfo supBook = SupBooks[supBookIndex];
			SheetDefinition result = new SheetDefinition();
			if (supBook.LinkType == XlsSupportingLinkType.Self) {
				if (firstSheetIndex == lastSheetIndex) {
					result.SheetNameStart = GetSheetNameByIndex(firstSheetIndex);
				}
				else {
					result.SheetNameStart = GetSheetNameByIndex(firstSheetIndex);
					result.SheetNameEnd = GetSheetNameByIndex(lastSheetIndex);
				}
				if (firstSheetIndex < 0 && lastSheetIndex < 0)
					result.ExternalReferenceIndex = 0;
				if (firstSheetIndex == -1 || lastSheetIndex == -1)
					result.ValidReference = false;
			}
			else if (supBook.LinkType == XlsSupportingLinkType.AddIn) {
				return null;
			}
			else if (supBook.LinkType == XlsSupportingLinkType.DataSource) {
				result.ExternalReferenceIndex = SupBooksTable[supBookIndex];
			}
			else if (supBook.LinkType == XlsSupportingLinkType.ExternalWorkbook) {
				result.ExternalReferenceIndex = SupBooksTable[supBookIndex];
				if (firstSheetIndex == lastSheetIndex) {
					result.SheetNameStart = supBook.GetSheetNameByIndex(firstSheetIndex);
				}
				else {
					result.SheetNameStart = supBook.GetSheetNameByIndex(firstSheetIndex);
					result.SheetNameEnd = supBook.GetSheetNameByIndex(lastSheetIndex);
				}
				if (firstSheetIndex == -1 || lastSheetIndex == -1)
					result.ValidReference = false;
			}
			else if (supBook.LinkType == XlsSupportingLinkType.SameSheet) {
			}
			else { 
				Exceptions.ThrowInvalidOperationException("Unused link must not be used.");
			}
			return result;
		}
		public int IndexOfSheetDefinition(SheetDefinition sheetDefinition) {
			return IndexOfSheetDefinitionCore(sheetDefinition, string.Empty);
		}
		public int IndexOfSheetDefinitionNameX(SheetDefinition sheetDefinition, string definedName) {
			return IndexOfSheetDefinitionCore(sheetDefinition, definedName);
		}
		int IndexOfSheetDefinitionCore(SheetDefinition sheetDefinition, string definedName) {
			int supBookIndex = -1;
			if (sheetDefinition == null) {
				supBookIndex = GetSupBookIndex(XlsSupportingLinkType.AddIn);
				if (supBookIndex != -1)
					return GetExternSheetIndex(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
				return -1;
			}
			if (sheetDefinition.IsExternalReference) {
				if (SupBooksTable.ContainsKey(sheetDefinition.ExternalReferenceIndex))
					supBookIndex = SupBooksTable[sheetDefinition.ExternalReferenceIndex];
				if (supBookIndex != -1) {
					XlsSupBookInfo supBook = SupBooks[supBookIndex];
					if (supBook.LinkType == XlsSupportingLinkType.DataSource || (!sheetDefinition.SheetDefinied && sheetDefinition.ValidReference)) {
						return GetExternSheetIndex(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
					}
					else if (sheetDefinition.Is3DReference) {
						return GetExternSheetIndex(supBookIndex,
							supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart),
							supBook.SheetNames.IndexOf(sheetDefinition.SheetNameEnd));
					}
					else {
						int sheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
						return GetExternSheetIndex(supBookIndex, sheetIndex, sheetIndex);
					}
				}
			}
			else if (sheetDefinition.IsCurrentSheetReference) {
				supBookIndex = GetSupBookIndex(XlsSupportingLinkType.SameSheet);
				if (supBookIndex != -1)
					return GetExternSheetIndex(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
			}
			else {
				ExternalWorkbook externalWorkbook = WorkbookContext.CurrentWorkbook as ExternalWorkbook;
				if (externalWorkbook != null) {
					int externalIndex = WorkbookContext.Workbook.ExternalLinks.IndexOf(externalWorkbook) + 1;
					if (externalIndex > 0 && SupBooksTable.ContainsKey(externalIndex))
						supBookIndex = SupBooksTable[externalIndex];
					if (supBookIndex != -1) {
						XlsSupBookInfo supBook = SupBooks[supBookIndex];
						if (supBook.LinkType == XlsSupportingLinkType.DataSource || !sheetDefinition.SheetDefinied) {
							return GetExternSheetIndex(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
						}
						else if (sheetDefinition.Is3DReference) {
							return GetExternSheetIndex(supBookIndex,
								supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart),
								supBook.SheetNames.IndexOf(sheetDefinition.SheetNameEnd));
						}
						else {
							int sheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
							return GetExternSheetIndex(supBookIndex, sheetIndex, sheetIndex);
						}
					}
				}
				else {
					supBookIndex = GetSupBookIndex(XlsSupportingLinkType.Self);
					if (supBookIndex != -1) {
						if (sheetDefinition.Is3DReference) {
							return GetExternSheetIndex(supBookIndex,
								GetSheetIndexByName(sheetDefinition.SheetNameStart),
								GetSheetIndexByName(sheetDefinition.SheetNameEnd));
						}
						else if (!string.IsNullOrEmpty(definedName)) {
							return GetExternSheetIndex(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
						}
						else {
							int sheetIndex = GetSheetIndexByName(sheetDefinition.SheetNameStart);
							return GetExternSheetIndex(supBookIndex, sheetIndex, sheetIndex);
						}
					}
				}
			}
			return -1;
		}
		public SheetScope GetSheetScope(SheetDefinition sheetDefinition) {
			Guard.ArgumentNotNull(sheetDefinition, "sheetDefinition");
			SheetScope result = new SheetScope();
			int supBookIndex = -1;
			if (sheetDefinition.IsExternalReference) {
				if (SupBooksTable.ContainsKey(sheetDefinition.ExternalReferenceIndex))
					supBookIndex = SupBooksTable[sheetDefinition.ExternalReferenceIndex];
				if (supBookIndex != -1) {
					XlsSupBookInfo supBook = SupBooks[supBookIndex];
					if (sheetDefinition.Is3DReference) {
						result.FirstSheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
						result.LastSheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameEnd);
					}
					else {
						int sheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
						result.FirstSheetIndex = sheetIndex;
						result.LastSheetIndex = sheetIndex;
					}
				}
			}
			else if (sheetDefinition.IsCurrentSheetReference) {
				supBookIndex = GetSupBookIndex(XlsSupportingLinkType.SameSheet);
				if (supBookIndex != -1) {
					result.FirstSheetIndex = XlsDefs.NoScope;
					result.LastSheetIndex = XlsDefs.NoScope;
				}
			}
			else {
				ExternalWorkbook externalWorkbook = WorkbookContext.CurrentWorkbook as ExternalWorkbook;
				if (externalWorkbook != null) {
					int externalIndex = WorkbookContext.Workbook.ExternalLinks.IndexOf(externalWorkbook) + 1;
					if (externalIndex > 0 && SupBooksTable.ContainsKey(externalIndex))
						supBookIndex = SupBooksTable[externalIndex];
					if (supBookIndex != -1) {
						XlsSupBookInfo supBook = SupBooks[supBookIndex];
						if (sheetDefinition.Is3DReference) {
							result.FirstSheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
							result.LastSheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameEnd);
						}
						else {
							int sheetIndex = supBook.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
							result.FirstSheetIndex = sheetIndex;
							result.LastSheetIndex = sheetIndex;
						}
					}
				}
				else {
					supBookIndex = GetSupBookIndex(XlsSupportingLinkType.Self);
					if (supBookIndex != -1) {
						if (sheetDefinition.Is3DReference) {
							result.FirstSheetIndex = GetSheetIndexByName(sheetDefinition.SheetNameStart);
							result.LastSheetIndex = GetSheetIndexByName(sheetDefinition.SheetNameEnd);
						}
						else if (sheetDefinition.SheetDefinied) {
							int sheetIndex = GetSheetIndexByName(sheetDefinition.SheetNameStart);
							result.FirstSheetIndex = sheetIndex;
							result.LastSheetIndex = sheetIndex;
						}
						else {
							result.FirstSheetIndex = XlsDefs.NoScope;
							result.LastSheetIndex = XlsDefs.NoScope;
						}
					}
				}
			}
			return result;
		}
		public string GetDefinedName(int index) {
			if (index < 1 || index > DefinedNames.Count)
				return string.Empty;
			return DefinedNames[index - 1].Name;
		}
		public string GetDefinedName(int index, int nameIndex) {
			if (index < 0 || index >= ExternSheets.Count)
				Exceptions.ThrowArgumentException("index", index);
			XlsExternInfo externSheet = ExternSheets[index];
			if (externSheet.SupBookIndex >= SupBooks.Count)
				Exceptions.ThrowInternalException();
			XlsSupBookInfo supBook = SupBooks[externSheet.SupBookIndex];
			if (supBook.LinkType == XlsSupportingLinkType.Self) {
				if (nameIndex < 1 || nameIndex > DefinedNames.Count)
					return string.Empty;
				return DefinedNames[nameIndex - 1].Name;
			}
			if (nameIndex < 1 || nameIndex > supBook.ExternalNames.Count)
				return string.Empty;
			return supBook.ExternalNames[nameIndex - 1].Name;
		}
		public int IndexOfDefinedName(string name) {
			if (name.StartsWith(WorksheetFunctionBase.FUTURE_FUNCTION_PREFIX))
				return IndexOfCore(name, XlsDefs.NoScope) + 1;
			DefinedNameBase item = WorkbookContext.GetDefinedName(name);
			if (item == null)
				return IndexOfCore(name, XlsDefs.NoScope) + 1;
			return IndexOfCore(item.Name, item.ScopedSheetId) + 1;
		}
		public int IndexOfDefinedName(string name, SheetDefinition sheetDefinition) {
			if (sheetDefinition == null) {
				int supBookIndex = GetSupBookIndex(XlsSupportingLinkType.AddIn);
				if (supBookIndex != -1) {
					XlsSupBookInfo supBook = SupBooks[supBookIndex];
					return supBook.IndexOfExternalName(name, XlsDefs.NoScope) + 1;
				}
				return 0;
			}
			if (sheetDefinition.IsExternalReference) {
				int supBookIndex = -1;
				if (SupBooksTable.ContainsKey(sheetDefinition.ExternalReferenceIndex))
					supBookIndex = SupBooksTable[sheetDefinition.ExternalReferenceIndex];
				if (supBookIndex != -1) {
					XlsSupBookInfo supBook = SupBooks[supBookIndex];
					if (supBook.LinkType == XlsSupportingLinkType.ExternalWorkbook)
						return supBook.IndexOfExternalName(name, sheetDefinition.SheetNameStart) + 1;
					else if (supBook.LinkType == XlsSupportingLinkType.DataSource) {
						ExternalLink link = WorkbookContext.Workbook.ExternalLinks[sheetDefinition.ExternalReferenceIndex - 1];
						DdeExternalWorkbook connection = link.Workbook as DdeExternalWorkbook;
						if (connection != null)
							return connection.Sheets.GetSheetIndexByName(name) + 1;
					}
				}
				return 0;
			}
			DefinedNameBase item = WorkbookContext.GetDefinedName(name, sheetDefinition);
			if (item == null)
				return IndexOfCore(name, GetScope(sheetDefinition)) + 1;
			return IndexOfCore(item.Name, item.ScopedSheetId) + 1;
		}
		#endregion
		protected internal string GetSheetNameByIndex(int index) {
			if (index < 0 || index >= SheetNames.Count)
				return string.Empty;
			return SheetNames[index];
		}
		protected internal int GetSheetIndexByName(string name) {
			return SheetNames.IndexOf(name);
		}
		protected internal int GetSupBookIndex(XlsSupportingLinkType linkType) {
			for (int i = 0; i < SupBooks.Count; i++) {
				if (SupBooks[i].LinkType == linkType)
					return i;
			}
			return -1;
		}
		protected internal int GetExternSheetIndex(int supBookIndex, int firstSheetIndex, int lastSheetIndex) {
			XlsExternInfo info = new XlsExternInfo();
			info.SupBookIndex = supBookIndex;
			info.FirstSheetIndex = firstSheetIndex;
			info.LastSheetIndex = lastSheetIndex;
			return ExternSheets.IndexOf(info);
		}
		public void RegisterDefinedName(string name, int scope) {
			XlsDefinedNameInfo info = new XlsDefinedNameInfo();
			info.Name = name;
			info.Scope = scope;
			DefinedNames.Add(info);
		}
		public bool IsRegisteredDefinedName(string name, int scope) {
			return IndexOfCore(name, scope) != -1;
		}
		int IndexOfCore(string name, int scope) {
			XlsDefinedNameInfo info = new XlsDefinedNameInfo();
			info.Name = name;
			info.Scope = scope;
			return DefinedNames.IndexOf(info);
		}
		#region SheetDefinition
		public void RegisterSheetDefinition(SheetDefinition sheetDefinition) {
			RegisterSheetDefinition(sheetDefinition, string.Empty);
		}
		void RegisterExternalNames(XlsSupBookInfo info, DefinedNameCollectionBase names, int scope) {
			foreach (DefinedNameBase definedName in names)
				info.RegisterExternalName(definedName.Name, scope);
		}
		public void RegisterSheetDefinition(SheetDefinition sheetDefinition, string name) {
			if (sheetDefinition == null) {
				int supBookIndex = GetSupBookIndexOrCreate(XlsSupportingLinkType.AddIn);
				XlsSupBookInfo info = SupBooks[supBookIndex];
				if (!info.IsRegisteredExternalName(name, XlsDefs.NoScope))
					info.RegisterExternalName(name, XlsDefs.NoScope);
				RegisterExternInfo(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
				return;
			}
			if (sheetDefinition.IsExternalReference) {
				ExternalLink link = WorkbookContext.Workbook.ExternalLinks[sheetDefinition.ExternalReferenceIndex - 1];
				if (link.IsExternalWorkbook) {
					ExternalWorkbook workbook = link.Workbook;
					XlsSupBookInfo info = new XlsSupBookInfo();
					info.LinkType = XlsSupportingLinkType.ExternalWorkbook;
					info.SheetCount = workbook.SheetCount;
					info.VirtualPath = XlsVirtualPath.GetVirtualPath(workbook.FilePath, WorkbookContext.Workbook.DocumentSaveOptions);
					foreach (ExternalWorksheet sheet in workbook.Sheets)
						info.SheetNames.Add(sheet.Name);
					RegisterExternalNames(info, workbook.DefinedNames, XlsDefs.NoScope);
					for (int i = 0; i < workbook.Sheets.Count; i++) {
						RegisterExternalNames(info, workbook.Sheets[i].DefinedNames, i);
					}
					int supBookIndex = GetSupBookIndex(info, sheetDefinition.ExternalReferenceIndex);
					info = SupBooks[supBookIndex];
					if (sheetDefinition.Is3DReference) {
						RegisterExternInfo(supBookIndex,
							info.SheetNames.IndexOf(sheetDefinition.SheetNameStart),
							info.SheetNames.IndexOf(sheetDefinition.SheetNameEnd));
					}
					else if (sheetDefinition.SheetDefinied) {
						int sheetIndex = info.SheetNames.IndexOf(sheetDefinition.SheetNameStart);
						RegisterExternInfo(supBookIndex, sheetIndex, sheetIndex);
					}
					else if (!sheetDefinition.ValidReference) {
						RegisterExternInfo(supBookIndex, -1, -1);
					}
					else
						RegisterExternInfo(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
				}
				else if (link.IsDataSource) {
					DdeExternalWorkbook ddeWorkbook = link.Workbook as DdeExternalWorkbook;
					XlsSupBookInfo info = new XlsSupBookInfo();
					info.LinkType = XlsSupportingLinkType.DataSource;
					info.VirtualPath = XlsVirtualPath.GetVirtualPath(ddeWorkbook.DdeServiceName, ddeWorkbook.DdeServerTopic);
					int supBookIndex = GetSupBookIndex(info, sheetDefinition.ExternalReferenceIndex);
					RegisterExternInfo(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
				}
			}
			else if (sheetDefinition.IsCurrentSheetReference) {
				int supBookIndex = GetSupBookIndexOrCreate(XlsSupportingLinkType.SameSheet);
				RegisterExternInfo(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
			}
			else { 
				int supBookIndex = GetSupBookIndexOrCreate(XlsSupportingLinkType.Self);
				if (sheetDefinition.Is3DReference) {
					RegisterExternInfo(supBookIndex,
						GetSheetIndexByName(sheetDefinition.SheetNameStart),
						GetSheetIndexByName(sheetDefinition.SheetNameEnd));
				}
				else if (string.IsNullOrEmpty(name)) {
					int sheetIndex = GetSheetIndexByName(sheetDefinition.SheetNameStart);
					RegisterExternInfo(supBookIndex, sheetIndex, sheetIndex);
				}
				else {
					RegisterExternInfo(supBookIndex, XlsDefs.NoScope, XlsDefs.NoScope);
				}
			}
		}
		public int GetScope(SheetDefinition sheetDefinition) {
			if (sheetDefinition != null) {
				if (sheetDefinition.IsCurrentSheetReference) {
					IWorksheet sheet = WorkbookContext.CurrentWorksheet;
					if (sheet != null)
						return sheet.SheetId;
				}
				else if (sheetDefinition.SheetDefinied) {
					IWorksheet sheet = sheetDefinition.GetSheetStart(WorkbookContext);
					if (sheet != null)
						return sheet.SheetId;
				}
			}
			return XlsDefs.NoScope;
		}
		int GetSupBookIndexOrCreate(XlsSupportingLinkType linkType) {
			int result = GetSupBookIndex(linkType);
			if (result == -1) {
				XlsSupBookInfo info = new XlsSupBookInfo();
				info.LinkType = linkType;
				if (linkType == XlsSupportingLinkType.Self)
					info.SheetCount = WorkbookContext.Workbook.SheetCount;
				SupBooks.Add(info);
				result = SupBooks.Count - 1;
			}
			return result;
		}
		int GetSupBookIndex(XlsSupBookInfo info, int externalIndex) {
			int result = SupBooks.IndexOf(info);
			if (result == -1) {
				result = SupBooks.Count;
				info.ExternalIndex = externalIndex;
				SupBooks.Add(info);
			}
			if (!SupBooksTable.ContainsKey(externalIndex))
				SupBooksTable.Add(externalIndex, result);
			return result;
		}
		void RegisterExternInfo(int supBookIndex, int firstSheetIndex, int lastSheetIndex) {
			XlsExternInfo info = new XlsExternInfo();
			info.SupBookIndex = supBookIndex;
			info.FirstSheetIndex = firstSheetIndex;
			info.LastSheetIndex = lastSheetIndex;
			if (ExternSheets.IndexOf(info) == -1)
				ExternSheets.Add(info);
		}
		#endregion
	}
	#endregion
	#region SheetScope
	public class SheetScope {
		public SheetScope() {
			FirstSheetIndex = -1;
			LastSheetIndex = -1;
		}
		public int FirstSheetIndex { get; set; }
		public int LastSheetIndex { get; set; }
	}
	#endregion
}
