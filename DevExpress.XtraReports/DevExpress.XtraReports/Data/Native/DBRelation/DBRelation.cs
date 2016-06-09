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
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Xpo.DB;
namespace DevExpress.XtraReports.Data {
	enum DBRelationType { OneToMany, OneToOne, ManyToMany }
	class DBRelation {
		readonly DBForeignKey foreignKey;
		readonly string childTable;
		readonly DBRelationType relationType;
		public string ParentTable { get { return foreignKey.PrimaryKeyTable; } }
		public StringCollection ParentColumns { get { return foreignKey.PrimaryKeyTableKeyColumns; } }
		public string ChildTable { get { return childTable; } }
		public StringCollection ChildColumns { get { return foreignKey.Columns; } }
		public DBRelation(DBForeignKey foreignKey, string childTable, DBRelationType relationType) {
			Guard.ArgumentNotNull(foreignKey, "foreignKey");
			Guard.ArgumentIsNotNullOrEmpty(childTable, "childTable");
			this.foreignKey = foreignKey;
			this.childTable = childTable;
			this.relationType = relationType;
		}
		public override string ToString() {
			string pkPart = string.Format("[PKTable = {0} | PKColumns = {1}]", ParentTable, string.Join(",", ParentColumns.OfType<string>().ToArray()));
			string separatorPart = string.Format("-[{0}]->", Enum.GetName(typeof(DBRelationType), relationType));
			string fkPart = string.Format("[Table = {0} | Columns = {1}]", childTable, string.Join(",", ChildColumns.OfType<string>().ToArray()));
			return string.Format("{0}  {1}  {2}", pkPart, separatorPart, fkPart);
		}
		public override int GetHashCode() {
			return this.ToString().GetHashCode();
		}
		public override bool Equals(object obj) {
			DBRelation other = obj as DBRelation;
			return !object.ReferenceEquals(other, null) &&
				this.ParentTable == other.ParentTable &&
				CollectionEquals(this.ParentColumns, other.ParentColumns) &&
				this.ChildTable == other.ChildTable &&
				CollectionEquals(this.ChildColumns, other.ChildColumns);
		}
		static bool CollectionEquals(IList firstCollection, IList secondCollection) {
			if(firstCollection == null || secondCollection == null)
				return firstCollection == secondCollection;
			if(firstCollection.Count != secondCollection.Count)
				return false;
			foreach(var item in firstCollection) {
				if(!secondCollection.Contains(item))
					return false;
			}
			return true;
		}
	}
}
