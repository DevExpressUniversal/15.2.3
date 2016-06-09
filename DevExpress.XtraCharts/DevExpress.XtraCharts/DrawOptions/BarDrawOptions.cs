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
namespace DevExpress.XtraCharts {
	public class BarDrawOptions : DrawOptions {
		RectangularBorder border;
		RectangleFillStyle fillStyle;
		Shadow shadow;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BarDrawOptionsBorder")]
#endif
		public RectangularBorder Border { get { return this.border; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BarDrawOptionsFillStyle")]
#endif
		public RectangleFillStyle FillStyle { get { return this.fillStyle; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("BarDrawOptionsShadow")]
#endif
		public Shadow Shadow { get { return this.shadow; } }
		internal BarDrawOptions(BarSeriesView view) : base(view) {
			this.border = (RectangularBorder)view.Border.Clone();
			this.fillStyle = (RectangleFillStyle)view.ActualFillStyle.Clone();
			this.shadow = (Shadow)view.Shadow.Clone();
		}
		protected BarDrawOptions() {
		}
		protected override DrawOptions CreateInstance() {
			return new BarDrawOptions();
		}
		protected override void DeepClone(object obj) {
			base.DeepClone(obj);
			BarDrawOptions drawOptions = obj as BarDrawOptions;
			if(drawOptions != null) {
				this.border = (RectangularBorder)drawOptions.border.Clone();
				this.fillStyle = (RectangleFillStyle)drawOptions.fillStyle.Clone();
				this.shadow = (Shadow)drawOptions.shadow.Clone();
			}
		}
	}
}
