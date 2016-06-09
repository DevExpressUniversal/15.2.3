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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Design.Ruler;
using System.Drawing.Printing;
using System.Collections;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.XtraReports.Design.Adapters;
using DevExpress.XtraReports.Design.MouseTargets;
using DevExpress.Utils;
using DevExpress.XtraPrinting;
namespace DevExpress.XtraReports.Design {
	[ToolboxItem(false)]
	public class ReportFrame : Control, ISupportLookAndFeel {
		#region inner classes
		class CornerPanel : Panel {
			ReportFrame reportFrame;
			IComponent component;
			WinControlSmartTagSelectionItem smartTagSelectionItem;
			Image image;
			ReportPaintStyle paintStyle;
			CornerPanelPainter painter;
			CornerPanelViewInfo viewInfo;
			public CornerPanel() {
				SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlConstants.DoubleBuffer, true);
				image = ResLoader.LoadBitmap("Images.CornerGlyph.bmp", typeof(LocalResFinder), Color.Magenta);
				viewInfo = new CornerPanelViewInfo();
			}
			public void Initialize(ReportFrame reportFrame) {
				this.reportFrame = reportFrame;
				component = reportFrame.designerHost.RootComponent;
				smartTagSelectionItem = new WinControlSmartTagSelectionItem((XRControl)component, (XRComponentDesigner)reportFrame.designerHost.GetDesigner(component), this);
			}
			protected override void Dispose(bool disposing) {
				if(disposing) {
					if(image != null) {
						image.Dispose();
						image = null;
					}
				}
				base.Dispose(disposing);
			}
			protected override void OnPaint(PaintEventArgs e) {
				base.OnPaint(e);
				if(smartTagSelectionItem != null) {
					UpdateViewInfo();
					UpdatePainter();
					painter.CalcObjectMinBounds(viewInfo);
					viewInfo.HotTracked = HotTracked;
					using(GraphicsCache cache = new GraphicsCache(e.Graphics)) {
						ObjectPainter.DrawObject(cache, painter, viewInfo);
					}
				}
			}
			bool ContainsMouse {
				get {
					return viewInfo.Bounds.Contains(PointToClient(MousePosition));
				}
			}
			bool HotTracked {
				get {
					return ComponentHasSelection || ContainsMouse;
				}
			}
			bool ComponentHasSelection {
				get {
					IList components = GetSelectedComponents();
					return components.Count == 1 && Object.Equals(components[0], component);
				}
			}
			IList GetSelectedComponents() {
				if(reportFrame != null) {
					ISelectionService svc = reportFrame.designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
					return new ArrayList(svc.GetSelectedComponents());
				}
				return new object[] { };
			}
			protected override void OnMouseMove(MouseEventArgs e) {
				base.OnMouseMove(e);
				UpdateViewInfo();
				ApplayHotTracked(HotTracked);
			}
			void ApplayHotTracked(bool hotTracked) {
				if(hotTracked != viewInfo.HotTracked)
					Invalidate();
			}
			protected override void OnMouseLeave(EventArgs e) {
				base.OnMouseLeave(e);
				UpdateViewInfo();
				ApplayHotTracked(HotTracked);
			}
			protected override void OnMouseUp(MouseEventArgs e) {
				base.OnMouseUp(e);
				if(ComponentHasSelection && e.Button.IsRight())
					reportFrame.ShowContextMenu();
			}
			protected override void OnMouseDown(MouseEventArgs e) {
				if(reportFrame.designerHost.IsDebugging())
					return;
				if(smartTagSelectionItem != null && viewInfo.HotTracked) {
					reportFrame.SelectComponent(component);
					if(ComponentHasSelection)
						smartTagSelectionItem.HandleMouseDown(this, new BandMouseEventArgs(e));
				}
				base.OnMouseDown(e);
			}
			void UpdatePainter() {
				if(reportFrame.LookAndFeel != null) {
					ReportPaintStyle newPaintStyle = ReportPaintStyles.GetPaintStyle(reportFrame.LookAndFeel);
					if(newPaintStyle == paintStyle)
						return;
					paintStyle = newPaintStyle;
					if(painter != null)
						painter.Dispose();
					painter = paintStyle.CreateCornerPanelPainter(reportFrame.LookAndFeel);
				}
			}
			void UpdateViewInfo() {
				viewInfo.Bounds = Bounds;
				viewInfo.TagBounds = RectangleToClient(smartTagSelectionItem.TagScreenBounds);
			}
		}
		class ChangeWidthInfo {
			int startScreenX;
			bool rightMarginChanging;
			bool rightSideChanging;
			bool leftMarginChanging;
			public ChangeWidthInfo() {
				Reset();
			}
			public bool RacesOnTheRightSide { get { return rightMarginChanging && rightSideChanging; } }
			public bool RightMarginChanging { get { return rightMarginChanging; } }
			public bool RightSideChanging { get { return rightSideChanging; } }
			public bool LeftMarginChanging { get { return leftMarginChanging; } }
			void Reset() {
				startScreenX = 0;
				rightMarginChanging = false;
				rightSideChanging = false;
				leftMarginChanging = false;
			}
			public RulerState GetHRulerState(HRuler hRuler, int x) {
				RulerState result = hRuler.State;
				if (RacesOnTheRightSide) {
					float dx = x - startScreenX;
					result = dx >= 0 ? RulerState.None : RulerState.RightMarginChanged;
				}
				else if (rightMarginChanging)
					result = RulerState.RightMarginChanged;
				else if (rightSideChanging)
					result = RulerState.None;
				else if (leftMarginChanging)
					result = RulerState.LeftMarginChanged;
				return result;
			}
			public void Init(ReportFrame frame, BandViewInfo viewInfo, Point screenPoint) {
				Point pt = frame.framePanel.PointToClient(screenPoint);
				rightMarginChanging = viewInfo.RightMarginHitTestBounds.Contains(pt);
				rightSideChanging = frame.Report.PaperKind == PaperKind.Custom && viewInfo.RightSideHitTestBounds.Contains(pt);
				leftMarginChanging = viewInfo.LeftMarginHitTestBounds.Contains(pt);
				startScreenX = screenPoint.X;
			}
			public void Init() {
				Reset();
			}
		}
		#endregion
		public static int CaptionHeight = SystemFonts.DefaultFont.Height + 8;  
		const int HRulerOffset = 1;
		const int VRulerOffset = 0;
		private PanelControl splitPanel;
		private ScrollablePanel scrollPanel;
		private FramePanel framePanel;
		private Panel topPanel;
		private SplitterControl splitter;
		private IComponent component;
		private IDesignerHost designerHost;
		private SplitService splitService;
		private HRuler hRuler;
		private CornerPanel cornerPanel;
		private Panel vRulerPanel;
		private Panel hRulerPanel;
		private VRuler vRuler;
		private int rulerSectionIndex = -1;
		private UserLookAndFeel userLookAndFeel;
		ZoomService zoomService;
		#region ISupportLookAndFeel
		bool ISupportLookAndFeel.IgnoreChildren {
			get { return false; }
		}
		public UserLookAndFeel LookAndFeel {
			get { return userLookAndFeel; }
		}
		#endregion //ISupportLookAndFeel
		protected XtraReport Report {
			get { return component as XtraReport; }
		}
		public VRuler VRuler {
			get { return vRuler; }
		}
		public HRuler HRuler {
			get { return hRuler; }
		}
		public BandViewInfo[] BandViewInfos {
			get { return framePanel.ViewInfos; }
		}
		public ReportFrame(IComponent component) {
			userLookAndFeel = new ControlUserLookAndFeel(this);
			InitializeComponent();
			designerHost = component.Site.GetService(typeof(IDesignerHost)) as IDesignerHost;
			scrollPanel = new ScrollablePanel(designerHost);
			hRuler = new HRuler(designerHost);
			vRuler = new VRuler(designerHost);
			((ISupportInitialize)hRuler).BeginInit();
			((ISupportInitialize)vRuler).BeginInit();
			scrollPanel.Size = new Size(20, 20);
			scrollPanel.Dock = DockStyle.Fill;
			splitPanel.Controls.Add(scrollPanel);
			scrollPanel.BringToFront();
			hRulerPanel.Controls.Add(hRuler);
			hRuler.Dock = DockStyle.Fill;
			hRuler.BeginDrag += hRuler_BeginDrag;
			hRuler.SelectionChanged += Rulers_SelectionChanged;
			vRulerPanel.Controls.Add(vRuler);
			vRuler.Dock = DockStyle.Fill;
			vRuler.BeginDrag += vRuler_BeginDrag;
			vRuler.SliderClick += vRuler_SliderClick;
			vRuler.SelectionChanged += Rulers_SelectionChanged;
			((ISupportInitialize)hRuler).EndInit();
			((ISupportInitialize)vRuler).EndInit();
			BackColor = SystemColors.Control;
			scrollPanel.BackColor = ControlPaint.Light(SystemColors.Control);
			this.component = component;
			splitService = new SplitService(designerHost);
			designerHost.AddService(typeof(SplitService), splitService);
			designerHost.AddService(typeof(RulerService), new RulerService(GetReportDesigner(), hRuler, vRuler));
			designerHost.AddService(typeof(ScrollablePanel), scrollPanel);
			UpdateLookAndFeel();
			zoomService = ZoomService.GetInstance(designerHost);
			zoomService.ZoomChanged += OnZoomChanged;
			UpdateRulersSize();
			LookAndFeel.StyleChanged += LookAndFeel_StyleChanged;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				designerHost.RemoveService(typeof(AdornerService));
				zoomService.ZoomChanged -= OnZoomChanged;
				designerHost.RemoveService(typeof(IBandViewInfoService));
				designerHost.RemoveService(typeof(SplitService));
				designerHost.RemoveService(typeof(RulerService));
				designerHost.RemoveService(typeof(ScrollablePanel));
				if(userLookAndFeel != null) {
					userLookAndFeel.StyleChanged -= LookAndFeel_StyleChanged;
					userLookAndFeel.Dispose();
					userLookAndFeel = null;
				}
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.splitPanel = new DevExpress.XtraEditors.PanelControl();
			this.vRulerPanel = new System.Windows.Forms.Panel();
			this.topPanel = new System.Windows.Forms.Panel();
			this.hRulerPanel = new System.Windows.Forms.Panel();
			this.cornerPanel = new CornerPanel();
			this.splitter = new DevExpress.XtraEditors.SplitterControl();
			this.splitPanel.SuspendLayout();
			this.topPanel.SuspendLayout();
			this.SuspendLayout();
			this.splitPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.splitPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																					 this.vRulerPanel,
																					 this.topPanel});
			this.splitPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitPanel.Name = "splitPanel";
			this.splitPanel.Size = new System.Drawing.Size(488, 309);
			this.splitPanel.TabIndex = 0;
			this.vRulerPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.vRulerPanel.Location = new System.Drawing.Point(0, 20);
			this.vRulerPanel.Name = "vRulerPanel";
			this.vRulerPanel.Size = new System.Drawing.Size(20, 289);
			this.vRulerPanel.TabIndex = 21;
			this.topPanel.Controls.AddRange(new System.Windows.Forms.Control[] {
																				   this.hRulerPanel,
																				   this.cornerPanel});
			this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
			this.topPanel.Name = "topPanel";
			this.topPanel.Size = new System.Drawing.Size(488, 20);
			this.topPanel.TabIndex = 22;
			this.hRulerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.hRulerPanel.Location = new System.Drawing.Point(20, 0);
			this.hRulerPanel.Name = "hRulerPanel";
			this.hRulerPanel.Size = new System.Drawing.Size(468, 20);
			this.hRulerPanel.TabIndex = 1;
			this.cornerPanel.Dock = System.Windows.Forms.DockStyle.Left;
			this.cornerPanel.Name = "cornerPanel";
			this.cornerPanel.Size = new System.Drawing.Size(19, 19);
			this.cornerPanel.TabIndex = 0;
			this.splitter.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.splitter.Location = new System.Drawing.Point(0, 309);
			this.splitter.Name = "splitter";
			this.splitter.Size = new System.Drawing.Size(488, 3);
			this.splitter.TabIndex = 1;
			this.splitter.TabStop = false;
			this.splitter.Visible = false;
			this.splitter.SplitterMoved += OnComponentTraySplitterMoved;
			this.Controls.AddRange(new System.Windows.Forms.Control[] {
																		  this.splitPanel,
																		  this.splitter});
			this.Name = "ReportFrame";
			this.Size = new System.Drawing.Size(10, 10);
			this.splitPanel.ResumeLayout(false);
			this.topPanel.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		protected override void WndProc(ref Message m) {
			if(m.Msg != DevExpress.XtraPrinting.Native.Win32.WM_CONTEXTMENU)
				base.WndProc(ref m);
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			if(hRuler != null)
				hRuler.Invalidate();
			if(vRuler != null)
				vRuler.Invalidate();
		}
		public void SetScrollOffset(Point offset) {
			scrollPanel.SetOffsetWithoutUpdate(offset.X, offset.Y);
		}
		public void BeginUpdate() {
			scrollPanel.SuppressRedraw();
		}
		public void EndUpdate() {
			scrollPanel.ResumeRedraw();
		}
		public void UpdateView() {
			ApplyLookAndFeel();
			framePanel.Invalidate();
			scrollPanel.Invalidate();
			cornerPanel.Invalidate();
			UpdateRulersSize();
		}
		public void OnLoadComplete() {
			framePanel = new FramePanel(designerHost, this);
			framePanel.Dock = DockStyle.Fill;
			framePanel.NotEnoughMemoryToPaint += framePanel_NotEnoughMemoryToPaint;
			scrollPanel.Controls.Add(framePanel);
			scrollPanel.MouseUp += HandleMouseUp;
			scrollPanel.Scroll += scrollPanel_Scroll;
			scrollPanel.Layout += scrollPanel_Layout;
			scrollPanel.MouseWheelZoom += scrollPanel_MouseWheelZoom;
			scrollPanel.MouseWheel += scrollPanel_MouseWheel;
			designerHost.AddService(typeof(AdornerService), new AdornerService(designerHost));
			designerHost.AddService(typeof(IBandViewInfoService), framePanel);
			cornerPanel.Initialize(this);
			ApplyLookAndFeel();
		}
		bool entered_framePanel_NotEnoughMemoryToPaint;
		void framePanel_NotEnoughMemoryToPaint(object sender, ExceptionEventArgs e) {
			if(!entered_framePanel_NotEnoughMemoryToPaint) {
				entered_framePanel_NotEnoughMemoryToPaint = true;
				zoomService.ZoomFactor = 1;
				NotificationService.ShowException<XtraReport>(LookAndFeel, designerHost.GetOwnerWindow(), new Exception(ReportStringId.Msg_NotEnoughMemoryToPaint.GetString(), e.Exception));
				entered_framePanel_NotEnoughMemoryToPaint = false;
			}
		}
		public void ApplyLookAndFeel() {
			scrollPanel.ApplyLookAndFeel(LookAndFeel);
			VRuler.UpdatePaintHelper(LookAndFeel);
			HRuler.UpdatePaintHelper(LookAndFeel);
		}
		public void AddSplitWindow(Control ctl) {
			if(!Contains(ctl)) {
				if(ctl is ISupportLookAndFeel)
					((ISupportLookAndFeel)ctl).LookAndFeel.ParentLookAndFeel = LookAndFeel;
				splitter.Visible = true;
				ctl.Dock = DockStyle.Bottom;
				Controls.Add(ctl);
			}
		}
		public void RemoveSplitWindow(Control ctl) {
			if(Contains(ctl)) {
				if(ctl is ISupportLookAndFeel)
					((ISupportLookAndFeel)ctl).LookAndFeel.ParentLookAndFeel = null;
				splitter.Visible = false;
				Controls.Remove(ctl);
			}
		}
		protected IDesigner GetDesigner(IComponent component) {
			return (component != null) ? designerHost.GetDesigner(component) : null;
		}
		public void UpdateProperties() {
			SetRulersUnit();
		}
		void SetRulersUnit() {
			hRuler.SetUnit(Report.ReportUnit);
			vRuler.SetUnit(Report.ReportUnit);
		}
		void SetReportHMargins(int leftMargin, int rightMargin) {
			Margins m = (Margins)Report.Margins.Clone();
			m.Left = Math.Max(0, zoomService.FromScaledPixels(leftMargin, Report.Dpi));
			m.Right = Math.Max(0, zoomService.FromScaledPixels(rightMargin, Report.Dpi));
			IComponentChangeService changeSvc = (IComponentChangeService)designerHost.GetService(typeof(IComponentChangeService));
			string desc = String.Format(DesignSR.Trans_ChangeProp, XRComponentPropertyNames.Margins_Right);
			DesignerTransaction trans = designerHost.CreateTransaction(desc);
			try {
				XRControlDesignerBase.RaiseComponentChanging(changeSvc, Report, XRComponentPropertyNames.Margins);
				Report.Margins = new Margins(m.Left, m.Right,
					Report.Margins.Top, Report.Margins.Bottom);
				XRControlDesignerBase.RaiseComponentChanged(changeSvc, Report,
					XRAccessor.GetPropertyDescriptor(Report, XRComponentPropertyNames.Margins), m, Report.Margins);
				trans.Commit();
			} catch {
				trans.Cancel();
			}
		}
		void HandleMouseUp(object sender, MouseEventArgs e) {
			SelectComponent(Report);
			if(e.Button.IsRight())
				ShowContextMenu();
		}
		void ShowContextMenu() {
			ReportDesigner designer = GetReportDesigner();
			if(designer != null)
				designer.OnContextMenu(MousePosition.X, MousePosition.Y);
		}
		ReportDesigner GetReportDesigner() {
			if(designerHost == null || designerHost.RootComponent == null)
				return null;
			return (ReportDesigner)designerHost.GetDesigner(designerHost.RootComponent);
		}
		protected internal void SelectComponent(IComponent component) {
			ISelectionService svc = designerHost.GetService(typeof(ISelectionService)) as ISelectionService;
			svc.SetSelectedComponents(new object[] { component }, SelectionTypes.Replace);
		}
		void vRuler_BeginDrag(object sender, EventArgs e) {
			StartBandHeightChanging(vRuler.Sections.Count - 1);
		}
		void vRuler_SliderClick(object sender, RulerSectionEventArgs e) {
			StartBandHeightChanging(e.RulerSection.Index);
		}
		void LookAndFeel_StyleChanged(object sender, EventArgs e) {
			UpdateLookAndFeel();
		}
		void UpdateLookAndFeel() {
			this.BackColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);			
			VRuler.UpdatePaintHelper(LookAndFeel);
			HRuler.UpdatePaintHelper(LookAndFeel);
			splitter.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			ReportPaintStyle paintStyle = ReportPaintStyles.GetPaintStyle(LookAndFeel);
			splitter.BackColor = paintStyle.GetReportBackgroundColor(LookAndFeel);
			scrollPanel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
			splitPanel.LookAndFeel.ParentLookAndFeel = LookAndFeel;
		}
		internal void StartBandHeightChanging(int index) {
			rulerSectionIndex = index;
			try {
				BandViewInfo viewInfo = framePanel.ViewInfos[index];
				if(!LockService.GetInstance(designerHost).CanChangeComponent(viewInfo.Band))
					return;
				if(viewInfo == null || !viewInfo.Expanded)
					return;
				float dy = vRuler.GetMarkingBounds(index).Top + viewInfo.BandBoundsF.Height;
				if(dy > 0)
					dy -= vRuler.PointToClient(MousePosition).Y;
				Cursor.Current = Cursors.HSplit;
				splitService.StartHMoving(framePanel, new PointF(0, dy));
				splitService.SplitterMoved += OnBandHeightChanged;
				splitService.SplitterMove += OnBandHeightChange;
				splitService.MoveCanceled += OnBandHeightChangeCanceled;
			} catch { ; }
		}
		void OnComponentTraySplitterMoved(object sender, SplitterEventArgs e) {
			UpdateVRuler();
		}
		void OnBandHeightChange(object sender, SplitEventArgs e) {
			int sectionIndex = ValidateSectionIndex(rulerSectionIndex, (SplitService)sender, e);
			if(sectionIndex != rulerSectionIndex) {
				vRuler.SetSectionHeight(vRuler.Sections[rulerSectionIndex], framePanel.ViewInfos[rulerSectionIndex].BandBounds.Height);
				rulerSectionIndex = sectionIndex;
			}
			Rectangle r = vRuler.GetMarkingBounds(rulerSectionIndex);
			Point pt = vRuler.PointToScreen(r.Location);
			int height = Math.Max(0, e.RoundedSplitY - pt.Y);
			vRuler.SetSectionHeight(vRuler.Sections[rulerSectionIndex], height);
			vRuler.Invalidate();
		}
		int ValidateSectionIndex(int sectionIndex, SplitService service, SplitEventArgs e) {
			Band bottomMarginBand = Report.Bands[BandKind.BottomMargin];
			if(bottomMarginBand == null || zoomService.ToScaledPixels(bottomMarginBand.Height, bottomMarginBand.Dpi) >= 1)
				return sectionIndex;
			BandDesigner bandDesigner = this.designerHost.GetDesigner(bottomMarginBand) as BandDesigner;
			if(bandDesigner == null || bandDesigner.Locked)
				return sectionIndex;
			BandViewInfo viewInfo = framePanel.GetViewInfoByBand(bottomMarginBand);
			int viewInfoIndex = Array.IndexOf(framePanel.ViewInfos, viewInfo);
			if(sectionIndex == viewInfoIndex || sectionIndex == viewInfoIndex - 1) {
				float dy = e.Y - service.StartPos.Y;
				return dy >= 0 ? viewInfoIndex : viewInfoIndex - 1;
			}
			return sectionIndex;
		}
		void OnBandHeightChanged(object sender, SplitEventArgs e) {
			System.Diagnostics.Debug.Assert(rulerSectionIndex >= 0);
			BandViewInfo viewInfo = framePanel.ViewInfos[rulerSectionIndex];
			System.Diagnostics.Debug.Assert(viewInfo != null);
			BandDesigner bandDesigner = designerHost.GetDesigner(viewInfo.Band) as BandDesigner;
			System.Diagnostics.Debug.Assert(bandDesigner != null);
			RectangleF r = framePanel.RectangleFToScreen(viewInfo.BandBoundsF);
			float height = Math.Max(zoomService.FromScaledPixels(e.SplitY - r.Top, viewInfo.Band.Dpi), 0);
			bandDesigner.SetBandHeight(height);
			rulerSectionIndex = -1;
		}
		void OnBandHeightChangeCanceled(object sender, EventArgs e) {
			UpdateVRuler();
		}
		int GetPageWidth(SplitEventArgs e) {
			return (e.RoundedSplitX - framePanel.RectangleToScreen(framePanel.BandBounds).Left);
		}
		void scrollPanel_Layout(object sender, LayoutEventArgs e) {
			OnViewPositionChanged();
		}
		void scrollPanel_Scroll(object sender, XtraScrollEventArgs e) {
			OnViewPositionChanged();
		}
		internal void OnViewPositionChanged() {
			hRuler.ViewOffset = new Point(framePanel.Left, 0);
			vRuler.ViewOffset = new Point(0, framePanel.Top);
			framePanel.UpdateViewVisibleBoundsCenter();
		}
		void scrollPanel_MouseWheelZoom(object sender, MouseWheelZoomEventArgs e) {
			framePanel.ZoomOffset = e.Offset;
			if(e.Delta > 0) zoomService.ScrollZoomIn();
			else if(e.Delta < 0) zoomService.ScrollZoomOut();
		}
		void scrollPanel_MouseWheel(object sender, MouseEventArgs e) {
			OnViewPositionChanged();
		}
		internal void UpdateRulers() {
			SetRulersUnit();
			UpdateVRuler();
			UpdateHRuler();
		}
		void UpdateVRuler() {
			try {
				vRuler.Offset = ReportPaintStyles.GetFullPageIndent(LookAndFeel).Height + VRulerOffset;
				vRuler.RecreateSections(RulerService.CreateSections(framePanel.ViewInfos));
			} catch(Exception ex) {
				if(ExceptionHelper.IsCriticalException(ex))
					throw;
			}
		}
		void UpdateHRuler() {
			hRuler.Offset = ReportPaintStyles.GetFullPageIndent(LookAndFeel).Width + HRulerOffset;
			ReportDesigner designer = GetReportDesigner();
			if(designer != null) {
				hRuler.SetClientLength(designer.PageWidth);
				hRuler.LeftMargin = designer.LeftMargin;
				hRuler.RightMargin = designer.RightMargin;
			}
		}
		void OnZoomChanged(object sender, EventArgs e) {
			OnViewPositionChanged();
		}
		void Rulers_SelectionChanged(object sender, BoundsEventArgs e) {
			if(Report.Bands.Count == 0)
				return;
			IMouseTarget mouseTarget = MouseTargetService.GetMouseTarget(designerHost, Report.Bands[0]);
			if(mouseTarget == null)
				return;
			RectangleF bounds = e.Bounds[0];
			if(sender is HRuler) {
				bounds.Y = Int16.MinValue;
				bounds.Height = 2 * Int16.MaxValue;
			} else {
				bounds.X = Int16.MinValue;
				bounds.Width = 2 * Int16.MaxValue;
			}
			mouseTarget.CommitSelection(bounds, Report);
		}
		void UpdateRulersSize() {
			int vRulerWidth = VRuler.RulerSize.Width;
			int hRulerHeight = HRuler.RulerSize.Height;
			int cornerPanelWidth = vRulerWidth;
			if(vRulerWidth == vRulerPanel.Width && hRulerHeight == topPanel.Height && cornerPanel.Width == cornerPanelWidth)
				return;
			SuspendLayout();
			cornerPanel.Width = cornerPanelWidth;
			topPanel.Height = hRulerHeight;
			vRulerPanel.Width = vRulerWidth;
			ResumeLayout(false);
		}
		#region width changing
		ChangeWidthInfo changeWidthInfo = new ChangeWidthInfo();
		void hRuler_BeginDrag(object sender, EventArgs e) {
			changeWidthInfo.Init();
			StartWidthChanging();
		}
		void StartWidthChanging() {
			if(framePanel == null || designerHost.IsDebugging())
				return;
			splitService.StartVMoving(framePanel);
			splitService.SplitterMove += OnWidthChange;
			splitService.SplitterMoved += OnWidthChanged;
			splitService.MoveCanceled += OnWidthChangeCanceled;
		}
		void OnWidthChange(object sender, SplitEventArgs e) {
			RulerState state = changeWidthInfo.GetHRulerState(hRuler, e.X);
			if (state == RulerState.LeftMarginChanged || state == RulerState.RightMarginChanged) {
				if (changeWidthInfo.RacesOnTheRightSide) {
					hRuler.State = RulerState.None;
					hRuler.SetClientLength(framePanel.RectangleToScreen(framePanel.BandBounds).Width);
				}				
				hRuler.State = state;
				hRuler.ResizeSelectedMargin(e.RoundedSplitX, framePanel.RectangleToScreen(framePanel.BandBounds));
			}
			else {
				if (changeWidthInfo.RacesOnTheRightSide) {
					hRuler.State = RulerState.RightMarginChanged;
					hRuler.ResizeSelectedMargin(e.RoundedSplitX, framePanel.RectangleToScreen(framePanel.BandBounds));
				}
				hRuler.State = state;
				hRuler.SetClientLength(GetPageWidth(e));
			}
			hRuler.Invalidate();
		}
		void OnWidthChanged(object sender, SplitEventArgs e) {
			hRuler.State = changeWidthInfo.GetHRulerState(hRuler, e.X);			
			if (hRuler.State == RulerState.LeftMarginChanged || hRuler.State == RulerState.RightMarginChanged) {
				SetReportHMargins(hRuler.LeftMargin, hRuler.RightMargin);
				hRuler.State = RulerState.None;
			} else {
				int width = zoomService.FromScaledPixels(GetPageWidth(e), Report.Dpi);
				string propName = Report.Landscape ? XRComponentPropertyNames.PageHeight : XRComponentPropertyNames.PageWidth;
				try {
					XRAccessor.ChangeProperty(Report, propName, width);
				} catch(Exception ex) {
					if(ExceptionHelper.IsCriticalException(ex))
						throw;
				}
				scrollPanel.Invalidate();
			}
		}
		void OnWidthChangeCanceled(object sender, EventArgs e) {
			UpdateHRuler();
			if (hRuler.State == RulerState.LeftMarginChanged || hRuler.State == RulerState.RightMarginChanged)
				hRuler.State = RulerState.None;
		}
		internal void StartWidthChanging(BandViewInfo viewInfo, Point screenPoint) {
			changeWidthInfo.Init(this, viewInfo, screenPoint);
			if (changeWidthInfo.RightMarginChanging || changeWidthInfo.RightSideChanging || changeWidthInfo.LeftMarginChanging)
				StartWidthChanging();
		}
		#endregion
	}
	public class CornerPanelViewInfo : ObjectInfoArgs {
		bool hotTracked;
		Rectangle tagBounds = Rectangle.Empty;
		public bool HotTracked {
			get { return hotTracked; }
			set { hotTracked = value; }
		}
		public Rectangle TagBounds {
			get { return tagBounds; }
			set { tagBounds = value; }
		}
	}
}
