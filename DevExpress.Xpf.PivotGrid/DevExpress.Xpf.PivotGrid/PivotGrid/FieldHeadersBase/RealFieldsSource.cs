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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Localization;
using DevExpress.XtraPivotGrid.Data;
using System.Collections.Specialized;
using PivotFieldsObservableCollection = DevExpress.Xpf.Core.ObservableCollectionCore<DevExpress.Xpf.PivotGrid.PivotGridField>;
using PivotFieldsReadOnlyObservableCollection = System.Collections.ObjectModel.ReadOnlyObservableCollection<DevExpress.Xpf.PivotGrid.PivotGridField>;
#if SL
using ApplicationException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class RealFieldsSource : FieldsSourceBase {
		List<PivotGridField> boundFields;
		List<PivotGridGroup> boundGroups;
		bool subscribe, subscribed;
#if DEBUGTEST
		internal
#endif
		bool isItemsValid;
		public RealFieldsSource(IFieldHeaders headers) : this(headers, true) { }
		public RealFieldsSource(IFieldHeaders headers, bool subscribe) 
			: base(headers) {
			this.subscribe = subscribe;
			this.boundFields = new List<PivotGridField>();
			this.boundGroups = new List<PivotGridGroup>();
		}
		public override void OnLoaded() {
			SubscribeEvents();
			ResetItems();
		}
		public override void OnUnloaded() {
			UnsubscribeEvents();
		}
		public override void Dispose() {
			UnsubscribeEvents(); 
			base.Dispose();
		}
		public override void EnsureItems() {
			if(isItemsValid || Headers == null)
				return;
			SubscribeEvents();
			if(Headers.Data == null)
				return;
			isItemsValid = true;
			PivotFieldsObservableCollection fields = null;
			if(Headers.Area != FieldListArea.All) {
				List<PivotFieldItemBase> fieldItems = Headers.Data.FieldItems.GetFieldItemsByArea(Headers.Area.ToPivotArea(), true);
				fields = new PivotFieldsObservableCollection();
				for(int i = 0; i < fieldItems.Count; i++)
					fields.Add(((PivotFieldItem)fieldItems[i]).Wrapper);
				if(!AreFieldsChanged(fields) && fields.Count != 0)
					return;
			}
			Headers.SetItems(Headers.Area != FieldListArea.All ? new PivotFieldsReadOnlyObservableCollection(fields) : Headers.Data.FieldListFields.HiddenFields);
			if(Headers.Area != FieldListArea.All) {
				boundFields.Clear();
				boundFields.AddRange(fields);
				boundGroups.Clear();
				for(int i = 0; i < fields.Count; i++) {
					boundGroups.Add(fields[i].Group);
				}
			}
		}
		protected bool AreFieldsChanged(PivotFieldsObservableCollection fields) {
			if(fields.Count != boundFields.Count)
				return true;
			for(int i = 0; i < fields.Count; i++) {
				if(fields[i] != boundFields[i] || fields[i].Group != boundGroups[i])
					return true;
			}
			return false;
		}
		public void ResetItems() {
			isItemsValid = false;
			EnsureItems();
		}
		protected virtual void SubscribeEvents() {
			if(Headers == null || Headers.Data == null || !subscribe || subscribed || !Headers.IsLoaded)
				return;
			Headers.Data.LayoutChangedEvent += OnDataLayoutChanged;
			subscribed = true;
		}
		protected virtual void UnsubscribeEvents() {
			if(Headers == null || Headers.Data == null || !subscribed)
				return;
			Headers.Data.LayoutChangedEvent -= OnDataLayoutChanged;
			subscribed = false;
		}
		protected virtual void OnDataLayoutChanged(object sender, EventArgs e) {
			ResetItems();
		}
	}
}
