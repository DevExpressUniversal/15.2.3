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
using System.Collections.Generic;
using DevExpress.Web;
namespace DevExpress.XtraCharts.Web {
	public class ChartClientSideEvents : CallbackClientSideEventsBase {
		internal const string ClientClickEventHandler = "ASPx.chartClick(event, '{0}')";
		internal const string ClientMouseMoveEventHandler = "ASPx.chartMouseMove(event, '{0}')";
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartClientSideEventsObjectHotTracked"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string ObjectHotTracked {
			get { return GetEventHandler("ObjectHotTracked"); }
			set { SetEventHandler("ObjectHotTracked", value); }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartClientSideEventsCustomDrawCrosshair"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string CustomDrawCrosshair {
			get { return GetEventHandler("CustomDrawCrosshair"); }
			set { SetEventHandler("CustomDrawCrosshair", value); }
		}
		[
#if !SL
	DevExpressXtraChartsWebLocalizedDescription("ChartClientSideEventsObjectSelected"),
#endif
		DefaultValue(""),
		NotifyParentProperty(true),
		Editor("DevExpress.Web.Design.CommonDesignerEditor, " + AssemblyInfo.SRAssemblyWebDesignFull, typeof(System.Drawing.Design.UITypeEditor))
		]
		public string ObjectSelected {
			get { return GetEventHandler("ObjectSelected"); }
			set { SetEventHandler("ObjectSelected", value); }
		}		
		public ChartClientSideEvents()
			: base() {
		}
		protected override void AddEventNames(List<string> names) {
			base.AddEventNames(names);
			names.Add("ObjectHotTracked");
			names.Add("ObjectSelected");
			names.Add("CustomDrawCrosshair");
		}
	}
}
