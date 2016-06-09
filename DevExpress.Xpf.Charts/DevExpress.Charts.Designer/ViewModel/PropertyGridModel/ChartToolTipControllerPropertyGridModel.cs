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

using System;
using System.Windows;
using DevExpress.Xpf.Charts;
namespace DevExpress.Charts.Designer.Native {
	public class WpfChartToolTipControllerPropertyGridModel : WpfChartElementPropertyGridModelBase {
		readonly ChartToolTipController toolTipController;
		protected internal ChartToolTipController ToolTipController { get { return toolTipController; } }
		public TimeSpan InitialDelay {
			get { return ToolTipController.InitialDelay; }
			set { SetProperty("InitialDelay", value); }
		}
		public TimeSpan AutoPopDelay {
			get { return ToolTipController.AutoPopDelay; }
			set { SetProperty("AutoPopDelay", value); }
		}
		public ToolTipOpenMode OpenMode {
			get { return ToolTipController.OpenMode; }
			set { SetProperty("OpenMode", value); }
		}
		public bool ShowBeak {
			get { return ToolTipController.ShowBeak; }
			set { SetProperty("ShowBeak", value); }
		}
		public bool CloseOnClick {
			get { return ToolTipController.CloseOnClick; }
			set { SetProperty("CloseOnClick", value); }
		}
		public bool ShowShadow {
			get { return ToolTipController.ShowShadow; }
			set { SetProperty("ShowShadow", value); }
		}
		public Thickness ContentMargin {
			get { return ToolTipController.ContentMargin; }
			set { SetProperty("ContentMargin", value); }
		}
		public WpfChartToolTipControllerPropertyGridModel() : this(null, null, string.Empty) { 
		}
		public WpfChartToolTipControllerPropertyGridModel(WpfChartModel chartModel, ChartToolTipController toolTipController, string propertyPath) 
			: base(chartModel, propertyPath) {
				this.toolTipController = toolTipController;
		}
	}
}
