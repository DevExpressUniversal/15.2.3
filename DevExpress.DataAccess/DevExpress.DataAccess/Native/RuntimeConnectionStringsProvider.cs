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

using DevExpress.Data.Entity;
using System.Configuration;
using System.Linq;
using System;
namespace DevExpress.DataAccess.Native {
	public class RuntimeConnectionStringsProvider : IConnectionStringsProvider {
		public RuntimeConnectionStringsProvider() {
		}
		public static IConnectionStringInfo[] GetConnectionStringInfos() {
			int count = ConfigurationManager.ConnectionStrings.Count;
			IConnectionStringInfo[] result = new IConnectionStringInfo[count];
			for(int i = 0; i < count; i++) {
				ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings[i];
				result[i] = new ConnectionStringInfo {
					Name = settings.Name,
					RunTimeConnectionString = settings.ConnectionString,
					ProviderName = settings.ProviderName
				};
			}
			return result;
		}
		public IConnectionStringInfo[] GetConnections() {
			return new IConnectionStringInfo[] {};
		}
		public IConnectionStringInfo[] GetConfigFileConnections() {
			return GetConnectionStringInfos();
		}
		public virtual IConnectionStringInfo GetConnectionStringInfo(string connectionStringName) {
			ConnectionStringSettings settings = ConfigurationManager.ConnectionStrings.Cast<ConnectionStringSettings>().FirstOrDefault(ci => ci.Name == connectionStringName);
			if(settings == null)
				throw new InvalidOperationException(string.Format("Can't find connection {0} in config file", connectionStringName));
			return new ConnectionStringInfo {
				Name = settings.Name,
				RunTimeConnectionString = settings.ConnectionString,
				ProviderName = settings.ProviderName
			};
		}
		public string GetConnectionString(string connectionStringName) {
			IConnectionStringInfo csi = GetConnectionStringInfo(connectionStringName);
			return csi != null ? csi.RunTimeConnectionString : null;
		}
	}
}
