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
	internal partial class AnnotationTextContentControl : ChartUserControl {
		struct StringAlignmentItem {
			StringAlignment alignment;
			string text;
			public StringAlignment Alignment { get { return alignment; } }
			public StringAlignmentItem(StringAlignment alignment) {
				this.alignment = alignment;
				switch (alignment) {
					case StringAlignment.Center:
						text = ChartLocalizer.GetString(ChartStringId.WizStringAlignmentCenter);
						break;
					case StringAlignment.Near:
						text = ChartLocalizer.GetString(ChartStringId.WizStringAlignmentNear);
						break;
					case StringAlignment.Far:
						text = ChartLocalizer.GetString(ChartStringId.WizStringAlignmentFar);
						break;
					default:
						ChartDebug.Fail("Unknown string alignment");
						goto case StringAlignment.Center;
				}
			}
			public override string ToString() {
				return text;
			}
			public override bool Equals(object obj) {
				return (obj is StringAlignmentItem) && alignment == ((StringAlignmentItem)obj).alignment;
			}
			public override int GetHashCode() {
				return alignment.GetHashCode();
			}
		}
		TextAnnotation annotation;
		Action0 changedCallback;
		public AnnotationTextContentControl() {
			InitializeComponent();
		}
		public void Initialize(TextAnnotation annotation, Action0 changedCallback) {
			this.annotation = annotation;
			this.changedCallback = changedCallback;
			memoText.Text = annotation.Text;
			cbAlignment.Properties.Items.Clear();
			foreach (StringAlignment alignment in Enum.GetValues(typeof(StringAlignment)))
				cbAlignment.Properties.Items.Add(new StringAlignmentItem(alignment));
			cbAlignment.SelectedItem = new StringAlignmentItem(annotation.TextAlignment);
			textAppearanceControl.Initialize(annotation, changedCallback);
		}
		void memoText_EditValueChanged(object sender, EventArgs e) {
			annotation.Text = memoText.Text;
			if (changedCallback != null)
				changedCallback();
		}
		void cbAlignment_SelectedIndexChanged(object sender, EventArgs e) {
			annotation.TextAlignment = ((StringAlignmentItem)cbAlignment.SelectedItem).Alignment;
		}
	}
}
