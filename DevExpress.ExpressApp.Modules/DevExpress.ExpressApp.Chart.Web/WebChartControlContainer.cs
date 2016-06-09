#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using DevExpress.ExpressApp.Web.Controls;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Web;
namespace DevExpress.ExpressApp.Chart.Web {
	public class WebChartControlContainer : ChartControlContainer {
		private ResizableControlContainer resizebleControlContainer;
		private void resizebleControlContainer_CallBack(object sender, EventArgs e) {
			if(CallBack != null) {
				CallBack(this, EventArgs.Empty);
			}
		}
		public WebChartControlContainer(IChartContainer chartContainer)
			: base(chartContainer) {
			resizebleControlContainer = new ResizableControlContainer((WebChartControl)ChartContainer);
			resizebleControlContainer.MinWidth = ChartAspNetModule.DefaultChartPreferredWidth;
			resizebleControlContainer.MinHeight = ChartAspNetModule.DefaultChartPreferredHeight;
			resizebleControlContainer.Callback += resizebleControlContainer_CallBack;
		}
		public override object Control {
			get { return resizebleControlContainer.Container; }
		}
		public int PreferredWidth {
			get { return resizebleControlContainer.MinWidth; }
			set { resizebleControlContainer.MinWidth = value; }
		}
		public int PreferredHeight {
			get { return resizebleControlContainer.MinHeight; }
			set { resizebleControlContainer.MinHeight = value; }
		}
		public override void Dispose() {
			base.Dispose();
			if(resizebleControlContainer != null) {
				resizebleControlContainer.Dispose();
				resizebleControlContainer = null;
			}
			CallBack = null;
		}
		public event EventHandler<EventArgs> CallBack;
	}
}
