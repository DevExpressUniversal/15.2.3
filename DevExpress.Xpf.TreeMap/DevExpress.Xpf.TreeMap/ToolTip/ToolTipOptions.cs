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
using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.TreeMap {
	public enum ToolTipOpenMode { OnHover, OnClick }
	public class ToolTipOptions : TreeMapDependencyObject {
		public static readonly DependencyProperty OpenModeProperty = DependencyProperty.Register("OpenMode",
			typeof(ToolTipOpenMode), typeof(ToolTipOptions), new FrameworkPropertyMetadata(ToolTipOpenMode.OnClick));
		public static readonly DependencyProperty PositionProperty = DependencyProperty.Register("Position",
			typeof(ToolTipPosition), typeof(ToolTipOptions), new FrameworkPropertyMetadata(null, PositionPropertyChanged));
		public static readonly DependencyProperty InitialDelayProperty = DependencyProperty.Register("InitialDelay",
			typeof(TimeSpan), typeof(ToolTipOptions), new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0, 0, 500)));
		public static readonly DependencyProperty AutoPopDelayProperty = DependencyProperty.Register("AutoPopDelay",
			typeof(TimeSpan), typeof(ToolTipOptions), new FrameworkPropertyMetadata(new TimeSpan(0, 0, 0, 5, 0)));
		public static readonly DependencyProperty CloseOnClickProperty = DependencyProperty.Register("CloseOnClick",
			typeof(bool), typeof(ToolTipOptions), new FrameworkPropertyMetadata(true));
		static void PositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ToolTipOptions options = d as ToolTipOptions;
			if (options != null)
				options.actualPosition = e.NewValue != null ? (ToolTipPosition)e.NewValue : new ToolTipMousePosition();
		}
		[Category("Behavior")]
		public ToolTipOpenMode OpenMode {
			get { return (ToolTipOpenMode)GetValue(OpenModeProperty); }
			set { SetValue(OpenModeProperty, value); }
		}
		[Category("Layout")]
		public ToolTipPosition Position {
			get { return (ToolTipPosition)GetValue(PositionProperty); }
			set { SetValue(PositionProperty, value); }
		}
		[Category("Behavior")]
		public TimeSpan InitialDelay {
			get { return (TimeSpan)GetValue(InitialDelayProperty); }
			set { SetValue(InitialDelayProperty, value); }
		}
		[Category("Behavior")]
		public TimeSpan AutoPopDelay {
			get { return (TimeSpan)GetValue(AutoPopDelayProperty); }
			set { SetValue(AutoPopDelayProperty, value); }
		}
		[Category("Behavior")]
		public bool CloseOnClick {
			get { return (bool)GetValue(CloseOnClickProperty); }
			set { SetValue(CloseOnClickProperty, value); }
		}
		ToolTipPosition actualPosition;
		internal ToolTipPosition ActualPosition { get { return actualPosition; } }
		public ToolTipOptions() {
			this.actualPosition = new ToolTipMousePosition();
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new ToolTipOptions();
		}
	}
	public abstract class ToolTipPosition : TreeMapDependencyObject {
		protected internal abstract Point CalclulateToolTipPoint(Point mousePosition, TreeMapItemPresentation itemPresentation, TreeMapControl target);
	}
	public class ToolTipMousePosition : ToolTipPosition {
		protected override TreeMapDependencyObject CreateObject() {
			return new ToolTipMousePosition();
		}
		protected internal override Point CalclulateToolTipPoint(Point mousePosition, TreeMapItemPresentation itemPresentation, TreeMapControl target) {
			return mousePosition;
		}
	}
	public class ToolTipRelativePosition : ToolTipPosition {
		public static readonly DependencyProperty OriginProperty = DependencyProperty.Register("Origin",
			typeof(Point), typeof(ToolTipRelativePosition), new FrameworkPropertyMetadata(new Point(0.5, 0.5)), new ValidateValueCallback(ValidateOrigin));
		static bool ValidateOrigin(object value) {
			Point origin = (Point)value;
			return origin.X <= 1.0 && origin.X >= 0.0 && origin.Y <= 1.0 && origin.Y >= 0.0;
		}
		[Category("Layout")]
		public Point Origin {
			get { return (Point)GetValue(OriginProperty); }
			set { SetValue(OriginProperty, value); }
		}
		protected override TreeMapDependencyObject CreateObject() {
			return new ToolTipRelativePosition();
		}
		protected internal override Point CalclulateToolTipPoint(Point mousePosition, TreeMapItemPresentation itemPresentation, TreeMapControl target) {
			Rect itemRect = LayoutHelper.GetRelativeElementRect(itemPresentation, target);
			return new Point(itemRect.Left + itemRect.Width * Origin.X, itemRect.Top + itemRect.Height * Origin.Y);
		}
	}
}
