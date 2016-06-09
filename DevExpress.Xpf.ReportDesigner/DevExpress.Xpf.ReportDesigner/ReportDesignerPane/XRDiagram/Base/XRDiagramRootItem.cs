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
using DevExpress.Diagram.Core;
using DevExpress.Xpf.Diagram;
using DevExpress.Xpf.Diagram.Native;
using DevExpress.Xpf.Reports.UserDesigner.Native;
using DevExpress.Xpf.Bars;
using DevExpress.XtraReports.UI;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using System.Linq;
using DevExpress.Mvvm.Native;
using System.Windows.Data;
using System.Globalization;
using DevExpress.Mvvm;
using DevExpress.Mvvm.UI.Native;
using DevExpress.Images;
namespace DevExpress.Xpf.Reports.UserDesigner.XRDiagram {
	public partial class XRDiagramRoot : DiagramRoot {
		protected class XRDiagramRootController : XRDiagramRootControllerBase {
			public XRDiagramRootController(IDiagramRoot item) : base(item) { }
			protected override IEnumerable<IAdorner> CreateAdorners() {
				var xRDiagramRootMarginsAdorner = new XRDiagramRootMarginsAdorner(Diagram);
				var multiBindingConverter = new XRDiagramZoomToMarginsConverter();
				xRDiagramRootMarginsAdorner.SetBinding(XRDiagramRootMarginsAdorner.AdornerLeftMarginProperty, GetMarginToZoomBinding(multiBindingConverter, XRDiagramControl.LeftPaddingProperty.Name));
				xRDiagramRootMarginsAdorner.SetBinding(XRDiagramRootMarginsAdorner.AdornerRightMarginProperty, GetMarginToZoomBinding(multiBindingConverter, XRDiagramControl.RightPaddingProperty.Name));
				yield return Diagram.CreateAdorner(xRDiagramRootMarginsAdorner);
				yield return Diagram.CreateHRulerAdorner(new XRHorizontalRulerScale(Diagram)).MakeTopmostEx();
				yield return Diagram.CreateHRulerAdorner(new XRHorizontalRulerScaleBackground(Diagram));
			}
			MultiBinding GetMarginToZoomBinding(XRDiagramZoomToMarginsConverter multiBindingConverter, string propertyName) {
				var multibinding = new MultiBinding() { Converter = multiBindingConverter };
				multibinding.Bindings.Add(new Binding(XRDiagramControl.ZoomFactorProperty.Name) { Source = Diagram });
				multibinding.Bindings.Add(new Binding(propertyName) { Source = Diagram });
				multibinding.NotifyOnSourceUpdated = true;
				return multibinding;
			}
		}
		static XRDiagramRoot() {
			AssemblyInitializer.Init();
			DependencyPropertyRegistrator<XRDiagramRoot>.New()
				.OverrideMetadata(SelectionLayerProperty, EmptySelectionLayer.Instance)
				.OverrideMetadata(ClipToBoundsProperty, true)
				.OverrideDefaultStyleKey();
			;
		}
		public XRDiagramRoot(DiagramControl diagram) : base(diagram) { }
		public XRDiagramControl Diagram { get { return (XRDiagramControl)base.diagramCore; } }
		ImageSource GetIcon(BandKind bandKind) {
			return ImageSourceHelper.GetImageSource(AssemblyHelper.GetResourceUri(typeof(BandHeaderAdorner).Assembly, string.Format("Images/BandIcons/{0}.png", bandKind.ToString())));
		}
	}
}
