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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Docking.VisualElements;
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.Platform {
	public enum DockVisualizerElement {
		None,
		Left, Right, Top, Bottom, Center,
		Selection, DockZone,
		HideLeft, HideRight, HideTop, HideBottom,
		RenameHint
	}
	public static class DockHintElementFactory {
		public static DockHintElement Make(DockVisualizerElement type) {
			switch(type) {
				case DockVisualizerElement.Center:
					return new CenterDockHintElement();
				case DockVisualizerElement.Left:
				case DockVisualizerElement.Right:
				case DockVisualizerElement.Top:
				case DockVisualizerElement.Bottom:
					return new SideDockHintElement(type);
				case DockVisualizerElement.DockZone:
					return new RectangleDockHint();
				case DockVisualizerElement.Selection:
					return new SelectionHint();
				case DockVisualizerElement.RenameHint:
					return new RenameHint();
			}
			return null;
		}
	}
	[DXToolboxBrowsable(false)]
	public abstract class DockHintElement : psvControl {
		#region static
		public static readonly DependencyProperty DockTypeProperty;
		static DockHintElement() {
			var dProp = new DependencyPropertyRegistrator<DockHintElement>();
			dProp.RegisterAttachedInherited("DockType", ref DockTypeProperty, DockType.None);
		}
		public static DockType GetDockType(DependencyObject obj) { 
			return (DockType)obj.GetValue(DockTypeProperty); 
		}
		public static void SetDockType(DependencyObject obj, DockType value) { 
			obj.SetValue(DockTypeProperty, value); 
		}
		#endregion
		public DockHintElement(DockVisualizerElement type) {
			Type = type;
		}
		public DockVisualizerElement Type { get; private set; }
		public virtual void UpdateAvailableState(bool dock, bool hide, bool fill) { }
		public virtual void UpdateHotTrack(DockHintButton hotButton) { }
		public virtual void UpdateAvailableState(DockingHintAdorner adorner) { }
		public virtual void UpdateEnabledState(DockingHintAdorner adorner) { }
		public virtual void UpdateState(DockingHintAdorner adorner) { }
		protected void UpdateHotTrack(DockHintButton button, DockHintButton hotButton) {
			if(button != null) button.IsHot = CalcHotTrack(button, hotButton);
		}
		protected bool UpdateVisibility(DockingHintAdorner adorner) {
			bool visible = CalcVisibileState(adorner);
			Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			return visible;
		}
		public void Arrange(DockingHintAdorner adorner, DockingHintAdorner.ElementInfo info) {
			if(UpdateVisibility(adorner)) {
				Rect rect = info.CalcPlacement(CalcBounds(adorner), VisualizerAdornerHelper.GetAdornerWindowIndent(adorner));
				if(!rect.IsEmpty) this.Arrange(rect);
			}
		}
		protected abstract bool CalcVisibileState(DockingHintAdorner adorner);
		protected abstract Rect CalcBounds(DockingHintAdorner adorner);
		bool CalcHotTrack(DockHintButton button, DockHintButton hotButton) {
			return button.IsAvailable && button == hotButton;
		}
	}
}
