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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Xml;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Native {
	public interface IChartAppearance {
		WholeChartAppearance WholeChartAppearance { get; }
		LegendAppearance LegendAppearance { get; }
		XYDiagramAppearance XYDiagramAppearance { get; }
		RadarDiagramAppearance RadarDiagramAppearance { get; }
		XYDiagram3DAppearance XYDiagram3DAppearance { get; }
		ConstantLineAppearance ConstantLineAppearance { get; }
		StripAppearance StripAppearance { get; }
		TextAnnotationAppearance TextAnnotationAppearance { get; }
		ImageAnnotationAppearance ImageAnnotationAppearance { get; }
		BarSeriesViewAppearance BarSeriesViewAppearance { get; }
		AreaSeriesViewAppearance AreaSeriesViewAppearance { get; }
		PieSeriesViewAppearance PieSeriesViewAppearance { get; }
		FunnelSeriesViewAppearance FunnelSeriesViewAppearance { get; }
		Bar3DSeriesViewAppearance Bar3DSeriesViewAppearance { get; }
		Pie3DSeriesViewAppearance Pie3DSeriesViewAppearance { get; }
		MarkerAppearance MarkerAppearance { get; }
		SeriesLabel2DAppearance SeriesLabel2DAppearance { get; }
		SeriesLabel3DAppearance SeriesLabel3DAppearance { get; }
		ScrollBarAppearance ScrollBarAppearance { get; }
		void ReadFromXml();
	}
}
namespace DevExpress.XtraCharts {
	public class ChartAppearance : IChartAppearance {
		readonly bool isDefault;
		readonly ChartStringId nameID;
		string paletteName = String.Empty;
		string indicatorsPaletteName = String.Empty;
		readonly string serializableName = String.Empty;
		readonly string resourceName;
		readonly WholeChartAppearance wholeChartAppearance = new WholeChartAppearance();
		readonly LegendAppearance legendAppearance = new LegendAppearance();
		readonly XYDiagramAppearance xyDiagramAppearance = new XYDiagramAppearance();
		readonly RadarDiagramAppearance radarDiagramAppearance = new RadarDiagramAppearance();
		readonly XYDiagram3DAppearance xyDiagram3DAppearance = new XYDiagram3DAppearance();
		readonly ConstantLineAppearance constantLineAppearance = new ConstantLineAppearance();
		readonly StripAppearance stripAppearance = new StripAppearance();
		readonly TextAnnotationAppearance textAnnotationAppearance = new TextAnnotationAppearance();
		readonly ImageAnnotationAppearance imageAnnotationAppearance = new ImageAnnotationAppearance();
		readonly BarSeriesViewAppearance barSeriesViewAppearance = new BarSeriesViewAppearance();
		readonly AreaSeriesViewAppearance areaSeriesViewAppearance = new AreaSeriesViewAppearance();
		readonly PieSeriesViewAppearance pieSeriesViewAppearance = new PieSeriesViewAppearance();
		readonly FunnelSeriesViewAppearance funnelSeriesViewAppearance = new FunnelSeriesViewAppearance();
		readonly Bar3DSeriesViewAppearance bar3DSeriesViewAppearance = new Bar3DSeriesViewAppearance();
		readonly Pie3DSeriesViewAppearance pie3DSeriesViewAppearance = new Pie3DSeriesViewAppearance();
		readonly MarkerAppearance markerAppearance = new MarkerAppearance();
		readonly SeriesLabel2DAppearance seriesLabel2DAppearance = new SeriesLabel2DAppearance();
		readonly SeriesLabel3DAppearance seriesLabel3DAppearance = new SeriesLabel3DAppearance();
		readonly ScrollBarAppearance scrollBarAppearance = new ScrollBarAppearance();
		#region IChartAppearance Members
		WholeChartAppearance IChartAppearance.WholeChartAppearance { get { return wholeChartAppearance; } }
		LegendAppearance IChartAppearance.LegendAppearance { get { return legendAppearance; } }
		XYDiagramAppearance IChartAppearance.XYDiagramAppearance { get { return xyDiagramAppearance; } }
		RadarDiagramAppearance IChartAppearance.RadarDiagramAppearance { get { return radarDiagramAppearance; } }
		XYDiagram3DAppearance IChartAppearance.XYDiagram3DAppearance { get { return xyDiagram3DAppearance; } }
		ConstantLineAppearance IChartAppearance.ConstantLineAppearance { get { return constantLineAppearance; } }
		StripAppearance IChartAppearance.StripAppearance { get { return stripAppearance; } }
		TextAnnotationAppearance IChartAppearance.TextAnnotationAppearance { get { return textAnnotationAppearance; } }
		ImageAnnotationAppearance IChartAppearance.ImageAnnotationAppearance { get { return imageAnnotationAppearance; } }
		BarSeriesViewAppearance IChartAppearance.BarSeriesViewAppearance { get { return barSeriesViewAppearance; } }
		AreaSeriesViewAppearance IChartAppearance.AreaSeriesViewAppearance { get { return areaSeriesViewAppearance; } }
		PieSeriesViewAppearance IChartAppearance.PieSeriesViewAppearance { get { return pieSeriesViewAppearance; } }
		FunnelSeriesViewAppearance IChartAppearance.FunnelSeriesViewAppearance { get { return funnelSeriesViewAppearance; } }
		Bar3DSeriesViewAppearance IChartAppearance.Bar3DSeriesViewAppearance { get { return bar3DSeriesViewAppearance; } }
		Pie3DSeriesViewAppearance IChartAppearance.Pie3DSeriesViewAppearance { get { return pie3DSeriesViewAppearance; } }
		MarkerAppearance IChartAppearance.MarkerAppearance { get { return markerAppearance; } }
		SeriesLabel2DAppearance IChartAppearance.SeriesLabel2DAppearance { get { return seriesLabel2DAppearance; } }
		SeriesLabel3DAppearance IChartAppearance.SeriesLabel3DAppearance { get { return seriesLabel3DAppearance; } }
		ScrollBarAppearance IChartAppearance.ScrollBarAppearance { get { return scrollBarAppearance; } }
		void IChartAppearance.ReadFromXml() {
			ReadFromXml();
		}
		#endregion
		internal string SerializableName { get { return serializableName; } }
		public string Name { get { return ChartLocalizer.GetString(nameID); } }
		public string PaletteName { get { return paletteName; } }
		public string IndicatorsPaletteName { get { return indicatorsPaletteName; } }
		public bool IsDefault { get { return isDefault; } }
		internal ChartAppearance(ChartStringId nameID, string serializableName, string resourceName) {
			this.nameID = nameID;
			this.serializableName = serializableName;
			this.resourceName = resourceName;
			ReadFromXml();
		}
		internal ChartAppearance(ChartStringId nameID, string serializationName, string resourceName, bool isDefault)
			: this(nameID, serializationName, resourceName) {
			this.isDefault = isDefault;
		}
		void ReadFromXml() {
			Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DevExpress.XtraCharts.Appearance.Data." + resourceName);
			TextReader reader = new StreamReader(stream);			
			try {
				XmlReader xmlReader = new XmlTextReader(reader);
				try {
					xmlReader.ReadStartElement("Appearance");
						paletteName = xmlReader.ReadElementString("PaletteName");
						indicatorsPaletteName = xmlReader.ReadElementString("IndicatorsPaletteName");
						wholeChartAppearance.ReadFromXml(xmlReader);
						legendAppearance.ReadFromXml(xmlReader);
						xyDiagramAppearance.ReadFromXml(xmlReader);
						radarDiagramAppearance.ReadFromXml(xmlReader);
						xyDiagram3DAppearance.ReadFromXml(xmlReader);
						constantLineAppearance.ReadFromXml(xmlReader);
						stripAppearance.ReadFromXml(xmlReader);
						textAnnotationAppearance.ReadFromXml(xmlReader);
						imageAnnotationAppearance.ReadFromXml(xmlReader);
						barSeriesViewAppearance.ReadFromXml(xmlReader);
						areaSeriesViewAppearance.ReadFromXml(xmlReader);
						pieSeriesViewAppearance.ReadFromXml(xmlReader);
						funnelSeriesViewAppearance.ReadFromXml(xmlReader);
						bar3DSeriesViewAppearance.ReadFromXml(xmlReader);
						pie3DSeriesViewAppearance.ReadFromXml(xmlReader);
						markerAppearance.ReadFromXml(xmlReader);
						seriesLabel2DAppearance.ReadFromXml(xmlReader);
						seriesLabel3DAppearance.ReadFromXml(xmlReader);
						scrollBarAppearance.ReadFromXml(xmlReader);
					xmlReader.ReadEndElement();
				} 
				finally {
					xmlReader.Close();
				}
			} 
			finally {
				reader.Close();
			}
		}
		public override string ToString() {
			return Name;
		}
		public virtual void Assign(ChartAppearance appearance) {
			if (appearance == null)
				throw new ArgumentNullException("appearance");
			this.paletteName = appearance.paletteName;
			this.indicatorsPaletteName = appearance.indicatorsPaletteName;
			this.wholeChartAppearance.Assign(appearance.wholeChartAppearance);
			this.legendAppearance.Assign(appearance.legendAppearance);
			this.xyDiagramAppearance.Assign(appearance.xyDiagramAppearance);
			this.radarDiagramAppearance.Assign(appearance.radarDiagramAppearance);
			this.xyDiagram3DAppearance.Assign(appearance.xyDiagram3DAppearance);
			this.constantLineAppearance.Assign(appearance.constantLineAppearance);
			this.stripAppearance.Assign(appearance.stripAppearance);
			this.textAnnotationAppearance.Assign(appearance.textAnnotationAppearance);
			this.imageAnnotationAppearance.Assign(appearance.imageAnnotationAppearance);
			this.barSeriesViewAppearance.Assign(appearance.barSeriesViewAppearance);
			this.areaSeriesViewAppearance.Assign(appearance.areaSeriesViewAppearance);
			this.pieSeriesViewAppearance.Assign(appearance.pieSeriesViewAppearance);
			this.funnelSeriesViewAppearance.Assign(appearance.funnelSeriesViewAppearance);
			this.bar3DSeriesViewAppearance.Assign(appearance.bar3DSeriesViewAppearance);
			this.pie3DSeriesViewAppearance.Assign(appearance.pie3DSeriesViewAppearance);
			this.markerAppearance.Assign(appearance.markerAppearance);
			this.seriesLabel2DAppearance.Assign(appearance.seriesLabel2DAppearance);
			this.seriesLabel3DAppearance.Assign(appearance.seriesLabel3DAppearance);
			this.scrollBarAppearance.Assign(appearance.scrollBarAppearance);
		}
	}
	public sealed class AppearanceRepository : IEnumerable<ChartAppearance> {
		static class Appearances {
			public const string NatureColorsName = "Nature Colors";
			public const string PastelKitName = "Pastel Kit";
			public const string InAFogName = "In A Fog";
			public const string TerracottaPieName = "Terracotta Pie";
			public const string NorthernLightsName = "Northern Lights";
			public const string ChameleonName = "Chameleon";
			public const string TheTreesName = "The Trees";
			public const string LightName = "Light";
			public const string GrayName = "Gray";
			public const string DarkName = "Dark";
			public const string DarkFlatName = "Dark Flat";
			public const string DefaultName = "Default";
			[ThreadStatic]
			static ChartAppearance natureColors, pastelKit, inAFog, terracottaPie, northernLights, chameleon, theTrees, light, gray, dark, darkFlat;
			public static ChartAppearance GetAppearance(string name) {
				if (name == ChartLocalizer.GetString(ChartStringId.AppNatureColors) || name == NatureColorsName) {
					if (natureColors == null)
						natureColors = new ChartAppearance(ChartStringId.AppNatureColors, NatureColorsName, "NatureColors.xml");
					return natureColors;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppPastelKit) || name == PastelKitName) {
					if (pastelKit == null)
						pastelKit = new ChartAppearance(ChartStringId.AppPastelKit, PastelKitName, "PastelKit.xml");
					return pastelKit;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppInAFog) || name == InAFogName) {
					if (inAFog == null)
						inAFog = new ChartAppearance(ChartStringId.AppInAFog, InAFogName, "InAFog.xml");
					return inAFog;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppTerracottaPie) || name == TerracottaPieName) {
					if (terracottaPie == null)
						terracottaPie = new ChartAppearance(ChartStringId.AppTerracottaPie, TerracottaPieName, "TerracottaPie.xml");
					return terracottaPie;
				} 
				if (name == ChartLocalizer.GetString(ChartStringId.AppNorthernLights) || name == NorthernLightsName) {
					if (northernLights == null)
						northernLights = new ChartAppearance(ChartStringId.AppNorthernLights, NorthernLightsName, "NorthernLights.xml");
					return northernLights;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppChameleon) || name == ChameleonName) {
					if (chameleon == null)
						chameleon = new ChartAppearance(ChartStringId.AppChameleon, ChameleonName, "Chameleon.xml");
					return chameleon;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppTheTrees) || name == TheTreesName) {
					if (theTrees == null)
						theTrees = new ChartAppearance(ChartStringId.AppTheTrees, TheTreesName, "TheTrees.xml");
					return theTrees;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppLight) || name == LightName) {
					if (light == null)
						light = new ChartAppearance(ChartStringId.AppLight, LightName, "Light.xml");
					return light;
				} 
				if (name == ChartLocalizer.GetString(ChartStringId.AppGray) || name == GrayName) {
					if (gray == null)
						gray = new ChartAppearance(ChartStringId.AppGray, GrayName, "Gray.xml");
					return gray;
				} 
				if (name == ChartLocalizer.GetString(ChartStringId.AppDark) || name == DarkName) {
					if (dark == null)
						dark = new ChartAppearance(ChartStringId.AppDark, DarkName, "Dark.xml");
					return dark;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppDarkFlat) || name == DarkFlatName) {  
					if(darkFlat == null)
						darkFlat = new ChartAppearance(ChartStringId.AppDarkFlat, DarkFlatName, "DarkFlat.xml");
					return darkFlat;
				}
				if (name == ChartLocalizer.GetString(ChartStringId.AppDefault) || name == DefaultName) 
					return new ChartAppearance(ChartStringId.AppDefault, DefaultName, "Default.xml", true);
				return null;
			}
			public static string[] GetAllNames() {
				return new string[] { NatureColorsName, PastelKitName, InAFogName, TerracottaPieName, NorthernLightsName, ChameleonName, TheTreesName, LightName, GrayName, DarkName, DarkFlatName, DefaultName }; 
			}
		}
		internal static ChartAppearance Default { get { return new ChartAppearance(ChartStringId.AppDefault, Appearances.DefaultName, "Default.xml", true); } }
		List<ChartAppearance> list;
		readonly ChartAppearance defaultAppearance;
		readonly Dictionary<string, ChartAppearance> repository = new Dictionary<string, ChartAppearance>();		
		public ChartAppearance this[string name] { get { return GetAppearance(name, false); } }
