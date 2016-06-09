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
using System.Collections.Generic;
using System.Text;
using DevExpress.ExpressApp.Chart;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraCharts.Web;
using System.Web.UI.WebControls;
using DevExpress.XtraCharts;
using System.Drawing;
using DevExpress.XtraCharts.Native;
using DevExpress.ExpressApp.Web.Editors;
namespace DevExpress.ExpressApp.Chart.Web {
	[DevExpress.ExpressApp.Editors.PropertyEditor(typeof(ISparklineProvider), true)]
	public class SparklinePropertyEditor : WebPropertyEditor {
		private WebChartControl chartControl;
		protected virtual ChartControlContainer CreateChartControlContainerCore() {
			WebChartControl chartControlCore = new WebChartControlWithEmptyArguments();
			return new WebChartControlContainer(chartControlCore);
		}
		protected ISparklineProvider SparklineProvider {
			get { return PropertyValue as ISparklineProvider; }
		}
		private void SetupSparkline() {
			if(chartControl != null && SparklineProvider != null) {
				chartControl.BeginInit();
				chartControl.Width = new Unit(150);
				chartControl.Height = new Unit(50);
				SparklineHelper.Setup(chartControl);
				chartControl.BorderOptions.Visibility = DevExpress.Utils.DefaultBoolean.False;
				chartControl.EndInit();
				#region Series
				chartControl.DataSource = SparklineProvider.DataSource;
				((IChartContainer)chartControl).Chart.DataContainer.DataSource = SparklineProvider.DataSource;
				if(SparklineProvider.SuppressedSeries != null) {
					foreach(string suppressedSeries in SparklineProvider.SuppressedSeries) {
						if(chartControl.Series[suppressedSeries] != null) {
							chartControl.Series[suppressedSeries].Visible = false;
						}
					}
				}
				#endregion
			}
		}
		private WebControl CreateChartControlCore() {
			chartControl = new WebChartControlWithEmptyArguments();
			return chartControl;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				chartControl = null;
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			return CreateChartControlCore();
		}
		protected override WebControl CreateViewModeControlCore() {
			return CreateChartControlCore();
		}
		protected override void ReadEditModeValueCore() {
			SetupSparkline();
		}
		protected override void ReadViewModeValueCore() {
			SetupSparkline();
		}
		protected override object GetControlValueCore() {
			if(chartControl != null) {
				return chartControl.DataSource;
			}
			return null;
		}
		public SparklinePropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			base.BreakLinksToControl(unwireEventsOnly);
			chartControl = null;
		}
	}
}
