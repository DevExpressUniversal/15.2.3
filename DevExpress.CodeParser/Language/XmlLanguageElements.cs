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
using System.Collections.Specialized;
#if DXCORE
namespace DevExpress.CodeRush.StructuralParser
#else
namespace DevExpress.CodeParser
#endif
{
	public enum XmlNodeType
	{
		Default,
		Document,
		Decl,
		DocTypeDecl,
		ExternalSystemID,
		ExternalPublicID,
		ElementDecl,
		EmptyContentSpec,
		AnyContentSpec,
		MixedContentSpec,
		ChildrenContentSpec,
		Name,
		NameReference,
		NamedCP,
		CPSequence,
		CPChoice,
		NotationDecl,
		AttributeListDecl,
		AttributeDecl,
		EntityDecl,
		ProcessingInstruction,
		CharReference,
		Reference,
		CharacterData
	}
	public class BaseXmlNode : XmlNode
	{
		public BaseXmlNode()
		{
			Name = String.Empty;
		}
		public virtual XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.Default;
			}
		}
		public override LanguageElementType ElementType
		{
			get
			{
				return  LanguageElementType.XmlDocElement;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			BaseXmlNode lClone = new BaseXmlNode();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is BaseXmlNode))
				return;
			BaseXmlNode lSource = (BaseXmlNode)source;
		}
		#endregion
	}
	public class XmlDecl : BaseXmlNode
	{
		string _Version = String.Empty;
		string _Encoding = String.Empty;
		string _StandAlone = String.Empty;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.Decl;
			}
		}
		public string Version
		{
			get
			{
				return _Version;
			}
			set
			{
				_Version = value;
			}
		}
		public string Encoding
		{
			get
			{
				return _Encoding;
			}
			set
			{
				_Encoding = value;
			}
		}
		public string StandAlone
		{
			get
			{
				return _StandAlone;
			}
			set
			{
				_StandAlone = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlDecl lClone = new XmlDecl();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlDecl))
				return;
			XmlDecl lSource = (XmlDecl)source;
			_Version = lSource._Version;
			_Encoding = lSource._Encoding;
			_StandAlone = lSource._StandAlone;
		}
		#endregion
	}
	public abstract class NewExternalIDLink : BaseXmlNode
	{}
	public class NewExternalIDSystemLink : NewExternalIDLink
	{
		string _SystemURI = String.Empty;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.ExternalSystemID;
			}
		}		
		public string SystemURI
		{
			get
			{
				return _SystemURI;
			}
			set
			{
				_SystemURI = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NewExternalIDSystemLink lClone = new NewExternalIDSystemLink();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is NewExternalIDSystemLink))
				return;
			NewExternalIDSystemLink lSource = (NewExternalIDSystemLink)source;
			_SystemURI = lSource._SystemURI;
		}
		#endregion
	}
	public class NewExternalIDPublicLink : NewExternalIDLink
	{
		string _SystemURI = String.Empty;
		string _PublicID = String.Empty;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.ExternalPublicID;
			}
		}
		public string SystemURI
		{
			get
			{
				return _SystemURI;
			}
			set
			{
				_SystemURI = value;
			}
		}
		public string PublicID
		{
			get
			{
				return _PublicID;
			}
			set
			{
				_PublicID = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			NewExternalIDPublicLink lClone = new NewExternalIDPublicLink();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is NewExternalIDPublicLink))
				return;
			NewExternalIDPublicLink lSource = (NewExternalIDPublicLink)source;
			_SystemURI = lSource._SystemURI;
			_PublicID = lSource._PublicID;
		}
		#endregion
	}
	public class XmlDocTypeDecl : BaseXmlNode
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.DocTypeDecl;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlDocTypeDecl lClone = new XmlDocTypeDecl();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlDocTypeDecl))
				return;
			XmlDocTypeDecl lSource = (XmlDocTypeDecl)source;
		}
		#endregion
	}
	public class XmlElementDecl : BaseXmlNode
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.ElementDecl;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlElementDecl lClone = new XmlElementDecl();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlElementDecl))
				return;
			XmlElementDecl lSource = (XmlElementDecl)source;
		}
		#endregion
	}
	public abstract class XmlBaseContentSpec : BaseXmlNode
	{
	}
	public class XmlEmptyContentSpec : XmlBaseContentSpec
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.EmptyContentSpec;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlEmptyContentSpec lClone = new XmlEmptyContentSpec();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlEmptyContentSpec))
				return;
			XmlEmptyContentSpec lSource = (XmlEmptyContentSpec)source;
		}
		#endregion
	}
	public class XmlAnyContentSpec : XmlBaseContentSpec
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.AnyContentSpec;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlAnyContentSpec lClone = new XmlAnyContentSpec();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlAnyContentSpec))
				return;
			XmlAnyContentSpec lSource = (XmlAnyContentSpec)source;
		}
		#endregion
	}
	public class XmlMixedContentSpec : XmlBaseContentSpec
	{
		LanguageElementCollection _Names = null;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.MixedContentSpec;
			}
		}
		public LanguageElementCollection Names
		{
			get
			{
				if (_Names == null)
					_Names = new LanguageElementCollection();
				return _Names;
			}
		}
		public int NamesCount
		{
			get
			{
				if (_Names == null)
					return 0;
				else
					return _Names.Count;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlMixedContentSpec lClone = new XmlMixedContentSpec();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlMixedContentSpec))
				return;
			XmlMixedContentSpec lSource = (XmlMixedContentSpec)source;
			Names.Clear();
			for (int i = 0; i < lSource.Names.Count; i++)
			{
				if (lSource.Names[i] != null)
					Names.Add(lSource.Names[i].Clone() as LanguageElement);
			}
		}
		#endregion
	}
	public class XmlName : BaseXmlNode
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.Name;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlName lClone = new XmlName();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlName))
				return;
			XmlName lSource = (XmlName)source;
		}
		#endregion
	}
	public class XmlNameReference : XmlName
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.NameReference;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlNameReference lClone = new XmlNameReference();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlNameReference))
				return;
			XmlNameReference lSource = (XmlNameReference)source;
		}
		#endregion
	}
	public enum RepeatCount
	{
		Once,
		ZeroOrOnce,
		ZeroOrMore,
		OnceOrMore
	}
	public abstract class XmlContentParticle : BaseXmlNode
	{
		RepeatCount _RepeatCount = RepeatCount.Once;
		public RepeatCount RepeatCount
		{
			get
			{
				return _RepeatCount;
			}
			set
			{
				_RepeatCount = value;
			}
		}
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlContentParticle))
				return;
			XmlContentParticle lSource = (XmlContentParticle)source;	  				
		}
		#endregion
	}
	public class XmlNamedContentParticle : XmlContentParticle
	{
		XmlName _ParticleName;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.NamedCP;
			}
		}				
		public XmlName ParticleName
		{
			get
			{
				return _ParticleName;
			}
			set
			{
				_ParticleName = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlNamedContentParticle lClone = new XmlNamedContentParticle();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlNamedContentParticle))
				return;
			XmlNamedContentParticle lSource = (XmlNamedContentParticle)source;
			if (lSource._ParticleName != null)
				_ParticleName = lSource._ParticleName.Clone() as XmlName;
	  else
				_ParticleName = null;
		}
		#endregion
	}
	public abstract class XmlSequencedContentParticle : XmlContentParticle
	{
		LanguageElementCollection _Particles;
		public LanguageElementCollection Particles
		{
			get
			{
				if (_Particles == null)
					_Particles = new LanguageElementCollection();
				return _Particles;
			}
		}
		public int ParticlesCount
		{
			get
			{
				if (_Particles == null)
					return 0;
				else
					return _Particles.Count;
			}
		}		
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlSequencedContentParticle))
				return;
			XmlSequencedContentParticle lSource = (XmlSequencedContentParticle)source;
			_Particles = lSource.Particles.DeepClone() as LanguageElementCollection;
		}
		#endregion
	}
	public class XmlChoiceContentParticle : XmlSequencedContentParticle
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.CPChoice;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlChoiceContentParticle lClone = new XmlChoiceContentParticle();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlChoiceContentParticle))
				return;
			XmlChoiceContentParticle lSource = (XmlChoiceContentParticle)source;
		}
		#endregion
	}
	public class XmlSequenceContentParticle : XmlSequencedContentParticle
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.CPSequence;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlSequenceContentParticle lClone = new XmlSequenceContentParticle();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlSequenceContentParticle))
				return;
			XmlSequenceContentParticle lSource = (XmlSequenceContentParticle)source;
		}
		#endregion
	}
	public class XmlChildrenContentSpec : XmlBaseContentSpec
	{
		XmlContentParticle _Source;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.ChildrenContentSpec;
			}
		}
		public XmlContentParticle Source
		{
			get
			{
				return _Source;
			}
			set
			{
				_Source = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlChildrenContentSpec lClone = new XmlChildrenContentSpec();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlChildrenContentSpec))
				return;
			XmlChildrenContentSpec lSource = (XmlChildrenContentSpec)source;
			if (lSource._Source != null)
				_Source = lSource._Source.Clone() as XmlContentParticle;
	  else
				_Source = null;
		}
		#endregion
	}
	public class XmlNotationDecl : BaseXmlNode
	{
		NewExternalIDLink _NotationLink;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.NotationDecl;
			}
		}
		public NewExternalIDLink NotationLink
		{
			get
			{
				return _NotationLink;
			}
			set
			{
				_NotationLink = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlNotationDecl lClone = new XmlNotationDecl();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlNotationDecl))
				return;
			XmlNotationDecl lSource = (XmlNotationDecl)source;
			if (lSource._NotationLink != null)
				_NotationLink = lSource._NotationLink.Clone() as NewExternalIDLink;
			else
				_NotationLink = null;	 				
		}
		#endregion
	}
	public class XmlAttributeListDeclaration : BaseXmlNode
	{
		LanguageElementCollection _AttributesDecl;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.AttributeListDecl;
			}
		}
		public LanguageElementCollection AttributesDecl
		{
			get
			{
				if (_AttributesDecl == null)
					_AttributesDecl = new LanguageElementCollection();
				return _AttributesDecl;
			}
		}
		public int AttributesDeclCount
		{
			get
			{
				if (_AttributesDecl == null)
					return 0;
				else 
					return _AttributesDecl.Count;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlAttributeListDeclaration lClone = new XmlAttributeListDeclaration();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlAttributeListDeclaration))
				return;
			XmlAttributeListDeclaration lSource = (XmlAttributeListDeclaration)source;
			_AttributesDecl = lSource.AttributesDecl.DeepClone() as LanguageElementCollection;
		}
		#endregion
	}
	public enum AttributeType
	{
		Default,
		Id,
		CData,
		IdRef,
		IdRefs,
		Entity,
		Entities,
		NmToken,
		NmTokens,
		Enumeration,
		Notation
	}
	public enum DefaultAttributeValueType
	{
		None,
		Required,
		Implied,
		Fixed
	}
	public class XmlAttributeDeclaration :BaseXmlNode
	{
		AttributeType _AttributeType = AttributeType.Default;
		StringCollection _EnumerationMembers;
		DefaultAttributeValueType _DefaultAttributeValueType = DefaultAttributeValueType.None;
		string _DefaultAttributeValue = String.Empty;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.AttributeDecl;
			}
		}
		public AttributeType AttributeType
		{
			get
			{
				return _AttributeType;
			}
			set
			{
				_AttributeType = value;
			}
		}
		public StringCollection EnumerationMembers
		{
			get
			{
				if (_EnumerationMembers == null)
					_EnumerationMembers = new StringCollection();
				return _EnumerationMembers;
			}
		}
		public int EnumerationMembersCount
		{
			get
			{
				if (_EnumerationMembers == null)
					return 0;
				else
					return _EnumerationMembers.Count;
			}
		}
		public DefaultAttributeValueType DefaultAttributeValueType
		{
			get
			{
				return _DefaultAttributeValueType;
			}
			set
			{
				_DefaultAttributeValueType = value;
			}
		}
		public string DefaultAttributeValue
		{
			get
			{
				return _DefaultAttributeValue;
			}
			set
			{
				_DefaultAttributeValue = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlAttributeDeclaration lClone = new XmlAttributeDeclaration();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlAttributeDeclaration))
				return;
			XmlAttributeDeclaration lSource = (XmlAttributeDeclaration)source;
			_AttributeType = lSource._AttributeType;
			EnumerationMembers.Clear();
			for (int i = 0; i < lSource.EnumerationMembers.Count; i++)
			{
				EnumerationMembers.Add(lSource.EnumerationMembers[i]);
			}
			_DefaultAttributeValueType = lSource._DefaultAttributeValueType;
			_DefaultAttributeValue = lSource._DefaultAttributeValue;
		}
		#endregion
	}
	public class XmlEntityDecl : BaseXmlNode
	{
		bool _IsParameterEntity = false;
		string _ImmediateValue = String.Empty;
		NewExternalIDLink _ExternalLinkValue = null;
		string _NDataValue = String.Empty;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.EntityDecl;
			}
		}
		public bool IsParameterEntity
		{
			get
			{
				return _IsParameterEntity;
			}
			set
			{
				_IsParameterEntity = value;
			}
		}
		public string ImmediateValue
		{
			get
			{
				return _ImmediateValue;
			}
			set
			{
				_ImmediateValue = value;
			}
		}
		public NewExternalIDLink ExternalLinkValue
		{
			get
			{
				return _ExternalLinkValue;
			}
			set
			{
				_ExternalLinkValue = value;
			}
		}
		public string NDataValue
		{
			get
			{
				return _NDataValue;
			}
			set
			{
				_NDataValue = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlEntityDecl lClone = new XmlEntityDecl();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlEntityDecl))
				return;
			XmlEntityDecl lSource = (XmlEntityDecl)source;
			_IsParameterEntity = lSource._IsParameterEntity;
			_ImmediateValue = lSource._ImmediateValue;
			if (lSource._ExternalLinkValue != null)
				_ExternalLinkValue = lSource._ExternalLinkValue.Clone() as NewExternalIDLink;
			else
				_ExternalLinkValue = null;
			_NDataValue = lSource._NDataValue;
		}
		#endregion
	}
	public class XmlProcessingInstruction : BaseXmlNode
	{
		string _InstructionText;
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.ProcessingInstruction;
			}
		}
		public string InstructionText
		{
			get
			{
				return _InstructionText;
			}
			set
			{
				_InstructionText = value;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlProcessingInstruction lClone = new XmlProcessingInstruction();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlProcessingInstruction))
				return;
			XmlProcessingInstruction lSource = (XmlProcessingInstruction)source;
			_InstructionText = lSource._InstructionText;
		}
		#endregion
	}
	public class XmlCharReference : BaseXmlNode
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.CharReference;
			}
		}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlCharReference lClone = new XmlCharReference();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlCharReference))
				return;
			XmlCharReference lSource = (XmlCharReference)source;
		}
		#endregion
	}
	public class XmlReference : BaseXmlNode
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.Reference;
			}
		}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.XmlReference;
	  }
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlReference lClone = new XmlReference();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlReference))
				return;
			XmlReference lSource = (XmlReference)source;
		}
		#endregion
	}
	public class XmlCharacterData : BaseXmlNode
	{
		public override XmlNodeType XmlNodeType
		{
			get
			{
				return XmlNodeType.CharacterData;
			}
		}
	public override LanguageElementType ElementType
	{
	  get
	  {
		return LanguageElementType.XmlCharacterData;
	  }
	}
		#region Clone
		public override BaseElement Clone(ElementCloneOptions options)
		{
			XmlCharacterData lClone = new XmlCharacterData();
			lClone.CloneDataFrom(this, options);
			return lClone;
		}
		#endregion
		#region CloneDataFrom
		protected override void CloneDataFrom(BaseElement source, ElementCloneOptions options)
		{			
			if (source == null)
				return;
			base.CloneDataFrom(source, options);
			if (!(source is XmlCharacterData))
				return;
			XmlCharacterData lSource = (XmlCharacterData)source;
		}
		#endregion
	}
}
