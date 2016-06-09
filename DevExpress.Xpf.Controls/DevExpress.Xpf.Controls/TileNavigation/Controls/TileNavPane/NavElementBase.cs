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

using DevExpress.Mvvm.Native;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.WindowsUI.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
namespace DevExpress.Xpf.Navigation {
	public abstract class NavElementBase : TileSelectorItem, INavElement {
		#region static
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty AllowSelectionProperty;
		public static readonly DependencyProperty TileContentProperty;
		public static readonly DependencyProperty TileContentTemplateProperty;
		public static readonly DependencyProperty TileContentTemplateSelectorProperty;
		static NavElementBase() {
			Type ownerType = typeof(NavElementBase);
			AllowSelectionProperty = DependencyProperty.Register("AllowSelection", typeof(bool), ownerType, new PropertyMetadata(true));
			TileContentProperty = DependencyProperty.Register("TileContent", typeof(object), ownerType, new PropertyMetadata(null, OnTileContentPropertyChanged));
			TileContentTemplateProperty = DependencyProperty.Register("TileContentTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, OnTileContentPropertyChanged));
			TileContentTemplateSelectorProperty = DependencyProperty.Register("TileContentTemplateSelector", typeof(DataTemplateSelector), ownerType, new PropertyMetadata(null, OnTileContentPropertyChanged));
		}
		private static void OnTileContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			NavElementBase navElementBase = d as NavElementBase;
			if(navElementBase != null)
				navElementBase.UpdateActualTileContent();
		}
		#endregion
		protected NavElementBase() {
		}
		protected override ControlControllerBase CreateController() {
			return new TileNavElementController(this);
		}
		protected virtual bool GetHasItemsCore() {
			return false;
		}
		void PrepareContainerCore(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			if(IsSelected) EnsureOwnerSelection(IsSelected);
		}
		protected override void ClearContainer() {
			base.ClearContainer();
		}
		protected override void PrepareContainer(object item, DataTemplate itemTemplate, DataTemplateSelector itemTemplateSelector) {
			base.PrepareContainer(item, itemTemplate, itemTemplateSelector);
			PrepareContainerCore(item, itemTemplate, itemTemplateSelector);
		}
		protected override void UpdateActualTileContent() {
			SetValue(ActualTileContentPropertyKey, TileContent ?? Content);
			SetValue(ActualTileContentTemplatePropertyKey, TileContentTemplate ?? ContentTemplate);
			SetValue(ActualTileContentTemplateSelectorPropertyKey, TileContentTemplateSelector ?? ContentTemplateSelector);
		}
		protected override void OnIsSelectedChanged(bool isSelected) {
			base.OnIsSelectedChanged(isSelected);
			if(!isSelected && Owner != null && Owner.SelectedItem == this) {
				Owner.SelectedItem = null;
			}
			INavElement NavElement = this;
			var tileNavPane = NavElement.TileNavPane;
			if(tileNavPane != null) tileNavPane.SetSelectedElement(this, isSelected);
		}
		private TileNavPane GetTileNavPane() {
			if(TileNavPane != null) return TileNavPane;
			INavElement navElement = this;
			INavElement parent = navElement.NavParent;
			while(parent != null) {
				if(parent.TileNavPane != null) return parent.TileNavPane;
				parent = parent.NavParent;
			}
			return null;
		}
		protected override void UpdateFlyout(Editors.Flyout.FlyoutControl flyoutControl) {
			base.UpdateFlyout(flyoutControl);
			flyoutControl.AlwaysOnTop = true;
		}
		protected override void OnFlyoutClosing(ref bool cancel) {
			cancel = GetTileNavPane().Return(x => x.ContinuousNavigation, () => false);
		}
		internal bool AllowSelection {
			get { return (bool)GetValue(AllowSelectionProperty); }
			set { SetValue(AllowSelectionProperty, value); }
		}
		[TypeConverter(typeof(StringConverter))]
		public object TileContent {
			get { return GetValue(TileContentProperty); }
			set { SetValue(TileContentProperty, value); }
		}
		public DataTemplate TileContentTemplate {
			get { return (DataTemplate)GetValue(TileContentTemplateProperty); }
			set { SetValue(TileContentTemplateProperty, value); }
		}
		public DataTemplateSelector TileContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(TileContentTemplateSelectorProperty); }
			set { SetValue(TileContentTemplateSelectorProperty, value); }
		}
		protected override bool CanBeSelected {
			get {
				return base.CanBeSelected && AllowSelection;
			}
		}
		internal override FlyoutManager GetFlyoutManager() {
			if(INavElement.TileNavPane != null) {
				return FlyoutManager.GetFlyoutManager(INavElement.TileNavPane);
			}
			return base.GetFlyoutManager();
		}
		protected override void OnClick() {
			bool isSelected = IsSelected;
			base.OnClick();
			if(isSelected)
				INavElement.TileNavPane.Do(x => x.UpdateFlayout());
		}
		#region INavElement Members
		INavElement INavElement { get { return (INavElement)this; } }
		INavElement INavElement.NavParent { get; set; }
		bool INavElement.AllowSelection {
			get { return (bool)GetValue(AllowSelectionProperty); }
			set { SetValue(AllowSelectionProperty, value); }
		}
		bool INavElement.HasItems {
			get { return GetHasItemsCore(); }
		}
		protected TileNavPane TileNavPane { get; private set; }
		TileNavPane INavElement.TileNavPane {
			get { return GetTileNavPane(); }
			set { TileNavPane = value; }
		}
		#endregion
		#region ILogicalOwner Members
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			RemoveLogicalChild(child);
		}
		#endregion
	}
}
