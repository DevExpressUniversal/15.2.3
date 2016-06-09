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

using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Data;
using System.Windows.Media;
using DevExpress.Diagram.Core;
using DevExpress.Mvvm.Native;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Reports.UserDesigner.XRDiagram;
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportModel {
	public class BandModel : BandModelBase {
		protected internal BandModel(XRControlModelFactory.ISource<Band> source, ImageSource icon = null)
			: base(source, icon ?? ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(BandModel).Assembly, string.Format("Images/BandIcons/{0}.png", source.XRObject.BandKind)))) {
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public MultiColumnMode MultiColumnMode {
			get { return ((DetailBand)XRObject).MultiColumn.Mode; }
			set { ((DetailBand)XRObject).MultiColumn.Mode = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public ColumnLayout MultiColumnLayout {
			get { return ((DetailBand)XRObject).MultiColumn.Layout; }
			set { ((DetailBand)XRObject).MultiColumn.Layout = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int MultiColumnColumnCount {
			get { return ((DetailBand)XRObject).MultiColumn.ColumnCount; }
			set { ((DetailBand)XRObject).MultiColumn.ColumnCount = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float MultiColumnColumnWidth {
			get { return ((DetailBand)XRObject).MultiColumn.ColumnWidth; }
			set { ((DetailBand)XRObject).MultiColumn.ColumnWidth = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public float MultiColumnColumnSpacing {
			get { return ((DetailBand)XRObject).MultiColumn.ColumnSpacing; }
			set { ((DetailBand)XRObject).MultiColumn.ColumnSpacing = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		public int DetailCountAtDesignTime {
			get { return ((DetailReportBand)XRObject).ReportPrintOptions.DetailCountAtDesignTime; }
			set { ((DetailReportBand)XRObject).ReportPrintOptions.DetailCountAtDesignTime = value; }
		}
		protected override IEnumerable<PropertyDescriptor> GetEditableProperties() {
			switch(XRObject.BandKind) {
				case BandKind.Detail:
					return base.GetEditableProperties()
						.InjectProperty(this, x => x.MultiColumnMode)
						.InjectProperty(this, x => x.MultiColumnLayout)
						.InjectProperty(this, x => x.MultiColumnColumnCount)
						.InjectProperty(this, x => x.MultiColumnColumnWidth)
						.InjectProperty(this, x => x.MultiColumnColumnSpacing);
				case BandKind.DetailReport:
					return base.GetEditableProperties()
						.InjectProperty(this, x => x.DetailCountAtDesignTime);
				default:
					return base.GetEditableProperties();
			}
		}
		public override double Height {
			get {
				if(Collapsed) {
					return 27;
				}
				return base.Height;
			}
			set { base.Height = value; }
		}
		protected override IList<XRControl> GetXRControlsCollection() {
			return ListAdapter<XRControl>.FromTwoObjectLists<SubBand, XRControl>(XRObject.SubBands, XRObject.Controls);
		}
		protected override void BindAnchorsProperty() {
			DiagramItem.Anchors = XRObject.BandKind == BandKind.SubBand ? Sides.Bottom : Sides.None;
		}
		protected override void AttachDiagramItem() {
			base.AttachDiagramItem();
			DiagramItem.BandKind = XRObject.BandKind;
			BindDiagramItemToXRObject(() => DiagramItem.BandName = XRObject.Name, () => XRObject.Name);
			DiagramItem.BandName = XRObject.Name;
			var collapsePropertyName = ExpressionHelper.GetPropertyName(() => Collapsed);
			DiagramItem.SetBinding(XRDiagramItemBase.BandCollapsedProperty, new Binding(collapsePropertyName) { Source = this, Mode = BindingMode.TwoWay });
			DiagramItem.Collapsed = Collapsed;
			DiagramItem.CanDelete = (XRObject.BandKind & (BandKind.Detail | BandKind.BottomMargin | BandKind.TopMargin)) == 0;
		}
	}
}
