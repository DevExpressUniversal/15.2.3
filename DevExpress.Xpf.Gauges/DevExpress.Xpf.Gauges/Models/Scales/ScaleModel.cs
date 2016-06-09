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
	public abstract class ScaleModel : ModelBase {
		public static readonly DependencyProperty LabelPresentationProperty = DependencyPropertyManager.Register("LabelPresentation",
			typeof(ScaleLabelPresentation), typeof(ScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty TickmarksPresentationProperty = DependencyPropertyManager.Register("TickmarksPresentation",
			typeof(TickmarksPresentation), typeof(ScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty LineOptionsProperty = DependencyPropertyManager.Register("LineOptions",
			typeof(ScaleLineOptions), typeof(ScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowLineProperty = DependencyPropertyManager.Register("ShowLine",
		   typeof(bool), typeof(ScaleModel), new PropertyMetadata(false, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowLabelsProperty = DependencyPropertyManager.Register("ShowLabels",
		   typeof(bool), typeof(ScaleModel), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowMajorTickmarksProperty = DependencyPropertyManager.Register("ShowMajorTickmarks",
		   typeof(bool), typeof(ScaleModel), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty ShowMinorTickmarksProperty = DependencyPropertyManager.Register("ShowMinorTickmarks",
		   typeof(bool), typeof(ScaleModel), new PropertyMetadata(true, NotifyPropertyChanged));
		public static readonly DependencyProperty MinorTickmarkOptionsProperty = DependencyPropertyManager.Register("MinorTickmarkOptions",
			typeof(MinorTickmarkOptions), typeof(ScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		public static readonly DependencyProperty MajorTickmarkOptionsProperty = DependencyPropertyManager.Register("MajorTickmarkOptions",
			typeof(MajorTickmarkOptions), typeof(ScaleModel), new PropertyMetadata(null, NotifyPropertyChanged));
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ScaleLabelPresentation LabelPresentation {
			get { return (ScaleLabelPresentation)GetValue(LabelPresentationProperty); }
			set { SetValue(LabelPresentationProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public TickmarksPresentation TickmarksPresentation {
			get { return (TickmarksPresentation)GetValue(TickmarksPresentationProperty); }
			set { SetValue(TickmarksPresentationProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ScaleLineOptions LineOptions {
			get { return (ScaleLineOptions)GetValue(LineOptionsProperty); }
			set { SetValue(LineOptionsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowLine {
			get { return (bool)GetValue(ShowLineProperty); }
			set { SetValue(ShowLineProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowLabels {
			get { return (bool)GetValue(ShowLabelsProperty); }
			set { SetValue(ShowLabelsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowMajorTickmarks {
			get { return (bool)GetValue(ShowMajorTickmarksProperty); }
			set { SetValue(ShowMajorTickmarksProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool ShowMinorTickmarks {
			get { return (bool)GetValue(ShowMinorTickmarksProperty); }
			set { SetValue(ShowMinorTickmarksProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public MinorTickmarkOptions MinorTickmarkOptions {
			get { return (MinorTickmarkOptions)GetValue(MinorTickmarkOptionsProperty); }
			set { SetValue(MinorTickmarkOptionsProperty, value); }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public MajorTickmarkOptions MajorTickmarkOptions {
			get { return (MajorTickmarkOptions)GetValue(MajorTickmarkOptionsProperty); }
			set { SetValue(MajorTickmarkOptionsProperty, value); }
		}
	}
}
