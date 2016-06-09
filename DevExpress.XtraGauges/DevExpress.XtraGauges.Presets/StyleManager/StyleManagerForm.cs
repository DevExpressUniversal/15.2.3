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
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGauges.Base;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Model;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.XtraGauges.Core.Styles;
using DevExpress.XtraLayout.Utils;
namespace DevExpress.XtraGauges.Presets.Styles {
	public partial class StyleManagerForm : XtraForm {
		IGaugeContainer gcControlStyle;
		IGaugeContainer gcGaugesStyle;
		IGaugeContainer gcGaugesLayout;
		Style controlStyle;
		Style[] gaugeStyles;
		StyleCollection[] gaugeStyleCollections;
		StyleManagerForm()
			: this(null) {
		}
		public StyleManagerForm(IGaugeContainer container) {
			Type winGaugeContainerType = ControlLoader.GetWinGaugeControlType();
			IsWinGaugeContainer = (container != null) &&
				(container.GetType() == winGaugeContainerType || winGaugeContainerType.IsAssignableFrom(container.GetType()));
			gaugeContainerCore = container;
			this.gcControlStyle = ControlLoader.CreateGaugeContainer();
			((Control)this.gcControlStyle).Location = new System.Drawing.Point(24, 48);
			((Control)this.gcControlStyle).Name = "gcControlStyle";
			((Control)this.gcControlStyle).Size = new System.Drawing.Size(300, 300);
			((Control)this.gcControlStyle).TabStop = false;
			gcGaugesStyle = ControlLoader.CreateGaugeContainer();
			((Control)this.gcGaugesStyle).Location = new System.Drawing.Point(24, 48);
			((Control)this.gcGaugesStyle).Name = "gcGaugesStyle";
			((Control)this.gcGaugesStyle).Size = new System.Drawing.Size(300, 300);
			((Control)this.gcGaugesStyle).TabStop = false;
			this.gcGaugesLayout = ControlLoader.CreateGaugeContainer();
			((Control)this.gcGaugesLayout).Location = new System.Drawing.Point(24, 48);
			((Control)this.gcGaugesLayout).Name = "gcGaugesLayout";
			((Control)this.gcGaugesLayout).Size = new System.Drawing.Size(300, 300);
			((Control)this.gcGaugesLayout).TabStop = false;
			InitializeComponent();
			this.itemGCControlStyle.Control = (Control)this.gcControlStyle;
			this.itemGCGaugesStyle.Control = (Control)this.gcGaugesStyle;
			this.itemGCGaugesLayout.Control = (Control)this.gcGaugesLayout;
			this.itemGCControlStyle.TextVisible = false;
			this.itemGCGaugesStyle.TextVisible = false;
			this.itemGCGaugesLayout.TextVisible = false;
			this.layout.Controls.Add((Control)this.gcControlStyle);
			this.layout.Controls.Add((Control)this.gcGaugesStyle);
			this.layout.Controls.Add((Control)this.gcGaugesLayout);
			if(container != null) {
				LoadLayout();
				InitControlStyle(container);
				InitGaugeStyles(container);
				InitSelectionTool();
			}
			layout.AllowCustomization = false;
			btnChangeStyle.Click += GaugeChangeStyleClick;
			if(container != null) {
				InitControlStylePage();
				InitGaugePages();
			}
			tabs.SelectedTabPageIndex = 0;
			btnOk.Click += new EventHandler(btnOk_Click);
			StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			ClientSize = new Size(650, 500);
		}
		protected override bool GetAllowSkin() {
			bool isDesignTime = (GaugeContainer != null) && GaugeContainer.DesignMode;
			bool isSkin = (LookAndFeel != null) && LookAndFeel.ActiveStyle == DevExpress.LookAndFeel.ActiveLookAndFeelStyle.Skin;
			return (isDesignTime && isSkin) || base.GetAllowSkin();
		}
		void btnOk_Click(object sender, EventArgs e) {
			for(int i = 0; i < gaugeStyles.Length; i++) {
				IGauge target = GaugeContainer.Gauges[i];
				gaugeStyles[i].Apply(target);
				StyleCollection style = gaugeStyleCollections[i];
				if(style != null)
					style.Apply(target);
			}
			if(useControlStyleSkinSettings) {
				ResetControlStyleSettings();
				ResetControlStyleSettings(GaugeContainer);
			}
			if(!IsWinGaugeContainer) {
				object value;
				if(controlStyle.Setters.TryGetValue("LayoutPadding", out value)) {
					((Core.Layout.ILayoutManagerContainer)GaugeContainer).LayoutPadding = value as IThickness;
					controlStyle.Setters.Remove("LayoutPadding");
				}
			}
			controlStyle.Apply(GaugeContainer);
			GaugeContainer.InvalidateRect(RectangleF.Empty);
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		void InitControlStylePage() {
			ceUseSkinSettings.Checked = useControlStyleSkinSettings;
			ceUseSkinSettings.EditValueChanged += UseSkinSettingsChanged;
			groupBackground.Enabled = !useControlStyleSkinSettings;
			InitBackColorEditor();
			InitBackgroundImageEditor();
			InitBackgroundImageLayoutEditor();
		}
		void InitGaugePages() {
			if(GaugeContainer.Gauges.Count > 0) {
				InitAutoLayoutEditor();
				InitGaugeSelectors();
				InitBoundsEditor();
				InitPaddingEditor();
				InitIntervalEditor();
			}
			else {
				tabs.TabPages[1].Visibility = LayoutVisibility.Never;
				tabs.TabPages[2].Visibility = LayoutVisibility.Never;
			}
		}
		PropertyDescriptor backColor;
		PropertyDescriptor backgroundImage;
		PropertyDescriptor backgroundImageLayout;
		PropertyDescriptor autoLayout;
		PropertyDescriptor layoutPadding;
		PropertyDescriptor layoutInterval;
		readonly bool IsWinGaugeContainer;
		bool useControlStyleSkinSettings = true;
		void InitControlStyle(IGaugeContainer container) {
			controlStyle = new Style(container.GetType());
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(container);
			backColor = properties["BackColor"];
			if(backColor != null) {
				controlStyle.Setters.Add("BackColor", backColor.GetValue(container));
				useControlStyleSkinSettings &= (!backColor.ShouldSerializeValue(container));
			}
			autoLayout = properties["AutoLayout"];
			if(autoLayout != null)
				controlStyle.Setters.Add("AutoLayout", autoLayout.GetValue(container));
			layoutPadding = properties["LayoutPadding"];
			if(layoutPadding != null)
				controlStyle.Setters.Add("LayoutPadding", layoutPadding.GetValue(container));
			layoutInterval = properties["LayoutInterval"];
			if(layoutInterval != null)
				controlStyle.Setters.Add("LayoutInterval", layoutInterval.GetValue(container));
			if(IsWinGaugeContainer) {
				backgroundImage = properties["BackgroundImage"];
				if(backgroundImage != null) {
					controlStyle.Setters.Add("BackgroundImage", backgroundImage.GetValue(container));
					useControlStyleSkinSettings &= (!backgroundImage.ShouldSerializeValue(container));
				}
				backgroundImageLayout = properties["BackgroundImageLayout"];
				if(backgroundImageLayout != null) {
					controlStyle.Setters.Add("BackgroundImageLayout", backgroundImageLayout.GetValue(container));
					useControlStyleSkinSettings &= (!backgroundImageLayout.ShouldSerializeValue(container));
				}
			}
			else {
				ceUseSkinSettingsItem.Visibility = LayoutVisibility.Never;
				controlStyle.Setters.Add("BorderStyle", DevExpress.XtraEditors.Controls.BorderStyles.NoBorder);
				ReInitWinPropertyDescriptors();
			}
			controlStyle.Apply(gcControlStyle, true);
			controlStyle.Apply(gcGaugesStyle, true);
			controlStyle.Apply(gcGaugesLayout, true);
		}
		void ReInitWinPropertyDescriptors() {
			PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(ControlLoader.GetWinGaugeControlType());
			backColor = properties["BackColor"];
			autoLayout = properties["AutoLayout"];
			layoutPadding = properties["LayoutPadding"];
			layoutInterval = properties["LayoutInterval"];
			object value;
			if(controlStyle.Setters.TryGetValue("LayoutPadding", out value))
				controlStyle.Setters.Add("LayoutPadding", new Thickness(value as IThickness));
		}
		void InitGaugeStyles(IGaugeContainer container) {
			gaugeStyles = new Style[container.Gauges.Count];
			gaugeStyleCollections = new StyleCollection[container.Gauges.Count];
			for(int i = 0; i < gaugeStyles.Length; i++) {
				gaugeStyles[i] = new Style(typeof(IGauge));
				gaugeStyles[i].Setters.Add("Bounds", container.Gauges[i].Bounds);
			}
		}
		IGaugeContainer gaugeContainerCore;
		public IGaugeContainer GaugeContainer {
			get { return gaugeContainerCore; }
		}
		void LoadLayout() {
			using(MemoryStream ms = new MemoryStream()) {
				GaugeContainer.SaveLayoutToStream(ms);
				ms.Seek(0, SeekOrigin.Begin);
				gcControlStyle.RestoreLayoutFromStream(ms);
				ms.Seek(0, SeekOrigin.Begin);
				gcGaugesStyle.RestoreLayoutFromStream(ms);
				ms.Seek(0, SeekOrigin.Begin);
				gcGaugesLayout.RestoreLayoutFromStream(ms);
			}
		}
		void InitSelectionTool() {
			if(GaugeContainer.Gauges.Count > 1) {
				((Control)gcGaugesLayout).MouseDown += LayoutGaugeMouseDown;
				((Control)gcGaugesStyle).MouseDown += StyleGaugeMouseDown;
				foreach(BaseGauge gauge in gcGaugesLayout.Gauges)
					gauge.CustomDrawElement += LayoutGaugeCustomDrawElement;
				foreach(BaseGauge gauge in gcGaugesStyle.Gauges)
					gauge.CustomDrawElement += StyleGaugeCustomDrawElement;
			}
		}
		void LayoutGaugeMouseDown(object sender, MouseEventArgs e) {
			SelectGaugeByMouse(gcGaugesLayout, e);
		}
		void StyleGaugeMouseDown(object sender, MouseEventArgs e) {
			SelectGaugeByMouse(gcGaugesStyle, e);
		}
		void LayoutGaugeCustomDrawElement(object sender, CustomDrawElementEventArgs e) {
			IGauge gauge = sender as IGauge;
			if(gauge.Name == (string)cbGaugeLayout.EditValue)
				DrawSelection(e);
		}
		void StyleGaugeCustomDrawElement(object sender, CustomDrawElementEventArgs e) {
			IGauge gauge = sender as IGauge;
			if(gauge.Name == (string)cbGaugeStyle.EditValue)
				DrawSelection(e);
		}
		void SelectGaugeByMouse(IGaugeContainer container, MouseEventArgs e) {
			BasePrimitiveHitInfo hitInfo = container.CalcHitInfo(e.Location);
			if(hitInfo.Element != null) {
				BaseGaugeModel model = BaseGaugeModel.Find(hitInfo.Element);
				if(model != null) {
					cbGaugeLayout.EditValue = model.Owner.Name;
					cbGaugeStyle.EditValue = model.Owner.Name;
				}
			}
		}
		Brush selectionFill = new SolidBrush(Color.FromArgb(200, 211, 234, 255));
		Pen selectionBorder = new Pen(Color.FromArgb(200, 160, 189, 226), 1);
		void DrawSelection(CustomDrawElementEventArgs e) {
			float l = Math.Min(e.Info.BoundBox.Width, e.Info.BoundBox.Height);
			float dx = l * 0.005f; float r = l * 0.025f;
			using(GraphicsPath rect = GetRoundedRect(RectangleF.Inflate(e.Info.BoundBox, dx, dx), r)) {
				e.Context.Graphics.FillPath(selectionFill, rect);
				e.Context.Graphics.DrawPath(selectionBorder, rect);
			}
		}
		static GraphicsPath GetRoundedRect(RectangleF baseRect, float radius) {
			float diameter = radius * 2.0F;
			RectangleF arc = new RectangleF(baseRect.Location, new SizeF(diameter, diameter));
			GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
			path.AddArc(arc, 180, 90);
			arc.X = baseRect.Right - diameter;
			path.AddArc(arc, 270, 90);
			arc.Y = baseRect.Bottom - diameter;
			path.AddArc(arc, 0, 90);
			arc.X = baseRect.Left;
			path.AddArc(arc, 90, 90);
			path.CloseFigure();
			return path;
		}
		void InitBackColorEditor() {
			object value = null;
			bool hasProperty = controlStyle.Setters.TryGetValue("BackColor", out value);
			itemBackColor.Visibility = hasProperty ? LayoutVisibility.Always : LayoutVisibility.Never;
			if(hasProperty) {
				clrBackColor.EditValue = value;
				clrBackColor.EditValueChanged += BackColorValueChanged;
			}
		}
		void InitBackgroundImageEditor() {
			object value = null;
			bool hasProperty = controlStyle.Setters.TryGetValue("BackgroundImage", out value);
			itemBackgroundImage.Visibility = hasProperty ? LayoutVisibility.Always : LayoutVisibility.Never;
			if(hasProperty) {
				peBackgroundImage.EditValue = value;
				peBackgroundImage.EditValueChanged += BackgroundImageValueChanged;
				cbBackgroundImageLayout.Enabled = (value != null);
			}
		}
		void InitBackgroundImageLayoutEditor() {
			object value = null;
			bool hasProperty = controlStyle.Setters.TryGetValue("BackgroundImageLayout", out value);
			itemBackgroundImageLayout.Visibility = hasProperty ? LayoutVisibility.Always : LayoutVisibility.Never;
			if(hasProperty) {
				cbBackgroundImageLayout.EditValue = value;
				cbBackgroundImageLayout.Properties.Items.AddRange(Enum.GetValues(value.GetType()));
				cbBackgroundImageLayout.EditValueChanged += BackgroundImageLayoutValueChanged;
			}
		}
		void UseSkinSettingsChanged(object sender, EventArgs e) {
			useControlStyleSkinSettings = ceUseSkinSettings.Checked;
			groupBackground.Enabled = !useControlStyleSkinSettings;
			if(useControlStyleSkinSettings) {
				ResetControlStyleSettings(gcControlStyle);
				ResetControlStyleSettings(gcGaugesStyle);
				ResetControlStyleSettings(gcGaugesLayout);
			}
			else {
				BackColorValueChanged(null, EventArgs.Empty);
				BackgroundImageValueChanged(null, EventArgs.Empty);
				BackgroundImageLayoutValueChanged(null, EventArgs.Empty);
			}
		}
		void ResetControlStyleSettings() {
			controlStyle.Setters.Remove("BackColor");
			controlStyle.Setters.Remove("BackgroundImage");
			controlStyle.Setters.Remove("BackgroundImageLayout");
		}
		void ResetControlStyleSettings(IGaugeContainer container) {
			backColor.ResetValue(container);
			backgroundImage.ResetValue(container);
			backgroundImageLayout.ResetValue(container);
			container.InvalidateRect(Rectangle.Empty);
		}
		void BackColorValueChanged(object sender, EventArgs e) {
			object value = clrBackColor.EditValue;
			controlStyle.Setters.Add("BackColor", value);
			backColor.SetValue(gcControlStyle, value);
			backColor.SetValue(gcGaugesStyle, value);
			backColor.SetValue(gcGaugesLayout, value);
			gcControlStyle.InvalidateRect(Rectangle.Empty);
		}
		void BackgroundImageValueChanged(object sender, EventArgs e) {
			object value = peBackgroundImage.Image;
			controlStyle.Setters.Add("BackgroundImage", value);
			backgroundImage.SetValue(gcControlStyle, value);
			backgroundImage.SetValue(gcGaugesStyle, value);
			backgroundImage.SetValue(gcGaugesLayout, value);
			cbBackgroundImageLayout.Enabled = (value != null);
			gcControlStyle.InvalidateRect(Rectangle.Empty);
		}
		void BackgroundImageLayoutValueChanged(object sender, EventArgs e) {
			object value = cbBackgroundImageLayout.EditValue;
			controlStyle.Setters.Add("BackgroundImageLayout", value);
			backgroundImageLayout.SetValue(gcControlStyle, value);
			backgroundImageLayout.SetValue(gcGaugesStyle, value);
			backgroundImageLayout.SetValue(gcGaugesLayout, value);
			gcControlStyle.InvalidateRect(Rectangle.Empty);
		}
		void InitAutoLayoutEditor() {
			object value = null;
			bool hasProperty = controlStyle.Setters.TryGetValue("AutoLayout", out value);
			itemAutoLayout.Visibility = hasProperty ? LayoutVisibility.Always : LayoutVisibility.Never;
			if(hasProperty) {
				checkAutoLayout.EditValue = value;
				checkAutoLayout.EditValueChanged += AutoLayoutValueChanged;
				itemGaugeBounds.Visibility = (bool)value ? LayoutVisibility.Never : LayoutVisibility.Always;
				groupBounds.Visibility = (bool)value ? LayoutVisibility.Never : LayoutVisibility.Always;
				groupPadding.Visibility = (bool)value ? LayoutVisibility.Always : LayoutVisibility.Never;
				itemInterval.Visibility = (bool)value ? LayoutVisibility.Always : LayoutVisibility.Never;
			}
		}
		void InitGaugeSelectors() {
			foreach(IGauge gauge in GaugeContainer.Gauges) {
				cbGaugeStyle.Properties.Items.Add(gauge.Name);
				cbGaugeLayout.Properties.Items.Add(gauge.Name);
			}
			cbGaugeStyle.EditValueChanged += StyleGaugeSelected;
			cbGaugeLayout.EditValueChanged += LayoutGaugeSelected;
			cbGaugeStyle.EditValue = GaugeContainer.Gauges[0].Name;
			cbGaugeLayout.EditValue = GaugeContainer.Gauges[0].Name;
		}
		void InitBoundsEditor() {
			bool auto = checkAutoLayout.Checked;
			UpdateBoundsEditor(auto ? null : gcGaugesLayout.Gauges[(string)cbGaugeStyle.EditValue]);
			spinEditLeft.EditValueChanged += GaugeBoundsChanged;
			spinEditTop.EditValueChanged += GaugeBoundsChanged;
			spinEditWidth.EditValueChanged += GaugeBoundsChanged;
			spinEditHeight.EditValueChanged += GaugeBoundsChanged;
		}
		void InitPaddingEditor() {
			bool auto = checkAutoLayout.Checked;
			UpdatePaddingEditor(auto ? gcGaugesLayout : null);
			spinEditPAll.EditValueChanged += GaugeContainerPaddindChanged;
			spinEditPLeft.EditValueChanged += GaugeContainerPaddindChanged;
			spinEditPTop.EditValueChanged += GaugeContainerPaddindChanged;
			spinEditPRight.EditValueChanged += GaugeContainerPaddindChanged;
			spinEditPBottom.EditValueChanged += GaugeContainerPaddindChanged;
		}
		void InitIntervalEditor() {
			bool auto = checkAutoLayout.Checked;
			if(GaugeContainer.Gauges.Count > 1) {
				UpdateIntervalEditor(auto ? gcGaugesLayout : null);
				spinEditInterval.EditValueChanged += GaugeContainerIntervalChanged;
			}
			else itemInterval.Control.Enabled = false;
		}
		int lockUpdateBoundsEditor = 0;
		void UpdateBoundsEditor(IGauge gauge) {
			lockUpdateBoundsEditor++;
			spinEditLeft.EditValue = (gauge == null) ? null : (object)gauge.Bounds.Left;
			spinEditTop.EditValue = (gauge == null) ? null : (object)gauge.Bounds.Top;
			spinEditWidth.EditValue = (gauge == null) ? null : (object)gauge.Bounds.Width;
			spinEditHeight.EditValue = (gauge == null) ? null : (object)gauge.Bounds.Height;
			lockUpdateBoundsEditor--;
		}
		int lockUpdatePaddingEditor = 0;
		void UpdatePaddingEditor(IGaugeContainer gaugeContainer) {
			lockUpdatePaddingEditor++;
			object value = (gaugeContainer == null) ? null : layoutPadding.GetValue(gaugeContainer);
			spinEditPAll.EditValue = (value == null) ? null : (object)((Thickness)value).All;
			spinEditPLeft.EditValue = (value == null) ? null : (object)((Thickness)value).Left;
			spinEditPTop.EditValue = (value == null) ? null : (object)((Thickness)value).Top;
			spinEditPRight.EditValue = (value == null) ? null : (object)((Thickness)value).Right;
			spinEditPBottom.EditValue = (value == null) ? null : (object)((Thickness)value).Bottom;
			lockUpdatePaddingEditor--;
		}
		int lockUpdateIntervalEditor = 0;
		void UpdateIntervalEditor(IGaugeContainer gaugeContainer) {
			lockUpdateIntervalEditor++;
			object value = (gaugeContainer == null) ? null : layoutInterval.GetValue(gaugeContainer);
			spinEditInterval.EditValue = (value == null) ? null : value;
			lockUpdateIntervalEditor--;
		}
		void GaugeBoundsChanged(object sender, EventArgs e) {
			if(lockUpdateBoundsEditor > 0) return;
			int index = cbGaugeLayout.SelectedIndex;
			Rectangle bounds = new Rectangle(
				(int)spinEditLeft.Value, (int)spinEditTop.Value,
				(int)spinEditWidth.Value, (int)spinEditHeight.Value);
			gaugeStyles[index].Setters.Add("Bounds", bounds);
			gcControlStyle.Gauges[index].Bounds = bounds;
			gcGaugesStyle.Gauges[index].Bounds = bounds;
			gcGaugesLayout.Gauges[index].Bounds = bounds;
			gcGaugesLayout.InvalidateRect(RectangleF.Empty);
		}
		void GaugeContainerPaddindChanged(object sender, EventArgs e) {
			if(lockUpdatePaddingEditor > 0) return;
			Thickness value;
			if(sender != spinEditPAll)
				value = new Thickness((int)spinEditPLeft.Value, (int)spinEditPTop.Value, (int)spinEditPRight.Value, (int)spinEditPBottom.Value);
			else value = new Thickness((int)spinEditPAll.Value);
			controlStyle.Setters.Add("LayoutPadding", value);
			layoutPadding.SetValue(gcControlStyle, value);
			layoutPadding.SetValue(gcGaugesStyle, value);
			layoutPadding.SetValue(gcGaugesLayout, value);
			UpdatePaddingEditor(gcGaugesLayout);
			gcGaugesLayout.InvalidateRect(RectangleF.Empty);
		}
		void GaugeContainerIntervalChanged(object sender, EventArgs e) {
			if(lockUpdateIntervalEditor > 0) return;
			int value = (int)spinEditInterval.Value;
			controlStyle.Setters.Add("LayoutInterval", value);
			layoutInterval.SetValue(gcControlStyle, value);
			layoutInterval.SetValue(gcGaugesStyle, value);
			layoutInterval.SetValue(gcGaugesLayout, value);
			gcGaugesLayout.InvalidateRect(RectangleF.Empty);
		}
		void StyleGaugeSelected(object sender, EventArgs e) {
			IGauge gauge = GaugeContainer.Gauges[(string)cbGaugeStyle.EditValue];
			string gaugeType = gauge.GetType().Name;
			switch(gaugeType) {
				case "CircularGauge":
				case "LinearGauge":
				case "DigitalGauge":
					btnChangeStyle.Enabled = true;
					break;
				default:
					btnChangeStyle.Enabled = false;
					break;
			}
			gcGaugesStyle.InvalidateRect(RectangleF.Empty);
		}
		void LayoutGaugeSelected(object sender, EventArgs e) {
			IGauge gauge = gcGaugesLayout.Gauges[(string)cbGaugeLayout.EditValue];
			UpdateBoundsEditor(gauge);
			gcGaugesLayout.InvalidateRect(RectangleF.Empty);
		}
		void AutoLayoutValueChanged(object sender, EventArgs e) {
			object value = checkAutoLayout.EditValue;
			controlStyle.Setters.Add("AutoLayout", value);
			autoLayout.SetValue(gcControlStyle, value);
			autoLayout.SetValue(gcGaugesStyle, value);
			autoLayout.SetValue(gcGaugesLayout, value);
			if((bool)value) {
				itemGaugeBounds.Visibility = LayoutVisibility.Never;
				groupBounds.Visibility = LayoutVisibility.Never;
				groupPadding.Visibility = LayoutVisibility.Always;
				itemInterval.Visibility = LayoutVisibility.Always;
			}
			else {
				groupPadding.Visibility = LayoutVisibility.Never;
				itemInterval.Visibility = LayoutVisibility.Never;
				itemGaugeBounds.Visibility = LayoutVisibility.Always;
				groupBounds.Visibility = LayoutVisibility.Always;
			}
			IGauge gauge = gcGaugesLayout.Gauges[(string)cbGaugeLayout.EditValue];
			UpdateBoundsEditor((bool)value ? null : gauge);
			UpdatePaddingEditor((bool)value ? gcGaugesLayout : null);
			UpdateIntervalEditor((bool)value ? gcGaugesLayout : null);
		}
		void GaugeChangeStyleClick(object sender, EventArgs e) {
			int index = cbGaugeStyle.SelectedIndex;
			IGauge gauge = gcGaugesStyle.Gauges[index];
			using(ChooseStyleForm styleChooser = new ChooseStyleForm(gauge)) {
				if(styleChooser.ShowDialog() != DialogResult.None) {
					if(styleChooser.IsStyleChanged) {
						StyleCollectionKey key = styleChooser.GetResult();
						StyleCollection styles = StyleLoader.Load(key);
						styles.Apply(gcControlStyle.Gauges[index]);
						styles.Apply(gcGaugesStyle.Gauges[index]);
						styles.Apply(gcGaugesLayout.Gauges[index]);
						gaugeStyleCollections[index] = styles;
					}
				}
			}
		}
		public static bool Show(IGaugeContainer gaugeContainer) {
			if(gaugeContainer != null) {
				using(StyleManagerForm styleManager = new StyleManagerForm(gaugeContainer)) {
					return styleManager.ShowDialog() == DialogResult.OK;
				}
			}
			return false;
		}
	}
	class StyleManagerServiceProvider : DevExpress.XtraGauges.Core.IStyleManagerService {
		bool DevExpress.XtraGauges.Core.IStyleManagerService.Show(IGaugeContainer gaugeContainer) {
			return StyleManagerForm.Show(gaugeContainer);
		}
	}
}
