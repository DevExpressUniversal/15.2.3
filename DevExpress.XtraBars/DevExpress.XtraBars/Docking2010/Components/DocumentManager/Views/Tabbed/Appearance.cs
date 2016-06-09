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
using DevExpress.XtraBars.Docking2010.Views.Tabbed;
using DevExpress.Utils.Base;
namespace DevExpress.XtraBars.Docking2010.Views {
	public interface IPageAppearanceProvider : ISupportBatchUpdate, ISupportObjectChanged, IDisposable {
		AppearanceObject Header { get; }
		AppearanceObject HeaderActive { get; }
		AppearanceObject HeaderSelected { get; }
		AppearanceObject HeaderHotTracked { get; }
		AppearanceObject HeaderDisabled { get; }
		AppearanceObject PageClient { get; }
	}
	class DocumentPageAppearanceProvider : BaseAppearanceCollection, IPageAppearanceProvider {
		AppearanceObject header;
		AppearanceObject headerActive;
		AppearanceObject headerDisabled;
		AppearanceObject headerHotTracked;
		AppearanceObject headerSelected;
		AppearanceObject pageClient;
		protected override void CreateAppearances() {
			base.CreateAppearances();
			header = CreateAppearance("Header");
			headerActive = CreateAppearance("HeaderActive");
			headerDisabled = CreateAppearance("HeaderDisabled");
			headerHotTracked = CreateAppearance("HeaderHotTracked");
			headerSelected = CreateAppearance("HeaderSelected");
			pageClient = CreateAppearance("PageClient");
		}
		public AppearanceObject Header { get { return header; } }
		public AppearanceObject HeaderActive { get { return headerActive; } }
		public AppearanceObject HeaderDisabled { get { return headerDisabled; } }
		public AppearanceObject HeaderHotTracked { get { return headerHotTracked; } }
		public AppearanceObject HeaderSelected { get { return headerSelected; } }
		public AppearanceObject PageClient { get { return pageClient; } }
		bool ISupportBatchUpdate.IsUpdateLocked { get { return IsLockUpdate; } }
	}
	public class TabbedViewAppearanceCollection : BaseViewAppearanceCollection, IPageAppearanceProvider {
		public TabbedViewAppearanceCollection(TabbedView owner)
			: base(owner) {
		}
		protected override void CreateAppearances() {
			base.CreateAppearances();
			header = CreateAppearance("Header");
			headerActive = CreateAppearance("HeaderActive");
			headerDisabled = CreateAppearance("HeaderDisabled");
			headerHotTracked = CreateAppearance("HeaderHotTracked");
			headerSelected = CreateAppearance("HeaderSelected");
			pageClient = CreateAppearance("PageClient");
			pageAppearance = CreatePageAppearance();
		}
		protected virtual TabbedViewPageAppearance CreatePageAppearance() {
			return new TabbedViewPageAppearance(this);
		}
		TabbedViewPageAppearance pageAppearance;
		AppearanceObject header;
		AppearanceObject headerActive;
		AppearanceObject headerDisabled;
		AppearanceObject headerHotTracked;
		AppearanceObject headerSelected;
		AppearanceObject pageClient;
		protected internal TabbedViewPageAppearance AppearancePage { get { return pageAppearance; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject Header { get { return header; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderActive { get { return headerActive; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderDisabled { get { return headerDisabled; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderHotTracked { get { return headerHotTracked; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject HeaderSelected { get { return headerSelected; } }
		[
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject PageClient { get { return pageClient; } }
		void ResetHeader() { Header.Reset(); }
		void ResetHeaderActive() { HeaderActive.Reset(); }
		void ResetHeaderDisabled() { HeaderDisabled.Reset(); }
		void ResetHeaderHotTracked() { HeaderHotTracked.Reset(); }
		void ResetHeaderSelected() { HeaderSelected.Reset(); }
		void ResetPageClient() { PageClient.Reset(); }
		bool ShouldSerializeHeader() { return Header.ShouldSerialize(); }
		bool ShouldSerializeHeaderActive() { return HeaderActive.ShouldSerialize(); }
		bool ShouldSerializeHeaderDisabled() { return HeaderDisabled.ShouldSerialize(); }
		bool ShouldSerializeHeaderHotTracked() { return HeaderHotTracked.ShouldSerialize(); }
		bool ShouldSerializeHeaderSelected() { return HeaderSelected.ShouldSerialize(); }
		bool ShouldSerializePageClient() { return PageClient.ShouldSerialize(); }
		bool ISupportBatchUpdate.IsUpdateLocked { get { return IsLockUpdate; } }
	}
	public class TabbedViewPageAppearance : DevExpress.XtraTab.PageAppearance {
		IPageAppearanceProvider ownerCollection;
		public TabbedViewPageAppearance(IPageAppearanceProvider appearanceProvider) {
			this.ownerCollection = appearanceProvider;
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override DevExpress.Utils.AppearanceObject Header {
			get { return ownerCollection.Header; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceObject HeaderActive {
			get { return ownerCollection.HeaderActive; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceObject HeaderDisabled {
			get { return ownerCollection.HeaderDisabled; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceObject HeaderHotTracked {
			get { return ownerCollection.HeaderHotTracked; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject HeaderSelected {
			get { return ownerCollection.HeaderSelected; }
		}
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public override AppearanceObject PageClient {
			get { return ownerCollection.PageClient; }
		}
		void ResetHeader() { Header.Reset(); }
		void ResetHeaderActive() { HeaderActive.Reset(); }
		void ResetHeaderDisabled() { HeaderDisabled.Reset(); }
		void ResetHeaderHotTracked() { HeaderHotTracked.Reset(); }
		void ResetHeaderSelected() { HeaderSelected.Reset(); }
		void ResetPageClient() { PageClient.Reset(); }
		bool ShouldSerializeHeader() { return Header.ShouldSerialize(); }
		bool ShouldSerializeHeaderActive() { return HeaderActive.ShouldSerialize(); }
		bool ShouldSerializeHeaderDisabled() { return HeaderDisabled.ShouldSerialize(); }
		bool ShouldSerializeHeaderHotTracked() { return HeaderHotTracked.ShouldSerialize(); }
		bool ShouldSerializeHeaderSelected() { return HeaderSelected.ShouldSerialize(); }
		bool ShouldSerializePageClient() { return PageClient.ShouldSerialize(); }
		public override void Reset() {
			BeginUpdate();
			try {
				ResetHeader();
				ResetHeaderActive();
				ResetHeaderDisabled();
				ResetHeaderHotTracked();
				ResetPageClient();
			}
			finally { EndUpdate(); }
		}
		public override string ToString() {
			return "PageAppearance";
		}
	}
}
