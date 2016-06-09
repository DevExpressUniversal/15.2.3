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
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Utils;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Design.SnapLines;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.XtraReports.Design.Behaviours;
using System.Linq;
namespace DevExpress.XtraReports.Design {
	public interface IKeyTarget {
		void HandleKeyPress(object sender, KeyPressEventArgs e);
	}
	public interface IMouseTarget {
		void HandleMouseMove(object sender, BandMouseEventArgs e);
		void HandleMouseDown(object sender, BandMouseEventArgs e);
		void HandleMouseUp(object sender, BandMouseEventArgs e);
		void HandleDoubleClick(object sender, BandMouseEventArgs e);
		void HandleMouseLeave(object sender, EventArgs e);
		bool ContainsPoint(Point pt, BandViewInfo viewInfo);
		void CommitSelection(RectangleF bounds, IComponent defaultSelection);
		bool IsDisposed { get; }
	}
	public class XRDesignerMethodWrapper : XRDesignerVerbBase {
		DesignerActionMethodItem actionMethod;
		public XRDesignerMethodWrapper(DesignerActionMethodItem actionMethod)
			: base(actionMethod.DisplayName, null, true, false) {
			this.actionMethod = actionMethod;
		}
		public override void Invoke() {
			actionMethod.Invoke();
		}
	}
	[
	MouseTarget(typeof(ControlMouseTarget)),
	DesignerBehaviour(typeof(DesignerBehaviour)),
	]
	public class XRControlDesigner : XRControlDesignerBase, IXRControlDesigner {
		#region static
		public static bool IsControlRotatable(XRControl control, IDesignerHost host) {
			XRControlDesigner designer = host.GetDesigner(control) as XRControlDesigner;
			return designer != null ? designer.IsRotatable : false;
		}
		public static XRControl[] GetControlsFrom(IDesignerHost designerHost, ICollection components) {
			List<XRControl> list = new List<XRControl>();
			foreach(IComponent item in components) {
				XRControl ctl = FrameSelectionUIService.GetAsXRControl(designerHost, item);
				if(ctl != null && LockService.GetInstance(designerHost).CanChangeComponent(ctl))
					list.Add(ctl);
			}
			return list.ToArray();
		}
		#endregion
		public enum DesignerState { None, ControlMoving, ControlCreating, ControlResizing }
		public override Band Band {
			get { return XRControl.Band; }
		}
		protected internal virtual bool IsRotatable {
			get { return false; }
		}
		public XRControlDesigner()
			: base() {
		}
		public override SelectionRules GetSelectionRules() {
			SelectionRules rules = GetSelectionRulesCore();
			if(Locked)
				rules &= ~SelectionRules.AllSizeable;
			return rules;
		}
		public virtual void MoveControl(XRControl parent, PointF pixelPoint, RectangleF screenRect) {
			PointF pt = ZoomService.GetInstance(this.DesignerHost).FromScaledPixels(pixelPoint, this.XRControl.Dpi);
			XRControlDesignerBase.RaiseComponentChanging(this.changeService, this.XRControl, XRComponentPropertyNames.Location);
			PointF oldValue = this.XRControl.LocationF;
			this.XRControl.LocationF = pt;
			XRControlDesignerBase.RaiseComponentChanged(this.changeService, this.XRControl, XRComponentPropertyNames.Location, oldValue, this.XRControl.LocationF);
			ChangeParent(parent);
		}
		PropertyDescriptorCollection IXRControlDesigner.GetBindableProperties(Type converterType) { return GetBindableProperties(converterType); }
		protected internal PropertyDescriptorCollection GetBindableProperties(Type conterterType) {
			PropertyDescriptorCollection designProperties = new PropertyDescriptorCollection(new PropertyDescriptor[] { });
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(XRControl, null);
			Attribute[] attrs = new Attribute[] { new NotifyParentPropertyAttribute(true), new TypeConverterAttribute(conterterType) };
			foreach(PropertyDescriptor property in properties) {
				Attribute attr = property.Attributes[typeof(BindableAttribute)];
				if(attr is BindableAttribute && ((BindableAttribute)attr).Bindable && ShouldAddBindableProperty(property))
					designProperties.Add(new DataBindingPropertyDescriptor(property, attrs));
			}
			return designProperties;
		}
		protected virtual bool ShouldAddBindableProperty(PropertyDescriptor property) {
			return true;
		}
		protected virtual SelectionRules GetSelectionRulesCore() {
			return SelectionRules.AllSizeable | SelectionRules.Moveable;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				XRControl.DrawInternal -= new DevExpress.XtraReports.UI.DrawEventHandler(xrControl_Draw);
			}
			base.Dispose(disposing);
		}
		protected internal virtual DesignBinding GetDesignBinding(string propName) {
			if(XRControl != null)
				return DesignBindingHelper.CreateDesignBinding(XRControl.DataBindings[propName]);
			return new DesignBinding();
		}
		protected internal virtual void SetBinding(string propName, DesignBinding value) {
			if(XRControl == null)
				return;
			XRBinding xrBinding = XRControl.DataBindings[propName];
			DesignBinding binding = DesignBindingHelper.CreateDesignBinding(xrBinding);
			if(binding.Equals(value) || value == null)
				return;
			DesignerTransaction transaction = DesignerHost.CreateTransaction(XRComponentPropertyNames.DataBindings);
			try {
				RaiseComponentChanging(XRControl, XRComponentPropertyNames.DataBindings);
				if(value.IsNull && xrBinding != null)
					XRControl.DataBindings.Remove(xrBinding);
				else if(!value.IsNull && xrBinding == null) {
					XRControl.DataBindings.Add(XRBinding.Create(propName, value.DataSource, value.DataMember, string.Empty));
				} else if(!value.IsNull && xrBinding != null)
					xrBinding.Assign(value.DataSource, value.DataMember);
				RaiseComponentChanged(XRControl);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
		protected internal virtual void SetBindingFormatString(string propName, string formatString) {
			if(XRControl == null)
				return;
			XRBinding xrBinding = XRControl.DataBindings[propName];
			if(formatString == null)
				formatString = String.Empty;
			if(xrBinding == null || Object.Equals(formatString, xrBinding.FormatString))
				return;
			DesignerTransaction transaction = DesignerHost.CreateTransaction(XRComponentPropertyNames.DataBindings);
			try {
				RaiseComponentChanging(XRControl, XRComponentPropertyNames.DataBindings);
				xrBinding.FormatString = formatString;
				RaiseComponentChanged(XRControl);
			} catch {
				transaction.Cancel();
			} finally {
				transaction.Commit();
			}
		}
		internal string GetBindablePropName() {
			AttributeCollection attributes = TypeDescriptor.GetAttributes(XRControl);
			foreach(Attribute attribute in attributes)
				if(attribute is DefaultBindablePropertyAttribute)
					return ((DefaultBindablePropertyAttribute)attribute).Name;
			return "Text";
		}
		internal virtual string GetBindablePropName(DataInfo data) {
			return GetBindablePropName();
		}
		public override string GetStatus() {
			try {
				return String.Format("{0} {{ {5}:{1},{2} {6}:{3},{4} }}", Component.Site.Name, 
					(int)Math.Round(XRControl.LeftF), 
					(int)Math.Round(XRControl.TopF), 
					(int)Math.Round(XRControl.WidthF), 
					(int)Math.Round(XRControl.HeightF),
					ReportLocalizer.GetString(ReportStringId.DesignerStatus_Location), ReportLocalizer.GetString(ReportStringId.DesignerStatus_Size));
			} catch { return string.Empty; }
		}
		public override void Initialize(IComponent component) {
			base.Initialize(component);
			XRControl.DrawInternal += new DevExpress.XtraReports.UI.DrawEventHandler(xrControl_Draw);
		}
		private void xrControl_Draw(object sender, DevExpress.XtraReports.UI.DrawEventArgs e) {
			IDesignerBehaviour behaviour = GetBehaviour();
			if(behaviour != null)
				behaviour.AdornControl(new ControlDrawEventArgs(e, ShouldDrawReportExplorerImage));
		}
		IDesignerBehaviour GetBehaviour() {
			IDesignerBehaviourService serv = GetService(typeof(IDesignerBehaviourService)) as IDesignerBehaviourService;
			return serv != null ? serv.GetBehaviour(this) : null;
		}
		protected virtual bool ShouldDrawReportExplorerImage {
			get { return false; }
		}
		public virtual void UpdateSelectionBoxes() {
		}
		public void ValidateControlBindings(object dataSource) {
			List<XRBinding> bindings = new List<XRBinding>(XRControl.DataBindings);
			IEnumerable<XRBinding> dataBindings = bindings.Where<XRBinding>(item => ReferenceEquals(item.DataSource, dataSource));
			IEnumerable<XRBinding> paramBindings = bindings.Where<XRBinding>(item => ReferenceEquals(item.Parameter, dataSource));
			if(dataBindings.Any<XRBinding>() || paramBindings.Any<XRBinding>()) {
				RaiseComponentChanging(XRControl, XRComponentPropertyNames.DataBindings);
				foreach(XRBinding binding in dataBindings)
					binding.Assign(null, binding.DataMember);
				foreach(XRBinding binding in paramBindings)
					XRControl.DataBindings.Remove(binding);
				RaiseComponentChanged(XRControl);
			}
		}
		public override void InitializeNewComponentCore() {
			CorrectControlHeight();
			base.InitializeNewComponentCore();
			IDesignerBehaviour behaviour = GetBehaviour();
			behaviour.SetDefaultComponentBounds();
		}
		protected virtual void CorrectControlHeight() {
			RectangleF dragBounds = CapturePaintService.GetDragBounds(DesignerHost);
			if(dragBounds.Height != 0)
				XRControl.HeightF = zoomService.FromScaledPixels(dragBounds.Height, XRControl.Dpi);
		}
		protected override XRControl FindParent() {
			return ParentSearchHelper.FindParent(RootReport, XRControl.GetType(), DesignerHost);
		}
		public virtual void SetLocation(PointF value, SizeF stepSize, RectangleSpecified specified, bool raiseChanged) {
			XRControlDesignerBase.RaiseComponentChanging(this.changeService, XRControl, XRComponentPropertyNames.Location);
			float y = XRControl.TopF;
			if((specified & RectangleSpecified.Y) > 0) {
				Band band = this.XRControl.Parent as Band;
				y = !CanChangeBandHeight(band, value.Y + XRControl.HeightF) ?
					Math.Min(value.Y, band.HeightF - XRControl.HeightF) :
					value.Y;
			}
			float x = (specified & RectangleSpecified.X) > 0 ? value.X : XRControl.LeftF;
			XRControl.LocationF = new PointF(x, y);
			if(raiseChanged)
				XRControlDesignerBase.RaiseComponentChanged(this.changeService, XRControl);
		}
		public override void SetSize(SizeF value, bool raiseChanged) {
			XRControlDesignerBase.RaiseComponentChanging(this.changeService, XRControl, XRComponentPropertyNames.Size);
			XRControl.SizeF = value;
			if(raiseChanged)
				XRControlDesignerBase.RaiseComponentChanged(this.changeService, XRControl);
		}
		public override void SetRightBottom(PointF value, SizeF stepSize, RectangleSpecified specified, bool raiseChanged) {
			float width = (specified & RectangleSpecified.Width) > 0 ?
				XRControl.ValidateRight(value.X, stepSize.Width) - XRControl.LeftF :
				XRControl.WidthF;
			float height = XRControl.HeightF;
			if((specified & RectangleSpecified.Height) > 0) {
				float bottom = XRControl.ValidateBottom(value.Y, stepSize.Height);
				Band band = this.XRControl.Parent as Band;
				bottom = !CanChangeBandHeight(band, bottom) ?
					Math.Min(bottom, band.HeightF) :
					bottom;
				height = bottom - XRControl.TopF;
			}
			SetSize(new SizeF(width, height), raiseChanged);
		}
		bool CanChangeBandHeight(Band band, float bottom) {
			return LockService.GetInstance(this.DesignerHost).CanChangeBandHeight(band, bottom);
		}
		public virtual bool ShouldSnapBounds {
			get {
				return ReportDesigner.RootReport.SnappingMode == SnappingMode.SnapToGrid && ReportDesigner.SnapToGrid;
			}
		}
		static RectangleF GetSnappedBounds(RectangleF r, SizeF gridSize) {
			r.Location = Divider.GetDivisibleValue(r.Location, gridSize);
			r.Size = Divider.GetDivisibleValue(r.Size, gridSize);
			r.Size = NativeMethods.GetMaxSize(r.Size, gridSize);
			return r;
		}
		protected void HandleGrabTick(object sender, EventArgs e) {
			IDesignerBehaviour behaviour = GetBehaviour();
			behaviour.ProcessGrabHandle();
		}
		protected override void OnLoadComplete(EventArgs e) {
			base.OnLoadComplete(e);
			if(Locked)
				Verbs.Clear();
		}
		public void ChangeParent(XRControl newParent) {
			XRControl oldParent = XRControl.Parent;
			if(oldParent == newParent)
				return;
			XRControlDesignerBase.RaiseCollectionChanging(newParent, changeService);
			XRControlDesignerBase.RaiseCollectionChanging(oldParent, changeService);
			XRControl.Parent = newParent;
			XRControlDesignerBase.RaiseCollectionChanged(oldParent, changeService);
			XRControlDesignerBase.RaiseCollectionChanged(newParent, changeService);
		}
	}
}
