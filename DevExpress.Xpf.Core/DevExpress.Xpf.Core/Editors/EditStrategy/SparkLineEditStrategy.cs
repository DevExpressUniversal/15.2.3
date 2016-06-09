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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Mvvm.Native;
using System.Collections;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
#endif
namespace DevExpress.Xpf.Editors {
	public class SparklineEditStrategy : EditStrategyBase {
		Range valueRange;
		Range argumentRange;
		new SparklineEdit Editor { get { return base.Editor as SparklineEdit; } }
		SparklineItemsProvider ItemsProvider { get { return Editor.ItemsProvider; } }
		public SparklineEditStrategy(SparklineEdit editor)
			: base(editor) {
			FilterChangedLocker = new Locker();
			valueRange = new Range();
			argumentRange = new Range();
		}
		SparklineControl Sparkline { get { return Editor.EditCore as SparklineControl; } }
		Locker FilterChangedLocker { get; set; }
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(SparklineEdit.ItemsProperty, baseValue => baseValue, baseValue => GetItems(baseValue));
		}
		protected virtual object GetItems(object baseValue) {
			if (baseValue is IEnumerable)
				return baseValue;
			return new List<object>() {baseValue};
		}
		public object CoerceItems(object value) {
			return CoerceValue(SparklineEdit.ItemsProperty, value);
		}
		public virtual void ItemsChanged(IEnumerable oldValue, IEnumerable newValue) {
			SyncWithValue(SparklineEdit.ItemsProperty, oldValue, newValue);
		}
		protected override void SyncWithValueInternal() {
			base.SyncWithValueInternal();
			ItemsProvider.ItemsSource = ValueContainer.EditValue;
			if (Sparkline != null) {
				valueRange.SetContainer(Sparkline);
				argumentRange.SetContainer(Sparkline);
			}
			Sparkline.Do(x => x.Points = ItemsProvider.Points);
			Sparkline.Do(x => x.ValueRange = valueRange);
			Sparkline.Do(x => x.ArgumentRange = argumentRange);
		}
		protected override void SyncEditCorePropertiesInternal() {
			base.SyncEditCorePropertiesInternal();
			if (Sparkline != null) {
				valueRange.SetContainer(Sparkline);
				argumentRange.SetContainer(Sparkline);
			}
			Sparkline.Do(x => x.Points = ItemsProvider.Points);
			Sparkline.Do(x => x.ValueRange = valueRange);
			Sparkline.Do(x => x.ArgumentRange = argumentRange);
		}
		public virtual void ArgumentMemberChanged(string argument) {
			ItemsProvider.PointArgumentMember = argument;
			SyncWithValue();
		}
		public virtual void ValueMemberChanged(string newValue) {
			ItemsProvider.PointValueMember = newValue;
			SyncWithValue();
		}
		public virtual void FilterCriteriaChanged(CriteriaOperator newCriteriaOperator) {
			ItemsProvider.FilterCriteria = newCriteriaOperator;
			SyncWithValue();
		}
		public virtual void PointArgumentSortOrderChanged(SparklineSortOrder newColumnSortOrder) {
			ItemsProvider.PointArgumentSortOrder = newColumnSortOrder;
			SyncWithValue();
		}
		public virtual void ItemProviderChanged(ItemsProviderChangedEventArgs e) {
			if (ValueContainer.HasValueCandidate)
				return;
			FilterChangedLocker.DoIfNotLocked(SyncWithValue);
		}
		public override void OnInitialized() {
			Editor.UnsubscribeToItemsProviderChanged();
			Editor.SubscribeToItemsProviderChanged();
			base.OnInitialized();
		}
		public virtual void PointValueRangeChanged(Range range) {
			valueRange = range;
			if (Sparkline != null) {
				if (valueRange != null)
					valueRange.SetContainer(Sparkline);
				if (argumentRange != null)
					argumentRange.SetContainer(Sparkline);
			}
			Sparkline.Do(x => x.ValueRange = valueRange);
		}
		public virtual void PointArgumentRangeChanged(Range range) {
			argumentRange = range;
			if (Sparkline != null) {
				if (valueRange != null)
					valueRange.SetContainer(Sparkline);
				if (argumentRange != null)
					argumentRange.SetContainer(Sparkline);
			}
			Sparkline.Do(x => x.ArgumentRange = argumentRange);
		}
	}
}
