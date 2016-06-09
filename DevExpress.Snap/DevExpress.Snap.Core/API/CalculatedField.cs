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
using System.Drawing.Design;
using System.ComponentModel;
using DevExpress.Utils;
using DevExpress.Office;
using DevExpress.Office.Utils;
using DevExpress.Snap.Core.Native;
using DevExpress.XtraReports.Native;
using System.Diagnostics;
using Debug = System.Diagnostics.Debug;
namespace DevExpress.Snap.Core.API {
	public class CalculatedField : ICalculatedField, IDisposable, ICloneable<CalculatedField> {
		readonly string dataMember;
		XtraReports.UI.FieldType fieldType = XtraReports.UI.FieldType.String;
		CalculatedField(string dataMember) {
			this.dataMember = dataMember;
		}
		public CalculatedField(string name, string dataMember) {
			Guard.ArgumentIsNotNullOrEmpty(name, "name");
			this.Name = name;
			this.dataMember = dataMember;
		}
#if !SL
	[DevExpressSnapCoreLocalizedDescription("CalculatedFieldDisposed")]
#endif
		public event EventHandler Disposed;
		internal IDataSourceDispatcher DataSourceDispatcher { get; set; }
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("CalculatedFieldDataSourceName"),
#endif
		Category("Data"),
		ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.CalculatedField_DataSourceName", "Data Source Name")]
		public string DataSourceName { get; set; }
		void OnDisposed() {
			if (Disposed != null)
				Disposed(this, EventArgs.Empty);
		}
		#region ICalculatedField Members
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("CalculatedFieldDataMember"),
#endif
		ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.CalculatedField_DataMember", "Data Member"),
		Category("Data")]
		public string DataMember {
			get { return dataMember; }
		}
		[Browsable(false)]
		public object DataSource {
			get {
				Debug.Assert(DataSourceDispatcher != null);
				return DataSourceDispatcher != null ? DataSourceDispatcher.GetDataSource(DataSourceName) : null;
			}
		}
		[Browsable(false)]
		public string DisplayName {
			get { return Name; }
		}
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("CalculatedFieldExpression"),
#endif
		Editor("DevExpress.Snap.Extensions.Native.ExpressionEditor," + AssemblyInfo.SRAssemblySnapExtensions, typeof(UITypeEditor))]
		[ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.CalculatedField_Expression", "Expression")]
		[Category("Data")]
		public string Expression { get; set; }
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("CalculatedFieldFieldType"),
#endif
		ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.CalculatedField_FieldType", "Field Type"),
		Category("Data")]
		public XtraReports.UI.FieldType FieldType {
			get { return fieldType; }
			set { fieldType = value; }
		}
		string name;
		[
#if !SL
	DevExpressSnapCoreLocalizedDescription("CalculatedFieldName"),
#endif
		ResDisplayName(typeof(CoreResFinder), DevExpress.Snap.Localization.SnapResLocalizer.DefaultResourceFile, "SnapStringId.CalculatedField_Name", "Name"),
		Category("Design")]
		public string Name {
			get { return name; }
			set {
				if (string.Compare(name, value) == 0)
					return;
				if (!ValidateName(value))
					throw new ArgumentException();
				name = value;
			}
		}
		bool ValidateName(string name) {
			if (string.IsNullOrEmpty(name))
				return false;
			if (DataSourceDispatcher != null)
				foreach (var calculatedField in DataSourceDispatcher.GetCalculatedFields())
					if (string.Compare(name, calculatedField.Name) == 0)
						return false;
			return true;
		}
		#endregion
		#region IDisposable Members
		public void Dispose() {
			Dispose(true);
			GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				OnDisposed();
			}
		}
		~CalculatedField() {
			Dispose(false);
		}
		#endregion
		public CalculatedField Clone() {
			CalculatedField clone = new CalculatedField(this.dataMember);
			clone.name = this.name;
			clone.fieldType = this.fieldType;
			clone.Expression = this.Expression;
			clone.DataSourceName = this.DataSourceName;
			clone.DataSourceDispatcher = this.DataSourceDispatcher;
			return clone;
		}
	}
}
