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

using System.IO;
using System.Collections.Generic;
using DevExpress.Xpo.DB;
using System.Collections;
using System;
using DevExpress.Xpo.Metadata.Helpers;
using System.Text;
using System.Globalization;
namespace DevExpress.Xpo.Design {
	internal enum PersistentClassGeneratorKeywords {
		Null,
		Return,
		OperatorEnd,
		New,
		TypeOf,
		Comment,
		MultilineCommentOpen,
		MultilineCommentClose,
		IgnoreEscapeSimbol
	}
	internal abstract class PersistentClassGenerator {
		Dictionary<string, int> reservedWords;
		protected Dictionary<string, int> ReservedWords { get { return reservedWords; } }
		protected abstract string[] ReservedWordsArray { get; }
		public PersistentClassGenerator() {
			reservedWords = new Dictionary<string, int>(ReservedWordsArray.Length);
			for(int i = 0; i < ReservedWordsArray.Length; i++)
				reservedWords.Add(ReservedWordsArray[i], 0);
		}
		public void Check(TextWriter log,
			Dictionary<string, bool> tablesMask, TableColumnsCheckContainer tables) {
			Hashtable list = new Hashtable();
			foreach(KeyValuePair<string, bool> tableMask in tablesMask) {
				if(!tableMask.Value) {
					continue;
				}
				string tableName = tableMask.Key;
				DBTable table = tables.GetStorageTable(StructurePage.GetName(tableName));
				switch(tableName) {
					case "XPObjectType":
					case "XPWeakReference":
					case "EventHandlerLink":
					case "XPDeletedObject": {
							continue;
						}
				}
				Columns columns = tables.GetTableColumns(table.Name);
				if((table.PrimaryKey == null) || (table.PrimaryKey.Columns.Count != 1) ||
					!columns.ContainsKey(table.PrimaryKey.Columns[0]) ||
					!columns[table.PrimaryKey.Columns[0]].Selected) {
					log.WriteLine(tableName + " no single column pk");
				}
			}
			foreach(string key in list.Keys) {
				DBTable table = (DBTable)list[key];
				Columns columns = tables.GetTableColumns(table.Name);
				foreach(DBColumn column in table.Columns) {
					if(((IsLockingField(column.Name, column.ColumnType) || IsGCRecordField(column.Name, column.ColumnType)) || IsObjectTypeField(column.Name, column.ColumnType)) || !columns[column.Name].Selected) {
						continue;
					}
					if(column.ColumnType == DBColumnType.Unknown)
						log.WriteLine(table.Name + " - " + column.Name + " not mapped");
				}
			}
		}
		public void Check(TextWriter log, IDataStoreSchemaExplorerSp prov, Dictionary<string, bool> sprocMasks, SProcsResultSetCheckContainer sprocedures) {
			ConnectionProviderSql provider = prov as ConnectionProviderSql;
			Dictionary<string, DBStoredProcedure> list = new Dictionary<string, DBStoredProcedure>();
			DBStoredProcedure[] sprocList = prov.GetStoredProcedures();
			if(sprocList == null || sprocList.Length == 0) return;
			for(int i = 0; i < sprocList.Length; i++) {
				list.Add(sprocList[i].Name, sprocList[i]);
			}
			foreach(KeyValuePair<string, bool> sprocMask in sprocMasks) {
				if(sprocMask.Value) continue;
				list.Remove(sprocMask.Key);
			}
			Dictionary<string, string> tableClassNameCache = new Dictionary<string, string>();
			Dictionary<string, bool> classNameDict = new Dictionary<string,bool>();
			Dictionary<string, LoadDataMemberOrderItem[]> loadProcOrderDict = new Dictionary<string, LoadDataMemberOrderItem[]>();
			foreach(string key in list.Keys) {
				DBStoredProcedure sproc = list[key];
				if(sproc.ResultSets.Count != 1) continue;
				string className = GetClassName(StructurePage.GetCaption(key), tableClassNameCache, classNameDict);
				ResultSetColumns columns = sprocedures.GetResultSetColumns(sproc.Name);
				int columnsCount = sproc.ResultSets[0].Columns.Count;
				for(int c = 0; c < columnsCount; c++) {
					DBNameTypePair column = sproc.ResultSets[0].Columns[c];
					if(((IsLockingField(column.Name, column.Type) || IsGCRecordField(column.Name, column.Type)) || IsObjectTypeField(column.Name, column.Type)) || !columns[c].Selected)
						continue;
					if(column.Type == DBColumnType.Unknown) {
						log.WriteLine(sproc.Name + " - " + column.Name + " not mapped");
						continue;
					}
					string type = GetLanguageType(column.Type);
					if(String.IsNullOrEmpty(type)) {
						log.WriteLine(sproc.Name + " - " + column.Name + " not mapped");
						continue;
					}
				}
			}
		}
		public static bool CheckTable(IDataStoreSchemaExplorer prov, string tableName, out string message) {
			DBTable table = prov.GetStorageTables(StructurePage.GetName(tableName))[0];
			switch(tableName) {
				case "XPObjectType":
				case "XPWeakReference":
				case "EventHandlerLink":
				case "XPDeletedObject":
					message = "Internal XPO table";
					return false;
			}
			if(table.PrimaryKey == null) {
				message = "Table has no primary key";
				return false;
			}
			if(table.PrimaryKey.Columns.Count > 1) {
				message = "Generator doesn't support composite (multi-column) keys";
				return false;
			}
			message = string.Empty;
			return true;
		}
		private static string DetectTextSplitter(string text) {
			string newSplitter = string.Empty;
			bool isR = false;
			bool isN = false;
			for(int i = 0; i < text.Length; i++) {
				char c = text[i];
				if(c == '\r') {
					if(isR) { break; }
					newSplitter += c;
					isR = true;
				} else if(c == '\n') {
					if(isN) { break; }
					newSplitter += c;
					isN = true;
				} else {
					if(isN || isR) { break; }
				}
			}
			if(string.IsNullOrEmpty(newSplitter)) newSplitter = "\r\n";
			return newSplitter;
		}
		private static string[] GetTextLines(string text, string splitter) {
			return text.Split(new string[] { splitter }, StringSplitOptions.None);
		}
		private string CommentAllLines(string text) {
			if(string.IsNullOrEmpty(text)) return string.Empty;
			string splitter = DetectTextSplitter(text);
			string[] lines = GetTextLines(text, splitter);
			for(int i = 0; i < lines.Length; i++) {
				lines[i] = string.Concat(GetLanguageKeyword(PersistentClassGeneratorKeywords.Comment), lines[i]);
			}
			return string.Join(splitter, lines);
		}
		public void Generate(TextWriter w, TextWriter log, string connectionString, IDataStoreSchemaExplorer prov, Dictionary<string, bool> tablesMask, TableColumnsCheckContainer tables,  string dbName) {
			Generate(w, log, connectionString, prov, tablesMask, tables, null, null, dbName);
		}
		public void Generate(TextWriter w, TextWriter log, string connectionString, IDataStoreSchemaExplorer prov, Dictionary<string, bool> tablesMask, TableColumnsCheckContainer tables, Dictionary<string, bool> sprocMasks, SProcsResultSetCheckContainer sprocedures, string dbName) {
			Generate(w, log, connectionString, prov, tablesMask, tables, sprocMasks, sprocedures, dbName, false);
		}
		public void Generate(TextWriter w, TextWriter log, string connectionString, IDataStoreSchemaExplorer prov, Dictionary<string, bool> tablesMask, TableColumnsCheckContainer tables, string dbName, bool storedProcedures) {
			Generate(w, log, connectionString, prov, tablesMask, tables, null, null, dbName);
		}
		public class AssociationInfo {
			public string Name;
			public string ReferenceOwnerType;
			public string ReferencePropertyName;
			public string CollectionOwnerType;
			public string GetCollectionPropertyName(Dictionary<string, bool> propertyNames, Func<string, string> fixProperty) {				
				string propertyName = ReferenceOwnerType == CollectionOwnerType ? ReferenceOwnerType + "Collection" : ReferenceOwnerType.EndsWith("s") ? ReferenceOwnerType : ReferenceOwnerType + "s";
				if(fixProperty != null) propertyName = fixProperty(propertyName);
				if(!propertyNames.ContainsKey(propertyName)){
					propertyNames.Add(propertyName, true);
					return propertyName;
				}
				propertyName = string.Concat(ReferenceOwnerType, ReferencePropertyName, "Collection");
				if(fixProperty != null) propertyName = fixProperty(propertyName);
				if(!propertyNames.ContainsKey(propertyName)) {
					propertyNames.Add(propertyName, true);
					return propertyName;
				}
				throw new InvalidOperationException(string.Format("GetCollectionPropertyName: \"{0}\"", propertyName));
			}
			public AssociationInfo(string className, string propertyName, string referenceType) {
				ReferencePropertyName = propertyName;
				ReferenceOwnerType = className;
				CollectionOwnerType = referenceType;
				Name = string.Format("{0}.{1}_{2}", className, ReferencePropertyName, referenceType);
			}
		}
		public void Generate(TextWriter w, TextWriter log, string connectionString, IDataStoreSchemaExplorer prov, Dictionary<string, bool> tablesMask, TableColumnsCheckContainer tables, Dictionary<string, bool> sprocMasks, SProcsResultSetCheckContainer sprocedures, string dbName, bool storedProcedures) {
			ConnectionProviderSql provider = prov as ConnectionProviderSql;
			Hashtable list = new Hashtable();
			List<string> dropList = null;
			if(storedProcedures) {
				dropList = new List<string>();
				if(SupportMultilineComment) {
					w.WriteLine(GetLanguageKeyword(PersistentClassGeneratorKeywords.MultilineCommentOpen));
					w.WriteLine(provider.GenerateStoredProceduresInfoOnce());
				} else {
					w.WriteLine(CommentAllLines(provider.GenerateStoredProceduresInfoOnce()));
				}
			}
			Dictionary<string, Columns> columnsDict = new Dictionary<string, Columns>();
			Dictionary<string, List<AssociationInfo>> associationDict = new Dictionary<string, List<AssociationInfo>>();
			foreach(KeyValuePair<string, bool> tableMask in tablesMask) {
				if(!tableMask.Value) { continue; }
				string tableName = tableMask.Key;
				DBTable table = tables.GetStorageTable(StructurePage.GetName(tableName));
				switch(tableName) {
					case "XPObjectType":
					case "XPWeakReference":
					case "EventHandlerLink":
					case "XPDeletedObject":
						continue;
				}
				Columns columns = tables.GetTableColumns(table.Name);
				if((table.PrimaryKey != null) && (table.PrimaryKey.Columns.Count == 1) && columns[table.PrimaryKey.Columns[0]].Selected) {
					list.Add(tableName, table);
					if(storedProcedures) {
						string dropLines;
						if(SupportMultilineComment) {
							w.WriteLine(provider.GenerateStoredProcedures(table, out dropLines));
						} else {
							w.WriteLine(CommentAllLines(provider.GenerateStoredProcedures(table, out dropLines)));
						}
						if(!string.IsNullOrEmpty(dropLines))dropList.Add(dropLines);
					}
				}
				else {
					log.WriteLine(tableName + " no single column pk");
				}
				columnsDict.Add(tableName, columns);
			}
			if(storedProcedures) {
				if(SupportMultilineComment) w.WriteLine(GetLanguageKeyword(PersistentClassGeneratorKeywords.MultilineCommentClose));
				w.WriteLine();
				if(SupportMultilineComment) w.WriteLine(GetLanguageKeyword(PersistentClassGeneratorKeywords.MultilineCommentOpen));
				if(dropList.Count > 0) {
					for(int i = 0; i < dropList.Count; i++) {
						if(SupportMultilineComment) {
							w.WriteLine(dropList[i]);
						} else {
							w.WriteLine(CommentAllLines(dropList[i]));
						}
					}
					w.WriteLine();
				}
				if(SupportMultilineComment) w.WriteLine(GetLanguageKeyword(PersistentClassGeneratorKeywords.MultilineCommentClose));
				w.WriteLine();
			}
			WriteUsing(w);
			Dictionary<string, string> tableClassNameCache = new Dictionary<string, string>();
			Dictionary<string, bool> classNameDict = new Dictionary<string, bool>(IsCaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase);
			StartNamespace(w, !String.IsNullOrEmpty(dbName) ? dbName : "DBObjects");
			if(!string.IsNullOrEmpty(connectionString)) DeclareConnectionHelper(w, connectionString);
			foreach(string key in list.Keys) {
				DBTable table = (DBTable)list[key];
				string className = GetClassName(StructurePage.GetCaption(key), tableClassNameCache, classNameDict);
				Columns columns = columnsDict[key];
				foreach(DBColumn column in table.Columns) {
					if(((IsLockingField(column.Name, column.ColumnType) || IsGCRecordField(column.Name, column.ColumnType)) || IsObjectTypeField(column.Name, column.ColumnType)) || !columns[column.Name].Selected)
						continue;
					string type = GetLanguageType(column.ColumnType);
					if(String.IsNullOrEmpty(type)) {
						log.WriteLine(table.Name + " - " + column.Name + " not mapped");
						continue;
					}
					bool fkFound = false;
					foreach(DBForeignKey fk in table.ForeignKeys) {
						if(fk.Columns[0] == column.Name) {
							foreach(DictionaryEntry s in list)
								if(fk.PrimaryKeyTable == StructurePage.GetName((string)s.Key) &&
									(key != (string)s.Key || column.Name != table.PrimaryKey.Columns[0]) &&
									(((DBTable)s.Value).PrimaryKey.Columns[0] == fk.PrimaryKeyTableKeyColumns[0])) {
									type = GetClassName(StructurePage.GetCaption((string)s.Key), tableClassNameCache, classNameDict);
									fkFound = true;
									break;
								}
						}
					}
					if(!fkFound) continue;
					string propertyName = GetPropertyName(className, columns, column);
					AssociationInfo association = new AssociationInfo(className, propertyName, type);
					List<AssociationInfo> aList;
					if(!associationDict.TryGetValue(className, out aList)) {
						aList = new List<AssociationInfo>();
						associationDict.Add(className, aList);
					}
					aList.Add(association);
					if(className != type) {
						if(!associationDict.TryGetValue(type, out aList)) {
							aList = new List<AssociationInfo>();
							associationDict.Add(type, aList);
						}
						aList.Add(association);
					}
				}
			}
			foreach(string key in list.Keys) {
				DBTable table = (DBTable)list[key];
				w.WriteLine();
				string className = GetClassName(StructurePage.GetCaption(key), tableClassNameCache, classNameDict);
				string baseClassName;
				if(storedProcedures) { WriteClassPersistentTag(w, table.Name + "_xpoView"); } else
					if(className != table.Name) WriteClassPersistentTag(w, table.Name);
				if(!HasLockingField(list, table)) {
					if(HasGCRecordField(table)) {
						TurnoffOptimisticLocking(w);
						baseClassName = "XPCustomObject";
					} else baseClassName = "XPLiteObject";
				} else {
					if(HasGCRecordField(table)) baseClassName = "XPCustomObject";
					else baseClassName = "XPBaseObject";
				}
				DeclareClass(w, className, baseClassName);
				Columns columns = tables.GetTableColumns(table.Name);
				List<AssociationInfo> aList;
				if(!associationDict.TryGetValue(className, out aList)){
					aList = null;
				}
				Dictionary<string, bool> propertyNameDict = new Dictionary<string, bool>(IsCaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase);
				foreach(DBColumn column in table.Columns) {
					if(((IsLockingField(column.Name, column.ColumnType) || IsGCRecordField(column.Name, column.ColumnType)) || IsObjectTypeField(column.Name, column.ColumnType)) || !columns[column.Name].Selected)
						continue;
					string type = GetLanguageType(column.ColumnType);
					if(String.IsNullOrEmpty(type)) {
						log.WriteLine(table.Name + " - " + column.Name + " not mapped");
						continue;
					}
					string associationName = null;
					string propertyName = GetPropertyName(className, columns, column);
					if(aList != null) {
						foreach(AssociationInfo ai in aList) {
							if(ai.ReferenceOwnerType == className && ai.ReferencePropertyName == propertyName) {
								type = ai.CollectionOwnerType;
								associationName = ai.Name;
								break;
							}
						}
					}
					string fieldName = "f" + propertyName;
					DeclareField(w, type, fieldName);
					if(column.Name == table.PrimaryKey.Columns[0]) {
						if(column.IsIdentity || (column.ColumnType == DBColumnType.Guid))
							WriteIdentityKey(w);
						else
							WriteKey(w);
						if(storedProcedures && column.IsIdentity && prov is MSSqlConnectionProvider) {
							WritePersistentProperty(w, ConnectionProviderSql.IdentityColumnMagicName);
						}
					}
					if(column.ColumnType == DBColumnType.String && column.Size != SizeAttribute.DefaultStringMappingFieldSize)
						WritePropertySize(w, column.Size);
					if(propertyName != column.Name)
						WritePersistentProperty(w, column.Name);
					if(!string.IsNullOrEmpty(associationName))
						WriteClassAssociationTag(w, associationName);
					propertyNameDict.Add(propertyName, true);
					DeclareProperty(w, type, propertyName, fieldName, column.Name);
				}
				if(aList != null) {
					foreach(AssociationInfo ai in aList) {
						if(ai.CollectionOwnerType == className) {
							WriteClassAssociationTag(w, ai.Name);
							DeclareCollectionProperty(w, ai.ReferenceOwnerType, ai.GetCollectionPropertyName(propertyNameDict, FixPropertyName));
						}
					}
				}
				WriteClassConstructors(w, className);
				CloseClass(w);
			}
			if(prov != null && sprocMasks != null && sprocMasks.Count > 0 && sprocedures != null) {
				GenerateSP(w, log, prov, sprocMasks, sprocedures, dbName);
			}
			w.WriteLine();
			CloseNamespace(w);
		}
		string FixPropertyName(string propertyName) {
			string result = propertyName;
			while(!IsPropertyNameReserved(result))
				result = "_" + result;
			return ValidatePropertyName(result);
		}
		string GetPropertyName(string className, Columns columns, DBColumn column) {
			string propertyName = StructurePage.GetCaption(columns[column.Name].Name);
			if(propertyName.IndexOf(" ") >= 0) propertyName = propertyName.Replace(" ", "");
			if(propertyName == className) propertyName += "_";
			propertyName = FixPropertyName(propertyName);
			return propertyName;
		}
#if DEBUGTEST        
public
#endif
 void GenerateSP(TextWriter w, TextWriter log, IDataStoreSchemaExplorer prov, Dictionary<string, bool> sprocMasks, SProcsResultSetCheckContainer sprocedures, string dbName) {
			ConnectionProviderSql provider = prov as ConnectionProviderSql;
			Dictionary<string, DBStoredProcedure> list = new Dictionary<string, DBStoredProcedure>();
			DBStoredProcedure[] sprocList = sprocedures.SprocList;
			if(sprocList == null || sprocList.Length == 0) return;
			for(int i = 0; i < sprocList.Length; i++) {
				list[sprocList[i].Name]= sprocList[i];
			}
			foreach(KeyValuePair<string, bool> sprocMask in sprocMasks) {
				if(sprocMask.Value) continue;
				list.Remove(sprocMask.Key);
			}
			Dictionary<string, bool> existsClassNameDict = new Dictionary<string, bool>(IsCaseSensitive ? StringComparer.CurrentCulture : StringComparer.CurrentCultureIgnoreCase);
			Dictionary<string, string> classNameDict = new Dictionary<string, string>();
			Dictionary<string, LoadDataMemberOrderItem[]> loadProcOrderDict = new Dictionary<string, LoadDataMemberOrderItem[]>();
			foreach(string key in list.Keys) {
				DBStoredProcedure sproc = list[key];
				string realClassName = GetClassName(StructurePage.GetCaption(key), null, null);
				string className = CheckClassName(existsClassNameDict, classNameDict, realClassName);
				if(sproc.ResultSets.Count != 1) continue;
				List<LoadDataMemberOrderItem> memberOrderList = null;
				w.WriteLine();
				WriteClassNonPersistentTag(w);
				string baseClassName = "PersistentBase";
				DeclareClass(w, className, baseClassName);
				ResultSetColumns columns = sprocedures.GetResultSetColumns(sproc.Name);
				int indexInResultSet = 0;
				int columnsCount = sproc.ResultSets[0].Columns.Count;
				for(int c = 0; c < columnsCount; c++){
					DBNameTypePair column = sproc.ResultSets[0].Columns[c];
					if(((IsLockingField(column.Name, column.Type) || IsGCRecordField(column.Name, column.Type)) || IsObjectTypeField(column.Name, column.Type)) || !columns[c].Selected)
						continue;
					string type = GetLanguageType(column.Type);
					if(String.IsNullOrEmpty(type)) {
						log.WriteLine(sproc.Name + " - " + column.Name + " not mapped");
						continue;
					}
					string propertyName = StructureSPPage.GetName(columns[c].Name);
					propertyName = TitleCase(propertyName.Replace(" ", "").Replace("@", ""));
					if(propertyName == className) propertyName += "_";
					while(!IsPropertyNameReserved(propertyName))
						propertyName = "_" + propertyName;
					propertyName = ValidatePropertyName(propertyName);
					string fieldName = "f" + propertyName;
					DeclareField(w, type, fieldName);
					DeclareProperty(w, type, propertyName, fieldName, column.Name);
					if(memberOrderList == null){
						memberOrderList = new List<LoadDataMemberOrderItem>();
					}
					memberOrderList.Add(new LoadDataMemberOrderItem(indexInResultSet++, propertyName));
				}
				if(memberOrderList != null && memberOrderList.Count > 0){
					if(memberOrderList.Count == sproc.ResultSets[0].Columns.Count)
						loadProcOrderDict.Add(className, null);
					else
						loadProcOrderDict.Add(className, memberOrderList.ToArray());
				}
				WriteClassConstructors(w, className);
				CloseClass(w);
			}
			if(list.Count > 0) {
				string classHelperName = GetClassName(TitleCase(dbName) + "SprocHelper", null, null);
				DeclareStaticClass(w, classHelperName);
				foreach(string key in list.Keys) {
					DBStoredProcedure sproc = list[key];
					w.WriteLine();
					string className = classNameDict[GetClassName(StructurePage.GetCaption(key), null, null)];
					string methodArgsWithSession = GetArgument("session", "Session");
					string methodArgs = string.Empty;
					string[] argNames = null;
					if(sproc.Arguments.Count > 0) {
						methodArgs = GetMethodArguments(sproc.Arguments, out argNames);
						if(!string.IsNullOrEmpty(methodArgs)) {
							methodArgsWithSession = methodArgsWithSession + ", " + methodArgs;
						}
					} else {
						argNames = new string[0];
					}
					DeclareSprocToSelectedData(w, key, className, methodArgsWithSession, argNames);
					if(sproc.ResultSets.Count != 1) continue;
					LoadDataMemberOrderItem[] memberOrder;
					if(!loadProcOrderDict.TryGetValue(className, out memberOrder)) continue;
					string orderArrayName = null;
					if(memberOrder != null) {
						w.WriteLine();
						orderArrayName = DeclareOrderArray(w, className, memberOrder);
					}
					w.WriteLine();
					DeclareSprocToObjectsGeneric(w, key, className, methodArgsWithSession, argNames, memberOrder, orderArrayName);
					w.WriteLine();
					DeclareSprocToDataViewCreate(w, key, className, methodArgsWithSession, argNames, memberOrder, orderArrayName);
					DeclareSprocToDataView(w, key, className, methodArgsWithSession, argNames, memberOrder, orderArrayName);
				}
				CloseStaticClass(w);
			}
			w.WriteLine();
		}
		static string CheckClassName(Dictionary<string, bool> existsClassNameDict, Dictionary<string, string> classNameDict, string realClassName) {
			string className = realClassName;
			int additionalSuffixCounter = 0;
			while(existsClassNameDict.ContainsKey(className)) {
				className = string.Format(CultureInfo.InvariantCulture, "{0}_{D:2}", realClassName, additionalSuffixCounter++);
			}
			existsClassNameDict[className] = true;
			classNameDict[realClassName] = className;
			return className;
		}
		void DeclareSprocToSelectedData(TextWriter w, string key, string className, string methodArgs, string[] argNames) {
			string methodResultType = "DevExpress.Xpo.DB.SelectedData";
			string body = string.Format("{0} session.ExecuteSproc(\"{1}\"{2}){3}",
				GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
				key,
				GetAdditionalArgsString(argNames),
				GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			DeclareStaticFunction(w, "Exec" + className, methodArgs, methodResultType, new string[] { body });
		}
		static string GetAdditionalArgsString(string[] argNames) {
			return argNames == null || argNames.Length == 0 ? string.Empty : ", " + string.Join(", ", argNames);
		}
		void DeclareSprocToObjects(TextWriter w, string key, string className, string methodArgs, string[] argNames, LoadDataMemberOrderItem[] memberOrder, string orderArrayName) {
			string methodResultType = "System.Collections.ICollection";
			string body;
			if(memberOrder == null) {
				body = string.Format("{0} session.GetObjectsFromSproc(session.GetClassInfo({1}({2})), \"{3}\"{4}){5}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.TypeOf),
					className,
					key,
					GetAdditionalArgsString(argNames),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			} else {
				body = string.Format("{0} session.GetObjectsFromSproc(session.GetClassInfo({1}({2})), {3}, \"{4}\"{5}){6}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.TypeOf),
					className,
					orderArrayName,
					key,
					GetAdditionalArgsString(argNames),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			}
			DeclareStaticFunction(w, "Exec" + className + "IntoObjects", methodArgs, methodResultType, new string[] { body });
		}
		void DeclareSprocToObjectsGeneric(TextWriter w, string key, string className, string methodArgs, string[] argNames, LoadDataMemberOrderItem[] memberOrder, string orderArrayName) {
			string methodResultType = "System.Collections.Generic.ICollection" + GetGeneric(className);
			string body;
			if(memberOrder == null) {
				body = string.Format("{0} session.GetObjectsFromSproc{1}(\"{2}\"{3}){4}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
					GetGeneric(className),
					key,
					GetAdditionalArgsString(argNames),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			} else {
				body = string.Format("{0} session.GetObjectsFromSproc{1}({2}, \"{3}\"{4}){5}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
					GetGeneric(className),
					orderArrayName,
					key,
					GetAdditionalArgsString(argNames),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			}
			DeclareStaticFunction(w, "Exec" + className + "IntoObjects", methodArgs, methodResultType, new string[] { body });
		}
		void DeclareSprocToDataViewCreate(TextWriter w, string key, string className, string methodArgs, string[] argNames, LoadDataMemberOrderItem[] memberOrder, string orderArrayName) {
			string methodResultType = "XPDataView";
			string[] body = new string[2];
			string sprocDataName = "sprocData";
			body[0] = string.Format("{0} = session.ExecuteSproc(\"{1}\"{2}){3}",
				GetVariableDeclaration("DevExpress.Xpo.DB.SelectedData", sprocDataName),
				key,
				GetAdditionalArgsString(argNames),
				GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			if(memberOrder == null) {
				body[1] = string.Format("{0} {1} XPDataView(session.Dictionary, session.GetClassInfo({2}({3})), {4}){5}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.New),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.TypeOf),
					className,
					sprocDataName,
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			} else {
				body[1] = string.Format("{0} {1} XPDataView(session.Dictionary, session.GetClassInfo({2}({3})), {4}, {5}){6}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.Return),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.New),
					GetLanguageKeyword(PersistentClassGeneratorKeywords.TypeOf),
					className,
					orderArrayName,
					sprocDataName,
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			}
			DeclareStaticFunction(w, "Exec" + className + "IntoDataView", methodArgs, methodResultType, body);
		}
		void DeclareSprocToDataView(TextWriter w, string key, string className, string methodArgs, string[] argNames, LoadDataMemberOrderItem[] memberOrder, string orderArrayName) {
			string methodResultType = string.Empty;
			string[] body = new string[3];
			string sprocDataName = "sprocData";
			body[0] = string.Format("{0} = session.ExecuteSproc(\"{1}\"{2}){3}",
				GetVariableDeclaration("DevExpress.Xpo.DB.SelectedData", sprocDataName),
				key,
				GetAdditionalArgsString(argNames),
				GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			if(memberOrder == null) {
				body[1] = string.Format("dataView.PopulateProperties(session.GetClassInfo({0}({1}))){2}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.TypeOf),
					className,
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
				body[2] = string.Format("dataView.LoadData({0}){1}",
					sprocDataName,
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			} else {
				body[1] = string.Format("dataView.PopulatePropertiesOrdered(session.GetClassInfo({0}({1})), {2}){3}",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.TypeOf),
					className,
					orderArrayName,
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
				body[2] = string.Format("dataView.LoadOrderedData({0}, {1}){2}",
					orderArrayName,
					sprocDataName,
					GetLanguageKeyword(PersistentClassGeneratorKeywords.OperatorEnd));
			}
			DeclareStaticFunction(w, "Exec" + className + "IntoDataView", 
				GetArgument("dataView", "XPDataView") + (string.IsNullOrEmpty(methodArgs) ? string.Empty : ", " + methodArgs),
				methodResultType, body);
		}
		string DeclareOrderArray(TextWriter w, string className, LoadDataMemberOrderItem[] memberOrder) {
			string[] elements = new string[memberOrder.Length];
			for(int i = 0; i < memberOrder.Length; i++) {
				elements[i] = string.Format("{0} LoadDataMemberOrderItem({1}, \"{2}\")",
					GetLanguageKeyword(PersistentClassGeneratorKeywords.New),
					memberOrder[i].IndexInResultSet,
					memberOrder[i].ClassMemberName);
			}
			string arrayName = className + "OrderArray";
			DeclarePrivatStaticArray(w, arrayName, "LoadDataMemberOrderItem", elements);
			return arrayName;
		}
		string GetMethodArguments(List<DBStoredProcedureArgument> sprocArgs, out string[] argNames) {
			Dictionary<string, bool> argNamesDict = IsCaseSensitive ? new Dictionary<string, bool>() : new Dictionary<string, bool>(StringComparer.InvariantCultureIgnoreCase);
			List<string> args = new List<string>(sprocArgs.Count);
			List<string> argNamesList = new List<string>(sprocArgs.Count);
			for(int i = 0; i < sprocArgs.Count; i++) {
				if(sprocArgs[i].Direction == DBStoredProcedureArgumentDirection.Out) continue;
				string name = sprocArgs[i].Name.Replace("@", "").Replace(" ", "");
				while(argNamesDict.ContainsKey(name) || !IsPropertyNameReserved(name)) { name = "_" + name; }
				args.Add(GetArgument(name, GetLanguageType(sprocArgs[i].Type)));
				argNamesList.Add(name);
			}
			argNames = argNamesList.ToArray();
			for(int i = 0; i < argNames.Length; i++) {
				argNames[i] = string.Format("{0} OperandValue({1})", GetLanguageKeyword(PersistentClassGeneratorKeywords.New), argNames[i]);
			}
			return string.Join(", ", args.ToArray());
		}
		protected abstract void WriteUsing(TextWriter w);
		protected abstract void WriteConnectionComment(TextWriter w, string connectionString);
		protected abstract void StartNamespace(TextWriter w, string name);
		protected abstract void CloseNamespace(TextWriter w);
		protected abstract void TurnoffOptimisticLocking(TextWriter w);
		protected abstract void WriteClassPersistentTag(TextWriter w, string name);
		protected abstract void WriteClassAssociationTag(TextWriter w, string name);
		protected abstract void WriteClassNonPersistentTag(TextWriter w);
		protected abstract void DeclareClass(TextWriter w, string className, string baseClassName);
		protected abstract void CloseClass(TextWriter w);
		protected abstract void DeclareStaticClass(TextWriter w, string className);
		protected abstract void CloseStaticClass(TextWriter w);
		protected abstract void WriteClassConstructors(TextWriter w, string className);
		protected abstract void DeclareCollectionProperty(TextWriter w, string type, string propertyName);
		protected abstract void DeclareStaticFunction(TextWriter w, string functionName, string arguments, string resultType, string[] body);
		protected abstract void DeclarePrivatStaticArray(TextWriter w, string arrayName, string arrayType, string[] elements);
		protected abstract string GetVariableDeclaration(string className, string variableName);
		protected abstract void DeclareConnectionHelper(TextWriter w, string connectionString);
		protected abstract string GetLanguageKeyword(PersistentClassGeneratorKeywords keyword);
		protected abstract string GetLanguageType(DBColumnType columnType);
		protected abstract string GetArgument(string name, string type);
		protected abstract string GetGeneric(string className);
		protected abstract void DeclareField(TextWriter w, string type, string columnName);
		protected abstract void WriteKey(TextWriter w);
		protected abstract void WriteIdentityKey(TextWriter w);
		protected abstract void WritePersistentProperty(TextWriter w, string columnName);
		protected abstract void WritePropertySize(TextWriter w, int size);
		protected abstract bool IsCaseSensitive { get; }
		protected abstract bool IsPropertyNameReserved(string propertyName);
		protected abstract bool SupportMultilineComment { get; }
		protected virtual string ValidatePropertyName(string propertyName) {
			StringBuilder res = new StringBuilder(propertyName.Length);
			for(int i = 0; i < propertyName.Length; i++) {
				if(IsValidChar(propertyName[i], i == 0))
					res.Append(propertyName[i]);
				else {
					if(IsFirstDigit(propertyName[i], i == 0)) {
						res.AppendFormat("_N{0}", propertyName[i]);
					} else {
						res.Append('_');
					}
				}
			}
			return res.ToString();
		}
		protected bool IsValidChar(char p, bool isFirst) {
			return Char.IsDigit(p) && !isFirst || Char.IsLetter(p);
		}
		protected bool IsFirstDigit(char p, bool isFirst) {
			return Char.IsDigit(p) && isFirst;
		}
		protected abstract void DeclareProperty(TextWriter w, string type, string propertyName, 
			string fieldName, string columnName);
		string GetClassName(string tableName, Dictionary<string, string> tableClassNameCache, Dictionary<string, bool> classNameDict) {
			string className;
			if(tableClassNameCache != null && classNameDict != null) {
				if(tableClassNameCache.TryGetValue(tableName, out className)) return className;
			}
			className = tableName;
			if((className.IndexOf(' ') >= 0) || (className.IndexOf('.') >= 0)) {
				className = className.Replace(" ", "").Replace(".", "_");
			}
			if(classNameDict != null && tableClassNameCache != null) {
				while(classNameDict.ContainsKey(className)) {
					className += "_";
				}
				tableClassNameCache.Add(tableName, className);
				classNameDict.Add(className, true);
			}
			return className;
		}
		string TitleCase(string input) {
			if(string.IsNullOrEmpty(input)) return string.Empty;
			return char.ToUpperInvariant(input[0]) + input.Substring(1, input.Length - 1);
		}
		bool HasGCRecordField(DBTable table) {
			foreach(DBColumn col in table.Columns) {
				if(IsGCRecordField(col.Name, col.ColumnType)) {
					return true;
				}
			}
			return false;
		}
		bool HasLockingField(Hashtable list, DBTable table) {
			foreach(DBColumn col in table.Columns) {
				if(IsLockingField(col.Name, col.ColumnType)) {
					return true;
				}
			}
			return false;
		}
		bool HasObjectTypeField(DBTable table) {
			foreach(DBColumn col in table.Columns) {
				if(IsObjectTypeField(col.Name, col.ColumnType)) {
					return true;
				}
			}
			return false;
		}
		bool IsGCRecordField(string columnName, DBColumnType columnType) {
			return ((columnType == DBColumnType.Int32) && (columnName == GCRecordField.StaticName));
		}
		bool IsLockingField(string columnName, DBColumnType columnType) {
			return ((columnType == DBColumnType.Int32) && (columnName == "OptimisticLockField"));
		}
		bool IsObjectTypeField(string columnName, DBColumnType columnType) {
			return ((columnType == DBColumnType.Int32) && (columnName == "ObjectType"));
		}
	}
	internal class CSharpPersistentClassGenerator : PersistentClassGenerator {
		string[] reservedWordsArray = new string[]{ "abstract", "event", "new", "struct",
				"as", "explicit", "null", "switch", "base", "extern", "object",
				"this", "bool", "false", "operator", "throw", "break", "finally",
				"out", "true", "byte", "fixed", "override", "try", "case",
				"float", "params", "typeof", "catch", "for", "private", "uint",
				"char", "foreach", "protected", "ulong", "checked", "goto",
				"public", "unchecked", "class", "if", "readonly", "unsafe",
				"const", "implicit", "ref", "ushort", "continue", "in",
				"return", "using", "decimal", "int", "sbyte", "virtual",
				"default", "interface", "sealed", "volatile", "delegate",
				"internal", "short", "void", "do", "is", "sizeof", "while",
				"double", "lock", "stackalloc", "else", "long", "static",
				"enum", "namespace", "string", "get", "partial", "set",
				"value", "where", "yield" };
		protected override string[] ReservedWordsArray { get { return reservedWordsArray; } }
		protected override bool SupportMultilineComment { get { return true; } }
		protected override void WriteUsing(TextWriter w) {
			w.WriteLine("using System;");
			w.WriteLine("using DevExpress.Xpo;");
			w.WriteLine("using DevExpress.Data.Filtering;");
		}
		protected override void StartNamespace(TextWriter w, string name) {
			w.WriteLine("namespace " + name + " {");
		}
		protected override void WriteConnectionComment(TextWriter w, string connectionString) {
			throw new NotSupportedException();
		}
		protected override void CloseNamespace(TextWriter w) {
			w.WriteLine("}");
		}
		protected override void TurnoffOptimisticLocking(TextWriter w) {
			w.WriteLine("\t[OptimisticLocking(false)]");
		}
		protected override void WriteClassPersistentTag(TextWriter w, string name) {
			w.WriteLine("\t[Persistent(@\"" + name + "\")]");
		}
		protected override void WriteClassAssociationTag(TextWriter w, string name) {
			w.WriteLine("\t\t[Association(@\"" + name + "\")]");
		}
		protected override void WriteClassNonPersistentTag(TextWriter w) {
			w.WriteLine("\t[NonPersistent]");
		}
		protected override void DeclareClass(TextWriter w, string className, string baseClassName) {
			w.WriteLine("\tpublic class " + className + " : " + baseClassName + " {");
		}
		protected override void CloseClass(TextWriter w) {
			w.WriteLine("\t}");
		}
		protected override void DeclareStaticClass(TextWriter w, string className) {
			w.WriteLine("\tpublic static class " + className + " {");
		}
		protected override void CloseStaticClass(TextWriter w) {
			w.WriteLine("\t}");
		}
		protected override void WriteClassConstructors(TextWriter w, string className) {
			w.WriteLine("\t\tpublic " + className + "(Session session) : base(session) { }");
			w.WriteLine("\t\tpublic " + className + "() : base(Session.DefaultSession) { }");
			w.WriteLine("\t\tpublic override void AfterConstruction() { base.AfterConstruction(); }");
		}
		protected override void DeclareConnectionHelper(TextWriter w, string connectionString) {
			w.WriteLine("\tpublic static class ConnectionHelper {");
			w.WriteLine(string.Concat("\t\tpublic const string ConnectionString = @\"", connectionString.Replace("\"","\"\""), "\";"));
			w.WriteLine("\t\tpublic static void Connect(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption) {");
			w.WriteLine("\t\t\tXpoDefault.DataLayer = XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);");
			w.WriteLine("\t\t\tXpoDefault.Session = null;");
			w.WriteLine("\t\t}");
			w.WriteLine("\t\tpublic static DevExpress.Xpo.DB.IDataStore GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption) {");
			w.WriteLine("\t\t\treturn XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption);");
			w.WriteLine("\t\t}");
			w.WriteLine("\t\tpublic static DevExpress.Xpo.DB.IDataStore GetConnectionProvider(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption, out IDisposable[] objectsToDisposeOnDisconnect) {");
			w.WriteLine("\t\t\treturn XpoDefault.GetConnectionProvider(ConnectionString, autoCreateOption, out objectsToDisposeOnDisconnect);");
			w.WriteLine("\t\t}");
			w.WriteLine("\t\tpublic static IDataLayer GetDataLayer(DevExpress.Xpo.DB.AutoCreateOption autoCreateOption) {");
			w.WriteLine("\t\t\treturn XpoDefault.GetDataLayer(ConnectionString, autoCreateOption);");
			w.WriteLine("\t\t}");
			w.WriteLine("\t}");
		}
		protected override string GetLanguageType(DBColumnType columnType) {
			switch(columnType) {
				case DBColumnType.Boolean:
					return "bool";
				case DBColumnType.Byte:
					return "byte";
				case DBColumnType.SByte:
					return "sbyte";
				case DBColumnType.Char:
					return "char";
				case DBColumnType.Decimal:
					return "decimal";
				case DBColumnType.Single:
					return "float";
				case DBColumnType.Double:
					return "double";
				case DBColumnType.Int32:
					return "int";
				case DBColumnType.UInt32:
					return "uint";
				case DBColumnType.Int16:
					return "short";
				case DBColumnType.UInt16:
					return "ushort";
				case DBColumnType.Int64:
					return "long";
				case DBColumnType.UInt64:
					return "ulong";
				case DBColumnType.String:
					return "string";
				case DBColumnType.DateTime:
					return "DateTime";
				case DBColumnType.Guid:
					return "Guid";
				case DBColumnType.TimeSpan:
					return "TimeSpan";
				case DBColumnType.ByteArray:
					return "byte[]";
				default:
					return "";
			}
		}
		protected override void DeclarePrivatStaticArray(TextWriter w, string arrayName, string arrayType, string[] elements) {
			w.WriteLine("\t\tstatic " + arrayType + "[] " + arrayName + " = {" + string.Join(", ", elements) + "};");
		}
		protected override bool IsCaseSensitive {
			get { return true; }
		}
		protected override string GetArgument(string name, string type) {
			return type + " " + name;
		}
		protected override void DeclareStaticFunction(TextWriter w, string functionName, string arguments, string resultType, string[] body) {
			w.WriteLine("\t\tpublic static " + (string.IsNullOrEmpty(resultType) ? "void" : resultType)  + " " + functionName + "(" + arguments + "){");
			for(int i = 0; i < body.Length; i++) {
				w.WriteLine("\t\t\t" + body[i]);
			}
			w.WriteLine("\t\t}");
		}
		protected override void DeclareField(TextWriter w, string type, string columnName) {
			w.WriteLine("\t\t" + type + " " + columnName + ";");
		}
		protected override void WriteKey(TextWriter w) {
			w.WriteLine("\t\t[Key]");
		}
		protected override void WriteIdentityKey(TextWriter w) {
			w.WriteLine("\t\t[Key(true)]");
		}
		protected override void WritePropertySize(TextWriter w, int size) {
			w.WriteLine("\t\t[Size(" + ((size <= 0 || size > 10000) ? "SizeAttribute.Unlimited" : size.ToString(CultureInfo.InvariantCulture)) + ")]");
		}
		protected override void WritePersistentProperty(TextWriter w, string columnName) {
			w.WriteLine("\t\t[Persistent(@\"" + columnName + "\")]");
		}
		protected override bool IsPropertyNameReserved(string propertyName) {
			return !ReservedWords.ContainsKey(propertyName);
		}
		protected override void DeclareProperty(TextWriter w, string type, string propertyName,
																string fieldName, string columnName) {
			w.WriteLine("\t\tpublic " + type + " " + propertyName + " {");
			w.WriteLine("\t\t\tget { return " + fieldName + "; }");
			w.WriteLine("\t\t\tset { SetPropertyValue<" + type + ">(\"" + propertyName + "\", ref " + fieldName + ", value); }");
			w.WriteLine("\t\t}");
		}
		protected override void DeclareCollectionProperty(TextWriter w, string type, string propertyName) {
			w.WriteLine("\t\tpublic XPCollection<"+ type +"> "+ propertyName + " {");
			w.WriteLine("\t\t\tget { return GetCollection<" + type + ">(@\""+ propertyName + "\"); }");
			w.WriteLine("\t\t}");
		}
		protected override string GetVariableDeclaration(string className, string variableName) {
			return string.Format("{0} {1}", className, variableName);
		}
		protected override string GetGeneric(string className) {
			return "<" + className + ">";
		}
		protected override string GetLanguageKeyword(PersistentClassGeneratorKeywords keyword) {
			switch(keyword) {
				case PersistentClassGeneratorKeywords.Null:
					return "null";
				case PersistentClassGeneratorKeywords.Return:
					return "return";
				case PersistentClassGeneratorKeywords.New:
					return "new";
				case PersistentClassGeneratorKeywords.TypeOf:
					return "typeof";
				case PersistentClassGeneratorKeywords.OperatorEnd:
					return ";";
				case PersistentClassGeneratorKeywords.Comment:
					return "//";
				case PersistentClassGeneratorKeywords.MultilineCommentOpen:
					return "/*";
				case PersistentClassGeneratorKeywords.MultilineCommentClose:
					return "*/";
				case PersistentClassGeneratorKeywords.IgnoreEscapeSimbol:
					return "@";
				default:
					return string.Empty;
			}
		}			
	}
	internal class VBPersistentClassGenerator : PersistentClassGenerator {
		string[] reservedWordsArray = new string[]{ "addhandler", "addressof", "alias",
				"and", "andalso", "as", "boolean", "byref", "byte", "byval", "call", "case", "catch",
				"cbool", "cbyte", "cchar", "cdate", "cdec", "cdbl", "char", "cint", "class", "clng", 
				"cobj", "const", "continue", "csbyte", "cshort", "csng", "cstr", "ctype", "cuint",
				"culng", "cushort", "date", "decimal", "declare", "default", "delegate", "dim", "directcast",
				"do", "double", "each", "else", "elseif", "end", "endif", "enum", "erase", "error", "event",
				"exit", "false", "finally", "for", "friend", "function", "get", "gettype", "global", "gosub",
				"goto", "handles", "if", "implements", "imports", "in", "inherits", "integer", "interface",
				"is", "isnot", "let", "lib", "like", "long", "loop", "me", "mod", "module", "mustinherit",
				"mustoverride", "mybase", "myclass", "namespace", "narrowing", "new", "next", "not",
				"nothing", "notinheritable", "notoverridable", "object", "of", "on", "operator",
				"option", "optional", "or", "orelse", "overloads", "overridable", "overrides", "paramarray",
				"partial", "private", "property", "protected", "public", "raiseevent", "readonly", "redim",
				"rem", "removehandler", "resume", "return", "sbyte", "select", "set", "shadows", "shared",
				"short", "single", "static", "step", "stop", "string", "structure", "sub", "synclock",
				"then", "throw", "to", "true", "try", "trycast", "typeof", "variant", "wend", "uinteger",
				"ulong", "ushort", "using", "when", "while", "widening", "with", "withevents", "writeonly",
				"xor", "ansi", "assembly", "auto", "binary", "compare", "custom", "explicit", "isfalse",
				"istrue", "mid", "off", "preserve", "strict", "text", "unicode", "until" };
		protected override string[] ReservedWordsArray { get { return reservedWordsArray; } }
		protected override bool SupportMultilineComment { get { return false; } }
		protected override void WriteUsing(TextWriter w) {
			w.WriteLine("Imports System");
			w.WriteLine("Imports DevExpress.Xpo");
			w.WriteLine("Imports DevExpress.Data.Filtering");
		}
		protected override void StartNamespace(TextWriter w, string name) {
			w.WriteLine("Namespace " + name);
		}
		protected override void WriteConnectionComment(TextWriter w, string connectionString) {
			throw new NotSupportedException();
		}
		protected override void CloseNamespace(TextWriter w) {
			w.WriteLine("End Namespace");
		}
		protected override void TurnoffOptimisticLocking(TextWriter w) {
			w.WriteLine("\t<OptimisticLocking(false)> _");
		}
		protected override void WriteClassAssociationTag(TextWriter w, string name) {
			w.WriteLine("\t\t<Association(\"" + name + "\")> _");
		}
		protected override void WriteClassPersistentTag(TextWriter w, string name) {
			w.WriteLine("\t<Persistent(\"" + name + "\")> _");
		}
		protected override void WriteClassNonPersistentTag(TextWriter w) {
			w.WriteLine("\t<NonPersistent()> _");
		}
		protected override void DeclareClass(TextWriter w, string className, string baseClassName) {
			w.WriteLine("\tPublic Class " + className);
			w.WriteLine("\t\tInherits " + baseClassName);
		}
		protected override void CloseClass(TextWriter w) {
			w.WriteLine("\tEnd Class");
		}
		int inModule = 0;
		protected override void DeclareStaticClass(TextWriter w, string className) {
			inModule++;
			w.WriteLine("\tPublic Module " + className);
		}
		protected override void CloseStaticClass(TextWriter w) {
			inModule--;
			w.WriteLine("\tEnd Module");
		}
		protected override void WriteClassConstructors(TextWriter w, string className) {
			w.WriteLine("\t\tPublic Sub New(ByVal session As Session)");
			w.WriteLine("\t\t\tMyBase.New(session)");
			w.WriteLine("\t\tEnd Sub");
			w.WriteLine("\t\tPublic Sub New()");
			w.WriteLine("\t\t\tMyBase.New(Session.DefaultSession)");
			w.WriteLine("\t\tEnd Sub");
			w.WriteLine("\t\tPublic Overrides Sub AfterConstruction()");
			w.WriteLine("\t\t\tMyBase.AfterConstruction()");
			w.WriteLine("\t\tEnd Sub");
		}
		protected override void DeclareConnectionHelper(TextWriter w, string connectionString) {
			w.WriteLine("\tPublic Class ConnectionHelper");
			w.WriteLine(string.Concat("\t\tConst ConnectionString = \"", connectionString.Replace("\"","\"\"") ,"\""));
			w.WriteLine("\t\tPublic Shared Sub Connect(ByVal autoCreationOption As DB.AutoCreateOption)");
			w.WriteLine("\t\t\tXpoDefault.DataLayer = XpoDefault.GetDataLayer(ConnectionString, autoCreationOption)");
			w.WriteLine("\t\t\tXpoDefault.Session = Nothing");
			w.WriteLine("\t\tEnd Sub");
			w.WriteLine("\t\tPublic Shared Function GetConnectionProvider(ByVal autoCreationOption As DB.AutoCreateOption) As DB.IDataStore");
			w.WriteLine("\t\t\tReturn XpoDefault.GetConnectionProvider(ConnectionString, autoCreationOption)");
			w.WriteLine("\t\tEnd Function");
			w.WriteLine("\t\tPublic Shared Function GetConnectionProvider(ByVal autoCreationOption As DB.AutoCreateOption, ByRef objectsToDisposeOnDisconnect() As IDisposable) As DB.IDataStore");
			w.WriteLine("\t\t\tReturn XpoDefault.GetConnectionProvider(ConnectionString, autoCreationOption, objectsToDisposeOnDisconnect)");
			w.WriteLine("\t\tEnd Function");
			w.WriteLine("\t\tPublic Shared Function GetDataLayer(ByVal autoCreationOption As DB.AutoCreateOption) As IDataLayer");
			w.WriteLine("\t\t\tReturn XpoDefault.GetDataLayer(ConnectionString, autoCreationOption)");
			w.WriteLine("\t\tEnd Function");
			w.WriteLine("\tEnd Class");
		}
		protected override string GetLanguageType(DBColumnType columnType) {
			switch(columnType) {
				case DBColumnType.Boolean:
					return "Boolean";
				case DBColumnType.Byte:
					return "Byte";
				case DBColumnType.SByte:
					return "SByte";
				case DBColumnType.Char:
					return "Char";
				case DBColumnType.Decimal:
					return "Decimal";
				case DBColumnType.Single:
					return "Single";
				case DBColumnType.Double:
					return "Double";
				case DBColumnType.Int32:
					return "Integer";
				case DBColumnType.UInt32:
					return "UInteger";
				case DBColumnType.Int16:
					return "Short";
				case DBColumnType.UInt16:
					return "UShort";
				case DBColumnType.Int64:
					return "Long";
				case DBColumnType.UInt64:
					return "ULong";
				case DBColumnType.String:
					return "String";
				case DBColumnType.DateTime:
					return "DateTime";
				case DBColumnType.Guid:
					return "Guid";
				case DBColumnType.TimeSpan:
					return "TimeSpan";
				case DBColumnType.ByteArray:
					return "Byte()";
				default:
					return "";
			}
		}
		protected override bool IsCaseSensitive {
			get { return false; }
		}
		protected override string GetArgument(string name, string type) {
			return "ByVal " + name + " As " + type;
		}
		protected override void DeclareStaticFunction(TextWriter w, string functionName, string arguments, string resultType, string[] body) {
			string functionKeyword = string.IsNullOrEmpty(resultType) ? "Sub" : "Function";
			string functionBegin = inModule > 0 ? "\t\tPublic " + functionKeyword + " " : "\t\tPublic Shared " + functionKeyword + " ";
			string functionType = string.IsNullOrEmpty(resultType) ? string.Empty : " As " + resultType;
			w.WriteLine(functionBegin + functionName + "(" + arguments + ")" + functionType);
			for(int i = 0; i < body.Length; i++) {
				w.WriteLine("\t\t\t" + body[i]);
			}
			w.WriteLine("\t\tEnd " + functionKeyword);
		}
		protected override void DeclarePrivatStaticArray(TextWriter w, string arrayName, string arrayType, string[] elements) {
			w.WriteLine("\t\tPrivate " + arrayName + "() As " + arrayType + " = {" + string.Join(", ", elements) + "}");
		}
		protected override void DeclareField(TextWriter w, string type, string columnName) {
			if(type == "Byte()") w.WriteLine("\t\tDim " + columnName + "() As Byte");
			else w.WriteLine("\t\tDim " + columnName + " As " + type);
		}
		protected override void WriteKey(TextWriter w) {
			w.WriteLine("\t\t<Key()> _");
		}
		protected override void WriteIdentityKey(TextWriter w) {
			w.WriteLine("\t\t<Key(true)> _");
		}
		protected override void WritePropertySize(TextWriter w, int size) {
			w.WriteLine("\t\t<Size(" + ((size <= 0 || size > 10000) ? "SizeAttribute.Unlimited" : size.ToString(CultureInfo.InvariantCulture)) + ")> _");
		}
		protected override void WritePersistentProperty(TextWriter w, string columnName) {
			w.WriteLine("\t\t<Persistent(\"" + columnName + "\")> _");
		}
		protected override bool IsPropertyNameReserved(string propertyName) {
			return !ReservedWords.ContainsKey(propertyName.ToLower());
		}
		protected override void DeclareProperty(TextWriter w, string type, string propertyName,
															string fieldName, string columnName) {
			w.WriteLine("\t\tPublic Property " + propertyName + "() As " + type);
			w.WriteLine("\t\t\tGet");
			w.WriteLine("\t\t\t\tReturn " + fieldName);
			w.WriteLine("\t\t\tEnd Get");
			if(type == "Byte()") w.WriteLine("\t\t\tSet(ByVal value() As Byte)");
			else w.WriteLine("\t\t\tSet(ByVal value As " + type + ")");
			w.WriteLine("\t\t\t\tSetPropertyValue(Of " + type + ")(\"" + propertyName + "\", " + fieldName + ", value)");
			w.WriteLine("\t\t\tEnd Set");
			w.WriteLine("\t\tEnd Property");
		}
		protected override void DeclareCollectionProperty(TextWriter w, string type, string propertyName) {
			w.WriteLine("\t\tPublic ReadOnly Property " + propertyName + "() As XPCollection(Of " + type + ")");
			w.WriteLine("\t\t\tGet");
			w.WriteLine("\t\t\t\tReturn GetCollection(Of " + type + ")(\"" + propertyName + "\")");
			w.WriteLine("\t\t\tEnd Get");
			w.WriteLine("\t\tEnd Property");
		}
		protected override string GetVariableDeclaration(string className, string variableName) {
			return string.Format("Dim {1} As {0}", className, variableName);
		}
		protected override string GetGeneric(string className) {
			return "(Of " + className + ")";
		}
		protected override string GetLanguageKeyword(PersistentClassGeneratorKeywords keyword) {
			switch(keyword) {
				case PersistentClassGeneratorKeywords.Null:
					return "Nothing";
				case PersistentClassGeneratorKeywords.Return:
					return "Return";
				case PersistentClassGeneratorKeywords.New:
					return "New";
				case PersistentClassGeneratorKeywords.TypeOf:
					return "GetType";
				case PersistentClassGeneratorKeywords.Comment:
					return "'";
				case PersistentClassGeneratorKeywords.MultilineCommentOpen:
				case PersistentClassGeneratorKeywords.MultilineCommentClose:
					throw new InvalidOperationException();
				case PersistentClassGeneratorKeywords.OperatorEnd:
				case PersistentClassGeneratorKeywords.IgnoreEscapeSimbol:
				default:
					return string.Empty;
			}
		}										
	}
}
