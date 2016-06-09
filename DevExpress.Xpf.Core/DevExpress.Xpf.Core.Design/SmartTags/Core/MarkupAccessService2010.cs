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

extern alias Platform;
using Platform::DevExpress.Mvvm.UI.Native.ViewGenerator.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.Design.SmartTags {
	public interface IMarkupAccessService2010 : IMarkupAccessService {
		IVirtualModelItem GetModelItem(IModelItem modelItem);
	}
	public interface IVirtualDocumentNode {
	}
	public interface IVirtualDocumentItem : IVirtualDocumentNode {
		IVS2010TypeMetadata ItemType { get; }
		IEnumerable<IVirtualDocumentProperty> Properties { get; }
	}
	public interface IModifiableDocumentItem : IVirtualDocumentItem {
	}
	public interface IXamlMarkupExtensionItem : IModifiableDocumentItem, IMarkupLocationProvider {
	}
	public interface IContextualModifiableDocumentProperty : IModifiableDocumentProperty {
	}
	public interface IXamlMarkupExtensionPropertyBase : IContextualModifiableDocumentProperty, IMarkupLocationProvider {
		string Source { get; }
	}
	public interface IXamlMarkupExtensionProperty : IXamlMarkupExtensionPropertyBase {
	}
	public interface IXamlMovableItem : IModifiableDocumentItem {
	}
	public interface IChangedModelDocumentItem : IModifiableDocumentItem {
	}
	public interface IXamlSourceDocument : IDisposable {
		IMarkupSourceProvider Provider { get; }
		IVS2010TypeMetadata ResolveType(IXamlElement element, string name);
	}
	public interface IXamlItem : IXamlMovableItem, IMarkupLocationProvider {
		IXamlSourceDocument Document { get; }
		IXamlObjectElement Element { get; }
	}
	public interface IVirtualModelHost {
		IVirtualModelItem Root { get; }
		IVirtualDocumentNode FindNode(object identity);
		IVS2010MetadataContext Metadata { get; }
	}
	public interface IVirtualModelItem {
		IVirtualModelHost Host { get; }
		object Identity { get; }
	}
	public interface IMarkupLocation {
		int Offset { get; }
		int Length { get; }
	}
	public class MarkupLocation : IMarkupLocation {
		public MarkupLocation(int offset, int length) {
			Offset = offset;
			Length = length;
		}
		public MarkupLocation(IMarkupLocation markupLocation) {
			Offset = markupLocation.Offset;
			Length = markupLocation.Length;
		}
		public int Offset { get; set; }
		public int Length { get; set; }
	}
	public interface IMarkupLocationProvider {
		MarkupLocation GetLocation();
	}
	public interface ISourceReader {
		string Read(long location, int length);
	}
	public interface IMarkupSourceProvider {
		ISourceReader CreateReader();
	}
	public interface IVirtualDocumentProperty : IVirtualDocumentNode {
		string Name { get; }
		IVS2010TypeMetadata PropertyType { get; }
		IVirtualDocumentValue PropertyValue { get; }
		IEnumerable<IVirtualDocumentItem> Items { get; }
	}
	public interface IModifiableDocumentProperty : IVirtualDocumentProperty {
	}
	public interface IChangedModelDocumentProperty : IModifiableDocumentProperty {
	}
	public interface IVirtualDocumentValue {
		string Source { get; }
	}
	public interface INode { }
	public interface IXamlNode : INode { }
	public interface IXamlElement : IXamlNode { }
	public interface IXamlObjectElement : IXamlElement { }
	public interface ISourceTextValue {
		string Source { get; }
	}
	public interface IXamlProperty : IContextualModifiableDocumentProperty, IMarkupLocationProvider, ISourceTextValue { }
}
