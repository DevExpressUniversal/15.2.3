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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Xml;
using DevExpress.Office;
namespace DevExpress.XtraRichEdit.Import.OpenXml {
	#region CommentsExDestination
	public class CommentsExDestination  : ElementDestination {
		static readonly ElementHandlerTable handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable CreateElementHandlerTable() {
			ElementHandlerTable result = new ElementHandlerTable();
			result.Add("commentEx", OnCommentEx);
			return result;
		}
		protected static Destination OnCommentEx(WordProcessingMLBaseImporter importer, XmlReader reader) {
			return new CommentExDestination(importer);
		}
		public CommentsExDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		protected internal override ElementHandlerTable ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region CommentExDestination 
	public class CommentExDestination : LeafElementDestination {
		public CommentExDestination(WordProcessingMLBaseImporter importer)
			: base(importer) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			CommentExListInfo listInfo = new CommentExListInfo();
			string value = reader.GetAttribute("paraId", Importer.W15NamespaceConst);
			listInfo.ParaId = Importer.ConvertToInt(value);
			value = reader.GetAttribute("paraIdParent", Importer.W15NamespaceConst);
			listInfo.ParaIdParent = Importer.ConvertToInt(value);
			value = reader.GetAttribute("done", Importer.W15NamespaceConst);
			listInfo.Done = Importer.ConvertToInt(value);
			if ((listInfo.ParaId > 0) && (listInfo.ParaIdParent > 0))
				Importer.CommentExListInfos.Add(listInfo);
		}
		Int32 CalculateIntegerValue(string value) {
			return Importer.GetIntegerValue(value, NumberStyles.HexNumber, Int32.MinValue);
		}
	}
	#endregion
	#region OpenXmlCommentExListInfo
	public class CommentExListInfo { 
		Int32 paraId;
		Int32 paraIdParent;
		Int32 done;
		public Int32 ParaId { get { return paraId; } set { paraId = value; } }
		public Int32 ParaIdParent { get { return paraIdParent; } set { paraIdParent = value; } }
		public Int32 Done { get { return done; } set { done = value; } }
	}
	#endregion
	#region CommentExListInfoCollection
	public class CommentExListInfoCollection {
		Dictionary<int, CommentExListInfo> mapParaId = new Dictionary<int,CommentExListInfo>();
		public void Add(CommentExListInfo info) {
			mapParaId.Add(info.ParaId, info);
		}
		public CommentExListInfo FindByParaId(int paraId) {
			CommentExListInfo listInfo;
			if(mapParaId.TryGetValue(paraId, out listInfo))
				return listInfo;
			else
				return null;
		}
	}
	#endregion
}
