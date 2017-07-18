//  Entity Designer Custom Attribute Generator
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using CustomAttributeGenerator.Utilities;

namespace CustomAttributeGenerator
{
    /// <summary>
    /// Updates XML EDMX file documentation nodes.
    /// </summary>
    internal class ModelDocumentationUpdater : IModelDocumentationUpdater
    {
        /// <summary>
        /// Initializes a new <see cref="ModelDocumentationUpdater"/>.
        /// </summary>
        /// <param name="customAttributeSource">The custom attribute source</param>
        public ModelDocumentationUpdater(ICustomAttributeSource customAttributeSource)
        {
            _customAttributeSource = customAttributeSource;
        }

        /// <summary>
        /// Iterates over the entities in the conceptual model and attempts to populate
        /// their documentation nodes with values from the database.
        /// Existing documentation will be removed and replaced by database content.
        /// </summary>
        /// <param name="modelDocument">An .edmx XML document to update</param>
        public void UpdateDocumentation(XDocument modelDocument)
        {
            _namespace = modelDocument.Edm().Namespace;

            var entityTypeElements = modelDocument.Edm().Descendants("EntityType").ToList();
            foreach (var entityType in entityTypeElements)
            {
                string tableName = entityType.Attribute("Name").Value;

                List<CustomAttribute> tableCustomAttributes = _customAttributeSource.GetCustomAttributes(tableName).ToList<CustomAttribute>();
                foreach (var customAttribute in tableCustomAttributes)
                {
                    if (customAttribute.Name.ToLower() == "ms_description")
                        UpdateNodeDocumentation(entityType, customAttribute.Value);
                }

                var properties =
                        entityType.Edm().Descendants("Property")
                                  .Select(e => new
                                  {
                                      Element = e,
                                      Property = new EntityProperty(
                                                   e.Attribute("Name").Value,
                                                   EntityPropertyType.Property)
                                  })
                    .Concat(
                        entityType.Edm().Descendants("NavigationProperty")
                                  .Select(e => new
                                  {
                                      Element = e,
                                      Property = CreateNavProperty(e, modelDocument)
                                  }));

                XNamespace customNamespace = "http://CustomNamespace.com";

                foreach (var property in properties)
                {
                    List<CustomAttribute> propertyCustomAttributes = _customAttributeSource.GetCustomAttributes(tableName, property.Property).ToList<CustomAttribute>();

                    // No need to add namespace attribute since it automatically adds for you.
                    //if (propertyCustomAttributes.Count > 0)
                    //    property.Element.SetAttributeValue(XNamespace.Xmlns + "a", "http://CustomNamespace.com");

                    foreach (var customAttribute in propertyCustomAttributes)
                    {
                        if (customAttribute.Name.ToLower() == "ms_description")
                            UpdateNodeDocumentation(property.Element, customAttribute.Value);
                        else
                            UpdateNodeCustomAttribute(customNamespace, property.Element, customAttribute.Name, customAttribute.Value);
                    }
                }
            }
        }

        private void UpdateNodeDocumentation(XContainer element, string documentation)
        {
            if (String.IsNullOrWhiteSpace(documentation))
                return;

            var fixedDocumentation = documentation.Trim();

            // Remove existing documentation.
            element.Edm().Descendants("Documentation").Remove();

            element.AddFirst(new XElement(XName.Get("Documentation", _namespace),
                                          new XElement(XName.Get("Summary", _namespace), fixedDocumentation)));
        }

        private void UpdateNodeCustomAttribute(XNamespace customNamespace, XElement element, string name, string value)
        {
            if (String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(value))
                return;

            var fixedDocumentation = value.Trim();

            element.SetAttributeValue(customNamespace + name, value);
        }

        private void AddCustomNamespace(XElement element)
        {
            element.SetAttributeValue(XNamespace.Xmlns + "ca", "http://CustomNamespace.com");
        }


        private static EntityProperty CreateNavProperty(XElement element, XContainer document)
        {
            var relationship = element.Attribute("Relationship").Value;
            var association = document.Edm()
                .Descendants("AssociationSet")
                .Single(ae => ae.Attribute("Association").Value == relationship);

            return new EntityProperty(
                association.Attribute("Name").Value,
                EntityPropertyType.NavigationProperty);
        }

        private string _namespace;

        private readonly ICustomAttributeSource _customAttributeSource;
    }
}