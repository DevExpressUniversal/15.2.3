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

using System.Linq;
using System.Collections.Generic;
using System.Collections;
using System.Windows.Data;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Mvvm.Native;
using System.ComponentModel;
namespace DevExpress.Xpf.Editors.Internal {
	public abstract class SelectorEditInnerListBoxItemsSourceHelper {
		public static SelectorEditInnerListBoxItemsSourceHelper CreateHelper(ISelectorEditInnerListBox listBox, bool useCustomItems) {
			return useCustomItems ? (SelectorEditInnerListBoxItemsSourceHelper)new PlainListSourceHelper(listBox) : new HierarchicalListSourceHelper(listBox);
		}
		IEnumerable customItemsSource;
		IEnumerable contentItemsSource;
		protected ISelectorEditInnerListBox ListBox { get; private set; }
		public IEnumerable CustomItemsSource { get { return customItemsSource ?? new List<object>(); } }
		public IEnumerable ContentItemsSource { get { return contentItemsSource ?? new List<object>(); } }
		public SelectorEditInnerListBoxItemsSourceHelper(ISelectorEditInnerListBox listBox) {
			ListBox = listBox;
		}
		public virtual void SetCustomItemsSource(IEnumerable customItemsSource) {
			this.customItemsSource = customItemsSource;
		}
		public virtual void SetContentItemsSource(IEnumerable contentItemsSource) {
			this.contentItemsSource = contentItemsSource;
		}
		public virtual void AssignItemsSource() {
			VerifySource();
			SetCustomItemsSource(ListBox.OwnerEdit.GetPopupContentCustomItemsSource());
			SetContentItemsSource(ListBox.OwnerEdit.GetPopupContentItemsSource() as IEnumerable);
		}
		protected virtual void VerifySource() {
		}
		public virtual bool? IsSelectAll() {
			if (ListBox.OwnerEdit.ItemsProvider.IsServerMode)
				return false;
			var selectedItems = ListBox.SelectedItems.Cast<object>();
			var filteredSelectedItems = selectedItems.Except(CustomItemsSource.Cast<object>());
			var difference = filteredSelectedItems.Intersect(ContentItemsSource.Cast<object>()).ToList();
			if (difference.Count == 0)
				return false;
			if (difference.Count == ContentItemsSource.Cast<object>().Count())
				return true;
			return null;
		}
	}
#if !SL
	public class PlainListSourceHelper : SelectorEditInnerListBoxItemsSourceHelper {
		EditorsCompositeCollection ItemsSource { get; set; }
		public PlainListSourceHelper(ISelectorEditInnerListBox listBox)
			: base(listBox) {
		}
		protected override void VerifySource() {
			EditorsCompositeCollection collection = ListBox.ItemsSource as EditorsCompositeCollection;
			if (collection == null)
				InitializeItemsSource();
			else
				RestoreItemsSource();
		}
		protected virtual void RestoreItemsSource() {
			ItemsSource = (EditorsCompositeCollection)ListBox.ItemsSource;
		}
		protected virtual void InitializeItemsSource() {
			ItemsSource = new EditorsCompositeCollection();
		}
		public override void AssignItemsSource() {
			base.AssignItemsSource();
			ListBox.ItemsSource = ItemsSource;
		}
		public override void SetContentItemsSource(IEnumerable contentItemsSource) {
			base.SetContentItemsSource(contentItemsSource);
			ItemsSource.SetContentCollection((IList)contentItemsSource);
		}
		public override void SetCustomItemsSource(IEnumerable customItemsSource) {
			base.SetCustomItemsSource(customItemsSource);
			ItemsSource.SetCustomCollection((IList)customItemsSource);
		}
	}
#else
	public class PlainListSourceHelper : SelectorEditInnerListBoxItemsSourceHelper {
		IEnumerable PlainListSource { get { return GetPlainListSource(); } }
		public PlainListSourceHelper(ISelectorEditInnerListBox listBox)
			: base(listBox) {
		}
		IEnumerable<object> GetPlainListSource() {
			return CustomItemsSource.Cast<object>().Append(ContentItemsSource.Cast<object>());
		}
		public override void AssignItemsSource() {
			base.AssignItemsSource();
			ListBox.ItemsSource = PlainListSource;
		}
	}
#endif
	public class HierarchicalListSourceHelper : SelectorEditInnerListBoxItemsSourceHelper {
		IEnumerable HierarchicalListSource { get { return GetHierarchicalListSource(); } }
		public HierarchicalListSourceHelper(ISelectorEditInnerListBox listBox)
			: base(listBox) {
		}
		IEnumerable GetHierarchicalListSource() {
			return ContentItemsSource;
		}
		public override void AssignItemsSource() {
			base.AssignItemsSource();
			ListBox.ItemsSource = HierarchicalListSource;
		}
	}
	internal class DummyListSourceHelper : SelectorEditInnerListBoxItemsSourceHelper {
		public DummyListSourceHelper()
			: base(null) {
		}
		public override void AssignItemsSource() {
		}
	}
}
