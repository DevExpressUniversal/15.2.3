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

using DevExpress.Utils.Controls;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraEditors;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
namespace DevExpress.XtraToolbox {
	public abstract class ToolboxOptionsBase : BaseOptions {
		ToolboxOptionChangingEventHandler changingCore;
		protected void OnChanging(string name, object value) {
			OnChanging(new ToolboxOptionChangingEventArgs(name, value));
		}
		protected virtual void OnChanging(ToolboxOptionChangingEventArgs e) {
			if(IsLockUpdate) return;
			RaiseOnChanging(e);
		}
		void RaiseOnChanging(ToolboxOptionChangingEventArgs e) {
			if(changingCore == null) return;
			changingCore(this, e);
		}
		internal event ToolboxOptionChangingEventHandler Changing {
			add { this.changingCore += value; }
			remove { this.changingCore -= value; }
		}
		internal event BaseOptionChangedEventHandler Changed {
			add { base.ChangedCore += value; }
			remove { base.ChangedCore -= value; }
		}
		protected internal new bool ShouldSerialize(IComponent owner) {
			return base.ShouldSerialize(owner);
		}
	}
	public delegate void ToolboxOptionChangingEventHandler(object sender, ToolboxOptionChangingEventArgs e);
	public class ToolboxOptionChangingEventArgs : EventArgs {
		readonly string name;
		readonly object value;
		public ToolboxOptionChangingEventArgs(string name, object value) {
			this.name = name;
			this.value = value;
		}
		public string Name {
			get { return name; }
		}
		public object Value {
			get { return value; }
		}
	}
	#region Behavior Options
	public class ToolboxBehaviorOptionChangedEventArgs : BaseOptionChangedEventArgs {
		public ToolboxBehaviorOptionChangedEventArgs(string name, object oldValue, object newValue) : base(name, oldValue, newValue) { }
	}
	public class ToolboxOptionsBehavior : ToolboxOptionsBase {
		bool allowSmoothScrolling;
		ToolboxItemSelectMode itemSelectMode;
		public ToolboxOptionsBehavior() : base() {
			this.allowSmoothScrolling = false;
			this.itemSelectMode = ToolboxItemSelectMode.None;
		}
		[ DefaultValue(false)]
		public bool AllowSmoothScrolling {
			get { return allowSmoothScrolling; }
			set {
				if(AllowSmoothScrolling == value)
					return;
				var oldValue = AllowSmoothScrolling;
				OnChanging("AllowSmoothScrolling", AllowSmoothScrolling);
				allowSmoothScrolling = value;
				OnChanged(new ToolboxBehaviorOptionChangedEventArgs("AllowSmoothScrolling", oldValue, AllowSmoothScrolling));
			}
		}
		[ DefaultValue(typeof(ToolboxItemSelectMode), "None")]
		public ToolboxItemSelectMode ItemSelectMode {
			get { return itemSelectMode; }
			set {
				if(ItemSelectMode == value)
					return;
				var oldValue = ItemSelectMode;
				OnChanging("ItemSelectMode", ItemSelectMode);
				itemSelectMode = value;
				OnChanged(new ToolboxBehaviorOptionChangedEventArgs("ItemSelectMode", oldValue, ItemSelectMode));
			}
		}
	}
	#endregion
	#region Minimizing Options 
	public class ToolboxMinimizeOptionChangedEventArgs : BaseOptionChangedEventArgs {
		public ToolboxMinimizeOptionChangedEventArgs(string name, object oldValue, object newValue) : base(name, oldValue, newValue) { }
	}
	public class ToolboxOptionsMinimizing : ToolboxOptionsBase {
		bool allowMinimizing;
		int minimizedWidth, normalWidth;
		ToolboxState state;
		MinimizeButtonMode minimizeButtonMode;
		int columnCount;
		ToolboxMinimizingScrollMode scrollMode;
		public ToolboxOptionsMinimizing() : base() {
			this.allowMinimizing = true;
			this.minimizedWidth = -1;
			this.normalWidth = -1;
			this.columnCount = 1;
			this.state = ToolboxState.Normal;
			this.minimizeButtonMode = MinimizeButtonMode.Default;
			this.scrollMode = ToolboxMinimizingScrollMode.Default;
		}
		[ Category(CategoryName.Appearance), DefaultValue(true)]
		public bool AllowMinimizing {
			get { return allowMinimizing; }
			set {
				if(AllowMinimizing == value)
					return;
				var oldValue = AllowMinimizing;
				OnChanging("AllowMinimizing", AllowMinimizing);
				allowMinimizing = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("AllowMinimizing", oldValue, AllowMinimizing));
			}
		}
		[ Category(CategoryName.Appearance), DefaultValue(ToolboxState.Normal)]
		public ToolboxState State {
			get { return AllowMinimizing ? state : ToolboxState.Normal; }
			set {
				if(State == value)
					return;
				var oldValue = State;
				OnChanging("State", State);
				state = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("State", oldValue, State));
			}
		}
		[ DefaultValue(-1)]
		public int MinimizedWidth {
			get { return minimizedWidth; }
			set {
				if(MinimizedWidth == value)
					return;
				var oldValue = MinimizedWidth;
				OnChanging("MinimizedWidth", MinimizedWidth);
				minimizedWidth = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("MinimizedWidth", oldValue, MinimizedWidth));
			}
		}
		[ DefaultValue(-1)]
		public int NormalWidth {
			get { return normalWidth; }
			set {
				if(NormalWidth == value)
					return;
				var oldValue = NormalWidth;
				OnChanging("NormalWidth", NormalWidth);
				normalWidth = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("NormalWidth", oldValue, NormalWidth));
			}
		}
		[ DefaultValue(MinimizeButtonMode.Default)]
		public MinimizeButtonMode MinimizeButtonMode {
			get { return minimizeButtonMode; }
			set {
				if(MinimizeButtonMode == value)
					return;
				var oldValue = MinimizeButtonMode;
				OnChanging("MinimizeButtonMode", MinimizeButtonMode);
				minimizeButtonMode = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("MinimizeButtonMode", oldValue, MinimizeButtonMode));
			}
		}
		[ DefaultValue(1)]
		public int ColumnCount {
			get { return columnCount; }
			set {
				if(ColumnCount == value)
					return;
				if(value < 1)
					throw new ArgumentOutOfRangeException("ColumnCount >= 1");
				var oldValue = ColumnCount;
				OnChanging("ColumnCount", ColumnCount);
				columnCount = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("ColumnCount", oldValue, ColumnCount));
			}
		}
		[ DefaultValue(typeof(ToolboxMinimizingScrollMode), "Default")]
		public ToolboxMinimizingScrollMode ScrollMode {
			get { return scrollMode; }
			set {
				if(ScrollMode == value)
					return;
				var oldValue = ScrollMode;
				OnChanging("ScrollMode", ScrollMode);
				scrollMode = value;
				OnChanged(new ToolboxMinimizeOptionChangedEventArgs("ScrollMode", oldValue, ScrollMode));
			}
		}
	}
	#endregion
	#region View Options
	public class ToolboxViewOptionChangedEventArgs : BaseOptionChangedEventArgs {
		public ToolboxViewOptionChangedEventArgs(string name, object oldValue, object newValue) : base(name, oldValue, newValue) { }
	}
	public class ToolboxOptionsView : ToolboxOptionsBase {
		const int DefaultColumnCount = 2;
		const int DefaultImageToTextDistance = 5;
		public ToolboxOptionsView() : base() {
			this.columnCount = DefaultColumnCount;
			this.imageToTextDistance = DefaultImageToTextDistance;
			this.showMenuButton = true;
			this.showToolboxCaption = false;
			this.menuButtonCaption = "Menu";
			this.itemImageSize = Size.Empty;
			this.itemViewMode = ToolboxItemViewMode.IconAndName;
			this.menuButtonImage = null;
			this.moreItemsButtonImage = null;
			this.groupPanelAutoHeight = true;
			this.showSearchPanel = true;
		}
		bool showMenuButton;
		[ DefaultValue(true)]
		public bool ShowMenuButton {
			get { return showMenuButton; }
			set {
				if(ShowMenuButton == value)
					return;
				var oldValue = ShowMenuButton;
				OnChanging("ShowMenuButton", ShowMenuButton);
				showMenuButton = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ShowMenuButton", oldValue, ShowMenuButton));
			}
		}
		string menuButtonCaption;
		[ DefaultValue("Menu")]
		public string MenuButtonCaption {
			get { return menuButtonCaption; }
			set {
				if(MenuButtonCaption == value)
					return;
				var oldValue = MenuButtonCaption;
				OnChanging("MenuButtonCaption", MenuButtonCaption);
				menuButtonCaption = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("MenuButtonCaption", oldValue, MenuButtonCaption));
			}
		}
		Image menuButtonImage;
		[Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		[ DefaultValue(null)]
		public Image MenuButtonImage {
			get { return menuButtonImage; }
			set {
				if(MenuButtonImage == value)
					return;
				var oldValue = MenuButtonImage;
				OnChanging("MenuButtonImage", MenuButtonImage);
				menuButtonImage = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("MenuButtonImage", oldValue, MenuButtonImage));
			}
		}
		Image moreItemsButtonImage;
		[Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(UITypeEditor))]
		[ DefaultValue(null)]
		public Image MoreItemsButtonImage {
			get { return moreItemsButtonImage; }
			set {
				if(MoreItemsButtonImage == value)
					return;
				var oldValue = MoreItemsButtonImage;
				OnChanging("MoreItemsButtonImage", MoreItemsButtonImage);
				moreItemsButtonImage = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("MoreItemsButtonImage", oldValue, MoreItemsButtonImage));
			}
		}
		bool showToolboxCaption;
		[ DefaultValue(false)]
		public bool ShowToolboxCaption {
			get { return showToolboxCaption; }
			set {
				if(ShowToolboxCaption == value)
					return;
				var oldValue = ShowToolboxCaption;
				OnChanging("ShowToolboxCaption", ShowToolboxCaption);
				showToolboxCaption = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ShowToolboxCaption", oldValue, ShowToolboxCaption));
			}
		}
		Size itemImageSize;
		public Size ItemImageSize {
			get { return itemImageSize; }
			set {
				if(ItemImageSize == value)
					return;
				var oldValue = ItemImageSize;
				OnChanging("ItemImageSize", ItemImageSize);
				itemImageSize = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ItemImageSize", oldValue, ItemImageSize));
			}
		}
		bool ShouldSerializeItemImageSize() {
			return !ItemImageSize.IsEmpty;
		}
		void ResetItemImageSize() {
			ItemImageSize = Size.Empty;
		}
		ToolboxItemViewMode itemViewMode;
		[ DefaultValue(typeof(ToolboxItemViewMode), "IconAndName")]
		public ToolboxItemViewMode ItemViewMode {
			get { return itemViewMode; }
			set {
				if(ItemViewMode == value)
					return;
				var oldValue = ItemViewMode;
				OnChanging("ItemViewMode", ItemViewMode);
				itemViewMode = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ItemViewMode", oldValue, ItemViewMode));
			}
		}
		bool showSearchPanel;
		[ DefaultValue(true)]
		public bool ShowSearchPanel {
			get { return showSearchPanel; }
			set {
				if(ShowSearchPanel == value)
					return;
				var oldValue = ShowSearchPanel;
				OnChanging("ShowSearchPanel", ShowSearchPanel);
				showSearchPanel = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ShowSearchPanel", oldValue, ShowSearchPanel));
			}
		}
		int columnCount;
		[ DefaultValue(DefaultColumnCount)]
		public int ColumnCount {
			get { return columnCount; }
			set {
				if(columnCount == value)
					return;
				var oldValue = ColumnCount;
				OnChanging("ColumnCount", ColumnCount);
				columnCount = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ColumnCount", oldValue, ColumnCount));
			}
		}
		int imageToTextDistance;
		[ DefaultValue(DefaultImageToTextDistance)]
		public int ImageToTextDistance {
			get { return imageToTextDistance; }
			set {
				if(ImageToTextDistance == value)
					return;
				var oldValue = ImageToTextDistance;
				OnChanging("ImageToTextDistance", ImageToTextDistance);
				imageToTextDistance = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("ImageToTextDistance", oldValue, ImageToTextDistance));
			}
		}
		bool groupPanelAutoHeight;
		[ DefaultValue(true)]
		public bool GroupPanelAutoHeight {
			get { return groupPanelAutoHeight; }
			set {
				if(GroupPanelAutoHeight == value)
					return;
				var oldValue = GroupPanelAutoHeight;
				OnChanging("GroupPanelAutoHeight", GroupPanelAutoHeight);
				groupPanelAutoHeight = value;
				OnChanged(new ToolboxViewOptionChangedEventArgs("GroupPanelAutoHeight", oldValue, GroupPanelAutoHeight));
			}
		}
		protected override void OnChanging(ToolboxOptionChangingEventArgs e) {
			base.OnChanging(e);
		}
	}
	#endregion
}
