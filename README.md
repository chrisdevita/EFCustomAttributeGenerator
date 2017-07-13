Entity Designer Custom Attribute Generator
========================

This is a Visual Studio plugin for the ADO.NET Entity Designer. It hooks into the model update process
in order to pull extended properties from a SQL Server database and populate an entity 
model's (.edmx file) with custom attributes nodes with them. This makes metadata available for use 
during code generation.

The plugin searches the project that an .edmx file belongs to for an App.config containing an Entity
Framework connection string so that it can connect to the database.

This is a fork based on https://github.com/mthamil/EFDocumentationGenerator

Currently developing.