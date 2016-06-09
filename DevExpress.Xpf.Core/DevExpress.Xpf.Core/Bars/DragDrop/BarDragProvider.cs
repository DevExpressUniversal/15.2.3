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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using DevExpress.Xpf.Bars.Customization;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Utils;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Bars {
	public class BarDragProvider : DependencyObject {
		#region static
		public static readonly DependencyProperty DragTypeProperty;
		public static readonly DependencyProperty DropIndicatorOrientationProperty;
		static BarDragProvider() {
			DragTypeProperty = DependencyPropertyManager.RegisterAttached("DragType", typeof(DragType), typeof(BarDragProvider), new FrameworkPropertyMetadata(DragType.Move, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDragTypePropertyChanged))); 
			DropIndicatorOrientationProperty = DependencyPropertyManager.RegisterAttached("DropIndicatorOrientation", typeof(Orientation), typeof(BarDragProvider), new FrameworkPropertyMetadata(Orientation.Horizontal, FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnDropIndicatorOrientationPropertyChanged))); 
		}
		protected static void OnDropIndicatorOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			VisualStateManager.GoToState((Control)d, e.NewValue.ToString(), false);
		}
		protected static void OnDragTypePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BarDragElementPopup popup = d as BarDragElementPopup;
			if(popup != null)
				popup.OnDragTypeChanged(e);
			BarItemDragElementContent item = d as BarItemDragElementContent;
			if(item != null)
				item.OnDragTypeChanged(e);
		}
		public static DragType GetDragType(DependencyObject obj) { return (DragType)obj.GetValue(DragTypeProperty); }
		public static void SetDragType(DependencyObject obj, DragType value) { obj.SetValue(DragTypeProperty, value); }
		public static Orientation GetDropIndicatorOrientation(DependencyObject obj) { return (Orientation)obj.GetValue(DropIndicatorOrientationProperty); }
		public static void SetDropIndicatorOrientation(DependencyObject obj, Orientation value) { obj.SetValue(DropIndicatorOrientationProperty, value); }
		internal static void SetDragTypeCore(UIElement source, DragType value) {
			BarItemList list = source as BarItemList;
			if(list != null) {
				BarDragProvider.SetDragType(list.DragElement as DependencyObject, value);
				return;
			}
			BarItemLinkControl link = source as BarItemLinkControl;
			if(link != null) {
				BarDragProvider.SetDragType(link.DragElement as DependencyObject, value);
				return;
			}
			if(source != null)
				BarDragProvider.SetDragType(source, value);
		}
		internal static DragType GetDragTypeCore(UIElement source) {
			BarItemList list = source as BarItemList;
			if(list != null)
				return BarDragProvider.GetDragType(list.DragElement as DependencyObject);
			BarItemLinkControl link = source as BarItemLinkControl;
			if(link != null)
				return BarDragProvider.GetDragType(link.DragElement as DependencyObject);
			return DragType.Remove;
		}
		public static void RemoveUnnesessarySeparators(BarItemLinkCollection links) {
			for(int i = links.Count - 1; i >= 0; i--) {
				BarItemLinkSeparator sep = links[i] as BarItemLinkSeparator;
				if(sep == null) continue;
				if(i == 0 || i == links.Count - 1 || (i < links.Count - 1 && links[i + 1] is BarItemLinkSeparator))
					links.RemoveAt(i);
			}
		}
		public static void RemoveUnnesessarySeparators(List<BarItemLinkBase> links) {
			for(int i = links.Count - 1; i >= 0; i--) {
				BarItemLinkSeparator sep = links[i] as BarItemLinkSeparator;
				if(sep == null) continue;
				if(i == 0 || i == links.Count - 1 || (i < links.Count - 1 && links[i + 1] is BarItemLinkSeparator))
					links.RemoveAt(i);
			}
		}
		#endregion
	}
	public class BarDragDropElementHelper : DevExpress.Xpf.Core.Native.DragDropElementHelper {
		static BarDragDropElementHelper current;
		public static BarDragDropElementHelper Current {
			get { return current; }
		}
		public ISupportDragDrop Owner { get { return SupportDragDrop; } }
		public BarDragDropElementHelper(ISupportDragDrop supportDragDrop, bool isRelativeMode = true) : base(supportDragDrop, isRelativeMode) { }
		public static IEnumerable<UIElement> GetTopLevelDropContainers(DependencyObject node) {
			var fe = BarNameScope.GetService<IElementRegistratorService>(node).GetElements<IFrameworkInputElement>(Native.ScopeSearchSettings.Local);
			var bc = fe.OfType<Bar>().Select(x => x.DockInfo.BarControl);
			var pm = fe.OfType<PopupMenuBase>().Where(x => x.IsOpen).Select(x => x.PopupContent);
			var bcc = fe.OfType<BarContainerControl>();
			return pm.Concat(bc).Concat(bcc).OfType<UIElement>().Where(x => x != null).ToList();
		}
		protected override void UpdateDragElementLocation(Point newPos) {
			if(DragElement == null) return;
			newPos = CorrectDragElementLocation(newPos);
			DragElement.UpdateLocation(newPos);
		}
		protected internal override void StartDragging(Point offset, IndependentMouseEventArgs e) {
			Current.Do(x => x.EndDragging(null));
			current = this;
			base.StartDragging(offset, e);
		}
		protected override void EndDragging(IndependentMouseButtonEventArgs e) {
			base.EndDragging(e);
			current = null;
			if (shouldDestroy)
				Destroy();
		}
		public void CancelDestroy() {
			shouldDestroy = false;
		}
		bool shouldDestroy = false;
		public void BeginDestroy() {
			if (IsDragging)
				shouldDestroy = true;
			else
				Destroy();
		}
		public override void Destroy() {
			base.Destroy();
			var lc = Owner as BarItemLinkControl;
			lc.AfterDestroyDragDropHelper();
		}
	}
}
