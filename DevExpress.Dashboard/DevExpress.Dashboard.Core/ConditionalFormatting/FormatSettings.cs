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

using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon.Native {
	public interface IStyleSettings : IXmlSerializableElement {
		void Assign(IStyleSettings obj);
		IStyleSettings Clone();
		StyleSettingsModel CreateViewModel();
	}
	public interface IBackColorStyleSettings : IStyleSettings {
		Color? BackColor { get; set; }
		FormatConditionAppearanceType AppearanceType { get; }
	}
	public interface IBarColorStyleSettings : IStyleSettings {
		Color? Color { get; set; }
		FormatConditionAppearanceType PredefinedColor { get; set; }
	}
}
namespace DevExpress.DashboardCommon {
	public abstract class StyleSettingsBase : IStyleSettings {
		IFormatStyleSettingsOwner owner;
		int lockUpdate;
		internal IFormatStyleSettingsOwner Owner {
			get { return owner; }
			set { this.owner = value; }
		}
		protected abstract void AssignCore(IStyleSettings obj);
		protected abstract IStyleSettings CreateInstance();
		protected abstract void SaveToXml(XElement element);
		protected abstract void LoadFromXml(XElement element);
		protected abstract StyleSettingsModel CreateViewModel();
		public void BeginUpdate() {
			lockUpdate++;
		}
		public void EndUpdate() {
			if(--lockUpdate == 0) {
				OnChanged();
			}
		}
		public override bool Equals(object obj) {
			return obj is StyleSettingsBase;
		}
		public override int GetHashCode() {
			return 0;
		}
		protected internal virtual void OnChanged() {
			if(Owner != null)
				Owner.OnChanged();
		}
		#region IStyleSettings Members
		void IStyleSettings.Assign(IStyleSettings obj) {
			BeginUpdate();
			try {
				AssignCore(obj);
			}
			finally {
				EndUpdate();
			}
		}
		IStyleSettings IStyleSettings.Clone() {
			var res = CreateInstance();
			res.Assign(this);
			return res;
		}
		StyleSettingsModel IStyleSettings.CreateViewModel() {
			return CreateViewModel();
		}
		#endregion
		#region IXmlSerializableElement Members
		void IXmlSerializableElement.SaveToXml(XElement element) {
			SaveToXml(element);
		}
		void IXmlSerializableElement.LoadFromXml(XElement element) {
			LoadFromXml(element);
		}
		#endregion
	}
	public class AppearanceSettings : StyleSettingsBase, IBackColorStyleSettings {
		const string XmlAppearanceType = "AppearanceType";
		const string XmlForeColor = "ForeColor";
		const string XmlBackColor = "BackColor";
		const string XmlFontFamily = "FontFamily";
		const string XmlFontSize = "FontSize";
		const string XmlFontStyle = "FontStyle";		
		const string DefaultFontFamily = null;
		const float DefaultFontSize = -1;
		const FormatConditionAppearanceType DefaultAppearanceType = FormatConditionAppearanceType.None;
		FormatConditionAppearanceType appearanceType = DefaultAppearanceType;
		Color? backColor;
		string fontFamily = DefaultFontFamily;
		float fontSize = DefaultFontSize;
		FontStyle? fontStyle;
		Color? foreColor;
		[
		DefaultValue(DefaultAppearanceType)
		]
		public FormatConditionAppearanceType AppearanceType {
			get { return appearanceType; }
			set {
				if(AppearanceType != value) {
					appearanceType = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(null)
		]
		public Color? BackColor {
			get { return backColor; }
			set {
				if(BackColor != value) {
					backColor = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(null)
		]
		public Color? ForeColor {
			get { return foreColor; }
			set {
				if(ForeColor != value) {
					foreColor = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultFontFamily),
		Localizable(false)
		]
		public string FontFamily {
			get { return fontFamily; }
			set {
				if(FontFamily != value) {
					fontFamily = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultFontSize)
		]
		internal float FontSize {
			get { return fontSize; }
			set {
				if(FontSize != value) {
					fontSize = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(null)
		]
		public FontStyle? FontStyle {
			get { return fontStyle; }
			set {
				if(FontStyle != value) {
					fontStyle = value;
					OnChanged();
				}
			}
		}
		public AppearanceSettings() {
		}
		public AppearanceSettings(FormatConditionAppearanceType predefinedType)
			: this() {
			this.appearanceType = predefinedType;
		}
		public AppearanceSettings(Color backColor)
			: this(FormatConditionAppearanceType.Custom) {
			this.backColor = backColor;
		}
		public AppearanceSettings(Color backColor, Color foreColor)
			: this(backColor) {
			this.foreColor = foreColor;
		}
		public AppearanceSettings(FontStyle fontStyle)
			: this(FormatConditionAppearanceType.Custom) {
			this.fontStyle = fontStyle;
		}
		public AppearanceSettings(Color foreColor, FontStyle fontStyle) : this(fontStyle) {
			this.foreColor = foreColor;
		}
		protected override void AssignCore(IStyleSettings obj) {
			var source = obj as AppearanceSettings;
			if(source != null) {
				this.appearanceType = source.AppearanceType;
				this.backColor = source.BackColor;
				this.fontFamily = source.FontFamily;
				this.fontSize = source.FontSize;
				this.fontStyle = source.FontStyle;
				this.foreColor = source.ForeColor;
			}
		}
		protected override IStyleSettings CreateInstance() {
			return new AppearanceSettings();
		}
		protected override void SaveToXml(XElement element) {
			XmlHelper.Save(element, XmlAppearanceType, AppearanceType, DefaultAppearanceType);
			if(BackColor.HasValue)
				XmlHelper.Save(element, XmlBackColor, BackColor.Value.ToArgb());
			XmlHelper.Save(element, XmlFontFamily, FontFamily, DefaultFontFamily);
			if(FontStyle.HasValue)
				XmlHelper.Save(element, XmlFontStyle, FontStyle.Value);
			if(ForeColor.HasValue)
				XmlHelper.Save(element, XmlForeColor, ForeColor.Value.ToArgb());
		}
		protected override void LoadFromXml(XElement element) {
			XmlHelper.LoadEnum<FormatConditionAppearanceType>(element, XmlAppearanceType, x => appearanceType = x);
			XmlHelper.LoadColor(element, XmlBackColor, x => backColor = x);
			XmlHelper.Load<string>(element, XmlFontFamily, x => fontFamily = x);
			XmlHelper.LoadEnum<FontStyle>(element, XmlFontStyle, x => fontStyle = x);
			XmlHelper.LoadColor(element, XmlForeColor, x => foreColor = x);
		}
		protected override StyleSettingsModel CreateViewModel() {
			StyleSettingsModel viewModel = new StyleSettingsModel(AppearanceType);
			if(BackColor.HasValue)
				viewModel.Color = BackColor.Value.ToArgb();
			viewModel.FontFamily = FontFamily;
			viewModel.FontSize = FontSize;
			if(FontStyle.HasValue)
				viewModel.FontStyle = (byte)FontStyle.Value;
			if(ForeColor.HasValue)
				viewModel.ForeColor = ForeColor.Value.ToArgb();
			return viewModel;
		}
		public override bool Equals(object obj) {
			if (base.Equals(obj)) {
				AppearanceSettings settings = obj as AppearanceSettings;
				if (settings != null) 
					return 
						settings.AppearanceType == AppearanceType &&
						settings.BackColor == BackColor &&
						settings.ForeColor == ForeColor &&
						settings.FontFamily == FontFamily &&
						settings.FontSize == FontSize &&
						settings.FontStyle == FontStyle;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^
				AppearanceType.GetHashCode() ^
				(BackColor != null ? BackColor.GetHashCode() : 0) ^
				(ForeColor != null ? ForeColor.GetHashCode() : 0) ^ 
				(FontFamily != null ? FontFamily.GetHashCode() : 0) ^ 
				FontSize.GetHashCode() ^ 
				(FontStyle != null ? FontStyle.GetHashCode() : 0);
		}
	}
	public class IconSettings : StyleSettingsBase {
		const string XmlIconType = "IconType";
		const FormatConditionIconType DefaultIconType = FormatConditionIconType.None;
		readonly DashboardImage image = new DashboardImage();
		FormatConditionIconType iconType;
		[
		Category(CategoryNames.Format),
		Editor(TypeNames.ImageFileNameEditor, typeof(UITypeEditor)),
		DefaultValue(null),
		Localizable(false)
		]
		internal string Url { get { return image.Url; } set { image.Url = value; } }
		[
		Category(CategoryNames.Format),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		TypeConverter(TypeNames.DisplayNameNoneObjectConverter),
		Editor(TypeNames.ImageDataEditor, typeof(UITypeEditor))
		]
		internal byte[] Data { get { return image.Data; } set { image.Data = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DefaultValue(null)
		]
		internal string DataSerializable { get { return image.Base64Data; } set { image.Base64Data = value; } }
		[
		DefaultValue(DefaultIconType)
		]
		public FormatConditionIconType IconType {
			get { return iconType; }
			set {
				if(IconType != value) {
					iconType = value;
					OnChanged();
				}
			}
		}
		public IconSettings() {
		}
		public IconSettings(FormatConditionIconType iconType) : this() {
			this.iconType = iconType;
		}
		protected override void AssignCore(IStyleSettings obj) {
			var source = obj as IconSettings;
			if(source != null) {
				this.Data = source.Data;
				this.Url = source.Url;
				this.IconType = source.IconType;
			}
		}
		protected override IStyleSettings CreateInstance() {
			return new IconSettings();
		}
		protected override void SaveToXml(XElement element) {
			XmlHelper.Save(element, XmlIconType, IconType, DefaultIconType);
		}
		protected override void LoadFromXml(XElement element) {
			XmlHelper.Load<FormatConditionIconType>(element, XmlIconType, x => iconType = x);
		}
		protected override StyleSettingsModel CreateViewModel() {
			return new StyleSettingsModel(IconType);
		}
		public override bool Equals(object obj) {
			if (base.Equals(obj)) {
				IconSettings settings = obj as IconSettings;
				if(settings != null)
					return settings.IconType == IconType &&
						settings.Data == Data &&
						settings.Url == Url;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^
				IconType.GetHashCode() ^ 
				(Data != null ? Data.GetHashCode() : 0) ^ 
				(Url != null ? Url.GetHashCode() : 0);
		}
	}
	public class BarStyleSettings : StyleSettingsBase, IBarColorStyleSettings {
		const string XmlColor = "Color";
		const string XmlPredefinedColor = "PredefinedColor";
		const FormatConditionAppearanceType DefaultPredefinedColor = FormatConditionAppearanceType.None;
		Color? color;
		FormatConditionAppearanceType predefinedColor = DefaultPredefinedColor;
		[
		DefaultValue(null)
		]
		public Color? Color {
			get { return color; }
			set {
				if(Color != value) {
					color = value;
					OnChanged();
				}
			}
		}
		[
		DefaultValue(DefaultPredefinedColor)
		]
		public FormatConditionAppearanceType PredefinedColor {
			get { return predefinedColor; }
			set {
				if(PredefinedColor != value) {
					predefinedColor = value;
					OnChanged();
				}
			}
		}
		public BarStyleSettings() {
		}
		public BarStyleSettings(FormatConditionAppearanceType predefinedColor)
			: this() {
			this.predefinedColor = predefinedColor;
		}
		public BarStyleSettings(Color color)
			: this(FormatConditionAppearanceType.Custom) {
			this.color = color;
		}
		protected override void AssignCore(IStyleSettings obj) {
			var source = obj as BarStyleSettings;
			if(source != null) {
				Color = source.Color;
				PredefinedColor = source.PredefinedColor;
			}
		}
		protected override IStyleSettings CreateInstance() {
			return new BarStyleSettings();
		}
		protected override void SaveToXml(XElement element) {
			if(Color.HasValue)
				XmlHelper.Save(element, XmlColor, Color.Value.ToArgb());
			XmlHelper.Save(element, XmlPredefinedColor, PredefinedColor, DefaultPredefinedColor);
		}
		protected override void LoadFromXml(XElement element) {
			XmlHelper.LoadColor(element, XmlColor, x => color = x);
			XmlHelper.LoadEnum<FormatConditionAppearanceType>(element, XmlPredefinedColor, x => predefinedColor = x);
		}
		protected override StyleSettingsModel CreateViewModel() {
			StyleSettingsModel viewModel = new StyleSettingsModel() {
				AppearanceType = PredefinedColor,
				IsBarStyle = true
			};
			if(Color.HasValue)
				viewModel.Color = Color.Value.ToArgb();
			return viewModel;
		}
		public override bool Equals(object obj) {
			if(base.Equals(obj)) {
				BarStyleSettings settings = obj as BarStyleSettings;
				if(settings != null)
					return 
						settings.Color == Color &&
						settings.PredefinedColor == PredefinedColor;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^
				(Color != null ? Color.GetHashCode() : 0) ^
				PredefinedColor.GetHashCode();
		}
	}
}
namespace DevExpress.DashboardCommon.Native {
	public class RangeIndexSettings : StyleSettingsBase {
		public int Index { get; set; }
		public FormatConditionRangeGradient Condition { get; set; }
		public bool IsBarStyle { get; set; }
		public RangeIndexSettings() {
		}
		public RangeIndexSettings(FormatConditionRangeGradient condition, int index)
			: this() {
			Condition = condition;
			Index = index;
		}
		public RangeIndexSettings(FormatConditionRangeGradient condition, int index, bool isBarStyle)
			: this(condition, index) {
			IsBarStyle = isBarStyle;
		}
		protected override void AssignCore(IStyleSettings obj) {
			var source = obj as RangeIndexSettings;
			if(source != null) {
				Index = source.Index;
				Condition = source.Condition;
			}
		}
		protected override IStyleSettings CreateInstance() {
			return new RangeIndexSettings();
		}
		protected override void SaveToXml(XElement element) {
		}
		protected override void LoadFromXml(XElement element) {
		}
		protected override StyleSettingsModel CreateViewModel() {
			return new StyleSettingsModel {
				RangeIndex = Index,
				IsBarStyle = IsBarStyle
			};
		}
		public override bool Equals(object obj) {
			if (base.Equals(obj)) {
				RangeIndexSettings settings = obj as RangeIndexSettings;
				if (settings != null)
					return settings.Index == Index && settings.Condition == Condition;
			}
			return false;
		}
		public override int GetHashCode() {
			return base.GetHashCode() ^ Index.GetHashCode() ^ Condition.GetHashCode();
		}
	}
}
