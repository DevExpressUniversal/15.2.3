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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class SubsetBasedIndicator : SingleLevelIndicator {
		int pointsCount;
		protected virtual int DefaultPointsCount { get { return 10; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("SubsetBasedIndicatorPointsCount"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.SubsetBasedIndicator.PointsCount"),
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public int PointsCount {
			get { return pointsCount; }
			set {
				if (value != pointsCount) {
					if (value < 2)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPointsCount));
					SendNotification(new ElementWillChangeNotification(this));
					pointsCount = value;
					RaiseControlChanged();
				}
			}
		}
		protected SubsetBasedIndicator(string name, ValueLevel valueLevel) : base(name, valueLevel) {
			pointsCount = DefaultPointsCount;
		}
		protected SubsetBasedIndicator(string name) : base(name) {
			pointsCount = DefaultPointsCount;
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializePointsCount() {
			return pointsCount != DefaultPointsCount;
		}
		void ResetPointsCount() {
			PointsCount = DefaultPointsCount;
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			return propertyName == "PointsCount" ? ShouldSerializePointsCount() : base.XtraShouldSerialize(propertyName);
		}
		#endregion
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			SubsetBasedIndicator indicator = obj as SubsetBasedIndicator;
			if (indicator != null)
				pointsCount = indicator.pointsCount;
		}
	}
}
