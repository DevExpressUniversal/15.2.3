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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DevExpress.Office.Internal;
using DevExpress.Xpf.RichEdit.UI;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Model;
using DevExpress.Office;
using DevExpress.Office.Design.Internal;
using System.Windows.Markup;
namespace DevExpress.Xpf.RichEdit.UI {
	public partial class BorderShadingTypeLineUserControl : UserControl {
		#region Fields
		BorderInfo border;
		DocumentModel documentModel;
		IRichEditControl richEditControl;
		#endregion
		public BorderShadingTypeLineUserControl() {
			InitializeComponent();
			border = new BorderInfo();
		}
		#region Properties
		public DocumentModel DocumentModel { get { return documentModel; } set { documentModel = value; 
		} }
		public IRichEditControl RichEditControl {
			get { return richEditControl; }
			set {
				richEditControl = value;
				borderLineStyleEdit.RichEditControl = RichEditControl;
			}
		}
		public BorderInfo Border { get { return border; } }
		#endregion
		public event EventHandler BorderChanged;
		protected void RaiseBorderChanged() {
			if (BorderChanged != null)
				BorderChanged(this, EventArgs.Empty);
		}
		private void colorEdit_ColorChanged(object sender, RoutedEventArgs e) {
			System.Windows.Media.Color mediacolor = colorEdit.Color;
#if!SL
			border.Color = System.Drawing.Color.FromArgb(mediacolor.A, mediacolor.R, mediacolor.G, mediacolor.B); 
#else
			border.Color = mediacolor;
#endif
			RaiseBorderChanged();
		}
		public void SetInition(BorderInfo border) {
			this.border.CopyFrom(border);
			double widthInPoints = (double)DocumentModel.UnitConverter.ModelUnitsToPointsF(border.Width);
			this.borderLineWidthEdit.EditValue = widthInPoints;
			borderLineStyleEdit.Value = border.Style;
			colorEdit.Color = XpfTypeConverter.ToPlatformColor(border.Color);
		}
		private void borderLineWidthEdit_EditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			if (this.borderLineWidthEdit.EditValue != null) {
				double width = Convert.ToDouble(this.borderLineWidthEdit.EditValue);
				int widthInModelUnits = (int)Math.Round(DocumentModel.UnitConverter.PointsToModelUnitsF((float)width));
				if (widthInModelUnits != border.Width) {
					border.Width = widthInModelUnits;
					RaiseBorderChanged();
				}
			}
		}
		private void borderLineStyleEdit_EditValueChanged(object sender, Editors.EditValueChangedEventArgs e) {
			if (e.NewValue != null) {
				BorderLineStyle style = (BorderLineStyle)e.NewValue;
				if (style != border.Style) {
					border.Style = style;
					RaiseBorderChanged();
				}
			}
		}
	}
}
