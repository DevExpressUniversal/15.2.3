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
using System.Collections.Generic;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using DevExpress.Data;
using DevExpress.Utils.Design;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public abstract class WebColumnBase : CollectionItem, ICaptionSupport, IWebColumn {
		public WebColumnBase()
			: base() {
		}
		#region Properties
		[
#if !SL
	DevExpressWebLocalizedDescription("WebColumnBaseName"),
#endif
		Category("Behavior"), DefaultValue(""), Localizable(false), NotifyParentProperty(true), AutoFormatDisable]
		public string Name {
			get { return GetStringProperty("Name", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(Name == value) return;
				SetStringProperty("Name", string.Empty, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebColumnBaseWidth"),
#endif
		Category("Appearance"), DefaultValue(typeof(Unit), ""), NotifyParentProperty(true), AutoFormatDisable]
		public virtual Unit Width {
			get { return GetUnitProperty("Width", Unit.Empty); }
			set {
				if(value == Width) return;
				SetUnitProperty("Width", Unit.Empty, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebColumnBaseCaption"),
#endif
		Category("Appearance"), DefaultValue(""), RefreshProperties(RefreshProperties.Repaint),
		NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string Caption {
			get { return GetStringProperty("Caption", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				SetStringProperty("Caption", string.Empty, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebColumnBaseToolTip"),
#endif
		Category("Appearance"), DefaultValue(""), NotifyParentProperty(true), Localizable(true), AutoFormatDisable]
		public string ToolTip {
			get { return GetStringProperty("ToolTip", string.Empty); }
			set {
				if(value == null) value = string.Empty;
				if(ToolTip == value) return;
				SetStringProperty("ToolTip", string.Empty, value);
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebColumnBaseVisible"),
#endif
		Category("Appearance"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		public bool Visible {
			get { return GetBoolProperty("ColVisible", true); }
			set {
				if(Visible == value)
					return;
				if(OwnerControl != null) {
					OwnerControl.SetColumnVisible(this, value);
				} else {
					SetColVisible(value);
				}
				OnColumnChanged();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("WebColumnBaseVisibleIndex"),
#endif
		Category("Appearance"), DefaultValue(-1), NotifyParentProperty(true), AutoFormatDisable]
		public virtual int VisibleIndex {
			get {
				if(OwnerControl != null)
					OwnerControl.EnsureVisibleIndices();
				return GetColVisibleIndex();
			}
			set {
				if(OwnerControl != null) {
					OwnerControl.SetColumnVisibleIndex(this, value);
				} else {
					SetColVisibleIndex(value);
				}
				OnColumnChanged();
			}
		}
		[Category("Behavior"), DefaultValue(true), NotifyParentProperty(true), AutoFormatDisable]
		protected internal bool ShowInCustomizationForm {
			get { return GetBoolProperty("ShowInCustomizationForm", true); }
			set {
				if(ShowInCustomizationForm == value)
					return;
				SetBoolProperty("ShowInCustomizationForm", true, value);
				OnColumnChanged();
			}
		}
		protected IWebColumnsOwner OwnerControl {
			get {
				if(Collection != null)
					return Collection.Owner as IWebColumnsOwner;
				return null;
			}
		}
		#endregion
		public override void Assign(CollectionItem source) {
			base.Assign(source);
			WebColumnBase col = source as WebColumnBase;
			Name = col.Name;
			Caption = col.Caption;
			ToolTip = col.ToolTip;
			Width = col.Width;
			ShowInCustomizationForm = col.ShowInCustomizationForm;
			SetColVisible(col.Visible);
			SetColVisibleIndex(col.VisibleIndex);
		}
		protected internal virtual bool GetAllowEllipsisInText() { return false; }
		bool IWebColumn.GetAllowEllipsisInText() { return GetAllowEllipsisInText(); }
		internal int GetColVisibleIndex() {
			return GetIntProperty("ColVisibleIndex", -1);
		}
		public void SetColVisible(bool value) {
			SetBoolProperty("ColVisible", true, value);
			ResetVisibleIndices();
		}
		public void SetColVisibleIndex(int value) {
			SetIntProperty("ColVisibleIndex", -1, value);
			ResetVisibleIndices();
		}
		public virtual bool IsClickable() { return false; }
		public void OnColumnChanged() {
			if(OwnerControl != null)
				OwnerControl.OnColumnChanged(this);
		}
		protected void ResetVisibleIndices() {
			if(OwnerControl != null)
				OwnerControl.ResetVisibleIndices();
		}
		string ICaptionSupport.Caption { get { return ToString(); } }
	}
}
namespace DevExpress.Web.Internal {
	public abstract class WebColumnCollectionBase : Collection {
		public WebColumnCollectionBase(IWebControlObject owner)
			: base(owner) {
		}
	}
	public interface IWebColumnsOwner {
		WebColumnCollectionBase Columns { get; }
		void EnsureVisibleIndices();
		void ResetVisibleIndices();
		void ResetVisibleColumns();
		List<WebColumnBase> GetVisibleColumns();
		void SetColumnVisible(WebColumnBase column, bool value);
		void SetColumnVisibleIndex(WebColumnBase column, int value);
		void OnColumnChanged(WebColumnBase column);
		void OnColumnCollectionChanged();
	}
	public class WebColumnsOwnerDefaultImplementation : IWebColumnsOwner {
		IWebControlObject control;
		List<WebColumnBase> visibleColumns;
		WebColumnCollectionBase columns;
		public WebColumnsOwnerDefaultImplementation(IWebControlObject control, WebColumnCollectionBase columns) {
			this.control = control;
			this.columns = columns;
		}
		protected IWebControlObject Control {
			get { return control; }
		}
		protected virtual int CompareColumnsByVisibleIndex(WebColumnBase col1, WebColumnBase col2) {
			if(col1.VisibleIndex == col2.VisibleIndex)
				return Comparer.Default.Compare(col1.Index, col2.Index);
			if(col1.VisibleIndex < 0) return 1;
			if(col2.VisibleIndex < 0) return -1;
			return Comparer.Default.Compare(col1.VisibleIndex, col2.VisibleIndex);
		}
		protected List<WebColumnBase> BuildVisibleColumns() {
			List<WebColumnBase> result = new List<WebColumnBase>();
			foreach(WebColumnBase column in Columns) {
				if(column.Visible)
					result.Add(column);
			}
			result.Sort(new Comparison<WebColumnBase>(CompareColumnsByVisibleIndex));
			return result;
		}
		#region Implementation
		public WebColumnCollectionBase Columns { get { return columns; } }
		bool lockEnsureVisibleIndices = false;
		public void EnsureVisibleIndices() {
			if(this.lockEnsureVisibleIndices) return;
			var occupiedIndices = new HashSet<int>();
			var targetColumns = new List<WebColumnBase>();
			int index = 0;
			foreach(WebColumnBase column in Columns) {
				index = column.GetColVisibleIndex();
				if(index >= 0)
					occupiedIndices.Add(index);
				else if(column.Visible)
					targetColumns.Add(column);
			}
			int visibleIndex = 0;
			foreach(var column in targetColumns) {
				while(occupiedIndices.Contains(visibleIndex))
					visibleIndex++;
				column.SetColVisibleIndex(visibleIndex);
				visibleIndex++;
			}
			this.lockEnsureVisibleIndices = true;
		}
		public void ResetVisibleIndices() {
			this.lockEnsureVisibleIndices = false;
		}
		public virtual void ResetVisibleColumns() {
			this.visibleColumns = null;
			this.lockEnsureVisibleIndices = false;
		}
		public List<WebColumnBase> GetVisibleColumns() {
			if(this.visibleColumns == null) {
				EnsureVisibleIndices();
				this.visibleColumns = BuildVisibleColumns();
			}
			return this.visibleColumns;
		}
		public void SetColumnVisible(WebColumnBase column, bool value) {
			column.SetColVisible(value);
			this.visibleColumns = null;
			Control.LayoutChanged();
		}
		public void SetColumnVisibleIndex(WebColumnBase column, int value) {
			if(column.Visible && value == column.VisibleIndex)
				return;
			if(value < 0) {
				column.SetColVisible(false);
				value = column.VisibleIndex;
			} else {
				column.SetColVisible(true);
				foreach(WebColumnBase item in Columns) {
					if(item.VisibleIndex > column.VisibleIndex)
						item.SetColVisibleIndex(item.VisibleIndex - 1);
				}
				foreach(WebColumnBase item in Columns) {
					if(item.VisibleIndex >= value)
						item.SetColVisibleIndex(item.VisibleIndex + 1);
				}
			}
			column.SetColVisibleIndex(value);
			this.visibleColumns = null;
			Control.LayoutChanged();
		}
		public void OnColumnChanged(WebColumnBase column) {
			ResetVisibleIndices();
			if(!Control.IsLoading())
				Control.LayoutChanged();
		}
		public void OnColumnCollectionChanged() {
			ResetVisibleColumns();
			ResetVisibleIndices();
			if(!Control.IsLoading())
				Control.LayoutChanged();
		}
		#endregion
	}
}
