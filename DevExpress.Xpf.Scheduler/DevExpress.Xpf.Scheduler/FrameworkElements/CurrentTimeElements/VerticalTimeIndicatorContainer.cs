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

using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Internal;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VerticalTimeIndicatorContainer : ContentPresenter, IVisualElement {
		static VerticalTimeIndicatorContainer() {
			DefaultStyleKeyProperty.OverrideMetadata(typeof(VerticalTimeIndicatorContainer), new FrameworkPropertyMetadata(typeof(VerticalTimeIndicatorContainer)));
		}
		public VerticalTimeIndicatorContainer() {
		}
		DateTime Now {
			get {
#if DEBUGTEST
				return (DevExpress.XtraScheduler.Tests.TestEnvironment.Now.HasValue) ? DevExpress.XtraScheduler.Tests.TestEnvironment.Now.Value : DateTime.Now;
#else
				return DateTime.Now;
#endif
			}
		}
		public event EventHandler<QueryPositionByTimeEventArgs> QueryPositionByTime;
		protected override Size ArrangeOverride(Size arrangeSize) {
			UIElement child = GetPresenterChild();
			if (child != null) 
				ArrangeChild(child, arrangeSize);
			return arrangeSize;
		}
		UIElement GetPresenterChild() {
			int childrenCount = VisualTreeHelper.GetChildrenCount(this);
			if (childrenCount <= 0)
				return null;
			return VisualTreeHelper.GetChild(this, 0) as UIElement;
		}
		void ArrangeChild(UIElement child, Size arrangeSize) {
			double position = RaiseQueryPositionByTime();
			if (position < 0)
				child.Visibility = System.Windows.Visibility.Collapsed;
			else
				child.Visibility = System.Windows.Visibility.Visible;			
			Rect bounds = new Rect(position, 0, child.DesiredSize.Width, arrangeSize.Height);
			child.Arrange(bounds);	
		}
		double RaiseQueryPositionByTime() {
			QueryPositionByTimeEventArgs ea = new QueryPositionByTimeEventArgs(Now);
			if (QueryPositionByTime == null)
				return -1;
			QueryPositionByTime(this, ea);
			return ea.Position;
		}
	}	
}
