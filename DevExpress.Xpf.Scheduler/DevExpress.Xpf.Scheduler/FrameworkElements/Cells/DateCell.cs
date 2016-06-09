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
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls;
using DevExpress.Xpf.Scheduler.Native;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Xpf.Core;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class VisualDateCell : VisualTimeCellBase {
		public VisualDateCell() {
			DefaultStyleKey = typeof(VisualDateCell);
		}
		protected override SchedulerHitTest HitTest { get { return SchedulerHitTest.Cell; } }
		#region IsAlternate
		public static readonly DependencyProperty IsAlternateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDateCell, bool>("IsAlternate", false, (d, e) => d.OnIsAlternateChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDateCellIsAlternate")]
#endif
		public bool IsAlternate { get { return (bool)GetValue(IsAlternateProperty); } set { SetValue(IsAlternateProperty, value); } }
		private void OnIsAlternateChanged(bool oldVal, bool newVal) {
			SelectTemplate();
		}
		#endregion
		#region NormalTemplate
		public static readonly DependencyProperty NormalTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDateCell, ControlTemplate>("NormalTemplate", null, (d, e) => d.OnNormalTemplateChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDateCellNormalTemplate")]
#endif
		public ControlTemplate NormalTemplate { get { return (ControlTemplate)GetValue(NormalTemplateProperty); } set { SetValue(NormalTemplateProperty, value); } }
		protected virtual void OnNormalTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue != null)
				 SealHelper.SealIfSealable(newValue);
			SelectTemplate();
		}
		#endregion
		#region AlternateTemplate
		public static readonly DependencyProperty AlternateTemplateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDateCell, ControlTemplate>("AlternateTemplate", null, (d, e) => d.OnAlternateTemplateChanged(e.OldValue, e.NewValue));
#if !SL
	[DevExpressXpfSchedulerLocalizedDescription("VisualDateCellAlternateTemplate")]
#endif
		public ControlTemplate AlternateTemplate { get { return (ControlTemplate)GetValue(AlternateTemplateProperty); } set { SetValue(AlternateTemplateProperty, value); } }
		protected virtual void OnAlternateTemplateChanged(ControlTemplate oldValue, ControlTemplate newValue) {
			if (newValue != null)
				SealHelper.SealIfSealable(newValue);
			SelectTemplate();
		}
		#endregion
		protected internal virtual void SelectTemplate() {
			ControlTemplate template = IsAlternate ? AlternateTemplate : NormalTemplate;
			if (!Object.ReferenceEquals(Template, template))
				Template = template;
		}
	}
	public class VisualDateCellContent : VisualTimeCellBaseContent {
		#region View
		static readonly DependencyPropertyKey ViewPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDateCellContent, SchedulerViewBase>("View", null);
		public static readonly DependencyProperty ViewProperty = ViewPropertyKey.DependencyProperty;
		public SchedulerViewBase View { get { return (SchedulerViewBase)GetValue(ViewProperty); } protected set { this.SetValue(ViewPropertyKey, value); } }
		#endregion
		#region IsAlternate
		bool lastSettedIsAlternate = false;
		public static readonly DependencyProperty IsAlternateProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDateCellContent, bool>("IsAlternate", false, (d, e) => d.OnIsAlternateChanged(e.OldValue, e.NewValue));
		public bool IsAlternate { get { return (bool)GetValue(IsAlternateProperty); } set { if (this.lastSettedIsAlternate != value) SetValue(IsAlternateProperty, value); } }
		protected virtual void OnIsAlternateChanged(bool oldValue, bool newValue) {
			lastSettedIsAlternate = newValue;
		}
		#endregion
		#region IsEvenMonth
		bool lastSettedIsEvenMonth = false;
		public static readonly DependencyProperty IsEvenMonthProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<VisualDateCellContent, bool>("IsEvenMonth", false, (d, e) => d.OnIsEvenMonthChanged(e.OldValue, e.NewValue));
		public bool IsEvenMonth { get { return (bool)GetValue(IsEvenMonthProperty); } set { if (this.lastSettedIsEvenMonth != value) SetValue(IsEvenMonthProperty, value); } }
		DependencyPropertyChangedEventHandler onIsEvenMonthChanged;
		public event DependencyPropertyChangedEventHandler IsEvenMonthChanged { add { onIsEvenMonthChanged += value; } remove { onIsEvenMonthChanged -= value; } }
		protected virtual void RaiseOnIsEvenMonthChanged(bool oldValue, bool newValue) {
			if (onIsEvenMonthChanged != null) {
				DependencyPropertyChangedEventArgs e = new DependencyPropertyChangedEventArgs();
				onIsEvenMonthChanged(this, e);
			}
			lastSettedIsEvenMonth = newValue;
		}
		protected virtual void OnIsEvenMonthChanged(bool oldValue, bool newValue) {
			RaiseOnIsEvenMonthChanged(oldValue, newValue);
		}
		#endregion
		#region DateFormats
		static readonly DependencyPropertyKey DateFormatsPropertyKey = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterReadOnlyProperty<VisualDateCellContent, ObservableStringCollection>("DateFormats", null);
		public static readonly DependencyProperty DateFormatsProperty = DateFormatsPropertyKey.DependencyProperty;
		public ObservableStringCollection DateFormats { get { return (ObservableStringCollection)GetValue(DateFormatsProperty); } protected set { this.SetValue(DateFormatsPropertyKey, value); } }
		#endregion
		protected internal override bool CopyFromCore(ResourceCellBase source) {
			bool wasChanged = base.CopyFromCore(source);
			DateCell dateCellSource = (DateCell)source;
			if (lastSettedIsAlternate != dateCellSource.Header.IsAlternate) {
				this.IsAlternate = dateCellSource.Header.IsAlternate;
				wasChanged = true;
			}
			bool isEvenMonth = dateCellSource.Interval.Start.Month % 2 == 0;
			if (lastSettedIsEvenMonth != isEvenMonth) {
				this.IsEvenMonth = isEvenMonth;
				wasChanged = true;
			}
			if (DateFormats == null) {
				DateFormats = new ObservableStringCollection();
				wasChanged = true;
			}
			CollectionCopyHelper.CopyDateFormats(DateFormats, dateCellSource.Header.DateFormats);
			this.View = dateCellSource.View;
			return wasChanged;
		}
	}
	public class VisualMonthDateCell : VisualDateCell {
		static VisualMonthDateCell() {
		}
		protected override void SubscribeContentEvents(object content) {
			base.SubscribeContentEvents(content);
			VisualDateCellContent cellContent = content as VisualDateCellContent;
			if (cellContent == null)
				return;
			cellContent.IsEvenMonthChanged += new DependencyPropertyChangedEventHandler(OnIsEvenMonthChanged);
		}
		protected override void UnsubscribeContentEvents(object content) {
			base.UnsubscribeContentEvents(content);
			VisualDateCellContent cellContent = content as VisualDateCellContent;
			if (cellContent == null)
				return;
			cellContent.IsEvenMonthChanged -= new DependencyPropertyChangedEventHandler(OnIsEvenMonthChanged);
		}
		void OnIsEvenMonthChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RecalculateBrush(true);
		}
	}
}
