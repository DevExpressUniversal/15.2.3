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
using System.Windows.Data;
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Xpf.Core;
namespace DevExpress.Xpf.Reports.UserDesigner.Native {
	public abstract class ReportDesignerDocumentSelectorBase<T> : Behavior<T>, IReportDesignerDocumentViewSource where T : DependencyObject {
		public static readonly DependencyProperty SelectedDocumentViewSourceProperty;
		static readonly DependencyPropertyKey SelectedDocumentPropertyKey;
		public static readonly DependencyProperty SelectedDocumentProperty;
		public static readonly DependencyProperty ViewKindProperty;
		public static readonly DependencyProperty DesignerPageIsSelectedProperty;
		public static readonly DependencyProperty PreviewPageIsSelectedProperty;
		public static readonly DependencyProperty ScriptEditorIsShownProperty;
		static ReportDesignerDocumentSelectorBase() {
			DependencyPropertyRegistrator<ReportDesignerDocumentSelectorBase<T>>.New()
				.Register(d => d.SelectedDocumentViewSource, out SelectedDocumentViewSourceProperty, null, (d, e) => d.OnSelectedDocumentViewSourceChanged(e))
				.RegisterReadOnly(d => d.SelectedDocument, out SelectedDocumentPropertyKey, out SelectedDocumentProperty, null, d => d.OnSelectedDocumentChanged())
				.Register(d => d.ViewKind, out ViewKindProperty, ReportDesignerDocumentViewKind.Designer, d => d.OnViewKindChanged())
				.Register(d => d.DesignerPageIsSelected, out DesignerPageIsSelectedProperty, true, d => d.OnDesignerPageIsSelectedChanged())
				.Register(d => d.PreviewPageIsSelected, out PreviewPageIsSelectedProperty, false, d => d.OnPreviewPageIsSelectedChanged())
				.Register(d => d.ScriptEditorIsShown, out ScriptEditorIsShownProperty, false, d => d.OnScriptEditorIsShownChanged())
			;
		}
		public ReportDesignerDocumentSelectorBase() {
			selectedDocumentViewSourceTracker = new ReportDesignerDocumentViewSourceDocumentTracker(x => SelectedDocument = x);
			BindingOperations.SetBinding(this, ViewKindProperty, new Binding() { Path = new PropertyPath("(0).(1)", SelectedDocumentProperty, ReportDesignerDocument.ViewKindProperty), Source = this, Mode = BindingMode.TwoWay });
		}
		public ReportDesignerDocumentViewKind ViewKind {
			get { return (ReportDesignerDocumentViewKind)GetValue(ViewKindProperty); }
			set { SetValue(ViewKindProperty, value); }
		}
		public bool DesignerPageIsSelected {
			get { return (bool)GetValue(DesignerPageIsSelectedProperty); }
			set { SetValue(DesignerPageIsSelectedProperty, value); }
		}
		public bool PreviewPageIsSelected {
			get { return (bool)GetValue(PreviewPageIsSelectedProperty); }
			set { SetValue(PreviewPageIsSelectedProperty, value); }
		}
		public bool ScriptEditorIsShown {
			get { return (bool)GetValue(ScriptEditorIsShownProperty); }
			set { SetValue(ScriptEditorIsShownProperty, value); }
		}
		public ReportDesignerDocumentViewSource SelectedDocumentViewSource {
			get { return (ReportDesignerDocumentViewSource)GetValue(SelectedDocumentViewSourceProperty); }
			set { SetValue(SelectedDocumentViewSourceProperty, value); }
		}
		public ReportDesignerDocument SelectedDocument {
			get { return (ReportDesignerDocument)GetValue(SelectedDocumentProperty); }
			private set { SetValue(SelectedDocumentPropertyKey, value); }
		}
		readonly ReportDesignerDocumentViewSourceDocumentTracker selectedDocumentViewSourceTracker;
		protected override void OnAttached() {
			base.OnAttached();
			ReportDesigner.SetDocumentSelector(AssociatedObject, this);
			BindingOperations.SetBinding(AssociatedObject, ReportDesigner.DocumentProperty, new Binding() { Path = new PropertyPath(SelectedDocumentProperty), Source = this, Mode = BindingMode.OneWay });
		}
		protected override void OnDetaching() {
			base.OnDetaching();
			BindingOperations.ClearBinding(AssociatedObject, ReportDesigner.DocumentProperty);
			ReportDesigner.SetDocumentSelector(AssociatedObject, null);
		}
		EventHandler documentChanged;
		event EventHandler IReportDesignerDocumentViewSource.DocumentChanged {
			add { documentChanged += value; }
			remove { documentChanged -= value; }
		}
		ReportDesignerDocument IReportDesignerDocumentViewSource.Document { get { return SelectedDocument; } }
		void OnSelectedDocumentChanged() {
			if(documentChanged != null)
				documentChanged(this, EventArgs.Empty);
		}
		void OnSelectedDocumentViewSourceChanged(DependencyPropertyChangedEventArgs e) {
			SelectedDocumentViewSource.Do(x => x.AttachedSelector = this);
			selectedDocumentViewSourceTracker.Update(e);
		}
		void OnViewKindChanged() {
			switch(ViewKind) {
				case ReportDesignerDocumentViewKind.Preview:
					PreviewPageIsSelected = true;
					ScriptEditorIsShown = false;
					break;
				case ReportDesignerDocumentViewKind.Designer:
					DesignerPageIsSelected = true;
					ScriptEditorIsShown = false;
					break;
				case ReportDesignerDocumentViewKind.ScriptEditor:
					DesignerPageIsSelected = true;
					ScriptEditorIsShown = true;
					break;
			}
		}
		void OnDesignerPageIsSelectedChanged() {
			if(!DesignerPageIsSelected) return;
			if(ViewKind == ReportDesignerDocumentViewKind.Preview)
				ViewKind = ReportDesignerDocumentViewKind.Designer;
		}
		void OnPreviewPageIsSelectedChanged() {
			if(!PreviewPageIsSelected) return;
			ViewKind = ReportDesignerDocumentViewKind.Preview;
		}
		void OnScriptEditorIsShownChanged() {
			if(ScriptEditorIsShown) {
				ViewKind = ReportDesignerDocumentViewKind.ScriptEditor;
			} else {
				if(ViewKind == ReportDesignerDocumentViewKind.ScriptEditor)
					ViewKind = ReportDesignerDocumentViewKind.Designer;
			}
		}
	}
}
