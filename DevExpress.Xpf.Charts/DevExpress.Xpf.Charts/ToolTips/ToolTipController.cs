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
using System.Windows;
using System.Windows.Threading;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Charts.Native {
	public enum ToolTipNavigationAction {
		MouseDown,
		MouseUp,
		MouseFreeMove,
		MouseDragging,
		MouseDragEnd,
		MouseLeave,
		Zooming,
		Inertia,
		KeyUp,
		KeyDown
	}
}
namespace DevExpress.Xpf.Charts {
	public enum ToolTipOpenMode {
		OnHover,
		OnClick
	}
	[NonCategorized]
	public class ChartToolTipController : ChartDependencyObject {
		public static readonly DependencyProperty InitialDelayProperty = DependencyPropertyManager.Register("InitialDelay",
			typeof(TimeSpan), typeof(ChartToolTipController), new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0, 0, 500)));
		public static readonly DependencyProperty AutoPopDelayProperty = DependencyPropertyManager.Register("AutoPopDelay",
			typeof(TimeSpan), typeof(ChartToolTipController), new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0, 5, 0)));
		public static readonly DependencyProperty OpenModeProperty = DependencyPropertyManager.Register("OpenMode",
			typeof(ToolTipOpenMode), typeof(ChartToolTipController), new FrameworkPropertyMetadata(ToolTipOpenMode.OnHover));
		public static readonly DependencyProperty ShowBeakProperty = DependencyPropertyManager.Register("ShowBeak",
			typeof(bool), typeof(ChartToolTipController), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty CloseOnClickProperty = DependencyPropertyManager.Register("CloseOnClick",
			typeof(bool), typeof(ChartToolTipController), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty ShowShadowProperty = DependencyPropertyManager.Register("ShowShadow",
			typeof(bool), typeof(ChartToolTipController), new FrameworkPropertyMetadata(true));
		public static readonly DependencyProperty ContentMarginProperty = DependencyPropertyManager.Register("ContentMargin",
			typeof(Thickness), typeof(ChartToolTipController), new FrameworkPropertyMetadata(new Thickness(24, 16, 24, 16)));
		[XtraSerializableProperty]
		public TimeSpan InitialDelay {
			get { return (TimeSpan)GetValue(InitialDelayProperty); }
			set { SetValue(InitialDelayProperty, value); }
		}
		[XtraSerializableProperty]
		public TimeSpan AutoPopDelay {
			get { return (TimeSpan)GetValue(AutoPopDelayProperty); }
			set { SetValue(AutoPopDelayProperty, value); }
		}
		[XtraSerializableProperty]
		public ToolTipOpenMode OpenMode {
			get { return (ToolTipOpenMode)GetValue(OpenModeProperty); }
			set { SetValue(OpenModeProperty, value); }
		}
		[XtraSerializableProperty]
		public bool ShowBeak {
			get { return (bool)GetValue(ShowBeakProperty); }
			set { SetValue(ShowBeakProperty, value); }
		}
		[XtraSerializableProperty]
		public bool CloseOnClick {
			get { return (bool)GetValue(CloseOnClickProperty); }
			set { SetValue(CloseOnClickProperty, value); }
		}
		[XtraSerializableProperty]
		public bool ShowShadow {
			get { return (bool)GetValue(ShowShadowProperty); }
			set { SetValue(ShowShadowProperty, value); }
		}
		[XtraSerializableProperty]
		public Thickness ContentMargin {
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public event ToolTipOpeningEventHandler ToolTipOpening;
		public event ToolTipClosingEventHandler ToolTipClosing;
		ChartControl chart;
		Point mousePosition;
		DispatcherTimer initialTimer = new DispatcherTimer();
		DispatcherTimer popTimer = new DispatcherTimer();
		object tooltipObject;
		object actualHint;
		ToolTipHitInfo hitInfo;
		bool isClosed = true;
		bool actualShowBeak;
		bool toolTipUpdateLocked = false;
		string actualPattern;
		DataTemplate actualTemplate;
		ToolTipNavigationAction actualState = ToolTipNavigationAction.MouseFreeMove;
		bool InSeriesPoint { get { return hitInfo.SeriesPoint != null; } }
		bool InSeries { get { return hitInfo.Series != null || hitInfo.SeriesPoint != null; } }
		Object NewToolTipObject { get { return InSeriesPoint ? hitInfo.RefinedPoint as Object : hitInfo.Series as Object; } }
		ToolTipPosition ActualToolTipPosition { get { return chart.ActualToolTipOptions.ActualToolTipPosition; } }
		public ChartToolTipController() {
			initialTimer.Tick += new EventHandler(initialTimer_Tick);
			popTimer.Tick += new EventHandler(popTimer_Tick);
		}
		void initialTimer_Tick(object sender, EventArgs e) {
			ShowToolTip();
		}
		void popTimer_Tick(object sender, EventArgs e) {
			if (OpenMode == ToolTipOpenMode.OnHover)
				CloseToolTip();
			else
				HideToolTip();
		}
		string GetToolTipText(string pattern) {
			if (InSeriesPoint)
				return PatternHelper.GetPointTextByFormatString(pattern, hitInfo.Series, hitInfo.RefinedPoint);
			else
				return PatternHelper.GetSeriesTextByFormatString(pattern, hitInfo.Series);
		}
		void FillPresentationData(ToolTipPresentationData presentationData, string text, object hint) {
			presentationData.ToolTipText = text;
			presentationData.Hint = hint;
			presentationData.ChartElement = InSeriesPoint ? (object)hitInfo.SeriesPoint : (object)hitInfo.Series;
		}
		ToolTipItem CreateToolTipItem() {
			if (hitInfo.Series == null || hitInfo.Series.Diagram == null)
				return null; 
			ToolTipItem item = new ToolTipItem();
			item.Position = mousePosition;
			item.ContentMargin = ContentMargin;
			actualShowBeak = ShowBeak;
			actualHint = InSeriesPoint ? hitInfo.SeriesPoint.ToolTipHint : hitInfo.Series.ToolTipHint;
			actualTemplate = InSeriesPoint ? hitInfo.Series.ToolTipPointTemplate : hitInfo.Series.ToolTipSeriesTemplate;
			actualPattern = InSeriesPoint ? hitInfo.Series.ActualToolTipPointPattern : actualPattern = hitInfo.Series.ToolTipSeriesPattern;
			ChartToolTipEventArgs toolTipOpeningEventArgs = RaiseOpeningEvent();
			if (toolTipOpeningEventArgs != null) {
				if (toolTipOpeningEventArgs.Cancel)
					return null;
				actualHint = toolTipOpeningEventArgs.Hint;
				actualTemplate = toolTipOpeningEventArgs.Template;
				actualPattern = toolTipOpeningEventArgs.Pattern;
				actualShowBeak = toolTipOpeningEventArgs.ShowBeak;
			}
			item.ContentTemplate = actualTemplate;
			string toolTipText = GetToolTipText(actualPattern);
			bool templateChanged = InSeriesPoint ? hitInfo.Series.ToolTipPointTemplateChanged : hitInfo.Series.ToolTipTemplateChanged;
			if (actualHint == null && String.IsNullOrEmpty(toolTipText) && !templateChanged)
				return null;
			FillPresentationData(item.PresentationData, toolTipText, actualHint);
			item.ToolTipPosition = ActualToolTipPosition;
			if (ActualToolTipPosition is ToolTipRelativePosition)
				item.Position = hitInfo.ToolTipPoint;
			actualShowBeak = actualShowBeak && !(ActualToolTipPosition is ToolTipFreePosition);
			item.BeakVisibility = actualShowBeak ? Visibility.Visible : Visibility.Collapsed;
			item.ShowShadow = (ActualToolTipPosition is ToolTipFreePosition) ? false : ShowShadow;
			return item;
		}
		void ShowToolTip() {
			initialTimer.Stop();
			if (InSeries) {
				chart.ToolTipItem = CreateToolTipItem();
				if (chart.ToolTipItem != null) {
					isClosed = false;
					if (AutoPopDelay.TotalMilliseconds != 0) {
						popTimer.Interval = AutoPopDelay;
						popTimer.Start();
					}
				}
			}
		}
		void CreateToolTip() {
			bool showToolTip = (hitInfo.SeriesPoint != null) ? chart.PointsToolTipEnabled : chart.SeriesToolTipEnabled;
			if (hitInfo != null && InSeries && showToolTip) {
				if (!NewToolTipObject.Equals(tooltipObject)) {
					CloseToolTip();
					initialTimer.Interval = InitialDelay;
					tooltipObject = NewToolTipObject;
					if (initialTimer.Interval.TotalMilliseconds != 0)
						initialTimer.Start();
					else
						ShowToolTip();
				}
			}
			else
				HideToolTip();
		}
		void CloseToolTip() {
			initialTimer.Stop();
			popTimer.Stop();
			chart.ToolTipItem = null;
			if (!isClosed) {
				isClosed = true;
				RaiseClosingEvent();
			}
		}
		void HideToolTip() {
			tooltipObject = null;
			CloseToolTip();
		}
		void UpdateLockedState(ToolTipNavigationAction action) {
			if (action == ToolTipNavigationAction.MouseDown || action == ToolTipNavigationAction.MouseDragging)
				toolTipUpdateLocked = true;
			if (action == ToolTipNavigationAction.MouseUp || action == ToolTipNavigationAction.MouseDragEnd || action == ToolTipNavigationAction.MouseLeave)
				toolTipUpdateLocked = false;
		}
		void ProcessToolTipOnHover(ToolTipNavigationAction navigationAction) {
			switch (navigationAction) {
			case ToolTipNavigationAction.MouseDown:
				if (CloseOnClick)
					CloseToolTip();
				break;
			case ToolTipNavigationAction.Zooming:
			case ToolTipNavigationAction.KeyDown:
				CloseToolTip();
				break;
			case ToolTipNavigationAction.MouseDragging:
			case ToolTipNavigationAction.Inertia:
				HideToolTip();
				break;
			case ToolTipNavigationAction.MouseUp:
			case ToolTipNavigationAction.MouseFreeMove:
				if (actualState != ToolTipNavigationAction.KeyDown && !toolTipUpdateLocked)
					CreateToolTip();
				break;
			case ToolTipNavigationAction.MouseDragEnd:
				tooltipObject = null;
				CreateToolTip();
				break;
			case ToolTipNavigationAction.KeyUp:
				CreateToolTip();
				break;
			case ToolTipNavigationAction.MouseLeave:
				HideToolTip();
				break;
			}
			actualState = navigationAction;
		}
		void ProcessToolTipOnClick(ToolTipNavigationAction navigationAction) {
			switch (navigationAction) {
			case ToolTipNavigationAction.MouseUp:
				CreateToolTip();
				break;
			case ToolTipNavigationAction.MouseFreeMove:
				if (!toolTipUpdateLocked) {
					if (hitInfo != null && InSeries) {
						if (!NewToolTipObject.Equals(tooltipObject))
							CloseToolTip();
					}
					else
						HideToolTip();
				}
				break;
			case ToolTipNavigationAction.MouseDragging:
			case ToolTipNavigationAction.Zooming:
			case ToolTipNavigationAction.KeyDown:
				HideToolTip();
				break;
			case ToolTipNavigationAction.MouseDown:
				if (CloseOnClick)
					if (isClosed)
						HideToolTip();
					else
						CloseToolTip();
				break;
			case ToolTipNavigationAction.MouseLeave:
				HideToolTip();
				break;
			}
			actualState = navigationAction;
		}
		internal void UpdateToolTip(Point position, ChartControl chart, ToolTipNavigationAction navigationAction) {
			if (chart.SeriesToolTipEnabled || chart.PointsToolTipEnabled)
				UpdateToolTip(position, chart, navigationAction, false);
		}
		internal void UpdateToolTip(Point position, ChartControl chart, ToolTipNavigationAction navigationAction, bool touchUses) {
			if (chart.SeriesToolTipEnabled || chart.PointsToolTipEnabled)
				UpdateToolTip(position, chart, navigationAction, new ToolTipHitInfo(chart, position, chart.SeriesToolTipEnabled, touchUses));
		}
		internal void UpdateToolTip(Point position, ChartControl chart, ToolTipNavigationAction navigationAction, ToolTipHitInfo hitInfo) {
			if (chart != null) {
				this.chart = chart;
				mousePosition = position;
				if (chart.SeriesToolTipEnabled || chart.PointsToolTipEnabled) {
					this.hitInfo = hitInfo;
					if (hitInfo.Series != null && hitInfo.Series.ActualToolTipEnabled || hitInfo.Series == null) {
						UpdateLockedState(navigationAction);
						if (OpenMode == ToolTipOpenMode.OnClick)
							ProcessToolTipOnClick(navigationAction);
						else
							ProcessToolTipOnHover(navigationAction);
					}
					else
						initialTimer.Stop();
				}
			}
		}
		protected internal ChartToolTipEventArgs RaiseOpeningEvent() {
			if (ToolTipOpening != null) {
				ChartToolTipEventArgs toolTipOpeningEventArgs = new ChartToolTipEventArgs(actualHint, actualShowBeak, actualPattern, actualTemplate, hitInfo.Series, hitInfo.SeriesPoint, chart);
				ToolTipOpening(this, toolTipOpeningEventArgs);
				return toolTipOpeningEventArgs;
			}
			else
				return null;
		}
		protected internal void RaiseClosingEvent() {
			if (ToolTipClosing != null)
				ToolTipClosing(this, new ChartToolTipEventArgs(actualHint, actualShowBeak, actualPattern, actualTemplate, hitInfo.Series, hitInfo.SeriesPoint, chart));
		}
		protected override ChartDependencyObject CreateObject() {
			return new ChartToolTipController();
		}
	}
}
