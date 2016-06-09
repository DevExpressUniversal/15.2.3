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
namespace DevExpress.Xpf.Charts.Localization {
	public enum ChartStringId {
		EmptyLegendText,
		MsgIncorrectPropertyUsage,
		MsgIncorrectPointsCreation,
		MsgIncorrectLabelAngle,
		MsgInvalidLogarithmicBase,
		MsgIncorrectSeriesRemoving,
		MsgIncorrectSeriesReplacement,
		MsgIncorrectSeriesReordering,
		MsgDiagramToPointIncorrectValue,
		ChartControlLocalizedControlType,
		ChartControlAutomationPeerHelpText,
		MsgSeriesPointIncorrectValue,
		ArgumentPatternDescription,
		ValuePatternDescription,
		SeriesNamePatternDescription,
		StackedGroupPatternDescription,
		Value1PatternDescription,
		Value2PatternDescription,
		WeightPatternDescription,
		HighValuePatternDescription,
		LowValuePatternDescription,
		OpenValuePatternDescription,
		CloseValuePatternDescription,
		PercentValuePatternDescription,
		PointHintPatternDescription,
		ValueDurationPatternDescription,
		PatternEditorPreviewCaption,
		InvalidPlaceholder,
		ErrorTitle
	}
	public class ChartResLocalizer : XtraResXLocalizer<ChartStringId> {
		public ChartResLocalizer() : base(new ChartLocalizer()) {
		}
		protected override ResourceManager CreateResourceManagerCore() {
			return new ResourceManager("DevExpress.Xpf.Charts.LocalizationRes", typeof(ChartResLocalizer).Assembly);
		}
	}
	public class ChartLocalizer : XtraLocalizer<ChartStringId> {
		static ChartLocalizer() {
			SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<ChartStringId>(CreateDefaultLocalizer()));
		}
		protected override void PopulateStringTable() {
			AddString(ChartStringId.EmptyLegendText, "Specify DisplayName");
			AddString(ChartStringId.MsgIncorrectPropertyUsage, "The '{0}' property isn't supported for the '{1}' class.");
			AddString(ChartStringId.MsgIncorrectPointsCreation, "The series point collection should be populated either from a data source, or manually, but not both at a time.");
			AddString(ChartStringId.MsgInvalidLogarithmicBase, "The logarithmic base should be greater than 1.");
			AddString(ChartStringId.MsgIncorrectLabelAngle, "The angle of the label should be greater than or equal to -360 and less than or equal to 360.");
			AddString(ChartStringId.MsgIncorrectSeriesRemoving, "It's impossible to remove auto-created series.");
			AddString(ChartStringId.MsgIncorrectSeriesReplacement, "The auto-created series cannot be assigned.");
			AddString(ChartStringId.MsgIncorrectSeriesReordering, "When reordering series within the collection, automatically created series cannot replace the manually created ones, and vice versa.");
			AddString(ChartStringId.MsgDiagramToPointIncorrectValue, "The specified {0} parameter type doesn't match the appropriate scale type, which is {1} for this axis.");
			AddString(ChartStringId.ChartControlLocalizedControlType, "chart");
			AddString(ChartStringId.ChartControlAutomationPeerHelpText, "A chart control.");
			AddString(ChartStringId.MsgSeriesPointIncorrectValue, "The point value cannot be equal to infinity.");
			AddString(ChartStringId.ArgumentPatternDescription, "Argument");
			AddString(ChartStringId.ValuePatternDescription, "Value");
			AddString(ChartStringId.SeriesNamePatternDescription, "Series Name");
			AddString(ChartStringId.StackedGroupPatternDescription, "Series Group");
			AddString(ChartStringId.Value1PatternDescription, "Value1");
			AddString(ChartStringId.Value2PatternDescription, "Value2");
			AddString(ChartStringId.WeightPatternDescription, "Weight");
			AddString(ChartStringId.HighValuePatternDescription, "High Value");
			AddString(ChartStringId.LowValuePatternDescription, "Low Value");
			AddString(ChartStringId.OpenValuePatternDescription, "Open Value");
			AddString(ChartStringId.CloseValuePatternDescription, "Close Value");
			AddString(ChartStringId.PercentValuePatternDescription, "Percent Value");
			AddString(ChartStringId.PointHintPatternDescription, "Point Hint");
			AddString(ChartStringId.ValueDurationPatternDescription, "Value Duration");
			AddString(ChartStringId.PatternEditorPreviewCaption, "Select a placeholder to see the preview");
			AddString(ChartStringId.InvalidPlaceholder, "Invalid Placeholder");
			AddString(ChartStringId.ErrorTitle, "Error");
		}
		public static XtraLocalizer<ChartStringId> CreateDefaultLocalizer() {
			return new ChartResLocalizer();
		}
		public static string GetString(ChartStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<ChartStringId> CreateResXLocalizer() {
			return new ChartResLocalizer();
		}
	}
}
