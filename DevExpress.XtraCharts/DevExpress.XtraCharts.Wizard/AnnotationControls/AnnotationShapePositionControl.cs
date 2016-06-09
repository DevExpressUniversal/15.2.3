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
	internal partial class AnnotationShapePositionControl : ChartUserControl {
		abstract class PositionHelper {
			public static PositionHelper CreateInstance(Type type) {
				if (type.Equals(typeof(FreePosition)))
					return new FreePositionHelper();
				if (type.Equals(typeof(RelativePosition)))
					return new RelativePositionHelper();
				ChartDebug.Fail("Unknown shape position.");
				return new FreePositionHelper();
			}
			public static ChartUserControl CreateOptionsControl(AnnotationShapePosition shapePositoin) {
				PositionHelper helper = CreateInstance(shapePositoin.GetType());
				return helper.CreateOptionsControlInternal(shapePositoin);
			}
			public abstract string ComboText { get; }
			protected abstract ChartUserControl CreateOptionsControlInternal(AnnotationShapePosition shapePosition);
			public abstract AnnotationShapePosition CreateShapePosition(Annotation annotation);
		}
		class FreePositionHelper : PositionHelper {
			public override string ComboText { get { return ChartLocalizer.GetString(ChartStringId.WizShapePositionKindFree); } }
			protected override ChartUserControl CreateOptionsControlInternal(AnnotationShapePosition shapePosition) {
				AnnotationFreePositionControl optionsControl = new AnnotationFreePositionControl();
				optionsControl.Initialize((FreePosition)shapePosition);
				return optionsControl;
			}
			public override AnnotationShapePosition CreateShapePosition(Annotation annotation) {
				Chart chart = CommonUtils.FindOwnerChart(annotation);
				return chart != null ? AnnotationHelper.CreateFreePosition(annotation, chart) : new FreePosition();
			}
		}
		class RelativePositionHelper : PositionHelper {
			public override string ComboText { get { return ChartLocalizer.GetString(ChartStringId.WizShapePositionKindRelative); } }
			protected override ChartUserControl CreateOptionsControlInternal(AnnotationShapePosition shapePosition) {
				AnnotationRelativePositionControl optionsControl = new AnnotationRelativePositionControl();
				optionsControl.Initialize((RelativePosition)shapePosition);
				return optionsControl;
			}
			public override AnnotationShapePosition CreateShapePosition(Annotation annotation) {
				return AnnotationHelper.CreateRelativePosition(annotation);
			}
		}
		struct PositionKindItem {
			Type type;
			string text;
			public Type Type { get { return type; } }
			public PositionKindItem(Type type) {
				this.type = type;
				text = PositionHelper.CreateInstance(type).ComboText;
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is PositionKindItem) && type.Equals(((PositionKindItem)obj).type);
			}
			public override int GetHashCode() {
				return type.GetHashCode();
			}
		}
		Annotation annotation;
		Action0 changedCallback;
		bool inUpdate;
		public AnnotationShapePositionControl() {
			InitializeComponent();
		}
		public void Initialize(Annotation annotation, Action0 changedCallback) {
			this.annotation = annotation;
			this.changedCallback = changedCallback;
			inUpdate = true;
			try {
				cbPositionKind.Properties.Items.Clear();
				cbPositionKind.Properties.Items.Add(new PositionKindItem(typeof(FreePosition)));
				cbPositionKind.Properties.Items.Add(new PositionKindItem(typeof(RelativePosition)));
				cbPositionKind.SelectedItem = new PositionKindItem(annotation.ShapePosition.GetType());
				UpdateOptionsControl();
			}
			finally {
				inUpdate = false;
			}
		}
		void UpdateOptionsControl() {
			pnlShapePositonOptions.Controls.Clear();
			ChartUserControl optionsControl = PositionHelper.CreateOptionsControl(annotation.ShapePosition);
			if (optionsControl != null) {
				optionsControl.Dock = DockStyle.Fill;
				pnlShapePositonOptions.Controls.Add(optionsControl);
			}
		}
		void cbPositionKind_SelectedIndexChanged(object sender, EventArgs e) {
			if (!inUpdate) {
				Type type = ((PositionKindItem)cbPositionKind.SelectedItem).Type;
				annotation.ShapePosition = PositionHelper.CreateInstance(type).CreateShapePosition(annotation);
				UpdateOptionsControl();
				changedCallback();
			}
		}
	}
}
