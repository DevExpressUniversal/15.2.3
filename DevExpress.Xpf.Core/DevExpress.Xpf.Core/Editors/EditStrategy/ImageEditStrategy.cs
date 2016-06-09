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
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.EditStrategy;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Data.Mask;
using DevExpress.Xpf.Editors.Automation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors.Validation.Native;
using DevExpress.Xpf.Bars;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Internal;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Editors.Themes;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Utils.Themes;
using System.IO;
#else
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Editors.WPFCompatibility.Extensions;
using DevExpress.WPFToSLUtils;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
#endif
namespace DevExpress.Xpf.Editors {
	public class ImageEditStrategy : EditStrategyBase {
		#region static
		public static ImageSource GetImageFromData(object data) {
			if(data is ImageSource) return (ImageSource)data;
			try {
				return GetImageFromDataCore(data);
			}
			catch {
				return null;
			}
		}
		static ImageSource GetImageFromDataCore(object data) {
#if SL
			if(data is byte[]) 
				return new BytesToImageSourceConverter().Convert(data, null, null, null) as ImageSource;
#endif
			if(data is byte[] || data is Uri || data is string)
				return new ImageSourceConverter().ConvertFrom(data) as ImageSource;
			return null;
		}
		static object GetDataFromImageSource(object baseValue) {
			ImageSource source = baseValue as ImageSource;
#if !SL
			if (source != null) {
				byte[] bytes = ImageLoader.ImageToByteArray(source);
				if (bytes != null) return bytes;
			}
#endif
			return source ?? baseValue;
		}
		#endregion
		public ImageEditStrategy(ImageEdit edit) : base(edit) { }
		protected new ImageEdit Editor { get { return (ImageEdit)base.Editor; } }
		public virtual object CoerceSource(ImageSource value) {
			return CoerceValue(ImageEdit.SourceProperty, value);
		}
		public virtual void OnSourceChanged(ImageSource oldValue, ImageSource newValue) {
			if(ShouldLockUpdate)
				return;
			SyncWithValue(ImageEdit.SourceProperty, oldValue, newValue);
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(BaseEdit.EditValueProperty, baseValue => baseValue, baseValue => ImageEditStrategy.GetDataFromImageSource(baseValue));
			PropertyUpdater.Register(ImageEdit.SourceProperty, baseValue => ((IImageEdit)Editor).GetDataFromImage((ImageSource)baseValue), baseValue => ImageEditStrategy.GetImageFromData(baseValue));
		}
		public virtual void SetImage(ImageSource imageSource) {
			Editor.SetCurrentValue(ImageEdit.SourceProperty, imageSource);
		}
	}
	public class PopupImageEditStrategy : PopupBaseEditStrategy {
		PostponedAction UpdateBaseUriOnLoadedAction { get; set; }
		public PopupImageEditStrategy(PopupImageEdit edit) : base(edit) {
			UpdateBaseUriOnLoadedAction = new PostponedAction(() => !Editor.IsLoaded);
		}
		protected new PopupImageEdit Editor { get { return (PopupImageEdit)base.Editor; } }
		protected ImageEdit ImageEditControl { get { return Editor.ImageEditControl; } }
		protected override void SyncWithValueInternal() {
			Editor.HasImage = Editor.Source != null;
			if(ImageEditControl != null) {
				ImageEditControl.ShowMenu = Editor.ShowMenu;
				ImageEditControl.ShowMenuMode = Editor.ShowMenuMode;
				if(Editor.EmptyContentTemplate != null)
					ImageEditControl.EmptyContentTemplate = Editor.EmptyContentTemplate;
				if(Editor.MenuTemplate != null)
					ImageEditControl.MenuTemplate = Editor.MenuTemplate;
				if(Editor.MenuContainerTemplate != null)
					ImageEditControl.MenuContainerTemplate = Editor.MenuContainerTemplate;
				ImageEditControl.ShowLoadDialogOnClickMode = Editor.ShowLoadDialogOnClickMode;
				ImageEditControl.Source = Editor.Source;
				ImageEditControl.IsReadOnly = Editor.IsReadOnly;
			}
		}
		public virtual object CoerceSource(ImageSource value) {
			return CoerceValue(PopupImageEdit.SourceProperty, value);
		}
		public virtual void SourceChanged(ImageSource oldSource, ImageSource newSource) {
#if !SL
			UpdateBaseUriOnLoadedAction.PerformPostpone(() => {
				Editor.UpdateBaseUri();
			});
#endif
			if(ShouldLockUpdate)
				return;
			SyncWithValue(PopupImageEdit.SourceProperty, oldSource, newSource);
		}
		public virtual void AcceptPopupValue() {
			if(ImageEditControl == null)
				return;
			ValueContainer.SetEditValue(ImageEditControl.EditValue, UpdateEditorSource.ValueChanging);
			UpdateDisplayText();
		}
		public override void OnLoaded() {
			base.OnLoaded();
			UpdateBaseUriOnLoadedAction.PerformForce();
		}
		protected override void RegisterUpdateCallbacks() {
			base.RegisterUpdateCallbacks();
			PropertyUpdater.Register(PopupImageEdit.SourceProperty, baseValue => ((IImageEdit)Editor).GetDataFromImage((ImageSource)baseValue), baseValue => ImageEditStrategy.GetImageFromData(baseValue));
		}
	}
}
