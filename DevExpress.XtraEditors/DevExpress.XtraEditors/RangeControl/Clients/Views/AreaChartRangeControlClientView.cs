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
using DevExpress.Sparkline;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraEditors {
	public class AreaChartRangeControlClientView : LineChartRangeControlClientView {
		const byte defaultAreaOpacity = 135;
		const bool defaultEnableAntialiasing = true;
		const int defaultLineWidth = 1;
		internal new AreaSparklineView SparklineView {
			get { return (AreaSparklineView)base.SparklineView; }
		}
		[
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraEditors.AreaChartRangeControlClientView.AreaOpacity"),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("AreaChartRangeControlClientViewAreaOpacity"),
#endif
		]
		public byte AreaOpacity {
			get { return SparklineView.AreaOpacity; }
			set {
				if (SparklineView.AreaOpacity != value) {
					SparklineView.AreaOpacity = value;
					RaiseChanged();
				}
			}
		}
		public AreaChartRangeControlClientView() : base(new AreaSparklineView()) {
			AreaSparklineView sparklineView = SparklineView;
			sparklineView.EndPointMarkerSize = 0;
			sparklineView.HighlightNegativePoints = false;
			sparklineView.MaxPointMarkerSize = 0;
			sparklineView.MinPointMarkerSize = 0;
			sparklineView.NegativePointMarkerSize = 0;
			sparklineView.StartPointMarkerSize = 0;
			sparklineView.AreaOpacity = defaultAreaOpacity;
			sparklineView.EnableAntialiasing = defaultEnableAntialiasing;
			sparklineView.LineWidth = defaultLineWidth;
		}
		#region ShouldSerialize & Reset
		protected internal override bool ShouldSerialize() {
			return base.ShouldSerialize() || 
				ShouldSerializeEnableAntialiasing() ||
				ShouldSerializeAreaOpacity() ||
				ShouldSerializeLineWidth();
		}
		bool ShouldSerializeAreaOpacity() {
			return AreaOpacity != defaultAreaOpacity;
		}
		bool ShouldSerializeEnableAntialiasing() {
			return EnableAntialiasing != defaultEnableAntialiasing;
		}
		bool ShouldSerializeLineWidth() {
			return (LineWidth != defaultLineWidth);
		}
		void ResetAreaOpacity() {
			AreaOpacity = defaultAreaOpacity;
		}
		void ResetEnableAntialiasing() {
			EnableAntialiasing = defaultEnableAntialiasing;
		}
		void ResetLineWidth() {
			LineWidth = defaultLineWidth;
		}
		#endregion
		protected override void Assign(ChartRangeControlClientView clone) {
			base.Assign(clone);
			AreaChartRangeControlClientView cloneView = (AreaChartRangeControlClientView)clone;
			cloneView.AreaOpacity = AreaOpacity;
			cloneView.EnableAntialiasing = EnableAntialiasing;
		}
		protected override ChartRangeControlClientView CreateInstance() {
			return new AreaChartRangeControlClientView();
		}
	}
}
