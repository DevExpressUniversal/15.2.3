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

#if SILVERLIGHT
extern alias Platform;
#endif
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.Features;
using Microsoft.Windows.Design.PropertyEditing;
using System;
using System.Linq;
using Microsoft.Windows.Design.Model;
using DevExpress.Xpf.Design;
using Microsoft.Windows.Design.Interaction;
using Microsoft.Windows.Design;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Windows.Design.Policies;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using System.Collections.Specialized;
using Microsoft.Windows.Design.Services;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows.Interop;
using System.Reflection;
using DevExpress.Xpf.Core.Design;
using DevExpress.Xpf.Core.Design.Wizards.ItemsSourceWizard;
using DevExpress.Design.UI;
#if SILVERLIGHT
using DevExpress.Xpf.Core.Design.CoreUtils;
using DevExpress.Utils.Design;
using Platform::DevExpress.Xpf.Editors.Settings;
using Platform::DevExpress.Xpf.Grid;
using Platform::DevExpress.Xpf.Grid.Native;
using Platform::DevExpress.Data;
using Platform::DevExpress.Xpf.Bars.Helpers;
using Platform::DevExpress.Utils;
using Platform::DevExpress.Xpf.Grid.LookUp;
#else
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Data;
using DevExpress.Xpf.Bars.Helpers;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Core;
using DevExpress.Utils;
using DevExpress.Xpf.Grid.LookUp;
#endif
namespace DevExpress.Xpf.Grid.Design {
	public class ChangePropertyTypeInfo : DependencyObject {
		public static readonly DependencyProperty IsEnabledProperty =
			DependencyProperty.Register("IsEnabled", typeof(bool), typeof(ChangePropertyTypeInfo), new UIPropertyMetadata(true));
		public ChangePropertyTypeInfo(ICommand command, Type type, ImageSource image) {
			Command = command;
			Type = type;
			Image = image;
		}
		public ICommand Command { get; private set; }
		public Type Type { get; private set; }
		public ImageSource Image { get; private set; }
		public bool IsEnabled {
			get { return (bool)GetValue(IsEnabledProperty); }
			set { SetValue(IsEnabledProperty, value); }
		}
	}
	public class DesignTimeModel : DependencyObject {
		public static readonly DependencyProperty IsViewSetProperty =
			DependencyProperty.Register("IsViewSet", typeof(bool), typeof(DesignTimeModel), new PropertyMetadata(false));
		public static readonly DependencyProperty GridViewInfoProperty =
			DependencyProperty.Register("GridViewInfo", typeof(string), typeof(DesignTimeModel), new PropertyMetadata(null));
		public static readonly DependencyProperty GridControlInfoProperty =
			DependencyProperty.Register("GridControlInfo", typeof(string), typeof(DesignTimeModel), new UIPropertyMetadata(null));
		public static readonly DependencyProperty IsGridSelectedProperty =
			DependencyProperty.Register("IsGridSelected", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsViewSelectedProperty =
			DependencyProperty.Register("IsViewSelected", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty GridColumnInfoProperty =
			DependencyProperty.Register("GridColumnInfo", typeof(string), typeof(DesignTimeModel), new UIPropertyMetadata(null));
		public static readonly DependencyProperty GridBandInfoProperty =
			DependencyProperty.Register("GridBandInfo", typeof(string), typeof(DesignTimeModel), new UIPropertyMetadata(null));
		public static readonly DependencyProperty IsColumnSelectedProperty =
			DependencyProperty.Register("IsColumnSelected", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsBandSelectedProperty =
	DependencyProperty.Register("IsBandSelected", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsEditSettingsSetProperty =
			DependencyProperty.Register("IsEditSettingsSet", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsEditSettingsSelectedProperty =
			DependencyProperty.Register("IsEditSettingsSelected", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsColumnControlPanelVisibleProperty =
			DependencyProperty.Register("IsColumnControlPanelVisible", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsBandControlPanelVisibleProperty =
			DependencyProperty.Register("IsBandControlPanelVisible", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty EditSettingsInfoProperty =
			DependencyProperty.Register("EditSettingsInfo", typeof(string), typeof(DesignTimeModel), new UIPropertyMetadata(null));
		public static readonly DependencyProperty IsPanelLeftAlignedProperty =
			DependencyProperty.Register("IsPanelLeftAligned", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(false));
		public static readonly DependencyProperty IsAdornerPanelExpandedProperty =
			DependencyProperty.Register("IsAdornerPanelExpanded", typeof(bool), typeof(DesignTimeModel), new UIPropertyMetadata(true, (d, e) => ((DesignTimeModel)d).OnIsAdornerPanelExpandedChanged()));
		public static readonly DependencyProperty EditSettingsIconProperty =
			DependencyProperty.Register("EditSettingsIcon", typeof(ImageSource), typeof(DesignTimeModel), new UIPropertyMetadata(null));
		public static readonly DependencyProperty ViewIconProperty =
			DependencyProperty.Register("ViewIcon", typeof(ImageSource), typeof(DesignTimeModel), new UIPropertyMetadata(null));
		public static readonly DependencyProperty DataSourceInfoCollectionProviderProperty =
			DependencyProperty.Register("DataSourceInfoCollectionProvider", typeof(IDataAccessTechnologyCollectionProvider), typeof(DesignTimeModel));
		private static readonly DependencyPropertyKey ProviderPropertyKey =
			DependencyProperty.RegisterReadOnly("Provider", typeof(DataControlAdornerProvider), typeof(DesignTimeModel), null);
		public static readonly DependencyProperty ProviderProperty = ProviderPropertyKey.DependencyProperty;
		public static readonly DependencyProperty ChangeViewInfoProperty = 
			DependencyProperty.Register("ChangeViewInfo", typeof(List<ChangePropertyTypeInfo>), typeof(DesignTimeModel), new PropertyMetadata(null));
		public static readonly DependencyProperty ChangeEditSettingsInfoProperty = 
			DependencyProperty.Register("ChangeEditSettingsInfo", typeof(List<ChangePropertyTypeInfo>), typeof(DesignTimeModel), new PropertyMetadata(null));
		internal static BitmapSource GetImageByViewType(Type type) {
			return GetImageByType(type, "Views");
		}
		internal static BitmapSource GetImageByEditSettingsType(Type type) {
			return GetImageByType(type, "EditSettings");
		}
		static BitmapSource GetImageByType(Type type, string folderName) {
			string resource = string.Format("{0}.Images.{1}.{2}.png", typeof(DesignTimeModel).Namespace, folderName, type.Name);
#if !SILVERLIGHT
			return DevExpress.Xpf.Core.Native.ImageHelper.CreateImageFromEmbeddedResource(
#else
			return DevExpress.Xpf.Core.Design.CoreUtils.ImageHelper.CreateImageFromEmbeddedResource(
#endif
				Assembly.GetExecutingAssembly(), resource);
		}
		public DataControlAdornerProvider Provider {
			get { return (DataControlAdornerProvider)GetValue(ProviderProperty); }
			private set { SetValue(ProviderPropertyKey, value); }
		}
		public bool IsViewSet {
			get { return (bool)GetValue(IsViewSetProperty); }
			set { SetValue(IsViewSetProperty, value); }
		}
		public bool IsEditSettingsSet {
			get { return (bool)GetValue(IsEditSettingsSetProperty); }
			set { SetValue(IsEditSettingsSetProperty, value); }
		}
		public string GridViewInfo {
			get { return (string)GetValue(GridViewInfoProperty); }
			set { SetValue(GridViewInfoProperty, value); }
		}
		public string GridControlInfo {
			get { return (string)GetValue(GridControlInfoProperty); }
			set { SetValue(GridControlInfoProperty, value); }
		}
		public bool IsViewSelected {
			get { return (bool)GetValue(IsViewSelectedProperty); }
			set { SetValue(IsViewSelectedProperty, value); }
		}
		public bool IsGridSelected {
			get { return (bool)GetValue(IsGridSelectedProperty); }
			set { SetValue(IsGridSelectedProperty, value); }
		}
		public bool IsColumnSelected {
			get { return (bool)GetValue(IsColumnSelectedProperty); }
			set { SetValue(IsColumnSelectedProperty, value); }
		}
		public bool IsBandSelected {
			get { return (bool)GetValue(IsBandSelectedProperty); }
			set { SetValue(IsBandSelectedProperty, value); }
		}
		public string GridColumnInfo {
			get { return (string)GetValue(GridColumnInfoProperty); }
			set { SetValue(GridColumnInfoProperty, value); }
		}
		public string GridBandInfo {
			get { return (string)GetValue(GridBandInfoProperty); }
			set { SetValue(GridBandInfoProperty, value); }
		}
		public bool IsEditSettingsSelected {
			get { return (bool)GetValue(IsEditSettingsSelectedProperty); }
			set { SetValue(IsEditSettingsSelectedProperty, value); }
		}
		public bool IsColumnControlPanelVisible {
			get { return (bool)GetValue(IsColumnControlPanelVisibleProperty); }
			set { SetValue(IsColumnControlPanelVisibleProperty, value); }
		}
		public bool IsBandControlPanelVisible {
			get { return (bool)GetValue(IsBandControlPanelVisibleProperty); }
			set { SetValue(IsBandControlPanelVisibleProperty, value); }
		}
		public string EditSettingsInfo {
			get { return (string)GetValue(EditSettingsInfoProperty); }
			set { SetValue(EditSettingsInfoProperty, value); }
		}
		public bool IsPanelLeftAligned {
			get { return (bool)GetValue(IsPanelLeftAlignedProperty); }
			set { SetValue(IsPanelLeftAlignedProperty, value); }
		}
		public bool IsAdornerPanelExpanded {
			get { return (bool)GetValue(IsAdornerPanelExpandedProperty); }
			set { SetValue(IsAdornerPanelExpandedProperty, value); }
		}
		public ImageSource ViewIcon {
			get { return (ImageSource)GetValue(ViewIconProperty); }
			set { SetValue(ViewIconProperty, value); }
		}
		public ImageSource EditSettingsIcon {
			get { return (ImageSource)GetValue(EditSettingsIconProperty); }
			set { SetValue(EditSettingsIconProperty, value); }
		}
		public IDataAccessTechnologyCollectionProvider DataSourceInfoCollectionProvider {
			get { return (IDataAccessTechnologyCollectionProvider)GetValue(DataSourceInfoCollectionProviderProperty); }
			set { SetValue(DataSourceInfoCollectionProviderProperty, value); }
		}
		public List<ChangePropertyTypeInfo> ChangeViewInfo {
			get { return (List<ChangePropertyTypeInfo>)GetValue(ChangeViewInfoProperty); }
			set { SetValue(ChangeViewInfoProperty, value); }
		}
		public List<ChangePropertyTypeInfo> ChangeEditSettingsInfo {
			get { return (List<ChangePropertyTypeInfo>)GetValue(ChangeEditSettingsInfoProperty); }
			set { SetValue(ChangeEditSettingsInfoProperty, value); }
		}
		public ICommand SelectViewCommand { get; private set; }
		public ICommand SelectGridCommand { get; private set; }
		public ICommand SelectColumnCommand { get; private set; }
		public ICommand SelectBandCommand { get; private set; }
		public ICommand SelectEditSettingsCommand { get; private set; }
		public ICommand DeleteSelectedColumnCommand { get; private set; }
		public ICommand DeleteSelectedBandCommand { get; private set; }
		public ICommand AddColumnCommand { get; private set; }
		public ICommand PopulateColumnsCommand { get; private set; }
		public bool IsAddColumnButtonVisible { get; private set; }
		public ImageSource ControlIcon { get; private set; }
		public DesignTimeModel(DataControlAdornerProvider provider) {
			Provider = provider;
			SelectViewCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.SelectView(), null, false);
			SelectGridCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.SelectGrid(), null, false);
			SelectColumnCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.SelectColumn(), null, false);
			SelectBandCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.SelectBand(), null, false);
			SelectEditSettingsCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.SelectEditSettings(), null, false);
			DeleteSelectedColumnCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.DeleteSelectedColumn(), null, false);
			DeleteSelectedBandCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.DeleteSelectedBand(), null, false);
			AddColumnCommand = (ICommand)new WpfDelegateCommand<object>(obj => provider.AddColumn(), null, false);
			UpdateChangeViewInfo(provider);
			ChangeEditSettingsInfo = CreateChangeTypeInfo((ICommand)new WpfDelegateCommand<Type>(type => provider.ChangeEditSettings(type), false), TypeLists.EditSettingsTypes.Where(t => (t.Item2 & PropertyTarget.Grid) != 0).Select(t => t.Item1).ToArray(), GetImageByEditSettingsType);
			IsAddColumnButtonVisible = DataControlAdornerProvider.CanSelectColumn;
			ControlIcon = GetImageByType(provider.ControlType, "DesignPanel");
			DataSourceInfoCollectionProvider = new GridDataAccessTechnologyCollectionProvider(provider);
		}
		public void UpdateChangeViewInfo(DataControlAdornerProvider provider) {
			ChangeViewInfo = CreateChangeTypeInfo((ICommand)new WpfDelegateCommand<Type>(type => provider.ChangeViewType(type), false), provider.AvailableViewTypes, GetImageByViewType);
		}
		List<ChangePropertyTypeInfo> CreateChangeTypeInfo(ICommand command, Type[] types, Func<Type, ImageSource> imageCreator) {
			List<ChangePropertyTypeInfo> list = new List<ChangePropertyTypeInfo>();
			foreach(Type type in types) {
				list.Add(new ChangePropertyTypeInfo(command, type, imageCreator(type)));
			}
			return list;
		}
		void OnIsAdornerPanelExpandedChanged() {
			DataControlAdornerProvider.SetIsAdornerPanelExpanded(Provider.DataControl, IsAdornerPanelExpanded);
		}
	}
	public class GridViewAdornerPanel : System.Windows.Controls.Grid {
		readonly DataViewBaseAdornerProvider provider;
		public GridViewAdornerPanel(DataViewBaseAdornerProvider provider) {
			this.provider = provider;
		}
		protected override System.Windows.Media.HitTestResult HitTestCore(System.Windows.Media.PointHitTestParameters hitTestParameters) {
			DataViewBase view = provider.AdornedElement.View.PlatformObject as DataViewBase;
			if(view != null && !GridControlHelper.GetDesignTimeAdorner(view).IsSelectGridArea(DesignHelper.ConvertToPlatformPoint(hitTestParameters.HitPoint)))
				return null;
			return base.HitTestCore(hitTestParameters);
		}
	}
}
