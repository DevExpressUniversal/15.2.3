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
using System.Text;
using DevExpress.XtraPivotGrid;
using DevExpress.LookAndFeel;
using DevExpress.LookAndFeel.DesignService;
using DevExpress.XtraReports.UI.PivotGrid;
using DevExpress.Data.Filtering;
using DevExpress.XtraEditors.Filtering;
using System.Windows.Forms;
using DevExpress.XtraPivotGrid.Data;
namespace DevExpress.XtraReports.Design {
	public class PivotCriteriaEditor : CriteriaEditorBase {
		public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, IServiceProvider provider, object value) {
			return base.EditValue(context, provider, value);
		}
		protected override UserLookAndFeel GetLookAndFeel(PrefilterBase prefilter, IServiceProvider provider) {
			ILookAndFeelService serv = provider.GetService(typeof(ILookAndFeelService)) as ILookAndFeelService;
			return serv != null ? serv.LookAndFeel : null;
		}
		protected override IPrefilterFormOwner GetPrefilterFormOwner(PrefilterBase prefilter) {
			return new PrefilterFormOwner((XRPrefilter)prefilter);
		}
	}
	class PrefilterFormOwner : IPrefilterFormOwner {
		XRPrefilter prefilter;
		FilteredComponent filteredComponent;
		public PrefilterFormOwner(XRPrefilter prefilter) {
			this.prefilter = prefilter;
		}
		Control IPrefilterFormOwner.ControlOwner {
			get { return null; }
		}
		IFilteredComponent IPrefilterFormOwner.FilteredComponent {
			get {
				if(filteredComponent == null)
					filteredComponent = new FilteredComponent(prefilter);
				return filteredComponent;
			}
		}
		void IPrefilterFormOwner.SetPrefilterVisible(bool visible) { }
		bool IPrefilterFormOwner.ShowOperandTypeIcon {
			get { return false; }
		}
	}
	class FilteredComponent : IFilteredComponent {
		XRPrefilter prefilter;
		public FilteredComponent(XRPrefilter prefilter) {
			this.prefilter = prefilter;
		}
		IBoundPropertyCollection IFilteredComponent.CreateFilterColumnCollection() {
			return new FieldFilterColumnCollection(prefilter.Data.Fields, prefilter.PrefilterColumnNames, null);
		}
		IFilteredComponentBase Component {
			get {
				return (IFilteredComponentBase)prefilter.Data;
			}
		}
		event EventHandler IFilteredComponentBase.PropertiesChanged {
			add { Component.PropertiesChanged += value; }
			remove { Component.PropertiesChanged -= value; }
		}
		event EventHandler IFilteredComponentBase.RowFilterChanged {
			add { Component.RowFilterChanged += value; }
			remove { Component.RowFilterChanged -= value; }
		}
		CriteriaOperator IFilteredComponentBase.RowCriteria {
			get {
				return Component.RowCriteria;
			}
			set {
				Component.RowCriteria = value;
			}
		}
	}
}
