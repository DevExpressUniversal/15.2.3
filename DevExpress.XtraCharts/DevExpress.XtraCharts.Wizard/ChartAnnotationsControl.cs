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
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts.Wizard {
	internal partial class ChartAnnotationsControl : SplitterWizardControlWithPreview {
		bool inUpdate;
		object SelectedTabHandle { get { return annotationControl != null ? annotationControl.SelectedTabHandle : null; } }
		public ChartAnnotationsControl() {
			InitializeComponent();
		}
		public override void InitializeChart(WizardFormBase form) {
			base.InitializeChart(form);
			DesignControl.SelectionMode = ElementSelectionMode.Single;
			DesignControl.ObjectHotTracked += new HotTrackEventHandler(designControl_ObjectHotTracked);
			DesignControl.ObjectSelected += new HotTrackEventHandler(designControl_ObjectSelected);
			annotationControl.Enabled = Chart.AnnotationRepository.Count > 0;
			annotationListRedactControl.Initialize(Chart.AnnotationRepository);
		}
		void annotationListRedactControl_SelectedElementChanged() {
			if (!inUpdate) {
				Annotation annotation = annotationListRedactControl.CurrentElement as Annotation;
				IHitTest hitElement = annotation as IHitTest;
				if (hitElement != null)
					Chart.SelectHitElement(hitElement);
				SelectAnnotation(annotation);
			}
		}
		void SelectAnnotation(Annotation annotation) {
			annotationControl.Enabled = annotation != null;
			if (annotation != null)
				annotationControl.Initialize(annotation, WizardForm.Chart, WizardLookAndFeel, WizardForm.OriginalChart, ((WizardAnnotationPage)WizardPage).HiddenPageTabs,
					SelectedTabHandle, annotationListRedactControl.ChangeAnnotationName);
		}
		void designControl_ObjectSelected(object sender, HotTrackEventArgs args) {
			if (!ValidateContent()) {
				args.Cancel = true;
				return;
			}
			Annotation annotation = args.Object as Annotation;
			if (annotation != null) {
				SelectAnnotation(annotation);
				inUpdate = true;
				try {
					annotationListRedactControl.CurrentElement = annotation;
				}
				finally {
					inUpdate = false;
				}
			}
			else
				args.Cancel = true;
		}
		void designControl_ObjectHotTracked(object sender, HotTrackEventArgs args) {
			if (!(args.Object is Annotation))
				args.Cancel = true;
		}
		public override void Release() {
			Chart.ClearSelection();
			DesignControl.SelectionMode = ElementSelectionMode.None;
			DesignControl.ObjectHotTracked -= new HotTrackEventHandler(designControl_ObjectHotTracked);
			DesignControl.ObjectSelected -= new HotTrackEventHandler(designControl_ObjectSelected);
			base.Release();
		}		
	}
}
