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
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows.Forms;
using System.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.NativeBricks;
using DevExpress.Skins;
namespace DevExpress.XtraEditors.Repository {
	internal class ObsoleteText {
		internal const string
			SRObsoletePropertiesText = "Please remove the 'Properties.' string from the call.",
			SRObsoleteComboBox = "Use the ComboBoxEdit control instead.",
			SRObsoleteProgressBar = "Use the ProgressBarControl control instead.",
			SRObsoletePickValue = "Use the EditValue property instead.",
			SRObsoletePickImage = "Use the ImageComboBoxEdit control instead.",
			SRObsoletePickImageRepository = "Use the RepositoryItemImageComboBox component instead.",
			SRObsoletePickImageItem = "Use the ImageComboBoxItem item instead.",
			SRObsoleteButtonsBorderStyle = "Use the ButtonsStyle property instead.",
			SRObsoleteCheckEditAllowHtmlString = "Use the AllowHtmlDraw property instead.",
			SRObsoleteLookUpData = "Please remove the 'LookUpData.' string from the call.",
			SRObsoleteLookUpKeyValue = "Use the editor's EditValue property instead.",
			SRObsoleteLookUpKeyField = "Use the ValueMember property instead.",
			SRObsoleteLookUpDisplayField = "Use the DisplayMember property instead.",
			SRObsoleteLookUpNullString = "Use the NullText property instead.",
			SRObsoleteDateOnError = "This property is obsolete.",
			SRObsoleteCycleEditing = "This property is obsolete.",
			SRObsoleteTimeFormat = "Use the EditMask property instead.",
			SRObsoleteHourFormat = "Use the EditMask property instead.",
			SRObsoleteImage = "Use the Image property instead.",
			SRObsoleteImageAlignment = "Use the ImageAlignment property instead.",
			SRObsoleteImageLocation = "Use the ImageLocation propery instead",
			SRObsoleteMaskData = "Use the Mask property instead.",
			SRObsoleteHotTrackItems = "Use the HotTrackItems property instead.",
			SRObsoleteDrawItem = "Use the DrawItem event instead.",
			SRObsoleteUseCtrlIncrement = "This property is obsolete.",
			SRObsoleteDefaultPopupHeight = "Use the DropDownRows property instead.",
			SRObsoletePopupStartSize = "Use the PopupFormSize property instead.",
			SRObsoleteShowAllItemCaption = "Use the SelectAllItemCaption property instead.",
			SRObsoleteShowAllItemVisible = "Use the SelectAllItemVisible property instead.",
			SRBaseStyleControl_Style = "Use the Appearance property instead.",
			SRLookUp_PopupHeaderStyle = "Use the AppearanceDropDownHeader property instead.",
			SRLookUp_PopupCellStyle = "Use the AppearanceDropDown property instead.",
			SRStyleController_Style = "Use the Appearance property instead.",
			SRDateEdit_PopupHeaderStyle = "Use the AppearanceDropDownHeader property instead.",
			SRDateEdit_PopupCalendarStyle = "Use the AppearanceDropDown property instead.",
			SRCalendarControl_AppearanceCalendar = "Use the CalendarAppearance property instead.",
			SRCalendarControl_AppearanceCalendar_DayCellDisabled = "Use the CalendarAppearance.DayCellDisabled property instead.",
			SRCalendarControl_AppearanceCalendar_CalendarHeader = "Use the CalendarAppearance.CalendarHeader and CalendarAppearance.Header property instead.",
			SRCalendarControl_AppearanceCalendar_WeekNumber = "Use the CalendarAppearance.WeekNumber property instead.",
			SRCalendarControl_EditValueChanged = "Use the EditValueChanged event instead.";
	}
	public class BitmapGraphicsHelper : IDisposable {
		Bitmap bmpCore;
		Graphics graphicsCore;
		public BitmapGraphicsHelper(int width, int height) {
			bmpCore = new Bitmap(width, height);
			graphicsCore = Graphics.FromImage(bmpCore);
		}
		public Graphics Graphics { get { return graphicsCore; } }
		public Bitmap Bitmap { get { return bmpCore; } }
		public Bitmap MemSafeBitmap { get { return new Bitmap(bmpCore); } }
		public void Dispose() {
			graphicsCore.Dispose();
			bmpCore.Dispose();
		}
	}
	[DesignTimeVisible(false), ToolboxItem(false), Designer("DevExpress.XtraEditors.Design.BaseRepositoryItemDesigner, " + AssemblyInfo.SRAssemblyEditorsDesign)
#if DXWhidbey
	,System.ComponentModel.Design.Serialization.DesignerSerializer("DevExpress.XtraEditors.Design.RepositoryItemCodeDomSerializer, " + AssemblyInfo.SRAssemblyEditorsDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, System.Design")
#endif
]
	public class RepositoryItem : Component, ISupportInitialize, ICustomTypeDescriptor, IImageCollectionHelper {
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), Obsolete(ObsoleteText.SRObsoletePropertiesText)]
		public RepositoryItem Properties { get { return this; } }
		IDictionary _links;
		IDictionary _propertyStore;
		BaseEdit _ownerEdit;
		string name;
		protected string fNullText;
		UserLookAndFeel lookAndFeel;
		int _lockInit, _lockUpdate, _lockEvents, bestFitWidth;
		bool _isFirstInit;
		ImageCollection htmlImages;
		DefaultBoolean allowHtmlDraw;
		protected BaseEditPainter fPainter;
		protected bool fAllowFocused, fAutoHeight, fEnabled, fReadOnly, fLockFormatParse, fAllowMouseWheel;
		protected EditValueChangedFiringMode fEditValueChangedFiringMode;
		protected BorderStyles fBorderStyle;
		ContextMenu _contextMenu;
#if DXWhidbey
		ContextMenuStrip _contextMenuStrip;
#endif
		Point popupOffset = Point.Empty;
		FormatInfo displayFormat, editFormat;
		protected ConvertEditValueEventArgs fConvertArgs;
		int editValueChangedDelay = 0;
		static int fEditValueChangedFiringDelay = 500;
		string accessibleDefaultActionDescription, accessibleDescription, accessibleName;
		AccessibleRole accessibleRole;
		object tag;
		bool useParentBackground;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static bool NormalizeDisplayText { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public static string GetNormalizedText(string source) {
			if(!NormalizeDisplayText || source == null) return source;
			return source.Normalize();
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public override ISite Site {
			get { return base.Site; }
			set {
				if(base.Site != value) {
					base.Site = value;
					if(value == null || string.IsNullOrEmpty(name) || IsDesignMode) return;
					try {
						Site.Name = name;
					}
					catch { }
				}
			}
		}
		private static readonly object queryAccessibilityHelp = new object();
		private static readonly object propertiesChanged = new object();
		private static readonly object refreshRequired = new object();
		private static readonly object click = new object();
		private static readonly object doubleClick = new object();
		private static readonly object dragDrop = new object();
		private static readonly object dragEnter = new object();
		private static readonly object dragLeave = new object();
		private static readonly object dragOver = new object();
		private static readonly object enter = new object();
		private static readonly object giveFeedback = new object();
		private static readonly object helpRequested = new object();
		private static readonly object keyDown = new object();
		private static readonly object keyPress = new object();
		private static readonly object keyUp = new object();
		private static readonly object leave = new object();
		private static readonly object mouseDown = new object();
		private static readonly object mouseEnter = new object();
		private static readonly object mouseHover = new object();
		private static readonly object mouseLeave = new object();
		private static readonly object mouseMove = new object();
		private static readonly object mouseUp = new object();
		private static readonly object mouseWheel = new object();
		private static readonly object queryContinueDrag = new object();
		private static readonly object validating = new object();
		private static readonly object editValueChanging = new object();
		private static readonly object editValueChanged = new object();
		private static readonly object modified = new object();
		private static readonly object parseEditValue = new object();
		private static readonly object formatEditValue = new object();
		private static readonly object customDisplayText = new object();
		private static readonly object queryProcessKey = new object();
		AppearanceObject appearanceDisabled, appearance, appearanceFocused, appearanceReadOnly;
		static RepositoryItem() {
		}
		public RepositoryItem() {
			this.htmlImages = null;
			this.appearance = CreateAppearance("Control");
			this.appearanceReadOnly = CreateAppearance("ControlReadOnly");
			this.appearanceFocused = CreateAppearance("ControlFocused");
			this.appearanceDisabled = CreateAppearance("ControlDisabled");
			this.fLockFormatParse = false;
			this.accessibleName = this.accessibleDefaultActionDescription = this.accessibleDescription = null;
			this.accessibleRole = AccessibleRole.Default;
			this.fConvertArgs = new ConvertEditValueEventArgs();
			this._propertyStore = new HybridDictionary();
			this.fNullText = "";
			this.allowHtmlDraw = DefaultBoolean.Default;
			this._isFirstInit = true;
			this._links = null;
			this._ownerEdit = null;
			CreateLookAndFeel();
			this.fPainter = CreatePainter();
			this._lockInit = this._lockUpdate = this._lockEvents = 0;
			this.fAllowFocused = true;
			this.fAllowMouseWheel = true;
			this.fAutoHeight = true;
			this.fEnabled = true;
			this.name = "";
			this.fReadOnly = false;
			this.fEditValueChangedFiringMode = EditValueChangedFiringMode.Default;
			this.fBorderStyle = BorderStyles.Default;
			this._contextMenu = null;
#if DXWhidbey
			this._contextMenuStrip = null;
#endif
			this.displayFormat = CreateDisplayFormat();
			this.editFormat = CreateEditFormat();
			this.tag = null;
			this.bestFitWidth = -1;
			this.AllowInplaceBorderPainter = true;
			this.DefaultBorderStyleInGrid = BorderStyles.NoBorder;
			DisplayFormat.Changed += new EventHandler(OnFormat_Changed);
			EditFormat.Changed += new EventHandler(OnFormat_Changed);
		}
		protected virtual FormatInfo CreateEditFormat() {
			return new FormatInfo();
		}
		protected virtual FormatInfo CreateDisplayFormat() {
			return new FormatInfo();
		}
		protected internal SizeF ScaleFactor { get { return OwnerEdit == null ? new SizeF(1, 1) : OwnerEdit.ScaleFactor; } }
		protected internal Size ScaleSize(Size size) { return RectangleHelper.ScaleSize(size, ScaleFactor); }
		protected internal int ScaleHorizontal(int width) { return RectangleHelper.ScaleHorizontal(width, ScaleFactor.Width); }
		protected internal int ScaleVertical(int height) { return RectangleHelper.ScaleVertical(height, ScaleFactor.Height); }
		protected internal int DeScaleHorizontal(int width) { return RectangleHelper.DeScaleHorizontal(width, ScaleFactor.Width); }
		protected internal int DeScaleVertical(int height) { return RectangleHelper.DeScaleVertical(height, ScaleFactor.Height); }
		public override string ToString() { return ""; }
		public override int GetHashCode() { return base.GetHashCode(); } 
		Control IImageCollectionHelper.OwnerControl { get { return OwnerEdit; } }
		protected internal virtual DevExpress.Accessibility.BaseAccessible CreateAccessibleInstance() {
			return EditorClassInfo.CreateAccessible(this);
		}
		protected virtual AppearanceObject CreateAppearanceCore(string name) {
			return new AppearanceObject(name);
		}
		protected AppearanceObject CreateAppearance(string name) {
			AppearanceObject res = CreateAppearanceCore(name);
			res.PaintChanged += new EventHandler(OnAppearancePaintChanged);
			res.SizeChanged += new EventHandler(OnAppearanceSizeChanged);
			return res;
		}
		protected void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.PaintChanged -= new EventHandler(OnAppearancePaintChanged);
			appearance.SizeChanged -= new EventHandler(OnAppearanceSizeChanged);
			appearance.Dispose();
		}
		protected internal bool Cloned = false;
		public virtual object Clone() {
			RepositoryItem item = CreateRepositoryItem();
			item.Cloned = true;
			item.Assign(this);
			return item;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void SetCloned() {
			Cloned = true;
		}
		bool isDisposedCore = false;
		[Browsable(false)]
		public bool IsDisposed { get { return isDisposedCore; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(OwnerEdit != null) OwnerEdit.StyleController = null;
				DestroyLookAndFeel();
				if(delayedEditValueChangedTimer != null)
					delayedEditValueChangedTimer.Dispose();
				if(this.designer != null) {
					this.designer.Dispose();
					this.designer = null;
				}
				DestroyAppearances();
				this.tag = null;
			}
			base.Dispose(disposing);
			isDisposedCore = true;
		}
		protected virtual void DestroyLookAndFeel() {
			if(lookAndFeel != null) {
				lookAndFeel.StyleChanged -= new EventHandler(OnLookAndFeelChanged);
				lookAndFeel.Dispose();
				this.lookAndFeel = null;
			}
		}
		protected virtual void CreateLookAndFeel() {
			LookAndFeelProperties props = null;
			if(this.lookAndFeel != null) {
				props = new LookAndFeelProperties(this.lookAndFeel);
				DestroyLookAndFeel();
			}
			if(OwnerEdit == null)
				this.lookAndFeel = new UserLookAndFeel(this);
			else
				this.lookAndFeel = new ControlUserLookAndFeel(OwnerEdit);
			if(props != null) props.SetTo(this.lookAndFeel);
			this.LookAndFeel.StyleChanged += new EventHandler(OnLookAndFeelChanged);
		}
		protected virtual void DestroyAppearances() {
			DestroyAppearance(Appearance);
			DestroyAppearance(AppearanceFocused);
			DestroyAppearance(AppearanceDisabled);
			DestroyAppearance(AppearanceReadOnly);
		}
		[Obsolete()]
		protected virtual void DestroyStyles() {
		}
		protected virtual ConvertEditValueEventArgs ConvertArgs { get { return fConvertArgs; } }
		protected internal virtual IDictionary PropertyStore {
			get { return _propertyStore; }
			set {
				if(value == null) return;
				this._propertyStore = value;
			}
		}
		protected internal virtual bool GetAllowHtmlDraw() {
			if(AllowHtmlDraw == DefaultBoolean.False) return false;
			if(AllowHtmlDraw == DefaultBoolean.True) return true;
			return DevExpress.Utils.Paint.XPaint.DefaultAllowHtmlDraw;
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAllowHtmlDraw"),
#endif
 DefaultValue(DefaultBoolean.Default)]
		public virtual DefaultBoolean AllowHtmlDraw {
			get { return allowHtmlDraw; }
			set {
				if(AllowHtmlDraw == value) return;
				allowHtmlDraw = value;
				OnPropertiesChanged();
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAccessibleName"),
#endif
 DXCategory(CategoryName.Accessibility), DefaultValue(null), Localizable(true)]
		public virtual string AccessibleName {
			get { return accessibleName; }
			set {
				accessibleName = value;
				if(OwnerEdit != null) OwnerEdit.SetAccessibleName(value);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAccessibleRole"),
#endif
 DXCategory(CategoryName.Accessibility), DefaultValue(AccessibleRole.Default)]
		public virtual AccessibleRole AccessibleRole {
			get { return accessibleRole; }
			set {
				accessibleRole = value;
				if(OwnerEdit != null) OwnerEdit.SetAccessibleRole(value);
			}
		}
		[DXCategory(CategoryName.Accessibility),
		Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual string AccessibleDefaultActionDescription {
			get { return accessibleDefaultActionDescription; }
			set {
				accessibleDefaultActionDescription = value;
				if(OwnerEdit != null) OwnerEdit.SetAccessibleDefaultActionDescription(value);
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAccessibleDescription"),
#endif
 DXCategory(CategoryName.Accessibility), DefaultValue(null), Localizable(true)]
		public virtual string AccessibleDescription {
			get { return accessibleDescription; }
			set {
				accessibleDescription = value;
				if(OwnerEdit != null) OwnerEdit.SetAccessibleDescription(value);
			}
		}
		#region PrintingSupport
		public static DevExpress.XtraPrinting.BorderSide GetBorderSides(bool printHLines, bool printVLines) {
			DevExpress.XtraPrinting.BorderSide sides = DevExpress.XtraPrinting.BorderSide.All;
			if(printHLines && printVLines)
				sides = DevExpress.XtraPrinting.BorderSide.All;
			if(!printHLines && !printVLines)
				sides = DevExpress.XtraPrinting.BorderSide.None;
			if(!printHLines && printVLines)
				sides = DevExpress.XtraPrinting.BorderSide.Left | DevExpress.XtraPrinting.BorderSide.Right;
			if(printHLines && !printVLines)
				sides = DevExpress.XtraPrinting.BorderSide.Top | DevExpress.XtraPrinting.BorderSide.Bottom;
			return sides;
		}
		protected virtual object GetEditValueForExport(object editValue, ExportTarget exportTarget) {
			return editValue;
		}
		public virtual object GetEditValueForExportByOptions(object editValue, object fnP1, object fnP2, object fnP3, Function3<string, object, object, object> retrieveDisplayValueFn, ExportTarget exportTarget)
		{
			return GetExportMode() != ExportMode.DisplayText ? GetEditValueForExport(editValue, exportTarget) : retrieveDisplayValueFn(fnP1, fnP2, fnP3);
		}
		protected void SetCommonBrickProperties(IVisualBrick brick, PrintCellHelperInfo info) {
			object textValue = GetEditValueForExportByOptions(info.EditValue, info.DisplayText, null, null, (p1, p2, p3) => { return (string)p1; }, ExportTarget.Pdf);
			brick.TextValue = textValue;
			PanelBrick pbrick = brick as PanelBrick;
			if(pbrick != null) pbrick.Value = textValue;
			brick.UseTextAsDefaultHint = true;
		}
		protected virtual void SetupTextBrickStyleProperties(PrintCellHelperInfo info, BrickStyle style) {
			HorzAlignment halign = info.HAlignment == HorzAlignment.Default ? info.Appearance.HAlignment : info.HAlignment;
			StringTrimming trimming = ToStringTrimming(info.Appearance.TextOptions.Trimming);
			StringFormatFlags formatFlags = StringFormatFlags.NoClip | (info.Appearance.TextOptions.WordWrap != WordWrap.Wrap ? StringFormatFlags.NoWrap : (StringFormatFlags)0);
			StringAlignment vAlign = StringAlignment.Center;
			switch(info.Appearance.TextOptions.VAlignment){
				case VertAlignment.Top:
					vAlign = StringAlignment.Near;
					break;
				case VertAlignment.Bottom:
					vAlign = StringAlignment.Far;
					break;
				case VertAlignment.Default:
				case VertAlignment.Center:
					break;
			}
			BrickStringFormat bsf = new BrickStringFormat(PSConvert.ToStringAlignment(halign), vAlign, formatFlags, System.Drawing.Text.HotkeyPrefix.None, trimming);
			bsf.PrototypeKind = BrickStringFormatPrototypeKind.GenericTypographic;
			style.TextAlignment = DevExpress.XtraPrinting.Native.TextAlignmentConverter.ToTextAlignment(halign, info.Appearance.TextOptions.VAlignment);
			style.Font = info.Appearance.Font;
			style.ForeColor = info.Appearance.ForeColor == Color.Empty ? Color.Black : info.Appearance.ForeColor;
			style.StringFormat = bsf;
		}
		protected void SetupCommonBrickStyleProperties(PrintCellHelperInfo info, BrickStyle style) {
			style.BorderWidth = 1;
			style.Sides = info.GetSides();
			style.BorderStyle = BrickBorderStyle.Center;
			style.Padding = new PaddingInfo(2, 1, 0, 0);
			style.BackColor = info.Appearance.BackColor;
			style.BorderColor = info.LineColor;
		}
		protected RectangleF GetInPanelrect(Rectangle rectPanel, RectangleF rectItem) {
			return new RectangleF(rectItem.X - rectPanel.X, rectItem.Y - rectPanel.Y, rectItem.Width, rectItem.Height);
		}
		protected IImageBrick CreateImageBrick(PrintCellHelperInfo info, BrickStyle style) {
			IImageBrick tempBrick = new ImageBrick(style);
			SetCommonBrickProperties(tempBrick, info);
			return tempBrick;
		}
		protected StringTrimming ToStringTrimming(Trimming trimming) {
			switch(trimming) {
				case Trimming.Character: return StringTrimming.Character;
				case Trimming.Default: return StringTrimming.Character;
				case Trimming.EllipsisCharacter: return StringTrimming.EllipsisCharacter;
				case Trimming.EllipsisPath: return StringTrimming.EllipsisPath;
				case Trimming.EllipsisWord: return StringTrimming.EllipsisWord;
				case Trimming.None: return StringTrimming.None;
				case Trimming.Word: return StringTrimming.Word;
			}
			return StringTrimming.Character;
		}
		protected virtual BrickStyle CreateBrickStyle(PrintCellHelperInfo info, string type) {
			BrickStyle style = new BrickStyle();
			SetupCommonBrickStyleProperties(info, style);
			return style;
		}
		protected ITextBrick CreateTextBrick(PrintCellHelperInfo info) {
			BrickStyle style = CreateBrickStyle(info, "text");
			SetupTextBrickStyleProperties(info, style);
			TextBrick tb = new TextBrick(style);
			tb.Text = info.DisplayText;
			SetCommonBrickProperties(tb, info);
			ExportMode exportMode = GetExportMode();
			tb.XlsExportNativeFormat = (exportModeCore == ExportMode.Default ? DefaultBoolean.Default : (exportMode == ExportMode.DisplayText ? DefaultBoolean.False : DefaultBoolean.True));
			if (info.TextValueFormatString == String.Empty && DisplayFormat.FormatString != null && DisplayFormat.FormatString.Length > 0)
				tb.TextValueFormatString = "{0:" + DisplayFormat.FormatString + "}";
			else
				tb.TextValueFormatString = info.TextValueFormatString;
			return tb;
		}
		protected IPanelBrick CreatePanelBrick(PrintCellHelperInfo info, bool useTextBrick) {
			return CreatePanelBrick(info, useTextBrick, null);
		}
		protected virtual IPanelBrick CreatePanelBrick(PrintCellHelperInfo info, bool useTextBrick, BrickStyle style) {
			IPanelBrick pbrick = null;
			if(style == null)
				pbrick = useTextBrick ? new XETextPanelBrick() : new PanelBrick();
			else
				pbrick = useTextBrick ? new XETextPanelBrick(style) : new PanelBrick(style);
			SetCommonBrickProperties(pbrick, info);
			return pbrick;
		}
		protected virtual BaseEditViewInfo PreparePrintViewInfo(PrintCellHelperInfo info, bool inflateBorders) {
			BaseEditViewInfo vi;
			vi = CreateViewInfo();
			Rectangle rect = info.Rectangle;
			rect.Location = Point.Empty;
			if(inflateBorders) {
				rect.Inflate(info.PrintHLines ? -1 : 0, info.PrintVLines ? -1 : 0);
			}
			vi.EditValue = info.EditValue;
			vi.Bounds = rect;
			vi.DefaultBorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			vi.AllowDrawFocusRect = false;
			vi.FillBackground = vi.Item.AllowFocusedAppearance ? false : true;
			vi.DetailLevel = DevExpress.XtraEditors.Controls.DetailLevel.Minimum;
			vi.InplaceType = InplaceType.Grid;
			vi.CalcViewInfo(null);
			return vi;
		}
		protected PaddingInfo CalcPaddingInfo(RectangleF cellRect, RectangleF contentRect) {
			return new PaddingInfo(Math.Max(0, (int)(contentRect.Left - cellRect.Left)), Math.Max(0, (int)(cellRect.Right - contentRect.Right)), Math.Max(0, (int)(contentRect.Top - cellRect.Top)), Math.Max(0, (int)(cellRect.Bottom - contentRect.Bottom)));
		}
		protected Image GetCachedPrintImage(MultiKey key, IPrintingSystem PS) {
			Image img = PS.Images.GetImageByKey(key);
			return img;
		}
		protected Image AddImageToPrintCache(MultiKey key, Image image, IPrintingSystem PS) {
			return PS.Images.GetImage(key, image);
		}
		public virtual IVisualBrick GetBrick(PrintCellHelperInfo info) {
			IPanelBrick brick = new PanelBrick();
			brick.Rect = info.Rectangle;
			brick.BorderColor = info.LineColor;
			brick.BorderWidth = 1;
			brick.BorderStyle = BrickBorderStyle.Center;
			brick.Sides = DevExpress.XtraPrinting.BorderSide.All;
			brick.BackColor = info.Appearance.BackColor;
			return brick;
		}
		protected virtual ExportMode GetExportMode() {
			if(exportModeCore == ExportMode.Default) return ExportMode.Value;
			return exportModeCore;
		}
		private ExportMode exportModeCore = ExportMode.Default;
		[DefaultValue(ExportMode.Default),
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemExportMode"),
#endif
		DXCategory(CategoryName.Behavior)]
		public virtual ExportMode ExportMode {
			get { return exportModeCore; }
			set { exportModeCore = value; }
		}
		#endregion
		[Browsable(false), DefaultValue("")]
		public virtual string Name {
			get {
				if(Site == null) return name;
				return Site.Name;
			}
			set {
				if(Site != null) {
					Site.Name = value;
					name = Site.Name;
				}
				else {
					name = value;
				}
			}
		}
		protected virtual bool ShouldSerializeNullText() { return NullText != string.Empty; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemNullText"),
#endif
 Localizable(true)]
		public virtual string NullText {
			get { return fNullText; }
			set {
				if(value == null) value = string.Empty;
				if(NullText == value) return;
				fNullText = value;
				OnPropertiesChanged();
			}
		}
		[Browsable(false)]
		public virtual HorzAlignment DefaultAlignment { get { return HorzAlignment.Default; } }
		protected virtual IDictionary Links {
			get {
				if(_links == null)
					_links = new HybridDictionary();
				return _links;
			}
		}
		public virtual void Connect(object connector) {
			Links[connector] = 1;
		}
		public virtual void Disconnect(object connector) {
			if(Links.Contains(connector)) Links.Remove(connector);
		}
		[Browsable(false)]
		public virtual int LinkCount { get { return Links.Count; } }
		protected virtual void OnFormat_Changed(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected internal virtual bool NeededKeysContains(Keys key) {
			return false;
		}
		protected internal virtual bool ActivationKeysContains(Keys key) {
			return key == Keys.ProcessKey;
		}
		[Browsable(false)]
		public BaseEdit OwnerEdit { get { return _ownerEdit; } }
		protected internal void SetOwnerEdit(BaseEdit edit) {
			if(_ownerEdit != null) _ownerEdit.fProperties = null;
			this._ownerEdit = edit;
			if(OwnerEdit != null) {
				this._ownerEdit.fProperties = this;
				if(this._ownerEdit.ViewInfo != null)
					this._ownerEdit.ViewInfo.SetOwner(this);
				CreateLookAndFeel();
				OnOwnerEditChanged();
			}
		}
		protected virtual void OnOwnerEditChanged() { }
		bool ShouldSerializeLookAndFeel() { return StyleController == null && LookAndFeel != null && LookAndFeel.ShouldSerialize(); }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLookAndFeel"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual UserLookAndFeel LookAndFeel {
			get {
				if(StyleController != null && StyleController.LookAndFeel != null) return StyleController.LookAndFeel;
				if(lookAndFeel == null) return UserLookAndFeel.Default;
				return lookAndFeel;
			}
		}
		int lockFireChanged = 0;
		internal void LockFireChanged() {
			lockFireChanged++;
			if(OwnerEdit != null) OwnerEdit.LockFireChanged();
		}
		internal void UnlockFireChanged() {
			lockFireChanged--;
			if(OwnerEdit != null) OwnerEdit.UnlockFireChanged();
		}
		protected virtual void OnLookAndFeelChanged(object sender, EventArgs e) {
			LockFireChanged();
			try {
				if(OwnerEdit != null) {
					if(OwnerEdit.InplaceType != InplaceType.Standalone) return;
					OnPropertiesChanged();
					return;
				}
				if(LookAndFeel.UseDefaultLookAndFeel) return;
				OnPropertiesChanged();
			}
			finally {
				UnlockFireChanged();
			}
		}
		[Browsable(false)]
		public virtual string EditorTypeName { get { return "BaseEdit"; } }
		protected virtual EditorClassInfo EditorClassInfo { get { return EditorRegistrationInfo.Default.Editors[EditorTypeName]; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Type GetEditorType() {
			var ecInfo = EditorClassInfo;
			return (ecInfo != null) ? ecInfo.EditorType : typeof(TextEdit);
		}
		public virtual BaseEditViewInfo CreateViewInfo() {
			return EditorClassInfo.CreateViewInfo(this);
		}
		public virtual BaseEdit CreateEditor() {
			return EditorClassInfo.CreateEditor();
		}
		protected virtual RepositoryItem CreateRepositoryItem() {
			return EditorClassInfo.CreateRepositoryItem();
		}
		public virtual BaseEditorGroupRowPainter CreateGroupPainter() {
			return new BaseEditorGroupRowPainter();
		}
		public virtual BaseEditPainter CreatePainter() {
			return EditorClassInfo.Painter;
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool RequireDisplayTextSorting { get { return false; } }
		protected internal virtual BaseEditPainter Painter { get { return fPainter; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void LockEvents() {
			_lockEvents++;
		}
		[EditorBrowsable( EditorBrowsableState.Never)]
		public virtual void UnLockEvents() {
			_lockEvents--;
		}
		public virtual void BeginUpdate() {
			_lockUpdate++;
		}
		protected internal bool IsLockEvents { get { return _lockEvents != 0; } }
		[Browsable(false)]
		public virtual bool IsLockUpdate { get { return _lockUpdate != 0; } }
		[Browsable(false)]
		public virtual bool IsLoading { get { return _lockInit != 0; } }
		public virtual void EndUpdate() {
			if(--_lockUpdate == 0)
				OnPropertiesChanged();
		}
		protected internal bool IsFirstInit { get { return _isFirstInit; } }
		public virtual void CancelUpdate() {
			--_lockUpdate;
		}
		public virtual void BeginInit() {
			_lockInit++;
			this._isFirstInit = false;
			DisplayFormat.LockParse();
			EditFormat.LockParse();
		}
		public virtual void EndInit() {
			if(--_lockInit == 0) {
				DisplayFormat.UnlockParse();
				EditFormat.UnlockParse();
				OnLoaded();
			}
		}
		bool containerLoadedFired = false;
		protected bool IsContainerLoaded { get { return containerLoadedFired; } }
		protected internal virtual void FireContainerLoaded() {
			if(this.containerLoadedFired) return;
			this.containerLoadedFired = true;
			OnContainerLoaded();
		}
		protected virtual void OnContainerLoaded() {
		}
		protected virtual void OnLoaded() {
			if(OwnerEdit != null) OwnerEdit.OnLoaded();
		}
		protected void OnAppearancePaintChanged(object sender, EventArgs e) {
			if(IsLoading || IsLockUpdate) return;
			if(OwnerEdit != null) {
				FireChanged();
				OwnerEdit.OnAppearancePaintChanged();
				return;
			}
			OnPropertiesChanged();
		}
		protected void OnAppearanceSizeChanged(object sender, EventArgs e) {
			if(IsLoading || IsLockUpdate) return;
			OnPropertiesChanged();
		}
		protected void OnAppearanceChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnStyleChanged(object sender, EventArgs e) {
			OnPropertiesChanged();
		}
		protected virtual void OnAutoHeightChanged() {
			if(IsLoading || IsLockUpdate) return;
			if(OwnerEdit != null) OwnerEdit.OnAutoHeightChanged();
		}
		protected virtual void OnEnabledChanged() {
			if(OwnerEdit != null)
				OwnerEdit.CheckEnabled();
		}
		internal void LayoutChanged() {
			if(OwnerEdit != null) OwnerEdit.LayoutChanged();
		}
		protected virtual void OnPropertiesChanged() {
			OnPropertiesChanged(EventArgs.Empty);
		}
		protected virtual void OnPropertiesChanged(EventArgs pchea) {
			if(IsLoading || IsLockUpdate) return;
			RaisePropertiesChanged(pchea);
			if(OwnerEdit != null) OwnerEdit.OnPropertiesChanged(true);
		}
		protected internal virtual void FireChanged() {
			if(IsLoading || this.lockFireChanged != 0) return;
			if(Site == null) {
				if(OwnerEdit != null) OwnerEdit.FireChanged();
				return;
			}
			IComponentChangeService srv = Site.GetService(typeof(IComponentChangeService)) as IComponentChangeService;
			if(srv != null) srv.OnComponentChanged(this, null, null, null);
		}
		public virtual void ResetEvents() {
			Events.Dispose();
			if(OwnerEdit != null) OwnerEdit.ResetEvents();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public BorderStyles DefaultBorderStyleInGrid { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool AllowInplaceBorderPainter { get; set; }
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Point PopupOffset { get { return popupOffset; } set { popupOffset = value; } }
		public virtual void Assign(RepositoryItem item) {
			BeginUpdate();
			try {
				this.accessibleName = item.AccessibleName;
				this.accessibleDefaultActionDescription = item.AccessibleDefaultActionDescription;
				this.accessibleDescription = item.AccessibleDescription;
				this.accessibleRole = item.AccessibleRole;
				this.editValueChangedDelay = item.EditValueChangedDelay;
				this.popupOffset = item.popupOffset;
				this.htmlImages = item.HtmlImages;
				this._propertyStore = item.PropertyStore;
				this.fAllowFocused = item.AllowFocused;
				this.fAllowMouseWheel = item.AllowMouseWheel;
				this.fAutoHeight = item.AutoHeight;
				this.fBorderStyle = item.BorderStyle;
				this.ContextMenu = item.ContextMenu;
				this.allowHtmlDraw = item.AllowHtmlDraw;
#if DXWhidbey
				this.ContextMenuStrip = item.ContextMenuStrip;
#endif
				this.name = item.Name;
				this.fNullText = item.NullText;
				this.fReadOnly = item.ReadOnly;
				this.fEditValueChangedFiringMode = item.EditValueChangedFiringMode;
				this.Appearance.Assign(item.Appearance);
				this.AppearanceFocused.Assign(item.AppearanceFocused);
				this.AppearanceReadOnly.Assign(item.AppearanceReadOnly);
				this.AppearanceDisabled.Assign(item.AppearanceDisabled);
				this.DisplayFormat.Assign(item.DisplayFormat);
				this.EditFormat.Assign(item.EditFormat);
				this.LookAndFeel.Assign(item.LookAndFeel);
				this.tag = item.Tag;
				this.UseParentBackground = item.UseParentBackground;
				this.BestFitWidth = item.BestFitWidth;
				this.AllowInplaceBorderPainter = item.AllowInplaceBorderPainter;
				this.DefaultBorderStyleInGrid = item.DefaultBorderStyleInGrid;
				Events.AddHandler(queryAccessibilityHelp, item.Events[queryAccessibilityHelp]);
				Events.AddHandler(click, item.Events[click]);
				Events.AddHandler(doubleClick, item.Events[doubleClick]);
				Events.AddHandler(dragDrop, item.Events[dragDrop]);
				Events.AddHandler(dragEnter, item.Events[dragEnter]);
				Events.AddHandler(dragLeave, item.Events[dragLeave]);
				Events.AddHandler(dragOver, item.Events[dragOver]);
				Events.AddHandler(editValueChanged, item.Events[editValueChanged]);
				Events.AddHandler(editValueChanging, item.Events[editValueChanging]);
				Events.AddHandler(enter, item.Events[enter]);
				Events.AddHandler(giveFeedback, item.Events[giveFeedback]);
				Events.AddHandler(helpRequested, item.Events[helpRequested]);
				Events.AddHandler(keyDown, item.Events[keyDown]);
				Events.AddHandler(keyPress, item.Events[keyPress]);
				Events.AddHandler(keyUp, item.Events[keyUp]);
				Events.AddHandler(leave, item.Events[leave]);
				Events.AddHandler(modified, item.Events[modified]);
				Events.AddHandler(mouseDown, item.Events[mouseDown]);
				Events.AddHandler(mouseEnter, item.Events[mouseEnter]);
				Events.AddHandler(mouseHover, item.Events[mouseHover]);
				Events.AddHandler(mouseLeave, item.Events[mouseLeave]);
				Events.AddHandler(mouseMove, item.Events[mouseMove]);
				Events.AddHandler(mouseUp, item.Events[mouseUp]);
				Events.AddHandler(mouseWheel, item.Events[mouseWheel]);
				Events.AddHandler(queryContinueDrag, item.Events[queryContinueDrag]);
				Events.AddHandler(validating, item.Events[validating]);
				Events.AddHandler(parseEditValue, item.Events[parseEditValue]);
				Events.AddHandler(formatEditValue, item.Events[formatEditValue]);
				Events.AddHandler(customDisplayText, item.Events[customDisplayText]);
				Events.AddHandler(queryProcessKey, item.Events[queryProcessKey]);
				if(OwnerEdit != null) {
					OwnerEdit.AccessibleName = AccessibleName;
					OwnerEdit.AccessibleDescription = AccessibleDescription;
					OwnerEdit.AccessibleDefaultActionDescription = AccessibleDefaultActionDescription;
					OwnerEdit.AccessibleRole = AccessibleRole;
					OwnerEdit.Click += (EventHandler)Events[click];
					OwnerEdit.DoubleClick += (EventHandler)Events[doubleClick];
					OwnerEdit.DragDrop += (DragEventHandler)Events[dragDrop];
					OwnerEdit.DragEnter += (DragEventHandler)Events[dragEnter];
					OwnerEdit.DragLeave += (EventHandler)Events[dragLeave];
					OwnerEdit.DragOver += (DragEventHandler)Events[dragOver];
					OwnerEdit.Enter += (EventHandler)Events[enter];
					OwnerEdit.GiveFeedback += (GiveFeedbackEventHandler)Events[giveFeedback];
					OwnerEdit.HelpRequested += (HelpEventHandler)Events[helpRequested];
					OwnerEdit.KeyDown += (KeyEventHandler)Events[keyDown];
					OwnerEdit.KeyPress += (KeyPressEventHandler)Events[keyPress];
					OwnerEdit.KeyUp += (KeyEventHandler)Events[keyUp];
					OwnerEdit.Leave += (EventHandler)Events[leave];
					OwnerEdit.MouseDown += (MouseEventHandler)Events[mouseDown];
					OwnerEdit.MouseEnter += (EventHandler)Events[mouseEnter];
					OwnerEdit.MouseHover += (EventHandler)Events[mouseHover];
					OwnerEdit.MouseLeave += (EventHandler)Events[mouseLeave];
					OwnerEdit.MouseMove += (MouseEventHandler)Events[mouseMove];
					OwnerEdit.MouseUp += (MouseEventHandler)Events[mouseUp];
					OwnerEdit.MouseWheel += (MouseEventHandler)Events[mouseWheel];
					OwnerEdit.QueryContinueDrag += (QueryContinueDragEventHandler)Events[queryContinueDrag];
					OwnerEdit.Validating += (CancelEventHandler)Events[validating];
				}
			}
			finally {
				EndUpdate();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool AllowInplaceAutoFilter { get { return true; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsNonSortableEditor { get { return false; } }
		[DXCategory(CategoryName.Appearance), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAllowFocused"),
#endif
 DefaultValue(true)]
		public virtual bool AllowFocused {
			get { return fAllowFocused; }
			set {
				if(AllowFocused == value) return;
				fAllowFocused = value;
				OnAllowFocusedChanged();
			}
		}
		protected virtual void OnAllowFocusedChanged() {
			OnPropertiesChanged();
		}
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAllowMouseWheel"),
#endif
 DefaultValue(true),
		Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowMouseWheel {
			get { return fAllowMouseWheel; }
			set {
				if(AllowMouseWheel == value) return;
				fAllowMouseWheel = value;
				OnPropertiesChanged(new PropertyChangedEventArgs("AllowMouseWheel"));
			}
		}
		[Localizable(true), DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAutoHeight"),
#endif
 DefaultValue(true)]
		public virtual bool AutoHeight {
			get { return fAutoHeight; }
			set {
				if(AutoHeight == value) return;
				fAutoHeight = value;
				OnAutoHeightChanged();
				OnPropertiesChanged();
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool AllowFocusedAppearance { get { return true; } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemTag"),
#endif
 DefaultValue(null),
		DXCategory(CategoryName.Data), 
		Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(DevExpress.Utils.Editors.ObjectEditorTypeConverter))
		]
		public object Tag {
			get { return tag; }
			set { tag = value; }
		}
		bool ShouldSerializeReadOnly() { return ReadOnly != false; }
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemReadOnly")
#else
	Description("")
#endif
]
		public virtual bool ReadOnly {
			get { return fReadOnly; }
			set {
				if(ReadOnly == value) return;
				fReadOnly = value;
				OnPropertiesChanged(new PropertyChangedEventArgs("ReadOnly"));
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEditValueChangedFiringDelay"),
#endif
 DXCategory(CategoryName.Behavior), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static int EditValueChangedFiringDelay {
			get { return fEditValueChangedFiringDelay; }
			set {
				if(value < 1) value = 1;
				fEditValueChangedFiringDelay = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEditValueChangedDelay"),
#endif
 DefaultValue(0), DXCategory(CategoryName.Behavior)]
		public int EditValueChangedDelay {
			get { return editValueChangedDelay; }
			set {
				if(value < 0) value = 0;
				editValueChangedDelay = value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEditValueChangedFiringMode"),
#endif
 DXCategory(CategoryName.Behavior), DefaultValue(EditValueChangedFiringMode.Default)]
		public virtual EditValueChangedFiringMode EditValueChangedFiringMode {
			get { return fEditValueChangedFiringMode; }
			set {
				if(EditValueChangedFiringMode == value)
					return;
				CompleteChanges();
				fEditValueChangedFiringMode = value;
				OnPropertiesChanged();
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public virtual bool Enabled {
			get { return OwnerEdit == null ? fEnabled : OwnerEdit.Enabled; }
			set {
				if(OwnerEdit == null) {
					if(Enabled == value) return;
					fEnabled = value;
					OnPropertiesChanged();
				}
				else {
					OwnerEdit.Enabled = value;
					OnPropertiesChanged();
				}
			}
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual int BestFitWidth {
			get { return bestFitWidth; }
			set {
				if(bestFitWidth == value) return;
				bestFitWidth = value;
			}
		}
		[Browsable(false)]
		public virtual bool IsDesignMode {
			get {
				return (DesignMode || (OwnerEdit != null && OwnerEdit.IsDesignMode));
			}
		}
		bool ShouldSerializeBorderStyle() { return StyleController == null || BorderStyle != BorderStyles.Default; }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemBorderStyle"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(BorderStyles.Default)]
		public virtual BorderStyles BorderStyle {
			get {
				if(fBorderStyle == BorderStyles.Default && StyleController != null) return StyleController.BorderStyle;
				return fBorderStyle;
			}
			set {
				if(BorderStyle == value) return;
				fBorderStyle = value;
				OnPropertiesChanged();
			}
		}
#if DXWhidbey
		[Browsable(false)]
#endif
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemContextMenu"),
#endif
 DefaultValue(null)]
		public virtual ContextMenu ContextMenu {
			get { return _contextMenu; }
			set {
				if(ContextMenu == value) return;
				_contextMenu = value;
				if(OwnerEdit != null) OwnerEdit.ContextMenu = ContextMenu;
				OnPropertiesChanged();
			}
		}
#if DXWhidbey
		[DXCategory(CategoryName.Behavior), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemContextMenuStrip"),
#endif
 DefaultValue(null)]
		public virtual ContextMenuStrip ContextMenuStrip {
			get { return _contextMenuStrip; }
			set {
				if(ContextMenuStrip == value) return;
				_contextMenuStrip = value;
				if(OwnerEdit != null) OwnerEdit.ContextMenuStrip = ContextMenuStrip;
				OnPropertiesChanged();
			}
		}
#endif
		bool ShouldSerializeDisplayFormat() { return DisplayFormat.ShouldSerialize(); }
		[DXCategory(CategoryName.Format), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDisplayFormat"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual FormatInfo DisplayFormat {
			get { return displayFormat; }
		}
		bool ShouldSerializeEditFormat() { return EditFormat.ShouldSerialize(); }
		[DXCategory(CategoryName.Format), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEditFormat"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual FormatInfo EditFormat {
			get { return editFormat; }
		}
		void ResetAppearanceFocused() { AppearanceFocused.Reset(); }
		bool ShouldSerializeAppearanceFocused() { return AppearanceFocused.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearanceFocused"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceFocused { get { return appearanceFocused; } }
		void ResetAppearanceReadOnly() { AppearanceReadOnly.Reset(); }
		bool ShouldSerializeAppearanceReadOnly() { return AppearanceReadOnly.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearanceReadOnly"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceReadOnly { get { return appearanceReadOnly; } }
		void ResetAppearance() { Appearance.Reset(); }
		bool ShouldSerializeAppearance() { return Appearance.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearance"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject Appearance { get { return appearance; } }
		void ResetAppearanceDisabled() { AppearanceDisabled.Reset(); }
		bool ShouldSerializeAppearanceDisabled() { return AppearanceDisabled.ShouldSerialize(); }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemAppearanceDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content), DXCategory(CategoryName.Appearance)]
		public virtual AppearanceObject AppearanceDisabled {
			get { return appearanceDisabled; }
		}
		[DefaultValue(null), 
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHtmlImages"),
#endif
 DXCategory(CategoryName.Appearance)]
		public ImageCollection HtmlImages {
			get { return htmlImages; }
			set {
				if(htmlImages == value) return;
				if(htmlImages != null) htmlImages.Changed -=new EventHandler(OnHtmlImagesChanged);
				htmlImages = value;
				if(htmlImages != null) htmlImages.Changed += new EventHandler(OnHtmlImagesChanged);
				OnPropertiesChanged();
			}
		}
		void OnHtmlImagesChanged(object sender, EventArgs e) {
			if(GetAllowHtmlDraw()) {
				OnPropertiesChanged();
			}
		}
		public virtual bool IsActivateKey(Keys keyData) {
			return (ActivationKeysContains(keyData));
		}
		public virtual bool IsNeededKey(Keys keyData) {
			QueryProcessKeyEventArgs e = new QueryProcessKeyEventArgs(IsNeededKeyCore(keyData), keyData);
			RaiseQueryProcessKey(e);
			return e.IsNeededKey;
		}
		protected virtual bool IsNeededKeyCore(Keys keyData) { 
			return (NeededKeysContains(keyData) || NeededKeysContains(keyData & (~Keys.Modifiers)));
		}
		protected internal virtual bool IsNeededChar(char ch) {
			return false;
		}
		protected internal virtual bool IsUseDisplayFormat {
			get {
				if(OwnerEdit != null) {
					if(OwnerEdit.InplaceType != InplaceType.Standalone) return false;
					if(OwnerEdit.IsEditorActive) return false;
				}
				return true;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool IsFilterLookUp { get { return false; } }
		protected internal virtual bool LockFormatParse { get { return fLockFormatParse; } set { fLockFormatParse = value; } }
		protected virtual bool AllowFormatEditValue { get { return true; } }
		protected virtual bool AllowParseEditValue { get { return true; } }
		protected internal virtual FormatInfo ActiveFormat {
			get {
				if(IsUseDisplayFormat) return DisplayFormat;
				return EditFormat;
			}
		}
		protected internal virtual bool IsNullValue(object editValue) {
			if(editValue == null || editValue is DBNull) return true;
			return false;
		}
		protected internal virtual string GetNullEditText() {
			return string.Empty;
		}
		protected virtual bool LockDefaultImmediateUpdateRowPosition { get { return false; } }
		public bool IsLockDefaultImmediateUpdateRowPosition() { return LockDefaultImmediateUpdateRowPosition; }
		protected string GetNullText(FormatInfo format) {
			if(format == DisplayFormat || this.OwnerEdit == null) return NullText;
			return format == EditFormat ? GetNullEditText() : NullText;
		}
		public virtual string GetDisplayText(FormatInfo format, object editValue) {
			editValue = DoFormatEditValue(editValue).Value;
			string displayText = string.Empty;
			if(IsNullValue(editValue))
				displayText = GetNullText(format);
			else
				displayText = format.GetDisplayText(editValue);
			return GetNormalizedText(RaiseCustomDisplayText(format, editValue, displayText));
		}
		protected internal string RaiseCustomDisplayText(FormatInfo format, object editValue, string displayText) {
			CustomDisplayTextEventArgs e = new CustomDisplayTextEventArgs(editValue, displayText);
			if(format != EditFormat)
				RaiseCustomDisplayText(e);
			return e.DisplayText;
		}
		public virtual string GetDisplayText(object editValue) {
			return GetDisplayText(ActiveFormat, editValue);
		}
		protected virtual internal ConvertEditValueEventArgs DoParseEditValue(object val) {
			ConvertArgs.Initialize(val);
			if(AllowParseEditValue && !LockFormatParse) RaiseParseEditValue(ConvertArgs);
			return ConvertArgs;
		}
		protected virtual internal ConvertEditValueEventArgs DoFormatEditValue(object val) {
			ConvertArgs.Initialize(val);
			if(AllowFormatEditValue && !LockFormatParse) RaiseFormatEditValue(ConvertArgs);
			return ConvertArgs;
		}
		protected virtual IStyleController StyleController {
			get { return OwnerEdit != null ? OwnerEdit.StyleController : null; }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemUseParentBackground"),
#endif
 DXCategory(CategoryName.Appearance), DefaultValue(false), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool UseParentBackground {
			get { return useParentBackground; }
			set { useParentBackground = value; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual bool Editable { get { return true; } }
		internal event EventHandler RefreshRequired {
			add { this.Events.AddHandler(refreshRequired, value); }
			remove { this.Events.RemoveHandler(refreshRequired, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemPropertiesChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler PropertiesChanged {
			add { this.Events.AddHandler(propertiesChanged, value); }
			remove { this.Events.RemoveHandler(propertiesChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEditValueChanged"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler EditValueChanged {
			add { this.Events.AddHandler(editValueChanged, value); }
			remove { this.Events.RemoveHandler(editValueChanged, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemModified"),
#endif
 DXCategory(CategoryName.Events)]
		public event EventHandler Modified {
			add { this.Events.AddHandler(modified, value); }
			remove { this.Events.RemoveHandler(modified, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEditValueChanging"),
#endif
 DXCategory(CategoryName.Events)]
		public event ChangingEventHandler EditValueChanging {
			add { this.Events.AddHandler(editValueChanging, value); }
			remove { this.Events.RemoveHandler(editValueChanging, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemParseEditValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event ConvertEditValueEventHandler ParseEditValue {
			add { this.Events.AddHandler(parseEditValue, value); }
			remove { this.Events.RemoveHandler(parseEditValue, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemFormatEditValue"),
#endif
 DXCategory(CategoryName.Events)]
		public event ConvertEditValueEventHandler FormatEditValue {
			add { this.Events.AddHandler(formatEditValue, value); }
			remove { this.Events.RemoveHandler(formatEditValue, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemCustomDisplayText"),
#endif
 DXCategory(CategoryName.Events)]
		public event CustomDisplayTextEventHandler CustomDisplayText {
			add { this.Events.AddHandler(customDisplayText, value); }
			remove { this.Events.RemoveHandler(customDisplayText, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemQueryProcessKey"),
#endif
 DXCategory(CategoryName.Events)]
		public event QueryProcessKeyEventHandler QueryProcessKey {
			add { this.Events.AddHandler(queryProcessKey, value); }
			remove { this.Events.RemoveHandler(queryProcessKey, value); }
		}
		protected virtual object GetEventSender() {
			if(OwnerEdit != null) return OwnerEdit;
			return this;
		}
		protected internal virtual void RaiseEditValueChanging(ChangingEventArgs e) {
			if(IsLockEvents) return;
			ChangingEventHandler handler = (ChangingEventHandler)this.Events[editValueChanging];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaisePropertiesChanged(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[propertiesChanged];
			if(handler != null) handler(this, e);
		}
		protected internal virtual void RaiseRefreshRequired(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[refreshRequired];
			if(handler != null) handler(this, e);
		}
		System.Windows.Forms.Timer delayedEditValueChangedTimer = null;
		int insideCompleteChanges = 0;
		protected internal virtual void CompleteChanges() {
			if(OwnerEdit != null && OwnerEdit.IsModified) {
				OwnerEdit.FlushPendingEditActions();
			}
			if(delayedEditValueChangedTimer == null)
				return;
			delayedEditValueChangedTimer.Stop();
			delayedEditValueChangedTimer = null;
			insideCompleteChanges++;
			try {
				RaiseEditValueChanged(EventArgs.Empty);
			}
			finally {
				insideCompleteChanges--;
			}
		}
		protected virtual bool PostponeDelayedEditValueTimer { get { return false; } }
		void DelayedEditValueTimerFiredHandler(object sender, EventArgs e) {
			if(this.PostponeDelayedEditValueTimer)
				RestartDelayedTimer();
			else
				CompleteChanges();
		}
		protected internal virtual bool TriggerDelayedEditValueChanged() {	
			if(insideCompleteChanges != 0)
				return false;
			if(EditValueChangedFiringMode != EditValueChangedFiringMode.Buffered)
				return false;
			if(OwnerEdit == null)
				return false;
			if(!OwnerEdit.EditorContainsFocus)
				return false;
			RestartDelayedTimer();
			return true;
		}
		void RestartDelayedTimer() {
			if(delayedEditValueChangedTimer != null) {
				delayedEditValueChangedTimer.Stop();
			}
			else {
				delayedEditValueChangedTimer = new System.Windows.Forms.Timer();
				delayedEditValueChangedTimer.Tick += new EventHandler(DelayedEditValueTimerFiredHandler);
			}
			delayedEditValueChangedTimer.Interval = GetEditValueChangedDelay();
			delayedEditValueChangedTimer.Start();
		}
		int GetEditValueChangedDelay() {
			if(EditValueChangedDelay == 0) return EditValueChangedFiringDelay;
			return EditValueChangedDelay;
		}
		protected internal virtual void RaiseEditValueChanged(EventArgs e) {
			if(IsLockEvents)
				return;
			if(TriggerDelayedEditValueChanged())
				return;
			RaiseEditValueChangedCore(e);
		}
		protected virtual void RaiseEditValueChangedCore(EventArgs e) {
			EventHandler handler = (EventHandler)this.Events[editValueChanged];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseModified(EventArgs e) {
			if(IsLockEvents) return;
			EventHandler handler = (EventHandler)this.Events[modified];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseParseEditValue(ConvertEditValueEventArgs e) {
			ConvertEditValueEventHandler handler = (ConvertEditValueEventHandler)this.Events[parseEditValue];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseFormatEditValue(ConvertEditValueEventArgs e) {
			ConvertEditValueEventHandler handler = (ConvertEditValueEventHandler)this.Events[formatEditValue];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseCustomDisplayText(CustomDisplayTextEventArgs e) {
			CustomDisplayTextEventHandler handler = (CustomDisplayTextEventHandler)this.Events[customDisplayText];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal virtual void RaiseQueryProcessKey(QueryProcessKeyEventArgs e) {
			QueryProcessKeyEventHandler handler = (QueryProcessKeyEventHandler)this.Events[queryProcessKey];
			if(handler != null) handler(GetEventSender(), e);
		}
		protected internal bool HasEditValueChangingSubscribers { get { return Events[editValueChanging] != null; } }
		#region NativeEvents
		protected internal virtual bool RaiseQueryAccessibilityHelp(QueryAccessibilityHelpEventArgs e) {
			QueryAccessibilityHelpEventHandler handler = (QueryAccessibilityHelpEventHandler)this.Events[queryAccessibilityHelp];
			if(handler != null) {
				handler(GetEventSender(), e);
				return true;
			}
			return false;
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemQueryAccessibilityHelp"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event QueryAccessibilityHelpEventHandler QueryAccessibilityHelp {
			add { this.Events.AddHandler(queryAccessibilityHelp, value); }
			remove { this.Events.RemoveHandler(queryAccessibilityHelp, value); }
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemClick"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler Click {
			add {
				this.Events.AddHandler(click, value);
				if(OwnerEdit != null) OwnerEdit.Click += value;
			}
			remove {
				this.Events.RemoveHandler(click, value);
				if(OwnerEdit != null) OwnerEdit.Click -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDoubleClick"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler DoubleClick {
			add {
				this.Events.AddHandler(doubleClick, value);
				if(OwnerEdit != null) OwnerEdit.DoubleClick += value;
			}
			remove {
				this.Events.RemoveHandler(doubleClick, value);
				if(OwnerEdit != null) OwnerEdit.DoubleClick -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDragDrop"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event DragEventHandler DragDrop {
			add {
				this.Events.AddHandler(dragDrop, value);
				if(OwnerEdit != null) OwnerEdit.DragDrop += value;
			}
			remove {
				this.Events.RemoveHandler(dragDrop, value);
				if(OwnerEdit != null) OwnerEdit.DragDrop -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDragEnter"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event DragEventHandler DragEnter {
			add {
				this.Events.AddHandler(dragEnter, value);
				if(OwnerEdit != null) OwnerEdit.DragEnter += value;
			}
			remove {
				this.Events.RemoveHandler(dragEnter, value);
				if(OwnerEdit != null) OwnerEdit.DragEnter -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDragLeave"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler DragLeave {
			add {
				this.Events.AddHandler(dragLeave, value);
				if(OwnerEdit != null) OwnerEdit.DragLeave += value;
			}
			remove {
				this.Events.RemoveHandler(dragLeave, value);
				if(OwnerEdit != null) OwnerEdit.DragLeave -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemDragOver"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event DragEventHandler DragOver {
			add {
				this.Events.AddHandler(dragOver, value);
				if(OwnerEdit != null) OwnerEdit.DragOver += value;
			}
			remove {
				this.Events.RemoveHandler(dragOver, value);
				if(OwnerEdit != null) OwnerEdit.DragOver -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemEnter"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler Enter {
			add {
				this.Events.AddHandler(enter, value);
				if(OwnerEdit != null) OwnerEdit.Enter += value;
			}
			remove {
				this.Events.RemoveHandler(enter, value);
				if(OwnerEdit != null) OwnerEdit.Enter -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemGiveFeedback"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event GiveFeedbackEventHandler GiveFeedback {
			add {
				this.Events.AddHandler(giveFeedback, value);
				if(OwnerEdit != null) OwnerEdit.GiveFeedback += value;
			}
			remove {
				this.Events.RemoveHandler(giveFeedback, value);
				if(OwnerEdit != null) OwnerEdit.GiveFeedback -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemHelpRequested"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event HelpEventHandler HelpRequested {
			add {
				this.Events.AddHandler(helpRequested, value);
				if(OwnerEdit != null) OwnerEdit.HelpRequested += value;
			}
			remove {
				this.Events.RemoveHandler(helpRequested, value);
				if(OwnerEdit != null) OwnerEdit.HelpRequested -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemKeyDown"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event KeyEventHandler KeyDown {
			add {
				this.Events.AddHandler(keyDown, value);
				if(OwnerEdit != null) OwnerEdit.KeyDown += value;
			}
			remove {
				this.Events.RemoveHandler(keyDown, value);
				if(OwnerEdit != null) OwnerEdit.KeyDown -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemKeyPress"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event KeyPressEventHandler KeyPress {
			add {
				this.Events.AddHandler(keyPress, value);
				if(OwnerEdit != null) OwnerEdit.KeyPress += value;
			}
			remove {
				this.Events.RemoveHandler(keyPress, value);
				if(OwnerEdit != null) OwnerEdit.KeyPress -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemKeyUp"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event KeyEventHandler KeyUp {
			add {
				this.Events.AddHandler(keyUp, value);
				if(OwnerEdit != null) OwnerEdit.KeyUp += value;
			}
			remove {
				this.Events.RemoveHandler(keyUp, value);
				if(OwnerEdit != null) OwnerEdit.KeyUp -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemLeave"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler Leave {
			add {
				this.Events.AddHandler(leave, value);
				if(OwnerEdit != null) OwnerEdit.Leave += value;
			}
			remove {
				this.Events.RemoveHandler(leave, value);
				if(OwnerEdit != null) OwnerEdit.Leave -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseDown"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event MouseEventHandler MouseDown {
			add {
				this.Events.AddHandler(mouseDown, value);
				if(OwnerEdit != null) OwnerEdit.MouseDown += value;
			}
			remove {
				this.Events.RemoveHandler(mouseDown, value);
				if(OwnerEdit != null) OwnerEdit.MouseDown -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseEnter"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler MouseEnter {
			add {
				this.Events.AddHandler(mouseEnter, value);
				if(OwnerEdit != null) OwnerEdit.MouseEnter += value;
			}
			remove {
				this.Events.RemoveHandler(mouseEnter, value);
				if(OwnerEdit != null) OwnerEdit.MouseEnter -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseHover"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler MouseHover {
			add {
				this.Events.AddHandler(mouseHover, value);
				if(OwnerEdit != null) OwnerEdit.MouseHover += value;
			}
			remove {
				this.Events.RemoveHandler(mouseHover, value);
				if(OwnerEdit != null) OwnerEdit.MouseHover -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseLeave"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event EventHandler MouseLeave {
			add {
				this.Events.AddHandler(mouseLeave, value);
				if(OwnerEdit != null) OwnerEdit.MouseLeave += value;
			}
			remove {
				this.Events.RemoveHandler(mouseLeave, value);
				if(OwnerEdit != null) OwnerEdit.MouseLeave -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseMove"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event MouseEventHandler MouseMove {
			add {
				this.Events.AddHandler(mouseMove, value);
				if(OwnerEdit != null) OwnerEdit.MouseMove += value;
			}
			remove {
				this.Events.RemoveHandler(mouseMove, value);
				if(OwnerEdit != null) OwnerEdit.MouseMove -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseUp"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event MouseEventHandler MouseUp {
			add {
				this.Events.AddHandler(mouseUp, value);
				if(OwnerEdit != null) OwnerEdit.MouseUp += value;
			}
			remove {
				this.Events.RemoveHandler(mouseUp, value);
				if(OwnerEdit != null) OwnerEdit.MouseUp -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemMouseWheel"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event MouseEventHandler MouseWheel {
			add {
				this.Events.AddHandler(mouseWheel, value);
				if(OwnerEdit != null) OwnerEdit.MouseWheel += value;
			}
			remove {
				this.Events.RemoveHandler(mouseWheel, value);
				if(OwnerEdit != null) OwnerEdit.MouseWheel -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemQueryContinueDrag"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event QueryContinueDragEventHandler QueryContinueDrag {
			add {
				this.Events.AddHandler(queryContinueDrag, value);
				if(OwnerEdit != null) OwnerEdit.QueryContinueDrag += value;
			}
			remove {
				this.Events.RemoveHandler(queryContinueDrag, value);
				if(OwnerEdit != null) OwnerEdit.QueryContinueDrag -= value;
			}
		}
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("RepositoryItemValidating"),
#endif
 DXCategory(CategoryName.NativeEventsCategory)]
		public event CancelEventHandler Validating {
			add {
				this.Events.AddHandler(validating, value);
				if(OwnerEdit != null) OwnerEdit.Validating += value;
			}
			remove {
				this.Events.RemoveHandler(validating, value);
				if(OwnerEdit != null) OwnerEdit.Validating -= value;
			}
		}
		#endregion
		protected internal DevExpress.Data.Utils.AnnotationAttributes AnnotationAttributes {
			get;
			set;
		}
		IDesigner designer = null;
		bool requireFakeDesigner = false;
		protected virtual bool RequireFakeDesigner { get { return requireFakeDesigner || (OwnerEdit != null && OwnerEdit.IsDesignMode); } }
		internal void SetRequireFakeDesigner(bool value) {
			this.requireFakeDesigner = value;
		}
		RepositoryItem ownerItem = null;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public RepositoryItem OwnerItem { get { return ownerItem; } }
		internal void SetOwnerItem(RepositoryItem item) { this.ownerItem = item; }
		void CreateDesigner() {
			if(this.designer != null || !RequireFakeDesigner) return;
			this.designer = TypeDescriptor.CreateDesigner(this, typeof(IDesigner));
			if(designer != null)
				this.designer.Initialize(this);
		}
		protected virtual PropertyDescriptorCollection FilterProperties(PropertyDescriptorCollection collection) {
			if(RequireFakeDesigner) CreateDesigner();
			if(this.designer != null) {
				IDesignerFilter filter = designer as IDesignerFilter;
				if(filter != null) {
					PropertyDescriptor[] coll = new PropertyDescriptor[collection.Count];
					collection.CopyTo(coll, 0);
					collection = new PropertyDescriptorCollection(coll);
					filter.PreFilterProperties(collection);
					filter.PostFilterProperties(collection);
				}
			}
			return collection;
		}
		AttributeCollection FilterAttributes(AttributeCollection attrs) {
			if(!RequireFakeDesigner) return attrs;
			AttributeCollection coll = OwnerEdit == null && OwnerItem != null? TypeDescriptor.GetAttributes(OwnerItem) : TypeDescriptor.GetAttributes(OwnerEdit);
			Attribute attr = coll[typeof(InheritanceAttribute)];
			if(attr == null) return attrs;
			ArrayList aColl = new ArrayList(attrs);
			for(int n = aColl.Count - 1; n >= 0; n--) {
				Attribute a = aColl[n] as Attribute;
				if(a.GetType().Equals(typeof(InheritanceAttribute))) aColl.RemoveAt(n);
			}
			aColl.Add(attr);
			return new AttributeCollection(aColl.ToArray(typeof(Attribute)) as Attribute[]);
		}
		string ICustomTypeDescriptor.GetClassName() { return TypeDescriptor.GetClassName(this, true); }
		string ICustomTypeDescriptor.GetComponentName() { return TypeDescriptor.GetComponentName(this, true); }
		TypeConverter ICustomTypeDescriptor.GetConverter() { return TypeDescriptor.GetConverter(this, true); }
		EventDescriptor ICustomTypeDescriptor.GetDefaultEvent() { return TypeDescriptor.GetDefaultEvent(this, true); }
		PropertyDescriptor ICustomTypeDescriptor.GetDefaultProperty() { return TypeDescriptor.GetDefaultProperty(this, true); }
		object ICustomTypeDescriptor.GetEditor(Type editorBaseType) { return TypeDescriptor.GetEditor(this, editorBaseType, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents() { return TypeDescriptor.GetEvents(this, true); }
		EventDescriptorCollection ICustomTypeDescriptor.GetEvents(Attribute[] attributes) { return TypeDescriptor.GetEvents(this, attributes, true); }
		object ICustomTypeDescriptor.GetPropertyOwner(PropertyDescriptor pd) { return this; }
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties(Attribute[] attributes) {
			return FilterProperties(TypeDescriptor.GetProperties(this, attributes, true));
		}
		PropertyDescriptorCollection ICustomTypeDescriptor.GetProperties() {
			return FilterProperties(TypeDescriptor.GetProperties(this, true));
		}
		AttributeCollection ICustomTypeDescriptor.GetAttributes() {
			return FilterAttributes(TypeDescriptor.GetAttributes(this, true));
		}
		protected internal virtual AppearanceObject GetOverrideAppearance() { return null; }
	}
	public enum ExportMode { Default, DisplayText, Value }
}
