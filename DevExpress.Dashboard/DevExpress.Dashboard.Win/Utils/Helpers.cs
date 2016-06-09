#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardWin.Localization;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using DevExpress.Skins;
namespace DevExpress.DashboardWin {
	internal class ResFinder { }
}
namespace DevExpress.DashboardWin.Native {
	public static class DashboardWinHelper {
		public const string ObsoleteBarItemMessage = "The Ribbon (Toolbar) contains obsolete bar items. To remove them, recreate the Ribbon (Toolbar) either using the smart tag menu or in code.";
		static readonly IEnumerable<PrintingSystemCommand> printPreviewHiddenCommands = new PrintingSystemCommand[] {
			PrintingSystemCommand.ExportXls,
			PrintingSystemCommand.SendXls,
			PrintingSystemCommand.ExportXlsx,
			PrintingSystemCommand.SendXlsx, 
			PrintingSystemCommand.ExportRtf,
			PrintingSystemCommand.SendRtf,
			PrintingSystemCommand.ExportMht,
			PrintingSystemCommand.SendMht,
			PrintingSystemCommand.ExportCsv,
			PrintingSystemCommand.SendCsv,
			PrintingSystemCommand.ExportTxt,
			PrintingSystemCommand.SendTxt,
			PrintingSystemCommand.ExportHtm,
		};
		public static void SetParentLookAndFeel(object obj, UserLookAndFeel parentLookAndFeel) {
			if(obj != null) {
				ISupportLookAndFeel supportLookAndFeel = obj as ISupportLookAndFeel;
				if(supportLookAndFeel == null) {
					PropertyInfo property = obj.GetType().GetProperty("LookAndFeel", BindingFlags.Public | BindingFlags.Instance);
					if(property != null) {
						UserLookAndFeel lookAndFeel = property.GetValue(obj, null) as UserLookAndFeel;
						if(lookAndFeel != null)
							lookAndFeel.ParentLookAndFeel = parentLookAndFeel;
					}
				}
				else
					if(supportLookAndFeel.LookAndFeel != null)
						supportLookAndFeel.LookAndFeel.ParentLookAndFeel = parentLookAndFeel;
			}
		}
		public static bool IsDarkScheme(UserLookAndFeel lookAndFeel) {
			return DevExpress.Utils.Frames.FrameHelper.IsDarkSkin(lookAndFeel, Utils.Frames.FrameHelper.SkinDefinitionReason.Rules);
		}
		public static Color GetBarAxisColor(UserLookAndFeel lookAndFeel) {
			return DashboardSkins.GetSkin(lookAndFeel).Colors.GetColor("BarAxisColor");
		}
		public static void ShowWarningMessage(UserLookAndFeel lookAndFeel, IWin32Window owner, string text) {
			XtraMessageBox.Show(lookAndFeel, owner, text, DashboardWinLocalizer.GetString(DashboardWinStringId.MessageBoxWarningTitle),
				MessageBoxButtons.OK, MessageBoxIcon.Warning);
		}
		public static void SetPrintPreviewCommandsVisibility(ReportPrintTool tool, IEnumerable<PrintingSystemCommand> hiddenCommands) {
			foreach(PrintingSystemCommand command in hiddenCommands)
				tool.PrintingSystem.SetCommandVisibility(command, CommandVisibility.None);
		}
		public static IEnumerable<PrintingSystemCommand> GetPrintPreviewHiddenCommands(string itemType) {
			IEnumerable<PrintingSystemCommand> exportToXlsCommands = new List<PrintingSystemCommand> {
				PrintingSystemCommand.ExportXls,
				PrintingSystemCommand.ExportXlsx,
				PrintingSystemCommand.SendXls,
				PrintingSystemCommand.SendXlsx
			};
			IEnumerable<PrintingSystemCommand> resultCommands = printPreviewHiddenCommands;
			if(itemType == DashboardItemType.Grid || itemType == DashboardItemType.Pivot)
				resultCommands = resultCommands.Except(exportToXlsCommands);
			return resultCommands;
		}
		public static IEnumerable<PrintingSystemCommand> GetPrintPreviewHiddenCommands() {
			return printPreviewHiddenCommands;
		}
		public static void AppendColoredText(StringBuilder sb, bool fontBold, Color fontColor, string text) {
			if(fontBold)
				sb.Append("<b>");
			sb.AppendFormat("<color={0},{1},{2}>{3}</color>", fontColor.R, fontColor.G, fontColor.B, text);
			if(fontBold)
				sb.Append("</b>");
		}
		public static ToolTipControlInfo GetSparklineTooltip(IList<double> values, ValueFormatViewModel format, object tooltipId) {
			if(values != null && values.Count > 0) {
				FormatterBase formatter = FormatterHelper.GetFormatter(format);
				StringBuilder builder = new StringBuilder();
				const string template = "<b>{0}</b> {1}";
				builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipStartValue),
					formatter.Format(values.FirstOrDefault())));
				builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipEndValue),
					formatter.Format(values.LastOrDefault())));
				builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipMinValue),
					formatter.Format(values.Min())));
				builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipMaxValue),
					formatter.Format(values.Max())));
				return new ToolTipControlInfo(tooltipId, builder.ToString(), true, ToolTipIconType.None) { AllowHtmlText = DefaultBoolean.True };
			}
			return null;
		}
		public static ToolTipControlInfo GetSparklineTooltip(string start, string end, string min, string max, object tooltipId) {
			StringBuilder builder = new StringBuilder();
			const string template = "<b>{0}</b> {1}";
			builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipStartValue), start));
			builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipEndValue), end));
			builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipMinValue), min));
			builder.AppendLine(string.Format(template, DashboardLocalizer.GetString(DashboardStringId.SparklineTooltipMaxValue), max));
			return new ToolTipControlInfo(tooltipId, builder.ToString(), true, ToolTipIconType.None) { AllowHtmlText = DefaultBoolean.True };
		}
		public static void SetCorrectSummaryType(Measure measure, IDataSourceSchema dataSource) {
			SummaryType summaryType = MeasureDefinition.DefaultSummaryType;
			if(measure != null && dataSource != null) {
				DataField dataField = dataSource.GetField(measure.DataMember);
				if(dataField != null) {
					switch(dataField.FieldType) {
						case DataFieldType.Bool:
						case DataFieldType.Text:
						case DataFieldType.DateTime:
							summaryType = SummaryType.Count;
							break;
						case DataFieldType.Custom:
							if(!dataField.IsConvertible)
								summaryType = SummaryType.Count;
							break;
						default:
							break;
					}
				}
			}
			if(summaryType != MeasureDefinition.DefaultSummaryType)
				measure.SummaryType = summaryType;
		}
		public static Dimension ConvertToDimension(DataItem dataItem) {
			return ConvertToDimensionInternal(dataItem, false);
		}
		public static Measure ConvertToMeasure(DataItem dataItem) {
			return ConvertToMeasureInternal(dataItem, false);
		}
		public static Dimension CloneToDimension(DataItem dataItem) {
			return ConvertToDimensionInternal(dataItem, true);
		}
		public static Measure CloneToMeasure(DataItem dataItem) {
			return ConvertToMeasureInternal(dataItem, true);
		}
		public static void DrawIgnoreUpdatesState(Graphics graphics, UserLookAndFeel lookAndFeel, Rectangle clientRectangle) {
			graphics.FillRectangle(new SolidBrush(Color.FromArgb(127, GetIgnoreUpdatesColor(lookAndFeel))), clientRectangle);
		}
		public static Color GetIgnoreUpdatesColor(UserLookAndFeel lookAndFeel){
			return CommonSkins.GetSkin(lookAndFeel).Colors.GetColor("Control");
		}
		static Dimension ConvertToDimensionInternal(DataItem dataItem, bool forceClone) {
			if (dataItem != null) {
				Dimension dimension = dataItem as Dimension;
				if (dimension != null) {
					if (forceClone)
						return dimension.Clone();
					else
						return dimension;
				}
				else
					return new Dimension(dataItem.DataMember);
			}
			else
				return null;
		}
		static Measure ConvertToMeasureInternal(DataItem dataItem, bool forceClone) {
			if (dataItem != null) {
				Measure measure = dataItem as Measure;
				if (measure != null) {
					if (forceClone)
						return measure.Clone();
					else
						return measure;
				}
				else
					return new Measure(dataItem.DataMember);
			}
			else
				return null;
		}
	}
}
