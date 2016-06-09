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
using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraCharts.Native;
namespace DevExpress.XtraCharts {
	public abstract class ChartElement : IDisposable, ICloneable, IXtraSupportShouldSerialize, IOwnedElement, IChartElementWizardAccess {
		#region Nested classes: ElementWillChangeNotification, ElementChangedNotification
		protected internal class ElementWillChangeNotification : Notification {
			public ElementWillChangeNotification(object sender) : base(sender, null) {
			}
		}
		protected internal class ElementChangedNotification : Notification {
			readonly ChartUpdateInfoBase updateInfo;
			public ChartUpdateInfoBase UpdateInfo {
				get {
					return updateInfo;
				}
			}
			public ElementChangedNotification(ChartElement element, ChartUpdateInfoBase updateInfo) : base(element, null) {
				this.updateInfo = updateInfo;
			}
		}		
		#endregion
		ChartElement owner;
		bool isDisposed = false;
		object tag;
		protected internal ChartElement Owner {
			get { return owner; }
			set {
				ChartElement oldOwner = owner;
				owner = value;
				OnOwnerChanged(oldOwner, owner);
			}
		}
		protected internal virtual NotificationCenter NotificationCenter {
			get {
				return (Owner != null) ? Owner.NotificationCenter : null;
			}
		}
		protected internal virtual bool Loading { get { return owner == null ? WebLoadingHelper.GlobalLoading : owner.Loading; } }
		protected internal virtual IChartContainer ChartContainer { get { return owner != null ? owner.ChartContainer : null; } }
		protected internal virtual ChartContainerAdapter ContainerAdapter { get { return owner != null ? owner.ContainerAdapter : null; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartElementTag"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartElement.Tag"),
		TypeConverter(typeof(StringConverter)),
		XtraSerializableProperty]
		public object Tag {
			get { return tag; }
			set {
				if (value != tag) {
					SendNotification(new ElementWillChangeNotification(this));
					tag = value;
					RaiseControlChanged();
				}
			}
		}
		[Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public virtual bool IsDisposed { get { return isDisposed; } }
		protected ChartElement()
			: this(null) { }
		protected ChartElement(ChartElement owner) {
			this.owner = owner;
		}
		~ChartElement() {
			Dispose(false);
		}
		#region IOwnedElement implementation
		IOwnedElement IOwnedElement.Owner { get { return Owner; } }
		IChartContainer IOwnedElement.ChartContainer { get { return ChartContainer; } }
		#endregion
		#region IChartElementWizardAccess
		void IChartElementWizardAccess.RaiseControlChanging() {
			SendNotification(new ElementWillChangeNotification(this));
		}
		void IChartElementWizardAccess.RaiseControlChanged() {
			RaiseControlChanged();
		}
		#endregion
		#region IDisposable implementation
		public void Dispose() {
			bool canDispose = ChartContainer == null || ChartContainer.CanDisposeItems;
			if (!isDisposed && canDispose) {
				Dispose(true);
				isDisposed = true;
				GC.SuppressFinalize(this);
			}
		}
		protected virtual void Dispose(bool disposing) {
		}
		#endregion
		#region ICloneable implementation
		public virtual object Clone() {
			ChartElement obj = CreateObjectForClone();
			obj.Assign(this);
			return obj;
		}
		#endregion
		#region XtraSerializing
		protected internal virtual bool XtraSerializing { get { return owner == null ? XtraSerializingHelper.XtraSerializing : owner.XtraSerializing; } }
		bool IXtraSupportShouldSerialize.ShouldSerialize(string propertyName) {
			return XtraShouldSerialize(propertyName);
		}
		protected virtual bool XtraShouldSerialize(string propertyName) {
			switch (propertyName) {
				case "Tag":
					return ShouldSerializeTag();
				default:
					return true;
			}
		}
		#endregion
		#region ShouldSerialize & Reset
		bool ShouldSerializeTag() {
			return !String.IsNullOrEmpty(tag as string);
		}
		void ResetTag() {
			Tag = null;
		}
		protected internal virtual bool ShouldSerialize() {
			return ShouldSerializeTag();
		}
		#endregion
		protected abstract ChartElement CreateObjectForClone();
		protected virtual void OnOwnerChanged(ChartElement oldOwner, ChartElement newOwner) { }
		protected virtual bool ProcessChanged(ChartElement sender, ChartUpdateInfoBase changeInfo) {
			if (owner != null)
				return owner.ProcessChanged(sender, changeInfo);
			return true;
		}
		protected void RaisePropertyChanged<T>(string name, T oldValue, T newValue) {
			RaiseControlChanged(new PropertyUpdateInfo<T>(this, name, oldValue, newValue));
		}
		internal void LockChanges() {
			if (NotificationCenter != null) {
				NotificationCenter.DenyNotifications(typeof(ElementWillChangeNotification));
				NotificationCenter.DenyNotifications(typeof(ChartElement.ElementChangedNotification));
			}
		}
		internal void InitTag(object tag) {
			this.tag = tag;
		}
		internal void UnlockChanges() {
			if (NotificationCenter != null) {
				NotificationCenter.AllowNotifications(typeof(ElementWillChangeNotification));
				NotificationCenter.AllowNotifications(typeof(ChartElement.ElementChangedNotification));
			}
		}
		protected internal T GetOwner<T>() where T : ChartElement {
			if (this.GetType() == typeof(T))
				return (T)this;
			if (Owner != null)
				return Owner.GetOwner<T>();
			return null;
		}
		protected internal void SendNotification(Notification notification) {
			if (NotificationCenter != null)
				NotificationCenter.Send(notification);
		}
		protected internal void RaiseControlChanged() {
			RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.NonSpecific));
		}
		protected internal void RaiseControlChanged(ChartUpdateInfoBase changeInfo) {
			if (ProcessChanged(this, changeInfo))
				SendNotification(new ElementChangedNotification(this, changeInfo));
		}
		protected internal virtual void ChildCollectionChanged(ChartCollectionBase collection, ChartUpdateInfoBase changeInfo) {
			RaiseControlChanged(changeInfo);
		}
		public override string ToString() {
			return String.Format("({0})", GetType().Name);
		}
		public virtual void Assign(ChartElement obj) {
			if (obj == null)
				throw new ArgumentNullException("obj");
			tag = obj.tag;
		}		
	}
	public abstract class ChartElementNamed : ChartElement {
		string name;
		protected virtual bool AllowEmptyName { get { return true; } }
		protected virtual string EmptyNameExceptionText { get { return ""; } }
		[
#if !SL
	DevExpressXtraChartsLocalizedDescription("ChartElementNamedName"),
#endif
		DXDisplayName(typeof(ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraCharts.ChartElementNamed.Name"),
		XtraSerializableProperty
		]
		public string Name {
			get { return name; }
			set {
				CheckName(value);
				SendNotification(new ElementWillChangeNotification(this));
				name = value;
				RaiseControlChanged();
			}
		}
		#region ShouldSerialize & Reset
		bool ShouldSerializeName() {
			return AllowEmptyName || !String.IsNullOrEmpty(name);
		}
		void ResetName() {
			if (AllowEmptyName)
				Name = "";
		}
		#endregion
		#region XtraSerializing
		protected override bool XtraShouldSerialize(string propertyName) {
			if (propertyName == "Name")
				return ShouldSerializeName();
			return base.XtraShouldSerialize(propertyName);
		}
		#endregion
		protected ChartElementNamed(string name)
			: base() {
			Initialize(name);
		}
		protected ChartElementNamed(string name, ChartElement owner)
			: base(owner) {
			Initialize(name);
		}
		void Initialize(string name) {
			this.name = name;
		}
		void CheckName(string elementName) {
			if (!AllowEmptyName && String.IsNullOrEmpty(elementName))
				throw new ArgumentException(EmptyNameExceptionText);
		}
		protected void CheckName() {
			CheckName(name);
		}
		public override string ToString() {
			return name;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			ChartElementNamed namedObj = obj as ChartElementNamed;
			if (namedObj == null)
				return;
			this.name = namedObj.name;
		}
	}
}
