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

#if !SL
using System.Collections.Generic;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.XtraReports.Native;
using DevExpress.XtraReports.Native.Data;
using DevExpress.XtraReports.Native.Parameters;
using DevExpress.XtraReports.Parameters.Native;
using DevExpress.XtraPrinting.Native;
#endif
namespace DevExpress.XtraReports.Parameters {
	public static class LookUpHelper {
#if SL
		public static LookUpValueCollection GetLookUpValues(LookUpSettings lookUpSettings) {
			StaticListLookUpSettings staticListLookUpSettings = lookUpSettings as StaticListLookUpSettings;
			if(staticListLookUpSettings != null)
				return staticListLookUpSettings.LookUpValues;
			return null;
		}
#else
		public static LookUpValueCollection GetLookUpValues(LookUpSettings lookUpSettings, DataContext dataContext) {
			return GetLookUpValues(lookUpSettings, dataContext, null);
		}
		public static LookUpValueCollection GetLookUpValues(LookUpSettings lookUpSettings, DataContext dataContext, IParameterEditorValueProvider parameterValueProvider) {
			StaticListLookUpSettings staticListLookUpSettings = lookUpSettings as StaticListLookUpSettings;
			if(staticListLookUpSettings != null)
				return GetStaticLookUpValues(staticListLookUpSettings, parameterValueProvider);
			DynamicListLookUpSettings dynamicListLookUpSettings = lookUpSettings as DynamicListLookUpSettings;
			if(dynamicListLookUpSettings != null) {
				return GetDynamicLookUpValues(dynamicListLookUpSettings, dataContext, parameterValueProvider);
			}
			return null;
		}
		static LookUpValueCollection GetStaticLookUpValues(StaticListLookUpSettings staticListLookUpSettings, IParameterEditorValueProvider parameterValueProvider) {
			var result = new LookUpValueCollection();
			var dataController = new ListSourceDataController() { ListSource = staticListLookUpSettings.LookUpValues };
			dataController.FilterCriteria = GetFilterCriteria(staticListLookUpSettings, parameterValueProvider);
			dataController.DoRefresh();
			var filteredRows = dataController.GetAllFilteredAndSortedRows();
			filteredRows.Cast<LookUpValue>().ForEach(x => result.Add(x.Clone()));
			return result;
		}
		static LookUpValueCollection GetDynamicLookUpValues(DynamicListLookUpSettings dynamicListLookUpSettings, DataContext dataContext, IParameterEditorValueProvider parameterValueProvider) {
			if(string.IsNullOrEmpty(dynamicListLookUpSettings.DisplayMember) || string.IsNullOrEmpty(dynamicListLookUpSettings.ValueMember))
				return null;
			dataContext.Clear();
			string displayMember = GetMemberPath(dynamicListLookUpSettings.DataMember, dynamicListLookUpSettings.DisplayMember);
			DataBrowser displayMemberBrowser = dataContext[dynamicListLookUpSettings.DataSource, displayMember];
			string valueMember = GetMemberPath(dynamicListLookUpSettings.DataMember, dynamicListLookUpSettings.ValueMember);
			DataBrowser valueMemberBrowser = dataContext[dynamicListLookUpSettings.DataSource, valueMember];
			string dataMember = dynamicListLookUpSettings.DataMember == null ? "" : dynamicListLookUpSettings.DataMember;
			DataBrowser parentBrowser = dataContext.GetDataBrowser(dynamicListLookUpSettings.DataSource, dataMember, true);
			if(parentBrowser == null)
				return null;
			System.Diagnostics.Debug.Assert(displayMemberBrowser.Parent == valueMemberBrowser.Parent);
			var listController = GetFilteredController(parentBrowser);
			if(listController != null) {
				listController.FilterCriteria = GetFilterCriteria(dynamicListLookUpSettings, parameterValueProvider);
			}
			List<LookUpValue> lookUpValues = new List<LookUpValue>();
			for(int i = 0; i < parentBrowser.Count; i++) {
				parentBrowser.Position = i;
				if(valueMemberBrowser.Current != null && valueMemberBrowser.Current != System.DBNull.Value) {
					var description = displayMemberBrowser.Current;
					lookUpValues.Add(new LookUpValue() {
						Value = valueMemberBrowser.Current,
						Description = (description != null) ? description.ToString() : null
					});
				}
			}
			LookUpValueCollection collection = new LookUpValueCollection();
			collection.AddRange(lookUpValues.Distinct(new LookUpValueValueComparer()));
			return collection;
		}
		static string GetMemberPath(string dataMember, string destinationMember) {
			const char separator = '.';
			return string.IsNullOrEmpty(dataMember)
				? destinationMember
				: (dataMember + separator + destinationMember);
		}
		#region FilterCriteria
		static IFilteredListController GetFilteredController(DataBrowser dataBrowser) {
			var listBrowser = dataBrowser as ListBrowser;
			if(listBrowser == null)
				return null;
			return listBrowser.ListController as IFilteredListController;
		}
		static CriteriaOperator GetFilterCriteria(LookUpSettings lookUpSettings, IParameterEditorValueProvider parameterValueProvider) {
			CriteriaOperator result = CriteriaOperator.Parse(lookUpSettings.FilterString);
			if(!ReferenceEquals(result, null)) {
				CriteriaOperator processedResult = ProcessCriteriaOperator(lookUpSettings, result);
				CascadingParametersValueSetter.Process(processedResult, lookUpSettings.Parameter.Owner, parameterValueProvider);
				return processedResult;
			}
			return null;
		}
		static CriteriaOperator ProcessCriteriaOperator(IDataContainerBase dataContainer, CriteriaOperator criteriaOperator) {
				using(DataContext dataContext = new XRDataContextBase(Enumerable.Empty<ICalculatedField>(), true)) {
					DeserializationFilterStringVisitor visitor = new DeserializationFilterStringVisitor(null, dataContext, dataContainer.DataSource, dataContainer.DataMember);
					return (CriteriaOperator)criteriaOperator.Accept(visitor);
				}
		}
		#endregion
#endif
	}
}
