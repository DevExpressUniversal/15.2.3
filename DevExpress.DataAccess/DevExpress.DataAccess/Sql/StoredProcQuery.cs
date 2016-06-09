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
using System.Linq;
using System.Reflection;
using DevExpress.DataAccess.Native;
using DevExpress.Xpo.DB;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DataAccess.Sql {
	public sealed class StoredProcQuery : SqlQuery {
		class StoredProcNameTypeConverter : TypeConverter {
			public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType) {
				if(sourceType == typeof(string))
					return false;
				return base.CanConvertFrom(context, sourceType);
			}
		}
		string storedProcName;
		[Obsolete("Use StoredProcQuery(string queryName, string storedProcName) instead.")]
		public StoredProcQuery(string storedProcName) : this(string.Empty, storedProcName) { }
		public StoredProcQuery(string queryName, string storedProcName) : base(queryName) {
			this.storedProcName = storedProcName;
		}
		public StoredProcQuery() : this(string.Empty, (string)null) { }
		[Obsolete("Use StoredProcQuery(string queryName, string storedProcName, IEnumerable<QueryParameter> parameters) instead.")]
		public StoredProcQuery(string storedProcName, IEnumerable<QueryParameter> parameters) 
			: this(string.Empty, storedProcName, parameters) { }
		public StoredProcQuery(string queryName, string storedProcName, IEnumerable<QueryParameter> parameters) 
			: this(queryName, storedProcName) {
				Parameters.AddRange(parameters);
		}
		internal StoredProcQuery(StoredProcQuery other) : base(other) { this.storedProcName = other.storedProcName; }
		[TypeConverter(typeof(StoredProcNameTypeConverter))]
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.Sql.StoredProcQuery.StoredProcName")]
#if !SL
	[DevExpressDataAccessLocalizedDescription("StoredProcQueryStoredProcName")]
#endif
		[LocalizableCategory(Localization.DataAccessStringId.QueryPropertyGridStoredProcCategoryName)]
		public string StoredProcName { get { return storedProcName; } set { storedProcName = value; } }
		public override void Validate() {
			if(string.IsNullOrEmpty(storedProcName)) {
				throw new StoredProcNameNullValidationException();
			}
		}
		public override void Validate(DBSchema schema) {
			Validate();
			if(schema == null)
				return;
			DBStoredProcedure dbProc = schema.StoredProcedures.FirstOrDefault(proc => string.Equals(proc.Name, storedProcName));
			if(object.ReferenceEquals(dbProc, null))
				throw new StoredProcNotInSchemaValidationException(storedProcName);
			List<DBStoredProcedureArgument> inArgs = dbProc.Arguments.Where(arg => arg.Direction != DBStoredProcedureArgumentDirection.Out).ToList();
			if(Parameters.Count != inArgs.Count)
				throw new StoredProcParamCountValidationException(Parameters.Count, inArgs.Count);
			var argEnumerator = inArgs.GetEnumerator();
			var parEnumerator = Parameters.GetEnumerator();
			while(argEnumerator.MoveNext() && parEnumerator.MoveNext()) {
				DBStoredProcedureArgument arg = argEnumerator.Current;
				QueryParameter par = parEnumerator.Current;
				if(par == null)
					throw new StoredProcParamNullValidationException();
				if(par.Name != arg.Name)
					throw new StoredProcParamNameValidationException(par.Name, arg.Name);
				if(par.Type != typeof(Expression) && !DBColumn.GetType(arg.Type).IsAssignableFrom(par.Type))
					throw new StoredProcParamTypeValidationException(par.Type.Name, arg.Type.ToString());
			}
		}
		internal override bool EqualsCore(SqlQuery obj) {
			StoredProcQuery other = (StoredProcQuery)obj;
			return string.Equals(this.storedProcName, other.storedProcName);
		}
		internal override SqlQuery Clone() { return new StoredProcQuery(this); }
	}
}
