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
using System.IO;
using System.Linq;
using System.Text;
namespace DevExpress.Map.Native {
	public class ShpExporter {
		IList<IMapItemCore> GetTypedItems(IEnumerable<IMapItemCore> items, ShpRecordTypes shpType) {
			return items.Where(item => IsItemOfType(item, shpType)).ToList();
		}
		void ExportToDbf(string dbfPath, IList<IMapItemCore> items) {
			DbfExporter exporter = new DbfExporter(items);
			using(FileStream stream = new FileStream(dbfPath, FileMode.Create)) {
				exporter.Export(stream);
			}
		}
		internal bool IsItemOfType(IMapItemCore item, ShpRecordTypes shpType) {
			switch(shpType) {
				case ShpRecordTypes.NullShape: return false;
				case ShpRecordTypes.Point: return item is IPointCore;
				case ShpRecordTypes.MultiPoint: return item is IPointCore;
				case ShpRecordTypes.PolyLine: return item is IPointContainerCore;
				case ShpRecordTypes.Polygon: return item is IPathCore || item is IPolygonCore;
				case ShpRecordTypes.MultiPointM: return item is IPointCore;
				case ShpRecordTypes.PointM: return item is IPointCore;
				case ShpRecordTypes.PolyLineM: return item is IPointContainerCore;
				case ShpRecordTypes.PolygonM: return item is IPathCore || item is IPolygonCore;
				case ShpRecordTypes.PointZ: return item is IPointCore;
				case ShpRecordTypes.MultiPatch: return false;
				case ShpRecordTypes.MultiPointZ: return item is IPointCore;
				case ShpRecordTypes.PolygonZ: return item is IPathCore || item is IPolygonCore;
				case ShpRecordTypes.PolyLineZ: return item is IPointContainerCore;
				default: return false;
			}
		}
		public void Export(string shpPath, IEnumerable<IMapItemCore> items, ShpRecordTypes shpType, bool exportToDbf, CoordinateConverterCore converter) {
			IList<IMapItemCore> matchedItems = GetTypedItems(items, shpType);
			ShapeExporterCoreBase exporter = ShapeExporterFactory.CreateCoreExporter(shpType);
			exporter.UpdateExportingItems(matchedItems);
			using(FileStream stream = new FileStream(shpPath, FileMode.Create)) {
				exporter.Export(stream);
			}
			if(exportToDbf)
				ExportToDbf(CoreUtils.GetDbfPath(shpPath), matchedItems);
		}
	}
}
