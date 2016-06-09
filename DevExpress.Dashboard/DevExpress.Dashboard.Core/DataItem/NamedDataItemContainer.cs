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

using DevExpress.DashboardCommon.Native;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public abstract class NamedDataItemContainer : DataItemContainer, IEditNameProvider {
		readonly NameBox nameBox = new NameBox("Name");
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("NamedDataItemContainerName"),
#endif
		Category(CategoryNames.General),
		DefaultValue(null),
		Localizable(true)
		]
		public string Name { get { return nameBox.Name; } set { nameBox.Name = value; } }
		internal string DisplayName { get { return nameBox.DisplayName; } }
		string IEditNameProvider.EditName { get { return nameBox.EditName; } set { nameBox.EditName = value; } }
		string IEditNameProvider.DisplayName { get { return DisplayName; } }
		protected internal abstract string DefaultName { get; }
		protected NamedDataItemContainer(IEnumerable<DataItemDescription> dataItemDescriptions)
			: base(dataItemDescriptions) {
			nameBox.NameChanged += (sender, e) => OnChanged(ChangeReason.View);
			nameBox.RequestDefaultName += (sender, e) => e.DefaultName = DefaultName;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			nameBox.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			nameBox.LoadFromXml(element);
		}
		protected internal override void Assign(DataItemContainer container) {
			base.Assign(container);
			NamedDataItemContainer namedContainer = container as NamedDataItemContainer;
			if(namedContainer != null)
				Name = namedContainer.Name;
		}
	}
}
