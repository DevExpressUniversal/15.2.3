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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Charts.Native;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Charts {
	[NonCategorized]
	public class ScrollBarItem : NotifyPropertyChangedObject, ILayoutElement {
		ScrollBarOptions options;
		Axis2DElementLayout layout;
		bool visible;
		Orientation orientation;
		double minPosition = 0;
		double maxPosition = 1;
		ICommand command;
		public bool Visible {
			get { return visible; }
			set { 
				visible =value;
				OnPropertyChanged("Visible");
			}
		}
		public Orientation Orientation {
			get { return orientation; }
			set {
				orientation = value;
				OnPropertyChanged("Orientation");
			}
		}
		public double MinPosition {
			get { return minPosition; }
			set {
				minPosition = value;
				OnPropertyChanged("MinPosition");
			}
		}
		public double MaxPosition {
			get { return maxPosition; }
			set {
				maxPosition = value;
				OnPropertyChanged("MaxPosition");
			}
		}
		public ICommand Command {
			get { return command; }
			set {
				command = value;
				OnPropertyChanged("Command");
			}
		}
		internal ScrollBarOptions Options { 
			get { return options; } 
			set { 
				options = value; 
				Visible = options.Visible;
			} 
		}
		internal Axis2DElementLayout Layout { get { return layout; } }
		AxisPosition Position {
			get {
				return Orientation == Orientation.Horizontal ? (options.Alignment == ScrollBarAlignment.Near ? AxisPosition.Bottom : AxisPosition.Top) :
															   (options.Alignment == ScrollBarAlignment.Near ? AxisPosition.Left : AxisPosition.Right);
			}
		}
		ILayout ILayoutElement.Layout {  get { return layout; } }
		internal ScrollBarItem() {
		}
		internal void CreateScrollBarLayout(IDictionary<AxisPosition, Axis2DItem> firstAxesItems, Rect viewport) {
			layout = null;
			if (options.Visible) {
				AxisPosition scrollBarPosition = Position;
				double barThickness = options.BarThickness;
				Rect scrollBarRect;
				switch (scrollBarPosition) {
					case AxisPosition.Left:
						scrollBarRect = new Rect(viewport.Left - barThickness + 1, viewport.Top, barThickness, viewport.Height);
						break;
					 case AxisPosition.Top:
						scrollBarRect = new Rect(viewport.Left, viewport.Top - barThickness + 1, viewport.Width, barThickness);
						break;
					 case AxisPosition.Right:
						scrollBarRect = new Rect(viewport.Right - 1, viewport.Top, barThickness, viewport.Height);
						break;
					case AxisPosition.Bottom:
						scrollBarRect = new Rect(viewport.Left, viewport.Bottom - 1, viewport.Width, barThickness);
						break;
					default:
						return;
				}
				layout = new Axis2DElementLayout(scrollBarRect);
				Axis2DItem axisItem;
				if (firstAxesItems.TryGetValue(scrollBarPosition, out axisItem) && axisItem != null)
					axisItem.SetScrollBarLayout(layout);
			}
		}
	}
}
