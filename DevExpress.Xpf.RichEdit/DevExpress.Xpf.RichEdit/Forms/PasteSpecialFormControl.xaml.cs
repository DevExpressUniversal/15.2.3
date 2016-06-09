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
using DevExpress.XtraRichEdit.Commands;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Xpf.RichEdit;
namespace DevExpress.XtraRichEdit.Forms {
	public partial class PasteSpecialFormControl : UserControl, IDialogContent {
		readonly PasteSpecialFormController controller;
		readonly IRichEditControl control;
		public PasteSpecialFormControl() {
			InitializeComponent();
		}
		public PasteSpecialFormControl(PasteSpecialFormControllerParameters controllerParameters) {
			Guard.ArgumentNotNull(controllerParameters, "controllerParameters");
			this.control = controllerParameters.Control;
			this.controller = CreateController(controllerParameters);
			Loaded += OnLoaded;
			InitializeComponent();
			SubscribeControlsEvents();
		}
		#region Properties
		protected internal PasteSpecialFormController Controller { get { return controller; } }
		public IRichEditControl Control { get { return control; } }
		#endregion
		public void SubscribeControlsEvents() {
#if !SL
			lbCommandType.MouseDoubleClick += OnLbDocumentFormatDoubleClick;
#endif
			lbCommandType.SelectedIndexChanged += OnLbDocumentFormatSelectedIndexChanged;
		}
		public void UnsubscribeControlsEvents() {
#if !SL
			lbCommandType.MouseDoubleClick -= OnLbDocumentFormatDoubleClick;
#endif
			lbCommandType.SelectedIndexChanged -= OnLbDocumentFormatSelectedIndexChanged;
		}
		void OnLbDocumentFormatSelectedIndexChanged(object sender, EventArgs e) {
			CommitValuesToController();
		}
		void OnLbDocumentFormatDoubleClick(object sender, EventArgs e) {
			ApplyChanges();
			IDialogOwner owner = FloatingContainer.GetDialogOwner(this);
			if (owner != null)
				owner.CloseDialog(true);
		}
		protected internal virtual PasteSpecialFormController CreateController(PasteSpecialFormControllerParameters controllerParameters) {
			return new PasteSpecialFormController(controllerParameters);
		}
		void OnLoaded(object sender, RoutedEventArgs e) {
			InitializeForm();
			UpdateForm();
		}
		void InitializeForm() {
			ListItemCollection items = lbCommandType.Items;
			items.BeginUpdate();
			try {
				items.Clear();
				IList<Type> availableCommandTypes = Controller.AvailableCommandTypes;
				int count = availableCommandTypes.Count;
				for (int i = 0; i < count; i++) {
					PasteSpecialListBoxItem item = new PasteSpecialListBoxItem(control, availableCommandTypes[i]);
					items.Add(item);
				}
			}
			finally {
				items.EndUpdate();
			}
			if (items.Count > 0)
				lbCommandType.SelectedIndex = 0;
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
		protected virtual void UpdateFormCore() {
			if (Controller == null)
				return;
		}
		protected internal virtual void CommitValuesToController() {
			if (Controller == null)
				return;
			PasteSpecialListBoxItem item = lbCommandType.SelectedItem as PasteSpecialListBoxItem;
			if (item != null)
				Controller.PasteCommandType = item.CommandType;
		}
		protected internal virtual void ApplyChanges() {
			if (Controller != null) {
				CommitValuesToController();
				Controller.ApplyChanges();
			}
		}
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
