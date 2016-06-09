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
using System.IO;
using System.Linq;
using System.Data.EntityClient;
using System.Collections.Generic;
namespace DevExpress.Entity.Model.DescendantBuilding {
	public class DefaultDescendantBuilder : DbDescendantBuilder {
		protected override bool SupressExceptions { get { return true; } }
		public DefaultDescendantBuilder(TypesCollector typesCollector)
			: base(typesCollector) {
		}
		public static string GetCeProviderConnectionString(string dataBaseFilePath) {
			if (String.IsNullOrEmpty(dataBaseFilePath))
				return String.Empty;
			return String.Format("Data Source={0}", dataBaseFilePath);
		}
		protected override object CreateDefaultDbConnection(Type dbContextType, TypesCollector typesCollector) {
			try {
				string dbFilePath = Path.Combine(TempFolder, Constants.DatabaseFileName);
				return Activator.CreateInstance(typesCollector.SqlCeConnection.ResolveType(), GetCeProviderConnectionString(dbFilePath));
			}
			catch {
				return null;
			}
		}
		protected override object CreateModelFirstDbConnection(TypesCollector typesCollector) {
			string dbFilePath = Path.Combine(TempFolder, Constants.DatabaseFileName);
			EntityConnectionStringBuilder entityBuilder = new EntityConnectionStringBuilder();
			entityBuilder.ProviderConnectionString = GetCeProviderConnectionString(dbFilePath);
			entityBuilder.Provider = typesCollector.SqlProvider;
			entityBuilder.Metadata = TempFolder;
			return entityBuilder.ToString();
		}
	}
}
