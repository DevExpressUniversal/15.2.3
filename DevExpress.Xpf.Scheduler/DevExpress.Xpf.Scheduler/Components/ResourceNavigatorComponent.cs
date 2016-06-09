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
using DevExpress.Xpf.Scheduler.Drawing;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core.Native;
namespace DevExpress.Xpf.Scheduler.Drawing.Components {
	public class ResourceNavigatorComponent : VisualComponent<ResourceNavigatorControl> {
		ResourceNavigatorControl resourceNavigator;
		SchedulerControl scheduler;
		public ResourceNavigatorComponent(ILayoutPanel panel, IVisualComponent owner)
			: base(panel, owner) {
		}
		public SchedulerControl Scheduler {
			get { return scheduler; }
			set {
				if (scheduler == value)
					return;
				scheduler = value;
				if (ResourceNavigator != null)
					ResourceNavigator.SchedulerControl = scheduler;
			}
		}
		IResourceNavigatorComponentProperties Properties { get { return Panel.GetProperties<IResourceNavigatorComponentProperties>(); } }
		protected Style ResourceNavigatorStyle { get { return Properties.ResourceNavigatorStyle; } }
		protected ResourceNavigatorControl ResourceNavigator {
			get { return resourceNavigator; }
			set { resourceNavigator = value; }
		}
		public override void Initialize() {
			base.Initialize();
			CreateResourceNavigator();
		}
		protected override void SubscribeToEvents() {
			base.SubscribeToEvents();
			PropertyChangedSubscriber.AddHandler(Properties, OnPropertyChanged);
		}
		protected override void UnsubscribeFromEvents() {
			base.UnsubscribeFromEvents();
			PropertyChangedSubscriber.RemoveHandler(Properties, OnPropertyChanged);
		}
		void OnPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e) {
			if (e.PropertyName == "ResourceNavigatorStyle")
				ResourceNavigator.Style = ResourceNavigatorStyle;
			RaiseComponentChanged();
		}
		protected override System.Windows.Size MeasureCore(System.Windows.Size availableSize) {
			ResourceNavigator.Measure(availableSize);
			return ResourceNavigator.DesiredSize;
		}
		protected override System.Windows.Size ArrangeCore(System.Windows.Rect arrangeBounds) {
			ResourceNavigator.Arrange(arrangeBounds);
			return arrangeBounds.Size();
		}
		void CreateResourceNavigator() {
			ResourceNavigator = new ResourceNavigatorControl();
			VisualItemsAccessor.Add(ResourceNavigator);
			ResourceNavigator.SchedulerControl = Scheduler;
			ResourceNavigator.Style = ResourceNavigatorStyle;
		}
		protected override IVisualElementAccessor<ResourceNavigatorControl> CreateVisualItemsAccessor(ILayoutPanel panel) {
			return new PanelChildrenAccessor<ResourceNavigatorControl>(panel, ZIndex);
		}
	}
}
