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
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Collections.Specialized;
using DevExpress.Xpf.Utils;
using System;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core.Native;
using System.Text;
using System.Windows.Data;
namespace DevExpress.Xpf.Bars {
#if DEBUGTEST
	[System.Diagnostics.DebuggerDisplay("{GetDebuggerString()}")]
#endif
	public class BarItemLinkInfo : ContentPresenter, IMutableNavigationElement {
		protected static readonly DependencyPropertyKey ContentDesiredSizePropertyKey;
		public static readonly DependencyProperty ContentDesiredSizeProperty;			 
		public static readonly DependencyProperty IndexProperty;
		public static readonly DependencyProperty IsRemovedProperty;
		public bool IsRemoved {
			get { return (bool)GetValue(IsRemovedProperty); }
			set { SetValue(IsRemovedProperty, value); }
		}
		static BarItemLinkInfo() {
			IndexProperty = DependencyProperty.Register("Index", typeof(int), typeof(BarItemLinkInfo), new FrameworkPropertyMetadata(0, (d, e) => ((BarItemLinkInfo)d).OnIndexChanged()));
			IsRemovedProperty = DependencyProperty.Register("IsRemoved", typeof(bool), typeof(BarItemLinkInfo), new FrameworkPropertyMetadata(false, (d,e)=>((BarItemLinkInfo)d).OnIsRemovedChanged()));
			ContentDesiredSizePropertyKey = DependencyPropertyManager.RegisterReadOnly("ContentDesiredSize", typeof(Size), typeof(BarItemLinkInfo), new FrameworkPropertyMetadata(Size.Empty, FrameworkPropertyMetadataOptions.AffectsMeasure | FrameworkPropertyMetadataOptions.AffectsParentMeasure));			
			ContentDesiredSizeProperty = ContentDesiredSizePropertyKey.DependencyProperty;
		}		
		public BarItemLinkInfo(BarItemLinkBase linkBase) {
			LinkBase = linkBase;
			LinkContainerType = LinkContainerType.None;
		}		
		#region props
		public BarItem Item { get { return Link != null ? Link.Item : null; } }
		BarPopupExpandMode barPopupExpandMode;
		public int Index {
			get { return (int)GetValue(IndexProperty); }
			set { SetValue(IndexProperty, value); }
		}
		public BarPopupExpandMode BarPopupExpandMode {
			get { return barPopupExpandMode; }
			set {
				if(barPopupExpandMode != value) {
					var oldValue = barPopupExpandMode;
					barPopupExpandMode = value;
					OnBarPopupExpandModeChanged(oldValue);
				}
			}
		}
		private BarItemLinkBase linkBaseCore = null;
		public BarItemLinkBase LinkBase {
			get { return linkBaseCore; }
			set {
				if(linkBaseCore == value) return;
				BarItemLinkBase oldValue = linkBaseCore;
				linkBaseCore = value;
				OnLinkBaseChanged(oldValue);
			}
		}
		public BarItemLink Link { get { return LinkBase as BarItemLink; } }		
		private LinkContainerType linkContainerTypeCore = LinkContainerType.None;
		public LinkContainerType LinkContainerType {
			get { return linkContainerTypeCore; }
			set {
				if(linkContainerTypeCore == value) return;
				LinkContainerType oldValue = linkContainerTypeCore;
				linkContainerTypeCore = value;
				OnLinkContainerTypeChanged(oldValue);
			}
		}
		internal BarItemLinkControlBase linkControlCore;
		public BarItemLinkControlBase LinkControl {
			get { return linkControlCore; }
			protected set {
				if(LinkControl == value) return;
				BarItemLinkControlBase oldValue = linkControlCore;
				linkControlCore = value;
				OnLinkControlChanged(oldValue);
			}
		}
		public LinksControl LinksControl {
			get { return ItemsControl.ItemsControlFromItemContainer(this) as LinksControl; }
		}
		private BarContainerControl containerCore = null;
		public BarContainerControl Container {
			get { return containerCore; }
			set {
				if(containerCore == value) return;
				BarContainerControl oldValue = containerCore;
				containerCore = value;
				OnContainerChanged(oldValue);
			}
		}
		public Size ContentDesiredSize {
			get { return (Size)GetValue(ContentDesiredSizeProperty); }
			protected internal set { this.SetValue(ContentDesiredSizePropertyKey, value); }
		}
		protected virtual void OnIndexChanged() {
			if (OwnerCollection != null)
				OwnerCollection.LinkInfoChanged(this);
		}
		protected virtual void OnIsRemovedChanged() {
			if (OwnerCollection != null)
				OwnerCollection.LinkInfoChanged(this);
		}
		#endregion
		protected virtual void OnLinkControlChanged(BarItemLinkControlBase oldValue) {
			if(oldValue != null) {
				oldValue.OnClear();
			}
			if(LinkControl != null) {
				InitializeLinkControl();
				if(VisualTreeHelper.GetParent(this) is UIElement)
					((UIElement)VisualTreeHelper.GetParent(this)).InvalidateMeasure();
			}
			UpdateLinkControlExpandMode();
			Content = LinkControl;
		}
		protected virtual void OnLinkBaseChanged(BarItemLinkBase oldValue) {
			if(oldValue != null)
				oldValue.LinkInfos.Remove(this);
			if(LinkBase != null) {
				LinkBase.LinkInfos.Add(this);
				SetBinding(IsRemovedProperty, new Binding() { Path = new PropertyPath(BarItemLinkBase.IsRemovedProperty), Source = LinkBase });
				SetBinding(IndexProperty, new Binding() { Path = new PropertyPath(BarItemLinkBase.IndexProperty), Source = LinkBase });
			} else {
				ClearValue(IndexProperty);
				ClearValue(IsRemovedProperty);
			}
		}
		protected virtual void OnLinkContainerTypeChanged(LinkContainerType oldValue) {
			UpdateLinkContainerType();
		}
		protected void UpdateLinkContainerType() {
			if(LinkControl != null) {
				LinkControl.ContainerType = LinkContainerType;
			}
		}
		protected void UpdateContainer() {
			if(LinkControl != null)
				LinkControl.Container = Container;
		}
		protected virtual void OnContainerChanged(BarContainerControl oldValue) {
			UpdateContainer();
		}
		void UpdateCustomResources() {
			if(LinkControl == null)
				return;
			ResourceDictionary dictionarty = new ResourceDictionary();			
			if(Link != null && Link.CustomResources != null) {
				dictionarty.MergedDictionaries.Add(Link.CustomResources);
			} else dictionarty = null;
			LinkControl.Resources = dictionarty;
		}	   
		protected virtual void OnLinksControlChanged(LinksControl oldValue) {
		}
		protected virtual void InitializeLinkControl() {
			if(LinkControl == null)
				return;
			LinkControl.LinkBase = LinkBase;
			LinkControl.LinkInfo = this;
			UpdateCustomResources();
			UpdateContainer();
			UpdateLinkContainerType();
			if(LinkControl is BarItemLinkControl)
				((BarItemLinkControl)LinkControl).InitializeRibbonStyle();
			LinkControl.UpdateVisibility();
			LinkControl.UpdateIsEnabled();
			LinkControl.UpdateActualProperties();
			LinkControl.UpdateOrientation();
			changed(this, EventArgs.Empty);
			LinksControl.Do(x => x.RaiseChanged());
		}
		protected virtual void OnBarPopupExpandModeChanged(BarPopupExpandMode oldValue) {
			UpdateLinkControlExpandMode();
		}
		protected virtual void UpdateLinkControlExpandMode() {
			var linkControl = LinkControl as BarItemLinkControl;
			if(linkControl != null)
				linkControl.SetExpandMode(BarPopupExpandMode);
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			if(LinkControl as BarItemLinkControl!=null && !LinkControl.IsEnabled)
				(LinkControl as BarItemLinkControl).UpdateToolTip();
		}
		public BarItemLinkInfoCollection OwnerCollection { get; internal set; }
#if DEBUGTEST
		public string GetDebuggerString() {
			StringBuilder builder = new StringBuilder();
			builder.Append("LinkInfo(");
			if (LinkControl as BarItemLinkControl!= null) {
				builder.Append(LinkControl.GetType().Name);
				builder.Append(String.Format(" '{0}'", ((BarItemLinkControl)LinkControl).ActualContent));
			} else {
				builder.Append("empty");
			}
			builder.Append(")");
			return builder.ToString();
		}
#endif
		protected internal void OnItemChanged(BarItem oldItem) {
			if(Item == null || oldItem == null || Item.GetType() != oldItem.GetType() || LinkControl == null)
				CreateLinkControl();
			else if(LinkControl != null)
				LinkControl.UpdateActualProperties();
			if(OwnerCollection != null && Item != oldItem)
				OwnerCollection.OnLinkItemChanged(this, oldItem);		
		}
		protected internal void CreateLinkControl() {
			if(LinkBase == null)
				return;
			LinkControl = LinkBase.CreateBarItemLinkControl();
		}
		protected internal void ClearLinkControl() {
			LinkControl = null;
		}
		protected internal void OnChildLinkCollectionChanged(NotifyCollectionChangedEventArgs e) {
			if(OwnerCollection != null)
				OwnerCollection.OnChildLinkCollectionChanged(this, e);
		}
		protected override Size MeasureOverride(Size constraint) {
			var sz = base.MeasureOverride(constraint);
			ContentDesiredSize = sz;
			return sz;
		}				
		bool INavigationElement.ProcessKeyDown(KeyEventArgs e) {
			return LinkControl.ProcessKeyDown(e);
		}
		bool IBarsNavigationSupport.IsSelectable {			
			get { return (LinkControl as BarItemLinkControl).Return(x => x.GetIsSelectable(), () => false); }
		}
		bool INavigationElement.IsSelected {
			get { return (LinkControl as BarItemLinkControl).Return(x => x.IsSelected, () => false); }
			set { if (!(LinkControl is BarItemLinkControl)) return; ((BarItemLinkControl)LinkControl).ForceSetIsSelected(value); }
		}
		INavigationOwner INavigationElement.BoundOwner { get { return LinkControl.With(x=>x.GetBoundOwner()); } }
		IBarsNavigationSupport IBarsNavigationSupport.Parent { get { return LinksControl; } }
		int IBarsNavigationSupport.ID { get { return Item.Return(x => x.GetHashCode(), () => -1); } }
		bool IBarsNavigationSupport.ExitNavigationOnMouseUp { get { return false; } }
		bool IBarsNavigationSupport.ExitNavigationOnFocusChangedWithin { get { return false; } }
		event EventHandler IMutableNavigationSupport.Changed {
			add { changed += value; }
			remove { changed -= value; }
		}
		void IMutableNavigationSupport.RaiseChanged() {
			changed(this, EventArgs.Empty);
		}
		EventHandler changed = new EventHandler((o, e) => { });		
	}
	public static class BarItemLinkInfoExtension {
		static bool GetIsHidden(DependencyObject obj) { return (bool)obj.GetValue(IsHiddenProperty); }
		static void SetIsHidden(DependencyObject obj, bool value) { obj.SetValue(IsHiddenProperty, value); }
		[IgnoreDependencyPropertiesConsistencyChecker]
		static readonly DependencyProperty IsHiddenProperty = DependencyProperty.RegisterAttached("IsHidden", typeof(bool), typeof(BarItemLinkInfoExtension), new PropertyMetadata(false));
		public static void Hide(this BarItemLinkInfo info) {
			if(info == null) return;
			SetIsHidden(info, true);
			info.Opacity = 0d;
			info.IsHitTestVisible = false;
		}
		public static void Show(this BarItemLinkInfo info) {
			if(info == null) return;
			SetIsHidden(info, false);
			info.Opacity = 1d;
			info.IsHitTestVisible = true;
		}
		public static bool IsHidden(this BarItemLinkInfo info) {
			if (info == null)
				return false;
			return GetIsHidden(info);
		}
	}
}
