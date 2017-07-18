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

namespace CustomAttributeGenerator
{
	/// <summary>
	/// Represents an entity property.
	/// </summary>
	public class EntityProperty
	{
		/// <summary>
		/// Initializes a new <see cref="EntityProperty"/>.
		/// </summary>
		/// <param name="name">The property name</param>
		/// <param name="type">The property type</param>
		public EntityProperty(string name, EntityPropertyType type)
		{
			Name = name;
			Type = type;
		}

		/// <summary>
		/// The property name.
		/// </summary>
		public string Name { get; }

		/// <summary>
		/// The property type.
		/// </summary>
		public EntityPropertyType Type { get; }
	}
}