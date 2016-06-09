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
using System.Diagnostics;
using DevExpress.Export.Xl;
using DevExpress.Office.Model;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PredefinedTableStyleId // 7 bits
	public enum PredefinedTableStyleId {
		#region TableStyleLightId
		TableStyleLight1 = 1,
		TableStyleLight2 = 2,
		TableStyleLight3 = 3,
		TableStyleLight4 = 4,
		TableStyleLight5 = 5,
		TableStyleLight6 = 6,
		TableStyleLight7 = 7,
		TableStyleLight8 = 8,
		TableStyleLight9 = 9,
		TableStyleLight10 = 10,
		TableStyleLight11 = 11,
		TableStyleLight12 = 12,
		TableStyleLight13 = 13,
		TableStyleLight14 = 14,
		TableStyleLight15 = 15,
		TableStyleLight16 = 16,
		TableStyleLight17 = 17,
		TableStyleLight18 = 18,
		TableStyleLight19 = 19,
		TableStyleLight20 = 20,
		TableStyleLight21 = 21,
		#endregion
		#region TableStyleMediumId
		TableStyleMedium1 = 22,
		TableStyleMedium2 = 23,
		TableStyleMedium3 = 24,
		TableStyleMedium4 = 25,
		TableStyleMedium5 = 26,
		TableStyleMedium6 = 27,
		TableStyleMedium7 = 28,
		TableStyleMedium8 = 29,
		TableStyleMedium9 = 30,
		TableStyleMedium10 = 31,
		TableStyleMedium11 = 32,
		TableStyleMedium12 = 33,
		TableStyleMedium13 = 34,
		TableStyleMedium14 = 35,
		TableStyleMedium15 = 36,
		TableStyleMedium16 = 37,
		TableStyleMedium17 = 38,
		TableStyleMedium18 = 39,
		TableStyleMedium19 = 40,
		TableStyleMedium20 = 41,
		TableStyleMedium21 = 42,
		TableStyleMedium22 = 43,
		TableStyleMedium23 = 44,
		TableStyleMedium24 = 45,
		TableStyleMedium25 = 46,
		TableStyleMedium26 = 47,
		TableStyleMedium27 = 48,
		TableStyleMedium28 = 49,
		#endregion
		#region TableStyleDarkId
		TableStyleDark1 = 50,
		TableStyleDark2 = 51,
		TableStyleDark3 = 52,
		TableStyleDark4 = 53,
		TableStyleDark5 = 54,
		TableStyleDark6 = 55,
		TableStyleDark7 = 56,
		TableStyleDark8 = 57,
		TableStyleDark9 = 58,
		TableStyleDark10 = 59,
		TableStyleDark11 = 60,
		#endregion
	}
	#endregion
	#region PredefinedPivotStyleId // 8 bits
	public enum PredefinedPivotStyleId {
		#region PivotStyleLightId
		PivotStyleLight1 = 1,
		PivotStyleLight2 = 2,
		PivotStyleLight3 = 3,
		PivotStyleLight4 = 4,
		PivotStyleLight5 = 5,
		PivotStyleLight6 = 6,
		PivotStyleLight7 = 7,
		PivotStyleLight8 = 8,
		PivotStyleLight9 = 9,
		PivotStyleLight10 = 10,
		PivotStyleLight11 = 11,
		PivotStyleLight12 = 12,
		PivotStyleLight13 = 13,
		PivotStyleLight14 = 14,
		PivotStyleLight15 = 15,
		PivotStyleLight16 = 16,
		PivotStyleLight17 = 17,
		PivotStyleLight18 = 18,
		PivotStyleLight19 = 19,
		PivotStyleLight20 = 20,
		PivotStyleLight21 = 21,
		PivotStyleLight22 = 22,
		PivotStyleLight23 = 23,
		PivotStyleLight24 = 24,
		PivotStyleLight25 = 25,
		PivotStyleLight26 = 26,
		PivotStyleLight27 = 27,
		PivotStyleLight28 = 28,
		#endregion
		#region PivotStyleMediumId
		PivotStyleMedium1 = 29,
		PivotStyleMedium2 = 30,
		PivotStyleMedium3 = 31,
		PivotStyleMedium4 = 32,
		PivotStyleMedium5 = 33,
		PivotStyleMedium6 = 34,
		PivotStyleMedium7 = 35,
		PivotStyleMedium8 = 36,
		PivotStyleMedium9 = 37,
		PivotStyleMedium10 = 38,
		PivotStyleMedium11 = 39,
		PivotStyleMedium12 = 40,
		PivotStyleMedium13 = 41,
		PivotStyleMedium14 = 42,
		PivotStyleMedium15 = 43,
		PivotStyleMedium16 = 44,
		PivotStyleMedium17 = 45,
		PivotStyleMedium18 = 46,
		PivotStyleMedium19 = 47,
		PivotStyleMedium20 = 48,
		PivotStyleMedium21 = 49,
		PivotStyleMedium22 = 50,
		PivotStyleMedium23 = 51,
		PivotStyleMedium24 = 52,
		PivotStyleMedium25 = 53,
		PivotStyleMedium26 = 54,
		PivotStyleMedium27 = 55,
		PivotStyleMedium28 = 56,
		#endregion
		#region PivotStyleDarkId
		PivotStyleDark1 = 57,
		PivotStyleDark2 = 58,
		PivotStyleDark3 = 59,
		PivotStyleDark4 = 60,
		PivotStyleDark5 = 61,
		PivotStyleDark6 = 62,
		PivotStyleDark7 = 63,
		PivotStyleDark8 = 64,
		PivotStyleDark9 = 65,
		PivotStyleDark10 = 66,
		PivotStyleDark11 = 67,
		PivotStyleDark12 = 68,
		PivotStyleDark13 = 69,
		PivotStyleDark14 = 70,
		PivotStyleDark15 = 71,
		PivotStyleDark16 = 72,
		PivotStyleDark17 = 73,
		PivotStyleDark18 = 74,
		PivotStyleDark19 = 75,
		PivotStyleDark20 = 76,
		PivotStyleDark21 = 77,
		PivotStyleDark22 = 78,
		PivotStyleDark23 = 79,
		PivotStyleDark24 = 80,
		PivotStyleDark25 = 81,
		PivotStyleDark26 = 82,
		PivotStyleDark27 = 83,
		PivotStyleDark28 = 84,
		#endregion
	}
	#endregion
	#region PredefinedTableStyleCalculator
	public class PredefinedTableStyleCalculator {
		#region Static Members
		public static void AppendPredefinedTableStyles(TableStyleCollection tableStyles) {
			Guard.ArgumentNotNull(tableStyles, "tableStyles");
			tableStyles.BeginUpdate();
			try {
				AppendPredefinedTableStylesCore(tableStyles);
				AppendPredefinedPivotStylesCore(tableStyles);
			} finally {
				tableStyles.EndUpdate();
			}
		}
		static void AppendPredefinedTableStylesCore(TableStyleCollection tableStyles) {
			DocumentModel documentModel = tableStyles.DocumentModel;
			Array values = Enum.GetValues(typeof(PredefinedTableStyleId));
			foreach (PredefinedTableStyleId id in values) {
				TableStyleName name = TableStyleName.CreatePredefined(id);
				if (!tableStyles.ContainsStyleName(name.Name))
					tableStyles.AddCore(new TableStyle(documentModel, name, TableStyleElementIndexTableType.Table));
			}
		}
		static void AppendPredefinedPivotStylesCore(TableStyleCollection tableStyles) {
			DocumentModel documentModel = tableStyles.DocumentModel;
			Array values = Enum.GetValues(typeof(PredefinedPivotStyleId));
			foreach (PredefinedPivotStyleId id in values) {
				TableStyleName name = TableStyleName.CreatePredefined(id);
				if (!tableStyles.ContainsStyleName(name.Name))
					tableStyles.AddCore(new TableStyle(documentModel, name, TableStyleElementIndexTableType.Pivot));
			}
		}
		#endregion
		#region Fields
		readonly TableStyle style;
		readonly RunFontInfo[] fonts;
		readonly FillInfo[] fills;
		readonly BorderInfo[] borders;
		readonly MultiOptionsInfo[] multiOptions;
		readonly BorderOptionsInfo[] borderOptions;
		#endregion
		public PredefinedTableStyleCalculator(TableStyle style) {
			Guard.ArgumentNotNull(style, "style");
			Debug.Assert(style.IsPredefined);
			this.style = style;
			fonts = new RunFontInfo[TableStyle.ElementsCount];
			fills = new FillInfo[TableStyle.ElementsCount];
			borders = new BorderInfo[TableStyle.ElementsCount];
			multiOptions = new MultiOptionsInfo[TableStyle.ElementsCount];
			borderOptions = new BorderOptionsInfo[TableStyle.ElementsCount];
		}
		#region Properties
		DocumentModel DocumentModel { get { return style.DocumentModel; } }
		DocumentCache Cache { get { return DocumentModel.Cache; } }
		ColorModelInfoCache ColorCache { get { return Cache.ColorModelInfoCache; } }
		#endregion
		delegate void CalculateStyleProperties();
		#region CalculateTableStyleProperties
		Dictionary<PredefinedTableStyleId, CalculateStyleProperties> GetTableStyleDelegateCollection() {
			Dictionary<PredefinedTableStyleId, CalculateStyleProperties> result = new Dictionary<PredefinedTableStyleId, CalculateStyleProperties>();
			#region TableStyleLight
			result.Add(PredefinedTableStyleId.TableStyleLight1, new CalculateStyleProperties(CalculateTableStyleLight1Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight2, new CalculateStyleProperties(CalculateTableStyleLight2Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight3, new CalculateStyleProperties(CalculateTableStyleLight3Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight4, new CalculateStyleProperties(CalculateTableStyleLight4Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight5, new CalculateStyleProperties(CalculateTableStyleLight5Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight6, new CalculateStyleProperties(CalculateTableStyleLight6Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight7, new CalculateStyleProperties(CalculateTableStyleLight7Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight8, new CalculateStyleProperties(CalculateTableStyleLight8Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight9, new CalculateStyleProperties(CalculateTableStyleLight9Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight10, new CalculateStyleProperties(CalculateTableStyleLight10Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight11, new CalculateStyleProperties(CalculateTableStyleLight11Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight12, new CalculateStyleProperties(CalculateTableStyleLight12Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight13, new CalculateStyleProperties(CalculateTableStyleLight13Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight14, new CalculateStyleProperties(CalculateTableStyleLight14Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight15, new CalculateStyleProperties(CalculateTableStyleLight15Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight16, new CalculateStyleProperties(CalculateTableStyleLight16Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight17, new CalculateStyleProperties(CalculateTableStyleLight17Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight18, new CalculateStyleProperties(CalculateTableStyleLight18Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight19, new CalculateStyleProperties(CalculateTableStyleLight19Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight20, new CalculateStyleProperties(CalculateTableStyleLight20Properties));
			result.Add(PredefinedTableStyleId.TableStyleLight21, new CalculateStyleProperties(CalculateTableStyleLight21Properties));
			#endregion
			#region TableStyleMedium
			result.Add(PredefinedTableStyleId.TableStyleMedium1, new CalculateStyleProperties(CalculateTableStyleMedium1Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium2, new CalculateStyleProperties(CalculateTableStyleMedium2Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium3, new CalculateStyleProperties(CalculateTableStyleMedium3Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium4, new CalculateStyleProperties(CalculateTableStyleMedium4Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium5, new CalculateStyleProperties(CalculateTableStyleMedium5Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium6, new CalculateStyleProperties(CalculateTableStyleMedium6Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium7, new CalculateStyleProperties(CalculateTableStyleMedium7Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium8, new CalculateStyleProperties(CalculateTableStyleMedium8Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium9, new CalculateStyleProperties(CalculateTableStyleMedium9Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium10, new CalculateStyleProperties(CalculateTableStyleMedium10Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium11, new CalculateStyleProperties(CalculateTableStyleMedium11Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium12, new CalculateStyleProperties(CalculateTableStyleMedium12Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium13, new CalculateStyleProperties(CalculateTableStyleMedium13Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium14, new CalculateStyleProperties(CalculateTableStyleMedium14Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium15, new CalculateStyleProperties(CalculateTableStyleMedium15Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium16, new CalculateStyleProperties(CalculateTableStyleMedium16Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium17, new CalculateStyleProperties(CalculateTableStyleMedium17Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium18, new CalculateStyleProperties(CalculateTableStyleMedium18Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium19, new CalculateStyleProperties(CalculateTableStyleMedium19Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium20, new CalculateStyleProperties(CalculateTableStyleMedium20Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium21, new CalculateStyleProperties(CalculateTableStyleMedium21Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium22, new CalculateStyleProperties(CalculateTableStyleMedium22Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium23, new CalculateStyleProperties(CalculateTableStyleMedium23Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium24, new CalculateStyleProperties(CalculateTableStyleMedium24Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium25, new CalculateStyleProperties(CalculateTableStyleMedium25Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium26, new CalculateStyleProperties(CalculateTableStyleMedium26Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium27, new CalculateStyleProperties(CalculateTableStyleMedium27Properties));
			result.Add(PredefinedTableStyleId.TableStyleMedium28, new CalculateStyleProperties(CalculateTableStyleMedium28Properties));
			#endregion
			#region TableStyleDark
			result.Add(PredefinedTableStyleId.TableStyleDark1, new CalculateStyleProperties(CalculateTableStyleDark1Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark2, new CalculateStyleProperties(CalculateTableStyleDark2Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark3, new CalculateStyleProperties(CalculateTableStyleDark3Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark4, new CalculateStyleProperties(CalculateTableStyleDark4Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark5, new CalculateStyleProperties(CalculateTableStyleDark5Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark6, new CalculateStyleProperties(CalculateTableStyleDark6Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark7, new CalculateStyleProperties(CalculateTableStyleDark7Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark8, new CalculateStyleProperties(CalculateTableStyleDark8Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark9, new CalculateStyleProperties(CalculateTableStyleDark9Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark10, new CalculateStyleProperties(CalculateTableStyleDark10Properties));
			result.Add(PredefinedTableStyleId.TableStyleDark11, new CalculateStyleProperties(CalculateTableStyleDark11Properties));
			#endregion
			return result;
		}
		#region CalculateTableStyleLight
		protected internal virtual void CalculateTableStyleLight1Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight2Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight3Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight4Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight5Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight6Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight7Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight8Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight9Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight10Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight11Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight12Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight13Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight14Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight15Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight16Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight17Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight18Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight19Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight20Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleLight21Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		#endregion
		#region CalculateTableStyleMedium
		protected internal virtual void CalculateTableStyleMedium1Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium2Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium3Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium4Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium5Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium6Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium7Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium8Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium9Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium10Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium11Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium12Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium13Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium14Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thick, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium15Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium16Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium17Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium18Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium19Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium20Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium21Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium22Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium23Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium24Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium25Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium26Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium27Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleMedium28Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		#endregion
		#region CalculateTableStyleDark
		protected internal virtual void CalculateTableStyleDark1Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.449995422223579), ColorModelInfo.Create(new ThemeColorIndex(1), 0.449995422223579));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(1), 0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark2Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark3Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark4Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark5Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark6Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark7Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.LastColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.LastColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark8Properties() {
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark9Properties() {
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark10Properties() {
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculateTableStyleDark11Properties() {
			AssignRunFontInfo(TableStyle.LastColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		#endregion
		#endregion
		#region CalculatePivotStyleProperties
		Dictionary<PredefinedPivotStyleId, CalculateStyleProperties> GetPivotStyleDelegateCollection() {
			Dictionary<PredefinedPivotStyleId, CalculateStyleProperties> result = new Dictionary<PredefinedPivotStyleId, CalculateStyleProperties>();
			#region PivotStyleLight
			result.Add(PredefinedPivotStyleId.PivotStyleLight1, new CalculateStyleProperties(CalculatePivotStyleLight1Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight2, new CalculateStyleProperties(CalculatePivotStyleLight2Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight3, new CalculateStyleProperties(CalculatePivotStyleLight3Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight4, new CalculateStyleProperties(CalculatePivotStyleLight4Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight5, new CalculateStyleProperties(CalculatePivotStyleLight5Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight6, new CalculateStyleProperties(CalculatePivotStyleLight6Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight7, new CalculateStyleProperties(CalculatePivotStyleLight7Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight8, new CalculateStyleProperties(CalculatePivotStyleLight8Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight9, new CalculateStyleProperties(CalculatePivotStyleLight9Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight10, new CalculateStyleProperties(CalculatePivotStyleLight10Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight11, new CalculateStyleProperties(CalculatePivotStyleLight11Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight12, new CalculateStyleProperties(CalculatePivotStyleLight12Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight13, new CalculateStyleProperties(CalculatePivotStyleLight13Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight14, new CalculateStyleProperties(CalculatePivotStyleLight14Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight15, new CalculateStyleProperties(CalculatePivotStyleLight15Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight16, new CalculateStyleProperties(CalculatePivotStyleLight16Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight17, new CalculateStyleProperties(CalculatePivotStyleLight17Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight18, new CalculateStyleProperties(CalculatePivotStyleLight18Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight19, new CalculateStyleProperties(CalculatePivotStyleLight19Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight20, new CalculateStyleProperties(CalculatePivotStyleLight20Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight21, new CalculateStyleProperties(CalculatePivotStyleLight21Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight22, new CalculateStyleProperties(CalculatePivotStyleLight22Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight23, new CalculateStyleProperties(CalculatePivotStyleLight23Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight24, new CalculateStyleProperties(CalculatePivotStyleLight24Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight25, new CalculateStyleProperties(CalculatePivotStyleLight25Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight26, new CalculateStyleProperties(CalculatePivotStyleLight26Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight27, new CalculateStyleProperties(CalculatePivotStyleLight27Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleLight28, new CalculateStyleProperties(CalculatePivotStyleLight28Properties));
			#endregion
			#region PivotStyleMedium
			result.Add(PredefinedPivotStyleId.PivotStyleMedium1, new CalculateStyleProperties(CalculatePivotStyleMedium1Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium2, new CalculateStyleProperties(CalculatePivotStyleMedium2Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium3, new CalculateStyleProperties(CalculatePivotStyleMedium3Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium4, new CalculateStyleProperties(CalculatePivotStyleMedium4Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium5, new CalculateStyleProperties(CalculatePivotStyleMedium5Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium6, new CalculateStyleProperties(CalculatePivotStyleMedium6Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium7, new CalculateStyleProperties(CalculatePivotStyleMedium7Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium8, new CalculateStyleProperties(CalculatePivotStyleMedium8Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium9, new CalculateStyleProperties(CalculatePivotStyleMedium9Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium10, new CalculateStyleProperties(CalculatePivotStyleMedium10Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium11, new CalculateStyleProperties(CalculatePivotStyleMedium11Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium12, new CalculateStyleProperties(CalculatePivotStyleMedium12Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium13, new CalculateStyleProperties(CalculatePivotStyleMedium13Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium14, new CalculateStyleProperties(CalculatePivotStyleMedium14Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium15, new CalculateStyleProperties(CalculatePivotStyleMedium15Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium16, new CalculateStyleProperties(CalculatePivotStyleMedium16Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium17, new CalculateStyleProperties(CalculatePivotStyleMedium17Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium18, new CalculateStyleProperties(CalculatePivotStyleMedium18Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium19, new CalculateStyleProperties(CalculatePivotStyleMedium19Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium20, new CalculateStyleProperties(CalculatePivotStyleMedium20Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium21, new CalculateStyleProperties(CalculatePivotStyleMedium21Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium22, new CalculateStyleProperties(CalculatePivotStyleMedium22Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium23, new CalculateStyleProperties(CalculatePivotStyleMedium23Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium24, new CalculateStyleProperties(CalculatePivotStyleMedium24Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium25, new CalculateStyleProperties(CalculatePivotStyleMedium25Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium26, new CalculateStyleProperties(CalculatePivotStyleMedium26Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium27, new CalculateStyleProperties(CalculatePivotStyleMedium27Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleMedium28, new CalculateStyleProperties(CalculatePivotStyleMedium28Properties));
			#endregion
			#region PivotStyleDark
			result.Add(PredefinedPivotStyleId.PivotStyleDark1, new CalculateStyleProperties(CalculatePivotStyleDark1Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark2, new CalculateStyleProperties(CalculatePivotStyleDark2Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark3, new CalculateStyleProperties(CalculatePivotStyleDark3Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark4, new CalculateStyleProperties(CalculatePivotStyleDark4Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark5, new CalculateStyleProperties(CalculatePivotStyleDark5Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark6, new CalculateStyleProperties(CalculatePivotStyleDark6Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark7, new CalculateStyleProperties(CalculatePivotStyleDark7Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark8, new CalculateStyleProperties(CalculatePivotStyleDark8Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark9, new CalculateStyleProperties(CalculatePivotStyleDark9Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark10, new CalculateStyleProperties(CalculatePivotStyleDark10Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark11, new CalculateStyleProperties(CalculatePivotStyleDark11Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark12, new CalculateStyleProperties(CalculatePivotStyleDark12Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark13, new CalculateStyleProperties(CalculatePivotStyleDark13Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark14, new CalculateStyleProperties(CalculatePivotStyleDark14Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark15, new CalculateStyleProperties(CalculatePivotStyleDark15Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark16, new CalculateStyleProperties(CalculatePivotStyleDark16Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark17, new CalculateStyleProperties(CalculatePivotStyleDark17Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark18, new CalculateStyleProperties(CalculatePivotStyleDark18Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark19, new CalculateStyleProperties(CalculatePivotStyleDark19Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark20, new CalculateStyleProperties(CalculatePivotStyleDark20Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark21, new CalculateStyleProperties(CalculatePivotStyleDark21Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark22, new CalculateStyleProperties(CalculatePivotStyleDark22Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark23, new CalculateStyleProperties(CalculatePivotStyleDark23Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark24, new CalculateStyleProperties(CalculatePivotStyleDark24Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark25, new CalculateStyleProperties(CalculatePivotStyleDark25Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark26, new CalculateStyleProperties(CalculatePivotStyleDark26Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark27, new CalculateStyleProperties(CalculatePivotStyleDark27Properties));
			result.Add(PredefinedPivotStyleId.PivotStyleDark28, new CalculateStyleProperties(CalculatePivotStyleDark28Properties));
			#endregion
			return result;
		}
		protected internal virtual void CalculatePivotStyleLight1Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight2Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4)), false);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(4)), false);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight3Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(5)), true);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight4Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(6)), true);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight5Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(7)), true);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight6Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(8)), true);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight7Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(9)), true);
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0)), ColorModelInfo.Create(new ThemeColorIndex(0)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight8Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight9Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight10Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight11Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight12Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight13Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight14Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.FirstColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight15Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight16Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight17Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight18Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight19Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight20Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight21Properties() {
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight22Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight23Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight24Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight25Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight26Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight27Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleLight28Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium1Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium2Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium3Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium4Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium5Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium6Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium7Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Double, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			AssignBorderInfo(TableStyle.SecondRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium8Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium9Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium10Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium11Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium12Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium13Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium14Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893)));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium15Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.0499893185216834), ColorModelInfo.Create(new ThemeColorIndex(0), -0.0499893185216834));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium16Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium17Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium18Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium19Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium20Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium21Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.ThirdRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1))));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium22Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium23Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium24Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium25Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium26Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium27Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleMedium28Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstColumnIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark1Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736), ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark2Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark3Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark4Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark5Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark6Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark7Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldValuesIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), false);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262));
			AssignFillInfo(TableStyle.PageFieldValuesIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9))));
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262)));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstSubtotalRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Horizontal, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), -0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark8Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark9Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.599993896298105)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark10Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.599993896298105)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark11Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.599993896298105)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark12Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.599993896298105)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark13Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.599993896298105)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark14Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(1), 0.249977111117893));
			AssignFillInfo(TableStyle.FirstSubtotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105), ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.599993896298105)));
			AssignBorderInfo(TableStyle.SecondRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.SecondColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.SecondColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.ThirdColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldLabelsIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262)));
			AssignBorderInfo(TableStyle.PageFieldValuesIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark15Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579), ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.249977111117893)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0), -0.349986266670736)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314)));
			AssignBorderInfo(TableStyle.FirstRowSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark16Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark17Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark18Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark19Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark20Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark21Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.HeaderRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.PageFieldLabelsIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.TotalRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1)), ColorModelInfo.Create(new ThemeColorIndex(1)));
			AssignFillInfo(TableStyle.FirstSubtotalColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstRowSubheadingIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstRowStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnStripeIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Left, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			borders.Add(BorderIndex.Right, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstSubtotalColumnIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419)));
			AssignBorderInfo(TableStyle.FirstColumnSubheadingIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark22Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0), -0.149998474074526), true);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(1)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(0), -0.499984740745262));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579), ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579), ColorModelInfo.Create(new ThemeColorIndex(0), -0.449995422223579));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262), ColorModelInfo.Create(new ThemeColorIndex(1), 0.499984740745262));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark23Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(4), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4)), ColorModelInfo.Create(new ThemeColorIndex(4)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(4), 0.399975585192419));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(4), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark24Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(5), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5)), ColorModelInfo.Create(new ThemeColorIndex(5)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(5), 0.399975585192419));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(5), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark25Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(6), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6)), ColorModelInfo.Create(new ThemeColorIndex(6)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(6), 0.399975585192419));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(6), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark26Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(7), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7)), ColorModelInfo.Create(new ThemeColorIndex(7)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(7), 0.399975585192419));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(7), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark27Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(8), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8)), ColorModelInfo.Create(new ThemeColorIndex(8)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(8), 0.399975585192419));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(8), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		protected internal virtual void CalculatePivotStyleDark28Properties() {
			AssignRunFontInfo(TableStyle.WholeTableIndex, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.TotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstHeaderCellIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(9), 0.799981688894314), false);
			AssignRunFontInfo(TableStyle.SecondSubtotalRowIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.FirstRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignRunFontInfo(TableStyle.SecondRowSubheadingIndex, ColorModelInfo.Create(new ThemeColorIndex(0)), true);
			AssignFillInfo(TableStyle.WholeTableIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9)), ColorModelInfo.Create(new ThemeColorIndex(9)));
			AssignFillInfo(TableStyle.HeaderRowIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.FirstColumnIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			AssignFillInfo(TableStyle.SecondRowStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419));
			AssignFillInfo(TableStyle.SecondColumnStripeIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419), ColorModelInfo.Create(new ThemeColorIndex(9), 0.399975585192419));
			AssignFillInfo(TableStyle.PageFieldLabelsIndex, XlPatternType.Solid, ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893), ColorModelInfo.Create(new ThemeColorIndex(9), -0.249977111117893));
			Dictionary<BorderIndex, BorderElementInfo> borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Vertical, BorderElementInfo.Create(XlBorderLineStyle.Thin, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.WholeTableIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Bottom, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.HeaderRowIndex, borders);
			borders = new Dictionary<BorderIndex, BorderElementInfo>();
			borders.Add(BorderIndex.Top, BorderElementInfo.Create(XlBorderLineStyle.Medium, ColorModelInfo.Create(new ThemeColorIndex(0))));
			AssignBorderInfo(TableStyle.TotalRowIndex, borders);
		}
		#endregion
		void CalculateNoneProperties() {
		}
		protected void AssignRunFontInfo(int elementIndex, ColorModelInfo colorInfo, bool bold) {
			fonts[elementIndex] = CreateRunFontInfo(colorInfo, bold);
			CalculateFontOptions(elementIndex);
		}
		RunFontInfo CreateRunFontInfo(ColorModelInfo colorInfo, bool bold) {
			RunFontInfo result = DocumentModel.GetDefaultRunFontInfo().Clone();
			result.Bold = bold;
			result.ColorIndex = ColorCache.GetItemIndex(colorInfo);
			return result;
		}
		void CalculateFontOptions(int elementIndex) {
			multiOptions[elementIndex].ApplyFontBold = true;
			multiOptions[elementIndex].ApplyFontColor = true;
		}
		protected void AssignFillInfo(int elementIndex, XlPatternType patternType, ColorModelInfo foreColorInfo, ColorModelInfo backColorInfo) {
			fills[elementIndex] = CreateFillInfo(patternType, foreColorInfo, backColorInfo);
			CalculateFillOptions(elementIndex);
		}
		FillInfo CreateFillInfo(XlPatternType patternType, ColorModelInfo foreColorInfo, ColorModelInfo backColorInfo) {
			FillInfo result = Cache.FillInfoCache.DefaultItem.Clone();
			result.PatternType = patternType;
			result.ForeColorIndex = ColorCache.GetItemIndex(foreColorInfo);
			result.BackColorIndex = ColorCache.GetItemIndex(backColorInfo);
			return result;
		}
		void CalculateFillOptions(int elementIndex) {
			multiOptions[elementIndex].ApplyFillPatternType = true;
			multiOptions[elementIndex].ApplyFillForeColor = true;
			multiOptions[elementIndex].ApplyFillBackColor = true;
		}
		protected void AssignBorderInfo(int elementIndex, Dictionary<BorderIndex, BorderElementInfo> borderCollection) {
			borders[elementIndex] = CreateBorderInfo(elementIndex, borderCollection);
		}
		BorderInfo CreateBorderInfo(int elementIndex, Dictionary<BorderIndex, BorderElementInfo> borderCollection) {
			BorderInfo result = Cache.BorderInfoCache.DefaultItem.Clone();
			foreach (BorderIndex index in borderCollection.Keys) {
				BorderElementInfo border = borderCollection[index];
				if (index == BorderIndex.Left) {
					result.LeftLineStyle = border.LineStyle;
					result.LeftColorIndex = ColorCache.GetItemIndex(border.ColorInfo);
					borderOptions[elementIndex].ApplyLeftLineStyle = !border.IsDefaultLineStyle;
					borderOptions[elementIndex].ApplyLeftColor = true;
				}
				if (index == BorderIndex.Right) {
					result.RightLineStyle = border.LineStyle;
					result.RightColorIndex = ColorCache.GetItemIndex(border.ColorInfo);
					borderOptions[elementIndex].ApplyRightLineStyle = !border.IsDefaultLineStyle;
					borderOptions[elementIndex].ApplyRightColor = true;
				}
				if (index == BorderIndex.Top) {
					result.TopLineStyle = border.LineStyle;
					result.TopColorIndex = ColorCache.GetItemIndex(border.ColorInfo);
					borderOptions[elementIndex].ApplyTopLineStyle = !border.IsDefaultLineStyle;
					borderOptions[elementIndex].ApplyTopColor = true;
				}
				if (index == BorderIndex.Bottom) {
					result.BottomLineStyle = border.LineStyle;
					result.BottomColorIndex = ColorCache.GetItemIndex(border.ColorInfo);
					borderOptions[elementIndex].ApplyBottomLineStyle = !border.IsDefaultLineStyle;
					borderOptions[elementIndex].ApplyBottomColor = true;
				}
				if (index == BorderIndex.Horizontal) {
					result.HorizontalLineStyle = border.LineStyle;
					result.HorizontalColorIndex = ColorCache.GetItemIndex(border.ColorInfo);
					borderOptions[elementIndex].ApplyHorizontalLineStyle = !border.IsDefaultLineStyle;
					borderOptions[elementIndex].ApplyHorizontalColor = true;
				}
				if (index == BorderIndex.Vertical) {
					result.VerticalLineStyle = border.LineStyle;
					result.VerticalColorIndex = ColorCache.GetItemIndex(border.ColorInfo);
					borderOptions[elementIndex].ApplyVerticalLineStyle = !border.IsDefaultLineStyle;
					borderOptions[elementIndex].ApplyVerticalColor = true;
				}
			}
			return result;
		}
		void CalculateDefaultInfoes() {
			for (int i = 0; i < TableStyle.ElementsCount; i++) {
				fonts[i] = RunFontInfo.CreateDefault();
				fills[i] = new FillInfo();
				borders[i] = new BorderInfo();
				multiOptions[i] = new MultiOptionsInfo();
				borderOptions[i] = new BorderOptionsInfo();
			}
		}
		public void CalculateProperties() {
			CalculateDefaultInfoes();
			CalculateStyleProperties calculator = TryGetCalculators();
			if (calculator != null) {
				calculator();
				AssignFormatInfo();
			}
		}
		CalculateStyleProperties TryGetCalculators() {
			if (style.IsTableStyle)
				return GetCalculatorsCore(GetTableStyleDelegateCollection(), (PredefinedTableStyleId)style.Name.Id.Value);
			if (style.IsPivotStyle)
				return GetCalculatorsCore(GetPivotStyleDelegateCollection(), (PredefinedPivotStyleId)style.Name.Id.Value);
			return null;
		}
		CalculateStyleProperties GetCalculatorsCore<TPredefinedStyleId>(Dictionary<TPredefinedStyleId, CalculateStyleProperties> calculators, TPredefinedStyleId id) {
			if (calculators.ContainsKey(id))
				return calculators[id];
			return CalculateNoneProperties;
		}
		void AssignFormatInfo() {
			for (int i = 0; i < TableStyle.ElementsCount; i++) {
				int formatIndex = GetTableStyleElementFormatIndex(i, fonts[i], fills[i], borders[i]);
				style.AssignFormatIndex(i, formatIndex);
			}
		}
		int GetTableStyleElementFormatIndex(int elementIndex, RunFontInfo fontInfo, FillInfo fillInfo, BorderInfo borderInfo) {
			DifferentialFormat format = new DifferentialFormat(DocumentModel);
			format.AssignFontIndex(Cache.FontInfoCache.GetItemIndex(fontInfo));
			format.AssignFillIndex(Cache.FillInfoCache.GetItemIndex(fillInfo));
			format.AssignBorderIndex(Cache.BorderInfoCache.GetItemIndex(borderInfo));
			format.AssignMultiOptionsIndex(multiOptions[elementIndex].PackedValues);
			format.AssignBorderOptionsIndex(borderOptions[elementIndex].PackedValues);
			int formatIndex = Cache.CellFormatCache.GetItemIndex(format);
			TableStyleElementFormat tableStyleElementFormat = new TableStyleElementFormat(DocumentModel);
			tableStyleElementFormat.AssignDifferentialFormatIndex(formatIndex);
			return Cache.TableStyleElementFormatCache.GetItemIndex(tableStyleElementFormat);
		}
	}
	#endregion
} 
