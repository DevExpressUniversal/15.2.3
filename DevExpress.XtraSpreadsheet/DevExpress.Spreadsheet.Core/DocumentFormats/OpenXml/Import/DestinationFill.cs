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
using System.Xml;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Export.Xl;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.XtraSpreadsheet.Export.OpenXml;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region FillsDestination
	public class FillsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("fill", OnFill);
			return result;
		}
		static Destination OnFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new FillDestination(importer);
		}
		#endregion
		public FillsDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region FillDestination
	public class FillDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("gradientFill", OnGradientFill);
			result.Add("patternFill", OnPatternFill);
			return result;
		}
		static FillDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (FillDestination)importer.PeekDestination();
		}
		static Destination OnGradientFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).hasGradientFill = true;
			return new GradientFillDestination(importer);
		}
		static Destination OnPatternFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GetThis(importer).hasPatternFill = true;
			return new PatternFillDestination(importer);
		}
		#endregion
		bool hasPatternFill = false;
		bool hasGradientFill = false;
		public FillDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected bool HasPatternFill { get { return hasPatternFill; } set { hasPatternFill = value; } }
		protected bool HasGradientFill { get { return hasGradientFill; } set { hasGradientFill = value; } }
		public override void ProcessElementClose(XmlReader reader) {
			if (!hasPatternFill && !hasGradientFill)
				Importer.StyleSheet.RegisterPatternFill(new FillInfo());
		}
	}
	#endregion
	#region PatternFillDestination
	public class PatternFillDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("bgColor", OnBackgroundColor);
			result.Add("fgColor", OnForegroundColor);
			return result;
		}
		internal static Dictionary<XlPatternType, string> patternTypeTable = CreatePatternTypeTable();
		static Dictionary<XlPatternType, string> CreatePatternTypeTable() {
			Dictionary<XlPatternType, string> result = new Dictionary<XlPatternType, string>();
			result.Add(XlPatternType.None, "none");
			result.Add(XlPatternType.Solid, "solid");
			result.Add(XlPatternType.MediumGray, "mediumGray");
			result.Add(XlPatternType.DarkGray, "darkGray");
			result.Add(XlPatternType.LightGray, "lightGray");
			result.Add(XlPatternType.DarkHorizontal, "darkHorizontal");
			result.Add(XlPatternType.DarkVertical, "darkVertical");
			result.Add(XlPatternType.DarkDown, "darkDown");
			result.Add(XlPatternType.DarkUp, "darkUp");
			result.Add(XlPatternType.DarkGrid, "darkGrid");
			result.Add(XlPatternType.DarkTrellis, "darkTrellis");
			result.Add(XlPatternType.LightHorizontal, "lightHorizontal");
			result.Add(XlPatternType.LightVertical, "lightVertical");
			result.Add(XlPatternType.LightDown, "lightDown");
			result.Add(XlPatternType.LightUp, "lightUp");
			result.Add(XlPatternType.LightGrid, "lightGrid");
			result.Add(XlPatternType.LightTrellis, "lightTrellis");
			result.Add(XlPatternType.Gray125, "gray125");
			result.Add(XlPatternType.Gray0625, "gray0625");
			return result;
		}
		static PatternFillDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PatternFillDestination)importer.PeekDestination();
		}
		static Destination OnBackgroundColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).backColorInfo);
		}
		static Destination OnForegroundColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new ColorDestination(importer, GetThis(importer).foreColorInfo);
		}
		#endregion
		readonly FillInfo fillInfo;
		readonly ColorModelInfo foreColorInfo;
		readonly ColorModelInfo backColorInfo;
		public PatternFillDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.fillInfo = new FillInfo();
			this.foreColorInfo = new ColorModelInfo();
			this.backColorInfo = new ColorModelInfo();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal ColorModelInfo ForeColorInfo { get { return foreColorInfo; } }
		protected internal ColorModelInfo BackColorInfo { get { return backColorInfo; } }
		protected internal FillInfo FillInfo { get { return fillInfo; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			FillInfo.PatternType = Importer.GetWpEnumValue<XlPatternType>(reader, "patternType", patternTypeTable, XlPatternType.None);
		}
		public override void ProcessElementClose(XmlReader reader) {
			CalculateColorIndexes();
			Importer.StyleSheet.RegisterPatternFill(fillInfo);
		}
		protected void CalculateColorIndexes() {
			ColorModelInfoCache cache = Importer.DocumentModel.Cache.ColorModelInfoCache;
			FillInfo.ForeColorIndex = cache.GetItemIndex(ForeColorInfo);
			FillInfo.BackColorIndex = cache.GetItemIndex(BackColorInfo);
		}
	}
	#endregion
	#region GradientFillDestination
	public class GradientFillDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("stop", OnGradientStop);
			return result;
		}
		static GradientFillDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GradientFillDestination)importer.PeekDestination();
		}
		static Destination OnGradientStop(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new GradientStopDestination(importer, GetThis(importer).Stops);
		}
		#endregion
		readonly GradientFillInfo info;
		readonly GradientStopInfoCollection stops;
		public GradientFillDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
			this.info = new GradientFillInfo();
			this.stops = new GradientStopInfoCollection(importer.DocumentModel);
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected virtual GradientStopInfoCollection Stops { get { return stops; } }
		protected internal GradientFillInfo Info { get { return info; } }
		public override void ProcessElementOpen(XmlReader reader) {
			info.Degree = Importer.GetWpDoubleValue(reader, "degree", GradientFillInfo.DefaultDegree);
			info.Type = Importer.GetWpEnumValue(reader, "type", OpenXmlExporter.GradientFillTypeTable, GradientFillInfo.DefaultType);
			info.Top = (float)Importer.GetWpDoubleValue(reader, "top", GradientFillInfo.DefaultConvergenceValue);
			info.Bottom = (float)Importer.GetWpDoubleValue(reader, "bottom", GradientFillInfo.DefaultConvergenceValue);
			info.Left = (float)Importer.GetWpDoubleValue(reader, "left", GradientFillInfo.DefaultConvergenceValue);
			info.Right = (float)Importer.GetWpDoubleValue(reader, "right", GradientFillInfo.DefaultConvergenceValue);
		}
		public override void ProcessElementClose(XmlReader reader) {
			Importer.StyleSheet.RegisterGradientFill(info, stops);
		}
	}
	#endregion
	#region GradientStopDestination
	public class GradientStopDestination : ElementDestination<SpreadsheetMLBaseImporter> {
	#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("color", OnColor);
			return result;
		}
		static GradientStopDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (GradientStopDestination)importer.PeekDestination();
		}
		static Destination OnColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			GradientStopDestination destination = GetThis(importer);
			destination.hasColor = true;
			return new ColorDestination(importer, destination.color);
		}
		#endregion
		readonly GradientStopInfo stop;
		readonly GradientStopInfoCollection stops;
		bool hasColor = false;
		ColorModelInfo color;
		public GradientStopDestination(SpreadsheetMLBaseImporter importer, GradientStopInfoCollection stops)
			: base(importer) {
			this.stops = stops;
			this.stop = new GradientStopInfo();
			this.color = new ColorModelInfo();
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			double position = Importer.GetWpDoubleValue(reader, "position", Double.MinValue);
			if (position == Double.MinValue)
				Importer.ThrowInvalidFile("Invalid gradient stop position");
			stop.Position = position;
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (!hasColor)
				Importer.ThrowInvalidFile("Gradient color is not specified");
			DocumentModel documentModel = Importer.DocumentModel;
			stop.SetColorIndex(documentModel, color);
			stops.AddCore(documentModel.Cache.GradientStopInfoCache.GetItemIndex(stop));
		}
	}
	#endregion
	#region ColorDestination
	public class ColorDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly ColorModelInfo colorInfo;
		public ColorDestination(SpreadsheetMLBaseImporter importer, ColorModelInfo colorInfo)
			: base(importer) {
			Guard.ArgumentNotNull(colorInfo, "colorInfo");
			this.colorInfo = colorInfo;
		}
		protected internal ColorModelInfo ColorInfo { get { return colorInfo; } }
		public override void ProcessElementOpen(XmlReader reader) {
			bool? auto = Importer.GetWpSTOnOffNullValue(reader, "auto");
			if (auto.HasValue)
				colorInfo.Auto = auto.Value;
			int index = Importer.GetWpSTIntegerValue(reader, "indexed", Int32.MinValue);
			if (Importer.DocumentModel.StyleSheet.Palette.IsValidColorIndex(index)) 
				colorInfo.ColorIndex = index;
			Color rgb = Importer.GetWpSTColorValue(reader, "rgb", DXColor.Empty);
			if (rgb != DXColor.Empty)
				colorInfo.Rgb = rgb;
			int theme = Importer.GetWpSTIntegerValue(reader, "theme", Int32.MinValue);
			if (theme != Int32.MinValue)
				colorInfo.Theme = new ThemeColorIndex(theme);
			double tint = Importer.GetWpDoubleValue(reader, "tint", double.MinValue);
			if (tint != double.MinValue)
				colorInfo.Tint = tint;
		}
	}
	#endregion
	#region DifferentialFillDestination
	public class DifferentialFillDestination : FillDestination {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("gradientFill", OnGradientFill);
			result.Add("patternFill", OnPatternFill);
			return result;
		}
		static DifferentialFillDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DifferentialFillDestination)importer.PeekDestination();
		}
		static Destination OnPatternFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialFillDestination destination = GetThis(importer);
			destination.HasPatternFill = true;
			return new DifferentialPatternFillDestination(importer, destination.DifferentialFormat);
		}
		static Destination OnGradientFill(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialFillDestination destination = GetThis(importer);
			GetThis(importer).HasGradientFill = true;
			return new DifferentialGradientFillDestination(importer, destination.DifferentialFormat);
		}
		#endregion
		readonly DifferentialFormat differentialFormat;
		public DifferentialFillDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer) {
			Guard.ArgumentNotNull(differentialFormat, "differentialFormat");
			this.differentialFormat = differentialFormat;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DifferentialFormat DifferentialFormat { get { return differentialFormat; } }
		#endregion
	}
	#endregion
	#region DifferentialPatternFillDestination
	public class DifferentialPatternFillDestination : PatternFillDestination {
		#region Static Members
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("bgColor", OnBackgroundColor);
			result.Add("fgColor", OnForegroundColor);
			return result;
		}
		static DifferentialPatternFillDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (DifferentialPatternFillDestination)importer.PeekDestination();
		}
		static Destination OnBackgroundColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialPatternFillDestination destination = GetThis(importer);
			destination.applyBackColor = true;
			return new ColorDestination(importer, destination.BackColorInfo);
		}
		static Destination OnForegroundColor(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			DifferentialPatternFillDestination destination = GetThis(importer);
			destination.applyForeColor = true;
			return new ColorDestination(importer, destination.ForeColorInfo);
		}
		#endregion
		#region Fields
		readonly DifferentialFormat differentialFormat;
		XlPatternType? patternType;
		bool applyBackColor;
		bool applyForeColor;
		#endregion
		public DifferentialPatternFillDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer) {
			this.differentialFormat = differentialFormat;
		}
		#region Properties
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal DifferentialFormat DifferentialFormat { get { return differentialFormat; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			patternType = Importer.GetWpEnumOnOffNullValue(reader, "patternType", patternTypeTable);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (patternType.HasValue)
				FillInfo.PatternType = patternType.Value;
			CalculateColorIndexes();
			int formatIndex = Importer.DocumentModel.Cache.FillInfoCache.AddItem(FillInfo);
			DifferentialFormat.AssignFillIndex(formatIndex);
			CalculateDifferentialFormatFillOptions();
		}
		void CalculateDifferentialFormatFillOptions() {
			MultiOptionsInfo info = DifferentialFormat.MultiOptionsInfo.Clone();
			info.ApplyFillPatternType = patternType.HasValue;
			info.ApplyFillBackColor = applyBackColor;
			info.ApplyFillForeColor = applyForeColor;
			DifferentialFormat.AssignMultiOptionsIndex(info.PackedValues);
		}
	}
	#endregion
	#region DifferentialGradientFillDestination
	public class DifferentialGradientFillDestination : GradientFillDestination {
		readonly DifferentialFormat differentialFormat;
		public DifferentialGradientFillDestination(SpreadsheetMLBaseImporter importer, DifferentialFormat differentialFormat)
			: base(importer) {
			this.differentialFormat = differentialFormat;
		}
		protected internal DifferentialFormat DifferentialFormat { get { return differentialFormat; } }
		protected override GradientStopInfoCollection Stops { get { return differentialFormat.GradientStopInfoCollection; } }
		public override void ProcessElementClose(XmlReader reader) {
			int formatIndex = Importer.DocumentModel.Cache.GradientFillInfoCache.AddItem(Info);
			differentialFormat.AssignGradientFillInfoIndex(formatIndex);
			differentialFormat.AssignCellFormatFlagsIndex(differentialFormat.CellFormatFlagsIndex + CellFormatFlagsInfo.MaskFillType);
		}
	}
	#endregion
}
