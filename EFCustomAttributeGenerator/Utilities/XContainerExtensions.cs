﻿//  Entity Designer Custom Attribute Generator
//  Copyright 2017 Christian DeVita - chris.devita@gmail.com
//  Based off of https://github.com/mthamil/EFDocumentationGenerator
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;

namespace CustomAttributeGenerator.Utilities
{
    /// <summary>
    /// Provides extension methods for <see cref="XElement"/>s.
    /// </summary>
    internal static class XContainerExtensions
    {
        /// <summary>
        /// Provides access to edm schema XML namespace-specific operations.
        /// </summary>
        public static INamespacedOperations Edm(this XContainer element) => new NamespacedOperations(element, EdmNamespace);

        private const string EdmNamespace = "http://schemas.microsoft.com/ado/2009/11/edm";

        /// <summary>
        /// Provides access to XML namespace-specific operations.
        /// </summary>
        public interface INamespacedOperations
        {
            /// <summary>
            /// Returns the Descendant <see cref="XElement"/>s with the passed in names from the EDM namespace 
            /// as an <see cref="IEnumerable{XElement}"/>
            /// </summary> 
            /// <param name="name">The name to match against descendant <see cref="XElement"/>s.</param>
            /// <returns>An <see cref="IEnumerable"/> of <see cref="XElement"/></returns> 
            IEnumerable<XElement> Descendants(string name);

            /// <summary>
            /// Returns the child element with the specified name or null if there is no matching child element. 
            /// <seealso cref="XContainer.Elements()"/>
            /// </summary> 
            /// <param name="name"> 
            /// The element name to match against this <see cref="XContainer"/>'s child elements.
            /// </param> 
            /// <returns>
            /// An <see cref="XElement"/> child that matches the name passed in, or null.
            /// </returns>
            XElement Element(string name);

            /// <summary>
            /// Returns the child elements of an <see cref="XContainer"/> that match the name passed in.
            /// </summary> 
            /// <param name="name">
            /// The element name to match against the <see cref="XElement"/> children of this <see cref="XContainer"/>. 
            /// </param> 
            /// <returns>
            /// An <see cref="IEnumerable"/> of <see cref="XElement"/> children of this <see cref="XContainer"/> that have 
            /// a matching name.
            /// </returns>
            IEnumerable<XElement> Elements(string name);

            /// <summary>
            /// Returns the attributes of an <see cref="XContainer"/> that match the name passed in.
            /// </summary> 
            /// <param name="name">
            /// The attribute name to match against the <see cref="XAttribute"/> attributes of this <see cref="XContainer"/>. 
            /// </param> 
            /// <returns>
            /// An <see cref="IEnumerable"/> of <see cref="XAttribute"/> attributes of this <see cref="XContainer"/> that have 
            /// a matching name.
            /// </returns>
            IEnumerable<XAttribute> Attributes(string name);

            /// <summary>
            /// The XML namespace a set of operations are for.
            /// </summary>
            string Namespace { get; }
        }

        private class NamespacedOperations : INamespacedOperations
        {
            public NamespacedOperations(XContainer element, string nameSpace)
            {
                _element = element;
                Namespace = nameSpace;
            }

            /// <see cref="INamespacedOperations.Descendants"/>
            public IEnumerable<XElement> Descendants(string name) => _element.Descendants(XName.Get(name, Namespace));

            /// <see cref="INamespacedOperations.Element"/>
            public XElement Element(string name) => _element.Element(XName.Get(name, Namespace));

            /// <see cref="INamespacedOperations.Elements"/>
            public IEnumerable<XElement> Elements(string name) => _element.Elements(XName.Get(name, Namespace));

            /// <see cref="INamespacedOperations.Attributes"/>
            public IEnumerable<XAttribute> Attributes(string name)
            {
                if (_element.GetType() == typeof(XElement))
                {
                    XElement xElement = (XElement)_element;
                    return xElement.Attributes();
                }
                else
                    return null;
            }

            /// <see cref="INamespacedOperations.Namespace"/>
            public string Namespace { get; }

            private readonly XContainer _element;
        }
    }
}