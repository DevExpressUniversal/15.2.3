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
using System.Windows.Forms.Design;
using System.Collections;
using System.Windows.Forms;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using DevExpress.XtraEditors;
using DevExpress.Utils.NonclientArea;
using DevExpress.Utils.Drawing.Helpers;
using System.ComponentModel.Design.Serialization;
namespace DevExpress.Utils.Design {
	public class XtraScrollableControlDesigner : BaseParentControlDesigner, DevExpress.XtraEditors.XtraScrollableControl.IXtraScrollableControlDesigner {
		DesignerVerbCollection verbs;
		ISelectionService selectionService;
		IComponentChangeService componentChangeService;
		ArrayList selecetedControls;
		bool isScrolling = false;
		public XtraScrollableControlDesigner() : base() { }
		public ISelectionService SelectionService {
			get {
				if(selectionService == null) {
					selectionService = (ISelectionService)GetService(typeof(ISelectionService));
				}
				return selectionService;
			}
		}
		public IComponentChangeService ComponentChangeService {
			get {
				if(componentChangeService == null) {
					componentChangeService = (IComponentChangeService)GetService(typeof(IComponentChangeService));
				}
				return componentChangeService;
			}
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			Component.Scroll += new XtraScrollEventHandler(OnScroll);
			((HScrollBarViewInfoWithHandler)GetProperty("HScrollBar")).MouseUp += new MouseEventHandler(OnScrollMouseUp);
			((VScrollBarViewInfoWithHandler)GetProperty("VScrollBar")).MouseUp += new MouseEventHandler(OnScrollMouseUp);
			Control.DockChanged += new EventHandler(OnDockChanged);
		}
		object GetProperty(string name) {
			PropertyInfo pi = typeof(XtraScrollableControl).GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if(pi != null) return pi.GetValue(Component, null);
			throw new Exception(string.Format("Property not found {0}", name));
		}
		public override DesignerVerbCollection DXVerbs {
			get {
				if(verbs == null) {
					verbs = new DesignerVerbCollection();
					DesignerVerb enableAutoScrollScrollVerb = new DesignerVerb("Enable AutoScroll", new EventHandler(OnAutoScroll));
					DesignerVerb disableAutoScrollVerb = new DesignerVerb("Disable AutoScroll", new EventHandler(OnAutoScroll));
					DesignerVerb dockVerb = new DesignerVerb("Dock in parent container", new EventHandler(OnDock));
					DesignerVerb unDockVerb = new DesignerVerb("Undock in parent container", new EventHandler(OnDock));
					verbs.Add(dockVerb);
					verbs.Add(unDockVerb);
					verbs.Add(enableAutoScrollScrollVerb);
					verbs.Add(disableAutoScrollVerb);
					DXSmartTagsHelper.CreateDefaultVerbs(this, this.verbs);
				}
				UpdateVerbsStatus();
				return verbs;
			}
		}
		public new XtraEditors.XtraScrollableControl Component {
			get { return (XtraEditors.XtraScrollableControl)base.Component; }
		}
		protected bool IsScrolling { get { return isScrolling; } }
		protected override bool UseVerbsAsActionList { get { return true; } }
		protected void UpdateVerbsStatus() {
			verbs[0].Enabled = Component.Dock != DockStyle.Fill;
			verbs[1].Enabled = !verbs[0].Enabled;
			verbs[2].Enabled = Component.AutoScroll != true;
			verbs[3].Enabled = !verbs[2].Enabled;
		}
		bool isSelectionReset = false;
		protected void UpdateControlsSelection() {
			if(IsScrolling) {
				if(!isSelectionReset) {
					selecetedControls = new ArrayList(SelectionService.GetSelectedComponents());
					SelectionService.SetSelectedComponents(new object[] { });
					isSelectionReset = true;
				}
			}
			else {
				if(SelectionService.GetSelectedComponents().Count == 0) {
					SelectionService.SetSelectedComponents(selecetedControls);
				}
				isSelectionReset = false;
			}
		}
		protected void RefreshSelection() {
			selecetedControls = new ArrayList(SelectionService.GetSelectedComponents());
			SelectionService.SetSelectedComponents(new object[] { });
			SelectionService.SetSelectedComponents(selecetedControls);
		}
		protected int MakeLParam(int low, int high) {
			return ((high << 0x10) | (low & 0xffff));
		}
		protected override bool GetHitTest(Point pt) {
			if(base.GetHitTest(pt)) { return true; }
			XtraEditors.XtraScrollableControl control = (XtraEditors.XtraScrollableControl)Control;
			if(control.IsHandleCreated && control.AutoScroll) {
				int num = (int)NativeMethods.SendMessage((IntPtr)control.Handle, 0x84, 0, (IntPtr)MakeLParam(pt.X, pt.Y));
				if((num == 7) || (num == 6)) {
					return true;
				}
			}
			return false;
		}
		void OnDock(object sender, EventArgs e) {
			try {
				if(Component.Dock != DockStyle.Fill)
					Component.Dock = DockStyle.Fill;
				else
					Component.Dock = DockStyle.None;
			}
			finally {
				UpdateVerbsStatus();
			}
		}
		void OnAutoScroll(object sender, EventArgs e) {
			try {
				Component.AutoScroll = (Component.AutoScroll == true) ? false : true;
				ComponentChangeService.OnComponentChanged(Component, null, null, null);
			}
			finally {
				UpdateVerbsStatus();
			}
		}
		void OnScroll(object sender, XtraScrollEventArgs e) {
			if(!isScrolling) {
				isScrolling = true;
				UpdateControlsSelection();
				Control.Invalidate();
			}
		}
		void OnScrollMouseUp(object sender, MouseEventArgs e) {
			isScrolling = false;
			UpdateControlsSelection();
		}
		protected void OnDockChanged(object sender, EventArgs e) {
			RefreshSelection();
			Control.Invalidate();
		}
		protected override void Dispose(bool disposing) {
			if(Component == null) return; 
			Component.Scroll -= new XtraScrollEventHandler(OnScroll);
			((HScrollBarViewInfoWithHandler)GetProperty("HScrollBar")).MouseUp -= new MouseEventHandler(OnScrollMouseUp);
			((VScrollBarViewInfoWithHandler)GetProperty("VScrollBar")).MouseUp -= new MouseEventHandler(OnScrollMouseUp);
			Control.DockChanged -= new EventHandler(OnDockChanged);
			base.Dispose(disposing);
		}
		protected bool DriveScrolls(ref Message m) {
			if(Component.IsHandleCreated && m.HWnd == Component.Handle) {
				Type childWindowType = Component.WindowTarget.GetType();
				System.Reflection.FieldInfo info = childWindowType.GetField("oldTarget", BindingFlags.Instance | BindingFlags.NonPublic);
				if(info == null) return false;
				IWindowTarget window = (IWindowTarget)info.GetValue(Component.WindowTarget);
				window.OnMessage(ref m);
				return true;
			}
			return false;
		}
		void DevExpress.XtraEditors.XtraScrollableControl.IXtraScrollableControlDesigner.Update() {
			RefreshSelection();
		}
		protected override void WndProc(ref Message m) {
			if(m.Msg == NonclientArea.NCMessages.WM_NCMOUSEMOVE || m.Msg == NonclientArea.NCMessages.WM_NCMOUSELEAVE) {
				if(DriveScrolls(ref m)) return;
			}
			if(m.Msg == NonclientArea.CMessages.WM_LBUTTONUP && Component.Capture) {
				Point p = new Point(m.LParam.ToInt32());
				int btns = m.WParam.ToInt32();
				MouseButtons buttons = MouseButtons.None;
				if((btns & 1) != 0) buttons |= MouseButtons.Left;
				MethodInfo mi = GetMethodInfo("OnMouseUp");
				if(mi != null) mi.Invoke(Control, new object[] { new MouseEventArgs(buttons, 0, p.X, p.Y, 0) });
			}
			base.WndProc(ref m);
		}
		protected override bool CanUseComponentSmartTags { get { return true; } }
	}
	internal class XtraScrollableControlSerializer : CodeDomSerializer {
		public override object Deserialize(IDesignerSerializationManager manager, object codeObject) {
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(XtraScrollableControl).BaseType, typeof(CodeDomSerializer));
			return baseClassSerializer.Deserialize(manager, codeObject);
		}
		public override object Serialize(IDesignerSerializationManager manager, object value) {
			CodeDomSerializer baseClassSerializer = (CodeDomSerializer)manager.GetSerializer(typeof(XtraScrollableControl).BaseType, typeof(CodeDomSerializer));
			XtraScrollableControl scrollableControl = value as XtraScrollableControl;
			if(scrollableControl != null) {
				MethodInfo mi = typeof(XtraScrollableControl).GetMethod("BeforeSerialize", BindingFlags.NonPublic | BindingFlags.InvokeMethod | BindingFlags.Instance);
				if(mi != null) mi.Invoke(scrollableControl, null);
			}
			return baseClassSerializer.Serialize(manager, value);
		}
	}
}
