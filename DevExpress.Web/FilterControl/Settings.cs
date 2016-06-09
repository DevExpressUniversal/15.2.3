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
using System.Linq;
using System.Text;
using System.Web.UI;
using DevExpress.Web.Internal;
namespace DevExpress.Web {
	public enum FilterControlViewMode { Visual, VisualAndText }
	public enum FilterControlDataType { Object, List }
	public class FilterControlSettings : PropertiesBase {
		protected internal const int DefaultMaxHierarchyDepth = 4;
		FilterControlGroupOperationsVisibility groupOperationsVisibility;
		public FilterControlSettings(IPropertiesOwner owner)
			: base(owner) {
		}
		protected internal FilterControlViewMode ViewMode {
			get { return (FilterControlViewMode)GetEnumProperty("ViewMode", FilterControlViewMode.Visual); }
			set {
				if(value == ViewMode)
					return;
				SetEnumProperty("ViewMode", FilterControlViewMode.Visual, value);
				Changed();
			}
		}
		protected internal bool AllowHierarchicalColumns {
			get { return GetBoolProperty("AllowHierarchicalColumns", false); }
			set {
				if(value == AllowHierarchicalColumns)
					return;
				SetBoolProperty("AllowHierarchicalColumns", false, value);
				Changed();
			}
		}
		protected internal virtual bool ShowAllDataSourceColumns {
			get { return GetBoolProperty("ShowAllDataSourceColumns", false); }
			set {
				if(value == ShowAllDataSourceColumns)
					return;
				SetBoolProperty("ShowAllDataSourceColumns", false, value);
				Changed();
			}
		}
		protected internal int MaxHierarchyDepth {
			get { return GetIntProperty("MaxHierarchyDepth", DefaultMaxHierarchyDepth); }
			set {
				if(value == MaxHierarchyDepth)
					return;
				SetIntProperty("MaxHierarchyDepth", DefaultMaxHierarchyDepth, value);
				Changed();
			}
		}
		protected internal bool ShowOperandTypeButton {
			get { return GetBoolProperty("ShowOperandTypeButton", false); }
			set {
				if(value == ShowOperandTypeButton)
					return;
				SetBoolProperty("ShowOperandTypeButton", false, value);
				Changed();
			}
		}
		protected internal FilterControlGroupOperationsVisibility GroupOperationsVisibility {
			get {
				if(groupOperationsVisibility == null)
					groupOperationsVisibility = new FilterControlGroupOperationsVisibility(Owner);
				return groupOperationsVisibility;
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as FilterControlSettings;
			if(src != null) {
				GroupOperationsVisibility.Assign(src.GroupOperationsVisibility);
				ViewMode = src.ViewMode;
				AllowHierarchicalColumns = src.AllowHierarchicalColumns;
				ShowAllDataSourceColumns = src.ShowAllDataSourceColumns;
				MaxHierarchyDepth = src.MaxHierarchyDepth;
				ShowOperandTypeButton = src.ShowOperandTypeButton;
			}
		}
		protected override IStateManager[] GetStateManagedObjects() {
			return ViewStateUtils.GetMergedStateManagedObjects(base.GetStateManagedObjects(), new IStateManager[] { GroupOperationsVisibility });
		}
	}
	public class FilterControlGroupOperationsVisibility : PropertiesBase {
		public FilterControlGroupOperationsVisibility(IPropertiesOwner owner) : base(owner) { }
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityAnd"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool And {
			get { return GetBoolProperty("And", true); }
			set {
				if(value == And)
					return;
				SetBoolProperty("And", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityOr"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Or {
			get { return GetBoolProperty("Or", true); }
			set {
				if(value == Or)
					return;
				SetBoolProperty("Or", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityNotAnd"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool NotAnd {
			get { return GetBoolProperty("NotAnd", true); }
			set {
				if(value == NotAnd)
					return;
				SetBoolProperty("NotAnd", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityNotOr"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool NotOr {
			get { return GetBoolProperty("NotOr", true); }
			set {
				if(value == NotOr)
					return;
				SetBoolProperty("NotOr", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityAddGroup"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool AddGroup {
			get { return GetBoolProperty("AddGroup", true); }
			set {
				if(value == AddGroup)
					return;
				SetBoolProperty("AddGroup", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityAddCondition"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool AddCondition {
			get { return GetBoolProperty("AddCondition", true); }
			set {
				if(value == AddCondition)
					return;
				SetBoolProperty("AddCondition", true, value);
				Changed();
			}
		}
		[
#if !SL
	DevExpressWebLocalizedDescription("FilterControlGroupOperationsVisibilityRemove"),
#endif
 NotifyParentProperty(true), DefaultValue(true), AutoFormatDisable]
		public bool Remove {
			get { return GetBoolProperty("Remove", true); }
			set {
				if(value == Remove)
					return;
				SetBoolProperty("Remove", true, value);
				Changed();
			}
		}
		public override void Assign(PropertiesBase source) {
			base.Assign(source);
			var src = source as FilterControlGroupOperationsVisibility;
			if(src != null) {
				And = src.And;
				Or = src.Or;
				NotAnd = src.NotAnd;
				NotOr = src.NotOr;
				AddGroup = src.AddGroup;
				AddCondition = src.AddCondition;
				Remove = src.Remove;
			}
		}
	}
}
