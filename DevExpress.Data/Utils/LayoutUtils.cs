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
using DevExpress.Utils.Controls;
using DevExpress.Compatibility.System.ComponentModel;
#if !SILVERLIGHT
using DevExpress.Utils.Serializing;
using System.Collections.Generic;
#endif
namespace DevExpress.Utils {
	public class LayoutUpgradeEventArgs : EventArgs {
		string previousVersion;
		public LayoutUpgradeEventArgs(string previousVersion) {
			this.previousVersion = previousVersion;
		}
		public string PreviousVersion { get { return previousVersion; } }
		public LayoutUpgradeEventArgs(string previousVersion, object[] newHiddenItems) {
			this.previousVersion = previousVersion;
			this.items = newHiddenItems;
		}
		object[] items;
		public object[] NewHiddenItems { get { return items; } }
	}
	public class LayoutAllowEventArgs : LayoutUpgradeEventArgs {
		bool allow;
		public LayoutAllowEventArgs(string previousVersion)
			: base(previousVersion) {
			this.allow = true;
		}
		public bool Allow { get { return allow; } set { allow = value; } }
	}
	public delegate void LayoutAllowEventHandler(object sender, LayoutAllowEventArgs e);
	public delegate void LayoutUpgradeEventHandler(object sender, LayoutUpgradeEventArgs e);
	public class OptionsLayoutBase : BaseOptions {
		static OptionsLayoutBase fullLayout;
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutBaseFullLayout"),
#endif
 NotifyParentProperty(true), AutoFormatDisable]
		public static OptionsLayoutBase FullLayout {
			get {
				if(fullLayout == null) fullLayout = new OptionsLayoutBase();
				return fullLayout;
			}
		}
		string layoutVersion = "";
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutBaseLayoutVersion"),
#endif
 DefaultValue(""), NotifyParentProperty(true), AutoFormatDisable]
		public string LayoutVersion { get { return layoutVersion; } set { layoutVersion = value; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShouldSerializeCore(IComponent owner) { return ShouldSerialize(owner); }
		public override void Assign(BaseOptions source) {
			base.Assign(source);
			OptionsLayoutBase options = source as OptionsLayoutBase;
			if (options != null) {
				LayoutVersion = options.LayoutVersion;
			}
		}
	}
#if !SILVERLIGHT
	public class OptionsColumnLayout : BaseOptions {
		bool storeLayout, storeAppearance, storeAllOptions,
			addNewColumns, removeOldColumns;
		public OptionsColumnLayout() {
			this.addNewColumns = true;
			this.removeOldColumns = true;
			this.storeLayout = true;
			this.storeAppearance = false;
			this.storeAllOptions = false;
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsColumnLayoutRemoveOldColumns"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.OptionsColumnLayout.RemoveOldColumns"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable
		]
		public bool RemoveOldColumns {
			get { return removeOldColumns; }
			set {
				if(RemoveOldColumns == value) return;
				bool prevValue = RemoveOldColumns;
				removeOldColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("RemoveOldColumns", prevValue, RemoveOldColumns));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsColumnLayoutAddNewColumns"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.OptionsColumnLayout.AddNewColumns"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable
		]
		public bool AddNewColumns {
			get { return addNewColumns; }
			set {
				if(AddNewColumns == value) return;
				bool prevValue = AddNewColumns;
				addNewColumns = value;
				OnChanged(new BaseOptionChangedEventArgs("AddNewColumns", prevValue, AddNewColumns));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsColumnLayoutStoreLayout"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.OptionsColumnLayout.StoreLayout"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(true), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable
		]
		public bool StoreLayout {
			get { return storeLayout; }
			set {
				if(StoreLayout == value) return;
				bool prevValue = StoreLayout;
				storeLayout = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreLayout", prevValue, StoreLayout));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsColumnLayoutStoreAppearance"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.OptionsColumnLayout.StoreAppearance"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable
		]
		public bool StoreAppearance {
			get { return storeAppearance; }
			set {
				if(StoreAppearance == value) return;
				bool prevValue = StoreAppearance;
				storeAppearance = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreAppearance", prevValue, StoreAppearance));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsColumnLayoutStoreAllOptions"),
#endif
		DXDisplayName(typeof(DevExpress.Data.ResFinder), "DevExpress.Utils.OptionsColumnLayout.StoreAllOptions"),
		TypeConverter(typeof(DevExpress.Utils.Design.BooleanTypeConverter)),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true), AutoFormatDisable
		]
		public bool StoreAllOptions {
			get { return storeAllOptions; }
			set {
				if(StoreAllOptions == value) return;
				bool prevValue = StoreAllOptions;
				storeAllOptions = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreAllOptions", prevValue, StoreAllOptions));
			}
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				OptionsColumnLayout opt = options as OptionsColumnLayout;
				if(opt == null) return;
				this.storeAppearance = opt.StoreAppearance;
				this.storeLayout = opt.StoreLayout;
				this.storeAllOptions = opt.StoreAllOptions;
				this.addNewColumns = opt.AddNewColumns;
				this.removeOldColumns = opt.RemoveOldColumns;
			} finally {
				EndUpdate();
			}
		}
		protected internal new bool ShouldSerialize(IComponent owner) { return base.ShouldSerialize(owner); }
	}
	public class OptionsLayoutGrid : OptionsLayoutBase {
		bool storeAppearance, storeVisualOptions, storeAllOptions, storeDataSettings, storeFormatRules;
		OptionsColumnLayout columns;
		public OptionsLayoutGrid() {
			this.storeAppearance = false;
			this.storeVisualOptions = true;
			this.storeAllOptions = false;
			this.storeDataSettings = true;
			this.storeFormatRules = false;
			this.columns = CreateOptionsColumn();
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutGridStoreAppearance"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		NotifyParentProperty(true), AutoFormatDisable]
		public bool StoreAppearance {
			get { return storeAppearance; }
			set {
				if(StoreAppearance == value) return;
				bool prevValue = StoreAppearance;
				storeAppearance = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreAppearance", prevValue, StoreAppearance));
			}
		}
		[ DefaultValue(false), XtraSerializableProperty(),
		NotifyParentProperty(true), AutoFormatDisable]
		public bool StoreFormatRules {
			get { return storeFormatRules; }
			set {
				if(StoreFormatRules == value) return;
				bool prevValue = StoreFormatRules;
				storeFormatRules = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreFormatRules", prevValue, StoreFormatRules));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutGridStoreVisualOptions"),
#endif
 DefaultValue(true), XtraSerializableProperty(),
		NotifyParentProperty(true), AutoFormatDisable]
		public bool StoreVisualOptions {
			get { return storeVisualOptions; }
			set {
				if(StoreVisualOptions == value) return;
				bool prevValue = StoreVisualOptions;
				storeVisualOptions = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreVisualOptions", prevValue, StoreVisualOptions));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutGridStoreAllOptions"),
#endif
 DefaultValue(false), XtraSerializableProperty(),
		NotifyParentProperty(true), AutoFormatDisable]
		public bool StoreAllOptions {
			get { return storeAllOptions; }
			set {
				if(StoreAllOptions == value) return;
				bool prevValue = StoreAllOptions;
				storeAllOptions = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreAllOptions", prevValue, StoreAllOptions));
			}
		}
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutGridStoreDataSettings"),
#endif
 DefaultValue(true), XtraSerializableProperty(),
		NotifyParentProperty(true), AutoFormatDisable]
		public bool StoreDataSettings {
			get { return storeDataSettings; }
			set {
				if(StoreDataSettings == value) return;
				bool prevValue = StoreDataSettings;
				storeDataSettings = value;
				OnChanged(new BaseOptionChangedEventArgs("StoreDataSettings", prevValue, StoreDataSettings));
			}
		}
		bool ShouldSerializeColumns() { return Columns.ShouldSerialize(null); }
		[
#if !SL
	DevExpressDataLocalizedDescription("OptionsLayoutGridColumns"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		NotifyParentProperty(true), AutoFormatDisable]
		public OptionsColumnLayout Columns {
			get { return columns; }
		}
		public override void Assign(BaseOptions options) {
			BeginUpdate();
			try {
				base.Assign(options);
				OptionsLayoutGrid opt = options as OptionsLayoutGrid;
				if(opt == null) return;
				this.storeFormatRules = opt.StoreFormatRules;
				this.storeAppearance = opt.StoreAppearance;
				this.storeVisualOptions = opt.StoreVisualOptions;
				this.storeAllOptions = opt.StoreAllOptions;
				this.storeDataSettings = opt.StoreDataSettings;
				this.columns.Assign(opt.Columns);
			} finally {
				EndUpdate();
			}
		}
		public override void Reset() {
			BeginUpdate();
			try {
				base.Reset();
				Columns.Reset();
			} finally {
				EndUpdate();
			}
		}
		protected virtual OptionsColumnLayout CreateOptionsColumn() {
			return new OptionsColumnLayout();
		}
	}
#endif
}
