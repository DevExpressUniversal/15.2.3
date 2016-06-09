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
using System.ComponentModel.Design;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms.Design;
using System.Drawing.Design;
using System.Collections;
using System.Collections.Generic;
using DevExpress.XtraReports.UI;
using System.Reflection;
using DevExpress.XtraReports.Design.Commands;
using DevExpress.XtraReports.Native;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using DevExpress.Utils.Design;
using DevExpress.XtraReports.Localization;
using DevExpress.Skins;
using System.Diagnostics;
using DevExpress.Utils.Drawing.Helpers;
using DevExpress.Utils.Win;
namespace DevExpress.XtraReports.Design {
	#region wrappers
	public class XRDesignerActionListCollection : IDesignerActionListCollection {
		readonly DesignerActionListCollection actionListCollection;
		public XRDesignerActionListCollection(DesignerActionListCollection actionListCollection) {
			Guard.ArgumentNotNull(actionListCollection, "actionListCollection");
			this.actionListCollection = actionListCollection;
		}
		public int Count { get { return actionListCollection.Count; } }
		public IDesignerActionList this[int index] { get { return new XRDesignerActionList(actionListCollection[index]); } }
		public IEnumerator GetEnumerator() {
			foreach(DesignerActionList actionList in actionListCollection) {
				yield return new XRDesignerActionList(actionList);
			}
		}
	}
	public class XRDesignerActionList : IDesignerActionList {
		readonly DesignerActionList designerActionList;
		public XRDesignerActionList(DesignerActionList designerActionList) {
			this.designerActionList = designerActionList;
		}
		public IComponent Component { get { return designerActionList.Component; } }
		public object PropertiesContainer { get { return designerActionList; } }
		public PropertyDescriptor FilterProperty(string name, PropertyDescriptor property) {
			return ComponentDesignerActionList.FilterProperty(designerActionList, name, property);
		}
		public IDesignerActionItemCollection GetSortedActionItems() {
			return new XRDesignerActionItemCollection(designerActionList.GetSortedActionItems());
		}
	}
	public class XRDesignerActionItemCollection : IDesignerActionItemCollection {
		readonly DesignerActionItemCollection designerActionItemCollection;
		public XRDesignerActionItemCollection(DesignerActionItemCollection designerActionItemCollection) {
			Guard.ArgumentNotNull(designerActionItemCollection, "designerActionItemCollection");
			this.designerActionItemCollection = designerActionItemCollection;
		}
		public IEnumerator GetEnumerator() {
			foreach(DesignerActionItem designerActionItem in designerActionItemCollection) {
				DesignerActionMethodItem methodItem = designerActionItem as DesignerActionMethodItem;
				if(methodItem != null)
					yield return new XRDesignerActionMethodItem(methodItem);
				DesignerActionPropertyItem propertyItem = designerActionItem as DesignerActionPropertyItem;
				if(propertyItem != null)
					yield return new XRDesignerActionPropertyItem(propertyItem);
				yield return new XRDesignerActionItem(designerActionItem);
			}
		}
	}
	public class XRDesignerActionItem : IDesignerActionItem {
		readonly DesignerActionItem designerActionItem;
		public XRDesignerActionItem(DesignerActionItem designerActionItem) {
			this.designerActionItem = designerActionItem;
		}
		protected DesignerActionItem DesignerActionItem { get { return designerActionItem; } }
		public string DisplayName { get { return designerActionItem.DisplayName; } }
	}
	public class XRDesignerActionMethodItem : XRDesignerActionItem, IDesignerActionMethodItem {
		public XRDesignerActionMethodItem(DesignerActionMethodItem methodItem) : base(methodItem) {
		}
		string IDesignerActionPropertyItem.MemberName { get { return null; } }
		public void Invoke() {
			((DesignerActionMethodItem)DesignerActionItem).Invoke();
		}
	}
	public class XRDesignerActionPropertyItem : XRDesignerActionItem, IDesignerActionPropertyItem {
		public XRDesignerActionPropertyItem(DesignerActionPropertyItem propertyItem) : base(propertyItem) {
		}
		public string MemberName { get { return ((DesignerActionPropertyItem)DesignerActionItem).MemberName; } }
	}
	#endregion
	public class SmartTagServiceBase : IDisposable {
		#region static
		public static bool HasSmartTagPresentation(ComponentDesigner designer) {
			return designer != null && (designer.ActionLists.Count > 0 || designer.Verbs.Count > 0);
		}
		#endregion
		#region Inner classes
		public class PopupHelper {
			readonly SmartTagServiceBase smartTagSvc;
			readonly Control container;
			LinesContainer linesContainer;
			public PopupHelper(SmartTagServiceBase smartTagSvc, Control container) {
				this.smartTagSvc = smartTagSvc;
				this.container = container;
				linesContainer = new LinesContainer();
				container.Controls.Add(linesContainer);
			}
			protected Control Container { get { return container; } }
			public SmartTagServiceBase SmartTagSvc { get { return smartTagSvc; } }
			public UserLookAndFeel LookAndFeel {
				get {
					ISupportLookAndFeel supportLookAndFeel = Container as ISupportLookAndFeel;
					if(supportLookAndFeel == null)
						return null;
					else
						return supportLookAndFeel.LookAndFeel;
				}
			}
			public Size FillLinesController(BaseLineController[] controllers, Point location) {
				BaseLine[] lines = Array.ConvertAll<ILine, BaseLine>(BaseLineController.GetLines(controllers, new LineFactory()), delegate(ILine line) { return (BaseLine)line; });
				container.ForeColor = ReportsSkins.GetSkin(LookAndFeel).GetSystemColor(SkinPaintHelper.GetSkinElement(LookAndFeel, ReportsSkins.SkinPopupFormBackground).Color.ForeColor);
				linesContainer.FillWithLines(lines, LookAndFeel, container.MinimumSize.Width, 3, 8);
				linesContainer.Location = location;
				return linesContainer.Size;
			}
		}
		#endregion
		public virtual BaseLineController[] CreateLineControllers(ComponentDesigner designer) {
			return CreateLineControllers(designer.Verbs, new XRDesignerActionListCollection(designer.ActionLists));
		}
		public virtual BaseLineController[] CreateLineControllers(DesignerVerbCollection verbs, IDesignerActionListCollection actionListCollection) {
			List<string> controllersActionsNames = new List<string>();
			List<BaseLineController> controllers = new List<BaseLineController>();
			if(verbs != null && verbs.Count > 0) {
				foreach(DesignerVerb verb in verbs) {
					if(!IncludeInSmartTag(verb))
						continue;
					if(verb.Enabled && verb.Visible) {
						controllers.Add(new VerbLineController(verb));
						controllersActionsNames.Add(verb.Text);
					}
				}
			}
			List<BaseLineController> existControllers = new List<BaseLineController>();
			foreach(IDesignerActionList actionList in actionListCollection) {
				IDesignerActionItemCollection actionItemCollection = actionList.GetSortedActionItems();
				foreach(IDesignerActionItem actionItem in actionItemCollection) {
					if(controllersActionsNames.Contains(actionItem.DisplayName)) continue;
					BaseLineController controller = CreateLineController(actionItem, actionList);
					if(controller != null) {
						existControllers.Add(controller);
						controllersActionsNames.Add(actionItem.DisplayName);
					}
				}
				controllersActionsNames.Clear();
				if(existControllers.Count > 0) {
					if(controllers.Count > 0)
						controllers.Add(new SeparatorLineController());
					controllers.AddRange(existControllers);
					existControllers.Clear();
				}
			}
			BaseLineController[] resultArray = controllers.ToArray();
			controllers.Clear();
			return resultArray;
		}
		protected virtual bool IncludeInSmartTag(DesignerVerb verb) {
			return true;
		}
		protected virtual BaseLineController CreateLineController(IDesignerActionItem actionItem, IDesignerActionList actionList) {
			if(actionItem is IDesignerActionMethodItem) {
				return new MethodLineController((IDesignerActionMethodItem)actionItem);
			} else if(actionItem is IDesignerActionPropertyItem) {
				IDesignerActionPropertyItem actionPropertyItem = (IDesignerActionPropertyItem)actionItem;
				PropertyDescriptor property = XRAccessor.GetPropertyDescriptor(actionList.PropertiesContainer, actionPropertyItem.MemberName);
				if(property.PropertyType == typeof(Color))
					return new ColorPropertyLineController(actionPropertyItem, actionList);
				else if(property.PropertyType == typeof(bool))
					return new BooleanLineController(actionPropertyItem, actionList);
				else if(PSNativeMethods.IsFloatType(property.PropertyType)) {
					return new FloatNumericPropertyLineController(actionPropertyItem, actionList);
				} else if(PSNativeMethods.IsNumericalType(property.PropertyType))
					return new NumericPropertyLineController(actionPropertyItem, actionList);
				UITypeEditor editor = property.GetEditor(typeof(UITypeEditor)) as UITypeEditor;
				if(editor != null)
					return new EditorPropertyLineController(actionPropertyItem, actionList);
				else
					return new ComboBoxPropertyLineController(actionPropertyItem, actionList);
			}
			return null;
		}
		#region IDisposable
		public void Dispose() {
			Dispose(true);
		}
		protected virtual void Dispose(bool disposing) {
		}
		~SmartTagServiceBase() {
			Dispose(false);
			GC.SuppressFinalize(this);
		}
		#endregion
	}
	public class XRSmartTagService : SmartTagServiceBase {
		#region static
#if DEBUGTEST
		internal
#endif
 static Point GetScreenOffset(Rectangle rect, RectangleF smartTagRect) {
			Point point = new Point(0, 0);
			foreach(Screen screen in Screen.AllScreens) {
				if(screen.Bounds.Contains(rect) || !screen.Bounds.IntersectsWith(rect))
					continue;
				int dx = Math.Min(0, screen.Bounds.Right - rect.Right);
				int dy = Math.Min(0, screen.Bounds.Bottom - rect.Bottom);
				int dxLocation = Math.Min(0, screen.Bounds.Right - rect.Left);
				int dyLocation = Math.Min(0, screen.Bounds.Bottom - rect.Top);
				if((dx != 0 && dxLocation == 0) || (rect.Left - screen.Bounds.Left <= 0))
					point.X = -rect.Size.Width - (int)Math.Round(smartTagRect.Width);
				if((dy != 0 && dyLocation == 0) || (rect.Top - screen.Bounds.Top <= 0))
					point.Y = -rect.Size.Height + (int)Math.Round(smartTagRect.Height);
				break;
			}
			return point;
		}
		#endregion
		protected override bool IncludeInSmartTag(DesignerVerb verb) {
			return !(verb is XRDesignerVerbBase && !((XRDesignerVerbBase)verb).IncludeInSmartTag);
		}
		#region inner classes
		public class PopupForm : XtraForm, IPopupControl, IPopupOwner {
			static IPopupServiceControl popupServiceControl = new DevExpress.XtraEditors.Controls.HookPopupController();
			BaseLineController[] controllers;
			PopupFormViewInfo viewInfo;
			ReportPaintStyle paintStyle;
			PopupFormPainter painter;
			PopupHelper popupHelper;
			bool allowClosing = true;
			public PopupForm(XRSmartTagService smartTagSvc) {
				SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);
				SuspendLayout();
				ShowInTaskbar = false;
				FormBorderStyle = FormBorderStyle.None;
				ControlBox = false;
				TopLevel = true;
				StartPosition = FormStartPosition.Manual;
				MinimumSize = new Size(150, 50);
				ResumeLayout(false);
				viewInfo = new PopupFormViewInfo();
				UpdatePainter();
				popupHelper = new PopupHelper(smartTagSvc, this);
			}
			protected XRSmartTagService SmartTagSvc { get { return (XRSmartTagService)popupHelper.SmartTagSvc; } }
			public void ShowPopup(IntPtr handle) {
				SetOwnerWindow(handle);
				Show();
				popupServiceControl.PopupShowing(this);
			}
			public void HidePopup() {
				SetOwnerWindow(IntPtr.Zero);
				Hide();
				popupServiceControl.PopupClosed(this);
			}
			void SetOwnerWindow(IntPtr handle) {
				Win32.GetWindowLong(Handle, -8);
				Win32.SetWindowLong(Handle, -8, handle);
			}
			protected override void OnPaintBackground(PaintEventArgs e) {
				UpdateViewInfo();
				UpdatePainter();
				using(GraphicsCache cache = new GraphicsCache(e)) {
					ObjectPainter.DrawObject(cache, painter, viewInfo);
				}
			}
			protected override bool ProcessDialogKey(Keys keyData) {
				Keys keys = keyData & Keys.KeyCode;
				if((keyData & (Keys.Alt | Keys.Control)) == Keys.None && keys != Keys.Tab) {
					switch(keys) {
						case Keys.Escape:
							if(!LineHasError())
								SmartTagSvc.HidePopup(false);
							break;
						case Keys.Left:
						case Keys.Up:
						case Keys.Right:
						case Keys.Down:
							if(ProcessTabKey(keys == Keys.Right || keys == Keys.Down))
								return true;
							break;
					}
				}
				return base.ProcessDialogKey(keyData);
			}
			bool LineHasWindow(IntPtr handle) {
				foreach(BaseLineController controller in controllers) {
					BaseLine line = (BaseLine)controller.GetLine(null);
					Control control = line.GetPopupWindow();
					if(control != null && Object.Equals(handle, control.Handle))
						return true;
				}
				return false;
			}
			bool LineHasError() {
				foreach(BaseLineController controller in controllers) {
					BaseLine line = (BaseLine)controller.GetLine(null);
					if(line.HasError)
						return true;
				}
				return false;
			}
			public void SetCaption(string text) {
				viewInfo.Text = text;
			}
			public void FillForm(BaseLineController[] controllers) {
				UpdatePainter();
				painter.CalcObjectMinBounds(viewInfo);
				this.controllers = controllers;
				Size linesContainerSize = popupHelper.FillLinesController(controllers, new Point(0, viewInfo.CaptionHeight));
				Size = new Size(linesContainerSize.Width, viewInfo.CaptionHeight + linesContainerSize.Height);
				Invalidate();
			}
			void UpdateViewInfo() {
				viewInfo.Bounds = ClientRectangle;
			}
			void UpdatePainter() {
				ReportPaintStyle newPaintStyle = ReportPaintStyles.GetPaintStyle(LookAndFeel);
				if(newPaintStyle == paintStyle) return;
				paintStyle = newPaintStyle;
				painter = paintStyle.CreatePopupFormPainter(LookAndFeel);
			}
			bool IPopupControl.AllowMouseClick(Control control, Point mousePosition) {
				if(!allowClosing || SmartTagSvc.SmartTagScreenRectangle.Contains(mousePosition))
					return true;
				IntPtr current = control != null ? control.Handle : Win32.GetActiveWindow();
				while(current != IntPtr.Zero) {
					if(Control.FromHandle(current) == this)
						return true;
					current = GetParentHandle(current);
				}
				return false;
			}
			static IntPtr GetParentHandle(IntPtr control) {
				const int GWL_HWNDPARENT = -8;
				return new IntPtr(Win32.GetWindowLong(control, GWL_HWNDPARENT));
			}
			void IPopupControl.ClosePopup() {
				if(allowClosing) 
					SmartTagSvc.HidePopup();
			}
			Control IPopupControl.PopupWindow {
				get { return this; }
			}
			bool IPopupControl.SuppressOutsideMouseClick { get { return false; } }
			void IPopupOwner.DisableClosing() {
				allowClosing = false;
			}
			void IPopupOwner.EnableClosing() {
				allowClosing = true;
			}
		}
		#endregion
		IDesignerHost designerHost = null;
		PopupForm popupWindow;
		BaseLineController[] controllers;
		SmartTagSelectionItem smartTagSelectionItem;
		RectangleF SmartTagScreenRectangle { get { return smartTagSelectionItem != null ? smartTagSelectionItem.GetScreenRectangle() : RectangleF.Empty; } 
		}
		public IDesignerHost DesignerHost { get { return designerHost; }
		}
		public bool PopupIsVisible { get { return popupWindow != null && popupWindow.Visible; }
		}
		public PopupForm PopupWindow {
			get {
				if(popupWindow == null) {
					popupWindow = new PopupForm(this);
					DesignLookAndFeelHelper.SetParentLookAndFeel(popupWindow, designerHost);
				}
				return popupWindow;
			}
		}
		public XRSmartTagService(IDesignerHost designerHost) {
			this.designerHost = designerHost;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposePopupWindow();
				smartTagSelectionItem = null;
			}
		}
		void DisposePopupWindow() {
			if(popupWindow != null) {
				popupWindow.Dispose();
				popupWindow = null;
			}
		}
		void DisposeLines(bool disableUpdatingValue) {
			if(controllers != null) {
				foreach(BaseLineController controller in controllers) {
					BaseLine line = (BaseLine)controller.GetLine(null);
					if(line != null)
						line.DisableUpdatingValue = disableUpdatingValue;
					controller.Dispose();
				}
				controllers = null;
			}
		}
		public void ShowPopup(Point location, XRComponentDesigner designer, SmartTagSelectionItem smartTagSelectionItem) {
			if(!HasSmartTagPresentation(designer))
				return;
			this.smartTagSelectionItem = smartTagSelectionItem;
			PopupWindow.SetCaption(GetPopupCaption(designer));
			UpdateForm(designer, false);
			PopupWindow.Location = ValidateScreenLocation(location, PopupWindow.Size);
			PopupWindow.ShowPopup(Win32.GetActiveWindow());
			PopupWindow.Focus();
			InvalidateSmartTag();
			do {
				Win32.WaitMessage();
				Application.DoEvents();
			} while(PopupIsVisible);
		}
		static string GetPopupCaption(XRComponentDesigner designer) {
			string typeName = DisplayTypeNameHelper.GetDisplayTypeName(designer.Component.GetType());
			return string.Format(ReportLocalizer.GetString(ReportStringId.STag_Capt_Format), typeName, ReportLocalizer.GetString(ReportStringId.STag_Capt_Tasks));
		}
		Point ValidateScreenLocation(Point pt, Size size) {
			Rectangle rect = new Rectangle(pt, size);
			pt.Offset(GetScreenOffset(rect, SmartTagScreenRectangle));
			return pt;
		}
		public void UpdateForm(XRComponentDesigner designer, bool disableUpdatingValue) {
			PopupWindow.SuspendLayout();
			DisposeLines(disableUpdatingValue);
			controllers = CreateLineControllers(designer);
			PopupWindow.FillForm(controllers);
			PopupWindow.ResumeLayout(false);
		}
		public void UpdateSmartTagItem(SmartTagSelectionItem smartTagSelectionItem) {
			if(smartTagSelectionItem != null) {
				this.smartTagSelectionItem = smartTagSelectionItem;
				PopupWindow.Location = ValidateScreenLocation(smartTagSelectionItem.GetPopupFormLocation(), PopupWindow.Size);
			}
		}
		public void HidePopup() {
			HidePopup(true);
		}
		public void HidePopup(bool commitChanges) {
			if(PopupIsVisible) {
				if(commitChanges) {
					BaseLineController controller = GetFocusedLineController();
					if(controller != null) {
						BaseLine line = (BaseLine)controller.GetLine(null);
						line.CommitChanges();
					}
				}
				PopupWindow.HidePopup();
				DisposeLines(false);
				InvalidateSmartTag();
				smartTagSelectionItem = null;
			}
		}
		BaseLineController GetFocusedLineController() {
			if(controllers != null) {
				foreach(BaseLineController controller in controllers) {
					BaseLine line = (BaseLine)controller.GetLine(null);
					if(line.ContainsFocus)
						return controller;
				}
			}
			return null;
		}
		void InvalidateSmartTag() {
			if(smartTagSelectionItem != null)
				smartTagSelectionItem.Invalidate();
		}
	}
	#region lines
	class LinkLine : BaseLine {
		static readonly object LinkClickedEvent = new object();
		public event EventHandler LinkClicked {
			add { Events.AddHandler(LinkClickedEvent, value); }
			remove { Events.RemoveHandler(LinkClickedEvent, value); }
		}
		string linkText;
		protected LinkLabel linkLabel;
		public LinkLine() : base() {
		}
		public override void SetText(string text) {
			linkText = text;
		}
		public override void Init(UserLookAndFeel lookAndFeel) {
			linkLabel = new LinkLabelControl();
			linkLabel.TabStop = true;
			linkLabel.Dock = DockStyle.Left;
			linkLabel.AutoSize = true;
			linkLabel.BackColor = Color.Transparent;
			((LinkLabelControl)linkLabel).Initialize(lookAndFeel);
			linkLabel.TextAlign = ContentAlignment.MiddleLeft;
			linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(OnLinkClicked);
			linkLabel.Text = linkText;
			linkLabel.LinkBehavior = LinkBehavior.HoverUnderline;
			base.Init(lookAndFeel);
		}
		public override Size GetLineSize() {
			return linkLabel.GetPreferredSize(Size.Empty);
		}
		protected override Control[] GetControls() {
			return new Control[] { linkLabel };
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(linkLabel != null)
					linkLabel.LinkClicked -= new LinkLabelLinkClickedEventHandler(OnLinkClicked);
			}
			base.Dispose(disposing);
		}
		void OnLinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			EventHandler handler = (EventHandler)this.Events[LinkClickedEvent];
			if(handler != null) handler(this, e);
		}
	}
	#endregion
	public sealed class TypeDescriptorContext : RuntimeTypeDescriptorContext {
		private IServiceProvider serviceProvider;
		public override IContainer Container {
			get {
				return GetService(typeof(IContainer)) as IContainer;
			}
		}
		public TypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor propDesc, object instance) : base(propDesc, instance) {
			this.serviceProvider = serviceProvider;
		}
		public override object GetService(Type serviceType) {
			return this.serviceProvider.GetService(serviceType);
		}
		public override bool OnComponentChanging() {
			IComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.OnComponentChanging(this.Instance, this.PropertyDescriptor);
			return true;
		}
		public override void OnComponentChanged() {
			IComponentChangeService componentChangeService = GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(componentChangeService != null)
				componentChangeService.OnComponentChanged(this.Instance, this.PropertyDescriptor, null, null);
		}
	}
	public class PopupFormViewInfo : ObjectInfoArgs {
		int captionHeight;
		string text = string.Empty;
		Rectangle textBounds = Rectangle.Empty;
		public int CaptionHeight { get { return captionHeight; } set { captionHeight = value; }
		}
		public string Text { get { return text; } set { text = value; }
		}
		public Rectangle TextBounds { get { return textBounds; } set { textBounds = value; }
		}
		public Rectangle GetCaptionRect() {
			return new Rectangle(Bounds.Location, new Size(Bounds.Width, captionHeight));
		}
	}
}
