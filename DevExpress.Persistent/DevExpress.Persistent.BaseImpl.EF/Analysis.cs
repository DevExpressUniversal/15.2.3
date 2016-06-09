#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       eXpressApp Framework                                        }
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
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DevExpress.Persistent.Base;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp;
namespace DevExpress.Persistent.BaseImpl.EF {
	[NavigationItem("Reports")]
	public class Analysis : IAnalysisInfo, IAnalysisInfoTestable, ISupportInitialize, IXafEntityObject {
		private const String propertiesSeparator = ";";
		private InitializeIndicator initializeIndicator;
		private DimensionPropertiesList dimensionProperties;
		private String criteria;
		private String objectTypeName;
		private Boolean IsInitializing {
			get { return initializeIndicator.IsInitializing; }
		}
		private void OnAnalysisInfoChanged(AnalysisInfoChangeType changeType) {
			if(InfoChanged != null) {
				InfoChanged(this, new AnalysisInfoChangedEventEventArgs(changeType));
			}
		}
		private void dimensionProperties_ListChanged(Object sender, EventArgs e) {
			DimensionPropertiesString = String.Join(propertiesSeparator, dimensionProperties);
			OnAnalysisInfoChanged(AnalysisInfoChangeType.DimensionPropertiesChanged);
		}
		private void SetDimensionProperties(String[] dimensionProperties) {
			this.dimensionProperties.ListChanged -= new EventHandler(dimensionProperties_ListChanged);
			this.dimensionProperties.Clear();
			foreach(String dimensionProperty in dimensionProperties) {
				this.dimensionProperties.Add(dimensionProperty);
			}
			this.dimensionProperties.ListChanged += new EventHandler(dimensionProperties_ListChanged);
		}
		public Analysis() {
			dimensionProperties = new DimensionPropertiesList();
			initializeIndicator = new InitializeIndicator();
		}
		[Browsable(false)]
		public Int32 ID { get; protected set; }
		public String Name { get; set; }
		[CriteriaOptions("DataType")]
		[EditorAlias(EditorAliases.PopupCriteriaPropertyEditor)]
		[FieldSize(FieldSizeAttribute.Unlimited)]
		[VisibleInListView(true)]
		[ModelDefault("RowCount", "0")]
		public String Criteria {
			get { return criteria; }
			set {
				if(criteria != value) {
					criteria = value;
					OnAnalysisInfoChanged(AnalysisInfoChangeType.CriteriaChanged);
				}
			}
		}
		[Browsable(false)]
		public String ObjectTypeName {
			get { return objectTypeName; }
			set {
				if(objectTypeName != value) {
					objectTypeName = value;
					SetDimensionProperties(DimensionPropertyExtractor.Instance.GetDimensionProperties(DataType));
					OnAnalysisInfoChanged(AnalysisInfoChangeType.ObjectTypeChanged);
				}
			}
		}
		[Browsable(false)]
		[FieldSize(FieldSizeAttribute.Unlimited)]
		public String DimensionPropertiesString { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Byte[] PivotGridSettingsContent { get; set; }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public Byte[] ChartSettingsContent { get; set; }
		[NotMapped, ImmediatePostData, TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
		public Type DataType {
			get {
				if(ObjectTypeName != null) {
					return ReflectionHelper.GetType(ObjectTypeName);
				}
				return null;
			}
			set {
				String stringValue = (value == null) ? null : value.FullName;
				String savedObjectTypeName = ObjectTypeName;
				try {
					if(stringValue != ObjectTypeName) {
						ObjectTypeName = stringValue;
					}
				}
				catch(Exception) {
					ObjectTypeName = savedObjectTypeName;
				}
				if(!IsInitializing) {
					Criteria = null;
				}
			}
		}
		[NotMapped, Browsable(false)]
		public IList<String> DimensionProperties {
			get { return dimensionProperties; }
		}
		[NotMapped, VisibleInListView(false)]
		public IAnalysisInfo Self {
			get { return this; }
		}
		[NotMapped, Browsable(false)]
		public Type ObjectType {
			get { return DataType; }
		}
		public event EventHandler<AnalysisInfoChangedEventEventArgs> InfoChanged;
		public void BeginInit() {
			initializeIndicator.BeginInit();
		}
		public void EndInit() {
			initializeIndicator.EndInit();
			UpdateDimensionProperties();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void UpdateDimensionProperties() {
			String[] dimensionProperties = null;
			if(!String.IsNullOrEmpty(DimensionPropertiesString)) {
				dimensionProperties = DimensionPropertiesString.Split(new String[] { propertiesSeparator }, StringSplitOptions.RemoveEmptyEntries);
			}
			else {
				dimensionProperties = new String[0];
			}
			SetDimensionProperties(dimensionProperties);
		}
		void IXafEntityObject.OnCreated()
		{
		}
		void IXafEntityObject.OnSaving()
		{
		}
		void IXafEntityObject.OnLoaded()
		{
			UpdateDimensionProperties();
		}
	}
}