#if !SL
	[DevExpressXtraChartsLocalizedDescription("AppearanceRepositoryNames")]
#endif
		public string[] Names { 
			get {
				return Appearances.GetAllNames();
			} 
		}
		internal AppearanceRepository() {
			defaultAppearance = Appearances.GetAppearance(Appearances.DefaultName);
			RegisterAppearance(defaultAppearance);
		}
		List<ChartAppearance> GetFullListAppearances() {
			if (list == null) {
				list = new List<ChartAppearance>();
				foreach (string name in Appearances.GetAllNames()) {
					if (!repository.ContainsKey(name))
						RegisterAppearance(Appearances.GetAppearance(name));
					list.Add(repository[name]);
				}
			}
			return list;
		}
		internal void RegisterAppearance(ChartAppearance appearance) {
			if (appearance == null || String.IsNullOrEmpty(appearance.Name) || String.IsNullOrEmpty(appearance.SerializableName))
				throw new ArgumentException("appearance");
			repository.Add(appearance.Name, appearance);
			if (appearance.Name != appearance.SerializableName)
				repository.Add(appearance.SerializableName, appearance);
		}
		public ChartAppearance GetAppearance(string name, bool useDefault) { 
			ChartAppearance res;
			if (!repository.TryGetValue(name, out res)) {
				res = Appearances.GetAppearance(name);
				if (res != null)
					RegisterAppearance(res);
			}
			return res == null && useDefault ? defaultAppearance : res;
		} 
		IEnumerator IEnumerable.GetEnumerator() {
			return GetFullListAppearances().GetEnumerator();
		}
		IEnumerator<ChartAppearance> IEnumerable<ChartAppearance>.GetEnumerator() {
			return GetFullListAppearances().GetEnumerator();
		}
	}
}
namespace DevExpress.XtraCharts.Native {	
	public abstract class AppearanceBase {
		public abstract void ReadFromXml(XmlReader xmlReader);
		public virtual void Assign(AppearanceBase obj) {
			if (obj == null)
				throw new ArgumentNullException("obj");
		}
	}
	public class WholeChartAppearance : AppearanceBase {
		Color borderColor;
		Color backColor;
		Color titleColor;
		BackgroundImage backImage;
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color TitleColor { get { return titleColor; } set { titleColor = value; } }
		public BackgroundImage BackImage { get { return backImage; } set { backImage = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("Chart");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
				backColor = XmlUtils.ReadColor(xmlReader, "BackColor");
				titleColor = XmlUtils.ReadColor(xmlReader, "TitleColor");
			xmlReader.ReadEndElement();
			backImage = null;
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			WholeChartAppearance appearance = obj as WholeChartAppearance;
			if (appearance != null) {
				this.borderColor = appearance.borderColor;
				this.backColor = appearance.backColor;
				this.titleColor = appearance.titleColor;
				if (appearance.backImage != null){
					if (this.backImage == null)
						this.backImage = (BackgroundImage)appearance.backImage.Clone();
					this.backImage.Assign(appearance.backImage);
				}
				else
					this.backImage = null;
			}
		}
	}
	public class LegendAppearance : AppearanceBase {
		readonly RectangleFillStyle fillStyle = new RectangleFillStyle(null);
		RectangleIndents padding = new RectangleIndents(null);
		Color textColor;
		Color backColor;
		Color borderColor;
		BackgroundImage backImage;
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		public RectangleIndents Padding { get { return padding; } set { padding = value; } }
		public Color TextColor { get { return textColor; } set { textColor = value; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public BackgroundImage BackImage { get { return backImage; } set { backImage = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("Legend");
				XmlUtils.ReadRectangleIndents(xmlReader, padding, "Padding");
				textColor = XmlUtils.ReadColor(xmlReader, "TextColor");
				backColor = XmlUtils.ReadColor(xmlReader, "BackgroundColor");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "BackgroundStyle");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
			xmlReader.ReadEndElement();
			backImage = null;
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			LegendAppearance appearance = obj as LegendAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.padding.Assign(appearance.padding);
				this.textColor = appearance.textColor;
				this.backColor = appearance.backColor;
				this.borderColor = appearance.borderColor;
				if (appearance.backImage != null) {
					if (this.backImage == null)
						this.backImage = (BackgroundImage)appearance.backImage.Clone();
					this.backImage.Assign(appearance.backImage);
				}
				else
					this.backImage = null;
			}
		}
	}
	public abstract class DiagramAppearance : AppearanceBase {
		readonly string xmlName;
		readonly FillStyleBase fillStyle;
		readonly FillStyleBase interlacedFillStyle;
		Color backColor;
		Color axisColor;
		Color labelsColor;
		Color gridLinesColor;
		Color minorGridLinesColor;
		Color interlacedColor;
		public FillStyleBase FillStyle { get { return fillStyle; } }
		public FillStyleBase InterlacedFillStyle { get { return interlacedFillStyle; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color AxisColor { get { return axisColor; } set { axisColor = value; } }
		public Color LabelsColor { get { return labelsColor; } set { labelsColor = value; } }
		public Color GridLinesColor { get { return gridLinesColor; } set { gridLinesColor = value; } }
		public Color MinorGridLinesColor { get { return minorGridLinesColor; } set { minorGridLinesColor = value; } }
		public Color InterlacedColor { get { return interlacedColor; } set { interlacedColor = value; } }
		protected DiagramAppearance(string xmlName) {
			this.xmlName = xmlName;
			fillStyle = CreateFillStyle();
			interlacedFillStyle = CreateFillStyle();
		}
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement(xmlName);
				backColor = XmlUtils.ReadColor(xmlReader, "BackgroundColor");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "BackgroundStyle");
				axisColor = XmlUtils.ReadColor(xmlReader, "AxisColor");
				labelsColor = XmlUtils.ReadColor(xmlReader, "LabelsColor");
				gridLinesColor = XmlUtils.ReadColor(xmlReader, "GridLinesColor");
				minorGridLinesColor = XmlUtils.ReadColor(xmlReader, "MinorGridLinesColor");
				interlacedColor = XmlUtils.ReadColor(xmlReader, "InterlacedColor");
				XmlUtils.ReadFillStyle(xmlReader, interlacedFillStyle, "InterlacedFillStyle");
				ReadFromXmlInternal(xmlReader);
			xmlReader.ReadEndElement();
		}
		protected virtual void ReadFromXmlInternal(XmlReader xmlReader) {
		}
		protected abstract FillStyleBase CreateFillStyle();
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			DiagramAppearance appearance = obj as DiagramAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.interlacedFillStyle.Assign(appearance.interlacedFillStyle);
				this.backColor = appearance.backColor;
				this.axisColor = appearance.axisColor;
				this.labelsColor = appearance.labelsColor;
				this.gridLinesColor = appearance.gridLinesColor;
				this.minorGridLinesColor = appearance.minorGridLinesColor;
				this.interlacedColor = appearance.interlacedColor;
			}
		}
	}
	public abstract class Diagram2DAppearance : DiagramAppearance {
		Color borderColor;
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		protected Diagram2DAppearance(string xmlName) : base(xmlName) {
		}
		protected override void ReadFromXmlInternal(XmlReader xmlReader) {
			borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			Diagram2DAppearance appearance = obj as Diagram2DAppearance;
			if (appearance != null) {
				this.borderColor = appearance.borderColor;
			}
		}
	}
	public class XYDiagramAppearance : Diagram2DAppearance {
		Color textColor;
		Color axisTitleColor;
		Color zoomRectangleColor;
		Color zoomRectangleBorderColor;
		public Color TextColor { get { return textColor; } set { textColor = value; } }
		public Color AxisTitleColor { get { return axisTitleColor; } set { axisTitleColor = value; } }
		public Color ZoomRectangleColor { get { return zoomRectangleColor; } set { zoomRectangleColor = value; } }
		public Color ZoomRectangleBorderColor { get { return zoomRectangleBorderColor; } set { zoomRectangleBorderColor = value; } }
		public XYDiagramAppearance() : base("XYDiagram") {
		}
		protected override void ReadFromXmlInternal(XmlReader xmlReader) {
			base.ReadFromXmlInternal(xmlReader);
			textColor = XmlUtils.ReadColor(xmlReader, "TextColor");
			axisTitleColor = XmlUtils.ReadColor(xmlReader, "AxisTitleColor");
			zoomRectangleColor = XmlUtils.ReadColor(xmlReader, "ZoomRectangleColor");
			zoomRectangleBorderColor = XmlUtils.ReadColor(xmlReader, "ZoomRectangleBorderColor");
		}
		protected override FillStyleBase CreateFillStyle() {
			return new RectangleFillStyle(null);
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			XYDiagramAppearance appearance = obj as XYDiagramAppearance;
			if (appearance != null) {
				this.textColor = appearance.textColor;
				this.axisTitleColor = appearance.axisTitleColor;
				this.zoomRectangleColor = appearance.zoomRectangleColor;
				this.zoomRectangleBorderColor = appearance.zoomRectangleBorderColor;
			}
		}
	}
	public class RadarDiagramAppearance : Diagram2DAppearance {
		public RadarDiagramAppearance() : base("RadarDiagram") {
		}
		protected override FillStyleBase CreateFillStyle() {
			return new PolygonFillStyle(null);
		}
	}
	public class XYDiagram3DAppearance : DiagramAppearance {
		public XYDiagram3DAppearance() : base("XYDiagram3D") {
		}
		protected override FillStyleBase CreateFillStyle() {
			return new RectangleFillStyle3D(null);
		}
	}
	public class ConstantLineAppearance : AppearanceBase {
		Color color;
		DashStyle dashStyle;
		Color titleColor;
		public Color Color { get { return color; } set { color = value; } }
		public DashStyle DashStyle { get { return dashStyle; } set { dashStyle = value; } }
		public Color TitleColor { get { return titleColor; } set { titleColor = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("ConstantLine");
				color = XmlUtils.ReadColor(xmlReader, "Color");
				dashStyle = (DashStyle)XmlUtils.ReadEnum(xmlReader, "DashStyle", typeof(DashStyle));
				titleColor = XmlUtils.ReadColor(xmlReader, "TitleColor");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			ConstantLineAppearance appearance = obj as ConstantLineAppearance;
			if (appearance != null) {
				this.color = appearance.color;
				this.dashStyle = appearance.dashStyle;
				this.titleColor = appearance.titleColor;
			}
		}
	}
	public class StripAppearance : AppearanceBase {
		readonly RectangleFillStyle fillStyle = new RectangleFillStyle(null);
		Color color;
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		public Color Color { get { return color; } set { color = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("Strip");
				color = XmlUtils.ReadColor(xmlReader, "Color");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			StripAppearance appearance = obj as StripAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.color = appearance.color;
			}
		}
	}
	public abstract class AnnotationAppearance : AppearanceBase {
		readonly string xmlName;
		readonly RectangleFillStyle fillStyle = new RectangleFillStyle(null);
		Color backColor;
		Color borderColor;
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public AnnotationAppearance(string xmlName) {
			this.xmlName = xmlName;
		}
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement(xmlName);
			backColor = XmlUtils.ReadColor(xmlReader, "BackColor");
			borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
			XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			ReadFromXmlInternal(xmlReader);
			xmlReader.ReadEndElement();
		}
		protected virtual void ReadFromXmlInternal(XmlReader xmlReader) {
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			AnnotationAppearance appearance = obj as AnnotationAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.backColor = appearance.backColor;
				this.borderColor = appearance.borderColor;
			}
		}
	}
	public class TextAnnotationAppearance : AnnotationAppearance {
		Color textColor;
		public Color TextColor { get { return textColor; } set { textColor = value; } }
		public TextAnnotationAppearance() : base("TextAnnotation") {
		}
		protected override void ReadFromXmlInternal(XmlReader xmlReader) {
			textColor = XmlUtils.ReadColor(xmlReader, "TextColor");
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			TextAnnotationAppearance appearance = obj as TextAnnotationAppearance;
			if (appearance != null) {
				this.textColor = appearance.textColor;
			}
		}
	}
	public class ImageAnnotationAppearance : AnnotationAppearance {
		public ImageAnnotationAppearance() : base("ImageAnnotation") {
		}
	}
	public class BarSeriesViewAppearance : AppearanceBase {
		readonly RectangleFillStyle fillStyle = new RectangleFillStyle(null);
		Color borderColor;
		bool showBorder;
		public RectangleFillStyle FillStyle { get { return fillStyle; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public bool ShowBorder { get { return showBorder; } set { showBorder = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("BarSeriesView");
				showBorder = XmlUtils.ReadBoolean(xmlReader, "ShowBorder");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			BarSeriesViewAppearance appearance = obj as BarSeriesViewAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.borderColor = appearance.borderColor;
				this.showBorder = appearance.showBorder;
			}
		}
	}
	public class AreaSeriesViewAppearance : AppearanceBase {
		readonly PolygonFillStyle fillStyle = new PolygonFillStyle(null);
		Color borderColor;
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("AreaSeriesView");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			AreaSeriesViewAppearance appearance = obj as AreaSeriesViewAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.borderColor = appearance.borderColor;
			}
		}
	}
	public class PieSeriesViewAppearance : AppearanceBase {
		readonly PolygonFillStyle fillStyle = new PolygonFillStyle(null);
		Color borderColor;
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("PieSeriesView");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			PieSeriesViewAppearance appearance = obj as PieSeriesViewAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.borderColor = appearance.borderColor;
			}
		}
	}
	public class FunnelSeriesViewAppearance : AppearanceBase {
		readonly PolygonFillStyle fillStyle = new PolygonFillStyle(null);
		Color borderColor;
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("FunnelSeriesView");
			borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
			XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			FunnelSeriesViewAppearance appearance = obj as FunnelSeriesViewAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.borderColor = appearance.borderColor;
			}
		}
	}
	public class Bar3DSeriesViewAppearance : AppearanceBase {
		readonly RectangleFillStyle3D fillStyle = new RectangleFillStyle3D(null);
		public RectangleFillStyle3D FillStyle { get { return fillStyle; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("Bar3DSeriesView");
			XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			Bar3DSeriesViewAppearance appearance = obj as Bar3DSeriesViewAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
			}
		}
	}
	public class Pie3DSeriesViewAppearance : AppearanceBase {
		readonly PolygonFillStyle3D fillStyle = new PolygonFillStyle3D(null);
		public PolygonFillStyle3D FillStyle { get { return fillStyle; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("Pie3DSeriesView");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			Pie3DSeriesViewAppearance appearance = obj as Pie3DSeriesViewAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
			}
		}
	}
	public class MarkerAppearance : AppearanceBase {
		readonly PolygonFillStyle fillStyle = new PolygonFillStyle(null);
		Color borderColor;
		public PolygonFillStyle FillStyle { get { return fillStyle; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("Marker");
				XmlUtils.ReadFillStyle(xmlReader, fillStyle, "FillStyle");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			MarkerAppearance appearance = obj as MarkerAppearance;
			if (appearance != null) {
				this.fillStyle.Assign(appearance.fillStyle);
				this.borderColor = appearance.borderColor;
			}
		}
	}
	public class SeriesLabelAppearance : AppearanceBase {
		readonly string xmlName;
		Color backColor;
		Color textColor;
		Color borderColor;
		Color connectorColor;
		bool showBorder;
		bool showConnector;
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color TextColor { get { return textColor; } set { textColor = value; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public Color ConnectorColor { get { return connectorColor; } set { connectorColor = value; } }
		public bool ShowBorder { get { return showBorder; } set { showBorder = value; } }
		public bool ShowConnector { get { return showConnector; } set { showConnector = value; } }
		public SeriesLabelAppearance(string xmlName) {
			this.xmlName = xmlName;
		}
		protected virtual void ReadFromXMLCore(XmlReader xmlReader) {
			backColor = XmlUtils.ReadColor(xmlReader, "BackColor");
			textColor = XmlUtils.ReadColor(xmlReader, "TextColor");
			showBorder = XmlUtils.ReadBoolean(xmlReader, "ShowBorder");
			borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
			showConnector = XmlUtils.ReadBoolean(xmlReader, "ShowConnector");
			connectorColor = XmlUtils.ReadColor(xmlReader, "ConnectorColor");
		}
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement(xmlName);
			ReadFromXMLCore(xmlReader);
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			SeriesLabelAppearance appearance = obj as SeriesLabelAppearance;
			if (appearance != null) {
				this.backColor = appearance.backColor;
				this.textColor = appearance.textColor;
				this.borderColor = appearance.borderColor;
				this.connectorColor = appearance.connectorColor;
				this.showBorder = appearance.showBorder;
				this.showConnector = appearance.showConnector;
			}
		}
	}
	public class SeriesLabel2DAppearance : SeriesLabelAppearance {
		bool showBubbleConnector;
		public bool ShowBubbleConnector { get { return showBubbleConnector; } set { showBubbleConnector = value; } }
		public SeriesLabel2DAppearance() : base("SeriesLabel2D") {
		}
		protected override void ReadFromXMLCore(XmlReader xmlReader) {
			base.ReadFromXMLCore(xmlReader);
			showBubbleConnector = XmlUtils.ReadBoolean(xmlReader, "ShowBubbleConnector");
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			SeriesLabel2DAppearance appearance = obj as SeriesLabel2DAppearance;
			if (appearance != null) {
				this.showBubbleConnector = appearance.showBubbleConnector;
			}
		}
	}
	public class SeriesLabel3DAppearance : SeriesLabelAppearance {
		public SeriesLabel3DAppearance() : base("SeriesLabel3D") {
		}
	}
	public class ScrollBarAppearance : AppearanceBase {
		Color backColor;
		Color barColor;
		Color borderColor;
		public Color BackColor { get { return backColor; } set { backColor = value; } }
		public Color BarColor { get { return barColor; } set { barColor = value; } }
		public Color BorderColor { get { return borderColor; } set { borderColor = value; } }
		public override void ReadFromXml(XmlReader xmlReader) {
			xmlReader.ReadStartElement("ScrollBar");
				backColor = XmlUtils.ReadColor(xmlReader, "BackColor");
				barColor = XmlUtils.ReadColor(xmlReader, "BarColor");
				borderColor = XmlUtils.ReadColor(xmlReader, "BorderColor");
			xmlReader.ReadEndElement();
		}
		public override void Assign(AppearanceBase obj) {
			base.Assign(obj);
			ScrollBarAppearance appearance = obj as ScrollBarAppearance;
			if (appearance != null) {
				this.backColor = appearance.backColor;
				this.barColor = appearance.barColor;
				this.borderColor = appearance.borderColor;
			}
		}
	}
}
