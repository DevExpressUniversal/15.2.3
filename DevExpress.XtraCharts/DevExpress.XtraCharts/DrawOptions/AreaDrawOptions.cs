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
	public class AreaDrawOptions : LineDrawOptions {
		IAreaSeriesView view;
		CustomBorder border;
		PolygonFillStyle fillStyle;
		new LineStyle LineStyle { get { return null; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AreaDrawOptionsBorder")]
#endif
		public CustomBorder Border { get { return this.border; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AreaDrawOptionsFillStyle")]
#endif
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		internal AreaDrawOptions(AreaSeriesViewBase view) : base(view) {
			Initialize(view);
		}
		internal AreaDrawOptions(RadarAreaSeriesView view) : base(view) {
			Initialize(view);
		}
		void Initialize(IAreaSeriesView view) {
			this.view = view;
			if(view.Border != null)
				border = (CustomBorder)view.Border.Clone();
			fillStyle = (PolygonFillStyle)view.ActualFillStyle.Clone();
		}
		protected AreaDrawOptions() {
		}
		protected override DrawOptions CreateInstance() {
			return new AreaDrawOptions();
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			AreaDrawOptions drawOptions = obj as AreaDrawOptions;
			if(drawOptions != null) {
				if(drawOptions.Border != null)
					border = (CustomBorder)drawOptions.border.Clone();
				fillStyle = (PolygonFillStyle)drawOptions.fillStyle.Clone();
			}
		}
	}
}
