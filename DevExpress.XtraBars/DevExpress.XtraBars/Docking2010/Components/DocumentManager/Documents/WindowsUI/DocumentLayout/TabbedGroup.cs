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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public enum HeaderStyle {
		Default,
		Tab,
		Tile
	}
	public enum TileHeaderContentAlignment {
		Default = 0,
		TopLeft = 1,
		TopCenter = 2,
		TopRight = 4,
		MiddleLeft = 16,
		MiddleCenter = 32,
		MiddleRight = 64,
		BottomLeft = 256,
		BottomCenter = 512,
		BottomRight = 1024,
	}
	public interface ITabbedGroupProperties : IDocumentSelectorProperties {
		HeaderStyle HeaderStyle { get; set; }
		int TabWidth { get; set; }
		int TileSize { get; set; }
		int TileColumnCount { get; set; }
		TileHeaderContentAlignment TileTextAlignment { get; set; }
		TileHeaderContentAlignment TileImageAlignment { get; set; }
		Padding TileContentMargin { get; set; }
	}
	public interface ITabbedGroupDefaultProperties : IDocumentSelectorDefaultProperties {
		HeaderStyle? HeaderStyle { get; set; }
		int? TabWidth { get; set; }
		int? TileSize { get; set; }
		int? TileColumnCount { get; set; }
		TileHeaderContentAlignment? TileTextAlignment { get; set; }
		TileHeaderContentAlignment? TileImageAlignment { get; set; }
		Padding? TileContentMargin { get; set; }
		HeaderStyle ActualHeaderStyle { get; }
		bool HasTabWidth { get; }
		int ActualTabWidth { get; }
		bool HasTileSize { get; }
		int ActualTileSize { get; }
		int ActualTileColumnCount { get; }
		TileHeaderContentAlignment ActualTileTextAlignment { get; }
		TileHeaderContentAlignment ActualTileImageAlignment { get; }
		bool HasTileContentMargin { get; }
		Padding ActualTileContentMargin { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class TabbedGroup : DocumentSelector {
		public TabbedGroup()
			: base((IContainer)null) {
		}
		public TabbedGroup(IContainer container)
			: base(container) {
		}
		public TabbedGroup(ITabbedGroupProperties defaultProperties)
			: base(defaultProperties) {
		}
		#region Info
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ITabbedGroupInfo Info {
			get { return base.Info as ITabbedGroupInfo; }
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new TabbedGroupInfo(view, this);
		}
		#endregion Info
		#region Properties
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("TabbedGroupProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ITabbedGroupDefaultProperties Properties {
			get { return base.Properties as ITabbedGroupDefaultProperties; }
		}
		protected override DevExpress.Utils.Base.IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.TabbedGroupProperties;
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new TabbedGroupDefaultProperties(parentProperties as ITabbedGroupProperties);
		}
		protected override void OnPropertiesPropertyChanged(string propertyName) {
			if(propertyName == "HeaderStyle" && Info != null)
				Info.UpdateStyle();
		}
		#endregion Properties
		#region Items
		protected override DocumentCollection CreateItems() {
			return new TabbedGroupDocumentCollection(this);
		}
		class TabbedGroupDocumentCollection : DocumentSelectorDocumentCollection {
			public TabbedGroupDocumentCollection(TabbedGroup owner)
				: base(owner) {
			}
		}
		#endregion Items
	}
	public class TabbedGroupProperties : DocumentSelectorProperties, ITabbedGroupProperties {
		public TabbedGroupProperties() {
			SetDefaultValueCore("Orientation", Orientation.Vertical);
			SetDefaultValueCore("TileColumnCount", 2);
		}
		[DefaultValue(Orientation.Vertical), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public override Orientation Orientation {
			get { return GetValueCore<Orientation>("Orientation"); }
			set { SetValueCore("Orientation", value); }
		}
		[DefaultValue(HeaderStyle.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		public HeaderStyle HeaderStyle {
			get { return GetValueCore<HeaderStyle>("HeaderStyle"); }
			set { SetValueCore("HeaderStyle", value); }
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int TabWidth {
			get { return GetValueCore<int>("TabWidth"); }
			set { SetValueCore("TabWidth", value); }
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int TileSize {
			get { return GetValueCore<int>("TileSize"); }
			set { SetValueCore("TileSize", value); }
		}
		[DefaultValue(2), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int TileColumnCount {
			get { return GetValueCore<int>("TileColumnCount"); }
			set { SetValueCore<int>("TileColumnCount", value); }
		}
		[DefaultValue(TileHeaderContentAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		public TileHeaderContentAlignment TileTextAlignment {
			get { return GetValueCore<TileHeaderContentAlignment>("TileTextAlignment"); }
			set { SetValueCore("TileTextAlignment", value); }
		}
		[DefaultValue(TileHeaderContentAlignment.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		public TileHeaderContentAlignment TileImageAlignment {
			get { return GetValueCore<TileHeaderContentAlignment>("TileImageAlignment"); }
			set { SetValueCore("TileImageAlignment", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public Padding TileContentMargin {
			get { return GetValueCore<Padding>("TileContentMargin"); }
			set { SetValueCore("TileContentMargin", value); }
		}
		bool ShouldSerializeTileContentMargin() { return !IsDefault("TileContentMargin"); }
		void ResetTileContentMargin() { Reset("TileContentMargin"); }
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new TabbedGroupProperties();
		}
	}
	public class TabbedGroupDefaultProperties : DocumentSelectorDefaultProperties, ITabbedGroupDefaultProperties {
		public TabbedGroupDefaultProperties(ITabbedGroupProperties parentProperties)
			: base(parentProperties) {
			SetConverter("Orientation", GetNullableValueConverter(System.Windows.Forms.Orientation.Vertical));
			SetConverter("HeaderStyle", GetNullableValueConverter(Views.WindowsUI.HeaderStyle.Default));
			SetConverter("TileTextAlignment", GetNullableValueConverter(Views.WindowsUI.TileHeaderContentAlignment.Default));
			SetConverter("TileImageAlignment", GetNullableValueConverter(Views.WindowsUI.TileHeaderContentAlignment.Default));
			SetConverter("TileColumnCount", GetNullableValueConverter(2));
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? TileColumnCount {
			get { return GetValueCore<int?>("TileColumnCount"); }
			set { SetValueCore("TileColumnCount", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public HeaderStyle? HeaderStyle {
			get { return GetValueCore<HeaderStyle?>("HeaderStyle"); }
			set { SetValueCore("HeaderStyle", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? TabWidth {
			get { return GetValueCore<int?>("TabWidth"); }
			set { SetValueCore("TabWidth", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? TileSize {
			get { return GetValueCore<int?>("TileSize"); }
			set { SetValueCore("TileSize", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileHeaderContentAlignment? TileTextAlignment {
			get { return GetValueCore<TileHeaderContentAlignment?>("TileTextAlignment"); }
			set { SetValueCore("TileTextAlignment", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Appearance")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public TileHeaderContentAlignment? TileImageAlignment {
			get { return GetValueCore<TileHeaderContentAlignment?>("TileImageAlignment"); }
			set { SetValueCore("TileImageAlignment", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public Padding? TileContentMargin {
			get { return GetValueCore<Padding?>("TileContentMargin"); }
			set { SetValueCore("TileContentMargin", value); }
		}
		[Browsable(false)]
		public HeaderStyle ActualHeaderStyle {
			get { return GetActualValueFromNullable<HeaderStyle>("HeaderStyle"); }
		}
		[Browsable(false)]
		public bool HasTabWidth {
			get { return HasValue("TabWidth"); }
		}
		[Browsable(false)]
		public int ActualTabWidth {
			get { return GetActualValueFromNullable<int>("TabWidth"); }
		}
		[Browsable(false)]
		public bool HasTileSize {
			get { return HasValue("TileSize"); }
		}
		[Browsable(false)]
		public int ActualTileSize {
			get { return GetActualValueFromNullable<int>("TileSize"); }
		}
		[Browsable(false)]
		public int ActualTileColumnCount {
			get { return GetActualValueFromNullable<int>("TileColumnCount"); }
		}
		[Browsable(false)]
		public TileHeaderContentAlignment ActualTileTextAlignment {
			get { return GetActualValueFromNullable<TileHeaderContentAlignment>("TileTextAlignment"); }
		}
		[Browsable(false)]
		public TileHeaderContentAlignment ActualTileImageAlignment {
			get { return GetActualValueFromNullable<TileHeaderContentAlignment>("TileImageAlignment"); }
		}
		[Browsable(false)]
		public bool HasTileContentMargin {
			get { return HasValue("TileContentMargin"); }
		}
		[Browsable(false)]
		public Padding ActualTileContentMargin {
			get { return GetActualValueFromNullable<Padding>("TileContentMargin"); }
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new TabbedGroupDefaultProperties(ParentProperties as ITabbedGroupProperties);
		}
	}
}
