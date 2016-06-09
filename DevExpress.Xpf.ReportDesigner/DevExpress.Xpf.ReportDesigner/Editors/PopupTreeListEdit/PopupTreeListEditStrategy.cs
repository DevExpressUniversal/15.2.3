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

using System.Windows;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Validation.Native;
namespace DevExpress.Xpf.Reports.UserDesigner.Editors {
	public class PopupTreeListEditStrategy : PopupBaseEditStrategy {
		new PopupTreeListEdit Editor { get { return (PopupTreeListEdit)base.Editor; } }
		new PopupTreeListEditSettings Settings { get { return (PopupTreeListEditSettings)base.Settings; } }
		PopupTreeListEditVisualClient VisualClient {
			get {
				return (PopupTreeListEditVisualClient)LookUpEditHelper.GetVisualClient(Editor);
			}
		}
		PostponedAction AcceptPopupValueAction { get; set; }
		public PopupTreeListEditStrategy(PopupTreeListEdit editor)
			: base(editor) {
			AcceptPopupValueAction = new PostponedAction(() => VisualClient.IsPopupOpened);
		}
		public virtual void AcceptPopupValue() {
			if(Editor.IsReadOnly)
				return;
			object selectedItem = VisualClient.GetSelectedItem();
			AcceptPopupValueAction.PerformPostpone(() => AcceptPopupValueInternal(selectedItem));
		}
		public virtual void PopupDestroyed() {
			VisualClient.PopupDestroyed();
			AcceptPopupValueAction.PerformForce();
		}
		protected virtual void AcceptPopupValueInternal(object selectedItem) {
			ValueContainer.SetEditValue(Settings.ItemsProvider.GetValue(selectedItem), UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		public virtual void ItemsSourceChanged(object newValue) {
			Settings.AssignToEditLocker.DoIfNotLocked(() => {
				Settings.ItemsSource = newValue;
			});
			VisualClient.SyncProperties(true);
		}
		public virtual void ValueMemberChanged(string newValue) {
			Settings.ValueMember = newValue;
		}
		public virtual void DisplayMemberChanged(string newValue) {
			Settings.DisplayMember = newValue;
		}
		public virtual void ChildNodesPathChanged(string newValue) {
			Settings.ChildNodesPath = newValue;
		}
		public virtual void HierarchicalPathProviderChanged(IHierarchicalPathProvider newValue) {
			Settings.HierarchicalPathProvider = newValue;
		}
		public virtual void PopupSelectionValidatorChanged(IPopupSelectionValidator newValue) {
			Settings.PopupSelectionValidator = newValue;
		}
		public virtual void PopupDisplayMemberChanged(string newValue) {
			Settings.PopupDisplayMember = newValue;
		}
		public virtual void TreeListCellTemplateChanged(DataTemplate newValue) {
			Settings.TreeListCellTemplate = newValue;
		}
		protected override object ConvertEditValueForFormatDisplayText(object convertedValue) {
			object item = Settings.ItemsProvider.GetItemByValue(base.ConvertEditValueForFormatDisplayText(convertedValue));
			return Settings.ItemsProvider.GetDisplayValue(item);
		}
		readonly Locker PopupSizeChangeLocker = new Locker();
		public virtual void SetInitialPopupSize() {
			PopupSizeChangeLocker.DoLockedAction(() => Editor.SetInitialPopupSizeInternal());
		}
	}
}
