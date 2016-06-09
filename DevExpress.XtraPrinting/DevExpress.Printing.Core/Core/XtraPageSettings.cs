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
using System.ComponentModel;
using System.Xml;
using System.Reflection;
using System.Drawing;
using System.Runtime.InteropServices;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils;
using DevExpress.Utils.Design;
#if SL
using DevExpress.Xpf.Drawing.Printing;
#else
using System.Drawing.Printing;
using System.Windows.Forms;
using DevExpress.Utils.StoredObjects;
#endif
namespace DevExpress.XtraPrinting.Native {
	public class ReadonlyPageData : IXtraSupportShouldSerialize {
		#region static
		static MarginsF GetMaxValue(MarginsF m1, MarginsF m2) {
			return new MarginsF(Math.Max(m1.Left, m2.Left), Math.Max(m1.Right, m2.Right), Math.Max(m1.Top, m2.Top), Math.Max(m1.Bottom, m2.Bottom));
		}
		#endregion
		protected bool fLandscape;
		protected MarginsF fMinMargins;
		protected MarginsF fMargins;
		protected PaperKind fPaperKind;
		protected Size fSize;
		protected SizeF fPageSize;
		protected string fPaperName = "";
		public virtual string PaperName {
			get { return fPaperName; }
			set { throw new NotSupportedException(); }
		}
		[
		XtraSerializableProperty,
		DefaultValue(false),
		]
		public virtual bool Landscape {
			get { return fLandscape; }
			set { throw new NotSupportedException(); }
		}
		public virtual Margins MinMargins {
			get { return MarginsF.ToMargins(fMinMargins); }
			set { throw new NotImplementedException(); }
		}
		public virtual Margins Margins {
			get { return MarginsF.ToMargins(fMargins); }
			set { throw new NotImplementedException(); }
		}
		[XtraSerializableProperty]
		public virtual MarginsF MinMarginsF {
			get { return fMinMargins; }
			set { throw new NotImplementedException(); }
		}
		[XtraSerializableProperty]
		public virtual MarginsF MarginsF {
			get { return fMargins; }
			set { throw new NotImplementedException(); }
		}
		[
		XtraSerializableProperty,
		DefaultValue(XtraPageSettingsBase.DefaultPaperKind),
		]
		public virtual PaperKind PaperKind {
			get { return fPaperKind; }
			set { throw new NotImplementedException(); }
		}
		[XtraSerializableProperty]
		public virtual Size Size {
			get { return fSize; }
			set { throw new NotSupportedException(); }
		}
		public Rectangle Bounds { get { return fLandscape ? new Rectangle(0, 0, fSize.Height, fSize.Width) : new Rectangle(0, 0, fSize.Width, fSize.Height); } }
		public virtual SizeF PageSize { 
			get { 
				if(fPageSize.IsEmpty)
					return GraphicsUnitConverter.Convert(Bounds.Size, GraphicsDpi.HundredthsOfAnInch, GraphicsDpi.Document);
				return fLandscape ? new SizeF(fPageSize.Height, fPageSize.Width) : new SizeF(fPageSize.Width, fPageSize.Height);
			}
			set { throw new NotSupportedException(); }
		}
		public RectangleF UsefulPageRectF { get { return RectF.DeflateRect(new RectangleF(PointF.Empty, PageSize), MarginsF); } }
		public float UsefulPageWidth { get { return UsefulPageRectF.Width; } }
		public float UsefulPageHeight { get { return UsefulPageRectF.Height; } }
		public RectangleF PageHeaderRect {
			get { return new RectangleF(MarginsF.Left, MinMarginsF.Top, UsefulPageWidth, MarginsF.Top - MinMarginsF.Top); }
		}
		public RectangleF PageFooterRect {
			get { return new RectangleF(MarginsF.Left, PageSize.Height - MarginsF.Bottom, UsefulPageWidth, MarginsF.Bottom - MinMarginsF.Bottom); }
		}
		public ReadonlyPageData(Margins margins, Margins minMargins, PaperKind paperKind, Size pageSize, bool landscape)
			: this(new MarginsF(margins), new MarginsF(minMargins), paperKind, pageSize, landscape) {
		}
		public ReadonlyPageData(Margins margins, Margins minMargins, PaperKind paperKind, bool landscape)
			: this(margins, minMargins, paperKind, PageSizeInfo.GetPageSize(paperKind), landscape) {
		}
		public ReadonlyPageData(MarginsF margins, MarginsF minMargins, PaperKind paperKind, Size pageSize, bool landscape) {
			this.fMinMargins = minMargins;
			this.fMargins = GetMaxValue(margins, minMargins);
			this.fPaperKind = paperKind;
			this.fSize = pageSize;
			this.fLandscape = landscape;
		}
		public ReadonlyPageData(ReadonlyPageData source) {
			this.fMinMargins = source.fMinMargins;
			this.fMargins = source.fMargins;
			this.fPaperKind = source.fPaperKind;
			this.fSize = source.fSize;
			this.fPageSize = source.fPageSize;
			this.fLandscape = source.fLandscape;
		}
		public override bool Equals(object obj) {
			ReadonlyPageData other = obj as ReadonlyPageData;
			return other != null &&
				Landscape == other.Landscape &&
				object.Equals(MinMargins, other.MinMargins) &&
				object.Equals(Margins, other.Margins) &&
				PaperKind == other.PaperKind &&
				object.Equals(Size, other.Size) &&
				object.Equals(PaperName, other.PaperName);
		}
		public override int GetHashCode() {
			return HashCodeHelper.CalcHashCode(
				Convert.ToInt32(Landscape),
				MinMargins.GetHashCode(),
				Margins.GetHashCode(),
				(int)PaperKind,
				Size.GetHashCode(),
				PaperName.GetHashCode()
			);
		}
		public RectangleF GetMarginRect(RectangleF pageRect) {
			RectangleF rect = pageRect;
			rect.Width -= (MarginsF.Left + MarginsF.Right);
			rect.Height -= (MarginsF.Top + MarginsF.Bottom);
			rect.X += MarginsF.Left;
			rect.Y += MarginsF.Top;
			return rect;
		}
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			if(propertyName == PrintingSystemSerializationNames.MarginsF)
				return !object.Equals(MarginsF, new MarginsF(XtraPageSettingsBase.DefaultMargins));
			else if(propertyName == "Margins" || propertyName == "MinMargins")
				return false;
			return true;
		}
	}
	public class PageData : ReadonlyPageData {
		public static Size ToSize(PaperSize paperSize) {
			return new Size(paperSize.Width, paperSize.Height);
		}
		public override string PaperName { get { return base.PaperName; } set { fPaperName = value; } }
		[XtraSerializableProperty]
		public override bool Landscape { get { return base.Landscape; } set { fLandscape = value; } }
		[XtraSerializableProperty]
		public override Margins Margins { get { return base.Margins; } set { MarginsF = new MarginsF(value); } }
		[XtraSerializableProperty]
		public override Margins MinMargins { get { return base.MinMargins; } set { MinMarginsF = new MarginsF(value); } }
		[XtraSerializableProperty]
		public override MarginsF MarginsF { get { return base.MarginsF; } set { fMargins = value; } }
		[XtraSerializableProperty]
		public override MarginsF MinMarginsF { get { return base.MinMarginsF; } set { fMinMargins = value; } }
		[XtraSerializableProperty]
		public override PaperKind PaperKind { get { return base.PaperKind; } set { fPaperKind = value; } }
		[XtraSerializableProperty]
		public override Size Size { get { return base.Size; } set { fSize = value; } }
		public override SizeF PageSize { get { return base.PageSize; } set { fPageSize = value; } }
		public PageData()
			: this(XtraPageSettingsBase.DefaultMargins, XtraPageSettingsBase.DefaultMinMargins, XtraPageSettingsBase.DefaultPaperKind, new Size(850, 1100), false) {
		}
		public PageData(Margins margins, Margins minMargins, PaperSize paperSize, bool landscape)
			: this(margins, minMargins, paperSize.Kind, ToSize(paperSize), landscape) {
		}
		public PageData(Margins margins, Margins minMargins, PaperKind paperKind, Size pageSize, bool landscape)
			: base(margins, minMargins, paperKind, pageSize, landscape) {
		}
		public PageData(MarginsF margins, PaperKind paperKind, bool landscape)
			: base(margins, new MarginsF(0, 0, 0, 0), paperKind, PageSizeInfo.GetPageSize(paperKind), landscape) {
		}
		public PageData(ReadonlyPageData pageData)
			: base(pageData) {
		}
	}
	public class PageDataWithIndices : PageData {
		string pageIndices;
		[XtraSerializableProperty]
		public string PageIndices { get { return pageIndices; } set { pageIndices = value; } }
		public PageDataWithIndices() {
		}
		public PageDataWithIndices(ReadonlyPageData pageData, string indices)
			: base(pageData) {
			this.pageIndices = indices;
		}
	}
}
namespace DevExpress.XtraPrinting {
	using DevExpress.DocumentView;
	using System.IO;
	public class XtraPageSettingsBase : IPageSettings, IDisposable {
		SizeF IPageSettings.PageSize {
			get {
				return PageSize;
			}
		}
		#region static
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseDefaultMinMargins")]
#endif
		public static Margins DefaultMinMargins {
			get { return new Margins(20, 20, 20, 20); }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseDefaultMargins")]
#endif
		public static Margins DefaultMargins {
			get { return new Margins(100, 100, 100, 100); }
		}
		public const PaperKind DefaultPaperKind = PaperKind.Letter;
		internal readonly static string DefaultPaperName = "";
		public static bool ApplyPageSettings(XtraPageSettingsBase pageSettings, PaperKind paperKind, Size customPaperSize, Margins margins, Margins minMargins, bool landscape) {
			return ApplyPageSettings(pageSettings, paperKind, customPaperSize, margins, minMargins, landscape, DefaultPaperName);
		}
		public static bool ApplyPageSettings(XtraPageSettingsBase pageSettings, PaperKind paperKind, Size customPaperSize, Margins margins, Margins minMargins, bool landscape, string paperName) {
			if(paperKind == PaperKind.Custom && !customPaperSize.IsEmpty) {
				pageSettings.Assign(margins, minMargins, paperKind, customPaperSize, landscape, paperName);
				return true;
			}
			Size paperSize = PageSizeInfo.GetPageSize(paperKind, Size.Empty);
			if(!paperSize.IsEmpty) {
				pageSettings.Assign(margins, minMargins, paperKind, paperSize, landscape);
				return true;
			}
			return false;
		}
		#endregion
		#region Fields & Properties
		protected PrintingSystemBase ps;
		PageData data;
		string printerName;
		bool rollPaper;
		internal bool IsPresetted { get; set; }
		public string PaperName {
			get { return Data.PaperName; }
			set { Data.PaperName = value; }
		}
		[XtraSerializableProperty]
		public string PrinterName {
			get { return printerName; }
			set { printerName = value; }
		}
		[
		XtraSerializableProperty(XtraSerializationVisibility.Content),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public PageData Data {
			get {
				if(data == null)
					data = CreateData();
				return data;
			}
		}
		protected virtual PageData CreateData() {
			return new PageData();
		}
		internal MarginsF MinMarginsF { get { return Data.MinMarginsF; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseMarginsF")]
#endif
		public MarginsF MarginsF { get { return Data.MarginsF; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseUsablePageSize")]
#endif
		public SizeF UsablePageSize { get { return UsablePageRect.Size; } }
		public SizeF UsablePageSizeInPixels {
			get {
				return DevExpress.XtraPrinting.GraphicsUnitConverter.Convert(
					UsefulPageRectF.Size,
					DevExpress.XtraPrinting.GraphicsDpi.Document,
					DevExpress.XtraPrinting.GraphicsDpi.DeviceIndependentPixel);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseMargins")]
#endif
		public Margins Margins { get { return Data.Margins; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseMinMargins")]
#endif
		public Margins MinMargins { get { return Data.MinMargins; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseBounds")]
#endif
		public Rectangle Bounds { get { return Data.Bounds; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseLandscape")]
#endif
		public bool Landscape {
			get { return Data.Landscape; }
			set {
				Data.Landscape = value;
				ps.OnPageSettingsChanged();
			}
		}
		public bool RollPaper {
			get {
				return rollPaper;
			}
			set {
				rollPaper = value;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBasePaperKind")]
#endif
		public PaperKind PaperKind {
			get { return Data.PaperKind; }
			set {
				if(Data.PaperKind != value) {
					Size size = PageSizeInfo.GetPageSize(value);
					Assign(new PageData(Margins, MinMargins, value, size, Landscape));
				}
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseLeftMargin")]
#endif
		public int LeftMargin {
			get { return Margins.Left; }
			set {
				float prev = MarginsF.Left;
				float margin = ps.OnBeforeMarginsChange(MarginSide.Left, value);
				SetLeftMargin(MarginsF.FromHundredths(margin));
				if(prev != MarginsF.Left) ps.OnAfterMarginsChange(MarginSide.Left, margin);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseTopMargin")]
#endif
		public int TopMargin {
			get { return Margins.Top; }
			set {
				float prev = MarginsF.Top;
				float margin = ps.OnBeforeMarginsChange(MarginSide.Top, value);
				SetTopMargin(MarginsF.FromHundredths(margin));
				if(prev != MarginsF.Top) ps.OnAfterMarginsChange(MarginSide.Top, margin);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseRightMargin")]
#endif
		public int RightMargin {
			get { return Margins.Right; }
			set {
				float prev = MarginsF.Right;
				float margin = ps.OnBeforeMarginsChange(MarginSide.Right, value);
				SetRightMargin(MarginsF.FromHundredths(margin));
				if(prev != MarginsF.Right) ps.OnAfterMarginsChange(MarginSide.Right, margin);
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseBottomMargin")]
#endif
		public int BottomMargin {
			get { return Margins.Bottom; }
			set {
				float prev = MarginsF.Bottom;
				float margin = ps.OnBeforeMarginsChange(MarginSide.Bottom, value);
				SetBottomMargin(MarginsF.FromHundredths(margin));
				if(prev != MarginsF.Bottom) ps.OnAfterMarginsChange(MarginSide.Bottom, margin);
			}
		}
		protected float MinUsefulPageWidth { get { return Math.Min(ps.Document.MinPageWidth, MaxUsefulPageWidth); } }
		protected float MinUsefulPageHeight { get { return Math.Min(ps.Document.MinPageHeight, MaxUsefulPageHeight); } }
		internal RectangleF MaxUsefulPageRect {
			get { return RectangleF.FromLTRB(MinMarginsF.Left, MinMarginsF.Top, PageSize.Width - MinMarginsF.Right, PageSize.Height - MinMarginsF.Bottom); }
		}
		internal float MaxUsefulPageWidth { get { return MaxUsefulPageRect.Width; } }
		internal float MaxUsefulPageHeight { get { return MaxUsefulPageRect.Height; } }
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("XtraPageSettingsBaseUsablePageRect")]
#endif
		public RectangleF UsablePageRect {
			get { return RectangleF.FromLTRB(Margins.Left, Margins.Top, Bounds.Width - Margins.Right, Bounds.Height - Margins.Bottom); }
		}
		internal RectangleF UsefulPageRectF { get { return Data.UsefulPageRectF; } }
		internal float UsefulPageWidth { get { return Data.UsefulPageWidth; } }
		internal float UsefulPageHeight { get { return Data.UsefulPageHeight; } }
		internal SizeF PageSize { get { return Data.PageSize; } }
		internal RectangleF PageHeaderRect { get { return Data.PageHeaderRect; } }
		internal RectangleF PageFooterRect { get { return Data.PageFooterRect ; }
		}
		#endregion
		internal XtraPageSettingsBase(PrintingSystemBase ps) {
			this.ps = ps;
		}
		public virtual void Dispose() {
		}
		#region Save & Restore
		public void SaveToXml(string xmlFile) {
			SaveCore(new XmlXtraSerializer(), xmlFile);
		}
		public void RestoreFromXml(string xmlFile) {
			RestoreCore(new XmlXtraSerializer(), xmlFile);
		}
		public void SaveToRegistry(string path) {
			SaveCore(new RegistryXtraSerializer(), path);
		}
		public void RestoreFromRegistry(string path) {
			RestoreCore(new RegistryXtraSerializer(), path);
		}
		public void SaveToStream(Stream stream) {
			SaveCore(new XmlXtraSerializer(), stream);
		}
		public void RestoreFromStream(Stream stream) {
			RestoreCore(new XmlXtraSerializer(), stream);
		}
		void SaveCore(XtraSerializer serializer, object path) {
			serializer.SerializeObject(this, path, "XtraPrintingPageSettings");
		}
		void RestoreCore(XtraSerializer serializer, object path) {
			serializer.DeserializeObject(this, path, "XtraPrintingPageSettings");
			IsPresetted = true;
		}
		#endregion
		public void Assign(PageSettings pageSettings, Margins minMargins) {
			try {
				PageData pageData = CreatePageData(pageSettings, minMargins);
				Assign(pageData);
			} catch {
			}
		}
		protected static PageData CreatePageData(PageSettings sets, Margins minMargins) {
			return new PageData(sets.Margins, minMargins, sets.PaperSize.Kind, PageData.ToSize(sets.PaperSize), sets.Landscape);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void AssignPrinterSettings(string printerName, string paperName, PrinterSettingsUsing settingsUsing) {
			ps.Extender.AssignPrinterSettings(printerName, paperName, settingsUsing);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void Assign(Margins margins, PaperKind paperKind, string paperName, bool landscape) {
			ps.Extender.Assign(margins, paperKind, paperName, landscape);
		}
		public virtual void Assign(Margins margins, PaperKind paperKind, bool landscape) {
			try {
				Assign(new PageData(margins, new Margins(0, 0, 0, 0), paperKind, PageSizeInfo.GetPageSize(paperKind), landscape));
			} catch {
			}
		}
		public void Assign(Margins margins, PaperKind paperKind, Size paperSize, bool landscape) {
			try {
				Assign(new PageData(margins, new Margins(0, 0, 0, 0), paperKind, paperSize, landscape));
			} catch {
			}
		}
		public void AssignDefaultPageSettings() {
			Assign(DefaultMargins, new Margins(0, 0, 0, 0), DefaultPaperKind, PageSizeInfo.GetPageSize(DefaultPaperKind), false);
		}
		public void Assign(Margins margins, Margins minMargins, PaperKind paperKind, Size pageSize, bool landscape) {
			try {
				Assign(new PageData(margins, minMargins, paperKind, pageSize, landscape));
			} catch {
			}
		}
		public void Assign(Margins margins, Margins minMargins, PaperKind paperKind, Size pageSize, bool landscape, string paperName) {
			try {
				PageData pageData = new PageData(margins, minMargins, paperKind, pageSize, landscape);
				pageData.PaperName = paperName;
				Assign(pageData);
			} catch {
			}
		}
		public void Assign(MarginsF margins, PaperKind paperKind, SizeF pageSize, bool landscape) {
			try {
				SizeF sizeInHundredsOfInch = GraphicsUnitConverter.Convert(pageSize, GraphicsDpi.Document, GraphicsDpi.HundredthsOfAnInch);
				Size size = new Size(ConvertSizeValue(sizeInHundredsOfInch.Width), ConvertSizeValue(sizeInHundredsOfInch.Height));
				Assign(new PageData(margins, paperKind, landscape) { Size = size, PageSize = pageSize });
			} catch {
			}
		}
		int ConvertSizeValue(float value) {
#if !SL
			return (int)Math.Round(value, MidpointRounding.AwayFromZero);
#else
			return (int)Math.Round(value);
#endif
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void Assign(PageData val) {
			data = val;
			ps.OnPageSettingsChanged();
		}
		internal void SetMarginsInHundredthsOfInch(MarginsF marginsInHundredthsOfInch) {
			MarginsF prev = (MarginsF)MarginsF.Clone();
			ps.OnBeforeMarginsChange(MarginSide.All, 0);
			SetBottomMargin(marginsInHundredthsOfInch.Bottom);
			SetTopMargin(marginsInHundredthsOfInch.Top);
			SetLeftMargin(marginsInHundredthsOfInch.Left);
			SetRightMargin(marginsInHundredthsOfInch.Right);
			if(!object.Equals(prev, MarginsF)) {
				ps.OnAfterMarginsChange(MarginSide.All, 0);
			}
		}
		private void SetLeftMargin(float value) {
			MarginsF.Left = GetValidLeftMargin(MarginsF.Right, value, MinMarginsF.Left);
		}
		private void SetTopMargin(float value) {
			MarginsF.Top = GetValidTopMargin(MarginsF.Bottom, value, MinMarginsF.Top);
		}
		private void SetRightMargin(float value) {
			MarginsF.Right = GetValidRightMargin(MarginsF.Left, value, MinMarginsF.Right);
		}
		private void SetBottomMargin(float value) {
			MarginsF.Bottom = GetValidBottomMargin(MarginsF.Top, value, MinMarginsF.Bottom);
		}
		private float GetValidLeftMargin(float rightMargin, float leftMargin, float minLeftMargin) {
			float maxMargin = PageSize.Width - rightMargin - MinUsefulPageWidth;
			return GetValidValue(leftMargin, minLeftMargin, maxMargin);
		}
		private float GetValidRightMargin(float leftMargin, float rightMargin, float minRightMargin) {
			float maxMargin = PageSize.Width - leftMargin - MinUsefulPageWidth;
			return GetValidValue(rightMargin, minRightMargin, maxMargin);
		}
		private float GetValidBottomMargin(float topMargin, float bottomMargin, float minBottomMargin) {
			float maxMargin = PageSize.Height - topMargin - MinUsefulPageHeight;
			float minMargin = minBottomMargin + ps.Document.PageFootBounds.Bottom;
			return GetValidValue(bottomMargin, minMargin, maxMargin);
		}
		private float GetValidTopMargin(float bottomMargin, float topMargin, float minTopMargin) {
			float maxMargin = PageSize.Height - bottomMargin - MinUsefulPageHeight;
			float minMargin = minTopMargin + ps.Document.PageHeadBounds.Bottom;
			return GetValidValue(topMargin, minMargin, maxMargin);
		}
		private float GetValidValue(float value, float min, float max) {
			max = Math.Max(min, max);
			return Math.Min(max, Math.Max(min, value));
		}
		internal void SetPaperSize(PaperSize paperSize) {
			PageData data = new PageData(Margins, MinMargins, paperSize, Landscape);
			data.PaperName = paperSize.PaperName;
			Assign(data);
		}
	}
}
