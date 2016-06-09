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
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Gauges.Native;
namespace DevExpress.Xpf.Gauges {
	public abstract class PresentationBase : GaugeDependencyObject, INamedElement { 
		public abstract string PresentationName { get; }
		string INamedElement.Name { get { return PresentationName; } }
	}
	public abstract class PresentationControl : Control {
		internal Point GetRenderTransformOrigin() {
			int count = VisualTreeHelper.GetChildrenCount(this);
			UIElement child = count > 0 ? VisualTreeHelper.GetChild(this, 0) as UIElement : null;
			return child != null ? child.RenderTransformOrigin : new Point(0, 0);
		}
	}
	public abstract class ValueIndicatorPresentationControl : PresentationControl {
		internal ValueIndicatorBase ValueIndicator {
			get {
				ValueIndicatorInfo indicatorinfo = DataContext as ValueIndicatorInfo;
				if (indicatorinfo != null)
					return indicatorinfo.Indicator;
				else
					return null;
			}
		}
		protected override AutomationPeer OnCreateAutomationPeer() {
			return new ValueIndicatorAutomationPeer(this);
		}
	}
	public class CustomPresentationControl : PresentationControl {
		public CustomPresentationControl() {
			DefaultStyleKey = typeof(CustomPresentationControl);
		}
	}
	public class CustomValueIndicatorPresentationControl : ValueIndicatorPresentationControl {
	}
}
