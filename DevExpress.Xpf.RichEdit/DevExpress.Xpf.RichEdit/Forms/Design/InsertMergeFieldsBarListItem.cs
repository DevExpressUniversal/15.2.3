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
using System.Windows.Input;
using DevExpress.Data;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.RichEdit;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using DevExpress.XtraRichEdit.Internal;
using DevExpress.XtraRichEdit.Native;
using DevExpress.Utils.Controls;
using DevExpress.XtraRichEdit.Model;
using DevExpress.XtraRichEdit.API.Native;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Utils.Commands;
using DevExpress.Xpf.Bars.Native;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.RichEdit.UI {
	#region InsertMergeFieldsBarListItem
	public class InsertMergeFieldsBarListItem : BarListItem {
		static InsertMergeFieldsBarListItem() {
			BarItemLinkCreator.Default.RegisterObject(typeof(InsertMergeFieldsBarListItem), typeof(InsertMergeFieldsBarListItemLink), delegate(object arg) { return new InsertMergeFieldsBarListItemLink(); });
		}
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(InsertMergeFieldsBarListItem), new FrameworkPropertyMetadata(new PropertyChangedCallback(OnRichEditControlChanged)));
		protected static void OnRichEditControlChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			InsertMergeFieldsBarListItem instance = d as InsertMergeFieldsBarListItem;
			if (instance != null)
				instance.OnRichEditControlChanged((RichEditControl)e.OldValue, (RichEditControl)e.NewValue);
		}
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		protected internal virtual void OnRichEditControlChanged(RichEditControl oldValue, RichEditControl newValue) {
			UnsubscribeRichEditControlEvents(oldValue);
			UpdateItems();
			SubscribeRichEditControlEvents(newValue);
		}
		public override BarItemLink CreateLink(bool isPrivate) {
			return new InsertMergeFieldsBarListItemLink();
		}
		protected internal virtual void SubscribeRichEditControlEvents(RichEditControl control) {
			RichEditMailMergeOptions options = GetMailMergeOptions(control);
			if (options != null)
				options.Changed += OnMailMergeChanged;
		}
		protected internal virtual void UnsubscribeRichEditControlEvents(RichEditControl control) {
			RichEditMailMergeOptions options = GetMailMergeOptions(control);
			if (options != null)
				options.Changed -= OnMailMergeChanged;
		}
		protected internal virtual RichEditMailMergeOptions GetMailMergeOptions(RichEditControl control) {
			if (control == null)
				return null;
			InnerRichEditControl innerControl = control.InnerControl;
			if (innerControl == null)
				return null;
			RichEditControlOptionsBase options = innerControl.Options;
			if (options == null)
				return null;
			return options.MailMerge;
		}
		void OnMailMergeChanged(object sender, BaseOptionChangedEventArgs e) {
			if (e.Name == "DataSource" || e.Name == "DataMember")
				UpdateItems();
		}
		protected override void UpdateItems() {
			InternalItems.BeginUpdate();
			try {
				InternalItems.Clear();
				if (RichEditControl != null)
					UpdateItemsCore();
			}
			finally {
				InternalItems.EndUpdate();
			}
		}
		protected internal virtual void UpdateItemsCore() {
			MergeFieldName[] fieldNames = RichEditControl.DocumentModel.GetDatabaseFieldNames();
			fieldNames = RichEditControl.InnerControl.RaiseCustomizeMergeFields(new CustomizeMergeFieldsEventArgs(fieldNames));
			foreach (MergeFieldName fieldName in fieldNames) {
				AppendItem(fieldName);
			}
		}
		protected internal virtual void AppendItem(MergeFieldName fieldName) {
			BarButtonItem item = new BarButtonItem();
			item.Content = fieldName.DisplayName;
			item.Command = CreateInsertMailMergeFieldCommand(fieldName.Name);
			item.CommandParameter = RichEditControl;
			InternalItems.Add(item);
		}
		protected internal ICommand CreateInsertMailMergeFieldCommand(string name) {
			return new InsertMailMergeFieldRichEditUICommand(name);
		}
	}
	#endregion
	#region InsertMergeFieldsBarListItemLink
	public class InsertMergeFieldsBarListItemLink : BarListItemLink {
	}
	#endregion
	#region InsertMergeFieldsBarSubItem
	public class InsertMergeFieldsBarSubItem : BarSubItem {
		public static readonly DependencyProperty RichEditControlProperty = DependencyPropertyManager.Register("RichEditControl", typeof(RichEditControl), typeof(InsertMergeFieldsBarSubItem));
		public RichEditControl RichEditControl {
			get { return (RichEditControl)GetValue(RichEditControlProperty); }
			set { SetValue(RichEditControlProperty, value); }
		}
		public InsertMergeFieldsBarSubItem() {
			this.GetItemData += OnGetItemData;
		}
		void OnGetItemData(object sender, EventArgs e) {
			UpdateItems();
		}
		protected internal void UpdateItems() {
			if (RichEditControl != null)
				UpdateItemsCore();
		}
		protected internal virtual void UpdateItemsCore() {
			MergeFieldName[] fieldNames = RichEditControl.DocumentModel.GetDatabaseFieldNames();
			fieldNames = RichEditControl.InnerControl.RaiseCustomizeMergeFields(new CustomizeMergeFieldsEventArgs(fieldNames));
			foreach (MergeFieldName fieldName in fieldNames) {
				AppendItem(fieldName);
			}
		}
		protected internal virtual void AppendItem(MergeFieldName fieldName) {
			BarButtonItem item = new BarButtonItem();
			item.Content = fieldName.DisplayName;
			item.Command = CreateInsertMailMergeFieldCommand(fieldName.Name);
			item.CommandParameter = RichEditControl;
			ItemLinks.Add(item);
		}
		protected internal ICommand CreateInsertMailMergeFieldCommand(string name) {
			return new InsertMailMergeFieldRichEditUICommand(name);
		}
	}
	#endregion
}
namespace DevExpress.XtraRichEdit.Commands {
	#region InsertMailMergeFieldRichEditUICommand
	public class InsertMailMergeFieldRichEditUICommand : RichEditUICommand {
		readonly string fieldName;
		public InsertMailMergeFieldRichEditUICommand(string fieldName)
			: base(RichEditCommandId.InsertMailMergeField) {
			this.fieldName = fieldName;
		}
		protected internal override void ExecuteCommand(RichEditControl control, RichEditCommandId commandId, object parameter) {
			base.ExecuteCommand(control, commandId, fieldName);
		}
	}
	#endregion
}
