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
using DevExpress.Data.IO;
using DevExpress.Data.PivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class MetadataColumnBase : IQueryMetadataColumn {
		protected static PivotCellValue ErrorCell = new FormattedPivotCellValue(PivotSummaryValue.ErrorValue, PivotGridLocalizer.GetString(PivotGridStringId.ValueError));
		public const string TotalMemberString = " XtraPivotGrid Total]";
		static MetadataColumnBase() {
		}
		public static T GetOriginalColumn<T>(QueryMetadataColumns columns, T column) where T : MetadataColumnBase {
			IQueryAliasColumn alias = column as IQueryAliasColumn;
			return alias == null ? column : (columns[alias.OriginalColumn] as T);
		}
		public static T GetOriginalColumn<T>(QueryColumns<T> columns, T column) where T : QueryColumn {
			IQueryAliasColumn alias = column.Metadata as IQueryAliasColumn;
			return alias == null ? column : (columns[alias.OriginalColumn] as T);
		}
		public static bool? UndefinedBoolFromByte(byte cbyte) {
			if(cbyte == 0)
				return null;
			return cbyte == 1;
		}		
		public static byte UndefinedBoolToByte(bool? ubool) {
			if(ubool == null)
				return 0;
			return ubool == true ? (byte)1 : (byte)2;
		}
		Type dataType;
		IQueryMetadata owner;
		bool? hasNullValues;
		MetadataColumnBase parentColumn, childColumn;
		public virtual Type SafeDataType { get { return dataType; } }
		public Type DataType { get { return dataType; } }
		public abstract bool IsMeasure { get; }
		public IQueryMetadata Owner {
			get { return owner; }
			internal set { owner = value; }
		}
		public abstract string Name { get; }
		public abstract string UniqueName { get; }
		public virtual string DisplayFolder {
			get { return string.Empty; }
		}
		public virtual string Caption {
			get { return null; }
		}
		public bool? HasNullValues { 
			get { return hasNullValues; } 
			set { hasNullValues = value; } 
		}
		IQueryMetadataColumn IQueryMetadataColumn.ParentColumn { get { return ParentColumn; } }
		IQueryMetadataColumn IQueryMetadataColumn.ChildColumn { get { return ChildColumn; } }
		public MetadataColumnBase ParentColumn {
			get { return parentColumn; }
			protected set {
				if(this.parentColumn != null)
					throw new Exception("parentColumn already set");
				this.parentColumn = value;
				if(value != null)
					value.childColumn = this;
			}
		}
		public MetadataColumnBase ChildColumn { get { return childColumn; } }
		public bool HasParent { get { return ParentColumn != null; } }
		protected internal MetadataColumnBase() {
		}
		protected MetadataColumnBase(Type dataType, MetadataColumnBase parentColumn) : this() {
			this.dataType = dataType ?? typeof(object);
			ParentColumn = parentColumn;
		}
		public List<IQueryMetadataColumn> GetColumnHierarchy() {
			List<IQueryMetadataColumn> res = new List<IQueryMetadataColumn>();
			IQueryMetadataColumn column = GetRoot();
			while(column != null) {
				res.Add(column);
				column = column.ChildColumn;
			}
			return res;
		}
		IQueryMetadataColumn GetRoot() {
			IQueryMetadataColumn res = this;
			while(res.ParentColumn != null)
				res = res.ParentColumn;
			return res;
		}
		public bool IsParent(QueryColumn column) {
			return IsParent(column.Metadata);
		}
		public bool IsParent(IQueryMetadataColumn child) {
			IQueryMetadataColumn column = child;
			while(column.ParentColumn != null) {
				if(column.ParentColumn == this)
					return true;
				column = column.ParentColumn;
			}
			return false;
		}
		public bool IsChildOrParent(MetadataColumnBase column) {
			return IsParent(column) || column.IsParent(this);
		}
		protected virtual void SaveToStream(IQueryMetadata owner, TypedBinaryWriter writer) {
			WriteDataTypeAndColumnName(writer);
#if DEBUGTEST
			writer.Write((int)54321);
#endif
		}
		protected virtual void WriteDataTypeAndColumnName(TypedBinaryWriter writer) {
			writer.WriteType(DataType);
			writer.Write(ParentColumn != null ? ParentColumn.Name : string.Empty);
		}
		public virtual void RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader) {
			dataType = ReadDataType(reader) ?? typeof(object);
			string parentUniqueName = reader.ReadString();
			ParentColumn = string.IsNullOrEmpty(parentUniqueName) ? null : metadata.Columns[parentUniqueName];
#if DEBUGTEST
			int sign = reader.ReadInt32();
			if(sign != 54321)
				throw new Exception("corrupted");
#endif
		}
		protected virtual Type ReadDataType(TypedBinaryReader reader) {
			return reader.ReadType();
		}
		#region IQueryMetadataColumn
		string IQueryMetadataColumn.Name {
			get { return Name; }
		}
		string IQueryMetadataColumn.UniqueName {
			get { return UniqueName; }
		}
		Type IQueryMetadataColumn.DataType {
			get { return DataType; }
		}
		void IQueryMetadataColumn.SaveToStream(IQueryMetadata metadata, TypedBinaryWriter writer) {
			SaveToStream(metadata, writer);
		}
		void IQueryMetadataColumn.RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader) {
			RestoreFromStream(metadata, reader);
		}
		QueryMember IQueryMetadataColumn.GetAllMember() {
			return GetAllMember();
		}
		protected abstract QueryMember GetAllMember();
		#endregion
	}
}
