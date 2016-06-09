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
using DevExpress.XtraEditors;
using System.ComponentModel;
using System.Reflection;
using DevExpress.Data;
using System.Collections.Generic;
using System.Collections.Specialized;
namespace DevExpress.XtraSpreadsheet.Forms.Design {
	[DXToolboxItem(false)]
	public class VerticalScrollableItemsControl : XtraUserControl, ISupportInitialize, INotifyPropertyChanged {
		object dataSource;
		readonly XtraScrollableControl scrollableControl;
		readonly CurrencyDataController dataAdapter;
		readonly List<Control> itemControls;
		int initCount;
		bool deferredCreateItemControls;
		const int verticalSpacingBetweenItemControls = 4;
		public VerticalScrollableItemsControl() {
			this.dataAdapter = new CurrencyDataController();
			this.scrollableControl = new XtraScrollableControl();
			this.scrollableControl.Dock = DockStyle.Fill;
			this.itemControls = new List<Control>();
			Controls.Add(scrollableControl);
		}
		#region Properties
		#region DataSource
		[Bindable(true)]
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource == value) {
					UpdateItemControls();
					return;
				}
				if (value != null && DataSource != null && DataSource.Equals(value)) {
					UpdateItemControls();
					return;
				}
				dataSource = value;
				ActivateDataSource();
				CreateItemControls();
				INotifyCollectionChanged source = dataSource as INotifyCollectionChanged;
				if(source != null) {
					source.CollectionChanged += OnCollectionChanged;
				}
			}
		}
		#endregion
		bool IsLoading { get { return initCount > 0; } }
		#region CurrentItemIndex
		public int CurrentItemIndex {
			get { return dataAdapter.CurrentListSourceIndex; }
			set {
				if (CurrentItemIndex == value)
					return;
				if(CurrentItemIndex >= 0 && CurrentItemIndex < itemControls.Count && !itemControls[CurrentItemIndex].Focused)
					itemControls[CurrentItemIndex].Select();
				dataAdapter.CurrentListSourceIndex = value;
				OnPropertyChanged("CurrentItemIndex");
			}
		}
		#endregion
		#endregion
		#region Events
		#region CreateItemControl
		CreateItemControlEventHandler onCreateItemControl;
		public event CreateItemControlEventHandler CreateItemControl { add { onCreateItemControl += value; } remove { onCreateItemControl -= value; } }
		protected internal Control RaiseCreateItemControl(object item) {
			if (onCreateItemControl == null)
				return null;
			CreateItemControlEventArgs args = new CreateItemControlEventArgs();
			args.Item = item;
			onCreateItemControl(this, args);
			return args.Control;
		}
		#endregion
		#region PropertyChanged
		PropertyChangedEventHandler onPropertyChanged;
		public event PropertyChangedEventHandler PropertyChanged { add { onPropertyChanged += value; } remove { onPropertyChanged -= value; } }
		protected void OnPropertyChanged(string propertyName) {
			if (onPropertyChanged != null)
				onPropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		#endregion
		#endregion
		void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			int y = 0;
			switch (e.Action) {
				case NotifyCollectionChangedAction.Add:
					if(e.NewStartingIndex == itemControls.Count) {
						for(int i = itemControls.Count - 1; i >= 0; i--) {
							if(itemControls[i] != null) {
								y = itemControls[i].Bounds.Bottom + verticalSpacingBetweenItemControls;
								break;
							}
						}
						for(int i = 0; i < e.NewItems.Count; i++) {
							Control itemControl = AppendItemControl(e.NewStartingIndex + i, y);
							itemControls.Add(itemControl);
							if(itemControl != null) {
								SubscribeItemControlEvents(itemControl);
								y += itemControl.Bounds.Height + verticalSpacingBetweenItemControls;
							}
						}
					}
					else {
						CreateItemControls();
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					for(int i = 0; i < e.OldItems.Count; i++)
						RemoveItemControl(i + e.OldStartingIndex);
					foreach(Control control in itemControls) {
						if(control == null)
							continue;
						control.Top = y;
						y += control.Bounds.Bottom + verticalSpacingBetweenItemControls;
					}
					break;
				default:
					CreateItemControls();
					break;
			}
		}
		protected virtual void ActivateDataSource() {
			dataAdapter.SetDataSource(BindingContext, DataSource, String.Empty);
			OnPropertyChanged("CurrentItemIndex");
		}
		void UpdateItemControls() {
			if (IsLoading) {
				deferredCreateItemControls = true;
				return;
			}
			int y = 0;
			int count = itemControls.Count;
			for (int i = 0; i < count; i++) {
				if(i >= dataAdapter.ListSourceRowCount) {
					UnsubscribeItemControlEvents(itemControls[i]);
					itemControls[i] = null;
				}
				else {
					Control itemControl = itemControls[i];
					if(itemControl == null) {
						itemControl = AppendItemControl(i, y);
						if(itemControl != null)
							SubscribeItemControlEvents(itemControl);
						itemControls[i] = itemControl;
					}
					if(itemControl != null)
						y = itemControl.Bottom + verticalSpacingBetweenItemControls;
				}
			}
		}
		void CreateItemControls() {
			if (IsLoading) {
				deferredCreateItemControls = true;
				return;
			}
			UnsubscribeItemControlsEvents();
			scrollableControl.Controls.Clear();
			itemControls.Clear();
			int y = 0;
			int count = dataAdapter.ListSourceRowCount;
			for (int i = 0; i < count; i++) {
				Control itemControl = AppendItemControl(i, y);
				itemControls.Add(itemControl);
				if (itemControl != null)
					y += itemControl.Bounds.Height + verticalSpacingBetweenItemControls;
			}
			SubscribeItemControlsEvents();
		}
		void RemoveItemControl(int index) {
			if(index >= itemControls.Count)
				return;
			UnsubscribeItemControlEvents(itemControls[index]);
			scrollableControl.Controls.RemoveAt(index);
			itemControls.RemoveAt(index);
		}
		Control AppendItemControl(int recordIndex, int y) {
			Control itemControl = CreateItemControlInstance(dataAdapter.GetListSourceRow(recordIndex));
			if (itemControl != null) {
				Size bestSize = CalculateItemControlSize(itemControl);
				AppendItemControl(itemControl, bestSize, y);
				return itemControl;
			}
			else
				return null;
		}
		void SubscribeItemControlsEvents() {
			foreach (Control itemControl in scrollableControl.Controls)
				SubscribeItemControlEvents(itemControl);
		}
		void UnsubscribeItemControlsEvents() {
			foreach (Control itemControl in scrollableControl.Controls)
				UnsubscribeItemControlEvents(itemControl);
		}
		void SubscribeItemControlEvents(Control itemControl) {
			itemControl.GotFocus += OnItemControlGotFocus;
			itemControl.LostFocus += OnItemControlLostFocus;
		}
		void UnsubscribeItemControlEvents(Control itemControl) {
			itemControl.GotFocus -= OnItemControlGotFocus;
			itemControl.LostFocus -= OnItemControlLostFocus;
		}
		void OnItemControlGotFocus(object sender, EventArgs e) {
			int index = CalculateItemControlIndex(sender as Control);
			if (index >= 0)
				CurrentItemIndex = index;
		}
		void OnItemControlLostFocus(object sender, EventArgs e) {
		}
		int CalculateItemControlIndex(Control itemControl) {
			if (itemControl == null)
				return -1;
			return itemControls.IndexOf(itemControl);
		}
		void AppendItemControl(Control control, Size bestSize, int y) {
			int width = Math.Max(10, this.Width - SystemInformation.VerticalScrollBarWidth);
			control.Bounds = new Rectangle(0, y, width, bestSize.Height);
			scrollableControl.Controls.Add(control);
		}
		Control CreateItemControlInstance(object item) {
			return RaiseCreateItemControl(item);
		}
		Size CalculateItemControlSize(Control itemControl) {
			BaseEdit baseEdit = itemControl as BaseEdit;
			if (baseEdit != null)
				return baseEdit.CalcBestSize();
			MethodInfo methodInfo = itemControl.GetType().GetMethod("CalcBestSize");
			if (methodInfo == null)
				return Size.Empty;
			object result = methodInfo.Invoke(itemControl, new object[] {});
			if (result is Size)
				return (Size)result;
			else
				return Size.Empty;
		}
		#region ISupportInitialize
		void ISupportInitialize.BeginInit() {
			this.initCount++;
		}
		void ISupportInitialize.EndInit() {
			this.initCount--;
			if (initCount == 0 && deferredCreateItemControls)
				CreateItemControls();
		}
		#endregion
	}
	public delegate void CreateItemControlEventHandler(object sender, CreateItemControlEventArgs e);
	public class CreateItemControlEventArgs : EventArgs {
		public Control Control { get; set; }
		public object Item { get; set; }
	}
}
