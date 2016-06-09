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
using System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Sql {
	public sealed class MasterDetailInfo {
		public class EqualityComparer : IEqualityComparer<MasterDetailInfo> {
			public static bool Equals(MasterDetailInfo x, MasterDetailInfo y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				int colCount = x.keyColumns.Count;
				if(y.keyColumns.Count != colCount)
					return false;
				if(!string.Equals(x.master, y.master, StringComparison.Ordinal) || !string.Equals(x.detail, y.detail, StringComparison.Ordinal) || !string.Equals(x.customName, y.customName, StringComparison.Ordinal))
					return false;
				for(int i = 0; i < colCount; i++)
					if(!RelationColumnInfo.EqualityComparer.Equals(x.keyColumns[i], y.keyColumns[i]))
						return false;
				return true;
			}
			#region IEqualityComparer<MasterDetailInfo> Members
			bool IEqualityComparer<MasterDetailInfo>.Equals(MasterDetailInfo x, MasterDetailInfo y) { return Equals(x, y); }
			int IEqualityComparer<MasterDetailInfo>.GetHashCode(MasterDetailInfo obj) { return 0; }
			#endregion
		}
		const string STR_Null = "<null>";
		string master;
		string detail;
		string customName;
		readonly List<RelationColumnInfo> keyColumns;
		public MasterDetailInfo(string master, string detail) {
			this.master = master;
			this.detail = detail;
			this.keyColumns = new List<RelationColumnInfo>();
		}
		public MasterDetailInfo() : this(null, null) { }
		public MasterDetailInfo(string masterQueryName, string detailQueryName, IEnumerable<RelationColumnInfo> keyColumns)
			: this(masterQueryName, detailQueryName) {
			this.keyColumns.AddRange(keyColumns);
		}
		public MasterDetailInfo(string masterQueryName, string detailQueryName, RelationColumnInfo keyColumn)
			: this(masterQueryName, detailQueryName) {
			this.keyColumns.Add(keyColumn);
		}
		public MasterDetailInfo(string masterQueryName, string detailQueryName, string masterKeyColumn, string detailKeyColumn) : this(masterQueryName, detailQueryName, new RelationColumnInfo(masterKeyColumn, detailKeyColumn)) { }
		internal MasterDetailInfo(MasterDetailInfo other) : this(other.master, other.detail) {
			this.customName = other.customName;
			foreach(RelationColumnInfo column in other.keyColumns)
				this.keyColumns.Add(new RelationColumnInfo(column));
		}
#if !SL
	[DevExpressDataAccessLocalizedDescription("MasterDetailInfoMasterQueryName")]
#endif
		public string MasterQueryName {
			get { return master; }
			set { master = value; }
		}
#if !SL
	[DevExpressDataAccessLocalizedDescription("MasterDetailInfoDetailQueryName")]
#endif
		public string DetailQueryName { get { return detail; } set { detail = value; } }
#if !SL
	[DevExpressDataAccessLocalizedDescription("MasterDetailInfoKeyColumns")]
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public List<RelationColumnInfo> KeyColumns { get { return keyColumns; } }
#if !SL
	[DevExpressDataAccessLocalizedDescription("MasterDetailInfoName")]
#endif
		public string Name {
			get { return this.customName ?? GenerateName(); }
			set { this.customName = value; }
		}
		bool ShouldSerializeName() { return HasCustomName; }
#if !SL
	[DevExpressDataAccessLocalizedDescription("MasterDetailInfoHasCustomName")]
#endif
		public bool HasCustomName { get { return this.customName != null; } }
		public override string ToString() { return Name; }
		public string GetDetailedDescription() {
			int keyColumnsCount = KeyColumns.Count;
			string detailStr = DetailQueryName ?? STR_Null;
			string masterStr = MasterQueryName ?? STR_Null;
			string[] columnDetails = new string[keyColumnsCount];
			for(int i = 0; i < keyColumnsCount; i++) {
				RelationColumnInfo column = KeyColumns[i];
				columnDetails[i] = string.Format("{0}.{1} = {2}.{3}", detailStr, column.NestedKeyColumn ?? STR_Null, masterStr, column.ParentKeyColumn ?? STR_Null);
			}
			return string.Format("{0} master-detailed {1} on {2}", masterStr, detailStr, string.Join(" and ", columnDetails));
		}
		public string GenerateName() { return this.master + this.detail; }
	}
}
