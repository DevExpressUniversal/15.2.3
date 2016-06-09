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

using System.Windows.Forms;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using System;
using System.Drawing;
using System.ComponentModel;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraEditors;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Skins;
using DevExpress.XtraVerticalGrid.Events;
namespace DevExpress.XtraVerticalGrid {
	[DXToolboxItem(true), ToolboxTabName(AssemblyInfo.DXTabData),
		Description("Displays the current PropertyGridControl's property description.")
]
	[ToolboxBitmap(typeof(ToolboxIcons.ToolboxIconsRootNS), "PropertyDescriptionControl")]
	public class PropertyDescriptionControl : Control, IPanelControlOwner, ISupportLookAndFeel, IToolTipControlClient {
		UserLookAndFeel lookAndFeel;
		GroupObjectInfoArgs viewInfo;
		GroupObjectPainter painter;
		PDescControlAppearanceCollection appearance;
		AppearanceDefault defaultAppearance;
		BorderStyles borderStyle;
		PropertyGridControl propertyGrid;
		AppearanceObject captionAppearance;
		AppearanceObject descriptionAppearance;
		AppearanceObject panelAppearance;
		bool showHint = true;
		public PropertyDescriptionControl() {
			SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserMouse | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
			this.lookAndFeel = new ControlUserLookAndFeel(this);
			this.lookAndFeel.StyleChanged += new EventHandler(LookAndFeelStyleChanged);
			this.appearance = new PDescControlAppearanceCollection(this);
			this.appearance.Changed += new EventHandler(AppearanceChanged);
			this.borderStyle = BorderStyles.Default;
			TabStop = false;
			SetBounds(0, 0, 150, 100);
			ToolTipController.DefaultController.AddClientControl(this);
		}
		protected override void Dispose(bool disposing) {
			if(this.lookAndFeel != null) {
				this.lookAndFeel.StyleChanged -= new EventHandler(LookAndFeelStyleChanged);
				this.lookAndFeel.Dispose();
				this.lookAndFeel = null;
				this.appearance.Changed -= new EventHandler(AppearanceChanged);
				this.appearance.Dispose();
				this.appearance = null;
			}
			ToolTipController.DefaultController.RemoveClientControl(this);
			PropertyGrid = null;
			base.Dispose(disposing);
		}
		protected override void WndProc(ref Message m)
		{
			base.WndProc(ref m);
			CodedUISupport.CodedUIMessagesHandler.ProcessCodedUIMessage(ref m, this);
		}
		bool ShouldSerializeLookAndFeel() { return LookAndFeel.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyDescriptionControlLookAndFeel"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel { get { return lookAndFeel; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyDescriptionControlAppearance"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual PDescControlAppearanceCollection Appearance { get { return appearance; } }
#if !SL
	[DevExpressXtraVerticalGridLocalizedDescription("PropertyDescriptionControlShowHint")]
#endif
		[DefaultValue(true)]
		[Category("Behavior")]
		public virtual bool ShowHint {
			get { return showHint; }
			set {
				if (ShowHint == value)
					return;
				showHint = value;
			}
		}
		string ToolTip { get; set; }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyDescriptionControlPropertyGrid"),
#endif
 Category("Data"), DefaultValue(null)]
		public PropertyGridControl PropertyGrid {
			get { return propertyGrid; }
			set {
				if(value != null && value.IsDisposed)
					return;
				if(propertyGrid != null) {
					propertyGrid.FocusedRowChanged -= propertyGrid_FocusedRowChanged;
					propertyGrid.Disposed -= propertyGrid_Disposed;
				}
				propertyGrid = value;
				if(propertyGrid != null) {
					propertyGrid.Disposed += propertyGrid_Disposed;
					propertyGrid.FocusedRowChanged += propertyGrid_FocusedRowChanged;
				}
				Invalidate();
			}
		}
		void propertyGrid_Disposed(object sender, EventArgs e) {
			PropertyGrid = null;
		}
		void propertyGrid_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e) {
			Invalidate();
		}
		protected AppearanceDefault DefaultAppearance {
			get {
				if(defaultAppearance == null)
					defaultAppearance = CreateDefaultAppearance();
				return defaultAppearance;
			}
		}
		protected AppearanceDefault CreateDefaultAppearance() {
			AppearanceDefault res;
			if(Painter != null)
				res = Painter.DefaultAppearance.Clone() as AppearanceDefault;
			else
				res = new AppearanceDefault(SystemColors.ControlText, SystemColors.Control);
			if(LookAndFeel.ActiveStyle == ActiveLookAndFeelStyle.Skin) {
				CommonSkins.GetSkin(LookAndFeel)[IsDrawNoBorder ? CommonSkins.SkinGroupPanelNoBorder : CommonSkins.SkinGroupPanel].Apply(res);
				CommonSkins.GetSkin(LookAndFeel)[CommonSkins.SkinLabel].Apply(res);
			}
			res = LookAndFeelHelper.CheckColors(LookAndFeel, res, this);
			res.VAlignment = VertAlignment.Top;
			return res;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetBackColor() { BackColor = Color.Empty; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color BackColor { get { return base.BackColor; } set { base.BackColor = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public override void ResetForeColor() { ForeColor = Color.Empty; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Color ForeColor { get { return base.ForeColor; } set { base.ForeColor = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Font Font { get { return base.Font; } set { base.Font = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override Image BackgroundImage { get { return base.BackgroundImage; } set { base.BackgroundImage = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ImageLayout BackgroundImageLayout { get { return base.BackgroundImageLayout; } set { base.BackgroundImageLayout = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override RightToLeft RightToLeft { get { return base.RightToLeft; } set { base.RightToLeft = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override string Text { get { return base.Text; } set { base.Text = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override bool AllowDrop { get { return base.AllowDrop; } set { base.AllowDrop = value; } }
		protected bool IsDrawNoBorder { get { return BorderStyle == BorderStyles.NoBorder; } }
		protected Rectangle ClientBoundsRect { get { return new Rectangle(Point.Empty, Bounds.Size); } }
		protected bool ShouldSerializeBorderStyle() { return BorderStyle != BorderStyles.Default; }
		[
#if !SL
	DevExpressXtraVerticalGridLocalizedDescription("PropertyDescriptionControlBorderStyle"),
#endif
 Category("Appearance")]
		public BorderStyles BorderStyle {
			get {
				return borderStyle;
			}
			set {
				if(borderStyle == value) return;
				borderStyle = value;
				RequireAppearanceUpdate();
			}
		}
		protected GroupObjectInfoArgs ViewInfo {
			get {
				if(viewInfo == null)
					viewInfo = CreateViewInfo();
				return viewInfo;
			}
		}
		protected GroupObjectPainter Painter {
			get {
				if(painter == null)
					painter = CreatePainter();
				return painter;
			}
		}
		protected AppearanceObject PanelAppearance {
			get {
				if(panelAppearance == null)
					panelAppearance = new AppearanceObject();
				return panelAppearance;
			}
		}
		protected AppearanceObject CaptionAppearance {
			get {
				if(captionAppearance == null)
					captionAppearance = new AppearanceObject();
				return captionAppearance;
			}
		}
		protected AppearanceObject DescriptionAppearance {
			get {
				if(descriptionAppearance == null)
					descriptionAppearance = new AppearanceObject();
				return descriptionAppearance;
			}
		}
		void LookAndFeelStyleChanged(object sender, EventArgs e) {
			RequireAppearanceUpdate();
		}
		void AppearanceChanged(object sender, EventArgs e) {
			RequireAppearanceUpdate();
		}
		GroupObjectPainter CreatePainter() {
			switch(LookAndFeel.ActiveStyle) {
				case ActiveLookAndFeelStyle.Office2003:
					return new GroupObjectPainter(this);
				case ActiveLookAndFeelStyle.WindowsXP:
					return new WindowsXPGroupObjectPainter(this);
				case ActiveLookAndFeelStyle.Skin:
					return new SkinGroupObjectPainter(this, LookAndFeel.ActiveLookAndFeel);
			}
			return new FlatGroupObjectPainter(this);
		}
		GroupObjectInfoArgs CreateViewInfo() {
			GroupObjectInfoArgs res = new GroupObjectInfoArgs();
			return res;
		}
		bool CanShowHint { get; set; }
		const int Offset = 8;
		protected override void OnPaint(PaintEventArgs e) {
			UpdateViewInfo();
			using(GraphicsCache cache = new GraphicsCache(e)) {
				ObjectPainter.DrawObject(cache, Painter, ViewInfo);
				if(PropertyGrid != null && PropertyGrid.FocusedRow != null && PropertyGrid.SelectedObject != null && !(PropertyGrid.FocusedRow is CategoryRow)) {
					Rectangle captionBounds = ClientRectangle;
					captionBounds.Inflate(-Offset, -Offset);
					string displayName = PropertyGrid.FocusedRow.Properties.Caption;
					Size captionSize = CaptionAppearance.CalcTextSizeInt(cache, displayName, captionBounds.Width);
					Rectangle captionRectangle = new Rectangle(captionBounds.Location, new Size(captionBounds.Width, captionSize.Height));
					CaptionAppearance.DrawString(cache, displayName, captionRectangle);
					string description = GetPropertyDescription(PropertyGrid.FocusedRow);
					ToolTip = description;
					if(!string.IsNullOrEmpty(description)) {
						Rectangle descriptionBounds = captionBounds;
						descriptionBounds.Y += (int)captionSize.Height + Offset;
						descriptionBounds.Height -= (int)captionSize.Height + Offset;
						Size desiredDescriptionSize = DescriptionAppearance.CalcTextSizeInt(cache, description, descriptionBounds.Width);
						CanShowHint = descriptionBounds.Height < desiredDescriptionSize.Height || descriptionBounds.Width <= desiredDescriptionSize.Width;
						DescriptionAppearance.DrawString(cache, description, descriptionBounds);
					}
				}
			}
		}
		protected string GetPropertyDescription(BaseRow row) {
			if(row == null)
				return string.Empty;
			PropertyDescriptor pd = PropertyGrid.GetPropertyDescriptor(row);
			if(pd == null)
				return string.Empty;
			return pd.Description;
		}
		void UpdateViewInfo() {
			if(ViewInfo.Bounds != Rectangle.Empty)
				return;
			AppearanceHelper.Combine(PanelAppearance, new AppearanceObject[] { Appearance.Panel }, DefaultAppearance);
			ViewInfo.SetAppearance(PanelAppearance);
			ViewInfo.Bounds = ClientBoundsRect;
			ViewInfo.BorderStyle = BorderStyle;
			ViewInfo.ShowCaption = false;
			ViewInfo.ShowCaptionImage = false;
			Painter.CalcObjectBounds(ViewInfo);
			SetCaptionDefaults();
			SetDescriptionDefaults();
		}
		protected virtual void SetCaptionDefaults() {
			AppearanceHelper.Combine(CaptionAppearance, new AppearanceObject[] { Appearance.Caption }, DefaultAppearance);
			if(CaptionAppearance.TextOptions.WordWrap == WordWrap.Default) {
				CaptionAppearance.TextOptions.WordWrap = WordWrap.NoWrap;
			}
			if(CaptionAppearance.TextOptions.Trimming == Trimming.Default) {
				CaptionAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			}
		}
		protected virtual void SetDescriptionDefaults() {
			AppearanceHelper.Combine(DescriptionAppearance, new AppearanceObject[] { Appearance.Description }, DefaultAppearance);
			if(DescriptionAppearance.TextOptions.WordWrap == WordWrap.Default) {
				DescriptionAppearance.TextOptions.WordWrap = WordWrap.Wrap;
			}
			if (DescriptionAppearance.TextOptions.Trimming == Trimming.Default) {
				DescriptionAppearance.TextOptions.Trimming = Trimming.EllipsisCharacter;
			}
		}
		protected override void OnResize(EventArgs e) {
			RequireAppearanceUpdate();
			base.OnResize(e);
		}
		protected virtual ToolTipControlInfo GetObjectInfo(Point point) {
			if (ShowHint && CanShowHint) {
				return new ToolTipControlInfo(ViewInfo, ToolTip);
			}
			return null;
		}
		private void RequireAppearanceUpdate() {
			this.defaultAppearance = null;
			this.painter = null;
			this.viewInfo = null;
			Invalidate();
		}
		Color IPanelControlOwner.GetForeColor() {
			return Color.Empty;
		}
		void IPanelControlOwner.OnCustomDrawCaption(GroupCaptionCustomDrawEventArgs e) {
		}
		bool ISupportLookAndFeel.IgnoreChildren { get { return false; } }
		ToolTipControlInfo IToolTipControlClient.GetObjectInfo(Point point) {
			return GetObjectInfo(point);
		}
		bool IToolTipControlClient.ShowToolTips { get { return true; } }
	}
}
