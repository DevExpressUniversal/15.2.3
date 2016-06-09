#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DataAccess;
using DevExpress.Utils;
using DevExpress.Compatibility.System.Collections.Specialized;
namespace DevExpress.DashboardCommon.Native {
	public class PieInternal : IElementContainer, IChangeService {
		const string xmlValues = "Values";
		const string xmlValue = "Value";
		readonly IPieContext context;
		readonly MeasureCollection values = new MeasureCollection();
		readonly DataItemNameRepository namesRepository;
		readonly PieElementContainer elementContainer;
		ContentArrangementMode contentArrangementMode;
		int contentLineCount;
		public event EventHandler<ChangedEventArgs> Changed;
		public DataItemNameRepository NamesRepository { get { return namesRepository; } }
		public IEnumerable<string> ValueDataMembers {
			get {
				DataItemRepository repository = context.DataItemRepositoryProvider.DataItemRepository;
				foreach(Measure measure in values) {
					if(repository.Contains(measure))
						yield return repository.GetActualID(measure);
				}
			}
		}
		public IEnumerable<string> ValueDisplayNames { 
			get {
				foreach(Measure measure in values)
					yield return measure.DisplayName;
			}
		}
		public MeasureCollection Values { get { return values; } }
		public Measure ActiveElement { get { return elementContainer.ActiveElement; } }
		public bool ElementSelectionEnabled { get { return elementContainer.ElementSelectionEnabled; } }
		public IList<string> ElementNames { get { return elementContainer.ElementNames; } }
		public int ActiveElementIndex {
			get { return elementContainer.ActiveElementIndex; }
			set { elementContainer.ActiveElementIndex = value; }
		}
		public bool AllowLayers { get { return context.AllowLayers; } }
		public int SelectedElementIndex {
			get { return elementContainer.ActiveElementIndex; }
			set { elementContainer.ActiveElementIndex = value; }
		}
		public ContentArrangementMode ContentArrangementMode {
			get { return contentArrangementMode; }
			set {
				if(value != contentArrangementMode) {
					contentArrangementMode = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		public int ContentLineCount {
			get { return contentLineCount; }
			set {
				if(value != contentLineCount) {
					contentLineCount = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		IList<string> IElementContainer.ElementNames { get { return elementContainer.ElementNames; } }
		bool IElementContainer.ElementSelectionEnabled { get { return elementContainer.ElementSelectionEnabled; } }
		int IElementContainer.SelectedElementIndex {
			get { return SelectedElementIndex; }
			set { SelectedElementIndex = value; }
		}
		bool ProvideValuesAsArguments {
			get {
				IList<Dimension> arguments = context.Arguments;
				return arguments == null || arguments.Count == 0;
			}
		}
		public PieInternal(IPieContext context, params Measure[] initialValues) {
			Guard.ArgumentNotNull(context, "context");
			this.context = context;
			namesRepository = new DataItemNameRepository(context.DataItemRepositoryProvider);
			elementContainer = new PieElementContainer(this, new ReadOnlyCollection<Measure>(values));
			values.AddRange(initialValues);
			values.CollectionChanged += OnValuesCollectionChanged;
			ContentArrangementMode = context.DefaultContentArrangementMode;
			ContentLineCount = context.DefaultContentLineCount;
		}
		void OnValuesCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Measure> e) {
			if (ProvideValuesAsArguments)
				elementContainer.ResetActiveElement();
			else
				elementContainer.ValidateActiveElement();
			context.OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		public void ResetActiveElement() {
			elementContainer.ResetActiveElement();
		}
		public void SaveToXml(XElement element) {
			values.SaveToXml(element, xmlValues, xmlValue);
		}
		public void LoadFromXml(XElement element) {
			values.LoadFromXml(element, xmlValues, xmlValue);
			namesRepository.LoadFromXml(element);
		}
		public void OnEndLoading(IDataItemContext dataItemContext) {
			values.OnEndLoading(context.DataItemRepositoryProvider.DataItemRepository, dataItemContext);
			namesRepository.OnEndLoading();
		}
		public bool IsValueEmpty(Measure value) {
			return ProvideValuesAsArguments && values.Count > 0 && value != values[0];
		}
		public SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if(values.Contains(measure))
				return SummaryTypeInfo.Number;
			return null;
		}
		public EditNameDescription GetEditNameDescription() {
			return new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionValues), values);
		}
		public void OnChanged(ChangedEventArgs e) {
			if(Changed != null)
				Changed(this, e);
		}
		void OnChanged(ChangeReason reason) {
			ChangedEventArgs e = new ChangedEventArgs(reason, this, new object[0]);
			OnChanged(e);
		}
	}
}
