#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       XtraReports for ASP.NET                                     }
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
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.XtraReports.Localization;
namespace DevExpress.XtraReports.Web.DocumentViewer {
	public class DocumentViewerViewerStyles : StylesBase {
		const string
			BookmarkSelectionBorderName = "BookmarkSelectionBorder",
			PaddingsName = "Paddings",
			ShowDocumentShadowName = "ShowDocumentShadow",
			WidthName = "Width",
			HeightName = "Height";
		const bool DefaultShowDocumentShadow = true;
		readonly Border defaultBookmarkSelectionBorder = new Border();
		readonly Paddings defaultPaddings = new Paddings();
		public DocumentViewerViewerStyles(ISkinOwner owner)
			: base(owner) {
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerViewerStylesBookmarkSelectionBorder")]
#endif
		[SRCategory(ReportStringId.CatAppearance)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[AutoFormatEnable]
		[NotifyParentProperty(true)]
		public Border BookmarkSelectionBorder {
			get { return (Border)GetObjectProperty(BookmarkSelectionBorderName, defaultBookmarkSelectionBorder); }
			set { SetObjectProperty(BookmarkSelectionBorderName, defaultBookmarkSelectionBorder, value); }
		}
		[DefaultValue(DefaultShowDocumentShadow)]
		[SRCategory(ReportStringId.CatAppearance)]
		[NotifyParentProperty(true)]
		[AutoFormatDisable]
		public bool ShowDocumentShadow {
			get { return GetBoolProperty(ShowDocumentShadowName, DefaultShowDocumentShadow); }
			set { SetBoolProperty(ShowDocumentShadowName, DefaultShowDocumentShadow, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerViewerStylesWidth")]
#endif
		[AutoFormatDisable]
		[Category("Layout")]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportViewerSizeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Unit), "")]
		public Unit Width {
			get { return GetUnitProperty(WidthName, Unit.Empty); }
			set { SetUnitProperty(WidthName, Unit.Empty, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerViewerStylesHeight")]
#endif
		[AutoFormatDisable]
		[Category("Layout")]
		[TypeConverter("DevExpress.Web.Design.Reports.Converters.ReportViewerSizeConverter, " + AssemblyInfo.SRAssemblyWebDesignFull)]
		[NotifyParentProperty(true)]
		[DefaultValue(typeof(Unit), "")]
		public Unit Height {
			get { return GetUnitProperty(HeightName, Unit.Empty); }
			set { SetUnitProperty(HeightName, Unit.Empty, value); }
		}
#if !SL
	[DevExpressXtraReportsWebLocalizedDescription("DocumentViewerViewerStylesPaddings")]
#endif
		[AutoFormatDisable]
		[NotifyParentProperty(true)]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public Paddings Paddings {
			get { return (Paddings)GetObjectProperty(PaddingsName, defaultPaddings); }
			set { SetObjectProperty(PaddingsName, defaultPaddings, value); }
		}
		internal Paddings ActualPaddings {
			get {
				var actualPaddings = new Paddings(10);
				actualPaddings.MergeWith(Paddings);
				return actualPaddings;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(),
				new IStateManager[] { Paddings });
		}
		public override void CopyFrom(StylesBase source) {
			base.CopyFrom(source);
			var src = source as DocumentViewerViewerStyles;
			if(src == null)
				return;
			BookmarkSelectionBorder.CopyFrom(src.BookmarkSelectionBorder);
			Width = src.Width;
			Height = src.Height;
			Paddings.CopyFrom(src.Paddings);
		}
	}
}
