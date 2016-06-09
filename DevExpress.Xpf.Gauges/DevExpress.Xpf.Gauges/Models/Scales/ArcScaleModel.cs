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

using System.ComponentModel;
using System.Windows;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public class ArcScaleModel : ScaleModel {
		public static readonly DependencyProperty SpindleCapPresentationProperty = DependencyPropertyManager.Register("SpindleCapPresentation",
			typeof(SpindleCapPresentation), typeof(ArcScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty LabelOptionsProperty = DependencyPropertyManager.Register("LabelOptions",
			typeof(ArcScaleLabelOptions), typeof(ArcScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty SpindleCapOptionsProperty = DependencyPropertyManager.Register("SpindleCapOptions",
			typeof(SpindleCapOptions), typeof(ArcScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowSpindleCapProperty = DependencyPropertyManager.Register("ShowSpindleCap",
		   typeof(bool), typeof(ArcScaleModel), new PropertyMetadata(false, NotifyPropertyChanged));
		public static readonly DependencyProperty LinePresentationProperty = DependencyPropertyManager.Register("LinePresentation",
			typeof(ArcScaleLinePresentation), typeof(ArcScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty LayoutMarginProperty = DependencyPropertyManager.Register("LayoutMargin",
		   typeof(Thickness), typeof(ArcScaleModel), new PropertyMetadata(new Thickness(), NotifyPropertyChanged));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SpindleCapPresentation SpindleCapPresentation {
			get { return (SpindleCapPresentation)GetValue(SpindleCapPresentationProperty); }
			set { SetValue(SpindleCapPresentationProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleLabelOptions LabelOptions {
			get { return (ArcScaleLabelOptions)GetValue(LabelOptionsProperty); }
			set { SetValue(LabelOptionsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public SpindleCapOptions SpindleCapOptions {
			get { return (SpindleCapOptions)GetValue(SpindleCapOptionsProperty); }
			set { SetValue(SpindleCapOptionsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowSpindleCap {
			get { return (bool)GetValue(ShowSpindleCapProperty); }
			set { SetValue(ShowSpindleCapProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ArcScaleLinePresentation LinePresentation {
			get { return (ArcScaleLinePresentation)GetValue(LinePresentationProperty); }
			set { SetValue(LinePresentationProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Thickness LayoutMargin {
			get { return (Thickness)GetValue(LayoutMarginProperty); }
			set { SetValue(LayoutMarginProperty, value); }
		}
		protected override GaugeDependencyObject CreateObject() {
			return new ArcScaleModel();
		}
	}
	public class ArcScaleModelCollection : ModelCollection<ArcScaleModel> {
	}
}
