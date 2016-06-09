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
using System.ComponentModel;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class AddRemoveChartNamedElementControl : ChartUserControl {
		ChartCollectionBase collection;
		ChartElement currentElement;
		bool locked;
		public ChartCollectionBase Collection { get { return collection; } }
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual ChartElement CurrentElement { 
			get { return currentElement; } 
			set {
				if (collection.Contains(value))
					cbElement.SelectedItem = value;
			} 
		}
		public event SelectedElementChangedEventHandler SelectedElementChanged;
		public AddRemoveChartNamedElementControl() {
			InitializeComponent();
			cbRemove.Enabled = false;
		}
		public void Initialize(ChartCollectionBase collection) {
			this.collection = collection;
			locked = true;
			try {
				UpdateControls();
				if (collection.Count == 0) {
					currentElement = null;
					cbElement.SelectedIndex = -1;
				}
				else {
					currentElement = (ChartElement)collection.GetElementByIndex(0);
					cbElement.SelectedIndex = 0;
				}
				OnSelectedElementChanged();
			}
			finally {
				locked = false;
			}
		}
		public void UpdateList() {
			locked = true;
			try {
				FillComboBox();
				cbElement.SelectedItem = cbElement.SelectedItem;
				cbRemove.Enabled = collection != null && collection.Count > 0;
			}
			finally {
				locked = false;
			}
		}
		protected virtual void FillComboBox() {
			cbElement.Properties.Items.Clear();
			cbElement.Properties.Items.AddRange(collection);
		}
		protected virtual int Add() {
			ChartDebug.Assert(true, "override this method!");
			return -1;
		}
		void OnSelectedElementChanged() {
			if (SelectedElementChanged != null)
				SelectedElementChanged();
		}
		void UpdateControls() {
			FillComboBox();
			cbRemove.Enabled = collection.Count > 0;
		}
		void cbAdd_Click(object sender, EventArgs e) {
			int index = Add();
			if (index >= 0) {
				UpdateControls();
				cbElement.SelectedIndex = index;
			}
		}
		void cbRemove_Click(object sender, EventArgs e) {
			currentElement = null; 
			int index = cbElement.SelectedIndex;
			collection.RemoveAt(index);
			int count = collection.Count;
			if (index >= count)
				index = count - 1;
			UpdateControls();
			if (index > -1)
				cbElement.SelectedIndex = index;
			else
				cbElement.Text = String.Empty;
			OnSelectedElementChanged();
		}
		void cbAxes_SelectedIndexChanged(object sender, EventArgs e) {
			if (!locked) {
				currentElement = (ChartElement)cbElement.SelectedItem;
				OnSelectedElementChanged();
			}
		}
	}
	internal delegate void SelectedElementChangedEventHandler();
}
