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
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Xpf.Spreadsheet.Extensions;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet;
using System.Windows;
namespace DevExpress.Xpf.Spreadsheet.Internal {
	public class SpreadsheetNameBoxControl : ComboBoxEdit, INameBoxControllerOwner, INameBoxControl {
		public static readonly DependencyProperty SpreadsheetControlProperty;
		static SpreadsheetNameBoxControl() {
			SpreadsheetControlProperty = DependencyProperty.Register("SpreadsheetControl", typeof(ISpreadsheetControl), typeof(SpreadsheetNameBoxControl),
				new FrameworkPropertyMetadata((d, e) => ((SpreadsheetNameBoxControl)d).OnSpreadsheetChanged(e.OldValue as ISpreadsheetControl)));
		}
		public SpreadsheetNameBoxControl() {
			Initialize();
			this.Loaded += SpreadsheetNameBoxControlLoaded;
		}
		public ISpreadsheetControl SpreadsheetControl {
			get { return (ISpreadsheetControl)GetValue(SpreadsheetControlProperty); }
			set { SetValue(SpreadsheetControlProperty, value); }
		}
		NameBoxController Controller { get; set; }
		bool INameBoxControllerOwner.SelectionMode { get; set; }
		bool INameBoxControl.SelectionMode { get; set; }
		SpreadsheetControl Control { get { return SpreadsheetControl as SpreadsheetControl; } }
		protected internal DocumentModel Workbook {
			get { return Control != null ? Control.DocumentModel : null; }
		}
		protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e) {
			base.OnKeyDown(e);
			Controller.OnNameBoxKeyDown(this, e.ToPlatformIndependent());
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			SetSpreadsheetToController();
		}
		private void OnSpreadsheetChanged(ISpreadsheetControl oldValue) {
			if (SpreadsheetControl != null) {
				if (oldValue != null) {
					RemoveNameBoxControlService();
					UnSubcribeSpreadsheetEvents();
				}
				RegistrateNameBoxControlService();
				SetSpreadsheetToController();
				SubcribeSpreadsheetEvents();
			}
		}
		private void UnSubcribeSpreadsheetEvents() {
			Control.InnerControlInitialized -= InnerControlInitialized;
		}
		private void SubcribeSpreadsheetEvents() {
			Control.InnerControlInitialized += InnerControlInitialized;
		}
		private void InnerControlInitialized(object sender, EventArgs e) {
			SetSpreadsheetToController();
		}
		void SpreadsheetNameBoxControlLoaded(object sender, RoutedEventArgs e) {
			SetSpreadsheetToController();
		}
		private void SetSpreadsheetToController() {
			if (Controller != null && SpreadsheetControl != null && Workbook != null && Workbook.ActiveSheet != null) Controller.SpreadsheetControl = SpreadsheetControl;
		}
		private void RemoveNameBoxControlService() {
			SpreadsheetControl.RemoveService(typeof(INameBoxControl));
		}
		private void RegistrateNameBoxControlService() {
			SpreadsheetControl.AddService(typeof(INameBoxControl), this);
		}
		private void Initialize() {
			UnSubcribeEvents();
			Controller = new NameBoxController(this);
			SubcribeEvents();
		}
		private void UnSubcribeEvents() {
			if (Controller != null) {
				Controller.VisibleDefinedNamesChanged -= OnControllerVisibleDefinedNamesChanged;
				Controller.SelectionChanged -= OnControllerSelectionChanged;
			}
		}
		private void SubcribeEvents() {
			Controller.VisibleDefinedNamesChanged += OnControllerVisibleDefinedNamesChanged;
			Controller.SelectionChanged += OnControllerSelectionChanged;
		}
		private void OnControllerSelectionChanged(object sender, EventArgs e) {
			ChangeNameBoxText();
		}
		void ChangeNameBoxText() {
			EditValue = Controller.OwnersText;
			SelectionLength = 0;
		}
		private void OnControllerVisibleDefinedNamesChanged(object sender, VisibleDefinedNamesChangedEventArgs e) {
			Items.Clear();
			if (e.VisibleDefinedNames.Count != 0)
				AddVisibleDefinedNames(e.VisibleDefinedNames);
		}
		private void AddVisibleDefinedNames(List<string> list) {
			foreach (string item in list)
				Items.Add(new ComboBoxEditItem() { Content = item });
		}
		#region INameBoxControllerOwner Members
		protected override void OnSelectedIndexChanged(int oldSelectedIndex, int selectedIndex) {
			base.OnSelectedIndexChanged(oldSelectedIndex, selectedIndex);
			RaiseSelectionIndexChanged(selectedIndex);
		}
		private void RaiseSelectionIndexChanged(int selectedIndex) {
			if (selectedIndexChanged != null)
				selectedIndexChanged(this, EventArgs.Empty);
		}
		EventHandler selectedIndexChanged;
		event EventHandler INameBoxControllerOwner.SelectedIndexChanged {
			add { selectedIndexChanged += value; }
			remove { selectedIndexChanged -= value; }
		}
		#endregion
	}
	public static class NameBoxButtonsBehavior {
		public static readonly DependencyProperty ButtonsProperty;
		static NameBoxButtonsBehavior() {
			ButtonsProperty = DependencyProperty.RegisterAttached("Buttons", typeof(ButtonInfoCollection), typeof(NameBoxButtonsBehavior),
				new FrameworkPropertyMetadata(null, OnButtonsChanged));
		}
		public static ButtonInfoCollection GetButtons(DependencyObject d) {
			return (ButtonInfoCollection)d.GetValue(ButtonsProperty);
		}
		public static void SetIsBroughtIntoViewWhenSelected(DependencyObject d, ButtonInfoCollection value) {
			d.SetValue(ButtonsProperty, value);
		}
		private static void OnButtonsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			SpreadsheetNameBoxControl namebox = d as SpreadsheetNameBoxControl;
			if (namebox != null) {
				var buttons = e.NewValue as ButtonInfoCollection;
				if (buttons == null) return;
				namebox.Buttons.Clear();
				foreach (var buttonInfo in buttons)
					namebox.Buttons.Add(((ICloneable)buttonInfo).Clone() as ButtonInfo);
			}
		}
	}
}
