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
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections;
using System.Globalization;
using DevExpress.XtraPrinting.HtmlExport.Native;
namespace DevExpress.XtraPrinting.HtmlExport.Controls {
	public delegate void RenderMethod(DXHtmlTextWriter output, DXWebControlBase container);
	public class DXWebControlBase : IDisposable {
		sealed class ControlRareFields : IDisposable {
			public IDictionary ControlDesignerAccessorUserData;
			public IDictionary DesignModeState;
			public RenderMethod RenderMethod;
			internal ControlRareFields() {
			}
			public void Dispose() {
				ControlDesignerAccessorUserData = null;
				DesignModeState = null;
			}
		}
		sealed class OccasionalFields : IDisposable {
			public DXWebControlCollection Controls;
			public IDictionary ControlsViewState;
			public IDictionary NamedControls;
			public int NamedControlsID;
			public DXWebControlBase.ControlRareFields RareFields;
			public string SkinId;
			public string UniqueIDPrefix;
			internal OccasionalFields() {
			}
			public void Dispose() {
				if(RareFields != null)
					RareFields.Dispose();
				ControlsViewState = null;
			}
		}
		string cachedUniqueID;
		DXWebControlState controlState;
		string id;
		DXWebControlBase namingContainer;
		OccasionalFields occasionalFields;
		DXWebControlBase parent;
		DXStateBag viewState;
		internal DXSimpleBitVector32 flags;
		protected bool ChildControlsCreated {
			set {
				if(!value && flags[8])
					Controls.Clear();
				if(value)
					flags.Set(8);
				else
					flags.Clear(8);
			}
			get { return flags[8]; }
		}
		public virtual string ClientID {
			get {
				EnsureID();
				string uniqueID = UniqueID;
				if(uniqueID != null && uniqueID.IndexOf(IdSeparator) >= 0)
					return uniqueID.Replace(IdSeparator, '_');
				return uniqueID;
			}
		}
		public virtual DXWebControlCollection Controls {
			get {
				if(occasionalFields == null || occasionalFields.Controls == null) {
					EnsureOccasionalFields();
					occasionalFields.Controls = CreateControlCollection();
				}
				return occasionalFields.Controls;
			}
		}
		public virtual bool EnableTheming {
			get {
				if(!flags[0x2000] && Parent != null)
					return Parent.EnableTheming;
				return !flags[0x1000];
			}
			set {
				if(controlState >= DXWebControlState.FrameworkInitialized)
					throw new InvalidOperationException("PropertySetBeforePreInitOrAddToControls");
				if(!value)
					flags.Set(0x1000);
				else
					flags.Clear(0x1000);
				flags.Set(0x2000);
			}
		}
		protected bool HasChildViewState {
			get { return (occasionalFields != null && occasionalFields.ControlsViewState != null && occasionalFields.ControlsViewState.Count > 0); }
		}
		public virtual string ID {
			get {
				if(!flags[1] && !flags[0x800])
					return null;
				return id;
			}
			set {
				if((value != null) && (value.Length == 0))
					value = null;
				string str = id;
				id = value;
				ClearCachedUniqueIDRecursive();
				flags.Set(1);
				flags.Clear(0x200000);
				if(namingContainer != null && str != null)
					namingContainer.DirtyNameTable();
			}
		}
		protected char IdSeparator {
			get { return ':'; }
		}
		protected internal bool IsChildControlStateCleared {
			get { return flags[0x40000]; }
		}
		protected bool IsTrackingViewState {
			get { return flags[2]; }
		}
		public virtual DXWebControlBase NamingContainer {
			get {
				if(namingContainer == null && Parent != null) {
					if(Parent.flags[0x80])
						namingContainer = Parent;
					else
						namingContainer = Parent.NamingContainer;
				}
				return namingContainer;
			}
		}
		public virtual DXWebControlBase Parent {
			get { return parent; }
		}
		ControlRareFields RareFields {
			get {
				if(occasionalFields != null)
					return occasionalFields.RareFields;
				return null;
			}
		}
		private ControlRareFields RareFieldsEnsured {
			get {
				EnsureOccasionalFields();
				ControlRareFields rareFields = occasionalFields.RareFields;
				if(rareFields == null) {
					rareFields = new ControlRareFields();
					occasionalFields.RareFields = rareFields;
				}
				return rareFields;
			}
		}
		public virtual string SkinID {
			get {
				if(occasionalFields != null && occasionalFields.SkinId != null)
					return occasionalFields.SkinId;
				return string.Empty;
			}
			set {
				if(flags[0x4000])
					throw new InvalidOperationException("PropertySetBeforeStyleSheetApplied");
				if(controlState >= DXWebControlState.FrameworkInitialized)
					throw new InvalidOperationException("PropertySetBeforePreInitOrAddToControls");
				EnsureOccasionalFields();
				occasionalFields.SkinId = value;
			}
		}
		public virtual string UniqueID {
			get {
				if(cachedUniqueID == null) {
					DXWebControlBase namingContainer = NamingContainer;
					if(namingContainer == null)
						return id;
					if(id == null)
						GenerateAutomaticID();
					string uniqueIDPrefix = namingContainer.GetUniqueIDPrefix();
					if(uniqueIDPrefix.Length == 0)
						return id;
					cachedUniqueID = uniqueIDPrefix + id;
				}
				return cachedUniqueID;
			}
		}
		protected virtual DXStateBag ViewState {
			get {
				if(viewState == null) {
					viewState = new DXStateBag();
					if(IsTrackingViewState)
						viewState.TrackViewState();
				}
				return viewState;
			}
		}
		public virtual bool Visible {
			get {
				if(flags[0x10])
					return false;
				if(parent != null)
					return parent.Visible;
				return true;
			}
			set {
				if(flags[2]) {
					bool flag = !flags[0x10];
					if(flag != value)
						flags.Set(0x20);
				}
				if(!value)
					flags.Set(0x10);
				else
					flags.Clear(0x10);
			}
		}
		public DXWebControlBase() {
		}
		public string ResolveClientUrl(string relativeUrl) {
			return relativeUrl;
		}
		public virtual DXWebControlBase FindControl(string id) {
			return FindControl(id, 0);
		}
		public virtual void Dispose() {
			if(occasionalFields != null)
				occasionalFields.Dispose();
		}
		public virtual void RenderControl(DXHtmlTextWriter writer) {
			if(!flags[0x10] && !flags[0x200])
				RenderControlInternal(writer);
		}
		[EditorBrowsable(EditorBrowsableState.Advanced)]
		public void SetRenderMethodDelegate(RenderMethod renderMethod) {
			RareFieldsEnsured.RenderMethod = renderMethod;
			Controls.SetCollectionReadOnly("Collection_readonly_Codeblocks");
		}
		public virtual void RemovedControl(DXWebControlBase control) {
			if(namingContainer != null && control.id != null)
				namingContainer.DirtyNameTable();
			control.UnloadRecursive(false);
			control.parent = null;
			control.namingContainer = null;
			control.flags.Clear(0x800);
			control.ClearCachedUniqueIDRecursive();
		}
		public virtual bool HasControls() {
			return occasionalFields != null && occasionalFields.Controls != null && occasionalFields.Controls.Count > 0;
		}
		internal void ClearNamingContainer() {
			EnsureOccasionalFields();
			occasionalFields.NamedControlsID = 0;
			DirtyNameTable();
		}
		internal virtual void UnloadRecursive(bool dispose) {
			if(flags[0x200000]) {
				id = null;
				flags.Clear(0x200000);
			}
			if(occasionalFields != null && occasionalFields.Controls != null) {
				string errorMsg = occasionalFields.Controls.SetCollectionReadOnly("Parent_collections_readonly");
				int count = occasionalFields.Controls.Count;
				for(int i = 0; i < count; i++)
					occasionalFields.Controls[i].UnloadRecursive(dispose);
				occasionalFields.Controls.SetCollectionReadOnly(errorMsg);
			}
			OnUnload(EventArgs.Empty);
			if(dispose)
				Dispose();
		}
		internal void DirtyNameTable() {
			if(occasionalFields != null)
				occasionalFields.NamedControls = null;
		}
		internal virtual string GetUniqueIDPrefix() {
			EnsureOccasionalFields();
			if(occasionalFields.UniqueIDPrefix == null) {
				string uniqueID = UniqueID;
				if(!string.IsNullOrEmpty(uniqueID))
					occasionalFields.UniqueIDPrefix = uniqueID + IdSeparator;
				else
					occasionalFields.UniqueIDPrefix = string.Empty;
			}
			return occasionalFields.UniqueIDPrefix;
		}
		internal bool HasRenderingData() {
			if(!HasControls())
				return HasRenderDelegate();
			return true;
		}
		internal virtual void InitRecursive(DXWebControlBase namingContainer) {
			if(occasionalFields != null && occasionalFields.Controls != null) {
				if(flags[0x80])
					namingContainer = this;
				string errorMsg = occasionalFields.Controls.SetCollectionReadOnly("Parent_collections_readonly");
				int count = occasionalFields.Controls.Count;
				for(int i = 0; i < count; i++) {
					DXWebControlBase control = occasionalFields.Controls[i];
					control.UpdateNamingContainer(namingContainer);
					if(control.id == null && namingContainer != null && !control.flags[0x40])
						control.GenerateAutomaticID();
					control.InitRecursive(namingContainer);
				}
				occasionalFields.Controls.SetCollectionReadOnly(errorMsg);
			}
			if(controlState < DXWebControlState.Initialized) {
				controlState = DXWebControlState.ChildrenInitialized;
				OnInit(EventArgs.Empty);
				controlState = DXWebControlState.Initialized;
			}
			TrackViewState();
		}
		internal bool IsDescendentOf(DXWebControlBase ancestor) {
			DXWebControlBase parent = this;
			while(parent != ancestor && parent.Parent != null)
				parent = parent.Parent;
			return parent == ancestor;
		}
		internal virtual void LoadRecursive() {
			if(controlState < DXWebControlState.Loaded)
				OnLoad(EventArgs.Empty);
			if(occasionalFields != null && occasionalFields.Controls != null) {
				string errorMsg = occasionalFields.Controls.SetCollectionReadOnly("Parent_collections_readonly");
				int count = occasionalFields.Controls.Count;
				for(int i = 0; i < count; i++)
					occasionalFields.Controls[i].LoadRecursive();
				occasionalFields.Controls.SetCollectionReadOnly(errorMsg);
			}
			if(controlState < DXWebControlState.Loaded)
				controlState = DXWebControlState.Loaded;
		}
		internal virtual void PreRenderRecursiveInternal() {
			if(!Visible)
				flags.Set(0x10);
			else {
				flags.Clear(0x10);
				EnsureChildControls();
				OnPreRender(EventArgs.Empty);
				if(occasionalFields != null && occasionalFields.Controls != null) {
					string errorMsg = occasionalFields.Controls.SetCollectionReadOnly("Parent_collections_readonly");
					int count = occasionalFields.Controls.Count;
					for(int i = 0; i < count; i++)
						occasionalFields.Controls[i].PreRenderRecursiveInternal();
					occasionalFields.Controls.SetCollectionReadOnly(errorMsg);
				}
			}
			controlState = DXWebControlState.PreRendered;
		}
		internal void PreventAutoID() {
			if(!flags[0x80])
				flags.Set(0x40);
		}
		internal void RenderChildrenInternal(DXHtmlTextWriter writer, ICollection children) {
			if(RareFields != null && RareFields.RenderMethod != null) {
				writer.BeginRender();
				RareFields.RenderMethod(writer, this);
				writer.EndRender();
			} else if(children != null)
				foreach(DXWebControlBase control in children)
					control.RenderControl(writer);
		}
		protected internal virtual void AddedControl(DXWebControlBase control, int index) {
			if(control.parent != null)
				control.parent.Controls.Remove(control);
			control.parent = this;
			control.flags.Clear(0x20000);
			DXWebControlBase namingContainerCurrent = flags[0x80] ? this : namingContainer;
			if(namingContainerCurrent != null) {
				control.UpdateNamingContainer(namingContainerCurrent);
				if((control.id == null) && !control.flags[0x40])
					control.GenerateAutomaticID();
				else if((control.id != null) || ((control.occasionalFields != null) && (control.occasionalFields.Controls != null)))
					namingContainerCurrent.DirtyNameTable();
			}
			if(controlState >= DXWebControlState.ChildrenInitialized) {
				control.InitRecursive(namingContainerCurrent);
				if(controlState >= DXWebControlState.ViewStateLoaded) {
					object savedState = null;
					if(occasionalFields != null && occasionalFields.ControlsViewState != null) {
						savedState = occasionalFields.ControlsViewState[index];
						savedState = occasionalFields.ControlsViewState[index];
						occasionalFields.ControlsViewState.Remove(index);
					}
					if(controlState >= DXWebControlState.Loaded) {
						control.LoadRecursive();
						if(controlState >= DXWebControlState.PreRendered)
							control.PreRenderRecursiveInternal();
					}
				}
			}
		}
		protected virtual void AddParsedSubObject(object obj) {
			DXWebControlBase child = obj as DXWebControlBase;
			if(child != null)
				Controls.Add(child);
		}
		protected internal virtual void OnInit(EventArgs e) {
		}
		protected internal virtual void OnLoad(EventArgs e) {
		}
		protected internal virtual void OnPreRender(EventArgs e) {
		}
		protected internal virtual void OnUnload(EventArgs e) {
		}
		protected void BuildProfileTree(bool calcViewState) {
			if(occasionalFields != null && occasionalFields.Controls != null)
				foreach(DXWebControlBase control in occasionalFields.Controls)
					control.BuildProfileTree(calcViewState);
		}
		protected void ClearChildViewState() {
			if(occasionalFields != null)
				occasionalFields.ControlsViewState = null;
		}
		protected internal virtual void CreateChildControls() {
		}
		protected virtual DXWebControlCollection CreateControlCollection() {
			return new DXWebControlCollection(this);
		}
		protected virtual void EnsureChildControls() {
			if(!ChildControlsCreated && !flags[0x100]) {
				flags.Set(0x100);
				try {
					ChildControlsCreated = true;
				} finally {
					flags.Clear(0x100);
				}
			}
		}
		protected void EnsureID() {
			if(namingContainer != null) {
				if(id == null)
					GenerateAutomaticID();
				flags.Set(0x800);
			}
		}
		protected virtual DXWebControlBase FindControl(string id, int pathOffset) {
			string str;
			EnsureChildControls();
			if(!flags[0x80]) {
				DXWebControlBase namingContainer = NamingContainer;
				if(namingContainer != null)
					return namingContainer.FindControl(id, pathOffset);
				return null;
			}
			if(HasControls() && occasionalFields.NamedControls == null)
				EnsureNamedControlsTable();
			if(occasionalFields == null || occasionalFields.NamedControls == null)
				return null;
			char[] anyOf = new char[] { '$', ':' };
			int num = id.IndexOfAny(anyOf, pathOffset);
			if(num == -1) {
				str = id.Substring(pathOffset);
				return (occasionalFields.NamedControls[str] as DXWebControlBase);
			}
			str = id.Substring(pathOffset, num - pathOffset);
			DXWebControlBase control2 = occasionalFields.NamedControls[str] as DXWebControlBase;
			if(control2 == null)
				return null;
			return control2.FindControl(id, num + 1);
		}
		protected bool IsLiteralContent() {
			return occasionalFields != null && occasionalFields.Controls != null && occasionalFields.Controls.Count == 1 && occasionalFields.Controls[0] is DXHtmlLiteralControl;
		}
		protected internal virtual void LoadControlState(object savedState) {
		}
		protected virtual bool OnBubbleEvent(object source, EventArgs args) {
			return false;
		}
		protected virtual void OnDataBinding(EventArgs e) {
		}
		protected void RaiseBubbleEvent(object source, EventArgs args) {
			for(DXWebControlBase control = Parent; control != null; control = control.Parent)
				if(control.OnBubbleEvent(source, args))
					return;
		}
		protected internal virtual void Render(DXHtmlTextWriter writer) {
			RenderChildren(writer);
		}
		protected internal virtual void RenderChildren(DXHtmlTextWriter writer) {
			ICollection children = occasionalFields == null ? null : (ICollection)occasionalFields.Controls;
			RenderChildrenInternal(writer, children);
		}
		protected virtual void TrackViewState() {
			if(viewState != null)
				viewState.TrackViewState();
			flags.Set(2);
		}
		void UpdateNamingContainer(DXWebControlBase namingContainer) {
			if(this.namingContainer != null && this.namingContainer != namingContainer)
				ClearCachedUniqueIDRecursive();
			this.namingContainer = namingContainer;
		}
		void ClearCachedUniqueIDRecursive() {
			cachedUniqueID = null;
			if(occasionalFields != null) {
				occasionalFields.UniqueIDPrefix = null;
				if(occasionalFields.Controls != null) {
					int count = occasionalFields.Controls.Count;
					for(int i = 0; i < count; i++)
						occasionalFields.Controls[i].ClearCachedUniqueIDRecursive();
				}
			}
		}
		void EnsureNamedControlsTable() {
			occasionalFields.NamedControls = new Dictionary<string, DXWebControlBase>(occasionalFields.NamedControlsID);
			FillNamedControlsTable(this, occasionalFields.Controls);
		}
		void EnsureOccasionalFields() {
			if(occasionalFields == null)
				occasionalFields = new OccasionalFields();
		}
		void FillNamedControlsTable(DXWebControlBase namingContainer, DXWebControlCollection controls) {
			foreach(DXWebControlBase control in controls) {
				if(control.id != null) {
					try {
						namingContainer.EnsureOccasionalFields();
						namingContainer.occasionalFields.NamedControls.Add(control.id, control);
					} catch {
						throw new Exception("Duplicate id used");
					}
				}
				if(control.HasControls() && !control.flags[0x80])
					FillNamedControlsTable(namingContainer, control.Controls);
			}
		}
		void GenerateAutomaticID() {
			flags.Set(0x200000);
			namingContainer.EnsureOccasionalFields();
			int index = namingContainer.occasionalFields.NamedControlsID++;
			id = "_ctl" + index.ToString(NumberFormatInfo.InvariantInfo);
			namingContainer.DirtyNameTable();
		}
		void RenderControlInternal(DXHtmlTextWriter writer) {
			Render(writer);
		}
		bool HasRenderDelegate() {
			return RareFields != null && RareFields.RenderMethod != null;
		}
	}
}
