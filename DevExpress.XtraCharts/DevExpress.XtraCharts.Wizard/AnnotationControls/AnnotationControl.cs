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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.LookAndFeel;
using DevExpress.XtraTab;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationControl : FilterTabsControl {
		Annotation annotation;
		Chart chart;
		Chart originalChart;
		Action0 changeNameMethod;
		public override XtraTabControl TabControl { get { return tbcPagesControl; } }
		public AnnotationControl() {
			InitializeComponent();
		}
		public void Initialize(Annotation annotation, Chart chart, UserLookAndFeel lookAndFeel, Chart originalChart, CollectionBase filter, object selectedTabHandle, Action0 changeNameMethod) {
			this.chart = chart;
			this.originalChart = originalChart;
			this.annotation = annotation;
			this.changeNameMethod = changeNameMethod;
			InitializeCore(lookAndFeel, filter, selectedTabHandle);
		}
		protected override void InitializeTags() {
			base.InitializeTags();
			tbGeneral.Tag = AnnotationPageTab.General;
			tbAnchorPoint.Tag = AnnotationPageTab.AnchorPoint;
			tbShapePosition.Tag = AnnotationPageTab.ShapePosition;
			tbContent.Tag = AnnotationPageTab.Content;
			tbPadding.Tag = AnnotationPageTab.Padding;
			tbAppearance.Tag = AnnotationPageTab.Appearance;
			tbBorder.Tag = AnnotationPageTab.Border;
			tbShadow.Tag = AnnotationPageTab.Shadow;
		}
		protected override void Initialize(UserLookAndFeel lookAndFeel) {
			base.Initialize(lookAndFeel);
			annotationGeneralControl.Initialize(annotation, UpdateControls, changeNameMethod);
			annotationAnchorPointControl.Initialize(annotation, chart.Diagram, annotationGeneralControl.UpdateLabelModeControls);
			annotationShapePositionControl.Initialize(annotation, annotationGeneralControl.UpdateLabelModeControls);
			InitializeContentControl();
			paddingControl.Initialize(annotation.Padding, annotationGeneralControl.UpdateLayoutControls);
			annotationAppearanceControl.Initialize(annotation, annotationGeneralControl.UpdateLayoutControls);
			borderControl.Initialize(annotation.Border, annotationGeneralControl.UpdateLayoutControls);
			shadowControl.Initialize(annotation.Shadow);
			UpdateControls();
		}
		void InitializeContentControl() {
			tbContent.Controls.Clear();
			TextAnnotation textAnnotation = annotation as TextAnnotation;
			if (textAnnotation != null) {
				AnnotationTextContentControl textContentControl = new AnnotationTextContentControl();
				textContentControl.Dock = DockStyle.Fill;
				textContentControl.Initialize(textAnnotation, annotationGeneralControl.UpdateLayoutControls);
				tbContent.Controls.Add(textContentControl);
				return;
			}
			ImageAnnotation imageAnnotation = annotation as ImageAnnotation;
			if (imageAnnotation != null) {
				AnnotationImageContentControl imageContentControl = new AnnotationImageContentControl();
				imageContentControl.Dock = DockStyle.Fill;
				imageContentControl.Initialize(imageAnnotation, annotationGeneralControl.UpdateLayoutControls, originalChart);
				annotationGeneralControl.UpdateLayoutCallback = imageContentControl.UpdateSizeModeCombo;
				tbContent.Controls.Add(imageContentControl);
			}
		}
		void UpdateControls() {
			annotationAnchorPointControl.Enabled = annotation.Visible;
			annotationShapePositionControl.Enabled = annotation.Visible;
			ChartUserControl contentControl = tbContent.Controls.Count > 0 ? (ChartUserControl)tbContent.Controls[0] : null;
			if (contentControl != null)
				contentControl.Enabled = annotation.Visible;
			paddingControl.Enabled = annotation.Visible;
			annotationAppearanceControl.Enabled = annotation.Visible;
			borderControl.Enabled = annotation.Visible;
			shadowControl.Enabled = annotation.Visible;
		}
	}
}
