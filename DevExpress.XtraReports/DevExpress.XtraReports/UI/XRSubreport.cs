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

using System.ComponentModel;
using System.Drawing;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.Native;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraReports.Localization;
using DevExpress.XtraReports.UserDesigner;
using System.IO;
using DevExpress.Utils.Serializing;
using System;
using DevExpress.XtraReports.Native.LayoutView;
using DevExpress.XtraReports.Native.Presenters;
using DevExpress.Utils.Design;
using DevExpress.Data;
using DevExpress.XtraReports.Serialization;
using DevExpress.Utils.Serializing.Helpers;
namespace DevExpress.XtraReports.UI {
	[
	XRDesigner("DevExpress.XtraReports.Design.XRSubreportDesigner," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	Designer("DevExpress.XtraReports.Design._XRSubreportDesigner," + AssemblyInfo.SRAssemblyReportsDesignFull),
	ToolboxBitmap(typeof(ResFinder), DevExpress.Utils.ControlConstants.BitmapPath + "XRSubreport.bmp"),
	DevExpress.Utils.ToolboxTabName(AssemblyInfo.DXTabReportControls),
	DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraReports.UI.XRSubreport", "SubReport"),
	ToolboxItemFilter(DevExpress.XtraReports.Design.AttributeSR.SchedulerToolboxItemFilter, ToolboxItemFilterType.Prevent),
	ToolboxItem(true),
	XRToolboxSubcategoryAttribute(2, 4),
	System.ComponentModel.Design.Serialization.DesignerSerializer(
		"DevExpress.XtraReports.Design.XRSubreportCodeDomSerializer, " + AssemblyInfo.SRAssemblyReportsExtensionsFull,
		AttributeConstants.CodeDomSerializer
	),
	ToolboxBitmap24("DevExpress.XtraReports.Images.Toolbox24x24.XRSubreport.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	ToolboxBitmap32("DevExpress.XtraReports.Images.Toolbox32x32.XRSubreport.png," + AssemblyInfo.SRAssemblyReportsExtensionsFull),
	]
	public class XRSubreport : SubreportBase {
		string reportSourceUrl = string.Empty;
		ParameterBindingCollection parameterBindings;
		[
		DefaultValue(""),
		SRCategory(ReportStringId.CatData),
		DXDisplayName(typeof(ResFinder), "DevExpress.XtraReports.UI.XRSubreport.ReportSourceUrl"),
		Editor("DevExpress.XtraReports.Design.ReportUrlEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor)),
		XtraSerializableProperty,
		]
		public string ReportSourceUrl {
			get { return reportSourceUrl; }
			set { reportSourceUrl = value; }
		}
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override VerticalAnchorStyles AnchorVertical { get { return VerticalAnchorStyles.None; } set { } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		XtraSerializableProperty(XtraSerializationVisibility.Hidden),
		]
		public override HorizontalAnchorStyles AnchorHorizontal { get { return HorizontalAnchorStyles.None; } set { } }
		[
#if !SL
	DevExpressXtraReportsLocalizedDescription("XRSubreportCanShrink"),
#endif
		Browsable(true),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public override bool CanShrink {
			get { return base.CanShrink; }
			set { base.CanShrink = value; }
		}
		[Browsable(false),
		DefaultValue(0),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible),
		XtraSerializableProperty,
		]
		public int Id { get; set; }
		[DXDisplayName(typeof(DevExpress.XtraReports.ResFinder), "DevExpress.XtraReports.UI.XRSubreport.ParameterBindings")]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[SRCategory(ReportStringId.CatData)]
		[Editor("DevExpress.XtraReports.Design.ParameterBindingCollectionEditor," + AssemblyInfo.SRAssemblyReportsExtensionsFull, typeof(System.Drawing.Design.UITypeEditor))]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, false, 0, XtraSerializationFlags.Cached)]
		public ParameterBindingCollection ParameterBindings { get { return parameterBindings; } }
		public XRSubreport() {
			parameterBindings = new ParameterBindingCollection(this);
		}
		internal protected override int GetMinimumHeight() {
			return 0;
		}
		protected override ControlPresenter CreatePresenter() {
			return CreatePresenter<ControlPresenter>(
				delegate() { return new SubreportPresenter(this); },
				delegate() { return new DesignSubreportPresenter(this); },
				delegate() { return new LayoutViewSubreportPresenter(this); }
			);
		}
		protected override VisualBrick CreateBrick(VisualBrick[] childrenBricks) {
			return CreatePresenter().CreateBrick(childrenBricks);
		}
		internal override IComponent[] GetNonSerializableComponents() {
			return !string.IsNullOrEmpty(ReportSourceUrl) && ReportSource != null ?
				new IComponent[] { ReportSource } :
				new IComponent[] { };
		}
		protected override void BeforeReportPrint() {
			base.BeforeReportPrint();
			ForceReportSource();
		}
		protected internal override void ForceReportSource() {
			if(!string.IsNullOrEmpty(ReportSourceUrl) && (ReportSource == null || !string.Equals(ReportSourceUrl, ReportSource.SourceUrl))) {
				byte[] layout = ReportStorageService.GetData(ReportSourceUrl);
				if(layout == null)
					layout = ReportStorageService.GetData(Id.ToString());
				using(Stream stream = new MemoryStream(layout)) {
					if(ReportSource == null)
						ReportSource = XtraReport.FromStream(stream, true);
					else
						ReportSource.LoadLayout(stream);
				}
				foreach(XtraReport item in RootReport.MasterReport.AllReports()) {
					if(string.Equals(ReportSourceUrl, item.SourceUrl))
						ThrowInvalidReportSourceException();
				}
				ReportSource.SourceUrl = ReportSourceUrl;
			}
		}
		bool ShouldSerializeParameterBindings() {
			return parameterBindings.Count != 0;
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(ReportSource != null) {
					ReportSource.PrintingSystem.ClearContent();
					ReportSource.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		protected override float CalculateBrickHeight(VisualBrick brick) {
			return this.CanShrink ? 0 : GetBrickBounds(brick).Height;
		}
		protected override SelfGeneratedSubreportDocumentBand CreateDocumentBand() {
			float minHeight = this.CanShrink ? 0f :
				XRConvert.Convert(this.HeightF, this.Dpi, GraphicsDpi.Document);
			return new SelfGeneratedSubreportDocumentBand(ReportSource, minHeight, this);
		}
		protected override BrickOwnerType BrickOwnerType {
			get { return XtraPrinting.BrickOwnerType.Subreport; }
		}
		protected override void AdjustDataSource() {
			foreach(ParameterBinding item in ParameterBindings)
				item.AdjustDataSourse();
			base.AdjustDataSource();
		}
		protected internal override void OnBeforePrint(System.Drawing.Printing.PrintEventArgs e) {
			base.OnBeforePrint(e);
			ApplyParameterBindings();
		}
		public void ApplyParameterBindings() {
			if(ReportSource == null)
				return;
			foreach(var binding in parameterBindings) {
				if(string.IsNullOrEmpty(binding.ParameterName) || binding.IsEmpty)
					continue;
				IParameter parameter = ReportSource.Parameters.GetByName(binding.ParameterName);
				if(parameter != null) {
					try {
						parameter.Value = Convert.ChangeType(GetColumnValue(binding), parameter.Type);
					} catch(FormatException exception) {
						throw new Exception(string.Format(ReportLocalizer.GetString(ReportStringId.Msg_ParameterBindingValueTypeMismatch), parameter.Name), exception);
					}
				}
			}
		}
		XRBinding GetXRBinding(ParameterBinding binding) {
			if(binding.Parameter != null)
				return new XRBinding(binding.Parameter, "Text", "");
			return new XRBinding("Text", binding.DataSource, binding.DataMember);
		}
		object GetColumnValue(ParameterBinding binding) {
			XRBinding xrBinding = GetXRBinding(binding);
			this.DataBindings.Add(xrBinding);
			try {
				return xrBinding.GetUnconvertedColumnValue(Report.DataContext, RootReport.PrintingSystem.Images);
			} finally {
				this.DataBindings.Remove(xrBinding);
			}
		}
		protected override void SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
			base.SetIndexCollectionItem(propertyName, e);
			if(propertyName == XtraReportsSerializationNames.ParameterBindings)
				ParameterBindings.Add(e.Item.Value as ParameterBinding);
		}
		protected override object CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == XtraReportsSerializationNames.ParameterBindings)
				return CreateParameterBinding(e.Item.ChildProperties);
			return base.CreateCollectionItem(propertyName, e);
		}
		ParameterBinding CreateParameterBinding(IXtraPropertyCollection properites) {
			string parameterName = properites["ParameterName"] != null ? properites["ParameterName"].Value as string : string.Empty;
			string dataMember = properites["DataMember"] != null ? properites["DataMember"].Value as string : string.Empty;
			return new ParameterBinding(parameterName, null, dataMember);
		}
		public void FillParameterBindings() {
			if(ReportSource == null)
				return;
			ParameterBindingHelper helper = new ParameterBindingHelper(ReportSource.Parameters, ParameterBindings);
			foreach(ParameterBinding item in helper.GetMissedBindings())
				ParameterBindings.Add(item);
		}
		public void RemoveUnusedParameterBindings() {
			if(ReportSource == null)
				return;
			ParameterBindingHelper helper = new ParameterBindingHelper(ReportSource.Parameters, ParameterBindings);
			foreach(ParameterBinding item in helper.GetOddBindings())
				ParameterBindings.Remove(item);
		}
	}
}
