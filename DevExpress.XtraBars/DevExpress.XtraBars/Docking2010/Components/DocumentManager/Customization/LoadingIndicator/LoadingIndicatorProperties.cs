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
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Customization {
	public interface ILoadingIndicatorProperties : DevExpress.Utils.Animation.IWaitingIndicatorProperties {
		string DescriptionFormat { get; set; }
		int MinShowingTime { get; set; }
	}
	class LoadingIndicatorProperties : BaseProperties, ILoadingIndicatorProperties {
		AppearanceObject appearanceCaptionCore;
		AppearanceObject appearanceDescriptionCore;
		public LoadingIndicatorProperties() {
			SetDefaultValueCore("ShowCaption", true);
			SetDefaultValueCore("ShowDescription", true);
			SetDefaultValueCore("CaptionToDescriptionDistance", 4);
			SetDefaultValueCore("ImageToTextDistance", 4);
			SetDefaultValueCore("MinShowingTime", 300);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new LoadingIndicatorProperties();
		}
		protected override void OnCreate() {
			base.OnCreate();
			appearanceCaptionCore = new AppearanceObject();
			appearanceDescriptionCore = new AppearanceObject();
			AppearanceCaption.Changed += OnAppearanceCaptionChanged;
			AppearanceDescription.Changed += OnAppearanceDescriptionChanged;
		}
		protected override void OnDispose() {
			AppearanceCaption.Changed -= OnAppearanceCaptionChanged;
			AppearanceDescription.Changed -= OnAppearanceDescriptionChanged;
			Ref.Dispose(ref appearanceCaptionCore);
			Ref.Dispose(ref appearanceDescriptionCore);
			base.OnDispose();
		}
		void OnAppearanceCaptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceCaption");
		}
		void OnAppearanceDescriptionChanged(object sender, EventArgs e) {
			OnObjectChanged("AppearanceDescription");
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
		public AppearanceObject AppearanceDescription {
			get { return appearanceDescriptionCore; }
		}
		bool ShouldSerializeAppearanceDescription() {
			return (AppearanceDescription != null) && AppearanceDescription.ShouldSerialize();
		}
		void ResetAppearanceDescription() {
			AppearanceDescription.Reset();
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance), Localizable(true)]
		public string Caption {
			get { return GetValueCore<string>("Caption"); }
			set { SetValueCore("Caption", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance), Localizable(true)]
		public string Description {
			get { return GetValueCore<string>("Description"); }
			set { SetValueCore("Description", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public string DescriptionFormat {
			get { return GetValueCore<string>("DescriptionFormat"); }
			set { SetValueCore("DescriptionFormat", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowCaption {
			get { return GetValueCore<bool>("ShowCaption"); }
			set { SetValueCore("ShowCaption", value); }
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Appearance)]
		public bool ShowDescription {
			get { return GetValueCore<bool>("ShowDescription"); }
			set { SetValueCore("ShowDescription", value); }
		}
		[XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Layout)]
		public System.Drawing.Size ContentMinSize {
			get { return GetValueCore<System.Drawing.Size>("ContentMinSize"); }
			set { SetValueCore("ContentMinSize", value); }
		}
		bool ShouldSerializeContentMinSize() { return !IsDefault("ContentMinSize"); }
		void ResetContentMinSize() { Reset("ContentMinSize"); }
		[DefaultValue(4), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int ImageToTextDistance {
			get { return GetValueCore<int>("ImageToTextDistance"); }
			set { SetValueCore("ImageToTextDistance", value); }
		}
		[DefaultValue(4), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int CaptionToDescriptionDistance {
			get { return GetValueCore<int>("CaptionToDescriptionDistance"); }
			set { SetValueCore("CaptionToDescriptionDistance", value); }
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int ImageOffset {
			get { return GetValueCore<int>("ImageOffset"); }
			set { SetValueCore("ImageOffset", value); }
		}
		[DefaultValue(300), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category(XtraEditors.CategoryName.Behavior)]
		public int MinShowingTime {
			get { return GetValueCore<int>("MinShowingTime"); }
			set { SetValueCore("MinShowingTime", value); }
		}
		bool DevExpress.Utils.Animation.IWaitingIndicatorProperties.AllowBackground {
			get { return true; }
			set { }
		}
		protected override void AssignCore(DevExpress.Utils.Base.IPropertiesProvider source) {
			base.AssignCore(source);
			ILoadingIndicatorProperties sourceProperties = source as ILoadingIndicatorProperties;
			if(source != null) {
				AppearanceCaption.AssignInternal(sourceProperties.AppearanceCaption);
				AppearanceDescription.AssignInternal(sourceProperties.AppearanceDescription);
			}
		}
		protected override bool ShouldSerializeCore() {
			return base.ShouldSerializeCore() ||
				AppearanceCaption.ShouldSerialize() ||
				AppearanceDescription.ShouldSerialize();
		}
		protected override void ResetCore() {
			base.ResetCore();
			AppearanceCaption.Reset();
			AppearanceDescription.Reset();
		}
	}
}
