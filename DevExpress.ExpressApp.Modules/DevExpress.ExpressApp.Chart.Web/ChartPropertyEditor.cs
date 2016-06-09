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
using DevExpress.XtraCharts.Web;
using System.Web.UI.WebControls;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Web.Editors;
namespace DevExpress.ExpressApp.Chart.Web {
	[DevExpress.ExpressApp.Editors.PropertyEditor(typeof(IChartDataSourceProvider), true)]
	public class ChartPropertyEditor : WebPropertyEditor {
		private ChartControlContainer chartControlContainer;
		private WebChartControl chartControlCore;
		public ChartPropertyEditor(Type objectType, IModelMemberViewItem info) : base(objectType, info) { }
		protected virtual ChartControlContainer CreateChartControlContainerCore() {
			chartControlCore = new WebChartControlWithEmptyArguments();
			chartControlCore.ID = "Chart";
			return new WebChartControlContainer(chartControlCore);
		}
		private void SetEditValue() {
			if(chartControlContainer != null) {
				chartControlContainer.EditValue = PropertyValue as IChartDataSourceProvider;
			}
		}
		private WebControl CreateChartControlCore() {
			chartControlContainer = CreateChartControlContainerCore();
			return chartControlContainer.Control as WebControl;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			if(disposing) {
				DisposeChartContainer();
				chartControlCore = null;
			}
		}
		public override void BreakLinksToControl(bool unwireEventsOnly) {
			base.BreakLinksToControl(unwireEventsOnly);
			if(!unwireEventsOnly) {
				DisposeChartContainer();
			}
		}
		private void DisposeChartContainer() {
			if(chartControlContainer != null) {
				chartControlContainer.Dispose();
				chartControlContainer = null;
			}
		}
		protected override WebControl CreateEditModeControlCore() {
			return CreateChartControlCore();
		}
		protected override WebControl CreateViewModeControlCore() {
			return CreateChartControlCore();
		}
		protected override void ReadEditModeValueCore() {
			SetEditValue();
		}
		protected override void ReadViewModeValueCore() {
			SetEditValue();
		}
		protected override object GetControlValueCore() {
			if(chartControlContainer != null) {
				return chartControlContainer.EditValue;
			}
			return null;
		}
		public new WebChartControlContainer Control {
			get { return (WebChartControlContainer)base.Control; }
		}
		public WebChartControl ChartControl {
			get { return chartControlCore; }
		}
	}
}
