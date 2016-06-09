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

using System.ComponentModel;
using DevExpress.PivotGrid.OLAP;
using DevExpress.PivotGrid.OLAP.AdoWrappers;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraPivotGrid {
	public class PivotGridAdomdDataSource : OLAPDataSourceBase {
		AdomdVersion? version = null;
		public PivotGridAdomdDataSource() : base() { }
		public PivotGridAdomdDataSource(AdomdVersion version) : base() {
			this.version = version;
		}
		protected PivotGridAdomdDataSource(OLAPMetadata metadata) : base(metadata) {
		}
		protected PivotGridAdomdDataSource(OLAPMetadata metadata, AdomdVersion version) : base(metadata) {
			this.version = version;
		}
		public string ConnectionString {
			get { return OLAPMetadata.FullConnectionString; }
			set { OLAPMetadata.FullConnectionString = value; }
		}
		protected override bool RequirePopulateColumnsOnEmptyOnly() {
			return false;
		}
		protected override IQueryContext<OLAPCubeColumn> CreateQueryContext(GroupInfo[] columns, GroupInfo[] rows, bool columnExpand, bool rowExpand) {
			return new OLAPQueryContextBase(columns, rows, columnExpand, rowExpand, this);
		}
		protected override OLAPMetadata CreateMetadata() {
			return version.HasValue ? new AdoMetadata(version.Value) : new AdoMetadata();
		}
		protected override QueryDataSource<OLAPCubeColumn> CreateInstance() {
			if(version.HasValue)
				return new PivotGridAdomdDataSource(OLAPMetadata, version.Value);
			else
				return new PivotGridAdomdDataSource(OLAPMetadata);
		}
	}
}
