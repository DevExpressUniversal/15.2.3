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
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.Compatibility.System.ComponentModel;
#if !DXPORTABLE
using System.Drawing.Design;
#endif
namespace DevExpress.DataAccess.Sql {
	public abstract class SqlQuery {
		public abstract class EqualityComparer : IEqualityComparer<SqlQuery> {
			public static bool Equals(SqlQuery x, SqlQuery y) {
				return Equals(x, y, true);
			}
			public static bool Equals(SqlQuery x, SqlQuery y, bool checkName) {
				if(ReferenceEquals(x, y))
					return true;
				if(x == null || y == null || x.GetType() != y.GetType())
					return false;
				if(checkName && !string.Equals(x.name, y.name, StringComparison.Ordinal))
					return false;
				int paramCount = x.parameters.Count;
				if(y.parameters.Count != paramCount)
					return false;
				for(int i = 0; i < paramCount; i++)
					if(!QueryParameter.EqualityComparer.Equals(x.parameters[i], y.parameters[i]))
						return false;
				return x.EqualsCore(y);
			}
			#region IEqualityComparer<SqlQuery> Members
			bool IEqualityComparer<SqlQuery>.Equals(SqlQuery x, SqlQuery y) { return Equals(x, y); }
			int IEqualityComparer<SqlQuery>.GetHashCode(SqlQuery obj) { return 0; }
			#endregion
		}
		readonly List<QueryParameter> parameters;
		string name;
		SqlQueryCollection sqlQueryCollection;
		[Obsolete("Use SqlQuery(string name) instead")]
		protected SqlQuery() : this(string.Empty) {}
		protected SqlQuery(string name) {
			parameters = new List<QueryParameter>();
			this.name = name;
		}
		protected SqlQuery(SqlQuery other) {
			Guard.ArgumentNotNull(other, "other");
			this.name = other.name;
			parameters = new List<QueryParameter>();
			foreach(QueryParameter pararmeter in other.parameters)
				this.parameters.Add(new QueryParameter(pararmeter));
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
#if !DXPORTABLE
		[Editor("DevExpress.DataAccess.UI.Native.Sql.SqlQueryParametersEditor, " + AssemblyInfo.SRAssemblyDataAccessUI, typeof(UITypeEditor))]
#endif
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.SqlQuery.Parameters")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlQueryParameters")]
#endif
		[LocalizableCategory(DataAccessStringId.QueryPropertyGridCommonCategoryName)]
		public List<QueryParameter> Parameters { get { return parameters; } }
		[DefaultValue("")]
		[NotifyParentProperty(true)]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.SqlQuery.Name")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("SqlQueryName")]
#endif
		[LocalizableCategory(DataAccessStringId.QueryPropertyGridCommonCategoryName)]
		public string Name { 
			get { return name; } 
			set {
				if(name == value)
					return;
				name = value;
				if(sqlQueryCollection != null) {
					sqlQueryCollection.CheckQueryName(this);
				}
			} 
		}
		public abstract void Validate();
		public abstract void Validate(DBSchema schema);
		internal SqlDataSource DataSource { 
			get {
				if(sqlQueryCollection == null)
					throw new InvalidOperationException("Query doesn't have an owner");
				return sqlQueryCollection.DataSource; 
			} 
		}
		internal SqlQueryCollection Owner {
			get { return sqlQueryCollection; }
			set { 
				sqlQueryCollection = value;
				if(sqlQueryCollection != null)
					sqlQueryCollection.CheckQueryName(this);
			}
		}
		public override string ToString() {
			if(!string.IsNullOrEmpty(Name))
				return Name;
			return base.ToString();
		}
		internal abstract bool EqualsCore(SqlQuery obj);
		internal abstract SqlQuery Clone();
	}
}
