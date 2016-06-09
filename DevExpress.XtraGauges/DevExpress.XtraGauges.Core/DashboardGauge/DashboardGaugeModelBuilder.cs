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

using System.Collections.Generic;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.XtraGauges.Core.Customization {
	public interface IDashboardGaugeModelProvider {
		void BuildModel(IDashboardGauge gauge);
	}
	public interface IDashboardGaugeModelProviderFactory {
		IDashboardGaugeModelProvider Resolve(DashboardGaugeType type);
	}
	class DashboardGaugeModelProviderFactory : IDashboardGaugeModelProviderFactory {
		IDictionary<DashboardGaugeType, IDashboardGaugeModelProvider> providers;
		public DashboardGaugeModelProviderFactory() {
			providers = new Dictionary<DashboardGaugeType, IDashboardGaugeModelProvider>();
			providers.Add(DashboardGaugeType.Circular, new CircularDashboardGaugeModelProvider());
			providers.Add(DashboardGaugeType.Linear, new LinearDashboardGaugeModelProvider());
		}
		public IDashboardGaugeModelProvider Resolve(DashboardGaugeType type) {
			return providers[type];
		}
	}
	abstract class BaseDashboardGaugeModelProvider : IDashboardGaugeModelProvider {
		public void BuildModel(IDashboardGauge gauge) {
			BuildModelCore(gauge);
		}
		protected abstract void BuildModelCore(IDashboardGauge gauge);
	}
	class CircularDashboardGaugeModelProvider : BaseDashboardGaugeModelProvider {
		protected override void BuildModelCore(IDashboardGauge gauge) {
			ArcScale arcScale = new ArcScale() { Name = "ArcScale" };
			ArcScaleNeedle arcScaleNeedle = new ArcScaleNeedle() { Name = "ArcScaleNeedle" };
			ArcScaleBackgroundLayer arcScaleBackgroundLayer = new ArcScaleBackgroundLayer() { Name = "ArcScaleBackgroundLayer" };
			gauge.Scale = arcScale.Provider;
			arcScaleNeedle.ArcScale = arcScale;
			arcScaleBackgroundLayer.ArcScale = arcScale;
			arcScaleNeedle.ZOrder = -50;
			gauge.Model.Composite.AddRange(new IElement<IRenderableElement>[] { arcScale, arcScaleBackgroundLayer, arcScaleNeedle });
			gauge.Elements.AddRange(new ISerizalizeableElement[] { arcScale, arcScaleNeedle, arcScaleBackgroundLayer });
			if(gauge.ShowMarker) {
				ArcScaleMarker arcScaleMarker = new ArcScaleMarker() { Name = "arcScaleMarker" };
				gauge.Marker = arcScaleMarker;
				arcScaleMarker.ArcScale = arcScale;
				arcScaleMarker.ZOrder = -100;
				gauge.Model.Composite.Add(arcScaleMarker);
				gauge.Elements.Add(arcScaleMarker);
			}
		}
	}
	class LinearDashboardGaugeModelProvider : BaseDashboardGaugeModelProvider {
		static SolidBrushObject RangeBrush = new SolidBrushObject("Color:#E73141");
		protected override void BuildModelCore(IDashboardGauge gauge) {
			LinearScale linearScale = new LinearScale() { Name = "LinearScale" };
			LinearScaleRangeBar linearScaleRangeBar = new LinearScaleRangeBar() { Name = "LinearScaleRangeBar" };
			gauge.Scale = linearScale.Provider;
			linearScaleRangeBar.LinearScale = linearScale;
			linearScaleRangeBar.ZOrder = 100;
			linearScaleRangeBar.Appearance.ContentBrush = RangeBrush;
			linearScaleRangeBar.StartOffset = 4F;
			linearScaleRangeBar.EndOffset = 8F;
			ModelRoot rotationNode = new ModelRoot(PredefinedCoreNames.LinearGaugeRotationNode);
			bool vertical = gauge.Style == DashboardGaugeStyle.Vertical;
			rotationNode.Angle = vertical ? 0f : 90f;
			rotationNode.Location = vertical ? PointF2D.Empty : new PointF2D(250f, 0f);
			rotationNode.Composite.AddRange(new IElement<IRenderableElement>[] { linearScaleRangeBar, linearScale });
			gauge.Model.Composite.Add(rotationNode);
			gauge.Elements.AddRange(new ISerizalizeableElement[] { linearScale, linearScaleRangeBar });
			if(gauge.ShowMarker) {
				LinearScaleMarker linearScaleMarker = new LinearScaleMarker() { Name = "LinearScaleMarker" };
				gauge.Marker = linearScaleMarker;
				linearScaleMarker.LinearScale = linearScale;
				linearScaleMarker.ZOrder = -100;
				rotationNode.Composite.Add(linearScaleMarker);
				gauge.Elements.Add(linearScaleMarker);
			}
		}
	}
}
