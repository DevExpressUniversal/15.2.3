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
using System.Windows.Input;
using DevExpress.Map.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Map {
	[
	TemplatePart(Name = "PART_ZoomInButton", Type = typeof(UIElement)),
	TemplatePart(Name = "PART_ZoomOutButton", Type = typeof(UIElement)),
	TemplatePart(Name = "PART_TrackBar", Type = typeof(UIElement)),
	TemplatePart(Name = "PART_Thumb", Type = typeof(UIElement)),
	NonCategorized
	]
	public abstract class MapZoomTrackbar : Control {
		const double defaultMaxPosition = 120.0;
		public static readonly DependencyProperty PositionProperty = DependencyPropertyManager.Register("Position",
			typeof(double), typeof(MapZoomTrackbar), new PropertyMetadata(0.0));
		public static readonly DependencyProperty ZoomLevelProperty = DependencyPropertyManager.Register("ZoomLevel",
			typeof(double), typeof(MapZoomTrackbar), new PropertyMetadata(1.0, ZoomLevelPropertyChanged));
		public static readonly DependencyProperty MinZoomLevelProperty = DependencyPropertyManager.Register("MinZoomLevel",
			typeof(int), typeof(MapZoomTrackbar), new PropertyMetadata(1));
		public static readonly DependencyProperty MaxZoomLevelProperty = DependencyPropertyManager.Register("MaxZoomLevel",
			typeof(int), typeof(MapZoomTrackbar), new PropertyMetadata(20));
		public static readonly DependencyProperty CommandProperty = DependencyPropertyManager.Register("Command",
			typeof(ICommand), typeof(MapZoomTrackbar), new PropertyMetadata(null));
		public static readonly DependencyProperty ZoomingStepProperty = DependencyPropertyManager.Register("ZoomingStep",
			typeof(double), typeof(MapZoomTrackbar), new PropertyMetadata(1.0), ZoomStepValidation);
		public double Position {
			get { return (double)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		public double ZoomLevel {
			get { return (double)GetValue(ZoomLevelProperty); }
			set { SetValue(ZoomLevelProperty, value); }
		}
		public int MinZoomLevel {
			get { return (int)GetValue(MinZoomLevelProperty); }
			set { SetValue(MinZoomLevelProperty, value); }
		}
		public int MaxZoomLevel {
			get { return (int)GetValue(MaxZoomLevelProperty); }
			set { SetValue(MaxZoomLevelProperty, value); }
		}
		public ICommand Command {
			get { return (ICommand)GetValue(CommandProperty); }
			set { SetValue(CommandProperty, value); }
		}
		public double ZoomingStep {
			get { return (double)GetValue(ZoomingStepProperty); }
			set { SetValue(ZoomingStepProperty, value); }
		}
		static void ZoomLevelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			MapZoomTrackbar trackbar = d as MapZoomTrackbar;
			if (trackbar != null)
				trackbar.UpdatePosition();
		}
		UIElement zoomInButton;
		UIElement zoomOutButton;
		UIElement trackBar;
		UIElement thumb;
		internal UIElement TrackBar { get { return trackBar; } }
		internal UIElement Thumb { get { return thumb; } }
		protected internal abstract MapZoomTrackbarNavigationController NavigationController { get; }
		protected abstract bool IsVertical { get; }
		void BindEvents() {
			zoomInButton.MouseLeftButtonDown += new MouseButtonEventHandler(NavigationController.MouseZoomInLeftButtonDown);
			zoomOutButton.MouseLeftButtonDown += new MouseButtonEventHandler(NavigationController.MouseZoomOutLeftButtonDown);
			trackBar.MouseLeftButtonDown += new MouseButtonEventHandler(NavigationController.MouseTrackBarLeftButtonDown);
			trackBar.MouseMove += new MouseEventHandler(NavigationController.TrackBarMouseMove);
			trackBar.MouseLeftButtonUp += new MouseButtonEventHandler(NavigationController.TrackBarMouseLeftButtonUp);
			thumb.MouseLeftButtonDown += new MouseButtonEventHandler(NavigationController.MouseTrackBarLeftButtonDown);
			thumb.MouseMove += new MouseEventHandler(NavigationController.TrackBarMouseMove);
			thumb.MouseLeftButtonUp += new MouseButtonEventHandler(NavigationController.TrackBarMouseLeftButtonUp);
			this.LayoutUpdated += MapZoomTrackbar_LayoutUpdated;
		}
		void MapZoomTrackbar_LayoutUpdated(object sender, System.EventArgs e) {
			UpdatePosition();
		}
		void UpdatePosition() {
			Position = (double)(ZoomLevel - MinZoomLevel) / (double)(MaxZoomLevel - MinZoomLevel) * CalculateMaxPosition();
		}
		double CalculateMaxPosition() {
			if (trackBar != null && thumb != null)
				return IsVertical ? trackBar.RenderSize.Height - thumb.RenderSize.Height : trackBar.RenderSize.Width - thumb.RenderSize.Width;
			else
				return defaultMaxPosition;
		}
		static bool ZoomStepValidation(object value) {
			return (double)value >= 0;
		}
		internal void ExecuteCommand(double zoomLevel) {
			ICommand command = Command;
			if (command != null) {
				object commandParameter = zoomLevel;
				if (command.CanExecute(commandParameter))
					command.Execute(commandParameter);
			}
		}
		internal bool CanExecuteCommand(int zoomLevel) {
			return Command.CanExecute(zoomLevel);
		}
		internal int CalculateZoomLevel(double position) {
			int calculatedLevel = (int)MathUtils.Truncate(position / CalculateMaxPosition() * (double)(MaxZoomLevel - MinZoomLevel) + MinZoomLevel, 0);
			return (int)MathUtils.MinMax(calculatedLevel, MinZoomLevel, MaxZoomLevel);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			zoomInButton = GetTemplateChild("PART_ZoomInButton") as UIElement;
			zoomOutButton = GetTemplateChild("PART_ZoomOutButton") as UIElement;
			trackBar = GetTemplateChild("PART_TrackBar") as UIElement;
			thumb = GetTemplateChild("PART_Thumb") as UIElement;
			BindEvents();
		}
	}
	public class MapVerticalZoomTrackbar : MapZoomTrackbar {
		readonly MapVerticalZoomTrackbarNavigationController navigationController;
		protected internal override MapZoomTrackbarNavigationController NavigationController {
			get { return navigationController; }
		}
		protected override bool IsVertical { get { return true; } }
		public MapVerticalZoomTrackbar() {
			DefaultStyleKey = typeof(MapVerticalZoomTrackbar);
			navigationController = new MapVerticalZoomTrackbarNavigationController(this);
		}
	}
	public class MapHorizontalZoomTrackbar : MapZoomTrackbar {
		readonly MapHorizontalZoomTrackbarNavigationController navigationController;
		protected internal override MapZoomTrackbarNavigationController NavigationController {
			get { return navigationController; }
		}
		protected override bool IsVertical { get { return false; } }
		public MapHorizontalZoomTrackbar() {
			DefaultStyleKey = typeof(MapHorizontalZoomTrackbar);
			navigationController = new MapHorizontalZoomTrackbarNavigationController(this);
		}
	}
	public class MapZoomTrackbarButton : Button {
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			e.Handled = false;
		}
		protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			e.Handled = false;
		}
	}
	public class MapZoomTrackbarLayoutControl : Control {
		public MapZoomTrackbarLayoutControl() {
			DefaultStyleKey = typeof(MapZoomTrackbarLayoutControl);
		}
	}
}
namespace DevExpress.Xpf.Map.Native {
	public class ZoomTrackbarInfo : OverlayInfoBase {
		ICommand command;
		public ICommand Command {
			get { return command; }
			set {
				if (command != value) {
					command = value;
					RaisePropertyChanged("Command");
				}
			}
		}
		public ZoomTrackbarInfo(MapControl map) : base(map) {
		}
		protected internal override Control CreatePresentationControl() {
			return new MapZoomTrackbarLayoutControl();
		}
	}
}
