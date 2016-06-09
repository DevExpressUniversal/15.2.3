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

using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.XtraBars.Docking2010.Base;
namespace DevExpress.XtraBars.Docking2010.Views.WindowsUI {
	public interface IPageGroupProperties : IDocumentSelectorProperties {
		bool ShowPageHeaders { get; set; }
	}
	public interface IPageGroupDefaultProperties : IDocumentSelectorDefaultProperties {
		DefaultBoolean ShowPageHeaders { get; set; }
		bool CanShowPageHeaders { get; }
	}
	[ToolboxItem(false), DesignTimeVisible(false)]
	public class PageGroup : DocumentSelector {
		public PageGroup()
			: base((IContainer)null) {
		}
		public PageGroup(IContainer container)
			: base(container) {
		}
		public PageGroup(IPageGroupProperties defaultProperties)
			: base(defaultProperties) {
		}
		protected override ContextualZoomLevel GetZoomLevel() {
			return Properties != null && !Properties.CanShowPageHeaders ? ContextualZoomLevel.Normal : ContextualZoomLevel.Detail;
		}
		#region Info
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public new IPageGroupInfo Info {
			get { return base.Info as IPageGroupInfo; }
		}
		protected override IContentContainerInfo CreateContentContainerInfo(WindowsUIView view) {
			return new PageGroupInfo(view, this);
		}
		#endregion Info
		#region Properties
		[
#if !SL
	DevExpressXtraBarsLocalizedDescription("PageGroupProperties"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public new IPageGroupDefaultProperties Properties {
			get { return base.Properties as IPageGroupDefaultProperties; }
		}
		protected override DevExpress.Utils.Base.IBaseProperties GetParentProperties(WindowsUIView view) {
			return view.PageGroupProperties;
		}
		protected override IContentContainerDefaultProperties CreateDefaultProperties(IContentContainerProperties parentProperties) {
			return new PageGroupDefaultProperties(parentProperties as IPageGroupProperties);
		}
		#endregion Properties
		#region Items
		protected override DocumentCollection CreateItems() {
			return new PageGroupDocumentCollection(this);
		}
		class PageGroupDocumentCollection : DocumentSelectorDocumentCollection {
			public PageGroupDocumentCollection(PageGroup owner)
				: base(owner) {
			}
		}
		#endregion Items
	}
	public class PageGroupProperties : DocumentSelectorProperties, IPageGroupProperties {
		public PageGroupProperties() {
			SetDefaultValueCore("ShowPageHeaders", true);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new PageGroupProperties();
		}
		[DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		public bool ShowPageHeaders {
			get { return GetValueCore<bool>("ShowPageHeaders"); }
			set { SetValueCore("ShowPageHeaders", value); }
		}
	}
	public class PageGroupDefaultProperties : DocumentSelectorDefaultProperties, IPageGroupDefaultProperties {
		public PageGroupDefaultProperties(IPageGroupProperties parentProperties)
			: base(parentProperties) {
			SetConverter("ShowPageHeaders", GetDefaultBooleanConverter(true));
			SetDefaultValueCore("ShowPageHeaders", DefaultBoolean.Default);
		}
		protected override DevExpress.Utils.Base.IBaseProperties CloneCore() {
			return new PageGroupDefaultProperties(ParentProperties as IPageGroupProperties);
		}
		[DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(XtraSerializationFlags.DefaultValue), Category("Behavior")]
		public DefaultBoolean ShowPageHeaders {
			get { return GetValueCore<DefaultBoolean>("ShowPageHeaders"); }
			set { SetValueCore("ShowPageHeaders", value); }
		}
		[Browsable(false)]
		public bool CanShowPageHeaders {
			get { return GetActualValue<DefaultBoolean, bool>("ShowPageHeaders"); }
		}
	}
}
