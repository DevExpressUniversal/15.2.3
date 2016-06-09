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
using DevExpress.ExpressApp.Localization;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.ExpressApp.Utils;
using System.Resources;
using DevExpress.ExpressApp.SystemModule;
using System.Reflection;
using DevExpress.ExpressApp.Model;
namespace DevExpress.ExpressApp.PivotChart {
	[System.ComponentModel.DisplayName("Pivot Grid Control")]
	public class PivotGridLocalizer : XtraPivotGrid.Localization.PivotGridResLocalizer, IXafResourceLocalizer {
		ControlResourcesLocalizerLogic logic;
		protected override void CreateResourceManager() {
			logic = new ControlResourcesLocalizerLogic(this);
		}
		protected override ResourceManager Manager {
			get {
				return logic.Manager;
			}
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			SessionLocalizerProvider<PivotGridLocalizer, DevExpress.XtraPivotGrid.Localization.PivotGridStringId>.XafActiveLocalizer = this;
		}
		public void Reset() {
			logic.Reset();
		}
		#endregion
		#region IXafResourceManagerParametersProvider Members
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public IXafResourceManagerParameters XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = new XafResourceManagerParameters(
						"DevExpress.XtraPivotGrid",
						"DevExpress.XtraPivotGrid.LocalizationRes",
						"PivotGridStringId.",
						typeof(PivotGridData).Assembly
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	[System.ComponentModel.DisplayName("Chart control")]
	public class ChartLocalizer : XtraCharts.Localization.ChartResLocalizer, IXafResourceLocalizer {
		ControlResourcesLocalizerLogic logic;
		protected override void CreateResourceManager() {
			logic = new ControlResourcesLocalizerLogic(this);
		}
		protected override ResourceManager Manager {
			get {
				return logic.Manager;
			}
		}
		#region IXafResourceLocalizer Members
		public void Setup(IModelApplication applicationNode) {
			logic.Setup(applicationNode);
			SessionLocalizerProvider<ChartLocalizer, DevExpress.XtraCharts.Localization.ChartStringId>.XafActiveLocalizer = this;
		}
		public void Reset() {
			logic.Reset();
		}
		#endregion
		#region IXafResourceManagerParametersProvider Members
		private IXafResourceManagerParameters xafResourceManagerParameters;
		public IXafResourceManagerParameters XafResourceManagerParameters {
			get {
				if(xafResourceManagerParameters == null) {
					xafResourceManagerParameters = new XafResourceManagerParameters(
						"DevExpress.XtraCharts",
						"DevExpress.XtraCharts.LocalizationRes",
						"ChartStringId.",
						typeof(DevExpress.XtraCharts.Native.Chart).Assembly
						);
				}
				return xafResourceManagerParameters;
			}
		}
		#endregion
	}
	public enum AnalysisControlId {
		ChartTabCaption,
		ChartTypeCaption,
		ChartWizardText,
		IncorrectChartTypeErrorMessage,
		PivotTabCaption,
		PrintChartText,
		ShowColumnGrandTotalText,
		ShowRowGrandTotalText
	}
	[System.ComponentModel.DisplayName("Analysis Module")]
	public class AnalysisControlLocalizer : XafResourceLocalizer<AnalysisControlId> {
		private static AnalysisControlLocalizer activeLocalizer;
		static AnalysisControlLocalizer() {
			activeLocalizer = new AnalysisControlLocalizer();
		}
		protected override IXafResourceManagerParameters GetXafResourceManagerParameters() {
			return new XafResourceManagerParameters(
				"AnalysisControl",
				"DevExpress.ExpressApp.PivotChart.LocalizationResources",
				String.Empty,
				GetType().Assembly
				);
		}
		public static AnalysisControlLocalizer Active {
			get { return activeLocalizer; }
			set { activeLocalizer = value; }
		}
		public override void Activate() {
			Active = this;
		}
	}
}
