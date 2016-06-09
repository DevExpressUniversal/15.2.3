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

using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Primitive;
namespace DevExpress.XtraGauges.Win.Wizard {
	[ToolboxItem(false)]
	public abstract class BaseGaugeDesignerPage : XtraPanel, ISupportAcceptOrder {
		int acceptOrderCore;
		string captionCore;
		int navigationGroupCore;
		Image imageCore;
		GaugeDesignerControl ownerCore;
		public BaseGaugeDesignerPage(int group, int index, string caption, Image img) {
			this.navigationGroupCore = group;
			this.acceptOrderCore = index;
			this.captionCore = caption;
			this.imageCore = img;
			this.BorderStyle = BorderStyle.None;
			DoubleBuffered = true;
			OnCreate();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				OnDispose();
			}
			base.Dispose(disposing);
		}
		protected abstract void OnCreate();
		protected abstract void OnDispose();
		protected internal virtual void OnDesignerClosing() { }
		public int PageIndex {
			get { return acceptOrderCore; }
		}
		int ISupportAcceptOrder.AcceptOrder {
			get { return acceptOrderCore; }
			set { acceptOrderCore = value; }
		}
		public int NavigationGroup {
			get { return navigationGroupCore; }
		}
		public string Caption {
			get { return captionCore; }
			set { captionCore = value; }
		}
		public Image Image {
			get { return imageCore; }
		}
		protected internal GaugeDesignerControl Owner {
			get { return ownerCore; }
		}
		internal void SetDesignerControl(GaugeDesignerControl newOwner) {
			this.ownerCore = newOwner;
			OnSetDesignerControl(Owner);
		}
		public virtual string SaveSettings() { return null; }
		public virtual void LoadSettings(string property) { }
		protected abstract void OnSetDesignerControl(GaugeDesignerControl designer);
		protected internal abstract bool IsHidden { get; }
		protected internal abstract bool IsAllowed { get; }
		protected internal abstract bool IsModified { get; }
		protected internal abstract void ApplyChanges();
		protected internal abstract void LayoutChanged();
		protected internal virtual BaseElement<IRenderableElement> GetElementByDesignedClone(BaseElement<IRenderableElement> designedClone) {
			return null;
		}
		protected internal virtual BaseElement<IRenderableElement> GetDesignedCloneByElement(BaseElement<IRenderableElement> designedClone) {
			return null;
		}
		protected internal virtual bool ProcessElementRemoveCommand(BaseElement<IRenderableElement> designedClone, BaseElement<IRenderableElement> element) {
			return false;
		}
		protected internal virtual bool ProcessElementAddNewCommand(out BaseElement<IRenderableElement> newElement) {
			newElement = null;
			return false;
		}
		protected internal virtual bool ProcessElementDuplicateCommand(BaseElement<IRenderableElement> designedPrototype, out BaseElement<IRenderableElement> dupElement) {
			dupElement = null;
			return false;
		}
		protected internal virtual void UpdateContent() { }
	}
	public class BaseGaugeDesignerPageCollection :
		BaseChangeableList<BaseGaugeDesignerPage> {
	}
}
