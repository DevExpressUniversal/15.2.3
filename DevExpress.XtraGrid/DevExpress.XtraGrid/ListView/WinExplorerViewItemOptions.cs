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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing.Animation;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Container;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Scrolling;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraGrid.Views.WinExplorer.Handler;
using DevExpress.XtraGrid.Views.WinExplorer.ViewInfo;
using DevExpress.XtraGrid.Views.Base.ViewInfo;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Gesture;
using DevExpress.Utils.Serializing;
namespace DevExpress.XtraGrid.Views.WinExplorer {
	public enum WinExplorerViewStyle { Default, ExtraLarge, Large, Medium, Small, Tiles, List, Content }
	public enum WinExplorerViewFieldType { Text, Description, CheckBox, Image }
	public enum IconItemSelectionMode { None, Press, Click, Hover }
	public class WinExplorerViewContentStyleOptions : WinExplorerViewStyleOptions {
		int descriptionMinOffset;
		int descriptionMaxOffset;
		int descriptionMinWidth;
		const int DefaultDescriptionMinOffset = 120;
		const int DefaultDescriptionMaxOffset = 250;
		const int DefaultDescriptionMinWidth = 300;
		public WinExplorerViewContentStyleOptions(WinExplorerView view) : base(view) {
			this.descriptionMaxOffset = DefaultDescriptionMaxOffset;
			this.descriptionMinOffset = DefaultDescriptionMinOffset;
			this.descriptionMinWidth = DefaultDescriptionMinWidth;
		}
		[DefaultValue(DefaultDescriptionMinOffset), XtraSerializableProperty]
		public int DescriptionMinOffset {
			get { return descriptionMinOffset; }
			set {
				if(DescriptionMinOffset == value)
					return;
				int prev = DescriptionMinOffset;
				descriptionMinOffset = value;
				OnChanged(new BaseOptionChangedEventArgs("DescriptionMinOffset", prev, DescriptionMinOffset));
			}
		}
		[DefaultValue(DefaultDescriptionMinWidth), XtraSerializableProperty]
		public int DescriptionMinWidth {
			get { return descriptionMinWidth; }
			set {
				if(DescriptionMinWidth == value)
					return;
				int prev = DescriptionMinWidth;
				descriptionMinWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("DescriptionMinWidth", prev, DescriptionMinWidth));
			}
		}
		[DefaultValue(DefaultDescriptionMaxOffset), XtraSerializableProperty]
		public int DescriptionMaxOffset {
			get { return descriptionMaxOffset; }
			set {
				if(DescriptionMaxOffset == value)
					return;
				int prev = DescriptionMaxOffset;
				descriptionMaxOffset = value;
				OnChanged(new BaseOptionChangedEventArgs("DescriptionMaxOffset", prev, DescriptionMaxOffset));
			}
		}
	}
	public enum SelectionDrawMode { AroundItem, AroundImage }
	public class WinExplorerViewStyleOptions : ViewBaseOptions {
		Padding imageMargins;
		Padding contentMargins;
		Padding checkBoxMargins;
		int horizontalIndent;
		int verticalIndent;
		int indentBetweenGroupAndItem;
		int imageToTextIndent;
		int itemWidth;
		int groupCaptionButtonIndent;
		int groupCheckBoxIndent;
		int indentBetweenGroups;
		Size imageSize;
		DefaultBoolean showDescription = DefaultBoolean.Default;
		SelectionDrawMode selectionDrawMode = SelectionDrawMode.AroundImage;
		public WinExplorerViewStyleOptions(WinExplorerView view)
			: base() {
			View = view;
			this.imageMargins = new Padding(-1);
			contentMargins = new Padding(-1);
			checkBoxMargins = new Padding(-1);
			this.horizontalIndent = -1;
			this.verticalIndent = -1;
			this.indentBetweenGroupAndItem = -1;
			this.imageToTextIndent = -1;
			this.itemWidth = -1;
			this.groupCaptionButtonIndent = -1;
			this.GroupCheckBoxIndent = -1;
			this.indentBetweenGroups = -1;
			this.imageSize = Size.Empty;
		}
		[XtraSerializableProperty, DefaultValue(SelectionDrawMode.AroundImage)]
		public SelectionDrawMode SelectionDrawMode {
			get { return selectionDrawMode; }
			set { 
				if(SelectionDrawMode == value)
					return;
				SelectionDrawMode prev = SelectionDrawMode;
				selectionDrawMode = value;
				OnChanged(new BaseOptionChangedEventArgs("SelectionDrawMode", prev, SelectionDrawMode));
			}
		}
		[XtraSerializableProperty, DefaultValue(DefaultBoolean.Default)]
		public DefaultBoolean ShowDescription {
			get { return showDescription; }
			set {
				DefaultBoolean prev = ShowDescription;
				if(ShowDescription == value)
					return;
				showDescription = value;
				OnChanged(new BaseOptionChangedEventArgs("ShowDescription", prev, ShowDescription));
			}
		}
		protected WinExplorerView View { get; set; }
		bool ShouldSerializeImageMargins() { return !ImageMargins.Equals(new Padding(-1)); }
		internal bool IsImageMarginsSet { get { return ShouldSerializeImageMargins(); } }
		void ResetImageMargins() { ImageMargins = new Padding(-1); }
		[XtraSerializableProperty]
		public Padding ImageMargins {
			get { return imageMargins; }
			set {
				if(ImageMargins == value)
					return;
				Padding prev = ImageMargins;
				imageMargins = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageMargins", prev, ImageMargins));
			}
		}
		bool ShouldSerializeContentMargins() { return !ContentMargins.Equals(new Padding(-1)); }
		internal bool IsContentMarginsSet { get { return ShouldSerializeContentMargins(); } }
		void ResetContentMargins() { ContentMargins = new Padding(-1); }
		[XtraSerializableProperty]
		public Padding ContentMargins {
			get { return contentMargins; }
			set {
				if(ContentMargins == value)
					return;
				Padding prev = ContentMargins;
				contentMargins = value;
				OnChanged(new BaseOptionChangedEventArgs("ContentMargins", prev, ContentMargins));
			}
		}
		bool ShouldSerializeCheckBoxMargins() { return !CheckBoxMargins.Equals(new Padding(-1)); }
		internal bool IsCheckBoxMarginsSet { get { return ShouldSerializeCheckBoxMargins(); } }
		void ResetCheckBoxMargins() { CheckBoxMargins = new Padding(-1); }
		[XtraSerializableProperty]
		public Padding CheckBoxMargins {
			get { return checkBoxMargins; }
			set {
				if(CheckBoxMargins == value)
					return;
				Padding prev = CheckBoxMargins;
				checkBoxMargins = value;
				OnChanged(new BaseOptionChangedEventArgs("CheckBoxMargins", prev, CheckBoxMargins));
			}
		}
		bool ShouldSerializeImageSize() { return !ImageSize.Equals(Size.Empty); }
		internal bool IsImageSizeSet { get { return ShouldSerializeImageSize(); } }
		void ResetImageSize() { ImageSize = Size.Empty; }
		[XtraSerializableProperty]
		public Size ImageSize {
			get { return imageSize; }
			set {
				if(ImageSize == value)
					return;
				Size prev = ImageSize;
				imageSize = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageSize", prev, ImageSize));
			}
		}
		internal bool IsHorizontalIndentSet { get { return HorizontalIndent != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int HorizontalIndent {
			get { return horizontalIndent; }
			set {
				if(HorizontalIndent == value)
					return;
				int prev = HorizontalIndent;
				horizontalIndent = value;
				OnChanged(new BaseOptionChangedEventArgs("HorizontalIndent", prev, HorizontalIndent));
			}
		}
		internal bool IsVerticalIndentSet { get { return VerticalIndent != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int VerticalIndent {
			get { return verticalIndent; }
			set {
				if(VerticalIndent == value)
					return;
				int prev = VerticalIndent;
				verticalIndent = value;
				OnChanged(new BaseOptionChangedEventArgs("VerticalIndent", prev, VerticalIndent));
			}
		}
		internal bool IsIndentBetweenGroupAndItemSet { get { return IndentBetweenGroupAndItem != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int IndentBetweenGroupAndItem {
			get { return indentBetweenGroupAndItem; }
			set {
				if(IndentBetweenGroupAndItem == value)
					return;
				int prev = IndentBetweenGroupAndItem;
				indentBetweenGroupAndItem = value;
				OnChanged(new BaseOptionChangedEventArgs("IndentBetweenGroupAndItem", prev, IndentBetweenGroupAndItem));
			}
		}
		internal bool IsIndentBetweenGroupsSet { get { return IndentBetweenGroups != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int IndentBetweenGroups {
			get { return indentBetweenGroups; }
			set {
				if(IndentBetweenGroups == value)
					return;
				int prev = IndentBetweenGroups;
				indentBetweenGroups = value;
				OnChanged(new BaseOptionChangedEventArgs("IndentBetweenGroups", prev, IndentBetweenGroups));
			}
		}
		internal bool IsImageToTextIndentSet { get { return ImageToTextIndent != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int ImageToTextIndent {
			get { return imageToTextIndent; }
			set {
				if(ImageToTextIndent == value)
					return;
				int prev = ImageToTextIndent;
				imageToTextIndent = value;
				OnChanged(new BaseOptionChangedEventArgs("ImageToTextIndent", prev, ImageToTextIndent));
			}
		}
		internal bool IsItemWidthSet { get { return ItemWidth != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int ItemWidth {
			get { return itemWidth; }
			set {
				if(ItemWidth == value)
					return;
				int prev = ItemWidth;
				itemWidth = value;
				OnChanged(new BaseOptionChangedEventArgs("ItemWidth", prev, ItemWidth));
			}
		}
		public bool IsGroupCaptionButtonIndentSet { get { return GroupCaptionButtonIndent != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int GroupCaptionButtonIndent {
			get { return groupCaptionButtonIndent; }
			set {
				if(GroupCaptionButtonIndent == value)
					return;
				int prev = GroupCaptionButtonIndent;
				groupCaptionButtonIndent = value;
				OnChanged(new BaseOptionChangedEventArgs("GroupCaptionButtonIndent", prev, GroupCaptionButtonIndent));
			}
		}
		public bool IsGroupCheckBoxIndentSet { get { return GroupCheckBoxIndent != -1; } }
		[DefaultValue(-1), XtraSerializableProperty]
		public int GroupCheckBoxIndent {
			get { return groupCheckBoxIndent; }
			set {
				if(GroupCheckBoxIndent == value)
					return;
				int prev = GroupCheckBoxIndent;
				groupCheckBoxIndent = value;
				OnChanged(new BaseOptionChangedEventArgs("GroupCheckBoxIndent", prev, GroupCheckBoxIndent));
			}
		}
		protected virtual void OnPropertiesChanged() {
			if(View != null)
				View.OnPropertiesChanged();
		}
	}
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class WinExplorerViewStyleOptionsCollection : ViewBaseOptions {
		WinExplorerView view;
		WinExplorerViewStyleOptions extraLarge, large, medium, small, tiles, list;
		WinExplorerViewContentStyleOptions content;
		public WinExplorerViewStyleOptionsCollection(WinExplorerView view) {
			this.view = view;
			this.extraLarge = CreateItemViewOptions();
			this.large = CreateItemViewOptions();
			this.small = CreateItemViewOptions();
			this.medium = CreateItemViewOptions();
			this.tiles = CreateItemViewOptions();
			this.list = CreateItemViewOptions();
			this.content = CreateItemViewOptionsContent();
			this.extraLarge.Changed += OnItemViewOptionsChanged;
			this.large.Changed += OnItemViewOptionsChanged;
			this.medium.Changed += OnItemViewOptionsChanged;
			this.small.Changed += OnItemViewOptionsChanged;
			this.tiles.Changed += OnItemViewOptionsChanged;
			this.list.Changed += OnItemViewOptionsChanged;
			this.content.Changed += OnItemViewOptionsChanged;
		}
		protected virtual void OnItemViewOptionsChanged(object sender, BaseOptionChangedEventArgs e) {
			if(View != null)
				View.OnOptionChangedCore(sender, e);
		}
		protected virtual WinExplorerViewStyleOptions CreateItemViewOptions() {
			return new WinExplorerViewStyleOptions(View);
		}
		protected virtual WinExplorerViewContentStyleOptions CreateItemViewOptionsContent() {
			return new WinExplorerViewContentStyleOptions(View);
		}
		bool ShouldSerializeExtraLarge() { return ExtraLarge.ShouldSerializeCore(View); }
		void ResetExtraLarge() { ExtraLarge.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewStyleOptions ExtraLarge {
			get { return extraLarge; }
		}
		bool ShouldSerializeLarge() { return Large.ShouldSerializeCore(View); }
		void ResetLarge() { Large.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewStyleOptions Large {
			get { return large; }
		}
		bool ShouldSerializeMedium() { return Medium.ShouldSerializeCore(View); }
		void ResetMedium() { Medium.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewStyleOptions Medium {
			get { return medium; }
		}
		bool ShouldSerializeSmall() { return Small.ShouldSerializeCore(View); }
		void ResetSmall() { Small.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewStyleOptions Small {
			get { return small; }
		}
		bool ShouldSerializeTiles() { return Tiles.ShouldSerializeCore(View); }
		void ResetTiles() { Tiles.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewStyleOptions Tiles {
			get { return tiles; }
		}
		bool ShouldSerializeList() { return List.ShouldSerializeCore(View); }
		void ResetList() { List.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewStyleOptions List {
			get { return list; }
		}
		bool ShouldSerializeContent() { return Content.ShouldSerializeCore(View); }
		void ResetContent() { Content.Reset(); }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content), XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public WinExplorerViewContentStyleOptions Content {
			get { return content; }
		}
		public void CheckDefaults() {
			WinExplorerViewInfo vi = View.ViewInfo as WinExplorerViewInfo;
			if(vi == null)
				return;
			if(View.OptionsView.Style == WinExplorerViewStyle.Content) {
				Content.ItemWidth = vi.ContentBounds.Width;
			}
		}
		public WinExplorerView View { get { return view; } }
		protected List<WinExplorerViewStyleOptions> Items {
			get {
				List<WinExplorerViewStyleOptions> res = new List<WinExplorerViewStyleOptions>();
				res.Add(ExtraLarge);
				res.Add(Large);
				res.Add(Medium);
				res.Add(Small);
				res.Add(Tiles);
				res.Add(List);
				res.Add(Content);
				return res;
			}
		}
		public void SetContentMargins(Padding value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.ContentMargins = value;
			}
		}
		public void SetCheckBoxMargins(Padding value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.CheckBoxMargins = value;
			}
		}
		public void SetGroupCaptionButtonIndent(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.GroupCaptionButtonIndent = value;
			}
		}
		public void SetHorizontalIndent(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.HorizontalIndent = value;
			}
		}
		public void SetImageMargins(Padding value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.ImageMargins = value;
			}
		}
		public void SetImageToTextIndent(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.ImageToTextIndent = value;
			}
		}
		public void SetIndentBetweenGroupAndItem(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.IndentBetweenGroupAndItem = value;
			}
		}
		public void SetIndentBetweenGroups(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.IndentBetweenGroups = value;
			}
		}
		public void SetItemWidth(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.ItemWidth = value;
			}
		}
		public void SetVerticalIndent(int value) {
			foreach(WinExplorerViewStyleOptions opt in Items) {
				opt.VerticalIndent = value;
			}
		}
	}
}
