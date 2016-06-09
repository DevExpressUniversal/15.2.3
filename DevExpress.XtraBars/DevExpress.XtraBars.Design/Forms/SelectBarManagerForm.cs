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
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors;
using DevExpress.Skins;
namespace DevExpress.XtraBars.Design.Forms {
	public partial class SelectBarManagerForm : XtraForm {
		NotificationCollection<Component> barContainerCollection;
		public SelectBarManagerForm() {
			SkinManager.EnableFormSkins();
			this.barContainerCollection = new NotificationCollection<Component>();
			InitializeComponent();
			SubscribeEvents();
		}
		public NotificationCollection<Component> BarContainerCollection { get { return barContainerCollection; } }
		public Component SelectedContainer {
			get {
				return this.cbBarContainer.EditValue as Component;
			}
		}
		void SubscribeEvents() {
			BarContainerCollection.CollectionChanged += new CollectionChangedEventHandler<Component>(OnBarManagerCollectionChanged);
		}
		void UnsubsribeEvents() {
			BarContainerCollection.CollectionChanged -= new CollectionChangedEventHandler<Component>(OnBarManagerCollectionChanged);
		}
		void OnBarManagerCollectionChanged(object sender, CollectionChangedEventArgs<Component> e) {
			UpdateCbBarManager(); 
		}
		void UpdateCbBarManager() {
			UnsubsribeEvents();
			try {
				this.cbBarContainer.Properties.Items.Clear();
				int count = BarContainerCollection.Count;
				for (int i = 0; i < count; i++) {
					Component container = BarContainerCollection[i];
					string componentName = TypeDescriptor.GetComponentName(container, true);
					ImageComboBoxItem item = new ImageComboBoxItem(componentName, container);
					this.cbBarContainer.Properties.Items.Add(item);
				}
				if (count > 0)
					this.cbBarContainer.SelectedIndex = 0;
			}
			finally {
				SubscribeEvents();
			}
		}
	}
}
