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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DevExpress.DashboardCommon.Native;
using DevExpress.Office.Utils;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.DashboardCommon.ViewModel {
	public class StyleSettingsModel {
		public int? Color { get; set; }
		public int? ForeColor { get; set; }
		public string FontFamily { get; set; }
		public float FontSize { get; set; }
		public byte? FontStyle { get; set; }
		public int? RangeIndex { get; set; }
		public int RuleIndex { get; set; }
		public FormatConditionAppearanceType AppearanceType { get; set; }
		public FormatConditionIconType IconType { get; set; }
		public FormatConditionImageModel Image { get; set; }
		public bool IsBarStyle { get; set; }
		public StyleSettingsModel() {
			this.FontSize = -1;
		}
		public StyleSettingsModel(FormatConditionAppearanceType appearanceType) : this() {
			this.AppearanceType = appearanceType;
		}
		public StyleSettingsModel(Color backColor) : this(FormatConditionAppearanceType.Custom) {
			this.Color = backColor.ToArgb();
		}
		public StyleSettingsModel(Color backColor, Color color) : this(backColor) {
			this.ForeColor = color.ToArgb();
		}
		public StyleSettingsModel(FontStyle style) : this(FormatConditionAppearanceType.Custom) {
			this.FontStyle = (byte)style;
		}
		public StyleSettingsModel(Color color, FontStyle style) : this(style) {
			this.ForeColor = color.ToArgb();
		}
		public StyleSettingsModel(FormatConditionIconType iconType) : this() {
			this.IconType = iconType;
		}
	}
	public class FormatConditionImageModel {
		const string ImagePngMimeType = "image/png";
		public string Url { get; set; }
		public string SourceBase64String { get; set; }
		public string MimeType { get; set; }
		public bool IsInternalImage { get; set; }
		public FormatConditionImageModel(DashboardImage image) {
			if(image.Data != null) {
				try {
					SetData(image.Data);
					using(Stream stream = new MemoryStream(image.Data))
						MimeType = OfficeImage.GetContentType(OfficeImage.CreateImage(stream).RawFormat);
				}
				catch {
				}
			}
			else if(!string.IsNullOrEmpty(image.Url))
				Url = image.Url;
		}
		void SetData(byte[] data) {
			SourceBase64String = Convert.ToBase64String(data);
		}
	}
	public class ConditionalFormattingModel {
		public IList<StyleSettingsModel> FormatConditionStyleSettings { get; set; }
		public IList<FormatRuleModelBase> RuleModels { get; set; }
	}
	public class FormatRuleModelBase {
		public string FormatConditionMeasureId { get; set; }
		public string NormalizedValueMeasureId { get; set; }
		public string ZeroPositionMeasureId { get; set; }
		public bool ApplyToRow { get; set; }
		public ConditionModel ConditionModel { get; set; }
	}
	public class PivotFormatRuleModel : FormatRuleModelBase {
		public string ApplyToDataId { get; set; }
		public bool ApplyToColumn { get; set; }
	}
	public class GridFormatRuleModel : FormatRuleModelBase {
		public string ApplyToDataId { get; set; }
		public string CalcByDataId { get; set; }
	}
	public class ConditionModel {
		public Dictionary<int, StyleSettingsModel> FixedColors { get; set; }
	}
	public class BarConditionModel : ConditionModel {
		public BarOptionsModel BarOptions { get; set; }
	}
	public class BarOptionsModel {
		public bool ShowBarOnly { get; set; }
		public bool AllowNegativeAxis { get; set; }
		public bool DrawAxis { get; set; }
	}
}
