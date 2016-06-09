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
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Diagnostics;
using DevExpress.XtraPrinting.Localization;
using System.Xml;
using System.Drawing.Printing;
using System.Drawing.Design;
using System.Text;
using System.ComponentModel;
using DevExpress.LookAndFeel;
using DevExpress.XtraBars;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.Preview;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraPrinting.Export;
using DevExpress.XtraPrinting.Native.Lines;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.XtraPrinting.Exports;
using DevExpress.XtraPrinting.Native.ExportOptionsControllers;
namespace DevExpress.XtraPrinting.Native {
	public static class DesignHelpers {
		public static Form GetForm(IContainer container) {
			return GetTypeFromContainer(container, typeof(Form)) as Form;
		}
		public static BarManager GetBarManager(IContainer container) {
			return GetTypeFromContainer(container, typeof(BarManager)) as BarManager;
		}
		static object FindComponentInternal(IContainer container, Predicate<Type> condition) {
			if(container == null) return null;
			foreach(object obj in container.Components) {
				if(condition(obj.GetType())) return obj;
			}
			return null;
		}
		public static object FindComponent(IContainer container, Type componentType) {
			return FindComponentInternal(container, delegate(Type testingObjectType) {
				return testingObjectType == componentType;
			});
		}
		public static IComponent FindInheritedComponent(IContainer container, Type componentType) {
			return (IComponent)FindComponentInternal(container, delegate(Type testingObjectType) {
				return testingObjectType.IsSubclassOf(componentType);
			});
		}
		public static IComponent FindSameOrInheritedComponent(IContainer container, Type componentType) {
			return (IComponent)FindComponentInternal(container, delegate(Type testingObjectType) {
				return testingObjectType == componentType || testingObjectType.IsSubclassOf(componentType);
			});
		}
		public static ContainerControl GetContainerControl(IContainer container) {
			if(GetForm(container) != null) return GetForm(container);
			return GetUserControl(container);
		}
		public static ContainerControl GetUserControl(IContainer container) {
			foreach(object obj in container.Components) {
				ContainerControl ctrl = obj as ContainerControl;
				if(ctrl != null && ctrl.ParentForm == null) return ctrl;
			}
			return null;
		}
		static object GetTypeFromContainer(IContainer container, Type type) {
			if(container == null || type == null) return null;
			foreach(object obj in container.Components) {
				if(type.IsInstanceOfType(obj)) return obj;
			}
			return null;
		}
		public static Component CreateComponent(IDesignerHost designerHost, Type componentType) {
			IToolboxUser toolboxUser = designerHost.GetDesigner(designerHost.RootComponent) as IToolboxUser;
			if(toolboxUser != null) {
				toolboxUser.ToolPicked(new ToolboxItem(componentType));
			}
			return FindComponent(designerHost.Container, componentType) as Component;
		}
	}
	public static class EditorContextMenuLookAndFeelHelper {
		public static void InitBarManager(ref System.ComponentModel.IContainer components, DevExpress.XtraEditors.XtraForm form) {
			if(components == null)
				components = new System.ComponentModel.Container();
			DevExpress.XtraBars.BarManager barManager = new XtraBars.BarManager(components);
			barManager.Controller = new DevExpress.XtraBars.BarAndDockingController();
			barManager.Controller.LookAndFeel.ParentLookAndFeel = form.LookAndFeel;
			barManager.Form = form;
		}
	}
	public static class RTLHelper {
		public static void ConvertGroupControlAlignments(DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup) {
			if(layoutControlGroup == null)
				return;
			foreach(DevExpress.XtraLayout.BaseLayoutItem item in new DevExpress.XtraLayout.Helpers.FlatItemsList().GetItemsList(layoutControlGroup)) {
				DevExpress.XtraLayout.LayoutControlItem layoutControlItem = item as DevExpress.XtraLayout.LayoutControlItem;
				if(layoutControlItem != null) {
					layoutControlItem.ControlAlignment = GraphicsConvertHelper.RTLContentAlignment(layoutControlItem.ControlAlignment);
				}
			};
		}
	}
}
namespace DevExpress.Utils {
	public static class LayoutHelper { 
		public static void DoButtonLayout(params Control[] controls) {
			Guard.ArgumentNotNull(controls, "controls");
			Guard.ArgumentPositive(controls.Length, "controls.Length");
			int maxWidth = 0;
			foreach(Control c in controls) {
				DevExpress.XtraEditors.BaseControl bc = c as DevExpress.XtraEditors.BaseControl;
				Size s = bc != null ? bc.CalcBestSize() : c.GetPreferredSize(c.Size);
				maxWidth = Math.Max(maxWidth, Math.Max(s.Width, c.Width));
			}
			Array.Sort(controls, (x, y) => -x.Left.CompareTo(y.Left));
			int right = controls[0].Right;
			int oldRight = controls[0].Right;
			foreach(Control c in controls) {
				int margin = oldRight - c.Right;
				oldRight = c.Left;
				c.Width = maxWidth;
				right = c.Left = right - margin - maxWidth;
			}
		}
		public static void DoLabelsEditorsLayout(DevExpress.XtraEditors.BaseControl[] labels, DevExpress.XtraEditors.BaseControl[] editors) {
			int delta = 0;
			foreach(var c in labels) {
				Size size = c.CalcBestSize();
				if(size.Width > c.Width) {
					delta = Math.Max(delta, size.Width - c.Width);
					c.Width = size.Width;
				}
			}
			if(delta > 0) {
				foreach(var c in editors) {
					c.Width -= delta;
					c.Left += delta;
				}
			}
		}
	}
}
namespace DevExpress.Utils.Drawing.Animation {
	class CallbackObjectWithBounds : IXtraObjectWithBounds {
		Func<Rectangle> getCallback;
		Action<Rectangle> setCallback;
		Rectangle IXtraObjectWithBounds.AnimatedBounds {
			get {
				return getCallback();
			}
			set {
				setCallback(value);
			}
		}
		void IXtraObjectWithBounds.OnEndBoundAnimation(BoundsAnimationInfo anim) {
		}
		public CallbackObjectWithBounds(Func<Rectangle> getCallback, Action<Rectangle> setCallback) {
			this.getCallback = getCallback;
			this.setCallback = setCallback;
		}
	}
	class CallbackSupportAnimation : ISupportXtraAnimation {
		Predicate<Control> callback;
		Control owner;
		public CallbackSupportAnimation(Control owner, Predicate<Control> callback) {
			this.owner = owner;
			this.callback = callback;
		}
		bool ISupportXtraAnimation.CanAnimate {
			get { return callback(owner); }
		}
		Control ISupportXtraAnimation.OwnerControl {
			get { return owner; }
		}
	}
}
