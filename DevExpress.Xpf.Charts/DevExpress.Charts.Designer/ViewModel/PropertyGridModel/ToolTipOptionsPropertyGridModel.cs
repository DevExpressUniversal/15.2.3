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
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartToolTipOptionsPropertyGridModel : WpfChartElementPropertyGridModelBase {
		readonly ToolTipOptions toolTipOptions;
		WpfChartToolTipPositionPropertyGridModel toolTipPosition;
		protected internal ToolTipOptions ToolTipOptions { get { return toolTipOptions; } }
		[Category(Categories.Behavior)]
		public bool ShowForSeries {
			get { return ToolTipOptions.ShowForSeries; }
			set { SetProperty("ShowForSeries", value); }
		}
		[Category(Categories.Behavior)]
		public bool ShowForPoints {
			get { return ToolTipOptions.ShowForPoints; }
			set { SetProperty("ShowForPoints", value); }
		}
		[
		DefaultValue(null),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Category(Categories.Behavior)
		]
		public WpfChartToolTipPositionPropertyGridModel ToolTipPosition {
			get { return toolTipPosition; }
			set { SetProperty("ToolTipPosition", value.NewToolTipPosition); }
		}
		public WpfChartToolTipOptionsPropertyGridModel() : this(null, null, string.Empty) {
		}
		public WpfChartToolTipOptionsPropertyGridModel(WpfChartModel chartModel, ToolTipOptions toolTipOptions, string propertyPath)
			: base(chartModel, propertyPath) {
			this.toolTipOptions = toolTipOptions;
			UpdateInternal();
		}
		protected override void UpdateInternal() {
			base.UpdateInternal();
			if (ToolTipOptions == null)
				return;
			if (ToolTipOptions.ToolTipPosition != null) {
				if (toolTipPosition != null && ToolTipOptions.ToolTipPosition != toolTipPosition.ToolTipPosition || toolTipPosition == null)
					if (ToolTipOptions.ToolTipPosition is ToolTipFreePosition)
						toolTipPosition = new WpfChartToolTipFreePositionPropertyGridModel(ChartModel, ToolTipOptions.ToolTipPosition, "ToolTipOptions.ToolTipPosition.");
					else if (ToolTipOptions.ToolTipPosition is ToolTipMousePosition)
						toolTipPosition = new WpfChartToolTipMousePositionPropertyGridModel(ChartModel, ToolTipOptions.ToolTipPosition, "ToolTipOptions.ToolTipPosition.");
					else if (ToolTipOptions.ToolTipPosition is ToolTipRelativePosition)
						toolTipPosition = new WpfChartToolTipRelativePositionPropertyGridModel(ChartModel, ToolTipOptions.ToolTipPosition, "ToolTipOptions.ToolTipPosition.");
			}
			else
				toolTipPosition = null;
		}
	}
}
