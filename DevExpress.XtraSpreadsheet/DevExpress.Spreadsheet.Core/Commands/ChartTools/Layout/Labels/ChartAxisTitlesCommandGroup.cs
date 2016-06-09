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
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Commands.Internal;
using DevExpress.XtraSpreadsheet.Localization;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Office.DrawingML;
using DevExpress.Office.Drawing;
namespace DevExpress.XtraSpreadsheet.Commands {
	#region ChartAxisTitlesCommandGroup
	public class ChartAxisTitlesCommandGroup : ChartCommandGroupBase {
		public ChartAxisTitlesCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartAxisTitlesCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartAxisTitlesCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartAxisTitlesCommandGroup; } }
		public override string ImageName { get { return "ChartAxisTitleGroup"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			foreach (IChartView view in chart.Views)
				if (view is RadarChartView)
					return false;
			return true;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisTitleCommandGroup
	public class ChartPrimaryHorizontalAxisTitleCommandGroup : ChartCommandGroupBase {
		public ChartPrimaryHorizontalAxisTitleCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleCommandGroup; } }
		public override string ImageName { get { return "ChartAxisTitleHorizontal"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return ChartModifyPrimaryAxisCommandBase.GetAxis(chart, true) != null;
		}
	}
	#endregion
	#region ChartPrimaryVerticalAxisTitleCommandGroup
	public class ChartPrimaryVerticalAxisTitleCommandGroup : ChartCommandGroupBase {
		public ChartPrimaryVerticalAxisTitleCommandGroup(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroup; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleCommandGroupDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleCommandGroup; } }
		public override string ImageName { get { return "ChartAxisTitleVertical"; } }
		#endregion
		protected override bool CanModifyChart(Chart chart) {
			if (chart.PrimaryAxes.Count <= 0)
				return false;
			return ChartModifyPrimaryAxisCommandBase.GetAxis(chart, false) != null;
		}
	}
	#endregion
	#region ChartModifyPrimaryAxisTitleCommandBase (abstract class)
	public abstract class ChartModifyPrimaryAxisTitleCommandBase : ChartModifyPrimaryAxisCommandBase {
		protected ChartModifyPrimaryAxisTitleCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		protected override AxisDataType AxisType { get { return AxisDataType.Agrument; } } 
		protected override bool ShouldHideCommand(Chart chart) {
			return false;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisTitleNoneCommand
	public class ChartPrimaryHorizontalAxisTitleNoneCommand : ChartModifyPrimaryAxisTitleCommandBase {
		public ChartPrimaryHorizontalAxisTitleNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleNone; } }
		public override string ImageName { get { return "ChartAxisTitleHorizontal_None"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return !axis.Title.Visible;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Title.Text = ChartText.Empty;
		}
	}
	#endregion
	#region ChartPrimaryHorizontalAxisTitleBelowCommand
	public class ChartPrimaryHorizontalAxisTitleBelowCommand : ChartModifyPrimaryAxisTitleCommandBase {
		public ChartPrimaryHorizontalAxisTitleBelowCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleBelowCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryHorizontalAxisTitleBelowCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryHorizontalAxisTitleBelow; } }
		public override string ImageName { get { return "ChartAxisTitleHorizontal"; } }
		protected override bool IsHorizontalAxis { get { return true; } }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			return axis.Title.Visible && !axis.Title.Overlay;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			axis.Title.Overlay = false;
			if(axis.Title.Text.TextType == ChartTextType.None)
				axis.Title.Text = ChartText.Auto;
		}
	}
	#endregion
	#region ChartPrimaryVerticalAxisTitleNoneCommand
	public class ChartPrimaryVerticalAxisTitleNoneCommand : ChartPrimaryHorizontalAxisTitleNoneCommand {
		public ChartPrimaryVerticalAxisTitleNoneCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleNoneCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleNoneCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleNone; } }
		public override string ImageName { get { return "ChartAxisTitleVertical_None"; } }
		protected override bool IsHorizontalAxis { get { return false; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisTitleCommandBase (abstract class)
	public abstract class ChartPrimaryVerticalAxisTitleCommandBase : ChartModifyPrimaryAxisTitleCommandBase {
		protected ChartPrimaryVerticalAxisTitleCommandBase(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		protected override bool IsHorizontalAxis { get { return false; } }
		protected abstract int RotationAngle { get; }
		protected abstract DrawingTextVerticalTextType VerticalTextType { get; }
		#endregion
		protected override bool IsChecked(Chart chart) {
			AxisBase axis = GetAxis(chart);
			TitleOptions title = axis.Title;
			TextProperties textProperties = title.TextProperties;
			if (title.Text.TextType == ChartTextType.Rich)
				textProperties = ((ChartRichText)title.Text).TextProperties;
			return title.Visible && !title.Overlay &&
				textProperties.BodyProperties.Rotation == DocumentModel.UnitConverter.AdjAngleToModelUnits(RotationAngle) &&
				textProperties.BodyProperties.VerticalText == VerticalTextType;
		}
		protected override void ModifyChart(Chart chart) {
			AxisBase axis = GetAxis(chart);
			TitleOptions title = axis.Title;
			title.Overlay = false;
			if (title.Text.TextType == ChartTextType.None)
				title.Text = ChartText.Auto;
			TextProperties textProperties = title.TextProperties;
			if (title.Text.TextType == ChartTextType.Rich)
				textProperties = ((ChartRichText)title.Text).TextProperties;
			textProperties.BodyProperties.Rotation = DocumentModel.UnitConverter.AdjAngleToModelUnits(RotationAngle);
			textProperties.BodyProperties.VerticalText = VerticalTextType;
			if (textProperties.Paragraphs.Count == 0) {
				DrawingTextParagraph paragraph = new DrawingTextParagraph(DocumentModel);
				paragraph.ApplyEndRunProperties = true;
				paragraph.ApplyParagraphProperties = true;
				textProperties.Paragraphs.Add(paragraph);
			}
		}
	}
	#endregion
	#region ChartPrimaryVerticalAxisTitleRotatedCommand
	public class ChartPrimaryVerticalAxisTitleRotatedCommand : ChartPrimaryVerticalAxisTitleCommandBase {
		public ChartPrimaryVerticalAxisTitleRotatedCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleRotatedCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleRotatedCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleRotated; } }
		public override string ImageName { get { return "ChartAxisTitleVertical_RotatedText"; } }
		protected override int RotationAngle { get { return -5400000; } }
		protected override DrawingTextVerticalTextType VerticalTextType { get { return DrawingTextVerticalTextType.Horizontal; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisTitleVerticalCommand
	public class ChartPrimaryVerticalAxisTitleVerticalCommand : ChartPrimaryVerticalAxisTitleCommandBase {
		public ChartPrimaryVerticalAxisTitleVerticalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleVerticalCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleVerticalCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleVertical; } }
		public override string ImageName { get { return "ChartAxisTitleVertical_VerticalText"; } }
		protected override int RotationAngle { get { return 0; } }
		protected override DrawingTextVerticalTextType VerticalTextType { get { return DrawingTextVerticalTextType.WordArtVertical; } }
		#endregion
	}
	#endregion
	#region ChartPrimaryVerticalAxisTitleHorizontalCommand
	public class ChartPrimaryVerticalAxisTitleHorizontalCommand : ChartPrimaryVerticalAxisTitleCommandBase {
		public ChartPrimaryVerticalAxisTitleHorizontalCommand(ISpreadsheetControl control)
			: base(control) {
		}
		#region Properties
		public override XtraSpreadsheetStringId MenuCaptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleHorizontalCommand; } }
		public override XtraSpreadsheetStringId DescriptionStringId { get { return XtraSpreadsheetStringId.MenuCmd_ChartPrimaryVerticalAxisTitleHorizontalCommandDescription; } }
		public override SpreadsheetCommandId Id { get { return SpreadsheetCommandId.ChartPrimaryVerticalAxisTitleHorizontal; } }
		public override string ImageName { get { return "ChartAxisTitleVertical_HorizonlalText"; } }
		protected override int RotationAngle { get { return 0; } }
		protected override DrawingTextVerticalTextType VerticalTextType { get { return DrawingTextVerticalTextType.Horizontal; } }
		#endregion
	}
	#endregion
}
