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
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Utils;
namespace DevExpress.Xpf.DataAccess.DataSourceWizard {
	public class BooleanViewModel : ImmutableObject {
		public static Lazy<IEnumerable<BooleanViewModel<T>>> CreateList<T>(string firstText, string secondText, bool firstValue, Func<bool, T> propertiesFactory) {
			return CreateList(firstText, null, secondText, null, firstValue, propertiesFactory);
		}
		public static Lazy<IEnumerable<BooleanViewModel>> CreateList(string firstText, string secondText, bool firstValue) {
			return CreateList(firstText, null, secondText, null, firstValue);
		}
		public static Lazy<IEnumerable<BooleanViewModel>> CreateList(string firstText, string firstDescription, string secondText, string secondDescription, bool firstValue) {
			return new Lazy<IEnumerable<BooleanViewModel>>(() => new List<BooleanViewModel>() {
				new BooleanViewModel(firstText, firstDescription, firstValue),
				new BooleanViewModel(secondText, secondDescription, !firstValue)
			}.AsReadOnly());
		}
		public static Lazy<IEnumerable<BooleanViewModel<T>>> CreateList<T>(string firstText, string firstDescription, string secondText, string secondDescription, bool firstValue, Func<bool, T> propertiesFactory) {
			return new Lazy<IEnumerable<BooleanViewModel<T>>>(() => new List<BooleanViewModel<T>>() {
				new BooleanViewModel<T>(firstText, firstDescription, firstValue, propertiesFactory(firstValue)),
				new BooleanViewModel<T>(secondText, secondDescription, !firstValue, propertiesFactory(!firstValue))
			}.AsReadOnly());
		}
		public BooleanViewModel(string text, string description, bool value) {
			this.text = text;
			this.description = description;
			this.value = value;
		}
		public override string ToString() { return Text; }
		readonly string text;
		public string Text { get { return text; } }
		readonly string description;
		public string Description { get { return description; } }
		readonly bool value;
		public bool Value { get { return value; } }
	}
	public class BooleanViewModel<T> : BooleanViewModel {
		public BooleanViewModel(string text, string description, bool value, T properties)
			: base(text, description, value) {
			this.properties = properties;
		}
		readonly T properties;
		public T Properties { get { return properties; } }
	}
	[POCOViewModel]
	public class SinglePropertyViewModel<T> {
		public static SinglePropertyViewModel<T> Create(T value) {
			return ViewModelSource.Create(() => new SinglePropertyViewModel<T>(value));
		}
		protected SinglePropertyViewModel(T value) {
			Value = value;
		}
		public virtual T Value { get; set; }
	}
}
