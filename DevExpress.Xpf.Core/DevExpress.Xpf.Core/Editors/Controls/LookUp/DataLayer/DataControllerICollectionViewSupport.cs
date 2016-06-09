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
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.Data.Linq;
using DevExpress.Utils;
using System.Windows.Data;
using System.Windows;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
#else
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Data.Browsing;
using DevExpress.Data.Access;
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Helpers;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Data.Filtering;
using DevExpress.Data.Filtering.Helpers;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Editors.Native;
#endif
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataControllerICollectionViewSupport {
		CollectionViewMoveCurrentEventHandler<DataControllerICollectionViewSupport> CollectionViewMoveCurrentTo { get; set; }
		public ICollectionView CollectionView { get { return ItemsProvider.ListSource; } }
		public bool HasCollectionView { get { return CollectionView != null; } }
		IItemsProviderCollectionViewSupport ItemsProvider { get; set; }
		object CurrentItem {
			get { return ItemsProvider.ListSource != null ? CollectionView.CurrentItem : null; }
			set {
				ICollectionView listSource = ItemsProvider.ListSource;
				if (listSource != null && !object.Equals(listSource.CurrentItem, value))
					listSource.MoveCurrentTo(value);
		   }
		}
		public DataControllerICollectionViewSupport(IItemsProviderCollectionViewSupport itemsProvider) {
			ItemsProvider = itemsProvider;
			CollectionViewMoveCurrentTo = new CollectionViewMoveCurrentEventHandler<DataControllerICollectionViewSupport>(this, (owner, o, e) => owner.RaiseCurrentChanged());
		}
		public virtual void Initialize() {
			if (HasCollectionView)
				CollectionView.CurrentChanged += CollectionViewMoveCurrentTo.Handler;
		}
		public virtual void Release() {
			if (HasCollectionView)
				CollectionView.CurrentChanged -= CollectionViewMoveCurrentTo.Handler;
		}
		public void SyncWithCurrent() {
			RaiseCurrentChanged();
		}
		protected virtual void RaiseCurrentChanged() {
			if (HasCollectionView && ItemsProvider.IsSynchronizedWithCurrentItem)
				ItemsProvider.RaiseCurrentChanged(CurrentItem);
		}
		public virtual void SetCurrentItem(object currentItem) {
			CurrentItem = currentItem;
		}
		public virtual void SyncWithData(IDataControllerVisualClient visual) {
			if (!HasCollectionView || ItemsProvider.DataSync == null)
				return;
			visual.RequireSynchronization(ItemsProvider.DataSync);
		}
	}
}
