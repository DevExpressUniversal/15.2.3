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

using DevExpress.Charts.Native;
using System;
using System.Collections.Generic;
namespace DevExpress.Xpf.Charts {
	public class LineStackedSeries2D : LineSeries2D, IStackedView {
		protected override bool NeedSeriesInteraction { get { return true; } }
		protected override Type PointInterfaceType { get { return typeof(IFullStackedPoint); } }
		public LineStackedSeries2D() {
			DefaultStyleKey = typeof(LineStackedSeries2D);
		}
		protected override Series CreateObjectForClone() {
			return new LineStackedSeries2D();
		}
		protected override SeriesContainer CreateContainer() {
			return new StackedInteractionContainer(this, true);
		}
		protected override GeometryStripCreator CreateStripCreator() {
			return new StackedLineGeometryStripCreator();
		}
		protected override double GetMaxValue(RefinedPoint refinedPoint) {
			return ((IStackedPoint)refinedPoint).MaxValue;
		}
		protected override IEnumerable<double> GetCrosshairValues(RefinedPoint refinedPoint) {
			yield return ((IStackedPoint)refinedPoint).TotalValue;
		}
	}
}
