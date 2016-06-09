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
using System.Reflection;
using System.Data.Metadata.Edm;
using System.Collections.Generic;
using DevExpress.Entity.ProjectModel;
using DevExpress.Utils;
namespace DevExpress.Entity.Model {
	public class TypesCollector {
		TypesCollector(Type type) {
			DbDescendantType = type;
			Init(type);
			TryLoadSqlCeServer();
			InitSystemDataEntityTypes();
		}
		public TypesCollector(IDXTypeInfo typeInfo) :
			this(typeInfo.ResolveType()) {
			DbDescendantInfo = typeInfo;
		}
		void Init(Type type) {
			try {
				DbContextType = EntityFrameworkModelBase.GetBaseContextType(type, Constants.DbContextTypeName) ?? EntityFrameworkModelBase.GetBaseContextType(type, Constants.DbContextEF7TypeName);
				if(DbContextType == null)
					return;
				Type[] asmTypes = DbContextType.GetAssembly().GetTypes();
				foreach(Type asmType in asmTypes) {
					if(IsCollected)
						break;
					TryInitFrameworkTypes(asmType);
				}
			} catch {
				return;
			}
		}
		void TryLoadSqlCeServer() {
			Assembly result = null;
			try {
				result = Assembly.Load(Constants.SystemDataSqlCe40AssemblyFullName);
				SqlProvider = Constants.Sql40ProviderName;
			} catch {
			}
			if(result == null)
				try {
					result = Assembly.Load(Constants.SystemDataSqlCe351AssemblyFullName);
					SqlProvider = Constants.Sql35ProviderName;
				} catch {
				}
			if(result == null)
				try {
					result = Assembly.Load(Constants.SystemDataSqlCe350AssemblyFullName);
					SqlProvider = Constants.Sql35ProviderName;
				} catch {
				}
			if(result == null)
				return;
			Type type = result.GetType(Constants.SqlCeConnectionTypeName);
			if(type == null)
				return;
			SqlCeConnection = new DXTypeInfo(type);
		}
		void InitSystemDataEntityTypes() {
			Assembly systemDataEntity = null;
			bool isEF6 = EntityFrameworkModelBase.IsAtLeastEF6(DbContextType);
			systemDataEntity = isEF6 ? DbContextType.GetAssembly() : typeof(MetadataItem).Assembly;
			Type metadataHelperType = systemDataEntity.GetType(isEF6 ? "System.Data.Entity.Core.Common.Utils.MetadataHelper" : Constants.MetadataHelperTypeName);
			if(metadataHelperType != null)
				MetadataHelper = new DXTypeInfo(metadataHelperType);
			bool isEF7 = EntityFrameworkModelBase.IsAtLeastEF7(DbContextType);
			if(isEF7) {
				Type dbSetFinderType = systemDataEntity.GetType("Microsoft.Data.Entity.Internal.DbSetFinder");
				if(dbSetFinderType != null)
					DbSetFinder = new DXTypeInfo(dbSetFinderType);
				DbContextOptions = systemDataEntity.GetType("Microsoft.Data.Entity.Infrastructure.DbContextOptions");
				DbContextOptionsT = systemDataEntity.GetType("Microsoft.Data.Entity.Infrastructure.DbContextOptions`1");
				DbContextOptionsBuilder = systemDataEntity.GetType("Microsoft.Data.Entity.DbContextOptionsBuilder");
			}
		}
		void TryInitFrameworkTypes(Type type) {
			switch(type.FullName) {
				case Constants.DbContextTypeName:
				case Constants.DbContextEF7TypeName:
					DbContext = new DXTypeInfo(type);
					return;
				case Constants.DbSetTypeName:
					DbSet = new DXTypeInfo(type);
					return;
				case Constants.IObjectContextAdapterTypeName:
					IObjectContextAdapter = new DXTypeInfo(type);
					return;
			}
		}
		bool IsCollected { get { return IObjectContextAdapter != null && DbContext != null && DbSet != null; } }
		public string SqlProvider { get; private set; }
		public IDXTypeInfo MetadataHelper { get; protected set; }
		public IDXTypeInfo IObjectContextAdapter { get; private set; }
		public IDXTypeInfo DbSet { get; private set; }
		public IDXTypeInfo DbSetFinder { get; private set; }
		public IDXTypeInfo DbContext { get; private set; }
		public Type DbContextOptions { get; private set; }
		public Type DbContextOptionsT { get; private set; }
		public Type DbContextOptionsBuilder { get; private set; }
		public IDXTypeInfo SqlCeConnection { get; set; }
		public IDXTypeInfo DbDescendantInfo { get; private set; }
		public Type DbContextType { get; private set; }
		public Type DbDescendantType { get; private set; }
	}
}
