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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using EnvDTE;
namespace DevExpress.Utils.Design {
	class InconsistentProjectStateException : Exception {
		public InconsistentProjectStateException() : base(string.Empty) { }
	}
	public class ProjectImage {
		string name;
		Image image;
		string resourceName;
		Type parentType;
		[CLSCompliant(false)]
		public ProjectImage(IServiceProvider sp, EnvDTE.CodeProperty codeProperty) {
			this.name = codeProperty.Name;
			this.resourceName = codeProperty.Parent.Name;
			this.image = CreateImage(sp, codeProperty);
		}
		public string Name { get { return this.name; } }
		public Image Image { get { return this.image; } }
		public string ResourceName { get { return this.resourceName; } }
		public Type ParentType { get { return this.parentType; } }
		public bool IsMatch(ProjectImageInfo img) {
			return string.Equals(ParentType.FullName, img.ParentType.FullName, StringComparison.OrdinalIgnoreCase) && string.Equals(Name, img.Name, StringComparison.OrdinalIgnoreCase);
		}
		[CLSCompliant(false)]
		protected virtual Image CreateImage(IServiceProvider sp, EnvDTE.CodeProperty codeProperty) {
			if(sp == null) return null;
			Type targetType = null;
			ITypeDiscoveryService svc = (ITypeDiscoveryService)sp.GetService(typeof(ITypeDiscoveryService));
			ICollection types = svc.GetTypes(typeof(object), true);
			foreach(Type type in types) {
				if(string.Equals(type.FullName, codeProperty.Parent.FullName, StringComparison.Ordinal)) {
					targetType = type;
					break;
				}
			}
			if(targetType == null) throw new InconsistentProjectStateException();
			this.parentType = targetType;
			PropertyInfo pi = targetType.GetProperty(codeProperty.Name, BindingFlags.Static | BindingFlags.NonPublic);
			if(pi == null) throw new InconsistentProjectStateException();
			return (Image)pi.GetValue(null, null);
		}
		public override string ToString() {
			return Name;
		}
	}
	[CLSCompliant(false)]
	public class ProjectResourceInfo : Collection<ProjectImage> {
		string resourceName;
		static ProjectResourceInfoComparer comparerCore = new ProjectResourceInfoComparer();
		public ProjectResourceInfo(string name) {
			this.resourceName = name;
		}
		public void Sort() {
			List.Sort(comparerCore);
		}
		public string ResourceName { get { return this.resourceName; } }
		protected List<ProjectImage> List { get { return (List<ProjectImage>)base.Items; } }
		public override string ToString() {
			return ResourceName;
		}
		public override bool Equals(object obj) {
			ProjectResourceInfo info = (ProjectResourceInfo)obj;
			return string.Equals(info.ResourceName, ResourceName, StringComparison.Ordinal);
		}
		public override int GetHashCode() {
			return ResourceName.GetHashCode();
		}
	}
	class ProjectResourceInfoComparer : IComparer<ProjectImage> {
		public int Compare(ProjectImage x, ProjectImage y) {
			return string.Compare(x.Name, y.Name);
		}
	}
	[CLSCompliant(false)]
	public class ProjectResourceCollection : Collection<ProjectResourceInfo> {
		static ProjectResourceCollectionComparer comparerCore = new ProjectResourceCollectionComparer();
		public void Sort() {
			List.Sort(comparerCore);
		}
		protected List<ProjectResourceInfo> List { get { return (List<ProjectResourceInfo>)base.Items; } }
	}
	class ProjectResourceCollectionComparer : IComparer<ProjectResourceInfo> {
		public int Compare(ProjectResourceInfo x, ProjectResourceInfo y) {
			return string.Compare(x.ResourceName, y.ResourceName);
		}
	}
	class ProjectImageResourceResearcher : ProjectResearcherBase {
		IServiceProvider sp;
		ProjectResourceCollection res;
		public ProjectImageResourceResearcher(EnvDTE.Project project, IServiceProvider sp)
			: base(project) {
			this.sp = sp;
			this.res = new ProjectResourceCollection();
		}
		public override void Refresh(object data) {
			res.Clear();
			base.Refresh(data);
		}
		protected override bool AllowTraverse(ProjectItem item) {
			return IsResXDesigner(item);
		}
		protected bool IsResXDesigner(ProjectItem item) {
			ProjectItem current = GetParent(item);
			while(current != null) {
				if(IsResX(current)) return true;
				current = GetParent(current);
			}
			return false;
		}
		protected ProjectItem GetParent(ProjectItem item) {
			ProjectItem parent = null;
			try {
				parent = item.Collection != null ? item.Collection.Parent as ProjectItem : null;
			}
			catch { }
			return parent;
		}
		protected bool IsResX(ProjectItem item) {
			bool isResx = false;
			try {
				isResx = item.Name.EndsWith(".resx");
			}
			catch { }
			return isResx;
		}
		public override void ProcessCodeElement(EnvDTE.CodeElement element, object data) {
			if(element.Kind != EnvDTE.vsCMElement.vsCMElementProperty)
				return;
			EnvDTE.CodeProperty codeProperty = (EnvDTE.CodeProperty)element;
			if(IsPropertyValid(codeProperty)) SaveImageInfo(codeProperty);
		}
		protected override void Completed(object data) {
			base.Completed(data);
			SortResult();
		}
		void SortResult() {
			if(res.Count == 0) return;
			if(res.Count > 1) res.Sort();
			foreach(ProjectResourceInfo resourceInfo in res)
				if(resourceInfo.Count > 1) resourceInfo.Sort();
		}
		bool IsPropertyValid(EnvDTE.CodeProperty codeProperty) {
			return IsTypeValid(codeProperty) && IsResourceRepresenting(codeProperty);
		}
		bool IsTypeValid(EnvDTE.CodeProperty codeProperty) {
			EnvDTE.CodeTypeRef codeTypeRef = codeProperty.Type;
			if(codeTypeRef.TypeKind != EnvDTE.vsCMTypeRef.vsCMTypeRefCodeType)
				return false;
			return codeTypeRef.CodeType.get_IsDerivedFrom(typeof(Image).FullName) && codeProperty.Getter != null;
		}
		bool IsResourceRepresenting(EnvDTE.CodeProperty codeProperty) {
			EnvDTE.CodeElements attributes = codeProperty.Parent.Attributes;
			foreach(EnvDTE.CodeElement attribute in attributes) {
				if(attribute.Name != null && attribute.Name.EndsWith(typeof(CompilerGeneratedAttribute).FullName))
					return true;
			}
			return false;
		}
		void SaveImageInfo(EnvDTE.CodeProperty codeProperty) {
			try {
				ProjectImage imageInfo = new ProjectImage(this.sp, codeProperty);
				SaveImageInfoCore(imageInfo);
			}
			catch(InconsistentProjectStateException) { }
		}
		void SaveImageInfoCore(ProjectImage imageInfo) {
			ProjectResourceInfo resourceInfo = new ProjectResourceInfo(imageInfo.ResourceName);
			if(res.Contains(resourceInfo)) res[res.IndexOf(resourceInfo)].Add(imageInfo);
			else {
				resourceInfo.Add(imageInfo);
				res.Add(resourceInfo);
			}
		}
		public ProjectResourceCollection ImageResourceCollection { get { return this.res; } }
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(this.sp != null)
					this.sp = null;
				if(this.res != null)
					this.res.Clear();
				this.res = null;
			}
			base.Dispose(disposing);
		}
	}
}
