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
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPMetadataColumns : QueryMetadataColumns {
		List<OLAPMetadataColumn> nonaggregatables;
		readonly Dictionary<string, object> drillDownColumns = new Dictionary<string, object>();
		public new OLAPMetadataColumn this[string name] { get { return (OLAPMetadataColumn)base[name]; } }
		public new OLAPMetadataColumn this[int index] { get { return (OLAPMetadataColumn)base[index]; } }
		public List<OLAPMetadataColumn> NonAggregatables {
			get {
				if(nonaggregatables == null) {
					nonaggregatables = new List<OLAPMetadataColumn>();
					for(int i = 0; i < Count; i++) {
						if(this[i].IsAggregatable || this[i].HasParent || this[i].IsMeasure)
							continue;
						nonaggregatables.Add(this[i]);
					}
				}
				return nonaggregatables;
			}
		}
		public OLAPMetadataColumns(IQueryMetadata owner) : base(owner) { }
		protected internal override void Add(MetadataColumnBase column) {
			OLAPMetadataColumn olapColumn = (OLAPMetadataColumn)column;
			Add(olapColumn.UniqueName, column);
			object duplicateColumn;
			if(!drillDownColumns.TryGetValue(olapColumn.DrillDownColumn, out duplicateColumn))
				drillDownColumns.Add(olapColumn.DrillDownColumn, column);
			else {
				List<OLAPMetadataColumn> list = duplicateColumn as List<OLAPMetadataColumn>;
				if(list == null) {
					list = new List<OLAPMetadataColumn>();
					list.Add(duplicateColumn as OLAPMetadataColumn);
					drillDownColumns[olapColumn.DrillDownColumn] = list;
				}
				list.Add(olapColumn);
			}
			AddCore(column);
		}
		public object GetByDrillDownColumn(string drilldownColumn) {
			object res;
			return !drillDownColumns.TryGetValue(drilldownColumn, out res) ? null : res;
		}
		protected override MetadataColumnBase CreateColumn(TypedBinaryReader reader) {
			return OLAPMetadataColumn.CreateFromTypeCode(reader.ReadByte());
		}
		public override void Clear() {
			base.Clear();
			nonaggregatables = null;
			drillDownColumns.Clear();
		}
		public override string GetFieldCaption(string fieldName) {
			OLAPMetadataColumn column = this[fieldName];
			if(column == null)
				return null;
			return !string.IsNullOrEmpty(column.Caption) ? column.Caption : column.Name;
		}
		protected override void SaveColumnToStream(TypedBinaryWriter writer, IQueryMetadataColumn column) {
			OLAPMetadataColumn oColumn = (OLAPMetadataColumn)column;
			writer.Write(oColumn.TypeCode);
			base.SaveColumnToStream(writer, column);
			if(oColumn.IsMeasure)
				return;
			oColumn.SaveMembersToStream(Owner, writer);
		}
		protected override void RestoreColumnFromStream(TypedBinaryReader reader, MetadataColumnBase column) {
			base.RestoreColumnFromStream(reader, column);
			if(column.IsMeasure)
				return;
			((OLAPMetadataColumn)column).RestoreMembersFromStream(Owner, reader);
		}
		protected internal OLAPMember GetMemberByUniqueLevelValue(string fieldName, object value) {
			OLAPMetadataColumn column = this[fieldName];
			string un = value as string;
			if(column == null || column.IsMeasure || string.IsNullOrEmpty(un))
				return null;
			return column.GetMemberByUniqueLevelValue(un);
		}
	}
}
