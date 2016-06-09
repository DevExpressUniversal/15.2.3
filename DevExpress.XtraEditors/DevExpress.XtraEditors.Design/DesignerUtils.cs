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
using System.Windows.Forms;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using DevExpress.XtraTab;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Windows.Forms.Design;
namespace DevExpress.XtraEditors.Design {
	public class CustomButtonsCollectionEditor : DevExpress.Utils.Design.DXCollectionEditorBase {
		public CustomButtonsCollectionEditor(Type type) : base(type) { }
		protected override bool AllowLiveUpdates {
			get { return true; }
		}
	}
	public class ContextItemAnchorElementEditor : UITypeEditor {
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context) {
			return UITypeEditorEditStyle.DropDown;
		}
		IWindowsFormsEditorService editorService;
		public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value) {
			editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
			ListBox lb = new ListBox();
			lb.SelectedIndexChanged += lb_SelectedIndexChanged;
			lb.SelectionMode = SelectionMode.One;
			ContextItem item = (ContextItem)context.Instance;
			if(item.Collection == null) return value;
			lb.Items.Clear();
			lb.Items.Add(new ElementWrapper(null));
			foreach(ContextItem child in item.Collection) {
				if(child != item) {
					int index = lb.Items.Add(new ElementWrapper(child));
					if(child.Equals(value)) lb.SelectedIndex = index;
				}
			}
			editorService.DropDownControl(lb);
			var ew = lb.SelectedItem as ElementWrapper;
			if(ew == null) return null;
			return ew.Item;
		}
		void lb_SelectedIndexChanged(object sender, EventArgs e) {
			if(editorService == null) return;
			editorService.CloseDropDown();
		}
		class ElementWrapper {
			public ElementWrapper(ContextItem item) {
				this.Item = item;
			}
			public ContextItem Item { get; private set; }
			public override string ToString() {
				if(Item == null) return "<None>";
				if(!String.IsNullOrEmpty(Item.Name))
					return Item.Name;
				return GetType().Name;
			}
		}
	}
}
