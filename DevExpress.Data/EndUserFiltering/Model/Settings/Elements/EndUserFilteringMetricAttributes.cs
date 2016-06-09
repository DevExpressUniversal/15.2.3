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

namespace DevExpress.Utils.Filtering.Internal {
	using System;
	using System.Collections;
	using System.Collections.Generic;
	public interface IEndUserFilteringMetricAttributesFactory {
		IEndUserFilteringMetricAttributes Create(string path, Type type, Attribute[] attributes = null);
	}
	sealed class DefaultEndUserFilteringMetricAttributesFactory : IEndUserFilteringMetricAttributesFactory {
		internal static IEndUserFilteringMetricAttributesFactory Instance = new DefaultEndUserFilteringMetricAttributesFactory();
		DefaultEndUserFilteringMetricAttributesFactory() { }
		IEndUserFilteringMetricAttributes IEndUserFilteringMetricAttributesFactory.Create(string path, Type type, Attribute[] attributes) {
			return new EndUserFilteringMetricAttributes(path, type, attributes);
		}
	}
	sealed class EndUserFilteringMetricAttributes : IEndUserFilteringMetricAttributes {
		readonly Attribute[] attributes;
		public EndUserFilteringMetricAttributes(string path, Type type, Attribute[] attributes = null) {
			this.Path = path;
			this.Type = type;
			this.attributes = attributes ?? new Attribute[] { };
		}
		public string Path {
			get;
			private set;
		}
		public Type Type {
			get;
			private set;
		}
		public AttributesMergeMode MergeMode {
			get;
			set;
		}
		IEnumerator IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
		public IEnumerator<Attribute> GetEnumerator() {
			foreach(Attribute a in attributes)
				yield return a;
		}
	}
}
