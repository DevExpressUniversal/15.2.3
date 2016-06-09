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
using System.Windows.Input;
using DevExpress.Xpf.Charts;
using DevExpress.Xpf.Charts.Native;
namespace DevExpress.Charts.Designer.Native {
	public abstract class WpfChartToolTipPositionPropertyGridModel : WpfChartElementPropertyGridModelBase {
		readonly ToolTipPosition toolTipPosition;
		protected internal ToolTipPosition ToolTipPosition { get { return toolTipPosition; } }
		protected internal abstract ToolTipPosition NewToolTipPosition { get; }
		[Category(Categories.Layout)]
		public Point Offset {
			get { return ToolTipPosition.Offset; }
			set { SetProperty("Offset", value); }
		}
		public WpfChartToolTipPositionPropertyGridModel() : this(null, null, string.Empty) {
		}
		public WpfChartToolTipPositionPropertyGridModel(WpfChartModel chartModel, ToolTipPosition toolTipPosition, string propertyPath)
			: base(chartModel, propertyPath) {
				this.toolTipPosition = toolTipPosition;
		}
	}
	public class WpfChartToolTipFreePositionPropertyGridModel : WpfChartToolTipPositionPropertyGridModel {
		new protected internal ToolTipFreePosition ToolTipPosition { get { return base.ToolTipPosition as ToolTipFreePosition; } }
		protected internal override ToolTipPosition NewToolTipPosition { get { return new ToolTipFreePosition(); } }
		[Category(Categories.Layout)]
		public DockCorner DockCorner {
			get { return ToolTipPosition.DockCorner; }
			set { SetProperty("DockCorner", value); }
		}
		public WpfChartToolTipFreePositionPropertyGridModel()
			: base() {
		}
		public WpfChartToolTipFreePositionPropertyGridModel(WpfChartModel chartModel, ToolTipPosition toolTipPosition, string propertyPath)
			: base(chartModel, toolTipPosition, propertyPath) {
		}		
	}
	public abstract class WpfChartToolTipPositionWithLocationPropertyGridModel : WpfChartToolTipPositionPropertyGridModel {
		new protected internal ToolTipPositionWithLocation ToolTipPosition { get { return base.ToolTipPosition as ToolTipPositionWithLocation; } }
		protected internal override ToolTipPosition NewToolTipPosition { get { return null; } }
		[Category(Categories.Layout)]
		public ToolTipLocation Location {
			get { return ToolTipPosition.Location; }
			set { SetProperty("Location", value); }
		}
		public WpfChartToolTipPositionWithLocationPropertyGridModel() : base() { 
		}
		public WpfChartToolTipPositionWithLocationPropertyGridModel(WpfChartModel chartModel, ToolTipPosition toolTipPosition, string propertyPath)
			: base(chartModel, toolTipPosition, propertyPath) {
		}
	}
	public class WpfChartToolTipMousePositionPropertyGridModel : WpfChartToolTipPositionWithLocationPropertyGridModel {
		new protected internal ToolTipMousePosition ToolTipPosition { get { return base.ToolTipPosition as ToolTipMousePosition; } }
		protected internal override ToolTipPosition NewToolTipPosition { get { return new ToolTipMousePosition(); } }	   
		public WpfChartToolTipMousePositionPropertyGridModel() : base() { 
		}
		public WpfChartToolTipMousePositionPropertyGridModel(WpfChartModel chartModel, ToolTipPosition toolTipPosition, string propertyPath)
			: base(chartModel, toolTipPosition, propertyPath) {
		}
	}
	public class WpfChartToolTipRelativePositionPropertyGridModel : WpfChartToolTipPositionWithLocationPropertyGridModel {
		new protected internal ToolTipRelativePosition ToolTipPosition { get { return base.ToolTipPosition as ToolTipRelativePosition; } }
		protected internal override ToolTipPosition NewToolTipPosition { get { return new ToolTipRelativePosition(); } }
		public WpfChartToolTipRelativePositionPropertyGridModel()
			: base() {
		}
		public WpfChartToolTipRelativePositionPropertyGridModel(WpfChartModel chartModel, ToolTipPosition toolTipPosition, string propertyPath)
			: base(chartModel, toolTipPosition, propertyPath) {
		}
	}
}
