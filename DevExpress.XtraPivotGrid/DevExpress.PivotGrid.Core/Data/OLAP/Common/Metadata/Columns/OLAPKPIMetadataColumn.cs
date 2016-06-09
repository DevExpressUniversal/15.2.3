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
using DevExpress.Data.IO;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.PivotGrid.OLAP {
	public class OLAPKPIMetadataColumn : OLAPMetadataColumn {
		static Dictionary<string, PivotKPIGraphic> ServerGraphicMap = GetServerGraphicMap();
		static Dictionary<string, PivotKPIGraphic> GetServerGraphicMap() {
			Dictionary<string, PivotKPIGraphic> ServerGraphicMap = new Dictionary<string, PivotKPIGraphic>();
			ServerGraphicMap.Add("Shapes", PivotKPIGraphic.Shapes);
			ServerGraphicMap.Add("Traffic Light", PivotKPIGraphic.TrafficLights);
			ServerGraphicMap.Add("Traffic Light - Single", PivotKPIGraphic.TrafficLights);
			ServerGraphicMap.Add("Traffic Light - Multiple", PivotKPIGraphic.TrafficLights);
			ServerGraphicMap.Add("Road Signs", PivotKPIGraphic.RoadSigns);
			ServerGraphicMap.Add("Gauge - Ascending", PivotKPIGraphic.Gauge);
			ServerGraphicMap.Add("Gauge - Descending", PivotKPIGraphic.ReversedGauge);
			ServerGraphicMap.Add("Thermometer", PivotKPIGraphic.Thermometer);
			ServerGraphicMap.Add("Cylinder", PivotKPIGraphic.Cylinder);
			ServerGraphicMap.Add("Smiley Face", PivotKPIGraphic.Faces);
			ServerGraphicMap.Add("Variance Arrow", PivotKPIGraphic.VarianceArrow);	
			ServerGraphicMap.Add("Standard Arrow", PivotKPIGraphic.StandardArrow);
			ServerGraphicMap.Add("Status Arrow - Ascending", PivotKPIGraphic.StatusArrow);
			ServerGraphicMap.Add("Status Arrow - Descending", PivotKPIGraphic.ReversedStatusArrow);
			return ServerGraphicMap;
		}
		string displayFolder;
		PivotKPIGraphic graphic;
		PivotKPIType type;
		public override string DisplayFolder { get { return displayFolder; } }
		public PivotKPIGraphic Graphic { get { return graphic; } }
		public PivotKPIType Type { get { return type; } }
		public override byte TypeCode { get { return OLAPKPIColumnTypeCode; } }
		public OLAPKPIMetadataColumn() { }
		public OLAPKPIMetadataColumn(int level, int cardinality, Type dataType, MetadataColumnBase parentColumn,
									  OLAPHierarchy columnHierarchy, string drilldownColumn, string defaultMemberName, OLAPHierarchy hierarchy, OLAPDataType olapDataType,
									  string graphic, PivotKPIType type, string displayFolder)
			: base(level, cardinality, dataType, parentColumn, columnHierarchy, drilldownColumn, defaultMemberName, hierarchy, olapDataType) {
			this.graphic = ConvertGraphic(graphic);
			this.type = type;
			this.displayFolder = displayFolder;
		}
		protected override void SaveToStream(IQueryMetadata owner, TypedBinaryWriter writer) {
			base.SaveToStream(owner, writer);
			writer.Write((int)Graphic);
			writer.Write((int)Type);
		}
		protected PivotKPIGraphic ConvertGraphic(string graphic) {
			if(!string.IsNullOrEmpty(graphic) && ServerGraphicMap.ContainsKey(graphic))
				return ServerGraphicMap[graphic];
			return PivotKPIGraphic.None;
		}
		public override void RestoreFromStream(IQueryMetadata metadata, TypedBinaryReader reader) {
			base.RestoreFromStream(metadata, reader);
			graphic = (PivotKPIGraphic)reader.ReadInt32();
			type = (PivotKPIType)reader.ReadInt32();
			displayFolder = string.Empty;
		}
	}
}
