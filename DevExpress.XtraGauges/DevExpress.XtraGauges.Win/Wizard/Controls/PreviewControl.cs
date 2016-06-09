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
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Customization;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Resources;
using DevExpress.XtraGauges.Win.Base;
namespace DevExpress.XtraGauges.Win.Wizard {
	[System.ComponentModel.ToolboxItem(false)]
	public class DesignerPreviewControl : Control {
		GaugeDesignerControl designerControlCore;
		PrimitivePreviewControlViewInfo viewInfoCore;
		Brush TextBrush;
		Brush BackgroundBrush;
		Brush CanvasBrush;
		Font TextFont;
		StringFormat TextFormat;
		BorderPainter BorderPainter;
		bool showBackgroundElementsCore = false;
		bool showForegroundElementsCore = true;
		ZoomTrackBarControl zoomCore;
		CheckButton showForegroundCore;
		CheckButton showBackgroundCore;
		SimpleButton removeButtonCore;
		SimpleButton duplicateButtonCore;
		float scaleFactorCore;
		ComboBoxEdit itemsComboBoxCore;
		ColorEdit backColorComboBoxCore;
		BaseElement<IRenderableElement>[] primitivesCore;
		internal int BackColorTextWidth = 65;
		internal int SelectedElementTextWidth = 100;
		public DesignerPreviewControl() {
			this.SetStyle(
				ControlStyles.OptimizedDoubleBuffer |
				ControlStyles.AllPaintingInWmPaint |
				ControlStyles.UserMouse |
				ControlStyles.UserPaint |
				ControlStyles.ResizeRedraw,
				true);
			OnCreate();
			LayoutChanged();
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				OnDispose();
			}
			base.Dispose(disposing);
		}
		protected virtual void OnCreate() {
			this.TextFont = new Font("Tahoma", 8.25f);
			this.scaleFactorCore = 1f;
			this.viewInfoCore = CreateViewInfo();
			UpdateStyle();
			this.TextFormat = new StringFormat();
			TextFormat.Alignment = StringAlignment.Center;
			TextFormat.LineAlignment = StringAlignment.Center;
			this.primitivesCore = new BaseElement<IRenderableElement>[0];
			this.itemsComboBoxCore = new ComboBoxEdit();
			this.backColorComboBoxCore = new ColorEdit();
			this.zoomCore = new ZoomTrackBarControl();
			this.showForegroundCore = new CheckButton();
			this.showBackgroundCore = new CheckButton();
			this.removeButtonCore = new SimpleButton();
			this.duplicateButtonCore = new SimpleButton();
			RemoveButton.Parent = this;
			RemoveButton.Text = "Remove";
			RemoveButton.Click += OnRemoveItem;
			DuplicateButton.Parent = this;
			DuplicateButton.Text = "Clone";
			DuplicateButton.Click += OnCloneItem;
			ItemsCombo.Properties.AllowNullInput = DefaultBoolean.False;
			ItemsCombo.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			ItemsCombo.Parent = this;
			ItemsCombo.EditValueChanging += OnComboItemChanging;
			ItemsCombo.EditValueChanged += OnComboItemChanged;
			BackColorEdit.Properties.AllowNullInput = DefaultBoolean.False;
			BackColorEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			BackColorEdit.Parent = this;
			BackColorEdit.EditValueChanged += OnBackColorComboItemChanged;
			BackColorEdit.EditValue = Color.Gainsboro;
			ShowBackground.Parent = this;
			ShowBackground.Image = UIHelper.UIButtonImages[0];
			ShowBackground.CheckedChanged += OnShowBackgroundChanged;
			ShowBackground.ImageLocation = ImageLocation.MiddleCenter;
			ShowBackground.Checked = ShowBackgroundElements;
			ShowForeground.Parent = this;
			ShowForeground.Image = UIHelper.UIButtonImages[1];
			ShowForeground.CheckedChanged += OnShowForegroundChanged;
			ShowForeground.ImageLocation = ImageLocation.MiddleCenter;
			ShowForeground.Checked = ShowForegroundElements;
			Zoom.Parent = this;
			Zoom.BorderStyle = BorderStyles.NoBorder;
			Zoom.Properties.Maximum = 190;
			Zoom.Properties.Minimum = 10;
			Zoom.Value = 100;
			Zoom.Properties.SmallChange = 10;
			Zoom.Properties.LargeChange = 10;
			Zoom.EditValueChanged += OnZoomChanged;
			this.CanvasBrush = new SolidBrush(Color.Gainsboro);
		}
		void OnShowForegroundChanged(object sender, EventArgs e) {
			ShowForegroundElements = ShowForeground.Checked;
		}
		void OnShowBackgroundChanged(object sender, EventArgs e) {
			ShowBackgroundElements = ShowBackground.Checked;
		}
		protected virtual void OnDispose() {
			if(SelectedPrimitive != null) {
				SelectedPrimitive.Changed -= OnPrimitiveChanged;
			}
			if(ItemsCombo != null) {
				ItemsCombo.EditValueChanging -= OnComboItemChanging;
				ItemsCombo.EditValueChanged -= OnComboItemChanged;
				ItemsCombo.Properties.Items.Clear();
				ItemsCombo.Parent = null;
				ItemsCombo.Dispose();
				itemsComboBoxCore = null;
			}
			if(BackColorEdit != null) {
				BackColorEdit.EditValueChanged -= OnComboItemChanged;
				BackColorEdit.Parent = null;
				BackColorEdit.Dispose();
				backColorComboBoxCore = null;
			}
			if(ShowBackground != null) {
				ShowBackground.CheckedChanged -= OnShowBackgroundChanged;
				ShowBackground.Parent = null;
				ShowBackground.Dispose();
				showBackgroundCore = null;
			}
			if(ShowForeground != null) {
				ShowForeground.CheckedChanged -= OnShowForegroundChanged;
				ShowForeground.Parent = null;
				ShowForeground.Dispose();
				showForegroundCore = null;
			}
			if(Zoom != null) {
				Zoom.Click -= OnZoomChanged;
				Zoom.Parent = null;
				Zoom.Dispose();
				zoomCore = null;
			}
			if(RemoveButton != null) {
				RemoveButton.Click -= OnRemoveItem;
				RemoveButton.Parent = null;
				RemoveButton.Dispose();
				removeButtonCore = null;
			}
			if(DuplicateButton != null) {
				DuplicateButton.Click -= OnCloneItem;
				DuplicateButton.Parent = null;
				DuplicateButton.Dispose();
				duplicateButtonCore = null;
			}
			if(ViewInfo != null) {
				ViewInfo.Dispose();
				viewInfoCore = null;
			}
			primitivesCore = null;
			designerControlCore = null;
		}
		protected PrimitivePreviewControlViewInfo CreateViewInfo() {
			return new PrimitivePreviewControlViewInfo(this);
		}
		protected virtual void UpdateStyle() {
			Color textColor = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.ControlText);
			Color bgColor2 = LookAndFeelHelper.GetSystemColor(LookAndFeel, SystemColors.Control);
			this.TextBrush = new SolidBrush(textColor);
			this.BackgroundBrush = new SolidBrush(bgColor2);
			this.BorderPainter = BorderHelper.GetPainter(DevExpress.XtraEditors.Controls.BorderStyles.Default, LookAndFeel);
		}
		internal void SetDesignerControl(GaugeDesignerControl designer) {
			this.designerControlCore = designer;
			UpdateStyle();
		}
		protected GaugeDesignerControl DesignerControl {
			get { return designerControlCore; }
		}
		protected PrimitivePreviewControlViewInfo ViewInfo {
			get { return viewInfoCore; }
		}
		public BaseElement<IRenderableElement> SelectedPrimitive {
			get { return ItemsCombo.SelectedIndex != -1 ? Primitives[ItemsCombo.SelectedIndex] : null; }
		}
		public BaseElement<IRenderableElement>[] Primitives {
			get { return primitivesCore; }
			set {
				if(Primitives == value) return;
				SetPrimitivesCore(value);
			}
		}
		protected CheckButton ShowBackground {
			get { return showForegroundCore; }
		}
		protected CheckButton ShowForeground {
			get { return showBackgroundCore; }
		}
		protected ZoomTrackBarControl Zoom {
			get { return zoomCore; }
		}
		public int ZoomValue {
			get { return zoomCore.Value; }
			set { zoomCore.Value = value; }
		}
		protected ComboBoxEdit ItemsCombo {
			get { return itemsComboBoxCore; }
		}
		protected SimpleButton RemoveButton {
			get { return removeButtonCore; }
		}
		protected SimpleButton DuplicateButton {
			get { return duplicateButtonCore; }
		}
		protected ColorEdit BackColorEdit {
			get { return backColorComboBoxCore; }
		}
		public float ScaleFactor {
			get { return scaleFactorCore; }
		}
		public void IncScaleFactor() {
			scaleFactorCore = (float)Math.Min(2.0f, scaleFactorCore + 0.1);
		}
		public void DecScaleFactor() {
			scaleFactorCore = (float)Math.Max(0.1f, scaleFactorCore - 0.1);
		}
		protected UserLookAndFeel LookAndFeel {
			get { return DesignerControl == null ? UserLookAndFeel.Default : DesignerControl.LookAndFeel; }
		}
		protected void SetPrimitivesCore(BaseElement<IRenderableElement>[] value) {
			this.primitivesCore = value;
			string[] names = new string[Primitives.Length];
			for(int i = 0; i < Primitives.Length; i++) {
				names[i] = Primitives[i].Name;
			}
			ItemsCombo.Properties.Items.Clear();
			ItemsCombo.Properties.Items.AddRange(names);
			ItemsCombo.SelectedIndex = 0;
		}
		void OnComboItemChanging(object sender, ChangingEventArgs e) {
			BaseElement<IRenderableElement> pPrev = FindPrimitiveByName(e.OldValue as string);
			BaseElement<IRenderableElement> pNew = FindPrimitiveByName(e.NewValue as string);
			if(pPrev != null) pPrev.Changed -= OnPrimitiveChanged;
			if(pNew != null) {
				pNew.Self.SetViewInfoDirty();
				pNew.Self.CalcViewInfo(null);
				pNew.Changed += OnPrimitiveChanged;
			}
			Invalidate();
		}
		void UpdateRemoveButton(BaseElement<IRenderableElement> designerElement) {
			if(DesignerControl != null) {
				BaseElement<IRenderableElement> elementToRemove = DesignerControl.SelectedPage.GetElementByDesignedClone(designerElement);
				RemoveButton.Enabled = DesignerControl.Gauge.CanRemoveGaugeElement(elementToRemove);
			}
		}
		void OnCloneItem(object sender, EventArgs e) {
			if(DesignerControl == null) return;
			BaseElement<IRenderableElement> duplicate = DesignerControl.DuplicateDesignedElement(SelectedPrimitive);
			if(duplicate != null) {
				BaseElement<IRenderableElement> designedElement = DesignerControl.SelectedPage.GetDesignedCloneByElement(duplicate);
				ItemsCombo.SelectedIndex = ItemsCombo.Properties.Items.IndexOf(designedElement.Name);
			}
		}
		void OnRemoveItem(object sender, EventArgs e) {
			if(DesignerControl == null) return;
			SelectedPrimitive.Changed -= OnPrimitiveChanged;
			DesignerControl.RemoveDesignedElement(SelectedPrimitive);
			if(ItemsCombo.Properties.Items.Count > 0)
				ItemsCombo.SelectedIndex = (ItemsCombo.Properties.Items.Count - 1);
		}
		void OnComboItemChanged(object sender, EventArgs e) {
			UpdateRemoveButton(SelectedPrimitive);
			if(SelectedItemChanged != null) {
				SelectedItemChanged(new PrimitiveChangedEventArgs(SelectedPrimitive));
			}
		}
		void OnBackColorComboItemChanged(object sender, EventArgs e) {
			if(CanvasBrush != null) {
				CanvasBrush.Dispose();
			}
			CanvasBrush = new SolidBrush((Color)BackColorEdit.EditValue);
			Invalidate(ViewInfo.Rects.Canvas);
		}
		public Color PageBackColor {
			get { return (Color)BackColorEdit.EditValue; }
			set { BackColorEdit.EditValue = value; }
		}
		BaseElement<IRenderableElement> FindPrimitiveByName(string name) {
			if(!string.IsNullOrEmpty(name)) {
				foreach(BaseElement<IRenderableElement> p in Primitives) {
					if(p.Name == name) return p;
				}
			}
			return null;
		}
		public event SelectedPrimitiveItemChangedHandler SelectedItemChanged;
		void OnPrimitiveChanged(object sender, EventArgs e) {
			SelectedPrimitive.Accept(
				delegate(IElement<IRenderableElement> element) {
					element.Self.ResetCache();
					element.Self.SetViewInfoDirty();
				}
			);
			Invalidate();
		}
		protected override void OnResize(EventArgs e) {
			base.OnResize(e);
			LayoutChanged();
		}
		public void LayoutChanged() {
			ViewInfo.SetDirty();
			UpdateTextWidths(Graphics.FromHwnd(Handle));
			ViewInfo.CalcInfo(null, Bounds);
			CalcControlsLocation();
			UpdateRemoveButton(SelectedPrimitive);
		}
		protected void CalcControlsLocation() {
			if(ItemsCombo.Bounds != ViewInfo.Rects.ItemsCombo) ItemsCombo.Bounds = ViewInfo.Rects.ItemsCombo;
			if(BackColorEdit.Bounds != ViewInfo.Rects.BackColorEdit) BackColorEdit.Bounds = ViewInfo.Rects.BackColorEdit;
			if(ShowBackground.Bounds != ViewInfo.Rects.ShowBackgroundElementsButton) ShowBackground.Bounds = ViewInfo.Rects.ShowBackgroundElementsButton;
			if(ShowForeground.Bounds != ViewInfo.Rects.ShowForegroundElementsButton) ShowForeground.Bounds = ViewInfo.Rects.ShowForegroundElementsButton;
			if(Zoom.Bounds != ViewInfo.Rects.Zoom) Zoom.Bounds = ViewInfo.Rects.Zoom;
			if(RemoveButton.Bounds != ViewInfo.Rects.ItemsRemoveButton) RemoveButton.Bounds = ViewInfo.Rects.ItemsRemoveButton;
			if(DuplicateButton.Bounds != ViewInfo.Rects.ItemsDuplicateButton) DuplicateButton.Bounds = ViewInfo.Rects.ItemsDuplicateButton;
		}
		protected override void OnPaint(PaintEventArgs e) {
			base.OnPaint(e);
			using(GraphicsCache cache = new GraphicsCache(e)) {
				GraphicsInfoArgs ea = new GraphicsInfoArgs(cache, e.ClipRectangle);
				UpdateBeforePaint(ea);
				DrawBackground(ea);
				ea.Graphics.SetClip(ViewInfo.Rects.Canvas);
				DrawCanvas(ea);
				ea.Graphics.ResetClip();
			}
		}
		public bool ShowBackgroundElements {
			get { return showBackgroundElementsCore; }
			set {
				if(ShowBackgroundElements == value) return;
				showBackgroundElementsCore = value;
				Invalidate();
			}
		}
		public bool ShowForegroundElements {
			get { return showForegroundElementsCore; }
			set {
				if(ShowForegroundElements == value) return;
				showForegroundElementsCore = value;
				Invalidate();
			}
		}
		const string bcText = "Back Color:";
		const string seText = "Selected Element:";
		protected void UpdateBeforePaint(GraphicsInfoArgs e) {
			if(!ViewInfo.IsReady) {
				UpdateTextWidths(e.Graphics);
				ViewInfo.CalcInfo(e.Graphics, Bounds);
			}
			if(SelectedPrimitive != null && !SelectedPrimitive.Self.ViewInfo.IsReady) {
				SelectedPrimitive.Self.CalcViewInfo(SelectedPrimitive.Self.Transform);
			}
		}
		void UpdateTextWidths(Graphics g) {
			if(g != null && !ViewInfo.IsReady) {
				BackColorTextWidth = (int)Math.Ceiling(g.MeasureString(bcText, TextFont, 150, TextFormat).Width);
				SelectedElementTextWidth = (int)Math.Ceiling(g.MeasureString(seText, TextFont, 200, TextFormat).Width);
			}
		}
		protected void DrawBackground(GraphicsInfoArgs e) {
			e.Graphics.FillRectangle(BackgroundBrush, ViewInfo.Rects.Bounds);
			e.Graphics.FillRectangle(CanvasBrush, ViewInfo.Rects.Canvas);
			if(ViewInfo.Rects.ComboText.Width > 0) e.Graphics.DrawString(seText, TextFont, TextBrush, ViewInfo.Rects.ComboText, TextFormat);
			e.Graphics.DrawString(bcText, TextFont, TextBrush, ViewInfo.Rects.BackColorText, TextFormat);
		}
		protected void DrawCanvas(GraphicsInfoArgs e) {
			IRenderableElement element = SelectedPrimitive as IRenderableElement;
			var canvas = ViewInfo.Rects.Canvas;
			if(element == null || canvas.Width == 0 || canvas.Height == 0) return;
			using(SafeGraphicsContext safeGraphics = new SafeGraphicsContext(e.Graphics)) {
				safeGraphics.Context.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
				safeGraphics.Context.Transform = CalcPreviewMatrix(element, (RectangleF)canvas, ScaleFactor);
				if(ShowBackgroundElements)
					DrawGaugePreview(safeGraphics.Context);
				element.LockCaching();
				element.Render(safeGraphics.Context);
				element.UnlockCaching();
				if(ShowForegroundElements)
					DrawForegroundElements(safeGraphics.Context);
			}
			DrawCanvasBorder(e);
		}
		static Matrix CalcPreviewMatrix(IRenderableElement element, RectangleF canvas, float scaleFactor) {
			float canvasSize = (float)Math.Min(canvas.Width, canvas.Height);
			element.WaitForPendingDelayedCalculation();
			RectangleF bounds = MathHelper.CalcRelativeBoundBox(element.ViewInfo.BoundBox, element.Transform);
			bounds.Inflate(bounds.Width * 0.05f, bounds.Height * 0.05f);
			float scalingFactor;
			if((float)Math.Max(bounds.Width, bounds.Height) != 0)
				scalingFactor = scaleFactor * canvasSize / (float)Math.Max(bounds.Width, bounds.Height);
			else
				scalingFactor = scaleFactor * canvasSize;
			Matrix result = new Matrix(scalingFactor, 0, 0, scalingFactor,
				(canvas.Left - bounds.Left * scalingFactor) + (canvas.Width - bounds.Width * scalingFactor) / 2f,
				(canvas.Top - bounds.Top * scalingFactor) + (canvas.Height - bounds.Height * scalingFactor) / 2f);
			if(!element.ViewInfo.IsReady) {
				element.CalcViewInfo(result);
				bounds = MathHelper.CalcRelativeBoundBox(element.ViewInfo.BoundBox, element.Transform);
				bounds.Inflate(bounds.Width * 0.05f, bounds.Height * 0.05f);
				if((float)Math.Max(bounds.Width, bounds.Height) != 0)
					scalingFactor = scaleFactor * canvasSize / (float)Math.Max(bounds.Width, bounds.Height);
				else
					scalingFactor = scaleFactor * canvasSize;
				result = new Matrix(scalingFactor, 0, 0, scalingFactor,
					(canvas.Left - bounds.Left * scalingFactor) + (canvas.Width - bounds.Width * scalingFactor) / 2f,
					(canvas.Top - bounds.Top * scalingFactor) + (canvas.Height - bounds.Height * scalingFactor) / 2f);
			}
			return result;
		}
		void DrawForegroundElements(IRenderingContext context) {
			ISupportVisualDesigning vd = SelectedPrimitive as ISupportVisualDesigning;
			if(vd != null) vd.RenderDesignerElements(context.Graphics);
		}
		void DrawGaugePreview(IRenderingContext context) {
			if(DesignerControl == null || SelectedPrimitive is IScaleBackgroundLayer || SelectedPrimitive is IDigitalBackgroundLayer) return;
			DesignerControl.Gauge.Model.Accept(
					delegate(IElement<IRenderableElement> element) {
						if(element is CustomizationFrameBase || element.IsComposite || element == SelectedPrimitive) return;
						element.Self.LockCaching();
						element.Self.Render(context);
						element.Self.UnlockCaching();
					}
				);
		}
		void DrawCanvasBorder(GraphicsInfoArgs e) {
			BorderObjectInfoArgs info = new BorderObjectInfoArgs(e.Cache, AppearanceObject.ControlAppearance, ViewInfo.Rects.Canvas, ObjectState.Normal);
			BorderPainter.DrawObject(info);
		}
		void OnZoomChanged(object sender, EventArgs e) {
			scaleFactorCore = (float)Zoom.Value * 0.01f;
			Invalidate(ViewInfo.Rects.Canvas);
		}
	}
	public class PrimitivePreviewControlViewInfo : BaseViewInfo {
		DesignerPreviewControl previewControlCore;
		PreviewControlViewRects viewRectsCore;
		public PrimitivePreviewControlViewInfo(DesignerPreviewControl owner) {
			this.previewControlCore = owner;
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.viewRectsCore = new PreviewControlViewRects();
		}
		protected override void OnDispose() {
			this.viewRectsCore = null;
			this.previewControlCore = null;
			base.OnDispose();
		}
		protected DesignerPreviewControl Owner {
			get { return previewControlCore; }
		}
		public PreviewControlViewRects Rects {
			get { return viewRectsCore; }
		}
		protected override void CalcViewRects(Rectangle bounds) {
			Rects.Clear();
			Rects.Bounds = Rects.Canvas = bounds;
			int left = 10;
			int headerHeight = 37;
			int footerHeight = 37;
			int buttonSize = 23;
			if(bounds.Height > headerHeight + footerHeight) {
				Rectangle header = new Rectangle(bounds.Left + left, bounds.Top, bounds.Width - left * 2, headerHeight);
				Rectangle canvas = new Rectangle(bounds.Left + left, header.Bottom, bounds.Width - left * 2, bounds.Height - headerHeight - footerHeight);
				Rectangle footer = new Rectangle(bounds.Left + left, canvas.Bottom, bounds.Width - left * 2, footerHeight);
				int p = (headerHeight - buttonSize) / 2;
				Rectangle comboText = new Rectangle(header.Left, header.Top + p, header.Width > 250 ? Owner.SelectedElementTextWidth : 0, buttonSize);
				Rectangle removeBtn = new Rectangle(header.Right - 55, header.Top + p, 55, buttonSize);
				Rectangle duplicateBtn = new Rectangle(header.Right - 2 * 55 - 2, header.Top + p, 55, buttonSize);
				int w = Math.Min(250, duplicateBtn.Left - 2 - (comboText.Right + 2));
				Rectangle combo = new Rectangle(comboText.Right + 2, header.Top + p + 1, w, buttonSize);
				p = (footerHeight - buttonSize) / 2;
				Rectangle btnBg = new Rectangle(footer.Left, footer.Top + p, buttonSize, buttonSize);
				Rectangle btnFg = new Rectangle(btnBg.Right + 3, footer.Top + p, buttonSize, buttonSize);
				Rectangle colorText = new Rectangle(btnFg.Right + 5, footer.Top + p, Owner.BackColorTextWidth, buttonSize);
				Rectangle color = new Rectangle(colorText.Right + 1, footer.Top + p + 1, 52, buttonSize);
				w = Math.Min(150, footer.Right - (color.Right + 10));
				Rectangle zoom = new Rectangle(footer.Right - w, footer.Top + p + 2, w, buttonSize - 2);
				Rects.Canvas = canvas;
				Rects.Header = header;
				Rects.Footer = footer;
				Rects.ItemsCombo = combo;
				Rects.ItemsRemoveButton = removeBtn;
				Rects.ItemsDuplicateButton = duplicateBtn;
				Rects.ComboText = comboText;
				Rects.Zoom = zoom;
				Rects.BackColorEdit = color;
				Rects.BackColorText = colorText;
				Rects.ShowBackgroundElementsButton = btnBg;
				Rects.ShowForegroundElementsButton = btnFg;
			}
		}
		protected override void CalcViewStates() { }
	}
	public class PreviewControlViewRects {
		public Rectangle Bounds;
		public Rectangle Header;
		public Rectangle Footer;
		public Rectangle Canvas;
		public Rectangle Zoom;
		public Rectangle ItemsCombo;
		public Rectangle ItemsRemoveButton;
		public Rectangle ItemsDuplicateButton;
		public Rectangle ComboText;
		public Rectangle BackColorText;
		public Rectangle BackColorEdit;
		public Rectangle ShowBackgroundElementsButton;
		public Rectangle ShowForegroundElementsButton;
		public void Clear() {
			Bounds = Canvas = Header = Footer =
			Zoom = ItemsCombo = ComboText = ItemsRemoveButton = ItemsDuplicateButton =
			BackColorText = BackColorEdit =
			ShowBackgroundElementsButton = ShowForegroundElementsButton = Rectangle.Empty;
		}
	}
	public delegate void SelectedPrimitiveItemChangedHandler(PrimitiveChangedEventArgs ea);
	public class PrimitiveChangedEventArgs : EventArgs {
		BaseElement<IRenderableElement> primitiveCore;
		public PrimitiveChangedEventArgs(BaseElement<IRenderableElement> primitive) {
			this.primitiveCore = primitive;
		}
		public BaseElement<IRenderableElement> Primitive {
			get { return primitiveCore; }
		}
	}
}
