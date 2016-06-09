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
using DevExpress.Mvvm.Native;
using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Input;
using System.Collections.ObjectModel;
namespace DevExpress.Xpf.PdfViewer {
	public enum PdfAttachmentsViewerState {
		Collapsed,
		Visible,
		Expanded,
	}
	public enum PdfAttachmentsViewerPanelStyle {
		Popup,
		DockPanel,
		Tab,
	}
	public class PdfAttachmentsViewerSettings : FrameworkElement {
		public static readonly DependencyProperty HideAttachmentsViewerProperty;
		public static readonly DependencyProperty AttachmentsViewerStateProperty;
		public static readonly DependencyProperty AttachmentsViewerInitialStateProperty;
		public static readonly DependencyProperty AttachmentsViewerPanelStyleProperty;
		public static readonly DependencyProperty AttachmentsViewerStyleProperty;
		public static readonly DependencyProperty HideAfterUseProperty;
		public static readonly DependencyProperty ActualHideAfterUseProperty;
		static readonly DependencyPropertyKey ActualHideAfterUsePropertyKey;
		static PdfAttachmentsViewerSettings() {
			HideAttachmentsViewerProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerSettings, bool>(owner => owner.HideAttachmentsViewer, false,
				(settings, value, newValue) => settings.HideAttachmentsViewerChanged(value, newValue));
			AttachmentsViewerStateProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerSettings, PdfAttachmentsViewerState>(owner => owner.AttachmentsViewerState,
				PdfAttachmentsViewerState.Collapsed, (settings, value, newValue) => settings.AttachmentsViewerStateChanged(value, newValue));
			AttachmentsViewerInitialStateProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerSettings, PdfAttachmentsViewerState?>(owner => owner.AttachmentsViewerInitialState, 
				null, (settings, value, newValue) => settings.AttachmentsViewerInitialStateChanged(value, newValue));
			AttachmentsViewerPanelStyleProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerSettings, PdfAttachmentsViewerPanelStyle>(owner => owner.AttachmentsViewerPanelStyle,
				PdfAttachmentsViewerPanelStyle.Tab, (settings, value, newValue) => settings.AttachmentsViewerStyleChanged(value, newValue));
			AttachmentsViewerStyleProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerSettings, Style>(owner => owner.AttachmentsViewerStyle, null);
			HideAfterUseProperty = DependencyPropertyRegistrator.Register<PdfAttachmentsViewerSettings, bool?>(owner => owner.HideAfterUse, null, (settings, value, newValue) => settings.RaiseInvalidate());
			ActualHideAfterUsePropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<PdfAttachmentsViewerSettings, bool>(owner => owner.ActualHideAfterUse, false);
			ActualHideAfterUseProperty = ActualHideAfterUsePropertyKey.DependencyProperty;
		}
		public PdfAttachmentsViewerState AttachmentsViewerState {
			get { return (PdfAttachmentsViewerState)GetValue(AttachmentsViewerStateProperty); }
			set { SetValue(AttachmentsViewerStateProperty, value); }
		}
		public PdfAttachmentsViewerState? AttachmentsViewerInitialState {
			get { return (PdfAttachmentsViewerState?)GetValue(AttachmentsViewerInitialStateProperty); }
			set { SetValue(AttachmentsViewerInitialStateProperty, value); }
		}
		public PdfAttachmentsViewerPanelStyle AttachmentsViewerPanelStyle {
			get { return (PdfAttachmentsViewerPanelStyle)GetValue(AttachmentsViewerPanelStyleProperty); }
			set { SetValue(AttachmentsViewerPanelStyleProperty, value); }
		}
		public Style AttachmentsViewerStyle {
			get { return (Style)GetValue(AttachmentsViewerStyleProperty); }
			set { SetValue(AttachmentsViewerStyleProperty, value); }
		}
		public bool HideAttachmentsViewer {
			get { return (bool)GetValue(HideAttachmentsViewerProperty); }
			set { SetValue(HideAttachmentsViewerProperty, value); }
		}
		public bool ActualHideAfterUse {
			get { return (bool)GetValue(ActualHideAfterUseProperty); }
			internal set { SetValue(ActualHideAfterUsePropertyKey, value); }
		}
		public bool? HideAfterUse {
			get { return (bool?)GetValue(HideAfterUseProperty); }
			set { SetValue(HideAfterUseProperty, value); }
		}
		PdfViewerControl owner;
		protected PdfViewerControl Owner { get { return owner; } }
		public IEnumerable<object> Source { get; internal set; }
		public ICommand OpenAttachmentCommand { get; internal set; }
		public ICommand SaveAttachmentCommand { get; internal set; }
		public event EventHandler Invalidate;
		protected internal void RaiseInvalidate() {
			Invalidate.Do(x => x(this, EventArgs.Empty));
		}
		protected internal virtual void Initialize(PdfViewerControl owner) {
			this.owner = owner;
		}
		protected internal virtual void Release() {
			this.owner = null;
		}
		public virtual void UpdateProperties() {
			if (this.owner == null)
				return;
			UpdatePropertiesInternal();
		}
		protected virtual void UpdatePropertiesInternal() {
			if (HideAfterUse != null)
				ActualHideAfterUse = HideAfterUse.Value;
			var model = Owner.Document;
			Source = new ReadOnlyObservableCollection<PdfAttachmentViewModel>(model.Return(x => new ObservableCollection<PdfAttachmentViewModel>(x.CreateAttachments()), () => new ObservableCollection<PdfAttachmentViewModel>()));
			OpenAttachmentCommand = ((PdfCommandProvider)Owner.ActualCommandProvider).OpenAttachmentCommand;
			SaveAttachmentCommand = ((PdfCommandProvider)Owner.ActualCommandProvider).SaveAttachmentCommand;
			if (AttachmentsViewerInitialState != null)
				UpdateAttachmentsViewerCurrentState();
		}
		protected virtual void UpdateAttachmentsViewerCurrentState() {
			AttachmentsViewerState = AttachmentsViewerInitialState ?? AttachmentsViewerState;
		}
		protected virtual void AttachmentsViewerStyleChanged(PdfAttachmentsViewerPanelStyle oldValue, PdfAttachmentsViewerPanelStyle newValue) {
		}
		protected virtual void AttachmentsViewerInitialStateChanged(PdfAttachmentsViewerState? oldValue, PdfAttachmentsViewerState? newValue) {
		}
		protected virtual void AttachmentsViewerStateChanged(PdfAttachmentsViewerState oldValue, PdfAttachmentsViewerState newValue) {
		}
		protected virtual void HideAttachmentsViewerChanged(bool oldValue, bool newValue) {
			Owner.Do(x => x.UpdateHasAttachments(newValue));
		}
	}
}
