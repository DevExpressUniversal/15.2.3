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
using System.Drawing;
using System.Collections;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Win.Design.Base;
using DevExpress.XtraGauges.Win.Gauges.State;
namespace DevExpress.XtraGauges.Win.Design {
	public class StateIndicatorGaugeDesigner : GaugeDesigner {
		protected override bool AllowEditInherited { get { return false; } }
		protected StateIndicatorGauge StateIndicatorGauge {
			get { return base.Component as StateIndicatorGauge; }
		}
		public override void InitializeNewComponent(IDictionary defaultValues) {
			base.InitializeNewComponent(defaultValues);
		}
		public override ICollection AssociatedComponents {
			get {
				ArrayList components = new ArrayList(base.AssociatedComponents);
				if(!Gauge.IsDisposing) {
					components.AddRange(StateIndicatorGauge.Indicators);
					components.AddRange(StateIndicatorGauge.Labels);
				}
				return components;
			}
		}
	}
	public class StateIndicatorGaugeKind : GaugeKind {
		public override string GetName() {
			return "StateIndicator Gauge";
		}
		public override Image GetImage() {
			return DesignHelper.GetImageFromDesignResources("StateIndicatorGauge.png");
		}
		public override IGauge CreateInstance() {
			return new StateIndicatorGauge();
		}
	}
}
