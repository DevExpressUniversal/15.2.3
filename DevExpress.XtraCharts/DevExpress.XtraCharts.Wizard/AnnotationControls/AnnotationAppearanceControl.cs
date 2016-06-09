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
using DevExpress.Charts.Native;
using DevExpress.Utils;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Wizard.AnnotationControls {
	internal partial class AnnotationAppearanceControl : ChartUserControl {
		struct ShapeKindItem {
			readonly ShapeKind shapeKind;
			readonly string text;
			public ShapeKind ShapeKind { get { return shapeKind; } }
			public ShapeKindItem(ShapeKind shapeKind) {
				this.shapeKind = shapeKind;
				switch (shapeKind) {
					case ShapeKind.Rectangle:
						text = ChartLocalizer.GetString(ChartStringId.WizShapeKindRectangle);
						break;
					case ShapeKind.RoundedRectangle:
						text = ChartLocalizer.GetString(ChartStringId.WizShapeKindRoundedRectangle);
						break;
					case ShapeKind.Ellipse:
						text = ChartLocalizer.GetString(ChartStringId.WizShapeKindEllipse);
						break;
					default:
						ChartDebug.Fail("Unknown shape kind.");
						goto case ShapeKind.Rectangle;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is ShapeKindItem) && shapeKind == ((ShapeKindItem)obj).shapeKind;
			}
			public override int GetHashCode() {
				return shapeKind.GetHashCode();
			}
		}
		struct AnnotationConnectorStyleItem {
			readonly AnnotationConnectorStyle annotationConnectorStyle;
			readonly string text;
			public AnnotationConnectorStyle AnnotationConnectorStyle { get { return annotationConnectorStyle; } }
			public AnnotationConnectorStyleItem(AnnotationConnectorStyle annotationConnectorStyle) {
				this.annotationConnectorStyle = annotationConnectorStyle;
				switch (annotationConnectorStyle) {
					case AnnotationConnectorStyle.Arrow:
						text = ChartLocalizer.GetString(ChartStringId.WizAnnotationConnectorStyleArrow);
						break;
					case AnnotationConnectorStyle.Line:
						text = ChartLocalizer.GetString(ChartStringId.WizAnnotationConnectorStyleLine);
						break;
					case AnnotationConnectorStyle.None:
						text = ChartLocalizer.GetString(ChartStringId.WizAnnotationConnectorStyleNone);
						break;
					case AnnotationConnectorStyle.NotchedArrow:
						text = ChartLocalizer.GetString(ChartStringId.WizAnnotationConnectorStyleNotchedArrow);
						break;
					case AnnotationConnectorStyle.Tail:
						text = ChartLocalizer.GetString(ChartStringId.WizAnnotationConnectorStyleTail);
						break;
					default:
						ChartDebug.Fail("Unknown annotation connector style");
						goto case AnnotationConnectorStyle.None;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is AnnotationConnectorStyleItem) && annotationConnectorStyle == ((AnnotationConnectorStyleItem)obj).annotationConnectorStyle;
			}
			public override int GetHashCode() {
				return annotationConnectorStyle.GetHashCode();
			}
		}
		Annotation annotation;
		Action0 changedCallback;
		bool inUpdate;
		public AnnotationAppearanceControl() {
			InitializeComponent();
		}
		public void Initialize(Annotation annotation, Action0 changedCallback) {
			this.annotation = annotation;
			this.changedCallback = changedCallback;
			backgroundControl.Initialize(annotation, null);
			inUpdate = true;
			try {
				cbShapeKind.Properties.Items.Clear();
				foreach (ShapeKind shapeKind in Enum.GetValues(typeof(ShapeKind)))
					cbShapeKind.Properties.Items.Add(new ShapeKindItem(shapeKind));
				cbShapeKind.SelectedItem = new ShapeKindItem(annotation.ShapeKind);
				spnShapeFillet.EditValue = annotation.ShapeFillet;
				cbConnectorStyle.Properties.Items.Clear();
				foreach (AnnotationConnectorStyle annotationConnectorStyle in Enum.GetValues(typeof(AnnotationConnectorStyle)))
					cbConnectorStyle.Properties.Items.Add(new AnnotationConnectorStyleItem(annotationConnectorStyle));
				cbConnectorStyle.SelectedItem = new AnnotationConnectorStyleItem(annotation.ConnectorStyle);
			}
			finally {
				inUpdate = false;
			}
			UpdateControls();
		}
		void UpdateControls() {
			inUpdate = true;
			try {
				pnlShapeFillet.Enabled = annotation.ShapeKind == ShapeKind.RoundedRectangle;
			}
			finally {
				inUpdate = false;
			}
			if (changedCallback != null)
				changedCallback();
		}
		void cbShapeKind_SelectedIndexChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				annotation.ShapeKind = ((ShapeKindItem)cbShapeKind.SelectedItem).ShapeKind;
				UpdateControls();
			}
		}
		void spnShapeFillet_EditValueChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				annotation.ShapeFillet = Convert.ToInt32(spnShapeFillet.EditValue);
				UpdateControls();
			}
		}
		void cbConnectorStyle_SelectedIndexChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				annotation.ConnectorStyle = ((AnnotationConnectorStyleItem)cbConnectorStyle.SelectedItem).AnnotationConnectorStyle;
				UpdateControls();
			}
		}
	}
}
