# ToggleCore
ToggleCore is a C# Library for implementing Feature Toggles through the use of custom Attributes. With this library, you, as a developer, can delegate Feature Toggles from your own database or application to work at runtime by referencing it in a simple attribute. The library will fetch the correct data and alter the code flow based on the toggle, additional rules, or even expiration date, if set, without the nested if statements and difficult maintainability associated with a bad implementation of a Feature Toggle system. This library can be used in new projects, as well as in projects that have already been deployed.

## Configuration
The latest version of the ToggleCore library can be installed from [nuget.org](https://www.nuget.org/packages/ToggleCoreLibrary/) or using the dotnet cli:
```
      dotnet add package ToggleCoreLibrary --version 1.0.4
```
Before starting up, this library requires the MrAdvice library, which should already have been installed with the installation of ToggleCore, if not, install it. Make sure that all the projects you intend to use the new FeatureToggle custom attribute has a reference to the MrAdvice library. You can do this adding the following line in the itemGroup of your project's .csproj:
```xml
      <PackageReference Include="MrAdvice" Version="2.16.0" />
```
After referencing the package, you need to connect your Feature Toggle database or API to the library. **This library does not manage your feature toggles for you**, it uses existing toggles to control code flow without the use of nested if statements and other coding practices that could harm future maintenance, creating bloated methods and other code smells. 

There is two ways to connect your feature toggle database: 
* create an app.config file in your project. In this file, you will be able to connect a database to the library by providing the server name, database name, and integrated security option. Here is an example of an app.config with the required informations:
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
The section name has to obligatorily be "FeatureToggleConfig".
This configuration will use the base feature toggle mapper already implemented in the library, however it uses SqlServer.
* However, if you use another database, such as PostgreSql, or use a feature toggle management API like [Unleash](https://www.getunleash.io/), you can also connect them! **Simply create a new FeatureToggleMapper** and use the ```FeatureToggleMapperHandler.SetMapper(YourMapper)```. FeatureToggleMapper is a public interface implemented in the library used to implement a mapper that will translate the feature toggle data stored wherever it is, to a FeatureToggleModel object that can be interpreted by the custom attribute. Here is a sample of FeatureToggleMapper (this is the custom Mapper used by the library):

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
	```
The code above shows the base mapper used, however, the structure of a custom mapper will be similar, using a connection request, getting the feature toggle data and mapping it into a FeatureToggleModel object.

**Remember** to include the ```FeatureToggleMapperHandler.SetMapper(YourCustomMapper)``` in the project's startup tu set your mapper as the one to be used.

Make sure to have the following data (the names are not obligatory, as long as they are mapped correctly):

* ToggleId (string ID for the toggle);
* Toggle (a bit or boolean type);
*  CreationDate (date - nullable);
*  ExpirationDate (date - nullable - do not fill this column if you do not want an expiration date);
*  AdditionalRules (text in JSON format - nullable - will be read as a string and converted to a dictionary where the additional rule will be compared to what is set in the app.config).

To use the additional rules, you can add values to an appSetings in an app.config file (it can be the same you used before) that will be compared with the arguments set in the AdditionalRules attribute of the FeatureToggleModel. Here is an example:
```xml
	<appSettings>
		<add key ="Country" value="br"/>
	</appSettings>
```
This is set as the default way of using additional rules. However, just like the FeatureToggleMapper, you can extend the DynamicRulesMapper to create a dynamic mapper using customized data and rules. Simply create a customized DynamicRules class and set it as default using the ```DynamicRulesHandler.SetMapper(YourMapper)``` in your startup file.

## How To Use
With your code completely configured to the library you can now start using the new attribute ```[FeatureToggle("toggleId")]``` which will set your method to be intercepted by the attribute class where the feature toggle operation work. The class will seek the data using your toggle id and then make the necessary checks to see if the code should proceed or not.

**This custom attribute only works when used in method declarations.**

This means that, if you already use feature toggles in your code, refactoring will be necessary. However, this is the library's intended purpose, to make the code more readable and less complex through refactoring. The default way you will use this attribute is by dividing your main method in separate functions, as you can see in the example below:
```c#
	   public void TestMethod()
	   {
		TestMethod1();
		TestMethod2();
	    }
	
	    [FeatureToggle("FT0001")]
	    public void TestMethod1()
	    {
		DoSomthing(param1);
	    }
	
	    [FeatureToggle("FT1000")]
	    public void TestMethod2()
	    {
		DoSomthing(param2);
	    }
```
Feature Toggle Rules:
* If the id of a feature toggle does not exist, the attribute will work as if the toggle is off;
* If there are no additional rules, the attribute will only check the toggle if on or off;
* If there is additional rules, the attribute will only proceed if both the toggle is true and the mapped additional rule parameter is present in the feature toggle;
* If a feature toggle does not have expiration date, it will work indefinitely, use this if you do not want to set an expiration date;
* If a feature toggle has an expiration date, the toggle will automatically be considered on if the date has been reached. **Be warned!** Only set an expiration date if you are okay with this happening.

These rules can be seen in the library's diagram below:

![image](https://github.com/user-attachments/assets/5b2f3e86-d280-48c5-ad41-ee58933920fc)

### Abrupt Returning code
If you intend to implement a code with multiple functions, but only one should be executed, that is, a code with multiple returns like this:
```c#
	   public Object TestMethod()
	   {
		if (toggle1.isEnabled && condition1)
		{
			return Object1();
		}
		if (toggle2.isEnabled && condition2)
		{
			return Object2();
		}
		return new DefaultObject();
	    }
```
**Simply using the FeatureToggle attribute will not work**.

However, you can use a variable to store an intended object resulting from a certain code path (like a response class) and the FeatureToggle custom attribute will know if a code path was already executed or not. To do this use the *ref* keyword in the method parameters and the ```[ArgumentBeholder]``` custom attribute to signal that this parameter should be observed by the FeatureToggle attribute. Here is an example:
```c#
	    public ResponseObject TestMethod()
	    {   
		ResponseObject result = null;

		Object1(ref result, condition1);
		Object2(ref result, condition2);

		if (result == null)
		{
			result = DoSomthingDefault();
		}

		return result;
	    }
	
	    [FeatureToggle("FT0001")]
	    public void Object1([ArgumentBeholder] ref ResponseObject result, bool condition)
	    {
		if (condition)
		{
			result = DoSomthing();
		}
	    }
	
	    [FeatureToggle("FT1000")]
	    public void Object2([ArgumentBeholder] ref ResponseObject result, bool condition)
	    {
		if (condition)
		{
			result = DoSomthingElse();
		}
	    }
```
This way the FeatureToggle custom attribute will only execute the intercepted method if result is still null, if not, the attribute will ignore the method and continue.
