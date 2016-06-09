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
using System.Linq;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Native;
using DevExpress.Data.Browsing.Design;
using DevExpress.XtraPrinting.Localization;
using DevExpress.Data.Browsing;
using System.ComponentModel;
namespace DevExpress.XtraReports.Design.BindingMapper {
	public abstract class BindingInfo {
		DesignBinding designBinding;
		string displayName = null;
		IServiceProvider servProvider;
		protected DesignBinding DesignBinding { 
			get { return designBinding; } 
			set { 
				designBinding = value;
				displayName = null;
			}
		}
		public string PropertyName { get; private set; }
		public BindingInfo(IServiceProvider servProvider, string propertyName) {
			PropertyName = propertyName;
			this.servProvider = servProvider;
		}
		string DisplayName {
			get {
				if(displayName == null) {
					if(DesignBinding != null)
						displayName = DesignBinding.GetDisplayName(servProvider);
					if(string.IsNullOrEmpty(displayName))
						displayName = PreviewLocalizer.GetString(PreviewStringId.NoneString);
				}
				return displayName;
			}
		}
		public abstract void AssignFrom(DesignBinding source);
		public override string ToString() {
			return DisplayName;
		}
	}
	class XRBindingInfo : BindingInfo {
		XRBinding XRBinding { get; set; }
		public XRBindingInfo(IServiceProvider servProvider, XRBinding binding)
			: base(servProvider, XRComponentPropertyNames.DataBindings) {
			XRBinding = binding;
			DesignBinding = new DesignBinding(binding.DataSource, binding.DataMember);
		}
		public override void AssignFrom(DesignBinding source) {
			if(!source.IsNull)
				XRBinding.Assign(source.DataSource, source.DataMember);
			else if(XRBinding.Control != null)
				XRBinding.Control.DataBindings.Remove(XRBinding);
		}
	}
	class DataMemberInfo : BindingInfo {
		IDataContainer DataContainer { get; set; }
		public DataMemberInfo(IServiceProvider servProvider, IDataContainer dataContainer)
			: base(servProvider, XRComponentPropertyNames.DataMember) {
			DataContainer = dataContainer;
			DesignBinding = new DesignBinding(dataContainer.GetEffectiveDataSource(), dataContainer.DataMember);
		}
		public override void AssignFrom(DesignBinding source) {
			DataContainer.DataMember = source.DataMember;
		}
	}
	public class DesignBindingInfo : BindingInfo {
		public new DesignBinding DesignBinding { get { return base.DesignBinding; } set { base.DesignBinding = value; } }
		public DesignBindingInfo(IServiceProvider servProvider, object dataSource, string dataMember)
			: base(servProvider, string.Empty) {
			DesignBinding = new DesignBinding(dataSource, dataMember);
		}
		public DesignBindingInfo(IServiceProvider servProvider)
			: this(servProvider, null, null) {
		}
		public override void AssignFrom(DesignBinding source) {
			DesignBinding = source;
		}
		public virtual DesignTreeListBindingPicker CreatePicker() {
			return new DesignTreeListBindingPicker(new TreeListPickManager(new DataContextOptions(true, true)));
		}
	}
	class DesignDataMemberInfo : DesignBindingInfo {
		public DesignDataMemberInfo(IServiceProvider servProvider) : base(servProvider) {
		}
		public DesignDataMemberInfo(IServiceProvider servProvider, object dataSource, string dataMember)
			: base(servProvider, dataSource, dataMember) {
		}
		public override DesignTreeListBindingPicker CreatePicker() {
			return new TreeListPicker();
		}
	}
	public class BindingMapInfo {
		public bool IsChecked { get; set; }
		public bool IsValid { get; set; }
		public string ControlName { get; private set; }
		public string PropertyName { get; private set; }
		public BindingInfo Source { get; set; }
		public DesignBindingInfo Destination { get; set; }
		public XRControl Control { get; set; }
		public BindingMapInfo(bool isChecked, bool isValid, string controlName, string propertyName, BindingInfo source, DesignBindingInfo destination, XRControl control) {
			IsChecked = isChecked;
			IsValid = isValid;
			ControlName = controlName;
			PropertyName = propertyName;
			Source = source;
			Destination = destination;
			Control = control;
		}
	}
}
