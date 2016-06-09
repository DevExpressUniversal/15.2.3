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

	using DevExpress.Utils.Design;
	using System.ComponentModel.Design;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Drawing;
namespace DevExpress.Utils {
	public class ControlBoundsProvider : ISmartTagClientBoundsProvider {
		protected virtual Point OffsetLocation { get { return new Point(-5, -8); } }
		public virtual Rectangle GetBounds(IComponent component) {
			Control control = GetOwnerControl(component);
			if(control != null)
				return new Rectangle(OffsetLocation, control.Bounds.Size);
			return Rectangle.Empty;
		}
		public virtual Control GetOwnerControl(IComponent component) {
			return component as Control;
		}
	}
	public class ComponentActions {
		protected virtual IDesigner GetDesigner(IComponent component) {
			if(component == null) return null;
			IDesignerHost host = (IDesignerHost)component.Site.GetService(typeof(IDesignerHost));
			return host.GetDesigner(component);
		}
	}
	public class ControlActions : ComponentActions {
		public virtual void DockInParentContainer(IComponent component) {
			Control control = component as Control;
			if(control != null) control.Dock = DockStyle.Fill;
		}
		public virtual void UndockFromParentContainer(IComponent component) {
			Control control = component as Control;
			if(control != null) control.Dock = DockStyle.None;
		}
	}
	public class ControlFilter : ISmartTagFilter {
		public Control Control { get; internal set; }
		protected virtual bool AllowDock { get { return false; } }
		public virtual void SetComponent(IComponent component) {
			Control = component as Control;
		}
		public virtual bool FilterMethod(string MethodName, object actionMethodItem) {
			if(Control != null) {
				if(!AllowDock && (MethodName == "DockInParentContainer" || MethodName == "UndockFromParentContainer")) return false;
				if(Control.Dock == DockStyle.Fill && MethodName == "DockInParentContainer") return false;
				if(Control.Dock != DockStyle.Fill && MethodName == "UndockFromParentContainer") return false;
			}
			return true;
		}
		public virtual bool FilterProperty(MemberDescriptor descriptor) {
			return true;
		}
	}
	public class XtraScrollableControlFilter : ControlFilter {
		protected override bool AllowDock { get { return true; } }		
	}
	public class XtraScrollableControlActions : ControlActions {	}
	public class PanelControlFilter : XtraScrollableControlFilter {
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(!base.FilterProperty(descriptor)) return false;
			if(descriptor.Name == "AllowTouchScroll") return false;
			if(descriptor.Name == "AlwaysScrollActiveControlIntoView") return false;
			if(descriptor.Name == "ScrollBarSmallChange") return false;
			return true;
		}
	}
	public class PanelControlActions : XtraScrollableControlActions {
		public void ContentImage(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "ContentImage");
		}
	}
	public class GroupControlActions : PanelControlActions {
		public void CaptionImage(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "CaptionImage");
		}
		public void CaptionImageUri(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "CaptionImageUri");
		}
		public void AddCustomHeaderButtons(IComponent component) {
			IDesigner designer = GetDesigner(component);
			if(designer != null)
				EditorContextHelper.EditValue(designer, component, "CustomHeaderButtons");
		}
	}
	public class SplitContainerControlFilter : PanelControlFilter {
		public override bool FilterProperty(MemberDescriptor descriptor) {
			if(!base.FilterProperty(descriptor)) return false;
			if(descriptor.Name == "ShowCaption") return false;
			if(descriptor.Name == "Text") return false;
			if(descriptor.Name == "CaptionLocation") return false;
			return true;
		}
		public override bool FilterMethod(string MethodName, object actionMethodItem) {
			if(!base.FilterMethod(MethodName, actionMethodItem)) return false;
			if(MethodName == "CaptionImage") return false;
			return true;
		}
	}
}
