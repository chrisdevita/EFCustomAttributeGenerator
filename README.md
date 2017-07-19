Entity Designer Custom Attribute Generator
========================

This is a Visual Studio plugin for the ADO.NET Entity Designer. It hooks into the model update process
in order to pull extended properties from a SQL Server database and populate an entity 
model's (.edmx file) with custom attributes nodes with them. This makes metadata available for use 
during code generation.

The plugin searches the project that an .edmx file belongs to for an App.config containing an Entity
Framework connection string so that it can connect to the database.

This is a fork based on https://github.com/mthamil/EFDocumentationGenerator

This project also maintains the original functionality of EF Document Generator and pulls MS_Description 
extended properties from a SQL Server database and populate an entity model's (.edmx file) Documentation 
nodes with them. Any other extended properties that are found that are not named MS_Description get added 
as a custom attribute to the corresponding node in the .edmx file.

You can access these extened properties in code generation files generates your POCO classes.

For example from the Model.tt autogenerate file that is associated with your edmx file and where the name of your extended property is 'Custom_Attribute':
<pre>
<#
	var simpleProperties = typeMapper.GetSimpleProperties(entity);
    if (simpleProperties.Any())
    {
		foreach (var edmProperty in simpleProperties)
        {
			if (edmProperty.MetadataProperties.Contains("http://CustomNamespace.com:Custom_Attribute"))
			{
				MetadataProperty annotationProperty = edmProperty.MetadataProperties["http://CustomNamespace.com:Custom_Attribute"];
#>
//<#=annotationProperty.Name #>:<#=annotationProperty.Value.ToString() #>
<#
			}
#>
    <#=codeStringGenerator.Property(edmProperty)#>
<#
        }
    }
#>
</pre>