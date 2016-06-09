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
using System.Linq;
using DevExpress.PivotGrid.QueryMode;
namespace DevExpress.PivotGrid.OLAP {
	public abstract class AxisColumnsProviderBase {
		public abstract OLAPCubeColumn GetColumn(string uniqueName);
		public abstract IEnumerable<OLAPCubeColumn> EnumerateMeasures();
		public abstract OLAPMetadataColumn ResolveMeasure(string uniqueName);
		public abstract OLAPMetadataColumn GetMetadataByHierarchy(string hierarchyName);
	}
	public abstract class AxisColumnsProvider : AxisColumnsProviderBase {
		OLAPMetadata metadata;
		public AxisColumnsProvider(OLAPMetadata metadata) {
			this.metadata = metadata;
		}
		public override OLAPMetadataColumn ResolveMeasure(string uniqueName) {
			return metadata.Columns[uniqueName];
		}
	}
	public class ByAreasAxisColumnProvider : AxisColumnsProvider {
		readonly QueryAreas<OLAPCubeColumn> areas;
		public ByAreasAxisColumnProvider(QueryMode.QueryAreas<OLAPCubeColumn> queryAreas, OLAPMetadata metadata)
			: base(metadata) {
			this.areas = queryAreas;
		}
		public override OLAPCubeColumn GetColumn(string uniqueName) {
			foreach(OLAPCubeColumn column in areas.ColumnArea)
				if(column.UniqueName == uniqueName)
					return column;
			foreach(OLAPCubeColumn column in areas.RowArea)
				if(column.UniqueName == uniqueName)
					return column;
			throw new NotImplementedException("not founded");
		}
		public override IEnumerable<OLAPCubeColumn> EnumerateMeasures() {
			return areas.ServerSideDataArea;
		}
		public override OLAPMetadataColumn GetMetadataByHierarchy(string hierarchyName) {
			foreach(OLAPCubeColumn column in areas.ColumnArea)
				if(column.Hierarchy.UniqueName == hierarchyName)
					return column.Metadata;
			foreach(OLAPCubeColumn column in areas.RowArea)
				if(column.Hierarchy.UniqueName == hierarchyName)
					return column.Metadata;
			throw new NotImplementedException("not founded");
		}
	}
	public class OneColumnAxisColumnsProvider : AxisColumnsProviderBase {
		OLAPCubeColumn column;
		OLAPMetadataColumn meta;
		public OneColumnAxisColumnsProvider(OLAPCubeColumn column, OLAPMetadataColumn meta) {
			this.column = column;
			this.meta = meta;
		}
		public override OLAPCubeColumn GetColumn(string uniqueName) {
			if(meta.UniqueName != uniqueName)
				throw new NotImplementedException();
			return column;
		}
		public override IEnumerable<OLAPCubeColumn> EnumerateMeasures() {
			throw new NotImplementedException();
		}
		public override OLAPMetadataColumn GetMetadataByHierarchy(string hierarchyName) {
			if(meta.Hierarchy.UniqueName != hierarchyName)
				throw new NotImplementedException();
			return meta;
		}
		public override OLAPMetadataColumn ResolveMeasure(string uniqueName) {
			throw new NotImplementedException();
		}
	}
	public class KpiAxisColumnProvider : AxisColumnsProvider {
		public KpiAxisColumnProvider(OLAPMetadata metadata)
			: base(metadata) {
		}
		public override OLAPCubeColumn GetColumn(string uniqueName) {
			throw new NotImplementedException();
		}
		public override IEnumerable<OLAPCubeColumn> EnumerateMeasures() {
			return Enumerable.Empty<OLAPCubeColumn>();
		}
		public override OLAPMetadataColumn GetMetadataByHierarchy(string hierarchyName) {
			throw new NotImplementedException();
		}
	}
}
