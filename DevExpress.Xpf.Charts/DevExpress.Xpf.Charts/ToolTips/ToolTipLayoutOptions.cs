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
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Charts.Native;
namespace DevExpress.Xpf.Charts {
	public class ToolTipOptions : ChartDependencyObject {
		public static readonly DependencyProperty ShowForSeriesProperty = DependencyPropertyManager.Register("ShowForSeries",
			typeof(bool), typeof(ToolTipOptions), new PropertyMetadata(false));
		public static readonly DependencyProperty ShowForPointsProperty = DependencyPropertyManager.Register("ShowForPoints",
			typeof(bool), typeof(ToolTipOptions), new PropertyMetadata(true));
		public static readonly DependencyProperty ToolTipPositionProperty = DependencyPropertyManager.Register("ToolTipPosition",
			typeof(ToolTipPosition), typeof(ToolTipOptions), new PropertyMetadata(null));
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowForSeries {
			get { return (bool)GetValue(ShowForSeriesProperty); }
			set { SetValue(ShowForSeriesProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty
		]
		public bool ShowForPoints {
			get { return (bool)GetValue(ShowForPointsProperty); }
			set { SetValue(ShowForPointsProperty, value); }
		}
		[
		Category(Categories.Behavior),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true)
		]
		public ToolTipPosition ToolTipPosition {
			get { return (ToolTipPosition)GetValue(ToolTipPositionProperty); }
			set { SetValue(ToolTipPositionProperty, value); }
		}
		readonly ToolTipPosition defaultToolTipPosition = new ToolTipMousePosition();
		public ToolTipPosition ActualToolTipPosition {
			get { return ToolTipPosition != null ? ToolTipPosition : defaultToolTipPosition; }
		}
		protected override ChartDependencyObject CreateObject() {
			return new ToolTipOptions();
		}
	}
	public enum ToolTipLocation {
		TopRight = AnnotationLocation.TopRight,
		TopLeft = AnnotationLocation.TopLeft,
		BottomRight = AnnotationLocation.BottomRight,
		BottomLeft = AnnotationLocation.BottomLeft
	}
	public enum DockCorner {
		TopRight = DockCornerCore.TopRight,
		TopLeft = DockCornerCore.TopLeft,
		BottomRight = DockCornerCore.BottomRight,
		BottomLeft = DockCornerCore.BottomLeft,
	}
	public abstract class ToolTipPosition : ChartDependencyObject {
		Point offset;
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public Point Offset {
			get { return offset; }
			set { offset = value; }
		}
	}
	public abstract class ToolTipPositionWithLocation : ToolTipPosition {
		public static readonly DependencyProperty LocationProperty = DependencyPropertyManager.Register("Location",
		typeof(ToolTipLocation), typeof(ToolTipPositionWithLocation), new PropertyMetadata());
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public ToolTipLocation Location {
			get { return (ToolTipLocation)GetValue(LocationProperty); }
			set { SetValue(LocationProperty, value); }
		}
	}
	public class ToolTipMousePosition : ToolTipPositionWithLocation {
		protected override ChartDependencyObject CreateObject() {
			return new ToolTipMousePosition();
		}
	}
	public class ToolTipRelativePosition : ToolTipPositionWithLocation {
		protected override ChartDependencyObject CreateObject() {
			return new ToolTipRelativePosition();
		}
	}
	public class ToolTipFreePosition : ToolTipPosition {
		public static readonly DependencyProperty DockTargetProperty = DependencyPropertyManager.Register("DockTarget", 
			typeof(IDockTarget), typeof(ToolTipFreePosition), new PropertyMetadata(null));
		[Category(Categories.Layout)]
		public IDockTarget DockTarget {
			get { return (IDockTarget)GetValue(DockTargetProperty); }
			set { SetValue(DockTargetProperty, value); }
		}
		DockCorner dockCorner;
		[
		Category(Categories.Layout),
		XtraSerializableProperty
		]
		public DockCorner DockCorner {
			get { return dockCorner; }
			set { dockCorner = value; }
		}
		public ToolTipFreePosition() {
			DockCorner = DockCorner.TopLeft;
		}
		protected override ChartDependencyObject CreateObject() {
			return new ToolTipFreePosition();
		}
	}
}
