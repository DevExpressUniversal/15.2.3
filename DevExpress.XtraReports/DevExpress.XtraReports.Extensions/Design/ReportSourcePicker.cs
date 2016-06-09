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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.XtraReports.UI;
using System.Collections.Specialized;
using System.Reflection;
using System.Collections.Generic;
namespace DevExpress.XtraReports.Design
{
	[ToolboxItem(false)]
	public abstract class PickerBase : System.Windows.Forms.ListBox {
		#region static
		protected static bool CanCloseDropDown(object item) {
			return item != null;
		}
		#endregion
		System.ComponentModel.Container components = null;
		string itemName;
		IWindowsFormsEditorService edSvc;
		protected ITypeDescriptorContext context;
		string[] itemNames;
		public string ItemName {
			get { return itemName; }
		}
		protected PickerBase() {
			InitializeComponent();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent() {
			this.BorderStyle = System.Windows.Forms.BorderStyle.None;
		}
		public void End() {
			Items.Clear();
			edSvc = null;
			itemName = "";
		}
		public void Start(ITypeDescriptorContext context, IWindowsFormsEditorService edSvc, string itemName) {
			this.context = context;
			this.edSvc = edSvc;
			this.itemName = itemName;
			itemNames = GetItemNames();
			RefreshItems();
			SetSelectedItem(itemName);
			Width = GetMaxItemWidth();
		}
		protected new object GetService(Type t) {
			return context.GetService(t);
		}
		protected abstract string[] GetItemNames();
		protected virtual void AddNoneString() {
			Items.Add(DesignSR.DataGridNoneString.ToLower());
		}
		new void RefreshItems() {
			if(Items.Count > 0)
				Items.Clear();
			Items.AddRange(itemNames);
			AddNoneString();
		}
		private void SetSelectedItem(string text) {
			foreach(object item in Items) {
				if(GetItemText(item) == text) {
					SelectedItem = item;
					break;
				}
			}
		}
		private int GetMaxItemWidth() {
			int width = 0;
			foreach(object item in Items) {
				SizeF size = XtraPrinting.Native.Measurement.MeasureString(GetItemText(item), Font, GraphicsUnit.Pixel);
				width = Math.Max(width, (int)size.Width);
			}
			return width;
		}
		protected override bool IsInputKey(Keys key) {
			if(key == Keys.Enter) {
				if(CanCloseDropDown(SelectedItem))
					SetSelectedSource(SelectedItem);
				else
					return true;
			}
			return base.IsInputKey(key);
		}
		protected override void OnClick(EventArgs e) {
			base.OnClick(e);
			object item = GetItemAt(PointToClient(MousePosition));
			if(CanCloseDropDown(item)) {
				SetSelectedSource(item);
				edSvc.CloseDropDown();
			}
		}
		private object GetItemAt(Point pt) {
			for(int i = 0; i < Items.Count; i++) {
				Rectangle rect = GetItemRectangle(i);
				if(rect.Contains(pt)) return Items[i];
			}
			return null;
		}
		private void SetSelectedSource(object item) {
			itemName = GetItemText(item);
		}
	}
	[ToolboxItem(false)]
	public class MethodPicker : PickerBase {
		protected override string[] GetItemNames() {
			ReportTabControl tabControl = context.GetService(typeof(ReportTabControl)) as ReportTabControl;
			System.Diagnostics.Debug.Assert(tabControl != null);
			try {
				return tabControl.GetCompatibleMethodsNames(((XRScriptsBase)context.Instance).Component as IScriptable, context.PropertyDescriptor.DisplayName);
			}
			catch {
			}
			return new string[] { };
		}
		protected override void AddNoneString() { }
	}
	[ToolboxItem(false)]
	public abstract class TypePickerBase : PickerBase {
		public string TypeName { get { return ItemName; }
		}
		protected TypePickerBase():base() {
		}
		protected override string[] GetItemNames(){
			return GetTypeNames();
		}
		protected abstract string[] GetTypeNames();
	}
	[ToolboxItem(false)]
	public class ReportSourcePicker : TypePickerBase {
		#region static
		static string[] GetTypeNames(IEnumerable<Type> types) {
			List<string> typeNames = new List<string>();
			foreach(Type type in types) {
				if(!typeNames.Contains(type.FullName))
					typeNames.Add(type.FullName);
			}
			return typeNames.ToArray();
		}
		#endregion
		protected override string[] GetTypeNames() {
			IDesignerHost host = (IDesignerHost)context.GetService(typeof(IDesignerHost));
			System.Diagnostics.Debug.Assert(host != null);
			try {
				var types = ReportSourcePickerHelper.GetAvailableReports(host.RootComponent);
				return GetTypeNames(types);
			} catch {
			}
			return new string[] {};
		}
	}
}
