## Reference
- Getting started with writing and debugging AWS Lambda Function with Visual Studio Code



## Commands
```
  dotnew new console  # Create source code
  dotnet restore      # Restore all the dependencies into the project.
  dotnet publish      # Build
```


## Set debugging
Click debug icon on the left panel
Click settings icon and select ".Net core" as the debug environment
Click on the "play" button at the top debug bar and it will ask you to configure a task runner
Click on "Configure Task Runner" and select ".Net core" from the list to create tasks.json


## AWS Lambda dependencies 
```
  <ItemGroup>
    <PackageReference Include="Amazon.Lambda.Core" Version="1.0.0 " />
    <PackageReference Include="Amazon.Lambda.S3" Version="1.0.0 " />
    <PackageReference Include="Amazon.Lambda.Serialization.Json" Version="1.3.0" />
  </ItemGroup>

  <ItemGroup>
    <DotNetCliToolReference Include="Amazon.Lambda.Tools" Version="2.2.0" />
  </ItemGroup>
```


## Commands of AWS Lambda tool
```
  deploy-function               # deploy the project to AWS Lambda
        
  dotnet lambda package
```