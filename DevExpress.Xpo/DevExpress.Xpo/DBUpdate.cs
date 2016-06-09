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
using System.Resources;
namespace DevExpress.Xpo.DB {
	using System.Collections;
	using System.Collections.Specialized;
	using System.Data;
	using System.Globalization;
	using System.Reflection;
	using DevExpress.Xpo;
	using DevExpress.Xpo.Metadata.Helpers;
	using DevExpress.Xpo.Exceptions;
	using DevExpress.Xpo.Metadata;
	using DevExpress.Xpo.Helpers;
	using DevExpress.Xpo.DB;
	using System.Collections.Generic;
	public abstract class DBTableHelper {
		DBTableHelper() { }
		static DBColumn AddColumn(DBTable table, XPMemberInfo mi, string name, bool isKey) {
			DBColumn column = table.GetColumn(name);
			if(column == null) {
				if(mi.ReferenceType != null)
					mi = mi.ReferenceType.KeyProperty;
				column = CreateColumn(table, mi, name);
				column.IsKey = isKey;
				table.AddColumn(column);
			} else
				if(isKey)
					column.IsKey = isKey;
			return column;
		}
		public static void ProcessClassInfo(DBTable table, XPClassInfo classInfo) {
			Dictionary<XPMemberInfo, List<DBColumn>> memberColumns = new Dictionary<XPMemberInfo,List<DBColumn>>();
			ProcessMembers(table, classInfo, memberColumns);
			ProcessIndexes(table, classInfo, memberColumns);
		}
		static void ProcessMembers(DBTable table, XPClassInfo classInfo, Dictionary<XPMemberInfo, List<DBColumn>> memberColumns) {
			foreach(XPMemberInfo memberInfo in classInfo.PersistentProperties) {
				if(memberInfo.IsMappingClass(classInfo)) {
					List<DBColumn> columns = ProcessMemberColumns(table, classInfo, memberInfo);
					memberColumns.Add(memberInfo, columns);
					if(memberInfo.IsKey)
						ProcessPrimaryKey(table, columns, classInfo);
				}
			}
		}
		static void ProcessIndexes(DBTable table, XPClassInfo classInfo, Dictionary<XPMemberInfo, List<DBColumn>> memberColumns) {
			Dictionary<XPMemberInfo, bool> membersDict = new Dictionary<XPMemberInfo, bool>();
			foreach(XPMemberInfo memberInfo in classInfo.PersistentProperties) {
				if(memberInfo.IsMappingClass(classInfo)) {
					if(memberInfo.HasAttribute(typeof(IndexedAttribute))) {
						IndexedAttribute indexed = (IndexedAttribute)memberInfo.GetAttributeInfo(typeof(IndexedAttribute));
						membersDict.Clear();
						membersDict.Add(memberInfo, true);
						List<DBColumn> columns = new List<DBColumn>(memberColumns[memberInfo]);
						foreach(string memberName in indexed.AdditionalFields) {
							XPMemberInfo member = classInfo.GetMember(memberName);
							if(!memberColumns.ContainsKey(member))
								throw new PropertyMissingException(memberInfo.Owner.FullName, memberName);
							if(membersDict.ContainsKey(member))
								throw new InvalidOperationException(Res.GetString(Res.MetaData_PropertyIsDuplicatedInIndexDeclaration, memberName, classInfo.FullName, ".", memberInfo.Name, "property"));
							membersDict.Add(member, true);
							List<DBColumn> addColumns = memberColumns[member];
							columns.AddRange(addColumns);
						}
						DBIndex index = new DBIndex(indexed.Name, columns, indexed.Unique);
						if(!table.IsIndexIncluded(index))
							table.AddIndex(index);
					}
				}
			}
			if(classInfo.HasAttribute(typeof(IndicesAttribute))) {
				IndicesAttribute indices = (IndicesAttribute)classInfo.GetAttributeInfo(typeof(IndicesAttribute));
				foreach(var memberNames in indices.Indices) {
					if(memberColumns.Count == 0) continue;
					XPMemberInfo memberInfo = classInfo.GetMember(memberNames[0]);
					membersDict.Clear();
					membersDict.Add(memberInfo, true);
					if(memberInfo.IsMappingClass(classInfo)) {
						List<DBColumn> columns = new List<DBColumn>(memberColumns[memberInfo]);
						for(int i = 1; i < memberNames.Count; i++) {
							string memberName = memberNames[i];
							XPMemberInfo member = classInfo.GetMember(memberName);
							if(!memberColumns.ContainsKey(member))
								throw new PropertyMissingException(memberInfo.Owner.FullName, memberName);
							if(membersDict.ContainsKey(member))
								throw new InvalidOperationException(Res.GetString(Res.MetaData_PropertyIsDuplicatedInIndexDeclaration, memberName, classInfo.FullName, string.Empty, string.Empty, "class"));
							membersDict.Add(member, true);
							List<DBColumn> addColumns = memberColumns[member];
							columns.AddRange(addColumns);
						}
						DBIndex index = new DBIndex(null, columns, false);
						if(!table.IsIndexIncluded(index))
							table.AddIndex(index);
					}
				}
			}
		}
		static List<DBColumn> ProcessMemberColumns(DBTable table, XPClassInfo classInfo, XPMemberInfo memberInfo) {
			return ProcessMemberColumns(table, memberInfo, memberInfo.SubMembers.Count == 0 ? memberInfo.MappingField : string.Empty, memberInfo.IsKey, !(memberInfo.IsKey && classInfo.PersistentBaseClass != null && classInfo.PersistentBaseClass.TableName != table.Name));
		}
		static List<DBColumn> ProcessMemberColumns(DBTable table, XPMemberInfo processedMemberInfo, string mappingPath, bool isKey, bool processFk) {
			if(processedMemberInfo.ReferenceType != null) {
				XPMemberInfo keyProperty = processedMemberInfo.ReferenceType.KeyProperty;
				List<DBColumn> columns = ProcessMemberColumns(table, keyProperty, mappingPath, isKey, false);
				if(processFk)
					ProcessForeignKey(table, processedMemberInfo, columns);
				return columns;
			} else if(processedMemberInfo.SubMembers.Count == 0) {
				List<DBColumn> columns = new List<DBColumn>(1);
				columns.Add(AddColumn(table, processedMemberInfo, mappingPath, isKey));
				return columns;
			} else {
				List<DBColumn> columns = new List<DBColumn>();
				foreach(XPMemberInfo mi in processedMemberInfo.SubMembers) {
					if(mi.IsPersistent) {
						columns.AddRange(ProcessMemberColumns(table, mi, mappingPath + mi.MappingField, isKey, processFk));
					}
				}
				return columns;
			}
		}
		static void ProcessForeignKey(DBTable table, XPMemberInfo memberInfo, List<DBColumn> columns) {
			if(memberInfo.HasAttribute(typeof(NoForeignKeyAttribute)))
				return;
			StringCollection todo = new StringCollection();
			XPMemberInfo refKey = memberInfo.ReferenceType.KeyProperty;
			if(refKey.SubMembers.Count > 0) {
				foreach(XPMemberInfo mi in refKey.SubMembers) {
					if(mi.IsPersistent)
						todo.Add(mi.MappingField);
				}
			} else
				todo.Add(refKey.MappingField);
			DBForeignKey fk = new DBForeignKey(columns,
				memberInfo.ReferenceType.TableName, todo);
			if(!table.IsForeignKeyIncluded(fk))
				table.AddForeignKey(fk);
		}
		static void ProcessPrimaryKey(DBTable table, List<DBColumn> columns, XPClassInfo classInfo) {
			if(table.PrimaryKey == null)
				table.PrimaryKey = new DBPrimaryKey(columns);
			if(columns.Count == 1 && classInfo == classInfo.IdClass &&
				classInfo.KeyProperty.IsIdentity) {
				((DBColumn)columns[0]).IsIdentity = true;
			}
			if(classInfo.PersistentBaseClass != null && classInfo.TableMapType == MapInheritanceType.OwnTable) {
				StringCollection todo = new StringCollection();
				foreach(DBColumn col in columns)
					todo.Add(col.Name);
				table.AddForeignKey(new DBForeignKey(columns, classInfo.PersistentBaseClass.TableName, todo));
			}
		}
		public static DBColumn CreateColumn(DBTable table, XPMemberInfo memberInfo, string name) {
			XPMemberInfo workMemberInfo = memberInfo.ReferenceType != null ?
				memberInfo.ReferenceType.KeyProperty :
				memberInfo;
			return new DBColumn(name, memberInfo.IsKey, memberInfo.HasAttribute(typeof(DbTypeAttribute)) ?
				((DbTypeAttribute)memberInfo.GetAttributeInfo(typeof(DbTypeAttribute))).DbColumnTypeName : String.Empty,
				workMemberInfo.MappingFieldSize, DBColumn.GetColumnType(workMemberInfo.StorageType));
		}
	}
}
