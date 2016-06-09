#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.ComponentModel;
using System.Windows.Forms;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Localization;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Win.Core;
using DevExpress.XtraEditors;
namespace DevExpress.ExpressApp.Win.Editors {
	[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
	public class ColumnChooserExtenderBase : IDisposable {
		private IModelObjectView objectViewModel;
		private BaseButton addButton;
		private BaseButton removeButton;
		private bool canAddListProperty;
		private bool multiSelect;
		public ColumnChooserExtenderBase(IModelObjectView listViewModel) {
			this.objectViewModel = listViewModel;
			canAddListProperty = false;
			multiSelect = false;
		}
		public IModelObjectView ObjectViewModel {
			get { return objectViewModel; }
		}
		public BaseButton AddButton {
			get { return addButton; }
		}
		public BaseButton RemoveButton {
			get { return removeButton; }
		}
		public bool CanAddListProperty {
			get { return canAddListProperty; }
			set { canAddListProperty = value; }
		}
		public bool MultiSelect {
			get { return multiSelect; }
			set { multiSelect = value; }
		}
		public void CreateButtons() {
			addButton = new BaseButton();
			addButton.Name = "AddButton";
			addButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Add", "Add") + "...";
			addButton.Dock = DockStyle.Bottom;
			addButton.Click += new EventHandler(addButton_Click);
			removeButton = new BaseButton();
			removeButton.Name = "RemoveButton";
			removeButton.Text = CaptionHelper.GetLocalizedText("DialogButtons", "Remove", "Remove");
			removeButton.Dock = DockStyle.Bottom;
			removeButton.Click += new EventHandler(removeButton_Click);
		}
		public void DeleteButtons() {
			if(addButton != null) {
				addButton.Click -= new EventHandler(addButton_Click);
				addButton.Dispose();
				addButton = null;
			}
			if(removeButton != null) {
				removeButton.Click -= new EventHandler(removeButton_Click);
				removeButton.Dispose();
				removeButton = null;
			}
		}
		public virtual void AddButtonsToCustomizationForm() {
			int activeListBoxIndex = ActiveListBox.Parent.Controls.IndexOf(ActiveListBox);
			ActiveListBox.Parent.Controls.Add(AddButton);
			ActiveListBox.Parent.Controls.Add(RemoveButton);
			ActiveListBox.Parent.Controls.SetChildIndex(AddButton, activeListBoxIndex + 1);
			ActiveListBox.Parent.Controls.SetChildIndex(RemoveButton, activeListBoxIndex + 2);
		}
		protected void InsertButtons() {
			CreateButtons();
			AddButtonsToCustomizationForm();
		}
		protected virtual Control ActiveListBox {
			get {
				CustomActiveListBoxEventArgs args = new CustomActiveListBoxEventArgs();
				if(CustomActiveListBox != null) {
					CustomActiveListBox(this, args);
				}
				return args.ActiveListBox;
			}
		}
		protected virtual List<string> GetUsedProperties() {
			GetCustomUsedPropertiesEventArgs args = new GetCustomUsedPropertiesEventArgs();
			if(GetCustomUsedProperties != null) {
				GetCustomUsedProperties(this, args);
			}
			return args.UsedProperties;
		}
		protected virtual ITypeInfo DisplayedTypeInfo {
			get {
				CustomDisplayedTypeInfoEventArgs args = new CustomDisplayedTypeInfoEventArgs();
				if(CustomDisplayedTypeInfo != null) {
					CustomDisplayedTypeInfo(this, args);
				}
				return args.DisplayedTypeInfo;
			}
		}
		protected void AddColumn(string propertyName) {
			OnBeginUpdate();
			try {
				AddColumnCore(propertyName);
			}
			finally {
				OnEndUpdate();
			}
		}
		protected virtual void AddColumnCore(string propertyName) {
			if(CustomAddColumn != null) {
				CustomAddColumn(this, new ColumnChooserObserverAddColumnEventArgs(propertyName));
			}
		}
		protected void RemoveSelectedColumn() {
			OnBeginUpdate();
			try {
				RemoveSelectedColumnCore();
			}
			finally {
				OnEndUpdate();
			}
		}
		protected virtual void RemoveSelectedColumnCore() {
			if(CustomRemoveSelectedColumn != null) {
				CustomRemoveSelectedColumn(this, EventArgs.Empty);
			}
		}
		protected void OnBeginUpdate() {
			if(BeginUpdate != null) {
				BeginUpdate(this, EventArgs.Empty);
			}
		}
		protected void OnEndUpdate() {
			if(EndUpdate != null) {
				EndUpdate(this, EventArgs.Empty);
			}
		}
		private void addButton_Click(object sender, EventArgs e) {
			using(ModelBrowser modelBrowser = new ModelBrowser(DisplayedTypeInfo.Type, GetUsedProperties(), CanAddListProperty)) {
				modelBrowser.MultiSelect = multiSelect;
				if(modelBrowser.ShowDialog(((Control)sender).FindForm())) {
					ProcessSelectedMembers(modelBrowser.SelectedMembers);
				}
			}
		}
		private void ProcessSelectedMembers(IEnumerable<string> selectedMembers) {
			foreach(string selectedMember in selectedMembers) {
				IMemberInfo memberDescriptor = DisplayedTypeInfo.FindMember(selectedMember);
				if(!CanAddListProperty && memberDescriptor.IsList && !ModelNodesGeneratorBase.IsBinaryImage(memberDescriptor)) {
					throw new Exception(SystemExceptionLocalizer.GetExceptionMessage(ExceptionId.CannotAddCollectionProperty, CaptionHelper.GetFullMemberCaption(DisplayedTypeInfo, selectedMember)));
				}
				else {
					AddColumn(selectedMember);
				}
			}
		}
		private void removeButton_Click(object sender, EventArgs e) {
			RemoveSelectedColumn();
		}
		public event EventHandler BeginUpdate;
		public event EventHandler EndUpdate;
		public event EventHandler<ColumnChooserObserverAddColumnEventArgs> CustomAddColumn;
		public event EventHandler CustomRemoveSelectedColumn;
		public event EventHandler<GetCustomUsedPropertiesEventArgs> GetCustomUsedProperties;
		public event EventHandler<CustomDisplayedTypeInfoEventArgs> CustomDisplayedTypeInfo;
		public event EventHandler<CustomActiveListBoxEventArgs> CustomActiveListBox;
		#region IDisposable Members
		public void Dispose() {
			DeleteButtons();
			DisposeCore();
		}
		protected virtual void DisposeCore() { }
		#endregion
		#region Obsolete 15.1
		[Obsolete("Use the OnBeginUpdate method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void OnBeginApdate() {
			OnBeginUpdate();
		}
		[Obsolete("Use the OnEndUpdate method instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected void OnEndApdate() {
			OnEndUpdate();
		}
		[Obsolete("Use the BeginUpdate event instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler BeginApdate {
			add { BeginUpdate += value; }
			remove { BeginUpdate -= value; }
		}
		[Obsolete("Use the EndUpdate event instead."), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event EventHandler EndApdate {
			add { EndUpdate += value; }
			remove { EndUpdate -= value; }
		}
		#endregion
#if DebugTest
		public void DebugTest_ProcessSelectedMembers(IEnumerable<string> selectedMembers) {
			ProcessSelectedMembers(selectedMembers);
		}
#endif
	}
}
