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

using System.Text.RegularExpressions;
using DevExpress.Data.XtraReports.ServiceModel.DataContracts;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.DemoBase.Helpers;
using DevExpress.Xpf.DemoBase.Internal;
using DevExpress.Xpf.Editors.Flyout;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
namespace DevExpress.Xpf.DemoBase {
	public interface IGroupedLink {
		string Title { get; }
		string Description { get; }
		bool IsNew { get; }
		bool IsUpdated { get; }
		bool IsHighlighted { get; }
	}
	public interface IGroupedLinks {
		string Header { get; }
		IEnumerable<IGroupedLink> Links { get; }
		ICommand ShowAllCommand { get; }
	}
	public enum GroupedLinksControlLayoutType {
		OneGroupPerColumn, Wrapped
	}
	public class UILink {
		public IGroupedLink link;
		public ContentControl control;
	}
	public class UIGroup {
		public List<UILink> links;
		public IGroupedLinks group;
		public ContentControl header;
		public ContentControl newHeader;
		public ContentControl updatedHeader;
		public ContentControl featuredHeader;
		public ContentControl othersHeader;
		public ContentControl showAllDemos;
	}
	public interface IGroupedLinksControl {
		List<UIGroup> UiGroups { get; }
		List<UIGroup> FilteredUiGroups { get; }
		double MeasureContraintWidth { get; }
		void SetVisibleGroupsCount(int count);
	}
	public class GroupedLinksControl : ItemsControl, IColumnControl, ISupportModuleSelectorScrollViewer {
		internal List<UIGroup> uiGroups;
		internal FrameworkElement flyout;
		FrameworkElement border;
		bool update = false;
		DelayedSearchLogger logger;
		IEnumerable<IGroupedLinks> Groups {
			get { return (IEnumerable<IGroupedLinks>)ItemsSource; }
		}
		public SolidColorBrush ItemForeground {
			get { return (SolidColorBrush)GetValue(ItemForegroundProperty); }
			set { SetValue(ItemForegroundProperty, value); }
		}
		public static readonly DependencyProperty ItemForegroundProperty =
			DependencyProperty.Register("ItemForeground", typeof(SolidColorBrush), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public DataTemplate GroupHeaderTemplate {
			get { return (DataTemplate)GetValue(GroupHeaderTemplateProperty); }
			set { SetValue(GroupHeaderTemplateProperty, value); }
		}
		public static readonly DependencyProperty GroupHeaderTemplateProperty =
			DependencyProperty.Register("GroupHeaderTemplate", typeof(DataTemplate), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public DataTemplate StatusHeaderTemplate {
			get { return (DataTemplate)GetValue(StatusHeaderTemplateProperty); }
			set { SetValue(StatusHeaderTemplateProperty, value); }
		}
		public static readonly DependencyProperty StatusHeaderTemplateProperty =
			DependencyProperty.Register("StatusHeaderTemplate", typeof(DataTemplate), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public DataTemplate ShowAllTemplate {
			get { return (DataTemplate)GetValue(ShowAllTemplateProperty); }
			set { SetValue(ShowAllTemplateProperty, value); }
		}
		public static readonly DependencyProperty ShowAllTemplateProperty =
			DependencyProperty.Register("ShowAllTemplate", typeof(DataTemplate), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public GroupedLinksControlLayoutType LayoutType {
			get { return (GroupedLinksControlLayoutType)GetValue(LayoutTypeProperty); }
			set { SetValue(LayoutTypeProperty, value); }
		}
		public static readonly DependencyProperty LayoutTypeProperty =
			DependencyProperty.Register("LayoutType", typeof(GroupedLinksControlLayoutType), typeof(GroupedLinksControl), new PropertyMetadata(GroupedLinksControlLayoutType.OneGroupPerColumn));
		public DataTemplate LinkTemplate {
			get { return (DataTemplate)GetValue(LinkTemplateProperty); }
			set { SetValue(LinkTemplateProperty, value); }
		}
		public static readonly DependencyProperty LinkTemplateProperty =
			DependencyProperty.Register("LinkTemplate", typeof(DataTemplate), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public GroupedLinksControlPanel Panel {
			get { return (GroupedLinksControlPanel)GetValue(PanelProperty); }
			set { SetValue(PanelProperty, value); }
		}
		public static readonly DependencyProperty PanelProperty =
			DependencyProperty.Register("Panel", typeof(GroupedLinksControlPanel), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public Visibility LeftFlyoutIndicatorVisibility {
			get { return (Visibility)GetValue(LeftFlyoutIndicatorVisibilityProperty); }
			set { SetValue(LeftFlyoutIndicatorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty LeftFlyoutIndicatorVisibilityProperty =
			DependencyProperty.Register("LeftFlyoutIndicatorVisibility", typeof(Visibility), typeof(GroupedLinksControl), new PropertyMetadata(Visibility.Collapsed));
		public Visibility RightFlyoutIndicatorVisibility {
			get { return (Visibility)GetValue(RightFlyoutIndicatorVisibilityProperty); }
			set { SetValue(RightFlyoutIndicatorVisibilityProperty, value); }
		}
		public static readonly DependencyProperty RightFlyoutIndicatorVisibilityProperty =
			DependencyProperty.Register("RightFlyoutIndicatorVisibility", typeof(Visibility), typeof(GroupedLinksControl), new PropertyMetadata(Visibility.Collapsed));
		public string FilterString {
			get { return (string)GetValue(FilterStringProperty); }
			set { SetValue(FilterStringProperty, value); }
		}
		public static readonly DependencyProperty FilterStringProperty =
			DependencyProperty.Register("FilterString", typeof(string), typeof(GroupedLinksControl),
				new PropertyMetadata(string.Empty, (d, e) => ((GroupedLinksControl)d).OnFilterStringChanged()));
		public int VisibleGroupsCount {
			get { return (int)GetValue(VisibleGroupsCountProperty); }
			set { SetValue(VisibleGroupsCountProperty, value); }
		}
		public static readonly DependencyProperty VisibleGroupsCountProperty =
			DependencyProperty.Register("VisibleGroupsCount", typeof(int), typeof(GroupedLinksControl), new PropertyMetadata(1));
		public ICommand LinkSelectedCommand {
			get { return (ICommand)GetValue(LinkSelectedCommandProperty); }
			set { SetValue(LinkSelectedCommandProperty, value); }
		}
		public static readonly DependencyProperty LinkSelectedCommandProperty =
			DependencyProperty.Register("LinkSelectedCommand", typeof(ICommand), typeof(GroupedLinksControl), new PropertyMetadata(null));
		public string SearchId {
			get { return (string)GetValue(SearchIdProperty); }
			set { SetValue(SearchIdProperty, value); }
		}
		public static readonly DependencyProperty SearchIdProperty =
			DependencyProperty.Register("SearchId", typeof(string), typeof(GroupedLinksControl), new PropertyMetadata(null,
				(d, e) => ((GroupedLinksControl)d).OnSearchIdChanged()));
		public int MaxGroupsPerColumn {
			get { return (int)GetValue(MaxGroupsPerColumnProperty); }
			set { SetValue(MaxGroupsPerColumnProperty, value); }
		}
		public static readonly DependencyProperty MaxGroupsPerColumnProperty =
			DependencyProperty.Register("MaxGroupsPerColumn", typeof(int), typeof(GroupedLinksControl), new PropertyMetadata(0));
		public double HColumnSpacing {
			get { return (double)GetValue(HColumnSpacingProperty); }
			set { SetValue(HColumnSpacingProperty, value); }
		}
		public static readonly DependencyProperty HColumnSpacingProperty =
			DependencyProperty.Register("HColumnSpacing", typeof(double), typeof(GroupedLinksControl), new PropertyMetadata(10d));
		public string PlatformName {
			get { return (string)GetValue(PlatformNameProperty); }
			set { SetValue(PlatformNameProperty, value); }
		}
		public static readonly DependencyProperty PlatformNameProperty =
			DependencyProperty.Register("PlatformName", typeof(string), typeof(GroupedLinksControl), new PropertyMetadata("",
				(d, e) => ((GroupedLinksControl)d).OnPlatformNameChanged()));
		void OnPlatformNameChanged() {
			if(!string.IsNullOrEmpty(SearchId)) {
				CreateLogger();
			}
		}
		public event EventHandler FilterStringChanged;
		void OnSearchIdChanged() {
			if(!string.IsNullOrEmpty(PlatformName)) {
				CreateLogger();
			}
		}
		private void CreateLogger() {
			logger = new DelayedSearchLogger(SearchId, PlatformName);
		}
		void OnFilterStringChanged() {
			if(FilterStringChanged != null)
				FilterStringChanged(this, EventArgs.Empty);
			Panel.InvalidateMeasure();
			logger.Do(l => l.Update(FilterString));
		}
		IEnumerable<double> columns;
		public IEnumerable<double> Columns {
			get { return columns; }
			set {
				columns = value;
				if(ColumnsChanged != null)
					ColumnsChanged();
			}
		}
		protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue) {
			if(flyout == null) {
				update = true;
			} else {
				CreateControls();
			}
			base.OnItemsSourceChanged(oldValue, newValue);
		}
		public static void Animate(FrameworkElement child, Point currentPosition, Point newPosition) {
			Point delta = new Point(newPosition.X - currentPosition.X, newPosition.Y - currentPosition.Y);
			var transform = new TranslateTransform();
			child.RenderTransform = transform;
			transform.X = newPosition.X;
			transform.Y = newPosition.Y;
		}
		void CreateControls() {
			uiGroups.Clear();
			var panel = new GroupedLinksControlPanel(uiGroups, this);
			foreach(var group in Groups) {
				uiGroups.Add(new UIGroup {
					group = group,
					header = new ContentControl { Content = group, ContentTemplate = GroupHeaderTemplate, Focusable = false },
					newHeader = new ContentControl { Content = "NEW", ContentTemplate = StatusHeaderTemplate, Focusable = false },
					updatedHeader = new ContentControl { Content = "UPDATED", ContentTemplate = StatusHeaderTemplate, Focusable = false },
					featuredHeader = new ContentControl { Content = "FEATURED", ContentTemplate = StatusHeaderTemplate, Focusable = false },
					othersHeader = new ContentControl { Content = "OTHERS", ContentTemplate = StatusHeaderTemplate, Focusable = false },
					showAllDemos = new ContentControl { Content = group, ContentTemplate = ShowAllTemplate, Focusable = false },
					links = new List<UILink>()
				});
				var groupLocal = group;
				MouseTouchAdapter.SubscribeToPointerUp(uiGroups.Last().header, this, p => groupLocal.ShowAllCommand.Do(c => c.Execute(null)));
				MouseTouchAdapter.SubscribeToPointerUp(uiGroups.Last().showAllDemos, this, p => groupLocal.ShowAllCommand.Do(c => c.Execute(null)));
				foreach(var link in group.Links) {
					var linkControl = new ContentControl { Content = link, ContentTemplate = LinkTemplate, Focusable = false };
					var uiLink = new UILink { control = linkControl, link = link };
					RoutedEventHandler loadedHandler = null;
					loadedHandler = new RoutedEventHandler((s, e) => {
						var c = (ContentControl)s;
						c.Loaded -= loadedHandler;
						var controlLocal = uiLink.control;
						MouseTouchAdapter.SubscribeToPointerUp(uiLink.control, this, a => linkControl_MouseUp(controlLocal));
					});
					linkControl.Loaded += loadedHandler;
					uiGroups.Last().links.Add(uiLink);
					panel.Children.Add(linkControl);
				}
				panel.Children.Add(uiGroups.Last().header);
				panel.Children.Add(uiGroups.Last().newHeader);
				panel.Children.Add(uiGroups.Last().updatedHeader);
				panel.Children.Add(uiGroups.Last().featuredHeader);
				panel.Children.Add(uiGroups.Last().othersHeader);
				panel.Children.Add(uiGroups.Last().showAllDemos);
			}
			Panel = panel;
		}
		void linkControl_MouseUp(ContentControl tb) {
			IGroupedLink link = GetLinkByControl(tb);
			LinkSelectedCommand.Do(c => c.Execute(link));
		}
		Point GetPosition(Visual visual) {
			return visual.TransformToAncestor(this).Transform(new Point());
		}
		IGroupedLink GetLinkByControl(ContentControl tb) {
			return uiGroups.SelectMany(g => g.links).First(l => l.control == tb).link;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			flyout = (FrameworkElement)GetTemplateChild("flyout");
			border = (FrameworkElement)GetTemplateChild("border");
			if(update) {
				CreateControls();
			}
		}
		Point UpdateFlyoutPosition(UIElement anchor) {
			var anchorPos = GetPosition(anchor);
			var flyoutSize = flyout.DesiredSize;
			double visibleAreaRight = VisibleArea.IsEmpty ? ActualWidth : VisibleArea.Right;
			bool onRight = flyoutSize.Width + anchor.DesiredSize.Width + anchorPos.X < visibleAreaRight;
			LeftFlyoutIndicatorVisibility = onRight ? Visibility.Visible : Visibility.Hidden;
			RightFlyoutIndicatorVisibility = onRight ? Visibility.Hidden : Visibility.Visible;
			double left = Math.Round(anchorPos.X + (onRight ? anchor.DesiredSize.Width : -flyoutSize.Width));
			double top = Math.Round(anchorPos.Y - flyoutSize.Height / 2 + anchor.DesiredSize.Height / 2 + 3);
			if(top < 0) {
				LeftFlyoutIndicatorVisibility = Visibility.Hidden;
				RightFlyoutIndicatorVisibility = Visibility.Hidden;
				top = 0;
			} else if(top + flyoutSize.Height > ActualHeight) {
				LeftFlyoutIndicatorVisibility = Visibility.Hidden;
				RightFlyoutIndicatorVisibility = Visibility.Hidden;
				top = ActualHeight - flyout.DesiredSize.Height;
			}
			return new Point(left, top);
		}
		protected override void OnPreviewMouseMove(MouseEventArgs e) {
			var pos = e.GetPosition(this);
			var hitTestResult = VisualTreeHelper.HitTest(border, pos);
			if(hitTestResult == null)
				return;
			var visualHit = hitTestResult.VisualHit;
			if(uiGroups == null)
				return;
			var uiLink = uiGroups.SelectMany(g => g.links).FirstOrDefault(l => l.control.VisualChildren().Any(c => c == visualHit));
			if(uiLink != null && uiLink.link.Description != null) {
				ShowFlyout(uiLink);
			} else {
				HideFlyout();
			}
			base.OnPreviewMouseMove(e);
		}
		internal void ShowFlyout(UILink uiLink) {
			ShowFlyout();
			flyout.DataContext = uiLink.link;
			flyout.UpdateLayout();
			Animate(flyout, new Point(), UpdateFlyoutPosition(uiLink.control));
		}
		public GroupedLinksControl() {
			VisibleArea = Rect.Empty;
			uiGroups = new List<UIGroup>();
			Columns = Enumerable.Empty<double>();
		}
		static GroupedLinksControl() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(GroupedLinksControl), new FrameworkPropertyMetadata(typeof(GroupedLinksControl)));
		}
		public Rect VisibleArea { get; set; }
		Size measureConstraint;
		public Size MeasureConstraint {
			get { return measureConstraint; }
			set {
				measureConstraint = value;
				Panel.Do(p => p.InvalidateMeasure());
			}
		}
		public void OnAnimationStarted() {
			HideFlyout();
		}
		public void ShowFlyout() {
			flyout.Opacity = 1;
			flyout.Visibility = Visibility.Visible;
		}
		public void HideFlyout() {
			flyout.Opacity = 0;
			flyout.Visibility = Visibility.Collapsed;
		}
		public event Action ColumnsChanged;
	}
}
