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
using System.Collections;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Windows.Markup;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Globalization;
using System.Windows.Automation.Peers;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Editors.Settings.Extension;
using DevExpress.Xpf.Bars;
using System.Windows.Media.Imaging;
using System.IO;
using System.Windows.Media;
using Microsoft.Win32;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using System.Windows.Interop;
using System.Windows.Media.Effects;
#else
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.Data.Mask;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Editors.Automation;
#endif
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using System.Windows.Media.Imaging;
using System.IO;
using DevExpress.Xpf.Core.Native;
#endif
namespace DevExpress.Xpf.Editors.Internal {
	public interface IImageEdit : IInputElement {
		object GetDataFromImage(ImageSource source);
		void Load();
#if !SL
		void Save();
#endif
	}
	public class ImageLoader {
		public static ImageSource LoadImage() {
#if !SL
			if(BrowserInteropHelper.IsBrowserHosted)
				return null;
#endif
			try {
				return LoadImageCore();
			}
#if !SL
			catch(Exception e) {
				if(e is NotSupportedException)
#else
			catch {
#endif
				MessageBoxHelper.ShowError(EditorLocalizer.GetString(EditorStringId.ImageEdit_InvalidFormatMessage), EditorLocalizer.GetString(EditorStringId.CaptionError), MessageBoxButton.OK);
				return null;
			}
		}
		static ImageSource LoadImageCore() {
			OpenFileDialog dlg = new OpenFileDialog();
#if !SL
			dlg.Filter = EditorLocalizer.GetString(EditorStringId.ImageEdit_OpenFileFilter);
#else
			dlg.Filter = EditorLocalizer.GetString(EditorStringId.ImageEdit_OpenFileFilter_SL);
#endif
			if(dlg.ShowDialog() == true) {
#if !SL
				using(Stream stream = dlg.OpenFile()) {
#else
				using (Stream stream = dlg.File.OpenRead()) {
#endif
					MemoryStream ms = new MemoryStream(stream.GetDataFromStream());
					return ImageHelper.CreateImageFromStream(ms);
				}
			}
			return null;
		}
#if !SL
		public static void SaveImage(BitmapSource image) {
			if(image == null || BrowserInteropHelper.IsBrowserHosted) return;
			SaveImageCore(image);
		}
		static void SaveImageCore(BitmapSource image) {
			SaveFileDialog dlg = new SaveFileDialog();
			dlg.Filter = EditorLocalizer.GetString(EditorStringId.ImageEdit_SaveFileFilter);
			if(dlg.ShowDialog() == true) {
				using(Stream stream = dlg.OpenFile()) {
					BitmapEncoder encoder = GetEncoderByFilterIndex(dlg.FilterIndex);
					encoder.Frames.Add(BitmapFrame.Create(image));
					encoder.Save(stream);
				}
			}
		}
		public static BitmapSource GetSafeBitmapSource(BitmapSource source, Effect effect) {
			if(effect == null)
				return source;
			DrawingVisual drawingVisual = new DrawingVisual() { Effect = effect };
			DrawingContext drawingContext = drawingVisual.RenderOpen();
			drawingContext.DrawImage(source, new Rect(0, 0, source.Width, source.Height));
			drawingContext.Close();
			RenderTargetBitmap rtb = new RenderTargetBitmap(source.PixelWidth, source.PixelHeight, 96, 96, PixelFormats.Pbgra32);
			rtb.Render(drawingVisual);
			return rtb;
		}
		static BitmapEncoder GetEncoderByFilterIndex(int index) {
			switch(index) {
				case 2:
					return new BmpBitmapEncoder();
				case 3:
					return new JpegBitmapEncoder();
				case 4:
					return new GifBitmapEncoder();
			}
			return new PngBitmapEncoder();
		}
		public static byte[] ImageToByteArray(ImageSource source) {
			try {
				return ImageToByArrayCore(source);
			}
			catch {
				return null;
			}
		}
		static byte[] ImageToByArrayCore(ImageSource source) {
			BitmapDecoder decoder = CreateDecoder(source);
			BitmapEncoder encoder = CreateEncoder(decoder, source);
			if (!Initialize(encoder, decoder, source))
				return null;
			using (MemoryStream stream = new MemoryStream()) {
				encoder.Save(stream);
				return stream.ToArray();
			}
		}
		static bool Initialize(BitmapEncoder encoder, BitmapDecoder decoder, ImageSource source) {
			if (encoder == null)
				return false;
			if (decoder != null) {
				foreach (var frame in decoder.Frames)
					encoder.Frames.Add(BitmapFrame.Create(frame));
				return true;
			}
			BitmapSource bitmapSource = source as BitmapSource;
			if (bitmapSource != null) {
				encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
				return true;
			}
			return false;
		}
		static BitmapEncoder CreateEncoder(BitmapDecoder decoder, ImageSource source) {
			if (decoder != null && decoder.CodecInfo != null)
				return BitmapEncoder.Create(decoder.CodecInfo.ContainerFormat);
			BitmapSource bitmapSource = source as BitmapSource;
			if (bitmapSource != null)
				return new BmpBitmapEncoder();
			return null;
		}
		static BitmapDecoder CreateDecoder(ImageSource imageSource) {
			if (imageSource == null)
				return null;
			BitmapImage bitmapImage = imageSource as BitmapImage;
			if (bitmapImage != null) {
				return CreateDecoder(bitmapImage);
			}
			BitmapFrame bitmapFrame = imageSource as BitmapFrame;
			if (bitmapFrame != null)
				return bitmapFrame.Decoder;
			return null;
		}
		static BitmapDecoder CreateDecoder(BitmapImage bitmapImage) {
			if (bitmapImage == null)
				return null;
			if (bitmapImage.StreamSource != null && bitmapImage.StreamSource.CanRead) {
				bitmapImage.StreamSource.Seek(0, SeekOrigin.Begin);
				return BitmapDecoder.Create(bitmapImage.StreamSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			}
			if (bitmapImage.UriSource != null) {
				return BitmapDecoder.Create(bitmapImage.UriSource, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);
			}
			return null;
		}
#endif
	}
}
