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
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Threading;
using System.ComponentModel;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Controls {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public enum MediaManagerState {
		Closed,
		Paused,
		Playing,
		Stopped
	}
	[TemplatePart(Name = "PART_MediaElementContentPresenter", Type = typeof(ContentPresenter)),
		TemplatePart(Name = "PART_InformationText", Type = typeof(TextBlock))]
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class MediaManager : Control {
		#region DependencyProperties
		public static readonly DependencyProperty BottomPanelProperty = DependencyProperty.Register("BottomPanel", typeof(UIElement), typeof(MediaManager),
			new PropertyMetadata(null));
		public static readonly DependencyProperty InformationProperty = DependencyProperty.Register("Information", typeof(string), typeof(MediaManager),
			new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty IsFullscreenProperty = DependencyProperty.Register("IsFullscreen", typeof(bool), typeof(MediaManager),
			new PropertyMetadata(false, (d, e) => ((MediaManager)d).OnIsFullscreenChanged(d, (bool)e.NewValue)));
		public static readonly DependencyProperty LeftPanelProperty = DependencyProperty.Register("LeftPanel", typeof(object), typeof(MediaManager),
			new PropertyMetadata(null));
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty MediaElementProperty = DependencyProperty.Register("MediaElement", typeof(MediaElement), typeof(MediaManager),
			new PropertyMetadata(null));
		public static readonly DependencyProperty MediaListProperty = DependencyProperty.Register("MediaList", typeof(ObservableCollection<MediaItem>), typeof(MediaManager),
			new PropertyMetadata(null, (d, e) => ((MediaManager)d).OnMediaListChanged(d, e)));
		public static readonly DependencyProperty RightPanelProperty = DependencyProperty.Register("RightPanel", typeof(object), typeof(MediaManager),
			new PropertyMetadata(null));
		public static readonly DependencyProperty SelectedIndexProperty =
			DependencyProperty.Register("SelectedIndex", typeof(int), typeof(MediaManager),
			new PropertyMetadata(-1, (d, e) => ((MediaManager)d).OnSelectedIndexChanged(d, e)));
		public static readonly DependencyProperty SelectedItemProperty =
			DependencyProperty.Register("SelectedItem", typeof(MediaItem), typeof(MediaManager),
			new PropertyMetadata(null, (d, e) => ((MediaManager)d).OnSelectedItemChanged(d, e)));
		public static readonly DependencyProperty TopPanelProperty = DependencyProperty.Register("TopPanel", typeof(object), typeof(MediaManager),
			new PropertyMetadata(null));
		public static readonly DependencyProperty IsRandomProperty =
			DependencyProperty.Register("IsRandom", typeof(bool), typeof(MediaManager), new PropertyMetadata(false));
		public static readonly DependencyProperty PositionInfoTypeProperty =
			DependencyProperty.Register("PositionInfoType", typeof(PositionInfoType), typeof(MediaManager), new PropertyMetadata(PositionInfoType.PositionDuration));
		public static readonly DependencyProperty PositionInfoTextProperty =
			DependencyProperty.Register("PositionInfoText", typeof(string), typeof(MediaManager), new PropertyMetadata("00:00:00"));
		#endregion
		#region Events
		public event CurrentStateChangedEventHandler CurrentStateChanged;
		public event IsFullscreenChangedEventHandler IsFullscreenChanged;
		public event SelectedIndexChangedEventHandler SelectedIndexChanged;
		public event SelectedItemChangedEventHandler SelectedItemChanged;
		public event RoutedEventHandler MediaEnded {
			add { MediaElement.MediaEnded += value; }
			remove { MediaElement.MediaEnded -= value; }
		}
		public event RoutedEventHandler MediaOpened {
			add { MediaElement.MediaOpened += value; }
			remove { MediaElement.MediaOpened -= value; }
		}
		public event EventHandler<ExceptionRoutedEventArgs> MediaFailed {
			add { MediaElement.MediaFailed += value; }
			remove { MediaElement.MediaFailed -= value; }
		}
		#endregion
		public MediaManager() {
			DefaultStyleKey = typeof(MediaManager);
			Initialize();
			SubscribeEvents();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			MediaElementContentPresenter = GetTemplateChild("PART_MediaElementContentPresenter") as ContentPresenter;
			SetPositionForMediaElement();
		}
		protected virtual void OnIsFullscreenChanged(DependencyObject d, bool e) {
#if WPF
			if(Application.Current == null) return;
			if(IsFullscreen) {
				SaveWindowSettings();
				MediaElement.Focus();
				Application.Current.MainWindow.WindowStyle = WindowStyle.None;
				Application.Current.MainWindow.WindowState = WindowState.Normal;
				Application.Current.MainWindow.WindowState = WindowState.Maximized;
			} else {
				Application.Current.MainWindow.WindowStyle = _winStyle;
				Application.Current.MainWindow.WindowState = _winState;
			}
#else
			Application.Current.Host.Content.IsFullScreen = IsFullscreen;
#endif
			if(IsFullscreenChanged != null) {
				IsFullscreenChanged(this, new IsFullscreenChangedEventArgs(e));
			}
		}
		protected virtual void OnMediaEnded(object sender, RoutedEventArgs e) {
			if(SelectedIndex < (MediaList.Count - 1)) {
				Next();
			} else {
				Stop();
			}
		}
		protected virtual void OnMediaFailed(object sender, ExceptionRoutedEventArgs e) {
			Information = "Failed: " + e.ErrorException.Message;
		}
		protected virtual void OnMediaListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SelectedIndex = -1;
		}
		protected virtual void OnMediaListCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(e.Action == NotifyCollectionChangedAction.Reset) {
				needIndex = -1;
				needItem = null;
				SelectedIndex = -1;
			} else if(needIndex < MediaList.Count && needIndex > -1) {
				SelectedIndex = needIndex;
			} else if(MediaList.Contains(needItem)) {
				SelectedItem = needItem;
			}
		}
		protected virtual void OnMediaOpened(object sender, RoutedEventArgs e) {
			InitializeMediaMarkers();
		}
		protected virtual void OnSelectedIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			int idx = (int)e.NewValue;
			if(MediaList.Count <= idx || idx < -1) {
				SelectedIndex = (int)e.OldValue;
				needIndex = (int)e.NewValue;
				SelectedItem = null;
			} else {
				SelectedItem = IsIndexInRange(idx) ? MediaList.ElementAt((int)e.NewValue) : null;
				if(SelectedIndexChanged != null) {
					SelectedIndexChanged(this, new SelectedIndexChangedEventArgs((int)e.OldValue, (int)e.NewValue));
				}
			}
		}
		protected virtual void OnSelectedItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MediaItem item = (MediaItem)e.NewValue;
			if(MediaList.Contains(item)) {
				SelectedIndex = MediaList.IndexOf((MediaItem)e.NewValue);
				UpdateSource();
				if(SelectedItemChanged != null)
					SelectedItemChanged(this, new SelectedItemChangedEventArgs((MediaItem)e.OldValue));
			} else {
				needItem = (MediaItem)e.NewValue;
				SelectedItem = MediaList.Contains((MediaItem)e.OldValue) ? (MediaItem)e.OldValue : null;
			}
		}
		protected virtual void OnCurrentStateChanged() {
			if(CurrentState == MediaManagerState.Playing || CurrentState == MediaManagerState.Paused)
				timer.Start();
			else
				timer.Stop();
			if(CurrentStateChanged != null)
				CurrentStateChanged(this, new CurrentStateChangedEventArgs(_currentState));
		}
		public virtual void Pause() {
			if((MediaElement != null) && (MediaElement.Source != null)) {
				MediaElement.Pause();
				CurrentState = MediaManagerState.Paused;
			}
		}
		public virtual void Play() {
			if((MediaElement != null) && (MediaElement.Source != null)) {
				MediaElement.Play();
				CurrentState = MediaManagerState.Playing;
			}
		}
		public virtual void Stop() {
			if((MediaElement != null) && (MediaElement.Source != null)) {
				MediaElement.Stop();
				CurrentState = MediaManagerState.Stopped;
			}
			MediaElement.Position = TimeSpan.Zero;
		}
		public virtual void Next() {
			if(IsRandom)
				SelectedIndex = GetRandomIndex();
			else
				SelectedIndex++;
		}
		public virtual void Previous() {
			if(IsRandom)
				SelectedIndex = GetRandomIndex();
			else
				SelectedIndex--;
		}
		#region Properties
