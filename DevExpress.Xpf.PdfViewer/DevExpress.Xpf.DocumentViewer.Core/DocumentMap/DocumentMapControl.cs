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
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DocumentViewer {
	public class DocumentMapControl : Control {
		const string GridName = "PART_GridControl";
		static readonly DependencyPropertyKey ActualSourcePropertyKey;
		public static readonly DependencyProperty ActualSourceProperty;
		public static readonly DependencyProperty TreeViewStyleProperty;
		public static readonly DependencyProperty SettingsProperty;
		static readonly DependencyPropertyKey ActualSettingsPropertyKey;
		public static readonly DependencyProperty ActualSettingsProperty;
		public static readonly DependencyProperty WrapLongLinesProperty;
		public static readonly DependencyProperty HideAfterUseProperty;
		static readonly DependencyPropertyKey HighlightedItemPropertyKey;
		public static readonly DependencyProperty HighlightedItemProperty;
		static DocumentMapControl() {
			ActualSourcePropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<DocumentMapControl, IEnumerable<object>>(
				owner => owner.ActualSource, null, (d, oldValue, newValue) => d.ActualSourceChanged(oldValue, newValue));
			ActualSourceProperty = ActualSourcePropertyKey.DependencyProperty;
			TreeViewStyleProperty = DependencyPropertyRegistrator.Register<DocumentMapControl, Style>(owner => owner.TreeViewStyle, null);
			SettingsProperty = DependencyPropertyRegistrator.Register<DocumentMapControl, DocumentMapSettings>(
				owner => owner.Settings, null, (control, oldValue, newValue) => control.SettingsChanged(oldValue, newValue));
			ActualSettingsPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<DocumentMapControl, DocumentMapSettings>(owner => owner.ActualSettings, null,
				(control, oldValue, newValue) => control.ActualSettingsChanged(oldValue, newValue));
			ActualSettingsProperty = ActualSettingsPropertyKey.DependencyProperty;
			WrapLongLinesProperty = DependencyPropertyRegistrator.Register<DocumentMapControl, bool>(owner => owner.WrapLongLines, false);
			HideAfterUseProperty = DependencyPropertyRegistrator.Register<DocumentMapControl, bool>(owner => owner.HideAfterUse, false, (control, value, newValue) => control.HideAfterUseChanged(value, newValue));
			HighlightedItemPropertyKey = DependencyPropertyRegistrator.RegisterReadOnly<DocumentMapControl, object>(owner => owner.HighlightedItem, null, (control, value, newValue) => control.HighlightedItemChanged(value, newValue));
			HighlightedItemProperty = HighlightedItemPropertyKey.DependencyProperty;
		}
		public object HighlightedItem {
			get { return GetValue(HighlightedItemProperty); }
			protected internal set { SetValue(HighlightedItemPropertyKey, value);}
		}
		public bool HideAfterUse {
			get { return (bool)GetValue(HideAfterUseProperty); }
			set { SetValue(HideAfterUseProperty, value); }
		}
		public bool WrapLongLines {
			get { return (bool)GetValue(WrapLongLinesProperty); }
			set { SetValue(WrapLongLinesProperty, value); }
		}
		public DocumentMapSettings ActualSettings {
			get { return (DocumentMapSettings)GetValue(ActualSettingsProperty); }
			private set { SetValue(ActualSettingsPropertyKey, value); }
		}
		public DocumentMapSettings Settings {
			get { return (DocumentMapSettings)GetValue(SettingsProperty); }
			set { SetValue(SettingsProperty, value); }
		}
		public Style TreeViewStyle {
			get { return (Style)GetValue(TreeViewStyleProperty); }
			set { SetValue(TreeViewStyleProperty, value); }
		}
		public IEnumerable<object> ActualSource {
			get { return (IEnumerable<object>)GetValue(ActualSourceProperty); }
			private set { SetValue(ActualSourcePropertyKey, value); }
		}
		GridControl grid;
		protected override void OnInitialized(EventArgs e) {
			base.OnInitialized(e);
			if (ActualSource == null)
				this.SetValue(ActualSourcePropertyKey, new ObservableCollection<IDocumentMapItem>());
		}
		public DocumentMapControl() {
			DefaultStyleKeyHelper.SetDefaultStyleKey(this, typeof(DocumentMapControl));
			ActualSettings = CreateDefaultMapSettings();
		}
		protected virtual DocumentMapSettings CreateDefaultMapSettings() {
			return new DocumentMapSettings();
		}
		protected void ActualSourceChanged(IEnumerable<object> oldValue, IEnumerable<object> newValue) {
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			grid = GetTemplateChild(GridName) as GridControl;
		}
		protected override void OnGotFocus(RoutedEventArgs e) {
			base.OnGotFocus(e);
			grid.Do(x => x.Focus());
		}
		protected virtual void SettingsChanged(DocumentMapSettings oldValue, DocumentMapSettings newValue) {
			ActualSettings = CalcActualSettings(newValue);
		}
		protected virtual DocumentMapSettings CalcActualSettings(DocumentMapSettings settings) {
			return settings ?? CreateDefaultMapSettings();
		}
		protected virtual void ActualSettingsChanged(DocumentMapSettings oldValue, DocumentMapSettings newValue) {
			oldValue.Do(x => x.Invalidate -= InvalidateSettings);
			newValue.Do(x => x.Invalidate += InvalidateSettings);
			AssignSource(newValue);
		}
		void InvalidateSettings(object sender, EventArgs eventArgs) {
			ActualSettings.Do(AssignSource);
		}
		void AssignSource(DocumentMapSettings newValue) {
			ClearValue(WrapLongLinesProperty);
			ClearValue(HideAfterUseProperty);
			if (newValue == null) {
				ActualSource = new ObservableCollection<object>();
				return;
			}
			newValue.UpdateProperties();
			ActualSource = newValue.Source;
			WrapLongLines = newValue.WrapLongLines ?? WrapLongLines;
			HideAfterUse = newValue.HideAfterUse ?? HideAfterUse;
		}
		protected virtual void HighlightedItemChanged(object oldValue, object newValue) {
		}
		protected virtual void HideAfterUseChanged(bool oldValue, bool newValue) {
			ActualSettings.ActualHideAfterUse = newValue;
		}
	}
	public class MouseEventArgsToDataRowConverter : EventArgsToDataRowConverter {
		public MouseButton ChangedButton { get; set; }
		protected override object Convert(object sender, EventArgs args) {
			return (args as MouseButtonEventArgs).If(x => x.ChangedButton == ChangedButton).With(x => base.Convert(sender, x));
		}
	}
}
