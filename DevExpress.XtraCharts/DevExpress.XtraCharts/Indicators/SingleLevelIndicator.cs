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

using System.Collections.Generic;
using System.ComponentModel;
using DevExpress.Charts.Native;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class SingleLevelIndicator : Indicator {
		const ValueLevel DefaultValueLevel = ValueLevel.Value;
		ValueLevel valueLevel = DefaultValueLevel;
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SingleLevelIndicatorValueLevel"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SingleLevelIndicator.ValueLevel"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public ValueLevel ValueLevel {
			get { return valueLevel; }
			set {
				if (value != valueLevel) {
					if (!Loading) {
						XYDiagram2DSeriesViewBase view = View;
						if (view != null)
							view.CheckValueLevel(value);
					}
					SendNotification(new ElementWillChangeNotification(this));
					valueLevel = value;
					RaiseControlChanged();
				}
			}
		}
		protected SingleLevelIndicator(string name, ValueLevel valueLevel) : base(name) {
			this.valueLevel = valueLevel;
		}
		protected SingleLevelIndicator(string name) : this(name, DefaultValueLevel) {
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeValueLevel() {
			return valueLevel != DefaultValueLevel;
		}
		void ResetValueLevel() {
			ValueLevel = DefaultValueLevel;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "ValueLevel" ? ShouldSerializeValueLevel() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected internal override void Validate(XYDiagram2DSeriesViewBase view, IRefinedSeries refinedSeries) {
			try {
				view.CheckValueLevel(valueLevel);
			}
			catch {
				valueLevel = view.DefaultValueLevel;
			}
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SingleLevelIndicator indicator = obj as SingleLevelIndicator;
			if (indicator != null)
				valueLevel = indicator.valueLevel;
		}
	}
}
