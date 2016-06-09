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
using System.Drawing;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using DevExpress.Utils;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.XtraLayout.Handlers;
using DevExpress.XtraLayout.HitInfo;
using DevExpress.XtraLayout.Resizing;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraLayout.Customization;
using System.Diagnostics;
using System.Windows.Forms;
#if DEBUGTEST
using Microsoft.VisualStudio.DebuggerVisualizers;
#endif
namespace DevExpress.XtraLayout.Diagnostics {
#if !DXOrcas
#if DEBUGTEST
#if DXWhidbey
	[Serializable]
	public class BaseLayoutItemProxy {
		public string text;
		public string name;
		public Rectangle bounds;
		public bool isVisible;
	}
	[Serializable]
	public class LayoutGroupProxy : BaseLayoutItemProxy {
		public BaseLayoutItemProxy[] items;
	}
	public class LayoutItemVisualizerObjectSourceCore : BaseLayoutItemVisualizerObjectSource {
		protected override BaseLayoutItemProxy CreateBItemProxy(DevExpress.XtraLayout.BaseLayoutItem item) {
			BaseLayoutItemProxy temp = base.CreateBItemProxy(item);
			temp.bounds = item.Bounds;
			return temp;
		}
		protected override LayoutGroupProxy CreateGroupProxy(DevExpress.XtraLayout.LayoutGroup group) {
			LayoutGroupProxy temp = base.CreateGroupProxy(group);
			temp.bounds = group.Bounds;
			return temp;
		}
	}
	public class BaseLayoutItemVisualizerObjectSource : VisualizerObjectSource {
		protected virtual BaseLayoutItemProxy CreateBItemProxy(DevExpress.XtraLayout.BaseLayoutItem item) {
			BaseLayoutItemProxy proxy = new BaseLayoutItemProxy();
			proxy.text = item.Text;
			proxy.name = item.Name;
			proxy.isVisible = item.Visibility == LayoutVisibility.Always;
			proxy.bounds = item.Bounds;
			return proxy;
		}
		protected virtual LayoutGroupProxy CreateCollectionProxy(BaseItemCollection collection) {
			LayoutGroupProxy gProxy = new LayoutGroupProxy();
			gProxy.text = "collection";
			gProxy.name = Guid.NewGuid().ToString();
			gProxy.bounds = collection.ItemsBounds;
			if(collection.Count > 0)
				gProxy.items = new BaseLayoutItemProxy[collection.Count];
			else
				gProxy.items = new BaseLayoutItemProxy[1];
			int i = 0;
			foreach(DevExpress.XtraLayout.BaseLayoutItem item in collection) {
				gProxy.items[i] = CreateBItemProxy(item);
				i++;
			}
			return gProxy;
		}
		protected virtual LayoutGroupProxy CreateGroupProxy(DevExpress.XtraLayout.LayoutGroup group) {
			LayoutGroupProxy gProxy = new LayoutGroupProxy();
			gProxy.text = group.Text;
			gProxy.name = group.Name;
			gProxy.bounds = group.ViewInfo.BoundsRelativeToControl;
			if(group.Items.Count > 0)
				gProxy.items = new BaseLayoutItemProxy[group.Items.Count];
			else
				gProxy.items = new BaseLayoutItemProxy[1];
			int i = 0;
			foreach(DevExpress.XtraLayout.BaseLayoutItem item in new ArrayList(group.Items)) {
				gProxy.items[i] = CreateBItemProxy(item);
				i++;
			}
			return gProxy;
		}
		public override void GetData(object target, System.IO.Stream outgoingData) {
			DevExpress.XtraLayout.LayoutGroup group = target as DevExpress.XtraLayout.LayoutGroup;
			DevExpress.XtraLayout.BaseLayoutItem bitem = target as DevExpress.XtraLayout.BaseLayoutItem;
			BaseItemCollection collection = target as BaseItemCollection;
			if(collection != null) {
				base.GetData(CreateCollectionProxy(collection), outgoingData);
				return;
			}
			if(group != null) {
				base.GetData(CreateGroupProxy(group), outgoingData);
				return;
			}
				if(bitem != null) {
					base.GetData(CreateBItemProxy(bitem), outgoingData);
				return;
			}
		}
	}
	public class SimpleVisualizerCore : SimpleVisualizer {
	}
	public class SimpleVisualizer : DialogDebuggerVisualizer {
		protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider) {
			object o = objectProvider.GetObject();
			BaseLayoutItemProxy proxy = o as BaseLayoutItemProxy;
			if(proxy != null) {
				VisualizerForm form = new VisualizerForm(proxy);
				windowService.ShowDialog(form);
			}
		}
	}
	public class VisualizerForm : Form {
		BaseLayoutItemProxy proxyCore = null;
		Pen itemPen = new Pen(Color.Red);
		SolidBrush brush = new SolidBrush(Color.FromArgb(100, 0, 0, 100));
		SolidBrush groupBrush = new SolidBrush(Color.FromArgb(100, 255, 255, 100));
		public VisualizerForm(BaseLayoutItemProxy proxy) {
			proxyCore = proxy;
		}
		protected void PaintItem(BaseLayoutItemProxy item, PaintEventArgs e, string id) {
			if(item == null) return;
			if(!(item is LayoutGroupProxy) && item.isVisible) e.Graphics.FillRectangle(brush, item.bounds);
			else
				e.Graphics.FillRectangle(groupBrush, item.bounds);
			e.Graphics.DrawRectangle(itemPen, item.bounds);
			AppearanceObject app = new AppearanceObject();
			if(!(item is LayoutGroupProxy)) e.Graphics.DrawString(id == null ? item.text : id, app.Font, Brushes.Black, item.bounds.Location);
		}
		protected void PaintGroupLayout(PaintEventArgs e) {
			LayoutGroupProxy groupProxy = proxyCore as LayoutGroupProxy;
			PaintItem(proxyCore, e, null);
			if(groupProxy != null) {
				int counter = 0;
				foreach(BaseLayoutItemProxy item in groupProxy.items) {
					PaintItem(item, e, counter.ToString());
					counter++;
				}
			}
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			PaintGroupLayout(e);
		}
	}
