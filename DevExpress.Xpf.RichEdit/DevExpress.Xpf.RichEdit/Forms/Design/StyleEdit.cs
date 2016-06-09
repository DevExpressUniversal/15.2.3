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
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Ribbon;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Markup;
using DevExpress.Office;
using DevExpress.Office.Internal;
using DevExpress.Xpf.Office.UI;
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using FrameworkContentElement = System.Windows.FrameworkElement;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Drawing;
using PlatformIndependentColor = System.Windows.Media.Color;
using DevExpress.Utils.Internal;
using ObjectConverter = DevExpress.Xpf.Core.WPFCompatibility.ObjectConverter;
#else
using PlatformIndependentColor = System.Drawing.Color;
using ObjectConverter = DevExpress.Xpf.Core.ObjectConverter;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region RichEditStyleComboBoxEditSettings
	public class RichEditStyleComboBoxEditSettings : ComboBoxEditSettings, IRichEditControlDependencyPropertyOwner, IRichEditStyleControlOwner {
		#region RichEditControl
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(RichEditStyleComboBoxEditSettings), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditStyleComboBoxEditSettings instance = d as RichEditStyleComboBoxEditSettings;
			if (instance != null)
				instance.InnerControl.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		#endregion
		RichEditStyleControl innerControl;
		protected internal RichEditStyleControl InnerControl { get { return innerControl; } }
		static RichEditStyleComboBoxEditSettings() {
			RegisterEditor();
		}
		public RichEditStyleComboBoxEditSettings() {
			this.innerControl = new RichEditStyleControl(this);
			ValueMember = "RichEditStyle";
		}
		internal static void RegisterEditor() {
			EditorSettingsProvider.Default.RegisterUserEditor(typeof(RichEditStyleEdit), typeof(RichEditStyleComboBoxEditSettings), delegate() { return new RichEditStyleEdit(); }, delegate() { return new RichEditStyleComboBoxEditSettings(); });
		}
		#region IRichEditControlDependencyPropertyOwner Members
		DependencyProperty IRichEditControlDependencyPropertyOwner.DependencyProperty { get { return RichEditControlProperty; } }
		#endregion
		#region IRichEditStyleControlOwner Members
		void IRichEditStyleControlOwner.ClearItems() {
			Items.Clear();
		}
		void IRichEditStyleControlOwner.AddItem(StyleFormattingBase style, CharacterProperties properties) {
			RichEditStyleEditItem item = new RichEditStyleEditItem() {  RichEditStyle = style };
			item.Text = style.ToString();
			item.FontFamily = RichEditStyleControl.GetFontFamily(properties.FontName);
			item.FontSize = properties.DoubleFontSize / 2f;
			item.Foreground = new SolidColorBrush(XpfTypeConverter.ToPlatformColor(RichEditStyleControl.CorrectColor(properties.ForeColor)));
			item.FontWeight = properties.FontBold ? FontWeights.Bold : FontWeights.Normal;
			item.FontStyle = properties.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			if (properties.FontUnderlineType != UnderlineType.None)
				item.TextDecorations = System.Windows.TextDecorations.Underline;
			Items.Add(item);
		}
		#endregion
	}
	#endregion
	#region RichEditStyleEdit
	[DXToolboxBrowsableAttribute(false)]
	public class RichEditStyleEdit : ComboBoxEdit {
		static RichEditStyleEdit() {
			RichEditStyleComboBoxEditSettings.RegisterEditor();
		}
		public RichEditStyleEdit() {
			DefaultStyleKey = typeof(RichEditStyleEdit);
			ValueMember = "RichEditStyle";
			this.PopupClosed += OnPopupClosed;
		}
		void OnPopupClosed(object sender, ClosePopupEventArgs e) {
			if (RichEditControl != null)
				RichEditControl.SetFocus();
		}
		RichEditStyleComboBoxEditSettings InnerSettings { get { return Settings as RichEditStyleComboBoxEditSettings; } }
		public RichEditControl RichEditControl {
			get {
				if (InnerSettings != null)
					return InnerSettings.RichEditControl;
				else
					return null;
			}
			set {
				if (InnerSettings != null)
					InnerSettings.RichEditControl = value;
			}
		}
		protected override BaseEditSettings CreateEditorSettings() {
			return new RichEditStyleComboBoxEditSettings();
		}
	}
	#endregion
	#region RichEditStyleEditItem
	public class RichEditStyleEditItem :  IConvertible {
		StyleFormattingBase richEditStyle;
		public StyleFormattingBase RichEditStyle { get { return richEditStyle; } set { richEditStyle = value; } }
		public string Text { get; set; }
		public FontFamily FontFamily { get; set; }
		public System.Windows.FontStyle FontStyle { get; set; }
		public FontWeight FontWeight { get; set; }
		public double FontSize { get; set; }
		public TextDecorationCollection TextDecorations { get; set; }
		public System.Windows.Media.Brush Foreground { get; set; }
		#region IConvertible Members
		TypeCode IConvertible.GetTypeCode() {
			throw new NotImplementedException();
		}
		bool IConvertible.ToBoolean(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		byte IConvertible.ToByte(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		char IConvertible.ToChar(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		DateTime IConvertible.ToDateTime(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		decimal IConvertible.ToDecimal(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		double IConvertible.ToDouble(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		short IConvertible.ToInt16(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		int IConvertible.ToInt32(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		long IConvertible.ToInt64(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		sbyte IConvertible.ToSByte(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		float IConvertible.ToSingle(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		string IConvertible.ToString(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		object IConvertible.ToType(Type conversionType, IFormatProvider provider) {
			if (conversionType == typeof(IXtraRichEditFormatting))
				return RichEditStyle;
			throw new NotImplementedException();
		}
		ushort IConvertible.ToUInt16(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		uint IConvertible.ToUInt32(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		ulong IConvertible.ToUInt64(IFormatProvider provider) {
			throw new NotImplementedException();
		}
		#endregion
	}
	#endregion
	#region RichEditStyleGalleryItemGroup
	public class RichEditStyleGalleryItemGroup : GalleryItemGroup {
		ObservableCollection<StyleItem> styleItems;
		public RichEditStyleGalleryItemGroup() {
			DefaultStyleKey = typeof(RichEditStyleGalleryItemGroup);
			this.Loaded += new RoutedEventHandler(RichEditStyleGalleryItemGroup_Loaded);
		}
		public ObservableCollection<StyleItem> StyleItems { get { return styleItems; } set { styleItems = value; } }
		void RichEditStyleGalleryItemGroup_Loaded(object sender, RoutedEventArgs e) {
#if SILVERLIGHT            
			ApplyTemplate();
#endif
			this.ItemsSource = StyleItems;
		}
	}
	#endregion
	#region RichEditStyleGallery
	public class RichEditStyleGallery : Gallery, IRichEditControlDependencyPropertyOwner, IRichEditStyleControlOwner {
		#region RichEditControl
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(RichEditStyleGallery), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			RichEditStyleGallery instance = d as RichEditStyleGallery;
			if (instance != null)
				instance.InnerControl.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		#endregion
		#region EditValue
		public static readonly DependencyProperty EditValueProperty;
		[TypeConverter(typeof(ObjectConverter))]
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		EditValueChangedEventHandler onEditValueChanged;
		public event EditValueChangedEventHandler EditValueChanged { add { onEditValueChanged += value; } remove { onEditValueChanged -= value; } }
		protected internal virtual void RaiseEditValueChanged(object oldValue, object newValue) {
			if (onEditValueChanged != null)
				onEditValueChanged(this, new EditValueChangedEventArgs(oldValue, newValue));
		}
		static RichEditStyleGallery() {
			Type ownerType = typeof(RichEditStyleGallery);
#if !SL
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValueChanged), OnCoerceEditValue, true, UpdateSourceTrigger.LostFocus));
#else
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValueChanged), OnCoerceEditValue));
#endif
		}
		protected static void OnEditValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((RichEditStyleGallery)obj).OnEditValueChanged(e.OldValue, e.NewValue);
		}
		protected static object OnCoerceEditValue(DependencyObject obj, object value) {
			return ((RichEditStyleGallery)obj).CoerceEditValue(obj, value);
		}
		protected virtual object CoerceEditValue(DependencyObject d, object value) {
			return value;
		}
		protected virtual void OnEditValueChanged(object oldValue, object newValue) {
			UpdateCheckedItem((EditValue as StyleFormattingBase));
		}
		#endregion
		RichEditStyleControl innerControl;
		protected internal RichEditStyleControl InnerControl { get { return innerControl; } }
		ObservableCollection<StyleItem> styleItems = new ObservableCollection<StyleItem>();
		public RichEditStyleGallery() {
			this.innerControl = new RichEditStyleControl(this);
			RichEditStyleGalleryItemGroup g = new RichEditStyleGalleryItemGroup();
			g.StyleItems = styleItems;
			Groups.Add(g);
		}
		#region IRichEditControlDependencyPropertyOwner Members
		DependencyProperty IRichEditControlDependencyPropertyOwner.DependencyProperty { get { return RichEditControlProperty; } }
		#endregion
		protected internal virtual GalleryItemCollection Items { get { return Groups[0].Items; } }
		#region IRichEditStyleControlOwner Members
		void IRichEditStyleControlOwner.ClearItems() {
			styleItems.Clear();
		}
		void IRichEditStyleControlOwner.AddItem(StyleFormattingBase style, CharacterProperties properties) {
			styleItems.Add(new StyleItem(InnerControl.RichEditControl.DocumentModel, style, properties));
		}
		public void UpdateCheckedItem(StyleFormattingBase editValue) {
			if (!(Groups != null && Groups.Count > 0 && Groups[0].Items.Count > 0))
				return;
			RichEditStyleGalleryItemGroup group = Groups[0] as RichEditStyleGalleryItemGroup;
			if (group == null)
				return;
			int count = group.StyleItems.Count;
			for (int i = 0; i < count; i++) {
				StyleItem item = group.StyleItems[i];
				if (item.Style.StyleId == editValue.StyleId )
					group.Items[i].IsChecked = true;
			}
		}
		protected override void OnItemClick(GalleryItemEventArgs args) {
			base.OnItemClick(args);
			StyleItem item = (StyleItem)args.Item.Caption;
			if (item == null)
				return;
			EditValue = item.Style;
		}
		#endregion
		internal void UpdateStyleItems() {
			int count = this.styleItems.Count;
			for (int i = 0; i < count; i++)
				this.styleItems[i].UpdateProperties();
		}
	}
	#endregion
	#region StyleItem (item for data binding)
	public class StyleItem : INotifyPropertyChanged {
		const float deviceIndependentDPI = 96.0f;
		DocumentModelUnitConverter unitConverter;
		DocumentModel documentModel;
		string styleName;
		int fontSize;
		string fontName;
		FontWeight fontWeight;
		FontStyle fontStyle;
		FontFamily fontFamily;
		Color fontColor;
		TextDecorationCollection textDecorations;
		public StyleItem(DocumentModel documentModel, StyleFormattingBase style, CharacterProperties properties) {
			this.documentModel = documentModel;
			Style = style;
			Properties = properties;
			this.unitConverter = documentModel.UnitConverter;
			UpdateProperties();
		}
		public StyleFormattingBase Style { get; set; }
		public CharacterProperties Properties { get; set; }
		DocumentModelUnitConverter UnitConverter { get { return unitConverter; } }
		public string StyleName {
			get { return styleName; }
			set {
				if (StyleName == value)
					return;
				this.styleName = value;
				OnPropertyChanged("StyleName");
			}
		}
		public int FontSize {
			get { return fontSize; }
			set {
				if (FontSize == value)
					return;
				this.fontSize = value;
				OnPropertyChanged("FontSize");
			}
		}
		public bool IsParagraphStyle { get { return Style is ParagraphStyleFormatting; } }
		public string FontName {
			get { return fontName; }
			set {
				if (FontName == value)
					return;
				this.fontName = value;
				OnPropertyChanged("FontName");
			}
		}
		public FontWeight FontWeight {
			get { return fontWeight; }
			set {
				if (FontWeight == value)
					return;
				this.fontWeight = value;
				OnPropertyChanged("FontWeight");
			}
		}
		public FontStyle FontStyle {
			get { return fontStyle; }
			set {
				if (FontStyle == value)
					return;
				this.fontStyle = value;
				OnPropertyChanged("FontStyle");
			}
		}
		public FontFamily FontFamily {
			get { return fontFamily; }
			set {
				if (FontFamily == value)
					return;
				this.fontFamily = value;
				OnPropertyChanged("FontFamily");
			}
		}
		public Color FontColor {
			get { return fontColor; }
			set {
				if (FontColor == value)
					return;
				this.fontColor = value;
				OnPropertyChanged("FontColor");
			}
		}
		public TextDecorationCollection TextDecorations {
			get { return textDecorations; }
			set {
				if (TextDecorations == value)
					return;
				this.textDecorations = value;
				OnPropertyChanged("TextDecorations");
			}
		}
		internal void UpdateProperties() {
			StyleName = Style.GetLocalizedCaption(this.documentModel);
			int fontSizeInModelUnits = unitConverter.PointsToModelUnits(Properties.DoubleFontSize) / 2;
			FontSize = unitConverter.ModelUnitsToPixels(fontSizeInModelUnits, deviceIndependentDPI);
			FontName = Properties.FontName;
			FontWeight = Properties.FontBold ? FontWeights.Bold : FontWeights.Normal;
			FontStyle = Properties.FontItalic ? FontStyles.Italic : FontStyles.Normal;
			FontFamily = RichEditStyleControl.GetFontFamily(Properties.FontName);
			FontColor = XpfTypeConverter.ToPlatformColor((RichEditStyleControl.CorrectColor(Properties.ForeColor)));
			if (Properties.FontUnderlineType != UnderlineType.None)
				TextDecorations = System.Windows.TextDecorations.Underline;
			else
				TextDecorations = new TextDecorationCollection();
		}
		#region PropertyChanged event
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		void OnPropertyChanged(string propertyName) {
			PropertyChangedEventHandler handler = onPropertyChanged;
			if (handler != null)
				handler(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
	}
	#endregion
	#region GalleryStyleItem (Ribbon)
	public class GalleryStyleItem : RibbonGalleryBarItem, IRichEditControlDependencyPropertyOwner, IEditValueBarItem {
#if!SL
		[ThreadStatic]
#endif
		static DefaultBarItemDataTemplates defaultBarItemTemplates;
		public static readonly DependencyProperty EditValueProperty;
		#region static GalleryStyleItem cstr
		static GalleryStyleItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(GalleryStyleItem), typeof(RibbonStyleGalleryItemLink), delegate(object arg) { return new RibbonStyleGalleryItemLink(); });
			Type ownerType = typeof(GalleryStyleItem);
#if !SL
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.Journal | FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValueChanged), OnCoerceEditValue, true, UpdateSourceTrigger.LostFocus));
#else
			EditValueProperty = DependencyPropertyManager.Register("EditValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, new PropertyChangedCallback(OnEditValueChanged), OnCoerceEditValue));
#endif
		}
		DefaultBarItemDataTemplates DefaultBarItemTemplates {
			get {
				if (defaultBarItemTemplates == null) {
					defaultBarItemTemplates = new DefaultBarItemDataTemplates();
#if SILVERLIGHT
					AddLogicalChild(defaultBarItemTemplates);
					RemoveLogicalChild(defaultBarItemTemplates);
#endif
					defaultBarItemTemplates.ApplyTemplate();
				}
				return defaultBarItemTemplates;
			}
		}
		#endregion
		protected override void OnDropDownGalleryInit(GalleryDropDownPopupMenu dropDownGalleryControl) {
			base.OnDropDownGalleryInit(dropDownGalleryControl);
			dropDownGalleryControl.Gallery.IsGroupCaptionVisible = DefaultBoolean.False;
			dropDownGalleryControl.Gallery.AllowFilter = false;
		}
		public GalleryStyleItem() {
		}
		#region RichEditControl
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(GalleryStyleItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GalleryStyleItem instance = d as GalleryStyleItem;
			if (instance != null) {
				RichEditStyleGallery gallery = instance.Gallery as RichEditStyleGallery;
				if (gallery != null)
					gallery.RichEditControl = (RichEditControl)e.NewValue;
			}
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		#endregion
		#region EditValue
		[TypeConverter(typeof(ObjectConverter))]
		public object EditValue {
			get { return GetValue(EditValueProperty); }
			set { SetValue(EditValueProperty, value); }
		}
		EditValueChangedEventHandler onEditValueChanged;
		public event EditValueChangedEventHandler EditValueChanged { add { onEditValueChanged += value; } remove { onEditValueChanged -= value; } }
		protected internal virtual void RaiseEditValueChanged(object oldValue, object newValue) {
			if (onEditValueChanged != null)
				onEditValueChanged(this, new EditValueChangedEventArgs(oldValue, newValue));
		}
		protected static void OnEditValueChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryStyleItem)obj).OnEditValueChanged(e.OldValue, e.NewValue);
		}
		protected static object OnCoerceEditValue(DependencyObject obj, object value) {
			return ((GalleryStyleItem)obj).CoerceEditValue(obj, value);
		}
		protected virtual object CoerceEditValue(DependencyObject d, object value) {
			return value;
		}
		protected virtual void OnEditValueChanged(object oldValue, object newValue) {
			RaiseEditValueChanged(oldValue, newValue);
			if (Gallery != null && Gallery is RichEditStyleGallery)
				(Gallery as RichEditStyleGallery).UpdateCheckedItem((EditValue as StyleFormattingBase));
		}
		#endregion
		#region IRichEditControlDependencyPropertyOwner Members
		DependencyProperty IRichEditControlDependencyPropertyOwner.DependencyProperty { get { return RichEditControlProperty; } }
		#endregion
		public override BarItemLink CreateLink(bool isPrivate) {
			return new RibbonStyleGalleryItemLink();
		}
		protected override void OnGalleryChanged(Gallery oldValue) {
			base.OnGalleryChanged(oldValue);
			if (oldValue != null)
				oldValue.ItemClick -= new GalleryItemEventHandler(Gallery_ItemClick);
			if (Gallery != null)
				Gallery.ItemClick += new GalleryItemEventHandler(Gallery_ItemClick);
		}
		void Gallery_ItemClick(object sender, GalleryItemEventArgs e) {
			StyleItem item = (StyleItem)e.Item.Caption;
			if (item == null)
				return;
			EditValue = item.Style;
		}
	}
	#endregion
	#region RibbonStyleGalleryItemLink
	public class RibbonStyleGalleryItemLink : RibbonGalleryBarItemLink {
	   protected override BarItemLinkControlBase CreateBarItemLinkControl() {
			return new RibbonStyleGalleryBarItemLinkControl();
		}
	}
	#endregion
	#region RibbonStyleGalleryBarItemLinkControl
	public class RibbonStyleGalleryBarItemLinkControl : RibbonGalleryBarItemLinkControl {
		public RibbonStyleGalleryBarItemLinkControl() {
		}
		public RibbonStyleGalleryBarItemLinkControl(RibbonGalleryBarItemLink link)
			: base(link) {
		}
	}
	#endregion
	#region GalleryBarSplitButtonEditItem (Bar)
	public class GalleryBarSplitButtonEditItem : BarSplitButtonEditItem, IEditValueBarItem {
		#region RichEditControl
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(GalleryBarSplitButtonEditItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			GalleryBarSplitButtonEditItem instance = d as GalleryBarSplitButtonEditItem;
			if (instance == null)
				return;
			SetRichEditControlToGallery(e, instance);
		}
		static void SetRichEditControlToGallery(DependencyPropertyChangedEventArgs e, GalleryBarSplitButtonEditItem instance) {
			RichEditStyleGallery gallery = instance.GetGallery();
			if (gallery != null) {
				gallery.RichEditControl = (RichEditControl)e.NewValue;
			}
			if (instance.PopupControl == null
					|| instance.PopupControl.Popup == null
					|| instance.PopupControl.Popup.PopupContent == null)
				return;
			GalleryDropDownPopupMenu popupMenu = instance.PopupControl.Popup as GalleryDropDownPopupMenu;
			if (popupMenu == null)
				return;
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		#endregion
		public RichEditStyleGallery GetGallery() {
			if (PopupControl == null
				|| PopupControl.Popup == null
				|| PopupControl.Popup.PopupContent == null)
				return null;
			GalleryDropDownControl galleryDropDownControl = PopupControl.Popup.PopupContent as GalleryDropDownControl;
			if (galleryDropDownControl == null)
				return null;
			RichEditStyleGallery result = galleryDropDownControl.Gallery as RichEditStyleGallery;
			return result;
		}
		protected override void OnEditValueChanged(object oldValue, object newValue) {
			base.OnEditValueChanged(oldValue, newValue);
		}
		protected override void OnItemClick(BarItemLink link) {
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Internal {
	#region IRichEditStyleControlOwner
	public interface IRichEditStyleControlOwner {
		RichEditControl RichEditControl { get; set; }
		void ClearItems();
		void AddItem(StyleFormattingBase style, CharacterProperties properties);
	}
	#endregion
	#region RichEditStyleControl
	public class RichEditStyleControl {
		readonly IRichEditStyleControlOwner owner;
		ParagraphStyleCollection paragraphStyles;
		CharacterStyleCollection characterStyles;
		public RichEditStyleControl(IRichEditStyleControlOwner owner) {
			Guard.ArgumentNotNull(owner, "owner");
			this.owner = owner;
		}
		public IRichEditStyleControlOwner Owner { get { return owner; } }
		public RichEditControl RichEditControl { get { return Owner.RichEditControl; } }
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			if (oldValue != null)
				UnsubscribeEvents(oldValue);
			OnControlChanged();
			if (newValue != null)
				SubscribeEvents(newValue);
		}
		protected internal virtual void SubscribeEvents(RichEditControl control) {
			if (control == null)
				return;
			control.DocumentLoaded += StyleCollectionChanged;
			control.EmptyDocumentCreated += StyleCollectionChanged;
			control.DocumentModel.DocumentCleared += OnDocumentCleared;
			SubscribeCollectionsChangedEvents();
			control.Loaded += OnRichEditLoaded;
			control.Unloaded += OnRichEditUnloaded;
		}
		protected internal virtual void SubscribeCollectionsChangedEvents() {
			characterStyles.CollectionChanged += StyleCollectionChanged;
			paragraphStyles.CollectionChanged += StyleCollectionChanged;
		}
		protected internal virtual void UnsubscribeEvents(RichEditControl control) {
			if (control == null)
				return;
			control.DocumentLoaded -= StyleCollectionChanged;
			control.EmptyDocumentCreated -= StyleCollectionChanged;
			control.DocumentModel.DocumentCleared -= OnDocumentCleared;
			UnsubscribeCollectionsChangedEvents();
			control.Loaded -= OnRichEditLoaded;
			control.Unloaded -= OnRichEditUnloaded;
		}
		protected internal virtual void UnsubscribeCollectionsChangedEvents() {
			characterStyles.CollectionChanged -= StyleCollectionChanged;
			paragraphStyles.CollectionChanged -= StyleCollectionChanged;
		}
		void OnRichEditLoaded(object sender, RoutedEventArgs e) {
			PopulateItems();
		}
		void OnRichEditUnloaded(object sender, RoutedEventArgs e) {
			Owner.ClearItems();
		}
		void OnDocumentCleared(object sender, EventArgs e) {
			UnsubscribeCollectionsChangedEvents();
			SetStylesCollections();
			SubscribeCollectionsChangedEvents();
			StyleCollectionChanged(sender, e);
		}
		void SetStylesCollections() {
			if (RichEditControl != null) {
				characterStyles = RichEditControl.DocumentModel.CharacterStyles;
				paragraphStyles = RichEditControl.DocumentModel.ParagraphStyles;
			}
		}
		void StyleCollectionChanged(object sender, EventArgs e) {
			PopulateItems();
		}
		protected virtual void OnControlChanged() {
			SetStylesCollections();
			PopulateItems();
		}
		void PopulateItems() {
			Owner.ClearItems();
			if (RichEditControl != null) {
				AddParagraphStyles();
				AddCharacterStyles();
			}
		}
		protected internal virtual bool IsStyleVisible(IStyle style) {
			return !(style.Deleted || style.Hidden || style.Semihidden);
		}
		public static FontFamily GetFontFamily(string name) {
#if!SL
			if (FontManager.GetFontNames().Contains(name))
				return new FontFamily(name);
#else
			if (FontManager.GetFontFamilyNames().Contains(name))
				return new FontFamily(name);
#endif
#if SL
			return new FontFamily(FontManager.DefaultFontFamilyName);
#else
			return new FontFamily("Arial");
#endif
		}
		public static PlatformIndependentColor CorrectColor(PlatformIndependentColor color) {
			if (color.A == 0 && color.B == 0 && color.G == 0 && color.R == 0)
				return DXColor.Black;
			int a = color.A;
			int r = color.R;
			int g = color.G;
			int b = color.B;
			if (a < 30) a = 30;
			if (b > 220 && r > 220 && g > 220) {
				b = 220;
				r = 220;
				g = 220;
			}
			return DXColor.FromArgb(a, r, g, b);
		}
		protected internal virtual void AddParagraphStyles() {
			ParagraphStyleCollection paragraphStyles = RichEditControl.DocumentModel.ParagraphStyles;
			int count = paragraphStyles.Count;
			for (int i = 0; i < count; i++) {
				ParagraphStyle paragraphStyle = paragraphStyles[i];
				if (IsStyleVisible(paragraphStyle)) {
					ParagraphStyleFormatting psf = new ParagraphStyleFormatting(paragraphStyle.Id);
					ParagraphStyle ps = paragraphStyle;
					AddStyle(psf, ps.CharacterProperties);
				}
			}
		}
		protected internal virtual void AddCharacterStyles() {
			CharacterStyleCollection characterStyles = RichEditControl.DocumentModel.CharacterStyles;
			int count = characterStyles.Count;
			for (int i = 0; i < count; i++) {
				CharacterStyle characterStyle = characterStyles[i];
				if (IsStyleVisible(characterStyle) && !characterStyle.HasLinkedStyle) {
					CharacterStyleFormatting csf = new CharacterStyleFormatting(characterStyle.Id);
					CharacterStyle cs = characterStyle;
					AddStyle(csf, cs.CharacterProperties);
				}
			}
		}
		protected virtual void AddStyle(StyleFormattingBase style, CharacterProperties properties) {
			Owner.AddItem(style, properties);
		}
	}
	#endregion
}
