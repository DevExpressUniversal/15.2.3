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
using System.Windows;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartCrosshairLabelPositionPropertyGridModel : WpfChartElementPropertyGridModelBase {
		readonly CrosshairLabelPosition crosshairLabelPosition;
		protected internal abstract CrosshairLabelPosition NewCrosshairLabelPosition { get; }
		protected internal CrosshairLabelPosition CrosshairLabelPosition { get { return crosshairLabelPosition; } }
		[Category(Categories.Layout)]
		public Point Offset {
			get { return CrosshairLabelPosition.Offset; }
			set { SetProperty("Offset", value); }
		}
		public WpfChartCrosshairLabelPositionPropertyGridModel() : this(null, null, string.Empty) {
		}
		public WpfChartCrosshairLabelPositionPropertyGridModel(WpfChartModel chartModel, CrosshairLabelPosition crosshairLabelPosition, string propertyPath)
			: base(chartModel, propertyPath) {
				this.crosshairLabelPosition = crosshairLabelPosition;
		}
	}
	public class WpfChartCrosshairFreePositionPropertyGridModel : WpfChartCrosshairLabelPositionPropertyGridModel {
		new protected internal CrosshairFreePosition CrosshairLabelPosition { get { return base.CrosshairLabelPosition as CrosshairFreePosition; } }
		protected internal override CrosshairLabelPosition NewCrosshairLabelPosition { get { return new CrosshairFreePosition(); } }
		[Category(Categories.Layout)]
		public DockCorner DockCorner {
			get { return CrosshairLabelPosition.DockCorner; }
			set { SetProperty("DockCorner", value); }
		}
		public WpfChartCrosshairFreePositionPropertyGridModel() : base() {
		}
		public WpfChartCrosshairFreePositionPropertyGridModel(WpfChartModel chartModel, CrosshairLabelPosition crosshairLabelPosition, string propertyPath)
			: base(chartModel, crosshairLabelPosition, propertyPath) {
		}
	}
	public class WpfChartCrosshairMousePositionPropertyGridModel : WpfChartCrosshairLabelPositionPropertyGridModel {
		new protected internal CrosshairMousePosition CrosshairLabelPosition { get { return base.CrosshairLabelPosition as CrosshairMousePosition; } }
		protected internal override CrosshairLabelPosition NewCrosshairLabelPosition { get { return new CrosshairMousePosition(); } }
		public WpfChartCrosshairMousePositionPropertyGridModel() : base() { 
		}
		public WpfChartCrosshairMousePositionPropertyGridModel(WpfChartModel chartModel, CrosshairLabelPosition crosshairLabelPosition, string propertyPath)
			: base(chartModel, crosshairLabelPosition, propertyPath) {
		}
	}
}
