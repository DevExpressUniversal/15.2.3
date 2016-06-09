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
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using System.ComponentModel;
using DevExpress.XtraGauges.Base;
using System.ComponentModel.Design;
using System.Collections;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Model;
using System.Windows.Forms;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Windows.Forms;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
namespace DevExpress.XtraGauges.Core.Customization {
	public enum Events { MouseDown, MouseUp, MouseMove, MouseLeave }
	public class CustomizeManagerBaseState {
		protected CustomizeManager ownerCore;
		public CustomizeManagerBaseState(CustomizeManager owner) { ownerCore = owner; }
		public virtual CustomizeManagerBaseState ProcessMessage(Events eventType, MouseEventArgs mea) {
			return this;
		}
	}
	public class NormalState : CustomizeManagerBaseState {
		public NormalState(CustomizeManager owner)
			: base(owner) {
			ownerCore.UpdateRect(RectangleF.Empty);
			ownerCore.SetCursor(CursorInfo.Normal);
		}
		public override CustomizeManagerBaseState ProcessMessage(Events eventType, MouseEventArgs mea) {
			ownerCore.HotClient = ownerCore.CalcHitInfo(mea.Location);
			if(eventType == Events.MouseDown && mea.Button == MouseButtons.Left) {
				ownerCore.SelectedClient = ownerCore.HotClient;
				if(ownerCore.SelectedClient != null) return new NormalSelectedState(ownerCore);
			}
			return base.ProcessMessage(eventType, mea);
		}
	}
	public class NormalSelectedState : NormalState {
		public NormalSelectedState(CustomizeManager owner) : base(owner) { }
		public override CustomizeManagerBaseState ProcessMessage(Events eventType, MouseEventArgs mea) {
			Point p = mea.Location;
			CustomizationFrameItemBase item = ownerCore.CalcCustomizeFramesHitInfo(p);
			ownerCore.SelectedGaugeFrameHitTestEnabled = false;
			ownerCore.HotClient = ownerCore.CalcHitInfo(p);
			ownerCore.SelectedGaugeFrameHitTestEnabled = true;
			ICustomizationFrameClient oldSelected = ownerCore.SelectedClient;
			if(eventType == Events.MouseDown && mea.Button == MouseButtons.Left) {
				if(item != null) {
					ClickChecker.CheckDoubleClick(item, mea, item.OnDoubleClick);
					return new BeforeDraggingState(ownerCore, item, p);
				}
				if(oldSelected != ownerCore.HotClient) {
					ownerCore.SelectedClient = ownerCore.HotClient;
					return new NormalSelectedState(ownerCore);
				}
				if(ownerCore.HotClient == null) return new NormalState(ownerCore);
			}
			ownerCore.SetCursor(item != null ? item.Cursor : CursorInfo.Normal);
			return this;
		}
	}
	public class BeforeDraggingState : CustomizeManagerBaseState {
		protected CustomizationFrameItemBase dragItemCore = null;
		protected Point startPointCore = Point.Empty;
		protected int startDragDistance = 5;
		public BeforeDraggingState(CustomizeManager owner, CustomizationFrameItemBase dragItem, Point startPoint)
			: base(owner) {
			dragItemCore = dragItem;
			startPointCore = startPoint;
		}
		protected bool ShouldStartDragging(Point p) {
			return Math.Max(Math.Abs(p.X - startPointCore.X), Math.Abs(p.Y - startPointCore.Y)) > startDragDistance;
		}
		public override CustomizeManagerBaseState ProcessMessage(Events eventType, MouseEventArgs mea) {
			if(dragItemCore != null) ownerCore.SetCursor(dragItemCore.Cursor);
			Point p = new Point(mea.X, mea.Y);
			if(eventType == Events.MouseDown) {
				ClickChecker.CheckDoubleClick(dragItemCore, mea, dragItemCore.OnDoubleClick);
			}
			if(eventType == Events.MouseUp) {
				if(mea.Button == MouseButtons.Left) ClickChecker.DoDelayedClick(dragItemCore.OnLeftClick);
				return new NormalSelectedState(ownerCore);
			}
			if(eventType == Events.MouseMove) {
				if(ShouldStartDragging(p)) return new DraggingState(ownerCore, dragItemCore, startPointCore);
			}
			return base.ProcessMessage(eventType, mea);
		}
	}
	public class DraggingState : BeforeDraggingState {
		public DraggingState(CustomizeManager owner, CustomizationFrameItemBase dragItem, Point startPoint)
			: base(owner, dragItem, startPoint) {
			dragItem.OnDraggingStarted(startPoint);
		}
		public override CustomizeManagerBaseState ProcessMessage(Events eventType, MouseEventArgs mea) {
			if(dragItemCore != null) ownerCore.SetCursor(dragItemCore.Cursor);
			if(eventType == Events.MouseMove) {
				Rectangle clip = new Rectangle(Point.Empty, ownerCore.Container.Bounds.Size);
				Cursor.Current = (clip.Contains(mea.Location)) ? Cursors.Default : Cursors.No;
				Point p = new Point(Math.Max(Math.Min(mea.X, clip.Width), 0), Math.Max(Math.Min(mea.Y, clip.Height), 0));
				dragItemCore.OnDragging(p);
				return this;
			}
			else {
				dragItemCore.OnDraggingFinished();
				return new NormalSelectedState(ownerCore);
			}
		}
	}
	public class CustomizeManager : IDisposable {
		List<CustomizationFrameBase> selectedGaugeFrames = null;
		bool enabledCore = false;
		IGaugeContainer gaugeContainerCore;
		CustomizeManagerBaseState stateCore;
		ICustomizationFrameClient selectedClientCore;
		ICustomizationFrameClient hotClientCore;
		HotTrackFrame hotTrackFrameCore;
		public CustomizeManager(IGaugeContainer container) {
			gaugeContainerCore = container;
			selectedGaugeFrames = new List<CustomizationFrameBase>();
			hotTrackFrameCore = new HotTrackFrame();
			HotTrackFrame.Name = "Customization_HotTrackFrame";
			Reset();
		}
		public void Reset() {
			State = new NormalState(this);
		}
		protected HotTrackFrame HotTrackFrame {
			get { return hotTrackFrameCore; }
		}
		public IGaugeContainer Container {
			get { return gaugeContainerCore; }
		}
		public CustomizeManagerBaseState State {
			get { return stateCore; }
			set { stateCore = value; }
		}
		public bool Enabled {
			get { return enabledCore; }
			set { enabledCore = value; }
		}
		public ICustomizationFrameClient SelectedClient {
			get { return selectedClientCore; }
			set {
				if(!Enabled) return;
				bool valueCorrected = false;
				if(value != null && !IsMyClient(value)) {
					value = null;
					valueCorrected = true;
				}
				selectedClientCore = value;
				RaiseSelectionChanged();
				UpdateSelectedGaugeCustomizeFramesCollection();
				if(SelectedClient == null) Reset();
				if(!valueCorrected) SelectedComponentsChanged(null, null);
			}
		}
		public event EventHandler SelectionChanged;
		protected void RaiseSelectionChanged() {
			if(SelectionChanged != null)
				SelectionChanged(SelectedClient, EventArgs.Empty);
		}
		public ICustomizationFrameClient HotClient {
			get { return hotClientCore; }
			set {
				if(HotClient == value) return;
				hotClientCore = value;
				UpdateHotTrack();
			}
		}
		void UpdateHotTrack() {
			Rectangle oldRect = (Rectangle)HotTrackFrame.Box;
			oldRect.Inflate(1, 1);
			Rectangle newRect = oldRect;
			if(HotClient == null || HotClient == SelectedClient || HotClient is BaseGaugeModel) {
				Container.RemovePrimitive(HotTrackFrame);
				newRect = Container.Bounds;
			}
			else {
				newRect = HotClient.Bounds;
				newRect.Inflate(-1, -1);
				HotTrackFrame.Box = newRect;
				newRect = HotClient.Bounds;
				Container.AddPrimitive(HotTrackFrame);
			}
			Container.InvalidateRect(Rectangle.Union(oldRect, newRect));
		}
		public void UpdateRect(RectangleF rect) {
			Container.UpdateRect(rect);
		}
		public void SetCursor(CursorInfo ci) {
			Container.SetCursor(ci);
		}
		public RectangleF CorrectRenderRect(RectangleF rect) {
			foreach(CustomizationFrameBase frame in selectedGaugeFrames) {
				rect = frame.CorrectRenderRect(rect);
			}
			return rect;
		}
		bool selectedGaugeFramgeHitTestEnabledCore = true;
		public bool SelectedGaugeFrameHitTestEnabled {
			get { return selectedGaugeFramgeHitTestEnabledCore; }
			set {
				selectedGaugeFramgeHitTestEnabledCore = value;
				EnableSelectedGaugeFrames(selectedGaugeFramgeHitTestEnabledCore);
			}
		}
		protected virtual void EnableSelectedGaugeFrames(bool enable) {
			if(selectedGaugeFrames == null) return;
			foreach(BaseCompositePrimitive temp in selectedGaugeFrames) {
				temp.HitTestEnabled = enable;
			}
		}
		void DestroySelectedFrames() {
			foreach(BaseCompositePrimitive frame in selectedGaugeFrames) {
				Container.RemovePrimitive(frame);
				frame.Dispose();
			}
			selectedGaugeFrames.Clear();
		}
		public void ResetSelection() {
			DestroySelectedFrames();
			selectedClientCore = null;
		}
		public virtual void UpdateSelectedGaugeCustomizeFramesCollection() {
			DestroySelectedFrames();
			if(selectedClientCore != null) {
				CustomizationFrameBase[] elements = selectedClientCore.CreateCustomizeFrames();
				selectedGaugeFrames.AddRange(elements);
				foreach(BaseCompositePrimitive temp in selectedGaugeFrames)
					Container.AddPrimitive(temp);
			}
			IComponent selected = selectedClientCore as IComponent;
			if(selected == null && selectedClientCore is BaseGaugeModel) selected = (selectedClientCore as BaseGaugeModel).Owner;
			if(SelectedClient != null)
				Container.UpdateRect(SelectedClient.Bounds);
			else
				Container.UpdateRect(Rectangle.Empty);
		}
		protected ICustomizationFrameClient FindICustomizationFrameClientInParentNodes(IRenderableElement element) {
			if(element is ICustomizationFrameClient)
				return element as ICustomizationFrameClient;
			if(element is CustomizationFrameBase)
				return ((CustomizationFrameBase)element).Client;
			IRenderableElement parent = (element != null) ? element.Parent as IRenderableElement : null;
			return (parent != null) ? FindICustomizationFrameClientInParentNodes(parent) : null;
		}
		public ICustomizationFrameClient CalcHitInfo(Point p) {
			BasePrimitiveHitInfo hitInfo = Container.CalcHitInfo(p);
			return FindICustomizationFrameClientInParentNodes(hitInfo.Element);
		}
		public class CustomizationFrameBaseComparer : IComparer {
			public int Compare(object a, object b) {
				if(object.Equals(a, b)) return 0;
				CustomizationFrameBase fa = a as CustomizationFrameBase;
				CustomizationFrameBase fb = b as CustomizationFrameBase;
				if(fa != null && fb != null) 
					return fa.ZOrder.CompareTo(fb.ZOrder);
				return 0;
			}
		}
		public CustomizationFrameItemBase CalcCustomizeFramesHitInfo(Point p) {
			ArrayList frames = new ArrayList(selectedGaugeFrames);
			frames.Sort(new CustomizationFrameBaseComparer());
			foreach(CustomizationFrameBase frame in frames) {
				CustomizationFrameItemBase hitItem = frame.CalcHitTest(p);
				if(hitItem != null) return hitItem;
			}
			return null;
		}
		public virtual void ProcessMessage(Events eventType, MouseEventArgs mea) {
			if(Enabled) State = State.ProcessMessage(eventType, mea);
		}
		ISelectionService selectionService;
		public ISelectionService SelectionService {
			get {
				if(selectionService == null && ((IComponent)Container).Site != null) {
					selectionService = (ISelectionService)((IComponent)Container).Site.GetService(typeof(ISelectionService));
					SubscribeEvents();
				}
				return selectionService;
			}
		}
		protected virtual void UnSubscribeEvents() {
			if(SelectionService != null) {
				SelectionService.SelectionChanged -= new EventHandler(SelectedComponentsChanged);
			}
		}
		protected virtual void SubscribeEvents() {
			if(SelectionService != null) {
				SelectionService.SelectionChanged += new EventHandler(SelectedComponentsChanged);
			}
		}
		protected virtual void SetDTSelectedComponent() {
			if(SelectionService == null) return;
			IComponent selected = SelectedClient as IComponent;
			if(selected == null) {
				BaseGaugeModel bgm = SelectedClient as BaseGaugeModel;
				if(bgm != null) selected = bgm.Owner;
			}
			if(selected == null) selected = Container as IComponent;
			SelectionService.SetSelectedComponents((selected == null) ? new IComponent[0] : new IComponent[] { selected });
		}
		protected virtual ArrayList DTSelectedComponents {
			get {
				return new ArrayList(SelectionService.GetSelectedComponents());
			}
		}
		protected bool IsMyClient(ICustomizationFrameClient client) {
			IGauge gb = client as IGauge;
			BaseGaugeModel model = client as BaseGaugeModel;
			IElement<IRenderableElement> element = client as IElement<IRenderableElement>;
			if(gb != null && Container.Gauges.Contains(gb)) return true;
			if(model != null && Container.Gauges.Contains(model.Owner)) return true;
			IComposite<IRenderableElement> modelRoot = (element != null) ? element.Parent : null;
			while(true) {
				if(modelRoot == null || modelRoot is BaseGaugeModel) break;
				modelRoot = modelRoot.Parent;
			}
			if(modelRoot != null && Container.Gauges.Contains(((BaseGaugeModel)modelRoot).Owner)) return true;
			return false;
		}
		bool isDesignerSelectionChange = false;
		protected virtual void SelectedComponentsChanged(object sender, EventArgs ea) {
			if(isDesignerSelectionChange) return;
			isDesignerSelectionChange = true;
			try {
				if(sender == null) {
					SetDTSelectedComponent();
				}
				else {
					ICustomizationFrameClient client = null;
					IGauge gauge = null;
					foreach(object obj in DTSelectedComponents) {
						client = obj as ICustomizationFrameClient;
						gauge = obj as IGauge;
						if(client != null || gauge != null) break;
					}
					if(gauge != null) SelectedClient = gauge.Model;
					else {
						if(client != null) SelectedClient = client;
						else SelectedClient = null;
					}
				}
			}
			finally {
				isDesignerSelectionChange = false;
			}
		}
		void IDisposable.Dispose() {
			UnSubscribeEvents();
			SelectionChanged = null;
		}
	}
	partial class CustomizeActionInfo {
		string groupIdAction;
		string methodNameCore;
		string descriptionCore;
		string actionNameCore;
		Image imageCore;
		public CustomizeActionInfo(string methodName, string description, string descriptionShort, Image image)
			: this(methodName, description, descriptionShort, image, null) {
		}
		public CustomizeActionInfo(string methodName, string description, string descriptionShort, Image image, string groupIdAction) {
			this.Image = image;
			this.MethodName = methodName;
			this.ActionName = descriptionShort;
			this.Description = description;
			this.groupIdAction = groupIdAction;
		}
		public string GroupIdAction { get { return groupIdAction; } }
		public string MethodName {
			get { return methodNameCore; }
			set { methodNameCore = value; }
		}
		public string Description {
			get { return descriptionCore; }
			set { descriptionCore = value; }
		}
		public string ActionName {
			get { return actionNameCore; }
			set { actionNameCore = value; }
		}
		public Image Image {
			get { return imageCore; }
			set { imageCore = value; }
		}
	}
	public delegate void ClickHandler();
	public class ClickChecker {
		static Timer menuActivator = new Timer();
		const long tickLength = 0x2710L;
		static Point lastMouseDowntPoint;
		static long lastMouseDownTime;
		static CustomizationFrameItemBase lastMouseDownFrame;
		static bool fWaitSecondClickMode = false;
		static ClickHandler delayedClickHandler;
		static ClickChecker() {
			menuActivator = new Timer();
			menuActivator.Interval = Math.Max(SystemInformation.MenuShowDelay, 100);
			menuActivator.Tick += OnMenuActivatorTick;
			menuActivator.Stop();
		}
		static void OnMenuActivatorTick(object sender, EventArgs e) {
			menuActivator.Stop();
			if(delayedClickHandler != null) {
				Lock();
				delayedClickHandler();
				delayedClickHandler = null;
				Unlock();
			}
		}
		public static void DoDelayedClick(ClickHandler handler) {
			delayedClickHandler = handler;
			menuActivator.Start();
		}
		public static void CheckDoubleClick(CustomizationFrameItemBase frame, MouseEventArgs e, ClickHandler handler) {
			if(IsLocked) return;
			if((e.Button & MouseButtons.Left) == 0) {
				fWaitSecondClickMode = false;
				return;
			}
			if(fWaitSecondClickMode) {
				if(CheckSameFrameCondition(frame) & CheckTimeCondition(DateTime.Now.Ticks) & CheckLocationCondition(e.Location)) {
					menuActivator.Stop();
					delayedClickHandler = null;
					if(handler != null) handler();
				}
				fWaitSecondClickMode = false;
			}
			else {
				SaveFirstClick(frame, e.Location, DateTime.Now.Ticks);
				fWaitSecondClickMode = true;
				return;
			}
		}
		static int lockCounter = 0;
		protected static bool IsLocked {
			get { return lockCounter > 0; }
		}
		public static void Lock() {
			lockCounter++;
		}
		public static void Unlock() {
			if(--lockCounter == 0) Reset();
		}
		private static void Reset() {
			lastMouseDownFrame = null;
			lastMouseDowntPoint = Point.Empty;
			lastMouseDownTime = 0;
			fWaitSecondClickMode = false;
		}
		private static void SaveFirstClick(CustomizationFrameItemBase frame, Point point, long time) {
			lastMouseDownFrame = frame;
			lastMouseDowntPoint = point;
			lastMouseDownTime = time;
		}
		private static bool CheckSameFrameCondition(CustomizationFrameItemBase Frame) {
			bool condition = lastMouseDownFrame == Frame;
			lastMouseDownFrame = Frame;
			return condition;
		}
		private static bool CheckTimeCondition(long currentMouseDownTime) {
			bool condition = (Math.Abs(lastMouseDownTime - currentMouseDownTime) / tickLength) < SystemInformation.DoubleClickTime;
			lastMouseDownTime = currentMouseDownTime;
			return condition;
		}
		private static bool CheckLocationCondition(Point currentMouseEventPoint) {
			bool conditionX = Math.Abs(currentMouseEventPoint.X - lastMouseDowntPoint.X) <= SystemInformation.DoubleClickSize.Width;
			bool conditionY = Math.Abs(currentMouseEventPoint.Y - lastMouseDowntPoint.Y) <= SystemInformation.DoubleClickSize.Height;
			lastMouseDowntPoint = currentMouseEventPoint;
			return conditionX && conditionY;
		}
	}
}
