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
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.Office.Model;
using DevExpress.Office.DrawingML;
#if SL
using System.Windows.Media;
#endif
namespace DevExpress.Office.Drawing {
	#region DrawingColorType
	public enum DrawingColorType {
		Rgb = 0,
		System = 1,
		Scheme = 2,
		Preset = 3,
		ScRgb = 4,
		Hsl = 5
	}
	#endregion
	#region DrawingColorModelInfo
	public class DrawingColorModelInfo : ICloneable<DrawingColorModelInfo>, ISupportsCopyFrom<DrawingColorModelInfo>, ISupportsSizeOf {
		#region Static Members
		internal static DrawingColorModelInfo CreateRGB(Color rgb) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.Rgb = DXColor.FromArgb(255, rgb.R, rgb.G, rgb.B);
			return result;
		}
		public static DrawingColorModelInfo CreateARGB(Color argb) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.Rgb = argb;
			return result;
		}
		public static DrawingColorModelInfo CreateSystem(SystemColorValues systemColor) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.SystemColor = systemColor;
			return result;
		}
		public static DrawingColorModelInfo CreateScheme(SchemeColorValues schemeColor) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.SchemeColor = schemeColor;
			return result;
		}
		public static DrawingColorModelInfo CreatePreset(string preset) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.Preset = preset;
			return result;
		}
		public static DrawingColorModelInfo CreateScRgb(int scR, int scG, int scB) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.ScRgb = new ScRGBColor(scR, scG, scB);
			return result;
		}
		public static DrawingColorModelInfo CreateHSL(int hue, int saturation, int luminance) {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.Hsl = new ColorHSL(hue, saturation, luminance);
			return result;
		}
		public static Color SRgbToRgb(string hexColor) {
			string sr = hexColor.Substring(0, 2);
			string sg = hexColor.Substring(2, 2);
			string sb = hexColor.Substring(4, 2);
			int r = Int32.Parse(sr, System.Globalization.NumberStyles.HexNumber);
			int g = Int32.Parse(sg, System.Globalization.NumberStyles.HexNumber);
			int b = Int32.Parse(sb, System.Globalization.NumberStyles.HexNumber);
			return DXColor.FromArgb(r, g, b);
		}
		static Dictionary<SystemColorValues, Color> systemColorTable = CreateSystemColorTable();
		static Dictionary<SystemColorValues, Color> CreateSystemColorTable() {
			Dictionary<SystemColorValues, Color> table = new Dictionary<SystemColorValues, Color>();
			table.Add(SystemColorValues.Sc3dDkShadow, DXSystemColors.ControlDarkDark);
			table.Add(SystemColorValues.Sc3dLight, DXSystemColors.ControlLightLight);
			table.Add(SystemColorValues.ScActiveBorder, DXSystemColors.ActiveBorder);
			table.Add(SystemColorValues.ScActiveCaption, DXSystemColors.ActiveCaption);
			table.Add(SystemColorValues.ScAppWorkspace, DXSystemColors.AppWorkspace);
			table.Add(SystemColorValues.ScBackground, DXSystemColors.Desktop);
			table.Add(SystemColorValues.ScBtnFace, DXSystemColors.Control);
			table.Add(SystemColorValues.ScBtnHighlight, DXSystemColors.ControlLight);
			table.Add(SystemColorValues.ScBtnShadow, DXSystemColors.ControlDark);
			table.Add(SystemColorValues.ScBtnText, DXSystemColors.ControlText);
			table.Add(SystemColorValues.ScCaptionText, DXSystemColors.ActiveCaptionText);
			table.Add(SystemColorValues.ScGradientActiveCaption, DXSystemColors.GradientActiveCaption);
			table.Add(SystemColorValues.ScGradientInactiveCaption, DXSystemColors.GradientInactiveCaption);
			table.Add(SystemColorValues.ScGrayText, DXSystemColors.GrayText);
			table.Add(SystemColorValues.ScHighlight, DXSystemColors.Highlight);
			table.Add(SystemColorValues.ScHighlightText, DXSystemColors.HighlightText);
			table.Add(SystemColorValues.ScHotLight, DXSystemColors.HotTrack);
			table.Add(SystemColorValues.ScInactiveBorder, DXSystemColors.InactiveBorder);
			table.Add(SystemColorValues.ScInactiveCaption, DXSystemColors.InactiveCaption);
			table.Add(SystemColorValues.ScInactiveCaptionText, DXSystemColors.InactiveCaptionText);
			table.Add(SystemColorValues.ScInfoBk, DXSystemColors.Info);
			table.Add(SystemColorValues.ScInfoText, DXSystemColors.InfoText);
			table.Add(SystemColorValues.ScMenu, DXSystemColors.Menu);
			table.Add(SystemColorValues.ScMenuBar, DXSystemColors.MenuBar);
			table.Add(SystemColorValues.ScMenuHighlight, DXSystemColors.MenuHighlight);
			table.Add(SystemColorValues.ScMenuText, DXSystemColors.MenuText);
			table.Add(SystemColorValues.ScScrollBar, DXSystemColors.ScrollBar);
			table.Add(SystemColorValues.ScWindow, DXSystemColors.Window);
			table.Add(SystemColorValues.ScWindowFrame, DXSystemColors.WindowFrame);
			table.Add(SystemColorValues.ScWindowText, DXSystemColors.WindowText);
			return table;
		}
		#endregion
		#region Fields
		DrawingColorType colorType;
		Color rgb;
		SystemColorValues systemColor;
		SchemeColorValues schemeColor;
		string preset;
		ScRGBColor scRgb;
		ColorHSL hsl;
		#endregion
		public DrawingColorModelInfo() {
			RestoreDefaultValues();
		}
		#region Properties
		public DrawingColorType ColorType { get { return colorType; } }
		public Color Rgb {
			get { return rgb; }
			set {
				if (ColorType != DrawingColorType.Rgb)
					SetColorType(DrawingColorType.Rgb);
				if (rgb != value)
					rgb = value;
			}
		}
		public SystemColorValues SystemColor {
			get { return systemColor; }
			set {
				if (ColorType != DrawingColorType.System)
					SetColorType(DrawingColorType.System);
				if (systemColor != value)
					systemColor = value;
			}
		}
		public SchemeColorValues SchemeColor {
			get { return schemeColor; }
			set {
				if (ColorType != DrawingColorType.Scheme)
					SetColorType(DrawingColorType.Scheme);
				if (schemeColor != value)
					schemeColor = value;
			}
		}
		public ColorHSL Hsl {
			get { return hsl; }
			set {
				if (ColorType != DrawingColorType.Hsl)
					SetColorType(DrawingColorType.Hsl);
				if (!hsl.Equals(value))
					hsl = value;
			}
		}
		public string Preset {
			get { return preset; }
			set {
				if (ColorType != DrawingColorType.Preset)
					SetColorType(DrawingColorType.Preset);
				if (preset != value && !String.IsNullOrEmpty(value))
					preset = value;
			}
		}
		public ScRGBColor ScRgb {
			get { return scRgb; }
			set {
				if (ColorType != DrawingColorType.ScRgb)
					SetColorType(DrawingColorType.ScRgb);
				if (!scRgb.Equals(value))
					scRgb = value;
			}
		}
		public bool IsEmpty { get { return DXColor.IsTransparentOrEmpty(Rgb) && ColorType == DrawingColorType.Rgb; } }
		#endregion
		#region ToRgb
		public Color ToRgb(ThemeDrawingColorCollection colors, Color styleColor) {
			switch (colorType) {
			case DrawingColorType.System:
			return GetRgbFromSystemColor();
			case DrawingColorType.Scheme:
			return GetRgbFromSchemeColor(colors, styleColor);
			case DrawingColorType.Hsl:
			return hsl.ToRgb();
			case DrawingColorType.Preset:
			return GetRgbFromPreset();
			case DrawingColorType.ScRgb:
			return scRgb.ToRgb();
			default:
			return rgb;
			}
		}
		public Color ToRgb(ThemeDrawingColorCollection colors) {
			return ToRgb(colors, DXColor.Empty);
		}
		Color GetRgbFromPreset() {
			string key = char.ToUpper(preset[0]) + preset.Substring(1);
			if (DXColor.PredefinedColors.ContainsKey(key))
				return DXColor.PredefinedColors[key];
			return DXColor.Empty;
		}
		Color GetRgbFromSystemColor() {
			if (SystemColor != SystemColorValues.Empty)
				return systemColorTable[SystemColor];
			return DXColor.Empty;
		}
		Color GetRgbFromSchemeColor(ThemeDrawingColorCollection colors, Color styleColor) {
			if (SchemeColor == SchemeColorValues.Style)
				return styleColor;
			if (SchemeColor != SchemeColorValues.Empty)
				return colors.GetColor(SchemeColor);
			return DXColor.Empty;
		}
		#endregion
		void SetColorType(DrawingColorType colorType) {
			RestoreDefaultValues();
			this.colorType = colorType;
		}
		void RestoreDefaultValues() {
			this.rgb = DXColor.Empty;
			this.schemeColor = SchemeColorValues.Empty;
			this.systemColor = SystemColorValues.Empty;
			this.preset = String.Empty;
			this.scRgb = ScRGBColor.DefaultValue;
			this.hsl = ColorHSL.DefaultValue;
		}
		#region ICloneable<DrawingColorModelInfo> Members
		public DrawingColorModelInfo Clone() {
			DrawingColorModelInfo result = new DrawingColorModelInfo();
			result.CopyFrom(this);
			return result;
		}
		#endregion
		#region ISupportsCopyFrom<DrawingColorModelInfo> Members
		public void CopyFrom(DrawingColorModelInfo value) {
			Guard.ArgumentNotNull(value, "DrawingColorInfo");
			this.colorType = value.colorType;
			this.rgb = value.rgb;
			this.schemeColor = value.schemeColor;
			this.systemColor = value.systemColor;
			this.preset = value.preset;
			this.scRgb = value.scRgb;
			this.hsl = value.hsl;
		}
		#endregion
		#region ISupportsSizeOf Members
		public int SizeOf() {
			return DXMarshal.SizeOf(GetType());
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingColorModelInfo info = obj as DrawingColorModelInfo;
			if (info == null)
				return false;
			return
				this.colorType == info.colorType &&
				this.rgb == info.Rgb &&
				this.schemeColor == info.schemeColor &&
				this.systemColor == info.systemColor &&
				this.preset == info.preset &&
				this.scRgb.Equals(info.scRgb) &&
				this.hsl.Equals(info.hsl);
		}
		public override int GetHashCode() {
			return
				(int)colorType ^ rgb.GetHashCode() ^ schemeColor.GetHashCode() ^
				systemColor.GetHashCode() ^ preset.GetHashCode() ^
				scRgb.GetHashCode() ^ hsl.GetHashCode();
		}
		#endregion
	}
	#endregion
	#region DrawingColorModelInfoCache
	public class DrawingColorModelInfoCache : UniqueItemsCache<DrawingColorModelInfo> {
		public const int DefaultItemIndex = 0;
		public DrawingColorModelInfoCache(IDocumentModelUnitConverter converter)
			: base(converter) {
		}
		protected override DrawingColorModelInfo CreateDefaultItem(IDocumentModelUnitConverter unitConverter) {
			return new DrawingColorModelInfo();
		}
	}
	#endregion
	#region IDrawingOriginalColor
	public interface IDrawingOriginalColor {
		Color Rgb { get; set; }
		SystemColorValues System { get; set; }
		SchemeColorValues Scheme { get; set; }
		string Preset { get; set; }
		ColorHSL Hsl { get; set; }
		ScRGBColor ScRgb { get; set; }
		ColorTransformCollection Transforms { get; }
		void SetColorFromRGB(Color rgb);
	}
	#endregion
	#region IDrawingColor
	public interface IDrawingColor : IDrawingOriginalColor {
		Color FinalColor { get; }
		DrawingColorType ColorType { get; }
		bool IsEmpty { get; }
		Color ToRgb(Color styleColor);
		DrawingColor CloneTo(IDocumentModel documentModel);
	}
	#endregion
	#region DrawingColor
	public class DrawingColor : DrawingUndoableIndexBasedObject<DrawingColorModelInfo>, ISupportsCopyFrom<DrawingColor>, IDrawingColor, IDrawingBullet {
		#region Static Members
		public static DrawingColor Create(IDocumentModel documentModel, DrawingColorModelInfo colorInfo) {
			DrawingColor result = new DrawingColor(documentModel);
			result.AssignInfo(colorInfo);
			return result;
		}
		public static DrawingColor Create(IDocumentModel documentModel, Color color) {
			DrawingColor result= new DrawingColor(documentModel);
			result.AssignColorRGB(color);
			return result;
		}
		#endregion
		readonly ColorTransformCollection transforms;
		public DrawingColor(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
			transforms = new ColorTransformCollection(documentModel);
		}
		#region Properties
		bool HasColorTransform { get { return transforms.Count != 0; } }
		#region IDrawingColor Members
		public bool IsEmpty { get { return !HasColorTransform && Info.IsEmpty; } }
		public Color FinalColor { get { return ToRgb(DXColor.Empty); } }
		public ColorTransformCollection Transforms { get { return transforms; } }
		public IDrawingOriginalColor OriginalColor { get { return this; } }
		#region IDrawingOriginalColor
		void IDrawingOriginalColor.SetColorFromRGB(Color rgb) {
			int a = rgb.A;
			if (a == 255)
				SetARGBColor(rgb);
			else {
				DocumentModel.BeginUpdate();
				try {
					SetARGBColor(DXColor.FromArgb(255, rgb.R, rgb.G, rgb.B));
					transforms.Add(AlphaColorTransform.CreateFromA(a));
				} finally {
					DocumentModel.EndUpdate();
				}
			}
		}
		Color IDrawingOriginalColor.Rgb {
			get { return Info.Rgb; }
			set { SetARGBColor(value); }
		}
		void SetARGBColor(Color argb) {
			if (Info.Rgb == argb && Info.ColorType == DrawingColorType.Rgb)
				return;
			SetPropertyValue(SetARGBColorCore, argb);
		}
		DocumentModelChangeActions SetARGBColorCore(DrawingColorModelInfo info, Color value) {
			info.Rgb = value;
			return DocumentModelChangeActions.None; 
		}
		SystemColorValues IDrawingOriginalColor.System {
			get { return Info.SystemColor; }
			set {
				if (Info.SystemColor == value && Info.ColorType == DrawingColorType.System)
					return;
				SetPropertyValue(SetSystemColor, value);
			}
		}
		DocumentModelChangeActions SetSystemColor(DrawingColorModelInfo info, SystemColorValues value) {
			info.SystemColor = value;
			return DocumentModelChangeActions.None; 
		}
		SchemeColorValues IDrawingOriginalColor.Scheme {
			get { return Info.SchemeColor; }
			set {
				if (Info.SchemeColor == value && Info.ColorType == DrawingColorType.Scheme)
					return;
				SetPropertyValue(SetSchemeColor, value);
			}
		}
		DocumentModelChangeActions SetSchemeColor(DrawingColorModelInfo info, SchemeColorValues value) {
			info.SchemeColor = value;
			return DocumentModelChangeActions.None; 
		}
		string IDrawingOriginalColor.Preset {
			get { return Info.Preset; }
			set {
				if (Info.Preset == value && Info.ColorType == DrawingColorType.Preset)
					return;
				SetPropertyValue(SetPresetColor, value);
			}
		}
		DocumentModelChangeActions SetPresetColor(DrawingColorModelInfo info, string value) {
			info.Preset = value;
			return DocumentModelChangeActions.None; 
		}
		ColorHSL IDrawingOriginalColor.Hsl {
			get { return Info.Hsl; }
			set {
				if (Info.Hsl.Equals(value) && Info.ColorType == DrawingColorType.Hsl)
					return;
				SetPropertyValue(SetHslColor, value);
			}
		}
		DocumentModelChangeActions SetHslColor(DrawingColorModelInfo info, ColorHSL value) {
			info.Hsl = value;
			return DocumentModelChangeActions.None; 
		}
		ScRGBColor IDrawingOriginalColor.ScRgb {
			get { return Info.ScRgb; }
			set {
				if (Info.ScRgb.Equals(value) && Info.ColorType == DrawingColorType.ScRgb)
					return;
				SetPropertyValue(SetScRgbColor, value);
			}
		}
		DocumentModelChangeActions SetScRgbColor(DrawingColorModelInfo info, ScRGBColor value) {
			info.ScRgb = value;
			return DocumentModelChangeActions.None; 
		}
		#endregion
		public DrawingColorType ColorType { get { return Info.ColorType; } }
		#endregion
		#endregion
		protected internal override UniqueItemsCache<DrawingColorModelInfo> GetCache(IDocumentModel documentModel) {
			return DrawingCache.DrawingColorModelInfoCache;
		}
		public Color ToRgb(Color styleColor) {
			ThemeDrawingColorCollection colors = DocumentModel.OfficeTheme.Colors;
			Color result = Info.ToRgb(colors, styleColor);
			return HasColorTransform ? transforms.ApplyTransform(result) : result;
		}
		public DrawingColor CloneTo(IDocumentModel documentModel) {
			DrawingColor clone = new DrawingColor(documentModel);
			clone.CopyFrom(this);
			return clone;
		}
		public void CopyFrom(DrawingColor other) {
			base.CopyFrom(other);
			transforms.CopyFrom(other.transforms);
		}
		void AssignColorRGB(Color color) {
			int a = color.A;
			if (a == 255)
				AssignInfo(DrawingColorModelInfo.CreateARGB(color));
			else {
				AssignInfo(DrawingColorModelInfo.CreateRGB(color));
				Transforms.AddInternal(AlphaColorTransform.CreateFromA(a));
			}
		}
		#region IDrawingBullet Members
		public DrawingBulletType Type { get { return DrawingBulletType.Color; } }
		IDrawingBullet IDrawingBullet.CloneTo(IDocumentModel documentModel) {
			return CloneTo(documentModel);
		}
		public void Visit(IDrawingBulletVisitor visitor) {
			visitor.Visit(this);
		}
		#endregion
		#region Equals
		public override bool Equals(object obj) {
			DrawingColor other = obj as DrawingColor;
			if (other == null)
				return false;
			return Index == other.Index && transforms.Equals(other.transforms);
		}
		public override int GetHashCode() {
			return Index ^ transforms.GetHashCode();
		}
		#endregion
		public void Clear() {
			if (IsUpdateLocked)
				Info.CopyFrom(DrawingCache.DrawingColorModelInfoCache.DefaultItem);
			else
				ChangeIndexCore(DrawingColorModelInfoCache.DefaultItemIndex, DocumentModelChangeActions.None);
			transforms.Clear();
		}
	}
	#endregion
}
