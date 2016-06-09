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

using System.ComponentModel;
using DevExpress.XtraBars.Docking2010;
using DevExpress.XtraBars.Docking2010.Views;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
namespace DevExpress.XtraBars.Design {
	[ToolboxItem(false)]
	public class PopupViewCollectionEdit : ImageComboBoxEdit {
		DocumentManager manager = null;
		public PopupViewCollectionEdit() {
			this.Properties.PopupSizeable = false;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BaseView View {
			get { return EditValue as BaseView; }
			set { EditValue = value; }
		}
		[DefaultValue(null)]
		public DocumentManager Manager {
			get { return manager; }
			set {
				if(Manager == value) return;
				if(manager != null) manager.ViewCollection.CollectionChanged -= new Docking2010.Base.CollectionChangedHandler<BaseView>(ViewCollection_CollectionChanged);
				manager = value;
				manager.ViewCollection.CollectionChanged += new Docking2010.Base.CollectionChangedHandler<BaseView>(ViewCollection_CollectionChanged);
				PopulateViews();
			}
		}
		void ViewCollection_CollectionChanged(DevExpress.XtraBars.Docking2010.Base.CollectionChangedEventArgs<BaseView> ea) {
			PopulateViews();
		}
		protected void PopulateViews() {
			Properties.Items.Clear();
			ImageComboBoxItem[] items = new ImageComboBoxItem[Manager.ViewCollection.Count];
			for(int i = 0; i < items.Length; i++) {
				BaseView view = Manager.ViewCollection[i];
				items[i] = new ImageComboBoxItem(view, (int)view.Type);
			}
			Properties.Items.AddRange(items);
		}
		protected internal void SetViewCore(BaseView view) {
			LockEditValueChanged();
			try {
				View = view;
			}
			finally {
				UnLockEditValueChanged();
			}
		}
	}
}
