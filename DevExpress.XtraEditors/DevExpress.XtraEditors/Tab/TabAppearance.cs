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
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.Utils.Controls;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
using DevExpress.XtraTab.Registrator;
using DevExpress.XtraTab.Drawing;
using DevExpress.XtraTab.ViewInfo;
namespace DevExpress.XtraTab {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class PageAppearance : IDisposable, IAppearanceOwner {
		IAppearanceOwner owner;
		public event EventHandler Changed;
		bool suppressNotifications;
		AppearanceObject header, headerHotTracked, headerDisabled, pageClient, headerActive;
		int lockUpdate = 0;
		public PageAppearance() : this(null, false) { 	}
		internal PageAppearance(IAppearanceOwner owner, bool suppressNotifications) {
			this.owner = owner;
			this.suppressNotifications = suppressNotifications;
		}
		bool IAppearanceOwner.IsLoading { get { return owner != null && owner.IsLoading; } }
		protected AppearanceObject CreateAppearance(bool suppressNotifications) {
			return CreateAppearance(suppressNotifications, null);
		}
		protected virtual AppearanceObject CreateAppearance(bool suppressNotifications, AppearanceObject parent) {
			if(suppressNotifications) return new AppearanceObject(parent);
			AppearanceObject res = new AppearanceObject(this, parent);
			res.Changed += new EventHandler(OnApperanceChanged);
			return res;
		}
		void ResetAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Reset();
		}
		protected virtual void DestroyAppearance(AppearanceObject appearance) {
			if(appearance == null) return;
			appearance.Changed -= new EventHandler(OnApperanceChanged);
			appearance.Dispose();
		}
		bool ShouldSerializePageClient() { return pageClient != null && PageClient.ShouldSerialize(); }
		void ResetPageClient() { if(pageClient != null) { PageClient.Reset(); PageClient.Options.Reset(); } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PageAppearancePageClient"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject PageClient {
			get {
				if(pageClient == null)
					this.pageClient = CreateAppearance(suppressNotifications);
				return pageClient;
			}
		}
		bool ShouldSerializeHeaderDisabled() { return headerDisabled != null && HeaderDisabled.ShouldSerialize(); }
		void ResetHeaderDisabled() { if(headerDisabled != null) { HeaderDisabled.Reset(); HeaderDisabled.Options.Reset(); } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PageAppearanceHeaderDisabled"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject HeaderDisabled {
			get {
				if(headerDisabled == null)
					headerDisabled = CreateAppearance(suppressNotifications, Header);
				return headerDisabled;
			}
		}
		bool ShouldSerializeHeaderActive() { return headerActive != null && HeaderActive.ShouldSerialize(); }
		void ResetHeaderActive() { if(headerActive != null) { HeaderActive.Reset(); HeaderActive.Options.Reset(); }  }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PageAppearanceHeaderActive"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject HeaderActive {
			get {
				if(headerActive == null)
					headerActive = CreateAppearance(suppressNotifications);
				return headerActive;
			}
		}
		bool ShouldSerializeHeaderHotTracked() { return headerHotTracked != null && HeaderHotTracked.ShouldSerialize(); }
		void ResetHeaderHotTracked() { if(headerHotTracked != null) { HeaderHotTracked.Reset(); HeaderHotTracked.Options.Reset(); } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PageAppearanceHeaderHotTracked"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject HeaderHotTracked {
			get {
				if(headerHotTracked == null)
					this.headerHotTracked = CreateAppearance(suppressNotifications, Header);
				return headerHotTracked;
			}
		}
		bool ShouldSerializeHeader() { return header != null && Header.ShouldSerialize(); }
		void ResetHeader() { if(header != null) { Header.Reset(); Header.Options.Reset(); } }
		[
#if !SL
	DevExpressXtraEditorsLocalizedDescription("PageAppearanceHeader"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public virtual AppearanceObject Header {
			get {
				if(header == null)
					this.header = CreateAppearance(suppressNotifications);
				return header;
			}
		}
		public virtual void Dispose() {
			DestroyAppearance(header);
			DestroyAppearance(headerHotTracked);
			DestroyAppearance(headerDisabled);
			DestroyAppearance(headerActive);
			DestroyAppearance(pageClient);
		}
		public virtual void BeginUpdate() {
			this.lockUpdate ++;
		}
		public virtual void EndUpdate() {
			if(--this.lockUpdate == 0)
				OnChanged();
		}
		public virtual void Reset() {
			BeginUpdate();
			try {
				ResetAppearance(header);
				ResetAppearance(headerHotTracked);
				ResetAppearance(headerDisabled);
				ResetAppearance(headerActive);
				ResetAppearance(pageClient);
			}
			finally {
				EndUpdate();
			}
		}
		protected virtual void OnChanged() {
			if(this.lockUpdate != 0) return;
			if(Changed != null) Changed(this, EventArgs.Empty);
		}
		protected void OnApperanceChanged(object sender, EventArgs e) {
			OnChanged();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ShouldSerialize() {
			PropertyDescriptorCollection pdColl = TypeDescriptor.GetProperties(this);
			foreach(PropertyDescriptor pd in pdColl) {
				if(pd.SerializationVisibility != DesignerSerializationVisibility.Hidden && pd.ShouldSerializeValue(this)) return true;
			}
			return false;
		}
	}
}
