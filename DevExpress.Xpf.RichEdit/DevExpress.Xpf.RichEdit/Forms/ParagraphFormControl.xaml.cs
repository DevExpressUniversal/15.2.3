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
using DevExpress.Utils;
using DevExpress.XtraRichEdit.Design.Internal;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.RichEdit;
using DevExpress.Xpf.RichEdit.Localization;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class ParagraphFormControl : UserControl, IDialogContent {
		readonly RichEditControl control;
		readonly ParagraphFormController controller;
		public ParagraphFormControl() {
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
		}
		public ParagraphFormControl(ParagraphFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = (RichEditControl)controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			Unloaded -= OnUnloaded;
			InitializeComponent();
			InitializeForm();
			SubscribeControlsEvents();
			UpdateForm();
		}
		public ParagraphFormController Controller { get { return controller; } }
		protected internal virtual ParagraphFormController CreateController(ParagraphFormControllerParameters controllerParameters) {
			return new ParagraphFormController(controllerParameters);
		}
		void InitializeForm() {
			if (control != null)
				this.paragraphIndentationControl.Properties.UnitType = (control.Unit == DocumentUnit.Document) ? DocumentUnit.Inch : control.Unit;
			this.paragraphSpacingControl.Properties.UnitType = DocumentUnit.Point;
			this.paragraphSpacingControl.Properties.MaxSpacing = ParagraphFormDefaults.MaxSpacingByDefault;
			IList<string> outlineLevelItems = Controller.OutlineLevelItems;
			ListItemCollection items = edtOutlineLevel.Items;
			int count = outlineLevelItems.Count;
			for (int i = 0; i < count; i++)
				items.Add(outlineLevelItems[i]);
		}
		Button tabsButton;
		void OnLoaded(object sender, RoutedEventArgs e) {
			if (this.tabsButton == null) {
				this.tabsButton = CreateTabsButton();
				if (this.tabsButton != null)
					this.tabsButton.Click += TabsClick;
			}
			UpdateForm();
		}
		void OnUnloaded(object sender, RoutedEventArgs e) {
			if (this.tabsButton != null) {
				this.tabsButton.Click -= TabsClick;
				this.tabsButton = null;
			}
		}
		Button CreateTabsButton() {
			if (!Controller.CanEditTabs)
				return null;
#if SL
			DXDialog dialog = FloatingContainer.GetDialogOwner(this) as DXDialog;
			if (dialog != null) {
				Panel footer = dialog.OkButton.Parent as Panel;
				if (footer != null) {
					Button tabsBtn = new Button();
					tabsBtn.Content = XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_ParagraphFormTabsButton);
					StyleManager.SetApplyApplicationTheme(tabsBtn, true);
					footer.Children.Add(tabsBtn);
					return tabsBtn;
				}
			}
#else
			FloatingContainer container = FloatingContainer.GetDialogOwner(this) as FloatingContainer;
			if (container != null) {
				DialogControl dialog = container.Content as DialogControl;
				if (dialog != null) {
					dialog.ShowApplyButton = true;
					dialog.ApplyButton.Content = XpfRichEditLocalizer.GetString(RichEditControlStringId.Caption_ParagraphFormTabsButton);
					return dialog.ApplyButton;
				}
			}
#endif
			return null;
		}
		void TabsClick(object sender, RoutedEventArgs e) {
			Controller.ApplyChanges();
			IDialogOwner owner = FloatingContainer.GetDialogOwner(this);
			if (owner != null)
				owner.CloseDialog(true);
			ShowTabsFormCommand showTabsFormCommand = new ShowTabsFormCommand(control);
			showTabsFormCommand.Execute();
		}
		protected internal virtual void UpdateForm() {
			UnsubscribeControlsEvents();
			try {
				UpdateFormCore();
			}
			finally {
				SubscribeControlsEvents();
			}
		}
		#region SubscribeControlsEvents
		protected internal virtual void SubscribeControlsEvents() {
			edtAlignment.EditValueChanged += ContentChanged;
			edtOutlineLevel.SelectedIndexChanged += ContentChanged;
			paragraphIndentationControl.ParagraphIndentControlChanged += ContentChanged;
			paragraphSpacingControl.ParagraphSpacingControlChanged += ContentChanged;
			chkKeepLinesTogether.EditValueChanged += OnParagraphKeepLinesTogetherChanged;
			chkPageBreakBefore.EditValueChanged += OnParagraphPageBreakBeforeChanged;
			chkContextualSpacing.EditValueChanged += OnContextualSpacingChanged;
		}
		#endregion
		#region UnsubscribeControlsEvents
		protected internal virtual void UnsubscribeControlsEvents() {
			edtAlignment.EditValueChanged -= ContentChanged;
			edtOutlineLevel.SelectedIndexChanged -= ContentChanged;
			paragraphIndentationControl.ParagraphIndentControlChanged -= ContentChanged;
			paragraphSpacingControl.ParagraphSpacingControlChanged -= ContentChanged;
			chkKeepLinesTogether.EditValueChanged -= OnParagraphKeepLinesTogetherChanged;
			chkPageBreakBefore.EditValueChanged -= OnParagraphPageBreakBeforeChanged;
			chkContextualSpacing.EditValueChanged -= OnContextualSpacingChanged;
		}
		#endregion
		void ContentChanged(object sender, EditValueChangedEventArgs e) {
			AssignValuesToController();
		}
		void ContentChanged(object sender, EventArgs e) {
			AssignValuesToController();
		}
		void OnParagraphKeepLinesTogetherChanged(object sender, RoutedEventArgs e) {
			Controller.KeepLinesTogether = chkKeepLinesTogether.IsChecked;
		}
		void OnContextualSpacingChanged(object sender, RoutedEventArgs e) {
			Controller.ContextualSpacing = chkContextualSpacing.IsChecked;
		}
		void OnParagraphPageBreakBeforeChanged(object sender, RoutedEventArgs e) {
			Controller.PageBreakBefore = chkPageBreakBefore.IsChecked;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null) {
				Controller.ApplyChanges();
			}
		}
		protected internal virtual void AssignValuesToController() {
			Controller.Alignment = edtAlignment.Alignment;
			Controller.FirstLineIndentType = paragraphIndentationControl.Properties.FirstLineIndentType;
			Controller.FirstLineIndent = paragraphIndentationControl.Properties.FirstLineIndent;
			Controller.LeftIndent = paragraphIndentationControl.Properties.LeftIndent;
			Controller.RightIndent = paragraphIndentationControl.Properties.RightIndent;
			Controller.SpacingAfter = paragraphSpacingControl.Properties.SpacingAfter;
			Controller.SpacingBefore = paragraphSpacingControl.Properties.SpacingBefore;
			Controller.LineSpacing = paragraphSpacingControl.Properties.LineSpacing;
			Controller.LineSpacingType = paragraphSpacingControl.Properties.LineSpacingType;
			if (edtOutlineLevel.SelectedIndex >= 0)
				Controller.OutlineLevel = edtOutlineLevel.SelectedIndex;
		}
		#region UpdateFormCore
		protected virtual void UpdateFormCore() {
			paragraphIndentationControl.BeginUpdate();
			try {
				ParagraphIndentationProperties properties = paragraphIndentationControl.Properties;
				properties.LeftIndent = Controller.LeftIndent;
				properties.RightIndent = Controller.RightIndent;
				properties.FirstLineIndent = Controller.FirstLineIndent;
				properties.FirstLineIndentType = Controller.FirstLineIndentType;
			}
			finally {
				paragraphIndentationControl.EndUpdate();
			}
			paragraphSpacingControl.BeginUpdate();
			try {
				ParagraphSpacingProperties properties = paragraphSpacingControl.Properties;
				properties.SpacingAfter = Controller.SpacingAfter;
				properties.SpacingBefore = Controller.SpacingBefore;
				properties.LineSpacing = Controller.LineSpacing;
				properties.LineSpacingType = Controller.LineSpacingType;
			}
			finally {
				paragraphSpacingControl.EndUpdate();
			}
			edtAlignment.EditValue = Controller.Alignment;
			chkContextualSpacing.IsChecked = Controller.ContextualSpacing;
			chkContextualSpacing.IsThreeState = !Controller.ContextualSpacing.HasValue;
			if (Controller.OutlineLevel.HasValue) {
				int level = Controller.OutlineLevel.Value;
				if (level < 0 || level > 9)
					level = 0;
				edtOutlineLevel.SelectedIndex = level;
			}
			else
				edtOutlineLevel.SelectedIndex = -1;
			chkKeepLinesTogether.IsChecked = Controller.KeepLinesTogether;
			chkKeepLinesTogether.IsThreeState = !Controller.KeepLinesTogether.HasValue;
			chkPageBreakBefore.IsChecked = Controller.PageBreakBefore;
			chkPageBreakBefore.IsThreeState = !Controller.PageBreakBefore.HasValue;
		}
		#endregion
		#region IDialogContent Members
		bool IDialogContent.CanCloseWithOKResult() {
			return true;
		}
		void IDialogContent.OnApply() {
			ApplyChanges();
		}
		void IDialogContent.OnOk() {
			ApplyChanges();
		}
		#endregion
	}
}
