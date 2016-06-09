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
using DevExpress.Utils;
using DevExpress.Xpf.Utils;
using DevExpress.Utils.Extensions.Helpers;
using DevExpress.Utils.Design;
using System.Windows.Input;
namespace DevExpress.Xpf.Core.Design.SmartTags {
	public sealed class DesignTimeImagePicker : MvvmControlBase<DesignTimeImagePickerMainViewModel, DesignTimeImagePickerMainView>, IDesignTimeImagePicker {
		#region Dependency Properties
		public static readonly DependencyProperty GroupsProperty =
			DependencyPropertyManager.Register("Groups", typeof(IEnumerable<IDesignTimeImagePickerGroupInfo>), typeof(DesignTimeImagePicker), new PropertyMetadata(null,
				(d, e) => ((DesignTimeImagePicker)d).OnGroupsChanged(e)));
		public static readonly DependencyProperty DataProviderProperty =
			DependencyPropertyManager.Register("DataProvider", typeof(IDesignTimeImagePickerDataProvier), typeof(DesignTimeImagePicker), new PropertyMetadata(null,
				(d, e) => ((DesignTimeImagePicker)d).OnDataProviderChanged(e)));
		public static readonly DependencyProperty SelectedImageProperty =
			DependencyProperty.Register("SelectedImage", typeof(IDesignTimeImagePickerImageInfo), typeof(DesignTimeImagePicker), new PropertyMetadata(null,
				(d, e) => ((DesignTimeImagePicker)d).OnSelectedImageChanged(e)));
		public static readonly DependencyProperty CloseCommandProperty =
			DependencyProperty.Register("CloseCommand", typeof(ICommand), typeof(DesignTimeImagePicker), new PropertyMetadata(null));
		public static readonly DependencyProperty CloseCommandParameterProperty =
			DependencyProperty.Register("CloseCommandParameter", typeof(object), typeof(DesignTimeImagePicker), new PropertyMetadata(null));
		public static readonly DependencyProperty PropertyNameProperty =
		   DependencyProperty.Register("PropertyName", typeof(string), typeof(DesignTimeImagePicker), new PropertyMetadata(null,
			   (d, e) => ((DesignTimeImagePicker)d).OnPropertyNameChanged(e)));
		public static readonly DependencyProperty SelectedImageOriginalStringProperty =
			DependencyProperty.Register("SelectedImageOriginalString", typeof(string), typeof(DesignTimeImagePicker), new PropertyMetadata(null,
				(d, e) => ((DesignTimeImagePicker)d).OnSelectedImageOriginalStringChanged(e)));
		#endregion
		WeakEventHandler<ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier>, EventHandler<ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier>>> dataProviderChanged;
		WeakEventHandler<ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>>, EventHandler<ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>>>> groupsChanged;
		WeakEventHandler<EventArgs, EventHandler> selectedImageChanged;
		WeakEventHandler<EventArgs, EventHandler> propertyNameChanged;
		WeakEventHandler<EventArgs, EventHandler> selectedImageOriginalStringChanged;
		public DesignTimeImagePicker() {
			InitializeComponent();
		}
		public IDesignTimeImagePickerDataProvier DataProvider { get { return (IDesignTimeImagePickerDataProvier)GetValue(DataProviderProperty); } set { SetValue(DataProviderProperty, value); } }
		public IEnumerable<IDesignTimeImagePickerGroupInfo> Groups { get { return (IEnumerable<IDesignTimeImagePickerGroupInfo>)GetValue(GroupsProperty); } set { SetValue(GroupsProperty, value); } }
		public IDesignTimeImagePickerImageInfo SelectedImage { get { return (IDesignTimeImagePickerImageInfo)GetValue(SelectedImageProperty); } set { SetValue(SelectedImageProperty, value); } }
		public ICommand CloseCommand { get { return (ICommand)GetValue(CloseCommandProperty); } set { SetValue(CloseCommandProperty, value); } }
		public object CloseCommandParameter { get { return GetValue(CloseCommandParameterProperty); } set { SetValue(CloseCommandParameterProperty, value); } }
		public string PropertyName { get { return (string)GetValue(PropertyNameProperty); } set { SetValue(PropertyNameProperty, value); } }
		public string SelectedImageOriginalString { get { return (string)GetValue(SelectedImageOriginalStringProperty); } set { SetValue(SelectedImageOriginalStringProperty, value); } }
		public event EventHandler<ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier>> DataProviderChanged { add { dataProviderChanged += value; } remove { dataProviderChanged -= value; } }
		public event EventHandler<ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>>> GroupsChanged { add { groupsChanged += value; } remove { groupsChanged -= value; } }
		public event EventHandler SelectedImageChanged { add { selectedImageChanged += value; } remove { selectedImageChanged -= value; } }
		public event EventHandler PropertyNameChanged { add { propertyNameChanged += value; } remove { propertyNameChanged -= value; } }
		public event EventHandler SelectedImageOriginalStringChanged { add { selectedImageOriginalStringChanged += value; } remove { selectedImageOriginalStringChanged -= value; } }
		protected override DesignTimeImagePickerMainViewModel CreateMainViewModel() { return new DesignTimeImagePickerMainViewModel(this); }
		void OnDataProviderChanged(DependencyPropertyChangedEventArgs e) {
			dataProviderChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IDesignTimeImagePickerDataProvier>(e));
		}
		void OnGroupsChanged(DependencyPropertyChangedEventArgs e) {
			groupsChanged.SafeRaise(this, new ThePropertyChangedEventArgs<IEnumerable<IDesignTimeImagePickerGroupInfo>>(e));
		}
		void OnSelectedImageChanged(DependencyPropertyChangedEventArgs e) {
			selectedImageChanged.SafeRaise(this, EventArgs.Empty);
		}
		void OnPropertyNameChanged(DependencyPropertyChangedEventArgs e) {
			propertyNameChanged.SafeRaise(this, EventArgs.Empty);
		}
		void OnSelectedImageOriginalStringChanged(DependencyPropertyChangedEventArgs e) {
			selectedImageOriginalStringChanged.SafeRaise(this, EventArgs.Empty);
		}
	}
	public interface IDesignTimeImagePickerDataProvier {
		void Load();
		IEnumerable<IDesignTimeImagePickerImageInfo> Images { get; }
		event EventHandler ImagesChanged;
	}
	public interface IDesignTimeImagePickerGroupInfo {
		string Name { get; }
	}
	public interface IDesignTimeImagePickerImageInfo {
		string Group { get; }
		string Name { get; }
		Uri Uri { get; }
		ImageType? ImageType { get; }
		ImageSize Size { get; }
		IEnumerable<string> Tags { get; }
	}
}
