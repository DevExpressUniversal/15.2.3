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

namespace DevExpress.Design.DataAccess {
	using System;
	using System.Collections.Generic;
	abstract class DataAccessTechnologyConstraint : IDataAccessTechnologyConstraint {
		IEnumerable<Metadata.TypeConstraint> constraintsCore;
		public IEnumerable<Metadata.TypeConstraint> Constraints {
			get {
				if(constraintsCore == null)
					constraintsCore = GetConstraints() ?? new Metadata.TypeConstraint[0];
				return constraintsCore;
			}
		}
		public IEnumerator<Metadata.TypeConstraint> GetEnumerator() {
			return Constraints.GetEnumerator();
		}
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public bool TryGetMembers(Type type, out IEnumerable<System.Reflection.MemberInfo> members) {
			members = null;
			foreach(Metadata.TypeConstraint c in Constraints) {
				if(c.Match(type)) {
					members = c.GetMembers(type);
					break;
				}
			}
			return members != null;
		}
		protected abstract IEnumerable<Metadata.TypeConstraint> GetConstraints();
	}
	class TypedDataSetTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
				new Metadata.PropertyConstraint("System.Data.DataSet", "System.Data.TypedTableBase`1") 
			};
		}
	}
	class SQLDataSourceTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
#if DEBUGTEST
				new Metadata.TypeNameConstraint("DevExpress.Design.DataAccess.Tests.SqlDataSource", "DevExpress.Design.DataAccess.Tests.DataComponentBase"),
#endif
				new Metadata.TypeNameConstraint("DevExpress.DataAccess.Sql.SqlDataSource", "DevExpress.DataAccess.Native.DataComponentBase"),
				new Metadata.TypeNameConstraint("DevExpress.DataAccess.Sql.SqlDataSource", "DevExpress.DataAccess.DataComponentBase")
			};
		}
	}
	class ExcelDataSourceTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
#if DEBUGTEST
				new Metadata.TypeNameConstraint("DevExpress.Design.DataAccess.Tests.ExcelDataSource", "DevExpress.Design.DataAccess.Tests.DataComponentBase"),
#endif
				new Metadata.TypeNameConstraint("DevExpress.DataAccess.Excel.ExcelDataSource", "DevExpress.DataAccess.Native.DataComponentBase"),
				new Metadata.TypeNameConstraint("DevExpress.DataAccess.Excel.ExcelDataSource", "DevExpress.DataAccess.DataComponentBase")
			};
		}
	}
	class LinqToSQLTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
				new Metadata.PropertyConstraint("System.Data.Linq.DataContext", "System.Data.Linq.Table`1") 
			};
		}
	}
	class EntityFrameworkTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
				new Metadata.PropertyConstraint("System.Data.Objects.ObjectContext", "System.Data.Objects.ObjectQuery`1"), 
				new Metadata.PropertyConstraint("System.Data.Objects.ObjectContext", "System.Data.Objects.ObjectSet`1"),   
				new Metadata.PropertyConstraint("System.Data.Entity.DbContext", "System.Data.Entity.DbSet`1") 
			};
		}
	}
	class WcfTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
				new Metadata.PropertyConstraint("System.Data.Services.Client.DataServiceContext", "System.Data.Services.Client.DataServiceQuery`1")
			};
		}
	}
	class RiaTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[]{
				new Metadata.MethodConstraint("System.ServiceModel.DomainServices.Client.DomainContext", "System.ServiceModel.DomainServices.Client.EntityQuery`1", null)
			};
		}
	}
	class IEnumerableTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { };
		}
	}
	class EnumTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { };
		}
	}
	class XPOTechnologyConstraint : DataAccessTechnologyConstraint {
		protected override IEnumerable<Metadata.TypeConstraint> GetConstraints() {
			return new Metadata.TypeConstraint[] { 
#if DEBUGTEST
				new Metadata.TypeSetConstraint("DevExpress.Design.DataAccess.Tests.TestXPObject", "DevExpress.Design.DataAccess.Tests.TestPersistentBase"),
#endif
				new Metadata.TypeSetConstraint("DevExpress.Xpo.XPObject", "DevExpress.Xpo.PersistentBase"),
				new Metadata.TypeSetConstraint("DevExpress.Xpo.XPLiteObject", "DevExpress.Xpo.PersistentBase"),
				new Metadata.TypeSetConstraint("DevExpress.Xpo.XPDataObject", "DevExpress.Xpo.PersistentBase"),
				new Metadata.TypeSetConstraint("DevExpress.Xpo.XPBaseObject", "DevExpress.Xpo.PersistentBase"),
				new Metadata.TypeSetConstraint("DevExpress.Xpo.XPCustomObject", "DevExpress.Xpo.PersistentBase"),
			};
		}
	}
}
