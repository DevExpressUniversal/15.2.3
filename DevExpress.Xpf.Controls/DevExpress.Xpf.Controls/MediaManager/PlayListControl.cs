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
using System.Text;
using DevExpress.Xpf.Core;
using System.Windows;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.ComponentModel;
namespace DevExpress.Xpf.Controls {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PlayListControl : ListBox {
		Button bRemoveAll;
		Button bRemoveSelectedItem;
		public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(PlayListControl), new PropertyMetadata(string.Empty));
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty MediaManagerProperty = DependencyProperty.Register("MediaManager", typeof(MediaManager), typeof(PlayListControl),
			new PropertyMetadata(null, (d, e) => ((PlayListControl)d).OnMediaManagerChanged(d, e)));
		public PlayListControl() {
			DefaultStyleKey = typeof(PlayListControl);
			Loaded += OnLoaded;
			SelectionChanged += OnSelectionChanged;
			DisplayMemberPath = "Description";
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			bRemoveSelectedItem = (Button)GetTemplateChild("bRemove");
			bRemoveAll = (Button)GetTemplateChild("bRemoveAll");
		}
		protected virtual void OnbRemoveAllClick(object sender, RoutedEventArgs e) {
			if(MediaManager != null) { MediaManager.MediaList.Clear(); }
		}
		protected virtual void OnbRemoveSelectedItemClick(object sender, RoutedEventArgs e) {
			if(MediaManager != null) { MediaManager.MediaList.Remove((MediaItem)SelectedItem); }
		}
		protected virtual void OnLoaded(object sender, RoutedEventArgs e) {
			if(MediaManager == null)
				InitializeMediaManager();
			InitializeSource();
			SubscribeEvents();
		}
		protected virtual void OnMediaElementMediaFailed(object sender, ExceptionRoutedEventArgs e) {
			UpdateSelectedItem();
		}
		protected virtual void OnMediaElementMediaOpened(object sender, RoutedEventArgs e) {
			UpdateSelectedItem();
		}
		protected virtual void OnMediaManagerChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			InitializeSource();
		}
		protected virtual void OnMediaManagerSelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e) {
			ItemsSource = MediaList;
			UpdateSelectedItem();
		}
		protected virtual void OnSelectionChanged(object sender, SelectionChangedEventArgs e) {
			if(((MediaManager != null) && (SelectedItem != null)) && MediaList.Contains((MediaItem)SelectedItem)) {
				MediaManager.SelectedIndex = MediaList.IndexOf((MediaItem)SelectedItem);
			}
		}
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		public ObservableCollection<MediaItem> MediaList {
			get { return ((MediaManager == null) ? null : MediaManager.MediaList); }
			set { if(value != MediaList) { MediaManager.MediaList = value; } }
		}
		public MediaManager MediaManager {
			get { return (MediaManager)GetValue(MediaManagerProperty); }
			internal set { SetValue(MediaManagerProperty, value); }
		}
		protected internal MediaElement MediaElement { get { return ((MediaManager == null) ? null : MediaManager.MediaElement); } }
		void InitializeMediaManager() {
			MediaManager = FindControlHelper.GetMediaManager(VisualTreeHelper.GetParent(this));
		}
		void InitializeSource() {
			ItemsSource = MediaList;
		}
		void SubscribeEvents() {
			if(MediaManager != null) {
				MediaElement.MediaOpened += new RoutedEventHandler(OnMediaElementMediaOpened);
				MediaElement.MediaFailed += new EventHandler<ExceptionRoutedEventArgs>(OnMediaElementMediaFailed);
				MediaManager.SelectedIndexChanged += new SelectedIndexChangedEventHandler(OnMediaManagerSelectedIndexChanged);
			}
			if(bRemoveAll != null) {
				bRemoveAll.Click += OnbRemoveAllClick;
			}
			if(bRemoveSelectedItem != null) {
				bRemoveSelectedItem.Click += OnbRemoveSelectedItemClick;
			}
		}
		void UpdateSelectedItem() {
			SelectedItem = MediaManager.SelectedItem;
			if(Items.Count > 0) {
				Caption = string.Concat(new object[] { "Playlist: ", MediaManager.SelectedIndex + 1, " of ", MediaList.Count });
			} else {
				Caption = "Playlist: <" + MediaList.Count + ">";
			}
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class PlayListInPopupMediaButton : MediaButton {
		public PlayListInPopupMediaButton() {
			DefaultStyleKey = typeof(PlayListInPopupMediaButton);
			Click += OnClick;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			popupWindow = (PopupBase)GetTemplateChild("PART_Popup");
			InitializePopup();
		}
		protected override bool SetEnabled() { return true; }
		protected virtual void OnClick(object sender, RoutedEventArgs e) {
			playlist.MediaManager = MediaManager;
			popupWindow.IsOpen = !popupWindow.IsOpen;
		}
		protected virtual void OnPopupWindowClosed(object sender, EventArgs e) {
		}
		protected virtual void OnRootVisualMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
			popupWindow.IsOpen = false;
			e.Handled = true;
		}
		void InitializePopup() {
			popupWindow.PlacementTarget = this;
			popupWindow.Placement = PlacementMode.Top;
			playlist = new PlayListControl();
			popupWindow.Child = playlist;
			popupWindow.Closed += OnPopupWindowClosed;
		}
		PlayListControl playlist;
		PopupBase popupWindow;
	}
}
