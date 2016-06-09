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
using System.Globalization;
using System.Runtime.InteropServices;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.History;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using DevExpress.XtraSpreadsheet.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region BandFormat
	public class BandFormat {
		#region Fields
		readonly IChart parent;
		int bandId;
		ShapeProperties shapeProperties;
		#endregion
		public BandFormat(IChart parent, int bandId) {
			Guard.ArgumentNotNull(parent, "parent");
			Guard.ArgumentNonNegative(bandId, "bandId");
			this.parent = parent;
			this.bandId = bandId;
			this.shapeProperties = new ShapeProperties(DocumentModel) { Parent = parent };
		}
		#region Properties
		protected internal IChart Parent { get { return parent; } }
		protected internal DocumentModel DocumentModel { get { return parent.DocumentModel; } }
		public int BandId { get { return bandId; } }
		public ShapeProperties ShapeProperties { get { return shapeProperties; } }
		#endregion
		protected internal void SetBandIdCore(int value) {
			Guard.ArgumentNonNegative(value, "value");
			this.bandId = value;
		}
		public void ResetToStyle() {
			ShapeProperties.ResetToStyle();
		}
	}
	#endregion
	#region BandFormatCollection
	public class BandFormatCollection : ChartUndoableCollection<BandFormat> {
		public BandFormatCollection(IChart parent)
			: base(parent) {
		}
	}
	#endregion
	#region SurfaceChartViewBase
	public abstract class SurfaceChartViewBase : ChartViewBase {
		#region Fields
		BandFormatCollection bandFormats;
		#endregion
		protected SurfaceChartViewBase(IChart parent)
			: base(parent) {
			this.bandFormats = new BandFormatCollection(parent);
		}
		#region Properties
		#region Wireframe
		public bool Wireframe {
			get { return Info.Wireframe; }
			set {
				if(Wireframe == value)
					return;
				SetPropertyValue(SetWireframeCore, value);
			}
		}
		DocumentModelChangeActions SetWireframeCore(ChartViewInfo info, bool value) {
			info.Wireframe = value;
			return DocumentModelChangeActions.None;
		}
		#endregion
		public BandFormatCollection BandFormats { get { return bandFormats; } }
		public override bool Is3DView { get { return true; } }
		#endregion
		protected override void CopyFrom(IChartView value, bool copySeries) {
			base.CopyFrom(value, copySeries);
			SurfaceChartViewBase view = value as SurfaceChartViewBase;
			if (view != null)
				CopyFromCore(view);
		}
		void CopyFromCore(SurfaceChartViewBase value) {
			bandFormats.Clear();
			for (int i = 0; i < value.bandFormats.Count; i++) {
				BandFormat source = value.bandFormats[i];
				BandFormat item = new BandFormat(Parent, source.BandId);
				item.ShapeProperties.CopyFrom(source.ShapeProperties);
				bandFormats.Add(item);
			}
		}
		public override void ResetToStyle() {
			base.ResetToStyle();
			foreach (BandFormat format in BandFormats)
				format.ResetToStyle();
		}
	}
	#endregion
	#region SurfaceChartView
	public class SurfaceChartView : SurfaceChartViewBase {
		public SurfaceChartView(IChart parent)
			: base(parent) {
		}
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Surface; } }
		public override ChartType ChartType { get { return Wireframe ? ChartType.SurfaceWireframe : ChartType.Surface; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValue; } }
		public override IChartView CloneTo(IChart parent) {
			SurfaceChartView result = new SurfaceChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return new SurfaceSeries(this);
		}
		#endregion
	}
	#endregion
	#region Surface3DChartView
	public class Surface3DChartView : SurfaceChartViewBase {
		public Surface3DChartView(IChart parent)
			: base(parent) {
		}
		#region IChartView Members
		public override ChartViewType ViewType { get { return ChartViewType.Surface3D; } }
		public override ChartType ChartType { get { return Wireframe ? ChartType.Surface3DWireframe : ChartType.Surface3D; } }
		public override AxisGroupType AxesType { get { return AxisGroupType.CategoryValueSeries; } }
		public override IChartView CloneTo(IChart parent) {
			Surface3DChartView result = new Surface3DChartView(parent);
			result.CopyFrom(this);
			return result;
		}
		public override void Visit(IChartViewVisitor visitor) {
			visitor.Visit(this);
		}
		public override ISeries CreateSeriesInstance() {
			return new SurfaceSeries(this);
		}
		#endregion
	}
	#endregion
}
