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
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface ISlideGroupProperties : IDocumentGroupProperties {
		int ItemLength { get; set; }
		double ItemLengthRatio { get; set; }
	}
	public interface ISlideGroupDefaultProperties : IDocumentGroupDefaultProperties {
		int? ItemLength { get; set; }
		double? ItemLengthRatio { get; set; }
		int ActualItemLength { get; }
		double ActualItemLengthRatio { get; }
		bool HasItemLengthRatio { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class SlideGroup : DocumentGroup, IParentOverviewContainer {
		public SlideGroup()
			: base((IContainer)null) {
		}
		public SlideGroup(IContainer container)
			: base(container) {
		}
		public SlideGroup(ISlideGroupProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			scrollBarVisibilityCore = ScrollBarVisibility.Auto;
			overviewContainerPropertiesCore = new OverviewContainerDefaultProperties(null);
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new SlideGroupDefaultProperties(parentProperties as ISlideGroupProperties);
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new SlideGroupInfo(view, this);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new ISlideGroupInfo Info {
			get { return base.Info as ISlideGroupInfo; }
		}
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("SlideGroupProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new ISlideGroupDefaultProperties Properties {
			get { return base.Properties as ISlideGroupDefaultProperties; }
		}
#if !SL
	[DevExpressXtraBarsLocalizedDescription("SlideGroupDetailContainerProperties")]
#endif
		[Category("Layout"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IContentContainerDefaultProperties DetailContainerProperties {
			get { return base.DetailContainerPropertiesCore; }
		}
		bool ShouldSerializeDetailContainerProperties() {
			return DetailContainerProperties != null && DetailContainerProperties.ShouldSerialize();
		}
		void ResetDetailContainerProperties() {
			DetailContainerProperties.Reset();
		}
		protected override DevExpress.Utils.Base.IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.SlideGroupProperties;
		}
		ScrollBarVisibility scrollBarVisibilityCore;
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("SlideGroupScrollBarVisibility"),
#endif
		DefaultValue(ScrollBarVisibility.Auto), XtraSerializableProperty]
		public ScrollBarVisibility ScrollBarVisibility {
			get { return scrollBarVisibilityCore; }
			set { SetValue(ref scrollBarVisibilityCore, value); }
		}
		protected override void EnsureDeferredControlLoadDocuments() {
			IDocumentInfo documentInfo;
			foreach(Document document in Items) {
				if(Info.TryGetValue(document, out documentInfo)) {
					if(!Info.Bounds.IntersectsWith(documentInfo.Bounds)) continue;
					document.EnsureIsBoundToControl(Info.Owner);
				}
			}
		}
		protected override void ActivateDocumentCore(Document document) {
			IDocumentInfo documentInfo;
			if(Info.TryGetValue(document, out documentInfo)) {
				if(document.EnsureIsBoundToControl(Info.Owner))
					Info.ScrollTo(documentInfo);
			}
		}
		protected override void EnsureParentPropertiesCore(WindowsUIView view) {
			base.EnsureParentPropertiesCore(view);
			OverviewContainerProperties.EnsureParentProperties(view.OverviewContainerProperties);
		}
		IOverviewContainerDefaultProperties overviewContainerPropertiesCore;
#if !SL
	[DevExpressXtraBarsLocalizedDescription("SlideGroupOverviewContainerProperties")]
#endif
		[Category("Appearance"), DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IOverviewContainerDefaultProperties OverviewContainerProperties {
			get { return overviewContainerPropertiesCore; }
		}
		bool ShouldSerializeOverviewContainerProperties() {
			return OverviewContainerProperties != null && OverviewContainerProperties.ShouldSerialize();
		}
		void ResetOverviewContainerProperties() {
			OverviewContainerProperties.Reset();
		}
	}
	public class SlideGroupProperties : DocumentGroupProperties, ISlideGroupProperties {
		public SlideGroupProperties() {
			SetDefaultValueCore("ItemLengthRatio", 0.66);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new SlideGroupProperties();
		}
		[DefaultValue(0), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public int ItemLength {
			get { return GetValueCore<int>("ItemLength"); }
			set { SetValueCore("ItemLength", value); }
		}
		[DefaultValue(0.66), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		public double ItemLengthRatio {
			get { return GetValueCore<double>("ItemLengthRatio"); }
			set { SetValueCore("ItemLengthRatio", value); }
		}
	}
	public class SlideGroupDefaultProperties : DocumentGroupDefaultProperties, ISlideGroupDefaultProperties {
		public SlideGroupDefaultProperties(ISlideGroupProperties parentProperties)
			: base(parentProperties) {
			SetConverter("ItemLengthRatio", GetNullableValueConverter(0.66));
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new SlideGroupDefaultProperties(ParentProperties as ISlideGroupProperties);
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public int? ItemLength {
			get { return GetValueCore<int?>("ItemLength"); }
			set { SetValueCore("ItemLength", value); }
		}
		[DefaultValue(null), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Layout")]
		[TypeConverter(typeof(DevExpress.Utils.Design.NullableTypeConverter))]
		public double? ItemLengthRatio {
			get { return GetValueCore<double?>("ItemLengthRatio"); }
			set {
				if(value.HasValue && value.Value <= 0)
					value = (double?)null;
				SetValueCore("ItemLengthRatio", value);
			}
		}
		[Browsable(false)]
		public int ActualItemLength {
			get { return GetActualValueFromNullable<int>("ItemLength"); }
		}
		[Browsable(false)]
		public double ActualItemLengthRatio {
			get { return GetActualValueFromNullable<double>("ItemLengthRatio"); }
		}
		[Browsable(false)]
		public bool HasItemLengthRatio {
			get { return HasValue("ItemLengthRatio"); }
		}
	}
}
