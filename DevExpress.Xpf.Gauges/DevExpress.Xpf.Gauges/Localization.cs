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

using System.Resources;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Xpf.Gauges.Localization {
	#region enum GaugeStringId
	public enum GaugeStringId {
		CircularGaugeLocalizedControlType,
		LinearGaugeLocalizedControlType,
		DigitalGaugeLocalizedControlType,
		StateIndicatorLocalizedControlType,
		ScaleLocalizedControlType,
		NeedleLocalizedControlType,
		MarkerLocalizedControlType,
		RangeBarLocalizedControlType,
		LevelBarLocalizedControlType,
		ValueIndicatorLocalizedControlType,
		CircularGaugeAutomationPeerHelpText,
		LinearGaugeAutomationPeerHelpText,
		DigitalGaugeAutomationPeerHelpText,
		StateIndicatorAutomationPeerHelpText
	}
	#endregion
	public class GaugeResLocalizer : XtraResXLocalizer<GaugeStringId> {
		public GaugeResLocalizer()
			: base(new GaugeLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Gauges.LocalizationRes", typeof(GaugeResLocalizer).Assembly);
		}
	}
	public class GaugeLocalizer : XtraLocalizer<GaugeStringId> {
		static GaugeLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<GaugeStringId>(CreateDefaultLocalizer()));
		}
		protected override void PopulateStringTable() {
			AddString(GaugeStringId.CircularGaugeLocalizedControlType, "circular gauge");
			AddString(GaugeStringId.LinearGaugeLocalizedControlType, "linear gauge");
			AddString(GaugeStringId.DigitalGaugeLocalizedControlType, "digital gauge");
			AddString(GaugeStringId.StateIndicatorLocalizedControlType, "state indicator");
			AddString(GaugeStringId.ScaleLocalizedControlType, "scale");
			AddString(GaugeStringId.NeedleLocalizedControlType, "needle");
			AddString(GaugeStringId.MarkerLocalizedControlType, "marker");
			AddString(GaugeStringId.RangeBarLocalizedControlType, "range bar");
			AddString(GaugeStringId.LevelBarLocalizedControlType, "level bar");
			AddString(GaugeStringId.ValueIndicatorLocalizedControlType, "value indicator");
			AddString(GaugeStringId.CircularGaugeAutomationPeerHelpText, "A gauge control that indicates values on circular scales");
			AddString(GaugeStringId.LinearGaugeAutomationPeerHelpText, "A gauge control that indicates values on linear scales");
			AddString(GaugeStringId.DigitalGaugeAutomationPeerHelpText, "A gauge control that displays values using digital symbols");
			AddString(GaugeStringId.StateIndicatorAutomationPeerHelpText, "A state indicator control that displays an image corresponds to its current state");
		}
		public static XtraLocalizer<GaugeStringId> CreateDefaultLocalizer() {
			return new GaugeResLocalizer();
		}
		public static string GetString(GaugeStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<GaugeStringId> CreateResXLocalizer() {
			return new GaugeResLocalizer();
		}
	}
}