#if WPF
		public virtual void Close() {
			if((MediaElement != null) && (MediaElement.Source != null)) {
				MediaElement.Close();
				CurrentState = MediaManagerState.Closed;
			}
		}
		public bool IsBuffering { get { return MediaElement.IsBuffering; } }
		public double SpeedRatio {
			get { return MediaElement.SpeedRatio; }
			set { MediaElement.SpeedRatio = value; }
		}
		public bool ScrubingEnabled { get { return MediaElement.ScrubbingEnabled; } set { MediaElement.ScrubbingEnabled = value; } }
#else
		public bool AutoPlay { get { return MediaElement.AutoPlay; } set { MediaElement.AutoPlay = value; } }
#endif
		public double Balance {
			get { return MediaElement.Balance; }
			set { MediaElement.Balance = value; }
		}
		public UIElement BottomPanel {
			get { return (UIElement)GetValue(BottomPanelProperty); }
			set { SetValue(BottomPanelProperty, value); }
		}
		public double BufferingProgress { get { return MediaElement.BufferingProgress; } }
		public bool CanPause { get { return MediaElement.CanPause; } }
		public MediaManagerState CurrentState {
			get { return _currentState; }
			protected internal set {
				if(value != _currentState) {
					_currentState = value;
					OnCurrentStateChanged();
				}
			}
		}
		public string Information {
			get { return (string)GetValue(InformationProperty); }
			set { SetValue(InformationProperty, value); }
		}
		public bool IsFullscreen {
			get { return (bool)GetValue(IsFullscreenProperty); }
			set { SetValue(IsFullscreenProperty, value); }
		}
		public bool IsMuted {
			get { return MediaElement.IsMuted; }
			set { MediaElement.IsMuted = value; }
		}
		public object LeftPanel {
			get { return GetValue(LeftPanelProperty); }
			set { SetValue(LeftPanelProperty, value); }
		}
		public ObservableCollection<MediaItem> MediaList {
			get { return (ObservableCollection<MediaItem>)GetValue(MediaListProperty); }
			set { SetValue(MediaListProperty, value); }
		}
		public TimeSpan Position {
			get { return MediaElement.Position; }
			set { MediaElement.Position = value; }
		}
		public object RightPanel {
			get { return GetValue(RightPanelProperty); }
			set { SetValue(RightPanelProperty, value); }
		}
		public int SelectedIndex {
			get { return (int)GetValue(SelectedIndexProperty); }
			set { SetValue(SelectedIndexProperty, value); }
		}
		public MediaItem SelectedItem {
			get { return (MediaItem)GetValue(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}
		public bool ShowControlsOnVideo {
			get { return _showControlsOnVideo; }
			set {
				if(_showControlsOnVideo != value) {
					_showControlsOnVideo = value;
					SetPositionForMediaElement();
				}
			}
		}
		public object TopPanel {
			get { return GetValue(TopPanelProperty); }
			set { SetValue(TopPanelProperty, value); }
		}
		public double Volume {
			get { return MediaElement.Volume; }
			set { MediaElement.Volume = value; }
		}
		public bool IsRandom {
			get { return (bool)GetValue(IsRandomProperty); }
			set { SetValue(IsRandomProperty, value); }
		}
		public PositionInfoType PositionInfoType {
			get { return (PositionInfoType)GetValue(PositionInfoTypeProperty); }
			set { SetValue(PositionInfoTypeProperty, value); }
		}
		public string PositionInfoText {
			get { return (string)GetValue(PositionInfoTextProperty); }
			set { SetValue(PositionInfoTextProperty, value); }
		}
		protected internal MediaElement MediaElement {
			get { return (MediaElement)GetValue(MediaElementProperty); }
			set { SetValue(MediaElementProperty, value); }
		}
		#endregion
		void Initialize() {
			MediaList = new ObservableCollection<MediaItem>();
			Background = new SolidColorBrush(Colors.Black);
			needIndex = -1;
			needItem = null;
			ShowControlsOnVideo = true;
			timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(0.5);
			timer.Tick += OnTimerTick;
			InitializeMediaElement();
		}
		void OnTimerTick(object sender, EventArgs e) {
			UpdatePositionInfoText();
		}
		void UpdatePositionInfoText() {
			string result = string.Empty;
			string position = TimeSpan.FromSeconds(0.0).ToString();
			string duration = TimeSpan.FromSeconds(0.0).ToString();
			string durationLeft = TimeSpan.FromSeconds(0.0).ToString();
			string format = "{0:HH:mm:ss}";
			position = MediaElement.Position.ToString();
			if(position.Contains('.'))
				position = position.Remove(position.LastIndexOf('.'));
			if(MediaElement.NaturalDuration.HasTimeSpan) {
				duration = string.Format(format, new DateTime(MediaElement.NaturalDuration.TimeSpan.Ticks));
				TimeSpan span = MediaElement.NaturalDuration.TimeSpan - MediaElement.Position;
				durationLeft = string.Format("{0:00}:{1:00}:{2:00}", span.Hours, span.Minutes, span.Seconds);
			}
			switch(PositionInfoType) {
				case PositionInfoType.PositionDuration:
					result = position + " / " + duration;
					break;
				case PositionInfoType.Position:
					result = position;
					break;
				case PositionInfoType.DurationLeft:
					result = "-" + durationLeft;
					break;
			}
			PositionInfoText = result;
		}
		void InitializeMediaElement() {
			MediaElement = new MediaElement();
			MediaElement.LoadedBehavior = MediaState.Manual;
			MediaElement.UnloadedBehavior = MediaState.Close;
		}
		void InitializeMediaMarkers() {
		}
		bool IsIndexInRange(int index) { return ((index >= 0) && (index < MediaList.Count)); }
		void SetPositionForMediaElement() {
			if(MediaElementContentPresenter != null) {
				if(ShowControlsOnVideo) {
					System.Windows.Controls.Grid.SetRow(MediaElementContentPresenter, 0);
					System.Windows.Controls.Grid.SetColumn(MediaElementContentPresenter, 0);
					System.Windows.Controls.Grid.SetRowSpan(MediaElementContentPresenter, 3);
					System.Windows.Controls.Grid.SetColumnSpan(MediaElementContentPresenter, 3);
				} else {
					System.Windows.Controls.Grid.SetRow(MediaElementContentPresenter, 1);
					System.Windows.Controls.Grid.SetColumn(MediaElementContentPresenter, 1);
					System.Windows.Controls.Grid.SetRowSpan(MediaElementContentPresenter, 1);
					System.Windows.Controls.Grid.SetColumnSpan(MediaElementContentPresenter, 1);
				}
			}
		}
		void SubscribeEvents() {
			MediaList.CollectionChanged += OnMediaListCollectionChanged;
			MediaElement.MediaOpened += OnMediaOpened;
			MediaElement.MediaFailed += OnMediaFailed;
			MediaElement.MediaEnded += OnMediaEnded;
		}
		void UpdateSource() {
			Information = string.Empty;
			if(MediaElement != null) {
				Stop();
#if WPF
				Close();
#endif
				if(SelectedItem != null) {
					MediaElement.Source = SelectedItem.Source;
					Play();
				} else {
					MediaElement.Source = null;
				}
			}
		}
		int GetRandomIndex() {
			if(MediaList.Count <= 1) return 0;
			Random rnd = new Random();
			int idx = rnd.Next(MediaList.Count);
			if(idx == SelectedIndex) idx = GetRandomIndex();
			return idx;
		}
#if WPF
		void SaveWindowSettings() {
			_winState = (Application.Current == null) ? WindowState.Normal : Application.Current.MainWindow.WindowState;
			_winStyle = (Application.Current == null) ? WindowStyle.SingleBorderWindow : Application.Current.MainWindow.WindowStyle;
		}
		WindowState _winState;
		WindowStyle _winStyle;
#endif
		bool _showControlsOnVideo;
		int needIndex;
		MediaItem needItem;
		MediaManagerState _currentState;
		ContentPresenter MediaElementContentPresenter;
		DispatcherTimer timer;
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class MediaItem : DependencyObject {
		#region DependencyProperties
		public static readonly DependencyProperty ChaptersProperty = DependencyProperty.Register("Chapters", typeof(List<MediaChapter>), typeof(MediaItem), new PropertyMetadata(null));
		public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register("Description", typeof(string), typeof(MediaItem), new PropertyMetadata(string.Empty));
		public static readonly DependencyProperty SourceProperty = DependencyProperty.Register("Source", typeof(Uri), typeof(MediaItem), null);
		public static readonly DependencyProperty ThumbnailProperty = DependencyProperty.Register("Thumbnail", typeof(Image), typeof(MediaItem), new PropertyMetadata(null));
		#endregion
		public MediaItem() {
			Chapters = new List<MediaChapter>();
		}
		public List<MediaChapter> Chapters {
			get { return (List<MediaChapter>)GetValue(ChaptersProperty); }
			set { SetValue(ChaptersProperty, value); }
		}
		public string Description {
			get { return (string)GetValue(DescriptionProperty); }
			set { SetValue(DescriptionProperty, value); }
		}
		public Uri Source {
			get { return (Uri)GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public Image Thumbnail {
			get { return (Image)GetValue(ThumbnailProperty); }
			set { SetValue(ThumbnailProperty, value); }
		}
	}
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DXToolboxBrowsable(false)]
	public class MediaChapter : IComparable {
		public TimeSpan Position { get; set; }
		public string Text { get; set; }
		public string Type { get; set; }
		public int CompareTo(object obj) {
			MediaChapter chapter = (MediaChapter)obj;
			return Position.CompareTo(chapter.Position);
		}
	}
}
