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
	/// The types of entity properties.
	/// </summary>
	public enum EntityPropertyType
	{
		/// <summary>
		/// Indicates a regular property.
		/// </summary>
		Property,

		/// <summary>
		/// Indicates a navigation property.
		/// </summary>
		NavigationProperty
	}
}