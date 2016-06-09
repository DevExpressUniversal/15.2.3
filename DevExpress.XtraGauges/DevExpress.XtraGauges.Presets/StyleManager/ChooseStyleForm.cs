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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Styles;
using DevExpress.XtraGauges.Presets.Localization;
using DevExpress.XtraGauges.Presets.PresetManager;
namespace DevExpress.XtraGauges.Presets.Styles {
	public partial class ChooseStyleForm : XtraForm {
		IGauge gaugeCore;
		ChooseStyleForm()
			: this(null) {
		}
		public ChooseStyleForm(IGauge gauge) {
			gaugeCore = gauge;
			InitializeComponent();
			layout.AllowCustomization = false;
			StartPosition = FormStartPosition.CenterParent;
			CanEditValue(gauge, out editValueCore);
			keys = new List<StyleCollectionKey>();
			items = new Dictionary<StyleCollectionKey, GalleryItem>();
			gallery.SelectedIndexChanged += OnSelectedIndexChanged;
			gallery.ItemDoubleClick += OnItemDoubleClick;
			checkUseFilter.CheckedChanged += UseFilterChanged;
			Initialize();
		}
		int styleChangedCore;
		public bool IsStyleChanged {
			get { return styleChangedCore > 0; }
		}
		protected override bool GetAllowSkin() {
			bool isDesignTime = (Gauge != null && Gauge.Container != null) && (Gauge.Site != null || Gauge.Container.EnableCustomizationMode);
			bool isSkin = (LookAndFeel != null) && LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin;
			return (isDesignTime && isSkin) || base.GetAllowSkin();
		}
		void UseFilterChanged(object sender, EventArgs e) {
			cbFilter.Enabled = checkUseFilter.Checked;
			UpdateGallery();
		}
		public IGauge Gauge {
			get { return gaugeCore; }
		}
		public static bool CanEditValue(IGauge gauge, out StyleCollectionKey result) {
			result = null;
			if(gauge == null)
				return false;
			string scope = string.Empty;
			Type gaugeType = gauge.GetType();
			switch(gaugeType.Name) {
				case "DigitalGauge": scope = "Digital"; break;
				case "CircularGauge": scope = "Circular"; break;
				case "LinearGauge": scope = "Linear"; break;
				default:
					return false;
			}
			result = new StyleCollectionKey(scope);
			result.Name = "Default";
			var themeNameResolutionService = Core.Styles.StyleCollectionHelper.GetService<IThemeNameResolutionService>();
			List<ISerizalizeableElement> children = ((ISerizalizeableElement)gauge).GetChildren();
			foreach(BaseObject child in children) {
				Type childType = child.GetType();
				if(childType.Name.StartsWith("LinearScale")) {
					if(scope == "Linear") {
						PropertyDescriptor orientation = TypeDescriptor.GetProperties(gaugeType)["Orientation"];
						if(orientation != null)
							result.Tag = orientation.GetValue(gauge).ToString();
					}
					break;
				}
				if(childType.Name.Contains("BackgroundLayer")) {
					PropertyDescriptor shapeType = TypeDescriptor.GetProperties(childType)["ShapeType"];
					if(shapeType != null) {
						string value = shapeType.GetValue(child).ToString();
						value = value.Replace(scope, string.Empty);
						value = value.TrimStart('_');
						value = value.Replace("_1", string.Empty);
						string themeName = themeNameResolutionService.Resolve(value);
						if(!string.IsNullOrEmpty(themeName)) {
							result.Name = themeName;
							if(scope == "Linear") {
								PropertyDescriptor orientation = TypeDescriptor.GetProperties(gaugeType)["Orientation"];
								if(orientation != null)
									result.Tag = orientation.GetValue(gauge).ToString();
							}
							break;
						}
						string[] parts = value.Split('_');
						if(parts.Length == 2) {
							themeName = parts[1];
							if(themeName.EndsWith("Left")) {
								result.Name = themeName.Replace("Left", string.Empty);
								result.Tag = parts[0] + "Left";
								break;
							}
							if(themeName.EndsWith("Right")) {
								result.Name = themeName.Replace("Right", string.Empty);
								result.Tag = parts[0] + "Right";
								break;
							}
							result.Name = themeName;
							result.Tag = parts[0];
						}
					}
				}
			}
			result.Name = themeNameResolutionService.Resolve(result.Name);
			if(result.Scope == "Circular" || result.Scope == "Linear") {
				bool dark;
				if(IsFlatGauge(gauge, out dark))
					result.Name = themeNameResolutionService.Resolve(dark ? "FlatDark" : "FlatLight");
				return result.Tag != null;
			}
			return true;
		}
		static bool IsFlatGauge(IGauge gauge, out bool dark) {
			dark = true;
			object scale = null;
			var children = ((ISerizalizeableElement)gauge).GetChildren();
			foreach(BaseObject child in children) {
				Type childType = child.GetType();
				if(childType.Name.StartsWith("LinearScaleComponent"))
					scale = child;
				if(childType.Name.StartsWith("ArcScaleComponent"))
					scale = child;
				if(childType.Name.Contains("BackgroundLayer"))
					return false;
			}
			if(scale != null) {
				PropertyDescriptor appearanceScale = TypeDescriptor.GetProperties(scale.GetType())["AppearanceScale"];
				if(appearanceScale != null) {
					var appearance = appearanceScale.GetValue(scale) as DevExpress.XtraGauges.Core.Drawing.BaseScaleAppearance;
					if(appearance != null && appearance.Brush != null) {
						var detector = new DarkSchemeDetector();
						appearance.Brush.Accept(detector);
						dark = detector.IsDark;
					}
				}
			}
			return true;
		}
		class DarkSchemeDetector : DevExpress.XtraGauges.Core.Drawing.IColorShader {
			public bool IsEmpty {
				get { return false; }
			}
			public bool IsDark { get; private set; }
			public void Process(ref System.Drawing.Color[] colors) {
				for(int i = 0; i < colors.Length; i++)
					IsDark |= IsDarkColor(colors[i]);
			}
			public void Process(ref System.Drawing.Color sourceColor) {
				IsDark = IsDarkColor(sourceColor);
			}
			static bool IsDarkColor(System.Drawing.Color source) {
				return (((float)source.R * 0.3f + (float)source.G * 0.59f + (float)source.B * 0.11f) / 255f) < 0.5f;
			}
		}
		StyleCollectionKey editValueCore;
		public StyleCollectionKey EditValue {
			get { return editValueCore; }
		}
		public StyleCollectionKey GetResult() {
			if(gallery.SelectedItem == null) return null;
			return ((StylesGalleryItem)gallery.SelectedItem).Key;
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			if(Gauge != null) {
				InitFilter();
				UpdateEnabledState();
			}
		}
		protected void InitFilter() {
			if(!string.IsNullOrEmpty(EditValue.Name) && string.IsNullOrEmpty(EditValue.Tag))
				checkUseFilter.Checked = true;
			cbFilter.Properties.Items.AddRange(new ImageComboBoxItem[] {  
				new ImageComboBoxItem(GaugesPresetsLocalizer.GetString(GaugesPresetsStringId.StyleChooserFilterShape), 0),
				new ImageComboBoxItem(GaugesPresetsLocalizer.GetString(GaugesPresetsStringId.StyleChooserFilterTheme), 1)});
			cbFilter.SelectedIndex = 0;
			cbFilter.SelectedIndexChanged += OnFilterChanged;
			UpdateGallery();
		}
		void btnOk_Click(object sender, EventArgs e) {
			CloseForm();
		}
		void btnApply_Click(object sender, EventArgs e) {
			Apply(GetResult());
		}
		public bool Apply(StyleCollectionKey key) {
			if(key == null || object.Equals(key, EditValue))
				return false;
			StyleCollection styles = StyleLoader.Load(key);
			styles.Apply(Gauge);
			editValueCore = key;
			styleChangedCore++;
			UpdateEnabledState();
			return true;
		}
		void OnItemDoubleClick(object sender, EventArgs e) {
			StyleCollectionKey result = GetResult();
			if(result == null || object.Equals(result, EditValue))
				DialogResult = DialogResult.Cancel;
			else {
				styleChangedCore++;
				CloseForm();
			}
		}
		void OnSelectedIndexChanged(object sender, EventArgs e) {
			UpdateEnabledState();
		}
		IList<StyleCollectionKey> keys;
		IDictionary<StyleCollectionKey, GalleryItem> items;
		void OnFilterChanged(object sender, EventArgs e) {
			UpdateGallery();
		}
		protected void UpdateGallery() {
			bool byTheme = cbFilter.SelectedIndex == 1;
			bool byTag = cbFilter.SelectedIndex == 0;
			StyleCollectionKey selected = GetResult() ?? EditValue;
			var themeNameResolver = Core.Styles.StyleCollectionHelper.GetService<IThemeNameResolutionService>();
			list_FlatStyles = new string[] {
				themeNameResolver.Resolve("Style27"),
				themeNameResolver.Resolve("Style28")
			};
			skipList_SkinnedStyles = new string[] {
				themeNameResolver.Resolve("Style29"),
				themeNameResolver.Resolve("Style30")
			};
			string theme = byTheme && (selected != null) ? themeNameResolver.Resolve(selected.Name) : null;
			string tag = byTag && (selected != null) ? selected.Tag : null;
			bool isFlat = false;
			if(selected.Scope != "Digital") {
				bool dark;
				if(IsFlatGauge(Gauge, out dark))
					isFlat = true;
			}
			UpdateKeys(theme, tag, selected, isFlat);
			UpdateGalleryItems(byTheme, !checkUseFilter.Checked, selected);
		}
		string[] skipList_SkinnedStyles;
		string[] list_FlatStyles;
		void UpdateKeys(string theme, string tag, StyleCollectionKey selected, bool isFlat) {
			keys.Clear();
			bool byTag = string.IsNullOrEmpty(theme);
			bool byTheme = string.IsNullOrEmpty(tag);
			bool showAll = !checkUseFilter.Checked || (byTag && byTheme);
			foreach(KeyValuePair<StyleCollectionKey, string> pair in StyleLoader.Resources) {
				StyleCollectionKey key = pair.Key;
				if(key.Scope != selected.Scope) continue;
				if(Array.IndexOf(skipList_SkinnedStyles, key.Name) != -1)
					continue;
				bool flatStyle = Array.IndexOf(list_FlatStyles, key.Name) != -1;
				if((flatStyle ^ isFlat) && selected.Scope == "Linear")
					continue;
				if(!showAll) {
					if(byTheme && key.Name != theme) continue;
					if(byTag && key.Tag != tag) continue;
				}
				keys.Add(key);
			}
		}
		void UpdateGalleryItems(bool byTheme, bool useFullName, StyleCollectionKey selected) {
			gallery.SelectedIndex = -1;
			gallery.Items.Clear();
			GalleryItem[] gItems = new GalleryItem[keys.Count];
			for(int i = 0; i < keys.Count; i++) {
				StyleCollectionKey key = keys[i];
				GalleryItem item;
				if(!items.TryGetValue(key, out item)) {
					item = new StylesGalleryItem(key, () => StyleLoader.Preview(key));
					items.Add(key, item);
				}
				string theme = Theme.FromString(key.Name).Name;
				bool hasTag = !string.IsNullOrEmpty(key.Tag);
				if(useFullName)
					item.Name = string.Format(hasTag ? "{0} ({1})" : "{0}", theme, GetLocalizedTagString(key.Tag));
				else item.Name = byTheme ? GetLocalizedTagString(key.Tag) : theme;
				gItems[i] = item;
			}
			Sort(gItems);
			gallery.Items = new GalleryItemCollection(gItems);
			GalleryItem selectedItem;
			if(items.TryGetValue(selected, out selectedItem)) {
				gallery.SelectedIndex = Array.IndexOf(gItems, selectedItem);
				gallery.Scroll.Value = Math.Max(0, Math.Min(gallery.SelectedIndex, gallery.Scroll.Maximum));
			}
		}
		void Sort(GalleryItem[] gItems) {
			Array.Sort(gItems, (item1, item2) =>
			{
				if(item1 == item2) return 0;
				return item1.Name.CompareTo(item2.Name);
			});
		}
		static string GetLocalizedTagString(string tag) {
			if(string.IsNullOrEmpty(tag)) return string.Empty;
			GaugesPresetsStringId id = GaugesPresetsStringId.ShapeFull;
			switch(tag) {
				case "Full": id = GaugesPresetsStringId.ShapeFull; break;
				case "Half": id = GaugesPresetsStringId.ShapeHalf; break;
				case "QuarterLeft": id = GaugesPresetsStringId.ShapeQuarterLeft; break;
				case "QuarterRight": id = GaugesPresetsStringId.ShapeQuarterRight; break;
				case "ThreeFourth": id = GaugesPresetsStringId.ShapeThreeFourth; break;
				case "Wide": id = GaugesPresetsStringId.ShapeWide; break;
				case "Horizontal": id = GaugesPresetsStringId.ShapeHorizontal; break;
				case "Vertical": id = GaugesPresetsStringId.ShapeVertical; break;
			}
			return GaugesPresetsLocalizer.Active.GetLocalizedString(id);
		}
		protected virtual void UpdateEnabledState() {
			StyleCollectionKey result = GetResult();
			btnOk.Enabled = btnApply.Enabled = (result != null) && !object.Equals(EditValue, result);
			if(EditValue != null && EditValue.Scope == "Digital") {
				groupFilter.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
			}
		}
		protected virtual void CloseForm() {
			DialogResult = DialogResult.OK;
		}
		protected virtual void Initialize() {
			ClientSize = new System.Drawing.Size(600, 474);
		}
		public static bool Show(IGauge gauge) {
			StyleCollectionKey result;
			if(gauge == null || !ChooseStyleForm.CanEditValue(gauge, out result))
				return false;
			using(ChooseStyleForm styleChooser = new ChooseStyleForm(gauge)) {
				if(styleChooser.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
					return styleChooser.Apply(styleChooser.GetResult());
				}
				return styleChooser.IsStyleChanged;
			}
		}
	}
	[ToolboxItem(false)]
	public class StylesGallery : GalleryControl {
		protected override BorderStyles BorderStyle {
			get { return BorderStyles.NoBorder; }
		}
		protected override ScrollBarBase CreateScrollBar() {
			return new DevExpress.XtraEditors.VScrollBar();
		}
	}
	class StylesGalleryItem : GalleryItem {
		public StylesGalleryItem(StyleCollectionKey key, GetImage imageGetter)
			: base(String.Empty, imageGetter) {
			Key = key;
		}
		public readonly StyleCollectionKey Key;
	}
	class StyleChooserServiceProvider : DevExpress.XtraGauges.Core.IStyleChooserService {
		bool DevExpress.XtraGauges.Core.IStyleChooserService.Show(IGauge gauge) {
			return ChooseStyleForm.Show(gauge);
		}
	}
}