#endif
#endif
#endif
	public class ControlHierarchyHelper {
		public static string PrintHierarchy(Control control) {
			Control root = control;
			while(root.Parent != null) root = root.Parent;
			int totalControls = 0;
			string res = PrintHierarchyCore(root, 0, ref totalControls);
			res += String.Format("total controls:{0}", totalControls.ToString());
			return res;
		}
		protected static string PrintHierarchyCore(Control control, int level, ref int totalControls) {
			totalControls++;
			string result = String.Empty;
			string currentLine = string.Empty;
			LayoutControl layoutControl = control as LayoutControl;
			LayoutControl parentLC = control.Parent as LayoutControl;
			for(int i = 0; i < level; i++) currentLine += " ";
			if(parentLC != null) {
				currentLine += String.Format("{0} NAME {1} LAYOUTITEM {3} {4} {2}", control.ToString(), control.Name, Environment.NewLine, parentLC.GetItemByControl(control).Name, parentLC.GetItemByControl(control).Text);
			}
			else
				currentLine += String.Format("{0} name {1}{2}", control.ToString(), control.Name, Environment.NewLine);
			result += currentLine;
			foreach(Control temp in control.Controls) {
				if(layoutControl != null && ((ILayoutDesignerMethods)layoutControl).IsInternalControl(temp)) continue;
				result += PrintHierarchyCore(temp, level + 1, ref totalControls);
			}
			return result;
		}
	}
	public class ConsistentLayoutChecker {
		public static string CheckLayout(LayoutGroup group) {
			string ErrStr = String.Empty;
			for(int j = 0; j < group.Items.Count - 1; j++) {
				BaseLayoutItem item2 = group[j];
				for(int i = j + 1; i < group.Items.Count; i++) {
					BaseLayoutItem item1 = group[i];
					if(item2.Bounds == Rectangle.Empty || item2.Size == Size.Empty) ErrStr += "Error: item " + item1.Text + " has empty bounds";
					if(item1.Bounds.IntersectsWith(item2.Bounds)) {ErrStr+= 
							"Error: item " + item1.Text + " " + item1.Bounds.ToString() + " intersects with item " +
							item2.Text + " " + item2.Bounds.ToString() + "\r\n";
					}
				}
			}
			return ErrStr;
		}
	}
}
