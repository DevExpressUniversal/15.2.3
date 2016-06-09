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

namespace DevExpress.Xpo.DB {
	using System;
	using System.Collections;
	using System.Xml.Serialization;
	using System.ComponentModel;
	using DevExpress.Xpo.DB.Exceptions;
	using System.Collections.Generic;
	using System.Collections.Specialized;
	using DevExpress.Compatibility.System.Collections.Specialized;
	using DevExpress.Utils;
#if !SL
	using DevExpress.Compatibility.System;
	using DevExpress.Compatibility.System.Xml.Serialization;
#endif
	[Serializable]
	public class DBColumnCollection : List<DBColumn> {
	}
	[Serializable]
	public class DBIndexCollection : List<DBIndex> {
	}
	[Serializable]
	public class DBForeignKeyCollection : List<DBForeignKey> {
	}
	[Serializable]
	public class DBProjection : DBTable {		
		public SelectStatement Projection;
		public DBProjection() { }
		public DBProjection(SelectStatement projection) {
			Projection = projection;
		}		 
		public override bool Equals(object obj) {
			DBProjection another = obj as DBProjection;
			if(another == null)
				return false;
			return object.Equals(Projection, another.Projection);
		}
		public override int GetHashCode() {
			return Projection == null ? 0x6428231 : Projection.GetHashCode();
		}
		public override string ToString() {
			return Projection == null ? "null" : Projection.ToString();
		}
	}
	[Serializable]
	[XmlInclude(typeof(DBProjection))]
	public class DBTable {
		DBColumnCollection columns = new DBColumnCollection();
		DBIndexCollection indexes = new DBIndexCollection();
		DBForeignKeyCollection foreignKeys = new DBForeignKeyCollection();
		[XmlAttribute]
		public string Name;
		public DBTable() { }
		public DBTable(string name) {
			this.Name = name;
		}
		public DBPrimaryKey PrimaryKey;
		[XmlAttribute]
		[DefaultValue(false)]
		public bool IsView;
		[XmlArrayItem(typeof(DBColumn))]
		public List<DBColumn> Columns { get { return columns; } }
		[XmlArrayItem(typeof(DBIndex))]
		public List<DBIndex> Indexes { get { return indexes; } }
		[XmlArrayItem(typeof(DBForeignKey))]
		public List<DBForeignKey> ForeignKeys { get { return foreignKeys; } }
		public DBColumn GetColumn(string columnName) {
			int count = Columns.Count;
			for(int i = 0; i < count; i++) {
				DBColumn column = (DBColumn)Columns[i];
				if(column.Name == columnName)
					return column;
			}
			return null;
		}
		public bool IsForeignKeyIncluded(DBForeignKey fk) {
			foreach(DBForeignKey key in ForeignKeys)
				if(IsGadgetsEqual(key, fk))
					return true;
			return false;
		}
		bool IsGadgetsEqual(DBTableMultiColumnGadget existingKey, DBTableMultiColumnGadget fk) {
			if(existingKey.Name != fk.Name || existingKey.Columns.Count != fk.Columns.Count)
				return false;
			for(int i = 0; i < existingKey.Columns.Count; ++i) {
				if(existingKey.Columns[i] != fk.Columns[i])
					return false;
			}
			return true;
		}
		public bool IsIndexIncluded(DBIndex index) {
			foreach(DBIndex key in Indexes)
				if(IsGadgetsEqual(key, index))
					return true;
			return false;
		}
		public void AddColumn(DBColumn column) {
			columns.Add(column);
		}
		public void AddIndex(DBIndex index) {
			indexes.Add(index);
		}
		public void AddForeignKey(DBForeignKey fk) {
			foreignKeys.Add(fk);
		}
		public override bool Equals(object obj) {
			DBTable another = obj as DBTable;
			if(another == null)
				return false;
			return this.Name == another.Name;
		}
		public override int GetHashCode() {
			if(this.Name == null)
				return 0;
			return this.Name.GetHashCode();
		}
		public override string ToString() {
			return Name;
		}
	}
	public enum DBColumnType {
		Unknown,
		Boolean,
		Byte,
		SByte,
		Char,
		Decimal,
		Double,
		Single,
		Int32,
		UInt32,
		Int16,
		UInt16,
		Int64,
		UInt64,
		String,
		DateTime,
		Guid,
		TimeSpan,
		ByteArray
	}
	[Serializable]
	public class DBColumn {
		[XmlAttribute]
		public DBColumnType ColumnType;
		public static bool IsStorableType(DBColumnType type) {
			return type != DBColumnType.Unknown;
		}
		public static bool IsStorableType(Type type) {
			return IsStorableType(GetColumnType(type, true));
		}
		public static DBColumnType GetColumnType(Type type) {
			return GetColumnType(type, false);
		}
		public static DBColumnType GetColumnType(Type type, bool supressExceptionOnUnknown) {
			TypeCode typeCode = DXTypeExtensions.GetTypeCode(type);
			switch(typeCode) {
				case TypeCode.Boolean:
					return DBColumnType.Boolean;
				case TypeCode.Byte:
					return DBColumnType.Byte;
				case TypeCode.SByte:
					return DBColumnType.SByte;
				case TypeCode.Char:
					return DBColumnType.Char;
				case TypeCode.Decimal:
					return DBColumnType.Decimal;
				case TypeCode.Double:
					return DBColumnType.Double;
				case TypeCode.Single:
					return DBColumnType.Single;
				case TypeCode.Int32:
					return DBColumnType.Int32;
				case TypeCode.UInt32:
					return DBColumnType.UInt32;
				case TypeCode.Int16:
					return DBColumnType.Int16;
				case TypeCode.UInt16:
					return DBColumnType.UInt16;
				case TypeCode.Int64:
					return DBColumnType.Int64;
				case TypeCode.UInt64:
					return DBColumnType.UInt64;
				case TypeCode.String:
					return DBColumnType.String;
				case TypeCode.DateTime:
					return DBColumnType.DateTime;
				default:
					if(type == typeof(Guid))
						return DBColumnType.Guid;
					else if(type == typeof(TimeSpan))
						return DBColumnType.TimeSpan;
					else if(type == typeof(byte[]))
						return DBColumnType.ByteArray;
					else if(supressExceptionOnUnknown)
						return DBColumnType.Unknown;
					else
						throw new PropertyTypeMappingMissingException(type);
			}
		}
		public static Type GetType(DBColumnType type) {
			switch(type) {
				case DBColumnType.Boolean:
					return typeof(bool);
				case DBColumnType.Byte:
					return typeof(byte);
				case DBColumnType.SByte:
					return typeof(SByte);
				case DBColumnType.Char:
					return typeof(Char);
				case DBColumnType.Decimal:
					return typeof(Decimal);
				case DBColumnType.Double:
					return typeof(Double);
				case DBColumnType.Single:
					return typeof(Single);
				case DBColumnType.Int32:
					return typeof(Int32);
				case DBColumnType.UInt32:
					return typeof(UInt32);
				case DBColumnType.Int16:
					return typeof(Int16);
				case DBColumnType.UInt16:
					return typeof(UInt16);
				case DBColumnType.Int64:
					return typeof(Int64);
				case DBColumnType.UInt64:
					return typeof(UInt64);
				case DBColumnType.String:
					return typeof(String);
				case DBColumnType.DateTime:
					return typeof(DateTime);
				case DBColumnType.Guid:
					return typeof(Guid);
				case DBColumnType.TimeSpan:
					return typeof(TimeSpan);
				case DBColumnType.ByteArray:
					return typeof(byte[]);
				default:
					return typeof(object);
			}
		}
		[XmlAttribute]
		public string Name;
		[XmlAttribute]
		[DefaultValue(0)]
		public int Size;
		[XmlAttribute]
		[DefaultValue(false)]
		public bool IsKey;
		[XmlAttribute]
		[DefaultValue(false)]
		public bool IsIdentity;
		[XmlAttribute]
		[DefaultValue("")]
		public string DBTypeName;
		public DBColumn() { }
		public DBColumn(string name, bool isKey, string dBTypeName, int size, DBColumnType type) {
			this.IsKey = isKey;
			this.Name = name;
			this.DBTypeName = dBTypeName;
			this.Size = size;
			this.ColumnType = type;
		}
	}
	[Serializable]
	public abstract class DBTableMultiColumnGadget {
		[XmlAttribute]
		public string Name;
		public StringCollection Columns = new StringCollection();
		protected DBTableMultiColumnGadget() { }
		protected DBTableMultiColumnGadget(ICollection columns) {
			if(columns.Count < 1)
				throw new ArgumentException(DbRes.GetString(DbRes.ConnectionProvider_AtLeastOneColumnExpected), "columns");
			if(columns is StringCollection) {
				foreach(string col in columns)
					this.Columns.Add(col);
			} else {
				foreach(DBColumn col in columns)
					this.Columns.Add(col.Name);
			}
		}
	}
	[Serializable]
	public class DBIndex : DBTableMultiColumnGadget {
		[XmlAttribute]
		[DefaultValue(false)]
		public bool IsUnique;
		public DBIndex() { }
		public DBIndex(string name, ICollection columns, bool isUnique)
			: base(columns) {
			this.Name = name;
			this.IsUnique = isUnique;
		}
		public DBIndex(ICollection columns, bool isUnique) : this(null, columns, isUnique) { }
	}
	[Serializable]
	public class DBPrimaryKey : DBIndex {
		public DBPrimaryKey() { }
		public DBPrimaryKey(string name, ICollection columns) : base(name, columns, true) { }
		public DBPrimaryKey(ICollection columns) : base(columns, true) { }
	}
	[Serializable]
	public class DBForeignKey : DBTableMultiColumnGadget {
		[XmlAttribute]
		public string PrimaryKeyTable;
		public StringCollection PrimaryKeyTableKeyColumns = new StringCollection();
		public DBForeignKey() { }
		public DBForeignKey(ICollection columns, string primaryKeyTable, StringCollection primaryKeyTableKeyColumns)
			: base(columns) {
			this.PrimaryKeyTable = primaryKeyTable;
			this.PrimaryKeyTableKeyColumns = primaryKeyTableKeyColumns;
		}
	}
	[Serializable]
	public class DBNameTypePair {
		[XmlAttribute]
		public string Name;
		[XmlAttribute]
		public DBColumnType Type;
		public DBNameTypePair() { }
		public DBNameTypePair(string name, DBColumnType type) {
			this.Name = name;
			this.Type = type;
		}
		public override string ToString() {
			return string.Format("{0} {1}", Name, Type);
		}
	}
	public enum DBStoredProcedureArgumentDirection { In, Out, InOut }
	[Serializable]
	public class DBStoredProcedureArgument : DBNameTypePair {
		[XmlAttribute]
		public DBStoredProcedureArgumentDirection Direction;
		public DBStoredProcedureArgument() : base() { }
		public DBStoredProcedureArgument(string name, DBColumnType type) : base(name, type) { }
		public DBStoredProcedureArgument(string name, DBColumnType type, DBStoredProcedureArgumentDirection direction)
			: this(name, type) {
			this.Direction = direction;
		}
		public override string ToString() {
			return string.Format("[{0}] {1} {2}", Direction.ToString().ToUpper(), Name, Type);
		}
	}
	[Serializable]
	public class DBStoredProcedureResultSet {
		List<DBNameTypePair> columns = new List<DBNameTypePair>();
		[XmlArrayItem(typeof(DBNameTypePair))]
		public List<DBNameTypePair> Columns { get { return columns; } }
		public DBStoredProcedureResultSet() { }
		public DBStoredProcedureResultSet(ICollection<DBNameTypePair> columns) {
			foreach(DBNameTypePair nameTypePair in columns) {
				this.columns.Add(nameTypePair);
			}
		}
		public override string ToString() {
			if(columns.Count == 1) { return "1 column"; }
			return string.Format("{0} columns", columns.Count);
		}
	}
	[Serializable]
	public class DBStoredProcedure {
		[XmlAttribute]
		public string Name;
		List<DBStoredProcedureArgument> arguments = new List<DBStoredProcedureArgument>();
		List<DBStoredProcedureResultSet> resultSets = new List<DBStoredProcedureResultSet>();
		[XmlArrayItem(typeof(DBStoredProcedureArgument))]
		public List<DBStoredProcedureArgument> Arguments { get { return arguments; } }
		[XmlArrayItem(typeof(DBStoredProcedureResultSet))]
		public List<DBStoredProcedureResultSet> ResultSets { get { return resultSets; } }
		public override string ToString() {
			return Name;
		}
	}
}
