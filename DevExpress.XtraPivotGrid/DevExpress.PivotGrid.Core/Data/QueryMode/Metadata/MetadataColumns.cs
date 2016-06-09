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
namespace DevExpress.PivotGrid.QueryMode {
	public abstract class QueryMetadataColumns : NamedColumnList<MetadataColumnBase> {
		IQueryMetadata owner;
		public IQueryMetadata Owner { get { return owner; } }
		protected QueryMetadataColumns(IQueryMetadata owner) {
			this.owner = owner;
		}
		public void SaveToSteam(TypedBinaryWriter writer) {
			writer.Write(Count);
			for(int i = 0; i < Count; i++) {
#if DEBUGTEST
				writer.Write((int)21342355);
#endif
				SaveColumnToStream(writer, this[i]);
			}
		}
		protected virtual void SaveColumnToStream(TypedBinaryWriter writer, IQueryMetadataColumn column) {
			column.SaveToStream(Owner, writer);
		}
		public void RestoreFromStream(TypedBinaryReader reader) {
			int columnsCount = reader.ReadInt32();
			for(int i = 0; i < columnsCount; i++) {
#if DEBUGTEST
				int sign = reader.ReadInt32();
				if(sign != 21342355)
					throw new Exception("corrupted");
#endif
				MetadataColumnBase column = CreateColumn(reader);
				RestoreColumnFromStream(reader, column);
			}
		}
		protected virtual void RestoreColumnFromStream(TypedBinaryReader reader, MetadataColumnBase column) {
			column.RestoreFromStream(Owner, reader);
			Add(column);
		}
		protected internal virtual void Add(MetadataColumnBase column) {
			Add(column.UniqueName, column);
			AddCore(column);
		}
		protected void AddCore(MetadataColumnBase column) {
			ColumnsList.Add(column);
			column.Owner = owner;
		}
		protected abstract MetadataColumnBase CreateColumn(TypedBinaryReader reader);
		public virtual string GetFieldCaption(string fieldName) {
			return fieldName;
		}
		public List<string> GetFieldList() {
			List<string> result = new List<string>(Count);
			foreach(KeyValuePair<string, MetadataColumnBase> column in this)
				result.Add(column.Key);
			return result;
		}
		internal MetadataColumnBase FindColumn(string name, bool caseSensitive) {
			if(caseSensitive)
				return this[name];
			return this.ColumnsList.Find(c => string.Equals(c.Name, name, StringComparison.OrdinalIgnoreCase));
		}
	}
}
