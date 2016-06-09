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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.TOC;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native.TOC;
using DevExpress.XtraReports.UserDesigner;
using System.Collections;
using DevExpress.XtraReports.Native.Printing;
using DevExpress.XtraReports.Native;
using DevExpress.DocumentView;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.XtraReports.Native.LayoutView;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.Design;
namespace DevExpress.XtraReports.UI {
	[XRDesigner("DevExpress.XtraReports.Design.XRTableOfContentsDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull)]
	[Designer("DevExpress.XtraReports.Design._XRTableOfContentsDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull)]
	[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTableOfContents", "TableOfContents")]
	[ToolboxItem(true)]
	[ToolboxTabName(AssemblyInfo.DXTabReportControls)]
	[ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "XRTableOfContents.bmp")]
	[ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItemFilter, ToolboxItemFilterType.Prevent)]
	[ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRTableOfContents.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull)]
	[ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRTableOfContents.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull)]
	[XRToolboxSubcategoryAttribute(3, 0)]
	[DefaultProperty("Levels")]
	public class XRTableOfContents : XRFieldEmbeddableControl {
		XtraReport realTocReport;
		IContentsReportGenerator contentsReportGenerator;
		XRTableOfContentsTitle levelTitle;
		XRTableOfContentsLevel levelDefault;
		XRTableOfContentsLevelCollection levels;
		TableOfContentsLineTextGenerator textGenerator;
		int maxNestedLevel = 0;
		int fakedPageIndex = -1;
		SubreportDocumentBand tocBand;
		PrintingSystemBase ps;
		public override bool CanHaveChildren {
			get {
				return false;
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override bool CanPublish { get { return true; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override HorizontalAnchorStyles AnchorHorizontal { get { return HorizontalAnchorStyles.None; } set { } }
		[
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContents.LevelTitle"),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
	   ]
		public XRTableOfContentsTitle LevelTitle {
			get {
				if(levelTitle == null) {
					levelTitle = new XRTableOfContentsTitle() { Text = "Title", Parent = this };
					levelTitle.HeightChanged += OnLevelHeightChanged;
				}
				return levelTitle;
			}
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContents.LevelDefault"),
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		]
		public XRTableOfContentsLevel LevelDefault {
			get {
				if(levelDefault == null) {
					levelDefault = new XRTableOfContentsLevel() { Parent = this };
					levelDefault.HeightChanged += OnLevelHeightChanged;
				}
				return levelDefault;
			}
		}
		[SRCategory(ReportStringId.CatBehavior),
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRTableOfContentsLevels"),
#endif
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContents.Levels"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor("DevExpress.XtraReports.Design.XRTableOfContentsLevelCollectionEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, -1, XtraSerializationFlags.Cached),
		]
		public XRTableOfContentsLevelCollection Levels {
			get {
				if(levels == null)
					levels = new XRTableOfContentsLevelCollection(this);
				return levels;
			}
		}
		bool ShouldSerializeLevels() {
			return levels != null && levels.Count > 0;
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		Browsable(false),
		]
		public override SizeF SizeF {
			get { return base.SizeF; }
			set { base.SizeF = value; }
		}
		bool TryGetWidth(out float result) {
			if(RootReport != null) {
				result = RootReport.GetClientSize().Width;
				return true;
			}
			result = 0;
			return false;
		}
		float GetHeight() {
			float height = 0;
			foreach(var item in AllLevels())
				height += item.Height;
			return height;
		}
		public override RectangleF BoundsF {
			get {
				RectangleF result = base.BoundsF;
				float width;
				if(TryGetWidth(out width))
					result.Width = width;
				result.Height = GetHeight();
				return result;
			}
			set { base.BoundsF = value; }
		}
		public override XRControl.XRControlStyles Styles {
			get { return new XRBandStyles(this); }
		}
		protected override BrickOwnerType BrickOwnerType {
			get { return BrickOwnerType.TableOfContents; }
		}
		protected internal override bool IsNavigateTarget {
			get { return false; }
		}
		[
		DefaultValue(0),
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContents.MaxNestingLevel"),
		]
		public int MaxNestingLevel {
			get { return maxNestedLevel; }
			set {
				if(value < 0)
					throw new ArgumentOutOfRangeException("MaxNestingLevel");
				maxNestedLevel = value;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IEnumerable<XRTableOfContentsLevelBase> AllLevels() {
			yield return LevelTitle;
			foreach(XRTableOfContentsLevel item in Levels)
				yield return item;
			yield return LevelDefault;
		}
		TableOfContentsLineTextGenerator TextGenerator {
			get { return textGenerator ?? (textGenerator = new TableOfContentsLineTextGenerator() { Measurer = Measurement.Measurer, GraphicsUnit = GraphicsUnit.Document }); }
		}
		internal void OnLevelHeightChanged(object sender, EventArgs e) {
			if(!ReportIsLoading)
				UpdateBandHeight();
		}
		public XRTableOfContents() {
			LocationF = new PointF(0, LocationF.Y);
		}
		protected override void WriteContentTo(XRWriteInfo writeInfo, VisualBrick brick) {
			if(writeInfo != null && brick is SubreportBrick) {
				tocBand = new TocDocumentBand(brick.Rect);
				ps = writeInfo.PrintingSystem;
				((SubreportBrick)brick).DocumentBand = tocBand;
				tocBand.Bands.Add(DocumentBand.CreatePageBreakBand(0f));
				DocumentBand detailBand = new DocumentBand(DocumentBandKind.Detail, 0);
				tocBand.Bands.Add(detailBand);
				tocBand.Bands.Add(DocumentBand.CreatePageBreakBand(0f));
				PanelBrick panelBrick = new PanelBrick(this);
				PutStateToBrick(panelBrick, writeInfo.PrintingSystem);
				VisualBrickHelper.InitializeBrick(panelBrick, writeInfo.PrintingSystem, new RectangleF(0, 0, 10, 10));
				detailBand.Bricks.Add(panelBrick);
				WriteContentToCore(writeInfo, brick);
				return;
			}
			base.WriteContentTo(writeInfo, brick);
		}
		protected internal override void OnBeforePrint(System.Drawing.Printing.PrintEventArgs e) {
			base.OnBeforePrint(e);
			if(tocBand != null) e.Cancel = true;
		}
		protected override Native.Presenters.ControlPresenter CreatePresenter() {
			return CreatePresenter<ControlPresenter>(
			   delegate() { return new TableOfContentsPresenter(this); },
			   delegate() { return new DesignTableOfContentsPresenter(this); },
			   delegate() { return new DesignTableOfContentsPresenter(this); }
			);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return CreatePresenter().CreateBrick(childrenBricks);
		}
		protected override bool RaisePrintOnPage(VisualBrick brick, int pageIndex, int pageCount) {
			fakedPageIndex = pageIndex;
			return true;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				DisposeTocReport();
			}
			base.Dispose(disposing);
		}
		protected override void SetBounds(float x, float y, float width, float height, BoundsSpecified specified) {
			if(specified.HasFlag(BoundsSpecified.Height) && HeightF != height) {
				float minHeight = XRConvert.Convert(XRTableOfContentsLevelBase.minHeight, GraphicsDpi.HundredthsOfAnInch, Dpi);
				LevelDefault.Height = Math.Max(minHeight, LevelDefault.Height + height - HeightF);
				height = GetHeight();
			}
			base.SetBounds(0f, y, WidthF, height, specified);
		}
		protected override object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Levels)
				return new XRTableOfContentsLevel();
			return base.CreateCollectionItem(propertyName, e);
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.Levels)
				Levels.Add(e.Item.Value as XRTableOfContentsLevel);
			else
				base.SetIndexCollectionItem(propertyName, e);
		}
		internal void ForEachLevel(Func<XRTableOfContentsLevelBase, RectangleF, bool> callback) {
			float y = 0;
			foreach(var level in AllLevels()) {
				RectangleF levelBounds = new RectangleF(0, y, WidthF, level.Height);
				if(callback(level, levelBounds))
					break;
				y += level.Height;
			}
		}
		#region TOC Report Generation
#if DEBUGTEST
		internal XtraReport Test_RealTocReport {
			get { return realTocReport; }
			set { realTocReport = value; }
		}
#endif // DEBUGTEST
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			DisposeTocReport();
			if(RootReport.MasterReport != null)
				throw new InvalidOperationException("The 'XRTableOfContents' control cannot be used in a subreport");
		}
		protected override void AfterReportPrint() {
			base.AfterReportPrint();
			if(this.RootReport.BrickPresentation != BrickPresentation.LayoutView)
				InsertTableOfContents(RootReport);
		}
		void InsertTableOfContents(XtraReport masterReport) {
			if(fakedPageIndex < 0)
				return;
			bool oldCanChangePageSettings = masterReport.Document.CanChangePageSettings;
			try {
				System.Diagnostics.Debug.Assert(tocBand != null);
				var page = masterReport.Pages[fakedPageIndex];
				masterReport.Pages.Remove(page);
				realTocReport = CreateTocReport(masterReport);
				realTocReport.MasterReport = masterReport;
				tocBand.Bands.Clear();
				SubreportBuilder builder = new SubreportBuilder(realTocReport, tocBand, true);
				tocBand.BandManager = builder;
				realTocReport.OnBeforePrint(new System.Drawing.Printing.PrintEventArgs());
				realTocReport.WriteToDocument(builder, ps);
				DocumentProxy documentProxy = new DocumentProxy();
				new PageBuildEngine(ps.PrintingDocument.Root, documentProxy).BuildPages(tocBand);
				InsertPages(documentProxy.Pages, masterReport.Document.Pages, fakedPageIndex);
				for(int i = 0; i < documentProxy.Pages.Count; i++)
					((Page)documentProxy.Pages[i]).AfterPrintOnPage(new List<int>(10), i + fakedPageIndex, masterReport.Pages.Count, brick => { });
			} finally {
				tocBand = null;
				fakedPageIndex = -1;
				masterReport.Document.CanChangePageSettings = oldCanChangePageSettings;
			}
		}
		protected internal XtraReport CreateTocReport(XtraReport sourceReport) {
			contentsReportGenerator = CreateContentsReportGenerator();
			var generationContext = CreateContentsReportGenerationContext(sourceReport);
			return contentsReportGenerator.GenerateContentsReport(generationContext);
		}
		protected virtual IContentsReportGenerator CreateContentsReportGenerator() {
			return new ContentsReportGenerator();
		}
		static void InsertPages(IList<Page> donor, IList<Page> acceptor, int pageOffset) {
			for(int contentsDocumentPageIndex = 0; contentsDocumentPageIndex < donor.Count; contentsDocumentPageIndex++) {
				int insertionIndex = pageOffset + contentsDocumentPageIndex;
				Page contentsPage = donor[contentsDocumentPageIndex];
				contentsPage.Owner = null; 
				acceptor.Insert(insertionIndex, contentsPage);
			}
		}
		void DisposeTocReport() {
			if(realTocReport != null) {
				realTocReport.Dispose();
				realTocReport = null;
			}
		}
		protected internal override void SyncDpi(float dpi) {
			foreach(var item in AllLevels()) {
				item.SyncDpi(Dpi, dpi);
			}
			base.SyncDpi(dpi);
		}
		ContentsReportGenerationContext CreateContentsReportGenerationContext(XtraReport sourceReport) {
			List<XRControlStyle> listStyles = new List<XRControlStyle>();
			List<float> listIndents = new List<float>();
			for(int i = 0; i < Levels.Count; i++) {
				listStyles.Add(GetLevelStyle(Levels[i]));
				listIndents.Add(Levels[i].Indent);
			}
			return new ContentsReportGenerationContext(GetLevelStyle) {
				Bookmarks = sourceReport.Document.BookmarkNodes,
				SourceReport = sourceReport,
				TitleLevel = LevelTitle,
				Levels = Levels,
				DefaultLevel = LevelDefault,
				TableOfContentsWidth = this.WidthF,
				DefaultStepIndent = XRConvert.Convert(XRTableOfContentsLevel.DefaultStepIndent, GraphicsDpi.HundredthsOfAnInch, Dpi),
				DefaultStyle = GetLevelStyle(LevelDefault),
				MaxNestingLevel = this.MaxNestingLevel
			};
		}
		internal XRControlStyle GetLevelStyle(XRTableOfContentsLevelBase level) {
			XRControlStyle style = new XRControlStyle();
			StyleProperty properties = StyleProperty.All;
			level.Style.ApplyProperties(style, level.Style.SetProperties, ref properties);
			CollectStyle(style, this, properties);
			Font validFont = style.FontInPoints.Validate(null);
			if(validFont != style.Font)
				style.SetBaseFont(validFont);
			style.StringFormat = BrickStringFormat.Create(style.TextAlignment, false);
			style.SyncDpi(Dpi);
			return style;
		}
		static void CollectStyle(XRControlStyle resultStyle, XRControl control, StyleProperty properties) {
			control.InnerStyle.ApplyProperties(resultStyle, control.InnerStyle.SetProperties, ref properties);
			if(properties != StyleProperty.None && control.Parent != null)
				CollectStyle(resultStyle, control.Parent, properties);
		}
		#endregion
	}
	[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTableOfContentsLevel", "XRTableOfContentsLevel")]
	public class XRTableOfContentsLevel : XRTableOfContentsLevelBase {
		const char defaultLeaderSymbol = '.';
		const float defaultIndent = 0f;
		Nullable<float> indent = null;
		char leaderSymbol;
		[
	   DefaultValue(defaultIndent),
	   SRCategory(ReportStringId.CatBehavior),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
	   DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevel.Indent"),
		XtraSerializableProperty,
	   ]
		public float Indent {
			get { return indent.HasValue ? indent.Value : 0; }
			set {
				if(value < 0)
					throw new ArgumentException("Indent");
				indent = value;
			}
		}
		internal float GetIndent(int depth) {
			return indent.HasValue ? indent.Value : XRConvert.Convert(DefaultStepIndent, GraphicsDpi.HundredthsOfAnInch, Dpi) * depth;
		}
		[
	   DefaultValue(defaultLeaderSymbol),
	   SRCategory(ReportStringId.CatAppearance),
	   DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
	   DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevel.LeaderSymbol"),
	   XtraSerializableProperty
	   ]
		public char LeaderSymbol {
			get { return leaderSymbol; }
			set {
				if(char.IsControl(value)) {
					throw new ArgumentException(ReportLocalizer.GetString(ReportStringId.Msg_InvalidLeaderSymbolForXrTocLevel));
				}
				leaderSymbol = value;
			}
		}
		public XRTableOfContentsLevel() {
			LeaderSymbol = '.';
		}
		internal override void SyncDpi(float fromDpi, float toDpi) {
			if(indent.HasValue)
				Indent = XRConvert.Convert(Indent, fromDpi, toDpi);
			base.SyncDpi(fromDpi, toDpi);
		}
	}
	[DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRTableOfContentsTitle", "XRTableOfContentsTitle")]
	public class XRTableOfContentsTitle : XRTableOfContentsLevelBase {
		[DefaultValue("")]
		[SRCategory(ReportStringId.CatAppearance)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsTitle.Text")]
		[XtraSerializableProperty]
		public string Text {
			get;
			set;
		}
		bool ShouldSerializeText() {
			try {
				DefaultValueAttribute defaultValueAttribute = TypeDescriptor.GetProperties(this)["Text"].Attributes[typeof(DefaultValueAttribute)] as DefaultValueAttribute;
				return defaultValueAttribute != null && !Comparer.Equals(defaultValueAttribute.Value, Text);
			} catch { return false; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsTitle.TextAlignment"),
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Editor("DevExpress.XtraReports.Design.TextAlignmentEditor," + AssemblyInfo.SRAssemblyUtilsUIFull, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		XtraSerializableProperty,
		]
		public TextAlignment TextAlignment {
			get { return Style.TextAlignment; }
			set { Style.TextAlignment = value; }
		}
		bool ShouldSerializeTextAlignment() {
			return Style.IsSetTextAlignment;
		}
		void ResetTextAlignment() {
			Style.ResetTextAlignment();
		}
	}
	[TypeConverter(typeof(XRTableOfContentsLevelBaseConverter))]
	public class XRTableOfContentsLevelBase {
		XRControlStyle style;
		internal const float DefaultStepIndent = 60f;
		const float defaultHeight = 23f;
		internal const float minHeight = 10f;
		float height;
		object parent;
		internal EventHandler<EventArgs> HeightChanged;
		void RaiseHeightChanged() {
			if(HeightChanged != null) HeightChanged(this, EventArgs.Empty);
		}
		internal object Parent {
			get { return parent; }
			set { parent = value; }
		}
		internal XRControlStyle Style {
			get { return style; }
		}
		[
		SRCategory(ReportStringId.CatBehavior),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.Height"),
		XtraSerializableProperty,
		]
		public float Height {
			get {
				return height;
			}
			set {
				if(value < 0)
					throw new ArgumentException("Height");
				value = Math.Max(XRConvert.Convert(minHeight, GraphicsDpi.HundredthsOfAnInch, Dpi), value);
				if(height != value) {
					height = value;
					RaiseHeightChanged();
				}
			}
		}
		[
		SRCategory(ReportStringId.CatAppearance),
		Editor("DevExpress.XtraReports.Design.XRFontEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		Localizable(true),
		TypeConverter(typeof(FontTypeConverter)),
		XtraSerializableProperty,
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.Font"),
		]
		public virtual Font Font {
			get { return style.Font.Validate(null); }
			set { style.Font = value.Validate(null); }
		}
		bool ShouldSerializeFont() {
			return style.IsSetFont;
		}
		void ResetFont() {
			style.ResetFont();
		}
		[
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Localizable(true),
		XtraSerializableProperty,
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.ForeColor"),
		]
		public Color ForeColor {
			get { return style.ForeColor; }
			set { style.ForeColor = value; }
		}
		bool ShouldSerializeForeColor() {
			return style.IsSetForeColor;
		}
		void ResetForeColor() {
			style.ResetForeColor();
		}
		[
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		Localizable(true),
		XtraSerializableProperty,
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.BackColor"),
		]
		public Color BackColor {
			get { return style.BackColor; }
			set { style.BackColor = value; }
		}
		bool ShouldSerializeBackColor() {
			return style.IsSetBackColor;
		}
		void ResetBackColor() {
			style.ResetBackColor();
		}
		[
		SRCategory(ReportStringId.CatAppearance),
		RefreshProperties(RefreshProperties.All),
		XtraSerializableProperty,
		DXDisplayNameAttribute(typeof(ResFinder), "DevExpress.XtraReports.UI.XRTableOfContentsLevelBase.Padding"),
		]
		public PaddingInfo Padding {
			get { return style.Padding; }
			set { style.Padding = value; }
		}
		bool ShouldSerializePadding() {
			return style.IsSetPadding;
		}
		void ResetPadding() {
			style.ResetPadding();
		}
		internal float Dpi {
			get { return style.Padding.Dpi; }
		}
		public XRTableOfContentsLevelBase() {
			style = new XRControlStyle();
			style.Padding = new PaddingInfo(0, 0, 0, 0, GraphicsDpi.HundredthsOfAnInch);
			height = XRConvert.Convert(defaultHeight, GraphicsDpi.HundredthsOfAnInch, Dpi);
		}
		internal virtual void SyncDpi(float fromDpi, float toDpi) {
			height = XRConvert.Convert(height, fromDpi, toDpi);
			style.SyncDpi(toDpi);
		}
	}
	public class XRTableOfContentsLevelCollection : Collection<XRTableOfContentsLevel> {
		XRTableOfContents owner;
		public XRTableOfContentsLevelCollection(XRTableOfContents owner) {
			this.owner = owner;
		}
		protected override void InsertItem(int index, XRTableOfContentsLevel item) {
			base.InsertItem(index, item);
			item.Parent = owner;
			if(!owner.ReportIsLoading) {
				item.SyncDpi(item.Dpi, owner.Dpi);
				owner.UpdateBandHeight();
			} 
			item.HeightChanged += owner.OnLevelHeightChanged;
		}
		protected override void RemoveItem(int index) {
			this[index].HeightChanged -= owner.OnLevelHeightChanged;
			base.RemoveItem(index);
		}
	}
}
