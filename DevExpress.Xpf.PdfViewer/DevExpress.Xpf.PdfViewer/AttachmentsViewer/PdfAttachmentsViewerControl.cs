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

using DevExpress.Xpf.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.PdfViewer {
	public class PdfAttachmentsViewerControl : Control {
		static readonly DependencyPropertyKey ActualSourcePropertyKey;
		public static readonly DependencyProperty ActualSourceProperty;
		public static readonly DependencyProperty SettingsProperty;
		static readonly DependencyPropertyKey ActualSettingsPropertyKey;
		public static readonly DependencyProperty ActualSettingsProperty;
		public static readonly DependencyProperty HideAfterUseProperty;
		static readonly DependencyPropertyKey HighlightedItemPropertyKey;
		public static readonly DependencyProperty HighlightedItemProperty;
		static PdfAttachmentsViewerControl() {
			Type ownerType = typeof(PdfAttachmentsViewerControl);
			ActualSourcePropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfAttachmentsViewerControl, IEnumerable<object>>(
				 owner => owner.ActualSource, null, (d, oldValue, newValue) => d.ActualSourceChanged(oldValue, newValue));
			ActualSourceProperty = ActualSourcePropertyKey.DependencyProperty;
			ActualSettingsPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfAttachmentsViewerControl, PdfAttachmentsViewerSettings>(
				 owner => owner.ActualSettings, null, (d, oldValue, newValue) => d.ActualSettingsChanged(oldValue, newValue));
			ActualSettingsProperty = ActualSettingsPropertyKey.DependencyProperty;
			SettingsProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerControl, PdfAttachmentsViewerSettings>(
				owner => owner.Settings, null, (d, oldValue, newValue) => d.SettingsChanged(oldValue, newValue));
			HideAfterUseProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerControl, bool>(owner => owner.HideAfterUse, false, (control, value, newValue) => control.HideAfterUseChanged(value, newValue));
			HighlightedItemPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfAttachmentsViewerControl, object>(owner => owner.HighlightedItem, null, (control, value, newValue) => control.HighlightedItemChanged(value, newValue));
			HighlightedItemProperty = HighlightedItemPropertyKey.DependencyProperty;
		}
		protected virtual void ActualSourceChanged(IEnumerable<object> oldValue, IEnumerable<object> newValue) {
		}
		protected virtual void SettingsChanged(PdfAttachmentsViewerSettings oldValue, PdfAttachmentsViewerSettings newValue) {
			ActualSettings = CalcActualSettings(newValue);
		}
		protected virtual PdfAttachmentsViewerSettings CalcActualSettings(PdfAttachmentsViewerSettings settings) {
			return settings ?? CreateAttachmentsViewerSettings();
		}
		protected virtual PdfAttachmentsViewerSettings CreateAttachmentsViewerSettings() {
			return new PdfAttachmentsViewerSettings();
		}
		protected virtual void ActualSettingsChanged(PdfAttachmentsViewerSettings oldValue, PdfAttachmentsViewerSettings newValue) {
			oldValue.Do(x => x.Invalidate -= InvalidateSettings);
			newValue.Do(x => x.Invalidate += InvalidateSettings);
			AssignSource(newValue);
		}
		protected virtual void HighlightedItemChanged(object oldValue, object newValue) {
		}
		protected virtual void HideAfterUseChanged(bool oldValue, bool newValue) {
			ActualSettings.ActualHideAfterUse = newValue;
		}
		void InvalidateSettings(object sender, EventArgs eventArgs) {
			ActualSettings.Do(AssignSource);
		}
		void AssignSource(PdfAttachmentsViewerSettings newValue) {
			ClearValue(HideAfterUseProperty);
			if (newValue == null) {
				ActualSource = new ObservableCollection<object>();
				return;
			}
			newValue.UpdateProperties();
			ActualSource = newValue.Source;
			HideAfterUse = newValue.HideAfterUse ?? HideAfterUse;
		}
		public IEnumerable<object> ActualSource {
			get { return (IEnumerable<object>)GetValue(ActualSourceProperty); }
			private set { SetValue(ActualSourcePropertyKey, value); }
		}
		public PdfAttachmentsViewerSettings ActualSettings {
			get { return (PdfAttachmentsViewerSettings)GetValue(ActualSettingsProperty); }
			private set { SetValue(ActualSettingsPropertyKey, value); }
		}
		public PdfAttachmentsViewerSettings Settings {
			get { return (PdfAttachmentsViewerSettings)GetValue(SettingsProperty); }
			set { SetValue(SettingsProperty, value); }
		}
		public object HighlightedItem {
			get { return GetValue(HighlightedItemProperty); }
			protected internal set { SetValue(HighlightedItemPropertyKey, value); }
		}
		public bool HideAfterUse {
			get { return (bool)GetValue(HideAfterUseProperty); }
			set { SetValue(HideAfterUseProperty, value); }
		}
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (ActualSource == null)
				this.SetValue(ActualSourcePropertyKey, new ObservableCollection<PdfAttachmentViewModel>());
		}
		public PdfAttachmentsViewerControl() {
			DefaultStyleKey = typeof(PdfAttachmentsViewerControl);
		}
	}
}
