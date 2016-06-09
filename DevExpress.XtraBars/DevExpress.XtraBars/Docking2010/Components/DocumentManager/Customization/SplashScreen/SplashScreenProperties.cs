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
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
using DevExpress.XtraEditors.ButtonPanel;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface ISplashScreenProperties : DevExpress.Utils.Base.IBaseProperties {
		AppearanceObject AppearanceCaption { get; }
		AppearanceObject AppearanceLoadingDescription { get; }
		Image Image { get; set; }
		string Caption { get; set; }
		string LoadingDescription { get; set; }
		bool ShowImage { get; set; }
		bool ShowCaption { get; set; }
		bool ShowLoadingDescription { get; set; }
		ImageLocation ImageLocation {get;set;}
		int ImageToCaptionDistance { get; set; }
		int? CaptionToLoadingElementsDistance { get; set; }
	}
	class SplashScreenProperties : BaseProperties, ISplashScreenProperties {
		AppearanceObject appearanceCaptionCore;
		AppearanceObject appearanceLoadingDescriptionCore;
		public SplashScreenProperties() {
			SetDefaultValueCore("ShowImage", true);
			SetDefaultValueCore("ShowCaption", true);
			SetDefaultValueCore("ShowLoadingDescription", true);
			SetDefaultValueCore("ImageToCaptionDistance", 50);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new SplashScreenProperties();
		}
		protected override void OnCreate() {
			base.OnCreate();
			appearanceCaptionCore = new AppearanceObject();
			appearanceLoadingDescriptionCore = new AppearanceObject();
			AppearanceCaption.Changed += OnAppearanceCaptionChanged;
			AppearanceLoadingDescription.Changed += OnAppearanceLoadingDescriptionChanged;
		}
		protected override void OnDispose() {
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			AppearanceLoadingDescription.Changed -= OnAppearanceLoadingDescriptionChanged;
			Ref.Dispose(ref appearanceCaptionCore);
			Ref.Dispose(ref appearanceLoadingDescriptionCore);
			base.OnDispose();
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceCaption");
		}
		void OnAppearanceLoadingDescriptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceLoadingDescription");
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceCaption {
			get { return appearanceCaptionCore; }
		}
		bool ShouldSerializeAppearanceCaption() {
			return (AppearanceCaption != null) && AppearanceCaption.ShouldSerialize();
		}
		void ResetAppearanceCaption() {
			AppearanceCaption.Reset();
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject AppearanceLoadingDescription {
			get { return appearanceLoadingDescriptionCore; }
		}
		bool ShouldSerializeAppearanceLoadingDescription() {
			return (AppearanceLoadingDescription != null) && AppearanceLoadingDescription.ShouldSerialize();
		}
		void ResetAppearanceDescription() {
			AppearanceLoadingDescription.Reset();
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance), Editor("DevExpress.Utils.Design.DXImageEditor, " + AssemblyInfo.SRAssemblyDesign, typeof(System.Drawing.Design.UITypeEditor))]
		public Image Image {
			get { return GetValueCore<Image>("Image"); }
			set { SetValueCore("Image", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance), Localizable(true)]
		public string Caption {
			get { return GetValueCore<string>("Caption"); }
			set { SetValueCore("Caption", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance), Localizable(true)]
		public string LoadingDescription {
			get { return GetValueCore<string>("LoadingDescription"); }
			set { SetValueCore("LoadingDescription", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowImage {
			get { return GetValueCore<bool>("ShowImage"); }
			set { SetValueCore("ShowImage", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowCaption {
			get { return GetValueCore<bool>("ShowCaption"); }
			set { SetValueCore("ShowCaption", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowLoadingDescription {
			get { return GetValueCore<bool>("ShowLoadingDescription"); }
			set { SetValueCore("ShowLoadingDescription", value); }
		}
		[DefaultValue(ImageLocation.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public ImageLocation ImageLocation {
			get { return GetValueCore<ImageLocation>("ImageLocation"); }
			set { SetValueCore("ImageLocation", value); }
		}
		[DefaultValue(50), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int ImageToCaptionDistance {
			get { return GetValueCore<int>("ImageToCaptionDistance"); }
			set { SetValueCore("ImageToCaptionDistance", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int? CaptionToLoadingElementsDistance {
			get { return GetValueCore<int?>("CaptionToLoadingElementsDistance"); }
			set { SetValueCore("CaptionToLoadingElementsDistance", value); }
		}
		protected override void AssignCore(DevExpress.Utils.Base.IPropertiesProvider source) {
			base.AssignCore(source);
			ISplashScreenProperties sourceProperties = source as ISplashScreenProperties;
			if(source != null) {
				AppearanceCaption.AssignInternal(sourceProperties.AppearanceCaption);
				AppearanceLoadingDescription.AssignInternal(sourceProperties.AppearanceLoadingDescription);
			}
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				AppearanceCaption.ShouldSerialize() ||
				AppearanceLoadingDescription.ShouldSerialize();
		}
		protected override void ResetCore() {
			base.ResetCore();
			AppearanceCaption.Reset();
			AppearanceLoadingDescription.Reset();
		}
	}
}
