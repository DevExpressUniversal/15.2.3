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
using System.Windows.Markup;
using System.Windows.Input;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Documents;
using DevExpress.Utils;
using System.Collections;
using System.ComponentModel;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Utils;
#if !SL
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils.Themes;
using DevExpress.Xpf.Core;
using System.Collections.ObjectModel;
using System.Windows.Controls.Primitives;
using System.Collections.Specialized;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
#endif
#if SL
using DevExpress.Xpf.Utils.Themes;
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using IInputElement = System.Windows.UIElement;
using Keyboard = DevExpress.Xpf.Editors.WPFCompatibility.SLKeyboard;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public class TransparentBrushRectangle : ContentControl {
		#region static
		public static readonly DependencyProperty LightColorProperty;
		public static readonly DependencyProperty DarkColorProperty;
		public static readonly DependencyProperty BlockWidthProperty;
		public static readonly DependencyProperty BlockHeightProperty;
		static TransparentBrushRectangle() {
			Type ownerType = typeof(TransparentBrushRectangle);
			DarkColorProperty = DependencyPropertyManager.Register("DarkColor", typeof(Color), ownerType, 
				new FrameworkPropertyMetadata(Color.FromArgb(0xff, 0xd3, 0xd3, 0xd3), FrameworkPropertyMetadataOptions.AffectsMeasure, (obj, args) => ((TransparentBrushRectangle)obj).UpdateRectangle()));
			LightColorProperty = DependencyPropertyManager.Register("LightColor", typeof(Color), ownerType,
				new FrameworkPropertyMetadata(Colors.White, FrameworkPropertyMetadataOptions.AffectsMeasure, (obj, args) => ((TransparentBrushRectangle)obj).UpdateRectangle()));
			BlockHeightProperty = DependencyPropertyManager.Register("BlockHeight", typeof(double), ownerType,
				new FrameworkPropertyMetadata(4d, FrameworkPropertyMetadataOptions.AffectsMeasure, (obj, args) => ((TransparentBrushRectangle)obj).UpdateRectangle()));
			BlockWidthProperty = DependencyPropertyManager.Register("BlockWidth", typeof(double), ownerType,
				new FrameworkPropertyMetadata(4d, FrameworkPropertyMetadataOptions.AffectsMeasure, (obj, args) => ((TransparentBrushRectangle)obj).UpdateRectangle()));
		}
		void UpdateRectangle() {
			GenerateRectangle(new Size(ActualWidth, ActualHeight));
		}
		#endregion
		public Color DarkColor {
			get { return (Color)GetValue(DarkColorProperty); }
			set { SetValue(DarkColorProperty, value); }
		}
		public Color LightColor {
			get { return (Color)GetValue(LightColorProperty); }
			set { SetValue(LightColorProperty, value); }
		}
		public double BlockWidth {
			get { return (double)GetValue(BlockWidthProperty); }
			set { SetValue(BlockWidthProperty, value); }
		}
		public double BlockHeight {
			get { return (double)GetValue(BlockHeightProperty); }
			set { SetValue(BlockHeightProperty, value); }
		}
		public TransparentBrushRectangle() {
			Loaded += OnLoaded;
			SizeChanged += OnSizeChanged;
		}
		void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			GenerateRectangle(new Size(ActualWidth, ActualHeight));
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			GenerateRectangle(new Size(ActualWidth, ActualHeight));
		}
		public void GenerateRectangle(Size containerSize) {
			GeometryGroup darkPathData = new GeometryGroup();
			GeometryGroup lightPathData = new GeometryGroup();
			int totalNeededX = (int)Math.Ceiling(containerSize.Width / BlockWidth);
			int totalNeededY = (int)Math.Ceiling(containerSize.Height / BlockHeight);
			double xOffset = 0d;
			double yOffset = 0d;
			for(int i = 0; i < totalNeededY; ++i) {
				for (int j = 0; j < totalNeededX; ++j) {
					if(i % 2 == j % 2)
						lightPathData.Children.Add(new RectangleGeometry() { Rect = new Rect(xOffset, yOffset, BlockWidth, BlockHeight) });
					else
						darkPathData.Children.Add(new RectangleGeometry() { Rect = new Rect(xOffset, yOffset, BlockWidth, BlockHeight) });
					xOffset += BlockWidth;
				}
				xOffset = 0;
				yOffset += BlockHeight;
			}
			Path darkPath = new Path() {
				Fill = new SolidColorBrush(DarkColor),
				Data = darkPathData
			};
			Path lightPath = new Path() {
				Fill = new SolidColorBrush(LightColor),
				Data = lightPathData
			};
			Canvas canvas = new Canvas { Width = containerSize.Width, Height = containerSize.Height, Clip = new RectangleGeometry { Rect = new Rect(0, 0, containerSize.Width, containerSize.Height) } };
			canvas.Children.Add(darkPath);
			canvas.Children.Add(lightPath);
			Content = canvas;
		}
	}
}
