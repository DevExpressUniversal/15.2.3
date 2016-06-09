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
using System.Windows.Forms;
using System.Drawing;
namespace DevExpress.Utils.VisualEffects {
	[Designer("DevExpress.Utils.Design.AdornerUIManagerDesigner, " + AssemblyInfo.SRAssemblyDesignFull),
	 Description("Provides an adorner layer that draws custom items above UI elements."),
	 ToolboxTabName(AssemblyInfo.DXTabComponents), DXToolboxItem(DXToolboxItemKind.Regular),
	 ToolboxBitmap(typeof(ToolBoxIcons.ToolboxIconsRootNS), "AdornerUIManager")
]
	public class AdornerUIManager : Component, IAdornerUIManagerInternal, IAdornerUIManager, ISupportInitialize {
		AdornerWrapperCollection adornerWrappers;
		AdornerElementsTree elementsTree;
		int lockUpdate = 0;
		int initializing = 0;
		bool isDisposing;
		AppearanceObject badgeAppearanceCore;
		AdornerElementCollection elementCollectionCore;
		AdornerUILayer adornerLayer;
		ContainerControl ownerCore;
		Form ownerForm;
		BadgeProperties badgePropertiesCore;
		public AdornerUIManager() : this(null) { }
		public AdornerUIManager(IContainer container) {
			if(container != null)
				container.Add(this);
			elementCollectionCore = CreateAdornerElementCollection();
			badgeAppearanceCore = CreateBadgeAppearance();
			badgePropertiesCore = CreateBadgeProperties();
			elementsTree = new AdornerElementsTree(this);
			adornerWrappers = new AdornerWrapperCollection();
			Subscribe();
		}
		#region propeties
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AdornerUIManagerBadgeAppearance"),
#endif
 Category("Appearance"),
		  DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AppearanceObject BadgeAppearance { get { return badgeAppearanceCore; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AdornerUIManagerBadgeProperties"),
#endif
 Category("Properties"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public BadgeProperties BadgeProperties { get { return badgePropertiesCore; } }
		[
#if !SL
	DevExpressUtilsLocalizedDescription("AdornerUIManagerElements"),
#endif
 Category("Layout"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public AdornerElementCollection Elements { get { return elementCollectionCore; } }
		[DefaultValue(null), 
#if !SL
	DevExpressUtilsLocalizedDescription("AdornerUIManagerOwner"),
#endif
 Category("Behavior")]
		public ContainerControl Owner {
			get { return ownerCore; }
			set {
				if(Owner == value) return;
				UnsubscribeOwner();
				ownerCore = value;
				UpdateOwnerForm();
				SubscribeOwner();
				OnOwnerChanged();
			}
		}
		void UpdateOwnerForm() {
			if(ownerForm != null) {
				ownerForm.Move -= OnOwnerFormMove;
				ownerForm.SizeChanged -= OnOwnerFormSizeChanged;
			}
			ownerForm = (ownerCore == null || ownerCore is Form) ? null : ownerCore.FindForm();
			if(ownerForm != null) {
				ownerForm.Move += OnOwnerFormMove;
				ownerForm.SizeChanged += OnOwnerFormSizeChanged;
			}
		}
		void OnOwnerFormSizeChanged(object sender, EventArgs e) {
			if(ownerForm.WindowState == FormWindowState.Minimized)
				Hide();
			else {
				BeginUpdate();
				Show();
				foreach(var element in Elements)
					element.Update();
				EndUpdate();
			}
		}
		bool ShouldSerializeBadgeAppearance() {
			return !isDisposing && BadgeAppearance.ShouldSerialize();
		}
		void ResetBadgeAppearance() {
			BadgeAppearance.Reset();
		}
		#endregion
		protected virtual AppearanceObject CreateBadgeAppearance() { return new AppearanceObject(); }
		protected virtual BadgeProperties CreateBadgeProperties() { return new BadgeProperties(); }
		protected virtual AdornerElementCollection CreateAdornerElementCollection() { return new AdornerElementCollection(); }
		protected virtual void OnAppearanceChanged(object sender, EventArgs e) { UpdateLayer(true); }
		protected virtual void OnPropertiesChanged(object sender, EventArgs e) { UpdateLayer(true); }
		protected virtual void OnElementCollectionChanged(object sender, CollectionChangeEventArgs e) {
			if(e.Action == CollectionChangeAction.Add)
				RegisterElement(e.Element as IAdornerElement);
			if(e.Action == CollectionChangeAction.Remove)
				UnregisterElement(e.Element as IAdornerElement);
			UpdateLayer(true);
		}
		bool RegisterAdornerWrapper(IAdornerElement element) {
			if(element.TargetElement is ISupportAdornerElementBarItem) {
				BarItemAdornerWrapper barElement = new BarItemAdornerWrapper(element, this);
				adornerWrappers.Add(barElement);
				element.Disposed += OnElementDisposed;
				return true;
			}
			return false;
		}
		protected virtual void RegisterElement(IAdornerElement element) {
			if(IsInitializing) return;
			if(RegisterAdornerWrapper(element)) return;
			element.Updated += OnElementUpdated;
			element.Disposed += OnElementDisposed;
			if(element is Badge) {
				element.EnsureParentProperties(BadgeProperties);
				element.EnsureParentAppearance(BadgeAppearance);
			}
			elementsTree.AddElement(element);
			if(!IsLayerCreated) return;
			adornerLayer.RegisterElement(element);
		}
		protected virtual void UnregisterElement(IAdornerElement element) {
			if(UnregisterAdornerWrapper(element)) return;
			element.Updated -= OnElementUpdated;
			element.Disposed -= OnElementDisposed;
			element.EnsureParentProperties(null);
			element.EnsureParentAppearance(null);
			elementsTree.RemoveElement(element);
			if(!IsLayerCreated) return;
			adornerLayer.UnregisterElement(element);
		}
		bool UnregisterAdornerWrapper(IAdornerElement element) {
			if(element.TargetElement is ISupportAdornerElementBarItem) {
				element.Disposed -= OnElementDisposed;
				BarItemAdornerWrapper barElement = adornerWrappers[element];
				if(barElement == null) return false;
				barElement.Dispose();
				adornerWrappers.Remove(barElement);
				return true;
			}
			return false;
		}
		void OnElementDisposed(object sender, EventArgs e) {
			AdornerElement element = sender as AdornerElement;
			if(element == null) return;
			if(!elementCollectionCore.Remove(element))
				UnregisterElement(element);
		}
		void OnElementUpdated(object sender, EventArgs e) { UpdateLayer(false); }
		protected virtual void Subscribe() {
			badgePropertiesCore.Changed += OnPropertiesChanged;
			badgeAppearanceCore.Changed += OnAppearanceChanged;
			elementCollectionCore.CollectionChanged += OnElementCollectionChanged;
			LookAndFeel.UserLookAndFeel.Default.StyleChanged += OnStyleChanged;
		}
		protected virtual void OnStyleChanged(object sender, EventArgs e) {
			if(ownerForm == null) return;
			ownerForm.BeginInvoke(new Action(() =>
			{
				if(isDisposing) return;
				BeginUpdate();
				foreach(var element in Elements)
					element.UpdateStyle();
				EndUpdate();
			}));
		}
		protected virtual void Unsubscribe() {
			badgePropertiesCore.Changed -= OnPropertiesChanged;
			badgeAppearanceCore.Changed -= OnAppearanceChanged;
			elementCollectionCore.CollectionChanged -= OnElementCollectionChanged;
			LookAndFeel.UserLookAndFeel.Default.StyleChanged -= OnStyleChanged;
		}
		#region OwnerChanged
		void OnOwnerLocationChanged(object sender, EventArgs e) { ChangeLayerLocation(); }
		void OnOwnerSizeChanged(object sender, EventArgs e) { ChangeLayerSize(); }
		void OnOwnerDisposed(object sender, EventArgs e) {
			if(IsLayerCreated)
				DestroyLayer();
			UnsubscribeOwner();
			ownerCore = null;
		}
		protected virtual void OnOwnerChanged() {
			DestroyLayer();
			CreateLayer();
			Show();
		}
		void OnOwnerVisibleChanged(object sender, EventArgs e) {
			if(ownerCore.Visible) {
				CreateLayer();
				Owner.BeginInvoke(
					new Action(() => { Show(); }
				));
			}
			else
				Hide();
		}
		protected void SubscribeOwner() {
			if(ownerCore == null || ownerCore.IsDisposed) return;
			ownerCore.SizeChanged += OnOwnerSizeChanged;
			ownerCore.LocationChanged += OnOwnerLocationChanged;
			ownerCore.Move += OnOwnerLocationChanged;
			ownerCore.VisibleChanged += OnOwnerVisibleChanged;
			ownerCore.Disposed += OnOwnerDisposed;
		}
		void OnOwnerFormMove(object sender, EventArgs e) {
			ChangeLayerLocation();
		}
		protected void UnsubscribeOwner() {
			if(ownerCore == null) return;
			ownerCore.SizeChanged -= OnOwnerSizeChanged;
			ownerCore.LocationChanged -= OnOwnerLocationChanged;
			ownerCore.Move -= OnOwnerLocationChanged;
			ownerCore.VisibleChanged -= OnOwnerVisibleChanged;
			ownerCore.Disposed -= OnOwnerDisposed;
		}
		protected bool CheckOwner() { return Owner != null && Owner.IsHandleCreated && Owner.Visible; }
		#endregion
		#region adornerLayer
		protected void UpdateLayer(bool updateRegions) {
			if(!CanUpdateLayer) return;
			adornerLayer.Update(updateRegions);
		}
		public void Show() {
			if(!IsLayerCreated) return;
			if(!IsLayerVisible) {
				adornerLayer.Location = Point.Empty;
				UpdateOwnerForm();
				adornerLayer.Show();
				UpdateLayer(true);
			}
		}
		public void Hide() {
			if(!IsLayerVisible) return;
			adornerLayer.Hide();
		}
		protected void DestroyLayer() {
			if(!IsLayerCreated) return;
			Ref.Dispose(ref adornerLayer);
		}
		protected void CreateLayer() {
			if(!CanCreateLayer) return;
			adornerLayer = CreateLayerCore();
			foreach(var barElement in adornerWrappers)
				adornerLayer.RegisterElementWrapper(barElement);
			foreach(IAdornerElement element in elementCollectionCore)
				adornerLayer.RegisterElement(element);
			adornerLayer.Size = GetLayerSize();
		}
		protected virtual AdornerUILayer CreateLayerCore() { return new AdornerUILayer(Owner); }
		protected bool CanUpdateLayer { get { return IsLayerCreated && adornerLayer.IsVisible && lockUpdate == 0 && !isDisposing; } }
		protected bool CanCreateLayer { get { return CheckOwner() && !IsLayerCreated && !DesignMode; } }
		protected bool IsLayerCreated { get { return adornerLayer != null && adornerLayer.IsCreated; } }
		protected bool IsLayerVisible { get { return IsLayerCreated && adornerLayer.IsVisible; } }
		protected void ChangeLayerLocation() {
			if(!CanUpdateLayer) return;
			adornerLayer.Location = Point.Empty;
		}
		protected void ChangeLayerSize() {
			if(!CanUpdateLayer) return;
			BeginUpdate();
			if(ownerForm == null) {
				Form formCore = ownerCore as Form;
				if(formCore.WindowState == FormWindowState.Minimized)
					Hide();
				else {
					Show();
					foreach(var element in Elements)
						element.Update();
				}
			}
			adornerLayer.Size = GetLayerSize();
			EndUpdate();
		}
		protected virtual Size GetLayerSize() {
			if(!CheckOwner()) return Size.Empty;
			return ownerCore.ClientSize;
		}
		#endregion
		protected override void Dispose(bool disposing) {
			if(!isDisposing) {
				isDisposing = true;
				UnsubscribeOwner();
				Unsubscribe();
				foreach(IAdornerElement element in elementCollectionCore) {
					UnregisterElement(element);
					element.Dispose();
				}
				elementCollectionCore.Clear();
				Ref.Dispose(ref elementsTree);
				ownerCore = null;
				UpdateOwnerForm();
				ownerForm = null;
				Ref.Dispose(ref badgePropertiesCore);
				Ref.Dispose(ref badgeAppearanceCore);
				DestroyLayer();
			}
			base.Dispose(disposing);
		}
		#region IAdornerUIManager Members
		[Browsable(false)]
		public bool IsUpdateLocked {
			get { return lockUpdate > 0; }
		}
		public void BeginUpdate() { lockUpdate++; }
		public void EndUpdate() {
			if(lockUpdate == 0) return;
			if(--lockUpdate == 0)
				UpdateLayer(true);
		}
		public void CancelUpdate() {
			if(lockUpdate == 0) return;
			lockUpdate = 0;
		}
		#endregion
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
			initializing++;
		}
		void ISupportInitialize.EndInit() {
			if(--initializing == 0)
				OnInitialize();
		}
		protected virtual void OnInitialize() {
			if(IsInitializing) return;
			foreach(var barElement in adornerWrappers)
				barElement.RegisterChildren();
			foreach(var element in elementCollectionCore)
				RegisterElement(element);
			UpdateLayer(true);
		}
		protected bool IsInitializing {
			get { return initializing > 0; }
		}
		#endregion
		#region IAdornerUIManagerInternal Members
		void IAdornerUIManagerInternal.RegisterElement(IAdornerElement element) { RegisterElement(element); }
		void IAdornerUIManagerInternal.UnregisterElement(IAdornerElement element) { UnregisterElement(element); }
		void IAdornerUIManagerInternal.UpdateLayer(bool updateRegions) { UpdateLayer(updateRegions); }
		#endregion
	}
}
