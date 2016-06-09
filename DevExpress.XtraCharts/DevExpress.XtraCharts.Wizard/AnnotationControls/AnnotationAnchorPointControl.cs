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
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraCharts.Native;
using DevExpress.XtraCharts.Localization;
using DevExpress.Charts.Native;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationAnchorPointControl : ChartUserControl {
		abstract class AnchorHelper {
			public static AnchorHelper CreateInstance(Type type) {
				if (type.Equals(typeof(ChartAnchorPoint)))
					return new ChartAnchorHelper();
				if (type.Equals(typeof(PaneAnchorPoint)))
					return new PaneAnchorHelper();
				if (type.Equals(typeof(SeriesPointAnchorPoint)))
					return new SeriesPointAnchorHelper();
				ChartDebug.Fail("Unknown anchor point");
				return new ChartAnchorHelper();
			}
			public abstract string ComboText { get; }
			public abstract ChartUserControl CreateOptionsControl(AnnotationAnchorPoint anchorPoint, Diagram diagram);
			public abstract AnnotationAnchorPoint CreateAnchorPoint(Annotation annotation);
			public abstract bool IsSupported(Annotation annotation);
		}
		class ChartAnchorHelper : AnchorHelper {
			public override string ComboText { get { return ChartLocalizer.GetString(ChartStringId.WizAnchorPointChart); } }
			public override ChartUserControl CreateOptionsControl(AnnotationAnchorPoint anchorPoint, Diagram diagram) {
				AnnotationChartAnchorPointControl optionsControl = new AnnotationChartAnchorPointControl();
				optionsControl.Initialize((ChartAnchorPoint)anchorPoint);
				return optionsControl;
			}
			public override AnnotationAnchorPoint CreateAnchorPoint(Annotation annotation) {
				Chart chart = CommonUtils.FindOwnerChart(annotation);
				return chart != null ? AnnotationHelper.CreateChartAnchorPoint(annotation, chart) : new ChartAnchorPoint();
			}
			public override bool IsSupported(Annotation annotation) {
				return true;
			}
		}
		class PaneAnchorHelper : AnchorHelper {
			public override string ComboText { get { return ChartLocalizer.GetString(ChartStringId.WizAnchorPointPane); } }
			public override ChartUserControl CreateOptionsControl(AnnotationAnchorPoint anchorPoint, Diagram diagram) {
				AnnotationPaneAnchorPointControl optionsControl = new AnnotationPaneAnchorPointControl();
				optionsControl.Initialize((PaneAnchorPoint)anchorPoint, (XYDiagram2D)diagram);
				return optionsControl;
			}
			public override AnnotationAnchorPoint CreateAnchorPoint(Annotation annotation) {
				Chart chart = CommonUtils.FindOwnerChart(annotation);
				return chart != null ? AnnotationHelper.CreatePaneAnchorPoint(annotation, chart) : new PaneAnchorPoint();
			}
			public override bool IsSupported(Annotation annotation) {
				return AnnotationHelper.IsPaneAnchorPointSupported(annotation);
			}
		}
		class SeriesPointAnchorHelper : AnchorHelper {
			public override string ComboText { get { return ChartLocalizer.GetString(ChartStringId.WizAnchorPointSeriesPoint); } }
			public override ChartUserControl CreateOptionsControl(AnnotationAnchorPoint anchorPoint, Diagram diagram) {
				AnnotationSeriesPointAnchorPointControl optionsControl = new AnnotationSeriesPointAnchorPointControl();
				optionsControl.Initialize((SeriesPointAnchorPoint)anchorPoint);
				return optionsControl;
			}
			public override AnnotationAnchorPoint CreateAnchorPoint(Annotation annotation) {
				return new SeriesPointAnchorPoint();
			}
			public override bool IsSupported(Annotation annotation) {
				return AnnotationHelper.IsSeriesPointAnchorPointSupported(annotation);
			}
		}
		struct AnchorKindItem {
			Type type;
			string text;
			public Type Type { get { return type; } }
			public AnchorKindItem(Type type) {
				this.type = type;
				text = AnchorHelper.CreateInstance(type).ComboText;
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is AnchorKindItem) && type.Equals(((AnchorKindItem)obj).type);
			}
			public override int GetHashCode() {
				return type.GetHashCode();
			}
		}
		Annotation annotation;
		Diagram diagram;
		Action0 changedCallback;
		bool inUpdate;
		public AnnotationAnchorPointControl() {
			InitializeComponent();
		}
		public void Initialize(Annotation annotation, Diagram diagram, Action0 changedCallback) {
			this.annotation = annotation;
			this.diagram = diagram;
			this.changedCallback = changedCallback;
			inUpdate = true;
			try {
				cbAnchorKind.Properties.Items.Clear();
				cbAnchorKind.Properties.Items.Add(new AnchorKindItem(typeof(ChartAnchorPoint)));
				if (AnnotationHelper.IsPaneAnchorPointSupported(annotation))
					cbAnchorKind.Properties.Items.Add(new AnchorKindItem(typeof(PaneAnchorPoint)));
				if (AnnotationHelper.IsSeriesPointAnchorPointSupported(annotation))
					cbAnchorKind.Properties.Items.Add(new AnchorKindItem(typeof(SeriesPointAnchorPoint)));
				AnchorHelper helper = AnchorHelper.CreateInstance(annotation.AnchorPoint.GetType());
				if (helper.IsSupported(annotation))
					cbAnchorKind.SelectedItem = new AnchorKindItem(annotation.AnchorPoint.GetType());
				else
					cbAnchorKind.SelectedIndex = -1;
				UpdateOptionsControl();
			}
			finally {
				inUpdate = false;
			}
		}
		void UpdateOptionsControl() {
			pnlAnchorOptions.Controls.Clear();
			AnchorHelper helper = AnchorHelper.CreateInstance(annotation.AnchorPoint.GetType());
			if (helper.IsSupported(annotation)) {
				ChartUserControl optionsControl = helper.CreateOptionsControl(annotation.AnchorPoint, diagram);
				if (optionsControl != null) {
					optionsControl.Dock = DockStyle.Fill;
					pnlAnchorOptions.Controls.Add(optionsControl);
				}
			}
		}
		void cbAnchorKind_SelectedIndexChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				Type type = ((AnchorKindItem)cbAnchorKind.SelectedItem).Type;
				annotation.AnchorPoint = AnchorHelper.CreateInstance(type).CreateAnchorPoint(annotation);
				UpdateOptionsControl();
				changedCallback();
			}
		}
	}
}
