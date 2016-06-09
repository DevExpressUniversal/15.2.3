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

using System.Collections;
#if SL
using DevExpress.Utils;
using DevExpress.Xpf.Collections;
#endif
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public class FileSourceRangeCollection : CollectionBase
	{
		SourceRange[] GetRanges()
		{
			ArrayList result = new ArrayList();
			for (int i = 0; i < this.Count; i++)
				result.Add(this[i].Range);
			return (SourceRange[])result.ToArray(typeof(SourceRange));
		}
		object[] GetData()
		{
			ArrayList result = new ArrayList();
			for (int i = 0; i < this.Count; i++)
				result.Add(this[i].Data);
			return (object[])result.ToArray(typeof(object));
		}
		static FileSourceRangeCollection GetFileRanges(SourceFile file, SourceRange[] ranges)
		{
			if (file == null || ranges == null)
				return null;
			FileSourceRangeCollection result = new FileSourceRangeCollection();
			for (int i = 0; i < ranges.Length; i++)	  
				result.Add(new FileSourceRange(file, ranges[i]));
			return result;
		}
		public int Add(FileSourceRange range)
		{
			return List.Add(range);
		}
		public void AddRange(FileSourceRangeCollection ranges)
		{
			for (int i = 0; i < ranges.Count; i++)
				Add(ranges[i]);
		}
	public void AddRange(ICollection ranges)
	{
	  InnerList.AddRange(ranges);	  
	}
		public int IndexOf(FileSourceRange range)
		{
			return List.IndexOf(range);
		}
		public void Insert(int index, FileSourceRange range)
		{
			List.Insert(index, range);
		}
		public void Remove(FileSourceRange range)
		{
			List.Remove(range);
		}
		public Hashtable GroupByFile()
		{
			Hashtable result = new Hashtable();
			foreach (FileSourceRange range in this)
			{
				SourceFile fileNode = range.File;
				IProjectElement project = fileNode.Project;
				if (project != null)
					fileNode = project.FindDiskFile(fileNode) as SourceFile;
				FileSourceRangeCollection group = null;
				object obj = result[fileNode];
				if (obj == null)
				{				
					group = new FileSourceRangeCollection();
					result.Add(fileNode, group);
				}
				else
					group = (FileSourceRangeCollection)obj;
				if (group != null)
					group.Add(range);
			}
			return result;
		}
		public FileSourceRangeCollection GetRangesFromData()
		{
			FileSourceRangeCollection result = new FileSourceRangeCollection();
			for (int i = 0; i < Count; i++)
			{
				object data = this[i].Data;
				if (data == null || !(data is SourceRange[]))
					continue;
				SourceRange[] linkRanges = (SourceRange[])data;
				FileSourceRangeCollection linkFileRanges = GetFileRanges(this[i].File, linkRanges);
				if (linkFileRanges != null)
					result.AddRange(linkFileRanges);
			}
			return result;
		}
		public static FileSourceRangeCollection CreateInstance(SourceFile fileNode, SourceRange[] ranges)
		{
			return GetFileRanges(fileNode, ranges);
		}
		public SourceRange[] Ranges
		{
			get
			{
				return GetRanges();
			}
		}
		public object[] Data
		{
			get
			{
				return GetData();
			}
		}
		public FileSourceRange this[int index] 
		{
			get
			{
				return (FileSourceRange)List[index];
			}
			set
			{
				List[index] = value;
			}
		}
	}
}
