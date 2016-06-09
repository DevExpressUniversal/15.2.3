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
using DevExpress.Utils;
using DevExpress.Data.XtraReports.DataProviders;
namespace DevExpress.XtraReports.Data {
	public class ObjectDataProvider : IDataProvider {
		readonly object dataSource;
		readonly string dataMember;
		public ObjectDataProvider(object dataSource) : this(dataSource, null) {
		}
		public ObjectDataProvider(object dataSource, string dataMember) {
			Guard.ArgumentNotNull(dataSource, "dataSource");
			this.dataSource = dataSource;
			this.dataMember = dataMember;
		}
		#region IDataProvider Members
		public bool TablesOrViewsSupported { get { return false; } }
		public IEnumerable<TableInfo> GetTables() {
			throw new NotSupportedException();
		}
		public IEnumerable<TableInfo> GetViews() {
			throw new NotSupportedException();
		}
		public IEnumerable<ColumnInfo> GetColumns(string dataMember) {
			throw new NotSupportedException();
		}
		public object GetData(string dataMember) {
			return dataSource;
		}
		#endregion
		static string GetEffectivePath(string dataMember, string path) {
			List<string> parts = new List<string>();
			if(!string.IsNullOrEmpty(dataMember))
				parts.Add(dataMember);
			if(!string.IsNullOrEmpty(path))
				parts.Add(path);
			return string.Join(".", parts.ToArray());
		}
	}
}
