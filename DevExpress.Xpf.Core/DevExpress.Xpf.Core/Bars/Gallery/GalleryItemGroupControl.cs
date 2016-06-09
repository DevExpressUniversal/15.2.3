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

using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using DevExpress.Xpf.Bars.Automation;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Bars {
	public class GalleryItemGroupControl : ItemsControl {
		#region static
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty GroupsControlProperty;
		public static readonly DependencyProperty ActualIsCaptionVisibleProperty;
		protected static readonly DependencyPropertyKey ActualIsCaptionVisiblePropertyKey;
		public static readonly DependencyProperty IsFirstVisibleGroupProperty;
		protected internal static readonly DependencyPropertyKey IsFirstVisibleGroupPropertyKey;
		static GalleryItemGroupControl() {
			DevExpress.Xpf.Bars.Automation.NavigationAutomationPeersCreator.Default.RegisterObject(typeof(GalleryItemGroupControl), typeof(GalleryItemGroupControlAutomationPeer), owner => new GalleryItemGroupControlAutomationPeer((GalleryItemGroupControl)owner));
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GalleryItemGroupControl), new FrameworkPropertyMetadata(typeof(GalleryItemGroupControl)));
			GroupProperty = DependencyPropertyManager.Register("Group", typeof(GalleryItemGroup), typeof(GalleryItemGroupControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupPropertyChanged)));
			GroupsControlProperty = DependencyPropertyManager.Register("GroupsControl", typeof(GalleryItemGroupsControl), typeof(GalleryItemGroupControl),
				new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnGroupsControlPropertyChanged)));
			ActualIsCaptionVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualIsCaptionVisible", typeof(bool), typeof(GalleryItemGroupControl), new FrameworkPropertyMetadata(false));
			ActualIsCaptionVisibleProperty = ActualIsCaptionVisiblePropertyKey.DependencyProperty;
			IsFirstVisibleGroupPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFirstVisibleGroup", typeof(bool), typeof(GalleryItemGroupControl), new FrameworkPropertyMetadata(false));
			IsFirstVisibleGroupProperty = IsFirstVisibleGroupPropertyKey.DependencyProperty;
		}
		static void OnGroupPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroupControl)obj).OnGroupChanged(e.OldValue as GalleryItemGroup);
		}
		static void OnGroupsControlPropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e) {
			((GalleryItemGroupControl)obj).OnGroupsControlChanged(e.OldValue as GalleryItemGroupControl);
		}
		#endregion
		#region dep props
		public GalleryItemGroup Group {
			get { return (GalleryItemGroup)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		public GalleryItemGroupsControl GroupsControl {
			get { return (GalleryItemGroupsControl)GetValue(GroupsControlProperty); }
			set { SetValue(GroupsControlProperty, value); }
		}
		public bool ActualIsCaptionVisible {
			get { return (bool)GetValue(ActualIsCaptionVisibleProperty); }
			protected set { this.SetValue(ActualIsCaptionVisiblePropertyKey, value); }
		}
		public bool IsFirstVisibleGroup {
			get { return (bool)GetValue(IsFirstVisibleGroupProperty); }
			protected internal set { this.SetValue(IsFirstVisibleGroupPropertyKey, value); }
		}
		#endregion
		public GalleryItemGroupControl() {
			IsTabStop = false;
		}
		protected GalleryControl GalleryControl {
			get {
				return GroupsControl == null ? null : GroupsControl.GalleryControl;
			}
		}
		UIElement ItemsElement { get; set; }
		protected virtual void OnGroupsControlChanged(GalleryItemGroupControl oldValue) {
			UpdateActualValues();
		}
		protected internal virtual void OnGroupsControlGalleryControlChanged() {
			UpdateActualValues();
			for(int i = 0; i < Items.Count; i++) {
				var galleryItemControl = ItemContainerGenerator.ContainerFromIndex(i) as GalleryItemControl;
				if(galleryItemControl == null) return;
				galleryItemControl.UpdateActualValues();
			}
		}
		protected internal virtual void UpdateActualValues() {
			UpdateActualCaptionVisibility();
		}
		protected virtual void OnGroupChanged(GalleryItemGroup oldValue) {
			ItemsSource = Group == null ? null : Group.Items;
			if(oldValue != null) {
				oldValue.CaptionVisibilityChanged -= OnGroupCaptionVisibilityChanged;
				if(oldValue.Gallery != null) {
					oldValue.Gallery.IsGroupCaptionVisibleChanged -= OnGalleryGroupCaptionVisibilityChanged;
				}
			}
			if(Group != null) {
				Group.CaptionVisibilityChanged += new System.EventHandler(OnGroupCaptionVisibilityChanged);
				if(Group.Gallery != null) {
					Group.Gallery.IsGroupCaptionVisibleChanged += new System.EventHandler(OnGalleryGroupCaptionVisibilityChanged);
				}
			}
			UpdateActualCaptionVisibility();
		}
		void OnGalleryGroupCaptionVisibilityChanged(object sender, System.EventArgs e) {
			UpdateActualCaptionVisibility();
		}
		void OnGroupCaptionVisibilityChanged(object sender, System.EventArgs e) {
			UpdateActualCaptionVisibility();
		}
		protected virtual void UpdateActualCaptionVisibility() {
			if(Group == null) return;
			if(Group.IsCaptionVisible != DevExpress.Utils.DefaultBoolean.Default) {
				ActualIsCaptionVisible = Group.IsCaptionVisible == DevExpress.Utils.DefaultBoolean.True;
				return;
			}
			if(GroupsControl != null && GroupsControl.GalleryControl != null)
				ActualIsCaptionVisible = GroupsControl.GalleryControl.ActualIsGroupCaptionVisible;
		}
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return NavigationAutomationPeersCreator.Default.Create(this);
		}
		protected override bool IsItemItsOwnContainerOverride(object item) {
			return item is GalleryItemControl;
		}
		protected override void PrepareContainerForItemOverride(DependencyObject element, object item) {
			(element as GalleryItemControl).Item = item as GalleryItem;
			(element as GalleryItemControl).GroupControl = this;		   
			base.PrepareContainerForItemOverride(element, item);
		}
		protected override DependencyObject GetContainerForItemOverride() {
			return new GalleryItemControl();
		}
		protected override void ClearContainerForItemOverride(DependencyObject element, object item) {
			(element as GalleryItemControl).Item = null;
			(element as GalleryItemControl).GroupControl = null;
			base.ClearContainerForItemOverride(element, item);
		}
		public GalleryItemControl GetItem(int index) {
			return ItemContainerGenerator.ContainerFromIndex(index) as GalleryItemControl;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			ItemsElement = GetTemplateChild("PART_Items") as UIElement;
		}
		public int GetRowCount() {
			int count = 0;
			for(int i = 0; i < Items.Count; i++) {
				GalleryItemControl itemControl = GetItem(i);
				if(!itemControl.Item.IsVisible)
					continue;
				if(itemControl.DesiredStartOfLine) {
					count++;
				}
			}
			return count;
		}
		internal double GetCaptionOffset() {
			return DesiredSize.Height - ItemsElement.DesiredSize.Height;
		}
	}
}
