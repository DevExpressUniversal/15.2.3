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
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraEditors;
using DevExpress.Utils.Design;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IBaseTileProperties : IBaseProperties, ITileItemProperties {
		bool AllowCheck { get; set; }
	}
	public interface IBaseTileDefaultProperties : IBaseDefaultProperties {
		DefaultBoolean AllowCheck { get; set; }
		DefaultBoolean AllowHtmlDraw { get; set; }
		DefaultBoolean AllowGlyphSkinning { get; set; }
		DefaultBoolean IsLarge { get; set; }
		TileItemSize? ItemSize { get; set; }
		int? RowCount { get; set; }
		TileItemImageScaleMode? BackgroundImageScaleMode { get; set; }
		TileItemContentAlignment? BackgroundImageAlignment { get; set; }
		TileItemContentShowMode? TextShowMode { get; set; }
		int? CurrentFrameIndex { get; set; }
		int? FrameAnimationInterval { get; set; }
		TileItemContentAnimationType? ContentAnimation { get; set; }
		TileItemBorderVisibility? BorderVisibility { get; set; }
		bool CanCheck { get; }
		bool CanHtmlDraw { get; }
		bool CanUseGlyphSkinning { get; }
		bool ActualIsLarge { get; }
		TileItemSize ActualItemSize { get; }
		bool ActualIsWide { get; }
		bool ActualIsMedium { get; }
		int ActualRowCount { get; }
		TileItemImageScaleMode ActualBackgroundImageScaleMode { get; }
		TileItemContentAlignment ActualBackgroundImageAlignment { get; }
		TileItemContentShowMode ActualTextShowMode { get; }
		int ActualCurrentFrameIndex { get; }
		int ActualFrameAnimationInterval { get; }
		TileItemContentAnimationType ActualContentAnimation { get; }
		TileItemBorderVisibility ActualBorderVisibility { get; }
		ITileItemProperties ActualProperties { get; }
	}
	public class BaseTileProperties : DevExpress.XtraBars.Docking2010.Base.BaseProperties, IBaseTileProperties {
		public BaseTileProperties() {
			SetDefaultValueCore("IsLarge", true);
			SetDefaultValueCore("AllowCheck", true);
			SetDefaultValueCore("RowCount", 1);
			SetDefaultValueCore("FrameAnimationInterval", 3000);
			SetDefaultValueCore("AllowGlyphSkinning", false);
		}
		protected override IBaseProperties CloneCore() {
			return new BaseTileProperties();
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowCheck {
			get { return GetValueCore<bool>("AllowCheck"); }
			set { SetValueCore("AllowCheck", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowHtmlDraw {
			get { return GetValueCore<bool>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		[Category("Layout")]
		[DefaultValue(TileItemSize.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemSize ItemSize {
			get { return GetValueCore<TileItemSize>("ItemSize"); }
			set { SetValueCore("ItemSize", value); }
		}
		[Category("Layout")]
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[Obsolete, Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsLarge {
			get { return ItemSize == TileItemSize.Wide || ItemSize == TileItemSize.Default; }
			set { ItemSize = value ? TileItemSize.Default : TileItemSize.Medium; }
		}
		[Category("Layout")]
		[DefaultValue(1), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int RowCount {
			get { return GetValueCore<int>("RowCount"); }
			set { SetValueCore("RowCount", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemImageScaleMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemImageScaleMode BackgroundImageScaleMode {
			get { return GetValueCore<TileItemImageScaleMode>("BackgroundImageScaleMode"); }
			set { SetValueCore("BackgroundImageScaleMode", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemContentAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentAlignment BackgroundImageAlignment {
			get { return GetValueCore<TileItemContentAlignment>("BackgroundImageAlignment"); }
			set { SetValueCore("BackgroundImageAlignment", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemContentShowMode.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentShowMode TextShowMode {
			get { return GetValueCore<TileItemContentShowMode>("TextShowMode"); }
			set { SetValueCore("TextShowMode", value); }
		}
		[Category(CategoryName.Properties)]
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int CurrentFrameIndex {
			get { return GetValueCore<int>("CurrentFrameIndex"); }
			set { SetValueCore("CurrentFrameIndex", value); }
		}
		[Category(CategoryName.Properties)]
		[DefaultValue(3000), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public int FrameAnimationInterval {
			get { return GetValueCore<int>("FrameAnimationInterval"); }
			set { SetValueCore("FrameAnimationInterval", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemContentAnimationType.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemContentAnimationType ContentAnimation {
			get { return GetValueCore<TileItemContentAnimationType>("ContentAnimation"); }
			set { SetValueCore("ContentAnimation", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(false), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public bool AllowGlyphSkinning {
			get { return GetValueCore<bool>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(TileItemBorderVisibility.Auto), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		public TileItemBorderVisibility BorderVisibility {
			get { return GetValueCore<TileItemBorderVisibility>("BorderVisibility"); }
			set { SetValueCore("BorderVisibility", value); }
		}
		void ITileItemProperties.Assign(ITileItemProperties source) {
			BeginUpdate();
			AllowHtmlDraw = source.AllowHtmlDraw;
			BackgroundImageAlignment = source.BackgroundImageAlignment;
			BackgroundImageScaleMode = source.BackgroundImageScaleMode;
			ContentAnimation = source.ContentAnimation;
			CurrentFrameIndex = source.CurrentFrameIndex;
			FrameAnimationInterval = source.FrameAnimationInterval;
			ItemSize = source.ItemSize;
			RowCount = source.RowCount;
			TextShowMode = source.TextShowMode;
			AllowGlyphSkinning = source.AllowGlyphSkinning;
			BorderVisibility = source.BorderVisibility;
			CancelUpdate();
		}
	}
	public class BaseTileDefaultProperties : DevExpress.XtraBars.Docking2010.Base.BaseDefaultProperties, IBaseTileDefaultProperties {
		public BaseTileDefaultProperties(IBaseTileProperties parentProperties)
			: base(parentProperties) {
			SetDefaultValueCore("AllowCheck", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowHtmlDraw", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("AllowGlyphSkinning", DevExpress.Utils.DefaultBoolean.Default);
			SetDefaultValueCore("IsLarge", DevExpress.Utils.DefaultBoolean.Default);
			SetConverter("AllowCheck", GetDefaultBooleanConverter(true));
			SetConverter("AllowHtmlDraw", GetDefaultBooleanConverter(false));
			SetConverter("AllowGlyphSkinning", GetDefaultBooleanConverter(false));
			SetConverter("IsLarge", GetDefaultBooleanConverter(true));
			SetConverter("RowCount", GetNullableValueConverter(1));
			SetConverter("FrameAnimationInterval", GetNullableValueConverter(3000));
		}
		protected override IBaseProperties CloneCore() {
			return new BaseTileDefaultProperties(ParentProperties as IBaseTileProperties);
		}
		[Category(CategoryName.Behavior)]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("AllowCheck", "")]
		public DefaultBoolean AllowCheck {
			get { return GetValueCore<DefaultBoolean>("AllowCheck"); }
			set { SetValueCore("AllowCheck", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("Allow Html Draw", "")]
		public DefaultBoolean AllowHtmlDraw {
			get { return GetValueCore<DefaultBoolean>("AllowHtmlDraw"); }
			set { SetValueCore("AllowHtmlDraw", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("Allow Glyph Skinning", "")]
		public DefaultBoolean AllowGlyphSkinning {
			get { return GetValueCore<DefaultBoolean>("AllowGlyphSkinning"); }
			set { SetValueCore("AllowGlyphSkinning", value); }
		}
		[Category("Layout")]
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("Item Size", "")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemSize? ItemSize {
			get { return GetValueCore<TileItemSize?>("ItemSize"); }
			set { SetValueCore("ItemSize", value); }
		}
		[Category("Layout")]
		[DefaultValue(DefaultBoolean.Default)]
		[Obsolete, Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public DefaultBoolean IsLarge {
			get { return ActualIsMedium ? DefaultBoolean.False : DefaultBoolean.Default; }
			set { ItemSize = (value == DefaultBoolean.False) ? TileItemSize.Medium : (TileItemSize?)null; }
		}
		[Category("Layout")]
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? RowCount {
			get { return GetValueCore<int?>("RowCount"); }
			set { SetValueCore("RowCount", value); }
		}
		[Category(CategoryName.Properties)]
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? CurrentFrameIndex {
			get { return GetValueCore<int?>("CurrentFrameIndex"); }
			set { SetValueCore("CurrentFrameIndex", value); }
		}
		[Category(CategoryName.Properties)]
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? FrameAnimationInterval {
			get { return GetValueCore<int?>("FrameAnimationInterval"); }
			set { SetValueCore("FrameAnimationInterval", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemImageScaleMode? BackgroundImageScaleMode {
			get { return GetValueCore<TileItemImageScaleMode?>("BackgroundImageScaleMode"); }
			set { SetValueCore("BackgroundImageScaleMode", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue),]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentAlignment? BackgroundImageAlignment {
			get { return GetValueCore<TileItemContentAlignment?>("BackgroundImageAlignment"); }
			set { SetValueCore("BackgroundImageAlignment", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), SmartTagProperty("Text Show Mode", "")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentShowMode? TextShowMode {
			get { return GetValueCore<TileItemContentShowMode?>("TextShowMode"); }
			set { SetValueCore("TextShowMode", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemContentAnimationType? ContentAnimation {
			get { return GetValueCore<TileItemContentAnimationType?>("ContentAnimation"); }
			set { SetValueCore("ContentAnimation", value); }
		}
		[Category(CategoryName.Appearance)]
		[DefaultValue(null), RefreshProperties(RefreshProperties.Repaint)]
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue)]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileItemBorderVisibility? BorderVisibility {
			get { return GetValueCore<TileItemBorderVisibility?>("BorderVisibility"); }
			set { SetValueCore("TileItemBorderVisibility", value); }
		}
		[Browsable(false)]
		public bool CanCheck {
			get { return GetActualValue<DefaultBoolean, bool>("AllowCheck"); }
		}
		[Browsable(false)]
		public bool CanUseGlyphSkinning {
			get { return GetActualValue<DefaultBoolean, bool>("AllowGlyphSkinning"); }
		}
		[Browsable(false)]
		public bool CanHtmlDraw {
			get { return GetActualValue<DefaultBoolean, bool>("AllowHtmlDraw"); }
		}
		[Browsable(false)]
		public TileItemSize ActualItemSize {
			get { return GetActualValueFromNullable<TileItemSize>("ItemSize"); }
		}
		[Browsable(false)]
		public bool ActualIsWide {
			get { return ActualItemSize == TileItemSize.Wide || ActualItemSize == TileItemSize.Default; }
		}
		[Browsable(false)]
		public bool ActualIsMedium {
			get { return ActualItemSize == TileItemSize.Medium; }
		}
		[Browsable(false)]
		public bool ActualIsLarge {
			get { return ActualItemSize == TileItemSize.Large; }
		}
		[Browsable(false)]
		public int ActualRowCount {
			get { return GetActualValueFromNullable<int>("RowCount"); }
		}
		[Browsable(false)]
		public int ActualCurrentFrameIndex {
			get { return GetActualValueFromNullable<int>("CurrentFrameIndex"); }
		}
		[Browsable(false)]
		public int ActualFrameAnimationInterval {
			get { return GetActualValueFromNullable<int>("FrameAnimationInterval"); }
		}
		[Browsable(false)]
		public TileItemImageScaleMode ActualBackgroundImageScaleMode {
			get { return GetActualValueFromNullable<TileItemImageScaleMode>("BackgroundImageScaleMode"); }
		}
		[Browsable(false)]
		public TileItemContentAlignment ActualBackgroundImageAlignment {
			get { return GetActualValueFromNullable<TileItemContentAlignment>("BackgroundImageAlignment"); }
		}
		[Browsable(false)]
		public TileItemContentShowMode ActualTextShowMode {
			get { return GetActualValueFromNullable<TileItemContentShowMode>("TextShowMode"); }
		}
		[Browsable(false)]
		public TileItemContentAnimationType ActualContentAnimation {
			get { return GetActualValueFromNullable<TileItemContentAnimationType>("ContentAnimation"); }
		}
		[Browsable(false)]
		public TileItemBorderVisibility ActualBorderVisibility {
			get { return GetActualValueFromNullable<TileItemBorderVisibility>("BorderVisibility"); }
		}
		[Browsable(false)]
		public ITileItemProperties ActualProperties {
			get { return new ActualTileItemProperties(this); }
		}
		internal bool LockUpdate { get; set; }
		protected override void PropertiesChanged(string property) {
			base.PropertiesChanged(property);
			LockUpdate = false;
		}
		class ActualTileItemProperties : ITileItemProperties {
			IBaseTileDefaultProperties ownerProperties;
			internal ActualTileItemProperties(IBaseTileDefaultProperties properties) {
				ownerProperties = properties;
			}
			public bool AllowGlyphSkinning {
				get { return ownerProperties.CanUseGlyphSkinning; }
				set { ownerProperties.AllowGlyphSkinning = !value ? DefaultBoolean.Default : DefaultBoolean.True; }
			}
			public TileItemSize ItemSize {
				get { return ownerProperties.ActualItemSize; }
				set { ownerProperties.ItemSize = GetNullableValue(value, TileItemSize.Default); }
			}
			[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
			public bool IsLarge {
				get { return ownerProperties.ActualIsLarge; }
				set { ownerProperties.IsLarge = value ? DefaultBoolean.Default : DefaultBoolean.False; }
			}
			public bool AllowHtmlDraw {
				get { return ownerProperties.CanHtmlDraw; }
				set { ownerProperties.AllowHtmlDraw = !value ? DefaultBoolean.Default : DefaultBoolean.True; }
			}
			public int RowCount {
				get { return ownerProperties.ActualRowCount; }
				set { ownerProperties.RowCount = GetNullableValue(value, 1); }
			}
			public TileItemImageScaleMode BackgroundImageScaleMode {
				get { return ownerProperties.ActualBackgroundImageScaleMode; }
				set { ownerProperties.BackgroundImageScaleMode = GetNullableValue(value, TileItemImageScaleMode.Default); }
			}
			public TileItemContentAlignment BackgroundImageAlignment {
				get { return ownerProperties.ActualBackgroundImageAlignment; }
				set { ownerProperties.BackgroundImageAlignment = GetNullableValue(value, TileItemContentAlignment.Default); }
			}
			public TileItemContentShowMode TextShowMode {
				get { return ownerProperties.ActualTextShowMode; }
				set { ownerProperties.TextShowMode = GetNullableValue(value, TileItemContentShowMode.Default); }
			}
			public int CurrentFrameIndex {
				get { return ownerProperties.ActualCurrentFrameIndex; }
				set { ownerProperties.CurrentFrameIndex = GetNullableValue(value, 0); }
			}
			public int FrameAnimationInterval {
				get { return ownerProperties.ActualFrameAnimationInterval; }
				set { ownerProperties.FrameAnimationInterval = GetNullableValue(value, 3000); }
			}
			public TileItemContentAnimationType ContentAnimation {
				get { return ownerProperties.ActualContentAnimation; }
				set { ownerProperties.ContentAnimation = GetNullableValue(value, TileItemContentAnimationType.Default); }
			}
			public TileItemBorderVisibility BorderVisibility {
				get { return ownerProperties.ActualBorderVisibility; }
				set { ownerProperties.BorderVisibility = GetNullableValue(value, TileItemBorderVisibility.Auto); }
			}
			public void Assign(ITileItemProperties source) { }
			static Nullable<T> GetNullableValue<T>(T value, T defaultValue) where T : struct {
				return object.Equals(value, defaultValue) ? (T?)null : new Nullable<T>(value);
			}
		}
	}
}
