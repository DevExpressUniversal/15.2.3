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
using DevExpress.ExpressApp.Editors;
namespace DevExpress.ExpressApp.Kpi {
	public class KpiGaugeDataSource : IValueWithTrendProvider, IZonesProvider, IMinMaxProvider, ISettingsProvider, IDisposable {
		private IKpiInstance kpiInstance;
		public KpiGaugeDataSource(IKpiInstance kpiInstance) {
			this.kpiInstance = kpiInstance;
		}
		public float Value {
			get { return kpiInstance.Current; }
		}
		public Trend? Trend {
			get { return kpiInstance.Trend; }
		}
		public float GreenZone {
			get { return kpiInstance.GetKpiDefinition().GreenZone; }
		}
		public float RedZone {
			get { return kpiInstance.GetKpiDefinition().RedZone; }
		}
		public float MinValue {
			get {
				float result;
				if(RedZone != 0 || GreenZone != 0) {
					float delta = (RedZone + GreenZone) / 4;
					result = RedZone > GreenZone ? GreenZone - delta : RedZone - delta;
					if(Value < result) {
						return Value;
					}
					return result < 0 ? 0 : result;
				}
				result = kpiInstance.GetHistoryItems().Count > 0 ? float.MaxValue : 0;
				foreach(IKpiHistoryItem item in kpiInstance.GetHistoryItems()) {
					if(item.Value < result) {
						result = item.Value;
					}
				}
				return result;
			}
		}
		public float MaxValue {
			get {
				float result;
				if(RedZone != 0 || GreenZone != 0) {
					float delta = (RedZone + GreenZone) / 4;
					result =  RedZone > GreenZone ? RedZone + delta : GreenZone + delta;
					if(Value > result) {
						return Value;
					}
					return result;
				}
				result = kpiInstance.GetHistoryItems().Count > 0 ? float.MinValue : 100;
				foreach(IKpiHistoryItem item in kpiInstance.GetHistoryItems()) {
					if(item.Value > result) {
						result = item.Value;
					}
				}
				return result;
			}
		}
		public string Settings {
			get {
				if(kpiInstance is ISettingsProvider) {
					return ((ISettingsProvider)kpiInstance).Settings;
				}
				return null;
			}
			set {
				if(kpiInstance is ISettingsProvider) {
					((ISettingsProvider)kpiInstance).Settings = value;
				}
			}
		}
		public void Dispose() {
			kpiInstance = null;
		}
	}
}
