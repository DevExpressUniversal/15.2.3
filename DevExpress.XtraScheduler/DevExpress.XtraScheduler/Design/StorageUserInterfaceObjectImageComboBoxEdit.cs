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
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraScheduler.Native;
using DevExpress.Utils;
using DevExpress.Internal;
namespace DevExpress.XtraScheduler.UI {
	#region StorageUserInterfaceObjectRepositoryItemImageComboBox
	[System.Runtime.InteropServices.ComVisible(false)]
	public abstract class StorageUserInterfaceObjectRepositoryItemImageComboBox<T> : StorageBindedRepositoryItemImageComboBox where T : IUserInterfaceObject {
		RepositoryImageComboBoxFillHelper<T> fillHelper;
		protected StorageUserInterfaceObjectRepositoryItemImageComboBox() {
			this.fillHelper = new RepositoryImageComboBoxFillHelper<T>();
		}
		protected internal abstract UserInterfaceObjectWinCollection<T> Collection { get; }
		internal RepositoryImageComboBoxFillHelper<T> FillHelper {
			get { return fillHelper; }
		}
		protected internal override void SubscribeStorageEvents() {
			base.SubscribeStorageEvents();
			UserInterfaceObjectCollection<T> collection = this.Collection;
			if (collection != null)
				collection.CollectionChanged += new CollectionChangedEventHandler<T>(OnCollectionChanged);
		}
		protected internal override void UnsubscribeStorageEvents() {
			base.UnsubscribeStorageEvents();
			UserInterfaceObjectCollection<T> collection = this.Collection;
			if (collection != null)
				collection.CollectionChanged -= new CollectionChangedEventHandler<T>(OnCollectionChanged);
		}
		protected internal virtual void OnCollectionChanged(object sender, CollectionChangedEventArgs<T> e) {
			RefreshData();
		}
		public override void RefreshData() {
			BeginUpdate();
			try {
				FillHelper.FillComboBoxWithItemValues(this, Collection);
			} finally {
				EndUpdate();
			}
		}
	}
	#endregion
}
namespace DevExpress.XtraScheduler.Native {
	public class RepositoryImageComboBoxFillHelper<T> where T : IUserInterfaceObject {
		protected internal delegate object SelectValue(T item, int index);
		protected Bitmap CreateBitmap(Brush brush, int width, int height) {
			Rectangle r = new Rectangle(0, 0, width, height);
			Bitmap bmp = new Bitmap(r.Width, r.Height);
			using ( Graphics gr = Graphics.FromImage(bmp) ) {
				if ( brush != null )
					gr.FillRectangle(brush, r);
				gr.FillRectangle(Brushes.Black, RectUtils.GetTopSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetLeftSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetRightSideRect(r, 1));
				gr.FillRectangle(Brushes.Black, RectUtils.GetBottomSideRect(r, 1));
			}
			return bmp;
		}
		protected internal void FillComboBoxCore(RepositoryItemImageComboBox repositoryItemImageComboBox, UserInterfaceObjectWinCollection<T> collection, SelectValue selectItemValue) {
			repositoryItemImageComboBox.Items.Clear();
			ImageList il = RepositoryImageComboBoxFillHelperBase.CreateSmallImageList();
			repositoryItemImageComboBox.SmallImages = il;
			if ( collection == null )
				return;
			int count = collection.Count;
			Rectangle r = RepositoryImageComboBoxFillHelperBase.CreateItemImageRectangle();
			for ( int i = 0; i < count; i++ ) {
				T item = collection[i];
				Bitmap bmp = CreateBitmap(collection.GetObjectBrush(item), r.Width, r.Height);
				il.Images.Add(bmp);
				repositoryItemImageComboBox.Items.Add(new ImageComboBoxItem(item.DisplayName, selectItemValue(item, i), i));
			}
		}
		protected internal virtual object SelectValueAsIndex(T item, int index) {
			return index;
		}
		protected internal virtual object SelectValueAsItem(T item, int index) {
			return item;
		}
		public void FillComboBoxWithIndexValues(RepositoryItemImageComboBox repositoryItemImageComboBox, UserInterfaceObjectWinCollection<T> collection) {
			FillComboBoxCore(repositoryItemImageComboBox, collection, SelectValueAsIndex);
		}
		public void FillComboBoxWithItemValues(RepositoryItemImageComboBox repositoryItemImageComboBox, UserInterfaceObjectWinCollection<T> collection) {
			FillComboBoxCore(repositoryItemImageComboBox, collection, SelectValueAsItem);
		}
	}
}
