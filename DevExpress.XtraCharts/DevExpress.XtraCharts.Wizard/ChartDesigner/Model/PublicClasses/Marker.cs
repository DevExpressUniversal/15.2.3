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
using System.Drawing;
using System.Linq;
using System.Text;
using DevExpress.Utils.Design;
using DevExpress.XtraCharts.Design;
namespace DevExpress.XtraCharts.Designer.Native {
	[ModelOf(typeof(MarkerBase)), TypeConverter(typeof(BaseMarkerTypeConverter))]
	public class MarkerBaseModel : DesignerChartElementModelBase {
		readonly MarkerBase marker;
		PolygonFillStyleModel fillStyleModel;
		protected MarkerBase Marker { get { return marker; } }
		protected internal override ChartElement ChartElement { get { return marker; } }
		[PropertyForOptions,
		DependentUpon("MarkerVisibility", -1)]
		public MarkerKind Kind {
			get { return Marker.Kind; }
			set { SetProperty("Kind", value); }
		}
		public int StarPointCount {
			get { return Marker.StarPointCount; }
			set { SetProperty("StarPointCount", value); }
		}
		[TypeConverter(typeof(ExpandableObjectConverter))]
		public PolygonFillStyleModel FillStyle { get { return fillStyleModel; } }
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool BorderVisible {
			get { return Marker.BorderVisible; }
			set { SetProperty("BorderVisible", value); }
		}
		public Color BorderColor {
			get { return Marker.BorderColor; }
			set { SetProperty("BorderColor", value); }
		}
		public MarkerBaseModel(MarkerBase marker, CommandManager commandManager)
			: base(commandManager) {
			this.marker = marker;
			Update();
		}
		protected override void AddChildren() {
			if(fillStyleModel != null)
				Children.Add(fillStyleModel);
			base.AddChildren();
		}
		public override void Update() {
			this.fillStyleModel = new PolygonFillStyleModel(Marker.FillStyle, CommandManager);
			ClearChildren();
			AddChildren();
			base.Update();
		}
	}
	[ModelOf(typeof(SimpleMarker))]
	public class SimpleMarkerModel : MarkerBaseModel {
		protected new SimpleMarker Marker { get { return (SimpleMarker)base.Marker; } }
		[PropertyForOptions,
		DependentUpon("MarkerVisibility", -1)]
		public int Size {
			get { return Marker.Size; }
			set { SetProperty("Size", value); }
		}
		public SimpleMarkerModel(SimpleMarker marker, CommandManager commandManager)
			: base(marker, commandManager) {
		}
	}
	[ModelOf(typeof(Marker))]
	public class MarkerModel : SimpleMarkerModel {
		protected new Marker Marker { get { return (Marker)base.Marker; } }
		[PropertyForOptions,
		DependentUpon("MarkerVisibility", -1)]
		public Color Color {
			get { return Marker.Color; }
			set { SetProperty("Color", value); }
		}
		public MarkerModel(Marker marker, CommandManager commandManager)
			: base(marker, commandManager) {
		}
	}
}
