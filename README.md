# ToggleCore
ToggleCore is a C# Library for implementing Feature Toggles through the use of custom Attributes. With this library, you, as a developer, can delegate Feature Toggles from your own database or application to work at runtime by referencing it in a simple attribute. The library will fetch the correct data and alter the code flow based on the toggle, additional rules, or even expiration date, if set, without the nested if statements and difficult maintainability associated with a bad implementation of a Feature Toggle systems. This library can be used in new projects, as well as in projects that already have been deployed.

## Configuration
The latest version of the ToggleCore library can be installed from [nuget.org](https://www.nuget.org/packages/ToggleCoreLibrary/) or using the dotnet cli:
```
      dotnet add package ToggleCoreLibrary --version 1.0.4
```
Before starting up, this library requires the MrAdvice library, wich should already have been installed with the instalation of ToggleCore, if not, intall it. Make sure that all the projects you intend to use the new FeatureToggle custom attribute has a reference to the MrAdvice library. You can do this adding the following line in the itemGroup of your project's .csproj:
```xml
      <PackageReference Include="MrAdvice" Version="2.16.0" />
```
After referencing the package, you need to connect your Feature Toggle database or API to the library. **This library does not manage your feature toggles for you**, it uses existing toggles to control code flow without the use of nested if statements and other coding practices that could harm future maintenance, creating bloated methods and other code smells. 

There is two ways to connect your feature toggle database: 
* create an app.config file in your project. In this file, you will be able to connect a database to the library by providing the server name, database name, and integrated security option. Here is an example of an app.config with the requiered informations:
  ```xml
  <configuration>
	<configSections>
		<section name ="FeatureToggleConfig"
				 type="System.Configuration.NameValueSectionHandler"/>
	</configSections>
	<FeatureToggleConfig>
		<add key="server" value="YOUR-SERVER-NAME" />
		<add key="database" value="YOUR-DATABASE-NAME" />
		<add key="integratedSecurity" value="true" />
		<add key="trustServerCertificate" value="true" />
	</FeatureToggleConfig>
  </configuration>
``
  The section name has to obligatorily be "FeatureToggleConfig".
  This configuration will use the base feature toggle mapper already implemented in the library, however it uses SqlServer.
* Houwever, if you use another database, such as PostgreSql, or use a feature toggle managment API like [Unleash](https://www.getunleash.io/), you can also connect them! **Simply create a new FeatureToggleMapper** and use the FeatureToggleMapperHandler.SetMapper(YourMapper). FeatureToggleMapper is a public interface implemented in the library used to implement a mapper that will translate the feature toggle data stored wherever it is, to a FeatureToggle object that can be interpreted by the custom attribute. Here is a sample of FeatureToggleMapper (this is the custom Mapper used by the library):

```c#
  public class FeatureToggleDbMapper : FeatureToggleMapper
  {
      // Place here the connection string or route if needed
      public ApplicationDbContext Context { get; set; }
      public static NameValueCollection section = (NameValueCollection)ConfigurationManager.GetSection("FeatureToggleConfig");
      public readonly DbContextOptions<ApplicationDbContext> contextOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
         .UseSqlServer(@$"Server={section["server"]};Database={section["database"]};ConnectRetryCount=0;Integrated Security={section["integratedSecurity"]};TrustServerCertificate={section["trustServerCertificate"]}")
         .Options;
  
      // Map function
      public override FeatureToggleModel Map(string featureToggleId)
      {
          Context = new ApplicationDbContext(contextOptions);
  
          // Get the feature toggle data from database or API (in this case a Sql Server database)
          var featureToggle = Context.featureToggleModels.FirstOrDefault(x => x.ToggleId.Equals(featureToggleId));
          if (featureToggle == null)
          {
              return new FeatureToggleModel()
              {
                  ToggleId = featureToggleId,
                  Toggle = false,
  
              };
          }
  
          Dictionary<string, List<string>>? ruleString = null;
          if (featureToggle.AdditionalRulesJson != null)
          {
              ruleString = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(featureToggle.AdditionalRulesJson);
          }
  
          // Map the feature toggle data into a Feature Toggle object
          return new FeatureToggleModel(
              featureToggle.ToggleId,
              featureToggle.Toggle,
              featureToggle.CreationDate,
              featureToggle.ExpirationDate,
              ruleString);
      }
  }
```
The code ubove shows the base mapper used, however, the structure of a custome mapper will be similar, using a conection request, getting the feature toggle data and mapping it into a FeatureToggle object.

**Remember** to include the FeatureToggleMapperHandler.SetMapper(YourCustomMapper) in the project's startup tu set your mapper as the one to be used.

