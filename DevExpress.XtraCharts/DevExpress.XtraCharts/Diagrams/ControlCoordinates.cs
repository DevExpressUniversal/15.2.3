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
using System.Drawing;
namespace DevExpress.XtraCharts {
	public enum ControlCoordinatesVisibility {
		Visible,
		Hidden,
		Undefined
	}
	public class ControlCoordinates {
		readonly XYDiagramPaneBase pane;
		readonly ControlCoordinatesVisibility visibility;
		readonly Point point;
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ControlCoordinatesPane")]
#endif
		public XYDiagramPaneBase Pane { get { return pane; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ControlCoordinatesVisibility")]
#endif
		public ControlCoordinatesVisibility Visibility { get { return visibility; } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("ControlCoordinatesPoint")]
#endif
		public Point Point { get { return point; } }
		internal ControlCoordinates(XYDiagramPaneBase pane, ControlCoordinatesVisibility visibility, Point point) {
			this.pane = pane;
			this.visibility = visibility;
			this.point = point;
		}
		internal ControlCoordinates(XYDiagramPaneBase pane, ControlCoordinatesVisibility visibility) : this(pane, visibility, Point.Empty) {
		}
		internal ControlCoordinates(XYDiagramPaneBase pane) : this(pane, ControlCoordinatesVisibility.Undefined) {
		}
		internal ControlCoordinates() : this(null) {
		}
	}
}
