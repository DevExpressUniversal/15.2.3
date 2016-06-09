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
using System.ComponentModel;
using DevExpress.LookAndFeel;
using System.Collections;
using System.Collections.Generic;
using DevExpress.Utils.Design;
namespace DevExpress.XtraLayout {
	public interface ISupportPropertyGridWrapper {
		Type WrapperType { get;}
	}
	public abstract class BasePropertyGridObjectWrapper : IDXObjectWrapper {
		ISupportPropertyGridWrapper wrappedObjectCore = null;
		protected BasePropertyGridObjectWrapper() { }
		public void SetWrappedObject(ISupportPropertyGridWrapper wrappedObject) {
			this.wrappedObjectCore = wrappedObject;
		}
		protected ISupportPropertyGridWrapper WrappedObject {
			get { return wrappedObjectCore; }
		}
		protected Type WrappedType {
			get { return WrappedObject.GetType(); }
		}
		object IDXObjectWrapper.SourceObject {
			get { return wrappedObjectCore; }
		}
		public abstract BasePropertyGridObjectWrapper Clone();
	}
}
namespace DevExpress.XtraLayout.Customization.Controls {
	[DesignTimeVisible(true), ToolboxItem(false)]
	[Designer(LayoutControlConstants.ButtonsPanelDesignerName, typeof(System.ComponentModel.Design.IDesigner)), ToolboxBitmap(typeof(LayoutControl), "Images.property-grid.bmp")]
	public class CustomizationPropertyGrid : DevExpress.XtraEditors.Designer.Utils.DXPropertyGridEx, ICustomizationFormControl {
		Dictionary<Type, BasePropertyGridObjectWrapper> wrappersCore = null;
		public CustomizationPropertyGrid() {
			wrappersCore = new Dictionary<Type, BasePropertyGridObjectWrapper>();
		}
		protected Dictionary<Type, BasePropertyGridObjectWrapper> Wrappers {
			get { return wrappersCore; }
		}
		public void Register() {
			if(OwnerControl != null)
				((ILayoutControlOwner)OwnerControl).ItemSelectionChanged += OwnerSelectionChanged;
		}
		public void UnRegister() {
			if(OwnerControl != null)
				((ILayoutControlOwner)OwnerControl).ItemSelectionChanged -= OwnerSelectionChanged;
			if(SelectedObjects != null && SelectedObjects.Length > 0) SelectedObjects = new object[0];
		}
		public ILayoutControl OwnerControl {
			get { return GetOwnerControl(); }
		}
		protected override void OnPaint(PaintEventArgs e) {
			if(OwnerControl == null) { WrongParentTypeMessagePainter.Default.Draw(new Rectangle(0, 0, Width, Height), e); return; }
			base.OnPaint(e);
		}
		protected virtual ILayoutControl GetOwnerControl() {
			return OwnerControlHelper.GetOwnerControl(Parent);
		}
		public UserLookAndFeel ControlOwnerLookAndFeel {
			get { return OwnerControl != null ? OwnerControl.LookAndFeel : null; }
		}
		protected internal virtual void OwnerSelectionChanged(object sender, EventArgs e) {
			if(isDisposing) return;
			SelectedObjects = GetSelection();
		}
		protected object[] GetSelection() {
			List<object> selection = new List<object>();
			if(OwnerControl != null && OwnerControl.Items != null)
				foreach(BaseLayoutItem item in OwnerControl.Items) {
					if(item.Selected && !item.IsHidden) selection.Add(GetObjectWrapper(item));
				}
			return selection.ToArray();
		}
		protected BasePropertyGridObjectWrapper GetObjectWrapper(object obj) {
			if(obj == null || !(obj is ISupportPropertyGridWrapper)) return null;
			return GetObjectWrapperCore(obj as ISupportPropertyGridWrapper);
		}
		protected virtual BasePropertyGridObjectWrapper GetObjectWrapperCore(ISupportPropertyGridWrapper obj) {
			BasePropertyGridObjectWrapper wrapper = null;
			Type wrapperType = obj.WrapperType;
			if(Wrappers.ContainsKey(wrapperType)) wrapper = Wrappers[wrapperType].Clone();
			else {
				wrapper = (BasePropertyGridObjectWrapper)Activator.CreateInstance(wrapperType, false);
				if(wrapper != null) Wrappers.Add(wrapperType, wrapper);
			}
			if(wrapper != null) wrapper.SetWrappedObject(obj);
			return wrapper;
		}
		protected bool isDisposing = false;
		protected override void Dispose(bool disposing) {
			if(disposing) {
				isDisposing = true;
				UnRegister();
				if(Wrappers != null) {
					Wrappers.Clear();
					wrappersCore = null;
				}
			}
			base.Dispose(disposing);
		}
		protected override object GetService(Type service) {
			object resultService = base.GetService(service);
			if(resultService == null) {
				IComponent component = SelectedObject as IComponent;
				if(component == null) {
					IDXObjectWrapper wrapper = SelectedObject as IDXObjectWrapper;
					if(wrapper != null) {
						component = wrapper.SourceObject as IComponent;
					}
				}
				if(component != null && component.Site != null) {
					resultService = component.Site.GetService(service);
				}
			}
			return resultService;
		}
	}
}
