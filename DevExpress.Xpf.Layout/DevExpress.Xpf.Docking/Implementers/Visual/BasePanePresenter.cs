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
using DevExpress.Xpf.Layout.Core;
namespace DevExpress.Xpf.Docking.VisualElements {
	public abstract class BasePanePresenter<TVisual, TLogical> : psvContentPresenter
		where TVisual : DependencyObject, IDisposable
		where TLogical : BaseLayoutItem {
		public TVisual Owner { get; private set; }
		readonly static DataTemplate Empty = new DataTemplate();
		protected BasePanePresenter() {
			ContentTemplate = Empty;
		}
		protected override void OnDispose() {
			TLogical item = ConvertToLogicalItem(Content);
			if(item != null && Owner is IUIElement)
				item.UIElements.Remove((IUIElement)Owner);
			base.OnDispose();
			Owner = null;
		}
		protected virtual TLogical ConvertToLogicalItem(object content) {
			return content as TLogical;
		}
		protected override void OnContentChanged(object content, object oldContent) {
			base.OnContentChanged(content, oldContent);
			TLogical item = ConvertToLogicalItem(content);
			if(Owner != null)
				ChangeTemplate(item);
		}
		protected internal void EnsureOwner(TVisual owner) {
			Owner = owner;
			ChangeTemplate(ConvertToLogicalItem(Content));
		}
		protected virtual void OnStylePropertyChanged() {
			ChangeTemplate(ConvertToLogicalItem(Content));
		}
		void ChangeTemplate(TLogical item) {
			if(Owner == null) return;
			ContentTemplate = SelectTemplate(item) ?? Empty;
		}
		DataTemplate SelectTemplate(TLogical item) {
			return (item != null && CanSelectTemplate(item)) ?
			   SelectTemplateCore(item) : null;
		}
		protected abstract bool CanSelectTemplate(TLogical item);
		protected abstract DataTemplate SelectTemplateCore(TLogical item);
		#region IFloatingPane Members
		protected override System.Windows.Automation.Peers.AutomationPeer OnCreateAutomationPeer() {
			return new DevExpress.Xpf.Docking.UIAutomation.BasePanePresenterAutomationPeer<TVisual, TLogical>(this);
		}
		#endregion
	}
	public abstract class DockItemContentPresenter<TVisual, TLogical> : BasePanePresenter<TVisual, TLogical>
		where TVisual : DependencyObject, IDisposable
		where TLogical : BaseLayoutItem {
		#region static
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsControlItemsHostProperty;
		[Core.Native.IgnoreDependencyPropertiesConsistencyChecker]
		public static readonly DependencyProperty IsDataBoundProperty;
		static DockItemContentPresenter() {
			var dProp = new DependencyPropertyRegistrator<DockItemContentPresenter<TVisual, TLogical>>();
			dProp.Register("IsControlItemsHost", ref IsControlItemsHostProperty, false,
				(dObj, ea) => ((DockItemContentPresenter<TVisual, TLogical>)dObj).OnIsControlItemsHostChanged((bool)ea.NewValue));
			dProp.Register("IsDataBound", ref IsDataBoundProperty, false,
				(dObj, ea) => ((DockItemContentPresenter<TVisual, TLogical>)dObj).OnIsDataBoundChanged((bool)ea.NewValue));
		}
		#endregion
		protected override TLogical ConvertToLogicalItem(object content) {
			return LayoutItemData.ConvertToBaseLayoutItem(content) as TLogical ?? base.ConvertToLogicalItem(content);
		}
		protected virtual void OnIsControlItemsHostChanged(bool value) {
			EnsureOwner(Owner);
		}
		protected virtual void OnIsDataBoundChanged(bool value) {
			EnsureOwner(Owner);
		}
	}
}
