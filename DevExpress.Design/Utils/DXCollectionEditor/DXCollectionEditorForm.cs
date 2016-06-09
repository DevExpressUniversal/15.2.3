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
using System.Reflection;
using DevExpress.LookAndFeel.Helpers;
using DevExpress.Utils.Design.Internal;
namespace DevExpress.Utils.Design {
	[ToolboxItem(false)]
	public partial class DXCollectionEditorForm : DevExpress.Utils.Design.XtraDesignFormWithAdaptiveSkining
#if DEBUGTEST
, DXCollectionEditorFormTest
#endif
 {
		#region ctor
		public DXCollectionEditorForm() {
			InitializeContent();
			InitializeComponent();
		}
		DXCollectionEditorContent collectionEditorContent;
		DevExpress.Utils.Design.PropertyStore store;
		string registryStoreCollection = String.Empty;
		bool isStore = false;
		ICollection externalCollection;
		ITypeDescriptorContext editContext = null;
		IServiceProvider serviceProvider = null;
		#endregion
		#region Properties
		public object EditValue {
			get { return this.collectionEditorContent.Items; }
			set {
				externalCollection = value as ICollection;
				if(externalCollection != null) {
					editContext = GetEditContext();
					serviceProvider = GetServiceProvider();
					this.collectionEditorContent.InitializeEditContex(editContext, serviceProvider);
					this.collectionEditorContent.Items = externalCollection;
				}
				else this.collectionEditorContent.Reset();
			}
		}
		protected DevExpress.Utils.Design.PropertyStore Store {
			get {
				if(this.store == null)
					this.store = new PropertyStore(this.RegistryStorePrefix + "\\" + this.RegistryStorePath + "\\");
				return this.store;
			}
		}
		public string RegistryStorePath {
			get { return this.registryStoreCollection; }
			set {
				if(!String.IsNullOrEmpty(value)) {
					isStore = true;
					this.registryStoreCollection = value;
				}
			}
		}
		protected virtual
		#endregion
		#region Events handler
 void XtraCollectionForm_Load(object sender, EventArgs e) {
			this.collectionEditorContent.Initialize(CreateNewItemTypes(), GetUISettings());
			this.Store.Restore();
		}
		protected override void OnFormClosing(System.Windows.Forms.FormClosingEventArgs e) {
			this.collectionEditorContent.Clear(this.DialogResult);
			if(this.collectionEditorContent.IsContentChanged)
				this.OnApprovedCollectionChanged();
			base.OnFormClosing(e);
		}
		void CollectionEditorContent_QueryNewItem(object sender, QueryNewItemEventArgs e) {
			e.Item = GetDefaultElement(e.ItemType);
		}
		void CollectionEditorContent_ItemChanged(object sender, PropertyItemChangedEventArgs e) {
			OnCollectionItemChanged(e);
		}
		void СollectionEditorContent_GetCustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			e.DisplayText = GetCustomDisplayText(e.Item, e.FieldName);
		}
		void CollectionEditorContent_CollectionChanged(object sender, CollectionChangedEventArgs e) {
			this.OnCollectionChanged(e);
		}
		void CollectionEditorContent_CollectionChanging(object sender, CollectionChangingEventArgs e) {
			this.OnCollectionChanging(e);
		}
		#endregion
		#region Virtual Methods
		protected virtual DevExpress.Utils.Design.DXCollectionEditorBase.ItemTypeInfo[] CreateNewItemTypes() {
			return null;
		}
		protected virtual DXCollectionEditorContent CreateContentControl() {
			return new DevExpress.Utils.Design.DXCollectionEditorContent();
		}
		protected virtual object GetDefaultElement(Type type) {
			return null;
		}
		protected virtual DevExpress.Utils.Design.DXCollectionEditorBase.UISettings GetUISettings() {
			return null;
		}
		protected virtual void OnCollectionItemChanged(PropertyItemChangedEventArgs e) { }
		protected virtual string GetCustomDisplayText(object value, string fieldName) {
			return null;
		}
		protected virtual void OnCollectionChanged(CollectionChangedEventArgs e) { }
		protected virtual void OnCollectionChanging(CollectionChangingEventArgs e) { }
		protected virtual void RestoreForm() {
			if(this.Store != null && isStore) {
				this.Store.RestoreForm(this);
				this.RestoreContent();
			}
		}
		protected virtual void StoreForm() {
			if(this.Store != null && isStore) {
				this.Store.AddForm(this);
				this.Store.Store();
				this.StoreContent();
			}
		}
		protected virtual void StoreContent() {
			if(this.collectionEditorContent != null && this.Store != null) {
				this.collectionEditorContent.Store(this.Store);
			}
		}
		protected virtual void RestoreContent() {
			if(this.collectionEditorContent != null && this.Store != null) {
				this.collectionEditorContent.Restore(this.Store);
			}
		}
		protected virtual void OnApprovedCollectionChanged() { }
		protected virtual string RegistryStorePrefix {
			get { return @"Software\Developer Express\Designer\DXCollectionEditorForm\"; }
		}
		protected virtual IServiceProvider GetServiceProvider() {
			return null;
		}
		protected virtual ITypeDescriptorContext GetEditContext() {
			return null;
		}
		protected override void OnClosed(EventArgs e) {
			this.StoreForm();
			base.OnClosed(e);
		}
		protected override void OnLoad(EventArgs e) {
			base.OnLoad(e);
			SetFormName(externalCollection);
			this.RestoreForm();
			ControlContainerLookAndFeelHelper.UpdateControlChildrenLookAndFeel(this, LookAndFeel);
		}
		protected override string AssemblyName {
			get {
				if(externalCollection == null) return base.AssemblyName;
				string assemblyName = Assembly.GetAssembly(editContext.Instance.GetType()).FullName;
				return assemblyName.Substring(0, assemblyName.IndexOf(", Culture")); ;
			}
		}
		#endregion
		#region Private Methods
		void SetFormName(ICollection collection) {
			string formatString = "Collection Editor ({0})";
			string collectionName = String.Empty;
			if(!String.IsNullOrEmpty(this.RegistryStorePath)) {
				var sub = this.RegistryStorePath.Split(new char[] { '\\' });
				collectionName = sub[sub.Length - 1];
			}
			else if(collection != null) {
				Type collectionType = collection.GetType();
				Type propertyType = null;
				var properties = collectionType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance | BindingFlags.Default);
				foreach(var p in properties) {
					if(p.Name == "Item") {
						propertyType = p.PropertyType;
						break;
					}
				}
				if(propertyType != null) {
					collectionName = propertyType.Name;
				}
			}
			this.Text = String.Format(formatString, collectionName);
		}
		void InitializeContent() {
			this.collectionEditorContent = CreateContentControl();
			InitializeContentControl();
			this.Controls.Add(this.collectionEditorContent);
		}
		void InitializeContentControl() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DXCollectionEditorForm));
			resources.ApplyResources(this.collectionEditorContent, "collectionEditorContent");
			this.collectionEditorContent.Name = "collectionEditorContent";
			this.collectionEditorContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.collectionEditorContent.ItemChanged += new DevExpress.Utils.Design.Internal.PropertyItemChangedEventHandler(this.CollectionEditorContent_ItemChanged);
			this.collectionEditorContent.QueryCustomDisplayText += new DevExpress.Utils.Design.Internal.QueryCustomDisplayTextEventHandler(this.СollectionEditorContent_GetCustomDisplayText);
			this.collectionEditorContent.QueryNewItem += new DevExpress.Utils.Design.Internal.QueryNewItemEventHandler(this.CollectionEditorContent_QueryNewItem);
			this.collectionEditorContent.CollectionChanged += new DevExpress.Utils.Design.Internal.CollectionChangedEventHandler(this.CollectionEditorContent_CollectionChanged);
			this.collectionEditorContent.CollectionChanging += new DevExpress.Utils.Design.Internal.CollectionChangingEventHandler(this.CollectionEditorContent_CollectionChanging);
		}
		#endregion
#if DEBUGTEST
		void DXCollectionEditorFormTest.Initialize() {
			this.XtraCollectionForm_Load(this, EventArgs.Empty);
		}
		DXCollectionEditorContent DXCollectionEditorFormTest.ContentControl {
			get { return this.collectionEditorContent; }
		}
#endif
	}
#if DEBUGTEST
	interface DXCollectionEditorFormTest {
		void Initialize();
		DXCollectionEditorContent ContentControl { get; }
	}
#endif
}
