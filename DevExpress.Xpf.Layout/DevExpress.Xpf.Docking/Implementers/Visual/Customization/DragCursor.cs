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
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	public enum DragCursorType { Item, Panel }
	[TemplatePart(Name = "PART_CaptionControl", Type = typeof(CaptionControl))]
	public class DragCursorControl : psvControl, IControlHost {
		#region static
		public static readonly DependencyProperty CursorTypeProperty;
		static DragCursorControl() {
			var dProp = new DependencyPropertyRegistrator<DragCursorControl>();
			dProp.OverrideDefaultStyleKey(DefaultStyleKeyProperty);
			dProp.Register("CursorType", ref CursorTypeProperty, DragCursorType.Item,
				(dObj, ea)=>((DragCursorControl)dObj).OnCursorTypeChanged((DragCursorType)ea.NewValue));
		}
		#endregion static
		public DragCursorType CursorType {
			get { return (DragCursorType)GetValue(CursorTypeProperty); }
			set { SetValue(CursorTypeProperty, value); }
		}
		public DragCursorControl() {
		}
		protected TemplatedCaptionControl CaptionControl { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			CaptionControl = GetTemplateChild("PART_CaptionControl") as TemplatedCaptionControl;
			UpdateVisualState();
		}
		public FrameworkElement[] GetChildren() {
			return new FrameworkElement[] { CaptionControl };
		}
		void UpdateVisualState() {
			VisualStateManager.GoToState(this, CursorType == DragCursorType.Item ? "CursorTypeItem" : "CursorTypePanel", false);
		}
		protected virtual void OnCursorTypeChanged(DragCursorType type) {
			UpdateVisualState();	
		}
	}
	public class DragCursorFactory {
		public static FloatingContainer CreateDragCursorContainer(DockLayoutManager manager, object content) {
			FloatingContainer container = FloatingContainerFactory.Create(Core.FloatingMode.Window);
			container.BeginUpdate();
			container.Owner = manager;
			container.FloatSize = new Size(150, 150);
			container.ShowActivated = false;
			container.AllowMoving = false;
			container.AllowSizing = false;
			container.ShowContentOnly = true;
			container.Content = content;
			manager.AddToLogicalTree(container, content);
			container.EndUpdate();
			return container;
		}
	}
}
