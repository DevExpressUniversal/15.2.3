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

using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using DevExpress.Mvvm.Native;
using DevExpress.Pdf.Drawing;
using DevExpress.Xpf.DocumentViewer;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.PdfViewer {
	public enum PdfOutlinesViewerState {
		Collapsed,
		Visible,
		Expanded,
	}
	public enum PdfOutlinesViewerPanelStyle {
		Popup,
		DockPanel,
		Tab,
	}
	public class PdfOutlinesViewerSettings : DocumentMapSettings {
		public static readonly DependencyProperty HideOutlinesViewerProperty;
		public static readonly DependencyProperty OutlinesViewerStateProperty;
		public static readonly DependencyProperty OutlinesViewerInitialStateProperty;
		public static readonly DependencyProperty OutlinesViewerPanelStyleProperty;
		public static readonly DependencyProperty OutlinesViewerStyleProperty;
		public static readonly DependencyProperty ApplyOutlinesForegroundProperty;
		static PdfOutlinesViewerSettings() {
			HideOutlinesViewerProperty = DependencyPropertyRegistrator.Register<PdfOutlinesViewerSettings, bool>(owner => owner.HideOutlinesViewer, false,
				(control, value, newValue) => control.HideOutlinesViewerChanged(value, newValue));
			OutlinesViewerStateProperty = DependencyPropertyRegistrator.Register<PdfOutlinesViewerSettings, PdfOutlinesViewerState>(
				owner => owner.OutlinesViewerState, PdfOutlinesViewerState.Collapsed, (settings, value, newValue) => settings.OutlinesViewerStateChanged(value, newValue));
			OutlinesViewerInitialStateProperty = DependencyPropertyRegistrator.Register<PdfOutlinesViewerSettings, PdfOutlinesViewerState?>(owner => owner.OutlinesViewerInitialState, null,
				(control, value, newValue) => control.OutlinesViewerInitialStateChanged(value, newValue));
			OutlinesViewerPanelStyleProperty = DependencyPropertyRegistrator.Register<PdfOutlinesViewerSettings, PdfOutlinesViewerPanelStyle>(owner => owner.OutlinesViewerPanelStyle, PdfOutlinesViewerPanelStyle.Tab,
				(control, value, newValue) => control.OutlinesViewerStyleChanged(value, newValue));
			ApplyOutlinesForegroundProperty = DependencyPropertyRegistrator.Register<PdfOutlinesViewerSettings, bool>(owner => owner.ApplyOutlinesForeground, false,
				(control, value, newValue) => control.ApplyOutlinesForegroundChanged(value, newValue));
			OutlinesViewerStyleProperty = DependencyPropertyRegistrator.Register<PdfOutlinesViewerSettings, Style>(owner => owner.OutlinesViewerStyle, null);
		}
		public bool ApplyOutlinesForeground {
			get { return (bool)GetValue(ApplyOutlinesForegroundProperty); }
			set { SetValue(ApplyOutlinesForegroundProperty, value); }
		}
		public PdfOutlinesViewerState OutlinesViewerState {
			get { return (PdfOutlinesViewerState)GetValue(OutlinesViewerStateProperty); }
			set { SetValue(OutlinesViewerStateProperty, value); }
		}
		public PdfOutlinesViewerState? OutlinesViewerInitialState {
			get { return (PdfOutlinesViewerState?)GetValue(OutlinesViewerInitialStateProperty); }
			set { SetValue(OutlinesViewerInitialStateProperty, value); }
		}
		public PdfOutlinesViewerPanelStyle OutlinesViewerPanelStyle {
			get { return (PdfOutlinesViewerPanelStyle)GetValue(OutlinesViewerPanelStyleProperty); }
			set { SetValue(OutlinesViewerPanelStyleProperty, value); }
		}
		public Style OutlinesViewerStyle {
			get { return (Style)GetValue(OutlinesViewerStyleProperty); }
			set { SetValue(OutlinesViewerStyleProperty, value); }
		}
		public bool HideOutlinesViewer {
			get { return (bool)GetValue(HideOutlinesViewerProperty); }
			set { SetValue(HideOutlinesViewerProperty, value); }
		}
		public ICommand PrintCommand { get; protected set; }
		public ICommand PrintSectionCommand { get; protected set; }
		new PdfViewerControl Owner { get { return base.Owner as PdfViewerControl; } }
		public PdfOutlinesViewerSettings() {
		}
		protected virtual void OutlinesViewerStyleChanged(PdfOutlinesViewerPanelStyle oldValue, PdfOutlinesViewerPanelStyle newValue) {
		}
		protected virtual void OutlinesViewerInitialStateChanged(PdfOutlinesViewerState? oldValue, PdfOutlinesViewerState? newValue) {
		}
		protected virtual void HideOutlinesViewerChanged(bool oldValue, bool newValue) {
			Owner.Do(x => x.UpdateHasOutlines(newValue));
		}
		protected override void UpdatePropertiesInternal() {
			base.UpdatePropertiesInternal();
			if (!Owner.With(x => x.Document).If(x => x.IsLoaded).ReturnSuccess()) {
				Source = null;
				GoToCommand = null;
				PrintCommand = null;
				PrintSectionCommand = null;
				return;
			}
			var model = Owner.Document;
			Source = new ReadOnlyObservableCollection<PdfOutlineViewerNode>(model.Return(x => new ObservableCollection<PdfOutlineViewerNode>(x.CreateOutlines()), () => new ObservableCollection<PdfOutlineViewerNode>()));
			Source.ForEach(x => ((PdfOutlineViewerNode)x).UseForeColor = ApplyOutlinesForeground);
			GoToCommand = Owner.ActualCommandProvider.NavigateCommand;
			PrintCommand = DelegateCommandFactory.Create<object>(
				parameter => Print(parameter, false),
				parameter => CanPrint(parameter, false));
			PrintSectionCommand = DelegateCommandFactory.Create<object>(
				parameter => Print(parameter, true),
				parameter => CanPrint(parameter, true));
			if (OutlinesViewerInitialState != null)
				UpdateOutlinesViewerCurrentState();
		}
		protected virtual void UpdateOutlinesViewerCurrentState() {
			OutlinesViewerState = OutlinesViewerInitialState ?? OutlinesViewerState;
		}
		protected virtual bool CanPrint(object parameter, bool isRange) {
			if (!Owner.With(x => x.Document).If(x => x.IsLoaded).ReturnSuccess())
				return false;
			var outlines = (parameter as IEnumerable).With(x => x.Cast<PdfOutlineViewerNode>().ToList());
			return Owner.Document.CanPrintPages(outlines, isRange);
		}
		protected virtual void Print(object parameter, bool isRange) {
			var outlines = (parameter as IEnumerable).With(x => x.Cast<PdfOutlineViewerNode>().ToList());
			if (outlines == null)
				return;
			Owner.Print(Owner.Document.CalcPrintPages(outlines, isRange));
		}
		protected virtual void ApplyOutlinesForegroundChanged(bool oldValue, bool newValue) {
			RaiseInvalidate();
		}
		protected virtual void OutlinesViewerStateChanged(PdfOutlinesViewerState oldValue, PdfOutlinesViewerState newValue) {
		}
	}
}
