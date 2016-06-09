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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public class PointDrawOptionsBase : DrawOptions {
		MarkerBase marker;
		Shadow shadow;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PointDrawOptionsBaseMarker")]
#endif
		public MarkerBase Marker { get { return marker; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("PointDrawOptionsBaseShadow")]
#endif
		public Shadow Shadow { get { return shadow; } }
		internal PointDrawOptionsBase(PointSeriesViewBase view) : base(view) {
			marker = CreateMarkerFromPattern(view.Marker);
			shadow = (Shadow)view.Shadow.Clone();
		}
		internal PointDrawOptionsBase(RadarPointSeriesView view) : base(view) {
			marker = CreateMarkerFromPattern(view.PointMarkerOptions);
			shadow = (Shadow)view.Shadow.Clone();
		}
		protected PointDrawOptionsBase() {
		}
		protected MarkerBase CreateMarkerFromPattern(MarkerBase patternMarker) {
			MarkerBase marker = (MarkerBase)patternMarker.Clone();
			if (patternMarker.FillStyle.FillMode == FillMode.Empty)
				marker.FillStyle.Assign(CommonUtils.GetActualAppearance(patternMarker).MarkerAppearance.FillStyle);
			return marker;
		}
		protected override DrawOptions CreateInstance() {
			return new PointDrawOptionsBase();
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			PointDrawOptionsBase drawOptions = obj as PointDrawOptionsBase;
			if (drawOptions != null) {
				marker = (MarkerBase)drawOptions.marker.Clone();
				shadow = (Shadow)drawOptions.shadow.Clone();
			}
		}
	}
}
