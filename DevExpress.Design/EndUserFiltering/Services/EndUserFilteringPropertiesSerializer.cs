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

namespace DevExpress.Design.Filtering.Services {
	using System;
	using System.Collections.Generic;
	using System.ComponentModel.Design;
	using DevExpress.Design.Filtering.UI;
	using DevExpress.Utils.Filtering;
	using DevExpress.Utils.Filtering.Internal;
	public interface IFilteringModelPropertiesSerializer {
		void Serialize(IServiceProvider serviceContainer, IFilteringModelConfiguratorContext configuratorContext);
	}
	sealed class FilteringModelPropertiesSerializer : IFilteringModelPropertiesSerializer {
		public void Serialize(IServiceProvider serviceProvider, IFilteringModelConfiguratorContext configuratorContext) {
			WinSerializeProperties(serviceProvider, configuratorContext);
		}
		void WinSerializeProperties(System.IServiceProvider serviceProvider, IFilteringModelConfiguratorContext configuratorContext) {
			IDesignerHost designerHost = serviceProvider.GetService(typeof(IDesignerHost)) as IDesignerHost;
			if(designerHost == null) return;
			FilteringUIContext component = configuratorContext.Component as FilteringUIContext;
			DesignerTransaction designerTransaction = null;
			bool transactionFlag = true;
			try {
				try {
					designerTransaction = designerHost.CreateTransaction();
				}
				catch(System.ComponentModel.Design.CheckoutException ex) {
					if(ex == System.ComponentModel.Design.CheckoutException.Canceled)
						return;
					throw ex;
				}
				try {
					bool isModelTypeChanged = false;
					if(component.ModelType != configuratorContext.ModelType && configuratorContext.ModelType != null) {
						component.ModelType = configuratorContext.ModelType;
						isModelTypeChanged = true;
					}
					if(component.CustomMetricAttributes == null) return;
					if(isModelTypeChanged || configuratorContext.CustomAttributes != null)
						component.CustomMetricAttributes.Clear();
					if(configuratorContext.CustomAttributes == null) return;
					foreach(var attribute in configuratorContext.CustomAttributes) {
						component.CustomMetricAttributes.Add(new CustomMetricsAttributeExpression(attribute.Path, attribute.Type, GetAttributeInfos(attribute)));
					}
				}
				catch {
					transactionFlag = false;
				}
			}
			finally {
				if(transactionFlag)
					designerTransaction.Commit();
				else
					designerTransaction.Cancel();
			}
		}
		AttributeInfo[] GetAttributeInfos(IEndUserFilteringMetricAttributes attributes) {
			List<AttributeInfo> infos = new List<AttributeInfo>();
			foreach(var attribute in attributes)
				infos.Add(AttributeInfoFactory.Instance.Create(attribute));
			return infos.ToArray();
		}
	}
}
