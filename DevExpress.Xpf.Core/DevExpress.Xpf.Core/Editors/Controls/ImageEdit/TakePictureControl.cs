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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Services;
namespace DevExpress.Xpf.Editors.Internal {
	public class TakePictureControl : Control {
		static TakePictureControl() {
			var ownerType = typeof(TakePictureControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
		public TakePictureControl() {
			DataContext = new TakePictureViewModel(this);
		}
		ImageEdit Editor { get; set; }
		PopupImageEdit PopupOwner { get; set; }
		public ImageSource Capture() {
			return camera.TakeSnapshot();
		}
		public void Save(ImageSource image) {
			SetImage(image);
			Close();
		}
		void SetImage(ImageSource image) {
			var valueContainer = Editor.PropertyProvider.GetService<ValueContainerService>();
			valueContainer.SetEditValue(image, Validation.Native.UpdateEditorSource.ValueChanging);
			if (PopupOwner != null)
				PopupOwner.Source = image;
		}
		public void Cancel() {
			Close();
		}
		public void SetEditor(ImageEdit edit, PopupImageEdit popupOwner) {
			Editor = edit;
			PopupOwner = popupOwner;
		}
		CameraControl camera;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			camera = LayoutHelper.FindElementByName(this, "PART_Camera") as CameraControl;
		}
		void Close() {
			Window window = FindWindow();
			if (window != null)
				window.Close();
		}
		Window FindWindow() {
			return LayoutHelper.FindLayoutOrVisualParentObject<Window>(this, true);
		}
	}
	public class TakePictureViewModel : ViewModelBase {
		public TakePictureViewModel(TakePictureControl owner) {
			Owner = owner;
			CaptureCaption = EditorLocalizer.GetString(EditorStringId.CameraCaptureButtonCaption);
			SaveCaption = EditorLocalizer.GetString(EditorStringId.Save);
			CancelCaption = EditorLocalizer.GetString(EditorStringId.Cancel);
			CaptureCommand = new DelegateCommand(() => Capture());
			SaveCommand = new DelegateCommand(() => Owner.Save(Image));
			CancelCommand = new DelegateCommand(() => Owner.Cancel());
		}
		TakePictureControl Owner { get; set; }
		public string CaptureCaption {
			get { return GetProperty<string>(() => CaptureCaption); }
			set { SetProperty<string>(() => CaptureCaption, value); }
		}
		public string SaveCaption {
			get { return GetProperty<string>(() => SaveCaption); }
			set { SetProperty<string>(() => SaveCaption, value); }
		}
		public string CancelCaption {
			get { return GetProperty<string>(() => CancelCaption); }
			set { SetProperty<string>(() => CancelCaption, value); }
		}
		public bool HasCapture {
			get { return GetProperty<bool>(() => HasCapture); }
			set { SetProperty<bool>(() => HasCapture, value); }
		}
		public ImageSource Image {
			get { return GetProperty<ImageSource>(() => Image); }
			set {
				SetProperty<ImageSource>(() => Image, value);
				HasCapture = value != null;
				CaptureCaption = HasCapture ? EditorLocalizer.GetString(EditorStringId.CameraAgainButtonCaption) : EditorLocalizer.GetString(EditorStringId.CameraCaptureButtonCaption);
			}
		}
		public ICommand CaptureCommand { get; private set; }
		public ICommand SaveCommand { get; private set; }
		public ICommand CancelCommand { get; private set; }
		void Capture() {
			if (HasCapture)
				Image = null;
			else
				Image = Owner.Capture();
		}
	}
}
