

trivy 
https://aquasecurity.github.io/trivy/
--format json contains more information on misconfigurations etc
--format cyclonedx is more foused on 

dotnet restore src/OpaDemo.csproj
trivy fs --scanners vuln,secret,misconfig,license --format cyclonedx --output trivy/trivy-dotnet.cdx src/

trivy image --scanners vuln,secret,misconfig,license --format cyclonedx --output trivy/trivy-image.cdx mcr.microsoft.com/dotnet/aspnet:5.0.10-alpine3.14

trivy aws --region ap-southeast-2 --service s3 --format json --output trivy/trivy-aws.cdx

```text title=".trivyignore"
# CRITICAL: ruby-git: package vulnerable to Command Injection via git argument injection
CVE-2022-25648
# CRITICAL: Denial of Service and SQL Injection
CVE-2013-0269
```

gitleaks
https://gitleaks.io/

dockle
Container Image Linter for Security, Helping build the Best-Practice Docker Image, Easy to start
https://github.com/goodwithtech/dockle

dependencytrack
https://dependencytrack.org/

API Keys are attached to a team

```
curl -X "POST" "http://localhost:8081/api/v1/bom" \
     -H 'Content-Type: multipart/form-data' \
     -H "X-Api-Key: odt_5odAjNP3KHVjjM47qEa4mHwcNJoLUbKc" \
     -F "autoCreate=true" \
     -F "projectName=OpaDemo" \
     -F "projectVersion=1.0.1" \
     -F "bom=@trivy/trivy-dotnet.cdx"
```

```
curl -X "POST" "http://localhost:8081/api/v1/bom" \
     -H 'Content-Type: multipart/form-data' \
     -H "X-Api-Key: odt_5odAjNP3KHVjjM47qEa4mHwcNJoLUbKc" \
     -F "autoCreate=true" \
     -F "projectName=aspnet" \
     -F "projectVersion=5.0.10" \
     -F "bom=@trivy/trivy-image.cdx"
```

https://www.securecodebox.io/blog/2023/09/01/sbom-part-one-generation/


```xml title="Directory.build.props"
<Project>
 <PropertyGroup>
   <RestorePackagesWithLockFile>true</RestorePackagesWithLockFile>
 </PropertyGroup>
</Project>
```

