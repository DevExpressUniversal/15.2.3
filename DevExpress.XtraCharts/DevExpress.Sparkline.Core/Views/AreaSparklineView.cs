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
using DevExpress.Utils.Serializing;
namespace DevExpress.Sparkline {
	public class AreaSparklineView : LineSparklineView {
		const byte defaultAreaOpacity = 135;
		byte areaOpacity = defaultAreaOpacity;
		[
#if !SL
	DevExpressSparklineCoreLocalizedDescription("AreaSparklineViewAreaOpacity"),
#endif
		XtraSerializableProperty,
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.Sparkline.AreaSparklineView.AreaOpacity")
		]
		public byte AreaOpacity {
			get { return areaOpacity; }
			set {
				if (areaOpacity != value) {
					areaOpacity = value;
					OnPropertiesChanged();
				}
			}
		}
#if !SL
	[DevExpressSparklineCoreLocalizedDescription("AreaSparklineViewType")]
#endif
		public override SparklineViewType Type {
			get { return SparklineViewType.Area; }
		}
		#region ShouldSerialize
		bool ShouldSerializeAreaOpacity() {
			return areaOpacity != defaultAreaOpacity;
		}
		protected override bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "AreaOpacity":
					return ShouldSerializeAreaOpacity();
				default:
					return base.XtraShouldSerialize(propertyName);
			}
		}
		#endregion
		#region Reset
		void ResetAreaOpacity() {
			areaOpacity = defaultAreaOpacity;
		}
		#endregion
		protected override string GetViewName() {
			return SparklineLocalizer.GetString(SparklineStringId.viewArea);
		}
		public override void Assign(SparklineViewBase view) {
			base.Assign(view);
			AreaSparklineView areaView = view as AreaSparklineView;
			if (areaView != null) {
				areaOpacity = areaView.areaOpacity;
			}
		}
		public override void Visit(ISparklineViewVisitor visitor) {
			visitor.Visit(this);
		}
	}
}
