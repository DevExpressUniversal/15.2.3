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
using System.Reflection;
namespace DevExpress.DataAccess.EntityFramework {
	public class EFConnectionParameters {
		public class EqualityComparer : IEqualityComparer<EFConnectionParameters> {
			public static bool Equals(EFConnectionParameters x, EFConnectionParameters y) {
				if(object.ReferenceEquals(x, y))
					return true;
				if(x == null || y == null)
					return false;
				if(!string.Equals(x.ConnectionString, y.ConnectionString))
					return false;
				if(!string.Equals(x.ConnectionStringName, y.ConnectionStringName))
					return false;
				if(!string.Equals(x.CustomContextName, y.CustomContextName))
					return false;
				if(!string.Equals(x.CustomAssemblyPath, y.CustomAssemblyPath))
					return false;
				if(x.Source != y.Source)
					return false;
				return true;
			}
			#region IEqualityComparer<StoredProcedureInfo> Members
			bool IEqualityComparer<EFConnectionParameters>.Equals(EFConnectionParameters x, EFConnectionParameters y) {
				return Equals(x, y);
			}
			int IEqualityComparer<EFConnectionParameters>.GetHashCode(EFConnectionParameters obj) {
				return 0;
			}
			#endregion
		}
		Type source;
		[DefaultValue(null), DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.EntityFramework.EFConnectionParameters.ConnectionString"), Category("Data")]
		public string ConnectionString { get; set; }
		[DefaultValue(null), DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.EntityFramework.EFConnectionParameters.ConnectionStringName"), Category("Data")]
		public string ConnectionStringName { get; set; }
		[DXDisplayName(typeof(ResFinder), "DevExpress.DataAccess.EntityFramework.EFConnectionParameters.Source")]
		[Category("Data")]
		public Type Source
		{
			get
			{
				if(this.source == null)
					if(!string.IsNullOrEmpty(CustomAssemblyPath)) {
						Assembly assembly = Assembly.LoadFile(CustomAssemblyPath);
						if(!string.IsNullOrEmpty(CustomContextName)) {
							this.source = assembly.GetType(CustomContextName, true);
						}
					}
				return this.source;
			}
			set { this.source = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string CustomContextName { get; set; }
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string CustomAssemblyPath { get; set; }
		public EFConnectionParameters(Type source) {
			Source = source;
		}
		public EFConnectionParameters(string contextName, string assemblyPath) {
			CustomContextName = contextName;
			CustomAssemblyPath = assemblyPath;
		}
		public EFConnectionParameters(Type source, string connectionStringName) :
			this(source) {
			ConnectionStringName = connectionStringName;
		}
		public EFConnectionParameters(Type source, string connectionStringName, string connectionString) :
			this(source) {
			ConnectionStringName = connectionStringName;
			ConnectionString = connectionString;
		}
		public EFConnectionParameters(string contextName, string assemblyPath, string connectionStringName, string connectionString) :
			this(contextName, assemblyPath) {
			ConnectionStringName = connectionStringName;
			ConnectionString = connectionString;
		}
		public EFConnectionParameters(EFConnectionParameters other) :
			this(other.source, other.ConnectionStringName, other.ConnectionString) {
			CustomContextName = other.CustomContextName;
			CustomAssemblyPath = other.CustomAssemblyPath;
		}
		public EFConnectionParameters() {
		}
		bool ShouldSerializeSource() {
			return Source != null && string.IsNullOrWhiteSpace(CustomContextName) && string.IsNullOrWhiteSpace(CustomAssemblyPath);
		}
		bool ShouldSerializeCustomContextName() {
			return !string.IsNullOrWhiteSpace(CustomContextName);
		}
		bool ShouldSerializeCustomAssemblyPath() {
			return !string.IsNullOrWhiteSpace(CustomAssemblyPath);
		}
	}
}
