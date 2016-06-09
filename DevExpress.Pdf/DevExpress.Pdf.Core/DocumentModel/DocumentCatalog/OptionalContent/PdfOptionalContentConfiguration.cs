#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

using System.Collections;
using System.Collections.Generic;
using DevExpress.Pdf.Native;
namespace DevExpress.Pdf {
	[PdfDefaultField(PdfOptionalContentState.On)]
	public enum PdfOptionalContentState { 
		[PdfFieldName("ON")]
		On, 
		[PdfFieldName("OFF")]
		Off, 
		Unchanged 
	}
	[PdfDefaultField(PdfOptionalContentOrderListMode.AllPages)]
	public enum PdfOptionalContentOrderListMode { AllPages, VisiblePages }
	public class PdfOptionalContentRadioButtonGroup : List<PdfOptionalContentGroup> {}
	public class PdfOptionalContentConfiguration {
		const string nameDictionaryKey = "Name";
		const string creatorDictionaryKey = "Creator";
		const string baseStateDictionaryKey = "BaseState";
		const string onGroupsDictionaryKey = "ON";
		const string offGroupsDictionaryKey = "OFF";
		const string intentDictionaryKey = "Intent";
		const string usageApplicationDictionaryKey = "AS";
		const string orderDictionaryKey = "Order";
		const string orderListModeDictionaryKey = "ListMode";
		const string radioButtonGroupsDictionaryKey = "RBGroups";
		const string lockedDictionaryKey = "Locked";
		readonly string name;
		readonly string creator;
		readonly PdfOptionalContentState baseState;
		readonly IList<PdfOptionalContentGroup> onGroups;
		readonly IList<PdfOptionalContentGroup> offGroups;
		readonly PdfOptionalContentIntent intent;
		readonly IList<PdfOptionalContentUsageApplication> usageApplication;
		readonly PdfOptionalContentOrder order;
		readonly PdfOptionalContentOrderListMode orderListMode;
		readonly IList<PdfOptionalContentRadioButtonGroup> radioButtonGroups;
		readonly IList<PdfOptionalContentGroup> locked;
		public string Name { get { return name; } }
		public string Creator { get { return creator; } }
		public PdfOptionalContentState BaseState { get { return baseState; } }
		public IList<PdfOptionalContentGroup> On { get { return onGroups; } }
		public IList<PdfOptionalContentGroup> Off { get { return offGroups; } }
		public PdfOptionalContentIntent Intent { get { return intent; } }
		public IList<PdfOptionalContentUsageApplication> UsageApplication { get { return usageApplication; } }
		public PdfOptionalContentOrder Order { get { return order; } }
		public PdfOptionalContentOrderListMode OrderListMode { get { return orderListMode; } }
		public IList<PdfOptionalContentRadioButtonGroup> RadioButtonGroups { get { return radioButtonGroups; } }
		public IList<PdfOptionalContentGroup> Locked { get { return locked; } }
		internal PdfOptionalContentConfiguration(PdfReaderDictionary dictionary) {
			if (dictionary == null) {
				intent = PdfOptionalContentIntent.View;
				onGroups = new List<PdfOptionalContentGroup>();
				offGroups = new List<PdfOptionalContentGroup>();
				radioButtonGroups = new List<PdfOptionalContentRadioButtonGroup>();
			}
			else {
				PdfObjectCollection objects = dictionary.Objects;
				name = dictionary.GetString(nameDictionaryKey);
				creator = dictionary.GetString(creatorDictionaryKey);
				baseState = dictionary.GetEnumName<PdfOptionalContentState>(baseStateDictionaryKey);
				onGroups = dictionary.GetArray<PdfOptionalContentGroup>(onGroupsDictionaryKey, oc => objects.GetOptionalContentGroup(oc)) ?? new List<PdfOptionalContentGroup>();
				offGroups = dictionary.GetArray<PdfOptionalContentGroup>(offGroupsDictionaryKey, oc => objects.GetOptionalContentGroup(oc)) ?? new List<PdfOptionalContentGroup>();
				intent = dictionary.GetOptionalContentIntent(intentDictionaryKey);
				usageApplication = dictionary.GetArray<PdfOptionalContentUsageApplication>(usageApplicationDictionaryKey, v => {
					PdfReaderDictionary usageApplicationDictionary = objects.TryResolve(v) as PdfReaderDictionary;
					if (usageApplicationDictionary == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					return new PdfOptionalContentUsageApplication(usageApplicationDictionary);
				});
				IList<object> orderList = dictionary.GetArray(orderDictionaryKey);
				if (orderList != null)
					order = new PdfOptionalContentOrder(objects, orderList);
				orderListMode = dictionary.GetEnumName<PdfOptionalContentOrderListMode>(orderListModeDictionaryKey);
				radioButtonGroups = dictionary.GetArray<PdfOptionalContentRadioButtonGroup>(radioButtonGroupsDictionaryKey, v => {
					IList<object> list = objects.TryResolve(v) as IList<object>;
					if (list == null)
						PdfDocumentReader.ThrowIncorrectDataException();
					PdfOptionalContentRadioButtonGroup group = new PdfOptionalContentRadioButtonGroup();
					foreach (object optionalContentDictionary in list)
						group.Add(objects.GetOptionalContentGroup(optionalContentDictionary));
					return group;
				});
				if (radioButtonGroups == null)
					radioButtonGroups = new List<PdfOptionalContentRadioButtonGroup>();
				locked = dictionary.GetArray<PdfOptionalContentGroup>(lockedDictionaryKey, oc => objects.GetOptionalContentGroup(oc));
			}
		}
		internal object Write(PdfObjectCollection objects) {
			PdfWriterDictionary dictionary = new PdfWriterDictionary(objects);
			dictionary.Add(nameDictionaryKey, name, null);
			dictionary.Add(creatorDictionaryKey, creator, null);
			dictionary.AddEnumName(baseStateDictionaryKey, baseState);
			dictionary.AddList(onGroupsDictionaryKey, onGroups);
			dictionary.AddList(offGroupsDictionaryKey, offGroups);
			dictionary.AddIntent(intentDictionaryKey, intent);
			dictionary.AddList(usageApplicationDictionaryKey, usageApplication, o => ((PdfOptionalContentUsageApplication)o).Write(objects));
			if (order != null)
				dictionary.Add(orderDictionaryKey, order.Write(objects));
			dictionary.AddEnumName(orderListModeDictionaryKey, orderListMode);
			dictionary.AddList(radioButtonGroupsDictionaryKey, radioButtonGroups, o => new PdfWritableObjectArray(o, objects));
			dictionary.AddList(lockedDictionaryKey, locked);
			return dictionary;
		}
	}
}
