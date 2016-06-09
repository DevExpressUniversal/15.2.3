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
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Windows.Forms;
using DevExpress.Data.Browsing;
using DevExpress.Data.Design;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Serialization;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.Sparkline;
using DevExpress.Sparkline.Core;
using DevExpress.XtraPrinting.Native;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Utils.Design;
namespace DevExpress.XtraReports.UI {
	[
	ToolboxItem(true),
	XRToolboxSubcategoryAttribute(2, 2),
	ToolboxTabName(AssemblyInfo.DXTabReportControls),
	ToolboxBitmap(typeof(ResFinder), ControlConstants.BitmapPath + "XRSparkline.bmp"),
	Designer("DevExpress.XtraReports.Design._XRSparklineDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	XRDesigner("DevExpress.XtraReports.Design.XRSparklineDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRSparkline", "Sparkline"),
	DefaultProperty("View"),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRSparkline.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRSparkline.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRSparkline : XRControl, ISparklineSettings, ISparklineData, IDataContainer, IXtraSupportCreateContentPropertyValue {
		static readonly double[] defaultValues = new double[] { 
			56, 44, 39, 38, 41, 41, 40, 46, 36, 47, 35, 47, 41, 29, 44, 43, 31, 32, 23, 36,
			35, 32, 33, 47, 38, 48, 48, 40, 59, 63, 54, 47, 55, 59, 57, 40, 43, 47, 37, 39,
			40, 45, 40, 33, 34, 29, 33, 42, 43, 36, 34, 28, 32, 35, 43, 51, 42, 48, 60, 58, 
			62, 54, 66, 53, 54, 47, 50, 60, 52, 66, 63, 68, 61, 71, 60, 67, 66, 48, 56, 46, 
			46, 52, 38, 39, 42, 36, 28, 34, 32, 25, 38, 39, 32, 27, 26, 37, 32, 19, 34 };
		SparklineViewBase view;
		object dataSource;
		string dataMember = string.Empty;
		string valueMember = string.Empty;
		readonly SparklinePaintersCache paintersCache = new SparklinePaintersCache();
		readonly SparklineRange valueRange = new SparklineRange();
		#region hidden properties
		[
		Bindable(false), 
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string Text {
			get { return base.Text; }
			set { base.Text = value; }
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new Font Font {
			get { return base.Font; }
			set { base.Font = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override TextAlignment TextAlignment {
			get { return base.TextAlignment; }
			set { base.TextAlignment = value; }
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool WordWrap {
			get { return base.WordWrap; }
			set { base.WordWrap = value; }
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override bool KeepTogether {
			get { return base.KeepTogether; }
			set { base.KeepTogether = value; }
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override string XlsxFormatString { 
			get { return base.XlsxFormatString; }
			set { base.XlsxFormatString = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public override Color ForeColor {
			get { return base.ForeColor; }
			set { base.ForeColor = value; }
		}		
		#endregion
		object ParentDataSource { get { return Report != null ? Report.GetEffectiveDataSource() : null; } }
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparkline.DataMember"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		TypeConverter(typeof(DevExpress.XtraReports.Design.DataMemberTypeConverter)),
		Editor("DevExpress.XtraReports.Design.DataContainerDataMemberEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		XtraSerializableProperty
		]
		public string DataMember {
			get { return dataMember; }
			set { dataMember = string.IsNullOrEmpty(value) ? String.Empty : value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparkline.ValueMember"),
		SRCategory(ReportStringId.CatData),
		DefaultValue(""),
		Editor("DevExpress.XtraReports.Design.DataContainerFieldNameEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		XtraSerializableProperty
		]
		public string ValueMember {
			get { return valueMember; }
			set { valueMember = string.IsNullOrEmpty(value) ? String.Empty : value; }
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparkline.DataSource"),
		SRCategory(ReportStringId.CatData),
		RefreshProperties(RefreshProperties.Repaint),
		Editor("DevExpress.XtraReports.Design.DataSourceEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		TypeConverter(typeof(DataSourceConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference)
		]
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource != value) {
					dataSource = value;
				}
			}
		}
		bool ShouldSerializeDataSource() {
			return dataSource is IComponent && !object.ReferenceEquals(dataSource, ParentDataSource);
		}
		bool ShouldSerializeXmlDataSource() {
			return dataSource != null && !object.ReferenceEquals(dataSource, ParentDataSource);
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparkline.DataAdapter"),
		DefaultValue(null),
		SRCategory(ReportStringId.CatData),
		Editor("DevExpress.XtraReports.Design.DataAdapterEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(UITypeEditor)),
		TypeConverterAttribute(typeof(DevExpress.XtraReports.Design.DataAdapterConverter)),
		XtraSerializableProperty(XtraSerializationVisibility.Reference)
		]
		public object DataAdapter {
			get;
			set;
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparkline.View"),
		SRCategory(ReportStringId.CatAppearance),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraEditors.Design.SparklineViewEditor, " + AssemblyInfo.SRAssemblyEditors + AssemblyInfo.FullAssemblyVersionExtension, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Content, true, false, false, -1),
		]
		public SparklineViewBase View {
			get { return view; }
			set {
				if (value == null)
					throw new ArgumentNullException();
				if (view != value)
					SetView(value);
			}
		}
		[
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSparkline.ValueRange"),
		SRCategory(ReportStringId.CatData),
		TypeConverter(typeof(ExpandableObjectConverter)),
		Editor("DevExpress.XtraEditors.Design.SparklineViewEditor, " + AssemblyInfo.SRAssemblyEditors + AssemblyInfo.FullAssemblyVersionExtension, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty(XtraSerializationVisibility.Content, false, false, false, -1)
		]
		public SparklineRange ValueRange {
			get { return valueRange; }
		}
		#region Events
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public override event EventHandler TextChanged { add { } remove { } }
		#endregion
		public XRSparkline() {
			SetView(new LineSparklineView());
		}
		protected override XRControlScripts CreateScripts() {
			return new XRSparklineScripts(this);
		}
		protected internal override int DefaultWidth {
			get { return DefaultSizes.Sparkline.Width; }
		}
		protected internal override int DefaultHeight {
			get { return DefaultSizes.Sparkline.Height; }
		}
		protected override void AdjustDataSource() {
			base.AdjustDataSource();
			if(dataSource == ParentDataSource)
				dataSource = null;
		}
		protected override void CollectAssociatedComponents(DesignItemList components) {
			base.CollectAssociatedComponents(components);
			components.AddDataSource(((IDataContainer)this).GetSerializableDataSource() as IComponent);
			components.AddDataAdapter(DataAdapter);
		}
		protected internal override void CopyDataProperties(XRControl control) {
			XRSparkline sparkline = control as XRSparkline;
			if(sparkline != null) {
				DataSource = sparkline.DataSource;
				DataAdapter = sparkline.DataAdapter;
			}
			base.CopyDataProperties(control);
		}
		void SetView(SparklineViewBase view) {
			this.view = view;
			SparklineAppearanceHelper.SetSparklineAppearanceProvider(this.view, new XRSparklineAppearanceProvider());
		}
		IList<double> ISparklineData.Values {
			get {
				return DesignMode || Report == null ? defaultValues : GetValues(Report.DataContext);
			}
		}
		object IDataContainer.GetEffectiveDataSource() {
			return dataSource != null ? dataSource : ParentDataSource;
		}
		object IDataContainer.GetSerializableDataSource() {
			return DataSourceHelper.ConvertToSerializableDataSource(dataSource);
		}
		Padding ISparklineSettings.Padding { get { return new Padding(Padding.Left, Padding.Top, Padding.Right, Padding.Bottom); } }
		SparklineViewBase ISparklineSettings.View { get { return View; } }
		SparklineRange ISparklineSettings.ValueRange { get { return ValueRange; } }
		Image GetImage(Rectangle bounds) {
			if(bounds.Width == 0 || bounds.Height == 0)
				return null;
			Image image = null;
			using(Graphics graphics = Graphics.FromHwndInternal(IntPtr.Zero)) {
				image = graphics.CreateMetafile(bounds, MetafileFrameUnit.Pixel, EmfType.EmfPlusOnly);
			}
			if(image != null) {
				BaseSparklinePainter painter = paintersCache.GetPainter(((ISparklineSettings)this).View);
				painter.Initialize(this, this, bounds);
				using (Graphics graphics = Graphics.FromImage(image)) {
					painter.Draw(graphics, null);
				}
			}
			return image;
		}
		XRSparklineAppearanceProvider GetAppearanceProvider() {
			return new XRSparklineAppearanceProvider();
		}
		IList<double> GetValues(DataContext dataContext) {
			List<double> values = new List<double>();
			object dataSource = ((IDataContainer)this).GetEffectiveDataSource();
			if(dataSource == null) {
				TraceCenter.TraceErrorOnce(this, TraceSR.Format_InvalidPropertyValue, "DataSource");
				return values;
			}
			ListBrowser listBrowser = dataContext.GetDataBrowser(dataSource, DataMember, true) as ListBrowser;
			if(listBrowser == null) {
				TraceCenter.TraceErrorOnce(this, TraceSR.Format_InvalidPropertyValue, "DataMember");
				return values;
			}
			string valueMember = string.IsNullOrEmpty(DataMember) ? ValueMember : string.Join(".", new string[] { DataMember, ValueMember });
			DataBrowser valueBrowser = dataContext.GetDataBrowser(dataSource, valueMember, true);
			if(valueBrowser == null || (valueBrowser is ListBrowser && valueBrowser.GetItemProperties().Count > 0)) {
				TraceCenter.TraceErrorOnce(this, TraceSR.Format_InvalidPropertyValue, "ValueMember");
				return values;
			}
			for(int i = 0; i < listBrowser.Count; i++) {
				listBrowser.Position = i;
				object value = valueBrowser.Current;
				values.Add(ToDouble(valueBrowser.Current));
			}
			return values;
		}
		double ToDouble(object value) {
			value = IsEmptyValue(value) ? 0d : OracleConverter.Instance.Convert(value);
			return value.GetType() == typeof(double) ? (double)value : (double)Convert.ChangeType(value, typeof(double));
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return new ImageBrick(this);
		}
		protected internal override void PutStateToBrick(VisualBrick brick, PrintingSystemBase ps) {
			base.PutStateToBrick(brick, ps);
			ImageBrick imageBrick = (ImageBrick)brick;
			imageBrick.Padding = PaddingInfo.Empty;
			imageBrick.SizeMode = ImageSizeMode.Normal;
			RectangleF rect = DeflateBorderWidth(ClientRectangleF);
			rect = XRConvert.Convert(rect, Dpi, GraphicsDpi.Pixel);
			Rectangle bounds = new Rectangle(Point.Empty, Size.Round(rect.Size));
			imageBrick.Image = GetImage(bounds);
		}
		#region IXtraSupportCreateContentPropertyValue Members
		object IXtraSupportCreateContentPropertyValue.Create(XtraItemEventArgs e) {
			if(e.Item.Name == "View") {
				string value = (string)e.Item.ChildProperties["Type"].Value;
				SparklineViewType viewType = (SparklineViewType)Enum.Parse(typeof(SparklineViewType), value);
				return SparklineViewBase.CreateView(viewType);
			}
			return null;
		}
		#endregion
	}
}
namespace DevExpress.XtraReports.Native {
	public static class GraphicsExtension {
		public static Metafile CreateMetafile(this Graphics graphics, Rectangle bounds, MetafileFrameUnit unit, EmfType emfType) {
			IntPtr hdc = graphics.GetHdc();
			try {
				const int metafileBugFixAddition = 1;
				Rectangle meatafileBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width + metafileBugFixAddition, bounds.Height + metafileBugFixAddition);
				return new Metafile(hdc, meatafileBounds, unit, emfType);
			} finally {
				graphics.ReleaseHdc();
			}
		}
	}
	public class XRSparklineAppearanceProvider : ISparklineAppearanceProvider {
		public Color Color { get; private set; }
		public Color EndPointColor { get; private set; }
		public Color MarkerColor { get; private set; }
		public Color MaxPointColor { get; private set; }
		public Color MinPointColor { get; private set; }
		public Color NegativePointColor { get; private set; }
		public Color StartPointColor { get; private set; }
		public XRSparklineAppearanceProvider() {
			Color = Color.Red;
			EndPointColor = Color.Blue;
			MarkerColor = Color.Green;
			MaxPointColor = Color.Yellow;
			MinPointColor = Color.Magenta;
			NegativePointColor = Color.Brown;
			StartPointColor = Color.Beige;
		}
	}
}
