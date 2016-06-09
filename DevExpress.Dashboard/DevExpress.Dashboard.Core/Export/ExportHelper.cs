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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.Helpers;
using DevExpress.PivotGrid.Printing;
using DevExpress.Skins;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Net;
using System.Security.Permissions;
namespace DevExpress.DashboardExport {
	public enum LabelType {
		Text,
		Parameters
	}
	public class TextWithParameters {
		public string Text { get; set; }
		public Font TextFont { get; set; }
		public Color TextColor { get; set; }
		public PaddingInfo TextPaddingInfo { get; set; }
		public string Parameters { get; set; }
		public Font ParametersFont { get; set; }
		public Color ParametersColor { get; set; }
		public PaddingInfo ParametersPaddingInfo { get; set; }
		public TextAlignment Alignment { get; set; }
		public Dictionary<LabelType, XRTrimmingTextLabel> GetLabels() {
			Dictionary<LabelType, XRTrimmingTextLabel> labels = new Dictionary<LabelType, XRTrimmingTextLabel>();
			if(!string.IsNullOrEmpty(Text)) {
				labels.Add(
					LabelType.Text, 
					new XRTrimmingTextLabel {
						Text = Text,
						Font = TextFont,
						Borders = BorderSide.None,
						TextAlignment = Alignment,
						WordWrap = false,
						Trimming = StringTrimming.EllipsisCharacter,
						ForeColor = TextColor,
						Padding = TextPaddingInfo,
						CanGrow = false
					}
				);
			}
			if(!string.IsNullOrEmpty(Parameters)) {
			   labels.Add(
					LabelType.Parameters, 
					new XRTrimmingTextLabel {
						Text = Parameters,
						Font = ParametersFont,
						Borders = BorderSide.None,
						TextAlignment = Alignment,
						WordWrap = false,
						Trimming = StringTrimming.EllipsisCharacter,
						ForeColor = ParametersColor,
						Padding = ParametersPaddingInfo, 
						CanGrow = false
					}
				);
			}
			return labels;
		}
	}
	public static class ExportHelper {
		const int MaxCaptionHeight = 16;
		public const string DefaultFontFamily = "Segoe UI";
		public static Font DefaultTitleFont { get { return new Font("Segoe UI Light", 32, GraphicsUnit.Point); } }
		public static Font DefaultCaptionFont { get { return new Font(DefaultFontFamily, MaxCaptionHeight, GraphicsUnit.Point); } }
		public static Font CalcCaptionFont(int captionHeight) {
			int fontSize = Math.Min(DashboardStringHelper.GetFontSizeByLineHeight(DefaultFontFamily, captionHeight), MaxCaptionHeight);
			return new Font(DefaultFontFamily, fontSize, GraphicsUnit.Point);
		}
		public static Bitmap DownloadImage(string imageUrl) {
			try {
				WebClient client = new WebClient();
				Stream stream = client.OpenRead(imageUrl);
				Bitmap bitmap = new Bitmap(stream);
				stream.Flush();
				stream.Close();
				return bitmap;
			} catch {
				return null;
			}
		}
		public static Size GetPageClientSize(PaperKind paperKind, Margins margins, bool landscape) {
			Size clientSize = PageSizeInfo.GetPageSize(paperKind, GraphicsUnit.Pixel);
			if(landscape)
				clientSize = new Size { Width = clientSize.Height, Height = clientSize.Width};
			return new Size(clientSize.Width - margins.Left - margins.Right - 1, clientSize.Height - margins.Top - margins.Bottom - 1);
		}
		public static void DrawEmptyBrick(IBrickGraphics graph, Size actualPrintSize) {
			ImageBrick emptyBrick = new ImageBrick();
			emptyBrick.Sides = BorderSide.None;
			RectangleF fakeBrickSize = new RectangleF(0, 0, actualPrintSize.Width, actualPrintSize.Height);
			graph.DrawBrick(emptyBrick, fakeBrickSize);
		}
		public static void ApplyPivotPrintAppearance(PrintAppearance appearance, DashboardFontInfo fontInfo) {
			appearance.Cell.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.Cell.Font, fontInfo);
			appearance.CustomTotalCell.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.CustomTotalCell.Font, fontInfo);
			appearance.FieldHeader.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.FieldHeader.Font, fontInfo);
			appearance.FieldValue.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.FieldValue.Font, fontInfo);
			appearance.FieldValueGrandTotal.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.FieldValueGrandTotal.Font, fontInfo);
			appearance.FieldValueTotal.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.FieldValueTotal.Font, fontInfo);
			appearance.GrandTotalCell.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.GrandTotalCell.Font, fontInfo);
			appearance.TotalCell.Font = DevExpress.DashboardCommon.Printing.FontHelper.GetFont(appearance.TotalCell.Font, fontInfo);
		}
		public static Color GetDefaultBackColor() {
			return CommonSkins.GetSkin(null).CommonSkin.GetSystemColor(SystemColors.Window);
		}
		public static bool CheckSecurityPermissions() {
			return SecurityHelper.IsPermissionGranted(new SecurityPermission(PermissionState.Unrestricted));
		}
		public static bool SupportMetafileImageFormat {
			get {
				return DashboardExportSettings.CompatibilityMode == DashboardExportCompatibilityMode.Full && CheckSecurityPermissions();
			}
		}
	}
	static class PivotFieldExportHelper {
		static public void SetupFieldFormat(PivotGridFieldBase field, ValueFormatViewModel format) {
			if (format != null && format.DataType == ValueDataType.Numeric) {
				NumericFormatter formatter = FormatterBase.CreateFormatter(format) as NumericFormatter;
				field.CellFormat.FormatType = FormatType.Numeric;
				field.CellFormat.FormatString = formatter.GetGeneralFormatPattern();
			}
		}
	}
}
