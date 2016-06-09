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
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region BordersDestination
	public class BordersDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("border", OnBorder);
			return result;
		}
		static Destination OnBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BorderDestination(importer);
		}
		#endregion
		public BordersDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
	#region BorderDestination
	public class BorderDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("left", OnLeftBorder);
			result.Add("right", OnRightBorder);
			result.Add("top", OnTopBorder);
			result.Add("bottom", OnBottomBorder);
			result.Add("diagonal", OnDiagonalBorder);
			result.Add("horizontal", OnHorizontalBorder);
			result.Add("vertical", OnVerticalBorder);
			return result;
		}
		static BorderDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BorderDestination)importer.PeekDestination();
		}
		static Destination OnLeftBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.LeftBorderAccessor);
		}
		static Destination OnRightBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.RightBorderAccessor);
		}
		static Destination OnTopBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.TopBorderAccessor);
		}
		static Destination OnBottomBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			 return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.BottomBorderAccessor);
		}
		static Destination OnDiagonalBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			 return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.DiagonalBorderAccessor);
		}
		static Destination OnHorizontalBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			 return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.HorizontalBorderAccessor);
		}
		static Destination OnVerticalBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new BorderItemDestination(importer, GetThis(importer).BorderInfo, BorderInfo.VerticalBorderAccessor);
		}
		#endregion
		readonly BorderInfo borderInfo;
		public BorderDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.borderInfo = new BorderInfo();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal BorderInfo BorderInfo { get { return borderInfo; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			BorderInfo.DiagonalUp = Importer.GetOnOffValue(reader, "diagonalUp", false);
			BorderInfo.DiagonalDown = Importer.GetOnOffValue(reader, "diagonalDown", false);
			BorderInfo.Outline = Importer.GetOnOffValue(reader, "outline", true);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.StyleSheet.RegisterBorder(BorderInfo);
		}
	}
	#endregion
	#region BorderItemDestination
	public class BorderItemDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("color", OnBorderColor);
			return result;
		}
		internal static Dictionary<XlBorderLineStyle, string> borderLineStyleTable = CreateBorderLineStyleTable();
		static Dictionary<XlBorderLineStyle, string> CreateBorderLineStyleTable() {
			Dictionary<XlBorderLineStyle, string> result = new Dictionary<XlBorderLineStyle, string>();
			result.Add(XlBorderLineStyle.None, "none");
			result.Add(XlBorderLineStyle.Thin, "thin");
			result.Add(XlBorderLineStyle.Medium, "medium");
			result.Add(XlBorderLineStyle.Dashed, "dashed");
			result.Add(XlBorderLineStyle.Dotted, "dotted");
			result.Add(XlBorderLineStyle.Thick, "thick");
			result.Add(XlBorderLineStyle.Double, "double");
			result.Add(XlBorderLineStyle.Hair, "hair");
			result.Add(XlBorderLineStyle.MediumDashed, "mediumDashed");
			result.Add(XlBorderLineStyle.DashDot, "dashDot");
			result.Add(XlBorderLineStyle.MediumDashDot, "mediumDashDot");
			result.Add(XlBorderLineStyle.DashDotDot, "dashDotDot");
			result.Add(XlBorderLineStyle.MediumDashDotDot, "mediumDashDotDot");
			result.Add(XlBorderLineStyle.SlantDashDot, "slantDashDot");
			return result;
		}
		static BorderItemDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (BorderItemDestination)importer.PeekDestination();
		}
		static Destination OnBorderColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			BorderItemDestination thisObject = GetThis(importer);
			return new BorderColorDestination(importer, thisObject.BorderInfo, thisObject.BorderItemAccessor);
		}
		#endregion
		readonly BorderInfo borderInfo;
		readonly BorderInfoBorderAccessor borderItemAccessor;
		public BorderItemDestination(SpreadsheetMLBaseImporter importer, BorderInfo borderInfo, BorderInfoBorderAccessor borderItemAccessor)
			: base(importer) {
			Guard.ArgumentNotNull(borderInfo, "borderInfo");
			Guard.ArgumentNotNull(borderItemAccessor, "borderItemAccessor");
			this.borderInfo = borderInfo;
			this.borderItemAccessor = borderItemAccessor;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal BorderInfo BorderInfo { get { return borderInfo; } }
		protected internal BorderInfoBorderAccessor BorderItemAccessor { get { return borderItemAccessor; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			XlBorderLineStyle lineStyle = Importer.GetWpEnumValue<XlBorderLineStyle>(reader, "style", borderLineStyleTable, XlBorderLineStyle.None);
			borderItemAccessor.SetLineStyle(BorderInfo, lineStyle);
		}
	}
	#endregion
	#region BorderColorDestination
	public class BorderColorDestination : ColorDestination {
		readonly BorderInfo borderInfo;
		readonly BorderInfoBorderAccessor borderItemAccessor;
		public BorderColorDestination(SpreadsheetMLBaseImporter importer, BorderInfo borderInfo, BorderInfoBorderAccessor borderItemAccessor)
			: base(importer, new ColorModelInfo()) {
			Guard.ArgumentNotNull(borderInfo, "border");
			Guard.ArgumentNotNull(borderItemAccessor, "borderItemAccessor");
			this.borderInfo = borderInfo;
			this.borderItemAccessor = borderItemAccessor;
		}
		public override void ProcessElementClose(XmlReader reader) {
			int colorIndex = Importer.RegisterColor(ColorInfo);
			borderItemAccessor.SetColorIndex(borderInfo, colorIndex);
		}
	}
	#endregion
	#region DifferentialBorderDestination
	public class DifferentialBorderDestination : BorderDestination {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("left", OnLeftBorder);
			result.Add("right", OnRightBorder);
			result.Add("top", OnTopBorder);
			result.Add("bottom", OnBottomBorder);
			result.Add("diagonal", OnDiagonalBorder);
			result.Add("horizontal", OnHorizontalBorder);
			result.Add("vertical", OnVerticalBorder);
			return result;
		}
		static DifferentialBorderDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DifferentialBorderDestination)importer.PeekDestination();
		}
		static Destination OnLeftBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.LeftBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		static Destination OnRightBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.RightBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		static Destination OnTopBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.TopBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		static Destination OnBottomBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.BottomBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		static Destination OnDiagonalBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.DiagonalBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		static Destination OnHorizontalBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.HorizontalBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		static Destination OnVerticalBorder(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderDestination destination = GetThis(importer);
			return new DifferentialBorderItemDestination(importer, destination.BorderInfo, BorderInfo.VerticalBorderAccessor, destination.ImportOptionsBorderInfo);
		}
		#endregion
		readonly DifferentialFormat differentialFormat;
		readonly ImportOptionsBorderInfo importOptionsBorderInfo;
		public DifferentialBorderDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer) {
			this.differentialFormat = differentialFormat;
			this.importOptionsBorderInfo = new ImportOptionsBorderInfo();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DifferentialFormat DifferentialFormat { get { return differentialFormat; } }
		protected internal ImportOptionsBorderInfo ImportOptionsBorderInfo { get { return importOptionsBorderInfo; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			bool? diagonalUp = Importer.GetWpSTOnOffNullValue(reader, "diagonalUp");
			ImportOptionsBorderInfo.ApplyDiagonalUp = diagonalUp.HasValue;
			if (diagonalUp.HasValue && diagonalUp.Value)
				BorderInfo.DiagonalUp = true;
			bool? diagonalDown = Importer.GetWpSTOnOffNullValue(reader, "diagonalDown");
			ImportOptionsBorderInfo.ApplyDiagonalDown = diagonalDown.HasValue;
			if (diagonalDown.HasValue && diagonalDown.Value)
				BorderInfo.DiagonalDown = true;
			bool? outline = Importer.GetWpSTOnOffNullValue(reader, "outline");
			ImportOptionsBorderInfo.ApplyOutline = outline.HasValue;
			if (outline.HasValue && !outline.Value)
				BorderInfo.Outline = false;
		}
		public override void ProcessElementClose(XmlReader reader) {
			int formatIndex = Importer.DocumentModel.Cache.BorderInfoCache.AddItem(BorderInfo);
			DifferentialFormat.AssignBorderIndex(formatIndex);
			CalculateDifferentialFormatBorderOptions();
		}
		void CalculateDifferentialFormatBorderOptions() {
			BorderOptionsInfo info = DifferentialFormat.BorderOptionsInfo.Clone();
			info.ApplyLeftLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.LeftBorderAccessor.BorderIndex];
			info.ApplyRightLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.RightBorderAccessor.BorderIndex];
			info.ApplyTopLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.TopBorderAccessor.BorderIndex];
			info.ApplyBottomLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.BottomBorderAccessor.BorderIndex];
			info.ApplyHorizontalLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.HorizontalBorderAccessor.BorderIndex];
			info.ApplyVerticalLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.VerticalBorderAccessor.BorderIndex];
			info.ApplyDiagonalLineStyle = ImportOptionsBorderInfo.ApplyLineStyles[BorderInfo.DiagonalBorderAccessor.BorderIndex];
			info.ApplyLeftColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.LeftBorderAccessor.BorderIndex];
			info.ApplyRightColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.RightBorderAccessor.BorderIndex];
			info.ApplyTopColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.TopBorderAccessor.BorderIndex];
			info.ApplyBottomColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.BottomBorderAccessor.BorderIndex];
			info.ApplyHorizontalColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.HorizontalBorderAccessor.BorderIndex];
			info.ApplyVerticalColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.VerticalBorderAccessor.BorderIndex];
			info.ApplyDiagonalColor = ImportOptionsBorderInfo.ApplyColorIndexes[BorderInfo.DiagonalBorderAccessor.BorderIndex];
			info.ApplyDiagonalUp = ImportOptionsBorderInfo.ApplyDiagonalUp;
			info.ApplyDiagonalDown = ImportOptionsBorderInfo.ApplyDiagonalDown;
			info.ApplyOutline = ImportOptionsBorderInfo.ApplyOutline;
			DifferentialFormat.AssignBorderOptionsIndex(info.PackedValues);
		}
	}
	#endregion
	#region DifferentialBorderItemDestination
	public class DifferentialBorderItemDestination : BorderItemDestination {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("color", OnBorderColor);
			return result;
		}
		static DifferentialBorderItemDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DifferentialBorderItemDestination)importer.PeekDestination();
		}
		static Destination OnBorderColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialBorderItemDestination destination = GetThis(importer);
			BorderInfoBorderAccessor borderItemAccessor = destination.BorderItemAccessor;
			destination.ImportOptionsBorderInfo.ApplyColorIndexes[borderItemAccessor.BorderIndex] = true;
			return new BorderColorDestination(importer, destination.BorderInfo, borderItemAccessor);
		}
		#endregion
		readonly ImportOptionsBorderInfo importOptionsBorderInfo;
		public DifferentialBorderItemDestination(SpreadsheetMLBaseImporter importer, BorderInfo borderInfo, BorderInfoBorderAccessor borderItemAccessor, ImportOptionsBorderInfo importOptionsBorderInfo)
			: base(importer, borderInfo, borderItemAccessor) {
			this.importOptionsBorderInfo = importOptionsBorderInfo;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal ImportOptionsBorderInfo ImportOptionsBorderInfo { get { return importOptionsBorderInfo; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			XlBorderLineStyle? lineStyle = Importer.GetWpEnumOnOffNullValue<XlBorderLineStyle>(reader, "style", borderLineStyleTable);
			ImportOptionsBorderInfo.ApplyLineStyles[BorderItemAccessor.BorderIndex] = true;
			if (lineStyle.HasValue)
				BorderItemAccessor.SetLineStyle(BorderInfo, lineStyle.Value);
		}
	}
	#endregion
}
