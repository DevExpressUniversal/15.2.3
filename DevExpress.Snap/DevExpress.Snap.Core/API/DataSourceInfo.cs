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
using System.Drawing.Design;
using DevExpress.Snap.Core.Native;
namespace DevExpress.Snap.Core.API {
	public class DataSourceInfo : IDataSourceOwner {
		CalculatedFieldCollection fCalculatedFields;
		object dataSource;
		string dataSourceName;
		public DataSourceInfo(string dataSourceName, object dataSource)
			: this() {
			this.DataSourceName = dataSourceName;
			this.DataSource = dataSource;
		}
		public DataSourceInfo() {
			InitCalculatedFields();
		}
		#region Properties
#if !SL
	[DevExpressSnapCoreLocalizedDescription("DataSourceInfoDataSourceName")]
#endif
		public string DataSourceName {
			get { return dataSourceName; }
			set {
				if (DataSourceName == value)
					return;
				dataSourceName = value;
				RaiseDataSourceNameChanged();
			}
		}
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("DataSourceInfoDataSource"),
#endif
		AttributeProvider(typeof(IListSource))]
		public object DataSource {
			get { return dataSource; }
			set {
				if (dataSource == value)
					return;
				dataSource = value;
				RaiseDataSourceChanged(DataSourceChangeType.DataSource);
			}
		}
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("DataSourceInfoCalculatedFields"),
#endif
		Editor("DevExpress.Snap.Extensions.Native.CalculatedFieldCollectionEditor, " + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
		public CalculatedFieldCollection CalculatedFields { get { return fCalculatedFields; } }
		internal bool IsEmpty { get { return string.IsNullOrEmpty(dataSourceName) && object.ReferenceEquals(dataSource, null); } }
		#endregion
		#region Events
		internal event DataSourceChangedEventHandler DataSourceChanged;
		#region DataSourceNameChanged
		EventHandler onDataSourceNameChanged;
		internal event EventHandler DataSourceNameChanged { add { onDataSourceNameChanged += value; } remove { onDataSourceNameChanged -= value; } }
		void RaiseDataSourceNameChanged() {
			EventHandler handler = onDataSourceNameChanged;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}
		#endregion
		#endregion
		protected internal void AddCalculatedFieldsWithReplace(ICollection<CalculatedField> calculatedFields) {
			if (object.ReferenceEquals(calculatedFields, fCalculatedFields))
				return;
			foreach (CalculatedField calculatedField in calculatedFields)
				AddCalculatedFieldWithReplace(calculatedField);
		}
		public void AddCalculatedFieldWithReplace(CalculatedField calculatedField) {
			int count = fCalculatedFields.Count;
			for (int i = 0; i < count; i++) {
				if (fCalculatedFields[i].Name == calculatedField.Name && fCalculatedFields[i].DataMember == calculatedField.DataMember) {
					fCalculatedFields.RemoveAt(i);
					break;
				}
			}
			fCalculatedFields.Add(calculatedField);
		}
		void InitCalculatedFields() {
			fCalculatedFields = new CalculatedFieldCollection();
			fCalculatedFields.ListChanged += new System.ComponentModel.ListChangedEventHandler(fCalculatedFields_ListChanged);
			fCalculatedFields.CalculatedFieldAdded += OnCalculatedFieldAdded;
		}
		void OnCalculatedFieldAdded(object sender, CalculatedFieldAddedEventArgs e) {
			RaiseCalculatedFieldAdded(e.Field);
		}
		void fCalculatedFields_ListChanged(object sender, System.ComponentModel.ListChangedEventArgs e) {
			RaiseDataSourceChanged(DataSourceChangeType.CalculatedFields);
		}
		void RaiseDataSourceChanged(DataSourceChangeType dataSourceChangeType) {
			if (DataSourceChanged != null)
				DataSourceChanged(this, new DataSourceChangedEventArgs(this, dataSourceChangeType));
		}
		protected internal event CalculatedFieldAddedEventHandler CalculatedFieldAdded;
		protected virtual void RaiseCalculatedFieldAdded(CalculatedField field) {
			if (CalculatedFieldAdded != null)
				CalculatedFieldAdded(this, new CalculatedFieldAddedEventArgs(field));
		}
	}
}
