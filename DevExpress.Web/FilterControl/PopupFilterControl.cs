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
using System.Web;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Linq;
using DevExpress.Web;
using DevExpress.Web.Internal;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.XtraEditors.Filtering;
using DevExpress.Data;
using DevExpress.Data.Filtering;
namespace DevExpress.Web.FilterControl {
	public interface IFilterControlRowOwner : IFilterControlOwner {
		string GetJavaScriptForShowFilterControl();
		string GetJavaScriptForClearFilter();
		string GetJavaScriptForSetFilterEnabledForCheckbox();
		bool IsFilterEnabledSupported { get; }
		bool IsFilterEnabled { get; }
		string ClearButtonText { get; }
		string ShowFilterBuilderText { get; }
		void AssignFilterStyleToControl(WebControl control);
		void AssignLinkStyleToControl(WebControl control);
		void AssignCheckBoxCellStyleToControl(WebControl control);
		void AssignImageCellStyleToControl(WebControl control);
		void AssignExpressionCellStyleToControl(WebControl control);
		void AssignClearButtonCellStyleToControl(WebControl control);
		void AppendDefaultDXClassName(WebControl control);
		ImageProperties CreateFilterImage { get; }
		void RaiseCustomFilterExpressionDisplayText(CustomFilterExpressionDisplayTextEventArgs e);
	}
	public interface IPopupFilterControlOwner : IFilterControlRowOwner {
		FilterControlImages GetImages();
		FilterControlStyles GetStyles();
		EditorImages GetImagesEditors();
		EditorStyles GetStylesEditors();
		object GetControlCallbackResult();
		void CloseFilterControl();
		ASPxWebControl OwnerControl { get; }
		string MainElementID { get; }
		string GetJavaScriptForApplyFilterControl();
		string GetJavaScriptForCloseFilterControl();
		string FilterPopupHeaderText { get; }
		bool EnablePopupMenuScrolling { get; }
		bool EnableCallBacks { get; }
		SettingsLoadingPanel SettingsLoadingPanel { get; }
	}
	public interface IPopupFilterControlStyleOwner {
		ImageProperties CloseButtonImage { get; }		
		AppearanceStyle CloseButtonStyle { get; }
		AppearanceStyle HeaderStyle { get; }		
		AppearanceStyle MainAreaStyle { get; }
		AppearanceStyle ButtonAreaStyle { get; }
		AppearanceStyleBase ModalBackgroundStyle { get; }
	}
	[ToolboxItem(false)]
	public class ASPxPopupFilterControl : ASPxFilterControlBase {
	#region IBoundPropertyWrappers
		public class PopupBoundPropertyWrapper : IBoundProperty, IFilterColumn {
			IFilterColumn filterColumn;
			public PopupBoundPropertyWrapper(IFilterColumn filterColumn) {
				this.filterColumn = filterColumn;
			}
			#region IBoundProperty Members
			List<IBoundProperty> IBoundProperty.Children { get { return new List<IBoundProperty>(); } }
			string IBoundProperty.DisplayName { get { return filterColumn.DisplayName; }  }
			bool IBoundProperty.HasChildren { get { return false; } }
			bool IBoundProperty.IsAggregate { get { return false; } }
			bool IBoundProperty.IsList { get { return false; } }
			string IBoundProperty.Name { get { return filterColumn.PropertyName; } }
			IBoundProperty IBoundProperty.Parent { get { return null; } }
			Type IBoundProperty.Type {get { return filterColumn.PropertyType; } }
			#endregion
			#region IFilterColumn Members
			int IFilterColumn.Index { get { return filterColumn.Index; } }
			FilterColumnClauseClass IFilterColumn.ClauseClass { get { return filterColumn.ClauseClass; } }
			EditPropertiesBase IFilterColumn.PropertiesEdit { get { return filterColumn.PropertiesEdit; } }
			#endregion
			#region IFilterablePropertyInfo Members
			string IFilterablePropertyInfo.PropertyName { get { return filterColumn.PropertyName; } }
			string IFilterablePropertyInfo.DisplayName { get { return filterColumn.DisplayName; } }
			Type IFilterablePropertyInfo.PropertyType { get { return filterColumn.PropertyType; } }
			#endregion
		}
		public class PopupFilterBoundPropertyCollection : IBoundPropertyCollection {
			List<IBoundProperty> list = new List<IBoundProperty>();
			#region IBoundPropertyCollection Members
			void IBoundPropertyCollection.Add(IBoundProperty property) { list.Add(property); }
			void IBoundPropertyCollection.Clear() { list.Clear(); }
			int IBoundPropertyCollection.Count {get { return list.Count; } }
			IBoundPropertyCollection IBoundPropertyCollection.CreateChildrenProperties(IBoundProperty listProperty) {
				return new PopupFilterBoundPropertyCollection();
			}
			string IBoundPropertyCollection.GetDisplayPropertyName(OperandProperty property, string fullPath) {
				if (object.Equals(property, null)) return string.Empty;
				IBoundProperty field = ((IBoundPropertyCollection)this)[property.PropertyName];
				return field != null ? field.GetFullDisplayName() : string.Empty;
			}
			string IBoundPropertyCollection.GetValueScreenText(OperandProperty property, object value) {
				return value != null ? value.ToString() : string.Empty;
			}
			IBoundProperty IBoundPropertyCollection.this[string fieldName] {
				get {
					foreach (IBoundProperty property in list) {
						if (property.Name == fieldName) return property;
					}
					return null;
				}
			}
			IBoundProperty IBoundPropertyCollection.this[int index] {  get { return list[index]; }  }
			#endregion
			#region IEnumerable Members
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() { return list.GetEnumerator(); }
			#endregion
		}
	#endregion
		public const string
			PopupFilterControlFormID = "DXPFCForm",
			PopupFilterControlID = "DXPFC";
		const string ApplyContextKey = "IsApplied";
		IPopupFilterControlOwner filterPopupOwner;
		protected bool IsApplyCalled { 
			get { return GetIsApplyCalled(UniqueID); }
			private set { SetIsApplyCalled(UniqueID, value); }
		}
		public ASPxPopupFilterControl(IPopupFilterControlOwner filterPopupOwner) {
			this.filterPopupOwner = filterPopupOwner;
			EnableCallBacks = FilterPopupOwner.EnableCallBacks;
			EnableViewState = false;
			ViewMode = FilterPopupOwner.ViewMode;
			ShowOperandTypeButton = FilterPopupOwner.ShowOperandTypeButton;
			Model.FilterProperties = FilterPopupOwner.GetFilterColumns();
			FilterExpression = FilterPopupOwner.FilterExpression;
			EnablePopupMenuScrolling = FilterPopupOwner.EnablePopupMenuScrolling;
			ParentSkinOwner = FilterPopupOwner as ISkinOwner;
			ParentImages = FilterPopupOwner.GetImages();
			ParentStyles = FilterPopupOwner.GetStyles();
			SettingsLoadingPanel.Assign(FilterPopupOwner.SettingsLoadingPanel);
			GroupOperationsVisibility.Assign(filterPopupOwner.GroupOperationsVisibility);
		}
		public IPopupFilterControlOwner FilterPopupOwner { get { return filterPopupOwner; } }
		protected internal override FilterControlImages GetImages() { return FilterPopupOwner.GetImages(); }
		protected internal override FilterControlStyles GetStyles() { return FilterPopupOwner.GetStyles(); }
		protected internal override EditorImages GetImagesEditors() { return FilterPopupOwner.GetImagesEditors(); }
		protected internal override EditorStyles GetStylesEditors() { return FilterPopupOwner.GetStylesEditors(); }
		protected override object GetCallbackResult() {
			if(IsApplyCalled) return FilterPopupOwner.GetControlCallbackResult();
			return base.GetCallbackResult();
		}
		protected override void OnAfterFilterApply(bool isClosing) {
			IsApplyCalled = true;
			FilterPopupOwner.FilterExpression = AppliedFilterExpression;
			if(isClosing) {
				FilterPopupOwner.CloseFilterControl();
			}
		}
		protected override void PrepareControlHierarchy() {
			base.PrepareControlHierarchy();
			if(RootTable != null) {
				RootTable.Width = Unit.Percentage(100);
			}
		}
		protected override bool IsOperationHiddenByUserImpl(IFilterablePropertyInfo propertyInfo, ClauseType operation) {
			return FilterPopupOwner.IsOperationHiddenByUser(propertyInfo, operation);
		}
		protected override bool TryConvertValueImpl(IFilterablePropertyInfo propertyInfo, string text, out object value) {
			return FilterPopupOwner.TryConvertValue(propertyInfo, text, out value);
		}
		protected override bool TryGetSpecialValueDisplayTextImpl(IFilterColumn column, object value, bool encodeValue, out string displayText) {
			return FilterPopupOwner.TryGetSpecialValueDisplayText(column, value, encodeValue, out displayText);
		}
		protected override void RaiseCustomValueDisplayTextImpl(FilterControlCustomValueDisplayTextEventArgs e) {
			FilterPopupOwner.RaiseCustomValueDisplayText(e);
		}
		protected override void RaiseCriteriaValueEditorInitializeImpl(FilterControlCriteriaValueEditorInitializeEventArgs e) {
			FilterPopupOwner.RaiseCriteriaValueEditorInitialize(e);
		}
		protected override void RaiseCriteriaValueEditorCreateImpl(FilterControlCriteriaValueEditorCreateEventArgs e) {
			FilterPopupOwner.RaiseCriteriaValueEditorCreate(e);
		}
		protected internal override bool SuppressEditorValidation { get { return true; } }
		static bool GetIsApplyCalled(string fullUniqueId) {
			var key = fullUniqueId + ApplyContextKey;
			return HttpUtils.GetContextValue<bool>(key, false);
		}
		protected internal static bool GetIsApplyCalled(string uniqueId, string separator) {
			var fullUniqueId = string.Join(separator, uniqueId, PopupFilterControlFormID, PopupFilterControlID);
			return GetIsApplyCalled(fullUniqueId);
		}
		protected internal static void SetIsApplyCalled(string fullUniqueId, bool value) {
			var key = fullUniqueId + ApplyContextKey;
			HttpUtils.SetContextValue<bool>(key, value);
		}
	}
}
