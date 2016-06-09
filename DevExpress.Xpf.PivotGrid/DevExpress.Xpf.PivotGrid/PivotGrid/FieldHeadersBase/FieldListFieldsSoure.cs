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
	public class FieldListFieldsSoure : FieldsSourceBase {
		public FieldListFieldsSoure(IFieldHeaders headers)
			: base(headers) {
		}
		public override void EnsureItems() {
			PivotFieldsReadOnlyObservableCollection ItemsSource = null;
			if(Headers.Data == null) {
				ItemsSource = null;
			} else {
				if(!Headers.GetActualShowAll()) {
					ItemsSource = (Headers.Area == FieldListArea.All) ?
						Headers.Data.FieldListFields.HiddenFields : Headers.Data.FieldListFields[Headers.Area.ToFieldArea()];
				} else {
					if(Headers.Area != FieldListArea.All)
						ItemsSource = Headers.Data.FieldListFields.HiddenDataFields;
					else
						ItemsSource = Headers.Data.FieldListFields.AllFields;
				}
			}
			Headers.SetItems(ItemsSource);
		}
		public override void OnLoaded() {
			EnsureItems();
		}
		public override void OnUnloaded() { }
	}
}
