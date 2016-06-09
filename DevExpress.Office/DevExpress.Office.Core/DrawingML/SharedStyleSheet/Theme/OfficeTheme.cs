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

using DevExpress.Office;
using DevExpress.Office.Internal;
using DevExpress.Office.Model;
using DevExpress.Utils;
using DevExpress.Office.Export;
using DevExpress.Office.Import;
using System;
using System.Collections.Generic;
using DevExpress.Office.Utils;
using DevExpress.Office.DrawingML;
namespace DevExpress.Office.Drawing {
	#region DrawingCache<TFormat>
	public class DrawingCache<TFormat>: IDrawingCache, IDisposable {
		#region Fields
		bool isDisposed;
		DrawingColorModelInfoCache drawingColorModelInfoCache;
		DrawingBlipFillInfoCache drawingBlipFillInfoCache;
		DrawingBlipTileInfoCache drawingBlipTileInfoCache;
		DrawingGradientFillInfoCache drawingGradientFillInfoCache;
		OutlineInfoCache outlineInfoCache;
		Scene3DPropertiesInfoCache scene3DPropertiesInfoCache;
		Scene3DRotationInfoCache scene3DRotationInfoCache;
		DrawingTextCharacterInfoCache drawingTextCharacterInfoCache;
		DrawingTextParagraphInfoCache drawingTextParagraphInfoCache;
		DrawingTextSpacingInfoCache drawingTextSpacingInfoCache;
		DrawingTextBodyInfoCache drawingTextBodyInfoCache;
		#endregion
		public DrawingCache(DocumentModelBase<TFormat> documentModelBase) {
			Guard.ArgumentNotNull(documentModelBase, "documentModelBase");
			Initialize(documentModelBase);
		}
		#region Properties
		protected internal bool IsDisposed { get { return isDisposed; } }
		public DrawingColorModelInfoCache DrawingColorModelInfoCache { get { return drawingColorModelInfoCache; } }
		public DrawingBlipFillInfoCache DrawingBlipFillInfoCache { get { return drawingBlipFillInfoCache; } }
		public DrawingBlipTileInfoCache DrawingBlipTileInfoCache { get { return drawingBlipTileInfoCache; } }
		public DrawingGradientFillInfoCache DrawingGradientFillInfoCache { get { return drawingGradientFillInfoCache; } }
		public OutlineInfoCache OutlineInfoCache { get { return outlineInfoCache; } }
		public Scene3DPropertiesInfoCache Scene3DPropertiesInfoCache { get { return scene3DPropertiesInfoCache; } }
		public Scene3DRotationInfoCache Scene3DRotationInfoCache { get { return scene3DRotationInfoCache; } }
		public DrawingTextCharacterInfoCache DrawingTextCharacterInfoCache { get { return drawingTextCharacterInfoCache; } }
		public DrawingTextParagraphInfoCache DrawingTextParagraphInfoCache { get { return drawingTextParagraphInfoCache; } }
		public DrawingTextSpacingInfoCache DrawingTextSpacingInfoCache { get { return drawingTextSpacingInfoCache; } }
		public DrawingTextBodyInfoCache DrawingTextBodyInfoCache { get { return drawingTextBodyInfoCache; } }
		#endregion
		#region IDisposable implementation
		protected virtual void Dispose(bool disposing) {
			isDisposed = true;
		}
		public void Dispose() {
			Dispose(true);
		}
		#endregion
		#region Initialize
		protected virtual void Initialize(DocumentModelBase<TFormat> documentModelBase) {
			DocumentModelUnitConverter unitConverter = documentModelBase.UnitConverter;
			drawingColorModelInfoCache = new DrawingColorModelInfoCache(unitConverter);
			drawingBlipFillInfoCache = new DrawingBlipFillInfoCache(unitConverter);
			drawingBlipTileInfoCache = new DrawingBlipTileInfoCache(unitConverter);
			drawingGradientFillInfoCache = new DrawingGradientFillInfoCache(unitConverter);
			outlineInfoCache = new OutlineInfoCache(unitConverter);
			scene3DPropertiesInfoCache = new Scene3DPropertiesInfoCache(unitConverter);
			scene3DRotationInfoCache = new Scene3DRotationInfoCache(unitConverter);
			drawingTextCharacterInfoCache = new DrawingTextCharacterInfoCache(unitConverter);
			drawingTextParagraphInfoCache = new DrawingTextParagraphInfoCache(unitConverter);
			drawingTextSpacingInfoCache = new DrawingTextSpacingInfoCache(unitConverter);
			drawingTextBodyInfoCache = new DrawingTextBodyInfoCache(unitConverter);
		}
		#endregion
		public List<SizeOfInfo> GetSizeOfInfo() {
			List<SizeOfInfo> result = ObjectSizeHelper.CalculateSizeOfInfo(this);
			result.Insert(0, ObjectSizeHelper.CalculateTotalSizeOfInfo(result, "ThemeOffice.Cache Total"));
			return result;
		}
	}
	#endregion
	#region IOfficeTheme
	public interface IOfficeTheme {
		ThemeDrawingColorCollection Colors { get; }
		ThemeFontScheme FontScheme { get; }
		ThemeFormatScheme FormatScheme { get; }
		string Name { get; set; }
		bool IsValidate { get; }
		void CopyFrom(IOfficeTheme sourceObj);
		void Clear();
		IOfficeTheme Clone();
	}
	#endregion
	#region OfficeThemeBase
	public class OfficeThemeBase<TFormat> : DocumentModelBase<TFormat>, IDocumentModelPart, IOfficeTheme {
		#region Fields
		readonly ThemeDrawingColorCollection colors;
		readonly ThemeFontScheme fontScheme;
		readonly ThemeFormatScheme formatScheme;
		string name = String.Empty;
		#endregion
		public OfficeThemeBase()
			: base() {
			SwitchToEmptyHistory(true);
			this.colors = new ThemeDrawingColorCollection(this);
			this.fontScheme = new ThemeFontScheme(this);
			this.formatScheme = new ThemeFormatScheme();
		}
		#region Properties
		public string Name { get { return name; } set { name = value; } }
		public ThemeDrawingColorCollection Colors { get { return colors; } }
		public ThemeFontScheme FontScheme { get { return fontScheme; } }
		public ThemeFormatScheme FormatScheme { get { return formatScheme; } }
		public override IOfficeTheme OfficeTheme {
			get {
				return this;
			}
			set { }
		}
		public bool IsValidate { get { return !String.IsNullOrEmpty(name) && colors.IsValidate && fontScheme.IsValidate && formatScheme.IsValidate; } }
		#endregion
		public void CopyFrom(IOfficeTheme sourceObj) {
			name = sourceObj.Name;
			colors.CopyFrom(sourceObj.Colors);
			fontScheme.CopyFrom(sourceObj.FontScheme);
			formatScheme.CopyFrom(this, sourceObj);
		}
		public void Clear() {
			name = String.Empty;
			colors.Clear();
			fontScheme.Clear();
			formatScheme.Clear();
		}
		public IOfficeTheme  Clone() {
			OfficeThemeBase<TFormat> result = new OfficeThemeBase<TFormat>();
			result.CopyFrom(this);
			return result;
		}
		#region IDocumentModelPart Members
		IDocumentModel IDocumentModelPart.DocumentModel { get { return this; } }
		#endregion
		#region  DocumentModelBase<DocumentFormat> Members
		public override IDocumentModelPart MainPart { get { return this; } }
		public override ExportHelper<TFormat, bool> CreateDocumentExportHelper(TFormat documentFormat) {
			return  null;
		}
		public override void OnBeginUpdate() {
		}
		public override void OnCancelUpdate() {
		}
		public override void OnEndUpdate() {
		}
		public override void OnFirstBeginUpdate() {
		}
		public override void OnLastCancelUpdate() {
		}
		public override void OnLastEndUpdate() {
		}
		#endregion
		protected internal override void OnHistoryOperationCompleted(object sender, EventArgs e) {
		}
		protected internal override void OnHistoryModifiedChanged(object sender, EventArgs e) {
		}
		protected internal override ImportHelper<TFormat, bool> CreateDocumentImportHelper() {
			return null;
		}
		protected internal override IImportManagerService<TFormat, bool> GetImportManagerService() {
			return null;
		}
		protected internal override IExportManagerService<TFormat, bool> GetExportManagerService() {
			return null;
		}
		protected override void CreateOfficeTheme() {
		}
	}
	#endregion
	#region OfficeThemePreset
	public enum OfficeThemePreset {
		Office, 
	}
	#endregion
	#region OfficeThemeBuilder
	public class OfficeThemeBuilder<TFormat> {
		#region Static Members
		public static OfficeThemeBase<TFormat> CreateTheme(OfficeThemePreset preset) {
			OfficeThemeBase<TFormat> result = new OfficeThemeBase<TFormat>();
			OfficeThemeBuilder<TFormat> builder = new OfficeThemeBuilder<TFormat>(result, preset);
			builder.Build();
			return result;
		}
		public static bool IsDefaultOfficeThemeVersion(int themeVersion) {
			return themeVersion == DefaultOfficeThemeVersion1 || themeVersion == DefaultOfficeThemeVersion2;
		}
		#endregion
		#region Fields
		public const int DefaultOfficeThemeVersion1 = 124226;
		public const int DefaultOfficeThemeVersion2 = 123820;
		public const string DefaultPartOfficeName = "Office";
		public const string DefaultOfficeName = "Office Theme";
		OfficeThemeBase<TFormat> theme;
		OfficeThemePreset preset;
		#endregion
		OfficeThemeBuilder(OfficeThemeBase<TFormat> theme, OfficeThemePreset preset) {
			this.theme = theme;
			this.preset = preset;
		}
		void Build() {
			theme.BeginUpdate();
			try {
				Dictionary<OfficeThemePreset, CalculateOfficeThemeProperties> collection = GetDelegateCollection();
				CalculateOfficeThemeProperties build = collection[preset];
				build();
			} finally {
				theme.EndUpdate();
			}
		}
		#region CalculateOfficeThemeProperties
		delegate void CalculateOfficeThemeProperties();
		Dictionary<OfficeThemePreset, CalculateOfficeThemeProperties> GetDelegateCollection() {
			Dictionary<OfficeThemePreset, CalculateOfficeThemeProperties> result = new Dictionary<OfficeThemePreset, CalculateOfficeThemeProperties>();
			result.Add(OfficeThemePreset.Office, new CalculateOfficeThemeProperties(this.CalculateDefaultOfficeThemeProperties));
			return result;
		}
		#region CalculateDefaultOfficeThemeProperties
		void CalculateDefaultOfficeThemeProperties() {
			theme.Name = DefaultOfficeName;
			CalculateDefaultOfficeThemeColorProperties();
			CalculateDefaultOfficeThemeFontProperties();
			CalculateDefaultOfficeThemeFormatProperties();
		}
		void CalculateDefaultOfficeThemeColorProperties() {
			ThemeDrawingColorCollection colors = theme.Colors;
			colors.Name = DefaultPartOfficeName;
			colors.SetColor(ThemeColorIndex.Light1, SystemColorValues.ScWindow);
			colors.SetColor(ThemeColorIndex.Dark1, SystemColorValues.ScWindowText);
			colors.SetColor(ThemeColorIndex.Light2, DXColor.FromArgb(0xEE, 0xEC, 0xE1));
			colors.SetColor(ThemeColorIndex.Dark2, DXColor.FromArgb(0x1F, 0x49, 0x7D));
			colors.SetColor(ThemeColorIndex.Accent1, DXColor.FromArgb(0x4F, 0x81, 0xBD));
			colors.SetColor(ThemeColorIndex.Accent2, DXColor.FromArgb(0xC0, 0x50, 0x4D));
			colors.SetColor(ThemeColorIndex.Accent3, DXColor.FromArgb(0x9B, 0xBB, 0x59));
			colors.SetColor(ThemeColorIndex.Accent4, DXColor.FromArgb(0x80, 0x64, 0xA2));
			colors.SetColor(ThemeColorIndex.Accent5, DXColor.FromArgb(0x4B, 0xAC, 0xC6));
			colors.SetColor(ThemeColorIndex.Accent6, DXColor.FromArgb(0xF7, 0x96, 0x46));
			colors.SetColor(ThemeColorIndex.Hyperlink, DXColor.FromArgb(0x0, 0x0, 0xFF));
			colors.SetColor(ThemeColorIndex.FollowedHyperlink, DXColor.FromArgb(0x80, 0x0, 0x80));
		}
		void CalculateDefaultOfficeThemeFontProperties() {
			ThemeFontScheme scheme = theme.FontScheme;
			scheme.Name = DefaultPartOfficeName;
			ThemeFontSchemePart majorFont = scheme.MajorFont;
			majorFont.Latin.Typeface = "Cambria";
			majorFont.AddSupplementalFont("Jpan", "ＭＳ Ｐゴシック");
			majorFont.AddSupplementalFont("Hang", "맑은 고딕");
			majorFont.AddSupplementalFont("Hans", "宋体");
			majorFont.AddSupplementalFont("Hant", "新細明體");
			majorFont.AddSupplementalFont("Arab", "Times New Roman");
			majorFont.AddSupplementalFont("Hebr", "Times New Roman");
			majorFont.AddSupplementalFont("Thai", "Tahoma");
			majorFont.AddSupplementalFont("Ethi", "Nyala");
			majorFont.AddSupplementalFont("Beng", "Vrinda");
			majorFont.AddSupplementalFont("Gujr", "Shruti");
			majorFont.AddSupplementalFont("Khmr", "MoolBoran");
			majorFont.AddSupplementalFont("Knda", "Tunga");
			majorFont.AddSupplementalFont("Guru", "Raavi");
			majorFont.AddSupplementalFont("Cans", "Euphemia");
			majorFont.AddSupplementalFont("Cher", "Plantagenet Cherokee");
			majorFont.AddSupplementalFont("Yiii", "Microsoft Yi Baiti");
			majorFont.AddSupplementalFont("Tibt", "Microsoft Himalaya");
			majorFont.AddSupplementalFont("Thaa", "MV Boli");
			majorFont.AddSupplementalFont("Deva", "Mangal");
			majorFont.AddSupplementalFont("Telu", "Gautami");
			majorFont.AddSupplementalFont("Taml", "Latha");
			majorFont.AddSupplementalFont("Syrc", "Estrangelo Edessa");
			majorFont.AddSupplementalFont("Orya", "Kalinga");
			majorFont.AddSupplementalFont("Mlym", "Kartika");
			majorFont.AddSupplementalFont("Laoo", "DokChampa");
			majorFont.AddSupplementalFont("Sinh", "Iskoola Pota");
			majorFont.AddSupplementalFont("Mong", "Mongolian Baiti");
			majorFont.AddSupplementalFont("Viet", "Times New Roman");
			majorFont.AddSupplementalFont("Uigh", "Microsoft Uighur");
			majorFont.AddSupplementalFont("Geor", "Sylfaen");
			majorFont.IsValid = true;
			ThemeFontSchemePart minorFont = scheme.MinorFont;
			minorFont.Latin.Typeface = "Calibri";
			minorFont.AddSupplementalFont("Jpan", "ＭＳ Ｐゴシック");
			minorFont.AddSupplementalFont("Hang", "맑은 고딕");
			minorFont.AddSupplementalFont("Hans", "宋体");
			minorFont.AddSupplementalFont("Hant", "新細明體");
			minorFont.AddSupplementalFont("Arab", "Arial");
			minorFont.AddSupplementalFont("Hebr", "Arial");
			minorFont.AddSupplementalFont("Thai", "Tahoma");
			minorFont.AddSupplementalFont("Ethi", "Nyala");
			minorFont.AddSupplementalFont("Beng", "Vrinda");
			minorFont.AddSupplementalFont("Gujr", "Shruti");
			minorFont.AddSupplementalFont("Khmr", "DaunPenh");
			minorFont.AddSupplementalFont("Knda", "Tunga");
			minorFont.AddSupplementalFont("Guru", "Raavi");
			minorFont.AddSupplementalFont("Cans", "Euphemia");
			minorFont.AddSupplementalFont("Cher", "Plantagenet Cherokee");
			minorFont.AddSupplementalFont("Yiii", "Microsoft Yi Baiti");
			minorFont.AddSupplementalFont("Tibt", "Microsoft Himalaya");
			minorFont.AddSupplementalFont("Thaa", "MV Boli");
			minorFont.AddSupplementalFont("Deva", "Mangal");
			minorFont.AddSupplementalFont("Telu", "Gautami");
			minorFont.AddSupplementalFont("Taml", "Latha");
			minorFont.AddSupplementalFont("Syrc", "Estrangelo Edessa");
			minorFont.AddSupplementalFont("Orya", "Kalinga");
			minorFont.AddSupplementalFont("Mlym", "Kartika");
			minorFont.AddSupplementalFont("Laoo", "DokChampa");
			minorFont.AddSupplementalFont("Sinh", "Iskoola Pota");
			minorFont.AddSupplementalFont("Mong", "Mongolian Baiti");
			minorFont.AddSupplementalFont("Viet", "Arial");
			minorFont.AddSupplementalFont("Uigh", "Microsoft Uighur");
			minorFont.AddSupplementalFont("Geor", "Sylfaen");
			minorFont.IsValid = true;
		}
		void CalculateDefaultOfficeThemeFormatProperties() {
			theme.FormatScheme.Name = DefaultPartOfficeName;
			CalculateDefaultOfficeFillStyleList();
			CalculateDefaultOfficeBackgroundFillStyleList();
			CalculateDefaultOfficeOutlineList();
			CalculateDefaultOfficeEffectStyleList();
		}
		void CalculateDefaultOfficeFillStyleList() {
			#region SolidFill
			theme.FormatScheme.FillStyleList.Add(CreateSolidFill());
			#endregion
			#region GradientFill1
			DrawingGradientFill gradientFill1 = CreateGradientFill(true, GradientType.Linear);
			DrawingGradientStop gradientFill1Stop1 = CreateGradientStopWithTint(0, 50000, 300000);
			DrawingGradientStop gradientFill1Stop2 = CreateGradientStopWithTint(35000, 37000, 300000);
			DrawingGradientStop gradientFill1Stop3 = CreateGradientStopWithTint(100000, 15000, 350000);
			gradientFill1.GradientStops.Add(gradientFill1Stop1);
			gradientFill1.GradientStops.Add(gradientFill1Stop2);
			gradientFill1.GradientStops.Add(gradientFill1Stop3);
			theme.FormatScheme.FillStyleList.Add(gradientFill1);
			#endregion
			#region GradientFill2
			DrawingGradientFill gradientFill2 = CreateGradientFill(false, GradientType.Linear);
			DrawingGradientStop gradientFill2Stop1 = CreateGradientStopWithShade(0, 51000, 130000);
			DrawingGradientStop gradientFill2Stop2 = CreateGradientStopWithShade(80000, 93000, 130000);
			DrawingGradientStop gradientFill2Stop3 = CreateGradientStopWithShade(100000, 94000, 135000);
			gradientFill2.GradientStops.Add(gradientFill2Stop1);
			gradientFill2.GradientStops.Add(gradientFill2Stop2);
			gradientFill2.GradientStops.Add(gradientFill2Stop3);
			theme.FormatScheme.FillStyleList.Add(gradientFill2);
			#endregion
		}
		void CalculateDefaultOfficeBackgroundFillStyleList() {
			#region SolidFill
			theme.FormatScheme.BackgroundFillStyleList.Add(CreateSolidFill());
			#endregion
			#region GradientFill1
			DrawingGradientFill gradientFill1 = CreateGradientFill(false, GradientType.Circle);
			DrawingGradientStop gradientFill1Stop1 = CreateGradientStopWithTint(0, 40000, 350000);
			DrawingGradientStop gradientFill1Stop2 = CreateGradientStopWithTintShade(40000, 45000, 99000, 350000);
			DrawingGradientStop gradientFill1Stop3 = CreateGradientStopWithShade(100000, 20000, 255000);
			gradientFill1.GradientStops.Add(gradientFill1Stop1);
			gradientFill1.GradientStops.Add(gradientFill1Stop2);
			gradientFill1.GradientStops.Add(gradientFill1Stop3);
			gradientFill1.FillRect = new RectangleOffset(180000, 50000, 50000, -80000);
			theme.FormatScheme.BackgroundFillStyleList.Add(gradientFill1);
			#endregion
			#region GradientFill2
			DrawingGradientFill gradientFill2 = CreateGradientFill(false, GradientType.Circle);
			DrawingGradientStop gradientFill2Stop1 = CreateGradientStopWithTint(0, 80000, 300000);
			DrawingGradientStop gradientFill2Stop2 = CreateGradientStopWithShade(100000, 30000, 200000);
			gradientFill2.GradientStops.Add(gradientFill2Stop1);
			gradientFill2.GradientStops.Add(gradientFill2Stop2);
			gradientFill2.FillRect = new RectangleOffset(50000, 50000, 50000, 50000);
			theme.FormatScheme.BackgroundFillStyleList.Add(gradientFill2);
			#endregion
		}
		void CalculateDefaultOfficeOutlineList() {
			#region Outline1
			Outline outline1 = CreateOutline(15);
			DrawingSolidFill solidFill1 = CreateSolidFill(95000, 105000);
			outline1.Fill = solidFill1;
			theme.FormatScheme.LineStyleList.Add(outline1);
			#endregion
			#region Outline2
			Outline outline2 = CreateOutline(40);
			DrawingSolidFill solidFill2 = CreateSolidFill();
			outline2.Fill = solidFill2;
			theme.FormatScheme.LineStyleList.Add(outline2);
			#endregion
			#region Outline3
			Outline outline3 = CreateOutline(60);
			DrawingSolidFill solidFill3 = CreateSolidFill();
			outline3.Fill = solidFill3;
			theme.FormatScheme.LineStyleList.Add(outline3);
			#endregion
		}
		void CalculateDefaultOfficeEffectStyleList() {
			OuterShadowEffect shadowEffect1 = CreateOuterShadowEffect(20000, 38000);
			AddOuterShadowEffectToContainer(shadowEffect1);
			OuterShadowEffect shadowEffect2 = CreateOuterShadowEffect(23000, 35000);
			AddOuterShadowEffectToContainer(shadowEffect2);
			DrawingEffectStyle effectStyle = new DrawingEffectStyle(theme);
			OuterShadowEffect shadowEffect = CreateOuterShadowEffect(23000, 35000);
			Scene3DProperties scene3d = new Scene3DProperties(theme);
			scene3d.Camera.Preset = PresetCameraType.OrthographicFront;
			scene3d.LightRig.Preset = LightRigPreset.ThreePt;
			scene3d.LightRig.Direction = LightRigDirection.Top;
			scene3d.Camera.Lat = 0;
			scene3d.LightRig.Rev = 1200000;
			Shape3DProperties shape3d = new Shape3DProperties(theme);
			shape3d.TopBevel.Width = 100;
			shape3d.TopBevel.Heigth = 40;
			effectStyle.ContainerEffect.Effects.Add(shadowEffect);
			effectStyle.Scene3DProperties.CopyFrom(scene3d);
			effectStyle.Shape3DProperties.CopyFrom(shape3d);
			theme.FormatScheme.EffectStyleList.Add(effectStyle);
		}
		#endregion
		#region Helper Methods
		#region CreateSolidFill
		DrawingSolidFill CreateSolidFill() {
			DrawingSolidFill result = new DrawingSolidFill(theme);
			result.Color.OriginalColor.Scheme = SchemeColorValues.Style;
			return result;
		}
		DrawingSolidFill CreateSolidFill(int shade, int satMod) {
			DrawingSolidFill result = CreateSolidFill();
			result.Color.OriginalColor.Scheme = SchemeColorValues.Style;
			result.Color.Transforms.AddCore(new ShadeColorTransform(shade));
			result.Color.Transforms.AddCore(new SaturationModulationColorTransform(satMod));
			return result;
		}
		#endregion
		#region CreateGradientFill
		DrawingGradientFill CreateGradientFill(bool scaled, GradientType type) {
			DrawingGradientFill result = new DrawingGradientFill(theme);
			result.RotateWithShape = true;
			result.GradientType = type;
			result.Angle = 16200000;
			result.Scaled = scaled;
			return result;
		}
		DrawingGradientStop CreateGradientStopCore(int position) {
			DrawingGradientStop result = new DrawingGradientStop(theme);
			result.Position = position;
			result.Color.OriginalColor.Scheme = SchemeColorValues.Style;
			return result;
		}
		DrawingGradientStop CreateGradientStopWithTint(int position, int tint, int satMod) {
			DrawingGradientStop result = CreateGradientStopCore(position);
			result.Color.Transforms.AddCore(new TintColorTransform(tint));
			result.Color.Transforms.AddCore(new SaturationModulationColorTransform(satMod));
			return result;
		}
		DrawingGradientStop CreateGradientStopWithShade(int position, int shade, int satMod) {
			DrawingGradientStop result = CreateGradientStopCore(position);
			result.Color.Transforms.AddCore(new ShadeColorTransform(shade));
			result.Color.Transforms.AddCore(new SaturationModulationColorTransform(satMod));
			return result;
		}
		DrawingGradientStop CreateGradientStopWithTintShade(int position, int tint, int shade, int satMod) {
			DrawingGradientStop result = CreateGradientStopCore(position);
			result.Color.Transforms.AddCore(new TintColorTransform(tint));
			result.Color.Transforms.AddCore(new ShadeColorTransform(shade));
			result.Color.Transforms.AddCore(new SaturationModulationColorTransform(satMod));
			return result;
		}
		#endregion
		#region CreateOutline
		Outline CreateOutline(int width) {
			Outline result = new Outline(theme);
			result.Width = width;
			result.EndCapStyle = OutlineEndCapStyle.Flat;
			result.CompoundType = OutlineCompoundType.Single;
			result.StrokeAlignment = OutlineStrokeAlignment.Center;
			result.Dashing = OutlineDashing.Solid;
			return result;
		}
		#endregion
		#region CreateEffects
		OuterShadowEffect CreateOuterShadowEffect(long distance, int alpha) {
			OffsetShadowInfo offsetShadowInfo = new OffsetShadowInfo(distance, 5400000);
			ScalingFactorInfo scalingFactorInfo = new ScalingFactorInfo(DrawingValueConstants.ThousandthOfPercentage, DrawingValueConstants.ThousandthOfPercentage);
			SkewAnglesInfo skewAnglesInfo = new SkewAnglesInfo(0, 0);
			OuterShadowEffectInfo shadowEffectInfo = new OuterShadowEffectInfo(offsetShadowInfo, scalingFactorInfo, skewAnglesInfo, RectangleAlignType.Bottom, 40000, false);
			DrawingColor color = DrawingColor.Create(theme, DrawingColorModelInfo.CreateARGB(DXColor.FromArgb(0, 0, 0)));
			color.Transforms.AddCore(new AlphaColorTransform(alpha));
			OuterShadowEffect result = new OuterShadowEffect(color, shadowEffectInfo);
			return result;
		}
		void AddOuterShadowEffectToContainer(OuterShadowEffect shadowEffect) {
			DrawingEffectStyle effectStyle = new DrawingEffectStyle(theme);
			effectStyle.ContainerEffect.Effects.Add(shadowEffect);
			theme.FormatScheme.EffectStyleList.Add(effectStyle);
		}
		#endregion
		#endregion
		#endregion
	}
	#endregion
}
