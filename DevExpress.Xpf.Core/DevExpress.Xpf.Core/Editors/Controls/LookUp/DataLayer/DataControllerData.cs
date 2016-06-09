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
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.Data.Filtering.Helpers;
using DevExpress.Data.Linq;
using DevExpress.Utils;
using System.Windows.Data;
using System.Windows;
#if !SL
using DevExpress.Data.Access;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors.Internal;
using DevExpress.Xpf.Editors.Native;
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data;
using DevExpress.Data.Helpers;
using System.Windows.Controls;
using DevExpress.Data.Filtering;
#else
using DevExpress.Xpf.Editors.Popups;
using DevExpress.Xpf.Editors.Settings;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Data.Browsing;
using DevExpress.Data.Access;
using DevExpress.Data;
using DevExpress.Xpf.Core.Native;
using DevExpress.Data.Helpers;
using System.Windows.Controls;
using DevExpress.Xpf.Core;
using DevExpress.Data.Filtering;
#endif
#if SL
using ContextMenu = System.Windows.Controls.SLContextMenu;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using SelectionChangedEventArgs = DevExpress.Xpf.Editors.WPFCompatibility.SLSelectionChangedEventArgs;
using TextBox = DevExpress.Xpf.Editors.Controls.SLTextBox;
using DevExpress.Xpf.Editors.Native;
#endif
namespace DevExpress.Xpf.Editors.Helpers {
	public class DataControllerData : IDataControllerData2 {
		public const string ToStringColumn = "LookUpEditBaseToStringColumn";
		public const string ValueColumn = "ValueColumn";
		public const string DisplayColumn = "DisplayColumn";
		public LookUpPropertyDescriptorBase ValueColumnDescriptor { get; private set; }
		public LookUpPropertyDescriptorBase DisplayColumnDescriptor { get; private set; }
		public string ValueColumnName { get { return ValueColumnDescriptor.Name; } }
		public string DisplayColumnName { get { return DisplayColumnDescriptor.Name; } }
		IItemsProviderOwner Owner { get; set; }
		Data.DataController DataController { get; set; }
		public DataControllerData(Data.DataController dataController, IItemsProviderOwner owner) {
			Owner = owner;
			DataController = dataController;
		}
		#region IDataControllerData Members
		UnboundColumnInfoCollection IDataControllerData.GetUnboundColumns() {
			return null;
		}
		object IDataControllerData.GetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
			return null;
		}
		void IDataControllerData.SetUnboundData(int listSourceRow, DataColumnInfo column, object value) {
		}
		#endregion
		#region IDataControllerData2 Members
		bool IDataControllerData2.CanUseFastProperties { get { return false; } }
		void IDataControllerData2.SubstituteFilter(SubstituteFilterEventArgs args) { }
		bool IDataControllerData2.HasUserFilter { get { return false; } }
		bool? IDataControllerData2.IsRowFit(int listSourceRow, bool fit) {
			return null;
		}
		PropertyDescriptorCollection IDataControllerData2.PatchPropertyDescriptorCollection(PropertyDescriptorCollection collection) {
			var descriptors = new List<PropertyDescriptor>(collection.Cast<PropertyDescriptor>());
			ValueColumnDescriptor = GetValueDescriptor();
			int valueDescriptorIndex = descriptors.FindIndex((propertyDescriptor) => propertyDescriptor.Name == ValueColumnDescriptor.Name);
			if (valueDescriptorIndex > -1)
				descriptors[valueDescriptorIndex] = ValueColumnDescriptor;
			else
				descriptors.Add(ValueColumnDescriptor);
			DisplayColumnDescriptor = GetDisplayDescriptor();
			int displayDescriptorIndex = descriptors.FindIndex((propertyDescriptor) => propertyDescriptor.Name == DisplayColumnDescriptor.Name);
			if (displayDescriptorIndex > -1)
				descriptors[displayDescriptorIndex] = DisplayColumnDescriptor;
			else
				descriptors.Add(DisplayColumnDescriptor);
			return new PropertyDescriptorCollection(descriptors.ToArray<PropertyDescriptor>());
		}
		LookUpPropertyDescriptorBase GetValueDescriptor() {
			string descriptorName = string.IsNullOrEmpty(Owner.ValueMember) ? ValueColumn : Owner.ValueMember;
			LookUpPropertyDescriptorBase lookupDescriptor = ValueColumnDescriptor;
			if (lookupDescriptor != null && lookupDescriptor.IsRelevant(descriptorName))
				return lookupDescriptor;
			return new LookUpPropertyDescriptor(LookUpPropertyDescriptorType.Value, descriptorName, Owner.ValueMember);
		}
		LookUpPropertyDescriptorBase GetDisplayDescriptor() {
			string descriptorName = string.IsNullOrEmpty(Owner.DisplayMember) ? DisplayColumn : Owner.DisplayMember;
			LookUpPropertyDescriptorBase lookupDescriptor = DisplayColumnDescriptor;
			if (lookupDescriptor != null && lookupDescriptor.IsRelevant(descriptorName))
				return lookupDescriptor;
			return new LookUpPropertyDescriptor(LookUpPropertyDescriptorType.Display, descriptorName, Owner.DisplayMember);
		}
		ComplexColumnInfoCollection IDataControllerData2.GetComplexColumns() {
			return null;
		}
		public void ResetDescriptors() {
			if (ValueColumnDescriptor != null)
				ValueColumnDescriptor.Reset();
			if (DisplayColumnDescriptor !=null)
				DisplayColumnDescriptor.Reset();
#if DEBUGTEST
			LogBase.Add(Owner, null);
#endif
		}
		#endregion
		#region inner classes
		class FilterEvaluatorContext : EvaluatorContextDescriptor {
			readonly object value = null;
			public override IEnumerable GetCollectionContexts(object source, string collectionName) { return null; }
			public override EvaluatorContext GetNestedContext(object source, string propertyPath) { return null; }
			public override IEnumerable GetQueryContexts(object source, string queryTypeName, CriteriaOperator condition, int top) { return null; }
			public override object GetPropertyValue(object source, EvaluatorProperty propertyPath) { return value; }
		}
		#endregion
	}
}
