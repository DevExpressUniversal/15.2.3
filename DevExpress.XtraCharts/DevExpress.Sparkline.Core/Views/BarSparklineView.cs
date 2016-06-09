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
using DevExpress.Sparkline.Core;
using DevExpress.Sparkline.Localization;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.Sparkline {
	public class BarSparklineView : BarSparklineViewBase, ISupportNegativePointsControl {
		const bool defaultHighlightNegativePoints = false;
		bool highlightNegativePoints = defaultHighlightNegativePoints;
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("BarSparklineViewHighlightNegativePoints"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.BarSparklineView.HighlightNegativePoints"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool HighlightNegativePoints {
			get { return highlightNegativePoints; }
			set {
				if (highlightNegativePoints != value) {
					highlightNegativePoints = value;
					OnPropertiesChanged();
				}
			}
		}
		#region ShouldSerialize
		bool ShouldSerializeHighlightNegativePoints() {
			return highlightNegativePoints != defaultHighlightNegativePoints;
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "HighlightNegativePoints":
					return ShouldSerializeHighlightNegativePoints();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region Reset
		void ResetHighlightNegativePoints() {
			highlightNegativePoints = defaultHighlightNegativePoints;
		}
		#endregion
		protected internal override bool ActualShowNegativePoint { get { return HighlightNegativePoints; } }
#if !SL
	[DevExpressSparklineCoreLocalizedDescription("BarSparklineViewType")]
#endif
		public override SparklineViewType Type { get { return SparklineViewType.Bar; } }
		protected override string GetViewName() {
			return SparklineLocalizer.GetString(SparklineStringId.viewBar);
		}
		public override void Assign(SparklineViewBase view) {
			base.Assign(view);
			ISupportNegativePointsControl negativePointsView = view as ISupportNegativePointsControl;
			if (negativePointsView != null) {
				highlightNegativePoints = negativePointsView.HighlightNegativePoints;
			}
		}
		public override void Visit(ISparklineViewVisitor visitor) {
			visitor.Visit(this);
		}
	}
}
