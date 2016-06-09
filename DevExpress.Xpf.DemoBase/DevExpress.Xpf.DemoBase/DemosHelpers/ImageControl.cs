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
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.DemoBase {
	public class ImageControl : Control {
		public static readonly DependencyProperty SourceProperty;
		public static readonly DependencyProperty StretchProperty;
		public static readonly DependencyProperty ImageLayoutTransformProperty = DependencyProperty.Register("ImageLayoutTransform", typeof(Transform), typeof(ImageControl), new PropertyMetadata(null));
		public static readonly DependencyProperty StretchDirectionProperty = DependencyProperty.Register("StretchDirection", typeof(StretchDirection), typeof(ImageControl), new PropertyMetadata(Image.StretchDirectionProperty.DefaultMetadata.DefaultValue));
		public StretchDirection StretchDirection {
			get { return (StretchDirection)GetValue(StretchDirectionProperty); }
			set { SetValue(StretchDirectionProperty, value); }
		}
		public Transform ImageLayoutTransform {
			get { return (Transform)GetValue(ImageLayoutTransformProperty); }
			set { SetValue(ImageLayoutTransformProperty, value); }
		}
		static ImageControl() {
			SourceProperty = DependencyProperty.Register("Source", typeof(object), typeof(ImageControl), new PropertyMetadata(null));
			StretchProperty = DependencyProperty.Register("Stretch", typeof(Stretch), typeof(ImageControl), new PropertyMetadata(Stretch.Uniform));
		}
		public ImageControl() {
			this.SetDefaultStyleKey(typeof(ImageControl));
		}
		public object Source {
			get { return GetValue(SourceProperty); }
			set { SetValue(SourceProperty, value); }
		}
		public Stretch Stretch {
			get { return (Stretch)GetValue(StretchProperty); }
			set { SetValue(StretchProperty, value); }
		}
	}
}
